using Unity.Multiplayer.PlayMode;
using Unity.VisualScripting;
//using UnityEditor.Build.Content;
using UnityEngine;
using Photon.Pun;

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

    [SerializeField] public NetworkUIManager networkUI;

    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (networkUI == null)
            networkUI = FindAnyObjectByType<NetworkUIManager>();
    }
    
    public void BuyProperty(BoardSpace space)
    {
        // Hide start turn UI for all
        if (photonView.IsMine)
            networkUI.HideUISynced(StartTurnUI.name);

        if (space.isOwned)
        {
            Debug.Log("This property is already owned by Player " + space.ownerPlayerIndex);
            return;
        }

        // Show buy property UI for all
        if (photonView.IsMine)
            networkUI.ShowUISynced(BuyPropertyUI.name);

        var ui = BuyPropertyUI.GetComponent<BuyPropertyUI>();

        ui.Show(space, boardTurns.currPlayer);
    }

    public void PayRent(BoardSpace space)
    {
        // Hide start turn UI for all
        if (photonView.IsMine)
            networkUI.HideUISynced(StartTurnUI.name);

        if (!space.isOwned)
        {
            Debug.Log("This property is not owned. No rent to pay.");
            return;
        }
        else
        {
            Debug.Log("This property is owned by Player " + space.ownerPlayerIndex + ". Player must pay rent of " + space.rent);
        }

        // Show pay rent UI for all
        if (photonView.IsMine)
            networkUI.ShowUISynced(PayRentUI.name);
        
        var ui = PayRentUI.GetComponent<PayRentUI>();
        ui.Show(space, boardTurns.currPlayer);
    }

    public void PassGo()
    {
        // Hide start turn UI for all
        if (photonView.IsMine)
            networkUI.HideUISynced(StartTurnUI.name);

        // Show pass go UI for all
        if (photonView.IsMine)
            networkUI.ShowUISynced(PassGoUI.name);
    }

    public void ChanceCard()
    {
        // Hide start turn UI for all
        if (photonView.IsMine)
            networkUI.HideUISynced(StartTurnUI.name);

        // Show chance card UI for all
        if (photonView.IsMine)
            networkUI.ShowUISynced(ChanceCardUI.name);
    }
    
    public void CommunityChestCard()
    {
        // Hide start turn UI for all
        if (photonView.IsMine)
            networkUI.HideUISynced(StartTurnUI.name);

        // Show community chest UI for all
        if (photonView.IsMine)
            networkUI.ShowUISynced(CommunityChestCardUI.name);
    }

    public void GoToJail()
    {
        // Hide start turn UI for all
        if (photonView.IsMine)
            networkUI.HideUISynced(StartTurnUI.name);

        // Show end turn UI for all
        if (photonView.IsMine)
            networkUI.ShowUISynced(EndTurnUI.name);
    }   

    public void FreeParking()
    {
        // Hide start turn UI for all
        if (photonView.IsMine)
            networkUI.HideUISynced(StartTurnUI.name);

        // Show end turn UI for all
        if (photonView.IsMine)
            networkUI.ShowUISynced(EndTurnUI.name);
    }

    public void Battle()
    {
        // Hide start turn UI for all
        if (photonView.IsMine)
            networkUI.HideUISynced(StartTurnUI.name);

        // Show battle UI for all
        if (photonView.IsMine)
            networkUI.ShowUISynced(BattleUI.name);
    }

    public void InJail()
    {
        // Hide start turn UI for all
        if (photonView.IsMine)
            networkUI.HideUISynced(StartTurnUI.name);

        // Show in jail UI for all
        if (photonView.IsMine)
            networkUI.ShowUISynced(InJailUI.name);
    }

    public void JustVisiting()
    {
        // Hide start turn UI for all
        if (photonView.IsMine)
            networkUI.HideUISynced(StartTurnUI.name);

        // Show end turn UI for all
        if (photonView.IsMine)
            networkUI.ShowUISynced(EndTurnUI.name);
    }

    public void AimChallenge()
    {
        // Hide start turn UI for all
        if (photonView.IsMine)
            networkUI.HideUISynced(StartTurnUI.name);

        // Show aim challenge UI for all
        if (photonView.IsMine)
            networkUI.ShowUISynced(AimChallengeUI.name);
    }
}
       