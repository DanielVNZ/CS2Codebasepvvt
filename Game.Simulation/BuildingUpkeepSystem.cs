using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Net;
using Game.Notifications;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Game.Triggers;
using Game.Vehicles;
using Game.Zones;
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
public class BuildingUpkeepSystem : GameSystemBase
{
	private struct UpkeepPayment
	{
		public Entity m_RenterEntity;

		public int m_Price;
	}

	[BurstCompile]
	private struct BuildingUpkeepJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public ComponentTypeHandle<BuildingCondition> m_ConditionType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[ReadOnly]
		public BufferTypeHandle<Renter> m_RenterType;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingDatas;

		[ReadOnly]
		public ComponentLookup<ZoneData> m_ZoneDatas;

		[ReadOnly]
		public BufferLookup<Resources> m_Resources;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDatas;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_BuildingPropertyDatas;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifierBufs;

		[ReadOnly]
		public ComponentLookup<Abandoned> m_Abandoned;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_Destroyed;

		[ReadOnly]
		public ComponentLookup<SignatureBuildingData> m_SignatureDatas;

		[ReadOnly]
		public ComponentLookup<Household> m_Households;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> m_OwnedVehicles;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_DeliveryTrucks;

		[ReadOnly]
		public BuildingConfigurationData m_BuildingConfigurationData;

		[ReadOnly]
		public ComponentLookup<ConsumptionData> m_ConsumptionDatas;

		[ReadOnly]
		public BufferLookup<ResourceAvailability> m_Availabilities;

		[ReadOnly]
		public Entity m_City;

		public uint m_UpdateFrameIndex;

		public uint m_SimulationFrame;

		public float m_TemperatureUpkeep;

		public bool m_DebugFastLeveling;

		public ParallelWriter<UpkeepPayment> m_UpkeepExpenseQueue;

		public ParallelWriter<Entity> m_LevelupQueue;

		public ParallelWriter<Entity> m_LevelDownQueue;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			NativeArray<BuildingCondition> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<BuildingCondition>(ref m_ConditionType);
			((ArchetypeChunk)(ref chunk)).GetNativeArray<Building>(ref m_BuildingType);
			BufferAccessor<Renter> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
			DynamicBuffer<Resources> resources = default(DynamicBuffer<Resources>);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				int num = 0;
				_ = nativeArray[i];
				Entity prefab = nativeArray2[i].m_Prefab;
				ConsumptionData consumptionData = m_ConsumptionDatas[prefab];
				_ = m_BuildingDatas[prefab];
				_ = m_BuildingPropertyDatas[prefab];
				DynamicBuffer<CityModifier> cityEffects = m_CityModifierBufs[m_City];
				SpawnableBuildingData spawnableBuildingData = m_SpawnableBuildingDatas[prefab];
				AreaType areaType = m_ZoneDatas[spawnableBuildingData.m_ZonePrefab].m_AreaType;
				BuildingPropertyData propertyData = m_BuildingPropertyDatas[prefab];
				int levelingCost = BuildingUtils.GetLevelingCost(areaType, propertyData, spawnableBuildingData.m_Level, cityEffects);
				int num2 = ((spawnableBuildingData.m_Level == 5) ? BuildingUtils.GetLevelingCost(areaType, propertyData, 4, cityEffects) : levelingCost);
				if (areaType == AreaType.Residential && propertyData.m_ResidentialProperties > 1)
				{
					num2 = Mathf.RoundToInt((float)(num2 * (6 - spawnableBuildingData.m_Level)) / math.sqrt((float)propertyData.m_ResidentialProperties));
				}
				DynamicBuffer<Renter> val = bufferAccessor[i];
				int num3 = consumptionData.m_Upkeep / kUpdatesPerDay;
				int num4 = num3 / kMaterialUpkeep;
				num += num3 - num4;
				int num5 = 0;
				for (int j = 0; j < val.Length; j++)
				{
					if (m_Resources.TryGetBuffer(val[j].m_Renter, ref resources))
					{
						num5 = ((!m_Households.HasComponent(val[j].m_Renter)) ? ((!m_OwnedVehicles.HasBuffer(val[j].m_Renter)) ? (num5 + EconomyUtils.GetCompanyTotalWorth(resources, m_ResourcePrefabs, ref m_ResourceDatas)) : (num5 + EconomyUtils.GetCompanyTotalWorth(resources, m_OwnedVehicles[val[j].m_Renter], ref m_LayoutElements, ref m_DeliveryTrucks, m_ResourcePrefabs, ref m_ResourceDatas))) : (num5 + EconomyUtils.GetResources(Resource.Money, resources)));
					}
				}
				BuildingCondition buildingCondition = nativeArray3[i];
				int num6 = 0;
				if (num > num5)
				{
					num6 = -m_BuildingConfigurationData.m_BuildingConditionDecrement * (int)math.pow(2f, (float)(int)spawnableBuildingData.m_Level) * math.max(1, val.Length);
				}
				else if (val.Length > 0)
				{
					num6 = m_BuildingConfigurationData.m_BuildingConditionIncrement * (int)math.pow(2f, (float)(int)spawnableBuildingData.m_Level) * math.max(1, val.Length);
					int num7 = num / val.Length;
					for (int k = 0; k < val.Length; k++)
					{
						m_UpkeepExpenseQueue.Enqueue(new UpkeepPayment
						{
							m_RenterEntity = val[k].m_Renter,
							m_Price = -num7
						});
					}
				}
				if (m_DebugFastLeveling)
				{
					buildingCondition.m_Condition = levelingCost;
				}
				else
				{
					buildingCondition.m_Condition += num6;
				}
				if (buildingCondition.m_Condition >= levelingCost)
				{
					m_LevelupQueue.Enqueue(nativeArray[i]);
					buildingCondition.m_Condition -= levelingCost;
				}
				if (!m_Abandoned.HasComponent(nativeArray[i]) && !m_Destroyed.HasComponent(nativeArray[i]) && nativeArray3[i].m_Condition <= -num2 && !m_SignatureDatas.HasComponent(prefab))
				{
					m_LevelDownQueue.Enqueue(nativeArray[i]);
					buildingCondition.m_Condition += levelingCost;
				}
				nativeArray3[i] = buildingCondition;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpkeepPaymentJob : IJob
	{
		public BufferLookup<Resources> m_Resources;

		public NativeQueue<UpkeepPayment> m_UpkeepExpenseQueue;

		public void Execute()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			UpkeepPayment upkeepPayment = default(UpkeepPayment);
			while (m_UpkeepExpenseQueue.TryDequeue(ref upkeepPayment))
			{
				if (m_Resources.HasBuffer(upkeepPayment.m_RenterEntity))
				{
					EconomyUtils.AddResources(Resource.Money, upkeepPayment.m_Price, m_Resources[upkeepPayment.m_RenterEntity]);
				}
			}
		}
	}

	[BurstCompile]
	private struct LeveldownJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildings;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDatas;

		public ComponentLookup<Building> m_Buildings;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> m_ElectricityConsumers;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> m_WaterConsumers;

		[ReadOnly]
		public ComponentLookup<GarbageProducer> m_GarbageProducers;

		[ReadOnly]
		public ComponentLookup<MailProducer> m_MailProducers;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_BuildingPropertyDatas;

		[ReadOnly]
		public ComponentLookup<OfficeBuilding> m_OfficeBuilding;

		public NativeQueue<TriggerAction> m_TriggerBuffer;

		public ComponentLookup<CrimeProducer> m_CrimeProducers;

		public BufferLookup<Renter> m_Renters;

		[ReadOnly]
		public BuildingConfigurationData m_BuildingConfigurationData;

		public NativeQueue<Entity> m_LeveldownQueue;

		public EntityCommandBuffer m_CommandBuffer;

		public NativeQueue<Entity> m_UpdatedElectricityRoadEdges;

		public NativeQueue<Entity> m_UpdatedWaterPipeRoadEdges;

		public IconCommandBuffer m_IconCommandBuffer;

		public uint m_SimulationFrame;

		public void Execute()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			Entity val = default(Entity);
			while (m_LeveldownQueue.TryDequeue(ref val))
			{
				if (!m_Prefabs.HasComponent(val))
				{
					continue;
				}
				Entity prefab = m_Prefabs[val].m_Prefab;
				if (!m_SpawnableBuildings.HasComponent(prefab))
				{
					continue;
				}
				_ = m_SpawnableBuildings[prefab];
				_ = m_BuildingDatas[prefab];
				BuildingPropertyData buildingPropertyData = m_BuildingPropertyDatas[prefab];
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Abandoned>(val, new Abandoned
				{
					m_AbandonmentTime = m_SimulationFrame
				});
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
				if (m_ElectricityConsumers.HasComponent(val))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<ElectricityConsumer>(val);
					Entity roadEdge = m_Buildings[val].m_RoadEdge;
					if (roadEdge != Entity.Null)
					{
						m_UpdatedElectricityRoadEdges.Enqueue(roadEdge);
					}
				}
				if (m_WaterConsumers.HasComponent(val))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<WaterConsumer>(val);
					Entity roadEdge2 = m_Buildings[val].m_RoadEdge;
					if (roadEdge2 != Entity.Null)
					{
						m_UpdatedWaterPipeRoadEdges.Enqueue(roadEdge2);
					}
				}
				if (m_GarbageProducers.HasComponent(val))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<GarbageProducer>(val);
				}
				if (m_MailProducers.HasComponent(val))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<MailProducer>(val);
				}
				if (m_CrimeProducers.HasComponent(val))
				{
					CrimeProducer crimeProducer = m_CrimeProducers[val];
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<CrimeProducer>(val, new CrimeProducer
					{
						m_Crime = crimeProducer.m_Crime * 2f,
						m_PatrolRequest = crimeProducer.m_PatrolRequest
					});
				}
				if (m_Renters.HasBuffer(val))
				{
					DynamicBuffer<Renter> val2 = m_Renters[val];
					for (int num = val2.Length - 1; num >= 0; num--)
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<PropertyRenter>(val2[num].m_Renter);
						val2.RemoveAt(num);
					}
				}
				if ((m_Buildings[val].m_Flags & Game.Buildings.BuildingFlags.HighRentWarning) != Game.Buildings.BuildingFlags.None)
				{
					Building building = m_Buildings[val];
					m_IconCommandBuffer.Remove(val, m_BuildingConfigurationData.m_HighRentNotification);
					building.m_Flags &= ~Game.Buildings.BuildingFlags.HighRentWarning;
					m_Buildings[val] = building;
				}
				m_IconCommandBuffer.Remove(val, IconPriority.Problem);
				m_IconCommandBuffer.Remove(val, IconPriority.FatalProblem);
				m_IconCommandBuffer.Add(val, m_BuildingConfigurationData.m_AbandonedNotification, IconPriority.FatalProblem);
				if (buildingPropertyData.CountProperties(AreaType.Commercial) > 0)
				{
					m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.LevelDownCommercialBuilding, Entity.Null, val, val));
				}
				if (buildingPropertyData.CountProperties(AreaType.Industrial) > 0)
				{
					if (m_OfficeBuilding.HasComponent(prefab))
					{
						m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.LevelDownOfficeBuilding, Entity.Null, val, val));
					}
					else
					{
						m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.LevelDownIndustrialBuilding, Entity.Null, val, val));
					}
				}
			}
		}
	}

	[BurstCompile]
	private struct LevelupJob : IJob
	{
		private struct Iterator : INativeQuadTreeIterator<Entity, Bounds2>, IUnsafeQuadTreeIterator<Entity, Bounds2>
		{
			public Bounds2 m_Bounds;

			public int2 m_LotSize;

			public float2 m_StartPosition;

			public float2 m_Right;

			public float2 m_Forward;

			public int m_MaxHeight;

			public ComponentLookup<Block> m_BlockData;

			public ComponentLookup<ValidArea> m_ValidAreaData;

			public BufferLookup<Cell> m_Cells;

			public bool Intersect(Bounds2 bounds)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds, m_Bounds);
			}

			public void Iterate(Bounds2 bounds, Entity blockEntity)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_0145: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0115: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				//IL_012e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0133: Unknown result type (might be due to invalid IL or missing references)
				//IL_0138: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(bounds, m_Bounds))
				{
					return;
				}
				ValidArea validArea = m_ValidAreaData[blockEntity];
				if (validArea.m_Area.y <= validArea.m_Area.x)
				{
					return;
				}
				Block block = m_BlockData[blockEntity];
				DynamicBuffer<Cell> val = m_Cells[blockEntity];
				float2 val2 = m_StartPosition;
				int2 val3 = default(int2);
				val3.y = 0;
				while (val3.y < m_LotSize.y)
				{
					float2 val4 = val2;
					val3.x = 0;
					while (val3.x < m_LotSize.x)
					{
						int2 cellIndex = ZoneUtils.GetCellIndex(block, val4);
						if (math.all((cellIndex >= ((int4)(ref validArea.m_Area)).xz) & (cellIndex < ((int4)(ref validArea.m_Area)).yw)))
						{
							int num = cellIndex.y * block.m_Size.x + cellIndex.x;
							Cell cell = val[num];
							if ((cell.m_State & CellFlags.Visible) != CellFlags.None)
							{
								m_MaxHeight = math.min(m_MaxHeight, (int)cell.m_Height);
							}
						}
						val4 -= m_Right;
						val3.x++;
					}
					val2 -= m_Forward;
					val3.y++;
				}
			}
		}

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<SpawnableBuildingData> m_SpawnableBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<BuildingData> m_BuildingType;

		[ReadOnly]
		public ComponentTypeHandle<BuildingPropertyData> m_BuildingPropertyType;

		[ReadOnly]
		public ComponentTypeHandle<ObjectGeometryData> m_ObjectGeometryType;

		[ReadOnly]
		public SharedComponentTypeHandle<BuildingSpawnGroupData> m_BuildingSpawnGroupType;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Block> m_BlockData;

		[ReadOnly]
		public ComponentLookup<ValidArea> m_ValidAreaData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<PrefabData> m_PrefabDatas;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildings;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_Buildings;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_BuildingPropertyDatas;

		[ReadOnly]
		public ComponentLookup<OfficeBuilding> m_OfficeBuilding;

		[ReadOnly]
		public ComponentLookup<ZoneData> m_ZoneData;

		[ReadOnly]
		public BufferLookup<Cell> m_Cells;

		[ReadOnly]
		public BuildingConfigurationData m_BuildingConfigurationData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_SpawnableBuildingChunks;

		[ReadOnly]
		public NativeQuadTree<Entity, Bounds2> m_ZoneSearchTree;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		public IconCommandBuffer m_IconCommandBuffer;

		public NativeQueue<Entity> m_LevelupQueue;

		public EntityCommandBuffer m_CommandBuffer;

		public NativeQueue<TriggerAction> m_TriggerBuffer;

		public NativeQueue<ZoneBuiltLevelUpdate> m_ZoneBuiltLevelQueue;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_RandomSeed.GetRandom(0);
			Entity val = default(Entity);
			while (m_LevelupQueue.TryDequeue(ref val))
			{
				Entity prefab = m_Prefabs[val].m_Prefab;
				if (!m_SpawnableBuildings.HasComponent(prefab))
				{
					continue;
				}
				SpawnableBuildingData spawnableBuildingData = m_SpawnableBuildings[prefab];
				if (!m_PrefabDatas.IsComponentEnabled(spawnableBuildingData.m_ZonePrefab))
				{
					continue;
				}
				BuildingData prefabBuildingData = m_Buildings[prefab];
				BuildingPropertyData buildingPropertyData = m_BuildingPropertyDatas[prefab];
				ZoneData zoneData = m_ZoneData[spawnableBuildingData.m_ZonePrefab];
				float maxHeight = GetMaxHeight(val, prefabBuildingData);
				Entity val2 = SelectSpawnableBuilding(zoneData.m_ZoneType, spawnableBuildingData.m_Level + 1, prefabBuildingData.m_LotSize, maxHeight, prefabBuildingData.m_Flags & (Game.Prefabs.BuildingFlags.LeftAccess | Game.Prefabs.BuildingFlags.RightAccess), buildingPropertyData, ref random);
				if (!(val2 != Entity.Null))
				{
					continue;
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<UnderConstruction>(val, new UnderConstruction
				{
					m_NewPrefab = val2,
					m_Progress = byte.MaxValue
				});
				if (buildingPropertyData.CountProperties(AreaType.Residential) > 0)
				{
					m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.LevelUpResidentialBuilding, Entity.Null, val, val));
				}
				if (buildingPropertyData.CountProperties(AreaType.Commercial) > 0)
				{
					m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.LevelUpCommercialBuilding, Entity.Null, val, val));
				}
				if (buildingPropertyData.CountProperties(AreaType.Industrial) > 0)
				{
					if (m_OfficeBuilding.HasComponent(prefab))
					{
						m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.LevelUpOfficeBuilding, Entity.Null, val, val));
					}
					else
					{
						m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.LevelUpIndustrialBuilding, Entity.Null, val, val));
					}
				}
				m_ZoneBuiltLevelQueue.Enqueue(new ZoneBuiltLevelUpdate
				{
					m_Zone = spawnableBuildingData.m_ZonePrefab,
					m_FromLevel = spawnableBuildingData.m_Level,
					m_ToLevel = spawnableBuildingData.m_Level + 1,
					m_Squares = prefabBuildingData.m_LotSize.x * prefabBuildingData.m_LotSize.y
				});
				m_IconCommandBuffer.Add(val, m_BuildingConfigurationData.m_LevelUpNotification, IconPriority.Info, IconClusterLayer.Transaction);
			}
		}

		private Entity SelectSpawnableBuilding(ZoneType zoneType, int level, int2 lotSize, float maxHeight, Game.Prefabs.BuildingFlags accessFlags, BuildingPropertyData buildingPropertyData, ref Random random)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			Entity result = Entity.Null;
			for (int i = 0; i < m_SpawnableBuildingChunks.Length; i++)
			{
				ArchetypeChunk val = m_SpawnableBuildingChunks[i];
				if (!((ArchetypeChunk)(ref val)).GetSharedComponent<BuildingSpawnGroupData>(m_BuildingSpawnGroupType).m_ZoneType.Equals(zoneType))
				{
					continue;
				}
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<SpawnableBuildingData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<SpawnableBuildingData>(ref m_SpawnableBuildingType);
				NativeArray<BuildingData> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<BuildingData>(ref m_BuildingType);
				NativeArray<BuildingPropertyData> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<BuildingPropertyData>(ref m_BuildingPropertyType);
				NativeArray<ObjectGeometryData> nativeArray5 = ((ArchetypeChunk)(ref val)).GetNativeArray<ObjectGeometryData>(ref m_ObjectGeometryType);
				for (int j = 0; j < ((ArchetypeChunk)(ref val)).Count; j++)
				{
					SpawnableBuildingData spawnableBuildingData = nativeArray2[j];
					BuildingData buildingData = nativeArray3[j];
					BuildingPropertyData buildingPropertyData2 = nativeArray4[j];
					ObjectGeometryData objectGeometryData = nativeArray5[j];
					if (level == spawnableBuildingData.m_Level && ((int2)(ref lotSize)).Equals(buildingData.m_LotSize) && objectGeometryData.m_Size.y <= maxHeight && (buildingData.m_Flags & (Game.Prefabs.BuildingFlags.LeftAccess | Game.Prefabs.BuildingFlags.RightAccess)) == accessFlags && buildingPropertyData.m_ResidentialProperties <= buildingPropertyData2.m_ResidentialProperties && buildingPropertyData.m_AllowedManufactured == buildingPropertyData2.m_AllowedManufactured && buildingPropertyData.m_AllowedInput == buildingPropertyData2.m_AllowedInput && buildingPropertyData.m_AllowedSold == buildingPropertyData2.m_AllowedSold && buildingPropertyData.m_AllowedStored == buildingPropertyData2.m_AllowedStored)
					{
						int num2 = 100;
						num += num2;
						if (((Random)(ref random)).NextInt(num) < num2)
						{
							result = nativeArray[j];
						}
					}
				}
			}
			return result;
		}

		private float GetMaxHeight(Entity building, BuildingData prefabBuildingData)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = m_TransformData[building];
			float3 val = math.rotate(transform.m_Rotation, new float3(8f, 0f, 0f));
			float2 xz = ((float3)(ref val)).xz;
			val = math.rotate(transform.m_Rotation, new float3(0f, 0f, 8f));
			float2 xz2 = ((float3)(ref val)).xz;
			float2 val2 = xz * ((float)prefabBuildingData.m_LotSize.x * 0.5f - 0.5f);
			float2 val3 = xz2 * ((float)prefabBuildingData.m_LotSize.y * 0.5f - 0.5f);
			float2 val4 = math.abs(val3) + math.abs(val2);
			Iterator iterator = new Iterator
			{
				m_Bounds = new Bounds2(((float3)(ref transform.m_Position)).xz - val4, ((float3)(ref transform.m_Position)).xz + val4),
				m_LotSize = prefabBuildingData.m_LotSize,
				m_StartPosition = ((float3)(ref transform.m_Position)).xz + val3 + val2,
				m_Right = xz,
				m_Forward = xz2,
				m_MaxHeight = int.MaxValue,
				m_BlockData = m_BlockData,
				m_ValidAreaData = m_ValidAreaData,
				m_Cells = m_Cells
			};
			m_ZoneSearchTree.Iterate<Iterator>(ref iterator, 0);
			return (float)iterator.m_MaxHeight - transform.m_Position.y;
		}
	}

	private struct TypeHandle
	{
		public ComponentTypeHandle<BuildingCondition> __Game_Buildings_BuildingCondition_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Renter> __Game_Buildings_Renter_RO_BufferTypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<ConsumptionData> __Game_Prefabs_ConsumptionData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ResourceAvailability> __Game_Net_ResourceAvailability_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<SignatureBuildingData> __Game_Prefabs_SignatureBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Abandoned> __Game_Buildings_Abandoned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Resources> __Game_Economy_Resources_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentTypeHandle;

		public SharedComponentTypeHandle<BuildingSpawnGroupData> __Game_Prefabs_BuildingSpawnGroupData_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Block> __Game_Zones_Block_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ValidArea> __Game_Zones_ValidArea_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OfficeBuilding> __Game_Prefabs_OfficeBuilding_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Cell> __Game_Zones_Cell_RO_BufferLookup;

		public ComponentLookup<Building> __Game_Buildings_Building_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarbageProducer> __Game_Buildings_GarbageProducer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MailProducer> __Game_Buildings_MailProducer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> __Game_Buildings_WaterConsumer_RO_ComponentLookup;

		public ComponentLookup<CrimeProducer> __Game_Buildings_CrimeProducer_RW_ComponentLookup;

		public BufferLookup<Renter> __Game_Buildings_Renter_RW_BufferLookup;

		public BufferLookup<Resources> __Game_Economy_Resources_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			__Game_Buildings_BuildingCondition_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildingCondition>(false);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_Buildings_Renter_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Renter>(true);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Prefabs_ConsumptionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ConsumptionData>(true);
			__Game_Net_ResourceAvailability_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ResourceAvailability>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingPropertyData>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Prefabs_SignatureBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SignatureBuildingData>(true);
			__Game_Buildings_Abandoned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Abandoned>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_ZoneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneData>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedVehicle>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Vehicles_DeliveryTruck_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.DeliveryTruck>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Economy_Resources_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SpawnableBuildingData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildingData>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildingPropertyData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ObjectGeometryData>(true);
			__Game_Prefabs_BuildingSpawnGroupData_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<BuildingSpawnGroupData>();
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Zones_Block_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Block>(true);
			__Game_Zones_ValidArea_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ValidArea>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_PrefabData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabData>(true);
			__Game_Prefabs_OfficeBuilding_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OfficeBuilding>(true);
			__Game_Zones_Cell_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Cell>(true);
			__Game_Buildings_Building_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(false);
			__Game_Buildings_ElectricityConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityConsumer>(true);
			__Game_Buildings_GarbageProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageProducer>(true);
			__Game_Buildings_MailProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MailProducer>(true);
			__Game_Buildings_WaterConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterConsumer>(true);
			__Game_Buildings_CrimeProducer_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CrimeProducer>(false);
			__Game_Buildings_Renter_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(false);
			__Game_Economy_Resources_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(false);
		}
	}

	public static readonly int kUpdatesPerDay = 16;

	public static readonly int kMaterialUpkeep = 4;

	private SimulationSystem m_SimulationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private ResourceSystem m_ResourceSystem;

	private ClimateSystem m_ClimateSystem;

	private CitySystem m_CitySystem;

	private IconCommandSystem m_IconCommandSystem;

	private TriggerSystem m_TriggerSystem;

	private ZoneBuiltRequirementSystem m_ZoneBuiltRequirementSystemSystem;

	private Game.Zones.SearchSystem m_ZoneSearchSystem;

	private ElectricityRoadConnectionGraphSystem m_ElectricityRoadConnectionGraphSystem;

	private WaterPipeRoadConnectionGraphSystem m_WaterPipeRoadConnectionGraphSystem;

	private NativeQueue<UpkeepPayment> m_UpkeepExpenseQueue;

	private NativeQueue<Entity> m_LevelupQueue;

	private NativeQueue<Entity> m_LeveldownQueue;

	private EntityQuery m_BuildingPrefabGroup;

	private EntityQuery m_BuildingSettingsQuery;

	private EntityQuery m_BuildingGroup;

	public bool debugFastLeveling;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (kUpdatesPerDay * 16);
	}

	public static float GetHeatingMultiplier(float temperature)
	{
		return math.max(0f, 15f - temperature);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_ZoneBuiltRequirementSystemSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ZoneBuiltRequirementSystem>();
		m_ZoneSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Zones.SearchSystem>();
		m_ElectricityRoadConnectionGraphSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ElectricityRoadConnectionGraphSystem>();
		m_WaterPipeRoadConnectionGraphSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterPipeRoadConnectionGraphSystem>();
		m_UpkeepExpenseQueue = new NativeQueue<UpkeepPayment>(AllocatorHandle.op_Implicit((Allocator)4));
		m_BuildingSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BuildingConfigurationData>() });
		m_LevelupQueue = new NativeQueue<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
		m_LeveldownQueue = new NativeQueue<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<BuildingCondition>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<UpdateFrame>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[0];
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Abandoned>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_BuildingGroup = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_BuildingPrefabGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<BuildingData>(),
			ComponentType.ReadOnly<BuildingSpawnGroupData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_BuildingGroup);
		((ComponentSystemBase)this).RequireForUpdate(m_BuildingSettingsQuery);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_UpkeepExpenseQueue.Dispose();
		m_LevelupQueue.Dispose();
		m_LeveldownQueue.Dispose();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0703: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0772: Unknown result type (might be due to invalid IL or missing references)
		//IL_0777: Unknown result type (might be due to invalid IL or missing references)
		//IL_078f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0794: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0815: Unknown result type (might be due to invalid IL or missing references)
		//IL_081a: Unknown result type (might be due to invalid IL or missing references)
		//IL_082a: Unknown result type (might be due to invalid IL or missing references)
		//IL_082f: Unknown result type (might be due to invalid IL or missing references)
		//IL_083c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0841: Unknown result type (might be due to invalid IL or missing references)
		//IL_0850: Unknown result type (might be due to invalid IL or missing references)
		//IL_0855: Unknown result type (might be due to invalid IL or missing references)
		//IL_0864: Unknown result type (might be due to invalid IL or missing references)
		//IL_0869: Unknown result type (might be due to invalid IL or missing references)
		//IL_089a: Unknown result type (might be due to invalid IL or missing references)
		//IL_089f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_08db: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0911: Unknown result type (might be due to invalid IL or missing references)
		//IL_0916: Unknown result type (might be due to invalid IL or missing references)
		//IL_091e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0923: Unknown result type (might be due to invalid IL or missing references)
		//IL_0930: Unknown result type (might be due to invalid IL or missing references)
		//IL_0935: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, kUpdatesPerDay, 16);
		BuildingConfigurationData singleton = ((EntityQuery)(ref m_BuildingSettingsQuery)).GetSingleton<BuildingConfigurationData>();
		BuildingUpkeepJob buildingUpkeepJob = new BuildingUpkeepJob
		{
			m_ConditionType = InternalCompilerInterface.GetComponentTypeHandle<BuildingCondition>(ref __TypeHandle.__Game_Buildings_BuildingCondition_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RenterType = InternalCompilerInterface.GetBufferTypeHandle<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConsumptionDatas = InternalCompilerInterface.GetComponentLookup<ConsumptionData>(ref __TypeHandle.__Game_Prefabs_ConsumptionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Availabilities = InternalCompilerInterface.GetBufferLookup<ResourceAvailability>(ref __TypeHandle.__Game_Net_ResourceAvailability_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingPropertyDatas = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifierBufs = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SignatureDatas = InternalCompilerInterface.GetComponentLookup<SignatureBuildingData>(ref __TypeHandle.__Game_Prefabs_SignatureBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Abandoned = InternalCompilerInterface.GetComponentLookup<Abandoned>(ref __TypeHandle.__Game_Buildings_Abandoned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Destroyed = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableBuildingDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneDatas = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicles = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeliveryTrucks = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_City = m_CitySystem.City,
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Resources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingConfigurationData = singleton,
			m_UpdateFrameIndex = updateFrame,
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_UpkeepExpenseQueue = m_UpkeepExpenseQueue.AsParallelWriter(),
			m_LevelupQueue = m_LevelupQueue.AsParallelWriter(),
			m_LevelDownQueue = m_LeveldownQueue.AsParallelWriter(),
			m_DebugFastLeveling = debugFastLeveling
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		buildingUpkeepJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		buildingUpkeepJob.m_TemperatureUpkeep = GetHeatingMultiplier(m_ClimateSystem.temperature);
		BuildingUpkeepJob buildingUpkeepJob2 = buildingUpkeepJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<BuildingUpkeepJob>(buildingUpkeepJob2, m_BuildingGroup, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
		m_ResourceSystem.AddPrefabsReader(((SystemBase)this).Dependency);
		JobHandle val2 = default(JobHandle);
		JobHandle dependencies;
		JobHandle deps;
		LevelupJob levelupJob = new LevelupJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableBuildingType = InternalCompilerInterface.GetComponentTypeHandle<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingPropertyType = InternalCompilerInterface.GetComponentTypeHandle<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectGeometryType = InternalCompilerInterface.GetComponentTypeHandle<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingSpawnGroupType = InternalCompilerInterface.GetSharedComponentTypeHandle<BuildingSpawnGroupData>(ref __TypeHandle.__Game_Prefabs_BuildingSpawnGroupData_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BlockData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ValidAreaData = InternalCompilerInterface.GetComponentLookup<ValidArea>(ref __TypeHandle.__Game_Zones_ValidArea_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabDatas = InternalCompilerInterface.GetComponentLookup<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableBuildings = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Buildings = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingPropertyDatas = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OfficeBuilding = InternalCompilerInterface.GetComponentLookup<OfficeBuilding>(ref __TypeHandle.__Game_Prefabs_OfficeBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneData = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Cells = InternalCompilerInterface.GetBufferLookup<Cell>(ref __TypeHandle.__Game_Zones_Cell_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingConfigurationData = singleton,
			m_SpawnableBuildingChunks = ((EntityQuery)(ref m_BuildingPrefabGroup)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val2),
			m_ZoneSearchTree = m_ZoneSearchSystem.GetSearchTree(readOnly: true, out dependencies),
			m_RandomSeed = RandomSeed.Next(),
			m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer(),
			m_LevelupQueue = m_LevelupQueue,
			m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer(),
			m_TriggerBuffer = m_TriggerSystem.CreateActionBuffer(),
			m_ZoneBuiltLevelQueue = m_ZoneBuiltRequirementSystemSystem.GetZoneBuiltLevelQueue(out deps)
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<LevelupJob>(levelupJob, JobUtils.CombineDependencies(((SystemBase)this).Dependency, val2, dependencies, deps));
		m_ZoneSearchSystem.AddSearchTreeReader(((SystemBase)this).Dependency);
		m_ZoneBuiltRequirementSystemSystem.AddWriter(((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
		m_TriggerSystem.AddActionBufferWriter(((SystemBase)this).Dependency);
		JobHandle deps2;
		JobHandle deps3;
		LeveldownJob leveldownJob = new LeveldownJob
		{
			m_BuildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableBuildings = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Buildings = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityConsumers = InternalCompilerInterface.GetComponentLookup<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageProducers = InternalCompilerInterface.GetComponentLookup<GarbageProducer>(ref __TypeHandle.__Game_Buildings_GarbageProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MailProducers = InternalCompilerInterface.GetComponentLookup<MailProducer>(ref __TypeHandle.__Game_Buildings_MailProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterConsumers = InternalCompilerInterface.GetComponentLookup<WaterConsumer>(ref __TypeHandle.__Game_Buildings_WaterConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingPropertyDatas = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OfficeBuilding = InternalCompilerInterface.GetComponentLookup<OfficeBuilding>(ref __TypeHandle.__Game_Prefabs_OfficeBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TriggerBuffer = m_TriggerSystem.CreateActionBuffer(),
			m_CrimeProducers = InternalCompilerInterface.GetComponentLookup<CrimeProducer>(ref __TypeHandle.__Game_Buildings_CrimeProducer_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Renters = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingConfigurationData = singleton,
			m_LeveldownQueue = m_LeveldownQueue,
			m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer(),
			m_UpdatedElectricityRoadEdges = m_ElectricityRoadConnectionGraphSystem.GetEdgeUpdateQueue(out deps2),
			m_UpdatedWaterPipeRoadEdges = m_WaterPipeRoadConnectionGraphSystem.GetEdgeUpdateQueue(out deps3),
			m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer(),
			m_SimulationFrame = m_SimulationSystem.frameIndex
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<LeveldownJob>(leveldownJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps2, deps3));
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
		m_ElectricityRoadConnectionGraphSystem.AddQueueWriter(((SystemBase)this).Dependency);
		m_IconCommandSystem.AddCommandBufferWriter(((SystemBase)this).Dependency);
		m_TriggerSystem.AddActionBufferWriter(((SystemBase)this).Dependency);
		UpkeepPaymentJob upkeepPaymentJob = new UpkeepPaymentJob
		{
			m_Resources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpkeepExpenseQueue = m_UpkeepExpenseQueue
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<UpkeepPaymentJob>(upkeepPaymentJob, ((SystemBase)this).Dependency);
	}

	public void DebugLevelUp(Entity building, ComponentLookup<BuildingCondition> conditions, ComponentLookup<SpawnableBuildingData> spawnables, ComponentLookup<PrefabRef> prefabRefs, ComponentLookup<ZoneData> zoneDatas, ComponentLookup<BuildingPropertyData> propertyDatas)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (!conditions.HasComponent(building) || !prefabRefs.HasComponent(building))
		{
			return;
		}
		_ = conditions[building];
		Entity prefab = prefabRefs[building].m_Prefab;
		if (spawnables.HasComponent(prefab) && propertyDatas.HasComponent(prefab))
		{
			SpawnableBuildingData spawnableBuildingData = spawnables[prefab];
			if (zoneDatas.HasComponent(spawnableBuildingData.m_ZonePrefab))
			{
				_ = zoneDatas[spawnableBuildingData.m_ZonePrefab];
				m_LevelupQueue.Enqueue(building);
			}
		}
	}

	public void DebugLevelDown(Entity building, ComponentLookup<BuildingCondition> conditions, ComponentLookup<SpawnableBuildingData> spawnables, ComponentLookup<PrefabRef> prefabRefs, ComponentLookup<ZoneData> zoneDatas, ComponentLookup<BuildingPropertyData> propertyDatas)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (!conditions.HasComponent(building) || !prefabRefs.HasComponent(building))
		{
			return;
		}
		BuildingCondition buildingCondition = conditions[building];
		Entity prefab = prefabRefs[building].m_Prefab;
		if (spawnables.HasComponent(prefab) && propertyDatas.HasComponent(prefab))
		{
			SpawnableBuildingData spawnableBuildingData = spawnables[prefab];
			if (zoneDatas.HasComponent(spawnableBuildingData.m_ZonePrefab))
			{
				AreaType areaType = zoneDatas[spawnableBuildingData.m_ZonePrefab].m_AreaType;
				BuildingPropertyData propertyData = propertyDatas[prefab];
				byte level = spawnableBuildingData.m_Level;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				int levelingCost = BuildingUtils.GetLevelingCost(areaType, propertyData, level, ((EntityManager)(ref entityManager)).GetBuffer<CityModifier>(m_CitySystem.City, true));
				buildingCondition.m_Condition = -3 * levelingCost / 2;
				conditions[building] = buildingCondition;
				m_LeveldownQueue.Enqueue(building);
			}
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
	public BuildingUpkeepSystem()
	{
	}
}
