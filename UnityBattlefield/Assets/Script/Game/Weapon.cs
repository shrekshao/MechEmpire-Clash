using UnityEngine;
using System.Collections;
using System;

public class Weapon : MonoBehaviour {
	//weapon attributes
	public int weapon_type;

	public float rotational_speed;
	public float cooling;

	public float max_cooling = 100;




	public Transform fire_position;



	public float target_rotation = 0;


	//public float _r,_g,_b;

	// Use this for initialization
	void Start () {

		fire_position = transform.FindChild("fire_position");

		ColorRandom();

		cooling = 0;
	}





	// Update is called once per frame
	void Update () {
		if(cooling >0)
		{
			cooling --;
		}
	}



	virtual public void Fire(int launcher)
	{
		if(cooling > 0)
		{
			return;
		}

		cooling = max_cooling;

		string bullet_name = String.Format( "b{0}", weapon_type);

		GameObject b = GameObject.Instantiate(Resources.Load(bullet_name),fire_position.position, Quaternion.Euler(Vector3.up * transform.eulerAngles.y)) as GameObject;

		b.GetComponentInChildren<Bullet>().launcher = launcher;

		GetComponent<AudioSource>().Play();
	}


	public void SimpleRotate(float r)
	{
		transform.Rotate(new Vector3(0,r,0));
	}


	public void AimAtDirection(float rr)
	{
		while(rr < 0)
		{
			rr += 360;
		}

		float c=0,a=0;		//clockwise顺时针，anticlockwise逆时针
		float rotation = transform.localEulerAngles.y;
		if(rr>rotation)
		{
			c=rr-rotation;
			a=rotation+360-rr;	//(rotation-(-180))+(180-rr)
		}
		else
		{
			a=rotation-rr;
			c=360-rotation+rr;	//(180-rotation)+(rr-(-180));
		}


		if(c>a)
		{
			if(Mathf.Abs(a)>rotational_speed)
			{
				transform.Rotate(new Vector3(0,-rotational_speed,0));
			}
			else
			{
				transform.localEulerAngles = new Vector3(0,rr,0);
			}
		}
		else
		{
			if(Mathf.Abs(c)>rotational_speed)
			{
				transform.Rotate(new Vector3(0,rotational_speed,0));
			}
			else
			{
				transform.localEulerAngles = new Vector3(0,rr,0);
			}
		}
	}



	public void ColorRandom()
	{
		
		
		Renderer[] renderer_list = gameObject.GetComponentsInChildren<Renderer>();
		float r = UnityEngine.Random.value;
		float g = UnityEngine.Random.value;
		float b = UnityEngine.Random.value;
		foreach (Renderer renderer in renderer_list)
		{
			foreach (Material m in renderer.materials)
			{
				m.SetColor("_Color",new Color(r,g,b));
			}
		}

	}


}
