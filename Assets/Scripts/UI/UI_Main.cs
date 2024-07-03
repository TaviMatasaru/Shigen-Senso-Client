using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DevelopersHub.RealtimeNetworking.Client;
using System;

public class UI_Main : MonoBehaviour
{
    [SerializeField] public GameObject _loginElements = null;
    [SerializeField] public GameObject _registerElements = null;
    [SerializeField] public GameObject _elements = null;
    [SerializeField] public GameObject _menuElements = null;
    [SerializeField] public GameObject _searchingElements = null;
    [SerializeField] public GameObject _connectingToServerElements = null;

    [SerializeField] public TextMeshProUGUI _opponentNameText = null;
    [SerializeField] public TextMeshProUGUI _opponentVictoriesText = null;
    [SerializeField] public TextMeshProUGUI _opponentRankText = null;


    //[SerializeField] public TextMeshProUGUI _goldText = null;
    //[SerializeField] public TextMeshProUGUI _gemsText = null;
    [SerializeField] public TextMeshProUGUI _woodText = null;
    [SerializeField] public TextMeshProUGUI _stoneText = null;
    [SerializeField] public TextMeshProUGUI _foodText = null;

    [SerializeField] public TextMeshProUGUI _loginErrorText = null;
    [SerializeField] public TextMeshProUGUI _registerErrorText = null;

    [SerializeField] public TextMeshProUGUI _menuUsernameText = null;
    [SerializeField] public TextMeshProUGUI _yourVictoriesText = null;
    [SerializeField] public TextMeshProUGUI _yourRankText = null;

    [SerializeField] public TMP_InputField _loginUsernameInput = null;
    [SerializeField] public TMP_InputField _loginPasswordInput = null;
    [SerializeField] public TMP_InputField _registerUsernameInput = null;
    [SerializeField] public TMP_InputField _registerPasswordInput = null;
    [SerializeField] public TMP_InputField _registerRetypePasswordInput = null;


    [SerializeField] private Button _loginButton = null;
    [SerializeField] private Button _registerButton = null;

    [SerializeField] private Button _goRegisterButton = null;
    [SerializeField] private Button _goLoginButton = null;

    [SerializeField] private Button _openGameMenuButton = null;

    [SerializeField] private Button _searchButton = null;
    [SerializeField] private Button _menuLogoutButton = null;

    [SerializeField] private Button _menuExitGameButton = null;

    //[SerializeField] private Button _inGameExitGameButton = null;
    //[SerializeField] private Button _endGameExitGameButton = null;

    [SerializeField] private Button _loginExitGameButton = null;
    [SerializeField] private Button _registerExitGameButton = null;

    [SerializeField] private Button _stopSearchingButton = null;

    [SerializeField] private List<Image> PlayerColorImages = null;



    private static UI_Main _instance = null; public static UI_Main instance { get { return _instance; } }

    private bool _active = true; public bool isActive { get { return _active; } }

    private void Awake()
    {
        _instance = this;

        _loginErrorText.text = "";
        _registerErrorText.text = "";
        _loginPasswordInput.contentType = TMP_InputField.ContentType.Password;
        _registerPasswordInput.contentType = TMP_InputField.ContentType.Password;
        _registerRetypePasswordInput.contentType = TMP_InputField.ContentType.Password;

        _connectingToServerElements.SetActive(true);
        _loginElements.SetActive(false);
        _registerElements.SetActive(false);
        _elements.SetActive(false);
        _menuElements.SetActive(false);
        _searchingElements.SetActive(false);       
    }

    private void Start()
    {
        _loginButton.onClick.AddListener(LoginButtonClicked);
        _goLoginButton.onClick.AddListener(GoLoginButtonClicked);
        _loginExitGameButton.onClick.AddListener(ExitGameButtonClicked);

        _registerButton.onClick.AddListener(RegisterButtonClicked);
        _goRegisterButton.onClick.AddListener(GoRegisterButtonClicked);
        _registerExitGameButton.onClick.AddListener(ExitGameButtonClicked);

        _searchButton.onClick.AddListener(SearchButtonClicked);
        _menuLogoutButton.onClick.AddListener(LogoutButtonClicked);
        _menuExitGameButton.onClick.AddListener(ExitGameButtonClicked);

        _stopSearchingButton.onClick.AddListener(StopSearchingButtonClicked);

        _openGameMenuButton.onClick.AddListener(OpenGameMenuClicked);        
    }

    private void LoginButtonClicked()
    {        

        string loginUsername = _loginUsernameInput.text;
        string loginPassword = _loginPasswordInput.text;

        string device = SystemInfo.deviceUniqueIdentifier;
        Packet packet = new Packet();
        packet.Write((int)Player.RequestsID.LOGIN);
        packet.Write(device);
        packet.Write(loginUsername);
        packet.Write(loginPassword);
        Sender.TCP_Send(packet);
    }

    public void RegisterButtonClicked()
    {
        

        string registerUsername = _registerUsernameInput.text;
        string registerPassword = _registerPasswordInput.text;
        string registerRetypePassword = _registerRetypePasswordInput.text;

        if(registerPassword == registerRetypePassword)
        {
            _registerErrorText.text = "";
            string device = SystemInfo.deviceUniqueIdentifier;
            Packet packet = new Packet();
            packet.Write((int)Player.RequestsID.REGISTER);
            packet.Write(device);
            packet.Write(registerUsername);
            packet.Write(registerPassword);
            Sender.TCP_Send(packet);
        }
        else
        {
            _registerErrorText.text = "Passwords do not match!";
            _registerPasswordInput.text = "";
            _registerRetypePasswordInput.text = "";
        }
    }

    private void GoLoginButtonClicked()
    {
        _registerErrorText.text = "";
        _loginErrorText.text = "";
        _loginUsernameInput.text = _registerUsernameInput.text;
        _loginPasswordInput.text = "";

        _loginElements.SetActive(true);
        _registerElements.SetActive(false);
    }

    private void GoRegisterButtonClicked()
    {
        _registerUsernameInput.text = _loginUsernameInput.text;
        _registerPasswordInput.text = "";
        _registerRetypePasswordInput.text = "";
        _registerErrorText.text = "";
        _loginErrorText.text = "";
        _registerElements.SetActive(true);
        _loginElements.SetActive(false);
    }

    private void LogoutButtonClicked()
    {
        Packet packet = new Packet();
        packet.Write((int)Player.RequestsID.LOGOUT);
        packet.Write(Player.instance.initializationData.username);
        Sender.TCP_Send(packet);
    }

    private void ExitGameButtonClicked()
    {
        Application.Quit();
    }

    private void OpenGameMenuClicked()
    {
        SetStatus(false);
        UI_InGameMenu.instance.SetStatus(true);
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

    public void SetPlayerUIColor(int isPlayer1)
    {
        foreach(Image image in PlayerColorImages)
        {
            if (isPlayer1 == 1)
            {              
                image.color =  new Color32(255, 247, 120, 140); // yellow
            }
            else
            {                
                image.color = new Color32(148, 197, 255, 140); // blue
            }
        }
        
    }
}
