Shader "Masked/CrackMaterial"
{
	SubShader
	{
		Tags { "Queue" = "Geometry" }

		Stencil
		{
			Ref 1
			Comp Equal
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _CrackTex;
			float4 _Color;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 crack = tex2D(_CrackTex, i.uv);
				return crack * _Color;
			}
			ENDCG
		}
	}
}
