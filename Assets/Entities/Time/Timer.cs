using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer {
	public static float Time { get; private set; }
	public static float DeltaTime { get; private set; }
	public static void OnMatchConnected() {
		Time = UnityEngine.Time.time;
	}
	public static void OnMatchDisconnected() {
	}
	public static void Update(float deltaTime) {
		if (deltaTime < 0) {
			Debug.LogError ("DeltaTime shouldn't be negative");
			return;
		}
		DeltaTime = deltaTime;
		Time += DeltaTime;
	}
	public static void GoToPast(float deltaTime) {
		if (deltaTime < 0) {
			Debug.LogError ("DeltaTime shouldn't be negative");
			return;
		}
		Time -= deltaTime;
	}
}
