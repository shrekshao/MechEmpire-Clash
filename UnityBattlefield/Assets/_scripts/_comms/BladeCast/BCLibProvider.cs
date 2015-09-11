
/*
Copyright (C) 2015 Electronic Arts Inc.  All rights reserved.
 
This software is solely licensed pursuant to the Hackathon License Agreement,
Available at:  [URL to Hackathon License Agreement].
All other use is strictly prohibited.  
*/


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using SocketIO;


namespace BladeCast
{
	/// <summary>
	/// Lite weight lib provider for unity.
	/// </summary>
	public class BCLibProvider : MonoPersistentSingleton<BCLibProvider>
	{ 
		/// <summary>
		/// RRR message index -- taken from libprovider (new).  Used to translate message
		/// types into callbacks...
		/// </summary>
		enum RRRMessageIndex
		{
			Closed = 0,
			Init = 1,
			Error = 2,
			Logs = 3,
			RegisterController = 4,
			RegisterSTB = 5,
			SetState = 6,
			GameMessage = 7,
			EndGameUserInput = 8,
			GameCommand = 9,
			Ping = 10,
			SystemMessage = 11,
			RegisterGame = 12,
			Mouse = 13,
			Keyboard = 14,
			Joystick = 15,
			Telemetry = 16,
			StoreSuccess = 17,
			StoreFailure = 18
		}

		public delegate void BladecastMessageDelegate (System.IntPtr token,JSONObject contents,int controlIndex);
		
		private	SocketIOComponent			mSocketComponent = null;
		private	BladecastMessageDelegate	mMessageCallback = null;
		private  bool                       mIsConnected = false;
     
		protected override void Awake()
		{
			base.Awake();

			mSocketComponent = SocketIOComponent.Instance;
			mSocketComponent.Init ();
			mSocketComponent.Connect ();

			// setup message received callback...
			if (mSocketComponent != null) {
				mSocketComponent.On ("game_message", OnMessage);
				mSocketComponent.On ("open", OnOpen);
				mSocketComponent.On ("error", OnError);
				mSocketComponent.On ("close", OnClose);
			}
		}

		public bool BladeCast_Game_IsConnected ()
		{
			return mIsConnected;
		}     

		/// <summary>
		/// The message MUST be in a format similar to this...
		/// {"data":{"tid":7,"payload":{<payload>}}}
		/// tid: message type ID (general type)  (7 = game_message). 
		/// payload: the contents of the message.  This is per-game.  
		/// </summary>
		public System.Int32 BladeCast_Game_SendMessage (JSONObject msg)
		{
			JSONObject i = new JSONObject (JSONObject.Type.OBJECT);
			JSONObject j = new JSONObject (JSONObject.Type.OBJECT);

			j.AddField ("tid", (int)RRRMessageIndex.GameMessage);
			j.AddField ("payload", msg);
			i.AddField ("data", j);		

			mSocketComponent.Emit ("game_message", i);
			return 0;
		}   
        	
		public System.Int32 BladeCast_Game_Close ()
		{
			mIsConnected = false;
			return 0;
		}
		
		public System.Int32 BladeCast_Game_RegisterMessageCB (BladecastMessageDelegate cb)
		{
			mMessageCallback = cb;
			return 0;
		}		
        
		/// <summary>
		/// Utility to check if field is in a json object and report error.
		/// </summary>
		public bool FieldCheck(JSONObject json, string fieldName)
		{
			bool retValue = json.HasField (fieldName);
			if (!retValue)
				Debug.LogError (string.Format ("Message Missing Field: \"{0}\" : {1}", fieldName, json.ToString ()));
			return retValue;
		}

		private void OnOpen (SocketIOEvent e)
		{
			if (!mIsConnected) {
				Debug.Log ("[SocketIO] Open received: " + e.name + " " + e.data);
				mIsConnected = true;
				mSocketComponent.Emit ("unity");
			}
		}
        
		private void OnError (SocketIOEvent e)
		{
			Debug.Log ("[SocketIO] Error received: " + e.name + " " + e.data);
		}
        
		private void OnClose (SocketIOEvent e)
		{   
			Debug.Log ("[SocketIO] Close received: " + e.name + " " + e.data);
		}  
           

		/// <summary>
		/// Raises the message event -- handles messages coming from controller(s) to unity by calling message handler.
		/// Dispatches messages based on type.
		/// </summary>
		private void OnMessage (SocketIOEvent e)
		{
			JSONObject msg = e.data;
			RRRMessageIndex tid = (RRRMessageIndex)msg ["tid"].i;				
			switch (tid) {
				case RRRMessageIndex.GameMessage:
					bool validate = true;					
					validate &= FieldCheck(msg, "payload");
					validate &= FieldCheck(msg, "index");					
					if(validate)
						mMessageCallback ((System.IntPtr)(0), msg ["payload"], msg ["index"].i);
					break;
				default:
					Debug.LogError (string.Format ("Unknown message type %d in data: '%s'", tid, msg.ToString ()));
					break;
			}
		}  
	}
}