/************************************************************************************************
 * This script is attached to the infection
 * Its purpose is to hold the health of the infection and then remove it when there is 
 * no health remaining
 * 
 * Author: Bruce Gustin
 * Date Written: Feb 1, 2026
 * Version 2.0
 *************************************************************************************************/

using UnityEngine;

public class InfectionHealth : MonoBehaviour
{
    public float maxInfectionLoad = 250;                    //Maximum infection load
    public float startInfectionLoad = 75;                   //Infection load at start of game
    public float currentInfectionLoad;                      //Infection load at any given time 
    public float infectionLoadIncreasePerRepeat = .03f ;    //How much load increases 
    public float infectionLoadDecreasePerCollision = .01f ;  //How much load decreases
    public float infectionLoadRepeatRate = 0.25f;            //How often does the infection load increase

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentInfectionLoad = startInfectionLoad;
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
}