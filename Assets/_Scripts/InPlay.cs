using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace TNHQoLImprovements
{
	/// <summary>
	/// Code here should only be run when in TNH play.
	/// </summary>
	public class InPlay : MonoBehaviour
	{
		public static TNH_Manager tnhManager;

		private static Transform[] hands;
		private static GameObject tnhInfo;

		public static void Patch(Harmony harmony)
        {
			var original = typeof(TNH_Manager).GetMethod("SetPhase", BindingFlags.NonPublic | BindingFlags.Instance);
			var patch = typeof(InPlay).GetMethod("MoveStatsToController", BindingFlags.NonPublic | BindingFlags.Static);
			harmony.Patch(original, postfix: new HarmonyMethod(patch));
        }

		private static void MoveStatsToController(TNH_Phase p)
        {
			if (tnhManager == null)
				return;

            if (p == TNH_Phase.Dead || p == TNH_Phase.Completed)
            {
				int handSide = tnhManager.RadarHand == TNH_RadarHand.Left ? 0 : 1;

				tnhInfo.transform.SetParent(hands[handSide], false);
				tnhInfo.transform.localScale = new Vector3(.0002f, .0002f, .0002f);
				tnhInfo.GetComponent<TNHInfo>().GameOverPos();
			}
		}

		// Use this for initialization
		void Start()
		{
			tnhManager = GameObject.Find("_GameManager").GetComponent<TNH_Manager>();

			var rig = Object.FindObjectOfType<FVRMovementManager>().transform;
			hands = new Transform[] {
				rig.transform.GetChild(1), rig.transform.GetChild(0)
			};

			tnhInfo = Instantiate<GameObject>(MeatKitPlugin.bundle.LoadAsset<GameObject>("TNHInfo"), FindObjectOfType<TAH_Reticle>().transform.GetChild(3));
			tnhInfo.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
		}
	}
}