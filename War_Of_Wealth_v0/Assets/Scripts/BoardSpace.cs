
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class BoardSpace
{
    //this is the script that will hold the info for each space. Its values will be accessed when
    //a player lands on the space. For example, if a player lands on a property space, 
    // the BuyProperty() function will access the price and name of 
    // the property from this script to display in the UI.
    
    public int spaceIndex;
    public string spaceName;
    public int price;
    public int rent;
    public Sprite cardimage;
    public int color;
    public bool isOwned = false;
    public int ownerPlayerIndex = -1; // -1 means no owner, otherwise 0 to 3 for player index

    public UnityEvent<BoardSpace> onLand; // Event to trigger when a player lands on this space

    public void Land(int currPlayer)
    {
        Debug.Log("Player " + currPlayer + " landed on " + spaceName);
        onLand?.Invoke(this); // Trigger the event when a player lands on this space
    }

}
