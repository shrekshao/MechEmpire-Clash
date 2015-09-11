function Connection (onGameMessage) {

    var getIndex = function() {
        var href = window.location.href;
        var index = href.substr(href.lastIndexOf('/') + 1)

        if (!isNaN(index) && parseInt(Number(index)) == index && !isNaN(parseInt(index,10))) {
            return parseInt(index);
        }
        else {
            throw new Error(index + " is not a valid controller index. Specify a controller index as an integer.");
        }

    };

    this.connected = null;
    this.socket = io();

    this.index = getIndex();

    this.socket.on('game_message', Connection.prototype.onMessage.bind(this));
    this.socket.on('connect', Connection.prototype.onOpen.bind(this));
    this.socket.on('disconnect', Connection.prototype.onClose.bind(this));
    this.socket.on('error', Connection.prototype.onError.bind(this));
    this.socket.on('ping', function() {
        this.socket.emit('pong');
    }.bind(this));
    // identify this as a controller to the web server.
    this.socket.emit('controller', this.index);
};

// All the different types of messages we can receive and categorize.
// The most important one is GameMessage, which is how all JSON game messages are sent.
Connection.prototype.messageIndex = {
    Closed: 0,
    Init: 1,
    Error: 2,
    Logs: 3,
    RegisterController: 4,
    RegisterSTB: 5,
    SetState: 6,
    GameMessage: 7,
    EndGameUserInput: 8,
    GameCommand: 9,
    Ping: 10,
    SystemMessage: 11,
    RegisterGame: 12,
    Mouse: 13,
    Keyboard: 14,
    Joystick: 15,
    Telemetry: 16,
    StoreSuccess: 17,
    StoreFailure: 18
};

//--------------------------------------------------------------------
// Event handler - socket open
//--------------------------------------------------------------------
Connection.prototype.onOpen = function () {
    console.log("Websocket Open");
    this.connected = true;
};

//--------------------------------------------------------------------
// Event handler - socket close
//--------------------------------------------------------------------
Connection.prototype.onClose = function () {
    console.log("Websocket Closed");
    this.connected = false;
};

//--------------------------------------------------------------------
// Event handler - socket error
//--------------------------------------------------------------------
Connection.prototype.onError = function (err) {
   console.log("---------------- onError:", err.code);
};

//--------------------------------------------------------------------
// Send a message.
// The message must be valid JSON, and must include a "tid" field in that JSON.
//--------------------------------------------------------------------
Connection.prototype.sendMessage = function (message) {
    var msg = {};
    msg.index = this.index;
    msg.payload = message;
    msg.tid = Connection.prototype.messageIndex.GameMessage;
    console.log("Sending message C(" + msg.index + ") -> U: " + JSON.stringify(msg));
    this.socket.emit('game_message', msg);
};


//--------------------------------------------------------------------
// Triggered when new message received from WS server
//--------------------------------------------------------------------
Connection.prototype.onMessage = function (msg) {
    var message = msg.data;
    var destination = parseInt(message.payload.controller_dest);
    //console.log('Received message: ' + JSON.stringify(message));

    // First check that this message was meant for us. Messages with a controller_dest of -1 are meant for everyone.
    if (destination === -1 || destination === this.index) {
        // Now check that this is a game message.
        if (message.tid === Connection.prototype.messageIndex.GameMessage) {
            $( document ).trigger( "game_message", [ message ] );
        }       
    }
};
