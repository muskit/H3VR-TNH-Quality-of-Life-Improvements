using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace TNHQoLImprovements
{
	public class ShopCostPatch : MonoBehaviour
	{
		public static void Patch(Harmony harmony)
        {
			var original = typeof(TNH_ObjectConstructorIcon).GetMethod("Init", BindingFlags.Public | BindingFlags.Instance);
			var postfix = typeof(ShopCostPatch).GetMethod("AddCostNumber", BindingFlags.NonPublic | BindingFlags.Static);
			harmony.Patch(original, new HarmonyMethod(postfix));
		}

		private static void AddCostNumber(TNH_ObjectConstructorIcon __instance)
        {
			foreach (Transform curTran in __instance.gameObject.transform)
			{
				if (curTran.name.Contains("Cost"))
					curTran.gameObject.AddComponent<CostNumber>();
			}
        }
	}

	public class CostNumber : MonoBehaviour
    {
		private TNH_ObjectConstructorIcon objConstructorIcon;
		private Text text;

		public void Awake()
        {
			objConstructorIcon = transform.parent.GetComponent<TNH_ObjectConstructorIcon>();
        }

		public void Start()
        {
			var textTran = new GameObject().transform;
			textTran.SetParent(transform, false);
			textTran.localPosition = new Vector2(0, 245);

			text = textTran.gameObject.AddComponent<Text>();
			text.font = MeatKitPlugin.fontBombardier;
            text.alignment = TextAnchor.MiddleCenter;
			text.fontSize = 72;
		}

		public void Update()
        {
			text.text = objConstructorIcon.Cost.ToString();
            text.color = objConstructorIcon.GetComponent<Image>().color;
		}
    }
}