using System;
using Colossal.IO.AssetDatabase.VirtualTexturing;
using Colossal.Mathematics;
using Unity.Collections;
using UnityEngine;

namespace Colossal.Rendering;

public class VTTextureRequester : IDisposable
{
	private NativeList<int>[] m_TexturesIndices;

	private NativeList<int>[] m_StackGlobalIndices;

	private NativeList<Bounds2>[] m_TextureBounds;

	private NativeList<float>[] m_TexturesMaxPixels;

	private TextureStreamingSystem m_TextureStreamingSystem;

	private int m_RequestedThisFrame;

	public int stacksCount => m_TexturesIndices.Length;

	public int registeredCount
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			NativeList<int>[] texturesIndices = m_TexturesIndices;
			foreach (NativeList<int> val in texturesIndices)
			{
				num += val.Length;
			}
			return num;
		}
	}

	public int requestCount => m_RequestedThisFrame;

	public NativeList<float>[] TexturesMaxPixels => m_TexturesMaxPixels;

	public VTTextureRequester(TextureStreamingSystem textureStreamingSystem)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		m_TextureStreamingSystem = textureStreamingSystem;
		m_TexturesIndices = new NativeList<int>[2];
		m_TexturesIndices[0] = new NativeList<int>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_TexturesIndices[1] = new NativeList<int>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_StackGlobalIndices = new NativeList<int>[2];
		m_StackGlobalIndices[0] = new NativeList<int>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_StackGlobalIndices[1] = new NativeList<int>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_TexturesMaxPixels = new NativeList<float>[2];
		m_TexturesMaxPixels[0] = new NativeList<float>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_TexturesMaxPixels[1] = new NativeList<float>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_TextureBounds = new NativeList<Bounds2>[2];
		m_TextureBounds[0] = new NativeList<Bounds2>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_TextureBounds[1] = new NativeList<Bounds2>(100, AllocatorHandle.op_Implicit((Allocator)4));
	}

	public void Dispose()
	{
		for (int i = 0; i < m_TexturesIndices.Length; i++)
		{
			m_TexturesIndices[i].Dispose();
		}
		for (int j = 0; j < m_StackGlobalIndices.Length; j++)
		{
			m_StackGlobalIndices[j].Dispose();
		}
		for (int k = 0; k < m_TexturesMaxPixels.Length; k++)
		{
			m_TexturesMaxPixels[k].Dispose();
		}
		for (int l = 0; l < m_TextureBounds.Length; l++)
		{
			m_TextureBounds[l].Dispose();
		}
	}

	public void Clear()
	{
		for (int i = 0; i < m_TexturesIndices.Length; i++)
		{
			m_TexturesIndices[i].Clear();
		}
		for (int j = 0; j < m_StackGlobalIndices.Length; j++)
		{
			m_StackGlobalIndices[j].Clear();
		}
		for (int k = 0; k < m_TexturesMaxPixels.Length; k++)
		{
			m_TexturesMaxPixels[k].Clear();
		}
		for (int l = 0; l < m_TextureBounds.Length; l++)
		{
			m_TextureBounds[l].Clear();
		}
	}

	public void UpdateTexturesVTRequests()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		m_RequestedThisFrame = 0;
		float num = 1f;
		if (m_TextureStreamingSystem.workingSetLodBias > 1f)
		{
			num = 1f / m_TextureStreamingSystem.workingSetLodBias;
		}
		for (int i = 0; i < 2; i++)
		{
			NativeList<int> val = m_TexturesIndices[i];
			NativeList<int> val2 = m_StackGlobalIndices[i];
			NativeList<float> val3 = m_TexturesMaxPixels[i];
			for (int j = 0; j < val.Length; j++)
			{
				float num2 = val3[j] * num;
				if (num2 > 4f)
				{
					m_TextureStreamingSystem.RequestRegion(val2[j], val[j], num2, m_TextureBounds[i][j]);
					val3[j] = -1f;
					m_RequestedThisFrame++;
				}
			}
		}
	}

	public int RegisterTexture(int stackConfigIndex, int stackGlobalIndex, int vtIndex, Bounds2 bounds)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		NativeList<int> val = m_TexturesIndices[stackConfigIndex];
		NativeList<int> val2 = m_StackGlobalIndices[stackConfigIndex];
		NativeList<Bounds2> val3 = m_TextureBounds[stackConfigIndex];
		for (int i = 0; i < val.Length; i++)
		{
			if (val[i] == vtIndex && val2[i] == stackGlobalIndex)
			{
				Bounds2 val4 = val3[i];
				if (((Bounds2)(ref val4)).Equals(bounds))
				{
					return i;
				}
			}
		}
		val.Add(ref vtIndex);
		ref NativeList<float> reference = ref m_TexturesMaxPixels[stackConfigIndex];
		float num = -1f;
		reference.Add(ref num);
		val3.Add(ref bounds);
		val2.Add(ref stackGlobalIndex);
		return val.Length - 1;
	}

	public int GetTextureIndex(int stackIndex, int texturesIndex)
	{
		return m_TexturesIndices[stackIndex][texturesIndex];
	}

	public void UpdateMaxPixel(int stackIndex, int texturesIndex, float maxPixel)
	{
		m_TexturesMaxPixels[stackIndex][texturesIndex] = Mathf.Max(m_TexturesMaxPixels[stackIndex][texturesIndex], maxPixel);
	}
}
