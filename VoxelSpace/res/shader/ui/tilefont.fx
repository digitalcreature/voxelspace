#include "quad.fxh"

struct a2v {
    float4 position : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f {
    float4 position : POSITION;
    float2 uv : TEXCOORD0;
};

float2 uvOffset;

v2f vert(a2v a) {
    v2f o;
    float4 pos = a.position;
    pos.xy *= size;
    pos.xy += position;
    o.position = mul(pos, proj);
    o.uv = a.uv * float2(1/16.0, 1/8.0) + uvOffset;
    return o;
}

float4 frag(v2f v) : COLOR {
    float4 color = tex2D(tex, v.uv);
    // only draw white pixels from the source texture
    return tint * color.x * color.y * color.z;
}

technique TileFont {
    pass {
        VertexShader = compile vs_4_0_level_9_1 vert();
        PixelShader = compile ps_4_0_level_9_1 frag();
    }
}