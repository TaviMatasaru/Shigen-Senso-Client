using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DevelopersHub.RealtimeNetworking.Client;

public class UI_Main : MonoBehaviour
{
    [SerializeField] public GameObject _elements = null;
    [SerializeField] public GameObject _menuElements = null;
    [SerializeField] public GameObject _searchingElements = null;

    [SerializeField] public TextMeshProUGUI _goldText = null;
    [SerializeField] public TextMeshProUGUI _gemsText = null;
    [SerializeField] public TextMeshProUGUI _woodText = null;
    [SerializeField] public TextMeshProUGUI _stoneText = null;
    [SerializeField] public TextMeshProUGUI _foodText = null;

    [SerializeField] private Button _shopButton = null;
    [SerializeField] private Button _searchButton = null;
    [SerializeField] private Button _stopSearchingButton = null;

    private static UI_Main _instance = null; public static UI_Main instance { get { return _instance; } }

    private bool _active = true; public bool isActive { get { return _active; } }

    private void Awake()
    {
        _instance = this;
        _elements.SetActive(false);
        _menuElements.SetActive(true);
        _searchingElements.SetActive(false);
    }

    private void Start()
    {
        _shopButton.onClick.AddListener(ShopButtonClicked);
        _searchButton.onClick.AddListener(SearchButtonClicked);
        _stopSearchingButton.onClick.AddListener(StopSearchingButtonClicked);
    }

    private void ShopButtonClicked()
    {
        SetStatus(false);
        UI_Shop.instance.SetStatus(true);
        UI_BuildingOptions.instance.SetStatus(false);
    }

    public void SearchButtonClicked()
    {
        Packet packetToSend = new Packet();
        packetToSend.Write((int)Player.RequestsID.SEARCH);
        Sender.TCP_Send(packetToSend);


        _menuElements.SetActive(false);
        _searchingElements.SetActive(true); 
    }

    public void StopSearchingButtonClicked()
    {
        Packet packetToSend = new Packet();
        packetToSend.Write((int)Player.RequestsID.CANCEL_SEARCH);
        Sender.TCP_Send(packetToSend);

        _searchingElements.SetActive(false);
        _menuElements.SetActive(true);       
    }


    public void SetStatus(bool status)
    {
        _active = status;
        _elements.SetActive(status);
    }
}
