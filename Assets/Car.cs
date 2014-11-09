using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {

	public GameObject missilePrefab;

	public WheelCollider fl;
	public WheelCollider fr;
	public WheelCollider bl;
	public WheelCollider br;

	public float MaxTurn;
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
				hasMissile = false;
				Vector3 pos = transform.position;
				Quaternion rot = Quaternion.Euler(transform.rotation.eulerAngles.x + 90f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
				Network.Instantiate(missilePrefab, pos, rot, 0);
			}
		}
	}

	private bool keyboardDetected = false;
	void FixedUpdate () {
		if (networkView.isMine) {
			speed = rigidbody.velocity.magnitude;
	 
	 		if(Main.TouchingIn(UI.LeftRect)) {
	 			steer = -1;
	 		} else if(Main.TouchingIn(UI.RightRect)) {
	 			steer = 1;
	 		} else {
				steer = Mathf.Clamp(Input.GetAxis("Horizontal"), -1, 1);
	 		}

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
					rigidbody.AddForce(new Vector3(0.1f, 1f, 0.1f) * 400);
					rigidbody.AddRelativeTorque(Vector3.forward * 25);
				}
			}
		}
	}

	public void GetMissile() {
		hasMissile = true;
	}

}
