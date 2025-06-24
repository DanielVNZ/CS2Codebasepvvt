using Game.City;
using Game.Common;
using Game.Objects;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct PersonalCarSelectData
{
	private struct CarData
	{
		public Entity m_Entity;

		public PersonalCarData m_PersonalCarData;

		public CarTrailerData m_TrailerData;

		public CarTractorData m_TractorData;

		public ObjectData m_ObjectData;

		public MovingObjectData m_MovingObjectData;
	}

	private NativeList<ArchetypeChunk> m_PrefabChunks;

	private VehicleSelectRequirementData m_RequirementData;

	private EntityTypeHandle m_EntityType;

	private ComponentTypeHandle<Game.Prefabs.CarData> m_CarDataType;

	private ComponentTypeHandle<PersonalCarData> m_PersonalCarDataType;

	private ComponentTypeHandle<CarTrailerData> m_CarTrailerDataType;

	private ComponentTypeHandle<CarTractorData> m_CarTractorDataType;

	private ComponentTypeHandle<ObjectData> m_ObjectDataType;

	private ComponentTypeHandle<MovingObjectData> m_MovingObjectDataType;

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
			ComponentType.ReadOnly<PersonalCarData>(),
			ComponentType.ReadOnly<Game.Prefabs.CarData>(),
			ComponentType.ReadOnly<MovingObjectData>(),
			ComponentType.ReadOnly<PrefabData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Locked>() };
		return val;
	}

	public PersonalCarSelectData(SystemBase system)
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
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabChunks = default(NativeList<ArchetypeChunk>);
		m_RequirementData = new VehicleSelectRequirementData(system);
		m_EntityType = ((ComponentSystemBase)system).GetEntityTypeHandle();
		m_CarDataType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Prefabs.CarData>(true);
		m_PersonalCarDataType = ((ComponentSystemBase)system).GetComponentTypeHandle<PersonalCarData>(true);
		m_CarTrailerDataType = ((ComponentSystemBase)system).GetComponentTypeHandle<CarTrailerData>(true);
		m_CarTractorDataType = ((ComponentSystemBase)system).GetComponentTypeHandle<CarTractorData>(true);
		m_ObjectDataType = ((ComponentSystemBase)system).GetComponentTypeHandle<ObjectData>(true);
		m_MovingObjectDataType = ((ComponentSystemBase)system).GetComponentTypeHandle<MovingObjectData>(true);
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
		m_CarDataType.Update(system);
		m_PersonalCarDataType.Update(system);
		m_CarTrailerDataType.Update(system);
		m_CarTractorDataType.Update(system);
		m_ObjectDataType.Update(system);
		m_MovingObjectDataType.Update(system);
	}

	public void PostUpdate(JobHandle jobHandle)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabChunks.Dispose(jobHandle);
	}

	public Entity CreateVehicle(ParallelWriter commandBuffer, int jobIndex, ref Random random, int passengerAmount, int baggageAmount, bool avoidTrailers, bool noSlowVehicles, Transform transform, Entity source, Entity keeper, PersonalCarFlags state, bool stopped, uint delay = 0u)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Entity trailer;
		Entity vehiclePrefab;
		Entity trailerPrefab;
		return CreateVehicle(commandBuffer, jobIndex, ref random, passengerAmount, baggageAmount, avoidTrailers, noSlowVehicles, transform, source, keeper, state, stopped, delay, out trailer, out vehiclePrefab, out trailerPrefab);
	}

	public Entity CreateVehicle(ParallelWriter commandBuffer, int jobIndex, ref Random random, int passengerAmount, int baggageAmount, bool avoidTrailers, bool noSlowVehicles, Transform transform, Entity source, Entity keeper, PersonalCarFlags state, bool stopped, uint delay, out Entity trailer, out Entity vehiclePrefab, out Entity trailerPrefab)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		trailer = Entity.Null;
		vehiclePrefab = Entity.Null;
		trailerPrefab = Entity.Null;
		if (GetVehicleData(ref random, passengerAmount, baggageAmount, avoidTrailers, noSlowVehicles, out var bestFirst, out var bestSecond))
		{
			Entity val = CreateVehicle(commandBuffer, jobIndex, ref random, bestFirst, transform, source, keeper, state, stopped, delay);
			vehiclePrefab = bestFirst.m_Entity;
			if (bestSecond.m_Entity != Entity.Null)
			{
				DynamicBuffer<LayoutElement> val2 = ((ParallelWriter)(ref commandBuffer)).AddBuffer<LayoutElement>(jobIndex, val);
				val2.Add(new LayoutElement(val));
				trailer = CreateVehicle(commandBuffer, jobIndex, ref random, bestSecond, transform, source, Entity.Null, (PersonalCarFlags)0u, stopped, delay);
				trailerPrefab = bestSecond.m_Entity;
				((ParallelWriter)(ref commandBuffer)).SetComponent<Controller>(jobIndex, trailer, new Controller(val));
				val2.Add(new LayoutElement(trailer));
			}
			return val;
		}
		return Entity.Null;
	}

	public Entity CreateVehicle(EntityCommandBuffer commandBuffer, ref Random random, int passengerAmount, int baggageAmount, bool avoidTrailers, bool noSlowVehicles, Transform transform, Entity source, Entity keeper, PersonalCarFlags state, bool stopped, uint delay = 0u)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (GetVehicleData(ref random, passengerAmount, baggageAmount, avoidTrailers, noSlowVehicles, out var bestFirst, out var bestSecond))
		{
			Entity val = CreateVehicle(commandBuffer, ref random, bestFirst, transform, source, keeper, state, stopped, delay);
			if (bestSecond.m_Entity != Entity.Null)
			{
				DynamicBuffer<LayoutElement> val2 = ((EntityCommandBuffer)(ref commandBuffer)).AddBuffer<LayoutElement>(val);
				val2.Add(new LayoutElement(val));
				Entity val3 = CreateVehicle(commandBuffer, ref random, bestSecond, transform, source, Entity.Null, (PersonalCarFlags)0u, stopped, delay);
				((EntityCommandBuffer)(ref commandBuffer)).SetComponent<Controller>(val3, new Controller(val));
				val2.Add(new LayoutElement(val3));
			}
			return val;
		}
		return Entity.Null;
	}

	public Entity CreateTrailer(ParallelWriter commandBuffer, int jobIndex, ref Random random, int passengerAmount, int baggageAmount, bool noSlowVehicles, Entity tractorPrefab, Transform tractorTransform, PersonalCarFlags state, bool stopped, uint delay = 0u)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PersonalCarData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PersonalCarData>(ref m_PersonalCarDataType);
			NativeArray<CarTractorData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTractorData>(ref m_CarTractorDataType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray3.Length; j++)
			{
				if (!(nativeArray[j] != tractorPrefab) && m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					CarData firstData = new CarData
					{
						m_Entity = tractorPrefab,
						m_PersonalCarData = nativeArray2[j],
						m_TractorData = nativeArray3[j]
					};
					CarData bestFirst = default(CarData);
					CarData bestSecond = default(CarData);
					CalculateProbability(passengerAmount, baggageAmount, firstData, default(CarData), out var probability, out var offset);
					CheckTrailers(passengerAmount, baggageAmount, 0, firstData, emptyOnly: false, noSlowVehicles, ref random, ref bestFirst, ref bestSecond, ref probability, ref offset);
					if (bestSecond.m_Entity == Entity.Null)
					{
						return Entity.Null;
					}
					Transform transform = tractorTransform;
					ref float3 position = ref transform.m_Position;
					position += math.rotate(tractorTransform.m_Rotation, firstData.m_TractorData.m_AttachPosition);
					ref float3 position2 = ref transform.m_Position;
					position2 -= math.rotate(transform.m_Rotation, bestSecond.m_TrailerData.m_AttachPosition);
					return CreateVehicle(commandBuffer, jobIndex, ref random, bestSecond, transform, Entity.Null, Entity.Null, (PersonalCarFlags)0u, stopped, delay);
				}
			}
		}
		return Entity.Null;
	}

	private bool GetVehicleData(ref Random random, int passengerAmount, int baggageAmount, bool avoidTrailers, bool noSlowVehicles, out CarData bestFirst, out CarData bestSecond)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		bestFirst = default(CarData);
		bestSecond = default(CarData);
		int totalProbability = 0;
		int bestOffset = -11 - (passengerAmount + baggageAmount);
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Game.Prefabs.CarData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Prefabs.CarData>(ref m_CarDataType);
			NativeArray<PersonalCarData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PersonalCarData>(ref m_PersonalCarDataType);
			NativeArray<CarTrailerData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTrailerData>(ref m_CarTrailerDataType);
			NativeArray<CarTractorData> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTractorData>(ref m_CarTractorDataType);
			NativeArray<ObjectData> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ObjectData>(ref m_ObjectDataType);
			NativeArray<MovingObjectData> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<MovingObjectData>(ref m_MovingObjectDataType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray3.Length; j++)
			{
				Game.Prefabs.CarData carData = nativeArray2[j];
				if ((noSlowVehicles && carData.m_MaxSpeed < 22.222223f) || !m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				CarData carData2 = new CarData
				{
					m_PersonalCarData = nativeArray3[j]
				};
				if (carData2.m_PersonalCarData.m_PassengerCapacity == 0 && carData2.m_PersonalCarData.m_BaggageCapacity == 0)
				{
					continue;
				}
				carData2.m_Entity = nativeArray[j];
				carData2.m_ObjectData = nativeArray6[j];
				carData2.m_MovingObjectData = nativeArray7[j];
				bool flag = false;
				if (nativeArray4.Length != 0)
				{
					carData2.m_TrailerData = nativeArray4[j];
					flag = true;
				}
				if (nativeArray5.Length != 0)
				{
					carData2.m_TractorData = nativeArray5[j];
					if (carData2.m_TractorData.m_FixedTrailer != Entity.Null)
					{
						if (!flag)
						{
							int extraOffset = math.select(0, -1, avoidTrailers);
							CheckTrailers(passengerAmount, baggageAmount, extraOffset, carData2, emptyOnly: true, noSlowVehicles, ref random, ref bestFirst, ref bestSecond, ref totalProbability, ref bestOffset);
						}
						continue;
					}
				}
				if (flag)
				{
					int extraOffset2 = math.select(0, -1, avoidTrailers);
					CheckTractors(passengerAmount, baggageAmount, extraOffset2, carData2, noSlowVehicles, ref random, ref bestFirst, ref bestSecond, ref totalProbability, ref bestOffset);
					continue;
				}
				CalculateProbability(passengerAmount, baggageAmount, carData2, default(CarData), out var probability, out var offset);
				if (PickVehicle(ref random, probability, offset, ref totalProbability, ref bestOffset))
				{
					bestFirst = carData2;
					bestSecond = default(CarData);
				}
			}
		}
		return bestFirst.m_Entity != Entity.Null;
	}

	private Entity CreateVehicle(ParallelWriter commandBuffer, int jobIndex, ref Random random, CarData data, Transform transform, Entity source, Entity keeper, PersonalCarFlags state, bool stopped, uint delay)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		Entity val = ((!stopped) ? ((ParallelWriter)(ref commandBuffer)).CreateEntity(jobIndex, data.m_ObjectData.m_Archetype) : ((ParallelWriter)(ref commandBuffer)).CreateEntity(jobIndex, data.m_MovingObjectData.m_StoppedArchetype));
		((ParallelWriter)(ref commandBuffer)).SetComponent<Transform>(jobIndex, val, transform);
		((ParallelWriter)(ref commandBuffer)).SetComponent<Game.Vehicles.PersonalCar>(jobIndex, val, new Game.Vehicles.PersonalCar(keeper, state));
		((ParallelWriter)(ref commandBuffer)).SetComponent<PrefabRef>(jobIndex, val, new PrefabRef(data.m_Entity));
		((ParallelWriter)(ref commandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val, new PseudoRandomSeed(ref random));
		if (source != Entity.Null)
		{
			((ParallelWriter)(ref commandBuffer)).AddComponent<TripSource>(jobIndex, val, new TripSource(source, delay));
			((ParallelWriter)(ref commandBuffer)).AddComponent<Unspawned>(jobIndex, val, default(Unspawned));
		}
		return val;
	}

	private Entity CreateVehicle(EntityCommandBuffer commandBuffer, ref Random random, CarData data, Transform transform, Entity source, Entity keeper, PersonalCarFlags state, bool stopped, uint delay)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		Entity val = ((!stopped) ? ((EntityCommandBuffer)(ref commandBuffer)).CreateEntity(data.m_ObjectData.m_Archetype) : ((EntityCommandBuffer)(ref commandBuffer)).CreateEntity(data.m_MovingObjectData.m_StoppedArchetype));
		((EntityCommandBuffer)(ref commandBuffer)).SetComponent<Transform>(val, transform);
		((EntityCommandBuffer)(ref commandBuffer)).SetComponent<Game.Vehicles.PersonalCar>(val, new Game.Vehicles.PersonalCar(keeper, state));
		((EntityCommandBuffer)(ref commandBuffer)).SetComponent<PrefabRef>(val, new PrefabRef(data.m_Entity));
		((EntityCommandBuffer)(ref commandBuffer)).SetComponent<PseudoRandomSeed>(val, new PseudoRandomSeed(ref random));
		if (source != Entity.Null)
		{
			((EntityCommandBuffer)(ref commandBuffer)).AddComponent<TripSource>(val, new TripSource(source, delay));
			((EntityCommandBuffer)(ref commandBuffer)).AddComponent<Unspawned>(val, default(Unspawned));
		}
		return val;
	}

	private void CheckTrailers(int passengerAmount, int baggageAmount, int extraOffset, CarData firstData, bool emptyOnly, bool noSlowVehicles, ref Random random, ref CarData bestFirst, ref CarData bestSecond, ref int totalProbability, ref int bestOffset)
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
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<CarTrailerData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTrailerData>(ref m_CarTrailerDataType);
			if (nativeArray.Length == 0)
			{
				continue;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Game.Prefabs.CarData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Prefabs.CarData>(ref m_CarDataType);
			NativeArray<PersonalCarData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PersonalCarData>(ref m_PersonalCarDataType);
			NativeArray<CarTractorData> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTractorData>(ref m_CarTractorDataType);
			NativeArray<ObjectData> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ObjectData>(ref m_ObjectDataType);
			NativeArray<MovingObjectData> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<MovingObjectData>(ref m_MovingObjectDataType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				Game.Prefabs.CarData carData = nativeArray3[j];
				if ((noSlowVehicles && carData.m_MaxSpeed < 22.222223f) || !m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				CarData carData2 = new CarData
				{
					m_PersonalCarData = nativeArray4[j]
				};
				if (emptyOnly && (carData2.m_PersonalCarData.m_PassengerCapacity != 0 || carData2.m_PersonalCarData.m_BaggageCapacity != 0))
				{
					continue;
				}
				carData2.m_Entity = nativeArray2[j];
				carData2.m_TrailerData = nativeArray[j];
				if (firstData.m_TractorData.m_TrailerType != carData2.m_TrailerData.m_TrailerType || (firstData.m_TractorData.m_FixedTrailer != Entity.Null && firstData.m_TractorData.m_FixedTrailer != carData2.m_Entity) || (carData2.m_TrailerData.m_FixedTractor != Entity.Null && carData2.m_TrailerData.m_FixedTractor != firstData.m_Entity))
				{
					continue;
				}
				carData2.m_ObjectData = nativeArray6[j];
				carData2.m_MovingObjectData = nativeArray7[j];
				if (nativeArray5.Length != 0)
				{
					carData2.m_TractorData = nativeArray5[j];
					if (carData2.m_TractorData.m_FixedTrailer != Entity.Null)
					{
						continue;
					}
				}
				CalculateProbability(passengerAmount, baggageAmount, firstData, carData2, out var probability, out var offset);
				if (PickVehicle(ref random, probability, offset + extraOffset, ref totalProbability, ref bestOffset))
				{
					bestFirst = firstData;
					bestSecond = carData2;
				}
			}
		}
	}

	private void CheckTractors(int passengerAmount, int baggageAmount, int extraOffset, CarData secondData, bool noSlowVehicles, ref Random random, ref CarData bestFirst, ref CarData bestSecond, ref int totalProbability, ref int bestOffset)
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
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<CarTractorData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTractorData>(ref m_CarTractorDataType);
			if (nativeArray.Length == 0 || ((ArchetypeChunk)(ref chunk)).Has<CarTrailerData>(ref m_CarTrailerDataType))
			{
				continue;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Game.Prefabs.CarData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Prefabs.CarData>(ref m_CarDataType);
			NativeArray<PersonalCarData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PersonalCarData>(ref m_PersonalCarDataType);
			NativeArray<ObjectData> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ObjectData>(ref m_ObjectDataType);
			NativeArray<MovingObjectData> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<MovingObjectData>(ref m_MovingObjectDataType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				Game.Prefabs.CarData carData = nativeArray3[j];
				if ((noSlowVehicles && carData.m_MaxSpeed < 22.222223f) || !m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				CarData carData2 = new CarData
				{
					m_PersonalCarData = nativeArray4[j],
					m_Entity = nativeArray2[j],
					m_TractorData = nativeArray[j]
				};
				if (carData2.m_TractorData.m_TrailerType == secondData.m_TrailerData.m_TrailerType && (!(carData2.m_TractorData.m_FixedTrailer != Entity.Null) || !(carData2.m_TractorData.m_FixedTrailer != secondData.m_Entity)) && (!(secondData.m_TrailerData.m_FixedTractor != Entity.Null) || !(secondData.m_TrailerData.m_FixedTractor != carData2.m_Entity)))
				{
					carData2.m_ObjectData = nativeArray5[j];
					carData2.m_MovingObjectData = nativeArray6[j];
					CalculateProbability(passengerAmount, baggageAmount, carData2, secondData, out var probability, out var offset);
					if (PickVehicle(ref random, probability, offset + extraOffset, ref totalProbability, ref bestOffset))
					{
						bestFirst = carData2;
						bestSecond = secondData;
					}
				}
			}
		}
	}

	private void CalculateProbability(int passengerAmount, int baggageAmount, CarData firstData, CarData secondData, out int probability, out int offset)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		int num = firstData.m_PersonalCarData.m_PassengerCapacity + secondData.m_PersonalCarData.m_PassengerCapacity;
		int num2 = firstData.m_PersonalCarData.m_BaggageCapacity + secondData.m_PersonalCarData.m_BaggageCapacity;
		int num3 = num - passengerAmount;
		int num4 = num2 - baggageAmount;
		offset = math.min(0, num3) + math.min(0, num4);
		offset = math.select(0, offset - 10, offset != 0) + math.min(0, 4 - num3) + math.min(0, 4 - num4);
		probability = firstData.m_PersonalCarData.m_Probability;
		probability = math.select(probability, probability * secondData.m_PersonalCarData.m_Probability / 50, secondData.m_Entity != Entity.Null);
		probability = math.max(1, probability / ((1 << math.max(0, num3)) + (1 << math.max(0, num4))));
	}

	private bool PickVehicle(ref Random random, int probability, int offset, ref int totalProbability, ref int bestOffset)
	{
		if (offset == bestOffset)
		{
			totalProbability += probability;
			return ((Random)(ref random)).NextInt(totalProbability) < probability;
		}
		if (offset > bestOffset)
		{
			totalProbability = probability;
			bestOffset = offset;
			return true;
		}
		return false;
	}
}
