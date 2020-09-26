#include "../geometry.fxh"

float3 sunDirection;
float diffuseIntensity;
float ambientIntensity;

uniform texture _tex_atlas;
sampler2D textureAtlas = sampler_state {
    Texture = (_tex_atlas);
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct a2v {
    float4 position : POSITION;
    float4 normal : NORMAL;
    float2 uv : TEXCOORD0;
};

struct v2f {
    float4 position : POSITION;
    float2 uv : TEXCOORD0;
    float3 light : TEXCOORD1;
};


v2f vert(a2v a) {
    v2f o;
    float4 world = mul(a.position, _mat_model);
    o.position = mul(mul(world, _mat_view), _mat_proj);
    float4 worldNormal = mul(_mat_model, a.normal);
    o.light = clamp(dot(worldNormal.xyz, -sunDirection), 0, 1) * diffuseIntensity + ambientIntensity;
    o.uv = a.uv;
    return o;
}


float4 frag(v2f v) : COLOR {
    float4 color = tex2D(textureAtlas, v.uv);
    clip(color.a - 0.5);
    return float4(v.light, 1) * color;
}

technique Geometry {
    pass Geometry {
        VertexShader = compile vs_4_0_level_9_1 vert();
        PixelShader = compile ps_4_0_level_9_1 frag();
    }
}