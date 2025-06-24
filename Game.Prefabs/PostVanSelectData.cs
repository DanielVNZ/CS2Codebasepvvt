using Game.City;
using Game.Common;
using Game.Objects;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct PostVanSelectData
{
	private NativeList<ArchetypeChunk> m_PrefabChunks;

	private VehicleSelectRequirementData m_RequirementData;

	private EntityTypeHandle m_EntityType;

	private ComponentTypeHandle<PostVanData> m_PostVanType;

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
			ComponentType.ReadOnly<PostVanData>(),
			ComponentType.ReadOnly<CarData>(),
			ComponentType.ReadOnly<ObjectData>(),
			ComponentType.ReadOnly<PrefabData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Locked>() };
		return val;
	}

	public PostVanSelectData(SystemBase system)
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
		m_PostVanType = ((ComponentSystemBase)system).GetComponentTypeHandle<PostVanData>(true);
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
		m_PostVanType.Update(system);
		m_ObjectData.Update(system);
		m_MovingObjectData.Update(system);
	}

	public void PostUpdate(JobHandle jobHandle)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabChunks.Dispose(jobHandle);
	}

	public Entity CreateVehicle(EntityCommandBuffer commandBuffer, ref Random random, Transform transform, Entity source, Entity vehiclePrefab, bool parked)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (vehiclePrefab == Entity.Null)
		{
			return Entity.Null;
		}
		Entity val = ((EntityCommandBuffer)(ref commandBuffer)).CreateEntity(GetArchetype(vehiclePrefab, parked));
		((EntityCommandBuffer)(ref commandBuffer)).SetComponent<Transform>(val, transform);
		((EntityCommandBuffer)(ref commandBuffer)).SetComponent<PrefabRef>(val, new PrefabRef(vehiclePrefab));
		((EntityCommandBuffer)(ref commandBuffer)).SetComponent<PseudoRandomSeed>(val, new PseudoRandomSeed(ref random));
		if (!parked)
		{
			((EntityCommandBuffer)(ref commandBuffer)).AddComponent<TripSource>(val, new TripSource(source));
			((EntityCommandBuffer)(ref commandBuffer)).AddComponent<Unspawned>(val, default(Unspawned));
		}
		return val;
	}

	public Entity CreateVehicle(ParallelWriter commandBuffer, int jobIndex, ref Random random, Transform transform, Entity source, Entity vehiclePrefab, bool parked)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (vehiclePrefab == Entity.Null)
		{
			return Entity.Null;
		}
		Entity val = ((ParallelWriter)(ref commandBuffer)).CreateEntity(jobIndex, GetArchetype(vehiclePrefab, parked));
		((ParallelWriter)(ref commandBuffer)).SetComponent<Transform>(jobIndex, val, transform);
		((ParallelWriter)(ref commandBuffer)).SetComponent<PrefabRef>(jobIndex, val, new PrefabRef(vehiclePrefab));
		((ParallelWriter)(ref commandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val, new PseudoRandomSeed(ref random));
		if (!parked)
		{
			((ParallelWriter)(ref commandBuffer)).AddComponent<TripSource>(jobIndex, val, new TripSource(source));
			((ParallelWriter)(ref commandBuffer)).AddComponent<Unspawned>(jobIndex, val, default(Unspawned));
		}
		return val;
	}

	public Entity SelectVehicle(ref Random random, ref int2 mailCapacity)
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
		int num2 = -mailCapacity.x;
		int totalProbability = 0;
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PostVanData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PostVanData>(ref m_PostVanType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray2.Length; j++)
			{
				if (!m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				PostVanData postVanData = nativeArray2[j];
				int2 val = postVanData.m_MailCapacity - mailCapacity;
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
					num = postVanData.m_MailCapacity;
				}
			}
		}
		mailCapacity = int2.op_Implicit(num);
		return result;
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

	private bool PickVehicle(ref Random random, int probability, ref int totalProbability)
	{
		totalProbability += probability;
		return ((Random)(ref random)).NextInt(totalProbability) < probability;
	}
}
