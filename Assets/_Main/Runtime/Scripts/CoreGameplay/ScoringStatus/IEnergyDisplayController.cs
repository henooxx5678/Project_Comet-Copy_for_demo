using System;
using UnityEngine;


namespace ProjectComet.CoreGameplay.ScoringStatus {

    public interface IEnergyDisplayController : IStatusDisplayController {

        void UpdateDisplay (float energyValue);

    }
}