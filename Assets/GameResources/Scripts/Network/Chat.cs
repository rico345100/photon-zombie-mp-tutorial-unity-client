using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon.Chat;

public class Chat : MonoBehaviour, IChatClientListener {
	[SerializeField] private GameObject container;
	[SerializeField] private GameObject messageList;
	[SerializeField] private InputField messageInput;
	[SerializeField] private Text messageText;
	private ChatClient chatClient;
	private string channelName;
	private bool isTalking = false;

	[Header("Key Settings")]
	[SerializeField] private KeyCode chatKey = KeyCode.T;
	[SerializeField] private KeyCode submitKey = KeyCode.Return;
	[SerializeField] private KeyCode cancelKey = KeyCode.Escape;

	void Update() {
		if(chatClient != null) {
			chatClient.Service();

			if(chatClient.State == ChatState.ConnectedToFrontEnd) {
				if(!isTalking && Input.GetKeyDown(chatKey)) {
					isTalking = true;
					ShowMessageInput();
				}
				else if(isTalking && Input.GetKeyDown(submitKey)) {
					string message = messageInput.text;

					if(message.Equals("")) return;

					chatClient.PublishMessage(channelName, message);

					isTalking = false;
					HideMessageInput();
				}
				else if(Input.GetKeyDown(cancelKey)) {
					isTalking = false;
					HideMessageInput();
				}
			}
		}
	}

	void ShowMessageInput() {
		messageInput.text = "";
		messageInput.gameObject.SetActive(true);

		if(Player.instance) {
			Player.instance.firstPersonController.enabled = false;
		}
		
		StartCoroutine(CoGiveMessageFocus());
	}

	void HideMessageInput() {
		if(Player.instance) {
			Player.instance.firstPersonController.enabled = true;
		}
		
		messageInput.gameObject.SetActive(false);
	}

	IEnumerator CoGiveMessageFocus() {
		yield return new WaitForSecondsRealtime(0.1f);

		messageInput.Select();
		messageInput.ActivateInputField();
	}

	public void Connect(string version, string userName, string channelName) {
		messageText.text = "";
		this.channelName = channelName;

		chatClient = new ChatClient(this);
		chatClient.Connect(
			PhotonNetwork.PhotonServerSettings.ChatAppID,
			version,
			new ExitGames.Client.Photon.Chat.AuthenticationValues(userName)
		);
	}

	public void Disconnect() {
		if(chatClient != null) {
			chatClient.Disconnect();
		}
	}

	public virtual void OnConnected() {
		print("Connected to PhotonChat");
		container.SetActive(true);

		chatClient.Subscribe(new string[]{ channelName });
		chatClient.SetOnlineStatus(ChatUserStatus.Online);
	}

	public virtual void OnDisconnected() {
		print("Disconnected from PhotonChat");
		container.SetActive(false);
	}

	public virtual void OnGetMessages(string channelName, string[] senders, object[] messages) {
		for(int i = 0; i < senders.Length; i++) {
			messageText.text = messageText.text + "\n" + senders[i] + ": " + messages[i];
		}

		// Scroll to bottom automatically
		StartCoroutine(CoScrollToBottom());
	}

	IEnumerator CoScrollToBottom() {
		yield return new WaitForSecondsRealtime(0.1f);
		messageList.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
	}

	public void OnSubscribed(string[] channels, bool[] results) {
		messageText.text = "Chat Online.";
		chatClient.PublishMessage(channelName, "Joined to channel " + channelName);
	}

	public void DebugReturn(ExitGames.Client.Photon.DebugLevel debugLevel, string message) {
		print(message);
	}

	void OnApplicationQuit() {
		Disconnect();
	}

	// Unusing Methods
	public void OnPrivateMessage(string sender, object message, string channelName) {}
	public void OnUnsubscribed(string[] channels) {}
	public void OnStatusUpdate(string user, int status, bool gotMessage, object message) {}
	public void OnChatStateChange(ChatState state) {}
}
