// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:True,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32753,y:32683,varname:node_3138,prsc:2|alpha-6525-OUT,refract-9079-OUT;n:type:ShaderForge.SFN_Tex2d,id:2600,x:32039,y:32981,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:node_2600,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:90465f31f9b152d448afc9f1d878214f,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:9079,x:32536,y:32980,varname:node_9079,prsc:2|A-6475-OUT,B-3467-OUT,T-5072-OUT;n:type:ShaderForge.SFN_Slider,id:7502,x:31960,y:33206,ptovrint:False,ptlb:Amount,ptin:_Amount,varname:node_7502,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.4155424,max:1;n:type:ShaderForge.SFN_Vector1,id:6525,x:32536,y:32881,varname:node_6525,prsc:2,v1:0;n:type:ShaderForge.SFN_Append,id:3467,x:32277,y:33018,varname:node_3467,prsc:2|A-2600-R,B-2600-G;n:type:ShaderForge.SFN_Vector1,id:6475,x:32277,y:32932,varname:node_6475,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:5072,x:32319,y:33216,varname:node_5072,prsc:2|A-7502-OUT,B-570-R;n:type:ShaderForge.SFN_VertexColor,id:570,x:32117,y:33287,varname:node_570,prsc:2;proporder:2600-7502;pass:END;sub:END;*/

Shader "Shader Forge/Distortion2" {
    Properties {
        _Normal ("Normal", 2D) = "bump" {}
        _Amount ("Amount", Range(0, 1)) = 0.4155424
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Amount;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float node_6475 = 0.0;
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + lerp(float2(node_6475,node_6475),float2(_Normal_var.r,_Normal_var.g),(_Amount*i.vertexColor.r));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
                float3 finalColor = 0;
                return fixed4(lerp(sceneColor.rgb, finalColor,0.0),1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
