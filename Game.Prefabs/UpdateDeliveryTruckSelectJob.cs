using Game.Economy;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Prefabs;

[BurstCompile]
public struct UpdateDeliveryTruckSelectJob : IJob
{
	private struct TruckData
	{
		public Entity m_Entity;

		public DeliveryTruckData m_DeliveryTruckData;

		public CarTrailerData m_TrailerData;

		public CarTractorData m_TractorData;

		public ObjectData m_ObjectData;
	}

	[ReadOnly]
	public EntityTypeHandle m_EntityType;

	[ReadOnly]
	public ComponentTypeHandle<DeliveryTruckData> m_DeliveryTruckDataType;

	[ReadOnly]
	public ComponentTypeHandle<CarTrailerData> m_CarTrailerDataType;

	[ReadOnly]
	public ComponentTypeHandle<CarTractorData> m_CarTractorDataType;

	[ReadOnly]
	public NativeList<ArchetypeChunk> m_PrefabChunks;

	[ReadOnly]
	public VehicleSelectRequirementData m_RequirementData;

	public NativeList<DeliveryTruckSelectItem> m_DeliveryTruckItems;

	public void Execute()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		m_DeliveryTruckItems.Clear();
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<DeliveryTruckData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<DeliveryTruckData>(ref m_DeliveryTruckDataType);
			NativeArray<CarTrailerData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTrailerData>(ref m_CarTrailerDataType);
			NativeArray<CarTractorData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTractorData>(ref m_CarTractorDataType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray2.Length; j++)
			{
				TruckData truckData = new TruckData
				{
					m_DeliveryTruckData = nativeArray2[j]
				};
				if (truckData.m_DeliveryTruckData.m_CargoCapacity == 0 || truckData.m_DeliveryTruckData.m_TransportedResources == Resource.NoResource || !m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				Resource transportedResources = truckData.m_DeliveryTruckData.m_TransportedResources;
				truckData.m_Entity = nativeArray[j];
				bool flag = false;
				if (nativeArray3.Length != 0)
				{
					truckData.m_TrailerData = nativeArray3[j];
					flag = true;
				}
				if (nativeArray4.Length != 0)
				{
					truckData.m_TractorData = nativeArray4[j];
					if (truckData.m_TractorData.m_FixedTrailer != Entity.Null)
					{
						CheckTrailers(transportedResources, flag, truckData);
						continue;
					}
				}
				if (flag)
				{
					CheckTractors(transportedResources, truckData);
					continue;
				}
				ref NativeList<DeliveryTruckSelectItem> deliveryTruckItems = ref m_DeliveryTruckItems;
				DeliveryTruckSelectItem deliveryTruckSelectItem = new DeliveryTruckSelectItem
				{
					m_Capacity = truckData.m_DeliveryTruckData.m_CargoCapacity,
					m_Cost = truckData.m_DeliveryTruckData.m_CostToDrive,
					m_Resources = transportedResources,
					m_Prefab1 = truckData.m_Entity
				};
				deliveryTruckItems.Add(ref deliveryTruckSelectItem);
			}
		}
		if (m_DeliveryTruckItems.Length >= 2)
		{
			NativeSortExtension.Sort<DeliveryTruckSelectItem>(m_DeliveryTruckItems);
			DeliveryTruckSelectItem deliveryTruckSelectItem2 = default(DeliveryTruckSelectItem);
			DeliveryTruckSelectItem deliveryTruckSelectItem3 = m_DeliveryTruckItems[0];
			int num = 0;
			for (int k = 1; k < m_DeliveryTruckItems.Length; k++)
			{
				DeliveryTruckSelectItem deliveryTruckSelectItem4 = m_DeliveryTruckItems[k];
				if (deliveryTruckSelectItem3.m_Resources != Resource.NoResource && deliveryTruckSelectItem3.m_Cost > deliveryTruckSelectItem4.m_Cost)
				{
					deliveryTruckSelectItem3.m_Resources &= ~deliveryTruckSelectItem4.m_Resources;
					for (int l = k + 1; l < m_DeliveryTruckItems.Length; l++)
					{
						if (deliveryTruckSelectItem3.m_Resources == Resource.NoResource)
						{
							break;
						}
						DeliveryTruckSelectItem deliveryTruckSelectItem5 = m_DeliveryTruckItems[l];
						if (deliveryTruckSelectItem3.m_Cost <= deliveryTruckSelectItem5.m_Cost)
						{
							break;
						}
						deliveryTruckSelectItem3.m_Resources &= ~deliveryTruckSelectItem5.m_Resources;
					}
				}
				if (deliveryTruckSelectItem3.m_Resources != Resource.NoResource)
				{
					m_DeliveryTruckItems[num++] = deliveryTruckSelectItem3;
					deliveryTruckSelectItem2 = deliveryTruckSelectItem3;
				}
				deliveryTruckSelectItem3 = deliveryTruckSelectItem4;
				if (deliveryTruckSelectItem3.m_Resources == Resource.NoResource || deliveryTruckSelectItem3.m_Cost * deliveryTruckSelectItem2.m_Capacity <= deliveryTruckSelectItem2.m_Cost * deliveryTruckSelectItem3.m_Capacity)
				{
					continue;
				}
				deliveryTruckSelectItem3.m_Resources &= ~deliveryTruckSelectItem2.m_Resources;
				int num2 = num - 2;
				while (num2 >= 0 && deliveryTruckSelectItem3.m_Resources != Resource.NoResource)
				{
					DeliveryTruckSelectItem deliveryTruckSelectItem6 = m_DeliveryTruckItems[num2];
					if (deliveryTruckSelectItem3.m_Cost * deliveryTruckSelectItem6.m_Capacity <= deliveryTruckSelectItem6.m_Cost * deliveryTruckSelectItem3.m_Capacity)
					{
						break;
					}
					deliveryTruckSelectItem3.m_Resources &= ~deliveryTruckSelectItem6.m_Resources;
					num2--;
				}
			}
			if (deliveryTruckSelectItem3.m_Resources != Resource.NoResource)
			{
				m_DeliveryTruckItems[num++] = deliveryTruckSelectItem3;
			}
			if (num < m_DeliveryTruckItems.Length)
			{
				m_DeliveryTruckItems.RemoveRange(num, m_DeliveryTruckItems.Length - num);
			}
		}
		m_DeliveryTruckItems.TrimExcess();
	}

	private void CheckTrailers(Resource resourceMask, bool firstIsTrailer, TruckData firstData)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<CarTrailerData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTrailerData>(ref m_CarTrailerDataType);
			if (nativeArray.Length == 0)
			{
				continue;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<DeliveryTruckData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<DeliveryTruckData>(ref m_DeliveryTruckDataType);
			NativeArray<CarTractorData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTractorData>(ref m_CarTractorDataType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				TruckData truckData = new TruckData
				{
					m_DeliveryTruckData = nativeArray3[j]
				};
				if (truckData.m_DeliveryTruckData.m_CargoCapacity != 0 || !m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				truckData.m_Entity = nativeArray2[j];
				truckData.m_TrailerData = nativeArray[j];
				if (firstData.m_TractorData.m_TrailerType != truckData.m_TrailerData.m_TrailerType || (firstData.m_TractorData.m_FixedTrailer != Entity.Null && firstData.m_TractorData.m_FixedTrailer != truckData.m_Entity) || (truckData.m_TrailerData.m_FixedTractor != Entity.Null && truckData.m_TrailerData.m_FixedTractor != firstData.m_Entity))
				{
					continue;
				}
				if (nativeArray4.Length != 0)
				{
					truckData.m_TractorData = nativeArray4[j];
					if (truckData.m_TractorData.m_FixedTrailer != Entity.Null)
					{
						CheckTrailers(resourceMask, firstIsTrailer, firstData, truckData);
						continue;
					}
				}
				if (firstIsTrailer)
				{
					CheckTractors(resourceMask, firstData, truckData);
					continue;
				}
				ref NativeList<DeliveryTruckSelectItem> deliveryTruckItems = ref m_DeliveryTruckItems;
				DeliveryTruckSelectItem deliveryTruckSelectItem = new DeliveryTruckSelectItem
				{
					m_Capacity = firstData.m_DeliveryTruckData.m_CargoCapacity + truckData.m_DeliveryTruckData.m_CargoCapacity,
					m_Cost = firstData.m_DeliveryTruckData.m_CostToDrive + truckData.m_DeliveryTruckData.m_CostToDrive,
					m_Resources = resourceMask,
					m_Prefab1 = firstData.m_Entity,
					m_Prefab2 = truckData.m_Entity
				};
				deliveryTruckItems.Add(ref deliveryTruckSelectItem);
			}
		}
	}

	private void CheckTrailers(Resource resourceMask, bool firstIsTrailer, TruckData firstData, TruckData secondData)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<CarTrailerData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTrailerData>(ref m_CarTrailerDataType);
			if (nativeArray.Length == 0)
			{
				continue;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<DeliveryTruckData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<DeliveryTruckData>(ref m_DeliveryTruckDataType);
			NativeArray<CarTractorData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTractorData>(ref m_CarTractorDataType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				TruckData truckData = new TruckData
				{
					m_DeliveryTruckData = nativeArray3[j]
				};
				if (truckData.m_DeliveryTruckData.m_CargoCapacity != 0 || !m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				truckData.m_Entity = nativeArray2[j];
				truckData.m_TrailerData = nativeArray[j];
				if (secondData.m_TractorData.m_TrailerType != truckData.m_TrailerData.m_TrailerType || (secondData.m_TractorData.m_FixedTrailer != Entity.Null && secondData.m_TractorData.m_FixedTrailer != truckData.m_Entity) || (truckData.m_TrailerData.m_FixedTractor != Entity.Null && truckData.m_TrailerData.m_FixedTractor != secondData.m_Entity))
				{
					continue;
				}
				if (nativeArray4.Length != 0)
				{
					truckData.m_TractorData = nativeArray4[j];
					if (truckData.m_TractorData.m_FixedTrailer != Entity.Null)
					{
						if (!firstIsTrailer)
						{
							CheckTrailers(resourceMask, firstData, secondData, truckData);
						}
						continue;
					}
				}
				if (firstIsTrailer)
				{
					CheckTractors(resourceMask, firstData, secondData, truckData);
					continue;
				}
				ref NativeList<DeliveryTruckSelectItem> deliveryTruckItems = ref m_DeliveryTruckItems;
				DeliveryTruckSelectItem deliveryTruckSelectItem = new DeliveryTruckSelectItem
				{
					m_Capacity = firstData.m_DeliveryTruckData.m_CargoCapacity + secondData.m_DeliveryTruckData.m_CargoCapacity + truckData.m_DeliveryTruckData.m_CargoCapacity,
					m_Cost = firstData.m_DeliveryTruckData.m_CostToDrive + secondData.m_DeliveryTruckData.m_CostToDrive + truckData.m_DeliveryTruckData.m_CostToDrive,
					m_Resources = resourceMask,
					m_Prefab1 = firstData.m_Entity,
					m_Prefab2 = secondData.m_Entity,
					m_Prefab3 = truckData.m_Entity
				};
				deliveryTruckItems.Add(ref deliveryTruckSelectItem);
			}
		}
	}

	private void CheckTrailers(Resource resourceMask, TruckData firstData, TruckData secondData, TruckData thirdData)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<CarTrailerData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTrailerData>(ref m_CarTrailerDataType);
			if (nativeArray.Length == 0)
			{
				continue;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<DeliveryTruckData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<DeliveryTruckData>(ref m_DeliveryTruckDataType);
			NativeArray<CarTractorData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTractorData>(ref m_CarTractorDataType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				TruckData truckData = new TruckData
				{
					m_DeliveryTruckData = nativeArray3[j]
				};
				if (truckData.m_DeliveryTruckData.m_CargoCapacity != 0 || !m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				truckData.m_Entity = nativeArray2[j];
				truckData.m_TrailerData = nativeArray[j];
				if (thirdData.m_TractorData.m_TrailerType != truckData.m_TrailerData.m_TrailerType || (thirdData.m_TractorData.m_FixedTrailer != Entity.Null && thirdData.m_TractorData.m_FixedTrailer != truckData.m_Entity) || (truckData.m_TrailerData.m_FixedTractor != Entity.Null && truckData.m_TrailerData.m_FixedTractor != thirdData.m_Entity))
				{
					continue;
				}
				if (nativeArray4.Length != 0)
				{
					truckData.m_TractorData = nativeArray4[j];
					if (truckData.m_TractorData.m_FixedTrailer != Entity.Null)
					{
						continue;
					}
				}
				ref NativeList<DeliveryTruckSelectItem> deliveryTruckItems = ref m_DeliveryTruckItems;
				DeliveryTruckSelectItem deliveryTruckSelectItem = new DeliveryTruckSelectItem
				{
					m_Capacity = firstData.m_DeliveryTruckData.m_CargoCapacity + secondData.m_DeliveryTruckData.m_CargoCapacity + thirdData.m_DeliveryTruckData.m_CargoCapacity + truckData.m_DeliveryTruckData.m_CargoCapacity,
					m_Cost = firstData.m_DeliveryTruckData.m_CostToDrive + secondData.m_DeliveryTruckData.m_CostToDrive + thirdData.m_DeliveryTruckData.m_CostToDrive + truckData.m_DeliveryTruckData.m_CostToDrive,
					m_Resources = resourceMask,
					m_Prefab1 = firstData.m_Entity,
					m_Prefab2 = secondData.m_Entity,
					m_Prefab3 = thirdData.m_Entity,
					m_Prefab4 = truckData.m_Entity
				};
				deliveryTruckItems.Add(ref deliveryTruckSelectItem);
			}
		}
	}

	private void CheckTractors(Resource resourceMask, TruckData secondData)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<CarTractorData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTractorData>(ref m_CarTractorDataType);
			if (nativeArray.Length == 0)
			{
				continue;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<DeliveryTruckData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<DeliveryTruckData>(ref m_DeliveryTruckDataType);
			NativeArray<CarTrailerData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTrailerData>(ref m_CarTrailerDataType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				TruckData secondData2 = new TruckData
				{
					m_DeliveryTruckData = nativeArray3[j]
				};
				Resource resource = resourceMask;
				if (secondData2.m_DeliveryTruckData.m_CargoCapacity != 0)
				{
					resource &= secondData2.m_DeliveryTruckData.m_TransportedResources;
					if (resource == Resource.NoResource)
					{
						continue;
					}
				}
				if (!m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				secondData2.m_Entity = nativeArray2[j];
				secondData2.m_TractorData = nativeArray[j];
				if (secondData2.m_TractorData.m_TrailerType == secondData.m_TrailerData.m_TrailerType && (!(secondData2.m_TractorData.m_FixedTrailer != Entity.Null) || !(secondData2.m_TractorData.m_FixedTrailer != secondData.m_Entity)) && (!(secondData.m_TrailerData.m_FixedTractor != Entity.Null) || !(secondData.m_TrailerData.m_FixedTractor != secondData2.m_Entity)))
				{
					if (nativeArray4.Length != 0)
					{
						secondData2.m_TrailerData = nativeArray4[j];
						CheckTractors(resource, secondData2, secondData);
						continue;
					}
					ref NativeList<DeliveryTruckSelectItem> deliveryTruckItems = ref m_DeliveryTruckItems;
					DeliveryTruckSelectItem deliveryTruckSelectItem = new DeliveryTruckSelectItem
					{
						m_Capacity = secondData2.m_DeliveryTruckData.m_CargoCapacity + secondData.m_DeliveryTruckData.m_CargoCapacity,
						m_Cost = secondData2.m_DeliveryTruckData.m_CostToDrive + secondData.m_DeliveryTruckData.m_CostToDrive,
						m_Resources = resource,
						m_Prefab1 = secondData2.m_Entity,
						m_Prefab2 = secondData.m_Entity
					};
					deliveryTruckItems.Add(ref deliveryTruckSelectItem);
				}
			}
		}
	}

	private void CheckTractors(Resource resourceMask, TruckData secondData, TruckData thirdData)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<CarTractorData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTractorData>(ref m_CarTractorDataType);
			if (nativeArray.Length == 0)
			{
				continue;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<DeliveryTruckData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<DeliveryTruckData>(ref m_DeliveryTruckDataType);
			NativeArray<CarTrailerData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTrailerData>(ref m_CarTrailerDataType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				TruckData secondData2 = new TruckData
				{
					m_DeliveryTruckData = nativeArray3[j]
				};
				Resource resource = resourceMask;
				if (secondData2.m_DeliveryTruckData.m_CargoCapacity != 0)
				{
					resource &= secondData2.m_DeliveryTruckData.m_TransportedResources;
					if (resource == Resource.NoResource)
					{
						continue;
					}
				}
				if (!m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				secondData2.m_Entity = nativeArray2[j];
				secondData2.m_TractorData = nativeArray[j];
				if (secondData2.m_TractorData.m_TrailerType == secondData.m_TrailerData.m_TrailerType && (!(secondData2.m_TractorData.m_FixedTrailer != Entity.Null) || !(secondData2.m_TractorData.m_FixedTrailer != secondData.m_Entity)) && (!(secondData.m_TrailerData.m_FixedTractor != Entity.Null) || !(secondData.m_TrailerData.m_FixedTractor != secondData2.m_Entity)))
				{
					if (nativeArray4.Length != 0)
					{
						secondData2.m_TrailerData = nativeArray4[j];
						CheckTractors(resource, secondData2, secondData, thirdData);
						continue;
					}
					ref NativeList<DeliveryTruckSelectItem> deliveryTruckItems = ref m_DeliveryTruckItems;
					DeliveryTruckSelectItem deliveryTruckSelectItem = new DeliveryTruckSelectItem
					{
						m_Capacity = secondData2.m_DeliveryTruckData.m_CargoCapacity + secondData.m_DeliveryTruckData.m_CargoCapacity + thirdData.m_DeliveryTruckData.m_CargoCapacity,
						m_Cost = secondData2.m_DeliveryTruckData.m_CostToDrive + secondData.m_DeliveryTruckData.m_CostToDrive + thirdData.m_DeliveryTruckData.m_CostToDrive,
						m_Resources = resource,
						m_Prefab1 = secondData2.m_Entity,
						m_Prefab2 = secondData.m_Entity,
						m_Prefab3 = thirdData.m_Entity
					};
					deliveryTruckItems.Add(ref deliveryTruckSelectItem);
				}
			}
		}
	}

	private void CheckTractors(Resource resourceMask, TruckData secondData, TruckData thirdData, TruckData forthData)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<CarTractorData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTractorData>(ref m_CarTractorDataType);
			if (nativeArray.Length == 0 || ((ArchetypeChunk)(ref chunk)).Has<CarTrailerData>(ref m_CarTrailerDataType))
			{
				continue;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<DeliveryTruckData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<DeliveryTruckData>(ref m_DeliveryTruckDataType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				TruckData truckData = new TruckData
				{
					m_DeliveryTruckData = nativeArray3[j]
				};
				Resource resource = resourceMask;
				if (truckData.m_DeliveryTruckData.m_CargoCapacity != 0)
				{
					resource &= truckData.m_DeliveryTruckData.m_TransportedResources;
					if (resource == Resource.NoResource)
					{
						continue;
					}
				}
				if (m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					truckData.m_Entity = nativeArray2[j];
					truckData.m_TractorData = nativeArray[j];
					if (truckData.m_TractorData.m_TrailerType == secondData.m_TrailerData.m_TrailerType && (!(truckData.m_TractorData.m_FixedTrailer != Entity.Null) || !(truckData.m_TractorData.m_FixedTrailer != secondData.m_Entity)) && (!(secondData.m_TrailerData.m_FixedTractor != Entity.Null) || !(secondData.m_TrailerData.m_FixedTractor != truckData.m_Entity)))
					{
						ref NativeList<DeliveryTruckSelectItem> deliveryTruckItems = ref m_DeliveryTruckItems;
						DeliveryTruckSelectItem deliveryTruckSelectItem = new DeliveryTruckSelectItem
						{
							m_Capacity = truckData.m_DeliveryTruckData.m_CargoCapacity + secondData.m_DeliveryTruckData.m_CargoCapacity + thirdData.m_DeliveryTruckData.m_CargoCapacity + forthData.m_DeliveryTruckData.m_CargoCapacity,
							m_Cost = truckData.m_DeliveryTruckData.m_CostToDrive + secondData.m_DeliveryTruckData.m_CostToDrive + thirdData.m_DeliveryTruckData.m_CostToDrive + forthData.m_DeliveryTruckData.m_CostToDrive,
							m_Resources = resource,
							m_Prefab1 = truckData.m_Entity,
							m_Prefab2 = secondData.m_Entity,
							m_Prefab3 = thirdData.m_Entity,
							m_Prefab4 = forthData.m_Entity
						};
						deliveryTruckItems.Add(ref deliveryTruckSelectItem);
					}
				}
			}
		}
	}
}
