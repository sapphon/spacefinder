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
    public class CrewPanelUI : MonoBehaviour
    {
        private PhaseManager _phaseManager;
        private ShipUIManager _shipsUI;
        private Text _phaseRolesText;
        private Text _phaseRolesTitle;

        void Awake()
        {
            _phaseManager = FindObjectOfType<PhaseManager>();
            _shipsUI = FindObjectOfType<ShipUIManager>();
            this._phaseRolesText = transform.parent.Find("PhaseAvailableRolesPanel").Find("CrewUIReadout")
                .GetComponent<Text>();
            this._phaseRolesTitle = transform.parent.Find("PhaseAvailableRolesPanel").Find("CrewUITitle")
                .GetComponent<Text>();
        }

        void Update()
        {
            SetActiveCharactersOrRoles();
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
                showPhaseRoleText(activePhaseRoles);
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
            _phaseRolesText.text = String.Join("\r\n",
                selectedShip.crew.getMembers().Where(crewperson => activePhaseRoles.Contains(crewperson.role))
                    .Select(crewperson => crewperson.name));
        }
    }
}