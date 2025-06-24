using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Areas;
using Game.Audio;
using Game.City;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Simulation;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class MapTilesUISystem : UISystemBase
{
	public readonly struct UIMapTileResource : IJsonWritable
	{
		public string id { get; }

		public string icon { get; }

		public float value { get; }

		public string unit { get; }

		public UIMapTileResource(string id, string icon, float value, string unit)
		{
			this.id = id;
			this.icon = icon;
			this.value = value;
			this.unit = unit;
		}

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin("mapTiles.UIMapTileResource");
			writer.PropertyName("id");
			writer.Write(id);
			writer.PropertyName("icon");
			writer.Write(icon);
			writer.PropertyName("value");
			writer.Write(value);
			writer.PropertyName("unit");
			writer.Write(unit);
			writer.TypeEnd();
		}
	}

	private const string kGroup = "mapTiles";

	private MapTilePurchaseSystem m_MapTileSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private AudioManager m_AudioManager;

	private GameScreenUISystem m_GameScreenUISystem;

	private EntityQuery m_SoundQuery;

	private GetterValueBinding<bool> m_MapTilesPanelVisibleBinding;

	private GetterValueBinding<bool> m_MapTilesViewActiveBinding;

	private RawValueBinding m_ResourcesBinding;

	private ValueBinding<UIMapTileResource> m_BuildableLandBinding;

	private ValueBinding<UIMapTileResource> m_WaterBinding;

	private GetterValueBinding<int> m_PurchasePriceBinding;

	private GetterValueBinding<int> m_PurchaseUpkeepBinding;

	private GetterValueBinding<int> m_PurchaseFlagsBinding;

	private GetterValueBinding<int> m_ExpansionPermitsBinding;

	private GetterValueBinding<int> m_ExpansionPermitCostBinding;

	private int m_LastSelected;

	private bool m_IsLastTimeZoomOut;

	public static bool mapTileViewActive { get; private set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Expected O, but got Unknown
		//IL_020b: Expected O, but got Unknown
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Expected O, but got Unknown
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_MapTileSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MapTilePurchaseSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_AudioManager = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AudioManager>();
		m_GameScreenUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GameScreenUISystem>();
		AddBinding((IBinding)(object)(m_MapTilesPanelVisibleBinding = new GetterValueBinding<bool>("mapTiles", "mapTilePanelVisible", (Func<bool>)(() => mapTileViewActive && !m_CityConfigurationSystem.unlockMapTiles), (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		AddBinding((IBinding)(object)(m_MapTilesViewActiveBinding = new GetterValueBinding<bool>("mapTiles", "mapTileViewActive", (Func<bool>)(() => mapTileViewActive), (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		AddBinding((IBinding)(object)(m_BuildableLandBinding = new ValueBinding<UIMapTileResource>("mapTiles", "buildableLand", GetResource(MapFeature.BuildableLand), (IWriter<UIMapTileResource>)(object)new ValueWriter<UIMapTileResource>(), (EqualityComparer<UIMapTileResource>)null)));
		AddBinding((IBinding)(object)(m_WaterBinding = new ValueBinding<UIMapTileResource>("mapTiles", "water", GetResource(MapFeature.GroundWater), (IWriter<UIMapTileResource>)(object)new ValueWriter<UIMapTileResource>(), (EqualityComparer<UIMapTileResource>)null)));
		AddBinding((IBinding)(object)(m_PurchasePriceBinding = new GetterValueBinding<int>("mapTiles", "purchasePrice", (Func<int>)(() => m_MapTileSystem.cost), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_PurchaseUpkeepBinding = new GetterValueBinding<int>("mapTiles", "purchaseUpkeep", (Func<int>)(() => m_MapTileSystem.upkeep), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_PurchaseFlagsBinding = new GetterValueBinding<int>("mapTiles", "purchaseFlags", (Func<int>)(() => (int)m_MapTileSystem.status), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_ExpansionPermitsBinding = new GetterValueBinding<int>("mapTiles", "expansionPermits", (Func<int>)(() => m_MapTileSystem.GetAvailableTiles()), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_ExpansionPermitCostBinding = new GetterValueBinding<int>("mapTiles", "expansionPermitCost", (Func<int>)(() => m_MapTileSystem.GetSelectedTileCount()), (IWriter<int>)null, (EqualityComparer<int>)null)));
		RawValueBinding val = new RawValueBinding("mapTiles", "resources", (Action<IJsonWriter>)BindResources);
		RawValueBinding binding = val;
		m_ResourcesBinding = val;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)new TriggerBinding<bool>("mapTiles", "setMapTileViewActive", (Action<bool>)SetMapTileViewActive, (IReader<bool>)null));
		AddBinding((IBinding)new TriggerBinding("mapTiles", "purchaseMapTiles", (Action)PurchaseMapTiles));
		m_SoundQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ToolUXSoundSettingsData>() });
		m_IsLastTimeZoomOut = false;
	}

	private void BindResources(IJsonWriter binder)
	{
		binder.ArrayBegin(5u);
		JsonWriterExtensions.Write<UIMapTileResource>(binder, GetResource(MapFeature.FertileLand));
		JsonWriterExtensions.Write<UIMapTileResource>(binder, GetResource(MapFeature.Forest));
		JsonWriterExtensions.Write<UIMapTileResource>(binder, GetResource(MapFeature.Oil));
		JsonWriterExtensions.Write<UIMapTileResource>(binder, GetResource(MapFeature.Ore));
		JsonWriterExtensions.Write<UIMapTileResource>(binder, GetResource(MapFeature.Fish));
		binder.ArrayEnd();
	}

	private UIMapTileResource GetResource(MapFeature feature)
	{
		return feature switch
		{
			MapFeature.BuildableLand => new UIMapTileResource("BuildableLand", "Media/Game/Icons/MapTile.svg", m_MapTileSystem.GetFeatureAmount(MapFeature.BuildableLand), "area"), 
			MapFeature.FertileLand => new UIMapTileResource("FertileLand", "Media/Game/Icons/Fertility.svg", m_MapTileSystem.GetFeatureAmount(MapFeature.FertileLand), "area"), 
			MapFeature.Forest => new UIMapTileResource("Forest", "Media/Game/Icons/Forest.svg", m_MapTileSystem.GetFeatureAmount(MapFeature.Forest), "weight"), 
			MapFeature.Oil => new UIMapTileResource("Oil", "Media/Game/Icons/Oil.svg", m_MapTileSystem.GetFeatureAmount(MapFeature.Oil), "weight"), 
			MapFeature.Ore => new UIMapTileResource("Ore", "Media/Game/Icons/Coal.svg", m_MapTileSystem.GetFeatureAmount(MapFeature.Ore), "weight"), 
			MapFeature.Fish => new UIMapTileResource("Fish", "Media/Game/Resources/Fish.svg", m_MapTileSystem.GetFeatureAmount(MapFeature.Fish), "weight"), 
			_ => new UIMapTileResource("Water", "Media/Game/Icons/Water.svg", m_MapTileSystem.GetFeatureAmount(MapFeature.GroundWater), "volume"), 
		};
	}

	[Preserve]
	protected override void OnUpdate()
	{
		m_MapTilesViewActiveBinding.Update();
		m_MapTilesPanelVisibleBinding.Update();
		m_ExpansionPermitsBinding.Update();
		if (!mapTileViewActive)
		{
			return;
		}
		((ComponentSystemBase)m_MapTileSystem).Update();
		if (m_MapTileSystem.selecting)
		{
			m_PurchaseFlagsBinding.Update();
			int selectedTileCount = m_MapTileSystem.GetSelectedTileCount();
			if (m_LastSelected != selectedTileCount)
			{
				m_LastSelected = selectedTileCount;
				m_PurchasePriceBinding.Update();
				m_PurchaseUpkeepBinding.Update();
				m_ExpansionPermitCostBinding.Update();
				m_ResourcesBinding.Update();
				m_BuildableLandBinding.Update(GetResource(MapFeature.BuildableLand));
				m_WaterBinding.Update(GetResource(MapFeature.GroundWater));
			}
		}
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, mode);
		mapTileViewActive = false;
	}

	private void SetMapTileViewActive(bool enabled)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (enabled && m_GameScreenUISystem.activeScreen != GameScreenUISystem.GameScreen.Main)
		{
			m_GameScreenUISystem.SetScreen(GameScreenUISystem.GameScreen.Main);
		}
		mapTileViewActive = enabled;
		m_MapTileSystem.selecting = enabled && !m_CityConfigurationSystem.unlockMapTiles;
		if (m_IsLastTimeZoomOut != enabled && !GameManager.instance.isGameLoading)
		{
			Entity clipEntity = (enabled ? ((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_CameraZoomInSound : ((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_CameraZoomOutSound);
			m_AudioManager.PlayUISound(clipEntity);
		}
		m_IsLastTimeZoomOut = enabled;
	}

	private void PurchaseMapTiles()
	{
		m_MapTileSystem.PurchaseSelection();
	}

	[Preserve]
	public MapTilesUISystem()
	{
	}
}
