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
