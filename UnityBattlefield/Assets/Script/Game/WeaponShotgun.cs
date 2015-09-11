using UnityEngine;
using System.Collections;
using System;

public class WeaponShotgun : Weapon {
	
	public float gap_rotation = 5f;
	public override void Fire(int launcher)
	{
		if(cooling > 0)
		{
			return;
		}
		
		cooling = max_cooling;
		
		string bullet_name = String.Format( "b{0}", weapon_type);


		Quaternion main_q = Quaternion.Euler(Vector3.up * transform.eulerAngles.y);
		for(int i=0; i < 5; i++ )
		{
			GameObject b = GameObject.Instantiate(
				Resources.Load(bullet_name)
				,fire_position.position
				,main_q * Quaternion.Euler(Vector3.up* ((float)i - 2.5f)*gap_rotation ) ) as GameObject;
			b.GetComponentInChildren<Bullet>().launcher = launcher;
		}

		

		
		GetComponent<AudioSource>().Play();
	}
}
