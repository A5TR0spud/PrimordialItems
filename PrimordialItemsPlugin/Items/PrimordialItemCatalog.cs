using HG;
using PrimordialItemsPlugin.ItemTiers;
using R2API;
using RoR2;
using RoR2.Items;
using RoR2.UI.LogBook;
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
        public static CustomItem HealingEulogyItem;

        public static void initItems()
        {
            PrimordialItemTier.init();

            HealingEulogy = HealingEulogyBehavior.init();
            //ContentAddition.AddItemDef(HealingEulogy);
            HealingEulogyItem = new CustomItem(HealingEulogy, (ItemDisplayRule[])null);
            ItemAPI.Add(HealingEulogyItem);
            HealingEulogyBehavior.hook();

            hook();
        }
        
        public static void hook()
        {
            //Item Displays and Adding Content
            /*On.RoR2.ItemCatalog.Init += (orig) =>
            {
                
                orig();
            };*/

            //Transformations
            On.RoR2.ItemCatalog.GetItemPairsForRelationship += (orig, relationshipType) =>
            {
                ReadOnlyArray<ItemDef.Pair> yah = orig(relationshipType);
                List<ItemDef.Pair> arr = new List<ItemDef.Pair>();
                arr = new List<ItemDef.Pair>(yah.src);

                //Prayer to [Him]
                ItemDef.Pair slugToEulogy = new ItemDef.Pair();
                slugToEulogy.itemDef1 = RoR2Content.Items.HealWhileSafe;
                slugToEulogy.itemDef2 = HealingEulogy;
                arr.Add(slugToEulogy);

                ItemDef.Pair medkitToEulogy = new ItemDef.Pair();
                medkitToEulogy.itemDef1 = RoR2Content.Items.Medkit;
                medkitToEulogy.itemDef2 = HealingEulogy;
                arr.Add(medkitToEulogy);

                ReadOnlyArray<ItemDef.Pair> toReturn = new ReadOnlyArray<ItemDef.Pair>(arr.ToArray());
                
                return toReturn;
            };

            
        }
    }
}
