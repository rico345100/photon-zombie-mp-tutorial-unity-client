using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderShotgun : TubeMagazineWeapon {
    public void OnPump() {
        audioSource.PlayOneShot(boltSound);
    }

    public override void OnBoltForwarded() {
        // Don't play any sound
        Invoke("ResetIsReloading", 0.5f);
    }
}
