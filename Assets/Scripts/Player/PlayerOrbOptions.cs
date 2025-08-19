//This script handles the player's ability to attack. The biggest responcibility of this script is to maintain the timing of attack cooldowns
//so that the player cannot attack too fast. Mostly, this is a "pass through" or "bridge" script, which means that it receives input from
//the PlayerInput scripts and then passes the input along to the appropriate attack. Very little attack logic (apart from timing) exists in this 
//script. 

using UnityEngine;
using System.Collections.Generic;

public class PlayerOrbOptions : MonoBehaviour
{
	[Header("ORB Options")]
	[SerializeField] private HealingController healingController;				//Reference to a lightning attack script
	[SerializeField] private FrostController frostController;					//Reference to a frost attack script
	[SerializeField] private StinkController stinkController;					//Reference to a stink attack script
	[SerializeField] private HealingShroudController healingShroudController;   //Reference to a slime attack script
	[SerializeField] private PlushizeController plushizeController;				//Reference to a lightning attack script

	[Header("UI")]
	[SerializeField] Countdown countDown;                     //A reference to the countdown slider

	private List<MonoBehaviour> controllers;
	private int numberOfOrbOptions;
	private int controllerIndex = 0;                                  //The index of the attack the player is currently using
	private float attackCooldown = 0f;                                //How long the player must wait before attacking again
	private float timeOfLastAttack = 0f;                              //The time when the player last attacked
	private bool canLaunch = true;                                    //Whether or not the player can attack

    private void Awake()
    {
		controllers = new List<MonoBehaviour>();

		if (healingController != null) controllers.Add(healingController);
		if (frostController != null) controllers.Add(frostController);
		if (stinkController != null) controllers.Add(stinkController);
		if (healingShroudController != null) controllers.Add(healingShroudController);
		if (plushizeController != null) controllers.Add(plushizeController);

		numberOfOrbOptions = controllers.Count;
	}


    //This method switches the active attack on the player
    public void SwitchAttack()
	{
		//If the player can't attack, leave
		if (!canLaunch || numberOfOrbOptions == 0)
			return;
		//Increase the attack index, then if the index is too high, set it back to 0
		controllerIndex++;
		if (controllerIndex >= numberOfOrbOptions)
			controllerIndex = 0;

		//Turn off all enabled attacks
		DisableAttacks();
		controllers[controllerIndex].gameObject.SetActive(true);

	}

	//This method is called whenever the player presses the attack input
	public void Fire()
	{
		//If the attack isn't ready, or the player cannot attack, leave
		if (!ReadyToAttack() || !canLaunch)
			return;

		//Examing the value of beam index. Note that we are only handling healing, plushize,
		//and frost here. This is because the stink and slime attacks only fire
		//when we release the trigger. Therefore, they are fired in the 
		//StopFiring() method
		switch (controllers[controllerIndex].GetType().Name)
		{
			case "HealingController":
				ShootHealing();
				break;
			case "FrostController":
				ShootFrost();
				break;
			case "PlushizeController":
				ShootPlushize(); 
				break;
		}
	}

	//This method is called whenever the player releases the beam input
	public void StopFiring()
	{
		//If the attack isn't ready, or the player cannot attack, leave
		if (!ReadyToAttack() || !canLaunch)
			return;

		//Examing the value of attackIndex. 
		switch (controllers[controllerIndex].GetType().Name)
		{
			case "StinkController":
				ShootStink();
				break;
			case "HealingShroudController":
				ShootShroud();
				break;
		}
	}

	//Handles telling the lightning attack to fire
	void ShootHealing()
	{
		//If there is no lightning attack, leave
		if (healingController == null)
			return;

		//Fire lightning
		healingController.Fire();
		//Record the cooldown of the lightning attack
		attackCooldown = healingController.Cooldown;
		//record how long to wait before we can attack again
		BeginCountdown();
	}

	//Handles telling the lightning attack to fire
	void ShootPlushize()
	{
		//If there is no lightning attack, leave
		if (plushizeController == null)
			return;

		//Fire lightning
		plushizeController.Fire();
		//Record the cooldown of the lightning attack
		attackCooldown = plushizeController.Cooldown;
		//record how long to wait before we can attack again
		BeginCountdown();
	}

	//Handles toggling frost on and off. Note that the frost attack has no cooldown
	void ShootFrost()
	{
		//If there is no frost attack, leave
		if (frostController == null)
			return;

		frostController.Fire();
		Invoke("StopFrost", 3.25f);
	}

	void StopFrost()
    {
		frostController.StopFiring();
    }

	//Handles telling the stink attack to fire
	void ShootStink()
	{
		//If there is no stink attack, leave
		if (stinkController == null)
			return;
		//Shoot a stink projectile
		stinkController.Fire();
		//record the cooldown of the stink attack
		attackCooldown = stinkController.Cooldown;
		//record how long to wait before we can attack again
		BeginCountdown();
	}

	//Handles telling the shroud attack to fire
	void ShootShroud()
	{
		//If there is no shroud attack, leave
		if (healingShroudController == null)
			return;
		//Attempt to fire shroud. If it was successful...
		if (healingShroudController.Fire())
		{
			//...record the cooldown of the shroud attack...
			attackCooldown = healingShroudController.Cooldown;
			//...and record how long to wait before we can attack again
			BeginCountdown();
		}
	}

	bool ReadyToAttack()
	{
		//If enough time has passed return true (the player can attack) otherwise return false
		return Time.time >= timeOfLastAttack + attackCooldown;
	}

	//Called from PlayerHealth script
	public void Defeated()
	{
		//Player cannot attack
		canLaunch = false;
		//Turn off all attacks
		DisableAttacks();
	}

	//This method sets the countdown until the player can attack again
	void BeginCountdown()
	{
		//Record the current time
		timeOfLastAttack = Time.time;
		//If there is a countdown slider, tell it to begin counting down
		if (countDown != null)
			countDown.BeginCountdown(attackCooldown);
	}

	//This method turns off all attacks
	void DisableAttacks()
	{
		//Go through each attack and if a game object for it exists, turn it off
		foreach(MonoBehaviour controller in controllers)
        {
			controller.gameObject.SetActive(false);
        }

	}
}
