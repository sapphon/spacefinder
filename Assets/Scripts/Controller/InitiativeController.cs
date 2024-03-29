﻿using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;
using UnityEngine.UI;

namespace Controller
{
    public class InitiativeController : MonoBehaviour
    {
        private GameObject _initiativePanel;
        private Button _submitButton;
        public GameObject inputRowPrefab;
        private Dictionary<Ship, short> _initiativesThisRound;

        void Awake()
        {
            this._initiativesThisRound = new Dictionary<Ship, short>();
            this._initiativePanel = GameObject.Find("InitiativeInputPanel");
            this._submitButton = _initiativePanel.transform.Find("Controls").Find("SubmitButton").GetComponent<Button>();
            this._submitButton.onClick.AddListener(() => this.OnSubmit());
            this._initiativePanel.SetActive(false);
        }

        public void GatherInitiatives()
        {
            initializeEmptyPanel();
            this._initiativePanel.SetActive(true);
        }

        private void initializeEmptyPanel()
        {
            ClearInputPanel();
            this._initiativesThisRound.Clear();
            PopulateEmptyInputs();
        }

        private void ClearInputPanel()
        {
            Transform inputContainer = _initiativePanel.transform.Find("InitiativeInputs").transform;
            foreach (Transform child in inputContainer) {
                Destroy(child.gameObject);
            }
        }

        private void PopulateEmptyInputs()
        {
            foreach (Ship ship in Ship.getAllShips())
            {
                CreateInputForShip(ship);
            }
        }

        private void CreateInputForShip(Ship ship)
        {
            Transform inputRow =
                Instantiate(inputRowPrefab, _initiativePanel.transform.Find("InitiativeInputs")).transform;
            inputRow.Find("ShipNameReadout").GetComponent<Text>().text = ship.displayName;
            inputRow.Find("InitiativeInputField").GetComponent<InputField>().text = "??";
        }

        private Dictionary<string, short> ReadInputs()
        {
            Dictionary<string, short> toReturn = new Dictionary<string, short>();
            foreach (Transform inputTransform in _initiativePanel.transform.Find("InitiativeInputs"))
            {
                toReturn.Add(inputTransform.Find("ShipNameReadout").GetComponent<Text>().text,
                    short.Parse(inputTransform.Find("InitiativeInputField").GetComponent<InputField>().text));
            }

            return toReturn;
        }

        private Queue<Ship> createInitiativeQueue(Dictionary<Ship, short> initiatives)
        {
            return new Queue<Ship>(from entry in initiatives orderby entry.Value ascending select entry.Key);
        }

        public void OnSubmit()
        {
            Dictionary<string, short> inputs = ReadInputs();
            foreach (Ship ship in Ship.getAllShips())
            {
                _initiativesThisRound[ship] = inputs[ship.displayName];
            }
            this._initiativePanel.SetActive(false);
            SendInitiativesToPhaseManager();
        }

        public void SendInitiativesToPhaseManager()
        {
            FindObjectOfType<PhaseManager>()
                .SetShipInitiativeOrder(createInitiativeQueue(this._initiativesThisRound));
        }

        public Dictionary<Ship, short> GetInitiativesThisRound()
        {
            return _initiativesThisRound;
        }
    }
}