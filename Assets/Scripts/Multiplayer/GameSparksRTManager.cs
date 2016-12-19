using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Core;
using GameSparks.Api.Responses;
using System;
using GameSparks.RT;
using UnityEngine;

public class GameSparksRTManager : MonoBehaviour {
	public static GameSparksRTManager Instance { get; private set; }
	[SerializeField] GameSparksRTUnity _RT;
	public event Action OnRTDisconnected;

	void Awake() {
		if (Instance!=null) {
			DestroyImmediate(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(this);
		_sentCommands = new Dictionary<int, SentCommand>();
		_lastReceivedMessageIds = new List<int>();
		_commandsParser = new DefaultCommandsParser();
	}
	bool _isStartingSession;
	Action<bool> _onResponse;
	public void StartNewRTSession(string host, int port, string accessToken, Action<bool> onResponse) {
		if (_isStartingSession) {
			if (onResponse!=null)
				onResponse(false);
			return;
		}
		_isStartingSession = true;
		_onResponse = onResponse;
		GSRequestData mockedResponse = new GSRequestData()
			.AddNumber("port", (double)port)
			.AddString("host", host)
			.AddString("accessToken", accessToken);
		FindMatchResponse response = new FindMatchResponse(mockedResponse);
		_RT.Configure(response, 
			(peerId) =>  {    OnPlayerConnectedToGame(peerId);  },
			(peerId) => {    OnPlayerDisconnectedFromGame(peerId);    },
			(ready) => {    OnRTReady(ready);    },
			(packet) => {    OnPacketReceived(packet);    });
		_RT.Connect();
		_sentCommands.Clear();
		_lastReceivedMessageIds.Clear();
	}

	public void RTSessionDisconnect() {
		_RT.Disconnect();
	}

	public event Action<int> OnPlayerConnected;
	private void OnPlayerConnectedToGame(int peerId){
		Debug.Log ("Player Connected, "+peerId);
		if (OnPlayerConnected!=null)
			OnPlayerConnected(peerId);
	}

	public event Action<int> OnPlayerDisconnected;
	private void OnPlayerDisconnectedFromGame(int peerId){
		Debug.Log ("Player Disconnected, "+peerId);
		if (OnPlayerDisconnected!=null)
			OnPlayerDisconnected(peerId);
	}
	private void OnRTReady(bool isReady){		
		if (isReady) {
			Debug.Log ("RT Session Connected...");
		} else {
			if (OnRTDisconnected!=null)
				OnRTDisconnected();
		}
		if (!_isStartingSession)
			return;
		if (_onResponse!=null) {
			_onResponse(isReady);
			_onResponse = null;
		}
		_isStartingSession = false;
	}
	private const int MaxLastMessageIds = 1000;
	private List<int> _lastReceivedMessageIds;
	public event Action<MatchCommand> OnCommandReceived;
	private void OnPacketReceived(RTPacket _packet){
		Debug.Log("Received RT packet, opCode = " + _packet.OpCode);
		/*int messageId = _packet.OpCode / MaxCommandId;
		int command = messageId % MaxCommandId;
		//		Debug.LogFormat ("received command {0} with messageid {1}", commandId, messageId);

		if (command != 0) {
			//			Debug.Log ("sending command confirmer with messageid " + messageId.ToString());
			SendDataUnreliableTry(new SentCommand(0, new RTData(), messageId)); // Send command confirmer.
		}

		if (_lastReceivedMessageIds.Contains (messageId)) {
			//			Debug.Log ("ignored as duplicate");
			return; // Ignore duplicate.
		}
		//if (commandId == (int)MultiplayerCommand.StartLightAttack)
		//	Debug.Log("hi2");
		_lastReceivedMessageIds.Add(messageId);
		if (_lastReceivedMessageIds.Count > MaxLastMessageIds)
			_lastReceivedMessageIds.RemoveAt (0);
		if (command == 0) {
			//			Debug.Log ("stopped sending command with messageid " + messageId.ToString());
			_sentCommands.Remove (messageId); // Command receiving confirmed. Dont resent this command.
			return; // Ignore command confirmer.
		}*/

		OnReliableDataReceived(_packet.OpCode, _packet.Data);
	}
	private ReceivedCommandsFactory _commandsParser;
	public void SetCommandsFactory(ReceivedCommandsFactory commandsParser) {
		_commandsParser = commandsParser;
	}
	/// <summary>
	/// Data that other side really sent. 
	/// </summary>
	/// <param name="opCode">Op code.</param>
	/// <param name="data">Data.</param>
	private void OnReliableDataReceived(int opCode, RTData data) {
		MatchCommand command = _commandsParser.ParseCommand(opCode, data);
		if (OnCommandReceived!=null)
			OnCommandReceived(command);
	}
	static int[] _serverPeerId = new int[]{ 0 };
	private void SendPacket(int command, RTData data, bool reliable) {
		if (data==null)
			data = new RTData();
		Debug.Log("Sent RT packet, opCode = " + command);
		_RT.SendData(command, reliable?GameSparksRT.DeliveryIntent.RELIABLE:GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data, _serverPeerId);
	}
	void Update() {
		UpdateRedudantSendingUnreliable();
	}

	#region Sending commands
	const int MaxCommandId = 1000;
	private void CheckCommandId(int command) {
		#if UNITY_EDITOR
		if (command > MaxCommandId)
			Debug.LogError (string.Format("command {0} has illegal ind {1}", command.ToString(), command));
		#endif
	}
	public void SendDataReliable (MatchCommand command) {
		int opCode = _commandsParser.GetOpCode(command.GetType());
		if (opCode==-1) {
			Debug.LogError("opcode not found for command " + command.ToString());
			return;
		}
		SendDataReliable(opCode, command.Data);
	}
	private void SendDataReliable (int opCode, RTData data) {
		//int messageId = UnityEngine.Random.Range (0, int.MaxValue);
		//messageId /= MaxCommandId;
		//CheckCommandId(opCode);
		//int commandId2 = opCode + messageId*MaxCommandId;
		//SendPacket(commandId2, data, true);
		SendPacket(opCode, data, true);
	}
	private void SendDataFastDuplicate (int opCode, RTData data) {
		Debug.LogError("This one is implemented only for peer to peer");
		CheckCommandId(opCode);
		int messageId = UnityEngine.Random.Range (0, int.MaxValue);
		messageId /= MaxCommandId;
		SendData (opCode, data, messageId);
	}

	private class SentCommand
	{
		public readonly int CommandId;
		public readonly RTData Data;
		public readonly int MessageId;
		public float LastSendTime;
		public SentCommand(int commandId, RTData data, int messageId) {
			this.CommandId = commandId;
			this.Data = data;
			this.MessageId = messageId;
		}
	}
	private Dictionary<int, SentCommand> _sentCommands;
	private void SendData (int command, RTData data, int messageId) {
		//Debug.LogFormat ("sending command {0} with messageid {1}", commandId, messageId);
		CheckCommandId(command);
		SentCommand sentCommand = new SentCommand (command, data, messageId);
		_sentCommands.Add (messageId, sentCommand);
		SendDataUnreliableTry (sentCommand);
		SendDataUnreliableTry (sentCommand); // Send command twice, redudancy for increasing reliability with no impact on latency.
	}
	private void SendDataUnreliableTry(SentCommand command) {		
		int commandId = command.CommandId + command.MessageId*MaxCommandId;
		command.LastSendTime = Time.time;
		SendPacket(commandId, command.Data, false);
		//Debug.LogFormat ("unreliable try sending command {0} with messageid {1}", command.CommandId, command.MessageId);
	}
	private const float RedudantResendingInterval = 0.050f;
	void UpdateRedudantSendingUnreliable() {
		foreach (SentCommand command in _sentCommands.Values) {
			if (command.LastSendTime + RedudantResendingInterval > Time.time)
				continue;
			SendDataUnreliableTry (command); // Send command over periodically, redudancy for increasing reliability with no impact on latency.
		}
	}
	#endregion
}
