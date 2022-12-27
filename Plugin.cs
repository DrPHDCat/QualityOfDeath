using BepInEx;
using HarmonyLib;
using System;
using UnityEngine;

namespace QualityOfDeath
{
	[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			base.Logger.LogInfo($"The mod {MyPluginInfo.PLUGIN_GUID} {MyPluginInfo.PLUGIN_VERSION} is loaded, have fun!");
			Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
			harmony.PatchAll();
		}
		private void Update()
		{
			
			if (MonoSingleton<OptionsManager>.Instance & MonoSingleton<NewMovement>.Instance)
			{
				if (!MonoSingleton<OptionsManager>.Instance.paused & MonoSingleton<NewMovement>.Instance.ridingRocket)
				{
					if (MonoSingleton<NewMovement>.Instance.antiHp < 75f)
					{
						MonoSingleton<NewMovement>.Instance.ForceAddAntiHP(1f, true, true, true);
					}
					if (MonoSingleton<NewMovement>.Instance.hp > 25)
					{
						MonoSingleton<NewMovement>.Instance.hp = MonoSingleton<NewMovement>.Instance.hp - 1;
						MonoSingleton<NewMovement>.Instance.FakeHurt(true);
					}
					this.RidingTicks = 0;
					this.RidingTicks++;
				}
			}
		}

		public int RidingTicks = 0;
	}
	[HarmonyPatch(typeof(SpiderBody), "Update")]
	static class MaliciousShootRocket
	{
		[HarmonyPostfix]
		private static void Postfix(ref GameObject ___player)
		{
			if (MonoSingleton<NewMovement>.Instance.ridingRocket && MonoSingleton<GrenadeList>.Instance.grenadeList.Count != 0)
			{
				for (int i = 0; i < MonoSingleton<GrenadeList>.Instance.grenadeList.Count; i++)
				{
					if (MonoSingleton<GrenadeList>.Instance.grenadeList[i].playerRiding)
					{
						___player = MonoSingleton<GrenadeList>.Instance.grenadeList[i].gameObject;
						break;
					}
					else
					{
						___player = MonoSingleton<NewMovement>.Instance.gameObject;
					}
				}
			}			
		}
        [HarmonyPrefix]
        private static void Prefix(ref GameObject ___player)
        {
			if (___player == null | !MonoSingleton<NewMovement>.Instance.ridingRocket)
            {
				___player = MonoSingleton<NewMovement>.Instance.gameObject;
            }
        }
	}
    [HarmonyPatch(typeof(Turret), "Update")]
    static class TurretShootGrenade
    {
        [HarmonyPostfix]
        public static void Postfix(ref Transform ___target)
        {
            for (int i = 0; i < MonoSingleton<GrenadeList>.Instance.grenadeList.Count; i++)
            {
				if (Vector3.Distance(MonoSingleton<GrenadeList>.Instance.grenadeList[i].transform.position, MonoSingleton<NewMovement>.Instance.gameObject.transform.position) < 12)
                {
					___target = MonoSingleton<GrenadeList>.Instance.grenadeList[i].transform;
					break;
				}
            }
        }
		[HarmonyPrefix]
        public static void Prefix(ref Transform ___target)
        {
			if (___target == null)
            {
				___target = MonoSingleton<NewMovement>.Instance.transform;
			}
			if (___target != null)
            {
				 if (Vector3.Distance(MonoSingleton<NewMovement>.Instance.transform.position, ___target.transform.position) > 12)
                {
					___target = MonoSingleton<NewMovement>.Instance.transform;
                }

			}
        }

	}
    [HarmonyPatch(typeof(SpiderBody), "Start")]
    static class Malicious2xSize
    {
        [HarmonyPostfix]
        private static void Postfix(SpiderBody __instance)
        {
			__instance.transform.localScale = new Vector3(__instance.transform.localScale.x * 2, __instance.transform.localScale.y * 2, __instance.transform.localScale.z * 2);
		}
    }
}