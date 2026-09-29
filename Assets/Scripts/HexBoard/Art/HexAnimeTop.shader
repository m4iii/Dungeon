Shader "Dungeon/Anime Hex Top"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite silhouette",2D)="white"{}
        _ArtTex("Painted surface",2D)="white"{}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Tags { "LightMode"="Universal2D" }
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_ArtTex); SAMPLER(sampler_ArtTex);
            struct Attributes { float4 positionOS:POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; };
            struct Varyings { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; float2 artUV:TEXCOORD1; float4 color:COLOR; };
            Varyings vert(Attributes v)
            {
                Varyings o; o.positionCS=TransformObjectToHClip(v.positionOS.xyz);
                o.uv=v.uv; o.artUV=v.uv; o.color=v.color; return o;
            }
            half4 frag(Varyings i):SV_Target
            {
                half4 mask=SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv);
                half3 art=SAMPLE_TEXTURE2D(_ArtTex,sampler_ArtTex,i.artUV).rgb;
                // Keep the geometrically exact sprite outline; only its interior uses painted art.
                half3 interior=lerp(half3(.78,.79,.75),art,.6);
                return half4(interior,mask.a)*i.color;
            }
            ENDHLSL
        }
    }
}
