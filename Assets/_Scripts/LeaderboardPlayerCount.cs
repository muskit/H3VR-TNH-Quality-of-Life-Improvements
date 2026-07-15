using System;
using System.Reflection;
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

		private TNH_ScoreDisplay scoreDisplay;
		private Text lblGlobalScores;

        #region INITIALIZATION
		public void Init(TNH_ScoreDisplay tnhScore, Text scoreLabel)
		{
			if (initialized)
				return;

            // don't run with TNHTweaker installed
            var loadedAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            if (Array.Exists<Assembly>(loadedAssemblies, x => x.GetName().Name == "TakeAndHoldTweaker"))
            {
                tnhTweakerInstalled = true;
                return;
            }

            this.scoreDisplay = tnhScore;
			this.lblGlobalScores = scoreLabel;
			this.lblGlobalScores.resizeTextForBestFit = true;
			this.lblGlobalScores.horizontalOverflow = HorizontalWrapMode.Overflow;

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
            }
            catch (KeyNotFoundException e)
            {
                lblGlobalScores.text = "Global Scores:";
                curID = null;
            }
            catch (Exception e)
            {
                MeatKitPlugin.Logger.LogWarning("Unknown error occurred trying to retrieve online player count.");
                MeatKitPlugin.Logger.LogWarning(e);
			}
        }
    }
	#endregion
}