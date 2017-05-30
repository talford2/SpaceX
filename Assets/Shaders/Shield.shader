// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:True,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:True,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:33257,y:32355,varname:node_4795,prsc:2|emission-4807-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:32115,y:32464,varname:_MainTex,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:3,isnm:False|UVIN-1625-UVOUT,TEX-304-TEX;n:type:ShaderForge.SFN_Color,id:797,x:32390,y:32300,ptovrint:True,ptlb:Color1,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5882353,c2:0.8977687,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:754,x:31816,y:32669,varname:node_754,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-8045-UVOUT,TEX-304-TEX;n:type:ShaderForge.SFN_Time,id:5075,x:31050,y:32639,varname:node_5075,prsc:2;n:type:ShaderForge.SFN_Panner,id:1625,x:31816,y:32421,varname:node_1625,prsc:2,spu:1,spv:1|UVIN-3622-OUT,DIST-4333-OUT;n:type:ShaderForge.SFN_TexCoord,id:1976,x:31266,y:32345,varname:node_1976,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:8045,x:31560,y:32608,varname:node_8045,prsc:2,spu:1,spv:-1|UVIN-1976-UVOUT,DIST-4333-OUT;n:type:ShaderForge.SFN_Slider,id:9245,x:30848,y:32481,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_9245,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5463246,max:1;n:type:ShaderForge.SFN_Multiply,id:4333,x:31349,y:32577,varname:node_4333,prsc:2|A-9245-OUT,B-5075-TSL;n:type:ShaderForge.SFN_OneMinus,id:893,x:32115,y:32602,varname:node_893,prsc:2|IN-754-RGB;n:type:ShaderForge.SFN_Tex2dAsset,id:304,x:31494,y:32139,ptovrint:False,ptlb:node_304,ptin:_node_304,varname:node_304,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_OneMinus,id:3622,x:31552,y:32355,varname:node_3622,prsc:2|IN-1976-UVOUT;n:type:ShaderForge.SFN_Multiply,id:2773,x:32361,y:32533,varname:node_2773,prsc:2|A-6074-RGB,B-893-OUT;n:type:ShaderForge.SFN_Multiply,id:4807,x:32754,y:32405,varname:node_4807,prsc:2|A-797-RGB,B-2773-OUT;n:type:ShaderForge.SFN_SceneColor,id:7990,x:32886,y:32256,varname:node_7990,prsc:2|UVIN-437-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:437,x:32597,y:32146,varname:node_437,prsc:2,uv:0,uaff:False;proporder:797-9245-304;pass:END;sub:END;*/

Shader "Custom/Shield" {
    Properties {
        _TintColor ("Color1", Color) = (0.5882353,0.8977687,1,1)
        _Speed ("Speed", Range(0, 1)) = 0.5463246
        _node_304 ("node_304", 2D) = "white" {}
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
            
            Stencil {
                Ref 128
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _TintColor;
            uniform float _Speed;
            uniform sampler2D _node_304; uniform float4 _node_304_ST;
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
                float4 node_5075 = _Time + _TimeEditor;
                float node_4333 = (_Speed*node_5075.r);
                float2 node_1625 = ((1.0 - i.uv0)+node_4333*float2(1,1));
                float4 _MainTex = tex2D(_node_304,TRANSFORM_TEX(node_1625, _node_304));
                float2 node_8045 = (i.uv0+node_4333*float2(1,-1));
                float4 node_754 = tex2D(_node_304,TRANSFORM_TEX(node_8045, _node_304));
                float3 emissive = (_TintColor.rgb*(_MainTex.rgb*(1.0 - node_754.rgb)));
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