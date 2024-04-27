using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonTextColor : MonoBehaviour
{

    // Start is called before the first frame update

    public Text t;
    public void OnMouseEnter()
    {
       
        t.color = Color.black;
    }

    public void OnMouseExit()
    {

        t.color = Color.white;
    }

    
}
