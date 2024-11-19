using RoR2.Items;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2;
using PrimordialItemsPlugin.Buffs;
using R2API;
using UnityEngine.Networking;
using RoR2.Projectile;
using PrimordialItemsPlugin.ItemTiers;
using RoR2.UI.LogBook;
using RoR2.ExpansionManagement;

namespace PrimordialItemsPlugin.Items
{
    internal class HealingEulogyBehavior : CharacterBody.ItemBehavior
    {
        private readonly static int regenDuration = 4;
        private readonly static float totalHealPercentInitial = 0.1f;
        private readonly static float totalHealPercentStacking = 0.1f;
        private readonly static float healingInterval = 0.25f;
        //static int cooldownDuration = 2;
        private readonly static float slugInitial = 0.01f;
        private readonly static float slugStacking = 0.01f;

        public static ItemDef init()
        {
            ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();

            itemDef.name = "PRIMORDIALHealingEulogy";
            itemDef.nameToken = "PRIMORDIAL_HEALINGEULOGY_NAME";
            itemDef.pickupToken = "PRIMORDIAL_HEALINGEULOGY_PICKUP";
            itemDef.descriptionToken = "PRIMORDIAL_HEALINGEULOGY_DESC";
            itemDef.loreToken = "PRIMORDIAL_HEALINGEULOGY_LORE";

            itemDef.tags = [
                ItemTag.Healing,
                ItemTag.CannotDuplicate,
                ItemTag.WorldUnique
            ];

            itemDef._itemTierDef = PrimordialItemTier.PrimordialTier;
#pragma warning disable CS0618 // Type or member is obsolete
            itemDef.deprecatedTier = ItemTier.AssignedAtRuntime;
#pragma warning restore CS0618 // Type or member is obsolete*/

            itemDef.canRemove = false;
            itemDef.hidden = false;
            itemDef.isConsumed = false;

            itemDef.pickupIconSprite = LegacyResourcesAPI.LoadAsync<Sprite>("Textures/ItemIcons/texDominoIcon").WaitForCompletion();
            //"RoR2/Base/Mystery/PickupMystery.prefab"
            itemDef.pickupModelPrefab = LegacyResourcesAPI.LoadAsync<GameObject>("Prefabs/PickupModels/PickupDomino").WaitForCompletion();

            itemDef.hideFlags = HideFlags.None;
            //itemDef.

            return itemDef;
        }

        public static void hook()
        {
            On.RoR2.HealthComponent.TakeDamageProcess += (orig, self, damageInfo) =>
            {
                orig(self, damageInfo);

                if (!self.body || !self.body.inventory || self.body.inventory.GetItemCount(PrimordialItemCatalog.HealingEulogy) < 1)
                {
                    return;
                }

                if (!damageInfo.rejected && damageInfo.damage > 0)
                {
                    apply(null, self);
                }
            };

            On.RoR2.CharacterBody.OnInventoryChanged += (orig, self) =>
            {
                if (NetworkServer.active)
                {
                    self.AddItemBehavior<HealingEulogyBehavior>(self.inventory.GetItemCount(PrimordialItemCatalog.HealingEulogy));
                }
                /*if (self.inventory.GetItemCount(PrimordialItemCatalog.HealingEulogy) > 0)
                {
                    Utilities.GreatOldItem(self, RoR2Content.Items.Medkit.itemIndex, PrimordialItemCatalog.HealingEulogy.itemIndex);
                    Utilities.GreatOldItem(self, RoR2Content.Items.HealWhileSafe.itemIndex, PrimordialItemCatalog.HealingEulogy.itemIndex);
                }*/
                orig(self);
            };

            //I know this *should* be in HealingEulogyRegenBuff, but at this point I can't be bothered.
            On.RoR2.CharacterBody.SetBuffCount += (orig, self, buffType, newCount) =>
            {
                int oldCount = self.GetBuffCount(buffType);
                orig(self, buffType, newCount);
                if (!NetworkServer.active)
                    return;
                if (buffType != PrimordialBuffCatalog.HealingEulogyRegen.buffIndex)
                    return;

                Inventory inventory = self.inventory;

                if (!(bool)inventory)
                    return;

                newCount = Mathf.Max(newCount, 0);
                int instancesToHeal = oldCount - newCount;
                HealthComponent healthComponent = self.healthComponent;

                if (!(bool)healthComponent)
                    return;
                if (instancesToHeal < 1)
                    return;

                float toHeal = instancesToHeal + (instancesToHeal * HealthPerInterval(inventory) * self.healthComponent.fullHealth);
                self.healthComponent.Heal(toHeal, default(ProcChainMask), true);
            };
        }

        public static float HealthPerInterval(Inventory inventory)
        {
            int eulogyCount = inventory.GetItemCount(PrimordialItemCatalog.HealingEulogy) - 1;
            float totalHealPercent = totalHealPercentInitial + (eulogyCount * totalHealPercentStacking);
            return healingInterval * (totalHealPercent / regenDuration);
        }

        private void Awake()
        {
            //base.enabled = false;
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            if (base.body.outOfDanger)
            {
                int stack = base.body.inventory.GetItemCount(PrimordialItemCatalog.HealingEulogy) - 1;
                float slugCoeff = slugInitial + (stack * slugStacking);
                float toHeal = delta * base.body.healthComponent.fullHealth * slugCoeff;
                base.body.healthComponent.Heal(toHeal, default(ProcChainMask), false);
            }
        }

        private static void apply(CharacterBody body, HealthComponent healthComponent)
        {
            if (body == null && (bool)healthComponent)
            {
                body = healthComponent.body;
            }
            if (healthComponent == null && (bool)body)
            {
                healthComponent = body.healthComponent;
            }
            if (!(bool)healthComponent || !(bool)body)
            {
                return;
            }

            BuffDef regen = PrimordialBuffCatalog.HealingEulogyRegen;
            BuffDef cooldown = PrimordialBuffCatalog.HealingEulogyCooldown;
            
            if (body.HasBuff(regen))
            {
                List<CharacterBody.TimedBuff> timedBuffs = body.timedBuffs;
                float timeLeft = 0f;
                foreach (CharacterBody.TimedBuff buff in timedBuffs)
                {
                    if (buff.buffIndex == PrimordialBuffCatalog.HealingEulogyRegen.buffIndex)
                    {
                        timeLeft = buff.timer;
                    }
                }
                body.AddTimedBuff(cooldown, timeLeft, 1);
                body.SetBuffCount(regen.buffIndex, 0);
                return;
            }
            if (!body.HasBuff(cooldown))
            {
                int most = Mathf.CeilToInt(regenDuration * (1.0f / healingInterval));
                for (int i = 0; i < most; i++)
                {
                    body.AddTimedBuff(regen, healingInterval * (i + 1), most);
                }
            }
        }
    }
}
