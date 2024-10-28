using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PrimordialItemsPlugin.Buffs
{
    internal class HealingEulogyCooldownBuff
    {
        public static BuffDef init()
        {
            BuffDef buff = ScriptableObject.CreateInstance<BuffDef>();

            buff.isDebuff = false;
            buff.isCooldown = true;
            buff.canStack = false;
            buff.ignoreGrowthNectar = true;
            buff.isHidden = false;

            buff.iconSprite = PrimPlugin.primordialAssetBundle.LoadAsset<Sprite>("texHealingEulogyCooldown");

            buff.buffColor = new Color(0.40138838f, 0.41509432f, 0.40138838f, 1f);
            buff.name = "primordialHealingEulogyCooldown";

            return buff;
        }
    }
}
