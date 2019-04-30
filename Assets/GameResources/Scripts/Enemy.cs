using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Enemy : Photon.MonoBehaviour {
	[SerializeField] private GameObject target;
	private NavMeshAgent agent;
	private Health health;
	private Animator animator;
	private Collider collider;
	private Health targetHealth;
	[HideInInspector] public bool isAttacking = false;
	[HideInInspector] public bool isDead = false;
	public float speed = 1.0f;
	public float angularSpeed = 120;
	public float damage = 20;
	public float attackAngle = 45;
	[HideInInspector] public UnityEvent onDead;
	private Vector3 syncPos = Vector3.zero;
	private Quaternion syncRot = Quaternion.identity;

	void Awake() {
		syncPos = transform.position;
		syncRot = transform.rotation;
	}

	void Start() {
		agent = GetComponent<NavMeshAgent>();
		health = GetComponent<Health>();
		animator = GetComponent<Animator>();
		collider = GetComponent<Collider>();

		if(PhotonNetwork.isMasterClient) {
			Retargeting();
		}
		else {
			agent.enabled = false;
		}
	}

	void Update() {
		if(!PhotonNetwork.isMasterClient) {
			SyncTransform();
			return;
		}

		CheckHealth();

		Chase();
		CheckAttack();
	}

	float GetDistanceFromTarget(GameObject player) {
		return Vector3.Distance(player.transform.position, transform.position);
	}

	GameObject GetClosestTarget() {
		Player[] players = GameManager.instance.players;

		for(int i = 0; i < players.Length; i++) {
			if(players[i] == null) {
				GameManager.instance.RefreshCurrentPlayers();
				return null;
			}
		}

		if(players.Length == 0) return null;

		GameObject closestTarget = players[0].gameObject;
		float minDist = 99999999;

		for(int i = 0; i < players.Length; i++) {
			float dist = GetDistanceFromTarget(players[i].gameObject);
			Health playerHealth = players[i].GetComponent<Health>();

			if(dist < minDist && playerHealth.value > 0) {
				minDist = dist;
				closestTarget = players[i].gameObject;
			}
		}

		return closestTarget;
	}

	void SyncTransform() {
		transform.position = Vector3.Lerp(transform.position, syncPos, 0.1f);
		transform.rotation = Quaternion.Lerp(transform.rotation, syncRot, 0.1f);
	}

	void CheckHealth() {
		if(isDead) return;

		if(health.value <= 0) {
			onDead.Invoke();

			isDead = true;
			agent.isStopped = true;
			collider.enabled = false;

			animator.CrossFadeInFixedTime("Death", 0.1f);
			BroadcastDead();

			DestroyAfterTime(gameObject, 3);
		}
	}

	void DestroyAfterTime(GameObject obj, float time) {
		StartCoroutine(CoDestroyAfterTime(obj, time));
	}

	IEnumerator CoDestroyAfterTime(GameObject obj, float time) {
		yield return new WaitForSeconds(time);
		PhotonNetwork.Destroy(obj);
	}

	private bool isLateUpdating = false;

	IEnumerator CoLateUpdateDestination(float latency) {
		isLateUpdating = true;

		yield return new WaitForSeconds(latency);

		if(target == null) {
			Retargeting();
		}
		else {
			agent.destination = target.transform.position;
		}

		isLateUpdating = false;
	}

	void Chase() {
		if(isDead) return;

		GameObject closestTarget = GetClosestTarget();

		if(closestTarget == null) return;

		Retargeting();

		float dist = GetDistanceFromTarget(target);

		if(dist >= 20) {
			if(!isLateUpdating) {
				StartCoroutine(CoLateUpdateDestination(0.1f));
			}
		}
		else {
			if(dist <= 40) {
				StartCoroutine(CoLateUpdateDestination(0.5f));
			}
			else if(dist <= 60) {
				StartCoroutine(CoLateUpdateDestination(1.0f));
			}
			else {
				StartCoroutine(CoLateUpdateDestination(2f));
			}
		}
	}

	void CheckAttack() {
		if(isDead) return;
		else if(isAttacking) return;
		else if(target == null) return;
		else if(targetHealth.value <= 0) {
			Retargeting();
			return;
		}

		float distanceFromTarget = Vector3.Distance(target.transform.position, transform.position);
		
		if(distanceFromTarget <= 1.8f) {
			Vector3 directionToTarget = target.transform.position - transform.position;
			float angle = Vector3.Angle(directionToTarget, transform.forward);

			if(angle <= attackAngle) {
				Attack();
			}
		}
	}

	void Retargeting() {
		GameObject closestTarget = GetClosestTarget();

		if(closestTarget == null) return;

		target = closestTarget;
		targetHealth = target.GetComponent<Health>();
	}

	void Attack() {
		targetHealth.TakeDamage(damage);

		agent.speed = 0;
		agent.angularSpeed = 0;
		isAttacking = true;
		animator.SetTrigger("ShouldAttack");
		BroadcastAttackAnimation();

		Invoke("ResetAttacking", 1.5f);
	}

	void BroadcastAttackAnimation() {
		photonView.RPC("RPCBroadcastAttackAnimation", PhotonTargets.Others);
	}

	[PunRPC]
	void RPCBroadcastAttackAnimation() {
		animator.SetTrigger("ShouldAttack");
	}

	void BroadcastDead() {
		photonView.RPC("RPCBroadcastDead", PhotonTargets.Others);
	}

	[PunRPC]
	void RPCBroadcastDead() {
		animator.CrossFadeInFixedTime("Death", 0.1f);
		isDead = true;
		collider.enabled = false;
	}

	void ResetAttacking() {
		isAttacking = false;
		agent.speed = speed;
		agent.angularSpeed = angularSpeed;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if(stream.isWriting) {
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		}
		else {
			syncPos = (Vector3) stream.ReceiveNext();
			syncRot = (Quaternion) stream.ReceiveNext();
		}
	}
}
