
#ifndef NACHAL_MAIN_LIGHT_INCLUDED
#define NACHAL_MAIN_LIGHT_INCLUDED

#ifndef SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#endif

void MainLightDirection_float(out float3 Direction)
{
#ifdef SHADERGRAPH_PREVIEW
    Direction = normalize(float3(0.5, 1.0, 0.5));
#else
    Light mainLight = GetMainLight();
    Direction = normalize(mainLight.direction);
#endif
}

#endif
