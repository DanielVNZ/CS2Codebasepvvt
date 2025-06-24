using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Areas;

public static class RaycastJobs
{
	[BurstCompile]
	public struct FindAreaJob : IJobParallelFor
	{
		private struct ValidationData
		{
			public bool m_EditorMode;

			public RaycastInput m_Input;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Placeholder> m_PlaceholderData;

			public ComponentLookup<Attachment> m_AttachmentData;

			public ComponentLookup<Building> m_BuildingData;

			public ComponentLookup<Game.Buildings.ServiceUpgrade> m_ServiceUpgradeData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<AreaGeometryData> m_PrefabAreaData;

			public BufferLookup<Node> m_Nodes;

			public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

			public bool ValidateResult(ref RaycastResult result)
			{
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_011a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0124: Unknown result type (might be due to invalid IL or missing references)
				//IL_0129: Unknown result type (might be due to invalid IL or missing references)
				//IL_0135: Unknown result type (might be due to invalid IL or missing references)
				//IL_010a: Unknown result type (might be due to invalid IL or missing references)
				//IL_010f: Unknown result type (might be due to invalid IL or missing references)
				//IL_016b: Unknown result type (might be due to invalid IL or missing references)
				//IL_016c: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
				if (!m_EditorMode)
				{
					PrefabRef prefabRef = m_PrefabRefData[result.m_Owner];
					if ((m_PrefabAreaData[prefabRef.m_Prefab].m_Flags & GeometryFlags.HiddenIngame) != 0)
					{
						return false;
					}
				}
				TypeMask typeMask = TypeMask.Areas;
				Entity owner = Entity.Null;
				TypeMask typeMask2 = TypeMask.None;
				DynamicBuffer<InstalledUpgrade> val = default(DynamicBuffer<InstalledUpgrade>);
				while (true)
				{
					if ((m_Input.m_Flags & RaycastFlags.UpgradeIsMain) != 0)
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
					else if ((m_Input.m_Flags & RaycastFlags.SubBuildings) != 0 && m_BuildingData.HasComponent(result.m_Owner) && m_ServiceUpgradeData.HasComponent(result.m_Owner))
					{
						break;
					}
					if (!m_OwnerData.HasComponent(result.m_Owner))
					{
						break;
					}
					if ((m_Input.m_TypeMask & typeMask) != TypeMask.None)
					{
						owner = result.m_Owner;
						typeMask2 = typeMask;
					}
					result.m_Owner = m_OwnerData[result.m_Owner].m_Owner;
					if (!m_Nodes.HasBuffer(result.m_Owner))
					{
						typeMask = TypeMask.StaticObjects;
					}
				}
				if ((m_Input.m_Flags & RaycastFlags.SubElements) != 0 && (m_Input.m_TypeMask & typeMask2) != TypeMask.None)
				{
					result.m_Owner = owner;
					typeMask = typeMask2;
				}
				else if ((m_Input.m_Flags & RaycastFlags.NoMainElements) != 0)
				{
					return false;
				}
				if ((m_Input.m_TypeMask & typeMask) == 0)
				{
					return false;
				}
				switch (typeMask)
				{
				case TypeMask.Areas:
				{
					PrefabRef prefabRef2 = m_PrefabRefData[result.m_Owner];
					return (AreaUtils.GetTypeMask(m_PrefabAreaData[prefabRef2.m_Prefab].m_Type) & m_Input.m_AreaTypeMask) != 0;
				}
				case TypeMask.StaticObjects:
					return CheckPlaceholder(ref result.m_Owner);
				default:
					return true;
				}
			}

			private bool CheckPlaceholder(ref Entity entity)
			{
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				if ((m_Input.m_Flags & RaycastFlags.Placeholders) != 0)
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
		}

		private struct GroundIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public int m_Index;

			public RaycastResult m_Result;

			public ComponentLookup<Space> m_SpaceData;

			public BufferLookup<Node> m_Nodes;

			public BufferLookup<Triangle> m_Triangles;

			public ValidationData m_ValidationData;

			public ParallelWriter<RaycastResult> m_Results;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((float3)(ref m_Result.m_Hit.m_HitPosition)).xz);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem areaItem)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((float3)(ref m_Result.m_Hit.m_HitPosition)).xz) || m_SpaceData.HasComponent(areaItem.m_Area))
				{
					return;
				}
				Triangle3 triangle = AreaUtils.GetTriangle3(m_Nodes[areaItem.m_Area], m_Triangles[areaItem.m_Area][areaItem.m_Triangle]);
				float2 val = default(float2);
				if (MathUtils.Intersect(((Triangle3)(ref triangle)).xz, ((float3)(ref m_Result.m_Hit.m_HitPosition)).xz, ref val))
				{
					RaycastResult result = m_Result;
					result.m_Owner = areaItem.m_Area;
					if (m_ValidationData.ValidateResult(ref result))
					{
						m_Results.Accumulate(m_Index, result);
					}
				}
			}
		}

		private struct OvergroundIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public int m_Index;

			public Segment m_Line;

			public ComponentLookup<Space> m_SpaceData;

			public BufferLookup<Node> m_Nodes;

			public BufferLookup<Triangle> m_Triangles;

			public ValidationData m_ValidationData;

			public ParallelWriter<RaycastResult> m_Results;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				float2 val = default(float2);
				return MathUtils.Intersect(bounds.m_Bounds, m_Line, ref val);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem areaItem)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_0096: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_0119: Unknown result type (might be due to invalid IL or missing references)
				//IL_011e: Unknown result type (might be due to invalid IL or missing references)
				float2 val = default(float2);
				if (!MathUtils.Intersect(bounds.m_Bounds, m_Line, ref val) || !m_SpaceData.HasComponent(areaItem.m_Area))
				{
					return;
				}
				Triangle3 triangle = AreaUtils.GetTriangle3(m_Nodes[areaItem.m_Area], m_Triangles[areaItem.m_Area][areaItem.m_Triangle]);
				float3 val2 = default(float3);
				if (MathUtils.Intersect(triangle, m_Line, ref val2))
				{
					RaycastResult result = default(RaycastResult);
					result.m_Owner = areaItem.m_Area;
					result.m_Hit.m_HitEntity = result.m_Owner;
					result.m_Hit.m_Position = MathUtils.Position(m_Line, val2.z);
					result.m_Hit.m_HitPosition = result.m_Hit.m_Position;
					result.m_Hit.m_HitDirection = MathUtils.NormalCW(triangle);
					result.m_Hit.m_NormalizedDistance = val2.z - 1f / math.max(1f, MathUtils.Length(m_Line));
					result.m_Hit.m_CellIndex = new int2(-1, -1);
					if (m_ValidationData.ValidateResult(ref result))
					{
						m_Results.Accumulate(m_Index, result);
					}
				}
			}
		}

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public NativeArray<RaycastInput> m_Input;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Space> m_SpaceData;

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
		public ComponentLookup<AreaGeometryData> m_PrefabAreaData;

		[ReadOnly]
		public BufferLookup<Node> m_Nodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_Triangles;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_SearchTree;

		[ReadOnly]
		public NativeArray<RaycastResult> m_TerrainResults;

		[NativeDisableContainerSafetyRestriction]
		public ParallelWriter<RaycastResult> m_Results;

		public void Execute(int index)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			int num = index % m_Input.Length;
			RaycastInput input = m_Input[num];
			if ((input.m_TypeMask & TypeMask.Areas) == 0)
			{
				return;
			}
			ValidationData validationData = new ValidationData
			{
				m_EditorMode = m_EditorMode,
				m_Input = input,
				m_OwnerData = m_OwnerData,
				m_PlaceholderData = m_PlaceholderData,
				m_AttachmentData = m_AttachmentData,
				m_BuildingData = m_BuildingData,
				m_ServiceUpgradeData = m_ServiceUpgradeData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabAreaData = m_PrefabAreaData,
				m_Nodes = m_Nodes,
				m_InstalledUpgrades = m_InstalledUpgrades
			};
			if (index < m_TerrainResults.Length)
			{
				RaycastResult result = m_TerrainResults[index];
				if (!(result.m_Owner == Entity.Null))
				{
					result.m_Owner = Entity.Null;
					result.m_Hit.m_CellIndex = new int2(-1, -1);
					result.m_Hit.m_NormalizedDistance -= 0.25f / math.max(1f, MathUtils.Length(input.m_Line));
					GroundIterator groundIterator = new GroundIterator
					{
						m_Index = num,
						m_Result = result,
						m_SpaceData = m_SpaceData,
						m_Nodes = m_Nodes,
						m_Triangles = m_Triangles,
						m_ValidationData = validationData,
						m_Results = m_Results
					};
					m_SearchTree.Iterate<GroundIterator>(ref groundIterator, 0);
				}
			}
			else
			{
				OvergroundIterator overgroundIterator = new OvergroundIterator
				{
					m_Index = num,
					m_Line = input.m_Line,
					m_SpaceData = m_SpaceData,
					m_Nodes = m_Nodes,
					m_Triangles = m_Triangles,
					m_ValidationData = validationData,
					m_Results = m_Results
				};
				m_SearchTree.Iterate<OvergroundIterator>(ref overgroundIterator, 0);
			}
		}
	}

	[BurstCompile]
	public struct RaycastLabelsJob : IJobChunk
	{
		[ReadOnly]
		public NativeArray<RaycastInput> m_Input;

		[ReadOnly]
		public float3 m_CameraRight;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Geometry> m_GeometryType;

		[ReadOnly]
		public BufferTypeHandle<LabelExtents> m_LabelExtentsType;

		[NativeDisableContainerSafetyRestriction]
		public ParallelWriter<RaycastResult> m_Results;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Geometry> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Geometry>(ref m_GeometryType);
			BufferAccessor<LabelExtents> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LabelExtents>(ref m_LabelExtentsType);
			quaternion labelRotation = AreaUtils.CalculateLabelRotation(m_CameraRight);
			Quad3 val3 = default(Quad3);
			float num = default(float);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Geometry geometry = nativeArray2[i];
				DynamicBuffer<LabelExtents> val = bufferAccessor[i];
				float3 labelPosition = AreaUtils.CalculateLabelPosition(geometry);
				for (int j = 0; j < m_Input.Length; j++)
				{
					RaycastInput raycastInput = m_Input[j];
					if ((raycastInput.m_TypeMask & TypeMask.Labels) == 0)
					{
						continue;
					}
					float4x4 val2 = AreaUtils.CalculateLabelMatrix(raycastInput.m_Line.a, labelPosition, labelRotation);
					for (int k = 0; k < val.Length; k++)
					{
						Bounds2 bounds = val[k].m_Bounds;
						val3.a = math.transform(val2, new float3(((float2)(ref bounds.min)).xy, 0f));
						val3.b = math.transform(val2, new float3(bounds.min.x, bounds.max.y, 0f));
						val3.c = math.transform(val2, new float3(((float2)(ref bounds.max)).xy, 0f));
						val3.d = math.transform(val2, new float3(bounds.max.x, bounds.min.y, 0f));
						if (MathUtils.Intersect(val3, raycastInput.m_Line, ref num))
						{
							float num2 = MathUtils.Size(((Bounds2)(ref bounds)).y) * AreaUtils.CalculateLabelScale(raycastInput.m_Line.a, labelPosition);
							RaycastResult raycastResult = default(RaycastResult);
							raycastResult.m_Owner = nativeArray[i];
							raycastResult.m_Hit.m_HitEntity = raycastResult.m_Owner;
							raycastResult.m_Hit.m_Position = geometry.m_CenterPosition;
							raycastResult.m_Hit.m_HitPosition = MathUtils.Position(raycastInput.m_Line, num);
							raycastResult.m_Hit.m_NormalizedDistance = num - num2 / math.max(1f, MathUtils.Length(raycastInput.m_Line));
							m_Results.Accumulate(j, raycastResult);
						}
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}
}
