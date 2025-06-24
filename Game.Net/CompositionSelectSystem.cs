using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.City;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Net;

[CompilerGenerated]
public class CompositionSelectSystem : GameSystemBase
{
	private struct CompositionCreateInfo
	{
		public Entity m_Entity;

		public Entity m_Prefab;

		public CompositionFlags m_EdgeFlags;

		public CompositionFlags m_StartFlags;

		public CompositionFlags m_EndFlags;

		public CompositionFlags m_ObsoleteEdgeFlags;

		public CompositionFlags m_ObsoleteStartFlags;

		public CompositionFlags m_ObsoleteEndFlags;

		public Composition m_CompositionData;
	}

	[BurstCompile]
	private struct SelectCompositionJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Edge> m_EdgeType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<Upgraded> m_UpgradedType;

		[ReadOnly]
		public ComponentTypeHandle<Fixed> m_FixedType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<Composition> m_CompositionType;

		public ComponentTypeHandle<Orphan> m_OrphanType;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Upgraded> m_UpgradedData;

		[ReadOnly]
		public ComponentLookup<Fixed> m_FixedData;

		[ReadOnly]
		public ComponentLookup<Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Attached> m_AttachedData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public ComponentLookup<NetObjectData> m_PrefabNetObjectData;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> m_PrefabPlaceableObjectData;

		[ReadOnly]
		public ComponentLookup<RoadData> m_PrefabRoadData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<NetGeometryComposition> m_PrefabGeometryCompositions;

		[ReadOnly]
		public BufferLookup<NetGeometryEdgeState> m_PrefabGeometryEdgeStates;

		[ReadOnly]
		public BufferLookup<NetGeometryNodeState> m_PrefabGeometryNodeStates;

		[ReadOnly]
		public BufferLookup<FixedNetElement> m_PrefabFixedNetElements;

		[ReadOnly]
		public bool m_LeftHandTraffic;

		public ParallelWriter<CompositionCreateInfo> m_CompositionCreateQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Edge> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Edge>(ref m_EdgeType);
			if (nativeArray2.Length != 0)
			{
				NativeArray<Curve> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
				NativeArray<Upgraded> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Upgraded>(ref m_UpgradedType);
				NativeArray<Fixed> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Fixed>(ref m_FixedType);
				NativeArray<PrefabRef> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				NativeArray<Composition> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Composition>(ref m_CompositionType);
				NativeArray<Owner> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
				Owner owner = default(Owner);
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Entity val = nativeArray[i];
					Edge edge = nativeArray2[i];
					Curve curve = nativeArray3[i];
					CollectionUtils.TryGet<Owner>(nativeArray8, i, ref owner);
					CompositionCreateInfo compositionCreateInfo = new CompositionCreateInfo
					{
						m_Entity = val,
						m_Prefab = nativeArray6[i].m_Prefab
					};
					NetData prefabNetData = m_PrefabNetData[compositionCreateInfo.m_Prefab];
					NetGeometryData prefabGeometryData = m_PrefabGeometryData[compositionCreateInfo.m_Prefab];
					DynamicBuffer<NetGeometryComposition> geometryCompositions = m_PrefabGeometryCompositions[compositionCreateInfo.m_Prefab];
					CompositionFlags upgradeFlags = default(CompositionFlags);
					CompositionFlags subObjectFlags = GetSubObjectFlags(val, curve, prefabGeometryData);
					CompositionFlags elevationFlags = GetElevationFlags(val, edge.m_Start, edge.m_End, prefabGeometryData);
					if (nativeArray4.Length != 0)
					{
						upgradeFlags = nativeArray4[i].m_Flags;
					}
					if (nativeArray5.Length != 0 && m_PrefabFixedNetElements.HasBuffer(compositionCreateInfo.m_Prefab))
					{
						Fixed obj = nativeArray5[i];
						DynamicBuffer<FixedNetElement> val2 = m_PrefabFixedNetElements[compositionCreateInfo.m_Prefab];
						if (obj.m_Index >= 0 && obj.m_Index < val2.Length)
						{
							FixedNetElement fixedNetElement = val2[obj.m_Index];
							upgradeFlags |= fixedNetElement.m_SetState;
							elevationFlags &= ~fixedNetElement.m_UnsetState;
						}
					}
					compositionCreateInfo.m_EdgeFlags = GetEdgeFlags(compositionCreateInfo.m_Entity, compositionCreateInfo.m_Prefab, prefabGeometryData, upgradeFlags, subObjectFlags, elevationFlags, out compositionCreateInfo.m_ObsoleteEdgeFlags);
					compositionCreateInfo.m_StartFlags = GetNodeFlags(compositionCreateInfo.m_Entity, edge.m_Start, owner.m_Owner, compositionCreateInfo.m_Prefab, prefabNetData, prefabGeometryData, compositionCreateInfo.m_EdgeFlags, curve, isStart: true, out compositionCreateInfo.m_ObsoleteStartFlags, ref compositionCreateInfo.m_ObsoleteEdgeFlags);
					compositionCreateInfo.m_EndFlags = GetNodeFlags(compositionCreateInfo.m_Entity, edge.m_End, owner.m_Owner, compositionCreateInfo.m_Prefab, prefabNetData, prefabGeometryData, compositionCreateInfo.m_EdgeFlags, curve, isStart: false, out compositionCreateInfo.m_ObsoleteEndFlags, ref compositionCreateInfo.m_ObsoleteEdgeFlags);
					compositionCreateInfo.m_EdgeFlags.m_General |= (compositionCreateInfo.m_StartFlags.m_General | compositionCreateInfo.m_EndFlags.m_General) & CompositionFlags.General.FixedNodeSize;
					compositionCreateInfo.m_CompositionData.m_Edge = FindComposition(geometryCompositions, compositionCreateInfo.m_EdgeFlags);
					compositionCreateInfo.m_CompositionData.m_StartNode = FindComposition(geometryCompositions, compositionCreateInfo.m_StartFlags);
					compositionCreateInfo.m_CompositionData.m_EndNode = FindComposition(geometryCompositions, compositionCreateInfo.m_EndFlags);
					if (compositionCreateInfo.m_CompositionData.m_Edge == Entity.Null || compositionCreateInfo.m_ObsoleteEdgeFlags != default(CompositionFlags) || compositionCreateInfo.m_CompositionData.m_StartNode == Entity.Null || compositionCreateInfo.m_ObsoleteStartFlags != default(CompositionFlags) || compositionCreateInfo.m_CompositionData.m_EndNode == Entity.Null || compositionCreateInfo.m_ObsoleteEndFlags != default(CompositionFlags))
					{
						m_CompositionCreateQueue.Enqueue(compositionCreateInfo);
					}
					else
					{
						nativeArray7[i] = compositionCreateInfo.m_CompositionData;
					}
				}
				return;
			}
			NativeArray<PrefabRef> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Orphan> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Orphan>(ref m_OrphanType);
			for (int j = 0; j < nativeArray10.Length; j++)
			{
				Entity val3 = nativeArray[j];
				CompositionCreateInfo compositionCreateInfo2 = new CompositionCreateInfo
				{
					m_Entity = val3,
					m_Prefab = nativeArray9[j].m_Prefab
				};
				NetData prefabNetData2 = m_PrefabNetData[compositionCreateInfo2.m_Prefab];
				NetGeometryData prefabGeometryData2 = m_PrefabGeometryData[compositionCreateInfo2.m_Prefab];
				DynamicBuffer<NetGeometryComposition> geometryCompositions2 = m_PrefabGeometryCompositions[compositionCreateInfo2.m_Prefab];
				CompositionFlags elevationFlags2 = GetElevationFlags(Entity.Null, val3, val3, prefabGeometryData2);
				CompositionFlags obsoleteEdgeFlags = default(CompositionFlags);
				compositionCreateInfo2.m_EdgeFlags = GetNodeFlags(Entity.Null, compositionCreateInfo2.m_Entity, Entity.Null, compositionCreateInfo2.m_Prefab, prefabNetData2, prefabGeometryData2, elevationFlags2, default(Curve), isStart: true, out compositionCreateInfo2.m_ObsoleteEdgeFlags, ref obsoleteEdgeFlags);
				compositionCreateInfo2.m_CompositionData.m_Edge = FindComposition(geometryCompositions2, compositionCreateInfo2.m_EdgeFlags);
				if (compositionCreateInfo2.m_CompositionData.m_Edge == Entity.Null || compositionCreateInfo2.m_ObsoleteEdgeFlags != default(CompositionFlags))
				{
					m_CompositionCreateQueue.Enqueue(compositionCreateInfo2);
					continue;
				}
				nativeArray10[j] = new Orphan
				{
					m_Composition = compositionCreateInfo2.m_CompositionData.m_Edge
				};
			}
		}

		private Entity FindComposition(DynamicBuffer<NetGeometryComposition> geometryCompositions, CompositionFlags flags)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < geometryCompositions.Length; i++)
			{
				NetGeometryComposition netGeometryComposition = geometryCompositions[i];
				if (netGeometryComposition.m_Mask == flags)
				{
					return netGeometryComposition.m_Composition;
				}
			}
			return Entity.Null;
		}

		private CompositionFlags GetSubObjectFlags(Entity entity, Curve curve, NetGeometryData prefabGeometryData)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			CompositionFlags result = default(CompositionFlags);
			DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
			if (m_SubObjects.TryGetBuffer(entity, ref val))
			{
				NetObjectData netObjectData = default(NetObjectData);
				PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
				Attached attached = default(Attached);
				for (int i = 0; i < val.Length; i++)
				{
					Entity subObject = val[i].m_SubObject;
					PrefabRef prefabRef = m_PrefabRefData[subObject];
					if (!m_PrefabNetObjectData.TryGetComponent(prefabRef.m_Prefab, ref netObjectData) || !m_PrefabPlaceableObjectData.TryGetComponent(prefabRef.m_Prefab, ref placeableObjectData))
					{
						continue;
					}
					netObjectData.m_CompositionFlags &= ~CompositionFlags.nodeMask;
					if ((placeableObjectData.m_Flags & Game.Objects.PlacementFlags.Attached) != Game.Objects.PlacementFlags.None && m_AttachedData.TryGetComponent(subObject, ref attached))
					{
						if (attached.m_Parent != entity)
						{
							continue;
						}
						Transform transform = m_TransformData[subObject];
						float3 val2 = MathUtils.Position(curve.m_Bezier, attached.m_CurvePosition);
						float3 val3 = math.normalizesafe(MathUtils.Tangent(curve.m_Bezier, attached.m_CurvePosition), default(float3));
						float num = math.dot(MathUtils.Left(((float3)(ref val3)).xz), ((float3)(ref transform.m_Position)).xz - ((float3)(ref val2)).xz);
						if (num > 0f)
						{
							netObjectData.m_CompositionFlags = NetCompositionHelpers.InvertCompositionFlags(netObjectData.m_CompositionFlags);
						}
						if ((netObjectData.m_CompositionFlags & ~CompositionFlags.optionMask) == default(CompositionFlags) && netObjectData.m_CompositionFlags.m_General != 0 && (netObjectData.m_CompositionFlags.m_Left != 0 || netObjectData.m_CompositionFlags.m_Right != 0))
						{
							if (math.abs(num) < prefabGeometryData.m_DefaultWidth * (1f / 6f))
							{
								netObjectData.m_CompositionFlags.m_Left = (CompositionFlags.Side)0u;
								netObjectData.m_CompositionFlags.m_Right = (CompositionFlags.Side)0u;
							}
							else
							{
								netObjectData.m_CompositionFlags.m_General = (CompositionFlags.General)0u;
							}
						}
					}
					result |= netObjectData.m_CompositionFlags;
				}
			}
			return result;
		}

		private CompositionFlags GetSubObjectFlags(Entity entity, NetData prefabNetData)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			CompositionFlags result = default(CompositionFlags);
			DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
			if (m_SubObjects.TryGetBuffer(entity, ref val))
			{
				NetObjectData netObjectData = default(NetObjectData);
				PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
				Attached attached = default(Attached);
				for (int i = 0; i < val.Length; i++)
				{
					Entity subObject = val[i].m_SubObject;
					PrefabRef prefabRef = m_PrefabRefData[subObject];
					if (m_PrefabNetObjectData.TryGetComponent(prefabRef.m_Prefab, ref netObjectData) && m_PrefabPlaceableObjectData.TryGetComponent(prefabRef.m_Prefab, ref placeableObjectData) && ((placeableObjectData.m_Flags & Game.Objects.PlacementFlags.Attached) == 0 || !m_AttachedData.TryGetComponent(subObject, ref attached) || !(attached.m_Parent != entity)))
					{
						result |= netObjectData.m_CompositionFlags & CompositionFlags.nodeMask;
					}
				}
				if ((result.m_General & CompositionFlags.General.FixedNodeSize) != 0)
				{
					PrefabRef prefabRef2 = m_PrefabRefData[entity];
					NetData netData = m_PrefabNetData[prefabRef2.m_Prefab];
					if ((prefabNetData.m_RequiredLayers & netData.m_RequiredLayers) == 0)
					{
						result.m_General &= ~CompositionFlags.General.FixedNodeSize;
					}
				}
			}
			return result;
		}

		private CompositionFlags GetElevationFlags(Entity edge, Entity startNode, Entity endNode, NetGeometryData prefabGeometryData)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			Elevation elevation = default(Elevation);
			m_ElevationData.TryGetComponent(startNode, ref elevation);
			Elevation endElevation = default(Elevation);
			m_ElevationData.TryGetComponent(endNode, ref endElevation);
			Elevation middleElevation = default(Elevation);
			if (edge == Entity.Null)
			{
				middleElevation = elevation;
			}
			else
			{
				m_ElevationData.TryGetComponent(edge, ref middleElevation);
			}
			return NetCompositionHelpers.GetElevationFlags(elevation, middleElevation, endElevation, prefabGeometryData);
		}

		private CompositionFlags GetEdgeFlags(Entity edge, Entity prefab, NetGeometryData prefabGeometryData, CompositionFlags upgradeFlags, CompositionFlags subObjectFlags, CompositionFlags elevationFlags, out CompositionFlags obsoleteEdgeFlags)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			CompositionFlags compositionFlags = upgradeFlags | subObjectFlags | elevationFlags;
			obsoleteEdgeFlags = default(CompositionFlags);
			compositionFlags.m_General |= CompositionFlags.General.Edge;
			compositionFlags |= GetHandednessFlags(prefabGeometryData);
			compositionFlags |= GetEdgeStates(prefab, compositionFlags);
			bool num = (compositionFlags.m_General & (CompositionFlags.General.Elevated | CompositionFlags.General.Tunnel)) != 0;
			bool flag = (compositionFlags.m_Left & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) != 0;
			bool flag2 = (compositionFlags.m_Right & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) != 0;
			if ((prefabGeometryData.m_Flags & (GeometryFlags.RequireElevated | GeometryFlags.ElevatedIsRaised)) == GeometryFlags.ElevatedIsRaised)
			{
				flag = false;
				flag2 = false;
			}
			if (num)
			{
				CompositionFlags.General general = upgradeFlags.m_General & (CompositionFlags.General.PrimaryMiddleBeautification | CompositionFlags.General.SecondaryMiddleBeautification);
				compositionFlags.m_General &= ~general;
				obsoleteEdgeFlags.m_General |= general;
			}
			if (num || flag)
			{
				CompositionFlags.Side side = upgradeFlags.m_Left & (CompositionFlags.Side.PrimaryBeautification | CompositionFlags.Side.SecondaryBeautification | CompositionFlags.Side.WideSidewalk | CompositionFlags.Side.SoundBarrier);
				compositionFlags.m_Left &= ~side;
				obsoleteEdgeFlags.m_Left |= side;
			}
			if (num || flag2)
			{
				CompositionFlags.Side side2 = upgradeFlags.m_Right & (CompositionFlags.Side.PrimaryBeautification | CompositionFlags.Side.SecondaryBeautification | CompositionFlags.Side.WideSidewalk | CompositionFlags.Side.SoundBarrier);
				compositionFlags.m_Right &= ~side2;
				obsoleteEdgeFlags.m_Right |= side2;
			}
			return compositionFlags;
		}

		private CompositionFlags GetEdgeStates(Entity prefab, CompositionFlags edgeFlags)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			CompositionFlags result = default(CompositionFlags);
			DynamicBuffer<NetGeometryEdgeState> val = default(DynamicBuffer<NetGeometryEdgeState>);
			if (m_PrefabGeometryEdgeStates.TryGetBuffer(prefab, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					NetGeometryEdgeState edgeState = val[i];
					if (NetCompositionHelpers.TestEdgeFlags(edgeState, edgeFlags))
					{
						result |= edgeState.m_State;
					}
				}
			}
			return result;
		}

		private CompositionFlags GetNodeStates(DynamicBuffer<NetGeometryNodeState> nodeStates, CompositionFlags edgeFlags1, CompositionFlags edgeFlags2, bool isLeft, bool isRight)
		{
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			CompositionFlags result = default(CompositionFlags);
			bool2 match = default(bool2);
			for (int i = 0; i < nodeStates.Length; i++)
			{
				NetGeometryNodeState netGeometryNodeState = nodeStates[i];
				NetGeometryNodeState nodeState = netGeometryNodeState;
				CompositionFlags compositionFlags = edgeFlags1;
				CompositionFlags compositionFlags2 = edgeFlags2;
				if (!isLeft)
				{
					nodeState.m_CompositionNone.m_Left = (CompositionFlags.Side)0u;
					nodeState.m_State.m_Left = (CompositionFlags.Side)0u;
					compositionFlags2.m_Right = (CompositionFlags.Side)0u;
					if (nodeState.m_MatchType == NetEdgeMatchType.Exclusive)
					{
						compositionFlags.m_Left = (CompositionFlags.Side)0u;
					}
				}
				if (!isRight)
				{
					nodeState.m_CompositionNone.m_Right = (CompositionFlags.Side)0u;
					nodeState.m_State.m_Right = (CompositionFlags.Side)0u;
					compositionFlags2.m_Left = (CompositionFlags.Side)0u;
					if (nodeState.m_MatchType == NetEdgeMatchType.Exclusive)
					{
						compositionFlags.m_Right = (CompositionFlags.Side)0u;
					}
				}
				((bool2)(ref match))._002Ector(NetCompositionHelpers.TestEdgeFlags(netGeometryNodeState, compositionFlags), NetCompositionHelpers.TestEdgeFlags(nodeState, compositionFlags2));
				if (NetCompositionHelpers.TestEdgeMatch(nodeState, match))
				{
					result |= nodeState.m_State;
				}
			}
			return result;
		}

		private CompositionFlags GetEdgeFlags(Entity edge, bool invert, Entity prefab, NetGeometryData prefabGeometryData, CompositionFlags elevationFlags)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			Curve curve = m_CurveData[edge];
			CompositionFlags compositionFlags = default(CompositionFlags);
			CompositionFlags subObjectFlags = GetSubObjectFlags(edge, curve, prefabGeometryData);
			if (m_UpgradedData.HasComponent(edge))
			{
				compositionFlags = m_UpgradedData[edge].m_Flags;
			}
			if (m_FixedData.HasComponent(edge) && m_PrefabFixedNetElements.HasBuffer(prefab))
			{
				Fixed obj = m_FixedData[edge];
				DynamicBuffer<FixedNetElement> val = m_PrefabFixedNetElements[prefab];
				if (obj.m_Index >= 0 && obj.m_Index < val.Length)
				{
					FixedNetElement fixedNetElement = val[obj.m_Index];
					compositionFlags |= fixedNetElement.m_SetState;
					elevationFlags &= ~fixedNetElement.m_UnsetState;
				}
			}
			CompositionFlags compositionFlags2 = compositionFlags | subObjectFlags | elevationFlags;
			compositionFlags2.m_General |= CompositionFlags.General.Edge;
			compositionFlags2 |= GetHandednessFlags(prefabGeometryData);
			compositionFlags2 |= GetEdgeStates(prefab, compositionFlags2);
			if (invert)
			{
				compositionFlags2 = NetCompositionHelpers.InvertCompositionFlags(compositionFlags2);
			}
			return compositionFlags2;
		}

		private CompositionFlags GetHandednessFlags(NetGeometryData prefabGeometryData)
		{
			CompositionFlags result = default(CompositionFlags);
			if ((prefabGeometryData.m_Flags & GeometryFlags.IsLefthanded) != 0 != m_LeftHandTraffic)
			{
				if ((prefabGeometryData.m_Flags & GeometryFlags.InvertCompositionHandedness) != 0)
				{
					result.m_General |= CompositionFlags.General.Invert;
				}
				if ((prefabGeometryData.m_Flags & GeometryFlags.FlipCompositionHandedness) != 0)
				{
					result.m_General |= CompositionFlags.General.Flip;
				}
			}
			return result;
		}

		private CompositionFlags GetNodeFlags(Entity edge, Entity node, Entity owner, Entity prefab, NetData prefabNetData, NetGeometryData prefabGeometryData, CompositionFlags edgeFlags, Curve curve, bool isStart, out CompositionFlags obsoleteNodeFlags, ref CompositionFlags obsoleteEdgeFlags)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d20: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d25: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0746: Unknown result type (might be due to invalid IL or missing references)
			//IL_074d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d36: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d40: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_077d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
			CompositionFlags compositionFlags = edgeFlags;
			edgeFlags.m_General &= CompositionFlags.General.Lighting | CompositionFlags.General.Elevated | CompositionFlags.General.Tunnel | CompositionFlags.General.MiddlePlatform | CompositionFlags.General.WideMedian | CompositionFlags.General.PrimaryMiddleBeautification | CompositionFlags.General.SecondaryMiddleBeautification;
			edgeFlags.m_Left &= CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered | CompositionFlags.Side.PrimaryTrack | CompositionFlags.Side.SecondaryTrack | CompositionFlags.Side.TertiaryTrack | CompositionFlags.Side.QuaternaryTrack | CompositionFlags.Side.PrimaryStop | CompositionFlags.Side.SecondaryStop | CompositionFlags.Side.PrimaryBeautification | CompositionFlags.Side.SecondaryBeautification | CompositionFlags.Side.AbruptEnd | CompositionFlags.Side.Gate | CompositionFlags.Side.WideSidewalk | CompositionFlags.Side.ParkingSpaces | CompositionFlags.Side.SoundBarrier | CompositionFlags.Side.ForbidLeftTurn | CompositionFlags.Side.ForbidRightTurn | CompositionFlags.Side.ForbidStraight;
			edgeFlags.m_Right &= CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered | CompositionFlags.Side.PrimaryTrack | CompositionFlags.Side.SecondaryTrack | CompositionFlags.Side.TertiaryTrack | CompositionFlags.Side.QuaternaryTrack | CompositionFlags.Side.PrimaryStop | CompositionFlags.Side.SecondaryStop | CompositionFlags.Side.PrimaryBeautification | CompositionFlags.Side.SecondaryBeautification | CompositionFlags.Side.AbruptEnd | CompositionFlags.Side.Gate | CompositionFlags.Side.WideSidewalk | CompositionFlags.Side.ParkingSpaces | CompositionFlags.Side.SoundBarrier | CompositionFlags.Side.ForbidLeftTurn | CompositionFlags.Side.ForbidRightTurn | CompositionFlags.Side.ForbidStraight;
			CompositionFlags handednessFlags = GetHandednessFlags(prefabGeometryData);
			obsoleteNodeFlags = default(CompositionFlags);
			handednessFlags.m_General |= CompositionFlags.General.Node;
			if (isStart)
			{
				handednessFlags.m_General ^= CompositionFlags.General.Invert;
				edgeFlags = NetCompositionHelpers.InvertCompositionFlags(edgeFlags);
			}
			handednessFlags |= GetSubObjectFlags(node, prefabNetData);
			handednessFlags |= edgeFlags;
			Edge edge2 = default(Edge);
			if (m_EdgeData.TryGetComponent(owner, ref edge2))
			{
				Curve curve2 = m_CurveData[owner];
				PrefabRef prefabRef = m_PrefabRefData[owner];
				NetData prefabNetData2 = m_PrefabNetData[prefabRef.m_Prefab];
				NetGeometryData netGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
				float3 val = (isStart ? curve.m_Bezier.a : curve.m_Bezier.d);
				float num = math.distancesq(val, curve2.m_Bezier.a);
				float num2 = math.distancesq(val, curve2.m_Bezier.d);
				Entity val2 = ((num <= num2) ? edge2.m_Start : edge2.m_End);
				handednessFlags |= GetSubObjectFlags(val2, prefabNetData2) & new CompositionFlags(CompositionFlags.General.FixedNodeSize, (CompositionFlags.Side)0u, (CompositionFlags.Side)0u);
				EdgeIterator edgeIterator = new EdgeIterator(owner, val2, m_ConnectedEdges, m_EdgeData, m_TempData, m_HiddenData);
				EdgeIteratorValue value;
				while (edgeIterator.GetNext(out value))
				{
					PrefabRef prefabRef2 = m_PrefabRefData[value.m_Edge];
					NetGeometryData netGeometryData2 = m_PrefabGeometryData[prefabRef2.m_Prefab];
					if ((netGeometryData2.m_MergeLayers & netGeometryData.m_MergeLayers) == 0)
					{
						Layer layer = netGeometryData2.m_MergeLayers | netGeometryData.m_MergeLayers;
						if (((layer & (Layer.Road | Layer.Pathway | Layer.MarkerPathway | Layer.PublicTransportRoad)) != Layer.None && (layer & (Layer.TrainTrack | Layer.SubwayTrack)) != Layer.None) || ((layer & (Layer.Road | Layer.TrainTrack | Layer.Pathway | Layer.TramTrack | Layer.SubwayTrack | Layer.MarkerPathway | Layer.PublicTransportRoad)) != Layer.None && (layer & Layer.Waterway) != Layer.None))
						{
							handednessFlags.m_General |= CompositionFlags.General.LevelCrossing;
						}
					}
				}
			}
			if ((prefabGeometryData.m_Flags & GeometryFlags.SupportRoundabout) == 0)
			{
				handednessFlags.m_General &= ~CompositionFlags.General.Roundabout;
			}
			int num3 = 0;
			int3 val3 = default(int3);
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			DynamicBuffer<NetGeometryNodeState> nodeStates = default(DynamicBuffer<NetGeometryNodeState>);
			m_PrefabGeometryNodeStates.TryGetBuffer(prefab, ref nodeStates);
			if (!isStart)
			{
				curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
			}
			FindFriendEdges(edge, node, prefabGeometryData.m_MergeLayers, curve, out var leftEdge, out var rightEdge);
			EdgeIterator edgeIterator2 = new EdgeIterator(edge, node, m_ConnectedEdges, m_EdgeData, m_TempData, m_HiddenData);
			EdgeIteratorValue value2;
			DynamicBuffer<NetGeometryNodeState> nodeStates2 = default(DynamicBuffer<NetGeometryNodeState>);
			while (edgeIterator2.GetNext(out value2))
			{
				PrefabRef prefabRef3 = m_PrefabRefData[value2.m_Edge];
				NetGeometryData prefabGeometryData2 = m_PrefabGeometryData[prefabRef3.m_Prefab];
				m_PrefabGeometryNodeStates.TryGetBuffer(prefabRef3.m_Prefab, ref nodeStates2);
				if (m_PrefabRoadData.HasComponent(prefabRef3.m_Prefab))
				{
					RoadData roadData = m_PrefabRoadData[prefabRef3.m_Prefab];
					flag3 |= (roadData.m_Flags & Game.Prefabs.RoadFlags.PreferTrafficLights) != 0;
					val3 = (((roadData.m_Flags & (Game.Prefabs.RoadFlags.DefaultIsForward | Game.Prefabs.RoadFlags.DefaultIsBackward)) == 0) ? int3.op_Increment(val3) : (val3 + math.select(new int3(0, 1, 1), new int3(1, 0, 1), value2.m_End == ((roadData.m_Flags & Game.Prefabs.RoadFlags.DefaultIsForward) != 0))));
				}
				if ((prefabGeometryData2.m_MergeLayers & prefabGeometryData.m_MergeLayers) == 0)
				{
					Layer layer2 = prefabGeometryData2.m_MergeLayers | prefabGeometryData.m_MergeLayers;
					if (((layer2 & (Layer.Road | Layer.Pathway | Layer.MarkerPathway | Layer.PublicTransportRoad)) != Layer.None && (layer2 & (Layer.TrainTrack | Layer.SubwayTrack)) != Layer.None) || ((layer2 & (Layer.Road | Layer.TrainTrack | Layer.Pathway | Layer.TramTrack | Layer.SubwayTrack | Layer.MarkerPathway | Layer.PublicTransportRoad)) != Layer.None && (layer2 & Layer.Waterway) != Layer.None))
					{
						handednessFlags.m_General |= CompositionFlags.General.LevelCrossing;
					}
					if ((layer2 & Layer.Road) != Layer.None && (layer2 & (Layer.Pathway | Layer.MarkerPathway)) != Layer.None)
					{
						flag2 = true;
					}
					continue;
				}
				bool flag9 = value2.m_Edge == leftEdge;
				bool flag10 = value2.m_Edge == rightEdge;
				Edge edge3 = m_EdgeData[value2.m_Edge];
				CompositionFlags elevationFlags = GetElevationFlags(value2.m_Edge, edge3.m_Start, edge3.m_End, prefabGeometryData2);
				CompositionFlags flags = elevationFlags;
				if (value2.m_End)
				{
					flags = NetCompositionHelpers.InvertCompositionFlags(flags);
				}
				CompositionFlags edgeFlags2 = GetEdgeFlags(value2.m_Edge, value2.m_End, prefabRef3.m_Prefab, prefabGeometryData2, elevationFlags);
				if ((edgeFlags.m_General & (CompositionFlags.General.Elevated | CompositionFlags.General.Tunnel)) != 0 || (edgeFlags.m_Left & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered | CompositionFlags.Side.SoundBarrier)) != 0 || (edgeFlags.m_Right & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered | CompositionFlags.Side.SoundBarrier)) != 0)
				{
					if ((edgeFlags.m_General & CompositionFlags.General.Elevated) != 0)
					{
						if ((flags.m_General & CompositionFlags.General.Elevated) == 0)
						{
							if (flag9)
							{
								if ((flags.m_Left & CompositionFlags.Side.Raised) != 0)
								{
									handednessFlags.m_Left |= CompositionFlags.Side.LowTransition;
								}
								else
								{
									handednessFlags.m_Left |= CompositionFlags.Side.HighTransition;
								}
							}
							if (flag10)
							{
								if ((flags.m_Right & CompositionFlags.Side.Raised) != 0)
								{
									handednessFlags.m_Right |= CompositionFlags.Side.LowTransition;
								}
								else
								{
									handednessFlags.m_Right |= CompositionFlags.Side.HighTransition;
								}
							}
						}
					}
					else if ((edgeFlags.m_General & CompositionFlags.General.Tunnel) != 0)
					{
						if ((flags.m_General & CompositionFlags.General.Tunnel) == 0)
						{
							if (flag9)
							{
								if ((flags.m_Left & CompositionFlags.Side.Lowered) != 0)
								{
									handednessFlags.m_Left |= CompositionFlags.Side.LowTransition;
								}
								else
								{
									handednessFlags.m_Left |= CompositionFlags.Side.HighTransition;
								}
							}
							if (flag10)
							{
								if ((flags.m_Right & CompositionFlags.Side.Lowered) != 0)
								{
									handednessFlags.m_Right |= CompositionFlags.Side.LowTransition;
								}
								else
								{
									handednessFlags.m_Right |= CompositionFlags.Side.HighTransition;
								}
							}
						}
					}
					else
					{
						if (flag9)
						{
							if ((edgeFlags.m_Left & CompositionFlags.Side.Raised) != 0)
							{
								if ((flags.m_Left & CompositionFlags.Side.Raised) == 0 && (flags.m_General & CompositionFlags.General.Elevated) == 0)
								{
									handednessFlags.m_Left |= CompositionFlags.Side.LowTransition;
								}
							}
							else if ((edgeFlags.m_Left & CompositionFlags.Side.Lowered) != 0)
							{
								if ((flags.m_Left & CompositionFlags.Side.Lowered) == 0 && (flags.m_General & CompositionFlags.General.Tunnel) == 0)
								{
									handednessFlags.m_Left |= CompositionFlags.Side.LowTransition;
								}
							}
							else if ((edgeFlags.m_Left & CompositionFlags.Side.SoundBarrier) != 0 && (edgeFlags2.m_Left & CompositionFlags.Side.SoundBarrier) == 0)
							{
								handednessFlags.m_Left |= CompositionFlags.Side.LowTransition;
							}
						}
						if (flag10)
						{
							if ((edgeFlags.m_Right & CompositionFlags.Side.Raised) != 0)
							{
								if ((flags.m_Right & CompositionFlags.Side.Raised) == 0 && (flags.m_General & CompositionFlags.General.Elevated) == 0)
								{
									handednessFlags.m_Right |= CompositionFlags.Side.LowTransition;
								}
							}
							else if ((edgeFlags.m_Right & CompositionFlags.Side.Lowered) != 0)
							{
								if ((flags.m_Right & CompositionFlags.Side.Lowered) == 0 && (flags.m_General & CompositionFlags.General.Tunnel) == 0)
								{
									handednessFlags.m_Right |= CompositionFlags.Side.LowTransition;
								}
							}
							else if ((edgeFlags.m_Right & CompositionFlags.Side.SoundBarrier) != 0 && (edgeFlags2.m_Right & CompositionFlags.Side.SoundBarrier) == 0)
							{
								handednessFlags.m_Right |= CompositionFlags.Side.LowTransition;
							}
						}
					}
				}
				if ((edgeFlags2.m_Left & CompositionFlags.Side.RemoveCrosswalk) != 0)
				{
					flag7 = true;
				}
				else
				{
					if (((edgeFlags2.m_Left | edgeFlags2.m_Right) & CompositionFlags.Side.Sidewalk) != 0 && (edgeFlags2.m_General & CompositionFlags.General.MiddlePlatform) != 0)
					{
						flag5 = true;
					}
					if ((edgeFlags2.m_Left & CompositionFlags.Side.AddCrosswalk) != 0)
					{
						flag5 = true;
						flag8 = true;
					}
					flag6 = true;
				}
				if (((edgeFlags2.m_Left | edgeFlags2.m_Right) & CompositionFlags.Side.ParkingSpaces) != 0)
				{
					flag4 = true;
				}
				if (prefabGeometryData.m_StyleType != prefabGeometryData2.m_StyleType)
				{
					handednessFlags.m_General |= CompositionFlags.General.StyleBreak;
				}
				if (nodeStates.IsCreated && nodeStates.Length != 0)
				{
					handednessFlags |= GetNodeStates(nodeStates, compositionFlags, edgeFlags2, flag9, flag10);
				}
				if (nodeStates2.IsCreated && nodeStates2.Length != 0)
				{
					handednessFlags |= NetCompositionHelpers.InvertCompositionFlags(GetNodeStates(nodeStates2, edgeFlags2, compositionFlags, flag10, flag9));
				}
				if (value2.m_Edge != edge)
				{
					if (prefabRef3.m_Prefab != prefab)
					{
						flag = false;
					}
					num3++;
				}
			}
			if ((handednessFlags.m_General & (CompositionFlags.General.Elevated | CompositionFlags.General.Tunnel)) != 0)
			{
				bool num4 = (handednessFlags.m_Left & (CompositionFlags.Side.LowTransition | CompositionFlags.Side.HighTransition)) != 0;
				bool flag11 = (handednessFlags.m_Right & (CompositionFlags.Side.LowTransition | CompositionFlags.Side.HighTransition)) != 0;
				if (num4 && !flag11)
				{
					handednessFlags.m_Right |= CompositionFlags.Side.LowTransition;
				}
				if (!num4 && flag11)
				{
					handednessFlags.m_Left |= CompositionFlags.Side.LowTransition;
				}
			}
			if (flag2 && (prefabGeometryData.m_MergeLayers & (Layer.Pathway | Layer.MarkerPathway)) != Layer.None)
			{
				return handednessFlags;
			}
			flag = flag && num3 == 1;
			flag2 = flag2 && num3 >= 1;
			flag6 = flag6 && flag2;
			if (num3 >= 2)
			{
				handednessFlags.m_General |= CompositionFlags.General.Intersection;
				if ((handednessFlags.m_Left & handednessFlags.m_Right & CompositionFlags.Side.Sidewalk) != 0 || flag2)
				{
					handednessFlags.m_General |= CompositionFlags.General.Crosswalk;
				}
			}
			else
			{
				switch (num3)
				{
				case 1:
					if (flag)
					{
						if (flag4 && ((compositionFlags.m_Left | compositionFlags.m_Right) & CompositionFlags.Side.ParkingSpaces) != 0 && (handednessFlags.m_General & CompositionFlags.General.LevelCrossing) == 0)
						{
							handednessFlags.m_General |= CompositionFlags.General.Crosswalk;
						}
					}
					else
					{
						handednessFlags.m_General |= CompositionFlags.General.Intersection;
					}
					if (flag2)
					{
						handednessFlags.m_General |= CompositionFlags.General.Crosswalk;
					}
					break;
				case 0:
					handednessFlags.m_General |= CompositionFlags.General.DeadEnd;
					flag5 = false;
					if ((handednessFlags.m_General & CompositionFlags.General.Invert) != 0)
					{
						obsoleteEdgeFlags.m_Left |= compositionFlags.m_Left & (CompositionFlags.Side.AddCrosswalk | CompositionFlags.Side.RemoveCrosswalk);
					}
					else
					{
						obsoleteEdgeFlags.m_Right |= compositionFlags.m_Right & (CompositionFlags.Side.AddCrosswalk | CompositionFlags.Side.RemoveCrosswalk);
					}
					break;
				}
			}
			if (num3 != 0)
			{
				if (((compositionFlags.m_Left | compositionFlags.m_Right) & CompositionFlags.Side.Sidewalk) != 0 && (compositionFlags.m_General & CompositionFlags.General.MiddlePlatform) != 0)
				{
					handednessFlags.m_General |= CompositionFlags.General.Crosswalk;
				}
				if ((handednessFlags.m_General & (CompositionFlags.General.Intersection | CompositionFlags.General.MedianBreak)) == 0)
				{
					if ((handednessFlags.m_General & CompositionFlags.General.Crosswalk) != 0)
					{
						if ((handednessFlags.m_General & CompositionFlags.General.Invert) != 0)
						{
							if ((compositionFlags.m_Left & CompositionFlags.Side.AddCrosswalk) != 0 || flag8)
							{
								obsoleteEdgeFlags.m_Left |= compositionFlags.m_Left & (CompositionFlags.Side.AddCrosswalk | CompositionFlags.Side.RemoveCrosswalk);
							}
							else if ((compositionFlags.m_Left & CompositionFlags.Side.RemoveCrosswalk) != 0 || flag7)
							{
								handednessFlags.m_General &= ~CompositionFlags.General.Crosswalk;
								flag6 = false;
							}
						}
						else if ((compositionFlags.m_Right & CompositionFlags.Side.AddCrosswalk) != 0 || flag8)
						{
							obsoleteEdgeFlags.m_Right |= compositionFlags.m_Right & (CompositionFlags.Side.AddCrosswalk | CompositionFlags.Side.RemoveCrosswalk);
						}
						else if ((compositionFlags.m_Right & CompositionFlags.Side.RemoveCrosswalk) != 0 || flag7)
						{
							handednessFlags.m_General &= ~CompositionFlags.General.Crosswalk;
							flag6 = false;
						}
					}
					else if ((handednessFlags.m_General & CompositionFlags.General.Invert) != 0)
					{
						if ((compositionFlags.m_Left & CompositionFlags.Side.RemoveCrosswalk) != 0 || flag7)
						{
							obsoleteEdgeFlags.m_Left |= compositionFlags.m_Left & (CompositionFlags.Side.AddCrosswalk | CompositionFlags.Side.RemoveCrosswalk);
						}
						else if ((compositionFlags.m_Left & CompositionFlags.Side.AddCrosswalk) != 0 || flag8)
						{
							handednessFlags.m_General |= CompositionFlags.General.Crosswalk;
						}
					}
					else if ((compositionFlags.m_Right & CompositionFlags.Side.RemoveCrosswalk) != 0 || flag7)
					{
						obsoleteEdgeFlags.m_Right |= compositionFlags.m_Right & (CompositionFlags.Side.AddCrosswalk | CompositionFlags.Side.RemoveCrosswalk);
					}
					else if ((compositionFlags.m_Right & CompositionFlags.Side.AddCrosswalk) != 0 || flag8)
					{
						handednessFlags.m_General |= CompositionFlags.General.Crosswalk;
					}
				}
				else if ((handednessFlags.m_General & CompositionFlags.General.Crosswalk) != 0)
				{
					if ((handednessFlags.m_General & CompositionFlags.General.Invert) != 0)
					{
						if ((compositionFlags.m_Left & CompositionFlags.Side.AddCrosswalk) != 0)
						{
							obsoleteEdgeFlags.m_Left |= CompositionFlags.Side.AddCrosswalk;
						}
						if ((compositionFlags.m_Left & CompositionFlags.Side.RemoveCrosswalk) != 0)
						{
							handednessFlags.m_General &= ~CompositionFlags.General.Crosswalk;
						}
					}
					else
					{
						if ((compositionFlags.m_Right & CompositionFlags.Side.AddCrosswalk) != 0)
						{
							obsoleteEdgeFlags.m_Right |= CompositionFlags.Side.AddCrosswalk;
						}
						if ((compositionFlags.m_Right & CompositionFlags.Side.RemoveCrosswalk) != 0)
						{
							handednessFlags.m_General &= ~CompositionFlags.General.Crosswalk;
						}
					}
				}
				else if ((handednessFlags.m_General & CompositionFlags.General.Invert) != 0)
				{
					if ((compositionFlags.m_Left & CompositionFlags.Side.RemoveCrosswalk) != 0)
					{
						obsoleteEdgeFlags.m_Left |= CompositionFlags.Side.RemoveCrosswalk;
					}
					if ((compositionFlags.m_Left & CompositionFlags.Side.AddCrosswalk) != 0)
					{
						handednessFlags.m_General |= CompositionFlags.General.Crosswalk;
					}
				}
				else
				{
					if ((compositionFlags.m_Right & CompositionFlags.Side.RemoveCrosswalk) != 0)
					{
						obsoleteEdgeFlags.m_Right |= CompositionFlags.Side.RemoveCrosswalk;
					}
					if ((compositionFlags.m_Right & CompositionFlags.Side.AddCrosswalk) != 0)
					{
						handednessFlags.m_General |= CompositionFlags.General.Crosswalk;
					}
				}
				if ((handednessFlags.m_General & CompositionFlags.General.MedianBreak) != 0 && ((handednessFlags.m_General & CompositionFlags.General.Crosswalk) != 0 || flag5))
				{
					handednessFlags.m_General |= CompositionFlags.General.Intersection;
				}
			}
			CompositionFlags compositionFlags2 = default(CompositionFlags);
			if (m_UpgradedData.HasComponent(node))
			{
				Upgraded upgraded = m_UpgradedData[node];
				compositionFlags2 = upgraded.m_Flags;
				upgraded.m_Flags.m_General &= ~CompositionFlags.General.RemoveTrafficLights;
				handednessFlags |= upgraded.m_Flags;
			}
			if ((handednessFlags.m_General & (CompositionFlags.General.Roundabout | CompositionFlags.General.LevelCrossing)) == 0 && ((handednessFlags.m_General & (CompositionFlags.General.Intersection | CompositionFlags.General.Crosswalk)) != 0 || flag2 || flag5))
			{
				if (flag3 && math.all(val3 >= new int3(1, 1, 2)) && (math.all(val3 >= new int3(2, 1, 3)) || (handednessFlags.m_General & CompositionFlags.General.Crosswalk) != 0 || flag5 || flag6))
				{
					if ((compositionFlags2.m_General & (CompositionFlags.General.RemoveTrafficLights | CompositionFlags.General.AllWayStop)) != 0)
					{
						handednessFlags.m_General &= ~CompositionFlags.General.TrafficLights;
					}
					else
					{
						handednessFlags.m_General |= CompositionFlags.General.TrafficLights;
					}
					obsoleteNodeFlags.m_General |= compositionFlags2.m_General & CompositionFlags.General.TrafficLights;
				}
				else
				{
					obsoleteNodeFlags.m_General |= compositionFlags2.m_General & CompositionFlags.General.RemoveTrafficLights;
				}
			}
			else
			{
				handednessFlags.m_General &= ~(CompositionFlags.General.TrafficLights | CompositionFlags.General.AllWayStop);
				obsoleteNodeFlags.m_General |= compositionFlags2.m_General & (CompositionFlags.General.TrafficLights | CompositionFlags.General.RemoveTrafficLights | CompositionFlags.General.AllWayStop);
			}
			if ((handednessFlags.m_General & (CompositionFlags.General.Intersection | CompositionFlags.General.Roundabout)) != CompositionFlags.General.Intersection)
			{
				handednessFlags.m_Left &= ~(CompositionFlags.Side.ForbidLeftTurn | CompositionFlags.Side.ForbidRightTurn | CompositionFlags.Side.ForbidStraight);
				handednessFlags.m_Right &= ~(CompositionFlags.Side.ForbidLeftTurn | CompositionFlags.Side.ForbidRightTurn | CompositionFlags.Side.ForbidStraight);
				if ((handednessFlags.m_General & CompositionFlags.General.Invert) != 0)
				{
					obsoleteEdgeFlags.m_Left |= compositionFlags.m_Left & (CompositionFlags.Side.ForbidLeftTurn | CompositionFlags.Side.ForbidRightTurn | CompositionFlags.Side.ForbidStraight);
				}
				else
				{
					obsoleteEdgeFlags.m_Right |= compositionFlags.m_Right & (CompositionFlags.Side.ForbidLeftTurn | CompositionFlags.Side.ForbidRightTurn | CompositionFlags.Side.ForbidStraight);
				}
			}
			return handednessFlags;
		}

		private void FindFriendEdges(Entity edge, Entity node, Layer mergeLayers, Curve curve, out Entity leftEdge, out Entity rightEdge)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			float2 val = float2.op_Implicit(0f);
			int num = 0;
			leftEdge = edge;
			rightEdge = edge;
			EdgeIterator edgeIterator = new EdgeIterator(edge, node, m_ConnectedEdges, m_EdgeData, m_TempData, m_HiddenData);
			EdgeIteratorValue value;
			while (edgeIterator.GetNext(out value))
			{
				if (!(value.m_Edge == edge))
				{
					PrefabRef prefabRef = m_PrefabRefData[value.m_Edge];
					if ((m_PrefabGeometryData[prefabRef.m_Prefab].m_MergeLayers & mergeLayers) != Layer.None)
					{
						Curve curve2 = m_CurveData[value.m_Edge];
						val += math.select(((float3)(ref curve2.m_Bezier.a)).xz, ((float3)(ref curve2.m_Bezier.d)).xz, value.m_End);
						num++;
						leftEdge = value.m_Edge;
						rightEdge = value.m_Edge;
					}
				}
			}
			if (num <= 1)
			{
				return;
			}
			val /= (float)num;
			float3 val2 = MathUtils.Position(curve.m_Bezier, 0.5f);
			float2 val3 = math.normalizesafe(((float3)(ref val2)).xz - val, default(float2));
			float2 val4 = MathUtils.Right(val3);
			float num2 = -2f;
			float num3 = 2f;
			edgeIterator = new EdgeIterator(edge, node, m_ConnectedEdges, m_EdgeData, m_TempData, m_HiddenData);
			EdgeIteratorValue value2;
			while (edgeIterator.GetNext(out value2))
			{
				if (value2.m_Edge == edge)
				{
					continue;
				}
				PrefabRef prefabRef2 = m_PrefabRefData[value2.m_Edge];
				if ((m_PrefabGeometryData[prefabRef2.m_Prefab].m_MergeLayers & mergeLayers) != Layer.None)
				{
					Curve curve3 = m_CurveData[value2.m_Edge];
					if (value2.m_End)
					{
						curve3.m_Bezier = MathUtils.Invert(curve3.m_Bezier);
					}
					val2 = MathUtils.Position(curve3.m_Bezier, 0.5f);
					float2 val5 = math.normalizesafe(((float3)(ref val2)).xz - val, default(float2));
					float num4;
					if (math.dot(val3, val5) < 0f)
					{
						num4 = math.dot(val4, val5) * 0.5f;
					}
					else
					{
						float num5 = math.dot(val4, val5);
						num4 = math.select(-1f, 1f, num5 >= 0f) - num5 * 0.5f;
					}
					if (num4 > num2)
					{
						num2 = num4;
						leftEdge = value2.m_Edge;
					}
					if (num4 < num3)
					{
						num3 = num4;
						rightEdge = value2.m_Edge;
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct CreatedCompositionKey : IEquatable<CreatedCompositionKey>
	{
		public Entity m_Prefab;

		public CompositionFlags m_Flags;

		public bool Equals(CreatedCompositionKey other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (((Entity)(ref m_Prefab)).Equals(other.m_Prefab))
			{
				return m_Flags == other.m_Flags;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Prefab)/*cast due to .constrained prefix*/).GetHashCode() * 31 + m_Flags.GetHashCode();
		}
	}

	[BurstCompile]
	private struct CreateCompositionJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public ComponentLookup<NetTerrainData> m_PrefabTerrainData;

		[ReadOnly]
		public BufferLookup<SubReplacement> m_SubReplacements;

		[ReadOnly]
		public BufferLookup<NetGeometrySection> m_PrefabGeometrySections;

		[ReadOnly]
		public BufferLookup<NetSubSection> m_PrefabSubSections;

		[ReadOnly]
		public BufferLookup<NetSectionPiece> m_PrefabSectionPieces;

		public ComponentLookup<Upgraded> m_UpgradedData;

		public ComponentLookup<Temp> m_TempData;

		public NativeQueue<CompositionCreateInfo> m_CompositionCreateQueue;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			int count = m_CompositionCreateQueue.Count;
			if (count == 0)
			{
				return;
			}
			NativeParallelHashMap<CreatedCompositionKey, Entity> createdCompositions = default(NativeParallelHashMap<CreatedCompositionKey, Entity>);
			Temp temp = default(Temp);
			Upgraded upgraded2 = default(Upgraded);
			Temp temp2 = default(Temp);
			Upgraded upgraded4 = default(Upgraded);
			Temp temp3 = default(Temp);
			Upgraded upgraded6 = default(Upgraded);
			for (int i = 0; i < count; i++)
			{
				CompositionCreateInfo compositionCreateInfo = m_CompositionCreateQueue.Dequeue();
				if (compositionCreateInfo.m_CompositionData.m_Edge == Entity.Null)
				{
					compositionCreateInfo.m_CompositionData.m_Edge = GetOrCreateComposition(compositionCreateInfo.m_Prefab, compositionCreateInfo.m_EdgeFlags, ref createdCompositions);
				}
				if (m_EdgeData.HasComponent(compositionCreateInfo.m_Entity))
				{
					Edge edge = m_EdgeData[compositionCreateInfo.m_Entity];
					if (compositionCreateInfo.m_CompositionData.m_StartNode == Entity.Null)
					{
						compositionCreateInfo.m_CompositionData.m_StartNode = GetOrCreateComposition(compositionCreateInfo.m_Prefab, compositionCreateInfo.m_StartFlags, ref createdCompositions);
					}
					if (compositionCreateInfo.m_CompositionData.m_EndNode == Entity.Null)
					{
						compositionCreateInfo.m_CompositionData.m_EndNode = GetOrCreateComposition(compositionCreateInfo.m_Prefab, compositionCreateInfo.m_EndFlags, ref createdCompositions);
					}
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Composition>(compositionCreateInfo.m_Entity, compositionCreateInfo.m_CompositionData);
					if (compositionCreateInfo.m_ObsoleteStartFlags != default(CompositionFlags))
					{
						Upgraded upgraded = m_UpgradedData[edge.m_Start];
						if (upgraded.m_Flags != default(CompositionFlags))
						{
							upgraded.m_Flags &= ~compositionCreateInfo.m_ObsoleteStartFlags;
							m_UpgradedData[edge.m_Start] = upgraded;
							if (upgraded.m_Flags == default(CompositionFlags))
							{
								((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Upgraded>(edge.m_Start);
							}
							if (m_TempData.TryGetComponent(edge.m_Start, ref temp) && temp.m_Original != Entity.Null && (temp.m_Flags & (TempFlags.Upgrade | TempFlags.Parent)) == (TempFlags.Upgrade | TempFlags.Parent))
							{
								m_UpgradedData.TryGetComponent(temp.m_Original, ref upgraded2);
								if (upgraded.m_Flags == upgraded2.m_Flags)
								{
									temp.m_Flags &= ~(TempFlags.Upgrade | TempFlags.Parent);
									m_TempData[edge.m_Start] = temp;
								}
							}
						}
					}
					if (compositionCreateInfo.m_ObsoleteEndFlags != default(CompositionFlags))
					{
						Upgraded upgraded3 = m_UpgradedData[edge.m_End];
						if (upgraded3.m_Flags != default(CompositionFlags))
						{
							upgraded3.m_Flags &= ~compositionCreateInfo.m_ObsoleteEndFlags;
							m_UpgradedData[edge.m_End] = upgraded3;
							if (upgraded3.m_Flags == default(CompositionFlags))
							{
								((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Upgraded>(edge.m_End);
							}
							if (m_TempData.TryGetComponent(edge.m_End, ref temp2) && temp2.m_Original != Entity.Null && (temp2.m_Flags & (TempFlags.Upgrade | TempFlags.Parent)) == (TempFlags.Upgrade | TempFlags.Parent))
							{
								m_UpgradedData.TryGetComponent(temp2.m_Original, ref upgraded4);
								if (upgraded3.m_Flags == upgraded4.m_Flags)
								{
									temp2.m_Flags &= ~(TempFlags.Upgrade | TempFlags.Parent);
									m_TempData[edge.m_End] = temp2;
								}
							}
						}
					}
				}
				else
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Orphan>(compositionCreateInfo.m_Entity, new Orphan
					{
						m_Composition = compositionCreateInfo.m_CompositionData.m_Edge
					});
				}
				if (!(compositionCreateInfo.m_ObsoleteEdgeFlags != default(CompositionFlags)))
				{
					continue;
				}
				Upgraded upgraded5 = m_UpgradedData[compositionCreateInfo.m_Entity];
				if (!(upgraded5.m_Flags != default(CompositionFlags)))
				{
					continue;
				}
				upgraded5.m_Flags &= ~compositionCreateInfo.m_ObsoleteEdgeFlags;
				m_UpgradedData[compositionCreateInfo.m_Entity] = upgraded5;
				if (upgraded5.m_Flags == default(CompositionFlags))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Upgraded>(compositionCreateInfo.m_Entity);
				}
				if (m_TempData.TryGetComponent(compositionCreateInfo.m_Entity, ref temp3) && temp3.m_Original != Entity.Null && (temp3.m_Flags & (TempFlags.Upgrade | TempFlags.Parent)) == (TempFlags.Upgrade | TempFlags.Parent))
				{
					m_UpgradedData.TryGetComponent(temp3.m_Original, ref upgraded6);
					if (upgraded5.m_Flags == upgraded6.m_Flags && EqualSubReplacements(compositionCreateInfo.m_Entity, temp3.m_Original))
					{
						temp3.m_Flags &= ~(TempFlags.Upgrade | TempFlags.Parent);
						m_TempData[compositionCreateInfo.m_Entity] = temp3;
					}
				}
			}
			if (createdCompositions.IsCreated)
			{
				createdCompositions.Dispose();
			}
		}

		private bool EqualSubReplacements(Entity entity1, Entity entity2)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<SubReplacement> val = default(DynamicBuffer<SubReplacement>);
			bool flag = m_SubReplacements.TryGetBuffer(entity1, ref val);
			DynamicBuffer<SubReplacement> val2 = default(DynamicBuffer<SubReplacement>);
			bool flag2 = m_SubReplacements.TryGetBuffer(entity2, ref val2);
			if (flag != flag2)
			{
				return false;
			}
			if (!flag)
			{
				return true;
			}
			if (val.Length != val2.Length)
			{
				return false;
			}
			for (int i = 0; i < val.Length; i++)
			{
				SubReplacement subReplacement = val[i];
				bool flag3 = false;
				for (int j = 0; j < val2.Length; j++)
				{
					SubReplacement other = val2[j];
					if (subReplacement.Equals(other))
					{
						flag3 = true;
						break;
					}
				}
				if (!flag3)
				{
					return false;
				}
			}
			return true;
		}

		private Entity GetOrCreateComposition(Entity prefab, CompositionFlags flags, ref NativeParallelHashMap<CreatedCompositionKey, Entity> createdCompositions)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			CreatedCompositionKey createdCompositionKey = new CreatedCompositionKey
			{
				m_Prefab = prefab,
				m_Flags = flags
			};
			if (createdCompositions.IsCreated)
			{
				Entity result = default(Entity);
				if (createdCompositions.TryGetValue(createdCompositionKey, ref result))
				{
					return result;
				}
			}
			else
			{
				createdCompositions = new NativeParallelHashMap<CreatedCompositionKey, Entity>(50, AllocatorHandle.op_Implicit((Allocator)2));
			}
			Entity val = CreateComposition(prefab, flags);
			createdCompositions.Add(createdCompositionKey, val);
			return val;
		}

		private Entity CreateComposition(Entity prefab, CompositionFlags mask)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			NetGeometryData netGeometryData = m_PrefabGeometryData[prefab];
			DynamicBuffer<NetGeometrySection> val = m_PrefabGeometrySections[prefab];
			Entity val2 = (((mask.m_General & CompositionFlags.General.Node) == 0) ? ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(netGeometryData.m_EdgeCompositionArchetype) : ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(netGeometryData.m_NodeCompositionArchetype));
			((EntityCommandBuffer)(ref m_CommandBuffer)).AppendToBuffer<NetGeometryComposition>(prefab, new NetGeometryComposition
			{
				m_Composition = val2,
				m_Mask = mask
			});
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val2, new PrefabRef(prefab));
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<NetCompositionData>(val2, new NetCompositionData
			{
				m_Flags = mask
			});
			DynamicBuffer<NetCompositionPiece> val3 = ((EntityCommandBuffer)(ref m_CommandBuffer)).SetBuffer<NetCompositionPiece>(val2);
			NativeList<NetCompositionPiece> resultBuffer = default(NativeList<NetCompositionPiece>);
			resultBuffer._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
			NetCompositionHelpers.GetCompositionPieces(resultBuffer, val.AsNativeArray(), mask, m_PrefabSubSections, m_PrefabSectionPieces);
			val3.CopyFrom(resultBuffer.AsArray());
			for (int i = 0; i < resultBuffer.Length; i++)
			{
				NetCompositionPiece netCompositionPiece = resultBuffer[i];
				if (m_PrefabTerrainData.HasComponent(netCompositionPiece.m_Piece))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<TerrainComposition>(val2, default(TerrainComposition));
					break;
				}
			}
			resultBuffer.Dispose();
			return val2;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Edge> __Game_Net_Edge_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Upgraded> __Game_Net_Upgraded_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Fixed> __Game_Net_Fixed_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Composition> __Game_Net_Composition_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Orphan> __Game_Net_Orphan_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Upgraded> __Game_Net_Upgraded_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Fixed> __Game_Net_Fixed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetObjectData> __Game_Prefabs_NetObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> __Game_Prefabs_PlaceableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RoadData> __Game_Prefabs_RoadData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetGeometryComposition> __Game_Prefabs_NetGeometryComposition_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetGeometryEdgeState> __Game_Prefabs_NetGeometryEdgeState_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetGeometryNodeState> __Game_Prefabs_NetGeometryNodeState_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<FixedNetElement> __Game_Prefabs_FixedNetElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<NetTerrainData> __Game_Prefabs_NetTerrainData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubReplacement> __Game_Net_SubReplacement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetGeometrySection> __Game_Prefabs_NetGeometrySection_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetSubSection> __Game_Prefabs_NetSubSection_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetSectionPiece> __Game_Prefabs_NetSectionPiece_RO_BufferLookup;

		public ComponentLookup<Upgraded> __Game_Net_Upgraded_RW_ComponentLookup;

		public ComponentLookup<Temp> __Game_Tools_Temp_RW_ComponentLookup;

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
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_Edge_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Edge>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Net_Upgraded_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Upgraded>(true);
			__Game_Net_Fixed_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Fixed>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Net_Composition_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Composition>(false);
			__Game_Net_Orphan_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Orphan>(false);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Upgraded_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Upgraded>(true);
			__Game_Net_Fixed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Fixed>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Elevation>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_NetObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetObjectData>(true);
			__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableObjectData>(true);
			__Game_Prefabs_RoadData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RoadData>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Prefabs_NetGeometryComposition_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetGeometryComposition>(true);
			__Game_Prefabs_NetGeometryEdgeState_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetGeometryEdgeState>(true);
			__Game_Prefabs_NetGeometryNodeState_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetGeometryNodeState>(true);
			__Game_Prefabs_FixedNetElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<FixedNetElement>(true);
			__Game_Prefabs_NetTerrainData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetTerrainData>(true);
			__Game_Net_SubReplacement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubReplacement>(true);
			__Game_Prefabs_NetGeometrySection_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetGeometrySection>(true);
			__Game_Prefabs_NetSubSection_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetSubSection>(true);
			__Game_Prefabs_NetSectionPiece_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetSectionPiece>(true);
			__Game_Net_Upgraded_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Upgraded>(false);
			__Game_Tools_Temp_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(false);
		}
	}

	private NetCompositionSystem m_NetCompositionSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private ModificationBarrier3 m_ModificationBarrier;

	private EntityQuery m_UpdatedQuery;

	private EntityQuery m_AllQuery;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_NetCompositionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NetCompositionSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier3>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Updated>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Composition>(),
			ComponentType.ReadWrite<Orphan>()
		};
		array[0] = val;
		m_UpdatedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Composition>(),
			ComponentType.ReadWrite<Orphan>()
		};
		array2[0] = val;
		m_AllQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		m_Loaded = true;
	}

	private bool GetLoaded()
	{
		if (m_Loaded)
		{
			m_Loaded = false;
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val = (GetLoaded() ? m_AllQuery : m_UpdatedQuery);
		if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
		{
			NativeQueue<CompositionCreateInfo> compositionCreateQueue = default(NativeQueue<CompositionCreateInfo>);
			compositionCreateQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			SelectCompositionJob selectCompositionJob = new SelectCompositionJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UpgradedType = InternalCompilerInterface.GetComponentTypeHandle<Upgraded>(ref __TypeHandle.__Game_Net_Upgraded_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_FixedType = InternalCompilerInterface.GetComponentTypeHandle<Fixed>(ref __TypeHandle.__Game_Net_Fixed_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CompositionType = InternalCompilerInterface.GetComponentTypeHandle<Composition>(ref __TypeHandle.__Game_Net_Composition_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OrphanType = InternalCompilerInterface.GetComponentTypeHandle<Orphan>(ref __TypeHandle.__Game_Net_Orphan_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_UpgradedData = InternalCompilerInterface.GetComponentLookup<Upgraded>(ref __TypeHandle.__Game_Net_Upgraded_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_FixedData = InternalCompilerInterface.GetComponentLookup<Fixed>(ref __TypeHandle.__Game_Net_Fixed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElevationData = InternalCompilerInterface.GetComponentLookup<Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AttachedData = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetObjectData = InternalCompilerInterface.GetComponentLookup<NetObjectData>(ref __TypeHandle.__Game_Prefabs_NetObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabPlaceableObjectData = InternalCompilerInterface.GetComponentLookup<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRoadData = InternalCompilerInterface.GetComponentLookup<RoadData>(ref __TypeHandle.__Game_Prefabs_RoadData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabGeometryCompositions = InternalCompilerInterface.GetBufferLookup<NetGeometryComposition>(ref __TypeHandle.__Game_Prefabs_NetGeometryComposition_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabGeometryEdgeStates = InternalCompilerInterface.GetBufferLookup<NetGeometryEdgeState>(ref __TypeHandle.__Game_Prefabs_NetGeometryEdgeState_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabGeometryNodeStates = InternalCompilerInterface.GetBufferLookup<NetGeometryNodeState>(ref __TypeHandle.__Game_Prefabs_NetGeometryNodeState_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabFixedNetElements = InternalCompilerInterface.GetBufferLookup<FixedNetElement>(ref __TypeHandle.__Game_Prefabs_FixedNetElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LeftHandTraffic = m_CityConfigurationSystem.leftHandTraffic,
				m_CompositionCreateQueue = compositionCreateQueue.AsParallelWriter()
			};
			CreateCompositionJob obj = new CreateCompositionJob
			{
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabTerrainData = InternalCompilerInterface.GetComponentLookup<NetTerrainData>(ref __TypeHandle.__Game_Prefabs_NetTerrainData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubReplacements = InternalCompilerInterface.GetBufferLookup<SubReplacement>(ref __TypeHandle.__Game_Net_SubReplacement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabGeometrySections = InternalCompilerInterface.GetBufferLookup<NetGeometrySection>(ref __TypeHandle.__Game_Prefabs_NetGeometrySection_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabSubSections = InternalCompilerInterface.GetBufferLookup<NetSubSection>(ref __TypeHandle.__Game_Prefabs_NetSubSection_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabSectionPieces = InternalCompilerInterface.GetBufferLookup<NetSectionPiece>(ref __TypeHandle.__Game_Prefabs_NetSectionPiece_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_UpgradedData = InternalCompilerInterface.GetComponentLookup<Upgraded>(ref __TypeHandle.__Game_Net_Upgraded_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CompositionCreateQueue = compositionCreateQueue,
				m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
			};
			JobHandle val2 = JobChunkExtensions.ScheduleParallel<SelectCompositionJob>(selectCompositionJob, val, ((SystemBase)this).Dependency);
			JobHandle val3 = IJobExtensions.Schedule<CreateCompositionJob>(obj, val2);
			compositionCreateQueue.Dispose(val3);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val3);
			((SystemBase)this).Dependency = val3;
		}
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
	public CompositionSelectSystem()
	{
	}
}
