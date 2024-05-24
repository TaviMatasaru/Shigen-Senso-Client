using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Menu : MonoBehaviour
{
    [SerializeField] public GameObject _elements = null;
    [SerializeField] public Button _newGameButton = null;

    private static UI_Menu _instance = null; public static UI_Menu instance { get { return _instance; } }


    private void Awake()
    {
        _instance = this;
        _elements.SetActive(true);
    }

    private void Start()
    {
        _newGameButton.onClick.AddListener(NewGameButtonClicked);
    }

    public void SetStatus(bool status)
    {
        _elements.SetActive(status);
    }

    public void NewGameButtonClicked()
    {
        //TODO : send NewGame request to server and process the answer
        //HexGridGenerator.Instance.GenerateGrid();

        _elements.SetActive(false);
        UI_Main.instance.SetStatus(true);
    }
}
