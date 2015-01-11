using UnityEngine;
using System.Collections;

public class CarFollow : MonoBehaviour {

	public float PositionChaseLerp;
	public float RotationChaseSlerp;

	public static GameObject player;

	private Vector3 followTranslate = (Vector3.back * 6) + (Vector3.up * 2);

	void Start () {

	}
	
	void FixedUpdate () {
		if (player)
		{
			Quaternion newRotation = Quaternion.Slerp(transform.rotation, player.transform.rotation, RotationChaseSlerp);
			transform.rotation = Quaternion.Euler(0, newRotation.eulerAngles.y, 0);

			transform.position = Vector3.Slerp(transform.position, player.transform.position, PositionChaseLerp);
			transform.Translate(followTranslate);
		}
	}

	public static void SetPlayer (GameObject aPlayer)
	{
		player = aPlayer;
	}
}
