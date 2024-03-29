﻿using System.Reflection;
using HarmonyLib;
using UnityEngine;
using FistVR;

namespace TNHQoLImprovements
{
    public enum HealthExpireIndicationType
    {
        None, Flashing, CircularGraphic
    }

    /// <summary>
    /// If KillAfter is attached to a HealthCrystal, show visual representation of expiration.
    /// </summary>
    public static class TimedHealthCrystalPatch
    {
        private static GameObject timerAsset;
        public const int VISUAL_APPROACH = 2;

        public static void Patch(Harmony harmony)
        {
            timerAsset = MeatKitPlugin.bundle.LoadAsset<GameObject>("HealthCrystalTimer");

            var original = typeof(KillAfter).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
            var newfunc = typeof(TimedHealthCrystalPatch).GetMethod("Setup");

            harmony.Patch(original, postfix: new HarmonyMethod(newfunc));
        }

        public static void Setup(KillAfter __instance)
        {
            // only work with Health Crystals
            if (__instance.transform.GetComponentInChildren<HealthPickUp>() == null)
                return;

            GameObject timer;
            Transform healthCrystal = __instance.transform.Find("HealthCrystal");
            switch (MeatKitPlugin.cfgHealthCrystalIndicator.Value)
            {
                case HealthExpireIndicationType.CircularGraphic: // ring above
                    timer = GameObject.Instantiate<GameObject>(timerAsset, healthCrystal);
                    timer.GetComponent<UIRingTimer>().Init(__instance.DieTime);
                    timer.transform.localScale = new Vector2(0.001f, 0.001f);
                    timer.transform.localPosition = new Vector3(0, .9f, 0);
                    // TODO: disable scoring?
                    break;
                case HealthExpireIndicationType.Flashing: // flashing crystal
                    var flicker = healthCrystal.gameObject.AddComponent<MeshRendererFlicker>();
                    flicker.Init(.6f, .7f, __instance.DieTime - 3);
                    break;
            }
        }
    }
}