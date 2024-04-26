using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class Painter : MonoBehaviour
{
    [SerializeField] private Shader drawShader;

    [SerializeField][Range(2, 16)] private float size;
    [SerializeField][Range(1, 15)] private float hardness;
    [SerializeField][Range(0, 1)] private float strength;

    private Material drawMaterial;

    public static Painter instance = null;

    private Vector2 lastHitCoord = Vector2.zero;

    private Input input;

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

    private void Awake()
    {
        input = new Input();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        //changeBrushStroke();
    }

    public void SetBrushPreset(int preset)
    {
        drawMaterial.SetFloat("_BrushPreset", preset);
    }

    public void SetBrush(float hardness, float strength, float width, float height)
    {
        drawMaterial.SetFloat("_IsRoundBrush", 0);
        drawMaterial.SetFloat("_BrushWidth", width);
        drawMaterial.SetFloat("_BrushHeight", height);
        drawMaterial.SetFloat("_Hardness", hardness);
        drawMaterial.SetFloat("_Strength", strength);
    }

    public void PaintMask(RenderTexture mask, RaycastHit hit, bool interpolate)
    {
        //drawMaterial.SetVector("_Color", Color.white);
        drawMaterial.SetVector("_Coordinates", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));

        RenderTexture temp = RenderTexture.GetTemporary(mask.width, mask.height, 0, RenderTextureFormat.ARGBFloat);
        Graphics.Blit(mask, temp);
        Graphics.Blit(temp, mask, drawMaterial);
        RenderTexture.ReleaseTemporary(temp);

        if(interpolate)
            lastHitCoord = Interpolation(mask, temp, hit, lastHitCoord);
    }

    public void resetInterpolation()
    {
        lastHitCoord = Vector2.zero;
        //print("Reset interpolation");
    }

    public Vector2 Interpolation(RenderTexture mask, RenderTexture temp, RaycastHit hit, Vector2 lastHitCoord)
    {

        float strokeSmoothingInterval = 0.01f;
        float distance = Vector2.Distance(lastHitCoord, hit.textureCoord);
        //print(lastHitCoord);

        int numPoints = Mathf.Min((int) ((distance/strokeSmoothingInterval)*2.2f), 40);

        for (int i = 0; i < numPoints; i++)
        {
            if (lastHitCoord == Vector2.zero)
                break;

            Vector2 direction = hit.textureCoord - lastHitCoord;

            float totalDistance = direction.magnitude;
            direction.Normalize();

            Vector2 pointC = lastHitCoord + direction * i/2.2f * Mathf.Clamp(strokeSmoothingInterval, 0f, totalDistance);

            //drawMaterial.SetVector("_Color", Color.yellow);
            drawMaterial.SetVector("_Coordinates", new Vector4(pointC.x, pointC.y, 0, 0));
            temp = RenderTexture.GetTemporary(mask.width, mask.height, 0, RenderTextureFormat.ARGBFloat);
            Graphics.Blit(mask, temp);
            Graphics.Blit(temp, mask, drawMaterial);
            RenderTexture.ReleaseTemporary(temp);
        }
        return hit.textureCoord;
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

    public void changeBrushStroke()
    {
        float newSize = 0f;
        if (input.ControlesDebug.IncreaseBrush.triggered)
            newSize = Math.Min(size + 2f, 16f);
        if (input.ControlesDebug.DecreaseBrush.triggered)
            newSize = Math.Max(size - 2f, 2f);

        if (newSize != 0f)
        {
            Debug.Log(newSize);
            drawMaterial.SetFloat("_Size", newSize);
            size = newSize;
            print("brushStrokChanged - new size: " + newSize);
        }
        
    }

    public TextMeshProUGUI textBrushStatus;
    public void ChangeBrushStrokeWithButton(Slider slider)
    {
        float _newSize = slider.value;
        if (textBrushStatus != null && slider != null)
        {
            drawMaterial.SetFloat("_Size", _newSize);
            textBrushStatus.text = slider.value.ToString();
            size = _newSize;
            print("new size: " + size);
        } else
        {
            Debug.LogError("Objeto Contador do tamanho do pincel ou slider n�o encontrado!!");
        }
    }


}
