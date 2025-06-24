using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Objects;

[CompilerGenerated]
public class AttachPositionSystem : GameSystemBase
{
	[BurstCompile]
	private struct AttachPositionJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Attached> m_AttachedType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<Attached> m_AttachedData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> m_PrefabPlaceableObjectData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> m_PrefabRouteConnectionData;

		[ReadOnly]
		public ComponentLookup<CarLaneData> m_PrefabCarLaneData;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> m_PrefabTrackLaneData;

		[ReadOnly]
		public ComponentLookup<PillarData> m_PrefabPillarData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabLaneData;

		[ReadOnly]
		public BufferLookup<SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<NetCompositionArea> m_NetCompositionAreas;

		[ReadOnly]
		public BufferLookup<NetCompositionLane> m_NetCompositionLanes;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Transform> m_TransformData;

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
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Attached> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Attached>(ref m_AttachedType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity val = nativeArray[i];
				Attached attached = nativeArray2[i];
				PrefabRef prefabRef = nativeArray3[i];
				Transform transform = m_TransformData[val];
				Transform transform2 = transform;
				FixAttachedPosition(attached, prefabRef, ref transform2);
				if (!transform2.Equals(transform))
				{
					MoveObject(val, transform, transform2);
				}
			}
		}

		private void MoveObject(Entity entity, Transform oldTransform, Transform newTransform)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			m_TransformData[entity] = newTransform;
			DynamicBuffer<SubObject> val = default(DynamicBuffer<SubObject>);
			if (!m_SubObjects.TryGetBuffer(entity, ref val))
			{
				return;
			}
			Transform inverseParentTransform = ObjectUtils.InverseTransform(oldTransform);
			Owner owner = default(Owner);
			for (int i = 0; i < val.Length; i++)
			{
				Entity subObject = val[i].m_SubObject;
				if (m_OwnerData.TryGetComponent(subObject, ref owner) && !(owner.m_Owner != entity) && m_UpdatedData.HasComponent(subObject) && !m_AttachedData.HasComponent(subObject))
				{
					Transform transform = m_TransformData[subObject];
					Transform newTransform2 = ObjectUtils.LocalToWorld(newTransform, ObjectUtils.WorldToLocal(inverseParentTransform, transform));
					if (!newTransform2.Equals(transform))
					{
						MoveObject(subObject, transform, newTransform2);
					}
				}
			}
		}

		private void FixAttachedPosition(Attached attached, PrefabRef prefabRef, ref Transform transform)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			if (!m_PrefabPlaceableObjectData.HasComponent(prefabRef.m_Prefab) || (m_PrefabPlaceableObjectData[prefabRef.m_Prefab].m_Flags & PlacementFlags.CanOverlap) != PlacementFlags.None)
			{
				return;
			}
			if (m_PrefabPillarData.HasComponent(prefabRef.m_Prefab))
			{
				PillarData pillarData = m_PrefabPillarData[prefabRef.m_Prefab];
				if (pillarData.m_Type != PillarType.Vertical && pillarData.m_Type != PillarType.Base)
				{
					return;
				}
			}
			if (m_NodeData.HasComponent(attached.m_Parent))
			{
				Node node = m_NodeData[attached.m_Parent];
				transform.m_Position = node.m_Position;
				transform.m_Rotation = node.m_Rotation;
			}
			else
			{
				if (!m_CompositionData.HasComponent(attached.m_Parent))
				{
					return;
				}
				Composition composition = m_CompositionData[attached.m_Parent];
				EdgeGeometry edgeGeometry = m_EdgeGeometryData[attached.m_Parent];
				Curve curve = m_CurveData[attached.m_Parent];
				if (!m_NetCompositionAreas.HasBuffer(composition.m_Edge))
				{
					return;
				}
				DynamicBuffer<NetCompositionArea> areas = m_NetCompositionAreas[composition.m_Edge];
				NetCompositionData prefabCompositionData = m_PrefabCompositionData[composition.m_Edge];
				float num = 0f;
				if (m_PrefabObjectGeometryData.HasComponent(prefabRef.m_Prefab))
				{
					ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
					if ((objectGeometryData.m_Flags & GeometryFlags.Standing) != GeometryFlags.None)
					{
						num = objectGeometryData.m_LegSize.z * 0.5f + objectGeometryData.m_LegOffset.y;
						if (objectGeometryData.m_LegSize.y <= prefabCompositionData.m_HeightRange.max)
						{
							num = math.max(num, objectGeometryData.m_Size.z * 0.5f);
						}
					}
					else
					{
						num = objectGeometryData.m_Size.z * 0.5f;
					}
				}
				Transform bestTransform = transform;
				Transform bestTransform2 = transform;
				float3 curvePosition = MathUtils.Position(curve.m_Bezier, attached.m_CurvePosition);
				bool snapEdge = false;
				if (m_PrefabRouteConnectionData.HasComponent(prefabRef.m_Prefab) && m_NetCompositionLanes.HasBuffer(composition.m_Edge))
				{
					RouteConnectionData routeConnectionData = m_PrefabRouteConnectionData[prefabRef.m_Prefab];
					DynamicBuffer<NetCompositionLane> lanes = m_NetCompositionLanes[composition.m_Edge];
					if (routeConnectionData.m_RouteConnectionType != RouteConnectionType.None)
					{
						float3 curveTangent = MathUtils.Tangent(curve.m_Bezier, attached.m_CurvePosition);
						SnapRouteLane(transform, ref bestTransform, ref snapEdge, curvePosition, curveTangent, routeConnectionData.m_RouteConnectionType, routeConnectionData.m_RouteTrackType, routeConnectionData.m_RouteRoadType, prefabCompositionData, lanes);
					}
				}
				if (attached.m_CurvePosition < 0.5f)
				{
					SnapSegmentAreas(bestTransform, ref bestTransform2, snapEdge, num, curvePosition, edgeGeometry.m_Start, prefabCompositionData, areas);
				}
				else
				{
					SnapSegmentAreas(bestTransform, ref bestTransform2, snapEdge, num, curvePosition, edgeGeometry.m_End, prefabCompositionData, areas);
				}
				transform = bestTransform2;
			}
		}

		private void SnapRouteLane(Transform transform, ref Transform bestTransform, ref bool snapEdge, float3 curvePosition, float3 curveTangent, RouteConnectionType connectionType, TrackTypes trackType, RoadTypes carType, NetCompositionData prefabCompositionData, DynamicBuffer<NetCompositionLane> lanes)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			LaneFlags laneFlags;
			switch (connectionType)
			{
			default:
				return;
			case RouteConnectionType.Road:
				laneFlags = LaneFlags.Road;
				break;
			case RouteConnectionType.Track:
				laneFlags = LaneFlags.Track;
				break;
			case RouteConnectionType.Pedestrian:
				laneFlags = LaneFlags.Pedestrian;
				break;
			}
			float2 val = MathUtils.Right(math.normalizesafe(((float3)(ref curveTangent)).xz, default(float2)));
			float3 val2 = transform.m_Position - curvePosition;
			float2 val3 = default(float2);
			((float2)(ref val3))._002Ector(math.dot(val, ((float3)(ref val2)).xz), val2.y);
			float num = float.MaxValue;
			for (int i = 0; i < lanes.Length; i++)
			{
				NetCompositionLane netCompositionLane = lanes[i];
				if ((netCompositionLane.m_Flags & laneFlags) != laneFlags)
				{
					continue;
				}
				float num2 = math.distancesq(((float3)(ref netCompositionLane.m_Position)).xy, val3);
				if (!(num2 < num))
				{
					continue;
				}
				switch (connectionType)
				{
				case RouteConnectionType.Track:
					if (!m_PrefabTrackLaneData.HasComponent(netCompositionLane.m_Lane) || (m_PrefabTrackLaneData[netCompositionLane.m_Lane].m_TrackTypes & trackType) == 0)
					{
						continue;
					}
					break;
				case RouteConnectionType.Road:
					if (!m_PrefabCarLaneData.HasComponent(netCompositionLane.m_Lane) || (m_PrefabCarLaneData[netCompositionLane.m_Lane].m_RoadTypes & carType) == 0)
					{
						continue;
					}
					break;
				}
				num = num2;
				float2 direction = val;
				if ((netCompositionLane.m_Flags & LaneFlags.Invert) == 0)
				{
					direction = -val;
				}
				float num3 = netCompositionLane.m_Position.x;
				if ((laneFlags & (LaneFlags.Road | LaneFlags.Track)) != 0)
				{
					num3 += m_PrefabLaneData[netCompositionLane.m_Lane].m_Width * math.select(0.25f, -0.25f, math.dot(((float3)(ref transform.m_Position)).xz - ((float3)(ref curvePosition)).xz, val) < 0f);
				}
				bestTransform.m_Position = curvePosition;
				ref float3 position = ref bestTransform.m_Position;
				((float3)(ref position)).xz = ((float3)(ref position)).xz + val * num3;
				bestTransform.m_Position.y += netCompositionLane.m_Position.y;
				bestTransform.m_Rotation = ToolUtils.CalculateRotation(direction);
				snapEdge = true;
			}
		}

		private void SnapSegmentAreas(Transform transform, ref Transform bestTransform, bool snapEdge, float radius, float3 curvePosition, Segment segment, NetCompositionData prefabCompositionData, DynamicBuffer<NetCompositionArea> areas)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			float num = float.MaxValue;
			float num3 = default(float);
			for (int i = 0; i < areas.Length; i++)
			{
				NetCompositionArea netCompositionArea = areas[i];
				if ((netCompositionArea.m_Flags & NetAreaFlags.Buildable) == 0)
				{
					continue;
				}
				float num2 = netCompositionArea.m_Width * 0.51f;
				if (radius >= num2)
				{
					continue;
				}
				Bezier4x3 val = MathUtils.Lerp(segment.m_Left, segment.m_Right, netCompositionArea.m_Position.x / prefabCompositionData.m_Width + 0.5f);
				MathUtils.Distance(val, curvePosition, ref num3);
				float3 val2 = MathUtils.Position(val, num3);
				float num4 = math.distancesq(val2, transform.m_Position);
				if (num4 < num)
				{
					num = num4;
					float3 val3 = MathUtils.Tangent(val, num3);
					float2 val4 = math.normalizesafe(((float3)(ref val3)).xz, default(float2));
					val4 = (((netCompositionArea.m_Flags & NetAreaFlags.Invert) == 0) ? MathUtils.Left(val4) : MathUtils.Right(val4));
					float3 val5 = MathUtils.Position(MathUtils.Lerp(segment.m_Left, segment.m_Right, netCompositionArea.m_SnapPosition.x / prefabCompositionData.m_Width + 0.5f), num3);
					float num5 = math.max(0f, math.min(netCompositionArea.m_Width * 0.5f, math.abs(netCompositionArea.m_SnapPosition.x - netCompositionArea.m_Position.x) + netCompositionArea.m_SnapWidth * 0.5f) - radius);
					((float3)(ref val2)).xz = ((float3)(ref val2)).xz + MathUtils.ClampLength(((float3)(ref val5)).xz - ((float3)(ref val2)).xz, num5);
					if (snapEdge)
					{
						float num6 = math.max(0f, netCompositionArea.m_SnapWidth * 0.5f - radius);
						((float3)(ref val2)).xz = ((float3)(ref val2)).xz + MathUtils.ClampLength(((float3)(ref transform.m_Position)).xz - ((float3)(ref val2)).xz, num6);
					}
					val2.y += netCompositionArea.m_Position.y;
					bestTransform.m_Position = val2;
					bestTransform.m_Rotation = ToolUtils.CalculateRotation(val4);
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
		public ComponentTypeHandle<Attached> __Game_Objects_Attached_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> __Game_Prefabs_PlaceableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> __Game_Prefabs_RouteConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLaneData> __Game_Prefabs_CarLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> __Game_Prefabs_TrackLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PillarData> __Game_Prefabs_PillarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetCompositionArea> __Game_Prefabs_NetCompositionArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetCompositionLane> __Game_Prefabs_NetCompositionLane_RO_BufferLookup;

		public ComponentLookup<Transform> __Game_Objects_Transform_RW_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_Attached_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Attached>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableObjectData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_RouteConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteConnectionData>(true);
			__Game_Prefabs_CarLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLaneData>(true);
			__Game_Prefabs_TrackLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackLaneData>(true);
			__Game_Prefabs_PillarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PillarData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubObject>(true);
			__Game_Prefabs_NetCompositionArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetCompositionArea>(true);
			__Game_Prefabs_NetCompositionLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetCompositionLane>(true);
			__Game_Objects_Transform_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(false);
		}
	}

	private EntityQuery m_UpdateQuery;

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
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_UpdateQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Attached>(),
			ComponentType.Exclude<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_UpdateQuery);
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
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		AttachPositionJob attachPositionJob = new AttachPositionJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AttachedType = InternalCompilerInterface.GetComponentTypeHandle<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachedData = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPlaceableObjectData = InternalCompilerInterface.GetComponentLookup<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRouteConnectionData = InternalCompilerInterface.GetComponentLookup<RouteConnectionData>(ref __TypeHandle.__Game_Prefabs_RouteConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarLaneData = InternalCompilerInterface.GetComponentLookup<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrackLaneData = InternalCompilerInterface.GetComponentLookup<TrackLaneData>(ref __TypeHandle.__Game_Prefabs_TrackLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPillarData = InternalCompilerInterface.GetComponentLookup<PillarData>(ref __TypeHandle.__Game_Prefabs_PillarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetCompositionAreas = InternalCompilerInterface.GetBufferLookup<NetCompositionArea>(ref __TypeHandle.__Game_Prefabs_NetCompositionArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetCompositionLanes = InternalCompilerInterface.GetBufferLookup<NetCompositionLane>(ref __TypeHandle.__Game_Prefabs_NetCompositionLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<AttachPositionJob>(attachPositionJob, m_UpdateQuery, ((SystemBase)this).Dependency);
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
	public AttachPositionSystem()
	{
	}
}
