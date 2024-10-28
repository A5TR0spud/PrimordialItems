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

namespace PrimordialItemsPlugin.Items
{
    internal class HealingEulogyBehavior : CharacterBody.ItemBehavior
    {
        private readonly static int regenDuration = 4;
        private readonly static float totalHealPercent = 1f;
        private readonly static float healingInterval = 0.25f;
        private readonly static float healthPerInterval = healingInterval * (totalHealPercent / regenDuration);
        private readonly static float intervalsPerSecond = 1.0f / healingInterval;
        //static int cooldownDuration = 2;
        private readonly static float regenerationHealthPerSecond = 0.1f;

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

            /*itemDef.tier = ItemTier.Boss;
#pragma warning disable CS0618 // Type or member is obsolete
            itemDef.deprecatedTier = ItemTier.Boss;
#pragma warning restore CS0618 // Type or member is obsolete*/

            itemDef._itemTierDef = PrimordialItemTier.PrimordialTier;

            itemDef.canRemove = false;
            itemDef.hidden = false;
            itemDef.isConsumed = false;

            itemDef.pickupIconSprite = LegacyResourcesAPI.LoadAsync<Sprite>("Textures/ItemIcons/texDominoIcon").WaitForCompletion();
            //"RoR2/Base/Mystery/PickupMystery.prefab"
            itemDef.pickupModelPrefab = LegacyResourcesAPI.LoadAsync<GameObject>("Prefabs/PickupModels/PickupDomino").WaitForCompletion();

            itemDef.AutoPopulateTokens();

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
                if (self.inventory.GetItemCount(PrimordialItemCatalog.HealingEulogy) > 0)
                {
                    Utilities.ShatterItem(self, RoR2Content.Items.Medkit.itemIndex);
                    Utilities.ShatterItem(self, RoR2Content.Items.HealWhileSafe.itemIndex);
                    Utilities.ShatterItem(self, PrimordialItemCatalog.HealingEulogy.itemIndex, true);
                }
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

                newCount = Mathf.Max(newCount, 0);
                int instancesToHeal = oldCount - newCount;
                HealthComponent healthComponent = self.healthComponent;

                if (!(bool)healthComponent)
                    return;
                if (instancesToHeal < 1)
                    return;

                float toHeal = instancesToHeal + instancesToHeal * healthPerInterval * self.healthComponent.fullHealth;
                self.healthComponent.Heal(toHeal, default(ProcChainMask), true);
            };
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

        private float stopwatch = 0;

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            if (base.body.outOfDanger)
            {
                float toHeal = delta * base.body.healthComponent.fullHealth * regenerationHealthPerSecond;
                base.body.healthComponent.Heal(toHeal, default(ProcChainMask), false);
            }

            if (!base.body.HasBuff(PrimordialBuffCatalog.HealingEulogyRegen))
            {
                stopwatch = 0;
                return;
            }

            stopwatch += delta;

            if (stopwatch >= healingInterval)
            {
                stopwatch -= healingInterval;
                base.body.RemoveOldestTimedBuff(PrimordialBuffCatalog.HealingEulogyRegen);
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
                int most = Mathf.CeilToInt(regenDuration * intervalsPerSecond);
                for (int i = 0; i < most; i++)
                {
                    body.AddTimedBuff(regen, regenDuration, most);
                }
            }
        }
    }
}
