using UnityEngine;
using System.Collections;

public class HomingWeapon : DumbWeapon {

	public override void LateUpdate () {
		if(Network.isServer){
			if(rigidbody.useGravity){
				GameObject target = null;
				if(!bounced) {
					Collider[] hitColliders = Physics.OverlapSphere(transform.position, homingRadius, playerLayer);
					foreach(Collider collider in hitColliders){
						if(firer != collider.gameObject.networkView.owner){
							target = collider.gameObject;
							break;
						}
					}
				}

				if(target){
					Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
					Vector3 directionToHitTarget = directionToTarget - rigidbody.velocity.normalized;
					rigidbody.AddForce(directionToHitTarget.normalized * homingMagnitude);
				} else{
					base.LateUpdate();
				}
			}
		}
	}

}
