using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Battlefield : MonoBehaviour {
	public AudioClip[] audios;

	private const float BULLET_HEIGHT_OFFSET = 25;
	private const float HIT_HEIGHT = 30;

	private const float LASER_HEIGHT = 35;
	private const float LIGHTNING_HEIHGT = 35;

	private const float ARSENAL_HEIGHT = 45;
	private const float RESPAWNING_TIME = 1000;
	public string[] shortcuts;
	public string switch_camera_button;
	public Camera[] camera_list;
	private int current_camera_id;

	public RecordManager recordManager;		//referrence of record manager

	//Prefabs
	public GameObject ObstaclePrefab;
	//public GameObject ArsenalPrefab;
	public ParticleSystem ArsenalPrefab;

	//public LineRenderer LaserPrefab;




	//public GameObject GroundPrefab;
	//public GameObject ground;

	public GameObject main_camera;


	private int numObstacles;
	public GameObject []obstacles;

	private int numArsenals;
	//private GameObject []arsenals;
	private ParticleSystem []arsenals;

	private int numRobots;
	public RobotRecord []robots;

	//bullets 
	public Dictionary<int, GameObject> bullets;

	private int cf;		//current frame id


	private bool enable_update;




	// Use this for initialization
	void Start () {

		enable_update = false;
		GameObject.Find("Canvas").GetComponent<TextFileFinder>().RecordLoaded += OnRecordLoaded;
		//GameObject.Find("Main Camera").GetComponent<Transform>().position = new Vector3(0,100,0);



	}
	
	// Update is called once per frame
	void Update () 
	{
		if(camera_list.Length > 1)
		{
			for(int i=0; i<numRobots+1; i++)
			{  
				if (Input.GetKeyUp(shortcuts[i]))
				{
					SwitchCamera(i);
				}
			}
			
			if(Input.GetKeyUp(switch_camera_button))
			{
				SwitchCamera((current_camera_id+1)%(numRobots+1));
			}
		}

		if(enable_update)
		{
			foreach(RobotRecord r in robots)
			{
				r.Update();
			}

			//tmp camera test
			//GameObject.Find("Main Camera").transform.position = new Vector3(robots[1].position.x,20,robots[1].position.z) ;





		}

	}

	void FixedUpdate()
	{
		if(enable_update)
		{
			//Robot update
			for(int i = 0; i < numRobots; i++)
			{
				robots[i].TriggerActions(recordManager.frameData[cf].robotData[i].fire
				                    ,recordManager.frameData[cf].robotData[i].wturn
				                    ,recordManager.frameData[cf].robotData[i].run
				                    ,recordManager.frameData[cf].robotData[i].eturn);
				robots[i].SetNextFramePosition(recordManager.frameData[cf].robotData[i].x
				                       ,0
				                       ,recordManager.frameData[cf].robotData[i].y
		                               ,recordManager.frameData[cf].robotData[i].weaponRotation
		                               ,recordManager.frameData[cf].robotData[i].engineRotation);
			}


			//Bullet update
			Dictionary<int,bool> bullets_exist = new Dictionary<int,bool>();
			if(recordManager.frameData[cf].bulletData != null)
			{
				foreach(RecordManager.BulletData b in recordManager.frameData[cf].bulletData)
				{
					bullets_exist.Add(b.entityID,true);
					GameObject bullet_object;
					float h = RobotRecord.BULLET_HEIGHT[b.bulletType];
					if(bullets.TryGetValue(b.entityID, out bullet_object))
					{
						//this bullet already exists
						bullet_object.transform.position = new Vector3(b.x,h,b.y);
						bullet_object.transform.rotation = Quaternion.Euler(Vector3.up*b.rotation);
					}
					else
					{
						//create a new bullet
						string bullet_name = String.Format( "b{0}", b.bulletType);
						float bullet_height;
						if(b.launcherID < 0)
						{
							//old version record txt
							//no launcherID
							bullet_height = RobotRecord.BULLET_HEIGHT[b.bulletType] + BULLET_HEIGHT_OFFSET;
						}
						else
						{
							bullet_height = robots[b.launcherID].GetLaunchingHeight(b.bulletType);
						}
						bullets.Add(b.entityID,GameObject.Instantiate(Resources.Load(bullet_name)
						                                              ,new Vector3(b.x,bullet_height,b.y)
						                                              ,Quaternion.Euler(Vector3.up * b.rotation)) as GameObject);
					}
				}
			}
			ArrayList destroy_key_list = new ArrayList(3);
			foreach (KeyValuePair<int, GameObject> kvp in bullets)
			{
				if(!bullets_exist.ContainsKey(kvp.Key))
				{
					//this bullet no longer exists
					//kvp.Value.renderer.enabled = false;
					//kvp.Value.SetActive(false);
					destroy_key_list.Add(kvp.Key);
				}

			}

			//remove bullets
			foreach(int key in destroy_key_list)
			{
				GameObject.Destroy(bullets[key]);
				bullets.Remove(key);
			}




			//Hit update
			if(recordManager.frameData[cf].hitData != null)
			{
				foreach(RecordManager.HitData h in recordManager.frameData[cf].hitData)
				{
					if(h.hitType == 4)
					{
						//光棱
						GameObject laser = GameObject.Instantiate(Resources.Load("laser")) as GameObject;
						laser.GetComponent<LineRenderer>().SetPosition(0,new Vector3(h.x,LASER_HEIGHT,h.y));
						laser.GetComponent<LineRenderer>().SetPosition(1,new Vector3(h.ex,LASER_HEIGHT,h.ey));
					}
					else if(h.hitType == 5)
					{
						//TODO:磁暴
						//土渣了的办法

						GameObject thunder = GameObject.Instantiate(Resources.Load("thunder")) as GameObject;

						float dy = h.ey-h.y;
						float dx = h.ex-h.x;
						float dist = (float)Math.Sqrt( Math.Pow (dy,2) + Math.Pow (dx,2) );
						dist *= 1366f / 3900f;

						float rotation = (float)Math.Atan2(dy,dx)/(float)Math.PI*180f;

						thunder.GetComponent<ParticleSystem>().startSize = dist;
						thunder.transform.position = new Vector3(h.x,LIGHTNING_HEIHGT,h.y);
						//thunder.transform.rotation = thunder.transform.rotation * Quaternion.Euler(Vector3.up*(rotation));
						thunder.transform.rotation = Quaternion.Euler(Vector3.up*(-rotation+90f));
						//thunder.transform.rotation = thunder.transform.rotation * Quaternion.Euler(Vector3.up*45f);

						float a = 1;
						a +=1;
					}
					else
					{
						string hit_name ;
						int htype = 0;
						switch(h.hitType)
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
							htype = h.hitType;
							//htype = 0;
							break;
						}
						hit_name = String.Format( "h{0}", htype);

						GameObject.Instantiate(Resources.Load(hit_name),new Vector3(h.x,HIT_HEIGHT,h.y),Quaternion.identity);
					}

				}
			}



			//arsenals
			if(recordManager.frameData[cf].arsenalRespawnTime != null)
			{
				for(int i = 0; i < numArsenals ; i++)
				{
					//TODO: alpha of particle system
					//arsenals[i].GetComponentInChildren<Renderer>().material.color.a = 1 - 
					//	((float)recordManager.frameData[cf].arsenalRespawnTime[i] / RESPAWNING_TIME);
					//arsenals[i].particleSystem
					if (recordManager.frameData[cf].arsenalRespawnTime[i] == 0)
					{
						arsenals[i].Play();
					}
					else
					{
						arsenals[i].Stop();
					}
				}
			}




			
			cf++;
			if(cf >= recordManager.numFrames)
			{
				//TODO: send event, battle end
				enable_update = false;

				explodeLoser();
			}
		}
	}

	public void explodeLoser()
	{
		robots[1-recordManager.winnerID].explode();
	}


	void OnRecordLoaded(object sender, EventArgs e)
	{
		//test
		//Instantiate (obstacle,new Vector3(3,0,0),new Quaternion(-0.707f,0,0,0.707f));
		recordManager = GameObject.Find("Canvas").GetComponent<TextFileFinder>().recordManager;

		//TODO: test if cancel

		InitBattleMap();

		enable_update = true;
	}


	void InitBattleMap()
	{
		cf = 0;

		//ground = Instantiate (GroundPrefab,Vector3.zero,Quaternion.identity) as GameObject;
		//ground.transform.localScale = new Vector3(recordManager.mapData.width,1,recordManager.mapData.height);


		//Obstacles
		numObstacles = recordManager.mapData.obstacleData.Length;
		obstacles = new GameObject[numObstacles];
		Quaternion q;
		int i = 0;
		foreach(RecordManager.ObstacleData o in recordManager.mapData.obstacleData)
		{
			q = Quaternion.Euler(Vector3.up * 90 * i);
			obstacles[i] = Instantiate (ObstaclePrefab,new Vector3(o.x,0,o.y),q) as GameObject;
			obstacles[i].transform.localScale = new Vector3(1,1,1);
			float size_radium = o.r / ( obstacles[i].GetComponent<Renderer>().bounds.size.x / 2 );
			obstacles[i].transform.localScale = new Vector3(size_radium,size_radium,size_radium);
			//obstacles[i].transform.position = new Vector3(o.x,0,o.y);
			//obstacles[i].transform.localScale = new Vector3(o.r,o.r,100);
			i++;
		}

		//arsenals
		numArsenals = recordManager.mapData.arsenalData.Length;
		//arsenals = new GameObject[numArsenals];
		arsenals = new ParticleSystem[numArsenals];
		i = 0;
		q = Quaternion.identity;
		foreach(RecordManager.ArsenalData a in recordManager.mapData.arsenalData)
		{
			//arsenals[i] = Instantiate (ArsenalPrefab,new Vector3(a.x,ARSENAL_HEIGHT,a.y),q) as GameObject;
			arsenals[i] = Instantiate (ArsenalPrefab,new Vector3(a.x,ARSENAL_HEIGHT,a.y),q) as ParticleSystem;
			arsenals[i].transform.localScale = new Vector3(4,4,4);
			i++;
		}




		//robots
		numRobots = recordManager.frameData[0].robotData.Length;
		robots = new RobotRecord[numRobots];
		int j = 0;
		foreach(RecordManager.RobotData r in recordManager.frameData[cf].robotData)
		{
			robots[j] = new RobotRecord(j,r.weaponTypeID,r.engineTypeID
			                      ,recordManager.robotAIData[j].wred
			                      ,recordManager.robotAIData[j].wgreen
			                      ,recordManager.robotAIData[j].wblue
			                      ,recordManager.robotAIData[j].ered
			                      ,recordManager.robotAIData[j].egreen
			                      ,recordManager.robotAIData[j].eblue);	//?
			//robots[j] = gameObject.AddComponent<Robot>();
			robots[j].SetPosition(r.x
			                      ,0
			                      ,r.y
			                      ,r.weaponRotation
			                      ,r.engineRotation
			                      );
			j++;
		}

	
		
		//bullets
		bullets = new Dictionary<int, GameObject>();

		//hits



		//camera
		current_camera_id = 0;
		camera_list = new Camera[numRobots+1];
		Camera.GetAllCameras(camera_list);
		SwitchCamera(0);

		cf = 1;
	}


	public const bool  changeAudioListener = true;
	void  SwitchCamera ( int index  ){
		Camera c;
		for(int i=0; i<=numRobots; i++){  

			if(i != index){
				//c = robots[i].camera.transform.FindChild("Camera") as GameObject;
				c = camera_list[i];
				if(changeAudioListener){
					c.GetComponent<AudioListener>().enabled = false;
				}
				c.enabled = false;
			} else {
				//c = robots[i].camera.FindChild("Camera") as GameObject;
				c = camera_list[i];
				if(changeAudioListener){
					c.GetComponent<AudioListener>().enabled = true;
				}
				c.enabled = true;
				current_camera_id = i;
			}
		}


		
	}
}
