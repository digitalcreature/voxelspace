#include "quad.fxh"

float2 borderMin;
float2 borderMax;

struct a2v {
    float4 position : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f {
    float4 position : POSITION;
    float2 uv : TEXCOORD0;
    float2 toosmall : TEXCOORD1;
};

v2f vert(a2v a) {
    v2f o;
    float4 pos = a.position;
    pos.xy *= size;
    pos.xy += position;
    o.position = mul(pos, proj);
    o.uv = a.uv;
    o.toosmall.x = size.x < (borderMin.x + borderMax.x);
    o.toosmall.y = size.y < (borderMin.y + borderMax.y);
    return o;
}

float4 frag(v2f v) : COLOR {
    float x = v.uv.x * size.x;
    float y = v.uv.y * size.y;
    float2 uv = float2(x, y);
    float2 centerSize = texSize - borderMin - borderMax;
    float2 max = size - borderMax;
    if (v.toosmall.x) {
        if (v.uv.x < 0.5) {
            uv.x = x;
        }
        else {
            uv.x = texSize.x - borderMax.x + (x - max.x);
        }
    }
    else {
        if (x < borderMin.x) {
            uv.x = x;
        }
        else if (x < max.x) {
            uv.x = (borderMin.x + (x - borderMin.x) % centerSize.x);
        }
        else {
            uv.x = texSize.x - borderMax.x + (x - max.x);
        }
    }
    if (v.toosmall.y) {
        if (v.uv.y < 0.5) {
            uv.y = y;
        }
        else {
            uv.y = texSize.y - borderMax.y + (y - max.y);
        }
    }
    else {
        if (y < borderMin.y) {
            uv.y = y;
        }
        else if (y < max.y) {
            uv.y = (borderMin.y + (y - borderMin.y) % centerSize.y);
        }
        else {
            uv.y = texSize.y - borderMax.y + (y - max.y);
        }
    }
    uv /= texSize;
    return tex2D(tex, uv) * tint;
}

technique NinePatch {
    pass {
        VertexShader = compile vs_4_0_level_9_1 vert();
        PixelShader = compile ps_4_0_level_9_1 frag();
    }
}