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

public struct TransportVehicleSelectData
{
	private NativeList<ArchetypeChunk> m_PrefabChunks;

	private VehicleSelectRequirementData m_RequirementData;

	private EntityTypeHandle m_EntityType;

	private ComponentTypeHandle<PublicTransportVehicleData> m_PublicTransportVehicleType;

	private ComponentTypeHandle<CargoTransportVehicleData> m_CargoTransportVehicleType;

	private ComponentTypeHandle<TrainData> m_TrainType;

	private ComponentTypeHandle<TrainEngineData> m_TrainEngineType;

	private ComponentTypeHandle<TrainCarriageData> m_TrainCarriageType;

	private ComponentTypeHandle<MultipleUnitTrainData> m_MultipleUnitTrainType;

	private ComponentTypeHandle<TaxiData> m_TaxiType;

	private ComponentTypeHandle<CarData> m_CarType;

	private ComponentTypeHandle<AircraftData> m_AircraftType;

	private ComponentTypeHandle<AirplaneData> m_AirplaneType;

	private ComponentTypeHandle<HelicopterData> m_HelicopterType;

	private ComponentTypeHandle<WatercraftData> m_WatercraftType;

	private ComponentLookup<ObjectData> m_ObjectData;

	private ComponentLookup<MovingObjectData> m_MovingObjectData;

	private ComponentLookup<TrainObjectData> m_TrainObjectData;

	private ComponentLookup<PublicTransportVehicleData> m_PublicTransportVehicleData;

	private ComponentLookup<CargoTransportVehicleData> m_CargoTransportVehicleData;

	private BufferLookup<VehicleCarriageElement> m_VehicleCarriages;

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
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<VehicleData>(),
			ComponentType.ReadOnly<ObjectData>(),
			ComponentType.ReadOnly<PrefabData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<PublicTransportVehicleData>(),
			ComponentType.ReadOnly<CargoTransportVehicleData>(),
			ComponentType.ReadOnly<TrainEngineData>(),
			ComponentType.ReadOnly<TaxiData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Locked>() };
		return val;
	}

	public TransportVehicleSelectData(SystemBase system)
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
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabChunks = default(NativeList<ArchetypeChunk>);
		m_RequirementData = new VehicleSelectRequirementData(system);
		m_EntityType = ((ComponentSystemBase)system).GetEntityTypeHandle();
		m_PublicTransportVehicleType = ((ComponentSystemBase)system).GetComponentTypeHandle<PublicTransportVehicleData>(true);
		m_CargoTransportVehicleType = ((ComponentSystemBase)system).GetComponentTypeHandle<CargoTransportVehicleData>(true);
		m_TrainType = ((ComponentSystemBase)system).GetComponentTypeHandle<TrainData>(true);
		m_TrainEngineType = ((ComponentSystemBase)system).GetComponentTypeHandle<TrainEngineData>(true);
		m_TrainCarriageType = ((ComponentSystemBase)system).GetComponentTypeHandle<TrainCarriageData>(true);
		m_MultipleUnitTrainType = ((ComponentSystemBase)system).GetComponentTypeHandle<MultipleUnitTrainData>(true);
		m_TaxiType = ((ComponentSystemBase)system).GetComponentTypeHandle<TaxiData>(true);
		m_CarType = ((ComponentSystemBase)system).GetComponentTypeHandle<CarData>(true);
		m_AircraftType = ((ComponentSystemBase)system).GetComponentTypeHandle<AircraftData>(true);
		m_AirplaneType = ((ComponentSystemBase)system).GetComponentTypeHandle<AirplaneData>(true);
		m_HelicopterType = ((ComponentSystemBase)system).GetComponentTypeHandle<HelicopterData>(true);
		m_WatercraftType = ((ComponentSystemBase)system).GetComponentTypeHandle<WatercraftData>(true);
		m_ObjectData = system.GetComponentLookup<ObjectData>(true);
		m_MovingObjectData = system.GetComponentLookup<MovingObjectData>(true);
		m_TrainObjectData = system.GetComponentLookup<TrainObjectData>(true);
		m_PublicTransportVehicleData = system.GetComponentLookup<PublicTransportVehicleData>(true);
		m_CargoTransportVehicleData = system.GetComponentLookup<CargoTransportVehicleData>(true);
		m_VehicleCarriages = system.GetBufferLookup<VehicleCarriageElement>(true);
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
		m_PublicTransportVehicleType.Update(system);
		m_CargoTransportVehicleType.Update(system);
		m_TrainType.Update(system);
		m_TrainEngineType.Update(system);
		m_TrainCarriageType.Update(system);
		m_MultipleUnitTrainType.Update(system);
		m_TaxiType.Update(system);
		m_CarType.Update(system);
		m_AircraftType.Update(system);
		m_AirplaneType.Update(system);
		m_HelicopterType.Update(system);
		m_WatercraftType.Update(system);
		m_ObjectData.Update(system);
		m_MovingObjectData.Update(system);
		m_TrainObjectData.Update(system);
		m_PublicTransportVehicleData.Update(system);
		m_CargoTransportVehicleData.Update(system);
		m_VehicleCarriages.Update(system);
	}

	public void PostUpdate(JobHandle jobHandle)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabChunks.Dispose(jobHandle);
	}

	public void ListVehicles(TransportType transportType, EnergyTypes energyTypes, SizeClass sizeClass, PublicTransportPurpose publicTransportPurpose, Resource cargoResources, NativeList<Entity> primaryPrefabs, NativeList<Entity> secondaryPrefabs)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		Random random = Random.CreateFromIndex(0u);
		int2 passengerCapacity = (int2)((publicTransportPurpose != 0) ? new int2(1, int.MaxValue) : int2.op_Implicit(0));
		int2 cargoCapacity = (int2)((cargoResources != Resource.NoResource) ? new int2(1, int.MaxValue) : int2.op_Implicit(0));
		GetRandomVehicle(ref random, transportType, energyTypes, sizeClass, publicTransportPurpose, cargoResources, Entity.Null, Entity.Null, primaryPrefabs, secondaryPrefabs, ignoreTheme: false, out var _, out var _, out var _, ref passengerCapacity, ref cargoCapacity);
	}

	public void SelectVehicle(ref Random random, TransportType transportType, EnergyTypes energyTypes, SizeClass sizeClass, PublicTransportPurpose publicTransportPurpose, Resource cargoResources, out Entity primaryPrefab, out Entity secondaryPrefab, ref int2 passengerCapacity, ref int2 cargoCapacity)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		primaryPrefab = GetRandomVehicle(ref random, transportType, energyTypes, sizeClass, publicTransportPurpose, cargoResources, Entity.Null, Entity.Null, default(NativeList<Entity>), default(NativeList<Entity>), ignoreTheme: false, out var _, out var _, out secondaryPrefab, ref passengerCapacity, ref cargoCapacity);
	}

	public Entity CreateVehicle(ParallelWriter commandBuffer, int jobIndex, ref Random random, Transform transform, Entity source, Entity primaryPrefab, Entity secondaryPrefab, TransportType transportType, EnergyTypes energyTypes, SizeClass sizeClass, PublicTransportPurpose publicTransportPurpose, Resource cargoResources, ref int2 passengerCapacity, ref int2 cargoCapacity, bool parked)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		NativeList<LayoutElement> layout = default(NativeList<LayoutElement>);
		Entity result = CreateVehicle(commandBuffer, jobIndex, ref random, transform, source, primaryPrefab, secondaryPrefab, transportType, energyTypes, sizeClass, publicTransportPurpose, cargoResources, ref passengerCapacity, ref cargoCapacity, parked, ref layout);
		if (layout.IsCreated)
		{
			layout.Dispose();
		}
		return result;
	}

	public Entity CreateVehicle(ParallelWriter commandBuffer, int jobIndex, ref Random random, Transform transform, Entity source, Entity primaryPrefab, Entity secondaryPrefab, TransportType transportType, EnergyTypes energyTypes, SizeClass sizeClass, PublicTransportPurpose publicTransportPurpose, Resource cargoResources, ref int2 passengerCapacity, ref int2 cargoCapacity, bool parked, ref NativeList<LayoutElement> layout)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		primaryPrefab = GetRandomVehicle(ref random, transportType, energyTypes, sizeClass, publicTransportPurpose, cargoResources, primaryPrefab, secondaryPrefab, default(NativeList<Entity>), default(NativeList<Entity>), ignoreTheme: false, out var isMultipleUnitTrain, out var unitCount, out var secondaryResult, ref passengerCapacity, ref cargoCapacity);
		secondaryPrefab = secondaryResult;
		if (primaryPrefab == Entity.Null)
		{
			return Entity.Null;
		}
		Entity val = ((transportType != TransportType.Train && transportType != TransportType.Tram && transportType != TransportType.Subway) ? ((ParallelWriter)(ref commandBuffer)).CreateEntity(jobIndex, GetArchetype(primaryPrefab, controller: false, parked)) : ((ParallelWriter)(ref commandBuffer)).CreateEntity(jobIndex, GetArchetype(primaryPrefab, controller: true, parked)));
		((ParallelWriter)(ref commandBuffer)).SetComponent<Transform>(jobIndex, val, transform);
		((ParallelWriter)(ref commandBuffer)).SetComponent<PrefabRef>(jobIndex, val, new PrefabRef(primaryPrefab));
		((ParallelWriter)(ref commandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val, new PseudoRandomSeed(ref random));
		AddTransportComponents(commandBuffer, jobIndex, publicTransportPurpose, val);
		if (!parked && source != Entity.Null)
		{
			((ParallelWriter)(ref commandBuffer)).AddComponent<TripSource>(jobIndex, val, new TripSource(source));
			((ParallelWriter)(ref commandBuffer)).AddComponent<Unspawned>(jobIndex, val, default(Unspawned));
		}
		bool flag = false;
		if (transportType == TransportType.Train || transportType == TransportType.Tram || transportType == TransportType.Subway)
		{
			((ParallelWriter)(ref commandBuffer)).SetComponent<Controller>(jobIndex, val, new Controller(val));
			flag = true;
			if (layout.IsCreated)
			{
				layout.Clear();
			}
			else
			{
				layout = new NativeList<LayoutElement>(32, AllocatorHandle.op_Implicit((Allocator)2));
			}
		}
		if (flag)
		{
			int num = 0;
			if (isMultipleUnitTrain)
			{
				LayoutElement layoutElement = new LayoutElement(val);
				layout.Add(ref layoutElement);
			}
			Entity val2 = (isMultipleUnitTrain ? primaryPrefab : secondaryPrefab);
			if (val2 != Entity.Null)
			{
				EntityArchetype archetype = GetArchetype(val2, controller: false, parked);
				for (int i = 0; i < unitCount; i++)
				{
					if (!isMultipleUnitTrain || i != 0)
					{
						Game.Vehicles.TrainFlags trainFlags = (Game.Vehicles.TrainFlags)0u;
						if (!isMultipleUnitTrain && i != 0 && ((Random)(ref random)).NextBool())
						{
							trainFlags |= Game.Vehicles.TrainFlags.Reversed;
						}
						Entity val3 = ((ParallelWriter)(ref commandBuffer)).CreateEntity(jobIndex, archetype);
						((ParallelWriter)(ref commandBuffer)).SetComponent<Transform>(jobIndex, val3, transform);
						((ParallelWriter)(ref commandBuffer)).SetComponent<PrefabRef>(jobIndex, val3, new PrefabRef(val2));
						((ParallelWriter)(ref commandBuffer)).SetComponent<Controller>(jobIndex, val3, new Controller(val));
						((ParallelWriter)(ref commandBuffer)).SetComponent<Train>(jobIndex, val3, new Train(trainFlags));
						((ParallelWriter)(ref commandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val3, new PseudoRandomSeed(ref random));
						AddTransportComponents(commandBuffer, jobIndex, publicTransportPurpose, val3);
						if (!parked && source != Entity.Null)
						{
							((ParallelWriter)(ref commandBuffer)).AddComponent<TripSource>(jobIndex, val3, new TripSource(source));
							((ParallelWriter)(ref commandBuffer)).AddComponent<Unspawned>(jobIndex, val3, default(Unspawned));
						}
						LayoutElement layoutElement = new LayoutElement(val3);
						layout.Add(ref layoutElement);
					}
					if (!m_VehicleCarriages.HasBuffer(val2))
					{
						continue;
					}
					DynamicBuffer<VehicleCarriageElement> val4 = m_VehicleCarriages[val2];
					for (int j = 0; j < val4.Length; j++)
					{
						VehicleCarriageElement vehicleCarriageElement = val4[j];
						if (vehicleCarriageElement.m_Prefab == Entity.Null)
						{
							num += vehicleCarriageElement.m_Count.x;
							continue;
						}
						EntityArchetype archetype2 = GetArchetype(vehicleCarriageElement.m_Prefab, controller: false, parked);
						for (int k = 0; k < vehicleCarriageElement.m_Count.x; k++)
						{
							Game.Vehicles.TrainFlags trainFlags2 = (Game.Vehicles.TrainFlags)0u;
							switch (vehicleCarriageElement.m_Direction)
							{
							case VehicleCarriageDirection.Reversed:
								trainFlags2 |= Game.Vehicles.TrainFlags.Reversed;
								break;
							case VehicleCarriageDirection.Random:
								if (((Random)(ref random)).NextBool())
								{
									trainFlags2 |= Game.Vehicles.TrainFlags.Reversed;
								}
								break;
							}
							Entity val5 = ((ParallelWriter)(ref commandBuffer)).CreateEntity(jobIndex, archetype2);
							((ParallelWriter)(ref commandBuffer)).SetComponent<Transform>(jobIndex, val5, transform);
							((ParallelWriter)(ref commandBuffer)).SetComponent<PrefabRef>(jobIndex, val5, new PrefabRef(vehicleCarriageElement.m_Prefab));
							((ParallelWriter)(ref commandBuffer)).SetComponent<Controller>(jobIndex, val5, new Controller(val));
							((ParallelWriter)(ref commandBuffer)).SetComponent<Train>(jobIndex, val5, new Train(trainFlags2));
							((ParallelWriter)(ref commandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val5, new PseudoRandomSeed(ref random));
							AddTransportComponents(commandBuffer, jobIndex, publicTransportPurpose, val5);
							if (!parked && source != Entity.Null)
							{
								((ParallelWriter)(ref commandBuffer)).AddComponent<TripSource>(jobIndex, val5, new TripSource(source));
								((ParallelWriter)(ref commandBuffer)).AddComponent<Unspawned>(jobIndex, val5, default(Unspawned));
							}
							LayoutElement layoutElement = new LayoutElement(val5);
							layout.Add(ref layoutElement);
						}
					}
				}
			}
			if (!isMultipleUnitTrain)
			{
				LayoutElement layoutElement = new LayoutElement(val);
				layout.Add(ref layoutElement);
				num--;
			}
			if (num > 0)
			{
				EntityArchetype archetype3 = GetArchetype(primaryPrefab, controller: false, parked);
				for (int l = 0; l < num; l++)
				{
					Game.Vehicles.TrainFlags trainFlags3 = (Game.Vehicles.TrainFlags)0u;
					if (((Random)(ref random)).NextBool())
					{
						trainFlags3 |= Game.Vehicles.TrainFlags.Reversed;
					}
					Entity val6 = ((ParallelWriter)(ref commandBuffer)).CreateEntity(jobIndex, archetype3);
					((ParallelWriter)(ref commandBuffer)).SetComponent<Transform>(jobIndex, val6, transform);
					((ParallelWriter)(ref commandBuffer)).SetComponent<PrefabRef>(jobIndex, val6, new PrefabRef(primaryPrefab));
					((ParallelWriter)(ref commandBuffer)).SetComponent<Controller>(jobIndex, val6, new Controller(val));
					((ParallelWriter)(ref commandBuffer)).SetComponent<Train>(jobIndex, val6, new Train(trainFlags3));
					((ParallelWriter)(ref commandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val6, new PseudoRandomSeed(ref random));
					AddTransportComponents(commandBuffer, jobIndex, publicTransportPurpose, val6);
					if (!parked && source != Entity.Null)
					{
						((ParallelWriter)(ref commandBuffer)).AddComponent<TripSource>(jobIndex, val6, new TripSource(source));
						((ParallelWriter)(ref commandBuffer)).AddComponent<Unspawned>(jobIndex, val6, default(Unspawned));
					}
					LayoutElement layoutElement = new LayoutElement(val6);
					layout.Add(ref layoutElement);
				}
			}
			((ParallelWriter)(ref commandBuffer)).SetBuffer<LayoutElement>(jobIndex, val).CopyFrom(layout.AsArray());
		}
		return val;
	}

	private void AddTransportComponents(ParallelWriter commandBuffer, int jobIndex, PublicTransportPurpose publicTransportPurpose, Entity entity)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if ((publicTransportPurpose & PublicTransportPurpose.TransportLine) != 0)
		{
			((ParallelWriter)(ref commandBuffer)).AddComponent<PassengerTransport>(jobIndex, entity, default(PassengerTransport));
		}
		if ((publicTransportPurpose & PublicTransportPurpose.Evacuation) != 0)
		{
			((ParallelWriter)(ref commandBuffer)).AddComponent<EvacuatingTransport>(jobIndex, entity, default(EvacuatingTransport));
		}
		if ((publicTransportPurpose & PublicTransportPurpose.PrisonerTransport) != 0)
		{
			((ParallelWriter)(ref commandBuffer)).AddComponent<PrisonerTransport>(jobIndex, entity, default(PrisonerTransport));
		}
	}

	private EntityArchetype GetArchetype(Entity prefab, bool controller, bool parked)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (controller)
		{
			TrainObjectData trainObjectData = m_TrainObjectData[prefab];
			if (!parked)
			{
				return trainObjectData.m_ControllerArchetype;
			}
			return trainObjectData.m_StoppedControllerArchetype;
		}
		if (parked)
		{
			return m_MovingObjectData[prefab].m_StoppedArchetype;
		}
		return m_ObjectData[prefab].m_Archetype;
	}

	private Entity GetRandomVehicle(ref Random random, TransportType transportType, EnergyTypes energyTypes, SizeClass sizeClass, PublicTransportPurpose publicTransportPurpose, Resource cargoResources, Entity primaryPrefab, Entity secondaryPrefab, NativeList<Entity> primaryPrefabs, NativeList<Entity> secondaryPrefabs, bool ignoreTheme, out bool isMultipleUnitTrain, out int unitCount, out Entity secondaryResult, ref int2 passengerCapacity, ref int2 cargoCapacity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0874: Unknown result type (might be due to invalid IL or missing references)
		//IL_0879: Unknown result type (might be due to invalid IL or missing references)
		//IL_087e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0888: Unknown result type (might be due to invalid IL or missing references)
		//IL_088d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0897: Unknown result type (might be due to invalid IL or missing references)
		//IL_089c: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c43: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_093a: Unknown result type (might be due to invalid IL or missing references)
		//IL_093f: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_078a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af7: Unknown result type (might be due to invalid IL or missing references)
		//IL_094f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0951: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b09: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_0964: Unknown result type (might be due to invalid IL or missing references)
		//IL_0966: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0807: Unknown result type (might be due to invalid IL or missing references)
		//IL_0809: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0823: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b70: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b75: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_083c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0841: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ced: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d46: Unknown result type (might be due to invalid IL or missing references)
		Entity val = Entity.Null;
		secondaryResult = Entity.Null;
		int2 val2 = int2.op_Implicit(0);
		int2 val3 = int2.op_Implicit(0);
		isMultipleUnitTrain = false;
		unitCount = 1;
		int num = 1;
		TrackTypes trackTypes = TrackTypes.None;
		HelicopterType helicopterType = HelicopterType.Helicopter;
		switch (transportType)
		{
		case TransportType.Train:
			trackTypes = TrackTypes.Train;
			break;
		case TransportType.Tram:
			trackTypes = TrackTypes.Tram;
			break;
		case TransportType.Subway:
			trackTypes = TrackTypes.Subway;
			break;
		case TransportType.Helicopter:
			helicopterType = HelicopterType.Helicopter;
			break;
		case TransportType.Rocket:
			helicopterType = HelicopterType.Rocket;
			break;
		}
		for (int i = 0; i < m_PrefabChunks.Length; i++)
		{
			ArchetypeChunk chunk = m_PrefabChunks[i];
			switch (transportType)
			{
			case TransportType.Bus:
			{
				NativeArray<CarData> nativeArray18 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarData>(ref m_CarType);
				if (nativeArray18.Length == 0)
				{
					break;
				}
				NativeArray<Entity> nativeArray19 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<PublicTransportVehicleData> nativeArray20 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PublicTransportVehicleData>(ref m_PublicTransportVehicleType);
				if (nativeArray20.Length == 0)
				{
					break;
				}
				VehicleSelectRequirementData.Chunk chunk6 = m_RequirementData.GetChunk(chunk);
				for (int num6 = 0; num6 < nativeArray18.Length; num6++)
				{
					if ((nativeArray20[num6].m_PurposeMask & publicTransportPurpose) == 0)
					{
						continue;
					}
					CarData carData2 = nativeArray18[num6];
					if ((carData2.m_EnergyType != EnergyTypes.None && (carData2.m_EnergyType & energyTypes) == 0) || carData2.m_SizeClass != sizeClass)
					{
						continue;
					}
					Entity val9 = nativeArray19[num6];
					if (m_RequirementData.CheckRequirements(ref chunk6, num6, ignoreTheme || val9 == primaryPrefab))
					{
						int num7 = math.select(0, 2, val9 == primaryPrefab);
						num7 += math.select(0, 1, (carData2.m_EnergyType & EnergyTypes.Fuel) != 0);
						if (primaryPrefabs.IsCreated)
						{
							primaryPrefabs.Add(ref val9);
						}
						if (PickVehicle(ref random, 100, num7, ref val2.x, ref val3.x))
						{
							val = val9;
							passengerCapacity = int2.op_Implicit(nativeArray20[num6].m_PassengerCapacity);
						}
					}
				}
				break;
			}
			case TransportType.Train:
			case TransportType.Tram:
			case TransportType.Subway:
			{
				NativeArray<TrainData> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TrainData>(ref m_TrainType);
				if (nativeArray10.Length == 0)
				{
					break;
				}
				NativeArray<Entity> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<TrainEngineData> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TrainEngineData>(ref m_TrainEngineType);
				NativeArray<PublicTransportVehicleData> nativeArray13 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PublicTransportVehicleData>(ref m_PublicTransportVehicleType);
				NativeArray<CargoTransportVehicleData> nativeArray14 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CargoTransportVehicleData>(ref m_CargoTransportVehicleType);
				bool flag = nativeArray12.Length != 0;
				bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<TrainCarriageData>(ref m_TrainCarriageType);
				bool flag3 = ((ArchetypeChunk)(ref chunk)).Has<MultipleUnitTrainData>(ref m_MultipleUnitTrainType);
				VehicleSelectRequirementData.Chunk chunk4 = m_RequirementData.GetChunk(chunk);
				if ((flag && flag3) || (flag2 && !flag3))
				{
					if (publicTransportPurpose != (PublicTransportPurpose)0 != (nativeArray13.Length != 0) || cargoResources != Resource.NoResource != (nativeArray14.Length != 0))
					{
						break;
					}
					for (int l = 0; l < nativeArray10.Length; l++)
					{
						if ((publicTransportPurpose != 0 && (nativeArray13[l].m_PurposeMask & publicTransportPurpose) == 0) || (cargoResources != Resource.NoResource && (nativeArray14[l].m_Resources & cargoResources) == Resource.NoResource))
						{
							continue;
						}
						TrainData trainData = nativeArray10[l];
						if ((trainData.m_EnergyType != EnergyTypes.None && (trainData.m_EnergyType & energyTypes) == 0) || trainData.m_TrackType != trackTypes)
						{
							continue;
						}
						Entity val6 = nativeArray11[l];
						if (!m_RequirementData.CheckRequirements(ref chunk4, l, ignoreTheme || val6 == primaryPrefab))
						{
							continue;
						}
						int num3 = math.select(0, 2, val6 == primaryPrefab);
						num3 += math.select(0, 1, (trainData.m_EnergyType & EnergyTypes.Fuel) != 0);
						if (primaryPrefabs.IsCreated)
						{
							primaryPrefabs.Add(ref val6);
						}
						if (PickVehicle(ref random, 100, num3, ref val2.x, ref val3.x))
						{
							isMultipleUnitTrain = flag3;
							if (flag)
							{
								unitCount = nativeArray12[l].m_Count.x;
							}
							val = val6;
							if (publicTransportPurpose != 0)
							{
								passengerCapacity = int2.op_Implicit(nativeArray13[l].m_PassengerCapacity);
							}
							if (cargoResources != Resource.NoResource)
							{
								cargoCapacity = int2.op_Implicit(nativeArray14[l].m_CargoCapacity);
							}
						}
					}
				}
				else
				{
					if (!flag || flag3)
					{
						break;
					}
					for (int m = 0; m < nativeArray10.Length; m++)
					{
						TrainData trainData2 = nativeArray10[m];
						if ((trainData2.m_EnergyType != EnergyTypes.None && (trainData2.m_EnergyType & energyTypes) == 0) || trainData2.m_TrackType != trackTypes)
						{
							continue;
						}
						Entity val7 = nativeArray11[m];
						if (m_RequirementData.CheckRequirements(ref chunk4, m, ignoreTheme || val7 == secondaryPrefab))
						{
							int num4 = math.select(0, 2, val7 == secondaryPrefab);
							num4 += math.select(0, 1, (trainData2.m_EnergyType & EnergyTypes.Fuel) != 0);
							if (secondaryPrefabs.IsCreated)
							{
								secondaryPrefabs.Add(ref val7);
							}
							if (PickVehicle(ref random, 100, num4, ref val2.y, ref val3.y))
							{
								num = nativeArray12[m].m_Count.x;
								secondaryResult = val7;
							}
						}
					}
				}
				break;
			}
			case TransportType.Taxi:
			{
				NativeArray<CarData> nativeArray15 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarData>(ref m_CarType);
				if (nativeArray15.Length == 0)
				{
					break;
				}
				NativeArray<Entity> nativeArray16 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<TaxiData> nativeArray17 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TaxiData>(ref m_TaxiType);
				if (nativeArray17.Length == 0)
				{
					break;
				}
				VehicleSelectRequirementData.Chunk chunk5 = m_RequirementData.GetChunk(chunk);
				for (int n = 0; n < nativeArray15.Length; n++)
				{
					CarData carData = nativeArray15[n];
					if ((carData.m_EnergyType != EnergyTypes.None && (carData.m_EnergyType & energyTypes) == 0) || carData.m_SizeClass != sizeClass)
					{
						continue;
					}
					Entity val8 = nativeArray16[n];
					if (m_RequirementData.CheckRequirements(ref chunk5, n, ignoreTheme || val8 == primaryPrefab))
					{
						int num5 = math.select(0, 2, val8 == primaryPrefab);
						num5 += math.select(0, 1, (carData.m_EnergyType & EnergyTypes.Electricity) != 0);
						if (primaryPrefabs.IsCreated)
						{
							primaryPrefabs.Add(ref val8);
						}
						if (PickVehicle(ref random, 100, num5, ref val2.x, ref val3.x))
						{
							val = val8;
							passengerCapacity = int2.op_Implicit(nativeArray17[n].m_PassengerCapacity);
						}
					}
				}
				break;
			}
			case TransportType.Ship:
			{
				NativeArray<WatercraftData> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WatercraftData>(ref m_WatercraftType);
				if (nativeArray6.Length == 0)
				{
					break;
				}
				NativeArray<Entity> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<PublicTransportVehicleData> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PublicTransportVehicleData>(ref m_PublicTransportVehicleType);
				NativeArray<CargoTransportVehicleData> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CargoTransportVehicleData>(ref m_CargoTransportVehicleType);
				if (publicTransportPurpose != (PublicTransportPurpose)0 != (nativeArray8.Length != 0) || cargoResources != Resource.NoResource != (nativeArray9.Length != 0))
				{
					break;
				}
				VehicleSelectRequirementData.Chunk chunk3 = m_RequirementData.GetChunk(chunk);
				for (int k = 0; k < nativeArray6.Length; k++)
				{
					WatercraftData watercraftData = nativeArray6[k];
					if ((watercraftData.m_EnergyType != EnergyTypes.None && (watercraftData.m_EnergyType & energyTypes) == 0) || watercraftData.m_SizeClass != sizeClass || (publicTransportPurpose != 0 && (nativeArray8[k].m_PurposeMask & publicTransportPurpose) == 0) || (cargoResources != Resource.NoResource && (nativeArray9[k].m_Resources & cargoResources) == Resource.NoResource))
					{
						continue;
					}
					Entity val5 = nativeArray7[k];
					if (!m_RequirementData.CheckRequirements(ref chunk3, k, ignoreTheme || val5 == primaryPrefab))
					{
						continue;
					}
					int num2 = math.select(0, 2, val5 == primaryPrefab);
					num2 += math.select(0, 1, (watercraftData.m_EnergyType & EnergyTypes.Fuel) != 0);
					if (primaryPrefabs.IsCreated)
					{
						primaryPrefabs.Add(ref val5);
					}
					if (PickVehicle(ref random, 100, num2, ref val2.x, ref val3.x))
					{
						val = val5;
						if (publicTransportPurpose != 0)
						{
							passengerCapacity = int2.op_Implicit(nativeArray8[k].m_PassengerCapacity);
						}
						if (cargoResources != Resource.NoResource)
						{
							cargoCapacity = int2.op_Implicit(nativeArray9[k].m_CargoCapacity);
						}
					}
				}
				break;
			}
			case TransportType.Airplane:
			{
				if (!((ArchetypeChunk)(ref chunk)).Has<AirplaneData>(ref m_AirplaneType))
				{
					break;
				}
				NativeArray<Entity> nativeArray21 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<AircraftData> nativeArray22 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AircraftData>(ref m_AircraftType);
				NativeArray<PublicTransportVehicleData> nativeArray23 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PublicTransportVehicleData>(ref m_PublicTransportVehicleType);
				NativeArray<CargoTransportVehicleData> nativeArray24 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CargoTransportVehicleData>(ref m_CargoTransportVehicleType);
				if (publicTransportPurpose != (PublicTransportPurpose)0 != (nativeArray23.Length != 0) || cargoResources != Resource.NoResource != (nativeArray24.Length != 0))
				{
					break;
				}
				VehicleSelectRequirementData.Chunk chunk7 = m_RequirementData.GetChunk(chunk);
				for (int num8 = 0; num8 < nativeArray22.Length; num8++)
				{
					if (nativeArray22[num8].m_SizeClass != sizeClass || (publicTransportPurpose != 0 && (nativeArray23[num8].m_PurposeMask & publicTransportPurpose) == 0) || (cargoResources != Resource.NoResource && (nativeArray24[num8].m_Resources & cargoResources) == Resource.NoResource))
					{
						continue;
					}
					Entity val10 = nativeArray21[num8];
					if (!m_RequirementData.CheckRequirements(ref chunk7, num8, ignoreTheme || val10 == primaryPrefab))
					{
						continue;
					}
					int priority2 = math.select(0, 2, val10 == primaryPrefab);
					if (primaryPrefabs.IsCreated)
					{
						primaryPrefabs.Add(ref val10);
					}
					if (PickVehicle(ref random, 100, priority2, ref val2.x, ref val3.x))
					{
						val = val10;
						if (publicTransportPurpose != 0)
						{
							passengerCapacity = int2.op_Implicit(nativeArray23[num8].m_PassengerCapacity);
						}
						if (cargoResources != Resource.NoResource)
						{
							cargoCapacity = int2.op_Implicit(nativeArray24[num8].m_CargoCapacity);
						}
					}
				}
				break;
			}
			case TransportType.Helicopter:
			case TransportType.Rocket:
			{
				NativeArray<HelicopterData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HelicopterData>(ref m_HelicopterType);
				if (nativeArray.Length == 0)
				{
					break;
				}
				NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<AircraftData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AircraftData>(ref m_AircraftType);
				NativeArray<PublicTransportVehicleData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PublicTransportVehicleData>(ref m_PublicTransportVehicleType);
				NativeArray<CargoTransportVehicleData> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CargoTransportVehicleData>(ref m_CargoTransportVehicleType);
				if (publicTransportPurpose != (PublicTransportPurpose)0 != (nativeArray4.Length != 0) || cargoResources != Resource.NoResource != (nativeArray5.Length != 0))
				{
					break;
				}
				VehicleSelectRequirementData.Chunk chunk2 = m_RequirementData.GetChunk(chunk);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					if (nativeArray3[j].m_SizeClass != sizeClass || (publicTransportPurpose != 0 && (nativeArray4[j].m_PurposeMask & publicTransportPurpose) == 0) || (cargoResources != Resource.NoResource && (nativeArray5[j].m_Resources & cargoResources) == Resource.NoResource) || nativeArray[j].m_HelicopterType != helicopterType)
					{
						continue;
					}
					Entity val4 = nativeArray2[j];
					if (!m_RequirementData.CheckRequirements(ref chunk2, j, ignoreTheme || val4 == primaryPrefab))
					{
						continue;
					}
					int priority = math.select(0, 2, val4 == primaryPrefab);
					if (primaryPrefabs.IsCreated)
					{
						primaryPrefabs.Add(ref val4);
					}
					if (PickVehicle(ref random, 100, priority, ref val2.x, ref val3.x))
					{
						val = val4;
						if (publicTransportPurpose != 0)
						{
							passengerCapacity = int2.op_Implicit(nativeArray4[j].m_PassengerCapacity);
						}
						if (cargoResources != Resource.NoResource)
						{
							cargoCapacity = int2.op_Implicit(nativeArray5[j].m_CargoCapacity);
						}
					}
				}
				break;
			}
			}
		}
		if (isMultipleUnitTrain)
		{
			secondaryResult = Entity.Null;
		}
		else
		{
			unitCount = num;
		}
		bool flag4 = false;
		if (transportType == TransportType.Train || transportType == TransportType.Tram || transportType == TransportType.Subway)
		{
			flag4 = true;
		}
		if (flag4)
		{
			passengerCapacity.y = 0;
			cargoCapacity.y = 0;
			int num9 = 0;
			if (isMultipleUnitTrain)
			{
				passengerCapacity.y += passengerCapacity.x;
				cargoCapacity.y += cargoCapacity.x;
			}
			Entity val11 = (isMultipleUnitTrain ? val : secondaryResult);
			if (val11 != Entity.Null)
			{
				PublicTransportVehicleData publicTransportVehicleData = default(PublicTransportVehicleData);
				CargoTransportVehicleData cargoTransportVehicleData = default(CargoTransportVehicleData);
				for (int num10 = 0; num10 < unitCount; num10++)
				{
					if (isMultipleUnitTrain && num10 != 0)
					{
						passengerCapacity.y += passengerCapacity.x;
						cargoCapacity.y += cargoCapacity.x;
					}
					if (!m_VehicleCarriages.HasBuffer(val11))
					{
						continue;
					}
					DynamicBuffer<VehicleCarriageElement> val12 = m_VehicleCarriages[val11];
					for (int num11 = 0; num11 < val12.Length; num11++)
					{
						VehicleCarriageElement vehicleCarriageElement = val12[num11];
						if (vehicleCarriageElement.m_Prefab == Entity.Null)
						{
							num9 += vehicleCarriageElement.m_Count.x;
							continue;
						}
						if (publicTransportPurpose != 0 && m_PublicTransportVehicleData.TryGetComponent(vehicleCarriageElement.m_Prefab, ref publicTransportVehicleData))
						{
							passengerCapacity.y += publicTransportVehicleData.m_PassengerCapacity * vehicleCarriageElement.m_Count.x;
						}
						if (cargoResources != Resource.NoResource && m_CargoTransportVehicleData.TryGetComponent(vehicleCarriageElement.m_Prefab, ref cargoTransportVehicleData))
						{
							cargoCapacity.y += cargoTransportVehicleData.m_CargoCapacity * vehicleCarriageElement.m_Count.x;
						}
					}
				}
			}
			if (!isMultipleUnitTrain)
			{
				passengerCapacity.y += passengerCapacity.x;
				cargoCapacity.y += cargoCapacity.x;
				num9--;
			}
			if (num9 > 0)
			{
				passengerCapacity.y += passengerCapacity.x * num9;
				cargoCapacity.y += cargoCapacity.x * num9;
			}
			passengerCapacity.x = passengerCapacity.y;
			cargoCapacity.x = cargoCapacity.y;
		}
		return val;
	}

	private bool PickVehicle(ref Random random, int probability, int priority, ref int totalProbability, ref int selectedPriority)
	{
		if (priority < selectedPriority)
		{
			return false;
		}
		if (priority > selectedPriority)
		{
			totalProbability = 0;
			selectedPriority = priority;
		}
		totalProbability += probability;
		return ((Random)(ref random)).NextInt(totalProbability) < probability;
	}
}
