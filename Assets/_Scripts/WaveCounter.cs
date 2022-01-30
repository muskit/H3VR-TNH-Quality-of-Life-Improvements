using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace TNHQoLImprovements
{
    public class WaveCounter : MonoBehaviour
	{
        private TNH_HoldPoint curHoldPoint;
        private Traverse<int> trCurPhaseIdx;
        private Traverse<int> trMaxPhases;

        private Text text;

		// Use this for initialization
		void Start()
		{
            text = transform.GetChild(1).GetComponent<Text>();
        }

		// Update is called once per frame
		void Update()
		{
            if (InPlay.tnhManager.Phase != TNH_Phase.Hold)
                return;

            if(!ReferenceEquals(curHoldPoint, InPlay.tnhManager.m_curHoldPoint))
            {
                Debug.Log("Hold point updated!");
                curHoldPoint = InPlay.tnhManager.m_curHoldPoint;
                trCurPhaseIdx = Traverse.Create(curHoldPoint).Field<int>("m_phaseIndex");
                trMaxPhases = Traverse.Create(curHoldPoint).Field<int>("m_maxPhases");
            }
            text.text = string.Format("{0} / {1}", trCurPhaseIdx.Value, trMaxPhases.Value);
        }
	}
}