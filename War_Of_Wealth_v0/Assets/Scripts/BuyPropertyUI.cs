using TMPro;
using UnityEngine;

public class BuyPropertyUI : MonoBehaviour
{
    public TextMeshProUGUI promptText;

    private BoardSpace currentSpace;

    public GameObject EndTurnUI;
    public void Show(BoardSpace space)
    {
        currentSpace = space;
        promptText.text = $"Buy {space.spaceName} for ${space.price}?";
    }

    public void OnBuy()
    {
        Debug.Log("Player bought " + currentSpace.spaceName);
        gameObject.SetActive(false);
        EndTurnUI.SetActive(true);
    }

    public void OnCancel()
    {
        Debug.Log("Player declined to buy.");
        gameObject.SetActive(false);
        EndTurnUI.SetActive(true);

    }
}