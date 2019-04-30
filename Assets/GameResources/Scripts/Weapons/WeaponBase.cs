using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public enum FireMode {
	SemiAuto,
	FullAuto
}

public class WeaponBase : MonoBehaviour {
	protected Animator animator;
	protected AudioSource audioSource;
	protected FirstPersonController controller;
	protected CashSystem cashSystem;
	protected bool fireLock = false;
	protected bool canShoot = false;
	protected bool isReloading = false;

	[Header("Object References")]
	public ParticleSystem muzzleflash;
	public Transform shootPoint;
	// public GameObject sparkPrefab;
	// public GameObject bloodPrefab;
	public string sparkPrefabName;
	public string bloodPrefabName;

	[Header("UI References")]
	public Text weaponNameText;
	public Text ammoText;

	[Header("Sound References")]
	public AudioClip fireSound;
	public AudioClip dryFireSound;
	public AudioClip drawSound;
	public AudioClip magOutSound;
	public AudioClip magInSound;
	public AudioClip boltSound;

	[Header("Weapon Attributes")]
	public FireMode fireMode = FireMode.FullAuto;
	public float damage = 20f;
	public int pellets = 1;
	public float fireRate = 1.0f;
	public int bulletsInClip;
	public int clipSize = 12;
	public int bulletsLeft;
	public int maxAmmo = 100;
	public float spread = 0.7f;
	public float recoil = 0.5f;

	[Header("Upgrade Attributes")]
	[HideInInspector] public int damageUpgrade = 0;
	public float damageUpgradeFactor = 5;
	[HideInInspector] public int fasterReloadUpgrade = 0;
	public float fasterReloadUpgradeFactor = 0.1f;

	void Start() {
		Transform inGameUITransform = GameObject.Find("/Canvas/InGame").transform;
		weaponNameText = inGameUITransform.Find("WeaponNameText").GetComponent<Text>();
		ammoText = inGameUITransform.Find("AmmoText").GetComponent<Text>();

		GameObject player = GameObject.FindGameObjectWithTag("Player");

		controller = player.GetComponent<FirstPersonController>();
		animator = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();
		cashSystem = player.GetComponent<CashSystem>();

		bulletsInClip = clipSize;
		bulletsLeft = maxAmmo;

		// Wait until weapon can fire
		Invoke("EnableWeapon", 1f);
	}

	public void UpdateTexts() {
		weaponNameText.text = GetWeaponName();
		ammoText.text = "Ammo: " + bulletsInClip + " / " + bulletsLeft;
	}

	string GetWeaponName() {
		string weaponName = "";

		if(this is Police9mm) {
			weaponName = "Police 9mm";
		}
		else if(this is PortableMagnum) {
			weaponName = "Portable Magnum";
		}
		else if(this is Compact9mm) {
			weaponName = "Compact 9mm";
		}
		else if(this is UMP45) {
			weaponName = "UMP45";
		}
		else if(this is StovRifle) {
			weaponName = "Stov Rifle";
		}
		else if(this is DefenderShotgun) {
			weaponName = "Defender Shotgun";
		}
		else {
			throw new System.Exception("Unknown Weapon");
		}

		return weaponName;
	}

	void EnableWeapon() {
		canShoot = true;
	}

	void Update() {
		if(fireMode == FireMode.FullAuto && Input.GetButton("Fire1")) {
			CheckFire();
		}
		else if(fireMode == FireMode.SemiAuto && Input.GetButtonDown("Fire1")) {
			CheckFire();
		}

		if(Input.GetButtonDown("Reload")) {
			CheckReload();
		}
	}

	public void Select() {
		isReloading = false;
		Invoke("UpdateTexts", Time.deltaTime);
	}

	void CheckFire() {
		if(!canShoot) return;
		if(isReloading) return;
		if(fireLock) return;

		if(bulletsInClip > 0) {
			Fire();
		}
		else {
			DryFire();
		}
	}
	void Fire() {
		audioSource.PlayOneShot(fireSound);
		Player.instance.PlaySoundThroughNetwork(GetWeaponName() + "_Fire");

		fireLock = true;

		for(int i = 0; i < pellets; i++) {
			DetectHit();
		}

		Recoil();

		muzzleflash.Stop();
		muzzleflash.Play();
		Player.instance.PlayMuzzleflashThroughNetwork(GetWeaponName());

		Player.instance.PlayFireAnimation();
		PlayFireAnimation();

		bulletsInClip--;
		UpdateTexts();

		StartCoroutine(CoResetFireLock());
	}

	public void CreateBlood(Vector3 pos, Quaternion rot) {
		// GameObject blood = Instantiate(bloodPrefab, pos, rot);
		// Destroy(blood, 1f);

		GameObject blood = PhotonNetwork.Instantiate(bloodPrefabName, pos, rot, 0);
		DestroyAfterTime(blood, 1);
	}

	public virtual void PlayFireAnimation() {
		animator.CrossFadeInFixedTime("Fire", 0.1f);
	}

	void Recoil() {
		controller.mouseLook.Recoil(recoil);
	}

	void DetectHit() {
		RaycastHit hit;

		if(Physics.Raycast(shootPoint.position, CalculateSpread(spread, shootPoint), out hit)) {
			if(hit.transform.CompareTag("Enemy")) {
				Health targetHealth = hit.transform.GetComponent<Health>();

				if(targetHealth == null) {
					throw new System.Exception("Cannot found Health Component on Enemy.");
				}
				else {
					float actualDamage = damage + (damageUpgradeFactor * damageUpgrade);

					targetHealth.TakeDamage(actualDamage);
					CreateBlood(hit.point, hit.transform.rotation);

					Transform targetTransform;
					float targetHealthValue;

					if(targetHealth.parentRef == null) {
						targetTransform = hit.transform;
						targetHealthValue = targetHealth.value;
					}
					else {
						targetTransform = targetHealth.parentRef.transform;
						targetHealthValue = targetHealth.parentRef.value;
					}

					if(targetHealthValue <= 0) {
						KillReward killReward = targetTransform.GetComponent<KillReward>();

						if(killReward == null) {
							throw new System.Exception("Cannot found KillReward Component on Enemy.");
						}

						cashSystem.cash += killReward.amount;
						GameManager.instance.zombieKilled++;
					}
				}
			}
			else {
				// GameObject spark = Instantiate(sparkPrefab, hit.point, hit.transform.rotation);
				// Destroy(spark, 1);

				GameObject spark = PhotonNetwork.Instantiate(sparkPrefabName, hit.point, hit.transform.rotation, 0);
				DestroyAfterTime(spark, 1);
			}
		}
	}

	void DestroyAfterTime(GameObject obj, float time) {
		StartCoroutine(CoDestroyAfterTime(obj, time));
	}

	IEnumerator CoDestroyAfterTime(GameObject obj, float time) {
		yield return new WaitForSeconds(time);
		PhotonNetwork.Destroy(obj);
	}

	Vector3 CalculateSpread(float spread, Transform shootPoint) {
		return Vector3.Lerp(shootPoint.TransformDirection(Vector3.forward * 100), Random.onUnitSphere, spread);
	}

	void DryFire() {
		audioSource.PlayOneShot(dryFireSound);
		Player.instance.PlaySoundThroughNetwork("DryFire");
		fireLock = true;

		StartCoroutine(CoResetFireLock());
	}

	IEnumerator CoResetFireLock() {
		yield return new WaitForSeconds(fireRate);
		fireLock = false;
	}

	void CheckReload() {
		if(bulletsLeft > 0 && bulletsInClip < clipSize) {
			Player.instance.PlayReloadAnimation();
			Reload();
		}
	}

	protected virtual void Reload() {
		if(isReloading) return;
		
		isReloading = true;
		animator.CrossFadeInFixedTime("Reload", 0.1f);
		animator.SetFloat("ReloadSpeed", 1 + (fasterReloadUpgradeFactor * fasterReloadUpgrade));
	}

	protected virtual void ReloadAmmo() {
		int bulletsToLoad = clipSize - bulletsInClip;
		int bulletsToSub = (bulletsLeft >= bulletsToLoad) ? bulletsToLoad : bulletsLeft;

		bulletsLeft -= bulletsToSub;
		bulletsInClip += bulletsToSub;

		UpdateTexts();
	}

	public virtual void OnDraw() {
		audioSource.PlayOneShot(drawSound);
		Player.instance.PlaySoundThroughNetwork(GetWeaponName() + "_Draw");
	}

	public virtual void OnMagOut() {
		audioSource.PlayOneShot(magOutSound);
		Player.instance.PlaySoundThroughNetwork(GetWeaponName() + "_MagOut");
	}

	public virtual void OnMagIn() {
		ReloadAmmo();
		audioSource.PlayOneShot(magInSound);
		Player.instance.PlaySoundThroughNetwork(GetWeaponName() + "_MagIn");
	}

	public virtual void OnBoltForwarded() {
		audioSource.PlayOneShot(boltSound);
		Player.instance.PlaySoundThroughNetwork(GetWeaponName() + "_BoltForwarded");

		Invoke("ResetIsReloading", 0.5f);
	}

	protected void ResetIsReloading() {
		isReloading = false;
	}

	public int GetTotalAmmo() {
		return clipSize + maxAmmo;
	}

	public int GetCurrentAmmo() {
		return bulletsInClip + bulletsLeft;
	}

	public void RefillAmmo() {
		bulletsLeft = clipSize + maxAmmo;
		UpdateTexts();
	}
}
