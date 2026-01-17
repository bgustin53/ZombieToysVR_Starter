//This script handles the stink attack. The stink attack is an area of effect (AoE) attack with a long cooldown. When fired, this attack
//launches a projectile that will explode and target all of the enemies in a radius, forcing them to run away. This attack only 
//fires when you release the trigger.

using UnityEngine;

public class StinkController : MonoBehaviour
{
	[Header("Weapon Specs")]
	public float Cooldown = 5f;								//Cooldown of the attack

	[SerializeField] float range = 5f;						//Range of the attack

	[Header("Weapon References")]
	[SerializeField] StinkProjectile stinkProjectile;		//Reference to the stink projectile
	[SerializeField] Renderer targetReticule;				//Reference to the targetting reticule

	[Header("Reticule Colors")]
	[SerializeField] Color invalidTargetTint = Color.red;   //Color of invalid targets           
	[SerializeField] Color notReadyTint = Color.yellow;     //Color when the attack isn't ready           
	[SerializeField] Color readyTint = Color.green;			//Color of a valid target

	float timeOfLastAttack = -10f;                          //The time when the attack was last made, initialized with a dummy value
	Transform target = null;                                //The target of the attack
	Vector3 targetPosition;									//The position of a target
	bool inRange = false;									//Is the target in range of the attack?

	//Called from PlayerAttack script
	public void Fire()
	{
		//If the target is in range, launch the projectile
		if(inRange && timeOfLastAttack + Cooldown <= Time.time)
			LaunchProjectile();
	}

	void Update()
	{
		// Use the center of the VR headset's view (gaze-based targeting)
		targetPosition = Camera.main.transform.position;

		//Create a RaycastHit variable
		RaycastHit hit;

		//Cast a ray from the camera in the direction the player is looking
		if (Physics.Raycast(targetPosition, Camera.main.transform.forward, out hit, 12.5f, LayerMask.GetMask("Floor")))
		{
			target = hit.transform;
			// Update targetPosition to where the ray hit (on the ground)
			targetPosition = hit.point;
			//Find the distance between the mouse and the player
			float distance = Vector3.Distance(targetPosition, transform.position);
			//If the distance is smaller than the range, then the attack is in range
			if (distance <= range)
				inRange = true;
		}
		else
		{
			target = null;
		}

		UpdateReticule();
	}

	//This method updates the position and color of the reticule
	void UpdateReticule()
	{
		//Place the reticule where the mouse is
		targetReticule.transform.position = targetPosition;

		//If the attack isn't in range, set invalid tint
		if (!inRange)
			targetReticule.material.SetColor("_TintColor", invalidTargetTint);
		//If attack is on cooldown, set not ready tint
		else if (timeOfLastAttack + Cooldown > Time.time)
			targetReticule.material.SetColor("_TintColor", notReadyTint);
		//Otherwise, set ready tint
		else
			targetReticule.material.SetColor("_TintColor", readyTint);
	}

	//This method launches the projectile
	void LaunchProjectile()
	{
		//record the time of the attack
		timeOfLastAttack = Time.time;
		//Turn the projectile on
		stinkProjectile.gameObject.SetActive(true);
		//Send the projectile on its way towards the target
		stinkProjectile.StartPath(transform.position, targetPosition);
	}
}
