using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public delegate void CallbackFn();

public class NetworkManager : Photon.PunBehaviour {
	public static NetworkManager instance;
	public static string Token;
	public static string UserId;
	public const string version = "1.0";
	public Text status;
	public Chat chat;
	private string roomName = "";
	private CallbackFn onConnected;
	private CallbackFn onConnectionFailed;
	private CallbackFn onRoomJoined;
	[HideInInspector] public UnityEvent onPlayerConnected;
	[HideInInspector] public UnityEvent onPlayerDisconnected;
	[HideInInspector] public UnityEvent onRoomListUpdate;
	[HideInInspector] public UnityEvent onGameStarted;
	[HideInInspector] public UnityEvent onGameOver;

	void Awake() {
		instance = this;

		onPlayerConnected = new UnityEvent();
		onPlayerDisconnected = new UnityEvent();
		onRoomListUpdate = new UnityEvent();
		onGameStarted = new UnityEvent();
		onGameOver = new UnityEvent();
	}
	
	void Update() {
		status.text = PhotonNetwork.connectionStateDetailed.ToString();
	}

	public void Connect(CallbackFn onConnected, CallbackFn onConnectionFailed) {
		this.onConnected = onConnected;
		this.onConnectionFailed = onConnectionFailed;
		PhotonNetwork.ConnectUsingSettings(version);
	}

	public void Disconnect() {
		PhotonNetwork.Disconnect();
	}

	public override void OnConnectionFail(DisconnectCause cause) {
		if(onConnectionFailed != null) {
			onConnectionFailed();
			onConnectionFailed = null;
		}
	}

	public override void OnJoinedLobby() {
		if(onConnected != null) {
			onConnected();
			onConnected = null;
		}
	}

	public void JoinOrCreateRoom(string roomName, CallbackFn onRoomJoined) {
		RoomOptions roomOptions = new RoomOptions() { IsVisible = true, MaxPlayers = 4 };
		PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);

		this.onRoomJoined = onRoomJoined;
		this.roomName = roomName;
	}

	public void JoinRoom(string roomName, CallbackFn onRoomJoined) {
		PhotonNetwork.JoinRoom(roomName);
		this.onRoomJoined = onRoomJoined;
		this.roomName = roomName;
	}

	public override void OnJoinedRoom() {
		onRoomJoined();
		onRoomJoined = null;

		chat.Connect(version, PhotonNetwork.player.ID.ToString(), roomName);
	}

	public override void OnLeftRoom() {
		chat.Disconnect();
		roomName = "";
	}

	public void LeaveRoom() {
		PhotonNetwork.LeaveRoom();
	}

	public virtual void OnReceivedRoomListUpdate() {
		onRoomListUpdate.Invoke();
	}

	public virtual void OnPhotonPlayerConnected(PhotonPlayer player) {
		this.onPlayerConnected.Invoke();
	}

	public virtual void OnPhotonPlayerDisconnected(PhotonPlayer player) {
		this.onPlayerDisconnected.Invoke();
	}

	public void StartGame() {
		photonView.RPC("RPCStartGame", PhotonTargets.All);
	}

	[PunRPC]
	void RPCStartGame() {
		GameManager.instance.StartGame();
		onGameStarted.Invoke();
	}

	public void UpdateUserStatus(int zombieKilled) {
		StartCoroutine(CoUpdateUserStatus(zombieKilled));
	}

	IEnumerator CoUpdateUserStatus(int zombieKilled) {
		WWWForm updateFormData = new WWWForm();
		updateFormData.AddField("_method", "put");
		updateFormData.AddField("kills", zombieKilled);

		string token = Token;
		string userId = UserId;

		WWW httpResult = new WWW("http://52.198.125.64:3000/users?token=" + token + "&id=" + userId, updateFormData);

		yield return httpResult;

		if(httpResult.responseHeaders.Count > 0) {
			string statusText = httpResult.responseHeaders["STATUS"];
			int statusCode = HttpHelper.GetStatusCode(statusText);

			if(statusCode == 200) {
				print("User Data Updated");
			}
			else {
				print("Failed to Update.");
			}
		}
		else {
			print("Can't Connect to Server.");
		}
	}
}
