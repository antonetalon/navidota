using System;
using GameSparks.Core;
using UnityEngine;
using GameSparks.Api.Requests;

public class AuthController {
	public static event Action OnAuthenticated;
    public const string AccountDetailsDataFileName = "AccountDetailsData";

    public static bool IsAuthenticated {
		get { return GS.Available && GS.Authenticated; }
    }

	public static void AuthWithLoginPass(string login, string password, Action<bool> onResponse) {
		new AuthenticationRequest()
			.SetUserName(login)
			.SetPassword(password)
			.Send(response => {
				bool success = !response.HasErrors;
				if (success)
					OnAuthed(AuthModel.Type.LoginPass);
				if (onResponse != null)
					onResponse(success);				
			});
	}
	public static void RegisterWithLoginPass(string login, string password, string name, Action<bool> onResponse) {
		new RegistrationRequest()
			.SetDisplayName(name)
			.SetPassword(password)
			.SetUserName(login)
			.Send(response => {
				bool success = !response.HasErrors;
				if (success)
					OnAuthed(AuthModel.Type.LoginPass);
				if (onResponse != null)
					onResponse(success);
			});
	}
	public static void AuthWithDevice(Action<bool> onResponse) {
		new DeviceAuthenticationRequest().Send((response) => {
			bool success = !response.HasErrors;
			if (success)
				OnAuthed(AuthModel.Type.Device);
			if (onResponse != null)
				onResponse(success);
		});
	}
	public static void RegisterWithDeviceId(Action<bool> onResponse) {
		AuthWithDevice(onResponse);
	}

	public static void SetLoginPass(string login, string password, Action<bool> onResponse) {
		if (!GameController.Inited) {
			if (onResponse!=null)
				onResponse(false);
			return;
		}
		SetPlayerDetails(password, null, login, (success)=>{
			if (success)
				GameController.Instance.CurrAuth.AddAuthType(AuthModel.Type.LoginPass);
			if (onResponse!=null)
				onResponse(success);
		});
	}
	public static void ChangePass(string password, string oldPassword, Action<bool> onResponse) {
		if (!GameController.Inited) {
			if (onResponse!=null)
				onResponse(false);
			return;
		}
		SetPlayerDetails(password, oldPassword, null, (success)=>{
			if (onResponse!=null)
				onResponse(success);
		});
	}
	private static void SetPlayerDetails(string password, string oldPassword, string login, Action<bool> onResponse) {
		ChangeUserDetailsRequest request = new ChangeUserDetailsRequest();
		if (!string.IsNullOrEmpty(password)) {
			request = request.SetNewPassword(password);
			request = request.SetOldPassword(oldPassword);
		}
		if (!string.IsNullOrEmpty(login))
			request = request.SetUserName(login);
		request.Send((response) => {
			if (onResponse!=null)
				onResponse(!response.HasErrors);
		});
	}

	public static void UnAuthenticate() {
		if (GS.Authenticated) 
			GS.Reset();
		GameController.Instance.SetCurrAuth(null);
		GameController.Instance.SetCurrPlayer(null);
	}

  	public static void Update() {
		UpdateStartAutoAuth();
	}

	private const float AutoAuthDuration = 2;
	private static bool _firstAuthSucceded;
	private static bool _firstAuthFailed;
	private static bool FirstAuthFinished { get { return _firstAuthFailed || _firstAuthSucceded; } }
    private static float _timeSinceStartUp;
	public static event Action<bool> OnAutoAuthFinished;
	static void UpdateStartAutoAuth(){
	    if (!GS.Available)
            return;
		if (FirstAuthFinished) 
			return;
		if (GS.Authenticated) {			
			OnAutoAuthed(true);
		} else {
            _timeSinceStartUp += Time.deltaTime;
			if (_timeSinceStartUp > AutoAuthDuration) {				
				OnAutoAuthed(false);
			}
			return;
		} 
	}
	private static void OnAutoAuthed(bool success) {
		if (success)
			_firstAuthSucceded = true;
		else
			_firstAuthFailed = true;
		if (success)
			OnAuthed(AuthModel.Type.None);// Nobody knows what auth did gs use.
		if (OnAutoAuthFinished!=null)
			OnAutoAuthFinished(success); 
	}
	private static void OnAuthed(AuthModel.Type type) {
		GameController.Instance.SetCurrAuth(new AuthModel(type));
		if (OnAuthed!=null)
			OnAuthed();
	}
}
