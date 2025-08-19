/*****************************************************************
 * This script is attached to the zombie prefabs assigned in the 
 * serialized field.  It creates a collection of that zombie and 
 * sets them active on a listed spawn rate.  
 * 
 * Author: Bruce Gustin
 * Date Written: July 8, 2025
 * Version 1.1 - Fixed VR movement direction
 *****************************************************************/

using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
	[Header("Spawner Properties")]
	[SerializeField] GameObject zombiePrefab;       //The zombie prefab to spawn
	[SerializeField] float firstAppearance = 5f;    //First appearance, in seconds, of this zombie
	[SerializeField] float restDuration = 15f;	   	//Rate, in seconds, to spawn this zombie
	[SerializeField] int maxNumberOfClones = 10;	//Maximum number of zombies that this spawner can have at a time

	[Header("Debugging Properties")]
	[SerializeField] bool canNotSpawn = false;      //Can this spawner spawn zombies? This is useful for testing when you want to turn a spawner off
 
	private ZombieHealth[] zombies;                 //An array of the pooled zombies
	private WaitForSeconds restDelay;               //The delay between attempts to spawn an zombies
	public bool lastCured { get; private set; }     //This field says that there is only one zombie in scene not taked cured


	void Awake()
	{
		//Create an array to store the pool of enemies
		zombies = new ZombieHealth[maxNumberOfClones];
		//Loop through the array and...
		for (int i = 0; i < maxNumberOfClones; i++)
		{
			//...instantiate an zombie game object from a prefab...
			GameObject obj = (GameObject)Instantiate(zombiePrefab);
			//...get a reference to its ZombieHealth script...
			ZombieHealth zombie = obj.GetComponent<ZombieHealth>();

			if (zombie == null)
			{
				Debug.LogError("ZombieHealth component not found on zombie prefab!");
				continue;
			}

			//...SET THE SPAWNER REFERENCE...
			zombie.Spawner = this;
			//...parent it to this gamn object...
			obj.transform.parent = transform;
			//...disable it...
			obj.SetActive(false);
			//...finally, store the enemy's health script in the pool
			zombies[i] = zombie;
		}
		//Create the WaitForSeconds variable that will be used to delay spawning
		restDelay = new WaitForSeconds(firstAppearance);
	}

	/* This is an interesting use of Start() where the Start() method itself is
	 * used as a coroutine. You could have the Start() method run a different coroutine to
	 * achieve the same effect, but it's useful to know that using the Start() method like
	 * this is possible */
	IEnumerator Start()
	{
		bool first = true;
		//While the spawner can spawn...
		while (!canNotSpawn)
		{
			//...wait the specified delay...
			yield return restDelay;
			//...and then spawn an enemy before looping again
			if(GameManager.Instance.startSpawning)
				SpawnZombie();

			// you can set the first spawn differently then the rest.
			if(first) restDelay = new WaitForSeconds(restDuration);
		}
	}

	/* This method spawns an zombie into the scene by searching the pool for an available zombie
	 * and enabling it. It's worth nothing that it would be more efficient to create a system
	 * where we didn't have to search the pool for an available zombie and instead pulled any
	 * enemies that weren't available out of the pool. It is constructed this way, however, in 
	 * an attempt to keep the code as simple and clean as possible. Also, the size of the pools are
	 * very small, so the difference in efficiency between the two ways of doing this is negligable */
	void SpawnZombie()
	{
		RemainingUnCuredZombies();
		//Loop through the pool of zombies
		for (int i = 0; i < zombies.Length; i++)
		{
			//If the current enemy is available (not active)...
			if (!zombies[i].gameObject.activeSelf && !zombies[i].isCured)
			{
				//...orient it with the spawner...
				zombies[i].transform.position = transform.position;
				zombies[i].transform.rotation = transform.rotation;
				//...enable it...
				zombies[i].gameObject.SetActive(true);
				//...and leave this method so it doesn't accidently spawn more enemies
				return;
			}
		}
		
	}

	public void RemainingUnCuredZombies()
    {
		int notCured = 0;
		foreach(ZombieHealth zombie in zombies)
        {
			if (!zombie.isCured)
				notCured++;
        }
		if (notCured == 1)
		{
			lastCured = true;
		}
    }

	public void Relapse()
    {
		lastCured = false;
		foreach(ZombieHealth zombie in zombies)
        {
			zombie.isCured = false;
        }
    }
}
