using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class ZoneSpawnSystem : GameSystemBase
{
	public struct SpawnLocation
	{
		public Entity m_Entity;

		public Entity m_Building;

		public int4 m_LotArea;

		public float m_Priority;

		public ZoneType m_ZoneType;

		public Game.Zones.AreaType m_AreaType;

		public LotFlags m_LotFlags;
	}

	[BurstCompile]
	public struct EvaluateSpawnAreas : IJobChunk
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_BuildingChunks;

		[ReadOnly]
		public ZonePrefabs m_ZonePrefabs;

		[ReadOnly]
		public ZonePreferenceData m_Preferences;

		[ReadOnly]
		public int m_SpawnResidential;

		[ReadOnly]
		public int m_SpawnCommercial;

		[ReadOnly]
		public int m_SpawnIndustrial;

		[ReadOnly]
		public int m_SpawnStorage;

		[ReadOnly]
		public int m_MinDemand;

		public int3 m_ResidentialDemands;

		[ReadOnly]
		public NativeArray<int> m_CommercialBuildingDemands;

		[ReadOnly]
		public NativeArray<int> m_IndustrialDemands;

		[ReadOnly]
		public NativeArray<int> m_StorageDemands;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Block> m_BlockType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<CurvePosition> m_CurvePositionType;

		[ReadOnly]
		public BufferTypeHandle<VacantLot> m_VacantLotType;

		[ReadOnly]
		public ComponentTypeHandle<BuildingData> m_BuildingDataType;

		[ReadOnly]
		public ComponentTypeHandle<SpawnableBuildingData> m_SpawnableBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<BuildingPropertyData> m_BuildingPropertyType;

		[ReadOnly]
		public ComponentTypeHandle<ObjectGeometryData> m_ObjectGeometryType;

		[ReadOnly]
		public SharedComponentTypeHandle<BuildingSpawnGroupData> m_BuildingSpawnGroupType;

		[ReadOnly]
		public ComponentTypeHandle<WarehouseData> m_WarehouseType;

		[ReadOnly]
		public ComponentLookup<ZoneData> m_ZoneData;

		[ReadOnly]
		public ComponentLookup<ZonePropertiesData> m_ZonePropertiesDatas;

		[ReadOnly]
		public BufferLookup<ResourceAvailability> m_Availabilities;

		[ReadOnly]
		public NativeList<IndustrialProcessData> m_Processes;

		[ReadOnly]
		public BufferLookup<ProcessEstimate> m_ProcessEstimates;

		[ReadOnly]
		public ComponentLookup<LandValue> m_LandValues;

		[ReadOnly]
		public ComponentLookup<Block> m_BlockData;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public NativeArray<GroundPollution> m_PollutionMap;

		public ParallelWriter<SpawnLocation> m_Residential;

		public ParallelWriter<SpawnLocation> m_Commercial;

		public ParallelWriter<SpawnLocation> m_Industrial;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			SpawnLocation bestLocation = default(SpawnLocation);
			SpawnLocation bestLocation2 = default(SpawnLocation);
			SpawnLocation bestLocation3 = default(SpawnLocation);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<VacantLot> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<VacantLot>(ref m_VacantLotType);
			if (bufferAccessor.Length != 0)
			{
				NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
				NativeArray<CurvePosition> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurvePosition>(ref m_CurvePositionType);
				NativeArray<Block> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Block>(ref m_BlockType);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity entity = nativeArray[i];
					DynamicBuffer<VacantLot> val = bufferAccessor[i];
					Owner owner = nativeArray2[i];
					CurvePosition curvePosition = nativeArray3[i];
					Block block = nativeArray4[i];
					for (int j = 0; j < val.Length; j++)
					{
						VacantLot lot = val[j];
						if (!m_ZonePropertiesDatas.HasComponent(m_ZonePrefabs[lot.m_Type]))
						{
							continue;
						}
						ZoneData zoneData = m_ZoneData[m_ZonePrefabs[lot.m_Type]];
						ZonePropertiesData zonePropertiesData = m_ZonePropertiesDatas[m_ZonePrefabs[lot.m_Type]];
						DynamicBuffer<ProcessEstimate> estimates = m_ProcessEstimates[m_ZonePrefabs[lot.m_Type]];
						switch (zoneData.m_AreaType)
						{
						case Game.Zones.AreaType.Residential:
							if (m_SpawnResidential != 0)
							{
								float curvePos2 = CalculateCurvePos(curvePosition, lot, block);
								TryAddLot(ref bestLocation, ref random, owner.m_Owner, curvePos2, entity, lot.m_Area, lot.m_Flags, lot.m_Height, zoneData, zonePropertiesData, estimates, m_Processes);
							}
							break;
						case Game.Zones.AreaType.Commercial:
							if (m_SpawnCommercial != 0)
							{
								float curvePos3 = CalculateCurvePos(curvePosition, lot, block);
								TryAddLot(ref bestLocation2, ref random, owner.m_Owner, curvePos3, entity, lot.m_Area, lot.m_Flags, lot.m_Height, zoneData, zonePropertiesData, estimates, m_Processes);
							}
							break;
						case Game.Zones.AreaType.Industrial:
							if (m_SpawnIndustrial != 0 || m_SpawnStorage != 0)
							{
								float curvePos = CalculateCurvePos(curvePosition, lot, block);
								TryAddLot(ref bestLocation3, ref random, owner.m_Owner, curvePos, entity, lot.m_Area, lot.m_Flags, lot.m_Height, zoneData, zonePropertiesData, estimates, m_Processes, m_SpawnIndustrial != 0, m_SpawnStorage != 0);
							}
							break;
						}
					}
				}
			}
			if (bestLocation.m_Priority != 0f)
			{
				m_Residential.Enqueue(bestLocation);
			}
			if (bestLocation2.m_Priority != 0f)
			{
				m_Commercial.Enqueue(bestLocation2);
			}
			if (bestLocation3.m_Priority != 0f)
			{
				m_Industrial.Enqueue(bestLocation3);
			}
		}

		private float CalculateCurvePos(CurvePosition curvePosition, VacantLot lot, Block block)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			float num = math.saturate((float)(lot.m_Area.x + lot.m_Area.y) * 0.5f / (float)block.m_Size.x);
			return math.lerp(curvePosition.m_CurvePosition.x, curvePosition.m_CurvePosition.y, num);
		}

		private void TryAddLot(ref SpawnLocation bestLocation, ref Random random, Entity road, float curvePos, Entity entity, int4 area, LotFlags flags, int height, ZoneData zoneData, ZonePropertiesData zonePropertiesData, DynamicBuffer<ProcessEstimate> estimates, NativeList<IndustrialProcessData> processes, bool normal = true, bool storage = false)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			if (!m_Availabilities.HasBuffer(road))
			{
				return;
			}
			if ((zoneData.m_ZoneFlags & ZoneFlags.SupportLeftCorner) == 0)
			{
				flags &= ~LotFlags.CornerLeft;
			}
			if ((zoneData.m_ZoneFlags & ZoneFlags.SupportRightCorner) == 0)
			{
				flags &= ~LotFlags.CornerRight;
			}
			SpawnLocation location = new SpawnLocation
			{
				m_Entity = entity,
				m_LotArea = area,
				m_ZoneType = zoneData.m_ZoneType,
				m_AreaType = zoneData.m_AreaType,
				m_LotFlags = flags
			};
			bool office = zoneData.m_AreaType == Game.Zones.AreaType.Industrial && estimates.Length == 0;
			DynamicBuffer<ResourceAvailability> availabilities = m_Availabilities[road];
			if (m_BlockData.HasComponent(location.m_Entity))
			{
				float3 position = ZoneUtils.GetPosition(m_BlockData[location.m_Entity], ((int4)(ref location.m_LotArea)).xz, ((int4)(ref location.m_LotArea)).yw);
				bool extractor = false;
				float pollution = GroundPollutionSystem.GetPollution(position, m_PollutionMap).m_Pollution;
				float landValue = m_LandValues[road].m_LandValue;
				float maxHeight = (float)height - position.y;
				if (SelectBuilding(ref location, ref random, availabilities, zoneData, zonePropertiesData, curvePos, pollution, landValue, maxHeight, estimates, processes, normal, storage, extractor, office) && location.m_Priority > bestLocation.m_Priority)
				{
					bestLocation = location;
				}
			}
		}

		private bool SelectBuilding(ref SpawnLocation location, ref Random random, DynamicBuffer<ResourceAvailability> availabilities, ZoneData zoneData, ZonePropertiesData zonePropertiesData, float curvePos, float pollution, float landValue, float maxHeight, DynamicBuffer<ProcessEstimate> estimates, NativeList<IndustrialProcessData> processes, bool normal = true, bool storage = false, bool extractor = false, bool office = false)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			int2 val = ((int4)(ref location.m_LotArea)).yw - ((int4)(ref location.m_LotArea)).xz;
			BuildingData buildingData = default(BuildingData);
			bool2 val2 = default(bool2);
			((bool2)(ref val2))._002Ector((location.m_LotFlags & LotFlags.CornerLeft) != 0, (location.m_LotFlags & LotFlags.CornerRight) != 0);
			bool flag = (zoneData.m_ZoneFlags & ZoneFlags.SupportNarrow) == 0;
			bool2 val4 = default(bool2);
			for (int i = 0; i < m_BuildingChunks.Length; i++)
			{
				ArchetypeChunk val3 = m_BuildingChunks[i];
				if (!((ArchetypeChunk)(ref val3)).GetSharedComponent<BuildingSpawnGroupData>(m_BuildingSpawnGroupType).m_ZoneType.Equals(location.m_ZoneType))
				{
					continue;
				}
				bool flag2 = ((ArchetypeChunk)(ref val3)).Has<WarehouseData>(ref m_WarehouseType);
				if ((flag2 && !storage) || (!flag2 && !normal))
				{
					continue;
				}
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray(m_EntityType);
				NativeArray<BuildingData> nativeArray2 = ((ArchetypeChunk)(ref val3)).GetNativeArray<BuildingData>(ref m_BuildingDataType);
				NativeArray<SpawnableBuildingData> nativeArray3 = ((ArchetypeChunk)(ref val3)).GetNativeArray<SpawnableBuildingData>(ref m_SpawnableBuildingType);
				NativeArray<BuildingPropertyData> nativeArray4 = ((ArchetypeChunk)(ref val3)).GetNativeArray<BuildingPropertyData>(ref m_BuildingPropertyType);
				NativeArray<ObjectGeometryData> nativeArray5 = ((ArchetypeChunk)(ref val3)).GetNativeArray<ObjectGeometryData>(ref m_ObjectGeometryType);
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					if (nativeArray3[j].m_Level != 1)
					{
						continue;
					}
					BuildingData buildingData2 = nativeArray2[j];
					int2 lotSize = buildingData2.m_LotSize;
					((bool2)(ref val4))._002Ector((buildingData2.m_Flags & Game.Prefabs.BuildingFlags.LeftAccess) != 0, (buildingData2.m_Flags & Game.Prefabs.BuildingFlags.RightAccess) != 0);
					float y = nativeArray5[j].m_Size.y;
					if (!math.all(lotSize <= val) || !(y <= maxHeight))
					{
						continue;
					}
					BuildingPropertyData buildingPropertyData = nativeArray4[j];
					ZoneDensity zoneDensity = PropertyUtils.GetZoneDensity(zoneData, zonePropertiesData);
					int num = EvaluateDemandAndAvailability(buildingPropertyData, zoneData.m_AreaType, zoneDensity, flag2);
					if (!(num >= m_MinDemand || extractor))
					{
						continue;
					}
					int2 val5 = math.select(val - lotSize, int2.op_Implicit(0), lotSize == val - 1);
					float num2 = (float)(lotSize.x * lotSize.y) * ((Random)(ref random)).NextFloat(1f, 1.05f);
					num2 += (float)(val5.x * lotSize.y) * ((Random)(ref random)).NextFloat(0.95f, 1f);
					num2 += (float)(val.x * val5.y) * ((Random)(ref random)).NextFloat(0.55f, 0.6f);
					num2 /= (float)(val.x * val.y);
					num2 *= (float)(num + 1);
					num2 *= math.csum(math.select(float2.op_Implicit(0.01f), float2.op_Implicit(0.5f), val2 == val4));
					if (!extractor)
					{
						float num3 = landValue;
						float num4;
						if (location.m_AreaType == Game.Zones.AreaType.Residential)
						{
							num4 = ((buildingPropertyData.m_ResidentialProperties == 1) ? 2f : ((float)buildingPropertyData.CountProperties()));
							lotSize.x = math.select(lotSize.x, val.x, lotSize.x == val.x - 1 && flag);
							num3 *= (float)(lotSize.x * val.y);
						}
						else
						{
							num4 = buildingPropertyData.m_SpaceMultiplier;
						}
						float score = ZoneEvaluationUtils.GetScore(location.m_AreaType, office, availabilities, curvePos, ref m_Preferences, flag2, flag2 ? m_StorageDemands : m_IndustrialDemands, buildingPropertyData, pollution, num3 / num4, estimates, processes, m_ResourcePrefabs, ref m_ResourceDatas);
						score = math.select(score, math.max(0f, score) + 1f, m_MinDemand == 0);
						num2 *= score;
					}
					if (num2 > location.m_Priority)
					{
						location.m_Building = nativeArray[j];
						buildingData = buildingData2;
						location.m_Priority = num2;
					}
				}
			}
			if (location.m_Building != Entity.Null)
			{
				if ((buildingData.m_Flags & Game.Prefabs.BuildingFlags.LeftAccess) == 0 && ((buildingData.m_Flags & Game.Prefabs.BuildingFlags.RightAccess) != 0 || ((Random)(ref random)).NextBool()))
				{
					location.m_LotArea.x = location.m_LotArea.y - buildingData.m_LotSize.x;
					location.m_LotArea.w = location.m_LotArea.z + buildingData.m_LotSize.y;
				}
				else
				{
					((int4)(ref location.m_LotArea)).yw = ((int4)(ref location.m_LotArea)).xz + buildingData.m_LotSize;
				}
				return true;
			}
			return false;
		}

		private int EvaluateDemandAndAvailability(BuildingPropertyData buildingPropertyData, Game.Zones.AreaType areaType, ZoneDensity zoneDensity, bool storage = false)
		{
			switch (areaType)
			{
			case Game.Zones.AreaType.Residential:
				return zoneDensity switch
				{
					ZoneDensity.Low => m_ResidentialDemands.x, 
					ZoneDensity.Medium => m_ResidentialDemands.y, 
					_ => m_ResidentialDemands.z, 
				};
			case Game.Zones.AreaType.Commercial:
			{
				int num2 = 0;
				ResourceIterator iterator2 = ResourceIterator.GetIterator();
				while (iterator2.Next())
				{
					if ((buildingPropertyData.m_AllowedSold & iterator2.resource) != Resource.NoResource)
					{
						num2 += m_CommercialBuildingDemands[EconomyUtils.GetResourceIndex(iterator2.resource)];
					}
				}
				return num2;
			}
			case Game.Zones.AreaType.Industrial:
			{
				int num = 0;
				ResourceIterator iterator = ResourceIterator.GetIterator();
				while (iterator.Next())
				{
					if (storage)
					{
						if ((buildingPropertyData.m_AllowedStored & iterator.resource) != Resource.NoResource)
						{
							num += m_StorageDemands[EconomyUtils.GetResourceIndex(iterator.resource)];
						}
					}
					else if ((buildingPropertyData.m_AllowedManufactured & iterator.resource) != Resource.NoResource)
					{
						num += m_IndustrialDemands[EconomyUtils.GetResourceIndex(iterator.resource)];
					}
				}
				return num;
			}
			default:
				return 0;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	public struct SpawnBuildingJob : IJobParallelFor
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
		public ComponentLookup<Block> m_BlockData;

		[ReadOnly]
		public ComponentLookup<ValidArea> m_ValidAreaData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> m_PrefabPlaceableObjectData;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> m_PrefabSpawnableObjectData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> m_PrefabAreaGeometryData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[ReadOnly]
		public BufferLookup<Cell> m_Cells;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubArea> m_PrefabSubAreas;

		[ReadOnly]
		public BufferLookup<SubAreaNode> m_PrefabSubAreaNodes;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubNet> m_PrefabSubNets;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> m_PrefabPlaceholderElements;

		[ReadOnly]
		public EntityArchetype m_DefinitionArchetype;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public bool m_LefthandTraffic;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public NativeQuadTree<Entity, Bounds2> m_ZoneSearchTree;

		[ReadOnly]
		public BuildingConfigurationData m_BuildingConfigurationData;

		[NativeDisableParallelForRestriction]
		public NativeQueue<SpawnLocation> m_Residential;

		[NativeDisableParallelForRestriction]
		public NativeQueue<SpawnLocation> m_Commercial;

		[NativeDisableParallelForRestriction]
		public NativeQueue<SpawnLocation> m_Industrial;

		public ParallelWriter m_CommandBuffer;

		public void Execute(int index)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			SpawnLocation location;
			switch (index)
			{
			default:
				return;
			case 0:
				if (!SelectLocation(m_Residential, out location))
				{
					return;
				}
				break;
			case 1:
				if (!SelectLocation(m_Commercial, out location))
				{
					return;
				}
				break;
			case 2:
				if (!SelectLocation(m_Industrial, out location))
				{
					return;
				}
				break;
			}
			Random random = m_RandomSeed.GetRandom(index);
			Spawn(index, location, ref random);
		}

		private bool SelectLocation(NativeQueue<SpawnLocation> queue, out SpawnLocation location)
		{
			location = default(SpawnLocation);
			SpawnLocation spawnLocation = default(SpawnLocation);
			while (queue.TryDequeue(ref spawnLocation))
			{
				if (spawnLocation.m_Priority > location.m_Priority)
				{
					location = spawnLocation;
				}
			}
			return location.m_Priority != 0f;
		}

		private void Spawn(int jobIndex, SpawnLocation location, ref Random random)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			BuildingData prefabBuildingData = m_PrefabBuildingData[location.m_Building];
			ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[location.m_Building];
			PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
			if (m_PrefabPlaceableObjectData.HasComponent(location.m_Building))
			{
				placeableObjectData = m_PrefabPlaceableObjectData[location.m_Building];
			}
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Prefab = location.m_Building
			};
			creationDefinition.m_Flags |= CreationFlags.Permanent | CreationFlags.Construction;
			creationDefinition.m_RandomSeed = ((Random)(ref random)).NextInt();
			Transform transform = default(Transform);
			if (m_BlockData.HasComponent(location.m_Entity))
			{
				Block block = m_BlockData[location.m_Entity];
				transform.m_Position = ZoneUtils.GetPosition(block, ((int4)(ref location.m_LotArea)).xz, ((int4)(ref location.m_LotArea)).yw);
				transform.m_Rotation = ZoneUtils.GetRotation(block);
			}
			else if (m_TransformData.HasComponent(location.m_Entity))
			{
				creationDefinition.m_Attached = location.m_Entity;
				creationDefinition.m_Flags |= CreationFlags.Attach;
				Transform transform2 = m_TransformData[location.m_Entity];
				PrefabRef prefabRef = m_PrefabRefData[location.m_Entity];
				BuildingData buildingData = m_PrefabBuildingData[prefabRef.m_Prefab];
				transform.m_Position = transform2.m_Position;
				transform.m_Rotation = transform2.m_Rotation;
				float num = (float)(buildingData.m_LotSize.y - prefabBuildingData.m_LotSize.y) * 4f;
				ref float3 position = ref transform.m_Position;
				position += math.rotate(transform.m_Rotation, new float3(0f, 0f, num));
			}
			float3 worldPosition = BuildingUtils.CalculateFrontPosition(transform, prefabBuildingData.m_LotSize.y);
			transform.m_Position.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, worldPosition);
			if ((placeableObjectData.m_Flags & (Game.Objects.PlacementFlags.Shoreline | Game.Objects.PlacementFlags.Floating)) == 0)
			{
				transform.m_Position.y += placeableObjectData.m_PlacementOffset.y;
			}
			float maxHeight = GetMaxHeight(transform, prefabBuildingData);
			transform.m_Position.y = math.min(transform.m_Position.y, maxHeight - objectGeometryData.m_Size.y - 0.1f);
			ObjectDefinition objectDefinition = new ObjectDefinition
			{
				m_ParentMesh = -1,
				m_Position = transform.m_Position,
				m_Rotation = transform.m_Rotation,
				m_LocalPosition = transform.m_Position,
				m_LocalRotation = transform.m_Rotation
			};
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_DefinitionArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CreationDefinition>(jobIndex, val, creationDefinition);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ObjectDefinition>(jobIndex, val, objectDefinition);
			OwnerDefinition ownerDefinition = new OwnerDefinition
			{
				m_Prefab = location.m_Building,
				m_Position = objectDefinition.m_Position,
				m_Rotation = objectDefinition.m_Rotation
			};
			if (m_PrefabSubAreas.HasBuffer(location.m_Building))
			{
				Spawn(jobIndex, ownerDefinition, m_PrefabSubAreas[location.m_Building], m_PrefabSubAreaNodes[location.m_Building], prefabBuildingData, ref random);
			}
			if (m_PrefabSubNets.HasBuffer(location.m_Building))
			{
				Spawn(jobIndex, ownerDefinition, m_PrefabSubNets[location.m_Building], ref random);
			}
		}

		private float GetMaxHeight(Transform transform, BuildingData prefabBuildingData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
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
			return iterator.m_MaxHeight;
		}

		private void Spawn(int jobIndex, OwnerDefinition ownerDefinition, DynamicBuffer<Game.Prefabs.SubArea> subAreas, DynamicBuffer<SubAreaNode> subAreaNodes, BuildingData prefabBuildingData, ref Random random)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelHashMap<Entity, int> selectedSpawnables = default(NativeParallelHashMap<Entity, int>);
			bool flag = false;
			DynamicBuffer<PlaceholderObjectElement> placeholderElements = default(DynamicBuffer<PlaceholderObjectElement>);
			for (int i = 0; i < subAreas.Length; i++)
			{
				Game.Prefabs.SubArea subArea = subAreas[i];
				AreaGeometryData areaGeometryData = m_PrefabAreaGeometryData[subArea.m_Prefab];
				if (areaGeometryData.m_Type == Game.Areas.AreaType.Surface)
				{
					if (flag)
					{
						continue;
					}
					subArea.m_Prefab = m_BuildingConfigurationData.m_ConstructionSurface;
					flag = true;
				}
				int seed;
				if (m_PrefabPlaceholderElements.TryGetBuffer(subArea.m_Prefab, ref placeholderElements))
				{
					if (!selectedSpawnables.IsCreated)
					{
						selectedSpawnables._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
					}
					if (!AreaUtils.SelectAreaPrefab(placeholderElements, m_PrefabSpawnableObjectData, selectedSpawnables, ref random, out subArea.m_Prefab, out seed))
					{
						continue;
					}
				}
				else
				{
					seed = ((Random)(ref random)).NextInt();
				}
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex);
				CreationDefinition creationDefinition = new CreationDefinition
				{
					m_Prefab = subArea.m_Prefab,
					m_RandomSeed = seed
				};
				creationDefinition.m_Flags |= CreationFlags.Permanent;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(jobIndex, val, creationDefinition);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, val, default(Updated));
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(jobIndex, val, ownerDefinition);
				DynamicBuffer<Game.Areas.Node> val2 = ((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<Game.Areas.Node>(jobIndex, val);
				if (areaGeometryData.m_Type == Game.Areas.AreaType.Surface)
				{
					Quad3 val3 = BuildingUtils.CalculateCorners(new Transform(ownerDefinition.m_Position, ownerDefinition.m_Rotation), prefabBuildingData.m_LotSize);
					val2.ResizeUninitialized(5);
					val2[0] = new Game.Areas.Node(val3.a, float.MinValue);
					val2[1] = new Game.Areas.Node(val3.b, float.MinValue);
					val2[2] = new Game.Areas.Node(val3.c, float.MinValue);
					val2[3] = new Game.Areas.Node(val3.d, float.MinValue);
					val2[4] = new Game.Areas.Node(val3.a, float.MinValue);
					continue;
				}
				val2.ResizeUninitialized(subArea.m_NodeRange.y - subArea.m_NodeRange.x + 1);
				int num = ObjectToolBaseSystem.GetFirstNodeIndex(subAreaNodes, subArea.m_NodeRange);
				int num2 = 0;
				for (int j = subArea.m_NodeRange.x; j <= subArea.m_NodeRange.y; j++)
				{
					float3 position = subAreaNodes[num].m_Position;
					float3 position2 = ObjectUtils.LocalToWorld(ownerDefinition.m_Position, ownerDefinition.m_Rotation, position);
					int parentMesh = subAreaNodes[num].m_ParentMesh;
					float elevation = math.select(float.MinValue, position.y, parentMesh >= 0);
					val2[num2] = new Game.Areas.Node(position2, elevation);
					num2++;
					if (++num == subArea.m_NodeRange.y)
					{
						num = subArea.m_NodeRange.x;
					}
				}
			}
			if (selectedSpawnables.IsCreated)
			{
				selectedSpawnables.Dispose();
			}
		}

		private void Spawn(int jobIndex, OwnerDefinition ownerDefinition, DynamicBuffer<Game.Prefabs.SubNet> subNets, ref Random random)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			NativeList<float4> nodePositions = default(NativeList<float4>);
			nodePositions._002Ector(subNets.Length * 2, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < subNets.Length; i++)
			{
				Game.Prefabs.SubNet subNet = subNets[i];
				if (subNet.m_NodeIndex.x >= 0)
				{
					while (nodePositions.Length <= subNet.m_NodeIndex.x)
					{
						float4 val = default(float4);
						nodePositions.Add(ref val);
					}
					ref NativeList<float4> reference = ref nodePositions;
					int x = subNet.m_NodeIndex.x;
					reference[x] += new float4(subNet.m_Curve.a, 1f);
				}
				if (subNet.m_NodeIndex.y >= 0)
				{
					while (nodePositions.Length <= subNet.m_NodeIndex.y)
					{
						float4 val = default(float4);
						nodePositions.Add(ref val);
					}
					ref NativeList<float4> reference = ref nodePositions;
					int x = subNet.m_NodeIndex.y;
					reference[x] += new float4(subNet.m_Curve.d, 1f);
				}
			}
			for (int j = 0; j < nodePositions.Length; j++)
			{
				ref NativeList<float4> reference = ref nodePositions;
				int x = j;
				reference[x] /= math.max(1f, nodePositions[j].w);
			}
			for (int k = 0; k < subNets.Length; k++)
			{
				Game.Prefabs.SubNet subNet2 = NetUtils.GetSubNet(subNets, k, m_LefthandTraffic, ref m_PrefabNetGeometryData);
				CreateSubNet(jobIndex, subNet2.m_Prefab, subNet2.m_Curve, subNet2.m_NodeIndex, subNet2.m_ParentMesh, subNet2.m_Upgrades, nodePositions, ownerDefinition, ref random);
			}
			nodePositions.Dispose();
		}

		private void CreateSubNet(int jobIndex, Entity netPrefab, Bezier4x3 curve, int2 nodeIndex, int2 parentMesh, CompositionFlags upgrades, NativeList<float4> nodePositions, OwnerDefinition ownerDefinition, ref Random random)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex);
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Prefab = netPrefab,
				m_RandomSeed = ((Random)(ref random)).NextInt()
			};
			creationDefinition.m_Flags |= CreationFlags.Permanent;
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(jobIndex, val, creationDefinition);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, val, default(Updated));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(jobIndex, val, ownerDefinition);
			NetCourse netCourse = default(NetCourse);
			netCourse.m_Curve = ObjectUtils.LocalToWorld(ownerDefinition.m_Position, ownerDefinition.m_Rotation, curve);
			netCourse.m_StartPosition.m_Position = netCourse.m_Curve.a;
			netCourse.m_StartPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.StartTangent(netCourse.m_Curve), ownerDefinition.m_Rotation);
			netCourse.m_StartPosition.m_CourseDelta = 0f;
			netCourse.m_StartPosition.m_Elevation = float2.op_Implicit(curve.a.y);
			netCourse.m_StartPosition.m_ParentMesh = parentMesh.x;
			float4 val2;
			if (nodeIndex.x >= 0)
			{
				ref CoursePos startPosition = ref netCourse.m_StartPosition;
				float3 position = ownerDefinition.m_Position;
				quaternion rotation = ownerDefinition.m_Rotation;
				val2 = nodePositions[nodeIndex.x];
				startPosition.m_Position = ObjectUtils.LocalToWorld(position, rotation, ((float4)(ref val2)).xyz);
			}
			netCourse.m_EndPosition.m_Position = netCourse.m_Curve.d;
			netCourse.m_EndPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.EndTangent(netCourse.m_Curve), ownerDefinition.m_Rotation);
			netCourse.m_EndPosition.m_CourseDelta = 1f;
			netCourse.m_EndPosition.m_Elevation = float2.op_Implicit(curve.d.y);
			netCourse.m_EndPosition.m_ParentMesh = parentMesh.y;
			if (nodeIndex.y >= 0)
			{
				ref CoursePos endPosition = ref netCourse.m_EndPosition;
				float3 position2 = ownerDefinition.m_Position;
				quaternion rotation2 = ownerDefinition.m_Rotation;
				val2 = nodePositions[nodeIndex.y];
				endPosition.m_Position = ObjectUtils.LocalToWorld(position2, rotation2, ((float4)(ref val2)).xyz);
			}
			netCourse.m_Length = MathUtils.Length(netCourse.m_Curve);
			netCourse.m_FixedIndex = -1;
			netCourse.m_StartPosition.m_Flags |= CoursePosFlags.IsFirst | CoursePosFlags.DisableMerge;
			netCourse.m_EndPosition.m_Flags |= CoursePosFlags.IsLast | CoursePosFlags.DisableMerge;
			if (((float3)(ref netCourse.m_StartPosition.m_Position)).Equals(netCourse.m_EndPosition.m_Position))
			{
				netCourse.m_StartPosition.m_Flags |= CoursePosFlags.IsLast;
				netCourse.m_EndPosition.m_Flags |= CoursePosFlags.IsFirst;
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<NetCourse>(jobIndex, val, netCourse);
			if (upgrades != default(CompositionFlags))
			{
				Upgraded upgraded = new Upgraded
				{
					m_Flags = upgrades
				};
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Upgraded>(jobIndex, val, upgraded);
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Block> __Game_Zones_Block_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurvePosition> __Game_Zones_CurvePosition_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<VacantLot> __Game_Zones_VacantLot_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentTypeHandle;

		public SharedComponentTypeHandle<BuildingSpawnGroupData> __Game_Prefabs_BuildingSpawnGroupData_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WarehouseData> __Game_Prefabs_WarehouseData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZonePropertiesData> __Game_Prefabs_ZonePropertiesData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ResourceAvailability> __Game_Net_ResourceAvailability_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<LandValue> __Game_Net_LandValue_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Block> __Game_Zones_Block_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ProcessEstimate> __Game_Zones_ProcessEstimate_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ValidArea> __Game_Zones_ValidArea_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> __Game_Prefabs_PlaceableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> __Game_Prefabs_SpawnableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Cell> __Game_Zones_Cell_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubArea> __Game_Prefabs_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubAreaNode> __Game_Prefabs_SubAreaNode_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubNet> __Game_Prefabs_SubNet_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> __Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup;

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
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Zones_Block_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Block>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Zones_CurvePosition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurvePosition>(true);
			__Game_Zones_VacantLot_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<VacantLot>(true);
			__Game_Prefabs_BuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildingData>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SpawnableBuildingData>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildingPropertyData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ObjectGeometryData>(true);
			__Game_Prefabs_BuildingSpawnGroupData_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<BuildingSpawnGroupData>();
			__Game_Prefabs_WarehouseData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WarehouseData>(true);
			__Game_Prefabs_ZoneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneData>(true);
			__Game_Prefabs_ZonePropertiesData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZonePropertiesData>(true);
			__Game_Net_ResourceAvailability_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ResourceAvailability>(true);
			__Game_Net_LandValue_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LandValue>(true);
			__Game_Zones_Block_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Block>(true);
			__Game_Zones_ProcessEstimate_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ProcessEstimate>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Zones_ValidArea_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ValidArea>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableObjectData>(true);
			__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableObjectData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_AreaGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaGeometryData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Zones_Cell_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Cell>(true);
			__Game_Prefabs_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubArea>(true);
			__Game_Prefabs_SubAreaNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubAreaNode>(true);
			__Game_Prefabs_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubNet>(true);
			__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PlaceholderObjectElement>(true);
		}
	}

	private ZoneSystem m_ZoneSystem;

	private ResidentialDemandSystem m_ResidentialDemandSystem;

	private CommercialDemandSystem m_CommercialDemandSystem;

	private IndustrialDemandSystem m_IndustrialDemandSystem;

	private GroundPollutionSystem m_PollutionSystem;

	private TerrainSystem m_TerrainSystem;

	private Game.Zones.SearchSystem m_SearchSystem;

	private ResourceSystem m_ResourceSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_LotQuery;

	private EntityQuery m_BuildingQuery;

	private EntityQuery m_ProcessQuery;

	private EntityQuery m_BuildingConfigurationQuery;

	private EntityArchetype m_DefinitionArchetype;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1944910157_0;

	public bool debugFastSpawn { get; set; }

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 13;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ZoneSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ZoneSystem>();
		m_ResidentialDemandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResidentialDemandSystem>();
		m_CommercialDemandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CommercialDemandSystem>();
		m_IndustrialDemandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IndustrialDemandSystem>();
		m_PollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_SearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Zones.SearchSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Block>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.ReadOnly<CurvePosition>(),
			ComponentType.ReadOnly<VacantLot>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[0];
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Temp>(),
			ComponentType.ReadWrite<Deleted>()
		};
		array[0] = val;
		m_LotQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_BuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<BuildingData>(),
			ComponentType.ReadOnly<SpawnableBuildingData>(),
			ComponentType.ReadOnly<BuildingSpawnGroupData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_DefinitionArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<CreationDefinition>(),
			ComponentType.ReadWrite<ObjectDefinition>(),
			ComponentType.ReadWrite<Updated>(),
			ComponentType.ReadWrite<Deleted>()
		});
		m_ProcessQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<IndustrialProcessData>() });
		m_BuildingConfigurationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BuildingConfigurationData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_LotQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_BuildingQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_067a: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_0685: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_069b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_0740: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0767: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		RandomSeed.Next().GetRandom(0);
		bool flag = debugFastSpawn || (m_ResidentialDemandSystem.buildingDemand.x + m_ResidentialDemandSystem.buildingDemand.y + m_ResidentialDemandSystem.buildingDemand.z) / 3 > 0;
		bool flag2 = debugFastSpawn || m_CommercialDemandSystem.buildingDemand > 0;
		bool flag3 = debugFastSpawn || (m_IndustrialDemandSystem.industrialBuildingDemand + m_IndustrialDemandSystem.officeBuildingDemand) / 2 > 0;
		bool flag4 = debugFastSpawn || m_IndustrialDemandSystem.storageBuildingDemand > 0;
		NativeQueue<SpawnLocation> residential = default(NativeQueue<SpawnLocation>);
		residential._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<SpawnLocation> commercial = default(NativeQueue<SpawnLocation>);
		commercial._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<SpawnLocation> industrial = default(NativeQueue<SpawnLocation>);
		industrial._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle val = default(JobHandle);
		JobHandle deps;
		JobHandle deps2;
		JobHandle deps3;
		JobHandle val2 = default(JobHandle);
		JobHandle dependencies;
		EvaluateSpawnAreas evaluateSpawnAreas = new EvaluateSpawnAreas
		{
			m_BuildingChunks = ((EntityQuery)(ref m_BuildingQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
			m_ZonePrefabs = m_ZoneSystem.GetPrefabs(),
			m_Preferences = ((EntityQuery)(ref __query_1944910157_0)).GetSingleton<ZonePreferenceData>(),
			m_SpawnResidential = (flag ? 1 : 0),
			m_SpawnCommercial = (flag2 ? 1 : 0),
			m_SpawnIndustrial = (flag3 ? 1 : 0),
			m_SpawnStorage = (flag4 ? 1 : 0),
			m_MinDemand = ((!debugFastSpawn) ? 1 : 0),
			m_ResidentialDemands = m_ResidentialDemandSystem.buildingDemand,
			m_CommercialBuildingDemands = m_CommercialDemandSystem.GetBuildingDemands(out deps),
			m_IndustrialDemands = m_IndustrialDemandSystem.GetBuildingDemands(out deps2),
			m_StorageDemands = m_IndustrialDemandSystem.GetStorageBuildingDemands(out deps3),
			m_RandomSeed = RandomSeed.Next(),
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BlockType = InternalCompilerInterface.GetComponentTypeHandle<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurvePositionType = InternalCompilerInterface.GetComponentTypeHandle<CurvePosition>(ref __TypeHandle.__Game_Zones_CurvePosition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_VacantLotType = InternalCompilerInterface.GetBufferTypeHandle<VacantLot>(ref __TypeHandle.__Game_Zones_VacantLot_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingDataType = InternalCompilerInterface.GetComponentTypeHandle<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableBuildingType = InternalCompilerInterface.GetComponentTypeHandle<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingPropertyType = InternalCompilerInterface.GetComponentTypeHandle<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectGeometryType = InternalCompilerInterface.GetComponentTypeHandle<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingSpawnGroupType = InternalCompilerInterface.GetSharedComponentTypeHandle<BuildingSpawnGroupData>(ref __TypeHandle.__Game_Prefabs_BuildingSpawnGroupData_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WarehouseType = InternalCompilerInterface.GetComponentTypeHandle<WarehouseData>(ref __TypeHandle.__Game_Prefabs_WarehouseData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneData = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZonePropertiesDatas = InternalCompilerInterface.GetComponentLookup<ZonePropertiesData>(ref __TypeHandle.__Game_Prefabs_ZonePropertiesData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Availabilities = InternalCompilerInterface.GetBufferLookup<ResourceAvailability>(ref __TypeHandle.__Game_Net_ResourceAvailability_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LandValues = InternalCompilerInterface.GetComponentLookup<LandValue>(ref __TypeHandle.__Game_Net_LandValue_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BlockData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Processes = ((EntityQuery)(ref m_ProcessQuery)).ToComponentDataListAsync<IndustrialProcessData>(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val2),
			m_ProcessEstimates = InternalCompilerInterface.GetBufferLookup<ProcessEstimate>(ref __TypeHandle.__Game_Zones_ProcessEstimate_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_PollutionMap = m_PollutionSystem.GetMap(readOnly: true, out dependencies),
			m_Residential = residential.AsParallelWriter(),
			m_Commercial = commercial.AsParallelWriter(),
			m_Industrial = industrial.AsParallelWriter()
		};
		JobHandle dependencies2;
		SpawnBuildingJob spawnBuildingJob = new SpawnBuildingJob
		{
			m_BlockData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ValidAreaData = InternalCompilerInterface.GetComponentLookup<ValidArea>(ref __TypeHandle.__Game_Zones_ValidArea_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPlaceableObjectData = InternalCompilerInterface.GetComponentLookup<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnableObjectData = InternalCompilerInterface.GetComponentLookup<SpawnableObjectData>(ref __TypeHandle.__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAreaGeometryData = InternalCompilerInterface.GetComponentLookup<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Cells = InternalCompilerInterface.GetBufferLookup<Cell>(ref __TypeHandle.__Game_Zones_Cell_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubAreas = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubArea>(ref __TypeHandle.__Game_Prefabs_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubAreaNodes = InternalCompilerInterface.GetBufferLookup<SubAreaNode>(ref __TypeHandle.__Game_Prefabs_SubAreaNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubNets = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubNet>(ref __TypeHandle.__Game_Prefabs_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPlaceholderElements = InternalCompilerInterface.GetBufferLookup<PlaceholderObjectElement>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DefinitionArchetype = m_DefinitionArchetype,
			m_RandomSeed = RandomSeed.Next(),
			m_LefthandTraffic = m_CityConfigurationSystem.leftHandTraffic,
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_ZoneSearchTree = m_SearchSystem.GetSearchTree(readOnly: true, out dependencies2),
			m_BuildingConfigurationData = ((EntityQuery)(ref m_BuildingConfigurationQuery)).GetSingleton<BuildingConfigurationData>(),
			m_Residential = residential,
			m_Commercial = commercial,
			m_Industrial = industrial
		};
		EntityCommandBuffer val3 = m_EndFrameBarrier.CreateCommandBuffer();
		spawnBuildingJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val3)).AsParallelWriter();
		SpawnBuildingJob spawnBuildingJob2 = spawnBuildingJob;
		JobHandle val4 = JobChunkExtensions.ScheduleParallel<EvaluateSpawnAreas>(evaluateSpawnAreas, m_LotQuery, JobUtils.CombineDependencies(val, deps, deps2, deps3, dependencies, ((SystemBase)this).Dependency, val2));
		JobHandle val5 = IJobParallelForExtensions.Schedule<SpawnBuildingJob>(spawnBuildingJob2, 3, 1, JobHandle.CombineDependencies(val4, dependencies2));
		m_ResourceSystem.AddPrefabsReader(val4);
		m_PollutionSystem.AddReader(val4);
		m_CommercialDemandSystem.AddReader(val4);
		m_IndustrialDemandSystem.AddReader(val4);
		residential.Dispose(val5);
		commercial.Dispose(val5);
		industrial.Dispose(val5);
		m_ZoneSystem.AddPrefabsReader(val4);
		m_TerrainSystem.AddCPUHeightReader(val5);
		m_EndFrameBarrier.AddJobHandleForProducer(val5);
		m_SearchSystem.AddSearchTreeReader(val5);
		((SystemBase)this).Dependency = val5;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<ZonePreferenceData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1944910157_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public ZoneSpawnSystem()
	{
	}
}
