
/*
Copyright (C) 2015 Electronic Arts Inc.  All rights reserved.
 
This software is solely licensed pursuant to the Hackathon License Agreement,
Available at:  [URL to Hackathon License Agreement].
All other use is strictly prohibited.  
*/


using System;
using UnityEngine;

/// <summary>
/// Application wide manager.  This is an application wide singleton class.  It handles
/// core application functions across all scenes.
/// </summary>
public class ApplicationManager : MonoPersistentSingleton<ApplicationManager>
{
	private static bool	m_isInited = false;

	protected override void Awake ()
	{
		base.Awake ();

		if (!m_isInited) 
		{
			// window mode for streamer...
			Screen.fullScreen = false;

			// Make the game run even when in background
			Application.runInBackground = true;
			m_isInited = true;
		}
	}

	/// <summary>
	/// Shuts down -- kills the game.
	/// </summary>
	public void ShutDown ()
	{
		Debug.Log ("Application Quit!");
		Application.Quit ();	
	}

	/// <summary>
	/// Get or set to pause the game.
	/// </summary>
	public  bool IsPause { get; set; }
	
	/// <summary>
	/// Sets the pause state -- kills the timer (pausing activity) if so.
	/// </summary>
	public void SetPause (bool isPaused)
	{
		IsPause = isPaused;
		Time.timeScale = isPaused ? 0.0f : 1.0f;
	}
}
