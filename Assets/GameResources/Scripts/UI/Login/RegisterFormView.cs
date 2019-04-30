using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RegisterFormView : ViewBase {
	[Header("RegisterForm UI Refs")]
	public InputField idField;
	public InputField pwField;
	public InputField pwcField;
	public Button cancelButton;
	public Button registerButton;
	public ViewBase loginFormView;
	public Text errorText;

	protected override void OnInit() {
		cancelButton.onClick.AddListener(MoveToLogin);
		registerButton.onClick.AddListener(ValidateFields);
	}

	protected override void OnShow() {
		ResetFields();
		HideError();
	}

	void ValidateFields() {
		string id = idField.text;
		string pw = pwField.text;
		string pwc = pwcField.text;

		if(id.Equals("")) {
			DisplayError("ID is Empty");
			return;
		}
		else if(pw.Equals("")) {
			DisplayError("Password is Empty");
			return;
		}
		else if(pwc.Equals("")) {
			DisplayError("Password Confirm is Empty");
			return;
		}
		else if(!pw.Equals(pwc)) {
			DisplayError("Password not matched");
			return;
		}

		HideError();

		WWWForm registerFormData = new WWWForm();
		registerFormData.AddField("id", id);
		registerFormData.AddField("password", pw);

		Submit(registerFormData);
	}

	void Submit(WWWForm formData) {
		StartCoroutine(CoSubmit(formData));
	}

	IEnumerator CoSubmit(WWWForm formData) {
		WWW httpResult = new WWW("http://52.198.125.64:3000/users", formData);

		yield return httpResult;

		if(httpResult.responseHeaders.Count > 0) {
			string statusText = httpResult.responseHeaders["STATUS"];
			int statusCode = HttpHelper.GetStatusCode(statusText);

			if(statusCode == 200) {
				ResetFields();
				MoveToLogin();
			}
			else {
				DisplayError("Failed to Register.");
			}
		}
		else {
			DisplayError("Can't Connect to Server.");
		}
	}

	void MoveToLogin() {
		this.Hide();
		loginFormView.Show();
	}

	void ResetFields() {
		idField.text = "";
		pwField.text = "";
		pwcField.text = "";
	}

	void DisplayError(string message) {
		errorText.text = message;
		errorText.gameObject.SetActive(true);
	}

	void HideError() {
		errorText.gameObject.SetActive(false);
	}
}
