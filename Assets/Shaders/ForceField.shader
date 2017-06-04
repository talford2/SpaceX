// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33225,y:32660,varname:node_3138,prsc:2|emission-6483-OUT,voffset-9970-OUT;n:type:ShaderForge.SFN_DepthBlend,id:9533,x:31941,y:32890,varname:node_9533,prsc:2|DIST-4294-OUT;n:type:ShaderForge.SFN_Slider,id:4294,x:31560,y:32952,ptovrint:False,ptlb:node_4294,ptin:_node_4294,varname:node_4294,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_OneMinus,id:8702,x:32164,y:32924,varname:node_8702,prsc:2|IN-9533-OUT;n:type:ShaderForge.SFN_Add,id:7740,x:32354,y:32893,varname:node_7740,prsc:2|A-1314-OUT,B-8702-OUT;n:type:ShaderForge.SFN_Slider,id:1314,x:31941,y:32772,ptovrint:False,ptlb:node_1314,ptin:_node_1314,varname:node_1314,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Time,id:6930,x:31833,y:33425,varname:node_6930,prsc:2;n:type:ShaderForge.SFN_Sin,id:4933,x:32371,y:33324,varname:node_4933,prsc:2|IN-8050-OUT;n:type:ShaderForge.SFN_Multiply,id:2671,x:32576,y:33388,varname:node_2671,prsc:2|A-4933-OUT,B-5157-OUT;n:type:ShaderForge.SFN_NormalVector,id:5157,x:32356,y:33474,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:9970,x:32762,y:33246,varname:node_9970,prsc:2|A-7730-OUT,B-2671-OUT;n:type:ShaderForge.SFN_Slider,id:7730,x:32386,y:33216,ptovrint:False,ptlb:PulseAmount,ptin:_PulseAmount,varname:node_7730,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.08676486,max:1;n:type:ShaderForge.SFN_Multiply,id:8050,x:32101,y:33306,varname:node_8050,prsc:2|A-3327-OUT,B-6930-T;n:type:ShaderForge.SFN_Slider,id:3327,x:31709,y:33285,ptovrint:False,ptlb:PulseSpeed,ptin:_PulseSpeed,varname:node_3327,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.663813,max:10;n:type:ShaderForge.SFN_Fresnel,id:6649,x:32355,y:32616,varname:node_6649,prsc:2|EXP-9394-OUT;n:type:ShaderForge.SFN_Add,id:2949,x:32611,y:32794,varname:node_2949,prsc:2|A-6649-OUT,B-7740-OUT;n:type:ShaderForge.SFN_Slider,id:9394,x:31964,y:32578,ptovrint:False,ptlb:node_9394,ptin:_node_9394,varname:node_9394,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:3;n:type:ShaderForge.SFN_Tex2d,id:9636,x:32611,y:32996,ptovrint:False,ptlb:node_9636,ptin:_node_9636,varname:node_9636,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:2983,x:32611,y:32576,ptovrint:False,ptlb:node_2983,ptin:_node_2983,varname:node_2983,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:6483,x:32906,y:32695,varname:node_6483,prsc:2|A-2983-RGB,B-2949-OUT,C-9636-RGB;proporder:4294-1314-7730-3327-9394-9636-2983;pass:END;sub:END;*/

Shader "Custom/ForceField" {
    Properties {
        _node_4294 ("node_4294", Range(0, 1)) = 0
        _node_1314 ("node_1314", Range(0, 1)) = 0
        _PulseAmount ("PulseAmount", Range(0, 1)) = 0.08676486
        _PulseSpeed ("PulseSpeed", Range(0, 10)) = 1.663813
        _node_9394 ("node_9394", Range(0, 3)) = 1
        _node_9636 ("node_9636", 2D) = "white" {}
        _node_2983 ("node_2983", Color) = (0.5,0.5,0.5,1)
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
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform float _node_4294;
            uniform float _node_1314;
            uniform float _PulseAmount;
            uniform float _PulseSpeed;
            uniform float _node_9394;
            uniform sampler2D _node_9636; uniform float4 _node_9636_ST;
            uniform float4 _node_2983;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 projPos : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_6930 = _Time + _TimeEditor;
                v.vertex.xyz += (_PulseAmount*(sin((_PulseSpeed*node_6930.g))*v.normal));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
////// Lighting:
////// Emissive:
                float4 _node_9636_var = tex2D(_node_9636,TRANSFORM_TEX(i.uv0, _node_9636));
                float3 emissive = (_node_2983.rgb*(pow(1.0-max(0,dot(normalDirection, viewDirection)),_node_9394)+(_node_1314+(1.0 - saturate((sceneZ-partZ)/_node_4294))))*_node_9636_var.rgb);
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
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles n3ds wiiu 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float _PulseAmount;
            uniform float _PulseSpeed;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_6930 = _Time + _TimeEditor;
                v.vertex.xyz += (_PulseAmount*(sin((_PulseSpeed*node_6930.g))*v.normal));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
