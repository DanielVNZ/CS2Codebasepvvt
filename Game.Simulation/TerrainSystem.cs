using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Colossal;
using Colossal.AssetPipeline.Native;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.Json;
using Colossal.Mathematics;
using Colossal.Rendering;
using Colossal.Serialization.Entities;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Rendering.Utilities;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Scripting;

namespace Game.Simulation;

[FormerlySerializedAs("Colossal.Terrain.TerrainSystem, Game")]
[CompilerGenerated]
public class TerrainSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	public static class ShaderID
	{
		public static readonly int _BlurTempHorz = Shader.PropertyToID("_BlurTempHorz");

		public static readonly int _AvgTerrainHeightsTemp = Shader.PropertyToID("_AvgTerrainHeightsTemp");

		public static readonly int _DebugSmooth = Shader.PropertyToID("_DebugSmooth");

		public static readonly int _Heightmap = Shader.PropertyToID("_Heightmap");

		public static readonly int _BrushTexture = Shader.PropertyToID("_BrushTexture");

		public static readonly int _WorldTexture = Shader.PropertyToID("_WorldTexture");

		public static readonly int _WaterTexture = Shader.PropertyToID("_WaterTexture");

		public static readonly int _Range = Shader.PropertyToID("_Range");

		public static readonly int _CenterSizeRotation = Shader.PropertyToID("_CenterSizeRotation");

		public static readonly int _Dims = Shader.PropertyToID("_Dims");

		public static readonly int _BrushData = Shader.PropertyToID("_BrushData");

		public static readonly int _BrushData2 = Shader.PropertyToID("_BrushData2");

		public static readonly int _ClampArea = Shader.PropertyToID("_ClampArea");

		public static readonly int _WorldOffsetScale = Shader.PropertyToID("_WorldOffsetScale");

		public static readonly int _EdgeMaxDifference = Shader.PropertyToID("_EdgeMaxDifference");

		public static readonly int _BuildingLotID = Shader.PropertyToID("_BuildingLots");

		public static readonly int _LanesID = Shader.PropertyToID("_Lanes");

		public static readonly int _TrianglesID = Shader.PropertyToID("_Triangles");

		public static readonly int _EdgesID = Shader.PropertyToID("_Edges");

		public static readonly int _HeightmapID = Shader.PropertyToID("_BaseHeightMap");

		public static readonly int _TerrainScaleOffsetID = Shader.PropertyToID("_TerrainScaleOffset");

		public static readonly int _MapOffsetScaleID = Shader.PropertyToID("_MapOffsetScale");

		public static readonly int _BrushID = Shader.PropertyToID("_Brush");

		public static readonly int _CascadeRangesID = Shader.PropertyToID("colossal_TerrainCascadeRanges");

		public static readonly int _CascadeOffsetScale = Shader.PropertyToID("_CascadeOffsetScale");

		public static readonly int _HeightScaleOffset = Shader.PropertyToID("_HeightScaleOffset");

		public static readonly int _RoadData = Shader.PropertyToID("_RoadData");

		public static readonly int _ClipOffset = Shader.PropertyToID("_ClipOffset");
	}

	public struct BuildingLotDraw
	{
		public float2x4 m_HeightsX;

		public float2x4 m_HeightsZ;

		public float3 m_FlatX0;

		public float3 m_FlatZ0;

		public float3 m_FlatX1;

		public float3 m_FlatZ1;

		public float3 m_Position;

		public float3 m_AxisX;

		public float3 m_AxisZ;

		public float2 m_Size;

		public float4 m_MinLimit;

		public float4 m_MaxLimit;

		public float m_Circular;

		public float m_SmoothingWidth;
	}

	public struct LaneSection
	{
		public Bounds2 m_Bounds;

		public float4x3 m_Left;

		public float4x3 m_Right;

		public float3 m_MinOffset;

		public float3 m_MaxOffset;

		public float2 m_ClipOffset;

		public float m_WidthOffset;

		public float m_MiddleSize;

		public LaneFlags m_Flags;
	}

	public struct LaneDraw
	{
		public float4x3 m_Left;

		public float4x3 m_Right;

		public float4 m_MinOffset;

		public float4 m_MaxOffset;

		public float2 m_WidthOffset;
	}

	public struct AreaTriangle
	{
		public float3 m_PositionA;

		public float3 m_PositionB;

		public float3 m_PositionC;

		public float2 m_NoiseSize;

		public float2 m_HeightDelta;
	}

	public struct AreaEdge
	{
		public float2 m_PositionA;

		public float2 m_PositionB;

		public float2 m_Angles;

		public float m_SideOffset;
	}

	[Flags]
	public enum LaneFlags
	{
		ShiftTerrain = 1,
		ClipTerrain = 2,
		MiddleLeft = 4,
		MiddleRight = 8,
		InverseClipOffset = 0x10
	}

	private class CascadeCullInfo
	{
		public JobHandle m_BuildingHandle;

		public NativeList<BuildingLotDraw> m_BuildingRenderList;

		public Material m_LotMaterial;

		public JobHandle m_LaneHandle;

		public NativeList<LaneDraw> m_LaneRenderList;

		public Material m_LaneMaterial;

		public JobHandle m_AreaHandle;

		public NativeList<AreaTriangle> m_TriangleRenderList;

		public NativeList<AreaEdge> m_EdgeRenderList;

		public Material m_AreaMaterial;

		public CascadeCullInfo(Material building, Material lane, Material area)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected O, but got Unknown
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			m_LotMaterial = new Material(building);
			m_LaneMaterial = new Material(lane);
			m_AreaMaterial = new Material(area);
			m_BuildingRenderList = default(NativeList<BuildingLotDraw>);
			m_BuildingHandle = default(JobHandle);
			m_LaneHandle = default(JobHandle);
			m_LaneRenderList = default(NativeList<LaneDraw>);
			m_TriangleRenderList = default(NativeList<AreaTriangle>);
			m_EdgeRenderList = default(NativeList<AreaEdge>);
			m_AreaHandle = default(JobHandle);
		}
	}

	private struct ClipMapDraw
	{
		public float4x3 m_Left;

		public float4x3 m_Right;

		public float m_Height;

		public float m_OffsetFactor;
	}

	private class TerrainMinMaxMap
	{
		private RenderTexture[] m_IntermediateTex;

		private RenderTexture m_DownsampledDetail;

		private RenderTexture m_ResultTex;

		public NativeArray<half4> MinMaxMap;

		private NativeArray<half4> m_UpdateBuffer;

		private AsyncGPUReadbackRequest m_Current;

		private ComputeShader m_Shader;

		private int2 m_IntermediateSize;

		private int2 m_ResultSize;

		private int4 m_UpdatedArea;

		private int4 m_DebugArea;

		private bool m_Pending;

		private bool m_Updated;

		private bool m_Valid;

		private bool m_Partial;

		private int m_Steps;

		private int m_DetailSteps;

		private int m_BlockSize;

		private int m_DetailBlockSize;

		private int m_ID_WorldTexture;

		private int m_ID_DetailTexture;

		private int m_ID_UpdateArea;

		private int m_ID_WorldOffsetScale;

		private int m_ID_DetailOffsetScale;

		private int m_ID_WorldTextureSizeInvSize;

		private int m_ID_Result;

		private int m_KernalCSTerainMinMax;

		private int m_KernalCSWorldTerainMinMax;

		private int m_KernalCSDownsampleMinMax;

		private int2 m_InitValues = int2.zero;

		private Texture m_AsyncNeeded;

		private List<int4> m_UpdatesRequested = new List<int4>();

		private TerrainSystem m_TerrainSystem;

		private JobHandle m_UpdateJob;

		public bool isValid => m_Valid;

		public bool isUpdated => m_Updated;

		public int size => m_ResultSize.x;

		public int4 UpdateArea => m_UpdatedArea;

		private RenderTexture CreateRenderTexture(string name, int2 size, bool compact)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			RenderTexture val = new RenderTexture(size.x, size.y, 0, (GraphicsFormat)(compact ? 46 : 48))
			{
				name = name,
				hideFlags = (HideFlags)52,
				enableRandomWrite = true,
				wrapMode = (TextureWrapMode)1,
				filterMode = (FilterMode)1
			};
			val.Create();
			return val;
		}

		public void Init(int size, int original)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if (m_IntermediateTex != null && size == m_InitValues.x && original == m_InitValues.y)
			{
				((JobHandle)(ref m_UpdateJob)).Complete();
				m_UpdateJob = default(JobHandle);
				return;
			}
			Dispose();
			m_InitValues = new int2(size, original);
			m_IntermediateSize = int2.op_Implicit(original / 2);
			m_ResultSize = int2.op_Implicit(size);
			if (m_ResultSize.x > m_IntermediateSize.x || m_ResultSize.y > m_IntermediateSize.y)
			{
				m_ResultSize = m_IntermediateSize;
				m_Steps = 1;
			}
			else
			{
				m_Steps = math.floorlog2(original) + 1 - (math.floorlog2(size) + 1);
			}
			int num = math.max(math.floorlog2(original) - 2, 1);
			int num2 = (int)math.pow(2f, (float)(num - 1));
			m_DetailSteps = math.floorlog2(original) + 1 - num;
			m_BlockSize = (int)math.pow(2f, (float)m_Steps);
			m_DetailBlockSize = (int)math.pow(2f, (float)m_DetailSteps);
			m_IntermediateTex = (RenderTexture[])(object)new RenderTexture[2];
			m_IntermediateTex[0] = CreateRenderTexture("HeightMinMax_Setup0", m_IntermediateSize, compact: true);
			m_IntermediateTex[1] = CreateRenderTexture("HeightMinMax_Setup1", m_IntermediateSize / 2, compact: true);
			m_DownsampledDetail = CreateRenderTexture("HeightMinMax_Detail", int2.op_Implicit(num2), compact: true);
			m_ResultTex = CreateRenderTexture("HeightMinMax_Result", int2.op_Implicit(m_ResultSize.x), compact: false);
			m_Valid = false;
			m_Partial = false;
			m_Updated = false;
			m_Pending = false;
			MinMaxMap = new NativeArray<half4>(size * size, (Allocator)4, (NativeArrayOptions)1);
			m_UpdateBuffer = new NativeArray<half4>(size * size, (Allocator)4, (NativeArrayOptions)1);
			m_Shader = Resources.Load<ComputeShader>("TerrainMinMax");
			m_KernalCSTerainMinMax = m_Shader.FindKernel("CSTerainGenerateMinMax");
			m_KernalCSWorldTerainMinMax = m_Shader.FindKernel("CSTerainWorldGenerateMinMax");
			m_KernalCSDownsampleMinMax = m_Shader.FindKernel("CSDownsampleMinMax");
			m_ID_WorldTexture = Shader.PropertyToID("_WorldTexture");
			m_ID_DetailTexture = Shader.PropertyToID("_DetailHeightTexture");
			m_ID_UpdateArea = Shader.PropertyToID("_UpdateArea");
			m_ID_WorldOffsetScale = Shader.PropertyToID("_WorldOffsetScale");
			m_ID_DetailOffsetScale = Shader.PropertyToID("_DetailOffsetScale");
			m_ID_WorldTextureSizeInvSize = Shader.PropertyToID("_WorldTextureSizeInvSize");
			m_ID_Result = Shader.PropertyToID("ResultMinMax");
		}

		public void Debug(TerrainSystem terrain, Texture map, Texture worldMap)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			CommandBuffer val = new CommandBuffer
			{
				name = "DebugMinMax"
			};
			try
			{
				val.SetExecutionFlags((CommandBufferExecutionFlags)2);
				RequestUpdate(terrain, map, worldMap, m_DebugArea, val, debug: true);
				Graphics.ExecuteCommandBuffer(val);
				val.Dispose();
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		public void UpdateMap(TerrainSystem terrain, Texture map, Texture worldMap)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected O, but got Unknown
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			m_Valid = false;
			m_Updated = false;
			m_Partial = false;
			((JobHandle)(ref m_UpdateJob)).Complete();
			m_UpdateJob = default(JobHandle);
			if (m_Pending && !((AsyncGPUReadbackRequest)(ref m_Current)).done)
			{
				((AsyncGPUReadbackRequest)(ref m_Current)).WaitForCompletion();
			}
			CommandBuffer val = new CommandBuffer
			{
				name = "TerrainMinMaxInit"
			};
			try
			{
				m_AsyncNeeded = RequestUpdate(terrain, map, worldMap, new int4(0, 0, map.width, map.height), val);
				Graphics.ExecuteCommandBuffer(val);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			m_Pending = true;
		}

		private int4 RemapArea(int4 area, int blockSize, int textureWidth, int textureHeight)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			int2 val = ((int4)(ref area)).xy / new int2(blockSize, blockSize) * new int2(blockSize, blockSize);
			((int4)(ref area)).zw = ((int4)(ref area)).zw + (((int4)(ref area)).xy - val);
			((int4)(ref area)).xy = val;
			((int4)(ref area)).zw = (((int4)(ref area)).zw + new int2(blockSize - 1, blockSize - 1)) / new int2(blockSize, blockSize) * new int2(blockSize, blockSize);
			if (area.z > textureWidth)
			{
				area.z = textureWidth;
			}
			if (area.x + area.z > textureWidth)
			{
				area.x = textureWidth - area.z;
			}
			if (area.w > textureHeight)
			{
				area.w = textureHeight;
			}
			if (area.y + area.w > textureHeight)
			{
				area.y = textureHeight - area.w;
			}
			return area;
		}

		public bool RequestUpdate(TerrainSystem terrain, Texture map, Texture worldMap, int4 area)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Expected O, but got Unknown
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			if (m_Pending || m_Updated)
			{
				m_UpdatesRequested.Add(area);
				m_TerrainSystem = terrain;
				return false;
			}
			int2 val = ((int4)(ref area)).xy / new int2(m_BlockSize, m_BlockSize) * new int2(m_BlockSize, m_BlockSize);
			((int4)(ref area)).zw = ((int4)(ref area)).zw + (((int4)(ref area)).xy - val);
			((int4)(ref area)).xy = val;
			((int4)(ref area)).zw = (((int4)(ref area)).zw + new int2(m_BlockSize - 1, m_BlockSize - 1)) / new int2(m_BlockSize, m_BlockSize) * new int2(m_BlockSize, m_BlockSize);
			if (area.z > map.width)
			{
				area.z = map.width;
			}
			if (area.x + area.z > map.width)
			{
				area.x = map.width - area.z;
			}
			if (area.w > map.height)
			{
				area.w = map.height;
			}
			if (area.y + area.w > map.height)
			{
				area.y = map.height - area.w;
			}
			area = RemapArea(area, m_BlockSize, ((Object)(object)worldMap != (Object)null) ? worldMap.width : map.width, ((Object)(object)worldMap != (Object)null) ? worldMap.height : map.height);
			CommandBuffer val2 = new CommandBuffer
			{
				name = "TerainMinMaxUpdate"
			};
			try
			{
				val2.SetExecutionFlags((CommandBufferExecutionFlags)2);
				m_AsyncNeeded = RequestUpdate(terrain, map, worldMap, area, val2);
				m_Pending = true;
				m_Partial = true;
				Graphics.ExecuteCommandBuffer(val2);
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			return true;
		}

		private bool Downsample(CommandBuffer commandBuffer, Texture target, int steps, int4 area, ref int4 updated)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			if (steps == 1)
			{
				return false;
			}
			float4 val = default(float4);
			((float4)(ref val))._002Ector((float)area.x, (float)area.y, (float)(area.x / 2), (float)(area.y / 2));
			int num = 1;
			int2 val2 = ((int4)(ref area)).zw / 2;
			int4 val3 = area / 2;
			((int4)(ref val3)).zw = math.max(((int4)(ref val3)).zw, new int2(1, 1));
			((int2)(ref val2)).xy = math.max(((int2)(ref val2)).xy, new int2(1, 1));
			updated = area / 2;
			((int4)(ref updated)).zw = math.max(((int4)(ref updated)).zw, new int2(1, 1));
			Texture val4 = (Texture)(object)m_IntermediateTex[1];
			Texture val5 = (Texture)(object)m_IntermediateTex[0];
			do
			{
				Texture obj = val5;
				val5 = val4;
				val4 = obj;
				if (num == steps - 1)
				{
					val5 = target;
				}
				((float4)(ref val)).xy = float2.op_Implicit(((int4)(ref val3)).xy);
				val3 /= 2;
				((int4)(ref val3)).zw = math.max(((int4)(ref val3)).zw, new int2(1, 1));
				((float4)(ref val)).zw = float2.op_Implicit(((int4)(ref val3)).xy);
				val2 /= 2;
				((int2)(ref val2)).xy = math.max(((int2)(ref val2)).xy, new int2(1, 1));
				updated /= 2;
				((int4)(ref updated)).zw = math.max(((int4)(ref updated)).zw, new int2(1, 1));
				commandBuffer.SetComputeVectorParam(m_Shader, m_ID_UpdateArea, float4.op_Implicit(val));
				commandBuffer.SetComputeTextureParam(m_Shader, m_KernalCSDownsampleMinMax, m_ID_WorldTexture, RenderTargetIdentifier.op_Implicit(val4));
				commandBuffer.SetComputeTextureParam(m_Shader, m_KernalCSDownsampleMinMax, m_ID_Result, RenderTargetIdentifier.op_Implicit(val5));
				commandBuffer.DispatchCompute(m_Shader, m_KernalCSDownsampleMinMax, (val2.x + 7) / 8, (val2.y + 7) / 8, 1);
			}
			while (++num < steps);
			return true;
		}

		private Texture RequestUpdate(TerrainSystem terrain, Texture map, Texture worldMap, int4 area, CommandBuffer commandBuffer, bool debug = false)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			if (!debug)
			{
				m_DebugArea = area;
			}
			bool num = (Object)(object)worldMap != (Object)null;
			float4 val = default(float4);
			((float4)(ref val))._002Ector((float)area.x, (float)area.y, (float)(area.x / 2), (float)(area.y / 2));
			float4 val2 = default(float4);
			((float4)(ref val2))._002Ector(terrain.heightScaleOffset.y, terrain.heightScaleOffset.x, 0f, 0f);
			commandBuffer.SetComputeVectorParam(m_Shader, m_ID_WorldOffsetScale, float4.op_Implicit(val2));
			int4 updated = int4.zero;
			if (num)
			{
				float4 val3 = default(float4);
				((float4)(ref val3))._002Ector((terrain.worldOffset - terrain.playableOffset) / terrain.playableArea, 1f / (float)worldMap.width * (terrain.worldSize / terrain.playableArea));
				float4 val4 = default(float4);
				((float4)(ref val4))._002Ector(float2.op_Implicit(((int4)(ref area)).xy) * ((float4)(ref val3)).zw + ((float4)(ref val3)).xy, float2.op_Implicit(((int4)(ref area)).xy + ((int4)(ref area)).zw) * ((float4)(ref val3)).zw + ((float4)(ref val3)).xy);
				if (!(val4.x > 1f) && !(val4.z < 0f) && !(val4.y > 1f) && !(val4.w < 0f))
				{
					val4 = math.clamp(val4, float4.zero, new float4(1f, 1f, 1f, 1f));
					((float4)(ref val4)).zw = ((float4)(ref val4)).zw - ((float4)(ref val4)).xy;
					((float4)(ref val4)).xy = math.floor(((float4)(ref val4)).xy * new float2((float)map.width, (float)map.height));
					((float4)(ref val4)).zw = math.max(math.ceil(((float4)(ref val4)).zw * new float2((float)map.width, (float)map.height)), new float2(1f, 1f));
					int4 val5 = RemapArea(new int4((int)val4.x, (int)val4.y, (int)val4.z, (int)val4.w), m_DetailBlockSize, map.width, map.height);
					float4 val6 = default(float4);
					((float4)(ref val6))._002Ector(val4.x, val4.y, val4.x / 2f, val4.y / 2f);
					commandBuffer.SetComputeVectorParam(m_Shader, m_ID_UpdateArea, float4.op_Implicit(val6));
					commandBuffer.SetComputeTextureParam(m_Shader, m_KernalCSTerainMinMax, m_ID_WorldTexture, RenderTargetIdentifier.op_Implicit(map));
					commandBuffer.SetComputeTextureParam(m_Shader, m_KernalCSTerainMinMax, m_ID_Result, RenderTargetIdentifier.op_Implicit((Texture)(object)m_IntermediateTex[0]));
					commandBuffer.DispatchCompute(m_Shader, m_KernalCSTerainMinMax, (val5.z + 7) / 8, (val5.w + 7) / 8, 1);
					Downsample(commandBuffer, (Texture)(object)m_DownsampledDetail, m_DetailSteps, val5, ref updated);
				}
				float4 val7 = default(float4);
				((float4)(ref val7))._002Ector((float)((Texture)m_DownsampledDetail).width, (float)((Texture)m_DownsampledDetail).height, 1f / (float)((Texture)m_DownsampledDetail).width, 1f / (float)((Texture)m_DownsampledDetail).height);
				commandBuffer.SetComputeVectorParam(m_Shader, m_ID_WorldTextureSizeInvSize, float4.op_Implicit(val7));
				commandBuffer.SetComputeVectorParam(m_Shader, m_ID_DetailOffsetScale, float4.op_Implicit(val3));
				commandBuffer.SetComputeVectorParam(m_Shader, m_ID_UpdateArea, float4.op_Implicit(val));
				commandBuffer.SetComputeTextureParam(m_Shader, m_KernalCSWorldTerainMinMax, m_ID_WorldTexture, RenderTargetIdentifier.op_Implicit(worldMap));
				commandBuffer.SetComputeTextureParam(m_Shader, m_KernalCSWorldTerainMinMax, m_ID_DetailTexture, RenderTargetIdentifier.op_Implicit((Texture)(object)m_DownsampledDetail));
				commandBuffer.SetComputeTextureParam(m_Shader, m_KernalCSWorldTerainMinMax, m_ID_Result, RenderTargetIdentifier.op_Implicit((Texture)(object)((m_Steps == 1) ? m_ResultTex : m_IntermediateTex[0])));
				commandBuffer.DispatchCompute(m_Shader, m_KernalCSWorldTerainMinMax, (area.z + 7) / 8, (area.w + 7) / 8, 1);
			}
			else
			{
				commandBuffer.SetComputeVectorParam(m_Shader, m_ID_UpdateArea, float4.op_Implicit(val));
				commandBuffer.SetComputeTextureParam(m_Shader, m_KernalCSTerainMinMax, m_ID_WorldTexture, RenderTargetIdentifier.op_Implicit(map));
				commandBuffer.SetComputeTextureParam(m_Shader, m_KernalCSTerainMinMax, m_ID_Result, RenderTargetIdentifier.op_Implicit((Texture)(object)((m_Steps == 1) ? m_ResultTex : m_IntermediateTex[0])));
				commandBuffer.DispatchCompute(m_Shader, m_KernalCSTerainMinMax, (area.z + 7) / 8, (area.w + 7) / 8, 1);
			}
			if (!debug)
			{
				m_UpdatedArea = area / 2;
				((int4)(ref m_UpdatedArea)).zw = math.max(((int4)(ref m_UpdatedArea)).zw, new int2(1, 1));
			}
			Downsample(commandBuffer, (Texture)(object)m_ResultTex, m_Steps, area, ref updated);
			if (!debug)
			{
				m_UpdatedArea = updated;
			}
			return (Texture)(object)m_ResultTex;
		}

		public unsafe void Update()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			((JobHandle)(ref m_UpdateJob)).Complete();
			m_UpdateJob = default(JobHandle);
			if (m_Pending)
			{
				if ((Object)(object)m_AsyncNeeded != (Object)null)
				{
					if (m_Partial)
					{
						m_Current = AsyncGPUReadback.RequestIntoNativeArray<half4>(ref m_UpdateBuffer, m_AsyncNeeded, 0, m_UpdatedArea.x, m_UpdatedArea.z, m_UpdatedArea.y, m_UpdatedArea.w, 0, 1, (GraphicsFormat)48, (Action<AsyncGPUReadbackRequest>)delegate(AsyncGPUReadbackRequest request)
						{
							//IL_0055: Unknown result type (might be due to invalid IL or missing references)
							//IL_006a: Unknown result type (might be due to invalid IL or missing references)
							m_Pending = false;
							if (!((AsyncGPUReadbackRequest)(ref request)).hasError)
							{
								m_Valid = true;
								m_Updated = true;
								if (m_Partial)
								{
									int num2 = m_UpdatedArea.y * m_ResultSize.x + m_UpdatedArea.x;
									for (int i = 0; i < m_UpdatedArea.w; i++)
									{
										UnsafeUtility.MemCpy((void*)((byte*)NativeArrayUnsafeUtility.GetUnsafePtr<half4>(MinMaxMap) + (nint)num2 * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<float2>()), (void*)((byte*)NativeArrayUnsafeUtility.GetUnsafePtr<half4>(m_UpdateBuffer) + (nint)(i * m_UpdatedArea.z) * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<float2>()), (long)(8 * m_UpdatedArea.z));
										num2 += m_ResultSize.x;
									}
									m_Partial = false;
								}
							}
						});
					}
					else
					{
						m_Current = AsyncGPUReadback.RequestIntoNativeArray<half4>(ref MinMaxMap, m_AsyncNeeded, 0, 0, m_ResultSize.x, 0, m_ResultSize.y, 0, 1, (GraphicsFormat)48, (Action<AsyncGPUReadbackRequest>)delegate(AsyncGPUReadbackRequest request)
						{
							//IL_0055: Unknown result type (might be due to invalid IL or missing references)
							//IL_006a: Unknown result type (might be due to invalid IL or missing references)
							m_Pending = false;
							if (!((AsyncGPUReadbackRequest)(ref request)).hasError)
							{
								m_Valid = true;
								m_Updated = true;
								if (m_Partial)
								{
									int num2 = m_UpdatedArea.y * m_ResultSize.x + m_UpdatedArea.x;
									for (int i = 0; i < m_UpdatedArea.w; i++)
									{
										UnsafeUtility.MemCpy((void*)((byte*)NativeArrayUnsafeUtility.GetUnsafePtr<half4>(MinMaxMap) + (nint)num2 * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<float2>()), (void*)((byte*)NativeArrayUnsafeUtility.GetUnsafePtr<half4>(m_UpdateBuffer) + (nint)(i * m_UpdatedArea.z) * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<float2>()), (long)(8 * m_UpdatedArea.z));
										num2 += m_ResultSize.x;
									}
									m_Partial = false;
								}
							}
						});
					}
					m_AsyncNeeded = null;
				}
				else
				{
					((AsyncGPUReadbackRequest)(ref m_Current)).Update();
				}
			}
			else if (!m_Updated && m_UpdatesRequested.Count > 0)
			{
				int4 area = m_UpdatesRequested[0];
				((int4)(ref area)).zw = ((int4)(ref area)).zw + ((int4)(ref area)).xy;
				for (int num = 1; num < m_UpdatesRequested.Count; num++)
				{
					int2 xy = ((int4)(ref area)).xy;
					int4 val = m_UpdatesRequested[num];
					((int4)(ref area)).xy = math.min(xy, ((int4)(ref val)).xy);
					int2 zw = ((int4)(ref area)).zw;
					val = m_UpdatesRequested[num];
					int2 xy2 = ((int4)(ref val)).xy;
					val = m_UpdatesRequested[num];
					((int4)(ref area)).zw = math.max(zw, xy2 + ((int4)(ref val)).zw);
				}
				((int4)(ref area)).zw = ((int4)(ref area)).zw - ((int4)(ref area)).xy;
				((int4)(ref area)).zw = math.clamp(((int4)(ref area)).zw, new int2(1, 1), new int2(m_ResultSize.x, m_ResultSize.y));
				RequestUpdate(m_TerrainSystem, m_TerrainSystem.heightmap, m_TerrainSystem.worldHeightmap, area);
				m_TerrainSystem = null;
				m_UpdatesRequested.Clear();
			}
		}

		private unsafe void UpdateMinMax(AsyncGPUReadbackRequest request)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			m_Pending = false;
			if (((AsyncGPUReadbackRequest)(ref request)).hasError)
			{
				return;
			}
			m_Valid = true;
			m_Updated = true;
			if (m_Partial)
			{
				int num = m_UpdatedArea.y * m_ResultSize.x + m_UpdatedArea.x;
				for (int i = 0; i < m_UpdatedArea.w; i++)
				{
					UnsafeUtility.MemCpy((void*)((byte*)NativeArrayUnsafeUtility.GetUnsafePtr<half4>(MinMaxMap) + (nint)num * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<float2>()), (void*)((byte*)NativeArrayUnsafeUtility.GetUnsafePtr<half4>(m_UpdateBuffer) + (nint)(i * m_UpdatedArea.z) * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<float2>()), (long)(8 * m_UpdatedArea.z));
					num += m_ResultSize.x;
				}
				m_Partial = false;
			}
		}

		public int4 ComsumeUpdate()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			m_Updated = false;
			return m_UpdatedArea;
		}

		public float2 GetMinMax(int4 area)
		{
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			float2 val = default(float2);
			((float2)(ref val))._002Ector(999999f, 0f);
			for (int i = 0; i < area.z * area.w; i++)
			{
				int num = (area.y + i / area.z) * m_ResultSize.x + area.x + i % area.z;
				val.x = math.min(val.x, half.op_Implicit(MinMaxMap[num].x));
				val.y = math.max(val.y, half.op_Implicit(MinMaxMap[num].y));
			}
			return val;
		}

		public void RegisterJobUpdate(JobHandle handle)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			m_UpdateJob = JobHandle.CombineDependencies(handle, m_UpdateJob);
		}

		public void Dispose()
		{
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			((AsyncGPUReadbackRequest)(ref m_Current)).WaitForCompletion();
			m_Pending = false;
			m_AsyncNeeded = null;
			m_Updated = false;
			if (m_IntermediateTex != null)
			{
				RenderTexture[] array = m_IntermediateTex;
				for (int i = 0; i < array.Length; i++)
				{
					CoreUtils.Destroy((Object)(object)array[i]);
				}
			}
			CoreUtils.Destroy((Object)(object)m_DownsampledDetail);
			CoreUtils.Destroy((Object)(object)m_ResultTex);
			if (MinMaxMap.IsCreated)
			{
				MinMaxMap.Dispose(m_UpdateJob);
			}
			if (m_UpdateBuffer.IsCreated)
			{
				m_UpdateBuffer.Dispose();
			}
			m_UpdateJob = default(JobHandle);
		}
	}

	private class TerrainDesc
	{
		public Hash128 heightMapGuid { get; set; }

		public Hash128 diffuseMapGuid { get; set; }

		public float heightScale { get; set; }

		public float heightOffset { get; set; }

		public Hash128 worldHeightMapGuid { get; set; }

		public float2 mapSize { get; set; }

		public float2 worldSize { get; set; }

		public float2 worldHeightMinMax { get; set; }

		private static void SupportValueTypesForAOT()
		{
			JSON.SupportTypeForAOT<float2>();
		}
	}

	[BurstCompile]
	private struct CullBuildingLotsJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Lot> m_LotHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.Elevation> m_ElevationHandle;

		[ReadOnly]
		public ComponentTypeHandle<Stack> m_StackHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeHandle;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<AssetStampData> m_PrefabAssetStampData;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> m_PrefabBuildingExtensionData;

		[ReadOnly]
		public ComponentLookup<BuildingTerraformData> m_OverrideTerraform;

		[ReadOnly]
		public BufferLookup<AdditionalBuildingTerraformElement> m_AdditionalLots;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

		[ReadOnly]
		public float4 m_Area;

		public ParallelWriter<BuildingUtils.LotInfo> Result;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0546: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05de: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_060c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Unknown result type (might be due to invalid IL or missing references)
			//IL_061a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_062b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0630: Unknown result type (might be due to invalid IL or missing references)
			//IL_0635: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0645: Unknown result type (might be due to invalid IL or missing references)
			//IL_064f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0654: Unknown result type (might be due to invalid IL or missing references)
			//IL_0659: Unknown result type (might be due to invalid IL or missing references)
			//IL_0662: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_0673: Unknown result type (might be due to invalid IL or missing references)
			//IL_0678: Unknown result type (might be due to invalid IL or missing references)
			//IL_067d: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06be: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06db: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_072a: Unknown result type (might be due to invalid IL or missing references)
			//IL_072f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0737: Unknown result type (might be due to invalid IL or missing references)
			//IL_0748: Unknown result type (might be due to invalid IL or missing references)
			//IL_076d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0784: Unknown result type (might be due to invalid IL or missing references)
			//IL_078b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0792: Unknown result type (might be due to invalid IL or missing references)
			//IL_0797: Unknown result type (might be due to invalid IL or missing references)
			//IL_079c: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0802: Unknown result type (might be due to invalid IL or missing references)
			//IL_0807: Unknown result type (might be due to invalid IL or missing references)
			//IL_080e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0813: Unknown result type (might be due to invalid IL or missing references)
			//IL_0855: Unknown result type (might be due to invalid IL or missing references)
			//IL_085c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0861: Unknown result type (might be due to invalid IL or missing references)
			//IL_0866: Unknown result type (might be due to invalid IL or missing references)
			//IL_086f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0874: Unknown result type (might be due to invalid IL or missing references)
			//IL_0882: Unknown result type (might be due to invalid IL or missing references)
			//IL_088e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0898: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08de: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0900: Unknown result type (might be due to invalid IL or missing references)
			//IL_090c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0911: Unknown result type (might be due to invalid IL or missing references)
			//IL_091a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0929: Unknown result type (might be due to invalid IL or missing references)
			//IL_0935: Unknown result type (might be due to invalid IL or missing references)
			//IL_093a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0954: Unknown result type (might be due to invalid IL or missing references)
			//IL_0997: Unknown result type (might be due to invalid IL or missing references)
			//IL_099c: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09be: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a20: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a25: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abd: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Game.Buildings.Lot> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.Lot>(ref m_LotHandle);
			NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformHandle);
			NativeArray<Game.Objects.Elevation> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Objects.Elevation>(ref m_ElevationHandle);
			NativeArray<Stack> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Stack>(ref m_StackHandle);
			NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefHandle);
			BufferAccessor<InstalledUpgrade> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeHandle);
			DynamicBuffer<AdditionalBuildingTerraformElement> val3 = default(DynamicBuffer<AdditionalBuildingTerraformElement>);
			BuildingExtensionData buildingExtensionData2 = default(BuildingExtensionData);
			BuildingTerraformData buildingTerraformData = default(BuildingTerraformData);
			ObjectGeometryData objectGeometryData2 = default(ObjectGeometryData);
			DynamicBuffer<AdditionalBuildingTerraformElement> val5 = default(DynamicBuffer<AdditionalBuildingTerraformElement>);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				PrefabRef prefabRef = nativeArray5[i];
				Transform transform = nativeArray2[i];
				Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
				if (nativeArray3.Length != 0)
				{
					elevation = nativeArray3[i];
				}
				Game.Buildings.Lot lot = default(Game.Buildings.Lot);
				if (nativeArray.Length != 0)
				{
					lot = nativeArray[i];
				}
				bool flag = m_PrefabBuildingData.HasComponent(prefabRef.m_Prefab);
				bool flag2 = !flag && m_PrefabBuildingExtensionData.HasComponent(prefabRef.m_Prefab);
				bool flag3 = !flag && !flag2 && m_PrefabAssetStampData.HasComponent(prefabRef.m_Prefab);
				bool flag4 = !flag && !flag2 && !flag3 && m_ObjectGeometryData.HasComponent(prefabRef.m_Prefab);
				if (!(flag || flag2 || flag3 || flag4))
				{
					continue;
				}
				ObjectGeometryData objectGeometryData = m_ObjectGeometryData[prefabRef.m_Prefab];
				Bounds3 val = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, objectGeometryData);
				Bounds2 xz = ((Bounds3)(ref val)).xz;
				float2 val2;
				if (flag)
				{
					val2 = new float2(m_PrefabBuildingData[prefabRef.m_Prefab].m_LotSize) * 4f;
				}
				else if (flag2)
				{
					BuildingExtensionData buildingExtensionData = m_PrefabBuildingExtensionData[prefabRef.m_Prefab];
					if (!buildingExtensionData.m_External)
					{
						continue;
					}
					val2 = new float2(buildingExtensionData.m_LotSize) * 4f;
				}
				else if (flag3)
				{
					val2 = new float2(m_PrefabAssetStampData[prefabRef.m_Prefab].m_Size) * 4f;
				}
				else
				{
					if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Standing) != Game.Objects.GeometryFlags.None)
					{
						val2 = ((float3)(ref objectGeometryData.m_LegSize)).xz * 0.5f + objectGeometryData.m_LegOffset;
					}
					else
					{
						ref float3 position = ref transform.m_Position;
						((float3)(ref position)).xz = ((float3)(ref position)).xz + MathUtils.Center(((Bounds3)(ref objectGeometryData.m_Bounds)).xz);
						val2 = MathUtils.Size(((Bounds3)(ref objectGeometryData.m_Bounds)).xz) * 0.5f;
					}
					if (nativeArray4.Length != 0)
					{
						Stack stack = nativeArray4[i];
						transform.m_Position.y += stack.m_Range.min - objectGeometryData.m_Bounds.min.y;
					}
				}
				xz = MathUtils.Expand(xz, float2.op_Implicit(ObjectUtils.GetTerrainSmoothingWidth(objectGeometryData) - 8f));
				if (xz.max.x < m_Area.x || xz.min.x > m_Area.z || xz.max.y < m_Area.y || xz.min.y > m_Area.w)
				{
					continue;
				}
				DynamicBuffer<InstalledUpgrade> upgrades = default(DynamicBuffer<InstalledUpgrade>);
				if (bufferAccessor.Length != 0)
				{
					upgrades = bufferAccessor[i];
				}
				bool hasExtensionLots;
				BuildingUtils.LotInfo lotInfo = BuildingUtils.CalculateLotInfo(val2, transform, elevation, lot, prefabRef, upgrades, m_TransformData, m_PrefabRefData, m_ObjectGeometryData, m_OverrideTerraform, m_PrefabBuildingExtensionData, flag4, out hasExtensionLots);
				float terrainSmoothingWidth = ObjectUtils.GetTerrainSmoothingWidth(val2 * 2f);
				lotInfo.m_Radius += terrainSmoothingWidth;
				Result.Enqueue(lotInfo);
				if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Standing) != Game.Objects.GeometryFlags.None)
				{
					BuildingUtils.LotInfo lotInfo2 = lotInfo;
					lotInfo2.m_Extents = MathUtils.Size(((Bounds3)(ref objectGeometryData.m_Bounds)).xz) * 0.5f;
					float terrainSmoothingWidth2 = ObjectUtils.GetTerrainSmoothingWidth(lotInfo2.m_Extents * 2f);
					ref float3 position2 = ref lotInfo2.m_Position;
					((float3)(ref position2)).xz = ((float3)(ref position2)).xz + MathUtils.Center(((Bounds3)(ref objectGeometryData.m_Bounds)).xz);
					lotInfo2.m_Position.y += objectGeometryData.m_LegSize.y;
					lotInfo2.m_MaxLimit = new float4(terrainSmoothingWidth2, terrainSmoothingWidth2, 0f - terrainSmoothingWidth2, 0f - terrainSmoothingWidth2);
					lotInfo2.m_MinLimit = new float4(-((float2)(ref lotInfo2.m_Extents)).xy, ((float2)(ref lotInfo2.m_Extents)).xy);
					lotInfo2.m_FrontHeights = default(float3);
					lotInfo2.m_RightHeights = default(float3);
					lotInfo2.m_BackHeights = default(float3);
					lotInfo2.m_LeftHeights = default(float3);
					lotInfo2.m_FlatX0 = float3.op_Implicit(lotInfo2.m_MinLimit.x * 0.5f);
					lotInfo2.m_FlatZ0 = float3.op_Implicit(lotInfo2.m_MinLimit.y * 0.5f);
					lotInfo2.m_FlatX1 = float3.op_Implicit(lotInfo2.m_MinLimit.z * 0.5f);
					lotInfo2.m_FlatZ1 = float3.op_Implicit(lotInfo2.m_MinLimit.w * 0.5f);
					lotInfo2.m_Radius = math.length(lotInfo2.m_Extents) + terrainSmoothingWidth2;
					lotInfo2.m_Circular = math.select(0f, 1f, (objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) != 0);
					Result.Enqueue(lotInfo2);
				}
				if (m_AdditionalLots.TryGetBuffer(prefabRef.m_Prefab, ref val3))
				{
					for (int j = 0; j < val3.Length; j++)
					{
						AdditionalBuildingTerraformElement additionalBuildingTerraformElement = val3[j];
						BuildingUtils.LotInfo lotInfo3 = lotInfo;
						lotInfo3.m_Position.y += additionalBuildingTerraformElement.m_HeightOffset;
						lotInfo3.m_MinLimit = new float4(additionalBuildingTerraformElement.m_Area.min, additionalBuildingTerraformElement.m_Area.max);
						lotInfo3.m_FlatX0 = math.max(lotInfo3.m_FlatX0, float3.op_Implicit(lotInfo3.m_MinLimit.x));
						lotInfo3.m_FlatZ0 = math.max(lotInfo3.m_FlatZ0, float3.op_Implicit(lotInfo3.m_MinLimit.y));
						lotInfo3.m_FlatX1 = math.min(lotInfo3.m_FlatX1, float3.op_Implicit(lotInfo3.m_MinLimit.z));
						lotInfo3.m_FlatZ1 = math.min(lotInfo3.m_FlatZ1, float3.op_Implicit(lotInfo3.m_MinLimit.w));
						lotInfo3.m_Circular = math.select(0f, 1f, additionalBuildingTerraformElement.m_Circular);
						lotInfo3.m_MaxLimit = math.select(lotInfo3.m_MinLimit, new float4(terrainSmoothingWidth, terrainSmoothingWidth, 0f - terrainSmoothingWidth, 0f - terrainSmoothingWidth), additionalBuildingTerraformElement.m_DontRaise);
						lotInfo3.m_MinLimit = math.select(lotInfo3.m_MinLimit, new float4(terrainSmoothingWidth, terrainSmoothingWidth, 0f - terrainSmoothingWidth, 0f - terrainSmoothingWidth), additionalBuildingTerraformElement.m_DontLower);
						Result.Enqueue(lotInfo3);
					}
				}
				if (!hasExtensionLots)
				{
					continue;
				}
				for (int k = 0; k < upgrades.Length; k++)
				{
					Entity upgrade = upgrades[k].m_Upgrade;
					PrefabRef prefabRef2 = m_PrefabRefData[upgrade];
					if (!m_PrefabBuildingExtensionData.TryGetComponent(prefabRef2.m_Prefab, ref buildingExtensionData2) || buildingExtensionData2.m_External || !m_OverrideTerraform.TryGetComponent(prefabRef2.m_Prefab, ref buildingTerraformData))
					{
						continue;
					}
					float3 val4 = m_TransformData[upgrade].m_Position - transform.m_Position;
					float num = 0f;
					if (m_ObjectGeometryData.TryGetComponent(prefabRef2.m_Prefab, ref objectGeometryData2))
					{
						bool flag5 = (objectGeometryData2.m_Flags & Game.Objects.GeometryFlags.Standing) != 0;
						bool flag6 = ((uint)objectGeometryData2.m_Flags & (uint)((!flag5) ? 1 : 256)) != 0;
						num = math.select(0f, 1f, flag6);
					}
					if (!math.all(buildingTerraformData.m_Smooth + ((float3)(ref val4)).xzxz == lotInfo.m_MaxLimit) || num != lotInfo.m_Circular)
					{
						BuildingUtils.LotInfo lotInfo4 = lotInfo;
						lotInfo4.m_Circular = num;
						lotInfo4.m_Position.y += buildingTerraformData.m_HeightOffset;
						lotInfo4.m_MinLimit = buildingTerraformData.m_Smooth + ((float3)(ref val4)).xzxz;
						lotInfo4.m_MaxLimit = lotInfo4.m_MinLimit;
						((float4)(ref lotInfo4.m_MinLimit)).xy = math.min(new float2(lotInfo4.m_FlatX0.y, lotInfo4.m_FlatZ0.y), ((float4)(ref lotInfo4.m_MinLimit)).xy);
						((float4)(ref lotInfo4.m_MinLimit)).zw = math.max(new float2(lotInfo4.m_FlatX1.y, lotInfo4.m_FlatZ1.y), ((float4)(ref lotInfo4.m_MinLimit)).zw);
						lotInfo4.m_MinLimit = math.select(lotInfo4.m_MinLimit, new float4(terrainSmoothingWidth, terrainSmoothingWidth, 0f - terrainSmoothingWidth, 0f - terrainSmoothingWidth), buildingTerraformData.m_DontLower);
						lotInfo4.m_MaxLimit = math.select(lotInfo4.m_MaxLimit, new float4(terrainSmoothingWidth, terrainSmoothingWidth, 0f - terrainSmoothingWidth, 0f - terrainSmoothingWidth), buildingTerraformData.m_DontRaise);
						Result.Enqueue(lotInfo4);
					}
					if (m_AdditionalLots.TryGetBuffer(prefabRef2.m_Prefab, ref val5))
					{
						for (int l = 0; l < val5.Length; l++)
						{
							AdditionalBuildingTerraformElement additionalBuildingTerraformElement2 = val5[l];
							BuildingUtils.LotInfo lotInfo5 = lotInfo;
							lotInfo5.m_Position.y += additionalBuildingTerraformElement2.m_HeightOffset;
							lotInfo5.m_MinLimit = new float4(additionalBuildingTerraformElement2.m_Area.min, additionalBuildingTerraformElement2.m_Area.max) + ((float3)(ref val4)).xzxz;
							lotInfo5.m_FlatX0 = math.max(lotInfo5.m_FlatX0, float3.op_Implicit(lotInfo5.m_MinLimit.x));
							lotInfo5.m_FlatZ0 = math.max(lotInfo5.m_FlatZ0, float3.op_Implicit(lotInfo5.m_MinLimit.y));
							lotInfo5.m_FlatX1 = math.min(lotInfo5.m_FlatX1, float3.op_Implicit(lotInfo5.m_MinLimit.z));
							lotInfo5.m_FlatZ1 = math.min(lotInfo5.m_FlatZ1, float3.op_Implicit(lotInfo5.m_MinLimit.w));
							lotInfo5.m_Circular = math.select(0f, 1f, additionalBuildingTerraformElement2.m_Circular);
							lotInfo5.m_MaxLimit = math.select(lotInfo5.m_MinLimit, new float4(terrainSmoothingWidth, terrainSmoothingWidth, 0f - terrainSmoothingWidth, 0f - terrainSmoothingWidth), additionalBuildingTerraformElement2.m_DontRaise);
							lotInfo5.m_MinLimit = math.select(lotInfo5.m_MinLimit, new float4(terrainSmoothingWidth, terrainSmoothingWidth, 0f - terrainSmoothingWidth, 0f - terrainSmoothingWidth), additionalBuildingTerraformElement2.m_DontLower);
							Result.Enqueue(lotInfo5);
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

	[BurstCompile]
	private struct DequeBuildingLotsJob : IJob
	{
		[ReadOnly]
		public NativeQueue<BuildingUtils.LotInfo> m_Queue;

		public NativeList<BuildingUtils.LotInfo> m_List;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<BuildingUtils.LotInfo> val = m_Queue.ToArray(AllocatorHandle.op_Implicit((Allocator)2));
			m_List.CopyFrom(ref val);
			val.Dispose();
		}
	}

	[BurstCompile]
	private struct CullRoadsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityHandle;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<Orphan> m_OrphanData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<NodeGeometry> m_NodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetData> m_NetData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_NetGeometryData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<TerrainComposition> m_TerrainCompositionData;

		[ReadOnly]
		public float4 m_Area;

		public ParallelWriter<LaneSection> Result;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityHandle);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				if (!m_PrefabRefData.HasComponent(val))
				{
					continue;
				}
				Entity prefab = m_PrefabRefData[val].m_Prefab;
				if (!m_NetGeometryData.HasComponent(prefab))
				{
					continue;
				}
				NetData net = m_NetData[prefab];
				NetGeometryData netGeometry = m_NetGeometryData[prefab];
				if (m_CompositionData.HasComponent(val))
				{
					Composition composition = m_CompositionData[val];
					EdgeGeometry geometry = m_EdgeGeometryData[val];
					StartNodeGeometry startNodeGeometry = m_StartNodeGeometryData[val];
					EndNodeGeometry endNodeGeometry = m_EndNodeGeometryData[val];
					if (math.any(geometry.m_Start.m_Length + geometry.m_End.m_Length > 0.1f))
					{
						NetCompositionData prefabCompositionData = m_PrefabCompositionData[composition.m_Edge];
						TerrainComposition terrainComposition = default(TerrainComposition);
						if (m_TerrainCompositionData.HasComponent(composition.m_Edge))
						{
							terrainComposition = m_TerrainCompositionData[composition.m_Edge];
						}
						AddEdge(geometry, m_Area, net, netGeometry, prefabCompositionData, terrainComposition);
					}
					if (math.any(startNodeGeometry.m_Geometry.m_Left.m_Length > 0.05f) | math.any(startNodeGeometry.m_Geometry.m_Right.m_Length > 0.05f))
					{
						NetCompositionData prefabCompositionData2 = m_PrefabCompositionData[composition.m_StartNode];
						TerrainComposition terrainComposition2 = default(TerrainComposition);
						if (m_TerrainCompositionData.HasComponent(composition.m_StartNode))
						{
							terrainComposition2 = m_TerrainCompositionData[composition.m_StartNode];
						}
						AddNode(startNodeGeometry.m_Geometry, m_Area, net, netGeometry, prefabCompositionData2, terrainComposition2);
					}
					if (math.any(endNodeGeometry.m_Geometry.m_Left.m_Length > 0.05f) | math.any(endNodeGeometry.m_Geometry.m_Right.m_Length > 0.05f))
					{
						NetCompositionData prefabCompositionData3 = m_PrefabCompositionData[composition.m_EndNode];
						TerrainComposition terrainComposition3 = default(TerrainComposition);
						if (m_TerrainCompositionData.HasComponent(composition.m_EndNode))
						{
							terrainComposition3 = m_TerrainCompositionData[composition.m_EndNode];
						}
						AddNode(endNodeGeometry.m_Geometry, m_Area, net, netGeometry, prefabCompositionData3, terrainComposition3);
					}
				}
				else if (m_OrphanData.HasComponent(val))
				{
					Orphan orphan = m_OrphanData[val];
					Game.Net.Node node = m_NodeData[val];
					NetCompositionData prefabCompositionData4 = m_PrefabCompositionData[orphan.m_Composition];
					TerrainComposition terrainComposition4 = default(TerrainComposition);
					if (m_TerrainCompositionData.HasComponent(orphan.m_Composition))
					{
						terrainComposition4 = m_TerrainCompositionData[orphan.m_Composition];
					}
					NodeGeometry nodeGeometry = m_NodeGeometryData[val];
					AddOrphans(node, nodeGeometry, m_Area, net, netGeometry, prefabCompositionData4, terrainComposition4);
				}
			}
		}

		private LaneFlags GetFlags(NetGeometryData netGeometry, NetCompositionData prefabCompositionData)
		{
			LaneFlags laneFlags = (LaneFlags)0;
			if ((netGeometry.m_Flags & Game.Net.GeometryFlags.ClipTerrain) != 0)
			{
				laneFlags |= LaneFlags.ClipTerrain;
			}
			if ((netGeometry.m_Flags & Game.Net.GeometryFlags.FlattenTerrain) != 0)
			{
				laneFlags |= LaneFlags.ShiftTerrain;
			}
			return laneFlags;
		}

		private void AddEdge(EdgeGeometry geometry, float4 area, NetData net, NetGeometryData netGeometry, NetCompositionData prefabCompositionData, TerrainComposition terrainComposition)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			LaneFlags laneFlags = GetFlags(netGeometry, prefabCompositionData);
			if ((laneFlags & (LaneFlags.ShiftTerrain | LaneFlags.ClipTerrain)) == 0)
			{
				return;
			}
			Bounds2 xz = ((Bounds3)(ref geometry.m_Bounds)).xz;
			if (!math.any(xz.max < ((float4)(ref area)).xy) && !math.any(xz.min > ((float4)(ref area)).zw))
			{
				if ((prefabCompositionData.m_Flags.m_General & CompositionFlags.General.Tunnel) != 0)
				{
					laneFlags |= LaneFlags.InverseClipOffset;
				}
				AddSegment(geometry.m_Start, net, netGeometry, prefabCompositionData, terrainComposition, laneFlags, isStart: true);
				AddSegment(geometry.m_End, net, netGeometry, prefabCompositionData, terrainComposition, laneFlags, isStart: false);
			}
		}

		private void MoveTowards(ref float3 position, float3 other, float amount)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			float3 val = other - position;
			val = MathUtils.Normalize(val, ((float3)(ref val)).xz);
			position += val * amount;
		}

		private void AddNode(EdgeNodeGeometry node, float4 area, NetData net, NetGeometryData netGeometry, NetCompositionData prefabCompositionData, TerrainComposition terrainComposition)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_071d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0722: Unknown result type (might be due to invalid IL or missing references)
			//IL_072d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0732: Unknown result type (might be due to invalid IL or missing references)
			//IL_0737: Unknown result type (might be due to invalid IL or missing references)
			//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_080b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0810: Unknown result type (might be due to invalid IL or missing references)
			//IL_081d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0822: Unknown result type (might be due to invalid IL or missing references)
			//IL_076a: Unknown result type (might be due to invalid IL or missing references)
			//IL_076f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0777: Unknown result type (might be due to invalid IL or missing references)
			//IL_077c: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_0570: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_0598: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05be: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0525: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_0668: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_067a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0601: Unknown result type (might be due to invalid IL or missing references)
			//IL_0606: Unknown result type (might be due to invalid IL or missing references)
			//IL_061f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0624: Unknown result type (might be due to invalid IL or missing references)
			//IL_063d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0642: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0696: Unknown result type (might be due to invalid IL or missing references)
			//IL_069b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
			LaneFlags laneFlags = GetFlags(netGeometry, prefabCompositionData);
			if ((laneFlags & (LaneFlags.ShiftTerrain | LaneFlags.ClipTerrain)) == 0)
			{
				return;
			}
			Bounds2 xz = ((Bounds3)(ref node.m_Bounds)).xz;
			if (math.any(xz.max < ((float4)(ref area)).xy) || math.any(xz.min > ((float4)(ref area)).zw))
			{
				return;
			}
			if (node.m_MiddleRadius > 0f)
			{
				NetCompositionData compositionData = prefabCompositionData;
				float num = 0f;
				float num2 = 0f;
				if ((prefabCompositionData.m_Flags.m_General & CompositionFlags.General.Elevated) != 0)
				{
					if ((prefabCompositionData.m_Flags.m_Left & CompositionFlags.Side.HighTransition) != 0)
					{
						num = prefabCompositionData.m_SyncVertexOffsetsLeft.x;
						compositionData.m_Flags.m_General &= ~CompositionFlags.General.Elevated;
						compositionData.m_Flags.m_Left &= ~CompositionFlags.Side.HighTransition;
					}
					else if ((prefabCompositionData.m_Flags.m_Left & CompositionFlags.Side.LowTransition) != 0)
					{
						num = prefabCompositionData.m_SyncVertexOffsetsLeft.x;
						compositionData.m_Flags.m_General &= ~CompositionFlags.General.Elevated;
						compositionData.m_Flags.m_Left &= ~CompositionFlags.Side.LowTransition;
						compositionData.m_Flags.m_Left |= CompositionFlags.Side.Raised;
					}
					if ((prefabCompositionData.m_Flags.m_Right & CompositionFlags.Side.HighTransition) != 0)
					{
						num2 = 1f - prefabCompositionData.m_SyncVertexOffsetsRight.w;
						compositionData.m_Flags.m_General &= ~CompositionFlags.General.Elevated;
						compositionData.m_Flags.m_Right &= ~CompositionFlags.Side.HighTransition;
					}
					else if ((prefabCompositionData.m_Flags.m_Right & CompositionFlags.Side.LowTransition) != 0)
					{
						num2 = 1f - prefabCompositionData.m_SyncVertexOffsetsRight.w;
						compositionData.m_Flags.m_General &= ~CompositionFlags.General.Elevated;
						compositionData.m_Flags.m_Right &= ~CompositionFlags.Side.LowTransition;
						compositionData.m_Flags.m_Right |= CompositionFlags.Side.Raised;
					}
				}
				else if ((prefabCompositionData.m_Flags.m_General & CompositionFlags.General.Tunnel) != 0)
				{
					laneFlags |= LaneFlags.InverseClipOffset;
					if ((prefabCompositionData.m_Flags.m_Left & CompositionFlags.Side.HighTransition) != 0)
					{
						num = prefabCompositionData.m_SyncVertexOffsetsLeft.x;
						compositionData.m_Flags.m_General &= ~CompositionFlags.General.Tunnel;
						compositionData.m_Flags.m_Left &= ~CompositionFlags.Side.HighTransition;
						laneFlags &= ~LaneFlags.InverseClipOffset;
					}
					else if ((prefabCompositionData.m_Flags.m_Left & CompositionFlags.Side.LowTransition) != 0)
					{
						num = prefabCompositionData.m_SyncVertexOffsetsLeft.x;
						compositionData.m_Flags.m_General &= ~CompositionFlags.General.Tunnel;
						compositionData.m_Flags.m_Left &= ~CompositionFlags.Side.LowTransition;
						compositionData.m_Flags.m_Left |= CompositionFlags.Side.Lowered;
						laneFlags &= ~LaneFlags.InverseClipOffset;
					}
					if ((prefabCompositionData.m_Flags.m_Right & CompositionFlags.Side.HighTransition) != 0)
					{
						num2 = 1f - prefabCompositionData.m_SyncVertexOffsetsRight.w;
						compositionData.m_Flags.m_General &= ~CompositionFlags.General.Tunnel;
						compositionData.m_Flags.m_Right &= ~CompositionFlags.Side.HighTransition;
						laneFlags &= ~LaneFlags.InverseClipOffset;
					}
					else if ((prefabCompositionData.m_Flags.m_Right & CompositionFlags.Side.LowTransition) != 0)
					{
						num2 = 1f - prefabCompositionData.m_SyncVertexOffsetsRight.w;
						compositionData.m_Flags.m_General &= ~CompositionFlags.General.Tunnel;
						compositionData.m_Flags.m_Right &= ~CompositionFlags.Side.LowTransition;
						compositionData.m_Flags.m_Right |= CompositionFlags.Side.Lowered;
						laneFlags &= ~LaneFlags.InverseClipOffset;
					}
				}
				else
				{
					if ((prefabCompositionData.m_Flags.m_Left & CompositionFlags.Side.LowTransition) != 0)
					{
						if ((prefabCompositionData.m_Flags.m_Left & CompositionFlags.Side.Raised) != 0)
						{
							num = prefabCompositionData.m_SyncVertexOffsetsLeft.x;
							compositionData.m_Flags.m_Left &= ~(CompositionFlags.Side.Raised | CompositionFlags.Side.LowTransition);
						}
						else if ((prefabCompositionData.m_Flags.m_Left & CompositionFlags.Side.Lowered) != 0)
						{
							num = prefabCompositionData.m_SyncVertexOffsetsLeft.x;
							compositionData.m_Flags.m_Left &= ~(CompositionFlags.Side.Lowered | CompositionFlags.Side.LowTransition);
						}
						else if ((prefabCompositionData.m_Flags.m_Left & CompositionFlags.Side.SoundBarrier) != 0)
						{
							num = prefabCompositionData.m_SyncVertexOffsetsLeft.x;
							compositionData.m_Flags.m_Left &= ~(CompositionFlags.Side.LowTransition | CompositionFlags.Side.SoundBarrier);
						}
					}
					if ((prefabCompositionData.m_Flags.m_Right & CompositionFlags.Side.LowTransition) != 0)
					{
						if ((prefabCompositionData.m_Flags.m_Right & CompositionFlags.Side.Raised) != 0)
						{
							num2 = 1f - prefabCompositionData.m_SyncVertexOffsetsRight.w;
							compositionData.m_Flags.m_Right &= ~(CompositionFlags.Side.Raised | CompositionFlags.Side.LowTransition);
						}
						else if ((prefabCompositionData.m_Flags.m_Right & CompositionFlags.Side.Lowered) != 0)
						{
							num2 = 1f - prefabCompositionData.m_SyncVertexOffsetsRight.w;
							compositionData.m_Flags.m_Right &= ~(CompositionFlags.Side.Lowered | CompositionFlags.Side.LowTransition);
						}
						else if ((prefabCompositionData.m_Flags.m_Right & CompositionFlags.Side.SoundBarrier) != 0)
						{
							num2 = 1f - prefabCompositionData.m_SyncVertexOffsetsRight.w;
							compositionData.m_Flags.m_Right &= ~(CompositionFlags.Side.LowTransition | CompositionFlags.Side.SoundBarrier);
						}
					}
				}
				if (num != 0f)
				{
					num *= math.distance(((float3)(ref node.m_Left.m_Left.a)).xz, ((float3)(ref node.m_Middle.a)).xz);
				}
				if (num2 != 0f)
				{
					num2 *= math.distance(((float3)(ref node.m_Middle.a)).xz, ((float3)(ref node.m_Left.m_Right.a)).xz);
				}
				Segment left = node.m_Left;
				left.m_Right = node.m_Middle;
				AddSegment(left, net, netGeometry, prefabCompositionData, terrainComposition, laneFlags | LaneFlags.MiddleRight, isStart: true);
				left.m_Left = left.m_Right;
				left.m_Right = node.m_Left.m_Right;
				AddSegment(left, net, netGeometry, prefabCompositionData, terrainComposition, laneFlags | LaneFlags.MiddleLeft, isStart: true);
				left = node.m_Right;
				left.m_Right = new Bezier4x3(node.m_Middle.d, node.m_Middle.d, node.m_Middle.d, node.m_Middle.d);
				if (num != 0f)
				{
					MoveTowards(ref left.m_Left.a, node.m_Middle.d, num);
					MoveTowards(ref left.m_Left.b, node.m_Middle.d, num);
					MoveTowards(ref left.m_Left.c, node.m_Middle.d, num);
					MoveTowards(ref left.m_Left.d, node.m_Middle.d, num);
				}
				AddSegment(left, net, netGeometry, compositionData, terrainComposition, laneFlags | LaneFlags.MiddleRight, isStart: false);
				left.m_Left = left.m_Right;
				left.m_Right = node.m_Right.m_Right;
				if (num2 != 0f)
				{
					MoveTowards(ref left.m_Right.a, node.m_Middle.d, num2);
					MoveTowards(ref left.m_Right.b, node.m_Middle.d, num2);
					MoveTowards(ref left.m_Right.c, node.m_Middle.d, num2);
					MoveTowards(ref left.m_Right.d, node.m_Middle.d, num2);
				}
				AddSegment(left, net, netGeometry, compositionData, terrainComposition, laneFlags | LaneFlags.MiddleLeft, isStart: false);
			}
			else if (math.lengthsq(node.m_Left.m_Right.d - node.m_Right.m_Left.d) > 0.0001f)
			{
				Segment left2 = node.m_Left;
				AddSegment(left2, net, netGeometry, prefabCompositionData, terrainComposition, laneFlags | LaneFlags.MiddleRight, isStart: true);
				left2.m_Left = left2.m_Right;
				left2.m_Right = node.m_Middle;
				AddSegment(left2, net, netGeometry, prefabCompositionData, terrainComposition, laneFlags | (LaneFlags.MiddleLeft | LaneFlags.MiddleRight), isStart: true);
				left2 = node.m_Right;
				AddSegment(left2, net, netGeometry, prefabCompositionData, terrainComposition, laneFlags | LaneFlags.MiddleLeft, isStart: true);
				left2.m_Right = left2.m_Left;
				left2.m_Left = node.m_Middle;
				AddSegment(left2, net, netGeometry, prefabCompositionData, terrainComposition, laneFlags | (LaneFlags.MiddleLeft | LaneFlags.MiddleRight), isStart: true);
			}
			else
			{
				Segment left3 = node.m_Left;
				left3.m_Right = node.m_Middle;
				AddSegment(left3, net, netGeometry, prefabCompositionData, terrainComposition, laneFlags | LaneFlags.MiddleRight, isStart: true);
				left3.m_Left = node.m_Middle;
				left3.m_Right = node.m_Right.m_Right;
				AddSegment(left3, net, netGeometry, prefabCompositionData, terrainComposition, laneFlags | LaneFlags.MiddleLeft, isStart: true);
			}
		}

		private void AddOrphans(Game.Net.Node node, NodeGeometry nodeGeometry, float4 area, NetData net, NetGeometryData netGeometry, NetCompositionData prefabCompositionData, TerrainComposition terrainComposition)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			LaneFlags laneFlags = GetFlags(netGeometry, prefabCompositionData);
			if ((laneFlags & (LaneFlags.ShiftTerrain | LaneFlags.ClipTerrain)) == 0)
			{
				return;
			}
			Segment segment = default(Segment);
			Bounds2 xz = ((Bounds3)(ref nodeGeometry.m_Bounds)).xz;
			if (!math.any(xz.max < ((float4)(ref area)).xy) && !math.any(xz.min > ((float4)(ref area)).zw))
			{
				if ((prefabCompositionData.m_Flags.m_General & CompositionFlags.General.Tunnel) != 0)
				{
					laneFlags |= LaneFlags.InverseClipOffset;
				}
				segment.m_Left.a = new float3(node.m_Position.x - prefabCompositionData.m_Width * 0.5f, node.m_Position.y, node.m_Position.z);
				segment.m_Left.b = new float3(node.m_Position.x - prefabCompositionData.m_Width * 0.5f, node.m_Position.y, node.m_Position.z + prefabCompositionData.m_Width * 0.2761424f);
				segment.m_Left.c = new float3(node.m_Position.x - prefabCompositionData.m_Width * 0.2761424f, node.m_Position.y, node.m_Position.z + prefabCompositionData.m_Width * 0.5f);
				segment.m_Left.d = new float3(node.m_Position.x, node.m_Position.y, node.m_Position.z + prefabCompositionData.m_Width * 0.5f);
				segment.m_Right = new Bezier4x3(node.m_Position, node.m_Position, node.m_Position, node.m_Position);
				segment.m_Length = new float2(prefabCompositionData.m_Width * ((float)Math.PI / 2f), 0f);
				AddSegment(segment, net, netGeometry, prefabCompositionData, terrainComposition, laneFlags | LaneFlags.MiddleRight, isStart: true);
				CommonUtils.Swap(ref segment.m_Left, ref segment.m_Right);
				segment.m_Right.a.x += prefabCompositionData.m_Width;
				segment.m_Right.b.x += prefabCompositionData.m_Width;
				segment.m_Right.c.x = node.m_Position.x * 2f - segment.m_Right.c.x;
				AddSegment(segment, net, netGeometry, prefabCompositionData, terrainComposition, laneFlags | LaneFlags.MiddleLeft, isStart: true);
				segment.m_Left.a = new float3(node.m_Position.x + prefabCompositionData.m_Width * 0.5f, node.m_Position.y, node.m_Position.z);
				segment.m_Left.b = new float3(node.m_Position.x + prefabCompositionData.m_Width * 0.5f, node.m_Position.y, node.m_Position.z - prefabCompositionData.m_Width * 0.2761424f);
				segment.m_Left.c = new float3(node.m_Position.x + prefabCompositionData.m_Width * 0.2761424f, node.m_Position.y, node.m_Position.z - prefabCompositionData.m_Width * 0.5f);
				segment.m_Left.d = new float3(node.m_Position.x, node.m_Position.y, node.m_Position.z - prefabCompositionData.m_Width * 0.5f);
				segment.m_Right = new Bezier4x3(node.m_Position, node.m_Position, node.m_Position, node.m_Position);
				segment.m_Length = new float2(prefabCompositionData.m_Width * ((float)Math.PI / 2f), 0f);
				AddSegment(segment, net, netGeometry, prefabCompositionData, terrainComposition, laneFlags | LaneFlags.MiddleRight, isStart: true);
				CommonUtils.Swap(ref segment.m_Left, ref segment.m_Right);
				segment.m_Right.a.x -= prefabCompositionData.m_Width;
				segment.m_Right.b.x -= prefabCompositionData.m_Width;
				segment.m_Right.c.x = node.m_Position.x * 2f - segment.m_Right.c.x;
				AddSegment(segment, net, netGeometry, prefabCompositionData, terrainComposition, laneFlags | LaneFlags.MiddleLeft, isStart: true);
			}
		}

		private void AddSegment(Segment segment, NetData net, NetGeometryData netGeometry, NetCompositionData compositionData, TerrainComposition terrainComposition, LaneFlags flags, bool isStart)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_060e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_062d: Unknown result type (might be due to invalid IL or missing references)
			//IL_062e: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_064a: Unknown result type (might be due to invalid IL or missing references)
			//IL_064b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_0668: Unknown result type (might be due to invalid IL or missing references)
			//IL_0670: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_0680: Unknown result type (might be due to invalid IL or missing references)
			//IL_0685: Unknown result type (might be due to invalid IL or missing references)
			//IL_0690: Unknown result type (might be due to invalid IL or missing references)
			//IL_0695: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0700: Unknown result type (might be due to invalid IL or missing references)
			//IL_0705: Unknown result type (might be due to invalid IL or missing references)
			//IL_0710: Unknown result type (might be due to invalid IL or missing references)
			//IL_0715: Unknown result type (might be due to invalid IL or missing references)
			//IL_0720: Unknown result type (might be due to invalid IL or missing references)
			//IL_0725: Unknown result type (might be due to invalid IL or missing references)
			//IL_072f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0734: Unknown result type (might be due to invalid IL or missing references)
			//IL_073c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0741: Unknown result type (might be due to invalid IL or missing references)
			//IL_074c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0751: Unknown result type (might be due to invalid IL or missing references)
			//IL_075c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0761: Unknown result type (might be due to invalid IL or missing references)
			//IL_076c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0771: Unknown result type (might be due to invalid IL or missing references)
			//IL_077c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0781: Unknown result type (might be due to invalid IL or missing references)
			//IL_078c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_079c: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0800: Unknown result type (might be due to invalid IL or missing references)
			//IL_0807: Unknown result type (might be due to invalid IL or missing references)
			//IL_0808: Unknown result type (might be due to invalid IL or missing references)
			//IL_080f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0810: Unknown result type (might be due to invalid IL or missing references)
			//IL_0817: Unknown result type (might be due to invalid IL or missing references)
			//IL_0818: Unknown result type (might be due to invalid IL or missing references)
			//IL_0849: Unknown result type (might be due to invalid IL or missing references)
			//IL_0850: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b87: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b93: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0891: Unknown result type (might be due to invalid IL or missing references)
			//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08db: Unknown result type (might be due to invalid IL or missing references)
			//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0904: Unknown result type (might be due to invalid IL or missing references)
			//IL_090e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0915: Unknown result type (might be due to invalid IL or missing references)
			//IL_0927: Unknown result type (might be due to invalid IL or missing references)
			//IL_092e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0946: Unknown result type (might be due to invalid IL or missing references)
			//IL_0948: Unknown result type (might be due to invalid IL or missing references)
			//IL_0949: Unknown result type (might be due to invalid IL or missing references)
			//IL_0963: Unknown result type (might be due to invalid IL or missing references)
			//IL_0965: Unknown result type (might be due to invalid IL or missing references)
			//IL_0966: Unknown result type (might be due to invalid IL or missing references)
			//IL_097e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0983: Unknown result type (might be due to invalid IL or missing references)
			//IL_098c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0991: Unknown result type (might be due to invalid IL or missing references)
			//IL_099d: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a08: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a19: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a25: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a36: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a86: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a97: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0adb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bdc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c08: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c29: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c36: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c79: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d60: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d82: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d87: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d93: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dcb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ddc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0deb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e27: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eaf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ebe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ecc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0edc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0edd: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_0556: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0567: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_0537: Unknown result type (might be due to invalid IL or missing references)
			float num = compositionData.m_Width;
			if (math.any(terrainComposition.m_WidthOffset != 0f))
			{
				Segment segment2 = segment;
				float4 val = default(float4);
				((float4)(ref val))._002Ector(math.distance(((float3)(ref segment.m_Left.a)).xz, ((float3)(ref segment.m_Right.a)).xz), math.distance(((float3)(ref segment.m_Left.b)).xz, ((float3)(ref segment.m_Right.b)).xz), math.distance(((float3)(ref segment.m_Left.c)).xz, ((float3)(ref segment.m_Right.c)).xz), math.distance(((float3)(ref segment.m_Left.d)).xz, ((float3)(ref segment.m_Right.d)).xz));
				val = 1f / math.max(float4.op_Implicit(0.001f), val);
				if (terrainComposition.m_WidthOffset.x != 0f && (flags & LaneFlags.MiddleLeft) == 0)
				{
					Bezier4x1 val2 = default(Bezier4x1);
					((Bezier4x1)(ref val2)).abcd = terrainComposition.m_WidthOffset.x * val;
					segment.m_Left = MathUtils.Lerp(segment2.m_Left, segment2.m_Right, val2);
					num -= terrainComposition.m_WidthOffset.x;
				}
				if (terrainComposition.m_WidthOffset.y != 0f && (flags & LaneFlags.MiddleRight) == 0)
				{
					Bezier4x1 val3 = default(Bezier4x1);
					((Bezier4x1)(ref val3)).abcd = terrainComposition.m_WidthOffset.y * val;
					segment.m_Right = MathUtils.Lerp(segment2.m_Right, segment2.m_Left, val3);
					num -= terrainComposition.m_WidthOffset.y;
				}
			}
			float3 val4 = math.select(new float3(compositionData.m_EdgeHeights.z, compositionData.m_SurfaceHeight.min, compositionData.m_EdgeHeights.w), new float3(compositionData.m_EdgeHeights.x, compositionData.m_SurfaceHeight.min, compositionData.m_EdgeHeights.y), isStart);
			float3 val5 = val4;
			float2 val6 = default(float2);
			((float2)(ref val6))._002Ector(math.cmin(val4), math.cmax(val5));
			float terrainSmoothingWidth = NetUtils.GetTerrainSmoothingWidth(net);
			val6 += terrainComposition.m_ClipHeightOffset;
			val4 += terrainComposition.m_MinHeightOffset;
			val5 += terrainComposition.m_MaxHeightOffset;
			float3 val7 = float3.op_Implicit(1000000f);
			float3 val8 = float3.op_Implicit(1000000f);
			float3 val9 = float3.op_Implicit(1000000f);
			if ((compositionData.m_State & CompositionState.HasSurface) == 0)
			{
				if ((compositionData.m_Flags.m_General & CompositionFlags.General.Tunnel) != 0)
				{
					val4 = float3.op_Implicit(1000000f);
					val5 = compositionData.m_HeightRange.max + 1f + terrainComposition.m_MaxHeightOffset;
				}
				else
				{
					val4 = compositionData.m_HeightRange.min + terrainComposition.m_MinHeightOffset;
					val5 = float3.op_Implicit(-1000000f);
				}
			}
			else if ((compositionData.m_Flags.m_General & CompositionFlags.General.Elevated) != 0 || (netGeometry.m_MergeLayers & Layer.Waterway) != Layer.None)
			{
				if (((compositionData.m_Flags.m_Left | compositionData.m_Flags.m_Right) & (CompositionFlags.Side.LowTransition | CompositionFlags.Side.HighTransition)) == 0)
				{
					val4 = float3.op_Implicit(compositionData.m_HeightRange.min);
				}
				val5 = float3.op_Implicit(-1000000f);
			}
			else if ((compositionData.m_Flags.m_General & CompositionFlags.General.Tunnel) != 0)
			{
				if ((compositionData.m_Flags.m_Left & CompositionFlags.Side.HighTransition) != 0)
				{
					((float3)(ref val7)).xy = math.min(((float3)(ref val7)).xy, ((float3)(ref val4)).xy);
				}
				if ((compositionData.m_Flags.m_Right & CompositionFlags.Side.HighTransition) != 0)
				{
					((float3)(ref val7)).yz = math.min(((float3)(ref val7)).yz, ((float3)(ref val4)).yz);
				}
				if (((compositionData.m_Flags.m_Left | compositionData.m_Flags.m_Right) & (CompositionFlags.Side.LowTransition | CompositionFlags.Side.HighTransition)) == 0)
				{
					val4 = float3.op_Implicit(1000000f);
					val5 = float3.op_Implicit(compositionData.m_HeightRange.max + 1f);
				}
				else
				{
					val8 = float3.op_Implicit(netGeometry.m_ElevationLimit * 3f);
					val9 = float3.op_Implicit(compositionData.m_HeightRange.max + 1f);
					val4 = math.max(val4, float3.op_Implicit(netGeometry.m_ElevationLimit * 3f));
					val6.y = math.max(val6.y, netGeometry.m_ElevationLimit * 3f);
				}
			}
			else
			{
				if ((compositionData.m_Flags.m_Left & CompositionFlags.Side.Lowered) != 0)
				{
					if ((compositionData.m_Flags.m_Left & CompositionFlags.Side.LowTransition) != 0)
					{
						((float3)(ref val7)).xy = math.min(((float3)(ref val7)).xy, ((float3)(ref val4)).xy);
					}
					((float3)(ref val4)).xy = math.max(((float3)(ref val4)).xy, float2.op_Implicit(netGeometry.m_ElevationLimit * 3f));
					val6.y = math.max(val6.y, netGeometry.m_ElevationLimit * 3f);
				}
				else if ((compositionData.m_Flags.m_Left & CompositionFlags.Side.Raised) != 0)
				{
					((float3)(ref val5)).xy = float2.op_Implicit(-1000000f);
				}
				if ((compositionData.m_Flags.m_Right & CompositionFlags.Side.Lowered) != 0)
				{
					if ((compositionData.m_Flags.m_Right & CompositionFlags.Side.LowTransition) != 0)
					{
						((float3)(ref val7)).yz = math.min(((float3)(ref val7)).yz, ((float3)(ref val4)).yz);
					}
					((float3)(ref val4)).yz = math.max(((float3)(ref val4)).yz, float2.op_Implicit(netGeometry.m_ElevationLimit * 3f));
					val6.y = math.max(val6.y, netGeometry.m_ElevationLimit * 3f);
				}
				else if ((compositionData.m_Flags.m_Right & CompositionFlags.Side.Raised) != 0)
				{
					((float3)(ref val5)).yz = float2.op_Implicit(-1000000f);
				}
			}
			float middleSize = math.saturate(1f - math.max(num * 0.2f, 3f) / math.max(1f, num));
			Bounds3 val10 = MathUtils.Bounds(segment.m_Left) | MathUtils.Bounds(segment.m_Right);
			ref float3 min = ref val10.min;
			((float3)(ref min)).xz = ((float3)(ref min)).xz - terrainSmoothingWidth;
			ref float3 max = ref val10.max;
			((float3)(ref max)).xz = ((float3)(ref max)).xz + terrainSmoothingWidth;
			val10.min.y += math.cmin(math.min(val4, val5));
			val10.max.y += math.cmax(math.max(val4, val5));
			LaneSection laneSection = new LaneSection
			{
				m_Bounds = ((Bounds3)(ref val10)).xz,
				m_Left = new float4x3(segment.m_Left.a.x, segment.m_Left.a.y, segment.m_Left.a.z, segment.m_Left.b.x, segment.m_Left.b.y, segment.m_Left.b.z, segment.m_Left.c.x, segment.m_Left.c.y, segment.m_Left.c.z, segment.m_Left.d.x, segment.m_Left.d.y, segment.m_Left.d.z),
				m_Right = new float4x3(segment.m_Right.a.x, segment.m_Right.a.y, segment.m_Right.a.z, segment.m_Right.b.x, segment.m_Right.b.y, segment.m_Right.b.z, segment.m_Right.c.x, segment.m_Right.c.y, segment.m_Right.c.z, segment.m_Right.d.x, segment.m_Right.d.y, segment.m_Right.d.z),
				m_MinOffset = val4,
				m_MaxOffset = val5,
				m_ClipOffset = val6,
				m_WidthOffset = terrainSmoothingWidth,
				m_MiddleSize = middleSize,
				m_Flags = flags
			};
			Result.AddNoResize(laneSection);
			if (math.any(val7 != 1000000f) && (flags & LaneFlags.ShiftTerrain) != 0)
			{
				Bounds1 val11 = default(Bounds1);
				((Bounds1)(ref val11))._002Ector(0f, 1f);
				Bounds1 val12 = default(Bounds1);
				((Bounds1)(ref val12))._002Ector(0f, 1f);
				MathUtils.ClampLengthInverse(((Bezier4x3)(ref segment.m_Left)).xz, ref val11, 3f);
				MathUtils.ClampLengthInverse(((Bezier4x3)(ref segment.m_Right)).xz, ref val12, 3f);
				Segment segment3 = segment;
				segment3.m_Left = MathUtils.Cut(segment.m_Left, val11);
				segment3.m_Right = MathUtils.Cut(segment.m_Right, val12);
				val10 = MathUtils.Bounds(segment3.m_Left) | MathUtils.Bounds(segment3.m_Right);
				ref float3 min2 = ref val10.min;
				((float3)(ref min2)).xz = ((float3)(ref min2)).xz - terrainSmoothingWidth;
				ref float3 max2 = ref val10.max;
				((float3)(ref max2)).xz = ((float3)(ref max2)).xz + terrainSmoothingWidth;
				val10.min.y += math.cmin(math.min(val7, val5));
				val10.max.y += math.cmax(math.max(val7, val5));
				laneSection = new LaneSection
				{
					m_Bounds = ((Bounds3)(ref val10)).xz,
					m_Left = new float4x3(segment3.m_Left.a.x, segment3.m_Left.a.y, segment3.m_Left.a.z, segment3.m_Left.b.x, segment3.m_Left.b.y, segment3.m_Left.b.z, segment3.m_Left.c.x, segment3.m_Left.c.y, segment3.m_Left.c.z, segment3.m_Left.d.x, segment3.m_Left.d.y, segment3.m_Left.d.z),
					m_Right = new float4x3(segment3.m_Right.a.x, segment3.m_Right.a.y, segment3.m_Right.a.z, segment3.m_Right.b.x, segment3.m_Right.b.y, segment3.m_Right.b.z, segment3.m_Right.c.x, segment3.m_Right.c.y, segment3.m_Right.c.z, segment3.m_Right.d.x, segment3.m_Right.d.y, segment3.m_Right.d.z),
					m_MinOffset = val7,
					m_MaxOffset = val5,
					m_ClipOffset = val6,
					m_WidthOffset = terrainSmoothingWidth,
					m_MiddleSize = middleSize,
					m_Flags = (flags & ~LaneFlags.ClipTerrain)
				};
				Result.AddNoResize(laneSection);
			}
			if ((math.any(val8 != 1000000f) || math.any(val9 != 1000000f)) && (flags & LaneFlags.ShiftTerrain) != 0)
			{
				float3 val13 = MathUtils.StartTangent(segment.m_Left);
				float3 val14 = MathUtils.StartTangent(segment.m_Right);
				val13 = MathUtils.Normalize(val13, ((float3)(ref val13)).xz);
				val14 = MathUtils.Normalize(val14, ((float3)(ref val14)).xz);
				Segment segment4 = segment;
				segment4.m_Left = NetUtils.StraightCurve(segment.m_Left.a + val13 * 2f, segment.m_Left.a - val13 * 2f);
				segment4.m_Right = NetUtils.StraightCurve(segment.m_Right.a + val14 * 2f, segment.m_Right.a - val14 * 2f);
				val10 = MathUtils.Bounds(segment4.m_Left) | MathUtils.Bounds(segment4.m_Right);
				ref float3 min3 = ref val10.min;
				((float3)(ref min3)).xz = ((float3)(ref min3)).xz - terrainSmoothingWidth;
				ref float3 max3 = ref val10.max;
				((float3)(ref max3)).xz = ((float3)(ref max3)).xz + terrainSmoothingWidth;
				val10.min.y += math.cmin(math.min(val8, val5));
				val10.max.y += math.cmax(math.max(val8, val5));
				laneSection = new LaneSection
				{
					m_Bounds = ((Bounds3)(ref val10)).xz,
					m_Left = new float4x3(segment4.m_Left.a.x, segment4.m_Left.a.y, segment4.m_Left.a.z, segment4.m_Left.b.x, segment4.m_Left.b.y, segment4.m_Left.b.z, segment4.m_Left.c.x, segment4.m_Left.c.y, segment4.m_Left.c.z, segment4.m_Left.d.x, segment4.m_Left.d.y, segment4.m_Left.d.z),
					m_Right = new float4x3(segment4.m_Right.a.x, segment4.m_Right.a.y, segment4.m_Right.a.z, segment4.m_Right.b.x, segment4.m_Right.b.y, segment4.m_Right.b.z, segment4.m_Right.c.x, segment4.m_Right.c.y, segment4.m_Right.c.z, segment4.m_Right.d.x, segment4.m_Right.d.y, segment4.m_Right.d.z),
					m_MinOffset = val8,
					m_MaxOffset = val9,
					m_ClipOffset = val6,
					m_WidthOffset = terrainSmoothingWidth,
					m_MiddleSize = middleSize,
					m_Flags = (flags & ~LaneFlags.ClipTerrain)
				};
				Result.AddNoResize(laneSection);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct DequeBuildingDrawsJob : IJob
	{
		[ReadOnly]
		public NativeQueue<BuildingLotDraw> m_Queue;

		public NativeList<BuildingLotDraw> m_List;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<BuildingLotDraw> val = m_Queue.ToArray(AllocatorHandle.op_Implicit((Allocator)2));
			m_List.CopyFrom(ref val);
			val.Dispose();
		}
	}

	[BurstCompile]
	private struct CullBuildingsCascadeJob : IJobParallelForDefer
	{
		[ReadOnly]
		public NativeList<BuildingUtils.LotInfo> m_LotsToCull;

		[ReadOnly]
		public float4 m_Area;

		public ParallelWriter<BuildingLotDraw> Result;

		public void Execute(int index)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			if (index < m_LotsToCull.Length)
			{
				BuildingUtils.LotInfo lotInfo = m_LotsToCull[index];
				if (!(lotInfo.m_Position.x + lotInfo.m_Radius < m_Area.x) && !(lotInfo.m_Position.x - lotInfo.m_Radius > m_Area.z) && !(lotInfo.m_Position.z + lotInfo.m_Radius < m_Area.y) && !(lotInfo.m_Position.z - lotInfo.m_Radius > m_Area.w))
				{
					float2 val = 0.5f / math.max(float2.op_Implicit(0.01f), lotInfo.m_Extents);
					BuildingLotDraw buildingLotDraw = new BuildingLotDraw
					{
						m_HeightsX = math.transpose(new float4x2(new float4(lotInfo.m_RightHeights, lotInfo.m_BackHeights.x), new float4(lotInfo.m_FrontHeights.x, ((float3)(ref lotInfo.m_LeftHeights)).zyx))),
						m_HeightsZ = math.transpose(new float4x2(new float4(lotInfo.m_RightHeights.x, ((float3)(ref lotInfo.m_FrontHeights)).zyx), new float4(lotInfo.m_BackHeights, lotInfo.m_LeftHeights.x))),
						m_FlatX0 = lotInfo.m_FlatX0 * val.x + 0.5f,
						m_FlatZ0 = lotInfo.m_FlatZ0 * val.y + 0.5f,
						m_FlatX1 = lotInfo.m_FlatX1 * val.x + 0.5f,
						m_FlatZ1 = lotInfo.m_FlatZ1 * val.y + 0.5f,
						m_Position = lotInfo.m_Position,
						m_AxisX = math.mul(lotInfo.m_Rotation, new float3(1f, 0f, 0f)),
						m_AxisZ = math.mul(lotInfo.m_Rotation, new float3(0f, 0f, 1f)),
						m_Size = lotInfo.m_Extents,
						m_MinLimit = lotInfo.m_MinLimit * ((float2)(ref val)).xyxy + 0.5f,
						m_MaxLimit = lotInfo.m_MaxLimit * ((float2)(ref val)).xyxy + 0.5f,
						m_Circular = lotInfo.m_Circular,
						m_SmoothingWidth = ObjectUtils.GetTerrainSmoothingWidth(lotInfo.m_Extents * 2f)
					};
					Result.Enqueue(buildingLotDraw);
				}
			}
		}
	}

	[BurstCompile]
	private struct CullTrianglesJob : IJob
	{
		[ReadOnly]
		public NativeList<AreaTriangle> m_Triangles;

		[ReadOnly]
		public float4 m_Area;

		public NativeList<AreaTriangle> Result;

		public void Execute()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_Triangles.Length; i++)
			{
				AreaTriangle areaTriangle = m_Triangles[i];
				float2 val = math.min(((float3)(ref areaTriangle.m_PositionA)).xz, math.min(((float3)(ref areaTriangle.m_PositionB)).xz, ((float3)(ref areaTriangle.m_PositionC)).xz));
				float2 val2 = math.max(((float3)(ref areaTriangle.m_PositionA)).xz, math.max(((float3)(ref areaTriangle.m_PositionB)).xz, ((float3)(ref areaTriangle.m_PositionC)).xz));
				if (!(val2.x < m_Area.x) && !(val.x > m_Area.z) && !(val2.y < m_Area.y) && !(val.y > m_Area.w))
				{
					Result.Add(ref areaTriangle);
				}
			}
		}
	}

	[BurstCompile]
	private struct CullEdgesJob : IJob
	{
		[ReadOnly]
		public NativeList<AreaEdge> m_Edges;

		[ReadOnly]
		public float4 m_Area;

		public NativeList<AreaEdge> Result;

		public void Execute()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_Edges.Length; i++)
			{
				AreaEdge areaEdge = m_Edges[i];
				float2 val = math.min(areaEdge.m_PositionA, areaEdge.m_PositionB) - areaEdge.m_SideOffset;
				float2 val2 = math.max(areaEdge.m_PositionA, areaEdge.m_PositionB) + areaEdge.m_SideOffset;
				if (!(val2.x < m_Area.x) && !(val.x > m_Area.z) && !(val2.y < m_Area.y) && !(val.y > m_Area.w))
				{
					Result.Add(ref areaEdge);
				}
			}
		}
	}

	[BurstCompile]
	private struct GenerateClipDataJob : IJobParallelForDefer
	{
		[ReadOnly]
		public NativeList<LaneSection> m_RoadsToCull;

		public ParallelWriter<ClipMapDraw> Result;

		public void Execute(int index)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			LaneSection laneSection = m_RoadsToCull[index];
			if ((laneSection.m_Flags & LaneFlags.ClipTerrain) != 0)
			{
				laneSection.m_ClipOffset.x -= 0.3f;
				laneSection.m_ClipOffset.y += 0.3f;
				ref float4 c = ref laneSection.m_Left.c1;
				c += laneSection.m_ClipOffset.x;
				ref float4 c2 = ref laneSection.m_Right.c1;
				c2 += laneSection.m_ClipOffset.x;
				ClipMapDraw clipMapDraw = new ClipMapDraw
				{
					m_Left = laneSection.m_Left,
					m_Right = laneSection.m_Right,
					m_Height = laneSection.m_ClipOffset.y - laneSection.m_ClipOffset.x,
					m_OffsetFactor = math.select(1f, -1f, (laneSection.m_Flags & LaneFlags.InverseClipOffset) != 0)
				};
				Result.AddNoResize(clipMapDraw);
			}
		}
	}

	[BurstCompile]
	private struct CullAreasJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Clip> m_ClipType;

		[ReadOnly]
		public ComponentTypeHandle<Area> m_AreaType;

		[ReadOnly]
		public ComponentTypeHandle<Geometry> m_GeometryType;

		[ReadOnly]
		public ComponentTypeHandle<Storage> m_StorageType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<Game.Areas.Node> m_NodeType;

		[ReadOnly]
		public BufferTypeHandle<Triangle> m_TriangleType;

		[ReadOnly]
		public ComponentLookup<TerrainAreaData> m_PrefabTerrainAreaData;

		[ReadOnly]
		public ComponentLookup<StorageAreaData> m_PrefabStorageAreaData;

		[ReadOnly]
		public float4 m_Area;

		public ParallelWriter<AreaTriangle> m_Triangles;

		public ParallelWriter<AreaEdge> m_Edges;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			//IL_053a: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_053e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_055f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0578: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0581: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_058e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0593: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).Has<Clip>(ref m_ClipType))
			{
				return;
			}
			NativeArray<Area> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Area>(ref m_AreaType);
			NativeArray<Geometry> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Geometry>(ref m_GeometryType);
			NativeArray<Storage> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Storage>(ref m_StorageType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<Game.Areas.Node> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Areas.Node>(ref m_NodeType);
			BufferAccessor<Triangle> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Triangle>(ref m_TriangleType);
			TerrainAreaData terrainAreaData = default(TerrainAreaData);
			float2 noiseSize = default(float2);
			float2 heightDelta = default(float2);
			StorageAreaData prefabStorageData = default(StorageAreaData);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Area area = nativeArray[i];
				Geometry geometry = nativeArray2[i];
				PrefabRef prefabRef = nativeArray4[i];
				DynamicBuffer<Game.Areas.Node> nodes = bufferAccessor[i];
				DynamicBuffer<Triangle> val = bufferAccessor2[i];
				if (geometry.m_Bounds.max.x < m_Area.x || geometry.m_Bounds.min.x > m_Area.z || geometry.m_Bounds.max.z < m_Area.y || geometry.m_Bounds.min.z > m_Area.w || val.Length == 0 || !m_PrefabTerrainAreaData.TryGetComponent(prefabRef.m_Prefab, ref terrainAreaData))
				{
					continue;
				}
				((float2)(ref noiseSize))._002Ector(terrainAreaData.m_NoiseFactor, terrainAreaData.m_NoiseScale);
				((float2)(ref heightDelta))._002Ector(terrainAreaData.m_HeightOffset, terrainAreaData.m_AbsoluteHeight);
				float num = math.abs(terrainAreaData.m_SlopeWidth);
				float expandAmount = math.max(0f, terrainAreaData.m_SlopeWidth * -1.5f);
				bool flag = (area.m_Flags & AreaFlags.CounterClockwise) != 0;
				if (nativeArray3.Length != 0 && m_PrefabStorageAreaData.TryGetComponent(prefabRef.m_Prefab, ref prefabStorageData))
				{
					Storage storage = nativeArray3[i];
					int num2 = AreaUtils.CalculateStorageCapacity(geometry, prefabStorageData);
					float num3 = (float)(int)((long)storage.m_Amount * 100L / math.max(1, num2)) * 0.015f;
					float num4 = math.min(1f, num3);
					noiseSize.x *= math.clamp(2f - num3, 0.5f, 1f);
					heightDelta.x *= num4;
					num *= math.sqrt(num4);
				}
				for (int j = 0; j < val.Length; j++)
				{
					Triangle triangle = val[j];
					m_Triangles.Enqueue(new AreaTriangle
					{
						m_PositionA = AreaUtils.GetExpandedNode(nodes, triangle.m_Indices.x, expandAmount, isComplete: true, flag),
						m_PositionB = AreaUtils.GetExpandedNode(nodes, triangle.m_Indices.y, expandAmount, isComplete: true, flag),
						m_PositionC = AreaUtils.GetExpandedNode(nodes, triangle.m_Indices.z, expandAmount, isComplete: true, flag),
						m_NoiseSize = noiseSize,
						m_HeightDelta = heightDelta
					});
				}
				float3 expandedNode;
				if (flag)
				{
					expandedNode = AreaUtils.GetExpandedNode(nodes, 0, expandAmount, isComplete: true, flag);
					float2 xz = ((float3)(ref expandedNode)).xz;
					expandedNode = AreaUtils.GetExpandedNode(nodes, 1, expandAmount, isComplete: true, flag);
					float2 val2 = ((float3)(ref expandedNode)).xz;
					expandedNode = AreaUtils.GetExpandedNode(nodes, 2, expandAmount, isComplete: true, flag);
					float2 xz2 = ((float3)(ref expandedNode)).xz;
					float2 val3 = math.normalizesafe(val2 - xz, default(float2));
					float2 val4 = math.normalizesafe(xz2 - val2, default(float2));
					float num5 = MathUtils.RotationAngleRight(-val3, val4);
					for (int k = 0; k < nodes.Length; k++)
					{
						int num6 = k + 3;
						num6 -= math.select(0, nodes.Length, num6 >= nodes.Length);
						xz = val2;
						val2 = xz2;
						expandedNode = AreaUtils.GetExpandedNode(nodes, num6, expandAmount, isComplete: true, flag);
						xz2 = ((float3)(ref expandedNode)).xz;
						val3 = val4;
						val4 = math.normalizesafe(xz2 - val2, default(float2));
						float num7 = num5;
						num5 = MathUtils.RotationAngleRight(-val3, val4);
						m_Edges.Enqueue(new AreaEdge
						{
							m_PositionA = val2,
							m_PositionB = xz,
							m_Angles = new float2(num5, num7),
							m_SideOffset = num
						});
					}
				}
				else
				{
					expandedNode = AreaUtils.GetExpandedNode(nodes, 0, expandAmount, isComplete: true, flag);
					float2 xz3 = ((float3)(ref expandedNode)).xz;
					expandedNode = AreaUtils.GetExpandedNode(nodes, 1, expandAmount, isComplete: true, flag);
					float2 val5 = ((float3)(ref expandedNode)).xz;
					expandedNode = AreaUtils.GetExpandedNode(nodes, 2, expandAmount, isComplete: true, flag);
					float2 xz4 = ((float3)(ref expandedNode)).xz;
					float2 val6 = math.normalizesafe(val5 - xz3, default(float2));
					float2 val7 = math.normalizesafe(xz4 - val5, default(float2));
					float num8 = MathUtils.RotationAngleLeft(-val6, val7);
					for (int l = 0; l < nodes.Length; l++)
					{
						int num9 = l + 3;
						num9 -= math.select(0, nodes.Length, num9 >= nodes.Length);
						xz3 = val5;
						val5 = xz4;
						expandedNode = AreaUtils.GetExpandedNode(nodes, num9, expandAmount, isComplete: true, flag);
						xz4 = ((float3)(ref expandedNode)).xz;
						val6 = val7;
						val7 = math.normalizesafe(xz4 - val5, default(float2));
						float num10 = num8;
						num8 = MathUtils.RotationAngleLeft(-val6, val7);
						m_Edges.Enqueue(new AreaEdge
						{
							m_PositionA = xz3,
							m_PositionB = val5,
							m_Angles = new float2(num10, num8),
							m_SideOffset = num
						});
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct DequeTrianglesJob : IJob
	{
		[ReadOnly]
		public NativeQueue<AreaTriangle> m_Queue;

		public NativeList<AreaTriangle> m_List;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<AreaTriangle> val = m_Queue.ToArray(AllocatorHandle.op_Implicit((Allocator)2));
			m_List.CopyFrom(ref val);
			val.Dispose();
		}
	}

	[BurstCompile]
	private struct DequeEdgesJob : IJob
	{
		[ReadOnly]
		public NativeQueue<AreaEdge> m_Queue;

		public NativeList<AreaEdge> m_List;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<AreaEdge> val = m_Queue.ToArray(AllocatorHandle.op_Implicit((Allocator)2));
			m_List.CopyFrom(ref val);
			val.Dispose();
		}
	}

	[BurstCompile]
	private struct GenerateAreaClipMeshJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentTypeHandle<Clip> m_ClipType;

		[ReadOnly]
		public ComponentTypeHandle<Area> m_AreaType;

		[ReadOnly]
		public BufferTypeHandle<Game.Areas.Node> m_NodeType;

		[ReadOnly]
		public BufferTypeHandle<Triangle> m_TriangleType;

		public MeshDataArray m_MeshData;

		public void Execute()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_0544: Unknown result type (might be due to invalid IL or missing references)
			//IL_0546: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_055f: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				if (((ArchetypeChunk)(ref val)).Has<Clip>(ref m_ClipType))
				{
					BufferAccessor<Game.Areas.Node> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<Game.Areas.Node>(ref m_NodeType);
					BufferAccessor<Triangle> bufferAccessor2 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<Triangle>(ref m_TriangleType);
					for (int j = 0; j < bufferAccessor.Length; j++)
					{
						DynamicBuffer<Game.Areas.Node> val2 = bufferAccessor[j];
						DynamicBuffer<Triangle> val3 = bufferAccessor2[j];
						num += val2.Length * 2;
						num2 += val3.Length * 6 + val2.Length * 6;
					}
				}
			}
			MeshData val4 = ((MeshDataArray)(ref m_MeshData))[0];
			NativeArray<VertexAttributeDescriptor> val5 = default(NativeArray<VertexAttributeDescriptor>);
			val5._002Ector(1, (Allocator)2, (NativeArrayOptions)0);
			val5[0] = new VertexAttributeDescriptor((VertexAttribute)0, (VertexAttributeFormat)0, 4, 0);
			((MeshData)(ref val4)).SetVertexBufferParams(num, val5);
			((MeshData)(ref val4)).SetIndexBufferParams(num2, (IndexFormat)1);
			val5.Dispose();
			((MeshData)(ref val4)).subMeshCount = 1;
			SubMeshDescriptor val6 = default(SubMeshDescriptor);
			((SubMeshDescriptor)(ref val6)).vertexCount = num;
			((SubMeshDescriptor)(ref val6)).indexCount = num2;
			((SubMeshDescriptor)(ref val6)).topology = (MeshTopology)0;
			((MeshData)(ref val4)).SetSubMesh(0, val6, (MeshUpdateFlags)13);
			NativeArray<float4> vertexData = ((MeshData)(ref val4)).GetVertexData<float4>(0);
			NativeArray<uint> indexData = ((MeshData)(ref val4)).GetIndexData<uint>();
			SubMeshDescriptor subMesh = ((MeshData)(ref val4)).GetSubMesh(0);
			Bounds3 val7 = default(Bounds3);
			((Bounds3)(ref val7))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			int num3 = 0;
			int num4 = 0;
			for (int k = 0; k < m_Chunks.Length; k++)
			{
				ArchetypeChunk val8 = m_Chunks[k];
				if (!((ArchetypeChunk)(ref val8)).Has<Clip>(ref m_ClipType))
				{
					continue;
				}
				NativeArray<Area> nativeArray = ((ArchetypeChunk)(ref val8)).GetNativeArray<Area>(ref m_AreaType);
				BufferAccessor<Game.Areas.Node> bufferAccessor3 = ((ArchetypeChunk)(ref val8)).GetBufferAccessor<Game.Areas.Node>(ref m_NodeType);
				BufferAccessor<Triangle> bufferAccessor4 = ((ArchetypeChunk)(ref val8)).GetBufferAccessor<Triangle>(ref m_TriangleType);
				for (int l = 0; l < nativeArray.Length; l++)
				{
					Area area = nativeArray[l];
					DynamicBuffer<Game.Areas.Node> val9 = bufferAccessor3[l];
					DynamicBuffer<Triangle> val10 = bufferAccessor4[l];
					int4 val11 = num3 + new int4(0, 1, val9.Length, val9.Length + 1);
					float num5 = 0f;
					float num6 = 0f;
					for (int m = 0; m < val10.Length; m++)
					{
						Triangle triangle = val10[m];
						int3 indices = triangle.m_Indices;
						num5 = math.min(num5, triangle.m_HeightRange.min);
						num6 = math.max(num6, triangle.m_HeightRange.max);
						int3 val12 = indices + val11.x;
						indexData[num4++] = (uint)val12.z;
						indexData[num4++] = (uint)val12.y;
						indexData[num4++] = (uint)val12.x;
						int3 val13 = indices + val11.z;
						indexData[num4++] = (uint)val13.x;
						indexData[num4++] = (uint)val13.y;
						indexData[num4++] = (uint)val13.z;
					}
					if ((area.m_Flags & AreaFlags.CounterClockwise) != 0)
					{
						for (int n = 0; n < val9.Length; n++)
						{
							int4 val14 = n + val11;
							((int4)(ref val14)).yw = ((int4)(ref val14)).yw - math.select(0, val9.Length, n == val9.Length - 1);
							indexData[num4++] = (uint)val14.x;
							indexData[num4++] = (uint)val14.y;
							indexData[num4++] = (uint)val14.w;
							indexData[num4++] = (uint)val14.w;
							indexData[num4++] = (uint)val14.z;
							indexData[num4++] = (uint)val14.x;
						}
					}
					else
					{
						for (int num7 = 0; num7 < val9.Length; num7++)
						{
							int4 val15 = num7 + val11;
							((int4)(ref val15)).yw = ((int4)(ref val15)).yw - math.select(0, val9.Length, num7 == val9.Length - 1);
							indexData[num4++] = (uint)val15.x;
							indexData[num4++] = (uint)val15.z;
							indexData[num4++] = (uint)val15.w;
							indexData[num4++] = (uint)val15.w;
							indexData[num4++] = (uint)val15.y;
							indexData[num4++] = (uint)val15.x;
						}
					}
					num5 -= 0.3f;
					num6 += 0.3f;
					for (int num8 = 0; num8 < val9.Length; num8++)
					{
						float3 position = val9[num8].m_Position;
						position.y += num5;
						val7 |= position;
						vertexData[num3++] = new float4(position, 0f);
					}
					for (int num9 = 0; num9 < val9.Length; num9++)
					{
						float3 position2 = val9[num9].m_Position;
						position2.y += num6;
						val7 |= position2;
						vertexData[num3++] = new float4(position2, 1f);
					}
				}
			}
			((SubMeshDescriptor)(ref subMesh)).bounds = RenderingUtils.ToBounds(val7);
			((MeshData)(ref val4)).SetSubMesh(0, subMesh, (MeshUpdateFlags)13);
		}
	}

	[BurstCompile]
	private struct CullRoadsCacscadeJob : IJobParallelForDefer
	{
		[ReadOnly]
		public NativeList<LaneSection> m_RoadsToCull;

		[ReadOnly]
		public float4 m_Area;

		[ReadOnly]
		public float m_Scale;

		public ParallelWriter<LaneDraw> Result;

		public void Execute(int index)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			LaneSection laneSection = m_RoadsToCull[index];
			if ((laneSection.m_Flags & LaneFlags.ShiftTerrain) != 0 && !math.any(laneSection.m_Bounds.max < ((float4)(ref m_Area)).xy) && !math.any(laneSection.m_Bounds.min > ((float4)(ref m_Area)).zw))
			{
				float4 minOffset = default(float4);
				float4 maxOffset = default(float4);
				float2 widthOffset = default(float2);
				if ((laneSection.m_Flags & (LaneFlags.MiddleLeft | LaneFlags.MiddleRight)) == (LaneFlags.MiddleLeft | LaneFlags.MiddleRight))
				{
					((float4)(ref minOffset))._002Ector(((float3)(ref laneSection.m_MinOffset)).yyy, 1f);
					((float4)(ref maxOffset))._002Ector(((float3)(ref laneSection.m_MaxOffset)).yyy, 1f);
					widthOffset = float2.op_Implicit(0f);
				}
				else if ((laneSection.m_Flags & LaneFlags.MiddleLeft) != 0)
				{
					((float4)(ref minOffset))._002Ector(((float3)(ref laneSection.m_MinOffset)).yyz, (laneSection.m_MiddleSize - 0.5f) * 2f);
					((float4)(ref maxOffset))._002Ector(((float3)(ref laneSection.m_MaxOffset)).yyz, (laneSection.m_MiddleSize - 0.5f) * 2f);
					((float2)(ref widthOffset))._002Ector(0f, laneSection.m_WidthOffset);
				}
				else if ((laneSection.m_Flags & LaneFlags.MiddleRight) != 0)
				{
					((float4)(ref minOffset))._002Ector(((float3)(ref laneSection.m_MinOffset)).xyy, (laneSection.m_MiddleSize - 0.5f) * 2f);
					((float4)(ref maxOffset))._002Ector(((float3)(ref laneSection.m_MaxOffset)).xyy, (laneSection.m_MiddleSize - 0.5f) * 2f);
					((float2)(ref widthOffset))._002Ector(laneSection.m_WidthOffset, 0f);
				}
				else
				{
					((float4)(ref minOffset))._002Ector(laneSection.m_MinOffset, laneSection.m_MiddleSize);
					((float4)(ref maxOffset))._002Ector(laneSection.m_MaxOffset, laneSection.m_MiddleSize);
					widthOffset = float2.op_Implicit(laneSection.m_WidthOffset);
				}
				LaneDraw laneDraw = new LaneDraw
				{
					m_Left = laneSection.m_Left,
					m_Right = laneSection.m_Right,
					m_MinOffset = minOffset,
					m_MaxOffset = maxOffset,
					m_WidthOffset = widthOffset
				};
				Result.AddNoResize(laneDraw);
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Updated> __Game_Common_Updated_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Orphan> __Game_Net_Orphan_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NodeGeometry> __Game_Net_NodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Game.Areas.Terrain> __Game_Areas_Terrain_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Clip> __Game_Areas_Clip_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Geometry> __Game_Areas_Geometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Lot> __Game_Buildings_Lot_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.Elevation> __Game_Objects_Elevation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Stack> __Game_Objects_Stack_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> __Game_Prefabs_BuildingExtensionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AssetStampData> __Game_Prefabs_AssetStampData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingTerraformData> __Game_Prefabs_BuildingTerraformData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<AdditionalBuildingTerraformElement> __Game_Prefabs_AdditionalBuildingTerraformElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TerrainComposition> __Game_Prefabs_TerrainComposition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Area> __Game_Areas_Area_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Storage> __Game_Areas_Storage_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Areas.Node> __Game_Areas_Node_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Triangle> __Game_Areas_Triangle_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<TerrainAreaData> __Game_Prefabs_TerrainAreaData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageAreaData> __Game_Prefabs_StorageAreaData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
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
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Common_Updated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Updated>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Net_Orphan_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Orphan>(true);
			__Game_Net_NodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NodeGeometry>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(true);
			__Game_Areas_Terrain_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Areas.Terrain>(true);
			__Game_Areas_Clip_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Clip>(true);
			__Game_Areas_Geometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Geometry>(true);
			__Game_Buildings_Lot_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.Lot>(true);
			__Game_Objects_Elevation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.Elevation>(true);
			__Game_Objects_Stack_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Stack>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingExtensionData>(true);
			__Game_Prefabs_AssetStampData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AssetStampData>(true);
			__Game_Prefabs_BuildingTerraformData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingTerraformData>(true);
			__Game_Prefabs_AdditionalBuildingTerraformElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AdditionalBuildingTerraformElement>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Node>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_TerrainComposition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TerrainComposition>(true);
			__Game_Areas_Area_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Area>(true);
			__Game_Areas_Storage_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Storage>(true);
			__Game_Areas_Node_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Triangle>(true);
			__Game_Prefabs_TerrainAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TerrainAreaData>(true);
			__Game_Prefabs_StorageAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageAreaData>(true);
		}
	}

	private const float kShiftTerrainAmount = 2000f;

	private const float kSoftenTerrainAmount = 1000f;

	private const float kSlopeAndLevelTerrainAmount = 4000f;

	public static readonly int kDefaultHeightmapWidth = 4096;

	public static readonly int kDefaultHeightmapHeight = kDefaultHeightmapWidth;

	private static readonly float2 kDefaultMapSize = new float2(14336f, 14336f);

	private static readonly float2 kDefaultMapOffset = kDefaultMapSize * -0.5f;

	private static readonly float2 kDefaultWorldSize = kDefaultMapSize * 4f;

	private static readonly float2 kDefaultWorldOffset = kDefaultWorldSize * -0.5f;

	private static readonly float2 kDefaultHeightScaleOffset = new float2(4096f, 0f);

	private AsyncGPUReadbackHelper m_AsyncGPUReadback;

	private NativeArray<ushort> m_CPUHeights;

	private JobHandle m_CPUHeightReaders;

	private RenderTexture m_Heightmap;

	private RenderTexture m_HeightmapCascade;

	private RenderTexture m_HeightmapDepth;

	private RenderTexture m_WorldMapEditable;

	private Vector4 m_MapOffsetScale;

	private bool m_HeightMapChanged;

	private int4 m_LastPreviewWrite;

	private int4 m_LastWorldPreviewWrite;

	private int4 m_LastWrite;

	private int4 m_LastWorldWrite;

	private int4 m_LastRequest;

	private int m_FailCount;

	private Vector4 m_WorldOffsetScale;

	private bool m_NewMap;

	private bool m_NewMapThisFrame;

	private bool m_Loaded;

	private bool m_HeightsReadyAfterLoading;

	private bool m_UpdateOutOfDate;

	private ComputeShader m_AdjustTerrainCS;

	private int m_ShiftTerrainKernal;

	private int m_BlurHorzKernal;

	private int m_BlurVertKernal;

	private int m_SmoothTerrainKernal;

	private int m_LevelTerrainKernal;

	private int m_SlopeTerrainKernal;

	private CommandBuffer m_CommandBuffer;

	private CommandBuffer m_CascadeCB;

	private Material m_TerrainBlit;

	private Material m_ClipMaterial;

	private EntityQuery m_BrushQuery;

	private NativeList<BuildingUtils.LotInfo> m_BuildingCullList;

	private NativeList<LaneSection> m_LaneCullList;

	private NativeList<AreaTriangle> m_TriangleCullList;

	private NativeList<AreaEdge> m_EdgeCullList;

	private JobHandle m_BuildingCull;

	private JobHandle m_LaneCull;

	private JobHandle m_AreaCull;

	private JobHandle m_ClipMapCull;

	private JobHandle m_CullFinished;

	private NativeParallelHashMap<Entity, Entity> m_BuildingUpgrade;

	private JobHandle m_BuildingUpgradeDependencies;

	public const int kCascadeMax = 4;

	private float4 m_LastCullArea;

	private float4[] m_CascadeRanges;

	private Vector4[] m_ShaderCascadeRanges;

	private float4 m_UpdateArea;

	private float4 m_TerrainChangeArea;

	private bool m_CascadeReset;

	private bool m_RoadUpdate;

	private bool m_AreaUpdate;

	private bool m_TerrainChange;

	private EntityQuery m_BuildingsChanged;

	private EntityQuery m_BuildingGroup;

	private EntityQuery m_RoadsChanged;

	private EntityQuery m_RoadsGroup;

	private EntityQuery m_EditorLotQuery;

	private EntityQuery m_AreasChanged;

	private EntityQuery m_AreasQuery;

	private List<CascadeCullInfo> m_CascadeCulling;

	private ManagedStructuredBuffers<BuildingLotDraw> m_BuildingInstanceData;

	private ManagedStructuredBuffers<LaneDraw> m_LaneInstanceData;

	private ManagedStructuredBuffers<AreaTriangle> m_TriangleInstanceData;

	private ManagedStructuredBuffers<AreaEdge> m_EdgeInstanceData;

	private Material m_MasterBuildingLotMaterial;

	private Material m_MasterLaneMaterial;

	private Material m_MasterAreaMaterial;

	private Mesh m_LaneMesh;

	private ToolSystem m_ToolSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private GroundHeightSystem m_GroundHeightSystem;

	private RenderingSystem m_RenderingSystem;

	private WaterSystem m_WaterSystem;

	private NativeList<ClipMapDraw> m_ClipMapList;

	private ManagedStructuredBuffers<ClipMapDraw> m_ClipMapBuffer;

	private ComputeBuffer m_CurrentClipMap;

	private Mesh m_ClipMesh;

	private Mesh m_AreaClipMesh;

	private MeshDataArray m_AreaClipMeshData;

	private bool m_HasAreaClipMeshData;

	private JobHandle m_AreaClipMeshDataDeps;

	private TerrainMinMaxMap m_TerrainMinMax;

	private TypeHandle __TypeHandle;

	public Vector4 VTScaleOffset => new Vector4(m_WorldOffsetScale.z, m_WorldOffsetScale.w, m_WorldOffsetScale.x, m_WorldOffsetScale.y);

	public bool NewMap => m_NewMapThisFrame;

	public Texture heightmap => (Texture)(object)m_Heightmap;

	public Vector4 mapOffsetScale => m_MapOffsetScale;

	public float2 heightScaleOffset { get; set; }

	public TextureAsset worldMapAsset { get; set; }

	public Texture worldHeightmap { get; set; }

	public float2 playableArea { get; private set; }

	public float2 playableOffset { get; private set; }

	public float2 worldSize { get; private set; }

	public float2 worldOffset { get; private set; }

	public float2 worldHeightMinMax { get; private set; }

	public float3 positionOffset => new float3(playableOffset.x, heightScaleOffset.y, playableOffset.y);

	public bool heightMapRenderRequired { get; private set; }

	public bool[] heightMapSliceUpdated { get; private set; }

	public float4[] heightMapViewport { get; private set; }

	public float4[] heightMapViewportUpdated { get; private set; }

	public float4[] heightMapSliceArea => m_CascadeRanges;

	public float4[] heightMapCullArea { get; private set; }

	public bool freezeCascadeUpdates { get; set; }

	public bool[] heightMapSliceUpdatedLast { get; private set; }

	public float4 lastCullArea => m_LastCullArea;

	public static int baseLod { get; private set; }

	private ComputeBuffer clipMapBuffer
	{
		get
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			if (m_CurrentClipMap == null)
			{
				((JobHandle)(ref m_ClipMapCull)).Complete();
				if (m_ClipMapList.Length > 0)
				{
					NativeArray<ClipMapDraw> data = m_ClipMapList.AsArray();
					m_ClipMapBuffer.StartFrame();
					m_CurrentClipMap = m_ClipMapBuffer.Request(data.Length);
					m_CurrentClipMap.SetData<ClipMapDraw>(data);
					m_ClipMapBuffer.EndFrame();
				}
			}
			return m_CurrentClipMap;
		}
	}

	private int clipMapInstances
	{
		get
		{
			((JobHandle)(ref m_ClipMapCull)).Complete();
			return m_ClipMapList.Length;
		}
	}

	public Mesh areaClipMesh
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)m_AreaClipMesh == (Object)null)
			{
				m_AreaClipMesh = new Mesh();
			}
			if (m_HasAreaClipMeshData)
			{
				m_HasAreaClipMeshData = false;
				((JobHandle)(ref m_AreaClipMeshDataDeps)).Complete();
				Mesh.ApplyAndDisposeWritableMeshData(m_AreaClipMeshData, m_AreaClipMesh, (MeshUpdateFlags)9);
			}
			return m_AreaClipMesh;
		}
		private set
		{
			m_AreaClipMesh = value;
		}
	}

	private float GetTerrainAdjustmentSpeed(TerraformingType type)
	{
		return type switch
		{
			TerraformingType.Soften => 1000f, 
			TerraformingType.Shift => 2000f, 
			_ => 4000f, 
		};
	}

	public Bounds GetTerrainBounds()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		float3 val = new float3(0f, (0f - heightScaleOffset.y) * 0.5f, 0f);
		float3 val2 = default(float3);
		((float3)(ref val2))._002Ector(14336f, heightScaleOffset.x, 14336f);
		return new Bounds(float3.op_Implicit(val), float3.op_Implicit(val2));
	}

	public TerrainHeightData GetHeightData(bool waitForPending = false)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		if (waitForPending && m_HeightMapChanged)
		{
			((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).WaitForCompletion();
			((JobHandle)(ref m_CPUHeightReaders)).Complete();
			m_CPUHeightReaders = default(JobHandle);
			UpdateGPUReadback();
		}
		int3 val = default(int3);
		if (!m_CPUHeights.IsCreated || (Object)(object)m_HeightmapCascade == (Object)null || m_CPUHeights.Length != ((Texture)m_HeightmapCascade).width * ((Texture)m_HeightmapCascade).height)
		{
			((int3)(ref val))._002Ector(2, 2, 2);
		}
		else
		{
			((int3)(ref val))._002Ector(((Texture)m_HeightmapCascade).width, 65536, ((Texture)m_HeightmapCascade).height);
		}
		float3 val2 = default(float3);
		((float3)(ref val2))._002Ector(14336f, math.max(1f, heightScaleOffset.x), 14336f);
		float3 scale = new float3((float)val.x, (float)(val.y - 1), (float)val.z) / val2;
		float3 offset = -positionOffset;
		((float3)(ref offset)).xz = ((float3)(ref offset)).xz - 0.5f / ((float3)(ref scale)).xz;
		return new TerrainHeightData(m_CPUHeights, val, scale, offset);
	}

	public void AddCPUHeightReader(JobHandle handle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_CPUHeightReaders = JobHandle.CombineDependencies(m_CPUHeightReaders, handle);
	}

	public NativeList<LaneSection> GetRoads()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_LaneCull)).Complete();
		return m_LaneCullList;
	}

	public bool GetTerrainBrushUpdate(out float4 viewport)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		viewport = m_TerrainChangeArea;
		if (m_TerrainChange)
		{
			m_TerrainChange = false;
			viewport = new float4(m_TerrainChangeArea.x - m_CascadeRanges[baseLod].x, m_TerrainChangeArea.y - m_CascadeRanges[baseLod].y, m_TerrainChangeArea.z - m_CascadeRanges[baseLod].x, m_TerrainChangeArea.w - m_CascadeRanges[baseLod].y);
			viewport /= new float4(m_CascadeRanges[baseLod].z - m_CascadeRanges[baseLod].x, m_CascadeRanges[baseLod].w - m_CascadeRanges[baseLod].y, m_CascadeRanges[baseLod].z - m_CascadeRanges[baseLod].x, m_CascadeRanges[baseLod].w - m_CascadeRanges[baseLod].y);
			((float4)(ref viewport)).zw = ((float4)(ref viewport)).zw - ((float4)(ref viewport)).xy;
			viewport = ClipViewport(viewport);
			m_TerrainChangeArea = viewport;
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Expected O, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Expected O, but got Unknown
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Expected O, but got Unknown
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Expected O, but got Unknown
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Expected O, but got Unknown
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Expected O, but got Unknown
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Expected O, but got Unknown
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Expected O, but got Unknown
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Expected O, but got Unknown
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0762: Unknown result type (might be due to invalid IL or missing references)
		//IL_0767: Unknown result type (might be due to invalid IL or missing references)
		//IL_0776: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Expected O, but got Unknown
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Expected O, but got Unknown
		//IL_0808: Unknown result type (might be due to invalid IL or missing references)
		//IL_080d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0814: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_082d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0832: Unknown result type (might be due to invalid IL or missing references)
		//IL_0839: Unknown result type (might be due to invalid IL or missing references)
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0850: Unknown result type (might be due to invalid IL or missing references)
		//IL_085f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0866: Expected O, but got Unknown
		//IL_0870: Unknown result type (might be due to invalid IL or missing references)
		//IL_0875: Unknown result type (might be due to invalid IL or missing references)
		//IL_087c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0881: Unknown result type (might be due to invalid IL or missing references)
		//IL_0888: Unknown result type (might be due to invalid IL or missing references)
		//IL_088d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08be: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08de: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f9: Expected O, but got Unknown
		//IL_0903: Unknown result type (might be due to invalid IL or missing references)
		//IL_0908: Unknown result type (might be due to invalid IL or missing references)
		//IL_090f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0914: Unknown result type (might be due to invalid IL or missing references)
		//IL_091b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0920: Unknown result type (might be due to invalid IL or missing references)
		//IL_0934: Unknown result type (might be due to invalid IL or missing references)
		//IL_0939: Unknown result type (might be due to invalid IL or missing references)
		//IL_0940: Unknown result type (might be due to invalid IL or missing references)
		//IL_0945: Unknown result type (might be due to invalid IL or missing references)
		//IL_094c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0951: Unknown result type (might be due to invalid IL or missing references)
		//IL_0965: Unknown result type (might be due to invalid IL or missing references)
		//IL_096a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0971: Unknown result type (might be due to invalid IL or missing references)
		//IL_0976: Unknown result type (might be due to invalid IL or missing references)
		//IL_0983: Unknown result type (might be due to invalid IL or missing references)
		//IL_0988: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_LastCullArea = float4.zero;
		freezeCascadeUpdates = false;
		m_CPUHeights = new NativeArray<ushort>(4, (Allocator)4, (NativeArrayOptions)1);
		m_AdjustTerrainCS = Resources.Load<ComputeShader>("AdjustTerrain");
		m_ShiftTerrainKernal = m_AdjustTerrainCS.FindKernel("ShiftTerrain");
		m_BlurHorzKernal = m_AdjustTerrainCS.FindKernel("HorzBlur");
		m_BlurVertKernal = m_AdjustTerrainCS.FindKernel("VertBlur");
		m_SmoothTerrainKernal = m_AdjustTerrainCS.FindKernel("SmoothTerrain");
		m_LevelTerrainKernal = m_AdjustTerrainCS.FindKernel("LevelTerrain");
		m_SlopeTerrainKernal = m_AdjustTerrainCS.FindKernel("SlopeTerrain");
		m_BuildingUpgrade = new NativeParallelHashMap<Entity, Entity>(1024, AllocatorHandle.op_Implicit((Allocator)4));
		m_CommandBuffer = new CommandBuffer();
		m_CommandBuffer.name = "TerrainAdjust";
		m_CascadeCB = new CommandBuffer();
		m_CascadeCB.name = "Terrain Cascade";
		Shader val = Resources.Load<Shader>("BuildingLot");
		m_MasterBuildingLotMaterial = new Material(val);
		Shader val2 = Resources.Load<Shader>("Lane");
		m_MasterLaneMaterial = new Material(val2);
		Shader val3 = Resources.Load<Shader>("Area");
		m_MasterAreaMaterial = new Material(val3);
		m_TerrainBlit = CoreUtils.CreateEngineMaterial(Resources.Load<Shader>("TerrainCascadeBlit"));
		m_ClipMaterial = CoreUtils.CreateEngineMaterial(Resources.Load<Shader>("RoadClip"));
		m_TerrainMinMax = new TerrainMinMaxMap();
		m_MapOffsetScale = new Vector4(0f, 0f, 1f, 1f);
		m_UpdateArea = float4.zero;
		m_TerrainChangeArea = float4.zero;
		m_TerrainChange = false;
		m_BuildingCullList = new NativeList<BuildingUtils.LotInfo>(1000, AllocatorHandle.op_Implicit((Allocator)4));
		m_LaneCullList = new NativeList<LaneSection>(1000, AllocatorHandle.op_Implicit((Allocator)4));
		m_TriangleCullList = new NativeList<AreaTriangle>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_EdgeCullList = new NativeList<AreaEdge>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_ClipMapList = new NativeList<ClipMapDraw>(1000, AllocatorHandle.op_Implicit((Allocator)4));
		m_CascadeCulling = new List<CascadeCullInfo>(4);
		for (int i = 0; i < 4; i++)
		{
			m_CascadeCulling.Add(new CascadeCullInfo(m_MasterBuildingLotMaterial, m_MasterLaneMaterial, m_MasterAreaMaterial));
		}
		m_BuildingInstanceData = new ManagedStructuredBuffers<BuildingLotDraw>(10000);
		m_LaneInstanceData = new ManagedStructuredBuffers<LaneDraw>(10000);
		m_TriangleInstanceData = new ManagedStructuredBuffers<AreaTriangle>(1000);
		m_EdgeInstanceData = new ManagedStructuredBuffers<AreaEdge>(1000);
		m_LastPreviewWrite = int4.zero;
		m_LastWorldPreviewWrite = int4.zero;
		m_LastWorldWrite = int4.zero;
		m_LastWrite = int4.zero;
		m_LastRequest = int4.zero;
		m_FailCount = 0;
		baseLod = 0;
		m_NewMap = true;
		m_NewMapThisFrame = true;
		m_CascadeReset = true;
		m_RoadUpdate = false;
		m_AreaUpdate = false;
		m_ClipMapBuffer = new ManagedStructuredBuffers<ClipMapDraw>(10000);
		m_CurrentClipMap = null;
		heightMapRenderRequired = false;
		heightMapSliceUpdated = new bool[4];
		heightMapSliceUpdatedLast = new bool[4];
		heightMapViewport = (float4[])(object)new float4[4];
		heightMapViewportUpdated = (float4[])(object)new float4[4];
		heightMapCullArea = (float4[])(object)new float4[4];
		m_BrushQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Brush>(),
			ComponentType.Exclude<Hidden>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[3];
		EntityQueryDesc val4 = new EntityQueryDesc();
		val4.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Game.Objects.Object>()
		};
		val4.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Buildings.Lot>(),
			ComponentType.ReadOnly<AssetStamp>(),
			ComponentType.ReadOnly<Pillar>()
		};
		val4.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val4;
		val4 = new EntityQueryDesc();
		val4.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Game.Objects.Object>()
		};
		val4.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Buildings.Lot>(),
			ComponentType.ReadOnly<AssetStamp>(),
			ComponentType.ReadOnly<Pillar>()
		};
		val4.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[1] = val4;
		val4 = new EntityQueryDesc();
		val4.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Game.Objects.Object>()
		};
		val4.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Buildings.Lot>(),
			ComponentType.ReadOnly<AssetStamp>(),
			ComponentType.ReadOnly<Pillar>()
		};
		val4.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[2] = val4;
		m_BuildingsChanged = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val4 = new EntityQueryDesc();
		val4.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Objects.Object>() };
		val4.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Buildings.Lot>(),
			ComponentType.ReadOnly<AssetStamp>(),
			ComponentType.ReadOnly<Pillar>()
		};
		val4.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array2[0] = val4;
		m_BuildingGroup = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		EntityQueryDesc[] array3 = new EntityQueryDesc[2];
		val4 = new EntityQueryDesc();
		val4.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EdgeGeometry>() };
		val4.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val4.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array3[0] = val4;
		val4 = new EntityQueryDesc();
		val4.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<NodeGeometry>() };
		val4.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val4.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array3[1] = val4;
		m_RoadsChanged = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array3);
		EntityQueryDesc[] array4 = new EntityQueryDesc[1];
		val4 = new EntityQueryDesc();
		val4.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<EdgeGeometry>(),
			ComponentType.ReadOnly<NodeGeometry>()
		};
		val4.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array4[0] = val4;
		m_RoadsGroup = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array4);
		EntityQueryDesc[] array5 = new EntityQueryDesc[2];
		val4 = new EntityQueryDesc();
		val4.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Clip>() };
		val4.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val4.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array5[0] = val4;
		val4 = new EntityQueryDesc();
		val4.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Areas.Terrain>() };
		val4.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val4.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array5[1] = val4;
		m_AreasChanged = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array5);
		EntityQueryDesc[] array6 = new EntityQueryDesc[1];
		val4 = new EntityQueryDesc();
		val4.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Clip>(),
			ComponentType.ReadOnly<Game.Areas.Terrain>()
		};
		val4.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array6[0] = val4;
		m_AreasQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array6);
		EntityQueryDesc[] array7 = new EntityQueryDesc[2];
		val4 = new EntityQueryDesc();
		val4.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Buildings.Lot>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val4.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Error>(),
			ComponentType.ReadOnly<Warning>()
		};
		val4.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Hidden>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array7[0] = val4;
		val4 = new EntityQueryDesc();
		val4.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<AssetStamp>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val4.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Error>(),
			ComponentType.ReadOnly<Warning>()
		};
		val4.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Hidden>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array7[1] = val4;
		m_EditorLotQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array7);
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_GroundHeightSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundHeightSystem>();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		CreateRoadMeshes();
		m_Heightmap = null;
		m_HeightmapCascade = null;
		m_HeightmapDepth = null;
		m_WorldMapEditable = null;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		CoreUtils.Destroy((Object)(object)m_TerrainBlit);
		CoreUtils.Destroy((Object)(object)m_ClipMaterial);
		if (m_CPUHeights.IsCreated)
		{
			m_CPUHeights.Dispose();
		}
		CoreUtils.Destroy((Object)(object)m_Heightmap);
		CoreUtils.Destroy((Object)(object)m_HeightmapCascade);
		CoreUtils.Destroy((Object)(object)m_WorldMapEditable);
		TextureAsset obj = worldMapAsset;
		if (obj != null)
		{
			((AssetData)obj).Unload(false);
		}
		CoreUtils.Destroy((Object)(object)m_HeightmapDepth);
		if (m_BuildingCullList.IsCreated)
		{
			((JobHandle)(ref m_CullFinished)).Complete();
			m_BuildingCullList.Dispose();
		}
		if (m_LaneCullList.IsCreated)
		{
			((JobHandle)(ref m_CullFinished)).Complete();
			m_LaneCullList.Dispose();
		}
		if (m_TriangleCullList.IsCreated)
		{
			((JobHandle)(ref m_CullFinished)).Complete();
			m_TriangleCullList.Dispose();
		}
		if (m_EdgeCullList.IsCreated)
		{
			((JobHandle)(ref m_CullFinished)).Complete();
			m_EdgeCullList.Dispose();
		}
		if (m_ClipMapList.IsCreated)
		{
			((JobHandle)(ref m_ClipMapCull)).Complete();
			m_ClipMapList.Dispose();
		}
		if (m_BuildingInstanceData != null)
		{
			m_BuildingInstanceData.Dispose();
			m_BuildingInstanceData = null;
		}
		if (m_LaneInstanceData != null)
		{
			m_LaneInstanceData.Dispose();
			m_LaneInstanceData = null;
		}
		if (m_TriangleInstanceData != null)
		{
			m_TriangleInstanceData.Dispose();
			m_TriangleInstanceData = null;
		}
		if (m_EdgeInstanceData != null)
		{
			m_EdgeInstanceData.Dispose();
			m_EdgeInstanceData = null;
		}
		if (m_ClipMapBuffer != null)
		{
			m_ClipMapBuffer.Dispose();
			m_ClipMapBuffer = null;
		}
		for (int i = 0; i < 4; i++)
		{
			if (!((JobHandle)(ref m_CascadeCulling[i].m_BuildingHandle)).IsCompleted)
			{
				((JobHandle)(ref m_CascadeCulling[i].m_BuildingHandle)).Complete();
			}
			if (m_CascadeCulling[i].m_BuildingRenderList.IsCreated)
			{
				m_CascadeCulling[i].m_BuildingRenderList.Dispose();
			}
			if (!((JobHandle)(ref m_CascadeCulling[i].m_LaneHandle)).IsCompleted)
			{
				((JobHandle)(ref m_CascadeCulling[i].m_LaneHandle)).Complete();
			}
			if (m_CascadeCulling[i].m_LaneRenderList.IsCreated)
			{
				m_CascadeCulling[i].m_LaneRenderList.Dispose();
			}
			if (!((JobHandle)(ref m_CascadeCulling[i].m_AreaHandle)).IsCompleted)
			{
				((JobHandle)(ref m_CascadeCulling[i].m_AreaHandle)).Complete();
			}
			if (m_CascadeCulling[i].m_TriangleRenderList.IsCreated)
			{
				m_CascadeCulling[i].m_TriangleRenderList.Dispose();
			}
			if (m_CascadeCulling[i].m_EdgeRenderList.IsCreated)
			{
				m_CascadeCulling[i].m_EdgeRenderList.Dispose();
			}
		}
		if (m_BuildingUpgrade.IsCreated)
		{
			((JobHandle)(ref m_BuildingUpgradeDependencies)).Complete();
			m_BuildingUpgrade.Dispose();
		}
		m_CascadeCB.Dispose();
		m_CommandBuffer.Dispose();
		m_TerrainMinMax.Dispose();
		base.OnDestroy();
	}

	private unsafe static void SerializeHeightmap<TWriter>(TWriter writer, Texture heightmap) where TWriter : IWriter
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)heightmap == (Object)null)
		{
			((IWriter)writer/*cast due to .constrained prefix*/).Write(0);
			((IWriter)writer/*cast due to .constrained prefix*/).Write(0);
			return;
		}
		int width = heightmap.width;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(width);
		int height = heightmap.height;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(height);
		NativeArray<ushort> val = default(NativeArray<ushort>);
		val._002Ector(heightmap.width * heightmap.height, (Allocator)4, (NativeArrayOptions)1);
		AsyncGPUReadbackRequest val2 = AsyncGPUReadback.RequestIntoNativeArray<ushort>(ref val, heightmap, 0, (Action<AsyncGPUReadbackRequest>)null);
		((AsyncGPUReadbackRequest)(ref val2)).WaitForCompletion();
		NativeArray<byte> val3 = default(NativeArray<byte>);
		val3._002Ector(val.Length * 2, (Allocator)2, (NativeArrayOptions)1);
		NativeCompression.FilterDataBeforeWrite((IntPtr)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr<ushort>(val), (IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr<byte>(val3), (long)val3.Length, 2);
		val.Dispose();
		NativeArray<byte> val4 = val3;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val4);
		val3.Dispose();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		SerializeHeightmap(writer, worldHeightmap);
		SerializeHeightmap(writer, (Texture)(object)m_Heightmap);
		float2 val = heightScaleOffset;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		float2 val2 = playableOffset;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val2);
		float2 val3 = playableArea;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val3);
		float2 val4 = worldOffset;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val4);
		float2 val5 = worldSize;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val5);
		float2 val6 = worldHeightMinMax;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val6);
	}

	private unsafe static Texture2D DeserializeHeightmap<TReader>(TReader reader, string name, ref NativeArray<ushort> unfiltered, bool makeNoLongerReadable) where TReader : IReader
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		int num2 = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
		if (num != 0 && num2 != 0)
		{
			Texture2D val = new Texture2D(num, num2, (GraphicsFormat)21, (TextureCreationFlags)1028)
			{
				hideFlags = (HideFlags)61,
				name = name,
				filterMode = (FilterMode)1,
				wrapMode = (TextureWrapMode)1
			};
			NativeArray<ushort> rawTextureData = val.GetRawTextureData<ushort>();
			try
			{
				Context context = ((IReader)reader).context;
				if (((Context)(ref context)).version >= Version.terrainWaterSnowCompression)
				{
					if (unfiltered.Length != rawTextureData.Length)
					{
						ArrayExtensions.ResizeArray<ushort>(ref unfiltered, rawTextureData.Length);
					}
					NativeArray<byte> val2 = unfiltered.Reinterpret<byte>(2);
					NativeArray<byte> val3 = val2;
					((IReader)reader/*cast due to .constrained prefix*/).Read(val3);
					NativeCompression.UnfilterDataAfterRead((IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr<byte>(val2), (IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr<ushort>(rawTextureData), (long)val2.Length, 2);
				}
				else
				{
					NativeArray<ushort> val4 = rawTextureData;
					((IReader)reader/*cast due to .constrained prefix*/).Read(val4);
				}
				val.Apply(false, makeNoLongerReadable);
				return val;
			}
			finally
			{
				((IDisposable)rawTextureData/*cast due to .constrained prefix*/).Dispose();
			}
		}
		return null;
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		m_Loaded = true;
		Context context = ((IReader)reader).context;
		ContextFormat format = ((Context)(ref context)).format;
		if (!((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.TerrainSystemCleanup))
		{
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version >= Version.terrainGuidToHash)
			{
				Hash128 val = default(Hash128);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
			}
			else
			{
				string text = default(string);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref text);
			}
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.terrainInSaves)
		{
			Texture2D val2 = null;
			TextureAsset val3 = null;
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version >= Version.worldmapInSaves)
			{
				val2 = DeserializeHeightmap(reader, "LoadedWorldHeightMap", ref m_CPUHeights, makeNoLongerReadable: true);
				Texture2D val4 = DeserializeHeightmap(reader, "LoadedHeightmap", ref m_CPUHeights, makeNoLongerReadable: false);
				float2 val5 = default(float2);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref val5);
				float2 inMapCorner = default(float2);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref inMapCorner);
				float2 inMapSize = default(float2);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref inMapSize);
				float2 inWorldCorner = default(float2);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref inWorldCorner);
				float2 inWorldSize = default(float2);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref inWorldSize);
				float2 inWorldHeightMinMax = default(float2);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref inWorldHeightMinMax);
				InitializeTerrainData(val4, val2, val5, inMapCorner, inMapSize, inWorldCorner, inWorldSize, inWorldHeightMinMax);
				if ((AssetData)(object)val3 != (IAssetData)(object)worldMapAsset)
				{
					TextureAsset obj = worldMapAsset;
					if (obj != null)
					{
						((AssetData)obj).Unload(false);
					}
				}
				worldMapAsset = val3;
				Object.Destroy((Object)(object)val4);
				return;
			}
			throw new NotSupportedException($"Saves prior to {Version.worldmapInSaves} are no longer supported");
		}
		throw new NotSupportedException($"Saves prior to {Version.terrainInSaves} are no longer supported");
	}

	public void SetDefaults(Context context)
	{
		m_Loaded = true;
		LoadTerrain();
	}

	public void Clear()
	{
		CoreUtils.Destroy((Object)(object)m_Heightmap);
	}

	public void TerrainHeightsReadyAfterLoading()
	{
		m_HeightsReadyAfterLoading = true;
	}

	private void LoadTerrain()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		InitializeTerrainData(null, null, kDefaultHeightScaleOffset, kDefaultMapOffset, kDefaultMapSize, kDefaultWorldOffset, kDefaultWorldSize, float2.zero);
	}

	private void InitializeTerrainData(Texture2D inMap, Texture2D worldMap, float2 heightScaleOffset, float2 inMapCorner, float2 inMapSize, float2 inWorldCorner, float2 inWorldSize, float2 inWorldHeightMinMax)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = (((Object)(object)inMap != (Object)null) ? inMap : CreateDefaultHeightmap(((Object)(object)worldMap != (Object)null) ? ((Texture)worldMap).width : kDefaultHeightmapWidth, ((Object)(object)worldMap != (Object)null) ? ((Texture)worldMap).height : kDefaultHeightmapHeight));
		SetHeightmap(val);
		SetWorldHeightmap(worldMap, m_ToolSystem.actionMode.IsEditor());
		FinalizeTerrainData(val, worldMap, heightScaleOffset, inMapCorner, inMapSize, inWorldCorner, inWorldSize, inWorldHeightMinMax);
		if ((Object)(object)val != (Object)(object)inMap)
		{
			Object.Destroy((Object)(object)val);
		}
	}

	public void ReplaceHeightmap(Texture2D inMap)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = (((Object)(object)inMap != (Object)null) ? inMap : CreateDefaultHeightmap(((Object)(object)worldHeightmap != (Object)null) ? worldHeightmap.width : kDefaultHeightmapWidth, ((Object)(object)worldHeightmap != (Object)null) ? worldHeightmap.height : kDefaultHeightmapHeight));
		Texture2D val2 = ToR16(val);
		SetHeightmap(val2);
		FinalizeTerrainData(val2, null, heightScaleOffset, kDefaultMapOffset, kDefaultMapSize, kDefaultWorldOffset, kDefaultWorldSize, worldHeightMinMax);
		if ((Object)(object)val2 != (Object)(object)val)
		{
			Object.Destroy((Object)(object)val2);
		}
		if ((Object)(object)val != (Object)(object)inMap)
		{
			Object.Destroy((Object)(object)val);
		}
	}

	public void ReplaceWorldHeightmap(Texture2D inMap)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = ToR16(inMap);
		SetWorldHeightmap(val, m_ToolSystem.actionMode.IsEditor());
		FinalizeTerrainData(null, val, heightScaleOffset, kDefaultMapOffset, kDefaultMapSize, kDefaultWorldOffset, kDefaultWorldSize, float2.zero);
		if ((Object)(object)val != (Object)(object)inMap && (Object)(object)val != (Object)(object)worldHeightmap)
		{
			Object.Destroy((Object)(object)val);
		}
	}

	public void SetTerrainProperties(float2 heightScaleOffset)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		FinalizeTerrainData(null, null, heightScaleOffset, playableOffset, playableArea, worldOffset, worldSize, worldHeightMinMax);
	}

	private void SetHeightmap(Texture2D map)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		if ((Object)(object)m_Heightmap == (Object)null || ((Texture)m_Heightmap).width != ((Texture)map).width || ((Texture)m_Heightmap).height != ((Texture)map).height)
		{
			if ((Object)(object)m_Heightmap != (Object)null)
			{
				m_Heightmap.Release();
				Object.Destroy((Object)(object)m_Heightmap);
			}
			m_Heightmap = new RenderTexture(((Texture)map).width, ((Texture)map).height, 0, (GraphicsFormat)21)
			{
				hideFlags = (HideFlags)61,
				enableRandomWrite = true,
				name = "TerrainHeights",
				filterMode = (FilterMode)1,
				wrapMode = (TextureWrapMode)1
			};
			m_Heightmap.Create();
		}
		Graphics.CopyTexture((Texture)(object)map, (Texture)(object)m_Heightmap);
		if ((Object)(object)worldHeightmap != (Object)null && (worldHeightmap.width != ((Texture)m_Heightmap).width || worldHeightmap.height != ((Texture)m_Heightmap).height))
		{
			DestroyWorldMap();
		}
	}

	private void SetWorldHeightmap(Texture2D map, bool isEditor)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		if ((Object)(object)map == (Object)null || ((Texture)map).width != ((Texture)m_Heightmap).width || ((Texture)map).height != ((Texture)m_Heightmap).height)
		{
			DestroyWorldMap();
		}
		else if (isEditor)
		{
			if ((Object)(object)m_WorldMapEditable == (Object)null || (Object)(object)worldHeightmap != (Object)(object)m_WorldMapEditable || ((Texture)m_WorldMapEditable).width != ((Texture)map).width || ((Texture)m_WorldMapEditable).height != ((Texture)map).height)
			{
				DestroyWorldMap();
				m_WorldMapEditable = new RenderTexture(((Texture)map).width, ((Texture)map).height, 0, (GraphicsFormat)21)
				{
					hideFlags = (HideFlags)61,
					enableRandomWrite = true,
					name = "TerrainWorldHeights",
					filterMode = (FilterMode)1,
					wrapMode = (TextureWrapMode)1
				};
				m_WorldMapEditable.Create();
				worldHeightmap = (Texture)(object)m_WorldMapEditable;
			}
			Graphics.CopyTexture((Texture)(object)map, (Texture)(object)m_WorldMapEditable);
		}
		else
		{
			if ((Object)(object)map != (Object)(object)worldHeightmap && ((Object)(object)m_WorldMapEditable != (Object)null || (Object)(object)worldHeightmap != (Object)null))
			{
				DestroyWorldMap();
			}
			worldHeightmap = (Texture)(object)map;
		}
	}

	private void FinalizeTerrainData(Texture2D map, Texture2D worldMap, float2 heightScaleOffset, float2 inMapCorner, float2 inMapSize, float2 inWorldCorner, float2 inWorldSize, float2 inWorldHeightMinMax)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Expected O, but got Unknown
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Expected O, but got Unknown
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		this.heightScaleOffset = heightScaleOffset;
		if (math.all(inWorldSize == inMapSize) || (Object)(object)worldHeightmap == (Object)null)
		{
			baseLod = 0;
			playableArea = inMapSize;
			worldSize = inMapSize;
			playableOffset = inMapCorner;
			worldOffset = inMapCorner;
		}
		else
		{
			baseLod = 1;
			playableArea = inMapSize;
			worldSize = inWorldSize;
			playableOffset = inMapCorner;
			worldOffset = inWorldCorner;
		}
		m_NewMap = true;
		m_NewMapThisFrame = true;
		m_CascadeReset = true;
		worldHeightMinMax = inWorldHeightMinMax;
		m_WorldOffsetScale = float4.op_Implicit(new float4((playableOffset - worldOffset) / worldSize, playableArea / worldSize));
		float3 val = default(float3);
		((float3)(ref val))._002Ector(playableArea.x, heightScaleOffset.x, playableArea.y);
		float3 val2 = 1f / val;
		float3 val3 = -positionOffset;
		m_MapOffsetScale = new Vector4(0f - positionOffset.x, 0f - positionOffset.z, 1f / val.x, 1f / val.z);
		if ((Object)(object)m_HeightmapCascade == (Object)null || ((Texture)m_HeightmapCascade).width != heightmap.width || ((Texture)m_HeightmapCascade).height != heightmap.height)
		{
			if ((Object)(object)m_HeightmapCascade != (Object)null)
			{
				m_HeightmapCascade.Release();
				Object.Destroy((Object)(object)m_HeightmapCascade);
				m_HeightmapCascade = null;
			}
			m_HeightmapCascade = new RenderTexture(heightmap.width, heightmap.height, 0, (GraphicsFormat)21)
			{
				hideFlags = (HideFlags)61,
				enableRandomWrite = false,
				name = "TerrainHeightsCascade",
				filterMode = (FilterMode)1,
				wrapMode = (TextureWrapMode)1,
				dimension = (TextureDimension)5,
				volumeDepth = 4
			};
			m_HeightmapCascade.Create();
		}
		if ((Object)(object)m_HeightmapDepth == (Object)null || ((Texture)m_HeightmapDepth).width != heightmap.width || ((Texture)m_HeightmapDepth).height != heightmap.height)
		{
			if ((Object)(object)m_HeightmapDepth != (Object)null)
			{
				m_HeightmapDepth.Release();
				Object.Destroy((Object)(object)m_HeightmapDepth);
				m_HeightmapDepth = null;
			}
			m_HeightmapDepth = new RenderTexture(heightmap.width, heightmap.height, 16, (RenderTextureFormat)1, (RenderTextureReadWrite)1)
			{
				name = "HeightmapDepth"
			};
			m_HeightmapDepth.Create();
		}
		if ((Object)(object)map != (Object)null)
		{
			Graphics.CopyTexture((Texture)(object)map, 0, 0, (Texture)(object)m_HeightmapCascade, baseLod, 0);
		}
		m_CascadeRanges = (float4[])(object)new float4[4];
		m_ShaderCascadeRanges = (Vector4[])(object)new Vector4[4];
		for (int i = 0; i < 4; i++)
		{
			m_CascadeRanges[i] = new float4(0f, 0f, 0f, 0f);
		}
		m_CascadeRanges[baseLod] = new float4(playableOffset, playableOffset + playableArea);
		if (baseLod > 0)
		{
			m_CascadeRanges[0] = new float4(worldOffset, worldOffset + worldSize);
			if ((Object)(object)worldMap != (Object)null)
			{
				Graphics.CopyTexture((Texture)(object)worldMap, 0, 0, (Texture)(object)m_HeightmapCascade, 0, 0);
			}
		}
		m_UpdateArea = new float4(m_CascadeRanges[baseLod]);
		Shader.SetGlobalTexture("colossal_TerrainTexture", (Texture)(object)m_Heightmap);
		Shader.SetGlobalVector("colossal_TerrainScale", float4.op_Implicit(new float4(val2, 0f)));
		Shader.SetGlobalVector("colossal_TerrainOffset", float4.op_Implicit(new float4(val3, 0f)));
		Shader.SetGlobalVector("colossal_TerrainCascadeLimit", float4.op_Implicit(new float4(0.5f / (float)((Texture)m_HeightmapCascade).width, 0.5f / (float)((Texture)m_HeightmapCascade).height, 0f, 0f)));
		Shader.SetGlobalTexture("colossal_TerrainTextureArray", (Texture)(object)m_HeightmapCascade);
		Shader.SetGlobalInt("colossal_TerrainTextureArrayBaseLod", baseLod);
		if ((Object)(object)map != (Object)null)
		{
			((JobHandle)(ref m_CPUHeightReaders)).Complete();
			m_CPUHeightReaders = default(JobHandle);
			WriteCPUHeights(map.GetRawTextureData<ushort>());
		}
		m_TerrainMinMax.Init(((Object)(object)worldHeightmap != (Object)null) ? 1024 : 512, ((Object)(object)worldHeightmap != (Object)null) ? worldHeightmap.width : ((Texture)m_Heightmap).width);
		m_TerrainMinMax.UpdateMap(this, (Texture)(object)m_Heightmap, worldHeightmap);
	}

	private void DestroyWorldMap()
	{
		if ((Object)(object)worldHeightmap != (Object)null)
		{
			Texture obj = worldHeightmap;
			RenderTexture val = (RenderTexture)(object)((obj is RenderTexture) ? obj : null);
			if (val != null)
			{
				val.Release();
			}
			Object.Destroy((Object)(object)worldHeightmap);
			worldHeightmap = null;
		}
		if ((Object)(object)m_WorldMapEditable != (Object)null)
		{
			m_WorldMapEditable.Release();
			Object.Destroy((Object)(object)m_WorldMapEditable);
			m_WorldMapEditable = null;
		}
		if ((AssetData)(object)worldMapAsset != (IAssetData)null)
		{
			((AssetData)worldMapAsset).Unload(false);
			worldMapAsset = null;
		}
	}

	private Texture2D CreateDefaultHeightmap(int width, int height)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0036: Expected O, but got Unknown
		Texture2D val = new Texture2D(width, height, (GraphicsFormat)21, (TextureCreationFlags)1028)
		{
			hideFlags = (HideFlags)61,
			name = "DefaultHeightmap",
			filterMode = (FilterMode)1,
			wrapMode = (TextureWrapMode)1
		};
		SetDefaultHeights(val);
		return val;
	}

	private static void SetDefaultHeights(Texture2D targetHeightmap)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ushort> rawTextureData = targetHeightmap.GetRawTextureData<ushort>();
		ushort num = 8191;
		for (int i = 0; i < rawTextureData.Length; i++)
		{
			rawTextureData[i] = num;
		}
		targetHeightmap.Apply(false, false);
	}

	private static Texture2D ToR16(Texture2D textureRGBA64)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		if ((Object)(object)textureRGBA64 != (Object)null && (int)((Texture)textureRGBA64).graphicsFormat != 21)
		{
			NativeArray<ushort> rawTextureData = textureRGBA64.GetRawTextureData<ushort>();
			NativeArray<ushort> val = default(NativeArray<ushort>);
			val._002Ector(((Texture)textureRGBA64).width * ((Texture)textureRGBA64).height, (Allocator)2, (NativeArrayOptions)1);
			for (int i = 0; i < val.Length; i++)
			{
				val[i] = rawTextureData[i * 4];
			}
			Texture2D val2 = new Texture2D(((Texture)textureRGBA64).width, ((Texture)textureRGBA64).height, (GraphicsFormat)21, (TextureCreationFlags)1028);
			val2.SetPixelData<ushort>(val, 0, 0);
			val2.Apply();
			return val2;
		}
		return textureRGBA64;
	}

	public static bool IsValidHeightmapFormat(Texture2D tex)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		if (((Texture)tex).width == kDefaultHeightmapWidth && ((Texture)tex).height == kDefaultHeightmapHeight)
		{
			if ((int)((Texture)tex).graphicsFormat != 21)
			{
				return (int)((Texture)tex).graphicsFormat == 24;
			}
			return true;
		}
		return false;
	}

	private void SaveBitmap(NativeArray<ushort> buffer, int width, int height)
	{
		using BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite("heightmapResult.raw"));
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				binaryWriter.Write(buffer[j + i * width]);
			}
		}
	}

	private void EnsureCPUHeights(int length)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (m_CPUHeights.IsCreated)
		{
			if (m_CPUHeights.Length != length)
			{
				m_CPUHeights.Dispose();
				m_CPUHeights = new NativeArray<ushort>(length, (Allocator)4, (NativeArrayOptions)1);
			}
		}
		else
		{
			m_CPUHeights = new NativeArray<ushort>(length, (Allocator)4, (NativeArrayOptions)1);
		}
	}

	private void WriteCPUHeights(NativeArray<ushort> buffer)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		EnsureCPUHeights(buffer.Length);
		m_CPUHeights.CopyFrom(buffer);
		m_GroundHeightSystem.AfterReadHeights();
	}

	private void WriteCPUHeights(NativeArray<ushort> buffer, int4 offsets)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < offsets.w; i++)
		{
			int num = (offsets.y + i) * ((Texture)m_HeightmapCascade).width + offsets.x;
			NativeArray<ushort>.Copy(buffer, i * offsets.z, m_CPUHeights, num, offsets.z);
		}
		m_GroundHeightSystem.AfterReadHeights();
	}

	private void UpdateGPUReadback()
	{
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		m_TerrainMinMax.Update();
		if (((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).isPending)
		{
			if (!((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).hasError)
			{
				if (((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).done)
				{
					NativeArray<ushort> data = ((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).GetData<ushort>(0);
					WriteCPUHeights(data, m_LastRequest);
					if (m_UpdateOutOfDate)
					{
						m_UpdateOutOfDate = false;
						OnHeightsChanged();
					}
					else
					{
						m_HeightMapChanged = false;
					}
					m_FailCount = 0;
				}
				((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).IncrementFrame();
			}
			else if (++m_FailCount < 10)
			{
				m_GroundHeightSystem.BeforeReadHeights();
				((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).Request((Texture)(object)m_HeightmapCascade, 0, m_LastRequest.x, m_LastRequest.z, m_LastRequest.y, m_LastRequest.w, baseLod, 1, (Action<AsyncGPUReadbackRequest>)null);
			}
			else
			{
				COSystemBase.baseLog.Error((object)"m_AsyncGPUReadback.hasError");
				m_LastRequest = new int4(0, 0, ((Texture)m_HeightmapCascade).width, ((Texture)m_HeightmapCascade).height);
				m_GroundHeightSystem.BeforeReadHeights();
				((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).Request((Texture)(object)m_HeightmapCascade, 0, 0, ((Texture)m_HeightmapCascade).width, 0, ((Texture)m_HeightmapCascade).height, baseLod, 1, (Action<AsyncGPUReadbackRequest>)null);
			}
		}
		else
		{
			m_HeightMapChanged = false;
		}
	}

	public void TriggerAsyncChange()
	{
		m_UpdateOutOfDate = ((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).isPending;
		m_HeightMapChanged = true;
		if (!m_UpdateOutOfDate)
		{
			OnHeightsChanged();
		}
	}

	public void HandleNewMap()
	{
		m_NewMap = false;
	}

	private void OnHeightsChanged()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		m_LastRequest = m_LastWrite;
		m_LastWrite = int4.zero;
		if (m_LastRequest.z == 0 || m_LastRequest.w == 0)
		{
			m_LastRequest = new int4(0, 0, ((Texture)m_HeightmapCascade).width, ((Texture)m_HeightmapCascade).height);
		}
		m_GroundHeightSystem.BeforeReadHeights();
		((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).Request((Texture)(object)m_HeightmapCascade, 0, m_LastRequest.x, m_LastRequest.z, m_LastRequest.y, m_LastRequest.w, baseLod, 1, (Action<AsyncGPUReadbackRequest>)null);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		m_NewMapThisFrame = m_NewMap;
		if (!((Object)(object)m_Heightmap == (Object)null))
		{
			((JobHandle)(ref m_CPUHeightReaders)).Complete();
			m_CPUHeightReaders = default(JobHandle);
			if (!freezeCascadeUpdates)
			{
				UpdateCascades(m_Loaded, m_HeightsReadyAfterLoading);
				m_Loaded = false;
				m_HeightsReadyAfterLoading = false;
			}
			UpdateGPUReadback();
			UpdateGPUTerrain();
		}
	}

	private void UpdateGPUTerrain()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		TerrainSurface validSurface = TerrainSurface.GetValidSurface();
		if (!((Object)(object)validSurface != (Object)null))
		{
			return;
		}
		validSurface.UsesCascade = true;
		GetCascadeInfo(out var _, out validSurface.BaseLOD, out var areas, out var ranges, out var size);
		validSurface.CascadeArea = float4x4.op_Implicit(areas);
		validSurface.CascadeRanges = float4.op_Implicit(ranges);
		validSurface.CascadeSizes = float4.op_Implicit(size);
		validSurface.CascadeTexture = (Texture)(object)m_HeightmapCascade;
		validSurface.TerrainHeightOffset = heightScaleOffset.y;
		validSurface.TerrainHeightScale = heightScaleOffset.x;
		if (validSurface.RenderClipAreas != null)
		{
			return;
		}
		validSurface.RenderClipAreas = delegate(RenderGraphContext ctx, HDCamera hdCamera)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			Camera camera = hdCamera.camera;
			bool flag = false;
			float num = math.tan(math.radians(camera.fieldOfView) * 0.5f) * 0.002f;
			m_ClipMaterial.SetBuffer(ShaderID._RoadData, clipMapBuffer);
			m_ClipMaterial.SetVector(ShaderID._ClipOffset, float4.op_Implicit(new float4(float3.op_Implicit(((Component)camera).transform.position), num)));
			if (clipMapInstances > 0)
			{
				ctx.cmd.DrawMeshInstancedProcedural(m_ClipMesh, 0, m_ClipMaterial, 0, clipMapInstances, (MaterialPropertyBlock)null);
			}
			if (m_RenderingSystem.hideOverlay || m_ToolSystem.activeTool == null || (m_ToolSystem.activeTool.requireAreas & AreaTypeMask.Surfaces) == 0)
			{
				ctx.cmd.DrawMesh(areaClipMesh, Matrix4x4.identity, m_ClipMaterial, 0, 2);
			}
			ctx.cmd.DrawProcedural(Matrix4x4.identity, m_ClipMaterial, flag ? 4 : 3, (MeshTopology)0, 3, 1);
		};
	}

	private void ApplyToTerrain(RenderTexture target, RenderTexture source, float delta, TerraformingType type, Bounds2 area, Brush brush, Texture texture, bool worldMap)
	{
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0845: Unknown result type (might be due to invalid IL or missing references)
		//IL_086f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_089a: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0815: Unknown result type (might be due to invalid IL or missing references)
		//IL_0817: Unknown result type (might be due to invalid IL or missing references)
		//IL_075c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0773: Unknown result type (might be due to invalid IL or missing references)
		//IL_079d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Unknown result type (might be due to invalid IL or missing references)
		//IL_0717: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0911: Unknown result type (might be due to invalid IL or missing references)
		//IL_0935: Unknown result type (might be due to invalid IL or missing references)
		//IL_093a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_0724: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0971: Unknown result type (might be due to invalid IL or missing references)
		//IL_097c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0988: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09be: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a74: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ceb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0daa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ddd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e4e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)target == (Object)null || !target.IsCreated())
		{
			return;
		}
		if (delta == 0f || brush.m_Strength == 0f)
		{
			if (worldMap && (Object)(object)source != (Object)null && m_LastWorldPreviewWrite.z != 0)
			{
				m_CommandBuffer.Clear();
				m_CommandBuffer.CopyTexture(RenderTargetIdentifier.op_Implicit((Texture)(object)source), 0, 0, m_LastWorldPreviewWrite.x, m_LastWorldPreviewWrite.y, m_LastWorldPreviewWrite.z, m_LastWorldPreviewWrite.w, RenderTargetIdentifier.op_Implicit((Texture)(object)target), 0, 0, m_LastWorldPreviewWrite.x, m_LastWorldPreviewWrite.y);
				Graphics.ExecuteCommandBuffer(m_CommandBuffer);
				m_LastWorldPreviewWrite = int4.zero;
			}
			if (!worldMap && (Object)(object)source != (Object)null && m_LastPreviewWrite.z != 0)
			{
				m_CommandBuffer.Clear();
				m_CommandBuffer.CopyTexture(RenderTargetIdentifier.op_Implicit((Texture)(object)source), 0, 0, m_LastPreviewWrite.x, m_LastPreviewWrite.y, m_LastPreviewWrite.z, m_LastPreviewWrite.w, RenderTargetIdentifier.op_Implicit((Texture)(object)target), 0, 0, m_LastPreviewWrite.x, m_LastPreviewWrite.y);
				Graphics.ExecuteCommandBuffer(m_CommandBuffer);
				m_LastPreviewWrite = int4.zero;
			}
			return;
		}
		float num = delta * brush.m_Strength * GetTerrainAdjustmentSpeed(type) / heightScaleOffset.x;
		float2 val = (worldMap ? worldSize : playableArea);
		float2 val2 = (worldMap ? worldOffset : playableOffset);
		float num2 = math.max(val.x, val.y);
		float2 val3 = (((float3)(ref brush.m_Position)).xz - val2) / val;
		m_GroundHeightSystem.GetUpdateBuffer().Add(ref area);
		if (math.lengthsq(m_UpdateArea) > 0f)
		{
			((float4)(ref m_UpdateArea)).xy = math.min(((float4)(ref m_UpdateArea)).xy, area.min);
			((float4)(ref m_UpdateArea)).zw = math.max(((float4)(ref m_UpdateArea)).zw, area.max);
		}
		else
		{
			m_UpdateArea = new float4(area.min, area.max);
		}
		if (!m_TerrainChange)
		{
			m_TerrainChange = true;
			m_TerrainChangeArea = new float4(area.min, area.max);
		}
		else
		{
			((float4)(ref m_TerrainChangeArea)).xy = math.min(((float4)(ref m_TerrainChangeArea)).xy, area.min);
			((float4)(ref m_TerrainChangeArea)).zw = math.max(((float4)(ref m_TerrainChangeArea)).zw, area.max);
		}
		ref float2 min = ref area.min;
		min -= val2;
		ref float2 max = ref area.max;
		max -= val2;
		ref float2 min2 = ref area.min;
		min2 /= val;
		ref float2 max2 = ref area.max;
		max2 /= val;
		int4 val4 = default(int4);
		((int4)(ref val4))._002Ector((int)math.max(math.floor(area.min.x * (float)((Texture)target).width), 0f), (int)math.max(math.floor(area.min.y * (float)((Texture)target).height), 0f), (int)math.min(math.ceil(area.max.x * (float)((Texture)target).width), (float)(((Texture)target).width - 1)), (int)math.min(math.ceil(area.max.y * (float)((Texture)target).height), (float)(((Texture)target).height - 1)));
		Vector4 val5 = default(Vector4);
		((Vector4)(ref val5))._002Ector(val3.x, val3.y, brush.m_Size / num2 * 0.5f, brush.m_Angle);
		int num3 = val4.z - val4.x + 1;
		int num4 = val4.w - val4.y + 1;
		int num5 = (num3 + 7) / 8;
		int num6 = (num4 + 7) / 8;
		m_CommandBuffer.Clear();
		int4 val6 = default(int4);
		((int4)(ref val6))._002Ector(math.max(val4.x - 2, 0), math.max(val4.y - 2, 0), num3 + 4, num4 + 4);
		if (val6.x + val6.z < 0 || val6.x > ((Texture)target).width || val6.y + val6.w < 0 || val6.y > ((Texture)target).height || num3 <= 0 || num4 <= 0)
		{
			return;
		}
		if (val6.x + val6.z > ((Texture)target).width)
		{
			val6.z = ((Texture)target).width - val6.x;
		}
		if (val6.y + val6.w > ((Texture)target).height)
		{
			val6.w = ((Texture)target).height - val6.y;
		}
		if ((Object)(object)source != (Object)null)
		{
			if (worldMap)
			{
				if (m_LastWorldPreviewWrite.z == 0)
				{
					m_CommandBuffer.CopyTexture(RenderTargetIdentifier.op_Implicit((Texture)(object)source), RenderTargetIdentifier.op_Implicit((Texture)(object)target));
				}
				else
				{
					m_CommandBuffer.CopyTexture(RenderTargetIdentifier.op_Implicit((Texture)(object)source), 0, 0, m_LastWorldPreviewWrite.x, m_LastWorldPreviewWrite.y, m_LastWorldPreviewWrite.z, m_LastWorldPreviewWrite.w, RenderTargetIdentifier.op_Implicit((Texture)(object)target), 0, 0, m_LastWorldPreviewWrite.x, m_LastWorldPreviewWrite.y);
				}
				m_LastWorldPreviewWrite = val6;
			}
			else
			{
				if (m_LastPreviewWrite.z == 0)
				{
					m_CommandBuffer.CopyTexture(RenderTargetIdentifier.op_Implicit((Texture)(object)source), RenderTargetIdentifier.op_Implicit((Texture)(object)target));
				}
				else
				{
					m_CommandBuffer.CopyTexture(RenderTargetIdentifier.op_Implicit((Texture)(object)source), 0, 0, m_LastPreviewWrite.x, m_LastPreviewWrite.y, m_LastPreviewWrite.z, m_LastPreviewWrite.w, RenderTargetIdentifier.op_Implicit((Texture)(object)target), 0, 0, m_LastPreviewWrite.x, m_LastPreviewWrite.y);
					float4 val7 = default(float4);
					((float4)(ref val7))._002Ector((float)m_LastPreviewWrite.x * (1f / (float)((Texture)target).width), (float)m_LastPreviewWrite.y * (1f / (float)((Texture)target).width), (float)m_LastPreviewWrite.z * (1f / (float)((Texture)target).width), (float)m_LastPreviewWrite.w * (1f / (float)((Texture)target).width));
					float4 val8 = default(float4);
					((float4)(ref val8))._002Ector(val2 + ((float4)(ref val7)).xy * val, val2 + (((float4)(ref val7)).xy + ((float4)(ref val7)).zw) * val);
					((float4)(ref m_UpdateArea)).xy = math.min(((float4)(ref m_UpdateArea)).xy, ((float4)(ref val8)).xy);
					((float4)(ref m_UpdateArea)).zw = math.max(((float4)(ref m_UpdateArea)).zw, ((float4)(ref val8)).zw);
				}
				m_LastPreviewWrite = val6;
			}
		}
		else if (worldMap)
		{
			if (m_LastWorldWrite.z == 0)
			{
				m_LastWorldWrite = val6;
			}
			else
			{
				int2 val9 = default(int2);
				((int2)(ref val9))._002Ector(math.min(m_LastWorldWrite.x, val6.x), math.min(m_LastWorldWrite.y, val6.y));
				int2 val10 = default(int2);
				((int2)(ref val10))._002Ector(math.max(m_LastWorldWrite.x + m_LastWorldWrite.z, val6.x + val6.z), math.max(m_LastWorldWrite.y + m_LastWorldWrite.w, val6.y + val6.w));
				((int4)(ref m_LastWorldWrite)).xy = val9;
				((int4)(ref m_LastWorldWrite)).zw = val10 - val9;
			}
		}
		else if (m_LastWrite.z == 0)
		{
			m_LastWrite = val6;
		}
		else
		{
			int2 val11 = default(int2);
			((int2)(ref val11))._002Ector(math.min(m_LastWrite.x, val6.x), math.min(m_LastWrite.y, val6.y));
			int2 val12 = default(int2);
			((int2)(ref val12))._002Ector(math.max(m_LastWrite.x + m_LastWrite.z, val6.x + val6.z), math.max(m_LastWrite.y + m_LastWrite.w, val6.y + val6.w));
			((int4)(ref m_LastWrite)).xy = val11;
			((int4)(ref m_LastWrite)).zw = val12 - val11;
		}
		m_CommandBuffer.SetComputeVectorParam(m_AdjustTerrainCS, ShaderID._CenterSizeRotation, val5);
		m_CommandBuffer.SetComputeVectorParam(m_AdjustTerrainCS, ShaderID._Dims, new Vector4(num2, (float)((Texture)target).width, (float)((Texture)target).height, 0f));
		int num7 = 0;
		Vector4 val13 = default(Vector4);
		((Vector4)(ref val13))._002Ector(num, 0f, 0f, 0f);
		Vector4 val14 = Vector4.zero;
		switch (type)
		{
		case TerraformingType.Shift:
			num7 = m_ShiftTerrainKernal;
			break;
		case TerraformingType.Level:
			num7 = m_LevelTerrainKernal;
			val13.y = (brush.m_Target.y - positionOffset.y) / heightScaleOffset.x;
			break;
		case TerraformingType.Slope:
		{
			num7 = m_SlopeTerrainKernal;
			float3 val17 = brush.m_Target - brush.m_Start;
			val13.y = (brush.m_Target.y - positionOffset.y) / heightScaleOffset.x;
			val13.z = (brush.m_Start.y - positionOffset.y) / heightScaleOffset.x;
			val13.w = val17.y / heightScaleOffset.x;
			float4 zero = float4.zero;
			((float4)(ref zero)).xy = math.normalize(((float3)(ref val17)).xz);
			zero.z = 0f - math.dot((((float3)(ref brush.m_Start)).xz - val2) / val, ((float4)(ref zero)).xy);
			zero.w = math.length(((float3)(ref val17)).xz) / num2;
			val14 = float4.op_Implicit(zero);
			break;
		}
		case TerraformingType.Soften:
		{
			RenderTextureDescriptor val15 = default(RenderTextureDescriptor);
			((RenderTextureDescriptor)(ref val15)).autoGenerateMips = false;
			((RenderTextureDescriptor)(ref val15)).bindMS = false;
			((RenderTextureDescriptor)(ref val15)).depthBufferBits = 0;
			((RenderTextureDescriptor)(ref val15)).dimension = (TextureDimension)2;
			((RenderTextureDescriptor)(ref val15)).enableRandomWrite = true;
			((RenderTextureDescriptor)(ref val15)).graphicsFormat = (GraphicsFormat)21;
			((RenderTextureDescriptor)(ref val15)).memoryless = (RenderTextureMemoryless)0;
			((RenderTextureDescriptor)(ref val15)).height = num4 + 8;
			((RenderTextureDescriptor)(ref val15)).width = num3 + 8;
			((RenderTextureDescriptor)(ref val15)).volumeDepth = 1;
			((RenderTextureDescriptor)(ref val15)).mipCount = 1;
			((RenderTextureDescriptor)(ref val15)).msaaSamples = 1;
			((RenderTextureDescriptor)(ref val15)).sRGB = false;
			((RenderTextureDescriptor)(ref val15)).useDynamicScale = false;
			((RenderTextureDescriptor)(ref val15)).useMipMap = false;
			RenderTextureDescriptor val16 = val15;
			m_CommandBuffer.GetTemporaryRT(ShaderID._AvgTerrainHeightsTemp, val16);
			m_CommandBuffer.GetTemporaryRT(ShaderID._BlurTempHorz, val16);
			num7 = m_SmoothTerrainKernal;
			val13.y = ((RenderTextureDescriptor)(ref val16)).width;
			val13.z = ((RenderTextureDescriptor)(ref val16)).height;
			val14.x = 4f;
			val14.y = 4f;
			m_CommandBuffer.SetComputeTextureParam(m_AdjustTerrainCS, m_BlurHorzKernal, ShaderID._Heightmap, RenderTargetIdentifier.op_Implicit((Texture)(object)target));
			m_CommandBuffer.SetComputeVectorParam(m_AdjustTerrainCS, ShaderID._BrushData, val13);
			m_CommandBuffer.SetComputeVectorParam(m_AdjustTerrainCS, ShaderID._Range, new Vector4((float)(val4.x - 4), (float)(val4.y - 4), (float)(val4.z + 4), (float)(val4.w + 4)));
			int num8 = (num3 + 15) / 8;
			int num9 = num4 + 8;
			m_CommandBuffer.DispatchCompute(m_AdjustTerrainCS, m_BlurHorzKernal, num8, num9, 1);
			int num10 = num3 + 8;
			int num11 = (num4 + 15) / 8;
			m_CommandBuffer.DispatchCompute(m_AdjustTerrainCS, m_BlurVertKernal, num10, num11, 1);
			break;
		}
		default:
			num7 = m_ShiftTerrainKernal;
			break;
		}
		int num12 = 2;
		float4 val18 = (((Object)(object)worldHeightmap != (Object)null && !m_ToolSystem.actionMode.IsEditor()) ? new float4((float)num12, (float)num12, (float)(((Texture)target).width - num12), (float)(((Texture)target).height - num12)) : new float4(-1f, -1f, (float)(((Texture)target).width + 1), (float)(((Texture)target).height + 1)));
		float num13 = 10f / heightScaleOffset.x;
		m_CommandBuffer.SetComputeTextureParam(m_AdjustTerrainCS, num7, ShaderID._Heightmap, RenderTargetIdentifier.op_Implicit((Texture)(object)target));
		m_CommandBuffer.SetComputeTextureParam(m_AdjustTerrainCS, num7, ShaderID._BrushTexture, RenderTargetIdentifier.op_Implicit(texture));
		m_CommandBuffer.SetComputeTextureParam(m_AdjustTerrainCS, num7, ShaderID._WorldTexture, RenderTargetIdentifier.op_Implicit((Texture)(((Object)(object)worldHeightmap != (Object)null) ? ((object)worldHeightmap) : ((object)Texture2D.whiteTexture))));
		m_CommandBuffer.SetComputeTextureParam(m_AdjustTerrainCS, num7, ShaderID._WaterTexture, RenderTargetIdentifier.op_Implicit((Texture)(object)m_WaterSystem.WaterTexture));
		m_CommandBuffer.SetComputeVectorParam(m_AdjustTerrainCS, ShaderID._HeightScaleOffset, float4.op_Implicit(new float4(heightScaleOffset.x, heightScaleOffset.y, 0f, 0f)));
		m_CommandBuffer.SetComputeVectorParam(m_AdjustTerrainCS, ShaderID._Range, new Vector4((float)val4.x, (float)val4.y, (float)val4.z, (float)val4.w));
		m_CommandBuffer.SetComputeVectorParam(m_AdjustTerrainCS, ShaderID._BrushData, val13);
		m_CommandBuffer.SetComputeVectorParam(m_AdjustTerrainCS, ShaderID._BrushData2, val14);
		m_CommandBuffer.SetComputeVectorParam(m_AdjustTerrainCS, ShaderID._ClampArea, float4.op_Implicit(val18));
		m_CommandBuffer.SetComputeVectorParam(m_AdjustTerrainCS, ShaderID._WorldOffsetScale, m_WorldOffsetScale);
		m_CommandBuffer.SetComputeFloatParam(m_AdjustTerrainCS, ShaderID._EdgeMaxDifference, num13);
		m_CommandBuffer.DispatchCompute(m_AdjustTerrainCS, num7, num5, num6, 1);
		if (type == TerraformingType.Soften)
		{
			m_CommandBuffer.ReleaseTemporaryRT(ShaderID._AvgTerrainHeightsTemp);
			m_CommandBuffer.ReleaseTemporaryRT(ShaderID._BlurTempHorz);
		}
		Graphics.ExecuteCommandBuffer(m_CommandBuffer);
	}

	public void PreviewBrush(TerraformingType type, Bounds2 area, Brush brush, Texture texture)
	{
	}

	public void ApplyBrush(TerraformingType type, Bounds2 area, Brush brush, Texture texture)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		m_WaterSystem.TerrainWillChangeFromBrush(area);
		ApplyToTerrain(m_Heightmap, null, Time.unscaledDeltaTime, type, area, brush, texture, worldMap: false);
		ApplyToTerrain(m_WorldMapEditable, null, Time.unscaledDeltaTime, type, area, brush, texture, worldMap: true);
		UpdateMinMax(brush, area);
		TriggerAsyncChange();
	}

	public void UpdateMinMax(Brush brush, Bounds2 area)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)worldHeightmap != (Object)null)
		{
			ref float2 min = ref area.min;
			min -= worldOffset;
			ref float2 max = ref area.max;
			max -= worldOffset;
			ref float2 min2 = ref area.min;
			min2 /= worldSize;
			ref float2 max2 = ref area.max;
			max2 /= worldSize;
		}
		else
		{
			ref float2 min3 = ref area.min;
			min3 -= playableOffset;
			ref float2 max3 = ref area.max;
			max3 -= playableOffset;
			ref float2 min4 = ref area.min;
			min4 /= playableArea;
			ref float2 max4 = ref area.max;
			max4 /= playableArea;
		}
		int4 area2 = default(int4);
		((int4)(ref area2))._002Ector((int)math.max(math.floor(area.min.x * (float)((Texture)m_Heightmap).width) - 1f, 0f), (int)math.max(math.floor(area.min.y * (float)((Texture)m_Heightmap).height) - 1f, 0f), (int)math.min(math.ceil(area.max.x * (float)((Texture)m_Heightmap).width) + 1f, (float)(((Texture)m_Heightmap).width - 1)), (int)math.min(math.ceil(area.max.y * (float)((Texture)m_Heightmap).height) + 1f, (float)(((Texture)m_Heightmap).height - 1)));
		((int4)(ref area2)).zw = ((int4)(ref area2)).zw - ((int4)(ref area2)).xy;
		((int4)(ref area2)).zw = math.clamp(((int4)(ref area2)).zw, new int2(((Texture)m_Heightmap).width / m_TerrainMinMax.size, ((Texture)m_Heightmap).height / m_TerrainMinMax.size), new int2(((Texture)m_Heightmap).width, ((Texture)m_Heightmap).height));
		m_TerrainMinMax.RequestUpdate(this, (Texture)(object)m_Heightmap, worldHeightmap, area2);
	}

	public void GetCascadeInfo(out int LODCount, out int baseLOD, out float4x4 areas, out float4 ranges, out float4 size)
	{
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		LODCount = 4;
		baseLOD = baseLod;
		if (m_CascadeRanges != null)
		{
			areas = new float4x4(m_CascadeRanges[0].x, m_CascadeRanges[0].y, m_CascadeRanges[0].z, m_CascadeRanges[0].w, m_CascadeRanges[1].x, m_CascadeRanges[1].y, m_CascadeRanges[1].z, m_CascadeRanges[1].w, m_CascadeRanges[2].x, m_CascadeRanges[2].y, m_CascadeRanges[2].z, m_CascadeRanges[2].w, m_CascadeRanges[3].x, m_CascadeRanges[3].y, m_CascadeRanges[3].z, m_CascadeRanges[3].w);
			ranges = new float4(math.min(m_CascadeRanges[0].z - m_CascadeRanges[0].x, m_CascadeRanges[0].w - m_CascadeRanges[0].y) * 0.75f, math.min(m_CascadeRanges[1].z - m_CascadeRanges[1].x, m_CascadeRanges[1].w - m_CascadeRanges[1].y) * 0.75f, math.min(m_CascadeRanges[2].z - m_CascadeRanges[2].x, m_CascadeRanges[2].w - m_CascadeRanges[2].y) * 0.75f, math.min(m_CascadeRanges[3].z - m_CascadeRanges[3].x, m_CascadeRanges[3].w - m_CascadeRanges[3].y) * 0.75f);
			size = new float4(math.max(m_CascadeRanges[0].z - m_CascadeRanges[0].x, m_CascadeRanges[0].w - m_CascadeRanges[0].y), math.max(m_CascadeRanges[1].z - m_CascadeRanges[1].x, m_CascadeRanges[1].w - m_CascadeRanges[1].y), math.max(m_CascadeRanges[2].z - m_CascadeRanges[2].x, m_CascadeRanges[2].w - m_CascadeRanges[2].y), math.max(m_CascadeRanges[3].z - m_CascadeRanges[3].x, m_CascadeRanges[3].w - m_CascadeRanges[3].y));
		}
		else
		{
			areas = default(float4x4);
			ranges = default(float4);
			size = default(float4);
		}
	}

	public Texture GetCascadeTexture()
	{
		return (Texture)(object)m_HeightmapCascade;
	}

	private bool Overlap(ref float4 A, ref float4 B)
	{
		if (A.x > B.z || B.x > A.z || A.z < B.x || B.z < A.x || A.y > B.w || B.y > A.w || A.w < B.y || B.w < A.y)
		{
			return false;
		}
		return true;
	}

	private float4 ClipViewport(float4 Viewport)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		if (Viewport.x < 0f)
		{
			Viewport.z = math.max(Viewport.z + Viewport.x, 0f);
			Viewport.x = 0f;
		}
		else if (Viewport.x > 1f)
		{
			Viewport.x = 1f;
			Viewport.z = 0f;
		}
		if (Viewport.x + Viewport.z > 1f)
		{
			Viewport.z = math.max(1f - Viewport.x, 0f);
		}
		if (Viewport.y < 0f)
		{
			Viewport.w = math.max(Viewport.w + Viewport.y, 0f);
			Viewport.y = 0f;
		}
		else if (Viewport.y > 1f)
		{
			Viewport.y = 1f;
			Viewport.w = 0f;
		}
		if (Viewport.y + Viewport.w > 1f)
		{
			Viewport.w = math.max(1f - Viewport.y, 0f);
		}
		return Viewport;
	}

	private void UpdateCascades(bool isLoaded, bool heightsReadyAfterLoading)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0925: Unknown result type (might be due to invalid IL or missing references)
		//IL_091d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_092a: Unknown result type (might be due to invalid IL or missing references)
		//IL_092f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0934: Unknown result type (might be due to invalid IL or missing references)
		//IL_0939: Unknown result type (might be due to invalid IL or missing references)
		//IL_094c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0951: Unknown result type (might be due to invalid IL or missing references)
		//IL_0964: Unknown result type (might be due to invalid IL or missing references)
		//IL_0969: Unknown result type (might be due to invalid IL or missing references)
		//IL_097c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0981: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c73: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0997: Unknown result type (might be due to invalid IL or missing references)
		//IL_099c: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e70: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eaa: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a16: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1009: Unknown result type (might be due to invalid IL or missing references)
		//IL_100e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1013: Unknown result type (might be due to invalid IL or missing references)
		//IL_1018: Unknown result type (might be due to invalid IL or missing references)
		//IL_1022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_154d: Unknown result type (might be due to invalid IL or missing references)
		//IL_150a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1520: Unknown result type (might be due to invalid IL or missing references)
		//IL_1525: Unknown result type (might be due to invalid IL or missing references)
		//IL_152a: Unknown result type (might be due to invalid IL or missing references)
		//IL_152d: Unknown result type (might be due to invalid IL or missing references)
		//IL_152f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_1072: Unknown result type (might be due to invalid IL or missing references)
		//IL_1077: Unknown result type (might be due to invalid IL or missing references)
		//IL_1084: Unknown result type (might be due to invalid IL or missing references)
		//IL_1085: Unknown result type (might be due to invalid IL or missing references)
		//IL_1033: Unknown result type (might be due to invalid IL or missing references)
		//IL_15db: Unknown result type (might be due to invalid IL or missing references)
		//IL_15e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_15ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_15fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1601: Unknown result type (might be due to invalid IL or missing references)
		//IL_1606: Unknown result type (might be due to invalid IL or missing references)
		//IL_160b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1618: Unknown result type (might be due to invalid IL or missing references)
		//IL_161f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1624: Unknown result type (might be due to invalid IL or missing references)
		//IL_1636: Unknown result type (might be due to invalid IL or missing references)
		//IL_1638: Unknown result type (might be due to invalid IL or missing references)
		//IL_163d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_081a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0826: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_083a: Unknown result type (might be due to invalid IL or missing references)
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0845: Unknown result type (might be due to invalid IL or missing references)
		//IL_084a: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_079e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_076b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		//IL_113d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1142: Unknown result type (might be due to invalid IL or missing references)
		//IL_1143: Unknown result type (might be due to invalid IL or missing references)
		//IL_1144: Unknown result type (might be due to invalid IL or missing references)
		//IL_1099: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_10be: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1108: Unknown result type (might be due to invalid IL or missing references)
		//IL_111a: Unknown result type (might be due to invalid IL or missing references)
		//IL_111f: Unknown result type (might be due to invalid IL or missing references)
		//IL_103e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0892: Unknown result type (might be due to invalid IL or missing references)
		//IL_089e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_0882: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07be: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_114b: Unknown result type (might be due to invalid IL or missing references)
		//IL_14d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_14d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_15b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_15b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_158d: Unknown result type (might be due to invalid IL or missing references)
		//IL_159a: Unknown result type (might be due to invalid IL or missing references)
		//IL_117d: Unknown result type (might be due to invalid IL or missing references)
		//IL_11bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1201: Unknown result type (might be due to invalid IL or missing references)
		//IL_1243: Unknown result type (might be due to invalid IL or missing references)
		//IL_1285: Unknown result type (might be due to invalid IL or missing references)
		//IL_128a: Unknown result type (might be due to invalid IL or missing references)
		//IL_129d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1336: Unknown result type (might be due to invalid IL or missing references)
		//IL_133b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1340: Unknown result type (might be due to invalid IL or missing references)
		//IL_1353: Unknown result type (might be due to invalid IL or missing references)
		//IL_1365: Unknown result type (might be due to invalid IL or missing references)
		//IL_136a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1385: Unknown result type (might be due to invalid IL or missing references)
		//IL_138a: Unknown result type (might be due to invalid IL or missing references)
		//IL_138f: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_13e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_13fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_140c: Unknown result type (might be due to invalid IL or missing references)
		//IL_141e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1423: Unknown result type (might be due to invalid IL or missing references)
		//IL_1428: Unknown result type (might be due to invalid IL or missing references)
		//IL_142d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1432: Unknown result type (might be due to invalid IL or missing references)
		//IL_1440: Unknown result type (might be due to invalid IL or missing references)
		//IL_1452: Unknown result type (might be due to invalid IL or missing references)
		//IL_1464: Unknown result type (might be due to invalid IL or missing references)
		//IL_1476: Unknown result type (might be due to invalid IL or missing references)
		//IL_147b: Unknown result type (might be due to invalid IL or missing references)
		//IL_148d: Unknown result type (might be due to invalid IL or missing references)
		//IL_149f: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_14b3: Unknown result type (might be due to invalid IL or missing references)
		float3 position = m_CameraUpdateSystem.position;
		float4 val = default(float4);
		((float4)(ref val))._002Ector(0);
		float4 A = m_UpdateArea;
		heightMapRenderRequired = math.lengthsq(A) > 0f;
		m_UpdateArea = float4.zero;
		m_RoadUpdate = m_CascadeReset;
		m_AreaUpdate = m_CascadeReset;
		if (m_CascadeReset)
		{
			heightMapRenderRequired = true;
			A = m_CascadeRanges[baseLod];
		}
		NativeList<Bounds2> updateBuffer = m_GroundHeightSystem.GetUpdateBuffer();
		bool flag = isLoaded || !((EntityQuery)(ref m_BuildingsChanged)).IsEmptyIgnoreFilter;
		ArchetypeChunk val4;
		if (flag || (m_ToolSystem.actionMode.IsEditor() && !((EntityQuery)(ref m_EditorLotQuery)).IsEmpty))
		{
			ComponentTypeHandle<Transform> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PrefabRef> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Updated> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<ObjectGeometryData> componentLookup = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			float4 area;
			if (flag)
			{
				((JobHandle)(ref m_BuildingUpgradeDependencies)).Complete();
				m_BuildingUpgradeDependencies = default(JobHandle);
				EntityQuery val2 = (isLoaded ? m_BuildingGroup : m_BuildingsChanged);
				NativeArray<ArchetypeChunk> val3 = ((EntityQuery)(ref val2)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)2));
				((SystemBase)this).CompleteDependency();
				Entity val6 = default(Entity);
				for (int i = 0; i < val3.Length; i++)
				{
					val4 = val3[i];
					NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val4)).GetNativeArray(entityTypeHandle);
					val4 = val3[i];
					NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref val4)).GetNativeArray<Transform>(ref componentTypeHandle);
					val4 = val3[i];
					NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref val4)).GetNativeArray<PrefabRef>(ref componentTypeHandle2);
					val4 = val3[i];
					bool flag2 = ((ArchetypeChunk)(ref val4)).Has<Updated>(ref componentTypeHandle3);
					if (isLoaded)
					{
						heightMapRenderRequired = true;
						m_WaterSystem.TerrainWillChange();
						A = m_CascadeRanges[baseLod];
						break;
					}
					for (int j = 0; j < nativeArray2.Length; j++)
					{
						PrefabRef prefabRef = nativeArray3[j];
						if (CalculateBuildingCullArea(nativeArray2[j], prefabRef.m_Prefab, componentLookup, out area))
						{
							Bounds2 val5 = new Bounds2(((float4)(ref area)).xy, ((float4)(ref area)).zw);
							updateBuffer.Add(ref val5);
							m_WaterSystem.TerrainWillChange();
							if (!heightMapRenderRequired)
							{
								heightMapRenderRequired = true;
								A = area;
							}
							else
							{
								((float4)(ref A)).xy = math.min(((float4)(ref A)).xy, ((float4)(ref area)).xy);
								((float4)(ref A)).zw = math.max(((float4)(ref A)).zw, ((float4)(ref area)).zw);
							}
						}
						if (!flag2 || !m_BuildingUpgrade.TryGetValue(nativeArray[j], ref val6))
						{
							continue;
						}
						if (val6 != prefabRef.m_Prefab && CalculateBuildingCullArea(nativeArray2[j], val6, componentLookup, out area))
						{
							if (!heightMapRenderRequired)
							{
								heightMapRenderRequired = true;
								A = area;
							}
							else
							{
								((float4)(ref A)).xy = math.min(((float4)(ref A)).xy, ((float4)(ref area)).xy);
								((float4)(ref A)).zw = math.max(((float4)(ref A)).zw, ((float4)(ref area)).zw);
							}
						}
						m_BuildingUpgrade.Remove(nativeArray[j]);
					}
				}
				val3.Dispose();
			}
			if (m_ToolSystem.actionMode.IsEditor() && !((EntityQuery)(ref m_EditorLotQuery)).IsEmpty)
			{
				NativeArray<ArchetypeChunk> val7 = ((EntityQuery)(ref m_EditorLotQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)2));
				((SystemBase)this).CompleteDependency();
				for (int k = 0; k < val7.Length; k++)
				{
					val4 = val7[k];
					NativeArray<Transform> nativeArray4 = ((ArchetypeChunk)(ref val4)).GetNativeArray<Transform>(ref componentTypeHandle);
					val4 = val7[k];
					NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref val4)).GetNativeArray<PrefabRef>(ref componentTypeHandle2);
					for (int l = 0; l < nativeArray4.Length; l++)
					{
						PrefabRef prefabRef2 = nativeArray5[l];
						if (CalculateBuildingCullArea(nativeArray4[l], prefabRef2.m_Prefab, componentLookup, out area))
						{
							m_WaterSystem.TerrainWillChange();
							if (!heightMapRenderRequired)
							{
								heightMapRenderRequired = true;
								A = area;
							}
							else
							{
								((float4)(ref A)).xy = math.min(((float4)(ref A)).xy, ((float4)(ref area)).xy);
								((float4)(ref A)).zw = math.max(((float4)(ref A)).zw, ((float4)(ref area)).zw);
							}
						}
					}
				}
				val7.Dispose();
			}
			m_BuildingUpgrade.Clear();
		}
		if (isLoaded || !((EntityQuery)(ref m_RoadsChanged)).IsEmptyIgnoreFilter)
		{
			EntityQuery val8 = (isLoaded ? m_RoadsGroup : m_RoadsChanged);
			NativeArray<ArchetypeChunk> val9 = ((EntityQuery)(ref val8)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)2));
			EntityTypeHandle entityTypeHandle2 = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PrefabRef> componentTypeHandle4 = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<NetData> componentLookup2 = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<NetGeometryData> componentLookup3 = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<Composition> componentLookup4 = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<Orphan> componentLookup5 = InternalCompilerInterface.GetComponentLookup<Orphan>(ref __TypeHandle.__Game_Net_Orphan_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<NodeGeometry> componentLookup6 = InternalCompilerInterface.GetComponentLookup<NodeGeometry>(ref __TypeHandle.__Game_Net_NodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<EdgeGeometry> componentLookup7 = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<StartNodeGeometry> componentLookup8 = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<EndNodeGeometry> componentLookup9 = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			((SystemBase)this).CompleteDependency();
			NetGeometryData netGeometryData = default(NetGeometryData);
			Bounds3 val11 = default(Bounds3);
			for (int m = 0; m < val9.Length; m++)
			{
				val4 = val9[m];
				NativeArray<Entity> nativeArray6 = ((ArchetypeChunk)(ref val4)).GetNativeArray(entityTypeHandle2);
				val4 = val9[m];
				NativeArray<PrefabRef> nativeArray7 = ((ArchetypeChunk)(ref val4)).GetNativeArray<PrefabRef>(ref componentTypeHandle4);
				if (isLoaded)
				{
					heightMapRenderRequired = true;
					A = m_CascadeRanges[baseLod];
					m_WaterSystem.TerrainWillChange();
					break;
				}
				for (int n = 0; n < nativeArray6.Length; n++)
				{
					Entity val10 = nativeArray6[n];
					if (!componentLookup3.TryGetComponent(nativeArray7[n].m_Prefab, ref netGeometryData) || (netGeometryData.m_Flags & (Game.Net.GeometryFlags.FlattenTerrain | Game.Net.GeometryFlags.ClipTerrain)) == 0)
					{
						continue;
					}
					m_RoadUpdate = true;
					if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.FlattenTerrain) == 0)
					{
						continue;
					}
					((Bounds3)(ref val11))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
					if (componentLookup4.HasComponent(val10))
					{
						EdgeGeometry edgeGeometry = componentLookup7[val10];
						StartNodeGeometry startNodeGeometry = componentLookup8[val10];
						EndNodeGeometry endNodeGeometry = componentLookup9[val10];
						if (math.any(edgeGeometry.m_Start.m_Length + edgeGeometry.m_End.m_Length > 0.1f))
						{
							val11 |= edgeGeometry.m_Bounds;
						}
						if (math.any(startNodeGeometry.m_Geometry.m_Left.m_Length > 0.05f) | math.any(startNodeGeometry.m_Geometry.m_Right.m_Length > 0.05f))
						{
							val11 |= startNodeGeometry.m_Geometry.m_Bounds;
						}
						if (math.any(endNodeGeometry.m_Geometry.m_Left.m_Length > 0.05f) | math.any(endNodeGeometry.m_Geometry.m_Right.m_Length > 0.05f))
						{
							val11 |= endNodeGeometry.m_Geometry.m_Bounds;
						}
					}
					else if (componentLookup5.HasComponent(val10))
					{
						NodeGeometry nodeGeometry = componentLookup6[val10];
						val11 |= nodeGeometry.m_Bounds;
					}
					if (val11.min.x <= val11.max.x)
					{
						NetData netData = componentLookup2[nativeArray7[n].m_Prefab];
						val11 = MathUtils.Expand(val11, float3.op_Implicit(NetUtils.GetTerrainSmoothingWidth(netData) - 8f));
						Bounds2 val5 = ((Bounds3)(ref val11)).xz;
						updateBuffer.Add(ref val5);
						m_WaterSystem.TerrainWillChange();
						if (!heightMapRenderRequired)
						{
							heightMapRenderRequired = true;
							((float4)(ref A))._002Ector(((float3)(ref val11.min)).xz, ((float3)(ref val11.max)).xz);
						}
						else
						{
							((float4)(ref A)).xy = math.min(((float4)(ref A)).xy, ((float3)(ref val11.min)).xz);
							((float4)(ref A)).zw = math.max(((float4)(ref A)).zw, ((float3)(ref val11.max)).xz);
						}
					}
				}
			}
			val9.Dispose();
		}
		bool num = isLoaded || !((EntityQuery)(ref m_AreasChanged)).IsEmptyIgnoreFilter;
		bool flag3 = isLoaded || heightsReadyAfterLoading;
		if (num)
		{
			EntityQuery val12 = (isLoaded ? m_AreasQuery : m_AreasChanged);
			NativeArray<ArchetypeChunk> val13 = ((EntityQuery)(ref val12)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)2));
			ComponentTypeHandle<Game.Areas.Terrain> componentTypeHandle5 = InternalCompilerInterface.GetComponentTypeHandle<Game.Areas.Terrain>(ref __TypeHandle.__Game_Areas_Terrain_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Clip> componentTypeHandle6 = InternalCompilerInterface.GetComponentTypeHandle<Clip>(ref __TypeHandle.__Game_Areas_Clip_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Geometry> componentTypeHandle7 = InternalCompilerInterface.GetComponentTypeHandle<Geometry>(ref __TypeHandle.__Game_Areas_Geometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			((SystemBase)this).CompleteDependency();
			for (int num2 = 0; num2 < val13.Length; num2++)
			{
				bool num3 = flag3;
				val4 = val13[num2];
				flag3 = num3 | ((ArchetypeChunk)(ref val4)).Has<Clip>(ref componentTypeHandle6);
				val4 = val13[num2];
				if (!((ArchetypeChunk)(ref val4)).Has<Game.Areas.Terrain>(ref componentTypeHandle5))
				{
					continue;
				}
				m_AreaUpdate = true;
				val4 = val13[num2];
				NativeArray<Geometry> nativeArray8 = ((ArchetypeChunk)(ref val4)).GetNativeArray<Geometry>(ref componentTypeHandle7);
				if (isLoaded)
				{
					heightMapRenderRequired = true;
					A = m_CascadeRanges[baseLod];
					break;
				}
				for (int num4 = 0; num4 < nativeArray8.Length; num4++)
				{
					Bounds3 bounds = nativeArray8[num4].m_Bounds;
					if (bounds.min.x <= bounds.max.x)
					{
						Bounds2 val5 = ((Bounds3)(ref bounds)).xz;
						updateBuffer.Add(ref val5);
						if (!heightMapRenderRequired)
						{
							heightMapRenderRequired = true;
							((float4)(ref A))._002Ector(((float3)(ref bounds.min)).xz, ((float3)(ref bounds.max)).xz);
						}
						else
						{
							((float4)(ref A)).xy = math.min(((float4)(ref A)).xy, ((float3)(ref bounds.min)).xz);
							((float4)(ref A)).zw = math.max(((float4)(ref A)).zw, ((float3)(ref bounds.max)).xz);
						}
					}
				}
			}
			val13.Dispose();
		}
		if (heightMapRenderRequired)
		{
			A += new float4(-10f, -10f, 10f, 10f);
		}
		float4 val14 = A;
		for (int num5 = 0; num5 <= baseLod; num5++)
		{
			if (heightMapRenderRequired)
			{
				heightMapViewport[num5] = new float4(A.x - m_CascadeRanges[num5].x, A.y - m_CascadeRanges[num5].y, A.z - m_CascadeRanges[num5].x, A.w - m_CascadeRanges[num5].y);
				ref float4 reference = ref heightMapViewport[num5];
				reference /= new float4(m_CascadeRanges[num5].z - m_CascadeRanges[num5].x, m_CascadeRanges[num5].w - m_CascadeRanges[num5].y, m_CascadeRanges[num5].z - m_CascadeRanges[num5].x, m_CascadeRanges[num5].w - m_CascadeRanges[num5].y);
				ref float4 reference2 = ref heightMapViewport[num5];
				((float4)(ref reference2)).zw = ((float4)(ref reference2)).zw - ((float4)(ref heightMapViewport[num5])).xy;
				heightMapViewport[num5] = ClipViewport(heightMapViewport[num5]);
				heightMapSliceUpdated[num5] = heightMapViewport[num5].w > 0f && heightMapViewport[num5].z > 0f;
				((float4)(ref val14)).xy = math.min(((float4)(ref val14)).xy, ((float4)(ref m_CascadeRanges[num5])).xy + ((float4)(ref heightMapViewport[num5])).xy * (((float4)(ref m_CascadeRanges[num5])).zw - ((float4)(ref m_CascadeRanges[num5])).xy));
				((float4)(ref val14)).zw = math.max(((float4)(ref val14)).zw, ((float4)(ref m_CascadeRanges[num5])).xy + (((float4)(ref heightMapViewport[num5])).xy + ((float4)(ref heightMapViewport[num5])).zw) * (((float4)(ref m_CascadeRanges[num5])).zw - ((float4)(ref m_CascadeRanges[num5])).xy));
			}
			else
			{
				heightMapViewport[num5] = float4.zero;
				heightMapSliceUpdated[num5] = false;
			}
		}
		for (int num6 = baseLod + 1; num6 < 4; num6++)
		{
			float2 val15 = ((float4)(ref m_CascadeRanges[baseLod])).zw - ((float4)(ref m_CascadeRanges[baseLod])).xy;
			val15 /= math.pow(2f, (float)(num6 - baseLod));
			float num7 = math.min(val15.x, val15.y) / 4f;
			((float4)(ref val)).xy = ((float3)(ref position)).xz - val15 * 0.5f;
			((float4)(ref val)).zw = ((float3)(ref position)).xz + val15 * 0.5f;
			if (val.x < m_CascadeRanges[0].x)
			{
				float num8 = m_CascadeRanges[0].x - val.x;
				val.x += num8;
				val.z += num8;
			}
			if (val.y < m_CascadeRanges[0].y)
			{
				float num9 = m_CascadeRanges[0].y - val.y;
				val.y += num9;
				val.w += num9;
			}
			if (val.z > m_CascadeRanges[0].z)
			{
				float num10 = m_CascadeRanges[0].z - val.z;
				val.x += num10;
				val.z += num10;
			}
			if (val.w > m_CascadeRanges[0].w)
			{
				float num11 = m_CascadeRanges[0].w - val.w;
				val.y += num11;
				val.w += num11;
			}
			float2 val16 = math.abs(((float4)(ref val)).xy - new float2(m_CascadeRanges[num6].x, m_CascadeRanges[num6].y));
			if (math.lengthsq(m_CascadeRanges[num6]) == 0f || val16.x > num7 || val16.y > num7)
			{
				heightMapSliceUpdated[num6] = true;
				heightMapViewport[num6] = new float4(0f, 0f, 1f, 1f);
				m_CascadeRanges[num6] = val;
				if (heightMapRenderRequired)
				{
					((float4)(ref A)).xy = math.min(((float4)(ref A)).xy, ((float4)(ref m_CascadeRanges[num6])).xy);
					((float4)(ref A)).zw = math.max(((float4)(ref A)).zw, ((float4)(ref m_CascadeRanges[num6])).zw);
					((float4)(ref val14)).xy = math.min(((float4)(ref val14)).xy, ((float4)(ref m_CascadeRanges[num6])).xy);
					((float4)(ref val14)).zw = math.max(((float4)(ref val14)).zw, ((float4)(ref m_CascadeRanges[num6])).zw);
				}
				else
				{
					heightMapRenderRequired = true;
					A = m_CascadeRanges[num6];
					val14 = A;
				}
			}
			else if (math.lengthsq(A) > 0f && Overlap(ref A, ref m_CascadeRanges[num6]))
			{
				heightMapViewport[num6] = new float4(math.clamp(A.x, m_CascadeRanges[num6].x, m_CascadeRanges[num6].z) - m_CascadeRanges[num6].x, math.clamp(A.y, m_CascadeRanges[num6].y, m_CascadeRanges[num6].w) - m_CascadeRanges[num6].y, math.clamp(A.z, m_CascadeRanges[num6].x, m_CascadeRanges[num6].z) - m_CascadeRanges[num6].x, math.clamp(A.w, m_CascadeRanges[num6].y, m_CascadeRanges[num6].w) - m_CascadeRanges[num6].y);
				ref float4 reference3 = ref heightMapViewport[num6];
				reference3 /= new float4(m_CascadeRanges[num6].z - m_CascadeRanges[num6].x, m_CascadeRanges[num6].w - m_CascadeRanges[num6].y, m_CascadeRanges[num6].z - m_CascadeRanges[num6].x, m_CascadeRanges[num6].w - m_CascadeRanges[num6].y);
				ref float4 reference4 = ref heightMapViewport[num6];
				((float4)(ref reference4)).zw = ((float4)(ref reference4)).zw - ((float4)(ref heightMapViewport[num6])).xy;
				heightMapViewport[num6] = ClipViewport(heightMapViewport[num6]);
				heightMapSliceUpdated[num6] = heightMapViewport[num6].w > 0f && heightMapViewport[num6].z > 0f;
				((float4)(ref val14)).xy = math.min(((float4)(ref val14)).xy, ((float4)(ref m_CascadeRanges[num6])).xy + ((float4)(ref heightMapViewport[num6])).xy * (((float4)(ref m_CascadeRanges[num6])).zw - ((float4)(ref m_CascadeRanges[num6])).xy));
				((float4)(ref val14)).zw = math.max(((float4)(ref val14)).zw, ((float4)(ref m_CascadeRanges[num6])).xy + (((float4)(ref heightMapViewport[num6])).xy + ((float4)(ref heightMapViewport[num6])).zw) * (((float4)(ref m_CascadeRanges[num6])).zw - ((float4)(ref m_CascadeRanges[num6])).xy));
			}
			else
			{
				heightMapSliceUpdated[num6] = false;
				heightMapViewport[num6] = float4.zero;
			}
		}
		if (heightMapRenderRequired || m_RoadUpdate || flag3)
		{
			if (heightMapRenderRequired)
			{
				val14 = (m_LastCullArea = val14 + new float4(-10f, -10f, 10f, 10f));
				heightMapSliceUpdatedLast = heightMapSliceUpdated;
				heightMapViewportUpdated = heightMapViewport;
			}
			CullForCascades(val14, heightMapRenderRequired, m_RoadUpdate, m_AreaUpdate, flag3, out var laneCount);
			if (heightMapRenderRequired)
			{
				for (int num12 = 3; num12 >= baseLod; num12--)
				{
					if (heightMapSliceUpdated[num12])
					{
						CullCascade(num12, m_CascadeRanges[num12], heightMapViewport[num12], laneCount);
					}
					else
					{
						heightMapCullArea[num12] = float4.zero;
					}
				}
			}
			JobHandle.ScheduleBatchedJobs();
		}
		for (int num13 = 0; num13 < 4; num13++)
		{
			float4 val17 = m_CascadeRanges[num13];
			((float4)(ref val17)).zw = 1f / math.max(float2.op_Implicit(0.001f), ((float4)(ref val17)).zw - ((float4)(ref val17)).xy);
			((float4)(ref val17)).xy = ((float4)(ref val17)).xy * ((float4)(ref val17)).zw;
			m_ShaderCascadeRanges[num13] = float4.op_Implicit(val17);
		}
		Shader.SetGlobalVectorArray(ShaderID._CascadeRangesID, m_ShaderCascadeRanges);
		m_CascadeReset = false;
	}

	public void RenderCascades()
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		if (heightMapRenderRequired)
		{
			m_GroundHeightSystem.BeforeUpdateHeights();
			m_CascadeCB.Clear();
			m_BuildingInstanceData.StartFrame();
			m_LaneInstanceData.StartFrame();
			m_TriangleInstanceData.StartFrame();
			m_EdgeInstanceData.StartFrame();
			if (baseLod != 0)
			{
				Texture val = (Texture)(((Object)(object)m_WorldMapEditable != (Object)null) ? ((object)m_WorldMapEditable) : ((object)worldHeightmap));
				m_TerrainBlit.SetTexture("_WorldMap", val);
			}
			for (int num = 3; num >= baseLod; num--)
			{
				if (heightMapSliceUpdated[num])
				{
					RenderCascade(num, m_CascadeRanges[num], heightMapViewport[num], ref m_CascadeCB);
				}
			}
			if (baseLod > 0 && heightMapSliceUpdated[0])
			{
				int4 val2 = default(int4);
				((int4)(ref val2))._002Ector((int)(heightMapViewport[0].x * (float)((Texture)m_HeightmapCascade).width), (int)(heightMapViewport[0].y * (float)((Texture)m_HeightmapCascade).height), (int)(heightMapViewport[0].z * (float)((Texture)m_HeightmapCascade).width), (int)(heightMapViewport[0].w * (float)((Texture)m_HeightmapCascade).height));
				if (m_LastWorldWrite.z == 0)
				{
					m_LastWorldWrite = val2;
				}
				else
				{
					int2 val3 = default(int2);
					((int2)(ref val3))._002Ector(math.min(m_LastWorldWrite.x, val2.x), math.min(m_LastWorldWrite.y, val2.y));
					int2 val4 = default(int2);
					((int2)(ref val4))._002Ector(math.max(m_LastWorldWrite.x + m_LastWorldWrite.z, val2.x + val2.z), math.max(m_LastWorldWrite.y + m_LastWorldWrite.w, val2.y + val2.w));
					((int4)(ref m_LastWorldWrite)).xy = val3;
					((int4)(ref m_LastWorldWrite)).zw = val4 - val3;
				}
				RenderWorldMapToCascade(m_CascadeRanges[0], heightMapViewport[0], ref m_CascadeCB);
			}
			m_BuildingInstanceData.EndFrame();
			m_LaneInstanceData.EndFrame();
			m_TriangleInstanceData.EndFrame();
			m_EdgeInstanceData.EndFrame();
			Graphics.ExecuteCommandBuffer(m_CascadeCB);
			if (heightMapSliceUpdated[baseLod])
			{
				int4 val5 = default(int4);
				((int4)(ref val5))._002Ector((int)(heightMapViewport[baseLod].x * (float)((Texture)m_HeightmapCascade).width), (int)(heightMapViewport[baseLod].y * (float)((Texture)m_HeightmapCascade).height), (int)(heightMapViewport[baseLod].z * (float)((Texture)m_HeightmapCascade).width), (int)(heightMapViewport[baseLod].w * (float)((Texture)m_HeightmapCascade).height));
				if (m_LastWrite.z == 0)
				{
					m_LastWrite = val5;
				}
				else
				{
					int2 val6 = default(int2);
					((int2)(ref val6))._002Ector(math.min(m_LastWrite.x, val5.x), math.min(m_LastWrite.y, val5.y));
					int2 val7 = default(int2);
					((int2)(ref val7))._002Ector(math.max(m_LastWrite.x + m_LastWrite.z, val5.x + val5.z), math.max(m_LastWrite.y + m_LastWrite.w, val5.y + val5.w));
					((int4)(ref m_LastWrite)).xy = val6;
					((int4)(ref m_LastWrite)).zw = val7 - val6;
				}
				TriggerAsyncChange();
			}
		}
		m_CascadeReset = false;
	}

	private void CullForCascades(float4 area, bool heightMapRenderRequired, bool roadsChanged, bool terrainAreasChanged, bool clipAreasChanged, out int laneCount)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0820: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Unknown result type (might be due to invalid IL or missing references)
		//IL_076d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_078a: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07de: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0801: Unknown result type (might be due to invalid IL or missing references)
		//IL_0809: Unknown result type (might be due to invalid IL or missing references)
		//IL_080f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0814: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_0729: Unknown result type (might be due to invalid IL or missing references)
		//IL_072e: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_CullFinished)).Complete();
		if (roadsChanged)
		{
			((JobHandle)(ref m_ClipMapCull)).Complete();
			m_LaneCullList.Clear();
			m_ClipMapList.Clear();
			laneCount = ((EntityQuery)(ref m_RoadsGroup)).CalculateEntityCountWithoutFiltering() * 6;
			if (laneCount > m_LaneCullList.Capacity)
			{
				m_LaneCullList.Capacity = laneCount + math.max(laneCount / 4, 250);
				m_ClipMapList.Capacity = m_LaneCullList.Capacity;
			}
		}
		else
		{
			laneCount = m_LaneCullList.Length;
		}
		if (heightMapRenderRequired)
		{
			NativeQueue<BuildingUtils.LotInfo> queue = default(NativeQueue<BuildingUtils.LotInfo>);
			queue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			CullBuildingLotsJob obj = new CullBuildingLotsJob
			{
				m_LotHandle = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.Lot>(ref __TypeHandle.__Game_Buildings_Lot_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformHandle = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ElevationHandle = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_StackHandle = InternalCompilerInterface.GetComponentTypeHandle<Stack>(ref __TypeHandle.__Game_Objects_Stack_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_InstalledUpgradeHandle = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabBuildingExtensionData = InternalCompilerInterface.GetComponentLookup<BuildingExtensionData>(ref __TypeHandle.__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabAssetStampData = InternalCompilerInterface.GetComponentLookup<AssetStampData>(ref __TypeHandle.__Game_Prefabs_AssetStampData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OverrideTerraform = InternalCompilerInterface.GetComponentLookup<BuildingTerraformData>(ref __TypeHandle.__Game_Prefabs_BuildingTerraformData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AdditionalLots = InternalCompilerInterface.GetBufferLookup<AdditionalBuildingTerraformElement>(ref __TypeHandle.__Game_Prefabs_AdditionalBuildingTerraformElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Area = area,
				Result = queue.AsParallelWriter()
			};
			DequeBuildingLotsJob dequeBuildingLotsJob = new DequeBuildingLotsJob
			{
				m_Queue = queue,
				m_List = m_BuildingCullList
			};
			JobHandle val = JobChunkExtensions.ScheduleParallel<CullBuildingLotsJob>(obj, m_BuildingGroup, ((SystemBase)this).Dependency);
			m_BuildingCull = IJobExtensions.Schedule<DequeBuildingLotsJob>(dequeBuildingLotsJob, val);
			m_CullFinished = m_BuildingCull;
			queue.Dispose(m_BuildingCull);
		}
		if (roadsChanged)
		{
			CullRoadsJob cullRoadsJob = new CullRoadsJob
			{
				m_EntityHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeGeometryData = InternalCompilerInterface.GetComponentLookup<NodeGeometry>(ref __TypeHandle.__Game_Net_NodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OrphanData = InternalCompilerInterface.GetComponentLookup<Orphan>(ref __TypeHandle.__Game_Net_Orphan_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TerrainCompositionData = InternalCompilerInterface.GetComponentLookup<TerrainComposition>(ref __TypeHandle.__Game_Prefabs_TerrainComposition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Area = m_CascadeRanges[baseLod],
				Result = m_LaneCullList.AsParallelWriter()
			};
			m_LaneCull = JobChunkExtensions.ScheduleParallel<CullRoadsJob>(cullRoadsJob, m_RoadsGroup, ((SystemBase)this).Dependency);
			m_CullFinished = JobHandle.CombineDependencies(m_CullFinished, m_LaneCull);
			GenerateClipDataJob generateClipDataJob = new GenerateClipDataJob
			{
				m_RoadsToCull = m_LaneCullList,
				Result = m_ClipMapList.AsParallelWriter()
			};
			m_CurrentClipMap = null;
			m_ClipMapCull = IJobParallelForDeferExtensions.Schedule<GenerateClipDataJob, LaneSection>(generateClipDataJob, m_LaneCullList, 128, m_LaneCull);
			m_CullFinished = JobHandle.CombineDependencies(m_CullFinished, m_ClipMapCull);
		}
		if (terrainAreasChanged)
		{
			NativeQueue<AreaTriangle> queue2 = default(NativeQueue<AreaTriangle>);
			queue2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeQueue<AreaEdge> queue3 = default(NativeQueue<AreaEdge>);
			queue3._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			CullAreasJob cullAreasJob = new CullAreasJob
			{
				m_ClipType = InternalCompilerInterface.GetComponentTypeHandle<Clip>(ref __TypeHandle.__Game_Areas_Clip_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_AreaType = InternalCompilerInterface.GetComponentTypeHandle<Area>(ref __TypeHandle.__Game_Areas_Area_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_GeometryType = InternalCompilerInterface.GetComponentTypeHandle<Geometry>(ref __TypeHandle.__Game_Areas_Geometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_StorageType = InternalCompilerInterface.GetComponentTypeHandle<Storage>(ref __TypeHandle.__Game_Areas_Storage_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NodeType = InternalCompilerInterface.GetBufferTypeHandle<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TriangleType = InternalCompilerInterface.GetBufferTypeHandle<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabTerrainAreaData = InternalCompilerInterface.GetComponentLookup<TerrainAreaData>(ref __TypeHandle.__Game_Prefabs_TerrainAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabStorageAreaData = InternalCompilerInterface.GetComponentLookup<StorageAreaData>(ref __TypeHandle.__Game_Prefabs_StorageAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Area = m_CascadeRanges[baseLod],
				m_Triangles = queue2.AsParallelWriter(),
				m_Edges = queue3.AsParallelWriter()
			};
			DequeTrianglesJob dequeTrianglesJob = new DequeTrianglesJob
			{
				m_Queue = queue2,
				m_List = m_TriangleCullList
			};
			DequeEdgesJob obj2 = new DequeEdgesJob
			{
				m_Queue = queue3,
				m_List = m_EdgeCullList
			};
			JobHandle val2 = JobChunkExtensions.ScheduleParallel<CullAreasJob>(cullAreasJob, m_AreasQuery, ((SystemBase)this).Dependency);
			JobHandle val3 = IJobExtensions.Schedule<DequeTrianglesJob>(dequeTrianglesJob, val2);
			JobHandle val4 = IJobExtensions.Schedule<DequeEdgesJob>(obj2, val2);
			m_AreaCull = JobHandle.CombineDependencies(val3, val4);
			m_CullFinished = JobHandle.CombineDependencies(m_CullFinished, m_AreaCull);
			queue2.Dispose(m_AreaCull);
			queue3.Dispose(m_AreaCull);
		}
		if (clipAreasChanged)
		{
			if (!m_HasAreaClipMeshData)
			{
				m_HasAreaClipMeshData = true;
				m_AreaClipMeshData = Mesh.AllocateWritableMeshData(1);
			}
			JobHandle val5 = default(JobHandle);
			GenerateAreaClipMeshJob generateAreaClipMeshJob = new GenerateAreaClipMeshJob
			{
				m_Chunks = ((EntityQuery)(ref m_AreasQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val5),
				m_ClipType = InternalCompilerInterface.GetComponentTypeHandle<Clip>(ref __TypeHandle.__Game_Areas_Clip_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_AreaType = InternalCompilerInterface.GetComponentTypeHandle<Area>(ref __TypeHandle.__Game_Areas_Area_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NodeType = InternalCompilerInterface.GetBufferTypeHandle<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TriangleType = InternalCompilerInterface.GetBufferTypeHandle<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MeshData = m_AreaClipMeshData
			};
			m_AreaClipMeshDataDeps = IJobExtensions.Schedule<GenerateAreaClipMeshJob>(generateAreaClipMeshJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val5));
			generateAreaClipMeshJob.m_Chunks.Dispose(m_AreaClipMeshDataDeps);
			m_CullFinished = JobHandle.CombineDependencies(m_CullFinished, m_AreaClipMeshDataDeps);
		}
		((SystemBase)this).Dependency = m_CullFinished;
	}

	public void CullClipMapForView(Viewer viewer)
	{
	}

	private void CullCascade(int cascadeIndex, float4 area, float4 viewport, int laneCount)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		if (viewport.z == 0f || viewport.w == 0f)
		{
			Debug.LogError((object)"Invalid Viewport");
		}
		CascadeCullInfo cascadeCullInfo = m_CascadeCulling[cascadeIndex];
		((JobHandle)(ref cascadeCullInfo.m_BuildingHandle)).Complete();
		cascadeCullInfo.m_BuildingRenderList = new NativeList<BuildingLotDraw>(AllocatorHandle.op_Implicit((Allocator)3));
		float2 xy = ((float4)(ref area)).xy;
		float2 val = ((float4)(ref area)).zw - ((float4)(ref area)).xy;
		((float4)(ref area))._002Ector(xy.x + val.x * viewport.x, xy.y + val.y * viewport.y, xy.x + val.x * (viewport.x + viewport.z), xy.y + val.y * (viewport.y + viewport.w));
		area += new float4(-10f, -10f, 10f, 10f);
		heightMapCullArea[cascadeIndex] = area;
		NativeQueue<BuildingLotDraw> queue = default(NativeQueue<BuildingLotDraw>);
		queue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		CullBuildingsCascadeJob obj = new CullBuildingsCascadeJob
		{
			m_LotsToCull = m_BuildingCullList,
			m_Area = area,
			Result = queue.AsParallelWriter()
		};
		DequeBuildingDrawsJob dequeBuildingDrawsJob = new DequeBuildingDrawsJob
		{
			m_Queue = queue,
			m_List = cascadeCullInfo.m_BuildingRenderList
		};
		JobHandle val2 = IJobParallelForDeferExtensions.Schedule<CullBuildingsCascadeJob, BuildingUtils.LotInfo>(obj, m_BuildingCullList, 128, m_BuildingCull);
		cascadeCullInfo.m_BuildingHandle = IJobExtensions.Schedule<DequeBuildingDrawsJob>(dequeBuildingDrawsJob, val2);
		queue.Dispose(cascadeCullInfo.m_BuildingHandle);
		((JobHandle)(ref cascadeCullInfo.m_LaneHandle)).Complete();
		cascadeCullInfo.m_LaneRenderList = new NativeList<LaneDraw>(laneCount, AllocatorHandle.op_Implicit((Allocator)3));
		CullRoadsCacscadeJob cullRoadsCacscadeJob = new CullRoadsCacscadeJob
		{
			m_RoadsToCull = m_LaneCullList,
			m_Area = area,
			m_Scale = 1f / heightScaleOffset.x,
			Result = cascadeCullInfo.m_LaneRenderList.AsParallelWriter()
		};
		cascadeCullInfo.m_LaneHandle = IJobParallelForDeferExtensions.Schedule<CullRoadsCacscadeJob, LaneSection>(cullRoadsCacscadeJob, m_LaneCullList, 128, m_LaneCull);
		((JobHandle)(ref cascadeCullInfo.m_AreaHandle)).Complete();
		cascadeCullInfo.m_TriangleRenderList = new NativeList<AreaTriangle>(AllocatorHandle.op_Implicit((Allocator)3));
		cascadeCullInfo.m_EdgeRenderList = new NativeList<AreaEdge>(AllocatorHandle.op_Implicit((Allocator)3));
		CullTrianglesJob cullTrianglesJob = new CullTrianglesJob
		{
			m_Triangles = m_TriangleCullList,
			m_Area = area,
			Result = cascadeCullInfo.m_TriangleRenderList
		};
		CullEdgesJob obj2 = new CullEdgesJob
		{
			m_Edges = m_EdgeCullList,
			m_Area = area,
			Result = cascadeCullInfo.m_EdgeRenderList
		};
		JobHandle val3 = IJobExtensions.Schedule<CullTrianglesJob>(cullTrianglesJob, m_AreaCull);
		JobHandle val4 = IJobExtensions.Schedule<CullEdgesJob>(obj2, m_AreaCull);
		cascadeCullInfo.m_AreaHandle = JobHandle.CombineDependencies(val3, val4);
		m_CullFinished = JobHandle.CombineDependencies(m_CullFinished, JobHandle.CombineDependencies(cascadeCullInfo.m_BuildingHandle, cascadeCullInfo.m_LaneHandle, cascadeCullInfo.m_AreaHandle));
	}

	private void DrawHeightAdjustments(ref CommandBuffer cmdBuffer, int cascade, float4 area, float4 viewport, RenderTargetBinding binding, ref NativeArray<BuildingLotDraw> lots, ref NativeArray<LaneDraw> lanes, ref NativeArray<AreaTriangle> triangles, ref NativeArray<AreaEdge> edges, ref Material lotMaterial, ref Material laneMaterial, ref Material areaMaterial)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		float4 val = default(float4);
		((float4)(ref val))._002Ector(-((float4)(ref area)).xy, 1f / (((float4)(ref area)).zw - ((float4)(ref area)).xy));
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(viewport.x * (float)((Texture)m_HeightmapCascade).width, viewport.y * (float)((Texture)m_HeightmapCascade).height, viewport.z * (float)((Texture)m_HeightmapCascade).width, viewport.w * (float)((Texture)m_HeightmapCascade).height);
		if (lots.Length > 0)
		{
			ComputeBuffer val3 = m_BuildingInstanceData.Request(lots.Length);
			val3.SetData<BuildingLotDraw>(lots);
			val3.name = $"BuildingLot Buffer Cascade{cascade}";
			lotMaterial.SetVector(ShaderID._TerrainScaleOffsetID, new Vector4(heightScaleOffset.x, heightScaleOffset.y, 0f, 0f));
			lotMaterial.SetVector(ShaderID._MapOffsetScaleID, m_MapOffsetScale);
			lotMaterial.SetVector(ShaderID._CascadeOffsetScale, float4.op_Implicit(val));
			lotMaterial.SetTexture(ShaderID._HeightmapID, heightmap);
			lotMaterial.SetBuffer(ShaderID._BuildingLotID, val3);
		}
		if (lanes.Length > 0)
		{
			ComputeBuffer val4 = m_LaneInstanceData.Request(lanes.Length);
			val4.SetData<LaneDraw>(lanes);
			val4.name = $"Lane Buffer Cascade{cascade}";
			laneMaterial.SetVector(ShaderID._TerrainScaleOffsetID, new Vector4(heightScaleOffset.x, heightScaleOffset.y, 0f, 0f));
			laneMaterial.SetVector(ShaderID._MapOffsetScaleID, m_MapOffsetScale);
			laneMaterial.SetVector(ShaderID._CascadeOffsetScale, float4.op_Implicit(val));
			laneMaterial.SetTexture(ShaderID._HeightmapID, heightmap);
			laneMaterial.SetBuffer(ShaderID._LanesID, val4);
		}
		if (triangles.Length > 0 || edges.Length > 0)
		{
			ComputeBuffer val5 = m_TriangleInstanceData.Request(triangles.Length);
			val5.SetData<AreaTriangle>(triangles);
			val5.name = $"Triangle Buffer Cascade{cascade}";
			ComputeBuffer val6 = m_EdgeInstanceData.Request(edges.Length);
			val6.SetData<AreaEdge>(edges);
			val6.name = $"Edge Buffer Cascade{cascade}";
			areaMaterial.SetVector(ShaderID._TerrainScaleOffsetID, new Vector4(heightScaleOffset.x, heightScaleOffset.y, 0f, 0f));
			areaMaterial.SetVector(ShaderID._MapOffsetScaleID, m_MapOffsetScale);
			areaMaterial.SetVector(ShaderID._CascadeOffsetScale, float4.op_Implicit(val));
			areaMaterial.SetTexture(ShaderID._HeightmapID, heightmap);
			areaMaterial.SetBuffer(ShaderID._TrianglesID, val5);
			areaMaterial.SetBuffer(ShaderID._EdgesID, val6);
		}
		if (lots.Length > 0)
		{
			cmdBuffer.DrawProcedural(Matrix4x4.identity, lotMaterial, 1, (MeshTopology)0, 6, lots.Length);
		}
		if (lanes.Length > 0)
		{
			cmdBuffer.DrawMeshInstancedProcedural(m_LaneMesh, 0, laneMaterial, 1, lanes.Length, (MaterialPropertyBlock)null);
		}
		int num = Shader.PropertyToID("_CascadeMinHeights");
		cmdBuffer.GetTemporaryRT(num, ((Texture)m_HeightmapCascade).width, ((Texture)m_HeightmapCascade).height, 0, (FilterMode)0, m_HeightmapCascade.graphicsFormat);
		int num2 = math.max(0, Mathf.FloorToInt(((Rect)(ref val2)).xMin));
		int num3 = math.max(0, Mathf.FloorToInt(((Rect)(ref val2)).yMin));
		int num4 = math.min(((Texture)m_HeightmapCascade).width, Mathf.CeilToInt(((Rect)(ref val2)).xMax)) - num2;
		int num5 = math.min(((Texture)m_HeightmapCascade).height, Mathf.CeilToInt(((Rect)(ref val2)).yMax)) - num3;
		cmdBuffer.CopyTexture(RenderTargetIdentifier.op_Implicit((Texture)(object)m_HeightmapCascade), cascade, 0, num2, num3, num4, num5, RenderTargetIdentifier.op_Implicit(num), 0, 0, num2, num3);
		cmdBuffer.SetRenderTarget(binding, 0, (CubemapFace)(-1), cascade);
		cmdBuffer.EnableScissorRect(val2);
		if (triangles.Length > 0)
		{
			cmdBuffer.DrawProcedural(Matrix4x4.identity, areaMaterial, 0, (MeshTopology)0, 3, triangles.Length);
		}
		if (edges.Length > 0)
		{
			cmdBuffer.DrawProcedural(Matrix4x4.identity, areaMaterial, 1, (MeshTopology)0, 6, edges.Length);
		}
		if (lots.Length > 0)
		{
			cmdBuffer.DrawProcedural(Matrix4x4.identity, lotMaterial, 0, (MeshTopology)0, 6, lots.Length);
		}
		if (lanes.Length > 0)
		{
			cmdBuffer.DrawMeshInstancedProcedural(m_LaneMesh, 0, laneMaterial, 0, lanes.Length, (MaterialPropertyBlock)null);
		}
		cmdBuffer.ReleaseTemporaryRT(num);
		if (lots.Length > 0)
		{
			cmdBuffer.DrawProcedural(Matrix4x4.identity, lotMaterial, 2, (MeshTopology)0, 6, lots.Length);
		}
		if (lanes.Length > 0)
		{
			cmdBuffer.DrawMeshInstancedProcedural(m_LaneMesh, 0, laneMaterial, 2, lanes.Length, (MaterialPropertyBlock)null);
		}
	}

	private void RenderWorldMapToCascade(float4 area, float4 viewport, ref CommandBuffer cmdBuffer)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_WorldMapEditable != (Object)null)
		{
			bool flag = viewport.x == 0f && viewport.y == 0f && viewport.z == 1f && viewport.w == 1f;
			Texture val = (Texture)(object)m_WorldMapEditable;
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector(viewport.x * (float)((Texture)m_HeightmapCascade).width, viewport.y * (float)((Texture)m_HeightmapCascade).height, viewport.z * (float)((Texture)m_HeightmapCascade).width, viewport.w * (float)((Texture)m_HeightmapCascade).height);
			RenderTargetBinding val3 = default(RenderTargetBinding);
			((RenderTargetBinding)(ref val3))._002Ector(RenderTargetIdentifier.op_Implicit((Texture)(object)m_HeightmapCascade), (RenderBufferLoadAction)(flag ? 2 : 0), (RenderBufferStoreAction)0, RenderTargetIdentifier.op_Implicit((Texture)(object)m_HeightmapDepth), (RenderBufferLoadAction)2, (RenderBufferStoreAction)3);
			cmdBuffer.SetRenderTarget(val3, 0, (CubemapFace)(-1), 0);
			cmdBuffer.ClearRenderTarget(true, false, Color.black, 1f);
			cmdBuffer.EnableScissorRect(val2);
			Vector2 val4 = default(Vector2);
			((Vector2)(ref val4))._002Ector(1f, 1f);
			Vector2 val5 = new Vector2
			{
				x = (area.x - worldOffset.x) / worldSize.x,
				y = (area.y - worldOffset.y) / worldSize.y
			};
			cmdBuffer.Blit(val, RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)1), val4, val5);
		}
	}

	private void RenderCascade(int cascadeIndex, float4 area, float4 viewport, ref CommandBuffer cmdBuffer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		bool flag = true;
		bool flag2 = viewport.x == 0f && viewport.y == 0f && viewport.z == 1f && viewport.w == 1f;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(viewport.x * (float)((Texture)m_HeightmapCascade).width, viewport.y * (float)((Texture)m_HeightmapCascade).height, viewport.z * (float)((Texture)m_HeightmapCascade).width, viewport.w * (float)((Texture)m_HeightmapCascade).height);
		RenderTargetBinding val2 = default(RenderTargetBinding);
		((RenderTargetBinding)(ref val2))._002Ector(RenderTargetIdentifier.op_Implicit((Texture)(object)m_HeightmapCascade), (RenderBufferLoadAction)((flag2 && flag) ? 2 : 0), (RenderBufferStoreAction)0, RenderTargetIdentifier.op_Implicit((Texture)(object)m_HeightmapDepth), (RenderBufferLoadAction)2, (RenderBufferStoreAction)3);
		cmdBuffer.SetRenderTarget(val2, 0, (CubemapFace)(-1), cascadeIndex);
		cmdBuffer.ClearRenderTarget(true, false, Color.black, 1f);
		cmdBuffer.EnableScissorRect(val);
		if (flag)
		{
			float num = ((cascadeIndex < baseLod) ? math.pow(2f, (float)(-(cascadeIndex - baseLod))) : (1f / math.pow(2f, (float)(cascadeIndex - baseLod))));
			float2 val3 = float2.op_Implicit(new Vector2(num, num));
			float2 val4 = (((float4)(ref area)).xy - playableOffset) / playableArea;
			if (cascadeIndex == baseLod || baseLod == 0)
			{
				cmdBuffer.Blit(heightmap, RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)1), float2.op_Implicit(val3), float2.op_Implicit(val4));
			}
			else
			{
				cmdBuffer.SetGlobalVector("_CascadeHeightmapOffsetScale", float4.op_Implicit(new float4(val4, val3)));
				val3 = (((float4)(ref area)).zw - ((float4)(ref area)).xy) / worldSize;
				val4 = (((float4)(ref area)).xy - worldOffset) / worldSize;
				cmdBuffer.SetGlobalVector("_CascadeWorldOffsetScale", float4.op_Implicit(new float4(val4, val3)));
				cmdBuffer.Blit(heightmap, RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)1), m_TerrainBlit);
			}
		}
		Matrix4x4 val5 = Matrix4x4.Ortho(area.x, area.z, area.w, area.y, heightScaleOffset.x + heightScaleOffset.y, heightScaleOffset.y);
		val5.m02 *= -1f;
		val5.m12 *= -1f;
		val5.m22 *= -1f;
		val5.m32 *= -1f;
		cmdBuffer.SetViewProjectionMatrices(GL.GetGPUProjectionMatrix(val5, true), Matrix4x4.identity);
		CascadeCullInfo cascadeCullInfo = m_CascadeCulling[cascadeIndex];
		((JobHandle)(ref cascadeCullInfo.m_BuildingHandle)).Complete();
		((JobHandle)(ref cascadeCullInfo.m_LaneHandle)).Complete();
		((JobHandle)(ref cascadeCullInfo.m_AreaHandle)).Complete();
		if (cascadeCullInfo.m_BuildingRenderList.IsCreated || cascadeCullInfo.m_LaneRenderList.IsCreated || cascadeCullInfo.m_TriangleRenderList.IsCreated || cascadeCullInfo.m_EdgeRenderList.IsCreated)
		{
			NativeArray<BuildingLotDraw> lots = default(NativeArray<BuildingLotDraw>);
			NativeArray<LaneDraw> lanes = default(NativeArray<LaneDraw>);
			NativeArray<AreaTriangle> triangles = default(NativeArray<AreaTriangle>);
			NativeArray<AreaEdge> edges = default(NativeArray<AreaEdge>);
			if (cascadeCullInfo.m_BuildingRenderList.IsCreated)
			{
				lots = cascadeCullInfo.m_BuildingRenderList.AsArray();
			}
			if (cascadeCullInfo.m_LaneRenderList.IsCreated)
			{
				lanes = cascadeCullInfo.m_LaneRenderList.AsArray();
			}
			if (cascadeCullInfo.m_TriangleRenderList.IsCreated)
			{
				triangles = cascadeCullInfo.m_TriangleRenderList.AsArray();
			}
			if (cascadeCullInfo.m_EdgeRenderList.IsCreated)
			{
				edges = cascadeCullInfo.m_EdgeRenderList.AsArray();
			}
			DrawHeightAdjustments(ref cmdBuffer, cascadeIndex, area, viewport, val2, ref lots, ref lanes, ref triangles, ref edges, ref cascadeCullInfo.m_LotMaterial, ref cascadeCullInfo.m_LaneMaterial, ref cascadeCullInfo.m_AreaMaterial);
			if (cascadeCullInfo.m_BuildingRenderList.IsCreated)
			{
				cascadeCullInfo.m_BuildingRenderList.Dispose();
			}
			if (cascadeCullInfo.m_LaneRenderList.IsCreated)
			{
				cascadeCullInfo.m_LaneRenderList.Dispose();
			}
			if (cascadeCullInfo.m_TriangleRenderList.IsCreated)
			{
				cascadeCullInfo.m_TriangleRenderList.Dispose();
			}
			if (cascadeCullInfo.m_EdgeRenderList.IsCreated)
			{
				cascadeCullInfo.m_EdgeRenderList.Dispose();
			}
		}
		cmdBuffer.DisableScissorRect();
	}

	private void CreateRoadMeshes()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Expected O, but got Unknown
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		m_LaneMesh = new Mesh
		{
			name = "Lane Mesh"
		};
		int num = 1;
		int num2 = 8;
		int num3 = (num + 1) * (num2 + 1);
		int num4 = num * num2 * 2 * 3;
		Vector3[] array = (Vector3[])(object)new Vector3[num3];
		Vector2[] array2 = (Vector2[])(object)new Vector2[num3];
		int[] array3 = new int[num4];
		for (int i = 0; i <= num2; i++)
		{
			for (int j = 0; j <= num; j++)
			{
				array[j + (num + 1) * i] = new Vector3((float)j / (float)num, 0f, (float)i / (float)num2);
				array2[j + (num + 1) * i] = new Vector2(array[j + (num + 1) * i].x, array[j + (num + 1) * i].z);
			}
		}
		int num5 = num + 1;
		int num6 = 0;
		for (int k = 0; k < num2; k++)
		{
			for (int l = 0; l < num; l++)
			{
				array3[num6++] = l + num5 * (k + 1);
				array3[num6++] = l + 1 + num5 * (k + 1);
				array3[num6++] = l + 1 + num5 * k;
				array3[num6++] = l + num5 * (k + 1);
				array3[num6++] = l + 1 + num5 * k;
				array3[num6++] = l + num5 * k;
			}
		}
		m_LaneMesh.vertices = array;
		m_LaneMesh.uv = array2;
		m_LaneMesh.subMeshCount = 1;
		m_LaneMesh.SetTriangles(array3, 0);
		m_LaneMesh.UploadMeshData(true);
		m_ClipMesh = new Mesh
		{
			name = "Clip Mesh"
		};
		int num7 = num3;
		num3 *= 2;
		num4 = num4 * 2 + num2 * 2 * 3 * 2 + num * 2 * 3 * 2;
		array = (Vector3[])(object)new Vector3[num3];
		array2 = (Vector2[])(object)new Vector2[num3];
		array3 = new int[num4];
		for (int m = 0; m <= num2; m++)
		{
			for (int n = 0; n <= num; n++)
			{
				array[n + (num + 1) * m] = new Vector3((float)n / (float)num, 1f, (float)m / (float)num2);
				array2[n + (num + 1) * m] = new Vector2(array[n + (num + 1) * m].x, array[n + (num + 1) * m].z);
				array[num7 + n + (num + 1) * m] = array[n + (num + 1) * m];
				array[num7 + n + (num + 1) * m].y = 0f;
				array2[num7 + n + (num + 1) * m] = array2[n + (num + 1) * m];
			}
		}
		num5 = num + 1;
		num6 = 0;
		for (int num8 = 0; num8 < num2; num8++)
		{
			for (int num9 = 0; num9 < num; num9++)
			{
				array3[num6++] = num9 + num5 * (num8 + 1);
				array3[num6++] = num9 + 1 + num5 * (num8 + 1);
				array3[num6++] = num9 + 1 + num5 * num8;
				array3[num6++] = num9 + num5 * (num8 + 1);
				array3[num6++] = num9 + 1 + num5 * num8;
				array3[num6++] = num9 + num5 * num8;
			}
		}
		for (int num10 = 0; num10 < num2; num10++)
		{
			for (int num11 = 0; num11 < num; num11++)
			{
				array3[num6++] = num7 + (num11 + 1 + num5 * (num10 + 1));
				array3[num6++] = num7 + (num11 + num5 * (num10 + 1));
				array3[num6++] = num7 + (num11 + 1 + num5 * num10);
				array3[num6++] = num7 + (num11 + 1 + num5 * num10);
				array3[num6++] = num7 + (num11 + num5 * (num10 + 1));
				array3[num6++] = num7 + (num11 + num5 * num10);
			}
		}
		int num12 = 0;
		for (int num13 = 0; num13 < num2; num13++)
		{
			array3[num6++] = num12 + num5 * (num13 + 1);
			array3[num6++] = num12 + num5 * num13;
			array3[num6++] = num7 + num12 + num5 * num13;
			array3[num6++] = num7 + num12 + num5 * num13;
			array3[num6++] = num7 + num12 + num5 * (num13 + 1);
			array3[num6++] = num12 + num5 * (num13 + 1);
		}
		num12 = num;
		for (int num14 = 0; num14 < num2; num14++)
		{
			array3[num6++] = num12 + num5 * num14;
			array3[num6++] = num12 + num5 * (num14 + 1);
			array3[num6++] = num7 + num12 + num5 * num14;
			array3[num6++] = num7 + num12 + num5 * (num14 + 1);
			array3[num6++] = num7 + num12 + num5 * num14;
			array3[num6++] = num12 + num5 * (num14 + 1);
		}
		for (int num15 = 0; num15 < num; num15++)
		{
			array3[num6++] = num15;
			array3[num6++] = num15 + num7;
			array3[num6++] = num15 + num7 + 1;
			array3[num6++] = num15 + num7 + 1;
			array3[num6++] = num15 + 1;
			array3[num6++] = num15;
		}
		for (int num16 = 1; num16 <= num; num16++)
		{
			array3[num6++] = num3 - num16;
			array3[num6++] = num3 - num16 - 1;
			array3[num6++] = num3 - num16 - num7 - 1;
			array3[num6++] = num3 - num16 - num7 - 1;
			array3[num6++] = num3 - num16 - num7;
			array3[num6++] = num3 - num16;
		}
		m_ClipMesh.vertices = array;
		m_ClipMesh.uv = array2;
		m_ClipMesh.subMeshCount = 1;
		m_ClipMesh.SetTriangles(array3, 0);
		m_ClipMesh.UploadMeshData(true);
	}

	public bool CalculateBuildingCullArea(Transform transform, Entity prefab, ComponentLookup<ObjectGeometryData> geometryData, out float4 area)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		area = float4.zero;
		ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
		if (geometryData.TryGetComponent(prefab, ref objectGeometryData))
		{
			Bounds3 val = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, objectGeometryData);
			val = MathUtils.Expand(val, float3.op_Implicit(ObjectUtils.GetTerrainSmoothingWidth(objectGeometryData) - 8f));
			((float4)(ref area)).xy = ((float3)(ref val.min)).xz;
			((float4)(ref area)).zw = ((float3)(ref val.max)).xz;
			return true;
		}
		return false;
	}

	public void OnBuildingMoved(Entity entity)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		ComponentLookup<Transform> componentLookup = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<PrefabRef> componentLookup2 = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ObjectGeometryData> componentLookup3 = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		((SystemBase)this).CompleteDependency();
		float4 area = float4.zero;
		if (!componentLookup2.HasComponent(entity) || !componentLookup.HasComponent(entity))
		{
			return;
		}
		PrefabRef prefabRef = componentLookup2[entity];
		Transform transform = componentLookup[entity];
		if (CalculateBuildingCullArea(transform, prefabRef.m_Prefab, componentLookup3, out area))
		{
			NativeList<Bounds2> updateBuffer = m_GroundHeightSystem.GetUpdateBuffer();
			Bounds2 val = new Bounds2(((float4)(ref area)).xy, ((float4)(ref area)).zw);
			updateBuffer.Add(ref val);
			if (math.lengthsq(m_UpdateArea) > 0f)
			{
				((float4)(ref m_UpdateArea)).xy = math.min(((float4)(ref m_UpdateArea)).xy, ((float4)(ref area)).xy);
				((float4)(ref m_UpdateArea)).zw = math.max(((float4)(ref m_UpdateArea)).zw, ((float4)(ref area)).zw);
			}
			else
			{
				m_UpdateArea = area;
			}
			m_UpdateArea += new float4(-10f, -10f, 10f, 10f);
		}
	}

	public void GetLastMinMaxUpdate(out float3 min, out float3 max)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		int4 updateArea = m_TerrainMinMax.UpdateArea;
		float2 minMax = m_TerrainMinMax.GetMinMax(updateArea);
		float4 val = default(float4);
		((float4)(ref val))._002Ector((float)updateArea.x / (float)m_TerrainMinMax.size, (float)updateArea.y / (float)m_TerrainMinMax.size, (float)(updateArea.x + updateArea.z) / (float)m_TerrainMinMax.size, (float)(updateArea.y + updateArea.w) / (float)m_TerrainMinMax.size);
		float4 val2 = val;
		float2 val3 = worldSize;
		val = val2 * ((float2)(ref val3)).xyxy;
		float4 val4 = val;
		val3 = worldOffset;
		val = val4 + ((float2)(ref val3)).xyxy;
		min = new float3(val.x, minMax.x, val.y);
		max = new float3(val.z, minMax.y, val.w);
	}

	public ParallelWriter<Entity, Entity> GetBuildingUpgradeWriter(int ExpectedAmount)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_BuildingUpgradeDependencies)).Complete();
		if (ExpectedAmount > m_BuildingUpgrade.Capacity)
		{
			m_BuildingUpgrade.Capacity = ExpectedAmount;
		}
		return m_BuildingUpgrade.AsParallelWriter();
	}

	public void SetBuildingUpgradeWriterDependency(JobHandle handle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_BuildingUpgradeDependencies = handle;
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
	public TerrainSystem()
	{
	}
}
