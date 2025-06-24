using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class RoadsInfoviewUISystem : InfoviewUISystemBase
{
	private enum Result
	{
		Slots,
		Parked,
		ResultCount
	}

	[BurstCompile]
	private struct UpdateParkingJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubNet> m_SubNetHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubLane> m_SubLaneHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Objects.SubObject> m_SubObjectHandle;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveFromEntity;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarFromEntity;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneFromEntity;

		[ReadOnly]
		public ComponentLookup<GarageLane> m_GarageLaneFromEntity;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefFromEntity;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> m_ParkingLaneDataFromEntity;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLaneFromEntity;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjectFromEntity;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjectFromEntity;

		public NativeArray<int> m_Results;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityHandle);
			BufferAccessor<Game.Net.SubLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Net.SubLane>(ref m_SubLaneHandle);
			BufferAccessor<Game.Net.SubNet> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Net.SubNet>(ref m_SubNetHandle);
			BufferAccessor<Game.Objects.SubObject> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Objects.SubObject>(ref m_SubObjectHandle);
			int num = 0;
			int parked = 0;
			for (int i = 0; i < nativeArray.Length; i++)
			{
				int slots = 0;
				if (bufferAccessor.Length != 0)
				{
					CheckParkingLanes(bufferAccessor[i], ref slots, ref parked);
				}
				if (bufferAccessor2.Length != 0)
				{
					CheckParkingLanes(bufferAccessor2[i], ref slots, ref parked);
				}
				if (bufferAccessor3.Length != 0)
				{
					CheckParkingLanes(bufferAccessor3[i], ref slots, ref parked);
				}
				num += math.select(0, slots, slots > 0);
			}
			ref NativeArray<int> reference = ref m_Results;
			reference[0] = reference[0] + num;
			reference = ref m_Results;
			reference[1] = reference[1] + parked;
		}

		private void CheckParkingLanes(DynamicBuffer<Game.Objects.SubObject> subObjects, ref int slots, ref int parked)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Net.SubLane> subLanes = default(DynamicBuffer<Game.Net.SubLane>);
			DynamicBuffer<Game.Objects.SubObject> subObjects2 = default(DynamicBuffer<Game.Objects.SubObject>);
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (m_SubLaneFromEntity.TryGetBuffer(subObject, ref subLanes))
				{
					CheckParkingLanes(subLanes, ref slots, ref parked);
				}
				if (m_SubObjectFromEntity.TryGetBuffer(subObject, ref subObjects2))
				{
					CheckParkingLanes(subObjects2, ref slots, ref parked);
				}
			}
		}

		private void CheckParkingLanes(DynamicBuffer<Game.Net.SubNet> subNets, ref int slots, ref int parked)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Net.SubLane> subLanes = default(DynamicBuffer<Game.Net.SubLane>);
			for (int i = 0; i < subNets.Length; i++)
			{
				Entity subNet = subNets[i].m_SubNet;
				if (m_SubLaneFromEntity.TryGetBuffer(subNet, ref subLanes))
				{
					CheckParkingLanes(subLanes, ref slots, ref parked);
				}
			}
		}

		private void CheckParkingLanes(DynamicBuffer<Game.Net.SubLane> subLanes, ref int slots, ref int parked)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			Game.Net.ParkingLane parkingLane = default(Game.Net.ParkingLane);
			GarageLane garageLane = default(GarageLane);
			for (int i = 0; i < subLanes.Length; i++)
			{
				Entity subLane = subLanes[i].m_SubLane;
				if (m_ParkingLaneFromEntity.TryGetComponent(subLane, ref parkingLane))
				{
					if ((parkingLane.m_Flags & ParkingLaneFlags.VirtualLane) != 0)
					{
						continue;
					}
					Entity prefab = m_PrefabRefFromEntity[subLane].m_Prefab;
					Curve curve = m_CurveFromEntity[subLane];
					DynamicBuffer<LaneObject> val = m_LaneObjectFromEntity[subLane];
					ParkingLaneData prefabParkingLane = m_ParkingLaneDataFromEntity[prefab];
					if (prefabParkingLane.m_SlotInterval != 0f)
					{
						int parkingSlotCount = NetUtils.GetParkingSlotCount(curve, parkingLane, prefabParkingLane);
						slots += parkingSlotCount;
					}
					else
					{
						slots = -1000000;
					}
					for (int j = 0; j < val.Length; j++)
					{
						if (m_ParkedCarFromEntity.HasComponent(val[j].m_LaneObject))
						{
							parked++;
						}
					}
				}
				else if (m_GarageLaneFromEntity.TryGetComponent(subLane, ref garageLane))
				{
					slots += garageLane.m_VehicleCapacity;
					parked += garageLane.m_VehicleCount;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarageLane> __Game_Net_GarageLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> __Game_Prefabs_ParkingLaneData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneObject> __Game_Net_LaneObject_RO_BufferLookup;

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
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_SubNet_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Net.SubNet>(true);
			__Game_Net_SubLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Net.SubLane>(true);
			__Game_Objects_SubObject_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Objects.SubObject>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Net_GarageLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarageLane>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ParkingLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingLaneData>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
		}
	}

	private const string kGroup = "roadsInfo";

	private CityStatisticsSystem m_CityStatisticsSystem;

	private ValueBinding<float> m_ParkingCapacity;

	private ValueBinding<int> m_ParkedCars;

	private ValueBinding<int> m_ParkingIncome;

	private ValueBinding<IndicatorValue> m_ParkingAvailability;

	private EntityQuery m_ParkingFacilityQuery;

	private EntityQuery m_ParkingFacilityModifiedQuery;

	private NativeArray<int> m_Results;

	private TypeHandle __TypeHandle;

	protected override bool Active
	{
		get
		{
			if (!base.Active && !((EventBindingBase)m_ParkedCars).active && !((EventBindingBase)m_ParkingAvailability).active && !((EventBindingBase)m_ParkingCapacity).active)
			{
				return ((EventBindingBase)m_ParkingIncome).active;
			}
			return true;
		}
	}

	protected override bool Modified => !((EntityQuery)(ref m_ParkingFacilityModifiedQuery)).IsEmptyIgnoreFilter;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Buildings.ParkingFacility>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Net.SubLane>(),
			ComponentType.ReadOnly<Game.Net.SubNet>(),
			ComponentType.ReadOnly<Game.Objects.SubObject>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_ParkingFacilityQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Buildings.ParkingFacility>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array2[0] = val;
		m_ParkingFacilityModifiedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		AddBinding((IBinding)(object)(m_ParkingCapacity = new ValueBinding<float>("roadsInfo", "parkingCapacity", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_ParkedCars = new ValueBinding<int>("roadsInfo", "parkedCars", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_ParkingIncome = new ValueBinding<int>("roadsInfo", "parkingIncome", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_ParkingAvailability = new ValueBinding<IndicatorValue>("roadsInfo", "parkingAvailability", default(IndicatorValue), (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		m_Results = new NativeArray<int>(2, (Allocator)4, (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Results.Dispose();
		base.OnDestroy();
	}

	protected override void PerformUpdate()
	{
		UpdateCapacity();
		UpdateAvailability();
		UpdateIncome();
	}

	private void ResetResults()
	{
		for (int i = 0; i < m_Results.Length; i++)
		{
			m_Results[i] = 0;
		}
	}

	private void UpdateCapacity()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		ResetResults();
		JobHandle val = JobChunkExtensions.Schedule<UpdateParkingJob>(new UpdateParkingJob
		{
			m_EntityHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubNetHandle = InternalCompilerInterface.GetBufferTypeHandle<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubLaneHandle = InternalCompilerInterface.GetBufferTypeHandle<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjectHandle = InternalCompilerInterface.GetBufferTypeHandle<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurveFromEntity = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarFromEntity = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneFromEntity = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarageLaneFromEntity = InternalCompilerInterface.GetComponentLookup<GarageLane>(ref __TypeHandle.__Game_Net_GarageLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefFromEntity = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneDataFromEntity = InternalCompilerInterface.GetComponentLookup<ParkingLaneData>(ref __TypeHandle.__Game_Prefabs_ParkingLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLaneFromEntity = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjectFromEntity = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjectFromEntity = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_Results
		}, m_ParkingFacilityQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		m_ParkingCapacity.Update((float)m_Results[0]);
		m_ParkedCars.Update(m_Results[1]);
	}

	private void UpdateAvailability()
	{
		m_ParkingAvailability.Update(IndicatorValue.Calculate(m_ParkingCapacity.value, m_ParkedCars.value));
	}

	private void UpdateIncome()
	{
		m_ParkingIncome.Update(m_CityStatisticsSystem.GetStatisticValue(StatisticType.Income, 9));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public RoadsInfoviewUISystem()
	{
	}
}
