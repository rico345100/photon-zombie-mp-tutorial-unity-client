using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeMagazineWeapon : WeaponBase {
	[Header("Tube Magazine Weapon Sound Refs")]
	public AudioClip ammoInsertSound;

	protected override void Reload() {
		if(isReloading) return;
		
		isReloading = true;

		if(bulletsInClip <= 0) {
			animator.CrossFadeInFixedTime("ReloadStartEmpty", 0.1f);
		}
		else {
			animator.CrossFadeInFixedTime("ReloadStart", 0.1f);
		}
	}

	protected override void ReloadAmmo() {
		bulletsLeft--;
		bulletsInClip++;

		UpdateTexts();
	}

	public void CheckNextReload() {
		isReloading = true;
		bool stopInserting = false;

		if(bulletsLeft <= 0) {
			stopInserting = true;
		}
		else if(bulletsInClip >= clipSize) {
			stopInserting = true;
		}

		if(stopInserting) {
			animator.CrossFadeInFixedTime("ReloadEnd", 0.1f);
		}
		else {
			animator.CrossFadeInFixedTime("ReloadInsert", 0.1f);
		}
	}

	public void OnAmmoInserted() {
		isReloading = false;
		audioSource.PlayOneShot(ammoInsertSound);
		ReloadAmmo();
	}
}
