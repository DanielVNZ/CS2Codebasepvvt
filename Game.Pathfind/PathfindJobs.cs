using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Pathfind;

public static class PathfindJobs
{
	private struct FullNode : IEquatable<FullNode>
	{
		public NodeID m_NodeID;

		public float m_CurvePos;

		public FullNode(NodeID nodeID, float curvePos)
		{
			m_NodeID = nodeID;
			m_CurvePos = curvePos;
		}

		public FullNode(EdgeID edgeID, float curvePos)
		{
			m_NodeID = new NodeID
			{
				m_Index = -4 - (edgeID.m_Index << 2)
			};
			m_CurvePos = curvePos;
		}

		public bool Equals(FullNode other)
		{
			if (m_NodeID.Equals(other.m_NodeID))
			{
				return m_CurvePos == other.m_CurvePos;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (m_NodeID.m_Index >> 2) ^ math.asint(m_CurvePos);
		}
	}

	[Flags]
	private enum PathfindItemFlags : ushort
	{
		End = 1,
		SingleOnly = 2,
		NextEdge = 4,
		ReducedCost = 8,
		ForbidExit = 0x10,
		ReducedAccess = 0x20,
		Forward = 0x40,
		Backward = 0x80
	}

	private struct NodeData
	{
		public FullNode m_PathNode;

		public int m_NextNodeIndex;

		public int m_SourceIndex;

		public float m_TotalCost;

		public float m_BaseCost;

		public int m_AccessRequirement;

		public EdgeID m_EdgeID;

		public EdgeID m_NextID;

		public float2 m_EdgeDelta;

		public PathfindItemFlags m_Flags;

		public PathMethod m_Method;

		public NodeData(int sourceIndex, float totalCost, float baseCost, int accessRequirement, EdgeID edgeID, EdgeID nextID, float2 edgeDelta, FullNode pathNode, PathfindItemFlags flags, PathMethod method)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			m_PathNode = pathNode;
			m_NextNodeIndex = -1;
			m_SourceIndex = sourceIndex;
			m_TotalCost = totalCost;
			m_BaseCost = baseCost;
			m_AccessRequirement = accessRequirement;
			m_EdgeID = edgeID;
			m_NextID = nextID;
			m_EdgeDelta = edgeDelta;
			m_Flags = flags;
			m_Method = method;
		}
	}

	private struct HeapData : ILessThan<HeapData>, IComparable<HeapData>
	{
		public float m_TotalCost;

		public int m_NodeIndex;

		public HeapData(float totalCost, int nodeIndex)
		{
			m_TotalCost = totalCost;
			m_NodeIndex = nodeIndex;
		}

		public bool LessThan(HeapData other)
		{
			return m_TotalCost < other.m_TotalCost;
		}

		public int CompareTo(HeapData other)
		{
			return m_NodeIndex - other.m_NodeIndex;
		}
	}

	private struct TargetData
	{
		public Entity m_Entity;

		public float m_Cost;

		public TargetData(Entity entity, float cost)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Entity = entity;
			m_Cost = cost;
		}
	}

	private struct PathfindExecutor
	{
		private UnsafePathfindData m_PathfindData;

		private Allocator m_Allocator;

		private Random m_Random;

		private PathfindParameters m_Parameters;

		private Bounds3 m_StartBounds;

		private Bounds3 m_EndBounds;

		private int3 m_AccessMask;

		private int2 m_AuthorizationMask;

		private Entity m_ParkingOwner;

		private float m_HeuristicCostFactor;

		private float m_MaxTotalCost;

		private float m_CostOffset;

		private float m_ReducedCostFactor;

		private float2 m_ParkingSize;

		private int m_MaxResultCount;

		private bool m_InvertPath;

		private bool m_ParkingReset;

		private EdgeFlags m_Forward;

		private EdgeFlags m_Backward;

		private EdgeFlags m_ForwardMiddle;

		private EdgeFlags m_BackwardMiddle;

		private EdgeFlags m_FreeForward;

		private EdgeFlags m_FreeBackward;

		private EdgeFlags m_ParkingEdgeMask;

		private PathMethod m_ParkingMethodMask;

		private const int NODE_INDEX_SHIFT = 4;

		private UnsafeHashMap<FullNode, TargetData> m_StartTargets;

		private UnsafeHashMap<FullNode, TargetData> m_EndTargets;

		private UnsafeList<int> m_NodeIndex;

		private UnsafeList<int> m_NodeIndexBits;

		private UnsafeMinHeap<HeapData> m_Heap;

		private UnsafeList<NodeData> m_NodeData;

		public void Initialize(NativePathfindData pathfindData, Allocator allocator, Random random, PathfindParameters parameters, PathfindHeuristicData pathfindHeuristicData, float maxPassengerTransportSpeed, float maxCargoTransportSpeed)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			m_PathfindData = pathfindData.GetReadOnlyData();
			m_Allocator = allocator;
			m_Random = random;
			m_Parameters = parameters;
			m_ReducedCostFactor = 1f;
			if ((parameters.m_PathfindFlags & PathfindFlags.ParkingReset) != 0)
			{
				m_ReducedCostFactor = 0.5f;
				m_ParkingReset = true;
			}
			if ((parameters.m_PathfindFlags & PathfindFlags.NoHeuristics) != 0)
			{
				m_HeuristicCostFactor = 0f;
			}
			else
			{
				m_HeuristicCostFactor = 1000000f;
				if ((parameters.m_Methods & PathMethod.Pedestrian) != 0)
				{
					PathfindCosts pedestrianCosts = pathfindHeuristicData.m_PedestrianCosts;
					pedestrianCosts.m_Value.x += 1f / math.max(0.01f, parameters.m_WalkSpeed.x);
					m_HeuristicCostFactor = math.min(m_HeuristicCostFactor, math.dot(pedestrianCosts.m_Value, parameters.m_Weights.m_Value));
				}
				if ((parameters.m_Methods & (PathMethod.Road | PathMethod.MediumRoad)) != 0)
				{
					PathfindCosts carCosts = pathfindHeuristicData.m_CarCosts;
					carCosts.m_Value.x += 1f / math.max(0.01f, parameters.m_MaxSpeed.x);
					m_HeuristicCostFactor = math.min(m_HeuristicCostFactor, math.dot(carCosts.m_Value, parameters.m_Weights.m_Value));
				}
				if ((parameters.m_Methods & PathMethod.Track) != 0)
				{
					PathfindCosts trackCosts = pathfindHeuristicData.m_TrackCosts;
					trackCosts.m_Value.x += 1f / math.max(0.01f, parameters.m_MaxSpeed.x);
					m_HeuristicCostFactor = math.min(m_HeuristicCostFactor, math.dot(trackCosts.m_Value, parameters.m_Weights.m_Value));
				}
				if ((parameters.m_Methods & PathMethod.Flying) != 0)
				{
					PathfindCosts flyingCosts = pathfindHeuristicData.m_FlyingCosts;
					flyingCosts.m_Value.x += 1f / math.max(0.01f, parameters.m_MaxSpeed.x);
					m_HeuristicCostFactor = math.min(m_HeuristicCostFactor, math.dot(flyingCosts.m_Value, parameters.m_Weights.m_Value));
				}
				if ((parameters.m_Methods & PathMethod.Offroad) != 0)
				{
					PathfindCosts offRoadCosts = pathfindHeuristicData.m_OffRoadCosts;
					offRoadCosts.m_Value.x += 1f / math.max(0.01f, parameters.m_MaxSpeed.x);
					m_HeuristicCostFactor = math.min(m_HeuristicCostFactor, math.dot(offRoadCosts.m_Value, parameters.m_Weights.m_Value));
				}
				if ((parameters.m_Methods & PathMethod.Taxi) != 0)
				{
					PathfindCosts taxiCosts = pathfindHeuristicData.m_TaxiCosts;
					taxiCosts.m_Value.x += 1f / math.max(0.01f, 111.111115f);
					m_HeuristicCostFactor = math.min(m_HeuristicCostFactor, math.dot(taxiCosts.m_Value, parameters.m_Weights.m_Value));
				}
				if ((parameters.m_Methods & (PathMethod.PublicTransportDay | PathMethod.PublicTransportNight)) != 0)
				{
					PathfindCosts pathfindCosts = default(PathfindCosts);
					pathfindCosts.m_Value.x += 1f / math.max(0.01f, maxPassengerTransportSpeed);
					m_HeuristicCostFactor = math.min(m_HeuristicCostFactor, math.dot(pathfindCosts.m_Value, parameters.m_Weights.m_Value));
				}
				if ((parameters.m_Methods & PathMethod.CargoTransport) != 0)
				{
					PathfindCosts pathfindCosts2 = default(PathfindCosts);
					pathfindCosts2.m_Value.x += 1f / math.max(0.01f, maxCargoTransportSpeed);
					m_HeuristicCostFactor = math.min(m_HeuristicCostFactor, math.dot(pathfindCosts2.m_Value, parameters.m_Weights.m_Value));
				}
				if ((parameters.m_PathfindFlags & PathfindFlags.Stable) == 0)
				{
					m_HeuristicCostFactor *= 2f;
				}
				m_HeuristicCostFactor *= m_ReducedCostFactor;
			}
			if (parameters.m_ParkingTarget != Entity.Null && parameters.m_ParkingDelta >= 0f)
			{
				m_ParkingOwner = parameters.m_ParkingTarget;
			}
			m_ParkingSize = (((m_Parameters.m_Methods & PathMethod.Boarding) == 0) ? m_Parameters.m_ParkingSize : float2.op_Implicit(float.MinValue));
			m_ParkingEdgeMask = ((m_Parameters.m_ParkingTarget == Entity.Null) ? EdgeFlags.OutsideConnection : (~(EdgeFlags.DefaultMask | EdgeFlags.Secondary)));
			int num = (math.max(m_PathfindData.GetNodeIDSize(), m_PathfindData.m_Edges.Length << 2) >> 4) + 1;
			int num2 = (num >> 5) + 1;
			m_NodeIndex = new UnsafeList<int>(num, AllocatorHandle.op_Implicit(allocator), (NativeArrayOptions)0);
			m_NodeIndexBits = new UnsafeList<int>(num2, AllocatorHandle.op_Implicit(allocator), (NativeArrayOptions)0);
			m_Heap = new UnsafeMinHeap<HeapData>(1000, allocator);
			m_NodeData = new UnsafeList<NodeData>(10000, AllocatorHandle.op_Implicit(allocator), (NativeArrayOptions)0);
			m_NodeIndex.Resize(num, (NativeArrayOptions)0);
			m_NodeIndexBits.Resize(num2, (NativeArrayOptions)1);
		}

		public void Release()
		{
			if (m_NodeData.IsCreated)
			{
				m_NodeIndex.Dispose();
				m_NodeIndexBits.Dispose();
				m_Heap.Dispose();
				m_NodeData.Dispose();
			}
			if (m_EndTargets.IsCreated)
			{
				m_EndTargets.Dispose();
			}
			if (m_StartTargets.IsCreated)
			{
				m_StartTargets.Dispose();
			}
		}

		public void AddTargets(UnsafeList<PathTarget> startTargets, UnsafeList<PathTarget> endTargets, ref ErrorCode errorCode)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			m_StartBounds = GetTargetBounds(startTargets, out var minCost, out var accessRequirement, out var multipleRequirements);
			m_EndBounds = GetTargetBounds(endTargets, out var minCost2, out var accessRequirement2, out var multipleRequirements2);
			if (multipleRequirements && multipleRequirements2)
			{
				multipleRequirements = (m_Parameters.m_PathfindFlags & (PathfindFlags.IgnoreExtraStartAccessRequirements | PathfindFlags.IgnoreExtraEndAccessRequirements)) != PathfindFlags.IgnoreExtraStartAccessRequirements;
				multipleRequirements2 = (m_Parameters.m_PathfindFlags & PathfindFlags.IgnoreExtraEndAccessRequirements) == 0;
			}
			if ((m_Parameters.m_PathfindFlags & PathfindFlags.ForceForward) != 0)
			{
				m_InvertPath = false;
			}
			else if ((m_Parameters.m_PathfindFlags & PathfindFlags.ForceBackward) != 0)
			{
				m_InvertPath = true;
			}
			else if ((m_Parameters.m_PathfindFlags & PathfindFlags.MultipleDestinations) != 0)
			{
				m_InvertPath = false;
			}
			else if ((m_Parameters.m_PathfindFlags & PathfindFlags.MultipleOrigins) != 0)
			{
				m_InvertPath = true;
			}
			else if (multipleRequirements)
			{
				m_InvertPath = false;
			}
			else if (multipleRequirements2)
			{
				m_InvertPath = true;
			}
			else
			{
				m_InvertPath = math.lengthsq(MathUtils.Size(m_StartBounds)) < math.lengthsq(MathUtils.Size(m_EndBounds));
			}
			if (m_InvertPath)
			{
				CommonUtils.Swap(ref startTargets, ref endTargets);
				CommonUtils.Swap(ref m_StartBounds, ref m_EndBounds);
				CommonUtils.Swap(ref minCost, ref minCost2);
				CommonUtils.Swap(ref accessRequirement, ref accessRequirement2);
				CommonUtils.Swap(ref multipleRequirements, ref multipleRequirements2);
				m_PathfindData.SwapConnections();
				m_Forward = EdgeFlags.Backward;
				m_Backward = EdgeFlags.Forward;
				m_FreeForward = EdgeFlags.FreeBackward;
				m_FreeBackward = EdgeFlags.FreeForward;
				m_ParkingMethodMask = PathMethod.Road | PathMethod.Track | PathMethod.Flying | PathMethod.Offroad | PathMethod.MediumRoad;
			}
			else
			{
				m_Forward = EdgeFlags.Forward;
				m_Backward = EdgeFlags.Backward;
				m_FreeForward = EdgeFlags.FreeForward;
				m_FreeBackward = EdgeFlags.FreeBackward;
				m_ParkingMethodMask = PathMethod.Pedestrian | PathMethod.PublicTransportDay | PathMethod.Taxi | PathMethod.PublicTransportNight;
			}
			if (multipleRequirements2)
			{
				errorCode = ErrorCode.TooManyEndAccessRequirements;
			}
			if (((m_Parameters.m_PathfindFlags & PathfindFlags.MultipleDestinations) != 0 && m_InvertPath) || ((m_Parameters.m_PathfindFlags & PathfindFlags.MultipleOrigins) != 0 && !m_InvertPath))
			{
				errorCode = ErrorCode.MultipleStartResults;
			}
			m_AccessMask = new int3(-1, -1, accessRequirement2);
			EdgeID edgeID = default(EdgeID);
			if (m_PathfindData.m_PathEdges.TryGetValue(m_Parameters.m_ParkingTarget, ref edgeID))
			{
				ref Edge edge = ref m_PathfindData.GetEdge(edgeID);
				m_AccessMask.y = edge.m_Specification.m_AccessRequirement;
			}
			m_AuthorizationMask = math.select(int2.op_Implicit(-2), new int2(m_Parameters.m_Authorization1.Index, m_Parameters.m_Authorization2.Index), new bool2(m_Parameters.m_Authorization1 != Entity.Null, m_Parameters.m_Authorization2 != Entity.Null));
			m_ForwardMiddle = m_Forward | EdgeFlags.AllowMiddle;
			m_BackwardMiddle = m_Backward | EdgeFlags.AllowMiddle;
			AddEndTargets(endTargets, minCost2);
			AddStartTargets(startTargets, minCost);
			m_CostOffset = minCost + minCost2;
			m_MaxTotalCost = math.select(m_Parameters.m_MaxCost - m_CostOffset, float.MaxValue, m_Parameters.m_MaxCost == 0f);
			m_MaxResultCount = math.select(1, m_Parameters.m_MaxResultCount, m_Parameters.m_MaxResultCount > 1 && (m_Parameters.m_PathfindFlags & (PathfindFlags.MultipleOrigins | PathfindFlags.MultipleDestinations)) != 0);
		}

		public Bounds3 GetTargetBounds(UnsafeList<PathTarget> pathTargets, out float minCost, out int accessRequirement, out bool multipleRequirements)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			Bounds3 val = default(Bounds3);
			((Bounds3)(ref val))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			int length = pathTargets.Length;
			minCost = float.MaxValue;
			accessRequirement = -1;
			multipleRequirements = false;
			EdgeID edgeID = default(EdgeID);
			for (int i = 0; i < length; i++)
			{
				PathTarget pathTarget = pathTargets[i];
				if ((pathTarget.m_Flags & EdgeFlags.Secondary) != 0)
				{
					if (!m_PathfindData.m_SecondaryEdges.TryGetValue(pathTarget.m_Entity, ref edgeID))
					{
						continue;
					}
				}
				else if (!m_PathfindData.m_PathEdges.TryGetValue(pathTarget.m_Entity, ref edgeID))
				{
					continue;
				}
				ref Edge edge = ref m_PathfindData.GetEdge(edgeID);
				val |= MathUtils.Position(edge.m_Location.m_Line, pathTarget.m_Delta);
				minCost = math.min(minCost, pathTarget.m_Cost);
				if ((edge.m_Specification.m_AccessRequirement != -1) & (edge.m_Specification.m_AccessRequirement != accessRequirement) & ((edge.m_Specification.m_Flags & (EdgeFlags.AllowEnter | EdgeFlags.AllowExit)) != EdgeFlags.AllowEnter))
				{
					multipleRequirements = accessRequirement != -1;
					accessRequirement = math.select(edge.m_Specification.m_AccessRequirement, accessRequirement, multipleRequirements);
				}
			}
			return val;
		}

		private void AddEndTargets(UnsafeList<PathTarget> pathTargets, float minCost)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			int length = pathTargets.Length;
			m_EndTargets = new UnsafeHashMap<FullNode, TargetData>(length, AllocatorHandle.op_Implicit(m_Allocator));
			EdgeID edgeID = default(EdgeID);
			for (int i = 0; i < length; i++)
			{
				PathTarget pathTarget = pathTargets[i];
				pathTarget.m_Cost -= minCost;
				if ((pathTarget.m_Flags & EdgeFlags.Secondary) != 0)
				{
					if (!m_PathfindData.m_SecondaryEdges.TryGetValue(pathTarget.m_Entity, ref edgeID))
					{
						continue;
					}
				}
				else if (!m_PathfindData.m_PathEdges.TryGetValue(pathTarget.m_Entity, ref edgeID))
				{
					continue;
				}
				FullNode fullNode = new FullNode(edgeID, pathTarget.m_Delta);
				TargetData targetData = new TargetData(pathTarget.m_Target, pathTarget.m_Cost);
				if (!m_EndTargets.TryAdd(fullNode, targetData))
				{
					TargetData targetData2 = m_EndTargets[fullNode];
					if (targetData.m_Cost < targetData2.m_Cost)
					{
						m_EndTargets[fullNode] = targetData;
					}
				}
				if (!GetOrAddNodeIndex(fullNode, out var _))
				{
					PathfindItemFlags pathfindItemFlags = PathfindItemFlags.End;
					if ((pathTarget.m_Flags & m_Forward) != 0)
					{
						pathfindItemFlags |= PathfindItemFlags.Forward;
					}
					if ((pathTarget.m_Flags & m_Backward) != 0)
					{
						pathfindItemFlags |= PathfindItemFlags.Backward;
					}
					ref UnsafeList<NodeData> nodeData = ref m_NodeData;
					NodeData nodeData2 = new NodeData(-1, float.MaxValue, pathTarget.m_Cost, -1, edgeID, default(EdgeID), float2.op_Implicit(pathTarget.m_Delta), fullNode, pathfindItemFlags, (PathMethod)0);
					nodeData.Add(ref nodeData2);
				}
			}
		}

		private void AddStartTargets(UnsafeList<PathTarget> pathTargets, float minCost)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			int length = pathTargets.Length;
			m_StartTargets = new UnsafeHashMap<FullNode, TargetData>(length, AllocatorHandle.op_Implicit(m_Allocator));
			EdgeID edgeID = default(EdgeID);
			bool3 directions = default(bool3);
			for (int i = 0; i < length; i++)
			{
				PathTarget pathTarget = pathTargets[i];
				pathTarget.m_Cost -= minCost;
				bool flag = (pathTarget.m_Flags & EdgeFlags.Secondary) != 0;
				if (flag)
				{
					if (!m_PathfindData.m_SecondaryEdges.TryGetValue(pathTarget.m_Entity, ref edgeID))
					{
						continue;
					}
				}
				else if (!m_PathfindData.m_PathEdges.TryGetValue(pathTarget.m_Entity, ref edgeID))
				{
					continue;
				}
				ref Edge edge = ref m_PathfindData.GetEdge(edgeID);
				FullNode fullNode = new FullNode(edgeID, pathTarget.m_Delta);
				TargetData targetData = new TargetData(pathTarget.m_Target, pathTarget.m_Cost);
				if (!m_StartTargets.TryAdd(fullNode, targetData))
				{
					TargetData targetData2 = m_StartTargets[fullNode];
					if (targetData.m_Cost < targetData2.m_Cost)
					{
						m_StartTargets[fullNode] = targetData;
					}
				}
				EdgeFlags flags = edge.m_Specification.m_Flags;
				RuleFlags rules = edge.m_Specification.m_Rules;
				flags &= pathTarget.m_Flags;
				rules = (RuleFlags)((uint)rules & (uint)(byte)(~(int)(flag ? m_Parameters.m_SecondaryIgnoredRules : m_Parameters.m_IgnoredRules)));
				((bool3)(ref directions))._002Ector((flags & m_Forward) != 0 || pathTarget.m_Delta == 1f, (flags & EdgeFlags.AllowMiddle) != 0, (flags & m_Backward) != 0 || pathTarget.m_Delta == 0f);
				bool reducedCost = m_ParkingReset && (edge.m_Specification.m_Methods & (PathMethod.Parking | PathMethod.SpecialParking)) != 0;
				bool reducedAccess = edge.m_Specification.m_AccessRequirement != -1 && (edge.m_Specification.m_Flags & (EdgeFlags.AllowEnter | EdgeFlags.AllowExit)) == EdgeFlags.AllowEnter;
				AddConnections(int.MaxValue, edgeID, in edge, flags, rules, pathTarget.m_Cost, pathTarget.m_Delta, directions, reducedCost, forbidExit: false, reducedAccess);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool HeapExtract(out HeapData heapData)
		{
			if (m_Heap.Length != 0)
			{
				heapData = m_Heap.Extract();
				return true;
			}
			heapData = default(HeapData);
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void HeapInsert(HeapData heapData)
		{
			m_Heap.Insert(heapData);
		}

		public bool FindEndNode(out int endNode, out float travelCost, out int graphTraversal)
		{
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			endNode = 0;
			travelCost = -1f;
			graphTraversal = m_NodeData.Length;
			if (m_MaxResultCount == 0)
			{
				return false;
			}
			HeapData heapData;
			int4 val = default(int4);
			while (HeapExtract(out heapData))
			{
				ref NodeData reference = ref m_NodeData.ElementAt(heapData.m_NodeIndex);
				if (reference.m_SourceIndex >= 0)
				{
					continue;
				}
				reference.m_SourceIndex = -1 - reference.m_SourceIndex;
				if (heapData.m_TotalCost > m_MaxTotalCost)
				{
					endNode = 0;
					travelCost = float.MaxValue;
					graphTraversal = m_NodeData.Length;
					return false;
				}
				if ((reference.m_Flags & PathfindItemFlags.End) != 0)
				{
					endNode = heapData.m_NodeIndex;
					travelCost = heapData.m_TotalCost + m_CostOffset;
					graphTraversal = m_NodeData.Length;
					m_MaxResultCount--;
					return true;
				}
				if ((reference.m_Flags & PathfindItemFlags.NextEdge) != 0)
				{
					ref Edge edge = ref m_PathfindData.GetEdge(reference.m_NextID);
					CheckNextEdge(heapData.m_NodeIndex, reference.m_NextID, reference.m_PathNode, reference.m_BaseCost, reference.m_Flags, in edge);
					continue;
				}
				int connectionCount = m_PathfindData.GetConnectionCount(reference.m_PathNode.m_NodeID);
				PathfindItemFlags flags = reference.m_Flags;
				bool flag = (flags & PathfindItemFlags.ForbidExit) != 0;
				bool flag2 = (flags & PathfindItemFlags.ReducedAccess) != 0;
				((int4)(ref val))._002Ector(m_AccessMask, math.select(reference.m_AccessRequirement, -1, flag2));
				FullNode pathNode = reference.m_PathNode;
				float baseCost = reference.m_BaseCost;
				PathMethod method = reference.m_Method;
				EdgeID edgeID = reference.m_EdgeID;
				for (int i = 0; i < connectionCount; i++)
				{
					EdgeID edgeID2 = new EdgeID
					{
						m_Index = m_PathfindData.GetConnection(pathNode.m_NodeID, i)
					};
					int accessRequirement = m_PathfindData.GetAccessRequirement(pathNode.m_NodeID, i);
					if (edgeID.Equals(edgeID2) || math.all(val != accessRequirement))
					{
						continue;
					}
					ref Edge edge2 = ref m_PathfindData.GetEdge(edgeID2);
					EdgeFlags edgeFlags = edge2.m_Specification.m_Flags;
					if (DisallowConnection(method, flags, in edge2.m_Specification, ref edgeFlags, edge2.m_Owner))
					{
						continue;
					}
					bool flag3 = edge2.m_Specification.m_AccessRequirement != reference.m_AccessRequirement;
					if (!(flag && flag3))
					{
						PathfindItemFlags pathfindItemFlags = flags;
						if (flag3 && edge2.m_Specification.m_AccessRequirement != -1)
						{
							pathfindItemFlags |= PathfindItemFlags.ForbidExit | PathfindItemFlags.ReducedAccess;
						}
						if ((edgeFlags & EdgeFlags.AllowExit) != 0)
						{
							pathfindItemFlags &= ~PathfindItemFlags.ForbidExit;
						}
						if ((edgeFlags & EdgeFlags.AllowEnter) == 0)
						{
							pathfindItemFlags &= ~PathfindItemFlags.ReducedAccess;
						}
						CheckNextEdge(heapData.m_NodeIndex, edgeID2, pathNode, baseCost, pathfindItemFlags, in edge2);
					}
				}
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckNextEdge(int sourceIndex, EdgeID nextID, FullNode pathNode, float baseCost, PathfindItemFlags itemFlags, in Edge edge)
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			EdgeFlags flags = edge.m_Specification.m_Flags;
			RuleFlags rules = edge.m_Specification.m_Rules;
			bool flag = (flags & EdgeFlags.Secondary) != 0;
			rules = (RuleFlags)((uint)rules & (uint)(byte)(~(int)(flag ? m_Parameters.m_SecondaryIgnoredRules : m_Parameters.m_IgnoredRules)));
			float curvePos = math.select(edge.m_StartCurvePos, m_Parameters.m_ParkingDelta, edge.m_Owner == m_ParkingOwner);
			float curvePos2 = math.select(edge.m_EndCurvePos, m_Parameters.m_ParkingDelta, edge.m_Owner == m_ParkingOwner);
			float startDelta;
			bool3 directions = default(bool3);
			if (pathNode.Equals(new FullNode(edge.m_StartID, curvePos)))
			{
				startDelta = 0f;
				((bool3)(ref directions))._002Ector((flags & m_Forward) != 0, (flags & m_ForwardMiddle) == m_ForwardMiddle, false);
			}
			else if (pathNode.Equals(new FullNode(edge.m_EndID, curvePos2)))
			{
				startDelta = 1f;
				((bool3)(ref directions))._002Ector(false, (flags & m_BackwardMiddle) == m_BackwardMiddle, (flags & m_Backward) != 0);
			}
			else
			{
				if (!pathNode.m_NodeID.Equals(edge.m_MiddleID))
				{
					return;
				}
				startDelta = pathNode.m_CurvePos;
				((bool3)(ref directions))._002Ector((flags & m_Forward) != 0, (flags & EdgeFlags.AllowMiddle) != 0, (flags & m_Backward) != 0);
			}
			bool reducedCost = m_ParkingReset && ((itemFlags & PathfindItemFlags.ReducedCost) != 0 || (edge.m_Specification.m_Methods & (PathMethod.Parking | PathMethod.SpecialParking)) != 0);
			bool forbidExit = (itemFlags & PathfindItemFlags.ForbidExit) != 0;
			bool reducedAccess = (itemFlags & PathfindItemFlags.ReducedAccess) != 0;
			AddConnections(sourceIndex, nextID, in edge, flags, rules, baseCost, startDelta, directions, reducedCost, forbidExit, reducedAccess);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float CalculateCost(in PathSpecification pathSpecification, EdgeFlags flags, RuleFlags rules, float2 delta)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			float num = PathUtils.CalculateSpeed(in pathSpecification, in m_Parameters);
			float num2 = delta.y - delta.x;
			float num3 = math.select(0f, 1f, (rules & (RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidTransitTraffic | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic)) != 0);
			float4 value = pathSpecification.m_Costs.m_Value;
			((float4)(ref value)).xy = ((float4)(ref value)).xy + pathSpecification.m_Length * new float2(1f / num, num3);
			value.y += math.select(0f, 100f, (flags & EdgeFlags.RequireAuthorization) != 0 != math.any(pathSpecification.m_AccessRequirement == m_AuthorizationMask));
			bool2 val = new float2(num2, 0f) >= new float2(0f, num2);
			val.x &= (flags & m_FreeForward) != 0;
			val.y &= (flags & m_FreeBackward) != 0;
			val.x |= (pathSpecification.m_Methods & m_Parameters.m_Methods) == PathMethod.Boarding;
			((float4)(ref value)).xyz = math.select(((float4)(ref value)).xyz, float3.op_Implicit(0f), math.any(val));
			return math.dot(value, m_Parameters.m_Weights.m_Value) * math.abs(num2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float CalculateTotalCost(in LocationSpecification location, float baseCost, float endDelta)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			float3 val = MathUtils.Position(location.m_Line, endDelta);
			float3 val2 = math.max(m_EndBounds.min - val, val - m_EndBounds.max);
			return baseCost + math.length(math.max(val2, float3.op_Implicit(0f))) * m_HeuristicCostFactor;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddConnections(int sourceIndex, EdgeID id, in Edge edge, EdgeFlags flags, RuleFlags rules, float baseCost, float startDelta, bool3 directions, bool reducedCost, bool forbidExit, bool reducedAccess)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0579: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			PathfindItemFlags pathfindItemFlags = (PathfindItemFlags)0;
			float num = 1f;
			if ((flags & EdgeFlags.SingleOnly) != 0)
			{
				pathfindItemFlags |= PathfindItemFlags.SingleOnly;
			}
			if (reducedCost)
			{
				pathfindItemFlags |= PathfindItemFlags.ReducedCost;
				num = m_ReducedCostFactor;
			}
			if (forbidExit)
			{
				pathfindItemFlags |= PathfindItemFlags.ForbidExit;
			}
			if (reducedAccess)
			{
				pathfindItemFlags |= PathfindItemFlags.ReducedAccess;
			}
			float num2 = num * math.select(((Random)(ref m_Random)).NextFloat(0.5f, 1f), 1f, (m_Parameters.m_PathfindFlags & PathfindFlags.Stable) != 0);
			if (directions.x)
			{
				float2 val = default(float2);
				((float2)(ref val))._002Ector(startDelta, 1f);
				if (IsValidDelta(in edge.m_Specification, rules, val))
				{
					float curvePos = math.select(edge.m_EndCurvePos, m_Parameters.m_ParkingDelta, edge.m_Owner == m_ParkingOwner);
					AddHeapData(pathNode: new FullNode(edge.m_EndID, curvePos), sourceIndex: sourceIndex, id: id, edge: in edge, flags: flags, rules: rules, baseCost: baseCost, costFactor: num2, edgeDelta: val, itemFlags: pathfindItemFlags);
				}
			}
			if (directions.y)
			{
				int connectionCount = m_PathfindData.GetConnectionCount(edge.m_MiddleID);
				if (connectionCount != 0)
				{
					int4 val2 = default(int4);
					((int4)(ref val2))._002Ector(m_AccessMask, math.select(edge.m_Specification.m_AccessRequirement, -1, reducedAccess));
					float2 val3 = default(float2);
					float2 val4 = default(float2);
					for (int i = 0; i < connectionCount; i++)
					{
						EdgeID edgeID = new EdgeID
						{
							m_Index = m_PathfindData.GetConnection(edge.m_MiddleID, i)
						};
						int accessRequirement = m_PathfindData.GetAccessRequirement(edge.m_MiddleID, i);
						if (id.Equals(edgeID) || math.all(val2 != accessRequirement))
						{
							continue;
						}
						ref Edge edge2 = ref m_PathfindData.GetEdge(edgeID);
						EdgeFlags edgeFlags = edge2.m_Specification.m_Flags;
						if (DisallowConnection(edge.m_Specification.m_Methods, pathfindItemFlags, in edge2.m_Specification, ref edgeFlags, edge2.m_Owner))
						{
							continue;
						}
						bool flag = edge2.m_Specification.m_AccessRequirement != edge.m_Specification.m_AccessRequirement;
						if (forbidExit && flag)
						{
							continue;
						}
						PathfindItemFlags pathfindItemFlags2 = pathfindItemFlags;
						if (flag && edge2.m_Specification.m_AccessRequirement != -1)
						{
							pathfindItemFlags2 |= PathfindItemFlags.ForbidExit | PathfindItemFlags.ReducedAccess;
						}
						if ((edgeFlags & EdgeFlags.AllowExit) != 0)
						{
							pathfindItemFlags2 &= ~PathfindItemFlags.ForbidExit;
						}
						if ((edgeFlags & EdgeFlags.AllowEnter) == 0)
						{
							pathfindItemFlags2 &= ~PathfindItemFlags.ReducedAccess;
						}
						if (edge.m_MiddleID.Equals(edge2.m_StartID) & ((edgeFlags & m_Forward) != 0))
						{
							float num3 = math.select(edge2.m_StartCurvePos, m_Parameters.m_ParkingDelta, edge2.m_Owner == m_ParkingOwner);
							if ((directions.x && num3 >= startDelta) | (directions.z && num3 <= startDelta))
							{
								((float2)(ref val3))._002Ector(startDelta, num3);
								if (IsValidDelta(in edge.m_Specification, rules, val3))
								{
									AddHeapData(pathNode: new FullNode(edge2.m_StartID, num3), sourceIndex: sourceIndex, id: id, id2: edgeID, edge: in edge, flags: flags, rules: rules, baseCost: baseCost, costFactor: num2, edgeDelta: val3, itemFlags: pathfindItemFlags2);
								}
							}
						}
						if (!(edge.m_MiddleID.Equals(edge2.m_EndID) & ((edgeFlags & m_Backward) != 0)))
						{
							continue;
						}
						float num4 = math.select(edge2.m_EndCurvePos, m_Parameters.m_ParkingDelta, edge2.m_Owner == m_ParkingOwner);
						if ((directions.x && num4 >= startDelta) | (directions.z && num4 <= startDelta))
						{
							((float2)(ref val4))._002Ector(startDelta, num4);
							if (IsValidDelta(in edge.m_Specification, rules, val4))
							{
								AddHeapData(pathNode: new FullNode(edge2.m_EndID, num4), sourceIndex: sourceIndex, id: id, id2: edgeID, edge: in edge, flags: flags, rules: rules, baseCost: baseCost, costFactor: num2, edgeDelta: val4, itemFlags: pathfindItemFlags2);
							}
						}
					}
				}
			}
			if (directions.z)
			{
				float2 val5 = default(float2);
				((float2)(ref val5))._002Ector(startDelta, 0f);
				if (IsValidDelta(in edge.m_Specification, rules, val5))
				{
					float curvePos2 = math.select(edge.m_StartCurvePos, m_Parameters.m_ParkingDelta, edge.m_Owner == m_ParkingOwner);
					AddHeapData(pathNode: new FullNode(edge.m_StartID, curvePos2), sourceIndex: sourceIndex, id: id, edge: in edge, flags: flags, rules: rules, baseCost: baseCost, costFactor: num2, edgeDelta: val5, itemFlags: pathfindItemFlags);
				}
			}
			FullNode pathNode = new FullNode(id, 0f);
			if (!TryGetFirstNodeIndex(pathNode, out var nodeIndex))
			{
				return;
			}
			float2 val6 = default(float2);
			do
			{
				ref NodeData reference = ref m_NodeData.ElementAt(nodeIndex);
				if (reference.m_PathNode.m_NodeID.Equals(pathNode.m_NodeID) && reference.m_SourceIndex < 0)
				{
					bool2 xz = ((bool3)(ref directions)).xz;
					xz.x &= (reference.m_Flags & PathfindItemFlags.Forward) != 0;
					xz.y &= (reference.m_Flags & PathfindItemFlags.Backward) != 0;
					if ((xz.x & (reference.m_EdgeDelta.y >= startDelta)) | (xz.y & (reference.m_EdgeDelta.y <= startDelta)))
					{
						((float2)(ref val6))._002Ector(startDelta, reference.m_EdgeDelta.y);
						if (IsValidDelta(in edge.m_Specification, rules, val6))
						{
							FullNode fullNode = new FullNode(id, reference.m_EdgeDelta.y);
							float num5 = reference.m_BaseCost;
							if (reference.m_TotalCost < float.MaxValue)
							{
								num5 = m_EndTargets[fullNode].m_Cost;
							}
							float num6 = baseCost + CalculateCost(in edge.m_Specification, flags, rules, val6) * num2;
							float num7 = num6 + num5 * num;
							if (num7 < reference.m_TotalCost)
							{
								PathfindItemFlags pathfindItemFlags3 = reference.m_Flags & (PathfindItemFlags.End | PathfindItemFlags.Forward | PathfindItemFlags.Backward);
								reference = new NodeData(-1 - sourceIndex, num7, num6, edge.m_Specification.m_AccessRequirement, id, default(EdgeID), val6, fullNode, pathfindItemFlags | pathfindItemFlags3, edge.m_Specification.m_Methods);
								HeapInsert(new HeapData(num7, nodeIndex));
							}
						}
					}
				}
				nodeIndex = reference.m_NextNodeIndex;
			}
			while (nodeIndex != -1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TryGetFirstNodeIndex(FullNode pathNode, out int nodeIndex)
		{
			int num = math.abs(pathNode.m_NodeID.m_Index) >> 4;
			int num2 = num >> 5;
			int num3 = 1 << (num & 0x1F);
			nodeIndex = m_NodeIndex[num];
			return (m_NodeIndexBits[num2] & num3) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool GetOrAddNodeIndex(FullNode pathNode, out int nodeIndex)
		{
			int num = math.abs(pathNode.m_NodeID.m_Index) >> 4;
			int num2 = num >> 5;
			int num3 = 1 << (num & 0x1F);
			if ((m_NodeIndexBits[num2] & num3) != 0)
			{
				nodeIndex = m_NodeIndex[num];
				ref NodeData reference = ref m_NodeData.ElementAt(nodeIndex);
				while (true)
				{
					if (reference.m_PathNode.Equals(pathNode))
					{
						return true;
					}
					nodeIndex = reference.m_NextNodeIndex;
					if (nodeIndex == -1)
					{
						break;
					}
					reference = ref m_NodeData.ElementAt(nodeIndex);
				}
				nodeIndex = m_NodeData.Length;
				reference.m_NextNodeIndex = nodeIndex;
			}
			else
			{
				ref UnsafeList<int> nodeIndexBits = ref m_NodeIndexBits;
				int num4 = num2;
				nodeIndexBits[num4] |= num3;
				nodeIndex = m_NodeData.Length;
				m_NodeIndex[num] = nodeIndex;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddHeapData(int sourceIndex, EdgeID id, in Edge edge, EdgeFlags flags, RuleFlags rules, float baseCost, float costFactor, FullNode pathNode, float2 edgeDelta, PathfindItemFlags itemFlags)
		{
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			if (GetOrAddNodeIndex(pathNode, out var nodeIndex))
			{
				ref NodeData reference = ref m_NodeData.ElementAt(nodeIndex);
				if (reference.m_SourceIndex < 0)
				{
					float baseCost2 = baseCost + CalculateCost(in edge.m_Specification, flags, rules, edgeDelta) * costFactor;
					float num = CalculateTotalCost(in edge.m_Location, baseCost2, edgeDelta.y);
					if (num < reference.m_TotalCost)
					{
						reference = new NodeData(-1 - sourceIndex, num, baseCost2, edge.m_Specification.m_AccessRequirement, id, default(EdgeID), edgeDelta, pathNode, itemFlags, edge.m_Specification.m_Methods);
						HeapInsert(new HeapData(num, nodeIndex));
					}
				}
			}
			else
			{
				float baseCost3 = baseCost + CalculateCost(in edge.m_Specification, flags, rules, edgeDelta) * costFactor;
				float totalCost = CalculateTotalCost(in edge.m_Location, baseCost3, edgeDelta.y);
				ref UnsafeList<NodeData> nodeData = ref m_NodeData;
				NodeData nodeData2 = new NodeData(-1 - sourceIndex, totalCost, baseCost3, edge.m_Specification.m_AccessRequirement, id, default(EdgeID), edgeDelta, pathNode, itemFlags, edge.m_Specification.m_Methods);
				nodeData.Add(ref nodeData2);
				HeapInsert(new HeapData(totalCost, nodeIndex));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddHeapData(int sourceIndex, EdgeID id, EdgeID id2, in Edge edge, EdgeFlags flags, RuleFlags rules, float baseCost, float costFactor, FullNode pathNode, float2 edgeDelta, PathfindItemFlags itemFlags)
		{
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			if (GetOrAddNodeIndex(pathNode, out var nodeIndex))
			{
				ref NodeData reference = ref m_NodeData.ElementAt(nodeIndex);
				if (reference.m_SourceIndex < 0)
				{
					float baseCost2 = baseCost + CalculateCost(in edge.m_Specification, flags, rules, edgeDelta) * costFactor;
					float num = CalculateTotalCost(in edge.m_Location, baseCost2, edgeDelta.y);
					if (num < reference.m_TotalCost)
					{
						reference = new NodeData(-1 - sourceIndex, num, baseCost2, edge.m_Specification.m_AccessRequirement, id, id2, edgeDelta, pathNode, itemFlags | PathfindItemFlags.NextEdge, edge.m_Specification.m_Methods);
						HeapInsert(new HeapData(num, nodeIndex));
					}
					else if (!id2.Equals(reference.m_NextID))
					{
						reference.m_NextID = default(EdgeID);
						reference.m_Flags &= ~PathfindItemFlags.NextEdge;
					}
				}
			}
			else
			{
				float baseCost3 = baseCost + CalculateCost(in edge.m_Specification, flags, rules, edgeDelta) * costFactor;
				float totalCost = CalculateTotalCost(in edge.m_Location, baseCost3, edgeDelta.y);
				ref UnsafeList<NodeData> nodeData = ref m_NodeData;
				NodeData nodeData2 = new NodeData(-1 - sourceIndex, totalCost, baseCost3, edge.m_Specification.m_AccessRequirement, id, id2, edgeDelta, pathNode, itemFlags | PathfindItemFlags.NextEdge, edge.m_Specification.m_Methods);
				nodeData.Add(ref nodeData2);
				HeapInsert(new HeapData(totalCost, nodeIndex));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsValidDelta(in PathSpecification spec, RuleFlags rules, float2 delta)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			if ((rules & RuleFlags.HasBlockage) == 0)
			{
				return true;
			}
			if (!(math.min(delta.x, delta.y) > (float)(int)spec.m_BlockageEnd * 0.003921569f))
			{
				return math.max(delta.x, delta.y) < (float)(int)spec.m_BlockageStart * 0.003921569f;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool DisallowConnection(PathMethod prevMethod, PathfindItemFlags itemFlags, in PathSpecification newSpec, ref EdgeFlags edgeFlags, Entity newOwner)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			if ((newSpec.m_Methods & m_Parameters.m_Methods) == 0 || ((itemFlags & PathfindItemFlags.SingleOnly) != 0 && (newSpec.m_Flags & EdgeFlags.SingleOnly) != 0))
			{
				return true;
			}
			if ((newSpec.m_Methods & (PathMethod.Parking | PathMethod.Boarding | PathMethod.SpecialParking)) != 0)
			{
				if ((prevMethod & m_ParkingMethodMask) != 0)
				{
					edgeFlags |= EdgeFlags.AllowExit;
					if (m_Parameters.m_ParkingTarget != newOwner)
					{
						return (newSpec.m_Flags & m_ParkingEdgeMask) == 0;
					}
					return false;
				}
				if ((prevMethod & (PathMethod.Parking | PathMethod.Boarding | PathMethod.SpecialParking)) != 0)
				{
					return true;
				}
				return math.any(m_ParkingSize > new float2(newSpec.m_Density, newSpec.m_MaxSpeed));
			}
			return false;
		}

		public void CreatePath(int endNode, ref UnsafeList<PathfindPath> path, out float distance, out float duration, out int pathLength, out Entity origin, out Entity destination, out PathMethod methods)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			distance = 0f;
			duration = 0f;
			pathLength = 0;
			methods = (PathMethod)0;
			ref NodeData reference = ref m_NodeData.ElementAt(endNode);
			FullNode fullNode = default(FullNode);
			TargetData targetData = default(TargetData);
			m_EndTargets.TryGetValue(reference.m_PathNode, ref targetData);
			destination = targetData.m_Entity;
			PathfindParameters pathfindParameters = m_Parameters;
			PathfindPath pathfindPath = default(PathfindPath);
			while (true)
			{
				ref Edge edge = ref m_PathfindData.GetEdge(reference.m_EdgeID);
				bool flag = (edge.m_Specification.m_Flags & EdgeFlags.OutsideConnection) != 0;
				pathfindParameters.m_MaxSpeed = math.select(pathfindParameters.m_MaxSpeed, float2.op_Implicit(277.77777f), flag);
				pathfindParameters.m_WalkSpeed = math.select(pathfindParameters.m_WalkSpeed, float2.op_Implicit(277.77777f), flag);
				float num = PathUtils.CalculateLength(in edge.m_Specification, reference.m_EdgeDelta);
				float num2 = PathUtils.CalculateSpeed(in edge.m_Specification, in pathfindParameters);
				distance += num;
				duration += num / num2;
				pathLength++;
				methods |= edge.m_Specification.m_Methods & m_Parameters.m_Methods;
				if (path.IsCreated)
				{
					pathfindPath.m_Target = edge.m_Owner;
					pathfindPath.m_TargetDelta = reference.m_EdgeDelta;
					pathfindPath.m_Flags = ~(PathElementFlags.Secondary | PathElementFlags.PathStart | PathElementFlags.Action | PathElementFlags.Return | PathElementFlags.Reverse | PathElementFlags.WaitPosition | PathElementFlags.Leader | PathElementFlags.Hangaround);
					if ((edge.m_Specification.m_Flags & EdgeFlags.Secondary) != 0)
					{
						pathfindPath.m_Flags |= PathElementFlags.Secondary;
					}
					path.Add(ref pathfindPath);
				}
				if (reference.m_SourceIndex == int.MaxValue)
				{
					break;
				}
				reference = ref m_NodeData.ElementAt(reference.m_SourceIndex);
			}
			fullNode = new FullNode(reference.m_EdgeID, reference.m_EdgeDelta.x);
			TargetData targetData2 = default(TargetData);
			m_StartTargets.TryGetValue(fullNode, ref targetData2);
			origin = targetData2.m_Entity;
			if (m_InvertPath)
			{
				CommonUtils.Swap(ref origin, ref destination);
			}
			if (!path.IsCreated || path.Length <= 0)
			{
				return;
			}
			if (m_InvertPath)
			{
				for (int i = 0; i < path.Length; i++)
				{
					ref PathfindPath reference2 = ref path.ElementAt(i);
					reference2.m_TargetDelta = ((float2)(ref reference2.m_TargetDelta)).yx;
				}
			}
			else
			{
				int num3 = 0;
				int num4 = path.Length - 1;
				while (num3 < num4)
				{
					CommonUtils.Swap(ref path.ElementAt(num3++), ref path.ElementAt(num4--));
				}
			}
			path.ElementAt(0).m_Flags |= PathElementFlags.PathStart;
		}
	}

	[BurstCompile]
	public struct PathfindJob : IJob
	{
		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public NativePathfindData m_PathfindData;

		[ReadOnly]
		public PathfindHeuristicData m_PathfindHeuristicData;

		[ReadOnly]
		public float m_MaxPassengerTransportSpeed;

		[ReadOnly]
		public float m_MaxCargoTransportSpeed;

		public PathfindAction m_Action;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			Execute(m_PathfindData, (Allocator)2, m_RandomSeed.GetRandom(0), m_PathfindHeuristicData, m_MaxPassengerTransportSpeed, m_MaxCargoTransportSpeed, ref m_Action.data);
		}

		public static void Execute(NativePathfindData pathfindData, Allocator allocator, Random random, PathfindHeuristicData pathfindHeuristicData, float maxPassengerTransportSpeed, float maxCargoTransportSpeed, ref PathfindActionData actionData)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			PathfindResult pathfindResult = new PathfindResult
			{
				m_Distance = -1f,
				m_Duration = -1f,
				m_TotalCost = -1f
			};
			if (actionData.m_StartTargets.Length == 0 || actionData.m_EndTargets.Length == 0)
			{
				actionData.m_Result.Add(ref pathfindResult);
				return;
			}
			UnsafeList<PathfindPath> val = default(UnsafeList<PathfindPath>);
			ref UnsafeList<PathfindPath> reference = ref val;
			reference = ref actionData.m_Path;
			PathfindParameters parameters = actionData.m_Parameters;
			actionData.m_Result.Capacity = math.max(1, parameters.m_MaxResultCount);
			if ((parameters.m_PathfindFlags & PathfindFlags.SkipPathfind) != 0)
			{
				pathfindResult.m_Distance = 0f;
				pathfindResult.m_Duration = 0f;
				pathfindResult.m_TotalCost = actionData.m_StartTargets[0].m_Cost + actionData.m_EndTargets[0].m_Cost;
				pathfindResult.m_GraphTraversal = 1;
				pathfindResult.m_PathLength = 1;
				pathfindResult.m_Origin = actionData.m_StartTargets[0].m_Entity;
				pathfindResult.m_Destination = actionData.m_EndTargets[0].m_Entity;
			}
			else
			{
				PathfindExecutor pathfindExecutor = default(PathfindExecutor);
				pathfindExecutor.Initialize(pathfindData, allocator, random, parameters, pathfindHeuristicData, maxPassengerTransportSpeed, maxCargoTransportSpeed);
				pathfindExecutor.AddTargets(actionData.m_StartTargets, actionData.m_EndTargets, ref pathfindResult.m_ErrorCode);
				int endNode;
				while (pathfindExecutor.FindEndNode(out endNode, out pathfindResult.m_TotalCost, out pathfindResult.m_GraphTraversal))
				{
					pathfindExecutor.CreatePath(endNode, ref reference, out pathfindResult.m_Distance, out pathfindResult.m_Duration, out pathfindResult.m_PathLength, out pathfindResult.m_Origin, out pathfindResult.m_Destination, out pathfindResult.m_Methods);
					actionData.m_Result.Add(ref pathfindResult);
					pathfindResult = new PathfindResult
					{
						m_Distance = -1f,
						m_Duration = -1f,
						m_TotalCost = -1f
					};
					reference = ref val;
				}
				pathfindExecutor.Release();
			}
			if (actionData.m_Result.Length == 0)
			{
				actionData.m_Result.Add(ref pathfindResult);
			}
		}
	}

	public struct ResultItem
	{
		public Entity m_Owner;

		public UnsafeList<PathfindResult> m_Result;

		public UnsafeList<PathfindPath> m_Path;
	}

	[BurstCompile]
	public struct ProcessResultsJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeList<ResultItem> m_ResultItems;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<PathOwner> m_PathOwner;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<PathInformation> m_PathInformation;

		[NativeDisableParallelForRestriction]
		public BufferLookup<PathElement> m_PathElements;

		[NativeDisableParallelForRestriction]
		public BufferLookup<PathInformations> m_PathInformations;

		public void Execute(int index)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			ResultItem resultItem = m_ResultItems[index];
			bool flag = false;
			PathOwner pathOwner = default(PathOwner);
			DynamicBuffer<PathElement> val2 = default(DynamicBuffer<PathElement>);
			if (m_PathOwner.TryGetComponent(resultItem.m_Owner, ref pathOwner))
			{
				if (resultItem.m_Path.Length == 0 && (pathOwner.m_State & PathFlags.Divert) != 0)
				{
					flag = true;
					pathOwner.m_State |= PathFlags.Failed;
				}
				else
				{
					DynamicBuffer<PathElement> val = default(DynamicBuffer<PathElement>);
					if (m_PathElements.TryGetBuffer(resultItem.m_Owner, ref val))
					{
						if ((pathOwner.m_State & PathFlags.Append) != 0)
						{
							if (pathOwner.m_ElementIndex != 0)
							{
								val.RemoveRange(0, pathOwner.m_ElementIndex);
								pathOwner.m_ElementIndex = 0;
							}
						}
						else
						{
							val.Clear();
							pathOwner.m_ElementIndex = 0;
						}
						if ((pathOwner.m_State & PathFlags.Obsolete) == 0)
						{
							for (int i = 0; i < resultItem.m_Path.Length; i++)
							{
								PathfindPath pathfindPath = resultItem.m_Path[i];
								val.Add(new PathElement
								{
									m_Target = pathfindPath.m_Target,
									m_TargetDelta = pathfindPath.m_TargetDelta,
									m_Flags = pathfindPath.m_Flags
								});
							}
							if ((pathOwner.m_State & PathFlags.AddDestination) != 0)
							{
								PathfindResult pathfindResult = resultItem.m_Result[0];
								val.Add(new PathElement
								{
									m_Target = pathfindResult.m_Destination
								});
							}
						}
					}
					if ((pathOwner.m_State & PathFlags.Obsolete) != 0)
					{
						flag = true;
					}
					else if (resultItem.m_Path.Length == 0)
					{
						pathOwner.m_State |= PathFlags.Failed;
					}
					if ((pathOwner.m_State & PathFlags.Divert) != 0)
					{
						pathOwner.m_State |= PathFlags.CachedObsolete;
					}
					else
					{
						pathOwner.m_State &= ~PathFlags.CachedObsolete;
					}
					pathOwner.m_State |= PathFlags.Updated;
				}
				pathOwner.m_State &= ~PathFlags.Pending;
				m_PathOwner[resultItem.m_Owner] = pathOwner;
			}
			else if (m_PathElements.TryGetBuffer(resultItem.m_Owner, ref val2))
			{
				val2.Clear();
				for (int j = 0; j < resultItem.m_Path.Length; j++)
				{
					PathfindPath pathfindPath2 = resultItem.m_Path[j];
					val2.Add(new PathElement
					{
						m_Target = pathfindPath2.m_Target,
						m_TargetDelta = pathfindPath2.m_TargetDelta,
						m_Flags = pathfindPath2.m_Flags
					});
				}
			}
			if (flag)
			{
				return;
			}
			PathInformation pathInformation = default(PathInformation);
			if (m_PathInformation.TryGetComponent(resultItem.m_Owner, ref pathInformation))
			{
				PathfindResult pathfindResult2 = resultItem.m_Result[0];
				pathInformation.m_Origin = pathfindResult2.m_Origin;
				pathInformation.m_Destination = pathfindResult2.m_Destination;
				pathInformation.m_Distance = pathfindResult2.m_Distance;
				pathInformation.m_Duration = pathfindResult2.m_Duration;
				pathInformation.m_TotalCost = pathfindResult2.m_TotalCost;
				pathInformation.m_Methods = pathfindResult2.m_Methods;
				pathInformation.m_State &= ~PathFlags.Pending;
				m_PathInformation[resultItem.m_Owner] = pathInformation;
			}
			DynamicBuffer<PathInformations> val3 = default(DynamicBuffer<PathInformations>);
			if (m_PathInformations.TryGetBuffer(resultItem.m_Owner, ref val3))
			{
				CollectionUtils.ResizeInitialized<PathInformations>(val3, resultItem.m_Result.Length, val3[0]);
				for (int k = 0; k < resultItem.m_Result.Length; k++)
				{
					PathfindResult pathfindResult3 = resultItem.m_Result[k];
					PathInformations pathInformations = val3[k];
					pathInformations.m_Origin = pathfindResult3.m_Origin;
					pathInformations.m_Destination = pathfindResult3.m_Destination;
					pathInformations.m_Distance = pathfindResult3.m_Distance;
					pathInformations.m_Duration = pathfindResult3.m_Duration;
					pathInformations.m_TotalCost = pathfindResult3.m_TotalCost;
					pathInformations.m_Methods = pathfindResult3.m_Methods;
					pathInformations.m_State &= ~PathFlags.Pending;
					val3[k] = pathInformations;
				}
			}
		}
	}
}
