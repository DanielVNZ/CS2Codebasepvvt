using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Achievements;
using Game.Areas;
using Game.Audio;
using Game.Buildings;
using Game.Common;
using Game.Input;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class BulldozeToolSystem : ToolBaseSystem
{
	public enum Mode
	{
		MainElements,
		SubElements,
		Everything
	}

	private enum State
	{
		Default,
		Applying,
		Waiting,
		Confirmed,
		Cancelled
	}

	private struct PathEdge
	{
		public Entity m_Edge;

		public bool m_Invert;
	}

	public struct PathItem : ILessThan<PathItem>
	{
		public Entity m_Node;

		public Entity m_Edge;

		public float m_Cost;

		public bool LessThan(PathItem other)
		{
			return m_Cost < other.m_Cost;
		}
	}

	[BurstCompile]
	private struct SnapJob : IJob
	{
		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public Mode m_Mode;

		[ReadOnly]
		public State m_State;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Static> m_StaticData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnectionData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> m_ServiceUpgradeData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		public NativeList<ControlPoint> m_ControlPoints;

		public void Execute()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			if (!m_EditorMode && m_OutsideConnectionData.HasComponent(m_ControlPoints[m_ControlPoints.Length - 1].m_OriginalEntity))
			{
				m_ControlPoints.RemoveAt(m_ControlPoints.Length - 1);
				return;
			}
			if (m_Mode == Mode.MainElements)
			{
				ControlPoint controlPoint = m_ControlPoints[m_ControlPoints.Length - 1];
				Owner owner = default(Owner);
				if (m_StaticData.HasComponent(controlPoint.m_OriginalEntity) && !m_ServiceUpgradeData.HasComponent(controlPoint.m_OriginalEntity) && m_OwnerData.TryGetComponent(controlPoint.m_OriginalEntity, ref owner))
				{
					controlPoint.m_OriginalEntity = owner.m_Owner;
					while (m_StaticData.HasComponent(controlPoint.m_OriginalEntity) && !m_ServiceUpgradeData.HasComponent(controlPoint.m_OriginalEntity) && m_OwnerData.TryGetComponent(controlPoint.m_OriginalEntity, ref owner))
					{
						controlPoint.m_OriginalEntity = owner.m_Owner;
					}
					m_ControlPoints[m_ControlPoints.Length - 1] = controlPoint;
				}
			}
			if (m_State != State.Applying)
			{
				return;
			}
			ControlPoint startPoint = m_ControlPoints[0];
			ControlPoint endPoint = m_ControlPoints[m_ControlPoints.Length - 1];
			if (m_EdgeData.HasComponent(startPoint.m_OriginalEntity) || m_NodeData.HasComponent(startPoint.m_OriginalEntity))
			{
				m_ControlPoints.Clear();
				NativeList<PathEdge> path = default(NativeList<PathEdge>);
				path._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
				CreatePath(startPoint, endPoint, path);
				AddControlPoints(startPoint, endPoint, path);
				return;
			}
			if (m_EdgeData.HasComponent(endPoint.m_OriginalEntity) || m_NodeData.HasComponent(endPoint.m_OriginalEntity))
			{
				m_ControlPoints.RemoveAt(m_ControlPoints.Length - 1);
				return;
			}
			Entity val = Entity.Null;
			Entity val2 = Entity.Null;
			if (m_OwnerData.HasComponent(startPoint.m_OriginalEntity))
			{
				val = m_OwnerData[startPoint.m_OriginalEntity].m_Owner;
			}
			if (m_OwnerData.HasComponent(endPoint.m_OriginalEntity))
			{
				val2 = m_OwnerData[endPoint.m_OriginalEntity].m_Owner;
			}
			if (val != val2)
			{
				m_ControlPoints.RemoveAt(m_ControlPoints.Length - 1);
				return;
			}
			for (int i = 0; i < m_ControlPoints.Length - 1; i++)
			{
				if (m_ControlPoints[i].m_OriginalEntity == endPoint.m_OriginalEntity)
				{
					m_ControlPoints.RemoveAt(m_ControlPoints.Length - 1);
					break;
				}
			}
		}

		private void CreatePath(ControlPoint startPoint, ControlPoint endPoint, NativeList<PathEdge> path)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			if (math.distance(startPoint.m_Position, endPoint.m_Position) < 4f)
			{
				endPoint = startPoint;
			}
			PrefabRef prefabRef = m_PrefabRefData[startPoint.m_OriginalEntity];
			NetData netData = m_PrefabNetData[prefabRef.m_Prefab];
			if (startPoint.m_OriginalEntity == endPoint.m_OriginalEntity)
			{
				if (m_EdgeData.HasComponent(endPoint.m_OriginalEntity))
				{
					PathEdge pathEdge = new PathEdge
					{
						m_Edge = endPoint.m_OriginalEntity,
						m_Invert = (endPoint.m_CurvePosition < startPoint.m_CurvePosition)
					};
					path.Add(ref pathEdge);
				}
				return;
			}
			NativeMinHeap<PathItem> val = default(NativeMinHeap<PathItem>);
			val._002Ector(100, (Allocator)2);
			NativeParallelHashMap<Entity, Entity> val2 = default(NativeParallelHashMap<Entity, Entity>);
			val2._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			if (m_EdgeData.HasComponent(endPoint.m_OriginalEntity))
			{
				Edge edge = m_EdgeData[endPoint.m_OriginalEntity];
				PrefabRef prefabRef2 = m_PrefabRefData[endPoint.m_OriginalEntity];
				NetData netData2 = m_PrefabNetData[prefabRef2.m_Prefab];
				if ((netData.m_RequiredLayers & netData2.m_RequiredLayers) != Layer.None)
				{
					val.Insert(new PathItem
					{
						m_Node = edge.m_Start,
						m_Edge = endPoint.m_OriginalEntity,
						m_Cost = 0f
					});
					val.Insert(new PathItem
					{
						m_Node = edge.m_End,
						m_Edge = endPoint.m_OriginalEntity,
						m_Cost = 0f
					});
				}
			}
			else if (m_NodeData.HasComponent(endPoint.m_OriginalEntity))
			{
				val.Insert(new PathItem
				{
					m_Node = endPoint.m_OriginalEntity,
					m_Edge = Entity.Null,
					m_Cost = 0f
				});
			}
			Entity val3 = Entity.Null;
			while (val.Length != 0)
			{
				PathItem pathItem = val.Extract();
				if (pathItem.m_Edge == startPoint.m_OriginalEntity)
				{
					val2[pathItem.m_Node] = pathItem.m_Edge;
					val3 = pathItem.m_Node;
					break;
				}
				if (!val2.TryAdd(pathItem.m_Node, pathItem.m_Edge))
				{
					continue;
				}
				if (pathItem.m_Node == startPoint.m_OriginalEntity)
				{
					val3 = pathItem.m_Node;
					break;
				}
				DynamicBuffer<ConnectedEdge> val4 = m_ConnectedEdges[pathItem.m_Node];
				PrefabRef prefabRef3 = default(PrefabRef);
				if (pathItem.m_Edge != Entity.Null)
				{
					prefabRef3 = m_PrefabRefData[pathItem.m_Edge];
				}
				for (int i = 0; i < val4.Length; i++)
				{
					Entity edge2 = val4[i].m_Edge;
					if (edge2 == pathItem.m_Edge)
					{
						continue;
					}
					Edge edge3 = m_EdgeData[edge2];
					Entity val5;
					if (edge3.m_Start == pathItem.m_Node)
					{
						val5 = edge3.m_End;
					}
					else
					{
						if (!(edge3.m_End == pathItem.m_Node))
						{
							continue;
						}
						val5 = edge3.m_Start;
					}
					if (!val2.ContainsKey(val5) || !(edge2 != startPoint.m_OriginalEntity))
					{
						PrefabRef prefabRef4 = m_PrefabRefData[edge2];
						NetData netData3 = m_PrefabNetData[prefabRef4.m_Prefab];
						if ((netData.m_RequiredLayers & netData3.m_RequiredLayers) != Layer.None)
						{
							Curve curve = m_CurveData[edge2];
							float num = pathItem.m_Cost + curve.m_Length;
							num += math.select(0f, 9.9f, prefabRef.m_Prefab != prefabRef3.m_Prefab);
							num += math.select(0f, 10f, val4.Length > 2);
							val.Insert(new PathItem
							{
								m_Node = val5,
								m_Edge = edge2,
								m_Cost = num
							});
						}
					}
				}
			}
			Entity val6 = default(Entity);
			while (val2.TryGetValue(val3, ref val6) && !(val6 == Entity.Null))
			{
				Edge edge4 = m_EdgeData[val6];
				bool flag = edge4.m_End == val3;
				PathEdge pathEdge = new PathEdge
				{
					m_Edge = val6,
					m_Invert = flag
				};
				path.Add(ref pathEdge);
				if (!(val6 == endPoint.m_OriginalEntity))
				{
					val3 = (flag ? edge4.m_Start : edge4.m_End);
					continue;
				}
				break;
			}
		}

		private void AddControlPoints(ControlPoint startPoint, ControlPoint endPoint, NativeList<PathEdge> path)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			m_ControlPoints.Add(ref startPoint);
			for (int i = 0; i < path.Length; i++)
			{
				PathEdge pathEdge = path[i];
				Edge edge = m_EdgeData[pathEdge.m_Edge];
				Curve curve = m_CurveData[pathEdge.m_Edge];
				if (pathEdge.m_Invert)
				{
					CommonUtils.Swap(ref edge.m_Start, ref edge.m_End);
					curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
				}
				ControlPoint controlPoint = endPoint;
				controlPoint.m_OriginalEntity = edge.m_Start;
				controlPoint.m_Position = curve.m_Bezier.a;
				ControlPoint controlPoint2 = endPoint;
				controlPoint2.m_OriginalEntity = edge.m_End;
				controlPoint2.m_Position = curve.m_Bezier.d;
				m_ControlPoints.Add(ref controlPoint);
				m_ControlPoints.Add(ref controlPoint2);
			}
			m_ControlPoints.Add(ref endPoint);
		}
	}

	[BurstCompile]
	private struct CreateDefinitionsJob : IJob
	{
		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public Mode m_Mode;

		[ReadOnly]
		public State m_State;

		[ReadOnly]
		public NativeList<ControlPoint> m_ControlPoints;

		[ReadOnly]
		public BufferLookup<LocalNodeCache> m_CachedNodes;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> m_SubNets;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> m_LocalTransformCacheData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Fixed> m_FixedData;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> m_LotData;

		[ReadOnly]
		public ComponentLookup<EditorContainer> m_EditorContainerData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Attached> m_AttachedData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Placeholder> m_PlaceholderData;

		[ReadOnly]
		public ComponentLookup<Attachment> m_AttachmentData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> m_ServiceUpgradeData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			NativeHashSet<Entity> bulldozeEntities = default(NativeHashSet<Entity>);
			bulldozeEntities._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
			if (m_State == State.Applying && m_ControlPoints.Length >= 2 && (m_EdgeData.HasComponent(m_ControlPoints[0].m_OriginalEntity) || m_NodeData.HasComponent(m_ControlPoints[0].m_OriginalEntity)))
			{
				int num = m_ControlPoints.Length / 2 - 1;
				if (num == 0 && m_ControlPoints[0].m_OriginalEntity == m_ControlPoints[1].m_OriginalEntity)
				{
					if (m_Mode != Mode.MainElements || !m_OwnerData.HasComponent(m_ControlPoints[0].m_OriginalEntity) || m_ServiceUpgradeData.HasComponent(m_ControlPoints[0].m_OriginalEntity))
					{
						bulldozeEntities.Add(m_ControlPoints[0].m_OriginalEntity);
					}
				}
				else
				{
					for (int i = 0; i < num; i++)
					{
						ControlPoint controlPoint = m_ControlPoints[i * 2 + 1];
						ControlPoint controlPoint2 = m_ControlPoints[i * 2 + 2];
						DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[controlPoint.m_OriginalEntity];
						for (int j = 0; j < val.Length; j++)
						{
							Entity edge = val[j].m_Edge;
							Edge edge2 = m_EdgeData[edge];
							if (m_Mode != Mode.MainElements || !m_OwnerData.HasComponent(edge) || m_ServiceUpgradeData.HasComponent(edge))
							{
								if (edge2.m_Start == controlPoint.m_OriginalEntity && edge2.m_End == controlPoint2.m_OriginalEntity)
								{
									bulldozeEntities.Add(edge);
								}
								else if (edge2.m_End == controlPoint.m_OriginalEntity && edge2.m_Start == controlPoint2.m_OriginalEntity)
								{
									bulldozeEntities.Add(edge);
								}
							}
						}
					}
				}
			}
			else
			{
				for (int k = 0; k < m_ControlPoints.Length; k++)
				{
					bulldozeEntities.Add(m_ControlPoints[k].m_OriginalEntity);
				}
			}
			if (!bulldozeEntities.IsEmpty)
			{
				NativeHashMap<Entity, OwnerDefinition> ownerDefinitions = default(NativeHashMap<Entity, OwnerDefinition>);
				ownerDefinitions._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
				NativeHashSet<Entity> attachedEntities = default(NativeHashSet<Entity>);
				attachedEntities._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
				NativeArray<Entity> val2 = bulldozeEntities.ToNativeArray(AllocatorHandle.op_Implicit((Allocator)2));
				for (int l = 0; l < val2.Length; l++)
				{
					Execute(val2[l], ownerDefinitions, bulldozeEntities, attachedEntities);
				}
				val2.Dispose();
				ownerDefinitions.Dispose();
				attachedEntities.Dispose();
			}
			bulldozeEntities.Dispose();
		}

		private void Execute(Entity bulldozeEntity, NativeHashMap<Entity, OwnerDefinition> ownerDefinitions, NativeHashSet<Entity> bulldozeEntities, NativeHashSet<Entity> attachedEntities)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0627: Unknown result type (might be due to invalid IL or missing references)
			//IL_060e: Unknown result type (might be due to invalid IL or missing references)
			//IL_060f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_0613: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0633: Unknown result type (might be due to invalid IL or missing references)
			//IL_0634: Unknown result type (might be due to invalid IL or missing references)
			//IL_0635: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_0638: Unknown result type (might be due to invalid IL or missing references)
			//IL_064c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0704: Unknown result type (might be due to invalid IL or missing references)
			//IL_0709: Unknown result type (might be due to invalid IL or missing references)
			//IL_0712: Unknown result type (might be due to invalid IL or missing references)
			//IL_0717: Unknown result type (might be due to invalid IL or missing references)
			//IL_0720: Unknown result type (might be due to invalid IL or missing references)
			//IL_0725: Unknown result type (might be due to invalid IL or missing references)
			//IL_0730: Unknown result type (might be due to invalid IL or missing references)
			//IL_0731: Unknown result type (might be due to invalid IL or missing references)
			//IL_0736: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0671: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_053e: Unknown result type (might be due to invalid IL or missing references)
			//IL_053f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0543: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0581: Unknown result type (might be due to invalid IL or missing references)
			//IL_0589: Unknown result type (might be due to invalid IL or missing references)
			//IL_0594: Unknown result type (might be due to invalid IL or missing references)
			//IL_0599: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_073e: Unknown result type (might be due to invalid IL or missing references)
			//IL_073f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0740: Unknown result type (might be due to invalid IL or missing references)
			//IL_074b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0750: Unknown result type (might be due to invalid IL or missing references)
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_067f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0680: Unknown result type (might be due to invalid IL or missing references)
			//IL_0684: Unknown result type (might be due to invalid IL or missing references)
			//IL_0689: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05be: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			Entity val = Entity.Null;
			Entity val2 = Entity.Null;
			OwnerDefinition ownerDefinition = default(OwnerDefinition);
			bool parent = false;
			if (m_OwnerData.HasComponent(bulldozeEntity))
			{
				if (m_EditorMode)
				{
					Entity val3 = bulldozeEntity;
					while (m_OwnerData.HasComponent(val3) && !m_BuildingData.HasComponent(val3))
					{
						val3 = m_OwnerData[val3].m_Owner;
						if (m_ServiceUpgradeData.HasComponent(val3))
						{
							val2 = val3;
						}
					}
					if (m_TransformData.HasComponent(val3) && m_SubObjects.HasBuffer(val3))
					{
						val = val3;
					}
				}
				else if (m_ServiceUpgradeData.HasComponent(bulldozeEntity))
				{
					val = m_OwnerData[bulldozeEntity].m_Owner;
					parent = true;
				}
			}
			if (m_TransformData.HasComponent(val))
			{
				if (!ownerDefinitions.TryGetValue(val, ref ownerDefinition))
				{
					Transform transform = m_TransformData[val];
					Entity owner = Entity.Null;
					if (m_OwnerData.HasComponent(val))
					{
						owner = m_OwnerData[val].m_Owner;
					}
					AddEntity(ownerDefinitions, bulldozeEntities, attachedEntities, val, owner, default(OwnerDefinition), val2 == Entity.Null, parent, delete: false);
					ownerDefinition.m_Prefab = m_PrefabRefData[val].m_Prefab;
					ownerDefinition.m_Position = transform.m_Position;
					ownerDefinition.m_Rotation = transform.m_Rotation;
					if (m_InstalledUpgrades.HasBuffer(val))
					{
						DynamicBuffer<InstalledUpgrade> val4 = m_InstalledUpgrades[val];
						for (int i = 0; i < val4.Length; i++)
						{
							Entity upgrade = val4[i].m_Upgrade;
							if (upgrade != bulldozeEntity && !bulldozeEntities.Contains(upgrade))
							{
								AddEntity(ownerDefinitions, bulldozeEntities, attachedEntities, upgrade, Entity.Null, ownerDefinition, val2 == upgrade, parent, delete: false);
							}
						}
					}
					Attachment attachment = default(Attachment);
					if (m_AttachmentData.TryGetComponent(val, ref attachment) && m_TransformData.HasComponent(attachment.m_Attached))
					{
						AddEntity(ownerDefinitions, bulldozeEntities, attachedEntities, attachment.m_Attached, Entity.Null, default(OwnerDefinition), upgrade: true, parent, delete: false);
					}
					ownerDefinitions.Add(val, ownerDefinition);
					if (m_TransformData.HasComponent(val2))
					{
						transform = m_TransformData[val2];
						ownerDefinition.m_Prefab = m_PrefabRefData[val2].m_Prefab;
						ownerDefinition.m_Position = transform.m_Position;
						ownerDefinition.m_Rotation = transform.m_Rotation;
					}
				}
			}
			else if (m_AttachmentData.HasComponent(bulldozeEntity))
			{
				Attachment attachment2 = m_AttachmentData[bulldozeEntity];
				if (!bulldozeEntities.Contains(attachment2.m_Attached) && m_TransformData.HasComponent(attachment2.m_Attached) && m_PlaceholderData.HasComponent(bulldozeEntity) && !m_OwnerData.HasComponent(attachment2.m_Attached))
				{
					AddEntity(ownerDefinitions, bulldozeEntities, attachedEntities, attachment2.m_Attached, Entity.Null, default(OwnerDefinition), upgrade: false, parent: false, delete: true);
				}
			}
			else if (m_AttachedData.HasComponent(bulldozeEntity))
			{
				Attached attached = m_AttachedData[bulldozeEntity];
				if (!bulldozeEntities.Contains(attached.m_Parent) && m_AttachmentData.HasComponent(attached.m_Parent) && m_AttachmentData[attached.m_Parent].m_Attached == bulldozeEntity)
				{
					AddEntity(ownerDefinitions, bulldozeEntities, attachedEntities, attached.m_Parent, Entity.Null, default(OwnerDefinition), upgrade: false, parent: false, delete: true);
					if (m_InstalledUpgrades.HasBuffer(attached.m_Parent))
					{
						Transform transform2 = m_TransformData[attached.m_Parent];
						DynamicBuffer<InstalledUpgrade> val5 = m_InstalledUpgrades[attached.m_Parent];
						OwnerDefinition ownerDefinition2 = new OwnerDefinition
						{
							m_Prefab = m_PrefabRefData[attached.m_Parent].m_Prefab,
							m_Position = transform2.m_Position,
							m_Rotation = transform2.m_Rotation
						};
						for (int j = 0; j < val5.Length; j++)
						{
							Entity upgrade2 = val5[j].m_Upgrade;
							AddEntity(ownerDefinitions, bulldozeEntities, attachedEntities, upgrade2, Entity.Null, ownerDefinition2, upgrade: false, parent: false, delete: true);
						}
					}
				}
			}
			if (m_ConnectedEdges.HasBuffer(bulldozeEntity))
			{
				DynamicBuffer<ConnectedEdge> val6 = m_ConnectedEdges[bulldozeEntity];
				PrefabRef prefabRef = m_PrefabRefData[bulldozeEntity];
				NetData netData = default(NetData);
				m_PrefabNetData.TryGetComponent(prefabRef.m_Prefab, ref netData);
				bool flag = true;
				bool flag2 = false;
				NetData netData2 = default(NetData);
				for (int k = 0; k < val6.Length; k++)
				{
					Entity edge = val6[k].m_Edge;
					Edge edge2 = m_EdgeData[edge];
					if (!(edge2.m_Start == bulldozeEntity) && !(edge2.m_End == bulldozeEntity))
					{
						continue;
					}
					PrefabRef prefabRef2 = m_PrefabRefData[edge];
					m_PrefabNetData.TryGetComponent(prefabRef2.m_Prefab, ref netData2);
					if ((netData.m_RequiredLayers & netData2.m_RequiredLayers) == 0)
					{
						flag2 = true;
						continue;
					}
					if (!bulldozeEntities.Contains(edge))
					{
						AddEdge(ownerDefinitions, bulldozeEntities, attachedEntities, edge, Entity.Null, ownerDefinition, upgrade: false, delete: true);
					}
					flag = false;
				}
				if (flag && flag2)
				{
					for (int l = 0; l < val6.Length; l++)
					{
						Entity edge3 = val6[l].m_Edge;
						Edge edge4 = m_EdgeData[edge3];
						if (edge4.m_Start == bulldozeEntity || edge4.m_End == bulldozeEntity)
						{
							if (!bulldozeEntities.Contains(edge3))
							{
								AddEdge(ownerDefinitions, bulldozeEntities, attachedEntities, edge3, Entity.Null, ownerDefinition, upgrade: false, delete: true);
							}
							flag = false;
						}
					}
				}
				if (flag)
				{
					AddEntity(ownerDefinitions, bulldozeEntities, attachedEntities, bulldozeEntity, Entity.Null, ownerDefinition, upgrade: false, parent: false, delete: true);
				}
				return;
			}
			if (m_EdgeData.HasComponent(bulldozeEntity))
			{
				AddEdge(ownerDefinitions, bulldozeEntities, attachedEntities, bulldozeEntity, Entity.Null, ownerDefinition, upgrade: false, delete: true);
				return;
			}
			if (m_AreaNodes.HasBuffer(bulldozeEntity))
			{
				AddEntity(ownerDefinitions, bulldozeEntities, attachedEntities, bulldozeEntity, Entity.Null, ownerDefinition, upgrade: false, parent: false, delete: true);
				DynamicBuffer<Game.Objects.SubObject> val7 = default(DynamicBuffer<Game.Objects.SubObject>);
				if (!m_SubObjects.TryGetBuffer(bulldozeEntity, ref val7))
				{
					return;
				}
				for (int m = 0; m < val7.Length; m++)
				{
					Game.Objects.SubObject subObject = val7[m];
					if (m_BuildingData.HasComponent(subObject.m_SubObject))
					{
						AddEntity(ownerDefinitions, bulldozeEntities, attachedEntities, subObject.m_SubObject, Entity.Null, default(OwnerDefinition), upgrade: false, parent: false, delete: true);
					}
				}
				return;
			}
			AddEntity(ownerDefinitions, bulldozeEntities, attachedEntities, bulldozeEntity, Entity.Null, ownerDefinition, upgrade: false, parent: false, delete: true);
			if (m_InstalledUpgrades.HasBuffer(bulldozeEntity) && m_TransformData.HasComponent(bulldozeEntity))
			{
				Transform transform3 = m_TransformData[bulldozeEntity];
				ownerDefinition.m_Prefab = m_PrefabRefData[bulldozeEntity].m_Prefab;
				ownerDefinition.m_Position = transform3.m_Position;
				ownerDefinition.m_Rotation = transform3.m_Rotation;
				DynamicBuffer<InstalledUpgrade> val8 = m_InstalledUpgrades[bulldozeEntity];
				for (int n = 0; n < val8.Length; n++)
				{
					AddEntity(ownerDefinitions, bulldozeEntities, attachedEntities, val8[n].m_Upgrade, Entity.Null, ownerDefinition, upgrade: false, parent: false, delete: true);
				}
			}
		}

		private void AddEdge(NativeHashMap<Entity, OwnerDefinition> ownerDefinitions, NativeHashSet<Entity> bulldozeEntities, NativeHashSet<Entity> attachedEntities, Entity entity, Entity owner, OwnerDefinition ownerDefinition, bool upgrade, bool delete)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			AddEntity(ownerDefinitions, bulldozeEntities, attachedEntities, entity, owner, ownerDefinition, upgrade, parent: false, delete);
			if (m_FixedData.HasComponent(entity))
			{
				Edge edge = m_EdgeData[entity];
				AddFixedEdges(ownerDefinitions, bulldozeEntities, attachedEntities, entity, edge.m_Start, owner, ownerDefinition, upgrade, delete);
				AddFixedEdges(ownerDefinitions, bulldozeEntities, attachedEntities, entity, edge.m_End, owner, ownerDefinition, upgrade, delete);
			}
		}

		private void AddFixedEdges(NativeHashMap<Entity, OwnerDefinition> ownerDefinitions, NativeHashSet<Entity> bulldozeEntities, NativeHashSet<Entity> attachedEntities, Entity lastEdge, Entity lastNode, Entity owner, OwnerDefinition ownerDefinition, bool upgrade, bool delete)
		{
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			while (m_FixedData.HasComponent(lastNode) && !bulldozeEntities.Contains(lastNode))
			{
				DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[lastNode];
				Entity val2 = Entity.Null;
				Entity val3 = Entity.Null;
				for (int i = 0; i < val.Length; i++)
				{
					Entity edge = val[i].m_Edge;
					if (!(edge != lastEdge) || !m_FixedData.HasComponent(edge))
					{
						continue;
					}
					Edge edge2 = m_EdgeData[edge];
					if (edge2.m_Start == lastNode)
					{
						if (bulldozeEntities.Add(edge))
						{
							AddEntity(ownerDefinitions, bulldozeEntities, attachedEntities, edge, owner, ownerDefinition, upgrade, parent: false, delete);
							val2 = edge;
							val3 = edge2.m_End;
						}
						break;
					}
					if (edge2.m_End == lastNode)
					{
						if (bulldozeEntities.Add(edge))
						{
							AddEntity(ownerDefinitions, bulldozeEntities, attachedEntities, edge, owner, ownerDefinition, upgrade, parent: false, delete);
							val2 = edge;
							val3 = edge2.m_Start;
						}
						break;
					}
				}
				lastEdge = val2;
				lastNode = val3;
			}
		}

		private void AddEntity(NativeHashMap<Entity, OwnerDefinition> ownerDefinitions, NativeHashSet<Entity> bulldozeEntities, NativeHashSet<Entity> attachedEntities, Entity entity, Entity owner, OwnerDefinition ownerDefinition, bool upgrade, bool parent, bool delete)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0620: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0694: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0631: Unknown result type (might be due to invalid IL or missing references)
			//IL_0636: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_063f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0644: Unknown result type (might be due to invalid IL or missing references)
			//IL_064a: Unknown result type (might be due to invalid IL or missing references)
			//IL_065a: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_066b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0670: Unknown result type (might be due to invalid IL or missing references)
			//IL_0678: Unknown result type (might be due to invalid IL or missing references)
			//IL_0679: Unknown result type (might be due to invalid IL or missing references)
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0684: Unknown result type (might be due to invalid IL or missing references)
			//IL_0721: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06be: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_073d: Unknown result type (might be due to invalid IL or missing references)
			//IL_072b: Unknown result type (might be due to invalid IL or missing references)
			//IL_072c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_074f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0751: Unknown result type (might be due to invalid IL or missing references)
			//IL_0756: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0567: Unknown result type (might be due to invalid IL or missing references)
			//IL_056c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_054e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05db: Unknown result type (might be due to invalid IL or missing references)
			//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_060e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0613: Unknown result type (might be due to invalid IL or missing references)
			//IL_0594: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0769: Unknown result type (might be due to invalid IL or missing references)
			//IL_076e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0776: Unknown result type (might be due to invalid IL or missing references)
			//IL_077b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0786: Unknown result type (might be due to invalid IL or missing references)
			//IL_0788: Unknown result type (might be due to invalid IL or missing references)
			//IL_0700: Unknown result type (might be due to invalid IL or missing references)
			//IL_0701: Unknown result type (might be due to invalid IL or missing references)
			//IL_0702: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0806: Unknown result type (might be due to invalid IL or missing references)
			//IL_0808: Unknown result type (might be due to invalid IL or missing references)
			//IL_080d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0815: Unknown result type (might be due to invalid IL or missing references)
			//IL_0816: Unknown result type (might be due to invalid IL or missing references)
			//IL_081b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0821: Unknown result type (might be due to invalid IL or missing references)
			//IL_0831: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_086b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0840: Unknown result type (might be due to invalid IL or missing references)
			//IL_0842: Unknown result type (might be due to invalid IL or missing references)
			//IL_0847: Unknown result type (might be due to invalid IL or missing references)
			//IL_084f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0850: Unknown result type (might be due to invalid IL or missing references)
			//IL_0855: Unknown result type (might be due to invalid IL or missing references)
			//IL_085b: Unknown result type (might be due to invalid IL or missing references)
			//IL_088e: Unknown result type (might be due to invalid IL or missing references)
			//IL_089b: Unknown result type (might be due to invalid IL or missing references)
			//IL_089c: Unknown result type (might be due to invalid IL or missing references)
			//IL_089d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
			Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Original = entity,
				m_Owner = owner
			};
			if (upgrade)
			{
				creationDefinition.m_Flags |= CreationFlags.Upgrade;
			}
			if (parent)
			{
				creationDefinition.m_Flags |= CreationFlags.Upgrade | CreationFlags.Parent;
			}
			if (delete)
			{
				creationDefinition.m_Flags |= CreationFlags.Delete;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
			if (ownerDefinition.m_Prefab != Entity.Null)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val, ownerDefinition);
			}
			if (m_EdgeData.HasComponent(entity))
			{
				if (m_EditorContainerData.HasComponent(entity))
				{
					creationDefinition.m_SubPrefab = m_EditorContainerData[entity].m_Prefab;
				}
				Edge edge = m_EdgeData[entity];
				NetCourse netCourse = default(NetCourse);
				netCourse.m_Curve = m_CurveData[entity].m_Bezier;
				netCourse.m_Length = MathUtils.Length(netCourse.m_Curve);
				netCourse.m_FixedIndex = -1;
				if (m_FixedData.HasComponent(entity))
				{
					netCourse.m_FixedIndex = m_FixedData[entity].m_Index;
				}
				netCourse.m_StartPosition.m_Entity = edge.m_Start;
				netCourse.m_StartPosition.m_Position = netCourse.m_Curve.a;
				netCourse.m_StartPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.StartTangent(netCourse.m_Curve));
				netCourse.m_StartPosition.m_CourseDelta = 0f;
				netCourse.m_EndPosition.m_Entity = edge.m_End;
				netCourse.m_EndPosition.m_Position = netCourse.m_Curve.d;
				netCourse.m_EndPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.EndTangent(netCourse.m_Curve));
				netCourse.m_EndPosition.m_CourseDelta = 1f;
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val, netCourse);
				ownerDefinition.m_Prefab = m_PrefabRefData[entity].m_Prefab;
				ownerDefinition.m_Position = netCourse.m_Curve.a;
				ownerDefinition.m_Rotation = quaternion.op_Implicit(new float4(netCourse.m_Curve.d, 0f));
			}
			else if (m_NodeData.HasComponent(entity))
			{
				if (m_EditorContainerData.HasComponent(entity))
				{
					creationDefinition.m_SubPrefab = m_EditorContainerData[entity].m_Prefab;
				}
				Game.Net.Node node = m_NodeData[entity];
				NetCourse netCourse2 = new NetCourse
				{
					m_Curve = new Bezier4x3(node.m_Position, node.m_Position, node.m_Position, node.m_Position),
					m_Length = 0f,
					m_FixedIndex = -1,
					m_StartPosition = 
					{
						m_Entity = entity,
						m_Position = node.m_Position,
						m_Rotation = node.m_Rotation,
						m_CourseDelta = 0f
					},
					m_EndPosition = 
					{
						m_Entity = entity,
						m_Position = node.m_Position,
						m_Rotation = node.m_Rotation,
						m_CourseDelta = 1f
					}
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val, netCourse2);
			}
			else if (m_TransformData.HasComponent(entity))
			{
				Transform transform = m_TransformData[entity];
				ObjectDefinition objectDefinition = new ObjectDefinition
				{
					m_Position = transform.m_Position,
					m_Rotation = transform.m_Rotation
				};
				Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
				if (m_ElevationData.TryGetComponent(entity, ref elevation))
				{
					objectDefinition.m_Elevation = elevation.m_Elevation;
					objectDefinition.m_ParentMesh = ObjectUtils.GetSubParentMesh(elevation.m_Flags);
				}
				else
				{
					objectDefinition.m_ParentMesh = -1;
				}
				Attached attached = default(Attached);
				PrefabRef prefabRef = default(PrefabRef);
				if (parent && m_AttachedData.TryGetComponent(entity, ref attached) && m_PrefabRefData.TryGetComponent(attached.m_Parent, ref prefabRef))
				{
					creationDefinition.m_Attached = prefabRef.m_Prefab;
					creationDefinition.m_Flags |= CreationFlags.Attach;
				}
				objectDefinition.m_Probability = 100;
				objectDefinition.m_PrefabSubIndex = -1;
				if (m_LocalTransformCacheData.HasComponent(entity))
				{
					LocalTransformCache localTransformCache = m_LocalTransformCacheData[entity];
					objectDefinition.m_LocalPosition = localTransformCache.m_Position;
					objectDefinition.m_LocalRotation = localTransformCache.m_Rotation;
					objectDefinition.m_ParentMesh = localTransformCache.m_ParentMesh;
					objectDefinition.m_GroupIndex = localTransformCache.m_GroupIndex;
					objectDefinition.m_Probability = localTransformCache.m_Probability;
					objectDefinition.m_PrefabSubIndex = localTransformCache.m_PrefabSubIndex;
				}
				else if (ownerDefinition.m_Prefab != Entity.Null)
				{
					Transform transform2 = ObjectUtils.WorldToLocal(ObjectUtils.InverseTransform(new Transform(ownerDefinition.m_Position, ownerDefinition.m_Rotation)), transform);
					objectDefinition.m_LocalPosition = transform2.m_Position;
					objectDefinition.m_LocalRotation = transform2.m_Rotation;
				}
				else if (m_TransformData.HasComponent(owner))
				{
					Transform transform3 = ObjectUtils.WorldToLocal(ObjectUtils.InverseTransform(m_TransformData[owner]), transform);
					objectDefinition.m_LocalPosition = transform3.m_Position;
					objectDefinition.m_LocalRotation = transform3.m_Rotation;
				}
				else
				{
					objectDefinition.m_LocalPosition = transform.m_Position;
					objectDefinition.m_LocalRotation = transform.m_Rotation;
				}
				if (m_EditorContainerData.HasComponent(entity))
				{
					EditorContainer editorContainer = m_EditorContainerData[entity];
					creationDefinition.m_SubPrefab = editorContainer.m_Prefab;
					objectDefinition.m_Scale = editorContainer.m_Scale;
					objectDefinition.m_Intensity = editorContainer.m_Intensity;
					objectDefinition.m_GroupIndex = editorContainer.m_GroupIndex;
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<ObjectDefinition>(val, objectDefinition);
				ownerDefinition.m_Prefab = m_PrefabRefData[entity].m_Prefab;
				ownerDefinition.m_Position = transform.m_Position;
				ownerDefinition.m_Rotation = transform.m_Rotation;
			}
			else if (m_AreaNodes.HasBuffer(entity))
			{
				DynamicBuffer<Game.Areas.Node> val2 = m_AreaNodes[entity];
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<Game.Areas.Node>(val).CopyFrom(val2.AsNativeArray());
				if (m_CachedNodes.HasBuffer(entity))
				{
					DynamicBuffer<LocalNodeCache> val3 = m_CachedNodes[entity];
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<LocalNodeCache>(val).CopyFrom(val3.AsNativeArray());
				}
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val, creationDefinition);
			if (delete && m_AttachedData.HasComponent(entity))
			{
				Attached attached2 = m_AttachedData[entity];
				AddAttachedParent(bulldozeEntities, ownerDefinitions, attachedEntities, attached2);
			}
			DynamicBuffer<Game.Objects.SubObject> val4 = default(DynamicBuffer<Game.Objects.SubObject>);
			if (delete && m_SubObjects.TryGetBuffer(entity, ref val4))
			{
				Attached attached3 = default(Attached);
				for (int i = 0; i < val4.Length; i++)
				{
					if (m_AttachedData.TryGetComponent(val4[i].m_SubObject, ref attached3))
					{
						AddAttachedParent(bulldozeEntities, ownerDefinitions, attachedEntities, attached3);
					}
				}
			}
			if (m_SubNets.HasBuffer(entity))
			{
				AddSubNets(bulldozeEntities, entity, ownerDefinition, delete);
			}
			if (!m_SubAreas.HasBuffer(entity))
			{
				return;
			}
			DynamicBuffer<Game.Areas.SubArea> val5 = m_SubAreas[entity];
			for (int j = 0; j < val5.Length; j++)
			{
				Entity area = val5[j].m_Area;
				val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
				CreationDefinition creationDefinition2 = new CreationDefinition
				{
					m_Original = area
				};
				if (delete)
				{
					creationDefinition2.m_Flags |= CreationFlags.Delete;
					if (!m_LotData.HasComponent(area))
					{
						creationDefinition2.m_Flags |= CreationFlags.Hidden;
					}
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val, creationDefinition2);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
				if (ownerDefinition.m_Prefab != Entity.Null)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val, ownerDefinition);
				}
				DynamicBuffer<Game.Areas.Node> val6 = m_AreaNodes[area];
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<Game.Areas.Node>(val).CopyFrom(val6.AsNativeArray());
				if (m_CachedNodes.HasBuffer(area))
				{
					DynamicBuffer<LocalNodeCache> val7 = m_CachedNodes[area];
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<LocalNodeCache>(val).CopyFrom(val7.AsNativeArray());
				}
				if (!m_SubObjects.TryGetBuffer(area, ref val4))
				{
					continue;
				}
				for (int k = 0; k < val4.Length; k++)
				{
					Game.Objects.SubObject subObject = val4[k];
					if (m_BuildingData.HasComponent(subObject.m_SubObject))
					{
						AddEntity(ownerDefinitions, bulldozeEntities, attachedEntities, subObject.m_SubObject, Entity.Null, default(OwnerDefinition), upgrade: false, parent: false, delete: true);
					}
				}
			}
		}

		private void AddAttachedParent(NativeHashSet<Entity> bulldozeEntities, NativeHashMap<Entity, OwnerDefinition> ownerDefinitions, NativeHashSet<Entity> attachedEntities, Attached attached)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			Entity val = attached.m_Parent;
			Owner owner = default(Owner);
			while (m_OwnerData.TryGetComponent(val, ref owner))
			{
				val = owner.m_Owner;
				if (bulldozeEntities.Contains(val) || ownerDefinitions.ContainsKey(val))
				{
					return;
				}
			}
			if (m_EdgeData.HasComponent(attached.m_Parent))
			{
				Edge edge = m_EdgeData[attached.m_Parent];
				if (!bulldozeEntities.Contains(attached.m_Parent) && !bulldozeEntities.Contains(edge.m_Start) && !bulldozeEntities.Contains(edge.m_End) && attachedEntities.Add(attached.m_Parent))
				{
					Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
					CreationDefinition creationDefinition = new CreationDefinition
					{
						m_Original = attached.m_Parent
					};
					creationDefinition.m_Flags |= CreationFlags.Align;
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val2, creationDefinition);
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2, default(Updated));
					NetCourse netCourse = default(NetCourse);
					netCourse.m_Curve = m_CurveData[attached.m_Parent].m_Bezier;
					netCourse.m_Length = MathUtils.Length(netCourse.m_Curve);
					netCourse.m_FixedIndex = -1;
					netCourse.m_StartPosition.m_Entity = edge.m_Start;
					netCourse.m_StartPosition.m_Position = netCourse.m_Curve.a;
					netCourse.m_StartPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.StartTangent(netCourse.m_Curve));
					netCourse.m_StartPosition.m_CourseDelta = 0f;
					netCourse.m_EndPosition.m_Entity = edge.m_End;
					netCourse.m_EndPosition.m_Position = netCourse.m_Curve.d;
					netCourse.m_EndPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.EndTangent(netCourse.m_Curve));
					netCourse.m_EndPosition.m_CourseDelta = 1f;
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val2, netCourse);
				}
			}
			else if (m_NodeData.HasComponent(attached.m_Parent) && !bulldozeEntities.Contains(attached.m_Parent) && attachedEntities.Add(attached.m_Parent))
			{
				Game.Net.Node node = m_NodeData[attached.m_Parent];
				Entity val3 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
				CreationDefinition creationDefinition2 = new CreationDefinition
				{
					m_Original = attached.m_Parent
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val3, creationDefinition2);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val3, default(Updated));
				NetCourse netCourse2 = new NetCourse
				{
					m_Curve = new Bezier4x3(node.m_Position, node.m_Position, node.m_Position, node.m_Position),
					m_Length = 0f,
					m_FixedIndex = -1,
					m_StartPosition = 
					{
						m_Entity = attached.m_Parent,
						m_Position = node.m_Position,
						m_Rotation = node.m_Rotation,
						m_CourseDelta = 0f
					},
					m_EndPosition = 
					{
						m_Entity = attached.m_Parent,
						m_Position = node.m_Position,
						m_Rotation = node.m_Rotation,
						m_CourseDelta = 1f
					}
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val3, netCourse2);
			}
		}

		private void AddSubNets(NativeHashSet<Entity> bulldozeEntities, Entity entity, OwnerDefinition ownerDefinition, bool delete)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Net.SubNet> val = m_SubNets[entity];
			for (int i = 0; i < val.Length; i++)
			{
				Entity subNet = val[i].m_SubNet;
				if (m_NodeData.HasComponent(subNet))
				{
					if (!HasEdgeStartOrEnd(subNet, entity) && !bulldozeEntities.Contains(subNet))
					{
						Game.Net.Node node = m_NodeData[subNet];
						Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
						CreationDefinition creationDefinition = new CreationDefinition
						{
							m_Original = subNet
						};
						if (delete)
						{
							creationDefinition.m_Flags |= CreationFlags.Delete;
						}
						if (m_EditorContainerData.HasComponent(subNet))
						{
							creationDefinition.m_SubPrefab = m_EditorContainerData[subNet].m_Prefab;
						}
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val2, creationDefinition);
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2, default(Updated));
						if (ownerDefinition.m_Prefab != Entity.Null)
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val2, ownerDefinition);
						}
						NetCourse netCourse = new NetCourse
						{
							m_Curve = new Bezier4x3(node.m_Position, node.m_Position, node.m_Position, node.m_Position),
							m_Length = 0f,
							m_FixedIndex = -1,
							m_StartPosition = 
							{
								m_Entity = subNet,
								m_Position = node.m_Position,
								m_Rotation = node.m_Rotation,
								m_CourseDelta = 0f
							},
							m_EndPosition = 
							{
								m_Entity = subNet,
								m_Position = node.m_Position,
								m_Rotation = node.m_Rotation,
								m_CourseDelta = 1f
							}
						};
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val2, netCourse);
					}
				}
				else
				{
					if (!m_EdgeData.HasComponent(subNet))
					{
						continue;
					}
					Edge edge = m_EdgeData[subNet];
					if (!bulldozeEntities.Contains(subNet) && !bulldozeEntities.Contains(edge.m_Start) && !bulldozeEntities.Contains(edge.m_End))
					{
						Entity val3 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
						CreationDefinition creationDefinition2 = new CreationDefinition
						{
							m_Original = subNet
						};
						if (delete)
						{
							creationDefinition2.m_Flags |= CreationFlags.Delete;
						}
						if (m_EditorContainerData.HasComponent(subNet))
						{
							creationDefinition2.m_SubPrefab = m_EditorContainerData[subNet].m_Prefab;
						}
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val3, creationDefinition2);
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val3, default(Updated));
						if (ownerDefinition.m_Prefab != Entity.Null)
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val3, ownerDefinition);
						}
						NetCourse netCourse2 = default(NetCourse);
						netCourse2.m_Curve = m_CurveData[subNet].m_Bezier;
						netCourse2.m_Length = MathUtils.Length(netCourse2.m_Curve);
						netCourse2.m_FixedIndex = -1;
						netCourse2.m_StartPosition.m_Entity = edge.m_Start;
						netCourse2.m_StartPosition.m_Position = netCourse2.m_Curve.a;
						netCourse2.m_StartPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.StartTangent(netCourse2.m_Curve));
						netCourse2.m_StartPosition.m_CourseDelta = 0f;
						netCourse2.m_EndPosition.m_Entity = edge.m_End;
						netCourse2.m_EndPosition.m_Position = netCourse2.m_Curve.d;
						netCourse2.m_EndPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.EndTangent(netCourse2.m_Curve));
						netCourse2.m_EndPosition.m_CourseDelta = 1f;
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val3, netCourse2);
						if (m_SubNets.HasBuffer(subNet))
						{
							AddSubNets(ownerDefinition: new OwnerDefinition
							{
								m_Prefab = m_PrefabRefData[subNet].m_Prefab,
								m_Position = netCourse2.m_Curve.a,
								m_Rotation = quaternion.op_Implicit(new float4(netCourse2.m_Curve.d, 0f))
							}, bulldozeEntities: bulldozeEntities, entity: subNet, delete: delete);
						}
					}
				}
			}
		}

		private bool HasEdgeStartOrEnd(Entity node, Entity owner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[node];
			for (int i = 0; i < val.Length; i++)
			{
				Entity edge = val[i].m_Edge;
				Edge edge2 = m_EdgeData[edge];
				if ((edge2.m_Start == node || edge2.m_End == node) && m_OwnerData.HasComponent(edge) && m_OwnerData[edge].m_Owner == owner)
				{
					return true;
				}
			}
			return false;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Static> __Game_Objects_Static_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> __Game_Buildings_ServiceUpgrade_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LocalNodeCache> __Game_Tools_LocalNodeCache_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> __Game_Tools_LocalTransformCache_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Fixed> __Game_Net_Fixed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> __Game_Areas_Lot_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EditorContainer> __Game_Tools_EditorContainer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> __Game_Objects_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Placeholder> __Game_Objects_Placeholder_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attachment> __Game_Objects_Attachment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

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
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Static_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Static>(true);
			__Game_Objects_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.OutsideConnection>(true);
			__Game_Buildings_ServiceUpgrade_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.ServiceUpgrade>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Node>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Tools_LocalNodeCache_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LocalNodeCache>(true);
			__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubNet>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Tools_LocalTransformCache_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalTransformCache>(true);
			__Game_Net_Fixed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Fixed>(true);
			__Game_Areas_Lot_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Areas.Lot>(true);
			__Game_Tools_EditorContainer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EditorContainer>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Objects_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.Elevation>(true);
			__Game_Objects_Placeholder_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Placeholder>(true);
			__Game_Objects_Attachment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attachment>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
		}
	}

	public const string kToolID = "Bulldoze Tool";

	public Action EventConfirmationRequested;

	private ToolOutputBarrier m_ToolOutputBarrier;

	private AudioManager m_AudioManager;

	private AchievementTriggerSystem m_AchievementTriggerSystem;

	private EntityQuery m_DefinitionQuery;

	private EntityQuery m_BuildingQuery;

	private EntityQuery m_RoadQuery;

	private EntityQuery m_PlantQuery;

	private EntityQuery m_SoundQuery;

	private ControlPoint m_LastRaycastPoint;

	private State m_State;

	private NativeList<ControlPoint> m_ControlPoints;

	private IProxyAction m_Bulldoze;

	private IProxyAction m_BulldozeDiscard;

	private bool m_ApplyBlocked;

	private TypeHandle __TypeHandle;

	public override string toolID => "Bulldoze Tool";

	public override int uiModeIndex => (int)actualMode;

	public override bool allowUnderground => true;

	public Mode mode { get; set; }

	public Mode actualMode
	{
		get
		{
			if (!m_ToolSystem.actionMode.IsEditor())
			{
				return Mode.MainElements;
			}
			return mode;
		}
	}

	public bool underground { get; set; }

	public bool allowManipulation { get; set; }

	public bool debugBypassBulldozeConfirmation { get; set; }

	public BulldozePrefab prefab { get; set; }

	private protected override IEnumerable<IProxyAction> toolActions
	{
		get
		{
			yield return m_Bulldoze;
			yield return m_BulldozeDiscard;
		}
	}

	public override void GetUIModes(List<ToolMode> modes)
	{
		modes.Add(new ToolMode(Mode.MainElements.ToString(), 0));
		if (m_ToolSystem.actionMode.IsEditor())
		{
			modes.Add(new ToolMode(Mode.SubElements.ToString(), 1));
			modes.Add(new ToolMode(Mode.Everything.ToString(), 2));
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		m_AudioManager = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AudioManager>();
		m_AchievementTriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AchievementTriggerSystem>();
		m_ControlPoints = new NativeList<ControlPoint>(4, AllocatorHandle.op_Implicit((Allocator)4));
		m_DefinitionQuery = GetDefinitionQuery();
		m_BuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_RoadQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_PlantQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Plant>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_SoundQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ToolUXSoundSettingsData>() });
		m_Bulldoze = InputManager.instance.toolActionCollection.GetActionState("Bulldoze", "BulldozeToolSystem");
		m_BulldozeDiscard = InputManager.instance.toolActionCollection.GetActionState("Bulldoze Discard", "BulldozeToolSystem");
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_ControlPoints.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		base.OnStartRunning();
		m_ControlPoints.Clear();
		m_LastRaycastPoint = default(ControlPoint);
		m_State = State.Default;
		m_ApplyBlocked = false;
		base.requireUnderground = false;
		base.requireStopIcons = false;
		base.requireAreas = AreaTypeMask.None;
		base.requireNet = Layer.None;
	}

	private protected override void UpdateActions()
	{
		using (ProxyAction.DeferStateUpdating())
		{
			base.applyActionOverride = m_Bulldoze;
			base.applyAction.enabled = base.actionsEnabled && m_State != State.Waiting && m_ControlPoints.Length != 0;
			base.cancelActionOverride = m_BulldozeDiscard;
			base.cancelAction.enabled = base.actionsEnabled && m_State == State.Applying && IsMultiSelection();
		}
	}

	private bool IsMultiSelection()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (m_ControlPoints.Length == 0)
		{
			return false;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Game.Net.Node>(m_ControlPoints[0].m_OriginalEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Edge>(m_ControlPoints[0].m_OriginalEntity))
			{
				return m_ControlPoints.Length > 1;
			}
		}
		return m_ControlPoints.Length > 4;
	}

	public override PrefabBase GetPrefab()
	{
		return prefab;
	}

	public override bool TrySetPrefab(PrefabBase prefab)
	{
		if (prefab is BulldozePrefab bulldozePrefab)
		{
			this.prefab = bulldozePrefab;
			return true;
		}
		return false;
	}

	public override void SetUnderground(bool underground)
	{
		this.underground = underground;
	}

	public override void ElevationUp()
	{
		underground = false;
	}

	public override void ElevationDown()
	{
		underground = true;
	}

	public override void ElevationScroll()
	{
		underground = !underground;
	}

	public override void InitializeRaycast()
	{
		base.InitializeRaycast();
		m_ToolRaycastSystem.typeMask = TypeMask.StaticObjects | TypeMask.Net;
		m_ToolRaycastSystem.netLayerMask = Layer.All;
		m_ToolRaycastSystem.raycastFlags |= RaycastFlags.BuildingLots;
		if (underground)
		{
			m_ToolRaycastSystem.collisionMask = CollisionMask.Underground;
		}
		else
		{
			m_ToolRaycastSystem.collisionMask = CollisionMask.OnGround | CollisionMask.Overground;
		}
		switch (actualMode)
		{
		case Mode.SubElements:
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.SubElements | RaycastFlags.NoMainElements;
			break;
		case Mode.Everything:
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.SubElements;
			break;
		}
		if (m_ToolSystem.actionMode.IsEditor())
		{
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.Markers | RaycastFlags.UpgradeIsMain | RaycastFlags.EditorContainers;
			m_ToolRaycastSystem.typeMask |= TypeMask.Areas;
			if (underground)
			{
				m_ToolRaycastSystem.areaTypeMask = AreaTypeMask.Spaces;
			}
			else
			{
				m_ToolRaycastSystem.areaTypeMask = AreaTypeMask.Lots | AreaTypeMask.Spaces | AreaTypeMask.Surfaces;
			}
		}
		else
		{
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.SubBuildings;
			if (!underground)
			{
				m_ToolRaycastSystem.typeMask |= TypeMask.Areas;
				m_ToolRaycastSystem.areaTypeMask = AreaTypeMask.Lots | AreaTypeMask.Surfaces;
			}
		}
		if (allowManipulation)
		{
			m_ToolRaycastSystem.typeMask |= TypeMask.MovingObjects;
		}
		m_ToolRaycastSystem.raycastFlags |= RaycastFlags.Placeholders | RaycastFlags.Decals;
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		base.requireUnderground = underground;
		base.requireStopIcons = true;
		if (underground)
		{
			base.requireAreas = (m_ToolSystem.actionMode.IsEditor() ? AreaTypeMask.Spaces : AreaTypeMask.None);
			base.requireNet = Layer.None;
		}
		else
		{
			base.requireAreas = (m_ToolSystem.actionMode.IsEditor() ? (AreaTypeMask.Lots | AreaTypeMask.Spaces | AreaTypeMask.Surfaces) : AreaTypeMask.None);
			base.requireNet = Layer.Waterway;
		}
		UpdateActions();
		if (m_State == State.Applying && !base.applyAction.enabled)
		{
			m_State = State.Default;
			m_ControlPoints.Clear();
			base.applyMode = ApplyMode.Clear;
			inputDeps = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
		}
		switch (m_State)
		{
		case State.Default:
			if (m_ApplyBlocked)
			{
				if (base.applyAction.WasReleasedThisFrame() || base.secondaryApplyAction.WasReleasedThisFrame())
				{
					m_ApplyBlocked = false;
				}
				return Update(inputDeps, fullUpdate: false);
			}
			if (base.cancelAction.IsInProgress())
			{
				base.applyMode = ApplyMode.None;
				return inputDeps;
			}
			if (m_ControlPoints.Length > 0 && base.applyAction.WasPressedThisFrame())
			{
				m_State = State.Applying;
				return Update(inputDeps, fullUpdate: true);
			}
			return Update(inputDeps, fullUpdate: false);
		case State.Applying:
			if (base.cancelAction.IsInProgress())
			{
				m_State = State.Default;
				m_ApplyBlocked = true;
				if (m_ControlPoints.Length >= 2)
				{
					m_ControlPoints.RemoveRange(0, m_ControlPoints.Length - 1);
				}
				return Update(inputDeps, fullUpdate: true);
			}
			if (!base.applyAction.IsInProgress())
			{
				if (!((EntityQuery)(ref m_BuildingQuery)).IsEmptyIgnoreFilter && !m_ToolSystem.actionMode.IsEditor() && EventConfirmationRequested != null && !debugBypassBulldozeConfirmation && ConfirmationNeeded())
				{
					m_State = State.Waiting;
					base.applyMode = ApplyMode.None;
					EventConfirmationRequested();
					return inputDeps;
				}
				m_State = State.Default;
				return Apply(inputDeps);
			}
			return Update(inputDeps, fullUpdate: false);
		case State.Confirmed:
			m_State = State.Default;
			return Apply(inputDeps);
		case State.Cancelled:
			m_State = State.Default;
			if (m_ControlPoints.Length >= 2)
			{
				m_ControlPoints.RemoveRange(0, m_ControlPoints.Length - 1);
			}
			return Update(inputDeps, fullUpdate: true);
		default:
			base.applyMode = ApplyMode.None;
			return inputDeps;
		}
	}

	private bool ConfirmationNeeded()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_BuildingQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		bool result = false;
		PrefabRef prefabRef = default(PrefabRef);
		for (int i = 0; i < val.Length; i++)
		{
			Entity val2 = val[i];
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if ((((EntityManager)(ref entityManager)).GetComponentData<Temp>(val2).m_Flags & TempFlags.Delete) == 0 || !EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val2, ref prefabRef))
			{
				continue;
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<SpawnableBuildingData>(prefabRef.m_Prefab))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<SignatureBuildingData>(prefabRef.m_Prefab))
				{
					continue;
				}
			}
			result = true;
		}
		val.Dispose();
		return result;
	}

	public void ConfirmAction(bool confirm)
	{
		if (m_State == State.Waiting)
		{
			m_State = (confirm ? State.Confirmed : State.Cancelled);
		}
	}

	private JobHandle Apply(JobHandle inputDeps)
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (GetAllowApply())
		{
			int num = ((EntityQuery)(ref m_BuildingQuery)).CalculateEntityCount();
			if (num > 0 || ((EntityQuery)(ref m_RoadQuery)).CalculateEntityCount() > 0)
			{
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_BulldozeSound);
			}
			else
			{
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PropPlantBulldozeSound);
			}
			if (num > 0 && m_ToolSystem.actionMode.IsGame())
			{
				m_AchievementTriggerSystem.m_SquasherDownerBuffer.AddProgress(num);
			}
			base.applyMode = ApplyMode.Apply;
			m_LastRaycastPoint = default(ControlPoint);
			m_ControlPoints.Clear();
			return DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
		}
		if (m_ControlPoints.Length >= 2)
		{
			m_ControlPoints.RemoveRange(0, m_ControlPoints.Length - 1);
		}
		return Update(inputDeps, fullUpdate: true);
	}

	protected override bool GetRaycastResult(out ControlPoint controlPoint)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (GetRaycastResult(out Entity entity, out RaycastHit hit))
		{
			EntityManager entityManager;
			if (m_ToolSystem.actionMode.IsEditor())
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				Owner owner = default(Owner);
				if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.ServiceUpgrade>(entity) && EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, entity, ref owner))
				{
					controlPoint.m_OriginalEntity = owner.m_Owner;
				}
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Net.Node>(entity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Edge>(hit.m_HitEntity))
				{
					entity = hit.m_HitEntity;
				}
			}
			controlPoint = new ControlPoint(entity, hit);
			return true;
		}
		controlPoint = default(ControlPoint);
		return false;
	}

	protected override bool GetRaycastResult(out ControlPoint controlPoint, out bool forceUpdate)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (GetRaycastResult(out var entity, out var hit, out forceUpdate))
		{
			EntityManager entityManager;
			if (m_ToolSystem.actionMode.IsEditor())
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				Owner owner = default(Owner);
				if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.ServiceUpgrade>(entity) && EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, entity, ref owner))
				{
					controlPoint.m_OriginalEntity = owner.m_Owner;
				}
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Net.Node>(entity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Edge>(hit.m_HitEntity))
				{
					entity = hit.m_HitEntity;
				}
			}
			controlPoint = new ControlPoint(entity, hit);
			return true;
		}
		controlPoint = default(ControlPoint);
		return false;
	}

	private JobHandle Update(JobHandle inputDeps, bool fullUpdate)
	{
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		if (GetRaycastResult(out ControlPoint controlPoint, out bool forceUpdate))
		{
			fullUpdate = fullUpdate || forceUpdate;
			if (m_ControlPoints.Length == 0)
			{
				base.applyMode = ApplyMode.Clear;
				m_ControlPoints.Add(ref controlPoint);
				inputDeps = SnapControlPoints(inputDeps);
				inputDeps = UpdateDefinitions(inputDeps);
			}
			else
			{
				base.applyMode = ApplyMode.None;
				if (fullUpdate || !m_LastRaycastPoint.Equals(controlPoint))
				{
					m_LastRaycastPoint = controlPoint;
					ControlPoint controlPoint2 = m_ControlPoints[m_ControlPoints.Length - 1];
					if (m_State == State.Applying && controlPoint.m_OriginalEntity != m_ControlPoints[m_ControlPoints.Length - 1].m_OriginalEntity)
					{
						m_ControlPoints.Add(ref controlPoint);
					}
					else
					{
						m_ControlPoints[m_ControlPoints.Length - 1] = controlPoint;
					}
					inputDeps = SnapControlPoints(inputDeps);
					JobHandle.ScheduleBatchedJobs();
					((JobHandle)(ref inputDeps)).Complete();
					ControlPoint other = default(ControlPoint);
					if (m_ControlPoints.Length != 0)
					{
						other = m_ControlPoints[m_ControlPoints.Length - 1];
					}
					if (fullUpdate || !controlPoint2.EqualsIgnoreHit(other))
					{
						base.applyMode = ApplyMode.Clear;
						inputDeps = UpdateDefinitions(inputDeps);
					}
				}
			}
		}
		else
		{
			if (m_State == State.Default)
			{
				m_ControlPoints.Clear();
			}
			base.applyMode = ApplyMode.Clear;
			inputDeps = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
		}
		return inputDeps;
	}

	private JobHandle SnapControlPoints(JobHandle inputDeps)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		return IJobExtensions.Schedule<SnapJob>(new SnapJob
		{
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_Mode = actualMode,
			m_State = m_State,
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StaticData = InternalCompilerInterface.GetComponentLookup<Static>(ref __TypeHandle.__Game_Objects_Static_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionData = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpgradeData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.ServiceUpgrade>(ref __TypeHandle.__Game_Buildings_ServiceUpgrade_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControlPoints = m_ControlPoints
		}, inputDeps);
	}

	private JobHandle UpdateDefinitions(JobHandle inputDeps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
		JobHandle val2 = IJobExtensions.Schedule<CreateDefinitionsJob>(new CreateDefinitionsJob
		{
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_Mode = actualMode,
			m_State = m_State,
			m_ControlPoints = m_ControlPoints,
			m_CachedNodes = InternalCompilerInterface.GetBufferLookup<LocalNodeCache>(ref __TypeHandle.__Game_Tools_LocalNodeCache_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubNets = InternalCompilerInterface.GetBufferLookup<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LocalTransformCacheData = InternalCompilerInterface.GetComponentLookup<LocalTransformCache>(ref __TypeHandle.__Game_Tools_LocalTransformCache_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FixedData = InternalCompilerInterface.GetComponentLookup<Fixed>(ref __TypeHandle.__Game_Net_Fixed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LotData = InternalCompilerInterface.GetComponentLookup<Game.Areas.Lot>(ref __TypeHandle.__Game_Areas_Lot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EditorContainerData = InternalCompilerInterface.GetComponentLookup<EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachedData = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElevationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceholderData = InternalCompilerInterface.GetComponentLookup<Placeholder>(ref __TypeHandle.__Game_Objects_Placeholder_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachmentData = InternalCompilerInterface.GetComponentLookup<Attachment>(ref __TypeHandle.__Game_Objects_Attachment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpgradeData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.ServiceUpgrade>(ref __TypeHandle.__Game_Buildings_ServiceUpgrade_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CommandBuffer = m_ToolOutputBarrier.CreateCommandBuffer()
		}, inputDeps);
		((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val2);
		return JobHandle.CombineDependencies(val, val2);
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
		base.OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public BulldozeToolSystem()
	{
	}
}
