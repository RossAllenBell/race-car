using UnityEngine;
using System.Collections;

public class GoodyBox : MonoBehaviour {

	void Start () {
		Ray ray = new Ray(transform.position, -transform.up);
		RaycastHit hit;
		Physics.Raycast(ray, out hit);
		transform.position += (-transform.up * (hit.distance - collider.bounds.extents.x));
	}
	
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (Network.isServer) {
			Car car = other.transform.root.GetComponent<Car>();
			if (car && (!car.Item && !car.RollingItem)) {
				car.networkView.RPC("GetItem", RPCMode.All);;
				Network.RemoveRPCs(networkView.viewID);
		        Network.Destroy(gameObject);
		    }
	    }
    }
}
