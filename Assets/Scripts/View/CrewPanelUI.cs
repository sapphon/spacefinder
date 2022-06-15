using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Controller.PhaseControllers;
using Model;
using Model.Crew;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class CrewPanelUI : MonoBehaviour, IShipSelectionObserver
    {
        public GameObject crewmemberButtonPrefab;
        private PhaseManager _phaseManager;
        private ShipUIManager _shipsUI;
        private Text _phaseRolesText;
        private Text _phaseRolesTitle;
        private Dictionary<Ship, CrewMember> _crewSelections;
        private Tuple<Ship, List<Crew.Role>> _lastRender;

        void Awake()
        {
            _lastRender = null;
            _crewSelections = new Dictionary<Ship, CrewMember>();
            _phaseManager = FindObjectOfType<PhaseManager>();
            _shipsUI = FindObjectOfType<ShipUIManager>();
            this._phaseRolesText = transform.parent.Find("PhaseAvailableRolesPanel").Find("CrewUIReadout")
                .GetComponent<Text>();
            this._phaseRolesTitle = transform.parent.Find("PhaseAvailableRolesPanel").Find("CrewUITitle")
                .GetComponent<Text>();
        }

        void Start()
        {
            this.SetActiveCharactersOrRoles();
            _shipsUI.AddObserver(this);
        }

        private void SetActiveCharactersOrRoles()
        {
            List<Crew.Role> activePhaseRoles = _phaseManager.GetActivePhaseRoles();
            Ship selectedShip = this._shipsUI.GetSelectedShip();
            if (selectedShip != null && selectedShip.crew.getMembers().Count > 0)
            {
                hidePhaseRoleText();
                showCrewPanelUI(selectedShip, activePhaseRoles);
            }
            else
            {
                clearCrewPanelUI();
                showPhaseRoleText(activePhaseRoles);
            }
        }

        private void clearCrewPanelUI()
        {
            for (int i = 0; i < _phaseRolesText.transform.childCount; i++)
            {
                GameObject.Destroy(_phaseRolesText.transform.GetChild(i).gameObject);
            }
        }

        private void hidePhaseRoleText()
        {
            _phaseRolesText.text = String.Empty;
        }

        private void showPhaseRoleText(List<Crew.Role> activePhaseRoles)
        {
            _phaseRolesTitle.text = "Active Roles".ToUpper();
            _phaseRolesText.text = String.Join("\r\n", activePhaseRoles.Select(role => role.ToString()));
        }

        private void showCrewPanelUI(Ship selectedShip, List<Crew.Role> activePhaseRoles)
        {
            
            _phaseRolesTitle.text = "Active Crew".ToUpper();
            List<CrewMember> crewMembers = selectedShip.crew.getMembers()
                .Where(crewperson => activePhaseRoles.Contains(crewperson.role)).ToList();
                clearCrewPanelUI();
                Util.logIfDebugging("Crewperson UI rerendered");
                for (int i = 0; i < crewMembers.Count; i++)
                {
                    int ughClosureProblem = i;
                    Button crewpersonButton =
                        GameObject.Instantiate(this.crewmemberButtonPrefab).GetComponent<Button>();
                    crewpersonButton.transform.SetParent(_phaseRolesText.transform, false);
                    RectTransform rectTransform = crewpersonButton.GetComponent<RectTransform>();
                    rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, i * rectTransform.rect.height,
                        rectTransform.rect.height);
                    crewpersonButton.onClick.AddListener(() =>
                    {
                        Util.logIfDebugging("Crewperson " + crewMembers[ughClosureProblem].name + " selected");
                        this._crewSelections.Remove(_shipsUI.GetSelectedShip());
                        this._crewSelections.Add(_shipsUI.GetSelectedShip(), crewMembers[ughClosureProblem]);
                        this.SetActiveCharactersOrRoles();
                    });
                    Text crewpersonText = crewpersonButton.GetComponentInChildren<Text>();
                    crewpersonText.text = crewMembers[i].name;
                    crewpersonText.color = crewMembers[i] == this.getSelectedCrewmember() ? Color.green : Color.black;
                }
        }

        private CrewMember getSelectedCrewmember()
        {
            Ship selectedShip = _shipsUI.GetSelectedShip();
            if (selectedShip == null || !this._crewSelections.ContainsKey(selectedShip))
            {
                return null;
            }

            return this._crewSelections[selectedShip];
        }

        public void ShipSelectionChanged(Ship newSelection)
        {
            SetActiveCharactersOrRoles();
        }
    }
}