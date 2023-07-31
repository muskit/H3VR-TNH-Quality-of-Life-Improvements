using HarmonyLib;
using UnityEngine;
using FistVR;

namespace TNHQoLImprovements
{
    class TNHInfo : MonoBehaviour
    {
        private GameObject holdCounter;
        private GameObject tokenCounter;
        private GameObject waveCounter;

        public void Start()
        {
            if (MeatKitPlugin.cfgShowHolds.Value)
                holdCounter = Instantiate<GameObject>(MeatKitPlugin.bundle.LoadAsset<GameObject>("HoldCounter"), transform);
            if (MeatKitPlugin.cfgShowTokens.Value)
                tokenCounter = Instantiate<GameObject>(MeatKitPlugin.bundle.LoadAsset<GameObject>("TokenCounter"), transform);
            if (MeatKitPlugin.cfgShowWaves.Value)
                waveCounter = Instantiate<GameObject>(MeatKitPlugin.bundle.LoadAsset<GameObject>("WaveCounter"), transform);

            PlayPos();
        }

        public void PlayPos()
        {
            transform.localPosition = new Vector3(0, 0, -1.2f);
            if (holdCounter != null)
                holdCounter.transform.localPosition = new Vector3(-333, 0, 0);

            if (tokenCounter != null)
                tokenCounter.transform.localPosition = new Vector3(333, 0, 0);

            if (waveCounter != null)
                waveCounter.transform.localPosition = new Vector3(333, 0, 0);
        }

        public void GameOverPos()
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

        public void Update()
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
    }
}
