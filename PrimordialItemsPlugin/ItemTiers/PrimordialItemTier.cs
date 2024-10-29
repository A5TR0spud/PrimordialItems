using R2API;
using RoR2;
using RoR2.UI.LogBook;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PrimordialItemsPlugin.ItemTiers
{
    internal class PrimordialItemTier
    {
        public static UnityEngine.Color color = new UnityEngine.Color(0.694f, 1.0f, 1.0f);
        public static UnityEngine.Color darkColor = new UnityEngine.Color(0.518f, 0.8f, 0.796f);
        public static UnityEngine.Color trailColor = new UnityEngine.Color(0.376f, 0.71f, 0.588f);
        public static UnityEngine.Color trailDarkColor = new UnityEngine.Color(0.341f, 0.576f, 0.529f);
        public static ColorCatalog.ColorIndex primordialColorIndex;
        public static ColorCatalog.ColorIndex primordialDarkColorIndex;
        public static ItemTierDef PrimordialTier;

        public static void init()
        {
            primordialColorIndex = ColorsAPI.RegisterColor(color);
            primordialDarkColorIndex = ColorsAPI.RegisterColor(darkColor);

            PrimordialTier = ScriptableObject.CreateInstance<ItemTierDef>();
            PrimordialTier.canScrap = false;
            PrimordialTier.isDroppable = false;
            
            PrimordialTier.colorIndex = primordialColorIndex;
            PrimordialTier.darkColorIndex = primordialDarkColorIndex;

            PrimordialTier.pickupRules = ItemTierDef.PickupRules.ConfirmFirst;
            PrimordialTier.canRestack = false;
            PrimordialTier._tier = ItemTier.AssignedAtRuntime;
            PrimordialTier.name = "Primordial";

            ContentAddition.AddItemTierDef(PrimordialTier);

            PrimordialTier.bgIconTexture = PrimPlugin.primordialAssetBundle.LoadAsset<Sprite>("texPrimordialBGIcon").texture;

            //var highlightPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/HighlightTier3Item.prefab").WaitForCompletion();
            var dropletPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/EquipmentOrb.prefab").WaitForCompletion();

            //PrimordialTier.highlightPrefab = highlightPrefab;
            PrimordialTier.dropletDisplayPrefab = CreateDroplet();
        }

        //function CreateDroplet() stolen from gemo (noodlegemo) (elementGEMO) with some modifications
        //https://github.com/elementGEMO/LunarsOfExiguity/blob/master/VisualStudio/Content/ItemTiers/PurifiedTier.cs
        private static GameObject CreateDroplet()
        {
            GameObject orbDrop = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/LunarOrb.prefab").WaitForCompletion().InstantiateClone("PrimordialOrb", true);
            Color nearbyColor = trailColor;
            Color trailingOffColor = trailDarkColor;

            var trail = orbDrop.GetComponentInChildren<TrailRenderer>();
            if (trail)
            {
                trail.startColor = nearbyColor;
                trail.set_startColor_Injected(ref nearbyColor);
                trail.endColor = trailingOffColor;
                trail.set_endColor_Injected(ref trailingOffColor);
            }

            foreach (ParticleSystem particle in orbDrop.GetComponentsInChildren<ParticleSystem>())
            {
                var main = particle.main;
                var colorLifetime = particle.colorOverLifetime;

                main.startColor = new ParticleSystem.MinMaxGradient(nearbyColor);
                colorLifetime.color = nearbyColor;
            }

            var light = orbDrop.GetComponentInChildren<Light>();
            if (light)
            {
                light.color = nearbyColor;
                light.intensity = 20;
                light.range = 3.5f;
            }

            return orbDrop;
        }
    }
}
