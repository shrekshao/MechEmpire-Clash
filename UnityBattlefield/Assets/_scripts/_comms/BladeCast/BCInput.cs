
/*
Copyright (C) 2015 Electronic Arts Inc.  All rights reserved.
 
This software is solely licensed pursuant to the Hackathon License Agreement,
Available at:  [URL to Hackathon License Agreement].
All other use is strictly prohibited.  
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Handles input messages comming from the controller(s)  These messages are queued on input and then
/// processed in the update handler (to avoid thread problems).  
/// </summary>
namespace BladeCast
{ 
	public class BCInput : MonoBehaviour
	{
		private Queue 									msgQueue;
		private Queue 									syncedMsgQueue;

		void Start()
		{
			// example of a message listener.  The below listener handles the "connect" type message.
			BCMessenger.Instance.RegisterListener ("connect", 0, this.gameObject, "HandleNewConnection");

			// setup communications with controllers...
			{
				// Set up the message queues
				msgQueue = new Queue ();
				syncedMsgQueue = Queue.Synchronized (msgQueue);

				//gameMessageCB = new BCLibProvider.BladecastMessageDelegate (OnGameMessage);
				BCLibProvider.Instance.BladeCast_Game_RegisterMessageCB (OnGameMessage);
			}
		}

		/// <summary>
		/// Messages are added to the queue in BCConnection. Here, we check if there are any messages
		/// and process them in the main thread.
		/// Messages need to be in the following format:
		/// {"type":"go_action", "index": 1}
		/// index -- the index of the controller that is sending the message (0 = unity, 1 = ctrl 1...)
		/// type -- the type of message (
		/// payload:
		/// </summary>
		void Update ()
		{
			// We check the count here so that we don't lock the queue just to find out it's empty.
			while (syncedMsgQueue.Count > 0)
			{
				lock (syncedMsgQueue)
				{
					if (syncedMsgQueue.Count > 0)
					{
						JSONObject json = syncedMsgQueue.Dequeue () as JSONObject;

						if (json != null) 
						{
							bool validate = true;
							validate &= BCLibProvider.Instance.FieldCheck(json, "index");
							validate &= BCLibProvider.Instance.FieldCheck(json, "type");
							if(validate)
								BCMessenger.Instance.SendToListeners (new ControllerMessage (json ["index"].i, 0, json ["type"].str, json));
						} 
						else 
						{
							Debug.LogError ("Non JSON Object as Messge in BCInput");
						}
					}
				}
			}
		}

		// Handle LibProviderGame callbacks
		private void OnGameMessage (System.IntPtr token, JSONObject json, int controllerIndex)
		{
			lock (syncedMsgQueue)
			{
				// adding the controller index into payload!
				json.AddField ("index", controllerIndex);
				syncedMsgQueue.Enqueue (json);
			}
			//Debug.Log("Game Message Received: " + contents);
		}

		/// <summary>
		/// Handles the new connection message
		/// </summary>
		private void HandleNewConnection (ControllerMessage msg)
		{
			Debug.Log ("New Connection: " + msg.ControllerSource);
		}	       
	}
}
