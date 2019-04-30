using UnityEngine;
using UnityEngine.UI;

public class RoomView : ViewBase {
	public StartView startView;
	public ViewBase browseRoomView;
	public Text roomNameText;
	public Text playerCountText;
	public Button startGameButton;
	public Button exitButton;

	protected override void OnInit() {
		startGameButton.onClick.AddListener(() => {
			NetworkManager.instance.StartGame();
		});

		exitButton.onClick.AddListener(() => {
			NetworkManager.instance.LeaveRoom();
			
			this.Hide();
			browseRoomView.Show();
		});

		NetworkManager.instance.onPlayerConnected.AddListener(UpdatePlayerCount);
		NetworkManager.instance.onPlayerDisconnected.AddListener(UpdatePlayerCount);
		NetworkManager.instance.onGameStarted.AddListener(() => {
			this.Hide();
			startView.lobbyCam.SetActive(false);
			startView.inGameUI.SetActive(true);
		});
		NetworkManager.instance.onGameOver.AddListener(() => {
			this.Show();
			startView.lobbyCam.SetActive(true);
			startView.inGameUI.SetActive(false);

			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		});
	}

	public void UpdatePlayerCount() {
		int playerCount = PhotonNetwork.playerList.Length;
		playerCountText.text = string.Format("{0} Player(s) Connected.", playerCount);
	}
}
