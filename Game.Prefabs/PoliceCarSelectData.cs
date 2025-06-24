using Game.City;
using Game.Common;
using Game.Net;
using Game.Objects;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct PoliceCarSelectData
{
	private NativeList<ArchetypeChunk> m_PrefabChunks;

	private VehicleSelectRequirementData m_RequirementData;

	private EntityTypeHandle m_EntityType;

	private ComponentTypeHandle<PoliceCarData> m_PoliceCarType;

	private ComponentTypeHandle<CarData> m_CarType;

	private ComponentTypeHandle<HelicopterData> m_HelicopterType;

	private ComponentLookup<ObjectData> m_ObjectData;

	private ComponentLookup<MovingObjectData> m_MovingObjectData;

	public static EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PoliceCarData>(),
			ComponentType.ReadOnly<ObjectData>(),
			ComponentType.ReadOnly<PrefabData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CarData>(),
			ComponentType.ReadOnly<HelicopterData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Locked>() };
		return val;
	}

	public PoliceCarSelectData(SystemBase system)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabChunks = default(NativeList<ArchetypeChunk>);
		m_RequirementData = new VehicleSelectRequirementData(system);
		m_EntityType = ((ComponentSystemBase)system).GetEntityTypeHandle();
		m_PoliceCarType = ((ComponentSystemBase)system).GetComponentTypeHandle<PoliceCarData>(true);
		m_CarType = ((ComponentSystemBase)system).GetComponentTypeHandle<CarData>(true);
		m_HelicopterType = ((ComponentSystemBase)system).GetComponentTypeHandle<HelicopterData>(true);
		m_ObjectData = system.GetComponentLookup<ObjectData>(true);
		m_MovingObjectData = system.GetComponentLookup<MovingObjectData>(true);
	}

	public void PreUpdate(SystemBase system, CityConfigurationSystem cityConfigurationSystem, EntityQuery query, Allocator allocator, out JobHandle jobHandle)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabChunks = ((EntityQuery)(ref query)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(allocator), ref jobHandle);
		m_RequirementData.Update(system, cityConfigurationSystem);
		((EntityTypeHandle)(ref m_EntityType)).Update(system);
		m_PoliceCarType.Update(system);
		m_CarType.Update(system);
		m_HelicopterType.Update(system);
		m_ObjectData.Update(system);
		m_MovingObjectData.Update(system);
	}

	public void PostUpdate(JobHandle jobHandle)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabChunks.Dispose(jobHandle);
	}

	public Entity SelectVehicle(ref Random random, ref PolicePurpose purposeMask, RoadTypes roadType)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return GetRandomVehicle(ref random, ref purposeMask, roadType);
	}

	public Entity CreateVehicle(EntityCommandBuffer commandBuffer, ref Random random, Transform transform, Entity source, Entity prefab, ref PolicePurpose purposeMask, RoadTypes roadType, bool parked)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (prefab == Entity.Null)
		{
			prefab = GetRandomVehicle(ref random, ref purposeMask, roadType);
			if (prefab == Entity.Null)
			{
				return Entity.Null;
			}
		}
		Entity val = ((EntityCommandBuffer)(ref commandBuffer)).CreateEntity(GetArchetype(prefab, parked));
		((EntityCommandBuffer)(ref commandBuffer)).SetComponent<Transform>(val, transform);
		((EntityCommandBuffer)(ref commandBuffer)).SetComponent<PrefabRef>(val, new PrefabRef(prefab));
		((EntityCommandBuffer)(ref commandBuffer)).SetComponent<PseudoRandomSeed>(val, new PseudoRandomSeed(ref random));
		if (!parked)
		{
			((EntityCommandBuffer)(ref commandBuffer)).AddComponent<TripSource>(val, new TripSource(source));
			((EntityCommandBuffer)(ref commandBuffer)).AddComponent<Unspawned>(val, default(Unspawned));
		}
		return val;
	}

	public Entity CreateVehicle(ParallelWriter commandBuffer, int jobIndex, ref Random random, Transform transform, Entity source, Entity prefab, ref PolicePurpose purposeMask, RoadTypes roadType, bool parked)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (prefab == Entity.Null)
		{
			prefab = GetRandomVehicle(ref random, ref purposeMask, roadType);
			if (prefab == Entity.Null)
			{
				return Entity.Null;
			}
		}
		Entity val = ((ParallelWriter)(ref commandBuffer)).CreateEntity(jobIndex, GetArchetype(prefab, parked));
		((ParallelWriter)(ref commandBuffer)).SetComponent<Transform>(jobIndex, val, transform);
		((ParallelWriter)(ref commandBuffer)).SetComponent<PrefabRef>(jobIndex, val, new PrefabRef(prefab));
		((ParallelWriter)(ref commandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val, new PseudoRandomSeed(ref random));
		if (!parked)
		{
			((ParallelWriter)(ref commandBuffer)).AddComponent<TripSource>(jobIndex, val, new TripSource(source));
			((ParallelWriter)(ref commandBuffer)).AddComponent<Unspawned>(jobIndex, val, default(Unspawned));
		}
		return val;
	}

	private EntityArchetype GetArchetype(Entity prefab, bool parked)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (parked)
		{
			return m_MovingObjectData[prefab].m_StoppedArchetype;
		}
		return m_ObjectData[prefab].m_Archetype;
	}

	private Entity GetRandomVehicle(ref Random random, ref PolicePurpose purposeMask, RoadTypes roadType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		Entity result = Entity.Null;
		PolicePurpose policePurpose = (PolicePurpose)0;
		int totalProbability = 0;
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			if (roadType != RoadTypes.Car)
			{
				if (roadType != RoadTypes.Helicopter || !((ArchetypeChunk)(ref chunk)).Has<HelicopterData>(ref m_HelicopterType))
				{
					continue;
				}
			}
			else if (!((ArchetypeChunk)(ref chunk)).Has<CarData>(ref m_CarType))
			{
				continue;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PoliceCarData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PoliceCarData>(ref m_PoliceCarType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray2.Length; j++)
			{
				PoliceCarData policeCarData = nativeArray2[j];
				if ((policeCarData.m_PurposeMask & purposeMask) != 0 && m_RequirementData.CheckRequirements(ref chunk2, j) && PickVehicle(ref random, 100, ref totalProbability))
				{
					result = nativeArray[j];
					policePurpose = policeCarData.m_PurposeMask;
				}
			}
		}
		purposeMask = policePurpose;
		return result;
	}

	private bool PickVehicle(ref Random random, int probability, ref int totalProbability)
	{
		totalProbability += probability;
		return ((Random)(ref random)).NextInt(totalProbability) < probability;
	}
}
