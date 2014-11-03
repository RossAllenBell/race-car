using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	public GameObject playerPrefab;

	//MasterServer.ipAddress = “127.0.0.1″;

	private string GetServerTypeName()
	{
		return Main.GameName + Main.Version;
	}

	private string GetNewRoomName()
	{
		return RandomString() + System.DateTime.UtcNow.ToString(" yyMMdd HH:mm");
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
	 
	private void StartServer()
	{
	    Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
	    MasterServer.RegisterHost(GetServerTypeName(), GetNewRoomName());
	}

	void OnServerInitialized()
	{
	    SpawnPlayer();
	}
	 
	void OnConnectedToServer()
	{
	    SpawnPlayer();
	}
	 
	private void SpawnPlayer()
	{
	    CarFollow.SetPlayer((GameObject) Network.Instantiate(playerPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity, 0));
	}

	private HostData[] hostList;
 
	private void RefreshHostList()
	{
	    MasterServer.RequestHostList(GetServerTypeName());
	}
	 
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
	    if (msEvent == MasterServerEvent.HostListReceived)
	        hostList = MasterServer.PollHostList();
	}

	private void JoinServer(HostData hostData)
	{
	    Network.Connect(hostData);
	}

	void Start ()
	{
	
	}
	
	void Update ()
	{
	
	}

	void OnGUI()
	{
	    if (!Network.isClient && !Network.isServer)
	    {
	        if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
	            StartServer();
	 
	        if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
	            RefreshHostList();
	 
	        if (hostList != null)
	        {
	            for (int i = 0; i < hostList.Length; i++)
	            {
	                if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
	                    JoinServer(hostList[i]);
	            }
	        }
	    }
	}
}
