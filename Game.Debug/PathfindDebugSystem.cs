using Colossal;
using Colossal.Mathematics;
using Game.Pathfind;
using Game.Rendering;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

public class PathfindDebugSystem : BaseDebugSystem
{
	private struct PathfindLine
	{
		public Entity m_Owner;

		public PathFlags m_Flags;

		public float3 m_Time;

		public Segment m_Line;

		public bool m_TooLong;

		public PathfindLine(Entity owner, PathFlags flags, float2 time, Segment line)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			m_Owner = owner;
			m_Flags = flags;
			m_Time = ((float2)(ref time)).xxy;
			m_Line = line;
			m_TooLong = false;
		}
	}

	[BurstCompile]
	private struct EdgeCountJob : IJob
	{
		[ReadOnly]
		public NativePathfindData m_PathfindData;

		public NativeReference<int> m_EdgeCount;

		public void Execute()
		{
			UnsafePathfindData readOnlyData = m_PathfindData.GetReadOnlyData();
			m_EdgeCount.Value = readOnlyData.m_Edges.Length;
		}
	}

	[BurstCompile]
	private struct PathfindEdgeGizmoJob : IJobParallelForDefer
	{
		[ReadOnly]
		public bool m_RestrictedOption;

		[ReadOnly]
		public bool4 m_CostOptions;

		[ReadOnly]
		public NativePathfindData m_PathfindData;

		public GizmoBatcher m_GizmoBatcher;

		public void Execute(int index)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_0579: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_053e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_062d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0639: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0646: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0654: Unknown result type (might be due to invalid IL or missing references)
			//IL_0662: Unknown result type (might be due to invalid IL or missing references)
			//IL_0667: Unknown result type (might be due to invalid IL or missing references)
			//IL_0619: Unknown result type (might be due to invalid IL or missing references)
			//IL_0620: Unknown result type (might be due to invalid IL or missing references)
			//IL_0625: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0733: Unknown result type (might be due to invalid IL or missing references)
			//IL_0738: Unknown result type (might be due to invalid IL or missing references)
			//IL_068a: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0691: Unknown result type (might be due to invalid IL or missing references)
			//IL_081b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0820: Unknown result type (might be due to invalid IL or missing references)
			//IL_0762: Unknown result type (might be due to invalid IL or missing references)
			//IL_076e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0773: Unknown result type (might be due to invalid IL or missing references)
			//IL_077b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0782: Unknown result type (might be due to invalid IL or missing references)
			//IL_0789: Unknown result type (might be due to invalid IL or missing references)
			//IL_0797: Unknown result type (might be due to invalid IL or missing references)
			//IL_079c: Unknown result type (might be due to invalid IL or missing references)
			//IL_074e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0755: Unknown result type (might be due to invalid IL or missing references)
			//IL_075a: Unknown result type (might be due to invalid IL or missing references)
			//IL_084a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0856: Unknown result type (might be due to invalid IL or missing references)
			//IL_085b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0863: Unknown result type (might be due to invalid IL or missing references)
			//IL_086a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0871: Unknown result type (might be due to invalid IL or missing references)
			//IL_087f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0884: Unknown result type (might be due to invalid IL or missing references)
			//IL_0836: Unknown result type (might be due to invalid IL or missing references)
			//IL_083d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0842: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
			UnsafePathfindData readOnlyData = m_PathfindData.GetReadOnlyData();
			ref Edge edge = ref readOnlyData.GetEdge(new EdgeID
			{
				m_Index = index
			});
			if (edge.m_Owner == Entity.Null)
			{
				return;
			}
			Color val = GetEdgeColor(edge.m_Specification, edge.m_Location);
			if ((edge.m_Specification.m_Flags & EdgeFlags.Secondary) != 0)
			{
				val *= 0.5f;
			}
			if (math.lengthsq(edge.m_Location.m_Line.b - edge.m_Location.m_Line.a) > 0.0001f)
			{
				switch (edge.m_Specification.m_Flags & (EdgeFlags.Forward | EdgeFlags.Backward))
				{
				case EdgeFlags.Forward:
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawArrow(edge.m_Location.m_Line.a, edge.m_Location.m_Line.b, val, 1f, 25f, 16);
					break;
				case EdgeFlags.Backward:
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawArrow(edge.m_Location.m_Line.b, edge.m_Location.m_Line.a, val, 1f, 25f, 16);
					break;
				case EdgeFlags.Forward | EdgeFlags.Backward:
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(edge.m_Location.m_Line.a, edge.m_Location.m_Line.b, val);
					break;
				default:
					val *= 0.5f;
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(edge.m_Location.m_Line.a, edge.m_Location.m_Line.b, val);
					break;
				}
			}
			if ((edge.m_Specification.m_Flags & (EdgeFlags.Forward | EdgeFlags.Backward)) != 0)
			{
				bool flag = false;
				bool flag2 = false;
				if ((edge.m_Specification.m_Flags & EdgeFlags.Forward) != 0)
				{
					int reversedConnectionCount = readOnlyData.GetReversedConnectionCount(edge.m_StartID);
					for (int i = 0; i < reversedConnectionCount; i++)
					{
						EdgeID edgeID = new EdgeID
						{
							m_Index = readOnlyData.GetReversedConnection(edge.m_StartID, i)
						};
						if (edgeID.m_Index != index)
						{
							ref Edge edge2 = ref readOnlyData.GetEdge(edgeID);
							if (edge.m_Owner != edge2.m_Owner)
							{
								flag = true;
								break;
							}
						}
					}
					int connectionCount = readOnlyData.GetConnectionCount(edge.m_EndID);
					for (int j = 0; j < connectionCount; j++)
					{
						EdgeID edgeID2 = new EdgeID
						{
							m_Index = readOnlyData.GetConnection(edge.m_EndID, j)
						};
						if (edgeID2.m_Index != index)
						{
							ref Edge edge3 = ref readOnlyData.GetEdge(edgeID2);
							if (edge.m_Owner != edge3.m_Owner)
							{
								flag2 = true;
								break;
							}
						}
					}
				}
				if ((edge.m_Specification.m_Flags & EdgeFlags.Backward) != 0)
				{
					if (!flag)
					{
						int connectionCount2 = readOnlyData.GetConnectionCount(edge.m_StartID);
						for (int k = 0; k < connectionCount2; k++)
						{
							EdgeID edgeID3 = new EdgeID
							{
								m_Index = readOnlyData.GetConnection(edge.m_StartID, k)
							};
							if (edgeID3.m_Index != index)
							{
								ref Edge edge4 = ref readOnlyData.GetEdge(edgeID3);
								if (edge.m_Owner != edge4.m_Owner)
								{
									flag = true;
									break;
								}
							}
						}
					}
					if (!flag2)
					{
						int reversedConnectionCount2 = readOnlyData.GetReversedConnectionCount(edge.m_EndID);
						for (int l = 0; l < reversedConnectionCount2; l++)
						{
							EdgeID edgeID4 = new EdgeID
							{
								m_Index = readOnlyData.GetReversedConnection(edge.m_EndID, l)
							};
							if (edgeID4.m_Index != index)
							{
								ref Edge edge5 = ref readOnlyData.GetEdge(edgeID4);
								if (edge.m_Owner != edge5.m_Owner)
								{
									flag2 = true;
									break;
								}
							}
						}
					}
				}
				if (!flag)
				{
					if ((edge.m_Specification.m_Flags & EdgeFlags.Secondary) != 0)
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(edge.m_Location.m_Line.a, 0.5f, Color.red * 0.5f);
					}
					else
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(edge.m_Location.m_Line.a, 0.5f, Color.red);
					}
				}
				if (!flag2)
				{
					if ((edge.m_Specification.m_Flags & EdgeFlags.Secondary) != 0)
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(edge.m_Location.m_Line.b, 0.5f, Color.red * 0.5f);
					}
					else
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(edge.m_Location.m_Line.b, 0.5f, Color.red);
					}
				}
			}
			if ((edge.m_Specification.m_Flags & EdgeFlags.AllowMiddle) == 0)
			{
				return;
			}
			int connectionCount3 = readOnlyData.GetConnectionCount(edge.m_MiddleID);
			for (int m = 0; m < connectionCount3; m++)
			{
				EdgeID edgeID5 = new EdgeID
				{
					m_Index = readOnlyData.GetConnection(edge.m_MiddleID, m)
				};
				if (edgeID5.m_Index == index)
				{
					continue;
				}
				ref Edge edge6 = ref readOnlyData.GetEdge(edgeID5);
				if ((edge6.m_Specification.m_Flags & EdgeFlags.Forward) != 0 && edge.m_MiddleID.Equals(edge6.m_StartID))
				{
					Color val2 = GetEdgeColor(edge6.m_Specification, edge6.m_Location);
					if ((edge6.m_Specification.m_Flags & EdgeFlags.Secondary) != 0)
					{
						val2 *= 0.5f;
					}
					float3 val3 = MathUtils.Position(edge.m_Location.m_Line, edge6.m_StartCurvePos);
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(val3, 0.5f, val2);
					if (math.lengthsq(val3 - edge6.m_Location.m_Line.a) > 0.0001f)
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(edge6.m_Location.m_Line.a, val3, val2);
					}
				}
				else if ((edge6.m_Specification.m_Flags & EdgeFlags.Backward) != 0 && edge.m_MiddleID.Equals(edge6.m_EndID))
				{
					Color val4 = GetEdgeColor(edge6.m_Specification, edge6.m_Location);
					if ((edge6.m_Specification.m_Flags & EdgeFlags.Secondary) != 0)
					{
						val4 *= 0.5f;
					}
					float3 val5 = MathUtils.Position(edge.m_Location.m_Line, edge6.m_EndCurvePos);
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(val5, 0.5f, val4);
					if (math.lengthsq(val5 - edge6.m_Location.m_Line.b) > 0.0001f)
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(edge6.m_Location.m_Line.b, val5, val4);
					}
				}
			}
			int reversedConnectionCount3 = readOnlyData.GetReversedConnectionCount(edge.m_MiddleID);
			for (int n = 0; n < reversedConnectionCount3; n++)
			{
				EdgeID edgeID6 = new EdgeID
				{
					m_Index = readOnlyData.GetReversedConnection(edge.m_MiddleID, n)
				};
				if (edgeID6.m_Index == index)
				{
					continue;
				}
				ref Edge edge7 = ref readOnlyData.GetEdge(edgeID6);
				if ((edge7.m_Specification.m_Flags & (EdgeFlags.Forward | EdgeFlags.Backward)) == EdgeFlags.Backward && edge.m_MiddleID.Equals(edge7.m_StartID))
				{
					Color val6 = GetEdgeColor(edge7.m_Specification, edge7.m_Location);
					if ((edge7.m_Specification.m_Flags & EdgeFlags.Secondary) != 0)
					{
						val6 *= 0.5f;
					}
					float3 val7 = MathUtils.Position(edge.m_Location.m_Line, edge7.m_StartCurvePos);
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(val7, 0.5f, val6);
					if (math.lengthsq(val7 - edge7.m_Location.m_Line.a) > 0.0001f)
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawArrow(edge7.m_Location.m_Line.a, val7, val6, 1f, 25f, 16);
					}
				}
				else if ((edge7.m_Specification.m_Flags & (EdgeFlags.Forward | EdgeFlags.Backward)) == EdgeFlags.Forward && edge.m_MiddleID.Equals(edge7.m_EndID))
				{
					Color val8 = GetEdgeColor(edge7.m_Specification, edge7.m_Location);
					if ((edge7.m_Specification.m_Flags & EdgeFlags.Secondary) != 0)
					{
						val8 *= 0.5f;
					}
					float3 val9 = MathUtils.Position(edge.m_Location.m_Line, edge7.m_EndCurvePos);
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(val9, 0.5f, val8);
					if (math.lengthsq(val9 - edge7.m_Location.m_Line.b) > 0.0001f)
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawArrow(edge7.m_Location.m_Line.b, val9, val8, 1f, 25f, 16);
					}
				}
			}
		}

		private Color GetEdgeColor(PathSpecification specification, LocationSpecification location)
		{
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			if (m_RestrictedOption)
			{
				if (specification.m_AccessRequirement >= 0)
				{
					if ((specification.m_Flags & EdgeFlags.RequireAuthorization) != 0)
					{
						return Color.yellow;
					}
					if ((specification.m_Flags & EdgeFlags.AllowExit) != 0)
					{
						return Color.cyan;
					}
					if ((specification.m_Flags & EdgeFlags.AllowEnter) != 0)
					{
						return Color.green;
					}
					return Color.magenta;
				}
				if ((specification.m_Flags & EdgeFlags.RequireAuthorization) != 0)
				{
					return Color.red;
				}
				return Color.gray;
			}
			if (math.any(m_CostOptions))
			{
				float4 value = specification.m_Costs.m_Value;
				value.x += specification.m_Length / specification.m_MaxSpeed;
				float num = math.dot(specification.m_Costs.m_Value, math.select(new float4(0f), new float4(1f), m_CostOptions));
				num = math.sqrt(num / math.max(1f, MathUtils.Length(location.m_Line))) * 5f;
				if (num < 1f)
				{
					return Color.Lerp(Color.cyan, Color.green, math.saturate(num));
				}
				if (num < 2f)
				{
					return Color.Lerp(Color.green, Color.yellow, math.saturate(num - 1f));
				}
				if (num < 3f)
				{
					return Color.Lerp(Color.yellow, Color.red, math.saturate(num - 2f));
				}
				return Color.Lerp(Color.red, Color.black, math.saturate(num - 3f));
			}
			return (Color)(specification.m_Methods switch
			{
				PathMethod.Road => Color.cyan, 
				PathMethod.MediumRoad => new Color(1f, 0.5f, 1f, 1f), 
				PathMethod.Offroad => new Color(1f, 0.5f, 0.5f, 1f), 
				PathMethod.Parking => Color.black, 
				PathMethod.Parking | PathMethod.Boarding => Color.black, 
				PathMethod.SpecialParking => Color.black, 
				PathMethod.Boarding | PathMethod.SpecialParking => Color.black, 
				PathMethod.Boarding => new Color(0.25f, 0.25f, 0.25f, 1f), 
				PathMethod.Pedestrian => Color.green, 
				PathMethod.PublicTransportDay => new Color(0.5f, 0.5f, 1f, 1f), 
				PathMethod.PublicTransportNight => new Color(0f, 0f, 1f, 1f), 
				PathMethod.PublicTransportDay | PathMethod.PublicTransportNight => new Color(0.25f, 0.25f, 1f, 1f), 
				PathMethod.Track => Color.white, 
				PathMethod.Taxi => Color.yellow, 
				PathMethod.CargoTransport => new Color(0.5f, 0f, 1f, 1f), 
				PathMethod.PublicTransportDay | PathMethod.CargoTransport => new Color(0.75f, 0.5f, 1f, 1f), 
				PathMethod.CargoTransport | PathMethod.PublicTransportNight => new Color(0.75f, 0f, 1f, 1f), 
				PathMethod.PublicTransportDay | PathMethod.CargoTransport | PathMethod.PublicTransportNight => new Color(0.75f, 0.25f, 1f, 1f), 
				PathMethod.CargoLoading => new Color(0f, 0.5f, 1f, 1f), 
				PathMethod.Pedestrian | PathMethod.CargoLoading => new Color(0f, 1f, 0.5f, 1f), 
				PathMethod.Road | PathMethod.Track => new Color(0.5f, 1f, 1f, 1f), 
				PathMethod.Flying => new Color(1f, 0.5f, 0f, 1f), 
				_ => Color.gray, 
			});
		}
	}

	[BurstCompile]
	private struct FillPathfindGizmoLinesJob : IJob
	{
		[ReadOnly]
		public NativePathfindData m_PathfindData;

		[ReadOnly]
		public Entity m_Owner;

		[ReadOnly]
		public PathFlags m_Flags;

		[ReadOnly]
		public PathfindAction m_Action;

		public NativeList<PathfindLine> m_PathfindLines;

		public void Execute()
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0589: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			UnsafePathfindData readOnlyData = m_PathfindData.GetReadOnlyData();
			int length = m_Action.readOnlyData.m_StartTargets.Length;
			int length2 = m_Action.readOnlyData.m_EndTargets.Length;
			Bounds3 val = default(Bounds3);
			((Bounds3)(ref val))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			Bounds3 val2 = default(Bounds3);
			((Bounds3)(ref val2))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			int num = 0;
			int num2 = 0;
			EdgeID edgeID = default(EdgeID);
			for (int i = 0; i < length; i++)
			{
				PathTarget pathTarget = m_Action.readOnlyData.m_StartTargets[i];
				if (readOnlyData.m_PathEdges.TryGetValue(pathTarget.m_Entity, ref edgeID))
				{
					float3 val3 = MathUtils.Position(readOnlyData.GetEdge(edgeID).m_Location.m_Line, pathTarget.m_Delta);
					val |= val3;
					num++;
				}
			}
			EdgeID edgeID2 = default(EdgeID);
			for (int j = 0; j < length2; j++)
			{
				PathTarget pathTarget2 = m_Action.readOnlyData.m_EndTargets[j];
				if (readOnlyData.m_PathEdges.TryGetValue(pathTarget2.m_Entity, ref edgeID2))
				{
					float3 val4 = MathUtils.Position(readOnlyData.GetEdge(edgeID2).m_Location.m_Line, pathTarget2.m_Delta);
					val2 |= val4;
					num2++;
				}
			}
			if (num == 0 || num2 == 0)
			{
				float3 val5 = MathUtils.Center(val);
				float3 val6 = MathUtils.Center(val2);
				EdgeID edgeID3 = default(EdgeID);
				for (int k = 0; k < length; k++)
				{
					PathTarget pathTarget3 = m_Action.readOnlyData.m_StartTargets[k];
					if (readOnlyData.m_PathEdges.TryGetValue(pathTarget3.m_Entity, ref edgeID3))
					{
						float3 val7 = MathUtils.Position(readOnlyData.GetEdge(edgeID3).m_Location.m_Line, pathTarget3.m_Delta);
						ref NativeList<PathfindLine> pathfindLines = ref m_PathfindLines;
						PathfindLine pathfindLine = new PathfindLine(m_Owner, m_Flags, new float2(-1f, 0f), new Segment(val7, val5));
						pathfindLines.Add(ref pathfindLine);
					}
				}
				EdgeID edgeID4 = default(EdgeID);
				for (int l = 0; l < length2; l++)
				{
					PathTarget pathTarget4 = m_Action.readOnlyData.m_EndTargets[l];
					if (readOnlyData.m_PathEdges.TryGetValue(pathTarget4.m_Entity, ref edgeID4))
					{
						float3 val8 = MathUtils.Position(readOnlyData.GetEdge(edgeID4).m_Location.m_Line, pathTarget4.m_Delta);
						ref NativeList<PathfindLine> pathfindLines2 = ref m_PathfindLines;
						PathfindLine pathfindLine = new PathfindLine(m_Owner, m_Flags, new float2(-1f, 0f), new Segment(val6, val8));
						pathfindLines2.Add(ref pathfindLine);
					}
				}
				if (num > 1)
				{
					ref NativeList<PathfindLine> pathfindLines3 = ref m_PathfindLines;
					PathfindLine pathfindLine = new PathfindLine(m_Owner, m_Flags, new float2(-1f, 0f), new Segment(val5, val5));
					pathfindLines3.Add(ref pathfindLine);
				}
				if (num2 > 1)
				{
					ref NativeList<PathfindLine> pathfindLines4 = ref m_PathfindLines;
					PathfindLine pathfindLine = new PathfindLine(m_Owner, m_Flags, new float2(-1f, 0f), new Segment(val6, val6));
					pathfindLines4.Add(ref pathfindLine);
				}
				return;
			}
			float3 val9 = MathUtils.Center(val);
			float3 val10 = MathUtils.Center(val2);
			float num3 = math.length(MathUtils.Size(val));
			float num4 = math.length(MathUtils.Size(val2));
			float num5 = math.distance(val9, val10);
			if (num > 1 && num2 > 1)
			{
				if (num3 >= num5 && num4 >= num5)
				{
					val9 = math.lerp(val9, val10, 0.5f);
					val10 = val9;
				}
				else if (num3 >= num5)
				{
					val9 = val10;
				}
				else if (num4 >= num5)
				{
					val10 = val9;
				}
			}
			else if (num > 1)
			{
				if (num3 >= num5)
				{
					val9 = val10;
				}
			}
			else if (num2 > 1 && num4 >= num5)
			{
				val10 = val9;
			}
			float2 val11 = default(float2);
			((float2)(ref val11))._002Ector(0f, 0f);
			if (num > 1)
			{
				val11.x -= 1f;
			}
			if (!((float3)(ref val9)).Equals(val10) || (num == 1 && num2 == 1))
			{
				val11.x -= 1f;
			}
			if (num2 > 1)
			{
				val11.x -= 1f;
			}
			if (num > 1)
			{
				EdgeID edgeID5 = default(EdgeID);
				for (int m = 0; m < length; m++)
				{
					PathTarget pathTarget5 = m_Action.readOnlyData.m_StartTargets[m];
					if (readOnlyData.m_PathEdges.TryGetValue(pathTarget5.m_Entity, ref edgeID5))
					{
						float3 val12 = MathUtils.Position(readOnlyData.GetEdge(edgeID5).m_Location.m_Line, pathTarget5.m_Delta);
						ref NativeList<PathfindLine> pathfindLines5 = ref m_PathfindLines;
						PathfindLine pathfindLine = new PathfindLine(m_Owner, m_Flags, val11, new Segment(val12, val9));
						pathfindLines5.Add(ref pathfindLine);
					}
				}
				val11.y -= 1f;
			}
			if (!((float3)(ref val9)).Equals(val10) || (num == 1 && num2 == 1))
			{
				ref NativeList<PathfindLine> pathfindLines6 = ref m_PathfindLines;
				PathfindLine pathfindLine = new PathfindLine(m_Owner, m_Flags, val11, new Segment(val9, val10));
				pathfindLines6.Add(ref pathfindLine);
				val11.y -= 1f;
			}
			if (num2 <= 1)
			{
				return;
			}
			EdgeID edgeID6 = default(EdgeID);
			for (int n = 0; n < length2; n++)
			{
				PathTarget pathTarget6 = m_Action.readOnlyData.m_EndTargets[n];
				if (readOnlyData.m_PathEdges.TryGetValue(pathTarget6.m_Entity, ref edgeID6))
				{
					float3 val13 = MathUtils.Position(readOnlyData.GetEdge(edgeID6).m_Location.m_Line, pathTarget6.m_Delta);
					ref NativeList<PathfindLine> pathfindLines7 = ref m_PathfindLines;
					PathfindLine pathfindLine = new PathfindLine(m_Owner, m_Flags, val11, new Segment(val10, val13));
					pathfindLines7.Add(ref pathfindLine);
				}
			}
		}
	}

	[BurstCompile]
	private struct SetPathfindGizmoLineFlagsJob : IJob
	{
		[ReadOnly]
		public Entity m_Owner;

		[ReadOnly]
		public PathFlags m_Flags;

		[ReadOnly]
		public bool m_TooLong;

		public NativeList<PathfindLine> m_PathfindLines;

		public void Execute()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_PathfindLines.Length; i++)
			{
				PathfindLine pathfindLine = m_PathfindLines[i];
				if ((pathfindLine.m_Flags & PathFlags.Pending) != 0 && pathfindLine.m_Owner == m_Owner)
				{
					pathfindLine.m_Flags = m_Flags;
					pathfindLine.m_TooLong = m_TooLong;
					m_PathfindLines[i] = pathfindLine;
				}
			}
		}
	}

	[BurstCompile]
	private struct PathfindLineGizmoJob : IJob
	{
		[ReadOnly]
		public float m_DeltaTime;

		public NativeList<PathfindLine> m_PathfindLines;

		public GizmoBatcher m_GizmoBatcher;

		public void Execute()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			int num2 = 0;
			while (num < m_PathfindLines.Length)
			{
				PathfindLine pathfindLine = m_PathfindLines[num++];
				ref float3 time = ref pathfindLine.m_Time;
				((float3)(ref time)).yz = ((float3)(ref time)).yz + m_DeltaTime;
				if (!(pathfindLine.m_Time.y >= pathfindLine.m_Time.x) || !(pathfindLine.m_Time.y <= 0f))
				{
					continue;
				}
				Color val = (((pathfindLine.m_Flags & PathFlags.Failed) != 0) ? (pathfindLine.m_TooLong ? Color.yellow : Color.red) : (((pathfindLine.m_Flags & PathFlags.Pending) == 0) ? Color.green : Color.gray));
				if (math.distancesq(pathfindLine.m_Line.a, pathfindLine.m_Line.b) > 1E-06f)
				{
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(pathfindLine.m_Line.a, pathfindLine.m_Line.b, val);
					if (pathfindLine.m_Time.z > 0f && pathfindLine.m_Time.z <= 1f)
					{
						float3 val2 = MathUtils.Position(pathfindLine.m_Line, pathfindLine.m_Time.z);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawArrowHead(val2, pathfindLine.m_Line.b - pathfindLine.m_Line.a, val, 10f, 25f, 16);
					}
				}
				else
				{
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(pathfindLine.m_Line.a, 5f, val);
				}
				m_PathfindLines[num2++] = pathfindLine;
			}
			if (num2 < m_PathfindLines.Length)
			{
				m_PathfindLines.RemoveRange(num2, m_PathfindLines.Length - num2);
			}
		}
	}

	private PathfindQueueSystem m_PathfindQueueSystem;

	private GizmosSystem m_GizmosSystem;

	private RenderingSystem m_RenderingSystem;

	private NativeList<PathfindLine> m_PathfindLines;

	private Option m_GraphOption;

	private Option m_RestrictedOption;

	private Option m_TimeCostOption;

	private Option m_BehaviorCostOption;

	private Option m_MoneyCostOption;

	private Option m_ComfortCostOption;

	private Option m_PathfindOption;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PathfindQueueSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindQueueSystem>();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_PathfindLines = new NativeList<PathfindLine>(AllocatorHandle.op_Implicit((Allocator)4));
		m_GraphOption = AddOption("Draw Graph", defaultEnabled: true);
		m_RestrictedOption = AddOption("Show Restrictions", defaultEnabled: false);
		m_TimeCostOption = AddOption("Show time cost", defaultEnabled: false);
		m_BehaviorCostOption = AddOption("Show behavior cost", defaultEnabled: false);
		m_MoneyCostOption = AddOption("Show money cost", defaultEnabled: false);
		m_ComfortCostOption = AddOption("Show comfort cost", defaultEnabled: false);
		m_PathfindOption = AddOption("Visualize Queries", defaultEnabled: true);
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_PathfindLines.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected unsafe override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		if (!m_GraphOption.enabled && !m_PathfindOption.enabled)
		{
			return inputDeps;
		}
		JobHandle val = inputDeps;
		JobHandle dependencies;
		NativePathfindData dataContainer = m_PathfindQueueSystem.GetDataContainer(out dependencies);
		inputDeps = JobHandle.CombineDependencies(inputDeps, dependencies);
		if (m_GraphOption.enabled)
		{
			NativeReference<int> val2 = default(NativeReference<int>);
			val2._002Ector(AllocatorHandle.op_Implicit((Allocator)3), (NativeArrayOptions)1);
			EdgeCountJob edgeCountJob = new EdgeCountJob
			{
				m_PathfindData = dataContainer,
				m_EdgeCount = val2
			};
			JobHandle val3 = default(JobHandle);
			PathfindEdgeGizmoJob obj = new PathfindEdgeGizmoJob
			{
				m_RestrictedOption = m_RestrictedOption.enabled,
				m_CostOptions = new bool4(m_TimeCostOption.enabled, m_BehaviorCostOption.enabled, m_MoneyCostOption.enabled, m_ComfortCostOption.enabled),
				m_PathfindData = dataContainer,
				m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val3)
			};
			JobHandle val4 = IJobExtensions.Schedule<EdgeCountJob>(edgeCountJob, inputDeps);
			JobHandle val5 = IJobParallelForDeferExtensions.Schedule<PathfindEdgeGizmoJob>(obj, NativeReferenceUnsafeUtility.GetUnsafePtrWithoutChecks<int>(val2), 64, JobHandle.CombineDependencies(val4, val3));
			val2.Dispose(val5);
			m_GizmosSystem.AddGizmosBatcherWriter(val5);
			m_PathfindQueueSystem.AddDataReader(val5);
			val = JobHandle.CombineDependencies(val, val5);
		}
		if (m_PathfindOption.enabled)
		{
			m_PathfindQueueSystem.RequireDebug();
			PathfindQueueSystem.ActionList<PathfindAction> pathfindActions = m_PathfindQueueSystem.GetPathfindActions();
			JobHandle val6 = inputDeps;
			for (int i = 0; i < pathfindActions.m_Items.Count; i++)
			{
				PathfindQueueSystem.ActionListItem<PathfindAction> value = pathfindActions.m_Items[i];
				if ((value.m_Flags & PathFlags.Debug) == 0)
				{
					JobHandle val7 = IJobExtensions.Schedule<FillPathfindGizmoLinesJob>(new FillPathfindGizmoLinesJob
					{
						m_Action = value.m_Action,
						m_Owner = value.m_Owner,
						m_Flags = PathFlags.Pending,
						m_PathfindLines = m_PathfindLines,
						m_PathfindData = dataContainer
					}, JobHandle.CombineDependencies(val6, value.m_Dependencies));
					m_PathfindQueueSystem.AddDataReader(val7);
					val6 = val7;
					value.m_Dependencies = val7;
					value.m_Flags |= PathFlags.Debug;
					pathfindActions.m_Items[i] = value;
				}
				if ((value.m_Flags & (PathFlags.Pending | PathFlags.Scheduled)) == 0)
				{
					PathFlags pathFlags = (PathFlags)0;
					bool tooLong = false;
					if (value.m_Action.readOnlyData.m_Result[0].m_Distance < 0f)
					{
						pathFlags |= PathFlags.Failed;
						tooLong = value.m_Action.readOnlyData.m_Result[0].m_TotalCost > 0f;
					}
					val6 = IJobExtensions.Schedule<SetPathfindGizmoLineFlagsJob>(new SetPathfindGizmoLineFlagsJob
					{
						m_Owner = value.m_Owner,
						m_Flags = pathFlags,
						m_TooLong = tooLong,
						m_PathfindLines = m_PathfindLines
					}, val6);
				}
			}
			JobHandle val9 = default(JobHandle);
			JobHandle val8 = IJobExtensions.Schedule<PathfindLineGizmoJob>(new PathfindLineGizmoJob
			{
				m_DeltaTime = m_RenderingSystem.frameDelta / 60f,
				m_PathfindLines = m_PathfindLines,
				m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val9)
			}, JobHandle.CombineDependencies(val6, val9));
			m_GizmosSystem.AddGizmosBatcherWriter(val8);
			val = JobHandle.CombineDependencies(val, val8);
		}
		return val;
	}

	[Preserve]
	public PathfindDebugSystem()
	{
	}
}
