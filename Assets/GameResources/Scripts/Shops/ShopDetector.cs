using UnityEngine;
using UnityEngine.UI;

public class ShopDetector : MonoBehaviour {
	private AudioSource audioSource;
	private WeaponManager weaponManager;
	private CashSystem cashSystem;
	public Transform shootPoint;
	public float detectRange = 2f;
	public Text shopText;
	public AudioClip buySound;
	public AudioClip errorSound;

	void Start() {
		Transform inGameUITransform = GameObject.Find("/Canvas/InGame").transform;
		shopText = inGameUITransform.Find("ShopText").GetComponent<Text>();

		audioSource = GetComponent<AudioSource>();
		weaponManager = GetComponentInChildren<WeaponManager>();
		cashSystem = GetComponent<CashSystem>();
	}

	void Update() {
		RaycastHit hit;

		if(Physics.Raycast(shootPoint.position, shootPoint.forward, out hit, detectRange)) {
			ShopBase shopBase = hit.transform.GetComponent<ShopBase>();

			if(shopBase == null) {
				shopText.text = "";
				return;
			}
			
			if(shopBase is AmmoShop) {
				shopText.text = "Press F to buy ammo";
			}
			else if(shopBase is PortableMagnumShop) {
				shopText.text = "Press F to buy Portable Magnum";
			}
			else if(shopBase is Compact9mmShop) {
				shopText.text = "Press F to buy Compact 9mm";
			}
			else if(shopBase is UMP45Shop) {
				shopText.text = "Press F to buy UMP45";
			}
			else if(shopBase is DefenderShotgunShop) {
				shopText.text = "Press F to buy Defender Shotgun";
			}
			else if(shopBase is StovRifleShop) {
				shopText.text = "Press F to buy Stov Rifle";
			}
			else if(shopBase is DamageUpgrade) {
				shopText.text = "Press F to buy Damage Upgrade";
			}
			else if(shopBase is FasterReloadUpgrade) {
				shopText.text = "Press F to buy Faster Reload Upgrade";
			}

			shopText.text += " (" + GetCost(shopBase) + "$)";

			if(Input.GetKeyDown(KeyCode.F)) {
				if(cashSystem.cash < GetCost(shopBase)) {
					print("NOT ENOUGH CASH");
					audioSource.PlayOneShot(errorSound);
					return;
				}

				bool purchased = false;

				if(shopBase is AmmoShop) {
					purchased = BuyAmmo();
				}
				else if(shopBase is PortableMagnumShop) {
					purchased = BuyWeapon(Weapon.PortableMagnum);
				}
				else if(shopBase is Compact9mmShop) {
					purchased = BuyWeapon(Weapon.Compact9mm);
				}
				else if(shopBase is UMP45Shop) {
					purchased = BuyWeapon(Weapon.UMP45);
				}
				else if(shopBase is DefenderShotgunShop) {
					purchased = BuyWeapon(Weapon.DefenderShotgun);
				}
				else if(shopBase is StovRifleShop) {
					purchased = BuyWeapon(Weapon.StovRifle);
				}
				else if(shopBase is UpgradeBase) {
					WeaponBase weaponBase = WeaponManager.instance.GetCurrentWeaponObject().GetComponent<WeaponBase>();

					if(shopBase is DamageUpgrade) {
						purchased = BuyUpgrade(Upgrade.Damage, weaponBase);
					}
					else if(shopBase is FasterReloadUpgrade) {
						purchased = BuyUpgrade(Upgrade.FasterReload, weaponBase);
					}
				}

				if(purchased) {
					audioSource.PlayOneShot(buySound);
					cashSystem.cash -= GetCost(shopBase);

					if(shopBase is UpgradeBase) {
						shopBase.purchased++;
					}
				}
				else {
					audioSource.PlayOneShot(errorSound);
				}
			}
		}
		else {
			shopText.text = "";
			return;
		}
	}

	float GetCost(ShopBase shopBase) {
		if(shopBase.accumulation) {
			return shopBase.cost * (shopBase.purchased + 1);
		}
		else {
			return shopBase.cost;
		}
	}

	bool BuyAmmo() {
		WeaponBase currentWeaponBase = weaponManager.GetCurrentWeaponObject().GetComponent<WeaponBase>();

		if(currentWeaponBase.GetCurrentAmmo() < currentWeaponBase.GetTotalAmmo()) {
			currentWeaponBase.RefillAmmo();
			return true;
		}
		else {
			print("ALREADY HAVE FULL AMMO");
			return false;
		}
	}

	bool BuyWeapon(Weapon weapon) {
		if(weaponManager.HasWeapon(weapon)) {
			print("YOU ALREADY HAVE " + weapon);
			return false;
		}

		if(!weaponManager.HasPrimaryWeapon()) {
			weaponManager.SetPrimaryWeapon(weapon);
		}
		else {
			weaponManager.ReplaceCurrentWeapon(weapon);
		}

		return true;
	}

	bool BuyUpgrade(Upgrade upgrade, WeaponBase weaponBase) {
		if(upgrade == Upgrade.Damage) {
			weaponBase.damageUpgrade++;
			return true;
		}
		else if(upgrade == Upgrade.FasterReload) {
			weaponBase.fasterReloadUpgrade++;
			return true;
		}

		return false;
	}
}
