using System;
using System.Collections.Generic;
using Colossal;
using Colossal.IO.AssetDatabase;
using Colossal.IO.AssetDatabase.VirtualTexturing;
using Colossal.Mathematics;
using Colossal.Rendering;
using Game.Prefabs;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Rendering.Debug;

public class RenderPrefabRenderer : MonoBehaviour
{
	public class Instance
	{
		private VTTextureRequester m_VTTexturesRequester;

		private List<int> m_VTTexturesIndices = new List<int>();

		private GameObject m_Root;

		private List<MeshRenderer> m_MeshRenderers = new List<MeshRenderer>();

		private Bounds3 m_Bounds;

		private RenderPrefabRenderer m_Owner;

		private ColorProperties m_ColorProperties;

		private EmissiveProperties m_EmissiveProperties;

		private ProceduralAnimationProperties m_ProceduralAnimationProperties;

		private CharacterProperties m_CharacterProperties;

		private CharacterGroupRenderer m_CharacterGroupRenderer;

		private DecalProperties m_DecalProperties;

		private ComputeBuffer m_LightBuffer;

		private ComputeBuffer m_AnimationBuffer;

		private RenderPrefab m_Prefab;

		private Dictionary<string, Transform> m_BoneMap = new Dictionary<string, Transform>();

		private Matrix4x4[] m_SkinMatrices;

		public string name => ((Object)m_Prefab).name;

		public GameObject root => m_Root;

		public bool enabled
		{
			get
			{
				return m_Root.activeInHierarchy;
			}
			set
			{
				m_Root.SetActive(value);
			}
		}

		public string GetStats()
		{
			return $"Vertices: {m_Prefab.vertexCount} Triangles: {m_Prefab.indexCount / 3}";
		}

		private void SetShaderPass(Material[] materials, string passName, bool enabled)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				materials[i].SetShaderPassEnabled(passName, enabled);
			}
		}

		private void SetKeyword(Material[] materials, string keywordName, bool enabled)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			foreach (Material val in materials)
			{
				LocalKeywordSpace keywordSpace = val.shader.keywordSpace;
				LocalKeyword val2 = ((LocalKeywordSpace)(ref keywordSpace)).FindKeyword(keywordName);
				if (((LocalKeyword)(ref val2)).isValid)
				{
					val.SetKeyword(ref val2, enabled);
				}
			}
		}

		public Instance(RenderPrefabRenderer mpr, RenderPrefab basePrefab, RenderPrefab prefab, bool useVT = true)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Expected O, but got Unknown
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			m_Owner = mpr;
			m_Prefab = prefab;
			m_Bounds = prefab.bounds;
			m_Root = new GameObject(((Object)prefab).name);
			m_Root.transform.parent = ((Component)mpr).transform;
			m_Root.transform.localPosition = Vector3.zero;
			m_CharacterProperties = prefab.GetComponent<CharacterProperties>();
			m_ColorProperties = basePrefab.GetComponent<ColorProperties>();
			m_EmissiveProperties = basePrefab.GetComponent<EmissiveProperties>();
			m_ProceduralAnimationProperties = prefab.GetComponent<ProceduralAnimationProperties>();
			Transform parent = ((Component)mpr).transform.parent;
			object characterGroupRenderer;
			if (parent == null)
			{
				characterGroupRenderer = null;
			}
			else
			{
				Transform parent2 = parent.parent;
				characterGroupRenderer = ((parent2 != null) ? ((Component)parent2).GetComponent<CharacterGroupRenderer>() : null);
			}
			m_CharacterGroupRenderer = (CharacterGroupRenderer)characterGroupRenderer;
			m_DecalProperties = basePrefab.GetComponent<DecalProperties>();
			Mesh[] array = prefab.ObtainMeshes();
			int num = 0;
			Material[] array2 = prefab.ObtainMaterials(useVT);
			LocalKeywordSpace keywordSpace;
			if (useVT)
			{
				TextureStreamingSystem existingSystemManaged = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<TextureStreamingSystem>();
				for (int i = 0; i < array2.Length; i++)
				{
					Material val = array2[i];
					val.SetFloat(ShaderIDs._LodFade, 1f);
					SurfaceAsset surfaceAsset = prefab.GetSurfaceAsset(i);
					VTAtlassingInfo[] array3 = surfaceAsset.VTAtlassingInfos;
					if (array3 == null)
					{
						array3 = surfaceAsset.PreReservedAtlassingInfos;
					}
					if (array3 == null)
					{
						continue;
					}
					if ((Object)(object)m_DecalProperties != (Object)null)
					{
						Bounds2 bounds = MathUtils.Bounds(m_DecalProperties.m_TextureArea.min, m_DecalProperties.m_TextureArea.max);
						if (m_VTTexturesRequester == null)
						{
							m_VTTexturesRequester = new VTTextureRequester(existingSystemManaged);
						}
						if (array3.Length >= 1 && array3[0].indexInStack >= 0)
						{
							m_VTTexturesIndices.Add(m_VTTexturesRequester.RegisterTexture(0, array3[0].stackGlobalIndex, array3[0].indexInStack, bounds));
						}
						if (array3.Length >= 2 && array3[1].indexInStack >= 0)
						{
							m_VTTexturesIndices.Add(m_VTTexturesRequester.RegisterTexture(1, array3[1].stackGlobalIndex, array3[1].indexInStack, bounds));
						}
					}
					if ((prefab.manualVTRequired || prefab.isImpostor) && (Object)(object)m_DecalProperties == (Object)null)
					{
						Bounds2 bounds2 = MathUtils.Bounds(new float2(0f, 0f), new float2(1f, 1f));
						if (m_VTTexturesRequester == null)
						{
							m_VTTexturesRequester = new VTTextureRequester(existingSystemManaged);
						}
						if (array3.Length >= 1 && array3[0].indexInStack >= 0)
						{
							m_VTTexturesIndices.Add(m_VTTexturesRequester.RegisterTexture(0, array3[0].stackGlobalIndex, array3[0].indexInStack, bounds2));
						}
						if (array3.Length >= 2 && array3[1].indexInStack >= 0)
						{
							m_VTTexturesIndices.Add(m_VTTexturesRequester.RegisterTexture(1, array3[1].stackGlobalIndex, array3[1].indexInStack, bounds2));
						}
					}
					for (int j = 0; j < 2; j++)
					{
						if (array3.Length > j && array3[j].indexInStack >= 0)
						{
							keywordSpace = val.shader.keywordSpace;
							LocalKeyword val2 = ((LocalKeywordSpace)(ref keywordSpace)).FindKeyword("ENABLE_VT");
							val.EnableKeyword(ref val2);
							existingSystemManaged.BindMaterial(val, array3[j].stackGlobalIndex, j, existingSystemManaged.GetTextureParamBlock(array3[j]));
						}
					}
				}
			}
			else
			{
				Material[] array4 = array2;
				foreach (Material obj in array4)
				{
					keywordSpace = obj.shader.keywordSpace;
					LocalKeyword val3 = ((LocalKeywordSpace)(ref keywordSpace)).FindKeyword("ENABLE_VT");
					obj.DisableKeyword(ref val3);
				}
			}
			Mesh[] array5 = array;
			foreach (Mesh val4 in array5)
			{
				try
				{
					GameObject val5 = new GameObject(((Object)val4).name);
					val5.transform.parent = m_Root.transform;
					val5.transform.localPosition = Vector3.zero;
					val5.AddComponent<MeshFilter>().sharedMesh = val4;
					MeshRenderer val6 = val5.AddComponent<MeshRenderer>();
					((Renderer)val6).sharedMaterials = new ReadOnlySpan<Material>(array2, num, val4.subMeshCount).ToArray();
					SetShaderPass(((Renderer)val6).sharedMaterials, "MOTIONVECTORS", ((Object)(object)m_ProceduralAnimationProperties != (Object)null && m_ProceduralAnimationProperties.active) || (Object)(object)m_CharacterGroupRenderer != (Object)null);
					SetKeyword(((Renderer)val6).sharedMaterials, "_GPU_ANIMATION_PROCEDURAL", (Object)(object)m_ProceduralAnimationProperties != (Object)null && m_ProceduralAnimationProperties.active);
					num += val4.subMeshCount;
					m_MeshRenderers.Add(val6);
				}
				catch (Exception arg)
				{
					Debug.LogError((object)$"Error with {((Object)basePrefab).name} {arg}", (Object)(object)basePrefab);
				}
			}
			SetupEmissiveProperties(-1);
			SetupProceduralAnimationProperties();
		}

		private float4x4 GetBone(string name, ProceduralAnimationProperties.BoneInfo[] bones, Transform root)
		{
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			string text = ((Object)m_Owner).name;
			string text2 = name + "@" + text;
			if (!m_BoneMap.TryGetValue(text2, out var value))
			{
				ProceduralAnimationProperties.BoneInfo[] array = bones;
				foreach (ProceduralAnimationProperties.BoneInfo boneInfo in array)
				{
					string text3 = boneInfo.name + "@" + text;
					GameObject val = new GameObject(boneInfo.name);
					m_BoneMap.Add(text3, val.transform);
					m_Owner.RegisterForAnimation(text3, val.transform, boneInfo);
				}
				array = bones;
				foreach (ProceduralAnimationProperties.BoneInfo boneInfo2 in array)
				{
					string text4 = boneInfo2.name + "@" + text;
					Transform val2 = m_BoneMap[text4];
					if (text4 == text2)
					{
						value = val2;
					}
					if (boneInfo2.parentId == -1)
					{
						val2.parent = root;
						val2.localPosition = boneInfo2.position;
						val2.localRotation = boneInfo2.rotation;
						val2.localScale = boneInfo2.scale;
					}
					else
					{
						string key = bones[boneInfo2.parentId].name + "@" + text;
						val2.parent = m_BoneMap[key];
						val2.localPosition = boneInfo2.position;
						val2.localRotation = boneInfo2.rotation;
						val2.localScale = boneInfo2.scale;
					}
				}
			}
			return math.mul(float4x4.op_Implicit(((Component)root).transform.worldToLocalMatrix), float4x4.op_Implicit(value.localToWorldMatrix));
		}

		private void SetupProceduralAnimationProperties()
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)m_ProceduralAnimationProperties != (Object)null && m_ProceduralAnimationProperties.active)
			{
				Transform transform = ((Component)m_MeshRenderers[0]).transform;
				int num = m_ProceduralAnimationProperties.m_Bones.Length;
				if (m_AnimationBuffer == null)
				{
					m_AnimationBuffer = new ComputeBuffer(num * 2, 64, (ComputeBufferType)1);
				}
				if (m_SkinMatrices == null || m_SkinMatrices.Length != num * 2)
				{
					m_SkinMatrices = (Matrix4x4[])(object)new Matrix4x4[num * 2];
				}
				float4x4.op_Implicit(transform.localToWorldMatrix);
				for (int i = 0; i < num; i++)
				{
					ProceduralAnimationProperties.BoneInfo boneInfo = m_ProceduralAnimationProperties.m_Bones[i];
					float4x4 bone = GetBone(boneInfo.name, m_ProceduralAnimationProperties.m_Bones, transform);
					m_SkinMatrices[i + num] = m_SkinMatrices[i];
					m_SkinMatrices[i] = float4x4.op_Implicit(math.mul(bone, float4x4.op_Implicit(boneInfo.bindPose)));
				}
				m_AnimationBuffer.SetData((Array)m_SkinMatrices);
			}
		}

		private void SetupEmissiveProperties(int lightIndex)
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected O, but got Unknown
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			if (!((Object)(object)m_EmissiveProperties != (Object)null) || !m_EmissiveProperties.active)
			{
				return;
			}
			int num = m_EmissiveProperties.lightsCount + 1;
			if (m_LightBuffer == null)
			{
				m_LightBuffer = new ComputeBuffer(num, 16, (ComputeBufferType)1);
			}
			List<Color> list = new List<Color>(num);
			list.Add(new Color(0f, 0f, 0f, 0f));
			Color val;
			if (m_EmissiveProperties.hasSingleLights)
			{
				foreach (EmissiveProperties.SingleLightMapping singleLight in m_EmissiveProperties.m_SingleLights)
				{
					val = new Color(singleLight.color.r, singleLight.color.g, singleLight.color.b, singleLight.intensity * 100f);
					list.Add(((Color)(ref val)).linear);
				}
			}
			if (m_EmissiveProperties.hasMultiLights)
			{
				if (lightIndex == -1)
				{
					foreach (EmissiveProperties.MultiLightMapping multiLight in m_EmissiveProperties.m_MultiLights)
					{
						val = new Color(multiLight.color.r, multiLight.color.g, multiLight.color.b, multiLight.intensity * 100f);
						list.Add(((Color)(ref val)).linear);
					}
				}
				else
				{
					for (int i = 0; i < m_EmissiveProperties.m_MultiLights.Count; i++)
					{
						EmissiveProperties.MultiLightMapping multiLightMapping = m_EmissiveProperties.m_MultiLights[i];
						if (i == lightIndex)
						{
							val = new Color(multiLightMapping.color.r, multiLightMapping.color.g, multiLightMapping.color.b, multiLightMapping.intensity * 100f);
							list.Add(((Color)(ref val)).linear);
						}
						else
						{
							list.Add(new Color(multiLightMapping.color.r, multiLightMapping.color.g, multiLightMapping.color.b, 0f));
						}
					}
				}
			}
			m_LightBuffer.SetData<Color>(list);
		}

		public void Update()
		{
			if (!enabled || m_VTTexturesRequester == null)
			{
				return;
			}
			float maxPixelSize = GetMaxPixelSize();
			for (int i = 0; i < m_VTTexturesIndices.Count; i++)
			{
				if (m_VTTexturesIndices[i] >= 0)
				{
					m_VTTexturesRequester.UpdateMaxPixel(0, m_VTTexturesIndices[i], maxPixelSize);
				}
			}
			m_VTTexturesRequester.UpdateTexturesVTRequests();
		}

		public void Dispose()
		{
			m_Prefab.Release();
			m_VTTexturesRequester?.Dispose();
			ComputeBuffer lightBuffer = m_LightBuffer;
			if (lightBuffer != null)
			{
				lightBuffer.Dispose();
			}
			ComputeBuffer animationBuffer = m_AnimationBuffer;
			if (animationBuffer != null)
			{
				animationBuffer.Dispose();
			}
			CoreUtils.Destroy((Object)(object)m_Root);
		}

		public void SetWindowProperties(float randomWin, ref MaterialPropertyBlock block)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			block.SetVector(ShaderIDs._BuildingState, new Vector4(0f, randomWin, 0f, 0f));
			foreach (MeshRenderer meshRenderer in m_MeshRenderers)
			{
				((Renderer)meshRenderer).SetPropertyBlock(block);
			}
		}

		public void SetColorProperties(int index, ref MaterialPropertyBlock block)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			if (!((Object)(object)m_ColorProperties != (Object)null) || !m_ColorProperties.active || index <= -1 || !m_ColorProperties.active)
			{
				return;
			}
			block.SetColor(ShaderIDs._ColorMask0, m_ColorProperties.GetColor(index, 0));
			block.SetColor(ShaderIDs._ColorMask1, m_ColorProperties.GetColor(index, 1));
			block.SetColor(ShaderIDs._ColorMask2, m_ColorProperties.GetColor(index, 2));
			foreach (MeshRenderer meshRenderer in m_MeshRenderers)
			{
				((Renderer)meshRenderer).SetPropertyBlock(block);
			}
		}

		public void SetDecalProperties(ref MaterialPropertyBlock block)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			if (!((Object)(object)m_DecalProperties != (Object)null) || !m_DecalProperties.active)
			{
				return;
			}
			block.SetVector(ShaderIDs._TextureArea, float4.op_Implicit(new float4(m_DecalProperties.m_TextureArea.min, m_DecalProperties.m_TextureArea.max)));
			block.SetVector(ShaderIDs._MeshSize, float4.op_Implicit(new float4(MathUtils.Size(m_Bounds), 0f)));
			block.SetFloat(ShaderIDs._DecalLayerMask, math.asfloat((int)m_DecalProperties.m_LayerMask));
			foreach (MeshRenderer meshRenderer in m_MeshRenderers)
			{
				((Renderer)meshRenderer).SetPropertyBlock(block);
			}
		}

		public void SetProceduralAnimationProperties(ref MaterialPropertyBlock block)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			SetupProceduralAnimationProperties();
			if (!((Object)(object)m_ProceduralAnimationProperties != (Object)null) || !m_ProceduralAnimationProperties.active)
			{
				return;
			}
			int num = m_ProceduralAnimationProperties.m_Bones.Length;
			block.SetBuffer("_BoneTransforms", m_AnimationBuffer);
			block.SetVector("colossal_BoneParameters", Vector4.op_Implicit(new Vector2(0f, (float)num)));
			block.SetInt("_BonePreviousTransformsByteOffset", num * 64);
			foreach (MeshRenderer meshRenderer in m_MeshRenderers)
			{
				((Renderer)meshRenderer).SetPropertyBlock(block);
			}
		}

		public void SetEmissiveProperties(int lightIndex, ref MaterialPropertyBlock block)
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			SetupEmissiveProperties(lightIndex);
			if (!((Object)(object)m_EmissiveProperties != (Object)null) || !m_EmissiveProperties.active)
			{
				return;
			}
			int num = m_EmissiveProperties.lightsCount + 1;
			block.SetBuffer("_LightInfo", m_LightBuffer);
			block.SetVector("colossal_LightParameters", new Vector4(0f, (float)num, 0f, 0f));
			int num2 = 0;
			foreach (MeshRenderer meshRenderer in m_MeshRenderers)
			{
				block.SetFloat("colossal_SingleLightsOffset", (float)m_EmissiveProperties.GetSingleLightOffset(num2++));
				((Renderer)meshRenderer).SetPropertyBlock(block);
			}
		}

		public void SetCharacterProperties(ref MaterialPropertyBlock block)
		{
			if (!((Object)(object)m_CharacterGroupRenderer != (Object)null))
			{
				return;
			}
			m_CharacterGroupRenderer.SetCharacterProperties(ref block);
			foreach (MeshRenderer meshRenderer in m_MeshRenderers)
			{
				((Renderer)meshRenderer).SetPropertyBlock(block);
			}
		}

		private float GetPixelSize(Camera camera, float radius)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			float num = math.distance(float3.op_Implicit(m_Root.transform.position), float3.op_Implicit(((Component)camera).transform.position));
			return radius / num * 360f / (float)Math.PI * (float)camera.pixelHeight / camera.fieldOfView;
		}

		private float GetMaxPixelSize()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			float radius = math.length(m_Bounds.max - m_Bounds.min) * 0.5f;
			float num = 0f;
			Camera[] allCameras = Camera.allCameras;
			foreach (Camera camera in allCameras)
			{
				num = Mathf.Max(num, GetPixelSize(camera, radius));
			}
			return num;
		}
	}

	public static class ShaderIDs
	{
		public static readonly int _BuildingState = Shader.PropertyToID("colossal_BuildingState");

		public static readonly int _ColorMask0 = Shader.PropertyToID("colossal_ColorMask0");

		public static readonly int _ColorMask1 = Shader.PropertyToID("colossal_ColorMask1");

		public static readonly int _ColorMask2 = Shader.PropertyToID("colossal_ColorMask2");

		public static readonly int _TextureArea = Shader.PropertyToID("colossal_TextureArea");

		public static readonly int _MeshSize = Shader.PropertyToID("colossal_MeshSize");

		public static readonly int _DecalLayerMask = Shader.PropertyToID("colossal_DecalLayerMask");

		public static readonly int _LodFade = Shader.PropertyToID("colossal_LodFade");

		public static readonly int _LodParameters = Shader.PropertyToID("colossal_LodParameters");
	}

	private class AnimationState
	{
		private BoneType type;

		public Transform target { get; }

		public AnimationState(Transform tr, ProceduralAnimationProperties.BoneInfo boneInfo)
		{
			target = tr;
			type = boneInfo.m_Type;
		}

		public virtual void Animate()
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			if (type == BoneType.SteeringTire || type == BoneType.VehicleConnection || type == BoneType.TrainBogie || type == BoneType.SteeringRotation || type == BoneType.SteeringSuspension)
			{
				target.Rotate(Vector3.up, Mathf.Cos(Time.time * 0.5f) * Time.deltaTime * 10f, (Space)0);
			}
			if (type == BoneType.RollingTire || type == BoneType.SteeringTire || type == BoneType.FixedTire)
			{
				target.Rotate(Vector3.right, Time.deltaTime * 60f, (Space)1);
			}
			if (type == BoneType.PoweredRotation || type == BoneType.PropellerRotation || type == BoneType.WindTurbineRotation || type == BoneType.WindSpeedRotation)
			{
				target.Rotate(Vector3.up, Time.deltaTime * 180f, (Space)1);
			}
			if (type == BoneType.OperatingRotation || type == BoneType.FixedRotation)
			{
				target.Rotate(Vector3.up, Time.deltaTime * 30f, (Space)1);
			}
			if (type == BoneType.LookAtDirection)
			{
				target.Rotate(Vector3.up, Mathf.Cos(Time.time * 0.8f) * Time.deltaTime * 50f, (Space)1);
			}
		}

		public virtual void Transfer(AnimationState state)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			state.target.localPosition = target.localPosition;
			state.target.localRotation = target.localRotation;
			state.target.localScale = target.localScale;
		}
	}

	private const bool kRefreshPrefabDataEveryFrame = true;

	public bool m_NoVT;

	public RenderPrefab m_Prefab;

	[Range(0f, 1f)]
	public float m_WindowsLight;

	[Range(-1f, 255f)]
	public int m_EmissiveLight = -1;

	[Range(-1f, 10f)]
	public int m_ColorIndex;

	[Range(0f, 3f)]
	public int m_LODIndex;

	public bool m_Animate = true;

	private List<Instance> m_Hierarchies;

	private MaterialPropertyBlock m_MaterialPropertyBlock;

	private Dictionary<string, List<AnimationState>> m_AnimationStates = new Dictionary<string, List<AnimationState>>();

	public IReadOnlyList<Instance> hierarchies => m_Hierarchies;

	public GameObject GetActiveRoot()
	{
		if (m_Hierarchies != null && m_LODIndex < m_Hierarchies.Count)
		{
			return m_Hierarchies[m_LODIndex].root;
		}
		return null;
	}

	private void OnEnable()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		if (!((Object)(object)m_Prefab != (Object)null))
		{
			return;
		}
		m_MaterialPropertyBlock = new MaterialPropertyBlock();
		m_Hierarchies = new List<Instance>();
		m_Hierarchies.Add(new Instance(this, m_Prefab, m_Prefab, !m_NoVT));
		if (m_Prefab.TryGet<LodProperties>(out var component))
		{
			RenderPrefab[] lodMeshes = component.m_LodMeshes;
			foreach (RenderPrefab prefab in lodMeshes)
			{
				Instance instance = new Instance(this, m_Prefab, prefab, !m_NoVT);
				instance.enabled = false;
				m_Hierarchies.Add(instance);
			}
		}
	}

	private void OnDisable()
	{
		if (m_Hierarchies == null)
		{
			return;
		}
		foreach (Instance hierarchy in m_Hierarchies)
		{
			hierarchy.Dispose();
		}
	}

	private void RegisterForAnimation(string boneName, Transform target, ProceduralAnimationProperties.BoneInfo boneInfo)
	{
		if (!m_AnimationStates.TryGetValue(boneName, out var value))
		{
			value = new List<AnimationState>();
			m_AnimationStates.Add(boneName, value);
		}
		if (value.FindIndex((AnimationState x) => (Object)(object)x.target == (Object)(object)target) == -1)
		{
			value.Add(new AnimationState(target, boneInfo));
		}
	}

	private void UpdateAnimations()
	{
		foreach (KeyValuePair<string, List<AnimationState>> animationState2 in m_AnimationStates)
		{
			AnimationState animationState = animationState2.Value[0];
			animationState.Animate();
			for (int i = 1; i < animationState2.Value.Count; i++)
			{
				animationState.Transfer(animationState2.Value[i]);
			}
		}
	}

	private void Update()
	{
		if (m_Animate)
		{
			UpdateAnimations();
		}
		for (int i = 0; i < m_Hierarchies.Count; i++)
		{
			m_MaterialPropertyBlock.Clear();
			Instance instance = m_Hierarchies[i];
			instance.Update();
			instance.SetWindowProperties(m_WindowsLight, ref m_MaterialPropertyBlock);
			instance.SetColorProperties(m_ColorIndex, ref m_MaterialPropertyBlock);
			instance.SetDecalProperties(ref m_MaterialPropertyBlock);
			instance.SetEmissiveProperties(m_EmissiveLight, ref m_MaterialPropertyBlock);
			instance.SetProceduralAnimationProperties(ref m_MaterialPropertyBlock);
			instance.SetCharacterProperties(ref m_MaterialPropertyBlock);
			instance.enabled = i == m_LODIndex;
		}
	}

	private void OnDrawGizmosSelected()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Renderer[] componentsInChildren = ((Component)this).GetComponentsInChildren<Renderer>();
		if (componentsInChildren != null && componentsInChildren.Length != 0)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Bounds bounds = componentsInChildren[i].bounds;
				Gizmos.matrix = Matrix4x4.identity;
				Gizmos.color = ColorUtils.NiceRandomColor(i);
				Gizmos.DrawWireCube(((Bounds)(ref bounds)).center, ((Bounds)(ref bounds)).extents * 2f);
			}
		}
	}
}
