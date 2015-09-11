using UnityEngine;
using System.Collections;
using System;

//Normal bullet(one hit and explode)
public class Bullet : MonoBehaviour {

	public int bullet_type;

	public float move_speed;

	public int damage;

	public int launcher;


	public int protect_time;

	// Use this for initialization
	void Start () {
		//Debug.Log (transform.right);
		//GetComponent<Rigidbody>().AddForce(move_speed *this.transform.parent.right);
		protect_time = 10;
		GetComponent<Rigidbody>().velocity = 20*move_speed * transform.parent.right;
	}
	
	// Update is called once per frame
	void Update () {
		//transform.parent.Translate(move_speed * this.transform.parent.right,Space.World)
		//transform.parent.position = transform.position;
		if(protect_time > 0)
		{
			protect_time -- ;
		}
	}


	virtual protected void OnCollisionEnter(Collision col)
	{

			if(col.gameObject.tag == "Boundary")
			{
				//do nothing
				//Debug.Log("hit wall");
			CreateHitAnimation();
			//Destroy (gameObject);
			Destroy (transform.parent.gameObject);
			}
			else if(col.gameObject.tag =="Mech" && col.gameObject.GetInstanceID() != launcher)
			{

				//TODO:Use Get Component to access script to give damage
					col.gameObject.GetComponent<Engine>().ReceiveDamage(damage);
			CreateHitAnimation();
			//Destroy (gameObject);
			Destroy (transform.parent.gameObject);
				
			}
			
			

	}


	/*
	virtual protected void OnTriggerEnter(Collider other)
	{
		//Debug.Log("hit occurs");
		//Destroy (other);

		if(other.tag == "Boundary")
		{
			//do nothing
			//Debug.Log("normal bullet hit wall");
		}
		else if(other.tag =="Mech")
		{
			//TODO:Use Get Component to access script to give damage
			other.GetComponent<Engine>().ReceiveDamage(damage);
		}
		
		
		
		CreateHitAnimation();
		//Destroy (gameObject);
		Destroy (transform.parent.gameObject);
	}
	*/


	protected void CreateHitAnimation()
	{
		string hit_name ;
		int htype = 0;
		switch(bullet_type)
		{
		case 8:
			htype = 1;
			break;
		case 9:
			htype = 2;
			break;
		case 10:
			htype = 2;
			break;
		case 11:
			htype = 0;
			break;
			
		default:
			htype = bullet_type;
			//htype = 0;
			break;
		}
		hit_name = String.Format( "h{0}", htype);
		//Debug.Log (hit_name);
		GameObject.Instantiate(Resources.Load(hit_name),transform.position,transform.rotation);
	}
	

}
