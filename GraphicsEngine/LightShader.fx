// DirectX 11
float4x4 WorldViewProjection;

struct VertexInput
{
  float3 position : SV_POSITION;
  float3 normal   : NORMAL;
};

struct VertexOutput
{
  float4 position : SV_POSITION;
  float4 color    : COLOR;
};

VertexOutput VS(VertexInput input)
{
  VertexOutput output;
  output.position = mul(WorldViewProjection,
    float4(input.position, 1));
  output.color = float4(0.5 + input.normal / 2, 1.0);
  return output;
}

float4 PS(VertexOutput input) : SV_TARGET
{
  return input.color;
}