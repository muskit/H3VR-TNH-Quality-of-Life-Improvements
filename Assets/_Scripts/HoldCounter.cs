using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace TNHQoLImprovements
{
	public class HoldCounter : MonoBehaviour
	{
		private Text lblHoldCount;
		private Text lblWinLose;

		public static int[] winLose = { -1, 1 };
		public const string WIN_LOSE_TEXT = "<color=#10ff10>{0}</color>              <color=red>{1}</color>";


		private void OnDeath(bool _)
        {
			Debug.Log("I died!");
			// TODO: bind stats to controller hand
        }

		// TODO: win/lose counter. patch postfix FistVR.TNH_Manager.HoldPointCompleted
		public static void Patch(Harmony harmony)
        {
			var original = typeof(TNH_Manager).GetMethod("HoldPointCompleted", BindingFlags.Public | BindingFlags.Instance);
			var patch = typeof(HoldCounter).GetMethod("OnHoldEnd");
			Debug.Log(string.Format("Original: {0} // Patch: {1}", original, patch));
			harmony.Patch(original, postfix: new HarmonyMethod(patch));
		}
		public static void OnHoldEnd(TNH_HoldPoint p, bool success)
        {
			if (success)
				winLose[0]++;
			else
				winLose[1]++;
        }

		void Start()
		{
			transform.localPosition = new Vector3(-1f, 0, -.5f);
			transform.localRotation = Quaternion.Euler(90, 0, 0);
			transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);

			lblHoldCount = transform.GetChild(1).GetComponent<Text>();
			lblWinLose = transform.GetChild(2).GetComponent<Text>();

			winLose[0]  = 0;
			winLose[1]  = 0;

			FindObjectOfType<FVRSceneSettings>().PlayerDeathEvent += OnDeath;
		}

		void Update()
		{
			// Total hold count
			string display = "";
			if (InPlay.tnhManager.ProgressionMode == TNHSetting_ProgressionType.Marathon)
				display = InPlay.tnhManager.m_level.ToString() + " / ∞";
			else
				display = string.Format("{0} / {1}", InPlay.tnhManager.m_level, InPlay.tnhManager.m_maxLevels);
			lblHoldCount.text = display;

			// Win/Lost holds
			lblWinLose.text = string.Format(WIN_LOSE_TEXT, winLose[0], winLose[1]);
		}
	}
}