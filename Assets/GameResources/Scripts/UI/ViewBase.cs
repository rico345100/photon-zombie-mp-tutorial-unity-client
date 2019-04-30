using UnityEngine;

public abstract class ViewBase : MonoBehaviour {
	public GameObject viewObject;
	protected virtual void OnShow() {}
	protected virtual void OnHide() {}
	protected virtual void OnInit() {}

	void Start() {
		OnInit();
	}

	public void Show() {
		OnShow();
		viewObject.SetActive(true);
	}

	public void Hide() {
		OnHide();
		viewObject.SetActive(false);
	}
}
