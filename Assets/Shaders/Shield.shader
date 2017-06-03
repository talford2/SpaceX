// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:True,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:True,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:33408,y:32340,varname:node_4795,prsc:2|emission-3246-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31955,y:32476,varname:_MainTex,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:3,isnm:False|UVIN-1625-UVOUT,TEX-304-TEX;n:type:ShaderForge.SFN_Tex2d,id:754,x:31955,y:32624,varname:node_754,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-8045-UVOUT,TEX-304-TEX;n:type:ShaderForge.SFN_Time,id:5075,x:30871,y:32688,varname:node_5075,prsc:2;n:type:ShaderForge.SFN_Panner,id:1625,x:31714,y:32498,varname:node_1625,prsc:2,spu:1,spv:1|UVIN-3949-OUT,DIST-4333-OUT;n:type:ShaderForge.SFN_TexCoord,id:1976,x:31258,y:32476,varname:node_1976,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:8045,x:31714,y:32659,varname:node_8045,prsc:2,spu:-1,spv:-1|UVIN-1976-UVOUT,DIST-4333-OUT;n:type:ShaderForge.SFN_Slider,id:9245,x:30816,y:32563,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_9245,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5463246,max:3;n:type:ShaderForge.SFN_Multiply,id:4333,x:31299,y:32684,varname:node_4333,prsc:2|A-9245-OUT,B-5075-TSL;n:type:ShaderForge.SFN_Tex2dAsset,id:304,x:31438,y:32168,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_304,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3949,x:31497,y:32395,varname:node_3949,prsc:2|A-4385-OUT,B-1976-UVOUT;n:type:ShaderForge.SFN_Vector1,id:4385,x:31145,y:32308,varname:node_4385,prsc:2,v1:0.8;n:type:ShaderForge.SFN_Color,id:6698,x:32619,y:32326,ptovrint:False,ptlb:Color2,ptin:_Color2,varname:node_6698,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:4225,x:32619,y:32521,ptovrint:False,ptlb:node_4225,ptin:_node_4225,varname:node_4225,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8c1fa8dd80a37c84ca3c5c0675be9a0e,ntxv:2,isnm:False|UVIN-9824-OUT;n:type:ShaderForge.SFN_Append,id:9824,x:32410,y:32552,varname:node_9824,prsc:2|A-5687-OUT,B-5687-OUT;n:type:ShaderForge.SFN_Add,id:5687,x:32191,y:32564,varname:node_5687,prsc:2|A-6074-R,B-754-R;n:type:ShaderForge.SFN_Multiply,id:3350,x:32880,y:32408,varname:node_3350,prsc:2|A-6698-RGB,B-4225-RGB;n:type:ShaderForge.SFN_DepthBlend,id:9374,x:32422,y:32797,varname:node_9374,prsc:2|DIST-3486-OUT;n:type:ShaderForge.SFN_Add,id:3246,x:33121,y:32529,varname:node_3246,prsc:2|A-3350-OUT,B-4739-OUT;n:type:ShaderForge.SFN_Slider,id:3486,x:31998,y:32905,ptovrint:False,ptlb:node_3486,ptin:_node_3486,varname:node_3486,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2473489,max:100;n:type:ShaderForge.SFN_OneMinus,id:1205,x:32639,y:32710,varname:node_1205,prsc:2|IN-9374-OUT;n:type:ShaderForge.SFN_Multiply,id:4739,x:32868,y:32595,varname:node_4739,prsc:2|A-6698-RGB,B-1205-OUT;proporder:6698-9245-304-4225-3486;pass:END;sub:END;*/

Shader "Custom/Shield" {
    Properties {
        _Color2 ("Color2", Color) = (1,1,1,1)
        _Speed ("Speed", Range(0, 3)) = 0.5463246
        _Texture ("Texture", 2D) = "white" {}
        _node_4225 ("node_4225", 2D) = "black" {}
        _node_3486 ("node_3486", Range(0, 100)) = 0.2473489
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
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform float _Speed;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float4 _Color2;
            uniform sampler2D _node_4225; uniform float4 _node_4225_ST;
            uniform float _node_3486;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 projPos : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
////// Lighting:
////// Emissive:
                float4 node_5075 = _Time + _TimeEditor;
                float node_4333 = (_Speed*node_5075.r);
                float2 node_1625 = ((0.8*i.uv0)+node_4333*float2(1,1));
                float4 _MainTex = tex2D(_Texture,TRANSFORM_TEX(node_1625, _Texture));
                float2 node_8045 = (i.uv0+node_4333*float2(-1,-1));
                float4 node_754 = tex2D(_Texture,TRANSFORM_TEX(node_8045, _Texture));
                float node_5687 = (_MainTex.r+node_754.r);
                float2 node_9824 = float2(node_5687,node_5687);
                float4 _node_4225_var = tex2D(_node_4225,TRANSFORM_TEX(node_9824, _node_4225));
                float3 emissive = ((_Color2.rgb*_node_4225_var.rgb)+(_Color2.rgb*(1.0 - saturate((sceneZ-partZ)/_node_3486))));
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
