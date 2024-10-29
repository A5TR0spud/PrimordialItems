using PrimordialItemsPlugin.Items;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrimordialItemsPlugin
{
    internal class Utilities
    {
        /*public static void GreatOldItem(CharacterBody body, ItemIndex victim, ItemIndex oPrimordialOne)
        {
            Inventory inventory = body.inventory;
            if (!(bool)inventory)
                return;
            int count = inventory.GetItemCount(victim);

            if (count > 0)
            {
                int toConvert = count;
                inventory.RemoveItem(victim, toConvert);
                inventory.GiveItem(oPrimordialOne, toConvert);
                CharacterMasterNotificationQueue.SendTransformNotification(body.master, victim, oPrimordialOne, CharacterMasterNotificationQueue.TransformationType.LunarSun);
            }
        }*/
    }
}
