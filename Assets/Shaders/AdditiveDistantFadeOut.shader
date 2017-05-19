// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:33125,y:32736,varname:node_4795,prsc:2|emission-2393-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:32156,y:33023,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:18afb273f119b0e4ebfdca893582593b,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2393,x:32912,y:32864,varname:node_2393,prsc:2|A-9248-OUT,B-6927-OUT,C-9088-OUT;n:type:ShaderForge.SFN_Color,id:797,x:32472,y:32690,ptovrint:True,ptlb:Black,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Vector1,id:9248,x:32701,y:32819,varname:node_9248,prsc:2,v1:2;n:type:ShaderForge.SFN_Clamp01,id:8517,x:32495,y:33101,varname:node_8517,prsc:2|IN-9796-OUT;n:type:ShaderForge.SFN_Distance,id:3729,x:32090,y:33213,varname:node_3729,prsc:2|A-7147-XYZ,B-7353-XYZ;n:type:ShaderForge.SFN_FragmentPosition,id:7147,x:31876,y:33129,varname:node_7147,prsc:2;n:type:ShaderForge.SFN_ViewPosition,id:7353,x:31876,y:33272,varname:node_7353,prsc:2;n:type:ShaderForge.SFN_OneMinus,id:9088,x:32701,y:33022,varname:node_9088,prsc:2|IN-8517-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2065,x:32090,y:33383,ptovrint:False,ptlb:Fade Distance,ptin:_FadeDistance,varname:node_2065,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:100;n:type:ShaderForge.SFN_Divide,id:9796,x:32294,y:33213,varname:node_9796,prsc:2|A-3729-OUT,B-2065-OUT;n:type:ShaderForge.SFN_Color,id:4156,x:32156,y:32656,ptovrint:False,ptlb:Grey,ptin:_Grey,varname:node_4156,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5019608,c2:0.5019608,c3:0.5019608,c4:0.5019608;n:type:ShaderForge.SFN_Color,id:5394,x:32156,y:32840,ptovrint:False,ptlb:White,ptin:_White,varname:node_5394,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Lerp,id:9557,x:32472,y:32837,varname:node_9557,prsc:2|A-4156-RGB,B-5394-RGB,T-6074-R;n:type:ShaderForge.SFN_Lerp,id:6927,x:32701,y:32885,varname:node_6927,prsc:2|A-797-RGB,B-9557-OUT,T-6074-R;proporder:6074-5394-4156-797-2065;pass:END;sub:END;*/

Shader "Effects/AdditiveDistantFadeOut" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _White ("White", Color) = (1,1,1,1)
        _Grey ("Grey", Color) = (0.5019608,0.5019608,0.5019608,0.5019608)
        _TintColor ("Black", Color) = (0,0,0,1)
        _FadeDistance ("Fade Distance", Float ) = 100
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform float _FadeDistance;
            uniform float4 _Grey;
            uniform float4 _White;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = (2.0*lerp(_TintColor.rgb,lerp(_Grey.rgb,_White.rgb,_MainTex_var.r),_MainTex_var.r)*(1.0 - saturate((distance(i.posWorld.rgb,_WorldSpaceCameraPos)/_FadeDistance))));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
