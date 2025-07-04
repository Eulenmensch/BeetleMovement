﻿using UnityEngine;

namespace Source.AI.Steering
{
	public class ScreenSpaceContainer : MonoBehaviour
	{
		private Camera mainCamera;
		private void Start()
		{
			mainCamera = Camera.main;
		}

		private void Update()
		{
			//you get a world space coord and transfom it to viewport space.
			Vector3 pos = mainCamera.WorldToViewportPoint(transform.position);

			//everything from here on is in viewport space where 0,0 is the bottom 
			//left of your screen and 1,1 the top right.
			if (pos.x < 0.0f) {
				pos = new Vector3(1.0f, pos.y, pos.z);
			}
			else if (pos.x >= 1.0f) {
				pos = new Vector3(0.0f, pos.y, pos.z);
			}
			if (pos.y < 0.0f) {
				pos = new Vector3(pos.x, 1.0f, pos.z);
			}
			else if (pos.y >= 1.0f) {
				pos = new Vector3(pos.x, 0.0f, pos.z);
			}

			//and here it gets transformed back to world space.
			transform.position = mainCamera.ViewportToWorldPoint(pos);
		}
	}
}