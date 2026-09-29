Shader "Dungeon/Hex Card Back"
{
    Properties
    {
        [PerRendererData] _MainTex("Hex silhouette",2D)="white"{}
        _Monster("Monster token",Float)=0
        _PaperTex("Antique cartography",2D)="white"{}
        _PaperSpan("Map texture span",Float)=12
        [PerRendererData] _MapOrigin("Resting map origin",Vector)=(0,0,0,0)
        [PerRendererData] _RevealEdgesA("Explored neighbors A",Vector)=(0,0,0,0)
        [PerRendererData] _RevealEdgesB("Explored neighbors B",Vector)=(0,0,0,0)
        [PerRendererData] _Explored("Explored terrain overlay",Float)=0
        [PerRendererData] _FrontierWidth("Parchment frontier width",Float)=.32
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
            TEXTURE2D(_PaperTex); SAMPLER(sampler_PaperTex);
            float _PaperSpan;
            float _Monster;
            float4 _MapOrigin;
            float4 _RevealEdgesA, _RevealEdgesB;
            float _Explored, _FrontierWidth;
            struct A { float4 positionOS:POSITION;float2 uv:TEXCOORD0;float4 color:COLOR; };
            struct V { float4 positionCS:SV_POSITION;float2 uv:TEXCOORD0;float4 color:COLOR; };
            V vert(A v){V o;o.positionCS=TransformObjectToHClip(v.positionOS.xyz);o.uv=v.uv;o.color=v.color;return o;}
            float hash(float2 p){return frac(sin(dot(p,float2(127.1,311.7)))*43758.5453);}
            float noise(float2 p)
            {
                float2 cell=floor(p),f=frac(p);f=f*f*(3-2*f);
                return lerp(lerp(hash(cell),hash(cell+float2(1,0)),f.x),
                    lerp(hash(cell+float2(0,1)),hash(cell+1),f.x),f.y);
            }
            half4 frag(V i):SV_Target
            {
                float2 p=(i.uv-.5)*2.56;
                float radius=length(p);
                float aa=max(fwidth(radius),.006);
                float2 map=p+_MapOrigin.xy;
                half3 color=SAMPLE_TEXTURE2D(_PaperTex,sampler_PaperTex,map / _PaperSpan + .5).rgb;
                float4 distances=float4((1-p.y+.5*p.x)/1.118,1+p.x,(1+p.y+.5*p.x)/1.118,(1+p.y-.5*p.x)/1.118);
                float2 more=float2(1-p.x,(1-p.y-.5*p.x)/1.118);
                float alpha=1;
                if(_Explored>.5)
                {
                    // Union of the adjoining unknown regions. A single map-space
                    // field makes the contour continue across tile corners.
                    float4 d=lerp(float4(100,100,100,100),distances,_RevealEdgesA);
                    float2 e=lerp(float2(100,100),more,_RevealEdgesB.xy);
                    float frontier=min(min(min(d.x,d.y),min(d.z,d.w)),min(e.x,e.y));
                    float grain=noise(map*19);
                    float contour=_FrontierWidth*(.38+.85*noise(map*3.1)+.24*noise(map*8.7));
                    float signedEdge=frontier-contour;
                    float feather=max(fwidth(signedEdge)*1.5,.018);
                    float paper=1-smoothstep(-feather,feather,signedEdge);
                    // A wider translucent ochre wash bridges saturated grass and
                    // opaque parchment, with no dark outline around each hex.
                    float wash=(1-smoothstep(-.02,.16+grain*.06,signedEdge))*.46;
                    alpha=max(paper,wash)*step(.001,_FrontierWidth);
                    half3 ochre=color*half3(.80,.73,.56);
                    color=lerp(ochre,color,paper);
                }
                if(_Monster>.5)
                {
                    float head=1-smoothstep(.37,.37+aa,length(p-float2(0,.1)));
                    float jaw=(1-smoothstep(.24,.24+aa,abs(p.x)))*(1-smoothstep(.24,.24+aa,abs(p.y+.2)));
                    float eyes=1-smoothstep(.095,.095+aa,min(length(p-float2(-.15,.12)),length(p-float2(.15,.12))));
                    float nose=1-smoothstep(.065,.065+aa,length(p-float2(0,-.08)));
                    color=lerp(half3(.33,.09,.09),half3(.87,.80,.64),saturate(max(head,jaw)-eyes-nose));
                }
                return half4(color,SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv).a*alpha)*i.color;
            }
            ENDHLSL
        }
    }
}


