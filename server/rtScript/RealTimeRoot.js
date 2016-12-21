var CommandsController = require("CommandsControllerModule");

RTSession.getLogger().debug("before register commands");
CommandsController.RegisterCommands();
RTSession.getLogger().debug("after register commands");