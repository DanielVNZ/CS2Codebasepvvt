using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Prefabs;
using Game.Rendering;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Simulation;

[FormerlySerializedAs("Colossal.Terrain.TerrainInitializeSystem, Game")]
[CompilerGenerated]
public class TerrainInitializeSystem : GameSystemBase
{
	private EntityQuery m_TerrainPropertiesQuery;

	private EntityQuery m_TerrainMaterialPropertiesQuery;

	private TerrainSystem m_TerrainSystem;

	private TerrainRenderSystem m_TerrainRenderSystem;

	private TerrainMaterialSystem m_TerrainMaterialSystem;

	private WaterSystem m_WaterSystem;

	private WaterRenderSystem m_WaterRenderSystem;

	private SnowSystem m_SnowSystem;

	private PrefabSystem m_PrefabSystem;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_TerrainMaterialSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainMaterialSystem>();
		m_TerrainRenderSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainRenderSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_WaterRenderSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterRenderSystem>();
		m_SnowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SnowSystem>();
		m_TerrainPropertiesQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<TerrainPropertiesData>()
		});
		m_TerrainMaterialPropertiesQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<TerrainMaterialPropertiesData>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_TerrainPropertiesQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref m_TerrainPropertiesQuery)).GetSingletonEntity();
		TerrainPropertiesPrefab prefab = m_PrefabSystem.GetPrefab<TerrainPropertiesPrefab>(singletonEntity);
		m_WaterSystem.MaxSpeed = prefab.m_WaterMaxSpeed;
	}

	[Preserve]
	public TerrainInitializeSystem()
	{
	}
}
