// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.27 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.27;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:2,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4013,x:33197,y:32796,varname:node_4013,prsc:2|diff-4761-OUT;n:type:ShaderForge.SFN_Tex2d,id:6393,x:31933,y:32629,ptovrint:False,ptlb:Ground2,ptin:_Ground2,varname:node_6393,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4436,x:31933,y:32395,ptovrint:False,ptlb:Ground1,ptin:_Ground1,varname:node_4436,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:69efe6cbea36ef6458cea4e36df6fb79,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:5716,x:32187,y:32581,varname:node_5716,prsc:2|A-4436-RGB,B-6393-RGB,T-3378-R;n:type:ShaderForge.SFN_Tex2d,id:3378,x:31856,y:33252,ptovrint:False,ptlb:Masks,ptin:_Masks,varname:node_3378,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:aa762ee7680f35f4fb9a9210fa94ce61,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:5434,x:32425,y:32711,varname:node_5434,prsc:2|A-5716-OUT,B-7102-RGB,T-3378-G;n:type:ShaderForge.SFN_Tex2d,id:7102,x:32163,y:32764,ptovrint:False,ptlb:Ground3,ptin:_Ground3,varname:node_7102,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:cb54d5e817a554b4389d6ef9b504e2b2,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:3313,x:32425,y:32944,ptovrint:False,ptlb:Ground4,ptin:_Ground4,varname:node_3313,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:206b0ac501c505b46b8e981ec357895e,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:4761,x:32653,y:32864,varname:node_4761,prsc:2|A-5434-OUT,B-3313-RGB,T-3378-B;proporder:3378-4436-6393-7102-3313;pass:END;sub:END;*/

Shader "Shader Forge/SplatGround1" {
	Properties{
		_Masks("Masks", 2D) = "white" {}
		_Ground1("Ground1", 2D) = "white" {}
		_Ground2("Ground2", 2D) = "white" {}
		_Ground3("Ground3", 2D) = "white" {}
		_Ground4("Ground4", 2D) = "white" {}
	}
		SubShader{
			Tags {
				"IgnoreProjector" = "True"
				"Queue" = "Transparent"
				"RenderType" = "Transparent"
			}
			Pass {
				Name "FORWARD"
				Tags {
					"LightMode" = "ForwardBase"
				}
				Blend SrcAlpha OneMinusSrcAlpha
				ZWrite Off

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#define UNITY_PASS_FORWARDBASE
				#include "UnityCG.cginc"
				#pragma multi_compile_fwdbase
				#pragma multi_compile_fog
				#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
				#pragma target 2.0
				uniform float4 _LightColor0;
				uniform sampler2D _Ground2; uniform float4 _Ground2_ST;
				uniform sampler2D _Ground1; uniform float4 _Ground1_ST;
				uniform sampler2D _Masks; uniform float4 _Masks_ST;
				uniform sampler2D _Ground3; uniform float4 _Ground3_ST;
				uniform sampler2D _Ground4; uniform float4 _Ground4_ST;
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
					UNITY_FOG_COORDS(3)
				};
				VertexOutput vert(VertexInput v) {
					VertexOutput o = (VertexOutput)0;
					o.uv0 = v.texcoord0;
					o.normalDir = UnityObjectToWorldNormal(v.normal);
					o.posWorld = mul(unity_ObjectToWorld, v.vertex);
					float3 lightColor = _LightColor0.rgb;
					o.pos = UnityObjectToClipPos(v.vertex);
					UNITY_TRANSFER_FOG(o,o.pos);
					return o;
				}
				float4 frag(VertexOutput i) : COLOR {
					i.normalDir = normalize(i.normalDir);
					float3 normalDirection = i.normalDir;
					float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
					float3 lightColor = _LightColor0.rgb;
					////// Lighting:
									float attenuation = 1;
									float3 attenColor = attenuation * _LightColor0.xyz;
									/////// Diffuse:
													float NdotL = max(0.0,dot(normalDirection, lightDirection));
													float3 directDiffuse = max(0.0, NdotL) * attenColor;
													float3 indirectDiffuse = float3(0,0,0);
													indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
													float4 _Ground1_var = tex2D(_Ground1,TRANSFORM_TEX(i.uv0, _Ground1));
													float4 _Ground2_var = tex2D(_Ground2,TRANSFORM_TEX(i.uv0, _Ground2));
													float4 _Masks_var = tex2D(_Masks,TRANSFORM_TEX(i.uv0, _Masks));
													float4 _Ground3_var = tex2D(_Ground3,TRANSFORM_TEX(i.uv0, _Ground3));
													float4 _Ground4_var = tex2D(_Ground4,TRANSFORM_TEX(i.uv0, _Ground4));
													float3 diffuseColor = lerp(lerp(lerp(_Ground1_var.rgb,_Ground2_var.rgb,_Masks_var.r),_Ground3_var.rgb,_Masks_var.g),_Ground4_var.rgb,_Masks_var.b);
													float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
													/// Final Color:
																	float3 finalColor = diffuse;
																	fixed4 finalRGBA = fixed4(finalColor,1);
																	UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
																	return finalRGBA;
																}
																ENDCG
															}
															Pass {
																Name "FORWARD_DELTA"
																Tags {
																	"LightMode" = "ForwardAdd"
																}
																Blend One One
																ZWrite Off

																CGPROGRAM
																#pragma vertex vert
																#pragma fragment frag
																#define UNITY_PASS_FORWARDADD
																#include "UnityCG.cginc"
																#include "AutoLight.cginc"
																#pragma multi_compile_fwdadd
																#pragma multi_compile_fog
																#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
																#pragma target 2.0
																uniform float4 _LightColor0;
																uniform sampler2D _Ground2; uniform float4 _Ground2_ST;
																uniform sampler2D _Ground1; uniform float4 _Ground1_ST;
																uniform sampler2D _Masks; uniform float4 _Masks_ST;
																uniform sampler2D _Ground3; uniform float4 _Ground3_ST;
																uniform sampler2D _Ground4; uniform float4 _Ground4_ST;
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
																VertexOutput vert(VertexInput v) {
																	VertexOutput o = (VertexOutput)0;
																	o.uv0 = v.texcoord0;
																	o.normalDir = UnityObjectToWorldNormal(v.normal);
																	o.posWorld = mul(unity_ObjectToWorld, v.vertex);
																	float3 lightColor = _LightColor0.rgb;
																	o.pos = UnityObjectToClipPos(v.vertex);
																	UNITY_TRANSFER_FOG(o,o.pos);
																	TRANSFER_VERTEX_TO_FRAGMENT(o)
																	return o;
																}
																float4 frag(VertexOutput i) : COLOR {
																	i.normalDir = normalize(i.normalDir);
																	float3 normalDirection = i.normalDir;
																	float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
																	float3 lightColor = _LightColor0.rgb;
																	////// Lighting:
																					float attenuation = LIGHT_ATTENUATION(i);
																					float3 attenColor = attenuation * _LightColor0.xyz;
																					/////// Diffuse:
																									float NdotL = max(0.0,dot(normalDirection, lightDirection));
																									float3 directDiffuse = max(0.0, NdotL) * attenColor;
																									float4 _Ground1_var = tex2D(_Ground1,TRANSFORM_TEX(i.uv0, _Ground1));
																									float4 _Ground2_var = tex2D(_Ground2,TRANSFORM_TEX(i.uv0, _Ground2));
																									float4 _Masks_var = tex2D(_Masks,TRANSFORM_TEX(i.uv0, _Masks));
																									float4 _Ground3_var = tex2D(_Ground3,TRANSFORM_TEX(i.uv0, _Ground3));
																									float4 _Ground4_var = tex2D(_Ground4,TRANSFORM_TEX(i.uv0, _Ground4));
																									float3 diffuseColor = lerp(lerp(lerp(_Ground1_var.rgb,_Ground2_var.rgb,_Masks_var.r),_Ground3_var.rgb,_Masks_var.g),_Ground4_var.rgb,_Masks_var.b);
																									float3 diffuse = directDiffuse * diffuseColor;
																									/// Final Color:
																													float3 finalColor = diffuse;
																													fixed4 finalRGBA = fixed4(finalColor * 1,0);
																													UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
																													return finalRGBA;
																												}
																												ENDCG
																											}
	}
		CustomEditor "ShaderForgeMaterialInspector"
}
