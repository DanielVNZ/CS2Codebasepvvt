using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public struct VehicleCollisionIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
{
	public ComponentLookup<Owner> m_OwnerData;

	public ComponentLookup<Transform> m_TransformData;

	public ComponentLookup<Moving> m_MovingData;

	public ComponentLookup<Controller> m_ControllerData;

	public ComponentLookup<Creature> m_CreatureData;

	public ComponentLookup<Curve> m_CurveData;

	public ComponentLookup<AreaLane> m_AreaLaneData;

	public ComponentLookup<PrefabRef> m_PrefabRefData;

	public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

	public ComponentLookup<NetLaneData> m_PrefabLaneData;

	public BufferLookup<Game.Areas.Node> m_AreaNodes;

	public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_StaticObjectSearchTree;

	public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_MovingObjectSearchTree;

	public TerrainHeightData m_TerrainHeightData;

	public Entity m_Entity;

	public Entity m_CurrentLane;

	public float m_CurvePosition;

	public float m_TimeStep;

	public ObjectGeometryData m_PrefabObjectGeometry;

	public Bounds1 m_SpeedRange;

	public float3 m_CurrentPosition;

	public float3 m_CurrentVelocity;

	public float m_MinDistance;

	public float3 m_TargetPosition;

	public float m_MaxSpeed;

	public float m_LanePosition;

	public Entity m_Blocker;

	public BlockerType m_BlockerType;

	private Segment m_TargetLine;

	private Bounds1 m_TargetLimits;

	private float m_PushFactor;

	private Bounds3 m_Bounds;

	private float m_Size;

	public bool IterateFirstLane(Entity currentLane)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		m_Size = (m_PrefabObjectGeometry.m_Bounds.max.x - m_PrefabObjectGeometry.m_Bounds.min.x) * 0.5f;
		m_PushFactor = 0.75f;
		if (m_AreaLaneData.HasComponent(currentLane))
		{
			CalculateTargetLine(currentLane, m_TargetPosition);
			m_MovingObjectSearchTree.Iterate<VehicleCollisionIterator>(ref this, 0);
			m_StaticObjectSearchTree.Iterate<VehicleCollisionIterator>(ref this, 0);
			return false;
		}
		return false;
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

	private void CalculateTargetLine(Entity targetLane, float3 targetPosition)
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
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		Owner owner = m_OwnerData[targetLane];
		AreaLane areaLane = m_AreaLaneData[targetLane];
		DynamicBuffer<Game.Areas.Node> val = m_AreaNodes[owner.m_Owner];
		float3 val2 = targetPosition - m_CurrentPosition;
		float num = math.length(((float3)(ref val2)).xz);
		if (num < m_MinDistance)
		{
			m_TargetLine = new Segment(targetPosition, targetPosition);
			m_TargetLimits = new Bounds1(0f, 1f);
		}
		else
		{
			if (num > m_MinDistance)
			{
				targetPosition = m_CurrentPosition + val2 * (m_MinDistance / num);
			}
			float2 val3 = MathUtils.Right(((float3)(ref val2)).xz) * (0.5f / math.max(0.1f, num));
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
			m_TargetLimits.min = math.min(val5.min + m_Size, 0f);
			m_TargetLimits.max = math.max(val5.max - m_Size, 0f);
			val5.min = math.max(m_TargetLimits.min, m_MinDistance * -0.9f);
			val5.max = math.min(m_TargetLimits.max, m_MinDistance * 0.9f);
			m_TargetLine.a = MathUtils.Position(val4, val5.min);
			m_TargetLine.b = MathUtils.Position(val4, val5.max);
			float num2 = 1f / math.max(1f, m_TargetLimits.max - m_TargetLimits.min);
			m_TargetLimits.min = (val5.min - m_TargetLimits.min) * num2;
			m_TargetLimits.max = (val5.max - m_TargetLimits.min) * num2;
		}
		m_Bounds = MathUtils.Expand(MathUtils.Bounds(m_TargetLine) | m_CurrentPosition, float3.op_Implicit(m_Size));
	}

	private void CheckCollision(Entity other)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_061f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = default(Transform);
		if (other == m_Entity || !m_TransformData.TryGetComponent(other, ref transform))
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
			Controller controller = default(Controller);
			if (m_CreatureData.HasComponent(other) || (m_ControllerData.TryGetComponent(other, ref controller) && controller.m_Controller == m_Entity))
			{
				return;
			}
			float num = (objectGeometryData.m_Bounds.max.x - objectGeometryData.m_Bounds.min.x) * 0.5f;
			float num2 = m_Size + num + 0.5f;
			Segment val = default(Segment);
			((Segment)(ref val))._002Ector(((float3)(ref m_CurrentPosition)).xz, ((float3)(ref m_TargetPosition)).xz);
			Segment val2 = default(Segment);
			((Segment)(ref val2))._002Ector(((float3)(ref transform.m_Position)).xz, ((float3)(ref transform.m_Position)).xz + ((float3)(ref moving.m_Velocity)).xz);
			if (math.dot(val2.a + ((float3)(ref moving.m_Velocity)).xz * (m_TimeStep * 2f) - val.a - ((float3)(ref m_CurrentVelocity)).xz * m_TimeStep, ((float3)(ref m_TargetPosition)).xz - ((float3)(ref m_CurrentPosition)).xz) < 0f)
			{
				return;
			}
			float2 val3 = default(float2);
			float num3 = MathUtils.Distance(val, val2, ref val3);
			if (!(num3 < num2))
			{
				return;
			}
			float2 val4 = MathUtils.Position(val, val3.x * 0.99f);
			float2 val5 = MathUtils.Position(val2, val3.y);
			float2 val6 = math.normalizesafe(((float3)(ref m_TargetPosition)).xz - ((float3)(ref m_CurrentPosition)).xz, default(float2));
			float2 val7 = val4 - val5;
			val7 -= val6 * math.dot(val7, val6);
			val7 = math.normalizesafe(val7, default(float2));
			float2 val8 = ((float3)(ref m_TargetPosition)).xz + val7 * ((num2 - num3) * m_PushFactor);
			m_PushFactor /= 2f;
			if (((float3)(ref m_TargetLine.a)).Equals(m_TargetLine.b))
			{
				m_TargetPosition = m_TargetLine.a;
			}
			else
			{
				MathUtils.Distance(((Segment)(ref m_TargetLine)).xz, val8, ref m_LanePosition);
				m_TargetPosition = MathUtils.Position(m_TargetLine, m_LanePosition);
				m_LanePosition = m_TargetLimits.min + m_LanePosition * (m_TargetLimits.max - m_TargetLimits.min) - 0.5f;
			}
			if (m_TerrainHeightData.isCreated)
			{
				m_TargetPosition.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, m_TargetPosition);
			}
			float num4 = math.min(1f, 0.7f + 0.3f * math.dot(val6, math.normalizesafe(((float3)(ref moving.m_Velocity)).xz, default(float2))) + num3 / num2);
			num4 *= m_SpeedRange.max;
			val7 = ((float3)(ref transform.m_Position)).xz - ((float3)(ref m_CurrentPosition)).xz;
			float num5 = math.length(val7);
			float num6 = math.dot(val7, val6);
			BlockerType blockerType = BlockerType.Crossing;
			if (num5 < num2 && num6 > 0f)
			{
				blockerType = BlockerType.Continuing;
				if (num5 > 0.01f)
				{
					float num7 = num6 * (num2 - num5) / (num2 * num5);
					num4 = math.min(num4, math.max(0f, math.max(1f, math.lerp(math.dot(val6, ((float3)(ref moving.m_Velocity)).xz), m_SpeedRange.max, num3 / num2)) - num7));
				}
				else
				{
					num4 = 0f;
				}
			}
			num4 = MathUtils.Clamp(num4, m_SpeedRange);
			if (num4 < m_MaxSpeed)
			{
				m_MaxSpeed = num4;
				m_Blocker = other;
				m_BlockerType = blockerType;
			}
			return;
		}
		float num8 = (((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Standing) == 0) ? (math.cmax(((float3)(ref objectGeometryData.m_Bounds.max)).xz - ((float3)(ref objectGeometryData.m_Bounds.min)).xz) * 0.5f) : math.cmax(((float3)(ref objectGeometryData.m_LegSize)).xz + objectGeometryData.m_LegOffset * 2f));
		float num9 = m_Size + num8 + 0.25f;
		Segment val9 = default(Segment);
		((Segment)(ref val9))._002Ector(((float3)(ref m_CurrentPosition)).xz, ((float3)(ref m_TargetPosition)).xz);
		float num11 = default(float);
		float num10 = MathUtils.Distance(val9, ((float3)(ref transform.m_Position)).xz, ref num11);
		if (num10 < num9)
		{
			float2 val10 = MathUtils.Position(val9, num11 * 0.99f);
			float2 val11 = math.normalizesafe(((float3)(ref m_TargetPosition)).xz - ((float3)(ref m_CurrentPosition)).xz, default(float2));
			float2 val12 = val10 - ((float3)(ref transform.m_Position)).xz;
			val12 -= val11 * math.dot(val12, val11);
			val12 = math.normalizesafe(val12, default(float2));
			float2 val13 = ((float3)(ref m_TargetPosition)).xz + val12 * ((num9 - num10) * m_PushFactor);
			m_PushFactor /= 2f;
			if (((float3)(ref m_TargetLine.a)).Equals(m_TargetLine.b))
			{
				m_TargetPosition = m_TargetLine.a;
			}
			else
			{
				MathUtils.Distance(((Segment)(ref m_TargetLine)).xz, val13, ref m_LanePosition);
				m_TargetPosition = MathUtils.Position(m_TargetLine, m_LanePosition);
				m_LanePosition = m_TargetLimits.min + m_LanePosition * (m_TargetLimits.max - m_TargetLimits.min) - 0.5f;
			}
			if (m_TerrainHeightData.isCreated)
			{
				m_TargetPosition.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, m_TargetPosition);
			}
		}
	}
}
