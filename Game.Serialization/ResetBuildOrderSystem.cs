using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Net;
using Game.Tools;
using Game.Zones;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Serialization;

[CompilerGenerated]
public class ResetBuildOrderSystem : GameSystemBase
{
	[BurstCompile]
	private struct ResetBuildOrderJob : IJob
	{
		private struct OrderItem : IComparable<OrderItem>
		{
			public uint m_Min;

			public uint m_Max;

			public OrderItem(uint min, uint max)
			{
				m_Min = min;
				m_Max = max;
			}

			public OrderItem(Game.Net.BuildOrder buildOrder)
			{
				m_Min = math.min(buildOrder.m_Start, buildOrder.m_End);
				m_Max = math.max(buildOrder.m_Start, buildOrder.m_End);
			}

			public OrderItem(Game.Zones.BuildOrder buildOrder)
			{
				m_Min = buildOrder.m_Order;
				m_Max = buildOrder.m_Order;
			}

			public int CompareTo(OrderItem other)
			{
				return math.select(0, math.select(-1, 1, m_Min > other.m_Min), m_Min != other.m_Min);
			}
		}

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		public ComponentTypeHandle<Game.Net.BuildOrder> m_NetBuildOrderType;

		public ComponentTypeHandle<Game.Zones.BuildOrder> m_ZoneBuildOrderType;

		public NativeValue<uint> m_BuildOrder;

		public void Execute()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				num += ((ArchetypeChunk)(ref val)).Count;
			}
			if (num != 0)
			{
				NativeArray<OrderItem> val2 = default(NativeArray<OrderItem>);
				val2._002Ector(num, (Allocator)2, (NativeArrayOptions)1);
				NativeParallelHashMap<uint, uint> val3 = default(NativeParallelHashMap<uint, uint>);
				val3._002Ector(num, AllocatorHandle.op_Implicit((Allocator)2));
				num = 0;
				for (int j = 0; j < m_Chunks.Length; j++)
				{
					ArchetypeChunk val4 = m_Chunks[j];
					NativeArray<Game.Net.BuildOrder> nativeArray = ((ArchetypeChunk)(ref val4)).GetNativeArray<Game.Net.BuildOrder>(ref m_NetBuildOrderType);
					NativeArray<Game.Zones.BuildOrder> nativeArray2 = ((ArchetypeChunk)(ref val4)).GetNativeArray<Game.Zones.BuildOrder>(ref m_ZoneBuildOrderType);
					for (int k = 0; k < nativeArray.Length; k++)
					{
						val2[num++] = new OrderItem(nativeArray[k]);
					}
					for (int l = 0; l < nativeArray2.Length; l++)
					{
						val2[num++] = new OrderItem(nativeArray2[l]);
					}
				}
				NativeSortExtension.Sort<OrderItem>(val2);
				OrderItem orderItem = val2[0];
				OrderItem orderItem2 = new OrderItem(0u, orderItem.m_Max - orderItem.m_Min);
				val3.TryAdd(orderItem.m_Min, orderItem2.m_Min);
				for (int m = 1; m < num; m++)
				{
					OrderItem orderItem3 = val2[m];
					if (orderItem3.m_Min <= orderItem.m_Max)
					{
						if (orderItem3.m_Max > orderItem.m_Max)
						{
							orderItem.m_Max = orderItem3.m_Max;
							orderItem2.m_Max = orderItem2.m_Min + (orderItem3.m_Max - orderItem.m_Min);
						}
						if (orderItem3.m_Min > orderItem.m_Min)
						{
							val3.TryAdd(orderItem3.m_Min, orderItem2.m_Min + (orderItem3.m_Min - orderItem.m_Min));
						}
					}
					else
					{
						orderItem = orderItem3;
						orderItem2.m_Min = orderItem2.m_Max + 1;
						orderItem2.m_Max = orderItem2.m_Min + (orderItem3.m_Max - orderItem3.m_Min);
						val3.TryAdd(orderItem3.m_Min, orderItem2.m_Min);
					}
				}
				Game.Net.BuildOrder buildOrder2 = default(Game.Net.BuildOrder);
				for (int n = 0; n < m_Chunks.Length; n++)
				{
					ArchetypeChunk val5 = m_Chunks[n];
					NativeArray<Game.Net.BuildOrder> nativeArray3 = ((ArchetypeChunk)(ref val5)).GetNativeArray<Game.Net.BuildOrder>(ref m_NetBuildOrderType);
					NativeArray<Game.Zones.BuildOrder> nativeArray4 = ((ArchetypeChunk)(ref val5)).GetNativeArray<Game.Zones.BuildOrder>(ref m_ZoneBuildOrderType);
					for (int num2 = 0; num2 < nativeArray3.Length; num2++)
					{
						Game.Net.BuildOrder buildOrder = nativeArray3[num2];
						if (buildOrder.m_End >= buildOrder.m_Start)
						{
							buildOrder2.m_Start = val3[buildOrder.m_Start];
							buildOrder2.m_End = buildOrder2.m_Start + (buildOrder.m_End - buildOrder.m_Start);
						}
						else
						{
							buildOrder2.m_End = val3[buildOrder.m_End];
							buildOrder2.m_Start = buildOrder2.m_End + (buildOrder.m_Start - buildOrder.m_End);
						}
						nativeArray3[num2] = buildOrder2;
					}
					for (int num3 = 0; num3 < nativeArray4.Length; num3++)
					{
						Game.Zones.BuildOrder buildOrder3 = nativeArray4[num3];
						buildOrder3.m_Order = val3[buildOrder3.m_Order];
						nativeArray4[num3] = buildOrder3;
					}
				}
				m_BuildOrder.value = orderItem2.m_Max + 1;
			}
			else
			{
				m_BuildOrder.value = 0u;
			}
		}
	}

	private struct TypeHandle
	{
		public ComponentTypeHandle<Game.Net.BuildOrder> __Game_Net_BuildOrder_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Zones.BuildOrder> __Game_Zones_BuildOrder_RW_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			__Game_Net_BuildOrder_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.BuildOrder>(false);
			__Game_Zones_BuildOrder_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Zones.BuildOrder>(false);
		}
	}

	private GenerateEdgesSystem m_GenerateEdgesSystem;

	private EntityQuery m_BuildOrderQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GenerateEdgesSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GenerateEdgesSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Net.BuildOrder>(),
			ComponentType.ReadOnly<Game.Zones.BuildOrder>()
		};
		array[0] = val;
		m_BuildOrderQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_BuildOrderQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = IJobExtensions.Schedule<ResetBuildOrderJob>(new ResetBuildOrderJob
		{
			m_Chunks = chunks,
			m_NetBuildOrderType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.BuildOrder>(ref __TypeHandle.__Game_Net_BuildOrder_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneBuildOrderType = InternalCompilerInterface.GetComponentTypeHandle<Game.Zones.BuildOrder>(ref __TypeHandle.__Game_Zones_BuildOrder_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildOrder = m_GenerateEdgesSystem.GetBuildOrder()
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
		chunks.Dispose(val2);
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
	public ResetBuildOrderSystem()
	{
	}
}
