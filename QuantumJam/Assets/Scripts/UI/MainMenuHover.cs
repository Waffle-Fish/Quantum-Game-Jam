using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuHover : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] GameObject noHover;
    [SerializeField] GameObject hover;
    [SerializeField] bool disable;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(disable){return;}
        hover.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(disable){return;}
        hover.SetActive(false);
    }
    
}
