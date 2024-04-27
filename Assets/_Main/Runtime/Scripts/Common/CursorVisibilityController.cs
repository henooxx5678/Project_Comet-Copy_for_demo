using System;
using UnityEngine;


namespace ProjectComet.Common {
    
    public class CursorVisibilityController : MonoBehaviour {
        
        public void SetCursorVisible (bool visible) {
            Cursor.visible = visible;
        }

    }

}