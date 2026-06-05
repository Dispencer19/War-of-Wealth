using Unity.Multiplayer.PlayMode;
using Unity.VisualScripting;
//using UnityEditor.Build.Content;
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

    [Header("Battle")]
    [SerializeField] public FightTeleport fightTeleport;
    [SerializeField] public ChanceCard chanceCard;
    [Tooltip("Seconds the 'Battle Incoming' bubble shows before the fight auto-starts. Set to 0 to require a button that calls StartBattle().")]
    [SerializeField] public float battleIntroSeconds = 1.5f;

    private int pendingAttacker = 0;
    private int pendingDefender = 1;

    private void Awake()
    {
        if (fightTeleport == null) fightTeleport = FindFirstObjectByType<FightTeleport>();
        if (chanceCard == null) chanceCard = FindFirstObjectByType<ChanceCard>();
    }

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
        // Reset the card panel so the Draw button is shown and old text is cleared.
        if (chanceCard != null) chanceCard.ResetCardUI();
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

    // Wire this to a contended property's onLand event. The BoardSpace lets us
    // figure out the owner so the fight is lander vs. owner.
    public void Battle(BoardSpace space)
    {
        StartTurnUI.SetActive(false);
        BattleUI.SetActive(true);

        pendingAttacker = boardTurns.currPlayer;

        if (space != null && space.isOwned && space.ownerPlayerIndex >= 0 &&
            space.ownerPlayerIndex != pendingAttacker)
        {
            pendingDefender = space.ownerPlayerIndex;
        }
        else
        {
            // Fallback: fight the other player (works for a 2-player game).
            pendingDefender = pendingAttacker == 0 ? 1 : 0;
        }

        if (battleIntroSeconds > 0f)
        {
            CancelInvoke(nameof(StartBattle));
            Invoke(nameof(StartBattle), battleIntroSeconds);
        }
        // If battleIntroSeconds <= 0, hook a button on BattleUI to StartBattle().
    }

    // Parameterless version kept for backward compatibility / simple buttons.
    public void Battle()
    {
        Battle(null);
    }

    // Actually launch the split-screen fight. Safe to call from a UI button too.
    public void StartBattle()
    {
        if (BattleUI != null) BattleUI.SetActive(false);

        if (fightTeleport == null)
        {
            Debug.LogError("TileEffects: no FightTeleport assigned, cannot start battle.");
            return;
        }

        fightTeleport.SetFighters(pendingAttacker, pendingDefender);
        fightTeleport.StartFight();
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

    // Hide every tile popup. Called when a turn ends so nothing lingers.
    public void HideAllTileUIs()
    {
        if (BuyPropertyUI != null) BuyPropertyUI.SetActive(false);
        if (PayRentUI != null) PayRentUI.SetActive(false);
        if (ChanceCardUI != null) ChanceCardUI.SetActive(false);
        if (CommunityChestCardUI != null) CommunityChestCardUI.SetActive(false);
        if (PassGoUI != null) PassGoUI.SetActive(false);
        if (BattleUI != null) BattleUI.SetActive(false);
        if (InJailUI != null) InJailUI.SetActive(false);
        if (AimChallengeUI != null) AimChallengeUI.SetActive(false);
    }
}