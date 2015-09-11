using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BladeCast;

public class GameManager : MonoBehaviour {

	public static Rect[] rect_ary = new Rect[]
		{
		new Rect(0,0,Screen.width/2,Screen.height/2)
		,new Rect(Screen.width/2,0,Screen.width/2,Screen.height/2)
		,new Rect(0,Screen.height/2,Screen.width/2,Screen.height/2)
		,new Rect(Screen.width/2,Screen.height/2,Screen.width/2,Screen.height/2)
	};

	//public int robot_dic.Count = 0;
	public int player_num_max = 4;
	public int player_id = 0;
	public bool[] player_rect = new bool[]{false,false,false,false};


	public Dictionary<int,GameObject> robot_dic;

	public Dictionary<int,GameObject> camera_dic;

	public string[] shortcuts;// = new string[]{"`","1","2","3","4"};
	//public KeyCode[] shortcuts = new KeyCode[]{KeyCode.BackQuote,KeyCode.Alpha1,KeyCode.Alpha2,KeyCode.Alpha3,KeyCode.Alpha4};

	private int cur_birth_idx = 0;
	public int num_birth_point;
	public Vector3[] birth_point = new Vector3[]{new Vector3(100,0,-100),new Vector3(1260,0,-100),
												new Vector3(1260,0,-580),new Vector3(100,0,-580)
	};

	public int num_weapon_available;
	public int num_engine_available;
	public int[] weapon_available;// = new int[]{0,1,2,3,6,11};
	public int[] engine_available;// = new int[]{0,2,3,4,5,6,7};

	// Use this for initialization
	void Start () {
		robot_dic = new Dictionary<int, GameObject>();
		camera_dic = new Dictionary<int, GameObject>();
		camera_dic.Add(Camera.main.GetInstanceID(),Camera.main.gameObject);
		num_birth_point = birth_point.Length;
		num_weapon_available = weapon_available.Length;
		num_engine_available = engine_available.Length;

		//camera_list = new Camera[1];
		//Camera.GetAllCameras(camera_list);
		BCMessenger.Instance.RegisterListener("connect", 0, this.gameObject, "HandleControllerRegister");
		//BCMessenger.
	}

	public void HandleControllerRegister(ControllerMessage msg)
	{
		Debug.Log ("New connected player "+msg.ControllerSource);

		//ADD player
		AddRobot("RobotPlayer",(int)Random.Range(0,num_weapon_available-0.01f)
		         ,(int)Random.Range(0,num_engine_available-0.01f),1,msg.ControllerSource);
	}

	// Update is called once per frame
	void Update () {
		//print ("num:"+robot_dic.Count);
		if(robot_dic.Count < player_num_max)
		{
			if(Input.GetKeyDown(KeyCode.Minus))
			{
				//add a ea control robot
				AddRobot("RobotPlayer",(int)Random.Range(0,num_weapon_available-0.01f),(int)Random.Range(0,num_engine_available-0.01f),1,robot_dic.Count%3);
			}

			if(Input.GetKeyDown(KeyCode.Equals))
			{
				//add a mouse keyboard control robot
				AddRobot("RobotPlayer",(int)Random.Range(0,num_weapon_available-0.01f),(int)Random.Range(0,num_engine_available-0.01f),0,-1);
			}
		}

		//print (Camera.allCamerasCount);
		for(int i=0; i<Camera.allCamerasCount; i++)
		{  
			if (Input.GetKeyUp(shortcuts[i]))
			{
				//Debug.Log ("press"+i);
				//print("switch press");
				SwitchCamera(i);
			}
		}


		rect_ary = new Rect[]
		{
			new Rect(0,0,Screen.width/2,Screen.height/2)
			,new Rect(Screen.width/2,0,Screen.width/2,Screen.height/2)
			,new Rect(0,Screen.height/2,Screen.width/2,Screen.height/2)
			,new Rect(Screen.width/2,Screen.height/2,Screen.width/2,Screen.height/2)
		};
	}

	//public int current_camera_id=0;
	Camera[] camera_list;
	//GameObject[] camera_list;
	public const bool  changeAudioListener = true;
	void  SwitchCamera ( int index  ){
		Camera c;

		//Debug.Log(camera_list);
		//Camera.current.enabled = false;
		//Camera.current.GetComponent<AudioListener>().enabled = false;
		//camera_list[index].enabled = true;
		//camera_list[index].GetComponent<AudioListener>().enabled = true;

		/*
		int i = 0;
		foreach(KeyValuePair<int,GameObject> pair in camera_dic)
		{
			if(i != index)
			{
				pair.Value.GetComponent<Camera>().enabled = false;
				pair.Value.GetComponent<AudioListener>().enabled = false;
			}
			else
			{
				pair.Value.GetComponent<Camera>().enabled = true;
				pair.Value.GetComponent<AudioListener>().enabled = true;
			}
			i++;
		}
		*/


		for(int i=0; i<camera_list.Length; i++){  
			
			if(i != index){
				//c = robots[i].camera.transform.FindChild("Camera") as GameObject;
				c = camera_list[i];
				c.GetComponent<AudioListener>().enabled = false;
				//c.enabled = false;
				//c.GetComponent<Camera>
			} else {
				//c = robots[i].camera.FindChild("Camera") as GameObject;
				c = camera_list[i];
				c.GetComponent<AudioListener>().enabled = true;
				c.enabled = true;
				//current_camera_id = i;
			}
		}



	}

	public void AddRobot(string prefab_name, int wtn, int etn,int control_type,int remote_id)
	{
		//wtn = 6;
		if(robot_dic.Count >= player_num_max)
		{
			print ("reached max players");
			return;
		}

		//robot_dic.Count ++;
		//Debug.LogError(wtn);
		//Debug.LogError(etn);
		GameObject robot = GameObject.Instantiate(Resources.Load (prefab_name),birth_point[cur_birth_idx],Quaternion.Euler(0,90f*cur_birth_idx,0)) as GameObject;
		cur_birth_idx = (cur_birth_idx+1)%num_birth_point;
		robot_dic.Add(robot.GetInstanceID(),robot);
		robot.GetComponent<Robot>().game_manager = this;

		RobotPlayer rbp = robot.GetComponent<RobotPlayer>();
		//rbp.control_type = control_type;
		rbp.InitControl(control_type,remote_id);

		for(int i = 0; i< player_num_max; i++)
		{
			if(player_rect[i] == false)
			{
				//print(i);
				rbp.playerID = i;
				player_rect[i] = true;
				break;
			}
		}

		//weapon,engine
		rbp.SetEngine(engine_available[etn]);
		rbp.SetWeapon(weapon_available[wtn]);


		//rbp.playerID = player_id;
		//player_id = (player_id + 1)%player_num_max;

		camera_dic.Add (robot.GetInstanceID(),robot.GetComponent<Robot>().fps_camera.transform.FindChild("Camera").gameObject);
		//TODO: camera list update
		//camera_list = new Camera[robot_dic.Count+1];
		//Camera.GetAllCameras(camera_list);

		camera_list = new Camera[robot_dic.Count + 1];
		Camera.GetAllCameras(camera_list);
		//camera_list = GameObject.FindGameObjectsWithTag("Camera");

		print ("add,num:"+robot_dic.Count);
	}

	public void RemoveRobot(int id,int playerID)
	{
		//robot_dic.Count --;
		print ("remove");

		player_rect[playerID] = false;

		robot_dic.Remove(id);
		camera_dic.Remove(id);
		//TODO: camera list update
		//camera_list = new Camera[robot_dic.Count+1];
		//Camera.GetAllCameras(camera_list);


		//temp.... buggy code..
		camera_list = new Camera[robot_dic.Count + 2];
		Camera.GetAllCameras(camera_list);

		//camera_list = GameObject.FindGameObjectsWithTag("Camera");
		print ("sub,num:"+robot_dic.Count);
	}
}
