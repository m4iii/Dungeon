Shader "Dungeon/Trial Terrain Surface"
{
    Properties
    {
        [PerRendererData] _MainTex("Hex mask",2D)="white"{}
        _ArtTex("Ground texture",2D)="white"{}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Tags { "LightMode"="Universal2D" }
            Cull Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_ArtTex); SAMPLER(sampler_ArtTex);
            struct A { float4 positionOS:POSITION; float2 uv:TEXCOORD0; };
            struct V { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; float2 art:TEXCOORD1; };
            // Preserve the complete original ground hex, including its painted edge.
            // The source is 256x384; the ground occupies its bottom 256 pixels.
            V vert(A v) { V o; o.positionCS=TransformObjectToHClip(v.positionOS.xyz); o.uv=v.uv; o.art=(v.uv-.5)*float2(1.28,.853333333)+float2(.5,.333333333); return o; }
            half4 frag(V i):SV_Target
            {
                half4 mask=SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv);
                half4 art=SAMPLE_TEXTURE2D(_ArtTex,sampler_ArtTex,i.art);
                return half4(art.rgb,art.a*mask.a);
            }
            ENDHLSL
        }
    }
}


