using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.RT;
using System;

public class ReceivedCommandsFactory {
	Dictionary<int, Func<RTData, MatchCommand>> _parsers = new Dictionary<int, Func<RTData, MatchCommand>>();
	Dictionary<MatchCommand, int> _opCodesByCommands = new Dictionary<MatchCommand, int>();
	protected void AddCommandParser(int opCode, Func<RTData, MatchCommand> singleCommandParser) {
		_parsers.Add(opCode, singleCommandParser);
		MatchCommand shallowCommand = singleCommandParser(null);
		_opCodesByCommands.Add(shallowCommand, opCode);
	}
	public int GetOpCode(Type t) {
		foreach (var item in _opCodesByCommands) {
			if (item.Key.GetType() == t)
				return item.Value;
		}
		return -1;
	}
	public MatchCommand ParseCommand(int opCode, RTData data) {
		Func<RTData, MatchCommand> parser = null;
		if (!_parsers.TryGetValue(opCode, out parser)) {
			Debug.LogError("Unrecognised command received, opCode = " + opCode.ToString());
			return null;
		}
		MatchCommand command = parser(data);
		return command;
	}
}
