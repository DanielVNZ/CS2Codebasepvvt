using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
using Game.Simulation;
using Game.Tools;
using Game.Zones;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class GuideLinesSystem : GameSystemBase
{
	public struct TooltipInfo
	{
		public TooltipType m_Type;

		public float3 m_Position;

		public float m_Value;

		public TooltipInfo(TooltipType type, float3 position, float value)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Type = type;
			m_Position = position;
			m_Value = value;
		}
	}

	public enum TooltipType
	{
		Angle,
		Length
	}

	[BurstCompile]
	private struct WaterToolGuideLinesJob : IJob
	{
		[ReadOnly]
		public ComponentTypeHandle<Game.Simulation.WaterSourceData> m_WaterSourceDataType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentLookup<GuideLineSettingsData> m_GuideLineSettingsData;

		[ReadOnly]
		public BufferLookup<WaterSourceColorElement> m_WaterSourceColors;

		[ReadOnly]
		public WaterToolSystem.Attribute m_Attribute;

		[ReadOnly]
		public float3 m_PositionOffset;

		[ReadOnly]
		public float3 m_CameraRight;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_WaterSourceChunks;

		[ReadOnly]
		public Entity m_GuideLineSettingsEntity;

		public OverlayRenderSystem.Buffer m_OverlayBuffer;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_0578: Unknown result type (might be due to invalid IL or missing references)
			//IL_0584: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_0594: Unknown result type (might be due to invalid IL or missing references)
			//IL_0596: Unknown result type (might be due to invalid IL or missing references)
			//IL_0598: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05df: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0601: Unknown result type (might be due to invalid IL or missing references)
			//IL_060d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_062a: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_0658: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_067f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0685: Unknown result type (might be due to invalid IL or missing references)
			//IL_0690: Unknown result type (might be due to invalid IL or missing references)
			GuideLineSettingsData guideLineSettingsData = m_GuideLineSettingsData[m_GuideLineSettingsEntity];
			DynamicBuffer<WaterSourceColorElement> val = m_WaterSourceColors[m_GuideLineSettingsEntity];
			Segment line = default(Segment);
			for (int i = 0; i < m_WaterSourceChunks.Length; i++)
			{
				ArchetypeChunk val2 = m_WaterSourceChunks[i];
				NativeArray<Game.Simulation.WaterSourceData> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<Game.Simulation.WaterSourceData>(ref m_WaterSourceDataType);
				NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<Transform>(ref m_TransformType);
				bool flag = ((ArchetypeChunk)(ref val2)).Has<Temp>(ref m_TempType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Game.Simulation.WaterSourceData waterSourceData = nativeArray[j];
					Transform transform = nativeArray2[j];
					WaterSourceColorElement waterSourceColorElement = val[math.clamp(waterSourceData.m_ConstantDepth, 0, val.Length - 1)];
					float3 val3 = math.forward(transform.m_Rotation);
					float num = math.max(1f, waterSourceData.m_Radius);
					float num2 = num * 0.02f;
					float3 position = transform.m_Position;
					if (waterSourceData.m_ConstantDepth > 0)
					{
						position.y = m_PositionOffset.y + waterSourceData.m_Amount;
					}
					else
					{
						position.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, position) + waterSourceData.m_Amount;
					}
					if (flag)
					{
						waterSourceColorElement.m_Fill.a = waterSourceColorElement.m_Fill.a * 0.5f + 0.5f;
						waterSourceColorElement.m_Outline.a = waterSourceColorElement.m_Outline.a * 0.5f + 0.5f;
					}
					m_OverlayBuffer.DrawCircle(waterSourceColorElement.m_Outline, waterSourceColorElement.m_Fill, num2, (OverlayRenderSystem.StyleFlags)0, ((float3)(ref val3)).xz, position, num * 2f);
					m_OverlayBuffer.DrawCircle(waterSourceColorElement.m_ProjectedOutline, waterSourceColorElement.m_ProjectedFill, num2, OverlayRenderSystem.StyleFlags.Projected, ((float3)(ref val3)).xz, position, num * 2f);
					if (!flag)
					{
						continue;
					}
					switch (m_Attribute)
					{
					case WaterToolSystem.Attribute.Location:
					{
						float2 xz2 = ((float3)(ref m_CameraRight)).xz;
						if (MathUtils.TryNormalize(ref xz2))
						{
							((Segment)(ref line))._002Ector(position, position);
							ref float3 a = ref line.a;
							((float3)(ref a)).xz = ((float3)(ref a)).xz - xz2 * (num * 0.5f);
							ref float3 b = ref line.b;
							((float3)(ref b)).xz = ((float3)(ref b)).xz + xz2 * (num * 0.5f);
							float dashLength = num * 0.5f - num2;
							m_OverlayBuffer.DrawDashedLine(guideLineSettingsData.m_HighPriorityColor, guideLineSettingsData.m_HighPriorityColor, 0f, (OverlayRenderSystem.StyleFlags)0, line, num2, dashLength, num2);
							float2 val6 = MathUtils.Left(xz2);
							((Segment)(ref line))._002Ector(position, position);
							ref float3 a2 = ref line.a;
							((float3)(ref a2)).xz = ((float3)(ref a2)).xz - val6 * (num * 0.5f);
							ref float3 b2 = ref line.b;
							((float3)(ref b2)).xz = ((float3)(ref b2)).xz + val6 * (num * 0.5f);
							m_OverlayBuffer.DrawDashedLine(guideLineSettingsData.m_HighPriorityColor, guideLineSettingsData.m_HighPriorityColor, 0f, (OverlayRenderSystem.StyleFlags)0, line, num2, dashLength, num2);
						}
						break;
					}
					case WaterToolSystem.Attribute.Radius:
					{
						float2 xz3 = ((float3)(ref m_CameraRight)).xz;
						if (MathUtils.TryNormalize(ref xz3))
						{
							float2 val7 = MathUtils.Left(xz3);
							float2 val8 = (xz3 + val7) * 0.70710677f;
							float3 startPos2 = position;
							float3 startTangent2 = default(float3);
							float3 endPos2 = position;
							float3 endTangent2 = default(float3);
							((float3)(ref startPos2)).xz = ((float3)(ref startPos2)).xz + val8 * (num - num2 * 0.5f);
							((float3)(ref startTangent2)).xz = MathUtils.Right(val8);
							((float3)(ref endPos2)).xz = ((float3)(ref endPos2)).xz + MathUtils.Right(val8) * (num - num2 * 0.5f);
							((float3)(ref endTangent2)).xz = -val8;
							Bezier4x3 curve2 = NetUtils.FitCurve(startPos2, startTangent2, endTangent2, endPos2);
							m_OverlayBuffer.DrawCurve(guideLineSettingsData.m_HighPriorityColor, guideLineSettingsData.m_HighPriorityColor, 0f, (OverlayRenderSystem.StyleFlags)0, curve2, num2);
							((float3)(ref curve2.a)).xz = 2f * ((float3)(ref position)).xz - ((float3)(ref curve2.a)).xz;
							((float3)(ref curve2.b)).xz = 2f * ((float3)(ref position)).xz - ((float3)(ref curve2.b)).xz;
							((float3)(ref curve2.c)).xz = 2f * ((float3)(ref position)).xz - ((float3)(ref curve2.c)).xz;
							((float3)(ref curve2.d)).xz = 2f * ((float3)(ref position)).xz - ((float3)(ref curve2.d)).xz;
							m_OverlayBuffer.DrawCurve(guideLineSettingsData.m_HighPriorityColor, guideLineSettingsData.m_HighPriorityColor, 0f, (OverlayRenderSystem.StyleFlags)0, curve2, num2);
						}
						break;
					}
					case WaterToolSystem.Attribute.Rate:
					case WaterToolSystem.Attribute.Height:
					{
						float2 xz = ((float3)(ref m_CameraRight)).xz;
						if (MathUtils.TryNormalize(ref xz))
						{
							float2 val4 = MathUtils.Left(xz);
							float2 val5 = (xz + val4) * 0.70710677f;
							float3 startPos = position;
							float3 startTangent = default(float3);
							float3 endPos = position;
							float3 endTangent = default(float3);
							((float3)(ref startPos)).xz = ((float3)(ref startPos)).xz + val5 * (num - num2 * 0.5f);
							((float3)(ref startTangent)).xz = MathUtils.Left(val5);
							((float3)(ref endPos)).xz = ((float3)(ref endPos)).xz + MathUtils.Left(val5) * (num - num2 * 0.5f);
							((float3)(ref endTangent)).xz = -val5;
							Bezier4x3 curve = NetUtils.FitCurve(startPos, startTangent, endTangent, endPos);
							m_OverlayBuffer.DrawCurve(guideLineSettingsData.m_HighPriorityColor, guideLineSettingsData.m_HighPriorityColor, 0f, (OverlayRenderSystem.StyleFlags)0, curve, num2);
							((float3)(ref curve.a)).xz = 2f * ((float3)(ref position)).xz - ((float3)(ref curve.a)).xz;
							((float3)(ref curve.b)).xz = 2f * ((float3)(ref position)).xz - ((float3)(ref curve.b)).xz;
							((float3)(ref curve.c)).xz = 2f * ((float3)(ref position)).xz - ((float3)(ref curve.c)).xz;
							((float3)(ref curve.d)).xz = 2f * ((float3)(ref position)).xz - ((float3)(ref curve.d)).xz;
							m_OverlayBuffer.DrawCurve(guideLineSettingsData.m_HighPriorityColor, guideLineSettingsData.m_HighPriorityColor, 0f, (OverlayRenderSystem.StyleFlags)0, curve, num2);
						}
						break;
					}
					}
				}
			}
		}
	}

	[BurstCompile]
	private struct ObjectToolGuideLinesJob : IJob
	{
		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> m_CreationDefinitionType;

		[ReadOnly]
		public ComponentTypeHandle<ObjectDefinition> m_ObjectDefinitionType;

		[ReadOnly]
		public ComponentTypeHandle<OwnerDefinition> m_OwnerDefinitionType;

		[ReadOnly]
		public ComponentTypeHandle<NetCourse> m_NetCourseType;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> m_PrefabPlaceableObjectData;

		[ReadOnly]
		public ComponentLookup<ServiceUpgradeData> m_PrefabServiceUpgradeData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<LotData> m_PrefabLotData;

		[ReadOnly]
		public ComponentLookup<PlaceableNetData> m_PrefabPlaceableNetData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubArea> m_PrefabSubAreas;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubNet> m_PrefabSubNets;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_DefinitionChunks;

		[ReadOnly]
		public NativeList<ControlPoint> m_ControlPoints;

		[ReadOnly]
		public NativeList<SubSnapPoint> m_SubSnapPoints;

		[ReadOnly]
		public NativeList<NetToolSystem.UpgradeState> m_NetUpgradeState;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[ReadOnly]
		public GuideLineSettingsData m_GuideLineSettingsData;

		[ReadOnly]
		public ObjectToolSystem.Mode m_Mode;

		[ReadOnly]
		public ObjectToolSystem.State m_State;

		[ReadOnly]
		public Entity m_Prefab;

		[ReadOnly]
		public float m_DistanceScale;

		public NativeList<bool> m_AngleSides;

		public NativeList<TooltipInfo> m_Tooltips;

		public OverlayRenderSystem.Buffer m_OverlayBuffer;

		public void Execute()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			bool flag = m_NetUpgradeState.Length != 0;
			NativeParallelHashSet<float> val = default(NativeParallelHashSet<float>);
			int angleIndex = 0;
			if (!flag && m_State != ObjectToolSystem.State.Adding && m_State != ObjectToolSystem.State.Removing && (m_Mode == ObjectToolSystem.Mode.Line || m_Mode == ObjectToolSystem.Mode.Curve))
			{
				DrawControlPoints();
			}
			NetGeometryData netGeometryData = default(NetGeometryData);
			PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
			Segment line = default(Segment);
			DynamicBuffer<Game.Prefabs.SubArea> val3 = default(DynamicBuffer<Game.Prefabs.SubArea>);
			LotData lotData = default(LotData);
			ServiceUpgradeData serviceUpgradeData = default(ServiceUpgradeData);
			OwnerDefinition ownerDefinition = default(OwnerDefinition);
			BuildingData ownerBuildingData = default(BuildingData);
			BuildingData buildingData = default(BuildingData);
			for (int i = 0; i < m_DefinitionChunks.Length; i++)
			{
				ArchetypeChunk val2 = m_DefinitionChunks[i];
				NativeArray<CreationDefinition> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
				if (flag)
				{
					NativeArray<NetCourse> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<NetCourse>(ref m_NetCourseType);
					for (int j = 0; j < nativeArray2.Length; j++)
					{
						CreationDefinition creationDefinition = nativeArray[j];
						NetCourse netCourse = nativeArray2[j];
						if ((creationDefinition.m_Flags & CreationFlags.Permanent) == 0 && m_PrefabNetGeometryData.TryGetComponent(creationDefinition.m_Prefab, ref netGeometryData))
						{
							DrawNetCourse(m_OverlayBuffer, netCourse, ref m_TerrainHeightData, ref m_WaterSurfaceData, netGeometryData, m_GuideLineSettingsData);
						}
					}
					continue;
				}
				NativeArray<ObjectDefinition> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<ObjectDefinition>(ref m_ObjectDefinitionType);
				NativeArray<OwnerDefinition> nativeArray4 = ((ArchetypeChunk)(ref val2)).GetNativeArray<OwnerDefinition>(ref m_OwnerDefinitionType);
				for (int k = 0; k < nativeArray3.Length; k++)
				{
					CreationDefinition creationDefinition2 = nativeArray[k];
					ObjectDefinition objectDefinition = nativeArray3[k];
					if ((creationDefinition2.m_Flags & CreationFlags.Permanent) != 0)
					{
						continue;
					}
					if (m_PrefabPlaceableObjectData.TryGetComponent(creationDefinition2.m_Prefab, ref placeableObjectData) && (placeableObjectData.m_Flags & Game.Objects.PlacementFlags.Hovering) != Game.Objects.PlacementFlags.None)
					{
						ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[creationDefinition2.m_Prefab];
						float num = MathUtils.Center(((Bounds3)(ref objectGeometryData.m_Bounds)).x);
						float width = MathUtils.Size(((Bounds3)(ref objectGeometryData.m_Bounds)).x);
						line.a = ObjectUtils.LocalToWorld(objectDefinition.m_Position, objectDefinition.m_Rotation, new float3(num, 0f, objectGeometryData.m_Bounds.min.z));
						line.b = ObjectUtils.LocalToWorld(objectDefinition.m_Position, objectDefinition.m_Rotation, new float3(num, 0f, objectGeometryData.m_Bounds.max.z));
						m_OverlayBuffer.DrawLine(m_GuideLineSettingsData.m_MediumPriorityColor, m_GuideLineSettingsData.m_MediumPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, line, width, float2.op_Implicit(0.02f));
					}
					if (m_PrefabSubAreas.TryGetBuffer(creationDefinition2.m_Prefab, ref val3))
					{
						for (int l = 0; l < val3.Length; l++)
						{
							Game.Prefabs.SubArea subArea = val3[l];
							if (m_PrefabLotData.TryGetComponent(subArea.m_Prefab, ref lotData) && lotData.m_MaxRadius > 0f)
							{
								if (!val.IsCreated)
								{
									val._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
								}
								if (val.Add(lotData.m_MaxRadius))
								{
									DrawAreaRange(m_OverlayBuffer, objectDefinition.m_Rotation, objectDefinition.m_Position, lotData);
								}
							}
						}
						if (val.IsCreated)
						{
							val.Clear();
						}
					}
					if (m_PrefabServiceUpgradeData.TryGetComponent(creationDefinition2.m_Prefab, ref serviceUpgradeData) && serviceUpgradeData.m_MaxPlacementDistance != 0f && CollectionUtils.TryGet<OwnerDefinition>(nativeArray4, k, ref ownerDefinition) && m_PrefabBuildingData.TryGetComponent(ownerDefinition.m_Prefab, ref ownerBuildingData) && m_PrefabBuildingData.TryGetComponent(creationDefinition2.m_Prefab, ref buildingData))
					{
						DrawUpgradeRange(m_OverlayBuffer, ownerDefinition.m_Rotation, ownerDefinition.m_Position, m_GuideLineSettingsData, ownerBuildingData, buildingData, serviceUpgradeData);
					}
					DrawSubSnapPoints(creationDefinition2.m_Prefab, objectDefinition.m_Position, objectDefinition.m_Rotation, ref angleIndex);
				}
			}
			if (m_Mode == ObjectToolSystem.Mode.Stamp)
			{
				for (int m = 0; m < m_ControlPoints.Length; m++)
				{
					ControlPoint controlPoint = m_ControlPoints[m];
					DrawSubSnapPoints(m_Prefab, controlPoint.m_Position, controlPoint.m_Rotation, ref angleIndex);
				}
			}
			if (val.IsCreated)
			{
				val.Dispose();
			}
		}

		private void DrawControlPoints()
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_06be: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0779: Unknown result type (might be due to invalid IL or missing references)
			//IL_077e: Unknown result type (might be due to invalid IL or missing references)
			//IL_078c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0797: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0702: Unknown result type (might be due to invalid IL or missing references)
			//IL_071f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0735: Unknown result type (might be due to invalid IL or missing references)
			//IL_0740: Unknown result type (might be due to invalid IL or missing references)
			//IL_074b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0753: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_057f: Unknown result type (might be due to invalid IL or missing references)
			//IL_058d: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_05be: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05db: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0608: Unknown result type (might be due to invalid IL or missing references)
			//IL_0614: Unknown result type (might be due to invalid IL or missing references)
			//IL_062d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0638: Unknown result type (might be due to invalid IL or missing references)
			//IL_0643: Unknown result type (might be due to invalid IL or missing references)
			//IL_0650: Unknown result type (might be due to invalid IL or missing references)
			//IL_0666: Unknown result type (might be due to invalid IL or missing references)
			//IL_0671: Unknown result type (might be due to invalid IL or missing references)
			//IL_067c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0689: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			int angleIndex = 0;
			Segment prevLine = default(Segment);
			float3 prevPoint = float3.op_Implicit(-1000000f);
			float num = m_DistanceScale;
			float num2 = num * 0.125f;
			float num3 = num * 4f;
			ControlPoint controlPoint;
			float2 val4;
			if (m_ControlPoints.Length >= 2)
			{
				Segment val = default(Segment);
				((Segment)(ref val))._002Ector(m_ControlPoints[0].m_Position, m_ControlPoints[1].m_Position);
				float num4 = MathUtils.Length(((Segment)(ref val)).xz);
				if (num4 > num2 * 7f)
				{
					float2 val2 = (((float3)(ref val.b)).xz - ((float3)(ref val.a)).xz) / num4;
					float2 leftDir = default(float2);
					float2 rightDir = default(float2);
					float2 leftDir2 = default(float2);
					float2 rightDir2 = default(float2);
					int bestLeft = 181;
					int bestRight = 181;
					int bestLeft2 = 181;
					int bestRight2 = 181;
					float2 val3 = default(float2);
					controlPoint = m_ControlPoints[0];
					ref float2 direction = ref controlPoint.m_Direction;
					val4 = default(float2);
					Transform transform = default(Transform);
					if (!((float2)(ref direction)).Equals(val4))
					{
						val3 = m_ControlPoints[0].m_Direction;
					}
					else if (m_TransformData.TryGetComponent(m_ControlPoints[0].m_OriginalEntity, ref transform))
					{
						float3 val5 = math.forward(transform.m_Rotation);
						val3 = ((float3)(ref val5)).xz;
					}
					val4 = default(float2);
					if (!((float2)(ref val3)).Equals(val4))
					{
						CheckDirection(val2, val3, ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
						CheckDirection(val2, MathUtils.Right(val3), ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
						CheckDirection(val2, -val3, ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
						CheckDirection(val2, MathUtils.Left(val3), ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
					}
					bool flag = bestRight < bestLeft;
					if (bestLeft == bestRight && m_AngleSides.Length > angleIndex)
					{
						flag = m_AngleSides[angleIndex];
					}
					if (bestLeft == 180 && bestRight == 180)
					{
						if (flag)
						{
							bestLeft = 181;
						}
						else
						{
							bestRight = 181;
						}
					}
					else
					{
						if (bestLeft2 <= 180 && bestRight2 <= 180)
						{
							if (bestLeft2 < bestRight2 || (bestLeft2 == bestRight2 && flag))
							{
								bestRight2 = 181;
							}
							else
							{
								bestLeft2 = 181;
							}
						}
						if (bestLeft2 <= 180)
						{
							leftDir = leftDir2;
							bestLeft = bestLeft2;
						}
						else if (bestRight2 <= 180)
						{
							rightDir = rightDir2;
							bestRight = bestRight2;
						}
					}
					if (bestLeft <= 180)
					{
						Segment val6 = default(Segment);
						((Segment)(ref val6))._002Ector(val.a, val.a);
						ref float3 a = ref val6.a;
						((float3)(ref a)).xz = ((float3)(ref a)).xz + leftDir * math.min(num4, num3);
						m_OverlayBuffer.DrawLine(m_GuideLineSettingsData.m_HighPriorityColor, val6, num2);
						GuideLinesSystem.DrawAngleIndicator(m_OverlayBuffer, m_Tooltips, m_GuideLineSettingsData, val6, val, -leftDir, -val2, math.min(num4, num3) * 0.5f, num2, bestLeft, angleSide: false);
					}
					if (bestRight <= 180)
					{
						Segment val7 = default(Segment);
						((Segment)(ref val7))._002Ector(val.a, val.a);
						ref float3 a2 = ref val7.a;
						((float3)(ref a2)).xz = ((float3)(ref a2)).xz + rightDir * math.min(num4, num3);
						m_OverlayBuffer.DrawLine(m_GuideLineSettingsData.m_HighPriorityColor, val7, num2);
						GuideLinesSystem.DrawAngleIndicator(m_OverlayBuffer, m_Tooltips, m_GuideLineSettingsData, val7, val, -rightDir, -val2, math.min(num4, num3) * 0.5f, num2, bestRight, angleSide: true);
					}
					if (m_AngleSides.Length > angleIndex)
					{
						m_AngleSides[angleIndex] = flag;
					}
					else
					{
						while (m_AngleSides.Length <= angleIndex)
						{
							m_AngleSides.Add(ref flag);
						}
					}
				}
				angleIndex++;
			}
			if (m_ControlPoints.Length >= 2)
			{
				ControlPoint controlPoint2 = m_ControlPoints[0];
				for (int i = 1; i < m_ControlPoints.Length; i++)
				{
					ControlPoint controlPoint3 = m_ControlPoints[i];
					float num5 = (float)Mathf.RoundToInt(math.distance(((float3)(ref controlPoint2.m_Position)).xz, ((float3)(ref controlPoint3.m_Position)).xz) * 2f) / 2f;
					if (num5 > 0f)
					{
						ref NativeList<TooltipInfo> reference = ref m_Tooltips;
						TooltipInfo tooltipInfo = new TooltipInfo(TooltipType.Length, (controlPoint2.m_Position + controlPoint3.m_Position) * 0.5f, num5);
						reference.Add(ref tooltipInfo);
					}
					controlPoint2 = controlPoint3;
				}
			}
			if (m_ControlPoints.Length >= 3)
			{
				controlPoint = m_ControlPoints[0];
				val4 = ((float3)(ref controlPoint.m_Position)).xz;
				controlPoint = m_ControlPoints[1];
				if (!((float2)(ref val4)).Equals(((float3)(ref controlPoint.m_Position)).xz))
				{
					controlPoint = m_ControlPoints[2];
					val4 = ((float3)(ref controlPoint.m_Position)).xz;
					controlPoint = m_ControlPoints[1];
					if (!((float2)(ref val4)).Equals(((float3)(ref controlPoint.m_Position)).xz))
					{
						float3 val8 = m_ControlPoints[1].m_Position - m_ControlPoints[0].m_Position;
						float3 val9 = m_ControlPoints[2].m_Position - m_ControlPoints[1].m_Position;
						val8 = MathUtils.Normalize(val8, ((float3)(ref val8)).xz);
						val9 = MathUtils.Normalize(val9, ((float3)(ref val9)).xz);
						Bezier4x3 val10 = NetUtils.FitCurve(m_ControlPoints[0].m_Position, val8, val9, m_ControlPoints[2].m_Position);
						float num6 = NetUtils.FindMiddleTangentPos(((Bezier4x3)(ref val10)).xz, new float2(0f, 1f));
						Bezier4x3 curve = default(Bezier4x3);
						Bezier4x3 curve2 = default(Bezier4x3);
						MathUtils.Divide(val10, ref curve, ref curve2, num6);
						m_OverlayBuffer.DrawCurve(m_GuideLineSettingsData.m_MediumPriorityColor, m_GuideLineSettingsData.m_MediumPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, curve, num2, new float2(1f, 0f));
						m_OverlayBuffer.DrawCurve(m_GuideLineSettingsData.m_MediumPriorityColor, m_GuideLineSettingsData.m_MediumPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, curve2, num2, new float2(0f, 1f));
						goto IL_07b9;
					}
				}
			}
			if (m_ControlPoints.Length >= 2)
			{
				controlPoint = m_ControlPoints[0];
				val4 = ((float3)(ref controlPoint.m_Position)).xz;
				controlPoint = m_ControlPoints[m_ControlPoints.Length - 1];
				if (!((float2)(ref val4)).Equals(((float3)(ref controlPoint.m_Position)).xz))
				{
					Segment line = default(Segment);
					((Segment)(ref line))._002Ector(m_ControlPoints[0].m_Position, m_ControlPoints[m_ControlPoints.Length - 1].m_Position);
					m_OverlayBuffer.DrawLine(m_GuideLineSettingsData.m_MediumPriorityColor, m_GuideLineSettingsData.m_MediumPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, line, num2, float2.op_Implicit(1f));
					goto IL_07b9;
				}
			}
			if (m_ControlPoints.Length >= 1)
			{
				float3 position = m_ControlPoints[0].m_Position;
				m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_MediumPriorityColor, m_GuideLineSettingsData.m_MediumPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, new float2(0f, 1f), position, num2);
			}
			goto IL_07b9;
			IL_07b9:
			for (int j = 0; j < m_ControlPoints.Length; j++)
			{
				ControlPoint controlPoint4 = m_ControlPoints[j];
				if (j > 0)
				{
					ControlPoint point = m_ControlPoints[j - 1];
					DrawControlPointLine(point, controlPoint4, num2, num3, ref angleIndex, ref prevLine);
				}
				DrawControlPoint(controlPoint4, num2, ref prevPoint);
			}
		}

		private void DrawControlPoint(ControlPoint point, float lineWidth, ref float3 prevPoint)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (math.distancesq(prevPoint, point.m_Position) > 0.01f)
			{
				m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_HighPriorityColor, point.m_Position, lineWidth * 5f);
			}
			prevPoint = point.m_Position;
		}

		private void DrawControlPointLine(ControlPoint point1, ControlPoint point2, float lineWidth, float lineLength, ref int angleIndex, ref Segment prevLine)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			Segment val = default(Segment);
			((Segment)(ref val))._002Ector(point1.m_Position, point2.m_Position);
			float num = math.distance(((float3)(ref point1.m_Position)).xz, ((float3)(ref point2.m_Position)).xz);
			if (num > lineWidth * 8f)
			{
				float3 val2 = (val.b - val.a) * (lineWidth * 4f / num);
				Segment line = default(Segment);
				((Segment)(ref line))._002Ector(val.a + val2, val.b - val2);
				m_OverlayBuffer.DrawDashedLine(m_GuideLineSettingsData.m_HighPriorityColor, line, lineWidth * 3f, lineWidth * 5f, lineWidth * 3f);
			}
			DrawAngleIndicator(prevLine, val, lineWidth, lineLength, angleIndex++);
			prevLine = val;
		}

		private void DrawAngleIndicator(Segment line1, Segment line2, float lineWidth, float lineLength, int angleIndex)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			bool flag = true;
			if (m_AngleSides.Length > angleIndex)
			{
				flag = m_AngleSides[angleIndex];
			}
			float num = math.distance(((float3)(ref line1.a)).xz, ((float3)(ref line1.b)).xz);
			float num2 = math.distance(((float3)(ref line2.a)).xz, ((float3)(ref line2.b)).xz);
			if (num > lineWidth * 7f && num2 > lineWidth * 7f)
			{
				float2 val = (((float3)(ref line1.b)).xz - ((float3)(ref line1.a)).xz) / num;
				float2 val2 = (((float3)(ref line2.a)).xz - ((float3)(ref line2.b)).xz) / num2;
				float size = math.min(lineLength, math.min(num, num2)) * 0.5f;
				int num3 = Mathf.RoundToInt(math.degrees(math.acos(math.clamp(math.dot(val, val2), -1f, 1f))));
				if (num3 < 180)
				{
					flag = math.dot(MathUtils.Right(val), val2) < 0f;
				}
				GuideLinesSystem.DrawAngleIndicator(m_OverlayBuffer, m_Tooltips, m_GuideLineSettingsData, line1, line2, val, val2, size, lineWidth, num3, flag);
			}
			if (m_AngleSides.Length > angleIndex)
			{
				m_AngleSides[angleIndex] = flag;
				return;
			}
			while (m_AngleSides.Length <= angleIndex)
			{
				m_AngleSides.Add(ref flag);
			}
		}

		private void DrawSubSnapPoints(Entity prefab, float3 position, quaternion rotation, ref int angleIndex)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Prefabs.SubNet> val = default(DynamicBuffer<Game.Prefabs.SubNet>);
			if (!m_PrefabSubNets.TryGetBuffer(prefab, ref val))
			{
				return;
			}
			for (int i = 0; i < val.Length; i++)
			{
				Game.Prefabs.SubNet subNet = val[i];
				float3 val2;
				if (subNet.m_Snapping.x)
				{
					float3 pos = ObjectUtils.LocalToWorld(position, rotation, subNet.m_Curve.a);
					val2 = math.mul(rotation, MathUtils.StartTangent(subNet.m_Curve));
					float2 tangent = math.select(default(float2), math.normalizesafe(((float3)(ref val2)).xz, default(float2)), subNet.m_NodeIndex.y != subNet.m_NodeIndex.x);
					DrawSubSnapPoints(subNet.m_Prefab, pos, tangent, ref angleIndex);
				}
				if (subNet.m_Snapping.y)
				{
					float3 pos2 = ObjectUtils.LocalToWorld(position, rotation, subNet.m_Curve.d);
					val2 = math.mul(rotation, -MathUtils.EndTangent(subNet.m_Curve));
					float2 tangent2 = math.normalizesafe(((float3)(ref val2)).xz, default(float2));
					DrawSubSnapPoints(subNet.m_Prefab, pos2, tangent2, ref angleIndex);
				}
			}
		}

		private void DrawSubSnapPoints(Entity prefab, float3 pos, float2 tangent, ref int angleIndex)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			float2 leftDir = default(float2);
			float2 rightDir = default(float2);
			float2 leftDir2 = default(float2);
			float2 rightDir2 = default(float2);
			int bestLeft = 181;
			int bestRight = 181;
			int bestLeft2 = 181;
			int bestRight2 = 181;
			for (int i = 0; i < m_SubSnapPoints.Length; i++)
			{
				SubSnapPoint subSnapPoint = m_SubSnapPoints[i];
				if (!(math.distancesq(((float3)(ref pos)).xz, ((float3)(ref subSnapPoint.m_Position)).xz) >= 0.01f))
				{
					CheckDirection(tangent, subSnapPoint.m_Tangent, ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
				}
			}
			float num = 1f;
			PlaceableNetData placeableNetData = default(PlaceableNetData);
			if (m_PrefabPlaceableNetData.TryGetComponent(prefab, ref placeableNetData))
			{
				num = math.min(placeableNetData.m_SnapDistance, 16f);
			}
			float num2 = num * 0.125f;
			float num3 = num * 4f;
			bool flag = bestRight < bestLeft;
			if (bestLeft == bestRight && m_AngleSides.Length > angleIndex)
			{
				flag = m_AngleSides[angleIndex];
			}
			if (bestLeft == 180 && bestRight == 180)
			{
				if (flag)
				{
					bestLeft = 181;
				}
				else
				{
					bestRight = 181;
				}
			}
			else
			{
				if (bestLeft2 <= 180 && bestRight2 <= 180)
				{
					if (bestLeft2 < bestRight2 || (bestLeft2 == bestRight2 && flag))
					{
						bestRight2 = 181;
					}
					else
					{
						bestLeft2 = 181;
					}
				}
				if (bestLeft2 <= 180)
				{
					leftDir = leftDir2;
					bestLeft = bestLeft2;
				}
				else if (bestRight2 <= 180)
				{
					rightDir = rightDir2;
					bestRight = bestRight2;
				}
			}
			if (bestLeft <= 180 || bestRight <= 180)
			{
				if (((float2)(ref tangent)).Equals(default(float2)))
				{
					Color highPriorityColor = m_GuideLineSettingsData.m_HighPriorityColor;
					highPriorityColor.a = 0f;
					m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_HighPriorityColor, highPriorityColor, num2, (OverlayRenderSystem.StyleFlags)0, new float2(0f, 1f), pos, num3 * 0.5f);
				}
				else
				{
					Segment val = default(Segment);
					((Segment)(ref val))._002Ector(pos, pos);
					ref float3 b = ref val.b;
					((float3)(ref b)).xz = ((float3)(ref b)).xz + tangent * num3;
					m_OverlayBuffer.DrawLine(m_GuideLineSettingsData.m_HighPriorityColor, val, num2);
					if (bestLeft <= 180)
					{
						Segment val2 = default(Segment);
						((Segment)(ref val2))._002Ector(pos, pos);
						ref float3 a = ref val2.a;
						((float3)(ref a)).xz = ((float3)(ref a)).xz + leftDir * num3;
						m_OverlayBuffer.DrawLine(m_GuideLineSettingsData.m_HighPriorityColor, val2, num2);
						GuideLinesSystem.DrawAngleIndicator(m_OverlayBuffer, m_Tooltips, m_GuideLineSettingsData, val2, val, -leftDir, -tangent, num3 * 0.5f, num2, bestLeft, angleSide: false);
					}
					if (bestRight <= 180)
					{
						Segment val3 = default(Segment);
						((Segment)(ref val3))._002Ector(pos, pos);
						ref float3 a2 = ref val3.a;
						((float3)(ref a2)).xz = ((float3)(ref a2)).xz + rightDir * num3;
						m_OverlayBuffer.DrawLine(m_GuideLineSettingsData.m_HighPriorityColor, val3, num2);
						GuideLinesSystem.DrawAngleIndicator(m_OverlayBuffer, m_Tooltips, m_GuideLineSettingsData, val3, val, -rightDir, -tangent, num3 * 0.5f, num2, bestRight, angleSide: true);
					}
				}
			}
			if (m_AngleSides.Length > angleIndex)
			{
				m_AngleSides[angleIndex] = flag;
			}
			else
			{
				while (m_AngleSides.Length <= angleIndex)
				{
					m_AngleSides.Add(ref flag);
				}
			}
			angleIndex++;
		}
	}

	[BurstCompile]
	private struct AreaToolGuideLinesJob : IJob
	{
		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> m_CreationDefinitionType;

		[ReadOnly]
		public BufferTypeHandle<Game.Areas.Node> m_NodeType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public ComponentLookup<LotData> m_PrefabLotData;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_Nodes;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_DefinitionChunks;

		[ReadOnly]
		public NativeList<ControlPoint> m_ControlPoints;

		[ReadOnly]
		public NativeList<ControlPoint> m_MoveStartPositions;

		[ReadOnly]
		public AreaToolSystem.State m_State;

		[ReadOnly]
		public Entity m_Prefab;

		[ReadOnly]
		public GuideLineSettingsData m_GuideLineSettingsData;

		public NativeList<bool> m_AngleSides;

		public NativeList<TooltipInfo> m_Tooltips;

		public OverlayRenderSystem.Buffer m_OverlayBuffer;

		public void Execute()
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_0613: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0588: Unknown result type (might be due to invalid IL or missing references)
			//IL_0624: Unknown result type (might be due to invalid IL or missing references)
			//IL_062a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0641: Unknown result type (might be due to invalid IL or missing references)
			//IL_0655: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0699: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			if (m_ControlPoints.Length <= 0)
			{
				return;
			}
			switch (m_State)
			{
			case AreaToolSystem.State.Default:
			{
				for (int n = 0; n < m_DefinitionChunks.Length; n++)
				{
					ArchetypeChunk val4 = m_DefinitionChunks[n];
					NativeArray<CreationDefinition> nativeArray3 = ((ArchetypeChunk)(ref val4)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
					BufferAccessor<Game.Areas.Node> bufferAccessor3 = ((ArchetypeChunk)(ref val4)).GetBufferAccessor<Game.Areas.Node>(ref m_NodeType);
					for (int num = 0; num < bufferAccessor3.Length; num++)
					{
						CreationDefinition creationDefinition3 = nativeArray3[num];
						if ((creationDefinition3.m_Flags & CreationFlags.Permanent) != 0 || !m_PrefabRefData.HasComponent(creationDefinition3.m_Original) || !m_OwnerData.HasComponent(creationDefinition3.m_Original))
						{
							continue;
						}
						PrefabRef prefabRef3 = m_PrefabRefData[creationDefinition3.m_Original];
						Owner owner3 = m_OwnerData[creationDefinition3.m_Original];
						if (m_PrefabLotData.HasComponent(prefabRef3.m_Prefab) && m_TransformData.HasComponent(owner3.m_Owner))
						{
							LotData lotData3 = m_PrefabLotData[prefabRef3.m_Prefab];
							Transform transform3 = m_TransformData[owner3.m_Owner];
							if (!(lotData3.m_MaxRadius <= 0f))
							{
								DrawAreaRange(m_OverlayBuffer, transform3.m_Rotation, transform3.m_Position, lotData3);
							}
						}
					}
				}
				ControlPoint controlPoint3 = m_ControlPoints[0];
				if (m_Nodes.HasBuffer(controlPoint3.m_OriginalEntity) && math.any(controlPoint3.m_ElementIndex >= 0))
				{
					PrefabRef prefabRef4 = m_PrefabRefData[controlPoint3.m_OriginalEntity];
					AreaGeometryData areaGeometryData3 = m_PrefabGeometryData[prefabRef4.m_Prefab];
					m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_HighPriorityColor, controlPoint3.m_Position, areaGeometryData3.m_SnapDistance * 0.5f);
					break;
				}
				for (int num2 = 0; num2 < m_DefinitionChunks.Length; num2++)
				{
					ArchetypeChunk val5 = m_DefinitionChunks[num2];
					NativeArray<CreationDefinition> nativeArray4 = ((ArchetypeChunk)(ref val5)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
					BufferAccessor<Game.Areas.Node> bufferAccessor4 = ((ArchetypeChunk)(ref val5)).GetBufferAccessor<Game.Areas.Node>(ref m_NodeType);
					for (int num3 = 0; num3 < bufferAccessor4.Length; num3++)
					{
						CreationDefinition creationDefinition4 = nativeArray4[num3];
						DynamicBuffer<Game.Areas.Node> val6 = bufferAccessor4[num3];
						if ((creationDefinition4.m_Flags & CreationFlags.Permanent) == 0 && m_PrefabRefData.HasComponent(creationDefinition4.m_Original) && val6.Length != 0)
						{
							PrefabRef prefabRef5 = m_PrefabRefData[creationDefinition4.m_Original];
							if (m_PrefabGeometryData.HasComponent(prefabRef5.m_Prefab))
							{
								AreaGeometryData areaGeometryData4 = m_PrefabGeometryData[prefabRef5.m_Prefab];
								Game.Areas.Node node2 = val6[0];
								m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_HighPriorityColor, node2.m_Position, areaGeometryData4.m_SnapDistance * 0.5f);
							}
						}
					}
				}
				break;
			}
			case AreaToolSystem.State.Create:
			{
				for (int j = 0; j < m_DefinitionChunks.Length; j++)
				{
					ArchetypeChunk val = m_DefinitionChunks[j];
					NativeArray<CreationDefinition> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
					BufferAccessor<Game.Areas.Node> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<Game.Areas.Node>(ref m_NodeType);
					for (int k = 0; k < bufferAccessor.Length; k++)
					{
						CreationDefinition creationDefinition = nativeArray[k];
						if ((creationDefinition.m_Flags & CreationFlags.Permanent) == 0 && m_PrefabLotData.HasComponent(creationDefinition.m_Prefab) && m_OwnerData.HasComponent(creationDefinition.m_Original))
						{
							LotData lotData2 = m_PrefabLotData[creationDefinition.m_Prefab];
							Owner owner2 = m_OwnerData[creationDefinition.m_Original];
							if (!(lotData2.m_MaxRadius <= 0f) && m_TransformData.HasComponent(owner2.m_Owner))
							{
								Transform transform2 = m_TransformData[owner2.m_Owner];
								DrawAreaRange(m_OverlayBuffer, transform2.m_Rotation, transform2.m_Position, lotData2);
							}
						}
					}
				}
				for (int l = 0; l < m_DefinitionChunks.Length; l++)
				{
					ArchetypeChunk val2 = m_DefinitionChunks[l];
					NativeArray<CreationDefinition> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
					BufferAccessor<Game.Areas.Node> bufferAccessor2 = ((ArchetypeChunk)(ref val2)).GetBufferAccessor<Game.Areas.Node>(ref m_NodeType);
					for (int m = 0; m < bufferAccessor2.Length; m++)
					{
						CreationDefinition creationDefinition2 = nativeArray2[m];
						if ((creationDefinition2.m_Flags & CreationFlags.Permanent) == 0 && m_PrefabGeometryData.HasComponent(creationDefinition2.m_Prefab))
						{
							DynamicBuffer<Game.Areas.Node> val3 = bufferAccessor2[m];
							if (val3.Length != 0)
							{
								AreaGeometryData areaGeometryData2 = m_PrefabGeometryData[creationDefinition2.m_Prefab];
								Game.Areas.Node node = val3[val3.Length - 1];
								m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_HighPriorityColor, node.m_Position, areaGeometryData2.m_SnapDistance * 0.5f);
							}
						}
					}
				}
				DrawAngles();
				break;
			}
			case AreaToolSystem.State.Modify:
			case AreaToolSystem.State.Remove:
			{
				PrefabRef prefabRef = default(PrefabRef);
				Owner owner = default(Owner);
				LotData lotData = default(LotData);
				Transform transform = default(Transform);
				for (int i = 0; i < m_MoveStartPositions.Length; i++)
				{
					ControlPoint controlPoint = m_MoveStartPositions[i];
					if (m_PrefabRefData.TryGetComponent(controlPoint.m_OriginalEntity, ref prefabRef) && m_OwnerData.TryGetComponent(controlPoint.m_OriginalEntity, ref owner) && m_PrefabLotData.TryGetComponent(prefabRef.m_Prefab, ref lotData) && m_TransformData.TryGetComponent(owner.m_Owner, ref transform) && lotData.m_MaxRadius > 0f)
					{
						DrawAreaRange(m_OverlayBuffer, transform.m_Rotation, transform.m_Position, lotData);
					}
				}
				if (m_MoveStartPositions.Length != 0)
				{
					ControlPoint other = m_MoveStartPositions[0];
					if (m_Nodes.HasBuffer(other.m_OriginalEntity) && math.any(other.m_ElementIndex >= 0))
					{
						PrefabRef prefabRef2 = m_PrefabRefData[other.m_OriginalEntity];
						AreaGeometryData areaGeometryData = m_PrefabGeometryData[prefabRef2.m_Prefab];
						ControlPoint controlPoint2 = m_ControlPoints[0];
						if (controlPoint2.Equals(default(ControlPoint)) || controlPoint2.Equals(other))
						{
							m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_HighPriorityColor, other.m_Position, areaGeometryData.m_SnapDistance * 0.5f);
						}
						else
						{
							m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_HighPriorityColor, controlPoint2.m_Position, areaGeometryData.m_SnapDistance * 0.5f);
						}
					}
				}
				DrawAngles();
				break;
			}
			}
		}

		private void DrawAngles()
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_0578: Unknown result type (might be due to invalid IL or missing references)
			//IL_057d: Unknown result type (might be due to invalid IL or missing references)
			//IL_057f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_0596: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05df: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_061b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0620: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_063a: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_0645: Unknown result type (might be due to invalid IL or missing references)
			//IL_0647: Unknown result type (might be due to invalid IL or missing references)
			//IL_0671: Unknown result type (might be due to invalid IL or missing references)
			//IL_0673: Unknown result type (might be due to invalid IL or missing references)
			//IL_0678: Unknown result type (might be due to invalid IL or missing references)
			//IL_067a: Unknown result type (might be due to invalid IL or missing references)
			//IL_068c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0691: Unknown result type (might be due to invalid IL or missing references)
			//IL_069b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06db: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
			int num;
			switch (m_State)
			{
			default:
				return;
			case AreaToolSystem.State.Create:
				num = math.select(0, 1, m_ControlPoints.Length >= 2);
				break;
			case AreaToolSystem.State.Modify:
				num = m_MoveStartPositions.Length * 2;
				break;
			}
			if (!m_PrefabGeometryData.HasComponent(m_Prefab))
			{
				return;
			}
			float num2 = math.min(m_PrefabGeometryData[m_Prefab].m_SnapDistance, 16f);
			float num3 = num2 * 0.125f;
			float num4 = num2 * 4f;
			Segment val = default(Segment);
			Segment line = default(Segment);
			Segment val5 = default(Segment);
			Segment val6 = default(Segment);
			for (int i = 0; i < num; i++)
			{
				float2 leftDir = default(float2);
				float2 rightDir = default(float2);
				float2 leftDir2 = default(float2);
				float2 rightDir2 = default(float2);
				int bestLeft = 181;
				int bestRight = 181;
				int bestLeft2 = 181;
				int bestRight2 = 181;
				float num5;
				float2 val2;
				if (m_State == AreaToolSystem.State.Create)
				{
					((Segment)(ref val))._002Ector(m_ControlPoints[m_ControlPoints.Length - 2].m_Position, m_ControlPoints[m_ControlPoints.Length - 1].m_Position);
					num5 = MathUtils.Length(((Segment)(ref val)).xz);
					val2 = (((float3)(ref val.b)).xz - ((float3)(ref val.a)).xz) / num5;
					if (m_ControlPoints.Length >= 3)
					{
						ControlPoint controlPoint = m_ControlPoints[m_ControlPoints.Length - 3];
						ControlPoint controlPoint2 = m_ControlPoints[m_ControlPoints.Length - 2];
						float2 checkDir = math.normalizesafe(((float3)(ref controlPoint.m_Position)).xz - ((float3)(ref controlPoint2.m_Position)).xz, default(float2));
						if (!((float2)(ref checkDir)).Equals(default(float2)))
						{
							CheckDirection(val2, checkDir, ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
						}
					}
					if (bestLeft > 180 && bestRight > 180)
					{
						ControlPoint controlPoint3 = m_ControlPoints[m_ControlPoints.Length - 2];
						if (!((float2)(ref controlPoint3.m_Direction)).Equals(default(float2)))
						{
							CheckDirection(val2, controlPoint3.m_Direction, ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
							CheckDirection(val2, MathUtils.Right(controlPoint3.m_Direction), ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
							CheckDirection(val2, -controlPoint3.m_Direction, ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
							CheckDirection(val2, MathUtils.Left(controlPoint3.m_Direction), ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
						}
					}
				}
				else
				{
					ControlPoint controlPoint4 = m_MoveStartPositions[i >> 1];
					if (!m_Nodes.HasBuffer(controlPoint4.m_OriginalEntity) || !math.any(controlPoint4.m_ElementIndex >= 0))
					{
						continue;
					}
					DynamicBuffer<Game.Areas.Node> val3 = m_Nodes[controlPoint4.m_OriginalEntity];
					int2 val4;
					if ((i & 1) == 0)
					{
						val4 = math.select(controlPoint4.m_ElementIndex.x + new int2(-1, -2), controlPoint4.m_ElementIndex.y + new int2(0, -1), controlPoint4.m_ElementIndex.y >= 0);
						val4 = math.select(val4, val4 + val3.Length, val4 < 0);
					}
					else
					{
						val4 = math.select(controlPoint4.m_ElementIndex.x + new int2(1, 2), controlPoint4.m_ElementIndex.y + new int2(1, 2), controlPoint4.m_ElementIndex.y >= 0);
						val4 = math.select(val4, val4 - val3.Length, val4 >= val3.Length);
					}
					float3 position = m_ControlPoints[0].m_Position;
					float3 position2 = val3[val4.x].m_Position;
					float3 position3 = val3[val4.y].m_Position;
					((Segment)(ref val))._002Ector(position2, position);
					num5 = MathUtils.Length(((Segment)(ref val)).xz);
					val2 = (((float3)(ref val.b)).xz - ((float3)(ref val.a)).xz) / num5;
					float2 checkDir2 = math.normalizesafe(((float3)(ref position3)).xz - ((float3)(ref position2)).xz, default(float2));
					if (!((float2)(ref checkDir2)).Equals(default(float2)))
					{
						CheckDirection(val2, checkDir2, ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
					}
				}
				bool flag = bestRight < bestLeft;
				if (bestLeft == bestRight && m_AngleSides.Length > i)
				{
					flag = m_AngleSides[i];
				}
				if (bestLeft == 180 && bestRight == 180)
				{
					if (flag)
					{
						bestLeft = 181;
					}
					else
					{
						bestRight = 181;
					}
				}
				else
				{
					if (bestLeft2 <= 180 && bestRight2 <= 180)
					{
						if (bestLeft2 < bestRight2 || (bestLeft2 == bestRight2 && flag))
						{
							bestRight2 = 181;
						}
						else
						{
							bestLeft2 = 181;
						}
					}
					if (bestLeft2 <= 180)
					{
						leftDir = leftDir2;
						bestLeft = bestLeft2;
					}
					else if (bestRight2 <= 180)
					{
						rightDir = rightDir2;
						bestRight = bestRight2;
					}
				}
				if (bestLeft <= 180 || bestRight <= 180)
				{
					((Segment)(ref line))._002Ector(val.a, val.a);
					ref float3 a = ref line.a;
					((float3)(ref a)).xz = ((float3)(ref a)).xz + val2 * math.min(num5, num4);
					m_OverlayBuffer.DrawLine(m_GuideLineSettingsData.m_HighPriorityColor, line, num3);
				}
				if (bestLeft <= 180)
				{
					((Segment)(ref val5))._002Ector(val.a, val.a);
					ref float3 a2 = ref val5.a;
					((float3)(ref a2)).xz = ((float3)(ref a2)).xz + leftDir * math.min(num5, num4);
					m_OverlayBuffer.DrawLine(m_GuideLineSettingsData.m_HighPriorityColor, val5, num3);
					DrawAngleIndicator(m_OverlayBuffer, m_Tooltips, m_GuideLineSettingsData, val5, val, -leftDir, -val2, math.min(num5, num4) * 0.5f, num3, bestLeft, angleSide: false);
				}
				if (bestRight <= 180)
				{
					((Segment)(ref val6))._002Ector(val.a, val.a);
					ref float3 a3 = ref val6.a;
					((float3)(ref a3)).xz = ((float3)(ref a3)).xz + rightDir * math.min(num5, num4);
					m_OverlayBuffer.DrawLine(m_GuideLineSettingsData.m_HighPriorityColor, val6, num3);
					DrawAngleIndicator(m_OverlayBuffer, m_Tooltips, m_GuideLineSettingsData, val6, val, -rightDir, -val2, math.min(num5, num4) * 0.5f, num3, bestRight, angleSide: true);
				}
				if (m_AngleSides.Length > i)
				{
					m_AngleSides[i] = flag;
					continue;
				}
				while (m_AngleSides.Length <= i)
				{
					m_AngleSides.Add(ref flag);
				}
			}
		}
	}

	[BurstCompile]
	private struct SelectionToolGuideLinesJob : IJob
	{
		[ReadOnly]
		public SelectionToolSystem.State m_State;

		[ReadOnly]
		public SelectionType m_SelectionType;

		[ReadOnly]
		public bool m_SelectionQuadIsValid;

		[ReadOnly]
		public Quad3 m_SelectionQuad;

		[ReadOnly]
		public GuideLineSettingsData m_GuideLineSettingsData;

		public OverlayRenderSystem.Buffer m_OverlayBuffer;

		public void Execute()
		{
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			SelectionToolSystem.State state = m_State;
			if ((uint)(state - 1) <= 1u && m_SelectionQuadIsValid)
			{
				float num = m_SelectionType switch
				{
					SelectionType.ServiceDistrict => AreaUtils.GetMinNodeDistance(Game.Areas.AreaType.District) * 2f, 
					SelectionType.MapTiles => AreaUtils.GetMinNodeDistance(Game.Areas.AreaType.MapTile) * 2f, 
					_ => 16f, 
				};
				float width = num * 0.125f;
				float dashLength = num * 0.7f;
				float gapLength = num * 0.3f;
				m_OverlayBuffer.DrawDashedLine(m_GuideLineSettingsData.m_HighPriorityColor, ((Quad3)(ref m_SelectionQuad)).ab, width, dashLength, gapLength, float2.op_Implicit(1f));
				m_OverlayBuffer.DrawDashedLine(m_GuideLineSettingsData.m_HighPriorityColor, ((Quad3)(ref m_SelectionQuad)).ad, width, dashLength, gapLength, float2.op_Implicit(1f));
				m_OverlayBuffer.DrawDashedLine(m_GuideLineSettingsData.m_HighPriorityColor, ((Quad3)(ref m_SelectionQuad)).bc, width, dashLength, gapLength, float2.op_Implicit(1f));
				m_OverlayBuffer.DrawDashedLine(m_GuideLineSettingsData.m_HighPriorityColor, ((Quad3)(ref m_SelectionQuad)).dc, width, dashLength, gapLength, float2.op_Implicit(1f));
			}
		}
	}

	[BurstCompile]
	private struct ZoneToolGuideLinesJob : IJob
	{
		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> m_CreationDefinitionType;

		[ReadOnly]
		public ComponentTypeHandle<Zoning> m_ZoningType;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_DefinitionChunks;

		[ReadOnly]
		public GuideLineSettingsData m_GuideLineSettingsData;

		public OverlayRenderSystem.Buffer m_OverlayBuffer;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_DefinitionChunks.Length; i++)
			{
				ArchetypeChunk val = m_DefinitionChunks[i];
				NativeArray<CreationDefinition> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
				NativeArray<Zoning> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Zoning>(ref m_ZoningType);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					CreationDefinition creationDefinition = nativeArray[j];
					Zoning zoning = nativeArray2[j];
					if ((creationDefinition.m_Flags & CreationFlags.Permanent) == 0 && (zoning.m_Flags & ZoningFlags.Marquee) != 0)
					{
						float3 a = zoning.m_Position.a;
						float3 b = zoning.m_Position.b;
						float3 c = zoning.m_Position.c;
						float3 d = zoning.m_Position.d;
						float width = 1f;
						float dashLength = 5.6f;
						float gapLength = 2.4f;
						m_OverlayBuffer.DrawDashedLine(m_GuideLineSettingsData.m_HighPriorityColor, new Segment(a, b), width, dashLength, gapLength, float2.op_Implicit(1f));
						m_OverlayBuffer.DrawDashedLine(m_GuideLineSettingsData.m_HighPriorityColor, new Segment(b, c), width, dashLength, gapLength, float2.op_Implicit(1f));
						m_OverlayBuffer.DrawDashedLine(m_GuideLineSettingsData.m_HighPriorityColor, new Segment(c, d), width, dashLength, gapLength, float2.op_Implicit(1f));
						m_OverlayBuffer.DrawDashedLine(m_GuideLineSettingsData.m_HighPriorityColor, new Segment(d, a), width, dashLength, gapLength, float2.op_Implicit(1f));
					}
				}
			}
		}
	}

	[BurstCompile]
	private struct RouteToolGuideLinesJob : IJob
	{
		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> m_CreationDefinitionType;

		[ReadOnly]
		public BufferTypeHandle<WaypointDefinition> m_WaypointDefinitionType;

		[ReadOnly]
		public ComponentLookup<Route> m_RouteData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<RouteData> m_PrefabRouteData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_DefinitionChunks;

		[ReadOnly]
		public NativeList<ControlPoint> m_ControlPoints;

		[ReadOnly]
		public ControlPoint m_MoveStartPosition;

		[ReadOnly]
		public RouteToolSystem.State m_State;

		[ReadOnly]
		public GuideLineSettingsData m_GuideLineSettingsData;

		public OverlayRenderSystem.Buffer m_OverlayBuffer;

		public void Execute()
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			if (m_ControlPoints.Length <= 0)
			{
				return;
			}
			switch (m_State)
			{
			case RouteToolSystem.State.Default:
			{
				ControlPoint controlPoint2 = m_ControlPoints[0];
				if (m_RouteData.HasComponent(controlPoint2.m_OriginalEntity) && math.any(controlPoint2.m_ElementIndex >= 0))
				{
					PrefabRef prefabRef2 = m_PrefabRefData[controlPoint2.m_OriginalEntity];
					RouteData routeData3 = m_PrefabRouteData[prefabRef2.m_Prefab];
					if (controlPoint2.m_ElementIndex.x >= 0)
					{
						m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_HighPriorityColor, controlPoint2.m_Position, routeData3.m_Width * 1.7777778f);
					}
					else
					{
						m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_HighPriorityColor, controlPoint2.m_Position, routeData3.m_Width * (8f / 9f));
					}
					break;
				}
				for (int k = 0; k < m_DefinitionChunks.Length; k++)
				{
					ArchetypeChunk val3 = m_DefinitionChunks[k];
					NativeArray<CreationDefinition> nativeArray2 = ((ArchetypeChunk)(ref val3)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
					BufferAccessor<WaypointDefinition> bufferAccessor2 = ((ArchetypeChunk)(ref val3)).GetBufferAccessor<WaypointDefinition>(ref m_WaypointDefinitionType);
					for (int l = 0; l < bufferAccessor2.Length; l++)
					{
						CreationDefinition creationDefinition2 = nativeArray2[l];
						if ((creationDefinition2.m_Flags & CreationFlags.Permanent) == 0 && m_PrefabRouteData.HasComponent(creationDefinition2.m_Prefab))
						{
							DynamicBuffer<WaypointDefinition> val4 = bufferAccessor2[l];
							if (val4.Length != 0)
							{
								RouteData routeData4 = m_PrefabRouteData[creationDefinition2.m_Prefab];
								WaypointDefinition waypointDefinition2 = val4[0];
								m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_HighPriorityColor, waypointDefinition2.m_Position, routeData4.m_Width * 1.7777778f);
							}
						}
					}
				}
				break;
			}
			case RouteToolSystem.State.Create:
			{
				for (int i = 0; i < m_DefinitionChunks.Length; i++)
				{
					ArchetypeChunk val = m_DefinitionChunks[i];
					NativeArray<CreationDefinition> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
					BufferAccessor<WaypointDefinition> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<WaypointDefinition>(ref m_WaypointDefinitionType);
					for (int j = 0; j < bufferAccessor.Length; j++)
					{
						CreationDefinition creationDefinition = nativeArray[j];
						if ((creationDefinition.m_Flags & CreationFlags.Permanent) == 0 && m_PrefabRouteData.HasComponent(creationDefinition.m_Prefab))
						{
							DynamicBuffer<WaypointDefinition> val2 = bufferAccessor[j];
							if (val2.Length != 0)
							{
								RouteData routeData2 = m_PrefabRouteData[creationDefinition.m_Prefab];
								WaypointDefinition waypointDefinition = val2[val2.Length - 1];
								m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_HighPriorityColor, waypointDefinition.m_Position, routeData2.m_Width * 1.7777778f);
							}
						}
					}
				}
				break;
			}
			case RouteToolSystem.State.Modify:
			case RouteToolSystem.State.Remove:
			{
				if (!m_RouteData.HasComponent(m_MoveStartPosition.m_OriginalEntity) || !math.any(m_MoveStartPosition.m_ElementIndex >= 0))
				{
					break;
				}
				PrefabRef prefabRef = m_PrefabRefData[m_MoveStartPosition.m_OriginalEntity];
				RouteData routeData = m_PrefabRouteData[prefabRef.m_Prefab];
				ControlPoint controlPoint = m_ControlPoints[m_ControlPoints.Length - 1];
				if (controlPoint.Equals(default(ControlPoint)) || controlPoint.Equals(m_MoveStartPosition))
				{
					if (m_MoveStartPosition.m_ElementIndex.x >= 0)
					{
						m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_HighPriorityColor, m_MoveStartPosition.m_Position, routeData.m_Width * 1.7777778f);
					}
					else
					{
						m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_HighPriorityColor, m_MoveStartPosition.m_Position, routeData.m_Width * (8f / 9f));
					}
				}
				else
				{
					m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_HighPriorityColor, controlPoint.m_Position, routeData.m_Width * 1.7777778f);
				}
				break;
			}
			}
		}
	}

	[BurstCompile]
	private struct NetToolGuideLinesJob : IJob
	{
		private struct SnapDir
		{
			public float2 m_Direction;

			public float2 m_Height;

			public float2 m_Factor;
		}

		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> m_CreationDefinitionType;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<NetCourse> m_NetCourseType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.Node> m_NodeType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<ConnectedEdge> m_ConnectedEdgeType;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_NetGeometryData;

		[ReadOnly]
		public ComponentLookup<PlaceableNetData> m_PlaceableNetData;

		[ReadOnly]
		public ComponentLookup<RoadData> m_RoadData;

		[ReadOnly]
		public ComponentLookup<ElectricityConnectionData> m_ElectricityConnectionData;

		[ReadOnly]
		public ComponentLookup<WaterPipeConnectionData> m_WaterPipeConnectionData;

		[ReadOnly]
		public ComponentLookup<ResourceConnectionData> m_ResourceConnectionData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<ConnectedNode> m_ConnectedNodes;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_DefinitionChunks;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_MarkerNodeChunks;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_TempNodeChunks;

		[ReadOnly]
		public NativeList<ControlPoint> m_ControlPoints;

		[ReadOnly]
		public NativeList<SnapLine> m_SnapLines;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[ReadOnly]
		public NetToolSystem.Mode m_Mode;

		[ReadOnly]
		public Game.Prefabs.ElectricityConnection.Voltage m_HighlightVoltage;

		[ReadOnly]
		public bool2 m_HighlightWater;

		[ReadOnly]
		public bool m_HighResourceLine;

		[ReadOnly]
		public Entity m_Prefab;

		[ReadOnly]
		public GuideLineSettingsData m_GuideLineSettingsData;

		public NativeList<bool> m_AngleSides;

		public NativeList<TooltipInfo> m_Tooltips;

		public OverlayRenderSystem.Buffer m_OverlayBuffer;

		public void Execute()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			DrawZones();
			DrawCourses();
			if (m_Mode != NetToolSystem.Mode.Replace)
			{
				DrawSnapLines();
				DrawControlPoints();
				DrawNodeConnections(out var lastNodePosition, out var lastNodeWidth);
				DrawMarkers(lastNodePosition, lastNodeWidth);
			}
		}

		private void DrawNodeConnections(out float3 lastNodePosition, out float lastNodeWidth)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			lastNodePosition = float3.op_Implicit(float.MinValue);
			lastNodeWidth = 0f;
			if (!m_TempNodeChunks.IsCreated)
			{
				return;
			}
			NetGeometryData netGeometryData = default(NetGeometryData);
			PrefabRef prefabRef2 = default(PrefabRef);
			Temp temp2 = default(Temp);
			bool2 val4 = default(bool2);
			DynamicBuffer<ConnectedNode> val5 = default(DynamicBuffer<ConnectedNode>);
			Temp temp3 = default(Temp);
			for (int i = 0; i < m_TempNodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_TempNodeChunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<Game.Net.Node> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Net.Node>(ref m_NodeType);
				NativeArray<Temp> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<Temp>(ref m_TempType);
				NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BufferAccessor<ConnectedEdge> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<ConnectedEdge>(ref m_ConnectedEdgeType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val2 = nativeArray[j];
					Game.Net.Node node = nativeArray2[j];
					Temp temp = nativeArray3[j];
					PrefabRef prefabRef = nativeArray4[j];
					DynamicBuffer<ConnectedEdge> val3 = bufferAccessor[j];
					if ((temp.m_Flags & TempFlags.IsLast) == 0 || !m_NetGeometryData.TryGetComponent(prefabRef.m_Prefab, ref netGeometryData))
					{
						continue;
					}
					Color positiveFeedbackColor = m_GuideLineSettingsData.m_PositiveFeedbackColor;
					positiveFeedbackColor.a *= 0.1f;
					float num = netGeometryData.m_DefaultWidth + 12f;
					float outlineWidth = (math.sqrt(num + 1f) - 1f) * 0.2f;
					if (m_PrefabRefData.TryGetComponent(temp.m_Original, ref prefabRef2))
					{
						if (!CheckConnectionType(prefabRef2))
						{
							continue;
						}
						if (m_ControlPoints.Length == 2)
						{
							bool flag = false;
							for (int k = 0; k < val3.Length; k++)
							{
								if (m_TempData.TryGetComponent(val3[k].m_Edge, ref temp2) && (temp2.m_Flags & TempFlags.Essential) != 0)
								{
									flag = true;
								}
							}
							if (!flag)
							{
								continue;
							}
						}
						m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_PositiveFeedbackColor, positiveFeedbackColor, outlineWidth, (OverlayRenderSystem.StyleFlags)0, new float2(0f, 1f), node.m_Position, num);
						continue;
					}
					if (val3.Length == 0)
					{
						lastNodePosition = node.m_Position;
						lastNodeWidth = netGeometryData.m_DefaultWidth;
					}
					for (int l = 0; l < val3.Length; l++)
					{
						Edge edge = m_EdgeData[val3[l].m_Edge];
						((bool2)(ref val4))._002Ector(edge.m_Start == val2, edge.m_End == val2);
						if (!m_ConnectedNodes.TryGetBuffer(val3[l].m_Edge, ref val5))
						{
							continue;
						}
						for (int m = 0; m < val5.Length; m++)
						{
							ConnectedNode connectedNode = val5[m];
							float3 position;
							if (math.any(val4))
							{
								if ((val4.x ? (connectedNode.m_CurvePosition > 0.5f) : (connectedNode.m_CurvePosition < 0.5f)) || !m_TempData.TryGetComponent(val3[l].m_Edge, ref temp3) || (temp3.m_Flags & TempFlags.Essential) == 0)
								{
									continue;
								}
								position = m_NodeData[connectedNode.m_Node].m_Position;
								prefabRef2 = m_PrefabRefData[connectedNode.m_Node];
							}
							else
							{
								if (!(connectedNode.m_Node == val2))
								{
									continue;
								}
								position = MathUtils.Position(m_CurveData[val3[l].m_Edge].m_Bezier, connectedNode.m_CurvePosition);
								prefabRef2 = m_PrefabRefData[val3[l].m_Edge];
							}
							if (CheckConnectionType(prefabRef2))
							{
								m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_PositiveFeedbackColor, positiveFeedbackColor, outlineWidth, (OverlayRenderSystem.StyleFlags)0, new float2(0f, 1f), position, num);
							}
						}
					}
				}
			}
		}

		private bool CheckConnectionType(PrefabRef prefabRef)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			ResourceConnectionData resourceConnectionData = default(ResourceConnectionData);
			if (m_HighlightVoltage != Game.Prefabs.ElectricityConnection.Voltage.Invalid)
			{
				ElectricityConnectionData electricityConnectionData = default(ElectricityConnectionData);
				if (!m_ElectricityConnectionData.TryGetComponent(prefabRef.m_Prefab, ref electricityConnectionData))
				{
					return false;
				}
				if (electricityConnectionData.m_Voltage != m_HighlightVoltage)
				{
					return false;
				}
			}
			else if (math.any(m_HighlightWater))
			{
				WaterPipeConnectionData waterPipeConnectionData = default(WaterPipeConnectionData);
				if (!m_WaterPipeConnectionData.TryGetComponent(prefabRef.m_Prefab, ref waterPipeConnectionData))
				{
					return false;
				}
				if (!math.any((new int2(waterPipeConnectionData.m_FreshCapacity, waterPipeConnectionData.m_SewageCapacity) > 0) & m_HighlightWater))
				{
					return false;
				}
			}
			else if (m_HighResourceLine && !m_ResourceConnectionData.TryGetComponent(prefabRef.m_Prefab, ref resourceConnectionData))
			{
				return false;
			}
			return true;
		}

		private void DrawMarkers(float3 lastNodePosition, float lastNodeWidth)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			if (!m_MarkerNodeChunks.IsCreated)
			{
				return;
			}
			NetGeometryData netGeometryData = default(NetGeometryData);
			for (int i = 0; i < m_MarkerNodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_MarkerNodeChunks[i];
				NativeArray<Game.Net.Node> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Net.Node>(ref m_NodeType);
				NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BufferAccessor<ConnectedEdge> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<ConnectedEdge>(ref m_ConnectedEdgeType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Game.Net.Node node = nativeArray[j];
					PrefabRef prefabRef = nativeArray2[j];
					if (bufferAccessor[j].Length == 0 && CheckConnectionType(prefabRef))
					{
						float2 xz = ((float3)(ref node.m_Position)).xz;
						if (((float2)(ref xz)).Equals(((float3)(ref lastNodePosition)).xz))
						{
							Color positiveFeedbackColor = m_GuideLineSettingsData.m_PositiveFeedbackColor;
							positiveFeedbackColor.a *= 0.1f;
							float num = lastNodeWidth + 12f;
							float outlineWidth = (math.sqrt(num + 1f) - 1f) * 0.2f;
							m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_PositiveFeedbackColor, positiveFeedbackColor, outlineWidth, (OverlayRenderSystem.StyleFlags)0, new float2(0f, 1f), node.m_Position, num);
						}
						else if (m_NetGeometryData.TryGetComponent(prefabRef.m_Prefab, ref netGeometryData))
						{
							Color mediumPriorityColor = m_GuideLineSettingsData.m_MediumPriorityColor;
							mediumPriorityColor.a *= 0.1f;
							float defaultWidth = netGeometryData.m_DefaultWidth;
							float outlineWidth2 = (math.sqrt(defaultWidth + 1f) - 1f) * 0.3f;
							m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_MediumPriorityColor, mediumPriorityColor, outlineWidth2, (OverlayRenderSystem.StyleFlags)0, new float2(0f, 1f), node.m_Position, defaultWidth);
						}
					}
				}
			}
		}

		private void DrawCourses()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			NetGeometryData netGeometryData = default(NetGeometryData);
			for (int i = 0; i < m_DefinitionChunks.Length; i++)
			{
				ArchetypeChunk val = m_DefinitionChunks[i];
				NativeArray<CreationDefinition> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
				NativeArray<NetCourse> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<NetCourse>(ref m_NetCourseType);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					NetCourse netCourse = nativeArray2[j];
					CreationDefinition creationDefinition = nativeArray[j];
					if ((creationDefinition.m_Flags & CreationFlags.Permanent) == 0 && m_NetGeometryData.TryGetComponent(creationDefinition.m_Prefab, ref netGeometryData))
					{
						DrawNetCourse(m_OverlayBuffer, netCourse, ref m_TerrainHeightData, ref m_WaterSurfaceData, netGeometryData, m_GuideLineSettingsData);
					}
				}
			}
		}

		private void DrawZones()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			float2 val5 = default(float2);
			for (int i = 0; i < m_DefinitionChunks.Length; i++)
			{
				ArchetypeChunk val = m_DefinitionChunks[i];
				NativeArray<CreationDefinition> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
				NativeArray<NetCourse> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<NetCourse>(ref m_NetCourseType);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					NetCourse netCourse = nativeArray2[j];
					CreationDefinition creationDefinition = nativeArray[j];
					if ((creationDefinition.m_Flags & (CreationFlags.Permanent | CreationFlags.Upgrade)) != 0 || !m_RoadData.HasComponent(creationDefinition.m_Prefab) || !m_NetGeometryData.HasComponent(creationDefinition.m_Prefab))
					{
						continue;
					}
					NetGeometryData netGeometryData = m_NetGeometryData[creationDefinition.m_Prefab];
					if (m_RoadData[creationDefinition.m_Prefab].m_ZoneBlockPrefab == Entity.Null)
					{
						continue;
					}
					float2 val2 = math.max(float2.op_Implicit(math.max(math.cmin(netCourse.m_StartPosition.m_Elevation), math.cmin(netCourse.m_EndPosition.m_Elevation))), netCourse.m_Elevation);
					float2 val3 = math.min(float2.op_Implicit(math.min(math.cmax(netCourse.m_StartPosition.m_Elevation), math.cmax(netCourse.m_EndPosition.m_Elevation))), netCourse.m_Elevation);
					bool2 val4 = (val2 < netGeometryData.m_ElevationLimit) & (val3 > 0f - netGeometryData.m_ElevationLimit);
					val4.x &= (netCourse.m_StartPosition.m_Flags & CoursePosFlags.IsLeft) != 0;
					val4.y &= (netCourse.m_StartPosition.m_Flags & CoursePosFlags.IsRight) != 0;
					if (!math.any(val4))
					{
						continue;
					}
					float num = ((float)ZoneUtils.GetCellWidth(netGeometryData.m_DefaultWidth) * 0.5f + 6f) * 8f - 1f;
					bool num2 = (netCourse.m_StartPosition.m_Flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsLast)) != 0;
					bool flag = (netCourse.m_EndPosition.m_Flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsLast)) != 0;
					bool flag2 = netCourse.m_Length > 0.1f;
					if (num2)
					{
						if ((netCourse.m_StartPosition.m_Flags & CoursePosFlags.IsGrid) != 0)
						{
							if ((netCourse.m_StartPosition.m_Flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsParallel)) == CoursePosFlags.IsFirst || (netCourse.m_StartPosition.m_Flags & (CoursePosFlags.IsLast | CoursePosFlags.IsParallel)) == (CoursePosFlags.IsLast | CoursePosFlags.IsParallel))
							{
								DrawZoneCircle(netCourse.m_StartPosition, num, start: true, val4.x, val4.y);
							}
						}
						else
						{
							DrawZoneCircle(netCourse.m_StartPosition, num, fullStart: true, !flag2, val4.x, val4.y);
						}
					}
					if (!flag2)
					{
						continue;
					}
					if (flag)
					{
						if ((netCourse.m_EndPosition.m_Flags & CoursePosFlags.IsGrid) != 0)
						{
							if ((netCourse.m_EndPosition.m_Flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsParallel)) == CoursePosFlags.IsFirst || (netCourse.m_EndPosition.m_Flags & (CoursePosFlags.IsLast | CoursePosFlags.IsParallel)) == (CoursePosFlags.IsLast | CoursePosFlags.IsParallel))
							{
								DrawZoneCircle(netCourse.m_EndPosition, num, start: false, val4.x, val4.y);
							}
						}
						else
						{
							DrawZoneCircle(netCourse.m_EndPosition, num, fullStart: false, fullEnd: true, val4.x, val4.y);
						}
					}
					((float2)(ref val5))._002Ector(netCourse.m_StartPosition.m_CourseDelta, netCourse.m_EndPosition.m_CourseDelta);
					Bezier4x3 source = MathUtils.Cut(netCourse.m_Curve, new float2(val5.x, math.lerp(val5.x, val5.y, 0.5f)));
					Bezier4x3 source2 = MathUtils.Cut(netCourse.m_Curve, new float2(math.lerp(val5.x, val5.y, 0.5f), val5.y));
					if (val4.x)
					{
						if (GetOffsetCurve(source, num, out var result))
						{
							m_OverlayBuffer.DrawCurve(m_GuideLineSettingsData.m_LowPriorityColor, m_GuideLineSettingsData.m_LowPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, result, 2f);
						}
						if (GetOffsetCurve(source2, num, out var result2))
						{
							m_OverlayBuffer.DrawCurve(m_GuideLineSettingsData.m_LowPriorityColor, m_GuideLineSettingsData.m_LowPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, result2, 2f);
						}
					}
					if (val4.y)
					{
						if (GetOffsetCurve(source, 0f - num, out var result3))
						{
							m_OverlayBuffer.DrawCurve(m_GuideLineSettingsData.m_LowPriorityColor, m_GuideLineSettingsData.m_LowPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, result3, 2f);
						}
						if (GetOffsetCurve(source2, 0f - num, out var result4))
						{
							m_OverlayBuffer.DrawCurve(m_GuideLineSettingsData.m_LowPriorityColor, m_GuideLineSettingsData.m_LowPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, result4, 2f);
						}
					}
				}
			}
		}

		private void DrawZoneCircle(CoursePos coursePos, float offset, bool start, bool left, bool right)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			Bezier4x3 curve = NetUtils.CircleCurve(coursePos.m_Position, coursePos.m_Rotation, 0f - offset, 0f - offset);
			Bezier4x3 curve2 = NetUtils.CircleCurve(coursePos.m_Position, coursePos.m_Rotation, offset, 0f - offset);
			Bezier4x3 curve3 = NetUtils.CircleCurve(coursePos.m_Position, coursePos.m_Rotation, offset, offset);
			Bezier4x3 curve4 = NetUtils.CircleCurve(coursePos.m_Position, coursePos.m_Rotation, 0f - offset, offset);
			if (start)
			{
				if (left)
				{
					m_OverlayBuffer.DrawCurve(m_GuideLineSettingsData.m_LowPriorityColor, m_GuideLineSettingsData.m_LowPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, curve, 2f);
				}
				if (right)
				{
					m_OverlayBuffer.DrawCurve(m_GuideLineSettingsData.m_LowPriorityColor, m_GuideLineSettingsData.m_LowPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, curve2, 2f);
				}
			}
			else
			{
				if (right)
				{
					m_OverlayBuffer.DrawCurve(m_GuideLineSettingsData.m_LowPriorityColor, m_GuideLineSettingsData.m_LowPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, curve3, 2f);
				}
				if (left)
				{
					m_OverlayBuffer.DrawCurve(m_GuideLineSettingsData.m_LowPriorityColor, m_GuideLineSettingsData.m_LowPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, curve4, 2f);
				}
			}
		}

		private void DrawZoneCircle(CoursePos coursePos, float offset, bool fullStart, bool fullEnd, bool left, bool right)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			Bezier4x3 val = NetUtils.CircleCurve(coursePos.m_Position, coursePos.m_Rotation, 0f - offset, 0f - offset);
			Bezier4x3 val2 = NetUtils.CircleCurve(coursePos.m_Position, coursePos.m_Rotation, offset, 0f - offset);
			Bezier4x3 val3 = NetUtils.CircleCurve(coursePos.m_Position, coursePos.m_Rotation, offset, offset);
			Bezier4x3 val4 = NetUtils.CircleCurve(coursePos.m_Position, coursePos.m_Rotation, 0f - offset, offset);
			if (fullStart)
			{
				if (left)
				{
					m_OverlayBuffer.DrawCurve(m_GuideLineSettingsData.m_LowPriorityColor, m_GuideLineSettingsData.m_LowPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, val, 2f);
				}
				if (right)
				{
					m_OverlayBuffer.DrawCurve(m_GuideLineSettingsData.m_LowPriorityColor, m_GuideLineSettingsData.m_LowPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, val2, 2f);
				}
			}
			else
			{
				if (left)
				{
					val = MathUtils.Cut(val, new float2(0.25f, 1f));
					m_OverlayBuffer.DrawCurve(m_GuideLineSettingsData.m_VeryLowPriorityColor, m_GuideLineSettingsData.m_VeryLowPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, val, 2f);
				}
				if (right)
				{
					val2 = MathUtils.Cut(val2, new float2(0.25f, 1f));
					m_OverlayBuffer.DrawCurve(m_GuideLineSettingsData.m_VeryLowPriorityColor, m_GuideLineSettingsData.m_VeryLowPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, val2, 2f);
				}
			}
			if (fullEnd)
			{
				if (right)
				{
					m_OverlayBuffer.DrawCurve(m_GuideLineSettingsData.m_LowPriorityColor, m_GuideLineSettingsData.m_LowPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, val3, 2f);
				}
				if (left)
				{
					m_OverlayBuffer.DrawCurve(m_GuideLineSettingsData.m_LowPriorityColor, m_GuideLineSettingsData.m_LowPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, val4, 2f);
				}
				return;
			}
			if (right)
			{
				val3 = MathUtils.Cut(val3, new float2(0.25f, 1f));
				m_OverlayBuffer.DrawCurve(m_GuideLineSettingsData.m_VeryLowPriorityColor, m_GuideLineSettingsData.m_VeryLowPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, val3, 2f);
			}
			if (left)
			{
				val4 = MathUtils.Cut(val4, new float2(0.25f, 1f));
				m_OverlayBuffer.DrawCurve(m_GuideLineSettingsData.m_VeryLowPriorityColor, m_GuideLineSettingsData.m_VeryLowPriorityColor, 0f, OverlayRenderSystem.StyleFlags.Projected, val4, 2f);
			}
		}

		private bool GetOffsetCurve(Bezier4x3 source, float offset, out Bezier4x3 result)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			result = NetUtils.OffsetCurveLeftSmooth(source, float2.op_Implicit(offset));
			return math.dot(((float3)(ref source.d)).xz - ((float3)(ref source.a)).xz, ((float3)(ref result.d)).xz - ((float3)(ref result.a)).xz) > 0f;
		}

		private void DrawSnapLines()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0559: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0579: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_0518: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05db: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0606: Unknown result type (might be due to invalid IL or missing references)
			//IL_060b: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			float num = 1f;
			if (m_PlaceableNetData.HasComponent(m_Prefab))
			{
				num = math.min(m_PlaceableNetData[m_Prefab].m_SnapDistance, 16f);
			}
			float num2 = num * 0.125f;
			float num3 = num * 4f;
			if (m_Mode == NetToolSystem.Mode.Replace || m_ControlPoints.Length < 1)
			{
				return;
			}
			ControlPoint controlPoint = m_ControlPoints[m_ControlPoints.Length - 1];
			NativeList<SnapDir> val = default(NativeList<SnapDir>);
			val._002Ector(4, AllocatorHandle.op_Implicit((Allocator)2));
			bool flag = false;
			for (int i = 0; i < m_SnapLines.Length; i++)
			{
				SnapLine snapLine = m_SnapLines[i];
				if ((snapLine.m_Flags & SnapLineFlags.Hidden) != 0 || !(NetUtils.ExtendedDistance(((Bezier4x3)(ref snapLine.m_Curve)).xz, ((float3)(ref controlPoint.m_Position)).xz, out var t) < 0.1f))
				{
					continue;
				}
				NetUtils.ExtendedPositionAndTangent(snapLine.m_Curve, t, out var position, out var tangent);
				tangent = MathUtils.Normalize(tangent, ((float3)(ref tangent)).xz);
				position.y = controlPoint.m_Position.y;
				float3 val2 = position - snapLine.m_Curve.a;
				SnapDir snapDir = new SnapDir
				{
					m_Direction = ((float3)(ref tangent)).xz,
					m_Height = float2.op_Implicit(MathUtils.Normalize(val2, ((float3)(ref val2)).xz).y)
				};
				if ((snapLine.m_Flags & SnapLineFlags.GuideLine) != 0)
				{
					float num4 = math.dot(((float3)(ref tangent)).xz, ((float3)(ref val2)).xz);
					snapDir.m_Factor.x = math.max(0f, num4);
					snapDir.m_Factor.y = 1000000f;
					flag = true;
				}
				else if ((snapLine.m_Flags & SnapLineFlags.Secondary) != 0)
				{
					snapDir.m_Factor = float2.op_Implicit(1000000f);
				}
				else
				{
					flag = true;
				}
				int num5 = 0;
				while (true)
				{
					if (num5 < val.Length)
					{
						SnapDir snapDir2 = val[num5];
						switch (Mathf.RoundToInt(math.degrees(math.acos(math.clamp(math.dot(snapDir.m_Direction, snapDir2.m_Direction), -1f, 1f)))))
						{
						case 0:
							if (snapDir.m_Factor.x < snapDir2.m_Factor.x)
							{
								snapDir2.m_Factor.x = snapDir.m_Factor.x;
								snapDir2.m_Height.x = snapDir.m_Height.x;
							}
							if (snapDir.m_Factor.y < snapDir2.m_Factor.y)
							{
								snapDir2.m_Factor.y = snapDir.m_Factor.y;
								snapDir2.m_Height.y = snapDir.m_Height.y;
							}
							val[num5] = snapDir2;
							break;
						case 180:
							if (snapDir.m_Factor.y < snapDir2.m_Factor.x)
							{
								snapDir2.m_Factor.x = snapDir.m_Factor.y;
								snapDir2.m_Height.x = 0f - snapDir.m_Height.y;
							}
							if (snapDir.m_Factor.x < snapDir2.m_Factor.y)
							{
								snapDir2.m_Factor.y = snapDir.m_Factor.x;
								snapDir2.m_Height.y = 0f - snapDir.m_Height.x;
							}
							val[num5] = snapDir2;
							break;
						default:
							goto IL_0370;
						}
					}
					else
					{
						val.Add(ref snapDir);
					}
					break;
					IL_0370:
					num5++;
				}
			}
			float3 val3 = default(float3);
			Segment line = default(Segment);
			float3 val5 = default(float3);
			Segment line2 = default(Segment);
			float3 val6 = default(float3);
			Segment line3 = default(Segment);
			for (int j = 0; j < val.Length; j++)
			{
				SnapDir snapDir3 = val[j];
				if (!flag || !math.all(snapDir3.m_Factor == 1000000f))
				{
					((float3)(ref val3))._002Ector(snapDir3.m_Direction.x, 0f, snapDir3.m_Direction.y);
					((Segment)(ref line))._002Ector(controlPoint.m_Position - val3 * num3, controlPoint.m_Position + val3 * num3);
					m_OverlayBuffer.DrawLine(m_GuideLineSettingsData.m_HighPriorityColor, line, num2);
					float2 val4 = math.min(num2 / math.abs(snapDir3.m_Height), float2.op_Implicit(num3));
					val4 = MathUtils.Snap(val4 - snapDir3.m_Factor, float2.op_Implicit(num2 * 4f)) + snapDir3.m_Factor;
					if (snapDir3.m_Factor.x > val4.x + num2 * 3f && snapDir3.m_Factor.x < 1000000f)
					{
						((float3)(ref val5))._002Ector(snapDir3.m_Direction.x, snapDir3.m_Height.x, snapDir3.m_Direction.y);
						((Segment)(ref line2))._002Ector(controlPoint.m_Position - val5 * (snapDir3.m_Factor.x + num2), controlPoint.m_Position - val5 * (val4.x + num2));
						m_OverlayBuffer.DrawDashedLine(m_GuideLineSettingsData.m_HighPriorityColor, line2, num2, num2 * 2f, num2 * 2f);
					}
					if (snapDir3.m_Factor.y > val4.y + num2 * 3f && snapDir3.m_Factor.y < 1000000f)
					{
						((float3)(ref val6))._002Ector(snapDir3.m_Direction.x, snapDir3.m_Height.y, snapDir3.m_Direction.y);
						((Segment)(ref line3))._002Ector(controlPoint.m_Position + val6 * (snapDir3.m_Factor.y + num2), controlPoint.m_Position + val6 * (val4.y + num2));
						m_OverlayBuffer.DrawDashedLine(m_GuideLineSettingsData.m_HighPriorityColor, line3, num2, num2 * 2f, num2 * 2f);
					}
				}
			}
			val.Dispose();
		}

		private void DrawControlPoints()
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0719: Unknown result type (might be due to invalid IL or missing references)
			//IL_0720: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0738: Unknown result type (might be due to invalid IL or missing references)
			//IL_0749: Unknown result type (might be due to invalid IL or missing references)
			//IL_075c: Unknown result type (might be due to invalid IL or missing references)
			//IL_076d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0783: Unknown result type (might be due to invalid IL or missing references)
			//IL_078f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0794: Unknown result type (might be due to invalid IL or missing references)
			//IL_079b: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0801: Unknown result type (might be due to invalid IL or missing references)
			//IL_0803: Unknown result type (might be due to invalid IL or missing references)
			//IL_0807: Unknown result type (might be due to invalid IL or missing references)
			//IL_080c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0811: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07df: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_09af: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0815: Unknown result type (might be due to invalid IL or missing references)
			//IL_081c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0821: Unknown result type (might be due to invalid IL or missing references)
			//IL_0825: Unknown result type (might be due to invalid IL or missing references)
			//IL_082a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0831: Unknown result type (might be due to invalid IL or missing references)
			//IL_0833: Unknown result type (might be due to invalid IL or missing references)
			//IL_0838: Unknown result type (might be due to invalid IL or missing references)
			//IL_083f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0844: Unknown result type (might be due to invalid IL or missing references)
			//IL_0848: Unknown result type (might be due to invalid IL or missing references)
			//IL_084d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0852: Unknown result type (might be due to invalid IL or missing references)
			//IL_0860: Unknown result type (might be due to invalid IL or missing references)
			//IL_086c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0873: Unknown result type (might be due to invalid IL or missing references)
			//IL_0878: Unknown result type (might be due to invalid IL or missing references)
			//IL_088b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0897: Unknown result type (might be due to invalid IL or missing references)
			//IL_089e: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08da: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a36: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a66: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			//IL_0524: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_0537: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_0566: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_0596: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05be: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_060a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_061a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0630: Unknown result type (might be due to invalid IL or missing references)
			//IL_0635: Unknown result type (might be due to invalid IL or missing references)
			//IL_0645: Unknown result type (might be due to invalid IL or missing references)
			//IL_0650: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Unknown result type (might be due to invalid IL or missing references)
			//IL_0654: Unknown result type (might be due to invalid IL or missing references)
			//IL_0656: Unknown result type (might be due to invalid IL or missing references)
			//IL_065b: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			int angleIndex = 0;
			Segment prevLine = default(Segment);
			float3 prevPoint = float3.op_Implicit(-1000000f);
			float num = 1f;
			if (m_PlaceableNetData.HasComponent(m_Prefab))
			{
				num = math.min(m_PlaceableNetData[m_Prefab].m_SnapDistance, 16f);
			}
			float num2 = num * 0.125f;
			float num3 = num * 4f;
			float3 val5;
			if (m_Mode != NetToolSystem.Mode.Replace && m_ControlPoints.Length >= 2)
			{
				Segment val = default(Segment);
				((Segment)(ref val))._002Ector(m_ControlPoints[0].m_Position, m_ControlPoints[1].m_Position);
				float num4 = MathUtils.Length(((Segment)(ref val)).xz);
				if (num4 > num2 * 7f)
				{
					float2 val2 = (((float3)(ref val.b)).xz - ((float3)(ref val.a)).xz) / num4;
					float2 leftDir = default(float2);
					float2 rightDir = default(float2);
					float2 leftDir2 = default(float2);
					float2 rightDir2 = default(float2);
					int bestLeft = 181;
					int bestRight = 181;
					int bestLeft2 = 181;
					int bestRight2 = 181;
					for (int i = 0; i < m_DefinitionChunks.Length; i++)
					{
						ArchetypeChunk val3 = m_DefinitionChunks[i];
						NativeArray<CreationDefinition> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
						NativeArray<NetCourse> nativeArray2 = ((ArchetypeChunk)(ref val3)).GetNativeArray<NetCourse>(ref m_NetCourseType);
						for (int j = 0; j < nativeArray2.Length; j++)
						{
							CreationDefinition creationDefinition = nativeArray[j];
							NetCourse netCourse = nativeArray2[j];
							if ((creationDefinition.m_Flags & CreationFlags.Permanent) != 0 || (netCourse.m_StartPosition.m_Flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsParallel)) != CoursePosFlags.IsFirst)
							{
								continue;
							}
							if (m_ConnectedEdges.HasBuffer(netCourse.m_StartPosition.m_Entity))
							{
								DynamicBuffer<ConnectedEdge> val4 = m_ConnectedEdges[netCourse.m_StartPosition.m_Entity];
								for (int k = 0; k < val4.Length; k++)
								{
									Entity edge = val4[k].m_Edge;
									Edge edge2 = m_EdgeData[edge];
									Curve curve = m_CurveData[edge];
									if (edge2.m_Start == netCourse.m_StartPosition.m_Entity)
									{
										val5 = MathUtils.StartTangent(curve.m_Bezier);
										CheckDirection(val2, ((float3)(ref val5)).xz, ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
									}
									else if (edge2.m_End == netCourse.m_StartPosition.m_Entity)
									{
										val5 = MathUtils.EndTangent(curve.m_Bezier);
										CheckDirection(val2, -((float3)(ref val5)).xz, ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
									}
								}
							}
							else if (m_CurveData.HasComponent(netCourse.m_StartPosition.m_Entity))
							{
								float3 val6 = MathUtils.Tangent(m_CurveData[netCourse.m_StartPosition.m_Entity].m_Bezier, netCourse.m_StartPosition.m_SplitPosition);
								CheckDirection(val2, ((float3)(ref val6)).xz, ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
								CheckDirection(val2, -((float3)(ref val6)).xz, ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
							}
						}
					}
					if (bestLeft > 180 && bestRight > 180)
					{
						float2 val7 = default(float2);
						ControlPoint controlPoint = m_ControlPoints[0];
						if (!((float2)(ref controlPoint.m_Direction)).Equals(default(float2)))
						{
							val7 = m_ControlPoints[0].m_Direction;
						}
						else if (m_TransformData.HasComponent(m_ControlPoints[0].m_OriginalEntity))
						{
							val5 = math.forward(m_TransformData[m_ControlPoints[0].m_OriginalEntity].m_Rotation);
							val7 = ((float3)(ref val5)).xz;
						}
						if (!((float2)(ref val7)).Equals(default(float2)))
						{
							CheckDirection(val2, val7, ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
							CheckDirection(val2, MathUtils.Right(val7), ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
							CheckDirection(val2, -val7, ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
							CheckDirection(val2, MathUtils.Left(val7), ref leftDir, ref rightDir, ref bestLeft, ref bestRight, ref leftDir2, ref rightDir2, ref bestLeft2, ref bestRight2);
						}
					}
					bool flag = bestRight < bestLeft;
					if (bestLeft == bestRight && m_AngleSides.Length > angleIndex)
					{
						flag = m_AngleSides[angleIndex];
					}
					if (bestLeft == 180 && bestRight == 180)
					{
						if (flag)
						{
							bestLeft = 181;
						}
						else
						{
							bestRight = 181;
						}
					}
					else
					{
						if (bestLeft2 <= 180 && bestRight2 <= 180)
						{
							if (bestLeft2 < bestRight2 || (bestLeft2 == bestRight2 && flag))
							{
								bestRight2 = 181;
							}
							else
							{
								bestLeft2 = 181;
							}
						}
						if (bestLeft2 <= 180)
						{
							leftDir = leftDir2;
							bestLeft = bestLeft2;
						}
						else if (bestRight2 <= 180)
						{
							rightDir = rightDir2;
							bestRight = bestRight2;
						}
					}
					if (bestLeft <= 180)
					{
						Segment val8 = default(Segment);
						((Segment)(ref val8))._002Ector(val.a, val.a);
						ref float3 a = ref val8.a;
						((float3)(ref a)).xz = ((float3)(ref a)).xz + leftDir * math.min(num4, num3);
						m_OverlayBuffer.DrawLine(m_GuideLineSettingsData.m_HighPriorityColor, val8, num2);
						GuideLinesSystem.DrawAngleIndicator(m_OverlayBuffer, m_Tooltips, m_GuideLineSettingsData, val8, val, -leftDir, -val2, math.min(num4, num3) * 0.5f, num2, bestLeft, angleSide: false);
					}
					if (bestRight <= 180)
					{
						Segment val9 = default(Segment);
						((Segment)(ref val9))._002Ector(val.a, val.a);
						ref float3 a2 = ref val9.a;
						((float3)(ref a2)).xz = ((float3)(ref a2)).xz + rightDir * math.min(num4, num3);
						m_OverlayBuffer.DrawLine(m_GuideLineSettingsData.m_HighPriorityColor, val9, num2);
						GuideLinesSystem.DrawAngleIndicator(m_OverlayBuffer, m_Tooltips, m_GuideLineSettingsData, val9, val, -rightDir, -val2, math.min(num4, num3) * 0.5f, num2, bestRight, angleSide: true);
					}
					if (m_AngleSides.Length > angleIndex)
					{
						m_AngleSides[angleIndex] = flag;
					}
					else
					{
						while (m_AngleSides.Length <= angleIndex)
						{
							m_AngleSides.Add(ref flag);
						}
					}
				}
				angleIndex++;
			}
			if (m_Mode == NetToolSystem.Mode.Continuous && m_ControlPoints.Length >= 3)
			{
				ControlPoint controlPoint2 = m_ControlPoints[0];
				ControlPoint controlPoint3 = m_ControlPoints[m_ControlPoints.Length - 2];
				ControlPoint controlPoint4 = m_ControlPoints[m_ControlPoints.Length - 1];
				if (math.dot(controlPoint4.m_Direction, controlPoint3.m_Direction) <= -0.01f)
				{
					float3 startTangent = default(float3);
					((float3)(ref startTangent))._002Ector(controlPoint3.m_Direction.x, 0f, controlPoint3.m_Direction.y);
					float3 val10 = default(float3);
					((float3)(ref val10))._002Ector(controlPoint4.m_Direction.x, 0f, controlPoint4.m_Direction.y);
					float num5 = math.dot(math.normalizesafe(((float3)(ref controlPoint4.m_Position)).xz - ((float3)(ref controlPoint2.m_Position)).xz, default(float2)), controlPoint3.m_Direction);
					Bezier4x3 val11;
					if (num5 <= -0.01f)
					{
						float3 endPos = controlPoint4.m_Position + val10 * num5;
						val11 = NetUtils.FitCurve(controlPoint2.m_Position, startTangent, val10, endPos);
						val11.d = controlPoint4.m_Position;
					}
					else
					{
						val11 = NetUtils.FitCurve(controlPoint2.m_Position, startTangent, val10, controlPoint4.m_Position);
					}
					val5 = MathUtils.Position(val11, 0.5f);
					Line2 val12 = default(Line2);
					val12.a = ((float3)(ref val5)).xz;
					float2 a3 = val12.a;
					val5 = MathUtils.Tangent(val11, 0.5f);
					val12.b = a3 + ((float3)(ref val5)).xz;
					Line2 val13 = default(Line2);
					((Line2)(ref val13))._002Ector(((float3)(ref controlPoint2.m_Position)).xz, ((float3)(ref controlPoint2.m_Position)).xz + controlPoint3.m_Direction);
					Line2 val14 = default(Line2);
					((Line2)(ref val14))._002Ector(((float3)(ref controlPoint4.m_Position)).xz, ((float3)(ref controlPoint4.m_Position)).xz - controlPoint4.m_Direction);
					ControlPoint controlPoint5 = controlPoint3;
					float2 val15 = default(float2);
					if (MathUtils.Intersect(val13, val12, ref val15))
					{
						((float3)(ref controlPoint3.m_Position)).xz = MathUtils.Position(val13, val15.x);
					}
					float2 val16 = default(float2);
					if (MathUtils.Intersect(val14, val12, ref val16))
					{
						((float3)(ref controlPoint5.m_Position)).xz = MathUtils.Position(val14, val16.x);
					}
					DrawControlPoint(controlPoint2, num2, ref prevPoint);
					DrawControlPointLine(controlPoint2, controlPoint3, num2, num3, ref angleIndex, ref prevLine);
					DrawControlPoint(controlPoint3, num2, ref prevPoint);
					DrawControlPointLine(controlPoint3, controlPoint5, num2, num3, ref angleIndex, ref prevLine);
					DrawControlPoint(controlPoint5, num2, ref prevPoint);
					DrawControlPointLine(controlPoint5, controlPoint4, num2, num3, ref angleIndex, ref prevLine);
					DrawControlPoint(controlPoint4, num2, ref prevPoint);
					return;
				}
			}
			if (m_Mode != NetToolSystem.Mode.Replace && m_ControlPoints.Length >= 3)
			{
				ControlPoint controlPoint6 = m_ControlPoints[0];
				int num6 = 0;
				for (int l = 1; l < m_ControlPoints.Length; l++)
				{
					ControlPoint controlPoint7 = m_ControlPoints[l];
					int num7 = Mathf.RoundToInt(math.distance(((float3)(ref controlPoint6.m_Position)).xz, ((float3)(ref controlPoint7.m_Position)).xz));
					num6 += math.select(0, 1, num7 > 0);
					controlPoint6 = controlPoint7;
				}
				if (num6 >= 2)
				{
					controlPoint6 = m_ControlPoints[0];
					for (int m = 1; m < m_ControlPoints.Length; m++)
					{
						ControlPoint controlPoint8 = m_ControlPoints[m];
						float num8 = (float)Mathf.RoundToInt(math.distance(((float3)(ref controlPoint6.m_Position)).xz, ((float3)(ref controlPoint8.m_Position)).xz) * 2f) / 2f;
						if (num8 > 0f)
						{
							ref NativeList<TooltipInfo> reference = ref m_Tooltips;
							TooltipInfo tooltipInfo = new TooltipInfo(TooltipType.Length, (controlPoint6.m_Position + controlPoint8.m_Position) * 0.5f, num8);
							reference.Add(ref tooltipInfo);
						}
						controlPoint6 = controlPoint8;
					}
				}
			}
			int num9 = 0;
			int num10 = m_ControlPoints.Length - 1;
			if (m_Mode == NetToolSystem.Mode.Replace)
			{
				num9 = 1;
				num10 = m_ControlPoints.Length - 2;
			}
			for (int n = num9; n <= num10; n++)
			{
				ControlPoint controlPoint9 = m_ControlPoints[n];
				if (n > num9)
				{
					ControlPoint point = m_ControlPoints[n - 1];
					DrawControlPointLine(point, controlPoint9, num2, num3, ref angleIndex, ref prevLine);
				}
				DrawControlPoint(controlPoint9, num2, ref prevPoint);
			}
		}

		private void DrawControlPoint(ControlPoint point, float lineWidth, ref float3 prevPoint)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (math.distancesq(prevPoint, point.m_Position) > 0.01f)
			{
				m_OverlayBuffer.DrawCircle(m_GuideLineSettingsData.m_HighPriorityColor, point.m_Position, lineWidth * 5f);
			}
			prevPoint = point.m_Position;
		}

		private void DrawControlPointLine(ControlPoint point1, ControlPoint point2, float lineWidth, float lineLength, ref int angleIndex, ref Segment prevLine)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			Segment val = default(Segment);
			((Segment)(ref val))._002Ector(point1.m_Position, point2.m_Position);
			float num = math.distance(((float3)(ref point1.m_Position)).xz, ((float3)(ref point2.m_Position)).xz);
			if (num > lineWidth * 8f)
			{
				float3 val2 = (val.b - val.a) * (lineWidth * 4f / num);
				Segment line = default(Segment);
				((Segment)(ref line))._002Ector(val.a + val2, val.b - val2);
				m_OverlayBuffer.DrawDashedLine(m_GuideLineSettingsData.m_HighPriorityColor, line, lineWidth * 3f, lineWidth * 5f, lineWidth * 3f);
			}
			DrawAngleIndicator(prevLine, val, lineWidth, lineLength, angleIndex++);
			prevLine = val;
		}

		private void DrawAngleIndicator(Segment line1, Segment line2, float lineWidth, float lineLength, int angleIndex)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			bool flag = true;
			if (m_AngleSides.Length > angleIndex)
			{
				flag = m_AngleSides[angleIndex];
			}
			float num = math.distance(((float3)(ref line1.a)).xz, ((float3)(ref line1.b)).xz);
			float num2 = math.distance(((float3)(ref line2.a)).xz, ((float3)(ref line2.b)).xz);
			if (num > lineWidth * 7f && num2 > lineWidth * 7f)
			{
				float2 val = (((float3)(ref line1.b)).xz - ((float3)(ref line1.a)).xz) / num;
				float2 val2 = (((float3)(ref line2.a)).xz - ((float3)(ref line2.b)).xz) / num2;
				float size = math.min(lineLength, math.min(num, num2)) * 0.5f;
				int num3 = Mathf.RoundToInt(math.degrees(math.acos(math.clamp(math.dot(val, val2), -1f, 1f))));
				if (num3 < 180)
				{
					flag = math.dot(MathUtils.Right(val), val2) < 0f;
				}
				GuideLinesSystem.DrawAngleIndicator(m_OverlayBuffer, m_Tooltips, m_GuideLineSettingsData, line1, line2, val, val2, size, lineWidth, num3, flag);
			}
			if (m_AngleSides.Length > angleIndex)
			{
				m_AngleSides[angleIndex] = flag;
				return;
			}
			while (m_AngleSides.Length <= angleIndex)
			{
				m_AngleSides.Add(ref flag);
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> __Game_Tools_CreationDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<NetCourse> __Game_Tools_NetCourse_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.Node> __Game_Net_Node_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableNetData> __Game_Prefabs_PlaceableNetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RoadData> __Game_Prefabs_RoadData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityConnectionData> __Game_Prefabs_ElectricityConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterPipeConnectionData> __Game_Prefabs_WaterPipeConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceConnectionData> __Game_Prefabs_ResourceConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedNode> __Game_Net_ConnectedNode_RO_BufferLookup;

		[ReadOnly]
		public BufferTypeHandle<WaypointDefinition> __Game_Routes_WaypointDefinition_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Route> __Game_Routes_Route_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteData> __Game_Prefabs_RouteData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Zoning> __Game_Tools_Zoning_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Areas.Node> __Game_Areas_Node_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LotData> __Game_Prefabs_LotData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<ObjectDefinition> __Game_Tools_ObjectDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<OwnerDefinition> __Game_Tools_OwnerDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> __Game_Prefabs_PlaceableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceUpgradeData> __Game_Prefabs_ServiceUpgradeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubArea> __Game_Prefabs_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubNet> __Game_Prefabs_SubNet_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Game.Simulation.WaterSourceData> __Game_Simulation_WaterSourceData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<GuideLineSettingsData> __Game_Prefabs_GuideLineSettingsData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<WaterSourceColorElement> __Game_Prefabs_WaterSourceColorElement_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			__Game_Tools_CreationDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreationDefinition>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tools_NetCourse_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetCourse>(true);
			__Game_Net_Node_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.Node>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Net_ConnectedEdge_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedEdge>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Node>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_PlaceableNetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableNetData>(true);
			__Game_Prefabs_RoadData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RoadData>(true);
			__Game_Prefabs_ElectricityConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityConnectionData>(true);
			__Game_Prefabs_WaterPipeConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeConnectionData>(true);
			__Game_Prefabs_ResourceConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceConnectionData>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_ConnectedNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedNode>(true);
			__Game_Routes_WaypointDefinition_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<WaypointDefinition>(true);
			__Game_Routes_Route_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Route>(true);
			__Game_Prefabs_RouteData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteData>(true);
			__Game_Tools_Zoning_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Zoning>(true);
			__Game_Areas_Node_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Areas.Node>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Prefabs_AreaGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaGeometryData>(true);
			__Game_Prefabs_LotData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LotData>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Tools_ObjectDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ObjectDefinition>(true);
			__Game_Tools_OwnerDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<OwnerDefinition>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableObjectData>(true);
			__Game_Prefabs_ServiceUpgradeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceUpgradeData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubArea>(true);
			__Game_Prefabs_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubNet>(true);
			__Game_Simulation_WaterSourceData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Simulation.WaterSourceData>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Prefabs_GuideLineSettingsData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GuideLineSettingsData>(true);
			__Game_Prefabs_WaterSourceColorElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<WaterSourceColorElement>(true);
		}
	}

	private EntityQuery m_DefinitionQuery;

	private EntityQuery m_MarkerNodeQuery;

	private EntityQuery m_TempNodeQuery;

	private EntityQuery m_WaterSourceQuery;

	private EntityQuery m_RenderingSettingsQuery;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private ToolSystem m_ToolSystem;

	private NetToolSystem m_NetToolSystem;

	private RouteToolSystem m_RouteToolSystem;

	private ZoneToolSystem m_ZoneToolSystem;

	private SelectionToolSystem m_SelectionToolSystem;

	private AreaToolSystem m_AreaToolSystem;

	private ObjectToolSystem m_ObjectToolSystem;

	private WaterToolSystem m_WaterToolSystem;

	private OverlayRenderSystem m_OverlayRenderSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private ToolRaycastSystem m_ToolRaycastSystem;

	private PrefabSystem m_PrefabSystem;

	private NativeList<bool> m_AngleSides;

	private NativeList<TooltipInfo> m_Tooltips;

	private JobHandle m_TooltipDeps;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_NetToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NetToolSystem>();
		m_RouteToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RouteToolSystem>();
		m_ZoneToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ZoneToolSystem>();
		m_SelectionToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SelectionToolSystem>();
		m_AreaToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AreaToolSystem>();
		m_ObjectToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ObjectToolSystem>();
		m_WaterToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterToolSystem>();
		m_OverlayRenderSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<OverlayRenderSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_ToolRaycastSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolRaycastSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CreationDefinition>() };
		val.Any = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<NetCourse>(),
			ComponentType.ReadOnly<WaypointDefinition>(),
			ComponentType.ReadOnly<Zoning>(),
			ComponentType.ReadOnly<Game.Areas.Node>(),
			ComponentType.ReadOnly<ObjectDefinition>()
		};
		val.None = (ComponentType[])(object)new ComponentType[0];
		array[0] = val;
		m_DefinitionQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_TempNodeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Net.Node>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_MarkerNodeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Game.Net.Marker>(),
			ComponentType.ReadOnly<Orphan>(),
			ComponentType.ReadOnly<Game.Net.Node>(),
			ComponentType.Exclude<Hidden>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_WaterSourceQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Simulation.WaterSourceData>(),
			ComponentType.Exclude<PrefabRef>(),
			ComponentType.Exclude<Hidden>(),
			ComponentType.Exclude<Deleted>()
		});
		m_RenderingSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<GuideLineSettingsData>() });
		m_AngleSides = new NativeList<bool>(4, AllocatorHandle.op_Implicit((Allocator)4));
		m_Tooltips = new NativeList<TooltipInfo>(8, AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_AngleSides.Dispose();
		m_Tooltips.Dispose();
		base.OnDestroy();
	}

	public NativeList<TooltipInfo> GetTooltips(out JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		dependencies = m_TooltipDeps;
		return m_Tooltips;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_0740: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0791: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_0798: Unknown result type (might be due to invalid IL or missing references)
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		//IL_079f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07af: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_082b: Unknown result type (might be due to invalid IL or missing references)
		//IL_082d: Unknown result type (might be due to invalid IL or missing references)
		//IL_085b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0860: Unknown result type (might be due to invalid IL or missing references)
		//IL_0862: Unknown result type (might be due to invalid IL or missing references)
		//IL_0867: Unknown result type (might be due to invalid IL or missing references)
		//IL_086c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0874: Unknown result type (might be due to invalid IL or missing references)
		//IL_087c: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08be: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08db: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0910: Unknown result type (might be due to invalid IL or missing references)
		//IL_0915: Unknown result type (might be due to invalid IL or missing references)
		//IL_092d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0932: Unknown result type (might be due to invalid IL or missing references)
		//IL_094a: Unknown result type (might be due to invalid IL or missing references)
		//IL_094f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0967: Unknown result type (might be due to invalid IL or missing references)
		//IL_096c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0984: Unknown result type (might be due to invalid IL or missing references)
		//IL_0989: Unknown result type (might be due to invalid IL or missing references)
		//IL_0997: Unknown result type (might be due to invalid IL or missing references)
		//IL_099e: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ade: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bde: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ccf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a06: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ebb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0edf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e25: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a25: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a70: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dcc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de9: Unknown result type (might be due to invalid IL or missing references)
		m_Tooltips.Clear();
		bool flag = (m_ToolRaycastSystem.raycastFlags & (RaycastFlags.DebugDisable | RaycastFlags.UIDisable)) != 0;
		if (m_ToolSystem.activeTool == m_NetToolSystem)
		{
			if (flag)
			{
				return;
			}
			JobHandle val = default(JobHandle);
			JobHandle dependencies;
			JobHandle dependencies2;
			JobHandle deps;
			JobHandle dependencies3;
			NetToolGuideLinesJob netToolGuideLinesJob = new NetToolGuideLinesJob
			{
				m_CreationDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<CreationDefinition>(ref __TypeHandle.__Game_Tools_CreationDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NetCourseType = InternalCompilerInterface.GetComponentTypeHandle<NetCourse>(ref __TypeHandle.__Game_Tools_NetCourse_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NodeType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedEdgeType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PlaceableNetData = InternalCompilerInterface.GetComponentLookup<PlaceableNetData>(ref __TypeHandle.__Game_Prefabs_PlaceableNetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RoadData = InternalCompilerInterface.GetComponentLookup<RoadData>(ref __TypeHandle.__Game_Prefabs_RoadData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElectricityConnectionData = InternalCompilerInterface.GetComponentLookup<ElectricityConnectionData>(ref __TypeHandle.__Game_Prefabs_ElectricityConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WaterPipeConnectionData = InternalCompilerInterface.GetComponentLookup<WaterPipeConnectionData>(ref __TypeHandle.__Game_Prefabs_WaterPipeConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceConnectionData = InternalCompilerInterface.GetComponentLookup<ResourceConnectionData>(ref __TypeHandle.__Game_Prefabs_ResourceConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedNodes = InternalCompilerInterface.GetBufferLookup<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DefinitionChunks = ((EntityQuery)(ref m_DefinitionQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val),
				m_ControlPoints = m_NetToolSystem.GetControlPoints(out dependencies),
				m_SnapLines = m_NetToolSystem.GetSnapLines(out dependencies2),
				m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
				m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
				m_Mode = m_NetToolSystem.actualMode,
				m_HighlightVoltage = Game.Prefabs.ElectricityConnection.Voltage.Invalid,
				m_HighlightWater = bool2.op_Implicit(false),
				m_HighResourceLine = false,
				m_Prefab = (((Object)(object)m_NetToolSystem.prefab != (Object)null) ? m_PrefabSystem.GetEntity(m_NetToolSystem.prefab) : Entity.Null),
				m_GuideLineSettingsData = ((EntityQuery)(ref m_RenderingSettingsQuery)).GetSingleton<GuideLineSettingsData>(),
				m_AngleSides = m_AngleSides,
				m_Tooltips = m_Tooltips,
				m_OverlayBuffer = m_OverlayRenderSystem.GetBuffer(out dependencies3)
			};
			JobHandle val2 = JobUtils.CombineDependencies(dependencies, dependencies2, deps, dependencies3);
			if (m_NetToolSystem.prefab is PowerLinePrefab)
			{
				if (m_NetToolSystem.prefab.TryGet<Game.Prefabs.ElectricityConnection>(out var component))
				{
					JobHandle val3 = default(JobHandle);
					netToolGuideLinesJob.m_MarkerNodeChunks = ((EntityQuery)(ref m_MarkerNodeQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val3);
					JobHandle val4 = default(JobHandle);
					netToolGuideLinesJob.m_TempNodeChunks = ((EntityQuery)(ref m_TempNodeQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val4);
					netToolGuideLinesJob.m_HighlightVoltage = component.m_Voltage;
					val2 = JobHandle.CombineDependencies(val2, val3, val4);
				}
			}
			else if (m_NetToolSystem.prefab is PipelinePrefab)
			{
				JobHandle val5 = default(JobHandle);
				netToolGuideLinesJob.m_MarkerNodeChunks = ((EntityQuery)(ref m_MarkerNodeQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val5);
				JobHandle val6 = default(JobHandle);
				netToolGuideLinesJob.m_TempNodeChunks = ((EntityQuery)(ref m_TempNodeQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val6);
				val2 = JobHandle.CombineDependencies(val2, val5, val6);
				if (m_NetToolSystem.prefab.TryGet<Game.Prefabs.WaterPipeConnection>(out var component2))
				{
					netToolGuideLinesJob.m_HighlightWater.x = component2.m_FreshCapacity > 0;
					netToolGuideLinesJob.m_HighlightWater.y = component2.m_SewageCapacity > 0;
				}
				else
				{
					netToolGuideLinesJob.m_HighResourceLine = true;
				}
			}
			JobHandle val7 = IJobExtensions.Schedule<NetToolGuideLinesJob>(netToolGuideLinesJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, val2));
			if (netToolGuideLinesJob.m_MarkerNodeChunks.IsCreated)
			{
				netToolGuideLinesJob.m_MarkerNodeChunks.Dispose(val7);
			}
			if (netToolGuideLinesJob.m_TempNodeChunks.IsCreated)
			{
				netToolGuideLinesJob.m_TempNodeChunks.Dispose(val7);
			}
			netToolGuideLinesJob.m_DefinitionChunks.Dispose(val7);
			m_TerrainSystem.AddCPUHeightReader(val7);
			m_WaterSystem.AddSurfaceReader(val7);
			m_OverlayRenderSystem.AddBufferWriter(val7);
			m_TooltipDeps = val7;
			((SystemBase)this).Dependency = val7;
		}
		else if (m_ToolSystem.activeTool == m_RouteToolSystem)
		{
			if (!flag)
			{
				JobHandle val8 = default(JobHandle);
				JobHandle dependencies4;
				JobHandle dependencies5;
				RouteToolGuideLinesJob routeToolGuideLinesJob = new RouteToolGuideLinesJob
				{
					m_CreationDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<CreationDefinition>(ref __TypeHandle.__Game_Tools_CreationDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_WaypointDefinitionType = InternalCompilerInterface.GetBufferTypeHandle<WaypointDefinition>(ref __TypeHandle.__Game_Routes_WaypointDefinition_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_RouteData = InternalCompilerInterface.GetComponentLookup<Route>(ref __TypeHandle.__Game_Routes_Route_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRouteData = InternalCompilerInterface.GetComponentLookup<RouteData>(ref __TypeHandle.__Game_Prefabs_RouteData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_DefinitionChunks = ((EntityQuery)(ref m_DefinitionQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val8),
					m_ControlPoints = m_RouteToolSystem.GetControlPoints(out dependencies4),
					m_MoveStartPosition = m_RouteToolSystem.moveStartPosition,
					m_State = m_RouteToolSystem.state,
					m_GuideLineSettingsData = ((EntityQuery)(ref m_RenderingSettingsQuery)).GetSingleton<GuideLineSettingsData>(),
					m_OverlayBuffer = m_OverlayRenderSystem.GetBuffer(out dependencies5)
				};
				JobHandle val9 = JobHandle.CombineDependencies(dependencies4, dependencies5);
				JobHandle val10 = IJobExtensions.Schedule<RouteToolGuideLinesJob>(routeToolGuideLinesJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val8, val9));
				routeToolGuideLinesJob.m_DefinitionChunks.Dispose(val10);
				m_OverlayRenderSystem.AddBufferWriter(val10);
				((SystemBase)this).Dependency = val10;
			}
		}
		else if (m_ToolSystem.activeTool == m_ZoneToolSystem)
		{
			if (!flag)
			{
				JobHandle val11 = default(JobHandle);
				JobHandle dependencies6;
				ZoneToolGuideLinesJob zoneToolGuideLinesJob = new ZoneToolGuideLinesJob
				{
					m_CreationDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<CreationDefinition>(ref __TypeHandle.__Game_Tools_CreationDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ZoningType = InternalCompilerInterface.GetComponentTypeHandle<Zoning>(ref __TypeHandle.__Game_Tools_Zoning_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_DefinitionChunks = ((EntityQuery)(ref m_DefinitionQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val11),
					m_GuideLineSettingsData = ((EntityQuery)(ref m_RenderingSettingsQuery)).GetSingleton<GuideLineSettingsData>(),
					m_OverlayBuffer = m_OverlayRenderSystem.GetBuffer(out dependencies6)
				};
				JobHandle val12 = IJobExtensions.Schedule<ZoneToolGuideLinesJob>(zoneToolGuideLinesJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val11, dependencies6));
				zoneToolGuideLinesJob.m_DefinitionChunks.Dispose(val12);
				m_OverlayRenderSystem.AddBufferWriter(val12);
				((SystemBase)this).Dependency = val12;
			}
		}
		else if (m_ToolSystem.activeTool == m_SelectionToolSystem)
		{
			if (!flag)
			{
				Quad3 quad;
				bool selectionQuad = m_SelectionToolSystem.GetSelectionQuad(out quad);
				JobHandle dependencies7;
				JobHandle val13 = IJobExtensions.Schedule<SelectionToolGuideLinesJob>(new SelectionToolGuideLinesJob
				{
					m_State = m_SelectionToolSystem.state,
					m_SelectionType = m_SelectionToolSystem.selectionType,
					m_SelectionQuadIsValid = selectionQuad,
					m_SelectionQuad = quad,
					m_GuideLineSettingsData = ((EntityQuery)(ref m_RenderingSettingsQuery)).GetSingleton<GuideLineSettingsData>(),
					m_OverlayBuffer = m_OverlayRenderSystem.GetBuffer(out dependencies7)
				}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies7));
				m_OverlayRenderSystem.AddBufferWriter(val13);
				((SystemBase)this).Dependency = val13;
			}
		}
		else if (m_ToolSystem.activeTool == m_AreaToolSystem)
		{
			if (!flag)
			{
				JobHandle val14 = default(JobHandle);
				NativeList<ControlPoint> moveStartPositions;
				JobHandle dependencies8;
				JobHandle dependencies9;
				AreaToolGuideLinesJob areaToolGuideLinesJob = new AreaToolGuideLinesJob
				{
					m_CreationDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<CreationDefinition>(ref __TypeHandle.__Game_Tools_CreationDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_NodeType = InternalCompilerInterface.GetBufferTypeHandle<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabLotData = InternalCompilerInterface.GetComponentLookup<LotData>(ref __TypeHandle.__Game_Prefabs_LotData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_Nodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_DefinitionChunks = ((EntityQuery)(ref m_DefinitionQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val14),
					m_ControlPoints = m_AreaToolSystem.GetControlPoints(out moveStartPositions, out dependencies8),
					m_MoveStartPositions = moveStartPositions,
					m_State = m_AreaToolSystem.state,
					m_Prefab = (((Object)(object)m_AreaToolSystem.prefab != (Object)null) ? m_PrefabSystem.GetEntity(m_AreaToolSystem.prefab) : Entity.Null),
					m_GuideLineSettingsData = ((EntityQuery)(ref m_RenderingSettingsQuery)).GetSingleton<GuideLineSettingsData>(),
					m_AngleSides = m_AngleSides,
					m_Tooltips = m_Tooltips,
					m_OverlayBuffer = m_OverlayRenderSystem.GetBuffer(out dependencies9)
				};
				JobHandle val15 = JobHandle.CombineDependencies(dependencies8, dependencies9);
				JobHandle val16 = IJobExtensions.Schedule<AreaToolGuideLinesJob>(areaToolGuideLinesJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val14, val15));
				areaToolGuideLinesJob.m_DefinitionChunks.Dispose(val16);
				m_OverlayRenderSystem.AddBufferWriter(val16);
				m_TooltipDeps = val16;
				((SystemBase)this).Dependency = val16;
			}
		}
		else if (m_ToolSystem.activeTool == m_ObjectToolSystem)
		{
			if (!flag)
			{
				JobHandle val17 = default(JobHandle);
				JobHandle dependencies10;
				JobHandle dependencies11;
				JobHandle deps2;
				JobHandle dependencies12;
				JobHandle dependencies13;
				ObjectToolGuideLinesJob objectToolGuideLinesJob = new ObjectToolGuideLinesJob
				{
					m_CreationDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<CreationDefinition>(ref __TypeHandle.__Game_Tools_CreationDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ObjectDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<ObjectDefinition>(ref __TypeHandle.__Game_Tools_ObjectDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_OwnerDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<OwnerDefinition>(ref __TypeHandle.__Game_Tools_OwnerDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_NetCourseType = InternalCompilerInterface.GetComponentTypeHandle<NetCourse>(ref __TypeHandle.__Game_Tools_NetCourse_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabPlaceableObjectData = InternalCompilerInterface.GetComponentLookup<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabServiceUpgradeData = InternalCompilerInterface.GetComponentLookup<ServiceUpgradeData>(ref __TypeHandle.__Game_Prefabs_ServiceUpgradeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabLotData = InternalCompilerInterface.GetComponentLookup<LotData>(ref __TypeHandle.__Game_Prefabs_LotData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabPlaceableNetData = InternalCompilerInterface.GetComponentLookup<PlaceableNetData>(ref __TypeHandle.__Game_Prefabs_PlaceableNetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabSubAreas = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubArea>(ref __TypeHandle.__Game_Prefabs_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabSubNets = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubNet>(ref __TypeHandle.__Game_Prefabs_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_DefinitionChunks = ((EntityQuery)(ref m_DefinitionQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val17),
					m_ControlPoints = m_ObjectToolSystem.GetControlPoints(out dependencies10),
					m_SubSnapPoints = m_ObjectToolSystem.GetSubSnapPoints(out dependencies11),
					m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
					m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps2),
					m_NetUpgradeState = m_ObjectToolSystem.GetNetUpgradeStates(out dependencies12),
					m_GuideLineSettingsData = ((EntityQuery)(ref m_RenderingSettingsQuery)).GetSingleton<GuideLineSettingsData>(),
					m_Mode = m_ObjectToolSystem.actualMode,
					m_State = m_ObjectToolSystem.state,
					m_Prefab = (((Object)(object)m_ObjectToolSystem.prefab != (Object)null) ? m_PrefabSystem.GetEntity(m_ObjectToolSystem.prefab) : Entity.Null),
					m_DistanceScale = m_ObjectToolSystem.distanceScale,
					m_AngleSides = m_AngleSides,
					m_Tooltips = m_Tooltips,
					m_OverlayBuffer = m_OverlayRenderSystem.GetBuffer(out dependencies13)
				};
				JobHandle val18 = IJobExtensions.Schedule<ObjectToolGuideLinesJob>(objectToolGuideLinesJob, JobUtils.CombineDependencies(((SystemBase)this).Dependency, val17, dependencies10, dependencies11, deps2, dependencies12, dependencies13));
				objectToolGuideLinesJob.m_DefinitionChunks.Dispose(val18);
				m_TerrainSystem.AddCPUHeightReader(val18);
				m_WaterSystem.AddSurfaceReader(val18);
				m_OverlayRenderSystem.AddBufferWriter(val18);
				m_TooltipDeps = val18;
				((SystemBase)this).Dependency = val18;
			}
		}
		else if (m_ToolSystem.activeTool == m_WaterToolSystem)
		{
			float3 cameraRight = default(float3);
			if (m_CameraUpdateSystem.TryGetViewer(out var viewer))
			{
				cameraRight = viewer.right;
			}
			JobHandle val19 = default(JobHandle);
			JobHandle dependencies14;
			WaterToolGuideLinesJob waterToolGuideLinesJob = new WaterToolGuideLinesJob
			{
				m_WaterSourceDataType = InternalCompilerInterface.GetComponentTypeHandle<Game.Simulation.WaterSourceData>(ref __TypeHandle.__Game_Simulation_WaterSourceData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_GuideLineSettingsData = InternalCompilerInterface.GetComponentLookup<GuideLineSettingsData>(ref __TypeHandle.__Game_Prefabs_GuideLineSettingsData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WaterSourceColors = InternalCompilerInterface.GetBufferLookup<WaterSourceColorElement>(ref __TypeHandle.__Game_Prefabs_WaterSourceColorElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Attribute = m_WaterToolSystem.attribute,
				m_PositionOffset = m_TerrainSystem.positionOffset,
				m_CameraRight = cameraRight,
				m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
				m_WaterSourceChunks = ((EntityQuery)(ref m_WaterSourceQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val19),
				m_GuideLineSettingsEntity = ((EntityQuery)(ref m_RenderingSettingsQuery)).GetSingletonEntity(),
				m_OverlayBuffer = m_OverlayRenderSystem.GetBuffer(out dependencies14)
			};
			JobHandle val20 = IJobExtensions.Schedule<WaterToolGuideLinesJob>(waterToolGuideLinesJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val19, dependencies14));
			waterToolGuideLinesJob.m_WaterSourceChunks.Dispose(val20);
			m_OverlayRenderSystem.AddBufferWriter(val20);
			((SystemBase)this).Dependency = val20;
		}
	}

	private static void CheckDirection(float2 startDir, float2 checkDir, ref float2 leftDir, ref float2 rightDir, ref int bestLeft, ref int bestRight, ref float2 leftDir2, ref float2 rightDir2, ref int bestLeft2, ref int bestRight2)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		if (!MathUtils.TryNormalize(ref checkDir))
		{
			return;
		}
		int num = Mathf.RoundToInt(math.degrees(math.acos(math.clamp(math.dot(startDir, checkDir), -1f, 1f))));
		if (num == 0)
		{
			return;
		}
		bool num2 = math.dot(MathUtils.Right(startDir), checkDir) > 0f;
		if (num2 || num == 180)
		{
			if (num < bestRight)
			{
				rightDir = checkDir;
				bestRight = num;
			}
			if ((num == 90 || num == 180) && num < bestRight2)
			{
				rightDir2 = checkDir;
				bestRight2 = num;
			}
		}
		if (!num2 || num == 180)
		{
			if (num < bestLeft)
			{
				leftDir = checkDir;
				bestLeft = num;
			}
			if ((num == 90 || num == 180) && num < bestLeft2)
			{
				leftDir2 = checkDir;
				bestLeft2 = num;
			}
		}
	}

	private static void DrawAngleIndicator(OverlayRenderSystem.Buffer buffer, NativeList<TooltipInfo> tooltips, GuideLineSettingsData guideLineSettings, Segment line1, Segment line2, float2 dir1, float2 dir2, float size, float lineWidth, int angle, bool angleSide)
	{
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		if (angle == 180)
		{
			float2 val = (angleSide ? MathUtils.Right(dir1) : MathUtils.Left(dir1));
			float2 val2 = (angleSide ? MathUtils.Right(dir2) : MathUtils.Left(dir2));
			float3 b = line1.b;
			((float3)(ref b)).xz = ((float3)(ref b)).xz - dir1 * size;
			float3 b2 = line1.b;
			float3 b3 = line1.b;
			((float3)(ref b2)).xz = ((float3)(ref b2)).xz + (val * (size - lineWidth * 0.5f) - dir1 * size);
			((float3)(ref b3)).xz = ((float3)(ref b3)).xz + (val * size - dir1 * (size + lineWidth * 0.5f));
			float3 a = line2.a;
			float3 a2 = line2.a;
			((float3)(ref a)).xz = ((float3)(ref a)).xz - (val2 * size + dir2 * (size + lineWidth * 0.5f));
			((float3)(ref a2)).xz = ((float3)(ref a2)).xz - (val2 * (size - lineWidth * 0.5f) + dir2 * size);
			float3 a3 = line2.a;
			((float3)(ref a3)).xz = ((float3)(ref a3)).xz - dir2 * size;
			buffer.DrawLine(guideLineSettings.m_HighPriorityColor, new Segment(b, b2), lineWidth);
			buffer.DrawLine(guideLineSettings.m_HighPriorityColor, new Segment(b3, a), lineWidth);
			buffer.DrawLine(guideLineSettings.m_HighPriorityColor, new Segment(a2, a3), lineWidth);
			float3 b4 = line1.b;
			((float3)(ref b4)).xz = ((float3)(ref b4)).xz + val * (size * 1.5f);
			TooltipInfo tooltipInfo = new TooltipInfo(TooltipType.Angle, b4, angle);
			tooltips.Add(ref tooltipInfo);
		}
		else if (angle > 90)
		{
			float2 val3 = math.normalize(dir1 + dir2);
			float3 b5 = line1.b;
			((float3)(ref b5)).xz = ((float3)(ref b5)).xz - dir1 * size;
			float3 startTangent = default(float3);
			((float3)(ref startTangent)).xz = (angleSide ? MathUtils.Right(dir1) : MathUtils.Left(dir1));
			float3 b6 = line1.b;
			((float3)(ref b6)).xz = ((float3)(ref b6)).xz - val3 * size;
			float3 val4 = default(float3);
			((float3)(ref val4)).xz = (angleSide ? MathUtils.Right(val3) : MathUtils.Left(val3));
			float3 a4 = line2.a;
			((float3)(ref a4)).xz = ((float3)(ref a4)).xz - dir2 * size;
			float3 endTangent = default(float3);
			((float3)(ref endTangent)).xz = (angleSide ? MathUtils.Right(dir2) : MathUtils.Left(dir2));
			buffer.DrawCurve(guideLineSettings.m_HighPriorityColor, NetUtils.FitCurve(b5, startTangent, val4, b6), lineWidth);
			buffer.DrawCurve(guideLineSettings.m_HighPriorityColor, NetUtils.FitCurve(b6, val4, endTangent, a4), lineWidth);
			float3 b7 = line1.b;
			((float3)(ref b7)).xz = ((float3)(ref b7)).xz - val3 * (size * 1.5f);
			TooltipInfo tooltipInfo = new TooltipInfo(TooltipType.Angle, b7, angle);
			tooltips.Add(ref tooltipInfo);
		}
		else if (angle == 90)
		{
			float3 b8 = line1.b;
			((float3)(ref b8)).xz = ((float3)(ref b8)).xz - dir1 * size;
			float3 b9 = line1.b;
			float3 b10 = line1.b;
			((float3)(ref b9)).xz = ((float3)(ref b9)).xz - (dir2 * (size - lineWidth * 0.5f) + dir1 * size);
			((float3)(ref b10)).xz = ((float3)(ref b10)).xz - (dir2 * size + dir1 * (size + lineWidth * 0.5f));
			float3 a5 = line2.a;
			((float3)(ref a5)).xz = ((float3)(ref a5)).xz - dir2 * size;
			buffer.DrawLine(guideLineSettings.m_HighPriorityColor, new Segment(b8, b9), lineWidth);
			buffer.DrawLine(guideLineSettings.m_HighPriorityColor, new Segment(b10, a5), lineWidth);
			float3 b11 = line1.b;
			((float3)(ref b11)).xz = ((float3)(ref b11)).xz - math.normalizesafe(dir1 + dir2, default(float2)) * (size * 1.5f);
			TooltipInfo tooltipInfo = new TooltipInfo(TooltipType.Angle, b11, angle);
			tooltips.Add(ref tooltipInfo);
		}
		else if (angle > 0)
		{
			float3 b12 = line1.b;
			((float3)(ref b12)).xz = ((float3)(ref b12)).xz - dir1 * size;
			float3 startTangent2 = default(float3);
			((float3)(ref startTangent2)).xz = (angleSide ? MathUtils.Right(dir1) : MathUtils.Left(dir1));
			float3 a6 = line2.a;
			((float3)(ref a6)).xz = ((float3)(ref a6)).xz - dir2 * size;
			float3 endTangent2 = default(float3);
			((float3)(ref endTangent2)).xz = (angleSide ? MathUtils.Right(dir2) : MathUtils.Left(dir2));
			buffer.DrawCurve(guideLineSettings.m_HighPriorityColor, NetUtils.FitCurve(b12, startTangent2, endTangent2, a6), lineWidth);
			float3 b13 = line1.b;
			((float3)(ref b13)).xz = ((float3)(ref b13)).xz - math.normalizesafe(dir1 + dir2, default(float2)) * (size * 1.5f);
			TooltipInfo tooltipInfo = new TooltipInfo(TooltipType.Angle, b13, angle);
			tooltips.Add(ref tooltipInfo);
		}
	}

	private static void DrawAreaRange(OverlayRenderSystem.Buffer buffer, quaternion rotation, float3 position, LotData lotData)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		float3 val = math.forward(rotation);
		Color val2 = Color32.op_Implicit(lotData.m_RangeColor);
		Color fillColor = val2;
		fillColor.a = 0f;
		OverlayRenderSystem.StyleFlags styleFlags = ((!lotData.m_OnWater) ? OverlayRenderSystem.StyleFlags.Projected : ((OverlayRenderSystem.StyleFlags)0));
		buffer.DrawCircle(val2, fillColor, lotData.m_MaxRadius * 0.02f, styleFlags, ((float3)(ref val)).xz, position, lotData.m_MaxRadius * 2f);
	}

	private static void DrawUpgradeRange(OverlayRenderSystem.Buffer buffer, quaternion rotation, float3 position, GuideLineSettingsData guideLineSettings, BuildingData ownerBuildingData, BuildingData buildingData, ServiceUpgradeData serviceUpgradeData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Color lowPriorityColor = guideLineSettings.m_LowPriorityColor;
		Color fillColor = lowPriorityColor;
		fillColor.a = 0f;
		BuildingUtils.CalculateUpgradeRangeValues(rotation, ownerBuildingData, buildingData, serviceUpgradeData, out var forward, out var width, out var length, out var roundness, out var circular);
		roundness *= 2f;
		length -= roundness;
		roundness /= width;
		if (circular)
		{
			buffer.DrawCircle(lowPriorityColor, fillColor, width * 0.01f, OverlayRenderSystem.StyleFlags.Projected, ((float3)(ref forward)).xz, position, width);
			return;
		}
		Segment line = default(Segment);
		((Segment)(ref line))._002Ector(position - forward * (length * 0.5f), position + forward * (length * 0.5f));
		buffer.DrawLine(lowPriorityColor, fillColor, width * 0.01f, OverlayRenderSystem.StyleFlags.Projected, line, width, float2.op_Implicit(roundness));
	}

	private static void DrawNetCourse(OverlayRenderSystem.Buffer buffer, NetCourse netCourse, ref TerrainHeightData terrainHeightData, ref WaterSurfaceData waterSurfaceData, NetGeometryData netGeometryData, GuideLineSettingsData guideLineSettings)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		float2 val = math.select(float2.op_Implicit(0f), float2.op_Implicit(1f), new bool2((netCourse.m_StartPosition.m_Flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsLast)) != 0, (netCourse.m_EndPosition.m_Flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsLast)) != 0));
		if (netCourse.m_Length < 0.01f)
		{
			OverlayRenderSystem.StyleFlags styleFlags = OverlayRenderSystem.StyleFlags.Projected;
			if (WaterUtils.SampleDepth(ref waterSurfaceData, netCourse.m_StartPosition.m_Position) >= 0.2f)
			{
				netCourse.m_StartPosition.m_Position.y = WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, netCourse.m_StartPosition.m_Position);
				styleFlags = (OverlayRenderSystem.StyleFlags)0;
			}
			buffer.DrawCircle(guideLineSettings.m_MediumPriorityColor, guideLineSettings.m_MediumPriorityColor, 0f, styleFlags, new float2(0f, 1f), netCourse.m_StartPosition.m_Position, netGeometryData.m_DefaultWidth);
			return;
		}
		float2 val2 = default(float2);
		((float2)(ref val2))._002Ector(netCourse.m_StartPosition.m_CourseDelta, netCourse.m_EndPosition.m_CourseDelta);
		float num = netCourse.m_Length * math.abs(val2.y - val2.x);
		int num2 = math.max(1, (int)(num * 0.0625f));
		int num3 = 0;
		float num4 = 1f / (float)num2;
		float3 val3 = default(float3);
		((float3)(ref val3))._002Ector(val2.x, math.lerp(val2.x, val2.y, num4), 0f);
		bool3 val4 = default(bool3);
		((bool3)(ref val4))._002Ector(WaterUtils.SampleDepth(ref waterSurfaceData, MathUtils.Position(netCourse.m_Curve, val3.x)) >= 0.2f, WaterUtils.SampleDepth(ref waterSurfaceData, MathUtils.Position(netCourse.m_Curve, val3.y)) >= 0.2f, false);
		for (int i = 1; i <= num2; i++)
		{
			bool flag = i == num2;
			if (!flag)
			{
				val3.z = math.lerp(val2.x, val2.y, (float)(i + 1) * num4);
				val4.z = WaterUtils.SampleDepth(ref waterSurfaceData, MathUtils.Position(netCourse.m_Curve, val3.z)) >= 0.2f;
			}
			if (flag || math.any(((bool3)(ref val4)).xy != ((bool3)(ref val4)).yz))
			{
				bool flag2 = i - num3 << 1 > num2;
				OverlayRenderSystem.StyleFlags styleFlags2 = ((!math.any(((bool3)(ref val4)).xy)) ? OverlayRenderSystem.StyleFlags.Projected : ((OverlayRenderSystem.StyleFlags)0));
				Bezier4x3 val5;
				Bezier4x3 val6;
				if (flag2)
				{
					val5 = MathUtils.Cut(netCourse.m_Curve, new float2(val3.x, math.lerp(val3.x, val3.y, 0.5f)));
					val6 = MathUtils.Cut(netCourse.m_Curve, new float2(math.lerp(val3.x, val3.y, 0.5f), val3.y));
				}
				else
				{
					val5 = MathUtils.Cut(netCourse.m_Curve, ((float3)(ref val3)).xy);
					val6 = default(Bezier4x3);
				}
				val5.a.y = WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, val5.a);
				val5.b.y = WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, val5.b);
				val5.c.y = WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, val5.c);
				val5.d.y = WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, val5.d);
				val5.b.y = math.lerp(val5.a.y, val5.d.y, 1f / 3f);
				val5.c.y = math.lerp(val5.a.y, val5.d.y, 2f / 3f);
				if (flag2)
				{
					val6.a.y = WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, val6.a);
					val6.b.y = WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, val6.b);
					val6.c.y = WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, val6.c);
					val6.d.y = WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, val6.d);
					val6.b.y = math.lerp(val6.a.y, val6.d.y, 1f / 3f);
					val6.c.y = math.lerp(val6.a.y, val6.d.y, 2f / 3f);
				}
				buffer.DrawCurve(guideLineSettings.m_MediumPriorityColor, guideLineSettings.m_MediumPriorityColor, 0f, styleFlags2, val5, netGeometryData.m_DefaultWidth, math.select(float2.op_Implicit(0f), val, new bool2(num3 == 0, !flag2 && flag)));
				if (flag2)
				{
					buffer.DrawCurve(guideLineSettings.m_MediumPriorityColor, guideLineSettings.m_MediumPriorityColor, 0f, styleFlags2, val6, netGeometryData.m_DefaultWidth, new float2(0f, math.select(0f, val.y, flag)));
				}
				num3 = i;
				val3.x = val3.y;
				val4.x = val4.y;
			}
			val3.y = val3.z;
			val4.y = val4.z;
		}
	}

	private static void DrawNetCourse(OverlayRenderSystem.Buffer buffer, NetCourse netCourse, OverlayRenderSystem.StyleFlags styleFlags, NetGeometryData netGeometryData, GuideLineSettingsData guideLineSettings)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		math.select(float2.op_Implicit(0f), float2.op_Implicit(1f), new bool2((netCourse.m_StartPosition.m_Flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsLast)) != 0, (netCourse.m_EndPosition.m_Flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsLast)) != 0));
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
	public GuideLinesSystem()
	{
	}
}
