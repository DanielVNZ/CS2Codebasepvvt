using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Prefabs;
using Game.Reflection;
using Game.Simulation;
using Game.UI.Localization;
using Game.UI.Widgets;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.Editor;

[CompilerGenerated]
public class DiversityPanelSystem : EditorPanelSystemBase
{
	private PrefabSystem m_PrefabSystem;

	private DiversitySystem m_DiversitySystem;

	private EntityQuery m_AtmosphereQuery;

	private EntityQuery m_BiomeQuery;

	private AtmospherePrefab m_Atmosphere;

	private BiomePrefab m_Biome;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_DiversitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DiversitySystem>();
		m_AtmosphereQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AtmosphereData>() });
		m_BiomeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BiomeData>() });
		title = LocalizedString.Value("Diversity");
		children = new IWidget[1] { Scrollable.WithChildren(new IWidget[1]
		{
			new EditorSection
			{
				displayName = "Diversity Settings",
				expanded = true,
				children = new IWidget[2]
				{
					new PopupValueField<PrefabBase>
					{
						displayName = "Atmosphere",
						accessor = new DelegateAccessor<PrefabBase>(() => m_Atmosphere, SetAtmosphere),
						popup = new PrefabPickerPopup(typeof(AtmospherePrefab))
					},
					new PopupValueField<PrefabBase>
					{
						displayName = "Biome",
						accessor = new DelegateAccessor<PrefabBase>(() => m_Biome, SetBiome),
						popup = new PrefabPickerPopup(typeof(BiomePrefab))
					}
				}
			}
		}) };
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		((COSystemBase)this).OnStartRunning();
		AtmosphereData singleton = ((EntityQuery)(ref m_AtmosphereQuery)).GetSingleton<AtmosphereData>();
		m_PrefabSystem.TryGetPrefab<AtmospherePrefab>(singleton.m_AtmospherePrefab, out m_Atmosphere);
		BiomeData singleton2 = ((EntityQuery)(ref m_BiomeQuery)).GetSingleton<BiomeData>();
		m_PrefabSystem.TryGetPrefab<BiomePrefab>(singleton2.m_BiomePrefab, out m_Biome);
	}

	private void SetAtmosphere(PrefabBase prefab)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		m_Atmosphere = (AtmospherePrefab)prefab;
		Entity entity = m_PrefabSystem.GetEntity(m_Atmosphere);
		m_DiversitySystem.ApplyAtmospherePreset(entity);
	}

	private void SetBiome(PrefabBase prefab)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		m_Biome = (BiomePrefab)prefab;
		Entity entity = m_PrefabSystem.GetEntity(m_Biome);
		m_DiversitySystem.ApplyBiomePreset(entity);
	}

	[Preserve]
	public DiversityPanelSystem()
	{
	}
}
