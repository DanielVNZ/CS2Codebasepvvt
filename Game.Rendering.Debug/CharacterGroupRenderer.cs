using System;
using System.Runtime.CompilerServices;
using Game.Prefabs;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Rendering.Debug;

public class CharacterGroupRenderer : MonoBehaviour
{
	public CharacterGroup m_Prefab;

	public bool m_NoVT;

	public int m_CharacterIndex;

	[Range(0f, 16f)]
	public int m_OverlayColorIndex;

	private GameObject m_Root;

	private ComputeBuffer m_BoneBuffer;

	private ComputeBuffer m_BoneHistoryBuffer;

	private ComputeBuffer m_MetaBuffer;

	private void OnEnable()
	{
		CreateRenderer();
	}

	public void Recreate()
	{
		ReleaseRenderer();
		CreateRenderer();
	}

	private void CreateRenderer()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_Prefab != (Object)null)
		{
			m_CharacterIndex = Mathf.Clamp(m_CharacterIndex, 0, m_Prefab.m_Characters.Length - 1);
			CharacterGroup.Character character = m_Prefab.m_Characters[m_CharacterIndex];
			SetupBuffers(character);
			m_Root = new GameObject($"{((Object)m_Prefab).name} index {m_CharacterIndex}");
			m_Root.transform.parent = ((Component)this).transform;
			m_Root.transform.localPosition = Vector3.zero;
			RenderPrefab[] meshPrefabs = character.m_MeshPrefabs;
			foreach (RenderPrefab renderPrefab in meshPrefabs)
			{
				GameObject val = new GameObject(((Object)renderPrefab).name);
				val.transform.parent = m_Root.transform;
				val.transform.localPosition = Vector3.zero;
				val.SetActive(false);
				RenderPrefabRenderer renderPrefabRenderer = val.AddComponent<RenderPrefabRenderer>();
				renderPrefabRenderer.m_NoVT = m_NoVT;
				renderPrefabRenderer.m_Prefab = renderPrefab;
				val.SetActive(true);
			}
		}
	}

	private Color GetBlendColor(RenderPrefab[] renderPrefabs, BlendWeight weight)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		Color val = Color.white;
		int num = 0;
		for (int i = 0; i < renderPrefabs.Length; i++)
		{
			if (!renderPrefabs[i].TryGet<CharacterProperties>(out var component))
			{
				continue;
			}
			CharacterOverlay[] overlays = component.m_Overlays;
			for (int j = 0; j < overlays.Length; j++)
			{
				if (overlays[j].TryGet<CharacterOverlay>(out var component2) && component2.m_Index == weight.m_Index && component2.TryGet<ColorProperties>(out var component3))
				{
					if (num == 0)
					{
						((Color)(ref val))._002Ector(0f, 0f, 0f, 0f);
					}
					Color val2 = val;
					Color color = component3.GetColor(m_OverlayColorIndex, 0);
					val = val2 + ((Color)(ref color)).linear * weight.m_Weight;
					num++;
				}
			}
		}
		if (num > 0)
		{
			return val / (float)num;
		}
		return Color.white;
	}

	private BlendColors GetBlendColors(RenderPrefab[] renderPrefabs, BlendWeights weights)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		return new BlendColors
		{
			m_Color0 = GetBlendColor(renderPrefabs, weights.m_Weight0),
			m_Color1 = GetBlendColor(renderPrefabs, weights.m_Weight1),
			m_Color2 = GetBlendColor(renderPrefabs, weights.m_Weight2),
			m_Color3 = GetBlendColor(renderPrefabs, weights.m_Weight3),
			m_Color4 = GetBlendColor(renderPrefabs, weights.m_Weight4),
			m_Color5 = GetBlendColor(renderPrefabs, weights.m_Weight5),
			m_Color6 = GetBlendColor(renderPrefabs, weights.m_Weight6),
			m_Color7 = GetBlendColor(renderPrefabs, weights.m_Weight7)
		};
	}

	private void SetupBuffers(CharacterGroup.Character character)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		m_BoneBuffer = new ComputeBuffer(character.m_Style.m_BoneCount, System.Runtime.CompilerServices.Unsafe.SizeOf<BoneElement>(), (ComputeBufferType)16);
		m_BoneHistoryBuffer = new ComputeBuffer(character.m_Style.m_BoneCount, System.Runtime.CompilerServices.Unsafe.SizeOf<BoneElement>(), (ComputeBufferType)16);
		m_MetaBuffer = new ComputeBuffer(1, System.Runtime.CompilerServices.Unsafe.SizeOf<MetaBufferData>(), (ComputeBufferType)16);
		BoneElement[] array = new BoneElement[character.m_Style.m_BoneCount];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new BoneElement
			{
				m_Matrix = float4x4.identity
			};
		}
		BlendWeights blendWeights = RenderingUtils.GetBlendWeights(character.m_Meta.overlayWeights);
		BlendColors blendColors = GetBlendColors(character.m_MeshPrefabs, blendWeights);
		MetaBufferData[] data = new MetaBufferData[1]
		{
			new MetaBufferData
			{
				m_BoneCount = character.m_Style.m_BoneCount,
				m_ShapeCount = character.m_Style.m_ShapeCount,
				m_ShapeWeights = RenderingUtils.GetBlendWeights(character.m_Meta.shapeWeights),
				m_TextureWeights = RenderingUtils.GetBlendWeights(character.m_Meta.textureWeights),
				m_OverlayWeights = blendWeights,
				m_MaskWeights = RenderingUtils.GetBlendWeights(character.m_Meta.maskWeights),
				m_OverlayColors1 = blendColors
			}
		};
		m_BoneBuffer.SetData((Array)array);
		m_BoneHistoryBuffer.SetData((Array)array);
		m_MetaBuffer.SetData((Array)data);
	}

	public void SetCharacterProperties(ref MaterialPropertyBlock block)
	{
		if (m_BoneBuffer != null && m_MetaBuffer != null)
		{
			block.SetBuffer("boneBuffer", m_BoneBuffer);
			block.SetBuffer("boneHistoryBuffer", m_BoneHistoryBuffer);
			block.SetBuffer("metaBuffer", m_MetaBuffer);
		}
	}

	private void ReleaseRenderer()
	{
		if ((Object)(object)m_Root != (Object)null)
		{
			Object.Destroy((Object)(object)m_Root);
		}
		if (m_BoneBuffer != null)
		{
			m_BoneBuffer.Release();
			m_BoneBuffer = null;
		}
		if (m_BoneHistoryBuffer != null)
		{
			m_BoneHistoryBuffer.Release();
			m_BoneHistoryBuffer = null;
		}
		if (m_MetaBuffer != null)
		{
			m_MetaBuffer.Release();
			m_MetaBuffer = null;
		}
	}

	private void OnDisable()
	{
		ReleaseRenderer();
	}
}
