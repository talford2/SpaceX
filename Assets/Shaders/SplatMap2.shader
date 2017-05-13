// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.27 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.27;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:2865,x:33746,y:32785,varname:node_2865,prsc:2|diff-6763-OUT,spec-358-OUT,gloss-1813-OUT,normal-1278-OUT;n:type:ShaderForge.SFN_Slider,id:358,x:32408,y:32618,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:node_358,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:1813,x:32416,y:32709,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Metallic_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Tex2d,id:7182,x:31719,y:32544,ptovrint:False,ptlb:Ground1,ptin:_Ground1,varname:node_7182,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:69efe6cbea36ef6458cea4e36df6fb79,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:677,x:31719,y:32760,ptovrint:False,ptlb:Ground2,ptin:_Ground2,varname:node_677,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1408372681a33574d8a32ab31262b5bb,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:4200,x:31982,y:32644,varname:node_4200,prsc:2|A-7182-RGB,B-677-RGB,T-366-R;n:type:ShaderForge.SFN_Tex2d,id:366,x:31549,y:32999,ptovrint:False,ptlb:Masks,ptin:_Masks,varname:node_366,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:aa762ee7680f35f4fb9a9210fa94ce61,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1504,x:31972,y:32814,ptovrint:False,ptlb:Ground3,ptin:_Ground3,varname:node_1504,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:721a3664919b0774981cbfdbb38f6d21,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:1482,x:32164,y:32738,varname:node_1482,prsc:2|A-4200-OUT,B-1504-RGB,T-366-G;n:type:ShaderForge.SFN_Tex2d,id:5908,x:32164,y:32928,ptovrint:False,ptlb:Ground4,ptin:_Ground4,varname:node_5908,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:d9262c051e4665d45babc8ec0c813b59,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:1192,x:32368,y:32850,varname:node_1192,prsc:2|A-1482-OUT,B-5908-RGB,T-366-B;n:type:ShaderForge.SFN_Tex2d,id:3756,x:32368,y:33039,ptovrint:False,ptlb:Ground5,ptin:_Ground5,varname:node_3756,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:19555d7d9d114c7f1100f5ab44295342,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:6763,x:32573,y:32936,varname:node_6763,prsc:2|A-1192-OUT,B-3756-RGB,T-366-A;n:type:ShaderForge.SFN_Tex2d,id:9366,x:32183,y:33528,ptovrint:False,ptlb:Normal4,ptin:_Normal4,varname:node_9366,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6adfb73a2bca6744aa67b5d6fe941ad9,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:3332,x:32550,y:33455,varname:node_3332,prsc:2|A-228-OUT,B-5248-RGB,T-366-A;n:type:ShaderForge.SFN_Tex2d,id:5248,x:32380,y:33579,ptovrint:False,ptlb:Normal5,ptin:_Normal5,varname:node_5248,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:4b8d081e9d114c7f1100f5ab44295342,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:9841,x:31962,y:33482,ptovrint:False,ptlb:Normal3,ptin:_Normal3,varname:node_9841,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e08c295755c0885479ad19f518286ff2,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:228,x:32380,y:33347,varname:node_228,prsc:2|A-6407-OUT,B-9366-RGB,T-366-B;n:type:ShaderForge.SFN_Tex2d,id:7034,x:31750,y:33421,ptovrint:False,ptlb:Normal2,ptin:_Normal2,varname:node_7034,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:bbab0a6f7bae9cf42bf057d8ee2755f6,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:7025,x:31727,y:33214,ptovrint:False,ptlb:Normal1,ptin:_Normal1,varname:node_7025,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:5a7c97fb78198b84cbac7f9680338e91,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:6407,x:32183,y:33316,varname:node_6407,prsc:2|A-151-OUT,B-9841-RGB,T-366-G;n:type:ShaderForge.SFN_Lerp,id:151,x:31962,y:33278,varname:node_151,prsc:2|A-7025-RGB,B-7034-RGB,T-366-R;n:type:ShaderForge.SFN_Lerp,id:1278,x:33377,y:33450,varname:node_1278,prsc:2|A-1746-OUT,B-1581-OUT,T-6942-OUT;n:type:ShaderForge.SFN_Vector3,id:1581,x:32631,y:33628,varname:node_1581,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:6942,x:32852,y:33785,ptovrint:False,ptlb:NormalStrength,ptin:_NormalStrength,varname:node_6942,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Tex2d,id:862,x:32820,y:33073,ptovrint:False,ptlb:GeneralNoraml,ptin:_GeneralNoraml,varname:node_862,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e08c295755c0885479ad19f518286ff2,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:1746,x:33079,y:33268,varname:node_1746,prsc:2|A-862-RGB,B-3332-OUT,T-3428-OUT;n:type:ShaderForge.SFN_Slider,id:3428,x:32571,y:33287,ptovrint:False,ptlb:GeneralNormalStrength,ptin:_GeneralNormalStrength,varname:node_3428,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5544945,max:1;proporder:358-1813-6942-366-7182-7025-677-7034-1504-9841-5908-9366-3756-5248-862-3428;pass:END;sub:END;*/

Shader "Custom/SplatMap" {
    Properties {
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0
        _NormalStrength ("NormalStrength", Range(0, 1)) = 0
        _Masks ("Masks", 2D) = "white" {}
        _Ground1 ("Ground1", 2D) = "white" {}
        _Normal1 ("Normal1", 2D) = "bump" {}
        _Ground2 ("Ground2", 2D) = "white" {}
        _Normal2 ("Normal2", 2D) = "bump" {}
        _Ground3 ("Ground3", 2D) = "white" {}
        _Normal3 ("Normal3", 2D) = "bump" {}
        _Ground4 ("Ground4", 2D) = "white" {}
        _Normal4 ("Normal4", 2D) = "bump" {}
        _Ground5 ("Ground5", 2D) = "white" {}
        _Normal5 ("Normal5", 2D) = "bump" {}
        _GeneralNoraml ("GeneralNoraml", 2D) = "bump" {}
        _GeneralNormalStrength ("GeneralNormalStrength", Range(0, 1)) = 0.5544945
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
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float _Metallic;
            uniform float _Gloss;
            uniform sampler2D _Ground1; uniform float4 _Ground1_ST;
            uniform sampler2D _Ground2; uniform float4 _Ground2_ST;
            uniform sampler2D _Masks; uniform float4 _Masks_ST;
            uniform sampler2D _Ground3; uniform float4 _Ground3_ST;
            uniform sampler2D _Ground4; uniform float4 _Ground4_ST;
            uniform sampler2D _Ground5; uniform float4 _Ground5_ST;
            uniform sampler2D _Normal4; uniform float4 _Normal4_ST;
            uniform sampler2D _Normal5; uniform float4 _Normal5_ST;
            uniform sampler2D _Normal3; uniform float4 _Normal3_ST;
            uniform sampler2D _Normal2; uniform float4 _Normal2_ST;
            uniform sampler2D _Normal1; uniform float4 _Normal1_ST;
            uniform float _NormalStrength;
            uniform sampler2D _GeneralNoraml; uniform float4 _GeneralNoraml_ST;
            uniform float _GeneralNormalStrength;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _GeneralNoraml_var = UnpackNormal(tex2D(_GeneralNoraml,TRANSFORM_TEX(i.uv0, _GeneralNoraml)));
                float3 _Normal1_var = UnpackNormal(tex2D(_Normal1,TRANSFORM_TEX(i.uv0, _Normal1)));
                float3 _Normal2_var = UnpackNormal(tex2D(_Normal2,TRANSFORM_TEX(i.uv0, _Normal2)));
                float4 _Masks_var = tex2D(_Masks,TRANSFORM_TEX(i.uv0, _Masks));
                float3 _Normal3_var = UnpackNormal(tex2D(_Normal3,TRANSFORM_TEX(i.uv0, _Normal3)));
                float3 _Normal4_var = UnpackNormal(tex2D(_Normal4,TRANSFORM_TEX(i.uv0, _Normal4)));
                float3 _Normal5_var = UnpackNormal(tex2D(_Normal5,TRANSFORM_TEX(i.uv0, _Normal5)));
                float3 normalLocal = lerp(lerp(_GeneralNoraml_var.rgb,lerp(lerp(lerp(lerp(_Normal1_var.rgb,_Normal2_var.rgb,_Masks_var.r),_Normal3_var.rgb,_Masks_var.g),_Normal4_var.rgb,_Masks_var.b),_Normal5_var.rgb,_Masks_var.a),_GeneralNormalStrength),float3(0,0,1),_NormalStrength);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                d.boxMax[0] = unity_SpecCube0_BoxMax;
                d.boxMin[0] = unity_SpecCube0_BoxMin;
                d.probePosition[0] = unity_SpecCube0_ProbePosition;
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.boxMax[1] = unity_SpecCube1_BoxMax;
                d.boxMin[1] = unity_SpecCube1_BoxMin;
                d.probePosition[1] = unity_SpecCube1_ProbePosition;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float4 _Ground1_var = tex2D(_Ground1,TRANSFORM_TEX(i.uv0, _Ground1));
                float4 _Ground2_var = tex2D(_Ground2,TRANSFORM_TEX(i.uv0, _Ground2));
                float4 _Ground3_var = tex2D(_Ground3,TRANSFORM_TEX(i.uv0, _Ground3));
                float4 _Ground4_var = tex2D(_Ground4,TRANSFORM_TEX(i.uv0, _Ground4));
                float4 _Ground5_var = tex2D(_Ground5,TRANSFORM_TEX(i.uv0, _Ground5));
                float3 diffuseColor = lerp(lerp(lerp(lerp(_Ground1_var.rgb,_Ground2_var.rgb,_Masks_var.r),_Ground3_var.rgb,_Masks_var.g),_Ground4_var.rgb,_Masks_var.b),_Ground5_var.rgb,_Masks_var.a); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, GGXTerm(NdotH, 1.0-gloss));
                float specularPBL = (NdotL*visTerm*normTerm) * (UNITY_PI / 4);
                if (IsGammaSpace())
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                specularPBL = max(0, specularPBL * NdotL);
                float3 directSpecular = 1*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
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
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float _Metallic;
            uniform float _Gloss;
            uniform sampler2D _Ground1; uniform float4 _Ground1_ST;
            uniform sampler2D _Ground2; uniform float4 _Ground2_ST;
            uniform sampler2D _Masks; uniform float4 _Masks_ST;
            uniform sampler2D _Ground3; uniform float4 _Ground3_ST;
            uniform sampler2D _Ground4; uniform float4 _Ground4_ST;
            uniform sampler2D _Ground5; uniform float4 _Ground5_ST;
            uniform sampler2D _Normal4; uniform float4 _Normal4_ST;
            uniform sampler2D _Normal5; uniform float4 _Normal5_ST;
            uniform sampler2D _Normal3; uniform float4 _Normal3_ST;
            uniform sampler2D _Normal2; uniform float4 _Normal2_ST;
            uniform sampler2D _Normal1; uniform float4 _Normal1_ST;
            uniform float _NormalStrength;
            uniform sampler2D _GeneralNoraml; uniform float4 _GeneralNoraml_ST;
            uniform float _GeneralNormalStrength;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _GeneralNoraml_var = UnpackNormal(tex2D(_GeneralNoraml,TRANSFORM_TEX(i.uv0, _GeneralNoraml)));
                float3 _Normal1_var = UnpackNormal(tex2D(_Normal1,TRANSFORM_TEX(i.uv0, _Normal1)));
                float3 _Normal2_var = UnpackNormal(tex2D(_Normal2,TRANSFORM_TEX(i.uv0, _Normal2)));
                float4 _Masks_var = tex2D(_Masks,TRANSFORM_TEX(i.uv0, _Masks));
                float3 _Normal3_var = UnpackNormal(tex2D(_Normal3,TRANSFORM_TEX(i.uv0, _Normal3)));
                float3 _Normal4_var = UnpackNormal(tex2D(_Normal4,TRANSFORM_TEX(i.uv0, _Normal4)));
                float3 _Normal5_var = UnpackNormal(tex2D(_Normal5,TRANSFORM_TEX(i.uv0, _Normal5)));
                float3 normalLocal = lerp(lerp(_GeneralNoraml_var.rgb,lerp(lerp(lerp(lerp(_Normal1_var.rgb,_Normal2_var.rgb,_Masks_var.r),_Normal3_var.rgb,_Masks_var.g),_Normal4_var.rgb,_Masks_var.b),_Normal5_var.rgb,_Masks_var.a),_GeneralNormalStrength),float3(0,0,1),_NormalStrength);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float4 _Ground1_var = tex2D(_Ground1,TRANSFORM_TEX(i.uv0, _Ground1));
                float4 _Ground2_var = tex2D(_Ground2,TRANSFORM_TEX(i.uv0, _Ground2));
                float4 _Ground3_var = tex2D(_Ground3,TRANSFORM_TEX(i.uv0, _Ground3));
                float4 _Ground4_var = tex2D(_Ground4,TRANSFORM_TEX(i.uv0, _Ground4));
                float4 _Ground5_var = tex2D(_Ground5,TRANSFORM_TEX(i.uv0, _Ground5));
                float3 diffuseColor = lerp(lerp(lerp(lerp(_Ground1_var.rgb,_Ground2_var.rgb,_Masks_var.r),_Ground3_var.rgb,_Masks_var.g),_Ground4_var.rgb,_Masks_var.b),_Ground5_var.rgb,_Masks_var.a); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, GGXTerm(NdotH, 1.0-gloss));
                float specularPBL = (NdotL*visTerm*normTerm) * (UNITY_PI / 4);
                if (IsGammaSpace())
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                specularPBL = max(0, specularPBL * NdotL);
                float3 directSpecular = attenColor*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float _Metallic;
            uniform float _Gloss;
            uniform sampler2D _Ground1; uniform float4 _Ground1_ST;
            uniform sampler2D _Ground2; uniform float4 _Ground2_ST;
            uniform sampler2D _Masks; uniform float4 _Masks_ST;
            uniform sampler2D _Ground3; uniform float4 _Ground3_ST;
            uniform sampler2D _Ground4; uniform float4 _Ground4_ST;
            uniform sampler2D _Ground5; uniform float4 _Ground5_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float4 _Ground1_var = tex2D(_Ground1,TRANSFORM_TEX(i.uv0, _Ground1));
                float4 _Ground2_var = tex2D(_Ground2,TRANSFORM_TEX(i.uv0, _Ground2));
                float4 _Masks_var = tex2D(_Masks,TRANSFORM_TEX(i.uv0, _Masks));
                float4 _Ground3_var = tex2D(_Ground3,TRANSFORM_TEX(i.uv0, _Ground3));
                float4 _Ground4_var = tex2D(_Ground4,TRANSFORM_TEX(i.uv0, _Ground4));
                float4 _Ground5_var = tex2D(_Ground5,TRANSFORM_TEX(i.uv0, _Ground5));
                float3 diffColor = lerp(lerp(lerp(lerp(_Ground1_var.rgb,_Ground2_var.rgb,_Masks_var.r),_Ground3_var.rgb,_Masks_var.g),_Ground4_var.rgb,_Masks_var.b),_Ground5_var.rgb,_Masks_var.a);
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _Metallic, specColor, specularMonochrome );
                float roughness = 1.0 - _Gloss;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
