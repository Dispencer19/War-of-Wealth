using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyPropertyUI : MonoBehaviour
{
    public TextMeshProUGUI promptText;

    private BoardSpace currentSpace;

    public GameObject EndTurnUI;

    // Drag your UI Image (the card display) here in the Inspector
    public Image cardImageUI;

    public void Show(BoardSpace space)
    {
        currentSpace = space;

        // Update text
        promptText.text = $"Buy {space.spaceName} for ${space.price}?";

        // Update the card image (Sprite)
        if (space.cardimage != null)
        {
            cardImageUI.sprite = space.cardimage;
            cardImageUI.enabled = true;
        }
        else
        {
            cardImageUI.sprite = null;
            cardImageUI.enabled = false; // or leave enabled if you want an empty frame
        }
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