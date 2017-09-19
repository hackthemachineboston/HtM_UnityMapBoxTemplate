// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "cylinderAdditiveDistortion"
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
			//ZTest off
 
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
				float3 normal : NORMAL;
				float3 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
 
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
				float fade : TEXCOORD2;
            };
 
            VertexOutput vert(VertexInput v) {

				float3 scale = float3(_Scale, _Scale, _Scale);

                VertexOutput o;
                o.uv0 = TRANSFORM_TEX(v.texcoord0, _MainTex);
                o.uv1 = TRANSFORM_TEX(v.texcoord1, _NoiseTex);
                o.uv1 += _Time.yy * _IntensityAndScrolling.zw;
				o.vertexColor = v.vertexColor;
				o.pos = UnityObjectToClipPos((v.vertex) * float4(scale, 1.0) + float4(0.0, scale.y - 1.0, 0.0, 0.0));
				//o.pos.z = o.pos.z * 4000.0;

				o.fade = (1.0 - (o.uv0.y + 0.35)) * (1.0 - v.normal.g);

                return o;
            }
 
            float4 frag(VertexOutput i) : COLOR {
                float4 noiseTex = tex2D(_NoiseTex, i.uv1);
				float4 color = _Color * i.vertexColor * noiseTex.y * i.fade;
				return color;
            }
            ENDCG
        }
    }
    FallBack "Mobile/Particles/Additive"
}