using System.Collections;
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
		private GameObject gObjHUD;
		private GameObject gObjTokens;
		public static TNH_Manager tnhManager;

		#region INITIALIZATION
		void ImproveHPTextReadability()
		{
			var canvas = gObjHUD.GetComponent<Canvas>();
			var gObjBG = new GameObject();
			var tranHPTitle = gObjHUD.transform.Find("Label_Title (1)");
			var tranHP = gObjHUD.transform.Find("Label_Title");

			// apply background
			if (MeatKitPlugin.showHPBackground.Value)
            {
				gObjBG.transform.parent = gObjHUD.transform;
				gObjBG.transform.SetSiblingIndex(0);
				gObjBG.transform.localPosition = new Vector3(0, 1, 0);
				gObjBG.transform.localRotation = Quaternion.identity;
				gObjBG.transform.localScale = tranHP.localScale;
				var rawImage = gObjBG.AddComponent<RawImage>();
				rawImage.color = new Color(0, 0, 0, MeatKitPlugin.hpBackgroundOpacity.Value);
				rawImage.rectTransform.SetWidth(100);
				rawImage.rectTransform.SetHeight(52);
			}

			// full text alphas
			tranHPTitle.GetComponent<Text>().color = Color.white;
			tranHP.GetComponent<Text>().color = Color.white;
			// text shadows
			var shadow = tranHPTitle.gameObject.AddComponent<Shadow>();
			shadow.effectColor = new Color(0, 0, 0, .95f);
			shadow.effectDistance = new Vector2(0.5f, -0.5f);
			shadow = tranHP.gameObject.AddComponent<Shadow>();
			shadow.effectColor = new Color(0, 0, 0, .95f);
			shadow.effectDistance = new Vector2(0.5f, -0.5f);
		}

		// Use this for initialization
		void Start()
		{
			tnhManager = GameObject.Find("_GameManager").GetComponent<TNH_Manager>();
			gObjHUD = GameObject.Find("HealthBar(Clone)/f");

			ImproveHPTextReadability();
			if (MeatKitPlugin.showTokens.Value)
				Instantiate(MeatKitPlugin.bundle.LoadAsset<GameObject>("TokenCounter"));
			if (MeatKitPlugin.showHolds.Value)
				Instantiate(MeatKitPlugin.bundle.LoadAsset<GameObject>("HoldCounter"));
		}
		#endregion
	}
}