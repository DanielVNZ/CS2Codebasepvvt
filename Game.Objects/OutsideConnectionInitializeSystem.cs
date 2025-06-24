using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Prefabs;
using Game.Serialization;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Objects;

[CompilerGenerated]
public class OutsideConnectionInitializeSystem : GameSystemBase, IPostDeserialize
{
	[BurstCompile]
	private struct CollectOutsideConnectionsJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public BufferTypeHandle<RandomLocalizationIndex> m_RandomLocalizationIndexType;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> m_OutsideConnectionData;

		public ParallelWriter<OutsideConnectionInfo> m_OutsideConnections;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			BufferAccessor<RandomLocalizationIndex> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<RandomLocalizationIndex>(ref m_RandomLocalizationIndexType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				DynamicBuffer<RandomLocalizationIndex> val = bufferAccessor[i];
				if (val.Length == 1)
				{
					m_OutsideConnections.AddNoResize(new OutsideConnectionInfo
					{
						m_TransferType = GetTransferType(nativeArray[i].m_Prefab, ref m_OutsideConnectionData),
						m_Position = nativeArray2[i].m_Position,
						m_RandomIndex = val[0]
					});
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct InitializeLocalizationJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		public BufferTypeHandle<RandomLocalizationIndex> m_RandomLocalizationIndexType;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> m_OutsideConnectionData;

		[ReadOnly]
		public BufferLookup<LocalizationCount> m_LocalizationCounts;

		public NativeList<OutsideConnectionInfo> m_OutsideConnections;

		public RandomSeed m_RandomSeed;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			BufferAccessor<RandomLocalizationIndex> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<RandomLocalizationIndex>(ref m_RandomLocalizationIndexType);
			DynamicBuffer<LocalizationCount> counts = default(DynamicBuffer<LocalizationCount>);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				Entity prefab = nativeArray2[i].m_Prefab;
				float3 position = nativeArray3[i].m_Position;
				DynamicBuffer<RandomLocalizationIndex> indices = bufferAccessor[i];
				if (m_LocalizationCounts.TryGetBuffer(prefab, ref counts))
				{
					OutsideConnectionTransferType transferType = GetTransferType(prefab, ref m_OutsideConnectionData);
					if (counts.Length == 1 && TryGetNearestConnectionRandomIndex(m_OutsideConnections, transferType, position, out var randomIndex) && randomIndex.m_Index <= counts[0].m_Count)
					{
						indices.ResizeUninitialized(1);
						indices[0] = randomIndex;
					}
					else
					{
						Random random = m_RandomSeed.GetRandom(val.Index + 1);
						RandomLocalizationIndex.GenerateRandomIndices(indices, counts, ref random);
					}
					if (indices.Length == 1)
					{
						ref NativeList<OutsideConnectionInfo> reference = ref m_OutsideConnections;
						OutsideConnectionInfo outsideConnectionInfo = new OutsideConnectionInfo
						{
							m_TransferType = transferType,
							m_Position = position,
							m_RandomIndex = indices[0]
						};
						reference.Add(ref outsideConnectionInfo);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct OutsideConnectionInfo
	{
		public OutsideConnectionTransferType m_TransferType;

		public float3 m_Position;

		public RandomLocalizationIndex m_RandomIndex;
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<RandomLocalizationIndex> __Game_Common_RandomLocalizationIndex_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> __Game_Prefabs_OutsideConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		public BufferTypeHandle<RandomLocalizationIndex> __Game_Common_RandomLocalizationIndex_RW_BufferTypeHandle;

		[ReadOnly]
		public BufferLookup<LocalizationCount> __Game_Prefabs_LocalizationCount_RO_BufferLookup;

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
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Common_RandomLocalizationIndex_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<RandomLocalizationIndex>(true);
			__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OutsideConnectionData>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_RandomLocalizationIndex_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<RandomLocalizationIndex>(false);
			__Game_Prefabs_LocalizationCount_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LocalizationCount>(true);
		}
	}

	private const float kNearbyMaxDistanceSqr = 10000f;

	private EntityQuery m_ExistingQuery;

	private EntityQuery m_CreatedQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ExistingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<OutsideConnection>(),
			ComponentType.ReadOnly<RandomLocalizationIndex>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Created>()
		});
		m_CreatedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<OutsideConnection>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadWrite<RandomLocalizationIndex>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CreatedQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		int num = ((EntityQuery)(ref m_ExistingQuery)).CalculateEntityCount() + ((EntityQuery)(ref m_CreatedQuery)).CalculateEntityCount();
		NativeList<OutsideConnectionInfo> outsideConnections = default(NativeList<OutsideConnectionInfo>);
		outsideConnections._002Ector(num, AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle val = JobChunkExtensions.ScheduleParallel<CollectOutsideConnectionsJob>(new CollectOutsideConnectionsJob
		{
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RandomLocalizationIndexType = InternalCompilerInterface.GetBufferTypeHandle<RandomLocalizationIndex>(ref __TypeHandle.__Game_Common_RandomLocalizationIndex_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionData = InternalCompilerInterface.GetComponentLookup<OutsideConnectionData>(ref __TypeHandle.__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnections = outsideConnections.AsParallelWriter()
		}, m_ExistingQuery, ((SystemBase)this).Dependency);
		InitializeLocalizationJob initializeLocalizationJob = new InitializeLocalizationJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RandomLocalizationIndexType = InternalCompilerInterface.GetBufferTypeHandle<RandomLocalizationIndex>(ref __TypeHandle.__Game_Common_RandomLocalizationIndex_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionData = InternalCompilerInterface.GetComponentLookup<OutsideConnectionData>(ref __TypeHandle.__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LocalizationCounts = InternalCompilerInterface.GetBufferLookup<LocalizationCount>(ref __TypeHandle.__Game_Prefabs_LocalizationCount_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnections = outsideConnections,
			m_RandomSeed = RandomSeed.Next()
		};
		((SystemBase)this).Dependency = JobChunkExtensions.Schedule<InitializeLocalizationJob>(initializeLocalizationJob, m_CreatedQuery, val);
		outsideConnections.Dispose(((SystemBase)this).Dependency);
	}

	public void PostDeserialize(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		if (!(((Context)(ref context)).version < Version.outsideConnNames))
		{
			return;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<OutsideConnection>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<RandomLocalizationIndex>()
		});
		if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
		{
			NativeList<OutsideConnectionInfo> connections = default(NativeList<OutsideConnectionInfo>);
			connections._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeArray<Entity> val2 = ((EntityQuery)(ref val)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			NativeArray<PrefabRef> val3 = ((EntityQuery)(ref val)).ToComponentDataArray<PrefabRef>(AllocatorHandle.op_Implicit((Allocator)3));
			NativeArray<Transform> val4 = ((EntityQuery)(ref val)).ToComponentDataArray<Transform>(AllocatorHandle.op_Implicit((Allocator)3));
			RandomSeed randomSeed = RandomSeed.Next();
			OutsideConnectionData outsideConnectionData = default(OutsideConnectionData);
			for (int i = 0; i < val2.Length; i++)
			{
				Entity prefab = val3[i].m_Prefab;
				OutsideConnectionTransferType transferType = (EntitiesExtensions.TryGetComponent<OutsideConnectionData>(((ComponentSystemBase)this).EntityManager, prefab, ref outsideConnectionData) ? outsideConnectionData.m_Type : OutsideConnectionTransferType.None);
				float3 position = val4[i].m_Position;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasBuffer<LocalizationCount>(prefab))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					DynamicBuffer<RandomLocalizationIndex> indices = ((EntityManager)(ref entityManager)).AddBuffer<RandomLocalizationIndex>(val2[i]);
					entityManager = ((ComponentSystemBase)this).EntityManager;
					DynamicBuffer<LocalizationCount> buffer = ((EntityManager)(ref entityManager)).GetBuffer<LocalizationCount>(prefab, true);
					if (buffer.Length == 1 && TryGetNearestConnectionRandomIndex(connections, transferType, position, out var randomIndex) && randomIndex.m_Index < buffer[0].m_Count)
					{
						indices.ResizeUninitialized(1);
						indices[0] = randomIndex;
					}
					else
					{
						Random random = randomSeed.GetRandom(val2[i].Index + 1);
						RandomLocalizationIndex.GenerateRandomIndices(indices, buffer, ref random);
					}
					if (indices.Length == 1)
					{
						OutsideConnectionInfo outsideConnectionInfo = new OutsideConnectionInfo
						{
							m_TransferType = transferType,
							m_Position = position,
							m_RandomIndex = indices[0]
						};
						connections.Add(ref outsideConnectionInfo);
					}
				}
			}
			val2.Dispose();
			val3.Dispose();
			val4.Dispose();
			connections.Dispose();
		}
		((EntityQuery)(ref val)).Dispose();
	}

	private static OutsideConnectionTransferType GetTransferType(Entity prefab, ref ComponentLookup<OutsideConnectionData> outsideConnectionData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OutsideConnectionData outsideConnectionData2 = default(OutsideConnectionData);
		if (!outsideConnectionData.TryGetComponent(prefab, ref outsideConnectionData2))
		{
			return OutsideConnectionTransferType.None;
		}
		return outsideConnectionData2.m_Type;
	}

	private static bool TryGetNearestConnectionRandomIndex(NativeList<OutsideConnectionInfo> connections, OutsideConnectionTransferType transferType, float3 position, out RandomLocalizationIndex randomIndex)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		randomIndex = default(RandomLocalizationIndex);
		float num = 10000f;
		Enumerator<OutsideConnectionInfo> enumerator = connections.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				OutsideConnectionInfo current = enumerator.Current;
				if ((current.m_TransferType & transferType) != OutsideConnectionTransferType.None)
				{
					float num2 = math.distancesq(current.m_Position, position);
					if (num2 < num)
					{
						randomIndex = current.m_RandomIndex;
						num = num2;
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		return num < 10000f;
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
	public OutsideConnectionInitializeSystem()
	{
	}
}
