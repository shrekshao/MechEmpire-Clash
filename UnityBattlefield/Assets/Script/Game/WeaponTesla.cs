using UnityEngine;
using System.Collections;

public class WeaponTesla : Weapon {
	public int damage;
	public override void Fire(int launcher)
	{
		if(cooling > 0)
		{
			return;
		}
		
		cooling = max_cooling;

		RaycastHit hit;
		// distanceToGround = 0;
		
		if (Physics.Raycast(transform.position, transform.right, out hit,2000f))
		{
			//Quaternion.Euler(transform.rotation.eulerAngles)* Vector3.right
			GameObject thunder = GameObject.Instantiate(Resources.Load("Thunder")) as GameObject;

			//TODO: figure it out

			thunder.GetComponent<ParticleSystem>().startSize = hit.distance;
			thunder.transform.position = fire_position.position;
			//thunder.transform.rotation = thunder.transform.rotation * Quaternion.Euler(Vector3.up*(rotation));
			//thunder.transform.rotation = Quaternion.Euler(0,transform.eulerAngles.y+180f,0);
			thunder.transform.rotation = Quaternion.Euler(Vector3.up*(transform.eulerAngles.y+90f));

			Engine e= hit.transform.gameObject.GetComponent<Engine>();
			if(e!=null)
			{
				e.ReceiveDamage(damage);
			}
		}
	}
}
