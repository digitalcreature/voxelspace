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
    float2 uv : TEXCOORD;
};

struct v2f {
    float4 position : POSITION;
    float2 uv : TEXCOORD0;
    float diff : TEXCOORD1;
};

v2f vert(a2v a) {
    v2f o;
    float4 world = mul(a.position, model);
    o.position = mul(mul(world, view), proj);
    float4 worldNormal = mul(model, a.normal);
    o.diff = abs(dot(worldNormal.xyz, lightDirection)) * lightIntensity + lightAmbient;
    o.uv = a.uv;
    return o;
}

float4 frag(v2f v) : COLOR {
    float4 color = tex2D(texSampler, v.uv);
    color.a = 1;
    return v.diff * color;
}

technique Terrain {
    pass Pass {
        VertexShader = compile vs_2_0 vert();
        PixelShader = compile ps_2_0 frag();
    }
}