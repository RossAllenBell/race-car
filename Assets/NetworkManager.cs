using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	public static string CurrentRoomName;

	public GameObject playerPrefab;

    //MasterServer.ipAddress = “127.0.0.1″;

    void Start()
    {
        RefreshHostList();
    }

    public void Reset()
    {
        RefreshHostList();
        CurrentRoomName = null;
        Main.Players.Clear();
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
    }

	private string GetServerTypeName()
	{
		return Main.GameName + Main.Version;
	}

	private string GetNewRoomName()
	{
		return RandomString() + System.DateTime.UtcNow.ToString(" yyMMddHHmm");
	}

	private string RandomString()
	{
		char[] letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
		string s = "";
		for(int i=0; i<3; i++)
		{
			s += letters[(int)Mathf.Floor(Random.value * letters.Length)];
		}
		return s;
	}
	 
	public void StartServer()
	{
		CurrentRoomName = GetNewRoomName();
	    Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
	    MasterServer.RegisterHost(GetServerTypeName(), CurrentRoomName);

	    Main.Players.Add(Network.player, new PlayerStat(Main.theInstance.PlayerName));

        MenuManager.theInstance.HideStartMenu();
	}

	void OnServerInitialized()
	{
        SpawnPlayer();
	}
	 
	void OnConnectedToServer()
	{
	    SpawnPlayer();
        Main.theInstance.networkView.RPC("SetName", RPCMode.Server, Network.player, Main.theInstance.PlayerName);
	}

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        Reset();
    }

	void OnPlayerConnected(NetworkPlayer nPlayer)
	{
         Main.Players.Add(nPlayer, new PlayerStat(""));
		 Main.PlayersUpdate = true;

         Car[] cars = GameObject.FindObjectsOfType<Car>();
         foreach (Car car in cars)
         {
             if (car.networkView.owner != nPlayer)
             {
                 Color color = car.color;
                 car.networkView.RPC("SetColor", RPCMode.All, color.r, color.g, color.b);
             }
         }
	}
	 
	private void SpawnPlayer()
	{
		GameObject me = (GameObject) Network.Instantiate(playerPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
	    CarFollow.SetPlayer(me);
        Main.theInstance.Me = me.GetComponent<Car>();

        Color color = Main.theInstance.CarColors[(int)(Main.theInstance.CarColors.Length * Random.value)];
        Main.theInstance.Me.networkView.RPC("SetColor", RPCMode.All, color.r, color.g, color.b);
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

            GameObject serverListPanel = GameObject.Find("MenuUIServerPanel");

            foreach (Transform child in serverListPanel.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < hostList.Length; i++)
            {
                HostData hostData = hostList[i];
                GameObject serverButton = (GameObject)Instantiate(Resources.Load("server_button"));
                RectTransform rt = serverButton.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0, 0.75f - (i * 0.25f));
                rt.anchorMax = new Vector2(1, 1f - (i * 0.25f));
                serverButton.transform.SetParent(serverListPanel.transform, false);

                serverButton.GetComponent<Button>().onClick.AddListener(() => { JoinServer(hostData.gameName); });

                serverButton.GetComponentInChildren<Text>().text = hostData.gameName;
            }
        }
	}

    public void JoinServer(string gameName)
    {
        foreach (HostData hostData in hostList)
        {
            if (hostData.gameName == gameName)
            {
                JoinServer(hostData);
                return;
            }
        }
    }

    public void JoinServer(HostData hostData)
	{
		CurrentRoomName = hostData.gameName;
	    Network.Connect(hostData);

        MenuManager.theInstance.HideStartMenu();
	}

	public void OnPlayerDisconnected(NetworkPlayer player) {
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);

        Main.theInstance.networkView.RPC("RemovePlayerStat", RPCMode.All, player);
    }

}
