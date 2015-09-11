
/*
Copyright (C) 2015 Electronic Arts Inc.  All rights reserved.
 
This software is solely licensed pursuant to the Hackathon License Agreement,
Available at:  [URL to Hackathon License Agreement].
All other use is strictly prohibited.  
*/

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Base class for application wide singleton-type behavior
/// From: http://stackoverflow.com/questions/100081/whats-a-good-threadsafe-singleton-generic-template-pattern-in-c-sharp
/// </summary>
public class Singleton<T> where T : class, new()
{
	private static object _syncobj = new object();
	private static volatile T _instance = null;
	
	// non-lazy init.
	public static void Create()
	{
		#pragma warning disable 219
		T unused = Instance;
		#pragma warning restore 219
	}
	
	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				lock (_syncobj)
				{
					if (_instance == null)
					{
						_instance = new T();
					}
				}
			}
			return _instance;
		}
	}
	
	
	// non-lazy init.
	public static void Destroy()
	{
		_instance = null;
	}
}
