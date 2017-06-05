// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:32807,y:32679,varname:node_4795,prsc:2|emission-8943-RGB;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31559,y:32822,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:163b5ab4892dfe94db5d28e651e1f2d8,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8943,x:32569,y:32740,ptovrint:False,ptlb:Ramp,ptin:_Ramp,varname:node_8943,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a434ede996774fc4da5779683bcce7b9,ntxv:0,isnm:False|UVIN-469-OUT;n:type:ShaderForge.SFN_Slider,id:7960,x:31224,y:32638,ptovrint:False,ptlb:BurnAmount,ptin:_BurnAmount,varname:node_7960,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:1521,x:31784,y:32677,varname:node_1521,prsc:2|A-3824-OUT,B-6074-R;n:type:ShaderForge.SFN_Add,id:2453,x:31985,y:32751,varname:node_2453,prsc:2|A-1521-OUT,B-6074-G;n:type:ShaderForge.SFN_Append,id:469,x:32384,y:32704,varname:node_469,prsc:2|A-9414-OUT,B-9414-OUT;n:type:ShaderForge.SFN_OneMinus,id:9414,x:32177,y:32727,varname:node_9414,prsc:2|IN-2453-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5190,x:31381,y:32553,ptovrint:False,ptlb:MaxMultiply,ptin:_MaxMultiply,varname:node_5190,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:7;n:type:ShaderForge.SFN_Multiply,id:3824,x:31586,y:32587,varname:node_3824,prsc:2|A-5190-OUT,B-7960-OUT;proporder:6074-8943-7960-5190;pass:END;sub:END;*/

Shader "Custom/Burner" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Ramp ("Ramp", 2D) = "white" {}
        _BurnAmount ("BurnAmount", Range(0, 1)) = 0
        _MaxMultiply ("MaxMultiply", Float ) = 7
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
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _Ramp; uniform float4 _Ramp_ST;
            uniform float _BurnAmount;
            uniform float _MaxMultiply;
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
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float node_9414 = (1.0 - (((_MaxMultiply*_BurnAmount)*_MainTex_var.r)+_MainTex_var.g));
                float2 node_469 = float2(node_9414,node_9414);
                float4 _Ramp_var = tex2D(_Ramp,TRANSFORM_TEX(node_469, _Ramp));
                float3 emissive = _Ramp_var.rgb;
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
