using UnityEngine;
using System.Collections.Generic;

public class Main : MonoBehaviour {

    public const string Version = "0.5.0";
    public const string GameName = "Battle Karts";

	public const float NormalWidth = 1920;
    public const float NormalHeight = 1080;

	public static bool Clicked { get { return click; } }
	public static Vector2 TouchLocation { get { return touchLocation; } }
	public static Vector2 TouchGuiLocation { get { return TouchLocationToGuiLocation(touchLocation); } }
    public static bool Touching { get { return touching; } }

    private static bool click;
    private static Vector2 touchLocation;
    private static bool touching;

    public static float GuiRatioWidth;
    public static float GuiRatioHeight;
    public static float GuiRatio;

    private const int NormalLargestFont = 300;
    public static int FontLargest;
    public static int FontMedium;
    public static int FontSmall;
    public static int FontSmallest;

    public Car Me;
    public string PlayerName;

    public static List<GameObject> GoodyBoxes;

    public GameObject goodyBoxPrefab;

    public static Dictionary<NetworkPlayer, PlayerStat> Players;
    public static bool PlayersUpdate;

    public static Main theInstance;

    public Color[] CarColors;

	public void Start () {
        theInstance = this;

		Screen.orientation = ScreenOrientation.Landscape;

        GuiRatioWidth = (float) Screen.width / (float) NormalWidth;
        GuiRatioHeight = (float) Screen.height / (float) NormalHeight;
        GuiRatio = Mathf.Min(GuiRatioWidth, GuiRatioHeight);

        FontLargest = (int)(NormalLargestFont * GuiRatio);
        FontMedium = (int)(NormalLargestFont * 0.5 * GuiRatio);
        FontSmall = (int)(NormalLargestFont * 0.2 * GuiRatio);
        FontSmallest = (int) (NormalLargestFont * 0.1 * GuiRatio);

        Players = new Dictionary<NetworkPlayer, PlayerStat>();
        PlayersUpdate = false;

        GoodyBoxes = new List<GameObject>();

        PlayerName = "Player " + NetworkManager.RandomString(2);
        MenuManager.theInstance.ShowStartMenu();
	}
	
	public void Update () {
        if (Network.isServer) {
            for (int i = GoodyBoxes.Count - 1; i >= 0; i--) {
                if (GoodyBoxes[i] == null) {
                    GoodyBoxes.RemoveAt(i);
                }
            }

            while (GoodyBoxes.Count < (Network.connections.Length + 1) * 2) {
                float x = 90f * Random.value * (Random.value < 0.5f? 1 : -1);
                float y = 24f;
                float z = 90f * Random.value * (Random.value < 0.5f? 1 : -1);
                GameObject goodyBox = (GameObject) Network.Instantiate(goodyBoxPrefab, new Vector3(x, y, z), Quaternion.identity, 0);
                GoodyBoxes.Add(goodyBox);
            }

            if(PlayersUpdate){
                PlayersUpdate = false;
                foreach(NetworkPlayer nPlayer in Players.Keys){
                    gameObject.networkView.RPC("UpdatePlayerStat", RPCMode.All, nPlayer, Players[nPlayer].name, Players[nPlayer].score);
                }
            }
        }
    }

    public void OnGUI(){
        if (Input.touchCount > 0 || Input.GetMouseButton (0)) {
            Vector2 tempLocation = Input.touchCount > 0 ? Input.GetTouch (0).position : (Vector2)Input.mousePosition;
            touchLocation = new Vector2 (tempLocation.x, tempLocation.y);
            click = !touching;
            touching = true;
        } else {
            click = false;
            touching = false;
        }
	}

    public static Vector2 TouchLocationToGuiLocation (Vector2 touchLocation)
    {
        return new Vector2 (touchLocation.x, Screen.height - touchLocation.y);
    }

    public static bool TouchingIn(Rect GuiSpaceRect) {
        for (int i=0; i<Input.touchCount; i++) {
            if (GuiSpaceRect.Contains(TouchLocationToGuiLocation(Input.GetTouch(i).position))) {
                return true;
            }
        }
        return Input.GetMouseButton(0) && GuiSpaceRect.Contains(TouchLocationToGuiLocation(Input.mousePosition));
    }

    [RPC]
    void UpdatePlayerStat(NetworkPlayer nPlayer, string name, int score) {
        if(!Players.ContainsKey(nPlayer)){
            Players[nPlayer] = new PlayerStat(name);
        }

        Players[nPlayer].name = name;
        Players[nPlayer].score = score;
    }

    [RPC]
    void RemovePlayerStat(NetworkPlayer nPlayer)
    {
        Main.Players.Remove(nPlayer);
    }

    [RPC]
    void SetName(NetworkPlayer nPlayer, string name)
    {
        Players[nPlayer].name = name;
        PlayersUpdate = true;
    }

}
