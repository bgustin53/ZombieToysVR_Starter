/*****************************************************************************
 * This script is attached to a Trigger empty next to the food.
 * It creates a list of all the food tagged object in th scene and then remove
 * one of those items from the list when the Player triggers it.
 * 
 * Author: Unity
 * Date Written: July 21, 2025
 * Version 1.0
 ****************************************************************************/

using UnityEngine;
using System.Collections.Generic;

public class FoodCollection : MonoBehaviour
{
    private List<GameObject> foodList = new List<GameObject>();

    private void Start()
    {
        GameObject[] foodObjects = GameObject.FindGameObjectsWithTag("Food");
        foreach(GameObject food in foodObjects)
        {
            foodList.Add(food);
        }
    }

    //Destroys a food object and removes it from the list.  Returns true if there are food items to destroy.
    private bool EatFoodFromList()
    {
        if (foodList.Count > 0 && foodList[0] != null)
        {
            Destroy(foodList[0]);
            foodList.RemoveAt(0);
            return true;
        }
        else return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && EatFoodFromList())
        {
            other.gameObject.GetComponentInChildren<PlayerHealth>().AddHealth();
        }
    }
}
