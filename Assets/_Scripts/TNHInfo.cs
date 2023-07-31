using HarmonyLib;
using System.Reflection;
using UnityEngine;
using FistVR;

namespace TNHQoLImprovements
{
    class TNHInfo : MonoBehaviour
    {
		public static TNHInfo instance;

        public  Transform[] hands;
		private GameObject holdCounter;
        private GameObject tokenCounter;
        private GameObject waveCounter;

        // Bring extra info into game over
		public static void Patch(Harmony harmony)
        {
			var original = typeof(TNH_Manager).GetMethod("SetPhase", BindingFlags.NonPublic | BindingFlags.Instance);
			var patch = typeof(TNHInfo).GetMethod("MoveStatsToController", BindingFlags.NonPublic | BindingFlags.Static);
			harmony.Patch(original, postfix: new HarmonyMethod(patch));
		}

		private static void MoveStatsToController(TNH_Phase p)
        {
			if (InPlay.tnhManager == null)
				return;

            if (p == TNH_Phase.Dead || p == TNH_Phase.Completed)
            {
				int handSide = InPlay.tnhManager.RadarHand == TNH_RadarHand.Left ? 0 : 1;

				instance.transform.SetParent(instance.hands[handSide], false);
				instance.GetComponent<TNHInfo>().GameOverPos();
			}
		}

        void Start()
        {
			instance = this;

			if (MeatKitPlugin.cfgShowHolds.Value)
                holdCounter = Instantiate<GameObject>(MeatKitPlugin.bundle.LoadAsset<GameObject>("HoldCounter"), transform);
            if (MeatKitPlugin.cfgShowTokens.Value)
                tokenCounter = Instantiate<GameObject>(MeatKitPlugin.bundle.LoadAsset<GameObject>("TokenCounter"), transform);
            if (MeatKitPlugin.cfgShowWaves.Value)
                waveCounter = Instantiate<GameObject>(MeatKitPlugin.bundle.LoadAsset<GameObject>("WaveCounter"), transform);

            var rig = Object.FindObjectOfType<FVRMovementManager>().transform;
            hands = new Transform[] {
				rig.transform.GetChild(1), rig.transform.GetChild(0)
			};

            PlayPos();
        }

        private void PlayPos()
        {
            transform.localPosition = new Vector3(0, 0, -1.2f);
            if (holdCounter != null)
                holdCounter.transform.localPosition = new Vector3(-333, 0, 0);

            if (tokenCounter != null)
                tokenCounter.transform.localPosition = new Vector3(333, 0, 0);

            if (waveCounter != null)
                waveCounter.transform.localPosition = new Vector3(333, 0, 0);
        }

        private void GameOverPos()
        {
            transform.localScale = new Vector3(.0002f, .0002f, .0002f);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            if (holdCounter != null)
            {
                holdCounter.gameObject.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
                holdCounter.transform.localPosition = new Vector3(-250, 0, 0);
            }

            if (tokenCounter != null)
            {
                tokenCounter.gameObject.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                tokenCounter.transform.localPosition = new Vector3(250, 0, 0);
            }

            if (waveCounter != null)
            {
                waveCounter.gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                waveCounter.transform.localPosition = new Vector3(0, 0, 140);
            }
        }

        private void Update()
        {
			// game over area; do not update anything else
			if (InPlay.tnhManager.Phase == TNH_Phase.Dead || InPlay.tnhManager.Phase == TNH_Phase.Completed)
            {
                if (tokenCounter != null)
                    tokenCounter.SetActive(true);

                return;
            }

            // TNHInfo rotate to player camera
            if (MeatKitPlugin.cfgInfoFollowCamera.Value)
            {
                transform.LookAt(MeatKitPlugin.playerCamera.transform);
                var rotLook = transform.localEulerAngles;
                var rot = Vector3.zero;

                rot.x = -rotLook.x - 90;
                transform.localRotation = Quaternion.Euler(rot);
            }

            // we're in a hold; hide token count and show wave count
            if (InPlay.tnhManager.Phase == TNH_Phase.Hold)
            {
                if (tokenCounter != null)
                    tokenCounter.SetActive(false);

                if (waveCounter != null)
                    waveCounter.SetActive(true);
            }
            else // NOT in hold; do the inverse
            {
                if (tokenCounter != null)
                    tokenCounter.SetActive(true);

                if (waveCounter != null)
                    waveCounter.SetActive(false);
            }
        }

        void OnDestroy()
        {
			instance = null;
		}
    }
}
