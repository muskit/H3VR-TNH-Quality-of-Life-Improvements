using HarmonyLib;
using System.Reflection;
using UnityEngine;
using FistVR;

namespace TNHQoLImprovements
{
	/// <summary>
	/// Code here should only be run when in TNH play.
	/// </summary>
	public class InPlay : MonoBehaviour
	{
		public static TNH_Manager tnhManager;

        public static bool InHold()
        {
            if (tnhManager == null)
                return false;

            return tnhManager.Phase == TNH_Phase.Hold;
        }

		void Start()
		{
			tnhManager = FindObjectOfType<TNH_Manager>();

			TNHInfo.instance = Instantiate<GameObject>(MeatKitPlugin.bundle.LoadAsset<GameObject>("TNHInfo"),
				FindObjectOfType<TAH_Reticle>().transform.GetChild(3))
				.GetComponent<TNHInfo>();
			TNHInfo.instance.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
		}

		void OnDestroy()
		{
			// Destroy statics
			tnhManager = null;
		}
	}
}