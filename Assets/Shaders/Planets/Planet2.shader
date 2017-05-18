// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:32871,y:32543,varname:node_9361,prsc:2|emission-1536-OUT,custl-3095-OUT;n:type:ShaderForge.SFN_LightColor,id:3406,x:31936,y:32767,varname:node_3406,prsc:2;n:type:ShaderForge.SFN_LightVector,id:6869,x:31662,y:32657,varname:node_6869,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:9684,x:31629,y:32797,prsc:2,pt:True;n:type:ShaderForge.SFN_HalfVector,id:9471,x:31687,y:32946,varname:node_9471,prsc:2;n:type:ShaderForge.SFN_Dot,id:7782,x:31936,y:32611,cmnt:Lambert,varname:node_7782,prsc:2,dt:1|A-6869-OUT,B-9684-OUT;n:type:ShaderForge.SFN_Dot,id:3269,x:31901,y:32913,cmnt:Blinn-Phong,varname:node_3269,prsc:2,dt:1|A-9684-OUT,B-9471-OUT;n:type:ShaderForge.SFN_Tex2d,id:851,x:31680,y:32291,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:node_851,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:2d44feab5f3e327448bd161f5afbb6a8,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:5927,x:31680,y:32493,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_5927,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:544,x:31963,y:32402,cmnt:Diffuse Color,varname:node_544,prsc:2|A-851-RGB,B-5927-RGB;n:type:ShaderForge.SFN_Fresnel,id:8598,x:31854,y:33100,varname:node_8598,prsc:2|EXP-2466-OUT;n:type:ShaderForge.SFN_Multiply,id:4858,x:32179,y:32971,varname:node_4858,prsc:2|A-3269-OUT,B-3739-OUT;n:type:ShaderForge.SFN_Slider,id:2466,x:31500,y:33150,ptovrint:False,ptlb:FrenelExp,ptin:_FrenelExp,varname:node_2466,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2.630652,max:5;n:type:ShaderForge.SFN_Multiply,id:3095,x:32250,y:32640,varname:node_3095,prsc:2|A-544-OUT,B-7782-OUT,C-3269-OUT,D-3406-RGB;n:type:ShaderForge.SFN_Multiply,id:1051,x:32411,y:32887,varname:node_1051,prsc:2|A-7782-OUT,B-4858-OUT;n:type:ShaderForge.SFN_Add,id:1536,x:32648,y:32597,varname:node_1536,prsc:2|A-3142-OUT,B-1051-OUT;n:type:ShaderForge.SFN_Slider,id:6233,x:32177,y:32386,ptovrint:False,ptlb:AddMultiply,ptin:_AddMultiply,varname:node_6233,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.1;n:type:ShaderForge.SFN_Multiply,id:3142,x:32561,y:32439,varname:node_3142,prsc:2|A-6233-OUT,B-3406-RGB;n:type:ShaderForge.SFN_Multiply,id:3739,x:32032,y:33182,varname:node_3739,prsc:2|A-8598-OUT,B-2210-OUT,C-3406-RGB;n:type:ShaderForge.SFN_Slider,id:2210,x:31654,y:33365,ptovrint:False,ptlb:FrenelAmount,ptin:_FrenelAmount,varname:node_2210,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;proporder:851-5927-2466-6233-2210;pass:END;sub:END;*/

Shader "Custom/Planet2" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _FrenelExp ("FrenelExp", Range(0, 5)) = 2.630652
        _AddMultiply ("AddMultiply", Range(0, 0.1)) = 0
        _FrenelAmount ("FrenelAmount", Range(0, 5)) = 1
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
                float node_8598 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_FrenelExp);
                float3 node_4858 = (node_3269*(node_8598*_FrenelAmount*_LightColor0.rgb));
                float3 emissive = ((_AddMultiply*_LightColor0.rgb)+(node_7782*node_4858));
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float3 node_544 = (_Diffuse_var.rgb*_Color.rgb); // Diffuse Color
                float3 finalColor = emissive + (node_544*node_7782*node_3269*_LightColor0.rgb);
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
                float3 node_544 = (_Diffuse_var.rgb*_Color.rgb); // Diffuse Color
                float node_7782 = max(0,dot(lightDirection,normalDirection)); // Lambert
                float node_3269 = max(0,dot(normalDirection,halfDirection)); // Blinn-Phong
                float3 finalColor = (node_544*node_7782*node_3269*_LightColor0.rgb);
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
