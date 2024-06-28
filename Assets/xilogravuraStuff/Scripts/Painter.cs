using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public enum Brush { HardCircle, HardSquare, SoftSquare, Ink }

public class Painter : MonoBehaviour
{
    [SerializeField] private Shader drawShader;

    [Header("Brush")]
    [SerializeField][Range(2, 16)] private float size;

    [Header("Max distance from object")]
    public float maxDistance = 0.7f;
    public float maxDistanceGlass = 0.3f;

    [Header("Objects and controllers")]
    public GrabController grabController;
    public MenuController menuController;
    [SerializeField] private Camera cam;
    [SerializeField] public ExperienceMode mode;
    private bool verifSound = true;
    private bool verifSoundWood = false;
    private bool verifSoundGlass = false;
    private Vector2 lastHitAngle = Vector2.zero;

    private Material drawMaterial;

    public static Painter instance = null;

    private Vector2 lastHitCoord = Vector2.zero;

    private Input input;

    private bool raycastAnterior = false;

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
        mode.getMode();
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

    public void SetBrushPreset(Brush preset)
    {
        drawMaterial.SetFloat("_BrushPreset", (int) preset);
    }
    public void SetBrushPreset(int preset)
    {
        drawMaterial.SetFloat("_BrushPreset", preset);
    }

    public RaycastHit? CheckDraw(GameObject tool, int layerMask, bool prevStep, bool nextStep, ParticleSystem particles){
        RaycastHit hit;
        if (mode.mode == Mode.VR && !grabController.isGrab(tool))
            return null;
        if (prevStep && !nextStep){
            Vector3 pointerPosition = getPointerPosition(tool);
            bool condicaoDePintura = mode.condicaoDePintura();
            Ray ray = cam.ScreenPointToRay(pointerPosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) 
            && (condicaoDePintura || click())){
                if (mode.mode == Mode.PROJECTION && !mode.GetComponent<ProjectionMode>().checkTool(tool)){
                    raycastAnterior = false;
                    return null;
                }
                float distance = Vector3.Distance(tool.transform.position, hit.transform.position);
                //print(distance);
                bool frontRaycast = layerMask == 1 << LayerMask.NameToLayer("wood") 
                                    || layerMask == 1 << LayerMask.NameToLayer("paper") 
                                    || layerMask == 1 << LayerMask.NameToLayer("newArt");
                if ((distance > maxDistance) && frontRaycast || (distance > maxDistanceGlass) && !frontRaycast){
                    disableActionTool(layerMask, tool, particles);
                    menuController.enableTextIndicator(true);
                    return null;
                }
                menuController.enableTextIndicator(false);

                //excecao para angulo da goiva
                if (tool.name.Equals("goiva") && !checkAngle(hit, 0.85f))
                    return null;

                checkLayer(layerMask, true);
                initSound(tool);
                if(particles != null)
                    instanciarParticulas(particles, hit.point);

                raycastAnterior = true;
                return hit;
            }
            else
            {
                if (mode.mode == Mode.VR || (mode.mode == Mode.PROJECTION && !raycastAnterior))
                    disableActionTool(layerMask, tool, particles);
                raycastAnterior = false;
            }
                
        }
        return null;
    }

    private void disableActionTool(int layerMask, GameObject tool, ParticleSystem particles)
    {
        checkLayer(layerMask, false);
        stopSound(tool);
        resetInterpolation();
        if (particles != null)
            desligarParticulas(particles);
    }

    private bool checkAngle(RaycastHit hit, float angle)
    {
        Vector2 direction = new Vector2(1, 0);
        if (lastHitAngle != Vector2.zero){
            direction = hit.textureCoord - lastHitAngle;
            direction.Normalize();
        }
        lastHitAngle = hit.textureCoord;
        return direction.y >= angle;
    }

    private void checkLayer(int layer, bool state)
    {
        if (layer == 1 << LayerMask.NameToLayer("wood") 
            || layer == 1 << LayerMask.NameToLayer("paper")
            || layer == 1 << LayerMask.NameToLayer("newArt"))
            verifSoundWood = state;
        else
            verifSoundGlass = state;
    }

    public void setVerifSound(bool value)
    {
        verifSound = value;
    }

    public void initSound(GameObject ferramenta)
    {
        if (verifSound){
            ferramenta.gameObject.GetComponent<AudioSource>().Play();
            verifSound = false;
        }
    }

    public void stopSound(GameObject ferramenta)
    {
        if (ferramenta.gameObject.GetComponent<AudioSource>().isPlaying && !verifSoundWood && !verifSoundGlass){
            ferramenta.gameObject.GetComponent<AudioSource>().Stop();
            verifSound = true;
        }
    }

    private Vector3 getPointerPosition(GameObject tool)
    {
        return cam.WorldToScreenPoint(isToolInteraction(tool));
    }

    private bool click(){
        return Mouse.current.leftButton.isPressed;
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
    }

    public Vector2 Interpolation(RenderTexture mask, RenderTexture temp, RaycastHit hit, Vector2 lastHitCoord)
    {
        float strokeSmoothingInterval = 0.01f;
        float distance = Vector2.Distance(lastHitCoord, hit.textureCoord);

        int numPoints = Mathf.Min((int) ((distance/strokeSmoothingInterval)*2.2f), 40);

        for (int i = 0; i < numPoints; i++){
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

    public Vector3 isToolInteraction(GameObject ferramenta)
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
        //rayLeftHand.startColor = Color.blue;
        //rayRightHand.startColor = Color.blue;
        if (textBrushStatus != null && slider != null)
        {
            drawMaterial.SetFloat("_Size", _newSize);
            textBrushStatus.text = slider.value.ToString();
            size = _newSize;
            print("new size: " + size);
        }
        else
        {
            Debug.LogError("Objeto Contador do tamanho do pincel ou slider n�o encontrado!!");
        }
    }
}
