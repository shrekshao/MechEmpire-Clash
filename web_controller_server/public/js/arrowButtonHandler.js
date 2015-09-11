
/*
 Manages the "arrow button"...
 */

// constants...
var fps = 1;

var BUTTON_UP = 0;
var BUTTON_DOWN = 1;

function ArrowButton(element,av) {
	
	var arrow_value = av;

	var self = this;
	this.m_element = element;
	this.m_state = BUTTON_UP;
	
	

    $(element).on("touchstart", function () {
		self.m_state = BUTTON_DOWN;
        self.handleClick();
    });
	
	
	$(element).on("touchend", function () {
		self.m_state = BUTTON_UP;
        self.handleClick();
    });
    
	
	this.handleClick = function () {
		//console.log("fuck");
		var msg = {
			"type": arrow_value
			,"state": self.m_state
        };
        conn.sendMessage(msg, 0);
	}	


	
}

//# sourceURL=powerupButtonHandler.js