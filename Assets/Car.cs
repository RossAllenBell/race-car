using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {

	public float Acceleration;
	public float TurnSpeed;

	void Start () {

	}
	
	void Update () {
	
	}

	void FixedUpdate () {
		if(IsGrounded()){
			if(Input.GetKey(KeyCode.UpArrow) || Main.TouchingIn(UI.ARect)) {
				rigidbody.AddRelativeForce (Vector3.forward * Acceleration);
			} else if(Input.GetKey(KeyCode.DownArrow)) {
				rigidbody.AddRelativeForce (Vector3.back * Acceleration);
			}

			if(Input.GetKey(KeyCode.LeftArrow) || Main.TouchingIn(UI.LeftRect)) {
				rigidbody.AddTorque(Vector3.down * TurnSpeed);
			} else if(Input.GetKey(KeyCode.RightArrow) || Main.TouchingIn(UI.RightRect)) {
				rigidbody.AddTorque(Vector3.up * TurnSpeed);
			} else {
				// var backToCenter = Vector3.Lerp(rigidbody.angularVelocity, Vector3.zero, 0.5f);
				// rigidbody.AddTorque(backToCenter);
			}
		}
	}

	bool IsGrounded () {
	    return Physics.Raycast(transform.position, Vector3.down, collider.bounds.extents.y + 0.1f);
	}
}
