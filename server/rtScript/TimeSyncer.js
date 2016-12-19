RTSession.onPlayerConnect(function(player){
    RTSession.getLogger().debug("This Is A Debug Message...");
    RTSession.newPacket().setOpCode(100).setTargetPeers().send(); // send an empty pack back to all players
});

RTSession.onPacket(1, function(packet){
    RTSession.getLogger().debug("This Is A Debug Message...");
    var rtData = RTSession.newData();
    RTSession.newPacket().setOpCode(2).setData(rtData).setTargetPeers(packet.getSender()).send();
    //RTSession.newPacket().setOpCode(2).setTargetPeers().send();
});