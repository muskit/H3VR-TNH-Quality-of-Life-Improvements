using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace TNHQoLImprovements
{
	public static class HoldCounterPatch
    {
		public static void Patch(Harmony harmony)
		{
			var original = typeof(TNH_Manager).GetMethod("HoldPointCompleted", BindingFlags.Public | BindingFlags.Instance);
			var patch = typeof(HoldCounter).GetMethod("OnHoldEnd");
			harmony.Patch(original, postfix: new HarmonyMethod(patch));
		}
	}

    public class HoldCounter : MonoBehaviour
	{
		private Text lblHoldCount;
		private Text lblWinLose;

		public static int[] winLose = { -1, 1 };
		public const string WIN_LOSE_TEXT = "<color=#10ff10>{0}</color>              <color=red>{1}</color>";

		public static void OnHoldEnd(TNH_HoldPoint p, bool success)
        {
			if (success)
				winLose[0]++;
			else
				winLose[1]++;
        }

		void Start()
		{
			transform.localPosition = new Vector3(-333, 0, -450);

			lblHoldCount = transform.GetChild(1).GetComponent<Text>();
			lblWinLose = transform.GetChild(2).GetComponent<Text>();

			winLose[0]  = 0;
			winLose[1]  = 0;
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