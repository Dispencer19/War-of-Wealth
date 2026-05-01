using UnityEngine;
using Photon.Pun;

/// <summary>
/// Network-synced UI manager that ensures all players see the same UI state.
/// Use this for board game UIs (rent, property purchase, chance cards, etc.)
/// </summary>
public class NetworkUIManager : MonoBehaviourPun
{
    public static NetworkUIManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // -------------------------
    // UI PANEL SYNC
    // -------------------------

    [PunRPC]
    private void RPC_ShowUI(string uiPanelName)
    {
        GameObject panel = GameObject.Find(uiPanelName);
        if (panel != null)
            panel.SetActive(true);
    }

    [PunRPC]
    private void RPC_HideUI(string uiPanelName)
    {
        GameObject panel = GameObject.Find(uiPanelName);
        if (panel != null)
            panel.SetActive(false);
    }

    public void ShowUISynced(string uiPanelName)
    {
        photonView.RPC("RPC_ShowUI", RpcTarget.All, uiPanelName);
    }

    public void HideUISynced(string uiPanelName)
    {
        photonView.RPC("RPC_HideUI", RpcTarget.All, uiPanelName);
    }

    // -------------------------
    // UI TEXT SYNC
    // -------------------------

    [PunRPC]
    private void RPC_UpdateUIText(string uiPanelName, string textComponentName, string textValue)
    {
        GameObject panel = GameObject.Find(uiPanelName);
        if (panel != null)
        {
            var textMesh = panel.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (textMesh != null)
                textMesh.text = textValue;
        }
    }

    public void UpdateUITextSynced(string uiPanelName, string textComponentName, string textValue)
    {
        photonView.RPC("RPC_UpdateUIText", RpcTarget.All, uiPanelName, textComponentName, textValue);
    }
}
