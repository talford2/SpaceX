// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33403,y:32638,varname:node_3138,prsc:2|emission-7241-RGB,clip-4918-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32817,y:32590,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Tex2d,id:2570,x:31859,y:32573,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_2570,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:87b8c7f9ed7ac4a46833878253775d36,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3184,x:32466,y:32668,varname:node_3184,prsc:2|A-7658-OUT,B-7995-OUT,C-1233-OUT;n:type:ShaderForge.SFN_Slider,id:7995,x:31811,y:32835,ptovrint:False,ptlb:Amount,ptin:_Amount,varname:node_7995,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.896186,max:1;n:type:ShaderForge.SFN_Vector1,id:1233,x:32257,y:32830,varname:node_1233,prsc:2,v1:1;n:type:ShaderForge.SFN_Color,id:3321,x:32623,y:32513,ptovrint:False,ptlb:node_3321,ptin:_node_3321,varname:node_3321,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Clamp01,id:6636,x:32817,y:32855,varname:node_6636,prsc:2|IN-6035-OUT;n:type:ShaderForge.SFN_Add,id:7658,x:32257,y:32590,varname:node_7658,prsc:2|A-2570-RGB,B-8564-OUT;n:type:ShaderForge.SFN_Vector1,id:8564,x:32065,y:32624,varname:node_8564,prsc:2,v1:1;n:type:ShaderForge.SFN_Trunc,id:6035,x:32641,y:32781,varname:node_6035,prsc:2|IN-3184-OUT;n:type:ShaderForge.SFN_ComponentMask,id:7045,x:33000,y:32893,varname:node_7045,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-6636-OUT;n:type:ShaderForge.SFN_Multiply,id:4918,x:33168,y:32825,varname:node_4918,prsc:2|A-2570-A,B-7045-OUT;proporder:7241-2570-7995;pass:END;sub:END;*/

Shader "Custom/ProgressBar" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Mask ("Mask", 2D) = "white" {}
        _Amount ("Amount", Range(0, 1)) = 0.896186
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _Amount;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float3 node_3184 = ((_Mask_var.rgb+1.0)*_Amount*1.0);
                float3 node_6636 = saturate(trunc(node_3184));
                clip((_Mask_var.a*node_6636.r) - 0.5);
////// Lighting:
////// Emissive:
                float3 emissive = _Color.rgb;
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _Amount;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float3 node_3184 = ((_Mask_var.rgb+1.0)*_Amount*1.0);
                float3 node_6636 = saturate(trunc(node_3184));
                clip((_Mask_var.a*node_6636.r) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
