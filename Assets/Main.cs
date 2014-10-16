﻿using UnityEngine;
using System.Collections.Generic;

public class Main : MonoBehaviour {

	public const float NormalWidth = 1920;
    public const float NormalHeight = 1080;

	public static bool Clicked { get { return click; } }
	public static Vector2 TouchLocation { get { return touchLocation; } }
	public static Vector2 TouchGuiLocation { get { return TouchLocationToGuiLocation(touchLocation); } }
    public static bool Touching { get { return touching; } }

    private static bool click;
    private static Vector2 touchLocation;
    private static bool touching;

	public void Start () {
		Screen.orientation = ScreenOrientation.Landscape;
	}
	
	public void Update () {

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

}
