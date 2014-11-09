using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour {

	bool impacted = false;

	void Start () {
	
	}
	
	void FixedUpdate () {
		transform.Translate(Vector3.up * 50 * Time.fixedDeltaTime);

		if(networkView.isMine && (impacted || transform.position.magnitude > 200)){
			Network.RemoveRPCs(networkView.viewID);
	        Network.Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other) {
		GameObject collidingObject = other.transform.root.gameObject;
		if(networkView.isMine && collidingObject != Main.Me){
			impacted = true;
		} else if(!impacted && !networkView.isMine && collidingObject == Main.Me) {
			impacted = true;
			collidingObject.rigidbody.AddForce(new Vector3(0.1f, 1f, 0.1f) * 400);
			collidingObject.rigidbody.AddRelativeTorque(Vector3.forward * 25);
		}
    }
}
