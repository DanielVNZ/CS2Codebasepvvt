using System;
using Colossal.AssetPipeline.Native;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.Serialization.Entities;
using Game.Prefabs;
using Game.Rendering;
using Game.Serialization;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Simulation;

[FormerlySerializedAs("Colossal.Terrain.SnowSystem, Game")]
public class SnowSystem : GameSystemBase, IDefaultSerializable, ISerializable, IPostDeserialize
{
	public struct ushort2
	{
		public ushort x;

		public ushort y;
	}

	private static ILog log = LogManager.GetLogger("SceneFlow");

	private const int kTexSize = 1024;

	private const int kGroupSizeAddSnow = 16;

	private const int kNumGroupAddSnow = 64;

	private const float kTimeStep = 0.2f;

	private const float kSnowHeightScale = 8f;

	private const float kSnowMeltScale = 1f;

	private const float m_SnowAddConstant = 1E-05f;

	private const float m_WaterAddConstant = 0.1f;

	private const int kSnowHeightBackdropTextureSize = 1024;

	private const float kSnowBackdropUpdateLerpFactor = 0.1f;

	private RenderTexture m_snowHeightBackdropTextureFinal;

	private ComputeBuffer m_snowBackdropBuffer;

	private ComputeBuffer m_MinHeights;

	private RenderTexture[] m_SnowHeights;

	private CommandBuffer m_CommandBuffer;

	private ComputeShader m_SnowUpdateShader;

	private int m_TransferKernel;

	private int m_AddKernel;

	private int m_ResetKernel;

	private int m_LoadKernel;

	private int m_LoadOldFormatKernel;

	private int m_UpdateBackdropSnowHeightTextureKernel;

	private int m_ClearBackdropSnowHeightTextureKernel;

	private int m_FinalizeBackdropSnowHeightTextureKernel;

	private int m_ID_SnowDepth;

	private int m_ID_OldSnowDepth;

	private int m_ID_Timestep;

	private int m_ID_AddMultiplier;

	private int m_ID_MeltMultiplier;

	private int m_ID_AddWaterMultiplier;

	private int m_ID_ElapseWaterMultiplier;

	private int m_ID_Temperature;

	private int m_ID_Rain;

	private int m_ID_Wind;

	private int m_ID_Time;

	private int m_ID_SnowScale;

	private int m_ID_MinHeights;

	private int m_ID_SnowHeightBackdropBuffer;

	private int m_ID_SnowHeightBackdropFinal;

	private int m_ID_SnowBackdropUpdateLerpFactor;

	private int m_ID_SnowHeightBackdropBufferSize;

	private TerrainSystem m_TerrainSystem;

	private SimulationSystem m_SimulationSystem;

	private TimeSystem m_TimeSystem;

	private ClimateSystem m_ClimateSystem;

	private WindSimulationSystem m_WindSimulationSystem;

	private WaterSystem m_WaterSystem;

	public RenderTexture SnowHeightBackdropTexture => m_snowHeightBackdropTextureFinal;

	public int SnowSimSpeed { get; set; }

	public int2 TextureSize => new int2(1024, 1024);

	public bool Loaded => (Object)(object)m_SnowUpdateShader != (Object)null;

	public ComputeShader m_SnowTransferShader { private get; set; }

	public ComputeShader m_DynamicHeightShader { private get; set; }

	private float4 SnowScaleVector => new float4(8f, 1f, 1f, 1f);

	private int Write { get; set; }

	private int Read => 1 - Write;

	public RenderTexture SnowDepth
	{
		get
		{
			if (m_SnowHeights != null)
			{
				return m_SnowHeights[Read];
			}
			return null;
		}
	}

	public bool IsAsync { get; set; }

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 4;
	}

	private void InitShader()
	{
		m_SnowUpdateShader = AssetDatabase.global.resources.shaders.snowUpdate;
		m_ResetKernel = m_SnowUpdateShader.FindKernel("Reset");
		m_LoadKernel = m_SnowUpdateShader.FindKernel("Load");
		m_LoadOldFormatKernel = m_SnowUpdateShader.FindKernel("LoadOldFormat");
		m_AddKernel = m_SnowUpdateShader.FindKernel("Add");
		m_TransferKernel = m_SnowUpdateShader.FindKernel("Transfer");
		m_UpdateBackdropSnowHeightTextureKernel = m_SnowUpdateShader.FindKernel("UpdateBackdropSnowHeightTexture");
		m_ClearBackdropSnowHeightTextureKernel = m_SnowUpdateShader.FindKernel("ClearBackdropSnowHeightTexture");
		m_FinalizeBackdropSnowHeightTextureKernel = m_SnowUpdateShader.FindKernel("FinalizeBackdropSnowHeightTexture");
	}

	public unsafe void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(1024);
		((IWriter)writer/*cast due to .constrained prefix*/).Write(1024);
		int num = UnsafeUtility.SizeOf(typeof(ushort2));
		NativeArray<byte> val = default(NativeArray<byte>);
		val._002Ector(1048576 * num, (Allocator)4, (NativeArrayOptions)1);
		AsyncGPUReadbackRequest val2 = AsyncGPUReadback.RequestIntoNativeArray<byte>(ref val, (Texture)(object)m_SnowHeights[Read], 0, (Action<AsyncGPUReadbackRequest>)null);
		((AsyncGPUReadbackRequest)(ref val2)).WaitForCompletion();
		if (!((AsyncGPUReadbackRequest)(ref val2)).done)
		{
			log.Warn((object)"Snow request not done after WaitForCompletion");
		}
		if (((AsyncGPUReadbackRequest)(ref val2)).hasError)
		{
			log.Warn((object)"Snow request has error after WaitForCompletion");
		}
		NativeArray<byte> val3 = default(NativeArray<byte>);
		val3._002Ector(val.Length, (Allocator)2, (NativeArrayOptions)1);
		NativeCompression.FilterDataBeforeWrite((IntPtr)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr<byte>(val), (IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr<byte>(val3), (long)val3.Length, num);
		val.Dispose();
		NativeArray<byte> val4 = val3;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val4);
		val3.Dispose();
	}

	public void PostDeserialize(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (((Context)(ref context)).version < Version.snow)
		{
			m_SnowUpdateShader.SetTexture(m_ResetKernel, "_Result", (Texture)(object)m_SnowHeights[Write]);
			m_SnowUpdateShader.Dispatch(m_ResetKernel, 64, 64, 1);
			m_SnowUpdateShader.SetTexture(m_ResetKernel, "_Result", (Texture)(object)m_SnowHeights[Read]);
			m_SnowUpdateShader.Dispatch(m_ResetKernel, 64, 64, 1);
		}
		Shader.SetGlobalTexture("_SnowMap", (Texture)(object)SnowDepth);
	}

	public void DebugReset()
	{
		m_SnowUpdateShader.SetTexture(m_ResetKernel, "_Result", (Texture)(object)m_SnowHeights[Write]);
		m_SnowUpdateShader.Dispatch(m_ResetKernel, 64, 64, 1);
		m_SnowUpdateShader.SetTexture(m_ResetKernel, "_Result", (Texture)(object)m_SnowHeights[Read]);
		m_SnowUpdateShader.Dispatch(m_ResetKernel, 64, 64, 1);
	}

	public unsafe void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Expected O, but got Unknown
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Expected O, but got Unknown
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		int num2 = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
		bool flag = true;
		if (num != 1024)
		{
			Debug.LogWarning((object)("Saved snow width = " + num + ", snow tex width = " + 1024));
			flag = false;
		}
		if (num2 != 1024)
		{
			Debug.LogWarning((object)("Saved snow height = " + num2 + ", snow tex height = " + 1024));
			flag = false;
		}
		int num3 = num * num2;
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.snow16bits)
		{
			int num4 = UnsafeUtility.SizeOf(typeof(ushort2));
			NativeArray<ushort2> val = default(NativeArray<ushort2>);
			val._002Ector(num3, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<byte> val2 = default(NativeArray<byte>);
			val2._002Ector(num3 * num4, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<byte> val3 = val2;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val3);
			NativeCompression.UnfilterDataAfterRead((IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr<byte>(val2), (IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr<ushort2>(val), (long)val2.Length, num4);
			val2.Dispose();
			if (flag)
			{
				NativeArray<float2> data = default(NativeArray<float2>);
				data._002Ector(num3, (Allocator)2, (NativeArrayOptions)1);
				int num5 = 0;
				Enumerator<ushort2> enumerator = val.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ushort2 current = enumerator.Current;
						data[num5++] = new float2((float)(int)current.x / 65535f, (float)(int)current.y / 65535f);
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
				ComputeBuffer val4 = new ComputeBuffer(num3, UnsafeUtility.SizeOf<float2>(), (ComputeBufferType)0);
				val4.SetData<float2>(data);
				m_CommandBuffer.SetComputeBufferParam(m_SnowUpdateShader, m_LoadKernel, "_LoadSource", val4);
				m_CommandBuffer.SetComputeTextureParam(m_SnowUpdateShader, m_LoadKernel, "_Result", RenderTargetIdentifier.op_Implicit((Texture)(object)m_SnowHeights[Write]));
				m_CommandBuffer.DispatchCompute(m_SnowUpdateShader, m_LoadKernel, 64, 64, 1);
				m_CommandBuffer.SetComputeTextureParam(m_SnowUpdateShader, m_LoadKernel, "_Result", RenderTargetIdentifier.op_Implicit((Texture)(object)m_SnowHeights[Read]));
				m_CommandBuffer.DispatchCompute(m_SnowUpdateShader, m_LoadKernel, 64, 64, 1);
				m_CommandBuffer.SetComputeBufferParam(m_SnowUpdateShader, m_ClearBackdropSnowHeightTextureKernel, m_ID_SnowHeightBackdropBuffer, m_snowBackdropBuffer);
				m_CommandBuffer.DispatchCompute(m_SnowUpdateShader, m_ClearBackdropSnowHeightTextureKernel, 64, 1, 1);
				AddSnow(m_CommandBuffer);
				SnowTransfer(m_CommandBuffer);
				UpdateSnowBackdropTexture(m_CommandBuffer, 1f);
				Graphics.ExecuteCommandBuffer(m_CommandBuffer);
				val4.Dispose();
				data.Dispose();
			}
			val.Dispose();
		}
		else
		{
			NativeArray<float4> val5 = default(NativeArray<float4>);
			val5._002Ector(num3, (Allocator)2, (NativeArrayOptions)1);
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version >= Version.terrainWaterSnowCompression)
			{
				NativeArray<byte> val6 = default(NativeArray<byte>);
				val6._002Ector(num3 * 16, (Allocator)2, (NativeArrayOptions)1);
				NativeArray<byte> val7 = val6;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val7);
				NativeCompression.UnfilterDataAfterRead((IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr<byte>(val6), (IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr<float4>(val5), (long)val6.Length, 16);
				val6.Dispose();
			}
			else
			{
				NativeArray<float4> val8 = val5;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val8);
			}
			if (flag)
			{
				ComputeBuffer val9 = new ComputeBuffer(num3, UnsafeUtility.SizeOf<float4>(), (ComputeBufferType)0);
				val9.SetData<float4>(val5);
				m_SnowUpdateShader.SetVector("_LoadScale", float4.op_Implicit(SnowScaleVector));
				m_SnowUpdateShader.SetBuffer(m_LoadOldFormatKernel, "_LoadSourceOldFormat", val9);
				m_SnowUpdateShader.SetTexture(m_LoadOldFormatKernel, "_Result", (Texture)(object)m_SnowHeights[Write]);
				m_SnowUpdateShader.Dispatch(m_LoadOldFormatKernel, 64, 64, 1);
				m_SnowUpdateShader.SetTexture(m_LoadOldFormatKernel, "_Result", (Texture)(object)m_SnowHeights[Read]);
				m_SnowUpdateShader.Dispatch(m_LoadOldFormatKernel, 64, 64, 1);
				val9.Dispose();
			}
			val5.Dispose();
		}
		Shader.SetGlobalVector("colossal_SnowScale", float4.op_Implicit(SnowScaleVector));
	}

	public void SetDefaults(Context context)
	{
		m_SnowUpdateShader.SetTexture(m_ResetKernel, "_Result", (Texture)(object)m_SnowHeights[Write]);
		m_SnowUpdateShader.Dispatch(m_ResetKernel, 64, 64, 1);
		m_SnowUpdateShader.SetTexture(m_ResetKernel, "_Result", (Texture)(object)m_SnowHeights[Read]);
		m_SnowUpdateShader.Dispatch(m_ResetKernel, 64, 64, 1);
		Shader.SetGlobalTexture("_SnowMap", (Texture)(object)SnowDepth);
	}

	public void UpdateDynamicHeights()
	{
	}

	private RenderTexture CreateTexture(string name)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		RenderTexture val = new RenderTexture(1024, 1024, 0, (GraphicsFormat)22)
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

	private void InitTextures()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		m_SnowHeights = (RenderTexture[])(object)new RenderTexture[2];
		m_SnowHeights[0] = CreateTexture("SnowRT0");
		m_SnowHeights[1] = CreateTexture("SnowRT1");
		m_MinHeights = new ComputeBuffer(4096, UnsafeUtility.SizeOf<float2>(), (ComputeBufferType)0);
		m_snowBackdropBuffer = new ComputeBuffer(1024, UnsafeUtility.SizeOf<uint2>(), (ComputeBufferType)0);
		m_snowHeightBackdropTextureFinal = new RenderTexture(1024, 1, 0, (GraphicsFormat)49)
		{
			name = "SnowBackdropHeightTextureFinal",
			hideFlags = (HideFlags)52,
			enableRandomWrite = true,
			wrapMode = (TextureWrapMode)1,
			filterMode = (FilterMode)1
		};
		m_snowHeightBackdropTextureFinal.Create();
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Expected O, but got Unknown
		base.OnCreate();
		InitShader();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_WindSimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindSimulationSystem>();
		InitTextures();
		m_ID_SnowDepth = Shader.PropertyToID("_Result");
		m_ID_OldSnowDepth = Shader.PropertyToID("_Previous");
		m_ID_Timestep = Shader.PropertyToID("_Timestep");
		m_ID_AddMultiplier = Shader.PropertyToID("_AddMultiplier");
		m_ID_MeltMultiplier = Shader.PropertyToID("_MeltMultiplier");
		m_ID_AddWaterMultiplier = Shader.PropertyToID("_AddWaterMultiplier");
		m_ID_ElapseWaterMultiplier = Shader.PropertyToID("_ElapseWaterMultiplier");
		m_ID_Temperature = Shader.PropertyToID("_Temperature");
		m_ID_Rain = Shader.PropertyToID("_Rain");
		m_ID_Time = Shader.PropertyToID("_SimTime");
		m_ID_Wind = Shader.PropertyToID("_Wind");
		m_ID_SnowScale = Shader.PropertyToID("_SnowScale");
		m_ID_MinHeights = Shader.PropertyToID("_MinHeights");
		m_ID_SnowHeightBackdropBuffer = Shader.PropertyToID("_SnowHeightBackdropBuffer");
		m_ID_SnowHeightBackdropFinal = Shader.PropertyToID("_SnowHeightBackdropTextureFinal");
		m_ID_SnowBackdropUpdateLerpFactor = Shader.PropertyToID("_SnowBackdropUpdateLerpFactor");
		m_ID_SnowHeightBackdropBufferSize = Shader.PropertyToID("_SnowHeightBackdropBufferSize");
		((ComponentSystemBase)this).RequireForUpdate<TerrainPropertiesData>();
		m_CommandBuffer = new CommandBuffer();
		m_CommandBuffer.name = "Snowsystem";
		SnowSimSpeed = 1;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_CommandBuffer.Dispose();
		CoreUtils.Destroy((Object)(object)m_SnowHeights[0]);
		CoreUtils.Destroy((Object)(object)m_SnowHeights[1]);
		m_MinHeights.Release();
		m_snowBackdropBuffer.Release();
		CoreUtils.Destroy((Object)(object)m_snowHeightBackdropTextureFinal);
	}

	private void FlipSnow()
	{
		Write = 1 - Write;
	}

	private float GetSnowiness()
	{
		return Mathf.Sin((float)Math.PI * 40f * m_TimeSystem.normalizedDate);
	}

	private unsafe void AddSnow(CommandBuffer cmd)
	{
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		ProfilingScope val = default(ProfilingScope);
		((ProfilingScope)(ref val))._002Ector(cmd, ProfilingSampler.Get<ProfileId>(ProfileId.AddSnow));
		try
		{
			cmd.SetComputeFloatParam(m_SnowUpdateShader, m_ID_Timestep, 0.2f);
			cmd.SetComputeFloatParam(m_SnowUpdateShader, m_ID_AddMultiplier, 1E-05f);
			cmd.SetComputeFloatParam(m_SnowUpdateShader, m_ID_MeltMultiplier, 0.00012f);
			cmd.SetComputeFloatParam(m_SnowUpdateShader, m_ID_AddWaterMultiplier, 0.1f);
			cmd.SetComputeFloatParam(m_SnowUpdateShader, m_ID_ElapseWaterMultiplier, 0.05f);
			cmd.SetComputeFloatParam(m_SnowUpdateShader, m_ID_Temperature, (float)m_ClimateSystem.temperature);
			cmd.SetComputeFloatParam(m_SnowUpdateShader, m_ID_Rain, (float)m_ClimateSystem.precipitation);
			cmd.SetComputeVectorParam(m_SnowUpdateShader, m_ID_Wind, float4.op_Implicit(new float4(m_WindSimulationSystem.constantWind, 0f, 0f)));
			cmd.SetComputeVectorParam(m_SnowUpdateShader, m_ID_SnowScale, float4.op_Implicit(SnowScaleVector));
			cmd.SetComputeFloatParam(m_SnowUpdateShader, m_ID_Time, m_TimeSystem.normalizedTime);
			cmd.SetComputeTextureParam(m_SnowUpdateShader, m_AddKernel, "_Terrain", RenderTargetIdentifier.op_Implicit(m_TerrainSystem.heightmap));
			cmd.SetComputeVectorParam(m_SnowUpdateShader, "_HeightScale", float4.op_Implicit(new float4(m_TerrainSystem.heightScaleOffset, m_ClimateSystem.temperatureBaseHeight, m_ClimateSystem.snowTemperatureHeightScale)));
			cmd.SetComputeTextureParam(m_SnowUpdateShader, m_AddKernel, m_ID_OldSnowDepth, RenderTargetIdentifier.op_Implicit((Texture)(object)m_SnowHeights[Read]));
			cmd.SetComputeTextureParam(m_SnowUpdateShader, m_AddKernel, m_ID_SnowDepth, RenderTargetIdentifier.op_Implicit((Texture)(object)m_SnowHeights[Write]));
			cmd.SetComputeTextureParam(m_SnowUpdateShader, m_AddKernel, "_Water", RenderTargetIdentifier.op_Implicit((Texture)(object)m_WaterSystem.WaterTexture));
			cmd.SetComputeBufferParam(m_SnowUpdateShader, m_AddKernel, m_ID_MinHeights, m_MinHeights);
			cmd.DispatchCompute(m_SnowUpdateShader, m_AddKernel, 64, 64, 1);
		}
		finally
		{
			((IDisposable)(*(ProfilingScope*)(&val))/*cast due to .constrained prefix*/).Dispose();
		}
		FlipSnow();
	}

	private unsafe void SnowTransfer(CommandBuffer cmd)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		ProfilingScope val = default(ProfilingScope);
		((ProfilingScope)(ref val))._002Ector(cmd, ProfilingSampler.Get<ProfileId>(ProfileId.TransferSnow));
		try
		{
			if ((float)m_ClimateSystem.precipitation < 0.1f || (float)m_ClimateSystem.temperature - 0.01f * (m_TerrainSystem.heightScaleOffset.x - m_ClimateSystem.temperatureBaseHeight) > 0f)
			{
				return;
			}
			cmd.SetComputeVectorParam(m_SnowUpdateShader, m_ID_SnowScale, float4.op_Implicit(SnowScaleVector));
			cmd.SetComputeVectorParam(m_SnowUpdateShader, "_HeightScale", float4.op_Implicit(new float4(m_TerrainSystem.heightScaleOffset, m_ClimateSystem.temperatureBaseHeight, 0f)));
			cmd.SetComputeTextureParam(m_SnowUpdateShader, m_TransferKernel, m_ID_OldSnowDepth, RenderTargetIdentifier.op_Implicit((Texture)(object)m_SnowHeights[Read]));
			cmd.SetComputeTextureParam(m_SnowUpdateShader, m_TransferKernel, m_ID_SnowDepth, RenderTargetIdentifier.op_Implicit((Texture)(object)m_SnowHeights[Write]));
			cmd.SetComputeTextureParam(m_SnowUpdateShader, m_TransferKernel, "_Terrain", RenderTargetIdentifier.op_Implicit(m_TerrainSystem.heightmap));
			cmd.SetComputeVectorParam(m_SnowUpdateShader, m_ID_Wind, float4.op_Implicit(new float4(m_WindSimulationSystem.constantWind, 0f, 0f)));
			cmd.DispatchCompute(m_SnowUpdateShader, m_TransferKernel, 64, 64, 1);
		}
		finally
		{
			((IDisposable)(*(ProfilingScope*)(&val))/*cast due to .constrained prefix*/).Dispose();
		}
		FlipSnow();
	}

	private unsafe void UpdateSnowBackdropTexture(CommandBuffer cmd, float lerpFactor)
	{
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		ProfilingScope val = default(ProfilingScope);
		((ProfilingScope)(ref val))._002Ector(m_CommandBuffer, ProfilingSampler.Get<ProfileId>(ProfileId.UpdateSnowHeightBackdrop));
		try
		{
			cmd.SetComputeBufferParam(m_SnowUpdateShader, m_ClearBackdropSnowHeightTextureKernel, m_ID_SnowHeightBackdropBuffer, m_snowBackdropBuffer);
			cmd.DispatchCompute(m_SnowUpdateShader, m_ClearBackdropSnowHeightTextureKernel, 64, 1, 1);
			cmd.SetComputeBufferParam(m_SnowUpdateShader, m_UpdateBackdropSnowHeightTextureKernel, m_ID_SnowHeightBackdropBuffer, m_snowBackdropBuffer);
			cmd.SetComputeBufferParam(m_SnowUpdateShader, m_UpdateBackdropSnowHeightTextureKernel, m_ID_MinHeights, m_MinHeights);
			cmd.SetComputeIntParam(m_SnowUpdateShader, m_ID_SnowHeightBackdropBufferSize, 1024);
			cmd.DispatchCompute(m_SnowUpdateShader, m_UpdateBackdropSnowHeightTextureKernel, 256, 1, 1);
			cmd.SetComputeBufferParam(m_SnowUpdateShader, m_FinalizeBackdropSnowHeightTextureKernel, m_ID_SnowHeightBackdropBuffer, m_snowBackdropBuffer);
			cmd.SetComputeTextureParam(m_SnowUpdateShader, m_FinalizeBackdropSnowHeightTextureKernel, m_ID_SnowHeightBackdropFinal, RenderTargetIdentifier.op_Implicit((Texture)(object)m_snowHeightBackdropTextureFinal));
			cmd.SetComputeFloatParam(m_SnowUpdateShader, m_ID_SnowBackdropUpdateLerpFactor, lerpFactor);
			cmd.DispatchCompute(m_SnowUpdateShader, m_FinalizeBackdropSnowHeightTextureKernel, 1, 1, 1);
		}
		finally
		{
			((IDisposable)(*(ProfilingScope*)(&val))/*cast due to .constrained prefix*/).Dispose();
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		if (m_WaterSystem.Loaded)
		{
			m_CommandBuffer.Clear();
			for (int i = 0; i < SnowSimSpeed; i++)
			{
				AddSnow(m_CommandBuffer);
				SnowTransfer(m_CommandBuffer);
			}
			UpdateSnowBackdropTexture(m_CommandBuffer, 0.1f);
			Shader.SetGlobalTexture("_SnowMap", (Texture)(object)SnowDepth);
			Graphics.ExecuteCommandBuffer(m_CommandBuffer);
		}
	}

	[Preserve]
	public SnowSystem()
	{
	}
}
