using UnityEngine;
using UnityEngine.UI;

namespace TNHQoLImprovements
{
    public enum HPTextType
    {
        Solidify, Untouched, Hidden
    }

	public static class HPReadability
	{
		public static void ImproveHPTextReadability(GameObject gObjHUD)
		{
			var canvas = gObjHUD.GetComponent<Canvas>();
			Transform[] tranHPText = {
				gObjHUD.transform.Find("Label_Title (1)"), // header
				gObjHUD.transform.Find("Label_Title")      // HP number
			};

			// apply background only if hp text type is not "Hidden"
			if (MeatKitPlugin.cfgShowHPBackground.Value &&
                MeatKitPlugin.cfgHPTextType.Value != HPTextType.Hidden)
			{
                var gObjBG = new GameObject();
                gObjBG.name = "Background";
                gObjBG.transform.SetParent(gObjHUD.transform, false);
				gObjBG.transform.SetSiblingIndex(0);
				gObjBG.transform.localPosition = new Vector3(0, 1, 0);
                gObjBG.transform.localScale = tranHPText[0].localScale;
                //gObjBG.transform.localRotation = Quaternion.identity;
                var rawImage = gObjBG.AddComponent<RawImage>();
				rawImage.color = new Color(0, 0, 0, MeatKitPlugin.cfgHPBackgroundOpacity.Value);
				rawImage.rectTransform.SetWidth(85);
				rawImage.rectTransform.SetHeight(44);
			}

            // set text type
            if (MeatKitPlugin.cfgHPTextType.Value == HPTextType.Untouched)
                return;

            switch (MeatKitPlugin.cfgHPTextType.Value)
            {
                case HPTextType.Solidify:
                    foreach (var text in tranHPText)
                    {
                        // full alpha
                        text.GetComponent<Text>().color = Color.white;
                        // drop shadow
                        var shadow = text.gameObject.AddComponent<Shadow>();
                        shadow.effectColor = new Color(0, 0, 0, .95f);
                        shadow.effectDistance = new Vector2(0.5f, -0.5f);
                    }
                    break;
                case HPTextType.Hidden:
                    foreach (var text in tranHPText)
                    {
                        text.GetComponent<Text>().color = new Color(0, 0, 0, 0);
                    }
                    break;
            }
		}
	}
}
