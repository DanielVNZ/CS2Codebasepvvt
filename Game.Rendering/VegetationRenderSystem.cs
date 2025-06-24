using System;
using Colossal.Entities;
using Game.Simulation;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;
using UnityEngine.VFX;

namespace Game.Rendering;

public class VegetationRenderSystem : GameSystemBase
{
	private TerrainSystem m_TerrainSystem;

	private TerrainMaterialSystem m_TerrainMaterialSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private static VisualEffectAsset s_FoliageVFXAsset;

	private VisualEffect m_FoliageVFX;

	[Preserve]
	protected override void OnCreate()
	{
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_TerrainMaterialSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainMaterialSystem>();
		s_FoliageVFXAsset = Resources.Load<VisualEffectAsset>("Vegetation/FoliageVFX");
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		((COSystemBase)this).OnStopRunning();
		CoreUtils.Destroy((Object)(object)m_FoliageVFX);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		if (m_CameraUpdateSystem.activeViewer != null)
		{
			CreateDynamicVFXIfNeeded();
			UpdateEffect();
		}
	}

	private void UpdateEffect()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		Bounds terrainBounds = m_TerrainSystem.GetTerrainBounds();
		m_FoliageVFX.SetVector3("TerrainBounds_center", ((Bounds)(ref terrainBounds)).center);
		m_FoliageVFX.SetVector3("TerrainBounds_size", ((Bounds)(ref terrainBounds)).size);
		m_FoliageVFX.SetTexture("Terrain HeightMap", m_TerrainSystem.heightmap);
		m_FoliageVFX.SetTexture("Terrain SplatMap", m_TerrainMaterialSystem.splatmap);
		Vector4 globalVector = Shader.GetGlobalVector("colossal_TerrainScale");
		Vector4 globalVector2 = Shader.GetGlobalVector("colossal_TerrainOffset");
		m_FoliageVFX.SetVector4("Terrain Offset Scale", new Vector4(globalVector.x, globalVector.z, globalVector2.x, globalVector2.z));
		m_FoliageVFX.SetVector3("CameraPosition", float3.op_Implicit(m_CameraUpdateSystem.position));
		m_FoliageVFX.SetVector3("CameraDirection", float3.op_Implicit(m_CameraUpdateSystem.direction));
	}

	private void CreateDynamicVFXIfNeeded()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)s_FoliageVFXAsset != (Object)null && (Object)(object)m_FoliageVFX == (Object)null)
		{
			COSystemBase.baseLog.DebugFormat("Creating FoliageVFX", Array.Empty<object>());
			m_FoliageVFX = new GameObject("FoliageVFX").AddComponent<VisualEffect>();
			m_FoliageVFX.visualEffectAsset = s_FoliageVFXAsset;
		}
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	[Preserve]
	public VegetationRenderSystem()
	{
	}
}
