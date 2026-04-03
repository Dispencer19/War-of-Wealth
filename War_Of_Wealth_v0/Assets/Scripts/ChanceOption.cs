using UnityEngine;

[System.Serializable]
public class ChanceOption
{
    public string cardName;        // Title of the card
    public string description;     // What the card says
    public int moveTo = -1;        // Optional: move player to a tile (-1 = no move)
    public int moneyChange = 0;    // Optional: give/take money
}