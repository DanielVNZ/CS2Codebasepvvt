using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Buildings;

[CompilerGenerated]
public class WaterPoweredInitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct WaterPoweredInitializeJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Created> m_CreatedType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubNet> m_SubnetType;

		public ComponentTypeHandle<WaterPowered> m_WaterPoweredType;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabData;

		[ReadOnly]
		public ComponentLookup<PlaceableNetData> m_PlaceableNetData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_NetCompositionData;

		[ReadOnly]
		public ComponentLookup<TerrainComposition> m_TerrainCompositionData;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<WaterPowered> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterPowered>(ref m_WaterPoweredType);
			BufferAccessor<Game.Net.SubNet> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Net.SubNet>(ref m_SubnetType);
			if (((ArchetypeChunk)(ref chunk)).Has<Temp>(ref m_TempType) || ((ArchetypeChunk)(ref chunk)).Has<Created>(ref m_CreatedType))
			{
				for (int i = 0; i < nativeArray.Length; i++)
				{
					WaterPowered waterPowered = nativeArray[i];
					DynamicBuffer<Game.Net.SubNet> subNets = bufferAccessor[i];
					waterPowered.m_Length = 0f;
					waterPowered.m_Height = 0f;
					waterPowered.m_Estimate = 0f;
					CalculateWaterPowered(ref waterPowered, subNets);
					waterPowered.m_Height /= math.max(1f, waterPowered.m_Length);
					nativeArray[i] = waterPowered;
				}
			}
		}

		private void CalculateWaterPowered(ref WaterPowered waterPowered, DynamicBuffer<Game.Net.SubNet> subNets)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			Curve curve = default(Curve);
			Composition composition = default(Composition);
			PlaceableNetData placeableData = default(PlaceableNetData);
			NetCompositionData compositionData = default(NetCompositionData);
			TerrainComposition terrainComposition = default(TerrainComposition);
			for (int i = 0; i < subNets.Length; i++)
			{
				Entity subNet = subNets[i].m_SubNet;
				PrefabRef prefabRef = m_PrefabData[subNet];
				if (m_CurveData.TryGetComponent(subNet, ref curve) && m_CompositionData.TryGetComponent(subNet, ref composition) && m_PlaceableNetData.TryGetComponent(prefabRef.m_Prefab, ref placeableData) && m_NetCompositionData.TryGetComponent(composition.m_Edge, ref compositionData) && m_TerrainCompositionData.TryGetComponent(composition.m_Edge, ref terrainComposition) && (placeableData.m_PlacementFlags & (PlacementFlags.FlowLeft | PlacementFlags.FlowRight)) != PlacementFlags.None)
				{
					CalculateWaterPowered(ref waterPowered, curve, placeableData, compositionData, terrainComposition);
				}
			}
		}

		private void CalculateWaterPowered(ref WaterPowered waterPowered, Curve curve, PlaceableNetData placeableData, NetCompositionData compositionData, TerrainComposition terrainComposition)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			int num = math.max(1, Mathf.RoundToInt(curve.m_Length * m_WaterSurfaceData.scale.x));
			bool flag = (placeableData.m_PlacementFlags & PlacementFlags.FlowLeft) != 0;
			float num2 = 0f;
			float num3 = 0f;
			for (int i = 0; i < num; i++)
			{
				float num4 = ((float)i + 0.5f) / (float)num;
				float3 val = MathUtils.Position(curve.m_Bezier, num4);
				float3 val2 = MathUtils.Tangent(curve.m_Bezier, num4);
				float2 val3 = math.normalizesafe(math.select(MathUtils.Right(((float3)(ref val2)).xz), MathUtils.Left(((float3)(ref val2)).xz), flag), default(float2));
				val.y += compositionData.m_SurfaceHeight.min + terrainComposition.m_MaxHeightOffset.y;
				WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, val, out var terrainHeight, out var waterHeight, out var waterDepth);
				float2 val4 = WaterUtils.SampleVelocity(ref m_WaterSurfaceData, val);
				num2 += math.max(0f, val.y - terrainHeight);
				num3 += math.dot(val4, val3) * waterDepth * math.max(0f, val.y - waterHeight);
			}
			waterPowered.m_Length += curve.m_Length;
			waterPowered.m_Height += num2 * curve.m_Length / (float)num;
			waterPowered.m_Estimate += num3 * curve.m_Length / (float)num;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Created> __Game_Common_Created_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferTypeHandle;

		public ComponentTypeHandle<WaterPowered> __Game_Buildings_WaterPowered_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableNetData> __Game_Prefabs_PlaceableNetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TerrainComposition> __Game_Prefabs_TerrainComposition_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			__Game_Common_Created_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Created>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Net_SubNet_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Net.SubNet>(true);
			__Game_Buildings_WaterPowered_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterPowered>(false);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_PlaceableNetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableNetData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_TerrainComposition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TerrainComposition>(true);
		}
	}

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private EntityQuery m_WaterPoweredQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_WaterPoweredQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<WaterPowered>(),
			ComponentType.Exclude<ServiceUpgrade>(),
			ComponentType.Exclude<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_WaterPoweredQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		JobHandle val = JobChunkExtensions.ScheduleParallel<WaterPoweredInitializeJob>(new WaterPoweredInitializeJob
		{
			m_CreatedType = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubnetType = InternalCompilerInterface.GetBufferTypeHandle<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPoweredType = InternalCompilerInterface.GetComponentTypeHandle<WaterPowered>(ref __TypeHandle.__Game_Buildings_WaterPowered_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceableNetData = InternalCompilerInterface.GetComponentLookup<PlaceableNetData>(ref __TypeHandle.__Game_Prefabs_PlaceableNetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TerrainCompositionData = InternalCompilerInterface.GetComponentLookup<TerrainComposition>(ref __TypeHandle.__Game_Prefabs_TerrainComposition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_WaterSurfaceData = m_WaterSystem.GetVelocitiesSurfaceData(out deps)
		}, m_WaterPoweredQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		m_TerrainSystem.AddCPUHeightReader(val);
		m_WaterSystem.AddSurfaceReader(val);
		((SystemBase)this).Dependency = val;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public WaterPoweredInitializeSystem()
	{
	}
}
