﻿using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {

	public GameObject missilePrefab;

	public WheelCollider fl;
	public WheelCollider fr;
	public WheelCollider bl;
	public WheelCollider br;

	public float MaxTurn;
	public float TurnIncreaseRate;
	public float MaxTorque;
	public float MaxBreak;
	public float MaxSpeed;
	 
	private float steer = 0;
	private float motor = 0;
	private float brake = 0;
	private float forward = 0;
	private float back = 0;
	private float speed = 0;
	private bool reverse = false;

	private float forwardDown = 0;

	private bool hasMissile = false;
	public bool HasMissile { get { return hasMissile; } }

	void Start () {
		rigidbody.centerOfMass = new Vector3(0, -0.5f, 0);
	}
	
	void Update () {
		if (networkView.isMine) {
			if (hasMissile && (Main.TouchingIn(UI.ARect) || Input.GetKeyDown("space"))){
				networkView.RPC("FireMissile", RPCMode.All);
			}
		}
	}

	private bool keyboardDetected = false;
	void FixedUpdate () {
		if (networkView.isMine) {
			speed = rigidbody.velocity.magnitude;
	 
	 		if(Main.TouchingIn(UI.LeftRect) || Input.GetKey("left")) {
	 			steer -= TurnIncreaseRate * Time.fixedDeltaTime;
	 		} else if(Main.TouchingIn(UI.RightRect) || Input.GetKey("right")) {
	 			steer += TurnIncreaseRate * Time.fixedDeltaTime;
	 		} else if(steer != 0) {
	 			if(steer > 0){
	 				steer = Mathf.Max(0f, steer - (TurnIncreaseRate * Time.fixedDeltaTime * 2f));
 				} else {
 					steer = Mathf.Min(0f, steer + (TurnIncreaseRate * Time.fixedDeltaTime * 2f));
 				}
	 		}
	 		steer = Mathf.Clamp(steer, -1, 1);

	 		keyboardDetected = keyboardDetected || Input.GetAxis("Vertical") != 0;
			back = Main.TouchingIn(UI.BRect) || Input.GetAxis("Vertical") < 0 ? 1 : 0;
			if (keyboardDetected) {
				forward = Input.GetAxis("Vertical") > 0 ? 1 : 0;
			} else {
				forward = 1 - back;
			}

			if(Mathf.Abs(speed) < 0.01) {
		  	    if(back > 0) { reverse = true; }
		        if(forward > 0) { reverse = false; }
			}
			 
			if(reverse) {
			  motor = -1 * back;
			  brake = forward;
			} else {
			  motor = forward;
			  brake = back;
			}

			if(Mathf.Abs(speed) >= MaxSpeed){
				motor = 0;
			}
			 
			bl.motorTorque = MaxTorque * motor;
			br.motorTorque = MaxTorque * motor;
			bl.brakeTorque = MaxBreak * brake;
			br.brakeTorque = MaxBreak * brake;
			 
			fl.steerAngle = MaxTurn * steer;
			fr.steerAngle = MaxTurn * steer;

			if (forward == 0 || rigidbody.velocity.magnitude > 1f) {
				forwardDown = 0;
			} else {
				forwardDown += Time.fixedDeltaTime;
				if (forwardDown > 3 && rigidbody.velocity.magnitude < 1f) {
					UnstickJolt();
				}
			}
		}
	}

	private void UnstickJolt(){
		rigidbody.AddForce(new Vector3(0.1f, 1f, 0.1f) * 400);
		rigidbody.AddRelativeTorque(Vector3.forward * 25);
	}

	[RPC]
    void MissileHit() {
    	float drift = 0.3f;
    	Vector2 xz = new Vector2((Random.value * drift * 2f) - (drift / 2f), (Random.value * drift * 2f) - (drift / 2f));
        rigidbody.AddForce(new Vector3(xz.x, 1f, xz.y) * 400);
		rigidbody.AddRelativeTorque(Vector3.forward * 30f);
    }

    [RPC]
	void GetMissile() {
		hasMissile = true;
	}

    [RPC]
	void FireMissile() {
		hasMissile = false;
		if(Network.isServer){
			Vector3 pos = transform.position;
			Quaternion rot = Quaternion.Euler(transform.rotation.eulerAngles.x + 90f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
			GameObject missile = (GameObject) Network.Instantiate(missilePrefab, pos, rot, 0);
			missile.networkView.RPC("SetFirer", RPCMode.All, gameObject.networkView.owner);
		}
	}

}
