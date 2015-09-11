using UnityEngine;
using System.Collections;

public class LaserDestroy : MonoBehaviour {

	public readonly static int EXIST_TIME = 8;

	public Color c;

	public int counter;
	// Use this for initialization
	void Start () {
		//c = this.GetComponent<LineRenderer>().material.color;
		counter = 0;
	}
	
	// Update is called once per frame
	void Update () {

		if(counter >= EXIST_TIME)
		{
			Destroy(this.GetComponent<LineRenderer>());
		}
		else
		{

			counter ++;

			//c.a = 1 - (float)counter/(float)EXIST_TIME;

			//this.GetComponent<LineRenderer>().SetColors(c,c);
		}
	}
}
