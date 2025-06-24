using System;
using System.Runtime.CompilerServices;
using Colossal.AssetPipeline.Native;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.Mathematics;
using Colossal.Rendering;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Events;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.IO.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Simulation;

[FormerlySerializedAs("Colossal.Terrain.WaterSystem, Game")]
[CompilerGenerated]
public class WaterSystem : GameSystemBase, IDefaultSerializable, ISerializable, IGPUSystem
{
	[Serializable]
	public struct WaterSource
	{
		public int constantDepth;

		public float amount;

		public float2 position;

		public float radius;

		public float pollution;

		public float floodheight;
	}

	public struct QuadWaterBuffer
	{
		public RenderTexture[] waterTextures;

		public RenderTexture[] downdScaledFlowTextures;

		public RenderTexture[] blurredFlowTextures;

		private RenderTexture CreateRenderTexture(string name, int2 size, GraphicsFormat format)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			RenderTexture val = new RenderTexture(size.x, size.y, 0, format)
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

		public void Init(int2 size)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			waterTextures = (RenderTexture[])(object)new RenderTexture[2];
			waterTextures[0] = CreateRenderTexture("WaterRT0", size, (GraphicsFormat)52);
			waterTextures[1] = CreateRenderTexture("WaterRT1", size, (GraphicsFormat)52);
			downdScaledFlowTextures = (RenderTexture[])(object)new RenderTexture[3];
			for (int i = 0; i < 3; i++)
			{
				size /= 2;
				downdScaledFlowTextures[i] = CreateRenderTexture($"FlowTextureDownScaled{i}", size, (GraphicsFormat)48);
			}
		}

		public void Dispose()
		{
			if (waterTextures != null)
			{
				RenderTexture[] array = waterTextures;
				for (int i = 0; i < array.Length; i++)
				{
					CoreUtils.Destroy((Object)(object)array[i]);
				}
			}
			if (downdScaledFlowTextures != null)
			{
				RenderTexture[] array = downdScaledFlowTextures;
				for (int i = 0; i < array.Length; i++)
				{
					CoreUtils.Destroy((Object)(object)array[i]);
				}
			}
			if (blurredFlowTextures != null)
			{
				RenderTexture[] array = blurredFlowTextures;
				for (int i = 0; i < array.Length; i++)
				{
					CoreUtils.Destroy((Object)(object)array[i]);
				}
			}
		}

		public RenderTexture FlowDownScaled(int index)
		{
			return downdScaledFlowTextures[index];
		}
	}

	private struct WaterSourceCache
	{
		public float2 m_Position;

		public float m_Amount;

		public float m_Polluted;

		public float m_Radius;

		public int m_ConstantDepth;

		public float m_Multiplier;
	}

	[BurstCompile]
	private struct SourceJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_SourceChunks;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_EventChunks;

		[ReadOnly]
		public ComponentTypeHandle<WaterLevelChange> m_ChangeType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<WaterSourceData> m_SourceType;

		[ReadOnly]
		public ComponentLookup<WaterLevelChangeData> m_ChangePrefabDatas;

		public NativeList<WaterSourceCache> m_Cache;

		public float3 m_TerrainOffset;

		private void HandleSource(WaterSourceData source, Transform transform)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			float3 val = transform.m_Position - m_TerrainOffset;
			WaterSourceCache waterSourceCache = new WaterSourceCache
			{
				m_Amount = source.m_Amount,
				m_ConstantDepth = source.m_ConstantDepth,
				m_Multiplier = source.m_Multiplier,
				m_Polluted = source.m_Polluted,
				m_Radius = source.m_Radius,
				m_Position = ((float3)(ref val)).xz
			};
			if (source.m_ConstantDepth == 2 || source.m_ConstantDepth == 3)
			{
				float num = source.m_Amount;
				WaterLevelTargetType waterLevelTargetType = ((source.m_ConstantDepth == 2) ? WaterLevelTargetType.River : WaterLevelTargetType.Sea);
				for (int i = 0; i < m_EventChunks.Length; i++)
				{
					ArchetypeChunk val2 = m_EventChunks[i];
					NativeArray<WaterLevelChange> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<WaterLevelChange>(ref m_ChangeType);
					val2 = m_EventChunks[i];
					NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabRef>(ref m_PrefabType);
					for (int j = 0; j < nativeArray.Length; j++)
					{
						WaterLevelChange waterLevelChange = nativeArray[j];
						Entity prefab = nativeArray2[j].m_Prefab;
						if (m_ChangePrefabDatas.HasComponent(prefab))
						{
							WaterLevelChangeData waterLevelChangeData = m_ChangePrefabDatas[prefab];
							if (SourceMatchesDirection(source, transform, waterLevelChange.m_Direction) && (waterLevelChangeData.m_TargetType & waterLevelTargetType) != WaterLevelTargetType.None)
							{
								num += source.m_Multiplier * waterLevelChange.m_Intensity;
							}
						}
					}
				}
				waterSourceCache.m_Amount = num;
			}
			m_Cache.Add(ref waterSourceCache);
		}

		public void Execute()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			m_Cache.Clear();
			for (int i = 0; i < m_SourceChunks.Length; i++)
			{
				ArchetypeChunk val = m_SourceChunks[i];
				NativeArray<WaterSourceData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<WaterSourceData>(ref m_SourceType);
				val = m_SourceChunks[i];
				NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Transform>(ref m_TransformType);
				if (nativeArray2.Length > 0)
				{
					for (int j = 0; j < nativeArray.Length; j++)
					{
						HandleSource(nativeArray[j], nativeArray2[j]);
					}
				}
			}
		}
	}

	private struct ReadCommandHelper
	{
		private long m_Position;

		public long currentPosition => m_Position;

		public ReadCommandHelper(int position = 0)
		{
			m_Position = position;
		}

		public unsafe ReadCommand CreateReadCmd(long size, void* buffer = null)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			ReadCommand val = default(ReadCommand);
			val.Offset = m_Position;
			val.Size = size;
			if (buffer == null)
			{
				val.Buffer = UnsafeUtility.Malloc(val.Size, 16, (Allocator)2);
			}
			else
			{
				val.Buffer = buffer;
			}
			m_Position += size;
			return val;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<WaterLevelChange> __Game_Events_WaterLevelChange_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaterSourceData> __Game_Simulation_WaterSourceData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<WaterLevelChangeData> __Game_Prefabs_WaterLevelChangeData_RO_ComponentLookup;

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
			__Game_Events_WaterLevelChange_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterLevelChange>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Simulation_WaterSourceData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterSourceData>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Prefabs_WaterLevelChangeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterLevelChangeData>(true);
		}
	}

	public static readonly int kMapSize = 14336;

	public static readonly float kDefaultMinWaterToRestoreHeight = -5f;

	private static float s_SeaLevel;

	private bool m_Loaded;

	public const int MAX_FLOW_DOWNSCALE = 3;

	private int m_numFlowDownsample = 3;

	public float MaxFlowlengthForRender = 0.4f;

	public float PostFlowspeedMultiplier = 2f;

	private SimulationSystem m_SimulationSystem;

	private TerrainSystem m_TerrainSystem;

	private SoilWaterSystem m_SoilWaterSystem;

	private SnowSystem m_SnowSystem;

	private EntityQuery m_SourceGroup;

	private EntityQuery m_SoilWaterParameterGroup;

	private EntityQuery m_WaterLevelChangeGroup;

	private NativeList<WaterSourceCache> m_SourceCache1;

	private NativeList<WaterSourceCache> m_SourceCache2;

	private JobHandle m_SourceHandle;

	private int m_SourceCacheIndex;

	private bool m_FlipSourceCache;

	private int m_lastFrameGridSize;

	private const float kGravity = 9.81f;

	private const int kGridSize = 32;

	private static readonly float kCellSize = 7f;

	public float m_TimeStep = 0.03f;

	public float m_Damping = 0.995f;

	public float m_Evaporation = 0.0001f;

	public float m_RainConstant = 5E-05f;

	public float m_PollutionDecayRate = 0.001f;

	public float m_Fluidness = 0.1f;

	public float m_FlowSpeed = 0.003f;

	public float m_ConstantDepthDepth = 200f;

	private float m_lastFrameTimeStep;

	private QuadWaterBuffer m_Water;

	private ComputeBuffer m_Active;

	private ComputeBuffer m_CurrentActiveTilesIndices;

	private int m_numThreadGroupsTotal;

	private int m_numThreadGroupsX;

	private int m_numThreadGroupsY;

	private NativeArray<int> m_ActiveCPU;

	private NativeArray<int> m_ActiveCPUTemp;

	private SurfaceDataReader m_depthsReader;

	private SurfaceDataReader m_velocitiesReader;

	private JobHandle m_ActiveReaders;

	private uint m_LastReadyFrame;

	private uint m_PreviousReadyFrame;

	private int m_SubFrame;

	private int2 m_TexSize;

	private bool m_NewMap;

	private int m_terrainChangeCounter;

	private float m_restoreHeightMinWaterHeight = kDefaultMinWaterToRestoreHeight;

	private ComputeShader m_UpdateShader;

	private int m_VelocityKernel;

	private int m_DownsampleKernel;

	private int m_VerticalBlurKernel;

	private int m_HorizontalBlurKernel;

	private int m_FlowPostProcessKernel;

	private int m_DepthKernel;

	private int m_CopyToHeightmapKernel;

	private int m_RestoreHeightFromHeightmapKernel;

	private int m_AddKernel;

	private int m_AddConstantKernel;

	private int m_EvaporateKernel;

	private int m_ResetKernel;

	private int m_ResetActiveKernel;

	private int m_ResetToLevelKernel;

	private int m_LoadKernel;

	private int m_LoadFlowMapKernel;

	private int m_AddBorderKernel;

	private int m_ID_AddPosition;

	private int m_ID_AddRadius;

	private int m_ID_AddAmount;

	private int m_ID_AddPolluted;

	private int m_ID_AreaX;

	private int m_ID_AreaY;

	private int m_ID_CellsPerArea;

	private int m_ID_AreaCountX;

	private int m_ID_AreaCountY;

	private int m_ID_Evaporation;

	private int m_ID_RainConstant;

	private int m_ID_TerrainScale;

	private int m_ID_Timestep;

	private int m_ID_Fluidness;

	private int m_ID_Damping;

	private int m_ID_FlowInterpolationFatcor;

	private int m_ID_CellSize;

	private int m_ID_SoilWaterDepthConstant;

	private int m_ID_SoilOutputMultiplier;

	private int m_ID_AddBorderPosition;

	private int m_ID_PollutionDecayRate;

	private int m_ID_Previous;

	private int m_ID_Result;

	private int m_ID_Terrain;

	private int m_ID_TerrainLod;

	private int m_ID_MaxVelocity;

	private int m_ID_RestoreHeightMinWaterHeight;

	private int m_ID_Active;

	private ulong m_NextSimulationFrame;

	private ulong m_LastReadbackRequest;

	private ulong m_LastDepthReadbackRequest;

	private CommandBuffer m_CommandBuffer;

	private WaterRenderSystem m_WaterRenderSystem;

	private AsyncGPUReadbackHelper m_AsyncGPUReadback;

	private AsyncGPUReadbackHelper m_SaveAsyncGPUReadback;

	private bool m_PendingActiveReadback;

	private static ProfilerMarker m_DepthUpdate = new ProfilerMarker("UpdateDepthMap");

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1000286415_0;

	public static float SeaLevel => s_SeaLevel;

	public int WaterSimSpeed { get; set; }

	public float TimeStepOverride { get; set; }

	public bool Loaded => m_Loaded;

	public bool UseActiveCellsCulling { get; set; } = true;

	public int2 TextureSize => m_TexSize;

	public RenderTexture WaterTexture => m_Water.waterTextures[0];

	public RenderTexture WaterRenderTexture => m_Water.waterTextures[1];

	public bool BlurFlowMap { get; set; } = true;

	public bool FlowPostProcess { get; set; } = true;

	public int FlowMapNumDownscale
	{
		get
		{
			return m_numFlowDownsample;
		}
		set
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			m_numFlowDownsample = value;
			Shader.SetGlobalTexture("colossal_FlowTexture", FlowTextureUpdated);
			Shader.SetGlobalVector("colossal_FlowTexture_TexelSize", new Vector4((float)FlowTextureUpdated.width, (float)FlowTextureUpdated.height, 1f / (float)FlowTextureUpdated.width));
		}
	}

	public bool EnableFlowDownscale
	{
		get
		{
			return m_numFlowDownsample > 1;
		}
		set
		{
			if (value)
			{
				FlowMapNumDownscale = 3;
			}
			else
			{
				FlowMapNumDownscale = 0;
			}
		}
	}

	public Texture FlowTextureUpdated
	{
		get
		{
			if (FlowMapNumDownscale > 0)
			{
				return (Texture)(object)m_Water.FlowDownScaled(FlowMapNumDownscale - 1);
			}
			return (Texture)(object)WaterRenderTexture;
		}
	}

	public float CellSize => kCellSize;

	public float2 MapSize => kCellSize * new float2((float)m_TexSize.x, (float)m_TexSize.y);

	public int GridSizeMultiplier { get; set; } = 3;

	public int GridSize => 32 * (1 << GridSizeMultiplier);

	public float MaxVelocity { get; set; } = 7f;

	public int MaxSpeed { get; set; }

	public int SimulationCycleSteps => 3;

	private int ReadbackRequestInterval => 8;

	private int DepthReadbackRequestInterval => 30;

	public static float WaveSpeed => kCellSize / 30f;

	public int2 m_ActiveGridSize { get; private set; }

	public bool IsAsync { get; set; }

	private NativeList<WaterSourceCache> LastFrameSourceCache
	{
		get
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			if (m_SourceCacheIndex != 0)
			{
				return m_SourceCache2;
			}
			return m_SourceCache1;
		}
	}

	private NativeList<WaterSourceCache> CurrentJobSourceCache
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			if (m_SourceCacheIndex != 1)
			{
				return m_SourceCache2;
			}
			return m_SourceCache1;
		}
	}

	public SurfaceWater GetDepth(float3 position, NativeArray<SurfaceWater> waterMap)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		SurfaceWater result = default(SurfaceWater);
		float2 val = (float)kMapSize / float2.op_Implicit(m_TexSize);
		int2 cell = GetCell(position - new float3(val.x / 2f, 0f, val.y / 2f), kMapSize, m_TexSize);
		float2 val2 = GetCellCoords(position, kMapSize, m_TexSize) - new float2(0.5f, 0.5f);
		_ = val2 - float2.op_Implicit(cell);
		cell.x = math.max(0, cell.x);
		cell.x = math.min(m_TexSize.x - 2, cell.x);
		cell.y = math.max(0, cell.y);
		cell.y = math.min(m_TexSize.y - 2, cell.y);
		SurfaceWater surfaceWater = waterMap[cell.x + 1 + m_TexSize.x * cell.y];
		SurfaceWater surfaceWater2 = waterMap[cell.x + m_TexSize.x * cell.y];
		SurfaceWater surfaceWater3 = waterMap[cell.x + m_TexSize.x * (cell.y + 1)];
		SurfaceWater surfaceWater4 = waterMap[cell.x + 1 + m_TexSize.x * (cell.y + 1)];
		result.m_Depth = math.lerp(math.lerp(surfaceWater.m_Depth, surfaceWater2.m_Depth, val2.x - (float)cell.x), math.lerp(surfaceWater3.m_Depth, surfaceWater4.m_Depth, val2.x - (float)cell.x), val2.y - (float)cell.y);
		result.m_Depth = math.max(result.m_Depth, 0f);
		result.m_Polluted = math.lerp(math.lerp(surfaceWater.m_Polluted, surfaceWater2.m_Polluted, val2.x - (float)cell.x), math.lerp(surfaceWater3.m_Polluted, surfaceWater4.m_Polluted, val2.x - (float)cell.x), val2.y - (float)cell.y);
		return result;
	}

	public NativeArray<SurfaceWater> GetDepths(out JobHandle deps)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		deps = m_depthsReader.JobWriters;
		return m_depthsReader.WaterSurfaceCPUArray;
	}

	public WaterSurfaceData GetSurfaceData(out JobHandle deps)
	{
		return m_depthsReader.GetSurfaceData(out deps);
	}

	public WaterSurfaceData GetVelocitiesSurfaceData(out JobHandle deps)
	{
		return m_velocitiesReader.GetSurfaceData(out deps);
	}

	public void AddSurfaceReader(JobHandle handle)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		m_depthsReader.JobReaders = JobHandle.CombineDependencies(m_depthsReader.JobReaders, handle);
	}

	public void AddVelocitySurfaceReader(JobHandle handle)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		m_velocitiesReader.JobReaders = JobHandle.CombineDependencies(m_velocitiesReader.JobReaders, handle);
	}

	public void AddActiveReader(JobHandle handle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_ActiveReaders = JobHandle.CombineDependencies(m_ActiveReaders, handle);
	}

	public static float CalculateSourceMultiplier(WaterSourceData source, float3 pos)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		if (source.m_Radius < 0.01f)
		{
			return 0f;
		}
		pos.y = 0f;
		int num = Mathf.CeilToInt(source.m_Radius / kCellSize);
		float num2 = 0f;
		float num3 = source.m_Radius * source.m_Radius;
		int num4 = Mathf.FloorToInt(pos.x / kCellSize) - num;
		int num5 = Mathf.FloorToInt(pos.z / kCellSize) - num;
		float3 val = default(float3);
		for (int i = num4; i <= num4 + 2 * num + 1; i++)
		{
			for (int j = num5; j <= num5 + 2 * num + 1; j++)
			{
				((float3)(ref val))._002Ector((float)i * kCellSize, 0f, (float)j * kCellSize);
				num2 += 1f - math.smoothstep(0f, 1f, math.distancesq(val, pos) / num3);
			}
		}
		if (num2 < 0.001f)
		{
			Debug.LogWarning((object)$"Warning: water source at {pos} has too small radius to work");
			return 1f;
		}
		return 1f / num2;
	}

	public NativeArray<int> GetActive()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_ActiveCPU;
	}

	private static float2 GetCellCoords(float3 position, int mapSize, int2 textureSize)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		float2 val = (float)mapSize / float2.op_Implicit(textureSize);
		return new float2(((float)(mapSize / 2) + position.x) / val.x, ((float)(mapSize / 2) + position.z) / val.y);
	}

	public static int2 GetCell(float3 position, int mapSize, int2 textureSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		float2 cellCoords = GetCellCoords(position, mapSize, textureSize);
		return new int2(Mathf.FloorToInt(cellCoords.x), Mathf.FloorToInt(cellCoords.y));
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		InitShader();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_SoilWaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SoilWaterSystem>();
		m_SnowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SnowSystem>();
		m_WaterRenderSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterRenderSystem>();
		((ComponentSystemBase)this).RequireForUpdate<TerrainPropertiesData>();
		m_SourceGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<WaterSourceData>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_SoilWaterParameterGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<SoilWaterParameterData>() });
		m_WaterLevelChangeGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<WaterLevelChange>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Deleted>()
		});
		WaterSimSpeed = 1;
		m_Loaded = false;
		m_CommandBuffer = new CommandBuffer();
		m_CommandBuffer.name = "Watersystem";
		m_SourceCache1 = new NativeList<WaterSourceCache>(AllocatorHandle.op_Implicit((Allocator)4));
		m_SourceCache2 = new NativeList<WaterSourceCache>(AllocatorHandle.op_Implicit((Allocator)4));
		InitTextures();
	}

	private bool HasWater(float3 position)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		float2 val = (float)kMapSize / float2.op_Implicit(m_TexSize);
		int2 cell = GetCell(position - new float3(val.x / 2f, 0f, val.y / 2f), kMapSize, m_TexSize);
		_ = GetCellCoords(position, kMapSize, m_TexSize) - new float2(0.5f, 0.5f) - float2.op_Implicit(cell);
		cell.x = math.max(0, cell.x);
		cell.x = math.min(m_TexSize.x - 2, cell.x);
		cell.y = math.max(0, cell.y);
		cell.y = math.min(m_TexSize.y - 2, cell.y);
		if (m_depthsReader.GetSurface(cell).m_Depth > 0f)
		{
			return true;
		}
		return false;
	}

	public void TerrainWillChangeFromBrush(Bounds2 area)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		if (m_terrainChangeCounter == 0 && HasWater((new float3[5]
		{
			new float3(((Bounds2)(ref area)).Center().x, 0f, ((Bounds2)(ref area)).Center().y),
			new float3(((Bounds2)(ref area)).x.min, 0f, ((Bounds2)(ref area)).y.min),
			new float3(((Bounds2)(ref area)).x.min, 0f, ((Bounds2)(ref area)).y.max),
			new float3(((Bounds2)(ref area)).x.max, 0f, ((Bounds2)(ref area)).y.min),
			new float3(((Bounds2)(ref area)).x.max, 0f, ((Bounds2)(ref area)).y.max)
		})[0]))
		{
			m_restoreHeightMinWaterHeight = -1000000f;
		}
		m_terrainChangeCounter = 15;
		WaterSimSpeed = 0;
	}

	public void TerrainWillChange()
	{
		m_terrainChangeCounter = 15;
		WaterSimSpeed = 0;
	}

	private void InitTextures()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		m_TexSize = new int2(2048, 2048);
		m_Water = default(QuadWaterBuffer);
		m_Water.Init(m_TexSize);
		int2 val = (m_ActiveGridSize = m_TexSize / GridSize);
		m_Active = new ComputeBuffer(val.x * val.y, UnsafeUtility.SizeOf<int>(), (ComputeBufferType)0);
		m_CurrentActiveTilesIndices = new ComputeBuffer(val.x * val.y, UnsafeUtility.SizeOf<int2>(), (ComputeBufferType)0);
		m_ActiveCPU = new NativeArray<int>(val.x * val.y, (Allocator)4, (NativeArrayOptions)1);
		if (m_depthsReader != null)
		{
			m_depthsReader.Dispose();
		}
		if (m_velocitiesReader != null)
		{
			m_velocitiesReader.Dispose();
		}
		m_depthsReader = new SurfaceDataReader(WaterTexture, kMapSize);
		m_velocitiesReader = new SurfaceDataReader(m_Water.FlowDownScaled(0), kMapSize);
		m_NewMap = true;
	}

	private void InitShader()
	{
		m_UpdateShader = AssetDatabase.global.resources.shaders.waterUpdate;
		m_VelocityKernel = m_UpdateShader.FindKernel("VelocityUpdate");
		m_DownsampleKernel = m_UpdateShader.FindKernel("CSDownsample");
		m_VerticalBlurKernel = m_UpdateShader.FindKernel("CSVerticalBlur");
		m_HorizontalBlurKernel = m_UpdateShader.FindKernel("CSHorizontalBlur");
		m_FlowPostProcessKernel = m_UpdateShader.FindKernel("CSFlowPostProcess");
		m_DepthKernel = m_UpdateShader.FindKernel("DepthUpdate");
		m_CopyToHeightmapKernel = m_UpdateShader.FindKernel("CopyToHeightmap");
		m_RestoreHeightFromHeightmapKernel = m_UpdateShader.FindKernel("RestoreHeightFromHeightmap");
		m_AddKernel = m_UpdateShader.FindKernel("Add");
		m_AddConstantKernel = m_UpdateShader.FindKernel("AddConstant");
		m_EvaporateKernel = m_UpdateShader.FindKernel("Evaporate");
		m_ResetKernel = m_UpdateShader.FindKernel("Reset");
		m_ResetActiveKernel = m_UpdateShader.FindKernel("ResetActive");
		m_ResetToLevelKernel = m_UpdateShader.FindKernel("ResetToLevel");
		m_LoadKernel = m_UpdateShader.FindKernel("Load");
		m_AddBorderKernel = m_UpdateShader.FindKernel("AddBorder");
		m_ID_AddAmount = Shader.PropertyToID("addAmount");
		m_ID_AddPolluted = Shader.PropertyToID("addPolluted");
		m_ID_AddPosition = Shader.PropertyToID("addPosition");
		m_ID_AddRadius = Shader.PropertyToID("addRadius");
		m_ID_AreaX = Shader.PropertyToID("areax");
		m_ID_AreaY = Shader.PropertyToID("areay");
		m_ID_CellsPerArea = Shader.PropertyToID("cellsPerArea");
		m_ID_AreaCountX = Shader.PropertyToID("areaCountX");
		m_ID_AreaCountY = Shader.PropertyToID("areaCountY");
		m_ID_Evaporation = Shader.PropertyToID("evaporation");
		m_ID_RainConstant = Shader.PropertyToID("rainConstant");
		m_ID_TerrainScale = Shader.PropertyToID("terrainScale");
		m_ID_Timestep = Shader.PropertyToID("timestep");
		m_ID_Fluidness = Shader.PropertyToID("fluidness");
		m_ID_Damping = Shader.PropertyToID("damping");
		m_ID_CellSize = Shader.PropertyToID("cellSize");
		m_ID_FlowInterpolationFatcor = Shader.PropertyToID("flowInterpolationFatcor");
		m_ID_PollutionDecayRate = Shader.PropertyToID("pollutionDecayRate");
		m_ID_AddBorderPosition = Shader.PropertyToID("addBorderPosition");
		m_ID_RestoreHeightMinWaterHeight = Shader.PropertyToID("restoreHeightMinWaterHeight");
		m_ID_Previous = Shader.PropertyToID("_Previous");
		m_ID_Result = Shader.PropertyToID("_Result");
		m_ID_Terrain = Shader.PropertyToID("_Terrain");
		m_ID_TerrainLod = Shader.PropertyToID("_TerrainLod");
		m_ID_Active = Shader.PropertyToID("_Active");
		m_ID_MaxVelocity = Shader.PropertyToID("maxVelo");
		m_ID_SoilWaterDepthConstant = Shader.PropertyToID("soilWaterDepthConstant");
		m_ID_SoilOutputMultiplier = Shader.PropertyToID("soilOutputMultiplier");
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		m_Loaded = false;
		Shader.SetGlobalVector("colossal_WaterParams", new Vector4(0f, 0f, 0f, 0f));
		s_SeaLevel = 0f;
		if (m_velocitiesReader != null)
		{
			m_velocitiesReader.Dispose();
		}
		if (m_depthsReader != null)
		{
			m_depthsReader.Dispose();
		}
		m_SourceCache2.Dispose();
		m_SourceCache1.Dispose();
		if (m_Active != null)
		{
			m_Active.Release();
		}
		if (m_CurrentActiveTilesIndices != null)
		{
			m_CurrentActiveTilesIndices.Release();
		}
		if (m_ActiveCPU.IsCreated)
		{
			((JobHandle)(ref m_ActiveReaders)).Complete();
			m_ActiveCPU.Dispose();
		}
		m_CommandBuffer.Release();
		m_Water.Dispose();
		base.OnDestroy();
	}

	public static bool SourceMatchesDirection(WaterSourceData source, Transform transform, float2 direction)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (source.m_ConstantDepth != 2 && source.m_ConstantDepth != 3)
		{
			return false;
		}
		if (math.abs(transform.m_Position.x) > math.abs(transform.m_Position.z))
		{
			return math.sign(transform.m_Position.x) != math.sign(direction.x);
		}
		return math.sign(transform.m_Position.z) != math.sign(direction.y);
	}

	public unsafe void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		uint num = m_PreviousReadyFrame;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		uint num2 = m_LastReadyFrame;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
		ulong num3 = m_NextSimulationFrame;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num3);
		int x = m_TexSize.x;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(x);
		int y = m_TexSize.y;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(y);
		NativeArray<float4> val = default(NativeArray<float4>);
		val._002Ector(((Texture)WaterTexture).width * ((Texture)WaterTexture).height, (Allocator)4, (NativeArrayOptions)1);
		AsyncGPUReadbackRequest val2 = AsyncGPUReadback.RequestIntoNativeArray<float4>(ref val, (Texture)(object)WaterTexture, 0, (Action<AsyncGPUReadbackRequest>)null);
		((AsyncGPUReadbackRequest)(ref val2)).WaitForCompletion();
		NativeArray<byte> val3 = default(NativeArray<byte>);
		val3._002Ector(val.Length * UnsafeUtility.SizeOf(typeof(float4)), (Allocator)2, (NativeArrayOptions)1);
		NativeCompression.FilterDataBeforeWrite((IntPtr)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr<float4>(val), (IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr<byte>(val3), (long)val3.Length, UnsafeUtility.SizeOf(typeof(float4)));
		val.Dispose();
		NativeArray<byte> val4 = val3;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val4);
		val3.Dispose();
	}

	public unsafe void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Expected O, but got Unknown
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.waterInterpolationFix)
		{
			ref uint reference = ref m_PreviousReadyFrame;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
			ref uint reference2 = ref m_LastReadyFrame;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
		}
		else
		{
			float num = default(float);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			float num2 = default(float);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
			m_PreviousReadyFrame = (uint)Mathf.RoundToInt(num);
			m_LastReadyFrame = (uint)Mathf.RoundToInt(num2);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.waterElectricityID)
		{
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version < Version.waterOverflowFix)
			{
				uint num3 = default(uint);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num3);
				m_NextSimulationFrame = num3;
			}
			else
			{
				ref ulong reference3 = ref m_NextSimulationFrame;
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference3);
			}
			int num4 = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num4);
			int num5 = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num5);
			bool flag = true;
			if (num4 != m_TexSize.x)
			{
				Debug.LogWarning((object)("Saved water width = " + num4 + ", water tex width = " + m_TexSize.x));
				flag = false;
			}
			if (num5 != m_TexSize.y)
			{
				Debug.LogWarning((object)("Saved water height = " + num5 + ", water tex height = " + m_TexSize.y));
				flag = false;
			}
			int num6 = 0;
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version < Version.waterGridNotNeeded)
			{
				int num7 = default(int);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num7);
				if (num7 > 0)
				{
					num6 = num4 * num5 / (num7 * num7);
					NativeArray<int> val = default(NativeArray<int>);
					val._002Ector(num6, (Allocator)2, (NativeArrayOptions)1);
					NativeArray<int> val2 = val;
					((IReader)reader/*cast due to .constrained prefix*/).Read(val2);
					val.Dispose();
				}
			}
			num6 = num4 * num5;
			NativeArray<float4> val3 = default(NativeArray<float4>);
			val3._002Ector(num6, (Allocator)2, (NativeArrayOptions)1);
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version >= Version.terrainWaterSnowCompression)
			{
				NativeArray<byte> val4 = default(NativeArray<byte>);
				val4._002Ector(num6 * System.Runtime.CompilerServices.Unsafe.SizeOf<float4>(), (Allocator)2, (NativeArrayOptions)1);
				NativeArray<byte> val5 = val4;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val5);
				NativeCompression.UnfilterDataAfterRead((IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr<byte>(val4), (IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr<float4>(val3), (long)val4.Length, System.Runtime.CompilerServices.Unsafe.SizeOf<float4>());
				val4.Dispose();
			}
			else
			{
				NativeArray<float4> val6 = val3;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val6);
			}
			if (flag)
			{
				m_depthsReader.LoadData(val3);
			}
			ComputeBuffer val7 = new ComputeBuffer(num6, UnsafeUtility.SizeOf<float4>(), (ComputeBufferType)0);
			val7.SetData<float4>(val3);
			m_CommandBuffer.SetComputeIntParam(m_UpdateShader, m_ID_CellsPerArea, GridSize);
			m_CommandBuffer.SetComputeIntParam(m_UpdateShader, m_ID_AreaCountX, m_TexSize.x / GridSize);
			m_CommandBuffer.SetComputeBufferParam(m_UpdateShader, m_LoadKernel, "_LoadSource", val7);
			m_CommandBuffer.SetComputeTextureParam(m_UpdateShader, m_LoadKernel, m_ID_Result, RenderTargetIdentifier.op_Implicit((Texture)(object)WaterTexture));
			m_CommandBuffer.DispatchCompute(m_UpdateShader, m_LoadKernel, m_TexSize.x / 16, m_TexSize.y / 16, 1);
			CurrentJobSourceCache.Clear();
			LastFrameSourceCache.Clear();
			ResetActive(m_CommandBuffer);
			Graphics.ExecuteCommandBuffer(m_CommandBuffer);
			((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).Request(m_Active, (Action<AsyncGPUReadbackRequest>)null);
			m_PendingActiveReadback = true;
			m_LastReadbackRequest = 0uL;
			m_LastDepthReadbackRequest = 0uL;
			m_NextSimulationFrame = 0uL;
			m_PreviousReadyFrame = 0u;
			m_CommandBuffer.Clear();
			val7.Dispose();
			val3.Dispose();
			BindTextures();
			m_depthsReader.ExecuteReadBack();
			m_velocitiesReader.ExecuteReadBack();
			m_NewMap = true;
		}
		Shader.SetGlobalVector("colossal_WaterParams", new Vector4(0f, 0f, 0f, 0f));
		s_SeaLevel = 0f;
		m_Loaded = true;
	}

	public void SetDefaults(Context context)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		m_LastReadbackRequest = 0uL;
		m_LastDepthReadbackRequest = 0uL;
		m_PreviousReadyFrame = 0u;
		m_LastReadyFrame = 0u;
		m_NextSimulationFrame = 0uL;
		Reset();
		BindTextures();
		m_NewMap = true;
		m_Loaded = true;
		Shader.SetGlobalVector("colossal_WaterParams", new Vector4(0f, 0f, 0f, 0f));
		s_SeaLevel = 0f;
	}

	private void BindTextures()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		Shader.SetGlobalTexture("colossal_WaterTexture", (Texture)(object)WaterTexture);
		Shader.SetGlobalVector("colossal_WaterTexture_TexelSize", new Vector4((float)((Texture)WaterRenderTexture).width, (float)((Texture)WaterRenderTexture).height, 1f / (float)((Texture)WaterRenderTexture).width, 1f / (float)((Texture)WaterRenderTexture).height));
		Shader.SetGlobalTexture("colossal_WaterRenderTexture", (Texture)(object)WaterRenderTexture);
		Shader.SetGlobalVector("colossal_WateRenderrTexture_TexelSize", new Vector4((float)((Texture)WaterRenderTexture).width, (float)((Texture)WaterRenderTexture).height, 1f / (float)((Texture)WaterRenderTexture).width, 1f / (float)((Texture)WaterRenderTexture).height));
		Shader.SetGlobalTexture("colossal_FlowTexture", FlowTextureUpdated);
		Shader.SetGlobalVector("colossal_FlowTexture_TexelSize", new Vector4((float)FlowTextureUpdated.width, (float)FlowTextureUpdated.height, 1f / (float)FlowTextureUpdated.width));
	}

	private void Reset()
	{
		m_UpdateShader.SetTexture(m_ResetKernel, m_ID_Result, (Texture)(object)WaterTexture);
		m_UpdateShader.Dispatch(m_ResetKernel, m_TexSize.x / 16, m_TexSize.y / 16, 1);
	}

	public void ResetToSealevel()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_SourceGroup)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		EntityManager entityManager = ((ComponentSystemBase)this).World.EntityManager;
		float num = float.MaxValue;
		bool flag = false;
		for (int i = 0; i < val.Length; i++)
		{
			Entity val2 = val[i];
			WaterSourceData componentData = ((EntityManager)(ref entityManager)).GetComponentData<WaterSourceData>(val2);
			if (componentData.m_ConstantDepth == 3)
			{
				num = math.min(num, componentData.m_Amount);
				flag = true;
			}
		}
		val.Dispose();
		if (flag)
		{
			ResetToLevel(num);
		}
	}

	private unsafe void ResetToLevel(float level)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		m_CommandBuffer.Clear();
		Debug.Log((object)level);
		ProfilingScope val = default(ProfilingScope);
		((ProfilingScope)(ref val))._002Ector(m_CommandBuffer, ProfilingSampler.Get<ProfileId>(ProfileId.WaterResetToLevel));
		try
		{
			int2 val2 = m_TexSize / GridSize;
			m_CommandBuffer.SetComputeFloatParam(m_UpdateShader, m_ID_AddAmount, level);
			m_CommandBuffer.SetComputeFloatParam(m_UpdateShader, m_ID_CellSize, kCellSize);
			m_CommandBuffer.SetComputeIntParam(m_UpdateShader, m_ID_CellsPerArea, GridSize);
			m_CommandBuffer.SetComputeTextureParam(m_UpdateShader, m_ResetToLevelKernel, m_ID_Terrain, RenderTargetIdentifier.op_Implicit(m_TerrainSystem.GetCascadeTexture()));
			CommandBuffer obj = m_CommandBuffer;
			ComputeShader obj2 = m_UpdateShader;
			int num = m_ID_TerrainScale;
			float x = m_TerrainSystem.heightScaleOffset.x;
			float3 positionOffset = m_TerrainSystem.positionOffset;
			obj.SetComputeVectorParam(obj2, num, float4.op_Implicit(new float4(x, ((float3)(ref positionOffset)).xy, 0f)));
			m_CommandBuffer.SetComputeIntParam(m_UpdateShader, m_ID_TerrainLod, TerrainSystem.baseLod);
			for (int i = 0; i < val2.x; i++)
			{
				m_CommandBuffer.SetComputeIntParam(m_UpdateShader, m_ID_AreaX, i);
				for (int j = 0; j < val2.y; j++)
				{
					m_CommandBuffer.SetComputeIntParam(m_UpdateShader, m_ID_AreaY, j);
					m_CommandBuffer.SetComputeTextureParam(m_UpdateShader, m_ResetToLevelKernel, m_ID_Result, RenderTargetIdentifier.op_Implicit((Texture)(object)WaterTexture));
					m_CommandBuffer.DispatchCompute(m_UpdateShader, m_ResetToLevelKernel, GridSize / 16, GridSize / 16, 1);
				}
			}
		}
		finally
		{
			((IDisposable)(*(ProfilingScope*)(&val))/*cast due to .constrained prefix*/).Dispose();
		}
		Graphics.ExecuteCommandBuffer(m_CommandBuffer);
	}

	private bool BorderCircleIntersection(bool isX, bool isPositive, float2 center, float radius, out int2 result)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)kMapSize / 2f;
		float num2 = radius * radius;
		float num3 = math.abs((isX ? center.x : center.y) - (isPositive ? num : (0f - num)));
		float num4 = num2 - num3 * num3;
		if (num4 < 0f)
		{
			result = default(int2);
			return false;
		}
		float num5 = (isX ? center.y : center.x);
		float num6 = math.sqrt(num4);
		float2 val = default(float2);
		((float2)(ref val))._002Ector(num5 - num6 + num, num5 + num6 + num);
		result = new int2(Mathf.FloorToInt((float)TextureSize.x * math.saturate(val.x / (float)kMapSize)), Mathf.CeilToInt((float)TextureSize.y * math.saturate(val.y / (float)kMapSize)));
		int num7 = (isX ? TextureSize.y : TextureSize.x) - 2;
		if (isX && isPositive)
		{
			num7++;
		}
		result.y = math.min(result.y, num7);
		result.x = math.min(result.x, result.y);
		return true;
	}

	private unsafe void SourceStep(CommandBuffer cmd)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		ProfilingScope val = default(ProfilingScope);
		((ProfilingScope)(ref val))._002Ector(cmd, ProfilingSampler.Get<ProfileId>(ProfileId.SourceStep));
		try
		{
			cmd.SetComputeFloatParam(m_UpdateShader, m_ID_CellSize, kCellSize);
			cmd.SetComputeVectorParam(m_UpdateShader, m_ID_TerrainScale, float4.op_Implicit(new float4(m_TerrainSystem.heightScaleOffset.x, m_TerrainSystem.positionOffset.y, 0f, 0f)));
			cmd.SetComputeIntParam(m_UpdateShader, m_ID_TerrainLod, TerrainSystem.baseLod);
			cmd.SetComputeFloatParam(m_UpdateShader, m_ID_Timestep, GetTimeStep());
			((JobHandle)(ref m_ActiveReaders)).Complete();
			float num2 = float.MaxValue;
			Enumerator<WaterSourceCache> enumerator = LastFrameSourceCache.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					WaterSourceCache current = enumerator.Current;
					float2 val2 = current.m_Position;
					float3 positionOffset = m_TerrainSystem.positionOffset;
					float2 center = val2 + ((float3)(ref positionOffset)).xz;
					cmd.SetComputeVectorParam(m_UpdateShader, m_ID_AddPosition, float4.op_Implicit(new float4(current.m_Position, 0f, 0f)));
					cmd.SetComputeFloatParam(m_UpdateShader, m_ID_AddRadius, current.m_Radius);
					int num3 = Mathf.CeilToInt(current.m_Radius / kCellSize);
					num3 = 2 * num3 + 1;
					if (current.m_ConstantDepth == 1)
					{
						num++;
						cmd.SetComputeTextureParam(m_UpdateShader, m_AddConstantKernel, m_ID_Result, RenderTargetIdentifier.op_Implicit((Texture)(object)WaterTexture));
						cmd.SetComputeTextureParam(m_UpdateShader, m_AddConstantKernel, m_ID_Terrain, RenderTargetIdentifier.op_Implicit(m_TerrainSystem.GetCascadeTexture()));
						cmd.SetComputeBufferParam(m_UpdateShader, m_AddConstantKernel, m_ID_Active, m_Active);
						cmd.SetComputeFloatParam(m_UpdateShader, m_ID_AddAmount, current.m_Amount);
						cmd.DispatchCompute(m_UpdateShader, m_AddConstantKernel, num3, num3, 1);
					}
					else if (current.m_ConstantDepth == 0)
					{
						num++;
						cmd.SetComputeFloatParam(m_UpdateShader, m_ID_AddAmount, (float)SimulationCycleSteps * current.m_Multiplier * current.m_Amount);
						cmd.SetComputeFloatParam(m_UpdateShader, m_ID_AddPolluted, current.m_Polluted);
						cmd.SetComputeTextureParam(m_UpdateShader, m_AddKernel, m_ID_Result, RenderTargetIdentifier.op_Implicit((Texture)(object)WaterTexture));
						cmd.SetComputeBufferParam(m_UpdateShader, m_AddKernel, m_ID_Active, m_Active);
						cmd.DispatchCompute(m_UpdateShader, m_AddKernel, num3, num3, 1);
					}
					else if (current.m_ConstantDepth == 2 || current.m_ConstantDepth == 3)
					{
						num2 = math.min(num2, current.m_Amount);
						cmd.SetComputeTextureParam(m_UpdateShader, m_AddBorderKernel, m_ID_Result, RenderTargetIdentifier.op_Implicit((Texture)(object)WaterTexture));
						cmd.SetComputeTextureParam(m_UpdateShader, m_AddBorderKernel, m_ID_Terrain, RenderTargetIdentifier.op_Implicit(m_TerrainSystem.GetCascadeTexture()));
						cmd.SetComputeBufferParam(m_UpdateShader, m_AddBorderKernel, m_ID_Active, m_Active);
						int4 val3 = default(int4);
						if (BorderCircleIntersection(isX: false, isPositive: false, center, current.m_Radius, out var result))
						{
							num++;
							val3.x = result.x;
							val3.y = 0;
							cmd.SetComputeVectorParam(m_UpdateShader, m_ID_AddBorderPosition, float4.op_Implicit(new float4(val3)));
							cmd.SetComputeFloatParam(m_UpdateShader, m_ID_AddAmount, current.m_Amount);
							cmd.DispatchCompute(m_UpdateShader, m_AddBorderKernel, result.y - result.x + 1, 1, 1);
						}
						if (BorderCircleIntersection(isX: false, isPositive: true, center, current.m_Radius, out result))
						{
							num++;
							val3.x = result.x;
							val3.y = TextureSize.y - 1;
							cmd.SetComputeVectorParam(m_UpdateShader, m_ID_AddBorderPosition, float4.op_Implicit(new float4(val3)));
							cmd.SetComputeFloatParam(m_UpdateShader, m_ID_AddAmount, current.m_Amount);
							cmd.DispatchCompute(m_UpdateShader, m_AddBorderKernel, result.y - result.x + 1, 1, 1);
						}
						if (BorderCircleIntersection(isX: true, isPositive: false, center, current.m_Radius, out result))
						{
							num++;
							val3.x = 0;
							val3.y = result.x;
							cmd.SetComputeVectorParam(m_UpdateShader, m_ID_AddBorderPosition, float4.op_Implicit(new float4(val3)));
							cmd.SetComputeFloatParam(m_UpdateShader, m_ID_AddAmount, current.m_Amount);
							cmd.DispatchCompute(m_UpdateShader, m_AddBorderKernel, 1, 1, result.y - result.x + 1);
						}
						if (BorderCircleIntersection(isX: true, isPositive: true, center, current.m_Radius, out result))
						{
							num++;
							val3.x = TextureSize.x - 1;
							val3.y = result.x;
							cmd.SetComputeVectorParam(m_UpdateShader, m_ID_AddBorderPosition, float4.op_Implicit(new float4(val3)));
							cmd.SetComputeFloatParam(m_UpdateShader, m_ID_AddAmount, current.m_Amount);
							cmd.DispatchCompute(m_UpdateShader, m_AddBorderKernel, 1, 1, result.y - result.x + 1);
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			if (num2 != float.MaxValue)
			{
				Shader.SetGlobalVector("colossal_WaterParams", new Vector4(num2, 0f, 0f, 0f));
				s_SeaLevel = num2;
			}
		}
		finally
		{
			((IDisposable)(*(ProfilingScope*)(&val))/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private void ResetActive(CommandBuffer cmd)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		int2 val = m_TexSize / GridSize;
		cmd.SetComputeFloatParam(m_UpdateShader, m_ID_CellSize, kCellSize);
		cmd.SetComputeBufferParam(m_UpdateShader, m_ResetActiveKernel, m_ID_Active, m_Active);
		cmd.SetComputeIntParam(m_UpdateShader, m_ID_AreaCountX, m_TexSize.x / GridSize);
		cmd.SetComputeIntParam(m_UpdateShader, m_ID_AreaCountY, m_TexSize.y / GridSize);
		cmd.DispatchCompute(m_UpdateShader, m_ResetActiveKernel, val.x, val.y, 1);
	}

	public float GetTimeStep()
	{
		if (m_NewMap)
		{
			return 1f;
		}
		if (m_SimulationSystem.selectedSpeed == 0f)
		{
			return 0f;
		}
		float num = Math.Min(Time.smoothDeltaTime * 30f, 1f);
		float num2 = m_SimulationSystem.selectedSpeed * 0.25f;
		if (TimeStepOverride > 0f)
		{
			return TimeStepOverride;
		}
		float num3 = math.min(1f, num2 * num);
		m_lastFrameTimeStep = math.lerp(m_lastFrameTimeStep, num3, Time.smoothDeltaTime * 0.2f);
		return m_lastFrameTimeStep;
	}

	private unsafe void EvaporateStep(CommandBuffer cmd)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		ProfilingScope val = default(ProfilingScope);
		((ProfilingScope)(ref val))._002Ector(cmd, ProfilingSampler.Get<ProfileId>(ProfileId.EvaporateStep));
		try
		{
			bool flag = false;
			int2 val2 = m_TexSize / GridSize;
			if (m_lastFrameGridSize != GridSize)
			{
				if (((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).isPending)
				{
					((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).WaitForCompletion();
					UpdateGPUReadback();
				}
				if (m_ActiveCPU.IsCreated)
				{
					((JobHandle)(ref m_ActiveReaders)).Complete();
					m_ActiveCPU.Dispose();
					m_Active.Dispose();
					m_CurrentActiveTilesIndices.Dispose();
				}
				m_ActiveGridSize = val2;
				m_Active = new ComputeBuffer(val2.x * val2.y, UnsafeUtility.SizeOf<int>(), (ComputeBufferType)0);
				m_CurrentActiveTilesIndices = new ComputeBuffer(val2.x * val2.y, UnsafeUtility.SizeOf<int2>(), (ComputeBufferType)0);
				m_ActiveCPU = new NativeArray<int>(val2.x * val2.y, (Allocator)4, (NativeArrayOptions)1);
				ResetActive(cmd);
				m_lastFrameGridSize = GridSize;
				flag = true;
			}
			SoilWaterParameterData soilWaterParameterData = default(SoilWaterParameterData);
			if (!((EntityQuery)(ref m_SoilWaterParameterGroup)).TryGetSingleton<SoilWaterParameterData>(ref soilWaterParameterData))
			{
				return;
			}
			cmd.SetComputeFloatParam(m_UpdateShader, m_ID_SoilWaterDepthConstant, soilWaterParameterData.m_MaximumWaterDepth);
			int num = 262144 / SoilWaterSystem.kUpdatesPerDay / SimulationCycleSteps;
			cmd.SetComputeFloatParam(m_UpdateShader, m_ID_SoilOutputMultiplier, soilWaterParameterData.m_WaterPerUnit / (float)num);
			cmd.SetComputeFloatParam(m_UpdateShader, m_ID_CellSize, kCellSize);
			cmd.SetComputeIntParam(m_UpdateShader, m_ID_AreaCountX, m_TexSize.x / GridSize);
			cmd.SetComputeIntParam(m_UpdateShader, m_ID_AreaCountY, m_TexSize.y / GridSize);
			cmd.SetComputeFloatParam(m_UpdateShader, m_ID_MaxVelocity, MaxVelocity);
			cmd.SetComputeBufferParam(m_UpdateShader, m_EvaporateKernel, m_ID_Active, m_Active);
			cmd.SetComputeTextureParam(m_UpdateShader, m_EvaporateKernel, "_Snow", RenderTargetIdentifier.op_Implicit((Texture)(object)m_SnowSystem.SnowDepth));
			cmd.SetComputeTextureParam(m_UpdateShader, m_EvaporateKernel, m_ID_Result, RenderTargetIdentifier.op_Implicit((Texture)(object)WaterTexture));
			cmd.SetComputeFloatParam(m_UpdateShader, m_ID_Timestep, GetTimeStep());
			cmd.SetComputeIntParam(m_UpdateShader, m_ID_CellsPerArea, GridSize);
			cmd.SetComputeFloatParam(m_UpdateShader, m_ID_Evaporation, m_Evaporation);
			cmd.SetComputeFloatParam(m_UpdateShader, m_ID_RainConstant, m_RainConstant);
			cmd.SetComputeFloatParam(m_UpdateShader, m_ID_PollutionDecayRate, m_PollutionDecayRate);
			bool flag2 = flag | m_NewMap;
			if (flag2)
			{
				cmd.SetComputeFloatParam(m_UpdateShader, m_ID_Timestep, m_TimeStep);
			}
			int numThreadGroupsX = 0;
			uint2[] array = (uint2[])(object)new uint2[val2.y * val2.x];
			uint2 val3 = default(uint2);
			for (uint num2 = 0u; num2 < val2.x; num2++)
			{
				for (uint num3 = 0u; num3 < val2.y; num3++)
				{
					if (!UseActiveCellsCulling || flag2 || m_ActiveCPU[(int)(num2 + val2.x * num3)] > 0)
					{
						((uint2)(ref val3))._002Ector(num2, num3);
						array[numThreadGroupsX++] = val3;
					}
				}
			}
			m_CurrentActiveTilesIndices.SetData((Array)array);
			cmd.SetComputeBufferParam(m_UpdateShader, m_EvaporateKernel, "_CurrentActiveIndices", m_CurrentActiveTilesIndices);
			m_numThreadGroupsX = numThreadGroupsX;
			m_numThreadGroupsY = GridSize / 8;
			m_numThreadGroupsTotal = m_numThreadGroupsX * m_numThreadGroupsY;
			if (m_numThreadGroupsTotal > 0)
			{
				cmd.DispatchCompute(m_UpdateShader, m_EvaporateKernel, m_numThreadGroupsX, m_numThreadGroupsY, m_numThreadGroupsY);
			}
		}
		finally
		{
			((IDisposable)(*(ProfilingScope*)(&val))/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private unsafe void VelocityStep(CommandBuffer cmd)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		ProfilingScope val = default(ProfilingScope);
		((ProfilingScope)(ref val))._002Ector(cmd, ProfilingSampler.Get<ProfileId>(ProfileId.VelocityStep));
		try
		{
			cmd.SetComputeTextureParam(m_UpdateShader, m_ResetKernel, m_ID_Result, RenderTargetIdentifier.op_Implicit((Texture)(object)WaterRenderTexture));
			cmd.DispatchCompute(m_UpdateShader, m_ResetKernel, m_TexSize.x / 16, m_TexSize.y / 16, 1);
			cmd.SetComputeTextureParam(m_UpdateShader, m_VelocityKernel, m_ID_Terrain, RenderTargetIdentifier.op_Implicit(m_TerrainSystem.GetCascadeTexture()));
			ComputeShader obj = m_UpdateShader;
			int num = m_ID_TerrainScale;
			float2 heightScaleOffset = m_TerrainSystem.heightScaleOffset;
			cmd.SetComputeVectorParam(obj, num, float4.op_Implicit(new float4(((float2)(ref heightScaleOffset)).xy, 0f, 0f)));
			cmd.SetComputeFloatParam(m_UpdateShader, m_ID_CellSize, kCellSize);
			cmd.SetComputeTextureParam(m_UpdateShader, m_VelocityKernel, m_ID_Previous, RenderTargetIdentifier.op_Implicit((Texture)(object)WaterTexture));
			cmd.SetComputeTextureParam(m_UpdateShader, m_VelocityKernel, m_ID_Result, RenderTargetIdentifier.op_Implicit((Texture)(object)WaterRenderTexture));
			cmd.SetComputeTextureParam(m_UpdateShader, m_VelocityKernel, "_DownscaledResult", RenderTargetIdentifier.op_Implicit((Texture)(object)m_Water.FlowDownScaled(0)));
			cmd.SetComputeIntParam(m_UpdateShader, m_ID_CellsPerArea, GridSize);
			cmd.SetComputeFloatParam(m_UpdateShader, m_ID_Fluidness, m_Fluidness);
			cmd.SetComputeFloatParam(m_UpdateShader, m_ID_Damping, m_Damping);
			cmd.SetComputeFloatParam(m_UpdateShader, m_ID_FlowInterpolationFatcor, m_NewMap ? 1f : 0.1f);
			int y = (m_TexSize / GridSize).y;
			_ = 0;
			((JobHandle)(ref m_ActiveReaders)).Complete();
			cmd.SetComputeBufferParam(m_UpdateShader, m_VelocityKernel, "_CurrentActiveIndices", m_CurrentActiveTilesIndices);
			if (m_numThreadGroupsTotal > 0)
			{
				cmd.DispatchCompute(m_UpdateShader, m_VelocityKernel, m_numThreadGroupsX, m_numThreadGroupsY, m_numThreadGroupsY);
			}
		}
		finally
		{
			((IDisposable)(*(ProfilingScope*)(&val))/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private void UpdateGPUReadback()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).isPending && !((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).hasError)
		{
			if (((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).done)
			{
				m_ActiveCPUTemp = ((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).GetData<int>(0);
				((JobHandle)(ref m_ActiveReaders)).Complete();
				m_ActiveCPU.CopyFrom(m_ActiveCPUTemp);
				m_PendingActiveReadback = false;
			}
			((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).IncrementFrame();
		}
	}

	private void UpdateSaveReadback()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((AsyncGPUReadbackHelper)(ref m_SaveAsyncGPUReadback)).isPending && !((AsyncGPUReadbackHelper)(ref m_SaveAsyncGPUReadback)).hasError)
		{
			if (((AsyncGPUReadbackHelper)(ref m_SaveAsyncGPUReadback)).done)
			{
				JobSaveToFile(((AsyncGPUReadbackHelper)(ref m_SaveAsyncGPUReadback)).GetData<float4>(0));
			}
			((AsyncGPUReadbackHelper)(ref m_SaveAsyncGPUReadback)).IncrementFrame();
		}
	}

	private unsafe void RestoreHeightFromHeightmap(CommandBuffer cmd)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		ProfilingScope val = default(ProfilingScope);
		((ProfilingScope)(ref val))._002Ector(cmd, ProfilingSampler.Get<ProfileId>(ProfileId.CopyToHeightMap));
		try
		{
			cmd.SetComputeFloatParam(m_UpdateShader, m_ID_RestoreHeightMinWaterHeight, m_restoreHeightMinWaterHeight);
			cmd.SetComputeTextureParam(m_UpdateShader, m_RestoreHeightFromHeightmapKernel, m_ID_Result, RenderTargetIdentifier.op_Implicit((Texture)(object)WaterTexture));
			cmd.SetComputeBufferParam(m_UpdateShader, m_RestoreHeightFromHeightmapKernel, m_ID_Active, m_Active);
			cmd.SetComputeTextureParam(m_UpdateShader, m_RestoreHeightFromHeightmapKernel, m_ID_Previous, RenderTargetIdentifier.op_Implicit((Texture)(object)WaterRenderTexture));
			cmd.SetComputeTextureParam(m_UpdateShader, m_RestoreHeightFromHeightmapKernel, m_ID_Terrain, RenderTargetIdentifier.op_Implicit(m_TerrainSystem.GetCascadeTexture()));
			cmd.SetComputeBufferParam(m_UpdateShader, m_RestoreHeightFromHeightmapKernel, "_CurrentActiveIndices", m_CurrentActiveTilesIndices);
			ComputeShader obj = m_UpdateShader;
			int num = m_ID_TerrainScale;
			float2 heightScaleOffset = m_TerrainSystem.heightScaleOffset;
			cmd.SetComputeVectorParam(obj, num, float4.op_Implicit(new float4(((float2)(ref heightScaleOffset)).xy, 0f, 0f)));
			cmd.SetComputeIntParam(m_UpdateShader, m_ID_TerrainLod, TerrainSystem.baseLod);
			if (m_numThreadGroupsTotal > 0)
			{
				cmd.DispatchCompute(m_UpdateShader, m_RestoreHeightFromHeightmapKernel, m_numThreadGroupsX, m_numThreadGroupsY, m_numThreadGroupsY);
			}
		}
		finally
		{
			((IDisposable)(*(ProfilingScope*)(&val))/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private unsafe void DepthStep(CommandBuffer cmd)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		ProfilingScope val = default(ProfilingScope);
		((ProfilingScope)(ref val))._002Ector(cmd, ProfilingSampler.Get<ProfileId>(ProfileId.DepthStep));
		try
		{
			cmd.SetComputeFloatParam(m_UpdateShader, m_ID_CellSize, kCellSize);
			cmd.SetComputeTextureParam(m_UpdateShader, m_DepthKernel, m_ID_Previous, RenderTargetIdentifier.op_Implicit((Texture)(object)WaterRenderTexture));
			cmd.SetComputeTextureParam(m_UpdateShader, m_DepthKernel, m_ID_Result, RenderTargetIdentifier.op_Implicit((Texture)(object)WaterTexture));
			cmd.SetComputeBufferParam(m_UpdateShader, m_DepthKernel, m_ID_Active, m_Active);
			cmd.SetComputeIntParam(m_UpdateShader, m_ID_CellsPerArea, GridSize);
			cmd.SetComputeIntParam(m_UpdateShader, m_ID_AreaCountX, m_TexSize.x / GridSize);
			cmd.SetComputeIntParam(m_UpdateShader, m_ID_AreaCountY, m_TexSize.y / GridSize);
			cmd.SetComputeTextureParam(m_UpdateShader, m_DepthKernel, m_ID_Terrain, RenderTargetIdentifier.op_Implicit(m_TerrainSystem.GetCascadeTexture()));
			cmd.SetComputeTextureParam(m_UpdateShader, m_DepthKernel, "_Snow", RenderTargetIdentifier.op_Implicit((Texture)(object)m_SnowSystem.SnowDepth));
			cmd.SetComputeVectorParam(m_UpdateShader, m_ID_TerrainScale, float4.op_Implicit(new float4(m_TerrainSystem.heightScaleOffset.x, m_TerrainSystem.heightScaleOffset.y, 0f, 0f)));
			cmd.SetComputeIntParam(m_UpdateShader, m_ID_TerrainLod, TerrainSystem.baseLod);
			int y = (m_TexSize / GridSize).y;
			_ = 0;
			((JobHandle)(ref m_ActiveReaders)).Complete();
			cmd.SetComputeBufferParam(m_UpdateShader, m_DepthKernel, "_CurrentActiveIndices", m_CurrentActiveTilesIndices);
			if (m_numThreadGroupsTotal > 0)
			{
				cmd.DispatchCompute(m_UpdateShader, m_DepthKernel, m_numThreadGroupsX, m_numThreadGroupsY, m_numThreadGroupsY);
			}
			if (FlowMapNumDownscale > 0)
			{
				int2 val2 = m_TexSize / 2 / 8;
				int num = FlowMapNumDownscale - 1;
				for (int i = 0; i < num; i++)
				{
					val2 /= 2;
					cmd.SetComputeTextureParam(m_UpdateShader, m_DownsampleKernel, m_ID_Previous, RenderTargetIdentifier.op_Implicit((Texture)(object)m_Water.FlowDownScaled(i)));
					cmd.SetComputeTextureParam(m_UpdateShader, m_DownsampleKernel, m_ID_Result, RenderTargetIdentifier.op_Implicit((Texture)(object)m_Water.FlowDownScaled(i + 1)));
					cmd.DispatchCompute(m_UpdateShader, m_DownsampleKernel, val2.x, val2.y, 1);
				}
				if (BlurFlowMap && FlowMapNumDownscale > 1)
				{
					cmd.SetComputeTextureParam(m_UpdateShader, m_VerticalBlurKernel, m_ID_Previous, RenderTargetIdentifier.op_Implicit((Texture)(object)m_Water.FlowDownScaled(FlowMapNumDownscale - 1)));
					cmd.SetComputeTextureParam(m_UpdateShader, m_VerticalBlurKernel, m_ID_Result, RenderTargetIdentifier.op_Implicit((Texture)(object)m_Water.FlowDownScaled(FlowMapNumDownscale - 2)));
					cmd.DispatchCompute(m_UpdateShader, m_VerticalBlurKernel, val2.x, val2.y, 1);
					cmd.SetComputeTextureParam(m_UpdateShader, m_HorizontalBlurKernel, m_ID_Previous, RenderTargetIdentifier.op_Implicit((Texture)(object)m_Water.FlowDownScaled(FlowMapNumDownscale - 2)));
					cmd.SetComputeTextureParam(m_UpdateShader, m_HorizontalBlurKernel, m_ID_Result, RenderTargetIdentifier.op_Implicit((Texture)(object)m_Water.FlowDownScaled(FlowMapNumDownscale - 1)));
					cmd.DispatchCompute(m_UpdateShader, m_HorizontalBlurKernel, val2.x, val2.y, 1);
				}
				if (FlowPostProcess)
				{
					cmd.SetComputeTextureParam(m_UpdateShader, m_FlowPostProcessKernel, m_ID_Result, RenderTargetIdentifier.op_Implicit((Texture)(object)m_Water.FlowDownScaled(FlowMapNumDownscale - 1)));
					cmd.SetComputeFloatParam(m_UpdateShader, "maxFlowlengthForRender", MaxFlowlengthForRender);
					cmd.SetComputeFloatParam(m_UpdateShader, "postFlowspeedMultiplier", PostFlowspeedMultiplier);
					cmd.DispatchCompute(m_UpdateShader, m_FlowPostProcessKernel, val2.x, val2.y, 1);
				}
			}
		}
		finally
		{
			((IDisposable)(*(ProfilingScope*)(&val))/*cast due to .constrained prefix*/).Dispose();
		}
		m_PreviousReadyFrame = m_LastReadyFrame;
		m_LastReadyFrame = (uint)(m_NextSimulationFrame / (ulong)MaxSpeed);
		if (m_NextSimulationFrame >= (ulong)((long)m_LastReadbackRequest + (long)ReadbackRequestInterval) && !m_PendingActiveReadback)
		{
			m_LastReadbackRequest = m_NextSimulationFrame;
			((AsyncGPUReadbackHelper)(ref m_AsyncGPUReadback)).Request(m_Active, (Action<AsyncGPUReadbackRequest>)null);
			m_PendingActiveReadback = true;
		}
		if (m_NextSimulationFrame >= (ulong)((long)m_LastDepthReadbackRequest + (long)DepthReadbackRequestInterval))
		{
			m_LastDepthReadbackRequest = m_NextSimulationFrame;
			m_depthsReader.ExecuteReadBack();
			m_velocitiesReader.ExecuteReadBack();
		}
	}

	private unsafe void CopyToHeightmapStep(CommandBuffer cmd)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		ProfilingScope val = default(ProfilingScope);
		((ProfilingScope)(ref val))._002Ector(cmd, ProfilingSampler.Get<ProfileId>(ProfileId.CopyToHeightMap));
		try
		{
			cmd.SetComputeTextureParam(m_UpdateShader, m_CopyToHeightmapKernel, m_ID_Previous, RenderTargetIdentifier.op_Implicit((Texture)(object)WaterTexture));
			cmd.SetComputeBufferParam(m_UpdateShader, m_CopyToHeightmapKernel, m_ID_Active, m_Active);
			cmd.SetComputeTextureParam(m_UpdateShader, m_CopyToHeightmapKernel, "_WaterOut", RenderTargetIdentifier.op_Implicit((Texture)(object)WaterRenderTexture));
			cmd.SetComputeTextureParam(m_UpdateShader, m_CopyToHeightmapKernel, m_ID_Terrain, RenderTargetIdentifier.op_Implicit(m_TerrainSystem.GetCascadeTexture()));
			cmd.SetComputeBufferParam(m_UpdateShader, m_CopyToHeightmapKernel, "_CurrentActiveIndices", m_CurrentActiveTilesIndices);
			ComputeShader obj = m_UpdateShader;
			int num = m_ID_TerrainScale;
			float2 heightScaleOffset = m_TerrainSystem.heightScaleOffset;
			cmd.SetComputeVectorParam(obj, num, float4.op_Implicit(new float4(((float2)(ref heightScaleOffset)).xy, 0f, 0f)));
			cmd.SetComputeIntParam(m_UpdateShader, m_ID_TerrainLod, TerrainSystem.baseLod);
			if (m_numThreadGroupsTotal > 0)
			{
				cmd.DispatchCompute(m_UpdateShader, m_CopyToHeightmapKernel, m_numThreadGroupsX, m_numThreadGroupsY, m_numThreadGroupsY);
			}
		}
		finally
		{
			((IDisposable)(*(ProfilingScope*)(&val))/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private unsafe void Simulate(CommandBuffer cmd)
	{
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref __query_1000286415_0)).HasSingleton<TerrainPropertiesData>())
		{
			return;
		}
		cmd.name = "WaterSimulation";
		ProfilingScope val = default(ProfilingScope);
		((ProfilingScope)(ref val))._002Ector(cmd, ProfilingSampler.Get<ProfileId>(ProfileId.SimulateWater));
		try
		{
			bool flag = false;
			if (m_terrainChangeCounter > 0)
			{
				if (m_NewMap)
				{
					m_terrainChangeCounter = 0;
					WaterSimSpeed = 1;
				}
				else
				{
					m_terrainChangeCounter--;
					if (m_terrainChangeCounter == 0)
					{
						WaterSimSpeed = 1;
						flag = true;
					}
				}
			}
			if (WaterSimSpeed > 0)
			{
				for (int i = 0; i < WaterSimSpeed; i++)
				{
					if (flag)
					{
						RestoreHeightFromHeightmap(cmd);
						flag = false;
						m_restoreHeightMinWaterHeight = kDefaultMinWaterToRestoreHeight;
					}
					EvaporateStep(cmd);
					SourceStep(cmd);
					VelocityStep(cmd);
					DepthStep(cmd);
					m_NextSimulationFrame += (uint)(4 * MaxSpeed / WaterSimSpeed);
					if (WaterSimSpeed > 1)
					{
						Graphics.ExecuteCommandBuffer(cmd);
						cmd.Clear();
					}
				}
				CopyToHeightmapStep(cmd);
				m_NewMap = false;
			}
			else
			{
				m_NextSimulationFrame += (uint)MaxSpeed;
				m_PreviousReadyFrame = (uint)(m_NextSimulationFrame / (ulong)MaxSpeed);
				m_LastReadyFrame = (uint)(m_NextSimulationFrame / (ulong)MaxSpeed);
			}
		}
		finally
		{
			((IDisposable)(*(ProfilingScope*)(&val))/*cast due to .constrained prefix*/).Dispose();
		}
		LastFrameSourceCache.Clear();
		UpdateGPUReadback();
	}

	public void OnSimulateGPU(CommandBuffer cmd)
	{
		if (Loaded && m_TexSize.x > 0)
		{
			Simulate(cmd);
		}
	}

	public void Save()
	{
		WaterSimSpeed = 0;
		((AsyncGPUReadbackHelper)(ref m_SaveAsyncGPUReadback)).Request((Texture)(object)WaterTexture, 0, (GraphicsFormat)52, (Action<AsyncGPUReadbackRequest>)null);
	}

	public void Restart()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		int2 val = m_TexSize / GridSize;
		int num = val.x * val.y;
		NativeArray<int> val2 = default(NativeArray<int>);
		val2._002Ector(num, (Allocator)3, (NativeArrayOptions)1);
		JobHandle val3 = IJobParallelForExtensions.Schedule<MemsetNativeArray<int>>(new MemsetNativeArray<int>
		{
			Source = val2,
			Value = 0
		}, num, 64, default(JobHandle));
		((JobHandle)(ref val3)).Complete();
		m_Active.SetData<int>(val2);
		val2.Dispose();
		num = m_TexSize.x * m_TexSize.y;
		NativeArray<float4> val4 = default(NativeArray<float4>);
		val4._002Ector(num, (Allocator)3, (NativeArrayOptions)1);
		JobHandle val5 = IJobParallelForExtensions.Schedule<MemsetNativeArray<float4>>(new MemsetNativeArray<float4>
		{
			Source = val4,
			Value = float4.op_Implicit(0)
		}, num, 64, default(JobHandle));
		((JobHandle)(ref val5)).Complete();
		ComputeBuffer val6 = new ComputeBuffer(num, UnsafeUtility.SizeOf<float4>(), (ComputeBufferType)0);
		val6.SetData<float4>(val4);
		m_UpdateShader.SetInt(m_ID_CellsPerArea, GridSize);
		m_UpdateShader.SetInt(m_ID_AreaCountX, m_TexSize.x / GridSize);
		m_UpdateShader.SetBuffer(m_LoadKernel, "_LoadSource", val6);
		m_UpdateShader.SetTexture(m_LoadKernel, m_ID_Result, (Texture)(object)WaterTexture);
		val6.Dispose();
		val4.Dispose();
	}

	public void JobLoad()
	{
		throw new NotImplementedException();
	}

	public unsafe byte[] CreateByteArray<T>(NativeArray<T> src) where T : struct
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		int num = UnsafeUtility.SizeOf<T>() * src.Length;
		byte* unsafeReadOnlyPtr = (byte*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr<T>(src);
		byte[] array = new byte[num];
		fixed (byte* ptr = array)
		{
			UnsafeUtility.MemCpy((void*)ptr, (void*)unsafeReadOnlyPtr, (long)num);
		}
		return array;
	}

	private void JobSaveToFile(NativeArray<float4> buffer)
	{
		throw new NotImplementedException();
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		((COSystemBase)this).OnStopRunning();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_SourceHandle)).Complete();
		m_SourceCacheIndex = 1 - m_SourceCacheIndex;
		CurrentJobSourceCache.Clear();
		JobHandle val = default(JobHandle);
		JobHandle val2 = default(JobHandle);
		SourceJob sourceJob = new SourceJob
		{
			m_SourceChunks = ((EntityQuery)(ref m_SourceGroup)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
			m_EventChunks = ((EntityQuery)(ref m_WaterLevelChangeGroup)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val2),
			m_ChangeType = InternalCompilerInterface.GetComponentTypeHandle<WaterLevelChange>(ref __TypeHandle.__Game_Events_WaterLevelChange_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SourceType = InternalCompilerInterface.GetComponentTypeHandle<WaterSourceData>(ref __TypeHandle.__Game_Simulation_WaterSourceData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ChangePrefabDatas = InternalCompilerInterface.GetComponentLookup<WaterLevelChangeData>(ref __TypeHandle.__Game_Prefabs_WaterLevelChangeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TerrainOffset = m_TerrainSystem.positionOffset,
			m_Cache = CurrentJobSourceCache
		};
		m_SourceHandle = IJobExtensions.Schedule<SourceJob>(sourceJob, JobUtils.CombineDependencies(((SystemBase)this).Dependency, m_SourceHandle, val, val2));
		((SystemBase)this).Dependency = m_SourceHandle;
		UpdateSaveReadback();
		((JobHandle)(ref m_ActiveReaders)).Complete();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<TerrainPropertiesData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1000286415_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public WaterSystem()
	{
	}

	bool IGPUSystem.get_Enabled()
	{
		return ((ComponentSystemBase)this).Enabled;
	}
}
