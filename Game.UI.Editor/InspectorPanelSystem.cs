using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Colossal;
using Colossal.Annotations;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.Localization;
using Colossal.Mathematics;
using Colossal.PSI.Common;
using Colossal.PSI.PdxSdk;
using Colossal.UI;
using Game.Achievements;
using Game.Common;
using Game.Input;
using Game.Objects;
using Game.Prefabs;
using Game.Reflection;
using Game.Rendering;
using Game.SceneFlow;
using Game.Tools;
using Game.UI.InGame;
using Game.UI.Localization;
using Game.UI.Widgets;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Editor;

[CompilerGenerated]
public class InspectorPanelSystem : EditorPanelSystemBase
{
	private enum Mode
	{
		Instance,
		Prefab
	}

	private struct LocalizationFields
	{
		public LocalizationField m_NameLocalization;

		public LocalizationField m_DescriptionLocalization;

		public LocalizationFields Clone()
		{
			LocalizationField localizationField = new LocalizationField(m_NameLocalization.placeholder);
			localizationField.Initialize(m_NameLocalization.localization);
			LocalizationField localizationField2 = new LocalizationField(m_DescriptionLocalization.placeholder);
			localizationField2.Initialize(m_DescriptionLocalization.localization);
			return new LocalizationFields
			{
				m_NameLocalization = localizationField,
				m_DescriptionLocalization = localizationField2
			};
		}
	}

	private PrefabSystem m_PrefabSystem;

	private ToolSystem m_ToolSystem;

	private ObjectToolSystem m_ObjectTool;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private EditorAssetUploadPanel m_AssetUploadPanel;

	private ProxyAction m_MoveAction;

	private ProxyAction m_CloneAction;

	private ProxyAction m_AutoAlignAction;

	private ProxyAction m_AutoConnectAction;

	private ProxyAction m_AlignXAction;

	private ProxyAction m_AlignYAction;

	private ProxyAction m_AlignZAction;

	private EditorGenerator m_EditorGenerator = new EditorGenerator();

	private Button[] m_MeshFooter;

	private Button[] m_InstanceFooter;

	private Button[] m_PrefabFooter;

	private Button[] m_CustomAssetFooter;

	private Entity m_CurrentSelectedEntity;

	[CanBeNull]
	private object m_SelectedObject;

	[CanBeNull]
	private object m_ParentObject;

	[CanBeNull]
	private ObjectSubObjectInfo m_LastSubObject;

	[CanBeNull]
	private ObjectSubObjectInfo m_CurrentSubObject;

	[CanBeNull]
	private LocalizedString m_SelectedName = null;

	[CanBeNull]
	private LocalizedString m_ParentName = null;

	private List<object> m_SectionObjects = new List<object>();

	private Dictionary<PrefabBase, LocalizationFields> m_WipLocalization = new Dictionary<PrefabBase, LocalizationFields>();

	private PdxSdkPlatform m_Platform;

	private Mode mode
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			if (!(m_CurrentSelectedEntity == Entity.Null) || !(m_SelectedObject is PrefabBase))
			{
				return Mode.Instance;
			}
			return Mode.Prefab;
		}
	}

	private bool canMoveSelected
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<PrefabRef>(m_CurrentSelectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Game.Objects.Object>(m_CurrentSelectedEntity))
				{
					return m_CurrentSelectedEntity == m_ToolSystem.selected;
				}
			}
			return false;
		}
	}

	private bool canCloneSelected
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<PrefabRef>(m_CurrentSelectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				return ((EntityManager)(ref entityManager)).HasComponent<Game.Objects.Object>(m_CurrentSelectedEntity);
			}
			return false;
		}
	}

	private bool canAlignSelected
	{
		get
		{
			if (m_CurrentSubObject != null)
			{
				return m_LastSubObject != null;
			}
			return false;
		}
	}

	private bool DisableSection(object obj, object parent)
	{
		if (obj is ComponentBase componentBase && componentBase.prefab.builtin)
		{
			return true;
		}
		if ((obj is ObjectMeshInfo || obj is ObjectSubObjectInfo) && parent is ComponentBase componentBase2 && componentBase2.prefab.builtin)
		{
			return true;
		}
		return false;
	}

	private static bool IsBuiltinAsset(AssetData asset)
	{
		if (asset != (IAssetData)null && asset.database != null)
		{
			return asset.database is AssetDatabase<Game>;
		}
		return false;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Expected O, but got Unknown
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_ObjectTool = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ObjectToolSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_AssetUploadPanel = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EditorAssetUploadPanel>();
		m_MoveAction = InputManager.instance.FindAction("Editor", "Move Selected");
		m_CloneAction = InputManager.instance.FindAction("Editor", "Clone");
		m_AutoAlignAction = InputManager.instance.FindAction("Editor", "Auto Align");
		m_AutoConnectAction = InputManager.instance.FindAction("Editor", "Auto Connect");
		m_AlignXAction = InputManager.instance.FindAction("Editor", "Align X");
		m_AlignYAction = InputManager.instance.FindAction("Editor", "Align Y");
		m_AlignZAction = InputManager.instance.FindAction("Editor", "Align Z");
		m_InstanceFooter = new Button[1]
		{
			new Button
			{
				displayName = "Editor.LOCATE",
				action = OnLocate
			}
		};
		m_PrefabFooter = new Button[2]
		{
			new Button
			{
				displayName = "Editor.DUPLICATE_TEMPLATE",
				action = OnDuplicate,
				tooltip = "Editor.DUPLICATE_TEMPLATE_TOOLTIP"
			},
			new Button
			{
				displayName = "Editor.ADD_COMPONENT",
				tooltip = "Editor.ADD_COMPONENT_TOOLTIP",
				action = ShowAddComponentPicker,
				disabled = () => DisableSection(m_SelectedObject, m_ParentObject)
			}
		};
		m_MeshFooter = new Button[1]
		{
			new Button
			{
				displayName = "Editor.ADD_COMPONENT",
				tooltip = "Editor.ADD_COMPONENT_TOOLTIP",
				action = ShowAddComponentPicker,
				disabled = () => DisableSection(m_SelectedObject, m_ParentObject)
			}
		};
		m_CustomAssetFooter = new Button[1]
		{
			new Button
			{
				displayName = "Editor.SAVE_ASSET",
				tooltip = "Editor.SAVE_ASSET_TOOLTIP",
				action = ShowSaveAssetPanel,
				disabled = () => DisableSection(m_SelectedObject, m_ParentObject)
			}
		};
		m_Platform = PlatformManager.instance.GetPSI<PdxSdkPlatform>("PdxSdk");
		PlatformManager.instance.onPlatformRegistered += (PlatformRegisteredHandler)delegate(IPlatformServiceIntegration psi)
		{
			PdxSdkPlatform val = (PdxSdkPlatform)(object)((psi is PdxSdkPlatform) ? psi : null);
			if (val != null)
			{
				m_Platform = val;
			}
		};
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		base.activeSubPanel = null;
		m_MoveAction.enabled = false;
		m_CloneAction.enabled = false;
		m_AutoAlignAction.enabled = false;
		m_AutoAlignAction.enabled = false;
		m_AutoConnectAction.enabled = false;
		m_AlignXAction.enabled = false;
		m_AlignYAction.enabled = false;
		m_AlignZAction.enabled = false;
		OnColorVariationChanged(Entity.Null, null, -1, -1);
		OnEmissiveChanged(Entity.Null, null, -1, -1);
		((COSystemBase)this).OnStopRunning();
	}

	protected override void OnValueChanged(IWidget widget)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		base.OnValueChanged(widget);
		if (IsColorVariationField(widget, out var variationSetIndex, out var colorIndex, out var mesh))
		{
			OnColorVariationChanged(m_ToolSystem.selected, mesh, variationSetIndex, colorIndex);
		}
		if (IsEmissiveField(widget, out var singleLightIndex, out var multiLightIndex, out mesh))
		{
			OnEmissiveChanged(m_ToolSystem.selected, mesh, singleLightIndex, multiLightIndex);
		}
		UpdateParent(moveSubObjects: true);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.OnUpdate();
		RefreshContent();
		HandleInput();
	}

	public bool SelectEntity(Entity entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (entity == m_CurrentSelectedEntity)
		{
			return entity != Entity.Null;
		}
		object obj = m_SelectedObject;
		object obj2 = m_ParentObject;
		base.activeSubPanel = null;
		if (SelectObjectForEntity(entity))
		{
			m_CurrentSelectedEntity = entity;
			if (obj2 == m_ParentObject && obj is ObjectSubObjectInfo lastSubObject && m_SelectedObject is ObjectSubObjectInfo currentSubObject)
			{
				m_LastSubObject = lastSubObject;
				m_CurrentSubObject = currentSubObject;
			}
			else
			{
				m_LastSubObject = null;
				m_CurrentSubObject = null;
			}
			return true;
		}
		m_CurrentSelectedEntity = Entity.Null;
		m_LastSubObject = null;
		m_CurrentSubObject = null;
		return false;
	}

	public void SelectPrefab(PrefabBase prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_CurrentSelectedEntity = Entity.Null;
		m_LastSubObject = null;
		m_CurrentSubObject = null;
		m_SelectedObject = prefab;
		m_ParentObject = prefab;
	}

	[Conditional("UNITY_EDITOR")]
	private void SelectInUnityEditor(Object obj)
	{
	}

	private bool SelectObjectForEntity(Entity entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		if (!(entity == Entity.Null))
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Objects.Object>(entity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				PrefabRef refData = default(PrefabRef);
				if (!((EntityManager)(ref entityManager)).HasComponent<Secondary>(entity) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, entity, ref refData))
				{
					if (!m_PrefabSystem.TryGetPrefab<PrefabBase>(refData, out var prefab))
					{
						m_SelectedObject = null;
						m_ParentObject = null;
						return false;
					}
					m_SelectedObject = prefab;
					m_ParentObject = prefab;
					Owner owner = default(Owner);
					PrefabRef refData2 = default(PrefabRef);
					Game.Tools.EditorContainer editorContainer2 = default(Game.Tools.EditorContainer);
					if (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, entity, ref owner) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, owner.m_Owner, ref refData2))
					{
						int num = -1;
						LocalTransformCache localTransformCache = default(LocalTransformCache);
						if (EntitiesExtensions.TryGetComponent<LocalTransformCache>(((ComponentSystemBase)this).EntityManager, entity, ref localTransformCache))
						{
							num = localTransformCache.m_PrefabSubIndex;
						}
						if (num == -1)
						{
							return false;
						}
						PrefabBase prefab2 = m_PrefabSystem.GetPrefab<PrefabBase>(refData2);
						Game.Tools.EditorContainer editorContainer = default(Game.Tools.EditorContainer);
						ObjectSubObjects component3;
						if (EntitiesExtensions.TryGetComponent<Game.Tools.EditorContainer>(((ComponentSystemBase)this).EntityManager, entity, ref editorContainer))
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							if (((EntityManager)(ref entityManager)).HasComponent<EffectData>(editorContainer.m_Prefab) && prefab2.TryGet<EffectSource>(out var component) && component.m_Effects != null && component.m_Effects.Count > num)
							{
								prefab = m_PrefabSystem.GetPrefab<PrefabBase>(editorContainer.m_Prefab);
								EffectSource.EffectSettings effectSettings = component.m_Effects[num];
								if (effectSettings != null && (Object)(object)effectSettings.m_Effect == (Object)(object)prefab)
								{
									m_SelectedObject = effectSettings;
									m_ParentObject = prefab2;
								}
							}
							else
							{
								entityManager = ((ComponentSystemBase)this).EntityManager;
								if (((EntityManager)(ref entityManager)).HasComponent<ActivityLocationData>(editorContainer.m_Prefab) && prefab2.TryGet<Game.Prefabs.ActivityLocation>(out var component2) && component2.m_Locations != null && component2.m_Locations.Length > num)
								{
									prefab = m_PrefabSystem.GetPrefab<PrefabBase>(editorContainer.m_Prefab);
									Game.Prefabs.ActivityLocation.LocationInfo locationInfo = component2.m_Locations[num];
									if (locationInfo != null && (Object)(object)locationInfo.m_Activity == (Object)(object)prefab)
									{
										m_SelectedObject = locationInfo;
										m_ParentObject = prefab2;
									}
								}
							}
						}
						else if (prefab2.TryGet<ObjectSubObjects>(out component3) && component3.m_SubObjects != null && component3.m_SubObjects.Length > num)
						{
							ObjectSubObjectInfo objectSubObjectInfo = component3.m_SubObjects[num];
							if (objectSubObjectInfo != null && (Object)(object)objectSubObjectInfo.m_Object == (Object)(object)prefab)
							{
								m_SelectedObject = objectSubObjectInfo;
								m_ParentObject = prefab2;
							}
						}
					}
					else if (EntitiesExtensions.TryGetComponent<Game.Tools.EditorContainer>(((ComponentSystemBase)this).EntityManager, entity, ref editorContainer2) && m_PrefabSystem.TryGetPrefab<PrefabBase>(editorContainer2.m_Prefab, out prefab))
					{
						m_SelectedObject = prefab;
						m_ParentObject = prefab;
					}
					return true;
				}
			}
		}
		m_SelectedObject = null;
		m_ParentObject = null;
		return false;
	}

	public bool SelectMesh(Entity entity, int meshIndex)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		PrefabRef prefabRef = default(PrefabRef);
		DynamicBuffer<SubMesh> val = default(DynamicBuffer<SubMesh>);
		if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, entity, ref prefabRef) && m_PrefabSystem.TryGetPrefab<PrefabBase>(prefabRef.m_Prefab, out var prefab) && EntitiesExtensions.TryGetBuffer<SubMesh>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, true, ref val) && m_PrefabSystem.TryGetPrefab<PrefabBase>(val[meshIndex].m_SubMesh, out var prefab2))
		{
			base.activeSubPanel = null;
			m_SelectedObject = prefab2;
			m_ParentObject = prefab;
			m_CurrentSelectedEntity = Entity.Null;
			m_LastSubObject = null;
			m_CurrentSubObject = null;
			if (prefab is ObjectGeometryPrefab prefab3 && FindObjectMeshInfo(prefab3, prefab2, out var info))
			{
				m_SelectedObject = info;
			}
			return true;
		}
		return false;
	}

	private bool FindObjectMeshInfo(ObjectGeometryPrefab prefab, PrefabBase meshPrefab, out ObjectMeshInfo info)
	{
		for (int i = 0; i < prefab.m_Meshes.Length; i++)
		{
			if ((Object)(object)prefab.m_Meshes[i].m_Mesh == (Object)(object)meshPrefab)
			{
				info = prefab.m_Meshes[i];
				return true;
			}
		}
		info = null;
		return false;
	}

	private void RefreshContent()
	{
		RefreshTitle();
		RefreshSections();
	}

	private void RefreshTitle()
	{
		LocalizedString objectName = GetObjectName(m_SelectedObject);
		LocalizedString objectName2 = GetObjectName(m_ParentObject);
		if (!objectName.Equals(m_SelectedName) || !objectName2.Equals(m_ParentName))
		{
			m_SelectedName = objectName;
			m_ParentName = objectName2;
			if (m_ParentObject != m_SelectedObject)
			{
				title = objectName2.value + " > " + objectName.value;
			}
			else
			{
				title = objectName;
			}
		}
	}

	[CanBeNull]
	private LocalizedString GetObjectName([CanBeNull] object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is PrefabBase prefab)
		{
			return EditorPrefabUtils.GetPrefabLabel(prefab);
		}
		return LocalizedString.Value(m_SelectedObject.GetType().Name);
	}

	private void RefreshSections()
	{
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		if (m_SectionObjects.SequenceEqual(GetSectionObjects()))
		{
			return;
		}
		m_SectionObjects.Clear();
		m_SectionObjects.AddRange(GetSectionObjects());
		List<IWidget> list = new List<IWidget>(m_SectionObjects.Count);
		for (int i = 0; i < m_SectionObjects.Count; i++)
		{
			object obj = m_SectionObjects[i];
			string name = obj.GetType().Name;
			List<IWidget> widgets = new List<IWidget>(m_EditorGenerator.BuildMembers(new ObjectAccessor<object>(obj), 0, name).ToArray());
			PrefabBase prefabBase = obj as PrefabBase;
			if ((Object)(object)prefabBase != (Object)null && !(prefabBase is RenderPrefabBase))
			{
				BuildLocalizationFields(prefabBase, widgets);
			}
			EditorSection editorSection = new EditorSection
			{
				path = new PathSegment(name),
				displayName = LocalizedString.Value(WidgetReflectionUtils.NicifyVariableName(name)),
				tooltip = name,
				expanded = true,
				children = widgets
			};
			if (DisableSection(obj, m_ParentObject))
			{
				DisableAllFields(editorSection);
			}
			if ((Object)(object)prefabBase != (Object)null)
			{
				editorSection.primary = true;
				editorSection.color = EditorSection.kPrefabColor;
				editorSection.active = GetActiveAccessor(prefabBase);
			}
			else
			{
				ComponentBase component = obj as ComponentBase;
				if (component != null)
				{
					editorSection.onDelete = delegate
					{
						ApplyPrefabsSystem.RemoveComponent(component.prefab, ((object)component).GetType());
					};
					editorSection.active = GetActiveAccessor(component);
				}
			}
			list.Add(editorSection);
		}
		List<IWidget> list2 = new List<IWidget>
		{
			Scrollable.WithChildren(list.ToArray()),
			new ModdingBetaBanner()
		};
		RefreshFooter(list2);
		children = list2.ToArray();
	}

	private void RefreshFooter(IList<IWidget> panelChildren)
	{
		if (m_SelectedObject is ObjectMeshInfo)
		{
			panelChildren.Add(ButtonRow.WithChildren(m_MeshFooter));
			return;
		}
		List<Button> list = new List<Button>();
		if (mode == Mode.Instance)
		{
			list.AddRange(m_InstanceFooter);
		}
		list.AddRange(m_PrefabFooter);
		panelChildren.Add(ButtonRow.WithChildren(list.ToArray()));
		if (m_SelectedObject is PrefabBase { builtin: false })
		{
			panelChildren.Add(ButtonRow.WithChildren(m_CustomAssetFooter));
		}
	}

	public static void DisableAllFields(IWidget widget)
	{
		if (widget is IDisableCallback disableCallback)
		{
			disableCallback.disabled = () => true;
		}
		if (widget is IContainerWidget containerWidget)
		{
			{
				foreach (IWidget child in containerWidget.children)
				{
					DisableAllFields(child);
				}
				return;
			}
		}
		if (widget is ButtonRow { children: var array })
		{
			for (int num = 0; num < array.Length; num++)
			{
				DisableAllFields(array[num]);
			}
		}
	}

	private IEnumerable<object> GetSectionObjects()
	{
		if (m_SelectedObject == null)
		{
			yield break;
		}
		yield return m_SelectedObject;
		object obj = m_SelectedObject;
		if (obj is PrefabBase prefab)
		{
			for (int i = 0; i < prefab.components.Count; i++)
			{
				if (((object)prefab.components[i]).GetType().GetCustomAttribute<HideInEditorAttribute>() == null)
				{
					yield return prefab.components[i];
				}
			}
		}
		obj = m_SelectedObject;
		if (!(obj is ObjectMeshInfo meshInfo))
		{
			yield break;
		}
		yield return meshInfo.m_Mesh;
		for (int i = 0; i < meshInfo.m_Mesh.components.Count; i++)
		{
			if (((object)meshInfo.m_Mesh.components[i]).GetType().GetCustomAttribute<HideInEditorAttribute>() == null)
			{
				yield return meshInfo.m_Mesh.components[i];
			}
		}
	}

	private static ITypedValueAccessor<bool> GetActiveAccessor(ComponentBase component)
	{
		return new DelegateAccessor<bool>(() => component.active, delegate(bool value)
		{
			component.active = value;
		});
	}

	private void ShowAddComponentPicker()
	{
		base.activeSubPanel = new TypePickerPanel(new LocalizedString("Editor.ADD_COMPONENT_NAMED", null, new Dictionary<string, ILocElement> { { "NAME", m_SelectedName } }), "Editor.COMPONENT_TYPES", GetComponentTypeItems().ToList(), OnAddComponent, base.CloseSubPanel);
	}

	private void OnDuplicate()
	{
		if (m_SelectedObject is PrefabBase template)
		{
			PrefabBase prefab = m_PrefabSystem.DuplicatePrefab(template);
			if (mode == Mode.Instance)
			{
				m_ToolSystem.ActivatePrefabTool(prefab);
			}
			else
			{
				SelectPrefab(prefab);
			}
		}
	}

	private void OnAddComponent(Type type)
	{
		CloseSubPanel();
		PrefabBase prefabBase = null;
		if (m_SelectedObject is PrefabBase prefabBase2)
		{
			prefabBase = prefabBase2;
		}
		else if (m_SelectedObject is ObjectMeshInfo objectMeshInfo)
		{
			prefabBase = objectMeshInfo.m_Mesh;
		}
		if ((Object)(object)prefabBase != (Object)null && !prefabBase.Has(type))
		{
			prefabBase.AddComponent(type);
		}
	}

	private IEnumerable<Item> GetComponentTypeItems()
	{
		object obj = m_SelectedObject;
		if (m_SelectedObject is ObjectMeshInfo objectMeshInfo)
		{
			obj = objectMeshInfo.m_Mesh;
		}
		if (!(obj is PrefabBase selectedPrefab))
		{
			yield break;
		}
		foreach (Type item in TypePickerPanel.GetAllConcreteTypesDerivedFrom<ComponentBase>())
		{
			if (item.IsSubclassOf(typeof(PrefabBase)) || selectedPrefab.Has(item))
			{
				continue;
			}
			Type prefabType = ((object)selectedPrefab).GetType();
			if (!(prefabType == item) && item.GetCustomAttribute<HideInEditorAttribute>() == null)
			{
				ComponentMenu customAttribute = item.GetCustomAttribute<ComponentMenu>();
				if (customAttribute?.requiredPrefab == null || customAttribute == null || customAttribute.requiredPrefab.Length == 0 || customAttribute.requiredPrefab.Any((Type t) => t.IsAssignableFrom(prefabType)))
				{
					yield return new Item
					{
						type = item,
						name = WidgetReflectionUtils.NicifyVariableName(item.Name),
						parentDir = customAttribute?.menu
					};
				}
			}
		}
	}

	private void HandleInput()
	{
		m_MoveAction.enabled = canMoveSelected;
		if (m_MoveAction.WasPerformedThisFrame())
		{
			MoveSelected();
		}
		m_CloneAction.enabled = canCloneSelected;
		if (m_CloneAction.WasPerformedThisFrame())
		{
			CloneSelected();
		}
		m_AutoAlignAction.enabled = canAlignSelected;
		if (m_AutoAlignAction.WasPerformedThisFrame())
		{
			AutoAlign();
		}
		m_AutoConnectAction.enabled = canAlignSelected;
		if (m_AutoConnectAction.WasPerformedThisFrame())
		{
			AutoConnect();
		}
		m_AlignXAction.enabled = canAlignSelected;
		if (m_AlignXAction.WasPerformedThisFrame())
		{
			AlignX();
		}
		m_AlignYAction.enabled = canAlignSelected;
		if (m_AlignYAction.WasPerformedThisFrame())
		{
			AlignY();
		}
		m_AlignZAction.enabled = canAlignSelected;
		if (m_AlignZAction.WasPerformedThisFrame())
		{
			AlignZ();
		}
	}

	private void MoveSelected()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_ObjectTool.StartMoving(m_CurrentSelectedEntity);
		m_ToolSystem.activeTool = m_ObjectTool;
	}

	private void CloneSelected()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(m_CurrentSelectedEntity);
		ObjectPrefab prefab = m_PrefabSystem.GetPrefab<ObjectPrefab>(componentData);
		m_ObjectTool.mode = ObjectToolSystem.Mode.Create;
		m_ObjectTool.prefab = prefab;
		m_ToolSystem.activeTool = m_ObjectTool;
		if (m_CurrentSelectedEntity == m_ToolSystem.selected)
		{
			m_ToolSystem.selected = Entity.Null;
		}
	}

	private void AutoAlign()
	{
		if (m_CurrentSubObject != null && m_LastSubObject != null)
		{
			float num = Mathf.Abs(m_CurrentSubObject.m_Position.x - m_LastSubObject.m_Position.x);
			float num2 = Mathf.Abs(m_CurrentSubObject.m_Position.y - m_LastSubObject.m_Position.y);
			float num3 = Mathf.Abs(m_CurrentSubObject.m_Position.z - m_LastSubObject.m_Position.z);
			if (num < num2 || num < num3)
			{
				m_CurrentSubObject.m_Position.x = m_LastSubObject.m_Position.x;
			}
			if (num2 < num || num2 < num3)
			{
				m_CurrentSubObject.m_Position.y = m_LastSubObject.m_Position.y;
			}
			if (num3 < num || num3 < num2)
			{
				m_CurrentSubObject.m_Position.z = m_LastSubObject.m_Position.z;
			}
			UpdateParent(moveSubObjects: false);
		}
	}

	private void AutoConnect()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		if (m_CurrentSubObject == null || m_LastSubObject == null)
		{
			return;
		}
		Entity entity = m_PrefabSystem.GetEntity(m_LastSubObject.m_Object);
		Entity entity2 = m_PrefabSystem.GetEntity(m_CurrentSubObject.m_Object);
		ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
		ObjectGeometryData objectGeometryData2 = default(ObjectGeometryData);
		if (EntitiesExtensions.TryGetComponent<ObjectGeometryData>(((ComponentSystemBase)this).EntityManager, entity, ref objectGeometryData) && EntitiesExtensions.TryGetComponent<ObjectGeometryData>(((ComponentSystemBase)this).EntityManager, entity2, ref objectGeometryData2))
		{
			float num = Mathf.Abs(m_CurrentSubObject.m_Position.x - m_LastSubObject.m_Position.x);
			float num2 = Mathf.Abs(m_CurrentSubObject.m_Position.y - m_LastSubObject.m_Position.y);
			float num3 = Mathf.Abs(m_CurrentSubObject.m_Position.z - m_LastSubObject.m_Position.z);
			Bounds3 val = MathUtils.Bounds(MathUtils.Box(objectGeometryData.m_Bounds, m_LastSubObject.m_Rotation, m_LastSubObject.m_Position));
			Bounds3 val2 = MathUtils.Bounds(MathUtils.Box(objectGeometryData2.m_Bounds, m_CurrentSubObject.m_Rotation, m_CurrentSubObject.m_Position));
			if (num > num2 && num > num3)
			{
				if (m_CurrentSubObject.m_Position.x > m_LastSubObject.m_Position.x)
				{
					m_CurrentSubObject.m_Position.x += val.max.x - val2.min.x;
				}
				else
				{
					m_CurrentSubObject.m_Position.x += val.min.x - val2.max.x;
				}
			}
			else if (num2 > num && num2 > num3)
			{
				if (m_CurrentSubObject.m_Position.y > m_LastSubObject.m_Position.y)
				{
					m_CurrentSubObject.m_Position.y += val.max.y - val2.min.y;
				}
				else
				{
					m_CurrentSubObject.m_Position.y += val.min.y - val2.max.y;
				}
			}
			else if (num3 > num && num3 > num2)
			{
				if (m_CurrentSubObject.m_Position.z > m_LastSubObject.m_Position.z)
				{
					m_CurrentSubObject.m_Position.z += val.max.z - val2.min.z;
				}
				else
				{
					m_CurrentSubObject.m_Position.z += val.min.z - val2.max.z;
				}
			}
		}
		UpdateParent(moveSubObjects: false);
	}

	private void AlignX()
	{
		if (m_CurrentSubObject != null && m_LastSubObject != null)
		{
			m_CurrentSubObject.m_Position.x = m_LastSubObject.m_Position.x;
			UpdateParent(moveSubObjects: false);
		}
	}

	private void AlignY()
	{
		if (m_CurrentSubObject != null && m_LastSubObject != null)
		{
			m_CurrentSubObject.m_Position.y = m_LastSubObject.m_Position.y;
			UpdateParent(moveSubObjects: false);
		}
	}

	private void AlignZ()
	{
		if (m_CurrentSubObject != null && m_LastSubObject != null)
		{
			m_CurrentSubObject.m_Position.z = m_LastSubObject.m_Position.z;
			UpdateParent(moveSubObjects: false);
		}
	}

	private void UpdateParent(bool moveSubObjects)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (m_SelectedObject is ObjectMeshInfo { m_Mesh: var mesh })
		{
			m_PrefabSystem.UpdatePrefab(mesh);
			PrefabAsset asset = mesh.asset;
			if (asset != null)
			{
				((AssetData)asset).MarkDirty();
			}
		}
		if (m_ParentObject is PrefabBase prefabBase)
		{
			if (moveSubObjects)
			{
				MoveSubObjects(prefabBase);
			}
			m_PrefabSystem.UpdatePrefab(prefabBase);
			PrefabAsset asset2 = prefabBase.asset;
			if (asset2 != null)
			{
				((AssetData)asset2).MarkDirty();
			}
		}
	}

	private void MoveSubObjects(PrefabBase prefab)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b56: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0773: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Unknown result type (might be due to invalid IL or missing references)
		//IL_0740: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_074a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0756: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Unknown result type (might be due to invalid IL or missing references)
		//IL_0769: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0982: Unknown result type (might be due to invalid IL or missing references)
		//IL_0908: Unknown result type (might be due to invalid IL or missing references)
		//IL_0911: Unknown result type (might be due to invalid IL or missing references)
		//IL_0916: Unknown result type (might be due to invalid IL or missing references)
		//IL_091b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0927: Unknown result type (might be due to invalid IL or missing references)
		//IL_0930: Unknown result type (might be due to invalid IL or missing references)
		//IL_0935: Unknown result type (might be due to invalid IL or missing references)
		//IL_093a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0946: Unknown result type (might be due to invalid IL or missing references)
		//IL_094f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0954: Unknown result type (might be due to invalid IL or missing references)
		//IL_0959: Unknown result type (might be due to invalid IL or missing references)
		//IL_0965: Unknown result type (might be due to invalid IL or missing references)
		//IL_096e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0973: Unknown result type (might be due to invalid IL or missing references)
		//IL_0978: Unknown result type (might be due to invalid IL or missing references)
		//IL_080d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_078f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0794: Unknown result type (might be due to invalid IL or missing references)
		//IL_0799: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0803: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b05: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0995: Unknown result type (might be due to invalid IL or missing references)
		//IL_099e: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0820: Unknown result type (might be due to invalid IL or missing references)
		//IL_0829: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0833: Unknown result type (might be due to invalid IL or missing references)
		//IL_0846: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		//IL_0859: Unknown result type (might be due to invalid IL or missing references)
		//IL_0863: Unknown result type (might be due to invalid IL or missing references)
		//IL_0868: Unknown result type (might be due to invalid IL or missing references)
		//IL_087b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0880: Unknown result type (might be due to invalid IL or missing references)
		//IL_0889: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0898: Unknown result type (might be due to invalid IL or missing references)
		//IL_089d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aac: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		Entity entity = m_PrefabSystem.GetEntity(prefab);
		DynamicBuffer<SubMesh> val = default(DynamicBuffer<SubMesh>);
		if (!EntitiesExtensions.TryGetBuffer<SubMesh>(((ComponentSystemBase)this).EntityManager, entity, false, ref val) || !(prefab is ObjectGeometryPrefab { m_Meshes: not null } objectGeometryPrefab))
		{
			return;
		}
		int num = math.min(val.Length, objectGeometryPrefab.m_Meshes.Length);
		for (int i = 0; i < num; i++)
		{
			SubMesh subMesh = val[i];
			ObjectMeshInfo objectMeshInfo = objectGeometryPrefab.m_Meshes[i];
			if (subMesh.m_SubMesh != m_PrefabSystem.GetEntity(objectMeshInfo.m_Mesh) || (((float3)(ref subMesh.m_Position)).Equals(objectMeshInfo.m_Position) && ((quaternion)(ref subMesh.m_Rotation)).Equals(objectMeshInfo.m_Rotation)))
			{
				continue;
			}
			if (((quaternion)(ref subMesh.m_Rotation)).Equals(objectMeshInfo.m_Rotation))
			{
				float3 val2 = objectMeshInfo.m_Position - subMesh.m_Position;
				if (prefab.TryGet<ObjectSubObjects>(out var component) && component.m_SubObjects != null)
				{
					for (int j = 0; j < component.m_SubObjects.Length; j++)
					{
						ObjectSubObjectInfo objectSubObjectInfo = component.m_SubObjects[j];
						if (objectSubObjectInfo.m_ParentMesh % 1000 == i)
						{
							objectSubObjectInfo.m_Position += val2;
						}
					}
				}
				if (prefab.TryGet<ObjectSubAreas>(out var component2) && component2.m_SubAreas != null)
				{
					for (int k = 0; k < component2.m_SubAreas.Length; k++)
					{
						ObjectSubAreaInfo objectSubAreaInfo = component2.m_SubAreas[k];
						if (objectSubAreaInfo.m_NodePositions == null || objectSubAreaInfo.m_ParentMeshes == null)
						{
							continue;
						}
						int num2 = math.min(objectSubAreaInfo.m_NodePositions.Length, objectSubAreaInfo.m_ParentMeshes.Length);
						for (int l = 0; l < num2; l++)
						{
							if (objectSubAreaInfo.m_ParentMeshes[l] == i)
							{
								ref float3 reference = ref objectSubAreaInfo.m_NodePositions[l];
								reference += val2;
							}
						}
					}
				}
				if (prefab.TryGet<ObjectSubLanes>(out var component3) && component3.m_SubLanes != null)
				{
					for (int m = 0; m < component3.m_SubLanes.Length; m++)
					{
						ObjectSubLaneInfo objectSubLaneInfo = component3.m_SubLanes[m];
						bool2 val3 = objectSubLaneInfo.m_ParentMesh == i;
						if (math.all(val3))
						{
							objectSubLaneInfo.m_BezierCurve += val2;
						}
						else if (val3.x)
						{
							ref float3 a = ref objectSubLaneInfo.m_BezierCurve.a;
							a += val2;
							ref float3 b = ref objectSubLaneInfo.m_BezierCurve.b;
							b += val2 * (2f / 3f);
							ref float3 c = ref objectSubLaneInfo.m_BezierCurve.c;
							c += val2 * (1f / 3f);
						}
						else if (val3.y)
						{
							ref float3 d = ref objectSubLaneInfo.m_BezierCurve.d;
							d += val2;
							ref float3 c2 = ref objectSubLaneInfo.m_BezierCurve.c;
							c2 += val2 * (2f / 3f);
							ref float3 b2 = ref objectSubLaneInfo.m_BezierCurve.b;
							b2 += val2 * (1f / 3f);
						}
					}
				}
				if (prefab.TryGet<ObjectSubNets>(out var component4) && component4.m_SubNets != null)
				{
					for (int n = 0; n < component4.m_SubNets.Length; n++)
					{
						ObjectSubNetInfo objectSubNetInfo = component4.m_SubNets[n];
						bool2 val4 = objectSubNetInfo.m_ParentMesh == i;
						if (math.all(val4))
						{
							objectSubNetInfo.m_BezierCurve += val2;
						}
						else if (val4.x)
						{
							ref float3 a2 = ref objectSubNetInfo.m_BezierCurve.a;
							a2 += val2;
							ref float3 b3 = ref objectSubNetInfo.m_BezierCurve.b;
							b3 += val2 * (2f / 3f);
							ref float3 c3 = ref objectSubNetInfo.m_BezierCurve.c;
							c3 += val2 * (1f / 3f);
						}
						else if (val4.y)
						{
							ref float3 d2 = ref objectSubNetInfo.m_BezierCurve.d;
							d2 += val2;
							ref float3 c4 = ref objectSubNetInfo.m_BezierCurve.c;
							c4 += val2 * (2f / 3f);
							ref float3 b4 = ref objectSubNetInfo.m_BezierCurve.b;
							b4 += val2 * (1f / 3f);
						}
					}
				}
				if (prefab.TryGet<EffectSource>(out var component5) && component5.m_Effects != null)
				{
					for (int num3 = 0; num3 < component5.m_Effects.Count; num3++)
					{
						EffectSource.EffectSettings effectSettings = component5.m_Effects[num3];
						if (effectSettings.m_ParentMesh == i)
						{
							effectSettings.m_PositionOffset += val2;
						}
					}
				}
			}
			else
			{
				float4x4 val5 = float4x4.TRS(subMesh.m_Position, subMesh.m_Rotation, float3.op_Implicit(1f));
				float4x4 val6 = math.mul(float4x4.TRS(objectMeshInfo.m_Position, objectMeshInfo.m_Rotation, float3.op_Implicit(1f)), math.inverse(val5));
				quaternion val7 = math.mul(objectMeshInfo.m_Rotation, math.inverse(subMesh.m_Rotation));
				if (prefab.TryGet<ObjectSubObjects>(out var component6) && component6.m_SubObjects != null)
				{
					for (int num4 = 0; num4 < component6.m_SubObjects.Length; num4++)
					{
						ObjectSubObjectInfo objectSubObjectInfo2 = component6.m_SubObjects[num4];
						if (objectSubObjectInfo2.m_ParentMesh % 1000 == i)
						{
							objectSubObjectInfo2.m_Position = math.transform(val6, objectSubObjectInfo2.m_Position);
							objectSubObjectInfo2.m_Rotation = math.normalize(math.mul(val7, objectSubObjectInfo2.m_Rotation));
						}
					}
				}
				if (prefab.TryGet<ObjectSubAreas>(out var component7) && component7.m_SubAreas != null)
				{
					for (int num5 = 0; num5 < component7.m_SubAreas.Length; num5++)
					{
						ObjectSubAreaInfo objectSubAreaInfo2 = component7.m_SubAreas[num5];
						if (objectSubAreaInfo2.m_NodePositions == null || objectSubAreaInfo2.m_ParentMeshes == null)
						{
							continue;
						}
						int num6 = math.min(objectSubAreaInfo2.m_NodePositions.Length, objectSubAreaInfo2.m_ParentMeshes.Length);
						for (int num7 = 0; num7 < num6; num7++)
						{
							if (objectSubAreaInfo2.m_ParentMeshes[num7] == i)
							{
								objectSubAreaInfo2.m_NodePositions[num7] = math.transform(val6, objectSubAreaInfo2.m_NodePositions[num7]);
							}
						}
					}
				}
				if (prefab.TryGet<ObjectSubLanes>(out var component8) && component8.m_SubLanes != null)
				{
					for (int num8 = 0; num8 < component8.m_SubLanes.Length; num8++)
					{
						ObjectSubLaneInfo objectSubLaneInfo2 = component8.m_SubLanes[num8];
						bool2 val8 = objectSubLaneInfo2.m_ParentMesh == i;
						if (math.all(val8))
						{
							objectSubLaneInfo2.m_BezierCurve.a = math.transform(val6, objectSubLaneInfo2.m_BezierCurve.a);
							objectSubLaneInfo2.m_BezierCurve.b = math.transform(val6, objectSubLaneInfo2.m_BezierCurve.b);
							objectSubLaneInfo2.m_BezierCurve.c = math.transform(val6, objectSubLaneInfo2.m_BezierCurve.c);
							objectSubLaneInfo2.m_BezierCurve.d = math.transform(val6, objectSubLaneInfo2.m_BezierCurve.d);
						}
						else if (val8.x)
						{
							objectSubLaneInfo2.m_BezierCurve.a = math.transform(val6, objectSubLaneInfo2.m_BezierCurve.a);
							objectSubLaneInfo2.m_BezierCurve.b = math.lerp(objectSubLaneInfo2.m_BezierCurve.b, math.transform(val6, objectSubLaneInfo2.m_BezierCurve.b), 2f / 3f);
							objectSubLaneInfo2.m_BezierCurve.c = math.lerp(objectSubLaneInfo2.m_BezierCurve.c, math.transform(val6, objectSubLaneInfo2.m_BezierCurve.c), 1f / 3f);
						}
						else if (val8.y)
						{
							objectSubLaneInfo2.m_BezierCurve.d = math.transform(val6, objectSubLaneInfo2.m_BezierCurve.d);
							objectSubLaneInfo2.m_BezierCurve.c = math.lerp(objectSubLaneInfo2.m_BezierCurve.c, math.transform(val6, objectSubLaneInfo2.m_BezierCurve.c), 2f / 3f);
							objectSubLaneInfo2.m_BezierCurve.b = math.lerp(objectSubLaneInfo2.m_BezierCurve.b, math.transform(val6, objectSubLaneInfo2.m_BezierCurve.b), 1f / 3f);
						}
					}
				}
				if (prefab.TryGet<ObjectSubNets>(out var component9) && component9.m_SubNets != null)
				{
					for (int num9 = 0; num9 < component9.m_SubNets.Length; num9++)
					{
						ObjectSubNetInfo objectSubNetInfo2 = component9.m_SubNets[num9];
						bool2 val9 = objectSubNetInfo2.m_ParentMesh == i;
						if (math.all(val9))
						{
							objectSubNetInfo2.m_BezierCurve.a = math.transform(val6, objectSubNetInfo2.m_BezierCurve.a);
							objectSubNetInfo2.m_BezierCurve.b = math.transform(val6, objectSubNetInfo2.m_BezierCurve.b);
							objectSubNetInfo2.m_BezierCurve.c = math.transform(val6, objectSubNetInfo2.m_BezierCurve.c);
							objectSubNetInfo2.m_BezierCurve.d = math.transform(val6, objectSubNetInfo2.m_BezierCurve.d);
						}
						else if (val9.x)
						{
							objectSubNetInfo2.m_BezierCurve.a = math.transform(val6, objectSubNetInfo2.m_BezierCurve.a);
							objectSubNetInfo2.m_BezierCurve.b = math.lerp(objectSubNetInfo2.m_BezierCurve.b, math.transform(val6, objectSubNetInfo2.m_BezierCurve.b), 2f / 3f);
							objectSubNetInfo2.m_BezierCurve.c = math.lerp(objectSubNetInfo2.m_BezierCurve.c, math.transform(val6, objectSubNetInfo2.m_BezierCurve.c), 1f / 3f);
						}
						else if (val9.y)
						{
							objectSubNetInfo2.m_BezierCurve.d = math.transform(val6, objectSubNetInfo2.m_BezierCurve.d);
							objectSubNetInfo2.m_BezierCurve.c = math.lerp(objectSubNetInfo2.m_BezierCurve.c, math.transform(val6, objectSubNetInfo2.m_BezierCurve.c), 2f / 3f);
							objectSubNetInfo2.m_BezierCurve.b = math.lerp(objectSubNetInfo2.m_BezierCurve.b, math.transform(val6, objectSubNetInfo2.m_BezierCurve.b), 1f / 3f);
						}
					}
				}
				if (prefab.TryGet<EffectSource>(out var component10) && component10.m_Effects != null)
				{
					for (int num10 = 0; num10 < component10.m_Effects.Count; num10++)
					{
						EffectSource.EffectSettings effectSettings2 = component10.m_Effects[num10];
						if (effectSettings2.m_ParentMesh == i)
						{
							effectSettings2.m_PositionOffset = math.transform(val6, effectSettings2.m_PositionOffset);
							effectSettings2.m_Rotation = math.normalize(math.mul(val7, effectSettings2.m_Rotation));
						}
					}
				}
			}
			subMesh.m_Position = objectMeshInfo.m_Position;
			subMesh.m_Rotation = objectMeshInfo.m_Rotation;
			val[i] = subMesh;
		}
	}

	private void OnLocate()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		int elementIndex = -1;
		if (m_CurrentSelectedEntity != Entity.Null && SelectedInfoUISystem.TryGetPosition(m_CurrentSelectedEntity, ((ComponentSystemBase)this).EntityManager, ref elementIndex, out var _, out var position, out var _, out var _))
		{
			if (m_CameraUpdateSystem.activeCameraController == m_CameraUpdateSystem.cinematicCameraController)
			{
				Vector3 rotation2 = m_CameraUpdateSystem.cinematicCameraController.rotation;
				rotation2.x = Mathf.Clamp(rotation2.x, 0f, 90f);
				m_CameraUpdateSystem.cinematicCameraController.rotation = rotation2;
				position = float3.op_Implicit(float3.op_Implicit(position) + Quaternion.Euler(rotation2) * new Vector3(0f, 0f, -1000f));
				m_CameraUpdateSystem.cinematicCameraController.position = float3.op_Implicit(position);
			}
			else
			{
				m_CameraUpdateSystem.activeCameraController.pivot = float3.op_Implicit(position);
			}
		}
	}

	private bool IsColorVariationField(IWidget widget, out int variationSetIndex, out int colorIndex, out RenderPrefabBase mesh)
	{
		variationSetIndex = -1;
		colorIndex = -1;
		mesh = null;
		if (widget.path.m_Key == null || !(widget is ColorField) || !(m_SelectedObject is ObjectMeshInfo objectMeshInfo))
		{
			return false;
		}
		Match match = Regex.Match(widget.path.m_Key, "^ColorProperties.m_ColorVariations\\[(\\d+)\\].m_Colors\\[(\\d+)\\]$");
		if (match.Success && int.TryParse(match.Groups[1].Value, out variationSetIndex) && int.TryParse(match.Groups[2].Value, out colorIndex))
		{
			mesh = objectMeshInfo.m_Mesh;
			return true;
		}
		return false;
	}

	private bool IsEmissiveField(IWidget widget, out int singleLightIndex, out int multiLightIndex, out RenderPrefabBase mesh)
	{
		singleLightIndex = -1;
		multiLightIndex = -1;
		mesh = null;
		if (widget.path.m_Key == null || !(m_SelectedObject is ObjectMeshInfo objectMeshInfo))
		{
			return false;
		}
		Match match = Regex.Match(widget.path.m_Key, "^EmissiveProperties.m_SingleLights\\[(\\d+)\\].");
		if (match.Success && int.TryParse(match.Groups[1].Value, out singleLightIndex))
		{
			mesh = objectMeshInfo.m_Mesh;
			return true;
		}
		match = Regex.Match(widget.path.m_Key, "^EmissiveProperties.m_MultiLights\\[(\\d+)\\].");
		if (match.Success && int.TryParse(match.Groups[1].Value, out multiLightIndex))
		{
			mesh = objectMeshInfo.m_Mesh;
			return true;
		}
		return false;
	}

	private void OnColorVariationChanged(Entity entity, RenderPrefabBase mesh, int variationSetIndex, int colorIndex)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		((ComponentSystemBase)this).World.GetExistingSystemManaged<MeshColorSystem>()?.SetOverride(entity, mesh, variationSetIndex);
	}

	private void OnEmissiveChanged(Entity entity, RenderPrefabBase mesh, int singleLightIndex, int multiLightIndex)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		((ComponentSystemBase)this).World.GetExistingSystemManaged<ProceduralUploadSystem>()?.SetOverride(entity, mesh, singleLightIndex, multiLightIndex);
	}

	private void ShowSaveAssetPanel()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		Identifier? obj;
		if (!(m_SelectedObject is PrefabBase prefabBase))
		{
			obj = null;
		}
		else
		{
			PrefabAsset asset = prefabBase.asset;
			obj = ((asset != null) ? new Identifier?(((AssetData)asset).id) : ((Identifier?)null));
		}
		Identifier? val = obj;
		Hash128? initialSelected = (val.HasValue ? new Hash128?(Identifier.op_Implicit(val.GetValueOrDefault())) : ((Hash128?)null));
		base.activeSubPanel = new SaveAssetPanel("Editor.SAVE_ASSET", GetCustomAssets(), initialSelected, delegate(string name, Hash128? overwriteGuid)
		{
			OnSaveAsset(name, overwriteGuid);
		}, base.CloseSubPanel);
	}

	private IEnumerable<AssetItem> GetCustomAssets()
	{
		if (m_SelectedObject is PrefabBase prefabBase && IsBuiltinAsset((AssetData)(object)prefabBase.asset) && TryGetAssetItem(prefabBase.asset, out var item))
		{
			yield return item;
		}
		foreach (PrefabAsset asset in ((IAssetDatabase)AssetDatabase.user).GetAssets<PrefabAsset>(default(SearchFilter<PrefabAsset>)))
		{
			if (TryGetAssetItem(asset, out var item2))
			{
				yield return item2;
			}
		}
	}

	private bool TryGetAssetItem(PrefabAsset asset, out AssetItem item)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			PrefabBase prefabBase = asset.Load() as PrefabBase;
			if (prefabBase is RenderPrefabBase)
			{
				item = null;
				return false;
			}
			SourceMeta meta = ((AssetData)asset).GetMeta();
			item = new AssetItem
			{
				guid = Identifier.op_Implicit(((AssetData)asset).id),
				fileName = meta.fileName,
				displayName = meta.fileName,
				image = (((Object)(object)prefabBase != (Object)null) ? ImageSystem.GetThumbnail(prefabBase) : null),
				badge = meta.remoteStorageSourceName
			};
			return true;
		}
		catch (Exception ex)
		{
			base.log.Error(ex);
			item = null;
		}
		item = null;
		return false;
	}

	private void OnSaveAsset(string name, Hash128? overwriteGuid, Action<PrefabAsset> callback = null)
	{
		CloseSubPanel();
		if (overwriteGuid.HasValue)
		{
			GameManager.instance.userInterface.appBindings.ShowConfirmationDialog(new ConfirmationDialog(null, "Common.DIALOG_MESSAGE[OverwriteAsset]", "Common.DIALOG_ACTION[Yes]", "Common.DIALOG_ACTION[No]"), delegate(int ret)
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				if (ret == 0)
				{
					PrefabAsset asset = AssetDatabase.global.GetAsset<PrefabAsset>(overwriteGuid.Value);
					SaveAsset(name, asset, callback);
				}
			});
		}
		else
		{
			SaveAsset(name, null, callback);
		}
	}

	private void SaveAsset(string name, PrefabAsset existing = null, Action<PrefabAsset> callback = null)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		PrefabBase prefabBase = m_SelectedObject as PrefabBase;
		if ((AssetData)(object)prefabBase.asset != (IAssetData)null && ((AssetData)(object)existing == (IAssetData)null || (AssetData)(object)existing != (IAssetData)(object)prefabBase.asset))
		{
			prefabBase = DuplicatePrefab(prefabBase);
			SelectPrefab(prefabBase);
		}
		PrefabAsset val = existing;
		if ((AssetData)(object)existing != (IAssetData)null)
		{
			existing.SetData((ScriptableObject)(object)prefabBase);
		}
		else
		{
			val = PrefabAssetExtensions.AddAsset(AssetDatabase.user, AssetDataPath.Create("StreamingData~/" + name, name ?? "", (EscapeStrategy)2), (ScriptableObject)(object)prefabBase);
		}
		SaveIcons(prefabBase, name);
		((AssetData)val).Save(false);
		if (!IsBuiltinAsset((AssetData)(object)val))
		{
			SaveLocalization(prefabBase, name);
		}
		PlatformManager.instance.UnlockAchievement(Game.Achievements.Achievements.IMadeThis);
		callback?.Invoke(val);
	}

	private void ShowShareAssetPanel()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		Identifier? obj;
		if (!(m_SelectedObject is PrefabBase prefabBase))
		{
			obj = null;
		}
		else
		{
			PrefabAsset asset = prefabBase.asset;
			obj = ((asset != null) ? new Identifier?(((AssetData)asset).id) : ((Identifier?)null));
		}
		Identifier? val = obj;
		Hash128? initialSelected = (val.HasValue ? new Hash128?(Identifier.op_Implicit(val.GetValueOrDefault())) : ((Hash128?)null));
		base.activeSubPanel = new SaveAssetPanel("Editor.SAVE_SHARE", GetCustomAssets(), initialSelected, delegate(string name, Hash128? overwriteGuid)
		{
			OnSaveAsset(name, overwriteGuid, OnShareAsset);
		}, base.CloseSubPanel, "Editor.SAVE_SHARE");
	}

	private void OnShareAsset(PrefabAsset asset)
	{
		m_AssetUploadPanel.Show((AssetData)(object)asset);
		base.activeSubPanel = m_AssetUploadPanel;
	}

	private PrefabBase DuplicatePrefab(PrefabBase oldPrefab)
	{
		PrefabBase prefabBase = m_PrefabSystem.DuplicatePrefab(oldPrefab, ((Object)oldPrefab).name);
		if (m_WipLocalization.TryGetValue(oldPrefab, out var value))
		{
			m_WipLocalization.Add(prefabBase, value.Clone());
		}
		prefabBase.asset = null;
		return prefabBase;
	}

	public void ShowThumbnailPicker(LoadAssetPanel.LoadCallback callback)
	{
		base.activeSubPanel = new LoadAssetPanel("Editor.THUMBNAIL", EditorPrefabUtils.GetUserImages(), delegate(Hash128 hash)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			callback(hash);
			CloseSubPanel();
		}, base.CloseSubPanel);
	}

	private void SaveIcons(PrefabBase prefab, string name)
	{
		Dictionary<ImageAsset, ImageAsset> dictionary = new Dictionary<ImageAsset, ImageAsset>();
		foreach (EditorPrefabUtils.IconInfo icon in EditorPrefabUtils.GetIcons(prefab))
		{
			if (!IsBuiltinAsset((AssetData)(object)icon.m_Asset))
			{
				if (!dictionary.ContainsKey(icon.m_Asset))
				{
					ImageAsset value = icon.m_Asset.Save((FileFormat)0, AssetDataPath.Create(prefab.asset.subPath, ((AssetData)icon.m_Asset).name ?? "", (EscapeStrategy)2), ((AssetData)prefab.asset).database);
					dictionary.Add(icon.m_Asset, value);
				}
				icon.m_Field.SetValue(icon.m_Component, UIExtensions.ToGlobalUri((AssetData)(object)dictionary[icon.m_Asset]));
			}
		}
	}

	private void BuildLocalizationFields(PrefabBase prefab, List<IWidget> widgets)
	{
		if (!m_WipLocalization.TryGetValue(prefab, out var value))
		{
			value = new LocalizationFields
			{
				m_NameLocalization = new LocalizationField("Editor.ASSET_NAME"),
				m_DescriptionLocalization = new LocalizationField("Editor.ASSET_DESCRIPTION")
			};
			List<LocalizationField.LocalizationFieldEntry> entries = InitializeLocalization(prefab, "Assets.NAME[" + ((Object)prefab).name + "]");
			value.m_NameLocalization.Initialize(entries);
			List<LocalizationField.LocalizationFieldEntry> entries2 = InitializeLocalization(prefab, "Assets.DESCRIPTION[" + ((Object)prefab).name + "]");
			value.m_DescriptionLocalization.Initialize(entries2);
			m_WipLocalization[prefab] = value;
		}
		widgets.Add(new Game.UI.Widgets.Group
		{
			displayName = "Localized Name",
			children = new IWidget[1] { value.m_NameLocalization },
			tooltip = "Editor.LOCALIZED_NAME_TOOLTIP"
		});
		widgets.Add(new Game.UI.Widgets.Group
		{
			displayName = "Localized Description",
			children = new IWidget[1] { value.m_DescriptionLocalization },
			tooltip = "Editor.LOCALIZED_DESCRIPTION_TOOLTIP"
		});
	}

	private List<LocalizationField.LocalizationFieldEntry> InitializeLocalization(PrefabBase prefab, string key)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		List<LocalizationField.LocalizationFieldEntry> list = new List<LocalizationField.LocalizationFieldEntry>();
		if ((AssetData)(object)prefab.asset != (IAssetData)null && ((AssetData)prefab.asset).database == AssetDatabase.user)
		{
			foreach (LocaleAsset localeAsset in EditorPrefabUtils.GetLocaleAssets(prefab))
			{
				if (localeAsset.data.entries.TryGetValue(key, out var value))
				{
					list.Add(new LocalizationField.LocalizationFieldEntry
					{
						localeId = localeAsset.localeId,
						text = value
					});
				}
			}
		}
		else
		{
			foreach (LocaleAsset asset in AssetDatabase.global.GetAssets<LocaleAsset>(default(SearchFilter<LocaleAsset>)))
			{
				if (asset.data.entries.TryGetValue(key, out var value2))
				{
					list.Add(new LocalizationField.LocalizationFieldEntry
					{
						localeId = asset.localeId,
						text = value2
					});
				}
			}
		}
		return list;
	}

	private void SaveLocalization(PrefabBase prefab, string name)
	{
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		if (!m_WipLocalization.TryGetValue(prefab, out var value))
		{
			return;
		}
		List<LocaleAsset> list = new List<LocaleAsset>();
		list.AddRange(EditorPrefabUtils.GetLocaleAssets(prefab));
		foreach (LocaleAsset item in list)
		{
			((IAssetDatabase)AssetDatabase.user).DeleteAsset<LocaleAsset>(item);
		}
		LocalizationManager localizationManager = GameManager.instance.localizationManager;
		Dictionary<string, LocaleData> dictionary = new Dictionary<string, LocaleData>();
		value.m_NameLocalization.BuildLocaleData("Assets.NAME[" + ((Object)prefab).name + "]", dictionary, ((Object)prefab).name);
		value.m_DescriptionLocalization.BuildLocaleData("Assets.DESCRIPTION[" + ((Object)prefab).name + "]", dictionary);
		if (prefab is UIAssetMenuPrefab || prefab is ServicePrefab)
		{
			value.m_NameLocalization.BuildLocaleData("Services.NAME[" + ((Object)prefab).name + "]", dictionary, ((Object)prefab).name);
			value.m_DescriptionLocalization.BuildLocaleData("Services.DESCRIPTION[" + ((Object)prefab).name + "]", dictionary);
		}
		if (prefab is UIAssetCategoryPrefab)
		{
			value.m_NameLocalization.BuildLocaleData("SubServices.NAME[" + ((Object)prefab).name + "]", dictionary, ((Object)prefab).name);
			value.m_DescriptionLocalization.BuildLocaleData("Assets.SUB_SERVICE_DESCRIPTION[" + ((Object)prefab).name + "]", dictionary);
		}
		if (prefab.Has<ServiceUpgrade>())
		{
			value.m_NameLocalization.BuildLocaleData("Assets.UPGRADE_NAME[" + ((Object)prefab).name + "]", dictionary, ((Object)prefab).name);
			value.m_DescriptionLocalization.BuildLocaleData("Assets.UPGRADE_DESCRIPTION[" + ((Object)prefab).name + "]", dictionary);
		}
		foreach (LocaleData value2 in dictionary.Values)
		{
			LocaleAsset obj = ((AssetData)prefab.asset).database.AddAsset<LocaleAsset>(AssetDataPath.Create(prefab.asset.subPath, name + "_" + value2.localeId, (EscapeStrategy)2), default(Hash128));
			obj.SetData(value2, localizationManager.LocaleIdToSystemLanguage(value2.localeId), GameManager.instance.localizationManager.GetLocalizedName(value2.localeId));
			((AssetData)obj).Save(false);
		}
	}

	[Preserve]
	public InspectorPanelSystem()
	{
	}
}
