using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class CollapsedBuildingSystem : GameSystemBase
{
	[BurstCompile]
	private struct CollapsedBuildingJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<RescueTarget> m_RescueTargetType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.ServiceUpgrade> m_ServiceUpgradeType;

		[ReadOnly]
		public ComponentTypeHandle<Extension> m_ExtensionType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Attached> m_AttachedType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<Destroyed> m_DestroyedType;

		[ReadOnly]
		public ComponentLookup<FireRescueRequest> m_FireRescueRequestData;

		[ReadOnly]
		public ComponentLookup<Area> m_AreaData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public EntityArchetype m_RescueRequestArchetype;

		public ParallelWriter m_CommandBuffer;

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
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Destroyed> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Destroyed>(ref m_DestroyedType);
			NativeArray<RescueTarget> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<RescueTarget>(ref m_RescueTargetType);
			NativeArray<Owner> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Attached>(ref m_AttachedType);
			bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.ServiceUpgrade>(ref m_ServiceUpgradeType);
			bool flag3 = ((ArchetypeChunk)(ref chunk)).Has<Extension>(ref m_ExtensionType);
			if (nativeArray3.Length != 0)
			{
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Destroyed destroyed = nativeArray2[i];
					Entity val = nativeArray[i];
					if (destroyed.m_Cleared < 1f)
					{
						RescueTarget rescueTarget = nativeArray3[i];
						RequestRescueIfNeeded(unfilteredChunkIndex, val, rescueTarget);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<RescueTarget>(unfilteredChunkIndex, val);
					}
				}
			}
			else
			{
				BuildingData buildingData = default(BuildingData);
				Owner owner = default(Owner);
				PrefabRef prefabRef2 = default(PrefabRef);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					ref Destroyed reference = ref CollectionUtils.ElementAt<Destroyed>(nativeArray2, j);
					PrefabRef prefabRef = nativeArray5[j];
					bool flag4 = false;
					if (m_PrefabBuildingData.TryGetComponent(prefabRef.m_Prefab, ref buildingData))
					{
						flag4 = (buildingData.m_Flags & Game.Prefabs.BuildingFlags.RequireRoad) != 0;
						if (CollectionUtils.TryGet<Owner>(nativeArray4, j, ref owner) && m_PrefabRefData.TryGetComponent(owner.m_Owner, ref prefabRef2) && m_PrefabBuildingData.TryGetComponent(prefabRef2.m_Prefab, ref buildingData))
						{
							flag4 |= (buildingData.m_Flags & Game.Prefabs.BuildingFlags.RequireRoad) != 0;
						}
					}
					if (reference.m_Cleared < 0f)
					{
						Entity val2 = nativeArray[j];
						reference.m_Cleared += 1.0666667f;
						if (reference.m_Cleared >= 0f)
						{
							reference.m_Cleared = math.select(1f, 0f, flag4);
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<InterpolatedTransform>(unfilteredChunkIndex, val2);
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(unfilteredChunkIndex, val2, default(Updated));
						}
					}
					else if (reference.m_Cleared < 1f && !flag3)
					{
						if (flag4)
						{
							Entity val3 = nativeArray[j];
							RescueTarget rescueTarget2 = default(RescueTarget);
							RequestRescueIfNeeded(unfilteredChunkIndex, val3, rescueTarget2);
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<RescueTarget>(unfilteredChunkIndex, val3, rescueTarget2);
						}
						else
						{
							reference.m_Cleared = 1f;
						}
					}
				}
			}
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			Owner owner2 = default(Owner);
			DynamicBuffer<Game.Areas.SubArea> val5 = default(DynamicBuffer<Game.Areas.SubArea>);
			for (int k = 0; k < nativeArray2.Length; k++)
			{
				if (!(nativeArray2[k].m_Cleared >= 1f) || flag2 || flag3)
				{
					continue;
				}
				Entity val4 = nativeArray[k];
				PrefabRef prefabRef3 = nativeArray5[k];
				if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef3.m_Prefab, ref objectGeometryData))
				{
					if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Overridable) != Game.Objects.GeometryFlags.None && !flag)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val4, default(Deleted));
					}
					else
					{
						if (!CollectionUtils.TryGet<Owner>(nativeArray4, k, ref owner2) || !m_AreaData.HasComponent(owner2.m_Owner))
						{
							continue;
						}
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val4, default(Deleted));
						if (m_SubAreas.TryGetBuffer(val4, ref val5))
						{
							for (int l = 0; l < val5.Length; l++)
							{
								((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(unfilteredChunkIndex, val5[l].m_Area, default(Updated));
							}
						}
					}
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val4, default(Deleted));
				}
			}
		}

		private void RequestRescueIfNeeded(int jobIndex, Entity entity, RescueTarget rescueTarget)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			if (!m_FireRescueRequestData.HasComponent(rescueTarget.m_Request))
			{
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_RescueRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<FireRescueRequest>(jobIndex, val, new FireRescueRequest(entity, 10f, FireRescueRequestType.Disaster));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(4u));
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
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<RescueTarget> __Game_Buildings_RescueTarget_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.ServiceUpgrade> __Game_Buildings_ServiceUpgrade_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Extension> __Game_Buildings_Extension_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Attached> __Game_Objects_Attached_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Destroyed> __Game_Common_Destroyed_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<FireRescueRequest> __Game_Simulation_FireRescueRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Area> __Game_Areas_Area_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

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
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_RescueTarget_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RescueTarget>(true);
			__Game_Buildings_ServiceUpgrade_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.ServiceUpgrade>(true);
			__Game_Buildings_Extension_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Extension>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Objects_Attached_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Attached>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Common_Destroyed_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Destroyed>(false);
			__Game_Simulation_FireRescueRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<FireRescueRequest>(true);
			__Game_Areas_Area_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Area>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
		}
	}

	private const uint UPDATE_INTERVAL = 64u;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_CollapsedQuery;

	private EntityArchetype m_RescueRequestArchetype;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 64;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Destroyed>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Extension>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_CollapsedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_RescueRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<FireRescueRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CollapsedQuery);
		Assert.IsTrue(true);
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
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		CollapsedBuildingJob collapsedBuildingJob = new CollapsedBuildingJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RescueTargetType = InternalCompilerInterface.GetComponentTypeHandle<RescueTarget>(ref __TypeHandle.__Game_Buildings_RescueTarget_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpgradeType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.ServiceUpgrade>(ref __TypeHandle.__Game_Buildings_ServiceUpgrade_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ExtensionType = InternalCompilerInterface.GetComponentTypeHandle<Extension>(ref __TypeHandle.__Game_Buildings_Extension_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AttachedType = InternalCompilerInterface.GetComponentTypeHandle<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedType = InternalCompilerInterface.GetComponentTypeHandle<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_FireRescueRequestData = InternalCompilerInterface.GetComponentLookup<FireRescueRequest>(ref __TypeHandle.__Game_Simulation_FireRescueRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaData = InternalCompilerInterface.GetComponentLookup<Area>(ref __TypeHandle.__Game_Areas_Area_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RescueRequestArchetype = m_RescueRequestArchetype
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		collapsedBuildingJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		CollapsedBuildingJob collapsedBuildingJob2 = collapsedBuildingJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CollapsedBuildingJob>(collapsedBuildingJob2, m_CollapsedQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public CollapsedBuildingSystem()
	{
	}
}
