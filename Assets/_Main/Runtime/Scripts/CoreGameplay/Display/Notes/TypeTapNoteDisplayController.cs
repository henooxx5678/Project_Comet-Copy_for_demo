using System;
using System.Collections;
using UnityEngine;
using ProjectComet.CoreGameplay.Notes;

namespace ProjectComet.CoreGameplay.Display {

    public class TypeTapNoteDisplayController : NoteDisplayController {

        protected override void UpdateStatus() {
            base.UpdateStatus();
            if (HasBeenHit && hasOnsetReachedDestination) {
                Destroy(gameObject);
            }
        }

    }

}
