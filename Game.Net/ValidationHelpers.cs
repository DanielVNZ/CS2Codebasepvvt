using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Common;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Net;

public static class ValidationHelpers
{
	private struct NetIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
	{
		public Entity m_TopLevelEntity;

		public Entity m_EdgeEntity;

		public Entity m_OriginalEntity;

		public Bounds3 m_Bounds;

		public bool m_Essential;

		public bool m_EditorMode;

		public Edge m_Edge;

		public Edge m_OwnerEdge;

		public Edge m_OriginalNodes;

		public Edge m_NodeOwners;

		public NativeArray<ConnectedNode> m_ConnectedNodes;

		public NativeArray<ConnectedNode> m_OriginalConnectedNodes;

		public EdgeGeometry m_EdgeGeometryData;

		public StartNodeGeometry m_StartNodeGeometryData;

		public EndNodeGeometry m_EndNodeGeometryData;

		public NetCompositionData m_EdgeCompositionData;

		public NetCompositionData m_StartCompositionData;

		public NetCompositionData m_EndCompositionData;

		public CollisionMask m_EdgeCollisionMask;

		public CollisionMask m_StartCollisionMask;

		public CollisionMask m_EndCollisionMask;

		public CollisionMask m_CombinedCollisionMask;

		public ValidationSystem.EntityData m_Data;

		public ParallelWriter<ErrorData> m_ErrorQueue;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			if ((m_CombinedCollisionMask & CollisionMask.OnGround) != 0)
			{
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_Bounds)).xz);
			}
			return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, Entity edgeEntity2)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			if ((m_CombinedCollisionMask & CollisionMask.OnGround) != 0)
			{
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_Bounds)).xz))
				{
					return;
				}
			}
			else if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds))
			{
				return;
			}
			if (m_Data.m_Hidden.HasComponent(edgeEntity2) || !m_Data.m_Composition.HasComponent(edgeEntity2))
			{
				return;
			}
			Entity val = edgeEntity2;
			bool hasOwner = false;
			Owner owner = default(Owner);
			if (m_Data.m_Owner.TryGetComponent(val, ref owner))
			{
				Edge edge = default(Edge);
				if (m_Data.m_Edge.TryGetComponent(owner.m_Owner, ref edge) && (m_OriginalNodes.m_Start == edge.m_Start || m_OriginalNodes.m_Start == edge.m_End || m_OriginalNodes.m_End == edge.m_Start || m_OriginalNodes.m_End == edge.m_End))
				{
					return;
				}
				while (!m_Data.m_Building.HasComponent(val))
				{
					hasOwner = true;
					if (m_Data.m_AssetStamp.HasComponent(owner.m_Owner))
					{
						break;
					}
					val = owner.m_Owner;
					if (!m_Data.m_Owner.TryGetComponent(val, ref owner))
					{
						break;
					}
				}
			}
			if (!(edgeEntity2 == m_NodeOwners.m_Start) && !(edgeEntity2 == m_NodeOwners.m_End))
			{
				Edge edgeData = m_Data.m_Edge[edgeEntity2];
				Composition compositionData = m_Data.m_Composition[edgeEntity2];
				EdgeGeometry edgeGeometryData = m_Data.m_EdgeGeometry[edgeEntity2];
				StartNodeGeometry startNodeGeometryData = m_Data.m_StartNodeGeometry[edgeEntity2];
				EndNodeGeometry endNodeGeometryData = m_Data.m_EndNodeGeometry[edgeEntity2];
				if (!(m_OwnerEdge.m_Start == edgeData.m_Start) && !(m_OwnerEdge.m_Start == edgeData.m_End) && !(m_OwnerEdge.m_End == edgeData.m_Start) && !(m_OwnerEdge.m_End == edgeData.m_End) && (!m_Data.m_Owner.TryGetComponent(edgeData.m_Start, ref owner) || !(owner.m_Owner == m_OriginalEntity)) && (!m_Data.m_Owner.TryGetComponent(edgeData.m_End, ref owner) || !(owner.m_Owner == m_OriginalEntity)))
				{
					CheckOverlap(val, edgeEntity2, bounds.m_Bounds, edgeData, compositionData, edgeGeometryData, startNodeGeometryData, endNodeGeometryData, essential: false, hasOwner);
				}
			}
		}

		public void CheckOverlap(Entity topLevelEntity2, Entity edgeEntity2, Bounds3 bounds2, Edge edgeData2, Composition compositionData2, EdgeGeometry edgeGeometryData2, StartNodeGeometry startNodeGeometryData2, EndNodeGeometry endNodeGeometryData2, bool essential, bool hasOwner)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0524: Unknown result type (might be due to invalid IL or missing references)
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_053a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0701: Unknown result type (might be due to invalid IL or missing references)
			//IL_0707: Unknown result type (might be due to invalid IL or missing references)
			//IL_070d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0714: Unknown result type (might be due to invalid IL or missing references)
			//IL_071b: Unknown result type (might be due to invalid IL or missing references)
			//IL_063d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0648: Unknown result type (might be due to invalid IL or missing references)
			//IL_064e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0654: Unknown result type (might be due to invalid IL or missing references)
			//IL_065b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0662: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0601: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_056c: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_084a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0855: Unknown result type (might be due to invalid IL or missing references)
			//IL_085b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0861: Unknown result type (might be due to invalid IL or missing references)
			//IL_0868: Unknown result type (might be due to invalid IL or missing references)
			//IL_086f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_079c: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07af: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0744: Unknown result type (might be due to invalid IL or missing references)
			//IL_074f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0754: Unknown result type (might be due to invalid IL or missing references)
			//IL_0757: Unknown result type (might be due to invalid IL or missing references)
			//IL_068e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0699: Unknown result type (might be due to invalid IL or missing references)
			//IL_069e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0582: Unknown result type (might be due to invalid IL or missing references)
			//IL_058b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_099e: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09af: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0903: Unknown result type (might be due to invalid IL or missing references)
			//IL_090a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0898: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b10: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a50: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0936: Unknown result type (might be due to invalid IL or missing references)
			//IL_0941: Unknown result type (might be due to invalid IL or missing references)
			//IL_0946: Unknown result type (might be due to invalid IL or missing references)
			//IL_0949: Unknown result type (might be due to invalid IL or missing references)
			//IL_0809: Unknown result type (might be due to invalid IL or missing references)
			//IL_081b: Unknown result type (might be due to invalid IL or missing references)
			//IL_082c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0831: Unknown result type (might be due to invalid IL or missing references)
			//IL_083b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0840: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b50: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_095d: Unknown result type (might be due to invalid IL or missing references)
			//IL_096f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0980: Unknown result type (might be due to invalid IL or missing references)
			//IL_0985: Unknown result type (might be due to invalid IL or missing references)
			//IL_098f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0994: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0caa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0caf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c82: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d51: Unknown result type (might be due to invalid IL or missing references)
			NetCompositionData netCompositionData = m_Data.m_PrefabComposition[compositionData2.m_Edge];
			NetCompositionData netCompositionData2 = m_Data.m_PrefabComposition[compositionData2.m_StartNode];
			NetCompositionData netCompositionData3 = m_Data.m_PrefabComposition[compositionData2.m_EndNode];
			CollisionMask collisionMask = NetUtils.GetCollisionMask(netCompositionData, !m_EditorMode || hasOwner);
			CollisionMask collisionMask2 = NetUtils.GetCollisionMask(netCompositionData2, !m_EditorMode || hasOwner);
			CollisionMask collisionMask3 = NetUtils.GetCollisionMask(netCompositionData3, !m_EditorMode || hasOwner);
			CollisionMask collisionMask4 = collisionMask | collisionMask2 | collisionMask3;
			if ((m_CombinedCollisionMask & collisionMask4) == 0 || ((m_CombinedCollisionMask & CollisionMask.OnGround) != 0 && !CommonUtils.ExclusiveGroundCollision(m_CombinedCollisionMask, collisionMask4) && !MathUtils.Intersect(bounds2, m_Bounds)))
			{
				return;
			}
			ErrorData errorData = default(ErrorData);
			Bounds3 intersection = default(Bounds3);
			intersection.min = float3.op_Implicit(float.MaxValue);
			intersection.max = float3.op_Implicit(float.MinValue);
			Bounds2 intersection2 = default(Bounds2);
			intersection2.min = float2.op_Implicit(float.MaxValue);
			intersection2.max = float2.op_Implicit(float.MinValue);
			Bounds1 val = default(Bounds1);
			val.min = float.MaxValue;
			val.max = float.MinValue;
			DynamicBuffer<ConnectedNode> val2 = m_Data.m_ConnectedNodes[edgeEntity2];
			if ((m_EdgeCollisionMask & collisionMask) != 0)
			{
				if (CommonUtils.ExclusiveGroundCollision(m_EdgeCollisionMask, collisionMask))
				{
					if (ValidationHelpers.Intersect(m_OriginalNodes, edgeData2, m_EdgeGeometryData, edgeGeometryData2, m_EdgeCompositionData, netCompositionData, ref intersection2))
					{
						errorData.m_ErrorType = ErrorType.OverlapExisting;
						val |= MathUtils.Center(((Bounds3)(ref m_EdgeGeometryData.m_Bounds)).y & ((Bounds3)(ref edgeGeometryData2.m_Bounds)).y);
					}
				}
				else if (ValidationHelpers.Intersect(m_OriginalNodes, edgeData2, m_EdgeGeometryData, edgeGeometryData2, m_EdgeCompositionData, netCompositionData, ref intersection))
				{
					errorData.m_ErrorType = ErrorType.OverlapExisting;
				}
			}
			if ((m_EdgeCollisionMask & collisionMask2) != 0)
			{
				if (CommonUtils.ExclusiveGroundCollision(m_EdgeCollisionMask, collisionMask2))
				{
					if (ValidationHelpers.Intersect(m_Edge, m_OriginalNodes, m_ConnectedNodes, m_OriginalConnectedNodes, edgeData2.m_Start, m_EdgeGeometryData, startNodeGeometryData2.m_Geometry, m_EdgeCompositionData, netCompositionData2, ref intersection2))
					{
						if (!IgnoreCollision(edgeEntity2, edgeData2.m_Start, m_Edge))
						{
							errorData.m_ErrorType = ErrorType.OverlapExisting;
						}
						val |= MathUtils.Center(((Bounds3)(ref m_EdgeGeometryData.m_Bounds)).y & ((Bounds3)(ref startNodeGeometryData2.m_Geometry.m_Bounds)).y);
					}
				}
				else if (ValidationHelpers.Intersect(m_Edge, m_OriginalNodes, m_ConnectedNodes, m_OriginalConnectedNodes, edgeData2.m_Start, m_EdgeGeometryData, startNodeGeometryData2.m_Geometry, m_EdgeCompositionData, netCompositionData2, ref intersection) && !IgnoreCollision(edgeEntity2, edgeData2.m_Start, m_Edge))
				{
					errorData.m_ErrorType = ErrorType.OverlapExisting;
				}
			}
			if ((m_EdgeCollisionMask & collisionMask3) != 0)
			{
				if (CommonUtils.ExclusiveGroundCollision(m_EdgeCollisionMask, collisionMask3))
				{
					if (ValidationHelpers.Intersect(m_Edge, m_OriginalNodes, m_ConnectedNodes, m_OriginalConnectedNodes, edgeData2.m_End, m_EdgeGeometryData, endNodeGeometryData2.m_Geometry, m_EdgeCompositionData, netCompositionData3, ref intersection2))
					{
						if (!IgnoreCollision(edgeEntity2, edgeData2.m_End, m_Edge))
						{
							errorData.m_ErrorType = ErrorType.OverlapExisting;
						}
						val |= MathUtils.Center(((Bounds3)(ref m_EdgeGeometryData.m_Bounds)).y & ((Bounds3)(ref endNodeGeometryData2.m_Geometry.m_Bounds)).y);
					}
				}
				else if (ValidationHelpers.Intersect(m_Edge, m_OriginalNodes, m_ConnectedNodes, m_OriginalConnectedNodes, edgeData2.m_End, m_EdgeGeometryData, endNodeGeometryData2.m_Geometry, m_EdgeCompositionData, netCompositionData3, ref intersection) && !IgnoreCollision(edgeEntity2, edgeData2.m_End, m_Edge))
				{
					errorData.m_ErrorType = ErrorType.OverlapExisting;
				}
			}
			if ((collisionMask & m_StartCollisionMask) != 0)
			{
				if (CommonUtils.ExclusiveGroundCollision(collisionMask, m_StartCollisionMask))
				{
					if (ValidationHelpers.Intersect(edgeData2, val2.AsNativeArray(), m_Edge.m_Start, m_OriginalNodes.m_Start, edgeGeometryData2, m_StartNodeGeometryData.m_Geometry, netCompositionData, m_StartCompositionData, ref intersection2))
					{
						if (!IgnoreCollision(m_EdgeEntity, m_Edge.m_Start, edgeData2))
						{
							errorData.m_ErrorType = ErrorType.OverlapExisting;
						}
						val |= MathUtils.Center(((Bounds3)(ref edgeGeometryData2.m_Bounds)).y & ((Bounds3)(ref m_StartNodeGeometryData.m_Geometry.m_Bounds)).y);
					}
				}
				else if (ValidationHelpers.Intersect(edgeData2, val2.AsNativeArray(), m_Edge.m_Start, m_OriginalNodes.m_Start, edgeGeometryData2, m_StartNodeGeometryData.m_Geometry, netCompositionData, m_StartCompositionData, ref intersection) && !IgnoreCollision(m_EdgeEntity, m_Edge.m_Start, edgeData2))
				{
					errorData.m_ErrorType = ErrorType.OverlapExisting;
				}
			}
			if ((collisionMask & m_EndCollisionMask) != 0)
			{
				if (CommonUtils.ExclusiveGroundCollision(collisionMask, m_EndCollisionMask))
				{
					if (ValidationHelpers.Intersect(edgeData2, val2.AsNativeArray(), m_Edge.m_End, m_OriginalNodes.m_End, edgeGeometryData2, m_EndNodeGeometryData.m_Geometry, netCompositionData, m_EndCompositionData, ref intersection2))
					{
						if (!IgnoreCollision(m_EdgeEntity, m_Edge.m_End, edgeData2))
						{
							errorData.m_ErrorType = ErrorType.OverlapExisting;
						}
						val |= MathUtils.Center(((Bounds3)(ref edgeGeometryData2.m_Bounds)).y & ((Bounds3)(ref m_EndNodeGeometryData.m_Geometry.m_Bounds)).y);
					}
				}
				else if (ValidationHelpers.Intersect(edgeData2, val2.AsNativeArray(), m_Edge.m_End, m_OriginalNodes.m_End, edgeGeometryData2, m_EndNodeGeometryData.m_Geometry, netCompositionData, m_EndCompositionData, ref intersection) && !IgnoreCollision(m_EdgeEntity, m_Edge.m_End, edgeData2))
				{
					errorData.m_ErrorType = ErrorType.OverlapExisting;
				}
			}
			if ((m_StartCollisionMask & collisionMask2) != 0)
			{
				if (CommonUtils.ExclusiveGroundCollision(m_StartCollisionMask, collisionMask2))
				{
					if (ValidationHelpers.Intersect(m_Edge.m_Start, m_OriginalNodes.m_Start, m_ConnectedNodes, m_OriginalConnectedNodes, edgeData2.m_Start, val2.AsNativeArray(), m_StartNodeGeometryData.m_Geometry, startNodeGeometryData2.m_Geometry, m_StartCompositionData, netCompositionData2, ref intersection2))
					{
						if (!IgnoreCollision(m_EdgeEntity, m_Edge.m_Start, edgeEntity2, edgeData2.m_Start))
						{
							errorData.m_ErrorType = ErrorType.OverlapExisting;
						}
						val |= MathUtils.Center(((Bounds3)(ref m_StartNodeGeometryData.m_Geometry.m_Bounds)).y & ((Bounds3)(ref startNodeGeometryData2.m_Geometry.m_Bounds)).y);
					}
				}
				else if (ValidationHelpers.Intersect(m_Edge.m_Start, m_OriginalNodes.m_Start, m_ConnectedNodes, m_OriginalConnectedNodes, edgeData2.m_Start, val2.AsNativeArray(), m_StartNodeGeometryData.m_Geometry, startNodeGeometryData2.m_Geometry, m_StartCompositionData, netCompositionData2, ref intersection) && !IgnoreCollision(m_EdgeEntity, m_Edge.m_Start, edgeEntity2, edgeData2.m_Start))
				{
					errorData.m_ErrorType = ErrorType.OverlapExisting;
				}
			}
			if ((m_StartCollisionMask & collisionMask3) != 0)
			{
				if (CommonUtils.ExclusiveGroundCollision(m_StartCollisionMask, collisionMask3))
				{
					if (ValidationHelpers.Intersect(m_Edge.m_Start, m_OriginalNodes.m_Start, m_ConnectedNodes, m_OriginalConnectedNodes, edgeData2.m_End, val2.AsNativeArray(), m_StartNodeGeometryData.m_Geometry, endNodeGeometryData2.m_Geometry, m_StartCompositionData, netCompositionData3, ref intersection2))
					{
						if (!IgnoreCollision(m_EdgeEntity, m_Edge.m_Start, edgeEntity2, edgeData2.m_End))
						{
							errorData.m_ErrorType = ErrorType.OverlapExisting;
						}
						val |= MathUtils.Center(((Bounds3)(ref m_StartNodeGeometryData.m_Geometry.m_Bounds)).y & ((Bounds3)(ref endNodeGeometryData2.m_Geometry.m_Bounds)).y);
					}
				}
				else if (ValidationHelpers.Intersect(m_Edge.m_Start, m_OriginalNodes.m_Start, m_ConnectedNodes, m_OriginalConnectedNodes, edgeData2.m_End, val2.AsNativeArray(), m_StartNodeGeometryData.m_Geometry, endNodeGeometryData2.m_Geometry, m_StartCompositionData, netCompositionData3, ref intersection) && !IgnoreCollision(m_EdgeEntity, m_Edge.m_Start, edgeEntity2, edgeData2.m_End))
				{
					errorData.m_ErrorType = ErrorType.OverlapExisting;
				}
			}
			if ((m_EndCollisionMask & collisionMask2) != 0)
			{
				if (CommonUtils.ExclusiveGroundCollision(m_EndCollisionMask, collisionMask2))
				{
					if (ValidationHelpers.Intersect(m_Edge.m_End, m_OriginalNodes.m_End, m_ConnectedNodes, m_OriginalConnectedNodes, edgeData2.m_Start, val2.AsNativeArray(), m_EndNodeGeometryData.m_Geometry, startNodeGeometryData2.m_Geometry, m_EndCompositionData, netCompositionData2, ref intersection2))
					{
						if (!IgnoreCollision(m_EdgeEntity, m_Edge.m_End, edgeEntity2, edgeData2.m_Start))
						{
							errorData.m_ErrorType = ErrorType.OverlapExisting;
						}
						val |= MathUtils.Center(((Bounds3)(ref m_EndNodeGeometryData.m_Geometry.m_Bounds)).y & ((Bounds3)(ref startNodeGeometryData2.m_Geometry.m_Bounds)).y);
					}
				}
				else if (ValidationHelpers.Intersect(m_Edge.m_End, m_OriginalNodes.m_End, m_ConnectedNodes, m_OriginalConnectedNodes, edgeData2.m_Start, val2.AsNativeArray(), m_EndNodeGeometryData.m_Geometry, startNodeGeometryData2.m_Geometry, m_EndCompositionData, netCompositionData2, ref intersection) && !IgnoreCollision(m_EdgeEntity, m_Edge.m_End, edgeEntity2, edgeData2.m_Start))
				{
					errorData.m_ErrorType = ErrorType.OverlapExisting;
				}
			}
			if ((m_EndCollisionMask & collisionMask3) != 0)
			{
				if (CommonUtils.ExclusiveGroundCollision(m_EndCollisionMask, collisionMask3))
				{
					if (ValidationHelpers.Intersect(m_Edge.m_End, m_OriginalNodes.m_End, m_ConnectedNodes, m_OriginalConnectedNodes, edgeData2.m_End, val2.AsNativeArray(), m_EndNodeGeometryData.m_Geometry, endNodeGeometryData2.m_Geometry, m_EndCompositionData, netCompositionData3, ref intersection2))
					{
						if (!IgnoreCollision(m_EdgeEntity, m_Edge.m_End, edgeEntity2, edgeData2.m_End))
						{
							errorData.m_ErrorType = ErrorType.OverlapExisting;
						}
						val |= MathUtils.Center(((Bounds3)(ref m_EndNodeGeometryData.m_Geometry.m_Bounds)).y & ((Bounds3)(ref endNodeGeometryData2.m_Geometry.m_Bounds)).y);
					}
				}
				else if (ValidationHelpers.Intersect(m_Edge.m_End, m_OriginalNodes.m_End, m_ConnectedNodes, m_OriginalConnectedNodes, edgeData2.m_End, val2.AsNativeArray(), m_EndNodeGeometryData.m_Geometry, endNodeGeometryData2.m_Geometry, m_EndCompositionData, netCompositionData3, ref intersection) && !IgnoreCollision(m_EdgeEntity, m_Edge.m_End, edgeEntity2, edgeData2.m_End))
				{
					errorData.m_ErrorType = ErrorType.OverlapExisting;
				}
			}
			if (errorData.m_ErrorType == ErrorType.None)
			{
				return;
			}
			((Bounds3)(ref intersection)).xz = ((Bounds3)(ref intersection)).xz | intersection2;
			((Bounds3)(ref intersection)).y = ((Bounds3)(ref intersection)).y | val;
			errorData.m_ErrorSeverity = ErrorSeverity.Error;
			errorData.m_TempEntity = m_EdgeEntity;
			errorData.m_PermanentEntity = edgeEntity2;
			errorData.m_Position = MathUtils.Center(intersection);
			if (!essential && topLevelEntity2 != edgeEntity2 && topLevelEntity2 != Entity.Null)
			{
				PrefabRef prefabRef = m_Data.m_PrefabRef[topLevelEntity2];
				if (m_Data.m_PrefabObjectGeometry.HasComponent(prefabRef.m_Prefab) && (m_Data.m_PrefabObjectGeometry[prefabRef.m_Prefab].m_Flags & (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.DeleteOverridden)) == (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.DeleteOverridden) && !m_Data.m_Attached.HasComponent(topLevelEntity2) && (!m_Data.m_Temp.HasComponent(topLevelEntity2) || (m_Data.m_Temp[topLevelEntity2].m_Flags & TempFlags.Essential) == 0))
				{
					errorData.m_ErrorSeverity = ErrorSeverity.Warning;
					errorData.m_PermanentEntity = topLevelEntity2;
				}
			}
			if (!m_Essential && m_TopLevelEntity != m_EdgeEntity && m_TopLevelEntity != Entity.Null)
			{
				PrefabRef prefabRef2 = m_Data.m_PrefabRef[m_TopLevelEntity];
				if (m_Data.m_PrefabObjectGeometry.HasComponent(prefabRef2.m_Prefab) && (m_Data.m_PrefabObjectGeometry[prefabRef2.m_Prefab].m_Flags & (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.DeleteOverridden)) == (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.DeleteOverridden) && !m_Data.m_Attached.HasComponent(m_TopLevelEntity) && (!m_Data.m_Temp.HasComponent(m_TopLevelEntity) || (m_Data.m_Temp[m_TopLevelEntity].m_Flags & TempFlags.Essential) == 0))
				{
					errorData.m_ErrorSeverity = ErrorSeverity.Warning;
					errorData.m_TempEntity = edgeEntity2;
					errorData.m_PermanentEntity = m_TopLevelEntity;
				}
			}
			m_ErrorQueue.Enqueue(errorData);
		}

		private bool IgnoreCollision(Entity edge1, Entity node1, Edge edge2)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			EdgeIterator edgeIterator = new EdgeIterator(edge1, node1, m_Data.m_ConnectedEdges, m_Data.m_Edge, m_Data.m_Temp, m_Data.m_Hidden, includeMiddleConnections: true);
			EdgeIteratorValue value;
			Temp temp = default(Temp);
			Temp temp2 = default(Temp);
			while (edgeIterator.GetNext(out value))
			{
				if (!value.m_Middle)
				{
					continue;
				}
				Edge edge3 = m_Data.m_Edge[value.m_Edge];
				if (edge3.m_Start == edge2.m_Start || edge3.m_End == edge2.m_Start || edge3.m_Start == edge2.m_End || edge3.m_End == edge2.m_End)
				{
					return true;
				}
				if (m_Data.m_Temp.TryGetComponent(edge3.m_Start, ref temp) && m_Data.m_Temp.TryGetComponent(edge3.m_End, ref temp2))
				{
					if (temp.m_Original == edge2.m_Start || temp2.m_Original == edge2.m_Start || temp.m_Original == edge2.m_End || temp2.m_Original == edge2.m_End)
					{
						return true;
					}
				}
				else if (m_Data.m_Temp.TryGetComponent(edge2.m_Start, ref temp) && m_Data.m_Temp.TryGetComponent(edge2.m_End, ref temp2) && (temp.m_Original == edge3.m_Start || temp2.m_Original == edge3.m_Start || temp.m_Original == edge3.m_End || temp2.m_Original == edge3.m_End))
				{
					return true;
				}
			}
			return false;
		}

		private bool IgnoreCollision(Entity edge1, Entity node1, Entity edge2, Entity node2)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			EdgeIterator edgeIterator = new EdgeIterator(edge1, node1, m_Data.m_ConnectedEdges, m_Data.m_Edge, m_Data.m_Temp, m_Data.m_Hidden, includeMiddleConnections: true);
			EdgeIteratorValue value;
			Temp temp = default(Temp);
			while (edgeIterator.GetNext(out value))
			{
				if (!value.m_Middle)
				{
					continue;
				}
				Edge edge3 = m_Data.m_Edge[value.m_Edge];
				if (edge3.m_Start == node2 || edge3.m_End == node2)
				{
					return true;
				}
				if (m_Data.m_Temp.TryGetComponent(node2, ref temp))
				{
					if (edge3.m_Start == temp.m_Original || edge3.m_End == temp.m_Original)
					{
						return true;
					}
					continue;
				}
				if (m_Data.m_Temp.TryGetComponent(edge3.m_Start, ref temp) && temp.m_Original == node2)
				{
					return true;
				}
				if (m_Data.m_Temp.TryGetComponent(edge3.m_End, ref temp) && temp.m_Original == node2)
				{
					return true;
				}
			}
			edgeIterator = new EdgeIterator(edge2, node2, m_Data.m_ConnectedEdges, m_Data.m_Edge, m_Data.m_Temp, m_Data.m_Hidden, includeMiddleConnections: true);
			EdgeIteratorValue value2;
			Temp temp2 = default(Temp);
			while (edgeIterator.GetNext(out value2))
			{
				if (!value2.m_Middle)
				{
					continue;
				}
				Edge edge4 = m_Data.m_Edge[value2.m_Edge];
				if (edge4.m_Start == node1 || edge4.m_End == node1)
				{
					return true;
				}
				if (m_Data.m_Temp.TryGetComponent(node1, ref temp2))
				{
					if (edge4.m_Start == temp2.m_Original || edge4.m_End == temp2.m_Original)
					{
						return true;
					}
					continue;
				}
				if (m_Data.m_Temp.TryGetComponent(edge4.m_Start, ref temp2) && temp2.m_Original == node1)
				{
					return true;
				}
				if (m_Data.m_Temp.TryGetComponent(edge4.m_End, ref temp2) && temp2.m_Original == node1)
				{
					return true;
				}
			}
			return false;
		}
	}

	private struct ObjectIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
	{
		public Entity m_EdgeEntity;

		public Entity m_TopLevelEntity;

		public Entity m_AssetStampEntity;

		public Bounds3 m_Bounds;

		public Edge m_OriginalNodes;

		public Edge m_NodeOwners;

		public Edge m_NodeAssetStamps;

		public Edge m_NodeEdges;

		public Edge m_OwnerEdge;

		public EdgeGeometry m_EdgeGeometryData;

		public StartNodeGeometry m_StartNodeGeometryData;

		public EndNodeGeometry m_EndNodeGeometryData;

		public NetCompositionData m_EdgeCompositionData;

		public NetCompositionData m_StartCompositionData;

		public NetCompositionData m_EndCompositionData;

		public CollisionMask m_EdgeCollisionMask;

		public CollisionMask m_StartCollisionMask;

		public CollisionMask m_EndCollisionMask;

		public CollisionMask m_CombinedCollisionMask;

		public DynamicBuffer<NetCompositionArea> m_EdgeCompositionAreas;

		public DynamicBuffer<NetCompositionArea> m_StartCompositionAreas;

		public DynamicBuffer<NetCompositionArea> m_EndCompositionAreas;

		public ValidationSystem.EntityData m_Data;

		public ParallelWriter<ErrorData> m_ErrorQueue;

		public bool m_EditorMode;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if ((bounds.m_Mask & BoundsMask.NotOverridden) == 0)
			{
				return false;
			}
			if ((m_CombinedCollisionMask & CollisionMask.OnGround) != 0)
			{
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_Bounds)).xz);
			}
			return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, Entity objectEntity2)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			if ((bounds.m_Mask & BoundsMask.NotOverridden) == 0)
			{
				return;
			}
			if ((m_CombinedCollisionMask & CollisionMask.OnGround) != 0)
			{
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_Bounds)).xz))
				{
					return;
				}
			}
			else if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds))
			{
				return;
			}
			if (m_Data.m_Hidden.HasComponent(objectEntity2) || m_AssetStampEntity == objectEntity2 || m_NodeAssetStamps.m_Start == objectEntity2 || m_NodeAssetStamps.m_End == objectEntity2)
			{
				return;
			}
			bool flag = true;
			bool flag2 = true;
			Entity val = objectEntity2;
			bool hasOwner = false;
			Owner owner = default(Owner);
			Edge edge = default(Edge);
			while (m_Data.m_Owner.TryGetComponent(val, ref owner) && !m_Data.m_Building.HasComponent(val))
			{
				Entity owner2 = owner.m_Owner;
				hasOwner = true;
				if (m_Data.m_AssetStamp.HasComponent(owner2))
				{
					break;
				}
				val = owner2;
				if (m_Data.m_Edge.TryGetComponent(val, ref edge))
				{
					flag &= edge.m_Start != m_OriginalNodes.m_Start && edge.m_End != m_OriginalNodes.m_Start;
					flag2 &= edge.m_Start != m_OriginalNodes.m_End && edge.m_End != m_OriginalNodes.m_End;
					if (m_NodeEdges.m_Start == val || m_NodeEdges.m_End == val)
					{
						return;
					}
				}
				else if (m_Data.m_Node.HasComponent(val))
				{
					if (m_NodeEdges.m_Start != Entity.Null)
					{
						Edge edge2 = m_Data.m_Edge[m_NodeEdges.m_Start];
						if (edge2.m_Start == val || edge2.m_End == val)
						{
							return;
						}
					}
					if (m_NodeEdges.m_End != Entity.Null)
					{
						Edge edge3 = m_Data.m_Edge[m_NodeEdges.m_End];
						if (edge3.m_Start == val || edge3.m_End == val)
						{
							return;
						}
					}
				}
				if (owner2 == m_OwnerEdge.m_Start || owner2 == m_OwnerEdge.m_End)
				{
					return;
				}
			}
			if (m_TopLevelEntity == val)
			{
				return;
			}
			flag &= m_NodeOwners.m_Start != val;
			flag2 &= m_NodeOwners.m_End != val;
			Attached attached = default(Attached);
			if (m_Data.m_Attached.TryGetComponent(objectEntity2, ref attached))
			{
				if (attached.m_Parent == m_OriginalNodes.m_Start)
				{
					flag &= (m_StartCompositionData.m_Flags.m_General & CompositionFlags.General.Roundabout) == 0;
				}
				if (attached.m_Parent == m_OriginalNodes.m_End)
				{
					flag2 &= (m_EndCompositionData.m_Flags.m_General & CompositionFlags.General.Roundabout) == 0;
				}
			}
			PrefabRef prefabRef = m_Data.m_PrefabRef[objectEntity2];
			Transform transform = m_Data.m_Transform[objectEntity2];
			CheckOverlap(objectEntity2, val, bounds.m_Bounds, prefabRef, transform, flag, flag2, hasOwner);
		}

		public void CheckOverlap(Entity objectEntity2, Entity topLevelEntity2, Bounds3 bounds2, PrefabRef prefabRef2, Transform transform2, bool checkStart, bool checkEnd, bool hasOwner)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			if (!m_Data.m_PrefabObjectGeometry.HasComponent(prefabRef2.m_Prefab))
			{
				return;
			}
			ObjectGeometryData objectGeometryData = m_Data.m_PrefabObjectGeometry[prefabRef2.m_Prefab];
			if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.IgnoreSecondaryCollision) != Game.Objects.GeometryFlags.None && m_Data.m_Secondary.HasComponent(objectEntity2))
			{
				return;
			}
			CollisionMask collisionMask = ((!m_Data.m_ObjectElevation.HasComponent(objectEntity2)) ? ObjectUtils.GetCollisionMask(objectGeometryData, !m_EditorMode || hasOwner) : ObjectUtils.GetCollisionMask(objectGeometryData, m_Data.m_ObjectElevation[objectEntity2], !m_EditorMode || hasOwner));
			if ((m_CombinedCollisionMask & collisionMask) == 0)
			{
				return;
			}
			ErrorData error = new ErrorData
			{
				m_ErrorSeverity = ErrorSeverity.Error,
				m_TempEntity = m_EdgeEntity,
				m_PermanentEntity = objectEntity2
			};
			if (topLevelEntity2 != objectEntity2)
			{
				if ((objectGeometryData.m_Flags & (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.DeleteOverridden)) == Game.Objects.GeometryFlags.Overridable)
				{
					error.m_ErrorSeverity = ErrorSeverity.Override;
				}
				else
				{
					PrefabRef prefabRef3 = m_Data.m_PrefabRef[topLevelEntity2];
					if (m_Data.m_PrefabObjectGeometry.HasComponent(prefabRef3.m_Prefab) && (m_Data.m_PrefabObjectGeometry[prefabRef3.m_Prefab].m_Flags & (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.DeleteOverridden)) == (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.DeleteOverridden) && !m_Data.m_Attached.HasComponent(topLevelEntity2))
					{
						error.m_ErrorSeverity = ErrorSeverity.Warning;
						error.m_PermanentEntity = topLevelEntity2;
					}
				}
			}
			else if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Overridable) != Game.Objects.GeometryFlags.None)
			{
				if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.DeleteOverridden) != Game.Objects.GeometryFlags.None)
				{
					if (!m_Data.m_Attached.HasComponent(objectEntity2))
					{
						error.m_ErrorSeverity = ErrorSeverity.Warning;
					}
				}
				else
				{
					error.m_ErrorSeverity = ErrorSeverity.Override;
				}
			}
			float3 origin = MathUtils.Center(bounds2);
			StackData stackData = default(StackData);
			Stack stack = default(Stack);
			if (m_Data.m_Stack.TryGetComponent(objectEntity2, ref stack))
			{
				m_Data.m_PrefabStackData.TryGetComponent(prefabRef2.m_Prefab, ref stackData);
			}
			if ((m_CombinedCollisionMask & CollisionMask.OnGround) == 0 || MathUtils.Intersect(bounds2, m_Bounds))
			{
				CheckOverlap3D(ref error, collisionMask, objectGeometryData, stackData, bounds2, transform2, stack, topLevelEntity2, origin, checkStart, checkEnd);
			}
			if (error.m_ErrorType == ErrorType.None && CommonUtils.ExclusiveGroundCollision(m_CombinedCollisionMask, collisionMask))
			{
				CheckOverlap2D(ref error, collisionMask, objectGeometryData, bounds2, transform2, topLevelEntity2, origin, checkStart, checkEnd);
			}
			if (error.m_ErrorType != ErrorType.None)
			{
				if ((error.m_ErrorSeverity == ErrorSeverity.Override || error.m_ErrorSeverity == ErrorSeverity.Warning) && error.m_ErrorType == ErrorType.OverlapExisting && m_Data.m_OnFire.HasComponent(error.m_PermanentEntity))
				{
					error.m_ErrorType = ErrorType.OnFire;
					error.m_ErrorSeverity = ErrorSeverity.Error;
				}
				m_ErrorQueue.Enqueue(error);
			}
		}

		private void CheckOverlap3D(ref ErrorData error, CollisionMask collisionMask2, ObjectGeometryData prefabObjectGeometryData2, StackData stackData2, Bounds3 bounds2, Transform transform2, Stack stack2, Entity topLevelEntity2, float3 origin, bool checkStart, bool checkEnd)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0527: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_055f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_0595: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_05db: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0619: Unknown result type (might be due to invalid IL or missing references)
			//IL_061b: Unknown result type (might be due to invalid IL or missing references)
			//IL_061c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_0626: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			Bounds3 intersection = default(Bounds3);
			intersection.min = float3.op_Implicit(float.MaxValue);
			intersection.max = float3.op_Implicit(float.MinValue);
			float3 val = math.mul(math.inverse(transform2.m_Rotation), transform2.m_Position - origin);
			Bounds3 bounds3 = ObjectUtils.GetBounds(stack2, prefabObjectGeometryData2, stackData2);
			DynamicBuffer<NetCompositionArea> areas = m_EdgeCompositionAreas;
			DynamicBuffer<NetCompositionArea> areas2 = m_StartCompositionAreas;
			DynamicBuffer<NetCompositionArea> areas3 = m_EndCompositionAreas;
			if ((prefabObjectGeometryData2.m_Flags & (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.DeleteOverridden)) == Game.Objects.GeometryFlags.Overridable)
			{
				areas = default(DynamicBuffer<NetCompositionArea>);
				areas2 = default(DynamicBuffer<NetCompositionArea>);
				areas3 = default(DynamicBuffer<NetCompositionArea>);
			}
			if ((prefabObjectGeometryData2.m_Flags & Game.Objects.GeometryFlags.IgnoreBottomCollision) != Game.Objects.GeometryFlags.None)
			{
				bounds3.min.y = math.max(bounds3.min.y, 0f);
			}
			if (ObjectUtils.GetStandingLegCount(prefabObjectGeometryData2, out var legCount))
			{
				Box3 box = default(Box3);
				for (int i = 0; i < legCount; i++)
				{
					float3 val2 = val + ObjectUtils.GetStandingLegOffset(prefabObjectGeometryData2, i);
					if ((prefabObjectGeometryData2.m_Flags & (Game.Objects.GeometryFlags.CircularLeg | Game.Objects.GeometryFlags.IgnoreLegCollision)) == Game.Objects.GeometryFlags.CircularLeg)
					{
						Cylinder3 cylinder = new Cylinder3
						{
							circle = new Circle2(prefabObjectGeometryData2.m_LegSize.x * 0.5f, ((float3)(ref val2)).xz),
							height = new Bounds1(bounds3.min.y, prefabObjectGeometryData2.m_LegSize.y) + val2.y,
							rotation = transform2.m_Rotation
						};
						if ((m_EdgeCollisionMask & collisionMask2) != 0 && ValidationHelpers.Intersect(m_NodeOwners, topLevelEntity2, m_EdgeGeometryData, -origin, cylinder, bounds2, m_EdgeCompositionData, areas, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
						}
						if ((m_StartCollisionMask & collisionMask2) != 0 && checkStart && ValidationHelpers.Intersect(m_NodeOwners.m_Start, topLevelEntity2, m_StartNodeGeometryData.m_Geometry, -origin, cylinder, bounds2, m_StartCompositionData, areas2, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
						}
						if ((m_EndCollisionMask & collisionMask2) != 0 && checkEnd && ValidationHelpers.Intersect(m_NodeOwners.m_End, topLevelEntity2, m_EndNodeGeometryData.m_Geometry, -origin, cylinder, bounds2, m_EndCompositionData, areas3, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
						}
					}
					else if ((prefabObjectGeometryData2.m_Flags & Game.Objects.GeometryFlags.IgnoreLegCollision) == 0)
					{
						Bounds3 val3 = new Bounds3
						{
							min = 
							{
								y = bounds3.min.y
							}
						};
						((float3)(ref val3.min)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * -0.5f;
						val3.max.y = prefabObjectGeometryData2.m_LegSize.y;
						((float3)(ref val3.max)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * 0.5f;
						((Box3)(ref box))._002Ector(val3 + val2, transform2.m_Rotation);
						if ((m_EdgeCollisionMask & collisionMask2) != 0 && ValidationHelpers.Intersect(m_NodeOwners, topLevelEntity2, m_EdgeGeometryData, -origin, box, bounds2, m_EdgeCompositionData, areas, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
						}
						if ((m_StartCollisionMask & collisionMask2) != 0 && checkStart && ValidationHelpers.Intersect(m_NodeOwners.m_Start, topLevelEntity2, m_StartNodeGeometryData.m_Geometry, -origin, box, bounds2, m_StartCompositionData, areas2, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
						}
						if ((m_EndCollisionMask & collisionMask2) != 0 && checkEnd && ValidationHelpers.Intersect(m_NodeOwners.m_End, topLevelEntity2, m_EndNodeGeometryData.m_Geometry, -origin, box, bounds2, m_EndCompositionData, areas3, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
						}
					}
				}
				bounds3.min.y = prefabObjectGeometryData2.m_LegSize.y;
			}
			if ((prefabObjectGeometryData2.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
			{
				Cylinder3 cylinder2 = new Cylinder3
				{
					circle = new Circle2(prefabObjectGeometryData2.m_Size.x * 0.5f, ((float3)(ref val)).xz),
					height = new Bounds1(bounds3.min.y, bounds3.max.y) + val.y,
					rotation = transform2.m_Rotation
				};
				if ((m_EdgeCollisionMask & collisionMask2) != 0 && ValidationHelpers.Intersect(m_NodeOwners, topLevelEntity2, m_EdgeGeometryData, -origin, cylinder2, bounds2, m_EdgeCompositionData, areas, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
				}
				if ((m_StartCollisionMask & collisionMask2) != 0 && checkStart && ValidationHelpers.Intersect(m_NodeOwners.m_Start, topLevelEntity2, m_StartNodeGeometryData.m_Geometry, -origin, cylinder2, bounds2, m_StartCompositionData, areas2, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
				}
				if ((m_EndCollisionMask & collisionMask2) != 0 && checkEnd && ValidationHelpers.Intersect(m_NodeOwners.m_End, topLevelEntity2, m_EndNodeGeometryData.m_Geometry, -origin, cylinder2, bounds2, m_EndCompositionData, areas3, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
				}
			}
			else
			{
				Box3 box2 = new Box3
				{
					bounds = bounds3 + val,
					rotation = transform2.m_Rotation
				};
				if ((m_EdgeCollisionMask & collisionMask2) != 0 && ValidationHelpers.Intersect(m_NodeOwners, topLevelEntity2, m_EdgeGeometryData, -origin, box2, bounds2, m_EdgeCompositionData, areas, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
				}
				if ((m_StartCollisionMask & collisionMask2) != 0 && checkStart && ValidationHelpers.Intersect(m_NodeOwners.m_Start, topLevelEntity2, m_StartNodeGeometryData.m_Geometry, -origin, box2, bounds2, m_StartCompositionData, areas2, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
				}
				if ((m_EndCollisionMask & collisionMask2) != 0 && checkEnd && ValidationHelpers.Intersect(m_NodeOwners.m_End, topLevelEntity2, m_EndNodeGeometryData.m_Geometry, -origin, box2, bounds2, m_EndCompositionData, areas3, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
				}
			}
			if (error.m_ErrorType != ErrorType.None)
			{
				error.m_Position = origin + MathUtils.Center(intersection);
			}
		}

		private void CheckOverlap2D(ref ErrorData error, CollisionMask collisionMask2, ObjectGeometryData prefabObjectGeometryData2, Bounds3 bounds2, Transform transformData2, Entity topLevelEntity2, float3 origin, bool checkStart, bool checkEnd)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_0617: Unknown result type (might be due to invalid IL or missing references)
			//IL_061e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0624: Unknown result type (might be due to invalid IL or missing references)
			//IL_0629: Unknown result type (might be due to invalid IL or missing references)
			//IL_062e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0632: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0657: Unknown result type (might be due to invalid IL or missing references)
			//IL_065c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_0665: Unknown result type (might be due to invalid IL or missing references)
			//IL_0670: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06de: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0681: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0694: Unknown result type (might be due to invalid IL or missing references)
			//IL_0699: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			//IL_0537: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0742: Unknown result type (might be due to invalid IL or missing references)
			//IL_0747: Unknown result type (might be due to invalid IL or missing references)
			//IL_0756: Unknown result type (might be due to invalid IL or missing references)
			//IL_075b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0760: Unknown result type (might be due to invalid IL or missing references)
			//IL_0764: Unknown result type (might be due to invalid IL or missing references)
			//IL_076f: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_070f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0716: Unknown result type (might be due to invalid IL or missing references)
			//IL_071b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0725: Unknown result type (might be due to invalid IL or missing references)
			//IL_072a: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_059f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0570: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_057f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_07be: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0781: Unknown result type (might be due to invalid IL or missing references)
			//IL_0792: Unknown result type (might be due to invalid IL or missing references)
			//IL_0799: Unknown result type (might be due to invalid IL or missing references)
			//IL_079e: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_0608: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			Bounds2 intersection = default(Bounds2);
			intersection.min = float2.op_Implicit(float.MaxValue);
			intersection.max = float2.op_Implicit(float.MinValue);
			Bounds1 val = default(Bounds1);
			val.min = float.MaxValue;
			val.max = float.MinValue;
			DynamicBuffer<NetCompositionArea> areas = m_EdgeCompositionAreas;
			DynamicBuffer<NetCompositionArea> areas2 = m_StartCompositionAreas;
			DynamicBuffer<NetCompositionArea> areas3 = m_EndCompositionAreas;
			if ((prefabObjectGeometryData2.m_Flags & (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.DeleteOverridden)) == Game.Objects.GeometryFlags.Overridable)
			{
				areas = default(DynamicBuffer<NetCompositionArea>);
				areas2 = default(DynamicBuffer<NetCompositionArea>);
				areas3 = default(DynamicBuffer<NetCompositionArea>);
			}
			Quad3 val2;
			if (ObjectUtils.GetStandingLegCount(prefabObjectGeometryData2, out var legCount))
			{
				Circle2 circle = default(Circle2);
				for (int i = 0; i < legCount; i++)
				{
					float3 position = ObjectUtils.GetStandingLegPosition(prefabObjectGeometryData2, transformData2, i) - origin;
					if ((prefabObjectGeometryData2.m_Flags & (Game.Objects.GeometryFlags.CircularLeg | Game.Objects.GeometryFlags.IgnoreLegCollision)) == Game.Objects.GeometryFlags.CircularLeg)
					{
						((Circle2)(ref circle))._002Ector(prefabObjectGeometryData2.m_LegSize.x * 0.5f, ((float3)(ref position)).xz);
						if (CommonUtils.ExclusiveGroundCollision(m_EdgeCollisionMask, collisionMask2) && ValidationHelpers.Intersect(m_NodeOwners, topLevelEntity2, m_EdgeGeometryData, -((float3)(ref origin)).xz, circle, ((Bounds3)(ref bounds2)).xz, m_EdgeCompositionData, areas, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
							val |= MathUtils.Center(((Bounds3)(ref m_EdgeGeometryData.m_Bounds)).y & ((Bounds3)(ref bounds2)).y);
						}
						if (CommonUtils.ExclusiveGroundCollision(m_StartCollisionMask, collisionMask2) && checkStart && ValidationHelpers.Intersect(m_NodeOwners.m_Start, topLevelEntity2, m_StartNodeGeometryData.m_Geometry, -((float3)(ref origin)).xz, circle, ((Bounds3)(ref bounds2)).xz, m_StartCompositionData, areas2, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
							val |= MathUtils.Center(((Bounds3)(ref m_StartNodeGeometryData.m_Geometry.m_Bounds)).y & ((Bounds3)(ref bounds2)).y);
						}
						if (CommonUtils.ExclusiveGroundCollision(m_EndCollisionMask, collisionMask2) && checkEnd && ValidationHelpers.Intersect(m_NodeOwners.m_End, topLevelEntity2, m_EndNodeGeometryData.m_Geometry, -((float3)(ref origin)).xz, circle, ((Bounds3)(ref bounds2)).xz, m_EndCompositionData, areas3, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
							val |= MathUtils.Center(((Bounds3)(ref m_EndNodeGeometryData.m_Geometry.m_Bounds)).y & ((Bounds3)(ref bounds2)).y);
						}
					}
					else if ((prefabObjectGeometryData2.m_Flags & Game.Objects.GeometryFlags.IgnoreLegCollision) == 0)
					{
						Bounds3 bounds3 = default(Bounds3);
						((float3)(ref bounds3.min)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * -0.5f;
						((float3)(ref bounds3.max)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * 0.5f;
						val2 = ObjectUtils.CalculateBaseCorners(position, transformData2.m_Rotation, bounds3);
						Quad2 xz = ((Quad3)(ref val2)).xz;
						if (CommonUtils.ExclusiveGroundCollision(m_EdgeCollisionMask, collisionMask2) && ValidationHelpers.Intersect(m_NodeOwners, topLevelEntity2, m_EdgeGeometryData, -((float3)(ref origin)).xz, xz, ((Bounds3)(ref bounds2)).xz, m_EdgeCompositionData, areas, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
							val |= MathUtils.Center(((Bounds3)(ref m_EdgeGeometryData.m_Bounds)).y & ((Bounds3)(ref bounds2)).y);
						}
						if (CommonUtils.ExclusiveGroundCollision(m_StartCollisionMask, collisionMask2) && checkStart && ValidationHelpers.Intersect(m_NodeOwners.m_Start, topLevelEntity2, m_StartNodeGeometryData.m_Geometry, -((float3)(ref origin)).xz, xz, ((Bounds3)(ref bounds2)).xz, m_StartCompositionData, areas2, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
							val |= MathUtils.Center(((Bounds3)(ref m_StartNodeGeometryData.m_Geometry.m_Bounds)).y & ((Bounds3)(ref bounds2)).y);
						}
						if (CommonUtils.ExclusiveGroundCollision(m_EndCollisionMask, collisionMask2) && checkEnd && ValidationHelpers.Intersect(m_NodeOwners.m_End, topLevelEntity2, m_EndNodeGeometryData.m_Geometry, -((float3)(ref origin)).xz, xz, ((Bounds3)(ref bounds2)).xz, m_EndCompositionData, areas3, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
							val |= MathUtils.Center(((Bounds3)(ref m_EndNodeGeometryData.m_Geometry.m_Bounds)).y & ((Bounds3)(ref bounds2)).y);
						}
					}
				}
			}
			else if ((prefabObjectGeometryData2.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
			{
				float num = prefabObjectGeometryData2.m_Size.x * 0.5f;
				float3 val3 = transformData2.m_Position - origin;
				Circle2 circle2 = default(Circle2);
				((Circle2)(ref circle2))._002Ector(num, ((float3)(ref val3)).xz);
				if (CommonUtils.ExclusiveGroundCollision(m_EdgeCollisionMask, collisionMask2) && ValidationHelpers.Intersect(m_NodeOwners, topLevelEntity2, m_EdgeGeometryData, -((float3)(ref origin)).xz, circle2, ((Bounds3)(ref bounds2)).xz, m_EdgeCompositionData, areas, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
					val |= MathUtils.Center(((Bounds3)(ref m_EdgeGeometryData.m_Bounds)).y & ((Bounds3)(ref bounds2)).y);
				}
				if (CommonUtils.ExclusiveGroundCollision(m_StartCollisionMask, collisionMask2) && checkStart && ValidationHelpers.Intersect(m_NodeOwners.m_Start, topLevelEntity2, m_StartNodeGeometryData.m_Geometry, -((float3)(ref origin)).xz, circle2, ((Bounds3)(ref bounds2)).xz, m_StartCompositionData, areas2, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
					val |= MathUtils.Center(((Bounds3)(ref m_StartNodeGeometryData.m_Geometry.m_Bounds)).y & ((Bounds3)(ref bounds2)).y);
				}
				if (CommonUtils.ExclusiveGroundCollision(m_EndCollisionMask, collisionMask2) && checkEnd && ValidationHelpers.Intersect(m_NodeOwners.m_End, topLevelEntity2, m_EndNodeGeometryData.m_Geometry, -((float3)(ref origin)).xz, circle2, ((Bounds3)(ref bounds2)).xz, m_EndCompositionData, areas3, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
					val |= MathUtils.Center(((Bounds3)(ref m_EndNodeGeometryData.m_Geometry.m_Bounds)).y & ((Bounds3)(ref bounds2)).y);
				}
			}
			else
			{
				val2 = ObjectUtils.CalculateBaseCorners(transformData2.m_Position - origin, transformData2.m_Rotation, prefabObjectGeometryData2.m_Bounds);
				Quad2 xz2 = ((Quad3)(ref val2)).xz;
				if (CommonUtils.ExclusiveGroundCollision(m_EdgeCollisionMask, collisionMask2) && ValidationHelpers.Intersect(m_NodeOwners, topLevelEntity2, m_EdgeGeometryData, -((float3)(ref origin)).xz, xz2, ((Bounds3)(ref bounds2)).xz, m_EdgeCompositionData, areas, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
					val |= MathUtils.Center(((Bounds3)(ref m_EdgeGeometryData.m_Bounds)).y & ((Bounds3)(ref bounds2)).y);
				}
				if (CommonUtils.ExclusiveGroundCollision(m_StartCollisionMask, collisionMask2) && checkStart && ValidationHelpers.Intersect(m_NodeOwners.m_Start, topLevelEntity2, m_StartNodeGeometryData.m_Geometry, -((float3)(ref origin)).xz, xz2, ((Bounds3)(ref bounds2)).xz, m_StartCompositionData, areas2, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
					val |= MathUtils.Center(((Bounds3)(ref m_StartNodeGeometryData.m_Geometry.m_Bounds)).y & ((Bounds3)(ref bounds2)).y);
				}
				if (CommonUtils.ExclusiveGroundCollision(m_EndCollisionMask, collisionMask2) && checkEnd && ValidationHelpers.Intersect(m_NodeOwners.m_End, topLevelEntity2, m_EndNodeGeometryData.m_Geometry, -((float3)(ref origin)).xz, xz2, ((Bounds3)(ref bounds2)).xz, m_EndCompositionData, areas3, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
					val |= MathUtils.Center(((Bounds3)(ref m_EndNodeGeometryData.m_Geometry.m_Bounds)).y & ((Bounds3)(ref bounds2)).y);
				}
			}
			if (error.m_ErrorType != ErrorType.None)
			{
				((float3)(ref error.m_Position)).xz = ((float3)(ref origin)).xz + MathUtils.Center(intersection);
				error.m_Position.y = MathUtils.Center(val);
			}
		}
	}

	private struct AreaIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
	{
		public Entity m_EdgeEntity;

		public Bounds3 m_Bounds;

		public Edge m_NodeOwners;

		public bool m_IgnoreCollisions;

		public bool m_IgnoreProtectedAreas;

		public bool m_EditorMode;

		public EdgeGeometry m_EdgeGeometryData;

		public StartNodeGeometry m_StartNodeGeometryData;

		public EndNodeGeometry m_EndNodeGeometryData;

		public NetCompositionData m_EdgeCompositionData;

		public NetCompositionData m_StartCompositionData;

		public NetCompositionData m_EndCompositionData;

		public CollisionMask m_EdgeCollisionMask;

		public CollisionMask m_StartCollisionMask;

		public CollisionMask m_EndCollisionMask;

		public CollisionMask m_CombinedCollisionMask;

		public ValidationSystem.EntityData m_Data;

		public ParallelWriter<ErrorData> m_ErrorQueue;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_Bounds)).xz);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem areaItem2)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_Bounds)).xz) || m_Data.m_Hidden.HasComponent(areaItem2.m_Area) || (m_Data.m_Area[areaItem2.m_Area].m_Flags & AreaFlags.Slave) != 0)
			{
				return;
			}
			PrefabRef prefabRef = m_Data.m_PrefabRef[areaItem2.m_Area];
			AreaGeometryData areaGeometryData = m_Data.m_PrefabAreaGeometry[prefabRef.m_Prefab];
			AreaUtils.SetCollisionFlags(ref areaGeometryData, !m_EditorMode || m_Data.m_Owner.HasComponent(areaItem2.m_Area));
			if ((areaGeometryData.m_Flags & (Game.Areas.GeometryFlags.PhysicalGeometry | Game.Areas.GeometryFlags.ProtectedArea)) == 0)
			{
				return;
			}
			if ((areaGeometryData.m_Flags & Game.Areas.GeometryFlags.ProtectedArea) != 0)
			{
				if (!m_Data.m_Native.HasComponent(areaItem2.m_Area) || m_IgnoreProtectedAreas)
				{
					return;
				}
			}
			else if (m_IgnoreCollisions)
			{
				return;
			}
			CollisionMask collisionMask = AreaUtils.GetCollisionMask(areaGeometryData);
			if ((m_CombinedCollisionMask & collisionMask) == 0)
			{
				return;
			}
			ErrorType errorType = ErrorType.OverlapExisting;
			if (areaGeometryData.m_Type == AreaType.MapTile)
			{
				errorType = ErrorType.ExceedsCityLimits;
				if ((m_EdgeCompositionData.m_State & CompositionState.Airspace) != 0)
				{
					return;
				}
			}
			DynamicBuffer<Game.Areas.Node> nodes = m_Data.m_AreaNodes[areaItem2.m_Area];
			Triangle triangle = m_Data.m_AreaTriangles[areaItem2.m_Area][areaItem2.m_Triangle];
			Triangle3 triangle2 = AreaUtils.GetTriangle3(nodes, triangle);
			ErrorData errorData = default(ErrorData);
			Bounds3 intersection = default(Bounds3);
			intersection.min = float3.op_Implicit(float.MaxValue);
			intersection.max = float3.op_Implicit(float.MinValue);
			if (areaGeometryData.m_Type != AreaType.MapTile && ((m_CombinedCollisionMask & CollisionMask.OnGround) == 0 || MathUtils.Intersect(bounds.m_Bounds, m_Bounds)))
			{
				Bounds1 heightRange = triangle.m_HeightRange;
				heightRange.max += areaGeometryData.m_MaxHeight;
				if ((m_EdgeCollisionMask & collisionMask) != 0 && ValidationHelpers.Intersect(m_NodeOwners, areaItem2.m_Area, m_EdgeGeometryData, triangle2, m_EdgeCompositionData, heightRange, ref intersection))
				{
					errorData.m_ErrorType = errorType;
				}
				if ((m_StartCollisionMask & collisionMask) != 0 && ValidationHelpers.Intersect(m_NodeOwners.m_Start, areaItem2.m_Area, m_StartNodeGeometryData.m_Geometry, triangle2, m_StartCompositionData, heightRange, ref intersection))
				{
					errorData.m_ErrorType = errorType;
				}
				if ((m_EndCollisionMask & collisionMask) != 0 && ValidationHelpers.Intersect(m_NodeOwners.m_End, areaItem2.m_Area, m_EndNodeGeometryData.m_Geometry, triangle2, m_EndCompositionData, heightRange, ref intersection))
				{
					errorData.m_ErrorType = errorType;
				}
			}
			if (areaGeometryData.m_Type == AreaType.MapTile || (errorData.m_ErrorType == ErrorType.None && CommonUtils.ExclusiveGroundCollision(m_CombinedCollisionMask, collisionMask)))
			{
				if ((m_EdgeCollisionMask & collisionMask) != 0 && ValidationHelpers.Intersect(m_NodeOwners, areaItem2.m_Area, m_EdgeGeometryData, ((Triangle3)(ref triangle2)).xz, bounds.m_Bounds, m_EdgeCompositionData, ref intersection))
				{
					errorData.m_ErrorType = errorType;
				}
				if ((m_StartCollisionMask & collisionMask) != 0 && ValidationHelpers.Intersect(m_NodeOwners.m_Start, areaItem2.m_Area, m_StartNodeGeometryData.m_Geometry, ((Triangle3)(ref triangle2)).xz, bounds.m_Bounds, m_StartCompositionData, ref intersection))
				{
					errorData.m_ErrorType = errorType;
				}
				if ((m_EndCollisionMask & collisionMask) != 0 && ValidationHelpers.Intersect(m_NodeOwners.m_End, areaItem2.m_Area, m_EndNodeGeometryData.m_Geometry, ((Triangle3)(ref triangle2)).xz, bounds.m_Bounds, m_EndCompositionData, ref intersection))
				{
					errorData.m_ErrorType = errorType;
				}
			}
			if (errorData.m_ErrorType != ErrorType.None)
			{
				errorData.m_ErrorSeverity = ErrorSeverity.Error;
				errorData.m_TempEntity = m_EdgeEntity;
				errorData.m_PermanentEntity = areaItem2.m_Area;
				errorData.m_Position = MathUtils.Center(intersection);
				errorData.m_Position.y = MathUtils.Clamp(errorData.m_Position.y, ((Bounds3)(ref m_Bounds)).y);
				m_ErrorQueue.Enqueue(errorData);
			}
		}
	}

	public static void ValidateEdge(Entity entity, Temp temp, Owner owner, Fixed _fixed, Edge edge, EdgeGeometry edgeGeometry, StartNodeGeometry startNodeGeometry, EndNodeGeometry endNodeGeometry, Composition composition, PrefabRef prefabRef, bool editorMode, ValidationSystem.EntityData data, NativeList<ValidationSystem.BoundsData> edgeList, NativeQuadTree<Entity, QuadTreeBoundsXZ> objectSearchTree, NativeQuadTree<Entity, QuadTreeBoundsXZ> netSearchTree, NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> areaSearchTree, WaterSurfaceData waterSurfaceData, TerrainHeightData terrainHeightData, ParallelWriter<ErrorData> errorQueue, NativeList<ConnectedNode> tempNodes)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_12fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1308: Unknown result type (might be due to invalid IL or missing references)
		//IL_1312: Unknown result type (might be due to invalid IL or missing references)
		//IL_1328: Unknown result type (might be due to invalid IL or missing references)
		//IL_1332: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_135e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1368: Unknown result type (might be due to invalid IL or missing references)
		//IL_137e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1388: Unknown result type (might be due to invalid IL or missing references)
		//IL_133f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1347: Unknown result type (might be due to invalid IL or missing references)
		//IL_134c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1351: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_11db: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_069c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_13aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_13af: Unknown result type (might be due to invalid IL or missing references)
		//IL_1395: Unknown result type (might be due to invalid IL or missing references)
		//IL_139d: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06db: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_12cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c43: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_072f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d74: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c74: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0741: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_0751: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ddf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dcc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0776: Unknown result type (might be due to invalid IL or missing references)
		//IL_077b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0769: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_0878: Unknown result type (might be due to invalid IL or missing references)
		//IL_0799: Unknown result type (might be due to invalid IL or missing references)
		//IL_079e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_088d: Unknown result type (might be due to invalid IL or missing references)
		//IL_089b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0808: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_081f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_10bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_10fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_110b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1112: Unknown result type (might be due to invalid IL or missing references)
		//IL_1117: Unknown result type (might be due to invalid IL or missing references)
		//IL_1129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		//IL_0837: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1157: Unknown result type (might be due to invalid IL or missing references)
		//IL_115c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1168: Unknown result type (might be due to invalid IL or missing references)
		//IL_116d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1177: Unknown result type (might be due to invalid IL or missing references)
		//IL_117c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1183: Unknown result type (might be due to invalid IL or missing references)
		//IL_1184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f70: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0848: Unknown result type (might be due to invalid IL or missing references)
		//IL_084f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f82: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0860: Unknown result type (might be due to invalid IL or missing references)
		//IL_0867: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fde: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1008: Unknown result type (might be due to invalid IL or missing references)
		//IL_100d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1012: Unknown result type (might be due to invalid IL or missing references)
		//IL_095f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0966: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0906: Unknown result type (might be due to invalid IL or missing references)
		//IL_1079: Unknown result type (might be due to invalid IL or missing references)
		//IL_107e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1085: Unknown result type (might be due to invalid IL or missing references)
		//IL_108a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1091: Unknown result type (might be due to invalid IL or missing references)
		//IL_1092: Unknown result type (might be due to invalid IL or missing references)
		//IL_101d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1026: Unknown result type (might be due to invalid IL or missing references)
		//IL_102b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1037: Unknown result type (might be due to invalid IL or missing references)
		//IL_103c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1046: Unknown result type (might be due to invalid IL or missing references)
		//IL_1050: Unknown result type (might be due to invalid IL or missing references)
		//IL_1055: Unknown result type (might be due to invalid IL or missing references)
		//IL_105a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0977: Unknown result type (might be due to invalid IL or missing references)
		//IL_097e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0917: Unknown result type (might be due to invalid IL or missing references)
		//IL_091e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0996: Unknown result type (might be due to invalid IL or missing references)
		//IL_092f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0936: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0947: Unknown result type (might be due to invalid IL or missing references)
		//IL_094e: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a77: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d4: Unknown result type (might be due to invalid IL or missing references)
		Edge originalNodes = default(Edge);
		originalNodes.m_Start = GetNetNode(edge.m_Start, data);
		originalNodes.m_End = GetNetNode(edge.m_End, data);
		DynamicBuffer<ConnectedNode> val = data.m_ConnectedNodes[entity];
		tempNodes.Clear();
		for (int i = 0; i < val.Length; i++)
		{
			ConnectedNode connectedNode = val[i];
			connectedNode.m_Node = GetNetNode(connectedNode.m_Node, data);
			tempNodes.Add(ref connectedNode);
		}
		bool flag = owner.m_Owner != Entity.Null;
		Bounds3 val2 = edgeGeometry.m_Bounds | startNodeGeometry.m_Geometry.m_Bounds | endNodeGeometry.m_Geometry.m_Bounds;
		NetCompositionData netCompositionData = data.m_PrefabComposition[composition.m_Edge];
		NetCompositionData netCompositionData2 = data.m_PrefabComposition[composition.m_StartNode];
		NetCompositionData netCompositionData3 = data.m_PrefabComposition[composition.m_EndNode];
		CollisionMask collisionMask = NetUtils.GetCollisionMask(netCompositionData, !editorMode || flag);
		CollisionMask collisionMask2 = NetUtils.GetCollisionMask(netCompositionData2, !editorMode || flag);
		CollisionMask collisionMask3 = NetUtils.GetCollisionMask(netCompositionData3, !editorMode || flag);
		CollisionMask collisionMask4 = collisionMask | collisionMask2 | collisionMask3;
		DynamicBuffer<NetCompositionArea> edgeCompositionAreas = data.m_PrefabCompositionAreas[composition.m_Edge];
		DynamicBuffer<NetCompositionArea> startCompositionAreas = data.m_PrefabCompositionAreas[composition.m_StartNode];
		DynamicBuffer<NetCompositionArea> endCompositionAreas = data.m_PrefabCompositionAreas[composition.m_EndNode];
		Entity val3 = entity;
		if (owner.m_Owner != Entity.Null && !data.m_AssetStamp.HasComponent(owner.m_Owner))
		{
			val3 = owner.m_Owner;
			while (data.m_Owner.HasComponent(val3) && !data.m_Building.HasComponent(val3))
			{
				Entity owner2 = data.m_Owner[val3].m_Owner;
				if (data.m_AssetStamp.HasComponent(owner2))
				{
					break;
				}
				val3 = owner2;
			}
		}
		Edge edge2 = default(Edge);
		bool flag2 = data.m_Edge.TryGetComponent(owner.m_Owner, ref edge2);
		Edge ownerEdge = default(Edge);
		if (flag2)
		{
			ownerEdge.m_Start = GetNetNode(edge2.m_Start, data);
			ownerEdge.m_End = GetNetNode(edge2.m_End, data);
		}
		Edge nodeOwners = default(Edge);
		Edge nodeAssetStamps = default(Edge);
		Edge nodeEdges = default(Edge);
		nodeOwners.m_Start = GetOwner(originalNodes.m_Start, data, out nodeAssetStamps.m_Start, out nodeEdges.m_Start);
		nodeOwners.m_End = GetOwner(originalNodes.m_End, data, out nodeAssetStamps.m_End, out nodeEdges.m_End);
		NetIterator netIterator = default(NetIterator);
		if ((temp.m_Flags & TempFlags.Delete) == 0)
		{
			netIterator = new NetIterator
			{
				m_Edge = edge,
				m_OwnerEdge = ownerEdge,
				m_OriginalNodes = originalNodes,
				m_NodeOwners = nodeOwners,
				m_ConnectedNodes = val.AsNativeArray(),
				m_OriginalConnectedNodes = tempNodes.AsArray(),
				m_TopLevelEntity = val3,
				m_Essential = ((temp.m_Flags & TempFlags.Essential) != 0),
				m_EditorMode = editorMode,
				m_EdgeEntity = entity,
				m_OriginalEntity = temp.m_Original,
				m_Bounds = val2,
				m_EdgeGeometryData = edgeGeometry,
				m_StartNodeGeometryData = startNodeGeometry,
				m_EndNodeGeometryData = endNodeGeometry,
				m_EdgeCompositionData = netCompositionData,
				m_StartCompositionData = netCompositionData2,
				m_EndCompositionData = netCompositionData3,
				m_EdgeCollisionMask = collisionMask,
				m_StartCollisionMask = collisionMask2,
				m_EndCollisionMask = collisionMask3,
				m_CombinedCollisionMask = collisionMask4,
				m_Data = data,
				m_ErrorQueue = errorQueue
			};
			netSearchTree.Iterate<NetIterator>(ref netIterator, 0);
		}
		ObjectIterator objectIterator = default(ObjectIterator);
		if ((temp.m_Flags & TempFlags.Delete) == 0)
		{
			Entity assetStamp;
			Entity edge3;
			Entity owner3 = GetOwner(entity, data, out assetStamp, out edge3);
			objectIterator = new ObjectIterator
			{
				m_OriginalNodes = originalNodes,
				m_NodeOwners = nodeOwners,
				m_NodeAssetStamps = nodeAssetStamps,
				m_NodeEdges = nodeEdges,
				m_OwnerEdge = ownerEdge,
				m_EdgeEntity = entity,
				m_TopLevelEntity = owner3,
				m_AssetStampEntity = assetStamp,
				m_Bounds = val2,
				m_EdgeGeometryData = edgeGeometry,
				m_StartNodeGeometryData = startNodeGeometry,
				m_EndNodeGeometryData = endNodeGeometry,
				m_EdgeCompositionData = netCompositionData,
				m_StartCompositionData = netCompositionData2,
				m_EndCompositionData = netCompositionData3,
				m_EdgeCollisionMask = collisionMask,
				m_StartCollisionMask = collisionMask2,
				m_EndCollisionMask = collisionMask3,
				m_CombinedCollisionMask = collisionMask4,
				m_EdgeCompositionAreas = edgeCompositionAreas,
				m_StartCompositionAreas = startCompositionAreas,
				m_EndCompositionAreas = endCompositionAreas,
				m_Data = data,
				m_ErrorQueue = errorQueue,
				m_EditorMode = editorMode
			};
			objectSearchTree.Iterate<ObjectIterator>(ref objectIterator, 0);
		}
		AreaIterator areaIterator = new AreaIterator
		{
			m_NodeOwners = nodeOwners,
			m_EdgeEntity = entity,
			m_Bounds = val2,
			m_IgnoreCollisions = ((temp.m_Flags & TempFlags.Delete) != 0),
			m_IgnoreProtectedAreas = ((temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Modify | TempFlags.Replace | TempFlags.Upgrade)) == 0 || (temp.m_Flags & TempFlags.Hidden) != 0),
			m_EditorMode = editorMode,
			m_EdgeGeometryData = edgeGeometry,
			m_StartNodeGeometryData = startNodeGeometry,
			m_EndNodeGeometryData = endNodeGeometry,
			m_EdgeCompositionData = netCompositionData,
			m_StartCompositionData = netCompositionData2,
			m_EndCompositionData = netCompositionData3,
			m_EdgeCollisionMask = collisionMask,
			m_StartCollisionMask = collisionMask2,
			m_EndCollisionMask = collisionMask3,
			m_CombinedCollisionMask = collisionMask4,
			m_Data = data,
			m_ErrorQueue = errorQueue
		};
		areaSearchTree.Iterate<AreaIterator>(ref areaIterator, 0);
		if ((temp.m_Flags & TempFlags.Delete) == 0 && edgeList.Length != 0)
		{
			int num = 0;
			int num2 = edgeList.Length;
			float3 val4 = edgeList[edgeList.Length - 1].m_Bounds.max - edgeList[0].m_Bounds.min;
			bool flag3 = val4.z > val4.x;
			while (num < num2)
			{
				int num3 = num + num2 >> 1;
				ValidationSystem.BoundsData boundsData = edgeList[num3];
				bool2 val5 = ((float3)(ref boundsData.m_Bounds.min)).xz < ((float3)(ref val2.min)).xz;
				if (flag3 ? val5.y : val5.x)
				{
					num = num3 + 1;
				}
				else
				{
					num2 = num3;
				}
			}
			Edge edge4 = default(Edge);
			Owner owner4 = default(Owner);
			if (data.m_Owner.TryGetComponent(edge.m_Start, ref owner4))
			{
				edge4.m_Start = owner4.m_Owner;
			}
			if (data.m_Owner.TryGetComponent(edge.m_End, ref owner4))
			{
				edge4.m_End = owner4.m_Owner;
			}
			Owner owner5 = default(Owner);
			Edge edge5 = default(Edge);
			for (int j = 0; j < edgeList.Length; j++)
			{
				ValidationSystem.BoundsData boundsData2 = edgeList[j];
				bool2 val6 = ((float3)(ref boundsData2.m_Bounds.min)).xz > ((float3)(ref val2.max)).xz;
				if (flag3 ? val6.y : val6.x)
				{
					break;
				}
				if ((collisionMask4 & CollisionMask.OnGround) != 0)
				{
					if (!MathUtils.Intersect(((Bounds3)(ref val2)).xz, ((Bounds3)(ref boundsData2.m_Bounds)).xz))
					{
						continue;
					}
				}
				else if (!MathUtils.Intersect(val2, boundsData2.m_Bounds))
				{
					continue;
				}
				if (boundsData2.m_Entity == entity || (boundsData2.m_Bounds.min.x == val2.min.x && boundsData2.m_Entity.Index < entity.Index))
				{
					continue;
				}
				Entity val7 = boundsData2.m_Entity;
				if (data.m_Owner.TryGetComponent(boundsData2.m_Entity, ref owner5))
				{
					Entity owner6 = owner5.m_Owner;
					if (!data.m_AssetStamp.HasComponent(owner6))
					{
						val7 = owner6;
						while (data.m_Owner.HasComponent(val7) && !data.m_Building.HasComponent(val7))
						{
							owner6 = data.m_Owner[val7].m_Owner;
							if (data.m_AssetStamp.HasComponent(owner6))
							{
								break;
							}
							val7 = owner6;
						}
					}
					if (data.m_Edge.TryGetComponent(owner5.m_Owner, ref edge5) && (edge.m_Start == edge5.m_Start || edge.m_Start == edge5.m_End || edge.m_End == edge5.m_Start || edge.m_End == edge5.m_End))
					{
						continue;
					}
				}
				if (!(val3 == val7))
				{
					Edge edgeData = data.m_Edge[boundsData2.m_Entity];
					if (!(edge.m_Start == edgeData.m_Start) && !(edge.m_Start == edgeData.m_End) && !(edge.m_End == edgeData.m_Start) && !(edge.m_End == edgeData.m_End) && (!flag2 || (!(edge2.m_Start == edgeData.m_Start) && !(edge2.m_Start == edgeData.m_End) && !(edge2.m_End == edgeData.m_Start) && !(edge2.m_End == edgeData.m_End))) && !(boundsData2.m_Entity == edge4.m_Start) && !(boundsData2.m_Entity == edge4.m_End) && (!data.m_Owner.TryGetComponent(edgeData.m_Start, ref owner4) || !(owner4.m_Owner == entity)) && (!data.m_Owner.TryGetComponent(edgeData.m_End, ref owner4) || !(owner4.m_Owner == entity)))
					{
						EdgeGeometry edgeGeometryData = data.m_EdgeGeometry[boundsData2.m_Entity];
						StartNodeGeometry startNodeGeometryData = data.m_StartNodeGeometry[boundsData2.m_Entity];
						EndNodeGeometry endNodeGeometryData = data.m_EndNodeGeometry[boundsData2.m_Entity];
						Composition compositionData = data.m_Composition[boundsData2.m_Entity];
						Temp temp2 = data.m_Temp[boundsData2.m_Entity];
						netIterator.CheckOverlap(val7, boundsData2.m_Entity, boundsData2.m_Bounds, edgeData, compositionData, edgeGeometryData, startNodeGeometryData, endNodeGeometryData, (temp2.m_Flags & TempFlags.Essential) != 0, owner5.m_Owner != Entity.Null);
					}
				}
			}
		}
		Bounds3 errorBounds = default(Bounds3);
		errorBounds.min = float3.op_Implicit(float.MaxValue);
		errorBounds.max = float3.op_Implicit(float.MinValue);
		bool flag4 = false;
		if ((temp.m_Flags & TempFlags.Essential) == 0)
		{
			flag4 = !data.m_NetElevation.HasComponent(entity) || !data.m_NetElevation.HasComponent(edge.m_Start) || !data.m_NetElevation.HasComponent(edge.m_End);
		}
		if ((temp.m_Flags & TempFlags.Delete) == 0)
		{
			bool num4 = val3 != entity && IsInternal(val3, edge.m_Start, data.m_ConnectedEdges[edge.m_Start], data);
			bool flag5 = val3 != entity && IsInternal(val3, edge.m_End, data.m_ConnectedEdges[edge.m_End], data);
			bool flag6 = false;
			if (!num4 || !flag5)
			{
				flag6 |= CheckGeometryShape(edgeGeometry, ref errorBounds);
			}
			if (!num4)
			{
				flag6 |= CheckGeometryShape(startNodeGeometry.m_Geometry, ref errorBounds);
			}
			if (!flag5)
			{
				flag6 |= CheckGeometryShape(endNodeGeometry.m_Geometry, ref errorBounds);
			}
			if (flag6)
			{
				errorQueue.Enqueue(new ErrorData
				{
					m_ErrorType = ErrorType.InvalidShape,
					m_ErrorSeverity = ErrorSeverity.Error,
					m_Position = MathUtils.Center(errorBounds),
					m_TempEntity = entity
				});
			}
			if (data.m_PrefabNetGeometry.HasComponent(prefabRef.m_Prefab))
			{
				Curve curve = data.m_Curve[entity];
				NetGeometryData netGeometryData = data.m_PrefabNetGeometry[prefabRef.m_Prefab];
				Bounds1 edgeLengthRange = netGeometryData.m_EdgeLengthRange;
				if (_fixed.m_Index >= 0 && data.m_PrefabFixedElements.HasBuffer(prefabRef.m_Prefab))
				{
					DynamicBuffer<FixedNetElement> val8 = data.m_PrefabFixedElements[prefabRef.m_Prefab];
					if (_fixed.m_Index < val8.Length)
					{
						Bounds1 lengthRange = val8[_fixed.m_Index].m_LengthRange;
						edgeLengthRange.min = math.select(lengthRange.min, lengthRange.min * 0.6f, lengthRange.max == lengthRange.min);
						edgeLengthRange.max = lengthRange.max;
					}
				}
				if ((netCompositionData.m_State & CompositionState.HalfLength) != 0)
				{
					edgeLengthRange.min *= 0.1f;
				}
				edgeLengthRange.max *= 1.1f;
				Bezier4x3 val9 = MathUtils.Lerp(edgeGeometry.m_Start.m_Left, edgeGeometry.m_Start.m_Right, 0.5f);
				Bezier4x3 val10 = MathUtils.Lerp(edgeGeometry.m_End.m_Left, edgeGeometry.m_End.m_Right, 0.5f);
				float num5 = MathUtils.Length(((Bezier4x3)(ref val9)).xz);
				float num6 = MathUtils.Length(((Bezier4x3)(ref val10)).xz);
				if (num5 + num6 < edgeLengthRange.min)
				{
					errorQueue.Enqueue(new ErrorData
					{
						m_ErrorType = ErrorType.ShortDistance,
						m_ErrorSeverity = ErrorSeverity.Error,
						m_Position = math.lerp(edgeGeometry.m_Start.m_Left.d, edgeGeometry.m_Start.m_Right.d, 0.5f),
						m_TempEntity = entity
					});
				}
				if (num5 + num6 > edgeLengthRange.max)
				{
					errorQueue.Enqueue(new ErrorData
					{
						m_ErrorType = ErrorType.LongDistance,
						m_ErrorSeverity = ErrorSeverity.Error,
						m_Position = math.lerp(edgeGeometry.m_Start.m_Left.d, edgeGeometry.m_Start.m_Right.d, 0.5f),
						m_TempEntity = entity
					});
				}
				if ((netGeometryData.m_Flags & GeometryFlags.FlattenTerrain) != 0 && (temp.m_Flags & TempFlags.Essential) != 0)
				{
					flag4 = false;
				}
				if (netGeometryData.m_MaxSlopeSteepness != 0f && !flag4)
				{
					float3 val11 = default(float3);
					val11.x = math.abs(val9.d.y - val9.a.y) / math.max(0.1f, num5);
					val11.y = math.abs(val10.d.y - val10.a.y) / math.max(0.1f, num6);
					val11.z = math.abs(curve.m_Bezier.d.y - curve.m_Bezier.a.y) / math.max(0.1f, MathUtils.Length(((Bezier4x3)(ref curve.m_Bezier)).xz));
					bool3 val12 = val11 >= new float3(netGeometryData.m_MaxSlopeSteepness * 2f, netGeometryData.m_MaxSlopeSteepness * 2f, netGeometryData.m_MaxSlopeSteepness + 0.0005f);
					if (math.any(val12))
					{
						float4 val13 = default(float4);
						if (val12.x)
						{
							val13 += new float4(math.lerp(MathUtils.Position(edgeGeometry.m_Start.m_Left, 0.5f), MathUtils.Position(edgeGeometry.m_Start.m_Right, 0.5f), 0.5f), 1f);
						}
						if (val12.y)
						{
							val13 += new float4(math.lerp(MathUtils.Position(edgeGeometry.m_End.m_Left, 0.5f), MathUtils.Position(edgeGeometry.m_End.m_Right, 0.5f), 0.5f), 1f);
						}
						if (val12.z)
						{
							val13 += new float4(math.lerp(edgeGeometry.m_Start.m_Left.d, edgeGeometry.m_Start.m_Right.d, 0.5f), 1f);
						}
						errorQueue.Enqueue(new ErrorData
						{
							m_ErrorType = ErrorType.SteepSlope,
							m_ErrorSeverity = ErrorSeverity.Error,
							m_Position = ((float4)(ref val13)).xyz / val13.w,
							m_TempEntity = entity
						});
					}
				}
				if ((netGeometryData.m_Flags & GeometryFlags.RequireElevated) != 0)
				{
					Elevation elevation = default(Elevation);
					data.m_NetElevation.TryGetComponent(edge.m_Start, ref elevation);
					Elevation elevation2 = default(Elevation);
					data.m_NetElevation.TryGetComponent(entity, ref elevation2);
					Elevation elevation3 = default(Elevation);
					data.m_NetElevation.TryGetComponent(edge.m_End, ref elevation3);
					if (!math.all(math.max(float2.op_Implicit(math.max(math.cmin(elevation.m_Elevation), math.cmin(elevation3.m_Elevation))), elevation2.m_Elevation) >= netGeometryData.m_ElevationLimit * 2f))
					{
						errorQueue.Enqueue(new ErrorData
						{
							m_ErrorType = ErrorType.LowElevation,
							m_ErrorSeverity = ErrorSeverity.Error,
							m_Position = math.lerp(edgeGeometry.m_Start.m_Left.d, edgeGeometry.m_Start.m_Right.d, 0.5f),
							m_TempEntity = entity
						});
					}
				}
			}
		}
		if ((temp.m_Flags & (TempFlags.Create | TempFlags.Modify)) != 0 && !flag4 && data.m_PlaceableNet.HasComponent(prefabRef.m_Prefab))
		{
			PlaceableNetData placeableNetData = data.m_PlaceableNet[prefabRef.m_Prefab];
			errorBounds.min = float3.op_Implicit(float.MaxValue);
			errorBounds.max = float3.op_Implicit(float.MinValue);
			if (CheckSurface(waterSurfaceData, terrainHeightData, placeableNetData, netCompositionData, edgeGeometry.m_Start, ref errorBounds) | CheckSurface(waterSurfaceData, terrainHeightData, placeableNetData, netCompositionData, edgeGeometry.m_End, ref errorBounds) | CheckSurface(waterSurfaceData, terrainHeightData, placeableNetData, netCompositionData2, startNodeGeometry.m_Geometry.m_Left, ref errorBounds) | CheckSurface(waterSurfaceData, terrainHeightData, placeableNetData, netCompositionData2, startNodeGeometry.m_Geometry.m_Right, ref errorBounds) | CheckSurface(waterSurfaceData, terrainHeightData, placeableNetData, netCompositionData3, endNodeGeometry.m_Geometry.m_Left, ref errorBounds) | CheckSurface(waterSurfaceData, terrainHeightData, placeableNetData, netCompositionData3, endNodeGeometry.m_Geometry.m_Right, ref errorBounds))
			{
				ErrorData errorData = default(ErrorData);
				if ((placeableNetData.m_PlacementFlags & PlacementFlags.Floating) != PlacementFlags.None)
				{
					errorData.m_ErrorType = ErrorType.NoWater;
				}
				else
				{
					errorData.m_ErrorType = ErrorType.InWater;
				}
				errorData.m_ErrorSeverity = ErrorSeverity.Error;
				errorData.m_Position = MathUtils.Center(errorBounds);
				errorData.m_TempEntity = entity;
				errorQueue.Enqueue(errorData);
			}
		}
		if ((temp.m_Flags & (TempFlags.Create | TempFlags.Modify | TempFlags.Replace | TempFlags.Upgrade)) != 0)
		{
			val2 = edgeGeometry.m_Bounds;
			if (math.any(startNodeGeometry.m_Geometry.m_Left.m_Length > 0.05f) | math.any(startNodeGeometry.m_Geometry.m_Right.m_Length > 0.05f))
			{
				val2 |= startNodeGeometry.m_Geometry.m_Bounds;
			}
			if (math.any(endNodeGeometry.m_Geometry.m_Left.m_Length > 0.05f) | math.any(endNodeGeometry.m_Geometry.m_Right.m_Length > 0.05f))
			{
				val2 |= endNodeGeometry.m_Geometry.m_Bounds;
			}
			Game.Objects.ValidationHelpers.ValidateWorldBounds(entity, owner, val2, data, terrainHeightData, errorQueue);
		}
	}

	private static bool IsInternal(Entity topLevelEntity, Entity node, DynamicBuffer<ConnectedEdge> connectedEdges, ValidationSystem.EntityData data)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < connectedEdges.Length; i++)
		{
			Entity val = connectedEdges[i].m_Edge;
			Edge edge = data.m_Edge[val];
			if (!(edge.m_Start == node) && !(edge.m_End == node))
			{
				continue;
			}
			while (data.m_Owner.HasComponent(val) && !data.m_Building.HasComponent(val))
			{
				Entity owner = data.m_Owner[val].m_Owner;
				if (data.m_AssetStamp.HasComponent(owner))
				{
					break;
				}
				val = owner;
			}
			if (topLevelEntity != val)
			{
				return false;
			}
		}
		return true;
	}

	private static bool CheckSurface(WaterSurfaceData waterSurfaceData, TerrainHeightData terrainHeightData, PlaceableNetData placeableNetData, NetCompositionData compositionData, Segment segment, ref Bounds3 errorBounds)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		bool flag = (placeableNetData.m_PlacementFlags & (PlacementFlags.OnGround | PlacementFlags.Floating)) == PlacementFlags.Floating;
		bool flag2 = (placeableNetData.m_PlacementFlags & (PlacementFlags.OnGround | PlacementFlags.Floating | PlacementFlags.ShoreLine)) == PlacementFlags.OnGround;
		flag2 &= (compositionData.m_Flags.m_General & CompositionFlags.General.Tunnel) == 0;
		if (flag || flag2)
		{
			float sampleInterval = WaterUtils.GetSampleInterval(ref waterSurfaceData);
			int num = (int)math.ceil(segment.middleLength / sampleInterval);
			for (int i = 0; i < num; i++)
			{
				float num2 = ((float)i + 0.5f) / (float)num;
				float3 val = MathUtils.Position(segment.m_Left, num2);
				float3 val2 = MathUtils.Position(segment.m_Right, num2);
				int num3 = (int)math.ceil(math.distance(val, val2) / sampleInterval);
				for (int j = 0; j < num3; j++)
				{
					float num4 = ((float)j + 0.5f) / (float)num3;
					float3 val3 = math.lerp(val, val2, num4);
					float num5 = WaterUtils.SampleDepth(ref waterSurfaceData, val3);
					if (flag2 && num5 >= 0.2f && WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, val3) > val3.y + compositionData.m_HeightRange.min)
					{
						errorBounds |= val3;
						result = true;
					}
					if (flag && num5 < 0.2f)
					{
						errorBounds |= val3;
						result = true;
					}
				}
			}
		}
		return result;
	}

	private static Entity GetNetNode(Entity entity, ValidationSystem.EntityData data)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Temp temp = default(Temp);
		if (data.m_Temp.TryGetComponent(entity, ref temp))
		{
			return temp.m_Original;
		}
		return entity;
	}

	private static Entity GetOwner(Entity entity, ValidationSystem.EntityData data, out Entity assetStamp, out Entity edge)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		assetStamp = Entity.Null;
		edge = Entity.Null;
		Owner owner = default(Owner);
		while (data.m_Owner.TryGetComponent(entity, ref owner) && !data.m_Building.HasComponent(entity))
		{
			if (data.m_AssetStamp.HasComponent(owner.m_Owner))
			{
				assetStamp = owner.m_Owner;
				break;
			}
			if (data.m_Edge.HasComponent(owner.m_Owner))
			{
				edge = owner.m_Owner;
			}
			entity = owner.m_Owner;
			if (data.m_Temp.HasComponent(entity))
			{
				entity = data.m_Temp[entity].m_Original;
			}
		}
		return entity;
	}

	public static void ValidateLane(Entity entity, Owner owner, Lane lane, TrackLane trackLane, Curve curve, EdgeLane edgeLane, PrefabRef prefabRef, ValidationSystem.EntityData data, ParallelWriter<ErrorData> errorQueue)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		if (data.m_Edge.HasComponent(owner.m_Owner))
		{
			TrackLaneData trackLaneData = data.m_TrackLaneData[prefabRef.m_Prefab];
			Temp temp = default(Temp);
			if (trackLane.m_Curviness > trackLaneData.m_MaxCurviness && data.m_Temp.TryGetComponent(owner.m_Owner, ref temp) && (temp.m_Flags & TempFlags.Essential) != 0)
			{
				errorQueue.Enqueue(new ErrorData
				{
					m_ErrorType = ErrorType.TightCurve,
					m_Position = MathUtils.Position(curve.m_Bezier, 0.5f),
					m_ErrorSeverity = ErrorSeverity.Error,
					m_TempEntity = owner.m_Owner
				});
			}
			Edge edge = data.m_Edge[owner.m_Owner];
			bool flag = (trackLane.m_Flags & TrackLaneFlags.Twoway) != 0;
			bool flag2;
			Entity val;
			if (edgeLane.m_EdgeDelta.x < 0.001f)
			{
				flag2 = FindConnectedLane(edge.m_Start, owner.m_Owner, lane.m_StartNode, data) || IsIgnored(owner.m_Owner, edge.m_Start, data, trackLaneData.m_TrackTypes, flag, isTarget: true);
				val = edge.m_Start;
			}
			else if (edgeLane.m_EdgeDelta.x > 0.999f)
			{
				flag2 = FindConnectedLane(edge.m_End, owner.m_Owner, lane.m_StartNode, data) || IsIgnored(owner.m_Owner, edge.m_End, data, trackLaneData.m_TrackTypes, flag, isTarget: true);
				val = edge.m_End;
			}
			else
			{
				flag2 = true;
				val = Entity.Null;
			}
			bool flag3;
			Entity val2;
			if (edgeLane.m_EdgeDelta.y < 0.001f)
			{
				flag3 = FindConnectedLane(edge.m_Start, owner.m_Owner, lane.m_EndNode, data) || IsIgnored(owner.m_Owner, edge.m_Start, data, trackLaneData.m_TrackTypes, isSource: true, flag);
				val2 = edge.m_Start;
			}
			else if (edgeLane.m_EdgeDelta.y > 0.999f)
			{
				flag3 = FindConnectedLane(edge.m_End, owner.m_Owner, lane.m_EndNode, data) || IsIgnored(owner.m_Owner, edge.m_End, data, trackLaneData.m_TrackTypes, isSource: true, flag);
				val2 = edge.m_End;
			}
			else
			{
				flag3 = true;
				val2 = Entity.Null;
			}
			Temp temp2 = default(Temp);
			if (!flag2 && data.m_Temp.TryGetComponent(val, ref temp2) && (temp2.m_Flags & TempFlags.Essential) != 0)
			{
				errorQueue.Enqueue(new ErrorData
				{
					m_ErrorType = ErrorType.TightCurve,
					m_Position = (curve.m_Bezier.a + data.m_Node[val].m_Position) * 0.5f,
					m_ErrorSeverity = ErrorSeverity.Warning,
					m_TempEntity = val
				});
			}
			Temp temp3 = default(Temp);
			if (!flag3 && data.m_Temp.TryGetComponent(val2, ref temp3) && (temp3.m_Flags & TempFlags.Essential) != 0)
			{
				errorQueue.Enqueue(new ErrorData
				{
					m_ErrorType = ErrorType.TightCurve,
					m_Position = (curve.m_Bezier.d + data.m_Node[val2].m_Position) * 0.5f,
					m_ErrorSeverity = ErrorSeverity.Warning,
					m_TempEntity = val2
				});
			}
		}
	}

	private static bool FindConnectedLane(Entity owner, Entity ignore, PathNode node, ValidationSystem.EntityData data)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (FindConnectedLane(owner, node, data))
		{
			return true;
		}
		DynamicBuffer<ConnectedEdge> val = default(DynamicBuffer<ConnectedEdge>);
		if (node.OwnerEquals(new PathNode(owner, 0)) && data.m_ConnectedEdges.TryGetBuffer(owner, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				ConnectedEdge connectedEdge = val[i];
				if (!(connectedEdge.m_Edge == ignore) && FindConnectedLane(connectedEdge.m_Edge, node, data))
				{
					return true;
				}
			}
		}
		return false;
	}

	private static bool FindConnectedLane(Entity owner, PathNode node, ValidationSystem.EntityData data)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<SubLane> val = data.m_Lanes[owner];
		for (int i = 0; i < val.Length; i++)
		{
			Lane lane = data.m_Lane[val[i].m_SubLane];
			if (lane.m_StartNode.Equals(node) || lane.m_EndNode.Equals(node))
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsIgnored(Entity edge, Entity node, ValidationSystem.EntityData data, TrackTypes trackTypes, bool isSource, bool isTarget)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		EdgeIterator edgeIterator = new EdgeIterator(edge, node, data.m_ConnectedEdges, data.m_Edge, data.m_Temp, data.m_Hidden);
		EdgeIteratorValue value;
		DynamicBuffer<SubLane> val = default(DynamicBuffer<SubLane>);
		TrackLane trackLane = default(TrackLane);
		while (edgeIterator.GetNext(out value))
		{
			if (value.m_Edge == edge || !data.m_Lanes.TryGetBuffer(value.m_Edge, ref val))
			{
				continue;
			}
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				if (!data.m_TrackLane.TryGetComponent(subLane, ref trackLane))
				{
					continue;
				}
				PrefabRef prefabRef = data.m_PrefabRef[subLane];
				if (data.m_TrackLaneData[prefabRef.m_Prefab].m_TrackTypes == trackTypes)
				{
					bool num = (trackLane.m_Flags & TrackLaneFlags.Twoway) != 0;
					bool flag = (trackLane.m_Flags & TrackLaneFlags.Invert) != 0;
					bool flag2 = num | (value.m_End != flag);
					bool flag3 = num | (value.m_End == flag);
					if ((isSource && flag3) || (isTarget && flag2))
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	private static bool CheckGeometryShape(EdgeGeometry geometry, ref Bounds3 errorBounds)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (math.any(geometry.m_Start.m_Length + geometry.m_End.m_Length > 0.1f))
		{
			return CheckSegmentShape(geometry.m_Start, ref errorBounds) | CheckSegmentShape(geometry.m_End, ref errorBounds);
		}
		return false;
	}

	private static bool CheckGeometryShape(EdgeNodeGeometry geometry, ref Bounds3 errorBounds)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (math.any(geometry.m_Left.m_Length > 0.05f) | math.any(geometry.m_Right.m_Length > 0.05f))
		{
			return CheckSegmentShape(geometry.m_Left, ref errorBounds) | CheckSegmentShape(geometry.m_Right, ref errorBounds);
		}
		return false;
	}

	private static bool CheckSegmentShape(Segment segment, ref Bounds3 errorBounds)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		Quad3 val = default(Quad3);
		val.a = segment.m_Left.a;
		val.b = segment.m_Right.a;
		float3 val2 = val.b - val.a;
		for (int i = 1; i <= 8; i++)
		{
			float num = (float)i / 8f;
			val.d = MathUtils.Position(segment.m_Left, num);
			val.c = MathUtils.Position(segment.m_Right, num);
			float3 val3 = val.d - val.a;
			float3 val4 = val.c - val.b;
			float3 val5 = val.c - val.d;
			val3 = math.select(val3, float3.op_Implicit(0f), math.lengthsq(val3) < 0.0001f);
			val4 = math.select(val4, float3.op_Implicit(0f), math.lengthsq(val4) < 0.0001f);
			if ((math.cross(val3, val2).y < 0f) | (math.cross(val4, val5).y < 0f))
			{
				errorBounds |= MathUtils.Bounds(val);
				result = true;
			}
			val.a = val.d;
			val.b = val.c;
			val2 = val5;
		}
		return result;
	}

	public static bool Intersect(Edge edge1, Entity node2, EdgeGeometry edgeGeometry1, float3 offset1, Box3 box2, Bounds3 bounds2, NetCompositionData prefabCompositionData1, DynamicBuffer<NetCompositionArea> areas1, ref Bounds3 intersection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(edgeGeometry1.m_Bounds, bounds2))
		{
			return false;
		}
		if (math.all(edgeGeometry1.m_Start.m_Length + edgeGeometry1.m_End.m_Length <= 0.1f))
		{
			return false;
		}
		bool flag = false;
		if (edge1.m_Start != node2 || (prefabCompositionData1.m_State & CompositionState.HasSurface) != 0)
		{
			flag |= Intersect(edgeGeometry1.m_Start, new float2(0f, 1f), offset1, box2, bounds2, prefabCompositionData1, areas1, ref intersection);
		}
		if (edge1.m_End != node2 || (prefabCompositionData1.m_State & CompositionState.HasSurface) != 0)
		{
			flag |= Intersect(edgeGeometry1.m_End, new float2(0f, 1f), offset1, box2, bounds2, prefabCompositionData1, areas1, ref intersection);
		}
		return flag;
	}

	public static bool Intersect(Edge edge1, Entity node2, EdgeGeometry edgeGeometry1, float2 offset1, Quad2 quad2, Bounds2 bounds2, NetCompositionData prefabCompositionData1, DynamicBuffer<NetCompositionArea> areas1, ref Bounds2 intersection)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(((Bounds3)(ref edgeGeometry1.m_Bounds)).xz, bounds2))
		{
			return false;
		}
		if (math.all(edgeGeometry1.m_Start.m_Length + edgeGeometry1.m_End.m_Length <= 0.1f))
		{
			return false;
		}
		bool flag = false;
		if (edge1.m_Start != node2 || (prefabCompositionData1.m_State & CompositionState.HasSurface) != 0)
		{
			flag |= Intersect(edgeGeometry1.m_Start, new float2(0f, 1f), offset1, quad2, bounds2, prefabCompositionData1, areas1, ref intersection);
		}
		if (edge1.m_End != node2 || (prefabCompositionData1.m_State & CompositionState.HasSurface) != 0)
		{
			flag |= Intersect(edgeGeometry1.m_End, new float2(0f, 1f), offset1, quad2, bounds2, prefabCompositionData1, areas1, ref intersection);
		}
		return flag;
	}

	public static bool Intersect(Entity node1, Entity node2, EdgeNodeGeometry nodeGeometry1, float3 offset1, Box3 box2, Bounds3 bounds2, NetCompositionData prefabCompositionData1, DynamicBuffer<NetCompositionArea> areas1, ref Bounds3 intersection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(nodeGeometry1.m_Bounds, bounds2))
		{
			return false;
		}
		if (node1 == node2)
		{
			return false;
		}
		if (math.all(nodeGeometry1.m_Left.m_Length <= 0.05f) && math.all(nodeGeometry1.m_Left.m_Length <= 0.05f))
		{
			return false;
		}
		if (nodeGeometry1.m_MiddleRadius > 0f)
		{
			Segment right = nodeGeometry1.m_Right;
			Segment right2 = nodeGeometry1.m_Right;
			right.m_Right = MathUtils.Lerp(nodeGeometry1.m_Right.m_Left, nodeGeometry1.m_Right.m_Right, 0.5f);
			right.m_Right.d = nodeGeometry1.m_Middle.d;
			right2.m_Left = right.m_Right;
			return Intersect(nodeGeometry1.m_Left, new float2(0f, 1f), offset1, box2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(right, new float2(0f, 0.5f), offset1, box2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(right2, new float2(0.5f, 1f), offset1, box2, bounds2, prefabCompositionData1, areas1, ref intersection);
		}
		Segment left = nodeGeometry1.m_Left;
		Segment right3 = nodeGeometry1.m_Right;
		left.m_Right = nodeGeometry1.m_Middle;
		right3.m_Left = nodeGeometry1.m_Middle;
		return Intersect(nodeGeometry1.m_Left, new float2(0f, 0.5f), offset1, box2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(nodeGeometry1.m_Right, new float2(0.5f, 1f), offset1, box2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(left, new float2(0f, 0.5f), offset1, box2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(right3, new float2(0.5f, 1f), offset1, box2, bounds2, prefabCompositionData1, areas1, ref intersection);
	}

	public static bool Intersect(Entity node1, Entity node2, EdgeNodeGeometry nodeGeometry1, float2 offset1, Quad2 quad2, Bounds2 bounds2, NetCompositionData prefabCompositionData1, DynamicBuffer<NetCompositionArea> areas1, ref Bounds2 intersection)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(((Bounds3)(ref nodeGeometry1.m_Bounds)).xz, bounds2))
		{
			return false;
		}
		if (node1 == node2)
		{
			return false;
		}
		if (math.all(nodeGeometry1.m_Left.m_Length <= 0.05f) && math.all(nodeGeometry1.m_Left.m_Length <= 0.05f))
		{
			return false;
		}
		if (nodeGeometry1.m_MiddleRadius > 0f)
		{
			Segment right = nodeGeometry1.m_Right;
			Segment right2 = nodeGeometry1.m_Right;
			right.m_Right = MathUtils.Lerp(nodeGeometry1.m_Right.m_Left, nodeGeometry1.m_Right.m_Right, 0.5f);
			right.m_Right.d = nodeGeometry1.m_Middle.d;
			right2.m_Left = right.m_Right;
			return Intersect(nodeGeometry1.m_Left, new float2(0f, 1f), offset1, quad2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(right, new float2(0f, 0.5f), offset1, quad2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(right2, new float2(0.5f, 1f), offset1, quad2, bounds2, prefabCompositionData1, areas1, ref intersection);
		}
		Segment left = nodeGeometry1.m_Left;
		Segment right3 = nodeGeometry1.m_Right;
		left.m_Right = nodeGeometry1.m_Middle;
		right3.m_Left = nodeGeometry1.m_Middle;
		return Intersect(nodeGeometry1.m_Left, new float2(0f, 0.5f), offset1, quad2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(nodeGeometry1.m_Right, new float2(0.5f, 1f), offset1, quad2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(left, new float2(0f, 0.5f), offset1, quad2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(right3, new float2(0.5f, 1f), offset1, quad2, bounds2, prefabCompositionData1, areas1, ref intersection);
	}

	public static bool Intersect(Segment segment1, float2 segmentSide1, float3 offset1, Box3 box2, Bounds3 bounds2, NetCompositionData prefabCompositionData1, DynamicBuffer<NetCompositionArea> areas1, ref Bounds3 intersection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(SetHeightRange(MathUtils.Bounds(segment1.m_Left) | MathUtils.Bounds(segment1.m_Right), prefabCompositionData1.m_HeightRange), bounds2))
		{
			return false;
		}
		float3 val4;
		if (areas1.IsCreated)
		{
			float num3 = default(float);
			for (int i = 0; i < areas1.Length; i++)
			{
				NetCompositionArea netCompositionArea = areas1[i];
				if ((netCompositionArea.m_Flags & (NetAreaFlags.Buildable | NetAreaFlags.Hole)) == 0)
				{
					continue;
				}
				float num = netCompositionArea.m_Width * 0.51f;
				float3 val = MathUtils.Size(box2.bounds) * 0.5f;
				if (math.cmin(val) >= num)
				{
					continue;
				}
				float num2 = netCompositionArea.m_Position.x / prefabCompositionData1.m_Width + 0.5f;
				if (num2 < segmentSide1.x || num2 > segmentSide1.y)
				{
					continue;
				}
				Bezier4x3 val2 = MathUtils.Lerp(segment1.m_Left, segment1.m_Right, (num2 - segmentSide1.x) / (segmentSide1.y - segmentSide1.x));
				Bounds3 bounds3 = MathUtils.Bounds(val2);
				ref float3 min = ref bounds3.min;
				((float3)(ref min)).xz = ((float3)(ref min)).xz - num;
				ref float3 max = ref bounds3.max;
				((float3)(ref max)).xz = ((float3)(ref max)).xz + num;
				if (!MathUtils.Intersect(SetHeightRange(bounds3, prefabCompositionData1.m_HeightRange), bounds2))
				{
					continue;
				}
				val2 += offset1;
				float3 val3 = math.mul(box2.rotation, MathUtils.Center(box2.bounds));
				val4 = math.mul(box2.rotation, new float3(val.x, 0f, 0f));
				float2 xz = ((float3)(ref val4)).xz;
				val4 = math.mul(box2.rotation, new float3(0f, val.y, 0f));
				float2 xz2 = ((float3)(ref val4)).xz;
				val4 = math.mul(box2.rotation, new float3(0f, 0f, val.z));
				float2 xz3 = ((float3)(ref val4)).xz;
				MathUtils.Distance(((Bezier4x3)(ref val2)).xz, ((float3)(ref val3)).xz, ref num3);
				float3 val5 = MathUtils.Position(val2, num3);
				if ((netCompositionArea.m_Flags & NetAreaFlags.Hole) != 0 || !(bounds2.min.y + offset1.y <= val5.y + prefabCompositionData1.m_HeightRange.min))
				{
					val4 = MathUtils.Tangent(val2, num3);
					float2 val6 = MathUtils.Right(math.normalizesafe(((float3)(ref val4)).xz, default(float2)));
					if (math.abs(math.dot(((float3)(ref val3)).xz - ((float3)(ref val5)).xz, val6)) + math.csum(math.abs(new float3(math.dot(xz, val6), math.dot(xz2, val6), math.dot(xz3, val6)))) < num)
					{
						return false;
					}
				}
			}
		}
		bool result = false;
		Quad3 val7 = default(Quad3);
		val7.a = segment1.m_Left.a;
		val7.b = segment1.m_Right.a;
		Bounds3 val8 = SetHeightRange(MathUtils.Bounds(val7.a, val7.b), prefabCompositionData1.m_HeightRange);
		TrigonalTrapezohedron3 val15 = default(TrigonalTrapezohedron3);
		float3 val16 = default(float3);
		float3 val17 = default(float3);
		Bounds3 val18 = default(Bounds3);
		Bounds3 val19 = default(Bounds3);
		Box3 val20 = default(Box3);
		for (int j = 1; j <= 8; j++)
		{
			float num4 = (float)j / 8f;
			val7.d = MathUtils.Position(segment1.m_Left, num4);
			val7.c = MathUtils.Position(segment1.m_Right, num4);
			Bounds3 val9 = SetHeightRange(MathUtils.Bounds(val7.d, val7.c), prefabCompositionData1.m_HeightRange);
			if (MathUtils.Intersect(val8 | val9, bounds2))
			{
				Quad3 val10 = val7;
				float3 val11 = val10.b - val10.a;
				val4 = default(float3);
				float3 val12 = math.normalizesafe(val11, val4) * 0.5f;
				float3 val13 = val10.d - val10.c;
				val4 = default(float3);
				float3 val14 = math.normalizesafe(val13, val4) * 0.5f;
				ref float3 a = ref val10.a;
				a += val12;
				ref float3 b = ref val10.b;
				b -= val12;
				ref float3 c = ref val10.c;
				c += val14;
				ref float3 d = ref val10.d;
				d -= val14;
				((TrigonalTrapezohedron3)(ref val15))._002Ector(val10, val10);
				((float3)(ref val16))._002Ector(offset1.x, offset1.y + prefabCompositionData1.m_HeightRange.min, offset1.z);
				((float3)(ref val17))._002Ector(offset1.x, offset1.y + prefabCompositionData1.m_HeightRange.max, offset1.z);
				ref float3 a2 = ref val15.a.a;
				a2 += val16;
				ref float3 b2 = ref val15.a.b;
				b2 += val16;
				ref float3 c2 = ref val15.a.c;
				c2 += val16;
				ref float3 d2 = ref val15.a.d;
				d2 += val16;
				ref float3 a3 = ref val15.b.a;
				a3 += val17;
				ref float3 b3 = ref val15.b.b;
				b3 += val17;
				ref float3 c3 = ref val15.b.c;
				c3 += val17;
				ref float3 d3 = ref val15.b.d;
				d3 += val17;
				if (MathUtils.Intersect(val15, box2, ref val18, ref val19))
				{
					((Box3)(ref val20))._002Ector(val19, box2.rotation);
					result = true;
					intersection |= val18 | MathUtils.Bounds(val20);
				}
			}
			val7.a = val7.d;
			val7.b = val7.c;
			val8 = val9;
		}
		return result;
	}

	public static bool Intersect(Segment segment1, float2 segmentSide1, float2 offset1, Quad2 quad2, Bounds2 bounds2, NetCompositionData prefabCompositionData1, DynamicBuffer<NetCompositionArea> areas1, ref Bounds2 intersection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 val = MathUtils.Bounds(segment1.m_Left) | MathUtils.Bounds(segment1.m_Right);
		if (!MathUtils.Intersect(((Bounds3)(ref val)).xz, bounds2))
		{
			return false;
		}
		if (areas1.IsCreated)
		{
			float num3 = default(float);
			for (int i = 0; i < areas1.Length; i++)
			{
				NetCompositionArea netCompositionArea = areas1[i];
				if ((netCompositionArea.m_Flags & (NetAreaFlags.Buildable | NetAreaFlags.Hole)) == 0)
				{
					continue;
				}
				float2 val2 = new float2(math.max(math.distance(quad2.a, quad2.b), math.distance(quad2.c, quad2.d)), math.max(math.distance(quad2.b, quad2.c), math.distance(quad2.d, quad2.a)));
				float num = netCompositionArea.m_Width * 0.51f;
				if (math.cmin(val2) * 0.5f >= num)
				{
					continue;
				}
				float num2 = netCompositionArea.m_Position.x / prefabCompositionData1.m_Width + 0.5f;
				if (num2 < segmentSide1.x || num2 > segmentSide1.y)
				{
					continue;
				}
				Bezier4x3 val3 = MathUtils.Lerp(segment1.m_Left, segment1.m_Right, (num2 - segmentSide1.x) / (segmentSide1.y - segmentSide1.x));
				Bezier4x2 xz = ((Bezier4x3)(ref val3)).xz;
				Bounds2 val4 = MathUtils.Bounds(xz);
				ref float2 min = ref val4.min;
				min -= num;
				ref float2 max = ref val4.max;
				max += num;
				if (MathUtils.Intersect(val4, bounds2))
				{
					xz += offset1;
					float2 val5 = MathUtils.Center(quad2);
					MathUtils.Distance(xz, val5, ref num3);
					float2 val6 = MathUtils.Position(xz, num3);
					float2 val7 = MathUtils.Right(math.normalizesafe(MathUtils.Tangent(xz, num3), default(float2)));
					if (math.cmax(math.abs(new float4(math.dot(val7, quad2.a - val6), math.dot(val7, quad2.b - val6), math.dot(val7, quad2.c - val6), math.dot(val7, quad2.d - val6)))) < num)
					{
						return false;
					}
				}
			}
		}
		bool result = false;
		Quad2 val8 = default(Quad2);
		val8.a = ((float3)(ref segment1.m_Left.a)).xz;
		val8.b = ((float3)(ref segment1.m_Right.a)).xz;
		Bounds2 val9 = MathUtils.Bounds(val8.a, val8.b);
		Bounds2 val15 = default(Bounds2);
		for (int j = 1; j <= 8; j++)
		{
			float num4 = (float)j / 8f;
			float3 val10 = MathUtils.Position(segment1.m_Left, num4);
			val8.d = ((float3)(ref val10)).xz;
			val10 = MathUtils.Position(segment1.m_Right, num4);
			val8.c = ((float3)(ref val10)).xz;
			Bounds2 val11 = MathUtils.Bounds(val8.d, val8.c);
			if (MathUtils.Intersect(val9 | val11, bounds2))
			{
				Quad2 val12 = val8;
				float2 val13 = math.normalizesafe(val12.b - val12.a, default(float2)) * 0.5f;
				float2 val14 = math.normalizesafe(val12.d - val12.c, default(float2)) * 0.5f;
				ref float2 a = ref val12.a;
				a += val13;
				ref float2 b = ref val12.b;
				b -= val13;
				ref float2 c = ref val12.c;
				c += val14;
				ref float2 d = ref val12.d;
				d -= val14;
				ref float2 a2 = ref val12.a;
				a2 += offset1;
				ref float2 b2 = ref val12.b;
				b2 += offset1;
				ref float2 c2 = ref val12.c;
				c2 += offset1;
				ref float2 d2 = ref val12.d;
				d2 += offset1;
				if (MathUtils.Intersect(val12, quad2, ref val15))
				{
					result = true;
					intersection |= val15;
				}
			}
			val8.a = val8.d;
			val8.b = val8.c;
			val9 = val11;
		}
		return result;
	}

	public static bool Intersect(Edge edge1, Entity node2, EdgeGeometry edgeGeometry1, Triangle2 triangle2, Bounds3 bounds2, NetCompositionData prefabCompositionData1, ref Bounds3 intersection)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(((Bounds3)(ref edgeGeometry1.m_Bounds)).xz, ((Bounds3)(ref bounds2)).xz))
		{
			return false;
		}
		if (math.all(edgeGeometry1.m_Start.m_Length + edgeGeometry1.m_End.m_Length <= 0.1f))
		{
			return false;
		}
		bool flag = false;
		if (edge1.m_Start != node2)
		{
			flag |= Intersect(edgeGeometry1.m_Start, new float2(0f, 1f), triangle2, bounds2, prefabCompositionData1, ref intersection);
		}
		if (edge1.m_End != node2)
		{
			flag |= Intersect(edgeGeometry1.m_End, new float2(0f, 1f), triangle2, bounds2, prefabCompositionData1, ref intersection);
		}
		return flag;
	}

	public static bool Intersect(Edge edge1, Entity node2, EdgeGeometry edgeGeometry1, Triangle3 triangle2, NetCompositionData prefabCompositionData1, Bounds1 heightRange2, ref Bounds3 intersection)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 val = SetHeightRange(MathUtils.Bounds(triangle2), heightRange2);
		if (!MathUtils.Intersect(edgeGeometry1.m_Bounds, val))
		{
			return false;
		}
		if (math.all(edgeGeometry1.m_Start.m_Length + edgeGeometry1.m_End.m_Length <= 0.1f))
		{
			return false;
		}
		bool flag = false;
		if (edge1.m_Start != node2)
		{
			flag |= Intersect(edgeGeometry1.m_Start, new float2(0f, 1f), triangle2, prefabCompositionData1, heightRange2, ref intersection);
		}
		if (edge1.m_End != node2)
		{
			flag |= Intersect(edgeGeometry1.m_End, new float2(0f, 1f), triangle2, prefabCompositionData1, heightRange2, ref intersection);
		}
		return flag;
	}

	public static bool Intersect(Edge edge1, Entity node2, EdgeGeometry edgeGeometry1, Segment line2, Bounds3 bounds2, NetCompositionData prefabCompositionData1, ref Bounds3 intersection)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(((Bounds3)(ref edgeGeometry1.m_Bounds)).xz, ((Bounds3)(ref bounds2)).xz))
		{
			return false;
		}
		if (math.all(edgeGeometry1.m_Start.m_Length + edgeGeometry1.m_End.m_Length <= 0.1f))
		{
			return false;
		}
		bool flag = false;
		if (edge1.m_Start != node2)
		{
			flag |= Intersect(edgeGeometry1.m_Start, new float2(0f, 1f), line2, bounds2, prefabCompositionData1, ref intersection);
		}
		if (edge1.m_End != node2)
		{
			flag |= Intersect(edgeGeometry1.m_End, new float2(0f, 1f), line2, bounds2, prefabCompositionData1, ref intersection);
		}
		return flag;
	}

	public static bool Intersect(Entity node1, Entity node2, EdgeNodeGeometry nodeGeometry1, Triangle2 triangle2, Bounds3 bounds2, NetCompositionData prefabCompositionData1, ref Bounds3 intersection)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(((Bounds3)(ref nodeGeometry1.m_Bounds)).xz, ((Bounds3)(ref bounds2)).xz))
		{
			return false;
		}
		if (node1 == node2)
		{
			return false;
		}
		if (math.all(nodeGeometry1.m_Left.m_Length <= 0.05f) && math.all(nodeGeometry1.m_Left.m_Length <= 0.05f))
		{
			return false;
		}
		if (nodeGeometry1.m_MiddleRadius > 0f)
		{
			Segment right = nodeGeometry1.m_Right;
			Segment right2 = nodeGeometry1.m_Right;
			right.m_Right = MathUtils.Lerp(nodeGeometry1.m_Right.m_Left, nodeGeometry1.m_Right.m_Right, 0.5f);
			right.m_Right.d = nodeGeometry1.m_Middle.d;
			right2.m_Left = right.m_Right;
			return Intersect(nodeGeometry1.m_Left, new float2(0f, 1f), triangle2, bounds2, prefabCompositionData1, ref intersection) | Intersect(right, new float2(0f, 0.5f), triangle2, bounds2, prefabCompositionData1, ref intersection) | Intersect(right2, new float2(0.5f, 1f), triangle2, bounds2, prefabCompositionData1, ref intersection);
		}
		Segment left = nodeGeometry1.m_Left;
		Segment right3 = nodeGeometry1.m_Right;
		left.m_Right = nodeGeometry1.m_Middle;
		right3.m_Left = nodeGeometry1.m_Middle;
		return Intersect(nodeGeometry1.m_Left, new float2(0f, 0.5f), triangle2, bounds2, prefabCompositionData1, ref intersection) | Intersect(nodeGeometry1.m_Right, new float2(0.5f, 1f), triangle2, bounds2, prefabCompositionData1, ref intersection) | Intersect(left, new float2(0f, 0.5f), triangle2, bounds2, prefabCompositionData1, ref intersection) | Intersect(right3, new float2(0.5f, 1f), triangle2, bounds2, prefabCompositionData1, ref intersection);
	}

	public static bool Intersect(Entity node1, Entity node2, EdgeNodeGeometry nodeGeometry1, Triangle3 triangle2, NetCompositionData prefabCompositionData1, Bounds1 heightRange2, ref Bounds3 intersection)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 val = SetHeightRange(MathUtils.Bounds(triangle2), heightRange2);
		if (!MathUtils.Intersect(nodeGeometry1.m_Bounds, val))
		{
			return false;
		}
		if (node1 == node2)
		{
			return false;
		}
		if (math.all(nodeGeometry1.m_Left.m_Length <= 0.05f) && math.all(nodeGeometry1.m_Left.m_Length <= 0.05f))
		{
			return false;
		}
		if (nodeGeometry1.m_MiddleRadius > 0f)
		{
			Segment right = nodeGeometry1.m_Right;
			Segment right2 = nodeGeometry1.m_Right;
			right.m_Right = MathUtils.Lerp(nodeGeometry1.m_Right.m_Left, nodeGeometry1.m_Right.m_Right, 0.5f);
			right.m_Right.d = nodeGeometry1.m_Middle.d;
			right2.m_Left = right.m_Right;
			return Intersect(nodeGeometry1.m_Left, new float2(0f, 1f), triangle2, prefabCompositionData1, heightRange2, ref intersection) | Intersect(right, new float2(0f, 0.5f), triangle2, prefabCompositionData1, heightRange2, ref intersection) | Intersect(right2, new float2(0.5f, 1f), triangle2, prefabCompositionData1, heightRange2, ref intersection);
		}
		Segment left = nodeGeometry1.m_Left;
		Segment right3 = nodeGeometry1.m_Right;
		left.m_Right = nodeGeometry1.m_Middle;
		right3.m_Left = nodeGeometry1.m_Middle;
		return Intersect(nodeGeometry1.m_Left, new float2(0f, 0.5f), triangle2, prefabCompositionData1, heightRange2, ref intersection) | Intersect(nodeGeometry1.m_Right, new float2(0.5f, 1f), triangle2, prefabCompositionData1, heightRange2, ref intersection) | Intersect(left, new float2(0f, 0.5f), triangle2, prefabCompositionData1, heightRange2, ref intersection) | Intersect(right3, new float2(0.5f, 1f), triangle2, prefabCompositionData1, heightRange2, ref intersection);
	}

	public static bool Intersect(Entity node1, Entity node2, EdgeNodeGeometry nodeGeometry1, Segment line2, Bounds3 bounds2, NetCompositionData prefabCompositionData1, ref Bounds3 intersection)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(((Bounds3)(ref nodeGeometry1.m_Bounds)).xz, ((Bounds3)(ref bounds2)).xz))
		{
			return false;
		}
		if (node1 == node2)
		{
			return false;
		}
		if (math.all(nodeGeometry1.m_Left.m_Length <= 0.05f) && math.all(nodeGeometry1.m_Left.m_Length <= 0.05f))
		{
			return false;
		}
		if (nodeGeometry1.m_MiddleRadius > 0f)
		{
			Segment right = nodeGeometry1.m_Right;
			Segment right2 = nodeGeometry1.m_Right;
			right.m_Right = MathUtils.Lerp(nodeGeometry1.m_Right.m_Left, nodeGeometry1.m_Right.m_Right, 0.5f);
			right.m_Right.d = nodeGeometry1.m_Middle.d;
			right2.m_Left = right.m_Right;
			return Intersect(nodeGeometry1.m_Left, new float2(0f, 1f), line2, bounds2, prefabCompositionData1, ref intersection) | Intersect(right, new float2(0f, 0.5f), line2, bounds2, prefabCompositionData1, ref intersection) | Intersect(right2, new float2(0.5f, 1f), line2, bounds2, prefabCompositionData1, ref intersection);
		}
		Segment left = nodeGeometry1.m_Left;
		Segment right3 = nodeGeometry1.m_Right;
		left.m_Right = nodeGeometry1.m_Middle;
		right3.m_Left = nodeGeometry1.m_Middle;
		return Intersect(nodeGeometry1.m_Left, new float2(0f, 0.5f), line2, bounds2, prefabCompositionData1, ref intersection) | Intersect(nodeGeometry1.m_Right, new float2(0.5f, 1f), line2, bounds2, prefabCompositionData1, ref intersection) | Intersect(left, new float2(0f, 0.5f), line2, bounds2, prefabCompositionData1, ref intersection) | Intersect(right3, new float2(0.5f, 1f), line2, bounds2, prefabCompositionData1, ref intersection);
	}

	public static bool Intersect(Segment segment1, float2 segmentSide1, Triangle2 triangle2, Bounds3 bounds2, NetCompositionData prefabCompositionData1, ref Bounds3 intersection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 val = SetHeightRange(MathUtils.Bounds(segment1.m_Left) | MathUtils.Bounds(segment1.m_Right), prefabCompositionData1.m_HeightRange);
		if (!MathUtils.Intersect(((Bounds3)(ref val)).xz, ((Bounds3)(ref bounds2)).xz))
		{
			return false;
		}
		bool result = false;
		Quad3 val2 = default(Quad3);
		val2.a = segment1.m_Left.a;
		val2.b = segment1.m_Right.a;
		Bounds3 val3 = SetHeightRange(MathUtils.Bounds(val2.a, val2.b), prefabCompositionData1.m_HeightRange);
		for (int i = 1; i <= 8; i++)
		{
			float num = (float)i / 8f;
			val2.d = MathUtils.Position(segment1.m_Left, num);
			val2.c = MathUtils.Position(segment1.m_Right, num);
			Bounds3 val4 = SetHeightRange(MathUtils.Bounds(val2.d, val2.c), prefabCompositionData1.m_HeightRange);
			Bounds3 val5 = val3 | val4;
			if (MathUtils.Intersect(((Bounds3)(ref val5)).xz, ((Bounds3)(ref bounds2)).xz))
			{
				Quad2 xz = ((Quad3)(ref val2)).xz;
				float2 val6 = math.normalizesafe(xz.b - xz.a, default(float2)) * 0.5f;
				float2 val7 = math.normalizesafe(xz.d - xz.c, default(float2)) * 0.5f;
				ref float2 a = ref xz.a;
				a += val6;
				ref float2 b = ref xz.b;
				b -= val6;
				ref float2 c = ref xz.c;
				c += val7;
				ref float2 d = ref xz.d;
				d -= val7;
				if (MathUtils.Intersect(xz, triangle2))
				{
					result = true;
					intersection |= val5 & bounds2;
				}
			}
			val2.a = val2.d;
			val2.b = val2.c;
			val3 = val4;
		}
		return result;
	}

	public static bool Intersect(Segment segment1, float2 segmentSide1, Segment line2, Bounds3 bounds2, NetCompositionData prefabCompositionData1, ref Bounds3 intersection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 val = SetHeightRange(MathUtils.Bounds(segment1.m_Left) | MathUtils.Bounds(segment1.m_Right), prefabCompositionData1.m_HeightRange);
		if (!MathUtils.Intersect(((Bounds3)(ref val)).xz, ((Bounds3)(ref bounds2)).xz))
		{
			return false;
		}
		bool result = false;
		Quad3 val2 = default(Quad3);
		val2.a = segment1.m_Left.a;
		val2.b = segment1.m_Right.a;
		Bounds3 val3 = SetHeightRange(MathUtils.Bounds(val2.a, val2.b), prefabCompositionData1.m_HeightRange);
		for (int i = 1; i <= 8; i++)
		{
			float num = (float)i / 8f;
			val2.d = MathUtils.Position(segment1.m_Left, num);
			val2.c = MathUtils.Position(segment1.m_Right, num);
			Bounds3 val4 = SetHeightRange(MathUtils.Bounds(val2.d, val2.c), prefabCompositionData1.m_HeightRange);
			Bounds3 val5 = val3 | val4;
			if (MathUtils.Intersect(((Bounds3)(ref val5)).xz, ((Bounds3)(ref bounds2)).xz))
			{
				Quad2 xz = ((Quad3)(ref val2)).xz;
				float2 val6 = math.normalizesafe(xz.b - xz.a, default(float2)) * 0.5f;
				float2 val7 = xz.d - xz.c;
				float2 val8 = default(float2);
				float2 val9 = math.normalizesafe(val7, val8) * 0.5f;
				ref float2 a = ref xz.a;
				a += val6;
				ref float2 b = ref xz.b;
				b -= val6;
				ref float2 c = ref xz.c;
				c += val9;
				ref float2 d = ref xz.d;
				d -= val9;
				if (MathUtils.Intersect(xz, line2, ref val8))
				{
					result = true;
					intersection |= val5 & bounds2;
				}
			}
			val2.a = val2.d;
			val2.b = val2.c;
			val3 = val4;
		}
		return result;
	}

	public static bool Intersect(Edge edge1, Entity node2, EdgeGeometry edgeGeometry1, float3 offset1, Cylinder3 cylinder2, Bounds3 bounds2, NetCompositionData prefabCompositionData1, DynamicBuffer<NetCompositionArea> areas1, ref Bounds3 intersection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(edgeGeometry1.m_Bounds, bounds2))
		{
			return false;
		}
		if (math.all(edgeGeometry1.m_Start.m_Length + edgeGeometry1.m_End.m_Length <= 0.1f))
		{
			return false;
		}
		bool flag = false;
		if (edge1.m_Start != node2 || (prefabCompositionData1.m_State & CompositionState.HasSurface) != 0)
		{
			flag |= Intersect(edgeGeometry1.m_Start, new float2(0f, 1f), offset1, cylinder2, bounds2, prefabCompositionData1, areas1, ref intersection);
		}
		if (edge1.m_End != node2 || (prefabCompositionData1.m_State & CompositionState.HasSurface) != 0)
		{
			flag |= Intersect(edgeGeometry1.m_End, new float2(0f, 1f), offset1, cylinder2, bounds2, prefabCompositionData1, areas1, ref intersection);
		}
		return flag;
	}

	public static bool Intersect(Edge edge1, Entity node2, EdgeGeometry edgeGeometry1, float2 offset1, Circle2 circle2, Bounds2 bounds2, NetCompositionData prefabCompositionData1, DynamicBuffer<NetCompositionArea> areas1, ref Bounds2 intersection)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(((Bounds3)(ref edgeGeometry1.m_Bounds)).xz, bounds2))
		{
			return false;
		}
		if (math.all(edgeGeometry1.m_Start.m_Length + edgeGeometry1.m_End.m_Length <= 0.1f))
		{
			return false;
		}
		bool flag = false;
		if (edge1.m_Start != node2 || (prefabCompositionData1.m_State & CompositionState.HasSurface) != 0)
		{
			flag |= Intersect(edgeGeometry1.m_Start, new float2(0f, 1f), offset1, circle2, bounds2, prefabCompositionData1, areas1, ref intersection);
		}
		if (edge1.m_End != node2 || (prefabCompositionData1.m_State & CompositionState.HasSurface) != 0)
		{
			flag |= Intersect(edgeGeometry1.m_End, new float2(0f, 1f), offset1, circle2, bounds2, prefabCompositionData1, areas1, ref intersection);
		}
		return flag;
	}

	public static bool Intersect(Entity node1, Entity node2, EdgeNodeGeometry nodeGeometry1, float3 offset1, Cylinder3 cylinder2, Bounds3 bounds2, NetCompositionData prefabCompositionData1, DynamicBuffer<NetCompositionArea> areas1, ref Bounds3 intersection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(nodeGeometry1.m_Bounds, bounds2))
		{
			return false;
		}
		if (node1 == node2)
		{
			return false;
		}
		if (math.all(nodeGeometry1.m_Left.m_Length <= 0.05f) && math.all(nodeGeometry1.m_Left.m_Length <= 0.05f))
		{
			return false;
		}
		if (nodeGeometry1.m_MiddleRadius > 0f)
		{
			Segment right = nodeGeometry1.m_Right;
			Segment right2 = nodeGeometry1.m_Right;
			right.m_Right = MathUtils.Lerp(nodeGeometry1.m_Right.m_Left, nodeGeometry1.m_Right.m_Right, 0.5f);
			right.m_Right.d = nodeGeometry1.m_Middle.d;
			right2.m_Left = right.m_Right;
			return Intersect(nodeGeometry1.m_Left, new float2(0f, 1f), offset1, cylinder2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(right, new float2(0f, 0.5f), offset1, cylinder2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(right2, new float2(0.5f, 1f), offset1, cylinder2, bounds2, prefabCompositionData1, areas1, ref intersection);
		}
		Segment left = nodeGeometry1.m_Left;
		Segment right3 = nodeGeometry1.m_Right;
		left.m_Right = nodeGeometry1.m_Middle;
		right3.m_Left = nodeGeometry1.m_Middle;
		return Intersect(nodeGeometry1.m_Left, new float2(0f, 0.5f), offset1, cylinder2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(nodeGeometry1.m_Right, new float2(0.5f, 1f), offset1, cylinder2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(left, new float2(0f, 0.5f), offset1, cylinder2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(right3, new float2(0.5f, 1f), offset1, cylinder2, bounds2, prefabCompositionData1, areas1, ref intersection);
	}

	public static bool Intersect(Entity node1, Entity node2, EdgeNodeGeometry nodeGeometry1, float2 offset1, Circle2 circle2, Bounds2 bounds2, NetCompositionData prefabCompositionData1, DynamicBuffer<NetCompositionArea> areas1, ref Bounds2 intersection)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(((Bounds3)(ref nodeGeometry1.m_Bounds)).xz, bounds2))
		{
			return false;
		}
		if (node1 == node2)
		{
			return false;
		}
		if (math.all(nodeGeometry1.m_Left.m_Length <= 0.05f) && math.all(nodeGeometry1.m_Left.m_Length <= 0.05f))
		{
			return false;
		}
		if (nodeGeometry1.m_MiddleRadius > 0f)
		{
			Segment right = nodeGeometry1.m_Right;
			Segment right2 = nodeGeometry1.m_Right;
			right.m_Right = MathUtils.Lerp(nodeGeometry1.m_Right.m_Left, nodeGeometry1.m_Right.m_Right, 0.5f);
			right.m_Right.d = nodeGeometry1.m_Middle.d;
			right2.m_Left = right.m_Right;
			return Intersect(nodeGeometry1.m_Left, new float2(0f, 1f), offset1, circle2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(right, new float2(0f, 0.5f), offset1, circle2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(right2, new float2(0.5f, 1f), offset1, circle2, bounds2, prefabCompositionData1, areas1, ref intersection);
		}
		Segment left = nodeGeometry1.m_Left;
		Segment right3 = nodeGeometry1.m_Right;
		left.m_Right = nodeGeometry1.m_Middle;
		right3.m_Left = nodeGeometry1.m_Middle;
		return Intersect(nodeGeometry1.m_Left, new float2(0f, 0.5f), offset1, circle2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(nodeGeometry1.m_Right, new float2(0.5f, 1f), offset1, circle2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(left, new float2(0f, 0.5f), offset1, circle2, bounds2, prefabCompositionData1, areas1, ref intersection) | Intersect(right3, new float2(0.5f, 1f), offset1, circle2, bounds2, prefabCompositionData1, areas1, ref intersection);
	}

	public static bool Intersect(Segment segment1, float2 segmentSide1, float3 offset1, Cylinder3 cylinder2, Bounds3 bounds2, NetCompositionData prefabCompositionData1, DynamicBuffer<NetCompositionArea> areas1, ref Bounds3 intersection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(SetHeightRange(MathUtils.Bounds(segment1.m_Left) | MathUtils.Bounds(segment1.m_Right), prefabCompositionData1.m_HeightRange), bounds2))
		{
			return false;
		}
		if (areas1.IsCreated)
		{
			float num4 = default(float);
			for (int i = 0; i < areas1.Length; i++)
			{
				NetCompositionArea netCompositionArea = areas1[i];
				if ((netCompositionArea.m_Flags & (NetAreaFlags.Buildable | NetAreaFlags.Hole)) == 0)
				{
					continue;
				}
				float num = netCompositionArea.m_Width * 0.51f;
				if (cylinder2.circle.radius >= num)
				{
					continue;
				}
				float num2 = netCompositionArea.m_Position.x / prefabCompositionData1.m_Width + 0.5f;
				if (num2 < segmentSide1.x || num2 > segmentSide1.y)
				{
					continue;
				}
				Bezier4x3 val = MathUtils.Lerp(segment1.m_Left, segment1.m_Right, (num2 - segmentSide1.x) / (segmentSide1.y - segmentSide1.x));
				Bounds3 bounds3 = MathUtils.Bounds(val);
				ref float3 min = ref bounds3.min;
				((float3)(ref min)).xz = ((float3)(ref min)).xz - num;
				ref float3 max = ref bounds3.max;
				((float3)(ref max)).xz = ((float3)(ref max)).xz + num;
				if (!MathUtils.Intersect(SetHeightRange(bounds3, prefabCompositionData1.m_HeightRange), bounds2))
				{
					continue;
				}
				val += offset1;
				float3 val2 = math.mul(cylinder2.rotation, new float3(cylinder2.circle.position.x, cylinder2.height.min, cylinder2.circle.position.y));
				float num3 = MathUtils.Distance(((Bezier4x3)(ref val)).xz, ((float3)(ref val2)).xz, ref num4);
				if ((netCompositionArea.m_Flags & NetAreaFlags.Hole) == 0)
				{
					float3 val3 = MathUtils.Position(val, num4);
					if (bounds2.min.y + offset1.y <= val3.y + prefabCompositionData1.m_HeightRange.min)
					{
						continue;
					}
				}
				float3 val4;
				if (num4 == 0f)
				{
					val4 = MathUtils.StartTangent(val);
					float num5 = math.dot(math.normalizesafe(((float3)(ref val4)).xz, default(float2)), ((float3)(ref val.a)).xz - ((float3)(ref val2)).xz);
					if (!(num5 >= cylinder2.circle.radius) && num * num - 2f * num * math.sqrt(math.max(0f, num3 * num3 - num5 * num5)) + num3 * num3 > cylinder2.circle.radius * cylinder2.circle.radius)
					{
						return false;
					}
				}
				else if (num4 == 1f)
				{
					val4 = MathUtils.EndTangent(val);
					float num6 = math.dot(math.normalizesafe(((float3)(ref val4)).xz, default(float2)), ((float3)(ref val2)).xz - ((float3)(ref val.d)).xz);
					if (!(num6 >= cylinder2.circle.radius) && num * num - 2f * num * math.sqrt(math.max(0f, num3 * num3 - num6 * num6)) + num3 * num3 > cylinder2.circle.radius * cylinder2.circle.radius)
					{
						return false;
					}
				}
				else if (num3 < num - cylinder2.circle.radius)
				{
					return false;
				}
			}
		}
		bool result = false;
		Quad3 val5 = default(Quad3);
		val5.a = segment1.m_Left.a;
		val5.b = segment1.m_Right.a;
		Bounds3 val6 = SetHeightRange(MathUtils.Bounds(val5.a, val5.b), prefabCompositionData1.m_HeightRange);
		Bounds3 val8 = default(Bounds3);
		for (int j = 1; j <= 8; j++)
		{
			float num7 = (float)j / 8f;
			val5.d = MathUtils.Position(segment1.m_Left, num7);
			val5.c = MathUtils.Position(segment1.m_Right, num7);
			Bounds3 val7 = SetHeightRange(MathUtils.Bounds(val5.d, val5.c), prefabCompositionData1.m_HeightRange);
			if (MathUtils.Intersect(val6 | val7, bounds2))
			{
				Quad3 quad = val5;
				ref float3 a = ref quad.a;
				a += offset1;
				ref float3 b = ref quad.b;
				b += offset1;
				ref float3 c = ref quad.c;
				c += offset1;
				ref float3 d = ref quad.d;
				d += offset1;
				if (QuadCylinderIntersect(quad, cylinder2, out var intersection2, out var intersection3))
				{
					intersection2 = SetHeightRange(intersection2, prefabCompositionData1.m_HeightRange);
					if (MathUtils.Intersect(intersection2, intersection3, ref val8))
					{
						result = true;
						intersection |= val8;
					}
				}
			}
			val5.a = val5.d;
			val5.b = val5.c;
			val6 = val7;
		}
		return result;
	}

	public static bool Intersect(Segment segment1, float2 segmentSide1, float2 offset1, Circle2 circle2, Bounds2 bounds2, NetCompositionData prefabCompositionData1, DynamicBuffer<NetCompositionArea> areas1, ref Bounds2 intersection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 val = MathUtils.Bounds(segment1.m_Left) | MathUtils.Bounds(segment1.m_Right);
		if (!MathUtils.Intersect(((Bounds3)(ref val)).xz, bounds2))
		{
			return false;
		}
		if (areas1.IsCreated)
		{
			float num4 = default(float);
			for (int i = 0; i < areas1.Length; i++)
			{
				NetCompositionArea netCompositionArea = areas1[i];
				if ((netCompositionArea.m_Flags & (NetAreaFlags.Buildable | NetAreaFlags.Hole)) == 0)
				{
					continue;
				}
				float num = netCompositionArea.m_Width * 0.51f;
				if (circle2.radius >= num)
				{
					continue;
				}
				float num2 = netCompositionArea.m_Position.x / prefabCompositionData1.m_Width + 0.5f;
				if (num2 < segmentSide1.x || num2 > segmentSide1.y)
				{
					continue;
				}
				Bezier4x3 val2 = MathUtils.Lerp(segment1.m_Left, segment1.m_Right, (num2 - segmentSide1.x) / (segmentSide1.y - segmentSide1.x));
				Bezier4x2 xz = ((Bezier4x3)(ref val2)).xz;
				Bounds2 val3 = MathUtils.Bounds(xz);
				ref float2 min = ref val3.min;
				min -= num;
				ref float2 max = ref val3.max;
				max += num;
				if (!MathUtils.Intersect(val3, bounds2))
				{
					continue;
				}
				xz += offset1;
				float num3 = MathUtils.Distance(xz, circle2.position, ref num4);
				if (num4 == 0f)
				{
					float num5 = math.dot(math.normalizesafe(MathUtils.StartTangent(xz), default(float2)), xz.a - circle2.position);
					if (!(num5 >= circle2.radius) && num * num - 2f * num * math.sqrt(math.max(0f, num3 * num3 - num5 * num5)) + num3 * num3 > circle2.radius * circle2.radius)
					{
						return false;
					}
				}
				else if (num4 == 1f)
				{
					float num6 = math.dot(math.normalizesafe(MathUtils.EndTangent(xz), default(float2)), circle2.position - xz.d);
					if (!(num6 >= circle2.radius) && num * num - 2f * num * math.sqrt(math.max(0f, num3 * num3 - num6 * num6)) + num3 * num3 > circle2.radius * circle2.radius)
					{
						return false;
					}
				}
				else if (num3 < num - circle2.radius)
				{
					return false;
				}
			}
		}
		bool result = false;
		Quad2 val4 = default(Quad2);
		val4.a = ((float3)(ref segment1.m_Left.a)).xz;
		val4.b = ((float3)(ref segment1.m_Right.a)).xz;
		Bounds2 val5 = MathUtils.Bounds(val4.a, val4.b);
		Bounds2 val9 = default(Bounds2);
		for (int j = 1; j <= 8; j++)
		{
			float num7 = (float)j / 8f;
			float3 val6 = MathUtils.Position(segment1.m_Left, num7);
			val4.d = ((float3)(ref val6)).xz;
			val6 = MathUtils.Position(segment1.m_Right, num7);
			val4.c = ((float3)(ref val6)).xz;
			Bounds2 val7 = MathUtils.Bounds(val4.d, val4.c);
			if (MathUtils.Intersect(val5 | val7, bounds2))
			{
				Quad2 val8 = val4;
				ref float2 a = ref val8.a;
				a += offset1;
				ref float2 b = ref val8.b;
				b += offset1;
				ref float2 c = ref val8.c;
				c += offset1;
				ref float2 d = ref val8.d;
				d += offset1;
				if (MathUtils.Intersect(val8, circle2, ref val9))
				{
					result = true;
					intersection |= val9;
				}
			}
			val4.a = val4.d;
			val4.b = val4.c;
			val5 = val7;
		}
		return result;
	}

	public static bool QuadCylinderIntersect(Quad3 quad1, Cylinder3 cylinder2, out Bounds3 intersection1, out Bounds3 intersection2)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		intersection1.min = float3.op_Implicit(float.MaxValue);
		intersection1.max = float3.op_Implicit(float.MinValue);
		intersection2.min = float3.op_Implicit(float.MaxValue);
		intersection2.max = float3.op_Implicit(float.MinValue);
		Segment line = new Segment(quad1.a, quad1.b);
		Segment line2 = default(Segment);
		((Segment)(ref line2))._002Ector(quad1.b, quad1.c);
		Segment line3 = default(Segment);
		((Segment)(ref line3))._002Ector(quad1.c, quad1.d);
		Segment line4 = default(Segment);
		((Segment)(ref line4))._002Ector(quad1.d, quad1.a);
		float3 val = math.mul(cylinder2.rotation, new float3(cylinder2.circle.position.x, cylinder2.height.min, cylinder2.circle.position.y));
		float3 val2 = math.mul(cylinder2.rotation, new float3(cylinder2.circle.position.x, cylinder2.height.max, cylinder2.circle.position.y));
		Circle2 circle = cylinder2.circle;
		circle.position = ((float3)(ref val)).xz;
		Bounds1 val3 = MathUtils.Bounds(val.y, val2.y);
		Line3 line5 = default(Line3);
		line5.a = new float3(circle.position.x, val3.min, circle.position.y);
		line5.b = new float3(circle.position.x, val3.max, circle.position.y);
		return QuadCylinderIntersectHelper(line, circle, val3, ref intersection1, ref intersection2) | QuadCylinderIntersectHelper(line2, circle, val3, ref intersection1, ref intersection2) | QuadCylinderIntersectHelper(line3, circle, val3, ref intersection1, ref intersection2) | QuadCylinderIntersectHelper(line4, circle, val3, ref intersection1, ref intersection2) | QuadCylinderIntersectHelper(quad1, line5, ref intersection1, ref intersection2);
	}

	public static bool TriangleCylinderIntersect(Triangle3 triangle1, Cylinder3 cylinder2, out Bounds3 intersection1, out Bounds3 intersection2)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		intersection1.min = float3.op_Implicit(float.MaxValue);
		intersection1.max = float3.op_Implicit(float.MinValue);
		intersection2.min = float3.op_Implicit(float.MaxValue);
		intersection2.max = float3.op_Implicit(float.MinValue);
		Segment line = new Segment(triangle1.a, triangle1.b);
		Segment line2 = default(Segment);
		((Segment)(ref line2))._002Ector(triangle1.b, triangle1.c);
		Segment line3 = default(Segment);
		((Segment)(ref line3))._002Ector(triangle1.c, triangle1.a);
		float3 val = math.mul(cylinder2.rotation, new float3(cylinder2.circle.position.x, cylinder2.height.min, cylinder2.circle.position.y));
		float3 val2 = math.mul(cylinder2.rotation, new float3(cylinder2.circle.position.x, cylinder2.height.max, cylinder2.circle.position.y));
		Circle2 circle = cylinder2.circle;
		circle.position = ((float3)(ref val)).xz;
		Bounds1 val3 = MathUtils.Bounds(val.y, val2.y);
		Line3 line4 = default(Line3);
		line4.a = new float3(circle.position.x, val3.min, circle.position.y);
		line4.b = new float3(circle.position.x, val3.max, circle.position.y);
		return QuadCylinderIntersectHelper(line, circle, val3, ref intersection1, ref intersection2) | QuadCylinderIntersectHelper(line2, circle, val3, ref intersection1, ref intersection2) | QuadCylinderIntersectHelper(line3, circle, val3, ref intersection1, ref intersection2) | TriangleCylinderIntersectHelper(triangle1, line4, ref intersection1, ref intersection2);
	}

	private static bool QuadCylinderIntersectHelper(Segment line1, Circle2 circle2, Bounds1 height2, ref Bounds3 intersection1, ref Bounds3 intersection2)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		float2 val = default(float2);
		if (MathUtils.Intersect(circle2, ((Segment)(ref line1)).xz, ref val))
		{
			float3 val2 = MathUtils.Position(line1, val.x);
			float3 val3 = MathUtils.Position(line1, val.y);
			intersection1 |= val2;
			intersection1 |= val3;
			val2.y = height2.min;
			val3.y = height2.max;
			intersection2 |= val2;
			intersection2 |= val3;
			return true;
		}
		return false;
	}

	private static bool QuadCylinderIntersectHelper(Quad3 quad1, Line3 line2, ref Bounds3 intersection1, ref Bounds3 intersection2)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		float num = default(float);
		if (MathUtils.Intersect(quad1, line2, ref num))
		{
			intersection1 |= MathUtils.Position(line2, num);
			intersection2 |= MathUtils.Position(line2, math.saturate(num));
			return true;
		}
		return false;
	}

	private static bool TriangleCylinderIntersectHelper(Triangle3 triangle1, Line3 line2, ref Bounds3 intersection1, ref Bounds3 intersection2)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		float3 val = default(float3);
		if (MathUtils.Intersect(triangle1, line2, ref val))
		{
			intersection1 |= MathUtils.Position(line2, val.z);
			intersection2 |= MathUtils.Position(line2, math.saturate(val.z));
			return true;
		}
		return false;
	}

	public static bool Intersect(Edge edge1, Edge edge2, EdgeGeometry edgeGeometry1, EdgeGeometry edgeGeometry2, NetCompositionData prefabCompositionData1, NetCompositionData prefabCompositionData2, ref Bounds3 intersection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(edgeGeometry1.m_Bounds, edgeGeometry2.m_Bounds))
		{
			return false;
		}
		if (math.all(edgeGeometry1.m_Start.m_Length + edgeGeometry1.m_End.m_Length <= 0.1f))
		{
			return false;
		}
		if (math.all(edgeGeometry2.m_Start.m_Length + edgeGeometry2.m_End.m_Length <= 0.1f))
		{
			return false;
		}
		bool flag = false;
		if (edge1.m_Start != edge2.m_Start)
		{
			flag |= Intersect(edgeGeometry1.m_Start, edgeGeometry2.m_Start, prefabCompositionData1, prefabCompositionData2, ref intersection);
		}
		if (edge1.m_Start != edge2.m_End)
		{
			flag |= Intersect(edgeGeometry1.m_Start, edgeGeometry2.m_End, prefabCompositionData1, prefabCompositionData2, ref intersection);
		}
		if (edge1.m_End != edge2.m_Start)
		{
			flag |= Intersect(edgeGeometry1.m_End, edgeGeometry2.m_Start, prefabCompositionData1, prefabCompositionData2, ref intersection);
		}
		if (edge1.m_End != edge2.m_End)
		{
			flag |= Intersect(edgeGeometry1.m_End, edgeGeometry2.m_End, prefabCompositionData1, prefabCompositionData2, ref intersection);
		}
		return flag;
	}

	public static bool Intersect(Edge edge1, Edge edge2, EdgeGeometry edgeGeometry1, EdgeGeometry edgeGeometry2, NetCompositionData prefabCompositionData1, NetCompositionData prefabCompositionData2, ref Bounds2 intersection)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(((Bounds3)(ref edgeGeometry1.m_Bounds)).xz, ((Bounds3)(ref edgeGeometry2.m_Bounds)).xz))
		{
			return false;
		}
		if (math.all(edgeGeometry1.m_Start.m_Length + edgeGeometry1.m_End.m_Length <= 0.1f))
		{
			return false;
		}
		if (math.all(edgeGeometry2.m_Start.m_Length + edgeGeometry2.m_End.m_Length <= 0.1f))
		{
			return false;
		}
		bool flag = false;
		if (edge1.m_Start != edge2.m_Start)
		{
			flag |= Intersect(edgeGeometry1.m_Start, edgeGeometry2.m_Start, prefabCompositionData1, prefabCompositionData2, ref intersection);
		}
		if (edge1.m_Start != edge2.m_End)
		{
			flag |= Intersect(edgeGeometry1.m_Start, edgeGeometry2.m_End, prefabCompositionData1, prefabCompositionData2, ref intersection);
		}
		if (edge1.m_End != edge2.m_Start)
		{
			flag |= Intersect(edgeGeometry1.m_End, edgeGeometry2.m_Start, prefabCompositionData1, prefabCompositionData2, ref intersection);
		}
		if (edge1.m_End != edge2.m_End)
		{
			flag |= Intersect(edgeGeometry1.m_End, edgeGeometry2.m_End, prefabCompositionData1, prefabCompositionData2, ref intersection);
		}
		return flag;
	}

	public static bool Intersect(Edge edge1, Edge originalEdge1, NativeArray<ConnectedNode> nodes1, NativeArray<ConnectedNode> originalNodes1, Entity node2, EdgeGeometry edgeGeometry1, EdgeNodeGeometry nodeGeometry2, NetCompositionData prefabCompositionData1, NetCompositionData prefabCompositionData2, ref Bounds3 intersection)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(edgeGeometry1.m_Bounds, nodeGeometry2.m_Bounds))
		{
			return false;
		}
		if (math.all(edgeGeometry1.m_Start.m_Length + edgeGeometry1.m_End.m_Length <= 0.1f))
		{
			return false;
		}
		if (math.all(nodeGeometry2.m_Left.m_Length <= 0.05f) && math.all(nodeGeometry2.m_Left.m_Length <= 0.05f))
		{
			return false;
		}
		for (int i = 0; i < nodes1.Length; i++)
		{
			if (nodes1[i].m_Node == node2)
			{
				return false;
			}
		}
		for (int j = 0; j < originalNodes1.Length; j++)
		{
			if (originalNodes1[j].m_Node == node2)
			{
				return false;
			}
		}
		bool flag = false;
		if (nodeGeometry2.m_MiddleRadius > 0f)
		{
			Segment right = nodeGeometry2.m_Right;
			Segment right2 = nodeGeometry2.m_Right;
			right.m_Right = MathUtils.Lerp(nodeGeometry2.m_Right.m_Left, nodeGeometry2.m_Right.m_Right, 0.5f);
			right.m_Right.d = nodeGeometry2.m_Middle.d;
			right2.m_Left = right.m_Right;
			if (edge1.m_Start != node2 && originalEdge1.m_Start != node2)
			{
				flag |= Intersect(edgeGeometry1.m_Start, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, right2, prefabCompositionData1, prefabCompositionData2, ref intersection);
			}
			if (edge1.m_End != node2 && originalEdge1.m_End != node2)
			{
				flag |= Intersect(edgeGeometry1.m_End, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, right2, prefabCompositionData1, prefabCompositionData2, ref intersection);
			}
		}
		else
		{
			Segment left = nodeGeometry2.m_Left;
			Segment right3 = nodeGeometry2.m_Right;
			left.m_Right = nodeGeometry2.m_Middle;
			right3.m_Left = nodeGeometry2.m_Middle;
			if (edge1.m_Start != node2 && originalEdge1.m_Start != node2)
			{
				flag |= Intersect(edgeGeometry1.m_Start, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, nodeGeometry2.m_Right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, right3, prefabCompositionData1, prefabCompositionData2, ref intersection);
			}
			if (edge1.m_End != node2 && originalEdge1.m_End != node2)
			{
				flag |= Intersect(edgeGeometry1.m_End, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, nodeGeometry2.m_Right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, right3, prefabCompositionData1, prefabCompositionData2, ref intersection);
			}
		}
		return flag;
	}

	public static bool Intersect(Edge edge1, NativeArray<ConnectedNode> nodes1, Entity node2, Entity originalNode2, EdgeGeometry edgeGeometry1, EdgeNodeGeometry nodeGeometry2, NetCompositionData prefabCompositionData1, NetCompositionData prefabCompositionData2, ref Bounds3 intersection)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(edgeGeometry1.m_Bounds, nodeGeometry2.m_Bounds))
		{
			return false;
		}
		if (math.all(edgeGeometry1.m_Start.m_Length + edgeGeometry1.m_End.m_Length <= 0.1f))
		{
			return false;
		}
		if (math.all(nodeGeometry2.m_Left.m_Length <= 0.05f) && math.all(nodeGeometry2.m_Left.m_Length <= 0.05f))
		{
			return false;
		}
		for (int i = 0; i < nodes1.Length; i++)
		{
			Entity node3 = nodes1[i].m_Node;
			if (node3 == node2 || node3 == originalNode2)
			{
				return false;
			}
		}
		bool flag = false;
		if (nodeGeometry2.m_MiddleRadius > 0f)
		{
			Segment right = nodeGeometry2.m_Right;
			Segment right2 = nodeGeometry2.m_Right;
			right.m_Right = MathUtils.Lerp(nodeGeometry2.m_Right.m_Left, nodeGeometry2.m_Right.m_Right, 0.5f);
			right.m_Right.d = nodeGeometry2.m_Middle.d;
			right2.m_Left = right.m_Right;
			if (edge1.m_Start != node2 && edge1.m_Start != originalNode2)
			{
				flag |= Intersect(edgeGeometry1.m_Start, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, right2, prefabCompositionData1, prefabCompositionData2, ref intersection);
			}
			if (edge1.m_End != node2 && edge1.m_End != originalNode2)
			{
				flag |= Intersect(edgeGeometry1.m_End, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, right2, prefabCompositionData1, prefabCompositionData2, ref intersection);
			}
		}
		else
		{
			Segment left = nodeGeometry2.m_Left;
			Segment right3 = nodeGeometry2.m_Right;
			left.m_Right = nodeGeometry2.m_Middle;
			right3.m_Left = nodeGeometry2.m_Middle;
			if (edge1.m_Start != node2 && edge1.m_Start != originalNode2)
			{
				flag |= Intersect(edgeGeometry1.m_Start, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, nodeGeometry2.m_Right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, right3, prefabCompositionData1, prefabCompositionData2, ref intersection);
			}
			if (edge1.m_End != node2 && edge1.m_End != originalNode2)
			{
				flag |= Intersect(edgeGeometry1.m_End, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, nodeGeometry2.m_Right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, right3, prefabCompositionData1, prefabCompositionData2, ref intersection);
			}
		}
		return flag;
	}

	public static bool Intersect(Edge edge1, Edge originalEdge1, NativeArray<ConnectedNode> nodes1, NativeArray<ConnectedNode> originalNodes1, Entity node2, EdgeGeometry edgeGeometry1, EdgeNodeGeometry nodeGeometry2, NetCompositionData prefabCompositionData1, NetCompositionData prefabCompositionData2, ref Bounds2 intersection)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(((Bounds3)(ref edgeGeometry1.m_Bounds)).xz, ((Bounds3)(ref nodeGeometry2.m_Bounds)).xz))
		{
			return false;
		}
		if (math.all(edgeGeometry1.m_Start.m_Length + edgeGeometry1.m_End.m_Length <= 0.1f))
		{
			return false;
		}
		if (math.all(nodeGeometry2.m_Left.m_Length <= 0.05f) && math.all(nodeGeometry2.m_Left.m_Length <= 0.05f))
		{
			return false;
		}
		for (int i = 0; i < nodes1.Length; i++)
		{
			if (nodes1[i].m_Node == node2)
			{
				return false;
			}
		}
		for (int j = 0; j < originalNodes1.Length; j++)
		{
			if (originalNodes1[j].m_Node == node2)
			{
				return false;
			}
		}
		bool flag = false;
		if (nodeGeometry2.m_MiddleRadius > 0f)
		{
			Segment right = nodeGeometry2.m_Right;
			Segment right2 = nodeGeometry2.m_Right;
			right.m_Right = MathUtils.Lerp(nodeGeometry2.m_Right.m_Left, nodeGeometry2.m_Right.m_Right, 0.5f);
			right.m_Right.d = nodeGeometry2.m_Middle.d;
			right2.m_Left = right.m_Right;
			if (edge1.m_Start != node2 && originalEdge1.m_Start != node2)
			{
				flag |= Intersect(edgeGeometry1.m_Start, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, right2, prefabCompositionData1, prefabCompositionData2, ref intersection);
			}
			if (edge1.m_End != node2 && originalEdge1.m_End != node2)
			{
				flag |= Intersect(edgeGeometry1.m_End, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, right2, prefabCompositionData1, prefabCompositionData2, ref intersection);
			}
		}
		else
		{
			Segment left = nodeGeometry2.m_Left;
			Segment right3 = nodeGeometry2.m_Right;
			left.m_Right = nodeGeometry2.m_Middle;
			right3.m_Left = nodeGeometry2.m_Middle;
			if (edge1.m_Start != node2 && originalEdge1.m_Start != node2)
			{
				flag |= Intersect(edgeGeometry1.m_Start, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, nodeGeometry2.m_Right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, right3, prefabCompositionData1, prefabCompositionData2, ref intersection);
			}
			if (edge1.m_End != node2 && originalEdge1.m_End != node2)
			{
				flag |= Intersect(edgeGeometry1.m_End, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, nodeGeometry2.m_Right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, right3, prefabCompositionData1, prefabCompositionData2, ref intersection);
			}
		}
		return flag;
	}

	public static bool Intersect(Edge edge1, NativeArray<ConnectedNode> nodes1, Entity node2, Entity originalNode2, EdgeGeometry edgeGeometry1, EdgeNodeGeometry nodeGeometry2, NetCompositionData prefabCompositionData1, NetCompositionData prefabCompositionData2, ref Bounds2 intersection)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(((Bounds3)(ref edgeGeometry1.m_Bounds)).xz, ((Bounds3)(ref nodeGeometry2.m_Bounds)).xz))
		{
			return false;
		}
		if (math.all(edgeGeometry1.m_Start.m_Length + edgeGeometry1.m_End.m_Length <= 0.1f))
		{
			return false;
		}
		if (math.all(nodeGeometry2.m_Left.m_Length <= 0.05f) && math.all(nodeGeometry2.m_Left.m_Length <= 0.05f))
		{
			return false;
		}
		for (int i = 0; i < nodes1.Length; i++)
		{
			Entity node3 = nodes1[i].m_Node;
			if (node3 == node2 || node3 == originalNode2)
			{
				return false;
			}
		}
		bool flag = false;
		if (nodeGeometry2.m_MiddleRadius > 0f)
		{
			Segment right = nodeGeometry2.m_Right;
			Segment right2 = nodeGeometry2.m_Right;
			right.m_Right = MathUtils.Lerp(nodeGeometry2.m_Right.m_Left, nodeGeometry2.m_Right.m_Right, 0.5f);
			right.m_Right.d = nodeGeometry2.m_Middle.d;
			right2.m_Left = right.m_Right;
			if (edge1.m_Start != node2 && edge1.m_Start != originalNode2)
			{
				flag |= Intersect(edgeGeometry1.m_Start, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, right2, prefabCompositionData1, prefabCompositionData2, ref intersection);
			}
			if (edge1.m_End != node2 && edge1.m_End != originalNode2)
			{
				flag |= Intersect(edgeGeometry1.m_End, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, right2, prefabCompositionData1, prefabCompositionData2, ref intersection);
			}
		}
		else
		{
			Segment left = nodeGeometry2.m_Left;
			Segment right3 = nodeGeometry2.m_Right;
			left.m_Right = nodeGeometry2.m_Middle;
			right3.m_Left = nodeGeometry2.m_Middle;
			if (edge1.m_Start != node2 && edge1.m_Start != originalNode2)
			{
				flag |= Intersect(edgeGeometry1.m_Start, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, nodeGeometry2.m_Right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_Start, right3, prefabCompositionData1, prefabCompositionData2, ref intersection);
			}
			if (edge1.m_End != node2 && edge1.m_End != originalNode2)
			{
				flag |= Intersect(edgeGeometry1.m_End, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, nodeGeometry2.m_Right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(edgeGeometry1.m_End, right3, prefabCompositionData1, prefabCompositionData2, ref intersection);
			}
		}
		return flag;
	}

	public static bool Intersect(Entity node1, Entity originalNode1, NativeArray<ConnectedNode> nodes1, NativeArray<ConnectedNode> originalNodes1, Entity node2, NativeArray<ConnectedNode> nodes2, EdgeNodeGeometry nodeGeometry1, EdgeNodeGeometry nodeGeometry2, NetCompositionData prefabCompositionData1, NetCompositionData prefabCompositionData2, ref Bounds3 intersection)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(nodeGeometry1.m_Bounds, nodeGeometry2.m_Bounds))
		{
			return false;
		}
		if (node1 == node2 || originalNode1 == node2)
		{
			return false;
		}
		if (math.all(nodeGeometry1.m_Left.m_Length <= 0.05f) && math.all(nodeGeometry1.m_Left.m_Length <= 0.05f))
		{
			return false;
		}
		if (math.all(nodeGeometry2.m_Left.m_Length <= 0.05f) && math.all(nodeGeometry2.m_Left.m_Length <= 0.05f))
		{
			return false;
		}
		for (int i = 0; i < nodes1.Length; i++)
		{
			if (nodes1[i].m_Node == node2)
			{
				return false;
			}
		}
		for (int j = 0; j < originalNodes1.Length; j++)
		{
			if (originalNodes1[j].m_Node == node2)
			{
				return false;
			}
		}
		for (int k = 0; k < nodes2.Length; k++)
		{
			Entity node3 = nodes2[k].m_Node;
			if (node3 == node1 || node3 == originalNode1)
			{
				return false;
			}
		}
		Segment segment;
		Segment right;
		if (nodeGeometry1.m_MiddleRadius > 0f)
		{
			segment = nodeGeometry1.m_Right;
			right = nodeGeometry1.m_Right;
			segment.m_Right = MathUtils.Lerp(nodeGeometry1.m_Right.m_Left, nodeGeometry1.m_Right.m_Right, 0.5f);
			segment.m_Right.d = nodeGeometry1.m_Middle.d;
			right.m_Left = segment.m_Right;
		}
		else
		{
			segment = nodeGeometry1.m_Left;
			right = nodeGeometry1.m_Right;
			segment.m_Right = nodeGeometry1.m_Middle;
			right.m_Left = nodeGeometry1.m_Middle;
		}
		Segment segment2;
		Segment right2;
		if (nodeGeometry2.m_MiddleRadius > 0f)
		{
			segment2 = nodeGeometry2.m_Right;
			right2 = nodeGeometry2.m_Right;
			segment2.m_Right = MathUtils.Lerp(nodeGeometry2.m_Right.m_Left, nodeGeometry2.m_Right.m_Right, 0.5f);
			segment2.m_Right.d = nodeGeometry2.m_Middle.d;
			right2.m_Left = segment2.m_Right;
		}
		else
		{
			segment2 = nodeGeometry2.m_Left;
			right2 = nodeGeometry2.m_Right;
			segment2.m_Right = nodeGeometry2.m_Middle;
			right2.m_Left = nodeGeometry2.m_Middle;
		}
		bool flag = Intersect(nodeGeometry1.m_Left, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(nodeGeometry1.m_Left, segment2, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(nodeGeometry1.m_Left, right2, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(segment, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(segment, segment2, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(segment, right2, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(right, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(right, segment2, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(right, right2, prefabCompositionData1, prefabCompositionData2, ref intersection);
		if (nodeGeometry1.m_MiddleRadius <= 0f)
		{
			flag |= Intersect(nodeGeometry1.m_Right, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(nodeGeometry1.m_Right, segment2, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(nodeGeometry1.m_Right, right2, prefabCompositionData1, prefabCompositionData2, ref intersection);
		}
		if (nodeGeometry2.m_MiddleRadius <= 0f)
		{
			flag |= Intersect(nodeGeometry1.m_Left, nodeGeometry2.m_Right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(segment, nodeGeometry2.m_Right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(right, nodeGeometry2.m_Right, prefabCompositionData1, prefabCompositionData2, ref intersection);
		}
		if (nodeGeometry1.m_MiddleRadius <= 0f && nodeGeometry2.m_MiddleRadius <= 0f)
		{
			flag |= Intersect(nodeGeometry1.m_Right, nodeGeometry2.m_Right, prefabCompositionData1, prefabCompositionData2, ref intersection);
		}
		return flag;
	}

	public static bool Intersect(Entity node1, Entity originalNode1, NativeArray<ConnectedNode> nodes1, NativeArray<ConnectedNode> originalNodes1, Entity node2, NativeArray<ConnectedNode> nodes2, EdgeNodeGeometry nodeGeometry1, EdgeNodeGeometry nodeGeometry2, NetCompositionData prefabCompositionData1, NetCompositionData prefabCompositionData2, ref Bounds2 intersection)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.Intersect(((Bounds3)(ref nodeGeometry1.m_Bounds)).xz, ((Bounds3)(ref nodeGeometry2.m_Bounds)).xz))
		{
			return false;
		}
		if (node1 == node2 || originalNode1 == node2)
		{
			return false;
		}
		if (math.all(nodeGeometry1.m_Left.m_Length <= 0.05f) && math.all(nodeGeometry1.m_Left.m_Length <= 0.05f))
		{
			return false;
		}
		if (math.all(nodeGeometry2.m_Left.m_Length <= 0.05f) && math.all(nodeGeometry2.m_Left.m_Length <= 0.05f))
		{
			return false;
		}
		for (int i = 0; i < nodes1.Length; i++)
		{
			if (nodes1[i].m_Node == node2)
			{
				return false;
			}
		}
		for (int j = 0; j < originalNodes1.Length; j++)
		{
			if (originalNodes1[j].m_Node == node2)
			{
				return false;
			}
		}
		for (int k = 0; k < nodes2.Length; k++)
		{
			Entity node3 = nodes2[k].m_Node;
			if (node3 == node1 || node3 == originalNode1)
			{
				return false;
			}
		}
		Segment segment;
		Segment right;
		if (nodeGeometry1.m_MiddleRadius > 0f)
		{
			segment = nodeGeometry1.m_Right;
			right = nodeGeometry1.m_Right;
			segment.m_Right = MathUtils.Lerp(nodeGeometry1.m_Right.m_Left, nodeGeometry1.m_Right.m_Right, 0.5f);
			segment.m_Right.d = nodeGeometry1.m_Middle.d;
			right.m_Left = segment.m_Right;
		}
		else
		{
			segment = nodeGeometry1.m_Left;
			right = nodeGeometry1.m_Right;
			segment.m_Right = nodeGeometry1.m_Middle;
			right.m_Left = nodeGeometry1.m_Middle;
		}
		Segment segment2;
		Segment right2;
		if (nodeGeometry2.m_MiddleRadius > 0f)
		{
			segment2 = nodeGeometry2.m_Right;
			right2 = nodeGeometry2.m_Right;
			segment2.m_Right = MathUtils.Lerp(nodeGeometry2.m_Right.m_Left, nodeGeometry2.m_Right.m_Right, 0.5f);
			segment2.m_Right.d = nodeGeometry2.m_Middle.d;
			right2.m_Left = segment2.m_Right;
		}
		else
		{
			segment2 = nodeGeometry2.m_Left;
			right2 = nodeGeometry2.m_Right;
			segment2.m_Right = nodeGeometry2.m_Middle;
			right2.m_Left = nodeGeometry2.m_Middle;
		}
		bool flag = Intersect(nodeGeometry1.m_Left, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(nodeGeometry1.m_Left, segment2, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(nodeGeometry1.m_Left, right2, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(segment, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(segment, segment2, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(segment, right2, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(right, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(right, segment2, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(right, right2, prefabCompositionData1, prefabCompositionData2, ref intersection);
		if (nodeGeometry1.m_MiddleRadius <= 0f)
		{
			flag |= Intersect(nodeGeometry1.m_Right, nodeGeometry2.m_Left, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(nodeGeometry1.m_Right, segment2, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(nodeGeometry1.m_Right, right2, prefabCompositionData1, prefabCompositionData2, ref intersection);
		}
		if (nodeGeometry2.m_MiddleRadius <= 0f)
		{
			flag |= Intersect(nodeGeometry1.m_Left, nodeGeometry2.m_Right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(segment, nodeGeometry2.m_Right, prefabCompositionData1, prefabCompositionData2, ref intersection) | Intersect(right, nodeGeometry2.m_Right, prefabCompositionData1, prefabCompositionData2, ref intersection);
		}
		if (nodeGeometry1.m_MiddleRadius <= 0f && nodeGeometry2.m_MiddleRadius <= 0f)
		{
			flag |= Intersect(nodeGeometry1.m_Right, nodeGeometry2.m_Right, prefabCompositionData1, prefabCompositionData2, ref intersection);
		}
		return flag;
	}

	public static bool Intersect(Segment segment1, float2 segmentSide1, Triangle3 triangle2, NetCompositionData prefabCompositionData1, Bounds1 heightRange2, ref Bounds3 intersection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 val = SetHeightRange(MathUtils.Bounds(segment1.m_Left) | MathUtils.Bounds(segment1.m_Right), prefabCompositionData1.m_HeightRange);
		Bounds3 val2 = SetHeightRange(MathUtils.Bounds(triangle2), heightRange2);
		if (!MathUtils.Intersect(val, val2))
		{
			return false;
		}
		bool result = false;
		Quad3 val3 = default(Quad3);
		val3.a = segment1.m_Left.a;
		val3.b = segment1.m_Right.a;
		Bounds3 val4 = SetHeightRange(MathUtils.Bounds(val3.a, val3.b), prefabCompositionData1.m_HeightRange);
		Bounds3 val9 = default(Bounds3);
		for (int i = 1; i <= 8; i++)
		{
			float num = (float)i / 8f;
			val3.d = MathUtils.Position(segment1.m_Left, num);
			val3.c = MathUtils.Position(segment1.m_Right, num);
			Bounds3 val5 = SetHeightRange(MathUtils.Bounds(val3.d, val3.c), prefabCompositionData1.m_HeightRange);
			if (MathUtils.Intersect(val4 | val5, val2))
			{
				Quad3 val6 = val3;
				float3 val7 = math.normalizesafe(val6.b - val6.a, default(float3)) * 0.5f;
				float3 val8 = math.normalizesafe(val6.d - val6.c, default(float3)) * 0.5f;
				ref float3 a = ref val6.a;
				a += val7;
				ref float3 b = ref val6.b;
				b -= val7;
				ref float3 c = ref val6.c;
				c += val8;
				ref float3 d = ref val6.d;
				d -= val8;
				if (QuadTriangleIntersect(val6, triangle2, out var intersection2, out var intersection3))
				{
					intersection2 = SetHeightRange(intersection2, prefabCompositionData1.m_HeightRange);
					intersection3 = SetHeightRange(intersection3, heightRange2);
					if (MathUtils.Intersect(intersection2, intersection3, ref val9))
					{
						result = true;
						intersection |= val9;
					}
				}
			}
			val3.a = val3.d;
			val3.b = val3.c;
			val4 = val5;
		}
		return result;
	}

	public static bool Intersect(Segment segment1, Segment segment2, NetCompositionData prefabCompositionData1, NetCompositionData prefabCompositionData2, ref Bounds3 intersection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 val = SetHeightRange(MathUtils.Bounds(segment1.m_Left) | MathUtils.Bounds(segment1.m_Right), prefabCompositionData1.m_HeightRange);
		Bounds3 val2 = SetHeightRange(MathUtils.Bounds(segment2.m_Left) | MathUtils.Bounds(segment2.m_Right), prefabCompositionData2.m_HeightRange);
		if (!MathUtils.Intersect(val, val2))
		{
			return false;
		}
		bool result = false;
		Quad3 val3 = default(Quad3);
		val3.a = segment1.m_Left.a;
		val3.b = segment1.m_Right.a;
		Bounds3 val4 = SetHeightRange(MathUtils.Bounds(val3.a, val3.b), prefabCompositionData1.m_HeightRange);
		Quad3 val7 = default(Quad3);
		Bounds3 val11 = default(Bounds3);
		for (int i = 1; i <= 8; i++)
		{
			float num = (float)i / 8f;
			val3.d = MathUtils.Position(segment1.m_Left, num);
			val3.c = MathUtils.Position(segment1.m_Right, num);
			Bounds3 val5 = SetHeightRange(MathUtils.Bounds(val3.d, val3.c), prefabCompositionData1.m_HeightRange);
			Bounds3 val6 = val4 | val5;
			if (MathUtils.Intersect(val6, val2))
			{
				val7.a = segment2.m_Left.a;
				val7.b = segment2.m_Right.a;
				Bounds3 val8 = SetHeightRange(MathUtils.Bounds(val7.a, val7.b), prefabCompositionData2.m_HeightRange);
				for (int j = 1; j <= 8; j++)
				{
					float num2 = (float)j / 8f;
					val7.d = MathUtils.Position(segment2.m_Left, num2);
					val7.c = MathUtils.Position(segment2.m_Right, num2);
					Bounds3 val9 = SetHeightRange(MathUtils.Bounds(val7.d, val7.c), prefabCompositionData2.m_HeightRange);
					Bounds3 val10 = val8 | val9;
					if (MathUtils.Intersect(val6, val10) && QuadIntersect(val3, val7, out var intersection2, out var intersection3))
					{
						intersection2 = SetHeightRange(intersection2, prefabCompositionData1.m_HeightRange);
						intersection3 = SetHeightRange(intersection3, prefabCompositionData2.m_HeightRange);
						if (MathUtils.Intersect(intersection2, intersection3, ref val11))
						{
							result = true;
							intersection |= val11;
						}
					}
					val7.a = val7.d;
					val7.b = val7.c;
					val8 = val9;
				}
			}
			val3.a = val3.d;
			val3.b = val3.c;
			val4 = val5;
		}
		return result;
	}

	public static bool Intersect(Segment segment1, Segment segment2, NetCompositionData prefabCompositionData1, NetCompositionData prefabCompositionData2, ref Bounds2 intersection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 val = MathUtils.Bounds(segment1.m_Left) | MathUtils.Bounds(segment1.m_Right);
		Bounds2 xz = ((Bounds3)(ref val)).xz;
		val = MathUtils.Bounds(segment2.m_Left) | MathUtils.Bounds(segment2.m_Right);
		Bounds2 xz2 = ((Bounds3)(ref val)).xz;
		if (!MathUtils.Intersect(xz, xz2))
		{
			return false;
		}
		bool result = false;
		Quad2 val2 = default(Quad2);
		val2.a = ((float3)(ref segment1.m_Left.a)).xz;
		val2.b = ((float3)(ref segment1.m_Right.a)).xz;
		Bounds2 val3 = MathUtils.Bounds(val2.a, val2.b);
		Quad2 val7 = default(Quad2);
		Bounds2 val11 = default(Bounds2);
		for (int i = 1; i <= 8; i++)
		{
			float num = (float)i / 8f;
			float3 val4 = MathUtils.Position(segment1.m_Left, num);
			val2.d = ((float3)(ref val4)).xz;
			val4 = MathUtils.Position(segment1.m_Right, num);
			val2.c = ((float3)(ref val4)).xz;
			Bounds2 val5 = MathUtils.Bounds(val2.d, val2.c);
			Bounds2 val6 = val3 | val5;
			if (MathUtils.Intersect(val6, xz2))
			{
				val7.a = ((float3)(ref segment2.m_Left.a)).xz;
				val7.b = ((float3)(ref segment2.m_Right.a)).xz;
				Bounds2 val8 = MathUtils.Bounds(val7.a, val7.b);
				for (int j = 1; j <= 8; j++)
				{
					float num2 = (float)j / 8f;
					val4 = MathUtils.Position(segment2.m_Left, num2);
					val7.d = ((float3)(ref val4)).xz;
					val4 = MathUtils.Position(segment2.m_Right, num2);
					val7.c = ((float3)(ref val4)).xz;
					Bounds2 val9 = MathUtils.Bounds(val7.d, val7.c);
					Bounds2 val10 = val8 | val9;
					if (MathUtils.Intersect(val6, val10) && MathUtils.Intersect(val2, val7, ref val11))
					{
						result = true;
						intersection |= val11;
					}
					val7.a = val7.d;
					val7.b = val7.c;
					val8 = val9;
				}
			}
			val2.a = val2.d;
			val2.b = val2.c;
			val3 = val5;
		}
		return result;
	}

	public static Bounds3 SetHeightRange(Bounds3 bounds, Bounds1 heightRange)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		bounds.min.y += heightRange.min;
		bounds.max.y += heightRange.max;
		return bounds;
	}

	public static bool QuadIntersect(Quad3 quad1, Quad3 quad2, out Bounds3 intersection1, out Bounds3 intersection2)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		intersection1.min = float3.op_Implicit(float.MaxValue);
		intersection1.max = float3.op_Implicit(float.MinValue);
		intersection2.min = float3.op_Implicit(float.MaxValue);
		intersection2.max = float3.op_Implicit(float.MinValue);
		Triangle3 triangle = new Triangle3(quad1.a, quad1.d, quad1.c);
		Triangle3 triangle2 = default(Triangle3);
		((Triangle3)(ref triangle2))._002Ector(quad1.c, quad1.b, quad1.a);
		Triangle3 triangle3 = default(Triangle3);
		((Triangle3)(ref triangle3))._002Ector(quad2.a, quad2.d, quad2.c);
		Triangle3 triangle4 = default(Triangle3);
		((Triangle3)(ref triangle4))._002Ector(quad2.c, quad2.b, quad2.a);
		Segment line = default(Segment);
		((Segment)(ref line))._002Ector(quad1.a, quad1.b);
		Segment line2 = default(Segment);
		((Segment)(ref line2))._002Ector(quad1.b, quad1.c);
		Segment line3 = default(Segment);
		((Segment)(ref line3))._002Ector(quad1.c, quad1.d);
		Segment line4 = default(Segment);
		((Segment)(ref line4))._002Ector(quad1.d, quad1.a);
		return QuadIntersectHelper(triangle, quad2, ref intersection1, ref intersection2) | QuadIntersectHelper(triangle2, quad2, ref intersection1, ref intersection2) | QuadIntersectHelper(triangle3, quad1, ref intersection2, ref intersection1) | QuadIntersectHelper(triangle4, quad1, ref intersection2, ref intersection1) | QuadIntersectHelper(line, quad2, ref intersection1, ref intersection2) | QuadIntersectHelper(line2, quad2, ref intersection1, ref intersection2) | QuadIntersectHelper(line3, quad2, ref intersection1, ref intersection2) | QuadIntersectHelper(line4, quad2, ref intersection1, ref intersection2);
	}

	public static bool QuadTriangleIntersect(Quad3 quad1, Triangle3 triangle2, out Bounds3 intersection1, out Bounds3 intersection2)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		intersection1.min = float3.op_Implicit(float.MaxValue);
		intersection1.max = float3.op_Implicit(float.MinValue);
		intersection2.min = float3.op_Implicit(float.MaxValue);
		intersection2.max = float3.op_Implicit(float.MinValue);
		Triangle3 triangle3 = new Triangle3(quad1.a, quad1.d, quad1.c);
		Triangle3 triangle4 = default(Triangle3);
		((Triangle3)(ref triangle4))._002Ector(quad1.c, quad1.b, quad1.a);
		Segment line = default(Segment);
		((Segment)(ref line))._002Ector(quad1.a, quad1.b);
		Segment line2 = default(Segment);
		((Segment)(ref line2))._002Ector(quad1.b, quad1.c);
		Segment line3 = default(Segment);
		((Segment)(ref line3))._002Ector(quad1.c, quad1.d);
		Segment line4 = default(Segment);
		((Segment)(ref line4))._002Ector(quad1.d, quad1.a);
		return QuadTriangleIntersectHelper(triangle3, triangle2, ref intersection1, ref intersection2) | QuadTriangleIntersectHelper(triangle4, triangle2, ref intersection1, ref intersection2) | QuadIntersectHelper(triangle2, quad1, ref intersection2, ref intersection1) | QuadTriangleIntersectHelper(line, triangle2, ref intersection1, ref intersection2) | QuadTriangleIntersectHelper(line2, triangle2, ref intersection1, ref intersection2) | QuadTriangleIntersectHelper(line3, triangle2, ref intersection1, ref intersection2) | QuadTriangleIntersectHelper(line4, triangle2, ref intersection1, ref intersection2);
	}

	private static bool QuadIntersectHelper(Triangle3 triangle1, Quad3 quad2, ref Bounds3 intersection1, ref Bounds3 intersection2)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		Triangle2 xz = ((Triangle3)(ref triangle1)).xz;
		bool result = false;
		float2 val = default(float2);
		if (MathUtils.Intersect(xz, ((float3)(ref quad2.a)).xz, ref val))
		{
			intersection1 |= MathUtils.Position(triangle1, val);
			intersection2 |= quad2.a;
			result = true;
		}
		if (MathUtils.Intersect(xz, ((float3)(ref quad2.b)).xz, ref val))
		{
			intersection1 |= MathUtils.Position(triangle1, val);
			intersection2 |= quad2.b;
			result = true;
		}
		if (MathUtils.Intersect(xz, ((float3)(ref quad2.c)).xz, ref val))
		{
			intersection1 |= MathUtils.Position(triangle1, val);
			intersection2 |= quad2.c;
			result = true;
		}
		if (MathUtils.Intersect(xz, ((float3)(ref quad2.d)).xz, ref val))
		{
			intersection1 |= MathUtils.Position(triangle1, val);
			intersection2 |= quad2.d;
			result = true;
		}
		return result;
	}

	private static bool QuadTriangleIntersectHelper(Triangle3 triangle1, Triangle3 triangle2, ref Bounds3 intersection1, ref Bounds3 intersection2)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		Triangle2 xz = ((Triangle3)(ref triangle1)).xz;
		bool result = false;
		float2 val = default(float2);
		if (MathUtils.Intersect(xz, ((float3)(ref triangle2.a)).xz, ref val))
		{
			intersection1 |= MathUtils.Position(triangle1, val);
			intersection2 |= triangle2.a;
			result = true;
		}
		if (MathUtils.Intersect(xz, ((float3)(ref triangle2.b)).xz, ref val))
		{
			intersection1 |= MathUtils.Position(triangle1, val);
			intersection2 |= triangle2.b;
			result = true;
		}
		if (MathUtils.Intersect(xz, ((float3)(ref triangle2.c)).xz, ref val))
		{
			intersection1 |= MathUtils.Position(triangle1, val);
			intersection2 |= triangle2.c;
			result = true;
		}
		return result;
	}

	private static bool QuadIntersectHelper(Segment line1, Quad3 quad2, ref Bounds3 intersection1, ref Bounds3 intersection2)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		Segment xz = ((Segment)(ref line1)).xz;
		bool result = false;
		float2 val = default(float2);
		if (MathUtils.Intersect(xz, new Segment(((float3)(ref quad2.a)).xz, ((float3)(ref quad2.b)).xz), ref val))
		{
			intersection1 |= MathUtils.Position(line1, val.x);
			intersection2 |= math.lerp(quad2.a, quad2.b, val.y);
			result = true;
		}
		if (MathUtils.Intersect(xz, new Segment(((float3)(ref quad2.b)).xz, ((float3)(ref quad2.c)).xz), ref val))
		{
			intersection1 |= MathUtils.Position(line1, val.x);
			intersection2 |= math.lerp(quad2.b, quad2.c, val.y);
			result = true;
		}
		if (MathUtils.Intersect(xz, new Segment(((float3)(ref quad2.c)).xz, ((float3)(ref quad2.d)).xz), ref val))
		{
			intersection1 |= MathUtils.Position(line1, val.x);
			intersection2 |= math.lerp(quad2.c, quad2.d, val.y);
			result = true;
		}
		if (MathUtils.Intersect(xz, new Segment(((float3)(ref quad2.d)).xz, ((float3)(ref quad2.a)).xz), ref val))
		{
			intersection1 |= MathUtils.Position(line1, val.x);
			intersection2 |= math.lerp(quad2.d, quad2.a, val.y);
			result = true;
		}
		return result;
	}

	private static bool QuadTriangleIntersectHelper(Segment line1, Triangle3 triangle2, ref Bounds3 intersection1, ref Bounds3 intersection2)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		Segment xz = ((Segment)(ref line1)).xz;
		bool result = false;
		float2 val = default(float2);
		if (MathUtils.Intersect(xz, new Segment(((float3)(ref triangle2.a)).xz, ((float3)(ref triangle2.b)).xz), ref val))
		{
			intersection1 |= MathUtils.Position(line1, val.x);
			intersection2 |= math.lerp(triangle2.a, triangle2.b, val.y);
			result = true;
		}
		if (MathUtils.Intersect(xz, new Segment(((float3)(ref triangle2.b)).xz, ((float3)(ref triangle2.c)).xz), ref val))
		{
			intersection1 |= MathUtils.Position(line1, val.x);
			intersection2 |= math.lerp(triangle2.b, triangle2.c, val.y);
			result = true;
		}
		if (MathUtils.Intersect(xz, new Segment(((float3)(ref triangle2.c)).xz, ((float3)(ref triangle2.a)).xz), ref val))
		{
			intersection1 |= MathUtils.Position(line1, val.x);
			intersection2 |= math.lerp(triangle2.c, triangle2.a, val.y);
			result = true;
		}
		return result;
	}
}
