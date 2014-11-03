using UnityEngine;
using System.Collections;

public class CarFollow : MonoBehaviour {

	public float PositionChaseLerp;
	public float RotationChaseSlerp;

	public static GameObject player;

	void Start () {
		//player = GameObject.FindWithTag ("PlayerCameraChase");
	}
	
	void FixedUpdate () {
		if (player)
		{
			transform.position = Vector3.Lerp(transform.position, player.transform.position, PositionChaseLerp);
			transform.rotation = Quaternion.Slerp(transform.rotation, player.transform.rotation, RotationChaseSlerp);
		}
	}

	public static void SetPlayer (GameObject aPlayer)
	{
		player = GameObject.FindWithTag ("PlayerCameraChase");
	}
}
