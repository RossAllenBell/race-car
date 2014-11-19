using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour {

	bool impacted = false;

	void Start () {
	
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
			if(collidingObject.networkView && collidingObject.networkView.owner != networkView.owner){
				collidingObject.networkView.RPC("MissileHit", RPCMode.All);
				impacted = true;
			}
		}
    }
}
