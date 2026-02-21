/************************************************************************************************
 * This script is attached to the infection
 * Its purpose is to hold the health of the infection and then remove it when there is 
 * no health remaining
 * 
 * Author: Bruce Gustin
 * Date Written: Feb 1, 2026
 * Version 1.0
 *************************************************************************************************/

using UnityEngine;

public class InfectionHealth : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float maxInfectionLoad = 1000;                   //Maximum infection load
    [SerializeField] private float startInfectionLoad = 300;                  //Infection load at start of game
    [SerializeField] private float infectionLoadIncreasePerRepeat = .12f ;    //How much load increases 
    [SerializeField] private float infectionLoadDecreasePerCollision = .2f ;  //How much load decreases
    private float currentInfectionLoad;                                       //Infection load at any given time 
    private float infectionLoadRepeatRate = 0.25f;                            //How often does the infection load increase
    private ParticleSystem infectionParticles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentInfectionLoad = startInfectionLoad;                            //Initializes the current load
        infectionParticles = GetComponent<ParticleSystem>();                  //Gets the particlw system component 
        var main = infectionParticles.main;                                   //Gets the main module
        var startLifetime = infectionParticles.main.startLifetime.constant;   //Gets the start lifetime value
        main.maxParticles = (int) (maxInfectionLoad * startLifetime);         //Prevents gaps in flow from too many particles in scene
        var emission = infectionParticles.emission;                           //Gets the emission module
        emission.rateOverTime = currentInfectionLoad;                         //Sets rate over time to current load for feedback to player
        InvokeRepeating("IncreaseInfectionLoad", 0, infectionLoadRepeatRate); //Starts the slow increase in viral load
    }
    // Keep the infection above the player
    void Update()
    {
        transform.position = player.position + Vector3.up * 6;
    }

    // Infection load grows until max over time
    void IncreaseInfectionLoad()
    {
        if(currentInfectionLoad < maxInfectionLoad)
        {
            currentInfectionLoad += infectionLoadIncreasePerRepeat;
            ParticleEmissionRate();
        }
    }

    // This is called from the PlushyCure class on the Plushizer Beam
    public void DecreaseInfectionLoad()
    {
            currentInfectionLoad -= infectionLoadDecreasePerCollision;
            Debug.Log($"Infection remaining: {currentInfectionLoad}");
            ParticleEmissionRate();

            if (currentInfectionLoad <= 0)
            {
                Debug.Log("Infection Cleared!");
                gameObject.SetActive(false);
            }
    }

    // This is to give the player a visual indication of the current infection load
    private void ParticleEmissionRate()
    {
        var emission = infectionParticles.emission;
        emission.rateOverTime = currentInfectionLoad;
    }
}