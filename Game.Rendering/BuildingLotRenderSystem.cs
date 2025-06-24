using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Common;
using Game.Net;
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
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class BuildingLotRenderSystem : GameSystemBase
{
	[BurstCompile]
	private struct BuildingLotRenderJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[ReadOnly]
		public ComponentTypeHandle<Extension> m_ExtensionType;

		[ReadOnly]
		public ComponentTypeHandle<AssetStamp> m_AssetStampType;

		[ReadOnly]
		public ComponentTypeHandle<Tree> m_TreeType;

		[ReadOnly]
		public ComponentTypeHandle<Composition> m_CompositionType;

		[ReadOnly]
		public ComponentTypeHandle<Edge> m_EdgeType;

		[ReadOnly]
		public ComponentTypeHandle<EdgeGeometry> m_EdgeGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<StartNodeGeometry> m_StartGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<EndNodeGeometry> m_EndGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Warning> m_WarningType;

		[ReadOnly]
		public ComponentTypeHandle<Error> m_ErrorType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> m_NetElevationData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> m_PrefabBuildingExtensionData;

		[ReadOnly]
		public ComponentLookup<AssetStampData> m_PrefabAssetStampData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public BufferLookup<ServiceUpgradeBuilding> m_PrefabServiceUpgradeBuildings;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public bool m_ZonesVisible;

		[ReadOnly]
		public RenderingSettingsData m_RenderingSettingsData;

		public OverlayRenderSystem.Buffer m_OverlayBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Temp> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			bool checkTempFlags;
			bool flag;
			Color lotColor;
			if (((ArchetypeChunk)(ref chunk)).Has<Error>(ref m_ErrorType))
			{
				checkTempFlags = false;
				flag = true;
				lotColor = m_RenderingSettingsData.m_ErrorColor;
				lotColor.a = 0.5f;
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<Warning>(ref m_WarningType))
			{
				checkTempFlags = false;
				flag = true;
				lotColor = m_RenderingSettingsData.m_WarningColor;
				lotColor.a = 0.5f;
			}
			else if (nativeArray.Length != 0)
			{
				checkTempFlags = true;
				flag = m_EditorMode || !((ArchetypeChunk)(ref chunk)).Has<Tree>(ref m_TreeType) || !((ArchetypeChunk)(ref chunk)).Has<Owner>(ref m_OwnerType);
				lotColor = m_RenderingSettingsData.m_HoveredColor;
				lotColor.a = 0.25f;
			}
			else
			{
				checkTempFlags = false;
				flag = false;
				lotColor = default(Color);
			}
			if (!flag)
			{
				return;
			}
			NativeArray<Composition> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Composition>(ref m_CompositionType);
			if (nativeArray2.Length != 0)
			{
				DrawNets(in chunk, nativeArray2, nativeArray, lotColor, checkTempFlags);
				return;
			}
			NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			if (nativeArray3.Length != 0)
			{
				DrawObjects(in chunk, nativeArray3, nativeArray, lotColor, checkTempFlags);
			}
		}

		private void DrawNets(in ArchetypeChunk chunk, NativeArray<Composition> compositions, NativeArray<Temp> temps, Color lotColor, bool checkTempFlags)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Edge> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Edge>(ref m_EdgeType);
			NativeArray<EdgeGeometry> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EdgeGeometry>(ref m_EdgeGeometryType);
			NativeArray<StartNodeGeometry> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<StartNodeGeometry>(ref m_StartGeometryType);
			NativeArray<EndNodeGeometry> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EndNodeGeometry>(ref m_EndGeometryType);
			Game.Net.Elevation elevation = default(Game.Net.Elevation);
			Game.Net.Elevation elevation2 = default(Game.Net.Elevation);
			for (int i = 0; i < compositions.Length; i++)
			{
				Composition composition = compositions[i];
				NetCompositionData netCompositionData = m_PrefabCompositionData[composition.m_Edge];
				if ((netCompositionData.m_State & CompositionState.Airspace) == 0 || (netCompositionData.m_Flags.m_General & CompositionFlags.General.Elevated) == 0)
				{
					continue;
				}
				Edge edge = nativeArray[i];
				EdgeGeometry edgeGeometry = nativeArray2[i];
				Color color = lotColor;
				if (checkTempFlags)
				{
					Temp temp = temps[i];
					if ((temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Replace | TempFlags.Parent)) == 0)
					{
						continue;
					}
					if ((temp.m_Flags & TempFlags.Parent) != 0)
					{
						color = m_RenderingSettingsData.m_OwnerColor;
						color.a = 0.25f;
					}
				}
				DrawSegment(color, edgeGeometry.m_Start);
				DrawSegment(color, edgeGeometry.m_End);
				if (m_NetElevationData.TryGetComponent(edge.m_Start, ref elevation) && math.all(elevation.m_Elevation > 0f))
				{
					EdgeNodeGeometry geometry = nativeArray3[i].m_Geometry;
					geometry.m_Left.m_Right = geometry.m_Right.m_Right;
					DrawSegment(color, geometry.m_Left);
				}
				if (m_NetElevationData.TryGetComponent(edge.m_End, ref elevation2) && math.all(elevation2.m_Elevation > 0f))
				{
					EdgeNodeGeometry geometry2 = nativeArray4[i].m_Geometry;
					geometry2.m_Left.m_Right = geometry2.m_Right.m_Right;
					DrawSegment(color, geometry2.m_Left);
				}
			}
		}

		private void DrawSegment(Color color, Segment segment)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			Color color2 = default(Color);
			((Color)(ref color2))._002Ector(color.r, color.g, color.b, math.select(color.a, 1f, !m_EditorMode && m_ZonesVisible));
			float num = MathUtils.Length(((Bezier4x3)(ref segment.m_Left)).xz) / 32f;
			float num2 = MathUtils.Length(((Bezier4x3)(ref segment.m_Right)).xz) / 32f;
			float num3 = num / math.max(1f, math.round(num)) * 16f;
			float num4 = num2 / math.max(1f, math.round(num2)) * 16f;
			if (num > 0.5f)
			{
				m_OverlayBuffer.DrawDashedCurve(color2, segment.m_Left, 4f, num3, num3);
			}
			if (num2 > 0.5f)
			{
				m_OverlayBuffer.DrawDashedCurve(color2, segment.m_Right, 4f, num4, num4);
			}
		}

		private void DrawObjects(in ArchetypeChunk chunk, NativeArray<Transform> transforms, NativeArray<Temp> temps, Color lotColor, bool checkTempFlags)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Building>(ref m_BuildingType);
			bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<Extension>(ref m_ExtensionType);
			bool flag3 = ((ArchetypeChunk)(ref chunk)).Has<AssetStamp>(ref m_AssetStampType);
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Transform transform = transforms[i];
				Entity prefab = nativeArray[i].m_Prefab;
				Color color = lotColor;
				if (checkTempFlags)
				{
					Temp temp = temps[i];
					if ((temp.m_Flags & TempFlags.Hidden) != 0 || (temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Replace | TempFlags.Parent)) == 0)
					{
						continue;
					}
					if ((temp.m_Flags & TempFlags.Parent) != 0)
					{
						color = m_RenderingSettingsData.m_OwnerColor;
						color.a = 0.25f;
					}
				}
				if (flag2)
				{
					BuildingExtensionData buildingExtensionData = m_PrefabBuildingExtensionData[prefab];
					bool flag4 = false;
					if (m_EditorMode && m_PrefabServiceUpgradeBuildings.HasBuffer(prefab))
					{
						DynamicBuffer<ServiceUpgradeBuilding> val = m_PrefabServiceUpgradeBuildings[prefab];
						for (int j = 0; j < val.Length; j++)
						{
							Entity building = val[j].m_Building;
							if (m_PrefabBuildingData.HasComponent(building))
							{
								BuildingData buildingData = m_PrefabBuildingData[building];
								ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[building];
								float3 position = transform.m_Position - math.mul(transform.m_Rotation, buildingExtensionData.m_Position);
								DrawLot(color, buildingData.m_LotSize, objectGeometryData, position, transform.m_Rotation);
								flag4 = true;
							}
						}
					}
					if (!flag4 && (m_EditorMode || buildingExtensionData.m_External))
					{
						ObjectGeometryData objectGeometryData2 = m_PrefabObjectGeometryData[prefab];
						DrawLot(color, buildingExtensionData.m_LotSize, objectGeometryData2, transform.m_Position, transform.m_Rotation);
					}
				}
				else if (flag3)
				{
					if (m_EditorMode)
					{
						AssetStampData assetStampData = m_PrefabAssetStampData[prefab];
						ObjectGeometryData objectGeometryData3 = m_PrefabObjectGeometryData[prefab];
						DrawLot(color, assetStampData.m_Size, objectGeometryData3, transform.m_Position, transform.m_Rotation);
					}
				}
				else if (flag)
				{
					BuildingData buildingData2 = m_PrefabBuildingData[prefab];
					ObjectGeometryData objectGeometryData4 = m_PrefabObjectGeometryData[prefab];
					if ((objectGeometryData4.m_Flags & Game.Objects.GeometryFlags.ExclusiveGround) != Game.Objects.GeometryFlags.None)
					{
						DrawLot(color, buildingData2.m_LotSize, objectGeometryData4, transform.m_Position, transform.m_Rotation);
					}
				}
				else
				{
					ObjectGeometryData objectGeometryData5 = m_PrefabObjectGeometryData[prefab];
					DrawCollision(color, objectGeometryData5, transform.m_Position, transform.m_Rotation, 2f);
				}
			}
		}

		private void DrawLot(Color color, int2 lotSize, ObjectGeometryData objectGeometryData, float3 position, quaternion rotation)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			Color val = default(Color);
			Color fillColor = default(Color);
			OverlayRenderSystem.StyleFlags styleFlags;
			float outlineWidth;
			float num;
			if (m_EditorMode)
			{
				val = color;
				((Color)(ref fillColor))._002Ector(color.r, color.g, color.b, 0f);
				styleFlags = OverlayRenderSystem.StyleFlags.Grid | OverlayRenderSystem.StyleFlags.Projected;
				outlineWidth = 0.1f;
				num = 0.1f;
			}
			else
			{
				((Color)(ref val))._002Ector(color.r, color.g, color.b, math.select(color.a, 1f, m_ZonesVisible));
				((Color)(ref fillColor))._002Ector(color.r, color.g, color.b, val.a * 0.2f);
				styleFlags = OverlayRenderSystem.StyleFlags.Projected;
				outlineWidth = math.select(0.2f, 0.4f, m_ZonesVisible);
				num = 0.5f;
			}
			bool flag;
			float2 val2;
			if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Standing) != Game.Objects.GeometryFlags.None)
			{
				flag = (objectGeometryData.m_Flags & Game.Objects.GeometryFlags.CircularLeg) != 0;
				val2 = math.min(float2.op_Implicit(lotSize) * 8f, ((float3)(ref objectGeometryData.m_LegSize)).xz + objectGeometryData.m_LegOffset * 2f);
				DrawCollision(val, objectGeometryData, position, rotation, 8f);
			}
			else
			{
				flag = (objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) != 0;
				val2 = float2.op_Implicit(lotSize) * 8f;
			}
			if (flag)
			{
				float3 val3 = math.forward(rotation);
				m_OverlayBuffer.DrawCircle(val, fillColor, outlineWidth, styleFlags, ((float3)(ref val3)).xz, position, math.cmax(val2));
			}
			else
			{
				float3 val4 = math.forward(rotation) * (val2.y * 0.5f - num);
				Segment line = default(Segment);
				((Segment)(ref line))._002Ector(position - val4, position + val4);
				m_OverlayBuffer.DrawLine(val, fillColor, outlineWidth, styleFlags, line, val2.x, float2.op_Implicit(num / val2.x * 2f));
			}
			if (m_EditorMode)
			{
				float3 val5 = math.forward(rotation);
				float3 val6 = position + val5 * (val2.y * 0.5f);
				Segment line2 = default(Segment);
				((Segment)(ref line2))._002Ector(val6, val6 + val5 * 4f);
				m_OverlayBuffer.DrawLine(val, val, 0f, OverlayRenderSystem.StyleFlags.Projected, line2, 0.5f, new float2(0f, 1f));
			}
		}

		private void DrawCollision(Color color, ObjectGeometryData objectGeometryData, float3 position, quaternion rotation, float size)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			float3 val = position;
			val.y += objectGeometryData.m_LegSize.y;
			if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
			{
				float3 val2 = math.rotate(rotation, new float3(objectGeometryData.m_Size.x * 0.5f, 0f, 0f));
				float3 val3 = math.rotate(rotation, new float3(objectGeometryData.m_Size.x * 0.2761424f, 0f, 0f));
				float3 val4 = math.rotate(rotation, new float3(0f, 0f, objectGeometryData.m_Size.z * 0.5f));
				float3 val5 = math.rotate(rotation, new float3(0f, 0f, objectGeometryData.m_Size.z * 0.2761424f));
				Bezier4x3 curve = default(Bezier4x3);
				((Bezier4x3)(ref curve))._002Ector(val - val2, val - val2 - val5, val - val3 - val4, val - val4);
				Bezier4x3 curve2 = default(Bezier4x3);
				((Bezier4x3)(ref curve2))._002Ector(val - val4, val + val3 - val4, val + val2 - val5, val + val2);
				Bezier4x3 curve3 = default(Bezier4x3);
				((Bezier4x3)(ref curve3))._002Ector(val + val2, val + val2 + val5, val + val3 + val4, val + val4);
				Bezier4x3 curve4 = default(Bezier4x3);
				((Bezier4x3)(ref curve4))._002Ector(val + val4, val - val3 + val4, val - val2 + val5, val - val2);
				float num = MathUtils.Length(((Bezier4x3)(ref curve)).xz) / size;
				float num2 = num / math.max(1f, math.round(num)) * (size * 0.5f);
				m_OverlayBuffer.DrawDashedCurve(color, curve, size * 0.125f, num2, num2);
				m_OverlayBuffer.DrawDashedCurve(color, curve2, size * 0.125f, num2, num2);
				m_OverlayBuffer.DrawDashedCurve(color, curve3, size * 0.125f, num2, num2);
				m_OverlayBuffer.DrawDashedCurve(color, curve4, size * 0.125f, num2, num2);
			}
			else
			{
				float3 val6 = math.rotate(rotation, new float3(objectGeometryData.m_Bounds.min.x, 0f, 0f));
				float3 val7 = math.rotate(rotation, new float3(objectGeometryData.m_Bounds.max.x, 0f, 0f));
				float3 val8 = math.rotate(rotation, new float3(0f, 0f, objectGeometryData.m_Bounds.min.z));
				float3 val9 = math.rotate(rotation, new float3(0f, 0f, objectGeometryData.m_Bounds.max.z));
				Segment line = default(Segment);
				((Segment)(ref line))._002Ector(val + val6 + val8, val + val7 + val8);
				Segment line2 = default(Segment);
				((Segment)(ref line2))._002Ector(val + val7 + val8, val + val7 + val9);
				Segment line3 = default(Segment);
				((Segment)(ref line3))._002Ector(val + val7 + val9, val + val6 + val9);
				Segment line4 = default(Segment);
				((Segment)(ref line4))._002Ector(val + val6 + val9, val + val6 + val8);
				float num3 = MathUtils.Length(((Segment)(ref line)).xz) / size;
				float num4 = MathUtils.Length(((Segment)(ref line2)).xz) / size;
				float num5 = num3 / math.max(1f, math.round(num3)) * (size * 0.5f);
				float num6 = num4 / math.max(1f, math.round(num4)) * (size * 0.5f);
				m_OverlayBuffer.DrawDashedLine(color, line, size * 0.125f, num5, num5);
				m_OverlayBuffer.DrawDashedLine(color, line2, size * 0.125f, num6, num6);
				m_OverlayBuffer.DrawDashedLine(color, line3, size * 0.125f, num5, num5);
				m_OverlayBuffer.DrawDashedLine(color, line4, size * 0.125f, num6, num6);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct BuildingTerraformRenderJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<BuildingTerraformData> m_PrefabBuildingTerraformData;

		[ReadOnly]
		public BufferLookup<AdditionalBuildingTerraformElement> m_PrefabAdditionalTerraform;

		[ReadOnly]
		public Entity m_Selected;

		public OverlayRenderSystem.Buffer m_OverlayBuffer;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = default(PrefabRef);
			BuildingTerraformData terraformData = default(BuildingTerraformData);
			if (m_PrefabRefData.TryGetComponent(m_Selected, ref prefabRef) && m_PrefabBuildingTerraformData.TryGetComponent(prefabRef.m_Prefab, ref terraformData))
			{
				Transform transform = m_TransformData[m_Selected];
				ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
				bool flag = (objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Standing) != 0;
				bool circular = ((uint)objectGeometryData.m_Flags & (uint)((!flag) ? 1 : 256)) != 0;
				DynamicBuffer<AdditionalBuildingTerraformElement> additionalElements = default(DynamicBuffer<AdditionalBuildingTerraformElement>);
				m_PrefabAdditionalTerraform.TryGetBuffer(prefabRef.m_Prefab, ref additionalElements);
				DrawTerraform(terraformData, additionalElements, transform.m_Position, transform.m_Rotation, circular);
			}
		}

		private void DrawTerraform(BuildingTerraformData terraformData, DynamicBuffer<AdditionalBuildingTerraformElement> additionalElements, float3 position, quaternion rotation, bool circular)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0509: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0524: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_0544: Unknown result type (might be due to invalid IL or missing references)
			//IL_054e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_055d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0567: Unknown result type (might be due to invalid IL or missing references)
			//IL_056c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_057d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0582: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			//IL_0589: Unknown result type (might be due to invalid IL or missing references)
			//IL_058a: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0596: Unknown result type (might be due to invalid IL or missing references)
			//IL_059b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05be: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_060d: Unknown result type (might be due to invalid IL or missing references)
			//IL_061d: Unknown result type (might be due to invalid IL or missing references)
			//IL_061e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0625: Unknown result type (might be due to invalid IL or missing references)
			//IL_0627: Unknown result type (might be due to invalid IL or missing references)
			//IL_0629: Unknown result type (might be due to invalid IL or missing references)
			//IL_063d: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_064e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0655: Unknown result type (might be due to invalid IL or missing references)
			//IL_0657: Unknown result type (might be due to invalid IL or missing references)
			//IL_0659: Unknown result type (might be due to invalid IL or missing references)
			//IL_066d: Unknown result type (might be due to invalid IL or missing references)
			//IL_067d: Unknown result type (might be due to invalid IL or missing references)
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0685: Unknown result type (might be due to invalid IL or missing references)
			//IL_0687: Unknown result type (might be due to invalid IL or missing references)
			//IL_0689: Unknown result type (might be due to invalid IL or missing references)
			//IL_069d: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06de: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_070d: Unknown result type (might be due to invalid IL or missing references)
			//IL_070e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0715: Unknown result type (might be due to invalid IL or missing references)
			//IL_0717: Unknown result type (might be due to invalid IL or missing references)
			//IL_0718: Unknown result type (might be due to invalid IL or missing references)
			//IL_072c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			Color magenta = Color.magenta;
			float3 val = math.rotate(rotation, math.right());
			float3 val2 = math.rotate(rotation, math.forward());
			if (circular)
			{
				Color fillColor = default(Color);
				((Color)(ref fillColor))._002Ector(magenta.r, magenta.g, magenta.b, 0f);
				float diameter = math.cmax(((float4)(ref terraformData.m_Smooth)).zw - ((float4)(ref terraformData.m_Smooth)).xy);
				m_OverlayBuffer.DrawCircle(magenta, fillColor, 0.25f, (OverlayRenderSystem.StyleFlags)0, ((float3)(ref val2)).xz, position, diameter);
			}
			else
			{
				float3 val3 = position + val * terraformData.m_Smooth.x + val2 * terraformData.m_Smooth.y;
				float3 val4 = position + val * terraformData.m_Smooth.x + val2 * terraformData.m_Smooth.w;
				float3 val5 = position + val * terraformData.m_Smooth.z + val2 * terraformData.m_Smooth.w;
				float3 val6 = position + val * terraformData.m_Smooth.z + val2 * terraformData.m_Smooth.y;
				m_OverlayBuffer.DrawLine(magenta, magenta, 0f, (OverlayRenderSystem.StyleFlags)0, new Segment(val3, val4), 0.25f, new float2(1f, 1f));
				m_OverlayBuffer.DrawLine(magenta, magenta, 0f, (OverlayRenderSystem.StyleFlags)0, new Segment(val4, val5), 0.25f, new float2(1f, 1f));
				m_OverlayBuffer.DrawLine(magenta, magenta, 0f, (OverlayRenderSystem.StyleFlags)0, new Segment(val5, val6), 0.25f, new float2(1f, 1f));
				m_OverlayBuffer.DrawLine(magenta, magenta, 0f, (OverlayRenderSystem.StyleFlags)0, new Segment(val6, val3), 0.25f, new float2(1f, 1f));
			}
			if (additionalElements.IsCreated)
			{
				Color fillColor2 = default(Color);
				for (int i = 0; i < additionalElements.Length; i++)
				{
					AdditionalBuildingTerraformElement additionalBuildingTerraformElement = additionalElements[i];
					if (additionalBuildingTerraformElement.m_Circular)
					{
						((Color)(ref fillColor2))._002Ector(magenta.r, magenta.g, magenta.b, 0f);
						float diameter2 = math.cmax(MathUtils.Size(additionalBuildingTerraformElement.m_Area));
						m_OverlayBuffer.DrawCircle(magenta, fillColor2, 0.25f, (OverlayRenderSystem.StyleFlags)0, ((float3)(ref val2)).xz, position, diameter2);
						continue;
					}
					float3 val7 = position + val * additionalBuildingTerraformElement.m_Area.min.x + val2 * additionalBuildingTerraformElement.m_Area.min.y;
					float3 val8 = position + val * additionalBuildingTerraformElement.m_Area.min.x + val2 * additionalBuildingTerraformElement.m_Area.max.y;
					float3 val9 = position + val * additionalBuildingTerraformElement.m_Area.max.x + val2 * additionalBuildingTerraformElement.m_Area.max.y;
					float3 val10 = position + val * additionalBuildingTerraformElement.m_Area.max.x + val2 * additionalBuildingTerraformElement.m_Area.min.y;
					m_OverlayBuffer.DrawLine(magenta, magenta, 0f, (OverlayRenderSystem.StyleFlags)0, new Segment(val7, val8), 0.25f, new float2(1f, 1f));
					m_OverlayBuffer.DrawLine(magenta, magenta, 0f, (OverlayRenderSystem.StyleFlags)0, new Segment(val8, val9), 0.25f, new float2(1f, 1f));
					m_OverlayBuffer.DrawLine(magenta, magenta, 0f, (OverlayRenderSystem.StyleFlags)0, new Segment(val9, val10), 0.25f, new float2(1f, 1f));
					m_OverlayBuffer.DrawLine(magenta, magenta, 0f, (OverlayRenderSystem.StyleFlags)0, new Segment(val10, val7), 0.25f, new float2(1f, 1f));
				}
			}
			float3 val11 = position + val * terraformData.m_FlatX0.x + val2 * terraformData.m_FlatZ0.y;
			float3 val12 = position + val * terraformData.m_FlatX0.y + val2 * terraformData.m_FlatZ0.x;
			float3 val13 = position + val * terraformData.m_FlatX0.y + val2 * terraformData.m_FlatZ1.x;
			float3 val14 = position + val * terraformData.m_FlatX0.z + val2 * terraformData.m_FlatZ1.y;
			float3 val15 = position + val * terraformData.m_FlatX1.z + val2 * terraformData.m_FlatZ1.y;
			float3 val16 = position + val * terraformData.m_FlatX1.y + val2 * terraformData.m_FlatZ1.z;
			float3 val17 = position + val * terraformData.m_FlatX1.y + val2 * terraformData.m_FlatZ0.z;
			float3 val18 = position + val * terraformData.m_FlatX1.x + val2 * terraformData.m_FlatZ0.y;
			m_OverlayBuffer.DrawLine(magenta, magenta, 0f, (OverlayRenderSystem.StyleFlags)0, new Segment(val11, val12), 0.5f, new float2(1f, 1f));
			m_OverlayBuffer.DrawLine(magenta, magenta, 0f, (OverlayRenderSystem.StyleFlags)0, new Segment(val12, val13), 0.5f, new float2(1f, 1f));
			m_OverlayBuffer.DrawLine(magenta, magenta, 0f, (OverlayRenderSystem.StyleFlags)0, new Segment(val13, val14), 0.5f, new float2(1f, 1f));
			m_OverlayBuffer.DrawLine(magenta, magenta, 0f, (OverlayRenderSystem.StyleFlags)0, new Segment(val14, val15), 0.5f, new float2(1f, 1f));
			m_OverlayBuffer.DrawLine(magenta, magenta, 0f, (OverlayRenderSystem.StyleFlags)0, new Segment(val15, val16), 0.5f, new float2(1f, 1f));
			m_OverlayBuffer.DrawLine(magenta, magenta, 0f, (OverlayRenderSystem.StyleFlags)0, new Segment(val16, val17), 0.5f, new float2(1f, 1f));
			m_OverlayBuffer.DrawLine(magenta, magenta, 0f, (OverlayRenderSystem.StyleFlags)0, new Segment(val17, val18), 0.5f, new float2(1f, 1f));
			m_OverlayBuffer.DrawLine(magenta, magenta, 0f, (OverlayRenderSystem.StyleFlags)0, new Segment(val18, val11), 0.5f, new float2(1f, 1f));
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Extension> __Game_Buildings_Extension_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AssetStamp> __Game_Objects_AssetStamp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Tree> __Game_Objects_Tree_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Composition> __Game_Net_Composition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Edge> __Game_Net_Edge_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Warning> __Game_Tools_Warning_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Error> __Game_Tools_Error_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> __Game_Prefabs_BuildingExtensionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AssetStampData> __Game_Prefabs_AssetStampData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ServiceUpgradeBuilding> __Game_Prefabs_ServiceUpgradeBuilding_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingTerraformData> __Game_Prefabs_BuildingTerraformData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<AdditionalBuildingTerraformElement> __Game_Prefabs_AdditionalBuildingTerraformElement_RO_BufferLookup;

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
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_Buildings_Extension_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Extension>(true);
			__Game_Objects_AssetStamp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AssetStamp>(true);
			__Game_Objects_Tree_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Tree>(true);
			__Game_Net_Composition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Composition>(true);
			__Game_Net_Edge_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Edge>(true);
			__Game_Net_EdgeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EndNodeGeometry>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Tools_Warning_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Warning>(true);
			__Game_Tools_Error_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Error>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Elevation>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingExtensionData>(true);
			__Game_Prefabs_AssetStampData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AssetStampData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_ServiceUpgradeBuilding_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceUpgradeBuilding>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_BuildingTerraformData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingTerraformData>(true);
			__Game_Prefabs_AdditionalBuildingTerraformElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AdditionalBuildingTerraformElement>(true);
		}
	}

	private ToolSystem m_ToolSystem;

	private OverlayRenderSystem m_OverlayRenderSystem;

	private EntityQuery m_LotQuery;

	private EntityQuery m_RenderingSettingsQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_OverlayRenderSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<OverlayRenderSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[3];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		val.Any = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Extension>(),
			ComponentType.ReadOnly<AssetStamp>(),
			ComponentType.ReadOnly<Taxiway>(),
			ComponentType.ReadOnly<Tree>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Hidden>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Overridden>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Error>() };
		val.Any = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Extension>(),
			ComponentType.ReadOnly<AssetStamp>(),
			ComponentType.ReadOnly<Taxiway>(),
			ComponentType.ReadOnly<Tree>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Hidden>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Overridden>()
		};
		array[1] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Warning>() };
		val.Any = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Extension>(),
			ComponentType.ReadOnly<AssetStamp>(),
			ComponentType.ReadOnly<Taxiway>(),
			ComponentType.ReadOnly<Tree>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Hidden>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Overridden>()
		};
		array[2] = val;
		m_LotQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_RenderingSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<RenderingSettingsData>() });
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		Entity val = (m_ToolSystem.actionMode.IsEditor() ? m_ToolSystem.selected : Entity.Null);
		bool flag = !((EntityQuery)(ref m_LotQuery)).IsEmptyIgnoreFilter;
		if (flag || !(val == Entity.Null))
		{
			RenderingSettingsData renderingSettingsData = new RenderingSettingsData
			{
				m_HoveredColor = new Color(0.5f, 0.5f, 1f, 1f),
				m_ErrorColor = new Color(1f, 0.5f, 0.5f, 1f),
				m_WarningColor = new Color(1f, 1f, 0.5f, 1f),
				m_OwnerColor = new Color(0.5f, 1f, 0.5f, 1f)
			};
			if (!((EntityQuery)(ref m_RenderingSettingsQuery)).IsEmptyIgnoreFilter)
			{
				RenderingSettingsData singleton = ((EntityQuery)(ref m_RenderingSettingsQuery)).GetSingleton<RenderingSettingsData>();
				renderingSettingsData.m_HoveredColor = singleton.m_HoveredColor;
				renderingSettingsData.m_ErrorColor = singleton.m_ErrorColor;
				renderingSettingsData.m_WarningColor = singleton.m_WarningColor;
				renderingSettingsData.m_OwnerColor = singleton.m_OwnerColor;
			}
			JobHandle dependencies;
			OverlayRenderSystem.Buffer buffer = m_OverlayRenderSystem.GetBuffer(out dependencies);
			((SystemBase)this).Dependency = JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies);
			if (flag)
			{
				bool zonesVisible = m_ToolSystem.activeTool != null && m_ToolSystem.activeTool.requireZones;
				BuildingLotRenderJob buildingLotRenderJob = new BuildingLotRenderJob
				{
					m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ExtensionType = InternalCompilerInterface.GetComponentTypeHandle<Extension>(ref __TypeHandle.__Game_Buildings_Extension_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_AssetStampType = InternalCompilerInterface.GetComponentTypeHandle<AssetStamp>(ref __TypeHandle.__Game_Objects_AssetStamp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_TreeType = InternalCompilerInterface.GetComponentTypeHandle<Tree>(ref __TypeHandle.__Game_Objects_Tree_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_CompositionType = InternalCompilerInterface.GetComponentTypeHandle<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_EdgeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_StartGeometryType = InternalCompilerInterface.GetComponentTypeHandle<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_EndGeometryType = InternalCompilerInterface.GetComponentTypeHandle<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_WarningType = InternalCompilerInterface.GetComponentTypeHandle<Warning>(ref __TypeHandle.__Game_Tools_Warning_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ErrorType = InternalCompilerInterface.GetComponentTypeHandle<Error>(ref __TypeHandle.__Game_Tools_Error_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_NetElevationData = InternalCompilerInterface.GetComponentLookup<Game.Net.Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabBuildingExtensionData = InternalCompilerInterface.GetComponentLookup<BuildingExtensionData>(ref __TypeHandle.__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabAssetStampData = InternalCompilerInterface.GetComponentLookup<AssetStampData>(ref __TypeHandle.__Game_Prefabs_AssetStampData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabServiceUpgradeBuildings = InternalCompilerInterface.GetBufferLookup<ServiceUpgradeBuilding>(ref __TypeHandle.__Game_Prefabs_ServiceUpgradeBuilding_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
					m_ZonesVisible = zonesVisible,
					m_RenderingSettingsData = renderingSettingsData,
					m_OverlayBuffer = buffer
				};
				((SystemBase)this).Dependency = JobChunkExtensions.Schedule<BuildingLotRenderJob>(buildingLotRenderJob, m_LotQuery, ((SystemBase)this).Dependency);
			}
			if (val != Entity.Null)
			{
				BuildingTerraformRenderJob buildingTerraformRenderJob = new BuildingTerraformRenderJob
				{
					m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabBuildingTerraformData = InternalCompilerInterface.GetComponentLookup<BuildingTerraformData>(ref __TypeHandle.__Game_Prefabs_BuildingTerraformData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabAdditionalTerraform = InternalCompilerInterface.GetBufferLookup<AdditionalBuildingTerraformElement>(ref __TypeHandle.__Game_Prefabs_AdditionalBuildingTerraformElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_Selected = val,
					m_OverlayBuffer = buffer
				};
				((SystemBase)this).Dependency = IJobExtensions.Schedule<BuildingTerraformRenderJob>(buildingTerraformRenderJob, ((SystemBase)this).Dependency);
			}
			m_OverlayRenderSystem.AddBufferWriter(((SystemBase)this).Dependency);
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
	public BuildingLotRenderSystem()
	{
	}
}
