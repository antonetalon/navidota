using UnityEngine;
using System.Collections;
using GameSparks.Core;

public class DebugOnGUIAuth {	
	private static string _name = "";
	private static string _login = "";
	private static string _pass = "";
	private static string _oldPass = "";
	private static bool _waitingResponse;
	public static GUILayoutOption ProperHeight { get { return GUILayout.Height(Screen.height*0.05f); } }
	public static void OnGUI() {
		GUILayout.BeginVertical();
		if (_waitingResponse) {
			GUILayout.Label("waiting server response...", ProperHeight);
		} else if (GameController.Authed) {
			if (GameController.Inited) {
				GUILayout.Label("Logged in as "+GameController.Instance.CurrPlayer.Name, ProperHeight);
				// Add login/pass.
				GUILayout.BeginHorizontal();
				GUILayout.Label("login", ProperHeight);
				_login = GUILayout.TextField(_login, ProperHeight);
				GUILayout.Label("pass", ProperHeight);
				_pass = GUILayout.TextField(_pass, ProperHeight);
				if (GUILayout.Button("Add login/pass", ProperHeight)) {
					_waitingResponse = true;
					AuthController.SetLoginPass(_login, _pass, (success)=>{
						_waitingResponse = false;
					});
				}
				GUILayout.EndHorizontal();
				// Change pass.
				GUILayout.BeginHorizontal();
				GUILayout.Label("new pass", ProperHeight);
				_pass = GUILayout.TextField(_pass, ProperHeight);
				GUILayout.Label("old pass", ProperHeight);
				_oldPass = GUILayout.TextField(_oldPass, ProperHeight);
				if (GUILayout.Button("Change pass", ProperHeight)) {
					_waitingResponse = true;
					AuthController.ChangePass(_pass, _oldPass, (success)=>{
						_waitingResponse = false;
					});
				}
				GUILayout.EndHorizontal();
				// Change name.
				GUILayout.BeginHorizontal();
				GUILayout.Label("Name", ProperHeight);
				_name = GUILayout.TextField(_name, ProperHeight);
				if (GUILayout.Button("Change name", ProperHeight)) {
					_waitingResponse = true;
					PlayerController.ChangePlayerName(_name, (success)=>{
						_waitingResponse = false;
					});
				}
				GUILayout.EndHorizontal();
			} else
				GUILayout.Label("Authenticated, waiting player data...", ProperHeight);
			if (GUILayout.Button("Log out", ProperHeight)) {
				AuthController.UnAuthenticate();
			}
		} else {
			if (!GS.Available)
				GUILayout.Label("not connectd", ProperHeight);
			else {
				if (GUILayout.Button("Auth/register with device", ProperHeight)) {
					_waitingResponse = true;
					AuthController.AuthWithDevice((success)=>{
						_waitingResponse = false;
					});
				}

				GUILayout.BeginHorizontal();
				GUILayout.Label("Login", ProperHeight);
				_login = GUILayout.TextField(_login, ProperHeight);
				GUILayout.Label("Pass", ProperHeight);
				_pass = GUILayout.TextField(_pass, ProperHeight);
				if (GUILayout.Button("Auth with login pass", ProperHeight)) {
					_waitingResponse = true;
					AuthController.AuthWithLoginPass(_login, _pass, (success)=>{
						_waitingResponse = false;
					});
				}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Login", ProperHeight);
				_login = GUILayout.TextField(_login, ProperHeight);
				GUILayout.Label("Pass", ProperHeight);
				_pass = GUILayout.TextField(_pass, ProperHeight);
				GUILayout.Label("Name", ProperHeight);
				_name = GUILayout.TextField(_name, ProperHeight);
				if (GUILayout.Button("Register with login pass", ProperHeight)) {
					_waitingResponse = true;
					AuthController.RegisterWithLoginPass(_login, _pass, _name, (success)=>{
						_waitingResponse = false;
					});
				}
				GUILayout.EndHorizontal();
			}
		}
		GUILayout.EndVertical();
	}
}
