/************************************************************************************************
 * This script is attached to the infection
 * Its purpose is to hold the health of the infection and then remove it when there is 
 * no health remaining
 * 
 * Author: Bruce Gustin
 * Date Written: Feb 1, 2026
 * Version 1.0
 *************************************************************************************************/
using Unity.VisualScripting;
using UnityEngine;

public class InfectionHealth : MonoBehaviour
{
    [SerializeField] private float maxInfectionLoad = 250;                    //Maximum infection load
    [SerializeField] private float startInfectionLoad = 100;                  //Infection load at start of game
    [SerializeField] private float currentInfectionLoad = 75;                 //Infection load at any given time 
    [SerializeField] private float infectionLoadIncreasePerRepeat = .03f ;    //How much load increases 
    [SerializeField] private float infectionLoadDecreasePerCollision = .01f ;  //How much load decreases
    [SerializeField] private float infectionLoadRepeatRate = 2.0f;            //How often does the infection load increase

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("IncreaseInfectionLoad", 0, infectionLoadRepeatRate);
    }

    // Infection load grows until max over time
    void IncreaseInfectionLoad()
    {
        if(currentInfectionLoad < maxInfectionLoad)
        {
            currentInfectionLoad += infectionLoadIncreasePerRepeat;
        }
    }

    // 
    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("PlushizerParticle") && GameManager.Instance.plushiesCured > 0)
            currentInfectionLoad -= infectionLoadDecreasePerCollision;

        if (currentInfectionLoad <= 0)
        {
            Debug.Log("Infection Cleared!");
            CancelInvoke(nameof(IncreaseInfectionLoad));
            Destroy(gameObject);
        }
    }
}
