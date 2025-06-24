using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Notifications;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Simulation;
using Game.Tools;
using Game.Triggers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Events;

[CompilerGenerated]
public class InitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct RandomEventTargetJob : IJob
	{
		[DeallocateOnJobCompletion]
		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_TargetChunks;

		[ReadOnly]
		public EventTargetType m_TargetType;

		[ReadOnly]
		public TransportType m_TransportType;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		public NativeValue<Entity> m_Result;

		[ReadOnly]
		public EntityTypeHandle m_EntitiesType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.TransportDepot> m_TransportDepotType;

		[ReadOnly]
		public ComponentTypeHandle<Tree> m_TreeType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Road> m_RoadType;

		[ReadOnly]
		public ComponentTypeHandle<Edge> m_EdgeType;

		[ReadOnly]
		public ComponentTypeHandle<Citizen> m_CitizenType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<TransportDepotData> m_PrefabTransportDepotData;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_RandomSeed.GetRandom(0);
			if (FindRandomTarget(ref random, m_TargetType, m_TransportType, out var target))
			{
				m_Result.value = target;
			}
		}

		private bool FindRandomTarget(ref Random random, EventTargetType type, TransportType transportType, out Entity target)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			int totalCount = 0;
			target = Entity.Null;
			for (int i = 0; i < m_TargetChunks.Length; i++)
			{
				ArchetypeChunk chunk = m_TargetChunks[i];
				switch (type)
				{
				case EventTargetType.Building:
					if (!((ArchetypeChunk)(ref chunk)).Has<Building>(ref m_BuildingType))
					{
						continue;
					}
					break;
				case EventTargetType.WildTree:
					if (!((ArchetypeChunk)(ref chunk)).Has<Tree>(ref m_TreeType) || ((ArchetypeChunk)(ref chunk)).Has<Owner>(ref m_OwnerType))
					{
						continue;
					}
					break;
				case EventTargetType.Road:
					if (!((ArchetypeChunk)(ref chunk)).Has<Road>(ref m_RoadType) || !((ArchetypeChunk)(ref chunk)).Has<Edge>(ref m_EdgeType))
					{
						continue;
					}
					break;
				case EventTargetType.Citizen:
					if (!((ArchetypeChunk)(ref chunk)).Has<Citizen>(ref m_CitizenType))
					{
						continue;
					}
					break;
				case EventTargetType.TransportDepot:
					if (!((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.TransportDepot>(ref m_TransportDepotType))
					{
						continue;
					}
					break;
				default:
					continue;
				}
				if (transportType != TransportType.None)
				{
					CheckDepotType(ref random, ref totalCount, ref target, chunk, transportType);
					continue;
				}
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntitiesType);
				int num = ((Random)(ref random)).NextInt(-totalCount, nativeArray.Length);
				if (num >= 0)
				{
					target = nativeArray[num];
				}
				totalCount += nativeArray.Length;
			}
			return target != Entity.Null;
		}

		private void CheckDepotType(ref Random random, ref int totalCount, ref Entity target, ArchetypeChunk chunk, TransportType transportType)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntitiesType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				PrefabRef prefabRef = nativeArray2[i];
				if (m_PrefabTransportDepotData[prefabRef.m_Prefab].m_TransportType == transportType)
				{
					if (((Random)(ref random)).NextInt(-totalCount, 1) >= 0)
					{
						target = nativeArray[i];
					}
					totalCount++;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.TransportDepot> __Game_Buildings_TransportDepot_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Tree> __Game_Objects_Tree_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Road> __Game_Net_Road_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Edge> __Game_Net_Edge_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Citizen> __Game_Citizens_Citizen_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<TransportDepotData> __Game_Prefabs_TransportDepotData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
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
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_Buildings_TransportDepot_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.TransportDepot>(true);
			__Game_Objects_Tree_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Tree>(true);
			__Game_Net_Road_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Road>(true);
			__Game_Net_Edge_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Edge>(true);
			__Game_Citizens_Citizen_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Citizen>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Prefabs_TransportDepotData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportDepotData>(true);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private CitySystem m_CitySystem;

	private IconCommandSystem m_IconCommandSystem;

	private EntityQuery m_EventQuery;

	private EntityQuery m_InstanceQuery;

	private EntityQuery m_DisasterConfigQuery;

	private EntityQuery m_TargetQuery;

	private EntityQuery m_EDWSBuildingQuery;

	private EntityArchetype m_IgniteEventArchetype;

	private EntityArchetype m_ImpactEventArchetype;

	private EntityArchetype m_AccidentSiteEventArchetype;

	private EntityArchetype m_HealthEventArchetype;

	private EntityArchetype m_DamageEventArchetype;

	private EntityArchetype m_DestroyEventArchetype;

	private EntityArchetype m_SpectateEventArchetype;

	private EntityArchetype m_CriminalEventArchetype;

	private TriggerSystem m_TriggerSystem;

	private EntityCommandBuffer m_CommandBuffer;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_EventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Event>()
		});
		m_InstanceQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Event>(),
			ComponentType.Exclude<Deleted>()
		});
		m_DisasterConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<DisasterConfigurationData>() });
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Tree>(),
			ComponentType.ReadOnly<Road>(),
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.ReadOnly<Household>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_TargetQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_EDWSBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Buildings.EarlyDisasterWarningSystem>() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_IgniteEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<Ignite>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_ImpactEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<Impact>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_AccidentSiteEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<AddAccidentSite>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_HealthEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<AddHealthProblem>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_DamageEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<Damage>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_DestroyEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<Destroy>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_SpectateEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<Spectate>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_CriminalEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<AddCriminal>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_EventQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_EventQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			NativeQueue<TriggerAction> val2 = m_TriggerSystem.CreateActionBuffer();
			DynamicBuffer<TargetElement> val4 = default(DynamicBuffer<TargetElement>);
			for (int i = 0; i < val.Length; i++)
			{
				Entity val3 = val[i];
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(val3);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				EventData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<EventData>(componentData.m_Prefab);
				if (componentData2.m_ConcurrentLimit > 0 && CountInstances(componentData.m_Prefab) > componentData2.m_ConcurrentLimit)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).AddComponent<Deleted>(val3);
					continue;
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Fire>(val3))
				{
					InitializeFire(val3);
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<TrafficAccident>(val3))
				{
					InitializeTrafficAccident(val3);
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<WeatherPhenomenon>(val3))
				{
					InitializeWeatherEvent(val3);
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<HealthEvent>(val3))
				{
					InitializeHealthEvent(val3);
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Destruction>(val3))
				{
					InitializeDestruction(val3);
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<SpectatorEvent>(val3))
				{
					InitializeSpectatorEvent(val3);
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Crime>(val3))
				{
					InitializeCrimeEvent(val3);
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<WaterLevelChange>(val3))
				{
					InitializeWaterLevelChangeEvent(val3);
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<CalendarEvent>(val3))
				{
					InitializeCalendarEvent(val3);
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<CoordinatedMeeting>(val3))
				{
					InitializeMeetingEvent(val3);
				}
				if (EntitiesExtensions.TryGetBuffer<TargetElement>(((ComponentSystemBase)this).EntityManager, val3, true, ref val4) && val4.Length > 0)
				{
					for (int j = 0; j < val4.Length; j++)
					{
						val2.Enqueue(new TriggerAction
						{
							m_TriggerPrefab = componentData.m_Prefab,
							m_PrimaryTarget = val4[j].m_Entity,
							m_SecondaryTarget = Entity.Null,
							m_TriggerType = TriggerType.EventHappened
						});
					}
				}
				else
				{
					val2.Enqueue(new TriggerAction
					{
						m_TriggerPrefab = componentData.m_Prefab,
						m_PrimaryTarget = Entity.Null,
						m_SecondaryTarget = Entity.Null,
						m_TriggerType = TriggerType.EventHappened
					});
				}
			}
		}
		finally
		{
			val.Dispose();
		}
		if (((EntityCommandBuffer)(ref m_CommandBuffer)).IsCreated)
		{
			((EntityCommandBuffer)(ref m_CommandBuffer)).Playback(((ComponentSystemBase)this).EntityManager);
			((EntityCommandBuffer)(ref m_CommandBuffer)).Dispose();
		}
	}

	private int CountInstances(Entity prefab)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		ComponentTypeHandle<PrefabRef> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_InstanceQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)2));
		((SystemBase)this).CompleteDependency();
		int num = 0;
		for (int i = 0; i < val.Length; i++)
		{
			ArchetypeChunk val2 = val[i];
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabRef>(ref componentTypeHandle);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				if (nativeArray[j].m_Prefab == prefab)
				{
					num++;
				}
			}
		}
		val.Dispose();
		return num;
	}

	private EntityCommandBuffer GetCommandBuffer()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityCommandBuffer)(ref m_CommandBuffer)).IsCreated)
		{
			m_CommandBuffer = new EntityCommandBuffer(AllocatorHandle.op_Implicit((Allocator)3));
		}
		return m_CommandBuffer;
	}

	private void InitializeFire(Entity eventEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(eventEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		FireData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<FireData>(componentData.m_Prefab);
		if (componentData2.m_RandomTargetType != EventTargetType.None)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<TargetElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TargetElement>(eventEntity, false);
			if (buffer.Length == 0)
			{
				AddRandomTarget(buffer, componentData2.m_RandomTargetType, TransportType.None);
			}
			EntityCommandBuffer commandBuffer = GetCommandBuffer();
			for (int i = 0; i < buffer.Length; i++)
			{
				Entity entity = buffer[i].m_Entity;
				Entity val = ((EntityCommandBuffer)(ref commandBuffer)).CreateEntity(m_IgniteEventArchetype);
				((EntityCommandBuffer)(ref commandBuffer)).SetComponent<Ignite>(val, new Ignite
				{
					m_Event = eventEntity,
					m_Target = entity,
					m_Intensity = componentData2.m_StartIntensity
				});
			}
		}
	}

	private void InitializeTrafficAccident(Entity eventEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(eventEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		TrafficAccidentData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<TrafficAccidentData>(componentData.m_Prefab);
		if (componentData2.m_RandomSiteType == EventTargetType.None)
		{
			return;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<TargetElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TargetElement>(eventEntity, false);
		if (buffer.Length == 0)
		{
			AddRandomTarget(buffer, componentData2.m_RandomSiteType, TransportType.None);
		}
		Random random = RandomSeed.Next().GetRandom(eventEntity.Index);
		EntityCommandBuffer commandBuffer = GetCommandBuffer();
		Moving moving = default(Moving);
		Road road = default(Road);
		for (int i = 0; i < buffer.Length; i++)
		{
			Entity entity = buffer[i].m_Entity;
			if (EntitiesExtensions.TryGetComponent<Moving>(((ComponentSystemBase)this).EntityManager, entity, ref moving))
			{
				Impact impact = new Impact
				{
					m_Event = eventEntity,
					m_Target = entity,
					m_Severity = 5f
				};
				if (((Random)(ref random)).NextBool())
				{
					impact.m_AngularVelocityDelta.y = -2f;
					((float3)(ref impact.m_VelocityDelta)).xz = impact.m_Severity * MathUtils.Left(math.normalizesafe(((float3)(ref moving.m_Velocity)).xz, default(float2)));
				}
				else
				{
					impact.m_AngularVelocityDelta.y = 2f;
					((float3)(ref impact.m_VelocityDelta)).xz = impact.m_Severity * MathUtils.Right(math.normalizesafe(((float3)(ref moving.m_Velocity)).xz, default(float2)));
				}
				Entity val = ((EntityCommandBuffer)(ref commandBuffer)).CreateEntity(m_ImpactEventArchetype);
				((EntityCommandBuffer)(ref commandBuffer)).SetComponent<Impact>(val, impact);
			}
			else if (EntitiesExtensions.TryGetComponent<Road>(((ComponentSystemBase)this).EntityManager, entity, ref road))
			{
				AddAccidentSite addAccidentSite = new AddAccidentSite
				{
					m_Event = eventEntity,
					m_Target = entity,
					m_Flags = (AccidentSiteFlags.StageAccident | AccidentSiteFlags.TrafficAccident)
				};
				Entity val2 = ((EntityCommandBuffer)(ref commandBuffer)).CreateEntity(m_AccidentSiteEventArchetype);
				((EntityCommandBuffer)(ref commandBuffer)).SetComponent<AddAccidentSite>(val2, addAccidentSite);
			}
		}
	}

	private void InitializeWeatherEvent(Entity eventEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		WeatherPhenomenon componentData = ((EntityManager)(ref entityManager)).GetComponentData<WeatherPhenomenon>(eventEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		Duration componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<Duration>(eventEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<TargetElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TargetElement>(eventEntity, false);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		PrefabRef componentData3 = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(eventEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		WeatherPhenomenonData componentData4 = ((EntityManager)(ref entityManager)).GetComponentData<WeatherPhenomenonData>(componentData3.m_Prefab);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<HotspotFrame> buffer2 = ((EntityManager)(ref entityManager)).GetBuffer<HotspotFrame>(eventEntity, false);
		Random random = RandomSeed.Next().GetRandom(eventEntity.Index);
		if (componentData.m_PhenomenonRadius == 0f)
		{
			componentData.m_PhenomenonRadius = ((Random)(ref random)).NextFloat(componentData4.m_PhenomenonRadius.min, componentData4.m_PhenomenonRadius.max);
		}
		if (componentData.m_HotspotRadius == 0f)
		{
			componentData.m_HotspotRadius = componentData.m_PhenomenonRadius * ((Random)(ref random)).NextFloat(componentData4.m_HotspotRadius.min, componentData4.m_HotspotRadius.max);
		}
		if (componentData2.m_StartFrame == 0)
		{
			float value = 0f;
			if (componentData4.m_DangerFlags != 0)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				DynamicBuffer<CityModifier> buffer3 = ((EntityManager)(ref entityManager)).GetBuffer<CityModifier>(m_CitySystem.City, true);
				CityUtils.ApplyModifier(ref value, buffer3, CityModifierType.DisasterWarningTime);
			}
			componentData2.m_StartFrame = m_SimulationSystem.frameIndex + (uint)(value * 60f);
		}
		if (componentData2.m_EndFrame == 0)
		{
			float num = ((Random)(ref random)).NextFloat(componentData4.m_Duration.min, componentData4.m_Duration.max);
			componentData2.m_EndFrame = componentData2.m_StartFrame + (uint)(num * 60f);
		}
		bool flag = !((float3)(ref componentData.m_PhenomenonPosition)).Equals(default(float3));
		Transform transform = default(Transform);
		for (int i = 0; i < buffer.Length; i++)
		{
			if (flag)
			{
				break;
			}
			Entity entity = buffer[i].m_Entity;
			if (EntitiesExtensions.TryGetComponent<Transform>(((ComponentSystemBase)this).EntityManager, entity, ref transform))
			{
				componentData.m_PhenomenonPosition = transform.m_Position;
				flag = true;
			}
		}
		if (!flag)
		{
			componentData.m_PhenomenonPosition = FindRandomLocation(ref random);
		}
		if (((float3)(ref componentData.m_HotspotPosition)).Equals(default(float3)))
		{
			componentData.m_HotspotPosition = componentData.m_PhenomenonPosition;
			ref float3 hotspotPosition = ref componentData.m_HotspotPosition;
			((float3)(ref hotspotPosition)).xz = ((float3)(ref hotspotPosition)).xz + ((Random)(ref random)).NextFloat2Direction() * ((Random)(ref random)).NextFloat(componentData.m_PhenomenonRadius - componentData.m_HotspotRadius);
		}
		if (componentData.m_LightningTimer == 0f && componentData4.m_LightningInterval.min > 0.001f)
		{
			float min = componentData4.m_LightningInterval.min;
			min = math.min(min, (float)(componentData2.m_EndFrame - componentData2.m_StartFrame) / 60f);
			componentData.m_LightningTimer = 5f + math.max(0f, min - 10f);
		}
		buffer2.ResizeUninitialized(4);
		for (int j = 0; j < buffer2.Length; j++)
		{
			buffer2[j] = new HotspotFrame(componentData);
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<EarlyDisasterWarningEventData>(componentData3.m_Prefab) && !((EntityQuery)(ref m_EDWSBuildingQuery)).IsEmptyIgnoreFilter)
		{
			Enumerator<Entity> enumerator = ((EntityQuery)(ref m_EDWSBuildingQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Entity current = enumerator.Current;
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).AddComponentData<EarlyDisasterWarningDuration>(current, new EarlyDisasterWarningDuration
					{
						m_EndFrame = componentData2.m_EndFrame
					});
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentData<WeatherPhenomenon>(eventEntity, componentData);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentData<Duration>(eventEntity, componentData2);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentData<InterpolatedTransform>(eventEntity, new InterpolatedTransform(componentData));
	}

	private void InitializeHealthEvent(Entity eventEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(eventEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		HealthEventData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<HealthEventData>(componentData.m_Prefab);
		if (componentData2.m_RandomTargetType == EventTargetType.None)
		{
			return;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<TargetElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TargetElement>(eventEntity, false);
		if (buffer.Length == 0)
		{
			AddRandomTarget(buffer, componentData2.m_RandomTargetType, TransportType.None);
		}
		Random random = RandomSeed.Next().GetRandom(eventEntity.Index);
		EntityCommandBuffer commandBuffer = GetCommandBuffer();
		Game.Creatures.Resident resident = default(Game.Creatures.Resident);
		Citizen citizen = default(Citizen);
		for (int i = 0; i < buffer.Length; i++)
		{
			Entity val = buffer[i].m_Entity;
			if (EntitiesExtensions.TryGetComponent<Game.Creatures.Resident>(((ComponentSystemBase)this).EntityManager, val, ref resident))
			{
				val = resident.m_Citizen;
				buffer[i] = new TargetElement(val);
			}
			if (EntitiesExtensions.TryGetComponent<Citizen>(((ComponentSystemBase)this).EntityManager, val, ref citizen))
			{
				HealthProblemFlags healthProblemFlags = HealthProblemFlags.None;
				switch (componentData2.m_HealthEventType)
				{
				case HealthEventType.Disease:
					healthProblemFlags |= HealthProblemFlags.Sick;
					break;
				case HealthEventType.Injury:
					healthProblemFlags |= HealthProblemFlags.Injured;
					break;
				case HealthEventType.Death:
					healthProblemFlags |= HealthProblemFlags.Dead;
					break;
				}
				float num = math.lerp(componentData2.m_TransportProbability.max, componentData2.m_TransportProbability.min, (float)(int)citizen.m_Health * 0.01f);
				if (((Random)(ref random)).NextFloat(100f) < num)
				{
					healthProblemFlags |= HealthProblemFlags.RequireTransport;
				}
				Entity val2 = ((EntityCommandBuffer)(ref commandBuffer)).CreateEntity(m_HealthEventArchetype);
				if (componentData2.m_RequireTracking)
				{
					((EntityCommandBuffer)(ref commandBuffer)).SetComponent<AddHealthProblem>(val2, new AddHealthProblem
					{
						m_Event = eventEntity,
						m_Target = val,
						m_Flags = healthProblemFlags
					});
				}
				else
				{
					((EntityCommandBuffer)(ref commandBuffer)).SetComponent<AddHealthProblem>(val2, new AddHealthProblem
					{
						m_Target = val,
						m_Flags = healthProblemFlags
					});
				}
			}
		}
		if (!componentData2.m_RequireTracking)
		{
			buffer.Clear();
		}
	}

	private void InitializeDestruction(Entity eventEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(eventEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DestructionData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<DestructionData>(componentData.m_Prefab);
		if (componentData2.m_RandomTargetType == EventTargetType.None)
		{
			return;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<TargetElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TargetElement>(eventEntity, false);
		if (buffer.Length == 0)
		{
			AddRandomTarget(buffer, componentData2.m_RandomTargetType, TransportType.None);
		}
		EntityCommandBuffer commandBuffer = GetCommandBuffer();
		IconCommandBuffer iconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer();
		DisasterConfigurationData disasterConfigurationData = default(DisasterConfigurationData);
		if (!((EntityQuery)(ref m_DisasterConfigQuery)).IsEmpty)
		{
			disasterConfigurationData = ((EntityQuery)(ref m_DisasterConfigQuery)).GetSingleton<DisasterConfigurationData>();
		}
		for (int i = 0; i < buffer.Length; i++)
		{
			Entity entity = buffer[i].m_Entity;
			Entity val = ((EntityCommandBuffer)(ref commandBuffer)).CreateEntity(m_DamageEventArchetype);
			((EntityCommandBuffer)(ref commandBuffer)).SetComponent<Damage>(val, new Damage
			{
				m_Object = entity,
				m_Delta = new float3(1f, 0f, 0f)
			});
			Entity val2 = ((EntityCommandBuffer)(ref commandBuffer)).CreateEntity(m_DestroyEventArchetype);
			((EntityCommandBuffer)(ref commandBuffer)).SetComponent<Destroy>(val2, new Destroy
			{
				m_Event = eventEntity,
				m_Object = entity
			});
			if (disasterConfigurationData.m_DestroyedNotificationPrefab != Entity.Null)
			{
				iconCommandBuffer.Remove(entity, IconPriority.Problem);
				iconCommandBuffer.Remove(entity, IconPriority.FatalProblem);
				iconCommandBuffer.Add(entity, disasterConfigurationData.m_DestroyedNotificationPrefab, IconPriority.FatalProblem, IconClusterLayer.Default, IconFlags.IgnoreTarget, eventEntity);
			}
		}
	}

	private void InitializeSpectatorEvent(Entity eventEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Duration componentData = ((EntityManager)(ref entityManager)).GetComponentData<Duration>(eventEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		PrefabRef componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(eventEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		SpectatorEventData componentData3 = ((EntityManager)(ref entityManager)).GetComponentData<SpectatorEventData>(componentData2.m_Prefab);
		if (componentData3.m_RandomSiteType != EventTargetType.None)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<TargetElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TargetElement>(eventEntity, false);
			if (buffer.Length == 0)
			{
				VehicleLaunchData vehicleLaunchData = default(VehicleLaunchData);
				if (EntitiesExtensions.TryGetComponent<VehicleLaunchData>(((ComponentSystemBase)this).EntityManager, componentData2.m_Prefab, ref vehicleLaunchData))
				{
					AddRandomTarget(buffer, componentData3.m_RandomSiteType, vehicleLaunchData.m_TransportType);
				}
				else
				{
					AddRandomTarget(buffer, componentData3.m_RandomSiteType, TransportType.None);
				}
			}
			EntityCommandBuffer commandBuffer = GetCommandBuffer();
			for (int i = 0; i < buffer.Length; i++)
			{
				Entity entity = buffer[i].m_Entity;
				Entity val = ((EntityCommandBuffer)(ref commandBuffer)).CreateEntity(m_SpectateEventArchetype);
				((EntityCommandBuffer)(ref commandBuffer)).SetComponent<Spectate>(val, new Spectate
				{
					m_Event = eventEntity,
					m_Target = entity
				});
			}
		}
		componentData.m_StartFrame = m_SimulationSystem.frameIndex + (uint)(262144f * componentData3.m_PreparationDuration);
		componentData.m_EndFrame = componentData.m_StartFrame + (uint)(262144f * componentData3.m_ActiveDuration);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentData<Duration>(eventEntity, componentData);
	}

	private void InitializeWaterLevelChangeEvent(Entity eventEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		WaterLevelChange componentData = ((EntityManager)(ref entityManager)).GetComponentData<WaterLevelChange>(eventEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		PrefabRef componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(eventEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		WaterLevelChangeData componentData3 = ((EntityManager)(ref entityManager)).GetComponentData<WaterLevelChangeData>(componentData2.m_Prefab);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		Duration componentData4 = ((EntityManager)(ref entityManager)).GetComponentData<Duration>(eventEntity);
		Random random = RandomSeed.Next().GetRandom(eventEntity.Index);
		float num = ((Random)(ref random)).NextFloat();
		componentData.m_MaxIntensity = 0.3f + 0.7f * num * num;
		componentData.m_Direction = new float2(0f, 1f);
		EntityQuery entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Simulation.WaterSourceData>(),
			ComponentType.ReadOnly<Transform>()
		});
		NativeArray<Game.Simulation.WaterSourceData> val = ((EntityQuery)(ref entityQuery)).ToComponentDataArray<Game.Simulation.WaterSourceData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<Transform> val2 = ((EntityQuery)(ref entityQuery)).ToComponentDataArray<Transform>(AllocatorHandle.op_Implicit((Allocator)3));
		componentData4.m_StartFrame = m_SimulationSystem.frameIndex;
		if (componentData3.m_ChangeType == WaterLevelChangeType.Sine)
		{
			componentData4.m_EndFrame = (uint)(componentData4.m_StartFrame + Mathf.CeilToInt((float)WaterLevelChangeSystem.TsunamiEndDelay + 12000f * componentData.m_MaxIntensity));
		}
		else
		{
			componentData4.m_EndFrame = componentData4.m_StartFrame + 10000;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentData<Duration>(eventEntity, componentData4);
		for (int i = 0; i < val.Length; i++)
		{
			Game.Simulation.WaterSourceData source = val[i];
			Transform transform = val2[i];
			if (((source.m_ConstantDepth == 2 && (componentData3.m_TargetType & WaterLevelTargetType.River) != WaterLevelTargetType.None) || (source.m_ConstantDepth == 3 && (componentData3.m_TargetType & WaterLevelTargetType.Sea) != WaterLevelTargetType.None)) && WaterSystem.SourceMatchesDirection(source, transform, componentData.m_Direction))
			{
				componentData.m_DangerHeight = math.max(componentData.m_DangerHeight, source.m_Amount + source.m_Multiplier * componentData.m_MaxIntensity * 2f);
			}
		}
		val.Dispose();
		val2.Dispose();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentData<WaterLevelChange>(eventEntity, componentData);
	}

	private void InitializeCrimeEvent(Entity eventEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(eventEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		CrimeData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<CrimeData>(componentData.m_Prefab);
		if (componentData2.m_RandomTargetType == EventTargetType.None)
		{
			return;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<TargetElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TargetElement>(eventEntity, false);
		if (buffer.Length == 0)
		{
			AddRandomTarget(buffer, componentData2.m_RandomTargetType, TransportType.None);
		}
		RandomSeed.Next().GetRandom(eventEntity.Index);
		EntityCommandBuffer commandBuffer = GetCommandBuffer();
		Game.Creatures.Resident resident = default(Game.Creatures.Resident);
		Citizen citizen = default(Citizen);
		for (int i = 0; i < buffer.Length; i++)
		{
			Entity val = buffer[i].m_Entity;
			if (EntitiesExtensions.TryGetComponent<Game.Creatures.Resident>(((ComponentSystemBase)this).EntityManager, val, ref resident))
			{
				val = resident.m_Citizen;
				buffer[i] = new TargetElement(val);
			}
			if (EntitiesExtensions.TryGetComponent<Citizen>(((ComponentSystemBase)this).EntityManager, val, ref citizen))
			{
				CriminalFlags criminalFlags = CriminalFlags.Planning;
				if (componentData2.m_CrimeType == CrimeType.Robbery)
				{
					criminalFlags |= CriminalFlags.Robber;
				}
				Entity val2 = ((EntityCommandBuffer)(ref commandBuffer)).CreateEntity(m_CriminalEventArchetype);
				((EntityCommandBuffer)(ref commandBuffer)).SetComponent<AddCriminal>(val2, new AddCriminal
				{
					m_Event = eventEntity,
					m_Target = val,
					m_Flags = criminalFlags
				});
			}
		}
	}

	private void InitializeMeetingEvent(Entity eventEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<CoordinatedMeetingAttendee> buffer = ((EntityManager)(ref entityManager)).GetBuffer<CoordinatedMeetingAttendee>(eventEntity, false);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<TargetElement> buffer2 = ((EntityManager)(ref entityManager)).GetBuffer<TargetElement>(eventEntity, false);
		for (int i = 0; i < buffer2.Length; i++)
		{
			Entity entity = buffer2[i].m_Entity;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<HouseholdCitizen>(entity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<AttendingEvent>(entity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					DynamicBuffer<HouseholdCitizen> buffer3 = ((EntityManager)(ref entityManager)).GetBuffer<HouseholdCitizen>(entity, true);
					for (int j = 0; j < buffer3.Length; j++)
					{
						buffer.Add(new CoordinatedMeetingAttendee
						{
							m_Attendee = buffer3[j].m_Citizen
						});
					}
					continue;
				}
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Citizen>(entity))
			{
				buffer.Add(new CoordinatedMeetingAttendee
				{
					m_Attendee = entity
				});
			}
		}
	}

	private void InitializeCalendarEvent(Entity eventEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(eventEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		CalendarEventData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<CalendarEventData>(componentData.m_Prefab);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		Duration componentData3 = ((EntityManager)(ref entityManager)).GetComponentData<Duration>(eventEntity);
		componentData3.m_StartFrame = m_SimulationSystem.frameIndex;
		componentData3.m_EndFrame = componentData3.m_StartFrame + (uint)(componentData2.m_Duration * 262144 / 4);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentData<Duration>(eventEntity, componentData3);
		EntityCommandBuffer commandBuffer = GetCommandBuffer();
		((EntityCommandBuffer)(ref commandBuffer)).AddComponent<FindingEventParticipants>(eventEntity);
	}

	private float3 FindRandomLocation(ref Random random)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		float3 result = default(float3);
		((float3)(ref result)).xz = ((Random)(ref random)).NextFloat2(float2.op_Implicit(-6000f), float2.op_Implicit(6000f));
		return result;
	}

	private void AddRandomTarget(DynamicBuffer<TargetElement> targets, EventTargetType targetType, TransportType transportType)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		NativeValue<Entity> result = default(NativeValue<Entity>);
		result._002Ector((Allocator)3);
		try
		{
			IJobExtensions.Run<RandomEventTargetJob>(new RandomEventTargetJob
			{
				m_TargetChunks = ((EntityQuery)(ref m_TargetQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3)),
				m_TargetType = targetType,
				m_TransportType = transportType,
				m_RandomSeed = RandomSeed.Next(),
				m_Result = result,
				m_EntitiesType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransportDepotType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.TransportDepot>(ref __TypeHandle.__Game_Buildings_TransportDepot_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TreeType = InternalCompilerInterface.GetComponentTypeHandle<Tree>(ref __TypeHandle.__Game_Objects_Tree_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RoadType = InternalCompilerInterface.GetComponentTypeHandle<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CitizenType = InternalCompilerInterface.GetComponentTypeHandle<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabTransportDepotData = InternalCompilerInterface.GetComponentLookup<TransportDepotData>(ref __TypeHandle.__Game_Prefabs_TransportDepotData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			});
			if (result.value != Entity.Null)
			{
				targets.Add(new TargetElement(result.value));
			}
		}
		finally
		{
			result.Dispose();
		}
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
