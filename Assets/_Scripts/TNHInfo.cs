using UnityEngine;
using FistVR;

namespace TNHQoLImprovements
{
    class TNHInfo : MonoBehaviour
    {
        private GameObject holdCounter;
        private GameObject tokenCounter;

        public void Start()
        {
            if (MeatKitPlugin.cfgShowHolds.Value)
                holdCounter = Instantiate<GameObject>(MeatKitPlugin.bundle.LoadAsset<GameObject>("HoldCounter"), transform);
            if (MeatKitPlugin.cfgShowTokens.Value)
                tokenCounter = Instantiate<GameObject>(MeatKitPlugin.bundle.LoadAsset<GameObject>("TokenCounter"), transform);

        }

        public void PlayPos()
        {
            if (holdCounter != null)
                holdCounter.transform.localPosition = new Vector3(-333, 0, -450);

            if (tokenCounter != null)
                tokenCounter.transform.localPosition = new Vector3(333, 0, -450);
        }

        public void GameOverPos()
        {
            if (holdCounter != null)
                holdCounter.transform.localPosition = new Vector3(-250, 0, 0);

            if (tokenCounter != null)
                tokenCounter.transform.localPosition = new Vector3(250, 0, 0);
        }
    }
}
