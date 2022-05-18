using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Controller.Input
{
    public class MouseInputBehavior : InputBehavior
    {
        private Tilemap _tilemap;
        public TileBase highlightedSpaceTile;

        private TileBase _prevTile;
        private Vector3Int _prevPosition;

        private void OnEnable()
        {
            getMouseMoveAction().performed += MousePositionChanged;
            getMouseClickAction().performed += MouseClicked;
            getMouseRightClickAction().performed += MouseRightClicked;
            getMouseScrollAction().performed += MouseScrolled;
        }

        private InputAction getMouseMoveAction()
        {
            return _input.actions["Mouse Position"];
        }
    
        private InputAction getMouseClickAction()
        {
            return _input.actions["Mouse Click"];
        }

        private InputAction getMouseRightClickAction()
        {
            return _input.actions["Target Ship"];
        }
        
        private InputAction getMouseScrollAction()
        {
            return _input.actions["Zoom"];
        }
    
        private void OnDisable()
        {
            getMouseMoveAction().performed -= MousePositionChanged;
            getMouseClickAction().performed -= MouseClicked;
            getMouseRightClickAction().performed -= MouseRightClicked;
            getMouseScrollAction().performed -= MouseScrolled;

        }

        void Start()
        {
            this._tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();

        
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
        
        private void MouseScrolled(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.ReadValue<Vector2>().y < 0)
            {
                CameraController.ZoomCameraOut();
            }
            else
            {
                CameraController.ZoomCameraIn();
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
    
        private void MouseRightClicked(InputAction.CallbackContext callbackContext)
        {
            if (ShouldTargetingControlsEnable() && !EventSystem.current.IsPointerOverGameObject())
            {
                Vector3Int tileCoordinates = GetMousePositionRelativeToTilemap();
                if (_tilemap.HasTile(tileCoordinates))
                {
                    _shipsUI.TryTargetShip(tileCoordinates);
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
}
