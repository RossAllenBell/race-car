using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour {

	private GameObject firer = null;
	public GameObject Firer {
		get {return firer;}
		set {this.firer = value;}
	}

	private bool impacted = false;

	void Start () {
	
	}
	
	void FixedUpdate () {
		transform.Translate(Vector3.up * 50 * Time.fixedDeltaTime);

		if(Network.isServer && transform.position.magnitude > 200){
			Network.RemoveRPCs(networkView.viewID);
	        Network.Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other) {
		if(impacted) return;

		Debug.Log(other.transform.root.gameObject);
		if (firer == null && other.transform.root.gameObject == Main.Me) {
			impacted = true;
			other.transform.root.rigidbody.AddForce(new Vector3(0.1f, 1f, 0.1f) * 400);
			other.transform.root.rigidbody.AddRelativeTorque(Vector3.forward * 25);
			Network.RemoveRPCs(networkView.viewID);
	        Network.Destroy(gameObject);
	    } else if(Network.isServer && other.transform.root.gameObject.tag != "Player") {
	  //   	Debug.Log(3);
			// Network.RemoveRPCs(networkView.viewID);
	  //       Network.Destroy(gameObject);
	    }
    }
}
