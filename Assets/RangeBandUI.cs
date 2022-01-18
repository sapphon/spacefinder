using System;
using System.Collections;
using System.Collections.Generic;
using Controller.PhaseControllers;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RangeBandUI : MonoBehaviour
{
    private HelmPhaseController _controller;
    private ShipUIManager _shipsUI;
    private Tilemap _overlayMap;

    void Awake()
    {
        _shipsUI = FindObjectOfType<ShipUIManager>();
        _overlayMap = GetComponent<Tilemap>();
    }

    void Update()
    {
        if (_shipsUI.GetSelectedShip() != null && _shipsUI.GetShowingRange() > 0)
        {
            _overlayMap.color = new Color(255, 255, 255, 51);
            Vector3Int shipPosition = _shipsUI.GetSelectedShip().gridPosition;
            for (int i = _overlayMap.cellBounds.xMin; i < _overlayMap.cellBounds.xMax; i++)
            {
                for (int j = _overlayMap.cellBounds.yMin;
                    j < _overlayMap.cellBounds.yMax;
                    j++)
                {
                    Vector3Int currentTilePosition = new Vector3Int(i, j, 0);
                    if(!_overlayMap.HasTile(currentTilePosition)) Debug.Log("tile coordinate error");
                    _overlayMap.SetColor(currentTilePosition, getColorForDistance(getDistance(shipPosition, currentTilePosition)));
                }
            }
            Debug.Log(_overlayMap.color);    //255,255,255,51 as expected.  When does it go full alpha?
        }
        else
        {
            _overlayMap.color = new Color(0, 0, 0, 0);
        }
    }

    private Color getColorForDistance(int distance)
    {
        if (distance <= 5) return new Color(255, 0, 0, 51);
        if (distance <= 10) return new Color(255, 255, 0, 51);
        if (distance <= 15) return new Color(255, 155, 0, 51);
        if (distance <= 20) return new Color(255, 0, 0, 51);
        return new Color(0, 0, 0, 0);
    }

    private int getDistance(Vector3Int origin, Vector3Int destination)
    {
        return Math.Abs(origin.x - destination.x + origin.y - destination.y);
    }
}