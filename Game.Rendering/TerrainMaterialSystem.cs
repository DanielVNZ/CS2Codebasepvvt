using System;
using System.Runtime.CompilerServices;
using Colossal;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.Serialization.Entities;
using Game.Prefabs;
using Game.Serialization;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Scripting;

namespace Game.Rendering;

[FormerlySerializedAs("Colossal.Terrain.TerrainMaterialSystem, Game")]
[CompilerGenerated]
public class TerrainMaterialSystem : GameSystemBase, IDefaultSerializable, ISerializable, IPostDeserialize
{
	public class ShaderID
	{
		public static readonly int _HeightmapArray = Shader.PropertyToID("_HeightMapArray");

		public static readonly int _WaterTexture = Shader.PropertyToID("_WaterTexture");

		public static readonly int _NoiseTex = Shader.PropertyToID("_NoiseTex");

		public static readonly int _HeightMapArrayOffsetScale = Shader.PropertyToID("_HeightMapArrayOffsetScale");

		public static readonly int _WaterTextureOffsetScale = Shader.PropertyToID("_WaterTextureOffsetScale");

		public static readonly int _HeightScaleOffset = Shader.PropertyToID("_HeightScaleOffset");

		public static readonly int _SplatHeightVariance = Shader.PropertyToID("_SplatHeightVariance");

		public static readonly int _SplatRockLimit = Shader.PropertyToID("_SplatRockLimit");

		public static readonly int _SplatGrassLimit = Shader.PropertyToID("_SplatGrassLimit");

		public static readonly int _WorldAdjust = Shader.PropertyToID("_WorldAdjust");

		public static readonly int _NoiseOffset = Shader.PropertyToID("_NoiseOffset");

		public static readonly int _HeightMapArrayIndex = Shader.PropertyToID("_HeightMapArrayIndex");

		public static readonly int _Splatmap = Shader.PropertyToID("_Splatmap");

		public static readonly int _VTScaleOffset = Shader.PropertyToID("_VTScaleOffset");

		public static readonly int _PlayableScaleOffset = Shader.PropertyToID("_PlayableScaleOffset");

		public static readonly int _VTInvBorder = Shader.PropertyToID("_InvVTBorder");

		public static readonly int _BackdropSnowHeightTexture = Shader.PropertyToID("_BackdropSnowHeight");

		public static readonly int _COSplatmap = Shader.PropertyToID("colossal_Splatmap");

		public static readonly int _COWorldSplatmap = Shader.PropertyToID("colossal_WorldSplatmap");

		public static readonly int _COTerrainRockDiffuse = Shader.PropertyToID("colossal_TerrainRockDiffuse");

		public static readonly int _COTerrainDirtDiffuse = Shader.PropertyToID("colossal_TerrainDirtDiffuse");

		public static readonly int _COTerrainGrassDiffuse = Shader.PropertyToID("colossal_TerrainGrassDiffuse");

		public static readonly int _COTerrainRockNormal = Shader.PropertyToID("colossal_TerrainRockNormal");

		public static readonly int _COTerrainDirtNormal = Shader.PropertyToID("colossal_TerrainDirtNormal");

		public static readonly int _COTerrainGrassNormal = Shader.PropertyToID("colossal_TerrainGrassNormal");

		public static readonly int _COTerrainTextureTiling = Shader.PropertyToID("colossal_TerrainTextureTiling");

		public static readonly int _COPlayableScaleOffset = Shader.PropertyToID("colossal_PlayableScaleOffset");

		public static readonly int _COInvVTBorder = Shader.PropertyToID("colossal_InvVTBorder");
	}

	private ILog log = LogManager.GetLogger("TerrainTexturing");

	private static readonly float4 kClearViewport = new float4(0f, 0f, 1f, 1f);

	private const int m_SplatUpdateSize = 128;

	private const int m_SplatRegularUpdateTick = 8;

	private float m_TerrainVTBorder = 1000f;

	private TerrainSystem m_TerrainSystem;

	private WaterRenderSystem m_WaterRenderSystem;

	private PrefabSystem m_PrefabSystem;

	private SnowSystem m_SnowSystem;

	private Material m_SplatMaterial;

	private MaterialPropertyBlock m_Properties = new MaterialPropertyBlock();

	private Mesh m_BlitMesh;

	private RenderTexture m_SplatMap;

	private RenderTexture m_SplatWorldMap;

	private CommandBuffer m_CommandBuffer;

	private Texture2D m_Noise;

	private int m_UpdateIndex;

	private int m_UpdateTick;

	private bool m_ForceUpdateWholeSplatmap;

	private NativeList<Entity> m_MaterialPrefabs;

	private Material splatMaterial
	{
		get
		{
			return m_SplatMaterial;
		}
		set
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			if ((Object)(object)value != (Object)null)
			{
				m_SplatMaterial = new Material(value);
			}
		}
	}

	public Texture splatmap => (Texture)(object)m_SplatMap;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Expected O, but got Unknown
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Expected O, but got Unknown
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterRenderSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterRenderSystem>();
		m_SnowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SnowSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_CommandBuffer = new CommandBuffer();
		m_CommandBuffer.name = "TerrainMaterialSystem";
		Terrain terrain = AssetDatabase.global.resources.terrain;
		splatMaterial = terrain.splatMaterial;
		Shader.SetGlobalTexture(ShaderID._COTerrainRockDiffuse, (Texture)(object)terrain.rockDiffuse);
		Shader.SetGlobalTexture(ShaderID._COTerrainDirtDiffuse, (Texture)(object)terrain.dirtDiffuse);
		Shader.SetGlobalTexture(ShaderID._COTerrainGrassDiffuse, (Texture)(object)terrain.grassDiffuse);
		Shader.SetGlobalTexture(ShaderID._COTerrainRockNormal, (Texture)(object)terrain.rockNormal);
		Shader.SetGlobalTexture(ShaderID._COTerrainDirtNormal, (Texture)(object)terrain.dirtNormal);
		Shader.SetGlobalTexture(ShaderID._COTerrainGrassNormal, (Texture)(object)terrain.grassNormal);
		Shader.SetGlobalVector(ShaderID._COTerrainTextureTiling, new Vector4(terrain.terrainFarTiling, terrain.terrainCloseTiling, terrain.terrainCloseDirtTiling, 1f));
		CreateNoiseTexture();
		m_SplatMap = new RenderTexture(4096, 4096, 0, (GraphicsFormat)6)
		{
			name = "Splatmap",
			hideFlags = (HideFlags)52
		};
		m_SplatMap.Create();
		m_SplatWorldMap = new RenderTexture(1024, 1024, 0, (GraphicsFormat)6)
		{
			name = "SplatmapWorld",
			hideFlags = (HideFlags)52
		};
		m_SplatWorldMap.Create();
		m_SplatMaterial.SetTexture(ShaderID._NoiseTex, (Texture)(object)m_Noise);
		m_BlitMesh = new Mesh();
		m_BlitMesh.vertices = (Vector3[])(object)new Vector3[3]
		{
			new Vector3(-1f, -1f, 0f),
			new Vector3(3f, -1f, 0f),
			new Vector3(-1f, 3f, 0f)
		};
		m_BlitMesh.uv = (Vector2[])(object)new Vector2[3]
		{
			new Vector2(0f, 0f),
			new Vector2(2f, 0f),
			new Vector2(0f, 2f)
		};
		m_BlitMesh.subMeshCount = 1;
		m_BlitMesh.SetTriangles(new int[3] { 0, 2, 1 }, 0);
		m_BlitMesh.UploadMeshData(true);
		m_MaterialPrefabs = new NativeList<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
	}

	public int GetOrAddMaterialIndex(Entity prefab)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_MaterialPrefabs.Length; i++)
		{
			if (m_MaterialPrefabs[i] == prefab)
			{
				return i;
			}
		}
		TerraformingPrefab prefab2 = m_PrefabSystem.GetPrefab<TerraformingPrefab>(prefab);
		Debug.Log((object)("Adding terrain material: " + ((Object)prefab2).name), (Object)(object)prefab2);
		int length = m_MaterialPrefabs.Length;
		m_MaterialPrefabs.Add(ref prefab);
		prefab2.GetComponent<TerrainMaterialProperties>();
		return length;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_MaterialPrefabs);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IReader)reader/*cast due to .constrained prefix*/).Read(m_MaterialPrefabs);
	}

	public void SetDefaults(Context context)
	{
		m_MaterialPrefabs.Clear();
	}

	public void PatchReferences(ref PrefabReferences references)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_MaterialPrefabs.Length; i++)
		{
			m_MaterialPrefabs[i] = references.Check(((ComponentSystemBase)this).EntityManager, m_MaterialPrefabs[i]);
		}
	}

	public void PostDeserialize(Context context)
	{
	}

	public void ForceUpdateWholeSplatmap()
	{
		m_ForceUpdateWholeSplatmap = true;
	}

	[Preserve]
	protected unsafe override void OnUpdate()
	{
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_SplatMaterial == (Object)null || (Object)(object)m_TerrainSystem.GetCascadeTexture() == (Object)null)
		{
			return;
		}
		log.Trace((object)"Updating..");
		ILog obj = log;
		Indent indent = obj.indent;
		obj.indent = Indent.op_Increment(indent);
		ProfilingScope val = default(ProfilingScope);
		((ProfilingScope)(ref val))._002Ector(m_CommandBuffer, ProfilingSampler.Get<ProfileId>(ProfileId.UpdateSplatmap));
		try
		{
			bool flag = false;
			if (m_TerrainSystem.heightMapRenderRequired)
			{
				if (m_TerrainSystem.heightMapSliceUpdated[TerrainSystem.baseLod])
				{
					m_UpdateTick = 0;
					float4 val2 = m_TerrainSystem.heightMapViewport[TerrainSystem.baseLod];
					UpdateSplatmap(m_CommandBuffer, val2, math.all(val2 == kClearViewport));
					flag = true;
				}
			}
			else if (m_ForceUpdateWholeSplatmap)
			{
				UpdateSplatmap(m_CommandBuffer, kClearViewport, bWorldUpdate: true);
				m_ForceUpdateWholeSplatmap = false;
				flag = true;
				m_UpdateTick = 0;
			}
			else if (++m_UpdateTick >= 8)
			{
				m_UpdateTick = 0;
				int num = 32;
				int num2 = m_UpdateIndex % num;
				int num3 = m_UpdateIndex / num;
				if (++m_UpdateIndex >= num * num)
				{
					m_UpdateIndex = 0;
				}
				float4 viewport = default(float4);
				((float4)(ref viewport))._002Ector((float)num2 / (float)num, (float)num3 / (float)num, 1f / (float)num, 1f / (float)num);
				flag = true;
				UpdateSplatmap(m_CommandBuffer, viewport, bWorldUpdate: false);
				float2 val3 = m_TerrainSystem.playableOffset + ((float4)(ref viewport)).xy * m_TerrainSystem.playableArea;
				float2 val4 = ((float4)(ref viewport)).zw * m_TerrainSystem.playableArea;
				foreach (WaterSurface instance in WaterSurface.instances)
				{
					instance.UpdateMinMaxArea(float2.op_Implicit(val3), float2.op_Implicit(val4));
				}
			}
			if (m_TerrainSystem.NewMap)
			{
				m_ForceUpdateWholeSplatmap = true;
				m_TerrainSystem.HandleNewMap();
				foreach (WaterSurface instance2 in WaterSurface.instances)
				{
					instance2.UpdateMinMaxArea(float2.op_Implicit(m_TerrainSystem.worldOffset), float2.op_Implicit(m_TerrainSystem.worldSize));
				}
			}
			if (flag)
			{
				log.Trace((object)"Executing command buffer");
				Graphics.ExecuteCommandBuffer(m_CommandBuffer);
			}
		}
		finally
		{
			((IDisposable)(*(ProfilingScope*)(&val))/*cast due to .constrained prefix*/).Dispose();
		}
		ILog obj2 = log;
		indent = obj2.indent;
		obj2.indent = Indent.op_Decrement(indent);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		CoreUtils.Destroy((Object)(object)m_SplatMap);
		CoreUtils.Destroy((Object)(object)m_SplatWorldMap);
		m_MaterialPrefabs.Dispose();
		CoreUtils.Destroy((Object)(object)m_Noise);
		CoreUtils.Destroy((Object)(object)m_BlitMesh);
	}

	private void UpdateSplatmap(CommandBuffer cmd, float4 viewport, bool bWorldUpdate)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		log.Trace((object)"UpdateSplatmap");
		cmd.Clear();
		float4 val = default(float4);
		((float4)(ref val))._002Ector((m_TerrainSystem.worldOffset - m_TerrainSystem.playableOffset) / m_TerrainSystem.playableArea, m_TerrainSystem.worldSize / m_TerrainSystem.playableArea);
		float4 val2 = default(float4);
		((float4)(ref val2))._002Ector(m_TerrainSystem.heightScaleOffset, 0f, 0f);
		float4 val3 = default(float4);
		((float4)(ref val3))._002Ector(0.2f, 0.4f, -1f, 0f);
		float4 val4 = default(float4);
		((float4)(ref val4))._002Ector(500f, 1500f, 0.75f, 0f);
		float4 val5 = default(float4);
		((float4)(ref val5))._002Ector(20f, 300f, 500f, 0.5f);
		float4 val6 = default(float4);
		((float4)(ref val6))._002Ector(1f, 1f, 0.001f, 0.002f);
		Texture cascadeTexture = m_TerrainSystem.GetCascadeTexture();
		m_Properties.Clear();
		m_Properties.SetTexture(ShaderID._HeightmapArray, cascadeTexture);
		m_Properties.SetTexture(ShaderID._WaterTexture, (Texture)(((object)m_WaterRenderSystem.waterTexture) ?? ((object)Texture2D.blackTexture)));
		m_Properties.SetTexture(ShaderID._NoiseTex, (Texture)(object)m_Noise);
		m_Properties.SetInt(ShaderID._HeightMapArrayIndex, TerrainSystem.baseLod);
		m_Properties.SetVector(ShaderID._HeightScaleOffset, float4.op_Implicit(val2));
		m_Properties.SetVector(ShaderID._HeightMapArrayOffsetScale, float4.op_Implicit(viewport));
		m_Properties.SetVector(ShaderID._VTScaleOffset, new Vector4(1f, 1f, 0f, 0f));
		m_Properties.SetVector(ShaderID._SplatHeightVariance, float4.op_Implicit(val3));
		m_Properties.SetVector(ShaderID._SplatRockLimit, float4.op_Implicit(val4));
		m_Properties.SetVector(ShaderID._SplatGrassLimit, float4.op_Implicit(val5));
		m_Properties.SetVector(ShaderID._NoiseOffset, float4.op_Implicit(val6));
		m_Properties.SetVector(ShaderID._WaterTextureOffsetScale, float4.op_Implicit(viewport));
		m_Properties.SetFloat(ShaderID._WorldAdjust, 1f);
		Rect viewport2 = default(Rect);
		((Rect)(ref viewport2))._002Ector(viewport.x * (float)((Texture)m_SplatMap).width, viewport.y * (float)((Texture)m_SplatMap).height, viewport.z * (float)((Texture)m_SplatMap).width, viewport.w * (float)((Texture)m_SplatMap).height);
		bool flag = viewport.x == 0f && viewport.y == 0f && viewport.z == 1f && viewport.w == 1f;
		cmd.SetRenderTarget(RenderTargetIdentifier.op_Implicit((Texture)(object)m_SplatMap), (RenderBufferLoadAction)(flag ? 2 : 0), (RenderBufferStoreAction)0);
		cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
		cmd.SetViewport(viewport2);
		cmd.DrawMesh(m_BlitMesh, Matrix4x4.identity, m_SplatMaterial, 0, 0, m_Properties);
		if (bWorldUpdate)
		{
			cmd.SetRenderTarget(RenderTargetIdentifier.op_Implicit((Texture)(object)m_SplatWorldMap), (RenderBufferLoadAction)2, (RenderBufferStoreAction)0);
			cmd.SetViewport(new Rect(0f, 0f, (float)((Texture)m_SplatWorldMap).width, (float)((Texture)m_SplatWorldMap).height));
			m_Properties.SetInt(ShaderID._HeightMapArrayIndex, 0);
			m_Properties.SetVector(ShaderID._HeightMapArrayOffsetScale, new Vector4(0f, 0f, 1f, 1f));
			m_Properties.SetVector(ShaderID._WaterTextureOffsetScale, float4.op_Implicit(val));
			((float4)(ref val3)).xy = ((float4)(ref val3)).xy * 0.25f;
			m_Properties.SetVector(ShaderID._SplatHeightVariance, float4.op_Implicit(val3));
			m_Properties.SetFloat(ShaderID._WorldAdjust, 0.7f);
			cmd.DrawMesh(m_BlitMesh, Matrix4x4.identity, m_SplatMaterial, 0, 0, m_Properties);
		}
	}

	public void UpdateMaterial(Material material)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		material.SetTexture(ShaderID._Splatmap, splatmap);
		material.SetTexture(ShaderID._BackdropSnowHeightTexture, (Texture)(object)m_SnowSystem.SnowHeightBackdropTexture);
		float2 val = new float2(m_TerrainVTBorder) / (m_TerrainSystem.playableArea + 2f * m_TerrainVTBorder);
		float4 val2 = default(float4);
		((float4)(ref val2))._002Ector(1f / val, val);
		float4 val3 = default(float4);
		((float4)(ref val3))._002Ector(m_TerrainSystem.playableArea / (m_TerrainSystem.playableArea + 2f * m_TerrainVTBorder), val);
		material.SetVector(ShaderID._VTInvBorder, float4.op_Implicit(val2));
		material.SetVector(ShaderID._PlayableScaleOffset, float4.op_Implicit(val3));
		Shader.SetGlobalTexture(ShaderID._COSplatmap, (Texture)(object)m_SplatMap);
		Shader.SetGlobalTexture(ShaderID._COWorldSplatmap, (Texture)(object)m_SplatWorldMap);
		Shader.SetGlobalVector(ShaderID._COInvVTBorder, float4.op_Implicit(val2));
		Shader.SetGlobalVector(ShaderID._COPlayableScaleOffset, float4.op_Implicit(val3));
	}

	private void CreateNoiseTexture()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		m_Noise = new Texture2D(256, 256, (TextureFormat)63, false);
		byte[] array = new byte[65536];
		float num = 0f;
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 256; j++)
			{
				num = Mathf.PerlinNoise((float)j * (5f / 128f), (float)i * (5f / 128f));
				array[i * 256 + j] = (byte)(255f * num);
			}
		}
		m_Noise.SetPixelData<byte>(array, 0, 0);
		m_Noise.Apply();
	}

	[Preserve]
	public TerrainMaterialSystem()
	{
	}//IL_001c: Unknown result type (might be due to invalid IL or missing references)
	//IL_0026: Expected O, but got Unknown

}
