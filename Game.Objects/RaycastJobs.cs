using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Objects;

public static class RaycastJobs
{
	[BurstCompile]
	public struct RaycastStaticObjectsJob : IJobParallelForDefer
	{
		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public bool m_LeftHandTraffic;

		[ReadOnly]
		public NativeArray<RaycastInput> m_Input;

		[ReadOnly]
		public NativeArray<RaycastSystem.EntityResult> m_Objects;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeArray<RaycastResult> m_TerrainResults;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Placeholder> m_PlaceholderData;

		[ReadOnly]
		public ComponentLookup<Attachment> m_AttachmentData;

		[ReadOnly]
		public ComponentLookup<Tree> m_TreeData;

		[ReadOnly]
		public ComponentLookup<NetObject> m_NetObjectData;

		[ReadOnly]
		public ComponentLookup<Quantity> m_QuantityData;

		[ReadOnly]
		public ComponentLookup<Stack> m_StackData;

		[ReadOnly]
		public ComponentLookup<Secondary> m_SecondaryData;

		[ReadOnly]
		public ComponentLookup<UnderConstruction> m_UnderConstructionData;

		[ReadOnly]
		public ComponentLookup<OutsideConnection> m_OutsideConnectionData;

		[ReadOnly]
		public ComponentLookup<Game.Tools.EditorContainer> m_EditorContainerData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectData;

		[ReadOnly]
		public ComponentLookup<GrowthScaleData> m_PrefabGrowthScaleData;

		[ReadOnly]
		public ComponentLookup<QuantityObjectData> m_PrefabQuantityObjectData;

		[ReadOnly]
		public ComponentLookup<StackData> m_PrefabStackData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Overridden> m_OverriddenData;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<CullingInfo> m_CullingInfoData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<Orphan> m_OrphanData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> m_ServiceUpgradeData;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> m_LotAreaData;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> m_SubNets;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public BufferLookup<Skeleton> m_Skeletons;

		[ReadOnly]
		public BufferLookup<Bone> m_Bones;

		[ReadOnly]
		public BufferLookup<MeshGroup> m_MeshGroups;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<MeshData> m_PrefabMeshData;

		[ReadOnly]
		public ComponentLookup<ImpostorData> m_PrefabImpostorData;

		[ReadOnly]
		public ComponentLookup<SharedMeshData> m_PrefabSharedMeshData;

		[ReadOnly]
		public BufferLookup<SubMesh> m_Meshes;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> m_SubMeshGroups;

		[ReadOnly]
		public BufferLookup<LodMesh> m_Lods;

		[ReadOnly]
		public BufferLookup<MeshVertex> m_Vertices;

		[ReadOnly]
		public BufferLookup<MeshIndex> m_Indices;

		[ReadOnly]
		public BufferLookup<MeshNode> m_Nodes;

		[ReadOnly]
		public BufferLookup<ProceduralBone> m_ProceduralBones;

		[NativeDisableContainerSafetyRestriction]
		public ParallelWriter<RaycastResult> m_Results;

		public void Execute(int index)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			RaycastSystem.EntityResult entityResult = m_Objects[index];
			RaycastInput input = m_Input[entityResult.m_RaycastIndex];
			if ((input.m_TypeMask & (TypeMask.StaticObjects | TypeMask.Net)) == 0 || m_OverriddenData.HasComponent(entityResult.m_Entity) || !IsNearCamera(entityResult.m_Entity) || ((input.m_Flags & RaycastFlags.IgnoreSecondary) != 0 && m_SecondaryData.HasComponent(entityResult.m_Entity)))
			{
				return;
			}
			Transform transform = m_TransformData[entityResult.m_Entity];
			PrefabRef prefabRef = m_PrefabRefData[entityResult.m_Entity];
			Segment val = input.m_Line + input.m_Offset;
			bool flag = false;
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			if (m_PrefabObjectData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData))
			{
				if ((objectGeometryData.m_Flags & GeometryFlags.Marker) != GeometryFlags.None)
				{
					if ((input.m_Flags & RaycastFlags.OutsideConnections) != 0 && m_OutsideConnectionData.HasComponent(entityResult.m_Entity))
					{
						if ((input.m_TypeMask & TypeMask.StaticObjects) != TypeMask.None)
						{
							input.m_Flags |= RaycastFlags.SubElements;
						}
					}
					else if ((input.m_Flags & RaycastFlags.Markers) == 0)
					{
						return;
					}
				}
				else
				{
					Elevation elevation = default(Elevation);
					CollisionMask collisionMask = ((!m_ElevationData.TryGetComponent(entityResult.m_Entity, ref elevation)) ? ObjectUtils.GetCollisionMask(objectGeometryData, ignoreMarkers: true) : ObjectUtils.GetCollisionMask(objectGeometryData, elevation, ignoreMarkers: true));
					if ((collisionMask & input.m_CollisionMask) == 0)
					{
						if ((input.m_CollisionMask & CollisionMask.Underground) == 0 || (input.m_Flags & RaycastFlags.PartialSurface) == 0 || !(objectGeometryData.m_Bounds.min.y < 0f))
						{
							return;
						}
						flag = true;
					}
				}
				quaternion val2 = math.inverse(transform.m_Rotation);
				Segment val3 = new Segment
				{
					a = math.mul(val2, val.a - transform.m_Position),
					b = math.mul(val2, val.b - transform.m_Position)
				};
				RaycastResult result = new RaycastResult
				{
					m_Owner = entityResult.m_Entity,
					m_Hit = 
					{
						m_HitEntity = entityResult.m_Entity,
						m_Position = transform.m_Position,
						m_NormalizedDistance = 1f
					}
				};
				Stack stack = default(Stack);
				StackData stackData = default(StackData);
				Bounds3 val4 = ((!m_StackData.TryGetComponent(entityResult.m_Entity, ref stack) || !m_PrefabStackData.TryGetComponent(prefabRef.m_Prefab, ref stackData)) ? ObjectUtils.GetBounds(objectGeometryData) : ObjectUtils.GetBounds(stack, objectGeometryData, stackData));
				float2 val5 = default(float2);
				if (MathUtils.Intersect(val4, val3, ref val5) && val5.x < result.m_Hit.m_NormalizedDistance)
				{
					float3 hitPosition = MathUtils.Position(val, val5.x);
					result.m_Hit.m_HitPosition = hitPosition;
					result.m_Hit.m_NormalizedDistance = val5.x;
					result.m_Hit.m_CellIndex = new int2(-1, -1);
					float num = math.cmax(MathUtils.Size(val4));
					val5 = math.saturate(new float2(val5.x - num, val5.y + num));
					float num2 = default(float);
					if (flag && MathUtils.Intersect(((Segment)(ref val3)).y, 0f, ref num2))
					{
						if (val3.b.y > val3.a.y)
						{
							val5.y = math.min(val5.y, num2);
						}
						else
						{
							val5.x = math.max(val5.x, num2);
						}
					}
					if (val5.y > val5.x)
					{
						Segment localLine = MathUtils.Cut(val3, val5);
						if (!RaycastMeshes(in input, ref result, entityResult.m_Entity, prefabRef, val, localLine, transform.m_Rotation, val5))
						{
							result.m_Hit.m_NormalizedDistance = 1f;
						}
					}
				}
				if ((objectGeometryData.m_Flags & GeometryFlags.HasLot) != GeometryFlags.None && (input.m_Flags & RaycastFlags.BuildingLots) != 0 && !flag)
				{
					RaycastLot(ref result, objectGeometryData, val, entityResult.m_RaycastIndex, val2, transform.m_Position);
				}
				if (result.m_Hit.m_NormalizedDistance < 1f && ValidateResult(in input, ref result))
				{
					m_Results.Accumulate(entityResult.m_RaycastIndex, result);
				}
			}
			else
			{
				if ((input.m_Flags & RaycastFlags.EditorContainers) == 0 && m_EditorContainerData.HasComponent(entityResult.m_Entity))
				{
					return;
				}
				float num4 = default(float);
				float num3 = MathUtils.Distance(val, transform.m_Position, ref num4);
				if (num3 < 1f)
				{
					RaycastResult result2 = new RaycastResult
					{
						m_Owner = entityResult.m_Entity,
						m_Hit = 
						{
							m_HitEntity = entityResult.m_Entity,
							m_Position = transform.m_Position,
							m_HitPosition = MathUtils.Position(val, num4),
							m_NormalizedDistance = num4 - (1f - num3) / math.max(1f, MathUtils.Length(val)),
							m_CellIndex = new int2(0, -1)
						}
					};
					if (ValidateResult(in input, ref result2))
					{
						m_Results.Accumulate(entityResult.m_RaycastIndex, result2);
					}
				}
			}
		}

		private bool IsNearCamera(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			CullingInfo cullingInfo = default(CullingInfo);
			if (m_CullingInfoData.TryGetComponent(entity, ref cullingInfo) && cullingInfo.m_CullingIndex != 0)
			{
				return (m_CullingData[cullingInfo.m_CullingIndex].m_Flags & PreCullingFlags.NearCamera) != 0;
			}
			return false;
		}

		private bool ValidateResult(in RaycastInput input, ref RaycastResult result)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			TypeMask typeMask = TypeMask.StaticObjects;
			float3 position = result.m_Hit.m_Position;
			Entity owner = Entity.Null;
			TypeMask typeMask2 = TypeMask.None;
			DynamicBuffer<InstalledUpgrade> val = default(DynamicBuffer<InstalledUpgrade>);
			Owner owner2 = default(Owner);
			Game.Net.Node node = default(Game.Net.Node);
			float num = default(float);
			while (true)
			{
				if ((input.m_Flags & RaycastFlags.UpgradeIsMain) != 0)
				{
					if (m_ServiceUpgradeData.HasComponent(result.m_Owner))
					{
						break;
					}
					if (m_InstalledUpgrades.TryGetBuffer(result.m_Owner, ref val) && val.Length != 0)
					{
						owner = Entity.Null;
						typeMask2 = TypeMask.None;
						typeMask = TypeMask.StaticObjects;
						result.m_Owner = val[0].m_Upgrade;
						break;
					}
				}
				else if ((input.m_Flags & RaycastFlags.SubBuildings) != 0 && m_ServiceUpgradeData.HasComponent(result.m_Owner) && (typeMask == TypeMask.Net || m_BuildingData.HasComponent(result.m_Owner)))
				{
					break;
				}
				if (!m_OwnerData.TryGetComponent(result.m_Owner, ref owner2))
				{
					break;
				}
				if (typeMask != TypeMask.Net || typeMask2 != TypeMask.Net || (input.m_Flags & RaycastFlags.ElevateOffset) == 0)
				{
					owner = result.m_Owner;
					typeMask2 = typeMask;
				}
				if (m_NodeData.TryGetComponent(owner2.m_Owner, ref node))
				{
					typeMask = TypeMask.Net;
					result.m_Owner = owner2.m_Owner;
					position = node.m_Position;
					if ((input.m_TypeMask & (TypeMask.StaticObjects | TypeMask.Net)) == TypeMask.Net)
					{
						typeMask2 = TypeMask.None;
						result.m_Hit.m_Position = position;
						break;
					}
				}
				else if (m_EdgeData.HasComponent(owner2.m_Owner))
				{
					typeMask = TypeMask.Net;
					result.m_Owner = owner2.m_Owner;
					Curve curve = m_CurveData[owner2.m_Owner];
					MathUtils.Distance(curve.m_Bezier, result.m_Hit.m_Position, ref num);
					position = MathUtils.Position(curve.m_Bezier, num);
					if ((input.m_TypeMask & (TypeMask.StaticObjects | TypeMask.Net)) == TypeMask.Net)
					{
						typeMask2 = TypeMask.None;
						result.m_Hit.m_Position = position;
						break;
					}
				}
				else if (m_LotAreaData.HasComponent(owner2.m_Owner))
				{
					typeMask = TypeMask.Areas;
					result.m_Owner = owner2.m_Owner;
					if ((input.m_TypeMask & TypeMask.Areas) == 0)
					{
						return false;
					}
				}
				else
				{
					typeMask = TypeMask.StaticObjects;
					result.m_Owner = owner2.m_Owner;
				}
			}
			if ((input.m_Flags & RaycastFlags.SubElements) != 0 && (input.m_TypeMask & typeMask2) != TypeMask.None)
			{
				result.m_Owner = owner;
				typeMask = typeMask2;
				if (typeMask2 == TypeMask.Net)
				{
					result.m_Hit.m_Position = position;
				}
			}
			else if ((input.m_Flags & RaycastFlags.NoMainElements) != 0)
			{
				return false;
			}
			if ((input.m_TypeMask & typeMask) == 0)
			{
				if ((input.m_TypeMask & TypeMask.Net) != TypeMask.None)
				{
					return FindClosestNode(in input, ref result);
				}
				return false;
			}
			switch (typeMask)
			{
			case TypeMask.Net:
			{
				PrefabRef prefabRef = m_PrefabRefData[result.m_Owner];
				if ((m_PrefabNetData[prefabRef.m_Prefab].m_ConnectLayers & input.m_NetLayerMask) != Layer.None && (input.m_Flags & RaycastFlags.ElevateOffset) == 0)
				{
					return CheckNetCollisionMask(in input, result.m_Owner);
				}
				return false;
			}
			case TypeMask.StaticObjects:
				return CheckPlaceholder(in input, ref result.m_Owner);
			default:
				return true;
			}
		}

		private bool CheckNetCollisionMask(in RaycastInput input, Entity owner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			Composition composition = default(Composition);
			if (m_CompositionData.TryGetComponent(owner, ref composition))
			{
				return CheckCompositionCollisionMask(in input, composition.m_Edge);
			}
			Orphan orphan = default(Orphan);
			if (m_OrphanData.TryGetComponent(owner, ref orphan))
			{
				return CheckCompositionCollisionMask(in input, orphan.m_Composition);
			}
			DynamicBuffer<ConnectedEdge> val = default(DynamicBuffer<ConnectedEdge>);
			if (m_ConnectedEdges.TryGetBuffer(owner, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					Entity edge = val[i].m_Edge;
					Edge edge2 = m_EdgeData[edge];
					if (edge2.m_Start == owner && m_CompositionData.TryGetComponent(edge, ref composition) && !CheckCompositionCollisionMask(in input, composition.m_StartNode))
					{
						return false;
					}
					if (edge2.m_End == owner && m_CompositionData.TryGetComponent(edge, ref composition) && !CheckCompositionCollisionMask(in input, composition.m_EndNode))
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}

		private bool CheckCompositionCollisionMask(in RaycastInput input, Entity composition)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			NetCompositionData compositionData = default(NetCompositionData);
			if (m_PrefabCompositionData.TryGetComponent(composition, ref compositionData))
			{
				if ((compositionData.m_State & CompositionState.Marker) != 0)
				{
					if ((input.m_Flags & RaycastFlags.Markers) == 0)
					{
						return false;
					}
				}
				else if ((NetUtils.GetCollisionMask(compositionData, ignoreMarkers: true) & input.m_CollisionMask) == 0)
				{
					return false;
				}
			}
			return true;
		}

		private bool CheckPlaceholder(in RaycastInput input, ref Entity entity)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			if ((input.m_Flags & RaycastFlags.Placeholders) != 0)
			{
				return true;
			}
			if (m_PlaceholderData.HasComponent(entity))
			{
				if (m_AttachmentData.HasComponent(entity))
				{
					Attachment attachment = m_AttachmentData[entity];
					if (m_PrefabRefData.HasComponent(attachment.m_Attached))
					{
						entity = attachment.m_Attached;
						return true;
					}
				}
				return false;
			}
			return true;
		}

		private bool FindClosestNode(in RaycastInput input, ref RaycastResult result)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			if (!m_SubNets.HasBuffer(result.m_Owner) || (input.m_Flags & (RaycastFlags.SubElements | RaycastFlags.NoMainElements)) != 0)
			{
				return false;
			}
			float num = float.MaxValue;
			Entity val = Entity.Null;
			float3 position = default(float3);
			DynamicBuffer<Game.Net.SubNet> val2 = m_SubNets[result.m_Owner];
			float num4 = default(float);
			for (int i = 0; i < val2.Length; i++)
			{
				Entity subNet = val2[i].m_SubNet;
				if (m_NodeData.HasComponent(subNet))
				{
					PrefabRef prefabRef = m_PrefabRefData[subNet];
					if ((m_PrefabNetData[prefabRef.m_Prefab].m_ConnectLayers & input.m_NetLayerMask) != Layer.None)
					{
						Game.Net.Node node = m_NodeData[subNet];
						float num2 = math.distance(result.m_Hit.m_HitPosition, node.m_Position);
						if (num2 < num)
						{
							num = num2;
							val = subNet;
							position = node.m_Position;
						}
					}
				}
				else
				{
					if (!m_EdgeData.HasComponent(subNet))
					{
						continue;
					}
					PrefabRef prefabRef2 = m_PrefabRefData[subNet];
					if ((m_PrefabNetData[prefabRef2.m_Prefab].m_ConnectLayers & input.m_NetLayerMask) != Layer.None)
					{
						Curve curve = m_CurveData[subNet];
						float num3 = MathUtils.Distance(curve.m_Bezier, result.m_Hit.m_HitPosition, ref num4);
						if (num3 < num)
						{
							num = num3;
							val = subNet;
							position = MathUtils.Position(curve.m_Bezier, num4);
						}
					}
				}
			}
			if (val == Entity.Null)
			{
				return false;
			}
			result.m_Owner = val;
			result.m_Hit.m_Position = position;
			return true;
		}

		private void RaycastLot(ref RaycastResult result, ObjectGeometryData prefabObjectData, Segment worldLine, int raycastIndex, quaternion inverseRotation, float3 position)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			RaycastResult raycastResult = m_TerrainResults[raycastIndex];
			if (raycastResult.m_Owner == Entity.Null)
			{
				return;
			}
			bool flag;
			float2 val;
			if ((prefabObjectData.m_Flags & GeometryFlags.Standing) != GeometryFlags.None)
			{
				flag = (prefabObjectData.m_Flags & GeometryFlags.CircularLeg) != 0;
				val = ((float3)(ref prefabObjectData.m_LegSize)).xz + prefabObjectData.m_LegOffset * 2f + 0.4f;
			}
			else
			{
				flag = (prefabObjectData.m_Flags & GeometryFlags.Circular) != 0;
				val = ((float3)(ref prefabObjectData.m_Size)).xz + 0.4f;
			}
			float3 val2 = math.mul(inverseRotation, raycastResult.m_Hit.m_HitPosition - position);
			val *= 0.5f;
			if (flag)
			{
				if (math.length(((float3)(ref val2)).xz) > math.csum(val) * 0.5f)
				{
					return;
				}
			}
			else if (!math.all((((float3)(ref val2)).xz >= -val) & (((float3)(ref val2)).xz <= val)))
			{
				return;
			}
			result.m_Hit.m_NormalizedDistance = math.distance(raycastResult.m_Hit.m_HitPosition, worldLine.a) / math.max(1f, MathUtils.Length(worldLine));
			result.m_Hit.m_HitPosition = raycastResult.m_Hit.m_HitPosition;
			result.m_Hit.m_HitDirection = raycastResult.m_Hit.m_HitDirection;
			result.m_Hit.m_CellIndex = int2.op_Implicit(-1);
		}

		private bool HasCachedMesh(Entity mesh, out Entity sharedMesh)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			SharedMeshData sharedMeshData = default(SharedMeshData);
			if (m_PrefabSharedMeshData.TryGetComponent(mesh, ref sharedMeshData))
			{
				sharedMesh = sharedMeshData.m_Mesh;
			}
			else
			{
				sharedMesh = mesh;
			}
			return m_Vertices.HasBuffer(sharedMesh);
		}

		private bool RaycastMeshes(in RaycastInput input, ref RaycastResult result, Entity entity, PrefabRef prefabRef, Segment worldLine, Segment localLine, quaternion localToWorldRotation, float2 cutOffset)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_071f: Unknown result type (might be due to invalid IL or missing references)
			//IL_076d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0774: Unknown result type (might be due to invalid IL or missing references)
			//IL_078d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0795: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_074d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_050c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_0518: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_0699: Unknown result type (might be due to invalid IL or missing references)
			//IL_069b: Unknown result type (might be due to invalid IL or missing references)
			//IL_069d: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_057d: Unknown result type (might be due to invalid IL or missing references)
			//IL_057f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_058b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0665: Unknown result type (might be due to invalid IL or missing references)
			//IL_0667: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_066b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0670: Unknown result type (might be due to invalid IL or missing references)
			//IL_0682: Unknown result type (might be due to invalid IL or missing references)
			//IL_0688: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0692: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0631: Unknown result type (might be due to invalid IL or missing references)
			//IL_0633: Unknown result type (might be due to invalid IL or missing references)
			//IL_0635: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_064e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0654: Unknown result type (might be due to invalid IL or missing references)
			//IL_0659: Unknown result type (might be due to invalid IL or missing references)
			//IL_065e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_061b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0620: Unknown result type (might be due to invalid IL or missing references)
			//IL_0625: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			RaycastHit hit = result.m_Hit;
			hit.m_NormalizedDistance = 2f;
			DynamicBuffer<SubMesh> val = default(DynamicBuffer<SubMesh>);
			if (m_Meshes.TryGetBuffer(prefabRef.m_Prefab, ref val))
			{
				SubMeshFlags subMeshFlags = SubMeshFlags.DefaultMissingMesh | SubMeshFlags.HasTransform | SubMeshFlags.OutlineOnly;
				subMeshFlags = (SubMeshFlags)((uint)subMeshFlags | (uint)(m_LeftHandTraffic ? 65536 : 131072));
				float3 scale = float3.op_Implicit(1f);
				float3 offsets = float3.op_Implicit(1f);
				float3 scale2 = float3.op_Implicit(1f);
				int3 tileCounts = int3.op_Implicit(0);
				Tree tree = default(Tree);
				if (m_TreeData.TryGetComponent(entity, ref tree))
				{
					GrowthScaleData growthScaleData = default(GrowthScaleData);
					subMeshFlags = ((!m_PrefabGrowthScaleData.TryGetComponent(prefabRef.m_Prefab, ref growthScaleData)) ? (subMeshFlags | SubMeshFlags.RequireAdult) : (subMeshFlags | BatchDataHelpers.CalculateTreeSubMeshData(tree, growthScaleData, out scale)));
				}
				Stack stack = default(Stack);
				StackData stackData = default(StackData);
				if (m_StackData.TryGetComponent(entity, ref stack) && m_PrefabStackData.TryGetComponent(prefabRef.m_Prefab, ref stackData))
				{
					subMeshFlags |= BatchDataHelpers.CalculateStackSubMeshData(stack, stackData, out tileCounts, out offsets, out scale2);
				}
				else
				{
					stackData = default(StackData);
				}
				NetObject netObject = default(NetObject);
				if (m_NetObjectData.TryGetComponent(entity, ref netObject))
				{
					subMeshFlags |= BatchDataHelpers.CalculateNetObjectSubMeshData(netObject);
				}
				Quantity quantity = default(Quantity);
				if (m_QuantityData.TryGetComponent(entity, ref quantity))
				{
					QuantityObjectData quantityObjectData = default(QuantityObjectData);
					subMeshFlags = ((!m_PrefabQuantityObjectData.TryGetComponent(prefabRef.m_Prefab, ref quantityObjectData)) ? (subMeshFlags | BatchDataHelpers.CalculateQuantitySubMeshData(quantity, default(QuantityObjectData), m_EditorMode)) : (subMeshFlags | BatchDataHelpers.CalculateQuantitySubMeshData(quantity, quantityObjectData, m_EditorMode)));
				}
				UnderConstruction underConstruction = default(UnderConstruction);
				if (m_UnderConstructionData.TryGetComponent(entity, ref underConstruction) && underConstruction.m_NewPrefab == Entity.Null)
				{
					return false;
				}
				ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
				if (m_DestroyedData.HasComponent(entity) && m_PrefabObjectData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData) && (objectGeometryData.m_Flags & (GeometryFlags.Physical | GeometryFlags.HasLot)) == (GeometryFlags.Physical | GeometryFlags.HasLot))
				{
					return false;
				}
				bool flag2 = false;
				bool flag3 = false;
				DynamicBuffer<MeshGroup> val2 = default(DynamicBuffer<MeshGroup>);
				int num = 1;
				DynamicBuffer<SubMeshGroup> val3 = default(DynamicBuffer<SubMeshGroup>);
				if (m_SubMeshGroups.TryGetBuffer(prefabRef.m_Prefab, ref val3) && m_MeshGroups.TryGetBuffer(entity, ref val2))
				{
					num = val2.Length;
				}
				MeshGroup meshGroup = default(MeshGroup);
				SubMeshGroup subMeshGroup = default(SubMeshGroup);
				DynamicBuffer<LodMesh> val5 = default(DynamicBuffer<LodMesh>);
				ImpostorData impostorData = default(ImpostorData);
				for (int i = 0; i < num; i++)
				{
					if (val3.IsCreated)
					{
						CollectionUtils.TryGet<MeshGroup>(val2, i, ref meshGroup);
						subMeshGroup = val3[(int)meshGroup.m_SubMeshGroup];
					}
					else
					{
						subMeshGroup.m_SubMeshRange = new int2(0, val.Length);
						meshGroup = default(MeshGroup);
					}
					for (int j = subMeshGroup.m_SubMeshRange.x; j < subMeshGroup.m_SubMeshRange.y; j++)
					{
						SubMesh subMesh = val[j];
						if ((subMesh.m_Flags & subMeshFlags) != subMesh.m_Flags)
						{
							continue;
						}
						int num2 = 1;
						num2 = math.select(num2, tileCounts.x, (subMesh.m_Flags & SubMeshFlags.IsStackStart) != 0);
						num2 = math.select(num2, tileCounts.y, (subMesh.m_Flags & SubMeshFlags.IsStackMiddle) != 0);
						num2 = math.select(num2, tileCounts.z, (subMesh.m_Flags & SubMeshFlags.IsStackEnd) != 0);
						if (num2 < 1)
						{
							continue;
						}
						Entity val4 = Entity.Null;
						if (HasCachedMesh(subMesh.m_SubMesh, out var sharedMesh))
						{
							val4 = sharedMesh;
						}
						else if (m_Lods.TryGetBuffer(subMesh.m_SubMesh, ref val5))
						{
							for (int num3 = val5.Length - 1; num3 >= 0; num3--)
							{
								if (HasCachedMesh(val5[num3].m_LodMesh, out sharedMesh))
								{
									val4 = sharedMesh;
									break;
								}
							}
						}
						if ((input.m_Flags & RaycastFlags.Decals) == 0 && (m_PrefabMeshData[(val4 != Entity.Null) ? val4 : subMesh.m_SubMesh].m_State & MeshFlags.Decal) != 0)
						{
							continue;
						}
						if (val4 == Entity.Null)
						{
							flag3 = true;
							continue;
						}
						DynamicBuffer<MeshVertex> vertices = m_Vertices[val4];
						DynamicBuffer<MeshIndex> indices = m_Indices[val4];
						DynamicBuffer<MeshNode> nodes = default(DynamicBuffer<MeshNode>);
						DynamicBuffer<ProceduralBone> prefabBones = default(DynamicBuffer<ProceduralBone>);
						DynamicBuffer<Bone> bones = default(DynamicBuffer<Bone>);
						DynamicBuffer<Skeleton> val6 = default(DynamicBuffer<Skeleton>);
						if (m_Nodes.TryGetBuffer(val4, ref nodes) && m_ProceduralBones.TryGetBuffer(val4, ref prefabBones) && m_Bones.TryGetBuffer(entity, ref bones))
						{
							val6 = m_Skeletons[entity];
							if (val6.Length == 0)
							{
								bones = default(DynamicBuffer<Bone>);
								val6 = default(DynamicBuffer<Skeleton>);
							}
						}
						flag2 |= indices.Length != 0;
						flag3 |= indices.Length == 0;
						int num4 = j - subMeshGroup.m_SubMeshRange.x + meshGroup.m_MeshOffset;
						for (int k = 0; k < num2; k++)
						{
							float3 subMeshPosition = subMesh.m_Position;
							float3 subMeshScale = scale;
							if ((subMesh.m_Flags & (SubMeshFlags.IsStackStart | SubMeshFlags.IsStackMiddle | SubMeshFlags.IsStackEnd)) != 0)
							{
								BatchDataHelpers.CalculateStackSubMeshData(stackData, offsets, scale2, k, subMesh.m_Flags, ref subMeshPosition, ref subMeshScale);
							}
							Segment val7 = localLine;
							if ((subMesh.m_Flags & (SubMeshFlags.IsStackStart | SubMeshFlags.IsStackMiddle | SubMeshFlags.IsStackEnd | SubMeshFlags.HasTransform)) != 0)
							{
								quaternion val8 = math.inverse(subMesh.m_Rotation);
								val7.a = math.mul(val8, localLine.a - subMeshPosition) / subMeshScale;
								val7.b = math.mul(val8, localLine.b - subMeshPosition) / subMeshScale;
							}
							else if (math.any(subMeshScale != 1f))
							{
								val7.a = localLine.a / subMeshScale;
								val7.b = localLine.b / subMeshScale;
							}
							if (m_PrefabImpostorData.TryGetComponent(val4, ref impostorData) && impostorData.m_Size != 0f)
							{
								val7.a = (val7.a - impostorData.m_Offset) / impostorData.m_Size;
								val7.b = (val7.b - impostorData.m_Offset) / impostorData.m_Size;
							}
							if (nodes.IsCreated)
							{
								if (prefabBones.IsCreated)
								{
									if (bones.IsCreated)
									{
										if (CheckMeshIntersect(val7, vertices, indices, nodes, prefabBones, bones, val6[num4], new int2(num4, -1), ref hit))
										{
											hit.m_HitDirection = math.rotate(subMesh.m_Rotation, hit.m_HitDirection);
										}
									}
									else if (CheckMeshIntersect(val7, vertices, indices, nodes, prefabBones, new int2(num4, -1), ref hit))
									{
										hit.m_HitDirection = math.rotate(subMesh.m_Rotation, hit.m_HitDirection);
									}
								}
								else if (CheckMeshIntersect(val7, vertices, indices, nodes, new int2(num4, -1), ref hit))
								{
									hit.m_HitDirection = math.rotate(subMesh.m_Rotation, hit.m_HitDirection);
								}
							}
							else if (CheckMeshIntersect(val7, vertices, indices, new int2(num4, -1), ref hit))
							{
								hit.m_HitDirection = math.rotate(subMesh.m_Rotation, hit.m_HitDirection);
							}
						}
					}
				}
				flag = val.Length != 0 && (flag2 || !flag3);
			}
			if (!flag && (m_PrefabObjectData[prefabRef.m_Prefab].m_Flags & GeometryFlags.HasLot) == 0)
			{
				result.m_Hit.m_NormalizedDistance += 10f / math.max(1f, MathUtils.Length(worldLine));
				return true;
			}
			if (hit.m_NormalizedDistance < 2f)
			{
				hit.m_NormalizedDistance = math.lerp(cutOffset.x, cutOffset.y, hit.m_NormalizedDistance);
				hit.m_HitPosition = MathUtils.Position(worldLine, hit.m_NormalizedDistance);
				hit.m_HitDirection = math.normalizesafe(math.rotate(localToWorldRotation, hit.m_HitDirection), default(float3));
				result.m_Hit = hit;
				return true;
			}
			return false;
		}
	}

	[BurstCompile]
	public struct GetSourceRangesJob : IJob
	{
		[ReadOnly]
		public NativeList<RaycastSystem.EntityResult> m_EdgeList;

		[ReadOnly]
		public NativeList<RaycastSystem.EntityResult> m_StaticObjectList;

		[NativeDisableParallelForRestriction]
		public NativeArray<int4> m_Ranges;

		public void Execute()
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			int4 val = default(int4);
			((int4)(ref val))._002Ector(m_EdgeList.Length + 1, 0, m_StaticObjectList.Length + 1, 0);
			for (int i = 0; i < m_Ranges.Length; i++)
			{
				m_Ranges[i] = val;
			}
			for (int j = 0; j < m_EdgeList.Length; j++)
			{
				RaycastSystem.EntityResult entityResult = m_EdgeList[j];
				ref int4 reference = ref CollectionUtils.ElementAt<int4>(m_Ranges, entityResult.m_RaycastIndex);
				reference.x = math.min(reference.x, j);
				reference.y = j;
			}
			for (int k = 0; k < m_StaticObjectList.Length; k++)
			{
				RaycastSystem.EntityResult entityResult2 = m_StaticObjectList[k];
				ref int4 reference2 = ref CollectionUtils.ElementAt<int4>(m_Ranges, entityResult2.m_RaycastIndex);
				reference2.z = math.min(reference2.z, k);
				reference2.w = k;
			}
		}
	}

	[BurstCompile]
	public struct ExtractLaneObjectsJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<RaycastInput> m_Input;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public NativeList<RaycastSystem.EntityResult> m_EdgeList;

		[ReadOnly]
		public NativeList<RaycastSystem.EntityResult> m_StaticObjectList;

		[ReadOnly]
		public NativeArray<int4> m_Ranges;

		public ParallelWriter<RaycastSystem.EntityResult> m_MovingObjectQueue;

		public void Execute(int index)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			RaycastInput input = m_Input[index];
			if ((input.m_TypeMask & TypeMask.MovingObjects) == 0)
			{
				return;
			}
			int4 val = m_Ranges[index];
			if (val.x > val.y && val.z > val.w)
			{
				return;
			}
			int2 val2 = math.max(int2.op_Implicit(0), ((int4)(ref val)).yw - ((int4)(ref val)).xz + 1);
			NativeParallelHashSet<Entity> checkedEntities = default(NativeParallelHashSet<Entity>);
			checkedEntities._002Ector(val2.x * 8 + val2.y, AllocatorHandle.op_Implicit((Allocator)2));
			Composition composition = default(Composition);
			for (int i = val.x; i <= val.y; i++)
			{
				RaycastSystem.EntityResult entity = m_EdgeList[i];
				if (entity.m_RaycastIndex == index && m_CompositionData.TryGetComponent(entity.m_Entity, ref composition))
				{
					Edge edge = m_EdgeData[entity.m_Entity];
					NetCompositionData compositionData = m_PrefabCompositionData[composition.m_StartNode];
					NetCompositionData compositionData2 = m_PrefabCompositionData[composition.m_Edge];
					NetCompositionData compositionData3 = m_PrefabCompositionData[composition.m_EndNode];
					CollisionMask collisionMask = NetUtils.GetCollisionMask(compositionData, ignoreMarkers: false);
					CollisionMask collisionMask2 = NetUtils.GetCollisionMask(compositionData2, ignoreMarkers: false);
					CollisionMask collisionMask3 = NetUtils.GetCollisionMask(compositionData3, ignoreMarkers: false);
					if ((collisionMask & input.m_CollisionMask) != 0)
					{
						TryCheckNode(input, checkedEntities, new RaycastSystem.EntityResult
						{
							m_Entity = edge.m_Start,
							m_RaycastIndex = entity.m_RaycastIndex
						}, entity.m_Entity);
					}
					if ((collisionMask2 & input.m_CollisionMask) != 0)
					{
						TryCheckLanes(checkedEntities, entity);
					}
					if ((collisionMask3 & input.m_CollisionMask) != 0)
					{
						TryCheckNode(input, checkedEntities, new RaycastSystem.EntityResult
						{
							m_Entity = edge.m_End,
							m_RaycastIndex = entity.m_RaycastIndex
						}, entity.m_Entity);
					}
				}
			}
			for (int j = val.z; j <= val.w; j++)
			{
				RaycastSystem.EntityResult obj = m_StaticObjectList[j];
				if (obj.m_RaycastIndex == index)
				{
					TryCheckObject(checkedEntities, obj);
				}
			}
		}

		private void TryCheckObject(NativeParallelHashSet<Entity> checkedEntities, RaycastSystem.EntityResult obj)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			TryCheckLanes(checkedEntities, obj);
		}

		private void TryCheckNode(RaycastInput input, NativeParallelHashSet<Entity> checkedEntities, RaycastSystem.EntityResult node, Entity ignoreEdge)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[node.m_Entity];
			TryCheckLanes(checkedEntities, node);
			for (int i = 0; i < val.Length; i++)
			{
				RaycastSystem.EntityResult entity = new RaycastSystem.EntityResult
				{
					m_Entity = val[i].m_Edge,
					m_RaycastIndex = node.m_RaycastIndex
				};
				if (!(entity.m_Entity == ignoreEdge))
				{
					Composition composition = m_CompositionData[entity.m_Entity];
					if ((NetUtils.GetCollisionMask(m_PrefabCompositionData[composition.m_Edge], ignoreMarkers: false) & input.m_CollisionMask) != 0)
					{
						TryCheckLanes(checkedEntities, entity);
					}
				}
			}
		}

		private void TryCheckLanes(NativeParallelHashSet<Entity> checkedEntities, RaycastSystem.EntityResult entity)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Net.SubLane> lanes = default(DynamicBuffer<Game.Net.SubLane>);
			if (checkedEntities.Add(entity.m_Entity) && m_SubLanes.TryGetBuffer(entity.m_Entity, ref lanes))
			{
				CheckLanes(entity.m_RaycastIndex, lanes);
			}
		}

		private void CheckLanes(int raycastIndex, DynamicBuffer<Game.Net.SubLane> lanes)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<LaneObject> val = default(DynamicBuffer<LaneObject>);
			for (int i = 0; i < lanes.Length; i++)
			{
				Entity subLane = lanes[i].m_SubLane;
				if (m_LaneObjects.TryGetBuffer(subLane, ref val))
				{
					for (int j = 0; j < val.Length; j++)
					{
						m_MovingObjectQueue.Enqueue(new RaycastSystem.EntityResult
						{
							m_Entity = val[j].m_LaneObject,
							m_RaycastIndex = raycastIndex
						});
					}
				}
			}
		}
	}

	[BurstCompile]
	public struct RaycastMovingObjectsJob : IJobParallelForDefer
	{
		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public bool m_LeftHandTraffic;

		[ReadOnly]
		public NativeArray<RaycastInput> m_Input;

		[ReadOnly]
		public NativeArray<RaycastSystem.EntityResult> m_ObjectList;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Quantity> m_QuantityData;

		[ReadOnly]
		public ComponentLookup<CullingInfo> m_CullingInfoData;

		[ReadOnly]
		public ComponentLookup<InterpolatedTransform> m_InterpolatedTransformData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectData;

		[ReadOnly]
		public ComponentLookup<QuantityObjectData> m_PrefabQuantityObjectData;

		[ReadOnly]
		public ComponentLookup<MeshData> m_PrefabMeshData;

		[ReadOnly]
		public ComponentLookup<SharedMeshData> m_PrefabSharedMeshData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public BufferLookup<SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<Passenger> m_Passengers;

		[ReadOnly]
		public BufferLookup<Skeleton> m_Skeletons;

		[ReadOnly]
		public BufferLookup<Bone> m_Bones;

		[ReadOnly]
		public BufferLookup<MeshGroup> m_MeshGroups;

		[ReadOnly]
		public BufferLookup<SubMesh> m_Meshes;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> m_SubMeshGroups;

		[ReadOnly]
		public BufferLookup<LodMesh> m_Lods;

		[ReadOnly]
		public BufferLookup<MeshVertex> m_Vertices;

		[ReadOnly]
		public BufferLookup<MeshIndex> m_Indices;

		[ReadOnly]
		public BufferLookup<MeshNode> m_Nodes;

		[ReadOnly]
		public BufferLookup<ProceduralBone> m_ProceduralBones;

		[NativeDisableContainerSafetyRestriction]
		public ParallelWriter<RaycastResult> m_Results;

		public void Execute(int index)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			RaycastSystem.EntityResult entityResult = m_ObjectList[index];
			RaycastInput input = m_Input[entityResult.m_RaycastIndex];
			if ((input.m_TypeMask & TypeMask.MovingObjects) != TypeMask.None)
			{
				RaycastObjects(entityResult.m_RaycastIndex, input, entityResult.m_Entity, entityResult.m_Entity);
			}
		}

		private void RaycastObjects(int raycastIndex, RaycastInput input, Entity owner, Entity entity)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			RaycastObject(raycastIndex, input, owner, entity);
			DynamicBuffer<SubObject> val = default(DynamicBuffer<SubObject>);
			if (m_SubObjects.TryGetBuffer(entity, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					Entity subObject = val[i].m_SubObject;
					RaycastObjects(raycastIndex, input, owner, subObject);
				}
			}
			DynamicBuffer<Passenger> val2 = default(DynamicBuffer<Passenger>);
			if (m_Passengers.TryGetBuffer(entity, ref val2))
			{
				for (int j = 0; j < val2.Length; j++)
				{
					Entity passenger = val2[j].m_Passenger;
					RaycastObjects(raycastIndex, input, passenger, passenger);
				}
			}
		}

		private void RaycastObject(int raycastIndex, RaycastInput input, Entity owner, Entity entity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			if (!IsNearCamera(entity))
			{
				return;
			}
			InterpolatedTransform interpolatedTransform = default(InterpolatedTransform);
			Transform transform = ((!m_InterpolatedTransformData.TryGetComponent(entity, ref interpolatedTransform)) ? m_TransformData[entity] : interpolatedTransform.ToTransform());
			PrefabRef prefabRefData = m_PrefabRefData[entity];
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			if (!m_PrefabObjectData.TryGetComponent(prefabRefData.m_Prefab, ref objectGeometryData))
			{
				return;
			}
			float num2 = default(float);
			float num = MathUtils.DistanceSquared(input.m_Line, transform.m_Position, ref num2);
			float3 size = objectGeometryData.m_Size;
			((float3)(ref size)).xz = ((float3)(ref size)).xz * 0.5f;
			if (num > math.lengthsq(size))
			{
				return;
			}
			Bounds3 bounds = objectGeometryData.m_Bounds;
			quaternion val = math.inverse(transform.m_Rotation);
			Segment val2 = new Segment
			{
				a = math.mul(val, input.m_Line.a - transform.m_Position),
				b = math.mul(val, input.m_Line.b - transform.m_Position)
			};
			float2 val3 = default(float2);
			if (MathUtils.Intersect(bounds, val2, ref val3))
			{
				float3 hitPosition = MathUtils.Position(input.m_Line, val3.x);
				RaycastResult result = new RaycastResult
				{
					m_Owner = owner,
					m_Hit = 
					{
						m_HitEntity = entity,
						m_Position = transform.m_Position,
						m_HitPosition = hitPosition,
						m_NormalizedDistance = val3.x,
						m_CellIndex = new int2(-1, -1)
					}
				};
				float num3 = math.cmax(MathUtils.Size(bounds));
				val3 = math.saturate(new float2(val3.x - num3, val3.y + num3));
				val2 = MathUtils.Cut(val2, val3);
				if (RaycastMeshes(input, ref result, entity, prefabRefData, input.m_Line, val2, transform.m_Rotation, val3))
				{
					m_Results.Accumulate(raycastIndex, result);
				}
			}
		}

		private bool IsNearCamera(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			CullingInfo cullingInfo = default(CullingInfo);
			if (m_CullingInfoData.TryGetComponent(entity, ref cullingInfo) && cullingInfo.m_CullingIndex != 0)
			{
				return (m_CullingData[cullingInfo.m_CullingIndex].m_Flags & PreCullingFlags.NearCamera) != 0;
			}
			return false;
		}

		private bool HasCachedMesh(Entity mesh, out Entity sharedMesh)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			SharedMeshData sharedMeshData = default(SharedMeshData);
			if (m_PrefabSharedMeshData.TryGetComponent(mesh, ref sharedMeshData))
			{
				sharedMesh = sharedMeshData.m_Mesh;
			}
			else
			{
				sharedMesh = mesh;
			}
			return m_Vertices.HasBuffer(sharedMesh);
		}

		private bool RaycastMeshes(RaycastInput input, ref RaycastResult result, Entity entity, PrefabRef prefabRefData, Segment worldLine, Segment localLine, quaternion localToWorldRotation, float2 cutOffset)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			RaycastHit hit = result.m_Hit;
			hit.m_NormalizedDistance = 2f;
			DynamicBuffer<SubMesh> val = default(DynamicBuffer<SubMesh>);
			if (m_Meshes.TryGetBuffer(prefabRefData.m_Prefab, ref val))
			{
				SubMeshFlags subMeshFlags = SubMeshFlags.DefaultMissingMesh | SubMeshFlags.HasTransform;
				subMeshFlags = (SubMeshFlags)((uint)subMeshFlags | (uint)(m_LeftHandTraffic ? 65536 : 131072));
				Quantity quantity = default(Quantity);
				if (m_QuantityData.TryGetComponent(entity, ref quantity))
				{
					QuantityObjectData quantityObjectData = default(QuantityObjectData);
					subMeshFlags = ((!m_PrefabQuantityObjectData.TryGetComponent(prefabRefData.m_Prefab, ref quantityObjectData)) ? (subMeshFlags | BatchDataHelpers.CalculateQuantitySubMeshData(quantity, default(QuantityObjectData), m_EditorMode)) : (subMeshFlags | BatchDataHelpers.CalculateQuantitySubMeshData(quantity, quantityObjectData, m_EditorMode)));
				}
				bool flag2 = false;
				bool flag3 = false;
				DynamicBuffer<MeshGroup> val2 = default(DynamicBuffer<MeshGroup>);
				int num = 1;
				DynamicBuffer<SubMeshGroup> val3 = default(DynamicBuffer<SubMeshGroup>);
				if (m_SubMeshGroups.TryGetBuffer(prefabRefData.m_Prefab, ref val3) && m_MeshGroups.TryGetBuffer(entity, ref val2))
				{
					num = val2.Length;
				}
				MeshGroup meshGroup = default(MeshGroup);
				SubMeshGroup subMeshGroup = default(SubMeshGroup);
				DynamicBuffer<LodMesh> val5 = default(DynamicBuffer<LodMesh>);
				DynamicBuffer<MeshNode> nodes = default(DynamicBuffer<MeshNode>);
				DynamicBuffer<ProceduralBone> prefabBones = default(DynamicBuffer<ProceduralBone>);
				DynamicBuffer<Bone> bones = default(DynamicBuffer<Bone>);
				for (int i = 0; i < num; i++)
				{
					if (val3.IsCreated)
					{
						CollectionUtils.TryGet<MeshGroup>(val2, i, ref meshGroup);
						subMeshGroup = val3[(int)meshGroup.m_SubMeshGroup];
					}
					else
					{
						subMeshGroup.m_SubMeshRange = new int2(0, val.Length);
						meshGroup = default(MeshGroup);
					}
					for (int j = subMeshGroup.m_SubMeshRange.x; j < subMeshGroup.m_SubMeshRange.y; j++)
					{
						SubMesh subMesh = val[j];
						if ((subMesh.m_Flags & subMeshFlags) != subMesh.m_Flags)
						{
							continue;
						}
						Entity val4 = Entity.Null;
						if (HasCachedMesh(subMesh.m_SubMesh, out var sharedMesh))
						{
							val4 = sharedMesh;
						}
						else if (m_Lods.TryGetBuffer(subMesh.m_SubMesh, ref val5))
						{
							for (int num2 = val5.Length - 1; num2 >= 0; num2--)
							{
								if (HasCachedMesh(val5[num2].m_LodMesh, out sharedMesh))
								{
									val4 = sharedMesh;
									break;
								}
							}
						}
						if ((input.m_Flags & RaycastFlags.Decals) == 0 && (m_PrefabMeshData[(val4 != Entity.Null) ? val4 : subMesh.m_SubMesh].m_State & MeshFlags.Decal) != 0)
						{
							continue;
						}
						if (val4 == Entity.Null)
						{
							flag3 = true;
							continue;
						}
						Segment localLine2 = localLine;
						if ((subMesh.m_Flags & SubMeshFlags.HasTransform) != 0)
						{
							quaternion val6 = math.inverse(subMesh.m_Rotation);
							localLine2.a = math.mul(val6, localLine.a - subMesh.m_Position);
							localLine2.b = math.mul(val6, localLine.b - subMesh.m_Position);
						}
						DynamicBuffer<MeshVertex> vertices = m_Vertices[val4];
						DynamicBuffer<MeshIndex> indices = m_Indices[val4];
						flag2 |= indices.Length != 0;
						flag3 |= indices.Length == 0;
						int num3 = j - subMeshGroup.m_SubMeshRange.x + meshGroup.m_MeshOffset;
						if (m_Nodes.TryGetBuffer(val4, ref nodes))
						{
							if (m_ProceduralBones.TryGetBuffer(val4, ref prefabBones))
							{
								if (m_Bones.TryGetBuffer(entity, ref bones))
								{
									DynamicBuffer<Skeleton> val7 = m_Skeletons[entity];
									if (CheckMeshIntersect(localLine2, vertices, indices, nodes, prefabBones, bones, val7[num3], new int2(num3, -1), ref hit))
									{
										hit.m_HitDirection = math.rotate(subMesh.m_Rotation, hit.m_HitDirection);
									}
								}
								else if (CheckMeshIntersect(localLine2, vertices, indices, nodes, prefabBones, new int2(num3, -1), ref hit))
								{
									hit.m_HitDirection = math.rotate(subMesh.m_Rotation, hit.m_HitDirection);
								}
							}
							else if (CheckMeshIntersect(localLine2, vertices, indices, nodes, new int2(num3, -1), ref hit))
							{
								hit.m_HitDirection = math.rotate(subMesh.m_Rotation, hit.m_HitDirection);
							}
						}
						else if (CheckMeshIntersect(localLine2, vertices, indices, new int2(num3, -1), ref hit))
						{
							hit.m_HitDirection = math.rotate(subMesh.m_Rotation, hit.m_HitDirection);
						}
					}
				}
				flag = val.Length != 0 && (flag2 || !flag3);
			}
			if (!flag)
			{
				return true;
			}
			if (hit.m_NormalizedDistance < 2f)
			{
				hit.m_NormalizedDistance = math.lerp(cutOffset.x, cutOffset.y, hit.m_NormalizedDistance);
				hit.m_HitPosition = MathUtils.Position(worldLine, hit.m_NormalizedDistance);
				hit.m_HitDirection = math.normalizesafe(math.rotate(localToWorldRotation, hit.m_HitDirection), default(float3));
				result.m_Hit = hit;
				return true;
			}
			return false;
		}
	}

	private static bool CheckMeshIntersect(Segment localLine, DynamicBuffer<MeshVertex> vertices, DynamicBuffer<MeshIndex> indices, int2 elementIndex, ref RaycastHit hit)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		Triangle3 val = default(Triangle3);
		float3 val2 = default(float3);
		for (int i = 0; i < indices.Length; i += 3)
		{
			((Triangle3)(ref val))._002Ector(vertices[indices[i].m_Index].m_Vertex, vertices[indices[i + 1].m_Index].m_Vertex, vertices[indices[i + 2].m_Index].m_Vertex);
			if (MathUtils.Intersect(val, localLine, ref val2) && val2.z < hit.m_NormalizedDistance)
			{
				hit.m_HitDirection = MathUtils.NormalCW(val);
				hit.m_NormalizedDistance = val2.z;
				hit.m_CellIndex = elementIndex;
				result = true;
			}
		}
		return result;
	}

	private unsafe static bool CheckMeshIntersect(Segment localLine, DynamicBuffer<MeshVertex> vertices, DynamicBuffer<MeshIndex> indices, DynamicBuffer<MeshNode> nodes, int2 elementIndex, ref RaycastHit hit)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		int* ptr = stackalloc int[128];
		int num = 0;
		if (nodes.Length != 0)
		{
			ptr[num++] = 0;
		}
		float2 val = default(float2);
		Triangle3 val2 = default(Triangle3);
		float3 val3 = default(float3);
		while (--num >= 0)
		{
			int num2 = ptr[num];
			MeshNode meshNode = nodes[num2];
			if (!MathUtils.Intersect(meshNode.m_Bounds, localLine, ref val))
			{
				continue;
			}
			for (int i = meshNode.m_IndexRange.x; i < meshNode.m_IndexRange.y; i += 3)
			{
				((Triangle3)(ref val2))._002Ector(vertices[indices[i].m_Index].m_Vertex, vertices[indices[i + 1].m_Index].m_Vertex, vertices[indices[i + 2].m_Index].m_Vertex);
				if (MathUtils.Intersect(val2, localLine, ref val3) && val3.z < hit.m_NormalizedDistance)
				{
					hit.m_HitDirection = MathUtils.NormalCW(val2);
					hit.m_NormalizedDistance = val3.z;
					hit.m_CellIndex = elementIndex;
					result = true;
				}
			}
			ptr[num] = meshNode.m_SubNodes1.x;
			num = math.select(num, num + 1, meshNode.m_SubNodes1.x != -1);
			ptr[num] = meshNode.m_SubNodes1.y;
			num = math.select(num, num + 1, meshNode.m_SubNodes1.y != -1);
			ptr[num] = meshNode.m_SubNodes1.z;
			num = math.select(num, num + 1, meshNode.m_SubNodes1.z != -1);
			ptr[num] = meshNode.m_SubNodes1.w;
			num = math.select(num, num + 1, meshNode.m_SubNodes1.w != -1);
			ptr[num] = meshNode.m_SubNodes2.x;
			num = math.select(num, num + 1, meshNode.m_SubNodes2.x != -1);
			ptr[num] = meshNode.m_SubNodes2.y;
			num = math.select(num, num + 1, meshNode.m_SubNodes2.y != -1);
			ptr[num] = meshNode.m_SubNodes2.z;
			num = math.select(num, num + 1, meshNode.m_SubNodes2.z != -1);
			ptr[num] = meshNode.m_SubNodes2.w;
			num = math.select(num, num + 1, meshNode.m_SubNodes2.w != -1);
		}
		return result;
	}

	private unsafe static bool CheckMeshIntersect(Segment localLine, DynamicBuffer<MeshVertex> vertices, DynamicBuffer<MeshIndex> indices, DynamicBuffer<MeshNode> nodes, DynamicBuffer<ProceduralBone> prefabBones, int2 elementIndex, ref RaycastHit hit)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		int* ptr = stackalloc int[128];
		float2 val = default(float2);
		Triangle3 val2 = default(Triangle3);
		float3 val3 = default(float3);
		for (int i = 0; i < prefabBones.Length; i++)
		{
			int num = 0;
			if (math.any(MathUtils.Size(nodes[i].m_Bounds) > 0f))
			{
				ptr[num++] = i;
			}
			while (--num >= 0)
			{
				int num2 = ptr[num];
				MeshNode meshNode = nodes[num2];
				if (!MathUtils.Intersect(meshNode.m_Bounds, localLine, ref val))
				{
					continue;
				}
				for (int j = meshNode.m_IndexRange.x; j < meshNode.m_IndexRange.y; j += 3)
				{
					((Triangle3)(ref val2))._002Ector(vertices[indices[j].m_Index].m_Vertex, vertices[indices[j + 1].m_Index].m_Vertex, vertices[indices[j + 2].m_Index].m_Vertex);
					if (MathUtils.Intersect(val2, localLine, ref val3) && val3.z < hit.m_NormalizedDistance)
					{
						hit.m_HitDirection = MathUtils.NormalCW(val2);
						hit.m_NormalizedDistance = val3.z;
						hit.m_CellIndex = elementIndex;
						result = true;
					}
				}
				ptr[num] = meshNode.m_SubNodes1.x;
				num = math.select(num, num + 1, meshNode.m_SubNodes1.x != -1);
				ptr[num] = meshNode.m_SubNodes1.y;
				num = math.select(num, num + 1, meshNode.m_SubNodes1.y != -1);
				ptr[num] = meshNode.m_SubNodes1.z;
				num = math.select(num, num + 1, meshNode.m_SubNodes1.z != -1);
				ptr[num] = meshNode.m_SubNodes1.w;
				num = math.select(num, num + 1, meshNode.m_SubNodes1.w != -1);
				ptr[num] = meshNode.m_SubNodes2.x;
				num = math.select(num, num + 1, meshNode.m_SubNodes2.x != -1);
				ptr[num] = meshNode.m_SubNodes2.y;
				num = math.select(num, num + 1, meshNode.m_SubNodes2.y != -1);
				ptr[num] = meshNode.m_SubNodes2.z;
				num = math.select(num, num + 1, meshNode.m_SubNodes2.z != -1);
				ptr[num] = meshNode.m_SubNodes2.w;
				num = math.select(num, num + 1, meshNode.m_SubNodes2.w != -1);
			}
		}
		return result;
	}

	private unsafe static bool CheckMeshIntersect(Segment localLine, DynamicBuffer<MeshVertex> vertices, DynamicBuffer<MeshIndex> indices, DynamicBuffer<MeshNode> nodes, DynamicBuffer<ProceduralBone> prefabBones, DynamicBuffer<Bone> bones, Skeleton skeleton, int2 elementIndex, ref RaycastHit hit)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		int* ptr = stackalloc int[128];
		float2 val4 = default(float2);
		Triangle3 val5 = default(Triangle3);
		float3 val6 = default(float3);
		for (int i = 0; i < prefabBones.Length; i++)
		{
			int num = 0;
			Segment val = default(Segment);
			ProceduralBone proceduralBone = prefabBones[i];
			if (math.any(MathUtils.Size(nodes[proceduralBone.m_BindIndex].m_Bounds) > 0f))
			{
				ptr[num++] = proceduralBone.m_BindIndex;
				Bone bone = bones[skeleton.m_BoneOffset + i];
				float4x4 val2 = float4x4.TRS(bone.m_Position, bone.m_Rotation, bone.m_Scale);
				int parentIndex = proceduralBone.m_ParentIndex;
				while (parentIndex >= 0)
				{
					Bone bone2 = bones[skeleton.m_BoneOffset + parentIndex];
					ProceduralBone proceduralBone2 = prefabBones[parentIndex];
					val2 = math.mul(float4x4.TRS(bone2.m_Position, bone2.m_Rotation, bone2.m_Scale), val2);
					parentIndex = proceduralBone2.m_ParentIndex;
				}
				val2 = math.mul(val2, proceduralBone.m_BindPose);
				val2 = math.inverse(val2);
				float4 val3 = math.mul(val2, new float4(localLine.a, 1f));
				val.a = ((float4)(ref val3)).xyz;
				val3 = math.mul(val2, new float4(localLine.b, 1f));
				val.b = ((float4)(ref val3)).xyz;
			}
			while (--num >= 0)
			{
				int num2 = ptr[num];
				MeshNode meshNode = nodes[num2];
				if (!MathUtils.Intersect(meshNode.m_Bounds, val, ref val4))
				{
					continue;
				}
				for (int j = meshNode.m_IndexRange.x; j < meshNode.m_IndexRange.y; j += 3)
				{
					((Triangle3)(ref val5))._002Ector(vertices[indices[j].m_Index].m_Vertex, vertices[indices[j + 1].m_Index].m_Vertex, vertices[indices[j + 2].m_Index].m_Vertex);
					if (MathUtils.Intersect(val5, val, ref val6) && val6.z < hit.m_NormalizedDistance)
					{
						hit.m_HitDirection = MathUtils.NormalCW(val5);
						hit.m_NormalizedDistance = val6.z;
						hit.m_CellIndex = elementIndex;
						result = true;
					}
				}
				ptr[num] = meshNode.m_SubNodes1.x;
				num = math.select(num, num + 1, meshNode.m_SubNodes1.x != -1);
				ptr[num] = meshNode.m_SubNodes1.y;
				num = math.select(num, num + 1, meshNode.m_SubNodes1.y != -1);
				ptr[num] = meshNode.m_SubNodes1.z;
				num = math.select(num, num + 1, meshNode.m_SubNodes1.z != -1);
				ptr[num] = meshNode.m_SubNodes1.w;
				num = math.select(num, num + 1, meshNode.m_SubNodes1.w != -1);
				ptr[num] = meshNode.m_SubNodes2.x;
				num = math.select(num, num + 1, meshNode.m_SubNodes2.x != -1);
				ptr[num] = meshNode.m_SubNodes2.y;
				num = math.select(num, num + 1, meshNode.m_SubNodes2.y != -1);
				ptr[num] = meshNode.m_SubNodes2.z;
				num = math.select(num, num + 1, meshNode.m_SubNodes2.z != -1);
				ptr[num] = meshNode.m_SubNodes2.w;
				num = math.select(num, num + 1, meshNode.m_SubNodes2.w != -1);
			}
		}
		return result;
	}
}
