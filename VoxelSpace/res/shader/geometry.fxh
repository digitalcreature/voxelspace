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

float4x4 _mat_proj_shadow;
float4x4 _mat_view_shadow;

Texture2D _shadowMap;

float _shadowBias = 0.01;

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

v2f_shadow vert_shadow(float4 position : POSITION) {
    v2f_shadow v;
    v.position = mul(mul(mul(position, _mat_model), _mat_view_shadow), _mat_proj_shadow);
    v.depth = v.position.z;
    return v;
}

float4 frag_shadow(v2f_shadow v) : COLOR {
    return EncodeFloatRGBA(v.depth);
}

// recieve shadows
//////////////////


float3 getShadowData(float4 position) {
    float4 pos = mul(mul(mul(position, _mat_model), _mat_view_shadow), _mat_proj_shadow);
    float3 data;
    data.x = pos.x / 2 + 0.5;
    data.y = -pos.y / 2 + 0.5;
    data.z = pos.z;
    return data;
}

#ifdef NOSHADOW

#define SHADOWDATA(semantic)
#define TRANSFERSHADOW(v, position) 0
#define GETSHADOW(v) 1

#else

// element for v2f struct. semantics left up to user
#define SHADOWDATA(semantic) float3 shadow_data : semantic;

#define TRANSFERSHADOW(v, position) v.shadow_data = getShadowData(position)

#define GETSHADOW(v) getShadowStrength(v.shadow_data)

#endif

float getShadowStrength(float3 data) {
    float decodedDepth = DecodeFloatRGBA(_shadowMap.Sample(LinearClamp, data.xy));
    return decodedDepth > data.z - _shadowBias;
}

technique CastShadow {
    pass {
        VertexShader = compile vs_4_0_level_9_1 vert_shadow();
        PixelShader = compile ps_4_0_level_9_1 frag_shadow();
    }
}