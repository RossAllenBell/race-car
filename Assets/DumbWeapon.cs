using UnityEngine;
using System.Collections;

public class DumbWeapon : MonoBehaviour {

	public GameObject explosionPrefab;
	public Material deadMaterial;
	public LayerMask playerLayer;
    public float speed;
    public Texture2D weaponSprite;
    public string weaponName;

	public float homingRadius;
	public float homingMagnitude;

	protected NetworkPlayer firer;
	protected bool bounced;

	private bool live;
	private bool impacted;

	public virtual void Start () {
		if (Network.isServer) {
			bounced = false;
		    impacted = false;
			rigidbody.AddRelativeForce(Vector3.forward * speed * 5);
		}
	}
	
	public virtual void FixedUpdate () {
		if(Network.isServer){
			if(impacted || transform.position.magnitude > 200){
                Network.RemoveRPCs(networkView.viewID);// only used to remove RPCs from a player as a whole?
                Network.Destroy(gameObject);// will beat RPCs and cause an error
                // networkView.RPC("Destroy", RPCMode.All);
		        Network.Instantiate(explosionPrefab, transform.position, transform.rotation, 0);
	        } else if(rigidbody.useGravity) {
	        	rigidbody.AddForce(Vector3.down * 10);
	        }
		}
	}
	
	public virtual void LateUpdate () {
		if(Network.isServer){
			if(rigidbody.useGravity){
				if(rigidbody.velocity.magnitude > speed + 1) {
					rigidbody.AddForce(-rigidbody.velocity.normalized);
				} else if(rigidbody.velocity.magnitude < speed) {
					rigidbody.AddForce(rigidbody.velocity.normalized);
				}
			}
		}
	}

	public virtual void OnCollisionEnter(Collision other) {
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
			} else if(collidingObject.tag == "Weapon"){
				impacted = true;
			} else if(!bounced && collidingObject.tag == "Wall") {
				bounced = true;
				networkView.RPC("SetDead", RPCMode.All);
			}
		}
    }

	public virtual void OnTriggerExit(Collider other) {
		if (Network.isServer) {
			GameObject collidingObject = other.gameObject.transform.root.gameObject;
			if(collidingObject.networkView && collidingObject.networkView.owner == firer){
				GetComponent<Collider>().isTrigger = false;
				rigidbody.useGravity = true;
			}
		}
    }

    [RPC]
	public virtual void SetDead() {
		GetComponent<MeshRenderer>().material = deadMaterial;
		Destroy(GetComponent<TrailRenderer>());
	}

    [RPC]
    public virtual void SetFirer(NetworkPlayer firer)
    {
        this.firer = firer;
    }

    // [RPC]
    // public virtual void Destroy()
    // {
    //     Destroy(gameObject);
    // }
}
