using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Game.Simulation;

[BurstCompile]
internal class SurfaceDataReader
{
	public delegate void CopyWaterValues_00005AAF_0024PostfixBurstDelegate(ref AsyncGPUReadbackRequest asyncReadback, ref NativeArray<SurfaceWater> cpu, ref NativeArray<float4> cpuTemp, ref JobHandle readers, ref int2 texSize, ref bool pendingReadback, int readbackDistribution, int readbackIndex);

	internal static class CopyWaterValues_00005AAF_0024BurstDirectCall
	{
		private static IntPtr Pointer;

		private static IntPtr DeferredCompilation;

		[BurstDiscard]
		private unsafe static void GetFunctionPointerDiscard(ref IntPtr P_0)
		{
			if (Pointer == (IntPtr)0)
			{
				Pointer = (nint)BurstCompiler.GetILPPMethodFunctionPointer2(DeferredCompilation, (RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/, typeof(CopyWaterValues_00005AAF_0024PostfixBurstDelegate).TypeHandle);
			}
			P_0 = Pointer;
		}

		private static IntPtr GetFunctionPointer()
		{
			nint result = 0;
			GetFunctionPointerDiscard(ref result);
			return result;
		}

		public static void Constructor()
		{
			DeferredCompilation = BurstCompiler.CompileILPPMethod2((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/);
		}

		public static void Initialize()
		{
		}

		static CopyWaterValues_00005AAF_0024BurstDirectCall()
		{
			Constructor();
		}

		public unsafe static void Invoke(ref AsyncGPUReadbackRequest asyncReadback, ref NativeArray<SurfaceWater> cpu, ref NativeArray<float4> cpuTemp, ref JobHandle readers, ref int2 texSize, ref bool pendingReadback, int readbackDistribution, int readbackIndex)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = GetFunctionPointer();
				if (functionPointer != (IntPtr)0)
				{
					((delegate* unmanaged[Cdecl]<ref AsyncGPUReadbackRequest, ref NativeArray<SurfaceWater>, ref NativeArray<float4>, ref JobHandle, ref int2, ref bool, int, int, void>)functionPointer)(ref asyncReadback, ref cpu, ref cpuTemp, ref readers, ref texSize, ref pendingReadback, readbackDistribution, readbackIndex);
					return;
				}
			}
			CopyWaterValues_0024BurstManaged(ref asyncReadback, ref cpu, ref cpuTemp, ref readers, ref texSize, ref pendingReadback, readbackDistribution, readbackIndex);
		}
	}

	private int m_ReadbackDistribution = 8;

	private int m_ReadbackIndex;

	private NativeArray<float4> m_CPUTemp;

	private NativeArray<SurfaceWater> m_CPU;

	private JobHandle m_Writers;

	private JobHandle m_Readers;

	private int2 m_TexSize;

	private int m_mapSize;

	private AsyncGPUReadbackRequest m_AsyncReadback;

	private bool m_PendingReadback;

	private RenderTexture m_sourceTexture;

	public JobHandle JobWriters => m_Writers;

	public JobHandle JobReaders
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_Readers;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Readers = value;
		}
	}

	public bool PendingReadback { get; set; }

	public NativeArray<SurfaceWater> WaterSurfaceCPUArray => m_CPU;

	public SurfaceDataReader(RenderTexture sourceTexture, int mapSize)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		m_sourceTexture = sourceTexture;
		m_TexSize = new int2(((Texture)sourceTexture).width, ((Texture)sourceTexture).height);
		m_mapSize = mapSize;
		m_CPU = new NativeArray<SurfaceWater>(m_TexSize.x * m_TexSize.y, (Allocator)4, (NativeArrayOptions)1);
		GetReadbackBounds(out var _, out var size);
		m_CPUTemp = new NativeArray<float4>(size.x * size.y, (Allocator)4, (NativeArrayOptions)1);
	}

	public void LoadData(NativeArray<float4> buffer)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_CPU.Length; i++)
		{
			float4 val = buffer[i];
			m_CPU[i] = new SurfaceWater
			{
				m_Depth = math.max(val.x, 0f),
				m_Polluted = val.w,
				m_Velocity = ((float4)(ref val)).yz
			};
		}
	}

	public void ExecuteReadBack()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (!m_PendingReadback)
		{
			((JobHandle)(ref m_Writers)).Complete();
			m_ReadbackIndex = (m_ReadbackIndex + 1) % (m_ReadbackDistribution * m_ReadbackDistribution);
			GetReadbackBounds(out var pos, out var size);
			m_AsyncReadback = AsyncGPUReadback.RequestIntoNativeArray<float4>(ref m_CPUTemp, (Texture)(object)m_sourceTexture, 0, pos.x, size.x, pos.y, size.y, 0, 1, (GraphicsFormat)52, (Action<AsyncGPUReadbackRequest>)CopyWaterValues);
			m_PendingReadback = true;
		}
	}

	private void CopyWaterValues(AsyncGPUReadbackRequest request)
	{
		CopyWaterValues(ref m_AsyncReadback, ref m_CPU, ref m_CPUTemp, ref m_Readers, ref m_TexSize, ref m_PendingReadback, m_ReadbackDistribution, m_ReadbackIndex);
	}

	[BurstCompile]
	private static void CopyWaterValues(ref AsyncGPUReadbackRequest asyncReadback, ref NativeArray<SurfaceWater> cpu, ref NativeArray<float4> cpuTemp, ref JobHandle readers, ref int2 texSize, ref bool pendingReadback, int readbackDistribution, int readbackIndex)
	{
		CopyWaterValues_00005AAF_0024BurstDirectCall.Invoke(ref asyncReadback, ref cpu, ref cpuTemp, ref readers, ref texSize, ref pendingReadback, readbackDistribution, readbackIndex);
	}

	public WaterSurfaceData GetSurfaceData(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		deps = m_Writers;
		int3 val = default(int3);
		if (m_CPU.Length != m_TexSize.x * m_TexSize.y)
		{
			((int3)(ref val))._002Ector(2, 2, 2);
		}
		else
		{
			((int3)(ref val))._002Ector(m_TexSize.x, 2, m_TexSize.y);
		}
		float3 val2 = default(float3);
		((float3)(ref val2))._002Ector((float)m_mapSize, 1f, (float)m_mapSize);
		float3 scale = new float3((float)val.x, (float)(val.y - 1), (float)val.z) / val2;
		float3 offset = -new float3((float)m_mapSize * -0.5f, 0f, (float)m_mapSize * -0.5f);
		return new WaterSurfaceData(m_CPU, val, scale, offset);
	}

	public SurfaceWater GetSurface(int2 cell)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return m_CPU[cell.x + 1 + m_TexSize.x * cell.y];
	}

	private void GetReadbackBounds(out int2 pos, out int2 size)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		GetReadbackBounds(m_TexSize, m_ReadbackDistribution, m_ReadbackIndex, out pos, out size);
	}

	private static void GetReadbackBounds(int2 texSize, int readbackDistribution, int readbackIndex, out int2 pos, out int2 size)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		size.x = texSize.x / readbackDistribution;
		size.y = texSize.y / readbackDistribution;
		pos.x = readbackIndex % readbackDistribution * size.x;
		pos.y = readbackIndex / readbackDistribution * size.y;
	}

	public void Dispose()
	{
		if (!((AsyncGPUReadbackRequest)(ref m_AsyncReadback)).done)
		{
			((AsyncGPUReadbackRequest)(ref m_AsyncReadback)).WaitForCompletion();
		}
		if (m_CPU.IsCreated)
		{
			m_CPU.Dispose();
		}
		if (m_CPUTemp.IsCreated)
		{
			m_CPUTemp.Dispose();
		}
		((JobHandle)(ref m_Readers)).Complete();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[BurstCompile]
	public static void CopyWaterValues_0024BurstManaged(ref AsyncGPUReadbackRequest asyncReadback, ref NativeArray<SurfaceWater> cpu, ref NativeArray<float4> cpuTemp, ref JobHandle readers, ref int2 texSize, ref bool pendingReadback, int readbackDistribution, int readbackIndex)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (!((AsyncGPUReadbackRequest)(ref asyncReadback)).hasError && cpu.IsCreated)
		{
			((JobHandle)(ref readers)).Complete();
			GetReadbackBounds(texSize, readbackDistribution, readbackIndex, out var pos, out var size);
			for (int i = 0; i < size.y; i++)
			{
				for (int j = 0; j < size.x; j++)
				{
					int num = pos.x + j + (pos.y + i) * texSize.x;
					float4 val = cpuTemp[j + i * size.x];
					float w = val.w;
					SurfaceWater surfaceWater = new SurfaceWater
					{
						m_Depth = val.x,
						m_Polluted = w,
						m_Velocity = ((float4)(ref val)).yz
					};
					cpu[num] = surfaceWater;
				}
			}
			pendingReadback = false;
		}
		else
		{
			Debug.LogWarning((object)"Error in readback");
		}
	}
}
