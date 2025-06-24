using System;
using Colossal;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Effects;
using Game.Net;
using Game.Objects;
using Game.Routes;
using Game.Zones;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

public class SearchTreeDebugSystem : BaseDebugSystem
{
	[BurstCompile]
	private struct NativeQuadTreeGizmoJob<TItem, TBounds, TIterator> : IJob where TItem : unmanaged, IEquatable<TItem> where TBounds : unmanaged, IEquatable<TBounds>, IBounds2<TBounds> where TIterator : unmanaged, INativeQuadTreeIterator<TItem, TBounds>
	{
		[ReadOnly]
		public NativeQuadTree<TItem, TBounds> m_Tree;

		public TIterator m_Iterator;

		public void Execute()
		{
			m_Tree.Iterate<TIterator>(ref m_Iterator, 0);
		}
	}

	public struct Bounds2DebugIterator<TItem> : INativeQuadTreeIterator<TItem, Bounds2>, IUnsafeQuadTreeIterator<TItem, Bounds2> where TItem : unmanaged, IEquatable<TItem>
	{
		private Bounds2 m_Bounds;

		private GizmoBatcher m_GizmoBatcher;

		public Bounds2DebugIterator(Bounds2 bounds, GizmoBatcher gizmoBatcher)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Bounds = bounds;
			m_GizmoBatcher = gizmoBatcher;
		}

		public bool Intersect(Bounds2 bounds)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			if (!MathUtils.Intersect(bounds, m_Bounds))
			{
				return false;
			}
			float2 val = MathUtils.Center(bounds);
			float2 val2 = MathUtils.Size(bounds) * 0.5f;
			((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireRect(new float3(val.x, 0f, val.y), val2, Color.white);
			return true;
		}

		public void Iterate(Bounds2 bounds, TItem edgeEntity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			if (MathUtils.Intersect(bounds, m_Bounds))
			{
				float2 val = MathUtils.Center(bounds);
				float2 val2 = MathUtils.Size(bounds) * 0.5f;
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireRect(new float3(val.x, 0f, val.y), val2, Color.red);
			}
		}
	}

	public struct LocalEffectDebugIterator : INativeQuadTreeIterator<LocalEffectSystem.EffectItem, LocalEffectSystem.EffectBounds>, IUnsafeQuadTreeIterator<LocalEffectSystem.EffectItem, LocalEffectSystem.EffectBounds>
	{
		private Bounds2 m_Bounds;

		private GizmoBatcher m_GizmoBatcher;

		public LocalEffectDebugIterator(Bounds2 bounds, GizmoBatcher gizmoBatcher)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Bounds = bounds;
			m_GizmoBatcher = gizmoBatcher;
		}

		public bool Intersect(LocalEffectSystem.EffectBounds bounds)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds))
			{
				return false;
			}
			float2 val = MathUtils.Center(bounds.m_Bounds);
			float2 val2 = MathUtils.Size(bounds.m_Bounds) * 0.5f;
			((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireRect(new float3(val.x, 0f, val.y), val2, Color.white);
			return true;
		}

		public void Iterate(LocalEffectSystem.EffectBounds bounds, LocalEffectSystem.EffectItem item)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			if (MathUtils.Intersect(bounds.m_Bounds, m_Bounds))
			{
				float2 val = MathUtils.Center(bounds.m_Bounds);
				float2 val2 = MathUtils.Size(bounds.m_Bounds) * 0.5f;
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireRect(new float3(val.x, 0f, val.y), val2, Color.red);
			}
		}
	}

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private Game.Zones.SearchSystem m_ZoneSearchSystem;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private Game.Routes.SearchSystem m_RouteSearchSystem;

	private Game.Effects.SearchSystem m_EffectSearchSystem;

	private LocalEffectSystem m_LocalEffectSystem;

	private GizmosSystem m_GizmosSystem;

	private Option m_StaticObjectOption;

	private Option m_MovingObjectOption;

	private Option m_NetOption;

	private Option m_LaneOption;

	private Option m_ZoneOption;

	private Option m_AreaOption;

	private Option m_RouteOption;

	private Option m_EffectOption;

	private Option m_LocalEffectOption;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_ZoneSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Zones.SearchSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_RouteSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Routes.SearchSystem>();
		m_EffectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Effects.SearchSystem>();
		m_LocalEffectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LocalEffectSystem>();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_StaticObjectOption = AddOption("Static Objects", defaultEnabled: true);
		m_MovingObjectOption = AddOption("Moving Objects", defaultEnabled: true);
		m_NetOption = AddOption("Nets", defaultEnabled: false);
		m_LaneOption = AddOption("Lanes", defaultEnabled: false);
		m_ZoneOption = AddOption("Zones", defaultEnabled: false);
		m_AreaOption = AddOption("Areas", defaultEnabled: false);
		m_RouteOption = AddOption("Routes", defaultEnabled: false);
		m_EffectOption = AddOption("Effects", defaultEnabled: false);
		m_LocalEffectOption = AddOption("Local Effects", defaultEnabled: false);
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 bounds = default(Bounds3);
		((Bounds3)(ref bounds))._002Ector(float3.op_Implicit(float.MinValue), float3.op_Implicit(float.MaxValue));
		JobHandle val = inputDeps;
		if (m_StaticObjectOption.enabled)
		{
			JobHandle val2 = StaticObjectSearchTreeDebug(inputDeps, bounds);
			val = JobHandle.CombineDependencies(val, val2);
		}
		if (m_MovingObjectOption.enabled)
		{
			JobHandle val3 = MovingObjectSearchTreeDebug(inputDeps, bounds);
			val = JobHandle.CombineDependencies(val, val3);
		}
		if (m_NetOption.enabled)
		{
			JobHandle val4 = NetSearchTreeDebug(inputDeps, bounds);
			val = JobHandle.CombineDependencies(val, val4);
		}
		if (m_LaneOption.enabled)
		{
			JobHandle val5 = LaneSearchTreeDebug(inputDeps, bounds);
			val = JobHandle.CombineDependencies(val, val5);
		}
		if (m_ZoneOption.enabled)
		{
			JobHandle val6 = ZoneSearchTreeDebug(inputDeps, bounds);
			val = JobHandle.CombineDependencies(val, val6);
		}
		if (m_AreaOption.enabled)
		{
			JobHandle val7 = AreaSearchTreeDebug(inputDeps, bounds);
			val = JobHandle.CombineDependencies(val, val7);
		}
		if (m_RouteOption.enabled)
		{
			JobHandle val8 = RouteSearchTreeDebug(inputDeps, bounds);
			val = JobHandle.CombineDependencies(val, val8);
		}
		if (m_EffectOption.enabled)
		{
			JobHandle val9 = EffectSearchTreeDebug(inputDeps, bounds);
			val = JobHandle.CombineDependencies(val, val9);
		}
		if (m_LocalEffectOption.enabled)
		{
			JobHandle val10 = LocalEffectSearchTreeDebug(inputDeps, bounds);
			val = JobHandle.CombineDependencies(val, val10);
		}
		return val;
	}

	private JobHandle StaticObjectSearchTreeDebug(JobHandle inputDeps, Bounds3 bounds)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle val2 = default(JobHandle);
		JobHandle val = IJobExtensions.Schedule<NativeQuadTreeGizmoJob<Entity, QuadTreeBoundsXZ, QuadTreeBoundsXZ.DebugIterator<Entity>>>(new NativeQuadTreeGizmoJob<Entity, QuadTreeBoundsXZ, QuadTreeBoundsXZ.DebugIterator<Entity>>
		{
			m_Tree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies),
			m_Iterator = new QuadTreeBoundsXZ.DebugIterator<Entity>(bounds, m_GizmosSystem.GetGizmosBatcher(ref val2))
		}, JobHandle.CombineDependencies(inputDeps, dependencies, val2));
		m_ObjectSearchSystem.AddStaticSearchTreeReader(val);
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		return val;
	}

	private JobHandle MovingObjectSearchTreeDebug(JobHandle inputDeps, Bounds3 bounds)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle val2 = default(JobHandle);
		JobHandle val = IJobExtensions.Schedule<NativeQuadTreeGizmoJob<Entity, QuadTreeBoundsXZ, QuadTreeBoundsXZ.DebugIterator<Entity>>>(new NativeQuadTreeGizmoJob<Entity, QuadTreeBoundsXZ, QuadTreeBoundsXZ.DebugIterator<Entity>>
		{
			m_Tree = m_ObjectSearchSystem.GetMovingSearchTree(readOnly: true, out dependencies),
			m_Iterator = new QuadTreeBoundsXZ.DebugIterator<Entity>(bounds, m_GizmosSystem.GetGizmosBatcher(ref val2))
		}, JobHandle.CombineDependencies(inputDeps, dependencies, val2));
		m_ObjectSearchSystem.AddMovingSearchTreeReader(val);
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		return val;
	}

	private JobHandle NetSearchTreeDebug(JobHandle inputDeps, Bounds3 bounds)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle val2 = default(JobHandle);
		JobHandle val = IJobExtensions.Schedule<NativeQuadTreeGizmoJob<Entity, QuadTreeBoundsXZ, QuadTreeBoundsXZ.DebugIterator<Entity>>>(new NativeQuadTreeGizmoJob<Entity, QuadTreeBoundsXZ, QuadTreeBoundsXZ.DebugIterator<Entity>>
		{
			m_Tree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_Iterator = new QuadTreeBoundsXZ.DebugIterator<Entity>(bounds, m_GizmosSystem.GetGizmosBatcher(ref val2))
		}, JobHandle.CombineDependencies(inputDeps, dependencies, val2));
		m_NetSearchSystem.AddNetSearchTreeReader(val);
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		return val;
	}

	private JobHandle LaneSearchTreeDebug(JobHandle inputDeps, Bounds3 bounds)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle val2 = default(JobHandle);
		JobHandle val = IJobExtensions.Schedule<NativeQuadTreeGizmoJob<Entity, QuadTreeBoundsXZ, QuadTreeBoundsXZ.DebugIterator<Entity>>>(new NativeQuadTreeGizmoJob<Entity, QuadTreeBoundsXZ, QuadTreeBoundsXZ.DebugIterator<Entity>>
		{
			m_Tree = m_NetSearchSystem.GetLaneSearchTree(readOnly: true, out dependencies),
			m_Iterator = new QuadTreeBoundsXZ.DebugIterator<Entity>(bounds, m_GizmosSystem.GetGizmosBatcher(ref val2))
		}, JobHandle.CombineDependencies(inputDeps, dependencies, val2));
		m_NetSearchSystem.AddLaneSearchTreeReader(val);
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		return val;
	}

	private JobHandle ZoneSearchTreeDebug(JobHandle inputDeps, Bounds3 bounds)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle val2 = default(JobHandle);
		JobHandle val = IJobExtensions.Schedule<NativeQuadTreeGizmoJob<Entity, Bounds2, Bounds2DebugIterator<Entity>>>(new NativeQuadTreeGizmoJob<Entity, Bounds2, Bounds2DebugIterator<Entity>>
		{
			m_Tree = m_ZoneSearchSystem.GetSearchTree(readOnly: true, out dependencies),
			m_Iterator = new Bounds2DebugIterator<Entity>(((Bounds3)(ref bounds)).xz, m_GizmosSystem.GetGizmosBatcher(ref val2))
		}, JobHandle.CombineDependencies(inputDeps, dependencies, val2));
		m_ZoneSearchSystem.AddSearchTreeReader(val);
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		return val;
	}

	private JobHandle AreaSearchTreeDebug(JobHandle inputDeps, Bounds3 bounds)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle val2 = default(JobHandle);
		JobHandle val = IJobExtensions.Schedule<NativeQuadTreeGizmoJob<AreaSearchItem, QuadTreeBoundsXZ, QuadTreeBoundsXZ.DebugIterator<AreaSearchItem>>>(new NativeQuadTreeGizmoJob<AreaSearchItem, QuadTreeBoundsXZ, QuadTreeBoundsXZ.DebugIterator<AreaSearchItem>>
		{
			m_Tree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies),
			m_Iterator = new QuadTreeBoundsXZ.DebugIterator<AreaSearchItem>(bounds, m_GizmosSystem.GetGizmosBatcher(ref val2))
		}, JobHandle.CombineDependencies(inputDeps, dependencies, val2));
		m_AreaSearchSystem.AddSearchTreeReader(val);
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		return val;
	}

	private JobHandle RouteSearchTreeDebug(JobHandle inputDeps, Bounds3 bounds)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle val2 = default(JobHandle);
		JobHandle val = IJobExtensions.Schedule<NativeQuadTreeGizmoJob<RouteSearchItem, QuadTreeBoundsXZ, QuadTreeBoundsXZ.DebugIterator<RouteSearchItem>>>(new NativeQuadTreeGizmoJob<RouteSearchItem, QuadTreeBoundsXZ, QuadTreeBoundsXZ.DebugIterator<RouteSearchItem>>
		{
			m_Tree = m_RouteSearchSystem.GetSearchTree(readOnly: true, out dependencies),
			m_Iterator = new QuadTreeBoundsXZ.DebugIterator<RouteSearchItem>(bounds, m_GizmosSystem.GetGizmosBatcher(ref val2))
		}, JobHandle.CombineDependencies(inputDeps, dependencies, val2));
		m_RouteSearchSystem.AddSearchTreeReader(val);
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		return val;
	}

	private JobHandle EffectSearchTreeDebug(JobHandle inputDeps, Bounds3 bounds)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle val2 = default(JobHandle);
		JobHandle val = IJobExtensions.Schedule<NativeQuadTreeGizmoJob<SourceInfo, QuadTreeBoundsXZ, QuadTreeBoundsXZ.DebugIterator<SourceInfo>>>(new NativeQuadTreeGizmoJob<SourceInfo, QuadTreeBoundsXZ, QuadTreeBoundsXZ.DebugIterator<SourceInfo>>
		{
			m_Tree = m_EffectSearchSystem.GetSearchTree(readOnly: true, out dependencies),
			m_Iterator = new QuadTreeBoundsXZ.DebugIterator<SourceInfo>(bounds, m_GizmosSystem.GetGizmosBatcher(ref val2))
		}, JobHandle.CombineDependencies(inputDeps, dependencies, val2));
		m_EffectSearchSystem.AddSearchTreeReader(val);
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		return val;
	}

	private JobHandle LocalEffectSearchTreeDebug(JobHandle inputDeps, Bounds3 bounds)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle val2 = default(JobHandle);
		JobHandle val = IJobExtensions.Schedule<NativeQuadTreeGizmoJob<LocalEffectSystem.EffectItem, LocalEffectSystem.EffectBounds, LocalEffectDebugIterator>>(new NativeQuadTreeGizmoJob<LocalEffectSystem.EffectItem, LocalEffectSystem.EffectBounds, LocalEffectDebugIterator>
		{
			m_Tree = m_LocalEffectSystem.GetSearchTree(readOnly: true, out dependencies),
			m_Iterator = new LocalEffectDebugIterator(((Bounds3)(ref bounds)).xz, m_GizmosSystem.GetGizmosBatcher(ref val2))
		}, JobHandle.CombineDependencies(inputDeps, dependencies, val2));
		m_LocalEffectSystem.AddLocalEffectReader(val);
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		return val;
	}

	[Preserve]
	public SearchTreeDebugSystem()
	{
	}
}
