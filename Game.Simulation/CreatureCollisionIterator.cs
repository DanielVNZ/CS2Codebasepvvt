using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public struct CreatureCollisionIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
{
	public ComponentLookup<Owner> m_OwnerData;

	public ComponentLookup<Transform> m_TransformData;

	public ComponentLookup<Moving> m_MovingData;

	public ComponentLookup<Creature> m_CreatureData;

	public ComponentLookup<GroupMember> m_GroupMemberData;

	public ComponentLookup<Waypoint> m_WaypointData;

	public ComponentLookup<TaxiStand> m_TaxiStandData;

	public ComponentLookup<Curve> m_CurveData;

	public ComponentLookup<AreaLane> m_AreaLaneData;

	public ComponentLookup<PrefabRef> m_PrefabRefData;

	public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

	public ComponentLookup<NetLaneData> m_PrefabLaneData;

	public BufferLookup<LaneObject> m_LaneObjects;

	public BufferLookup<Game.Areas.Node> m_AreaNodes;

	public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_StaticObjectSearchTree;

	public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_MovingObjectSearchTree;

	public Entity m_Entity;

	public Entity m_Leader;

	public Entity m_CurrentLane;

	public Entity m_CurrentVehicle;

	public float m_CurvePosition;

	public float m_TimeStep;

	public ObjectGeometryData m_PrefabObjectGeometry;

	public Bounds1 m_SpeedRange;

	public float3 m_CurrentPosition;

	public float3 m_CurrentDirection;

	public float3 m_CurrentVelocity;

	public float m_TargetDistance;

	public PathOwner m_PathOwner;

	public DynamicBuffer<PathElement> m_PathElements;

	public float m_MinSpeed;

	public float3 m_TargetPosition;

	public float m_MaxSpeed;

	public float m_LanePosition;

	public Entity m_Blocker;

	public BlockerType m_BlockerType;

	public Entity m_QueueEntity;

	public Sphere3 m_QueueArea;

	public DynamicBuffer<Queue> m_Queues;

	private Segment m_TargetLine;

	private float m_PushFactor;

	private Bounds3 m_Bounds;

	private float m_Size;

	public bool IterateFirstLane(Entity currentLane, float2 currentOffset, bool isBackward)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return IterateFirstLane(currentLane, currentLane, currentOffset, currentOffset, isBackward);
	}

	public bool IterateFirstLane(Entity currentLane, Entity targetLane, float2 currentOffset, float2 targetOffset, bool isBackward)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		m_Size = (m_PrefabObjectGeometry.m_Bounds.max.x - m_PrefabObjectGeometry.m_Bounds.min.x) * 0.5f;
		m_PushFactor = 0.75f;
		if (m_AreaLaneData.HasComponent(targetLane))
		{
			CalculateTargetLine(targetLane, m_TargetPosition, isBackward);
			m_MovingObjectSearchTree.Iterate<CreatureCollisionIterator>(ref this, 0);
			m_StaticObjectSearchTree.Iterate<CreatureCollisionIterator>(ref this, 0);
			return false;
		}
		Curve curve = default(Curve);
		if (m_CurveData.TryGetComponent(targetLane, ref curve))
		{
			CalculateTargetLine(targetLane, targetOffset.x);
			bool result = false;
			DynamicBuffer<LaneObject> val = default(DynamicBuffer<LaneObject>);
			if (m_LaneObjects.TryGetBuffer(currentLane, ref val))
			{
				float num = m_TargetDistance / math.max(1f, curve.m_Length);
				Bounds1 val2 = default(Bounds1);
				((Bounds1)(ref val2))._002Ector(currentOffset.x - num, currentOffset.x + num);
				result = MathUtils.Intersect(val2, currentOffset.y);
				for (int i = 0; i < val.Length; i++)
				{
					LaneObject laneObject = val[i];
					Bounds1 val3 = MathUtils.Bounds(laneObject.m_CurvePosition.x, laneObject.m_CurvePosition.y);
					if (MathUtils.Intersect(val2, val3) && laneObject.m_LaneObject != m_Entity)
					{
						CheckCollision(laneObject.m_LaneObject);
					}
				}
			}
			m_StaticObjectSearchTree.Iterate<CreatureCollisionIterator>(ref this, 0);
			return result;
		}
		return false;
	}

	public bool IterateNextLane(Entity nextLane, float2 nextOffset)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		DynamicBuffer<LaneObject> val = default(DynamicBuffer<LaneObject>);
		if (m_LaneObjects.TryGetBuffer(nextLane, ref val))
		{
			float num = 5f / math.max(1f, m_CurveData[nextLane].m_Length);
			Bounds1 val2 = default(Bounds1);
			((Bounds1)(ref val2))._002Ector(nextOffset.x - num, nextOffset.x + num);
			result = MathUtils.Intersect(val2, nextOffset.y);
			for (int i = 0; i < val.Length; i++)
			{
				LaneObject laneObject = val[i];
				Bounds1 val3 = MathUtils.Bounds(laneObject.m_CurvePosition.x, laneObject.m_CurvePosition.y);
				if (MathUtils.Intersect(val2, val3) && laneObject.m_LaneObject != m_Entity)
				{
					CheckCollision(laneObject.m_LaneObject);
				}
			}
		}
		else if (m_AreaLaneData.HasComponent(nextLane))
		{
			m_MovingObjectSearchTree.Iterate<CreatureCollisionIterator>(ref this, 0);
		}
		return result;
	}

	public bool Intersect(QuadTreeBoundsXZ bounds)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if ((bounds.m_Mask & (BoundsMask.NotOverridden | BoundsMask.NotWalkThrough)) != (BoundsMask.NotOverridden | BoundsMask.NotWalkThrough))
		{
			return false;
		}
		return MathUtils.Intersect(m_Bounds, bounds.m_Bounds);
	}

	public void Iterate(QuadTreeBoundsXZ bounds, Entity item)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if ((bounds.m_Mask & (BoundsMask.NotOverridden | BoundsMask.NotWalkThrough)) == (BoundsMask.NotOverridden | BoundsMask.NotWalkThrough) && MathUtils.Intersect(m_Bounds, bounds.m_Bounds))
		{
			CheckCollision(item);
		}
	}

	private void CalculateTargetLine(Entity targetLane, float3 targetPosition, bool isBackward)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		Owner owner = m_OwnerData[targetLane];
		AreaLane areaLane = m_AreaLaneData[targetLane];
		DynamicBuffer<Game.Areas.Node> val = m_AreaNodes[owner.m_Owner];
		float3 val2 = targetPosition - m_CurrentPosition;
		float num = math.length(((float3)(ref val2)).xz);
		if (num < m_TargetDistance)
		{
			m_TargetLine = new Segment(targetPosition, targetPosition);
		}
		else
		{
			if (num > m_TargetDistance)
			{
				targetPosition = m_CurrentPosition + val2 * (m_TargetDistance / num);
			}
			float2 val3 = math.select(MathUtils.Right(((float3)(ref val2)).xz), MathUtils.Left(((float3)(ref val2)).xz), isBackward) * (0.5f / math.max(0.1f, num));
			Line3 val4 = Line3.op_Implicit(new Segment(targetPosition, targetPosition));
			ref float3 a = ref val4.a;
			((float3)(ref a)).xz = ((float3)(ref a)).xz - val3;
			ref float3 b = ref val4.b;
			((float3)(ref b)).xz = ((float3)(ref b)).xz + val3;
			Bounds1 val5 = default(Bounds1);
			float2 val6 = default(float2);
			if (areaLane.m_Nodes.y == areaLane.m_Nodes.z)
			{
				float3 position = val[areaLane.m_Nodes.x].m_Position;
				float3 position2 = val[areaLane.m_Nodes.y].m_Position;
				float3 position3 = val[areaLane.m_Nodes.w].m_Position;
				if (MathUtils.Intersect(new Segment(((float3)(ref position)).xz, ((float3)(ref position2)).xz), ((Line3)(ref val4)).xz, ref val6))
				{
					val5 |= val6.y;
				}
				if (MathUtils.Intersect(new Segment(((float3)(ref position2)).xz, ((float3)(ref position3)).xz), ((Line3)(ref val4)).xz, ref val6))
				{
					val5 |= val6.y;
				}
				if (MathUtils.Intersect(new Segment(((float3)(ref position3)).xz, ((float3)(ref position)).xz), ((Line3)(ref val4)).xz, ref val6))
				{
					val5 |= val6.y;
				}
			}
			else
			{
				float3 position4 = val[areaLane.m_Nodes.x].m_Position;
				float3 position5 = val[areaLane.m_Nodes.y].m_Position;
				float3 position6 = val[areaLane.m_Nodes.w].m_Position;
				float3 position7 = val[areaLane.m_Nodes.z].m_Position;
				if (MathUtils.Intersect(new Segment(((float3)(ref position4)).xz, ((float3)(ref position5)).xz), ((Line3)(ref val4)).xz, ref val6))
				{
					val5 |= val6.y;
				}
				if (MathUtils.Intersect(new Segment(((float3)(ref position5)).xz, ((float3)(ref position6)).xz), ((Line3)(ref val4)).xz, ref val6))
				{
					val5 |= val6.y;
				}
				if (MathUtils.Intersect(new Segment(((float3)(ref position6)).xz, ((float3)(ref position7)).xz), ((Line3)(ref val4)).xz, ref val6))
				{
					val5 |= val6.y;
				}
				if (MathUtils.Intersect(new Segment(((float3)(ref position7)).xz, ((float3)(ref position4)).xz), ((Line3)(ref val4)).xz, ref val6))
				{
					val5 |= val6.y;
				}
			}
			val5.min = math.clamp(val5.min + m_Size, m_TargetDistance * -0.9f, 0f);
			val5.max = math.clamp(val5.max - m_Size, 0f, m_TargetDistance * 0.9f);
			m_TargetLine.a = MathUtils.Position(val4, val5.min);
			m_TargetLine.b = MathUtils.Position(val4, val5.max);
		}
		m_Bounds = MathUtils.Expand(MathUtils.Bounds(m_TargetLine) | m_CurrentPosition, float3.op_Implicit(m_Size));
	}

	private void CalculateTargetLine(Entity targetLane, float targetOffset)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		Curve curve = m_CurveData[targetLane];
		PrefabRef prefabRef = m_PrefabRefData[targetLane];
		float num = math.max(0f, m_PrefabLaneData[prefabRef.m_Prefab].m_Width * 0.5f - m_Size);
		float3 val = MathUtils.Position(curve.m_Bezier, targetOffset);
		float3 val2 = MathUtils.Tangent(curve.m_Bezier, targetOffset);
		float2 val3 = MathUtils.Right(math.normalizesafe(((float3)(ref val2)).xz, default(float2))) * num;
		m_TargetLine = new Segment(val, val);
		ref float3 a = ref m_TargetLine.a;
		((float3)(ref a)).xz = ((float3)(ref a)).xz - val3;
		ref float3 b = ref m_TargetLine.b;
		((float3)(ref b)).xz = ((float3)(ref b)).xz + val3;
		float num3 = default(float);
		float num2 = MathUtils.Distance(m_TargetLine, m_CurrentPosition, ref num3);
		if (num2 > m_TargetDistance)
		{
			m_TargetLine += (m_CurrentPosition - MathUtils.Position(m_TargetLine, num3)) * (1f - m_TargetDistance / num2);
		}
		m_Bounds = MathUtils.Expand(MathUtils.Bounds(m_TargetLine) | m_CurrentPosition, float3.op_Implicit(m_Size));
	}

	private void CheckCollision(Entity other)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		//IL_0760: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = default(Transform);
		if (!m_TransformData.TryGetComponent(other, ref transform))
		{
			return;
		}
		PrefabRef prefabRef = m_PrefabRefData[other];
		ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
		if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.WalkThrough) != Game.Objects.GeometryFlags.None)
		{
			return;
		}
		Moving moving = default(Moving);
		if (m_MovingData.TryGetComponent(other, ref moving))
		{
			float num = (objectGeometryData.m_Bounds.max.x - objectGeometryData.m_Bounds.min.x) * 0.5f;
			float num2 = m_Size + num + 0.5f;
			Segment val = default(Segment);
			((Segment)(ref val))._002Ector(m_CurrentPosition, m_TargetPosition);
			Segment val2 = default(Segment);
			((Segment)(ref val2))._002Ector(transform.m_Position, transform.m_Position + moving.m_Velocity);
			if (math.dot(val2.a + moving.m_Velocity * (m_TimeStep * 2f) - val.a - m_CurrentVelocity * m_TimeStep, m_TargetPosition - m_CurrentPosition) < 0f)
			{
				return;
			}
			float2 val3 = default(float2);
			float num3 = MathUtils.Distance(val, val2, ref val3);
			if (!(num3 < num2))
			{
				return;
			}
			float3 val4 = MathUtils.Position(val, val3.x * 0.99f);
			float3 val5 = MathUtils.Position(val2, val3.y);
			Bounds1 val6 = val4.y + ((Bounds3)(ref m_PrefabObjectGeometry.m_Bounds)).y;
			Bounds1 val7 = val5.y + ((Bounds3)(ref objectGeometryData.m_Bounds)).y;
			if (!MathUtils.Intersect(val6, val7))
			{
				return;
			}
			float3 val8 = math.normalizesafe(m_TargetPosition - m_CurrentPosition, default(float3));
			float3 val9 = val4 - val5;
			val9 -= val8 * math.dot(val9, val8);
			val9 = math.normalizesafe(val9, default(float3));
			float3 val10 = m_TargetPosition + val9 * ((num2 - num3) * m_PushFactor);
			m_PushFactor /= 2f;
			if (((float3)(ref m_TargetLine.a)).Equals(m_TargetLine.b))
			{
				m_TargetPosition = m_TargetLine.a;
			}
			else
			{
				MathUtils.Distance(m_TargetLine, val10, ref m_LanePosition);
				m_TargetPosition = MathUtils.Position(m_TargetLine, m_LanePosition);
				m_LanePosition -= 0.5f;
			}
			float num4 = math.min(1f, 0.7f + 0.3f * math.dot(val8, math.normalizesafe(moving.m_Velocity, default(float3))) + num3 / num2);
			num4 *= m_SpeedRange.max;
			val9 = transform.m_Position - m_CurrentPosition;
			float num5 = math.length(val9);
			float num6 = math.dot(val9, val8);
			Entity queueEntity = Entity.Null;
			Sphere3 queueArea = default(Sphere3);
			BlockerType blockerType = BlockerType.Crossing;
			if (num5 < num2 && num6 > 0f)
			{
				blockerType = BlockerType.Continuing;
				if (CheckQueue(other, out queueEntity, out queueArea))
				{
					if (num5 > 0.01f)
					{
						float num7 = num6 * (num2 - num5) / (num2 * num5);
						num4 = math.min(num4, math.max(0f, math.max(1f, math.lerp(math.dot(val8, moving.m_Velocity), m_SpeedRange.max, num3 / num2)) - num7));
					}
					else
					{
						num4 = 0f;
					}
				}
				else if (num5 > 0.01f && ((objectGeometryData.m_Flags & ~m_PrefabObjectGeometry.m_Flags & Game.Objects.GeometryFlags.LowCollisionPriority) == 0 || math.dot(moving.m_Velocity, val9) < 0f))
				{
					float num8 = num6 * (num2 - num5) / (num2 * num5);
					num4 = math.min(num4, math.max(m_MinSpeed, math.max(1f, math.lerp(math.dot(val8, moving.m_Velocity), m_SpeedRange.max, num3 / num2)) - num8));
				}
			}
			num4 = MathUtils.Clamp(num4, m_SpeedRange);
			if (num4 < m_MaxSpeed)
			{
				m_MaxSpeed = num4;
				m_Blocker = other;
				m_BlockerType = blockerType;
				CreatureUtils.SetQueue(ref m_QueueEntity, ref m_QueueArea, queueEntity, queueArea);
			}
			return;
		}
		float num9 = (((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Standing) == 0) ? (math.cmax(((float3)(ref objectGeometryData.m_Bounds.max)).xz - ((float3)(ref objectGeometryData.m_Bounds.min)).xz) * 0.5f) : math.cmax(((float3)(ref objectGeometryData.m_LegSize)).xz + objectGeometryData.m_LegOffset * 2f));
		float num10 = m_Size + num9 + 0.25f;
		Segment val11 = default(Segment);
		((Segment)(ref val11))._002Ector(m_CurrentPosition, m_TargetPosition);
		float num12 = default(float);
		float num11 = MathUtils.Distance(val11, transform.m_Position, ref num12);
		if (!(num11 < num10))
		{
			return;
		}
		float3 val12 = MathUtils.Position(val11, num12 * 0.99f);
		Bounds1 val13 = val12.y + ((Bounds3)(ref m_PrefabObjectGeometry.m_Bounds)).y;
		Bounds1 val14 = transform.m_Position.y + ((Bounds3)(ref objectGeometryData.m_Bounds)).y;
		if (MathUtils.Intersect(val13, val14))
		{
			float3 val15 = math.normalizesafe(m_TargetPosition - m_CurrentPosition, default(float3));
			float3 val16 = val12 - transform.m_Position;
			val16 -= val15 * math.dot(val16, val15);
			val16 = math.normalizesafe(val16, default(float3));
			float3 val17 = m_TargetPosition + val16 * ((num10 - num11) * m_PushFactor);
			m_PushFactor /= 2f;
			if (((float3)(ref m_TargetLine.a)).Equals(m_TargetLine.b))
			{
				m_TargetPosition = m_TargetLine.a;
			}
			else
			{
				MathUtils.Distance(m_TargetLine, val17, ref m_LanePosition);
				m_TargetPosition = MathUtils.Position(m_TargetLine, m_LanePosition);
				m_LanePosition -= 0.5f;
			}
			float num13 = math.min(1f, 0.7f + num11 / num10);
			num13 *= m_SpeedRange.max;
			val16 = transform.m_Position - m_CurrentPosition;
			float num14 = math.length(val16);
			float num15 = math.dot(val16, val15);
			if (num14 < num10 && num15 > 0f && num14 > 0.01f)
			{
				float num16 = num15 * (num10 - num14) / (num10 * num14);
				num13 = math.min(num13, math.max(0.5f, math.max(1f, m_SpeedRange.max * num11 / num10) - num16));
			}
			num13 = MathUtils.Clamp(num13, m_SpeedRange);
			if (num13 < m_MaxSpeed)
			{
				m_MaxSpeed = num13;
				m_Blocker = other;
				m_BlockerType = BlockerType.Limit;
				m_QueueEntity = Entity.Null;
				m_QueueArea = default(Sphere3);
			}
		}
	}

	private bool CheckQueue(Entity other, out Entity queueEntity, out Sphere3 queueArea)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		queueEntity = Entity.Null;
		queueArea = default(Sphere3);
		Creature creature = default(Creature);
		if (m_CreatureData.TryGetComponent(other, ref creature) && creature.m_QueueArea.radius > 0f)
		{
			Transform transform = m_TransformData[other];
			float3 val = math.forward(transform.m_Rotation);
			if (math.dot(transform.m_Position - m_CurrentPosition, m_CurrentDirection) < math.dot(m_CurrentPosition - transform.m_Position, val))
			{
				return false;
			}
			if (m_Leader != Entity.Null)
			{
				GroupMember groupMember = default(GroupMember);
				if (m_GroupMemberData.TryGetComponent(other, ref groupMember))
				{
					other = groupMember.m_Leader;
				}
				if (other == m_Leader)
				{
					queueEntity = creature.m_QueueEntity;
					queueArea = creature.m_QueueArea;
					return true;
				}
			}
			else
			{
				GroupMember groupMember2 = default(GroupMember);
				if (m_GroupMemberData.TryGetComponent(other, ref groupMember2))
				{
					other = groupMember2.m_Leader;
				}
				if (other != m_Entity && ShouldQueue(creature.m_QueueEntity, creature.m_QueueArea, out queueArea))
				{
					queueEntity = creature.m_QueueEntity;
					return true;
				}
			}
		}
		return false;
	}

	private bool ShouldQueue(Entity entity, Sphere3 area, out Sphere3 queueArea)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		if (!m_Queues.IsCreated || (m_PathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Obsolete | PathFlags.Updated)) != 0)
		{
			queueArea = default(Sphere3);
			return false;
		}
		Entity val = Entity.Null;
		if (m_PathElements.Length > m_PathOwner.m_ElementIndex)
		{
			PathElement pathElement = m_PathElements[m_PathOwner.m_ElementIndex];
			if (m_WaypointData.HasComponent(pathElement.m_Target) || m_TaxiStandData.HasComponent(pathElement.m_Target))
			{
				val = pathElement.m_Target;
			}
		}
		Curve curve = default(Curve);
		for (int i = 0; i < m_Queues.Length; i++)
		{
			Queue queue = m_Queues[i];
			if (queue.m_TargetEntity == entity)
			{
				if ((queue.m_TargetEntity == val || queue.m_TargetEntity == m_CurrentLane) && m_CurveData.TryGetComponent(m_CurrentLane, ref curve))
				{
					PrefabRef prefabRef = m_PrefabRefData[m_CurrentLane];
					NetLaneData prefabLaneData = m_PrefabLaneData[prefabRef.m_Prefab];
					float laneOffset = CreatureUtils.GetLaneOffset(m_PrefabObjectGeometry, prefabLaneData, m_LanePosition);
					queue.m_TargetArea.position = CreatureUtils.GetLanePosition(curve.m_Bezier, m_CurvePosition, laneOffset);
				}
				queue.m_ObsoleteTime = 0;
				m_Queues[i] = queue;
				if (queue.m_TargetArea.radius > 0f && MathUtils.Intersect(queue.m_TargetArea, area))
				{
					Sphere3 queueArea2 = CreatureUtils.GetQueueArea(m_PrefabObjectGeometry, m_CurrentPosition, m_TargetPosition);
					queueArea = MathUtils.Sphere(area, MathUtils.Sphere(queueArea2, queue.m_TargetArea));
					return true;
				}
				queueArea = default(Sphere3);
				return false;
			}
		}
		if (m_CurrentLane == entity)
		{
			Queue queue2 = default(Queue);
			queue2.m_TargetEntity = entity;
			queue2.m_TargetArea = CreatureUtils.GetQueueArea(m_PrefabObjectGeometry, GetTargetPosition(m_PathOwner.m_ElementIndex - 1, m_CurrentLane, m_CurvePosition));
			queue2.m_ObsoleteTime = 0;
			m_Queues.Add(queue2);
			if (MathUtils.Intersect(queue2.m_TargetArea, area))
			{
				Sphere3 queueArea3 = CreatureUtils.GetQueueArea(m_PrefabObjectGeometry, m_CurrentPosition, m_TargetPosition);
				queueArea = MathUtils.Sphere(area, MathUtils.Sphere(queueArea3, queue2.m_TargetArea));
				return true;
			}
			queueArea = default(Sphere3);
			return false;
		}
		if (m_CurrentVehicle == Entity.Null)
		{
			Queue queue3 = default(Queue);
			for (int j = m_PathOwner.m_ElementIndex; j < m_PathElements.Length; j++)
			{
				PathElement pathElement2 = m_PathElements[j];
				if (pathElement2.m_Target == entity)
				{
					queue3.m_TargetEntity = entity;
					queue3.m_TargetArea = CreatureUtils.GetQueueArea(m_PrefabObjectGeometry, GetTargetPosition(j, pathElement2.m_Target, pathElement2.m_TargetDelta.y));
					queue3.m_ObsoleteTime = 0;
					m_Queues.Add(queue3);
					if (MathUtils.Intersect(queue3.m_TargetArea, area))
					{
						Sphere3 queueArea4 = CreatureUtils.GetQueueArea(m_PrefabObjectGeometry, m_CurrentPosition, m_TargetPosition);
						queueArea = MathUtils.Sphere(area, MathUtils.Sphere(queueArea4, queue3.m_TargetArea));
						return true;
					}
					queueArea = default(Sphere3);
					return false;
				}
			}
		}
		m_Queues.Add(new Queue
		{
			m_TargetEntity = entity
		});
		queueArea = default(Sphere3);
		return false;
	}

	private float3 GetTargetPosition(int elementIndex, Entity targetElement, float curvePos)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		while (m_WaypointData.HasComponent(targetElement) || m_TaxiStandData.HasComponent(targetElement))
		{
			if (--elementIndex >= m_PathOwner.m_ElementIndex)
			{
				PathElement pathElement = m_PathElements[elementIndex];
				targetElement = pathElement.m_Target;
				curvePos = pathElement.m_TargetDelta.y;
				continue;
			}
			targetElement = m_CurrentLane;
			curvePos = m_CurvePosition;
			break;
		}
		Curve curve = default(Curve);
		if (m_CurveData.TryGetComponent(targetElement, ref curve))
		{
			PrefabRef prefabRef = m_PrefabRefData[targetElement];
			NetLaneData prefabLaneData = m_PrefabLaneData[prefabRef.m_Prefab];
			float laneOffset = CreatureUtils.GetLaneOffset(m_PrefabObjectGeometry, prefabLaneData, m_LanePosition);
			return CreatureUtils.GetLanePosition(curve.m_Bezier, curvePos, laneOffset);
		}
		Transform transform = default(Transform);
		if (m_TransformData.TryGetComponent(targetElement, ref transform))
		{
			return transform.m_Position;
		}
		return m_TargetPosition;
	}

	public void IterateBlocker(HumanData prefabHumanData, Entity other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		Moving moving = default(Moving);
		if (CheckQueue(other, out var queueEntity, out var queueArea) && m_MovingData.TryGetComponent(other, ref moving))
		{
			Transform transform = m_TransformData[other];
			PrefabRef prefabRef = m_PrefabRefData[other];
			ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
			float num = (m_PrefabObjectGeometry.m_Bounds.max.x - m_PrefabObjectGeometry.m_Bounds.min.x) * 0.5f;
			float num2 = (objectGeometryData.m_Bounds.max.x - objectGeometryData.m_Bounds.min.x) * 0.5f;
			float num3 = num + num2 + 0.5f;
			float3 val = transform.m_Position - m_CurrentPosition;
			float3 val2 = math.normalizesafe(m_TargetPosition - m_CurrentPosition, default(float3));
			float distance = math.max(0f, math.length(val) * 2f - num3 - math.dot(val, val2));
			float maxResultSpeed = math.max(0f, math.dot(val2, moving.m_Velocity));
			float maxBrakingSpeed = CreatureUtils.GetMaxBrakingSpeed(prefabHumanData, distance, maxResultSpeed, m_TimeStep);
			maxBrakingSpeed = MathUtils.Clamp(maxBrakingSpeed, m_SpeedRange);
			if (maxBrakingSpeed <= m_MaxSpeed)
			{
				m_MaxSpeed = maxBrakingSpeed;
				m_Blocker = other;
				m_BlockerType = BlockerType.Continuing;
				CreatureUtils.SetQueue(ref m_QueueEntity, ref m_QueueArea, queueEntity, queueArea);
			}
		}
	}

	public void IterateBlocker(AnimalData prefabAnimalData, Entity other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		Moving moving = default(Moving);
		if (CheckQueue(other, out var queueEntity, out var queueArea) && m_MovingData.TryGetComponent(other, ref moving))
		{
			Transform transform = m_TransformData[other];
			PrefabRef prefabRef = m_PrefabRefData[other];
			ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
			float num = (m_PrefabObjectGeometry.m_Bounds.max.x - m_PrefabObjectGeometry.m_Bounds.min.x) * 0.5f;
			float num2 = (objectGeometryData.m_Bounds.max.x - objectGeometryData.m_Bounds.min.x) * 0.5f;
			float num3 = num + num2 + 0.5f;
			float3 val = transform.m_Position - m_CurrentPosition;
			float3 val2 = math.normalizesafe(m_TargetPosition - m_CurrentPosition, default(float3));
			float distance = math.max(0f, math.length(val) * 2f - num3 - math.dot(val, val2));
			float maxResultSpeed = math.max(0f, math.dot(val2, moving.m_Velocity));
			float maxBrakingSpeed = CreatureUtils.GetMaxBrakingSpeed(prefabAnimalData, distance, maxResultSpeed, m_TimeStep);
			maxBrakingSpeed = MathUtils.Clamp(maxBrakingSpeed, m_SpeedRange);
			if (maxBrakingSpeed <= m_MaxSpeed)
			{
				m_MaxSpeed = maxBrakingSpeed;
				m_Blocker = other;
				m_BlockerType = BlockerType.Continuing;
				CreatureUtils.SetQueue(ref m_QueueEntity, ref m_QueueArea, queueEntity, queueArea);
			}
		}
	}
}
