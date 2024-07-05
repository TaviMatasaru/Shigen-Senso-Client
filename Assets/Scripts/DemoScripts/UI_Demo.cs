using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Demo : MonoBehaviour
{
    [SerializeField] public GameObject _elements;

    [SerializeField] public TextMeshProUGUI _mountainText = null;
    [SerializeField] public TextMeshProUGUI _forestText = null;
    [SerializeField] public TextMeshProUGUI _cropsText = null;

    private static UI_Demo _instance = null; public static UI_Demo instance { get { return _instance; } }

    private void Awake()
    {
        _instance = this;

        _elements.SetActive(true);
        _mountainText.text = "10%";
        _forestText.text = "10%";
        _cropsText.text = "10%";
    }
}
