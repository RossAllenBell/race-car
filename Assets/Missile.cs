using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour {

	public float speed;

	private bool impacted;
	private NetworkPlayer firer;

	void Start () {
		if (Network.isServer) {
		    impacted = false;
		    rigidbody.AddRelativeForce(Vector3.up * speed);
		}
	}
	
	void FixedUpdate () {
		if(Network.isServer && (impacted || transform.position.magnitude > 200)){
			Network.RemoveRPCs(networkView.viewID);
	        Network.Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (Network.isServer) {
			GameObject collidingObject = other.gameObject;
			if(collidingObject.networkView == null || collidingObject.networkView.owner != firer){
				impacted = true;
				if(collidingObject.GetComponent<Car>()){
					collidingObject.networkView.RPC("MissileHit", RPCMode.All);
				}
			}
		}
    }

    [RPC]
	void SetFirer(NetworkPlayer firer) {
		this.firer = firer;
	}
}
