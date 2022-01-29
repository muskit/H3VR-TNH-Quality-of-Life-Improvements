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
			var postfix = typeof(ShopCostPatch).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static);
			harmony.Patch(original, new HarmonyMethod(postfix));
		}

		private static void AddNumericalRepresentation(TNH_ObjectConstructorIcon __instance)
        {
			foreach (Transform curTran in __instance.gameObject.transform)
			{
				if (curTran.name.Contains("Cost"))
					curTran.gameObject.AddComponent<ShopCostNumber>();
			}
        }
	}

	public class ShopCostNumber : MonoBehaviour
    {
		private TNH_ObjectConstructorIcon objConstructor;
		private Text text;

		public void Awake()
        {
			objConstructor = transform.parent.GetComponent<TNH_ObjectConstructorIcon>();
        }

		public void Start()
        {
			var textTran = new GameObject().transform;
			textTran.SetParent(transform, false);
			textTran.localPosition = new Vector2(0, -245);

			text = textTran.gameObject.AddComponent<Text>();
			text.font = MeatKitPlugin.fontAgencyFB;
			text.alignment = TextAnchor.MiddleCenter;
			text.fontSize = 50;
		}

		public void Update()
        {
			text.text = objConstructor.Cost.ToString();
		}
    }
}