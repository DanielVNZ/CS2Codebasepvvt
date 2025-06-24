using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Buildings;
using Game.Common;
using Game.Notifications;
using Game.Objects;
using Game.Prefabs;
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

namespace Game.Simulation;

[CompilerGenerated]
public class WaterPumpingStationAISystem : GameSystemBase
{
	[BurstCompile]
	public struct PumpTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public BufferTypeHandle<Game.Objects.SubObject> m_SubObjectType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public ComponentTypeHandle<WaterPipeBuildingConnection> m_BuildingConnectionType;

		[ReadOnly]
		public BufferTypeHandle<IconElement> m_IconElementType;

		public ComponentTypeHandle<Game.Buildings.WaterPumpingStation> m_WaterPumpingStationType;

		public ComponentTypeHandle<Game.Buildings.SewageOutlet> m_SewageOutletType;

		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<WaterPumpingStationData> m_PumpDatas;

		[ReadOnly]
		public ComponentLookup<Transform> m_Transforms;

		public ComponentLookup<WaterSourceData> m_WaterSources;

		public ComponentLookup<WaterPipeEdge> m_FlowEdges;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		public NativeArray<GroundWater> m_GroundWaterMap;

		public IconCommandBuffer m_IconCommandBuffer;

		public WaterPipeParameterData m_Parameters;

		public unsafe void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
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
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_056a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0584: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			BufferAccessor<Game.Objects.SubObject> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Objects.SubObject>(ref m_SubObjectType);
			BufferAccessor<InstalledUpgrade> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			NativeArray<WaterPipeBuildingConnection> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterPipeBuildingConnection>(ref m_BuildingConnectionType);
			BufferAccessor<IconElement> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<IconElement>(ref m_IconElementType);
			NativeArray<Game.Buildings.WaterPumpingStation> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.WaterPumpingStation>(ref m_WaterPumpingStationType);
			NativeArray<Game.Buildings.SewageOutlet> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.SewageOutlet>(ref m_SewageOutletType);
			BufferAccessor<Efficiency> bufferAccessor4 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			Span<float> factors = new Span<float>((void*)stackalloc byte[120], 30);
			WaterSourceData waterSourceData = default(WaterSourceData);
			Transform transform = default(Transform);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity entity = nativeArray[i];
				Entity prefab = nativeArray2[i].m_Prefab;
				WaterPipeBuildingConnection waterPipeBuildingConnection = nativeArray4[i];
				DynamicBuffer<IconElement> iconElements = ((bufferAccessor3.Length != 0) ? bufferAccessor3[i] : default(DynamicBuffer<IconElement>));
				ref Game.Buildings.WaterPumpingStation reference = ref CollectionUtils.ElementAt<Game.Buildings.WaterPumpingStation>(nativeArray5, i);
				WaterPumpingStationData data = m_PumpDatas[prefab];
				if (bufferAccessor2.Length != 0)
				{
					UpgradeUtils.CombineStats<WaterPumpingStationData>(ref data, bufferAccessor2[i], ref m_Prefabs, ref m_PumpDatas);
				}
				if (waterPipeBuildingConnection.m_ProducerEdge == Entity.Null)
				{
					Debug.LogError((object)"WaterPumpingStation is missing producer edge!");
					continue;
				}
				if (bufferAccessor4.Length != 0)
				{
					BuildingUtils.GetEfficiencyFactors(bufferAccessor4[i], factors);
					factors[19] = 1f;
				}
				else
				{
					factors.Fill(1f);
				}
				float efficiency = BuildingUtils.GetEfficiency(factors);
				WaterPipeEdge waterPipeEdge = m_FlowEdges[waterPipeBuildingConnection.m_ProducerEdge];
				reference.m_LastProduction = waterPipeEdge.m_FreshFlow;
				float num = reference.m_LastProduction;
				reference.m_Pollution = 0f;
				reference.m_Capacity = 0;
				int num2 = 0;
				if (nativeArray6.Length != 0)
				{
					ref Game.Buildings.SewageOutlet reference2 = ref CollectionUtils.ElementAt<Game.Buildings.SewageOutlet>(nativeArray6, i);
					num2 = reference2.m_LastPurified;
					reference2.m_UsedPurified = math.min(reference.m_LastProduction, reference2.m_LastPurified);
					num -= (float)reference2.m_UsedPurified;
				}
				float num3 = 0f;
				float num4 = 0f;
				bool flag = false;
				bool flag2 = false;
				if (data.m_Types != AllowedWaterTypes.None)
				{
					if ((data.m_Types & AllowedWaterTypes.Groundwater) != AllowedWaterTypes.None)
					{
						GroundWater groundWater = GroundWaterSystem.GetGroundWater(nativeArray3[i].m_Position, m_GroundWaterMap);
						float num5 = (float)groundWater.m_Polluted / math.max(1f, (float)groundWater.m_Amount);
						float num6 = (float)groundWater.m_Amount / m_Parameters.m_GroundwaterPumpEffectiveAmount;
						float num7 = math.clamp(num6 * (float)data.m_Capacity, 0f, (float)data.m_Capacity - num3);
						num3 += num7;
						num4 += num5 * num7;
						flag = num6 < 0.75f && (float)groundWater.m_Amount < 0.75f * (float)groundWater.m_Max;
						int num8 = (int)math.ceil(num * m_Parameters.m_GroundwaterUsageMultiplier);
						int num9 = math.min(num8, (int)groundWater.m_Amount);
						GroundWaterSystem.ConsumeGroundWater(nativeArray3[i].m_Position, m_GroundWaterMap, num9);
						num = Mathf.FloorToInt((float)(num8 - num9) / m_Parameters.m_GroundwaterUsageMultiplier);
					}
					if ((data.m_Types & AllowedWaterTypes.SurfaceWater) != AllowedWaterTypes.None && bufferAccessor.Length != 0)
					{
						DynamicBuffer<Game.Objects.SubObject> val = bufferAccessor[i];
						for (int j = 0; j < val.Length; j++)
						{
							Entity subObject = val[j].m_SubObject;
							if (m_WaterSources.TryGetComponent(subObject, ref waterSourceData) && m_Transforms.TryGetComponent(subObject, ref transform))
							{
								float surfaceWaterAvailability = GetSurfaceWaterAvailability(transform.m_Position, data.m_Types, m_WaterSurfaceData, m_Parameters.m_SurfaceWaterPumpEffectiveDepth);
								float num10 = WaterUtils.SamplePolluted(ref m_WaterSurfaceData, transform.m_Position);
								float num11 = math.clamp(surfaceWaterAvailability * (float)data.m_Capacity, 0f, (float)data.m_Capacity - num3);
								num3 += num11;
								num4 += num11 * num10;
								flag2 = surfaceWaterAvailability < 0.75f;
								waterSourceData.m_Amount = (0f - m_Parameters.m_SurfaceWaterUsageMultiplier) * num;
								waterSourceData.m_Polluted = 0f;
								m_WaterSources[subObject] = waterSourceData;
								num = 0f;
							}
						}
					}
				}
				else
				{
					num3 = data.m_Capacity;
					num4 = 0f;
					num = 0f;
				}
				reference.m_Capacity = (int)math.round(efficiency * num3 + (float)num2);
				reference.m_Pollution = ((reference.m_Capacity > 0) ? ((1f - data.m_Purification) * num4 / (float)reference.m_Capacity) : 0f);
				waterPipeEdge.m_FreshCapacity = reference.m_Capacity;
				waterPipeEdge.m_FreshPollution = ((reference.m_Capacity > 0) ? reference.m_Pollution : 0f);
				m_FlowEdges[waterPipeBuildingConnection.m_ProducerEdge] = waterPipeEdge;
				if (bufferAccessor4.Length != 0)
				{
					if (data.m_Capacity > 0)
					{
						float num12 = (num3 + (float)num2) / (float)(data.m_Capacity + num2);
						factors[19] = num12;
					}
					BuildingUtils.SetEfficiencyFactors(bufferAccessor4[i], factors);
				}
				bool flag3 = num3 < 0.1f * (float)data.m_Capacity;
				UpdateNotification(entity, m_Parameters.m_NotEnoughGroundwaterNotification, flag && flag3, iconElements);
				UpdateNotification(entity, m_Parameters.m_NotEnoughSurfaceWaterNotification, flag2 && flag3, iconElements);
				UpdateNotification(entity, m_Parameters.m_DirtyWaterPumpNotification, reference.m_Pollution > m_Parameters.m_MaxToleratedPollution, iconElements);
				bool flag4 = (waterPipeEdge.m_Flags & WaterPipeEdgeFlags.WaterShortage) != 0;
				UpdateNotification(entity, m_Parameters.m_NotEnoughWaterCapacityNotification, reference.m_Capacity > 0 && flag4, iconElements);
			}
		}

		private void UpdateNotification(Entity entity, Entity notificationPrefab, bool enabled, DynamicBuffer<IconElement> iconElements)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			bool flag = HasNotification(iconElements, notificationPrefab);
			if (enabled != flag)
			{
				if (enabled)
				{
					m_IconCommandBuffer.Add(entity, notificationPrefab);
				}
				else
				{
					m_IconCommandBuffer.Remove(entity, notificationPrefab);
				}
			}
		}

		private bool HasNotification(DynamicBuffer<IconElement> iconElements, Entity notificationPrefab)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			if (iconElements.IsCreated)
			{
				Enumerator<IconElement> enumerator = iconElements.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						IconElement current = enumerator.Current;
						if (m_Prefabs[current.m_Icon].m_Prefab == notificationPrefab)
						{
							return true;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
			return false;
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
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaterPipeBuildingConnection> __Game_Simulation_WaterPipeBuildingConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<IconElement> __Game_Notifications_IconElement_RO_BufferTypeHandle;

		public ComponentTypeHandle<Game.Buildings.WaterPumpingStation> __Game_Buildings_WaterPumpingStation_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Buildings.SewageOutlet> __Game_Buildings_SewageOutlet_RW_ComponentTypeHandle;

		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterPumpingStationData> __Game_Prefabs_WaterPumpingStationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		public ComponentLookup<WaterSourceData> __Game_Simulation_WaterSourceData_RW_ComponentLookup;

		public ComponentLookup<WaterPipeEdge> __Game_Simulation_WaterPipeEdge_RW_ComponentLookup;

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
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Objects_SubObject_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Objects.SubObject>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Simulation_WaterPipeBuildingConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterPipeBuildingConnection>(true);
			__Game_Notifications_IconElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<IconElement>(true);
			__Game_Buildings_WaterPumpingStation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.WaterPumpingStation>(false);
			__Game_Buildings_SewageOutlet_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.SewageOutlet>(false);
			__Game_Buildings_Efficiency_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(false);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_WaterPumpingStationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPumpingStationData>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Simulation_WaterSourceData_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterSourceData>(false);
			__Game_Simulation_WaterPipeEdge_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeEdge>(false);
		}
	}

	private GroundWaterSystem m_GroundWaterSystem;

	private WaterSystem m_WaterSystem;

	private IconCommandSystem m_IconCommandSystem;

	private EntityQuery m_PumpQuery;

	private EntityQuery m_ParameterQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 128;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 64;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GroundWaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundWaterSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_PumpQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadWrite<Game.Buildings.WaterPumpingStation>(),
			ComponentType.ReadOnly<WaterPipeBuildingConnection>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<WaterPipeParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_PumpQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_ParameterQuery);
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
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		JobHandle dependencies;
		PumpTickJob pumpTickJob = new PumpTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjectType = InternalCompilerInterface.GetBufferTypeHandle<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingConnectionType = InternalCompilerInterface.GetComponentTypeHandle<WaterPipeBuildingConnection>(ref __TypeHandle.__Game_Simulation_WaterPipeBuildingConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_IconElementType = InternalCompilerInterface.GetBufferTypeHandle<IconElement>(ref __TypeHandle.__Game_Notifications_IconElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPumpingStationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.WaterPumpingStation>(ref __TypeHandle.__Game_Buildings_WaterPumpingStation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SewageOutletType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.SewageOutlet>(ref __TypeHandle.__Game_Buildings_SewageOutlet_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PumpDatas = InternalCompilerInterface.GetComponentLookup<WaterPumpingStationData>(ref __TypeHandle.__Game_Prefabs_WaterPumpingStationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Transforms = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterSources = InternalCompilerInterface.GetComponentLookup<WaterSourceData>(ref __TypeHandle.__Game_Simulation_WaterSourceData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FlowEdges = InternalCompilerInterface.GetComponentLookup<WaterPipeEdge>(ref __TypeHandle.__Game_Simulation_WaterPipeEdge_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
			m_GroundWaterMap = m_GroundWaterSystem.GetMap(readOnly: false, out dependencies),
			m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer(),
			m_Parameters = ((EntityQuery)(ref m_ParameterQuery)).GetSingleton<WaterPipeParameterData>()
		};
		((SystemBase)this).Dependency = JobChunkExtensions.Schedule<PumpTickJob>(pumpTickJob, m_PumpQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps, dependencies));
		m_GroundWaterSystem.AddWriter(((SystemBase)this).Dependency);
		m_WaterSystem.AddSurfaceReader(((SystemBase)this).Dependency);
		m_IconCommandSystem.AddCommandBufferWriter(((SystemBase)this).Dependency);
	}

	public static float GetSurfaceWaterAvailability(float3 position, AllowedWaterTypes allowedTypes, WaterSurfaceData waterSurfaceData, float effectiveDepth)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return math.clamp(WaterUtils.SampleDepth(ref waterSurfaceData, position) / effectiveDepth, 0f, 1f);
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
	public WaterPumpingStationAISystem()
	{
	}
}
