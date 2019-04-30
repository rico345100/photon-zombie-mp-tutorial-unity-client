using UnityEngine;
using UnityEngine.UI;

public class CreateRoomView : ViewBase {
	public ViewBase startView;
	public RoomView roomView;
	public InputField roomNameField;
	public Button createButton;
	public Button exitButton;

	protected override void OnInit() {
		createButton.onClick.AddListener(CreateRoom);
		exitButton.onClick.AddListener(() => {
			NetworkManager.instance.Disconnect();

			this.Hide();
			startView.Show();
		});
	}

	protected override void OnShow() {
		roomNameField.text = "";
	}

	void CreateRoom() {
		string roomName = roomNameField.text;

		if(roomName.Equals("")) {
			print("Room Name is Empty!");
			return;
		}

		NetworkManager.instance.JoinOrCreateRoom(roomName, () => {
			roomView.roomNameText.text = roomName;
			roomView.playerCountText.text = "1 Player(s) Connected.";

			this.Hide();
			roomView.Show();

			roomView.startGameButton.gameObject.SetActive(true);
		});
	}
}
