using UnityEngine;
using System.Collections;
using BladeCast;

public class RobotPlayer : Robot {



	public int control_type = 0;	//0 - mouse&keyboard, 1 - html5 remote
	public int remote_id = 0;		//remote controller url id

	public int w_down = 0;
	public int s_down = 0;
	public int d_down = 0;
	public int a_down = 0;

	void Start()
	{
		base.Start();


		//InitControllerListeners();


		/*
		//temp camera activation
		Camera[] camera_list;
		camera_list = new Camera[2];
		Camera.GetAllCameras(camera_list);
		//SwitchCamera(1);
		camera_list[0].enabled = false;
		camera_list[1].enabled = true;
		*/
	}

	//-------------------------------------
	//EA js remote controller
	//-------------------------------------
	void InitControllerListeners()
	{


		BCMessenger.Instance.RegisterListener("angle", 0, this.gameObject, "HandleRotate_ControllerInput");
		//BCMessenger.Instance.RegisterListener("powerup", 0, this.gameObject, "HandlePowerup_ControllerInput");
		BCMessenger.Instance.RegisterListener("shoot", 0, this.gameObject, "HandleShoot_ControllerInput");

		//TODO: move
		BCMessenger.Instance.RegisterListener("w", 0, this.gameObject, "HandleW_ControllerInput");
		BCMessenger.Instance.RegisterListener("s", 0, this.gameObject, "HandleS_ControllerInput");
		BCMessenger.Instance.RegisterListener("d", 0, this.gameObject, "HandleD_ControllerInput");
		BCMessenger.Instance.RegisterListener("a", 0, this.gameObject, "HandleA_ControllerInput");
	}

	public void InitControl(int ct,int rt)
	{
		control_type = ct;
		remote_id = rt;
		if(control_type == 1)
		{
			//EA
			InitControllerListeners();
		}
	}




	void HandleRotate_ControllerInput(ControllerMessage msg) {
		if(msg.ControllerSource != remote_id)
		{
			return;
		}

		if (msg.Payload.HasField ("angle")) {
			string angle_value_raw = msg.Payload.GetField("angle").ToString();
			float angle_value_parsed;
			if(float.TryParse(angle_value_raw, out angle_value_parsed)) {
				weapon_script.AimAtDirection(-Mathf.Rad2Deg*angle_value_parsed);
				//weapon_script.SimpleRotate(-Mathf.Rad2Deg*angle_value_parsed*0.01f);
			}
		} else {
			print ("angle field did not exist");
		}
		//print(msg.ControllerSource);
	}

	void HandleShoot_ControllerInput(ControllerMessage msg) {
		if(msg.ControllerSource != remote_id)
		{
			return;
		}

		weapon_script.Fire(engine.GetInstanceID());
	}




	void HandleW_ControllerInput(ControllerMessage msg)
	{
		if(msg.ControllerSource != remote_id)
		{
			return;
		}
		string str_state = msg.Payload.GetField("state").ToString();
		int tmp;
		if(int.TryParse(str_state, out tmp)) {
			w_down = tmp;
		}

		//print ("w"+w_down);
	}

	void HandleS_ControllerInput(ControllerMessage msg)
	{
		if(msg.ControllerSource != remote_id)
		{
			return;
		}
		string str_state = msg.Payload.GetField("state").ToString();
		int tmp;
		if(int.TryParse(str_state, out tmp)) {
			s_down = tmp;
		}

		//print ("s"+tmp);
	}


	void HandleD_ControllerInput(ControllerMessage msg)
	{
		if(msg.ControllerSource != remote_id)
		{
			return;
		}
		string str_state = msg.Payload.GetField("state").ToString();
		int tmp;
		if(int.TryParse(str_state, out tmp)) {
			d_down = tmp;
		}
		//print ("d"+tmp);
	}

	void HandleA_ControllerInput(ControllerMessage msg)
	{
		if(msg.ControllerSource != remote_id)
		{
			return;
		}
		string str_state = msg.Payload.GetField("state").ToString();
		int tmp;
		if(int.TryParse(str_state, out tmp)) {
			a_down = tmp;
		}
		//print ("a"+tmp);
	}
	//--------------------------------------











	// Update is called once per frame
	public float sensitivityX = 15F;
	void Update () {
		//weapon

		//facing (used for spider and quad)




		if(weapon!=null && control_type == 0)
		{

			//keyboard & mouse
			float rotate_y = Input.GetAxis("Mouse X") * sensitivityX;
			weapon.transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);

			if (Input.GetMouseButtonDown(0))
			{
				//left click
				//Debug.Log (weapon_script);
				weapon_script.Fire (engine.GetInstanceID());
			}
			
		}



			//engine

		if(engine!=null)
		{
			//the move part should be in engine
			//MoveUp(),...
			switch(engine_type)
			{
			case 0:
			case 4:
				//spider, quad
				float weapon_ry;
				Vector3[] dir_ary = new Vector3[4];
				weapon_ry = weapon.transform.rotation.eulerAngles.y;

					if( weapon_ry < 45f || weapon_ry > 315f)
					{
						dir_ary[0] = Vector3.right;
						dir_ary[1] = Vector3.left;
						dir_ary[2] = Vector3.back;
						dir_ary[3] = Vector3.forward;
					}
					else if (weapon_ry < 135f && weapon_ry > 45f)
					{
						dir_ary[0] = Vector3.back;
						dir_ary[1] = Vector3.forward;
						dir_ary[2] = Vector3.left;
						dir_ary[3] = Vector3.right;
					}
					else if (weapon_ry < 225f && weapon_ry > 135f)
					{
						dir_ary[0] = Vector3.left;
						dir_ary[1] = Vector3.right;
						dir_ary[2] = Vector3.forward;
						dir_ary[3] = Vector3.back;
					}
					else 
					{
						//if (weapon_ry < 315f && weapon_ry > 225f)
						dir_ary[0] = Vector3.forward;
						dir_ary[1] = Vector3.back;
						dir_ary[2] = Vector3.right;
						dir_ary[3] = Vector3.left;
					}



				if ( (Input.GetKey(KeyCode.W)&&control_type==0) || (w_down==1 && control_type==1))
				{
					//engine.transform.Translate(Vector3.right * engine_script.move_speed * Time.deltaTime *10 ,Space.World);
					engine.transform.Translate(dir_ary[0] * engine_script.move_speed * Time.deltaTime *10 ,Space.World);
				}
				
				if (Input.GetKey(KeyCode.S) && control_type==0 || s_down==1 && control_type==1)
				{
					//engine.transform.Translate(Vector3.left * engine_script.move_speed * Time.deltaTime*10,Space.World);
					engine.transform.Translate(dir_ary[1] * engine_script.move_speed * Time.deltaTime *10 ,Space.World);
				}
				
				if (Input.GetKey(KeyCode.D) && control_type==0 || d_down==1 && control_type==1)
				{
					//engine.transform.Translate(Vector3.back * engine_script.move_speed * Time.deltaTime*10,Space.World);
					engine.transform.Translate(dir_ary[2] * engine_script.move_speed * Time.deltaTime *10 ,Space.World);
				}
				
				if (Input.GetKey(KeyCode.A) && control_type==0 || a_down==1 && control_type==1)
				{
					//engine.transform.Translate(Vector3.forward * engine_script.move_speed * Time.deltaTime*10,Space.World);
					engine.transform.Translate(dir_ary[3] * engine_script.move_speed * Time.deltaTime *10 ,Space.World);
				}
				break;

			case 2:
			case 5:
				//tanks
				if (Input.GetKey(KeyCode.W) && control_type==0 || w_down==1 && control_type==1)
				{
					engine.GetComponent<Rigidbody>().velocity += engine_script.move_speed * engine.transform.right;
					float v = engine.GetComponent<Rigidbody>().velocity.magnitude;
					if( v > engine_script.max_speed)
					{
						engine.GetComponent<Rigidbody>().velocity = engine.GetComponent<Rigidbody>().velocity / v * engine_script.max_speed;
					}
				}
				if (Input.GetKey(KeyCode.S) && control_type==0 || s_down==1 && control_type==1)
				{
					engine.GetComponent<Rigidbody>().velocity -= engine_script.move_speed * engine.transform.right;
					float v = engine.GetComponent<Rigidbody>().velocity.magnitude;
					if( v > engine_script.max_speed)
					{
						engine.GetComponent<Rigidbody>().velocity = engine.GetComponent<Rigidbody>().velocity / v * engine_script.max_speed;
					}
				}
				if (Input.GetKey(KeyCode.D) && control_type==0 || d_down==1 && control_type==1)
				{
					Quaternion q = Quaternion.Euler(new Vector3(0,engine_script.rotational_speed * engine.GetComponent<Rigidbody>().velocity.magnitude/engine_script.max_speed,0));
					engine.GetComponent<Rigidbody>().MoveRotation(engine.transform.rotation * q);
					engine.GetComponent<Rigidbody>().velocity = q*engine.GetComponent<Rigidbody>().velocity;
				}
				if (Input.GetKey(KeyCode.A) && control_type==0 || a_down==1 && control_type==1)
				{
					Quaternion q = Quaternion.Euler(new Vector3(0,-engine_script.rotational_speed * engine.GetComponent<Rigidbody>().velocity.magnitude/engine_script.max_speed,0));
					engine.GetComponent<Rigidbody>().MoveRotation(engine.transform.rotation * q);
					engine.GetComponent<Rigidbody>().velocity = q*engine.GetComponent<Rigidbody>().velocity;
				}
				break;



			case 3:
			case 6:
				//ufo, x wing
				if (Input.GetKey(KeyCode.W) && control_type==0 || w_down==1 && control_type==1)
				{
					engine.GetComponent<Rigidbody>().velocity += engine_script.move_speed * engine.transform.right;
					float v = engine.GetComponent<Rigidbody>().velocity.magnitude;
					if( v > engine_script.max_speed)
					{
						engine.GetComponent<Rigidbody>().velocity = engine.GetComponent<Rigidbody>().velocity / v * engine_script.max_speed;
					}
				}
				if (Input.GetKey(KeyCode.S) && control_type==0 || s_down==1 && control_type==1)
				{
					engine.GetComponent<Rigidbody>().velocity -= engine_script.move_speed * engine.transform.right;
					float v = engine.GetComponent<Rigidbody>().velocity.magnitude;
					if( v > engine_script.max_speed)
					{
						engine.GetComponent<Rigidbody>().velocity = engine.GetComponent<Rigidbody>().velocity / v * engine_script.max_speed;
					}
				}
				if (Input.GetKey(KeyCode.D) && control_type==0 || d_down==1 && control_type==1)
				{
					Quaternion q = Quaternion.Euler(new Vector3(0,engine_script.rotational_speed,0));
					engine.GetComponent<Rigidbody>().MoveRotation(engine.transform.rotation * q);
					//engine.GetComponent<Rigidbody>().velocity = q*engine.GetComponent<Rigidbody>().velocity;
				}
				if (Input.GetKey(KeyCode.A) && control_type==0 || a_down==1 && control_type==1)
				{
					Quaternion q = Quaternion.Euler(new Vector3(0,-engine_script.rotational_speed,0));
					engine.GetComponent<Rigidbody>().MoveRotation(engine.transform.rotation * q);
					//engine.GetComponent<Rigidbody>().velocity = q*engine.GetComponent<Rigidbody>().velocity;
				}
				break;
			}
		}



	}


	public override void Damaged()
	{
		GameObject.Instantiate(Resources.Load("h2")
		                       ,transform.position
		                       ,Quaternion.identity);
		

		BCMessenger.Instance.RemoveListener(this.gameObject);

		if(game_manager!=null)
		{
			game_manager.RemoveRobot(this.gameObject.GetInstanceID(),playerID);
		}
		
		Destroy(weapon);
		Destroy (engine);
		Destroy (gameObject);
		
	}




	/*
	public override void Damaged()
	{
		GameObject.Instantiate(Resources.Load("h2")
		                       ,transform.position
		                       ,Quaternion.identity);
		
		//BCMessenger.Instance.
		
		if(game_manager!=null)
		{
			game_manager.RemoveRobot(this.gameObject.GetInstanceID(),playerID);
		}
		
		Destroy(weapon);
		Destroy (engine);
		Destroy (gameObject);
		
	}
	*/



}
