using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : Photon.MonoBehaviour {
	public static Player instance;
	public Animator characterAnimator;
	private Health health;
	private AudioSource audioSource;
	[HideInInspector] public bool isDead = false;
	private Vector3 syncPos = Vector3.zero;
	private Quaternion syncRot = Quaternion.identity;
	[HideInInspector] public FirstPersonController firstPersonController;
	public Transform characterWeapons;
	public MuzzleflashController muzzleflashController;
	public GameObject inspectorPrefab;

	void Awake() {
		if(photonView.isMine) {
			instance = this;
		}

		syncPos = transform.position;
		syncRot = transform.rotation;
	}

	void Start() {
		audioSource = GetComponents<AudioSource>()[1];
		muzzleflashController = GetComponent<MuzzleflashController>();

		if(!photonView.isMine) {
			Destroy(transform.Find("FirstPersonCharacter").gameObject);
			transform.Find("PlayerCharacter").gameObject.SetActive(true);
			DisableRagdoll();

			MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();

			for(int i = 0; i < scripts.Length; i++) {
				MonoBehaviour script = scripts[i];

				if(script == this) continue;
				else if(script is PhotonView) continue;
				else if(script is Health) continue;

				Destroy(script);
			}
			
			return;
		}
		else {
			transform.Find("PlayerCharacter").gameObject.SetActive(false);
		}

		firstPersonController = GetComponent<FirstPersonController>();
		health = GetComponent<Health>();
	}

	[PunRPC]
	void RPCDeployDeadBody() {
		GameObject playerCharacter = transform.Find("PlayerCharacter").gameObject;
		playerCharacter.SetActive(true);
		characterAnimator.enabled = false;

		GameManager.instance.deadBodies.Add(playerCharacter);

		EnableRagdoll();
		playerCharacter.transform.SetParent(null);

		if(photonView.isMine) {
			PhotonNetwork.Destroy(gameObject);
		}
	}

	void EnableRagdoll() {
		Transform character = transform.Find("PlayerCharacter");
		CharacterJoint[] joints = character.GetComponentsInChildren<CharacterJoint>();
		Rigidbody[] rigidbodies = character.GetComponentsInChildren<Rigidbody>();
		Collider[] colliders = character.GetComponentsInChildren<Collider>();

		foreach(CharacterJoint joint in joints) {
			joint.enablePreprocessing = true;
			joint.enableProjection = true;
		}
		
		foreach(Rigidbody rb in rigidbodies) {
			rb.useGravity = true;
			rb.isKinematic = false;
		}

		foreach(Collider col in colliders) {
			col.enabled = true;
		}
	}

	void DisableRagdoll() {
		Transform character = transform.Find("PlayerCharacter");
		CharacterJoint[] joints = character.GetComponentsInChildren<CharacterJoint>();
		Rigidbody[] rigidbodies = character.GetComponentsInChildren<Rigidbody>();
		Collider[] colliders = character.GetComponentsInChildren<Collider>();

		foreach(CharacterJoint joint in joints) {
			joint.enablePreprocessing = false;
			joint.enableProjection = false;
		}
		
		foreach(Rigidbody rb in rigidbodies) {
			rb.useGravity = false;
			rb.isKinematic = true;
		}

		foreach(Collider col in colliders) {
			col.enabled = false;
		}
	}

	void Update() {
		if(!photonView.isMine) {
			transform.position = Vector3.Lerp(transform.position, syncPos, 0.1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, syncRot, 0.1f);
			return;
		}

		CheckHealth();
		UpdateAnimator();
	}

	public void PlaySoundThroughNetwork(string identifier) {
		photonView.RPC("RPCPlaySoundThroughNetwork", PhotonTargets.Others, identifier);
	}

	[PunRPC]
	void RPCPlaySoundThroughNetwork(string identifier) {
		switch(identifier) {
			case "DryFire":
				audioSource.PlayOneShot(AudioManager.instance.dryFire);
				break;
			case "Police 9mm_Draw":
				audioSource.PlayOneShot(AudioManager.instance.police9mmDraw);
				break;
			case "Police 9mm_Fire":
				audioSource.PlayOneShot(AudioManager.instance.police9mmFire);
				break;
			case "Police 9mm_MagOut":
				audioSource.PlayOneShot(AudioManager.instance.police9mmMagOut);
				break;
			case "Police 9mm_MagIn":
				audioSource.PlayOneShot(AudioManager.instance.police9mmMagIn);
				break;
			case "Police 9mm_BoltForwarded":
				audioSource.PlayOneShot(AudioManager.instance.police9mmBoltForwarded);
				break;
			case "Portable Magnum_Draw":
				audioSource.PlayOneShot(AudioManager.instance.portableMagnumDraw);
				break;
			case "Portable Magnum_Fire":
				audioSource.PlayOneShot(AudioManager.instance.portableMagnumFire);
				break;
			case "Portable Magnum_MagOut":
				audioSource.PlayOneShot(AudioManager.instance.portableMagnumMagOut);
				break;
			case "Portable Magnum_MagIn":
				audioSource.PlayOneShot(AudioManager.instance.portableMagnumMagIn);
				break;
			case "Portable Magnum_BoltForwarded":
				audioSource.PlayOneShot(AudioManager.instance.portableMagnumBoltForwarded);
				break;
			case "Compact 9mm_Draw":
				audioSource.PlayOneShot(AudioManager.instance.compact9mmDraw);
				break;
			case "Compact 9mm_Fire":
				audioSource.PlayOneShot(AudioManager.instance.compact9mmFire);
				break;
			case "Compact 9mm_MagOut":
				audioSource.PlayOneShot(AudioManager.instance.compact9mmMagOut);
				break;
			case "Compact 9mm_MagIn":
				audioSource.PlayOneShot(AudioManager.instance.compact9mmMagIn);
				break;
			case "Compact 9mm_BoltForwarded":
				audioSource.PlayOneShot(AudioManager.instance.compact9mmBoltForwarded);
				break;
			case "UMP45_Draw":
				audioSource.PlayOneShot(AudioManager.instance.ump45Draw);
				break;
			case "UMP45_Fire":
				audioSource.PlayOneShot(AudioManager.instance.ump45Fire);
				break;
			case "UMP45_MagOut":
				audioSource.PlayOneShot(AudioManager.instance.ump45MagOut);
				break;
			case "UMP45_MagIn":
				audioSource.PlayOneShot(AudioManager.instance.ump45MagIn);
				break;
			case "UMP45_BoltForwarded":
				audioSource.PlayOneShot(AudioManager.instance.ump45BoltForwarded);
				break;
			case "Defender Shotgun_Draw":
				audioSource.PlayOneShot(AudioManager.instance.defenderShotgunDraw);
				break;
			case "Defender Shotgun_Fire":
				audioSource.PlayOneShot(AudioManager.instance.defenderShotgunFire);
				break;
			case "Defender Shotgun_MagOut":
				audioSource.PlayOneShot(AudioManager.instance.defenderShotgunMagOut);
				break;
			case "Defender Shotgun_MagIn":
				audioSource.PlayOneShot(AudioManager.instance.defenderShotgunMagIn);
				break;
			case "Defender Shotgun_BoltForwarded":
				audioSource.PlayOneShot(AudioManager.instance.defenderShotgunBoltForwarded);
				break;
			case "Stov Rifle_Draw":
				audioSource.PlayOneShot(AudioManager.instance.stovRifleDraw);
				break;
			case "Stov Rifle_Fire":
				audioSource.PlayOneShot(AudioManager.instance.stovRifleFire);
				break;
			case "Stov Rifle_MagOut":
				audioSource.PlayOneShot(AudioManager.instance.stovRifleMagOut);
				break;
			case "Stov Rifle_MagIn":
				audioSource.PlayOneShot(AudioManager.instance.stovRifleMagIn);
				break;
			case "Stov Rifle_BoltForwarded":
				audioSource.PlayOneShot(AudioManager.instance.stovRifleBoltForwarded);
				break;
		}
	}

	public void PlayMuzzleflashThroughNetwork(string identifier) {
		photonView.RPC("RPCPlayMuzzleflashThroughNetwork", PhotonTargets.Others, identifier);
	}

	[PunRPC]
	void RPCPlayMuzzleflashThroughNetwork(string identifier) {
		muzzleflashController.PlayMuzzleflash(identifier);
	}

	public void SetWeapon(Weapon weapon) {
		photonView.RPC("RPCSetWeapon", PhotonTargets.Others, weapon);
	}

	[PunRPC]
	void RPCSetWeapon(Weapon weapon) {
		characterAnimator.SetTrigger("SwitchWeapon");

		characterAnimator.SetBool("IsPolice9mm", false);
		characterAnimator.SetBool("IsPortableMagnum", false);
		characterAnimator.SetBool("IsCompact9mm", false);
		characterAnimator.SetBool("IsUMP45", false);
		characterAnimator.SetBool("IsDefenderShotgun", false);
		characterAnimator.SetBool("IsStovRifle", false);

		switch(weapon) {
			case Weapon.Police9mm:
				characterAnimator.SetBool("IsPolice9mm", true);
				break;
			case Weapon.PortableMagnum:
				characterAnimator.SetBool("IsPortableMagnum", true);
				break;
			case Weapon.Compact9mm:
				characterAnimator.SetBool("IsCompact9mm", true);
				break;
			case Weapon.UMP45:
				characterAnimator.SetBool("IsUMP45", true);
				break;
			case Weapon.DefenderShotgun:
				characterAnimator.SetBool("IsDefenderShotgun", true);
				break;
			case Weapon.StovRifle:
				characterAnimator.SetBool("IsStovRifle", true);
				break;
		}

		for(int i = 0; i < characterWeapons.childCount; i++) {
			characterWeapons.GetChild(i).gameObject.SetActive(false);
		}

		characterWeapons.Find(weapon.ToString()).gameObject.SetActive(true);
	}

	public void PlayFireAnimation() {
		photonView.RPC("RPCPlayFireAnimation", PhotonTargets.Others);
	}

	[PunRPC]
	void RPCPlayFireAnimation() {
		characterAnimator.SetTrigger("Firing");
	}

	public void PlayReloadAnimation() {
		photonView.RPC("RPCPlayReloadAnimation", PhotonTargets.Others);
	}

	[PunRPC]
	void RPCPlayReloadAnimation() {
		characterAnimator.SetTrigger("Reloading");
	}

	void CheckHealth() {
		if(isDead) return;

		if(health.value <= 0) {
			isDead = true;

			GameManager.instance.NotifyPlayerDead();
			
			// deathAnimator.SetTrigger("Show");
			// GameManager.instance.GameOver();

			// Invoke("RestartGame", 5);
			Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z - 1);
			Instantiate(inspectorPrefab, spawnPos, transform.rotation);
			photonView.RPC("RPCDeployDeadBody", PhotonTargets.All);
		}
	}

	void UpdateAnimator() {
		photonView.RPC("RPCSyncAnimator", PhotonTargets.Others, firstPersonController.controller.velocity.magnitude != 0);
	}

	[PunRPC]
	void RPCSyncAnimator(bool isMoving) {
		characterAnimator.SetBool("IsWalking", isMoving);
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

	public void OnPhotonPlayerConnected(PhotonPlayer newPlayer) {
		SetWeapon(WeaponManager.instance.GetCurrentWeapon());
	}
}
