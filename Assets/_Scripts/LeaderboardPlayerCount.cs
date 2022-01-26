using System;
using System.Reflection;
using BepInEx.Bootstrap;
using UnityEngine;
using UnityEngine.UI;
using FistVR;
using RUST.Steamworks;
using System.Collections.Generic;

namespace TNHQoLImprovements
{
	public class LeaderboardPlayerCount : MonoBehaviour
	{
		private bool initialized = false;
		private bool tnhTweakerInstalled = false;

		private string curID;
		private string loadingStr;

		private TNH_ScoreDisplay scoreDisplay;
		private Text lblGlobalScores;
		private GameObject gObjLoading;

        #region INITIALIZATION
		public void Init(TNH_ScoreDisplay tnhScore, Text scoreLabel, GameObject gObjLoading)
		{
			if (initialized)
				return;

			this.scoreDisplay = tnhScore;
			this.lblGlobalScores = scoreLabel;
			this.lblGlobalScores.resizeTextForBestFit = true;
			this.lblGlobalScores.horizontalOverflow = HorizontalWrapMode.Overflow;
			this.gObjLoading = gObjLoading;
			loadingStr = gObjLoading.GetComponentInChildren<Text>().text;

			var loadedAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
			if (Array.Exists<Assembly>(loadedAssemblies, x => x.GetName().Name == "TakeAndHoldTweaker"))
			{
				tnhTweakerInstalled = true;
				this.gObjLoading.transform.GetChild(0).GetComponent<Text>().text = "<color=lightblue><size=30>Online player count is incompatible with TNHTweaker.</size></color>";
				this.gObjLoading.SetActive(true);
			}

			initialized = true;
		}
        #endregion

        #region UPDATE
        private void Update()
		{
			if (!initialized || tnhTweakerInstalled)
				return;

			string newID = scoreDisplay.m_curSequenceID;
			if (newID != curID)
				UpdatePlayerCountDisplay(newID);
		}

		private void UpdatePlayerCountDisplay(string id)
        {
            try
            {
                string playerCountText = Steamworks.SteamUserStats
                    .GetLeaderboardEntryCount(HighScoreManager.Leaderboards[id])
                    .ToString("N0");

                lblGlobalScores.text = "Global Scores: <color=lightblue>(" + playerCountText + " players)</color>";
                curID = id;
				gObjLoading.SetActive(false);
            }
            catch (KeyNotFoundException e)
            {
                lblGlobalScores.text = "Global Scores:";
				gObjLoading.GetComponentInChildren<Text>().text = loadingStr;
				gObjLoading.SetActive(true);
                curID = null;
            }
            catch (Exception e)
            {
                gObjLoading.GetComponentInChildren<Text>().text = string.Format("<color=lightblue><size=30>Unknown error occured trying to retrieve online player count.</size></color>\n\n" +
					"<color=red>{0}</color>", e);
                gObjLoading.SetActive(true);
			}
        }
    }
	#endregion
}