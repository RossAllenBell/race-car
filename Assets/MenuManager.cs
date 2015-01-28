using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public static MenuManager theInstance;

    public GameObject StartMenuCanvas;
    public GameObject NameLabel;
    public GameObject ChangeNamePanel;
    public GameObject NameInputField;

    public void Start()
    {
        theInstance = this;
    }

    public void ShowStartMenu()
    {
        NameLabel.GetComponent<Text>().text = Main.theInstance.PlayerName;
        StartMenuCanvas.SetActive(true);
    }

    public void HideStartMenu()
    {
        StartMenuCanvas.SetActive(false);
    }

    public void ShowChangeNameMenu()
    {
        ChangeNamePanel.SetActive(true);
    }

    public void HideChangeNameMenu()
    {
        Main.theInstance.PlayerName = NameInputField.GetComponent<Text>().text;
        ShowStartMenu();
        ChangeNamePanel.SetActive(false);
    }
    
}
