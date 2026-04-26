using Unity.Multiplayer.PlayMode;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;

public class TileEffects : MonoBehaviour
{
    
    //List of events that can be assigned to each tile. These will be called when a player lands on the tile.
    [SerializeField] public GameObject StartTurnUI;
    
    [SerializeField] public GameObject BuyPropertyUI;

    [SerializeField] public GameObject PayRentUI;

    [SerializeField] public GameObject ChanceCardUI;

    [SerializeField] public GameObject CommunityChestCardUI;

    [SerializeField] public GameObject PassGoUI;

    [SerializeField] public GameObject EndTurnUI;

    [SerializeField] public GameObject BattleUI;

    [SerializeField] public GameObject InJailUI;

    [SerializeField] public BoardTurns boardTurns;

    [SerializeField] public GameObject AimChallengeUI;
    
    public void BuyProperty(BoardSpace space)
    {
        StartTurnUI.SetActive(false);
        if (space.isOwned)
        {
            Debug.Log("This property is already owned by Player " + space.ownerPlayerIndex);
            return;
        }
        BuyPropertyUI.SetActive(true);

        var ui = BuyPropertyUI.GetComponent<BuyPropertyUI>();

        ui.Show(space, boardTurns.currPlayer);
    }

    public void PayRent(BoardSpace space)
    {
        StartTurnUI.SetActive(false);
        if (!space.isOwned)
        {
            Debug.Log("This property is not owned. No rent to pay.");
            return;
        }
        else
        {
            Debug.Log("This property is owned by Player " + space.ownerPlayerIndex + ". Player must pay rent of " + space.rent);
        }
        
        var ui = PayRentUI.GetComponent<PayRentUI>();
        ui.Show(space, boardTurns.currPlayer);
    }

    public void PassGo()
    {
        StartTurnUI.SetActive(false);
        PassGoUI.SetActive(true);
    }

    public void ChanceCard()
    {
        StartTurnUI.SetActive(false);   
        ChanceCardUI.SetActive(true);
    }
    
    public void CommunityChestCard()
    {
        StartTurnUI.SetActive(false);
        CommunityChestCardUI.SetActive(true);
    }

    public void GoToJail()
    {
        StartTurnUI.SetActive(false);
        EndTurnUI.SetActive(true);
    }   

    public void FreeParking()
    {
        StartTurnUI.SetActive(false);
        EndTurnUI.SetActive(true);
    }

    public void Battle()
    {
        StartTurnUI.SetActive(false);
        BattleUI.SetActive(true);
    }

    public void InJail()
    {
        StartTurnUI.SetActive(false);
        InJailUI.SetActive(true);
    }

    public void JustVisiting()
    {
        StartTurnUI.SetActive(false);
        EndTurnUI.SetActive(true);
    }

    public void AimChallenge()
    {
        StartTurnUI.SetActive(false);
        AimChallengeUI.SetActive(true);
    }
}
