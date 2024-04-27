using System;
using UnityEngine;
using DoubleHeat.Common;
using ProjectComet.CoreGameplay;
using ProjectComet.CoreGameplay.Display;

namespace ProjectComet.OptionsHandling {


    public class GamePlayOptionsApplier : OptionsApplier {

        [SerializeField] Sheet _sheet;
        [SerializeField] TracksDisplayController _trackDisplayController;



        protected override void ApplyOptionsData(OptionsData data) {
            if (_sheet) {
                _sheet.InputCompensationInSeconds = data.InputTimingCompensation;
            }

            if (_trackDisplayController) {
                _trackDisplayController.TravellingVelocity = data.NoteSpeed;
            }

        }

    }
}