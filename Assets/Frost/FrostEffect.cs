using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Frost")]
public class FrostEffect : MonoBehaviour
{
    public float FrostAmount = 0.5f; // 0-1 (0=minimum Frost, 1=maximum frost)
    public float EdgeSharpness = 1; // >=1
    public float minFrost = 0; // 0-1
    public float maxFrost = 1; // 0-1
    public float seethroughness = 0.2f; // blends between 2 ways of applying the frost effect: 0=normal blend mode, 1="overlay" blend mode
    public float distortion = 0.1f; // how much the original image is distorted through the frost (value depends on normal map)
    public Texture2D Frost; // RGBA
    public Texture2D FrostNormals; // normalmap
    public Shader Shader; // ImageBlendEffect.shader

    public GameMgr gameMgr;

    private Material material;
    private bool isEffectActive = false;

    private void Awake()
    {
        material = new Material(Shader);
        material.SetTexture("_BlendTex", Frost);
        material.SetTexture("_BumpMap", FrostNormals);
    }

    private void Start()
    {
        // Start the effect automatically when the scene starts

    }

    private void Update()
    {
        if (gameMgr.isStopAttack)
        {
            if (!isEffectActive)
            {
                StartCoroutine(ActivateFrostEffect());
            }
        }
        else if(!gameMgr.isStopAttack)
        {
            //StopAllCoroutines();
        }
    }

    private IEnumerator ActivateFrostEffect()
    {
        isEffectActive = true;

        // Frost growing from outer to inner over 0.8 seconds
        float growDuration = 0.8f;
        float elapsedTime = 0.0f;
        while (elapsedTime < growDuration)
        {
            FrostAmount = Mathf.Lerp(0, 2, elapsedTime / growDuration); // 2배까지 퍼지도록 수정
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        FrostAmount = 2;

        // Hold the effect for 2 seconds
        yield return new WaitForSeconds(0.8f);

        // Frost dissolving from inner to outer over 0.8 seconds
        float dissolveDuration = 0.8f;
        elapsedTime = 0.0f;
        while (elapsedTime < dissolveDuration)
        {
            FrostAmount = Mathf.Lerp(2, 0, elapsedTime / dissolveDuration); // 완전히 사라질 때까지 퍼지도록 수정
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        FrostAmount = 0;

        isEffectActive = false;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!Application.isPlaying)
        {
            material.SetTexture("_BlendTex", Frost);
            material.SetTexture("_BumpMap", FrostNormals);
            EdgeSharpness = Mathf.Max(1, EdgeSharpness);
        }
        material.SetFloat("_BlendAmount", Mathf.Clamp01(FrostAmount * (maxFrost - minFrost) + minFrost));
        material.SetFloat("_EdgeSharpness", EdgeSharpness);
        material.SetFloat("_SeeThroughness", seethroughness);
        material.SetFloat("_Distortion", distortion);

        // Calculate the center of the screen
        Vector2 center = new Vector2(0.5f, 0.5f);
        material.SetVector("_Center", new Vector4(center.x, center.y, 0, 0));

        Graphics.Blit(source, destination, material);
    }
}
