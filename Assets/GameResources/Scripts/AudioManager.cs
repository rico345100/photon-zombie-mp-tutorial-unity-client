using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	public static AudioManager instance;
	public AudioClip dryFire;

	// Police 9mm
	public AudioClip police9mmFire;
	public AudioClip police9mmDraw;
	public AudioClip police9mmMagOut;
	public AudioClip police9mmMagIn;
	public AudioClip police9mmBoltForwarded;

	// Portable Magnum
	public AudioClip portableMagnumFire;
	public AudioClip portableMagnumDraw;
	public AudioClip portableMagnumMagOut;
	public AudioClip portableMagnumMagIn;
	public AudioClip portableMagnumBoltForwarded;

	// Compact 9mm
	public AudioClip compact9mmFire;
	public AudioClip compact9mmDraw;
	public AudioClip compact9mmMagOut;
	public AudioClip compact9mmMagIn;
	public AudioClip compact9mmBoltForwarded;

	// UMP45
	public AudioClip ump45Fire;
	public AudioClip ump45Draw;
	public AudioClip ump45MagOut;
	public AudioClip ump45MagIn;
	public AudioClip ump45BoltForwarded;

	// Defender Shotgun
	public AudioClip defenderShotgunFire;
	public AudioClip defenderShotgunDraw;
	public AudioClip defenderShotgunMagOut;
	public AudioClip defenderShotgunMagIn;
	public AudioClip defenderShotgunBoltForwarded;

	// Stov Rifle
	public AudioClip stovRifleFire;
	public AudioClip stovRifleDraw;
	public AudioClip stovRifleMagOut;
	public AudioClip stovRifleMagIn;
	public AudioClip stovRifleBoltForwarded;

	void Awake() {
		instance = this;
	}
}
