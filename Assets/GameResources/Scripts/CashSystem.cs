using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CashSystem : MonoBehaviour {
	private float m_Cash;
	public Text cashText;
	public float initialCash = 0;
	public float cash {
		get {
			return m_Cash;
		}
		set {
			m_Cash = value;
			UpdateUI();
		}
	}

	void Start() {
		Transform inGameUITransform = GameObject.Find("/Canvas/InGame").transform;
		cashText = inGameUITransform.Find("Cash").GetComponent<Text>();

		cash = initialCash;
	}

	void UpdateUI() {
		cashText.text = "Cash: " + m_Cash + "$";
	}
}
