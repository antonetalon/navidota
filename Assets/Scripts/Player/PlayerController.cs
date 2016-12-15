using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameSparks.Api.Requests;

public class PlayerController {
	public static void Init() {
		AuthController.OnAuthenticated += OnAuthenticated;
	}

	static void OnAuthenticated () {
		GetPlayerData(null);
	}
	private static void GetPlayerData(Action<bool> onResponse) {
		new AccountDetailsRequest()
			.Send(response => {
				bool success = !response.HasErrors;
				if (success) {					
					JSONDictData data = new JSONDictData(response.JSONString);
					string name = data.GetString("displayName", string.Empty);
					PlayerModel player = new PlayerModel(name);
					GameController.Instance.SetCurrPlayer(player);
				}
				if (onResponse != null)
					onResponse(success);
			});
	}
	public static void ChangePlayerName(string name, Action<bool> onResponse) {
		if (!GameController.Inited) {
			if (onResponse!=null)
				onResponse(false);
			return;
		}
		SetPlayerDetails(name, null, (success)=>{
			if (success)
				GameController.Instance.CurrPlayer.Name = name;
			if (onResponse!=null)
				onResponse(success);
		});
	}
	private static void SetPlayerDetails(string name, string lang, Action<bool> onResponse) {
		ChangeUserDetailsRequest request = new ChangeUserDetailsRequest();
		if (!string.IsNullOrEmpty(name))
			request = request.SetDisplayName(name);
		if (!string.IsNullOrEmpty(lang))
			request = request.SetLanguage(lang);
		request.Send((response) => {
			if (onResponse!=null)
				onResponse(!response.HasErrors);
		});
	}
}
