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

		private TNH_ScoreDisplay scoreDisplay;
		private Text lblGlobalScores;
		private GameObject gObjLoading;

		public void Start()
        {
			Debug.Log("--- Installed BepInEx Plugins ---");
			foreach (var plugin in Chainloader.PluginInfos)
            {
				Debug.Log(plugin.Key);
            }
			Debug.Log("--- End Plugins ---");
		}

		public void Init(TNH_ScoreDisplay tnhScore, Text scoreLabel, GameObject gObjLoading)
		{
			if (initialized)
				return;

			this.scoreDisplay = tnhScore;
			this.lblGlobalScores = scoreLabel;
			this.lblGlobalScores.resizeTextForBestFit = true;
			this.lblGlobalScores.horizontalOverflow = HorizontalWrapMode.Overflow;
			this.gObjLoading = gObjLoading;

			var loadedAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
			if (Array.Exists<Assembly>(loadedAssemblies, x => x.GetName().Name == "TakeAndHoldTweaker"))
			{
				tnhTweakerInstalled = true;
				this.gObjLoading.transform.GetChild(0).GetComponent<Text>().text = "<color=lightblue>Online leaderboards player count is incompatible with TNHTweaker.</color>";
				this.gObjLoading.SetActive(true);
			}

			initialized = true;
		}

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
				gObjLoading.SetActive(true);
				curID = null;
			}
		}
	}
}