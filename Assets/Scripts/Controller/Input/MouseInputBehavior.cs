using UnityEngine;
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
            getCursorMoveAction().performed += cursorPositionChanged;
            getPrimaryAction().performed += mainButtonPressed;
            getSecondaryAction().performed += alternatePressed;
            getZoomAction().performed += zoomChanged;
        }

        private InputAction getCursorMoveAction()
        {
            return _input.actions["Cursor Position"];
        }
    
        private InputAction getPrimaryAction()
        {
            return _input.actions["Primary Action"];
        }

        private InputAction getSecondaryAction()
        {
            return _input.actions["Secondary Action"];
        }
        
        private InputAction getZoomAction()
        {
            return _input.actions["Zoom"];
        }
    
        private void OnDisable()
        {
            getCursorMoveAction().performed -= cursorPositionChanged;
            getPrimaryAction().performed -= mainButtonPressed;
            getSecondaryAction().performed -= alternatePressed;
            getZoomAction().performed -= zoomChanged;

        }

        void Start()
        {
            this._tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();

        
        }

        private void cursorPositionChanged(InputAction.CallbackContext callbackContext)
        {
            Vector3Int mousePosition = getMousePositionRelativeToTilemap();
            if (_tilemap.HasTile(mousePosition))
            {
                undoPreviousHighlight();
                setHighlight(mousePosition);
            }
        }
        
        private void zoomChanged(InputAction.CallbackContext callbackContext)
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
    
        private void mainButtonPressed(InputAction.CallbackContext callbackContext)
        {
            if (!Util.IsMouseOverUI())
            {
                Vector3Int tileCoordinates = getMousePositionRelativeToTilemap();
                if (_tilemap.HasTile(tileCoordinates))
                {
                    _shipsUI.TrySelectShip(tileCoordinates);
                }
            }
        }
    
        private void alternatePressed(InputAction.CallbackContext callbackContext)
        {
            if (!ShouldTargetingControlsEnable() || Util.IsMouseOverUI()) return;
            Vector3Int tileCoordinates = getMousePositionRelativeToTilemap();
            if (_tilemap.HasTile(tileCoordinates))
            {
                _shipsUI.TryTargetShip(tileCoordinates);
            }
        }

        private void setHighlight(Vector3Int mousePosition)
        {
            _prevTile = _tilemap.GetTile(mousePosition);
            _prevPosition = mousePosition;
            _tilemap.SetTile(mousePosition, highlightedSpaceTile);
        }

        private void undoPreviousHighlight()
        {
            if (this._prevTile)
            {
                _tilemap.SetTile(_prevPosition, _prevTile);
            }
        }

        private Vector3Int getMousePositionRelativeToTilemap()
        {
            if (Camera.main != null)
            {
                return _tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(getCursorMoveAction().ReadValue<Vector2>()));
            }

            Util.logIfDebugging("Main camera not found when mouse moved!");
            return Vector3Int.zero;
        }
    }
}
