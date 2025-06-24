using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Prefabs;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Net;

[CompilerGenerated]
public class NetComponentsSystem : GameSystemBase
{
	[BurstCompile]
	private struct CheckNodeComponentsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<TrafficLights> m_TrafficLightsType;

		[ReadOnly]
		public ComponentTypeHandle<Composition> m_CompositionType;

		[ReadOnly]
		public BufferTypeHandle<ConnectedEdge> m_ConnectedEdgeType;

		public ComponentTypeHandle<Roundabout> m_RoundaboutType;

		public ComponentTypeHandle<Gate> m_GateType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<TrafficLights> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TrafficLights>(ref m_TrafficLightsType);
			NativeArray<Composition> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Composition>(ref m_CompositionType);
			NativeArray<Roundabout> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Roundabout>(ref m_RoundaboutType);
			NativeArray<Gate> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Gate>(ref m_GateType);
			BufferAccessor<ConnectedEdge> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedEdge>(ref m_ConnectedEdgeType);
			Roundabout roundabout = default(Roundabout);
			Composition composition = default(Composition);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				Entity val = nativeArray[i];
				DynamicBuffer<ConnectedEdge> val2 = bufferAccessor[i];
				CompositionFlags compositionFlags = default(CompositionFlags);
				CompositionFlags compositionFlags2 = default(CompositionFlags);
				if (nativeArray3.Length != 0)
				{
					if ((nativeArray3[i].m_Flags & TrafficLightFlags.LevelCrossing) != 0)
					{
						compositionFlags.m_General |= CompositionFlags.General.LevelCrossing;
					}
					else
					{
						compositionFlags.m_General |= CompositionFlags.General.TrafficLights;
					}
				}
				if (CollectionUtils.TryGet<Roundabout>(nativeArray5, i, ref roundabout))
				{
					compositionFlags.m_General |= CompositionFlags.General.Roundabout;
				}
				roundabout.m_Radius = 0f;
				for (int j = 0; j < val2.Length; j++)
				{
					Entity edge = val2[j].m_Edge;
					Edge edge2 = m_EdgeData[edge];
					bool flag = edge2.m_Start == val;
					bool flag2 = edge2.m_End == val;
					if ((flag || flag2) && m_CompositionData.TryGetComponent(edge, ref composition))
					{
						NetCompositionData netCompositionData = m_PrefabCompositionData[flag ? composition.m_StartNode : composition.m_EndNode];
						compositionFlags2 |= netCompositionData.m_Flags;
						if ((netCompositionData.m_Flags.m_General & CompositionFlags.General.Roundabout) != 0)
						{
							EdgeNodeGeometry edgeNodeGeometry = ((!flag) ? m_EndNodeGeometryData[edge].m_Geometry : m_StartNodeGeometryData[edge].m_Geometry);
							float num = math.select(netCompositionData.m_RoundaboutSize.x, netCompositionData.m_RoundaboutSize.y, flag2);
							num += edgeNodeGeometry.m_MiddleRadius;
							roundabout.m_Radius = math.max(roundabout.m_Radius, num);
						}
					}
				}
				CompositionFlags compositionFlags3 = compositionFlags ^ compositionFlags2;
				if ((compositionFlags3.m_General & (CompositionFlags.General.LevelCrossing | CompositionFlags.General.TrafficLights)) != 0)
				{
					if ((compositionFlags2.m_General & CompositionFlags.General.LevelCrossing) != 0)
					{
						if ((compositionFlags.m_General & CompositionFlags.General.TrafficLights) != 0)
						{
							TrafficLights trafficLights = nativeArray3[i];
							trafficLights.m_Flags |= TrafficLightFlags.LevelCrossing;
							((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TrafficLights>(unfilteredChunkIndex, val, trafficLights);
						}
						else
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TrafficLights>(unfilteredChunkIndex, val, new TrafficLights
							{
								m_Flags = TrafficLightFlags.LevelCrossing
							});
						}
					}
					else if ((compositionFlags2.m_General & CompositionFlags.General.TrafficLights) != 0)
					{
						if ((compositionFlags.m_General & CompositionFlags.General.LevelCrossing) != 0)
						{
							TrafficLights trafficLights2 = nativeArray3[i];
							trafficLights2.m_Flags &= ~TrafficLightFlags.LevelCrossing;
							((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TrafficLights>(unfilteredChunkIndex, val, trafficLights2);
						}
						else
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TrafficLights>(unfilteredChunkIndex, val, default(TrafficLights));
						}
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TrafficLights>(unfilteredChunkIndex, val);
					}
				}
				if ((compositionFlags3.m_General & CompositionFlags.General.Roundabout) != 0)
				{
					if ((compositionFlags2.m_General & CompositionFlags.General.Roundabout) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Roundabout>(unfilteredChunkIndex, val, roundabout);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Roundabout>(unfilteredChunkIndex, val);
					}
				}
				CollectionUtils.TrySet<Roundabout>(nativeArray5, i, roundabout);
			}
			Gate gate = default(Gate);
			Owner owner = default(Owner);
			Owner owner2 = default(Owner);
			for (int k = 0; k < nativeArray4.Length; k++)
			{
				Entity val3 = nativeArray[k];
				Composition composition2 = nativeArray4[k];
				CompositionFlags compositionFlags4 = default(CompositionFlags);
				CompositionFlags compositionFlags5 = default(CompositionFlags);
				if (CollectionUtils.TryGet<Gate>(nativeArray6, k, ref gate))
				{
					compositionFlags4.m_General |= CompositionFlags.General.Roundabout;
				}
				gate.m_Domain = Entity.Null;
				NetCompositionData netCompositionData2 = m_PrefabCompositionData[composition2.m_Edge];
				compositionFlags5 = netCompositionData2.m_Flags;
				if (((netCompositionData2.m_Flags.m_Left | netCompositionData2.m_Flags.m_Right) & CompositionFlags.Side.Gate) != 0 && CollectionUtils.TryGet<Owner>(nativeArray2, k, ref owner))
				{
					while (m_OwnerData.TryGetComponent(owner.m_Owner, ref owner2))
					{
						owner = owner2;
					}
					gate.m_Domain = owner.m_Owner;
				}
				CompositionFlags compositionFlags6 = compositionFlags4 ^ compositionFlags5;
				if (((compositionFlags6.m_Left | compositionFlags6.m_Right) & CompositionFlags.Side.Gate) != 0)
				{
					if (((compositionFlags5.m_Left | compositionFlags5.m_Right) & CompositionFlags.Side.Gate) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Gate>(unfilteredChunkIndex, val3, gate);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Gate>(unfilteredChunkIndex, val3);
					}
				}
				CollectionUtils.TrySet<Gate>(nativeArray6, k, gate);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TrafficLights> __Game_Net_TrafficLights_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Composition> __Game_Net_Composition_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferTypeHandle;

		public ComponentTypeHandle<Roundabout> __Game_Net_Roundabout_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Gate> __Game_Net_Gate_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Net_TrafficLights_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrafficLights>(true);
			__Game_Net_Composition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Composition>(true);
			__Game_Net_ConnectedEdge_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedEdge>(true);
			__Game_Net_Roundabout_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Roundabout>(false);
			__Game_Net_Gate_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Gate>(false);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
		}
	}

	private ModificationBarrier4 m_ModificationBarrier;

	private EntityQuery m_UpdatedNetQuery;

	private EntityQuery m_AllNetQuery;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Updated>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadOnly<Edge>()
		};
		array[0] = val;
		m_UpdatedNetQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadOnly<Edge>()
		};
		array2[0] = val;
		m_AllNetQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		m_Loaded = true;
	}

	private bool GetLoaded()
	{
		if (m_Loaded)
		{
			m_Loaded = false;
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val = (GetLoaded() ? m_AllNetQuery : m_UpdatedNetQuery);
		if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
		{
			CheckNodeComponentsJob checkNodeComponentsJob = new CheckNodeComponentsJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TrafficLightsType = InternalCompilerInterface.GetComponentTypeHandle<TrafficLights>(ref __TypeHandle.__Game_Net_TrafficLights_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CompositionType = InternalCompilerInterface.GetComponentTypeHandle<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedEdgeType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RoundaboutType = InternalCompilerInterface.GetComponentTypeHandle<Roundabout>(ref __TypeHandle.__Game_Net_Roundabout_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_GateType = InternalCompilerInterface.GetComponentTypeHandle<Gate>(ref __TypeHandle.__Game_Net_Gate_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			EntityCommandBuffer val2 = m_ModificationBarrier.CreateCommandBuffer();
			checkNodeComponentsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			JobHandle val3 = JobChunkExtensions.ScheduleParallel<CheckNodeComponentsJob>(checkNodeComponentsJob, val, ((SystemBase)this).Dependency);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val3);
			((SystemBase)this).Dependency = val3;
		}
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
	public NetComponentsSystem()
	{
	}
}
