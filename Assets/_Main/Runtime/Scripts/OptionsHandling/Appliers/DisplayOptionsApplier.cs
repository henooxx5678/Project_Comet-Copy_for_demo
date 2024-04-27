using System;
using UnityEngine;
using DoubleHeat.Common;

namespace ProjectComet.OptionsHandling {


    public class DisplayOptionsApplier : OptionsApplier {


        protected override void ApplyOptionsData(OptionsData data) {
            Screen.SetResolution(data.GetResolutaionValue().x, data.GetResolutaionValue().y, data.GetFullscreenValue());
        }

    }
}