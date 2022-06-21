using Controller;
using Model;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class ShipDoneWithPhaseUI : MonoBehaviour
    {
        private PhaseManager _phaseManager;
        private Slider _phaseFinishedSignal;
        private Text _phaseFinishedText;
        private Color _cautionOrange;
        private ShipUIManager _shipsUiManager;

        void Awake()
        {
            this._phaseManager = FindObjectOfType<PhaseManager>();
            this._phaseFinishedSignal = transform.Find("EndPhaseOKSlider").GetComponent<Slider>();
            this._phaseFinishedSignal.onValueChanged.AddListener(ToggleDone);
            this._phaseFinishedText = transform.Find("EndPhaseOKReadout").GetComponent<Text>();
            this._cautionOrange = new Color(.886f, .435f, .02f);
            this._shipsUiManager = FindObjectOfType<ShipUIManager>();

        }

        void Update()
        {
            Ship selectedShip = getSelectedShip();
            if (selectedShip != null)
            {
                GrowPanel();
                if (_phaseManager.isShipDone(selectedShip))
                {
                    _phaseFinishedText.text = "!!! DONE WITH PHASE !!!";
                    _phaseFinishedText.color = _cautionOrange;
                    ColorBlock colorBlock = ColorBlock.defaultColorBlock;
                    colorBlock.normalColor = _cautionOrange;
                    colorBlock.selectedColor = _cautionOrange;
                    _phaseFinishedSignal.colors = colorBlock;
                    _phaseFinishedSignal.SetValueWithoutNotify(1);
                }
                else
                {
                    _phaseFinishedText.text = "Done w/ Phase?";
                    _phaseFinishedText.color = new Color(50, 50, 50);
                    _phaseFinishedSignal.colors = ColorBlock.defaultColorBlock;
                    _phaseFinishedSignal.SetValueWithoutNotify(0);
                }
            }
            else
            {
                ShrinkPanel();
            }
        }

        void ToggleDone(float dontCare)
        {
            Ship selectedShip = getSelectedShip();
            if (_phaseManager.isShipDone(selectedShip))
            {
                _phaseManager.SignalStillWorking(selectedShip);
            }
            else
            {
                _phaseManager.SignalComplete(selectedShip);
            }
        }
        
        private Ship getSelectedShip()
        {
            return _shipsUiManager.GetSelectedShip();
        }

        protected void GrowPanel()
        {
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 60f);
            _phaseFinishedSignal.GetComponent<RectTransform>()
                .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30f);
            _phaseFinishedSignal.gameObject.transform.Find("Handle Slide Area/Handle").GetComponent<RectTransform>()
                .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30f);
        }

        protected void ShrinkPanel()
        {
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
            _phaseFinishedSignal.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
            _phaseFinishedSignal.gameObject.transform.Find("Handle Slide Area/Handle").GetComponent<RectTransform>()
                .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
        }
    }
}