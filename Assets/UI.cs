using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {

	private Texture2D Left;
    private Texture2D Right;
    private Texture2D Backwards;
    private Texture2D Trigger;
    private Texture2D TriggerDisabled;

    public static  Rect RoomNameRect;
    public static  GUIStyle RoomNameStyle;

    public static  Rect GameStatsRect;
    public static  GUIStyle GameStatsStyle;

    public static  Rect CarStatsRect;
    public static  GUIStyle CarStatsStyle;

	public static Rect LeftRect;
	public static Rect RightRect;
	public static Rect ARect;
	public static Rect BRect;
	private static int lastKnownScreenWidth = -1;

	public int NormalButtonWidth = 200;
	public int NormalButtonpadding = 40;

	void Start () {
		Left = Resources.Load("left") as Texture2D;
	    Right = Resources.Load("right") as Texture2D;
	    Backwards = Resources.Load("backwards") as Texture2D;
	    Trigger = Resources.Load("trigger") as Texture2D;
	    TriggerDisabled = Resources.Load("disabled") as Texture2D;
	}

	void Update () {
		if (lastKnownScreenWidth != Screen.width) {
			float guiRatio = Screen.width / Main.NormalWidth;
			LeftRect = new Rect(NormalButtonpadding * guiRatio, Screen.height - ((NormalButtonWidth + NormalButtonpadding) * guiRatio), NormalButtonWidth * guiRatio, NormalButtonWidth * guiRatio);
			//int dpiPadding = (int) (Screen.dpi > 0 ? 0.3 * Screen.dpi : NormalButtonWidth * guiRatio);
			RightRect = new Rect((NormalButtonpadding * guiRatio) + (NormalButtonWidth * guiRatio * 1.5f), Screen.height - ((NormalButtonWidth + NormalButtonpadding) * guiRatio), NormalButtonWidth * guiRatio, NormalButtonWidth * guiRatio);
			ARect = new Rect((Screen.width - ((NormalButtonWidth + NormalButtonpadding) * guiRatio)),  Screen.height - ((NormalButtonWidth + NormalButtonpadding) * guiRatio * 2), NormalButtonWidth * guiRatio, NormalButtonWidth * guiRatio);
			BRect = new Rect((Screen.width - ((NormalButtonWidth + NormalButtonpadding) * guiRatio)),  Screen.height - ((NormalButtonWidth + NormalButtonpadding) * guiRatio), NormalButtonWidth * guiRatio, NormalButtonWidth * guiRatio);

		    RoomNameStyle = new GUIStyle();
		    RoomNameStyle.fontSize = Main.FontSmall;
		    RoomNameStyle.normal.textColor = Color.red;
		    RoomNameStyle.alignment = TextAnchor.UpperLeft;
		    RoomNameRect = new Rect((Screen.width / 500), (Screen.width / 500), Screen.width, RoomNameStyle.CalcSize(new GUIContent("A")).y);

		    GameStatsStyle = new GUIStyle();
		    GameStatsStyle.fontSize = Main.FontSmall;
		    GameStatsStyle.normal.textColor = Color.red;
		    GameStatsStyle.alignment = TextAnchor.UpperLeft;
		    GameStatsRect = new Rect((Screen.width / 500), (Screen.width / 500) + ( GameStatsStyle.CalcSize(new GUIContent("A")).y * 2), Screen.width, Screen.height);

			CarStatsStyle = new GUIStyle();
		    CarStatsStyle.fontSize = Main.FontSmallest;
		    CarStatsStyle.normal.textColor = Color.red;
		    CarStatsStyle.alignment = TextAnchor.UpperRight;
		    CarStatsRect = new Rect(0, (Screen.width / 500), Screen.width - (Screen.width / 500), Screen.height);

			lastKnownScreenWidth = Screen.width;
		}
		// LeftRect = new Rect(20, Screen.height - 120, 100, 100);
		// RightRect = new Rect(140, Screen.height - 120, 100, 100);
		// ARect = new Rect(Screen.width - 120, Screen.height - 120, 100, 100);
	}

	void OnGUI() {
        if (NetworkManager.CurrentRoomName != null) {
            GUI.Label(RoomNameRect, NetworkManager.CurrentRoomName, RoomNameStyle);
        }

        string gameStats = "";
        foreach(PlayerStat stat in Main.Players.Values){
        	gameStats = gameStats + stat.name + " " + stat.score + "\n";
        }

        if (GameStatsRect != null)
        {
            GUI.Label(GameStatsRect, gameStats, GameStatsStyle);
        }

        if (Main.theInstance.Me) {
            GUI.Label(CarStatsRect, Main.theInstance.Me.rigidbody.velocity.magnitude.ToString("F1") + "\n" + Main.theInstance.Me.transform.rotation.eulerAngles, CarStatsStyle);

			GUI.DrawTexture(LeftRect, Left);
			GUI.DrawTexture(RightRect, Right);
			GUI.DrawTexture(BRect, Backwards);

            Car me = Main.theInstance.Me;
			if (me.Item)
            {
                GUI.DrawTexture(ARect, me.Item.GetComponent<DumbWeapon>().weaponSprite);
            } else if (me.RollingItem) {
                string[] names = WeaponManager.theInstance.WeaponNames;
                float timeSinceRolling = Time.time - me.StartedRolling;
                string name = names[(int)(names.Length * (timeSinceRolling % 0.25f) * 4)];
                GUI.DrawTexture(ARect, WeaponManager.theInstance.WeaponSprites[name]);
                GUI.DrawTexture(ARect, TriggerDisabled);
            } else {
				GUI.DrawTexture(ARect, TriggerDisabled);
			}
        }
	}
}
