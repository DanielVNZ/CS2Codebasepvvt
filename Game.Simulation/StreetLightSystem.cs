using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Rendering;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class StreetLightSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateStreetLightsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PseudoRandomSeed> m_PseudoRandomSeedType;

		[ReadOnly]
		public BufferTypeHandle<SubObject> m_SubObjectType;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityNodeConnection> m_ElectricityNodeConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityConsumer> m_ElectricityConsumerType;

		public ComponentTypeHandle<Road> m_RoadType;

		public ComponentTypeHandle<Building> m_BuildingType;

		public ComponentTypeHandle<Watercraft> m_WatercraftType;

		[ReadOnly]
		public BufferLookup<SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> m_ConnectedFlowEdges;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowEdge> m_ElectricityFlowEdges;

		[ReadOnly]
		public ComponentLookup<ElectricityNodeConnection> m_ElectricityNodeConnections;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<StreetLight> m_StreetLightData;

		[ReadOnly]
		public int m_Brightness;

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
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityTypeHandle);
			NativeArray<Road> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Road>(ref m_RoadType);
			NativeArray<ElectricityNodeConnection> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityNodeConnection>(ref m_ElectricityNodeConnectionType);
			NativeArray<Building> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Building>(ref m_BuildingType);
			NativeArray<ElectricityConsumer> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityConsumer>(ref m_ElectricityConsumerType);
			NativeArray<Watercraft> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Watercraft>(ref m_WatercraftType);
			NativeArray<PseudoRandomSeed> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PseudoRandomSeed>(ref m_PseudoRandomSeedType);
			BufferAccessor<SubObject> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubObject>(ref m_SubObjectType);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				ref Road reference = ref CollectionUtils.ElementAt<Road>(nativeArray2, i);
				if ((reference.m_Flags & RoadFlags.IsLit) == 0)
				{
					continue;
				}
				Random random = nativeArray7[i].GetRandom(PseudoRandomSeed.kBrightnessLimit);
				bool flag = IsElectricityConnected(nativeArray3, i) && (m_Brightness < ((Random)(ref random)).NextInt(200, 300) || (reference.m_Flags & RoadFlags.AlwaysLit) != 0);
				if (flag == ((reference.m_Flags & RoadFlags.LightsOff) != 0))
				{
					if (flag)
					{
						reference.m_Flags &= ~RoadFlags.LightsOff;
					}
					else
					{
						reference.m_Flags |= RoadFlags.LightsOff;
					}
					DynamicBuffer<SubObject> subObjects = bufferAccessor[i];
					UpdateStreetLightObjects(unfilteredChunkIndex, subObjects, reference);
				}
			}
			for (int j = 0; j < nativeArray4.Length; j++)
			{
				ref Building reference2 = ref CollectionUtils.ElementAt<Building>(nativeArray4, j);
				Random random2 = nativeArray7[j].GetRandom(PseudoRandomSeed.kBrightnessLimit);
				bool flag2 = IsElectricityConnected(nativeArray5, j, in reference2);
				bool flag3 = flag2 && m_Brightness < ((Random)(ref random2)).NextInt(200, 300);
				if (flag3 == ((reference2.m_Flags & BuildingFlags.StreetLightsOff) != 0))
				{
					if (flag3)
					{
						reference2.m_Flags &= ~BuildingFlags.StreetLightsOff;
					}
					else
					{
						reference2.m_Flags |= BuildingFlags.StreetLightsOff;
					}
					DynamicBuffer<SubObject> subObjects2 = bufferAccessor[j];
					UpdateStreetLightObjects(unfilteredChunkIndex, subObjects2, reference2);
				}
				if (flag2 != ((reference2.m_Flags & BuildingFlags.Illuminated) != 0))
				{
					if (flag2)
					{
						reference2.m_Flags |= BuildingFlags.Illuminated;
					}
					else
					{
						reference2.m_Flags &= ~BuildingFlags.Illuminated;
					}
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(unfilteredChunkIndex, nativeArray[j]);
				}
			}
			for (int k = 0; k < nativeArray6.Length; k++)
			{
				ref Watercraft reference3 = ref CollectionUtils.ElementAt<Watercraft>(nativeArray6, k);
				Random random3 = nativeArray7[k].GetRandom(PseudoRandomSeed.kBrightnessLimit);
				bool flag4 = (m_Brightness < ((Random)(ref random3)).NextInt(200, 300)) & ((reference3.m_Flags & WatercraftFlags.DeckLights) != 0);
				if (flag4 == ((reference3.m_Flags & WatercraftFlags.LightsOff) != 0))
				{
					if (flag4)
					{
						reference3.m_Flags &= ~WatercraftFlags.LightsOff;
					}
					else
					{
						reference3.m_Flags |= WatercraftFlags.LightsOff;
					}
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(unfilteredChunkIndex, nativeArray[k], default(EffectsUpdated));
					if (bufferAccessor.Length != 0)
					{
						DynamicBuffer<SubObject> subObjects3 = bufferAccessor[k];
						UpdateStreetLightObjects(unfilteredChunkIndex, subObjects3, reference3);
					}
				}
			}
		}

		private bool IsElectricityConnected(NativeArray<ElectricityNodeConnection> nodeConnections, int i)
		{
			if (nodeConnections.Length == 0)
			{
				return true;
			}
			return IsElectricityConnected(nodeConnections[i]);
		}

		private bool IsElectricityConnected(in ElectricityNodeConnection nodeConnection)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			Entity electricityNode = nodeConnection.m_ElectricityNode;
			DynamicBuffer<ConnectedFlowEdge> val = m_ConnectedFlowEdges[electricityNode];
			bool flag = false;
			Enumerator<ConnectedFlowEdge> enumerator = val.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ConnectedFlowEdge current = enumerator.Current;
					flag |= m_ElectricityFlowEdges[(Entity)current].isDisconnected;
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			return !flag;
		}

		private bool IsElectricityConnected(NativeArray<ElectricityConsumer> consumers, int i, in Building building)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (consumers.Length != 0)
			{
				return consumers[i].electricityConnected;
			}
			ElectricityNodeConnection nodeConnection = default(ElectricityNodeConnection);
			if (m_ElectricityNodeConnections.TryGetComponent(building.m_RoadEdge, ref nodeConnection))
			{
				return IsElectricityConnected(in nodeConnection);
			}
			return true;
		}

		private void UpdateStreetLightObjects(int jobIndex, DynamicBuffer<SubObject> subObjects, Road road)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			StreetLight streetLight = default(StreetLight);
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (m_StreetLightData.TryGetComponent(subObject, ref streetLight))
				{
					UpdateStreetLightState(ref streetLight, road);
					m_StreetLightData[subObject] = streetLight;
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(jobIndex, subObject, default(EffectsUpdated));
				}
			}
		}

		private void UpdateStreetLightObjects(int jobIndex, DynamicBuffer<SubObject> subObjects, Building building)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			StreetLight streetLight = default(StreetLight);
			DynamicBuffer<SubObject> subObjects2 = default(DynamicBuffer<SubObject>);
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (m_StreetLightData.TryGetComponent(subObject, ref streetLight))
				{
					UpdateStreetLightState(ref streetLight, building);
					m_StreetLightData[subObject] = streetLight;
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(jobIndex, subObject, default(EffectsUpdated));
				}
				if (m_SubObjects.TryGetBuffer(subObject, ref subObjects2))
				{
					UpdateStreetLightObjects(jobIndex, subObjects2, building);
				}
			}
		}

		private void UpdateStreetLightObjects(int jobIndex, DynamicBuffer<SubObject> subObjects, Watercraft watercraft)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			StreetLight streetLight = default(StreetLight);
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (m_StreetLightData.TryGetComponent(subObject, ref streetLight))
				{
					UpdateStreetLightState(ref streetLight, watercraft);
					m_StreetLightData[subObject] = streetLight;
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(jobIndex, subObject, default(EffectsUpdated));
				}
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
		public ComponentTypeHandle<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<SubObject> __Game_Objects_SubObject_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityNodeConnection> __Game_Simulation_ElectricityNodeConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Road> __Game_Net_Road_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Building> __Game_Buildings_Building_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Watercraft> __Game_Vehicles_Watercraft_RW_ComponentTypeHandle;

		[ReadOnly]
		public BufferLookup<SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> __Game_Simulation_ConnectedFlowEdge_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowEdge> __Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityNodeConnection> __Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup;

		public ComponentLookup<StreetLight> __Game_Objects_StreetLight_RW_ComponentLookup;

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
			__Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PseudoRandomSeed>(true);
			__Game_Objects_SubObject_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubObject>(true);
			__Game_Simulation_ElectricityNodeConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityNodeConnection>(true);
			__Game_Buildings_ElectricityConsumer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityConsumer>(true);
			__Game_Net_Road_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Road>(false);
			__Game_Buildings_Building_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(false);
			__Game_Vehicles_Watercraft_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Watercraft>(false);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubObject>(true);
			__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedFlowEdge>(true);
			__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityFlowEdge>(true);
			__Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityNodeConnection>(true);
			__Game_Objects_StreetLight_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StreetLight>(false);
		}
	}

	private const uint UPDATE_INTERVAL = 256u;

	private SimulationSystem m_SimulationSystem;

	private LightingSystem m_LightingSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_StreetLightQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_LightingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LightingSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<UpdateFrame>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Road>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Watercraft>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_StreetLightQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_StreetLightQuery);
		Assert.AreEqual(16, 16);
		Assert.AreEqual(16, 16);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		((EntityQuery)(ref m_StreetLightQuery)).ResetFilter();
		((EntityQuery)(ref m_StreetLightQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(SimulationUtils.GetUpdateFrameWithInterval(m_SimulationSystem.frameIndex, (uint)GetUpdateInterval(SystemUpdatePhase.GameSimulation), 16)));
		UpdateStreetLightsJob updateStreetLightsJob = new UpdateStreetLightsJob
		{
			m_EntityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PseudoRandomSeedType = InternalCompilerInterface.GetComponentTypeHandle<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjectType = InternalCompilerInterface.GetBufferTypeHandle<SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityNodeConnectionType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityNodeConnection>(ref __TypeHandle.__Game_Simulation_ElectricityNodeConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityConsumerType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RoadType = InternalCompilerInterface.GetComponentTypeHandle<Road>(ref __TypeHandle.__Game_Net_Road_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftType = InternalCompilerInterface.GetComponentTypeHandle<Watercraft>(ref __TypeHandle.__Game_Vehicles_Watercraft_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedFlowEdges = InternalCompilerInterface.GetBufferLookup<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityFlowEdges = InternalCompilerInterface.GetComponentLookup<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityNodeConnections = InternalCompilerInterface.GetComponentLookup<ElectricityNodeConnection>(ref __TypeHandle.__Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StreetLightData = InternalCompilerInterface.GetComponentLookup<StreetLight>(ref __TypeHandle.__Game_Objects_StreetLight_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Brightness = Mathf.RoundToInt(m_LightingSystem.dayLightBrightness * 1000f)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		updateStreetLightsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<UpdateStreetLightsJob>(updateStreetLightsJob, m_StreetLightQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		((SystemBase)this).Dependency = val2;
	}

	public static void UpdateStreetLightState(ref StreetLight streetLight, Road road)
	{
		if ((road.m_Flags & RoadFlags.LightsOff) != 0)
		{
			streetLight.m_State |= StreetLightState.TurnedOff;
		}
		else
		{
			streetLight.m_State &= ~StreetLightState.TurnedOff;
		}
	}

	public static void UpdateStreetLightState(ref StreetLight streetLight, Building building)
	{
		if ((building.m_Flags & BuildingFlags.StreetLightsOff) != BuildingFlags.None)
		{
			streetLight.m_State |= StreetLightState.TurnedOff;
		}
		else
		{
			streetLight.m_State &= ~StreetLightState.TurnedOff;
		}
	}

	public static void UpdateStreetLightState(ref StreetLight streetLight, Watercraft watercraft)
	{
		if ((watercraft.m_Flags & WatercraftFlags.LightsOff) != 0)
		{
			streetLight.m_State |= StreetLightState.TurnedOff;
		}
		else
		{
			streetLight.m_State &= ~StreetLightState.TurnedOff;
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
	public StreetLightSystem()
	{
	}
}
