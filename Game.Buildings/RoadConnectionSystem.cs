using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Audio;
using Game.Common;
using Game.Effects;
using Game.Net;
using Game.Notifications;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Buildings;

[CompilerGenerated]
public class RoadConnectionSystem : GameSystemBase
{
	[BurstCompile]
	private struct CheckRoadConnectionJob : IJobChunk
	{
		private struct CheckRoadConnectionIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds3 m_Bounds;

			public EdgeGeometry m_EdgeGeometry;

			public EdgeNodeGeometry m_StartGeometry;

			public EdgeNodeGeometry m_EndGeometry;

			public ComponentLookup<Building> m_BuildingData;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<BuildingData> m_PrefabBuildingData;

			public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

			public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

			public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

			public ParallelWriter<Entity> m_ReplaceRoadConnectionQueue;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_Bounds)).xz);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity buildingEntity)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0164: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_010c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0125: Unknown result type (might be due to invalid IL or missing references)
				//IL_0141: Unknown result type (might be due to invalid IL or missing references)
				//IL_0157: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_Bounds)).xz) || !m_BuildingData.HasComponent(buildingEntity))
				{
					return;
				}
				Transform transform = m_TransformData[buildingEntity];
				PrefabRef prefabRef = m_PrefabRefData[buildingEntity];
				BuildingData buildingData = m_PrefabBuildingData[prefabRef.m_Prefab];
				if ((buildingData.m_Flags & Game.Prefabs.BuildingFlags.NoRoadConnection) != 0)
				{
					return;
				}
				float3 position = BuildingUtils.CalculateFrontPosition(transform, buildingData.m_LotSize.y);
				if (!MathUtils.Intersect(((Bounds3)(ref m_Bounds)).xz, ((float3)(ref position)).xz))
				{
					return;
				}
				float maxDistance = 8.4f;
				bool canBeOnRoad = (buildingData.m_Flags & Game.Prefabs.BuildingFlags.CanBeOnRoad) != 0;
				CheckDistance(m_EdgeGeometry, m_StartGeometry, m_EndGeometry, position, canBeOnRoad, ref maxDistance);
				if (!(maxDistance < 8.4f))
				{
					return;
				}
				Building building = m_BuildingData[buildingEntity];
				if (building.m_RoadEdge != Entity.Null)
				{
					EdgeGeometry edgeGeometry = m_EdgeGeometryData[building.m_RoadEdge];
					EdgeNodeGeometry geometry = m_StartNodeGeometryData[building.m_RoadEdge].m_Geometry;
					EdgeNodeGeometry geometry2 = m_EndNodeGeometryData[building.m_RoadEdge].m_Geometry;
					float maxDistance2 = 8.4f;
					CheckDistance(edgeGeometry, geometry, geometry2, position, canBeOnRoad, ref maxDistance2);
					if (maxDistance < maxDistance2)
					{
						m_ReplaceRoadConnectionQueue.Enqueue(buildingEntity);
					}
				}
				else
				{
					m_ReplaceRoadConnectionQueue.Enqueue(buildingEntity);
				}
			}
		}

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<EdgeGeometry> m_EdgeGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<StartNodeGeometry> m_StartNodeGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<EndNodeGeometry> m_EndNodeGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.SpawnLocation> m_SpawnLocationType;

		[ReadOnly]
		public BufferTypeHandle<ConnectedBuilding> m_ConnectedBuildingType;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_ObjectSearchTree;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		public ParallelWriter<Entity> m_ReplaceRoadConnectionQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Game.Objects.SpawnLocation> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Objects.SpawnLocation>(ref m_SpawnLocationType);
			BufferAccessor<ConnectedBuilding> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedBuilding>(ref m_ConnectedBuildingType);
			if (bufferAccessor.Length != 0)
			{
				for (int i = 0; i < bufferAccessor.Length; i++)
				{
					DynamicBuffer<ConnectedBuilding> val = bufferAccessor[i];
					for (int j = 0; j < val.Length; j++)
					{
						m_ReplaceRoadConnectionQueue.Enqueue(val[j].m_Building);
					}
				}
				if (!((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType))
				{
					NativeArray<EdgeGeometry> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EdgeGeometry>(ref m_EdgeGeometryType);
					NativeArray<StartNodeGeometry> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<StartNodeGeometry>(ref m_StartNodeGeometryType);
					NativeArray<EndNodeGeometry> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EndNodeGeometry>(ref m_EndNodeGeometryType);
					for (int k = 0; k < nativeArray2.Length; k++)
					{
						EdgeGeometry edgeGeometry = nativeArray2[k];
						EdgeNodeGeometry geometry = nativeArray3[k].m_Geometry;
						EdgeNodeGeometry geometry2 = nativeArray4[k].m_Geometry;
						CheckRoadConnectionIterator checkRoadConnectionIterator = new CheckRoadConnectionIterator
						{
							m_Bounds = MathUtils.Expand(edgeGeometry.m_Bounds | geometry.m_Bounds | geometry2.m_Bounds, float3.op_Implicit(8.4f)),
							m_EdgeGeometry = edgeGeometry,
							m_StartGeometry = geometry,
							m_EndGeometry = geometry2,
							m_BuildingData = m_BuildingData,
							m_TransformData = m_TransformData,
							m_PrefabRefData = m_PrefabRefData,
							m_PrefabBuildingData = m_PrefabBuildingData,
							m_EdgeGeometryData = m_EdgeGeometryData,
							m_StartNodeGeometryData = m_StartNodeGeometryData,
							m_EndNodeGeometryData = m_EndNodeGeometryData,
							m_ReplaceRoadConnectionQueue = m_ReplaceRoadConnectionQueue
						};
						m_ObjectSearchTree.Iterate<CheckRoadConnectionIterator>(ref checkRoadConnectionIterator, 0);
					}
				}
			}
			else if (nativeArray.Length != 0)
			{
				NativeArray<Entity> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				Owner owner = default(Owner);
				for (int l = 0; l < nativeArray5.Length; l++)
				{
					Entity val2 = nativeArray5[l];
					while (m_OwnerData.TryGetComponent(val2, ref owner))
					{
						val2 = owner.m_Owner;
						if (m_BuildingData.HasComponent(val2))
						{
							m_ReplaceRoadConnectionQueue.Enqueue(val2);
						}
					}
				}
			}
			else
			{
				NativeArray<Entity> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				for (int m = 0; m < nativeArray6.Length; m++)
				{
					m_ReplaceRoadConnectionQueue.Enqueue(nativeArray6[m]);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FillReplacementListJob : IJob
	{
		public NativeQueue<Entity> m_ReplaceRoadConnectionQueue;

		public NativeList<ReplaceRoad> m_ReplaceRoadConnection;

		public void Execute()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			int count = m_ReplaceRoadConnectionQueue.Count;
			m_ReplaceRoadConnection.ResizeUninitialized(count);
			for (int i = 0; i < count; i++)
			{
				m_ReplaceRoadConnection[i] = new ReplaceRoad(m_ReplaceRoadConnectionQueue.Dequeue());
			}
			NativeSortExtension.Sort<ReplaceRoad>(m_ReplaceRoadConnection);
			Entity val = Entity.Null;
			int num = 0;
			int num2 = 0;
			while (num < m_ReplaceRoadConnection.Length)
			{
				ReplaceRoad replaceRoad = m_ReplaceRoadConnection[num++];
				if (replaceRoad.m_Building != val)
				{
					m_ReplaceRoadConnection[num2++] = replaceRoad;
					val = replaceRoad.m_Building;
				}
			}
			if (num2 < m_ReplaceRoadConnection.Length)
			{
				m_ReplaceRoadConnection.RemoveRange(num2, m_ReplaceRoadConnection.Length - num2);
			}
		}
	}

	private struct ReplaceRoad : IComparable<ReplaceRoad>
	{
		public Entity m_Building;

		public Entity m_NewRoad;

		public float3 m_FrontPos;

		public float m_CurvePos;

		public bool m_Deleted;

		public bool m_Required;

		public ReplaceRoad(Entity building)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			m_Building = building;
			m_NewRoad = Entity.Null;
			m_FrontPos = default(float3);
			m_CurvePos = 0f;
			m_Deleted = false;
			m_Required = false;
		}

		public int CompareTo(ReplaceRoad other)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return m_Building.Index - other.m_Building.Index;
		}
	}

	[BurstCompile]
	private struct FindRoadConnectionJob : IJobParallelForDefer
	{
		public struct FindRoadConnectionIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds3 m_Bounds;

			public float m_MinDistance;

			public float m_BestCurvePos;

			public Entity m_BestRoad;

			public float3 m_FrontPosition;

			public bool m_CanBeOnRoad;

			public BufferLookup<ConnectedBuilding> m_ConnectedBuildings;

			public ComponentLookup<Curve> m_CurveData;

			public ComponentLookup<Composition> m_CompositionData;

			public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

			public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

			public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

			public ComponentLookup<NetCompositionData> m_PrefabNetCompositionData;

			public ComponentLookup<Deleted> m_DeletedData;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_Bounds)).xz);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity edgeEntity)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_Bounds)).xz) && !m_DeletedData.HasComponent(edgeEntity))
				{
					CheckEdge(edgeEntity);
				}
			}

			public void CheckEdge(Entity edgeEntity)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0109: Unknown result type (might be due to invalid IL or missing references)
				//IL_0110: Unknown result type (might be due to invalid IL or missing references)
				//IL_0115: Unknown result type (might be due to invalid IL or missing references)
				//IL_014a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0151: Unknown result type (might be due to invalid IL or missing references)
				//IL_0157: Unknown result type (might be due to invalid IL or missing references)
				//IL_015e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0163: Unknown result type (might be due to invalid IL or missing references)
				//IL_0168: Unknown result type (might be due to invalid IL or missing references)
				//IL_017e: Unknown result type (might be due to invalid IL or missing references)
				//IL_017f: Unknown result type (might be due to invalid IL or missing references)
				if (!m_ConnectedBuildings.HasBuffer(edgeEntity))
				{
					return;
				}
				NetCompositionData netCompositionData = default(NetCompositionData);
				Composition composition = default(Composition);
				if (m_CompositionData.TryGetComponent(edgeEntity, ref composition) && m_PrefabNetCompositionData.TryGetComponent(composition.m_Edge, ref netCompositionData) && (netCompositionData.m_Flags.m_General & (CompositionFlags.General.Elevated | CompositionFlags.General.Tunnel)) != 0)
				{
					return;
				}
				EdgeGeometry edgeGeometry = m_EdgeGeometryData[edgeEntity];
				EdgeNodeGeometry geometry = m_StartNodeGeometryData[edgeEntity].m_Geometry;
				EdgeNodeGeometry geometry2 = m_EndNodeGeometryData[edgeEntity].m_Geometry;
				float maxDistance = m_MinDistance;
				CheckDistance(edgeGeometry, geometry, geometry2, m_FrontPosition, m_CanBeOnRoad, ref maxDistance);
				if (maxDistance < m_MinDistance)
				{
					Curve curve = m_CurveData[edgeEntity];
					float num = default(float);
					MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref m_FrontPosition)).xz, ref num);
					float3 val = MathUtils.Position(curve.m_Bezier, num);
					float3 val2 = MathUtils.Tangent(curve.m_Bezier, num);
					if ((((math.dot(MathUtils.Right(((float3)(ref val2)).xz), ((float3)(ref m_FrontPosition)).xz - ((float3)(ref val)).xz) >= 0f) ? netCompositionData.m_Flags.m_Right : netCompositionData.m_Flags.m_Left) & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) == 0)
					{
						m_Bounds = new Bounds3(m_FrontPosition - maxDistance, m_FrontPosition + maxDistance);
						m_MinDistance = maxDistance;
						m_BestCurvePos = num;
						m_BestRoad = edgeEntity;
					}
				}
			}
		}

		[ReadOnly]
		public BufferLookup<ConnectedBuilding> m_ConnectedBuildings;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> m_SubNets;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabNetCompositionData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<BackSide> m_BackSideData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_UpdatedNetChunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		public NativeArray<ReplaceRoad> m_ReplaceRoadConnection;

		public void Execute(int index)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			ReplaceRoad replaceRoad = m_ReplaceRoadConnection[index];
			if (m_DeletedData.HasComponent(replaceRoad.m_Building))
			{
				replaceRoad.m_Deleted = true;
				m_ReplaceRoadConnection[index] = replaceRoad;
				return;
			}
			PrefabRef prefabRef = m_PrefabRefData[replaceRoad.m_Building];
			BuildingData buildingData = m_PrefabBuildingData[prefabRef.m_Prefab];
			BackSide backSide = default(BackSide);
			if ((buildingData.m_Flags & Game.Prefabs.BuildingFlags.NoRoadConnection) == 0)
			{
				Transform transform = m_TransformData[replaceRoad.m_Building];
				float3 val = BuildingUtils.CalculateFrontPosition(transform, buildingData.m_LotSize.y);
				replaceRoad.m_Required = (buildingData.m_Flags & Game.Prefabs.BuildingFlags.RequireRoad) != 0;
				bool canBeOnRoad = (buildingData.m_Flags & Game.Prefabs.BuildingFlags.CanBeOnRoad) != 0;
				FindRoadConnectionIterator findRoadConnectionIterator = new FindRoadConnectionIterator
				{
					m_Bounds = default(Bounds3),
					m_MinDistance = float.MaxValue,
					m_BestCurvePos = 0f,
					m_BestRoad = Entity.Null,
					m_FrontPosition = transform.m_Position,
					m_CanBeOnRoad = canBeOnRoad,
					m_ConnectedBuildings = m_ConnectedBuildings,
					m_CurveData = m_CurveData,
					m_CompositionData = m_CompositionData,
					m_EdgeGeometryData = m_EdgeGeometryData,
					m_StartNodeGeometryData = m_StartNodeGeometryData,
					m_EndNodeGeometryData = m_EndNodeGeometryData,
					m_PrefabNetCompositionData = m_PrefabNetCompositionData,
					m_DeletedData = m_DeletedData
				};
				if (m_SubNets.HasBuffer(replaceRoad.m_Building))
				{
					DynamicBuffer<Game.Net.SubNet> val2 = m_SubNets[replaceRoad.m_Building];
					for (int i = 0; i < val2.Length; i++)
					{
						Entity subNet = val2[i].m_SubNet;
						if (!m_DeletedData.HasComponent(subNet))
						{
							findRoadConnectionIterator.CheckEdge(subNet);
						}
					}
				}
				if (findRoadConnectionIterator.m_BestRoad == Entity.Null && m_TempData.HasComponent(replaceRoad.m_Building))
				{
					Temp temp = m_TempData[replaceRoad.m_Building];
					if (m_SubNets.HasBuffer(temp.m_Original))
					{
						DynamicBuffer<Game.Net.SubNet> val3 = m_SubNets[temp.m_Original];
						for (int j = 0; j < val3.Length; j++)
						{
							Entity subNet2 = val3[j].m_SubNet;
							if (!m_DeletedData.HasComponent(subNet2))
							{
								findRoadConnectionIterator.CheckEdge(subNet2);
							}
						}
					}
				}
				bool flag = false;
				ArchetypeChunk val4;
				if (findRoadConnectionIterator.m_BestRoad == Entity.Null)
				{
					float num = 8.4f;
					findRoadConnectionIterator.m_Bounds = new Bounds3(val - num, val + num);
					findRoadConnectionIterator.m_MinDistance = num;
					findRoadConnectionIterator.m_FrontPosition = val;
					m_NetSearchTree.Iterate<FindRoadConnectionIterator>(ref findRoadConnectionIterator, 0);
					for (int k = 0; k < m_UpdatedNetChunks.Length; k++)
					{
						val4 = m_UpdatedNetChunks[k];
						NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val4)).GetNativeArray(m_EntityType);
						for (int l = 0; l < nativeArray.Length; l++)
						{
							findRoadConnectionIterator.CheckEdge(nativeArray[l]);
						}
					}
					flag = (buildingData.m_Flags & Game.Prefabs.BuildingFlags.BackAccess) != 0 && m_TempData.HasComponent(replaceRoad.m_Building);
				}
				replaceRoad.m_NewRoad = findRoadConnectionIterator.m_BestRoad;
				replaceRoad.m_FrontPos = findRoadConnectionIterator.m_FrontPosition;
				replaceRoad.m_CurvePos = findRoadConnectionIterator.m_BestCurvePos;
				if (flag)
				{
					val = BuildingUtils.CalculateFrontPosition(transform, -buildingData.m_LotSize.y);
					float num2 = 8.4f;
					findRoadConnectionIterator.m_BestRoad = Entity.Null;
					findRoadConnectionIterator.m_BestCurvePos = 0f;
					findRoadConnectionIterator.m_Bounds = new Bounds3(val - num2, val + num2);
					findRoadConnectionIterator.m_MinDistance = num2;
					findRoadConnectionIterator.m_FrontPosition = val;
					m_NetSearchTree.Iterate<FindRoadConnectionIterator>(ref findRoadConnectionIterator, 0);
					for (int m = 0; m < m_UpdatedNetChunks.Length; m++)
					{
						val4 = m_UpdatedNetChunks[m];
						NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref val4)).GetNativeArray(m_EntityType);
						for (int n = 0; n < nativeArray2.Length; n++)
						{
							findRoadConnectionIterator.CheckEdge(nativeArray2[n]);
						}
					}
					backSide.m_RoadEdge = findRoadConnectionIterator.m_BestRoad;
					backSide.m_CurvePosition = findRoadConnectionIterator.m_BestCurvePos;
				}
			}
			if (m_BackSideData.HasComponent(replaceRoad.m_Building))
			{
				m_BackSideData[replaceRoad.m_Building] = backSide;
			}
			m_ReplaceRoadConnection[index] = replaceRoad;
		}
	}

	[BurstCompile]
	private struct ReplaceRoadConnectionJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<Created> m_CreatedData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		public ComponentLookup<Building> m_BuildingData;

		public BufferLookup<ConnectedBuilding> m_ConnectedBuildings;

		[ReadOnly]
		public EntityArchetype m_RoadConnectionEventArchetype;

		[ReadOnly]
		public NativeList<ReplaceRoad> m_ReplaceRoadConnection;

		[ReadOnly]
		public TrafficConfigurationData m_TrafficConfigurationData;

		public EntityCommandBuffer m_CommandBuffer;

		public IconCommandBuffer m_IconCommandBuffer;

		public SourceUpdateData m_SourceUpdateData;

		public void Execute()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_ReplaceRoadConnection.Length; i++)
			{
				ReplaceRoad replaceRoad = m_ReplaceRoadConnection[i];
				Building building = m_BuildingData[replaceRoad.m_Building];
				bool flag = m_CreatedData.HasComponent(replaceRoad.m_Building);
				if (replaceRoad.m_NewRoad != building.m_RoadEdge || flag)
				{
					if (m_TempData.HasComponent(replaceRoad.m_Building))
					{
						if (building.m_RoadEdge == Entity.Null && replaceRoad.m_NewRoad != Entity.Null && m_TempData[replaceRoad.m_Building].m_Original == Entity.Null)
						{
							m_SourceUpdateData.AddSnap();
						}
						building.m_RoadEdge = replaceRoad.m_NewRoad;
						building.m_CurvePosition = replaceRoad.m_CurvePos;
						m_BuildingData[replaceRoad.m_Building] = building;
						continue;
					}
					if (building.m_RoadEdge != Entity.Null != (replaceRoad.m_NewRoad != Entity.Null))
					{
						if (replaceRoad.m_NewRoad != Entity.Null)
						{
							m_IconCommandBuffer.Remove(replaceRoad.m_Building, m_TrafficConfigurationData.m_RoadConnectionNotification);
						}
						else if (!replaceRoad.m_Deleted && replaceRoad.m_Required)
						{
							m_IconCommandBuffer.Add(replaceRoad.m_Building, m_TrafficConfigurationData.m_RoadConnectionNotification, replaceRoad.m_FrontPos, IconPriority.Warning);
						}
					}
					RoadConnectionUpdated roadConnectionUpdated = new RoadConnectionUpdated
					{
						m_Building = replaceRoad.m_Building,
						m_Old = (flag ? Entity.Null : building.m_RoadEdge),
						m_New = replaceRoad.m_NewRoad
					};
					Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_RoadConnectionEventArchetype);
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<RoadConnectionUpdated>(val, roadConnectionUpdated);
					if (building.m_RoadEdge != Entity.Null)
					{
						CollectionUtils.RemoveValue<ConnectedBuilding>(m_ConnectedBuildings[building.m_RoadEdge], new ConnectedBuilding(replaceRoad.m_Building));
					}
					building.m_RoadEdge = replaceRoad.m_NewRoad;
					building.m_CurvePosition = replaceRoad.m_CurvePos;
					m_BuildingData[replaceRoad.m_Building] = building;
					if (replaceRoad.m_NewRoad != Entity.Null)
					{
						m_ConnectedBuildings[replaceRoad.m_NewRoad].Add(new ConnectedBuilding(replaceRoad.m_Building));
					}
				}
				else if (replaceRoad.m_CurvePos != building.m_CurvePosition)
				{
					building.m_CurvePosition = replaceRoad.m_CurvePos;
					m_BuildingData[replaceRoad.m_Building] = building;
				}
			}
		}
	}

	private struct ConnectionLaneKey : IEquatable<ConnectionLaneKey>
	{
		private PathNode m_Node1;

		private PathNode m_Node2;

		public ConnectionLaneKey(PathNode node1, PathNode node2)
		{
			if (node1.GetOrder(node2))
			{
				m_Node1 = node2;
				m_Node2 = node1;
			}
			else
			{
				m_Node1 = node1;
				m_Node2 = node2;
			}
		}

		public bool Equals(ConnectionLaneKey other)
		{
			if (m_Node1.Equals(other.m_Node1))
			{
				return m_Node2.Equals(other.m_Node2);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (17 * 31 + m_Node1.GetHashCode()) * 31 + m_Node2.GetHashCode();
		}
	}

	private struct SpawnLocationData
	{
		public Entity m_Entity;

		public Entity m_Original;

		public float3 m_Position;

		public Game.Prefabs.SpawnLocationData m_PrefabData;

		public int m_Group;
	}

	[BurstCompile]
	private struct UpdateSecondaryLanesJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<WaterConsumer> m_WaterConsumerData;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> m_ElectricityConsumerData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.UtilityObject> m_UtilityObjectData;

		[ReadOnly]
		public ComponentLookup<Secondary> m_SecondaryData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Game.Net.UtilityLane> m_UtilityLaneData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<EdgeLane> m_EdgeLaneData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<Game.Net.SecondaryLane> m_SecondaryLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> m_PrefabUtilityLaneData;

		[ReadOnly]
		public ComponentLookup<UtilityObjectData> m_PrefabUtilityObjectData;

		[ReadOnly]
		public ComponentLookup<NetLaneArchetypeData> m_PrefabNetLaneArchetypeData;

		[ReadOnly]
		public ComponentLookup<ObjectData> m_PrefabObjectData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<Game.Prefabs.SpawnLocationData> m_PrefabSpawnLocationData;

		[ReadOnly]
		public BufferLookup<SpawnLocationElement> m_SpawnLocations;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public NativeList<Entity> m_ConnectionPrefabs;

		[ReadOnly]
		public NativeList<ReplaceRoad> m_ReplaceRoadConnection;

		[ReadOnly]
		public ComponentTypeSet m_AppliedTypes;

		[ReadOnly]
		public BuildingConfigurationData m_BuildingConfigurationData;

		public ParallelWriter m_CommandBuffer;

		public void Execute(int index)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			ReplaceRoad replaceRoad = m_ReplaceRoadConnection[index];
			FindRoadUtilityLanes(replaceRoad.m_Building, replaceRoad.m_NewRoad, replaceRoad.m_FrontPos, out var electricityCurve, out var electricityLanePrefab, out var electricityObjectPrefab, out var electricityNode, out var sewageCurve, out var sewageLanePrefab, out var sewageObjectPrefab, out var sewageNode, out var waterCurve, out var waterLanePrefab, out var waterObjectPrefab, out var waterNode);
			Temp temp = default(Temp);
			Temp subTemp = default(Temp);
			bool isTemp = false;
			if (m_TempData.HasComponent(replaceRoad.m_Building))
			{
				temp = m_TempData[replaceRoad.m_Building];
				subTemp.m_Flags = temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden | TempFlags.Duplicate);
				if ((temp.m_Flags & (TempFlags.Replace | TempFlags.Upgrade)) != 0)
				{
					subTemp.m_Flags |= TempFlags.Modify;
				}
				isTemp = true;
			}
			FindOriginalLanes(temp.m_Original, out var originalConnections, out var originalElectricityLane, out var originalSewageLane, out var originalWaterLane);
			FindOriginalObjects(temp.m_Original, out var originalElectricityObject, out var originalSewageObject, out var originalWaterObject);
			float3 electricityObjectPosition = CalculateObjectPosition(electricityCurve, electricityObjectPrefab, start: true);
			float3 sewageObjectPosition = CalculateObjectPosition(sewageCurve, sewageObjectPrefab, start: true);
			float3 waterObjectPosition = CalculateObjectPosition(waterCurve, waterObjectPrefab, start: true);
			UpdateLanes(index, replaceRoad.m_Building, replaceRoad.m_FrontPos, replaceRoad.m_Deleted, isTemp, subTemp, originalConnections, electricityCurve, electricityLanePrefab, electricityNode, originalElectricityLane, sewageCurve, sewageLanePrefab, sewageNode, originalSewageLane, waterCurve, waterLanePrefab, waterNode, originalWaterLane);
			UpdateObjects(index, replaceRoad.m_Building, replaceRoad.m_FrontPos, isTemp, subTemp, electricityObjectPrefab, originalElectricityObject, electricityObjectPosition, sewageObjectPrefab, originalSewageObject, sewageObjectPosition, waterObjectPrefab, originalWaterObject, waterObjectPosition);
			if (originalConnections.IsCreated)
			{
				originalConnections.Dispose();
			}
		}

		private void UpdateObjects(int jobIndex, Entity building, float3 connectPos, bool isTemp, Temp subTemp, Entity electricityObjectPrefab, Entity originalElectricityObject, float3 electricityObjectPosition, Entity sewageObjectPrefab, Entity originalSewageObject, float3 sewageObjectPosition, Entity waterObjectPrefab, Entity originalWaterObject, float3 waterObjectPosition)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			if (!m_SubObjects.HasBuffer(building))
			{
				return;
			}
			DynamicBuffer<Game.Objects.SubObject> val = m_SubObjects[building];
			for (int i = 0; i < val.Length; i++)
			{
				Entity subObject = val[i].m_SubObject;
				if (!m_UtilityObjectData.HasComponent(subObject) || !m_SecondaryData.HasComponent(subObject))
				{
					continue;
				}
				Transform transform = m_TransformData[subObject];
				PrefabRef prefabRef = m_PrefabRefData[subObject];
				UtilityObjectData utilityObjectData = m_PrefabUtilityObjectData[prefabRef.m_Prefab];
				if ((utilityObjectData.m_UtilityTypes & UtilityTypes.LowVoltageLine) != UtilityTypes.None)
				{
					if (prefabRef.m_Prefab != electricityObjectPrefab)
					{
						DeleteObject(jobIndex, subObject);
						continue;
					}
					electricityObjectPrefab = Entity.Null;
					if (m_DeletedData.HasComponent(subObject))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, subObject);
					}
					if (!((float3)(ref transform.m_Position)).Equals(electricityObjectPosition))
					{
						UpdateObject(jobIndex, subObject, electricityObjectPosition, isTemp, subTemp, originalElectricityObject);
					}
				}
				else if ((utilityObjectData.m_UtilityTypes & UtilityTypes.SewagePipe) != UtilityTypes.None)
				{
					if (prefabRef.m_Prefab != sewageObjectPrefab)
					{
						DeleteObject(jobIndex, subObject);
						continue;
					}
					sewageObjectPrefab = Entity.Null;
					if (m_DeletedData.HasComponent(subObject))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, subObject);
					}
					if (!((float3)(ref transform.m_Position)).Equals(sewageObjectPosition))
					{
						UpdateObject(jobIndex, subObject, sewageObjectPosition, isTemp, subTemp, originalSewageObject);
					}
				}
				else
				{
					if ((utilityObjectData.m_UtilityTypes & UtilityTypes.WaterPipe) == 0)
					{
						continue;
					}
					if (prefabRef.m_Prefab != waterObjectPrefab)
					{
						DeleteObject(jobIndex, subObject);
						continue;
					}
					waterObjectPrefab = Entity.Null;
					if (m_DeletedData.HasComponent(subObject))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, subObject);
					}
					if (!((float3)(ref transform.m_Position)).Equals(waterObjectPosition))
					{
						UpdateObject(jobIndex, subObject, waterObjectPosition, isTemp, subTemp, originalWaterObject);
					}
				}
			}
			if (electricityObjectPrefab != Entity.Null)
			{
				CreateObject(jobIndex, building, electricityObjectPrefab, electricityObjectPosition, connectPos, isTemp, subTemp, originalElectricityObject);
			}
			if (sewageObjectPrefab != Entity.Null)
			{
				CreateObject(jobIndex, building, sewageObjectPrefab, sewageObjectPosition, connectPos, isTemp, subTemp, originalSewageObject);
			}
			if (waterObjectPrefab != Entity.Null)
			{
				CreateObject(jobIndex, building, waterObjectPrefab, waterObjectPosition, connectPos, isTemp, subTemp, originalWaterObject);
			}
		}

		private void UpdateLanes(int jobIndex, Entity building, float3 connectPos, bool isDeleted, bool isTemp, Temp subTemp, NativeParallelHashMap<ConnectionLaneKey, Entity> originalConnections, Bezier4x3 electricityCurve, Entity electricityLanePrefab, PathNode electricityNode, Entity originalElectricityLane, Bezier4x3 sewageCurve, Entity sewageLanePrefab, PathNode sewageNode, Entity originalSewageLane, Bezier4x3 waterCurve, Entity waterLanePrefab, PathNode waterNode, Entity originalWaterLane)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0786: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0792: Unknown result type (might be due to invalid IL or missing references)
			//IL_079b: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0625: Unknown result type (might be due to invalid IL or missing references)
			//IL_062c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0631: Unknown result type (might be due to invalid IL or missing references)
			//IL_0636: Unknown result type (might be due to invalid IL or missing references)
			//IL_0638: Unknown result type (might be due to invalid IL or missing references)
			//IL_0641: Unknown result type (might be due to invalid IL or missing references)
			//IL_0643: Unknown result type (might be due to invalid IL or missing references)
			//IL_0648: Unknown result type (might be due to invalid IL or missing references)
			//IL_064c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0653: Unknown result type (might be due to invalid IL or missing references)
			//IL_0658: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_0807: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0800: Unknown result type (might be due to invalid IL or missing references)
			//IL_074f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0756: Unknown result type (might be due to invalid IL or missing references)
			//IL_075b: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0832: Unknown result type (might be due to invalid IL or missing references)
			//IL_0813: Unknown result type (might be due to invalid IL or missing references)
			//IL_081c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0828: Unknown result type (might be due to invalid IL or missing references)
			//IL_082a: Unknown result type (might be due to invalid IL or missing references)
			//IL_082b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_085d: Unknown result type (might be due to invalid IL or missing references)
			//IL_083e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0847: Unknown result type (might be due to invalid IL or missing references)
			//IL_0853: Unknown result type (might be due to invalid IL or missing references)
			//IL_0855: Unknown result type (might be due to invalid IL or missing references)
			//IL_0856: Unknown result type (might be due to invalid IL or missing references)
			//IL_0869: Unknown result type (might be due to invalid IL or missing references)
			//IL_0872: Unknown result type (might be due to invalid IL or missing references)
			//IL_087e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0880: Unknown result type (might be due to invalid IL or missing references)
			//IL_0881: Unknown result type (might be due to invalid IL or missing references)
			//IL_088f: Unknown result type (might be due to invalid IL or missing references)
			//IL_089f: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelHashMap<ConnectionLaneKey, Entity> oldConnections = default(NativeParallelHashMap<ConnectionLaneKey, Entity>);
			DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[building];
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				if (!m_SecondaryLaneData.HasComponent(subLane))
				{
					continue;
				}
				if (m_UtilityLaneData.HasComponent(subLane))
				{
					Curve curve = m_CurveData[subLane];
					Lane lane = m_LaneData[subLane];
					PrefabRef prefabRef = m_PrefabRefData[subLane];
					UtilityLaneData utilityLaneData = m_PrefabUtilityLaneData[prefabRef.m_Prefab];
					if ((utilityLaneData.m_UtilityTypes & UtilityTypes.LowVoltageLine) != UtilityTypes.None)
					{
						if (prefabRef.m_Prefab != electricityLanePrefab)
						{
							DeleteLane(jobIndex, subLane);
							continue;
						}
						electricityLanePrefab = Entity.Null;
						if (m_DeletedData.HasComponent(subLane))
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, subLane);
						}
						if (!((Bezier4x3)(ref curve.m_Bezier)).Equals(electricityCurve) || !lane.m_EndNode.Equals(electricityNode))
						{
							UpdateLane(jobIndex, subLane, electricityCurve, electricityNode, isTemp, subTemp, originalElectricityLane);
						}
					}
					else if ((utilityLaneData.m_UtilityTypes & UtilityTypes.SewagePipe) != UtilityTypes.None)
					{
						if (prefabRef.m_Prefab != sewageLanePrefab)
						{
							DeleteLane(jobIndex, subLane);
							continue;
						}
						sewageLanePrefab = Entity.Null;
						if (m_DeletedData.HasComponent(subLane))
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, subLane);
						}
						if (!((Bezier4x3)(ref curve.m_Bezier)).Equals(sewageCurve) || !lane.m_EndNode.Equals(sewageNode))
						{
							UpdateLane(jobIndex, subLane, sewageCurve, sewageNode, isTemp, subTemp, originalSewageLane);
						}
					}
					else
					{
						if ((utilityLaneData.m_UtilityTypes & UtilityTypes.WaterPipe) == 0)
						{
							continue;
						}
						if (prefabRef.m_Prefab != waterLanePrefab)
						{
							DeleteLane(jobIndex, subLane);
							continue;
						}
						waterLanePrefab = Entity.Null;
						if (m_DeletedData.HasComponent(subLane))
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, subLane);
						}
						if (!((Bezier4x3)(ref curve.m_Bezier)).Equals(waterCurve) || !lane.m_EndNode.Equals(waterNode))
						{
							UpdateLane(jobIndex, subLane, waterCurve, waterNode, isTemp, subTemp, originalWaterLane);
						}
					}
				}
				else if (m_ConnectionLaneData.HasComponent(subLane))
				{
					Lane lane2 = m_LaneData[subLane];
					if (!oldConnections.IsCreated)
					{
						oldConnections._002Ector(val.Length, AllocatorHandle.op_Implicit((Allocator)2));
					}
					oldConnections.TryAdd(new ConnectionLaneKey(lane2.m_StartNode, lane2.m_EndNode), subLane);
				}
				else
				{
					DeleteLane(jobIndex, subLane);
				}
			}
			if (electricityLanePrefab != Entity.Null)
			{
				CreateLane(jobIndex, building, 65530, electricityLanePrefab, electricityCurve, electricityNode, connectPos, isTemp, subTemp, originalElectricityLane);
			}
			if (sewageLanePrefab != Entity.Null)
			{
				CreateLane(jobIndex, building, 65532, sewageLanePrefab, sewageCurve, sewageNode, connectPos, isTemp, subTemp, originalSewageLane);
			}
			if (waterLanePrefab != Entity.Null)
			{
				CreateLane(jobIndex, building, 65534, waterLanePrefab, waterCurve, waterNode, connectPos, isTemp, subTemp, originalWaterLane);
			}
			if (!isDeleted)
			{
				DynamicBuffer<SpawnLocationElement> val2 = m_SpawnLocations[building];
				bool flag = val2.Length >= 2;
				if (flag)
				{
					Entity val3 = building;
					while (m_OwnerData.HasComponent(val3))
					{
						val3 = m_OwnerData[val3].m_Owner;
						if (m_SpawnLocations.HasBuffer(val3))
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					NativeParallelHashMap<ConnectionLaneKey, Entity> newConnections = default(NativeParallelHashMap<ConnectionLaneKey, Entity>);
					newConnections._002Ector(val2.Length * 4, AllocatorHandle.op_Implicit((Allocator)2));
					NativeArray<SpawnLocationData> val4 = default(NativeArray<SpawnLocationData>);
					val4._002Ector(val2.Length, (Allocator)2, (NativeArrayOptions)1);
					Temp temp = default(Temp);
					Game.Objects.SpawnLocation spawnLocation2 = default(Game.Objects.SpawnLocation);
					for (int j = 0; j < val2.Length; j++)
					{
						if (val2[j].m_Type != SpawnLocationType.SpawnLocation)
						{
							continue;
						}
						Entity spawnLocation = val2[j].m_SpawnLocation;
						if (m_TransformData.HasComponent(spawnLocation))
						{
							PrefabRef prefabRef2 = m_PrefabRefData[spawnLocation];
							Game.Prefabs.SpawnLocationData prefabData = m_PrefabSpawnLocationData[prefabRef2.m_Prefab];
							if (prefabData.m_ActivityMask.m_Mask == 0 && prefabData.m_ConnectionType != RouteConnectionType.Air && prefabData.m_ConnectionType != RouteConnectionType.Track)
							{
								m_TempData.TryGetComponent(spawnLocation, ref temp);
								m_SpawnLocationData.TryGetComponent(spawnLocation, ref spawnLocation2);
								val4[j] = new SpawnLocationData
								{
									m_Entity = spawnLocation,
									m_Original = temp.m_Original,
									m_Position = m_TransformData[spawnLocation].m_Position,
									m_PrefabData = prefabData,
									m_Group = spawnLocation2.m_GroupIndex
								};
							}
						}
					}
					for (int k = 0; k < val4.Length; k++)
					{
						SpawnLocationData spawnLocationData = val4[k];
						if (spawnLocationData.m_PrefabData.m_ConnectionType == RouteConnectionType.None)
						{
							continue;
						}
						float3 val5 = float3.op_Implicit(float.MaxValue);
						float3 val6 = float3.op_Implicit(float.MaxValue);
						int3 val7 = int3.op_Implicit(-1);
						int3 val8 = int3.op_Implicit(-1);
						for (int l = 0; l < val4.Length; l++)
						{
							if (l == k)
							{
								continue;
							}
							SpawnLocationData spawnLocationData2 = val4[l];
							if (spawnLocationData2.m_Group != spawnLocationData.m_Group)
							{
								continue;
							}
							switch (spawnLocationData.m_PrefabData.m_ConnectionType)
							{
							case RouteConnectionType.Pedestrian:
								if (spawnLocationData2.m_PrefabData.m_ConnectionType != RouteConnectionType.Pedestrian)
								{
									continue;
								}
								break;
							case RouteConnectionType.Cargo:
								if (spawnLocationData2.m_PrefabData.m_ConnectionType != RouteConnectionType.Cargo)
								{
									continue;
								}
								break;
							case RouteConnectionType.Road:
							case RouteConnectionType.Air:
								if ((spawnLocationData2.m_PrefabData.m_ConnectionType != RouteConnectionType.Road && spawnLocationData2.m_PrefabData.m_ConnectionType != RouteConnectionType.Air) || (spawnLocationData.m_PrefabData.m_RoadTypes & spawnLocationData2.m_PrefabData.m_RoadTypes) == 0)
								{
									continue;
								}
								break;
							case RouteConnectionType.Track:
								if (spawnLocationData2.m_PrefabData.m_ConnectionType != RouteConnectionType.Track || (spawnLocationData.m_PrefabData.m_TrackTypes & spawnLocationData2.m_PrefabData.m_TrackTypes) == 0)
								{
									continue;
								}
								break;
							case RouteConnectionType.Parking:
								if (spawnLocationData2.m_PrefabData.m_ConnectionType != RouteConnectionType.Parking || (spawnLocationData.m_PrefabData.m_RoadTypes & spawnLocationData2.m_PrefabData.m_RoadTypes) == 0)
								{
									continue;
								}
								break;
							}
							float3 val9 = spawnLocationData2.m_Position - spawnLocationData.m_Position;
							float distance = math.length(val9);
							float3 val10 = math.abs(val9);
							bool3 val11 = ((float3)(ref val10)).xxy >= ((float3)(ref val10)).yzz;
							if (math.all(((bool3)(ref val11)).xy))
							{
								CheckDistance(val9.x, distance, l, ref val5.x, ref val6.x, ref val7.x, ref val8.x);
							}
							else if (val11.z)
							{
								CheckDistance(val9.y, distance, l, ref val5.y, ref val6.y, ref val7.y, ref val8.y);
							}
							else
							{
								CheckDistance(val9.z, distance, l, ref val5.z, ref val6.z, ref val7.z, ref val8.z);
							}
						}
						float num = float.MaxValue;
						int num2 = -1;
						if (spawnLocationData.m_PrefabData.m_ConnectionType == RouteConnectionType.Parking)
						{
							for (int m = 0; m < val4.Length; m++)
							{
								SpawnLocationData spawnLocationData3 = val4[m];
								if (spawnLocationData3.m_PrefabData.m_ConnectionType == RouteConnectionType.Pedestrian)
								{
									float num3 = math.length(spawnLocationData3.m_Position - spawnLocationData.m_Position);
									if (num3 < num)
									{
										num = num3;
										num2 = m;
									}
								}
							}
						}
						if (val7.x != -1)
						{
							CheckConnection(jobIndex, building, isTemp, subTemp, spawnLocationData, val4[val7.x], originalConnections, oldConnections, newConnections);
						}
						if (val7.y != -1)
						{
							CheckConnection(jobIndex, building, isTemp, subTemp, spawnLocationData, val4[val7.y], originalConnections, oldConnections, newConnections);
						}
						if (val7.z != -1)
						{
							CheckConnection(jobIndex, building, isTemp, subTemp, spawnLocationData, val4[val7.z], originalConnections, oldConnections, newConnections);
						}
						if (val8.x != -1)
						{
							CheckConnection(jobIndex, building, isTemp, subTemp, spawnLocationData, val4[val8.x], originalConnections, oldConnections, newConnections);
						}
						if (val8.y != -1)
						{
							CheckConnection(jobIndex, building, isTemp, subTemp, spawnLocationData, val4[val8.y], originalConnections, oldConnections, newConnections);
						}
						if (val8.z != -1)
						{
							CheckConnection(jobIndex, building, isTemp, subTemp, spawnLocationData, val4[val8.z], originalConnections, oldConnections, newConnections);
						}
						if (num2 != -1)
						{
							CheckConnection(jobIndex, building, isTemp, subTemp, spawnLocationData, val4[num2], originalConnections, oldConnections, newConnections);
						}
					}
					val4.Dispose();
					newConnections.Dispose();
				}
			}
			if (oldConnections.IsCreated)
			{
				Enumerator<ConnectionLaneKey, Entity> enumerator = oldConnections.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DeleteLane(jobIndex, enumerator.Current.Value);
				}
				enumerator.Dispose();
				oldConnections.Dispose();
			}
		}

		private void CheckConnection(int jobIndex, Entity building, bool isTemp, Temp temp, SpawnLocationData spawnLocationData1, SpawnLocationData spawnLocationData2, NativeParallelHashMap<ConnectionLaneKey, Entity> originalConnections, NativeParallelHashMap<ConnectionLaneKey, Entity> oldConnections, NativeParallelHashMap<ConnectionLaneKey, Entity> newConnections)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			PathNode pathNode = new PathNode(spawnLocationData1.m_Entity, 0);
			PathNode pathNode2 = new PathNode(spawnLocationData2.m_Entity, 0);
			ConnectionLaneKey connectionLaneKey = new ConnectionLaneKey(pathNode, pathNode2);
			if (newConnections.ContainsKey(connectionLaneKey))
			{
				return;
			}
			Lane lane = default(Lane);
			lane.m_StartNode = pathNode;
			lane.m_MiddleNode = new PathNode(spawnLocationData1.m_Entity, 3);
			lane.m_EndNode = pathNode2;
			Curve curve = default(Curve);
			curve.m_Bezier = NetUtils.StraightCurve(spawnLocationData1.m_Position, spawnLocationData2.m_Position);
			curve.m_Length = math.distance(spawnLocationData1.m_Position, spawnLocationData2.m_Position);
			if (isTemp && originalConnections.IsCreated)
			{
				PathNode node = new PathNode(spawnLocationData1.m_Original, 0);
				PathNode node2 = new PathNode(spawnLocationData2.m_Original, 0);
				ConnectionLaneKey connectionLaneKey2 = new ConnectionLaneKey(node, node2);
				Entity original = default(Entity);
				if (originalConnections.TryGetValue(connectionLaneKey2, ref original))
				{
					originalConnections.Remove(connectionLaneKey2);
					temp.m_Original = original;
				}
			}
			Entity val = default(Entity);
			if (oldConnections.IsCreated && oldConnections.TryGetValue(connectionLaneKey, ref val))
			{
				oldConnections.Remove(connectionLaneKey);
				if (m_DeletedData.HasComponent(val))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, val);
				}
				Curve curve2 = m_CurveData[val];
				if (!((Bezier4x3)(ref curve.m_Bezier)).Equals(curve2.m_Bezier))
				{
					Bezier4x3 val2 = MathUtils.Invert(curve.m_Bezier);
					if (!((Bezier4x3)(ref val2)).Equals(curve2.m_Bezier))
					{
						if (m_LaneData[val].m_StartNode.Equals(pathNode2))
						{
							CommonUtils.Swap(ref lane.m_StartNode, ref lane.m_EndNode);
							curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
						}
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val, lane);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val, curve);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, val, default(Updated));
					}
				}
				if (isTemp)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, val, temp);
				}
				newConnections.Add(connectionLaneKey, val);
				return;
			}
			Entity val3 = m_ConnectionPrefabs[0];
			NetLaneArchetypeData netLaneArchetypeData = m_PrefabNetLaneArchetypeData[val3];
			Owner owner = new Owner(building);
			PrefabRef prefabRef = new PrefabRef(val3);
			Game.Net.SecondaryLane secondaryLane = default(Game.Net.SecondaryLane);
			Game.Net.ConnectionLane connectionLane = new Game.Net.ConnectionLane
			{
				m_Flags = ConnectionLaneFlags.Inside
			};
			switch (spawnLocationData1.m_PrefabData.m_ConnectionType)
			{
			case RouteConnectionType.Pedestrian:
				connectionLane.m_Flags |= ConnectionLaneFlags.Pedestrian;
				break;
			case RouteConnectionType.Cargo:
				connectionLane.m_Flags |= ConnectionLaneFlags.AllowCargo;
				break;
			case RouteConnectionType.Road:
			case RouteConnectionType.Air:
				connectionLane.m_Flags |= ConnectionLaneFlags.Road;
				connectionLane.m_RoadTypes = spawnLocationData1.m_PrefabData.m_RoadTypes | spawnLocationData2.m_PrefabData.m_RoadTypes;
				break;
			case RouteConnectionType.Track:
				connectionLane.m_Flags |= ConnectionLaneFlags.Track;
				connectionLane.m_TrackTypes = spawnLocationData1.m_PrefabData.m_TrackTypes | spawnLocationData2.m_PrefabData.m_TrackTypes;
				break;
			case RouteConnectionType.Parking:
				if (spawnLocationData2.m_PrefabData.m_ConnectionType == RouteConnectionType.Pedestrian)
				{
					connectionLane.m_Flags |= ConnectionLaneFlags.Parking;
					connectionLane.m_RoadTypes = spawnLocationData1.m_PrefabData.m_RoadTypes | spawnLocationData2.m_PrefabData.m_RoadTypes;
				}
				else
				{
					connectionLane.m_Flags |= ConnectionLaneFlags.Road;
					connectionLane.m_RoadTypes = spawnLocationData1.m_PrefabData.m_RoadTypes | spawnLocationData2.m_PrefabData.m_RoadTypes;
				}
				break;
			}
			Entity val4 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, netLaneArchetypeData.m_LaneArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val4, prefabRef);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val4, lane);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val4, curve);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Net.ConnectionLane>(jobIndex, val4, connectionLane);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val4, owner);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Game.Net.SecondaryLane>(jobIndex, val4, secondaryLane);
			if ((connectionLane.m_Flags & ConnectionLaneFlags.Parking) != 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<GarageLane>(jobIndex, val4, default(GarageLane));
			}
			if (isTemp)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(jobIndex, val4, temp);
			}
			newConnections.Add(connectionLaneKey, val4);
		}

		private void CheckDistance(float offset, float distance, int index, ref float bestDistance1, ref float bestDistance2, ref int bestIndex1, ref int bestIndex2)
		{
			if (offset >= 0f)
			{
				if (distance < bestDistance1)
				{
					bestDistance1 = distance;
					bestIndex1 = index;
				}
			}
			else if (distance < bestDistance2)
			{
				bestDistance2 = distance;
				bestIndex2 = index;
			}
		}

		private float3 CalculateObjectPosition(Bezier4x3 curve, Entity prefab, bool start = false)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			float3 result = (start ? curve.a : curve.d);
			if (prefab != Entity.Null)
			{
				ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefab];
				result.y -= MathUtils.Center(objectGeometryData.m_Bounds).y;
			}
			return result;
		}

		private void FindOriginalLanes(Entity originalBuilding, out NativeParallelHashMap<ConnectionLaneKey, Entity> originalConnections, out Entity originalElectricityLane, out Entity originalSewageLane, out Entity originalWaterLane)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			originalConnections = default(NativeParallelHashMap<ConnectionLaneKey, Entity>);
			originalElectricityLane = default(Entity);
			originalSewageLane = default(Entity);
			originalWaterLane = default(Entity);
			if (!m_SubLanes.HasBuffer(originalBuilding))
			{
				return;
			}
			DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[originalBuilding];
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				if (!m_SecondaryLaneData.HasComponent(subLane))
				{
					continue;
				}
				if (m_UtilityLaneData.HasComponent(subLane))
				{
					PrefabRef prefabRef = m_PrefabRefData[subLane];
					UtilityLaneData utilityLaneData = m_PrefabUtilityLaneData[prefabRef.m_Prefab];
					if ((utilityLaneData.m_UtilityTypes & UtilityTypes.LowVoltageLine) != UtilityTypes.None)
					{
						originalElectricityLane = subLane;
					}
					else if ((utilityLaneData.m_UtilityTypes & UtilityTypes.SewagePipe) != UtilityTypes.None)
					{
						originalSewageLane = subLane;
					}
					else if ((utilityLaneData.m_UtilityTypes & UtilityTypes.WaterPipe) != UtilityTypes.None)
					{
						originalWaterLane = subLane;
					}
				}
				else if (m_ConnectionLaneData.HasComponent(subLane))
				{
					Lane lane = m_LaneData[subLane];
					if (!originalConnections.IsCreated)
					{
						originalConnections = new NativeParallelHashMap<ConnectionLaneKey, Entity>(val.Length, AllocatorHandle.op_Implicit((Allocator)2));
					}
					originalConnections.TryAdd(new ConnectionLaneKey(lane.m_StartNode, lane.m_EndNode), subLane);
				}
			}
		}

		private void FindOriginalObjects(Entity originalBuilding, out Entity originalElectricityObject, out Entity originalSewageObject, out Entity originalWaterObject)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			originalElectricityObject = default(Entity);
			originalSewageObject = default(Entity);
			originalWaterObject = default(Entity);
			if (!m_SubObjects.HasBuffer(originalBuilding))
			{
				return;
			}
			DynamicBuffer<Game.Objects.SubObject> val = m_SubObjects[originalBuilding];
			for (int i = 0; i < val.Length; i++)
			{
				Entity subObject = val[i].m_SubObject;
				if (m_UtilityObjectData.HasComponent(subObject) && m_SecondaryData.HasComponent(subObject))
				{
					PrefabRef prefabRef = m_PrefabRefData[subObject];
					UtilityObjectData utilityObjectData = m_PrefabUtilityObjectData[prefabRef.m_Prefab];
					if ((utilityObjectData.m_UtilityTypes & UtilityTypes.LowVoltageLine) != UtilityTypes.None)
					{
						originalElectricityObject = subObject;
					}
					else if ((utilityObjectData.m_UtilityTypes & UtilityTypes.SewagePipe) != UtilityTypes.None)
					{
						originalSewageObject = subObject;
					}
					else if ((utilityObjectData.m_UtilityTypes & UtilityTypes.WaterPipe) != UtilityTypes.None)
					{
						originalWaterObject = subObject;
					}
				}
			}
		}

		private void FindRoadUtilityLanes(Entity building, Entity road, float3 connectionPosition, out Bezier4x3 electricityCurve, out Entity electricityLanePrefab, out Entity electricityObjectPrefab, out PathNode electricityNode, out Bezier4x3 sewageCurve, out Entity sewageLanePrefab, out Entity sewageObjectPrefab, out PathNode sewageNode, out Bezier4x3 waterCurve, out Entity waterLanePrefab, out Entity waterObjectPrefab, out PathNode waterNode)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_062e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0630: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0699: Unknown result type (might be due to invalid IL or missing references)
			//IL_069b: Unknown result type (might be due to invalid IL or missing references)
			//IL_063f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0641: Unknown result type (might be due to invalid IL or missing references)
			//IL_0643: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_064e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0656: Unknown result type (might be due to invalid IL or missing references)
			//IL_065e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_066a: Unknown result type (might be due to invalid IL or missing references)
			//IL_066f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0679: Unknown result type (might be due to invalid IL or missing references)
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0683: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0704: Unknown result type (might be due to invalid IL or missing references)
			//IL_0706: Unknown result type (might be due to invalid IL or missing references)
			//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06da: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0715: Unknown result type (might be due to invalid IL or missing references)
			//IL_0717: Unknown result type (might be due to invalid IL or missing references)
			//IL_0719: Unknown result type (might be due to invalid IL or missing references)
			//IL_071f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0724: Unknown result type (might be due to invalid IL or missing references)
			//IL_072c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0734: Unknown result type (might be due to invalid IL or missing references)
			//IL_0739: Unknown result type (might be due to invalid IL or missing references)
			//IL_0740: Unknown result type (might be due to invalid IL or missing references)
			//IL_0745: Unknown result type (might be due to invalid IL or missing references)
			//IL_074f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0754: Unknown result type (might be due to invalid IL or missing references)
			//IL_0759: Unknown result type (might be due to invalid IL or missing references)
			//IL_0761: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_053a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_054c: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_056c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_0580: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_058b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_060e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			electricityCurve = default(Bezier4x3);
			electricityLanePrefab = default(Entity);
			electricityObjectPrefab = default(Entity);
			electricityNode = default(PathNode);
			sewageCurve = default(Bezier4x3);
			sewageLanePrefab = default(Entity);
			sewageObjectPrefab = default(Entity);
			sewageNode = default(PathNode);
			waterCurve = default(Bezier4x3);
			waterLanePrefab = default(Entity);
			waterObjectPrefab = default(Entity);
			waterNode = default(PathNode);
			DynamicBuffer<Game.Net.SubLane> val = default(DynamicBuffer<Game.Net.SubLane>);
			if (!m_SubLanes.TryGetBuffer(road, ref val))
			{
				return;
			}
			EdgeGeometry edgeGeometry = m_EdgeGeometryData[road];
			Transform transform = m_TransformData[building];
			PrefabRef prefabRef = m_PrefabRefData[building];
			BuildingData buildingData = m_PrefabBuildingData[prefabRef.m_Prefab];
			int3 val2 = default(int3);
			if (m_ElectricityConsumerData.HasComponent(building) && (buildingData.m_Flags & Game.Prefabs.BuildingFlags.HasLowVoltageNode) == 0)
			{
				val2.x = 4;
			}
			if (m_WaterConsumerData.HasComponent(building))
			{
				if ((buildingData.m_Flags & Game.Prefabs.BuildingFlags.HasSewageNode) == 0)
				{
					val2.y = 4;
				}
				if ((buildingData.m_Flags & Game.Prefabs.BuildingFlags.HasWaterNode) == 0)
				{
					val2.z = 4;
				}
			}
			float3 val3 = math.rotate(transform.m_Rotation, new float3(0.15f, 0f, 0f));
			float3 val4 = connectionPosition - val3 * (float)math.csum(((int3)(ref val2)).xz);
			float3 val5 = transform.m_Position - val3 * (float)math.csum(((int3)(ref val2)).xz);
			float3 val6 = val4;
			float3 startPos = val5;
			val4 += val3 * (float)math.csum(((int3)(ref val2)).xy);
			float3 val7 = val5 + val3 * (float)math.csum(((int3)(ref val2)).xy);
			float3 val8 = val4;
			float3 startPos2 = val7;
			val4 += val3 * (float)math.csum(((int3)(ref val2)).yz);
			float3 val9 = val7 + val3 * (float)math.csum(((int3)(ref val2)).yz);
			float3 val10 = val4;
			float3 startPos3 = val9;
			Entity val11 = Entity.Null;
			Entity val12 = Entity.Null;
			Entity val13 = Entity.Null;
			float delta = 0f;
			float delta2 = 0f;
			float delta3 = 0f;
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float num3 = float.MaxValue;
			float num5 = default(float);
			float num7 = default(float);
			float num9 = default(float);
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				if (!m_UtilityLaneData.HasComponent(subLane) || !m_EdgeLaneData.HasComponent(subLane))
				{
					continue;
				}
				Curve curve = m_CurveData[subLane];
				PrefabRef prefabRef2 = m_PrefabRefData[subLane];
				UtilityLaneData utilityLaneData = m_PrefabUtilityLaneData[prefabRef2.m_Prefab];
				if ((utilityLaneData.m_UtilityTypes & UtilityTypes.LowVoltageLine) != UtilityTypes.None && val2.x != 0)
				{
					float num4 = MathUtils.Distance(curve.m_Bezier, val8, ref num5);
					if (num4 < num)
					{
						val11 = subLane;
						delta = num5;
						num = num4;
					}
				}
				if ((utilityLaneData.m_UtilityTypes & UtilityTypes.SewagePipe) != UtilityTypes.None && val2.y != 0)
				{
					float num6 = MathUtils.Distance(curve.m_Bezier, val6, ref num7);
					if (num6 < num2)
					{
						val12 = subLane;
						delta2 = num7;
						num2 = num6;
					}
				}
				if ((utilityLaneData.m_UtilityTypes & UtilityTypes.WaterPipe) != UtilityTypes.None && val2.z != 0)
				{
					float num8 = MathUtils.Distance(curve.m_Bezier, val10, ref num9);
					if (num8 < num3)
					{
						val13 = subLane;
						delta3 = num9;
						num3 = num8;
					}
				}
			}
			float yOffset = CalculateYOffset(edgeGeometry, val11);
			float yOffset2 = CalculateYOffset(edgeGeometry, val12);
			float yOffset3 = CalculateYOffset(edgeGeometry, val13);
			bool2 val14 = ShouldCheckNodeLanes(val11, delta);
			bool2 val15 = ShouldCheckNodeLanes(val12, delta2);
			bool2 val16 = ShouldCheckNodeLanes(val13, delta3);
			bool2 val17 = val14 | val15 | val16;
			if (math.any(val17))
			{
				Game.Net.Edge edge = m_EdgeData[road];
				if (val17.x && m_SubLanes.TryGetBuffer(edge.m_Start, ref val))
				{
					float num11 = default(float);
					float num13 = default(float);
					float num15 = default(float);
					for (int j = 0; j < val.Length; j++)
					{
						Entity subLane2 = val[j].m_SubLane;
						if (!m_UtilityLaneData.HasComponent(subLane2))
						{
							continue;
						}
						Curve curve2 = m_CurveData[subLane2];
						PrefabRef prefabRef3 = m_PrefabRefData[subLane2];
						UtilityLaneData utilityLaneData2 = m_PrefabUtilityLaneData[prefabRef3.m_Prefab];
						if ((utilityLaneData2.m_UtilityTypes & UtilityTypes.LowVoltageLine) != UtilityTypes.None && val14.x)
						{
							float num10 = MathUtils.Distance(curve2.m_Bezier, val8, ref num11);
							if (num10 < num)
							{
								val11 = subLane2;
								delta = num11;
								num = num10;
							}
						}
						if ((utilityLaneData2.m_UtilityTypes & UtilityTypes.SewagePipe) != UtilityTypes.None && val15.x)
						{
							float num12 = MathUtils.Distance(curve2.m_Bezier, val6, ref num13);
							if (num12 < num2)
							{
								val12 = subLane2;
								delta2 = num13;
								num2 = num12;
							}
						}
						if ((utilityLaneData2.m_UtilityTypes & UtilityTypes.WaterPipe) != UtilityTypes.None && val16.x)
						{
							float num14 = MathUtils.Distance(curve2.m_Bezier, val10, ref num15);
							if (num14 < num3)
							{
								val13 = subLane2;
								delta3 = num15;
								num3 = num14;
							}
						}
					}
				}
				if (val17.y && m_SubLanes.TryGetBuffer(edge.m_End, ref val))
				{
					float num17 = default(float);
					float num19 = default(float);
					float num21 = default(float);
					for (int k = 0; k < val.Length; k++)
					{
						Entity subLane3 = val[k].m_SubLane;
						if (!m_UtilityLaneData.HasComponent(subLane3))
						{
							continue;
						}
						Curve curve3 = m_CurveData[subLane3];
						PrefabRef prefabRef4 = m_PrefabRefData[subLane3];
						UtilityLaneData utilityLaneData3 = m_PrefabUtilityLaneData[prefabRef4.m_Prefab];
						if ((utilityLaneData3.m_UtilityTypes & UtilityTypes.LowVoltageLine) != UtilityTypes.None && val14.y)
						{
							float num16 = MathUtils.Distance(curve3.m_Bezier, val8, ref num17);
							if (num16 < num)
							{
								val11 = subLane3;
								delta = num17;
								num = num16;
							}
						}
						if ((utilityLaneData3.m_UtilityTypes & UtilityTypes.SewagePipe) != UtilityTypes.None && val15.y)
						{
							float num18 = MathUtils.Distance(curve3.m_Bezier, val6, ref num19);
							if (num18 < num2)
							{
								val12 = subLane3;
								delta2 = num19;
								num2 = num18;
							}
						}
						if ((utilityLaneData3.m_UtilityTypes & UtilityTypes.WaterPipe) != UtilityTypes.None && val16.y)
						{
							float num20 = MathUtils.Distance(curve3.m_Bezier, val10, ref num21);
							if (num20 < num3)
							{
								val13 = subLane3;
								delta3 = num21;
								num3 = num20;
							}
						}
					}
				}
			}
			if (val11 != Entity.Null)
			{
				electricityCurve = CalculateConnectCurve(startPos2, val8, val11, yOffset, delta);
				electricityLanePrefab = GetLanePrefab(val11, m_BuildingConfigurationData.m_ElectricityConnectionLane, val2.x);
				electricityObjectPrefab = GetNodeObjectPrefab(electricityLanePrefab);
				electricityNode = GetPathNode(val11, delta);
			}
			if (val12 != Entity.Null)
			{
				sewageCurve = CalculateConnectCurve(startPos, val6, val12, yOffset2, delta2);
				sewageLanePrefab = GetLanePrefab(val12, m_BuildingConfigurationData.m_SewageConnectionLane, val2.y);
				sewageObjectPrefab = GetNodeObjectPrefab(sewageLanePrefab);
				sewageNode = GetPathNode(val12, delta2);
			}
			if (val13 != Entity.Null)
			{
				waterCurve = CalculateConnectCurve(startPos3, val10, val13, yOffset3, delta3);
				waterLanePrefab = GetLanePrefab(val13, m_BuildingConfigurationData.m_WaterConnectionLane, val2.z);
				waterObjectPrefab = GetNodeObjectPrefab(waterLanePrefab);
				waterNode = GetPathNode(val13, delta3);
			}
		}

		private bool2 ShouldCheckNodeLanes(Entity bestLane, float delta)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			if (bestLane != Entity.Null)
			{
				bool2 val = delta == new float2(0f, 1f);
				if (math.any(val))
				{
					EdgeLane edgeLane = m_EdgeLaneData[bestLane];
					return math.select(edgeLane.m_EdgeDelta.x, edgeLane.m_EdgeDelta.y, val.y) == new float2(0f, 1f);
				}
			}
			return bool2.op_Implicit(false);
		}

		private float CalculateYOffset(EdgeGeometry edgeGeometry, Entity roadLane)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			if (roadLane != Entity.Null)
			{
				Curve curve = m_CurveData[roadLane];
				EdgeLane edgeLane = m_EdgeLaneData[roadLane];
				if (edgeLane.m_EdgeDelta.x > 0.5f)
				{
					float num = math.saturate(edgeLane.m_EdgeDelta.x * 2f - 1f);
					return curve.m_Bezier.a.y - math.lerp(MathUtils.Position(edgeGeometry.m_End.m_Left, num).y, MathUtils.Position(edgeGeometry.m_End.m_Right, num).y, 0.5f);
				}
				float num2 = math.saturate(edgeLane.m_EdgeDelta.x * 2f);
				return curve.m_Bezier.a.y - math.lerp(MathUtils.Position(edgeGeometry.m_Start.m_Left, num2).y, MathUtils.Position(edgeGeometry.m_Start.m_Right, num2).y, 0.5f);
			}
			return 0f;
		}

		private Bezier4x3 CalculateConnectCurve(float3 startPos, float3 connectPos, Entity roadLane, float yOffset, float delta)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			Curve curve = m_CurveData[roadLane];
			startPos.y += yOffset;
			connectPos.y += yOffset;
			float3 val = MathUtils.Position(curve.m_Bezier, delta);
			return NetUtils.FitCurve(new Segment(startPos, connectPos), new Segment(val, connectPos));
		}

		private Entity GetNodeObjectPrefab(Entity lanePrefab)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return m_PrefabUtilityLaneData[lanePrefab].m_NodeObjectPrefab;
		}

		private Entity GetLanePrefab(Entity roadLane, Entity consumerConnectionPrefab, int needConnection)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			if (needConnection < 6)
			{
				return consumerConnectionPrefab;
			}
			Entity prefab = m_PrefabRefData[roadLane].m_Prefab;
			if (needConnection > 6)
			{
				UtilityLaneData utilityLaneData = m_PrefabUtilityLaneData[prefab];
				if (!(utilityLaneData.m_LocalConnectionPrefab != Entity.Null))
				{
					return prefab;
				}
				return utilityLaneData.m_LocalConnectionPrefab;
			}
			return prefab;
		}

		private PathNode GetPathNode(Entity roadLane, float delta)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return new PathNode(m_LaneData[roadLane].m_MiddleNode, delta);
		}

		private void DeleteLane(int jobIndex, Entity lane)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, lane, ref m_AppliedTypes);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, lane);
		}

		private void UpdateLane(int jobIndex, Entity lane, Bezier4x3 curve, PathNode endNode, bool isTemp, Temp temp, Entity original)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			Curve curve2 = default(Curve);
			curve2.m_Bezier = curve;
			curve2.m_Length = MathUtils.Length(curve);
			Lane lane2 = m_LaneData[lane];
			lane2.m_EndNode = endNode;
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, lane, curve2);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, lane, lane2);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, lane, default(Updated));
			if (isTemp)
			{
				temp.m_Original = original;
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, lane, temp);
			}
		}

		private void CreateLane(int jobIndex, Entity owner, int laneIndex, Entity prefab, Bezier4x3 curve, PathNode endNode, float3 connectPos, bool isTemp, Temp temp, Entity original)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			NetLaneArchetypeData netLaneArchetypeData = m_PrefabNetLaneArchetypeData[prefab];
			Owner owner2 = new Owner(owner);
			PrefabRef prefabRef = new PrefabRef(prefab);
			Game.Net.SecondaryLane secondaryLane = default(Game.Net.SecondaryLane);
			Lane lane = default(Lane);
			lane.m_StartNode = new PathNode(owner, (ushort)laneIndex);
			lane.m_MiddleNode = new PathNode(owner, (ushort)(laneIndex + 1));
			lane.m_EndNode = endNode;
			Curve curve2 = default(Curve);
			curve2.m_Bezier = curve;
			curve2.m_Length = MathUtils.Length(curve);
			Game.Net.Elevation elevation = default(Game.Net.Elevation);
			elevation.m_Elevation.x = curve.a.y - connectPos.y;
			elevation.m_Elevation.y = curve.d.y - connectPos.y;
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, netLaneArchetypeData.m_LaneArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val, prefabRef);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val, lane);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val, curve2);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val, owner2);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Game.Net.SecondaryLane>(jobIndex, val, secondaryLane);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Game.Net.Elevation>(jobIndex, val, elevation);
			if (isTemp)
			{
				temp.m_Original = original;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(jobIndex, val, temp);
			}
		}

		private void DeleteObject(int jobIndex, Entity lane)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, lane, ref m_AppliedTypes);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, lane);
		}

		private void UpdateObject(int jobIndex, Entity obj, float3 position, bool isTemp, Temp temp, Entity original)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = default(Transform);
			transform.m_Position = position;
			transform.m_Rotation = quaternion.identity;
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, obj, transform);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, obj, default(Updated));
			if (isTemp)
			{
				temp.m_Original = original;
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, obj, temp);
			}
		}

		private void CreateObject(int jobIndex, Entity owner, Entity prefab, float3 position, float3 connectPos, bool isTemp, Temp temp, Entity original)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			ObjectData objectData = m_PrefabObjectData[prefab];
			Owner owner2 = new Owner(owner);
			PrefabRef prefabRef = new PrefabRef(prefab);
			Secondary secondary = default(Secondary);
			Transform transform = default(Transform);
			transform.m_Position = position;
			transform.m_Rotation = quaternion.identity;
			Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
			elevation.m_Elevation = position.y - connectPos.y;
			elevation.m_Flags = (ElevationFlags)0;
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, objectData.m_Archetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val, prefabRef);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, val, transform);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val, owner2);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Secondary>(jobIndex, val, secondary);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Game.Objects.Elevation>(jobIndex, val, elevation);
			if (isTemp)
			{
				temp.m_Original = original;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(jobIndex, val, temp);
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<ConnectedBuilding> __Game_Buildings_ConnectedBuilding_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedBuilding> __Game_Buildings_ConnectedBuilding_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		public ComponentLookup<BackSide> __Game_Buildings_BackSide_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Created> __Game_Common_Created_RO_ComponentLookup;

		public ComponentLookup<Building> __Game_Buildings_Building_RW_ComponentLookup;

		public BufferLookup<ConnectedBuilding> __Game_Buildings_ConnectedBuilding_RW_BufferLookup;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> __Game_Buildings_WaterConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.UtilityObject> __Game_Objects_UtilityObject_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Secondary> __Game_Objects_Secondary_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.UtilityLane> __Game_Net_UtilityLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeLane> __Game_Net_EdgeLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.SecondaryLane> __Game_Net_SecondaryLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> __Game_Prefabs_UtilityLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UtilityObjectData> __Game_Prefabs_UtilityObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneArchetypeData> __Game_Prefabs_NetLaneArchetypeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectData> __Game_Prefabs_ObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Prefabs.SpawnLocationData> __Game_Prefabs_SpawnLocationData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SpawnLocationElement> __Game_Buildings_SpawnLocationElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

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
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Net_EdgeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EndNodeGeometry>(true);
			__Game_Objects_SpawnLocation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.SpawnLocation>(true);
			__Game_Buildings_ConnectedBuilding_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedBuilding>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(true);
			__Game_Buildings_ConnectedBuilding_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedBuilding>(true);
			__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubNet>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Buildings_BackSide_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BackSide>(false);
			__Game_Common_Created_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Created>(true);
			__Game_Buildings_Building_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(false);
			__Game_Buildings_ConnectedBuilding_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedBuilding>(false);
			__Game_Buildings_WaterConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterConsumer>(true);
			__Game_Buildings_ElectricityConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityConsumer>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Objects_UtilityObject_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.UtilityObject>(true);
			__Game_Objects_Secondary_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Secondary>(true);
			__Game_Net_UtilityLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.UtilityLane>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Edge>(true);
			__Game_Net_EdgeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeLane>(true);
			__Game_Net_SecondaryLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.SecondaryLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Prefabs_UtilityLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UtilityLaneData>(true);
			__Game_Prefabs_UtilityObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UtilityObjectData>(true);
			__Game_Prefabs_NetLaneArchetypeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneArchetypeData>(true);
			__Game_Prefabs_ObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_SpawnLocationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Prefabs.SpawnLocationData>(true);
			__Game_Buildings_SpawnLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SpawnLocationElement>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
		}
	}

	private ModificationBarrier4B m_ModificationBarrier;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private IconCommandSystem m_IconCommandSystem;

	private AudioManager m_AudioManager;

	private EntityQuery m_ModificationQuery;

	private EntityQuery m_UpdatedNetQuery;

	private EntityQuery m_TrafficConfigQuery;

	private EntityQuery m_BuildingConfigQuery;

	private EntityQuery m_ConnectionQuery;

	private EntityArchetype m_RoadConnectionEventArchetype;

	private ComponentTypeSet m_AppliedTypes;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected O, but got Unknown
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4B>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_AudioManager = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AudioManager>();
		EntityQueryDesc[] array = new EntityQueryDesc[3];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Building>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Objects.SpawnLocation>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[1] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Net.Edge>(),
			ComponentType.ReadOnly<ConnectedBuilding>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[2] = val;
		m_ModificationQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_UpdatedNetQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Net.Edge>(),
			ComponentType.ReadOnly<ConnectedBuilding>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_TrafficConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TrafficConfigurationData>() });
		m_BuildingConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BuildingConfigurationData>() });
		m_ConnectionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ConnectionLaneData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_RoadConnectionEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<RoadConnectionUpdated>()
		});
		m_AppliedTypes = new ComponentTypeSet(ComponentType.ReadWrite<Applied>(), ComponentType.ReadWrite<Created>(), ComponentType.ReadWrite<Updated>());
		((ComponentSystemBase)this).RequireForUpdate(m_ModificationQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07de: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_081d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_083a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0841: Unknown result type (might be due to invalid IL or missing references)
		//IL_0843: Unknown result type (might be due to invalid IL or missing references)
		//IL_084a: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0853: Unknown result type (might be due to invalid IL or missing references)
		//IL_0858: Unknown result type (might be due to invalid IL or missing references)
		//IL_0877: Unknown result type (might be due to invalid IL or missing references)
		//IL_087c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0880: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0894: Unknown result type (might be due to invalid IL or missing references)
		//IL_0899: Unknown result type (might be due to invalid IL or missing references)
		//IL_089a: Unknown result type (might be due to invalid IL or missing references)
		//IL_089f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08af: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08db: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08df: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_090a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0917: Unknown result type (might be due to invalid IL or missing references)
		//IL_0924: Unknown result type (might be due to invalid IL or missing references)
		//IL_092c: Unknown result type (might be due to invalid IL or missing references)
		//IL_092e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0930: Unknown result type (might be due to invalid IL or missing references)
		//IL_0941: Unknown result type (might be due to invalid IL or missing references)
		//IL_094e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0953: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<Entity> replaceRoadConnectionQueue = default(NativeQueue<Entity>);
		replaceRoadConnectionQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<ReplaceRoad> val = default(NativeList<ReplaceRoad>);
		val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle dependencies;
		CheckRoadConnectionJob checkRoadConnectionJob = new CheckRoadConnectionJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StartNodeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EndNodeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedBuildingType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedBuilding>(ref __TypeHandle.__Game_Buildings_ConnectedBuilding_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ReplaceRoadConnectionQueue = replaceRoadConnectionQueue.AsParallelWriter()
		};
		FillReplacementListJob fillReplacementListJob = new FillReplacementListJob
		{
			m_ReplaceRoadConnectionQueue = replaceRoadConnectionQueue,
			m_ReplaceRoadConnection = val
		};
		JobHandle val2 = default(JobHandle);
		NativeList<ArchetypeChunk> updatedNetChunks = ((EntityQuery)(ref m_UpdatedNetQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
		JobHandle dependencies2;
		FindRoadConnectionJob findRoadConnectionJob = new FindRoadConnectionJob
		{
			m_ConnectedBuildings = InternalCompilerInterface.GetBufferLookup<ConnectedBuilding>(ref __TypeHandle.__Game_Buildings_ConnectedBuilding_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubNets = InternalCompilerInterface.GetBufferLookup<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BackSideData = InternalCompilerInterface.GetComponentLookup<BackSide>(ref __TypeHandle.__Game_Buildings_BackSide_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpdatedNetChunks = updatedNetChunks,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies2),
			m_ReplaceRoadConnection = val.AsDeferredJobArray()
		};
		JobHandle deps;
		ReplaceRoadConnectionJob replaceRoadConnectionJob = new ReplaceRoadConnectionJob
		{
			m_CreatedData = InternalCompilerInterface.GetComponentLookup<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedBuildings = InternalCompilerInterface.GetBufferLookup<ConnectedBuilding>(ref __TypeHandle.__Game_Buildings_ConnectedBuilding_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadConnectionEventArchetype = m_RoadConnectionEventArchetype,
			m_ReplaceRoadConnection = val,
			m_TrafficConfigurationData = ((EntityQuery)(ref m_TrafficConfigQuery)).GetSingleton<TrafficConfigurationData>(),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer(),
			m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer(),
			m_SourceUpdateData = m_AudioManager.GetSourceUpdateData(out deps)
		};
		JobHandle val3 = default(JobHandle);
		NativeList<Entity> connectionPrefabs = ((EntityQuery)(ref m_ConnectionQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val3);
		UpdateSecondaryLanesJob updateSecondaryLanesJob = new UpdateSecondaryLanesJob
		{
			m_WaterConsumerData = InternalCompilerInterface.GetComponentLookup<WaterConsumer>(ref __TypeHandle.__Game_Buildings_WaterConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityConsumerData = InternalCompilerInterface.GetComponentLookup<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UtilityObjectData = InternalCompilerInterface.GetComponentLookup<Game.Objects.UtilityObject>(ref __TypeHandle.__Game_Objects_UtilityObject_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SecondaryData = InternalCompilerInterface.GetComponentLookup<Secondary>(ref __TypeHandle.__Game_Objects_Secondary_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UtilityLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.UtilityLane>(ref __TypeHandle.__Game_Net_UtilityLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SecondaryLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.SecondaryLane>(ref __TypeHandle.__Game_Net_SecondaryLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabUtilityLaneData = InternalCompilerInterface.GetComponentLookup<UtilityLaneData>(ref __TypeHandle.__Game_Prefabs_UtilityLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabUtilityObjectData = InternalCompilerInterface.GetComponentLookup<UtilityObjectData>(ref __TypeHandle.__Game_Prefabs_UtilityObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetLaneArchetypeData = InternalCompilerInterface.GetComponentLookup<NetLaneArchetypeData>(ref __TypeHandle.__Game_Prefabs_NetLaneArchetypeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectData = InternalCompilerInterface.GetComponentLookup<ObjectData>(ref __TypeHandle.__Game_Prefabs_ObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Prefabs.SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocations = InternalCompilerInterface.GetBufferLookup<SpawnLocationElement>(ref __TypeHandle.__Game_Buildings_SpawnLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionPrefabs = connectionPrefabs,
			m_ReplaceRoadConnection = val,
			m_AppliedTypes = m_AppliedTypes,
			m_BuildingConfigurationData = ((EntityQuery)(ref m_BuildingConfigQuery)).GetSingleton<BuildingConfigurationData>()
		};
		EntityCommandBuffer val4 = m_ModificationBarrier.CreateCommandBuffer();
		updateSecondaryLanesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val4)).AsParallelWriter();
		UpdateSecondaryLanesJob updateSecondaryLanesJob2 = updateSecondaryLanesJob;
		JobHandle val5 = JobChunkExtensions.ScheduleParallel<CheckRoadConnectionJob>(checkRoadConnectionJob, m_ModificationQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		JobHandle val6 = IJobExtensions.Schedule<FillReplacementListJob>(fillReplacementListJob, val5);
		JobHandle val7 = IJobParallelForDeferExtensions.Schedule<FindRoadConnectionJob, ReplaceRoad>(findRoadConnectionJob, val, 1, JobUtils.CombineDependencies(val6, dependencies2, val2, val3));
		JobHandle val8 = IJobExtensions.Schedule<ReplaceRoadConnectionJob>(replaceRoadConnectionJob, JobHandle.CombineDependencies(val7, deps));
		JobHandle val9 = IJobParallelForDeferExtensions.Schedule<UpdateSecondaryLanesJob, ReplaceRoad>(updateSecondaryLanesJob2, val, 1, val7);
		replaceRoadConnectionQueue.Dispose(val6);
		updatedNetChunks.Dispose(val7);
		connectionPrefabs.Dispose(val9);
		m_ObjectSearchSystem.AddStaticSearchTreeReader(val5);
		m_NetSearchSystem.AddNetSearchTreeReader(val7);
		m_IconCommandSystem.AddCommandBufferWriter(val8);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(val8, val9);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
		val.Dispose(((SystemBase)this).Dependency);
	}

	private static void CheckDistance(EdgeGeometry edgeGeometry, EdgeNodeGeometry startGeometry, EdgeNodeGeometry endGeometry, float3 position, bool canBeOnRoad, ref float maxDistance)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		if (MathUtils.DistanceSquared(((Bounds3)(ref edgeGeometry.m_Bounds)).xz, ((float3)(ref position)).xz) < maxDistance * maxDistance)
		{
			CheckDistance(edgeGeometry.m_Start.m_Left, position, ref maxDistance);
			CheckDistance(edgeGeometry.m_Start.m_Right, position, ref maxDistance);
			CheckDistance(edgeGeometry.m_End.m_Left, position, ref maxDistance);
			CheckDistance(edgeGeometry.m_End.m_Right, position, ref maxDistance);
			if (canBeOnRoad)
			{
				CheckDistance(edgeGeometry.m_Start.m_Left, edgeGeometry.m_Start.m_Right, position, ref maxDistance);
				CheckDistance(edgeGeometry.m_End.m_Left, edgeGeometry.m_End.m_Right, position, ref maxDistance);
			}
		}
		if (MathUtils.DistanceSquared(((Bounds3)(ref startGeometry.m_Bounds)).xz, ((float3)(ref position)).xz) < maxDistance * maxDistance)
		{
			CheckDistance(startGeometry.m_Left.m_Left, position, ref maxDistance);
			CheckDistance(startGeometry.m_Right.m_Right, position, ref maxDistance);
			if (startGeometry.m_MiddleRadius > 0f)
			{
				CheckDistance(startGeometry.m_Left.m_Right, position, ref maxDistance);
				CheckDistance(startGeometry.m_Right.m_Left, position, ref maxDistance);
			}
			if (canBeOnRoad)
			{
				if (startGeometry.m_MiddleRadius > 0f)
				{
					CheckDistance(startGeometry.m_Left.m_Left, startGeometry.m_Left.m_Right, position, ref maxDistance);
					CheckDistance(startGeometry.m_Right.m_Left, startGeometry.m_Middle, position, ref maxDistance);
					CheckDistance(startGeometry.m_Middle, startGeometry.m_Right.m_Right, position, ref maxDistance);
				}
				else
				{
					CheckDistance(startGeometry.m_Left.m_Left, startGeometry.m_Middle, position, ref maxDistance);
					CheckDistance(startGeometry.m_Middle, startGeometry.m_Right.m_Right, position, ref maxDistance);
				}
			}
		}
		if (!(MathUtils.DistanceSquared(((Bounds3)(ref endGeometry.m_Bounds)).xz, ((float3)(ref position)).xz) < maxDistance * maxDistance))
		{
			return;
		}
		CheckDistance(endGeometry.m_Left.m_Left, position, ref maxDistance);
		CheckDistance(endGeometry.m_Right.m_Right, position, ref maxDistance);
		if (endGeometry.m_MiddleRadius > 0f)
		{
			CheckDistance(endGeometry.m_Left.m_Right, position, ref maxDistance);
			CheckDistance(endGeometry.m_Right.m_Left, position, ref maxDistance);
		}
		if (canBeOnRoad)
		{
			if (endGeometry.m_MiddleRadius > 0f)
			{
				CheckDistance(endGeometry.m_Left.m_Left, endGeometry.m_Left.m_Right, position, ref maxDistance);
				CheckDistance(endGeometry.m_Right.m_Left, endGeometry.m_Middle, position, ref maxDistance);
				CheckDistance(endGeometry.m_Middle, endGeometry.m_Right.m_Right, position, ref maxDistance);
			}
			else
			{
				CheckDistance(endGeometry.m_Left.m_Left, endGeometry.m_Middle, position, ref maxDistance);
				CheckDistance(endGeometry.m_Middle, endGeometry.m_Right.m_Right, position, ref maxDistance);
			}
		}
	}

	private static void CheckDistance(Bezier4x3 curve1, Bezier4x3 curve2, float3 position, ref float maxDistance)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (MathUtils.DistanceSquared(MathUtils.Bounds(((Bezier4x3)(ref curve1)).xz) | MathUtils.Bounds(((Bezier4x3)(ref curve2)).xz), ((float3)(ref position)).xz) < maxDistance * maxDistance)
		{
			float num = default(float);
			MathUtils.Distance(MathUtils.Lerp(((Bezier4x3)(ref curve1)).xz, ((Bezier4x3)(ref curve2)).xz, 0.5f), ((float3)(ref position)).xz, ref num);
			float num3 = default(float);
			float num2 = MathUtils.Distance(new Segment(MathUtils.Position(((Bezier4x3)(ref curve1)).xz, num), MathUtils.Position(((Bezier4x3)(ref curve2)).xz, num)), ((float3)(ref position)).xz, ref num3);
			maxDistance = math.min(num2, maxDistance);
		}
	}

	private static void CheckDistance(Bezier4x3 curve, float3 position, ref float maxDistance)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (MathUtils.DistanceSquared(MathUtils.Bounds(((Bezier4x3)(ref curve)).xz), ((float3)(ref position)).xz) < maxDistance * maxDistance)
		{
			float num2 = default(float);
			float num = MathUtils.Distance(((Bezier4x3)(ref curve)).xz, ((float3)(ref position)).xz, ref num2);
			maxDistance = math.min(num, maxDistance);
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
	public RoadConnectionSystem()
	{
	}
}
