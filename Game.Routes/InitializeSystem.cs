using System.Runtime.CompilerServices;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Prefabs;
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

namespace Game.Routes;

[CompilerGenerated]
public class InitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct AssignRouteNumbersJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_RouteChunks;

		[ReadOnly]
		public ComponentTypeHandle<Created> m_CreatedType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<RouteNumber> m_RouteNumberType;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_RouteChunks.Length; i++)
			{
				ArchetypeChunk val = m_RouteChunks[i];
				if (((ArchetypeChunk)(ref val)).Has<Created>(ref m_CreatedType))
				{
					NativeArray<RouteNumber> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<RouteNumber>(ref m_RouteNumberType);
					NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
					for (int j = 0; j < nativeArray.Length; j++)
					{
						RouteNumber routeNumber = nativeArray[j];
						routeNumber.m_Number = FindFreeRouteNumber(nativeArray2[j].m_Prefab);
						nativeArray[j] = routeNumber;
					}
				}
			}
		}

		private int FindFreeRouteNumber(Entity prefab)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < m_RouteChunks.Length; i++)
			{
				ArchetypeChunk val = m_RouteChunks[i];
				if (!((ArchetypeChunk)(ref val)).Has<Created>(ref m_CreatedType))
				{
					NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
					for (int j = 0; j < nativeArray.Length; j++)
					{
						num += math.select(0, 1, nativeArray[j].m_Prefab == prefab);
					}
				}
			}
			if (num > 0)
			{
				NativeBitArray val2 = default(NativeBitArray);
				((NativeBitArray)(ref val2))._002Ector(num + 1, AllocatorHandle.op_Implicit((Allocator)2), (NativeArrayOptions)1);
				for (int k = 0; k < m_RouteChunks.Length; k++)
				{
					ArchetypeChunk val3 = m_RouteChunks[k];
					if (((ArchetypeChunk)(ref val3)).Has<Created>(ref m_CreatedType))
					{
						continue;
					}
					NativeArray<RouteNumber> nativeArray2 = ((ArchetypeChunk)(ref val3)).GetNativeArray<RouteNumber>(ref m_RouteNumberType);
					NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref val3)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
					for (int l = 0; l < nativeArray3.Length; l++)
					{
						if (nativeArray3[l].m_Prefab == prefab)
						{
							RouteNumber routeNumber = nativeArray2[l];
							if (routeNumber.m_Number <= num)
							{
								((NativeBitArray)(ref val2)).Set(routeNumber.m_Number, true);
							}
						}
					}
				}
				for (int m = 1; m <= num; m++)
				{
					if (!((NativeBitArray)(ref val2)).IsSet(m))
					{
						return m;
					}
				}
				((NativeBitArray)(ref val2)).Dispose();
			}
			return num + 1;
		}
	}

	[BurstCompile]
	private struct SelectVehicleJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<VehicleModel> m_VehicleModelType;

		[ReadOnly]
		public ComponentLookup<TransportLineData> m_PrefabTransportLineData;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public TransportVehicleSelectData m_TransportVehicleSelectData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<VehicleModel> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<VehicleModel>(ref m_VehicleModelType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			TransportLineData transportLineData = default(TransportLineData);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				VehicleModel vehicleModel = nativeArray[i];
				PrefabRef prefabRef = nativeArray2[i];
				if (m_PrefabTransportLineData.TryGetComponent(prefabRef.m_Prefab, ref transportLineData))
				{
					PublicTransportPurpose publicTransportPurpose = (transportLineData.m_PassengerTransport ? PublicTransportPurpose.TransportLine : ((PublicTransportPurpose)0));
					Resource cargoResources = (Resource)(transportLineData.m_CargoTransport ? 8 : 0);
					int2 passengerCapacity = (int2)(transportLineData.m_PassengerTransport ? new int2(1, int.MaxValue) : int2.op_Implicit(0));
					int2 cargoCapacity = (int2)(transportLineData.m_CargoTransport ? new int2(1, int.MaxValue) : int2.op_Implicit(0));
					m_TransportVehicleSelectData.SelectVehicle(ref random, transportLineData.m_TransportType, EnergyTypes.FuelAndElectricity, transportLineData.m_SizeClass, publicTransportPurpose, cargoResources, out vehicleModel.m_PrimaryPrefab, out vehicleModel.m_SecondaryPrefab, ref passengerCapacity, ref cargoCapacity);
				}
				nativeArray[i] = vehicleModel;
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
		public ComponentTypeHandle<Created> __Game_Common_Created_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<RouteNumber> __Game_Routes_RouteNumber_RW_ComponentTypeHandle;

		public ComponentTypeHandle<VehicleModel> __Game_Routes_VehicleModel_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<TransportLineData> __Game_Prefabs_TransportLineData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			__Game_Common_Created_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Created>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Routes_RouteNumber_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RouteNumber>(false);
			__Game_Routes_VehicleModel_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<VehicleModel>(false);
			__Game_Prefabs_TransportLineData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportLineData>(true);
		}
	}

	private CityConfigurationSystem m_CityConfigurationSystem;

	private EntityQuery m_CreatedQuery;

	private EntityQuery m_RouteQuery;

	private EntityQuery m_VehiclePrefabQuery;

	private TransportVehicleSelectData m_TransportVehicleSelectData;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_TransportVehicleSelectData = new TransportVehicleSelectData((SystemBase)(object)this);
		m_CreatedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Route>(),
			ComponentType.ReadOnly<RouteNumber>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.Exclude<Temp>()
		});
		m_RouteQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Route>(),
			ComponentType.ReadOnly<RouteNumber>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_VehiclePrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { TransportVehicleSelectData.GetEntityQueryDesc() });
		((ComponentSystemBase)this).RequireForUpdate(m_CreatedQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> routeChunks = ((EntityQuery)(ref m_RouteQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		m_TransportVehicleSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_VehiclePrefabQuery, (Allocator)3, out var jobHandle);
		AssignRouteNumbersJob assignRouteNumbersJob = new AssignRouteNumbersJob
		{
			m_RouteChunks = routeChunks,
			m_CreatedType = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RouteNumberType = InternalCompilerInterface.GetComponentTypeHandle<RouteNumber>(ref __TypeHandle.__Game_Routes_RouteNumber_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		SelectVehicleJob obj = new SelectVehicleJob
		{
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleModelType = InternalCompilerInterface.GetComponentTypeHandle<VehicleModel>(ref __TypeHandle.__Game_Routes_VehicleModel_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTransportLineData = InternalCompilerInterface.GetComponentLookup<TransportLineData>(ref __TypeHandle.__Game_Prefabs_TransportLineData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_TransportVehicleSelectData = m_TransportVehicleSelectData
		};
		JobHandle val2 = IJobExtensions.Schedule<AssignRouteNumbersJob>(assignRouteNumbersJob, JobHandle.CombineDependencies(val, ((SystemBase)this).Dependency));
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<SelectVehicleJob>(obj, m_CreatedQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, jobHandle));
		m_TransportVehicleSelectData.PostUpdate(val3);
		routeChunks.Dispose(val2);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(val2, val3);
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
	public InitializeSystem()
	{
	}
}
