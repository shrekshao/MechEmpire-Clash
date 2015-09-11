using UnityEngine;
using System.Collections;
using System;

public class RobotRecord {
	public readonly static float[] BULLET_HEIGHT = {25,25,30,25,20,20,20,20,20,20,0,20,25};
	public readonly static float[] ENGINE_HEIGHT = {33,50,25,38
		,25,25,38,35};

	private float t;
	public float h;

	private Vector3 cf_position;	//current frame position
	private Vector3 nf_position;	//next frame position
	//private Vector3 frame_move;			//current frame translate vector
	public Vector3 position;		//current physical position

	private Quaternion cf_weapon_quaternion;
	private Quaternion nf_weapon_quaternion;
	//private Quaternion from_to_weapon_quaternion;
	private Quaternion cf_engine_quaternion;
	private Quaternion nf_engine_quaternion;
	//private Quaternion from_to_engine_quaternion;

	private GameObject weapon;
	private GameObject engine;

	public GameObject camera;

	private float _wr;
	private float _wg;
	private float _wb;
	private float _er;
	private float _eg;
	private float _eb;

	// Use this for initialization
	public RobotRecord (int robot_id,int wtn,int etn,int wr,int wg,int wb,int er,int eg,int eb) 
	{
		t = 0;

		string weapon_name = String.Format( "w{0}", wtn);
		string engine_name = String.Format( "e{0}", etn);

		weapon = GameObject.Instantiate(Resources.Load(weapon_name)) as GameObject;
		engine = GameObject.Instantiate(Resources.Load(engine_name)) as GameObject;



		_wr = ((float)wr+80+255)/512;
		_wg = ((float)wg+80+255)/512;
		_wb = ((float)wb+80+255)/512;
		_er = ((float)er+80+255)/512;
		_eg = ((float)eg+80+255)/512;
		_eb = ((float)eb+80+255)/512;

		//h = engine.GetComponentInChildren<Renderer>().bounds.size.y;
		//h = engine.transform.Find("model").transform.renderer.bounds.size.y;
		h = ENGINE_HEIGHT[etn];

		camera = GameObject.Instantiate(Resources.Load("fpscamera")) as GameObject;

		ColorReset();

		//camera.name = String.Format( "fpscamera{0}", robot_id);
		//tmp size test
		//weapon.transform.localScale = new Vector3(1,1,1);
		//float w_radium = 95 / weapon.renderer.bounds.size.x;
		//weapon.transform.localScale = new Vector3 (w_radium,w_radium,w_radium);

		//engine.transform.localScale = new Vector3(1,1,1);
		//float e_radium = 2 * 50 / engine.renderer.bounds.size.x;
		//engine.transform.localScale = new Vector3 (e_radium,e_radium,e_radium);
		//////

		//h = engine.renderer.bounds.size.y;

	}

	public void Update()
	{
		//position += (Time.deltaTime / Time.fixedDeltaTime) * frame_move;
		t += Time.deltaTime / Time.fixedDeltaTime;
		position = (1-t) * cf_position + t * nf_position;

		weapon.transform.position = position;
		engine.transform.position = position;


		weapon.transform.rotation = Quaternion.Slerp(cf_weapon_quaternion,nf_weapon_quaternion,t);
		engine.transform.rotation = Quaternion.Slerp(cf_engine_quaternion,nf_engine_quaternion,t);

		camera.transform.position = weapon.transform.position;
		camera.transform.rotation = weapon.transform.rotation;
	}

	public void SetNextFramePosition(float x,float y,float z,float w_rotation,float e_rotation)
	{
		t = 0;
		cf_position = nf_position;
		position = cf_position;
		nf_position = new Vector3(x,h+y,z);
		//frame_move = nf_position - cf_position;

		cf_weapon_quaternion = nf_weapon_quaternion;
		nf_weapon_quaternion = Quaternion.Euler(Vector3.up * w_rotation);
		weapon.transform.rotation = cf_weapon_quaternion;

		camera.transform.position = weapon.transform.position;
		camera.transform.rotation = weapon.transform.rotation;


		cf_engine_quaternion = nf_engine_quaternion;
		nf_engine_quaternion = Quaternion.Euler(Vector3.up * e_rotation);
		engine.transform.rotation = cf_engine_quaternion;
	}

	public void TriggerActions(int fire,int wturn,int run,int eturn)
	{
		if (fire == 1)
		{
			weapon.GetComponent<AudioSource>().Play();
		}
	}

	/*
	public void SetPosition(Vector3 v3)
	{
		cf_position = new Vector3(v3.x,v3.y,v3.z);
		weapon.transform.position = cf_position;
		engine.transform.position = cf_position;
		nf_position = cf_position;
	}
	*/

	public void SetPosition(float x,float y,float z,float w_rotation,float e_rotation)
	{
		cf_position = new Vector3(x,h+y,z);
		weapon.transform.position = cf_position;
		engine.transform.position = cf_position;
		nf_position = cf_position;
		position = cf_position;


		cf_weapon_quaternion = Quaternion.Euler(Vector3.up * w_rotation);
		weapon.transform.rotation = cf_weapon_quaternion;
		nf_weapon_quaternion = cf_weapon_quaternion;


		cf_engine_quaternion = Quaternion.Euler(Vector3.up * e_rotation);
		engine.transform.rotation = cf_engine_quaternion;
		nf_engine_quaternion = cf_engine_quaternion;
	}

	public float GetLaunchingHeight(int btn)
	{
		return (h + BULLET_HEIGHT[btn]);
	}

	public void explode()
	{
		GameObject.Destroy(weapon);
		GameObject.Destroy(engine);
		GameObject.Instantiate(Resources.Load("h2")
		                       ,cf_position
		                       ,Quaternion.identity);
	}

	public void ColorReset()
	{
		
		
		Renderer[] weapon_renderer_list = weapon.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in weapon_renderer_list)
		{
			foreach (Material m in renderer.materials)
			{
				m.SetColor("_Color",new Color(_wr,_wg,_wb));
			}
		}
		
		Renderer[] engine_renderer_list = engine.GetComponentsInChildren<Renderer>();
		foreach (Renderer r in engine_renderer_list)
		{
			foreach (Material m in r.materials)
			{
				m.SetColor("_Color",new Color(_er,_eg,_eb));
			}
		}
	}
}
