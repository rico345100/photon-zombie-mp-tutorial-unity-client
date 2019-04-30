using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartView : ViewBase {
	[Header("View Refs")]
	public ViewBase loginView;
	public ViewBase browseRoomView;
	public ViewBase createRoomView;
	public GameObject lobbyCam;
	public GameObject mainUI;
	public GameObject inGameUI;
	public Button browseRoomButton;
	public Button createRoomButton;
	public Button exitButton;

	protected override void OnInit() {
		browseRoomButton.onClick.AddListener(() => {
			NetworkManager.instance.Connect(
				() => {
					this.Hide();
					browseRoomView.Show();
				},
				() => {
					print("Failed to Connect");
				}
			);
		});

		createRoomButton.onClick.AddListener(() => {
			NetworkManager.instance.Connect(
				() => {
					this.Hide();
					createRoomView.Show();
				},
				() => {
					print("Failed to Connect");
				}
			);
		});

		exitButton.onClick.AddListener(() => {
			this.Hide();
			loginView.Show();

			Logout();
		});
	}

	protected override void OnShow() {
		lobbyCam.SetActive(true);
		mainUI.SetActive(true);
		inGameUI.SetActive(false);
	}

	void Logout() {
		WWWForm logoutFormData = new WWWForm();
		logoutFormData.AddField("_method", "delete");
		Submit(logoutFormData);
	}

	void Submit(WWWForm formData) {
		StartCoroutine(CoSubmit(formData));
	}

	IEnumerator CoSubmit(WWWForm formData) {
		string token = NetworkManager.Token;
		string userId = NetworkManager.UserId;

		WWW httpResult = new WWW("http://52.198.125.64:3000/auth?token=" + token + "&id=" + userId, formData);

		yield return httpResult;

		NetworkManager.Token = null;
		NetworkManager.UserId = null;

		if(httpResult.responseHeaders.Count > 0) {
			string statusText = httpResult.responseHeaders["STATUS"];
			int statusCode = HttpHelper.GetStatusCode(statusText);
			JSONObject resultJson = new JSONObject(httpResult.text);

			if(statusCode == 200) {
				print("Logout Success");
			}
			else {
				string errorMessage = resultJson.GetField("message").str;
				print(errorMessage);
			}
		}
		else {
			print("Can't Connect to Server.");
		}
	}
}
