using System.Runtime.CompilerServices;
using Colossal;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.Common;
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
public class WaterStatisticsSystem : GameSystemBase, IWaterStatisticsSystem, IDefaultSerializable, ISerializable
{
	[BurstCompile]
	private struct CountPumpCapacityJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<WaterPumpingStation> m_PumpType;

		public Concurrent m_Capacity;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<WaterPumpingStation> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterPumpingStation>(ref m_PumpType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				((Concurrent)(ref m_Capacity)).Add(nativeArray[i].m_Capacity);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CountOutletCapacityJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<SewageOutlet> m_OutletType;

		public Concurrent m_Capacity;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<SewageOutlet> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<SewageOutlet>(ref m_OutletType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				((Concurrent)(ref m_Capacity)).Add(nativeArray[i].m_Capacity);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CountWaterConsumptionJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<WaterConsumer> m_ConsumerType;

		public Concurrent m_Consumption;

		public Concurrent m_FulfilledFreshConsumption;

		public Concurrent m_FulfilledSewageConsumption;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<WaterConsumer> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterConsumer>(ref m_ConsumerType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				WaterConsumer waterConsumer = nativeArray[i];
				((Concurrent)(ref m_Consumption)).Add(waterConsumer.m_WantedConsumption);
				((Concurrent)(ref m_FulfilledFreshConsumption)).Add(waterConsumer.m_FulfilledFresh);
				((Concurrent)(ref m_FulfilledSewageConsumption)).Add(waterConsumer.m_FulfilledSewage);
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
		public ComponentTypeHandle<WaterPumpingStation> __Game_Buildings_WaterPumpingStation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SewageOutlet> __Game_Buildings_SewageOutlet_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaterConsumer> __Game_Buildings_WaterConsumer_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Buildings_WaterPumpingStation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterPumpingStation>(true);
			__Game_Buildings_SewageOutlet_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SewageOutlet>(true);
			__Game_Buildings_WaterConsumer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterConsumer>(true);
		}
	}

	private EntityQuery m_PumpGroup;

	private EntityQuery m_OutletGroup;

	private EntityQuery m_ConsumerGroup;

	private NativePerThreadSumInt m_FreshCapacity;

	private NativePerThreadSumInt m_SewageCapacity;

	private NativePerThreadSumInt m_Consumption;

	private NativePerThreadSumInt m_FulfilledFreshConsumption;

	private NativePerThreadSumInt m_FulfilledSewageConsumption;

	private int m_LastFreshCapacity;

	private int m_LastFreshConsumption;

	private int m_LastFulfilledFreshConsumption;

	private int m_LastSewageCapacity;

	private int m_LastSewageConsumption;

	private int m_LastFulfilledSewageConsumption;

	private TypeHandle __TypeHandle;

	public int freshCapacity => m_LastFreshCapacity;

	public int freshConsumption => m_LastFreshConsumption;

	public int fulfilledFreshConsumption => m_LastFulfilledFreshConsumption;

	public int sewageCapacity => m_LastSewageCapacity;

	public int sewageConsumption => m_LastSewageConsumption;

	public int fulfilledSewageConsumption => m_LastFulfilledSewageConsumption;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 128;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 63;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PumpGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<WaterPumpingStation>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_OutletGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<SewageOutlet>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ConsumerGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<WaterConsumer>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_FreshCapacity = new NativePerThreadSumInt((Allocator)4);
		m_SewageCapacity = new NativePerThreadSumInt((Allocator)4);
		m_Consumption = new NativePerThreadSumInt((Allocator)4);
		m_FulfilledFreshConsumption = new NativePerThreadSumInt((Allocator)4);
		m_FulfilledSewageConsumption = new NativePerThreadSumInt((Allocator)4);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((NativePerThreadSumInt)(ref m_FreshCapacity)).Dispose();
		((NativePerThreadSumInt)(ref m_SewageCapacity)).Dispose();
		((NativePerThreadSumInt)(ref m_Consumption)).Dispose();
		((NativePerThreadSumInt)(ref m_FulfilledFreshConsumption)).Dispose();
		((NativePerThreadSumInt)(ref m_FulfilledSewageConsumption)).Dispose();
		base.OnDestroy();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int num = m_LastFreshCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		int num2 = m_LastFreshConsumption;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
		int num3 = m_LastFulfilledFreshConsumption;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num3);
		int num4 = m_LastSewageCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num4);
		int num5 = m_LastSewageConsumption;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num5);
		int num6 = m_LastFulfilledSewageConsumption;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num6);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int reference = ref m_LastFreshCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		ref int reference2 = ref m_LastFreshConsumption;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
		ref int reference3 = ref m_LastFulfilledFreshConsumption;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference3);
		ref int reference4 = ref m_LastSewageCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference4);
		ref int reference5 = ref m_LastSewageConsumption;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference5);
		ref int reference6 = ref m_LastFulfilledSewageConsumption;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference6);
	}

	public void SetDefaults(Context context)
	{
		m_LastFreshCapacity = 0;
		m_LastFreshConsumption = 0;
		m_LastFulfilledFreshConsumption = 0;
		m_LastSewageCapacity = 0;
		m_LastSewageConsumption = 0;
		m_LastFulfilledSewageConsumption = 0;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		m_LastFreshCapacity = ((NativePerThreadSumInt)(ref m_FreshCapacity)).Count;
		m_LastFreshConsumption = ((NativePerThreadSumInt)(ref m_Consumption)).Count;
		m_LastFulfilledFreshConsumption = ((NativePerThreadSumInt)(ref m_FulfilledFreshConsumption)).Count;
		m_LastSewageCapacity = ((NativePerThreadSumInt)(ref m_SewageCapacity)).Count;
		m_LastSewageConsumption = ((NativePerThreadSumInt)(ref m_Consumption)).Count;
		m_LastFulfilledSewageConsumption = ((NativePerThreadSumInt)(ref m_FulfilledSewageConsumption)).Count;
		((NativePerThreadSumInt)(ref m_FreshCapacity)).Count = 0;
		((NativePerThreadSumInt)(ref m_SewageCapacity)).Count = 0;
		((NativePerThreadSumInt)(ref m_Consumption)).Count = 0;
		((NativePerThreadSumInt)(ref m_FulfilledFreshConsumption)).Count = 0;
		((NativePerThreadSumInt)(ref m_FulfilledSewageConsumption)).Count = 0;
		JobHandle val = default(JobHandle);
		if (!((EntityQuery)(ref m_PumpGroup)).IsEmptyIgnoreFilter)
		{
			val = JobChunkExtensions.ScheduleParallel<CountPumpCapacityJob>(new CountPumpCapacityJob
			{
				m_PumpType = InternalCompilerInterface.GetComponentTypeHandle<WaterPumpingStation>(ref __TypeHandle.__Game_Buildings_WaterPumpingStation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Capacity = ((NativePerThreadSumInt)(ref m_FreshCapacity)).ToConcurrent()
			}, m_PumpGroup, ((SystemBase)this).Dependency);
		}
		JobHandle val2 = default(JobHandle);
		if (!((EntityQuery)(ref m_PumpGroup)).IsEmptyIgnoreFilter)
		{
			val2 = JobChunkExtensions.ScheduleParallel<CountOutletCapacityJob>(new CountOutletCapacityJob
			{
				m_OutletType = InternalCompilerInterface.GetComponentTypeHandle<SewageOutlet>(ref __TypeHandle.__Game_Buildings_SewageOutlet_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Capacity = ((NativePerThreadSumInt)(ref m_SewageCapacity)).ToConcurrent()
			}, m_OutletGroup, ((SystemBase)this).Dependency);
		}
		JobHandle val3 = default(JobHandle);
		if (!((EntityQuery)(ref m_ConsumerGroup)).IsEmptyIgnoreFilter)
		{
			val3 = JobChunkExtensions.ScheduleParallel<CountWaterConsumptionJob>(new CountWaterConsumptionJob
			{
				m_ConsumerType = InternalCompilerInterface.GetComponentTypeHandle<WaterConsumer>(ref __TypeHandle.__Game_Buildings_WaterConsumer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Consumption = ((NativePerThreadSumInt)(ref m_Consumption)).ToConcurrent(),
				m_FulfilledFreshConsumption = ((NativePerThreadSumInt)(ref m_FulfilledFreshConsumption)).ToConcurrent(),
				m_FulfilledSewageConsumption = ((NativePerThreadSumInt)(ref m_FulfilledSewageConsumption)).ToConcurrent()
			}, m_ConsumerGroup, ((SystemBase)this).Dependency);
		}
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(val, val2, val3);
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
	public WaterStatisticsSystem()
	{
	}
}
