using UnityEngine;
using System.Collections;

public class GoodyBox : MonoBehaviour {

	void Start () {

	}
	
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if(other.transform.root.gameObject == Main.Me) {
			Car car = other.transform.root.GetComponent<Car>();
			if (!car.HasMissile) {
				car.GetMissile();
				Network.RemoveRPCs(networkView.viewID);
		        Network.Destroy(gameObject);
		    }
	    }
    }
}
