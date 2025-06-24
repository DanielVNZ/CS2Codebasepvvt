using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Game.Serialization;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Net;

[CompilerGenerated]
public class AirwaySystem : GameSystemBase, IJobSerializable
{
	[BurstCompile]
	internal struct SerializeJob<TWriter> : IJob where TWriter : struct, IWriter
	{
		[ReadOnly]
		public AirwayHelpers.AirwayMap m_HelicopterMap;

		[ReadOnly]
		public AirwayHelpers.AirwayMap m_AirplaneMap;

		public EntityWriterData m_WriterData;

		public void Execute()
		{
			TWriter writer = ((EntityWriterData)(ref m_WriterData)).GetWriter<TWriter>();
			m_HelicopterMap.Serialize(writer);
			m_AirplaneMap.Serialize(writer);
		}
	}

	[BurstCompile]
	internal struct DeserializeJob<TReader> : IJob where TReader : struct, IReader
	{
		public AirwayHelpers.AirwayMap m_HelicopterMap;

		public AirwayHelpers.AirwayMap m_AirplaneMap;

		public EntityReaderData m_ReaderData;

		public void Execute()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			TReader reader = ((EntityReaderData)(ref m_ReaderData)).GetReader<TReader>();
			m_HelicopterMap.Deserialize(reader);
			Context context = ((IReader)reader).context;
			if (((Context)(ref context)).version >= Version.airplaneAirways)
			{
				m_AirplaneMap.Deserialize(reader);
			}
			else
			{
				m_AirplaneMap.SetDefaults(((IReader)reader).context);
			}
		}
	}

	[BurstCompile]
	private struct SetDefaultsJob : IJob
	{
		[ReadOnly]
		public Context m_Context;

		public AirwayHelpers.AirwayMap m_HelicopterMap;

		public AirwayHelpers.AirwayMap m_AirplaneMap;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			m_HelicopterMap.SetDefaults(m_Context);
			m_AirplaneMap.SetDefaults(m_Context);
		}
	}

	[BurstCompile]
	private struct GenerateAirwayLanesJob : IJobParallelFor
	{
		[ReadOnly]
		public AirwayHelpers.AirwayMap m_AirwayMap;

		[ReadOnly]
		public Entity m_Prefab;

		[ReadOnly]
		public RoadTypes m_RoadType;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Lane> m_LaneData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Curve> m_CurveData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<ConnectionLane> m_ConnectionLaneData;

		public void Execute(int entityIndex)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			AirwayHelpers.LaneDirection direction;
			int2 cellIndex = m_AirwayMap.GetCellIndex(entityIndex, out direction);
			switch (direction)
			{
			case AirwayHelpers.LaneDirection.HorizontalZ:
				CreateLane(entityIndex, cellIndex, new int2(cellIndex.x, cellIndex.y + 1));
				break;
			case AirwayHelpers.LaneDirection.HorizontalX:
				CreateLane(entityIndex, cellIndex, new int2(cellIndex.x + 1, cellIndex.y));
				break;
			case AirwayHelpers.LaneDirection.Diagonal:
				CreateLane(entityIndex, cellIndex, cellIndex + 1);
				break;
			case AirwayHelpers.LaneDirection.DiagonalCross:
				CreateLane(entityIndex, new int2(cellIndex.x + 1, cellIndex.y), new int2(cellIndex.x, cellIndex.y + 1));
				break;
			}
		}

		private void CreateLane(int entityIndex, int2 startNode, int2 endNode)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_AirwayMap.entities[entityIndex];
			Lane lane = default(Lane);
			lane.m_StartNode = m_AirwayMap.GetPathNode(startNode);
			lane.m_MiddleNode = new PathNode(val, 1);
			lane.m_EndNode = m_AirwayMap.GetPathNode(endNode);
			float3 nodePosition = m_AirwayMap.GetNodePosition(startNode);
			float3 nodePosition2 = m_AirwayMap.GetNodePosition(endNode);
			nodePosition.y += WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, nodePosition);
			nodePosition2.y += WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, nodePosition2);
			Curve curve = default(Curve);
			curve.m_Bezier = NetUtils.StraightCurve(nodePosition, nodePosition2);
			curve.m_Length = math.distance(curve.m_Bezier.a, curve.m_Bezier.d);
			ConnectionLane connectionLane = default(ConnectionLane);
			connectionLane.m_AccessRestriction = Entity.Null;
			connectionLane.m_Flags = ConnectionLaneFlags.AllowMiddle | ConnectionLaneFlags.Airway;
			connectionLane.m_TrackTypes = TrackTypes.None;
			connectionLane.m_RoadTypes = m_RoadType;
			m_PrefabRefData[val] = new PrefabRef(m_Prefab);
			m_LaneData[val] = lane;
			m_CurveData[val] = curve;
			m_ConnectionLaneData[val] = connectionLane;
		}
	}

	private struct TypeHandle
	{
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RW_ComponentLookup;

		public ComponentLookup<Lane> __Game_Net_Lane_RW_ComponentLookup;

		public ComponentLookup<Curve> __Game_Net_Curve_RW_ComponentLookup;

		public ComponentLookup<ConnectionLane> __Game_Net_ConnectionLane_RW_ComponentLookup;

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
			__Game_Prefabs_PrefabRef_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(false);
			__Game_Net_Lane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(false);
			__Game_Net_Curve_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(false);
			__Game_Net_ConnectionLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ConnectionLane>(false);
		}
	}

	private const float TERRAIN_SIZE = 14336f;

	private const int HELICOPTER_GRID_WIDTH = 28;

	private const int HELICOPTER_GRID_LENGTH = 28;

	private const float HELICOPTER_CELL_SIZE = 494.34482f;

	private const float HELICOPTER_PATH_HEIGHT = 200f;

	private const int AIRPLANE_GRID_WIDTH = 14;

	private const int AIRPLANE_GRID_LENGTH = 14;

	private const float AIRPLANE_CELL_SIZE = 988.68964f;

	private const float AIRPLANE_PATH_HEIGHT = 1000f;

	private LoadGameSystem m_LoadGameSystem;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private EntityQuery m_PrefabQuery;

	private EntityQuery m_AirplaneConnectionQuery;

	private EntityQuery m_OldConnectionQuery;

	private AirwayHelpers.AirwayData m_AirwayData;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_LoadGameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LoadGameSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ConnectionLaneData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		m_AirplaneConnectionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<AirplaneStop>(),
			ComponentType.ReadOnly<Game.Routes.TakeoffLocation>(),
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ConnectionLane>() };
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<OutsideConnection>(),
			ComponentType.ReadOnly<Owner>()
		};
		array[0] = val;
		m_OldConnectionQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_PrefabQuery);
		AirwayHelpers.AirwayMap helicopterMap = new AirwayHelpers.AirwayMap(new int2(28, 28), 494.34482f, 200f, (Allocator)4);
		AirwayHelpers.AirwayMap airplaneMap = new AirwayHelpers.AirwayMap(new int2(14, 14), 988.68964f, 1000f, (Allocator)4);
		m_AirwayData = new AirwayHelpers.AirwayData(helicopterMap, airplaneMap);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_AirwayData.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		Context context = m_LoadGameSystem.context;
		if ((int)((Context)(ref context)).purpose == 1 && ((EntityQuery)(ref m_OldConnectionQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val = ((EntityQuery)(ref m_PrefabQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			NetLaneArchetypeData componentData = ((EntityManager)(ref entityManager)).GetComponentData<NetLaneArchetypeData>(val[0]);
			if (!((EntityQuery)(ref m_AirplaneConnectionQuery)).IsEmptyIgnoreFilter)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponent<Updated>(m_AirplaneConnectionQuery);
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).CreateEntity(componentData.m_LaneArchetype, m_AirwayData.helicopterMap.entities);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).CreateEntity(componentData.m_LaneArchetype, m_AirwayData.airplaneMap.entities);
			TerrainHeightData heightData = m_TerrainSystem.GetHeightData(waitForPending: true);
			JobHandle deps;
			WaterSurfaceData surfaceData = m_WaterSystem.GetSurfaceData(out deps);
			GenerateAirwayLanesJob generateAirwayLanesJob = new GenerateAirwayLanesJob
			{
				m_AirwayMap = m_AirwayData.helicopterMap,
				m_Prefab = val[0],
				m_RoadType = RoadTypes.Helicopter,
				m_TerrainHeightData = heightData,
				m_WaterSurfaceData = surfaceData,
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			GenerateAirwayLanesJob obj = new GenerateAirwayLanesJob
			{
				m_AirwayMap = m_AirwayData.airplaneMap,
				m_Prefab = val[0],
				m_RoadType = RoadTypes.Airplane,
				m_TerrainHeightData = heightData,
				m_WaterSurfaceData = surfaceData,
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			JobHandle val2 = IJobParallelForExtensions.Schedule<GenerateAirwayLanesJob>(generateAirwayLanesJob, m_AirwayData.helicopterMap.entities.Length, 4, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
			JobHandle val3 = IJobParallelForExtensions.Schedule<GenerateAirwayLanesJob>(obj, m_AirwayData.airplaneMap.entities.Length, 4, val2);
			val.Dispose();
			m_TerrainSystem.AddCPUHeightReader(val3);
			m_WaterSystem.AddSurfaceReader(val3);
			((SystemBase)this).Dependency = val3;
		}
	}

	public AirwayHelpers.AirwayData GetAirwayData()
	{
		return m_AirwayData;
	}

	public JobHandle Serialize<TWriter>(EntityWriterData writerData, JobHandle inputDeps) where TWriter : struct, IWriter
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		return IJobExtensions.Schedule<SerializeJob<TWriter>>(new SerializeJob<TWriter>
		{
			m_HelicopterMap = m_AirwayData.helicopterMap,
			m_AirplaneMap = m_AirwayData.airplaneMap,
			m_WriterData = writerData
		}, inputDeps);
	}

	public JobHandle Deserialize<TReader>(EntityReaderData readerData, JobHandle inputDeps) where TReader : struct, IReader
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		return IJobExtensions.Schedule<DeserializeJob<TReader>>(new DeserializeJob<TReader>
		{
			m_HelicopterMap = m_AirwayData.helicopterMap,
			m_AirplaneMap = m_AirwayData.airplaneMap,
			m_ReaderData = readerData
		}, inputDeps);
	}

	public JobHandle SetDefaults(Context context)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		return IJobExtensions.Schedule<SetDefaultsJob>(new SetDefaultsJob
		{
			m_Context = context,
			m_HelicopterMap = m_AirwayData.helicopterMap,
			m_AirplaneMap = m_AirwayData.airplaneMap
		}, default(JobHandle));
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
	public AirwaySystem()
	{
	}
}
