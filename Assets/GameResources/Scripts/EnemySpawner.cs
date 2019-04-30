using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Photon.MonoBehaviour {
	// public GameObject zombiePrefab;
	public string zombiePrefabName;
	
	public GameObject SpawnAt(Vector3 pos, Quaternion rot) {
		// GameObject enemy = Instantiate(zombiePrefab, pos, rot);
		GameObject enemy = PhotonNetwork.Instantiate(zombiePrefabName, pos, rot, 0);
		return enemy;
	}
}
