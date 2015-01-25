using UnityEngine;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{

    public static WeaponManager theInstance;

    public Dictionary<string, GameObject> WeaponPrefabs;
    public Dictionary<string, Texture2D> WeaponSprites;

    public string[] WeaponNames = new string[] { "green", "red" };

    public void Start()
    {
        theInstance = this;

        WeaponPrefabs = new Dictionary<string, GameObject>();
        WeaponSprites = new Dictionary<string, Texture2D>();
        foreach(string weaponName in WeaponNames){
            WeaponPrefabs[weaponName] = Resources.Load(weaponName + "_weapon") as GameObject;
            WeaponSprites[weaponName] = Resources.Load(weaponName + "_weapon_sprite") as Texture2D;
        }
    }

	public GameObject GetItem(){
		if(Random.value < 0.9f){
            return WeaponPrefabs["green"];
		} else {
            return WeaponPrefabs["red"];
		}
	}

}
