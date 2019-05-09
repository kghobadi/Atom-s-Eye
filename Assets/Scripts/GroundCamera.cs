using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCamera : MonoBehaviour
{
    GameObject character;
    Transform player;
    FirstPersonController fpc;
    Vector2 mouseLook;
    Vector2 smoothV;
    Camera mainCam;

    public float sensitivityX;
    public float sensitivityY;
    public float smoothing = 2.0f;

    public float normalFOV, zoomedFOV, lerpSpeed;

    void Start()
    {
        character = transform.parent.gameObject;
        player = transform.parent;
        fpc = player.GetComponent<FirstPersonController>();
        mainCam = GetComponent<Camera>();
    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;

        var newRotate = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        newRotate = Vector2.Scale(newRotate, new Vector2(sensitivityX * smoothing, sensitivityY * smoothing));
        smoothV.x = Mathf.Lerp(smoothV.x, newRotate.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, newRotate.y, 1f / smoothing);
        mouseLook += smoothV;

        mouseLook.y = Mathf.Clamp(mouseLook.y, -90f, 90f);

        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);

        //right click to zoom view
        if (Input.GetMouseButton(1))
        {
            //lerp in
            if (mainCam.fieldOfView > zoomedFOV)
            {
                mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, zoomedFOV, lerpSpeed * Time.deltaTime);
            }
        }
        //not holding right click
        else
        {
            //lerp out
            if (mainCam.fieldOfView < normalFOV)
            {
                mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, normalFOV, lerpSpeed * Time.deltaTime);
            }
        }
    }
    
    public void HeadBob()
    {

    }

}
