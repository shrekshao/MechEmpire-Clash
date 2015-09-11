using UnityEngine;
using System.Collections;
using System;

public class Robot : MonoBehaviour {
	public readonly static float[] ENGINE_HEIGHT = {33,50,25,38
		,25,25,38,35};


	public int playerID;

	public GameObject fps_camera;

	public GameManager game_manager = null;

	public int weapon_type = 11;
	public GameObject weapon;

	protected Weapon weapon_script;


	public int engine_type = 0;
	public GameObject engine;

	protected Engine engine_script;


	public void SetWeapon(int wtn)
	{
		weapon_type = wtn;
		if(weapon != null)
		{
			//destroy previous one
			Destroy (weapon);
			weapon = null;
		}
		string weapon_name = String.Format( "w{0}", weapon_type);
		weapon = this.Instantiate(Resources.Load(weapon_name),transform.position,transform.rotation) as GameObject;
		weapon.transform.parent = engine.transform;

		weapon_script = weapon.GetComponentInChildren<Weapon>();

		//fps_camera = GameObject.Instantiate(Resources.Load("fpscamera"),transform.position,transform.rotation) as GameObject;
		fps_camera = GameObject.Instantiate(Resources.Load("fpscamera"),weapon.transform.position,weapon.transform.rotation) as GameObject;
		fps_camera.transform.parent = weapon.transform;
		//fps_camera.GetComponentInChildren<AudioListener>().enabled = false;
		//fps_camera.GetComponentInChildren<Camera>().enabled = false;
		//print (fps_camera.GetComponentInChildren<Camera>());
		GameObject camera = fps_camera.transform.FindChild("Camera").gameObject;
		//camera.GetComponent<Camera>().enabled = false;
		camera.GetComponent<AudioListener>().enabled = false;

		camera.GetComponent<Camera>().pixelRect = GameManager.rect_ary[playerID];//new Rect(50,100,50,100);
	}

	public void SetEngine(int etn)
	{
		engine_type = etn;
		if(engine_type != null)
		{
			//destroy previous one
			Destroy (engine);
			engine = null;
		}
		string engine_name = String.Format( "e{0}", engine_type);
		engine = this.Instantiate(Resources.Load(engine_name),transform.position,transform.rotation) as GameObject;
		//engine.transform.parent = transform;
		transform.parent = engine.transform;
		
		//transform.parent.Translate(Vector3.up * engine.GetComponentInChildren<Renderer>().bounds.size.y);

		engine.transform.position = new Vector3(transform.position.x
		                                 //,engine.GetComponentInChildren<Renderer>().bounds.size.y * 2f
		                                        ,ENGINE_HEIGHT[etn]
		                                 ,transform.position.z);

		//engine_script = engine.GetComponentInChildren<Engine>();
		engine_script = engine.GetComponent<Engine>();
		//Debug.Log(engine_script.engine_type);
		engine_script.robot_script = this;//GetComponent<Robot>();
		                                 
	}




	// Use this for initialization
	protected void Start () {
		//weapon = GameObject.Instantiate(Resources.Load("WeaponControl")) as GameObject;
		//weapon.GetComponent<Transform>().parent = this.transform;
		//engine = GameObject.Instantiate(Resources.Load("EngineControl")) as GameObject;
		//engine.GetComponent<Transform>().parent = this.transform;

		//SetEngine(engine_type);
		//SetWeapon(weapon_type);






	}


	// Update is called once per frame
	void Update()
	{
		//weapon.transform.position = engine.transform.position;
		fps_camera.transform.FindChild("Camera").gameObject.GetComponent<Camera>().pixelRect = GameManager.rect_ary[playerID];
	}

	virtual public void Damaged()
	{
		GameObject.Instantiate(Resources.Load("h2")
		                       ,transform.position
		                       ,Quaternion.identity);


		if(game_manager!=null)
		{
			game_manager.RemoveRobot(this.gameObject.GetInstanceID(),playerID);
		}

		Destroy(weapon);
		Destroy (engine);
		Destroy (gameObject);

	}

	/*
	public float sensitivityX = 15F;
	void Update () {
		float rotate_y = Input.GetAxis("Mouse X") * sensitivityX;
		weapon.transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
	}
	*/
}
