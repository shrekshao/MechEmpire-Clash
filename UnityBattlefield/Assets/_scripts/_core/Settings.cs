
/*
Copyright (C) 2015 Electronic Arts Inc.  All rights reserved.
 
This software is solely licensed pursuant to the Hackathon License Agreement,
Available at:  [URL to Hackathon License Agreement].
All other use is strictly prohibited.  
*/

using UnityEngine;
using System.Collections;

/// <summary>
/// Settings -- stores game wide settings.
/// </summary>
public class Settings : MonoPersistentSingleton<Settings>
{
	[System.Serializable]
	public class _GameSettings
	{
		// add game wide settings here. These appear in the properties inspector...
		public  float m_newPlayerWaitTime = 5.0f;
	}
	
	[SerializeField] 
	private _GameSettings m_gameSettings;
	
	/// <summary>
	/// Accessor for game settings.  (ex:  "Settings.Game.xxx")
	/// </summary>
	public static _GameSettings Game
	{
		get { return Settings.Instance.m_gameSettings; }
	}
	
	// .. add more setting classes here... 
}
