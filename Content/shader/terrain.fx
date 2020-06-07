float4x4 model;
float4x4 view;
float4x4 proj;

float3 lightDirection;

struct a2v {
    float4 position : POSITION;
    float4 normal : NORMAL;
};

struct v2f {
    float4 position : POSITION;
    float diff : TEXCOORD0;
};

v2f vert(a2v a) {
    v2f o;
    float4 world = mul(a.position, model);
    o.position = mul(mul(world, view), proj);
    float4 worldNormal = mul(model, a.normal);
    o.diff = abs(dot(worldNormal.xyz, lightDirection));
    return o;
}

float4 frag(v2f v) : COLOR {
    return v.diff;
}

technique Terrain {
    pass Pass {
        VertexShader = compile vs_2_0 vert();
        PixelShader = compile ps_2_0 frag();
    }
}