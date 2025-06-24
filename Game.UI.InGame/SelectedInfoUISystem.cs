using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Creatures;
using Game.Events;
using Game.Net;
using Game.Notifications;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Routes;
using Game.SceneFlow;
using Game.Simulation;
using Game.Tools;
using Game.UI.Debug;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class SelectedInfoUISystem : UISystemBase
{
	private const string kGroup = "selectedInfo";

	public static OrbitCameraController s_CameraController;

	private float3 m_SelectedPosition;

	private Entity m_SelectedEntity;

	private Entity m_SelectedPrefab;

	private Entity m_SelectedRoute;

	private Entity m_LastSelectedEntity;

	private EntityQuery m_TransportConfigQuery;

	private List<ISectionSource> m_TopSections;

	private List<ISectionSource> m_MiddleSections;

	private List<ISectionSource> m_BottomSections;

	private TitleSection m_TitleSection;

	private DeveloperSection m_DeveloperSection;

	private LineVisualizerSection m_LineVisualizerSection;

	private HouseholdSidebarSection m_HouseholdSidebarSection;

	private DebugUISystem m_DebugUISystem;

	private ToolSystem m_ToolSystem;

	private PrefabSystem m_PrefabSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private ValueBinding<Entity> m_SelectedEntityBinding;

	private ValueBinding<Entity> m_SelectedTrailerControllerBinding;

	private ValueBinding<string> m_SelectedUITagBinding;

	private GetterValueBinding<Entity> m_SelectedRouteBinding;

	private ValueBinding<bool> m_ActiveSelectionBinding;

	private RawValueBinding m_TopSectionsBinding;

	private RawValueBinding m_MiddleSectionsBinding;

	private RawValueBinding m_BottomSectionsBinding;

	private RawValueBinding m_IDSectionBinding;

	private RawValueBinding m_LineVisualizerSectionBinding;

	private RawValueBinding m_DeveloperSectionBinding;

	private RawValueBinding m_HouseholdSidebarSectionBinding;

	private ValueBinding<float2> m_PositionBinding;

	private RawValueBinding m_TooltipTagsBinding;

	private bool m_BindingsDirty;

	private UIUpdateState m_UpdateState;

	public override GameMode gameMode => GameMode.Game;

	public float3 selectedPosition => m_SelectedPosition;

	public Entity selectedEntity => m_SelectedEntity;

	public Entity selectedPrefab => m_SelectedPrefab;

	public Entity selectedRoute
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_SelectedRoute;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_SelectedRoute = value;
			m_SelectedRouteBinding.Update();
		}
	}

	public Action<Entity, Entity, float3> eventSelectionChanged { get; set; }

	public List<TooltipTags> tooltipTags { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_01a7: Expected O, but got Unknown
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Expected O, but got Unknown
		//IL_01d3: Expected O, but got Unknown
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Expected O, but got Unknown
		//IL_01ff: Expected O, but got Unknown
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Expected O, but got Unknown
		//IL_022b: Expected O, but got Unknown
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Expected O, but got Unknown
		//IL_0257: Expected O, but got Unknown
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Expected O, but got Unknown
		//IL_0283: Expected O, but got Unknown
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Expected O, but got Unknown
		//IL_02af: Expected O, but got Unknown
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Expected O, but got Unknown
		//IL_0307: Expected O, but got Unknown
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Expected O, but got Unknown
		base.OnCreate();
		AddSections(m_TopSections = new List<ISectionSource>(), m_MiddleSections = new List<ISectionSource>(), m_BottomSections = new List<ISectionSource>());
		tooltipTags = new List<TooltipTags>();
		m_DebugUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DebugUISystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_TransportConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<UITransportConfigurationData>() });
		m_SelectedEntity = Entity.Null;
		m_SelectedPrefab = Entity.Null;
		m_LastSelectedEntity = Entity.Null;
		AddBinding((IBinding)(object)(m_SelectedEntityBinding = new ValueBinding<Entity>("selectedInfo", "selectedEntity", Entity.Null, (IWriter<Entity>)null, (EqualityComparer<Entity>)null)));
		AddBinding((IBinding)(object)(m_SelectedTrailerControllerBinding = new ValueBinding<Entity>("selectedInfo", "selectedTrailerController", Entity.Null, (IWriter<Entity>)null, (EqualityComparer<Entity>)null)));
		AddBinding((IBinding)(object)(m_SelectedUITagBinding = new ValueBinding<string>("selectedInfo", "selectedUITag", string.Empty, (IWriter<string>)null, (EqualityComparer<string>)null)));
		AddBinding((IBinding)(object)(m_SelectedRouteBinding = new GetterValueBinding<Entity>("selectedInfo", "selectedRoute", (Func<Entity>)(() => m_SelectedRoute), (IWriter<Entity>)null, (EqualityComparer<Entity>)null)));
		AddBinding((IBinding)(object)(m_ActiveSelectionBinding = new ValueBinding<bool>("selectedInfo", "activeSelection", false, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		RawValueBinding val = new RawValueBinding("selectedInfo", "topSections", (Action<IJsonWriter>)delegate(IJsonWriter writer)
		{
			WriteSections(m_TopSections, writer);
		});
		RawValueBinding binding = val;
		m_TopSectionsBinding = val;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val2 = new RawValueBinding("selectedInfo", "middleSections", (Action<IJsonWriter>)delegate(IJsonWriter writer)
		{
			WriteSections(m_MiddleSections, writer);
		});
		binding = val2;
		m_MiddleSectionsBinding = val2;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val3 = new RawValueBinding("selectedInfo", "bottomSections", (Action<IJsonWriter>)delegate(IJsonWriter writer)
		{
			WriteSections(m_BottomSections, writer);
		});
		binding = val3;
		m_BottomSectionsBinding = val3;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val4 = new RawValueBinding("selectedInfo", "titleSection", (Action<IJsonWriter>)delegate(IJsonWriter writer)
		{
			m_TitleSection.Write(writer);
		});
		binding = val4;
		m_IDSectionBinding = val4;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val5 = new RawValueBinding("selectedInfo", "lineVisualizerSection", (Action<IJsonWriter>)delegate(IJsonWriter writer)
		{
			m_LineVisualizerSection.Write(writer);
		});
		binding = val5;
		m_LineVisualizerSectionBinding = val5;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val6 = new RawValueBinding("selectedInfo", "developerSection", (Action<IJsonWriter>)WriteDeveloperSection);
		binding = val6;
		m_DeveloperSectionBinding = val6;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val7 = new RawValueBinding("selectedInfo", "householdSidebarSection", (Action<IJsonWriter>)delegate(IJsonWriter writer)
		{
			m_HouseholdSidebarSection.Write(writer);
		});
		binding = val7;
		m_HouseholdSidebarSectionBinding = val7;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)(m_PositionBinding = new ValueBinding<float2>("selectedInfo", "position", default(float2), (IWriter<float2>)null, (EqualityComparer<float2>)null)));
		RawValueBinding val8 = new RawValueBinding("selectedInfo", "tooltipTags", (Action<IJsonWriter>)WriteTooltipFlags);
		binding = val8;
		m_TooltipTagsBinding = val8;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)new TriggerBinding<Entity>("selectedInfo", "selectEntity", (Action<Entity>)OnSelect, (IReader<Entity>)null));
		AddBinding((IBinding)new TriggerBinding("selectedInfo", "clearSelection", (Action)OnClearSelection));
		AddBinding((IBinding)(object)new TriggerBinding<Entity>("selectedInfo", "setSelectedRoute", (Action<Entity>)OnSetSelectedRoute, (IReader<Entity>)null));
		m_UpdateState = UIUpdateState.Create(((ComponentSystemBase)this).World, 256);
		ToolSystem toolSystem = m_ToolSystem;
		toolSystem.EventToolChanged = (Action<ToolBaseSystem>)Delegate.Combine(toolSystem.EventToolChanged, new Action<ToolBaseSystem>(OnToolChanged));
	}

	private void OnToolChanged(ToolBaseSystem obj)
	{
		m_UpdateState.ForceUpdate();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		if ((Object)(object)s_CameraController != (Object)null)
		{
			OrbitCameraController orbitCameraController = s_CameraController;
			orbitCameraController.EventCameraMove = (Action)Delegate.Remove(orbitCameraController.EventCameraMove, new Action(OnCameraStoppedFollowing));
		}
		base.OnDestroy();
	}

	public void AddTopSection(ISectionSource section)
	{
		m_TopSections.Add(section);
	}

	public void AddMiddleSection(ISectionSource section)
	{
		m_MiddleSections.Add(section);
	}

	public void AddBottomSection(ISectionSource section)
	{
		m_BottomSections.Add(section);
	}

	public void AddDeveloperInfo(ISubsectionSource subsection)
	{
		m_DeveloperSection.AddSubsection(subsection);
	}

	private void AddSections(List<ISectionSource> topSections, List<ISectionSource> sections, List<ISectionSource> bottomSections)
	{
		m_TitleSection = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TitleSection>();
		m_LineVisualizerSection = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LineVisualizerSection>();
		m_HouseholdSidebarSection = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<HouseholdSidebarSection>();
		m_DeveloperSection = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DeveloperSection>();
		topSections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NotificationsSection>());
		topSections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AverageHappinessSection>());
		topSections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<StatusSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DescriptionSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ContentPrerequisiteSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DestroyedBuildingSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DestroyedTreeSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PoliciesSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LevelSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpkeepSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpgradePropertiesSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EfficiencySection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResidentsSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EmployeesSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AttractivenessSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ComfortSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EducationSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ElectricitySection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatterySection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TransformerSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<HealthcareSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DeathcareSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GarbageSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PoliceSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ShelterSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrisonSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SewageSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ParkSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ParkingSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MailSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitizenSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DummyHumanSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AnimalSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrivateVehicleSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PublicTransportVehicleSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CargoTransportVehicleSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DeliveryVehicleSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<HealthcareVehicleSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DeathcareVehicleSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<FireVehicleSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PoliceVehicleSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GarbageVehicleSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PostVehicleSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MaintenanceVehicleSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ExtractorVehicleSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DistrictsSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LocalServicesSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PassengersSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CargoSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LoadSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CompanySection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<StorageSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TradedResourcesSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SelectVehiclesSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LineSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TicketPriceSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<VehicleCountSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ScheduleSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ColorSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LinesSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RoadSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PollutionSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DispatchedVehiclesSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<VehiclesSection>());
		sections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpgradesSection>());
		bottomSections.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ActionsSection>());
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		s_CameraController = m_CameraUpdateSystem.orbitCameraController;
		if ((Object)(object)s_CameraController != (Object)null)
		{
			if (GameManager.instance.gameMode == GameMode.Editor)
			{
				s_CameraController.mode = OrbitCameraController.Mode.Editor;
				return;
			}
			OrbitCameraController orbitCameraController = s_CameraController;
			orbitCameraController.EventCameraMove = (Action)Delegate.Combine(orbitCameraController.EventCameraMove, new Action(OnCameraStoppedFollowing));
			s_CameraController.mode = OrbitCameraController.Mode.Follow;
		}
	}

	private void OnCameraStoppedFollowing()
	{
		StopFollowing();
	}

	private void StartFollowing(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).Exists(entity) && (Object)(object)s_CameraController != (Object)null && s_CameraController.followedEntity != entity)
		{
			s_CameraController.followedEntity = entity;
			s_CameraController.TryMatchPosition(m_CameraUpdateSystem.activeCameraController);
			m_CameraUpdateSystem.activeCameraController = s_CameraController;
			SetBindingsDirty();
		}
	}

	private void StopFollowing()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (m_CameraUpdateSystem.orbitCameraController.mode == OrbitCameraController.Mode.Follow)
		{
			m_CameraUpdateSystem.orbitCameraController.followedEntity = Entity.Null;
			m_CameraUpdateSystem.gamePlayController.TryMatchPosition(m_CameraUpdateSystem.orbitCameraController);
			m_CameraUpdateSystem.activeCameraController = m_CameraUpdateSystem.gamePlayController;
			SetBindingsDirty();
		}
	}

	public void RequestUpdate()
	{
		m_UpdateState.ForceUpdate();
	}

	public void SetDirty()
	{
		SetBindingsDirty();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		RefreshSelection();
		m_SelectedEntityBinding.Update(m_SelectedEntity);
		Controller controller = default(Controller);
		DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
		if (EntitiesExtensions.TryGetComponent<Controller>(((ComponentSystemBase)this).EntityManager, m_SelectedEntity, ref controller))
		{
			m_SelectedTrailerControllerBinding.Update(controller.m_Controller);
		}
		else if (EntitiesExtensions.TryGetBuffer<LayoutElement>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val) && val.Length != 0)
		{
			m_SelectedTrailerControllerBinding.Update(m_SelectedEntity);
		}
		else
		{
			m_SelectedTrailerControllerBinding.Update(Entity.Null);
		}
		m_SelectedUITagBinding.Update(m_PrefabSystem.TryGetPrefab<PrefabBase>(m_SelectedPrefab, out var prefab) ? prefab.uiTag : string.Empty);
		m_ActiveSelectionBinding.Update(m_SelectedEntity != Entity.Null);
		if (m_LastSelectedEntity != m_SelectedEntity)
		{
			ResetRouteVisibility();
			m_LastSelectedEntity = m_SelectedEntity;
			eventSelectionChanged?.Invoke(m_SelectedEntity, m_SelectedPrefab, m_SelectedPosition);
			SetBindingsDirty();
		}
		else
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Updated>(m_SelectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<BatchesUpdated>(m_SelectedEntity) && !m_UpdateState.Advance())
				{
					goto IL_0158;
				}
			}
			SetBindingsDirty();
		}
		goto IL_0158;
		IL_0158:
		UpdateSections();
		UpdatePosition();
	}

	private void RefreshSelection()
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		PrefabRef prefabRef = default(PrefabRef);
		if (TryGetSelection(out var entity) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, entity, ref prefabRef))
		{
			Entity prefab = prefabRef.m_Prefab;
			FilterSelection(ref entity, ref prefab);
			int elementIndex = m_ToolSystem.selectedIndex;
			if (!TryGetPosition(entity, ((ComponentSystemBase)this).EntityManager, ref elementIndex, out var _, out var position, out var bounds, out var _, reinterpolate: true))
			{
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<Household>(entity))
				{
					goto IL_008f;
				}
			}
			position.y = MathUtils.Center(((Bounds3)(ref bounds)).y);
			m_SelectedEntity = entity;
			m_SelectedPrefab = prefab;
			m_SelectedPosition = position;
			return;
		}
		goto IL_008f;
		IL_008f:
		m_SelectedEntity = Entity.Null;
		m_SelectedPrefab = Entity.Null;
		m_SelectedPosition = float3.zero;
	}

	private bool TryGetSelection(out Entity entity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		entity = m_ToolSystem.selected;
		return entity != Entity.Null;
	}

	private void FilterSelection(ref Entity entity, ref Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Owner owner = default(Owner);
		if (((EntityManager)(ref entityManager)).HasComponent<Icon>(entity) && EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, entity, ref owner))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<RouteLane>(owner.m_Owner))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				Owner owner2 = default(Owner);
				if (((EntityManager)(ref entityManager)).HasComponent<Waypoint>(owner.m_Owner) && EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, owner.m_Owner, ref owner2))
				{
					entity = owner2.m_Owner;
					goto IL_00d1;
				}
			}
			CurrentBuilding currentBuilding = default(CurrentBuilding);
			if (EntitiesExtensions.TryGetComponent<CurrentBuilding>(((ComponentSystemBase)this).EntityManager, owner.m_Owner, ref currentBuilding))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).Exists(currentBuilding.m_CurrentBuilding))
				{
					entity = currentBuilding.m_CurrentBuilding;
				}
			}
			else
			{
				entity = owner.m_Owner;
			}
			goto IL_00d1;
		}
		goto IL_00ff;
		IL_00d1:
		PrefabRef prefabRef = default(PrefabRef);
		if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, entity, ref prefabRef))
		{
			prefab = prefabRef.m_Prefab;
		}
		SetSelection(entity);
		goto IL_00ff;
		IL_00ff:
		Game.Creatures.Resident resident = default(Game.Creatures.Resident);
		PrefabRef prefabRef2 = default(PrefabRef);
		if (EntitiesExtensions.TryGetComponent<Game.Creatures.Resident>(((ComponentSystemBase)this).EntityManager, entity, ref resident) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, resident.m_Citizen, ref prefabRef2))
		{
			entity = resident.m_Citizen;
			prefab = prefabRef2.m_Prefab;
		}
		Game.Creatures.Pet pet = default(Game.Creatures.Pet);
		PrefabRef prefabRef3 = default(PrefabRef);
		if (EntitiesExtensions.TryGetComponent<Game.Creatures.Pet>(((ComponentSystemBase)this).EntityManager, entity, ref pet) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, pet.m_HouseholdPet, ref prefabRef3))
		{
			entity = pet.m_HouseholdPet;
			prefab = prefabRef3.m_Prefab;
		}
	}

	public void SetSelection(Entity entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!(entity == m_SelectedEntity))
		{
			m_ToolSystem.selected = entity;
		}
	}

	public void SetRoutesVisible()
	{
		if (TryGetTransportConfig(out var config) && m_ToolSystem.GetInfomodes(m_ToolSystem.activeInfoview)?.Find((InfomodeInfo mode) => (Object)(object)mode.m_Mode == (Object)(object)config.m_RoutesInfomode) == null)
		{
			m_ToolSystem.SetInfomodeActive(config.m_RoutesInfomode, active: true, 0);
		}
	}

	private void ResetRouteVisibility()
	{
		if (TryGetTransportConfig(out var config) && m_ToolSystem.GetInfomodes(m_ToolSystem.activeInfoview)?.Find((InfomodeInfo mode) => (Object)(object)mode.m_Mode == (Object)(object)config.m_RoutesInfomode) == null)
		{
			m_ToolSystem.SetInfomodeActive(config.m_RoutesInfomode, active: false, 0);
		}
	}

	private bool TryGetTransportConfig(out UITransportConfigurationPrefab config)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return m_PrefabSystem.TryGetSingletonPrefab<UITransportConfigurationPrefab>(m_TransportConfigQuery, out config);
	}

	private void SetBindingsDirty()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (!(m_SelectedEntity == Entity.Null))
		{
			for (int i = 0; i < m_TopSections.Count; i++)
			{
				m_TopSections[i]?.RequestUpdate();
			}
			for (int j = 0; j < m_MiddleSections.Count; j++)
			{
				m_MiddleSections[j]?.RequestUpdate();
			}
			for (int k = 0; k < m_BottomSections.Count; k++)
			{
				m_BottomSections[k]?.RequestUpdate();
			}
			m_TitleSection?.RequestUpdate();
			m_LineVisualizerSection?.RequestUpdate();
			m_DeveloperSection?.RequestUpdate();
			m_HouseholdSidebarSection?.RequestUpdate();
			m_BindingsDirty = true;
		}
	}

	private void UpdateSections()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (!(m_SelectedEntity == Entity.Null))
		{
			tooltipTags.Clear();
			m_TitleSection?.PerformUpdate();
			m_HouseholdSidebarSection?.PerformUpdate();
			m_LineVisualizerSection?.PerformUpdate();
			m_LineVisualizerSectionBinding.Update();
			for (int i = 0; i < m_TopSections.Count; i++)
			{
				m_TopSections[i]?.PerformUpdate();
			}
			for (int j = 0; j < m_MiddleSections.Count; j++)
			{
				m_MiddleSections[j]?.PerformUpdate();
			}
			for (int k = 0; k < m_BottomSections.Count; k++)
			{
				m_BottomSections[k]?.PerformUpdate();
			}
			if (m_DebugUISystem.developerInfoVisible)
			{
				m_DeveloperSection?.PerformUpdate();
			}
			if (m_BindingsDirty)
			{
				UpdateSectionBindings();
				m_BindingsDirty = false;
			}
		}
	}

	private void UpdateSectionBindings()
	{
		m_TooltipTagsBinding.Update();
		m_IDSectionBinding.Update();
		m_DeveloperSectionBinding.Update();
		m_TopSectionsBinding.Update();
		m_MiddleSectionsBinding.Update();
		m_BottomSectionsBinding.Update();
		m_HouseholdSidebarSectionBinding.Update();
	}

	private void UpdatePosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (!(m_SelectedEntity == Entity.Null))
		{
			Vector2 val = Vector2.op_Implicit((Vector3)(((Object)(object)Camera.main == (Object)null) ? default(Vector3) : Camera.main.WorldToViewportPoint(float3.op_Implicit(selectedPosition))));
			m_PositionBinding.Update(float2.op_Implicit(val));
		}
	}

	private void OnClearSelection()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetSelection(Entity.Null);
	}

	private void OnSelect(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).Exists(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Game.Events.TrafficAccident>(entity))
			{
				SetSelection(entity);
			}
			else
			{
				StartFollowing(entity);
			}
		}
	}

	private void OnSetSelectedRoute(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		selectedRoute = entity;
	}

	public void Focus(Entity entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (entity == Entity.Null)
		{
			StopFollowing();
		}
		else
		{
			StartFollowing(entity);
		}
	}

	public static bool TryGetPosition(Entity entity, EntityManager entityManager, ref int elementIndex, out Entity location, out float3 position, out Bounds3 bounds, out quaternion rotation, bool reinterpolate = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		location = entity;
		FilterPositionTarget(ref location, entityManager);
		DynamicBuffer<TransformFrame> transformFrames = default(DynamicBuffer<TransformFrame>);
		Relative relative = default(Relative);
		Transform transform = default(Transform);
		DynamicBuffer<RouteWaypoint> routeWaypoints = default(DynamicBuffer<RouteWaypoint>);
		DynamicBuffer<LabelPosition> labelPositions = default(DynamicBuffer<LabelPosition>);
		Geometry geometry = default(Geometry);
		Icon icon = default(Icon);
		Game.Net.Node node = default(Game.Net.Node);
		if (EntitiesExtensions.TryGetBuffer<TransformFrame>(entityManager, location, true, ref transformFrames))
		{
			Transform interpolatedPosition = GetInterpolatedPosition(location, entityManager, transformFrames, reinterpolate, out bounds);
			position = interpolatedPosition.m_Position;
			rotation = interpolatedPosition.m_Rotation;
		}
		else if (EntitiesExtensions.TryGetComponent<Relative>(entityManager, location, ref relative))
		{
			Transform relativePosition = GetRelativePosition(location, entityManager, relative, reinterpolate, out bounds);
			position = relativePosition.m_Position;
			rotation = relativePosition.m_Rotation;
		}
		else if (EntitiesExtensions.TryGetComponent<Transform>(entityManager, location, ref transform))
		{
			Transform objectPosition = GetObjectPosition(location, entityManager, transform, out bounds);
			position = objectPosition.m_Position;
			rotation = objectPosition.m_Rotation;
		}
		else if (EntitiesExtensions.TryGetBuffer<RouteWaypoint>(entityManager, location, true, ref routeWaypoints))
		{
			position = GetRoutePosition(entityManager, routeWaypoints);
			bounds = new Bounds3(position, position);
			rotation = quaternion.identity;
		}
		else if (EntitiesExtensions.TryGetBuffer<LabelPosition>(entityManager, location, true, ref labelPositions))
		{
			position = GetAggregatePosition(labelPositions, ref elementIndex);
			bounds = new Bounds3(position, position);
			rotation = quaternion.identity;
		}
		else if (EntitiesExtensions.TryGetComponent<Geometry>(entityManager, location, ref geometry))
		{
			position = geometry.m_CenterPosition;
			bounds = new Bounds3(position, position);
			rotation = quaternion.identity;
		}
		else if (EntitiesExtensions.TryGetComponent<Icon>(entityManager, location, ref icon))
		{
			position = icon.m_Location;
			bounds = new Bounds3(position, position);
			rotation = quaternion.identity;
		}
		else if (EntitiesExtensions.TryGetComponent<Game.Net.Node>(entityManager, location, ref node))
		{
			position = GetNodePosition(location, entityManager, node, out bounds, out rotation);
		}
		else
		{
			Curve curve = default(Curve);
			if (!EntitiesExtensions.TryGetComponent<Curve>(entityManager, location, ref curve))
			{
				position = float3.zero;
				bounds = default(Bounds3);
				rotation = quaternion.identity;
				return false;
			}
			position = GetCurvePosition(location, entityManager, curve, out bounds, out rotation);
		}
		return true;
	}

	private static Transform GetInterpolatedPosition(Entity entity, EntityManager entityManager, DynamicBuffer<TransformFrame> transformFrames, bool reinterpolate, out Bounds3 bounds)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		InterpolatedTransform interpolatedTransform;
		if (!reinterpolate && IsNearCamera(entity, entityManager))
		{
			interpolatedTransform = ((EntityManager)(ref entityManager)).GetComponentData<InterpolatedTransform>(entity);
		}
		else
		{
			RenderingSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<RenderingSystem>();
			UpdateFrame sharedComponent = ((EntityManager)(ref entityManager)).GetSharedComponent<UpdateFrame>(entity);
			ObjectInterpolateSystem.CalculateUpdateFrames(orCreateSystemManaged.frameIndex, orCreateSystemManaged.frameTime, sharedComponent.m_Index, out var updateFrame, out var updateFrame2, out var framePosition);
			TransformFrame frame = transformFrames[(int)updateFrame];
			TransformFrame frame2 = transformFrames[(int)updateFrame2];
			interpolatedTransform = ObjectInterpolateSystem.CalculateTransform(frame, frame2, framePosition);
		}
		return GetObjectPosition(entity, entityManager, interpolatedTransform.ToTransform(), out bounds);
	}

	private static Transform GetRelativePosition(Entity entity, EntityManager entityManager, Relative relative, bool reinterpolate, out Bounds3 bounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((EntityManager)(ref entityManager)).GetComponentData<Transform>(entity);
		Entity val = Entity.Null;
		CurrentVehicle currentVehicle = default(CurrentVehicle);
		Owner owner = default(Owner);
		if (EntitiesExtensions.TryGetComponent<CurrentVehicle>(entityManager, entity, ref currentVehicle))
		{
			val = currentVehicle.m_Vehicle;
		}
		else if (EntitiesExtensions.TryGetComponent<Owner>(entityManager, entity, ref owner))
		{
			val = owner.m_Owner;
		}
		DynamicBuffer<TransformFrame> transformFrames = default(DynamicBuffer<TransformFrame>);
		Transform interpolatedPosition = default(Transform);
		if (EntitiesExtensions.TryGetBuffer<TransformFrame>(entityManager, val, true, ref transformFrames))
		{
			interpolatedPosition = GetInterpolatedPosition(val, entityManager, transformFrames, reinterpolate, out var _);
			transform = ObjectUtils.LocalToWorld(interpolatedPosition, relative.ToTransform());
		}
		else if (EntitiesExtensions.TryGetComponent<Transform>(entityManager, val, ref interpolatedPosition))
		{
			transform = ObjectUtils.LocalToWorld(interpolatedPosition, relative.ToTransform());
		}
		return GetObjectPosition(entity, entityManager, transform, out bounds);
	}

	private static bool IsNearCamera(Entity entity, EntityManager entityManager)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		CullingInfo cullingInfo = default(CullingInfo);
		if (EntitiesExtensions.TryGetComponent<CullingInfo>(entityManager, entity, ref cullingInfo) && cullingInfo.m_CullingIndex != 0)
		{
			JobHandle dependencies;
			NativeList<PreCullingData> cullingData = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PreCullingSystem>().GetCullingData(readOnly: true, out dependencies);
			((JobHandle)(ref dependencies)).Complete();
			return (cullingData[cullingInfo.m_CullingIndex].m_Flags & PreCullingFlags.NearCamera) != 0;
		}
		return false;
	}

	private static Transform GetObjectPosition(Entity entity, EntityManager entityManager, Transform transform, out Bounds3 bounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		bounds = new Bounds3(transform.m_Position, transform.m_Position);
		PrefabRef prefabRef = default(PrefabRef);
		ObjectGeometryData geometryData = default(ObjectGeometryData);
		if (EntitiesExtensions.TryGetComponent<PrefabRef>(entityManager, entity, ref prefabRef) && EntitiesExtensions.TryGetComponent<ObjectGeometryData>(entityManager, prefabRef.m_Prefab, ref geometryData))
		{
			bounds = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, geometryData);
		}
		return transform;
	}

	private static float3 GetNodePosition(Entity entity, EntityManager entityManager, Game.Net.Node node, out Bounds3 bounds, out quaternion rotation)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		bounds = new Bounds3(node.m_Position, node.m_Position);
		PrefabRef prefabRef = default(PrefabRef);
		NetGeometryData netGeometryData = default(NetGeometryData);
		if (EntitiesExtensions.TryGetComponent<PrefabRef>(entityManager, entity, ref prefabRef) && EntitiesExtensions.TryGetComponent<NetGeometryData>(entityManager, prefabRef.m_Prefab, ref netGeometryData))
		{
			((Bounds3)(ref bounds)).y = node.m_Position.y + netGeometryData.m_DefaultSurfaceHeight;
		}
		rotation = node.m_Rotation;
		return node.m_Position;
	}

	private static float3 GetCurvePosition(Entity entity, EntityManager entityManager, Curve curve, out Bounds3 bounds, out quaternion rotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		float3 val = MathUtils.Position(curve.m_Bezier, 0.5f);
		bounds = new Bounds3(val, val);
		rotation = quaternion.Euler(MathUtils.Tangent(curve.m_Bezier, 0.5f), (RotationOrder)4);
		PrefabRef prefabRef = default(PrefabRef);
		NetGeometryData netGeometryData = default(NetGeometryData);
		if (EntitiesExtensions.TryGetComponent<PrefabRef>(entityManager, entity, ref prefabRef) && EntitiesExtensions.TryGetComponent<NetGeometryData>(entityManager, prefabRef.m_Prefab, ref netGeometryData))
		{
			((Bounds3)(ref bounds)).y = val.y + netGeometryData.m_DefaultSurfaceHeight;
		}
		return val;
	}

	private static float3 GetRoutePosition(EntityManager entityManager, DynamicBuffer<RouteWaypoint> routeWaypoints)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		float3 result = default(float3);
		float3 val = float3.op_Implicit((Vector3)(((Object)(object)s_CameraController != (Object)null) ? s_CameraController.pivot : default(Vector3)));
		float num = float.MaxValue;
		Position position = default(Position);
		for (int i = 0; i < routeWaypoints.Length; i++)
		{
			if (EntitiesExtensions.TryGetComponent<Position>(entityManager, routeWaypoints[i].m_Waypoint, ref position))
			{
				float num2 = math.distancesq(position.m_Position, val);
				if (!(num2 >= num))
				{
					result = position.m_Position;
					num = num2;
				}
			}
		}
		return result;
	}

	private static float3 GetAggregatePosition(DynamicBuffer<LabelPosition> labelPositions, ref int selectedIndex)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		float3 result = default(float3);
		float3 val = float3.op_Implicit((Vector3)(((Object)(object)s_CameraController != (Object)null) ? s_CameraController.pivot : default(Vector3)));
		float num = float.MaxValue;
		int num2 = -1;
		for (int i = 0; i < labelPositions.Length; i++)
		{
			LabelPosition labelPosition = labelPositions[i];
			float3 val2 = MathUtils.Position(labelPosition.m_Curve, 0.5f);
			float num3 = math.distancesq(val2, val);
			if (labelPosition.m_ElementIndex == selectedIndex)
			{
				return val2;
			}
			if (!(num3 >= num))
			{
				result = val2;
				num = num3;
				num2 = labelPosition.m_ElementIndex;
			}
		}
		selectedIndex = num2;
		return result;
	}

	private static void FilterPositionTarget(ref Entity entity, EntityManager entityManager)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<TargetElement> val = default(DynamicBuffer<TargetElement>);
		if (EntitiesExtensions.TryGetBuffer<TargetElement>(entityManager, entity, true, ref val) && val.Length != 0)
		{
			entity = val[0].m_Entity;
		}
		CurrentTransport currentTransport = default(CurrentTransport);
		if (EntitiesExtensions.TryGetComponent<CurrentTransport>(entityManager, entity, ref currentTransport))
		{
			entity = currentTransport.m_CurrentTransport;
		}
		if (((EntityManager)(ref entityManager)).HasComponent<Unspawned>(entity))
		{
			CurrentVehicle currentVehicle = default(CurrentVehicle);
			Game.Creatures.Resident resident = default(Game.Creatures.Resident);
			CurrentBuilding currentBuilding = default(CurrentBuilding);
			Game.Creatures.Pet pet = default(Game.Creatures.Pet);
			CurrentBuilding currentBuilding2 = default(CurrentBuilding);
			if (EntitiesExtensions.TryGetComponent<CurrentVehicle>(entityManager, entity, ref currentVehicle))
			{
				entity = currentVehicle.m_Vehicle;
			}
			else if (EntitiesExtensions.TryGetComponent<Game.Creatures.Resident>(entityManager, entity, ref resident) && EntitiesExtensions.TryGetComponent<CurrentBuilding>(entityManager, resident.m_Citizen, ref currentBuilding))
			{
				entity = currentBuilding.m_CurrentBuilding;
			}
			else if (EntitiesExtensions.TryGetComponent<Game.Creatures.Pet>(entityManager, entity, ref pet) && EntitiesExtensions.TryGetComponent<CurrentBuilding>(entityManager, pet.m_HouseholdPet, ref currentBuilding2))
			{
				entity = currentBuilding2.m_CurrentBuilding;
			}
		}
		if (((EntityManager)(ref entityManager)).HasComponent<Unspawned>(entity))
		{
			HumanCurrentLane humanCurrentLane = default(HumanCurrentLane);
			AnimalCurrentLane animalCurrentLane = default(AnimalCurrentLane);
			CarCurrentLane carCurrentLane = default(CarCurrentLane);
			ParkedCar parkedCar = default(ParkedCar);
			TrainCurrentLane trainCurrentLane = default(TrainCurrentLane);
			ParkedTrain parkedTrain = default(ParkedTrain);
			AircraftCurrentLane aircraftCurrentLane = default(AircraftCurrentLane);
			WatercraftCurrentLane watercraftCurrentLane = default(WatercraftCurrentLane);
			if (EntitiesExtensions.TryGetComponent<HumanCurrentLane>(entityManager, entity, ref humanCurrentLane))
			{
				FilterPositionTarget(out entity, humanCurrentLane.m_Lane, entityManager);
			}
			else if (EntitiesExtensions.TryGetComponent<AnimalCurrentLane>(entityManager, entity, ref animalCurrentLane))
			{
				FilterPositionTarget(out entity, animalCurrentLane.m_Lane, entityManager);
			}
			else if (EntitiesExtensions.TryGetComponent<CarCurrentLane>(entityManager, entity, ref carCurrentLane))
			{
				FilterPositionTarget(out entity, carCurrentLane.m_Lane, entityManager);
			}
			else if (EntitiesExtensions.TryGetComponent<ParkedCar>(entityManager, entity, ref parkedCar))
			{
				FilterPositionTarget(out entity, parkedCar.m_Lane, entityManager);
			}
			else if (EntitiesExtensions.TryGetComponent<TrainCurrentLane>(entityManager, entity, ref trainCurrentLane))
			{
				FilterPositionTarget(out entity, trainCurrentLane.m_Front.m_Lane, entityManager);
			}
			else if (EntitiesExtensions.TryGetComponent<ParkedTrain>(entityManager, entity, ref parkedTrain))
			{
				FilterPositionTarget(out entity, parkedTrain.m_FrontLane, entityManager);
			}
			else if (EntitiesExtensions.TryGetComponent<AircraftCurrentLane>(entityManager, entity, ref aircraftCurrentLane))
			{
				FilterPositionTarget(out entity, aircraftCurrentLane.m_Lane, entityManager);
			}
			else if (EntitiesExtensions.TryGetComponent<WatercraftCurrentLane>(entityManager, entity, ref watercraftCurrentLane))
			{
				FilterPositionTarget(out entity, watercraftCurrentLane.m_Lane, entityManager);
			}
		}
		CurrentBuilding currentBuilding3 = default(CurrentBuilding);
		if (EntitiesExtensions.TryGetComponent<CurrentBuilding>(entityManager, entity, ref currentBuilding3))
		{
			entity = currentBuilding3.m_CurrentBuilding;
		}
		PropertyRenter propertyRenter = default(PropertyRenter);
		if (EntitiesExtensions.TryGetComponent<PropertyRenter>(entityManager, entity, ref propertyRenter))
		{
			entity = propertyRenter.m_Property;
		}
		TouristHousehold touristHousehold = default(TouristHousehold);
		if (EntitiesExtensions.TryGetComponent<TouristHousehold>(entityManager, entity, ref touristHousehold))
		{
			entity = touristHousehold.m_Hotel;
		}
	}

	private static void FilterPositionTarget(out Entity entity, Entity location, EntityManager entityManager)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		entity = Entity.Null;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Objects.Object>(location))
		{
			entity = location;
		}
		Owner owner = default(Owner);
		while (EntitiesExtensions.TryGetComponent<Owner>(entityManager, location, ref owner))
		{
			location = owner.m_Owner;
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Objects.Object>(location))
			{
				entity = location;
			}
		}
		DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
		if (!((EntityManager)(ref entityManager)).HasComponent<Game.Net.OutsideConnection>(location) || !EntitiesExtensions.TryGetBuffer<Game.Objects.SubObject>(entityManager, location, true, ref val))
		{
			return;
		}
		for (int i = 0; i < val.Length; i++)
		{
			Entity subObject = val[i].m_SubObject;
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Objects.OutsideConnection>(subObject))
			{
				entity = subObject;
			}
		}
	}

	private void WriteDeveloperSection(IJsonWriter binder)
	{
		if (m_DebugUISystem.developerInfoVisible)
		{
			m_DeveloperSection.Write(binder);
		}
		else
		{
			binder.WriteNull();
		}
	}

	private void WriteSections(List<ISectionSource> list, IJsonWriter binder)
	{
		JsonWriterExtensions.ArrayBegin(binder, list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			JsonWriterExtensions.Write<ISectionSource>(binder, list[i]);
		}
		binder.ArrayEnd();
	}

	private void WriteTooltipFlags(IJsonWriter writer)
	{
		JsonWriterExtensions.ArrayBegin(writer, tooltipTags.Count);
		for (int i = 0; i < tooltipTags.Count; i++)
		{
			writer.Write(tooltipTags[i].ToString());
		}
		writer.ArrayEnd();
	}

	[Preserve]
	public SelectedInfoUISystem()
	{
	}
}
