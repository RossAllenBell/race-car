using UnityEngine;
using System.Collections;

public class WeaponManager {

	public static GameObject LoadWeapon(string weaponName){
		return Resources.Load(weaponName) as GameObject;
	}

	public static GameObject GetItem(){
		if(Random.value < 0.5f){
			return LoadWeapon("green_weapon");
		} else {
			return LoadWeapon("red_weapon");
		}
	}

}
