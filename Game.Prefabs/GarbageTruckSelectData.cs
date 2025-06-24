using Game.City;
using Game.Common;
using Game.Objects;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct GarbageTruckSelectData
{
	private NativeList<ArchetypeChunk> m_PrefabChunks;

	private VehicleSelectRequirementData m_RequirementData;

	private EntityTypeHandle m_EntityType;

	private ComponentTypeHandle<GarbageTruckData> m_GarbageTruckType;

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
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<GarbageTruckData>(),
			ComponentType.ReadOnly<CarData>(),
			ComponentType.ReadOnly<ObjectData>(),
			ComponentType.ReadOnly<PrefabData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Locked>() };
		return val;
	}

	public GarbageTruckSelectData(SystemBase system)
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
		m_PrefabChunks = default(NativeList<ArchetypeChunk>);
		m_RequirementData = new VehicleSelectRequirementData(system);
		m_EntityType = ((ComponentSystemBase)system).GetEntityTypeHandle();
		m_GarbageTruckType = ((ComponentSystemBase)system).GetComponentTypeHandle<GarbageTruckData>(true);
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
		m_GarbageTruckType.Update(system);
		m_ObjectData.Update(system);
		m_MovingObjectData.Update(system);
	}

	public void PostUpdate(JobHandle jobHandle)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabChunks.Dispose(jobHandle);
	}

	public Entity SelectVehicle(ref Random random, ref int2 garbageCapacity)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return GetRandomVehicle(ref random, ref garbageCapacity);
	}

	public Entity CreateVehicle(EntityCommandBuffer commandBuffer, ref Random random, Transform transform, Entity source, Entity prefab, ref int2 garbageCapacity, bool parked)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (prefab == Entity.Null)
		{
			prefab = GetRandomVehicle(ref random, ref garbageCapacity);
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

	public Entity CreateVehicle(ParallelWriter commandBuffer, int jobIndex, ref Random random, Transform transform, Entity source, Entity prefab, ref int2 garbageCapacity, bool parked)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (prefab == Entity.Null)
		{
			prefab = GetRandomVehicle(ref random, ref garbageCapacity);
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

	private Entity GetRandomVehicle(ref Random random, ref int2 garbageCapacity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		Entity result = Entity.Null;
		int num = 0;
		int num2 = -garbageCapacity.x;
		int totalProbability = 0;
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<GarbageTruckData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GarbageTruckData>(ref m_GarbageTruckType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray2.Length; j++)
			{
				if (!m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				GarbageTruckData garbageTruckData = nativeArray2[j];
				int2 val = garbageTruckData.m_GarbageCapacity - garbageCapacity;
				int num3 = math.max(math.min(0, val.x), val.y);
				if (num3 != num2)
				{
					if ((num3 < 0 && num2 > num3) || (num2 >= 0 && num2 < num3))
					{
						continue;
					}
					num2 = num3;
					totalProbability = 0;
				}
				if (PickVehicle(ref random, 100, ref totalProbability))
				{
					result = nativeArray[j];
					num = garbageTruckData.m_GarbageCapacity;
				}
			}
		}
		garbageCapacity = int2.op_Implicit(num);
		return result;
	}

	private bool PickVehicle(ref Random random, int probability, ref int totalProbability)
	{
		totalProbability += probability;
		return ((Random)(ref random)).NextInt(totalProbability) < probability;
	}
}
