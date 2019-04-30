using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspector : MonoBehaviour {
	float speed = 6;
	Vector3 moveDir = Vector3.zero;
	CharacterController controller;

	void Start() {
		controller = GetComponent<CharacterController>();
	}

	void FixedUpdate() {
		moveDir = new Vector3(
			Input.GetAxis("Horizontal"),
			Input.GetAxis("Jump") + (-Input.GetAxis("Crouch")),
			Input.GetAxis("Vertical")
		);

		moveDir = transform.TransformDirection(moveDir);
		moveDir *= speed;

		controller.Move(moveDir * Time.fixedDeltaTime);
	}
}
