using UnityEngine;
using UnityEngine.UI;

public class BrowseRoomView : ViewBase {
	public ViewBase startView;
	public RoomView roomView;
	public Transform roomList;
	public Button exitButton;
	public Button roomJoinButtonPrefab;

	protected override void OnInit() {
		exitButton.onClick.AddListener(() => {
			this.Hide();
			startView.Show();

			NetworkManager.instance.Disconnect();
		});

		NetworkManager.instance.onRoomListUpdate.AddListener(UpdateRoomList);
	}

	protected override void OnShow() {
		// Empty Room List
		while(roomList.childCount > 0) {
			Destroy(roomList.GetChild(0).gameObject);
		}
	}

	void UpdateRoomList() {
		RoomInfo[] rooms = PhotonNetwork.GetRoomList();

		foreach(RoomInfo room in rooms) {
			Button roomJoinButton = Instantiate(roomJoinButtonPrefab);
			roomJoinButton.onClick.AddListener(() => {
				NetworkManager.instance.JoinRoom(room.Name, () => {
					roomView.startGameButton.gameObject.SetActive(false);
					roomView.roomNameText.text = room.Name;
					roomView.UpdatePlayerCount();

					this.Hide();
					roomView.Show();
				});
			});

			roomJoinButton.GetComponentInChildren<Text>().text = string.Format("{0} ({1}/4) - Join", room.Name, room.PlayerCount);
			roomJoinButton.transform.parent = roomList;
		}
	}
}
