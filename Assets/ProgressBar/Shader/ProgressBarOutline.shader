// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'

Shader "Custom/ProgressBarOutline" {

Properties {
    _Under ("Color", Color) = (1,1,0,1)
	_Even ("Color", Color) = (0,1,0,1)
	_Over ("Color", Color) = (1,0,0,1)
	_Border ("Color", Color) = (0,0,0,1)
	_Peak ("Color", Color) = (1,1,1,1)
	//_OutlineColor ("Outline Color", Color) = (0,0,0,1)
	_Stripe ("Stripe", Vector) = (0.3,0.3,0.3,0.1)
    _MainTex ("Main Tex (RGBA)", 2D) = "white" {}
    _Outline ("Outline", Range(0.0,1.0)) = 0.05
    _PeakValue ("Peak Value", Range(0.0,1.0)) = 0
	//_OutlineWidth ("Outline Width", Range(0.0,1.0)) = 0.1
}

SubShader {
    	Tags { "Queue"="Geometry" }
    	ZTest Always
    	Blend SrcAlpha OneMinusSrcAlpha
        Pass {

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform float4 _Under;
uniform float4 _Even;
uniform float4 _Over;
uniform float4 _Border;
uniform float4 _Peak;
uniform float4 _Stripe;
uniform float _Outline;
uniform float _PeakValue;

struct v2f {
    float4 pos : POSITION;
    float2 uv : TEXCOORD0;
};

v2f vert (appdata_base v)
{
    v2f o;
    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
    o.uv = TRANSFORM_UV(0);

    return o;
}

half4 frag( v2f i ) : COLOR
{ 
    half4 color = tex2D( _MainTex, i.uv);
	float pillar2 = _Stripe.x + _Stripe.y;
	
	float4 result;
	
	if (i.uv.x < _Stripe.x)
	{
		if (i.uv.x > _Outline && i.uv.x < _Stripe.x - _Outline)
			result = _Under;
		else
			result = _Border;
	}
	else if (i.uv.x > pillar2)
	{
		if (i.uv.x > pillar2 + _Outline)
			result = _Over;
		else
			result = _Border;
	}
	else
	{
		result = _Even;
		if (_PeakValue > 0)
		{
			if (i.uv.x > _PeakValue - _Outline)
			{
				result = _Peak;
				if (i.uv.x > (_PeakValue + _Outline))
					result = _Even;
			}
			else
				result = _Even;	
		}
			
	}

	if ((i.uv.y < _Outline) || 
		(i.uv.y > 1 - _Outline) ||
		(i.uv.x > 1 - _Outline))
		result = _Border;
	
	return color*result;
}

ENDCG

    }
}

}