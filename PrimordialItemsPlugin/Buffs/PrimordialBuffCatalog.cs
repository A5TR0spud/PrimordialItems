using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrimordialItemsPlugin.Buffs
{
    internal class PrimordialBuffCatalog
    {
        //NOTE: EulogyRegen and EulogyCooldown do almost nothing on their own. Most of the code is handled by the item.
            // EulogyRegen will heal when it's lost, but that's it
        public static BuffDef HealingEulogyRegen;
        public static BuffDef HealingEulogyCooldown;

        public static void initBuffs()
        {
            HealingEulogyRegen = HealingEulogyRegenBuff.init();
            ContentAddition.AddBuffDef(HealingEulogyRegen);

            HealingEulogyCooldown = HealingEulogyCooldownBuff.init();
            ContentAddition.AddBuffDef(HealingEulogyCooldown);
        }
    }
}
