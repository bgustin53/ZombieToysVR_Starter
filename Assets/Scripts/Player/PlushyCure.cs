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

public class PlushyCure : MonoBehaviour
{
 
    void OnParticleCollision(GameObject other)
        {
            if (other.CompareTag("Infection")) // && GameManager.Instance.plushiesCured > 0)
            {
                InfectionHealth infectionHealth = other.GetComponent<InfectionHealth>();
                infectionHealth.currentInfectionLoad -= infectionHealth.infectionLoadDecreasePerCollision;
                Debug.Log($"Infection remaining: {infectionHealth.currentInfectionLoad}");

                if (infectionHealth.currentInfectionLoad <= 0)
                {
                    Debug.Log("Infection Cleared!");
                    other.SetActive(false);
                }
            }
        }
}
