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
public class ElectricityStatisticsSystem : GameSystemBase, IElectricityStatisticsSystem, IDefaultSerializable, ISerializable
{
	[BurstCompile]
	private struct CountElectricityProductionJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<ElectricityProducer> m_ProducerType;

		public Concurrent m_Production;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<ElectricityProducer> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityProducer>(ref m_ProducerType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				ElectricityProducer electricityProducer = nativeArray[i];
				((Concurrent)(ref m_Production)).Add(electricityProducer.m_Capacity);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CountElectricityConsumptionJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<ElectricityConsumer> m_ConsumerType;

		public Concurrent m_Consumption;

		public Concurrent m_FulfilledConsumption;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<ElectricityConsumer> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityConsumer>(ref m_ConsumerType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				ElectricityConsumer electricityConsumer = nativeArray[i];
				((Concurrent)(ref m_Consumption)).Add(electricityConsumer.m_WantedConsumption);
				((Concurrent)(ref m_FulfilledConsumption)).Add(electricityConsumer.m_FulfilledConsumption);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CountBatteryCapacityJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Battery> m_BatteryType;

		public Concurrent m_Charge;

		public Concurrent m_Capacity;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Battery> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Battery>(ref m_BatteryType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Battery battery = nativeArray[i];
				((Concurrent)(ref m_Charge)).Add(battery.storedEnergyHours);
				((Concurrent)(ref m_Capacity)).Add(battery.m_Capacity);
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
		public ComponentTypeHandle<ElectricityProducer> __Game_Buildings_ElectricityProducer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Battery> __Game_Buildings_Battery_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Buildings_ElectricityProducer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityProducer>(true);
			__Game_Buildings_ElectricityConsumer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityConsumer>(true);
			__Game_Buildings_Battery_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Battery>(true);
		}
	}

	private EntityQuery m_ProducerGroup;

	private EntityQuery m_ConsumerGroup;

	private EntityQuery m_BatteryGroup;

	private NativePerThreadSumInt m_Production;

	private NativePerThreadSumInt m_Consumption;

	private NativePerThreadSumInt m_FulfilledConsumption;

	private NativePerThreadSumInt m_BatteryCharge;

	private NativePerThreadSumInt m_BatteryCapacity;

	private int m_LastProduction;

	private int m_LastConsumption;

	private int m_LastFulfilledConsumption;

	private int m_LastBatteryCharge;

	private int m_LastBatteryCapacity;

	private TypeHandle __TypeHandle;

	public int production => m_LastProduction;

	public int consumption => m_LastConsumption;

	public int fulfilledConsumption => m_LastFulfilledConsumption;

	public int batteryCharge => m_LastBatteryCharge;

	public int batteryCapacity => m_LastBatteryCapacity;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 128;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 127;
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
		m_ProducerGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ElectricityProducer>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ConsumerGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ElectricityConsumer>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_BatteryGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Battery>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_Production = new NativePerThreadSumInt((Allocator)4);
		m_Consumption = new NativePerThreadSumInt((Allocator)4);
		m_FulfilledConsumption = new NativePerThreadSumInt((Allocator)4);
		m_BatteryCharge = new NativePerThreadSumInt((Allocator)4);
		m_BatteryCapacity = new NativePerThreadSumInt((Allocator)4);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((NativePerThreadSumInt)(ref m_Production)).Dispose();
		((NativePerThreadSumInt)(ref m_Consumption)).Dispose();
		((NativePerThreadSumInt)(ref m_FulfilledConsumption)).Dispose();
		((NativePerThreadSumInt)(ref m_BatteryCharge)).Dispose();
		((NativePerThreadSumInt)(ref m_BatteryCapacity)).Dispose();
		base.OnDestroy();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int num = m_LastProduction;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		int num2 = m_LastConsumption;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
		int num3 = m_LastFulfilledConsumption;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num3);
		int num4 = m_LastBatteryCharge;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num4);
		int num5 = m_LastBatteryCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num5);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.electricityStats)
		{
			ref int reference = ref m_LastProduction;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
			ref int reference2 = ref m_LastConsumption;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
			ref int reference3 = ref m_LastFulfilledConsumption;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference3);
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version >= Version.batteryStats)
			{
				ref int reference4 = ref m_LastBatteryCharge;
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference4);
				ref int reference5 = ref m_LastBatteryCapacity;
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference5);
			}
			return;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version > Version.seekerReferences)
		{
			float num = default(float);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			ref int reference6 = ref m_LastConsumption;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference6);
			ref int reference7 = ref m_LastProduction;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference7);
			int num2 = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version > Version.transmittedElectricity)
			{
				ref int reference8 = ref m_LastFulfilledConsumption;
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference8);
			}
		}
	}

	public void SetDefaults(Context context)
	{
		m_LastProduction = 0;
		m_LastConsumption = 0;
		m_LastFulfilledConsumption = 0;
		m_LastBatteryCharge = 0;
		m_LastBatteryCapacity = 0;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		m_LastProduction = ((NativePerThreadSumInt)(ref m_Production)).Count;
		m_LastConsumption = ((NativePerThreadSumInt)(ref m_Consumption)).Count;
		m_LastFulfilledConsumption = ((NativePerThreadSumInt)(ref m_FulfilledConsumption)).Count;
		m_LastBatteryCharge = ((NativePerThreadSumInt)(ref m_BatteryCharge)).Count;
		m_LastBatteryCapacity = ((NativePerThreadSumInt)(ref m_BatteryCapacity)).Count;
		((NativePerThreadSumInt)(ref m_Production)).Count = 0;
		((NativePerThreadSumInt)(ref m_Consumption)).Count = 0;
		((NativePerThreadSumInt)(ref m_FulfilledConsumption)).Count = 0;
		((NativePerThreadSumInt)(ref m_BatteryCharge)).Count = 0;
		((NativePerThreadSumInt)(ref m_BatteryCapacity)).Count = 0;
		JobHandle val = default(JobHandle);
		if (!((EntityQuery)(ref m_ProducerGroup)).IsEmptyIgnoreFilter)
		{
			val = JobChunkExtensions.ScheduleParallel<CountElectricityProductionJob>(new CountElectricityProductionJob
			{
				m_ProducerType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityProducer>(ref __TypeHandle.__Game_Buildings_ElectricityProducer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Production = ((NativePerThreadSumInt)(ref m_Production)).ToConcurrent()
			}, m_ProducerGroup, ((SystemBase)this).Dependency);
		}
		JobHandle val2 = default(JobHandle);
		if (!((EntityQuery)(ref m_ConsumerGroup)).IsEmptyIgnoreFilter)
		{
			val2 = JobChunkExtensions.ScheduleParallel<CountElectricityConsumptionJob>(new CountElectricityConsumptionJob
			{
				m_ConsumerType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Consumption = ((NativePerThreadSumInt)(ref m_Consumption)).ToConcurrent(),
				m_FulfilledConsumption = ((NativePerThreadSumInt)(ref m_FulfilledConsumption)).ToConcurrent()
			}, m_ConsumerGroup, ((SystemBase)this).Dependency);
		}
		JobHandle val3 = default(JobHandle);
		if (!((EntityQuery)(ref m_ConsumerGroup)).IsEmptyIgnoreFilter)
		{
			val3 = JobChunkExtensions.ScheduleParallel<CountBatteryCapacityJob>(new CountBatteryCapacityJob
			{
				m_BatteryType = InternalCompilerInterface.GetComponentTypeHandle<Battery>(ref __TypeHandle.__Game_Buildings_Battery_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Charge = ((NativePerThreadSumInt)(ref m_BatteryCharge)).ToConcurrent(),
				m_Capacity = ((NativePerThreadSumInt)(ref m_BatteryCapacity)).ToConcurrent()
			}, m_BatteryGroup, ((SystemBase)this).Dependency);
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
	public ElectricityStatisticsSystem()
	{
	}
}
