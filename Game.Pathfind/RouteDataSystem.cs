using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Pathfind;

[CompilerGenerated]
public class RouteDataSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateRouteDataJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<Game.Routes.TransportStop> m_TransportStopType;

		public ComponentTypeHandle<Game.Routes.TakeoffLocation> m_TakeoffLocationType;

		public ComponentTypeHandle<Game.Objects.SpawnLocation> m_SpawnLocationType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Attachment> m_AttachmentData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> m_PrefabRouteConnectionData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_PrefabSpawnLocationData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
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
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Game.Routes.TransportStop> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Routes.TransportStop>(ref m_TransportStopType);
			NativeArray<Game.Routes.TakeoffLocation> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Routes.TakeoffLocation>(ref m_TakeoffLocationType);
			NativeArray<Game.Objects.SpawnLocation> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Objects.SpawnLocation>(ref m_SpawnLocationType);
			NativeArray<Owner> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			if (nativeArray.Length != 0)
			{
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Game.Routes.TransportStop transportStop = nativeArray[i];
					transportStop.m_AccessRestriction = Entity.Null;
					transportStop.m_Flags &= ~StopFlags.AllowEnter;
					if (nativeArray4.Length != 0)
					{
						Owner owner = nativeArray4[i];
						PrefabRef prefabRef = nativeArray5[i];
						RouteConnectionData routeConnectionData = m_PrefabRouteConnectionData[prefabRef.m_Prefab];
						Game.Prefabs.BuildingFlags flag = GetRestrictFlag(routeConnectionData.m_AccessConnectionType, routeConnectionData.m_AccessRoadType) | GetRestrictFlag(routeConnectionData.m_RouteConnectionType, routeConnectionData.m_RouteRoadType);
						transportStop.m_AccessRestriction = GetAccessRestriction(owner, flag, isTakeOffLocation: false, out var allowEnter, out var _);
						if (allowEnter)
						{
							transportStop.m_Flags |= StopFlags.AllowEnter;
						}
					}
					nativeArray[i] = transportStop;
				}
			}
			if (nativeArray3.Length != 0)
			{
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					Game.Objects.SpawnLocation spawnLocation = nativeArray3[j];
					spawnLocation.m_AccessRestriction = Entity.Null;
					spawnLocation.m_Flags &= ~(SpawnLocationFlags.AllowEnter | SpawnLocationFlags.AllowExit);
					if (nativeArray4.Length != 0)
					{
						Owner owner2 = nativeArray4[j];
						PrefabRef prefabRef2 = nativeArray5[j];
						SpawnLocationData spawnLocationData = m_PrefabSpawnLocationData[prefabRef2.m_Prefab];
						Game.Prefabs.BuildingFlags restrictFlag = GetRestrictFlag(spawnLocationData.m_ConnectionType, spawnLocationData.m_RoadTypes);
						spawnLocation.m_AccessRestriction = GetAccessRestriction(owner2, restrictFlag, isTakeOffLocation: false, out var allowEnter2, out var allowExit2);
						if (allowEnter2)
						{
							spawnLocation.m_Flags |= SpawnLocationFlags.AllowEnter;
						}
						if (allowExit2)
						{
							spawnLocation.m_Flags |= SpawnLocationFlags.AllowExit;
						}
					}
					nativeArray3[j] = spawnLocation;
				}
			}
			if (nativeArray2.Length == 0)
			{
				return;
			}
			for (int k = 0; k < nativeArray2.Length; k++)
			{
				Game.Routes.TakeoffLocation takeoffLocation = nativeArray2[k];
				takeoffLocation.m_AccessRestriction = Entity.Null;
				takeoffLocation.m_Flags &= ~(TakeoffLocationFlags.AllowEnter | TakeoffLocationFlags.AllowExit);
				if (nativeArray3.Length != 0)
				{
					Game.Objects.SpawnLocation spawnLocation2 = nativeArray3[k];
					takeoffLocation.m_AccessRestriction = spawnLocation2.m_AccessRestriction;
					if ((spawnLocation2.m_Flags & SpawnLocationFlags.AllowEnter) != 0)
					{
						takeoffLocation.m_Flags |= TakeoffLocationFlags.AllowEnter;
					}
					if ((spawnLocation2.m_Flags & SpawnLocationFlags.AllowExit) != 0)
					{
						takeoffLocation.m_Flags |= TakeoffLocationFlags.AllowExit;
					}
				}
				else if (nativeArray4.Length != 0)
				{
					Owner owner3 = nativeArray4[k];
					PrefabRef prefabRef3 = nativeArray5[k];
					RouteConnectionData routeConnectionData2 = m_PrefabRouteConnectionData[prefabRef3.m_Prefab];
					Game.Prefabs.BuildingFlags flag2 = GetRestrictFlag(routeConnectionData2.m_AccessConnectionType, routeConnectionData2.m_AccessRoadType) | GetRestrictFlag(routeConnectionData2.m_RouteConnectionType, routeConnectionData2.m_RouteRoadType);
					takeoffLocation.m_AccessRestriction = GetAccessRestriction(owner3, flag2, isTakeOffLocation: true, out var allowEnter3, out var allowExit3);
					if (allowEnter3)
					{
						takeoffLocation.m_Flags |= TakeoffLocationFlags.AllowEnter;
					}
					if (allowExit3)
					{
						takeoffLocation.m_Flags |= TakeoffLocationFlags.AllowExit;
					}
				}
				nativeArray2[k] = takeoffLocation;
			}
		}

		private Game.Prefabs.BuildingFlags GetRestrictFlag(RouteConnectionType routeConnectionType, RoadTypes routeRoadType)
		{
			switch (routeConnectionType)
			{
			case RouteConnectionType.Road:
				if ((routeRoadType & (RoadTypes.Car | RoadTypes.Helicopter | RoadTypes.Airplane)) == 0)
				{
					return (Game.Prefabs.BuildingFlags)0u;
				}
				return Game.Prefabs.BuildingFlags.RestrictedCar;
			case RouteConnectionType.Pedestrian:
				return Game.Prefabs.BuildingFlags.RestrictedPedestrian;
			case RouteConnectionType.Cargo:
				return Game.Prefabs.BuildingFlags.RestrictedCar;
			case RouteConnectionType.Parking:
				return Game.Prefabs.BuildingFlags.RestrictedPedestrian | Game.Prefabs.BuildingFlags.RestrictedCar;
			case RouteConnectionType.Air:
				return Game.Prefabs.BuildingFlags.RestrictedCar;
			case RouteConnectionType.Track:
				return Game.Prefabs.BuildingFlags.RestrictedTrack;
			default:
				return (Game.Prefabs.BuildingFlags)0u;
			}
		}

		private Entity GetAccessRestriction(Owner owner, Game.Prefabs.BuildingFlags flag, bool isTakeOffLocation, out bool allowEnter, out bool allowExit)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			Entity val = owner.m_Owner;
			while (m_OwnerData.TryGetComponent(val, ref owner))
			{
				val = owner.m_Owner;
			}
			Attachment attachment = default(Attachment);
			if (m_AttachmentData.TryGetComponent(val, ref attachment) && attachment.m_Attached != Entity.Null)
			{
				val = attachment.m_Attached;
			}
			if (m_BuildingData.HasComponent(val))
			{
				PrefabRef prefabRef = m_PrefabRefData[val];
				BuildingData buildingData = m_PrefabBuildingData[prefabRef.m_Prefab];
				bool flag2 = (buildingData.m_Flags & flag) != 0;
				bool flag3 = (flag & Game.Prefabs.BuildingFlags.RestrictedCar) != 0;
				bool flag4 = (flag & Game.Prefabs.BuildingFlags.RestrictedPedestrian) != 0;
				if (flag2 || flag3 || flag4)
				{
					allowEnter = !flag2;
					if (flag3 && flag4)
					{
						allowExit = (buildingData.m_Flags & Game.Prefabs.BuildingFlags.RestrictedParking) == 0;
					}
					else if (flag4)
					{
						if (allowEnter && (buildingData.m_Flags & Game.Prefabs.BuildingFlags.RestrictedCar) != 0)
						{
							allowEnter &= isTakeOffLocation;
							allowExit = allowEnter;
						}
						else
						{
							allowExit = false;
						}
					}
					else
					{
						allowExit = false;
					}
					return val;
				}
			}
			allowEnter = false;
			allowExit = false;
			return Entity.Null;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Routes.TransportStop> __Game_Routes_TransportStop_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Routes.TakeoffLocation> __Game_Routes_TakeoffLocation_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attachment> __Game_Objects_Attachment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> __Game_Prefabs_RouteConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> __Game_Prefabs_SpawnLocationData_RO_ComponentLookup;

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
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Routes_TransportStop_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Routes.TransportStop>(false);
			__Game_Routes_TakeoffLocation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Routes.TakeoffLocation>(false);
			__Game_Objects_SpawnLocation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.SpawnLocation>(false);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Attachment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attachment>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_RouteConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteConnectionData>(true);
			__Game_Prefabs_SpawnLocationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocationData>(true);
		}
	}

	private EntityQuery m_UpdateQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Updated>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Routes.TransportStop>(),
			ComponentType.ReadOnly<Game.Routes.TakeoffLocation>(),
			ComponentType.ReadOnly<Game.Objects.SpawnLocation>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Game.Routes.MailBox>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PathfindUpdated>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Routes.TransportStop>(),
			ComponentType.ReadOnly<Game.Routes.TakeoffLocation>(),
			ComponentType.ReadOnly<Game.Objects.SpawnLocation>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Game.Routes.MailBox>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[1] = val;
		m_UpdateQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_UpdateQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		UpdateRouteDataJob updateRouteDataJob = new UpdateRouteDataJob
		{
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransportStopType = InternalCompilerInterface.GetComponentTypeHandle<Game.Routes.TransportStop>(ref __TypeHandle.__Game_Routes_TransportStop_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TakeoffLocationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Routes.TakeoffLocation>(ref __TypeHandle.__Game_Routes_TakeoffLocation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachmentData = InternalCompilerInterface.GetComponentLookup<Attachment>(ref __TypeHandle.__Game_Objects_Attachment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRouteConnectionData = InternalCompilerInterface.GetComponentLookup<RouteConnectionData>(ref __TypeHandle.__Game_Prefabs_RouteConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<UpdateRouteDataJob>(updateRouteDataJob, m_UpdateQuery, ((SystemBase)this).Dependency);
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
	public RouteDataSystem()
	{
	}
}
