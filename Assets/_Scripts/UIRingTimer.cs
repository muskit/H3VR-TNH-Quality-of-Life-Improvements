using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRingTimer : MonoBehaviour {

	private bool initialized = false;
	private float endTime;
	private float length;

	private Image ringImg;

	private void Start()
    {
		ringImg = GetComponentInChildren<Image>();
    }

	public void Init(float timeInSeconds)
    {
		length = timeInSeconds;
		endTime = Time.time + length;
		initialized = true;
    }
	
	void Update () {
		if (!initialized)
			return;

		float amount = (endTime - Time.time) / length;
		ringImg.fillAmount = Mathf.Clamp01(amount);
	}
}
