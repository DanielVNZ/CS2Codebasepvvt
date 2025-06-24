using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Rendering;
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
public class SearchSystem : GameSystemBase, IPreDeserialize
{
	[BurstCompile]
	private struct UpdateSearchTreeJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Stack> m_StackType;

		[ReadOnly]
		public ComponentTypeHandle<Marker> m_MarkerType;

		[ReadOnly]
		public ComponentTypeHandle<OutsideConnection> m_OutsideConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<Tree> m_TreeType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Created> m_CreatedType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<Overridden> m_OverriddenType;

		[ReadOnly]
		public ComponentTypeHandle<CullingInfo> m_CullingInfoType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<StackData> m_PrefabStackData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public bool m_Loaded;

		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			if (((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType))
			{
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity val = nativeArray[i];
					m_SearchTree.TryRemove(val);
				}
				return;
			}
			if (m_Loaded || ((ArchetypeChunk)(ref chunk)).Has<Created>(ref m_CreatedType))
			{
				NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
				NativeArray<Stack> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Stack>(ref m_StackType);
				NativeArray<Owner> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
				bool flag = ((ArchetypeChunk)(ref chunk)).Has<Marker>(ref m_MarkerType) && !((ArchetypeChunk)(ref chunk)).Has<OutsideConnection>(ref m_OutsideConnectionType);
				bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<Overridden>(ref m_OverriddenType);
				bool flag3 = ((ArchetypeChunk)(ref chunk)).Has<Tree>(ref m_TreeType);
				bool flag4 = ((ArchetypeChunk)(ref chunk)).Has<CullingInfo>(ref m_CullingInfoType);
				ObjectGeometryData geometryData = default(ObjectGeometryData);
				StackData stackData = default(StackData);
				Owner owner = default(Owner);
				Bounds3 bounds2 = default(Bounds3);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val2 = nativeArray[j];
					PrefabRef prefabRef = nativeArray2[j];
					Transform transform = nativeArray3[j];
					if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref geometryData))
					{
						Bounds3 bounds = ((nativeArray4.Length == 0 || !m_PrefabStackData.TryGetComponent(prefabRef.m_Prefab, ref stackData)) ? ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, geometryData) : ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, nativeArray4[j], geometryData, stackData));
						BoundsMask boundsMask = BoundsMask.Debug;
						if (!flag)
						{
							if (flag3)
							{
								boundsMask |= BoundsMask.IsTree;
							}
							if ((geometryData.m_Flags & GeometryFlags.OccupyZone) != GeometryFlags.None)
							{
								boundsMask |= BoundsMask.OccupyZone;
							}
							if ((geometryData.m_Flags & GeometryFlags.WalkThrough) == 0)
							{
								boundsMask |= BoundsMask.NotWalkThrough;
							}
							if ((geometryData.m_Flags & GeometryFlags.HasLot) != GeometryFlags.None)
							{
								boundsMask |= BoundsMask.HasLot;
							}
						}
						if (!flag2)
						{
							boundsMask |= BoundsMask.NotOverridden;
							if (!flag || m_EditorMode)
							{
								MeshLayer layers = geometryData.m_Layers;
								CollectionUtils.TryGet<Owner>(nativeArray5, j, ref owner);
								boundsMask |= CommonUtils.GetBoundsMask(Game.Net.SearchSystem.GetLayers(owner, default(Game.Net.UtilityLane), layers, ref m_PrefabRefData, ref m_PrefabNetData, ref m_PrefabNetGeometryData));
							}
						}
						if (!flag4)
						{
							boundsMask &= ~(BoundsMask.AllLayers | BoundsMask.Debug);
						}
						m_SearchTree.Add(val2, new QuadTreeBoundsXZ(bounds, boundsMask, geometryData.m_MinLod));
					}
					else
					{
						((Bounds3)(ref bounds2))._002Ector(transform.m_Position - 1f, transform.m_Position + 1f);
						int lod = RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float3(2f)));
						BoundsMask boundsMask2 = ((!m_EditorMode) ? BoundsMask.Debug : (BoundsMask.Debug | BoundsMask.NormalLayers));
						if (!flag4)
						{
							boundsMask2 &= ~(BoundsMask.AllLayers | BoundsMask.Debug);
						}
						m_SearchTree.Add(val2, new QuadTreeBoundsXZ(bounds2, boundsMask2, lod));
					}
				}
				return;
			}
			NativeArray<PrefabRef> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Transform> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Stack> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Stack>(ref m_StackType);
			NativeArray<Owner> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			bool flag5 = ((ArchetypeChunk)(ref chunk)).Has<Marker>(ref m_MarkerType) && !((ArchetypeChunk)(ref chunk)).Has<OutsideConnection>(ref m_OutsideConnectionType);
			bool flag6 = ((ArchetypeChunk)(ref chunk)).Has<Overridden>(ref m_OverriddenType);
			bool flag7 = ((ArchetypeChunk)(ref chunk)).Has<Tree>(ref m_TreeType);
			bool flag8 = ((ArchetypeChunk)(ref chunk)).Has<CullingInfo>(ref m_CullingInfoType);
			ObjectGeometryData geometryData2 = default(ObjectGeometryData);
			StackData stackData2 = default(StackData);
			Owner owner2 = default(Owner);
			Bounds3 bounds4 = default(Bounds3);
			for (int k = 0; k < nativeArray.Length; k++)
			{
				Entity val3 = nativeArray[k];
				PrefabRef prefabRef2 = nativeArray6[k];
				Transform transform2 = nativeArray7[k];
				if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef2.m_Prefab, ref geometryData2))
				{
					Bounds3 bounds3 = ((nativeArray8.Length == 0 || !m_PrefabStackData.TryGetComponent(prefabRef2.m_Prefab, ref stackData2)) ? ObjectUtils.CalculateBounds(transform2.m_Position, transform2.m_Rotation, geometryData2) : ObjectUtils.CalculateBounds(transform2.m_Position, transform2.m_Rotation, nativeArray8[k], geometryData2, stackData2));
					BoundsMask boundsMask3 = BoundsMask.Debug;
					if (!flag5)
					{
						if (flag7)
						{
							boundsMask3 |= BoundsMask.IsTree;
						}
						if ((geometryData2.m_Flags & GeometryFlags.OccupyZone) != GeometryFlags.None)
						{
							boundsMask3 |= BoundsMask.OccupyZone;
						}
						if ((geometryData2.m_Flags & GeometryFlags.WalkThrough) == 0)
						{
							boundsMask3 |= BoundsMask.NotWalkThrough;
						}
						if ((geometryData2.m_Flags & GeometryFlags.HasLot) != GeometryFlags.None)
						{
							boundsMask3 |= BoundsMask.HasLot;
						}
					}
					if (!flag6)
					{
						boundsMask3 |= BoundsMask.NotOverridden;
						if (!flag5 || m_EditorMode)
						{
							MeshLayer layers2 = geometryData2.m_Layers;
							CollectionUtils.TryGet<Owner>(nativeArray9, k, ref owner2);
							boundsMask3 |= CommonUtils.GetBoundsMask(Game.Net.SearchSystem.GetLayers(owner2, default(Game.Net.UtilityLane), layers2, ref m_PrefabRefData, ref m_PrefabNetData, ref m_PrefabNetGeometryData));
						}
					}
					if (!flag8)
					{
						boundsMask3 &= ~(BoundsMask.AllLayers | BoundsMask.Debug);
					}
					m_SearchTree.Update(val3, new QuadTreeBoundsXZ(bounds3, boundsMask3, geometryData2.m_MinLod));
				}
				else
				{
					((Bounds3)(ref bounds4))._002Ector(transform2.m_Position - 1f, transform2.m_Position + 1f);
					int lod2 = RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float3(2f)));
					BoundsMask boundsMask4 = ((!m_EditorMode) ? BoundsMask.Debug : (BoundsMask.Debug | BoundsMask.NormalLayers));
					if (!flag8)
					{
						boundsMask4 &= ~(BoundsMask.AllLayers | BoundsMask.Debug);
					}
					m_SearchTree.Update(val3, new QuadTreeBoundsXZ(bounds4, boundsMask4, lod2));
				}
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
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Stack> __Game_Objects_Stack_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Marker> __Game_Objects_Marker_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Tree> __Game_Objects_Tree_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Created> __Game_Common_Created_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Overridden> __Game_Common_Overridden_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CullingInfo> __Game_Rendering_CullingInfo_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StackData> __Game_Prefabs_StackData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

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
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Objects_Stack_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Stack>(true);
			__Game_Objects_Marker_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Marker>(true);
			__Game_Objects_OutsideConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<OutsideConnection>(true);
			__Game_Objects_Tree_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Tree>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Common_Created_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Created>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Common_Overridden_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Overridden>(true);
			__Game_Rendering_CullingInfo_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CullingInfo>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_StackData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StackData>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
		}
	}

	private ToolSystem m_ToolSystem;

	private EntityQuery m_UpdatedStaticsQuery;

	private EntityQuery m_AllStaticsQuery;

	private NativeQuadTree<Entity, QuadTreeBoundsXZ> m_StaticSearchTree;

	private NativeQuadTree<Entity, QuadTreeBoundsXZ> m_MovingSearchTree;

	private JobHandle m_StaticReadDependencies;

	private JobHandle m_StaticWriteDependencies;

	private JobHandle m_MovingReadDependencies;

	private JobHandle m_MovingWriteDependencies;

	private bool m_Loaded;

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
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Object>(),
			ComponentType.ReadOnly<Static>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		m_UpdatedStaticsQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_AllStaticsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Object>(),
			ComponentType.ReadOnly<Static>(),
			ComponentType.Exclude<Temp>()
		});
		m_StaticSearchTree = new NativeQuadTree<Entity, QuadTreeBoundsXZ>(1f, (Allocator)4);
		m_MovingSearchTree = new NativeQuadTree<Entity, QuadTreeBoundsXZ>(1f, (Allocator)4);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((JobHandle)(ref m_StaticReadDependencies)).Complete();
		((JobHandle)(ref m_StaticWriteDependencies)).Complete();
		m_StaticSearchTree.Dispose();
		((JobHandle)(ref m_MovingReadDependencies)).Complete();
		((JobHandle)(ref m_MovingWriteDependencies)).Complete();
		m_MovingSearchTree.Dispose();
		base.OnDestroy();
	}

	private bool GetLoaded()
	{
		if (m_Loaded)
		{
			m_Loaded = false;
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		bool loaded = GetLoaded();
		EntityQuery val = (loaded ? m_AllStaticsQuery : m_UpdatedStaticsQuery);
		if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
		{
			JobHandle dependencies;
			UpdateSearchTreeJob updateSearchTreeJob = new UpdateSearchTreeJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_StackType = InternalCompilerInterface.GetComponentTypeHandle<Stack>(ref __TypeHandle.__Game_Objects_Stack_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MarkerType = InternalCompilerInterface.GetComponentTypeHandle<Marker>(ref __TypeHandle.__Game_Objects_Marker_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OutsideConnectionType = InternalCompilerInterface.GetComponentTypeHandle<OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TreeType = InternalCompilerInterface.GetComponentTypeHandle<Tree>(ref __TypeHandle.__Game_Objects_Tree_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CreatedType = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OverriddenType = InternalCompilerInterface.GetComponentTypeHandle<Overridden>(ref __TypeHandle.__Game_Common_Overridden_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CullingInfoType = InternalCompilerInterface.GetComponentTypeHandle<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabStackData = InternalCompilerInterface.GetComponentLookup<StackData>(ref __TypeHandle.__Game_Prefabs_StackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
				m_Loaded = loaded,
				m_SearchTree = GetStaticSearchTree(readOnly: false, out dependencies)
			};
			((SystemBase)this).Dependency = JobChunkExtensions.Schedule<UpdateSearchTreeJob>(updateSearchTreeJob, val, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
			AddStaticSearchTreeWriter(((SystemBase)this).Dependency);
		}
	}

	public NativeQuadTree<Entity, QuadTreeBoundsXZ> GetStaticSearchTree(bool readOnly, out JobHandle dependencies)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		dependencies = (readOnly ? m_StaticWriteDependencies : JobHandle.CombineDependencies(m_StaticReadDependencies, m_StaticWriteDependencies));
		return m_StaticSearchTree;
	}

	public NativeQuadTree<Entity, QuadTreeBoundsXZ> GetMovingSearchTree(bool readOnly, out JobHandle dependencies)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		dependencies = (readOnly ? m_MovingWriteDependencies : JobHandle.CombineDependencies(m_MovingReadDependencies, m_MovingWriteDependencies));
		return m_MovingSearchTree;
	}

	public void AddStaticSearchTreeReader(JobHandle jobHandle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_StaticReadDependencies = JobHandle.CombineDependencies(m_StaticReadDependencies, jobHandle);
	}

	public void AddStaticSearchTreeWriter(JobHandle jobHandle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_StaticWriteDependencies = jobHandle;
	}

	public void AddMovingSearchTreeReader(JobHandle jobHandle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_MovingReadDependencies = JobHandle.CombineDependencies(m_MovingReadDependencies, jobHandle);
	}

	public void AddMovingSearchTreeWriter(JobHandle jobHandle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_MovingWriteDependencies = jobHandle;
	}

	public void PreDeserialize(Context context)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeQuadTree<Entity, QuadTreeBoundsXZ> staticSearchTree = GetStaticSearchTree(readOnly: false, out dependencies);
		JobHandle dependencies2;
		NativeQuadTree<Entity, QuadTreeBoundsXZ> movingSearchTree = GetMovingSearchTree(readOnly: false, out dependencies2);
		((JobHandle)(ref dependencies)).Complete();
		((JobHandle)(ref dependencies2)).Complete();
		staticSearchTree.Clear();
		movingSearchTree.Clear();
		m_Loaded = true;
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
	public SearchSystem()
	{
	}
}
