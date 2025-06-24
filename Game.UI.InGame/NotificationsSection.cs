using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Companies;
using Game.Notifications;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class NotificationsSection : InfoSectionBase
{
	[BurstCompile]
	private struct CheckAndCacheNotificationsJob : IJob
	{
		[ReadOnly]
		public Entity m_Entity;

		[ReadOnly]
		public ComponentLookup<Citizen> m_CitizenDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Icon> m_IconDataFromEntity;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefDataFromEntity;

		[ReadOnly]
		public BufferLookup<IconElement> m_IconElementBufferFromEntity;

		[ReadOnly]
		public BufferLookup<Employee> m_EmployeeBufferFromEntity;

		[ReadOnly]
		public BufferLookup<Renter> m_RenterBufferFromEntity;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizenBufferFromEntity;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_RouteWaypointBufferFromEntity;

		public NativeArray<bool> m_DisplayResult;

		public NativeList<Notification> m_NotificationResult;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			if (m_IconDataFromEntity.HasComponent(m_Entity) || m_CitizenDataFromEntity.HasComponent(m_Entity))
			{
				return;
			}
			if (HasNotifications(m_Entity, m_IconElementBufferFromEntity, m_IconDataFromEntity))
			{
				m_DisplayResult[0] = true;
				m_NotificationResult = GetNotifications(m_Entity, m_PrefabRefDataFromEntity, m_IconDataFromEntity, m_IconElementBufferFromEntity, m_NotificationResult);
			}
			DynamicBuffer<Employee> val = default(DynamicBuffer<Employee>);
			if (m_EmployeeBufferFromEntity.TryGetBuffer(m_Entity, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					if (HasNotifications(val[i].m_Worker, m_IconElementBufferFromEntity, m_IconDataFromEntity))
					{
						m_DisplayResult[0] = true;
						m_NotificationResult = GetNotifications(val[i].m_Worker, m_PrefabRefDataFromEntity, m_IconDataFromEntity, m_IconElementBufferFromEntity, m_NotificationResult);
					}
				}
			}
			DynamicBuffer<Renter> val2 = default(DynamicBuffer<Renter>);
			if (m_RenterBufferFromEntity.TryGetBuffer(m_Entity, ref val2))
			{
				DynamicBuffer<HouseholdCitizen> val3 = default(DynamicBuffer<HouseholdCitizen>);
				DynamicBuffer<Employee> val4 = default(DynamicBuffer<Employee>);
				for (int j = 0; j < val2.Length; j++)
				{
					if (m_HouseholdCitizenBufferFromEntity.TryGetBuffer(val2[j].m_Renter, ref val3))
					{
						for (int k = 0; k < val3.Length; k++)
						{
							if (HasNotifications(val3[k].m_Citizen, m_IconElementBufferFromEntity, m_IconDataFromEntity))
							{
								m_DisplayResult[0] = true;
								m_NotificationResult = GetNotifications(val3[k].m_Citizen, m_PrefabRefDataFromEntity, m_IconDataFromEntity, m_IconElementBufferFromEntity, m_NotificationResult);
							}
						}
					}
					if (!m_EmployeeBufferFromEntity.TryGetBuffer(val2[j].m_Renter, ref val4))
					{
						continue;
					}
					for (int l = 0; l < val4.Length; l++)
					{
						if (HasNotifications(val4[l].m_Worker, m_IconElementBufferFromEntity, m_IconDataFromEntity))
						{
							m_DisplayResult[0] = true;
							m_NotificationResult = GetNotifications(val4[l].m_Worker, m_PrefabRefDataFromEntity, m_IconDataFromEntity, m_IconElementBufferFromEntity, m_NotificationResult);
						}
					}
				}
			}
			DynamicBuffer<RouteWaypoint> val5 = default(DynamicBuffer<RouteWaypoint>);
			if (!m_RouteWaypointBufferFromEntity.TryGetBuffer(m_Entity, ref val5))
			{
				return;
			}
			for (int m = 0; m < val5.Length; m++)
			{
				if (HasNotifications(val5[m].m_Waypoint, m_IconElementBufferFromEntity, m_IconDataFromEntity))
				{
					m_DisplayResult[0] = true;
					m_NotificationResult = GetNotifications(val5[m].m_Waypoint, m_PrefabRefDataFromEntity, m_IconDataFromEntity, m_IconElementBufferFromEntity, m_NotificationResult);
				}
			}
		}
	}

	[BurstCompile]
	private struct CheckAndCacheVisitorNotificationsJob : IJobChunk
	{
		[ReadOnly]
		public Entity m_Entity;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> m_CurrentBuildingTypeHandle;

		[ReadOnly]
		public BufferLookup<IconElement> m_IconElementBufferFromEntity;

		[ReadOnly]
		public ComponentLookup<Icon> m_IconDataFromEntity;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefDataFromEntity;

		public NativeArray<bool> m_DisplayResult;

		public NativeList<Notification> m_NotificationResult;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<CurrentBuilding> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentBuilding>(ref m_CurrentBuildingTypeHandle);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				if (nativeArray2[i].m_CurrentBuilding == m_Entity && HasNotifications(nativeArray[i], m_IconElementBufferFromEntity, m_IconDataFromEntity))
				{
					m_DisplayResult[0] = true;
					m_NotificationResult = GetNotifications(nativeArray[i], m_PrefabRefDataFromEntity, m_IconDataFromEntity, m_IconElementBufferFromEntity, m_NotificationResult);
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
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Icon> __Game_Notifications_Icon_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<IconElement> __Game_Notifications_IconElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Employee> __Game_Companies_Employee_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle;

		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RW_ComponentLookup;

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
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Notifications_Icon_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Icon>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Notifications_IconElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<IconElement>(true);
			__Game_Companies_Employee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Employee>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentBuilding>(true);
			__Game_Prefabs_PrefabRef_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(false);
		}
	}

	private ImageSystem m_ImageSystem;

	private EntityQuery m_CitizenQuery;

	private NativeList<Notification> m_NotificationsResult;

	private NativeArray<bool> m_DisplayResult;

	private TypeHandle __TypeHandle;

	protected override string group => "NotificationsSection";

	protected override bool displayForDestroyedObjects => true;

	protected override bool displayForOutsideConnections => true;

	protected override bool displayForUnderConstruction => true;

	protected override bool displayForUpgrades => true;

	private List<NotificationInfo> notifications { get; set; }

	protected override void Reset()
	{
		notifications.Clear();
		m_NotificationsResult.Clear();
		m_DisplayResult[0] = false;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ImageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ImageSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CurrentBuilding>(),
			ComponentType.ReadOnly<IconElement>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_CitizenQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_DisplayResult = new NativeArray<bool>(1, (Allocator)4, (NativeArrayOptions)1);
		m_NotificationsResult = new NativeList<Notification>(10, AllocatorHandle.op_Implicit((Allocator)4));
		notifications = new List<NotificationInfo>(10);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_NotificationsResult.Dispose();
		m_DisplayResult.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = IJobExtensions.Schedule<CheckAndCacheNotificationsJob>(new CheckAndCacheNotificationsJob
		{
			m_Entity = selectedEntity,
			m_CitizenDataFromEntity = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_IconDataFromEntity = InternalCompilerInterface.GetComponentLookup<Icon>(ref __TypeHandle.__Game_Notifications_Icon_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefDataFromEntity = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_IconElementBufferFromEntity = InternalCompilerInterface.GetBufferLookup<IconElement>(ref __TypeHandle.__Game_Notifications_IconElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EmployeeBufferFromEntity = InternalCompilerInterface.GetBufferLookup<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RenterBufferFromEntity = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizenBufferFromEntity = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteWaypointBufferFromEntity = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DisplayResult = m_DisplayResult,
			m_NotificationResult = m_NotificationsResult
		}, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		val = JobChunkExtensions.Schedule<CheckAndCacheVisitorNotificationsJob>(new CheckAndCacheVisitorNotificationsJob
		{
			m_Entity = selectedEntity,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentBuildingTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_IconElementBufferFromEntity = InternalCompilerInterface.GetBufferLookup<IconElement>(ref __TypeHandle.__Game_Notifications_IconElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_IconDataFromEntity = InternalCompilerInterface.GetComponentLookup<Icon>(ref __TypeHandle.__Game_Notifications_Icon_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefDataFromEntity = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DisplayResult = m_DisplayResult,
			m_NotificationResult = m_NotificationsResult
		}, m_CitizenQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		base.visible = m_DisplayResult[0];
	}

	protected override void OnProcess()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_NotificationsResult.Length; i++)
		{
			NotificationInfo notificationInfo = new NotificationInfo(m_NotificationsResult[i]);
			bool flag = false;
			for (int j = 0; j < notifications.Count; j++)
			{
				if (notifications[j].entity == notificationInfo.entity)
				{
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Building>(selectedEntity))
					{
						notifications[j].AddTarget(notificationInfo.target);
					}
					flag = true;
				}
			}
			if (!flag)
			{
				notifications.Add(notificationInfo);
			}
		}
		notifications.Sort();
	}

	public static bool HasNotifications(Entity entity, BufferLookup<IconElement> iconBuffer, ComponentLookup<Icon> iconDataFromEntity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (iconBuffer.HasBuffer(entity))
		{
			DynamicBuffer<IconElement> val = iconBuffer[entity];
			for (int i = 0; i < val.Length; i++)
			{
				Entity icon = val[i].m_Icon;
				if (iconDataFromEntity.HasComponent(icon) && iconDataFromEntity[icon].m_ClusterLayer != IconClusterLayer.Marker)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static NativeList<Notification> GetNotifications(EntityManager EntityManager, Entity entity, NativeList<Notification> notifications)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<IconElement> val = default(DynamicBuffer<IconElement>);
		if (EntitiesExtensions.TryGetBuffer<IconElement>(EntityManager, entity, true, ref val))
		{
			Icon icon2 = default(Icon);
			PrefabRef prefabRef = default(PrefabRef);
			for (int i = 0; i < val.Length; i++)
			{
				Entity icon = val[i].m_Icon;
				if (EntitiesExtensions.TryGetComponent<Icon>(EntityManager, icon, ref icon2) && icon2.m_ClusterLayer != IconClusterLayer.Marker && EntitiesExtensions.TryGetComponent<PrefabRef>(EntityManager, icon, ref prefabRef))
				{
					Notification notification = new Notification(prefabRef.m_Prefab, entity, icon2.m_Priority);
					notifications.Add(ref notification);
				}
			}
		}
		return notifications;
	}

	public static NativeList<Notification> GetNotifications(Entity entity, ComponentLookup<PrefabRef> prefabRefDataFromEntity, ComponentLookup<Icon> iconDataFromEntity, BufferLookup<IconElement> iconBufferFromEntity, NativeList<Notification> notifications)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (iconBufferFromEntity.HasBuffer(entity))
		{
			DynamicBuffer<IconElement> val = iconBufferFromEntity[entity];
			for (int i = 0; i < val.Length; i++)
			{
				Entity icon = val[i].m_Icon;
				if (iconDataFromEntity.HasComponent(icon))
				{
					Icon icon2 = iconDataFromEntity[icon];
					if (icon2.m_ClusterLayer != IconClusterLayer.Marker && prefabRefDataFromEntity.HasComponent(icon))
					{
						PrefabRef prefabRef = prefabRefDataFromEntity[icon];
						Notification notification = new Notification(prefabRef.m_Prefab, entity, icon2.m_Priority);
						notifications.Add(ref notification);
					}
				}
			}
		}
		return notifications;
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("notifications");
		JsonWriterExtensions.ArrayBegin(writer, notifications.Count);
		for (int i = 0; i < notifications.Count; i++)
		{
			Entity entity = notifications[i].entity;
			writer.TypeBegin(typeof(Notification).FullName);
			writer.PropertyName("key");
			writer.Write(m_PrefabSystem.GetPrefabName(entity));
			writer.PropertyName("count");
			writer.Write(notifications[i].count);
			writer.PropertyName("iconPath");
			if (m_PrefabSystem.TryGetPrefab<PrefabBase>(entity, out var prefab))
			{
				writer.Write(ImageSystem.GetIcon(prefab) ?? m_ImageSystem.placeholderIcon);
			}
			else
			{
				writer.Write(m_ImageSystem.placeholderIcon);
			}
			writer.TypeEnd();
		}
		writer.ArrayEnd();
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
	public NotificationsSection()
	{
	}
}
