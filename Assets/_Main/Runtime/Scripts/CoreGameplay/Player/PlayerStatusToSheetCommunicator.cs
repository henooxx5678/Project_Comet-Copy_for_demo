using System;
using System.Collections;
using UnityEngine;
using DoubleHeat.Utilities;

namespace ProjectComet.CoreGameplay.Player {
    
    public class PlayerStatusToSheetCommunicator : MonoBehaviour {

        [SerializeField] PlayerStatus _playerStatus;
        [SerializeField] Sheet _sheet;


        protected virtual void OnEnable () {
            _playerStatus.MovementPositionChanged += OnPlayerMovementPositionChanged;
            SendPlayerMovementPositionUpdateToSheet();
        }

        protected virtual void OnDisable() {
            _playerStatus.MovementPositionChanged -= OnPlayerMovementPositionChanged;
        }

        protected void SendPlayerMovementPositionUpdateToSheet () {
            _sheet.UpdatePlayerPosition(ConvertToTrackIndexFromPlayerMovementPositionNumber(_playerStatus.CurrentMovementPositionNumber));
        }


        protected int ConvertToTrackIndexFromPlayerMovementPositionNumber (int playerMovementPositionNumber) {
            return playerMovementPositionNumber + 1;
        }



        void OnPlayerMovementPositionChanged (object sender, EventArgs args) {
            SendPlayerMovementPositionUpdateToSheet();
        }

    }
}