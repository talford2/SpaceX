// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:32724,y:32693,varname:node_4795,prsc:2|alpha-5169-OUT,refract-4988-OUT;n:type:ShaderForge.SFN_Tex2d,id:2472,x:31916,y:32859,varname:node_2472,prsc:2,tex:e08c295755c0885479ad19f518286ff2,ntxv:3,isnm:True|UVIN-4375-UVOUT,TEX-5698-TEX;n:type:ShaderForge.SFN_Multiply,id:4988,x:32524,y:33012,varname:node_4988,prsc:2|A-2566-OUT,B-2051-OUT,C-2426-R;n:type:ShaderForge.SFN_Slider,id:2051,x:32130,y:33138,ptovrint:False,ptlb:Strength,ptin:_Strength,varname:node_2051,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.3;n:type:ShaderForge.SFN_Tex2d,id:2426,x:31992,y:32654,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_2426,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0f6e04a3ae0182c48b2aaed3d5206a98,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Time,id:7142,x:31201,y:32926,varname:node_7142,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2017,x:31446,y:32874,varname:node_2017,prsc:2|A-4642-OUT,B-7142-TSL;n:type:ShaderForge.SFN_Slider,id:4642,x:31055,y:32827,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_4642,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Panner,id:4375,x:31679,y:32708,varname:node_4375,prsc:2,spu:-1,spv:-1|UVIN-429-OUT,DIST-2017-OUT;n:type:ShaderForge.SFN_TexCoord,id:6168,x:31276,y:32660,varname:node_6168,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2dAsset,id:5698,x:31514,y:33082,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:node_5698,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e08c295755c0885479ad19f518286ff2,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:6941,x:31916,y:32987,varname:node_6941,prsc:2,tex:e08c295755c0885479ad19f518286ff2,ntxv:0,isnm:False|UVIN-8596-UVOUT,TEX-5698-TEX;n:type:ShaderForge.SFN_Panner,id:8596,x:31702,y:32924,varname:node_8596,prsc:2,spu:-1,spv:-1|UVIN-6168-UVOUT,DIST-2017-OUT;n:type:ShaderForge.SFN_Lerp,id:8335,x:32089,y:32955,varname:node_8335,prsc:2|A-2472-RGB,B-6941-RGB,T-7835-OUT;n:type:ShaderForge.SFN_Vector1,id:7835,x:31916,y:33122,varname:node_7835,prsc:2,v1:0.5;n:type:ShaderForge.SFN_ComponentMask,id:2566,x:32287,y:32924,varname:node_2566,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-8335-OUT;n:type:ShaderForge.SFN_OneMinus,id:429,x:31484,y:32650,varname:node_429,prsc:2|IN-6168-UVOUT;n:type:ShaderForge.SFN_Vector1,id:5169,x:32478,y:32877,varname:node_5169,prsc:2,v1:0;proporder:2051-2426-4642-5698;pass:END;sub:END;*/

Shader "Shader Forge/HeatDistortion" {
    Properties {
        _Strength ("Strength", Range(0, 0.3)) = 0
        _Mask ("Mask", 2D) = "white" {}
        _Speed ("Speed", Range(0, 10)) = 1
        _Normal ("Normal", 2D) = "bump" {}
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
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform float _Strength;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _Speed;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
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
                float4 node_7142 = _Time + _TimeEditor;
                float node_2017 = (_Speed*node_7142.r);
                float2 node_4375 = ((1.0 - i.uv0)+node_2017*float2(-1,-1));
                float3 node_2472 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_4375, _Normal)));
                float2 node_8596 = (i.uv0+node_2017*float2(-1,-1));
                float3 node_6941 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_8596, _Normal)));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (lerp(node_2472.rgb,node_6941.rgb,0.5).rg*_Strength*_Mask_var.r);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
                float3 finalColor = 0;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,0.0),1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
