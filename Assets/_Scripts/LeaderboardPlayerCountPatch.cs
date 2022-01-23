using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace TNHQoLImprovements
{
    [HarmonyPatch]
    public class LeaderboardPlayerCountPatch
    {
        Harmony harmony;
        private static GameObject gObjLoading;
        private static Text uiGlobalText;

        public LeaderboardPlayerCountPatch()
        {
            harmony = new Harmony("me.muskit.TNHQualityOfLifeImprovements.LeaderboardPlayerCount");
            harmony.PatchAll();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TNH_ScoreDisplay), "Start")]
        public static void Setup(TNH_ScoreDisplay __instance)
        {
            GameObject gObjLeaderboard = null;
            gObjLoading = GameObject.Instantiate(MeatKitPlugin.bundle.LoadAsset<GameObject>("PlayerCount_LoadingText"));
            if (__instance.gameObject.name == "_MainMenu") // lobby
            {
                gObjLeaderboard = __instance.transform.GetChild(3).gameObject; // MainMenuCanvas_RightFar
                gObjLoading.transform.SetParent(gObjLeaderboard.transform, false);
                gObjLoading.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                gObjLoading.transform.localPosition = new Vector3(520, 380, 0);
                gObjLoading.transform.GetChild(0).GetComponent<Text>().alignment = TextAnchor.UpperLeft;
            }
            if (__instance.gameObject.name == "_FinalScoreDisplay(Clone)") // game over room
            {
                gObjLeaderboard = __instance.transform.GetChild(0).gameObject; // ScoreCanvas_PastScores
                gObjLoading.transform.SetParent(gObjLeaderboard.transform, false);
                gObjLoading.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
                gObjLoading.transform.localPosition = new Vector3(-520, 380, 0);
                gObjLoading.transform.GetChild(0).GetComponent<Text>().alignment = TextAnchor.UpperRight;
            }
            uiGlobalText = gObjLeaderboard.transform.GetChild(0).GetChild(2).GetComponent<Text>();
            gObjLoading.transform.rotation = gObjLeaderboard.transform.rotation;
            gObjLoading.transform.GetChild(0).GetComponent<Text>().font = uiGlobalText.font;

            var playerCountComponent = gObjLeaderboard.AddComponent<LeaderboardPlayerCount>();
            playerCountComponent.Init(__instance, uiGlobalText, gObjLoading);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TNH_ScoreDisplay), "SwitchToModeID")]
        public static void OnModeIDSwitch_Post(string id, TNH_ScoreDisplay __instance)
        {
            Debug.Log(string.Format("Changing scoreboard mode to {0}!", id));
        }
    }
}