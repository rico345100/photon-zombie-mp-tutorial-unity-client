using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class WeaponBob : MonoBehaviour {
    private FirstPersonController fpsController;
	private float timer = 0.0f;
    private float xInit, yInit;
    private float xOffset, yOffset;
    public float bobbingSpeed = 0.1f;
    public float bobbingAmount = 0.1f;

    void Start() {
		fpsController = transform.parent.parent.parent.GetComponent<FirstPersonController>();

        xInit = transform.localPosition.x;
        yInit = transform.localPosition.y;
        
        xOffset = xInit;
        yOffset = yInit;
    }

    public void Reset() {
        xOffset = xInit;
        yOffset = yInit;
    }

    void Update () {
        if(!fpsController.controller.isGrounded) return;

        float xMovement = 0.0f;
        float yMovement = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 calcPosition = transform.localPosition; 

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0) {
            timer = 0.0f;
        }
        else {
            xMovement = Mathf.Sin(timer) / 2;
            yMovement = -Mathf.Sin(timer);

			bool isRunning = Input.GetKey(KeyCode.LeftShift);

            if(isRunning) {
                timer += bobbingSpeed * 1.2f;
                xMovement *= 1.5f;
                yMovement *= 1.5f;
            }
            else {
                timer += bobbingSpeed;
            }
            
            if (timer > Mathf.PI * 2) {
                timer = timer - (Mathf.PI * 2);
            }
        }

        if (xMovement != 0) {
            float translateChange = xMovement * bobbingAmount;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;

            calcPosition.x = xOffset + translateChange;
        }
        else {
            calcPosition.x = xOffset;
        }
        
        if (yMovement != 0) {
            float translateChange = yMovement * bobbingAmount;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;

            calcPosition.y = yOffset + translateChange;
        }
        else {
            calcPosition.y = yOffset;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, calcPosition, Time.deltaTime);
    }
}
