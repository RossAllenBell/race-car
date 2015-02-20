using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class NetworkManager : MonoBehaviour {

	public static string CurrentRoomName;
    public static NetworkManager theInstance;

	public GameObject playerPrefab;

    public string Password;
    public HostData HostData;

    void Start()
    {
        theInstance = this;
        ResetNetworkState();
    }

    public void ResetNetworkState()
    {
        Password = "";
        CurrentRoomName = null;
        HostData = null;
        Main.theInstance.ClearNPlayers();
        MenuManager.theInstance.ShowStartMenu();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            Destroy(go);
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Weapon"))
        {
            Destroy(go);
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("GoodyBox"))
        {
            Destroy(go);
        }
        RefreshHostList();
    }

    public void ManualDisconnect()
    {
        if (Network.isServer)
        {
            MasterServer.UnregisterHost();
            Network.Disconnect();
        }
        else if (Network.isClient)
        {
            Network.Disconnect();
        }
        ResetNetworkState();
    }

	private string GetServerTypeName()
	{
		return Main.GameName + Main.Version;
	}

	private string GetNewRoomName()
	{
		return RandomString() + System.DateTime.UtcNow.ToString(" yyMMddHHmm");
	}

    public static string RandomString(int length=3)
    {
        char[] letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        string s = "";
        for (int i = 0; i < length; i++)
        {
            s += letters[(int)Mathf.Floor(UnityEngine.Random.value * letters.Length)];
        }
        return s;
    }
	 
	public void StartServer(bool passwordExplicitlyEntered=false)
	{
        if (MenuManager.theInstance.UsePasswordToggle.GetComponent<Toggle>().isOn && (Password == "" && !passwordExplicitlyEntered))
        {
            MenuManager.theInstance.ShowServerPasswordMenu();
            return;
        }

        CurrentRoomName = GetNewRoomName();
        Network.incomingPassword = Password;
	    Network.InitializeServer(7, 25123, !Network.HavePublicAddress());
	    MasterServer.RegisterHost(GetServerTypeName(), CurrentRoomName);

        Main.theInstance.EnsureNPlayerExists(Network.player);
        Main.theInstance.SetPlayerName(Network.player, Main.theInstance.PlayerName);

        MenuManager.theInstance.HideStartMenu();
	}

	void OnServerInitialized()
	{
        SpawnPlayer();
	}
	 
	void OnConnectedToServer()
	{
	    SpawnPlayer();
        Main.theInstance.networkView.RPC("SetPlayerName", RPCMode.Server, Network.player, Main.theInstance.PlayerName);
	}

	void OnPlayerConnected(NetworkPlayer nPlayer)
	{
        Main.theInstance.EnsureNPlayerExists(nPlayer);

        Car[] cars = GameObject.FindObjectsOfType<Car>();
        foreach (Car car in cars)
        {
            if (car.networkView.owner != nPlayer)
            {
                Color color = car.color;
                car.networkView.RPC("SetColor", RPCMode.All, Network.player, color.r, color.g, color.b);
            }
        }

        ReconcileDeadWeaponsWithNewlyConnectedPlayer(nPlayer);
	}

    private void ReconcileDeadWeaponsWithNewlyConnectedPlayer(NetworkPlayer nPlayer)
    {
        foreach(DumbWeapon dw in FindObjectsOfType(typeof(DumbWeapon))){
            if(dw.bounced){
                dw.networkView.RPC("SetDead", nPlayer);
            }
        }
    }
	 
	private void SpawnPlayer()
	{
		GameObject me = (GameObject) Network.Instantiate(playerPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
	    CarFollow.SetPlayer(me);
        Main.theInstance.Me = me.GetComponent<Car>();

        Color color = Main.theInstance.CarColors[(int)(Main.theInstance.CarColors.Length * UnityEngine.Random.value)];
        me.networkView.RPC("SetColor", RPCMode.All, Network.player, color.r, color.g, color.b);
	}

	private HostData[] hostList;

    public void RefreshHostList()
	{
	    MasterServer.RequestHostList(GetServerTypeName());
	}
	 
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
        if (msEvent == MasterServerEvent.HostListReceived)
        {
            hostList = MasterServer.PollHostList();

            GameObject serverListPanel = MenuManager.theInstance.ServerListPanel;

            foreach (Transform child in serverListPanel.transform)
            {
                Destroy(child.gameObject);
            }

            Array.Sort(hostList, delegate(HostData host1, HostData host2)
            {
                return host1.gameName.CompareTo(host2.gameName);
            });

            if (hostList.Length > 0)
            {
                serverListPanel.SetActive(true);
            }
            else
            {
                serverListPanel.SetActive(false);
            }

            for (int i = 0; i < hostList.Length; i++)
            {
                HostData hostData = hostList[i];
                GameObject serverButton = (GameObject)Instantiate(Resources.Load("server_button"));

                serverButton.GetComponent<Button>().onClick.AddListener(() => { JoinServer(hostData); });
                serverButton.GetComponentInChildren<Text>().text = hostData.gameName + (hostData.passwordProtected ? " (pass)" : "");
                serverButton.transform.SetParent(serverListPanel.transform);
            }
        }
	}

    public void JoinServer(HostData hostData, string password="")
    {
        if (hostData.passwordProtected && password == "")
        {
            MenuManager.theInstance.ShowConnectingPasswordMenu(hostData);
            return;
        }

		CurrentRoomName = hostData.gameName;
        Network.Connect(hostData, password);

        MenuManager.theInstance.HideStartMenu();
	}

	public void OnPlayerDisconnected(NetworkPlayer player) {
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);

        Main.theInstance.networkView.RPC("RemovePlayerStat", RPCMode.All, player);
    }

    void OnFailedToConnectToMasterServer(NetworkConnectionError error)
    {
        MenuManager.theInstance.ShowErrorMenu(error.ToString());
    }

    private string lastFailedConnectError="";
    void OnFailedToConnect(NetworkConnectionError error)
    {
        if (error.ToString() != "ConnectionFailed" || lastFailedConnectError != "InvalidPassword")
        {
            MenuManager.theInstance.ShowErrorMenu(error.ToString());
        }
        lastFailedConnectError = error.ToString();
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        ResetNetworkState();
        if (!Network.isServer)
        {
            MenuManager.theInstance.ShowErrorMenu("Disconnected from server.");
        }
    }

}
