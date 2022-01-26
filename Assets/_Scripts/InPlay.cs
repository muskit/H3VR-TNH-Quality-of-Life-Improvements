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
		public static TNH_Manager tnhManager;

		void ImproveHPTextReadability()
		{
			var canvas = gObjHUD.GetComponent<Canvas>();
			var gObjBG = new GameObject();
			Transform[] tranHPText = {
				gObjHUD.transform.Find("Label_Title (1)"),
				gObjHUD.transform.Find("Label_Title")
			};

			// apply background
			if (MeatKitPlugin.cfgShowHPBackground.Value)
            {
				gObjBG.transform.parent = gObjHUD.transform;
				gObjBG.transform.SetSiblingIndex(0);
				gObjBG.transform.localPosition = new Vector3(0, 1, 0);
				gObjBG.transform.localRotation = Quaternion.identity;
				gObjBG.transform.localScale = tranHPText[0].localScale;
				var rawImage = gObjBG.AddComponent<RawImage>();
				rawImage.color = new Color(0, 0, 0, MeatKitPlugin.cfgHPBackgroundOpacity.Value);
				rawImage.rectTransform.SetWidth(100);
				rawImage.rectTransform.SetHeight(52);
			}
			if (MeatKitPlugin.cfgSolidifyHPText.Value)
            {
				foreach (var text in tranHPText)
                {
					// full alpha
					text.GetComponent<Text>().color = Color.white;
					// drop shadow
					var shadow = text.gameObject.AddComponent<Shadow>();
					shadow.effectColor = new Color(0, 0, 0, .95f);
					shadow.effectDistance = new Vector2(0.5f, -0.5f);
				}
			}
		}

		// Use this for initialization
		void Start()
		{
			tnhManager = GameObject.Find("_GameManager").GetComponent<TNH_Manager>();
			gObjHUD = GameObject.Find("HealthBar(Clone)/f");

			if(MeatKitPlugin.cfgShowHPBackground.Value || MeatKitPlugin.cfgSolidifyHPText.Value)
				ImproveHPTextReadability();
			if (MeatKitPlugin.cfgShowTokens.Value)
				Instantiate(MeatKitPlugin.bundle.LoadAsset<GameObject>("TokenCounter"), FindObjectOfType<TAH_Reticle>().transform.GetChild(3));
			if (MeatKitPlugin.cfgShowHolds.Value)
				Instantiate(MeatKitPlugin.bundle.LoadAsset<GameObject>("HoldCounter"), FindObjectOfType<TAH_Reticle>().transform.GetChild(3));
		}
	}
}