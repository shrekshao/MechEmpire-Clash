/*
Copyright (C) 2015 Electronic Arts Inc.  All rights reserved.
 
This software is solely licensed pursuant to the Hackathon License Agreement,
Available at:  www.eapathfinders.com/license
All other use is strictly prohibited. 
*/

/*
 Manages the "TouchBox"...
 */

// constants...
var fps = 1;

var OUTBOUNDS = 0;
var INBOUNDS = 1;

function TouchBox(element) {
	var self = this;
	this.m_element = element;
	this.m_state = OUTBOUNDS;
	
	$(element).on('touchmove', function (e) {
		
		console.log("entered touchmove")
		if (self.m_state == INBOUNDS) {
			touchobj = e.originalEvent.changedTouches[0];
			var touchX = parseInt(touchobj.clientX);
			var touchY = parseInt(touchobj.clientY);
			
			
			var $player = $("#player");
			var offset = $player.offset();
			var width = $player.width();
			var height = $player.height();

			var centerX = offset.left + width / 2;
			var centerY = offset.top + height / 2;
			
			var angle = Math.atan2((touchX - centerX),(touchY - centerY)) + Math.PI;
			self.sendAngleMessage(angle);
		}
    });
	
	this.sendAngleMessage = function (angle) {
		var msg = {
			"type": "angle",
			"angle": angle
        };
        conn.sendMessage(msg, 0);
	}	
	
	$(element).on("touchstart", function () {
		self.m_state = INBOUNDS;
		//self.sendShootMessage(self.m_state);
    });
	
    $(element).on("touchend", function () {
		self.m_state = OUTBOUNDS;
		self.sendShootMessage(self.m_state);
    });
	
	this.sendShootMessage = function (state) {
		var msg = {
			"type": "shoot",
			"state": state
        };
        conn.sendMessage(msg, 0);
	}	
}

//# sourceURL=touchBoxHandler.js