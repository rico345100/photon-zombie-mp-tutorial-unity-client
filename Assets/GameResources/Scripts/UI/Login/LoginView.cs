using UnityEngine;
using UnityEngine.UI;

public class LoginView : ViewBase {
	public Button exitButton;
	public ViewBase loginFormView;

	protected override void OnShow() {
		loginFormView.Show();
	}

	protected override void OnInit() {
		exitButton.onClick.AddListener(() => {
			Application.Quit();
		});
	}
}
