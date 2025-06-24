using System;
using Colossal;
using Colossal.Collections;
using Colossal.Mathematics;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Common;

public struct QuadTreeBoundsXZ : IEquatable<QuadTreeBoundsXZ>, IBounds2<QuadTreeBoundsXZ>
{
	public struct DebugIterator<TItem> : INativeQuadTreeIterator<TItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<TItem, QuadTreeBoundsXZ> where TItem : unmanaged, IEquatable<TItem>
	{
		private Bounds3 m_Bounds;

		private GizmoBatcher m_GizmoBatcher;

		public DebugIterator(Bounds3 bounds, GizmoBatcher gizmoBatcher)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Bounds = bounds;
			m_GizmoBatcher = gizmoBatcher;
		}

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds))
			{
				return false;
			}
			float3 val = MathUtils.Center(bounds.m_Bounds);
			float3 val2 = MathUtils.Size(bounds.m_Bounds);
			((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(val, val2, Color.white);
			return true;
		}

		public void Iterate(QuadTreeBoundsXZ bounds, TItem edgeEntity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (MathUtils.Intersect(bounds.m_Bounds, m_Bounds))
			{
				float3 val = MathUtils.Center(bounds.m_Bounds);
				float3 val2 = MathUtils.Size(bounds.m_Bounds);
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(val, val2, Color.red);
			}
		}
	}

	public Bounds3 m_Bounds;

	public BoundsMask m_Mask;

	public byte m_MinLod;

	public byte m_MaxLod;

	public QuadTreeBoundsXZ(Bounds3 bounds)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Bounds = bounds;
		m_Mask = BoundsMask.Debug | BoundsMask.NormalLayers | BoundsMask.NotOverridden | BoundsMask.NotWalkThrough;
		m_MinLod = 1;
		m_MaxLod = 1;
	}

	public QuadTreeBoundsXZ(Bounds3 bounds, BoundsMask mask, int lod)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Bounds = bounds;
		m_Mask = mask;
		m_MinLod = (byte)lod;
		m_MaxLod = (byte)lod;
	}

	public QuadTreeBoundsXZ(Bounds3 bounds, BoundsMask mask, int minLod, int maxLod)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Bounds = bounds;
		m_Mask = mask;
		m_MinLod = (byte)minLod;
		m_MaxLod = (byte)maxLod;
	}

	public bool Equals(QuadTreeBoundsXZ other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Bounds3)(ref m_Bounds)).Equals(other.m_Bounds) & (m_Mask == other.m_Mask) & m_MinLod.Equals(other.m_MinLod) & m_MaxLod.Equals(other.m_MaxLod);
	}

	public void Reset()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		m_Bounds.min = float3.op_Implicit(float.MaxValue);
		m_Bounds.max = float3.op_Implicit(float.MinValue);
		m_Mask = (BoundsMask)0;
		m_MinLod = byte.MaxValue;
		m_MaxLod = 0;
	}

	public float2 Center()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		float3 val = MathUtils.Center(m_Bounds);
		return ((float3)(ref val)).xz;
	}

	public float2 Size()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		float3 val = MathUtils.Size(m_Bounds);
		return ((float3)(ref val)).xz;
	}

	public QuadTreeBoundsXZ Merge(QuadTreeBoundsXZ other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new QuadTreeBoundsXZ(m_Bounds | other.m_Bounds, m_Mask | other.m_Mask, math.min((int)m_MinLod, (int)other.m_MinLod), math.max((int)m_MaxLod, (int)other.m_MaxLod));
	}

	public bool Intersect(QuadTreeBoundsXZ other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return MathUtils.Intersect(m_Bounds, other.m_Bounds);
	}
}
