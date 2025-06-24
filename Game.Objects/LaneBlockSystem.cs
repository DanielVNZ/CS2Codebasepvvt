using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Tools;
using Game.Vehicles;
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
public class LaneBlockSystem : GameSystemBase
{
	[BurstCompile]
	private struct FindBlockedLanesJob : IJobChunk
	{
		private struct FindBlockedLanesIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds3 m_Bounds;

			public Entity m_Entity;

			public float3 m_Position;

			public float m_Radius;

			public DynamicBuffer<BlockedLane> m_BlockedLanes;

			public LaneObjectCommandBuffer m_LaneObjectBuffer;

			public BufferLookup<Game.Net.SubLane> m_SubLanes;

			public ComponentLookup<MasterLane> m_MasterLaneData;

			public ComponentLookup<Curve> m_CurveData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<NetLaneData> m_PrefabLaneData;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity edgeEntity)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_0132: Unknown result type (might be due to invalid IL or missing references)
				//IL_013e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0150: Unknown result type (might be due to invalid IL or missing references)
				//IL_0151: Unknown result type (might be due to invalid IL or missing references)
				//IL_0158: Unknown result type (might be due to invalid IL or missing references)
				//IL_015f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0175: Unknown result type (might be due to invalid IL or missing references)
				//IL_0177: Unknown result type (might be due to invalid IL or missing references)
				//IL_017c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0183: Unknown result type (might be due to invalid IL or missing references)
				//IL_018a: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || !m_SubLanes.HasBuffer(edgeEntity))
				{
					return;
				}
				DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[edgeEntity];
				float num3 = default(float);
				Bounds1 val2 = default(Bounds1);
				Bounds1 val3 = default(Bounds1);
				for (int i = 0; i < val.Length; i++)
				{
					Entity subLane = val[i].m_SubLane;
					if (m_MasterLaneData.HasComponent(subLane))
					{
						continue;
					}
					Entity prefab = m_PrefabRefData[subLane].m_Prefab;
					Bezier4x3 bezier = m_CurveData[subLane].m_Bezier;
					NetLaneData netLaneData = m_PrefabLaneData[prefab];
					float num = m_Radius + netLaneData.m_Width * 0.5f;
					float num2 = MathUtils.Distance(MathUtils.Bounds(bezier), m_Position);
					if (num2 < num)
					{
						num2 = MathUtils.Distance(bezier, m_Position, ref num3);
						if (num2 < num)
						{
							num2 = math.max(0f, num2 - netLaneData.m_Width * 0.5f);
							float num4 = math.sqrt(math.max(0f, m_Radius * m_Radius - num2 * num2));
							((Bounds1)(ref val2))._002Ector(0f, num3);
							((Bounds1)(ref val3))._002Ector(num3, 1f);
							MathUtils.ClampLengthInverse(bezier, ref val2, num4);
							MathUtils.ClampLength(bezier, ref val3, num4);
							m_BlockedLanes.Add(new BlockedLane(subLane, new float2(val2.min, val3.max)));
							m_LaneObjectBuffer.Add(subLane, m_Entity, new float2(val2.min, val3.max));
						}
					}
				}
			}
		}

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Created> m_CreatedType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		public BufferTypeHandle<BlockedLane> m_BlockedLaneType;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<MasterLane> m_MasterLaneData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabLaneData;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		public LaneObjectCommandBuffer m_LaneObjectBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<BlockedLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<BlockedLane>(ref m_BlockedLaneType);
			if (((ArchetypeChunk)(ref chunk)).Has<Created>(ref m_CreatedType))
			{
				NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
				NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity entity = nativeArray[i];
					DynamicBuffer<BlockedLane> blockedLanes = bufferAccessor[i];
					Transform transform = nativeArray2[i];
					AddBlockedLanes(entity, blockedLanes, transform, nativeArray3[i].m_Prefab);
				}
				return;
			}
			if (((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType))
			{
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity entity2 = nativeArray[j];
					DynamicBuffer<BlockedLane> val = bufferAccessor[j];
					for (int k = 0; k < val.Length; k++)
					{
						m_LaneObjectBuffer.Remove(val[k].m_Lane, entity2);
					}
				}
				return;
			}
			NativeArray<Transform> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			for (int l = 0; l < nativeArray.Length; l++)
			{
				Entity entity3 = nativeArray[l];
				DynamicBuffer<BlockedLane> blockedLanes2 = bufferAccessor[l];
				Transform transform2 = nativeArray4[l];
				PrefabRef prefabRef = nativeArray5[l];
				for (int m = 0; m < blockedLanes2.Length; m++)
				{
					m_LaneObjectBuffer.Remove(blockedLanes2[m].m_Lane, entity3);
				}
				blockedLanes2.Clear();
				AddBlockedLanes(entity3, blockedLanes2, transform2, prefabRef.m_Prefab);
			}
		}

		private void AddBlockedLanes(Entity entity, DynamicBuffer<BlockedLane> blockedLanes, Transform transform, Entity prefab)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			Bounds3 val = (Bounds3)((!m_PrefabGeometryData.HasComponent(prefab)) ? default(Bounds3) : m_PrefabGeometryData[prefab].m_Bounds);
			FindBlockedLanesIterator findBlockedLanesIterator = new FindBlockedLanesIterator
			{
				m_Bounds = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, val),
				m_Entity = entity,
				m_Position = transform.m_Position,
				m_Radius = (val.max.x - val.min.y) * 0.5f,
				m_BlockedLanes = blockedLanes,
				m_LaneObjectBuffer = m_LaneObjectBuffer,
				m_SubLanes = m_SubLanes,
				m_MasterLaneData = m_MasterLaneData,
				m_CurveData = m_CurveData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabLaneData = m_PrefabLaneData
			};
			m_NetSearchTree.Iterate<FindBlockedLanesIterator>(ref findBlockedLanesIterator, 0);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Created> __Game_Common_Created_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		public BufferTypeHandle<BlockedLane> __Game_Objects_BlockedLane_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MasterLane> __Game_Net_MasterLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Common_Created_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Created>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Objects_BlockedLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<BlockedLane>(false);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_MasterLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MasterLane>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
		}
	}

	private Game.Net.SearchSystem m_NetSearchSystem;

	private EntityQuery m_ObjectQuery;

	private LaneObjectUpdater m_LaneObjectUpdater;

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
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<BlockedLane>(),
			ComponentType.ReadOnly<Transform>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Vehicle>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_ObjectQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_LaneObjectUpdater = new LaneObjectUpdater((SystemBase)(object)this);
		((ComponentSystemBase)this).RequireForUpdate(m_ObjectQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle val = JobChunkExtensions.ScheduleParallel<FindBlockedLanesJob>(new FindBlockedLanesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreatedType = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BlockedLaneType = InternalCompilerInterface.GetBufferTypeHandle<BlockedLane>(ref __TypeHandle.__Game_Objects_BlockedLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MasterLaneData = InternalCompilerInterface.GetComponentLookup<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_LaneObjectBuffer = m_LaneObjectUpdater.Begin((Allocator)3)
		}, m_ObjectQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		m_NetSearchSystem.AddNetSearchTreeReader(val);
		JobHandle dependency = m_LaneObjectUpdater.Apply((SystemBase)(object)this, val);
		((SystemBase)this).Dependency = dependency;
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
	public LaneBlockSystem()
	{
	}
}
