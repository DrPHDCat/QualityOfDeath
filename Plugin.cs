using BepInEx;
using HarmonyLib;
using System;

namespace Quality_Of_Death
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public int RidingTicks = 0;
        private void Awake()
        {
            // Plugin startup logic

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
        private void Update()
        {
            if (!OptionsManager.Instance.paused)
            { 
                if (NewMovement.Instance.ridingRocket)
                {
                    if (NewMovement.Instance.hp > 25)
                    {
                        RidingTicks = RidingTicks + 1;
                        if (RidingTicks >= 4)
                        {
                            NewMovement.Instance.hp = NewMovement.Instance.hp - 1;
                            RidingTicks = 0;
                        }
                    }
                }
            }
        }
    }
}