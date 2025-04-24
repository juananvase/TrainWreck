Shader "Masked/Mask" 
{
	SubShader 
	{
		Tags { "Queue" = "Geometry-1" }

		Pass
		{
			Cull Off
			ColorMask 0
			
			Stencil
			{
				Ref 1
				Comp Always
				Pass Replace
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

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

			float _Health;

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag (v2f i) : SV_Target
			{
				// Procedural shrinking circle based on health
				float2 center = float2(0.5, 0.5);
				float dist = distance(i.uv, center);

				// Shrink the mask as health decreases
				if (dist > _Health) discard;

				return float4(0, 0, 0, 0);
			}
			ENDCG
		}
	}
}
