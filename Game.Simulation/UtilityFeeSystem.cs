using System;
using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Tools;
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
public class UtilityFeeSystem : GameSystemBase
{
	[BurstCompile]
	private struct SellUtilitiesJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<ElectricityConsumer> m_ElectricityConsumerType;

		[ReadOnly]
		public ComponentTypeHandle<WaterConsumer> m_WaterConsumerType;

		[ReadOnly]
		public BufferTypeHandle<Renter> m_RenterType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Resources> m_Resources;

		[ReadOnly]
		public BufferLookup<ServiceFee> m_Fees;

		public Entity m_City;

		public uint m_UpdateFrameIndex;

		public ParallelWriter<ServiceFeeSystem.FeeEvent> m_FeeQueue;

		public RandomSeed m_RandomSeed;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			Random random = m_RandomSeed.GetRandom(1 + unfilteredChunkIndex);
			((Random)(ref random)).NextUInt();
			NativeArray<ElectricityConsumer> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityConsumer>(ref m_ElectricityConsumerType);
			NativeArray<WaterConsumer> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterConsumer>(ref m_WaterConsumerType);
			BufferAccessor<Renter> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
			bool flag = nativeArray.Length != 0;
			bool flag2 = nativeArray2.Length != 0;
			float fee = ServiceFeeSystem.GetFee(PlayerResource.Water, m_Fees[m_City]);
			float fee2 = ServiceFeeSystem.GetFee(PlayerResource.Water, m_Fees[m_City]);
			float3 val = default(float3);
			((float3)(ref val))._002Ector(fee, fee2, fee2);
			float3 val2 = float3.op_Implicit(0);
			float3 val4 = default(float3);
			DynamicBuffer<Resources> resources = default(DynamicBuffer<Resources>);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				DynamicBuffer<Renter> val3 = bufferAccessor[i];
				if (val3.Length <= 0)
				{
					continue;
				}
				((float3)(ref val4))._002Ector(flag ? ((float)nativeArray[i].m_FulfilledConsumption) : 0f, flag2 ? ((float)nativeArray2[i].m_FulfilledFresh) : 0f, flag2 ? ((float)nativeArray2[i].m_FulfilledSewage) : 0f);
				val4 /= 128f;
				val2 += val4;
				float num = math.csum(val4 * val) / (float)val3.Length;
				Enumerator<Renter> enumerator = val3.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Renter current = enumerator.Current;
						if (m_Resources.TryGetBuffer(current.m_Renter, ref resources))
						{
							int num2 = MathUtils.RoundToIntRandom(ref random, num);
							EconomyUtils.AddResources(Resource.Money, -num2, resources);
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
			float3 val5 = val2 * val;
			EnqueueFeeEvent(PlayerResource.Electricity, val2.x, val5.x);
			EnqueueFeeEvent(PlayerResource.Water, val2.y, val5.y);
			EnqueueFeeEvent(PlayerResource.Sewage, val2.z, val5.z);
		}

		private void EnqueueFeeEvent(PlayerResource resource, float totalSold, float totalIncome)
		{
			m_FeeQueue.Enqueue(new ServiceFeeSystem.FeeEvent
			{
				m_Resource = resource,
				m_Amount = totalSold,
				m_Cost = totalIncome,
				m_Outside = false
			});
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferTypeHandle<Renter> __Game_Buildings_Renter_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaterConsumer> __Game_Buildings_WaterConsumer_RO_ComponentTypeHandle;

		public BufferLookup<Resources> __Game_Economy_Resources_RW_BufferLookup;

		[ReadOnly]
		public BufferLookup<ServiceFee> __Game_City_ServiceFee_RO_BufferLookup;

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
			__Game_Buildings_Renter_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Renter>(true);
			__Game_Buildings_ElectricityConsumer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityConsumer>(true);
			__Game_Buildings_WaterConsumer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterConsumer>(true);
			__Game_Economy_Resources_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(false);
			__Game_City_ServiceFee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceFee>(true);
		}
	}

	private const int kUpdatesPerDay = 128;

	private SimulationSystem m_SimulationSystem;

	private CitySystem m_CitySystem;

	private ServiceFeeSystem m_ServiceFeeSystem;

	private EntityQuery m_ConsumerGroup;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 128;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_ServiceFeeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ServiceFeeSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Renter>(),
			ComponentType.ReadOnly<UpdateFrame>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ElectricityConsumer>(),
			ComponentType.ReadOnly<WaterConsumer>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_ConsumerGroup = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, 128, 16);
		JobHandle deps;
		SellUtilitiesJob sellUtilitiesJob = new SellUtilitiesJob
		{
			m_RenterType = InternalCompilerInterface.GetBufferTypeHandle<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityConsumerType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterConsumerType = InternalCompilerInterface.GetComponentTypeHandle<WaterConsumer>(ref __TypeHandle.__Game_Buildings_WaterConsumer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = ((ComponentSystemBase)this).GetSharedComponentTypeHandle<UpdateFrame>(),
			m_Resources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Fees = InternalCompilerInterface.GetBufferLookup<ServiceFee>(ref __TypeHandle.__Game_City_ServiceFee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_City = m_CitySystem.City,
			m_UpdateFrameIndex = updateFrame,
			m_FeeQueue = m_ServiceFeeSystem.GetFeeQueue(out deps).AsParallelWriter(),
			m_RandomSeed = RandomSeed.Next()
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<SellUtilitiesJob>(sellUtilitiesJob, m_ConsumerGroup, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		m_ServiceFeeSystem.AddQueueWriter(((SystemBase)this).Dependency);
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
	public UtilityFeeSystem()
	{
	}
}
