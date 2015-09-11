/*
Copyright (C) 2015 Electronic Arts Inc.  All rights reserved.
 
This software is solely licensed pursuant to the Hackathon License Agreement,
Available at:  www.eapathfinders.com/license
All other use is strictly prohibited. 
*/

require('log-timestamp');

var express = require('express');
var app = express();
var http = require('http').Server(app)
var io = require('socket.io')(http);
var path = require('path');
var fs = require('fs');

var port = 8088;
var clients = {};

var printClients = function() {
	var output = "";
	var ping = 100;

	for (var index in clients) {
		if (clients[index] !== null) {
			var latency = typeof clients[index].latency === 'undefined' ? 'N/A' : clients[index].latency + "ms";
			output += "[" + index + ":" + latency + "] ";
		}
	}
	if (output === "") {
		console.log("[Ping] No clients connected.");
	}
	else {
		console.log("[Ping] " + output);
	}
}

setInterval(function() {
	for (var index in clients) {
		if (clients[index] !== null) {
			clients[index].ping = Date.now();
			clients[index].emit('ping');			
		}
	}
	printClients();
}, 5000);

app.use(express.static(__dirname + '/public'));

app.get('/*', function (req, res) {
	res.sendFile(path.join(__dirname+'/public/index.html'));
});

io.on('connection', function(socket) {
	console.log("[Connection] Socket " + socket.id);

	// Save a reference to the Unity socket so that we can send messages to it.
	socket.on("unity", function() {
		clients["unity"] = socket;
		console.log("[Unity] Unity connected. Socket " + socket.id);
	});

	// Save a reference to each controller so we can send messages to just specific ones.
	socket.on("controller", function(msg) {
		var index = msg.toString();
		if (index in clients) {
			if (clients[index] !== null) {
				console.log("[Controller] Controller " + index + " is already connected, but the server recieved another connection request. Terminating old connection and starting new one.");
			}
			else {
				console.log("[Controller] Controller " + index + " re-connected. Socket = " + socket.id);	
			}
			
		}
		else {
			console.log("[Controller] Controller " + index + " connected. Socket = " + socket.id);
		}
		clients[index] = socket;
	});

	socket.on("disconnect", function() {
		if (socket.id === clients["unity"]) {
			console.log("[Unity] Unity has disconnected.");
		}
		else {
			for (var index in clients) {
				if (clients[index] !== null && clients[index].id === socket.id) {
					clients[index] = null;
					console.log("[Controller] Controller " + index + " has disconnected.");					
				}
			}
		}
	});

	socket.on("error", function(e) {
		console.log(e);
	})

	// Send game messages from the controllers to Unity and vice versa.
	socket.on('game_message', function(msg) {		
		if ("unity" in clients) {
			// If the message is from Unity, send it to the controllers.	
			if (socket.id === clients["unity"].id) {
				console.log("[Message] Sending message U -> C: " + JSON.stringify(msg));
				socket.broadcast.emit('game_message', {"data": msg.data});				
			}
			// If the message is from a controller, send it to Unity.
			else {
				console.log("[Message] Sending message C(" + msg.index + ") -> U: " + JSON.stringify(msg));
				clients["unity"].emit("game_message", msg);
			}			
		}
		else {
			console.log("[Warning] Unity hasn't connected yet. Cannot send message. " + JSON.stringify(msg));
		}
	});

	socket.on('pong', function(msg) {
		var now = Date.now();

		for (index in clients) {
			if (clients[index] !== null) {
				if (clients[index].id === socket.id) {
					clients[index].latency = now - socket.ping;
				}				
			}

		}
	});
});

http.listen(port, function() {
	console.log('[Status] Web server running. Connect to ' + require('ip').address() + ":" + port);
});