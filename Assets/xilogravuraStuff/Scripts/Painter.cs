using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Painter : MonoBehaviour
{
    [SerializeField] private Shader drawShader;

    [SerializeField][Range(0, 40)] private float size;
    [SerializeField][Range(1, 15)] private float hardness;
    [SerializeField][Range(0, 1)] private float strength;

    private Material drawMaterial;

    public static Painter instance = null;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        drawMaterial = new Material(drawShader);
        drawMaterial.SetVector("_Color", Color.white);
    }

    void Update()
    {
    }

    public void SetBrush(float hardness, float strength, float size)
    {
        drawMaterial.SetFloat("_IsRoundBrush", 1);
        drawMaterial.SetFloat("_Size", size);
        drawMaterial.SetFloat("_Hardness", hardness);
        drawMaterial.SetFloat("_Strength", strength);
    }

    public void SetBrush(float hardness, float strength, float width, float height)
    {
        drawMaterial.SetFloat("_IsRoundBrush", 0);
        drawMaterial.SetFloat("_BrushWidth", width);
        drawMaterial.SetFloat("_BrushHeight", height);
        drawMaterial.SetFloat("_Hardness", hardness);
        drawMaterial.SetFloat("_Strength", strength);
    }

    public void PaintMask(RenderTexture mask, RaycastHit hit)
    {
        //Debug.Log("pintando");
        drawMaterial.SetVector("_Coordinates", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));

        RenderTexture temp = RenderTexture.GetTemporary(mask.width, mask.height, 0, RenderTextureFormat.ARGBFloat);
        Graphics.Blit(mask, temp);
        Graphics.Blit(temp, mask, drawMaterial);
        RenderTexture.ReleaseTemporary(temp); 
    }

    public Vector3 isToolInteraction(XRGrabInteractable ferramenta)
    {
        Transform transform = ferramenta.GetComponent<Transform>();
        Transform point = transform.GetChild(0);

        return point.position;
    }

    public void instanciarParticulas(ParticleSystem particulas, Vector3 point)
    {
        particulas.gameObject.SetActive(true);
        particulas.transform.position = point;
        particulas.Play();
    }

    public void desligarParticulas(ParticleSystem particulas)
    {
        particulas.Pause();
        particulas.gameObject.SetActive(false);
    }
}
