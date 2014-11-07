﻿using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {

	GUIStyle style;

    public static  Rect RoomNameRect;
    public static  GUIStyle RoomNameStyle;

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
		style = new GUIStyle();
		var texture = new Texture2D(1,1);
	    texture.SetPixel(0,0,new Color(1f,0f,0f,0.5f));
	    texture.Apply();
		style.normal.background = texture;
	}

	void Update () {
		if (lastKnownScreenWidth != Screen.width) {
			float guiRatio = Screen.width / Main.NormalWidth;
			LeftRect = new Rect(NormalButtonpadding * guiRatio, Screen.height - ((NormalButtonWidth + NormalButtonpadding) * guiRatio), NormalButtonWidth * guiRatio, NormalButtonWidth * guiRatio);
			RightRect = new Rect((NormalButtonpadding * guiRatio * 2) + (NormalButtonWidth * guiRatio * 2), Screen.height - ((NormalButtonWidth + NormalButtonpadding) * guiRatio), NormalButtonWidth * guiRatio, NormalButtonWidth * guiRatio);
			ARect = new Rect((Screen.width - ((NormalButtonWidth + NormalButtonpadding) * guiRatio)),  Screen.height - ((NormalButtonWidth + NormalButtonpadding) * guiRatio * 2), NormalButtonWidth * guiRatio, NormalButtonWidth * guiRatio);
			BRect = new Rect((Screen.width - ((NormalButtonWidth + NormalButtonpadding) * guiRatio)),  Screen.height - ((NormalButtonWidth + NormalButtonpadding) * guiRatio), NormalButtonWidth * guiRatio, NormalButtonWidth * guiRatio);

		    RoomNameStyle = new GUIStyle();
		    RoomNameStyle.fontSize = Main.FontSmallest;
		    RoomNameStyle.normal.textColor = Color.red;
		    RoomNameStyle.alignment = TextAnchor.UpperLeft;
		    RoomNameRect = new Rect((Screen.width / 500), (Screen.width / 500), Screen.width, RoomNameStyle.CalcSize(new GUIContent("A")).y);

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
		GUI.Box(LeftRect, GUIContent.none, style);
		GUI.Box(RightRect, GUIContent.none, style);
		GUI.Box(ARect, GUIContent.none, style);
		GUI.Box(BRect, GUIContent.none, style);

        if (NetworkManager.CurrentRoomName != null) {
            GUI.Label(RoomNameRect, NetworkManager.CurrentRoomName, RoomNameStyle);
        }

        if (Main.Me) {
        	GUI.Label(CarStatsRect, Main.Me.rigidbody.velocity.magnitude.ToString("F1") + "\n" + Main.Me.transform.rotation.eulerAngles, CarStatsStyle);
        }
	}
}
