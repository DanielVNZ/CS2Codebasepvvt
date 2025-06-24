using Game.City;
using Game.Common;
using Game.Objects;
using Game.Simulation;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct MaintenanceVehicleSelectData
{
	private NativeList<ArchetypeChunk> m_PrefabChunks;

	private VehicleSelectRequirementData m_RequirementData;

	private EntityTypeHandle m_EntityType;

	private ComponentTypeHandle<MaintenanceVehicleData> m_MaintenanceVehicleType;

	private ComponentTypeHandle<ObjectGeometryData> m_ObjectGeometryDataType;

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
			ComponentType.ReadOnly<MaintenanceVehicleData>(),
			ComponentType.ReadOnly<CarData>(),
			ComponentType.ReadOnly<ObjectData>(),
			ComponentType.ReadOnly<PrefabData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Locked>() };
		return val;
	}

	public MaintenanceVehicleSelectData(SystemBase system)
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
		m_PrefabChunks = default(NativeList<ArchetypeChunk>);
		m_RequirementData = new VehicleSelectRequirementData(system);
		m_EntityType = ((ComponentSystemBase)system).GetEntityTypeHandle();
		m_MaintenanceVehicleType = ((ComponentSystemBase)system).GetComponentTypeHandle<MaintenanceVehicleData>(true);
		m_ObjectGeometryDataType = ((ComponentSystemBase)system).GetComponentTypeHandle<ObjectGeometryData>(true);
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
		m_MaintenanceVehicleType.Update(system);
		m_ObjectGeometryDataType.Update(system);
		m_ObjectData.Update(system);
		m_MovingObjectData.Update(system);
	}

	public void PostUpdate(JobHandle jobHandle)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabChunks.Dispose(jobHandle);
	}

	public Entity SelectVehicle(ref Random random, MaintenanceType allMaintenanceTypes, MaintenanceType anyMaintenanceTypes, float4 maxParkingSizes)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return GetRandomVehicle(ref random, allMaintenanceTypes, anyMaintenanceTypes, maxParkingSizes);
	}

	public Entity CreateVehicle(EntityCommandBuffer commandBuffer, ref Random random, Transform transform, Entity source, Entity prefab, MaintenanceType allMaintenanceTypes, MaintenanceType anyMaintenanceTypes, float4 maxParkingSizes, bool parked)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (prefab == Entity.Null)
		{
			prefab = GetRandomVehicle(ref random, allMaintenanceTypes, anyMaintenanceTypes, maxParkingSizes);
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

	public Entity CreateVehicle(ParallelWriter commandBuffer, int jobIndex, ref Random random, Transform transform, Entity source, Entity prefab, MaintenanceType allMaintenanceTypes, MaintenanceType anyMaintenanceTypes, float4 maxParkingSizes, bool parked)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (prefab == Entity.Null)
		{
			prefab = GetRandomVehicle(ref random, allMaintenanceTypes, anyMaintenanceTypes, maxParkingSizes);
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

	private Entity GetRandomVehicle(ref Random random, MaintenanceType allMaintenanceTypes, MaintenanceType anyMaintenanceTypes, float4 maxParkingSizes)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		Entity result = Entity.Null;
		int totalProbability = 0;
		int num = 100;
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<MaintenanceVehicleData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<MaintenanceVehicleData>(ref m_MaintenanceVehicleType);
			NativeArray<ObjectGeometryData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ObjectGeometryData>(ref m_ObjectGeometryDataType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray2.Length; j++)
			{
				MaintenanceVehicleData maintenanceVehicleData = nativeArray2[j];
				if ((maintenanceVehicleData.m_MaintenanceType & allMaintenanceTypes) != allMaintenanceTypes || ((maintenanceVehicleData.m_MaintenanceType & anyMaintenanceTypes) == 0 && anyMaintenanceTypes != MaintenanceType.None) || !m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				float offset;
				float2 parkingSize = VehicleUtils.GetParkingSize(nativeArray3[j], out offset);
				bool4 val = ((float2)(ref parkingSize)).xyxy > maxParkingSizes;
				if (math.all(val | ((bool4)(ref val)).yxwz))
				{
					continue;
				}
				int num2 = math.select(math.countbits((int)(maintenanceVehicleData.m_MaintenanceType ^ allMaintenanceTypes)), 0, allMaintenanceTypes == MaintenanceType.None);
				if (num2 <= num)
				{
					if (num2 < num)
					{
						totalProbability = 0;
						num = num2;
					}
					if (PickVehicle(ref random, 100, ref totalProbability))
					{
						result = nativeArray[j];
					}
				}
			}
		}
		return result;
	}

	private bool PickVehicle(ref Random random, int probability, ref int totalProbability)
	{
		totalProbability += probability;
		return ((Random)(ref random)).NextInt(totalProbability) < probability;
	}
}
