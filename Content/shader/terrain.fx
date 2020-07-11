float4x4 model;
float4x4 view;
float4x4 proj;

float3 lightDirection;
float lightIntensity;
float lightAmbient;

uniform texture tex;
sampler2D texSampler = sampler_state {
    Texture = (tex);
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
    float light : TEXCOORD1;
    float ao : TEXCOORD2;
};

v2f vert(a2v a) {
    v2f o;
    float4 world = mul(a.position, model);
    o.position = mul(mul(world, view), proj);
    float4 worldNormal = mul(model, a.normal);
    o.light = clamp(dot(worldNormal.xyz, -lightDirection), 0, 1) * lightIntensity + lightAmbient;
    o.ao = 1 - (1 - a.ao) * .5;
    o.uv = a.uv;
    float3 sunDir = lightDirection;
    // normalize so that the sum == 1
    sunDir /= abs(sunDir.x) + abs(sunDir.y) + abs(sunDir.z);
    float3 sun = lerp(a.lightSunN, a.lightSunP, clamp(sign(sunDir), 0, 1)) * abs(sunDir);
    float totalSunLight = sun.x + sun.y + sun.z;
    o.light *= totalSunLight; //clamp(totalSunLight + a.lightPoint, 0, 1);
    return o;
}

float4 frag(v2f v) : COLOR {
    float4 color = tex2D(texSampler, v.uv);
    clip(color.a - 0.5);
    return v.light * v.ao * color;
}

technique Terrain {
    pass Pass {
        VertexShader = compile vs_4_0 vert();
        PixelShader = compile ps_4_0 frag();
    }
}