// DirectX 11
float4x4 WorldViewProjection;
float3 LightDirection;

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
  float Ambient = 0.2;
  float Diffuse = 0.8;
  float lightIntensity = saturate(dot(LightDirection, input.normal));
  float light = Ambient + Diffuse * lightIntensity;
  output.color = float4(light, light, light, 1.0);
  return output;
}

float4 PS(VertexOutput input) : SV_TARGET
{
  return input.color;
}