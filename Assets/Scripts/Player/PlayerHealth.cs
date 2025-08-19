//This script keeps track of the player's health. It is also used to communicate with the GameManager

using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
	[Header("Health Properties")]
	[SerializeField] int maxHealth = 100;               //Player's maximum health
	[Tooltip("Percentage health remaining before warning")]
	[Range(5, 25)]
	[SerializeField] int healthNotification = 10;       //Player's maximum health
	[SerializeField] AudioClip deathClip = null;		//Sound clip for the player's death

	[Header("Script References")]
	[SerializeField] XRRigMovement playerMovement;		//Reference to the player's movement script
	[SerializeField] PlayerOrbOptions playerOrbOptions;	//Reference to the player's attack script

	[Header("Components")]
	[SerializeField] Animator animator;					//Reference to the animator component
	[SerializeField] AudioSource audioSource;           //Reference to the audio source component
	[SerializeField] Renderer meshRenderer;             //Reference to the renderer component

	[Header("UI")]
	[SerializeField] GameObject criticalHealth;         //Critical health message
	[SerializeField] FlashFade damageImage;				//Reference to the FlashFade script on the DamageImage UI element
	[SerializeField] Slider healthSlider;               //The slider that will represent the player's health
	
		
	[Header("Debugging Properties")]				
	[SerializeField] bool isInvulnerable = false;		//Is the player invulnerable? Useful for debugging so the player won't take damage

	private int currentHealth;                          //The current health of the player

	// These are all fields of the health indicator
	private float startIndicatorRatio = 1.0f;           //This is for the orb health indicator
	private float endIndicatorRatio = 1.0f;             //This is for the orb health indicator
	private float rateOfIndicatorRatio = 0.001f;        //The speed at which the indicator morphs

	//Reset() defines the default values for properties in the inspector
	void Reset ()
	{
		//Grab a reference to the needed components
		animator = GetComponent <Animator> ();
		audioSource = GetComponent <AudioSource> ();
		playerMovement = GetComponentInParent<XRRigMovement>();
		playerOrbOptions = GetComponent <PlayerOrbOptions> ();
		meshRenderer = GetComponent<Renderer>();
	}

	void Awake()
	{
		//Set the player's health
		currentHealth = maxHealth;
	}

	//This method allows the zombies to assign damage to the player
	public void TakeDamage (int amount)
	{
		//If the player isn't alive, leave
		if (!IsAlive())
			return;

		//If the player is not invulnerable, reduce the current health
		if (!isInvulnerable)
		{
			startIndicatorRatio = (currentHealth * 1.0f) / maxHealth;
			currentHealth -= amount;
			endIndicatorRatio = (currentHealth * 1.0f) / maxHealth;
		}

			//If there is a damage image, tell it to flash
			if (damageImage != null)
			damageImage.Flash();

		//If there is a health slider, update its value
		if (healthSlider != null)
			healthSlider.value = currentHealth / (float)maxHealth;
		else
			DisplayHealthIndicator();


		//If the player has been defeated by this attack...
		if (!IsAlive())
		{
			//...if there is a player movement script, tell it to be defeated
			if (playerMovement != null)
				playerMovement.Defeated();
			//...if there is a player attack script, tell it to be defeated
			if (playerOrbOptions != null)
				playerOrbOptions.Defeated();

			//...set the Die parameter in the animator
			animator.SetTrigger ("Die");
			//...if there is an audio source, assign the deathclip to it
			if(audioSource != null)
				audioSource.clip = deathClip;
			//...finally, tell the GameManager that the player has been defeated
			GameManager.Instance.PlayerDied();
		}
		//If there is an audio source, tell it to play
		if(audioSource != null)
			audioSource.Play();
	}

	public void AddHealth()
    {
		//Player adds half of max health to current health
		currentHealth += maxHealth / 2;

		//Prevents health from exceeding maximum health
		currentHealth = Mathf.Min(currentHealth, maxHealth);

		DisplayHealthIndicator();
	}

	private void DisplayHealthIndicator()
	{
		/*
		if (startIndicatorRatio < healthNotification / 100)
        {
			criticalHealth.SetActive(true);
		}
        else
        {
			criticalHealth.SetActive(false);

		}
			*/

		// Morph Health Indicator slowly from start indicator to end indicator
		InvokeRepeating("MorphDisplayHealthIndicator", 0, rateOfIndicatorRatio);
	}


	public bool IsAlive()
	{
		//If the currentHealth is above 0 return true (the player is alive), otherwise return false
		return currentHealth > 0;
	}

	//This method is called by an event in the Death animation on the player
	void DeathComplete ()
	{
		//If this player is the registered player of the GameManager, tell the GameManager that this player
		//has finished its death animation
		if(GameManager.Instance.Player == this)
			GameManager.Instance.PlayerDeathComplete();
	}

	private void MorphDisplayHealthIndicator()
	{
		// Normalize health to 0-1 range
		float healthLossRatio = startIndicatorRatio;

		// Calculate red and green components with power curves for better visual perception
		// Use power curve to make red appear earlier and more prominently
		float red = Mathf.Pow(1f - healthLossRatio, 0.5f);   // More red when health is low (squared for emphasis)
														   // Use power curve to make green fade faster as health decreases
		float green = Mathf.Pow(healthLossRatio, 2f);    // More green when health is high (square root for gradual fade)

		// Create base color
		Color baseColor = new Color(red, green, 0f, 1f);

		// Apply emission intensity multiplier (typical range: 1-5)
		float emissionIntensity = 3.5f - 3.5f * healthLossRatio;  // brighter when health loss is high
		Color emissionColor = baseColor * emissionIntensity;

		// Apply the emission color to the material
		Material material = meshRenderer.materials[0];
		material.SetColor("_EmissionColor", emissionColor);

		// Make sure emission is enabled
		material.EnableKeyword("_EMISSION");

		// Start morphing
		startIndicatorRatio += (endIndicatorRatio - startIndicatorRatio) * rateOfIndicatorRatio;
		if (startIndicatorRatio <= endIndicatorRatio)
			CancelInvoke();
	}

}
