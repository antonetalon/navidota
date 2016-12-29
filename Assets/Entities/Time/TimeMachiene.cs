using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeMachiene {
	private class FrameStamp {
		public IEnumerable<ComponentChange> Changes;
		public bool IsFromServer;
		public float deltaTime;
	}
	private const float MaxTimeShift = 2;
	private static List<FrameStamp> _frames = new List<FrameStamp>();
	private static List<FrameStamp> _futureFramesFromServer = new List<FrameStamp>();
	public static void OnStartMatch() {
		_frames.Clear ();
		_futureFramesFromServer.Clear ();
	}
	public static void SaveCalculatedChanges(IEnumerable<ComponentChange> changes, float deltaTime) {
		FrameStamp frame = new FrameStamp ();
		frame.Changes = changes;
		frame.IsFromServer = false;
		frame.deltaTime = deltaTime;
		_frames.Add (frame);
	}
	public static void SaveReceivedCommands(IEnumerable<ComponentChange> changes) {
		FrameStamp frame = new FrameStamp ();
		frame.Changes = changes;
		frame.IsFromServer = true;
		frame.deltaTime = 0;
		_frames.Add (frame);
	}
	const float MaxStep = 0.05f;
	private static bool _wentToPast;
	public static void GoToPast(float timeShift) {
		if (_wentToPast) {
			Debug.LogError ("GoToPast cant go to past second time while previous trip not returned.");
			return;
		}
		if (timeShift < 0) {
			Debug.LogError ("GoToPast timeShift should be non-negative" + timeShift.ToString ());
			return;
		}
		_wentToPast = true;
		_futureFramesFromServer.Clear ();
		float currShift = 0;
		while (currShift < timeShift && _frames.Count > 0) {
			FrameStamp frame = _frames [_frames.Count - 1];
			_frames.RemoveAt (_frames.Count - 1);
			if (frame.IsFromServer) {
				_futureFramesFromServer.Insert (0, frame);
				frame.deltaTime = timeShift - currShift;
			}
			foreach (var change in frame.Changes)
				SyncChangesController.UndoChange (change);
			currShift += frame.deltaTime;
			Timer.GoToPast (frame.deltaTime);
		}
		if (currShift < timeShift) {
			Debug.LogError ("Cant go to past so much " + timeShift.ToString ());
			return;
		}
		GoForward (currShift - timeShift);
	}
	public static void GoForward(float timeShift) {
		if (timeShift < 0) {
			Debug.LogError ("GoForward timeShift should be non-negative" + timeShift.ToString ());
			return;
		}
		float currShift = 0;
		while (currShift < timeShift) {
			float delta = Mathf.Min (MaxStep, timeShift - currShift);
			float deltaFromServer = float.PositiveInfinity;
			if (_futureFramesFromServer.Count > 0)
				deltaFromServer = _futureFramesFromServer [0].deltaTime;
			bool applyServerChange = delta > deltaFromServer;
			delta = Mathf.Min (delta, deltaFromServer);
			if (delta > 0) {
				Timer.Update (delta);
				currShift += delta;
				MatchController.Instance.Update();
			}
			if (applyServerChange) {
				FrameStamp frame = _futureFramesFromServer [0];
				_futureFramesFromServer.RemoveAt (0);
				foreach (var change in frame.Changes)
					SyncChangesController.ApplyChange (change);
			}
			foreach (var frame in _futureFramesFromServer)
				frame.deltaTime -= delta;
		}
		_wentToPast = false;
	}
}
