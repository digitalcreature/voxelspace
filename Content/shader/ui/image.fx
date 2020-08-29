#include "quad.fxh"

struct a2v {
    float4 position : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f {
    float4 position : POSITION;
    float2 uv : TEXCOORD0;
};

v2f vert(a2v a) {
    v2f o;
    float4 pos = a.position;
    pos.xy *= size;
    pos.xy += position;
    o.position = mul(pos, proj);
    o.uv = a.uv;
    return o;
}

float4 frag(v2f v) : COLOR {
    return tex2D(tex, v.uv);
}

technique NinePatch {
    pass {
        VertexShader = compile vs_4_0_level_9_1 vert();
        PixelShader = compile ps_4_0_level_9_1 frag();
    }
}