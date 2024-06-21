using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionController : MonoBehaviour
{
    [Header("Atributos da projeção")]
    [Tooltip("- para o usuário (em metros)")]
    [Range(0.3f, 5f)][SerializeField] private float distaciaDaCamera;
    [Tooltip("(em metros)")]
    [Range(0.5f, 1.5f)][SerializeField] private float AlturaDaCamera;

    [Header("Componentes")]
    public Transform virtualCamera;
    public Transform TrackCameraOffset;
    public Transform ARCamera;
    //public Transform Tools;

    private bool setVideo = false;

    public void Start()
    {
        float distancia = Mathf.Abs(virtualCamera.localPosition.z) - distaciaDaCamera;
        TrackCameraOffset.localPosition = new Vector3(TrackCameraOffset.localPosition.x, AlturaDaCamera, distaciaDaCamera * (-1));
    }

    public void Update()
    {
        if (!setVideo && ARCamera.childCount > 0){
            Transform firstChild = ARCamera.GetChild(0);
            firstChild.gameObject.layer = LayerMask.NameToLayer("video");
            setVideo = true;
        }
    }
}
