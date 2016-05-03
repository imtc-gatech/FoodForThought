Shader "Tinted Alpha Cutoff" { 

Properties {
    _Color ("Tint (A = Opacity)", Color) = (1,1,1,1)
	_ColorHUD ("HUD Color", Color) = (0,1,0,1) 
    _MainTex ("Texture (A = Transparency)", 2D) = "" {}
	_Cutoff ("Cutoff", Range(0.0,1.0)) = 1
} 

SubShader {
    Tags {Queue = Transparent}
    ZWrite Off
    //Blend SrcAlpha OneMinusSrcAlpha

    Pass {
		AlphaTest Greater [_Cutoff]
        SetTexture[_MainTex] {
			
            ConstantColor [_ColorHUD]
            //Combine texture * constant
			Combine texture * constant DOUBLE, texture * constant
        } 
    }
	

}

}

/*

Tags { "Queue"="Geometry" }
    ZTest Always
    Blend SrcAlpha OneMinusSrcAlpha

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

uniform float4 _Color;
uniform float _Cutoff;

struct appdata {
	float4 vertex : POSITION;
	fixed4 color : COLOR;
}

struct v2f {
    float4 pos : SV_POSITION;
    fixed4 color : COLOR;
};

v2f vert (appdata v)
{
    v2f o;
    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
    o.color = v.color;
    return o;
}

half4 frag (v2f i) : COLOR
{
	if (i.color.w > _Cutoff)
		return half4 (_Color, 1);
	else
		return half4(0,0,0,0);
}
ENDCG

*/

