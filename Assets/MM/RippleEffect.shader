
Shader "Custom/RippleEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RippleCenter ("Ripple Center", Vector) = (0.5, 0.5, 0, 0)
        _RippleStrength ("Ripple Strength", Float) = 0.03
        _RippleRadius ("Ripple Radius", Float) = 1.5
        _RippleWidth ("Ripple Width", Float) = 0.06
        _RippleProgress ("Ripple Progress", Float) = -1
        _Aspect ("Aspect", Float) = 1.7778
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _RippleCenter;
            float _RippleStrength;
            float _RippleRadius;
            float _RippleWidth;
            float _RippleProgress;
            float _Aspect;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // 파동 계산에는 원본 UV 사용
                o.uv = v.uv;
                o.color = v.color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 1. 텍스처만 타일링
                float2 tiledUV =
                    i.uv * _MainTex_ST.xy + _MainTex_ST.zw;

                // 2. 파동은 타일링되지 않은 UV에서 계산
                float2 delta = i.uv - _RippleCenter.xy;
                delta.x *= _Aspect;

                float dist = length(delta);

                float2 direction =
                    delta / max(dist, 0.0001);

                // 3. 한 개의 원형 파동
                float waveRadius =
                    _RippleProgress * _RippleRadius;

                float width = max(_RippleWidth, 0.0001);

                float wave = exp(
                    -pow((dist - waveRadius) / width, 2.0)
                );

                // 재생하지 않을 때 파동 제거
                wave *= step(0.0, _RippleProgress);

                // 4. UV 왜곡
                float2 distortedUV = tiledUV +
                    float2(
                        direction.x / max(_Aspect, 0.0001),
                        direction.y
                    ) * wave * _RippleStrength;

                fixed4 col =
                    tex2D(_MainTex, distortedUV);

                return col * i.color;
            }
            ENDCG
        }
    }
}
