Shader "Hidden/Dungeon/Map Art Composition"
{
    Properties { _MainTex("Art",2D)="white"{} _Rock("Rock",Float)=0 _Lake("Lake",Float)=0 _River("River",Float)=0 _Hex("Hex",Float)=1 }
    SubShader {
        Tags { "Queue"="Transparent" }
        Pass {
            Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float _Rock,_Lake,_River,_Hex,_Angle,_Dry,_CutoutHex;
            float4 _Cell,_Region;
            float4 _DrawRect;
            float2 _CanvasSize;
            float4 _MouthA,_MouthB;
            float2 AlignMouth(float2 p,float4 mouth)
            {
                float2 normal=float2(cos(mouth.x),sin(mouth.x));
                float2 tangent=float2(-normal.y,normal.x);
                float along=dot(p,tangent);
                float blend=smoothstep(.55,.85,dot(p,normal))*(1-smoothstep(.25,.45,abs(along)))*mouth.w;
                return p+tangent*(mouth.y+along*(mouth.z-1))*blend;
            }
            fixed4 frag(v2f_img i):SV_Target {
                float2 uv=i.uv;
                float2 screenPoint=_DrawRect.xy+float2(uv.x,1-uv.y)*_DrawRect.zw;
                clip(screenPoint);clip(_CanvasSize-screenPoint);
                float mask=1;
                if(_CutoutHex>.5) {
                    float2 ground=float2(uv.x*2-1,uv.y*3-1);
                    clip(1-abs(ground.y)-abs(ground.x)*.5);
                }
                if(_River>.5) {
                    float2 p=(uv-.5)*2;
                    if(_Hex>.5)clip(1.001-abs(p.y)-abs(p.x)*.5);
                    float factor=_Hex>.5?.8660254:1;
                    p.x*=factor;
                    float s=sin(_Angle),c=cos(_Angle);
                    p=float2(c*p.x-s*p.y,s*p.x+c*p.y);
                    p=AlignMouth(AlignMouth(p,_MouthA),_MouthB);
                    p.x/=factor;
                    uv=float2(.5+p.x*.5,1.0/3+p.y/3);
                    mask=step(0,uv.x)*step(uv.x,1)*step(0,uv.y)*step(uv.y,1);
                }
                if(_Lake>.5) {
                    float2 p=float2(uv.x*2-1,1-uv.y*2);
                    mask=_Hex>.5?1-smoothstep(.99,1,abs(p.y)+abs(p.x)*.5):1;
                    uv=(_Cell.xy+p-_Region.xy)/_Region.zw;
                    mask*=step(0,uv.x)*step(uv.x,1)*step(0,uv.y)*step(uv.y,1);
                    uv.y=1-uv.y;
                }
                fixed4 c=tex2D(_MainTex,uv);
                if(_Rock>.5) {
                    float vegetation=smoothstep(.015,.15,c.g-c.b)*smoothstep(-.015,.07,c.g-c.r);
                    float light=dot(c.rgb,float3(.3,.5,.2));
                    c.rgb=lerp(c.rgb,light*float3(.91,.87,.76),vegetation*.83);
                }
                if(_Dry>.5) {
                    float luminance=dot(c.rgb,float3(.3,.5,.2));
                    c.rgb=luminance*float3(1.12,.84,.48);
                }
                #ifndef UNITY_COLORSPACE_GAMMA
                c.rgb=LinearToGammaSpace(c.rgb);
                #endif
                c.a*=mask;clip(c.a-.01);return c;
            }
            ENDCG
        }
    }
}
