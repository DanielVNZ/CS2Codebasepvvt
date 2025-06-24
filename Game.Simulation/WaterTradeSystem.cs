using System.Runtime.CompilerServices;
using Colossal;
using Colossal.Serialization.Entities;
using Game.City;
using Game.Prefabs;
using Unity.Assertions;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class WaterTradeSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	[BurstCompile]
	private struct SumJob : IJobChunk
	{
		[ReadOnly]
		public BufferTypeHandle<ConnectedFlowEdge> m_FlowConnectionType;

		[ReadOnly]
		public ComponentLookup<WaterPipeEdge> m_FlowEdges;

		public Concurrent m_FreshExport;

		public Concurrent m_PollutedExport;

		public Concurrent m_FreshImport;

		public Concurrent m_SewageExport;

		public OutsideTradeParameterData m_OutsideTradeParameters;

		public Entity m_SourceNode;

		public Entity m_SinkNode;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<ConnectedFlowEdge> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedFlowEdge>(ref m_FlowConnectionType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				DynamicBuffer<ConnectedFlowEdge> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					WaterPipeEdge waterPipeEdge = m_FlowEdges[val[j].m_Edge];
					if (waterPipeEdge.m_End == m_SinkNode)
					{
						Assert.IsTrue(waterPipeEdge.m_FreshFlow >= 0);
						Assert.AreEqual(0, waterPipeEdge.m_SewageFlow);
						((Concurrent)(ref m_FreshExport)).Add(waterPipeEdge.m_FreshFlow);
						float num = waterPipeEdge.m_FreshPollution / m_OutsideTradeParameters.m_WaterExportPollutionTolerance;
						((Concurrent)(ref m_PollutedExport)).Add(math.min((int)math.round(num * (float)waterPipeEdge.m_FreshFlow), waterPipeEdge.m_FreshFlow));
					}
					else if (waterPipeEdge.m_Start == m_SourceNode)
					{
						Assert.IsTrue(waterPipeEdge.m_FreshFlow >= 0);
						Assert.IsTrue(waterPipeEdge.m_SewageFlow >= 0);
						((Concurrent)(ref m_FreshImport)).Add(waterPipeEdge.m_FreshFlow);
						((Concurrent)(ref m_SewageExport)).Add(waterPipeEdge.m_SewageFlow);
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
	private struct WaterTradeJob : IJob
	{
		public int m_AvailableWater;

		public NativePerThreadSumInt m_FreshExport;

		public NativePerThreadSumInt m_PollutedExport;

		public NativePerThreadSumInt m_FreshImport;

		public NativePerThreadSumInt m_SewageExport;

		public NativeQueue<ServiceFeeSystem.FeeEvent> m_FeeQueue;

		public OutsideTradeParameterData m_OutsideTradeParameters;

		public void Execute()
		{
			((NativePerThreadSumInt)(ref m_FreshExport)).Count = math.max(math.min(m_AvailableWater, ((NativePerThreadSumInt)(ref m_FreshExport)).Count), 0);
			float num = (float)((NativePerThreadSumInt)(ref m_FreshExport)).Count / 2048f;
			float num2 = (float)((NativePerThreadSumInt)(ref m_PollutedExport)).Count / 2048f;
			float num3 = (float)((NativePerThreadSumInt)(ref m_FreshImport)).Count / 2048f;
			float num4 = (float)((NativePerThreadSumInt)(ref m_SewageExport)).Count / 2048f;
			float num5 = (num - num2) * m_OutsideTradeParameters.m_WaterExportPrice;
			float num6 = num3 * m_OutsideTradeParameters.m_WaterImportPrice;
			float num7 = num4 * m_OutsideTradeParameters.m_SewageExportPrice;
			if (num5 > 0f)
			{
				m_FeeQueue.Enqueue(new ServiceFeeSystem.FeeEvent
				{
					m_Resource = PlayerResource.Water,
					m_Cost = num5,
					m_Amount = num,
					m_Outside = true
				});
			}
			if (num6 > 0f)
			{
				m_FeeQueue.Enqueue(new ServiceFeeSystem.FeeEvent
				{
					m_Resource = PlayerResource.Water,
					m_Cost = num6,
					m_Amount = 0f - num3,
					m_Outside = true
				});
			}
			if (num7 > 0f)
			{
				m_FeeQueue.Enqueue(new ServiceFeeSystem.FeeEvent
				{
					m_Resource = PlayerResource.Sewage,
					m_Cost = num7,
					m_Amount = 0f - num4,
					m_Outside = true
				});
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferTypeHandle<ConnectedFlowEdge> __Game_Simulation_ConnectedFlowEdge_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<WaterPipeEdge> __Game_Simulation_WaterPipeEdge_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			__Game_Simulation_ConnectedFlowEdge_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedFlowEdge>(true);
			__Game_Simulation_WaterPipeEdge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeEdge>(true);
		}
	}

	private WaterPipeFlowSystem m_WaterPipeFlowSystem;

	private WaterStatisticsSystem m_WaterStatisticsSystem;

	private EntityQuery m_TradeNodeGroup;

	private ServiceFeeSystem m_ServiceFeeSystem;

	private NativePerThreadSumInt m_FreshExport;

	private NativePerThreadSumInt m_PollutedExport;

	private NativePerThreadSumInt m_FreshImport;

	private NativePerThreadSumInt m_SewageExport;

	private int m_LastFreshExport;

	private int m_LastFreshImport;

	private int m_LastSewageExport;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1457460959_0;

	public int freshExport => m_LastFreshExport;

	public int freshImport => m_LastFreshImport;

	public int sewageExport => m_LastSewageExport;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 128;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 62;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_WaterPipeFlowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterPipeFlowSystem>();
		m_ServiceFeeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ServiceFeeSystem>();
		m_WaterStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterStatisticsSystem>();
		m_TradeNodeGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<TradeNode>(),
			ComponentType.ReadOnly<WaterPipeNode>(),
			ComponentType.ReadOnly<ConnectedFlowEdge>()
		});
		((ComponentSystemBase)this).RequireForUpdate<OutsideTradeParameterData>();
		m_FreshExport = new NativePerThreadSumInt((Allocator)4);
		m_PollutedExport = new NativePerThreadSumInt((Allocator)4);
		m_FreshImport = new NativePerThreadSumInt((Allocator)4);
		m_SewageExport = new NativePerThreadSumInt((Allocator)4);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((NativePerThreadSumInt)(ref m_FreshExport)).Dispose();
		((NativePerThreadSumInt)(ref m_PollutedExport)).Dispose();
		((NativePerThreadSumInt)(ref m_FreshImport)).Dispose();
		((NativePerThreadSumInt)(ref m_SewageExport)).Dispose();
		base.OnDestroy();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int num = m_LastFreshExport;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		int num2 = m_LastFreshImport;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
		int num3 = m_LastSewageExport;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num3);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int reference = ref m_LastFreshExport;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		ref int reference2 = ref m_LastFreshImport;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
		ref int reference3 = ref m_LastSewageExport;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference3);
	}

	public void SetDefaults(Context context)
	{
		m_LastFreshExport = 0;
		m_LastFreshImport = 0;
		m_LastSewageExport = 0;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		m_LastFreshExport = ((NativePerThreadSumInt)(ref m_FreshExport)).Count;
		m_LastFreshImport = ((NativePerThreadSumInt)(ref m_FreshImport)).Count;
		m_LastSewageExport = ((NativePerThreadSumInt)(ref m_SewageExport)).Count;
		int availableWater = m_WaterStatisticsSystem.freshCapacity - m_WaterStatisticsSystem.freshConsumption;
		((NativePerThreadSumInt)(ref m_FreshExport)).Count = 0;
		((NativePerThreadSumInt)(ref m_PollutedExport)).Count = 0;
		((NativePerThreadSumInt)(ref m_FreshImport)).Count = 0;
		((NativePerThreadSumInt)(ref m_SewageExport)).Count = 0;
		if (!((EntityQuery)(ref m_TradeNodeGroup)).IsEmptyIgnoreFilter)
		{
			OutsideTradeParameterData singleton = ((EntityQuery)(ref __query_1457460959_0)).GetSingleton<OutsideTradeParameterData>();
			SumJob sumJob = new SumJob
			{
				m_FlowConnectionType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_FlowEdges = InternalCompilerInterface.GetComponentLookup<WaterPipeEdge>(ref __TypeHandle.__Game_Simulation_WaterPipeEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_FreshExport = ((NativePerThreadSumInt)(ref m_FreshExport)).ToConcurrent(),
				m_PollutedExport = ((NativePerThreadSumInt)(ref m_PollutedExport)).ToConcurrent(),
				m_FreshImport = ((NativePerThreadSumInt)(ref m_FreshImport)).ToConcurrent(),
				m_SewageExport = ((NativePerThreadSumInt)(ref m_SewageExport)).ToConcurrent(),
				m_OutsideTradeParameters = singleton,
				m_SourceNode = m_WaterPipeFlowSystem.sourceNode,
				m_SinkNode = m_WaterPipeFlowSystem.sinkNode
			};
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<SumJob>(sumJob, m_TradeNodeGroup, ((SystemBase)this).Dependency);
			JobHandle deps;
			WaterTradeJob waterTradeJob = new WaterTradeJob
			{
				m_AvailableWater = availableWater,
				m_FreshExport = m_FreshExport,
				m_PollutedExport = m_PollutedExport,
				m_FreshImport = m_FreshImport,
				m_SewageExport = m_SewageExport,
				m_FeeQueue = m_ServiceFeeSystem.GetFeeQueue(out deps),
				m_OutsideTradeParameters = singleton
			};
			((SystemBase)this).Dependency = IJobExtensions.Schedule<WaterTradeJob>(waterTradeJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
			m_ServiceFeeSystem.AddQueueWriter(((SystemBase)this).Dependency);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<OutsideTradeParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1457460959_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public WaterTradeSystem()
	{
	}
}
