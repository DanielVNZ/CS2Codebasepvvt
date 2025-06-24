using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Citizens;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Pathfind;

public static class PathUtils
{
	private struct AppendPathValue
	{
		public float m_TargetDelta;

		public int m_Index;
	}

	public const float MIN_DENSITY = 0.01f;

	public static float CalculateCost(ref Random random, in PathSpecification pathSpecification, in PathfindParameters pathfindParameters)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		float num = CalculateSpeed(in pathSpecification, in pathfindParameters);
		float4 value = pathSpecification.m_Costs.m_Value;
		value.x += pathSpecification.m_Length / num;
		float num2 = math.dot(value, pathfindParameters.m_Weights.m_Value);
		return math.select(num2 * ((Random)(ref random)).NextFloat(0.5f, 1f), num2, (pathfindParameters.m_PathfindFlags & PathfindFlags.Stable) != 0);
	}

	public static float CalculateCost(in PathSpecification pathSpecification, in CoverageParameters coverageParameters, float2 delta)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return pathSpecification.m_Length * pathSpecification.m_Density * math.abs(delta.y - delta.x);
	}

	public static float CalculateCost(in PathSpecification pathSpecification, in AvailabilityParameters availabilityParameters, float2 delta)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		float num = math.lerp(1f, pathSpecification.m_Density, availabilityParameters.m_DensityWeight);
		return pathSpecification.m_Length * num * math.abs(delta.y - delta.x) * availabilityParameters.m_CostFactor / pathSpecification.m_MaxSpeed;
	}

	public static float CalculateLength(in PathSpecification pathSpecification, float2 delta)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return pathSpecification.m_Length * math.abs(delta.y - delta.x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float CalculateSpeed(in PathSpecification pathSpecification, in PathfindParameters pathfindParameters)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		PathMethod pathMethod = pathSpecification.m_Methods & pathfindParameters.m_Methods;
		float2 val = math.select(pathfindParameters.m_MaxSpeed, pathfindParameters.m_WalkSpeed, (pathMethod & PathMethod.Pedestrian) != 0);
		bool flag = (pathSpecification.m_Flags & EdgeFlags.Secondary) != 0;
		float num = math.min(math.select(val.x, val.y, flag), pathSpecification.m_MaxSpeed);
		num = math.select(pathSpecification.m_MaxSpeed, num, (pathMethod & (PathMethod.Pedestrian | PathMethod.Road | PathMethod.Track | PathMethod.Flying | PathMethod.MediumRoad)) != 0);
		return math.select(num - (float)(int)pathSpecification.m_FlowOffset * 0.00390625f * num, num, (pathfindParameters.m_PathfindFlags & PathfindFlags.IgnoreFlow) != 0);
	}

	public static void CombinePaths(DynamicBuffer<PathElement> sourceElements1, DynamicBuffer<PathElement> sourceElements2, DynamicBuffer<PathElement> targetElements)
	{
		targetElements.ResizeUninitialized(sourceElements1.Length + sourceElements2.Length);
		int num = 0;
		for (int i = 0; i < sourceElements1.Length; i++)
		{
			targetElements[num++] = sourceElements1[i];
		}
		for (int j = 0; j < sourceElements2.Length; j++)
		{
			targetElements[num++] = sourceElements2[j];
		}
	}

	public static PathInformation CombinePaths(PathInformation pathInformation1, PathInformation pathInformation2)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		pathInformation1.m_Distance += pathInformation2.m_Distance;
		pathInformation1.m_Duration += pathInformation2.m_Duration;
		pathInformation1.m_TotalCost += pathInformation2.m_TotalCost;
		pathInformation1.m_Destination = pathInformation2.m_Destination;
		return pathInformation1;
	}

	public static void CopyPath(DynamicBuffer<PathElement> sourceElements, PathOwner sourceOwner, int skipCount, DynamicBuffer<PathElement> targetElements)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		CopyPath(sourceElements, sourceOwner, skipCount, sourceElements.Length, targetElements);
	}

	public static void CopyPath(DynamicBuffer<PathElement> sourceElements, PathOwner sourceOwner, int skipCount, int endIndex, DynamicBuffer<PathElement> targetElements)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		endIndex = math.min(endIndex, sourceElements.Length);
		int num = sourceOwner.m_ElementIndex + skipCount;
		int num2 = endIndex - num;
		if (num2 > 0)
		{
			targetElements.ResizeUninitialized(num2);
			sourceElements.AsNativeArray().GetSubArray(num, num2).CopyTo(targetElements.AsNativeArray());
		}
		else
		{
			targetElements.Clear();
		}
	}

	public static void TrimPath(DynamicBuffer<PathElement> pathElements, ref PathOwner pathOwner)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		TrimPath(pathElements, ref pathOwner, pathOwner.m_ElementIndex);
	}

	public static void TrimPath(DynamicBuffer<PathElement> pathElements, ref PathOwner pathOwner, int startIndex)
	{
		if (startIndex > 0)
		{
			if (startIndex >= pathElements.Length)
			{
				pathElements.Clear();
			}
			else
			{
				pathElements.RemoveRange(0, startIndex);
			}
		}
		pathOwner.m_ElementIndex = 0;
	}

	public static int FindFirstLane(DynamicBuffer<PathElement> pathElements, PathOwner pathOwner, int skipCount, ComponentLookup<Game.Net.ParkingLane> parkingLaneData)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		for (int i = pathOwner.m_ElementIndex + skipCount; i < pathElements.Length; i++)
		{
			if (parkingLaneData.HasComponent(pathElements[i].m_Target))
			{
				return i;
			}
		}
		return pathElements.Length;
	}

	public static bool GetStartDirection(DynamicBuffer<PathElement> path, PathOwner pathOwner, ref ComponentLookup<Curve> curveData, out int startOffset, out bool forward)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		startOffset = 0;
		forward = true;
		if (pathOwner.m_ElementIndex >= path.Length)
		{
			return false;
		}
		PathElement pathElement = path[pathOwner.m_ElementIndex];
		while (!curveData.HasComponent(pathElement.m_Target))
		{
			int num = pathOwner.m_ElementIndex + ++startOffset;
			if (num >= path.Length)
			{
				return false;
			}
			pathElement = path[num];
		}
		if (pathElement.m_TargetDelta.x != pathElement.m_TargetDelta.y)
		{
			forward = pathElement.m_TargetDelta.y > pathElement.m_TargetDelta.x;
			return true;
		}
		forward = pathElement.m_TargetDelta.y == 1f;
		bool result = forward || pathElement.m_TargetDelta.y == 0f;
		for (int i = pathOwner.m_ElementIndex + startOffset + 1; i < path.Length; i++)
		{
			PathElement pathElement2 = path[i];
			if (pathElement2.m_Target != pathElement.m_Target)
			{
				return result;
			}
			if (pathElement2.m_TargetDelta.x != pathElement2.m_TargetDelta.y)
			{
				forward = pathElement2.m_TargetDelta.y > pathElement2.m_TargetDelta.x;
				return true;
			}
		}
		return result;
	}

	public static bool GetEndDirection(DynamicBuffer<PathElement> path, PathOwner pathOwner, ref ComponentLookup<Curve> curveData, out int endOffset, out bool forward)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		endOffset = 0;
		forward = true;
		if (pathOwner.m_ElementIndex >= path.Length)
		{
			return false;
		}
		PathElement pathElement = path[path.Length - 1];
		while (!curveData.HasComponent(pathElement.m_Target))
		{
			int num = path.Length - 1 - ++endOffset;
			if (num < pathOwner.m_ElementIndex)
			{
				return false;
			}
			pathElement = path[num];
		}
		if (pathElement.m_TargetDelta.x != pathElement.m_TargetDelta.y)
		{
			forward = pathElement.m_TargetDelta.y > pathElement.m_TargetDelta.x;
			return true;
		}
		forward = pathElement.m_TargetDelta.x == 0f;
		bool result = forward || pathElement.m_TargetDelta.x == 1f;
		for (int num2 = path.Length - endOffset - 2; num2 >= pathOwner.m_ElementIndex; num2--)
		{
			PathElement pathElement2 = path[num2];
			if (pathElement2.m_Target != pathElement.m_Target)
			{
				return result;
			}
			if (pathElement2.m_TargetDelta.x != pathElement2.m_TargetDelta.y)
			{
				forward = pathElement2.m_TargetDelta.y > pathElement2.m_TargetDelta.x;
				return true;
			}
		}
		return result;
	}

	public static void ExtendPath(DynamicBuffer<PathElement> path, PathOwner pathOwner, ref float distance, ref ComponentLookup<Curve> curveData, ref ComponentLookup<Lane> laneData, ref ComponentLookup<EdgeLane> edgeLaneData, ref ComponentLookup<Owner> ownerData, ref ComponentLookup<Game.Net.Edge> edgeData, ref BufferLookup<ConnectedEdge> connectedEdges, ref BufferLookup<Game.Net.SubLane> subLanes)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		float num = distance;
		distance = 0f;
		if (!GetEndDirection(path, pathOwner, ref curveData, out var endOffset, out var forward))
		{
			return;
		}
		PathElement pathElement = path[path.Length - endOffset - 1];
		pathElement.m_TargetDelta = new float2(pathElement.m_TargetDelta.y, math.select(0f, 1f, forward));
		for (int i = 0; i < 10000; i++)
		{
			if (!curveData.HasComponent(pathElement.m_Target))
			{
				break;
			}
			if (pathElement.m_TargetDelta.x != pathElement.m_TargetDelta.y)
			{
				float num2 = curveData[pathElement.m_Target].m_Length * math.abs(pathElement.m_TargetDelta.y - pathElement.m_TargetDelta.x);
				if (num2 >= num)
				{
					float num3 = math.select(num / num2, 1f, num2 == 0f);
					pathElement.m_TargetDelta.y = math.lerp(pathElement.m_TargetDelta.x, pathElement.m_TargetDelta.y, num3);
					path.Insert(path.Length - endOffset, pathElement);
					distance += num;
					break;
				}
				path.Insert(path.Length - endOffset, pathElement);
				distance += num2;
				num -= num2;
			}
			if (!NetUtils.FindConnectedLane(ref pathElement.m_Target, ref forward, ref laneData, ref edgeLaneData, ref ownerData, ref edgeData, ref connectedEdges, ref subLanes))
			{
				break;
			}
			pathElement.m_TargetDelta = math.select(new float2(1f, 0f), new float2(0f, 1f), forward);
			pathElement.m_Flags = ~(PathElementFlags.Secondary | PathElementFlags.PathStart | PathElementFlags.Action | PathElementFlags.Return | PathElementFlags.Reverse | PathElementFlags.WaitPosition | PathElementFlags.Leader | PathElementFlags.Hangaround);
		}
	}

	public static void ExtendReverseLocations(PathElement prevElement, DynamicBuffer<PathElement> path, PathOwner pathOwner, float distance, ComponentLookup<Curve> curveData, ComponentLookup<Lane> laneData, ComponentLookup<EdgeLane> edgeLaneData, ComponentLookup<Owner> ownerData, ComponentLookup<Game.Net.Edge> edgeData, BufferLookup<ConnectedEdge> connectedEdges, BufferLookup<Game.Net.SubLane> subLanes)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		if (pathOwner.m_ElementIndex >= path.Length)
		{
			return;
		}
		int num = pathOwner.m_ElementIndex - 1;
		if (prevElement.m_Target == Entity.Null)
		{
			prevElement = path[++num];
		}
		Entity val = Entity.Null;
		if ((prevElement.m_Flags & PathElementFlags.Return) == 0 && prevElement.m_TargetDelta.x != prevElement.m_TargetDelta.y && ownerData.HasComponent(prevElement.m_Target))
		{
			val = ownerData[prevElement.m_Target].m_Owner;
		}
		while (++num < path.Length)
		{
			PathElement pathElement = path[num];
			if (pathElement.m_TargetDelta.x == pathElement.m_TargetDelta.y)
			{
				if ((pathElement.m_Flags & PathElementFlags.Return) != 0)
				{
					val = Entity.Null;
				}
				continue;
			}
			Entity val2 = Entity.Null;
			if (pathElement.m_TargetDelta.x != pathElement.m_TargetDelta.y && ownerData.HasComponent(pathElement.m_Target))
			{
				val2 = ownerData[pathElement.m_Target].m_Owner;
			}
			if (val2 != Entity.Null && val2 == val && curveData.HasComponent(prevElement.m_Target) && curveData.HasComponent(pathElement.m_Target))
			{
				Curve curve = curveData[prevElement.m_Target];
				Curve curve2 = curveData[pathElement.m_Target];
				float3 val3 = MathUtils.Tangent(curve.m_Bezier, prevElement.m_TargetDelta.y);
				float3 val4 = MathUtils.Tangent(curve2.m_Bezier, pathElement.m_TargetDelta.x);
				bool forward = prevElement.m_TargetDelta.y > prevElement.m_TargetDelta.x;
				bool flag = pathElement.m_TargetDelta.y > pathElement.m_TargetDelta.x;
				if (math.dot(val3, val4) * math.select(1f, -1f, forward != flag) < 0f)
				{
					float num2 = distance;
					prevElement.m_TargetDelta = new float2(prevElement.m_TargetDelta.y, math.select(0f, 1f, forward));
					for (int i = 0; i < 10000; i++)
					{
						if (prevElement.m_TargetDelta.x != prevElement.m_TargetDelta.y)
						{
							float num3 = curveData[prevElement.m_Target].m_Length * math.abs(prevElement.m_TargetDelta.y - prevElement.m_TargetDelta.x);
							if (num3 >= num2)
							{
								float num4 = math.select(num2 / num3, 1f, num3 == 0f);
								prevElement.m_TargetDelta.y = math.lerp(prevElement.m_TargetDelta.x, prevElement.m_TargetDelta.y, num4);
								path.Insert(num++, prevElement);
								num2 = 0f;
								break;
							}
							path.Insert(num++, prevElement);
							num2 -= num3;
						}
						if (!NetUtils.FindConnectedLane(ref prevElement.m_Target, ref forward, ref laneData, ref edgeLaneData, ref ownerData, ref edgeData, ref connectedEdges, ref subLanes))
						{
							break;
						}
						prevElement.m_TargetDelta = math.select(new float2(1f, 0f), new float2(0f, 1f), forward);
						prevElement.m_Flags = PathElementFlags.Reverse;
					}
					if (num > 0)
					{
						prevElement = path[num - 1];
						prevElement.m_Flags |= PathElementFlags.Return;
						path[num - 1] = prevElement;
					}
					else
					{
						prevElement.m_TargetDelta.x = prevElement.m_TargetDelta.y;
						prevElement.m_Flags |= PathElementFlags.Return;
						path.Insert(num++, prevElement);
					}
					if (num2 > 0f)
					{
						while (num < path.Length)
						{
							pathElement = path[num];
							if (!curveData.HasComponent(pathElement.m_Target))
							{
								break;
							}
							float num5 = curveData[pathElement.m_Target].m_Length * math.abs(pathElement.m_TargetDelta.y - pathElement.m_TargetDelta.x);
							if (num5 >= num2)
							{
								float num6 = math.select(num2 / num5, 1f, num5 == 0f);
								pathElement.m_TargetDelta.x = math.lerp(pathElement.m_TargetDelta.x, pathElement.m_TargetDelta.y, num6);
								path[num] = pathElement;
								num2 = 0f;
								break;
							}
							path.RemoveAt(num);
							num2 -= num5;
						}
					}
				}
			}
			prevElement = pathElement;
			val = (((pathElement.m_Flags & PathElementFlags.Return) == 0) ? val2 : Entity.Null);
		}
	}

	public static void InitializeSpawnPath(DynamicBuffer<PathElement> path, NativeList<PathElement> laneBuffer, Entity parkingLocation, ref PathOwner pathOwner, float length, ref ComponentLookup<Curve> curveData, ref ComponentLookup<Lane> laneData, ref ComponentLookup<EdgeLane> edgeLaneData, ref ComponentLookup<Owner> ownerData, ref ComponentLookup<Game.Net.Edge> edgeData, ref ComponentLookup<Game.Objects.SpawnLocation> spawnLocationData, ref BufferLookup<ConnectedEdge> connectedEdges, ref BufferLookup<Game.Net.SubLane> subLanes)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		Entity laneEntity;
		Entity laneEntity2;
		bool forward2;
		bool forward3;
		float2 val = default(float2);
		float2 val2 = default(float2);
		if (path.IsCreated)
		{
			if (!GetStartDirection(path, pathOwner, ref curveData, out var startOffset, out var forward))
			{
				return;
			}
			pathOwner.m_ElementIndex += startOffset;
			PathElement pathElement = path[pathOwner.m_ElementIndex];
			laneEntity = pathElement.m_Target;
			laneEntity2 = pathElement.m_Target;
			forward2 = !forward;
			forward3 = forward;
			val = (forward2 ? new float2(1f, pathElement.m_TargetDelta.x) : new float2(0f, pathElement.m_TargetDelta.x));
			val2 = (forward3 ? new float2(pathElement.m_TargetDelta.x, 1f) : new float2(pathElement.m_TargetDelta.x, 0f));
		}
		else
		{
			Game.Objects.SpawnLocation spawnLocation = default(Game.Objects.SpawnLocation);
			if (!spawnLocationData.TryGetComponent(parkingLocation, ref spawnLocation) || !curveData.HasComponent(spawnLocation.m_ConnectedLane1))
			{
				return;
			}
			laneEntity = spawnLocation.m_ConnectedLane1;
			laneEntity2 = spawnLocation.m_ConnectedLane1;
			forward2 = false;
			forward3 = true;
			((float2)(ref val))._002Ector(0f, spawnLocation.m_CurvePosition1);
			((float2)(ref val2))._002Ector(spawnLocation.m_CurvePosition1, 1f);
		}
		float num = length * 0.5f;
		float num2 = 0f;
		for (int i = 0; i < 10000; i++)
		{
			float num3 = curveData[laneEntity].m_Length * math.abs(val.y - val.x);
			PathElement pathElement2;
			if (num3 >= num)
			{
				float num4 = math.select(num / num3, 1f, num3 == 0f);
				val.x = math.lerp(val.y, val.x, num4);
				pathElement2 = new PathElement(laneEntity, val);
				laneBuffer.Add(ref pathElement2);
				num2 = num;
				num = 0f;
				break;
			}
			pathElement2 = new PathElement(laneEntity, val);
			laneBuffer.Add(ref pathElement2);
			num -= num3;
			if (!NetUtils.FindConnectedLane(ref laneEntity, ref forward2, ref laneData, ref edgeLaneData, ref ownerData, ref edgeData, ref connectedEdges, ref subLanes))
			{
				break;
			}
			val = (forward2 ? new float2(1f, 0f) : new float2(0f, 1f));
		}
		CollectionUtils.Reverse<PathElement>(laneBuffer.AsArray());
		num += length * 0.5f;
		if (path.IsCreated)
		{
			Curve curve = default(Curve);
			while (pathOwner.m_ElementIndex < path.Length)
			{
				PathElement pathElement3 = path[pathOwner.m_ElementIndex];
				if (!curveData.TryGetComponent(pathElement3.m_Target, ref curve))
				{
					break;
				}
				float num5 = curve.m_Length * math.abs(pathElement3.m_TargetDelta.y - pathElement3.m_TargetDelta.x);
				if (num5 >= num)
				{
					float num6 = math.select(num / num5, 1f, num5 == 0f);
					num6 = math.lerp(pathElement3.m_TargetDelta.x, pathElement3.m_TargetDelta.y, num6);
					PathElement pathElement2 = new PathElement(pathElement3.m_Target, new float2(pathElement3.m_TargetDelta.x, num6));
					laneBuffer.Add(ref pathElement2);
					num = 0f;
					pathElement3.m_TargetDelta.x = num6;
					path[pathOwner.m_ElementIndex] = pathElement3;
					break;
				}
				laneEntity2 = pathElement3.m_Target;
				if (pathElement3.m_TargetDelta.y != pathElement3.m_TargetDelta.x)
				{
					forward3 = pathElement3.m_TargetDelta.y > pathElement3.m_TargetDelta.x;
				}
				else if (pathElement3.m_TargetDelta.y == 0f)
				{
					forward3 = false;
				}
				else if (pathElement3.m_TargetDelta.y == 1f)
				{
					forward3 = true;
				}
				val2 = (forward3 ? new float2(pathElement3.m_TargetDelta.y, 1f) : new float2(pathElement3.m_TargetDelta.y, 0f));
				laneBuffer.Add(ref pathElement3);
				pathOwner.m_ElementIndex++;
				num -= num5;
			}
		}
		if (num > 0f)
		{
			for (int j = 0; j < 10000; j++)
			{
				float num7 = curveData[laneEntity2].m_Length * math.abs(val2.y - val2.x);
				PathElement pathElement2;
				if (num7 >= num)
				{
					float num8 = math.select(num / num7, 1f, num7 == 0f);
					val2.y = math.lerp(val2.x, val2.y, num8);
					pathElement2 = new PathElement(laneEntity2, val2);
					laneBuffer.Add(ref pathElement2);
					break;
				}
				pathElement2 = new PathElement(laneEntity2, val2);
				laneBuffer.Add(ref pathElement2);
				num -= num7;
				if (!NetUtils.FindConnectedLane(ref laneEntity2, ref forward3, ref laneData, ref edgeLaneData, ref ownerData, ref edgeData, ref connectedEdges, ref subLanes))
				{
					break;
				}
				val2 = (forward3 ? new float2(0f, 1f) : new float2(1f, 0f));
			}
		}
		CollectionUtils.Reverse<PathElement>(laneBuffer.AsArray());
		if (!(num > 0f) || !(num2 > 0f))
		{
			return;
		}
		laneBuffer.RemoveAt(laneBuffer.Length - 1);
		num += num2;
		val = (forward2 ? new float2(1f, 0f) : new float2(0f, 1f));
		for (int k = 0; k < 10000; k++)
		{
			float num9 = curveData[laneEntity].m_Length * math.abs(val.y - val.x);
			PathElement pathElement2;
			if (num9 >= num)
			{
				float num10 = math.select(num / num9, 1f, num9 == 0f);
				val.x = math.lerp(val.y, val.x, num10);
				pathElement2 = new PathElement(laneEntity, val);
				laneBuffer.Add(ref pathElement2);
				break;
			}
			pathElement2 = new PathElement(laneEntity, val);
			laneBuffer.Add(ref pathElement2);
			num -= num9;
			if (NetUtils.FindConnectedLane(ref laneEntity, ref forward2, ref laneData, ref edgeLaneData, ref ownerData, ref edgeData, ref connectedEdges, ref subLanes))
			{
				val = (forward2 ? new float2(1f, 0f) : new float2(0f, 1f));
				continue;
			}
			break;
		}
	}

	public static void ResetPath(ref CarCurrentLane currentLane, DynamicBuffer<PathElement> path, ComponentLookup<SlaveLane> slaveLaneData, ComponentLookup<Owner> ownerData, BufferLookup<Game.Net.SubLane> subLanes)
	{
	}

	public static void ResetPath(ref WatercraftCurrentLane currentLane, DynamicBuffer<PathElement> path, ComponentLookup<SlaveLane> slaveLaneData, ComponentLookup<Owner> ownerData, BufferLookup<Game.Net.SubLane> subLanes)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (currentLane.m_Lane != Entity.Null && path.Length > 0)
		{
			Entity masterLane = GetMasterLane(currentLane.m_Lane, slaveLaneData, ownerData, subLanes);
			PathElement pathElement = path[0];
			if (pathElement.m_Target == masterLane)
			{
				currentLane.m_CurvePosition.z = pathElement.m_TargetDelta.x;
				return;
			}
		}
		currentLane.m_CurvePosition.z = currentLane.m_CurvePosition.y;
	}

	public static void ResetPath(ref AircraftCurrentLane currentLane, DynamicBuffer<PathElement> path)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (currentLane.m_Lane != Entity.Null && path.Length > 0)
		{
			PathElement pathElement = path[0];
			if (pathElement.m_Target == currentLane.m_Lane)
			{
				currentLane.m_CurvePosition.z = pathElement.m_TargetDelta.x;
				return;
			}
		}
		if ((currentLane.m_LaneFlags & (AircraftLaneFlags.Airway | AircraftLaneFlags.Flying)) == (AircraftLaneFlags.Airway | AircraftLaneFlags.Flying))
		{
			currentLane.m_LaneFlags |= AircraftLaneFlags.SkipLane;
		}
		currentLane.m_CurvePosition.z = currentLane.m_CurvePosition.y;
	}

	public static bool TryAppendPath(ref CarCurrentLane currentLane, DynamicBuffer<CarNavigationLane> navigationLanes, DynamicBuffer<PathElement> path, DynamicBuffer<PathElement> appendPath, ComponentLookup<SlaveLane> slaveLaneData, ComponentLookup<Owner> ownerData, BufferLookup<Game.Net.SubLane> subLanes)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		int appendedCount;
		return TryAppendPath(ref currentLane, navigationLanes, path, appendPath, slaveLaneData, ownerData, subLanes, out appendedCount);
	}

	public static bool TryAppendPath(ref CarCurrentLane currentLane, DynamicBuffer<CarNavigationLane> navigationLanes, DynamicBuffer<PathElement> path, DynamicBuffer<PathElement> appendPath, ComponentLookup<SlaveLane> slaveLaneData, ComponentLookup<Owner> ownerData, BufferLookup<Game.Net.SubLane> subLanes, out int appendedCount)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		NativeParallelHashMap<Entity, AppendPathValue> val = default(NativeParallelHashMap<Entity, AppendPathValue>);
		val._002Ector(navigationLanes.Length + path.Length + 1, AllocatorHandle.op_Implicit((Allocator)2));
		if (currentLane.m_Lane != Entity.Null && (navigationLanes.Length == 0 || (navigationLanes[0].m_Flags & Game.Vehicles.CarLaneFlags.Reserved) == 0))
		{
			Entity masterLane = GetMasterLane(currentLane.m_Lane, slaveLaneData, ownerData, subLanes);
			val.TryAdd(masterLane, new AppendPathValue
			{
				m_Index = 0,
				m_TargetDelta = currentLane.m_CurvePosition.y
			});
		}
		for (int i = 1; i <= navigationLanes.Length; i++)
		{
			if (navigationLanes.Length == i || (navigationLanes[i].m_Flags & Game.Vehicles.CarLaneFlags.Reserved) == 0)
			{
				CarNavigationLane carNavigationLane = navigationLanes[i - 1];
				Entity masterLane2 = GetMasterLane(carNavigationLane.m_Lane, slaveLaneData, ownerData, subLanes);
				float targetDelta = math.select(carNavigationLane.m_CurvePosition.x, carNavigationLane.m_CurvePosition.y, (carNavigationLane.m_Flags & Game.Vehicles.CarLaneFlags.Reserved) != 0);
				val.TryAdd(masterLane2, new AppendPathValue
				{
					m_Index = i,
					m_TargetDelta = targetDelta
				});
			}
		}
		int num = navigationLanes.Length + 1;
		for (int j = 0; j < path.Length; j++)
		{
			PathElement pathElement = path[j];
			val.TryAdd(pathElement.m_Target, new AppendPathValue
			{
				m_Index = num + j,
				m_TargetDelta = pathElement.m_TargetDelta.x
			});
		}
		AppendPathValue appendPathValue = default(AppendPathValue);
		for (int k = 0; appendPath.Length > k; k++)
		{
			PathElement pathElement2 = appendPath[k];
			if (!val.TryGetValue(pathElement2.m_Target, ref appendPathValue))
			{
				continue;
			}
			if (appendPathValue.m_Index == 0)
			{
				currentLane.m_CurvePosition.z = appendPathValue.m_TargetDelta;
				navigationLanes.Clear();
				path.Clear();
			}
			else if (appendPathValue.m_Index <= navigationLanes.Length)
			{
				CarNavigationLane carNavigationLane2 = navigationLanes[appendPathValue.m_Index - 1];
				carNavigationLane2.m_CurvePosition.y = appendPathValue.m_TargetDelta;
				navigationLanes[appendPathValue.m_Index - 1] = carNavigationLane2;
				if (appendPathValue.m_Index < navigationLanes.Length)
				{
					navigationLanes.RemoveRange(appendPathValue.m_Index, navigationLanes.Length - appendPathValue.m_Index);
				}
				path.Clear();
			}
			else if (appendPathValue.m_Index < num + path.Length)
			{
				path.RemoveRange(appendPathValue.m_Index - num, num + path.Length - appendPathValue.m_Index);
			}
			path.EnsureCapacity(path.Length + appendPath.Length - k);
			pathElement2.m_TargetDelta.x = appendPathValue.m_TargetDelta;
			path.Add(pathElement2);
			for (int l = k + 1; l < appendPath.Length; l++)
			{
				path.Add(appendPath[l]);
			}
			val.Dispose();
			appendedCount = appendPath.Length - k;
			return true;
		}
		val.Dispose();
		appendedCount = 0;
		return false;
	}

	public static bool TryAppendPath(ref WatercraftCurrentLane currentLane, DynamicBuffer<WatercraftNavigationLane> navigationLanes, DynamicBuffer<PathElement> path, DynamicBuffer<PathElement> appendPath, ComponentLookup<SlaveLane> slaveLaneData, ComponentLookup<Owner> ownerData, BufferLookup<Game.Net.SubLane> subLanes)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		int appendedCount;
		return TryAppendPath(ref currentLane, navigationLanes, path, appendPath, slaveLaneData, ownerData, subLanes, out appendedCount);
	}

	public static bool TryAppendPath(ref WatercraftCurrentLane currentLane, DynamicBuffer<WatercraftNavigationLane> navigationLanes, DynamicBuffer<PathElement> path, DynamicBuffer<PathElement> appendPath, ComponentLookup<SlaveLane> slaveLaneData, ComponentLookup<Owner> ownerData, BufferLookup<Game.Net.SubLane> subLanes, out int appendedCount)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		NativeParallelHashMap<Entity, AppendPathValue> val = default(NativeParallelHashMap<Entity, AppendPathValue>);
		val._002Ector(navigationLanes.Length + path.Length + 1, AllocatorHandle.op_Implicit((Allocator)2));
		if (currentLane.m_Lane != Entity.Null && (navigationLanes.Length == 0 || (navigationLanes[0].m_Flags & WatercraftLaneFlags.Reserved) == 0))
		{
			Entity masterLane = GetMasterLane(currentLane.m_Lane, slaveLaneData, ownerData, subLanes);
			val.TryAdd(masterLane, new AppendPathValue
			{
				m_Index = 0,
				m_TargetDelta = currentLane.m_CurvePosition.y
			});
		}
		for (int i = 1; i <= navigationLanes.Length; i++)
		{
			if (navigationLanes.Length == i || (navigationLanes[i].m_Flags & WatercraftLaneFlags.Reserved) == 0)
			{
				WatercraftNavigationLane watercraftNavigationLane = navigationLanes[i - 1];
				Entity masterLane2 = GetMasterLane(watercraftNavigationLane.m_Lane, slaveLaneData, ownerData, subLanes);
				float targetDelta = math.select(watercraftNavigationLane.m_CurvePosition.x, watercraftNavigationLane.m_CurvePosition.y, (watercraftNavigationLane.m_Flags & WatercraftLaneFlags.Reserved) != 0);
				val.TryAdd(masterLane2, new AppendPathValue
				{
					m_Index = i,
					m_TargetDelta = targetDelta
				});
			}
		}
		int num = navigationLanes.Length + 1;
		for (int j = 0; j < path.Length; j++)
		{
			PathElement pathElement = path[j];
			val.TryAdd(pathElement.m_Target, new AppendPathValue
			{
				m_Index = num + j,
				m_TargetDelta = pathElement.m_TargetDelta.x
			});
		}
		AppendPathValue appendPathValue = default(AppendPathValue);
		for (int k = 0; appendPath.Length > k; k++)
		{
			PathElement pathElement2 = appendPath[k];
			if (!val.TryGetValue(pathElement2.m_Target, ref appendPathValue))
			{
				continue;
			}
			if (appendPathValue.m_Index == 0)
			{
				currentLane.m_CurvePosition.z = appendPathValue.m_TargetDelta;
				navigationLanes.Clear();
				path.Clear();
			}
			else if (appendPathValue.m_Index <= navigationLanes.Length)
			{
				WatercraftNavigationLane watercraftNavigationLane2 = navigationLanes[appendPathValue.m_Index - 1];
				watercraftNavigationLane2.m_CurvePosition.y = appendPathValue.m_TargetDelta;
				navigationLanes[appendPathValue.m_Index - 1] = watercraftNavigationLane2;
				if (appendPathValue.m_Index < navigationLanes.Length)
				{
					navigationLanes.RemoveRange(appendPathValue.m_Index, navigationLanes.Length - appendPathValue.m_Index);
				}
				path.Clear();
			}
			else if (appendPathValue.m_Index < num + path.Length)
			{
				path.RemoveRange(appendPathValue.m_Index - num, num + path.Length - appendPathValue.m_Index);
			}
			path.EnsureCapacity(path.Length + appendPath.Length - k);
			pathElement2.m_TargetDelta.x = appendPathValue.m_TargetDelta;
			path.Add(pathElement2);
			for (int l = k + 1; l < appendPath.Length; l++)
			{
				path.Add(appendPath[l]);
			}
			val.Dispose();
			appendedCount = appendPath.Length - k;
			return true;
		}
		val.Dispose();
		appendedCount = 0;
		return false;
	}

	public static bool TryAppendPath(ref AircraftCurrentLane currentLane, DynamicBuffer<AircraftNavigationLane> navigationLanes, DynamicBuffer<PathElement> path, DynamicBuffer<PathElement> appendPath)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		NativeParallelHashMap<Entity, AppendPathValue> val = default(NativeParallelHashMap<Entity, AppendPathValue>);
		val._002Ector(navigationLanes.Length + path.Length + 1, AllocatorHandle.op_Implicit((Allocator)2));
		if (currentLane.m_Lane != Entity.Null && (navigationLanes.Length == 0 || (navigationLanes[0].m_Flags & AircraftLaneFlags.Reserved) == 0))
		{
			val.TryAdd(currentLane.m_Lane, new AppendPathValue
			{
				m_Index = 0,
				m_TargetDelta = currentLane.m_CurvePosition.y
			});
		}
		for (int i = 1; i <= navigationLanes.Length; i++)
		{
			if (navigationLanes.Length == i || (navigationLanes[i].m_Flags & AircraftLaneFlags.Reserved) == 0)
			{
				AircraftNavigationLane aircraftNavigationLane = navigationLanes[i - 1];
				float targetDelta = math.select(aircraftNavigationLane.m_CurvePosition.x, aircraftNavigationLane.m_CurvePosition.y, (aircraftNavigationLane.m_Flags & AircraftLaneFlags.Reserved) != 0);
				val.TryAdd(aircraftNavigationLane.m_Lane, new AppendPathValue
				{
					m_Index = i,
					m_TargetDelta = targetDelta
				});
			}
		}
		int num = navigationLanes.Length + 1;
		for (int j = 0; j < path.Length; j++)
		{
			PathElement pathElement = path[j];
			val.TryAdd(pathElement.m_Target, new AppendPathValue
			{
				m_Index = num + j,
				m_TargetDelta = pathElement.m_TargetDelta.x
			});
		}
		AppendPathValue appendPathValue = default(AppendPathValue);
		for (int k = 0; appendPath.Length > k; k++)
		{
			PathElement pathElement2 = appendPath[k];
			if (!val.TryGetValue(pathElement2.m_Target, ref appendPathValue))
			{
				continue;
			}
			if (appendPathValue.m_Index == 0)
			{
				currentLane.m_CurvePosition.z = appendPathValue.m_TargetDelta;
				navigationLanes.Clear();
				path.Clear();
			}
			else if (appendPathValue.m_Index <= navigationLanes.Length)
			{
				AircraftNavigationLane aircraftNavigationLane2 = navigationLanes[appendPathValue.m_Index - 1];
				aircraftNavigationLane2.m_CurvePosition.y = appendPathValue.m_TargetDelta;
				navigationLanes[appendPathValue.m_Index - 1] = aircraftNavigationLane2;
				if (appendPathValue.m_Index < navigationLanes.Length)
				{
					navigationLanes.RemoveRange(appendPathValue.m_Index, navigationLanes.Length - appendPathValue.m_Index);
				}
				path.Clear();
			}
			else if (appendPathValue.m_Index < num + path.Length)
			{
				path.RemoveRange(appendPathValue.m_Index - num, num + path.Length - appendPathValue.m_Index);
			}
			path.EnsureCapacity(path.Length + appendPath.Length - k);
			pathElement2.m_TargetDelta.x = appendPathValue.m_TargetDelta;
			path.Add(pathElement2);
			for (int l = k + 1; l < appendPath.Length; l++)
			{
				path.Add(appendPath[l]);
			}
			val.Dispose();
			return true;
		}
		val.Dispose();
		return false;
	}

	public static bool TryAppendPath(ref TrainCurrentLane currentLane, DynamicBuffer<TrainNavigationLane> navigationLanes, DynamicBuffer<PathElement> path, DynamicBuffer<PathElement> appendPath)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		NativeParallelHashMap<Entity, AppendPathValue> val = default(NativeParallelHashMap<Entity, AppendPathValue>);
		val._002Ector(navigationLanes.Length + path.Length + 1, AllocatorHandle.op_Implicit((Allocator)2));
		if (currentLane.m_Front.m_Lane != Entity.Null && (navigationLanes.Length == 0 || (navigationLanes[0].m_Flags & TrainLaneFlags.Reserved) == 0))
		{
			val.TryAdd(currentLane.m_Front.m_Lane, new AppendPathValue
			{
				m_Index = 0,
				m_TargetDelta = currentLane.m_Front.m_CurvePosition.z
			});
		}
		for (int i = 1; i <= navigationLanes.Length; i++)
		{
			if (navigationLanes.Length == i || (navigationLanes[i].m_Flags & TrainLaneFlags.Reserved) == 0)
			{
				TrainNavigationLane trainNavigationLane = navigationLanes[i - 1];
				float targetDelta = math.select(trainNavigationLane.m_CurvePosition.x, trainNavigationLane.m_CurvePosition.y, (trainNavigationLane.m_Flags & TrainLaneFlags.Reserved) != 0);
				val.TryAdd(trainNavigationLane.m_Lane, new AppendPathValue
				{
					m_Index = i,
					m_TargetDelta = targetDelta
				});
			}
		}
		int num = navigationLanes.Length + 1;
		for (int j = 0; j < path.Length; j++)
		{
			PathElement pathElement = path[j];
			val.TryAdd(pathElement.m_Target, new AppendPathValue
			{
				m_Index = num + j,
				m_TargetDelta = pathElement.m_TargetDelta.x
			});
		}
		AppendPathValue appendPathValue = default(AppendPathValue);
		for (int k = 0; appendPath.Length > k; k++)
		{
			PathElement pathElement2 = appendPath[k];
			if (!val.TryGetValue(pathElement2.m_Target, ref appendPathValue))
			{
				continue;
			}
			if (appendPathValue.m_Index == 0)
			{
				currentLane.m_Front.m_CurvePosition.w = appendPathValue.m_TargetDelta;
				navigationLanes.Clear();
				path.Clear();
			}
			else if (appendPathValue.m_Index <= navigationLanes.Length)
			{
				TrainNavigationLane trainNavigationLane2 = navigationLanes[appendPathValue.m_Index - 1];
				trainNavigationLane2.m_CurvePosition.y = appendPathValue.m_TargetDelta;
				navigationLanes[appendPathValue.m_Index - 1] = trainNavigationLane2;
				if (appendPathValue.m_Index < navigationLanes.Length)
				{
					navigationLanes.RemoveRange(appendPathValue.m_Index, navigationLanes.Length - appendPathValue.m_Index);
				}
				path.Clear();
			}
			else if (appendPathValue.m_Index < num + path.Length)
			{
				path.RemoveRange(appendPathValue.m_Index - num, num + path.Length - appendPathValue.m_Index);
			}
			path.EnsureCapacity(path.Length + appendPath.Length - k);
			pathElement2.m_TargetDelta.x = appendPathValue.m_TargetDelta;
			path.Add(pathElement2);
			for (int l = k + 1; l < appendPath.Length; l++)
			{
				path.Add(appendPath[l]);
			}
			val.Dispose();
			return true;
		}
		val.Dispose();
		return false;
	}

	public static Entity GetMasterLane(Entity lane, ComponentLookup<SlaveLane> slaveLaneData, ComponentLookup<Owner> ownerData, BufferLookup<Game.Net.SubLane> subLanes)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (slaveLaneData.HasComponent(lane) && ownerData.HasComponent(lane))
		{
			SlaveLane slaveLane = slaveLaneData[lane];
			Owner owner = ownerData[lane];
			if (subLanes.HasBuffer(owner.m_Owner))
			{
				return subLanes[owner.m_Owner][(int)slaveLane.m_MasterIndex].m_SubLane;
			}
		}
		return lane;
	}

	public static void TryAddCosts(ref PathfindCosts costs, PathfindCosts add)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		costs.m_Value += add.m_Value;
	}

	public static void TryAddCosts(ref PathfindCosts costs, PathfindCosts add, float distance)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		costs.m_Value += add.m_Value * distance;
	}

	public static void TryAddCosts(ref PathfindCosts costs, PathfindCosts add, Bezier4x3 curve)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		float3 val = MathUtils.StartTangent(curve);
		float2 xz = ((float3)(ref val)).xz;
		val = MathUtils.EndTangent(curve);
		float2 xz2 = ((float3)(ref val)).xz;
		if (MathUtils.TryNormalize(ref xz) && MathUtils.TryNormalize(ref xz2))
		{
			float distance = math.acos(math.clamp(math.dot(xz, xz2), -1f, 1f));
			TryAddCosts(ref costs, add, distance);
		}
	}

	public static void TryAddCosts(ref PathfindCosts costs, PathfindCosts add, bool doIt)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		costs.m_Value = math.select(costs.m_Value, costs.m_Value + add.m_Value, doIt);
	}

	public static void TryAddCosts(ref PathfindCosts costs, PathfindCosts add, float distance, bool doIt)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		costs.m_Value = math.select(costs.m_Value, costs.m_Value + add.m_Value * distance, doIt);
	}

	public static PathSpecification GetCarDriveSpecification(Curve curve, Game.Net.CarLane carLane, CarLaneData carLaneData, PathfindCarData carPathfindData, float density)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		PathSpecification result = new PathSpecification
		{
			m_Flags = EdgeFlags.Forward,
			m_Methods = VehicleUtils.GetPathMethods(carLaneData),
			m_Length = curve.m_Length,
			m_MaxSpeed = carLane.m_SpeedLimit,
			m_Density = math.sqrt(density),
			m_FlowOffset = carLane.m_FlowOffset,
			m_AccessRequirement = math.select(-1, carLane.m_AccessRestriction.Index, carLane.m_AccessRestriction != Entity.Null)
		};
		TryAddCosts(ref result.m_Costs, carPathfindData.m_DrivingCost, result.m_Length);
		TryAddCosts(ref result.m_Costs, carPathfindData.m_CurveAngleCost, curve.m_Bezier);
		TryAddCosts(ref result.m_Costs, carPathfindData.m_LaneCrossCost, (float)(int)carLane.m_LaneCrossCount);
		if ((carLane.m_Flags & Game.Net.CarLaneFlags.Approach) == 0)
		{
			bool flag = (carLane.m_Flags & Game.Net.CarLaneFlags.Forbidden) != 0;
			bool flag2 = (carLane.m_Flags & Game.Net.CarLaneFlags.Unsafe) != 0;
			bool flag3 = (carLane.m_Flags & Game.Net.CarLaneFlags.Highway) != 0;
			bool doIt = (carLane.m_Flags & (Game.Net.CarLaneFlags.TurnLeft | Game.Net.CarLaneFlags.TurnRight | Game.Net.CarLaneFlags.GentleTurnLeft | Game.Net.CarLaneFlags.GentleTurnRight)) != 0;
			bool flag4 = (carLane.m_Flags & (Game.Net.CarLaneFlags.UTurnLeft | Game.Net.CarLaneFlags.UTurnRight)) != 0;
			TryAddCosts(ref result.m_Costs, carPathfindData.m_ForbiddenCost, flag || (flag2 && flag3 && flag4));
			TryAddCosts(ref result.m_Costs, carPathfindData.m_TurningCost, doIt);
			if ((carLane.m_Flags & Game.Net.CarLaneFlags.Unsafe) != 0)
			{
				TryAddCosts(ref result.m_Costs, carPathfindData.m_UnsafeUTurnCost, flag4);
			}
			else
			{
				TryAddCosts(ref result.m_Costs, carPathfindData.m_UTurnCost, flag4);
			}
		}
		if ((carLane.m_Flags & Game.Net.CarLaneFlags.Unsafe) == 0)
		{
			result.m_Flags |= EdgeFlags.AllowMiddle;
		}
		if ((carLane.m_Flags & Game.Net.CarLaneFlags.Twoway) != 0)
		{
			result.m_Flags |= EdgeFlags.Backward;
		}
		if (carLane.m_BlockageEnd >= carLane.m_BlockageStart)
		{
			result.m_Rules |= RuleFlags.HasBlockage;
			result.m_BlockageStart = carLane.m_BlockageStart;
			result.m_BlockageEnd = carLane.m_BlockageEnd;
		}
		if ((carLane.m_Flags & Game.Net.CarLaneFlags.ForbidCombustionEngines) != 0)
		{
			result.m_Rules |= RuleFlags.ForbidCombustionEngines;
		}
		if ((carLane.m_Flags & Game.Net.CarLaneFlags.ForbidTransitTraffic) != 0)
		{
			result.m_Rules |= RuleFlags.ForbidTransitTraffic;
		}
		if ((carLane.m_Flags & Game.Net.CarLaneFlags.ForbidHeavyTraffic) != 0)
		{
			result.m_Rules |= RuleFlags.ForbidHeavyTraffic;
		}
		if ((carLane.m_Flags & Game.Net.CarLaneFlags.PublicOnly) != 0)
		{
			result.m_Rules |= RuleFlags.ForbidPrivateTraffic;
		}
		if ((carLane.m_Flags & Game.Net.CarLaneFlags.Highway) != 0)
		{
			result.m_Rules |= RuleFlags.ForbidSlowTraffic;
		}
		if ((carLane.m_Flags & Game.Net.CarLaneFlags.SideConnection) != 0)
		{
			result.m_Flags |= EdgeFlags.SingleOnly;
		}
		if (((uint)carLane.m_Flags & 0x80000000u) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowEnter;
		}
		return result;
	}

	public static PathSpecification GetCarDriveSpecification(Curve curve, Game.Net.CarLane carLane, Game.Net.TrackLane trackLaneData, CarLaneData carLaneData, PathfindCarData carPathfindData, float density)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		PathSpecification result = new PathSpecification
		{
			m_Flags = EdgeFlags.Forward,
			m_Methods = (VehicleUtils.GetPathMethods(carLaneData) | PathMethod.Track),
			m_Length = curve.m_Length,
			m_MaxSpeed = carLane.m_SpeedLimit,
			m_Density = math.sqrt(density),
			m_FlowOffset = carLane.m_FlowOffset,
			m_AccessRequirement = math.select(-1, carLane.m_AccessRestriction.Index, carLane.m_AccessRestriction != Entity.Null)
		};
		TryAddCosts(ref result.m_Costs, carPathfindData.m_DrivingCost, result.m_Length);
		TryAddCosts(ref result.m_Costs, carPathfindData.m_CurveAngleCost, curve.m_Bezier);
		TryAddCosts(ref result.m_Costs, carPathfindData.m_LaneCrossCost, (float)(int)carLane.m_LaneCrossCount);
		if ((carLane.m_Flags & Game.Net.CarLaneFlags.Approach) == 0)
		{
			bool flag = (carLane.m_Flags & Game.Net.CarLaneFlags.Forbidden) != 0;
			bool flag2 = (carLane.m_Flags & Game.Net.CarLaneFlags.Unsafe) != 0;
			bool flag3 = (carLane.m_Flags & Game.Net.CarLaneFlags.Highway) != 0;
			bool doIt = (carLane.m_Flags & (Game.Net.CarLaneFlags.TurnLeft | Game.Net.CarLaneFlags.TurnRight | Game.Net.CarLaneFlags.GentleTurnLeft | Game.Net.CarLaneFlags.GentleTurnRight)) != 0;
			bool flag4 = (carLane.m_Flags & (Game.Net.CarLaneFlags.UTurnLeft | Game.Net.CarLaneFlags.UTurnRight)) != 0;
			TryAddCosts(ref result.m_Costs, carPathfindData.m_ForbiddenCost, flag || (flag2 && flag3 && flag4));
			TryAddCosts(ref result.m_Costs, carPathfindData.m_TurningCost, doIt);
			if ((carLane.m_Flags & Game.Net.CarLaneFlags.Unsafe) != 0)
			{
				TryAddCosts(ref result.m_Costs, carPathfindData.m_UnsafeUTurnCost, flag4);
			}
			else
			{
				TryAddCosts(ref result.m_Costs, carPathfindData.m_UTurnCost, flag4);
			}
		}
		if ((carLane.m_Flags & Game.Net.CarLaneFlags.Unsafe) == 0 || (trackLaneData.m_Flags & TrackLaneFlags.AllowMiddle) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowMiddle;
		}
		if ((carLane.m_Flags & Game.Net.CarLaneFlags.Twoway) != 0 || (trackLaneData.m_Flags & TrackLaneFlags.Twoway) != 0)
		{
			result.m_Flags |= EdgeFlags.Backward;
		}
		if (carLane.m_BlockageEnd >= carLane.m_BlockageStart)
		{
			result.m_Rules |= RuleFlags.HasBlockage;
			result.m_BlockageStart = carLane.m_BlockageStart;
			result.m_BlockageEnd = carLane.m_BlockageEnd;
		}
		if ((carLane.m_Flags & Game.Net.CarLaneFlags.ForbidCombustionEngines) != 0)
		{
			result.m_Rules |= RuleFlags.ForbidCombustionEngines;
		}
		if ((carLane.m_Flags & Game.Net.CarLaneFlags.ForbidTransitTraffic) != 0)
		{
			result.m_Rules |= RuleFlags.ForbidTransitTraffic;
		}
		if ((carLane.m_Flags & Game.Net.CarLaneFlags.ForbidHeavyTraffic) != 0)
		{
			result.m_Rules |= RuleFlags.ForbidHeavyTraffic;
		}
		if ((carLane.m_Flags & Game.Net.CarLaneFlags.PublicOnly) != 0)
		{
			result.m_Rules |= RuleFlags.ForbidPrivateTraffic;
		}
		if ((carLane.m_Flags & Game.Net.CarLaneFlags.Highway) != 0)
		{
			result.m_Rules |= RuleFlags.ForbidSlowTraffic;
		}
		if ((carLane.m_Flags & Game.Net.CarLaneFlags.SideConnection) != 0)
		{
			result.m_Flags |= EdgeFlags.SingleOnly;
		}
		if (((uint)carLane.m_Flags & 0x80000000u) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowEnter;
		}
		return result;
	}

	public static PathSpecification GetTaxiDriveSpecification(Curve curveData, Game.Net.CarLane carLaneData, PathfindCarData carPathfindData, PathfindTransportData transportPathfindData, float density)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		PathSpecification result = new PathSpecification
		{
			m_Flags = (EdgeFlags.Forward | EdgeFlags.SecondaryStart | EdgeFlags.SecondaryEnd),
			m_Methods = PathMethod.Taxi,
			m_Length = curveData.m_Length,
			m_MaxSpeed = carLaneData.m_SpeedLimit,
			m_Density = math.sqrt(density),
			m_FlowOffset = carLaneData.m_FlowOffset,
			m_AccessRequirement = math.select(-1, carLaneData.m_AccessRestriction.Index, carLaneData.m_AccessRestriction != Entity.Null)
		};
		transportPathfindData.m_TravelCost.m_Value.z *= 0.03f;
		TryAddCosts(ref result.m_Costs, transportPathfindData.m_TravelCost, result.m_Length);
		TryAddCosts(ref result.m_Costs, carPathfindData.m_CurveAngleCost, curveData.m_Bezier);
		TryAddCosts(ref result.m_Costs, carPathfindData.m_LaneCrossCost, (float)(int)carLaneData.m_LaneCrossCount);
		if ((carLaneData.m_Flags & Game.Net.CarLaneFlags.Approach) == 0)
		{
			bool flag = (carLaneData.m_Flags & Game.Net.CarLaneFlags.Forbidden) != 0;
			bool flag2 = (carLaneData.m_Flags & Game.Net.CarLaneFlags.Unsafe) != 0;
			bool flag3 = (carLaneData.m_Flags & Game.Net.CarLaneFlags.Highway) != 0;
			bool doIt = (carLaneData.m_Flags & (Game.Net.CarLaneFlags.TurnLeft | Game.Net.CarLaneFlags.TurnRight | Game.Net.CarLaneFlags.GentleTurnLeft | Game.Net.CarLaneFlags.GentleTurnRight)) != 0;
			bool flag4 = (carLaneData.m_Flags & (Game.Net.CarLaneFlags.UTurnLeft | Game.Net.CarLaneFlags.UTurnRight)) != 0;
			TryAddCosts(ref result.m_Costs, carPathfindData.m_ForbiddenCost, flag || (flag2 && flag3 && flag4));
			TryAddCosts(ref result.m_Costs, carPathfindData.m_TurningCost, doIt);
			if ((carLaneData.m_Flags & Game.Net.CarLaneFlags.Unsafe) != 0)
			{
				TryAddCosts(ref result.m_Costs, carPathfindData.m_UnsafeUTurnCost, flag4);
			}
			else
			{
				TryAddCosts(ref result.m_Costs, carPathfindData.m_UTurnCost, flag4);
			}
		}
		if ((carLaneData.m_Flags & Game.Net.CarLaneFlags.Unsafe) == 0)
		{
			result.m_Flags |= EdgeFlags.AllowMiddle;
		}
		if ((carLaneData.m_Flags & Game.Net.CarLaneFlags.Twoway) != 0)
		{
			result.m_Flags |= EdgeFlags.Backward;
		}
		if (carLaneData.m_BlockageEnd >= carLaneData.m_BlockageStart)
		{
			result.m_Rules |= RuleFlags.HasBlockage;
			result.m_BlockageStart = carLaneData.m_BlockageStart;
			result.m_BlockageEnd = carLaneData.m_BlockageEnd;
		}
		if ((carLaneData.m_Flags & Game.Net.CarLaneFlags.ForbidCombustionEngines) != 0)
		{
			result.m_Rules |= RuleFlags.ForbidCombustionEngines;
		}
		if ((carLaneData.m_Flags & Game.Net.CarLaneFlags.ForbidTransitTraffic) != 0)
		{
			result.m_Rules |= RuleFlags.ForbidTransitTraffic;
		}
		if ((carLaneData.m_Flags & Game.Net.CarLaneFlags.ForbidHeavyTraffic) != 0)
		{
			result.m_Rules |= RuleFlags.ForbidHeavyTraffic;
		}
		if ((carLaneData.m_Flags & Game.Net.CarLaneFlags.PublicOnly) != 0)
		{
			result.m_Rules |= RuleFlags.ForbidPrivateTraffic;
		}
		if ((carLaneData.m_Flags & Game.Net.CarLaneFlags.Highway) != 0)
		{
			result.m_Rules |= RuleFlags.ForbidSlowTraffic;
		}
		if ((carLaneData.m_Flags & Game.Net.CarLaneFlags.SideConnection) != 0)
		{
			result.m_Flags |= EdgeFlags.SingleOnly;
		}
		if (((uint)carLaneData.m_Flags & 0x80000000u) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowEnter;
		}
		return result;
	}

	public static PathSpecification GetTrackDriveSpecification(Curve curveData, Game.Net.TrackLane trackLaneData, PathfindTrackData trackPathfindData)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		PathSpecification result = new PathSpecification
		{
			m_Flags = (((trackLaneData.m_Flags & TrackLaneFlags.Twoway) == 0) ? EdgeFlags.Forward : (EdgeFlags.Forward | EdgeFlags.Backward)),
			m_Methods = PathMethod.Track,
			m_Length = curveData.m_Length,
			m_MaxSpeed = trackLaneData.m_SpeedLimit,
			m_Density = 0f,
			m_AccessRequirement = math.select(-1, trackLaneData.m_AccessRestriction.Index, trackLaneData.m_AccessRestriction != Entity.Null)
		};
		if ((trackLaneData.m_Flags & TrackLaneFlags.AllowMiddle) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowMiddle;
		}
		TryAddCosts(ref result.m_Costs, trackPathfindData.m_DrivingCost, result.m_Length);
		TryAddCosts(ref result.m_Costs, trackPathfindData.m_CurveAngleCost, curveData.m_Bezier);
		TryAddCosts(ref result.m_Costs, trackPathfindData.m_TwowayCost, (trackLaneData.m_Flags & TrackLaneFlags.Twoway) != 0);
		TryAddCosts(ref result.m_Costs, trackPathfindData.m_SwitchCost, (trackLaneData.m_Flags & TrackLaneFlags.Switch) != 0);
		TryAddCosts(ref result.m_Costs, trackPathfindData.m_SwitchCost, (trackLaneData.m_Flags & TrackLaneFlags.DoubleSwitch) != 0);
		TryAddCosts(ref result.m_Costs, trackPathfindData.m_DiamondCrossingCost, (trackLaneData.m_Flags & TrackLaneFlags.DiamondCrossing) != 0);
		return result;
	}

	public static PathSpecification GetParkingSpaceSpecification(Game.Net.ParkingLane parkingLane, ParkingLaneData parkingLaneData, PathfindCarData carPathfindData)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		PathSpecification result = new PathSpecification
		{
			m_Flags = (EdgeFlags.Forward | EdgeFlags.Backward | EdgeFlags.FreeBackward),
			m_Methods = PathMethod.Boarding,
			m_Length = 0f,
			m_MaxSpeed = math.max(1f, parkingLane.m_FreeSpace),
			m_Density = VehicleUtils.GetParkingSize(parkingLaneData).x,
			m_AccessRequirement = math.select(-1, parkingLane.m_AccessRestriction.Index, parkingLane.m_AccessRestriction != Entity.Null)
		};
		if ((parkingLane.m_Flags & ParkingLaneFlags.VirtualLane) == 0)
		{
			result.m_Methods |= (PathMethod)(((parkingLane.m_Flags & ParkingLaneFlags.SpecialVehicles) != 0) ? 4096 : 4);
			result.m_MaxSpeed = math.select(result.m_MaxSpeed, 1f, (parkingLane.m_Flags & ParkingLaneFlags.ParkingDisabled) != 0);
		}
		if ((parkingLane.m_Flags & ParkingLaneFlags.AllowEnter) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowEnter;
		}
		if ((parkingLane.m_Flags & ParkingLaneFlags.AllowExit) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowExit;
		}
		carPathfindData.m_ParkingCost.m_Value.z *= (int)parkingLane.m_ParkingFee;
		carPathfindData.m_ParkingCost.m_Value.w *= (float)(65535 - parkingLane.m_ComfortFactor) * 1.5259022E-05f;
		TryAddCosts(ref result.m_Costs, carPathfindData.m_ParkingCost);
		return result;
	}

	public static float GetTaxiAvailabilityDelay(Game.Net.ParkingLane parkingLaneData)
	{
		return 100f / (0.25f + (float)(int)parkingLaneData.m_TaxiAvailability * 1.5259022E-05f) - 80f;
	}

	public static PathSpecification GetTaxiAccessSpecification(Game.Net.ParkingLane parkingLaneData, PathfindCarData carPathfindData, PathfindTransportData transportPathfindData)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		PathSpecification result = new PathSpecification
		{
			m_Flags = (EdgeFlags.Forward | EdgeFlags.SecondaryStart | EdgeFlags.FreeForward),
			m_Methods = PathMethod.Taxi,
			m_Length = 0f,
			m_MaxSpeed = 1f,
			m_Density = 0f,
			m_AccessRequirement = math.select(-1, parkingLaneData.m_AccessRestriction.Index, parkingLaneData.m_AccessRestriction != Entity.Null)
		};
		if ((parkingLaneData.m_Flags & ParkingLaneFlags.AllowEnter) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowEnter;
		}
		if ((parkingLaneData.m_Flags & ParkingLaneFlags.AllowExit) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowExit;
		}
		if (parkingLaneData.m_TaxiAvailability != 0)
		{
			result.m_Flags |= EdgeFlags.Backward;
			transportPathfindData.m_OrderingCost.m_Value.x += GetTaxiAvailabilityDelay(parkingLaneData);
			transportPathfindData.m_StartingCost.m_Value.z *= (int)parkingLaneData.m_TaxiFee;
			TryAddCosts(ref result.m_Costs, transportPathfindData.m_OrderingCost);
			TryAddCosts(ref result.m_Costs, transportPathfindData.m_StartingCost);
		}
		return result;
	}

	public static PathSpecification GetSpecification(Curve curveData, Game.Net.PedestrianLane pedestrianLaneData, PathfindPedestrianData pedestrianPathfindData)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		PathSpecification result = new PathSpecification
		{
			m_Flags = (EdgeFlags.Forward | EdgeFlags.Backward),
			m_Methods = PathMethod.Pedestrian,
			m_Length = curveData.m_Length,
			m_MaxSpeed = 5.555556f,
			m_Density = 1f,
			m_AccessRequirement = math.select(-1, pedestrianLaneData.m_AccessRestriction.Index, pedestrianLaneData.m_AccessRestriction != Entity.Null)
		};
		if ((pedestrianLaneData.m_Flags & PedestrianLaneFlags.AllowMiddle) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowMiddle;
		}
		if ((pedestrianLaneData.m_Flags & PedestrianLaneFlags.AllowEnter) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowEnter;
		}
		if ((pedestrianLaneData.m_Flags & PedestrianLaneFlags.AllowExit) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowExit;
		}
		if ((pedestrianLaneData.m_Flags & PedestrianLaneFlags.ForbidTransitTraffic) != 0)
		{
			result.m_Rules |= RuleFlags.ForbidTransitTraffic;
		}
		if ((pedestrianLaneData.m_Flags & PedestrianLaneFlags.OnWater) != 0)
		{
			result.m_Flags &= ~(EdgeFlags.Forward | EdgeFlags.Backward);
		}
		TryAddCosts(ref result.m_Costs, pedestrianPathfindData.m_WalkingCost, result.m_Length);
		TryAddCosts(ref result.m_Costs, pedestrianPathfindData.m_CrosswalkCost, (pedestrianLaneData.m_Flags & (PedestrianLaneFlags.Unsafe | PedestrianLaneFlags.Crosswalk)) == PedestrianLaneFlags.Crosswalk);
		TryAddCosts(ref result.m_Costs, pedestrianPathfindData.m_UnsafeCrosswalkCost, (pedestrianLaneData.m_Flags & (PedestrianLaneFlags.Unsafe | PedestrianLaneFlags.Crosswalk)) == (PedestrianLaneFlags.Unsafe | PedestrianLaneFlags.Crosswalk));
		return result;
	}

	public static PathSpecification GetSpecification(Curve curveData, Game.Net.ConnectionLane connectionLaneData, GarageLane garageLane, Game.Net.OutsideConnection outsideConnection, PathfindConnectionData connectionPathfindData)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		PathSpecification result = new PathSpecification
		{
			m_Flags = (EdgeFlags.Forward | EdgeFlags.Backward),
			m_AccessRequirement = math.select(-1, connectionLaneData.m_AccessRestriction.Index, connectionLaneData.m_AccessRestriction != Entity.Null),
			m_Costs = 
			{
				m_Value = 
				{
					x = outsideConnection.m_Delay
				}
			}
		};
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.Parking) != 0)
		{
			result.m_Methods |= PathMethod.Parking;
			if ((connectionLaneData.m_Flags & ConnectionLaneFlags.Outside) != 0)
			{
				result.m_Methods |= PathMethod.Boarding;
				result.m_MaxSpeed = 1000000f;
				result.m_Density = 1000000f;
			}
			else
			{
				bool flag = garageLane.m_VehicleCount < garageLane.m_VehicleCapacity && (connectionLaneData.m_Flags & ConnectionLaneFlags.Disabled) == 0;
				result.m_Flags |= EdgeFlags.FreeBackward;
				result.m_MaxSpeed = math.select(1f, 1000000f, flag);
				result.m_Density = math.select(0f, 1000000f, flag);
				connectionPathfindData.m_ParkingCost.m_Value.z *= (int)garageLane.m_ParkingFee;
				connectionPathfindData.m_ParkingCost.m_Value.w *= (float)(65535 - garageLane.m_ComfortFactor) * 1.5259022E-05f;
				TryAddCosts(ref result.m_Costs, connectionPathfindData.m_ParkingCost);
			}
		}
		else if ((connectionLaneData.m_Flags & ConnectionLaneFlags.Airway) != 0)
		{
			result.m_Length = curveData.m_Length;
			result.m_MaxSpeed = math.select(83.333336f, 277.77777f, (connectionLaneData.m_RoadTypes & RoadTypes.Airplane) != 0);
		}
		else if ((connectionLaneData.m_Flags & (ConnectionLaneFlags.Inside | ConnectionLaneFlags.Area)) != 0)
		{
			result.m_Length = curveData.m_Length;
			result.m_MaxSpeed = math.select(3f, 5.555556f, (connectionLaneData.m_Flags & ConnectionLaneFlags.Pedestrian) != 0);
		}
		else
		{
			result.m_MaxSpeed = 1f;
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.Road) != 0)
		{
			if ((connectionLaneData.m_Flags & ConnectionLaneFlags.Area) != 0)
			{
				result.m_Methods |= PathMethod.Offroad;
			}
			else
			{
				result.m_Methods |= PathMethod.Road;
			}
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.Track) != 0)
		{
			result.m_Methods |= PathMethod.Track;
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.Pedestrian) != 0)
		{
			result.m_Methods |= PathMethod.Pedestrian;
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.AllowMiddle) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowMiddle;
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.AllowCargo) != 0)
		{
			result.m_Methods |= PathMethod.CargoLoading;
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.Airway) != 0)
		{
			result.m_Methods |= PathMethod.Flying;
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.Outside) != 0)
		{
			result.m_Flags |= EdgeFlags.OutsideConnection;
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.AllowEnter) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowEnter;
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.AllowExit) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowExit;
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.Parking) == 0)
		{
			TryAddCosts(ref result.m_Costs, ((connectionLaneData.m_Flags & ConnectionLaneFlags.Pedestrian) != 0) ? connectionPathfindData.m_PedestrianBorderCost : connectionPathfindData.m_BorderCost, (connectionLaneData.m_Flags & ConnectionLaneFlags.Start) != 0);
			TryAddCosts(ref result.m_Costs, connectionPathfindData.m_DistanceCost, math.select(0f, curveData.m_Length, (connectionLaneData.m_Flags & ConnectionLaneFlags.Distance) != 0));
		}
		TryAddCosts(ref result.m_Costs, connectionPathfindData.m_AirwayCost, result.m_Length, (connectionLaneData.m_Flags & ConnectionLaneFlags.Airway) != 0);
		TryAddCosts(ref result.m_Costs, connectionPathfindData.m_InsideCost, result.m_Length, (connectionLaneData.m_Flags & ConnectionLaneFlags.Inside) != 0);
		TryAddCosts(ref result.m_Costs, connectionPathfindData.m_AreaCost, result.m_Length, (connectionLaneData.m_Flags & ConnectionLaneFlags.Area) != 0);
		return result;
	}

	public static PathSpecification GetSecondarySpecification(Curve curveData, Game.Net.ConnectionLane connectionLaneData, Game.Net.OutsideConnection outsideConnection, PathfindConnectionData connectionPathfindData)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		PathSpecification result = new PathSpecification
		{
			m_Flags = (EdgeFlags.Forward | EdgeFlags.Backward),
			m_Length = 0f,
			m_MaxSpeed = 1f,
			m_Density = 0f,
			m_AccessRequirement = math.select(-1, connectionLaneData.m_AccessRestriction.Index, connectionLaneData.m_AccessRestriction != Entity.Null),
			m_Costs = 
			{
				m_Value = 
				{
					x = outsideConnection.m_Delay
				}
			}
		};
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.Road) != 0)
		{
			result.m_Methods |= PathMethod.Taxi;
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.Track) != 0)
		{
			result.m_Methods |= PathMethod.Track;
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.Pedestrian) != 0)
		{
			result.m_Methods |= PathMethod.Pedestrian;
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.AllowMiddle) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowMiddle;
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.SecondaryStart) != 0)
		{
			result.m_Flags |= EdgeFlags.SecondaryStart;
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.SecondaryEnd) != 0)
		{
			result.m_Flags |= EdgeFlags.SecondaryEnd;
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.Outside) != 0)
		{
			result.m_Flags |= EdgeFlags.OutsideConnection;
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.AllowEnter) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowEnter;
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.AllowExit) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowExit;
		}
		if ((connectionLaneData.m_Flags & ConnectionLaneFlags.Parking) != 0)
		{
			result.m_Methods |= PathMethod.Taxi;
			result.m_Flags |= EdgeFlags.FreeForward;
			TryAddCosts(ref result.m_Costs, connectionPathfindData.m_TaxiStartCost);
		}
		else
		{
			TryAddCosts(ref result.m_Costs, ((connectionLaneData.m_Flags & ConnectionLaneFlags.Pedestrian) != 0) ? connectionPathfindData.m_PedestrianBorderCost : connectionPathfindData.m_BorderCost, (connectionLaneData.m_Flags & ConnectionLaneFlags.Start) != 0);
			TryAddCosts(ref result.m_Costs, connectionPathfindData.m_DistanceCost, math.select(0f, curveData.m_Length, (connectionLaneData.m_Flags & ConnectionLaneFlags.Distance) != 0));
		}
		return result;
	}

	public static PathSpecification GetTransportStopSpecification(Game.Routes.TransportStop transportStop, TransportLine transportLine, WaitingPassengers waitingPassengers, TransportLineData transportLineData, PathfindTransportData transportPathfindData, bool isWaypoint)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		PathSpecification result = new PathSpecification
		{
			m_Flags = EdgeFlags.FreeBackward,
			m_Length = 0f,
			m_MaxSpeed = 1f,
			m_Density = 0f,
			m_AccessRequirement = math.select(-1, transportStop.m_AccessRestriction.Index, transportStop.m_AccessRestriction != Entity.Null)
		};
		if ((transportStop.m_Flags & StopFlags.Active) != 0)
		{
			result.m_Flags |= EdgeFlags.Forward;
		}
		if ((transportStop.m_Flags & StopFlags.AllowEnter) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowEnter;
		}
		if (!isWaypoint)
		{
			result.m_Flags |= EdgeFlags.Backward;
		}
		if (transportStop.m_AccessRestriction != Entity.Null)
		{
			result.m_Flags |= EdgeFlags.AllowExit;
		}
		if (transportLineData.m_PassengerTransport)
		{
			result.m_Methods |= PathMethod.PublicTransportDay | PathMethod.PublicTransportNight;
		}
		if (transportLineData.m_CargoTransport)
		{
			result.m_Methods |= PathMethod.CargoTransport;
		}
		float stopDuration = RouteUtils.GetStopDuration(transportLineData, transportStop);
		float num = math.max(transportLine.m_VehicleInterval * 0.5f, (float)(int)waitingPassengers.m_AverageWaitingTime) - stopDuration;
		transportPathfindData.m_StartingCost.m_Value.x = math.max(0f, transportPathfindData.m_StartingCost.m_Value.x + num);
		transportPathfindData.m_StartingCost.m_Value.z *= (int)transportLine.m_TicketPrice;
		transportPathfindData.m_StartingCost.m_Value.w *= 1f - transportStop.m_ComfortFactor;
		TryAddCosts(ref result.m_Costs, transportPathfindData.m_StartingCost);
		return result;
	}

	public static PathSpecification GetTaxiStopSpecification(Game.Routes.TransportStop transportStop, TaxiStand taxiStand, WaitingPassengers waitingPassengers, PathfindTransportData transportPathfindData)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		PathSpecification result = new PathSpecification
		{
			m_Flags = EdgeFlags.SecondaryEnd,
			m_Methods = PathMethod.Taxi,
			m_Length = 0f,
			m_MaxSpeed = 1f,
			m_Density = 0f,
			m_AccessRequirement = math.select(-1, transportStop.m_AccessRestriction.Index, transportStop.m_AccessRestriction != Entity.Null)
		};
		if ((transportStop.m_Flags & StopFlags.Active) != 0)
		{
			result.m_Flags |= EdgeFlags.Forward;
		}
		if ((transportStop.m_Flags & StopFlags.AllowEnter) != 0)
		{
			result.m_Flags |= EdgeFlags.AllowEnter;
		}
		if (transportStop.m_AccessRestriction != Entity.Null)
		{
			result.m_Flags |= EdgeFlags.AllowExit;
		}
		transportPathfindData.m_StartingCost.m_Value.x += (int)waitingPassengers.m_AverageWaitingTime;
		transportPathfindData.m_StartingCost.m_Value.z *= (int)taxiStand.m_StartingFee;
		transportPathfindData.m_StartingCost.m_Value.w *= 1f - transportStop.m_ComfortFactor;
		TryAddCosts(ref result.m_Costs, transportPathfindData.m_StartingCost);
		return result;
	}

	public static PathSpecification GetSpawnLocationSpecification(PathfindPedestrianData pedestrianPathfindData, float distance, Entity accessRestriction, bool requireAuthorization, bool allowEnter, bool allowExit)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		PathSpecification result = new PathSpecification
		{
			m_Flags = (EdgeFlags.Forward | EdgeFlags.Backward),
			m_Methods = PathMethod.Pedestrian,
			m_Length = distance,
			m_MaxSpeed = 5.555556f,
			m_AccessRequirement = math.select(-1, accessRestriction.Index, accessRestriction != Entity.Null)
		};
		if (requireAuthorization)
		{
			result.m_Flags |= EdgeFlags.RequireAuthorization;
		}
		if (allowEnter)
		{
			result.m_Flags |= EdgeFlags.AllowEnter;
		}
		if (allowExit)
		{
			result.m_Flags |= EdgeFlags.AllowExit;
		}
		TryAddCosts(ref result.m_Costs, pedestrianPathfindData.m_SpawnCost);
		return result;
	}

	public static PathSpecification GetSpawnLocationSpecification(RouteConnectionType connectionType, PathfindCarData carPathfindData, Game.Net.CarLane carLane, float distance, int laneCrossCount, Entity accessRestriction, bool requireAuthorization, bool allowEnter, bool allowExit)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		PathSpecification result = new PathSpecification
		{
			m_Flags = (EdgeFlags.Forward | EdgeFlags.Backward),
			m_Length = distance,
			m_MaxSpeed = carLane.m_SpeedLimit,
			m_AccessRequirement = math.select(-1, accessRestriction.Index, accessRestriction != Entity.Null)
		};
		if (requireAuthorization)
		{
			result.m_Flags |= EdgeFlags.RequireAuthorization;
		}
		if (allowEnter)
		{
			result.m_Flags |= EdgeFlags.AllowEnter;
		}
		if (allowExit)
		{
			result.m_Flags |= EdgeFlags.AllowExit;
		}
		switch (connectionType)
		{
		case RouteConnectionType.Cargo:
			result.m_Methods = PathMethod.CargoLoading;
			result.m_Flags |= EdgeFlags.SingleOnly;
			break;
		case RouteConnectionType.Road:
		case RouteConnectionType.Parking:
			result.m_Methods = PathMethod.Road;
			result.m_Flags |= EdgeFlags.SingleOnly;
			break;
		}
		TryAddCosts(ref result.m_Costs, carPathfindData.m_SpawnCost);
		TryAddCosts(ref result.m_Costs, carPathfindData.m_DrivingCost, distance);
		TryAddCosts(ref result.m_Costs, carPathfindData.m_LaneCrossCost, (float)laneCrossCount);
		return result;
	}

	public static PathSpecification GetSpawnLocationSpecification(PathfindTrackData trackPathfindData, Entity accessRestriction)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		PathSpecification result = new PathSpecification
		{
			m_Flags = (EdgeFlags.Forward | EdgeFlags.Backward),
			m_Methods = PathMethod.Track,
			m_MaxSpeed = 1f,
			m_AccessRequirement = math.select(-1, accessRestriction.Index, accessRestriction != Entity.Null)
		};
		TryAddCosts(ref result.m_Costs, trackPathfindData.m_SpawnCost);
		return result;
	}

	public static PathSpecification GetSpawnLocationSpecification(RouteConnectionType connectionType, PathfindConnectionData connectionPathfindData, RoadTypes roadType, float distance, Entity accessRestriction, bool requireAuthorization, bool allowEnter, bool allowExit)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		PathSpecification result = new PathSpecification
		{
			m_Flags = (EdgeFlags.Forward | EdgeFlags.Backward),
			m_AccessRequirement = math.select(-1, accessRestriction.Index, accessRestriction != Entity.Null)
		};
		if (requireAuthorization)
		{
			result.m_Flags |= EdgeFlags.RequireAuthorization;
		}
		if (allowEnter)
		{
			result.m_Flags |= EdgeFlags.AllowEnter;
		}
		if (allowExit)
		{
			result.m_Flags |= EdgeFlags.AllowExit;
		}
		switch (connectionType)
		{
		case RouteConnectionType.Pedestrian:
			result.m_Methods = PathMethod.Pedestrian;
			result.m_Length = distance;
			result.m_MaxSpeed = 5.555556f;
			TryAddCosts(ref result.m_Costs, connectionPathfindData.m_PedestrianSpawnCost);
			break;
		case RouteConnectionType.Road:
			result.m_Methods = PathMethod.Road;
			result.m_Flags |= EdgeFlags.SingleOnly;
			if (roadType == RoadTypes.Car)
			{
				result.m_Length = distance;
				result.m_MaxSpeed = 3f;
				TryAddCosts(ref result.m_Costs, connectionPathfindData.m_CarSpawnCost);
			}
			else
			{
				result.m_MaxSpeed = 1f;
			}
			break;
		case RouteConnectionType.Track:
			result.m_Methods = PathMethod.Track;
			result.m_MaxSpeed = 1f;
			break;
		case RouteConnectionType.Air:
			result.m_Methods = PathMethod.Flying;
			switch (roadType)
			{
			case RoadTypes.Helicopter:
				result.m_Length = 750f;
				result.m_MaxSpeed = 83.333336f;
				TryAddCosts(ref result.m_Costs, connectionPathfindData.m_HelicopterTakeoffCost);
				break;
			case RoadTypes.Airplane:
				result.m_Length = 1500f;
				result.m_MaxSpeed = 277.77777f;
				TryAddCosts(ref result.m_Costs, connectionPathfindData.m_AirplaneTakeoffCost);
				break;
			default:
				result.m_MaxSpeed = 1f;
				break;
			}
			break;
		}
		return result;
	}

	public static PathSpecification GetTransportLineSpecification(TransportLineData transportLineData, PathfindTransportData transportPathfindData, RouteInfo routeInfo)
	{
		PathSpecification result = new PathSpecification
		{
			m_Length = routeInfo.m_Distance,
			m_MaxSpeed = math.max(1f, routeInfo.m_Distance) / math.max(1f, routeInfo.m_Duration),
			m_Density = 1f,
			m_AccessRequirement = -1
		};
		if (routeInfo.m_Distance > 0f && routeInfo.m_Duration > 0f)
		{
			result.m_Flags |= EdgeFlags.Forward;
		}
		if (transportLineData.m_PassengerTransport)
		{
			if ((routeInfo.m_Flags & RouteInfoFlags.InactiveDay) == 0)
			{
				result.m_Methods |= PathMethod.PublicTransportDay;
			}
			if ((routeInfo.m_Flags & RouteInfoFlags.InactiveNight) == 0)
			{
				result.m_Methods |= PathMethod.PublicTransportNight;
			}
		}
		if (transportLineData.m_CargoTransport && (routeInfo.m_Flags & (RouteInfoFlags)3) != (RouteInfoFlags)3)
		{
			result.m_Methods |= PathMethod.CargoTransport;
		}
		TryAddCosts(ref result.m_Costs, transportPathfindData.m_TravelCost, routeInfo.m_Distance);
		return result;
	}

	public static LocationSpecification GetLocationSpecification(Curve curveData)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		return new LocationSpecification
		{
			m_Line = new Segment(curveData.m_Bezier.a, curveData.m_Bezier.d)
		};
	}

	public static LocationSpecification GetLocationSpecification(Curve curveData, Game.Net.ParkingLane parkingLaneData)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		LocationSpecification result = default(LocationSpecification);
		float3 val = MathUtils.Position(curveData.m_Bezier, 0.5f);
		result.m_Line = new Segment(val, val);
		return result;
	}

	public static LocationSpecification GetLocationSpecification(float3 position)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return new LocationSpecification
		{
			m_Line = new Segment(position, position)
		};
	}

	public static LocationSpecification GetLocationSpecification(float3 position1, float3 position2)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return new LocationSpecification
		{
			m_Line = new Segment(position1, position2)
		};
	}

	public static void UpdateOwnedVehicleMethods(Entity householdEntity, ref BufferLookup<OwnedVehicle> ownedVehicleBuffs, ref PathfindParameters parameters, ref SetupQueueTarget origin, ref SetupQueueTarget destination)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<OwnedVehicle> val = default(DynamicBuffer<OwnedVehicle>);
		if (ownedVehicleBuffs.TryGetBuffer(householdEntity, ref val) && val.Length != 0)
		{
			parameters.m_Methods |= PathMethod.Road | PathMethod.Parking | PathMethod.MediumRoad;
			parameters.m_ParkingSize = float2.op_Implicit(float.MinValue);
			parameters.m_IgnoredRules |= RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidSlowTraffic;
			origin.m_Methods |= PathMethod.Road | PathMethod.MediumRoad;
			origin.m_RoadTypes |= RoadTypes.Car;
			destination.m_Methods |= PathMethod.Road | PathMethod.MediumRoad;
			destination.m_RoadTypes |= RoadTypes.Car;
		}
	}

	public static bool IsPathfindingPurpose(Purpose purpose)
	{
		switch (purpose)
		{
		case Purpose.GoingHome:
		case Purpose.Hospital:
		case Purpose.Safety:
		case Purpose.EmergencyShelter:
		case Purpose.Crime:
		case Purpose.Escape:
		case Purpose.Sightseeing:
		case Purpose.VisitAttractions:
			return true;
		default:
			return false;
		}
	}
}
