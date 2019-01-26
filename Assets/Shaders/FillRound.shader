Shader "Custom/Fill-Round" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
		_FillRate ("FillRate", Range(0.0, 1.0)) = 1.0
	}
	SubShader {
		Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}
		LOD 100

		ZWrite Off
		Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf Unlit alpha:fade

		sampler2D _MainTex;
		float _FillRate;
		float4 _Center;
		float4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
		{
			return half4(s.Albedo/1.35, s.Alpha);
		}

		void surf (Input IN, inout SurfaceOutput o) {
			half2 uv = IN.uv_MainTex.xy;
 			half2 _textCord = uv;
			
			half4 c = tex2D(_MainTex, _textCord);
			o.Albedo = c.rgb * _Color;

			float angle = (atan2(uv.y - 0.5, uv.x - 0.5) + 3.14) / 6.28;
 			if ( angle <= _FillRate ) 
 			{
				o.Alpha = c.a;
 			} 
			else 
			{
				o.Alpha = 0;
			}	
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
