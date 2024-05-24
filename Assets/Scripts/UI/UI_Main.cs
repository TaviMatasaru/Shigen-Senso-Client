using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Main : MonoBehaviour
{
    [SerializeField] public GameObject _elements = null;
    [SerializeField] public GameObject _menuElements = null;
    [SerializeField] public TextMeshProUGUI _goldText = null;
    [SerializeField] public TextMeshProUGUI _gemsText = null;
    [SerializeField] public TextMeshProUGUI _woodText = null;
    [SerializeField] public TextMeshProUGUI _stoneText = null;
    [SerializeField] public TextMeshProUGUI _foodText = null;
    [SerializeField] private Button _shopButton = null;
    [SerializeField] private Button _newGameButton = null;
    [SerializeField] public Building[] _buildingsPrefabs = null;

    private static UI_Main _instance = null; public static UI_Main instance { get { return _instance; } }

    private bool _active = true; public bool isActive { get { return _active; } }

    private void Awake()
    {
        _instance = this;
        _elements.SetActive(false);
        _menuElements.SetActive(true);
    }

    private void Start()
    {
        _shopButton.onClick.AddListener(ShopButtonClicked);
        _newGameButton.onClick.AddListener(NewGameButtonClicked);
        
    }

    private void ShopButtonClicked()
    {
        SetStatus(false);
        UI_Shop.instance.SetStatus(true);
        UI_BuildingOptions.instance.SetStatus(false);
    }

    public void NewGameButtonClicked()
    {
        //TODO : send NewGame request to server and process the answer
        HexGridGenerator.Instance.GenerateGrid();
        _menuElements.SetActive(false);
        _elements.SetActive(true);
        
    }

    public void SetStatus(bool status)
    {
        _active = status;
        _elements.SetActive(status);
    }
}
