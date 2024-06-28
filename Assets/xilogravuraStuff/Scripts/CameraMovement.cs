using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public ProjectionMode projectionMode;
    public InkRollerController inkRollerController;
    private bool cameraDown = false;
    private float initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        Transform tool = projectionMode.getTool();
        float startPos = transform.position.y;
        if (tool != null && tool.name.Equals("tinta") && !cameraDown)
            StartCoroutine(MoveToPosition(startPos, 1.15f, true));
        if(inkRollerController.isInkEnable() && cameraDown)
            StartCoroutine(MoveToPosition(startPos, initialPosition, false));
    }

    IEnumerator MoveToPosition(float StartPosition, float EndPosition, bool status)
    {
        cameraDown = status;
        float duration = .5f;
        float elapsedTime = 0.0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(transform.position.x, EndPosition, transform.position.z);
        startPos.y = StartPosition;

        while (elapsedTime < duration){
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
    }
}
