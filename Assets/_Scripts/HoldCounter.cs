using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace TNHQoLImprovements
{
	public class HoldCounter : MonoBehaviour
	{
		private void OnDeath(bool _)
        {
			Debug.Log("I died!");
        }

		void Start()
		{
			transform.parent = GameObject.Find("_NewTAHReticle/TAHReticle_HealthBar").transform;
			transform.localPosition = new Vector3(-1f, 0, -.5f);
			transform.localRotation = Quaternion.Euler(90, 0, 0);
			transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);

			GameObject.Find("[SceneSettings]").GetComponent<FVRSceneSettings>().PlayerDeathEvent += OnDeath;
		}

		void Update()
		{
			string display = "";
			if (InPlay.tnhManager.ProgressionMode == TNHSetting_ProgressionType.Marathon)
				display = InPlay.tnhManager.m_level.ToString() + " / ∞";
			else
				display = string.Format("{0} / {1}", InPlay.tnhManager.m_level, InPlay.tnhManager.m_maxLevels);

			transform.GetChild(1).GetComponent<Text>().text = display;
		}
	}
}