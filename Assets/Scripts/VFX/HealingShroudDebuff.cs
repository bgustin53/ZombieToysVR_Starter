//This script handles the slime debuff. This debuff attaches to an enemy and slowly damages them while
//preventing the zombie= from attacking

using UnityEngine;
using System.Collections;

public class HealingShroudDebuff : MonoBehaviour
{
	[HideInInspector] ZombieAttack targetAttack;	//Reference to the zombie's attack script
	[HideInInspector] ZombieHealth targetHealth;    //Reference to the zombie's health script

	[SerializeField] float effectDuration = 3f;		//How long the effect lasts
	[SerializeField] int healthPerSecond = 4;       //How many times the health improved per second

	WaitForSeconds healDelay;						//How long to wait in between healing

	void Awake()
	{
		//Calculate the delay between attacks by dividing 1 by the number of healing events in a second.
		//(example: 2 heals per second = 1f / 2 = .5f delay between attacks
		healDelay = new WaitForSeconds(1f / healthPerSecond);
	}

	//Called by SlimeProjectile script
	public void AttachToZombie(ZombieAttack zombie)
	{
		//Set the targetAttack variable to the provided enemy
		targetAttack = zombie;
		//Get a reference to the zombie's health
		targetHealth = targetAttack.GetComponent<ZombieHealth>();
		//Tell the zombie about this debuff
		targetAttack.healingShroudDebuff = this;
		//Nest this debuff to the zombie (so it follows the zombie around)
		transform.parent = targetAttack.transform;
		//Center this debuff on the zombie, except move it slightly up
		transform.localPosition = new Vector3(0f, 1f, 0f);
		//Start healing the zombie
		StartCoroutine("HealZombie");
	}

	//Releases the zombie after the appropriate amount of time
	public void ReleaseZombie()
	{
		//Forget the target of the debuff and un-parent it from the enemy
		targetAttack = null;
		targetHealth = null;
		transform.parent = null;
		//Turn the debuff off
		gameObject.SetActive(false);
	}

	//This coroutine heals the zombie over time
	IEnumerator HealZombie()
	{
		//Calculate how many healing events should occur
		int totalEvents = Mathf.RoundToInt(effectDuration * healthPerSecond);
		//Loop until enough healing events have been completed
		for (int i = 0; i < totalEvents; i++)
		{
			//Tell the zombie to eat their medicine
			targetHealth.ImproveHealth(this.name);
			//Wait until it is time to healing event again
			yield return healDelay;
		}

		//After the healing events have been complete, release the enemy
		ReleaseZombie();
	}
}
