//This script handles the slime attack. The slime attack is a targetted attack with a long cooldown. One enemy will be targetted
//by the attack and will get "slimed". The slime prevents them from attacking and causes them to take damage over time. This attack only 
//fires when you release the trigger.

using UnityEngine;

public class HealingShroudController : MonoBehaviour
{
	[Header("Beam Specs")]
	public float Cooldown = 3.5f;											//Cooldown of the attack

	[Header("Beam References")]
	[SerializeField] HealingShroudProjectile healingShroudProjectile;		//Reference to the slime projectile
	[SerializeField] Renderer targetReticule;								//Reference to the targetting reticule

	[Header("Reticule Colors")]
	[SerializeField] Color invalidTargetTint = Color.red;				    //Color of invalid targets
	[SerializeField] Color notReadyTint = Color.yellow;						//Color when the attack isn't ready
	[SerializeField] Color readyTint = Color.green;							//Color of a valid target

	float timeOfLastAttack = -10f;											//The time when the attack was last made, initialized with a dummy value
	Transform target = null;												//The target of the attack
	Vector3 targetPosition;													//The position of a target
	Collider[] testColliders;												//An array for storing a collection of colliders
					
	//Called from PlayerAttack script. This attack is not void like other
	//attacks and will instead let the PlayerAttack script know if it was successful
	public bool Fire()
	{
		//If the attack has a target...
		if (target != null)
		{
			//...Launch the slime projectile and return true
			LaunchProjectile();
			return true;
		}
		//If attack wasn't successful, return false
		return false;
	}

	void Update()
	{
		// Use the center of the VR headset's view (gaze-based targeting)
		targetPosition = Camera.main.transform.position;

		//Create a RaycastHit variable
		RaycastHit hit;

		//Cast a ray from the camera in the direction the player is looking
		if (Physics.Raycast( targetPosition, Camera.main.transform.forward, out hit, 6.5f, LayerMask.GetMask("Shootable")))
		{
			target = hit.transform;
			// Update targetPosition to where the ray hit (on the ground)
			targetPosition = hit.point;
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
		//If there is a target move the reticule to its location
		if (target != null)
			targetReticule.transform.position = target.position;
		//Otherwise, place the reticule where the mouse is
		else
			targetReticule.transform.position = targetPosition;

		//If there is no target, set the invalid tint
		if (target == null)
			targetReticule.material.SetColor("_TintColor", invalidTargetTint);
		//If there is a target but the attack in on cooldown, set the not ready tint
		else if (timeOfLastAttack + Cooldown > Time.time)
			targetReticule.material.SetColor("_TintColor", notReadyTint);
		//Otherwise, the reticule should be set to the ready tint
		else
			targetReticule.material.SetColor("_TintColor", readyTint);
	}

	//This method launches a "seeking" projectile at the targetted enemy
	void LaunchProjectile()
	{
		//Record the current time
		timeOfLastAttack = Time.time;
		//Move the slime position to the attack's position
		healingShroudProjectile.transform.position = transform.position;
		//Turn the projectile on
		healingShroudProjectile.gameObject.SetActive(true);
		//Start the projectile along its path to the target
		healingShroudProjectile.StartPath(target);

		//Forget the current target
		target = null;
	}
}
