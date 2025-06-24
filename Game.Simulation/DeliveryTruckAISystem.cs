using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
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
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class DeliveryTruckAISystem : GameSystemBase
{
	public struct DeliveredStack
	{
		public Entity vehicle;

		public Entity target;

		public Entity location;

		public Resource resource;

		public int amount;

		public Entity costPayer;

		public float distance;

		public bool storageTransfer;

		public bool moneyRefund;
	}

	public struct RemoveGuestVehicle
	{
		public Entity m_Vehicle;

		public Entity m_Target;
	}

	[CompilerGenerated]
	public class Actions : GameSystemBase
	{
		private struct TypeHandle
		{
			public BufferLookup<Resources> __Game_Economy_Resources_RW_BufferLookup;

			public ComponentLookup<Game.Vehicles.DeliveryTruck> __Game_Vehicles_DeliveryTruck_RW_ComponentLookup;

			public ComponentLookup<BuyingCompany> __Game_Companies_BuyingCompany_RW_ComponentLookup;

			public ComponentLookup<Game.Companies.StorageCompany> __Game_Companies_StorageCompany_RW_ComponentLookup;

			public ComponentLookup<Game.Buildings.CargoTransportStation> __Game_Buildings_CargoTransportStation_RW_ComponentLookup;

			public BufferLookup<StorageTransferRequest> __Game_Companies_StorageTransferRequest_RW_BufferLookup;

			[ReadOnly]
			public ComponentLookup<CompanyData> __Game_Companies_CompanyData_RO_ComponentLookup;

			[ReadOnly]
			public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

			[ReadOnly]
			public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

			public BufferLookup<GuestVehicle> __Game_Vehicles_GuestVehicle_RW_BufferLookup;

			[ReadOnly]
			public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

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
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				__Game_Economy_Resources_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(false);
				__Game_Vehicles_DeliveryTruck_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.DeliveryTruck>(false);
				__Game_Companies_BuyingCompany_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuyingCompany>(false);
				__Game_Companies_StorageCompany_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Companies.StorageCompany>(false);
				__Game_Buildings_CargoTransportStation_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.CargoTransportStation>(false);
				__Game_Companies_StorageTransferRequest_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<StorageTransferRequest>(false);
				__Game_Companies_CompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CompanyData>(true);
				__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
				__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
				__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
				__Game_Vehicles_GuestVehicle_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<GuestVehicle>(false);
				__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			}
		}

		private ResourceSystem m_ResourceSystem;

		private CityStatisticsSystem m_CityStatisticsSystem;

		private EntityQuery m_EconomyParameterQuery;

		public JobHandle m_Dependency;

		public NativeQueue<DeliveredStack> m_DeliveredQueue;

		public NativeQueue<RemoveGuestVehicle> m_RemoveGuestVehicleQueue;

		private TypeHandle __TypeHandle;

		[Preserve]
		protected override void OnCreate()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			base.OnCreate();
			m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
			m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
			m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		}

		[Preserve]
		protected override void OnUpdate()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			JobHandle val = JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_Dependency);
			JobHandle deps;
			JobHandle val2 = IJobExtensions.Schedule<DeliverJob>(new DeliverJob
			{
				m_DeliveredQueue = m_DeliveredQueue,
				m_RemoveGuestVehicleQueue = m_RemoveGuestVehicleQueue,
				m_Resources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DeliveryTrucks = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuyingCompanies = InternalCompilerInterface.GetComponentLookup<BuyingCompany>(ref __TypeHandle.__Game_Companies_BuyingCompany_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StorageCompanies = InternalCompilerInterface.GetComponentLookup<Game.Companies.StorageCompany>(ref __TypeHandle.__Game_Companies_StorageCompany_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CargoTransportStationData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.CargoTransportStation>(ref __TypeHandle.__Game_Buildings_CargoTransportStation_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StorageTransferRequests = InternalCompilerInterface.GetBufferLookup<StorageTransferRequest>(ref __TypeHandle.__Game_Companies_StorageTransferRequest_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Companies = InternalCompilerInterface.GetComponentLookup<CompanyData>(ref __TypeHandle.__Game_Companies_CompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Renters = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Owners = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Controllers = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_GuestVehicles = InternalCompilerInterface.GetBufferLookup<GuestVehicle>(ref __TypeHandle.__Game_Vehicles_GuestVehicle_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
				m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StatisticsEventQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps).AsParallelWriter(),
				m_EconomyParameters = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>()
			}, JobHandle.CombineDependencies(val, deps));
			m_DeliveredQueue.Dispose(val2);
			m_RemoveGuestVehicleQueue.Dispose(val2);
			m_ResourceSystem.AddPrefabsReader(val2);
			m_CityStatisticsSystem.AddWriter(val2);
			((SystemBase)this).Dependency = val2;
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
		public Actions()
		{
		}
	}

	[BurstCompile]
	private struct DeliverJob : IJob
	{
		public NativeQueue<DeliveredStack> m_DeliveredQueue;

		public NativeQueue<RemoveGuestVehicle> m_RemoveGuestVehicleQueue;

		public BufferLookup<Resources> m_Resources;

		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_DeliveryTrucks;

		public ComponentLookup<BuyingCompany> m_BuyingCompanies;

		public ComponentLookup<Game.Companies.StorageCompany> m_StorageCompanies;

		public ComponentLookup<Game.Buildings.CargoTransportStation> m_CargoTransportStationData;

		[ReadOnly]
		public BufferLookup<Renter> m_Renters;

		[ReadOnly]
		public ComponentLookup<CompanyData> m_Companies;

		[ReadOnly]
		public ComponentLookup<Owner> m_Owners;

		[ReadOnly]
		public ComponentLookup<Controller> m_Controllers;

		public BufferLookup<StorageTransferRequest> m_StorageTransferRequests;

		public BufferLookup<GuestVehicle> m_GuestVehicles;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		public EconomyParameterData m_EconomyParameters;

		public ParallelWriter<StatisticsEvent> m_StatisticsEventQueue;

		private Entity FindCompany(DynamicBuffer<Renter> renters)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < renters.Length; i++)
			{
				if (m_Companies.HasComponent(renters[i].m_Renter))
				{
					return renters[i].m_Renter;
				}
			}
			return Entity.Null;
		}

		private void AddWork(Entity target, int workAmount)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			if (workAmount <= 0)
			{
				return;
			}
			Owner owner = default(Owner);
			Game.Buildings.CargoTransportStation cargoTransportStation = default(Game.Buildings.CargoTransportStation);
			while (m_Owners.TryGetComponent(target, ref owner))
			{
				target = owner.m_Owner;
				if (m_CargoTransportStationData.TryGetComponent(target, ref cargoTransportStation))
				{
					cargoTransportStation.m_WorkAmount += workAmount;
					m_CargoTransportStationData[target] = cargoTransportStation;
					break;
				}
			}
		}

		public void Execute()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0594: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_054c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_047d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			DeliveredStack deliveredStack = default(DeliveredStack);
			while (m_DeliveredQueue.TryDequeue(ref deliveredStack))
			{
				if (m_ResourcePrefabs[deliveredStack.resource] == Entity.Null)
				{
					continue;
				}
				int num = Mathf.RoundToInt((float)deliveredStack.amount * EconomyUtils.GetIndustrialPrice(deliveredStack.resource, m_ResourcePrefabs, ref m_ResourceDatas));
				float weight = EconomyUtils.GetWeight(deliveredStack.resource, m_ResourcePrefabs, ref m_ResourceDatas);
				int num2 = Mathf.RoundToInt((float)EconomyUtils.GetTransportCost(deliveredStack.distance, deliveredStack.resource, deliveredStack.amount, weight));
				if (!m_Resources.HasBuffer(deliveredStack.target) && m_Renters.HasBuffer(deliveredStack.target))
				{
					deliveredStack.target = FindCompany(m_Renters[deliveredStack.target]);
				}
				if (!m_Resources.HasBuffer(deliveredStack.costPayer) && m_Renters.HasBuffer(deliveredStack.costPayer))
				{
					deliveredStack.costPayer = FindCompany(m_Renters[deliveredStack.costPayer]);
				}
				if (!m_Resources.HasBuffer(deliveredStack.target))
				{
					if (deliveredStack.moneyRefund && m_Resources.HasBuffer(deliveredStack.costPayer) && !m_StorageCompanies.HasComponent(deliveredStack.costPayer))
					{
						EconomyUtils.AddResources(Resource.Money, num, m_Resources[deliveredStack.costPayer]);
					}
					continue;
				}
				DynamicBuffer<Resources> resources = m_Resources[deliveredStack.target];
				if (deliveredStack.amount < 0)
				{
					int num3 = math.min(-deliveredStack.amount, EconomyUtils.GetResources(deliveredStack.resource, resources));
					Game.Vehicles.DeliveryTruck deliveryTruck = m_DeliveryTrucks[deliveredStack.vehicle];
					deliveryTruck.m_Amount += num3;
					m_DeliveryTrucks[deliveredStack.vehicle] = deliveryTruck;
					EconomyUtils.AddResources(deliveredStack.resource, -num3, resources);
				}
				else
				{
					if (!deliveredStack.moneyRefund)
					{
						EconomyUtils.AddResources(deliveredStack.resource, deliveredStack.amount, resources);
						m_StatisticsEventQueue.Enqueue(new StatisticsEvent
						{
							m_Statistic = StatisticType.CargoCountTruck,
							m_Change = deliveredStack.amount
						});
					}
					if (m_Resources.HasBuffer(deliveredStack.costPayer))
					{
						if (deliveredStack.moneyRefund)
						{
							if (!m_StorageCompanies.HasComponent(deliveredStack.costPayer))
							{
								EconomyUtils.AddResources(Resource.Money, num, m_Resources[deliveredStack.costPayer]);
							}
							if (!m_StorageCompanies.HasComponent(deliveredStack.target))
							{
								EconomyUtils.AddResources(Resource.Money, -num, m_Resources[deliveredStack.target]);
							}
						}
						else if (deliveredStack.storageTransfer)
						{
							if (!m_StorageCompanies.HasComponent(deliveredStack.costPayer))
							{
								EconomyUtils.AddResources(Resource.Money, -num2, m_Resources[deliveredStack.costPayer]);
							}
							if (m_BuyingCompanies.HasComponent(deliveredStack.costPayer))
							{
								BuyingCompany buyingCompany = m_BuyingCompanies[deliveredStack.costPayer];
								if (buyingCompany.m_MeanInputTripLength > 0f)
								{
									buyingCompany.m_MeanInputTripLength = math.lerp(buyingCompany.m_MeanInputTripLength, deliveredStack.distance, 0.5f);
								}
								else
								{
									buyingCompany.m_MeanInputTripLength = deliveredStack.distance;
								}
								m_BuyingCompanies[deliveredStack.costPayer] = buyingCompany;
							}
						}
						else
						{
							if (!m_StorageCompanies.HasComponent(deliveredStack.costPayer))
							{
								EconomyUtils.AddResources(Resource.Money, num, m_Resources[deliveredStack.costPayer]);
							}
							if (!m_StorageCompanies.HasComponent(deliveredStack.target))
							{
								EconomyUtils.AddResources(Resource.Money, -num, m_Resources[deliveredStack.target]);
							}
						}
					}
					if (deliveredStack.amount > 0 && m_StorageCompanies.HasComponent(deliveredStack.target) && m_StorageTransferRequests.HasBuffer(deliveredStack.target) && (m_Owners.HasComponent(deliveredStack.vehicle) || (m_Controllers.HasComponent(deliveredStack.vehicle) && m_Owners.HasComponent(m_Controllers[deliveredStack.vehicle].m_Controller))))
					{
						Entity val = (m_Controllers.HasComponent(deliveredStack.vehicle) ? m_Controllers[deliveredStack.vehicle].m_Controller : deliveredStack.vehicle);
						Entity owner = m_Owners[val].m_Owner;
						DynamicBuffer<StorageTransferRequest> val2 = m_StorageTransferRequests[deliveredStack.target];
						for (int i = 0; i < val2.Length; i++)
						{
							StorageTransferRequest storageTransferRequest = val2[i];
							if ((storageTransferRequest.m_Flags & StorageTransferFlags.Incoming) != 0 && storageTransferRequest.m_Target == owner && storageTransferRequest.m_Resource == deliveredStack.resource)
							{
								if (storageTransferRequest.m_Amount > deliveredStack.amount)
								{
									storageTransferRequest.m_Amount -= deliveredStack.amount;
									val2[i] = storageTransferRequest;
									break;
								}
								deliveredStack.amount -= storageTransferRequest.m_Amount;
								val2.RemoveAtSwapBack(i);
								i--;
							}
						}
					}
				}
				AddWork(deliveredStack.location, math.abs(deliveredStack.amount));
			}
			RemoveGuestVehicle removeGuestVehicle = default(RemoveGuestVehicle);
			while (m_RemoveGuestVehicleQueue.TryDequeue(ref removeGuestVehicle))
			{
				if (m_GuestVehicles.HasBuffer(removeGuestVehicle.m_Target))
				{
					CollectionUtils.RemoveValue<GuestVehicle>(m_GuestVehicles[removeGuestVehicle.m_Target], new GuestVehicle(removeGuestVehicle.m_Vehicle));
				}
			}
		}
	}

	[BurstCompile]
	private struct DeliveryTruckTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> m_UnspawnedType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> m_PathInformationType;

		[ReadOnly]
		public BufferTypeHandle<LayoutElement> m_LayoutElementType;

		public ComponentTypeHandle<Car> m_CarType;

		public ComponentTypeHandle<CarCurrentLane> m_CurrentLaneType;

		public ComponentTypeHandle<Target> m_TargetType;

		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Quantity> m_QuantityData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<CarData> m_PrefabCarData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyData;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_DeliveryTruckData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<ReturnLoad> m_ReturnLoadData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<PathElement> m_PathElements;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public ParallelWriter<DeliveredStack> m_DeliveredQueue;

		public ParallelWriter<RemoveGuestVehicle> m_RemoveGuestVehicleQueue;

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
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Car> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Car>(ref m_CarType);
			NativeArray<CarCurrentLane> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarCurrentLane>(ref m_CurrentLaneType);
			NativeArray<Target> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			NativeArray<PathOwner> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			NativeArray<PathInformation> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathInformation>(ref m_PathInformationType);
			BufferAccessor<LayoutElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LayoutElement>(ref m_LayoutElementType);
			bool isUnspawned = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
			Owner owner = default(Owner);
			DynamicBuffer<LayoutElement> layout = default(DynamicBuffer<LayoutElement>);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				PrefabRef prefabRef = nativeArray3[i];
				PathInformation pathInformation = nativeArray8[i];
				ref Car car = ref CollectionUtils.ElementAt<Car>(nativeArray4, i);
				ref CarCurrentLane reference = ref CollectionUtils.ElementAt<CarCurrentLane>(nativeArray5, i);
				ref PathOwner pathOwner = ref CollectionUtils.ElementAt<PathOwner>(nativeArray7, i);
				ref Target target = ref CollectionUtils.ElementAt<Target>(nativeArray6, i);
				ref Game.Vehicles.DeliveryTruck valueRW = ref m_DeliveryTruckData.GetRefRW(val).ValueRW;
				CollectionUtils.TryGet<Owner>(nativeArray2, i, ref owner);
				CollectionUtils.TryGet<LayoutElement>(bufferAccessor, i, ref layout);
				VehicleUtils.CheckUnspawned(unfilteredChunkIndex, val, reference, isUnspawned, m_CommandBuffer);
				Tick(unfilteredChunkIndex, val, owner, prefabRef, pathInformation, layout, ref valueRW, ref car, ref reference, ref pathOwner, ref target);
			}
		}

		private void CancelTransaction(int jobIndex, Entity vehicleEntity, ref Game.Vehicles.DeliveryTruck deliveryTruck, DynamicBuffer<LayoutElement> layout, PathInformation pathInformation, Owner owner)
		{
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			if ((deliveryTruck.m_State & DeliveryTruckFlags.TransactionCancelled) != 0)
			{
				return;
			}
			if (layout.IsCreated && layout.Length != 0)
			{
				for (int i = 0; i < layout.Length; i++)
				{
					Entity vehicle = layout[i].m_Vehicle;
					if (!(vehicle != vehicleEntity) || !m_DeliveryTruckData.HasComponent(vehicle))
					{
						continue;
					}
					Game.Vehicles.DeliveryTruck deliveryTruck2 = m_DeliveryTruckData[vehicle];
					if ((deliveryTruck2.m_State & DeliveryTruckFlags.Loaded) != 0 && deliveryTruck2.m_Amount > 0)
					{
						if ((deliveryTruck.m_State & DeliveryTruckFlags.Returning) != 0)
						{
							DeliveredStack deliveredStack = new DeliveredStack
							{
								vehicle = vehicle,
								target = owner.m_Owner,
								resource = deliveryTruck2.m_Resource,
								amount = deliveryTruck2.m_Amount,
								costPayer = owner.m_Owner,
								distance = pathInformation.m_Distance
							};
							m_DeliveredQueue.Enqueue(deliveredStack);
							if (deliveredStack.amount != 0)
							{
								TargetQuantityUpdated(jobIndex, deliveredStack.target);
							}
							continue;
						}
						DeliveredStack deliveredStack2 = new DeliveredStack
						{
							vehicle = vehicle,
							target = pathInformation.m_Origin,
							resource = deliveryTruck2.m_Resource,
							amount = deliveryTruck2.m_Amount,
							costPayer = pathInformation.m_Destination,
							distance = 0f,
							storageTransfer = ((deliveryTruck.m_State & DeliveryTruckFlags.StorageTransfer) != 0)
						};
						m_DeliveredQueue.Enqueue(deliveredStack2);
						if (deliveredStack2.amount != 0)
						{
							TargetQuantityUpdated(jobIndex, deliveredStack2.target);
						}
					}
					else if ((deliveryTruck2.m_State & DeliveryTruckFlags.Loaded) == 0 && (deliveryTruck.m_State & DeliveryTruckFlags.Returning) == 0)
					{
						DeliveredStack deliveredStack3 = new DeliveredStack
						{
							vehicle = vehicle,
							target = pathInformation.m_Destination,
							resource = deliveryTruck2.m_Resource,
							amount = deliveryTruck2.m_Amount,
							distance = 0f,
							costPayer = owner.m_Owner,
							moneyRefund = true
						};
						m_DeliveredQueue.Enqueue(deliveredStack3);
						if (deliveredStack3.amount < 0)
						{
							TargetQuantityUpdated(jobIndex, deliveredStack3.target);
						}
					}
				}
			}
			else if ((deliveryTruck.m_State & DeliveryTruckFlags.Loaded) != 0 && deliveryTruck.m_Amount > 0)
			{
				if ((deliveryTruck.m_State & DeliveryTruckFlags.Returning) != 0)
				{
					DeliveredStack deliveredStack4 = new DeliveredStack
					{
						vehicle = vehicleEntity,
						target = owner.m_Owner,
						resource = deliveryTruck.m_Resource,
						amount = deliveryTruck.m_Amount,
						costPayer = owner.m_Owner,
						distance = pathInformation.m_Distance
					};
					m_DeliveredQueue.Enqueue(deliveredStack4);
					if (deliveredStack4.amount != 0)
					{
						TargetQuantityUpdated(jobIndex, deliveredStack4.target);
					}
				}
				else
				{
					DeliveredStack deliveredStack5 = new DeliveredStack
					{
						vehicle = vehicleEntity,
						target = pathInformation.m_Origin,
						resource = deliveryTruck.m_Resource,
						amount = deliveryTruck.m_Amount,
						costPayer = pathInformation.m_Destination,
						storageTransfer = ((deliveryTruck.m_State & DeliveryTruckFlags.StorageTransfer) != 0)
					};
					m_DeliveredQueue.Enqueue(deliveredStack5);
					if (deliveredStack5.amount != 0)
					{
						TargetQuantityUpdated(jobIndex, deliveredStack5.target);
					}
				}
			}
			else if ((deliveryTruck.m_State & DeliveryTruckFlags.Loaded) == 0 && (deliveryTruck.m_State & DeliveryTruckFlags.Returning) == 0)
			{
				DeliveredStack deliveredStack6 = new DeliveredStack
				{
					vehicle = vehicleEntity,
					target = pathInformation.m_Destination,
					resource = deliveryTruck.m_Resource,
					amount = deliveryTruck.m_Amount,
					distance = 0f,
					costPayer = owner.m_Owner,
					moneyRefund = true
				};
				m_DeliveredQueue.Enqueue(deliveredStack6);
				if (deliveredStack6.amount < 0)
				{
					TargetQuantityUpdated(jobIndex, deliveredStack6.target);
				}
			}
		}

		private void Tick(int jobIndex, Entity vehicleEntity, Owner owner, PrefabRef prefabRef, PathInformation pathInformation, DynamicBuffer<LayoutElement> layout, ref Game.Vehicles.DeliveryTruck deliveryTruck, ref Car car, ref CarCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			if (VehicleUtils.ResetUpdatedPath(ref pathOwner))
			{
				DynamicBuffer<PathElement> path = m_PathElements[vehicleEntity];
				PathUtils.ResetPath(ref currentLane, path, m_SlaveLaneData, m_OwnerData, m_SubLanes);
				if (layout.IsCreated && layout.Length >= 2)
				{
					car.m_Flags |= CarFlags.CannotReverse;
				}
				else
				{
					car.m_Flags &= ~CarFlags.CannotReverse;
				}
				if ((deliveryTruck.m_State & DeliveryTruckFlags.UpdateOwnerQuantity) != 0)
				{
					deliveryTruck.m_State &= ~DeliveryTruckFlags.UpdateOwnerQuantity;
					TargetQuantityUpdated(jobIndex, owner.m_Owner);
				}
			}
			if (!m_PrefabRefData.HasComponent(target.m_Target) || VehicleUtils.PathfindFailed(pathOwner))
			{
				if ((deliveryTruck.m_State & DeliveryTruckFlags.DummyTraffic) != 0)
				{
					m_RemoveGuestVehicleQueue.Enqueue(new RemoveGuestVehicle
					{
						m_Vehicle = vehicleEntity,
						m_Target = pathInformation.m_Destination
					});
					VehicleUtils.DeleteVehicle(m_CommandBuffer, jobIndex, vehicleEntity, layout);
					return;
				}
				if (VehicleUtils.IsStuck(pathOwner) || (deliveryTruck.m_State & DeliveryTruckFlags.Returning) != 0)
				{
					if (VehicleUtils.PathfindFailed(pathOwner) || VehicleUtils.IsStuck(pathOwner))
					{
						CancelTransaction(jobIndex, vehicleEntity, ref deliveryTruck, layout, pathInformation, owner);
						deliveryTruck.m_State |= DeliveryTruckFlags.TransactionCancelled;
					}
					m_RemoveGuestVehicleQueue.Enqueue(new RemoveGuestVehicle
					{
						m_Vehicle = vehicleEntity,
						m_Target = pathInformation.m_Destination
					});
					VehicleUtils.DeleteVehicle(m_CommandBuffer, jobIndex, vehicleEntity, layout);
					return;
				}
				if (VehicleUtils.PathfindFailed(pathOwner) || VehicleUtils.IsStuck(pathOwner))
				{
					CancelTransaction(jobIndex, vehicleEntity, ref deliveryTruck, layout, pathInformation, owner);
					deliveryTruck.m_State |= DeliveryTruckFlags.TransactionCancelled;
				}
				deliveryTruck.m_State |= DeliveryTruckFlags.Returning;
				m_RemoveGuestVehicleQueue.Enqueue(new RemoveGuestVehicle
				{
					m_Vehicle = vehicleEntity,
					m_Target = pathInformation.m_Destination
				});
				VehicleUtils.SetTarget(ref pathOwner, ref target, owner.m_Owner);
			}
			else if (VehicleUtils.PathEndReached(currentLane))
			{
				if ((deliveryTruck.m_State & DeliveryTruckFlags.DummyTraffic) != 0)
				{
					m_RemoveGuestVehicleQueue.Enqueue(new RemoveGuestVehicle
					{
						m_Vehicle = vehicleEntity,
						m_Target = pathInformation.m_Destination
					});
					VehicleUtils.DeleteVehicle(m_CommandBuffer, jobIndex, vehicleEntity, layout);
					return;
				}
				DeliverCargo(jobIndex, vehicleEntity, owner.m_Owner, pathInformation, layout, ref deliveryTruck, ref currentLane);
				if ((deliveryTruck.m_State & DeliveryTruckFlags.Returning) != 0 || !m_PrefabRefData.HasComponent(owner.m_Owner))
				{
					m_RemoveGuestVehicleQueue.Enqueue(new RemoveGuestVehicle
					{
						m_Vehicle = vehicleEntity,
						m_Target = pathInformation.m_Destination
					});
					VehicleUtils.DeleteVehicle(m_CommandBuffer, jobIndex, vehicleEntity, layout);
					return;
				}
				deliveryTruck.m_State |= DeliveryTruckFlags.Returning;
				m_RemoveGuestVehicleQueue.Enqueue(new RemoveGuestVehicle
				{
					m_Vehicle = vehicleEntity,
					m_Target = pathInformation.m_Destination
				});
				VehicleUtils.SetTarget(ref pathOwner, ref target, owner.m_Owner);
			}
			FindPathIfNeeded(vehicleEntity, prefabRef, ref currentLane, ref pathOwner, ref target);
		}

		private void DeliverCargo(int jobIndex, Entity truck, Entity truckOwner, PathInformation pathInformation, DynamicBuffer<LayoutElement> layout, ref Game.Vehicles.DeliveryTruck truckDelivery, ref CarCurrentLane currentLane)
		{
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0512: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			bool resourceChanged = false;
			bool flag = false;
			if (layout.IsCreated && layout.Length != 0)
			{
				for (int i = 0; i < layout.Length; i++)
				{
					Entity vehicle = layout[i].m_Vehicle;
					if (!(vehicle != truck) || !m_DeliveryTruckData.HasComponent(vehicle))
					{
						continue;
					}
					Game.Vehicles.DeliveryTruck deliveryTruck = m_DeliveryTruckData[vehicle];
					if ((deliveryTruck.m_State & DeliveryTruckFlags.Loaded) != 0 && deliveryTruck.m_Amount > 0 && (truckDelivery.m_State & DeliveryTruckFlags.NoUnloading) == 0)
					{
						DeliveredStack deliveredStack = new DeliveredStack
						{
							vehicle = vehicle,
							target = pathInformation.m_Destination,
							location = currentLane.m_Lane,
							resource = deliveryTruck.m_Resource,
							amount = deliveryTruck.m_Amount,
							costPayer = truckOwner,
							distance = pathInformation.m_Distance
						};
						m_DeliveredQueue.Enqueue(deliveredStack);
						flag |= deliveredStack.amount != 0;
					}
					deliveryTruck.m_State ^= DeliveryTruckFlags.Loaded;
					if ((deliveryTruck.m_State & DeliveryTruckFlags.Loaded) == 0 && (truckDelivery.m_State & DeliveryTruckFlags.Returning) == 0 && m_ReturnLoadData.HasComponent(vehicle))
					{
						ReturnLoad returnLoad = m_ReturnLoadData[vehicle];
						if (returnLoad.m_Amount > 0)
						{
							DeliveredStack deliveredStack2 = new DeliveredStack
							{
								vehicle = vehicle,
								target = pathInformation.m_Destination,
								location = currentLane.m_Lane,
								resource = returnLoad.m_Resource,
								amount = -returnLoad.m_Amount,
								costPayer = truckOwner,
								distance = pathInformation.m_Distance
							};
							m_DeliveredQueue.Enqueue(deliveredStack2);
							flag |= deliveredStack2.amount != 0;
							deliveryTruck.m_State |= DeliveryTruckFlags.Loaded;
							deliveryTruck.m_Resource = returnLoad.m_Resource;
							deliveryTruck.m_Amount = 0;
							resourceChanged = true;
						}
						m_ReturnLoadData[vehicle] = default(ReturnLoad);
					}
					m_DeliveryTruckData[vehicle] = deliveryTruck;
					QuantityUpdated(jobIndex, vehicle, resourceChanged);
				}
			}
			if ((truckDelivery.m_State & DeliveryTruckFlags.Loaded) != 0 && truckDelivery.m_Amount > 0)
			{
				DeliveredStack deliveredStack3 = new DeliveredStack
				{
					vehicle = truck,
					target = pathInformation.m_Destination,
					location = currentLane.m_Lane,
					resource = truckDelivery.m_Resource,
					amount = truckDelivery.m_Amount,
					costPayer = truckOwner,
					distance = pathInformation.m_Distance
				};
				m_DeliveredQueue.Enqueue(deliveredStack3);
				flag |= deliveredStack3.amount != 0;
			}
			if ((truckDelivery.m_State & DeliveryTruckFlags.UpdateSellerQuantity) != 0)
			{
				truckDelivery.m_State &= ~DeliveryTruckFlags.UpdateSellerQuantity;
				int amount = truckDelivery.m_Amount;
				truckDelivery.m_Amount = 0;
				DeliveredStack deliveredStack4 = new DeliveredStack
				{
					vehicle = truck,
					target = pathInformation.m_Destination,
					location = currentLane.m_Lane,
					resource = truckDelivery.m_Resource,
					amount = -amount,
					costPayer = truckOwner,
					distance = pathInformation.m_Distance
				};
				m_DeliveredQueue.Enqueue(deliveredStack4);
				flag |= deliveredStack4.amount != 0;
				if (layout.IsCreated && layout.Length != 0)
				{
					for (int j = 0; j < layout.Length; j++)
					{
						Entity vehicle2 = layout[j].m_Vehicle;
						if (vehicle2 != truck && m_DeliveryTruckData.HasComponent(vehicle2))
						{
							Game.Vehicles.DeliveryTruck deliveryTruck2 = m_DeliveryTruckData[vehicle2];
							int amount2 = deliveryTruck2.m_Amount;
							deliveryTruck2.m_Amount = 0;
							m_DeliveryTruckData[vehicle2] = deliveryTruck2;
							DeliveredStack deliveredStack5 = new DeliveredStack
							{
								vehicle = vehicle2,
								target = pathInformation.m_Destination,
								location = currentLane.m_Lane,
								resource = deliveryTruck2.m_Resource,
								amount = -amount2,
								costPayer = truckOwner,
								distance = pathInformation.m_Distance
							};
							m_DeliveredQueue.Enqueue(deliveredStack5);
							flag |= deliveredStack5.amount != 0;
						}
					}
				}
			}
			truckDelivery.m_State ^= DeliveryTruckFlags.Loaded;
			resourceChanged = false;
			if ((truckDelivery.m_State & (DeliveryTruckFlags.Returning | DeliveryTruckFlags.Loaded)) == 0 && m_ReturnLoadData.HasComponent(truck))
			{
				ReturnLoad returnLoad2 = m_ReturnLoadData[truck];
				if (returnLoad2.m_Amount > 0)
				{
					DeliveredStack deliveredStack6 = new DeliveredStack
					{
						vehicle = truck,
						target = pathInformation.m_Destination,
						location = currentLane.m_Lane,
						resource = returnLoad2.m_Resource,
						amount = -returnLoad2.m_Amount,
						costPayer = truckOwner,
						distance = pathInformation.m_Distance
					};
					m_DeliveredQueue.Enqueue(deliveredStack6);
					flag |= deliveredStack6.amount != 0;
					truckDelivery.m_State |= DeliveryTruckFlags.Loaded;
					truckDelivery.m_Resource = returnLoad2.m_Resource;
					truckDelivery.m_Amount = 0;
					resourceChanged = true;
				}
				m_ReturnLoadData[truck] = default(ReturnLoad);
			}
			if (resourceChanged)
			{
				truckDelivery.m_State |= DeliveryTruckFlags.Buying;
			}
			QuantityUpdated(jobIndex, truck, resourceChanged);
			if (flag)
			{
				TargetQuantityUpdated(jobIndex, pathInformation.m_Destination);
			}
		}

		private void QuantityUpdated(int jobIndex, Entity vehicleEntity, bool resourceChanged)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			if (!m_SubObjects.HasBuffer(vehicleEntity))
			{
				return;
			}
			if (resourceChanged)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, vehicleEntity, default(Updated));
				return;
			}
			DynamicBuffer<Game.Objects.SubObject> val = m_SubObjects[vehicleEntity];
			for (int i = 0; i < val.Length; i++)
			{
				Entity subObject = val[i].m_SubObject;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, subObject, default(BatchesUpdated));
			}
		}

		private void TargetQuantityUpdated(int jobIndex, Entity buildingEntity, bool updateAll = false)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			PropertyRenter propertyRenter = default(PropertyRenter);
			if (m_PropertyData.TryGetComponent(buildingEntity, ref propertyRenter))
			{
				buildingEntity = propertyRenter.m_Property;
			}
			DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
			if (!m_SubObjects.TryGetBuffer(buildingEntity, ref val))
			{
				return;
			}
			for (int i = 0; i < val.Length; i++)
			{
				Entity subObject = val[i].m_SubObject;
				bool updateAll2 = false;
				if (updateAll || m_QuantityData.HasComponent(subObject))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, subObject, default(BatchesUpdated));
					updateAll2 = true;
				}
				TargetQuantityUpdated(jobIndex, subObject, updateAll2);
			}
		}

		private void FindPathIfNeeded(Entity vehicleEntity, PrefabRef prefabRef, ref CarCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			if (VehicleUtils.RequireNewPath(pathOwner))
			{
				CarData carData = m_PrefabCarData[prefabRef.m_Prefab];
				PathfindParameters parameters = new PathfindParameters
				{
					m_MaxSpeed = float2.op_Implicit(carData.m_MaxSpeed),
					m_WalkSpeed = float2.op_Implicit(5.555556f),
					m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
					m_Methods = (PathMethod.Road | PathMethod.CargoLoading),
					m_IgnoredRules = VehicleUtils.GetIgnoredPathfindRules(carData)
				};
				SetupQueueTarget origin = new SetupQueueTarget
				{
					m_Type = SetupTargetType.CurrentLocation,
					m_Methods = (PathMethod.Road | PathMethod.CargoLoading),
					m_RoadTypes = RoadTypes.Car,
					m_RandomCost = 30f
				};
				SetupQueueTarget destination = new SetupQueueTarget
				{
					m_Type = SetupTargetType.CurrentLocation,
					m_Methods = (PathMethod.Road | PathMethod.CargoLoading),
					m_RoadTypes = RoadTypes.Car,
					m_Entity = target.m_Target,
					m_RandomCost = 30f
				};
				VehicleUtils.SetupPathfind(item: new SetupQueueItem(vehicleEntity, parameters, origin, destination), currentLane: ref currentLane, pathOwner: ref pathOwner, queue: m_PathfindQueue);
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
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> __Game_Objects_Unspawned_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferTypeHandle;

		public ComponentTypeHandle<Car> __Game_Vehicles_Car_RW_ComponentTypeHandle;

		public ComponentTypeHandle<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Target> __Game_Common_Target_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Quantity> __Game_Objects_Quantity_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarData> __Game_Prefabs_CarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		public ComponentLookup<Game.Vehicles.DeliveryTruck> __Game_Vehicles_DeliveryTruck_RW_ComponentLookup;

		public ComponentLookup<ReturnLoad> __Game_Vehicles_ReturnLoad_RW_ComponentLookup;

		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RW_BufferLookup;

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
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Objects_Unspawned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unspawned>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Pathfind_PathInformation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathInformation>(true);
			__Game_Vehicles_LayoutElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LayoutElement>(true);
			__Game_Vehicles_Car_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Car>(false);
			__Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarCurrentLane>(false);
			__Game_Common_Target_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(false);
			__Game_Pathfind_PathOwner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(false);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Quantity_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Quantity>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Prefabs_CarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Vehicles_DeliveryTruck_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.DeliveryTruck>(false);
			__Game_Vehicles_ReturnLoad_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ReturnLoad>(false);
			__Game_Pathfind_PathElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(false);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private Actions m_Actions;

	private EntityQuery m_DeliveryTruckQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_Actions = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Actions>();
		m_DeliveryTruckQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadWrite<Game.Vehicles.DeliveryTruck>(),
			ComponentType.ReadOnly<CarCurrentLane>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<TripSource>(),
			ComponentType.Exclude<OutOfControl>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		uint index = m_SimulationSystem.frameIndex % 16;
		((EntityQuery)(ref m_DeliveryTruckQuery)).ResetFilter();
		((EntityQuery)(ref m_DeliveryTruckQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(index));
		m_Actions.m_DeliveredQueue = new NativeQueue<DeliveredStack>(AllocatorHandle.op_Implicit((Allocator)3));
		m_Actions.m_RemoveGuestVehicleQueue = new NativeQueue<RemoveGuestVehicle>(AllocatorHandle.op_Implicit((Allocator)3));
		DeliveryTruckTickJob deliveryTruckTickJob = new DeliveryTruckTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationType = InternalCompilerInterface.GetComponentTypeHandle<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElementType = InternalCompilerInterface.GetBufferTypeHandle<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarType = InternalCompilerInterface.GetComponentTypeHandle<Car>(ref __TypeHandle.__Game_Vehicles_Car_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_QuantityData = InternalCompilerInterface.GetComponentLookup<Quantity>(ref __TypeHandle.__Game_Objects_Quantity_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarData = InternalCompilerInterface.GetComponentLookup<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyData = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeliveryTruckData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ReturnLoadData = InternalCompilerInterface.GetComponentLookup<ReturnLoad>(ref __TypeHandle.__Game_Vehicles_ReturnLoad_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		deliveryTruckTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		deliveryTruckTickJob.m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter();
		deliveryTruckTickJob.m_DeliveredQueue = m_Actions.m_DeliveredQueue.AsParallelWriter();
		deliveryTruckTickJob.m_RemoveGuestVehicleQueue = m_Actions.m_RemoveGuestVehicleQueue.AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<DeliveryTruckTickJob>(deliveryTruckTickJob, m_DeliveryTruckQuery, ((SystemBase)this).Dependency);
		m_PathfindSetupSystem.AddQueueWriter(val2);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		m_Actions.m_Dependency = val2;
		((SystemBase)this).Dependency = val2;
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
	public DeliveryTruckAISystem()
	{
	}
}
