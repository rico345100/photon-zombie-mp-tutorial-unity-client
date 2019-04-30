using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon {
	None,
	Police9mm,
	PortableMagnum,
	Compact9mm,
	UMP45,
	StovRifle,
	DefenderShotgun
}

public class WeaponManager : MonoBehaviour {
	public static WeaponManager instance;
	private int currentWeaponIndex = 0;
	private Weapon primaryWeapon = Weapon.None;
	private Weapon secondaryWeapon = Weapon.Police9mm;
	private Weapon currentWeapon;
	private GameObject primaryWeaponObj;
	private GameObject secondaryWeaponObj;
	private GameObject currentWeaponObj;

	void Awake() {
		if(instance == null) {
			instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	void Start() {
		currentWeapon = secondaryWeapon;
		secondaryWeaponObj = FindWeaponObject(secondaryWeapon);

		currentWeaponObj = secondaryWeaponObj;
		
		SelectCurrentWeapon();
	}

	public void SetPrimaryWeapon(Weapon weapon) {
		currentWeaponObj.SetActive(false);

		primaryWeapon = weapon;
		primaryWeaponObj = FindWeaponObject(weapon);
		
		currentWeapon = primaryWeapon;
		currentWeaponObj = primaryWeaponObj;
		SelectCurrentWeapon();
	}

	public void SetSecondaryWeapon(Weapon weapon) {
		currentWeaponObj.SetActive(false);

		secondaryWeapon = weapon;
		secondaryWeaponObj = FindWeaponObject(weapon);

		currentWeapon = secondaryWeapon;
		currentWeaponObj = secondaryWeaponObj;
		SelectCurrentWeapon();
	}

	public void ReplaceCurrentWeapon(Weapon weapon) {
		if(currentWeapon == primaryWeapon) {
			SetPrimaryWeapon(weapon);
		}
		else {
			SetSecondaryWeapon(weapon);
		}
	}

	public bool HasPrimaryWeapon() {
		return primaryWeapon != Weapon.None;
	}

	public bool HasWeapon(Weapon weapon) {
		return primaryWeapon == weapon || secondaryWeapon == weapon;
	}
	
	GameObject FindWeaponObject(Weapon weapon) {
		return transform.Find(weapon.ToString()).gameObject;
	}

	public GameObject GetCurrentWeaponObject() {
		return currentWeaponObj;
	}

	void SelectCurrentWeapon() {
		currentWeaponObj.SetActive(true);
		currentWeaponObj.GetComponent<WeaponBase>().Select();

		Player.instance.SetWeapon(currentWeapon);
	}

	public Weapon GetCurrentWeapon() {
		return currentWeapon;
	}

	void Update() {
		if(primaryWeapon != null && currentWeapon != primaryWeapon && Input.GetKeyDown(KeyCode.Alpha1)) {
			currentWeapon = primaryWeapon;
			currentWeaponObj = primaryWeaponObj;

			secondaryWeaponObj.SetActive(false);
			SelectCurrentWeapon();
		}
		else if(secondaryWeapon != null && currentWeapon != secondaryWeapon && Input.GetKeyDown(KeyCode.Alpha2)) {
			currentWeapon = secondaryWeapon;
			currentWeaponObj = secondaryWeaponObj;

			primaryWeaponObj.SetActive(false);
			SelectCurrentWeapon();
		}
	}
}
