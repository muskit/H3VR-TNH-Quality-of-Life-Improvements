using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace TNHQoLImprovements
{
    public static class LeaderboardPlayerCountPatch
    {
        private static Text uiGlobalText;

        public static void Patch(Harmony harmony)
        {
            var original = typeof(TNH_ScoreDisplay).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
            var newfunc = typeof(LeaderboardPlayerCountPatch).GetMethod("Setup");

            harmony.Patch(original, postfix: new HarmonyMethod(newfunc));
        }

        public static void Setup(TNH_ScoreDisplay __instance)
        {
            GameObject gObjLeaderboard = null;
            if (__instance.gameObject.name == "_MainMenu") // lobby
            {
                gObjLeaderboard = __instance.transform.GetChild(3).gameObject; // MainMenuCanvas_RightFar
            }
            if (__instance.gameObject.name == "_FinalScoreDisplay(Clone)") // game over room
            {
                gObjLeaderboard = __instance.transform.GetChild(0).gameObject; // ScoreCanvas_PastScores
            }
            uiGlobalText = gObjLeaderboard.transform.GetChild(0).GetChild(2).GetComponent<Text>();

            var playerCountComponent = gObjLeaderboard.AddComponent<LeaderboardPlayerCount>();
            playerCountComponent.Init(__instance, uiGlobalText);
        }
    }
}