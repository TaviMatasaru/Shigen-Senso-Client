using System.Collections;
using System.Collections.Generic;
using DevelopersHub.RealtimeNetworking.Client;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Shop : MonoBehaviour
{
    [SerializeField] public GameObject _elements = null;
    [SerializeField] public GameObject _endGameElements = null;


    [SerializeField] public TextMeshProUGUI _matchResultText = null;
    [SerializeField] public TextMeshProUGUI _resultReasonText = null;

    [SerializeField] private Button _closeGameMenuButton = null;

    [SerializeField] private Button _leaveMatchButton = null;
    [SerializeField] private Button _exitGameButton = null;

    [SerializeField] private Button _endLeaveMatchButton = null;
    [SerializeField] private Button _endExitGameButton = null;


    private static UI_Shop _instance = null; public static UI_Shop instance { get { return _instance; } }

    private void Awake()
    {
        _instance = this;
        _elements.SetActive(false);
        _endGameElements.SetActive(false);
    }

    private void Start()
    {
        _closeGameMenuButton.onClick.AddListener(CloseGameMenuButtonClicked);
        _leaveMatchButton.onClick.AddListener(LeaveMatchButtonClicked);
        _exitGameButton.onClick.AddListener(ExitGameButtonClicked);
        _endLeaveMatchButton.onClick.AddListener(LeaveMatchButtonClicked);
        _endExitGameButton.onClick.AddListener(ExitGameButtonClicked);
    }

    public void SetStatus(bool status)
    {    
        _elements.SetActive(status);
    }

    private void CloseGameMenuButtonClicked()
    {
        SetStatus(false);
        UI_Main.instance._elements.SetActive(true);
    }


    private void LeaveMatchButtonClicked()
    {
        Packet packet = new Packet();
        packet.Write((int)Player.RequestsID.LEAVE_MATCH);
        packet.Write(Player.instance.initializationData.accountID);
        Sender.TCP_Send(packet);

    }


    private void ExitGameButtonClicked()
    {
        Application.Quit();
    }
}
