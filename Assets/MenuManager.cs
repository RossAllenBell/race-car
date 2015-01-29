using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public static MenuManager theInstance;

    public GameObject StartMenuCanvas;
    public GameObject NameLabel;
    public GameObject ChangeNamePanel;
    public GameObject NameInputField;
    public GameObject UsePasswordToggle;

    public GameObject ServerPasswordCanvas;
    public GameObject ServerPasswordInputField;

    public GameObject ConnectingPasswordCanvas;
    public GameObject ConnectingPasswordInputField;
    public GameObject ConnectingPasswordAcceptButton;

    public GameObject ErrorCanvas;
    public GameObject ErrorLabel;

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

    public void ShowServerPasswordMenu()
    {
        ServerPasswordCanvas.SetActive(true);
    }

    public void HideServerPasswordMenu()
    {
        NetworkManager.theInstance.Password = ServerPasswordInputField.GetComponent<Text>().text;
        ServerPasswordCanvas.SetActive(false);
        NetworkManager.theInstance.StartServer(true);
    }

    public void ShowConnectingPasswordMenu(HostData hostData)
    {
        ConnectingPasswordAcceptButton.GetComponent<Button>().onClick.AddListener(() => { HideConnectingPasswordMenu(hostData); });
        ConnectingPasswordCanvas.SetActive(true);
    }

    public void HideConnectingPasswordMenu(HostData hostData)
    {
        string password = ConnectingPasswordInputField.GetComponent<Text>().text;
        ConnectingPasswordCanvas.SetActive(false);
        NetworkManager.theInstance.JoinServer(hostData, password);
    }

    public void ShowErrorMenu(string error)
    {
        ErrorCanvas.SetActive(true);
        ErrorLabel.GetComponent<Text>().text = error;
    }

    public void HideErrorMenu()
    {
        ErrorCanvas.SetActive(false);
        NetworkManager.theInstance.ResetNetworkState();
    }
    
}
