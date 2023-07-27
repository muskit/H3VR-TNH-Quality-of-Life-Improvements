using System.Collections;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace TNHQoLImprovements
{
    public static class ShopTokenPatch
    {
        public static void Patch(Harmony harmony)
        {
            var original = typeof(TNH_ObjectConstructor).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
            var postfix = typeof(ShopTokenPatch).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(original, postfix: new HarmonyMethod(postfix));
        }
        private static void Postfix(TNH_ObjectConstructor __instance)
        {
            // add component to 1st token icon
            __instance.transform.GetChild(0).GetChild(0).GetChild(2).GetChild(0).gameObject.AddComponent<ShopTokenNumber>();
        }
    }

    // child of TopCell (the 0th child)
    class ShopTokenNumber : MonoBehaviour
    {
        private Text text;

        private IEnumerator Start()
        {
            var gObjText = new GameObject("TokenCounter");
            gObjText.transform.SetParent(transform, false);
            gObjText.transform.localPosition = new Vector3(0, -4, 0);

            text = gObjText.AddComponent<Text>();
            text.alignment = TextAnchor.MiddleCenter;
            text.font = MeatKitPlugin.fontBombardier;
            text.fontSize = 55;
            text.color = new Color(0.1307786f, 0.2461715f, 0.359f);

            while (InPlay.tnhManager == null)
            {
				Debug.Log("[ShopTokenNumber] tnhManager is null!");
				yield return null;
			}

			InPlay.tnhManager.TokenCountChangeEvent += UpdateText;
			UpdateText();
		}

        private void UpdateText(int _ = 0)
        {
            int tokens = InPlay.tnhManager.GetNumTokens();
            text.text = tokens.ToString();
        }

        private void OnDestroy()
        {
			InPlay.tnhManager.TokenCountChangeEvent -= UpdateText;
		}
    }
}