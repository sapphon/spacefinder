using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class MouseInputBehavior : MonoBehaviour
{
    private MouseInput _input;
    private Tilemap _tilemap;
    public TileBase highlightedSpaceTile;

    private TileBase _prevTile;
    private Vector3Int _prevPosition;

    void Awake()
    {
        _input = new MouseInput();
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    void Start()
    {
        _tilemap = FindObjectOfType<Tilemap>();
        _input.Mouse.Mouseposition.performed += _ => MousePositionChanged();
    }

    private void MousePositionChanged()
    {
        Vector3Int mousePosition = GetMousePositionRelativeToTilemap();
        if (_tilemap.HasTile(mousePosition))
        {
            undoPreviousHighlight();
            setHighlight(mousePosition);
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
        return _tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(_input.Mouse.Mouseposition.ReadValue<Vector2>()));
    }
}
