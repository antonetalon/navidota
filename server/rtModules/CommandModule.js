//require("CommandCodes");

module.exports.Command = function(opCode, time, sendersPeer, rtData) {
    this.OpCode = opCode;
    this.Time = time;
    this.sendersPeer = sendersPeer;
    this.rtData = rtData;
}