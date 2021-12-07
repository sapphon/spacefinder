using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class MouseInputBehavior : MonoBehaviour
{
    private PlayerInput _input;
    private Tilemap _tilemap;
    private ShipUIManager _shipsUI;
    public TileBase highlightedSpaceTile;

    private TileBase _prevTile;
    private Vector3Int _prevPosition;
   

    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _shipsUI = FindObjectOfType<ShipUIManager>();
    }

    private void OnEnable()
    {
        getMouseMoveAction().performed += MousePositionChanged;
        getMouseClickAction().performed += MouseClicked;
    }

    private InputAction getMouseMoveAction()
    {
        return _input.actions["Mouse Position"];
    }
    
    private InputAction getMouseClickAction()
    {
        return _input.actions["Mouse Click"];
    }

    private void OnDisable()
    {
        getMouseMoveAction().performed -= MousePositionChanged;
        getMouseClickAction().performed -= MouseClicked;
    }

    void Start()
    {
        _tilemap = FindObjectOfType<Tilemap>();
        
    }

    private void MousePositionChanged(InputAction.CallbackContext callbackContext)
    {
        Vector3Int mousePosition = GetMousePositionRelativeToTilemap();
        if (_tilemap.HasTile(mousePosition))
        {
            undoPreviousHighlight();
            setHighlight(mousePosition);
        }
    }
    
    private void MouseClicked(InputAction.CallbackContext callbackContext)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Vector3Int tileCoordinates = GetMousePositionRelativeToTilemap();
            if (_tilemap.HasTile(tileCoordinates))
            {
                _shipsUI.TrySelectShip(tileCoordinates);
            }
        }
    }

    private void setHighlight(Vector3Int mousePosition)
    {
        this._prevTile = _tilemap.GetTile(mousePosition);
        this._prevPosition = mousePosition;
        _tilemap.SetTile(mousePosition, highlightedSpaceTile);
    }

    private void undoPreviousHighlight()
    {
        if (this._prevTile)
        {
            _tilemap.SetTile(_prevPosition, _prevTile);
        }
    }

    Vector3Int GetMousePositionRelativeToTilemap()
    {
        return _tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(getMouseMoveAction().ReadValue<Vector2>()));
    }
}
