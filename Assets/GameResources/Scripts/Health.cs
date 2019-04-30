using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : Photon.MonoBehaviour {
	public Health parentRef;
	public float value = 100f;
	public float damageMultipler = 1.0f;
	[HideInInspector] public UnityEvent onHit;

	public void TakeDamage(float damage) {
		damage *= damageMultipler;

		if(parentRef != null) {
			parentRef.TakeDamage(damage);
			return;
		}

		value -= damage;

		photonView.RPC("RPCSyncHealth", PhotonTargets.Others, value);

		onHit.Invoke();

		if(value < 0) {
			value = 0;
		}
	}

	[PunRPC]
	void RPCSyncHealth(float newValue) {
		value = newValue;
		onHit.Invoke();
	}
}
