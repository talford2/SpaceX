// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33063,y:32557,varname:node_9361,prsc:2|emission-9291-OUT,custl-2017-OUT;n:type:ShaderForge.SFN_LightColor,id:3406,x:31860,y:32770,varname:node_3406,prsc:2;n:type:ShaderForge.SFN_LightVector,id:6869,x:31662,y:32657,varname:node_6869,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:9684,x:31629,y:32797,prsc:2,pt:True;n:type:ShaderForge.SFN_HalfVector,id:9471,x:31687,y:32946,varname:node_9471,prsc:2;n:type:ShaderForge.SFN_Dot,id:7782,x:31910,y:32579,cmnt:Lambert,varname:node_7782,prsc:2,dt:1|A-6869-OUT,B-9684-OUT;n:type:ShaderForge.SFN_Dot,id:3269,x:31901,y:32913,cmnt:Blinn-Phong,varname:node_3269,prsc:2,dt:1|A-9684-OUT,B-9471-OUT;n:type:ShaderForge.SFN_Tex2d,id:851,x:31680,y:32291,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:node_851,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:2d44feab5f3e327448bd161f5afbb6a8,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:5927,x:31680,y:32493,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_5927,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:544,x:31963,y:32402,cmnt:Diffuse Color,varname:node_544,prsc:2|A-851-RGB,B-5927-RGB;n:type:ShaderForge.SFN_Fresnel,id:8598,x:31854,y:33100,varname:node_8598,prsc:2|EXP-2466-OUT;n:type:ShaderForge.SFN_Multiply,id:4858,x:32179,y:32971,varname:node_4858,prsc:2|A-3269-OUT,B-3739-OUT;n:type:ShaderForge.SFN_Slider,id:2466,x:31500,y:33150,ptovrint:False,ptlb:FrenelExp,ptin:_FrenelExp,varname:node_2466,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3.360862,max:5;n:type:ShaderForge.SFN_Multiply,id:3095,x:32352,y:32678,varname:node_3095,prsc:2|A-544-OUT,B-7782-OUT,C-3269-OUT,D-3406-RGB;n:type:ShaderForge.SFN_Multiply,id:1051,x:32411,y:32887,varname:node_1051,prsc:2|A-7782-OUT,B-4858-OUT;n:type:ShaderForge.SFN_Add,id:1536,x:32648,y:32597,varname:node_1536,prsc:2|A-3142-OUT,B-1051-OUT;n:type:ShaderForge.SFN_Slider,id:6233,x:32086,y:32317,ptovrint:False,ptlb:AddMultiply,ptin:_AddMultiply,varname:node_6233,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.1;n:type:ShaderForge.SFN_Multiply,id:3142,x:32463,y:32494,varname:node_3142,prsc:2|A-6233-OUT,B-9360-OUT;n:type:ShaderForge.SFN_Multiply,id:3739,x:32032,y:33182,varname:node_3739,prsc:2|A-8598-OUT,B-2210-OUT,C-3406-RGB;n:type:ShaderForge.SFN_Slider,id:2210,x:31654,y:33365,ptovrint:False,ptlb:FrenelAmount,ptin:_FrenelAmount,varname:node_2210,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2.734975,max:5;n:type:ShaderForge.SFN_Multiply,id:9291,x:32848,y:32443,varname:node_9291,prsc:2|A-4918-OUT,B-1536-OUT;n:type:ShaderForge.SFN_Multiply,id:264,x:32463,y:32318,varname:node_264,prsc:2|A-7782-OUT,B-3269-OUT;n:type:ShaderForge.SFN_OneMinus,id:4918,x:32648,y:32353,varname:node_4918,prsc:2|IN-264-OUT;n:type:ShaderForge.SFN_Power,id:2017,x:32839,y:32804,varname:node_2017,prsc:2|VAL-3095-OUT,EXP-2062-OUT;n:type:ShaderForge.SFN_Slider,id:2062,x:32491,y:32849,ptovrint:False,ptlb:LightingPow,ptin:_LightingPow,varname:node_2062,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_Power,id:9360,x:32243,y:32507,varname:node_9360,prsc:2|VAL-3406-RGB,EXP-7055-OUT;n:type:ShaderForge.SFN_Slider,id:7055,x:31901,y:32713,ptovrint:False,ptlb:AmplifyColour,ptin:_AmplifyColour,varname:node_7055,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;proporder:851-5927-2466-6233-2210-2062-7055;pass:END;sub:END;*/

Shader "Custom/Planet2" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _FrenelExp ("FrenelExp", Range(0, 5)) = 3.360862
        _AddMultiply ("AddMultiply", Range(0, 0.1)) = 0
        _FrenelAmount ("FrenelAmount", Range(0, 5)) = 2.734975
        _LightingPow ("LightingPow", Range(0, 2)) = 1
        _AmplifyColour ("AmplifyColour", Range(0, 5)) = 1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _Color;
            uniform float _FrenelExp;
            uniform float _AddMultiply;
            uniform float _FrenelAmount;
            uniform float _LightingPow;
            uniform float _AmplifyColour;
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
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
////// Emissive:
                float node_7782 = max(0,dot(lightDirection,normalDirection)); // Lambert
                float node_3269 = max(0,dot(normalDirection,halfDirection)); // Blinn-Phong
                float3 emissive = ((1.0 - (node_7782*node_3269))*((_AddMultiply*pow(_LightColor0.rgb,_AmplifyColour))+(node_7782*(node_3269*(pow(1.0-max(0,dot(normalDirection, viewDirection)),_FrenelExp)*_FrenelAmount*_LightColor0.rgb)))));
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float3 finalColor = emissive + pow(((_Diffuse_var.rgb*_Color.rgb)*node_7782*node_3269*_LightColor0.rgb),_LightingPow);
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _Color;
            uniform float _FrenelExp;
            uniform float _AddMultiply;
            uniform float _FrenelAmount;
            uniform float _LightingPow;
            uniform float _AmplifyColour;
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
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float node_7782 = max(0,dot(lightDirection,normalDirection)); // Lambert
                float node_3269 = max(0,dot(normalDirection,halfDirection)); // Blinn-Phong
                float3 finalColor = pow(((_Diffuse_var.rgb*_Color.rgb)*node_7782*node_3269*_LightColor0.rgb),_LightingPow);
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
