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
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class ElectricityTradeSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	[BurstCompile]
	private struct SumJob : IJobChunk
	{
		[ReadOnly]
		public BufferTypeHandle<ConnectedFlowEdge> m_FlowConnectionType;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowEdge> m_FlowEdges;

		public Concurrent m_Export;

		public Concurrent m_Import;

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
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<ConnectedFlowEdge> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedFlowEdge>(ref m_FlowConnectionType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				DynamicBuffer<ConnectedFlowEdge> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					ElectricityFlowEdge electricityFlowEdge = m_FlowEdges[val[j].m_Edge];
					if (electricityFlowEdge.m_End == m_SinkNode)
					{
						Assert.IsTrue(electricityFlowEdge.m_Flow >= 0);
						((Concurrent)(ref m_Export)).Add(electricityFlowEdge.m_Flow);
					}
					else if (electricityFlowEdge.m_Start == m_SourceNode)
					{
						Assert.IsTrue(electricityFlowEdge.m_Flow >= 0);
						((Concurrent)(ref m_Import)).Add(electricityFlowEdge.m_Flow);
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
	private struct ElectricityTradeJob : IJob
	{
		public NativePerThreadSumInt m_Export;

		public NativePerThreadSumInt m_Import;

		public NativeQueue<ServiceFeeSystem.FeeEvent> m_FeeQueue;

		public OutsideTradeParameterData m_OutsideTradeParameters;

		public void Execute()
		{
			float num = (float)((NativePerThreadSumInt)(ref m_Export)).Count / 2048f;
			float num2 = (float)((NativePerThreadSumInt)(ref m_Import)).Count / 2048f;
			float num3 = num * m_OutsideTradeParameters.m_ElectricityExportPrice;
			float num4 = num2 * m_OutsideTradeParameters.m_ElectricityImportPrice;
			if (num3 > 0f)
			{
				m_FeeQueue.Enqueue(new ServiceFeeSystem.FeeEvent
				{
					m_Resource = PlayerResource.Electricity,
					m_Cost = num3,
					m_Amount = num,
					m_Outside = true
				});
			}
			if (num4 > 0f)
			{
				m_FeeQueue.Enqueue(new ServiceFeeSystem.FeeEvent
				{
					m_Resource = PlayerResource.Electricity,
					m_Cost = num4,
					m_Amount = 0f - num2,
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
		public ComponentLookup<ElectricityFlowEdge> __Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			__Game_Simulation_ConnectedFlowEdge_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedFlowEdge>(true);
			__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityFlowEdge>(true);
		}
	}

	private ElectricityFlowSystem m_ElectricityFlowSystem;

	private EntityQuery m_TradeNodeGroup;

	private ServiceFeeSystem m_ServiceFeeSystem;

	private NativePerThreadSumInt m_Export;

	private NativePerThreadSumInt m_Import;

	private int m_LastExport;

	private int m_LastImport;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1233563293_0;

	public int export => m_LastExport;

	public int import => m_LastImport;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 128;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 126;
	}

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
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ElectricityFlowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ElectricityFlowSystem>();
		m_ServiceFeeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ServiceFeeSystem>();
		m_TradeNodeGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<TradeNode>(),
			ComponentType.ReadOnly<ElectricityFlowNode>(),
			ComponentType.ReadOnly<ConnectedFlowEdge>()
		});
		((ComponentSystemBase)this).RequireForUpdate<OutsideTradeParameterData>();
		m_Export = new NativePerThreadSumInt((Allocator)4);
		m_Import = new NativePerThreadSumInt((Allocator)4);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((NativePerThreadSumInt)(ref m_Export)).Dispose();
		((NativePerThreadSumInt)(ref m_Import)).Dispose();
		base.OnDestroy();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int num = m_LastExport;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		int num2 = m_LastImport;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int reference = ref m_LastExport;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		ref int reference2 = ref m_LastImport;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
	}

	public void SetDefaults(Context context)
	{
		m_LastExport = 0;
		m_LastImport = 0;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		m_LastExport = ((NativePerThreadSumInt)(ref m_Export)).Count;
		m_LastImport = ((NativePerThreadSumInt)(ref m_Import)).Count;
		((NativePerThreadSumInt)(ref m_Export)).Count = 0;
		((NativePerThreadSumInt)(ref m_Import)).Count = 0;
		if (!((EntityQuery)(ref m_TradeNodeGroup)).IsEmptyIgnoreFilter)
		{
			SumJob sumJob = new SumJob
			{
				m_FlowConnectionType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_FlowEdges = InternalCompilerInterface.GetComponentLookup<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Export = ((NativePerThreadSumInt)(ref m_Export)).ToConcurrent(),
				m_Import = ((NativePerThreadSumInt)(ref m_Import)).ToConcurrent(),
				m_SourceNode = m_ElectricityFlowSystem.sourceNode,
				m_SinkNode = m_ElectricityFlowSystem.sinkNode
			};
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<SumJob>(sumJob, m_TradeNodeGroup, ((SystemBase)this).Dependency);
			JobHandle deps;
			ElectricityTradeJob electricityTradeJob = new ElectricityTradeJob
			{
				m_Export = m_Export,
				m_Import = m_Import,
				m_FeeQueue = m_ServiceFeeSystem.GetFeeQueue(out deps),
				m_OutsideTradeParameters = ((EntityQuery)(ref __query_1233563293_0)).GetSingleton<OutsideTradeParameterData>()
			};
			((SystemBase)this).Dependency = IJobExtensions.Schedule<ElectricityTradeJob>(electricityTradeJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
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
		__query_1233563293_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public ElectricityTradeSystem()
	{
	}
}
