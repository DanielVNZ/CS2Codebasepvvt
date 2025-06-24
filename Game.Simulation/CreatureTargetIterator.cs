using Colossal.Mathematics;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public struct CreatureTargetIterator
{
	public ComponentLookup<Moving> m_MovingData;

	public ComponentLookup<Curve> m_CurveData;

	public ComponentLookup<LaneReservation> m_LaneReservationData;

	public BufferLookup<LaneOverlap> m_LaneOverlaps;

	public BufferLookup<LaneObject> m_LaneObjects;

	public ObjectGeometryData m_PrefabObjectGeometry;

	public Entity m_Blocker;

	public BlockerType m_BlockerType;

	public Entity m_QueueEntity;

	public Sphere3 m_QueueArea;

	private float m_TargetDelta;

	public bool IterateLane(Entity currentLane, ref float curveDelta, float targetDelta)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		m_TargetDelta = targetDelta;
		DynamicBuffer<LaneOverlap> val = default(DynamicBuffer<LaneOverlap>);
		if (m_LaneOverlaps.TryGetBuffer(currentLane, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				LaneOverlap laneOverlap = val[i];
				if ((laneOverlap.m_Flags & (OverlapFlags.MergeStart | OverlapFlags.MergeEnd | OverlapFlags.MergeMiddleStart | OverlapFlags.MergeMiddleEnd)) == 0 && (laneOverlap.m_Flags & (OverlapFlags.Road | OverlapFlags.Track)) != 0)
				{
					float4 val2 = new float4((float)(int)laneOverlap.m_ThisStart, (float)(int)laneOverlap.m_ThisEnd, (float)(int)laneOverlap.m_OtherStart, (float)(int)laneOverlap.m_OtherEnd) * 0.003921569f;
					if ((curveDelta <= val2.x) & (m_TargetDelta > val2.x))
					{
						CheckOverlapLane(currentLane, laneOverlap.m_Other, val2.x, targetDelta, ((float4)(ref val2)).zw);
					}
					else if ((curveDelta >= val2.y) & (m_TargetDelta < val2.y))
					{
						CheckOverlapLane(currentLane, laneOverlap.m_Other, val2.y, targetDelta, ((float4)(ref val2)).zw);
					}
				}
			}
		}
		curveDelta = m_TargetDelta;
		return m_TargetDelta == targetDelta;
	}

	private void CheckOverlapLane(Entity currentLane, Entity overlapLane, float limitDelta, float targetDelta, float2 overlapRange)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		LaneReservation laneReservation = default(LaneReservation);
		if (m_LaneReservationData.TryGetComponent(overlapLane, ref laneReservation))
		{
			float offset = laneReservation.GetOffset();
			int priority = laneReservation.GetPriority();
			if (offset > overlapRange.x || priority >= 108)
			{
				m_TargetDelta = limitDelta;
				m_Blocker = Entity.Null;
				m_BlockerType = BlockerType.Crossing;
				Curve curve = m_CurveData[currentLane];
				float3 position = MathUtils.Position(curve.m_Bezier, limitDelta);
				float3 position2 = math.select(curve.m_Bezier.a, curve.m_Bezier.d, targetDelta > limitDelta);
				m_QueueEntity = currentLane;
				m_QueueArea = CreatureUtils.GetQueueArea(m_PrefabObjectGeometry, position, position2);
				return;
			}
		}
		DynamicBuffer<LaneObject> val = default(DynamicBuffer<LaneObject>);
		if (!m_LaneObjects.TryGetBuffer(overlapLane, ref val))
		{
			return;
		}
		for (int i = 0; i < val.Length; i++)
		{
			LaneObject laneObject = val[i];
			float num = math.min(laneObject.m_CurvePosition.x, laneObject.m_CurvePosition.y);
			float num2 = math.max(laneObject.m_CurvePosition.x, laneObject.m_CurvePosition.y);
			if (((num <= overlapRange.y) & (num2 >= overlapRange.x)) && m_MovingData.HasComponent(laneObject.m_LaneObject))
			{
				m_TargetDelta = limitDelta;
				m_Blocker = laneObject.m_LaneObject;
				m_BlockerType = BlockerType.Crossing;
				Curve curve2 = m_CurveData[currentLane];
				float3 position3 = MathUtils.Position(curve2.m_Bezier, limitDelta);
				float3 position4 = math.select(curve2.m_Bezier.a, curve2.m_Bezier.d, targetDelta > limitDelta);
				m_QueueEntity = currentLane;
				m_QueueArea = CreatureUtils.GetQueueArea(m_PrefabObjectGeometry, position3, position4);
				break;
			}
		}
	}
}
