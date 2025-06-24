using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Buildings;

public static class ValidationHelpers
{
	public static void ValidateBuilding(Entity entity, Building building, Transform transform, PrefabRef prefabRef, ValidationSystem.EntityData data, NativeArray<GroundWater> groundWaterMap, ParallelWriter<ErrorData> errorQueue)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		if (building.m_RoadEdge == Entity.Null)
		{
			BuildingData buildingData = data.m_PrefabBuilding[prefabRef.m_Prefab];
			if ((buildingData.m_Flags & Game.Prefabs.BuildingFlags.RequireRoad) != 0)
			{
				float3 position = BuildingUtils.CalculateFrontPosition(transform, buildingData.m_LotSize.y);
				errorQueue.Enqueue(new ErrorData
				{
					m_ErrorSeverity = ErrorSeverity.Warning,
					m_ErrorType = ErrorType.NoRoadAccess,
					m_TempEntity = entity,
					m_Position = position
				});
			}
		}
		WaterPumpingStationData waterPumpingStationData = default(WaterPumpingStationData);
		if (((data.m_WaterPumpingStationData.TryGetComponent(prefabRef.m_Prefab, ref waterPumpingStationData) && (waterPumpingStationData.m_Types & AllowedWaterTypes.Groundwater) != AllowedWaterTypes.None) || data.m_GroundWaterPoweredData.HasComponent(prefabRef.m_Prefab)) && GroundWaterSystem.GetGroundWater(transform.m_Position, groundWaterMap).m_Max <= 500)
		{
			errorQueue.Enqueue(new ErrorData
			{
				m_ErrorSeverity = ErrorSeverity.Error,
				m_ErrorType = ErrorType.NoGroundWater,
				m_TempEntity = entity,
				m_Position = transform.m_Position
			});
		}
	}

	public static void ValidateUpgrade(Entity entity, Owner owner, PrefabRef prefabRef, ValidationSystem.EntityData data, ParallelWriter<ErrorData> errorQueue)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		if (data.m_PrefabBuilding.HasComponent(prefabRef.m_Prefab) || !data.m_Upgrades.HasBuffer(owner.m_Owner))
		{
			return;
		}
		DynamicBuffer<InstalledUpgrade> val = data.m_Upgrades[owner.m_Owner];
		for (int i = 0; i < val.Length; i++)
		{
			Entity upgrade = val[i].m_Upgrade;
			if (upgrade != entity && data.m_PrefabRef[upgrade].m_Prefab == prefabRef.m_Prefab)
			{
				errorQueue.Enqueue(new ErrorData
				{
					m_ErrorSeverity = ErrorSeverity.Error,
					m_ErrorType = ErrorType.AlreadyUpgraded,
					m_TempEntity = entity,
					m_PermanentEntity = owner.m_Owner,
					m_Position = data.m_Transform[owner.m_Owner].m_Position
				});
			}
		}
	}
}
