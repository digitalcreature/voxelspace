texture _tex;
sampler2D tex = sampler_state {
    Texture = (_tex);
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float2 position;
float2 size;
float2 texSize;

float4x4 proj;
