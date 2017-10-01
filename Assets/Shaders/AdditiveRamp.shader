// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:32724,y:32693,varname:node_4795,prsc:2|emission-2393-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31493,y:32714,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2393,x:32497,y:32781,varname:node_2393,prsc:2|A-6225-RGB,B-2053-RGB,C-1272-OUT,D-3267-RGB;n:type:ShaderForge.SFN_VertexColor,id:2053,x:32231,y:32850,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:6225,x:32231,y:32678,ptovrint:False,ptlb:Ramp,ptin:_Ramp,varname:node_6225,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6875-OUT;n:type:ShaderForge.SFN_Append,id:5807,x:31700,y:32664,varname:node_5807,prsc:2|A-6074-R,B-6074-R;n:type:ShaderForge.SFN_OneMinus,id:6838,x:31871,y:32630,varname:node_6838,prsc:2|IN-5807-OUT;n:type:ShaderForge.SFN_Clamp01,id:6875,x:32057,y:32644,varname:node_6875,prsc:2|IN-6838-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:7095,x:31495,y:32989,varname:node_7095,prsc:2;n:type:ShaderForge.SFN_ViewPosition,id:664,x:31495,y:33128,varname:node_664,prsc:2;n:type:ShaderForge.SFN_Distance,id:8029,x:31697,y:33058,varname:node_8029,prsc:2|A-7095-XYZ,B-664-XYZ;n:type:ShaderForge.SFN_Divide,id:900,x:31877,y:33058,varname:node_900,prsc:2|A-8029-OUT,B-6553-OUT;n:type:ShaderForge.SFN_Clamp01,id:3083,x:32054,y:33033,varname:node_3083,prsc:2|IN-900-OUT;n:type:ShaderForge.SFN_OneMinus,id:1272,x:32231,y:32993,varname:node_1272,prsc:2|IN-3083-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6553,x:31697,y:33223,ptovrint:False,ptlb:FadeDistance,ptin:_FadeDistance,varname:node_6553,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:500;n:type:ShaderForge.SFN_Color,id:3267,x:32231,y:33173,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_3267,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;proporder:6074-6225-6553-3267;pass:END;sub:END;*/

Shader "Custom/AdditiveRamp" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Ramp ("Ramp", 2D) = "white" {}
        _FadeDistance ("FadeDistance", Float ) = 500
        [HDR]_Color ("Color", Color) = (1,1,1,1)
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
            Cull Off
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
            uniform sampler2D _Ramp; uniform float4 _Ramp_ST;
            uniform float _FadeDistance;
            uniform float4 _Color;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float2 node_6875 = saturate((1.0 - float2(_MainTex_var.r,_MainTex_var.r)));
                float4 _Ramp_var = tex2D(_Ramp,TRANSFORM_TEX(node_6875, _Ramp));
                float3 emissive = (_Ramp_var.rgb*i.vertexColor.rgb*(1.0 - saturate((distance(i.posWorld.rgb,_WorldSpaceCameraPos)/_FadeDistance)))*_Color.rgb);
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
