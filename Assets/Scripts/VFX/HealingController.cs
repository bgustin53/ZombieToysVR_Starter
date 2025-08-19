using UnityEngine;

public class HealingController : MonoBehaviour
{
	[Header("VFX Specs")]
	public float Cooldown = 1f;                     //The cooldown of the attach

	[SerializeField] float range = 20.0f;           //How far the attack can shoot
	[SerializeField] LayerMask strikeableMask;      //Layermask that determines what the attack can hit

	[Header("VFX References")]
	[SerializeField] HealingBeam healingBeam;      //Reference to the lightningBolt script on the lightning bolt game object
	[SerializeField] AVPlayer healHit;             //Reference to the AVPlayer (Audio Visual Player) on the lightning hit game object

	//Called from PlayerOrbOptions script
	public void Fire()
	{
		//Create a ray from the current position and extending straight forward
		Ray ray = new Ray(transform.position, transform.forward);
		//Create a RaycastHit variable which will store information about the raycast
		RaycastHit hit;

		//If our raycast hits an object in the strikeable layer within range of the origin
		if (Physics.Raycast(ray, out hit, range, strikeableMask))
		{
			//...move the lightning hit game object to the point of the hit...
			healHit.transform.position = hit.point;
			//...and play the effect...
			healHit.Play();
			//...then set the end point of the lightning bolt..
			healingBeam.EndPoint = hit.point;
			//...then try to get a reference to an EnemyHealth script...
			ZombieHealth zombieHealth = hit.collider.GetComponent<ZombieHealth>();
			//...if the script exists...
			if (zombieHealth != null)
			{
				//...tell the enemy to take damage
				zombieHealth.ImproveHealth(this.name);
			}
		}
		//Otherwise, if our raycast doesn't hit anything...
		else
		{
			//...place the end of the beam at maximum range
			healingBeam.EndPoint = ray.GetPoint(range);
		}
		//Turn the healing beam game object on
		healingBeam.gameObject.SetActive(true);
	}
}
