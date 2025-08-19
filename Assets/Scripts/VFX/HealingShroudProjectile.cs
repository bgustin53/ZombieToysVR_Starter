//This script handles the shroud projectile. The shroud projectile is a seeking attack, meaning it can't miss.
//Once the proctile reaches the target zombie, it attaches a shroud debuff to the zombie

using UnityEngine;

public class HealingShroudProjectile : MonoBehaviour
{
	[Header("Projectile Properties")]
	[SerializeField] float speed = 20f;							//How fast the projectile moves
	[SerializeField] float projectileRadius = 1f;				//How wide the projectile is

	[Header("Projectile References")]
	[SerializeField] HealingShroudDebuff healingShroudDebuff;   //Reference to the HealingShroudDebuff debuff 
	[SerializeField] AVPlayer shroudHit;						//Reference to the AVPlayer (Audio Visual Player) on the slime hit game object

	Transform attackTarget;										//The target of this projectile
	bool isFlying;                                              //Is the proctile currently traveling?

	//When this game object in enabled, reset isFlying to false
	void OnEnable()
	{
		isFlying = false;
	}

	//Called from SlimeAttack script
	public void StartPath(Transform target)
	{
		//Set the target of this attack and start flying
		attackTarget = target;
		isFlying = true;
	}

	void Update()
	{
		//If projectile isn't flying, leave
		if (!isFlying)
			return;

		//If projectile has no target (maybe the enemy died while it was traveling) disable this projectile
		if(attackTarget == null)
			gameObject.SetActive(false);
		//Turn the projectile to face the target
		transform.LookAt(attackTarget);
		//Move towards the target
		transform.Translate(0f, 0f, speed * Time.deltaTime);
		//If the projectile is in range of the target, explode!
		if (Vector3.Distance(transform.position, attackTarget.position) <= projectileRadius)
		{
			Explode();
		}
	}

	//This method handles disabling the projectile and applying a debuff to the target zombie
	void Explode()
	{
		//Projectile is no longer flying
		isFlying = false;
		//Move the shroud hit to the target position
		shroudHit.transform.position = attackTarget.position;
		//Play the shroud hit effect
		shroudHit.Play();
		//Get a reference to the zombie attack script
		ZombieAttack zombie = attackTarget.GetComponent<ZombieAttack>();
		//If an EnemyAttack script exists on the target...
		if (zombie != null)
		{
			//...activate the shroud debuff and attach it to the zombie
			healingShroudDebuff.gameObject.SetActive(true);
			healingShroudDebuff.AttachToZombie(zombie);
		}
		//Turn this projectile off
		gameObject.SetActive(false);
	}
}
