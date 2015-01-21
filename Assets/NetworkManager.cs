using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	public static string CurrentRoomName;

	public GameObject playerPrefab;

	//MasterServer.ipAddress = “127.0.0.1″;

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

	    Main.Players.Add(Network.player, new PlayerStat(RandomString()));

        GameObject.Find("MenuUICanvas").SetActive(false);
	}

	void OnServerInitialized()
	{
	    SpawnPlayer();
	}
	 
	void OnConnectedToServer()
	{
	    SpawnPlayer();
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		 Main.Players.Add(player, new PlayerStat(RandomString()));
		 Main.PlayersUpdate = true;
	}
	 
	private void SpawnPlayer()
	{
		GameObject me = (GameObject) Network.Instantiate(playerPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
	    CarFollow.SetPlayer(me);
	    Main.Me = me;
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

        GameObject.Find("MenuUICanvas").SetActive(false);
	}

	public void OnPlayerDisconnected(NetworkPlayer player) {
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);

        Main.theInstance.networkView.RPC("RemovePlayerStat", RPCMode.All, player);
    }

	void Start ()
	{
	    RefreshHostList();
	}

}
