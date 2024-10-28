using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PrimordialItemsPlugin.Buffs
{
    internal class HealingEulogyRegenBuff
    {
        //The behaviour for this buff is defined in HealingEulogyBehavior
        //It heals when removed.
        public static BuffDef init()
        {
            BuffDef buff = ScriptableObject.CreateInstance<BuffDef>();

            buff.isDebuff = false;
            buff.isCooldown = false;
            buff.canStack = true;
            buff.ignoreGrowthNectar = false;
            buff.isHidden = false;

            buff.iconSprite = PrimPlugin.primordialAssetBundle.LoadAsset<Sprite>("texHealingEulogyRegen");

            buff.buffColor = new Color(0.78431374f, 0.9372549f, 0.42745098f, 1f);
            buff.name = "primordialHealingEulogyRegen";

            return buff;
        }
    }
}
