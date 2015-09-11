using UnityEngine;
using System;

public class RecordManager {

	


	//
	public struct RobotData
	{
		public int entityID;
		public int id;
		public float x;
		public float y;
		public float weaponRotation;
		public float engineRotation;
		public int hp;
		public int weaponTypeID;
		public int engineTypeID;
		public int fire;
		public int wturn;
		public int run;
		public int eturn;
		public int remainingAmmo;

		public RobotData(string text)
		{
			string[] data = text.Split(',');
			entityID = Convert.ToInt32(data[0]);
			id = Convert.ToInt32(data[1]);
			x = Convert.ToSingle(data[2]);
			y = -Convert.ToSingle(data[3]);
			weaponRotation = Convert.ToSingle(data[4]);
			engineRotation = Convert.ToSingle(data[5]);
			hp = Convert.ToInt32(data[6]);
			weaponTypeID = Convert.ToInt32(data[7]);
			engineTypeID = Convert.ToInt32(data[8]);
			fire = Convert.ToInt32(data[9]);
			wturn = Convert.ToInt32(data[10]);
			run = Convert.ToInt32(data[11]);
			eturn = Convert.ToInt32(data[12]);
			remainingAmmo = Convert.ToInt32(data[13]);
		}
	}

	public struct BulletData
	{
		//public int indexBullet;
		public int entityID;
		public int bulletType;
		public float x;
		public float y;
		public float rotation;

		public int launcherID;	//only exists in new version record

		public BulletData(string text)
		{
			launcherID = -1;

			string[] data = text.Split(',');
			entityID = Convert.ToInt32(data[0]);
			bulletType = Convert.ToInt32(data[1]);

			x = Convert.ToSingle(data[2]);
			y = -Convert.ToSingle(data[3]);
			rotation = Convert.ToSingle(data[4]);

			if(data.Length >= 6)
			{
				launcherID = Convert.ToInt32(data[5]);
			}
		}
	}

	public struct HitData
	{
		public int hitType;
		public float x;
		public float y;

		public float ex;
		public float ey;

		public HitData(string text)
		{
			string[] data = text.Split(',');

			hitType = Convert.ToInt32(data[0]);
			
			x = Convert.ToSingle(data[1]);
			y = -Convert.ToSingle(data[2]);

			if(data.Length > 3)
			{
				ex = Convert.ToSingle(data[3]);
				ey = -Convert.ToSingle(data[4]);
			}
			else
			{
				ex = 0;
				ey = 0;
			}
		}
	}

	public struct FrameData
	{
		public RobotData[] robotData;
		public BulletData[] bulletData;
		public HitData[] hitData;
		public int[] arsenalRespawnTime;

		public FrameData(string text)
		{
			string[] data = text.Split('|');

			string[] robotText = data[0].Split('*');
			string[] bulletText = data[1].Split ('*');
			string[] hitText = data[2].Split('*');
			string[] arsenalText = data[3].Split('*');

			robotData = new RobotData[robotText.Length];
			for(int i = 0; i<robotText.Length; i++)
			{
				robotData[i] = new RobotData(robotText[i]);
			}

			if(bulletText[0] != "")
			{
				bulletData = new BulletData[bulletText.Length];
				for(int i = 0; i<bulletText.Length; i++)
				{
					bulletData[i] = new BulletData(bulletText[i]);
				}
			}
			else
			{
				bulletData = null;
			}

			if(hitText[0] != "")
			{
				hitData = new HitData[hitText.Length];
				for(int i = 0; i<hitText.Length; i++)
				{
					hitData[i] = new HitData(hitText[i]);
				}
			}
			else
			{
				hitData = null;
			}

			if(arsenalText[0] != "")
			{
				arsenalRespawnTime = new int[arsenalText.Length];
				for(int i = 0; i<arsenalText.Length; i++)
				{
					arsenalRespawnTime[i] = Convert.ToInt32(arsenalText[i]);
				}
			}
			else
			{
				arsenalRespawnTime = null;
			}
		}
	}


	//info at the beginning of txt
	public struct ObstacleData
	{
		public float x;
		public float y;
		public float r;

		public ObstacleData(string text)
		{
			string[] data = text.Split(',');
			

			x = Convert.ToSingle(data[0]);
			y = -Convert.ToSingle(data[1]);
			r = Convert.ToSingle(data[2]);
		}
	}

	public struct ArsenalData
	{
		public float x;
		public float y;
		public float r;

		public ArsenalData(string text)
		{
			string[] data = text.Split(',');
			
			x = Convert.ToSingle(data[0]);
			y = -Convert.ToSingle(data[1]);
			r = Convert.ToSingle(data[2]);
		}
	}

	public struct AchievementData
	{
		public int fire,hit,damageReceive,damageOutput;


		public AchievementData(string text)
		{
			string[] data = text.Split(',');
			fire = Convert.ToInt32(data[0]);
			hit = Convert.ToInt32(data[1]);
			damageReceive = Convert.ToInt32(data[2]);
			damageOutput = Convert.ToInt32(data[3]);


		}
	}


	public struct RobotAIData
	{
		public string robotName;
		public string authorName;
		public int wred,wgreen,wblue;
		public int ered,egreen,eblue;

		public int maxHp,maxAmmo;

		public RobotAIData(string text)
		{
			string[] data = text.Split(',');
			robotName = data[0];
			authorName = data[1];
			wred = Convert.ToInt32(data[2]);
			wgreen = Convert.ToInt32(data[3]);
			wblue = Convert.ToInt32(data[4]);
			ered = Convert.ToInt32(data[5]);
			egreen = Convert.ToInt32(data[6]);
			eblue = Convert.ToInt32(data[7]);

			maxHp = Convert.ToInt32(data[8]);
			maxAmmo = Convert.ToInt32(data[9]);
		}
	}

	public struct MapData
	{
		public int width,height;

		public ObstacleData[] obstacleData;
		public ArsenalData[] arsenalData;

		public MapData(string text_boundary
		               , string text_obstacles
		               , string text_arsenals
		               )
		{
			string[] data_boundary = text_boundary.Split(',');
			width = Convert.ToInt32(data_boundary[0]);
			height = Convert.ToInt32(data_boundary[1]);

			string[] obstacleText = text_obstacles.Split('*');
			if(obstacleText[0] != "")
			{
				obstacleData = new ObstacleData[obstacleText.Length];
				for(int i = 0; i<obstacleText.Length; i++)
				{
					obstacleData[i] = new ObstacleData(obstacleText[i]);
				}
			}
			else
			{
				obstacleData = null;
			}

			string[] arsenalText = text_arsenals.Split('*');
			if(arsenalText[0] != "")
			{
				arsenalData = new ArsenalData[arsenalText.Length];
				for(int i = 0; i<arsenalText.Length; i++)
				{
					arsenalData[i] = new ArsenalData(arsenalText[i]);
				}
			}
			else
			{
				arsenalData = null;
			}
		}
	}


	public int numFrames;
	public FrameData[] frameData;
	public RobotAIData[] robotAIData;
	public MapData mapData;
	public AchievementData[] achievementData;
	public int winnerID;

	public RecordManager(string text)
	{
		string[] frameText = text.Split('\n');
		numFrames = frameText.Length - 1;

		//handle the first row
		string[] first_row_data = frameText[0].Split('|');
		string[] robotAIText = first_row_data[0].Split('*');
		robotAIData = new RobotAIData[robotAIText.Length];
		for(int i = 0; i<robotAIText.Length; i++)
		{
			robotAIData[i] = new RobotAIData(robotAIText[i]);
		}
		mapData = new MapData(first_row_data[1],first_row_data[2],first_row_data[3]);

		string[] achievementText = first_row_data[4].Split('*');
		achievementData = new AchievementData[achievementText.Length];
		for(int i = 0; i<achievementText.Length; i++)
		{
			achievementData[i] = new AchievementData(achievementText[i]);
		}

		winnerID = Convert.ToInt32(first_row_data[5]);

		//handle the following frame rows
		frameData = new FrameData[numFrames];

		for(int i = 1; i < frameText.Length; i++)
		{
			frameData[i-1] = new FrameData(frameText[i]);
		}
		
	}


}
