using Game.Areas;
using Game.Buildings;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public static class EventHelpers
{
	public struct FireHazardData
	{
		public LocalEffectSystem.ReadData m_LocalEffectData;

		public float m_ForestFireHazardFactor;

		public ComponentLookup<DestructibleObjectData> m_PrefabDestructibleObjectData;

		public ComponentLookup<SpawnableBuildingData> m_PrefabSpawnableBuildingData;

		public BufferLookup<Game.Net.ServiceCoverage> m_ServiceCoverages;

		public BufferLookup<DistrictModifier> m_DistrictModifiers;

		public ComponentLookup<ZonePropertiesData> m_ZonePropertiesData;

		public FireHazardData(SystemBase system)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			m_LocalEffectData = default(LocalEffectSystem.ReadData);
			m_ForestFireHazardFactor = 0f;
			m_PrefabDestructibleObjectData = system.GetComponentLookup<DestructibleObjectData>(true);
			m_PrefabSpawnableBuildingData = system.GetComponentLookup<SpawnableBuildingData>(true);
			m_ServiceCoverages = system.GetBufferLookup<Game.Net.ServiceCoverage>(true);
			m_DistrictModifiers = system.GetBufferLookup<DistrictModifier>(true);
			m_ZonePropertiesData = system.GetComponentLookup<ZonePropertiesData>(true);
		}

		public void Update(SystemBase system, LocalEffectSystem.ReadData localEffectData, FireConfigurationPrefab fireConfigurationPrefab, float temperature, float noRainDays)
		{
			m_LocalEffectData = localEffectData;
			m_ForestFireHazardFactor = fireConfigurationPrefab.m_TemperatureForestFireHazard.Evaluate(temperature);
			m_ForestFireHazardFactor *= fireConfigurationPrefab.m_NoRainForestFireHazard.Evaluate(noRainDays);
			m_PrefabDestructibleObjectData.Update(system);
			m_PrefabSpawnableBuildingData.Update(system);
			m_ServiceCoverages.Update(system);
			m_DistrictModifiers.Update(system);
			m_ZonePropertiesData.Update(system);
		}

		public bool GetFireHazard(PrefabRef prefabRef, Building building, CurrentDistrict currentDistrict, Damaged damaged, UnderConstruction underConstruction, out float fireHazard, out float riskFactor)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			fireHazard = GetFireHazard(prefabRef);
			fireHazard = math.select(fireHazard, 0f, underConstruction.m_NewPrefab == Entity.Null && underConstruction.m_Progress < byte.MaxValue);
			riskFactor = 0f;
			if (fireHazard == 0f)
			{
				return false;
			}
			SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
			if (m_PrefabSpawnableBuildingData.TryGetComponent(prefabRef.m_Prefab, ref spawnableBuildingData))
			{
				fireHazard *= 1f - (float)(spawnableBuildingData.m_Level - 1) * 0.03f;
				ZonePropertiesData zonePropertiesData = default(ZonePropertiesData);
				if (m_ZonePropertiesData.TryGetComponent(spawnableBuildingData.m_ZonePrefab, ref zonePropertiesData))
				{
					fireHazard *= zonePropertiesData.m_FireHazardMultiplier;
				}
			}
			float num = 0f;
			DynamicBuffer<Game.Net.ServiceCoverage> coverages = default(DynamicBuffer<Game.Net.ServiceCoverage>);
			if (m_ServiceCoverages.TryGetBuffer(building.m_RoadEdge, ref coverages))
			{
				num = NetUtils.GetServiceCoverage(coverages, CoverageService.FireRescue, building.m_CurvePosition);
				fireHazard *= math.max(0.01f, 1f - num * 0.01f);
			}
			DynamicBuffer<DistrictModifier> modifiers = default(DynamicBuffer<DistrictModifier>);
			if (m_DistrictModifiers.TryGetBuffer(currentDistrict.m_District, ref modifiers))
			{
				AreaUtils.ApplyModifier(ref fireHazard, modifiers, DistrictModifierType.BuildingFireHazard);
			}
			riskFactor = fireHazard / (1f + num * 0.5f);
			fireHazard *= GetFireHazardFactor(damaged);
			return true;
		}

		public bool GetFireHazard(PrefabRef prefabRef, Tree tree, Transform transform, Damaged damaged, out float fireHazard, out float riskFactor)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			fireHazard = GetFireHazard(prefabRef);
			riskFactor = 0f;
			if (fireHazard == 0f)
			{
				return false;
			}
			fireHazard *= m_ForestFireHazardFactor;
			m_LocalEffectData.ApplyModifier(ref fireHazard, transform.m_Position, LocalModifierType.ForestFireHazard);
			riskFactor = fireHazard;
			m_LocalEffectData.ApplyModifier(ref riskFactor, transform.m_Position, LocalModifierType.ForestFireResponseTime);
			fireHazard *= GetFireHazardFactor(damaged);
			return true;
		}

		private float GetFireHazard(PrefabRef prefabRef)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (m_PrefabDestructibleObjectData.HasComponent(prefabRef.m_Prefab))
			{
				return m_PrefabDestructibleObjectData[prefabRef.m_Prefab].m_FireHazard;
			}
			return 100f;
		}

		private float GetFireHazardFactor(Damaged damaged)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			float num = math.max(0f, 1f - math.csum(((float3)(ref damaged.m_Damage)).yz));
			float num2 = num * num;
			return num2 * num2;
		}
	}

	public struct StructuralIntegrityData
	{
		public ComponentLookup<SpawnableBuildingData> m_PrefabSpawnableBuildingData;

		public ComponentLookup<DestructibleObjectData> m_PrefabDestructibleObjectData;

		public FireConfigurationData m_FireConfigurationData;

		public StructuralIntegrityData(SystemBase system)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			m_PrefabSpawnableBuildingData = system.GetComponentLookup<SpawnableBuildingData>(true);
			m_PrefabDestructibleObjectData = system.GetComponentLookup<DestructibleObjectData>(true);
			m_FireConfigurationData = default(FireConfigurationData);
		}

		public void Update(SystemBase system, FireConfigurationData fireConfigurationData)
		{
			m_PrefabSpawnableBuildingData.Update(system);
			m_PrefabDestructibleObjectData.Update(system);
			m_FireConfigurationData = fireConfigurationData;
		}

		public float GetStructuralIntegrity(Entity prefab, bool isBuilding)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			if (m_PrefabDestructibleObjectData.HasComponent(prefab))
			{
				return m_PrefabDestructibleObjectData[prefab].m_StructuralIntegrity;
			}
			if (isBuilding)
			{
				if (m_PrefabSpawnableBuildingData.HasComponent(prefab))
				{
					return m_PrefabSpawnableBuildingData[prefab].m_Level switch
					{
						1 => m_FireConfigurationData.m_StructuralIntegrityLevel1, 
						2 => m_FireConfigurationData.m_StructuralIntegrityLevel2, 
						3 => m_FireConfigurationData.m_StructuralIntegrityLevel3, 
						4 => m_FireConfigurationData.m_StructuralIntegrityLevel4, 
						5 => m_FireConfigurationData.m_StructuralIntegrityLevel5, 
						_ => m_FireConfigurationData.m_BuildingStructuralIntegrity, 
					};
				}
				return m_FireConfigurationData.m_BuildingStructuralIntegrity;
			}
			return m_FireConfigurationData.m_DefaultStructuralIntegrity;
		}
	}
}
