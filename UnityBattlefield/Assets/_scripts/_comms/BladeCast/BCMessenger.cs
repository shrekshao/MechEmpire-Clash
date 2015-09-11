
/*
Copyright (C) 2015 Electronic Arts Inc.  All rights reserved.
 
This software is solely licensed pursuant to the Hackathon License Agreement,
Available at:  [URL to Hackathon License Agreement].
All other use is strictly prohibited.  
*/


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BladeCast
{
	/// <summary>
	/// A message to or from a (game) controller (ie: html5) or any game object that subscribes to it.
	/// </summary>
	public class ControllerMessage
	{
		public int 			ControllerSource 	{get; set;}				// the controller index of the source (0 = game client)
		public int 			ControllerDest		{get; set;}				// the controller index of the dest (0 = game client)
		public string 		Name   				{get; set;}
		public JSONObject  	Payload {get; set;}

		/// <summary>
		/// A controller Message. 
		/// cs -- control source (who is sending the message)  (0 = unity, 1 = cntrl #1... etc)
		/// cd -- control destination (target of message).  (0 = unity (local), 1 = cntrl #1... etc,  -1 is to all)
		/// mn -- message name
		/// payload - the contents of the message (can be null).
		/// </summary>
		public ControllerMessage(int cs, int cd, string mn, JSONObject payload)
		{
			ControllerSource = cs;
			ControllerDest = cd;
			Name = mn;
			Payload = payload;
		}
	}
	
	public class BCMessenger : MonoPersistentSingleton<BCMessenger>
	{
		/// <summary>
		/// Listener -- Defines who is interested in which messages...
		/// </summary>
		private class Listener
		{	
			public string 				MessageType;
			public int					ControllerIndex;
			public GameObject		 	ForwardToObject;
			public string 				ForwardToMethod;

			public Listener(string messageType, int controlIndex, GameObject forwardToObject, string forwardToMethod)
			{
				MessageType = messageType;
				ControllerIndex = controlIndex;
				ForwardToObject = forwardToObject;
				ForwardToMethod = forwardToMethod;
			}
		}

		private List<Listener> listeners = new List<Listener>();

		/// <summary>
		/// Registers a new message listener -- Associates a message handling routine with a particular message type
		/// </summary>
		/// <param name="messageType"> -- Which message(s) this listener handles ("*" for all)</param>
		/// <param name="ControlIndex"> -- The index of the controller(s) destination that this listener handles (-1) for all </param>
		/// <param name="ForwardObject"> -- A reference to the instance (gameobject) that will handle the messages </param>
		/// <param name="ForwardToMethod"> -- The name of the method that will handle the messages </param>
		public void RegisterListener(string messageType, int controlIndex, GameObject forwardToObject, string forwardToMethod)
		{
			Listener listener = new Listener(messageType, controlIndex, forwardToObject, forwardToMethod);
			listeners.Add (listener);
		}

		/// <summary>
		/// "controllerDest" of "m" is the target controller... 0 (for unity), 1, 2, 3...  (if -1 then for ALL controllers)
		/// "controllerIndex" of l is the receiving controller.  Same as dest...  -1 is wildcard for all messages
		/// </summary>
		public void SendToListeners(ControllerMessage m)
		{
	        foreach (var f in listeners.FindAll(l => ((m.ControllerDest < 0) || (l.ControllerIndex < 0) ||
	                                                  (l.ControllerIndex == m.ControllerDest)) &&
			                                    	 ((l.MessageType == "*") || (l.MessageType == m.Name)) ))  
			{    
			    f.ForwardToObject.gameObject.BroadcastMessage(f.ForwardToMethod, m, SendMessageOptions.DontRequireReceiver);
			}
		}

	    /// <summary>
	    /// Sends to listeners -- simple form.  Message only contains "name"
	    /// </summary>
	    public void SendToListeners(string messageName, int cd)
	    {
	        JSONObject msgOut = new JSONObject (JSONObject.Type.OBJECT);
	        SendToListeners (new ControllerMessage (0, cd, messageName, msgOut));
	    }

	    /// <summary>
	    /// Sends to listeners -- simple form.  Same, but with a single string field
	    /// </summary>
	    public void SendToListeners(string messageName, string field1Name, string field1, int cd)
	    {
	        JSONObject msg = new JSONObject (JSONObject.Type.OBJECT);
	        msg.AddField (field1Name, field1);    
	        SendToListeners (new ControllerMessage (0, cd, messageName, msg));
	    }

	    /// <summary>
	    /// Sends to listeners -- simple form.  Same, but with a single bool field
	    /// </summary>
	    public void SendToListeners(string messageName, string field1Name, bool field1, int cd)
	    {
	        JSONObject msg = new JSONObject (JSONObject.Type.OBJECT);
	        msg.AddField (field1Name, field1);    
	        SendToListeners (new ControllerMessage (0, cd, messageName, msg));
	    }
	 
	    /// <summary>
	    /// Sends to listeners -- simple form.  Same, but with a single int field
	    /// </summary>
	    public void SendToListeners(string messageName, string field1Name, int field1, int cd)
	    {
	        JSONObject msg = new JSONObject (JSONObject.Type.OBJECT);
	        msg.AddField (field1Name, field1);    
	        SendToListeners (new ControllerMessage (0, cd, messageName, msg));
	    }

	    /// <summary>
	    /// Sends to listeners -- simple form.  Same, but with a single float field
	    /// </summary>
	    public void SendToListeners(string messageName, string field1Name, float field1, int cd)
	    {
	        JSONObject msg = new JSONObject (JSONObject.Type.OBJECT);
	        msg.AddField (field1Name, field1);    
	        SendToListeners (new ControllerMessage (0, cd, messageName, msg));
	    }

		public void RemoveListener(GameObject toRemoveObject)
		{

			for(int i = 0; i<listeners.Count; i++)
			{
				if(listeners.ElementAt(i).ForwardToObject == toRemoveObject)
				{
					listeners.RemoveAt(i);
				}
			}

		}
	}
}
