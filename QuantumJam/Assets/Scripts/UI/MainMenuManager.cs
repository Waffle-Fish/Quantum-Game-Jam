using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> menus = new List<GameObject>();
    // 0=LevelSelect 1=Settings 2=Credits
    public void PlayButton()
    {
        foreach(GameObject menu in menus)
        {
            menu.SetActive(false);
        }
        menus[0].SetActive(true);
    }

    public void SettingsButton()
    {
        foreach(GameObject menu in menus)
        {
            menu.SetActive(false);
        }
        menus[1].SetActive(true);
    }

    public void CreditsButton()
    {
        foreach(GameObject menu in menus)
        {
            menu.SetActive(false);
        }
        menus[2].SetActive(true);
    }

    public void CloseButton()
    {
        foreach(GameObject menu in menus)
        {
            menu.SetActive(false);
        }
    }
}
