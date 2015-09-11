using UnityEngine;
using System.Collections;
using System;

public class Engine : MonoBehaviour {

	public int engine_type;
	public float move_speed = 5f;
	public float rotational_speed = 1f;
	public float max_speed;

	public int hp;

	public int max_hp;


	public Robot robot_script;
	

	// Use this for initialization
	void Start () {
		//Debug.Log("fuck");
		hp = max_hp;
		ColorRandom();
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void ReceiveDamage(int damage)
	{
		Debug.Log (String.Format( "Receive Damage {0}", damage));
		hp -= damage;
		//TODO: hp < 0 events
		if(hp <= 0)
		{
			Damaged ();
		}
	}

	public void Damaged()
	{
		//transform.parent.gameObject.GetComponent<Robot>().Damaged();
		robot_script.Damaged();
		//Destroy (gameObject);

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
