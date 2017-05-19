// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:32906,y:32686,varname:node_4795,prsc:2|emission-6996-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31908,y:33122,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:028c34018f655b143b2c41563ee2b405,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:4464,x:32478,y:32793,varname:node_4464,prsc:2|A-6956-OUT,B-4974-OUT,T-6074-RGB;n:type:ShaderForge.SFN_Color,id:7463,x:31757,y:32570,ptovrint:False,ptlb:Black,ptin:_Black,varname:node_7463,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:8246,x:31730,y:32985,ptovrint:False,ptlb:White,ptin:_White,varname:node_8246,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:5292,x:31745,y:32768,ptovrint:False,ptlb:Grey,ptin:_Grey,varname:node_5292,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5019608,c2:0.5019608,c3:0.5019608,c4:0.5019608;n:type:ShaderForge.SFN_Lerp,id:4974,x:32197,y:32832,varname:node_4974,prsc:2|A-4683-OUT,B-6233-OUT,T-6074-RGB;n:type:ShaderForge.SFN_Vector1,id:3159,x:32456,y:32683,varname:node_3159,prsc:2,v1:2;n:type:ShaderForge.SFN_Multiply,id:6996,x:32668,y:32721,varname:node_6996,prsc:2|A-3159-OUT,B-4464-OUT;n:type:ShaderForge.SFN_Multiply,id:4683,x:31962,y:32757,varname:node_4683,prsc:2|A-5292-RGB,B-5292-A;n:type:ShaderForge.SFN_Multiply,id:6956,x:31962,y:32593,varname:node_6956,prsc:2|A-7463-RGB,B-7463-A;n:type:ShaderForge.SFN_Multiply,id:6233,x:31962,y:32932,varname:node_6233,prsc:2|A-8246-RGB,B-8246-A;proporder:6074-8246-5292-7463;pass:END;sub:END;*/

Shader "Custom/AdditiveGradientBlend" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _White ("White", Color) = (1,1,1,1)
        _Grey ("Grey", Color) = (0.5019608,0.5019608,0.5019608,0.5019608)
        _Black ("Black", Color) = (0,0,0,1)
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
            uniform float4 _Black;
            uniform float4 _White;
            uniform float4 _Grey;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = (2.0*lerp((_Black.rgb*_Black.a),lerp((_Grey.rgb*_Grey.a),(_White.rgb*_White.a),_MainTex_var.rgb),_MainTex_var.rgb));
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
