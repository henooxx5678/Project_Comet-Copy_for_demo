using UnityEngine;

namespace ProjectComet.Debuging {


    
    public class ResolutaionAdjuster : MonoBehaviour {
        
        public void SetResFHD () {
            Screen.SetResolution(1920, 1080, false);
        }

        public void SetResHDPlus () {
            Screen.SetResolution(1600, 900, false);
        }

        public void SetResHD () {
            Screen.SetResolution(1280, 720, false);
        }

        public void SetToNativeFullscreen () {
            Display mainDisplay = Display.main;
            Screen.SetResolution(mainDisplay.systemWidth, mainDisplay.systemHeight, true);
        }
    }
}