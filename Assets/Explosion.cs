using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	private float startTime;

	void Start () {
		startTime = Time.time;
	}
	
	void Update () {
		if(Time.time - startTime > 2f){
			GameObject.Destroy(gameObject);
		}
	}
}
