using Colossal.IO.AssetDatabase.VirtualTexturing;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Rendering;

public static class VTTextureParamBlockExtension
{
	public static void SetTextureParamBlock(this MaterialPropertyBlock material, (int transform, int textureInfo) nameId, VTTextureParamBlock block)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		material.SetVector(nameId.transform, float4.op_Implicit(block.transform));
		material.SetVector(nameId.textureInfo, float4.op_Implicit(block.textureInfo));
	}
}
