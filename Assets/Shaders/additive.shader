// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "additiveDistortion"
{
    Properties{
        _MainTex("Main Texture", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _IntensityAndScrolling("Intensity (XY), Scrolling (ZW)", Vector) = (0.1,0.1,0.1,0.1)
		_Color("Main Color", Color) = (1, 1, 1, 1)
		_Scale("Scale", float) = 1.0
	}
    SubShader{
        Tags {
            "IgnoreProjector" = "True"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
        Pass {
            Blend One One
            ZWrite Off
			//ZTest always
			Cull off
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
			uniform float4 _Color;
			uniform float _Scale;

            uniform sampler2D _MainTex;
            uniform sampler2D _NoiseTex;
            uniform float4 _MainTex_ST;
            uniform float4 _NoiseTex_ST;
            uniform float4 _IntensityAndScrolling;
 
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
 
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
 
            VertexOutput vert(VertexInput v) {

                VertexOutput o;
                o.uv0 = TRANSFORM_TEX(v.texcoord0, _MainTex);
                o.uv1 = TRANSFORM_TEX(v.texcoord1, _NoiseTex);
                o.uv1 += _Time.yy * _IntensityAndScrolling.zw;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex * float4(_Scale, _Scale, 1.0, 1.0));

                return o;
            }
 
            float4 frag(VertexOutput i) : COLOR {
                float4 noiseTex = tex2D(_NoiseTex, i.uv1);
                float2 offset = (noiseTex.rg * 2 - 1) * _IntensityAndScrolling.rg;
                float2 uvNoise = i.uv0 + offset;
                float4 mainTex = tex2D(_MainTex, uvNoise);
                float3 emissive = _Color * (mainTex.rgb * i.vertexColor.rgb) * (mainTex.a * i.vertexColor.a);
 
                return fixed4(emissive, 1);
            }
            ENDCG
        }
    }
    FallBack "Mobile/Particles/Additive"
}