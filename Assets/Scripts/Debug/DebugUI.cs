﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI : MonoBehaviour {

	public void OnGUI() {
		GUILayout.BeginArea(new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height)));
		DebugOnGUIAuth.OnGUI();
		GUILayout.EndArea();
	}
}