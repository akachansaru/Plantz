using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Button loadGameButton;

    void Start()
    {
        if (GlobalControl.Instance.savedValues.SaveFile == "")
        {
            loadGameButton.interactable = false;
        }
    }

    public void CheckIfCreateNewGame()
    {
        //if new game button pressed, ask if the user wants to erase old game and create new
    }

}
