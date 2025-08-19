/*****************************************************************************
 * This script is attached to the Game Manager.
 * This script handles the game management. Game managers are often completely different and generally provide whatever
 * specific and varied services an individual game may need. In this project, in an effort to make the code simple to understand
 * and modular, the game manager is tied into several core functions of the player, enemies, and allies. Namely, this manager
 * keeps track of the player and the players state, handles all scoring, interfaces with the UI, summons the allies, and 
 * reloads the scene when the player is defeated
 * 
 * Author: Unity
 * Date Written: July 8, 2025
 * Version 1.1 - Fixed VR movement direction
 *****************************************************************/



using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;                   //This script, like MouseLocation, has a public static reference to itself to that other scripts
                                                          //can access it from anywhere without needing to find a reference to it

    [Header("Player and Enemy Properties")]
    public PlayerHealth Player;						      //A reference to the player's health script which will be considered "the player"
    public Transform ZombieTarget;                        //The object that zombies are chasing. This needs to be separate from the player because the game manager
                                                          //can make zombies chase something that isn't the player (as is the case with allies)

    [Header("Healing Beam Properties")]
    [Range(10, 30)]
    [Tooltip("Heal cycle should be 4 to 12 hits")]
    public int healingFromHealingBeam;
    [Range(1, 3)]
    [Tooltip("This should be from 1/10 to 1/4 of healing beam.  It will repeat 12 times per hit")]
    public int healingFromHealingShroud;

    [Space(12)]
    [SerializeField] float delayOnPlayerDeath = 3f;       //How long to wait once the player has been defeated

    [Header("UI Properties")]
    [SerializeField] GameObject startCanvas;              //Canvas containing instructions

    [Header("Book of Lore Property")]
    [SerializeField] GameObject[] bookOfLorePages;        //Pages in Book of Lore
    [SerializeField] TextMeshProUGUI tooSoonMessage;      //Can turn page yet message
    [SerializeField] int timeBetweenPageTurns;	 	      //Required time to pass to get to next page

    [Header("Starts or Pauses Game")]
    public bool startSpawning;                            //The set true when player presses the B button;

    private int currentPage;                              //Which page is open in book, title page is page zero.
    public bool pageCanTurn = true;                       //Toggled based on time between page turns
    public bool playerNearBook { private get; set; }      //Is set from XR Rig Movement script when Rig is in the Book of Lore trigger

    void Awake()
    {
        //This is a common approach to handling a class with a reference to itself.
        //If instance variable doesn't exist, assign this object to it
        if (Instance == null)
            Instance = this;
        //Otherwise, if the instance variable does exist, but it isn't this object, destroy this object.
        //This is useful so that we cannot have more than one GameManager object in a scene at a time.
        else if (Instance != this)
            Destroy(this);
    }

    //Called from the PlayerHealth script when the player has been defeated
    public void PlayerDied()
    {
        //The enemies no longer have a target
        ZombieTarget = null;

        //If the game over text UI element exists, turn it on
        PlayerDeathComplete();
    }

    //Called from the PlayerHealth script when the player is done playing their death animation
    public void PlayerDeathComplete()
    {
        //Call the ReloadScene() method after the set delay
        Invoke("ReloadScene", delayOnPlayerDeath);
    }


    //This method reloads the scene after the player has been defeated
    void ReloadScene()
    {
        //Get a reference to the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        
        //Tell the SceneManager to load the current scene (which reloads it)
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    //Referenced by the Start button on the title screen
    public void StartGame()
    {
        startCanvas.SetActive(false);
        startSpawning = true;

    }

    //Called from PlayerInputController when VR Grip is pressed
    public void TurnPage()
    {
        //Checks to see if turning is allowed
        if (playerNearBook)
        {
            if (pageCanTurn)
            {
                //Set up for next page turns
                pageCanTurn = false;

                //Changes Page
                bookOfLorePages[currentPage].SetActive(false);
                currentPage++;
                if (currentPage >= bookOfLorePages.Length)
                    currentPage = 0;
                bookOfLorePages[currentPage].SetActive(true);

                //Routine will toogle page can turn back to true
                StartCoroutine(TurnPageTimer());
            }
            else
            {
                Debug.Log("too soon");
                tooSoonMessage.gameObject.SetActive(true);
                Invoke("DisableText", 4.5f); // Disable after 4.5 seconds
            }
        }
    }

    //Routine will toogle page can turn back to true
    IEnumerator TurnPageTimer()
    {
        yield return new WaitForSeconds(timeBetweenPageTurns);
        pageCanTurn = true;
    }

    void DisableText()
    {
        tooSoonMessage.gameObject.SetActive(false);
    }
}
