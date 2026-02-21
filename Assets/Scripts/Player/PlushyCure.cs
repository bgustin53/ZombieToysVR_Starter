/************************************************************************
 * This script is attached to the plushizer beam
 * Its purpose is to decrease the health of the infection when the 
 * particle hits the infection 
 * Author: Bruce Gustin
 * Date Written: Feb 1, 2026
 * Version 1.1
 ************************************************************************/

using UnityEngine;

public class PlushyCure : MonoBehaviour
{
 
    // Since this object can only collide with the Infection Layer, all collisions are with the infection
    void OnParticleCollision(GameObject other)
    {
        InfectionHealth infectionHealth = other.GetComponent<InfectionHealth>();
        Debug.Log($"Hits: {other}");
        
        if(infectionHealth != null)
        {
            infectionHealth.DecreaseInfectionLoad();
        }
    }
}
