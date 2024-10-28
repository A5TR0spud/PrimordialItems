using PrimordialItemsPlugin.ItemTiers;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace PrimordialItemsPlugin.Items
{
    internal class PrimordialItemCatalog
    {
        public static ItemDef HealingEulogy;
        public static ItemDef ShatteredRemains;

        public static void initItems()
        {
            PrimordialItemTier.init();

            HealingEulogy = HealingEulogyBehavior.init();
            ContentAddition.AddItemDef(HealingEulogy);
            HealingEulogyBehavior.hook();

            ShatteredRemains = ShatteredRemainsDefinition.init();
            ContentAddition.AddItemDef(ShatteredRemains);
        }
    }
}
