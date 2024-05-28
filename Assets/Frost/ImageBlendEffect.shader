Shader "Custom/ImageBlendEffect"
{
    Properties
    {
        _MainTex ("Base", 2D) = "" {}
        _BlendTex ("Image", 2D) = "" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _BlendAmount ("Blend Amount", Range(0, 1)) = 0.5
        _EdgeSharpness ("Edge Sharpness", Float) = 1
        _SeeThroughness ("See Throughness", Range(0, 1)) = 0.2
        _Distortion ("Distortion", Float) = 0.1
        _Center ("Effect Center", Vector) = (0.5, 0.5, 0, 0)
    }
    
    CGINCLUDE
    
    #include "UnityCG.cginc"
    
    struct v2f
    {
        float4 pos : POSITION;
        float2 uv : TEXCOORD0;
    };
    
    sampler2D _MainTex;
    sampler2D _BlendTex;
    sampler2D _BumpMap;
    
    float _BlendAmount;
    float _EdgeSharpness;
    float _SeeThroughness;
    float _Distortion;
    float4 _Center;
        
    v2f vert(appdata_img v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = v.texcoord.xy;
        return o;
    } 
    
    half4 frag(v2f i) : COLOR
{ 
    float2 uv = i.uv;
    float2 center = _Center.xy;
    
    // Calculate distance from the center
    float dx = abs(uv.x - center.x);
    float dy = abs(uv.y - center.y);
    float dist = max(dx, dy); // 사각형 모양의 거리 계산

    float4 blendColor = tex2D(_BlendTex, uv);

    // Adjust blendColor.a based on distance and EdgeSharpness
    float adjustedBlendAmount = _BlendAmount * (1.0 - smoothstep(0.0, 1.0 / _EdgeSharpness, dist));
    blendColor.a = saturate(adjustedBlendAmount * 2 - 1);
    
    // Distortion:
    half2 bump = UnpackNormal(tex2D(_BumpMap, uv)).rg;
    float4 mainColor = tex2D(_MainTex, uv + bump * blendColor.a * _Distortion);
    
    float4 overlayColor = blendColor;
    overlayColor.rgb = mainColor.rgb * (blendColor.rgb + 0.5) * (blendColor.rgb + 0.5); // double overlay
    
    blendColor = lerp(blendColor, overlayColor, _SeeThroughness);

    return lerp(mainColor, blendColor, blendColor.a);
}


    ENDCG 
    
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            Fog
            {
                Mode off
            }

            CGPROGRAM
            #pragma fragmentoption ARB_precision_hint_fastest 
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }

    Fallback off    
}
