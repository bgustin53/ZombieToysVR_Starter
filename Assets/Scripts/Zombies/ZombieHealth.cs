/************************************************************************************************
 * This script is attached to the Zombies
 * This script controls the health functions of the zombies. It is also responsible for turning
 * the zombie movement and healing off in the event of the zombie being cured. Since the zombies 
 * aren't destroyed after being cured (they are just disabled since the game maintains 'pools' 
 * or collections of zombies) there is code in place to reset the values of the zombies when 
 * they respawn
 * 
 * Author: Bruce Gustin
 * Date Written: July 8, 2025
 * Version 1.1 - Fixed VR movement direction
 *************************************************************************************************/



using UnityEngine;
using System.Collections;

public class ZombieHealth : MonoBehaviour
{
	[HideInInspector] public ZombieSpawner Spawner;					//A Reference to the spawner that created this zombie

	[Header("Health Properties")]
	[SerializeField] int healthToRest = 100;						//How much health this zombie needs to be healed
	[SerializeField] int cyclesToCure = 6;							//How much health this zombie needs to be healed
	[SerializeField] float timeUntilRelapse = 30;                   //If zombie will relapse, in how long in seconds

	[Header("Rest Effects")]
	[SerializeField] float sinkSpeed = 2.5f;						//How fast the enemy sinks into the ground		
	[SerializeField] float fallEffectTime = 2f;					    //How long it takes the enemy to play its full death sequence before being deactivated
	[SerializeField] AudioClip fallAsleepClip = null;				//Audio clip of the death sound of the enemy
	[SerializeField] AudioClip addHealthClip = null;				//Audio clip of the hurt sound of the enemy

	[Header("Script References")]
	[SerializeField] ZombieAttack zombieAttack;						//Reference to the zombie's attack script
	[SerializeField] ZombieMovement zombieMovement;					//Reference to the zombie's movement script

	[Header("Components")]
	[SerializeField] Animator animator;								//Reference to the animator component
	[SerializeField] AudioSource audioSource;						//Reference to the audio source component
	[SerializeField] CapsuleCollider capsuleCollider;				//Reference to the capsule collider component
	[SerializeField] ParticleSystem hitParticles;                   //Reference to the particle system on the hit particles game object
	[SerializeField] ParticleSystem lastZombieIndicator;            //Reference to the particle system on the Zombie

	[Header("Debugging Properties")]
	[SerializeField] bool isInvulnerable;                           //Is the enemy immune to all damage?

	//[SerializeField] TextMeshPro debugMessage;

	private int currentHealth;										//Current health amount of zombie
	private int currentCycle;										//Current health amount of zombie
	private bool isSinking;											//Is the zombie currently sinking?
	public bool headHome { get; private set; }					    //Heads home
	public bool isCured;						 				    //Mark this clone cured
	private bool willRelapse = true;                                //This occurs when the last shot at the zombie is not with the Plushizer
	private bool lastCycleOfLastZombieCured;					    //Mark last zombie cured

	//Reset() defines the default values for properties in the inspector
	void Reset ()
	{
		//Grab references to all of the needed enemy components
		zombieAttack = GetComponent<ZombieAttack>();
		zombieMovement = GetComponent<ZombieMovement>();

		animator = GetComponent <Animator> ();
		audioSource = GetComponent <AudioSource> ();
		capsuleCollider = GetComponent <CapsuleCollider> ();


		//Get the particle indicator component
		lastZombieIndicator = GetComponent<ParticleSystem>();
	}

	//When this game object is enabled...
	void OnEnable ()
	{
		//...reset the health, isSinking, and make the capsule collider solid again (it is 
		//turned into a trigger so the enemy can sink through the ground)
		isSinking = false;
		capsuleCollider.isTrigger = false;
		lastCycleOfLastZombieCured = false;

		//If there is an audio source, set the clip to the hurt sound
		if (audioSource != null)
			audioSource.clip = addHealthClip;

		
	}

	void Update()
	{
		//If the enemy isn't currently sinking, return
		if(!isSinking)
			return;
		//If the enemy is sinking, move downward along the -Y axis
		transform.Translate(-Vector3.up * sinkSpeed * Time.deltaTime);
	}

	//This method is called whenever the zombie is hit with a healing or plushizing beam
	public void ImproveHealth(string typeOfBeam)
    {
		switch (typeOfBeam)
        {
			case "Healing_Indicator":
				ImproveHealth(GameManager.Instance.healingFromHealingBeam);
				break;
			case "Plushize_Indicator":
				if (lastCycleOfLastZombieCured)
				{
					willRelapse = false;
					ImproveHealth(999);  //Value irrevelent, this will plushize this zombie
				}
				break;
			case "HealingShroudDebuff":
				ImproveHealth(GameManager.Instance.healingFromHealingShroud);
				break;
		}

	}

	//This method is called from its overload above
	public void ImproveHealth(int healthImprovedPerHit) 
	{
		//If the zombie is already healed or is invulnerable, leave
		if (isInvulnerable)
			return;

		//Increase the current health by the amount of health per hit
		currentHealth += healthImprovedPerHit;
		//Debug.Log($"currentHealth {currentHealth}");

		// Check if entity can respond (must meet health requirement)
		if (currentHealth >= healthToRest)
		{
			currentHealth = 0;
			currentCycle++;

			// Check if this zombie has reached cure threshold
			if (currentCycle >= cyclesToCure)
			{
				isCured = true;

				// Only plushize if all zombies are cured
				if (Spawner.lastCured)
				{
					if (!lastCycleOfLastZombieCured)
					{
						lastZombieIndicator.Play();
						lastCycleOfLastZombieCured = true;
					}
					else 
					{
						//Set new NavMesh Agent destination
						headHome = true;

						//Stop playing particle effect
						lastZombieIndicator.Stop();

						if (willRelapse)
							StartCoroutine(RelapseZombies());
                    }
				}
				else
				{
					RespawnNextCycle(); // Will be final cycle since we're at cure threshold
				}
			}
			else
			{
				// Regular respawn cycle
				RespawnNextCycle();
			}
		}


		//If there is an audio source, play it
		if (audioSource != null)
			audioSource.Play();
		//Play the hit particle effect
		hitParticles.Play();
	}

	//Called when the enemy health is reduce to 0 or lower
	void RespawnNextCycle()
	{
		//Capsule collider becomes a trigger to that the enemy can sink into the ground and so that
		//this collider won't interfere with player attacks
		capsuleCollider.isTrigger = true;

		//Enabled the animator (in case it was disabled by a frost debuff)
		animator.enabled = true;
		//Trigger the "Dead" parameter of the animator
		animator.SetTrigger("Dead");

		//If there is an audio source, set its clip to the death sound
		if(audioSource != null)
			audioSource.clip = fallAsleepClip;

		//Tell the attack and movement script that the zombie is resting
		zombieAttack.Resting();
		zombieMovement.Resting();

		//Call the TurnOff() method after a period of time
		Invoke("TurnOff", fallEffectTime);
	}

	//Called once the enemy's "defeated" effects have finished playing
	void TurnOff()
	{
		// Add a particle effect to note that it is on the last cycle

		//Disable the game object
		gameObject.SetActive(false);
	}

	// Accessed from an event in the enemy's Death animation
	public void StartSinking()
	{
		//The enemy is now sinking
		isSinking = true;
	}

	IEnumerator RelapseZombies()
    {
		//Wait then head back to target
		yield return new WaitForSeconds(timeUntilRelapse);
		headHome = false;

		//Reset all the health fields to start values
		currentHealth = 0;
		currentCycle = 0;
		isCured = false;
		lastCycleOfLastZombieCured = false;

		//Reset all this type zombie to not cured
		Spawner.Relapse();
    }
}

