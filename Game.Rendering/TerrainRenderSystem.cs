using Colossal.IO.AssetDatabase;
using Colossal.Serialization.Entities;
using Game.Prefabs;
using Game.Simulation;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Scripting;

namespace Game.Rendering;

[FormerlySerializedAs("Colossal.Terrain.TerrainRenderSystem, Game")]
public class TerrainRenderSystem : GameSystemBase
{
	public class ShaderID
	{
		public static readonly int _COTerrainTextureArrayLODArea = Shader.PropertyToID("colossal_TerrainTextureArrayLODArea");

		public static readonly int _COTerrainTextureArrayLODRange = Shader.PropertyToID("colossal_TerrainTextureArrayLODRange");

		public static readonly int _COTerrainTextureArrayBaseLod = Shader.PropertyToID("colossal_TerrainTextureArrayBaseLod");

		public static readonly int _COTerrainHeightScaleOffset = Shader.PropertyToID("colossal_TerrainHeightScaleOffset");

		public static readonly int _LODArea = Shader.PropertyToID("_LODArea");

		public static readonly int _LODRange = Shader.PropertyToID("_LODRange");

		public static readonly int _TerrainScaleOffset = Shader.PropertyToID("_TerrainScaleOffset");

		public static readonly int _VTScaleOffset = Shader.PropertyToID("_VTScaleOffset");

		public static readonly int _HeightMap = Shader.PropertyToID("_HeightMap");

		public static readonly int _SplatMap = Shader.PropertyToID("_SplatMap");

		public static readonly int _HeightMapArray = Shader.PropertyToID("_HeightMapArray");

		public static readonly int _BaseColorMap = Shader.PropertyToID("_BaseColorMap");

		public static readonly int _OverlayExtra = Shader.PropertyToID("_OverlayExtra");

		public static readonly int _SnowMap = Shader.PropertyToID("_SnowMap");

		public static readonly int _OverlayArrowMask = Shader.PropertyToID("_OverlayArrowMask");

		public static readonly int _OverlayArrowSource = Shader.PropertyToID("_OverlayArrowSource");

		public static readonly int _OverlayPollutionMask = Shader.PropertyToID("_OverlayPollutionMask");

		public static readonly int _CODecalLayerMask = Shader.PropertyToID("colossal_DecalLayerMask");
	}

	private TerrainSystem m_TerrainSystem;

	private TerrainMaterialSystem m_TerrainMaterialSystem;

	private OverlayInfomodeSystem m_OverlayInfomodeSystem;

	private SnowSystem m_SnowSystem;

	private Material m_CachedMaterial;

	public Texture overrideOverlaymap { get; set; }

	public Texture overlayExtramap { get; set; }

	public float4 overlayArrowMask { get; set; }

	private Material material
	{
		get
		{
			return m_CachedMaterial;
		}
		set
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			if ((Object)(object)m_CachedMaterial != (Object)null)
			{
				Object.DestroyImmediate((Object)(object)m_CachedMaterial);
			}
			m_CachedMaterial = new Material(value);
			m_CachedMaterial.SetFloat(ShaderID._CODecalLayerMask, (float)math.asuint(1));
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		((ComponentSystemBase)this).RequireForUpdate<TerrainPropertiesData>();
		material = AssetDatabase.global.resources.terrain.renderMaterial;
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_TerrainMaterialSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainMaterialSystem>();
		m_OverlayInfomodeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<OverlayInfomodeSystem>();
		m_SnowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SnowSystem>();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		CoreUtils.Destroy((Object)(object)m_CachedMaterial);
	}

	private void UpdateMaterial()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		TerrainSurface validSurface = TerrainSurface.GetValidSurface();
		m_TerrainSystem.GetCascadeInfo(out var _, out var baseLOD, out var areas, out var ranges, out var _);
		Shader.SetGlobalMatrix(ShaderID._COTerrainTextureArrayLODArea, float4x4.op_Implicit(areas));
		Shader.SetGlobalVector(ShaderID._COTerrainTextureArrayLODRange, float4.op_Implicit(ranges));
		Shader.SetGlobalInt(ShaderID._COTerrainTextureArrayBaseLod, baseLOD);
		Shader.SetGlobalVector(ShaderID._COTerrainHeightScaleOffset, float4.op_Implicit(new float4(m_TerrainSystem.heightScaleOffset.x, m_TerrainSystem.heightScaleOffset.y, 0f, 0f)));
		if ((Object)(object)validSurface == (Object)null)
		{
			return;
		}
		Material val = (((Object)(object)material == (Object)null) ? validSurface.material : material);
		if (!((Object)(object)val == (Object)null))
		{
			SetKeywords(val);
			val.SetMatrix(ShaderID._LODArea, float4x4.op_Implicit(areas));
			val.SetVector(ShaderID._LODRange, float4.op_Implicit(ranges));
			val.SetVector(ShaderID._TerrainScaleOffset, float4.op_Implicit(new float4(m_TerrainSystem.heightScaleOffset.x, m_TerrainSystem.heightScaleOffset.y, 0f, 0f)));
			val.SetVector(ShaderID._VTScaleOffset, m_TerrainSystem.VTScaleOffset);
			Texture heightmap = m_TerrainSystem.heightmap;
			Texture val2 = overrideOverlaymap;
			Texture snowDepth = (Texture)(object)m_SnowSystem.SnowDepth;
			Texture cascadeTexture = m_TerrainSystem.GetCascadeTexture();
			Texture splatmap = m_TerrainMaterialSystem.splatmap;
			if ((Object)(object)heightmap != (Object)null)
			{
				val.SetTexture(ShaderID._HeightMap, heightmap);
			}
			if ((Object)(object)splatmap != (Object)null)
			{
				val.SetTexture(ShaderID._SplatMap, splatmap);
			}
			if ((Object)(object)cascadeTexture != (Object)null)
			{
				val.SetTexture(ShaderID._HeightMapArray, cascadeTexture);
			}
			if ((Object)(object)val2 != (Object)null)
			{
				val.SetTexture(ShaderID._BaseColorMap, val2);
			}
			if ((Object)(object)overlayExtramap != (Object)null)
			{
				val.SetTexture(ShaderID._OverlayExtra, overlayExtramap);
			}
			if ((Object)(object)snowDepth != (Object)null)
			{
				val.SetTexture(ShaderID._SnowMap, snowDepth);
			}
			val.SetVector(ShaderID._OverlayArrowMask, float4.op_Implicit(overlayArrowMask));
			m_TerrainMaterialSystem.UpdateMaterial(val);
			validSurface.material = val;
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		UpdateMaterial();
		if (m_TerrainSystem.heightMapRenderRequired)
		{
			m_TerrainSystem.RenderCascades();
		}
	}

	private void SetKeywords(Material materialToUpdate)
	{
		if ((Object)(object)overlayExtramap != (Object)null)
		{
			if ((Object)(object)overrideOverlaymap == (Object)null)
			{
				overrideOverlaymap = (Texture)(object)Texture2D.whiteTexture;
			}
			materialToUpdate.EnableKeyword("OVERRIDE_OVERLAY_EXTRA");
			materialToUpdate.DisableKeyword("OVERRIDE_OVERLAY_SIMPLE");
		}
		else if ((Object)(object)overrideOverlaymap != (Object)null)
		{
			materialToUpdate.DisableKeyword("OVERRIDE_OVERLAY_EXTRA");
			materialToUpdate.EnableKeyword("OVERRIDE_OVERLAY_SIMPLE");
		}
		else
		{
			materialToUpdate.DisableKeyword("OVERRIDE_OVERLAY_EXTRA");
			materialToUpdate.DisableKeyword("OVERRIDE_OVERLAY_SIMPLE");
		}
		if (TerrainSystem.baseLod == 0)
		{
			materialToUpdate.DisableKeyword("_PLAYABLEWORLDSELECT");
		}
		else
		{
			materialToUpdate.EnableKeyword("_PLAYABLEWORLDSELECT");
		}
	}

	public Bounds GetCascadeRegion(int index)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		Bounds result = default(Bounds);
		if (index >= 0 && index < m_TerrainSystem.heightMapSliceArea.Length)
		{
			float3 val = default(float3);
			((float3)(ref val))._002Ector(m_TerrainSystem.heightMapSliceArea[index].x, m_TerrainSystem.heightScaleOffset.x, m_TerrainSystem.heightMapSliceArea[index].y);
			float3 val2 = default(float3);
			((float3)(ref val2))._002Ector(m_TerrainSystem.heightMapSliceArea[index].z, 0f, m_TerrainSystem.heightMapSliceArea[index].w);
			((Bounds)(ref result)).SetMinMax(float3.op_Implicit(val), float3.op_Implicit(val2));
		}
		return result;
	}

	public Bounds GetCascadeViewport(int index)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		Bounds result = default(Bounds);
		if (index >= 0 && index < m_TerrainSystem.heightMapViewportUpdated.Length)
		{
			float2 xy = ((float4)(ref m_TerrainSystem.heightMapSliceArea[index])).xy;
			float2 val = ((float4)(ref m_TerrainSystem.heightMapSliceArea[index])).zw - ((float4)(ref m_TerrainSystem.heightMapSliceArea[index])).xy;
			float3 zero = float3.zero;
			float3 zero2 = float3.zero;
			((float3)(ref zero)).xz = xy + val * ((float4)(ref m_TerrainSystem.heightMapViewportUpdated[index])).xy;
			((float3)(ref zero2)).xz = xy + val * (((float4)(ref m_TerrainSystem.heightMapViewportUpdated[index])).xy + ((float4)(ref m_TerrainSystem.heightMapViewportUpdated[index])).zw);
			zero.y = 0f;
			zero2.y = m_TerrainSystem.heightScaleOffset.x;
			((Bounds)(ref result)).SetMinMax(float3.op_Implicit(zero), float3.op_Implicit(zero2));
		}
		return result;
	}

	public Bounds GetCascadeCullArea(int index)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		Bounds result = default(Bounds);
		if (index >= 0 && index < m_TerrainSystem.heightMapCullArea.Length)
		{
			float3 val = default(float3);
			((float3)(ref val))._002Ector(m_TerrainSystem.heightMapCullArea[index].x, float.MaxValue, m_TerrainSystem.heightMapCullArea[index].y);
			float3 val2 = default(float3);
			((float3)(ref val2))._002Ector(m_TerrainSystem.heightMapCullArea[index].z, float.MinValue, m_TerrainSystem.heightMapCullArea[index].w);
			((Bounds)(ref result)).SetMinMax(float3.op_Implicit(val), float3.op_Implicit(val2));
		}
		return result;
	}

	public Bounds GetLastCullArea()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		Bounds result = default(Bounds);
		float3 val = default(float3);
		((float3)(ref val))._002Ector(m_TerrainSystem.lastCullArea.x, float.MaxValue, m_TerrainSystem.lastCullArea.y);
		float3 val2 = default(float3);
		((float3)(ref val2))._002Ector(m_TerrainSystem.lastCullArea.z, float.MinValue, m_TerrainSystem.lastCullArea.w);
		((Bounds)(ref result)).SetMinMax(float3.op_Implicit(val), float3.op_Implicit(val2));
		return result;
	}

	[Preserve]
	public TerrainRenderSystem()
	{
	}
}
