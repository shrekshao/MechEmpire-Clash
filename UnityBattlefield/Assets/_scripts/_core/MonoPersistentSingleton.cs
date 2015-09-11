
/*
Copyright (C) 2015 Electronic Arts Inc.  All rights reserved.
 
This software is solely licensed pursuant to the Hackathon License Agreement,
Available at:  [URL to Hackathon License Agreement].
All other use is strictly prohibited.  
*/

using UnityEngine;

/// <summary>
/// Mono Persistant singleton. From http://unitypatterns.com/singletons/
/// This singleton gameobject will persist scene to scen.
/// </summary>
public abstract class MonoPersistentSingleton<T> : MonoBehaviour where T : MonoPersistentSingleton<T>
{
	protected static T m_Instance = null;
	public static T Instance
	{
		get
		{
			if(m_Instance == null) 
			{
				m_Instance = GameObject.FindObjectOfType<T>();
				
				//Tell unity not to destroy this object when loading a new scene!
				DontDestroyOnLoad(m_Instance.gameObject);
			}

			return m_Instance;
		}
	}

	protected virtual void Awake() 
	{
		if(m_Instance == null)
		{
			//If I am the first instance, make me the Singleton
			m_Instance = this as T;
			DontDestroyOnLoad(this);
		}
		else
		{
			//If a Singleton already exists and you find
			//another reference in scene, destroy it!
			if(this != m_Instance)
				Destroy(this.gameObject);
		}
	}
	
	/// <summary>
	/// Clear the reference when the application quits. Override when necessary and call base.OnApplicationQuit() last.
	/// </summary>
	protected virtual void OnApplicationQuit()
	{
		m_Instance = null;
	}	
}