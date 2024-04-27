using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseSelect : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        anim.SetBool("OnSelect", true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        anim.SetBool("OnSelect", true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        anim.SetBool("OnSelect", false);
    }
}
