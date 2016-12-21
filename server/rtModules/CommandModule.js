//require("CommandCodes");

module.exports.Command = function(opCode, time, sendersPeer, rtData) {
    this.OpCode = opCode;
    this.Time = time;
    this.SendersPeer = sendersPeer;
    this.RtData = rtData;
}