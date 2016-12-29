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
		ClampFrames ();
	}
	public static void SaveReceivedCommands(IEnumerable<ComponentChange> changes) {
		FrameStamp frame = new FrameStamp ();
		frame.Changes = changes;
		frame.IsFromServer = true;
		frame.deltaTime = 0;
		_frames.Add (frame);
	}
	static void ClampFrames() {
		float framesSum = 0;
		for (int i = _frames.Count - 1; i >= 0; i--) {
			if (framesSum < MaxTimeShift)
				framesSum += _frames [i].deltaTime;
			else {
				_frames.RemoveRange (0, i+1);
				break;
			}
		}
	}
	const float MaxStep = 0.05f;
	private static bool _wentToPast;
	public static void GoToPast(float timeShift) {
		//Debug.Log ("go to past on "+timeShift.ToString());
		if (_wentToPast) {
			Debug.LogError ("GoToPast cant go to past second time while previous trip not returned.");
			return;
		}
		if (timeShift < 0) {
			Debug.LogError ("GoToPast timeShift should be non-negative" + timeShift.ToString ());
			return;
		}
		_futureFramesFromServer.Clear ();
		float currShift = 0;
		while (currShift < timeShift && _frames.Count > 0) {
			FrameStamp frame = _frames [_frames.Count - 1];
			//Debug.Log ("undoing change on dt="+frame.deltaTime.ToString());
			_frames.RemoveAt (_frames.Count - 1);
			if (frame.IsFromServer) {
				_futureFramesFromServer.Insert (0, frame);
				frame.deltaTime = timeShift - currShift;
			}
			foreach (var change in frame.Changes) {
				//Debug.LogFormat ("frame change = (before={0}, after={1})", change.Before.ToString(), change.After.ToString());
				SyncChangesController.UndoChange (change);
			}
			currShift += frame.deltaTime;
			Timer.GoToPast (frame.deltaTime);
			//SyncChangesController.LogCharPos ("after undoing change");
		}
		if (currShift < timeShift) {
			Debug.LogError ("Cant go to past so much " + timeShift.ToString ());
			return;
		}
		//Debug.Log ("little goforward on "+(currShift - timeShift).ToString());
		GoForward (currShift - timeShift);
		_wentToPast = true;
	}
	public static void GoForward(float timeShift) {
		//Debug.Log ("go forward on "+timeShift.ToString());
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
			//Debug.Log ("going forward on "+delta.ToString());
			if (delta > 0) {
				currShift += delta;
				MatchController.Instance.UpdateWithDelta(delta);
			}
			//SyncChangesController.LogCharPos ("after forward simulation");
			if (applyServerChange) {
				FrameStamp frame = _futureFramesFromServer [0];
				_futureFramesFromServer.RemoveAt (0);
				foreach (var change in frame.Changes)
					SyncChangesController.ApplyChange (change);
			}
			//SyncChangesController.LogCharPos ("after apply server change in forward simulation");
			foreach (var frame in _futureFramesFromServer)
				frame.deltaTime -= delta;
		}
		_wentToPast = false;
	}
}
