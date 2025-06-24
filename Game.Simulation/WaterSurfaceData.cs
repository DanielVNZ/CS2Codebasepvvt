using Unity.Collections;
using Unity.Mathematics;

namespace Game.Simulation;

public struct WaterSurfaceData
{
	public NativeArray<SurfaceWater> depths { get; private set; }

	public int3 resolution { get; private set; }

	public float3 scale { get; private set; }

	public float3 offset { get; private set; }

	public bool isCreated => depths.IsCreated;

	public WaterSurfaceData(NativeArray<SurfaceWater> _depths, int3 _resolution, float3 _scale, float3 _offset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		depths = _depths;
		resolution = _resolution;
		scale = _scale;
		offset = _offset;
	}
}
