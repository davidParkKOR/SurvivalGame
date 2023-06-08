using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComputerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] ComputerKit theComputer;
    [SerializeField] int buttonNumber;


    public void OnPointerEnter(PointerEventData eventData)
    {
        theComputer.ShowToolTip(buttonNumber);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        theComputer.HideToolTip();
    }


}
