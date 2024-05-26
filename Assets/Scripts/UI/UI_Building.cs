using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DevelopersHub.RealtimeNetworking.Client;

public class UI_Building : MonoBehaviour
{
    [SerializeField] private int _prefabIndex = 0;
    [SerializeField] private Button _button = null;

    private void Start()
    {

        _button.onClick.AddListener(Clicked);
    }

    private void Clicked()
    {
        UI_Shop.instance.SetStatus(false);
        UI_Main.instance.SetStatus(true);

        Vector3 position = Vector3.zero;

        Building building = Instantiate(UI_Main.instance._buildingsPrefabs[_prefabIndex], position, Quaternion.identity);
    }

    public void ConfirmBuild()
    {    
    }

}
