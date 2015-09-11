
/*
Copyright (C) 2015 Electronic Arts Inc.  All rights reserved.
 
This software is solely licensed pursuant to the Hackathon License Agreement,
Available at:  [URL to Hackathon License Agreement].
All other use is strictly prohibited.  
*/

using UnityEngine;

namespace BladeCast
{
	/// <summary>
	/// BC output. -- Routes messages to connected controllers.
	/// </summary>
	public class BCOutput : MonoBehaviour
    {
		void Start()
		{
			BCMessenger.Instance.RegisterListener("*", -1, this.gameObject, "RouteControllerMessages");
		}

		/// <summary>
		/// Forwards messages to the controllers... listens for any "controller" messages (msgs > 0)
		/// and sends them.
		/// </summary>
		private void RouteControllerMessages(ControllerMessage msg)
		{
			// just messages for controllers... (-1 dest is for ALL)
			if ((msg.ControllerDest > 0) || (msg.ControllerDest < 0))
			{
				JSONObject json = msg.Payload;
				json.AddField("type", msg.Name);
				json.AddField ("controller_dest", msg.ControllerDest.ToString());
				//Debug.Log ("Game Message Sent: " + json.ToString ());
				BCLibProvider.Instance.BladeCast_Game_SendMessage (json);
			}
        }
    }
}
