using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour {

	public float speed;

	private bool live;
	private bool impacted;
	private NetworkPlayer firer;

	void Start () {
		if (Network.isServer) {
		    impacted = false;
		    rigidbody.AddRelativeForce(Vector3.forward * speed);
		}
	}
	
	void FixedUpdate () {
		if(Network.isServer){
			if(impacted || transform.position.magnitude > 200){
				Network.RemoveRPCs(networkView.viewID);
		        Network.Destroy(gameObject);
	        } else {
	        	rigidbody.AddForce(Vector3.down * 10);
	        }
		}
	}

	void OnCollisionEnter(Collision other) {
		if (Network.isServer) {
			GameObject collidingObject = other.gameObject.transform.root.gameObject;
			Car car;
			if(car = collidingObject.GetComponent<Car>()){
				if(firer == car.networkView.owner){
					Main.Players[firer].score -= 2;
				} else {
					Main.Players[firer].score += 5;
				}
				Main.PlayersUpdate = true;
				impacted = true;
				collidingObject.networkView.RPC("MissileHit", RPCMode.All);
			} else if(collidingObject.GetComponent<Missile>()){
				impacted = true;
			}
		}
    }

	void OnTriggerExit(Collider other) {
		if (Network.isServer) {
			GameObject collidingObject = other.gameObject.transform.root.gameObject;
			if(collidingObject.networkView.owner == firer){
				GetComponent<Collider>().isTrigger = false;
				rigidbody.useGravity = true;
			}
		}
    }

    [RPC]
	void SetFirer(NetworkPlayer firer) {
		this.firer = firer;
	}
}
