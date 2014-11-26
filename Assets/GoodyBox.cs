using UnityEngine;
using System.Collections;

public class GoodyBox : MonoBehaviour {

	void Start () {

	}
	
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (Network.isServer) {
			Car car = other.transform.root.GetComponent<Car>();
			if (car && !car.HasMissile) {
				car.networkView.RPC("GetMissile", RPCMode.All);;
				Network.RemoveRPCs(networkView.viewID);
		        Network.Destroy(gameObject);
		    }
	    }
    }
}
