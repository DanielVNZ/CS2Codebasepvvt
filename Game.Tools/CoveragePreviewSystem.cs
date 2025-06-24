using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Simulation;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class CoveragePreviewSystem : GameSystemBase
{
	[BurstCompile]
	public struct InitializeCoverageJob : IJobChunk
	{
		[ReadOnly]
		public int m_SourceCoverageIndex;

		[ReadOnly]
		public int m_TargetCoverageIndex;

		public BufferTypeHandle<Game.Net.ServiceCoverage> m_ServiceCoverageType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<Game.Net.ServiceCoverage> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Net.ServiceCoverage>(ref m_ServiceCoverageType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<Game.Net.ServiceCoverage> val = bufferAccessor[i];
				val[m_TargetCoverageIndex] = val[m_SourceCoverageIndex];
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CopyServiceCoverageJob : IJob
	{
		[ReadOnly]
		public Entity m_Source;

		[ReadOnly]
		public Entity m_Target;

		public BufferLookup<CoverageElement> m_CoverageElements;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			if (m_CoverageElements.HasBuffer(m_Source) && m_CoverageElements.HasBuffer(m_Target))
			{
				DynamicBuffer<CoverageElement> val = m_CoverageElements[m_Source];
				m_CoverageElements[m_Target].CopyFrom(val);
			}
		}
	}

	[BurstCompile]
	public struct SetupCoverageSearchJob : IJob
	{
		[ReadOnly]
		public Entity m_Entity;

		[ReadOnly]
		public ComponentLookup<BackSide> m_BackSideData;

		[ReadOnly]
		public ComponentLookup<CoverageData> m_PrefabCoverageData;

		public CoverageAction m_Action;

		public PathfindTargetSeeker<PathfindTargetBuffer> m_TargetSeeker;

		public void Execute()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_TargetSeeker.m_PrefabRef[m_Entity];
			CoverageData coverageData = default(CoverageData);
			m_PrefabCoverageData.TryGetComponent(prefabRef.m_Prefab, ref coverageData);
			Building building = default(Building);
			if (m_TargetSeeker.m_Building.TryGetComponent(m_Entity, ref building))
			{
				Transform transform = m_TargetSeeker.m_Transform[m_Entity];
				if (building.m_RoadEdge != Entity.Null)
				{
					BuildingData buildingData = m_TargetSeeker.m_BuildingData[prefabRef.m_Prefab];
					float3 comparePosition = transform.m_Position;
					Owner owner = default(Owner);
					if (!m_TargetSeeker.m_Owner.TryGetComponent(building.m_RoadEdge, ref owner) || owner.m_Owner != m_Entity)
					{
						comparePosition = BuildingUtils.CalculateFrontPosition(transform, buildingData.m_LotSize.y);
					}
					Random random = m_TargetSeeker.m_RandomSeed.GetRandom(m_Entity.Index);
					m_TargetSeeker.AddEdgeTargets(ref random, m_Entity, 0f, EdgeFlags.DefaultMask, building.m_RoadEdge, comparePosition, 0f, allowLaneGroupSwitch: true, allowAccessRestriction: false);
				}
			}
			else
			{
				m_TargetSeeker.FindTargets(m_Entity, 0f);
			}
			BackSide backSide = default(BackSide);
			if (m_BackSideData.TryGetComponent(m_Entity, ref backSide))
			{
				Transform transform2 = m_TargetSeeker.m_Transform[m_Entity];
				if (backSide.m_RoadEdge != Entity.Null)
				{
					BuildingData buildingData2 = m_TargetSeeker.m_BuildingData[prefabRef.m_Prefab];
					float3 comparePosition2 = transform2.m_Position;
					Owner owner2 = default(Owner);
					if (!m_TargetSeeker.m_Owner.TryGetComponent(backSide.m_RoadEdge, ref owner2) || owner2.m_Owner != m_Entity)
					{
						comparePosition2 = BuildingUtils.CalculateFrontPosition(transform2, -buildingData2.m_LotSize.y);
					}
					Random random2 = m_TargetSeeker.m_RandomSeed.GetRandom(m_Entity.Index);
					m_TargetSeeker.AddEdgeTargets(ref random2, m_Entity, 0f, EdgeFlags.DefaultMask, backSide.m_RoadEdge, comparePosition2, 0f, allowLaneGroupSwitch: true, allowAccessRestriction: false);
				}
			}
			m_Action.data.m_Parameters = new CoverageParameters
			{
				m_Methods = m_TargetSeeker.m_PathfindParameters.m_Methods,
				m_Range = coverageData.m_Range
			};
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<InfoviewCoverageData> __Game_Prefabs_InfoviewCoverageData_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		public SharedComponentTypeHandle<CoverageServiceType> __Game_Net_CoverageServiceType_SharedComponentTypeHandle;

		public BufferTypeHandle<Game.Net.ServiceCoverage> __Game_Net_ServiceCoverage_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Created> __Game_Common_Created_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<BackSide> __Game_Buildings_BackSide_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CoverageData> __Game_Prefabs_CoverageData_RO_ComponentLookup;

		public BufferLookup<CoverageElement> __Game_Pathfind_CoverageElement_RW_BufferLookup;

		[ReadOnly]
		public BufferTypeHandle<CoverageElement> __Game_Pathfind_CoverageElement_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Density> __Game_Net_Density_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ModifiedServiceCoverage> __Game_Buildings_ModifiedServiceCoverage_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BorderDistrict> __Game_Areas_BorderDistrict_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CoverageElement> __Game_Pathfind_CoverageElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> __Game_Areas_ServiceDistrict_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Efficiency> __Game_Buildings_Efficiency_RO_BufferLookup;

		public BufferLookup<Game.Net.ServiceCoverage> __Game_Net_ServiceCoverage_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_InfoviewCoverageData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewCoverageData>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_CoverageServiceType_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<CoverageServiceType>();
			__Game_Net_ServiceCoverage_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Net.ServiceCoverage>(false);
			__Game_Common_Created_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Created>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Buildings_BackSide_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BackSide>(true);
			__Game_Prefabs_CoverageData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CoverageData>(true);
			__Game_Pathfind_CoverageElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CoverageElement>(false);
			__Game_Pathfind_CoverageElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<CoverageElement>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Density_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Density>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Buildings_ModifiedServiceCoverage_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ModifiedServiceCoverage>(true);
			__Game_Areas_BorderDistrict_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BorderDistrict>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Pathfind_CoverageElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CoverageElement>(true);
			__Game_Areas_ServiceDistrict_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceDistrict>(true);
			__Game_Buildings_Efficiency_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Efficiency>(true);
			__Game_Net_ServiceCoverage_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.ServiceCoverage>(false);
		}
	}

	private PathfindQueueSystem m_PathfindQueueSystem;

	private AirwaySystem m_AirwaySystem;

	private EntityQuery m_EdgeQuery;

	private EntityQuery m_ModifiedQuery;

	private EntityQuery m_UpdatedBuildingQuery;

	private EntityQuery m_ServiceBuildingQuery;

	private EntityQuery m_InfomodeQuery;

	private EntityQuery m_EventQuery;

	private CoverageService m_LastService;

	private PathfindTargetSeekerData m_TargetSeekerData;

	private HashSet<Entity> m_PendingCoverages;

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
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PathfindQueueSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindQueueSystem>();
		m_AirwaySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirwaySystem>();
		m_EdgeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Net.Edge>(),
			ComponentType.ReadWrite<Game.Net.ServiceCoverage>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Updated>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Net.Edge>(),
			ComponentType.ReadWrite<District>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Net.Edge>(),
			ComponentType.ReadWrite<District>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[1] = val;
		m_ModifiedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_UpdatedBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<CoverageServiceType>(),
			ComponentType.ReadOnly<CoverageElement>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ServiceBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<CoverageServiceType>(),
			ComponentType.ReadOnly<CoverageElement>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Hidden>(),
			ComponentType.Exclude<Deleted>()
		});
		m_InfomodeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<InfomodeActive>(),
			ComponentType.ReadOnly<InfoviewCoverageData>()
		});
		m_EventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Event>(),
			ComponentType.ReadOnly<CoverageUpdated>()
		});
		m_LastService = CoverageService.Count;
		m_TargetSeekerData = new PathfindTargetSeekerData((SystemBase)(object)this);
		m_PendingCoverages = new HashSet<Entity>();
		((ComponentSystemBase)this).RequireForUpdate(m_InfomodeQuery);
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		m_LastService = CoverageService.Count;
		((COSystemBase)this).OnStopRunning();
	}

	private bool GetInfoviewCoverageData(out InfoviewCoverageData coverageData)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_InfomodeQuery)).IsEmptyIgnoreFilter)
		{
			coverageData = default(InfoviewCoverageData);
			return false;
		}
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_InfomodeQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		ComponentTypeHandle<InfoviewCoverageData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<InfoviewCoverageData>(ref __TypeHandle.__Game_Prefabs_InfoviewCoverageData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ArchetypeChunk val2 = val[0];
		coverageData = ((ArchetypeChunk)(ref val2)).GetNativeArray<InfoviewCoverageData>(ref componentTypeHandle)[0];
		val.Dispose();
		return true;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_0629: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_067b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0711: Unknown result type (might be due to invalid IL or missing references)
		//IL_0729: Unknown result type (might be due to invalid IL or missing references)
		//IL_072e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0763: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Unknown result type (might be due to invalid IL or missing references)
		//IL_077b: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0784: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07da: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		if (!GetInfoviewCoverageData(out var coverageData))
		{
			m_LastService = CoverageService.Count;
		}
		bool flag = m_LastService != coverageData.m_Service;
		bool flag2 = flag || !((EntityQuery)(ref m_ModifiedQuery)).IsEmptyIgnoreFilter;
		m_LastService = coverageData.m_Service;
		bool flag3 = (flag2 ? (!((EntityQuery)(ref m_ServiceBuildingQuery)).IsEmptyIgnoreFilter) : (!((EntityQuery)(ref m_UpdatedBuildingQuery)).IsEmptyIgnoreFilter));
		bool flag4 = !((EntityQuery)(ref m_EventQuery)).IsEmptyIgnoreFilter;
		if (!flag3 && !flag2 && !flag4)
		{
			return;
		}
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_ServiceBuildingQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		SharedComponentTypeHandle<CoverageServiceType> sharedComponentTypeHandle = InternalCompilerInterface.GetSharedComponentTypeHandle<CoverageServiceType>(ref __TypeHandle.__Game_Net_CoverageServiceType_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		BufferTypeHandle<Game.Net.ServiceCoverage> bufferTypeHandle = InternalCompilerInterface.GetBufferTypeHandle<Game.Net.ServiceCoverage>(ref __TypeHandle.__Game_Net_ServiceCoverage_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<Created> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<Temp> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		JobHandle val2 = default(JobHandle);
		JobHandle val3 = default(JobHandle);
		if (flag2)
		{
			m_PendingCoverages.Clear();
		}
		if (flag3)
		{
			m_TargetSeekerData.Update((SystemBase)(object)this, m_AirwaySystem.GetAirwayData());
			PathfindParameters pathfindParameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(111.111115f),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
				m_PathfindFlags = (PathfindFlags.Stable | PathfindFlags.IgnoreFlow),
				m_IgnoredRules = (RuleFlags.HasBlockage | RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidTransitTraffic | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic)
			};
			SetupQueueTarget setupQueueTarget = default(SetupQueueTarget);
			ServiceCoverageSystem.SetupPathfindMethods(coverageData.m_Service, ref pathfindParameters, ref setupQueueTarget);
			NativeArray<ArchetypeChunk> val4 = ((!flag2) ? ((EntityQuery)(ref m_UpdatedBuildingQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3)) : val);
			SetupCoverageSearchJob setupCoverageSearchJob = new SetupCoverageSearchJob
			{
				m_BackSideData = InternalCompilerInterface.GetComponentLookup<BackSide>(ref __TypeHandle.__Game_Buildings_BackSide_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCoverageData = InternalCompilerInterface.GetComponentLookup<CoverageData>(ref __TypeHandle.__Game_Prefabs_CoverageData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).CompleteDependencyBeforeRO<Temp>();
			Temp temp = default(Temp);
			DynamicBuffer<CoverageElement> val8 = default(DynamicBuffer<CoverageElement>);
			for (int i = 0; i < val4.Length; i++)
			{
				ArchetypeChunk val5 = val4[i];
				if (((ArchetypeChunk)(ref val5)).GetSharedComponent<CoverageServiceType>(sharedComponentTypeHandle).m_Service != coverageData.m_Service)
				{
					continue;
				}
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val5)).GetNativeArray(entityTypeHandle);
				NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref val5)).GetNativeArray<Temp>(ref componentTypeHandle2);
				for (int j = 0; j < ((ArchetypeChunk)(ref val5)).Count; j++)
				{
					Entity val6 = nativeArray[j];
					Entity val7 = ((CollectionUtils.TryGet<Temp>(nativeArray2, j, ref temp) && temp.m_Original != Entity.Null) ? temp.m_Original : val6);
					if (EntitiesExtensions.TryGetBuffer<CoverageElement>(((ComponentSystemBase)this).EntityManager, val7, true, ref val8) && val8.Length == 0)
					{
						m_PendingCoverages.Add(val6);
					}
				}
			}
			Temp temp2 = default(Temp);
			for (int k = 0; k < val4.Length; k++)
			{
				ArchetypeChunk val9 = val4[k];
				if (((ArchetypeChunk)(ref val9)).GetSharedComponent<CoverageServiceType>(sharedComponentTypeHandle).m_Service != coverageData.m_Service)
				{
					continue;
				}
				NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref val9)).GetNativeArray(entityTypeHandle);
				NativeArray<Temp> nativeArray4 = ((ArchetypeChunk)(ref val9)).GetNativeArray<Temp>(ref componentTypeHandle2);
				bool flag5 = ((ArchetypeChunk)(ref val9)).Has<Created>(ref componentTypeHandle);
				for (int l = 0; l < ((ArchetypeChunk)(ref val9)).Count; l++)
				{
					Entity val10 = nativeArray3[l];
					CollectionUtils.TryGet<Temp>(nativeArray4, l, ref temp2);
					CoverageAction action = new CoverageAction((Allocator)4);
					if (temp2.m_Original != Entity.Null && (temp2.m_Flags & TempFlags.Modify) == 0)
					{
						setupCoverageSearchJob.m_Entity = temp2.m_Original;
					}
					else
					{
						setupCoverageSearchJob.m_Entity = val10;
					}
					setupCoverageSearchJob.m_TargetSeeker = new PathfindTargetSeeker<PathfindTargetBuffer>(m_TargetSeekerData, pathfindParameters, setupQueueTarget, action.data.m_Sources.AsParallelWriter(), RandomSeed.Next(), isStartTarget: true);
					setupCoverageSearchJob.m_Action = action;
					JobHandle val11 = IJobExtensions.Schedule<SetupCoverageSearchJob>(setupCoverageSearchJob, ((SystemBase)this).Dependency);
					val2 = JobHandle.CombineDependencies(val2, val11);
					m_PathfindQueueSystem.Enqueue(action, val10, val11, uint.MaxValue, this, default(PathEventData), nativeArray4.Length != 0);
					if (flag5 && temp2.m_Original != Entity.Null)
					{
						val3 = IJobExtensions.Schedule<CopyServiceCoverageJob>(new CopyServiceCoverageJob
						{
							m_Source = temp2.m_Original,
							m_Target = val10,
							m_CoverageElements = InternalCompilerInterface.GetBufferLookup<CoverageElement>(ref __TypeHandle.__Game_Pathfind_CoverageElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
						}, val3);
					}
				}
			}
			if (!flag2)
			{
				val4.Dispose();
			}
		}
		if (flag4)
		{
			NativeArray<CoverageUpdated> val12 = ((EntityQuery)(ref m_EventQuery)).ToComponentDataArray<CoverageUpdated>(AllocatorHandle.op_Implicit((Allocator)2));
			for (int m = 0; m < val12.Length; m++)
			{
				m_PendingCoverages.Remove(val12[m].m_Owner);
			}
			val12.Dispose();
		}
		if (m_PendingCoverages.Count != 0)
		{
			if (flag)
			{
				JobHandle val13 = JobChunkExtensions.ScheduleParallel<InitializeCoverageJob>(new InitializeCoverageJob
				{
					m_SourceCoverageIndex = (int)coverageData.m_Service,
					m_TargetCoverageIndex = 8,
					m_ServiceCoverageType = bufferTypeHandle
				}, m_EdgeQuery, ((SystemBase)this).Dependency);
				val2 = JobHandle.CombineDependencies(val2, val3, val13);
			}
			else
			{
				val2 = JobHandle.CombineDependencies(val2, val3);
			}
			val.Dispose();
			((SystemBase)this).Dependency = val2;
			return;
		}
		NativeList<ServiceCoverageSystem.BuildingData> val14 = default(NativeList<ServiceCoverageSystem.BuildingData>);
		val14._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<ServiceCoverageSystem.CoverageElement> elements = default(NativeList<ServiceCoverageSystem.CoverageElement>);
		elements._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		ServiceCoverageSystem.PrepareCoverageJob prepareCoverageJob = new ServiceCoverageSystem.PrepareCoverageJob
		{
			m_Service = coverageData.m_Service,
			m_BuildingChunks = val,
			m_CoverageServiceType = sharedComponentTypeHandle,
			m_EntityType = entityTypeHandle,
			m_CoverageElementType = InternalCompilerInterface.GetBufferTypeHandle<CoverageElement>(ref __TypeHandle.__Game_Pathfind_CoverageElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = val14,
			m_Elements = elements
		};
		ServiceCoverageSystem.ClearCoverageJob clearCoverageJob = new ServiceCoverageSystem.ClearCoverageJob
		{
			m_CoverageIndex = 8,
			m_ServiceCoverageType = bufferTypeHandle
		};
		ServiceCoverageSystem.ProcessCoverageJob processCoverageJob = new ServiceCoverageSystem.ProcessCoverageJob
		{
			m_CoverageIndex = 8,
			m_BuildingData = val14,
			m_Elements = elements,
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DensityData = InternalCompilerInterface.GetComponentLookup<Density>(ref __TypeHandle.__Game_Net_Density_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ModifiedServiceCoverageData = InternalCompilerInterface.GetComponentLookup<ModifiedServiceCoverage>(ref __TypeHandle.__Game_Buildings_ModifiedServiceCoverage_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BorderDistrictData = InternalCompilerInterface.GetComponentLookup<BorderDistrict>(ref __TypeHandle.__Game_Areas_BorderDistrict_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCoverageData = InternalCompilerInterface.GetComponentLookup<CoverageData>(ref __TypeHandle.__Game_Prefabs_CoverageData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CoverageElements = InternalCompilerInterface.GetBufferLookup<CoverageElement>(ref __TypeHandle.__Game_Pathfind_CoverageElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDistricts = InternalCompilerInterface.GetBufferLookup<ServiceDistrict>(ref __TypeHandle.__Game_Areas_ServiceDistrict_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Efficiencies = InternalCompilerInterface.GetBufferLookup<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CoverageData = InternalCompilerInterface.GetBufferLookup<Game.Net.ServiceCoverage>(ref __TypeHandle.__Game_Net_ServiceCoverage_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		ServiceCoverageSystem.ApplyCoverageJob obj = new ServiceCoverageSystem.ApplyCoverageJob
		{
			m_BuildingData = val14,
			m_Elements = elements
		};
		JobHandle val15 = IJobExtensions.Schedule<ServiceCoverageSystem.PrepareCoverageJob>(prepareCoverageJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val3));
		JobHandle val16 = JobChunkExtensions.ScheduleParallel<ServiceCoverageSystem.ClearCoverageJob>(clearCoverageJob, m_EdgeQuery, ((SystemBase)this).Dependency);
		JobHandle val17 = IJobParallelForDeferExtensions.Schedule<ServiceCoverageSystem.ProcessCoverageJob, ServiceCoverageSystem.BuildingData>(processCoverageJob, val14, 1, JobHandle.CombineDependencies(val15, val16));
		JobHandle val18 = IJobExtensions.Schedule<ServiceCoverageSystem.ApplyCoverageJob>(obj, val17);
		val.Dispose(val15);
		val14.Dispose(val18);
		elements.Dispose(val18);
		val2 = JobHandle.CombineDependencies(val2, val18);
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
	public CoveragePreviewSystem()
	{
	}
}
