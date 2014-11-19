using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour {

	private bool impacted;
	private NetworkPlayer firer;

	void Start () {
	    impacted = false;
	}
	
	void FixedUpdate () {
		transform.Translate(Vector3.up * 50 * Time.fixedDeltaTime);

		if(Network.isServer && (impacted || transform.position.magnitude > 200)){
			Network.RemoveRPCs(networkView.viewID);
	        Network.Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (Network.isServer) {
			GameObject collidingObject = other.gameObject;
			if(collidingObject.networkView && collidingObject.networkView.owner != firer){
				collidingObject.networkView.RPC("MissileHit", RPCMode.All);
				impacted = true;
			}
		}
    }

    [RPC]
	void SetFirer(NetworkPlayer firer) {
		this.firer = firer;
	}
}
