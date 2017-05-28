// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:33076,y:32342,varname:node_4795,prsc:2|emission-290-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31987,y:32349,ptovrint:False,ptlb:Tex2,ptin:_Tex2,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:3,isnm:False|UVIN-1625-UVOUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:32587,y:32673,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:32343,y:32214,ptovrint:True,ptlb:Color1,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5882353,c2:0.8977687,c3:1,c4:1;n:type:ShaderForge.SFN_Vector1,id:9248,x:32235,y:33081,varname:node_9248,prsc:2,v1:2;n:type:ShaderForge.SFN_Tex2d,id:754,x:31841,y:32625,ptovrint:False,ptlb:Tex1,ptin:_Tex1,varname:node_754,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-8045-UVOUT;n:type:ShaderForge.SFN_Time,id:5075,x:31226,y:32622,varname:node_5075,prsc:2;n:type:ShaderForge.SFN_Panner,id:1625,x:31746,y:32372,varname:node_1625,prsc:2,spu:0.33,spv:0.66|UVIN-1976-UVOUT,DIST-4333-OUT;n:type:ShaderForge.SFN_TexCoord,id:1976,x:31433,y:32390,varname:node_1976,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:8045,x:31663,y:32554,varname:node_8045,prsc:2,spu:0.88,spv:0.11|UVIN-1976-UVOUT,DIST-4333-OUT;n:type:ShaderForge.SFN_Slider,id:9245,x:31069,y:32514,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_9245,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1,max:1;n:type:ShaderForge.SFN_Multiply,id:4333,x:31472,y:32554,varname:node_4333,prsc:2|A-9245-OUT,B-5075-T;n:type:ShaderForge.SFN_Add,id:7541,x:32198,y:32514,varname:node_7541,prsc:2|A-6074-RGB,B-893-OUT;n:type:ShaderForge.SFN_Clamp01,id:4361,x:32391,y:32563,varname:node_4361,prsc:2|IN-7541-OUT;n:type:ShaderForge.SFN_Lerp,id:7206,x:32587,y:32505,varname:node_7206,prsc:2|A-797-RGB,B-2772-RGB,T-4361-OUT;n:type:ShaderForge.SFN_Color,id:2772,x:32331,y:32385,ptovrint:False,ptlb:Color2,ptin:_Color2,varname:node_2772,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.4554386,c2:0.1332721,c3:0.625,c4:1;n:type:ShaderForge.SFN_OneMinus,id:893,x:32010,y:32592,varname:node_893,prsc:2|IN-754-RGB;n:type:ShaderForge.SFN_Multiply,id:290,x:32805,y:32540,varname:node_290,prsc:2|A-7206-OUT,B-2053-RGB;proporder:754-6074-797-2772-9245;pass:END;sub:END;*/

Shader "Custom/Shield" {
    Properties {
        _Tex1 ("Tex1", 2D) = "white" {}
        _Tex2 ("Tex2", 2D) = "bump" {}
        _TintColor ("Color1", Color) = (0.5882353,0.8977687,1,1)
        _Color2 ("Color2", Color) = (0.4554386,0.1332721,0.625,1)
        _Speed ("Speed", Range(0, 1)) = 0.1
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
            uniform float4 _TimeEditor;
            uniform sampler2D _Tex2; uniform float4 _Tex2_ST;
            uniform float4 _TintColor;
            uniform sampler2D _Tex1; uniform float4 _Tex1_ST;
            uniform float _Speed;
            uniform float4 _Color2;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_5075 = _Time + _TimeEditor;
                float node_4333 = (_Speed*node_5075.g);
                float2 node_1625 = (i.uv0+node_4333*float2(0.33,0.66));
                float4 _Tex2_var = tex2D(_Tex2,TRANSFORM_TEX(node_1625, _Tex2));
                float2 node_8045 = (i.uv0+node_4333*float2(0.88,0.11));
                float4 _Tex1_var = tex2D(_Tex1,TRANSFORM_TEX(node_8045, _Tex1));
                float3 emissive = (lerp(_TintColor.rgb,_Color2.rgb,saturate((_Tex2_var.rgb+(1.0 - _Tex1_var.rgb))))*i.vertexColor.rgb);
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
