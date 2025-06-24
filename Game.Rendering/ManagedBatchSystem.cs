using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Colossal;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.IO.AssetDatabase.VirtualTexturing;
using Colossal.Mathematics;
using Colossal.Rendering;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class ManagedBatchSystem : GameSystemBase
{
	private struct KeywordData
	{
		public string name { get; private set; }

		public bool remove { get; private set; }

		public KeywordData(string name, bool remove)
		{
			this.name = name;
			this.remove = remove;
		}
	}

	private struct TextureData
	{
		public int nameID { get; private set; }

		public Texture texture { get; private set; }

		public TextureData(int nameID, Texture texture)
		{
			this.nameID = nameID;
			this.texture = texture;
		}
	}

	public class MaterialKey : IEquatable<MaterialKey>
	{
		public Shader shader { get; set; }

		public Material template { get; set; }

		public int decalLayerMask { get; set; }

		public int renderQueue { get; set; }

		public HashSet<string> keywords { get; private set; }

		public List<int> vtStacks { get; private set; }

		public Dictionary<int, object> textures { get; private set; }

		public MaterialKey()
		{
			decalLayerMask = -1;
			renderQueue = -1;
			keywords = new HashSet<string>();
			vtStacks = new List<int>();
			textures = new Dictionary<int, object>();
		}

		public MaterialKey(MaterialKey source)
		{
			shader = source.shader;
			template = source.template;
			decalLayerMask = source.decalLayerMask;
			renderQueue = source.renderQueue;
			keywords = new HashSet<string>(source.keywords);
			vtStacks = new List<int>(source.vtStacks);
			textures = new Dictionary<int, object>(source.textures);
		}

		public void Initialize(SurfaceAsset surface)
		{
			template = GetTemplate(surface);
			renderQueue = template.renderQueue;
			foreach (string keyword in surface.keywords)
			{
				keywords.Add(keyword);
			}
			foreach (KeyValuePair<string, TextureAsset> texture in surface.textures)
			{
				if (!surface.IsHandledByVirtualTexturing(texture))
				{
					textures.Add(Shader.PropertyToID(texture.Key), texture.Value);
				}
			}
		}

		public void Initialize(Material material)
		{
			shader = material.shader;
			renderQueue = material.renderQueue;
			string[] shaderKeywords = material.shaderKeywords;
			foreach (string item in shaderKeywords)
			{
				keywords.Add(item);
			}
			int[] texturePropertyNameIDs = material.GetTexturePropertyNameIDs();
			foreach (int num in texturePropertyNameIDs)
			{
				textures.Add(num, material.GetTexture(num));
			}
		}

		public void Clear()
		{
			shader = null;
			template = null;
			decalLayerMask = -1;
			renderQueue = -1;
			keywords.Clear();
			vtStacks.Clear();
			textures.Clear();
		}

		public bool Equals(MaterialKey other)
		{
			if ((Object)(object)shader != (Object)(object)other.shader || (Object)(object)template != (Object)(object)other.template || decalLayerMask != other.decalLayerMask || renderQueue != other.renderQueue || keywords.Count != other.keywords.Count || vtStacks.Count != other.vtStacks.Count || textures.Count != other.textures.Count)
			{
				return false;
			}
			foreach (string keyword in keywords)
			{
				if (!other.keywords.Contains(keyword))
				{
					return false;
				}
			}
			for (int i = 0; i < vtStacks.Count; i++)
			{
				if (vtStacks[i] != other.vtStacks[i])
				{
					return false;
				}
			}
			foreach (KeyValuePair<int, object> texture in textures)
			{
				if (!other.textures.TryGetValue(texture.Key, out var value) || texture.Value != value)
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = decalLayerMask.GetHashCode() ^ renderQueue.GetHashCode();
			if ((Object)(object)shader != (Object)null)
			{
				num ^= ((object)shader).GetHashCode();
			}
			if ((Object)(object)template != (Object)null)
			{
				num ^= ((object)template).GetHashCode();
			}
			foreach (string keyword in keywords)
			{
				num ^= keyword.GetHashCode();
			}
			int count = vtStacks.Count;
			foreach (int vtStack in vtStacks)
			{
				num ^= vtStack.GetHashCode() + count;
			}
			foreach (KeyValuePair<int, object> texture in textures)
			{
				num ^= ((texture.Value != null) ? texture.Value.GetHashCode() : texture.Key.GetHashCode());
			}
			return num;
		}
	}

	private class GroupKey : IEquatable<GroupKey>
	{
		public struct Batch : IEquatable<Batch>
		{
			public Material loadedMaterial { get; set; }

			public BatchFlags flags { get; set; }

			public Batch(CustomBatch batch)
			{
				loadedMaterial = batch.loadedMaterial;
				flags = batch.sourceFlags;
			}

			public bool Equals(Batch other)
			{
				if ((Object)(object)loadedMaterial == (Object)(object)other.loadedMaterial)
				{
					return flags == other.flags;
				}
				return false;
			}

			public override int GetHashCode()
			{
				return ((object)loadedMaterial).GetHashCode() ^ flags.GetHashCode();
			}
		}

		public Entity mesh { get; set; }

		public ushort partition { get; set; }

		public MeshLayer layer { get; set; }

		public MeshType type { get; set; }

		public List<Batch> batches { get; private set; }

		public GroupKey()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			mesh = Entity.Null;
			partition = 0;
			layer = (MeshLayer)0;
			type = (MeshType)0;
			batches = new List<Batch>();
		}

		public void Initialize(Entity sharedMesh, GroupData groupData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			mesh = sharedMesh;
			partition = groupData.m_Partition;
			layer = groupData.m_Layer;
			type = groupData.m_MeshType;
		}

		public void Clear()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			mesh = Entity.Null;
			partition = 0;
			layer = (MeshLayer)0;
			type = (MeshType)0;
			batches.Clear();
		}

		public bool Equals(GroupKey other)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (mesh != other.mesh || partition != other.partition || layer != other.layer || type != other.type || batches.Count != other.batches.Count)
			{
				return false;
			}
			for (int i = 0; i < batches.Count; i++)
			{
				if (!batches[i].Equals(other.batches[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			int num = ((object)mesh/*cast due to .constrained prefix*/).GetHashCode() ^ partition.GetHashCode() ^ layer.GetHashCode() ^ type.GetHashCode();
			for (int i = 0; i < batches.Count; i++)
			{
				num ^= batches[i].GetHashCode();
			}
			return num;
		}
	}

	private struct MeshKey : IEquatable<MeshKey>
	{
		public Hash128 meshGuid { get; set; }

		public MeshFlags flags { get; set; }

		public MeshKey(RenderPrefab meshPrefab, MeshData meshData)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			AssetData geometryAsset;
			if ((geometryAsset = (AssetData)(object)meshPrefab.geometryAsset) != (IAssetData)null)
			{
				meshGuid = Identifier.op_Implicit(geometryAsset.id);
			}
			else
			{
				meshGuid = default(Hash128);
			}
			flags = meshData.m_State & MeshFlags.Base;
		}

		public bool Equals(MeshKey other)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (meshGuid == other.meshGuid)
			{
				return flags == other.flags;
			}
			return false;
		}

		public override int GetHashCode()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((object)meshGuid/*cast due to .constrained prefix*/).GetHashCode() ^ flags.GetHashCode();
		}
	}

	private PrefabSystem m_PrefabSystem;

	private RenderingSystem m_RenderingSystem;

	private BatchMeshSystem m_BatchMeshSystem;

	private BatchManagerSystem m_BatchManagerSystem;

	private TextureStreamingSystem m_TextureStreamingSystem;

	private Dictionary<MaterialKey, Material> m_Materials;

	private Dictionary<GroupKey, Entity> m_Groups;

	private Dictionary<MeshKey, Entity> m_Meshes;

	private List<KeywordData> m_CachedKeywords;

	private List<TextureData> m_CachedTextures;

	private MaterialKey m_CachedMaterialKey;

	private GroupKey m_CachedGroupKey;

	private VTTextureRequester m_VTTextureRequester;

	private JobHandle m_VTRequestDependencies;

	private EntityQuery m_MeshSettingsQuery;

	private bool m_VTRequestsUpdated;

	private int m_TunnelLayer;

	private int m_MovingLayer;

	private int m_PipelineLayer;

	private int m_SubPipelineLayer;

	private int m_WaterwayLayer;

	private int m_OutlineLayer;

	private int m_MarkerLayer;

	private int m_DecalLayerMask;

	private int m_AnimationTexture;

	private int m_UseStack1;

	private int m_ImpostorSize;

	private int m_ImpostorOffset;

	private int m_WorldspaceAlbedo;

	private int m_MaskMap;

	public int materialCount => m_Materials.Count;

	public int groupCount { get; private set; }

	public int batchCount { get; private set; }

	public IReadOnlyDictionary<MaterialKey, Material> materials => m_Materials;

	public VTTextureRequester VTTextureRequester => m_VTTextureRequester;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_BatchMeshSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchMeshSystem>();
		m_BatchManagerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchManagerSystem>();
		m_TextureStreamingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TextureStreamingSystem>();
		m_Materials = new Dictionary<MaterialKey, Material>();
		m_Groups = new Dictionary<GroupKey, Entity>();
		m_Meshes = new Dictionary<MeshKey, Entity>();
		m_CachedKeywords = new List<KeywordData>();
		m_CachedTextures = new List<TextureData>();
		m_VTTextureRequester = new VTTextureRequester(m_TextureStreamingSystem);
		m_MeshSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MeshSettingsData>() });
		m_TunnelLayer = LayerMask.NameToLayer("Tunnel");
		m_MovingLayer = LayerMask.NameToLayer("Moving");
		m_PipelineLayer = LayerMask.NameToLayer("Pipeline");
		m_SubPipelineLayer = LayerMask.NameToLayer("SubPipeline");
		m_WaterwayLayer = LayerMask.NameToLayer("Waterway");
		m_OutlineLayer = LayerMask.NameToLayer("Outline");
		m_MarkerLayer = LayerMask.NameToLayer("Marker");
		m_DecalLayerMask = Shader.PropertyToID("colossal_DecalLayerMask");
		m_AnimationTexture = Shader.PropertyToID("_AnimationTexture");
		m_UseStack1 = Shader.PropertyToID("colossal_UseStack1");
		m_ImpostorSize = Shader.PropertyToID("_ImpostorSize");
		m_ImpostorOffset = Shader.PropertyToID("_ImpostorOffset");
		m_WorldspaceAlbedo = Shader.PropertyToID("_WorldspaceAlbedo");
		m_MaskMap = Shader.PropertyToID("_MaskMap");
	}

	[Preserve]
	protected override void OnDestroy()
	{
		foreach (KeyValuePair<MaterialKey, Material> item in m_Materials)
		{
			CoreUtils.Destroy((Object)(object)item.Value);
		}
		((JobHandle)(ref m_VTRequestDependencies)).Complete();
		m_VTTextureRequester.Dispose();
		base.OnDestroy();
	}

	public void RemoveMesh(Entity oldMesh, Entity newMesh = default(Entity))
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		List<GroupKey> list = new List<GroupKey>();
		List<MeshKey> list2 = new List<MeshKey>();
		foreach (KeyValuePair<GroupKey, Entity> item in m_Groups)
		{
			if (item.Key.mesh == oldMesh || item.Value == oldMesh)
			{
				list.Add(item.Key);
			}
		}
		foreach (KeyValuePair<MeshKey, Entity> item2 in m_Meshes)
		{
			if (item2.Value == oldMesh)
			{
				list2.Add(item2.Key);
			}
		}
		bool flag = false;
		MeshData meshData = default(MeshData);
		if (EntitiesExtensions.TryGetComponent<MeshData>(((ComponentSystemBase)this).EntityManager, newMesh, ref meshData) && m_PrefabSystem.TryGetPrefab<RenderPrefab>(newMesh, out var prefab) && (Object)(object)prefab != (Object)null)
		{
			MeshKey other = new MeshKey(prefab, meshData);
			foreach (MeshKey item3 in list2)
			{
				if (item3.Equals(other))
				{
					m_Meshes[item3] = newMesh;
					flag = true;
				}
				else
				{
					m_Meshes.Remove(item3);
				}
			}
			if (flag)
			{
				m_BatchMeshSystem.ReplaceMesh(oldMesh, newMesh);
			}
		}
		else
		{
			foreach (MeshKey item4 in list2)
			{
				m_Meshes.Remove(item4);
			}
		}
		foreach (GroupKey item5 in list)
		{
			if (m_Groups[item5] == oldMesh)
			{
				m_Groups.Remove(item5);
				groupCount--;
				batchCount -= item5.batches.Count;
			}
		}
	}

	public void ResetSharedMeshes()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> nativeBatchGroups = m_BatchManagerSystem.GetNativeBatchGroups(readOnly: false, out dependencies);
		ManagedBatches<OptionalProperties> managedBatches = m_BatchManagerSystem.GetManagedBatches();
		((JobHandle)(ref dependencies)).Complete();
		int num = nativeBatchGroups.GetGroupCount();
		for (int i = 0; i < num; i++)
		{
			if (!nativeBatchGroups.IsValidGroup(i))
			{
				continue;
			}
			int num2 = nativeBatchGroups.GetBatchCount(i);
			GroupData groupData = nativeBatchGroups.GetGroupData(i);
			GroupKey groupKey = null;
			Entity value = groupData.m_Mesh;
			if (m_PrefabSystem.TryGetPrefab<RenderPrefab>(groupData.m_Mesh, out var prefab) && (Object)(object)prefab != (Object)null)
			{
				try
				{
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					MeshData componentData = ((EntityManager)(ref entityManager)).GetComponentData<MeshData>(groupData.m_Mesh);
					MeshKey key = new MeshKey(prefab, componentData);
					if (!m_Meshes.TryGetValue(key, out value))
					{
						m_Meshes.Add(key, groupData.m_Mesh);
						value = groupData.m_Mesh;
					}
					if (m_CachedGroupKey != null)
					{
						groupKey = m_CachedGroupKey;
						m_CachedGroupKey = null;
					}
					else
					{
						groupKey = new GroupKey();
					}
					groupKey.Initialize(value, groupData);
				}
				catch (Exception ex)
				{
					COSystemBase.baseLog.ErrorFormat((Object)(object)prefab, ex, "Error when initializing batches for {0}", (object)((Object)prefab).name);
				}
			}
			int num3 = 0;
			while (true)
			{
				if (num3 < num2)
				{
					int managedBatchIndex = nativeBatchGroups.GetManagedBatchIndex(i, num3);
					if (managedBatchIndex >= 0)
					{
						groupKey?.batches.Add(new GroupKey.Batch((CustomBatch)(object)managedBatches.GetBatch(managedBatchIndex)));
						num3++;
						continue;
					}
				}
				else if (groupKey != null && m_Groups.TryAdd(groupKey, groupData.m_Mesh))
				{
					num++;
					batchCount += num2;
					break;
				}
				if (groupKey != null)
				{
					groupKey.Clear();
					m_CachedGroupKey = groupKey;
				}
				break;
			}
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> nativeBatchGroups = m_BatchManagerSystem.GetNativeBatchGroups(readOnly: false, out dependencies);
		ManagedBatches<OptionalProperties> managedBatches = m_BatchManagerSystem.GetManagedBatches();
		((JobHandle)(ref dependencies)).Complete();
		UpdatedManagedBatchEnumerator updatedManagedBatches = nativeBatchGroups.GetUpdatedManagedBatches();
		int num = default(int);
		while (((UpdatedManagedBatchEnumerator)(ref updatedManagedBatches)).GetNextUpdatedGroup(ref num))
		{
			int num2 = nativeBatchGroups.GetBatchCount(num);
			GroupData groupData = nativeBatchGroups.GetGroupData(num);
			GroupKey groupKey = null;
			Entity value = groupData.m_Mesh;
			if (m_PrefabSystem.TryGetPrefab<RenderPrefab>(groupData.m_Mesh, out var prefab) && (Object)(object)prefab != (Object)null)
			{
				try
				{
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					MeshData componentData = ((EntityManager)(ref entityManager)).GetComponentData<MeshData>(groupData.m_Mesh);
					MeshKey key = new MeshKey(prefab, componentData);
					if (!m_Meshes.TryGetValue(key, out value))
					{
						m_Meshes.Add(key, groupData.m_Mesh);
						value = groupData.m_Mesh;
					}
					if (m_CachedGroupKey != null)
					{
						groupKey = m_CachedGroupKey;
						m_CachedGroupKey = null;
					}
					else
					{
						groupKey = new GroupKey();
					}
					groupKey.Initialize(value, groupData);
				}
				catch (Exception ex)
				{
					COSystemBase.baseLog.ErrorFormat((Object)(object)prefab, ex, "Error when initializing batches for {0}", (object)((Object)prefab).name);
				}
			}
			for (int i = 0; i < num2; i++)
			{
				int num3 = nativeBatchGroups.GetManagedBatchIndex(num, i);
				if (num3 < 0)
				{
					try
					{
						BatchData batchData = nativeBatchGroups.GetBatchData(num, i);
						PropertyData lodFadeData;
						CustomBatch customBatch = CreateBatch(num, i, value, ref groupData, ref batchData, out lodFadeData);
						nativeBatchGroups.SetBatchData(num, i, batchData);
						BatchFlags batchFlags = customBatch.sourceFlags;
						if (!m_BatchManagerSystem.IsMotionVectorsEnabled())
						{
							batchFlags &= ~BatchFlags.MotionVectors;
						}
						if (!m_BatchManagerSystem.IsLodFadeEnabled())
						{
							batchFlags &= ~BatchFlags.LodFade;
						}
						OptionalProperties optionalProperties = new OptionalProperties(batchFlags, customBatch.sourceType);
						num3 = managedBatches.AddBatch<CullingData, GroupData, BatchData, InstanceData>((ManagedBatch)(object)customBatch, i, nativeBatchGroups);
						m_BatchMeshSystem.AddBatch(customBatch, num3);
						NativeBatchProperties batchProperties = managedBatches.GetBatchProperties(((ManagedBatch)customBatch).material.shader, optionalProperties);
						nativeBatchGroups.SetBatchProperties(num, i, batchProperties);
						if (lodFadeData.m_DataIndex >= 0)
						{
							nativeBatchGroups.SetBatchDataIndex(num, i, lodFadeData.m_NameID, lodFadeData.m_DataIndex);
						}
						WriteableBatchDefaultsAccessor batchDefaultsAccessor = nativeBatchGroups.GetBatchDefaultsAccessor(num, i);
						if ((AssetData)(object)customBatch.sourceSurface != (IAssetData)null)
						{
							managedBatches.SetDefaults(GetTemplate(customBatch.sourceSurface), customBatch.sourceSurface.floats, customBatch.sourceSurface.ints, customBatch.sourceSurface.vectors, customBatch.sourceSurface.colors, ((ManagedBatch)customBatch).customProps, batchProperties, batchDefaultsAccessor);
						}
						else
						{
							managedBatches.SetDefaults(customBatch.sourceMaterial, ((ManagedBatch)customBatch).customProps, batchProperties, batchDefaultsAccessor);
						}
					}
					catch (Exception ex2)
					{
						if ((Object)(object)prefab != (Object)null)
						{
							COSystemBase.baseLog.ErrorFormat((Object)(object)prefab, ex2, "Error when initializing batch {0} for {1}", (object)i, (object)((Object)prefab).name);
						}
						else
						{
							COSystemBase.baseLog.ErrorFormat(ex2, "Error when initializing batch {0} for {1}", (object)i, (object)groupData.m_Mesh);
						}
						continue;
					}
				}
				groupKey?.batches.Add(new GroupKey.Batch((CustomBatch)(object)managedBatches.GetBatch(num3)));
			}
			nativeBatchGroups.SetGroupData(num, groupData);
			if (groupKey != null)
			{
				if (m_Groups.TryGetValue(groupKey, out var value2))
				{
					m_BatchManagerSystem.MergeGroups(value2, num);
					groupKey.Clear();
					m_CachedGroupKey = groupKey;
				}
				else
				{
					m_Groups.Add(groupKey, groupData.m_Mesh);
					groupCount++;
					batchCount += num2;
				}
			}
			else
			{
				groupCount++;
				batchCount += num2;
			}
		}
		nativeBatchGroups.ClearUpdatedManagedBatches();
	}

	public void EnabledShadersUpdated()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> nativeBatchGroups = m_BatchManagerSystem.GetNativeBatchGroups(readOnly: false, out dependencies);
		ManagedBatches<OptionalProperties> managedBatches = m_BatchManagerSystem.GetManagedBatches();
		((JobHandle)(ref dependencies)).Complete();
		int num = nativeBatchGroups.GetGroupCount();
		for (int i = 0; i < num; i++)
		{
			if (!nativeBatchGroups.IsValidGroup(i))
			{
				continue;
			}
			int num2 = nativeBatchGroups.GetBatchCount(i);
			GroupData groupData = nativeBatchGroups.GetGroupData(i);
			groupData.m_RenderFlags &= ~BatchRenderFlags.IsEnabled;
			for (int j = 0; j < num2; j++)
			{
				int managedBatchIndex = nativeBatchGroups.GetManagedBatchIndex(i, j);
				if (managedBatchIndex >= 0)
				{
					CustomBatch customBatch = (CustomBatch)(object)managedBatches.GetBatch(managedBatchIndex);
					BatchData batchData = nativeBatchGroups.GetBatchData(i, j);
					if (m_RenderingSystem.IsShaderEnabled(customBatch.loadedMaterial.shader))
					{
						groupData.m_RenderFlags |= BatchRenderFlags.IsEnabled;
						batchData.m_RenderFlags |= BatchRenderFlags.IsEnabled;
					}
					else
					{
						batchData.m_RenderFlags &= ~BatchRenderFlags.IsEnabled;
					}
					nativeBatchGroups.SetBatchData(i, j, batchData);
				}
			}
			nativeBatchGroups.SetGroupData(i, groupData);
		}
	}

	public JobHandle GetVTRequestMaxPixels(out NativeList<float> maxPixels0, out NativeList<float> maxPixels1)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		maxPixels0 = m_VTTextureRequester.TexturesMaxPixels[0];
		maxPixels1 = m_VTTextureRequester.TexturesMaxPixels[1];
		return m_VTRequestDependencies;
	}

	public void AddVTRequestWriter(JobHandle dependencies)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_VTRequestDependencies = dependencies;
		m_VTRequestsUpdated = true;
	}

	public void ResetVT(int desiredMipBias, FilterMode filterMode)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (m_TextureStreamingSystem.ShouldResetVT(desiredMipBias, filterMode))
		{
			((JobHandle)(ref m_VTRequestDependencies)).Complete();
			m_VTTextureRequester.Clear();
			m_TextureStreamingSystem.Initialize(desiredMipBias, filterMode);
			m_BatchManagerSystem.VirtualTexturingUpdated();
		}
	}

	public void ReloadVT()
	{
		((JobHandle)(ref m_VTRequestDependencies)).Complete();
		m_VTTextureRequester.Clear();
		m_TextureStreamingSystem.Reload();
		m_BatchManagerSystem.VirtualTexturingUpdated();
	}

	public void CompleteVTRequests()
	{
		if (m_VTRequestsUpdated)
		{
			((JobHandle)(ref m_VTRequestDependencies)).Complete();
			m_VTTextureRequester.UpdateTexturesVTRequests();
			m_VTRequestsUpdated = false;
		}
		m_TextureStreamingSystem.UpdateWorkingSetMipBias();
	}

	private CustomBatch CreateBatch(int groupIndex, int batchIndex, Entity sharedMesh, ref GroupData groupData, ref BatchData batchData, out PropertyData lodFadeData)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_115e: Unknown result type (might be due to invalid IL or missing references)
		//IL_112d: Unknown result type (might be due to invalid IL or missing references)
		//IL_116e: Unknown result type (might be due to invalid IL or missing references)
		//IL_113d: Unknown result type (might be due to invalid IL or missing references)
		//IL_111d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1192: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Expected O, but got Unknown
		//IL_0744: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_098c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0991: Unknown result type (might be due to invalid IL or missing references)
		//IL_096f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0976: Expected O, but got Unknown
		//IL_0925: Unknown result type (might be due to invalid IL or missing references)
		//IL_092a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0901: Unknown result type (might be due to invalid IL or missing references)
		//IL_0908: Expected O, but got Unknown
		//IL_0a08: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a00: Expected O, but got Unknown
		//IL_0b31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1e: Expected O, but got Unknown
		//IL_0a34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3b: Expected O, but got Unknown
		//IL_0b99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b73: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3f: Expected O, but got Unknown
		//IL_0cd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ceb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dde: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ecf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f80: Expected O, but got Unknown
		SurfaceAsset val = null;
		Material val2 = null;
		Material value = null;
		Material value2 = null;
		Mesh val3 = null;
		MaterialPropertyBlock val4 = null;
		Entity val5 = Entity.Null;
		ShadowCastingMode val6 = (ShadowCastingMode)1;
		bool flag = true;
		BatchFlags batchFlags = (BatchFlags)0;
		GeneratedType generatedType = GeneratedType.None;
		int num = 0;
		int num2 = batchData.m_SubMeshIndex;
		MaterialKey materialKey;
		if (m_CachedMaterialKey != null)
		{
			materialKey = m_CachedMaterialKey;
			m_CachedMaterialKey = null;
		}
		else
		{
			materialKey = new MaterialKey();
		}
		lodFadeData = new PropertyData
		{
			m_DataIndex = -1
		};
		m_CachedKeywords.Clear();
		m_CachedTextures.Clear();
		Entity val7;
		if (batchData.m_LodMesh != Entity.Null)
		{
			val5 = batchData.m_LodMesh;
			val7 = groupData.m_Mesh;
		}
		else
		{
			val5 = groupData.m_Mesh;
			val7 = Entity.Null;
		}
		if (batchData.m_LodIndex > 0)
		{
			batchFlags |= BatchFlags.Lod;
		}
		EntityManager entityManager;
		if (groupData.m_MeshType == MeshType.Zone)
		{
			val2 = m_PrefabSystem.GetPrefab<ZoneBlockPrefab>(val5).m_Material;
			materialKey.Initialize(val2);
			if (groupData.m_Partition >= 1)
			{
				batchFlags |= BatchFlags.Extended1;
			}
			if (groupData.m_Partition >= 2)
			{
				batchFlags |= BatchFlags.Extended2;
			}
			if (groupData.m_Partition >= 3)
			{
				batchFlags |= BatchFlags.Extended3;
			}
			val6 = (ShadowCastingMode)0;
			flag = false;
		}
		else if (groupData.m_MeshType == MeshType.Net)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			NetCompositionMeshData componentData = ((EntityManager)(ref entityManager)).GetComponentData<NetCompositionMeshData>(val5);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<MeshMaterial> buffer = ((EntityManager)(ref entityManager)).GetBuffer<MeshMaterial>(val5, true);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<NetCompositionPiece> buffer2 = ((EntityManager)(ref entityManager)).GetBuffer<NetCompositionPiece>(val5, true);
			int materialIndex = buffer[num2].m_MaterialIndex;
			DynamicBuffer<MeshMaterial> val8 = default(DynamicBuffer<MeshMaterial>);
			for (int i = 0; i < buffer2.Length; i++)
			{
				NetCompositionPiece netCompositionPiece = buffer2[i];
				if (!EntitiesExtensions.TryGetBuffer<MeshMaterial>(((ComponentSystemBase)this).EntityManager, netCompositionPiece.m_Piece, true, ref val8))
				{
					continue;
				}
				int num3 = 0;
				while (num3 < val8.Length)
				{
					if (val8[num3].m_MaterialIndex != materialIndex)
					{
						num3++;
						continue;
					}
					goto IL_01c9;
				}
				continue;
				IL_01c9:
				val = m_PrefabSystem.GetPrefab<RenderPrefab>(netCompositionPiece.m_Piece).GetSurfaceAsset(num3);
				val.LoadProperties(true);
				materialKey.Initialize(val);
				break;
			}
			materialKey.decalLayerMask = 2;
			lodFadeData = m_BatchManagerSystem.GetPropertyData(((batchData.m_LodIndex & 1) == 0) ? NetProperty.LodFade0 : NetProperty.LodFade1);
			batchFlags |= BatchFlags.InfoviewColor | BatchFlags.LodFade;
			generatedType = GeneratedType.NetComposition;
			sharedMesh = val5;
			if ((componentData.m_Flags.m_General & CompositionFlags.General.Node) != 0)
			{
				batchFlags |= BatchFlags.Node;
			}
			if ((componentData.m_Flags.m_General & CompositionFlags.General.Roundabout) != 0)
			{
				batchFlags |= BatchFlags.Roundabout;
			}
			if ((componentData.m_State & MeshFlags.Default) != 0)
			{
				materialKey.textures.Clear();
				materialKey.textures.Add(m_WorldspaceAlbedo, Texture2D.grayTexture);
			}
			batchData.m_ShadowArea = float.PositiveInfinity;
			batchData.m_ShadowHeight = 1f;
			if ((componentData.m_Flags.m_General & CompositionFlags.General.Elevated) != 0 || (componentData.m_Flags.m_Left & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered | CompositionFlags.Side.SoundBarrier)) != 0 || (componentData.m_Flags.m_Right & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered | CompositionFlags.Side.SoundBarrier)) != 0 || (componentData.m_State & MeshFlags.Default) != 0)
			{
				batchData.m_ShadowHeight = componentData.m_Width;
			}
		}
		else
		{
			RenderPrefab renderPrefab = m_PrefabSystem.GetPrefab<RenderPrefab>(val5);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			MeshData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<MeshData>(val5);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			SharedMeshData componentData3 = ((EntityManager)(ref entityManager)).GetComponentData<SharedMeshData>(val5);
			RenderPrefab renderPrefab2 = null;
			if (val7 != Entity.Null)
			{
				renderPrefab2 = m_PrefabSystem.GetPrefab<RenderPrefab>(val7);
			}
			if (batchData.m_LodIndex > 0)
			{
				MeshKey key = new MeshKey(renderPrefab, componentData2);
				if (!m_Meshes.TryGetValue(key, out sharedMesh))
				{
					m_Meshes.Add(key, val5);
					sharedMesh = val5;
				}
			}
			componentData3.m_Mesh = sharedMesh;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).SetComponentData<SharedMeshData>(val5, componentData3);
			DecalProperties decalProperties = renderPrefab.GetComponent<DecalProperties>();
			AnimationProperties component = renderPrefab.GetComponent<AnimationProperties>();
			ProceduralAnimationProperties component2 = renderPrefab.GetComponent<ProceduralAnimationProperties>();
			EmissiveProperties component3 = renderPrefab.GetComponent<EmissiveProperties>();
			ColorProperties component4 = renderPrefab.GetComponent<ColorProperties>();
			CurveProperties component5 = renderPrefab.GetComponent<CurveProperties>();
			DilationProperties component6 = renderPrefab.GetComponent<DilationProperties>();
			OverlayProperties component7 = renderPrefab.GetComponent<OverlayProperties>();
			BaseProperties component8 = renderPrefab.GetComponent<BaseProperties>();
			if ((Object)(object)component3 == (Object)null && (Object)(object)renderPrefab2 != (Object)null)
			{
				component3 = renderPrefab2.GetComponent<EmissiveProperties>();
			}
			if ((Object)(object)component4 == (Object)null && (Object)(object)renderPrefab2 != (Object)null)
			{
				component4 = renderPrefab2.GetComponent<ColorProperties>();
			}
			if ((Object)(object)component8 == (Object)null && (Object)(object)renderPrefab2 != (Object)null)
			{
				component8 = renderPrefab2.GetComponent<BaseProperties>();
			}
			if ((Object)(object)decalProperties != (Object)null && groupData.m_Layer == MeshLayer.Outline)
			{
				MeshSettingsData singleton = ((EntityQuery)(ref m_MeshSettingsQuery)).GetSingleton<MeshSettingsData>();
				renderPrefab = m_PrefabSystem.GetPrefab<RenderPrefab>(singleton.m_MissingObjectMesh);
				num2 = 0;
				val = renderPrefab.GetSurfaceAsset(num2);
				decalProperties = null;
			}
			else if ((componentData2.m_State & MeshFlags.Base) != 0 && num2 == componentData2.m_SubMeshCount)
			{
				if ((Object)(object)component8 != (Object)null)
				{
					renderPrefab = component8.m_BaseType;
				}
				else
				{
					MeshSettingsData singleton2 = ((EntityQuery)(ref m_MeshSettingsQuery)).GetSingleton<MeshSettingsData>();
					renderPrefab = m_PrefabSystem.GetPrefab<RenderPrefab>(singleton2.m_DefaultBaseMesh);
				}
				num2 = 0;
				val = renderPrefab.GetSurfaceAsset(num2);
				generatedType = GeneratedType.ObjectBase;
				batchFlags |= BatchFlags.Base;
			}
			else
			{
				val = renderPrefab.GetSurfaceAsset(num2);
			}
			if (groupData.m_MeshType == MeshType.Object)
			{
				lodFadeData = m_BatchManagerSystem.GetPropertyData(((batchData.m_LodIndex & 1) == 0) ? ObjectProperty.LodFade0 : ObjectProperty.LodFade1);
			}
			else if (groupData.m_MeshType == MeshType.Lane)
			{
				lodFadeData = m_BatchManagerSystem.GetPropertyData(((batchData.m_LodIndex & 1) == 0) ? LaneProperty.LodFade0 : LaneProperty.LodFade1);
			}
			val.LoadProperties(true);
			materialKey.Initialize(val);
			Bounds3 val9 = RenderingUtils.SafeBounds(componentData2.m_Bounds);
			float3 val10 = MathUtils.Center(val9);
			float3 val11 = MathUtils.Size(val9);
			float4 val12 = default(float4);
			((float4)(ref val12))._002Ector(val11, val10.y);
			batchData.m_ShadowHeight = val11.y;
			batchData.m_ShadowArea = math.sqrt(val11.x * val11.x + val11.z * val11.z) * batchData.m_ShadowHeight;
			VTAtlassingInfo[] array = val.VTAtlassingInfos;
			if (array == null)
			{
				array = val.PreReservedAtlassingInfos;
			}
			int lod = componentData2.m_MinLod;
			if (groupData.m_MeshType == MeshType.Lane)
			{
				lod = groupData.m_Partition;
			}
			PropertyData propertyData = m_BatchManagerSystem.GetPropertyData(MaterialProperty.TextureArea);
			PropertyData propertyData2 = m_BatchManagerSystem.GetPropertyData(MaterialProperty.MeshSize);
			PropertyData propertyData3 = m_BatchManagerSystem.GetPropertyData(MaterialProperty.LodDistanceFactor);
			PropertyData propertyData4 = m_BatchManagerSystem.GetPropertyData(MaterialProperty.SingleLightsOffset);
			PropertyData propertyData5 = m_BatchManagerSystem.GetPropertyData(MaterialProperty.DilationParams);
			if ((Object)(object)decalProperties != (Object)null)
			{
				materialKey.renderQueue = materialKey.template.shader.renderQueue + decalProperties.m_RendererPriority;
				if (val4 == null)
				{
					val4 = new MaterialPropertyBlock();
				}
				val4.SetVector(propertyData.m_NameID, float4.op_Implicit(new float4(decalProperties.m_TextureArea.min, decalProperties.m_TextureArea.max)));
				val4.SetVector(propertyData2.m_NameID, float4.op_Implicit(val12));
				val4.SetFloat(propertyData3.m_NameID, RenderingUtils.CalculateDistanceFactor(lod));
				materialKey.decalLayerMask = (int)decalProperties.m_LayerMask;
				if (array != null)
				{
					Bounds2 bounds = MathUtils.Bounds(decalProperties.m_TextureArea.min, decalProperties.m_TextureArea.max);
					((JobHandle)(ref m_VTRequestDependencies)).Complete();
					if (array.Length >= 1 && array[0].indexInStack >= 0)
					{
						batchData.m_VTIndex0 = m_VTTextureRequester.RegisterTexture(0, array[0].stackGlobalIndex, array[0].indexInStack, bounds);
					}
					if (array.Length >= 2 && array[1].indexInStack >= 0)
					{
						batchData.m_VTIndex1 = m_VTTextureRequester.RegisterTexture(1, array[1].stackGlobalIndex, array[1].indexInStack, bounds);
					}
					batchData.m_VTSizeFactor = math.cmax(val11);
				}
				if (groupData.m_MeshType == MeshType.Lane)
				{
					val4.SetFloat(m_BatchManagerSystem.GetPropertyData(MaterialProperty.SmoothingDistance).m_NameID, componentData2.m_SmoothingDistance);
				}
				if (decalProperties.m_EnableInfoviewColor)
				{
					batchFlags |= BatchFlags.InfoviewColor;
				}
			}
			else
			{
				DecalLayers decalLayerMask = ((componentData2.m_DecalLayer != 0) ? componentData2.m_DecalLayer : DecalLayers.Other);
				materialKey.decalLayerMask = (int)decalLayerMask;
				batchFlags |= BatchFlags.InfoviewColor | BatchFlags.LodFade | BatchFlags.SurfaceState;
			}
			bool flag2 = groupData.m_MeshType == MeshType.Object;
			bool flag3 = groupData.m_MeshType == MeshType.Lane;
			if ((Object)(object)component != (Object)null && component.m_Clips != null && component.m_Clips.Length != 0)
			{
				AnimationProperties.BakedAnimationClip bakedAnimationClip = component.m_Clips[0];
				SetTexture(materialKey, m_AnimationTexture, (Texture)(object)bakedAnimationClip.animationTexture);
				DisableKeyword(materialKey, "_GPU_ANIMATION_TEXTURE");
				batchFlags |= BatchFlags.MotionVectors | BatchFlags.Animated;
			}
			if ((Object)(object)component2 != (Object)null && component2.m_Bones != null && component2.m_Bones.Length != 0)
			{
				PropertyData propertyData6 = m_BatchManagerSystem.GetPropertyData(ObjectProperty.BoneParameters);
				if (val4 == null)
				{
					val4 = new MaterialPropertyBlock();
				}
				val4.SetVector(propertyData6.m_NameID, Vector4.op_Implicit(new Vector2(math.asfloat(0), math.asfloat(component2.m_Bones.Length))));
				EnableKeyword(materialKey, "_GPU_ANIMATION_PROCEDURAL");
				batchFlags |= BatchFlags.MotionVectors | BatchFlags.Bones;
				flag2 = false;
			}
			if ((Object)(object)component3 != (Object)null && component3.hasAnyLights)
			{
				PropertyData propertyData7 = m_BatchManagerSystem.GetPropertyData(ObjectProperty.LightParameters);
				if (val4 == null)
				{
					val4 = new MaterialPropertyBlock();
				}
				val4.SetVector(propertyData7.m_NameID, Vector4.op_Implicit(new Vector2(0f, (float)component3.lightsCount)));
				val4.SetFloat(propertyData4.m_NameID, (float)component3.GetSingleLightOffset(batchData.m_SubMeshIndex));
				batchFlags |= BatchFlags.Emissive;
			}
			if ((Object)(object)component4 != (Object)null && component4.m_ColorVariations != null && component4.m_ColorVariations.Count != 0)
			{
				batchFlags |= BatchFlags.ColorMask;
			}
			if ((componentData2.m_State & MeshFlags.Character) != 0)
			{
				if (val4 == null)
				{
					val4 = new MaterialPropertyBlock();
				}
				m_BatchMeshSystem.SetShapeParameters(val4, sharedMesh, num2);
				batchFlags |= BatchFlags.Bones | BatchFlags.BlendWeights;
			}
			if ((Object)(object)component5 != (Object)null)
			{
				if (component5.m_GeometryTiling)
				{
					if (val4 == null)
					{
						val4 = new MaterialPropertyBlock();
					}
					float4 val13 = default(float4);
					((float4)(ref val13))._002Ector(0.1f, 10f, 0f, 0f);
					if (componentData2.m_TilingCount != 0 && component5.m_StraightTiling)
					{
						val13.x = 1f / (float)componentData2.m_TilingCount;
						val13.y = 0.01f;
					}
					val4.SetVector(propertyData5.m_NameID, float4.op_Implicit(val13));
					EnableKeyword(materialKey, "COLOSSAL_GEOMETRY_TILING");
					flag3 = false;
				}
				if (component5.m_SubFlow)
				{
					batchFlags |= BatchFlags.InfoviewFlow;
				}
				if (component5.m_HangingSwaying)
				{
					batchFlags |= BatchFlags.Hanging;
					float num4 = batchData.m_ShadowArea / batchData.m_ShadowHeight;
					batchData.m_ShadowHeight += 0.5f;
					batchData.m_ShadowArea = num4 * batchData.m_ShadowHeight;
				}
			}
			if ((Object)(object)component6 != (Object)null)
			{
				if (val4 == null)
				{
					val4 = new MaterialPropertyBlock();
				}
				float4 val14 = default(float4);
				((float4)(ref val14))._002Ector(component6.m_MinSize, 1f / math.max(1E-05f, math.max(val12.x, val12.y)), component6.m_InfoviewFactor * 2.5f, 2.5f);
				if (component6.m_InfoviewOnly)
				{
					val14.x = math.max(val12.x, val12.y);
					val14.w = 0f;
				}
				val4.SetVector(propertyData5.m_NameID, float4.op_Implicit(val14));
				val4.SetFloat(propertyData3.m_NameID, RenderingUtils.CalculateDistanceFactor(lod));
				EnableKeyword(materialKey, "COLOSSAL_GEOMETRY_DILATED");
				flag3 = false;
				if (val.keywords.Contains("_SURFACE_TYPE_TRANSPARENT"))
				{
					batchFlags &= ~BatchFlags.LodFade;
				}
			}
			if (flag2)
			{
				EnableKeyword(materialKey, "_GPU_ANIMATION_OFF");
			}
			if (flag3)
			{
				EnableKeyword(materialKey, "COLOSSAL_GEOMETRY_DEFAULT");
			}
			if ((Object)(object)component7 != (Object)null)
			{
				if (materialKey.template.renderQueue == 3000)
				{
					materialKey.renderQueue = 3900;
				}
				if (val4 == null)
				{
					val4 = new MaterialPropertyBlock();
				}
				val4.SetVector(propertyData.m_NameID, float4.op_Implicit(new float4(component7.m_TextureArea.min, component7.m_TextureArea.max)));
				val4.SetVector(propertyData2.m_NameID, float4.op_Implicit(val12));
				val4.SetFloat(propertyData3.m_NameID, RenderingUtils.CalculateDistanceFactor(lod));
				batchFlags &= ~BatchFlags.SurfaceState;
			}
			else if ((Object)(object)decalProperties == (Object)null && materialKey.template.renderQueue >= 2000 && materialKey.template.renderQueue <= 2500 && math.any(MathUtils.Size(val9) < new float3(7f, 3f, 7f)))
			{
				materialKey.renderQueue = materialKey.template.renderQueue + 1;
			}
			if ((Object)(object)decalProperties != (Object)null || (Object)(object)component7 != (Object)null)
			{
				val6 = (ShadowCastingMode)0;
				flag = false;
			}
			if (renderPrefab.isImpostor)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				ImpostorData componentData4 = ((EntityManager)(ref entityManager)).GetComponentData<ImpostorData>(val5);
				val.vectors.TryGetValue("_ImpostorOffset", out var value3);
				float4 val15 = float4.op_Implicit(value3);
				componentData4.m_Offset = ((float4)(ref val15)).xyz;
				val.floats.TryGetValue("_ImpostorSize", out componentData4.m_Size);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<ImpostorData>(val5, componentData4);
				if (batchData.m_LodIndex == groupData.m_LodCount)
				{
					groupData.m_SecondarySize = val11 * val11 / (componentData4.m_Size * math.cmax(val11));
					groupData.m_SecondaryCenter = val10 - componentData4.m_Offset;
				}
			}
			if ((renderPrefab.manualVTRequired || renderPrefab.isImpostor) && (Object)(object)decalProperties == (Object)null && array != null)
			{
				Bounds2 bounds2 = MathUtils.Bounds(new float2(0f, 0f), new float2(1f, 1f));
				((JobHandle)(ref m_VTRequestDependencies)).Complete();
				if (array.Length >= 1 && array[0].indexInStack >= 0)
				{
					batchData.m_VTIndex0 = m_VTTextureRequester.RegisterTexture(0, array[0].stackGlobalIndex, array[0].indexInStack, bounds2);
				}
				if (array.Length >= 2 && array[1].indexInStack >= 0)
				{
					batchData.m_VTIndex1 = m_VTTextureRequester.RegisterTexture(1, array[1].stackGlobalIndex, array[1].indexInStack, bounds2);
				}
				batchData.m_VTSizeFactor = math.cmax(val11) * 2f;
			}
			if ((componentData2.m_State & MeshFlags.Default) != 0)
			{
				batchData.m_ShadowArea = float.PositiveInfinity;
				batchData.m_ShadowHeight = float.PositiveInfinity;
				DisableKeyword(materialKey, "_TANGENTSPACE_OCTO");
				materialKey.textures.Add(m_MaskMap, Texture2D.blackTexture);
			}
			if (array != null)
			{
				if ((componentData2.m_State & MeshFlags.Default) != 0)
				{
					DisableKeyword(materialKey, "ENABLE_VT");
				}
				else
				{
					for (int j = 0; j < 2; j++)
					{
						if (array.Length > j && array[j].indexInStack >= 0)
						{
							if (val4 == null)
							{
								val4 = new MaterialPropertyBlock();
							}
							val4.SetTextureParamBlock(m_BatchManagerSystem.GetVTTextureParamBlockID(j), m_TextureStreamingSystem.GetTextureParamBlock(array[j]));
							materialKey.vtStacks.Add(array[j].stackGlobalIndex);
							EnableKeyword(materialKey, "ENABLE_VT");
						}
						else
						{
							materialKey.vtStacks.Add(-1);
						}
					}
				}
			}
		}
		val3 = m_BatchMeshSystem.GetDefaultMesh(groupData.m_MeshType, batchFlags, generatedType);
		if ((batchFlags & BatchFlags.Animated) != 0)
		{
			if (!m_Materials.TryGetValue(materialKey, out value))
			{
				value = CreateMaterial(val, val2, materialKey);
				m_Materials.Add(materialKey, value);
				materialKey = new MaterialKey(materialKey);
			}
			DisableKeyword(materialKey, "_GPU_ANIMATION_OFF");
			EnableKeyword(materialKey, "_GPU_ANIMATION_TEXTURE");
		}
		if (m_Materials.TryGetValue(materialKey, out value2))
		{
			materialKey.Clear();
			m_CachedMaterialKey = materialKey;
		}
		else
		{
			value2 = CreateMaterial(val, val2, materialKey);
			m_Materials.Add(materialKey, value2);
		}
		if (value2.IsKeywordEnabled("_TRANSPARENT_WRITES_MOTION_VEC"))
		{
			batchFlags |= BatchFlags.MotionVectors;
		}
		if ((Object)(object)value == (Object)null)
		{
			value = value2;
		}
		switch (groupData.m_Layer)
		{
		case MeshLayer.Moving:
			num = m_MovingLayer;
			batchFlags |= BatchFlags.MotionVectors;
			break;
		case MeshLayer.Tunnel:
			num = m_TunnelLayer;
			break;
		case MeshLayer.Pipeline:
			num = m_PipelineLayer;
			val6 = (ShadowCastingMode)0;
			flag = false;
			break;
		case MeshLayer.SubPipeline:
			num = m_SubPipelineLayer;
			val6 = (ShadowCastingMode)0;
			flag = false;
			break;
		case MeshLayer.Waterway:
			num = m_WaterwayLayer;
			val6 = (ShadowCastingMode)0;
			flag = false;
			break;
		case MeshLayer.Outline:
			num = m_OutlineLayer;
			batchFlags &= ~(BatchFlags.MotionVectors | BatchFlags.Emissive | BatchFlags.ColorMask | BatchFlags.InfoviewColor | BatchFlags.LodFade | BatchFlags.InfoviewFlow | BatchFlags.SurfaceState);
			batchFlags |= BatchFlags.Outline;
			val6 = (ShadowCastingMode)0;
			flag = false;
			break;
		case MeshLayer.Marker:
			num = m_MarkerLayer;
			val6 = (ShadowCastingMode)0;
			break;
		}
		if ((batchFlags & BatchFlags.MotionVectors) != 0)
		{
			batchData.m_RenderFlags |= BatchRenderFlags.MotionVectors;
		}
		if (flag)
		{
			batchData.m_RenderFlags |= BatchRenderFlags.ReceiveShadows;
		}
		if ((int)val6 != 0)
		{
			batchData.m_RenderFlags |= BatchRenderFlags.CastShadows;
		}
		if (m_RenderingSystem.IsShaderEnabled(value2.shader))
		{
			batchData.m_RenderFlags |= BatchRenderFlags.IsEnabled;
		}
		batchData.m_ShadowCastingMode = (byte)val6;
		batchData.m_Layer = (byte)num;
		groupData.m_RenderFlags |= batchData.m_RenderFlags;
		return new CustomBatch(groupIndex, batchIndex, val, val2, value, value2, val3, val5, sharedMesh, batchFlags, generatedType, groupData.m_MeshType, num2, val4);
	}

	public void SetupVT(RenderPrefab meshPrefab, Material material, int materialIndex)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		SurfaceAsset surfaceAsset = meshPrefab.GetSurfaceAsset(materialIndex);
		VTAtlassingInfo[] array = surfaceAsset.VTAtlassingInfos;
		if (array == null)
		{
			array = surfaceAsset.PreReservedAtlassingInfos;
		}
		if (array == null || meshPrefab.Has<DefaultMesh>())
		{
			return;
		}
		for (int i = 0; i < 2; i++)
		{
			if (array.Length > i && array[i].indexInStack >= 0)
			{
				m_TextureStreamingSystem.BindMaterial(material, array[i].stackGlobalIndex, i, m_TextureStreamingSystem.GetTextureParamBlock(array[i]));
			}
		}
	}

	private Material CreateMaterial(SurfaceAsset sourceSurface, Material sourceMaterial, MaterialKey materialKey)
	{
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Expected O, but got Unknown
		Material val;
		if ((AssetData)(object)sourceSurface != (IAssetData)null)
		{
			val = new Material(materialKey.template)
			{
				name = "Batch (" + ((AssetData)sourceSurface).name + ")",
				hideFlags = (HideFlags)61
			};
			foreach (KeyValuePair<string, float> @float in sourceSurface.floats)
			{
				val.SetFloat(@float.Key, @float.Value);
			}
			foreach (KeyValuePair<string, int> @int in sourceSurface.ints)
			{
				val.SetInt(@int.Key, @int.Value);
			}
			foreach (KeyValuePair<string, Vector4> vector in sourceSurface.vectors)
			{
				val.SetVector(vector.Key, vector.Value);
			}
			foreach (KeyValuePair<string, Color> color in sourceSurface.colors)
			{
				val.SetColor(color.Key, color.Value);
			}
			foreach (KeyValuePair<int, object> texture in materialKey.textures)
			{
				object value = texture.Value;
				TextureAsset val2 = (TextureAsset)((value is TextureAsset) ? value : null);
				if (val2 != null)
				{
					val.SetTexture(texture.Key, val2.Load(-1));
				}
				else
				{
					val.SetTexture(texture.Key, (Texture)texture.Value);
				}
			}
			foreach (string keyword in sourceSurface.keywords)
			{
				val.EnableKeyword(keyword);
			}
			HDMaterial.ValidateMaterial(val);
		}
		else
		{
			val = new Material(sourceMaterial)
			{
				name = "Batch (" + ((Object)sourceMaterial).name + ")",
				hideFlags = (HideFlags)61
			};
			foreach (TextureData item in m_CachedTextures)
			{
				val.SetTexture(item.nameID, item.texture);
			}
		}
		if (materialKey.decalLayerMask != -1)
		{
			val.SetFloat(m_DecalLayerMask, math.asfloat(materialKey.decalLayerMask));
		}
		if (materialKey.renderQueue != -1)
		{
			val.renderQueue = materialKey.renderQueue;
		}
		foreach (KeywordData item2 in m_CachedKeywords)
		{
			if (item2.remove)
			{
				val.DisableKeyword(item2.name);
			}
			else
			{
				val.EnableKeyword(item2.name);
			}
		}
		for (int i = 0; i < materialKey.vtStacks.Count; i++)
		{
			int num = materialKey.vtStacks[i];
			if (num >= 0)
			{
				VTAtlassingInfo[] array = ((sourceSurface != null) ? sourceSurface.VTAtlassingInfos : null);
				VTTextureParamBlock val3 = ((array != null) ? m_TextureStreamingSystem.GetTextureParamBlock(array[i]) : VTTextureParamBlock.Identity);
				m_TextureStreamingSystem.BindMaterial(val, num, i, val3);
			}
		}
		return val;
	}

	private void EnableKeyword(MaterialKey materialKey, string keyword)
	{
		if (materialKey.keywords.Add(keyword))
		{
			m_CachedKeywords.Add(new KeywordData(keyword, remove: false));
		}
	}

	private void DisableKeyword(MaterialKey materialKey, string keyword)
	{
		if (materialKey.keywords.Remove(keyword))
		{
			m_CachedKeywords.Add(new KeywordData(keyword, remove: true));
		}
	}

	private void SetTexture(MaterialKey materialKey, int nameID, Texture texture)
	{
		if (materialKey.textures.TryGetValue(nameID, out var value))
		{
			if (texture != value)
			{
				materialKey.textures[nameID] = texture;
				m_CachedTextures.Add(new TextureData(nameID, texture));
			}
		}
		else
		{
			materialKey.textures.Add(nameID, texture);
			m_CachedTextures.Add(new TextureData(nameID, texture));
		}
	}

	public static Material GetTemplate(SurfaceAsset surfaceAsset)
	{
		Material val = surfaceAsset.GetTemplateMaterial();
		if ((Object)(object)val == (Object)null)
		{
			val = SurfaceAsset.kDefaultMaterial;
		}
		return val;
	}

	[Preserve]
	public ManagedBatchSystem()
	{
	}
}
