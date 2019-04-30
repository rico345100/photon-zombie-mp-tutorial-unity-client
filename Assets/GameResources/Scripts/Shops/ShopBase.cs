using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShopBase : MonoBehaviour {
    public float cost = 100;
    public bool accumulation = false;
    [HideInInspector] public int purchased = 0;
}
