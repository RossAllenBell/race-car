using UnityEngine;
using System.Collections;

public class DumbWeapon : MonoBehaviour {

	public GameObject explosionPrefab;
	public Material deadMaterial;

	public float speed;

	private bool live;
	private bool impacted;
	private NetworkPlayer firer;

	private bool bounced;

	void Start () { 
		if (Network.isServer) {
			bounced = false;
		    impacted = false;
			rigidbody.AddRelativeForce(Vector3.forward * speed * 5);
		}
	}
	
	void FixedUpdate () {
		if(Network.isServer){
			if(impacted || transform.position.magnitude > 200){
				Network.RemoveRPCs(networkView.viewID);
		        Network.Destroy(gameObject);
		        Network.Instantiate(explosionPrefab, transform.position, transform.rotation, 0);
	        } else if(rigidbody.useGravity) {
	        	rigidbody.AddForce(Vector3.down * 10);
	        }
		}
	}
	
	void LateUpdate () {
		if(Network.isServer){
			if(rigidbody.useGravity){
				if(rigidbody.velocity.magnitude > speed + 1) {
					rigidbody.AddForce(-rigidbody.velocity.normalized / 10f);
				} else if(rigidbody.velocity.magnitude < speed) {
					rigidbody.AddForce(rigidbody.velocity.normalized / 10f);
				}
			}
		}
	}

	void OnCollisionEnter(Collision other) {
		if (Network.isServer) {
			GameObject collidingObject = other.gameObject.transform.root.gameObject;
			Car car;
			if(car = collidingObject.GetComponent<Car>()){
				if(firer == car.networkView.owner){
					Main.Players[firer].score -= bounced? 1 : 2;
				} else {
					Main.Players[firer].score += bounced? 1 : 5;
				}
				Main.PlayersUpdate = true;
				impacted = true;
				collidingObject.networkView.RPC("MissileHit", RPCMode.All);
			} else if(collidingObject.GetComponent<DumbWeapon>()){
				impacted = true;
			} else if(!bounced && collidingObject.tag == "Wall") {
				bounced = true;
				GetComponent<MeshRenderer>().material = deadMaterial;
				Destroy(GetComponent<TrailRenderer>());
			}
		}
    }

	void OnTriggerExit(Collider other) {
		if (Network.isServer) {
			GameObject collidingObject = other.gameObject.transform.root.gameObject;
			if(collidingObject.networkView && collidingObject.networkView.owner == firer){
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
