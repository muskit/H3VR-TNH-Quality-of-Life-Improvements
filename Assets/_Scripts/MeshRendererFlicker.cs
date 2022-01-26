using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TNHQoLImprovements
{
	/// <summary>
	/// Flash attached MeshRenderer the timer runs out.
	/// </summary>
	public class MeshRendererFlicker : MonoBehaviour
	{
		private bool initialized = false;

		private float beginTime;
		private float onLength;
		private float offLength;
		private float stateChangeTime = 0;
		private bool visible = true;

		private MeshRenderer mesh;

		void Start()
		{
			mesh = GetComponent<MeshRenderer>();
		}

		public void Init(float interval, float onToOffRatio = 0.5f,  float beginAfter = 0)
        {
			beginTime = Time.time + beginAfter;
			onLength = interval * onToOffRatio;
			offLength = interval * (1 - onToOffRatio);
			
			initialized = true;
        }

		void Update()
		{
			if (!initialized || Time.time < beginTime)
				return;

			if (Time.time >= stateChangeTime)
			{
				visible = !visible;
				mesh.enabled = visible;

				if (visible) // set time to stay on
				{
					stateChangeTime = Time.time + onLength;
				}
				else // set time to stay off
				{
					stateChangeTime = Time.time + offLength;
				}
			}

			//if (Time.time >= stateChangeTime)
   //         {
			//	stateChangeTime = Time.time + interval;
			//	visible = !visible;
			//	mesh.enabled = visible;
   //         }
		}
	}
}