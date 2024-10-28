using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PrimordialItemsPlugin.Items
{
    internal class ShatteredRemainsDefinition
    {
        public static ItemDef init()
        {
            ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();

            itemDef.name = "PRIMORDIALShatteredRemains";
            itemDef.nameToken = "PRIMORDIAL_SHATTEREDREMAINS_NAME";
            itemDef.pickupToken = "PRIMORDIAL_SHATTEREDREMAINS_PICKUP";
            itemDef.descriptionToken = "PRIMORDIAL_SHATTEREDREMAINS_DESC";
            itemDef.loreToken = "PRIMORDIAL_SHATTEREDREMAINS_LORE";

            itemDef.tags = [
                ItemTag.CannotDuplicate,
                ItemTag.WorldUnique,
                ItemTag.RebirthBlacklist
            ];

            itemDef.tier = ItemTier.NoTier;
#pragma warning disable CS0618 // Type or member is obsolete
            itemDef.deprecatedTier = ItemTier.NoTier;
#pragma warning restore CS0618 // Type or member is obsolete

            itemDef.canRemove = false;
            itemDef.hidden = false;
            itemDef.isConsumed = true;

            itemDef.pickupIconSprite = LegacyResourcesAPI.LoadAsync<Sprite>("Textures/ItemIcons/texScrapYellowIcon").WaitForCompletion();
            itemDef.pickupModelPrefab = LegacyResourcesAPI.LoadAsync<GameObject>("Prefabs/PickupModels/PickupScrap").WaitForCompletion();

            return itemDef;
        }
    }
}
