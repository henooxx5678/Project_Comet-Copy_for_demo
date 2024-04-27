using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DoubleHeat.UI {

    public static class TextDisplayTools {

        public static string GetTextOfTextDisplayComponent (Component textDisplayComponent) {

            if (textDisplayComponent is Text) {
                return ((Text) textDisplayComponent).text;
            }

            if (textDisplayComponent is TMP_Text) {
                return ((TMP_Text) textDisplayComponent).text;
            }

            return string.Empty;
        }

        public static void SetTextOfTextDisplayComponent (Component textDisplayComponent, string text) {

            if (textDisplayComponent is Text) {
                ((Text) textDisplayComponent).text = text;
            }
            else if (textDisplayComponent is TMP_Text) {
                ((TMP_Text) textDisplayComponent).text = text;
            }

        }

    }

}