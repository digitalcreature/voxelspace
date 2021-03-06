float4x4 _mat_proj;
float4x4 _mat_view;
float4x4 _mat_model;

Texture2D _mainTex;

SamplerState PointClamped {
    MinFilter = Point;
    MagFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

SamplerState LinearClamp {
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

SamplerState ShadowMapSample {
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Border;
    AddressV = Border;
    BorderColor = 0x00000000;
};

float4x4 _mat_proj_shadow;
float4x4 _mat_view_shadow;

Texture2D _shadowMap;

float _shadowMapRadius;
float _shadowBias = 0.1;

struct PositionNormal {
    float4 position : POSITION;
    float4 normal : NORMAL;
};

#define NORMALBIAS 0.01

float4 getOffsetPosition(PositionNormal pn) {
    return pn.position + pn.normal * NORMALBIAS;
}

float4 EncodeFloatRGBA(float v) {
    float4 kEncodeMul = float4(1.0, 255.0, 65025.0, 16581375.0);
    float kEncodeBit = 1.0 / 255.0;
    float4 enc = kEncodeMul * v;
    enc = frac(enc);
    enc -= enc.yzww * kEncodeBit;
    return enc;
}

float DecodeFloatRGBA(float4 enc) {
    float4 kDecodeDot = float4(1.0, 1 / 255.0, 1 / 65025.0, 1 / 16581375.0);
    return dot(enc, kDecodeDot);
}

// cast shadows
////////////////
struct v2f_shadow {
    float4 position : POSITION;
    float depth : TEXCOORD;
};

v2f_shadow vert_shadow(PositionNormal pn) {
    v2f_shadow v;
    v.position = mul(mul(mul(pn.position, _mat_model), _mat_view_shadow), _mat_proj_shadow);
    v.depth = 1 - v.position.z;
    return v;
}

float4 frag_shadow(v2f_shadow v) : COLOR {
    return EncodeFloatRGBA(v.depth);
}

// recieve shadows
//////////////////


float3 getShadowData(PositionNormal pn) {
    float4 pos = mul(mul(mul(pn.position, _mat_model), _mat_view_shadow), _mat_proj_shadow);
    float3 data;
    data.x = pos.x / 2 + 0.5;
    data.y = -pos.y / 2 + 0.5;
    data.z = 1 - pos.z;
    return data;
}

#ifdef NOSHADOW

#define SHADOWDATA(semantic)
#define TRANSFERSHADOW(v, pn) 0
#define GETSHADOW(v) 1

#else

// element for v2f struct. semantics left up to user
#define SHADOWDATA(semantic) float3 shadow_data : semantic;

#define TRANSFERSHADOW(v, pn) v.shadow_data = getShadowData(pn)

#define GETSHADOW(v) getBlurredShadowStrength(v.shadow_data)

#endif

#define BLURDIST 0.01

float getShadowStrength(float2 uv, float depth);

float getBlurredShadowStrength(float3 data) {
    float2 uv = data.xy;
    float depth = data.z;
    // float strength = 0;
    // float range = 2 * _shadowMapRadius;
    // strength += getShadowStrength(uv + float2(-1, -1) * BLURDIST / range, depth);
    // strength += getShadowStrength(uv + float2( 0, -1) * BLURDIST / range, depth);
    // strength += getShadowStrength(uv + float2(+1, -1) * BLURDIST / range, depth);
    // strength += getShadowStrength(uv + float2(-1,  0) * BLURDIST / range, depth);
    // strength += getShadowStrength(uv + float2( 0,  0) * BLURDIST / range, depth);
    // strength += getShadowStrength(uv + float2(+1,  0) * BLURDIST / range, depth);
    // strength += getShadowStrength(uv + float2(-1, +1) * BLURDIST / range, depth);
    // strength += getShadowStrength(uv + float2( 0, +1) * BLURDIST / range, depth);
    // strength += getShadowStrength(uv + float2(+1, +1) * BLURDIST / range, depth);
    // return strength / 9;
    return getShadowStrength(uv, depth) * .5 + .5;
}

float getShadowStrength(float2 uv, float depth) {
    float decodedDepth = DecodeFloatRGBA(_shadowMap.Sample(ShadowMapSample, uv));
    if (depth > 1 || depth < 0) return 1;
    float range = 2 * _shadowMapRadius;
    decodedDepth *= range;
    depth *= range;
    float diff = depth - decodedDepth + _shadowBias;
    return smoothstep(0, _shadowBias, diff);
}

technique CastShadow {
    pass {
        VertexShader = compile vs_4_0_level_9_1 vert_shadow();
        PixelShader = compile ps_4_0_level_9_1 frag_shadow();
    }
}