using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexTile : MonoBehaviour
{
    public LandType _landType;
    public Vector2Int _gridPosition;
    private Vector3 _originalScale;
    private Vector3 _originalPosition;

    public void Initialize(LandType landType, Vector2Int gridPosition)
    {
        this._landType = landType;
        this._gridPosition = gridPosition;       
    }

    void Awake()
    {
        _originalScale = transform.localScale;
        _originalPosition = transform.position;
    }

    void OnMouseDown()
    {        
        if (EventSystem.current.IsPointerOverGameObject())
            return;  
  
        HexGridGenerator.Instance.SelectTile(this);
    }

    public void Select()
    {
        transform.localScale = _originalScale * 1.3f;
        transform.position = new Vector3(_originalPosition.x, _originalPosition.y + 0.15f, _originalPosition.z);

        if(this._landType == LandType.FreeLand && HexGridGenerator.Instance._isCastleBuild == false)
        {
            UI_BuildingOptions.instance._castleElement.SetActive(true);
        }
        if(this._landType == LandType.PlayerFreeLand)
        {
            UI_BuildingOptions.instance._buildingElements.SetActive(true);
        }
    }

    public void Deselect()
    {
        transform.localScale = _originalScale;
        transform.position = _originalPosition;
        UI_BuildingOptions.instance.SetStatus(false);
    }

    public void RequestChangeLandType(LandType newLandType)
    {
        HexGridGenerator.Instance.ChangeTileLandType(this, newLandType);
    }

}
