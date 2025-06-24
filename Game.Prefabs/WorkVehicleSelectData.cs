using Colossal.Collections;
using Game.Areas;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct WorkVehicleSelectData
{
	private struct VehicleData
	{
		public Entity m_Entity;

		public WorkVehicleData m_WorkVehicleData;

		public CarTrailerData m_TrailerData;

		public CarTractorData m_TractorData;

		public ObjectData m_ObjectData;
	}

	private NativeList<ArchetypeChunk> m_PrefabChunks;

	private VehicleSelectRequirementData m_RequirementData;

	private EntityTypeHandle m_EntityType;

	private ComponentTypeHandle<WorkVehicleData> m_WorkVehicleDataType;

	private ComponentTypeHandle<CarTrailerData> m_CarTrailerDataType;

	private ComponentTypeHandle<CarTractorData> m_CarTractorDataType;

	private ComponentTypeHandle<CarData> m_CarDataType;

	private ComponentTypeHandle<WatercraftData> m_WatercraftDataType;

	private ComponentTypeHandle<ObjectData> m_ObjectDataType;

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
			ComponentType.ReadOnly<WorkVehicleData>(),
			ComponentType.ReadOnly<ObjectData>(),
			ComponentType.ReadOnly<PrefabData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CarData>(),
			ComponentType.ReadOnly<WatercraftData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Locked>() };
		return val;
	}

	public WorkVehicleSelectData(SystemBase system)
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
		m_WorkVehicleDataType = ((ComponentSystemBase)system).GetComponentTypeHandle<WorkVehicleData>(true);
		m_CarTrailerDataType = ((ComponentSystemBase)system).GetComponentTypeHandle<CarTrailerData>(true);
		m_CarTractorDataType = ((ComponentSystemBase)system).GetComponentTypeHandle<CarTractorData>(true);
		m_CarDataType = ((ComponentSystemBase)system).GetComponentTypeHandle<CarData>(true);
		m_WatercraftDataType = ((ComponentSystemBase)system).GetComponentTypeHandle<WatercraftData>(true);
		m_ObjectDataType = ((ComponentSystemBase)system).GetComponentTypeHandle<ObjectData>(true);
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
		m_WorkVehicleDataType.Update(system);
		m_CarTrailerDataType.Update(system);
		m_CarTractorDataType.Update(system);
		m_CarDataType.Update(system);
		m_WatercraftDataType.Update(system);
		m_ObjectDataType.Update(system);
	}

	public void PostUpdate(JobHandle jobHandle)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabChunks.Dispose(jobHandle);
	}

	public Entity CreateVehicle(ParallelWriter commandBuffer, int jobIndex, ref Random random, RoadTypes roadTypes, SizeClass sizeClass, VehicleWorkType workType, MapFeature mapFeature, Resource resource, ref float workAmount, Transform transform, Entity source, WorkVehicleFlags state)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		VehicleData bestFirst = default(VehicleData);
		VehicleData bestSecond = default(VehicleData);
		VehicleData bestThird = default(VehicleData);
		VehicleData bestForth = default(VehicleData);
		int totalProbability = 0;
		CarData carData = default(CarData);
		WatercraftData watercraftData = default(WatercraftData);
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<WorkVehicleData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WorkVehicleData>(ref m_WorkVehicleDataType);
			NativeArray<CarTrailerData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTrailerData>(ref m_CarTrailerDataType);
			NativeArray<CarTractorData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTractorData>(ref m_CarTractorDataType);
			NativeArray<CarData> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarData>(ref m_CarDataType);
			NativeArray<WatercraftData> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WatercraftData>(ref m_WatercraftDataType);
			NativeArray<ObjectData> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ObjectData>(ref m_ObjectDataType);
			if ((nativeArray5.Length != 0 && (roadTypes & RoadTypes.Car) == 0) || (nativeArray6.Length != 0 && (roadTypes & RoadTypes.Watercraft) == 0))
			{
				continue;
			}
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray2.Length; j++)
			{
				VehicleData vehicleData = new VehicleData
				{
					m_WorkVehicleData = nativeArray2[j]
				};
				if (vehicleData.m_WorkVehicleData.m_WorkType == VehicleWorkType.None || vehicleData.m_WorkVehicleData.m_WorkType != workType || ((vehicleData.m_WorkVehicleData.m_MapFeature != MapFeature.None || vehicleData.m_WorkVehicleData.m_Resources != Resource.NoResource) && vehicleData.m_WorkVehicleData.m_MapFeature != mapFeature && (vehicleData.m_WorkVehicleData.m_Resources & resource) == Resource.NoResource) || vehicleData.m_WorkVehicleData.m_MaxWorkAmount == 0f)
				{
					continue;
				}
				if (sizeClass != SizeClass.Undefined)
				{
					if (CollectionUtils.TryGet<CarData>(nativeArray5, j, ref carData))
					{
						if (carData.m_SizeClass != SizeClass.Undefined && carData.m_SizeClass != sizeClass)
						{
							continue;
						}
					}
					else if (CollectionUtils.TryGet<WatercraftData>(nativeArray6, j, ref watercraftData) && watercraftData.m_SizeClass != SizeClass.Undefined && watercraftData.m_SizeClass != sizeClass)
					{
						continue;
					}
				}
				if (!m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				vehicleData.m_Entity = nativeArray[j];
				vehicleData.m_ObjectData = nativeArray7[j];
				bool flag = false;
				if (nativeArray3.Length != 0)
				{
					vehicleData.m_TrailerData = nativeArray3[j];
					flag = true;
				}
				if (nativeArray4.Length != 0)
				{
					vehicleData.m_TractorData = nativeArray4[j];
					if (vehicleData.m_TractorData.m_FixedTrailer != Entity.Null)
					{
						CheckTrailers(workType, mapFeature, resource, flag, vehicleData, ref random, ref bestFirst, ref bestSecond, ref bestThird, ref bestForth, ref totalProbability);
						continue;
					}
				}
				if (flag)
				{
					CheckTractors(workType, mapFeature, resource, vehicleData, ref random, ref bestFirst, ref bestSecond, ref bestThird, ref bestForth, ref totalProbability);
				}
				else if (PickVehicle(ref random, 100, ref totalProbability))
				{
					bestFirst = vehicleData;
					bestSecond = default(VehicleData);
					bestThird = default(VehicleData);
					bestForth = default(VehicleData);
				}
			}
		}
		if (bestFirst.m_Entity == Entity.Null)
		{
			workAmount = 0f;
			return Entity.Null;
		}
		float workAmount2 = workAmount;
		Entity val = CreateVehicle(commandBuffer, jobIndex, ref random, bestFirst, workType, ref workAmount2, transform, source, state);
		if (bestSecond.m_Entity != Entity.Null)
		{
			DynamicBuffer<LayoutElement> val2 = ((ParallelWriter)(ref commandBuffer)).AddBuffer<LayoutElement>(jobIndex, val);
			val2.Add(new LayoutElement(val));
			Entity val3 = CreateVehicle(commandBuffer, jobIndex, ref random, bestSecond, workType, ref workAmount2, transform, source, state & WorkVehicleFlags.ExtractorVehicle);
			((ParallelWriter)(ref commandBuffer)).SetComponent<Controller>(jobIndex, val3, new Controller(val));
			val2.Add(new LayoutElement(val3));
			if (bestThird.m_Entity != Entity.Null)
			{
				val3 = CreateVehicle(commandBuffer, jobIndex, ref random, bestThird, workType, ref workAmount2, transform, source, state & WorkVehicleFlags.ExtractorVehicle);
				((ParallelWriter)(ref commandBuffer)).SetComponent<Controller>(jobIndex, val3, new Controller(val));
				val2.Add(new LayoutElement(val3));
			}
			if (bestForth.m_Entity != Entity.Null)
			{
				val3 = CreateVehicle(commandBuffer, jobIndex, ref random, bestForth, workType, ref workAmount2, transform, source, state & WorkVehicleFlags.ExtractorVehicle);
				((ParallelWriter)(ref commandBuffer)).SetComponent<Controller>(jobIndex, val3, new Controller(val));
				val2.Add(new LayoutElement(val3));
			}
		}
		workAmount -= workAmount2;
		return val;
	}

	private Entity CreateVehicle(ParallelWriter commandBuffer, int jobIndex, ref Random random, VehicleData data, VehicleWorkType workType, ref float workAmount, Transform transform, Entity source, WorkVehicleFlags state)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		Game.Vehicles.WorkVehicle workVehicle = new Game.Vehicles.WorkVehicle
		{
			m_State = state
		};
		if (workType == data.m_WorkVehicleData.m_WorkType && workAmount > 0f)
		{
			workVehicle.m_WorkAmount = math.min(workAmount, data.m_WorkVehicleData.m_MaxWorkAmount);
			workAmount -= workVehicle.m_WorkAmount;
		}
		Entity val = ((ParallelWriter)(ref commandBuffer)).CreateEntity(jobIndex, data.m_ObjectData.m_Archetype);
		((ParallelWriter)(ref commandBuffer)).SetComponent<Transform>(jobIndex, val, transform);
		((ParallelWriter)(ref commandBuffer)).SetComponent<Game.Vehicles.WorkVehicle>(jobIndex, val, workVehicle);
		((ParallelWriter)(ref commandBuffer)).SetComponent<PrefabRef>(jobIndex, val, new PrefabRef(data.m_Entity));
		((ParallelWriter)(ref commandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val, new PseudoRandomSeed(ref random));
		((ParallelWriter)(ref commandBuffer)).AddComponent<TripSource>(jobIndex, val, new TripSource(source));
		((ParallelWriter)(ref commandBuffer)).AddComponent<Unspawned>(jobIndex, val, default(Unspawned));
		return val;
	}

	private void CheckTrailers(VehicleWorkType workType, MapFeature mapFeature, Resource resource, bool firstIsTrailer, VehicleData firstData, ref Random random, ref VehicleData bestFirst, ref VehicleData bestSecond, ref VehicleData bestThird, ref VehicleData bestForth, ref int totalProbability)
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
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<CarTrailerData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTrailerData>(ref m_CarTrailerDataType);
			if (nativeArray.Length == 0)
			{
				continue;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<WorkVehicleData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WorkVehicleData>(ref m_WorkVehicleDataType);
			NativeArray<CarTractorData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTractorData>(ref m_CarTractorDataType);
			NativeArray<ObjectData> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ObjectData>(ref m_ObjectDataType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				VehicleData vehicleData = new VehicleData
				{
					m_WorkVehicleData = nativeArray3[j]
				};
				if ((vehicleData.m_WorkVehicleData.m_WorkType != VehicleWorkType.None && vehicleData.m_WorkVehicleData.m_WorkType != workType) || ((vehicleData.m_WorkVehicleData.m_MapFeature != MapFeature.None || vehicleData.m_WorkVehicleData.m_Resources != Resource.NoResource) && vehicleData.m_WorkVehicleData.m_MapFeature != mapFeature && (vehicleData.m_WorkVehicleData.m_Resources & resource) == Resource.NoResource) || vehicleData.m_WorkVehicleData.m_MaxWorkAmount != 0f || !m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				vehicleData.m_Entity = nativeArray2[j];
				vehicleData.m_TrailerData = nativeArray[j];
				if (firstData.m_TractorData.m_TrailerType != vehicleData.m_TrailerData.m_TrailerType || (firstData.m_TractorData.m_FixedTrailer != Entity.Null && firstData.m_TractorData.m_FixedTrailer != vehicleData.m_Entity) || (vehicleData.m_TrailerData.m_FixedTractor != Entity.Null && vehicleData.m_TrailerData.m_FixedTractor != firstData.m_Entity))
				{
					continue;
				}
				vehicleData.m_ObjectData = nativeArray5[j];
				if (nativeArray4.Length != 0)
				{
					vehicleData.m_TractorData = nativeArray4[j];
					if (vehicleData.m_TractorData.m_FixedTrailer != Entity.Null)
					{
						CheckTrailers(workType, mapFeature, resource, firstIsTrailer, firstData, vehicleData, ref random, ref bestFirst, ref bestSecond, ref bestThird, ref bestForth, ref totalProbability);
						continue;
					}
				}
				if (firstIsTrailer)
				{
					CheckTractors(workType, mapFeature, resource, firstData, vehicleData, ref random, ref bestFirst, ref bestSecond, ref bestThird, ref bestForth, ref totalProbability);
				}
				else if (PickVehicle(ref random, 100, ref totalProbability))
				{
					bestFirst = firstData;
					bestSecond = vehicleData;
					bestThird = default(VehicleData);
					bestForth = default(VehicleData);
				}
			}
		}
	}

	private void CheckTrailers(VehicleWorkType workType, MapFeature mapFeature, Resource resource, bool firstIsTrailer, VehicleData firstData, VehicleData secondData, ref Random random, ref VehicleData bestFirst, ref VehicleData bestSecond, ref VehicleData bestThird, ref VehicleData bestForth, ref int totalProbability)
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
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<CarTrailerData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTrailerData>(ref m_CarTrailerDataType);
			if (nativeArray.Length == 0)
			{
				continue;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<WorkVehicleData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WorkVehicleData>(ref m_WorkVehicleDataType);
			NativeArray<CarTractorData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTractorData>(ref m_CarTractorDataType);
			NativeArray<ObjectData> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ObjectData>(ref m_ObjectDataType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				VehicleData vehicleData = new VehicleData
				{
					m_WorkVehicleData = nativeArray3[j]
				};
				if ((vehicleData.m_WorkVehicleData.m_WorkType != VehicleWorkType.None && vehicleData.m_WorkVehicleData.m_WorkType != workType) || ((vehicleData.m_WorkVehicleData.m_MapFeature != MapFeature.None || vehicleData.m_WorkVehicleData.m_Resources != Resource.NoResource) && vehicleData.m_WorkVehicleData.m_MapFeature != mapFeature && (vehicleData.m_WorkVehicleData.m_Resources & resource) == Resource.NoResource) || vehicleData.m_WorkVehicleData.m_MaxWorkAmount != 0f || !m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				vehicleData.m_Entity = nativeArray2[j];
				vehicleData.m_TrailerData = nativeArray[j];
				if (secondData.m_TractorData.m_TrailerType != vehicleData.m_TrailerData.m_TrailerType || (secondData.m_TractorData.m_FixedTrailer != Entity.Null && secondData.m_TractorData.m_FixedTrailer != vehicleData.m_Entity) || (vehicleData.m_TrailerData.m_FixedTractor != Entity.Null && vehicleData.m_TrailerData.m_FixedTractor != secondData.m_Entity))
				{
					continue;
				}
				vehicleData.m_ObjectData = nativeArray5[j];
				if (nativeArray4.Length != 0)
				{
					vehicleData.m_TractorData = nativeArray4[j];
					if (vehicleData.m_TractorData.m_FixedTrailer != Entity.Null)
					{
						if (!firstIsTrailer)
						{
							CheckTrailers(workType, mapFeature, resource, firstData, secondData, vehicleData, ref random, ref bestFirst, ref bestSecond, ref bestThird, ref bestForth, ref totalProbability);
						}
						continue;
					}
				}
				if (firstIsTrailer)
				{
					CheckTractors(workType, mapFeature, resource, firstData, secondData, vehicleData, ref random, ref bestFirst, ref bestSecond, ref bestThird, ref bestForth, ref totalProbability);
				}
				else if (PickVehicle(ref random, 100, ref totalProbability))
				{
					bestFirst = firstData;
					bestSecond = secondData;
					bestThird = vehicleData;
					bestForth = default(VehicleData);
				}
			}
		}
	}

	private void CheckTrailers(VehicleWorkType workType, MapFeature mapFeature, Resource resource, VehicleData firstData, VehicleData secondData, VehicleData thirdData, ref Random random, ref VehicleData bestFirst, ref VehicleData bestSecond, ref VehicleData bestThird, ref VehicleData bestForth, ref int totalProbability)
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
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<CarTrailerData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTrailerData>(ref m_CarTrailerDataType);
			if (nativeArray.Length == 0)
			{
				continue;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<WorkVehicleData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WorkVehicleData>(ref m_WorkVehicleDataType);
			NativeArray<CarTractorData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTractorData>(ref m_CarTractorDataType);
			NativeArray<ObjectData> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ObjectData>(ref m_ObjectDataType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				VehicleData vehicleData = new VehicleData
				{
					m_WorkVehicleData = nativeArray3[j]
				};
				if ((vehicleData.m_WorkVehicleData.m_WorkType != VehicleWorkType.None && vehicleData.m_WorkVehicleData.m_WorkType != workType) || ((vehicleData.m_WorkVehicleData.m_MapFeature != MapFeature.None || vehicleData.m_WorkVehicleData.m_Resources != Resource.NoResource) && vehicleData.m_WorkVehicleData.m_MapFeature != mapFeature && (vehicleData.m_WorkVehicleData.m_Resources & resource) == Resource.NoResource) || vehicleData.m_WorkVehicleData.m_MaxWorkAmount != 0f || !m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				vehicleData.m_Entity = nativeArray2[j];
				vehicleData.m_TrailerData = nativeArray[j];
				if (thirdData.m_TractorData.m_TrailerType != vehicleData.m_TrailerData.m_TrailerType || (thirdData.m_TractorData.m_FixedTrailer != Entity.Null && thirdData.m_TractorData.m_FixedTrailer != vehicleData.m_Entity) || (vehicleData.m_TrailerData.m_FixedTractor != Entity.Null && vehicleData.m_TrailerData.m_FixedTractor != thirdData.m_Entity))
				{
					continue;
				}
				vehicleData.m_ObjectData = nativeArray5[j];
				if (nativeArray4.Length != 0)
				{
					vehicleData.m_TractorData = nativeArray4[j];
					if (vehicleData.m_TractorData.m_FixedTrailer != Entity.Null)
					{
						continue;
					}
				}
				if (PickVehicle(ref random, 100, ref totalProbability))
				{
					bestFirst = firstData;
					bestSecond = secondData;
					bestThird = thirdData;
					bestForth = vehicleData;
				}
			}
		}
	}

	private void CheckTractors(VehicleWorkType workType, MapFeature mapFeature, Resource resource, VehicleData secondData, ref Random random, ref VehicleData bestFirst, ref VehicleData bestSecond, ref VehicleData bestThird, ref VehicleData bestForth, ref int totalProbability)
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
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<CarTractorData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTractorData>(ref m_CarTractorDataType);
			if (nativeArray.Length == 0)
			{
				continue;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<WorkVehicleData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WorkVehicleData>(ref m_WorkVehicleDataType);
			NativeArray<CarTrailerData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTrailerData>(ref m_CarTrailerDataType);
			NativeArray<ObjectData> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ObjectData>(ref m_ObjectDataType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				VehicleData vehicleData = new VehicleData
				{
					m_WorkVehicleData = nativeArray3[j]
				};
				if ((vehicleData.m_WorkVehicleData.m_WorkType != VehicleWorkType.None && vehicleData.m_WorkVehicleData.m_WorkType != workType) || ((vehicleData.m_WorkVehicleData.m_MapFeature != MapFeature.None || vehicleData.m_WorkVehicleData.m_Resources != Resource.NoResource) && vehicleData.m_WorkVehicleData.m_MapFeature != mapFeature && (vehicleData.m_WorkVehicleData.m_Resources & resource) == Resource.NoResource) || !m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				vehicleData.m_Entity = nativeArray2[j];
				vehicleData.m_TractorData = nativeArray[j];
				if (vehicleData.m_TractorData.m_TrailerType == secondData.m_TrailerData.m_TrailerType && (!(vehicleData.m_TractorData.m_FixedTrailer != Entity.Null) || !(vehicleData.m_TractorData.m_FixedTrailer != secondData.m_Entity)) && (!(secondData.m_TrailerData.m_FixedTractor != Entity.Null) || !(secondData.m_TrailerData.m_FixedTractor != vehicleData.m_Entity)))
				{
					vehicleData.m_ObjectData = nativeArray5[j];
					if (nativeArray4.Length != 0)
					{
						vehicleData.m_TrailerData = nativeArray4[j];
						CheckTractors(workType, mapFeature, resource, vehicleData, secondData, ref random, ref bestFirst, ref bestSecond, ref bestThird, ref bestForth, ref totalProbability);
					}
					else if (PickVehicle(ref random, 100, ref totalProbability))
					{
						bestFirst = vehicleData;
						bestSecond = secondData;
						bestThird = default(VehicleData);
						bestForth = default(VehicleData);
					}
				}
			}
		}
	}

	private void CheckTractors(VehicleWorkType workType, MapFeature mapFeature, Resource resource, VehicleData secondData, VehicleData thirdData, ref Random random, ref VehicleData bestFirst, ref VehicleData bestSecond, ref VehicleData bestThird, ref VehicleData bestForth, ref int totalProbability)
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
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<CarTractorData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTractorData>(ref m_CarTractorDataType);
			if (nativeArray.Length == 0)
			{
				continue;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<WorkVehicleData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WorkVehicleData>(ref m_WorkVehicleDataType);
			NativeArray<CarTrailerData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTrailerData>(ref m_CarTrailerDataType);
			NativeArray<ObjectData> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ObjectData>(ref m_ObjectDataType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				VehicleData vehicleData = new VehicleData
				{
					m_WorkVehicleData = nativeArray3[j]
				};
				if ((vehicleData.m_WorkVehicleData.m_WorkType != VehicleWorkType.None && vehicleData.m_WorkVehicleData.m_WorkType != workType) || ((vehicleData.m_WorkVehicleData.m_MapFeature != MapFeature.None || vehicleData.m_WorkVehicleData.m_Resources != Resource.NoResource) && vehicleData.m_WorkVehicleData.m_MapFeature != mapFeature && (vehicleData.m_WorkVehicleData.m_Resources & resource) == Resource.NoResource) || !m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				vehicleData.m_Entity = nativeArray2[j];
				vehicleData.m_TractorData = nativeArray[j];
				if (vehicleData.m_TractorData.m_TrailerType == secondData.m_TrailerData.m_TrailerType && (!(vehicleData.m_TractorData.m_FixedTrailer != Entity.Null) || !(vehicleData.m_TractorData.m_FixedTrailer != secondData.m_Entity)) && (!(secondData.m_TrailerData.m_FixedTractor != Entity.Null) || !(secondData.m_TrailerData.m_FixedTractor != vehicleData.m_Entity)))
				{
					vehicleData.m_ObjectData = nativeArray5[j];
					if (nativeArray4.Length != 0)
					{
						vehicleData.m_TrailerData = nativeArray4[j];
						CheckTractors(workType, mapFeature, resource, vehicleData, secondData, thirdData, ref random, ref bestFirst, ref bestSecond, ref bestThird, ref bestForth, ref totalProbability);
					}
					else if (PickVehicle(ref random, 100, ref totalProbability))
					{
						bestFirst = vehicleData;
						bestSecond = secondData;
						bestThird = thirdData;
						bestForth = default(VehicleData);
					}
				}
			}
		}
	}

	private void CheckTractors(VehicleWorkType workType, MapFeature mapFeature, Resource resource, VehicleData secondData, VehicleData thirdData, VehicleData forthData, ref Random random, ref VehicleData bestFirst, ref VehicleData bestSecond, ref VehicleData bestThird, ref VehicleData bestForth, ref int totalProbability)
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
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			NativeArray<CarTractorData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTractorData>(ref m_CarTractorDataType);
			if (nativeArray.Length == 0 || ((ArchetypeChunk)(ref chunk)).Has<CarTrailerData>(ref m_CarTrailerDataType))
			{
				continue;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<WorkVehicleData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WorkVehicleData>(ref m_WorkVehicleDataType);
			NativeArray<ObjectData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ObjectData>(ref m_ObjectDataType);
			VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				VehicleData vehicleData = new VehicleData
				{
					m_WorkVehicleData = nativeArray3[j]
				};
				if ((vehicleData.m_WorkVehicleData.m_WorkType != VehicleWorkType.None && vehicleData.m_WorkVehicleData.m_WorkType != workType) || ((vehicleData.m_WorkVehicleData.m_MapFeature != MapFeature.None || vehicleData.m_WorkVehicleData.m_Resources != Resource.NoResource) && vehicleData.m_WorkVehicleData.m_MapFeature != mapFeature && (vehicleData.m_WorkVehicleData.m_Resources & resource) == Resource.NoResource) || !m_RequirementData.CheckRequirements(ref chunk2, j))
				{
					continue;
				}
				vehicleData.m_Entity = nativeArray2[j];
				vehicleData.m_TractorData = nativeArray[j];
				if (vehicleData.m_TractorData.m_TrailerType == secondData.m_TrailerData.m_TrailerType && (!(vehicleData.m_TractorData.m_FixedTrailer != Entity.Null) || !(vehicleData.m_TractorData.m_FixedTrailer != secondData.m_Entity)) && (!(secondData.m_TrailerData.m_FixedTractor != Entity.Null) || !(secondData.m_TrailerData.m_FixedTractor != vehicleData.m_Entity)))
				{
					vehicleData.m_ObjectData = nativeArray4[j];
					if (PickVehicle(ref random, 100, ref totalProbability))
					{
						bestFirst = vehicleData;
						bestSecond = secondData;
						bestThird = thirdData;
						bestForth = forthData;
					}
				}
			}
		}
	}

	private bool PickVehicle(ref Random random, int probability, ref int totalProbability)
	{
		totalProbability += probability;
		return ((Random)(ref random)).NextInt(totalProbability) < probability;
	}
}
