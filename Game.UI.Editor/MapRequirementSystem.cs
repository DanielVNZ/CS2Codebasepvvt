using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Routes;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.Editor;

[CompilerGenerated]
public class MapRequirementSystem : GameSystemBase
{
	[BurstCompile]
	public struct CollectResourcesJob : IJob
	{
		[ReadOnly]
		[DeallocateOnJobCompletion]
		public NativeArray<ArchetypeChunk> m_AreaChunks;

		[ReadOnly]
		public BufferTypeHandle<MapFeatureElement> m_MapFeatureElementType;

		public NativeArray<bool> m_Results;

		public void Execute()
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_Results.Length; i++)
			{
				m_Results[i] = false;
			}
			for (int j = 0; j < m_AreaChunks.Length; j++)
			{
				ArchetypeChunk val = m_AreaChunks[j];
				Check(((ArchetypeChunk)(ref val)).GetBufferAccessor<MapFeatureElement>(ref m_MapFeatureElementType));
			}
		}

		private void Check(BufferAccessor<MapFeatureElement> mapFeatureAccessor)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < mapFeatureAccessor.Length; i++)
			{
				DynamicBuffer<MapFeatureElement> val = mapFeatureAccessor[i];
				for (int j = 0; j < 9; j++)
				{
					ref NativeArray<bool> reference = ref m_Results;
					int num = j;
					reference[num] |= val[j].m_Amount > 0f;
				}
			}
		}
	}

	[BurstCompile]
	public struct CollectStartingResourcesJob : IJob
	{
		[ReadOnly]
		public NativeArray<Entity> m_StartingTiles;

		[ReadOnly]
		public BufferLookup<MapFeatureElement> m_MapFeatureElements;

		public NativeArray<bool> m_Results;

		public void Execute()
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_Results.Length; i++)
			{
				m_Results[i] = false;
			}
			for (int j = 0; j < m_StartingTiles.Length; j++)
			{
				Check(m_StartingTiles[j]);
			}
		}

		private void Check(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<MapFeatureElement> val = m_MapFeatureElements[entity];
			for (int i = 0; i < 9; i++)
			{
				ref NativeArray<bool> reference = ref m_Results;
				int num = i;
				reference[num] |= val[i].m_Amount > 0f;
			}
		}
	}

	[BurstCompile]
	public struct CheckWaterJob : IJob
	{
		[ReadOnly]
		public WaterSurfaceData m_SurfaceData;

		[ReadOnly]
		public NativeArray<Entity> m_StartingTiles;

		[ReadOnly]
		public ComponentLookup<Geometry> m_GeometryData;

		public NativeValue<bool> m_Result;

		public void Execute()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			m_Result.value = false;
			for (int i = 0; i < m_StartingTiles.Length; i++)
			{
				if (HasWater(m_GeometryData[m_StartingTiles[i]].m_Bounds))
				{
					m_Result.value = true;
					break;
				}
			}
		}

		private bool HasWater(Bounds3 bounds)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			float3 val = WaterUtils.ToSurfaceSpace(ref m_SurfaceData, bounds.min);
			int2 val2 = (int2)math.floor(((float3)(ref val)).xz);
			int2 zero = int2.zero;
			int3 resolution = m_SurfaceData.resolution;
			int2 val3 = math.clamp(val2, zero, ((int3)(ref resolution)).xz);
			val = WaterUtils.ToSurfaceSpace(ref m_SurfaceData, bounds.max);
			int2 val4 = (int2)math.floor(((float3)(ref val)).xz);
			int2 zero2 = int2.zero;
			resolution = m_SurfaceData.resolution;
			int2 val5 = math.clamp(val4, zero2, ((int3)(ref resolution)).xz);
			for (int i = val3.y; i < val5.y; i++)
			{
				for (int j = val3.x; j < val5.x; j++)
				{
					if (m_SurfaceData.depths[i * m_SurfaceData.resolution.x + j].m_Depth > 0f)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Geometry> __Game_Areas_Geometry_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<MapFeatureElement> __Game_Areas_MapFeatureElement_RO_BufferLookup;

		[ReadOnly]
		public BufferTypeHandle<MapFeatureElement> __Game_Areas_MapFeatureElement_RO_BufferTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Areas_Geometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Geometry>(true);
			__Game_Areas_MapFeatureElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MapFeatureElement>(true);
			__Game_Areas_MapFeatureElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<MapFeatureElement>(true);
		}
	}

	private MapTileSystem m_MapTileSystem;

	private WaterSystem m_WaterSystem;

	private EntityQuery m_TileQuery;

	private EntityQuery m_OutsideRoadNodeQuery;

	private EntityQuery m_OutsideTrainNodeQuery;

	private EntityQuery m_OutsideAirNodeQuery;

	private EntityQuery m_OutsideElectricityConnectionQuery;

	private JobHandle m_ResultDependency;

	private NativeValue<bool> m_WaterResult;

	private NativeArray<bool> m_StartingAreaResources;

	private NativeArray<bool> m_MapResources;

	private TypeHandle __TypeHandle;

	public bool hasStartingArea { get; private set; }

	public bool roadConnection { get; private set; }

	public bool trainConnection { get; private set; }

	public bool airConnection { get; private set; }

	public bool electricityConnection { get; private set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Expected O, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Expected O, but got Unknown
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Expected O, but got Unknown
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_MapTileSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MapTileSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<MapTile>(),
			ComponentType.ReadOnly<Geometry>(),
			ComponentType.ReadOnly<MapFeatureElement>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_TileQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Net.Node>(),
			ComponentType.ReadOnly<Road>(),
			ComponentType.ReadOnly<Game.Net.OutsideConnection>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array2[0] = val;
		m_OutsideRoadNodeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		EntityQueryDesc[] array3 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Net.Node>(),
			ComponentType.ReadOnly<TrainTrack>(),
			ComponentType.ReadOnly<Game.Net.OutsideConnection>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array3[0] = val;
		m_OutsideTrainNodeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array3);
		EntityQueryDesc[] array4 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<AirplaneStop>(),
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array4[0] = val;
		m_OutsideAirNodeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array4);
		EntityQueryDesc[] array5 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ElectricityOutsideConnection>() };
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array5[0] = val;
		m_OutsideElectricityConnectionQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array5);
		m_WaterResult = new NativeValue<bool>((Allocator)4);
		m_StartingAreaResources = new NativeArray<bool>(9, (Allocator)4, (NativeArrayOptions)1);
		m_MapResources = new NativeArray<bool>(9, (Allocator)4, (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_ResultDependency)).Complete();
		NativeArray<Entity> startingTiles = m_MapTileSystem.GetStartTiles().ToArray(AllocatorHandle.op_Implicit((Allocator)3));
		hasStartingArea = startingTiles.Length != 0;
		roadConnection = !((EntityQuery)(ref m_OutsideRoadNodeQuery)).IsEmptyIgnoreFilter;
		trainConnection = !((EntityQuery)(ref m_OutsideTrainNodeQuery)).IsEmptyIgnoreFilter;
		airConnection = !((EntityQuery)(ref m_OutsideAirNodeQuery)).IsEmptyIgnoreFilter;
		electricityConnection = !((EntityQuery)(ref m_OutsideElectricityConnectionQuery)).IsEmptyIgnoreFilter;
		JobHandle deps;
		CheckWaterJob checkWaterJob = new CheckWaterJob
		{
			m_Result = m_WaterResult,
			m_SurfaceData = m_WaterSystem.GetSurfaceData(out deps),
			m_StartingTiles = startingTiles,
			m_GeometryData = InternalCompilerInterface.GetComponentLookup<Geometry>(ref __TypeHandle.__Game_Areas_Geometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<CheckWaterJob>(checkWaterJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		m_WaterSystem.AddSurfaceReader(((SystemBase)this).Dependency);
		CollectStartingResourcesJob collectStartingResourcesJob = new CollectStartingResourcesJob
		{
			m_StartingTiles = startingTiles,
			m_MapFeatureElements = InternalCompilerInterface.GetBufferLookup<MapFeatureElement>(ref __TypeHandle.__Game_Areas_MapFeatureElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_StartingAreaResources
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<CollectStartingResourcesJob>(collectStartingResourcesJob, ((SystemBase)this).Dependency);
		startingTiles.Dispose(((SystemBase)this).Dependency);
		CollectResourcesJob collectResourcesJob = new CollectResourcesJob
		{
			m_AreaChunks = ((EntityQuery)(ref m_TileQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3)),
			m_MapFeatureElementType = InternalCompilerInterface.GetBufferTypeHandle<MapFeatureElement>(ref __TypeHandle.__Game_Areas_MapFeatureElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_MapResources
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<CollectResourcesJob>(collectResourcesJob, ((SystemBase)this).Dependency);
		m_ResultDependency = ((SystemBase)this).Dependency;
	}

	public bool StartingAreaHasResource(MapFeature feature)
	{
		((JobHandle)(ref m_ResultDependency)).Complete();
		switch (feature)
		{
		case MapFeature.SurfaceWater:
			if (!m_StartingAreaResources[(int)feature])
			{
				return m_WaterResult.value;
			}
			return true;
		case MapFeature.Area:
		case MapFeature.BuildableLand:
		case MapFeature.FertileLand:
		case MapFeature.Forest:
		case MapFeature.Oil:
		case MapFeature.Ore:
		case MapFeature.GroundWater:
		case MapFeature.Fish:
			return m_StartingAreaResources[(int)feature];
		default:
			return false;
		}
	}

	public bool MapHasResource(MapFeature feature)
	{
		((JobHandle)(ref m_ResultDependency)).Complete();
		if (feature > MapFeature.None && feature < MapFeature.Count)
		{
			return m_MapResources[(int)feature];
		}
		return false;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_WaterResult.Dispose();
		m_StartingAreaResources.Dispose();
		m_MapResources.Dispose();
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
	public MapRequirementSystem()
	{
	}
}
