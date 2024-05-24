using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{

    private static Building _instance = null; public static Building instance { get { return _instance; } }

    [System.Serializable] public class Level
    {
        public int level = 1;
        public Sprite icon = null;
        public GameObject mesh = null;
    }
    //[SerializeField] private Level[] _levels = null;

    [SerializeField] private int _row;
    [SerializeField] private int _column;

    private int x_coord;
    private int y_coord;
}
