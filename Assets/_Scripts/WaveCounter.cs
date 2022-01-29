using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using FistVR;

namespace TNHQoLImprovements
{
	public static class WavePatch
    {
		public static void Patch(Harmony harmony)
        {
			var original = typeof(TNH_Manager).GetMethod("HoldPointStarted", BindingFlags.Public | BindingFlags.Instance);
			var patch = typeof(WavePatch).GetMethod("OnHoldStart", BindingFlags.NonPublic | BindingFlags.Static);
			harmony.Patch(original, postfix: new HarmonyMethod(patch));
        }

		private static void OnHoldStart(TNH_HoldPoint p)
        {
			WaveCounter.WaveStarted.Invoke(p);
        }
    }

    public class WaveCounter : MonoBehaviour
	{
		[System.Serializable]
		public class WaveStartedEvent : UnityEvent<TNH_HoldPoint> { }
		public static WaveStartedEvent WaveStarted = new WaveStartedEvent();

		private bool initialized = false;

		private TNH_HoldPoint holdPoint;

		// Use this for initialization
		void Start()
		{

		}

		void Init(TNH_Manager manager)
		{
			holdPoint = manager.m_curHoldPoint;

			initialized = true;
		}

		// Update is called once per frame
		void Update()
		{
			if (!initialized)
				return;
		}
	}
}