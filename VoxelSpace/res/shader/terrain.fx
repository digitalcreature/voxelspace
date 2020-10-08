#include "geometry.fxh"

float3 sunDirection;
float diffuseIntensity;
float ambientIntensity;

float3 sunlightColor;
float3 starlightColor;

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
    float ao : TEXCOORD2;
    float3 lightSunP : TEXCOORD3;
    float3 lightSunN : TEXCOORD4;
    float lightPoint : TEXCOORD5;
};

struct v2f {
    float4 position : POSITION;
    float2 uv : TEXCOORD0;
    float3 light : TEXCOORD1;
    float ao : TEXCOORD2;
};


v2f vert(a2v a) {
    v2f o;
    float4 world = mul(a.position, _mat_model);
    o.position = mul(mul(world, _mat_view), _mat_proj);
    float4 worldNormal = mul(_mat_model, a.normal);
    o.light = clamp(dot(worldNormal.xyz, -sunDirection), 0, 1) * diffuseIntensity + ambientIntensity;
    o.ao = 1 - (1 - a.ao) * .5;
    o.uv = a.uv;
    float3 sunDir = sunDirection;
    // normalize so that the sum == 1
    sunDir /= abs(sunDir.x) + abs(sunDir.y) + abs(sunDir.z);
    float3 starDir = -sunDir;
    float3 sun = lerp(a.lightSunN, a.lightSunP, clamp(sign(sunDir), 0, 1)) * abs(sunDir);
    float3 star = lerp(a.lightSunN, a.lightSunP, clamp(sign(starDir), 0, 1)) * abs(starDir);
    float sunLight = sun.x + sun.y + sun.z;
    float starLight = star.x + star.y + star.z;
    // sunLight = clamp(sunLight * 2 - 0.5, 0, 1);
    // starLight = clamp(starLight * 2 - 0.5, 0, 1);
    float averageSkyLight = max(sunLight, starLight);
    float t = sunLight - starLight;
    // t = (t + 2) / 4;
    t = clamp(t + .5, 0, 1);
    o.light *= lerp(starlightColor, sunlightColor, t) * averageSkyLight; //clamp(sunLight + a.lightPoint, 0, 1);
    return o;
}


float4 frag(v2f v) : COLOR {
    float4 color = tex2D(textureAtlas, v.uv);
    clip(color.a - 0.5);
    return float4(v.light, 1) * v.ao * color;
}

technique Geometry {
    pass Geometry {
        VertexShader = compile vs_4_0_level_9_1 vert();
        PixelShader = compile ps_4_0_level_9_1 frag();
    }
}