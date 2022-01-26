using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace TNHQoLImprovements
{
	public class TokenCounter : MonoBehaviour
	{
		void Start()
		{
			transform.localPosition = new Vector3(1, 0, -.5f);
			transform.localRotation = Quaternion.Euler(90, 0, 0);
			transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);

			StartCoroutine(SetTokenImage());
		}

		private IEnumerator SetTokenImage()
        {
			int debug_iterations = 0;
			Sprite tokenSprite = null;
			while (tokenSprite == null)  // END: loop until Token sprite is found
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
			transform.GetChild(1).GetComponent<Text>().text = tokens.ToString();
		}
	}
}