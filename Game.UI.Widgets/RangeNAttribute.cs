using Unity.Mathematics;
using UnityEngine;

namespace Game.UI.Widgets;

public class RangeNAttribute : PropertyAttribute
{
	public float4 min { get; private set; }

	public float4 max { get; private set; }

	public RangeNAttribute(float min, float max, bool componentExpansion = true)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (componentExpansion)
		{
			this.min = float4.op_Implicit(min);
			this.max = float4.op_Implicit(max);
		}
		else
		{
			this.min = new float4(min, float3.zero);
			this.max = new float4(max, float3.zero);
		}
	}

	public RangeNAttribute(float2 min, float2 max)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		this.min = new float4(min, float2.zero);
		this.max = new float4(max, float2.zero);
	}

	public RangeNAttribute(float3 min, float3 max)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		this.min = new float4(min, 0f);
		this.max = new float4(max, 0f);
	}

	public RangeNAttribute(float4 min, float4 max)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		this.min = min;
		this.max = max;
	}
}
