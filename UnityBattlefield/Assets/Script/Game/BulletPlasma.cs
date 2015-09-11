using UnityEngine;
using System.Collections;

public class BulletPlasma : Bullet {
	public int cur_bouncing_times = 0;
	public int max_bouncing_times;





	protected override void OnCollisionEnter (Collision col)
	{
		//Debug.Log (GetComponent<Rigidbody>().velocity);
		if(col.gameObject.tag == "Boundary")
		{
			cur_bouncing_times += 1;
			if(cur_bouncing_times > max_bouncing_times)
			{
				CreateHitAnimation();
				Destroy (transform.parent.gameObject);
			}
		}
		else if(col.gameObject.tag =="Mech" && col.gameObject.GetInstanceID() != launcher)
		{
			//TODO: identify enemy and friends

			col.gameObject.GetComponent<Engine>().ReceiveDamage(damage);
			CreateHitAnimation();
			Destroy (transform.parent.gameObject);
		}

	}
}
