//This script handles the stink hit "splash" attack (splash attack is a term meaning the attack hits all enemies in a radius, or "splashes" all over an area). The splash hit
//will play a graphical effect and cause enemies in the area to go running from the player.

using UnityEngine;

public class StinkHit : MonoBehaviour
{
	[SerializeField] float explosionRadius = 3f;	//Radius of the explosion
	[SerializeField] float explosionDuration = 4f;	//How long the explosion effect plays

	Collider[] zombiesHit;							//An array holding a collection of enemy colliders

	//When this game object is enabled, it immediately explodes
	void OnEnable()
	{
		//Create a Physics.OverlapSphere which tests the volume of a sphere for any colliders. Like a raycast, this
		//can be set to only find colliders on certain layers. The colliders hit are stored in our enemiesHit array
		zombiesHit = Physics.OverlapSphere(transform.position, explosionRadius, LayerMask.GetMask("Shootable"));
		//Loop through the array of enemy colliders
		for (int i = 0; i < zombiesHit.Length; i++)
		{
			//try to get a reference to an EnemyMovement script off of the colliders
			ZombieMovement zombieMovement = zombiesHit[i].GetComponent<ZombieMovement>();

			//If the ZombieMovement script exists, tell the enemy to run away
			if (zombieMovement != null)
				zombieMovement.Runaway();
		}

		//Call the StopExploding() method after a set period of time
		Invoke("StopExploding", explosionDuration);
	}

	//This method tells zombie that were hit to stop running away
	void StopExploding()
	{
		//Loop through the array of enemy colliders
		for (int i = 0; i < zombiesHit.Length; i++)
		{
			//try to get a reference to an ZombieMovement script off of the colliders
			ZombieMovement zombieMovement = zombiesHit[i].GetComponent<ZombieMovement>();

			//If the ZomnieMovement script exists, tell the enemy to come back
			if (zombieMovement != null)
				zombieMovement.ComeBack();
		}
		//Turn this game object off
		gameObject.SetActive(false);
	}
}
