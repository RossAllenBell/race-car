using UnityEngine;
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

	public GameObject Item = null;
    public bool RollingItem = false;
    public float StartedRolling;

    public Color color;

	void Start () {
		rigidbody.centerOfMass = new Vector3(0, -0.5f, 0);
	}
	
	void Update () {
		if (networkView.isMine) {
			if (Item && (Main.TouchingIn(UI.ARect) || Input.GetKeyDown("space"))){
                string weaponName = Item.GetComponent<DumbWeapon>().weaponName;
				networkView.RPC("FireMissile", RPCMode.All, weaponName);
			} else if (RollingItem && Time.time - StartedRolling > 2)
            {
                CompleteItemRoll();
            }
		}
	}

	private bool keyboardDetected = false;
	void FixedUpdate () {
		if (networkView.isMine) {
			speed = rigidbody.velocity.magnitude;
	 
	 		if(Main.TouchingIn(UI.LeftRect) || Input.GetKey("left")) {
	 			steer -= TurnIncreaseRate * Time.fixedDeltaTime;
	 			if(steer > 0) steer -= TurnIncreaseRate * Time.fixedDeltaTime;
	 		} else if(Main.TouchingIn(UI.RightRect) || Input.GetKey("right")) {
	 			steer += TurnIncreaseRate * Time.fixedDeltaTime;
	 			if(steer < 0) steer += TurnIncreaseRate * Time.fixedDeltaTime;
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
			 
			fl.motorTorque = MaxTorque * motor;
			fr.motorTorque = MaxTorque * motor;
			fl.brakeTorque = MaxBreak * brake;
			fr.brakeTorque = MaxBreak * brake;
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

	private Vector3 RandomJoltVector(){
		float drift = 0.3f;
    	Vector2 xz = new Vector2((Random.value * drift * 2f) - (drift / 2f), (Random.value * drift * 2f) - (drift / 2f));
        return new Vector3(xz.x, 1f, xz.y);
	}

	private void UnstickJolt(){
		rigidbody.AddForce(RandomJoltVector() * 400);
		rigidbody.AddRelativeTorque(Vector3.forward * 25);
	}

	[RPC]
    void MissileHit() {
    	rigidbody.AddForce(RandomJoltVector() * 400);
		rigidbody.AddRelativeTorque(Vector3.forward * 30f);
    }

    [RPC]
    void GetItem()
    {
        RollingItem = true;
        StartedRolling = Time.time;
    }

    void CompleteItemRoll()
    {
        RollingItem = false;
        Item = WeaponManager.theInstance.GetItem();
    }

    [RPC]
	void FireMissile(string weaponName) {
		if(Network.isServer){
            GameObject missile = (GameObject)Network.Instantiate(WeaponManager.theInstance.WeaponPrefabs[weaponName], transform.position, transform.rotation, 0);
			missile.networkView.RPC("SetFirer", RPCMode.All, gameObject.networkView.owner);
		}
        Item = null;
	}

    [RPC]
    void SetColor(float r, float g, float b)
    {
        this.color = new Color(r, g, b);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = this.color;
        }
    }

}
