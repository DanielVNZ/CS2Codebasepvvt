using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Routes;

public static class RaycastJobs
{
	[BurstCompile]
	public struct FindRoutesFromTreeJob : IJob
	{
		private struct FindRoutesIterator : INativeQuadTreeIterator<RouteSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<RouteSearchItem, QuadTreeBoundsXZ>
		{
			public int m_RaycastIndex;

			public Segment m_Line;

			public NativeList<RouteItem> m_RouteList;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				float2 val = default(float2);
				return MathUtils.Intersect(bounds.m_Bounds, m_Line, ref val);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, RouteSearchItem item)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				float2 val = default(float2);
				if (MathUtils.Intersect(bounds.m_Bounds, m_Line, ref val))
				{
					ref NativeList<RouteItem> routeList = ref m_RouteList;
					RouteItem routeItem = new RouteItem
					{
						m_Entity = item.m_Entity,
						m_Element = item.m_Element,
						m_RaycastIndex = m_RaycastIndex
					};
					routeList.Add(ref routeItem);
				}
			}
		}

		[ReadOnly]
		public NativeArray<RaycastInput> m_Input;

		[ReadOnly]
		public NativeQuadTree<RouteSearchItem, QuadTreeBoundsXZ> m_SearchTree;

		[WriteOnly]
		public NativeList<RouteItem> m_RouteList;

		public void Execute()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_Input.Length; i++)
			{
				RaycastInput raycastInput = m_Input[i];
				if ((raycastInput.m_TypeMask & (TypeMask.RouteWaypoints | TypeMask.RouteSegments)) != TypeMask.None)
				{
					FindRoutesIterator findRoutesIterator = new FindRoutesIterator
					{
						m_RaycastIndex = i,
						m_Line = raycastInput.m_Line,
						m_RouteList = m_RouteList
					};
					m_SearchTree.Iterate<FindRoutesIterator>(ref findRoutesIterator, 0);
				}
			}
		}
	}

	public struct RouteItem
	{
		public Entity m_Entity;

		public int m_Element;

		public int m_RaycastIndex;
	}

	[BurstCompile]
	public struct RaycastRoutesJob : IJobParallelForDefer
	{
		[ReadOnly]
		public NativeArray<RaycastInput> m_Input;

		[ReadOnly]
		public NativeArray<RouteItem> m_Routes;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<RouteData> m_PrefabRouteData;

		[ReadOnly]
		public ComponentLookup<TransportLineData> m_PrefabTransportLineData;

		[ReadOnly]
		public ComponentLookup<Waypoint> m_WaypointData;

		[ReadOnly]
		public ComponentLookup<Segment> m_SegmentData;

		[ReadOnly]
		public ComponentLookup<Position> m_PositionData;

		[ReadOnly]
		public ComponentLookup<HiddenRoute> m_HiddenRouteData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public BufferLookup<CurveElement> m_CurveElements;

		[NativeDisableContainerSafetyRestriction]
		public ParallelWriter<RaycastResult> m_Results;

		public void Execute(int index)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			RouteItem routeItem = m_Routes[index];
			RaycastInput raycastInput = m_Input[routeItem.m_RaycastIndex];
			Waypoint waypoint = default(Waypoint);
			if ((raycastInput.m_TypeMask & TypeMask.RouteWaypoints) != TypeMask.None && m_WaypointData.TryGetComponent(routeItem.m_Entity, ref waypoint))
			{
				PrefabRef prefabRef = m_PrefabRefData[routeItem.m_Entity];
				Position position = m_PositionData[routeItem.m_Entity];
				Owner owner = m_OwnerData[routeItem.m_Entity];
				if (m_HiddenRouteData.HasComponent(owner.m_Owner))
				{
					return;
				}
				RouteData routeData = m_PrefabRouteData[prefabRef.m_Prefab];
				TransportLineData transportLineData = default(TransportLineData);
				if ((raycastInput.m_RouteType != RouteType.None && raycastInput.m_RouteType != routeData.m_Type) || (m_PrefabTransportLineData.TryGetComponent(prefabRef.m_Prefab, ref transportLineData) && ((raycastInput.m_TransportType != TransportType.None && raycastInput.m_TransportType != transportLineData.m_TransportType) || ((raycastInput.m_Flags & (RaycastFlags.Cargo | RaycastFlags.Passenger)) != 0 && ((raycastInput.m_Flags & RaycastFlags.Passenger) == 0 || !transportLineData.m_PassengerTransport) && ((raycastInput.m_Flags & RaycastFlags.Cargo) == 0 || !transportLineData.m_CargoTransport)))))
				{
					return;
				}
				float num2 = default(float);
				float num = MathUtils.Distance(raycastInput.m_Line, position.m_Position, ref num2) - routeData.m_SnapDistance;
				if (num < 0f)
				{
					RaycastResult raycastResult = default(RaycastResult);
					raycastResult.m_Owner = owner.m_Owner;
					raycastResult.m_Hit.m_HitEntity = raycastResult.m_Owner;
					raycastResult.m_Hit.m_Position = position.m_Position;
					raycastResult.m_Hit.m_HitPosition = MathUtils.Position(raycastInput.m_Line, num2);
					raycastResult.m_Hit.m_NormalizedDistance = num2;
					raycastResult.m_Hit.m_CellIndex = new int2(waypoint.m_Index, -1);
					raycastResult.m_Hit.m_NormalizedDistance -= 100f / math.max(1f, MathUtils.Length(raycastInput.m_Line));
					raycastResult.m_Hit.m_NormalizedDistance += num * 1E-06f / math.max(1f, routeData.m_SnapDistance);
					m_Results.Accumulate(routeItem.m_RaycastIndex, raycastResult);
				}
			}
			Segment segment = default(Segment);
			if ((raycastInput.m_TypeMask & TypeMask.RouteSegments) == 0 || !m_SegmentData.TryGetComponent(routeItem.m_Entity, ref segment))
			{
				return;
			}
			PrefabRef prefabRef2 = m_PrefabRefData[routeItem.m_Entity];
			DynamicBuffer<CurveElement> val = m_CurveElements[routeItem.m_Entity];
			Owner owner2 = m_OwnerData[routeItem.m_Entity];
			if (m_HiddenRouteData.HasComponent(owner2.m_Owner))
			{
				return;
			}
			RouteData routeData2 = m_PrefabRouteData[prefabRef2.m_Prefab];
			TransportLineData transportLineData2 = default(TransportLineData);
			if ((raycastInput.m_RouteType == RouteType.None || raycastInput.m_RouteType == routeData2.m_Type) && (!m_PrefabTransportLineData.TryGetComponent(prefabRef2.m_Prefab, ref transportLineData2) || ((raycastInput.m_TransportType == TransportType.None || raycastInput.m_TransportType == transportLineData2.m_TransportType) && ((raycastInput.m_Flags & (RaycastFlags.Cargo | RaycastFlags.Passenger)) == 0 || ((raycastInput.m_Flags & RaycastFlags.Passenger) != 0 && transportLineData2.m_PassengerTransport) || ((raycastInput.m_Flags & RaycastFlags.Cargo) != 0 && transportLineData2.m_CargoTransport)))) && val.Length > routeItem.m_Element)
			{
				CurveElement curveElement = val[routeItem.m_Element];
				float2 val2 = default(float2);
				float num3 = MathUtils.Distance(curveElement.m_Curve, raycastInput.m_Line, ref val2) - routeData2.m_SnapDistance * 0.5f;
				if (num3 < 0f)
				{
					RaycastResult raycastResult2 = default(RaycastResult);
					raycastResult2.m_Owner = owner2.m_Owner;
					raycastResult2.m_Hit.m_HitEntity = raycastResult2.m_Owner;
					raycastResult2.m_Hit.m_Position = MathUtils.Position(curveElement.m_Curve, val2.x);
					raycastResult2.m_Hit.m_HitPosition = MathUtils.Position(raycastInput.m_Line, val2.y);
					raycastResult2.m_Hit.m_NormalizedDistance = val2.y;
					raycastResult2.m_Hit.m_CellIndex = new int2(-1, segment.m_Index);
					raycastResult2.m_Hit.m_NormalizedDistance -= 100f / math.max(1f, MathUtils.Length(raycastInput.m_Line));
					raycastResult2.m_Hit.m_NormalizedDistance += num3 * 1E-06f / math.max(1f, routeData2.m_SnapDistance);
					m_Results.Accumulate(routeItem.m_RaycastIndex, raycastResult2);
				}
			}
		}
	}
}
