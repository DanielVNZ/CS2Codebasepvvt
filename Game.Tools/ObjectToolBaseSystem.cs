using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public abstract class ObjectToolBaseSystem : ToolBaseSystem
{
	public struct AttachmentData
	{
		public Entity m_Entity;

		public float3 m_Offset;
	}

	[BurstCompile]
	private struct CreateDefinitionsJob : IJob
	{
		private struct VariationData
		{
			public Entity m_Prefab;

			public int m_Probability;
		}

		private struct BrushIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public NativeParallelHashSet<Entity> m_RequirePrefab;

			public Bounds2 m_Bounds;

			public RandomSeed m_RandomSeed;

			public Segment m_BrushLine;

			public float4 m_BrushCellSizeFactor;

			public float4 m_BrushTextureSizeAdd;

			public float2 m_BrushDirX;

			public float2 m_BrushDirZ;

			public float2 m_BrushCellSize;

			public int2 m_BrushResolution;

			public float m_TileSize;

			public float m_BrushStrength;

			public float m_StrengthFactor;

			public int m_BrushCount;

			public DynamicBuffer<BrushCell> m_BrushCells;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<Game.Objects.Elevation> m_ElevationData;

			public ComponentLookup<EditorContainer> m_EditorContainerData;

			public ComponentLookup<LocalTransformCache> m_LocalTransformCacheData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

			public EntityCommandBuffer m_CommandBuffer;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity item)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0101: Unknown result type (might be due to invalid IL or missing references)
				//IL_0106: Unknown result type (might be due to invalid IL or missing references)
				//IL_010d: Unknown result type (might be due to invalid IL or missing references)
				//IL_010f: Unknown result type (might be due to invalid IL or missing references)
				//IL_011a: Unknown result type (might be due to invalid IL or missing references)
				//IL_011f: Unknown result type (might be due to invalid IL or missing references)
				//IL_012d: Unknown result type (might be due to invalid IL or missing references)
				//IL_013d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0142: Unknown result type (might be due to invalid IL or missing references)
				//IL_0144: Unknown result type (might be due to invalid IL or missing references)
				//IL_0146: Unknown result type (might be due to invalid IL or missing references)
				//IL_0150: Unknown result type (might be due to invalid IL or missing references)
				//IL_0157: Unknown result type (might be due to invalid IL or missing references)
				//IL_015c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0161: Unknown result type (might be due to invalid IL or missing references)
				//IL_016e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0175: Unknown result type (might be due to invalid IL or missing references)
				//IL_017a: Unknown result type (might be due to invalid IL or missing references)
				//IL_017f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0186: Unknown result type (might be due to invalid IL or missing references)
				//IL_0188: Unknown result type (might be due to invalid IL or missing references)
				//IL_018d: Unknown result type (might be due to invalid IL or missing references)
				//IL_018f: Unknown result type (might be due to invalid IL or missing references)
				//IL_019d: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01af: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0204: Unknown result type (might be due to invalid IL or missing references)
				//IL_0210: Unknown result type (might be due to invalid IL or missing references)
				//IL_0216: Unknown result type (might be due to invalid IL or missing references)
				//IL_0222: Unknown result type (might be due to invalid IL or missing references)
				//IL_0228: Unknown result type (might be due to invalid IL or missing references)
				//IL_0237: Unknown result type (might be due to invalid IL or missing references)
				//IL_023e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0245: Unknown result type (might be due to invalid IL or missing references)
				//IL_024c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0253: Unknown result type (might be due to invalid IL or missing references)
				//IL_0259: Unknown result type (might be due to invalid IL or missing references)
				//IL_025e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0264: Unknown result type (might be due to invalid IL or missing references)
				//IL_0269: Unknown result type (might be due to invalid IL or missing references)
				//IL_026e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0273: Unknown result type (might be due to invalid IL or missing references)
				//IL_0278: Unknown result type (might be due to invalid IL or missing references)
				//IL_027a: Unknown result type (might be due to invalid IL or missing references)
				//IL_027d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0288: Unknown result type (might be due to invalid IL or missing references)
				//IL_028e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0293: Unknown result type (might be due to invalid IL or missing references)
				//IL_0298: Unknown result type (might be due to invalid IL or missing references)
				//IL_029a: Unknown result type (might be due to invalid IL or missing references)
				//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_043f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0444: Unknown result type (might be due to invalid IL or missing references)
				//IL_0450: Unknown result type (might be due to invalid IL or missing references)
				//IL_0451: Unknown result type (might be due to invalid IL or missing references)
				//IL_0468: Unknown result type (might be due to invalid IL or missing references)
				//IL_0484: Unknown result type (might be due to invalid IL or missing references)
				//IL_0489: Unknown result type (might be due to invalid IL or missing references)
				//IL_0491: Unknown result type (might be due to invalid IL or missing references)
				//IL_0496: Unknown result type (might be due to invalid IL or missing references)
				//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0508: Unknown result type (might be due to invalid IL or missing references)
				//IL_033c: Unknown result type (might be due to invalid IL or missing references)
				//IL_035c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0361: Unknown result type (might be due to invalid IL or missing references)
				//IL_0364: Unknown result type (might be due to invalid IL or missing references)
				//IL_0386: Unknown result type (might be due to invalid IL or missing references)
				//IL_038b: Unknown result type (might be due to invalid IL or missing references)
				//IL_038f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0391: Unknown result type (might be due to invalid IL or missing references)
				//IL_0393: Unknown result type (might be due to invalid IL or missing references)
				//IL_0398: Unknown result type (might be due to invalid IL or missing references)
				//IL_039a: Unknown result type (might be due to invalid IL or missing references)
				//IL_039c: Unknown result type (might be due to invalid IL or missing references)
				//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_0577: Unknown result type (might be due to invalid IL or missing references)
				//IL_057c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0584: Unknown result type (might be due to invalid IL or missing references)
				//IL_0589: Unknown result type (might be due to invalid IL or missing references)
				//IL_0516: Unknown result type (might be due to invalid IL or missing references)
				//IL_0522: Unknown result type (might be due to invalid IL or missing references)
				//IL_0527: Unknown result type (might be due to invalid IL or missing references)
				//IL_0530: Unknown result type (might be due to invalid IL or missing references)
				//IL_0535: Unknown result type (might be due to invalid IL or missing references)
				//IL_0594: Unknown result type (might be due to invalid IL or missing references)
				//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds) || m_OwnerData.HasComponent(item))
				{
					return;
				}
				PrefabRef prefabRef = m_PrefabRefData[item];
				ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
				if (m_RequirePrefab.IsCreated)
				{
					if (!m_RequirePrefab.Contains(prefabRef.m_Prefab))
					{
						return;
					}
				}
				else if (!m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData) || (objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Brushable) == 0)
				{
					return;
				}
				Transform transform = m_TransformData[item];
				int2 val = (int2)math.floor(((float3)(ref transform.m_Position)).xz / m_TileSize);
				int index = ((val.y & 0xFFFF) << 16) | (val.x & 0xFFFF);
				Random random = m_RandomSeed.GetRandom(index);
				if (((Random)(ref random)).NextFloat(1f) >= m_BrushStrength)
				{
					return;
				}
				float num = 0f;
				Bounds2 val2 = default(Bounds2);
				val2.min = float2.op_Implicit(val) * m_TileSize;
				val2.max = val2.min + m_TileSize;
				float4 val5 = default(float4);
				float4 val6 = default(float4);
				float4 val7 = default(float4);
				Quad2 val13 = default(Quad2);
				float num3 = default(float);
				for (int i = 1; i <= m_BrushCount; i++)
				{
					float3 val3 = MathUtils.Position(m_BrushLine, (float)i / (float)m_BrushCount);
					Bounds2 val4 = val2;
					ref float2 min = ref val4.min;
					min -= ((float3)(ref val3)).xz;
					ref float2 max = ref val4.max;
					max -= ((float3)(ref val3)).xz;
					((float4)(ref val5))._002Ector(val4.min, val4.max);
					((float4)(ref val6))._002Ector(math.dot(((float4)(ref val5)).xy, m_BrushDirX), math.dot(((float4)(ref val5)).xw, m_BrushDirX), math.dot(((float4)(ref val5)).zy, m_BrushDirX), math.dot(((float4)(ref val5)).zw, m_BrushDirX));
					((float4)(ref val7))._002Ector(math.dot(((float4)(ref val5)).xy, m_BrushDirZ), math.dot(((float4)(ref val5)).xw, m_BrushDirZ), math.dot(((float4)(ref val5)).zy, m_BrushDirZ), math.dot(((float4)(ref val5)).zw, m_BrushDirZ));
					int4 val8 = (int4)math.floor(new float4(math.cmin(val6), math.cmin(val7), math.cmax(val6), math.cmax(val7)) * m_BrushCellSizeFactor + m_BrushTextureSizeAdd);
					val8 = math.clamp(val8, int4.op_Implicit(0), ((int2)(ref m_BrushResolution)).xyxy - 1);
					for (int j = val8.y; j <= val8.w; j++)
					{
						float2 val9 = m_BrushDirZ * (((float)j - m_BrushTextureSizeAdd.y) * m_BrushCellSize.y);
						float2 val10 = m_BrushDirZ * (((float)(j + 1) - m_BrushTextureSizeAdd.y) * m_BrushCellSize.y);
						for (int k = val8.x; k <= val8.z; k++)
						{
							int num2 = k + m_BrushResolution.x * j;
							BrushCell brushCell = m_BrushCells[num2];
							if (brushCell.m_Opacity >= 0.0001f)
							{
								float2 val11 = m_BrushDirX * (((float)k - m_BrushTextureSizeAdd.x) * m_BrushCellSize.x);
								float2 val12 = m_BrushDirX * (((float)(k + 1) - m_BrushTextureSizeAdd.x) * m_BrushCellSize.x);
								((Quad2)(ref val13))._002Ector(val9 + val11, val9 + val12, val10 + val12, val10 + val11);
								if (MathUtils.Intersect(val4, val13, ref num3))
								{
									num += brushCell.m_Opacity * num3;
								}
							}
						}
					}
				}
				num *= m_StrengthFactor;
				if (!(math.abs(num) >= 0.0001f) || !(((Random)(ref random)).NextFloat() < num))
				{
					return;
				}
				Entity val14 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
				CreationDefinition creationDefinition = new CreationDefinition
				{
					m_Original = item
				};
				creationDefinition.m_Flags |= CreationFlags.Delete;
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val14, default(Updated));
				ObjectDefinition objectDefinition = new ObjectDefinition
				{
					m_Position = transform.m_Position,
					m_Rotation = transform.m_Rotation
				};
				Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
				if (m_ElevationData.TryGetComponent(item, ref elevation))
				{
					objectDefinition.m_Elevation = elevation.m_Elevation;
					objectDefinition.m_ParentMesh = ObjectUtils.GetSubParentMesh(elevation.m_Flags);
					if ((elevation.m_Flags & ElevationFlags.Lowered) != 0)
					{
						creationDefinition.m_Flags |= CreationFlags.Lowered;
					}
				}
				else
				{
					objectDefinition.m_ParentMesh = -1;
				}
				objectDefinition.m_Probability = 100;
				objectDefinition.m_PrefabSubIndex = -1;
				if (m_LocalTransformCacheData.HasComponent(item))
				{
					LocalTransformCache localTransformCache = m_LocalTransformCacheData[item];
					objectDefinition.m_LocalPosition = localTransformCache.m_Position;
					objectDefinition.m_LocalRotation = localTransformCache.m_Rotation;
					objectDefinition.m_ParentMesh = localTransformCache.m_ParentMesh;
					objectDefinition.m_GroupIndex = localTransformCache.m_GroupIndex;
					objectDefinition.m_Probability = localTransformCache.m_Probability;
					objectDefinition.m_PrefabSubIndex = localTransformCache.m_PrefabSubIndex;
				}
				else
				{
					objectDefinition.m_LocalPosition = transform.m_Position;
					objectDefinition.m_LocalRotation = transform.m_Rotation;
				}
				if (m_EditorContainerData.HasComponent(item))
				{
					EditorContainer editorContainer = m_EditorContainerData[item];
					creationDefinition.m_SubPrefab = editorContainer.m_Prefab;
					objectDefinition.m_Scale = editorContainer.m_Scale;
					objectDefinition.m_Intensity = editorContainer.m_Intensity;
					objectDefinition.m_GroupIndex = editorContainer.m_GroupIndex;
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<ObjectDefinition>(val14, objectDefinition);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val14, creationDefinition);
			}
		}

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public bool m_LefthandTraffic;

		[ReadOnly]
		public bool m_Removing;

		[ReadOnly]
		public bool m_Stamping;

		[ReadOnly]
		public float m_BrushSize;

		[ReadOnly]
		public float m_BrushAngle;

		[ReadOnly]
		public float m_BrushStrength;

		[ReadOnly]
		public float m_Distance;

		[ReadOnly]
		public float m_DeltaTime;

		[ReadOnly]
		public Entity m_ObjectPrefab;

		[ReadOnly]
		public Entity m_TransformPrefab;

		[ReadOnly]
		public Entity m_BrushPrefab;

		[ReadOnly]
		public Entity m_Owner;

		[ReadOnly]
		public Entity m_Original;

		[ReadOnly]
		public Entity m_LaneEditor;

		[ReadOnly]
		public Entity m_Theme;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public Snap m_Snap;

		[ReadOnly]
		public AgeMask m_AgeMask;

		[ReadOnly]
		public NativeList<ControlPoint> m_ControlPoints;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeReference<AttachmentData> m_AttachmentPrefab;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Attached> m_AttachedData;

		[ReadOnly]
		public ComponentLookup<Attachment> m_AttachmentData;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> m_LocalTransformCacheData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Lot> m_LotData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> m_NetElevationData;

		[ReadOnly]
		public ComponentLookup<Orphan> m_OrphanData;

		[ReadOnly]
		public ComponentLookup<Upgraded> m_UpgradedData;

		[ReadOnly]
		public ComponentLookup<Clear> m_AreaClearData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<Space> m_AreaSpaceData;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> m_AreaLotData;

		[ReadOnly]
		public ComponentLookup<EditorContainer> m_EditorContainerData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetObjectData> m_PrefabNetObjectData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<AssetStampData> m_PrefabAssetStampData;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> m_PrefabBuildingExtensionData;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> m_PrefabSpawnableObjectData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> m_PrefabPlaceableObjectData;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> m_PrefabAreaGeometryData;

		[ReadOnly]
		public ComponentLookup<PlaceholderBuildingData> m_PlaceholderBuildingData;

		[ReadOnly]
		public ComponentLookup<BrushData> m_PrefabBrushData;

		[ReadOnly]
		public ComponentLookup<BuildingTerraformData> m_PrefabBuildingTerraformData;

		[ReadOnly]
		public ComponentLookup<CreatureSpawnData> m_PrefabCreatureSpawnData;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public BufferLookup<LocalNodeCache> m_CachedNodes;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> m_SubNets;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_AreaTriangles;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubObject> m_PrefabSubObjects;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubNet> m_PrefabSubNets;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubLane> m_PrefabSubLanes;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubArea> m_PrefabSubAreas;

		[ReadOnly]
		public BufferLookup<SubAreaNode> m_PrefabSubAreaNodes;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> m_PrefabPlaceholderElements;

		[ReadOnly]
		public BufferLookup<ObjectRequirementElement> m_PrefabRequirementElements;

		[ReadOnly]
		public BufferLookup<ServiceUpgradeBuilding> m_PrefabServiceUpgradeBuilding;

		[ReadOnly]
		public BufferLookup<BrushCell> m_PrefabBrushCells;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_ObjectSearchTree;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0647: Unknown result type (might be due to invalid IL or missing references)
			//IL_0648: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0667: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_066f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_067b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0681: Unknown result type (might be due to invalid IL or missing references)
			//IL_0687: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0693: Unknown result type (might be due to invalid IL or missing references)
			//IL_069f: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_0796: Unknown result type (might be due to invalid IL or missing references)
			//IL_0798: Unknown result type (might be due to invalid IL or missing references)
			//IL_0799: Unknown result type (might be due to invalid IL or missing references)
			//IL_077b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0780: Unknown result type (might be due to invalid IL or missing references)
			//IL_0721: Unknown result type (might be due to invalid IL or missing references)
			//IL_0726: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0854: Unknown result type (might be due to invalid IL or missing references)
			//IL_0857: Unknown result type (might be due to invalid IL or missing references)
			//IL_085c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0861: Unknown result type (might be due to invalid IL or missing references)
			//IL_0863: Unknown result type (might be due to invalid IL or missing references)
			//IL_0868: Unknown result type (might be due to invalid IL or missing references)
			//IL_0869: Unknown result type (might be due to invalid IL or missing references)
			//IL_086c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0872: Unknown result type (might be due to invalid IL or missing references)
			//IL_0886: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_0565: Unknown result type (might be due to invalid IL or missing references)
			//IL_056a: Unknown result type (might be due to invalid IL or missing references)
			//IL_056c: Unknown result type (might be due to invalid IL or missing references)
			//IL_056d: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0570: Unknown result type (might be due to invalid IL or missing references)
			//IL_058a: Unknown result type (might be due to invalid IL or missing references)
			//IL_059f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0903: Unknown result type (might be due to invalid IL or missing references)
			//IL_0908: Unknown result type (might be due to invalid IL or missing references)
			//IL_0919: Unknown result type (might be due to invalid IL or missing references)
			//IL_091e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0923: Unknown result type (might be due to invalid IL or missing references)
			//IL_0928: Unknown result type (might be due to invalid IL or missing references)
			//IL_092d: Unknown result type (might be due to invalid IL or missing references)
			//IL_092f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0930: Unknown result type (might be due to invalid IL or missing references)
			//IL_0941: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05da: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_061f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_0539: Unknown result type (might be due to invalid IL or missing references)
			//IL_0551: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_083b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0840: Unknown result type (might be due to invalid IL or missing references)
			ControlPoint startPoint = m_ControlPoints[0];
			Entity val = m_Owner;
			Entity val2 = m_Original;
			Entity updatedTopLevel = Entity.Null;
			Entity lotEntity = Entity.Null;
			OwnerDefinition ownerDefinition = default(OwnerDefinition);
			bool upgrade = false;
			bool flag = val2 != Entity.Null;
			bool topLevel = true;
			int parentMesh = ((!(val != Entity.Null)) ? (-1) : 0);
			if (!flag && m_PrefabNetObjectData.HasComponent(m_ObjectPrefab) && m_AttachedData.HasComponent(startPoint.m_OriginalEntity) && (m_EditorMode || !m_OwnerData.HasComponent(startPoint.m_OriginalEntity)))
			{
				Attached attached = m_AttachedData[startPoint.m_OriginalEntity];
				if (m_NodeData.HasComponent(attached.m_Parent) || m_EdgeData.HasComponent(attached.m_Parent))
				{
					val2 = startPoint.m_OriginalEntity;
					startPoint.m_OriginalEntity = attached.m_Parent;
					upgrade = true;
				}
			}
			Owner owner2 = default(Owner);
			if (m_EditorMode)
			{
				Entity val3 = startPoint.m_OriginalEntity;
				int num = startPoint.m_ElementIndex.x;
				while (m_OwnerData.HasComponent(val3) && !m_BuildingData.HasComponent(val3))
				{
					if (m_LocalTransformCacheData.HasComponent(val3))
					{
						num = m_LocalTransformCacheData[val3].m_ParentMesh;
						num += math.select(1000, -1000, num < 0);
					}
					val3 = m_OwnerData[val3].m_Owner;
				}
				DynamicBuffer<InstalledUpgrade> val4 = default(DynamicBuffer<InstalledUpgrade>);
				if (m_InstalledUpgrades.TryGetBuffer(val3, ref val4) && val4.Length != 0)
				{
					val3 = val4[0].m_Upgrade;
				}
				bool flag2 = false;
				PrefabRef prefabRef = default(PrefabRef);
				DynamicBuffer<ServiceUpgradeBuilding> val5 = default(DynamicBuffer<ServiceUpgradeBuilding>);
				if (m_PrefabRefData.TryGetComponent(val3, ref prefabRef) && m_PrefabServiceUpgradeBuilding.TryGetBuffer(m_ObjectPrefab, ref val5))
				{
					Entity val6 = Entity.Null;
					Transform transform = default(Transform);
					BuildingExtensionData buildingExtensionData = default(BuildingExtensionData);
					if (m_TransformData.TryGetComponent(val3, ref transform) && m_PrefabBuildingExtensionData.TryGetComponent(m_ObjectPrefab, ref buildingExtensionData))
					{
						for (int i = 0; i < val5.Length; i++)
						{
							if (val5[i].m_Building == prefabRef.m_Prefab)
							{
								val6 = val3;
								startPoint.m_Position = ObjectUtils.LocalToWorld(transform, buildingExtensionData.m_Position);
								startPoint.m_Rotation = transform.m_Rotation;
								break;
							}
						}
					}
					val3 = val6;
					flag2 = true;
				}
				if (m_TransformData.HasComponent(val3) && m_SubObjects.HasBuffer(val3))
				{
					val = val3;
					topLevel = flag2;
					parentMesh = num;
				}
				if (m_OwnerData.HasComponent(val2))
				{
					Owner owner = m_OwnerData[val2];
					if (owner.m_Owner != val)
					{
						val = owner.m_Owner;
						topLevel = flag2;
						parentMesh = -1;
					}
				}
				if (!m_EdgeData.HasComponent(startPoint.m_OriginalEntity) && !m_NodeData.HasComponent(startPoint.m_OriginalEntity))
				{
					startPoint.m_OriginalEntity = Entity.Null;
				}
			}
			else if (flag && val == Entity.Null && m_OwnerData.TryGetComponent(val2, ref owner2))
			{
				val = owner2.m_Owner;
			}
			NativeHashSet<Entity> attachedEntities = default(NativeHashSet<Entity>);
			NativeList<ClearAreaData> clearAreas = default(NativeList<ClearAreaData>);
			if (m_TransformData.HasComponent(val))
			{
				Transform transform2 = m_TransformData[val];
				Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
				m_ElevationData.TryGetComponent(val, ref elevation);
				Entity owner3 = Entity.Null;
				if (m_OwnerData.HasComponent(val))
				{
					owner3 = m_OwnerData[val].m_Owner;
				}
				ownerDefinition.m_Prefab = m_PrefabRefData[val].m_Prefab;
				ownerDefinition.m_Position = transform2.m_Position;
				ownerDefinition.m_Rotation = transform2.m_Rotation;
				if (m_Stamping || CheckParentPrefab(ownerDefinition.m_Prefab, m_ObjectPrefab))
				{
					updatedTopLevel = val;
					if (m_PrefabServiceUpgradeBuilding.HasBuffer(m_ObjectPrefab))
					{
						ClearAreaHelpers.FillClearAreas(ownerTransform: new Transform(startPoint.m_Position, startPoint.m_Rotation), ownerPrefab: m_ObjectPrefab, prefabObjectGeometryData: m_PrefabObjectGeometryData, prefabAreaGeometryData: m_PrefabAreaGeometryData, prefabSubAreas: m_PrefabSubAreas, prefabSubAreaNodes: m_PrefabSubAreaNodes, clearAreas: ref clearAreas);
						ClearAreaHelpers.InitClearAreas(clearAreas, transform2);
						if (val2 == Entity.Null)
						{
							lotEntity = val;
						}
					}
					bool flag3 = m_ObjectPrefab == Entity.Null;
					Entity parent = Entity.Null;
					DynamicBuffer<InstalledUpgrade> installedUpgrades = default(DynamicBuffer<InstalledUpgrade>);
					if (flag3 && m_InstalledUpgrades.TryGetBuffer(val, ref installedUpgrades))
					{
						ClearAreaHelpers.FillClearAreas(installedUpgrades, Entity.Null, m_TransformData, m_AreaClearData, m_PrefabRefData, m_PrefabObjectGeometryData, m_SubAreas, m_AreaNodes, m_AreaTriangles, ref clearAreas);
						ClearAreaHelpers.InitClearAreas(clearAreas, transform2);
					}
					Attached attached2 = default(Attached);
					if (flag3 && m_AttachedData.TryGetComponent(val, ref attached2) && m_BuildingData.HasComponent(attached2.m_Parent))
					{
						Transform transform3 = m_TransformData[attached2.m_Parent];
						parent = m_PrefabRefData[attached2.m_Parent].m_Prefab;
						UpdateObject(Entity.Null, Entity.Null, Entity.Null, attached2.m_Parent, Entity.Null, attached2.m_Parent, Entity.Null, transform3, 0f, default(OwnerDefinition), ref attachedEntities, clearAreas, upgrade: false, relocate: false, rebuild: false, topLevel: true, optional: false, -1, -1);
					}
					UpdateObject(Entity.Null, Entity.Null, owner3, val, parent, updatedTopLevel, Entity.Null, transform2, elevation.m_Elevation, default(OwnerDefinition), ref attachedEntities, clearAreas, upgrade: true, relocate: false, flag3, topLevel: true, optional: false, -1, -1);
					Attachment attachment = default(Attachment);
					if (m_AttachmentData.TryGetComponent(val, ref attachment) && m_BuildingData.HasComponent(attachment.m_Attached))
					{
						Transform transform4 = m_TransformData[attachment.m_Attached];
						parent = m_PrefabRefData[val].m_Prefab;
						UpdateObject(Entity.Null, Entity.Null, Entity.Null, attachment.m_Attached, parent, attachment.m_Attached, Entity.Null, transform4, 0f, default(OwnerDefinition), ref attachedEntities, clearAreas, upgrade: true, relocate: false, rebuild: false, topLevel: true, optional: false, -1, -1);
					}
					if (clearAreas.IsCreated)
					{
						clearAreas.Clear();
					}
				}
				else
				{
					ownerDefinition = default(OwnerDefinition);
				}
			}
			DynamicBuffer<InstalledUpgrade> installedUpgrades2 = default(DynamicBuffer<InstalledUpgrade>);
			if (val2 != Entity.Null && m_InstalledUpgrades.TryGetBuffer(val2, ref installedUpgrades2))
			{
				ClearAreaHelpers.FillClearAreas(installedUpgrades2, Entity.Null, m_TransformData, m_AreaClearData, m_PrefabRefData, m_PrefabObjectGeometryData, m_SubAreas, m_AreaNodes, m_AreaTriangles, ref clearAreas);
				ClearAreaHelpers.TransformClearAreas(clearAreas, m_TransformData[val2], new Transform(startPoint.m_Position, startPoint.m_Rotation));
				ClearAreaHelpers.InitClearAreas(clearAreas, new Transform(startPoint.m_Position, startPoint.m_Rotation));
			}
			if (m_ObjectPrefab != Entity.Null)
			{
				if (m_BrushPrefab != Entity.Null)
				{
					if (m_ControlPoints.Length >= 2)
					{
						CreateBrushes(startPoint, m_ControlPoints[1], updatedTopLevel, ownerDefinition, ref attachedEntities, clearAreas, topLevel, parentMesh);
					}
				}
				else if (m_Distance > 0f)
				{
					CreateCurve(startPoint, m_ControlPoints[math.min(1, m_ControlPoints.Length - 1)], m_ControlPoints[m_ControlPoints.Length - 1], updatedTopLevel, ownerDefinition, ref attachedEntities, clearAreas, topLevel, parentMesh);
				}
				else
				{
					Entity val7 = m_ObjectPrefab;
					DynamicBuffer<PlaceholderObjectElement> val8 = default(DynamicBuffer<PlaceholderObjectElement>);
					if (val2 == Entity.Null && (!m_EditorMode || ownerDefinition.m_Prefab == Entity.Null) && m_PrefabPlaceholderElements.TryGetBuffer(m_ObjectPrefab, ref val8) && !m_PrefabCreatureSpawnData.HasComponent(m_ObjectPrefab))
					{
						Random random = m_RandomSeed.GetRandom(1000000);
						int num2 = 0;
						for (int j = 0; j < val8.Length; j++)
						{
							if (GetVariationData(val8[j], out var variation))
							{
								num2 += variation.m_Probability;
								if (((Random)(ref random)).NextInt(num2) < variation.m_Probability)
								{
									val7 = variation.m_Prefab;
								}
							}
						}
					}
					UpdateObject(val7, m_TransformPrefab, Entity.Null, val2, startPoint.m_OriginalEntity, updatedTopLevel, lotEntity, new Transform(startPoint.m_Position, startPoint.m_Rotation), startPoint.m_Elevation, ownerDefinition, ref attachedEntities, clearAreas, upgrade, flag, rebuild: false, topLevel, optional: false, parentMesh, 0);
					if (m_AttachmentPrefab.IsCreated && m_AttachmentPrefab.Value.m_Entity != Entity.Null)
					{
						Transform transform5 = new Transform(startPoint.m_Position, startPoint.m_Rotation);
						ref float3 position = ref transform5.m_Position;
						position += math.rotate(transform5.m_Rotation, m_AttachmentPrefab.Value.m_Offset);
						UpdateObject(m_AttachmentPrefab.Value.m_Entity, Entity.Null, Entity.Null, Entity.Null, val7, updatedTopLevel, Entity.Null, transform5, startPoint.m_Elevation, ownerDefinition, ref attachedEntities, clearAreas, upgrade: false, relocate: false, rebuild: false, topLevel, optional: false, parentMesh, 0);
					}
				}
			}
			if (attachedEntities.IsCreated)
			{
				attachedEntities.Dispose();
			}
			if (clearAreas.IsCreated)
			{
				clearAreas.Dispose();
			}
		}

		private bool GetVariationData(PlaceholderObjectElement placeholder, out VariationData variation)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			variation = new VariationData
			{
				m_Prefab = placeholder.m_Object,
				m_Probability = 100
			};
			DynamicBuffer<ObjectRequirementElement> val = default(DynamicBuffer<ObjectRequirementElement>);
			if (m_PrefabRequirementElements.TryGetBuffer(variation.m_Prefab, ref val))
			{
				int num = -1;
				bool flag = true;
				for (int i = 0; i < val.Length; i++)
				{
					ObjectRequirementElement objectRequirementElement = val[i];
					if (objectRequirementElement.m_Group != num)
					{
						if (!flag)
						{
							break;
						}
						num = objectRequirementElement.m_Group;
						flag = false;
					}
					flag |= m_Theme == objectRequirementElement.m_Requirement;
				}
				if (!flag)
				{
					return false;
				}
			}
			SpawnableObjectData spawnableObjectData = default(SpawnableObjectData);
			if (m_PrefabSpawnableObjectData.TryGetComponent(variation.m_Prefab, ref spawnableObjectData))
			{
				variation.m_Probability = spawnableObjectData.m_Probability;
			}
			return true;
		}

		private void CreateCurve(ControlPoint startPoint, ControlPoint middlePoint, ControlPoint endPoint, Entity updatedTopLevel, OwnerDefinition ownerDefinition, ref NativeHashSet<Entity> attachedEntities, NativeList<ClearAreaData> clearAreas, bool topLevel, int parentMesh)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			Bezier4x3 val = default(Bezier4x3);
			bool flag = false;
			float num = math.distance(((float3)(ref startPoint.m_Position)).xz, ((float3)(ref endPoint.m_Position)).xz);
			float2 xz = ((float3)(ref startPoint.m_Position)).xz;
			if (!((float2)(ref xz)).Equals(((float3)(ref middlePoint.m_Position)).xz))
			{
				xz = ((float3)(ref endPoint.m_Position)).xz;
				if (!((float2)(ref xz)).Equals(((float3)(ref middlePoint.m_Position)).xz))
				{
					float3 val2 = middlePoint.m_Position - startPoint.m_Position;
					float3 val3 = endPoint.m_Position - middlePoint.m_Position;
					val2 = MathUtils.Normalize(val2, ((float3)(ref val2)).xz);
					val3 = MathUtils.Normalize(val3, ((float3)(ref val3)).xz);
					val = NetUtils.FitCurve(startPoint.m_Position, val2, val3, endPoint.m_Position);
					flag = true;
					num = MathUtils.Length(((Bezier4x3)(ref val)).xz);
				}
			}
			PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
			m_PrefabPlaceableObjectData.TryGetComponent(m_ObjectPrefab, ref placeableObjectData);
			NativeList<VariationData> val4 = default(NativeList<VariationData>);
			DynamicBuffer<PlaceholderObjectElement> val5 = default(DynamicBuffer<PlaceholderObjectElement>);
			if ((!m_EditorMode || ownerDefinition.m_Prefab == Entity.Null) && m_PrefabPlaceholderElements.TryGetBuffer(m_ObjectPrefab, ref val5) && !m_PrefabCreatureSpawnData.HasComponent(m_ObjectPrefab))
			{
				val4._002Ector(val5.Length, AllocatorHandle.op_Implicit((Allocator)2));
				for (int i = 0; i < val5.Length; i++)
				{
					if (GetVariationData(val5[i], out var variation))
					{
						val4.Add(ref variation);
					}
				}
			}
			Random random = m_RandomSeed.GetRandom(1000000);
			int num2 = (int)(num / m_Distance + 1.5f);
			float num3 = 1f / (float)math.max(1, num2 - 1);
			Bounds1 val8 = default(Bounds1);
			for (int j = 0; j < num2; j++)
			{
				float num4 = (float)j * num3;
				float3 val6 = startPoint.m_Position;
				quaternion val7 = startPoint.m_Rotation;
				if (j != 0)
				{
					float num5 = (float)Math.PI * 2f;
					num5 = ((placeableObjectData.m_RotationSymmetry == RotationSymmetry.Any) ? ((Random)(ref random)).NextFloat(num5) : ((placeableObjectData.m_RotationSymmetry == RotationSymmetry.None) ? 0f : (num5 * ((float)((Random)(ref random)).NextInt((int)placeableObjectData.m_RotationSymmetry) / (float)(int)placeableObjectData.m_RotationSymmetry))));
					if (flag)
					{
						((Bounds1)(ref val8))._002Ector(0f, 1f);
						if (j < num2 - 1 && MathUtils.ClampLength(((Bezier4x3)(ref val)).xz, ref val8, num4 * num))
						{
							num4 = val8.max;
						}
						val6 = MathUtils.Position(val, num4);
						float3 val9 = MathUtils.StartTangent(val);
						float2 xz2 = ((float3)(ref val9)).xz;
						val9 = MathUtils.Tangent(val, num4);
						float2 xz3 = ((float3)(ref val9)).xz;
						if (MathUtils.TryNormalize(ref xz2) && MathUtils.TryNormalize(ref xz3))
						{
							num5 += MathUtils.RotationAngleRight(xz2, xz3);
						}
					}
					else
					{
						val6 = math.lerp(startPoint.m_Position, endPoint.m_Position, num4);
					}
					if (num5 != 0f)
					{
						val7 = math.normalizesafe(math.mul(val7, quaternion.RotateY(num5)), quaternion.identity);
					}
				}
				float elevation;
				Transform transform = SampleTransform(placeableObjectData, val6, val7, out elevation);
				if (parentMesh != -1)
				{
					transform.m_Position.y = val6.y;
					transform.m_Rotation = val7;
					elevation = startPoint.m_Elevation;
				}
				Entity val10 = Entity.Null;
				if (val4.IsCreated)
				{
					int num6 = 0;
					for (int k = 0; k < val4.Length; k++)
					{
						VariationData variationData = val4[k];
						num6 += variationData.m_Probability;
						if (((Random)(ref random)).NextInt(num6) < variationData.m_Probability)
						{
							val10 = variationData.m_Prefab;
						}
					}
				}
				else
				{
					val10 = m_ObjectPrefab;
				}
				if (val10 != Entity.Null)
				{
					UpdateObject(val10, m_TransformPrefab, Entity.Null, Entity.Null, Entity.Null, updatedTopLevel, Entity.Null, transform, elevation, ownerDefinition, ref attachedEntities, clearAreas, upgrade: false, relocate: false, rebuild: false, topLevel, optional: false, parentMesh, j);
				}
			}
			if (val4.IsCreated)
			{
				val4.Dispose();
			}
		}

		private void CreateBrushes(ControlPoint startPoint, ControlPoint endPoint, Entity updatedTopLevel, OwnerDefinition ownerDefinition, ref NativeHashSet<Entity> attachedEntities, NativeList<ClearAreaData> clearAreas, bool topLevel, int parentMesh)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_054e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_0555: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_0566: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a94: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a80: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_061b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0620: Unknown result type (might be due to invalid IL or missing references)
			//IL_0646: Unknown result type (might be due to invalid IL or missing references)
			//IL_0925: Unknown result type (might be due to invalid IL or missing references)
			//IL_092a: Unknown result type (might be due to invalid IL or missing references)
			//IL_092f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0654: Unknown result type (might be due to invalid IL or missing references)
			//IL_0935: Unknown result type (might be due to invalid IL or missing references)
			//IL_0974: Unknown result type (might be due to invalid IL or missing references)
			//IL_097c: Unknown result type (might be due to invalid IL or missing references)
			//IL_097e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0983: Unknown result type (might be due to invalid IL or missing references)
			//IL_0985: Unknown result type (might be due to invalid IL or missing references)
			//IL_098c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0991: Unknown result type (might be due to invalid IL or missing references)
			//IL_099e: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0951: Unknown result type (might be due to invalid IL or missing references)
			//IL_0678: Unknown result type (might be due to invalid IL or missing references)
			//IL_0684: Unknown result type (might be due to invalid IL or missing references)
			//IL_0689: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0697: Unknown result type (might be due to invalid IL or missing references)
			//IL_069e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0700: Unknown result type (might be due to invalid IL or missing references)
			//IL_0705: Unknown result type (might be due to invalid IL or missing references)
			//IL_070e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0713: Unknown result type (might be due to invalid IL or missing references)
			//IL_0723: Unknown result type (might be due to invalid IL or missing references)
			//IL_0728: Unknown result type (might be due to invalid IL or missing references)
			//IL_0731: Unknown result type (might be due to invalid IL or missing references)
			//IL_0736: Unknown result type (might be due to invalid IL or missing references)
			//IL_073f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0744: Unknown result type (might be due to invalid IL or missing references)
			//IL_074d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0752: Unknown result type (might be due to invalid IL or missing references)
			//IL_075e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0765: Unknown result type (might be due to invalid IL or missing references)
			//IL_076c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0773: Unknown result type (might be due to invalid IL or missing references)
			//IL_077a: Unknown result type (might be due to invalid IL or missing references)
			//IL_077f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0781: Unknown result type (might be due to invalid IL or missing references)
			//IL_0786: Unknown result type (might be due to invalid IL or missing references)
			//IL_0788: Unknown result type (might be due to invalid IL or missing references)
			//IL_078d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0792: Unknown result type (might be due to invalid IL or missing references)
			//IL_0797: Unknown result type (might be due to invalid IL or missing references)
			//IL_0799: Unknown result type (might be due to invalid IL or missing references)
			//IL_079c: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
			//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0800: Unknown result type (might be due to invalid IL or missing references)
			//IL_0802: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a64: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0813: Unknown result type (might be due to invalid IL or missing references)
			//IL_083f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0844: Unknown result type (might be due to invalid IL or missing references)
			//IL_084c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0854: Unknown result type (might be due to invalid IL or missing references)
			//IL_0859: Unknown result type (might be due to invalid IL or missing references)
			//IL_085b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0862: Unknown result type (might be due to invalid IL or missing references)
			//IL_086a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0872: Unknown result type (might be due to invalid IL or missing references)
			//IL_0877: Unknown result type (might be due to invalid IL or missing references)
			//IL_087b: Unknown result type (might be due to invalid IL or missing references)
			//IL_087d: Unknown result type (might be due to invalid IL or missing references)
			//IL_087f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0884: Unknown result type (might be due to invalid IL or missing references)
			//IL_0886: Unknown result type (might be due to invalid IL or missing references)
			//IL_0888: Unknown result type (might be due to invalid IL or missing references)
			//IL_088d: Unknown result type (might be due to invalid IL or missing references)
			//IL_088f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0891: Unknown result type (might be due to invalid IL or missing references)
			//IL_0896: Unknown result type (might be due to invalid IL or missing references)
			//IL_0898: Unknown result type (might be due to invalid IL or missing references)
			//IL_089a: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
			if (endPoint.Equals(default(ControlPoint)))
			{
				return;
			}
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Prefab = m_BrushPrefab
			};
			BrushDefinition brushDefinition = new BrushDefinition
			{
				m_Tool = m_ObjectPrefab
			};
			if (startPoint.Equals(default(ControlPoint)))
			{
				brushDefinition.m_Line = new Segment(endPoint.m_Position, endPoint.m_Position);
			}
			else
			{
				brushDefinition.m_Line = new Segment(startPoint.m_Position, endPoint.m_Position);
			}
			brushDefinition.m_Size = m_BrushSize;
			brushDefinition.m_Angle = m_BrushAngle;
			brushDefinition.m_Strength = m_BrushStrength;
			brushDefinition.m_Time = m_DeltaTime;
			Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val, creationDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BrushDefinition>(val, brushDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
			BrushData brushData = m_PrefabBrushData[m_BrushPrefab];
			DynamicBuffer<BrushCell> brushCells = m_PrefabBrushCells[m_BrushPrefab];
			if (math.any(brushData.m_Resolution == 0) || brushCells.Length == 0)
			{
				return;
			}
			float num = MathUtils.Length(brushDefinition.m_Line);
			int num2 = 1 + Mathf.FloorToInt(num / (m_BrushSize * 0.25f));
			float num3 = m_BrushStrength * m_BrushStrength * math.saturate(m_DeltaTime * 10f);
			quaternion val2 = quaternion.RotateY(m_BrushAngle);
			float3 val3 = math.mul(val2, new float3(1f, 0f, 0f));
			float2 xz = ((float3)(ref val3)).xz;
			val3 = math.mul(val2, new float3(0f, 0f, 1f));
			float2 xz2 = ((float3)(ref val3)).xz;
			float num4 = 16f;
			float2 val4 = (math.abs(xz) + math.abs(xz2)) * (m_BrushSize * 0.5f);
			Bounds2 val5 = default(Bounds2);
			((Bounds2)(ref val5))._002Ector(float2.op_Implicit(float.MaxValue), float2.op_Implicit(float.MinValue));
			for (int i = 1; i <= num2; i++)
			{
				float3 val6 = MathUtils.Position(brushDefinition.m_Line, (float)i / (float)num2);
				val5 |= new Bounds2(((float3)(ref val6)).xz - val4, ((float3)(ref val6)).xz + val4);
			}
			float2 val7 = m_BrushSize / float2.op_Implicit(brushData.m_Resolution);
			float2 val8 = 1f / val7;
			float4 xyxy = ((float2)(ref val8)).xyxy;
			val8 = float2.op_Implicit(brushData.m_Resolution) * 0.5f;
			float4 xyxy2 = ((float2)(ref val8)).xyxy;
			float num5 = 1f / ((float)num2 * num4 * num4);
			if (m_Removing)
			{
				BrushIterator brushIterator = new BrushIterator
				{
					m_Bounds = val5,
					m_RandomSeed = m_RandomSeed,
					m_BrushLine = brushDefinition.m_Line,
					m_BrushCellSizeFactor = xyxy,
					m_BrushTextureSizeAdd = xyxy2,
					m_BrushDirX = xz,
					m_BrushDirZ = xz2,
					m_BrushCellSize = val7,
					m_BrushResolution = brushData.m_Resolution,
					m_TileSize = num4,
					m_BrushStrength = num3,
					m_StrengthFactor = num5,
					m_BrushCount = num2,
					m_BrushCells = brushCells,
					m_OwnerData = m_OwnerData,
					m_TransformData = m_TransformData,
					m_ElevationData = m_ElevationData,
					m_EditorContainerData = m_EditorContainerData,
					m_LocalTransformCacheData = m_LocalTransformCacheData,
					m_PrefabRefData = m_PrefabRefData,
					m_PrefabObjectGeometryData = m_PrefabObjectGeometryData,
					m_CommandBuffer = m_CommandBuffer
				};
				if ((m_Snap & Snap.PrefabType) != Snap.None && m_ObjectPrefab != Entity.Null)
				{
					DynamicBuffer<PlaceholderObjectElement> val9 = default(DynamicBuffer<PlaceholderObjectElement>);
					if (m_PrefabPlaceholderElements.TryGetBuffer(m_ObjectPrefab, ref val9))
					{
						brushIterator.m_RequirePrefab = new NativeParallelHashSet<Entity>(1 + val9.Length, AllocatorHandle.op_Implicit((Allocator)2));
						brushIterator.m_RequirePrefab.Add(m_ObjectPrefab);
						for (int j = 0; j < val9.Length; j++)
						{
							brushIterator.m_RequirePrefab.Add(val9[j].m_Object);
						}
					}
					else
					{
						brushIterator.m_RequirePrefab = new NativeParallelHashSet<Entity>(1, AllocatorHandle.op_Implicit((Allocator)2));
						brushIterator.m_RequirePrefab.Add(m_ObjectPrefab);
					}
				}
				m_ObjectSearchTree.Iterate<BrushIterator>(ref brushIterator, 0);
				if (brushIterator.m_RequirePrefab.IsCreated)
				{
					brushIterator.m_RequirePrefab.Dispose();
				}
				return;
			}
			PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
			m_PrefabPlaceableObjectData.TryGetComponent(m_ObjectPrefab, ref placeableObjectData);
			NativeList<VariationData> val10 = default(NativeList<VariationData>);
			DynamicBuffer<PlaceholderObjectElement> val11 = default(DynamicBuffer<PlaceholderObjectElement>);
			if (m_PrefabPlaceholderElements.TryGetBuffer(m_ObjectPrefab, ref val11) && !m_PrefabCreatureSpawnData.HasComponent(m_ObjectPrefab))
			{
				val10._002Ector(val11.Length, AllocatorHandle.op_Implicit((Allocator)2));
				for (int k = 0; k < val11.Length; k++)
				{
					if (GetVariationData(val11[k], out var variation))
					{
						val10.Add(ref variation);
					}
				}
			}
			int4 val12 = (int4)math.floor(new float4(val5.min, val5.max) / num4);
			int2 val13 = int2.op_Implicit(0);
			Bounds2 val14 = default(Bounds2);
			float4 val17 = default(float4);
			float4 val18 = default(float4);
			float4 val19 = default(float4);
			Quad2 val25 = default(Quad2);
			float num13 = default(float);
			for (int l = 0; l < 3; l++)
			{
				float num6 = 0f;
				bool flag = false;
				for (int m = val12.y; m <= val12.w; m++)
				{
					val14.min.y = (float)m * num4;
					val14.max.y = val14.min.y + num4;
					for (int n = val12.x; n <= val12.z; n++)
					{
						val14.min.x = (float)n * num4;
						val14.max.x = val14.min.x + num4;
						int index = ((m & 0xFFFF) << 16) | (n & 0xFFFF);
						Random random = m_RandomSeed.GetRandom(index);
						float num7 = ((Random)(ref random)).NextFloat(4f);
						switch (l)
						{
						case 0:
							if (num7 >= num3)
							{
								continue;
							}
							break;
						case 2:
							if (n != val13.x || m != val13.y)
							{
								continue;
							}
							break;
						}
						float num8 = 0f;
						if (l != 2)
						{
							for (int num9 = 1; num9 <= num2; num9++)
							{
								float3 val15 = MathUtils.Position(brushDefinition.m_Line, (float)num9 / (float)num2);
								Bounds2 val16 = val14;
								ref float2 min = ref val16.min;
								min -= ((float3)(ref val15)).xz;
								ref float2 max = ref val16.max;
								max -= ((float3)(ref val15)).xz;
								((float4)(ref val17))._002Ector(val16.min, val16.max);
								((float4)(ref val18))._002Ector(math.dot(((float4)(ref val17)).xy, xz), math.dot(((float4)(ref val17)).xw, xz), math.dot(((float4)(ref val17)).zy, xz), math.dot(((float4)(ref val17)).zw, xz));
								((float4)(ref val19))._002Ector(math.dot(((float4)(ref val17)).xy, xz2), math.dot(((float4)(ref val17)).xw, xz2), math.dot(((float4)(ref val17)).zy, xz2), math.dot(((float4)(ref val17)).zw, xz2));
								int4 val20 = (int4)math.floor(new float4(math.cmin(val18), math.cmin(val19), math.cmax(val18), math.cmax(val19)) * xyxy + xyxy2);
								val20 = math.clamp(val20, int4.op_Implicit(0), ((int2)(ref brushData.m_Resolution)).xyxy - 1);
								for (int num10 = val20.y; num10 <= val20.w; num10++)
								{
									float2 val21 = xz2 * (((float)num10 - xyxy2.y) * val7.y);
									float2 val22 = xz2 * (((float)(num10 + 1) - xyxy2.y) * val7.y);
									for (int num11 = val20.x; num11 <= val20.z; num11++)
									{
										int num12 = num11 + brushData.m_Resolution.x * num10;
										BrushCell brushCell = brushCells[num12];
										if (brushCell.m_Opacity >= 0.0001f)
										{
											float2 val23 = xz * (((float)num11 - xyxy2.x) * val7.x);
											float2 val24 = xz * (((float)(num11 + 1) - xyxy2.x) * val7.x);
											((Quad2)(ref val25))._002Ector(val21 + val23, val21 + val24, val22 + val24, val22 + val23);
											if (MathUtils.Intersect(val16, val25, ref num13))
											{
												num8 += brushCell.m_Opacity * num13;
											}
										}
									}
								}
							}
							num8 *= num5;
							if (math.abs(num8) < 0.0001f)
							{
								continue;
							}
						}
						float4 val26 = ((Random)(ref random)).NextFloat4(new float4(1f, 1f, 1f, (float)Math.PI * 2f));
						switch (l)
						{
						case 0:
							if (val26.x >= num8)
							{
								continue;
							}
							break;
						case 1:
							num6 += num8;
							if (val26.x * num6 < num8)
							{
								((int2)(ref val13))._002Ector(n, m);
							}
							continue;
						}
						float3 position = default(float3);
						((float3)(ref position)).xz = math.lerp(val14.min, val14.max, ((float4)(ref val26)).yz);
						float elevation;
						Transform transform = SampleTransform(placeableObjectData, position, quaternion.RotateY(val26.w), out elevation);
						Entity val27 = Entity.Null;
						if (val10.IsCreated)
						{
							int num14 = 0;
							for (int num15 = 0; num15 < val10.Length; num15++)
							{
								VariationData variationData = val10[num15];
								num14 += variationData.m_Probability;
								if (((Random)(ref random)).NextInt(num14) < variationData.m_Probability)
								{
									val27 = variationData.m_Prefab;
								}
							}
						}
						else
						{
							val27 = m_ObjectPrefab;
						}
						if (val27 != Entity.Null)
						{
							index = ((n & 0xFFFF) << 16) | (m & 0xFFFF);
							UpdateObject(val27, m_TransformPrefab, Entity.Null, Entity.Null, Entity.Null, updatedTopLevel, Entity.Null, transform, elevation, ownerDefinition, ref attachedEntities, clearAreas, upgrade: false, relocate: false, rebuild: false, topLevel, optional: true, parentMesh, index);
						}
						flag = true;
					}
				}
				if (flag)
				{
					break;
				}
			}
			if (val10.IsCreated)
			{
				val10.Dispose();
			}
		}

		private Transform SampleTransform(PlaceableObjectData placeableObjectData, float3 position, quaternion rotation, out float elevation)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			float3 normal;
			float num = TerrainUtils.SampleHeight(ref m_TerrainHeightData, position, out normal);
			elevation = 0f;
			if ((placeableObjectData.m_Flags & Game.Objects.PlacementFlags.Hovering) != Game.Objects.PlacementFlags.None)
			{
				float num2 = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, position);
				num2 += placeableObjectData.m_PlacementOffset.y;
				elevation = math.max(0f, num2 - num);
				num = math.max(num, num2);
			}
			else if ((placeableObjectData.m_Flags & (Game.Objects.PlacementFlags.Shoreline | Game.Objects.PlacementFlags.Floating)) == 0)
			{
				num += placeableObjectData.m_PlacementOffset.y;
			}
			else
			{
				float num3 = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, position, out var waterDepth);
				if (waterDepth >= 0.2f)
				{
					num3 += placeableObjectData.m_PlacementOffset.y;
					if ((placeableObjectData.m_Flags & Game.Objects.PlacementFlags.Floating) != Game.Objects.PlacementFlags.None)
					{
						elevation = math.max(0f, num3 - num);
					}
					num = math.max(num, num3);
				}
			}
			Transform result = default(Transform);
			result.m_Position = position;
			result.m_Position.y = num;
			result.m_Rotation = rotation;
			if ((m_Snap & Snap.Upright) == 0)
			{
				float3 val = math.cross(math.right(), normal);
				result.m_Rotation = math.mul(quaternion.LookRotation(val, normal), result.m_Rotation);
			}
			return result;
		}

		private bool CheckParentPrefab(Entity parentPrefab, Entity objectPrefab)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (parentPrefab == objectPrefab)
			{
				return false;
			}
			if (m_PrefabSubObjects.HasBuffer(objectPrefab))
			{
				DynamicBuffer<Game.Prefabs.SubObject> val = m_PrefabSubObjects[objectPrefab];
				for (int i = 0; i < val.Length; i++)
				{
					if (!CheckParentPrefab(parentPrefab, val[i].m_Prefab))
					{
						return false;
					}
				}
			}
			return true;
		}

		private void UpdateObject(Entity objectPrefab, Entity transformPrefab, Entity owner, Entity original, Entity parent, Entity updatedTopLevel, Entity lotEntity, Transform transform, float elevation, OwnerDefinition ownerDefinition, ref NativeHashSet<Entity> attachedEntities, NativeList<ClearAreaData> clearAreas, bool upgrade, bool relocate, bool rebuild, bool topLevel, bool optional, int parentMesh, int randomIndex)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0555: Unknown result type (might be due to invalid IL or missing references)
			//IL_0556: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05da: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_060a: Unknown result type (might be due to invalid IL or missing references)
			//IL_060b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0625: Unknown result type (might be due to invalid IL or missing references)
			//IL_0626: Unknown result type (might be due to invalid IL or missing references)
			//IL_0628: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0645: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_0578: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_0599: Unknown result type (might be due to invalid IL or missing references)
			//IL_059b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			OwnerDefinition ownerDefinition2 = ownerDefinition;
			Random random = m_RandomSeed.GetRandom(randomIndex);
			if (!m_PrefabAssetStampData.HasComponent(objectPrefab) || (!m_Stamping && ownerDefinition.m_Prefab == Entity.Null))
			{
				Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
				CreationDefinition creationDefinition = new CreationDefinition
				{
					m_Prefab = objectPrefab,
					m_SubPrefab = transformPrefab,
					m_Owner = owner,
					m_Original = original,
					m_RandomSeed = ((Random)(ref random)).NextInt()
				};
				if (optional)
				{
					creationDefinition.m_Flags |= CreationFlags.Optional;
				}
				if (objectPrefab == Entity.Null && m_PrefabRefData.HasComponent(original))
				{
					objectPrefab = m_PrefabRefData[original].m_Prefab;
				}
				if (m_PrefabBuildingData.HasComponent(objectPrefab))
				{
					parentMesh = -1;
				}
				ObjectDefinition objectDefinition = new ObjectDefinition
				{
					m_ParentMesh = parentMesh,
					m_Position = transform.m_Position,
					m_Rotation = transform.m_Rotation,
					m_Probability = 100,
					m_PrefabSubIndex = -1,
					m_Scale = float3.op_Implicit(1f),
					m_Intensity = 1f
				};
				if (original == Entity.Null && transformPrefab != Entity.Null)
				{
					objectDefinition.m_GroupIndex = -1;
				}
				if (m_PrefabPlaceableObjectData.HasComponent(objectPrefab))
				{
					PlaceableObjectData placeableObjectData = m_PrefabPlaceableObjectData[objectPrefab];
					if ((placeableObjectData.m_Flags & Game.Objects.PlacementFlags.HasProbability) != Game.Objects.PlacementFlags.None)
					{
						objectDefinition.m_Probability = placeableObjectData.m_DefaultProbability;
					}
				}
				if (m_EditorContainerData.HasComponent(original))
				{
					EditorContainer editorContainer = m_EditorContainerData[original];
					creationDefinition.m_SubPrefab = editorContainer.m_Prefab;
					objectDefinition.m_Scale = editorContainer.m_Scale;
					objectDefinition.m_Intensity = editorContainer.m_Intensity;
					objectDefinition.m_GroupIndex = editorContainer.m_GroupIndex;
				}
				if (m_LocalTransformCacheData.HasComponent(original))
				{
					LocalTransformCache localTransformCache = m_LocalTransformCacheData[original];
					objectDefinition.m_Probability = localTransformCache.m_Probability;
					objectDefinition.m_PrefabSubIndex = localTransformCache.m_PrefabSubIndex;
				}
				if (parentMesh != -1)
				{
					objectDefinition.m_Elevation = transform.m_Position.y - ownerDefinition.m_Position.y;
				}
				else
				{
					objectDefinition.m_Elevation = elevation;
				}
				if (m_EditorMode)
				{
					objectDefinition.m_Age = ((Random)(ref random)).NextFloat(1f);
				}
				else
				{
					objectDefinition.m_Age = ToolUtils.GetRandomAge(ref random, m_AgeMask);
				}
				if (ownerDefinition.m_Prefab != Entity.Null)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val, ownerDefinition);
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
				Entity val2 = Entity.Null;
				if (m_SubObjects.HasBuffer(parent))
				{
					creationDefinition.m_Flags |= CreationFlags.Attach;
					if (parentMesh == -1 && m_NetElevationData.HasComponent(parent))
					{
						objectDefinition.m_ParentMesh = 0;
						objectDefinition.m_Elevation = math.csum(m_NetElevationData[parent].m_Elevation) * 0.5f;
						if (IsLoweredParent(parent))
						{
							creationDefinition.m_Flags |= CreationFlags.Lowered;
						}
					}
					if (m_PrefabNetObjectData.HasComponent(objectPrefab))
					{
						val2 = parent;
						UpdateAttachedParent(parent, updatedTopLevel, ref attachedEntities);
					}
					else
					{
						creationDefinition.m_Attached = parent;
					}
				}
				else if (m_PlaceholderBuildingData.HasComponent(parent))
				{
					creationDefinition.m_Flags |= CreationFlags.Attach;
					creationDefinition.m_Attached = parent;
				}
				if (m_AttachedData.HasComponent(original))
				{
					Attached attached = m_AttachedData[original];
					if (attached.m_Parent != val2)
					{
						UpdateAttachedParent(attached.m_Parent, updatedTopLevel, ref attachedEntities);
					}
				}
				DynamicBuffer<Game.Objects.SubObject> val3 = default(DynamicBuffer<Game.Objects.SubObject>);
				if (relocate && m_SubObjects.TryGetBuffer(original, ref val3))
				{
					Attached attached2 = default(Attached);
					for (int i = 0; i < val3.Length; i++)
					{
						if (m_AttachedData.TryGetComponent(val3[i].m_SubObject, ref attached2))
						{
							UpdateAttachedParent(attached2.m_Parent, updatedTopLevel, ref attachedEntities);
						}
					}
				}
				if (upgrade)
				{
					creationDefinition.m_Flags |= CreationFlags.Upgrade | CreationFlags.Parent;
				}
				if (relocate)
				{
					creationDefinition.m_Flags |= CreationFlags.Relocate;
				}
				if (rebuild)
				{
					creationDefinition.m_Flags |= CreationFlags.Repair;
				}
				ownerDefinition2.m_Prefab = objectPrefab;
				ownerDefinition2.m_Position = objectDefinition.m_Position;
				ownerDefinition2.m_Rotation = objectDefinition.m_Rotation;
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val, creationDefinition);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<ObjectDefinition>(val, objectDefinition);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
			}
			else
			{
				if (m_PrefabSubObjects.HasBuffer(objectPrefab))
				{
					DynamicBuffer<Game.Prefabs.SubObject> val4 = m_PrefabSubObjects[objectPrefab];
					for (int j = 0; j < val4.Length; j++)
					{
						Game.Prefabs.SubObject subObject = val4[j];
						Transform transform4 = ObjectUtils.LocalToWorld(transform, subObject.m_Position, subObject.m_Rotation);
						UpdateObject(subObject.m_Prefab, Entity.Null, owner, Entity.Null, parent, updatedTopLevel, lotEntity, transform4, elevation, ownerDefinition, ref attachedEntities, default(NativeList<ClearAreaData>), upgrade: false, relocate: false, rebuild: false, topLevel: false, optional: false, parentMesh, j);
					}
				}
				original = Entity.Null;
				topLevel = true;
			}
			NativeParallelHashMap<Entity, int> selectedSpawnables = default(NativeParallelHashMap<Entity, int>);
			Transform mainInverseTransform = transform;
			if (original != Entity.Null)
			{
				mainInverseTransform = ObjectUtils.InverseTransform(m_TransformData[original]);
			}
			UpdateSubObjects(transform, transform, mainInverseTransform, objectPrefab, original, relocate, rebuild, topLevel, upgrade, ownerDefinition2, ref random, ref selectedSpawnables);
			UpdateSubNets(transform, transform, mainInverseTransform, objectPrefab, original, lotEntity, relocate, topLevel, ownerDefinition2, clearAreas, ref random);
			UpdateSubAreas(transform, objectPrefab, original, relocate, rebuild, topLevel, ownerDefinition2, clearAreas, ref random, ref selectedSpawnables);
			if (selectedSpawnables.IsCreated)
			{
				selectedSpawnables.Dispose();
			}
		}

		private void UpdateAttachedParent(Entity parent, Entity updatedTopLevel, ref NativeHashSet<Entity> attachedEntities)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			if (updatedTopLevel != Entity.Null)
			{
				Entity val = parent;
				if (val == updatedTopLevel)
				{
					return;
				}
				while (m_OwnerData.HasComponent(val))
				{
					val = m_OwnerData[val].m_Owner;
					if (val == updatedTopLevel)
					{
						return;
					}
				}
			}
			if (!attachedEntities.IsCreated)
			{
				attachedEntities = new NativeHashSet<Entity>(16, AllocatorHandle.op_Implicit((Allocator)2));
			}
			if (attachedEntities.Add(parent))
			{
				if (m_EdgeData.HasComponent(parent))
				{
					Edge edge = m_EdgeData[parent];
					Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
					CreationDefinition creationDefinition = new CreationDefinition
					{
						m_Original = parent
					};
					creationDefinition.m_Flags |= CreationFlags.Align;
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val2, creationDefinition);
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2, default(Updated));
					NetCourse netCourse = default(NetCourse);
					netCourse.m_Curve = m_CurveData[parent].m_Bezier;
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
				else if (m_NodeData.HasComponent(parent))
				{
					Game.Net.Node node = m_NodeData[parent];
					Entity val3 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
					CreationDefinition creationDefinition2 = new CreationDefinition
					{
						m_Original = parent
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
							m_Entity = parent,
							m_Position = node.m_Position,
							m_Rotation = node.m_Rotation,
							m_CourseDelta = 0f
						},
						m_EndPosition = 
						{
							m_Entity = parent,
							m_Position = node.m_Position,
							m_Rotation = node.m_Rotation,
							m_CourseDelta = 1f
						}
					};
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val3, netCourse2);
				}
			}
		}

		private bool IsLoweredParent(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			Composition composition = default(Composition);
			NetCompositionData netCompositionData = default(NetCompositionData);
			if (m_CompositionData.TryGetComponent(entity, ref composition) && m_PrefabCompositionData.TryGetComponent(composition.m_Edge, ref netCompositionData) && ((netCompositionData.m_Flags.m_Left | netCompositionData.m_Flags.m_Right) & CompositionFlags.Side.Lowered) != 0)
			{
				return true;
			}
			Orphan orphan = default(Orphan);
			if (m_OrphanData.TryGetComponent(entity, ref orphan) && m_PrefabCompositionData.TryGetComponent(orphan.m_Composition, ref netCompositionData) && ((netCompositionData.m_Flags.m_Left | netCompositionData.m_Flags.m_Right) & CompositionFlags.Side.Lowered) != 0)
			{
				return true;
			}
			DynamicBuffer<ConnectedEdge> val = default(DynamicBuffer<ConnectedEdge>);
			if (m_ConnectedEdges.TryGetBuffer(entity, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					ConnectedEdge connectedEdge = val[i];
					Edge edge = m_EdgeData[connectedEdge.m_Edge];
					if (edge.m_Start == entity)
					{
						if (m_CompositionData.TryGetComponent(connectedEdge.m_Edge, ref composition) && m_PrefabCompositionData.TryGetComponent(composition.m_StartNode, ref netCompositionData) && ((netCompositionData.m_Flags.m_Left | netCompositionData.m_Flags.m_Right) & CompositionFlags.Side.Lowered) != 0)
						{
							return true;
						}
					}
					else if (edge.m_End == entity && m_CompositionData.TryGetComponent(connectedEdge.m_Edge, ref composition) && m_PrefabCompositionData.TryGetComponent(composition.m_EndNode, ref netCompositionData) && ((netCompositionData.m_Flags.m_Left | netCompositionData.m_Flags.m_Right) & CompositionFlags.Side.Lowered) != 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void UpdateSubObjects(Transform transform, Transform mainTransform, Transform mainInverseTransform, Entity prefab, Entity original, bool relocate, bool rebuild, bool topLevel, bool isParent, OwnerDefinition ownerDefinition, ref Random random, ref NativeParallelHashMap<Entity, int> selectedSpawnables)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			if (!m_InstalledUpgrades.HasBuffer(original) || !m_TransformData.HasComponent(original))
			{
				return;
			}
			Transform inverseParentTransform = ObjectUtils.InverseTransform(m_TransformData[original]);
			DynamicBuffer<InstalledUpgrade> val = m_InstalledUpgrades[original];
			Transform transform2 = default(Transform);
			Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
			PrefabRef prefabRef = default(PrefabRef);
			PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
			for (int i = 0; i < val.Length; i++)
			{
				Entity upgrade = val[i].m_Upgrade;
				if (upgrade == m_Original || !m_TransformData.HasComponent(upgrade))
				{
					continue;
				}
				Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
				CreationDefinition creationDefinition = new CreationDefinition
				{
					m_Original = upgrade
				};
				if (relocate)
				{
					creationDefinition.m_Flags |= CreationFlags.Relocate;
				}
				if (rebuild)
				{
					creationDefinition.m_Flags |= CreationFlags.Repair;
				}
				if (isParent)
				{
					creationDefinition.m_Flags |= CreationFlags.Parent;
					if (m_ObjectPrefab == Entity.Null)
					{
						creationDefinition.m_Flags |= CreationFlags.Upgrade;
					}
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val2, creationDefinition);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2, default(Updated));
				if (ownerDefinition.m_Prefab != Entity.Null)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val2, ownerDefinition);
				}
				ObjectDefinition objectDefinition = new ObjectDefinition
				{
					m_Probability = 100,
					m_PrefabSubIndex = -1
				};
				if (m_LocalTransformCacheData.HasComponent(upgrade))
				{
					LocalTransformCache localTransformCache = m_LocalTransformCacheData[upgrade];
					objectDefinition.m_ParentMesh = localTransformCache.m_ParentMesh;
					objectDefinition.m_GroupIndex = localTransformCache.m_GroupIndex;
					objectDefinition.m_Probability = localTransformCache.m_Probability;
					objectDefinition.m_PrefabSubIndex = localTransformCache.m_PrefabSubIndex;
					transform2.m_Position = localTransformCache.m_Position;
					transform2.m_Rotation = localTransformCache.m_Rotation;
				}
				else
				{
					objectDefinition.m_ParentMesh = (m_BuildingData.HasComponent(upgrade) ? (-1) : 0);
					transform2 = ObjectUtils.WorldToLocal(inverseParentTransform, m_TransformData[upgrade]);
				}
				if (m_ElevationData.TryGetComponent(upgrade, ref elevation))
				{
					objectDefinition.m_Elevation = elevation.m_Elevation;
				}
				Transform transform3 = ObjectUtils.LocalToWorld(transform, transform2);
				transform3.m_Rotation = math.normalize(transform3.m_Rotation);
				if (relocate && m_BuildingData.HasComponent(upgrade) && m_PrefabRefData.TryGetComponent(upgrade, ref prefabRef) && m_PrefabPlaceableObjectData.TryGetComponent(prefabRef.m_Prefab, ref placeableObjectData))
				{
					float num = TerrainUtils.SampleHeight(ref m_TerrainHeightData, transform3.m_Position);
					if ((placeableObjectData.m_Flags & Game.Objects.PlacementFlags.Hovering) != Game.Objects.PlacementFlags.None)
					{
						float num2 = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, transform3.m_Position);
						num2 += placeableObjectData.m_PlacementOffset.y;
						objectDefinition.m_Elevation = math.max(0f, num2 - num);
						num = math.max(num, num2);
					}
					else if ((placeableObjectData.m_Flags & (Game.Objects.PlacementFlags.Shoreline | Game.Objects.PlacementFlags.Floating)) == 0)
					{
						num += placeableObjectData.m_PlacementOffset.y;
					}
					else
					{
						float num3 = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, transform3.m_Position, out var waterDepth);
						if (waterDepth >= 0.2f)
						{
							num3 += placeableObjectData.m_PlacementOffset.y;
							if ((placeableObjectData.m_Flags & Game.Objects.PlacementFlags.Floating) != Game.Objects.PlacementFlags.None)
							{
								objectDefinition.m_Elevation = math.max(0f, num3 - num);
							}
							num = math.max(num, num3);
						}
					}
					transform3.m_Position.y = num;
				}
				objectDefinition.m_Position = transform3.m_Position;
				objectDefinition.m_Rotation = transform3.m_Rotation;
				objectDefinition.m_LocalPosition = transform2.m_Position;
				objectDefinition.m_LocalRotation = transform2.m_Rotation;
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<ObjectDefinition>(val2, objectDefinition);
				OwnerDefinition ownerDefinition2 = new OwnerDefinition
				{
					m_Prefab = m_PrefabRefData[upgrade].m_Prefab,
					m_Position = transform3.m_Position,
					m_Rotation = transform3.m_Rotation
				};
				UpdateSubNets(transform3, mainTransform, mainInverseTransform, ownerDefinition2.m_Prefab, upgrade, Entity.Null, relocate, topLevel: true, ownerDefinition2, default(NativeList<ClearAreaData>), ref random);
				UpdateSubAreas(transform3, ownerDefinition2.m_Prefab, upgrade, relocate, rebuild, topLevel: true, ownerDefinition2, default(NativeList<ClearAreaData>), ref random, ref selectedSpawnables);
			}
		}

		private void CreateSubNet(Entity netPrefab, Entity lanePrefab, Bezier4x3 curve, int2 nodeIndex, int2 parentMesh, CompositionFlags upgrades, NativeList<float4> nodePositions, Transform parentTransform, OwnerDefinition ownerDefinition, NativeList<ClearAreaData> clearAreas, BuildingUtils.LotInfo lotInfo, bool hasLot, ref Random random)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Unknown result type (might be due to invalid IL or missing references)
			//IL_056d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_063f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_0690: Unknown result type (might be due to invalid IL or missing references)
			//IL_0691: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			NetGeometryData netGeometryData = default(NetGeometryData);
			m_PrefabNetGeometryData.TryGetComponent(netPrefab, ref netGeometryData);
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Prefab = netPrefab,
				m_SubPrefab = lanePrefab,
				m_RandomSeed = ((Random)(ref random)).NextInt()
			};
			bool flag = parentMesh.x >= 0 && parentMesh.y >= 0;
			NetCourse netCourse = default(NetCourse);
			Bezier4x1 val;
			if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.OnWater) != 0)
			{
				val = (((Bezier4x3)(ref curve)).y = default(Bezier4x1));
				Curve curve2 = new Curve
				{
					m_Bezier = ObjectUtils.LocalToWorld(parentTransform.m_Position, parentTransform.m_Rotation, curve)
				};
				netCourse.m_Curve = NetUtils.AdjustPosition(curve2, fixedStart: false, linearMiddle: false, fixedEnd: false, ref m_TerrainHeightData, ref m_WaterSurfaceData).m_Bezier;
			}
			else if (!flag)
			{
				Curve curve3 = new Curve
				{
					m_Bezier = ObjectUtils.LocalToWorld(parentTransform.m_Position, parentTransform.m_Rotation, curve)
				};
				bool flag2 = parentMesh.x >= 0;
				bool flag3 = parentMesh.y >= 0;
				flag = flag2 || flag3;
				if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.FlattenTerrain) != 0)
				{
					if (hasLot)
					{
						netCourse.m_Curve = NetUtils.AdjustPosition(curve3, bool2.op_Implicit(flag2), flag, bool2.op_Implicit(flag3), ref lotInfo).m_Bezier;
						netCourse.m_Curve.a.y += curve.a.y;
						netCourse.m_Curve.b.y += curve.b.y;
						netCourse.m_Curve.c.y += curve.c.y;
						netCourse.m_Curve.d.y += curve.d.y;
					}
					else
					{
						netCourse.m_Curve = curve3.m_Bezier;
					}
				}
				else
				{
					netCourse.m_Curve = NetUtils.AdjustPosition(curve3, flag2, flag, flag3, ref m_TerrainHeightData).m_Bezier;
					netCourse.m_Curve.a.y += curve.a.y;
					netCourse.m_Curve.b.y += curve.b.y;
					netCourse.m_Curve.c.y += curve.c.y;
					netCourse.m_Curve.d.y += curve.d.y;
				}
			}
			else
			{
				netCourse.m_Curve = ObjectUtils.LocalToWorld(parentTransform.m_Position, parentTransform.m_Rotation, curve);
			}
			int num;
			if (flag)
			{
				val = ((Bezier4x3)(ref curve)).y;
				num = ((math.cmin(math.abs(((Bezier4x1)(ref val)).abcd)) < 2f) ? 1 : 0);
			}
			else
			{
				num = 1;
			}
			bool onGround = (byte)num != 0;
			if (ClearAreaHelpers.ShouldClear(clearAreas, netCourse.m_Curve, onGround))
			{
				return;
			}
			Entity val3 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val3, creationDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val3, default(Updated));
			if (ownerDefinition.m_Prefab != Entity.Null)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val3, ownerDefinition);
			}
			netCourse.m_StartPosition.m_Position = netCourse.m_Curve.a;
			netCourse.m_StartPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.StartTangent(netCourse.m_Curve), parentTransform.m_Rotation);
			netCourse.m_StartPosition.m_CourseDelta = 0f;
			netCourse.m_StartPosition.m_Elevation = float2.op_Implicit(curve.a.y);
			netCourse.m_StartPosition.m_ParentMesh = parentMesh.x;
			float4 val4;
			float3 val5;
			if (nodeIndex.x >= 0)
			{
				if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.OnWater) != 0)
				{
					ref float3 position = ref netCourse.m_StartPosition.m_Position;
					Transform transform = parentTransform;
					val4 = nodePositions[nodeIndex.x];
					val5 = ObjectUtils.LocalToWorld(transform, ((float4)(ref val4)).xyz);
					((float3)(ref position)).xz = ((float3)(ref val5)).xz;
					netCourse.m_StartPosition.m_Position.y = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, netCourse.m_StartPosition.m_Position);
				}
				else
				{
					ref CoursePos startPosition = ref netCourse.m_StartPosition;
					Transform transform2 = parentTransform;
					val4 = nodePositions[nodeIndex.x];
					startPosition.m_Position = ObjectUtils.LocalToWorld(transform2, ((float4)(ref val4)).xyz);
				}
			}
			netCourse.m_EndPosition.m_Position = netCourse.m_Curve.d;
			netCourse.m_EndPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.EndTangent(netCourse.m_Curve), parentTransform.m_Rotation);
			netCourse.m_EndPosition.m_CourseDelta = 1f;
			netCourse.m_EndPosition.m_Elevation = float2.op_Implicit(curve.d.y);
			netCourse.m_EndPosition.m_ParentMesh = parentMesh.y;
			if (nodeIndex.y >= 0)
			{
				if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.OnWater) != 0)
				{
					ref float3 position2 = ref netCourse.m_EndPosition.m_Position;
					Transform transform3 = parentTransform;
					val4 = nodePositions[nodeIndex.y];
					val5 = ObjectUtils.LocalToWorld(transform3, ((float4)(ref val4)).xyz);
					((float3)(ref position2)).xz = ((float3)(ref val5)).xz;
					netCourse.m_EndPosition.m_Position.y = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, netCourse.m_EndPosition.m_Position);
				}
				else
				{
					ref CoursePos endPosition = ref netCourse.m_EndPosition;
					Transform transform4 = parentTransform;
					val4 = nodePositions[nodeIndex.y];
					endPosition.m_Position = ObjectUtils.LocalToWorld(transform4, ((float4)(ref val4)).xyz);
				}
			}
			netCourse.m_Length = MathUtils.Length(netCourse.m_Curve);
			netCourse.m_FixedIndex = -1;
			netCourse.m_StartPosition.m_Flags |= CoursePosFlags.IsFirst;
			netCourse.m_EndPosition.m_Flags |= CoursePosFlags.IsLast;
			if (((float3)(ref netCourse.m_StartPosition.m_Position)).Equals(netCourse.m_EndPosition.m_Position))
			{
				netCourse.m_StartPosition.m_Flags |= CoursePosFlags.IsLast;
				netCourse.m_EndPosition.m_Flags |= CoursePosFlags.IsFirst;
			}
			if (ownerDefinition.m_Prefab == Entity.Null)
			{
				netCourse.m_StartPosition.m_Flags |= CoursePosFlags.FreeHeight;
				netCourse.m_EndPosition.m_Flags |= CoursePosFlags.FreeHeight;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val3, netCourse);
			if (upgrades != default(CompositionFlags))
			{
				Upgraded upgraded = new Upgraded
				{
					m_Flags = upgrades
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Upgraded>(val3, upgraded);
			}
			if (m_EditorMode)
			{
				LocalCurveCache localCurveCache = new LocalCurveCache
				{
					m_Curve = curve
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalCurveCache>(val3, localCurveCache);
			}
		}

		private bool GetOwnerLot(Entity lotOwner, out BuildingUtils.LotInfo lotInfo)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			Game.Buildings.Lot lot = default(Game.Buildings.Lot);
			Transform transform = default(Transform);
			PrefabRef prefabRef = default(PrefabRef);
			BuildingData buildingData = default(BuildingData);
			if (m_LotData.TryGetComponent(lotOwner, ref lot) && m_TransformData.TryGetComponent(lotOwner, ref transform) && m_PrefabRefData.TryGetComponent(lotOwner, ref prefabRef) && m_PrefabBuildingData.TryGetComponent(prefabRef.m_Prefab, ref buildingData))
			{
				float2 extents = new float2(buildingData.m_LotSize) * 4f;
				Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
				m_ElevationData.TryGetComponent(lotOwner, ref elevation);
				DynamicBuffer<InstalledUpgrade> upgrades = default(DynamicBuffer<InstalledUpgrade>);
				m_InstalledUpgrades.TryGetBuffer(lotOwner, ref upgrades);
				lotInfo = BuildingUtils.CalculateLotInfo(extents, transform, elevation, lot, prefabRef, upgrades, m_TransformData, m_PrefabRefData, m_PrefabObjectGeometryData, m_PrefabBuildingTerraformData, m_PrefabBuildingExtensionData, defaultNoSmooth: false, out var _);
				return true;
			}
			lotInfo = default(BuildingUtils.LotInfo);
			return false;
		}

		private void UpdateSubNets(Transform transform, Transform mainTransform, Transform mainInverseTransform, Entity prefab, Entity original, Entity lotEntity, bool relocate, bool topLevel, OwnerDefinition ownerDefinition, NativeList<ClearAreaData> clearAreas, ref Random random)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0620: Unknown result type (might be due to invalid IL or missing references)
			//IL_0625: Unknown result type (might be due to invalid IL or missing references)
			//IL_062d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0909: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_053f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_091d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0922: Unknown result type (might be due to invalid IL or missing references)
			//IL_092e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0930: Unknown result type (might be due to invalid IL or missing references)
			//IL_093b: Unknown result type (might be due to invalid IL or missing references)
			//IL_094a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0650: Unknown result type (might be due to invalid IL or missing references)
			//IL_0655: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0551: Unknown result type (might be due to invalid IL or missing references)
			//IL_0509: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_095b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0960: Unknown result type (might be due to invalid IL or missing references)
			//IL_067f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0684: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0579: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0596: Unknown result type (might be due to invalid IL or missing references)
			//IL_059b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05da: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_097c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0980: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a62: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a67: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0adb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b13: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0714: Unknown result type (might be due to invalid IL or missing references)
			//IL_0723: Unknown result type (might be due to invalid IL or missing references)
			//IL_0740: Unknown result type (might be due to invalid IL or missing references)
			//IL_0747: Unknown result type (might be due to invalid IL or missing references)
			//IL_074e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0755: Unknown result type (might be due to invalid IL or missing references)
			//IL_075a: Unknown result type (might be due to invalid IL or missing references)
			//IL_075f: Unknown result type (might be due to invalid IL or missing references)
			//IL_077f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0781: Unknown result type (might be due to invalid IL or missing references)
			//IL_078f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0794: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0822: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0704: Unknown result type (might be due to invalid IL or missing references)
			//IL_0709: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b46: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b50: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b73: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b82: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b87: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0baa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0baf: Unknown result type (might be due to invalid IL or missing references)
			//IL_083d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0844: Unknown result type (might be due to invalid IL or missing references)
			//IL_0849: Unknown result type (might be due to invalid IL or missing references)
			//IL_0850: Unknown result type (might be due to invalid IL or missing references)
			//IL_0855: Unknown result type (might be due to invalid IL or missing references)
			//IL_085a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0863: Unknown result type (might be due to invalid IL or missing references)
			//IL_086a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0871: Unknown result type (might be due to invalid IL or missing references)
			//IL_0878: Unknown result type (might be due to invalid IL or missing references)
			//IL_087d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0882: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0894: Unknown result type (might be due to invalid IL or missing references)
			//IL_089a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c13: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c59: Unknown result type (might be due to invalid IL or missing references)
			bool flag = original == Entity.Null || (relocate && m_EditorMode);
			if (flag && topLevel && m_PrefabSubNets.HasBuffer(prefab))
			{
				DynamicBuffer<Game.Prefabs.SubNet> subNets = m_PrefabSubNets[prefab];
				NativeList<float4> nodePositions = default(NativeList<float4>);
				nodePositions._002Ector(subNets.Length * 2, AllocatorHandle.op_Implicit((Allocator)2));
				BuildingUtils.LotInfo lotInfo;
				bool ownerLot = GetOwnerLot(lotEntity, out lotInfo);
				for (int i = 0; i < subNets.Length; i++)
				{
					Game.Prefabs.SubNet subNet = subNets[i];
					if (subNet.m_NodeIndex.x >= 0)
					{
						while (nodePositions.Length <= subNet.m_NodeIndex.x)
						{
							float4 val = default(float4);
							nodePositions.Add(ref val);
						}
						ref NativeList<float4> reference = ref nodePositions;
						int x = subNet.m_NodeIndex.x;
						reference[x] += new float4(subNet.m_Curve.a, 1f);
					}
					if (subNet.m_NodeIndex.y >= 0)
					{
						while (nodePositions.Length <= subNet.m_NodeIndex.y)
						{
							float4 val = default(float4);
							nodePositions.Add(ref val);
						}
						ref NativeList<float4> reference = ref nodePositions;
						int x = subNet.m_NodeIndex.y;
						reference[x] += new float4(subNet.m_Curve.d, 1f);
					}
				}
				for (int j = 0; j < nodePositions.Length; j++)
				{
					ref NativeList<float4> reference = ref nodePositions;
					int x = j;
					reference[x] /= math.max(1f, nodePositions[j].w);
				}
				for (int k = 0; k < subNets.Length; k++)
				{
					Game.Prefabs.SubNet subNet2 = NetUtils.GetSubNet(subNets, k, m_LefthandTraffic, ref m_PrefabNetGeometryData);
					CreateSubNet(subNet2.m_Prefab, Entity.Null, subNet2.m_Curve, subNet2.m_NodeIndex, subNet2.m_ParentMesh, subNet2.m_Upgrades, nodePositions, transform, ownerDefinition, clearAreas, lotInfo, ownerLot, ref random);
				}
				nodePositions.Dispose();
			}
			if (flag && topLevel && m_EditorMode && m_PrefabSubLanes.HasBuffer(prefab))
			{
				DynamicBuffer<Game.Prefabs.SubLane> val2 = m_PrefabSubLanes[prefab];
				NativeList<float4> nodePositions2 = default(NativeList<float4>);
				nodePositions2._002Ector(val2.Length * 2, AllocatorHandle.op_Implicit((Allocator)2));
				for (int l = 0; l < val2.Length; l++)
				{
					Game.Prefabs.SubLane subLane = val2[l];
					if (subLane.m_NodeIndex.x >= 0)
					{
						while (nodePositions2.Length <= subLane.m_NodeIndex.x)
						{
							float4 val = default(float4);
							nodePositions2.Add(ref val);
						}
						ref NativeList<float4> reference = ref nodePositions2;
						int x = subLane.m_NodeIndex.x;
						reference[x] += new float4(subLane.m_Curve.a, 1f);
					}
					if (subLane.m_NodeIndex.y >= 0)
					{
						while (nodePositions2.Length <= subLane.m_NodeIndex.y)
						{
							float4 val = default(float4);
							nodePositions2.Add(ref val);
						}
						ref NativeList<float4> reference = ref nodePositions2;
						int x = subLane.m_NodeIndex.y;
						reference[x] += new float4(subLane.m_Curve.d, 1f);
					}
				}
				for (int m = 0; m < nodePositions2.Length; m++)
				{
					ref NativeList<float4> reference = ref nodePositions2;
					int x = m;
					reference[x] /= math.max(1f, nodePositions2[m].w);
				}
				for (int n = 0; n < val2.Length; n++)
				{
					Game.Prefabs.SubLane subLane2 = val2[n];
					CreateSubNet(m_LaneEditor, subLane2.m_Prefab, subLane2.m_Curve, subLane2.m_NodeIndex, subLane2.m_ParentMesh, default(CompositionFlags), nodePositions2, transform, ownerDefinition, clearAreas, default(BuildingUtils.LotInfo), hasLot: false, ref random);
				}
				nodePositions2.Dispose();
			}
			if (!m_SubNets.HasBuffer(original))
			{
				return;
			}
			DynamicBuffer<Game.Net.SubNet> val3 = m_SubNets[original];
			NativeHashMap<Entity, int> val4 = default(NativeHashMap<Entity, int>);
			NativeList<float4> nodePositions3 = default(NativeList<float4>);
			BuildingUtils.LotInfo lotInfo2 = default(BuildingUtils.LotInfo);
			bool hasLot = false;
			if (!flag && relocate)
			{
				val4._002Ector(val3.Length, AllocatorHandle.op_Implicit((Allocator)2));
				nodePositions3._002Ector(val3.Length, AllocatorHandle.op_Implicit((Allocator)2));
				hasLot = GetOwnerLot(lotEntity, out lotInfo2);
				Game.Net.Node node = default(Game.Net.Node);
				Edge edge = default(Edge);
				for (int num = 0; num < val3.Length; num++)
				{
					Entity subNet3 = val3[num].m_SubNet;
					if (m_NodeData.TryGetComponent(subNet3, ref node))
					{
						if (val4.TryAdd(subNet3, nodePositions3.Length))
						{
							node.m_Position = ObjectUtils.WorldToLocal(mainInverseTransform, node.m_Position);
							float4 val = new float4(node.m_Position, 1f);
							nodePositions3.Add(ref val);
						}
					}
					else if (m_EdgeData.TryGetComponent(subNet3, ref edge))
					{
						if (val4.TryAdd(edge.m_Start, nodePositions3.Length))
						{
							node.m_Position = ObjectUtils.WorldToLocal(mainInverseTransform, m_NodeData[edge.m_Start].m_Position);
							float4 val = new float4(node.m_Position, 1f);
							nodePositions3.Add(ref val);
						}
						if (val4.TryAdd(edge.m_End, nodePositions3.Length))
						{
							node.m_Position = ObjectUtils.WorldToLocal(mainInverseTransform, m_NodeData[edge.m_End].m_Position);
							float4 val = new float4(node.m_Position, 1f);
							nodePositions3.Add(ref val);
						}
					}
				}
			}
			Game.Net.Node node2 = default(Game.Net.Node);
			Game.Net.Elevation elevation = default(Game.Net.Elevation);
			Upgraded upgraded = default(Upgraded);
			Edge edge2 = default(Edge);
			Game.Net.Elevation elevation2 = default(Game.Net.Elevation);
			int2 nodeIndex = default(int2);
			int2 parentMesh = default(int2);
			Upgraded upgraded2 = default(Upgraded);
			for (int num2 = 0; num2 < val3.Length; num2++)
			{
				Entity subNet4 = val3[num2].m_SubNet;
				if (m_NodeData.TryGetComponent(subNet4, ref node2))
				{
					if (HasEdgeStartOrEnd(subNet4, original))
					{
						continue;
					}
					Entity val5 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
					CreationDefinition creationDefinition = new CreationDefinition
					{
						m_Original = subNet4
					};
					bool flag2 = m_NetElevationData.TryGetComponent(subNet4, ref elevation);
					bool onGround = !flag2 || math.cmin(math.abs(elevation.m_Elevation)) < 2f;
					if (flag || relocate || ClearAreaHelpers.ShouldClear(clearAreas, node2.m_Position, onGround))
					{
						creationDefinition.m_Flags |= CreationFlags.Delete | CreationFlags.Hidden;
					}
					else if (ownerDefinition.m_Prefab != Entity.Null)
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val5, ownerDefinition);
					}
					if (m_EditorContainerData.HasComponent(subNet4))
					{
						creationDefinition.m_SubPrefab = m_EditorContainerData[subNet4].m_Prefab;
					}
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val5, creationDefinition);
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val5, default(Updated));
					NetCourse netCourse = new NetCourse
					{
						m_Curve = new Bezier4x3(node2.m_Position, node2.m_Position, node2.m_Position, node2.m_Position),
						m_Length = 0f,
						m_FixedIndex = -1,
						m_StartPosition = 
						{
							m_Entity = subNet4,
							m_Position = node2.m_Position,
							m_Rotation = node2.m_Rotation,
							m_CourseDelta = 0f,
							m_ParentMesh = -1
						},
						m_EndPosition = 
						{
							m_Entity = subNet4,
							m_Position = node2.m_Position,
							m_Rotation = node2.m_Rotation,
							m_CourseDelta = 1f,
							m_ParentMesh = -1
						}
					};
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val5, netCourse);
					if (!flag && relocate)
					{
						Entity netPrefab = m_PrefabRefData[subNet4];
						node2.m_Position = ObjectUtils.WorldToLocal(mainInverseTransform, node2.m_Position);
						netCourse.m_Curve = new Bezier4x3(node2.m_Position, node2.m_Position, node2.m_Position, node2.m_Position);
						if (!flag2)
						{
							((Bezier4x3)(ref netCourse.m_Curve)).y = default(Bezier4x1);
						}
						int num3 = val4[subNet4];
						int num4 = ((!flag2) ? (-1) : 0);
						m_UpgradedData.TryGetComponent(subNet4, ref upgraded);
						CreateSubNet(netPrefab, creationDefinition.m_SubPrefab, netCourse.m_Curve, int2.op_Implicit(num3), int2.op_Implicit(num4), upgraded.m_Flags, nodePositions3, mainTransform, ownerDefinition, clearAreas, lotInfo2, hasLot, ref random);
					}
				}
				else
				{
					if (!m_EdgeData.TryGetComponent(subNet4, ref edge2))
					{
						continue;
					}
					Entity val6 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
					CreationDefinition creationDefinition2 = new CreationDefinition
					{
						m_Original = subNet4
					};
					Curve curve = m_CurveData[subNet4];
					bool flag3 = m_NetElevationData.TryGetComponent(subNet4, ref elevation2);
					bool onGround2 = !flag3 || math.cmin(math.abs(elevation2.m_Elevation)) < 2f;
					if (flag || relocate || ClearAreaHelpers.ShouldClear(clearAreas, curve.m_Bezier, onGround2))
					{
						creationDefinition2.m_Flags |= CreationFlags.Delete | CreationFlags.Hidden;
					}
					else if (ownerDefinition.m_Prefab != Entity.Null)
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val6, ownerDefinition);
					}
					if (m_EditorContainerData.HasComponent(subNet4))
					{
						creationDefinition2.m_SubPrefab = m_EditorContainerData[subNet4].m_Prefab;
					}
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val6, creationDefinition2);
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val6, default(Updated));
					NetCourse netCourse2 = default(NetCourse);
					netCourse2.m_Curve = curve.m_Bezier;
					netCourse2.m_Length = MathUtils.Length(netCourse2.m_Curve);
					netCourse2.m_FixedIndex = -1;
					netCourse2.m_StartPosition.m_Entity = edge2.m_Start;
					netCourse2.m_StartPosition.m_Position = netCourse2.m_Curve.a;
					netCourse2.m_StartPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.StartTangent(netCourse2.m_Curve));
					netCourse2.m_StartPosition.m_CourseDelta = 0f;
					netCourse2.m_StartPosition.m_ParentMesh = -1;
					netCourse2.m_EndPosition.m_Entity = edge2.m_End;
					netCourse2.m_EndPosition.m_Position = netCourse2.m_Curve.d;
					netCourse2.m_EndPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.EndTangent(netCourse2.m_Curve));
					netCourse2.m_EndPosition.m_CourseDelta = 1f;
					netCourse2.m_EndPosition.m_ParentMesh = -1;
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val6, netCourse2);
					if (!flag && relocate)
					{
						Entity netPrefab2 = m_PrefabRefData[subNet4];
						netCourse2.m_Curve.a = ObjectUtils.WorldToLocal(mainInverseTransform, netCourse2.m_Curve.a);
						netCourse2.m_Curve.b = ObjectUtils.WorldToLocal(mainInverseTransform, netCourse2.m_Curve.b);
						netCourse2.m_Curve.c = ObjectUtils.WorldToLocal(mainInverseTransform, netCourse2.m_Curve.c);
						netCourse2.m_Curve.d = ObjectUtils.WorldToLocal(mainInverseTransform, netCourse2.m_Curve.d);
						if (!flag3)
						{
							((Bezier4x3)(ref netCourse2.m_Curve)).y = default(Bezier4x1);
						}
						((int2)(ref nodeIndex))._002Ector(val4[edge2.m_Start], val4[edge2.m_End]);
						((int2)(ref parentMesh))._002Ector((!m_NetElevationData.HasComponent(edge2.m_Start)) ? (-1) : 0, (!m_NetElevationData.HasComponent(edge2.m_End)) ? (-1) : 0);
						m_UpgradedData.TryGetComponent(subNet4, ref upgraded2);
						CreateSubNet(netPrefab2, creationDefinition2.m_SubPrefab, netCourse2.m_Curve, nodeIndex, parentMesh, upgraded2.m_Flags, nodePositions3, mainTransform, ownerDefinition, clearAreas, lotInfo2, hasLot, ref random);
					}
				}
			}
			if (val4.IsCreated)
			{
				val4.Dispose();
			}
			if (nodePositions3.IsCreated)
			{
				nodePositions3.Dispose();
			}
		}

		private void UpdateSubAreas(Transform transform, Entity prefab, Entity original, bool relocate, bool rebuild, bool topLevel, OwnerDefinition ownerDefinition, NativeList<ClearAreaData> clearAreas, ref Random random, ref NativeParallelHashMap<Entity, int> selectedSpawnables)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			bool flag = original == Entity.Null || relocate || rebuild;
			if (flag && topLevel && m_PrefabSubAreas.HasBuffer(prefab))
			{
				DynamicBuffer<Game.Prefabs.SubArea> val = m_PrefabSubAreas[prefab];
				DynamicBuffer<SubAreaNode> val2 = m_PrefabSubAreaNodes[prefab];
				for (int i = 0; i < val.Length; i++)
				{
					Game.Prefabs.SubArea subArea = val[i];
					int seed;
					if (!m_EditorMode && m_PrefabPlaceholderElements.HasBuffer(subArea.m_Prefab))
					{
						DynamicBuffer<PlaceholderObjectElement> placeholderElements = m_PrefabPlaceholderElements[subArea.m_Prefab];
						if (!selectedSpawnables.IsCreated)
						{
							selectedSpawnables = new NativeParallelHashMap<Entity, int>(10, AllocatorHandle.op_Implicit((Allocator)2));
						}
						if (!AreaUtils.SelectAreaPrefab(placeholderElements, m_PrefabSpawnableObjectData, selectedSpawnables, ref random, out subArea.m_Prefab, out seed))
						{
							continue;
						}
					}
					else
					{
						seed = ((Random)(ref random)).NextInt();
					}
					AreaGeometryData areaGeometryData = m_PrefabAreaGeometryData[subArea.m_Prefab];
					if (areaGeometryData.m_Type == AreaType.Space)
					{
						if (ClearAreaHelpers.ShouldClear(clearAreas, val2, subArea.m_NodeRange, transform))
						{
							continue;
						}
					}
					else if (areaGeometryData.m_Type == AreaType.Lot && rebuild)
					{
						continue;
					}
					Entity val3 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
					CreationDefinition creationDefinition = new CreationDefinition
					{
						m_Prefab = subArea.m_Prefab,
						m_RandomSeed = seed
					};
					if (areaGeometryData.m_Type != AreaType.Lot)
					{
						creationDefinition.m_Flags |= CreationFlags.Hidden;
					}
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val3, creationDefinition);
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val3, default(Updated));
					if (ownerDefinition.m_Prefab != Entity.Null)
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val3, ownerDefinition);
					}
					DynamicBuffer<Game.Areas.Node> val4 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<Game.Areas.Node>(val3);
					val4.ResizeUninitialized(subArea.m_NodeRange.y - subArea.m_NodeRange.x + 1);
					DynamicBuffer<LocalNodeCache> val5 = default(DynamicBuffer<LocalNodeCache>);
					if (m_EditorMode)
					{
						val5 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<LocalNodeCache>(val3);
						val5.ResizeUninitialized(val4.Length);
					}
					int num = GetFirstNodeIndex(val2, subArea.m_NodeRange);
					int num2 = 0;
					for (int j = subArea.m_NodeRange.x; j <= subArea.m_NodeRange.y; j++)
					{
						float3 position = val2[num].m_Position;
						float3 position2 = ObjectUtils.LocalToWorld(transform, position);
						int parentMesh = val2[num].m_ParentMesh;
						float elevation = math.select(float.MinValue, position.y, parentMesh >= 0);
						val4[num2] = new Game.Areas.Node(position2, elevation);
						if (m_EditorMode)
						{
							val5[num2] = new LocalNodeCache
							{
								m_Position = position,
								m_ParentMesh = parentMesh
							};
						}
						num2++;
						if (++num == subArea.m_NodeRange.y)
						{
							num = subArea.m_NodeRange.x;
						}
					}
				}
			}
			if (!m_SubAreas.HasBuffer(original))
			{
				return;
			}
			DynamicBuffer<Game.Areas.SubArea> val6 = m_SubAreas[original];
			for (int k = 0; k < val6.Length; k++)
			{
				Entity area = val6[k].m_Area;
				DynamicBuffer<Game.Areas.Node> nodes = m_AreaNodes[area];
				bool flag2 = flag;
				if (!flag2 && m_AreaSpaceData.HasComponent(area))
				{
					DynamicBuffer<Triangle> triangles = m_AreaTriangles[area];
					flag2 = ClearAreaHelpers.ShouldClear(clearAreas, nodes, triangles, transform);
				}
				if (m_AreaLotData.HasComponent(area))
				{
					if (!flag2)
					{
						continue;
					}
					flag2 = !rebuild;
				}
				Entity val7 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
				CreationDefinition creationDefinition2 = new CreationDefinition
				{
					m_Original = area
				};
				if (flag2)
				{
					creationDefinition2.m_Flags |= CreationFlags.Delete | CreationFlags.Hidden;
				}
				else if (ownerDefinition.m_Prefab != Entity.Null)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val7, ownerDefinition);
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val7, creationDefinition2);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val7, default(Updated));
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<Game.Areas.Node>(val7).CopyFrom(nodes.AsNativeArray());
				if (m_CachedNodes.HasBuffer(area))
				{
					DynamicBuffer<LocalNodeCache> val8 = m_CachedNodes[area];
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<LocalNodeCache>(val7).CopyFrom(val8.AsNativeArray());
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
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attachment> __Game_Objects_Attachment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> __Game_Tools_LocalTransformCache_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> __Game_Objects_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Lot> __Game_Buildings_Lot_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Orphan> __Game_Net_Orphan_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Upgraded> __Game_Net_Upgraded_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Clear> __Game_Areas_Clear_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Space> __Game_Areas_Space_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> __Game_Areas_Lot_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EditorContainer> __Game_Tools_EditorContainer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetObjectData> __Game_Prefabs_NetObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AssetStampData> __Game_Prefabs_AssetStampData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> __Game_Prefabs_BuildingExtensionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> __Game_Prefabs_SpawnableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> __Game_Prefabs_PlaceableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BrushData> __Game_Prefabs_BrushData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingTerraformData> __Game_Prefabs_BuildingTerraformData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CreatureSpawnData> __Game_Prefabs_CreatureSpawnData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceholderBuildingData> __Game_Prefabs_PlaceholderBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LocalNodeCache> __Game_Tools_LocalNodeCache_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubObject> __Game_Prefabs_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubNet> __Game_Prefabs_SubNet_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubLane> __Game_Prefabs_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubArea> __Game_Prefabs_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubAreaNode> __Game_Prefabs_SubAreaNode_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> __Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ObjectRequirementElement> __Game_Prefabs_ObjectRequirementElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ServiceUpgradeBuilding> __Game_Prefabs_ServiceUpgradeBuilding_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<BrushCell> __Game_Prefabs_BrushCell_RO_BufferLookup;

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
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Objects_Attachment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attachment>(true);
			__Game_Tools_LocalTransformCache_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalTransformCache>(true);
			__Game_Objects_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.Elevation>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_Lot_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Lot>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Node>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Elevation>(true);
			__Game_Net_Orphan_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Orphan>(true);
			__Game_Net_Upgraded_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Upgraded>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Areas_Clear_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Clear>(true);
			__Game_Areas_Space_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Space>(true);
			__Game_Areas_Lot_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Areas.Lot>(true);
			__Game_Tools_EditorContainer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EditorContainer>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetObjectData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_AssetStampData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AssetStampData>(true);
			__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingExtensionData>(true);
			__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableObjectData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableObjectData>(true);
			__Game_Prefabs_AreaGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaGeometryData>(true);
			__Game_Prefabs_BrushData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BrushData>(true);
			__Game_Prefabs_BuildingTerraformData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingTerraformData>(true);
			__Game_Prefabs_CreatureSpawnData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CreatureSpawnData>(true);
			__Game_Prefabs_PlaceholderBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceholderBuildingData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Tools_LocalNodeCache_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LocalNodeCache>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubNet>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Prefabs_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubObject>(true);
			__Game_Prefabs_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubNet>(true);
			__Game_Prefabs_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubLane>(true);
			__Game_Prefabs_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubArea>(true);
			__Game_Prefabs_SubAreaNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubAreaNode>(true);
			__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PlaceholderObjectElement>(true);
			__Game_Prefabs_ObjectRequirementElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ObjectRequirementElement>(true);
			__Game_Prefabs_ServiceUpgradeBuilding_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceUpgradeBuilding>(true);
			__Game_Prefabs_BrushCell_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<BrushCell>(true);
		}
	}

	protected ToolOutputBarrier m_ToolOutputBarrier;

	protected Game.Objects.SearchSystem m_ObjectSearchSystem;

	protected WaterSystem m_WaterSystem;

	protected TerrainSystem m_TerrainSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
	}

	protected JobHandle CreateDefinitions(Entity objectPrefab, Entity transformPrefab, Entity brushPrefab, Entity owner, Entity original, Entity laneEditor, Entity theme, NativeList<ControlPoint> controlPoints, NativeReference<AttachmentData> attachmentPrefab, bool editorMode, bool lefthandTraffic, bool removing, bool stamping, float brushSize, float brushAngle, float brushStrength, float distance, float deltaTime, RandomSeed randomSeed, Snap snap, AgeMask ageMask, JobHandle inputDeps)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Unknown result type (might be due to invalid IL or missing references)
		//IL_067f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_06db: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_0711: Unknown result type (might be due to invalid IL or missing references)
		//IL_0717: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle deps;
		JobHandle val = IJobExtensions.Schedule<CreateDefinitionsJob>(new CreateDefinitionsJob
		{
			m_EditorMode = editorMode,
			m_LefthandTraffic = lefthandTraffic,
			m_Removing = removing,
			m_Stamping = stamping,
			m_BrushSize = brushSize,
			m_BrushAngle = brushAngle,
			m_BrushStrength = brushStrength,
			m_Distance = distance,
			m_DeltaTime = deltaTime,
			m_ObjectPrefab = objectPrefab,
			m_TransformPrefab = transformPrefab,
			m_BrushPrefab = brushPrefab,
			m_Owner = owner,
			m_Original = original,
			m_LaneEditor = laneEditor,
			m_Theme = theme,
			m_RandomSeed = randomSeed,
			m_Snap = snap,
			m_AgeMask = ageMask,
			m_ControlPoints = controlPoints,
			m_AttachmentPrefab = attachmentPrefab,
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachedData = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachmentData = InternalCompilerInterface.GetComponentLookup<Attachment>(ref __TypeHandle.__Game_Objects_Attachment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LocalTransformCacheData = InternalCompilerInterface.GetComponentLookup<LocalTransformCache>(ref __TypeHandle.__Game_Tools_LocalTransformCache_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElevationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LotData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Lot>(ref __TypeHandle.__Game_Buildings_Lot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetElevationData = InternalCompilerInterface.GetComponentLookup<Game.Net.Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OrphanData = InternalCompilerInterface.GetComponentLookup<Orphan>(ref __TypeHandle.__Game_Net_Orphan_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpgradedData = InternalCompilerInterface.GetComponentLookup<Upgraded>(ref __TypeHandle.__Game_Net_Upgraded_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaClearData = InternalCompilerInterface.GetComponentLookup<Clear>(ref __TypeHandle.__Game_Areas_Clear_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaSpaceData = InternalCompilerInterface.GetComponentLookup<Space>(ref __TypeHandle.__Game_Areas_Space_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaLotData = InternalCompilerInterface.GetComponentLookup<Game.Areas.Lot>(ref __TypeHandle.__Game_Areas_Lot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EditorContainerData = InternalCompilerInterface.GetComponentLookup<EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetObjectData = InternalCompilerInterface.GetComponentLookup<NetObjectData>(ref __TypeHandle.__Game_Prefabs_NetObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAssetStampData = InternalCompilerInterface.GetComponentLookup<AssetStampData>(ref __TypeHandle.__Game_Prefabs_AssetStampData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingExtensionData = InternalCompilerInterface.GetComponentLookup<BuildingExtensionData>(ref __TypeHandle.__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnableObjectData = InternalCompilerInterface.GetComponentLookup<SpawnableObjectData>(ref __TypeHandle.__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPlaceableObjectData = InternalCompilerInterface.GetComponentLookup<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAreaGeometryData = InternalCompilerInterface.GetComponentLookup<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBrushData = InternalCompilerInterface.GetComponentLookup<BrushData>(ref __TypeHandle.__Game_Prefabs_BrushData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingTerraformData = InternalCompilerInterface.GetComponentLookup<BuildingTerraformData>(ref __TypeHandle.__Game_Prefabs_BuildingTerraformData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCreatureSpawnData = InternalCompilerInterface.GetComponentLookup<CreatureSpawnData>(ref __TypeHandle.__Game_Prefabs_CreatureSpawnData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceholderBuildingData = InternalCompilerInterface.GetComponentLookup<PlaceholderBuildingData>(ref __TypeHandle.__Game_Prefabs_PlaceholderBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CachedNodes = InternalCompilerInterface.GetBufferLookup<LocalNodeCache>(ref __TypeHandle.__Game_Tools_LocalNodeCache_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubNets = InternalCompilerInterface.GetBufferLookup<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaTriangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubObjects = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubObject>(ref __TypeHandle.__Game_Prefabs_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubNets = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubNet>(ref __TypeHandle.__Game_Prefabs_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubLanes = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubLane>(ref __TypeHandle.__Game_Prefabs_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubAreas = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubArea>(ref __TypeHandle.__Game_Prefabs_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubAreaNodes = InternalCompilerInterface.GetBufferLookup<SubAreaNode>(ref __TypeHandle.__Game_Prefabs_SubAreaNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPlaceholderElements = InternalCompilerInterface.GetBufferLookup<PlaceholderObjectElement>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRequirementElements = InternalCompilerInterface.GetBufferLookup<ObjectRequirementElement>(ref __TypeHandle.__Game_Prefabs_ObjectRequirementElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabServiceUpgradeBuilding = InternalCompilerInterface.GetBufferLookup<ServiceUpgradeBuilding>(ref __TypeHandle.__Game_Prefabs_ServiceUpgradeBuilding_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBrushCells = InternalCompilerInterface.GetBufferLookup<BrushCell>(ref __TypeHandle.__Game_Prefabs_BrushCell_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies),
			m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_CommandBuffer = m_ToolOutputBarrier.CreateCommandBuffer()
		}, JobHandle.CombineDependencies(inputDeps, dependencies, deps));
		m_ObjectSearchSystem.AddStaticSearchTreeReader(val);
		m_WaterSystem.AddSurfaceReader(val);
		m_TerrainSystem.AddCPUHeightReader(val);
		((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val);
		return val;
	}

	public static int GetFirstNodeIndex(DynamicBuffer<SubAreaNode> nodes, int2 range)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		int result = 0;
		float num = float.MaxValue;
		float num4 = default(float);
		for (int i = range.x; i < range.y; i++)
		{
			int num2 = math.select(i + 1, range.x, i + 1 == range.y);
			SubAreaNode subAreaNode = nodes[i];
			float2 xz = ((float3)(ref subAreaNode.m_Position)).xz;
			subAreaNode = nodes[num2];
			float num3 = MathUtils.Distance(new Segment(xz, ((float3)(ref subAreaNode.m_Position)).xz), default(float2), ref num4);
			if (num3 < num)
			{
				result = i;
				num = num3;
			}
		}
		return result;
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
	protected ObjectToolBaseSystem()
	{
	}
}
