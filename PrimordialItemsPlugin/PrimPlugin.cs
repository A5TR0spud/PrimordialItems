using BepInEx;
using PrimordialItemsPlugin.Buffs;
using PrimordialItemsPlugin.Items;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PrimordialItemsPlugin
{
    [BepInDependency(ItemAPI.PluginGUID)]

    [BepInDependency(LanguageAPI.PluginGUID)]

    [BepInDependency(ColorsAPI.PluginGUID)]

    [BepInDependency(PrefabAPI.PluginGUID)]

    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class PrimPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "A5TR0spud";
        public const string PluginName = "PrimordialItems";
        public const string PluginVersion = "0.1.0";

        public static AssetBundle primordialAssetBundle;

        public void Awake()
        {
            Log.Init(Logger);

            primordialAssetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Info.Location), "AssetBundles", "primordialassets"));

            PrimordialBuffCatalog.initBuffs();

            PrimordialItemCatalog.initItems();
        }

        private void Update()
        {
            // This if statement checks if the player has currently pressed F2.
            if (Input.GetKeyDown(KeyCode.F2))
            {
                // Get the player body to use a position:
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                // And then drop our defined item in front of the player.

                Log.Info($"Player pressed F2. Spawning our custom item at coordinates {transform.position}");
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(PrimordialItemCatalog.HealingEulogy.itemIndex), transform.position, transform.forward * 20f);
            }
        }
    }
}
