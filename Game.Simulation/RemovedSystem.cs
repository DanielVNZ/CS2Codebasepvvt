using System.Runtime.CompilerServices;
using Game.Agents;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Companies;
using Game.Notifications;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class RemovedSystem : GameSystemBase
{
	[BurstCompile]
	private struct RemovedPropertyJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentLookup<TouristHousehold> m_TouristHouseholds;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public ComponentLookup<LodgingProvider> m_LodgingProviders;

		[ReadOnly]
		public BufferLookup<Renter> m_RenterBufs;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				DynamicBuffer<Renter> val = m_RenterBufs[nativeArray[i]];
				for (int j = 0; j < val.Length; j++)
				{
					if (m_PropertyRenters.HasComponent(val[j].m_Renter))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<PropertyRenter>(unfilteredChunkIndex, val[j].m_Renter);
					}
					if (!m_LodgingProviders.HasComponent(val[j].m_Renter) || !m_RenterBufs.HasBuffer(val[j].m_Renter))
					{
						continue;
					}
					DynamicBuffer<Renter> val2 = m_RenterBufs[val[j].m_Renter];
					for (int num = val2.Length - 1; num >= 0; num--)
					{
						if (m_TouristHouseholds.HasComponent(val2[num].m_Renter))
						{
							TouristHousehold touristHousehold = m_TouristHouseholds[val2[num].m_Renter];
							touristHousehold.m_Hotel = Entity.Null;
							((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TouristHousehold>(unfilteredChunkIndex, val2[num].m_Renter, touristHousehold);
						}
					}
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val[j].m_Renter);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct RemovedWorkplaceJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<Employee> m_EmployeeType;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> m_Purposes;

		[ReadOnly]
		public ComponentLookup<Worker> m_Workers;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<FreeWorkplaces>(unfilteredChunkIndex, nativeArray[i]);
			}
			BufferAccessor<Employee> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Employee>(ref m_EmployeeType);
			for (int j = 0; j < bufferAccessor.Length; j++)
			{
				DynamicBuffer<Employee> val = bufferAccessor[j];
				for (int k = 0; k < val.Length; k++)
				{
					Entity worker = val[k].m_Worker;
					if (m_Purposes.HasComponent(worker) && (m_Purposes[worker].m_Purpose == Purpose.GoingToWork || m_Purposes[worker].m_Purpose == Purpose.Working))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, worker);
					}
					if (m_Workers.HasComponent(worker))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Worker>(unfilteredChunkIndex, worker);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct RemovedCompanyJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<CompanyNotifications> m_NotificationsType;

		public IconCommandBuffer m_IconCommandBuffer;

		public CompanyNotificationParameterData m_CompanyNotificationParameters;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CompanyNotifications> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CompanyNotifications>(ref m_NotificationsType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				CompanyNotifications companyNotifications = nativeArray[i];
				if (companyNotifications.m_NoCustomersEntity != default(Entity))
				{
					m_IconCommandBuffer.Remove(companyNotifications.m_NoCustomersEntity, m_CompanyNotificationParameters.m_NoCustomersNotificationPrefab);
				}
				if (companyNotifications.m_NoInputEntity != default(Entity))
				{
					m_IconCommandBuffer.Remove(companyNotifications.m_NoInputEntity, m_CompanyNotificationParameters.m_NoInputsNotificationPrefab);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct RentersUpdateJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<RentersUpdated> m_RentersUpdatedType;

		public BufferLookup<Renter> m_Renters;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> m_Parks;

		[ReadOnly]
		public ComponentLookup<Abandoned> m_Abandoneds;

		[ReadOnly]
		public ComponentLookup<MovingAway> m_MovingAways;

		[ReadOnly]
		public ComponentLookup<Deleted> m_Deleteds;

		public ComponentLookup<Building> m_Buildings;

		[ReadOnly]
		public BuildingConfigurationData m_BuildingConfigurationData;

		public IconCommandBuffer m_IconCommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<RentersUpdated> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<RentersUpdated>(ref m_RentersUpdatedType);
			DynamicBuffer<Renter> val = default(DynamicBuffer<Renter>);
			Building building = default(Building);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				RentersUpdated rentersUpdated = nativeArray[i];
				if (!m_Renters.TryGetBuffer(rentersUpdated.m_Property, ref val))
				{
					continue;
				}
				for (int num = val.Length - 1; num >= 0; num--)
				{
					if (m_MovingAways.HasComponent(val[num].m_Renter) || m_Deleteds.HasComponent(val[num].m_Renter))
					{
						val.RemoveAt(num);
					}
				}
				if (!BuildingUtils.IsHomelessShelterBuilding(rentersUpdated.m_Property, ref m_Parks, ref m_Abandoneds))
				{
					for (int num2 = val.Length - 1; num2 >= 0; num2--)
					{
						if (!m_PropertyRenters.HasComponent(val[num2].m_Renter) || m_PropertyRenters[val[num2].m_Renter].m_Property != rentersUpdated.m_Property)
						{
							val.RemoveAt(num2);
						}
					}
				}
				if (m_Buildings.TryGetComponent(rentersUpdated.m_Property, ref building) && (building.m_Flags & Game.Buildings.BuildingFlags.HighRentWarning) != Game.Buildings.BuildingFlags.None && val.Length == 0)
				{
					m_IconCommandBuffer.Remove(rentersUpdated.m_Property, m_BuildingConfigurationData.m_HighRentNotification);
					building.m_Flags &= ~Game.Buildings.BuildingFlags.HighRentWarning;
					m_Buildings[rentersUpdated.m_Property] = building;
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
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<TouristHousehold> __Game_Citizens_TouristHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LodgingProvider> __Game_Companies_LodgingProvider_RO_ComponentLookup;

		[ReadOnly]
		public BufferTypeHandle<Employee> __Game_Companies_Employee_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> __Game_Citizens_TravelPurpose_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Worker> __Game_Citizens_Worker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<CompanyNotifications> __Game_Companies_CompanyNotifications_RO_ComponentTypeHandle;

		public ComponentTypeHandle<RentersUpdated> __Game_Buildings_RentersUpdated_RW_ComponentTypeHandle;

		public BufferLookup<Renter> __Game_Buildings_Renter_RW_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Abandoned> __Game_Buildings_Abandoned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> __Game_Buildings_Park_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MovingAway> __Game_Agents_MovingAway_RO_ComponentLookup;

		public ComponentLookup<Building> __Game_Buildings_Building_RW_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Citizens_TouristHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TouristHousehold>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Companies_LodgingProvider_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LodgingProvider>(true);
			__Game_Companies_Employee_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Employee>(true);
			__Game_Citizens_TravelPurpose_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TravelPurpose>(true);
			__Game_Citizens_Worker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Worker>(true);
			__Game_Companies_CompanyNotifications_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CompanyNotifications>(true);
			__Game_Buildings_RentersUpdated_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RentersUpdated>(false);
			__Game_Buildings_Renter_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(false);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Buildings_Abandoned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Abandoned>(true);
			__Game_Buildings_Park_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Park>(true);
			__Game_Agents_MovingAway_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MovingAway>(true);
			__Game_Buildings_Building_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(false);
		}
	}

	private EntityQuery m_DeletedBuildings;

	private EntityQuery m_DeletedWorkplaces;

	private EntityQuery m_DeletedCompanies;

	private EntityQuery m_NeedUpdateRenterQuery;

	private EntityQuery m_BuildingParameterQuery;

	private EntityQuery m_CompanyNotificationParameterQuery;

	private IconCommandSystem m_IconCommandSystem;

	private ModificationBarrier5 m_ModificationBarrier;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_DeletedBuildings = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Renter>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_DeletedWorkplaces = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Employee>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_DeletedCompanies = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<CompanyNotifications>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_NeedUpdateRenterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Event>(),
			ComponentType.ReadOnly<RentersUpdated>()
		});
		m_BuildingParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BuildingConfigurationData>() });
		m_CompanyNotificationParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CompanyNotificationParameterData>() });
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		EntityCommandBuffer val2;
		if (!((EntityQuery)(ref m_DeletedBuildings)).IsEmptyIgnoreFilter)
		{
			RemovedPropertyJob removedPropertyJob = new RemovedPropertyJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RenterBufs = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TouristHouseholds = InternalCompilerInterface.GetComponentLookup<TouristHousehold>(ref __TypeHandle.__Game_Citizens_TouristHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LodgingProviders = InternalCompilerInterface.GetComponentLookup<LodgingProvider>(ref __TypeHandle.__Game_Companies_LodgingProvider_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			val2 = m_ModificationBarrier.CreateCommandBuffer();
			removedPropertyJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			val = JobChunkExtensions.ScheduleParallel<RemovedPropertyJob>(removedPropertyJob, m_DeletedBuildings, ((SystemBase)this).Dependency);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val);
		}
		JobHandle val3 = default(JobHandle);
		if (!((EntityQuery)(ref m_DeletedWorkplaces)).IsEmptyIgnoreFilter)
		{
			RemovedWorkplaceJob removedWorkplaceJob = new RemovedWorkplaceJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EmployeeType = InternalCompilerInterface.GetBufferTypeHandle<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Purposes = InternalCompilerInterface.GetComponentLookup<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Workers = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			val2 = m_ModificationBarrier.CreateCommandBuffer();
			removedWorkplaceJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			val3 = JobChunkExtensions.ScheduleParallel<RemovedWorkplaceJob>(removedWorkplaceJob, m_DeletedWorkplaces, ((SystemBase)this).Dependency);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val3);
		}
		JobHandle val4 = default(JobHandle);
		if (!((EntityQuery)(ref m_DeletedCompanies)).IsEmptyIgnoreFilter && !((EntityQuery)(ref m_CompanyNotificationParameterQuery)).IsEmptyIgnoreFilter)
		{
			val4 = JobChunkExtensions.ScheduleParallel<RemovedCompanyJob>(new RemovedCompanyJob
			{
				m_NotificationsType = InternalCompilerInterface.GetComponentTypeHandle<CompanyNotifications>(ref __TypeHandle.__Game_Companies_CompanyNotifications_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CompanyNotificationParameters = ((EntityQuery)(ref m_CompanyNotificationParameterQuery)).GetSingleton<CompanyNotificationParameterData>(),
				m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer()
			}, m_DeletedCompanies, ((SystemBase)this).Dependency);
			m_IconCommandSystem.AddCommandBufferWriter(val4);
		}
		JobHandle val5 = default(JobHandle);
		if (!((EntityQuery)(ref m_NeedUpdateRenterQuery)).IsEmptyIgnoreFilter)
		{
			val5 = JobChunkExtensions.Schedule<RentersUpdateJob>(new RentersUpdateJob
			{
				m_RentersUpdatedType = InternalCompilerInterface.GetComponentTypeHandle<RentersUpdated>(ref __TypeHandle.__Game_Buildings_RentersUpdated_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Renters = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Deleteds = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Abandoneds = InternalCompilerInterface.GetComponentLookup<Abandoned>(ref __TypeHandle.__Game_Buildings_Abandoned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Parks = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Park>(ref __TypeHandle.__Game_Buildings_Park_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MovingAways = InternalCompilerInterface.GetComponentLookup<MovingAway>(ref __TypeHandle.__Game_Agents_MovingAway_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Buildings = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingConfigurationData = ((EntityQuery)(ref m_BuildingParameterQuery)).GetSingleton<BuildingConfigurationData>(),
				m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer()
			}, m_NeedUpdateRenterQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
			m_IconCommandSystem.AddCommandBufferWriter(val5);
		}
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(val, val3, JobHandle.CombineDependencies(val4, val5));
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
	public RemovedSystem()
	{
	}
}
