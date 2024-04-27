using System;
using UnityEngine;


namespace ProjectComet.CoreGameplay.ScoringStatus {

    public interface IScoreDisplayController : IStatusDisplayController {

        void UpdateDisplay (int scoreValue);

    }
}