using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.City;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class CameraCollisionSystem : GameSystemBase
{
	[BurstCompile]
	private struct FindEntitiesFromTreeJob : IJob
	{
		private struct Iterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Segment m_Line;

			public float3 m_Expand;

			public NativeList<Entity> m_EntityList;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				float2 val = default(float2);
				if (MathUtils.Intersect(MathUtils.Expand(bounds.m_Bounds, m_Expand), m_Line, ref val))
				{
					return (bounds.m_Mask & BoundsMask.NotOverridden) != 0;
				}
				return false;
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				if (Intersect(bounds))
				{
					m_EntityList.Add(ref entity);
				}
			}
		}

		[ReadOnly]
		public Segment m_Line;

		[ReadOnly]
		public quaternion m_Rotation;

		[ReadOnly]
		public float2 m_FovOffset;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		[WriteOnly]
		public NativeList<Entity> m_EntityList;

		public void Execute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			float3 val = math.mul(m_Rotation, new float3(m_FovOffset.x, 0f, 0f));
			float3 val2 = math.mul(m_Rotation, new float3(0f, m_FovOffset.y, 0f));
			float3 expand = math.abs(val) + math.abs(val2);
			Iterator iterator = new Iterator
			{
				m_Line = m_Line,
				m_Expand = expand,
				m_EntityList = m_EntityList
			};
			m_SearchTree.Iterate<Iterator>(ref iterator, 0);
		}
	}

	[BurstCompile]
	private struct ObjectCollisionJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Tree> m_TreeData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.NetObject> m_NetObjectData;

		[ReadOnly]
		public ComponentLookup<Quantity> m_QuantityData;

		[ReadOnly]
		public ComponentLookup<Stack> m_StackData;

		[ReadOnly]
		public ComponentLookup<UnderConstruction> m_UnderConstructionData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnectionData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<MeshData> m_PrefabMeshData;

		[ReadOnly]
		public ComponentLookup<ImpostorData> m_PrefabImpostorData;

		[ReadOnly]
		public ComponentLookup<SharedMeshData> m_PrefabSharedMeshData;

		[ReadOnly]
		public ComponentLookup<GrowthScaleData> m_PrefabGrowthScaleData;

		[ReadOnly]
		public ComponentLookup<QuantityObjectData> m_PrefabQuantityObjectData;

		[ReadOnly]
		public ComponentLookup<StackData> m_PrefabStackData;

		[ReadOnly]
		public BufferLookup<MeshGroup> m_MeshGroups;

		[ReadOnly]
		public BufferLookup<Skeleton> m_Skeletons;

		[ReadOnly]
		public BufferLookup<Bone> m_Bones;

		[ReadOnly]
		public BufferLookup<SubMesh> m_Meshes;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> m_SubMeshGroups;

		[ReadOnly]
		public BufferLookup<LodMesh> m_Lods;

		[ReadOnly]
		public BufferLookup<MeshVertex> m_Vertices;

		[ReadOnly]
		public BufferLookup<MeshIndex> m_Indices;

		[ReadOnly]
		public BufferLookup<MeshNode> m_Nodes;

		[ReadOnly]
		public BufferLookup<ProceduralBone> m_ProceduralBones;

		[ReadOnly]
		public Segment m_Line;

		[ReadOnly]
		public quaternion m_Rotation;

		[ReadOnly]
		public float2 m_FovOffset;

		[ReadOnly]
		public float m_MinClearRange;

		[ReadOnly]
		public bool m_LeftHandTraffic;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public NativeList<Entity> m_EntityList;

		[NativeDisableContainerSafetyRestriction]
		public ParallelWriter<Collision> m_Collisions;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_EntityList[index];
			Transform transform = m_TransformData[val];
			PrefabRef prefabRef = m_PrefabRefData[val];
			ObjectGeometryData geometryData = default(ObjectGeometryData);
			if (!m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref geometryData))
			{
				return;
			}
			if ((geometryData.m_Flags & GeometryFlags.Marker) != GeometryFlags.None)
			{
				if (!m_OutsideConnectionData.HasComponent(val))
				{
					return;
				}
			}
			else if ((geometryData.m_Flags & GeometryFlags.Physical) == 0)
			{
				return;
			}
			quaternion val2 = math.inverse(transform.m_Rotation);
			Line line = default(Line);
			line.m_Line.a = math.mul(val2, m_Line.a - transform.m_Position);
			line.m_Line.b = math.mul(val2, m_Line.b - transform.m_Position);
			line.m_XVector = math.mul(val2, math.mul(m_Rotation, new float3(m_FovOffset.x, 0f, 0f)));
			line.m_YVector = math.mul(val2, math.mul(m_Rotation, new float3(0f, m_FovOffset.y, 0f)));
			line.m_Expand = math.abs(line.m_XVector) + math.abs(line.m_YVector);
			line.m_Scale = float3.op_Implicit(1f);
			line.m_CutOffset = default(float2);
			Stack stack = default(Stack);
			StackData stackData = default(StackData);
			Bounds3 val3 = ((!m_StackData.TryGetComponent(val, ref stack) || !m_PrefabStackData.TryGetComponent(prefabRef.m_Prefab, ref stackData)) ? ObjectUtils.GetBounds(geometryData) : ObjectUtils.GetBounds(stack, geometryData, stackData));
			if (Intersect(line, val3, out var t))
			{
				float num = math.cmax(MathUtils.Size(val3));
				line.m_CutOffset = new float2(t.x - num, t.y + num);
				line.m_Line = MathUtils.Cut(line.m_Line, line.m_CutOffset);
				NativeList<Collision> collisions = default(NativeList<Collision>);
				collisions._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
				RaycastMeshes(collisions, val, prefabRef, line);
				CheckCollisions(collisions, m_MinClearRange, t);
				for (int i = 0; i < collisions.Length; i++)
				{
					Collision collision = collisions[i];
					collision.m_CoverAreas = float2.op_Implicit(0f);
					m_Collisions.Enqueue(collision);
				}
				collisions.Dispose();
			}
		}

		private bool HasCachedMesh(Entity mesh, out Entity sharedMesh)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			SharedMeshData sharedMeshData = default(SharedMeshData);
			if (m_PrefabSharedMeshData.TryGetComponent(mesh, ref sharedMeshData))
			{
				sharedMesh = sharedMeshData.m_Mesh;
			}
			else
			{
				sharedMesh = mesh;
			}
			return m_Vertices.HasBuffer(sharedMesh);
		}

		private void RaycastMeshes(NativeList<Collision> collisions, Entity entity, PrefabRef prefabRef, Line line)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_0565: Unknown result type (might be due to invalid IL or missing references)
			//IL_056a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_0512: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0546: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05da: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_058e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<SubMesh> val = default(DynamicBuffer<SubMesh>);
			if (!m_Meshes.TryGetBuffer(prefabRef.m_Prefab, ref val))
			{
				return;
			}
			SubMeshFlags subMeshFlags = SubMeshFlags.DefaultMissingMesh | SubMeshFlags.HasTransform;
			subMeshFlags = (SubMeshFlags)((uint)subMeshFlags | (uint)(m_LeftHandTraffic ? 65536 : 131072));
			int3 tileCounts = int3.op_Implicit(0);
			float3 offsets = float3.op_Implicit(0f);
			float3 scale = float3.op_Implicit(0f);
			Tree tree = default(Tree);
			if (m_TreeData.TryGetComponent(entity, ref tree))
			{
				GrowthScaleData growthScaleData = default(GrowthScaleData);
				subMeshFlags = ((!m_PrefabGrowthScaleData.TryGetComponent(prefabRef.m_Prefab, ref growthScaleData)) ? (subMeshFlags | SubMeshFlags.RequireAdult) : (subMeshFlags | BatchDataHelpers.CalculateTreeSubMeshData(tree, growthScaleData, out line.m_Scale)));
			}
			Game.Objects.NetObject netObject = default(Game.Objects.NetObject);
			if (m_NetObjectData.TryGetComponent(entity, ref netObject))
			{
				subMeshFlags |= BatchDataHelpers.CalculateNetObjectSubMeshData(netObject);
			}
			Quantity quantity = default(Quantity);
			if (m_QuantityData.TryGetComponent(entity, ref quantity))
			{
				QuantityObjectData quantityObjectData = default(QuantityObjectData);
				subMeshFlags = ((!m_PrefabQuantityObjectData.TryGetComponent(prefabRef.m_Prefab, ref quantityObjectData)) ? (subMeshFlags | BatchDataHelpers.CalculateQuantitySubMeshData(quantity, default(QuantityObjectData), m_EditorMode)) : (subMeshFlags | BatchDataHelpers.CalculateQuantitySubMeshData(quantity, quantityObjectData, m_EditorMode)));
			}
			UnderConstruction underConstruction = default(UnderConstruction);
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			if ((m_UnderConstructionData.TryGetComponent(entity, ref underConstruction) && underConstruction.m_NewPrefab == Entity.Null) || (m_DestroyedData.HasComponent(entity) && m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData) && (objectGeometryData.m_Flags & (GeometryFlags.Physical | GeometryFlags.HasLot)) == (GeometryFlags.Physical | GeometryFlags.HasLot)))
			{
				return;
			}
			Stack stack = default(Stack);
			StackData stackData = default(StackData);
			if (m_StackData.TryGetComponent(entity, ref stack) && m_PrefabStackData.TryGetComponent(prefabRef.m_Prefab, ref stackData))
			{
				subMeshFlags |= BatchDataHelpers.CalculateStackSubMeshData(stack, stackData, out tileCounts, out offsets, out scale);
			}
			else
			{
				stackData = default(StackData);
			}
			DynamicBuffer<MeshGroup> val2 = default(DynamicBuffer<MeshGroup>);
			int num = 1;
			DynamicBuffer<SubMeshGroup> val3 = default(DynamicBuffer<SubMeshGroup>);
			if (m_SubMeshGroups.TryGetBuffer(prefabRef.m_Prefab, ref val3) && m_MeshGroups.TryGetBuffer(entity, ref val2))
			{
				num = val2.Length;
			}
			MeshGroup meshGroup = default(MeshGroup);
			SubMeshGroup subMeshGroup = default(SubMeshGroup);
			DynamicBuffer<LodMesh> val5 = default(DynamicBuffer<LodMesh>);
			ImpostorData impostorData = default(ImpostorData);
			for (int i = 0; i < num; i++)
			{
				if (val3.IsCreated)
				{
					CollectionUtils.TryGet<MeshGroup>(val2, i, ref meshGroup);
					subMeshGroup = val3[(int)meshGroup.m_SubMeshGroup];
				}
				else
				{
					subMeshGroup.m_SubMeshRange = new int2(0, val.Length);
					meshGroup = default(MeshGroup);
				}
				for (int j = subMeshGroup.m_SubMeshRange.x; j < subMeshGroup.m_SubMeshRange.y; j++)
				{
					SubMesh subMesh = val[j];
					if ((subMesh.m_Flags & subMeshFlags) != subMesh.m_Flags)
					{
						continue;
					}
					Entity val4 = Entity.Null;
					if (HasCachedMesh(subMesh.m_SubMesh, out var sharedMesh))
					{
						val4 = sharedMesh;
					}
					else if (m_Lods.TryGetBuffer(subMesh.m_SubMesh, ref val5))
					{
						for (int num2 = val5.Length - 1; num2 >= 0; num2--)
						{
							if (HasCachedMesh(val5[num2].m_LodMesh, out sharedMesh))
							{
								val4 = sharedMesh;
								break;
							}
						}
					}
					if (val4 == Entity.Null || (m_PrefabMeshData[val4].m_State & MeshFlags.Decal) != 0)
					{
						continue;
					}
					int num3 = 1;
					num3 = math.select(num3, tileCounts.x, (subMesh.m_Flags & SubMeshFlags.IsStackStart) != 0);
					num3 = math.select(num3, tileCounts.y, (subMesh.m_Flags & SubMeshFlags.IsStackMiddle) != 0);
					num3 = math.select(num3, tileCounts.z, (subMesh.m_Flags & SubMeshFlags.IsStackEnd) != 0);
					if (num3 < 1)
					{
						continue;
					}
					DynamicBuffer<MeshVertex> vertices = m_Vertices[val4];
					DynamicBuffer<MeshIndex> indices = m_Indices[val4];
					DynamicBuffer<MeshNode> nodes = default(DynamicBuffer<MeshNode>);
					DynamicBuffer<ProceduralBone> prefabBones = default(DynamicBuffer<ProceduralBone>);
					DynamicBuffer<Bone> bones = default(DynamicBuffer<Bone>);
					DynamicBuffer<Skeleton> val6 = default(DynamicBuffer<Skeleton>);
					if (m_Nodes.TryGetBuffer(val4, ref nodes) && m_ProceduralBones.TryGetBuffer(val4, ref prefabBones) && m_Bones.TryGetBuffer(entity, ref bones))
					{
						val6 = m_Skeletons[entity];
						if (val6.Length == 0)
						{
							bones = default(DynamicBuffer<Bone>);
							val6 = default(DynamicBuffer<Skeleton>);
						}
					}
					for (int k = 0; k < num3; k++)
					{
						SubMesh subMesh2 = subMesh;
						Line line2 = line;
						if ((subMesh2.m_Flags & (SubMeshFlags.IsStackStart | SubMeshFlags.IsStackMiddle | SubMeshFlags.IsStackEnd)) != 0)
						{
							BatchDataHelpers.CalculateStackSubMeshData(stackData, offsets, scale, k, subMesh.m_Flags, ref subMesh2.m_Position, ref line2.m_Scale);
						}
						if ((subMesh2.m_Flags & (SubMeshFlags.IsStackStart | SubMeshFlags.IsStackMiddle | SubMeshFlags.IsStackEnd | SubMeshFlags.HasTransform)) != 0)
						{
							quaternion val7 = math.inverse(subMesh2.m_Rotation);
							line2.m_Line.a = math.mul(val7, line2.m_Line.a - subMesh2.m_Position);
							line2.m_Line.b = math.mul(val7, line2.m_Line.b - subMesh2.m_Position);
							line2.m_XVector = math.mul(val7, line2.m_XVector);
							line2.m_YVector = math.mul(val7, line2.m_YVector);
						}
						if (m_PrefabImpostorData.TryGetComponent(val4, ref impostorData) && impostorData.m_Size != 0f)
						{
							ref float3 reference = ref line2.m_Scale;
							reference *= impostorData.m_Size;
							ref float3 a = ref line2.m_Line.a;
							a -= impostorData.m_Offset;
							ref float3 b = ref line2.m_Line.b;
							b -= impostorData.m_Offset;
						}
						line2.m_Expand = math.abs(line2.m_XVector) + math.abs(line2.m_YVector);
						if (nodes.IsCreated)
						{
							if (prefabBones.IsCreated)
							{
								if (bones.IsCreated)
								{
									int num4 = j - subMeshGroup.m_SubMeshRange.x + meshGroup.m_MeshOffset;
									CheckMeshIntersect(line2, vertices, indices, nodes, prefabBones, bones, val6[num4], collisions);
								}
								else
								{
									CheckMeshIntersect(line2, vertices, indices, nodes, prefabBones, collisions);
								}
							}
							else
							{
								CheckMeshIntersect(line2, vertices, indices, nodes, collisions);
							}
						}
						else
						{
							CheckMeshIntersect(line2, vertices, indices, collisions);
						}
					}
				}
			}
		}
	}

	[BurstCompile]
	private struct SelectCameraPositionJob : IJob
	{
		[ReadOnly]
		public Segment m_Line;

		[ReadOnly]
		public float3 m_PreviousPosition;

		[ReadOnly]
		public float m_MinClearRange;

		[ReadOnly]
		public float m_NearPlaneRange;

		[ReadOnly]
		public float m_Smoothing;

		[ReadOnly]
		public float m_DeltaTime;

		[ReadOnly]
		public TerrainHeightData m_TerrainData;

		[ReadOnly]
		public WaterSurfaceData m_WaterData;

		public NativeQueue<Collision> m_Collisions;

		public NativeReference<Result> m_Result;

		public void Execute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			ref Result reference = ref CollectionUtils.ValueAsRef<Result>(m_Result);
			NativeList<Collision> collisions = default(NativeList<Collision>);
			collisions._002Ector(m_Collisions.Count, AllocatorHandle.op_Implicit((Allocator)2));
			Collision collision = default(Collision);
			while (m_Collisions.TryDequeue(ref collision))
			{
				collisions.Add(ref collision);
			}
			CheckCollisions(collisions, m_MinClearRange, new float2(0f, 1f));
			float num = default(float);
			MathUtils.Distance(m_Line, reference.m_Position, ref num);
			float num2 = default(float);
			MathUtils.Distance(m_Line, m_PreviousPosition, ref num2);
			float num3 = 0f;
			float num4 = (num + num2) * 0.5f;
			float num5 = num;
			float num6 = float.MaxValue;
			float2 val = float2.op_Implicit(num);
			bool flag = true;
			for (int i = 0; i <= collisions.Length; i++)
			{
				float num7 = 1f;
				float num8 = 1f;
				if (i < collisions.Length)
				{
					Collision collision2 = collisions[i];
					num7 = collision2.m_LineBounds.min;
					num8 = collision2.m_LineBounds.max;
				}
				if (num7 - num3 >= m_MinClearRange)
				{
					float num9;
					float num10;
					if (num3 > num)
					{
						num9 = num3;
						num10 = math.select(float.MaxValue, math.abs(num9 - num4), CheckOffset(num9));
					}
					else if (num7 - m_MinClearRange < num)
					{
						num9 = num7 - m_MinClearRange;
						num10 = math.select(float.MaxValue, math.abs(num9 - num4), CheckOffset(num9));
					}
					else
					{
						num9 = num;
						num10 = 0f;
					}
					if (num10 < num6 || (flag && num10 != float.MaxValue))
					{
						num5 = num9;
						num6 = num10;
						val = new float2(num3, num7) - m_NearPlaneRange;
						flag = false;
					}
				}
				else if (flag)
				{
					float num11;
					float num12;
					if (num3 > num)
					{
						num11 = num3;
						num12 = math.select(float.MaxValue, math.abs(num11 - num4), CheckOffset(num11));
					}
					else if (num7 - m_MinClearRange < num)
					{
						num11 = num7 - m_MinClearRange;
						num12 = math.select(float.MaxValue, math.abs(num11 - num4), CheckOffset(num11));
					}
					else
					{
						num11 = num;
						num12 = float.MaxValue;
					}
					if (num12 < num6)
					{
						num5 = num11;
						num6 = num12;
						val = new float2(num3, num7) - m_NearPlaneRange;
					}
				}
				num3 = num8;
			}
			val.x = math.min(val.x, num5);
			val.y = math.max(val.y, num5);
			num5 += (reference.m_Offset - (num5 - num)) * math.pow(m_Smoothing, m_DeltaTime);
			num5 = math.clamp(num5, val.x, val.y);
			if (num5 != num)
			{
				reference.m_Position = MathUtils.Position(m_Line, num5);
			}
			reference.m_Offset = num5 - num;
			collisions.Dispose();
		}

		private bool CheckOffset(float offset)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			float3 val = MathUtils.Position(m_Line, offset);
			return val.y >= WaterUtils.SampleHeight(ref m_WaterData, ref m_TerrainData, val) + m_MinClearRange;
		}
	}

	private struct Line
	{
		public Segment m_Line;

		public float3 m_XVector;

		public float3 m_YVector;

		public float2 m_CutOffset;

		public float3 m_Expand;

		public float3 m_Scale;
	}

	private struct Collision : IComparable<Collision>
	{
		public Bounds1 m_LineBounds;

		public float2 m_CoverAreas;

		public bool2 m_StartEnd;

		public int CompareTo(Collision other)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return m_LineBounds.min.CompareTo(other.m_LineBounds.min);
		}
	}

	private struct Result
	{
		public float3 m_Position;

		public float m_Offset;
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Tree> __Game_Objects_Tree_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.NetObject> __Game_Objects_NetObject_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Quantity> __Game_Objects_Quantity_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Stack> __Game_Objects_Stack_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UnderConstruction> __Game_Objects_UnderConstruction_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MeshData> __Game_Prefabs_MeshData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ImpostorData> __Game_Prefabs_ImpostorData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SharedMeshData> __Game_Prefabs_SharedMeshData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GrowthScaleData> __Game_Prefabs_GrowthScaleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<QuantityObjectData> __Game_Prefabs_QuantityObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StackData> __Game_Prefabs_StackData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<MeshGroup> __Game_Rendering_MeshGroup_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Skeleton> __Game_Rendering_Skeleton_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Bone> __Game_Rendering_Bone_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMesh> __Game_Prefabs_SubMesh_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> __Game_Prefabs_SubMeshGroup_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LodMesh> __Game_Prefabs_LodMesh_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<MeshVertex> __Game_Prefabs_MeshVertex_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<MeshIndex> __Game_Prefabs_MeshIndex_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<MeshNode> __Game_Prefabs_MeshNode_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ProceduralBone> __Game_Prefabs_ProceduralBone_RO_BufferLookup;

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
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Tree_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Tree>(true);
			__Game_Objects_NetObject_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.NetObject>(true);
			__Game_Objects_Quantity_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Quantity>(true);
			__Game_Objects_Stack_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Stack>(true);
			__Game_Objects_UnderConstruction_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UnderConstruction>(true);
			__Game_Objects_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.OutsideConnection>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_MeshData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MeshData>(true);
			__Game_Prefabs_ImpostorData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ImpostorData>(true);
			__Game_Prefabs_SharedMeshData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SharedMeshData>(true);
			__Game_Prefabs_GrowthScaleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GrowthScaleData>(true);
			__Game_Prefabs_QuantityObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<QuantityObjectData>(true);
			__Game_Prefabs_StackData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StackData>(true);
			__Game_Rendering_MeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshGroup>(true);
			__Game_Rendering_Skeleton_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Skeleton>(true);
			__Game_Rendering_Bone_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Bone>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
			__Game_Prefabs_SubMeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMeshGroup>(true);
			__Game_Prefabs_LodMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LodMesh>(true);
			__Game_Prefabs_MeshVertex_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshVertex>(true);
			__Game_Prefabs_MeshIndex_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshIndex>(true);
			__Game_Prefabs_MeshNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshNode>(true);
			__Game_Prefabs_ProceduralBone_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ProceduralBone>(true);
		}
	}

	private CityConfigurationSystem m_CityConfigurationSystem;

	private ToolSystem m_ToolSystem;

	private SearchSystem m_ObjectSearchSystem;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private float3 m_PreviousPosition;

	private quaternion m_Rotation;

	private float m_MaxForwardOffset;

	private float m_MaxBackwardOffset;

	private float m_MinClearDistance;

	private float m_NearPlane;

	private float m_Smoothing;

	private float2 m_FieldOfView;

	private NativeReference<Result> m_Result;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_Result = new NativeReference<Result>(AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Result.Dispose();
		base.OnDestroy();
	}

	public void CheckCollisions(ref float3 position, float3 previousPosition, quaternion rotation, float maxForwardOffset, float maxBackwardOffset, float minClearDistance, float nearPlane, float smoothing, float2 fieldOfView)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (!m_ToolSystem.actionMode.IsEditor())
		{
			CollectionUtils.ValueAsRef<Result>(m_Result).m_Position = position;
			m_PreviousPosition = previousPosition;
			m_Rotation = rotation;
			m_MaxForwardOffset = maxForwardOffset;
			m_MaxBackwardOffset = maxBackwardOffset;
			m_MinClearDistance = minClearDistance;
			m_NearPlane = nearPlane;
			m_Smoothing = smoothing;
			m_FieldOfView = fieldOfView;
			((ComponentSystemBase)this).Update();
			position = CollectionUtils.ValueAsRef<Result>(m_Result).m_Position;
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		float3 val = m_Result.Value.m_Position;
		float3 val2 = math.forward(m_Rotation);
		float num = m_MaxForwardOffset + m_MinClearDistance;
		float num2 = m_MaxBackwardOffset;
		Segment line = default(Segment);
		((Segment)(ref line))._002Ector(val - val2 * num2, val + val2 * num);
		float2 fovOffset = math.tan(math.radians(m_FieldOfView) * 0.5f) * m_MinClearDistance;
		float minClearRange = m_MinClearDistance / (num + num2);
		float nearPlaneRange = m_NearPlane / (num + num2);
		NativeList<Entity> val3 = default(NativeList<Entity>);
		val3._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<Collision> collisions = default(NativeQueue<Collision>);
		collisions._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle dependencies;
		FindEntitiesFromTreeJob findEntitiesFromTreeJob = new FindEntitiesFromTreeJob
		{
			m_Line = line,
			m_Rotation = m_Rotation,
			m_FovOffset = fovOffset,
			m_SearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies),
			m_EntityList = val3
		};
		ObjectCollisionJob objectCollisionJob = new ObjectCollisionJob
		{
			m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TreeData = InternalCompilerInterface.GetComponentLookup<Tree>(ref __TypeHandle.__Game_Objects_Tree_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetObjectData = InternalCompilerInterface.GetComponentLookup<Game.Objects.NetObject>(ref __TypeHandle.__Game_Objects_NetObject_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_QuantityData = InternalCompilerInterface.GetComponentLookup<Quantity>(ref __TypeHandle.__Game_Objects_Quantity_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StackData = InternalCompilerInterface.GetComponentLookup<Stack>(ref __TypeHandle.__Game_Objects_Stack_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UnderConstructionData = InternalCompilerInterface.GetComponentLookup<UnderConstruction>(ref __TypeHandle.__Game_Objects_UnderConstruction_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionData = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabMeshData = InternalCompilerInterface.GetComponentLookup<MeshData>(ref __TypeHandle.__Game_Prefabs_MeshData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabImpostorData = InternalCompilerInterface.GetComponentLookup<ImpostorData>(ref __TypeHandle.__Game_Prefabs_ImpostorData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSharedMeshData = InternalCompilerInterface.GetComponentLookup<SharedMeshData>(ref __TypeHandle.__Game_Prefabs_SharedMeshData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGrowthScaleData = InternalCompilerInterface.GetComponentLookup<GrowthScaleData>(ref __TypeHandle.__Game_Prefabs_GrowthScaleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabQuantityObjectData = InternalCompilerInterface.GetComponentLookup<QuantityObjectData>(ref __TypeHandle.__Game_Prefabs_QuantityObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabStackData = InternalCompilerInterface.GetComponentLookup<StackData>(ref __TypeHandle.__Game_Prefabs_StackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeshGroups = InternalCompilerInterface.GetBufferLookup<MeshGroup>(ref __TypeHandle.__Game_Rendering_MeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Skeletons = InternalCompilerInterface.GetBufferLookup<Skeleton>(ref __TypeHandle.__Game_Rendering_Skeleton_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Bones = InternalCompilerInterface.GetBufferLookup<Bone>(ref __TypeHandle.__Game_Rendering_Bone_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Meshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshGroups = InternalCompilerInterface.GetBufferLookup<SubMeshGroup>(ref __TypeHandle.__Game_Prefabs_SubMeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Lods = InternalCompilerInterface.GetBufferLookup<LodMesh>(ref __TypeHandle.__Game_Prefabs_LodMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Vertices = InternalCompilerInterface.GetBufferLookup<MeshVertex>(ref __TypeHandle.__Game_Prefabs_MeshVertex_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Indices = InternalCompilerInterface.GetBufferLookup<MeshIndex>(ref __TypeHandle.__Game_Prefabs_MeshIndex_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Nodes = InternalCompilerInterface.GetBufferLookup<MeshNode>(ref __TypeHandle.__Game_Prefabs_MeshNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ProceduralBones = InternalCompilerInterface.GetBufferLookup<ProceduralBone>(ref __TypeHandle.__Game_Prefabs_ProceduralBone_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Line = line,
			m_Rotation = m_Rotation,
			m_FovOffset = fovOffset,
			m_MinClearRange = minClearRange,
			m_LeftHandTraffic = m_CityConfigurationSystem.leftHandTraffic,
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_EntityList = val3,
			m_Collisions = collisions.AsParallelWriter()
		};
		JobHandle deps;
		SelectCameraPositionJob obj = new SelectCameraPositionJob
		{
			m_Line = line,
			m_PreviousPosition = m_PreviousPosition,
			m_MinClearRange = minClearRange,
			m_NearPlaneRange = nearPlaneRange,
			m_Smoothing = m_Smoothing,
			m_DeltaTime = Time.deltaTime,
			m_TerrainData = m_TerrainSystem.GetHeightData(),
			m_WaterData = m_WaterSystem.GetSurfaceData(out deps),
			m_Collisions = collisions,
			m_Result = m_Result
		};
		JobHandle val4 = IJobExtensions.Schedule<FindEntitiesFromTreeJob>(findEntitiesFromTreeJob, dependencies);
		JobHandle val5 = IJobParallelForDeferExtensions.Schedule<ObjectCollisionJob, Entity>(objectCollisionJob, val3, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val4));
		JobHandle val6 = IJobExtensions.Schedule<SelectCameraPositionJob>(obj, JobHandle.CombineDependencies(val5, deps));
		m_ObjectSearchSystem.AddStaticSearchTreeReader(val4);
		m_TerrainSystem.AddCPUHeightReader(val6);
		m_WaterSystem.AddSurfaceReader(val6);
		val3.Dispose(val5);
		collisions.Dispose(val6);
		((JobHandle)(ref val6)).Complete();
	}

	private static void CheckCollisions(NativeList<Collision> collisions, float minClearRange, float2 limits)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		if (collisions.Length == 0)
		{
			return;
		}
		NativeSortExtension.Sort<Collision>(collisions);
		int num = 0;
		Collision collision = collisions[0];
		for (int i = 1; i < collisions.Length; i++)
		{
			Collision collision2 = collisions[i];
			if (collision2.m_LineBounds.min - collision.m_LineBounds.max < minClearRange)
			{
				collision.m_LineBounds.max = math.max(collision.m_LineBounds.max, collision2.m_LineBounds.max);
				ref float2 reference = ref collision.m_CoverAreas;
				reference += collision2.m_CoverAreas;
				ref bool2 reference2 = ref collision.m_StartEnd;
				reference2 |= collision2.m_StartEnd;
			}
			else
			{
				ref bool2 reference3 = ref collision.m_StartEnd;
				reference3 &= collision.m_CoverAreas >= ((float2)(ref collision.m_CoverAreas)).yx * 0.5f;
				collisions[num++] = collision;
				collision = collision2;
			}
		}
		ref bool2 reference4 = ref collision.m_StartEnd;
		reference4 &= collision.m_CoverAreas >= ((float2)(ref collision.m_CoverAreas)).yx * 0.5f;
		collisions[num++] = collision;
		collisions.RemoveRange(num, collisions.Length - num);
		num = 0;
		collision = collisions[0];
		if (!collision.m_StartEnd.x)
		{
			collision.m_LineBounds.min = math.min(collision.m_LineBounds.min, limits.x);
			collision.m_StartEnd.x = true;
		}
		for (int j = 1; j < collisions.Length; j++)
		{
			Collision collision3 = collisions[j];
			if (!collision.m_StartEnd.y || !collision3.m_StartEnd.x)
			{
				collision.m_LineBounds.max = collision3.m_LineBounds.max;
				ref float2 reference5 = ref collision.m_CoverAreas;
				reference5 += collision3.m_CoverAreas;
				collision.m_StartEnd.y = collision3.m_StartEnd.y;
			}
			else
			{
				collisions[num++] = collision;
				collision = collision3;
			}
		}
		if (!collision.m_StartEnd.y)
		{
			collision.m_LineBounds.max = math.max(collision.m_LineBounds.max, limits.y);
			collision.m_StartEnd.y = true;
		}
		collisions[num++] = collision;
		collisions.RemoveRange(num, collisions.Length - num);
	}

	private static bool Intersect(Line line, Bounds3 bounds, out float2 t)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		bounds = MathUtils.Expand(bounds, line.m_Expand) * line.m_Scale;
		return MathUtils.Intersect(bounds, line.m_Line, ref t);
	}

	private static void CheckTriangleIntersect(Line line, Triangle3 triangle, NativeList<Collision> collisions)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		triangle *= line.m_Scale;
		float2 val = default(float2);
		if (!MathUtils.Intersect(MathUtils.Expand(MathUtils.Bounds(triangle), line.m_Expand), line.m_Line, ref val))
		{
			return;
		}
		float3 val2 = triangle.a - line.m_Line.a;
		float3 val3 = triangle.b - line.m_Line.a;
		float3 val4 = triangle.c - line.m_Line.a;
		Bounds2 val5 = default(Bounds2);
		val5.max = new float2(math.lengthsq(line.m_XVector), math.lengthsq(line.m_YVector));
		val5.min = -val5.max;
		Triangle2 val6 = default(Triangle2);
		val6.a = new float2(math.dot(val2, line.m_XVector), math.dot(val2, line.m_YVector));
		val6.b = new float2(math.dot(val3, line.m_XVector), math.dot(val3, line.m_YVector));
		val6.c = new float2(math.dot(val4, line.m_XVector), math.dot(val4, line.m_YVector));
		float num = default(float);
		if (!MathUtils.Intersect(val5, val6, ref num))
		{
			return;
		}
		float3 val7 = line.m_Line.b - line.m_Line.a;
		float3 val8 = val7 * (1f / math.lengthsq(val7));
		Triangle1 val9 = default(Triangle1);
		val9.a = math.dot(val2, val8);
		val9.b = math.dot(val3, val8);
		val9.c = math.dot(val4, val8);
		Bounds1 val10 = default(Bounds1);
		((Bounds1)(ref val10))._002Ector(float.MaxValue, float.MinValue);
		float2 val11 = default(float2);
		if (MathUtils.Intersect(val5, ((Triangle2)(ref val6)).ab, ref val11))
		{
			val11 = math.lerp(float2.op_Implicit(val9.a), float2.op_Implicit(val9.b), val11);
			val10.min = math.min(val10.min, math.cmin(val11));
			val10.max = math.max(val10.max, math.cmax(val11));
		}
		if (MathUtils.Intersect(val5, ((Triangle2)(ref val6)).bc, ref val11))
		{
			val11 = math.lerp(float2.op_Implicit(val9.b), float2.op_Implicit(val9.c), val11);
			val10.min = math.min(val10.min, math.cmin(val11));
			val10.max = math.max(val10.max, math.cmax(val11));
		}
		if (MathUtils.Intersect(val5, ((Triangle2)(ref val6)).ca, ref val11))
		{
			val11 = math.lerp(float2.op_Implicit(val9.c), float2.op_Implicit(val9.a), val11);
			val10.min = math.min(val10.min, math.cmin(val11));
			val10.max = math.max(val10.max, math.cmax(val11));
		}
		if (MathUtils.Intersect(val6, val5.min, ref val11))
		{
			val10 |= MathUtils.Position(val9, val11);
		}
		if (MathUtils.Intersect(val6, new float2(val5.max.x, val5.min.y), ref val11))
		{
			val10 |= MathUtils.Position(val9, val11);
		}
		if (MathUtils.Intersect(val6, new float2(val5.min.x, val5.max.y), ref val11))
		{
			val10 |= MathUtils.Position(val9, val11);
		}
		if (MathUtils.Intersect(val6, val5.max, ref val11))
		{
			val10 |= MathUtils.Position(val9, val11);
		}
		if (val10.min <= 1f && val10.max >= 0f)
		{
			Collision collision = default(Collision);
			collision.m_LineBounds.min = math.lerp(line.m_CutOffset.x, line.m_CutOffset.y, val10.min);
			collision.m_LineBounds.max = math.lerp(line.m_CutOffset.x, line.m_CutOffset.y, val10.max);
			if (MathUtils.IsClockwise(val6))
			{
				collision.m_CoverAreas = new float2(num, 0f);
				collision.m_StartEnd = new bool2(true, false);
			}
			else
			{
				collision.m_CoverAreas = new float2(0f, num);
				collision.m_StartEnd = new bool2(false, true);
			}
			collisions.Add(ref collision);
		}
	}

	private static void CheckMeshIntersect(Line line, DynamicBuffer<MeshVertex> vertices, DynamicBuffer<MeshIndex> indices, NativeList<Collision> collisions)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Triangle3 triangle = default(Triangle3);
		for (int i = 0; i < indices.Length; i += 3)
		{
			((Triangle3)(ref triangle))._002Ector(vertices[indices[i].m_Index].m_Vertex, vertices[indices[i + 1].m_Index].m_Vertex, vertices[indices[i + 2].m_Index].m_Vertex);
			CheckTriangleIntersect(line, triangle, collisions);
		}
	}

	private unsafe static void CheckMeshIntersect(Line line, DynamicBuffer<MeshVertex> vertices, DynamicBuffer<MeshIndex> indices, DynamicBuffer<MeshNode> nodes, NativeList<Collision> collisions)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		int* ptr = stackalloc int[128];
		int num = 0;
		if (nodes.Length != 0)
		{
			ptr[num++] = 0;
		}
		Triangle3 triangle = default(Triangle3);
		while (--num >= 0)
		{
			int num2 = ptr[num];
			MeshNode meshNode = nodes[num2];
			if (Intersect(line, meshNode.m_Bounds, out var _))
			{
				for (int i = meshNode.m_IndexRange.x; i < meshNode.m_IndexRange.y; i += 3)
				{
					((Triangle3)(ref triangle))._002Ector(vertices[indices[i].m_Index].m_Vertex, vertices[indices[i + 1].m_Index].m_Vertex, vertices[indices[i + 2].m_Index].m_Vertex);
					CheckTriangleIntersect(line, triangle, collisions);
				}
				ptr[num] = meshNode.m_SubNodes1.x;
				num = math.select(num, num + 1, meshNode.m_SubNodes1.x != -1);
				ptr[num] = meshNode.m_SubNodes1.y;
				num = math.select(num, num + 1, meshNode.m_SubNodes1.y != -1);
				ptr[num] = meshNode.m_SubNodes1.z;
				num = math.select(num, num + 1, meshNode.m_SubNodes1.z != -1);
				ptr[num] = meshNode.m_SubNodes1.w;
				num = math.select(num, num + 1, meshNode.m_SubNodes1.w != -1);
				ptr[num] = meshNode.m_SubNodes2.x;
				num = math.select(num, num + 1, meshNode.m_SubNodes2.x != -1);
				ptr[num] = meshNode.m_SubNodes2.y;
				num = math.select(num, num + 1, meshNode.m_SubNodes2.y != -1);
				ptr[num] = meshNode.m_SubNodes2.z;
				num = math.select(num, num + 1, meshNode.m_SubNodes2.z != -1);
				ptr[num] = meshNode.m_SubNodes2.w;
				num = math.select(num, num + 1, meshNode.m_SubNodes2.w != -1);
			}
		}
	}

	private unsafe static void CheckMeshIntersect(Line line, DynamicBuffer<MeshVertex> vertices, DynamicBuffer<MeshIndex> indices, DynamicBuffer<MeshNode> nodes, DynamicBuffer<ProceduralBone> prefabBones, NativeList<Collision> collisions)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		int* ptr = stackalloc int[128];
		Triangle3 triangle = default(Triangle3);
		for (int i = 0; i < prefabBones.Length; i++)
		{
			int num = 0;
			if (math.any(MathUtils.Size(nodes[i].m_Bounds) > 0f))
			{
				ptr[num++] = i;
			}
			while (--num >= 0)
			{
				int num2 = ptr[num];
				MeshNode meshNode = nodes[num2];
				if (Intersect(line, meshNode.m_Bounds, out var _))
				{
					for (int j = meshNode.m_IndexRange.x; j < meshNode.m_IndexRange.y; j += 3)
					{
						((Triangle3)(ref triangle))._002Ector(vertices[indices[j].m_Index].m_Vertex, vertices[indices[j + 1].m_Index].m_Vertex, vertices[indices[j + 2].m_Index].m_Vertex);
						CheckTriangleIntersect(line, triangle, collisions);
					}
					ptr[num] = meshNode.m_SubNodes1.x;
					num = math.select(num, num + 1, meshNode.m_SubNodes1.x != -1);
					ptr[num] = meshNode.m_SubNodes1.y;
					num = math.select(num, num + 1, meshNode.m_SubNodes1.y != -1);
					ptr[num] = meshNode.m_SubNodes1.z;
					num = math.select(num, num + 1, meshNode.m_SubNodes1.z != -1);
					ptr[num] = meshNode.m_SubNodes1.w;
					num = math.select(num, num + 1, meshNode.m_SubNodes1.w != -1);
					ptr[num] = meshNode.m_SubNodes2.x;
					num = math.select(num, num + 1, meshNode.m_SubNodes2.x != -1);
					ptr[num] = meshNode.m_SubNodes2.y;
					num = math.select(num, num + 1, meshNode.m_SubNodes2.y != -1);
					ptr[num] = meshNode.m_SubNodes2.z;
					num = math.select(num, num + 1, meshNode.m_SubNodes2.z != -1);
					ptr[num] = meshNode.m_SubNodes2.w;
					num = math.select(num, num + 1, meshNode.m_SubNodes2.w != -1);
				}
			}
		}
	}

	private unsafe static void CheckMeshIntersect(Line line, DynamicBuffer<MeshVertex> vertices, DynamicBuffer<MeshIndex> indices, DynamicBuffer<MeshNode> nodes, DynamicBuffer<ProceduralBone> prefabBones, DynamicBuffer<Bone> bones, Skeleton skeleton, NativeList<Collision> collisions)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		int* ptr = stackalloc int[128];
		Triangle3 triangle = default(Triangle3);
		for (int i = 0; i < prefabBones.Length; i++)
		{
			int num = 0;
			Line line2 = line;
			ProceduralBone proceduralBone = prefabBones[i];
			if (math.any(MathUtils.Size(nodes[proceduralBone.m_BindIndex].m_Bounds) > 0f))
			{
				ptr[num++] = proceduralBone.m_BindIndex;
				Bone bone = bones[skeleton.m_BoneOffset + i];
				float4x4 val = float4x4.TRS(bone.m_Position, bone.m_Rotation, bone.m_Scale);
				int parentIndex = proceduralBone.m_ParentIndex;
				while (parentIndex >= 0)
				{
					Bone bone2 = bones[skeleton.m_BoneOffset + parentIndex];
					ProceduralBone proceduralBone2 = prefabBones[parentIndex];
					val = math.mul(float4x4.TRS(bone2.m_Position, bone2.m_Rotation, bone2.m_Scale), val);
					parentIndex = proceduralBone2.m_ParentIndex;
				}
				val = math.mul(val, proceduralBone.m_BindPose);
				val = math.inverse(val);
				ref Segment reference = ref line2.m_Line;
				float4 val2 = math.mul(val, new float4(line.m_Line.a, 1f));
				reference.a = ((float4)(ref val2)).xyz;
				ref Segment reference2 = ref line2.m_Line;
				val2 = math.mul(val, new float4(line.m_Line.b, 1f));
				reference2.b = ((float4)(ref val2)).xyz;
				val2 = math.mul(val, new float4(line.m_XVector, 0f));
				line2.m_XVector = ((float4)(ref val2)).xyz;
				val2 = math.mul(val, new float4(line.m_YVector, 0f));
				line2.m_YVector = ((float4)(ref val2)).xyz;
				line2.m_Expand = math.abs(line2.m_XVector) + math.abs(line2.m_YVector);
			}
			while (--num >= 0)
			{
				int num2 = ptr[num];
				MeshNode meshNode = nodes[num2];
				if (Intersect(line2, meshNode.m_Bounds, out var _))
				{
					for (int j = meshNode.m_IndexRange.x; j < meshNode.m_IndexRange.y; j += 3)
					{
						((Triangle3)(ref triangle))._002Ector(vertices[indices[j].m_Index].m_Vertex, vertices[indices[j + 1].m_Index].m_Vertex, vertices[indices[j + 2].m_Index].m_Vertex);
						CheckTriangleIntersect(line2, triangle, collisions);
					}
					ptr[num] = meshNode.m_SubNodes1.x;
					num = math.select(num, num + 1, meshNode.m_SubNodes1.x != -1);
					ptr[num] = meshNode.m_SubNodes1.y;
					num = math.select(num, num + 1, meshNode.m_SubNodes1.y != -1);
					ptr[num] = meshNode.m_SubNodes1.z;
					num = math.select(num, num + 1, meshNode.m_SubNodes1.z != -1);
					ptr[num] = meshNode.m_SubNodes1.w;
					num = math.select(num, num + 1, meshNode.m_SubNodes1.w != -1);
					ptr[num] = meshNode.m_SubNodes2.x;
					num = math.select(num, num + 1, meshNode.m_SubNodes2.x != -1);
					ptr[num] = meshNode.m_SubNodes2.y;
					num = math.select(num, num + 1, meshNode.m_SubNodes2.y != -1);
					ptr[num] = meshNode.m_SubNodes2.z;
					num = math.select(num, num + 1, meshNode.m_SubNodes2.z != -1);
					ptr[num] = meshNode.m_SubNodes2.w;
					num = math.select(num, num + 1, meshNode.m_SubNodes2.w != -1);
				}
			}
		}
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
	public CameraCollisionSystem()
	{
	}
}
