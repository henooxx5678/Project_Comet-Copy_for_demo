using System.Collections.Generic;
using UnityEngine;

namespace ProjectComet.CoreGameplay.Display.Curtains {
    
    public class BlockedByCurtainsController : MonoBehaviour {

        [SerializeField] bool _isBlockedByCurtains = true;

        [Header("REFS")]
        [SerializeField] GameObject _displayObject;

        void Update () {

            if (_isBlockedByCurtains) {

                bool isInfrontOfAllCurtains = true;

                foreach (Curtain curtain in Curtain.Instances) {
                    if (Vector3.Dot(transform.position - curtain.transform.position, curtain.transform.forward) < 0) {
                        // Is behind the curtain
                        isInfrontOfAllCurtains = false;
                        break;
                    }
                }

                if (_displayObject) {
                    _displayObject.SetActive(isInfrontOfAllCurtains);
                }
            }

        }

    }

}