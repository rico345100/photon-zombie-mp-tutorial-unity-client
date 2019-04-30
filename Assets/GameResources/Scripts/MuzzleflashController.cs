using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleflashController : MonoBehaviour {
	public ParticleSystem police9mmMuzzleflash;
	public ParticleSystem portableMagnumMuzzleflash;
	public ParticleSystem compact9mmMuzzleflash;
	public ParticleSystem ump45Muzzleflash;
	public ParticleSystem defenderShotgunMuzzleflash;
	public ParticleSystem stovRifleMuzzleflash;

	public void PlayMuzzleflash(string identifier) {
		switch(identifier) {
			case "Police 9mm":
				police9mmMuzzleflash.Stop();
				police9mmMuzzleflash.Play();
				break;
			case "Portable Magnum":
				portableMagnumMuzzleflash.Stop();
				portableMagnumMuzzleflash.Play();
				break;
			case "Compact 9mm":
				compact9mmMuzzleflash.Stop();
				compact9mmMuzzleflash.Play();
				break;
			case "UMP45":
				ump45Muzzleflash.Stop();
				ump45Muzzleflash.Play();
				break;
			case "Defender Shotgun":
				defenderShotgunMuzzleflash.Stop();
				defenderShotgunMuzzleflash.Play();
				break;
			case "Stov Rifle":
				stovRifleMuzzleflash.Stop();
				stovRifleMuzzleflash.Play();
				break;
		}
	}
}
