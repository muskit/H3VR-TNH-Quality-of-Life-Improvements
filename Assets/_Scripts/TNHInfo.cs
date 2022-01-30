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
            if (holdCounter != null)
                holdCounter.transform.localPosition = new Vector3(-333, 0, -450);

            if (tokenCounter != null)
                tokenCounter.transform.localPosition = new Vector3(333, 0, -450);

            if (waveCounter != null)
                waveCounter.transform.localPosition = new Vector3(333, 0, -450);
        }

        public void GameOverPos()
        {
            if (holdCounter != null)
                holdCounter.transform.localPosition = new Vector3(-250, 0, 0);

            if (tokenCounter != null)
                tokenCounter.transform.localPosition = new Vector3(250, 0, 0);

            if (waveCounter != null)
            {
                waveCounter.gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                waveCounter.transform.localPosition = new Vector3(0, 0, 140);
            }
        }

        public void Update()
        {
            if (InPlay.tnhManager.Phase == TNH_Phase.Dead)
            {
                if (tokenCounter != null)
                    tokenCounter.SetActive(true);

                return;
            }

            // we're in a hold; hide token count
            if(InPlay.tnhManager.Phase == TNH_Phase.Hold)
            {
                if (tokenCounter != null)
                    tokenCounter.SetActive(false);

                if (waveCounter != null)
                    waveCounter.SetActive(true);
            }
            else // show token count
            {
                if (tokenCounter != null)
                    tokenCounter.SetActive(true);

                if (waveCounter != null)
                    waveCounter.SetActive(false);
            }
        }
    }
}
