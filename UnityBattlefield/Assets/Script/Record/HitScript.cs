using UnityEngine;
using System.Collections;

public class HitScript : MonoBehaviour {
	

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (! this.GetComponent<ParticleSystem>().IsAlive())
		{
			Destroy (GetComponent<ParticleSystem>());
			Destroy (gameObject);
			//Destroy (this);
		}
	}
}
