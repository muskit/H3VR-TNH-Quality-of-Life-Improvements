using UnityEngine;
using UnityEngine.UI;

namespace TNHQoLImprovements
{
	public static class HPReadability
	{
		public static void ImproveHPTextReadability(GameObject gObjHUD)
		{
			Debug.Log("gObjHUD: " + gObjHUD);
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
	}
}
