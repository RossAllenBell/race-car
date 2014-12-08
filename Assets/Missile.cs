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
		if(Network.isServer && (impacted || transform.position.magnitude > 200)){
			Network.RemoveRPCs(networkView.viewID);
	        Network.Destroy(gameObject);
		}
	}

	void OnCollisionEnter(Collision other) {
		if (Network.isServer) {
			GameObject collidingObject = other.gameObject.transform.root.gameObject;
			if(collidingObject.GetComponent<Car>()){
				impacted = true;
				collidingObject.networkView.RPC("MissileHit", RPCMode.All);
			}
			if(collidingObject.GetComponent<Missile>()){
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
