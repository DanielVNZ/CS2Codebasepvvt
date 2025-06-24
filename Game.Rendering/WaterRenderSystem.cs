using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Simulation;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Scripting;

namespace Game.Rendering;

[FormerlySerializedAs("Colossal.Terrain.WaterRenderSystem, Game")]
[CompilerGenerated]
public class WaterRenderSystem : GameSystemBase
{
	private TerrainRenderSystem m_TerrainRenderSystem;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private RenderingSystem m_RenderingSystem;

	public Texture overrideOverlaymap { get; set; }

	public Texture overlayExtramap { get; set; }

	public float4 overlayPollutionMask { get; set; }

	public float4 overlayArrowMask { get; set; }

	public Texture waterTexture => (Texture)(object)m_WaterSystem.WaterRenderTexture;

	public Texture flowTexture => m_WaterSystem.FlowTextureUpdated;

	public bool IsAsync { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		base.OnCreate();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		foreach (WaterSurface instance in WaterSurface.instances)
		{
			if ((Object)(object)instance.customMaterial != (Object)null)
			{
				instance.customMaterial = new Material(instance.customMaterial);
			}
		}
	}

	[Preserve]
	protected override void OnDestroy()
	{
		foreach (WaterSurface instance in WaterSurface.instances)
		{
			if ((Object)(object)instance.customMaterial != (Object)null)
			{
				CoreUtils.Destroy((Object)(object)instance.customMaterial);
			}
		}
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		m_TerrainSystem.GetCascadeInfo(out var _, out var baseLOD, out var areas, out var _, out var _);
		foreach (WaterSurface instance in WaterSurface.instances)
		{
			float frameDelta = m_RenderingSystem.frameDelta;
			WorldUnmanaged worldUnmanaged = ((SystemState)(ref ((SystemBase)this).CheckedStateRef)).WorldUnmanaged;
			float timeMultiplier = frameDelta / math.max(1E-06f, ((WorldUnmanaged)(ref worldUnmanaged)).Time.DeltaTime * 60f);
			instance.timeMultiplier = timeMultiplier;
			instance.CascadeArea = float4x4.op_Implicit(areas);
			if (baseLOD == 0)
			{
				instance.WaterSimArea = new Vector4(areas.c0.x, areas.c1.x, areas.c2.x - areas.c0.x, areas.c3.x - areas.c1.x);
			}
			else
			{
				instance.WaterSimArea = new Vector4(areas.c0.y, areas.c1.y, areas.c2.y - areas.c0.y, areas.c3.y - areas.c1.y);
			}
			instance.TerrainScaleOffset = float2.op_Implicit(m_TerrainSystem.heightScaleOffset);
			instance.TerrainCascadeTexture = m_TerrainSystem.GetCascadeTexture();
			if (m_WaterSystem.Loaded)
			{
				instance.WaterSimulationTexture = (Texture)(object)m_WaterSystem.WaterTexture;
			}
			else
			{
				instance.WaterSimulationTexture = (Texture)(object)Texture2D.blackTexture;
			}
			if (!Object.op_Implicit((Object)(object)instance.customMaterial))
			{
				continue;
			}
			instance.customMaterial.SetVector(TerrainRenderSystem.ShaderID._OverlayArrowMask, float4.op_Implicit(overlayArrowMask));
			instance.customMaterial.SetVector(TerrainRenderSystem.ShaderID._OverlayPollutionMask, float4.op_Implicit(overlayPollutionMask));
			if ((Object)(object)overrideOverlaymap != (Object)null)
			{
				instance.customMaterial.SetTexture(TerrainRenderSystem.ShaderID._BaseColorMap, overrideOverlaymap);
			}
			if ((Object)(object)overlayExtramap != (Object)null)
			{
				if ((Object)(object)overrideOverlaymap == (Object)null)
				{
					overrideOverlaymap = (Texture)(object)Texture2D.whiteTexture;
				}
				if ((Object)(object)overlayExtramap == (Object)(object)flowTexture)
				{
					instance.customMaterial.SetFloat(TerrainRenderSystem.ShaderID._OverlayArrowSource, 1f);
				}
				else
				{
					instance.customMaterial.SetTexture(TerrainRenderSystem.ShaderID._OverlayExtra, overlayExtramap);
					instance.customMaterial.SetFloat(TerrainRenderSystem.ShaderID._OverlayArrowSource, 0f);
				}
				instance.customMaterial.EnableKeyword("OVERRIDE_OVERLAY_EXTRA");
			}
			else
			{
				instance.customMaterial.DisableKeyword("OVERRIDE_OVERLAY_EXTRA");
			}
		}
		_ = m_WaterSystem.Loaded;
	}

	[Preserve]
	public WaterRenderSystem()
	{
	}
}
