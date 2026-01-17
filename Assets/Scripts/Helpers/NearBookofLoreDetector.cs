/*****************************************************************************
 * This script is attached to the Book of Lore
 * It makes sure that the Player cannot turn pages if it is not near the book.
 * 
 * Author: Bruce Gustin
 * Date Written: August 23, 2025
 * Version 1.0
 *****************************************************************/

using UnityEngine;

public class NearBookofLoreDetector : MonoBehaviour
{ 
    //This changes the playerNearBook field to true so the player can change the page in the book
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.playerNearBook = true;
        }
    }

    //This changes the playerNearBook field to false so the player can't change the page in the book
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.playerNearBook = false;
        }
    }
}
