/*
Copyright (C) 2015 Electronic Arts Inc.  All rights reserved.
 
This software is solely licensed pursuant to the Hackathon License Agreement,
Available at:  www.eapathfinders.com/license
All other use is strictly prohibited. 
*/

/*
 Manages the "Powerup button"...
 */

// constants...
var fps = 1;

var BUTTON_UP = 0;
var BUTTON_DOWN = 1;

function PowerupButton(element) {
	
	var self = this;
	this.m_element = element;
	this.m_state = BUTTON_UP;
	
	this.m_element[0].hidden = true;
	this.timerCount = 0;
	this.isPowerUpEnabled = false;

    $(element).on("touchstart", function () {
		self.m_state = BUTTON_DOWN;
        self.handleClick();
    });
	
    $(element).on("touchend", function () {
		self.m_state = BUTTON_UP;
        self.handleClick();
    });
	
	this.handleClick = function () {
		var msg = {
			"type": "powerup"
        };
		if(!this.isPowerUpEnabled) {
			this.isPowerUpEnabled = true;
			conn.sendMessage(msg, 0);
		}
	}	
	
	this.enablePowerup = function() {
        this.m_element[0].hidden = false;
		console.log(this.m_element[0].childNodes[1].innerHTML);
		this.m_element[0].childNodes[1].innerHTML = "Powerup!"
    };
	
	this.disablePowerup = function() {
		this.isPowerUpEnabled = false;
		this.m_element[0].hidden = true;
    };
	
	this.setPowerupTime = function(val) {
        this.timerCount = val;
		this.m_element[0].childNodes[1].innerHTML = this.timerCount;
		console.log(this.m_element[0].childNodes[1].innerHTML);
    };
}

//# sourceURL=powerupButtonHandler.js