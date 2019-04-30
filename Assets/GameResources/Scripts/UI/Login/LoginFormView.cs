using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoginFormView : ViewBase {
	[Header("LoginForm UI Refs")]
	public InputField idField;
	public InputField pwField;
	public Button loginButton;
	public Button registerButton;
	public ViewBase registerFormView;
	public ViewBase loginView;
	public ViewBase startView;
	public Text errorText;

	protected override void OnInit() {
		loginButton.onClick.AddListener(ValidateFields);
		registerButton.onClick.AddListener(MoveToRegisterForm);
	}

	protected override void OnShow() {
		ResetFields();
		HideError();
	}

	void ValidateFields() {
		string id = idField.text;
		string pw = pwField.text;

		if(id.Equals("")) {
			DisplayError("ID is Empty");
			return;
		}
		else if(pw.Equals("")) {
			DisplayError("Password is Empty");
			return;
		}

		HideError();

		WWWForm loginFormData = new WWWForm();
		loginFormData.AddField("id", id);
		loginFormData.AddField("password", pw);

		Submit(loginFormData);
	}

	void Submit(WWWForm formData) {
		StartCoroutine(CoSubmit(formData));
	}

	IEnumerator CoSubmit(WWWForm formData) {
		WWW httpResult = new WWW("http://52.198.125.64:3000/auth", formData);

		yield return httpResult;

		if(httpResult.responseHeaders.Count > 0) {
			string statusText = httpResult.responseHeaders["STATUS"];
			int statusCode = HttpHelper.GetStatusCode(statusText);
			JSONObject resultJson = new JSONObject(httpResult.text);

			if(statusCode == 200) {
				string token = resultJson.GetField("token").str;

				NetworkManager.Token = token;
				NetworkManager.UserId = idField.text;
				
				ResetFields();
				MoveToStartView();
			}
			else {
				string errorMessage = resultJson.GetField("message").str;
				DisplayError(errorMessage);
			}
		}
		else {
			DisplayError("Can't Connect to Server.");
		}
	}

	void MoveToRegisterForm() {
		this.Hide();
		registerFormView.Show();
	}

	void MoveToStartView() {
		this.Hide();
		loginView.Hide();
		startView.Show();
	}

	void ResetFields() {
		idField.text = "";
		pwField.text = "";
	}

	void DisplayError(string message) {
		errorText.text = message;
		errorText.gameObject.SetActive(true);
	}

	void HideError() {
		errorText.gameObject.SetActive(false);
	}
}
