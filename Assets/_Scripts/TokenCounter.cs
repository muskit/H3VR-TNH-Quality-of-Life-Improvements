using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace TNHQoLImprovements
{
	public class TokenCounter : MonoBehaviour
	{
        private Text text;

		void Start()
		{
            text = transform.GetChild(1).GetComponent<Text>();
            text.font = MeatKitPlugin.fontAgencyFB;

            StartCoroutine(SetTokenImage());
		}

		private IEnumerator SetTokenImage()
        {
			int debug_iterations = 0;
			Sprite tokenSprite = null;
			while (tokenSprite == null)  // loop until Token sprite is found
			{
				var obj = GameObject.Find("_TNH_ObjectConstructor(Clone)/_CanvasHolder/_UITest_Canvas/Icon_0/Cost_1/Image");
				if (obj != null)
				{
					tokenSprite = obj.GetComponent<Image>().sprite;
				}
				else
				{
					debug_iterations++;
					yield return new WaitForEndOfFrame();
				}
			}
			Debug.Log("Token sprite found after " + debug_iterations.ToString() + " iterations.");
			transform.GetChild(0).GetComponent<Image>().sprite = tokenSprite;
		}

		void Update()
		{
			int tokens = InPlay.tnhManager.GetNumTokens();
			text.text = tokens.ToString();
		}
	}
}