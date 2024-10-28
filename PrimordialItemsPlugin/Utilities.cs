using PrimordialItemsPlugin.Items;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrimordialItemsPlugin
{
    internal class Utilities
    {
        public static void ShatterItem(CharacterBody body, ItemIndex itemIndex, bool allButOne = false)
        {
            Inventory inventory = body.inventory;
            if (!(bool)inventory)
                return;
            int count = inventory.GetItemCount(itemIndex);
            int threshold = allButOne ? 1 : 0;

            ItemIndex shatteredIndex = PrimordialItemCatalog.ShatteredRemains.itemIndex;

            if (count > threshold)
            {
                int toConvert = count - threshold;
                inventory.RemoveItem(itemIndex, toConvert);
                inventory.GiveItem(shatteredIndex, toConvert);
                CharacterMasterNotificationQueue.SendTransformNotification(body.master, itemIndex, shatteredIndex, CharacterMasterNotificationQueue.TransformationType.LunarSun);
            }
        }
    }
}
