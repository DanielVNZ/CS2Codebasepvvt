using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Colossal;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.Localization;
using Colossal.PSI.Common;
using Colossal.PSI.PdxSdk;
using Colossal.Serialization.Entities;
using Colossal.UI;
using Game.Achievements;
using Game.Areas;
using Game.Assets;
using Game.City;
using Game.Common;
using Game.Prefabs;
using Game.Prefabs.Climate;
using Game.Reflection;
using Game.SceneFlow;
using Game.Serialization;
using Game.Simulation;
using Game.UI.Localization;
using Game.UI.Menu;
using Game.UI.Widgets;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Scripting;

namespace Game.UI.Editor;

[CompilerGenerated]
public class MapPanelSystem : EditorPanelSystemBase
{
	private struct PreviewInfo : IEquatable<PreviewInfo>
	{
		private ImageAsset m_ImageAsset;

		private TextureAsset m_TextureAsset;

		public Texture texture
		{
			get
			{
				ImageAsset obj = m_ImageAsset;
				object obj2 = ((obj != null) ? obj.Load(true) : null);
				if (obj2 == null)
				{
					TextureAsset obj3 = m_TextureAsset;
					if (obj3 == null)
					{
						return null;
					}
					obj2 = obj3.Load(-1);
				}
				return (Texture)obj2;
			}
		}

		public IconButton button { get; set; }

		public string name
		{
			get
			{
				ImageAsset obj = m_ImageAsset;
				object obj2 = ((obj != null) ? ((AssetData)obj).name : null);
				if (obj2 == null)
				{
					TextureAsset obj3 = m_TextureAsset;
					if (obj3 == null)
					{
						return null;
					}
					obj2 = ((AssetData)obj3).name;
				}
				return (string)obj2;
			}
		}

		public void Set(ImageAsset imageAsset, TextureAsset fallback = null)
		{
			m_ImageAsset = imageAsset;
			m_TextureAsset = null;
			button.icon = ((imageAsset != null) ? UIExtensions.ToUri((AssetData)(object)imageAsset) : null) ?? ((fallback != null) ? UIExtensions.ToUri(fallback, (TextureAsset)null, 0) : null);
		}

		public void Set(TextureAsset textureAsset, TextureAsset fallback = null)
		{
			m_TextureAsset = textureAsset;
			m_ImageAsset = null;
			button.icon = ((textureAsset != null) ? UIExtensions.ToUri(textureAsset, (TextureAsset)null, 0) : null) ?? ((fallback != null) ? UIExtensions.ToUri(fallback, (TextureAsset)null, 0) : null);
		}

		public PreviewInfo(IconButton button)
		{
			this.button = button;
			m_ImageAsset = null;
			m_TextureAsset = null;
		}

		public bool CopyToTextureAsset(ILocalAssetDatabase db, AssetDataPath path, out TextureAsset asset)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Expected O, but got Unknown
			if ((AssetData)(object)m_ImageAsset != (IAssetData)null || (AssetData)(object)m_TextureAsset != (IAssetData)null)
			{
				asset = db.AddAsset<TextureAsset>(path, default(Hash128));
				if ((AssetData)(object)m_ImageAsset != (IAssetData)null)
				{
					Texture2D val = m_ImageAsset.Load(false);
					Texture2D val2 = new Texture2D(((Texture)val).width, ((Texture)val).height, ((Texture)val).graphicsFormat, (TextureCreationFlags)4);
					Graphics.CopyTexture((Texture)(object)val, (Texture)(object)val2);
					asset.SetData((Texture)(object)val2);
					return true;
				}
				using Stream stream = ((AssetData)m_TextureAsset).GetReadStream();
				using Stream destination = ((AssetData)asset).GetWriteStream();
				stream.CopyTo(destination);
				return true;
			}
			asset = null;
			return false;
		}

		public bool Equals(PreviewInfo other)
		{
			if ((AssetData)(object)m_ImageAsset == (IAssetData)(object)other.m_ImageAsset)
			{
				return (AssetData)(object)m_TextureAsset == (IAssetData)(object)other.m_TextureAsset;
			}
			return false;
		}
	}

	private Hash128 m_CurrentSourceDataGuid;

	private TerrainSystem m_TerrainSystem;

	private SaveGameSystem m_SaveGameSystem;

	private MapMetadataSystem m_MapMetadataSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private IMapTilePurchaseSystem m_MapTilePurchaseSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private PrefabSystem m_PrefabSystem;

	private EditorAssetUploadPanel m_AssetUploadPanel;

	private EntityQuery m_TimeQuery;

	private EntityQuery m_ThemeQuery;

	public bool m_MapNameAsCityName;

	public int m_StartingYear;

	public int m_StartingMonth;

	public float m_StartingTime;

	public bool m_CurrentYearAsStartingYear;

	private IconButtonGroup m_ThemeButtonGroup;

	private LocalizationField m_MapNameLocalization;

	private LocalizationField m_MapDescriptionLocalization;

	private PreviewInfo m_Preview;

	private PreviewInfo m_Thumbnail;

	private Button m_MapTileSelectionButton;

	private static readonly string kSelectStartingTilesPrompt = "Editor.SELECT_STARTING_TILES";

	private static readonly string kStopSelectingStartingTilesPrompt = "Editor.STOP_SELECTING_STARTING_TILES";

	private MapRequirementSystem m_MapRequirementSystem;

	private PagedList m_RequiredListWidget;

	private EditorGenerator m_Generator;

	private PdxSdkPlatform m_Platform;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d72: Expected O, but got Unknown
		base.OnCreate();
		m_Generator = new EditorGenerator();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_SaveGameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SaveGameSystem>();
		m_MapMetadataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MapMetadataSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_MapTilePurchaseSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MapTilePurchaseSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_MapRequirementSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MapRequirementSystem>();
		m_AssetUploadPanel = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EditorAssetUploadPanel>();
		m_TimeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeData>() });
		m_ThemeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ThemeData>() });
		m_Preview = new PreviewInfo(new IconButton
		{
			action = ShowPreviewPicker
		});
		m_Thumbnail = new PreviewInfo(new IconButton
		{
			action = ShowThumbnailPicker
		});
		title = "Editor.MAP";
		IWidget[] array = new IWidget[2];
		IWidget[] array2 = new IWidget[2];
		EditorSection editorSection = new EditorSection
		{
			displayName = "Editor.MAP_SETTINGS",
			expanded = true
		};
		EditorSection editorSection2 = editorSection;
		IWidget[] obj = new IWidget[14]
		{
			new Group
			{
				displayName = "Editor.MAP_NAME",
				tooltip = "Editor.MAP_NAME_TOOLTIP",
				children = new IWidget[1] { m_MapNameLocalization = new LocalizationField("Editor.MAP_NAME") }
			},
			new Group
			{
				displayName = "Editor.MAP_DESCRIPTION",
				tooltip = "Editor.MAP_DESCRIPTION_TOOLTIP",
				children = new IWidget[1] { m_MapDescriptionLocalization = new LocalizationField("Editor.MAP_DESCRIPTION") }
			},
			new ToggleField
			{
				displayName = "Editor.MAP_NAME_AS_DEFAULT",
				tooltip = "Editor.MAP_NAME_AS_DEFAULT_TOOLTIP",
				accessor = new DelegateAccessor<bool>(() => m_MapNameAsCityName, delegate(bool value)
				{
					m_MapNameAsCityName = value;
				})
			},
			new Divider(),
			new IntInputField
			{
				displayName = "Editor.STARTING_YEAR",
				tooltip = "Editor.STARTING_YEAR_TOOLTIP",
				disabled = () => m_CurrentYearAsStartingYear,
				min = 0,
				max = 3000,
				accessor = new DelegateAccessor<int>(() => (!m_CurrentYearAsStartingYear) ? m_StartingYear : DateTime.Now.Year, SetStartingYear)
			},
			new ToggleField
			{
				displayName = "Editor.CURRENT_YEAR_AS_DEFAULT",
				tooltip = "Editor.CURRENT_YEAR_AS_DEFAULT_TOOLTIP",
				accessor = new DelegateAccessor<bool>(() => m_CurrentYearAsStartingYear, delegate(bool value)
				{
					m_CurrentYearAsStartingYear = value;
				})
			},
			new IntInputField
			{
				displayName = "Editor.STARTING_MONTH",
				tooltip = "Editor.STARTING_MONTH_TOOLTIP",
				min = 1,
				max = 12,
				accessor = new DelegateAccessor<int>(() => m_StartingMonth, SetStartingMonth)
			},
			new TimeSliderField
			{
				displayName = "Editor.STARTING_TIME",
				tooltip = "Editor.STARTING_TIME_TOOLTIP",
				min = 0f,
				max = 0.99930555f,
				accessor = new DelegateAccessor<float>(() => m_StartingTime, SetStartingTime)
			},
			new Group
			{
				displayName = "Editor.CAMERA_STARTING_POSITION",
				tooltip = "Editor.CAMERA_STARTING_POSITION_TOOLTIP",
				children = new IWidget[4]
				{
					new Float3InputField
					{
						displayName = "Editor.CAMERA_PIVOT",
						tooltip = "Editor.CAMERA_PIVOT_TOOLTIP",
						accessor = new DelegateAccessor<float3>(() => m_CityConfigurationSystem.m_CameraPivot, delegate(float3 value)
						{
							//IL_0006: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							m_CityConfigurationSystem.m_CameraPivot = value;
						})
					},
					new Float2InputField
					{
						displayName = "Editor.CAMERA_ANGLE",
						tooltip = "Editor.CAMERA_ANGLE_TOOLTIP",
						accessor = new DelegateAccessor<float2>(() => m_CityConfigurationSystem.m_CameraAngle, delegate(float2 value)
						{
							//IL_0006: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							m_CityConfigurationSystem.m_CameraAngle = value;
						})
					},
					new FloatInputField
					{
						displayName = "Editor.CAMERA_ZOOM",
						tooltip = "Editor.CAMERA_ZOOM_TOOLTIP",
						accessor = new DelegateAccessor<double>(() => m_CityConfigurationSystem.m_CameraZoom, delegate(double value)
						{
							m_CityConfigurationSystem.m_CameraZoom = (float)value;
						})
					},
					new Button
					{
						displayName = "Editor.CAPTURE_CAMERA_POSITION",
						tooltip = "Editor.CAPTURE_CAMERA_POSITION_TOOLTIP",
						action = CaptureCameraProperties
					}
				}
			},
			null,
			null,
			null,
			null,
			null
		};
		Button obj2 = new Button
		{
			displayName = kSelectStartingTilesPrompt,
			action = ToggleMapTileSelection
		};
		Button button = obj2;
		m_MapTileSelectionButton = obj2;
		obj[9] = button;
		obj[10] = new EditorSection
		{
			displayName = "Editor.THEME",
			tooltip = "Editor.THEME_TOOLTIP",
			expanded = true,
			children = new IWidget[1] { m_ThemeButtonGroup = new IconButtonGroup() }
		};
		obj[11] = new EditorSection
		{
			displayName = "Editor.CONTENT_PREREQUISITES",
			tooltip = "Editor.CONTENT_PREREQUISITES_TOOLTIP",
			expanded = true,
			children = new IWidget[1] { m_RequiredListWidget = EditorGenerator.NamedWidget(m_Generator.TryBuildList(new ObjectAccessor<PrefabEntityListWrapper<ContentPrefab>>(new PrefabEntityListWrapper<ContentPrefab>(m_CityConfigurationSystem.requiredContent, m_PrefabSystem), readOnly: false), 0, null, Array.Empty<object>()), "Editor.REQUIREMENTS", "Editor.REQUIREMENTS_TOOLTIP") }
		};
		obj[12] = new Group
		{
			displayName = "Editor.PREVIEW",
			tooltip = "Editor.PREVIEW_TOOLTIP",
			children = new IWidget[1] { m_Preview.button }
		};
		obj[13] = new Group
		{
			displayName = "Editor.THUMBNAIL",
			tooltip = "Editor.THUMBNAIL_TOOLTIP",
			children = new IWidget[1] { m_Thumbnail.button }
		};
		editorSection2.children = obj;
		array2[0] = editorSection;
		array2[1] = new EditorSection
		{
			displayName = "Editor.CHECKLIST",
			tooltip = "Editor.CHECKLIST_TOOLTIP",
			children = new IWidget[2]
			{
				new Group
				{
					displayName = "Editor.CHECKLIST_REQUIRED",
					tooltip = "Editor.CHECKLIST_REQUIRED_TOOLTIP",
					children = new IWidget[4]
					{
						new ToggleField
						{
							displayName = "Editor.CHECKLIST_STARTING_TILES",
							tooltip = "Editor.CHECKLIST_STARTING_TILES_TOOLTIP",
							disabled = () => true,
							accessor = new DelegateAccessor<bool>(() => m_MapRequirementSystem.hasStartingArea)
						},
						new ToggleField
						{
							displayName = "Editor.CHECKLIST_WATER",
							tooltip = "Editor.CHECKLIST_WATER_TOOLTIP",
							disabled = () => true,
							accessor = new DelegateAccessor<bool>(() => m_MapRequirementSystem.StartingAreaHasResource(MapFeature.SurfaceWater) || m_MapRequirementSystem.StartingAreaHasResource(MapFeature.GroundWater))
						},
						new ToggleField
						{
							displayName = "Editor.CHECKLIST_ROAD_CONNECTION",
							tooltip = "Editor.CHECKLIST_ROAD_CONNECTION_TOOLTIP",
							disabled = () => true,
							accessor = new DelegateAccessor<bool>(() => m_MapRequirementSystem.roadConnection)
						},
						new ToggleField
						{
							displayName = "Editor.CHECKLIST_NAME",
							tooltip = "Editor.CHECKLIST_NAME_TOOLTIP",
							disabled = () => true,
							accessor = new DelegateAccessor<bool>(() => m_MapNameLocalization.IsValid())
						}
					}
				},
				new Group
				{
					displayName = "Editor.CHECKLIST_OPTIONAL",
					tooltip = "Editor.CHECKLIST_OPTIONAL_TOOLTIP",
					children = new IWidget[7]
					{
						new ToggleField
						{
							displayName = "Editor.CHECKLIST_TRAIN_CONNECTION",
							tooltip = "Editor.CHECKLIST_TRAIN_CONNECTION_TOOLTIP",
							disabled = () => true,
							accessor = new DelegateAccessor<bool>(() => m_MapRequirementSystem.trainConnection)
						},
						new ToggleField
						{
							displayName = "Editor.CHECKLIST_AIR_CONNECTION",
							tooltip = "Editor.CHECKLIST_AIR_CONNECTION_TOOLTIP",
							disabled = () => true,
							accessor = new DelegateAccessor<bool>(() => m_MapRequirementSystem.airConnection)
						},
						new ToggleField
						{
							displayName = "Editor.CHECKLIST_ELECTRICITY_CONNECTION",
							tooltip = "Editor.CHECKLIST_ELECTRICITY_CONNECTION_TOOLTIP",
							disabled = () => true,
							accessor = new DelegateAccessor<bool>(() => m_MapRequirementSystem.electricityConnection)
						},
						new ToggleField
						{
							displayName = "Editor.CHECKLIST_OIL",
							tooltip = "Editor.CHECKLIST_OIL_TOOLTIP",
							disabled = () => true,
							accessor = new DelegateAccessor<bool>(() => m_MapRequirementSystem.MapHasResource(MapFeature.Oil))
						},
						new ToggleField
						{
							displayName = "Editor.CHECKLIST_ORE",
							tooltip = "Editor.CHECKLIST_ORE_TOOLTIP",
							disabled = () => true,
							accessor = new DelegateAccessor<bool>(() => m_MapRequirementSystem.MapHasResource(MapFeature.Ore))
						},
						new ToggleField
						{
							displayName = "Editor.CHECKLIST_FOREST",
							tooltip = "Editor.CHECKLIST_FOREST_TOOLTIP",
							disabled = () => true,
							accessor = new DelegateAccessor<bool>(() => m_MapRequirementSystem.MapHasResource(MapFeature.Forest))
						},
						new ToggleField
						{
							displayName = "Editor.CHECKLIST_FERTILE",
							tooltip = "Editor.CHECKLIST_FERTILE_TOOLTIP",
							disabled = () => true,
							accessor = new DelegateAccessor<bool>(() => m_MapRequirementSystem.MapHasResource(MapFeature.FertileLand))
						}
					}
				}
			}
		};
		array[0] = Scrollable.WithChildren(array2);
		array[1] = ButtonRow.WithChildren(new Button[3]
		{
			new Button
			{
				displayName = "Editor.LOAD_MAP",
				tooltip = "Editor.LOAD_MAP_TOOLTIP",
				action = ShowLoadMapPanel
			},
			new Button
			{
				displayName = "Editor.SAVE_MAP",
				tooltip = "Editor.SAVE_MAP_TOOLTIP",
				action = ShowSaveMapPanel
			},
			new Button
			{
				displayName = "GameListScreen.GAME_OPTION[shareMap]",
				action = ShowShareMapPanel,
				hidden = () => m_Platform == null || !m_Platform.cachedLoggedIn
			}
		});
		children = array;
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

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		if ((int)((Context)(ref serializationContext)).purpose == 4 || (int)((Context)(ref serializationContext)).purpose == 5)
		{
			if (((EntityQuery)(ref m_TimeQuery)).IsEmptyIgnoreFilter)
			{
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				Entity val = ((EntityManager)(ref entityManager)).CreateEntity();
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponentData<TimeData>(val, new TimeData
				{
					m_StartingYear = DateTime.Now.Year,
					m_StartingMonth = 6,
					m_StartingHour = 6,
					m_StartingMinutes = 0
				});
			}
			m_RequiredListWidget.SetPropertiesChanged();
			FetchThemes();
			FetchTime();
			m_CurrentSourceDataGuid = ((Context)(ref serializationContext)).instigatorGuid;
			MapMetadata asset = AssetDatabase.global.GetAsset<MapMetadata>(m_CurrentSourceDataGuid);
			m_MapMetadataSystem.mapName = ((!string.IsNullOrEmpty(((Metadata<MapInfo>)asset)?.target?.displayName)) ? ((Metadata<MapInfo>)asset).target.displayName : Guid.NewGuid().ToString());
			InitLocalization(asset);
			InitPreview(asset);
		}
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		((COSystemBase)this).OnStartRunning();
		base.activeSubPanel = null;
		((ComponentSystemBase)m_MapRequirementSystem).Enabled = true;
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		((COSystemBase)this).OnStopRunning();
		m_MapTilePurchaseSystem.selecting = false;
		((ComponentSystemBase)m_MapRequirementSystem).Enabled = false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.OnUpdate();
		((ComponentSystemBase)m_MapRequirementSystem).Update();
		if (m_MapTilePurchaseSystem.selecting)
		{
			UpdateMapTileButton(kStopSelectingStartingTilesPrompt);
			UpdateStartingTiles();
		}
		else
		{
			UpdateMapTileButton(kSelectStartingTilesPrompt);
		}
	}

	private void FetchThemes()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		List<IconButton> list = new List<IconButton>();
		NativeArray<Entity> val = ((EntityQuery)(ref m_ThemeQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		Enumerator<Entity> enumerator = val.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Entity theme = enumerator.Current;
				ThemePrefab prefab = m_PrefabSystem.GetPrefab<ThemePrefab>(theme);
				list.Add(new IconButton
				{
					icon = (ImageSystem.GetIcon(prefab) ?? "Media/Editor/Object.svg"),
					tooltip = LocalizedString.Id("Assets.THEME[" + ((Object)prefab).name + "]"),
					action = delegate
					{
						//IL_000c: Unknown result type (might be due to invalid IL or missing references)
						m_CityConfigurationSystem.defaultTheme = theme;
					},
					selected = () => m_CityConfigurationSystem.defaultTheme == theme
				});
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		val.Dispose();
		m_ThemeButtonGroup.children = list.ToArray();
	}

	private void FetchTime()
	{
		TimeData singleton = ((EntityQuery)(ref m_TimeQuery)).GetSingleton<TimeData>();
		m_StartingYear = singleton.m_StartingYear;
		m_CurrentYearAsStartingYear = true;
		m_StartingMonth = singleton.m_StartingMonth + 1;
		m_StartingTime = singleton.TimeOffset;
	}

	private void SetStartingYear(int value)
	{
		m_StartingYear = value;
		ApplyTime();
	}

	private void SetStartingMonth(int value)
	{
		m_StartingMonth = value;
		ApplyTime();
	}

	private void SetStartingTime(float value)
	{
		m_StartingTime = value;
		ApplyTime();
	}

	private void ApplyTime()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		TimeData timeData = new TimeData
		{
			m_StartingYear = m_StartingYear,
			m_StartingMonth = (byte)(m_StartingMonth - 1),
			TimeOffset = m_StartingTime
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		Entity singletonEntity = ((EntityQuery)(ref m_TimeQuery)).GetSingletonEntity();
		((EntityCommandBuffer)(ref val)).SetComponent<TimeData>(singletonEntity, timeData);
	}

	public void ShowLoadMapPanel()
	{
		base.activeSubPanel = new LoadAssetPanel("Editor.LOAD_MAP", GetMaps(), OnLoadMap, base.CloseSubPanel);
	}

	public void ShowSaveMapPanel()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		base.activeSubPanel = new SaveAssetPanel("Editor.SAVE_MAP", GetMaps(), m_CurrentSourceDataGuid, delegate(string name, Hash128? overwriteGuid)
		{
			OnSaveMap(name, overwriteGuid);
		}, base.CloseSubPanel);
	}

	private void ShowShareMapPanel()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		base.activeSubPanel = new SaveAssetPanel("Editor.SAVE_MAP_SHARE", GetMaps(), m_CurrentSourceDataGuid, delegate(string name, Hash128? overwriteGuid)
		{
			OnSaveMap(name, overwriteGuid, ShareMap);
		}, base.CloseSubPanel, "Editor.SAVE_SHARE");
	}

	private IEnumerable<AssetItem> GetMaps()
	{
		foreach (MapMetadata asset in AssetDatabase.global.GetAssets<MapMetadata>(default(SearchFilter<MapMetadata>)))
		{
			MapMetadata mapMetadata = asset;
			try
			{
				if (!(((AssetData)asset).database is AssetDatabase<Game>) && TryGetAssetItem(asset, out var item))
				{
					yield return item;
				}
			}
			finally
			{
				((IDisposable)mapMetadata)?.Dispose();
			}
		}
	}

	private bool TryGetAssetItem(MapMetadata asset, out AssetItem item)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			MapInfo target = ((Metadata<MapInfo>)asset).target;
			SourceMeta meta = ((AssetData)asset).GetMeta();
			item = new AssetItem
			{
				guid = Identifier.op_Implicit(((AssetData)asset).id),
				fileName = meta.fileName,
				displayName = meta.displayName,
				image = UIExtensions.ToUri(target.thumbnail, MenuHelpers.defaultPreview, 0),
				badge = ((meta.remoteStorageSourceName != "Local") ? meta.remoteStorageSourceName : null)
			};
			return true;
		}
		catch (Exception ex)
		{
			base.log.Error(ex);
			item = null;
		}
		return false;
	}

	private void ShowPreviewPicker()
	{
		base.activeSubPanel = new LoadAssetPanel("Editor.PREVIEW", EditorPrefabUtils.GetUserImages(), OnSelectPreview, base.CloseSubPanel);
	}

	private void ShowThumbnailPicker()
	{
		base.activeSubPanel = new LoadAssetPanel("Editor.THUMBNAIL", EditorPrefabUtils.GetUserImages(), OnSelectThumbnail, base.CloseSubPanel);
	}

	private void OnSelectPreview(Hash128 guid)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		m_Preview.Set(AssetDatabase.global.GetAsset<ImageAsset>(guid));
		CloseSubPanel();
	}

	private void OnSelectThumbnail(Hash128 guid)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		m_Thumbnail.Set(AssetDatabase.global.GetAsset<ImageAsset>(guid));
		CloseSubPanel();
	}

	private void OnLoadMap(Hash128 guid)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		GameManager.instance.userInterface.appBindings.ShowConfirmationDialog(new ConfirmationDialog(null, "Common.DIALOG_MESSAGE[ProgressLoss]", "Common.DIALOG_ACTION[Yes]", "Common.DIALOG_ACTION[No]"), delegate(int ret)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			if (ret == 0)
			{
				LoadMap(guid);
			}
		});
	}

	public async Task LoadMap(Hash128 guid)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		CloseSubPanel();
		MapMetadata asset = default(MapMetadata);
		if (AssetDatabase.global.TryGetAsset<MapMetadata>(guid, ref asset))
		{
			await GameManager.instance.Load(GameMode.Editor, (Purpose)5, (IAssetData)(object)asset).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private void InitPreview(MapMetadata asset = null)
	{
		m_Preview.Set(((Metadata<MapInfo>)asset)?.target?.preview, MenuHelpers.defaultPreview);
		m_Thumbnail.Set(((Metadata<MapInfo>)asset)?.target?.thumbnail, MenuHelpers.defaultThumbnail);
	}

	private void InitLocalization(MapMetadata asset = null)
	{
		if ((AssetData)(object)asset != (IAssetData)null)
		{
			m_MapNameLocalization.Initialize(((Metadata<MapInfo>)asset).target.localeAssets, $"Maps.MAP_TITLE[{((Metadata<MapInfo>)asset).target.displayName}]");
			m_MapDescriptionLocalization.Initialize(((Metadata<MapInfo>)asset).target.localeAssets, $"Maps.MAP_DESCRIPTION[{((Metadata<MapInfo>)asset).target.displayName}]");
		}
		else
		{
			m_MapNameLocalization.Initialize();
			m_MapDescriptionLocalization.Initialize();
		}
	}

	private void OnSaveMap(string fileName, Hash128? overwriteGuid, Action<MapMetadata> callback = null)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		((ComponentSystemBase)m_MapMetadataSystem).Update();
		if (overwriteGuid.HasValue)
		{
			GameManager.instance.userInterface.appBindings.ShowConfirmationDialog(new ConfirmationDialog(null, "Common.DIALOG_MESSAGE[OverwriteMap]", "Common.DIALOG_ACTION[Yes]", "Common.DIALOG_ACTION[No]"), delegate(int ret)
			{
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				if (ret == 0)
				{
					CloseSubPanel();
					MapMetadata asset = AssetDatabase.global.GetAsset<MapMetadata>(overwriteGuid.Value);
					SourceMeta meta = ((AssetData)asset).GetMeta();
					SaveMap(meta.displayName, overwriteGuid.Value, ((Metadata<MapInfo>)asset).target, ((AssetData)asset).database, AssetDataPath.Create(meta.subPath, meta.fileName, (EscapeStrategy)2), ((AssetData)asset).database != AssetDatabase.game, callback);
				}
			});
		}
		else
		{
			CloseSubPanel();
			SaveMap(fileName, Hash128.Empty, null, AssetDatabase.user, SaveHelpers.GetAssetDataPath<MapMetadata>(AssetDatabase.user, fileName), embedLocalization: true, callback);
		}
	}

	public unsafe async Task SaveMap(string fileName, Hash128 overwriteGuid, MapInfo existing, ILocalAssetDatabase finalDb, AssetDataPath packagePath, bool embedLocalization, Action<MapMetadata> callback = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		m_MapMetadataSystem.mapName = fileName;
		ILocalAssetDatabase db = AssetDatabase.GetTransient(0L, (string)null);
		try
		{
			MapInfo info = GetMapInfo(existing);
			AssetDataPath val = AssetDataPath.op_Implicit(fileName);
			MapMetadata meta = db.AddAsset<MapMetadata>(val, overwriteGuid);
			((Metadata<MapInfo>)meta).target = info;
			MapData val2 = (info.mapData = db.AddAsset<MapData>(val, default(Hash128)));
			info.climate = SaveClimate(db);
			m_CurrentSourceDataGuid = Identifier.op_Implicit(((AssetData)meta).id);
			m_SaveGameSystem.context = new Context((Purpose)3, Version.current, m_CurrentSourceDataGuid);
			m_SaveGameSystem.stream = ((AssetData)val2).GetWriteStream();
			await m_SaveGameSystem.RunOnce();
			string[] array = ((IEnumerable<Entity>)(object)m_SaveGameSystem.referencedContent).Select((Entity x) => m_PrefabSystem.GetPrefabName(x)).ToArray();
			info.contentPrerequisites = ((array.Length != 0) ? array : null);
			if (m_Preview.CopyToTextureAsset(db, AssetDataPath.op_Implicit(m_Preview.name), out var asset))
			{
				asset.Save(0, false);
				info.preview = asset;
			}
			TextureAsset asset2;
			if (m_Preview.Equals(m_Thumbnail))
			{
				info.thumbnail = asset;
			}
			else if (m_Thumbnail.CopyToTextureAsset(db, AssetDataPath.op_Implicit(m_Thumbnail.name), out asset2))
			{
				asset2.Save(0, false);
				info.thumbnail = asset2;
			}
			if (embedLocalization)
			{
				info.localeAssets = SaveLocalization(db, fileName);
			}
			else
			{
				info.localeAssets = null;
			}
			((AssetData)meta).Save(false);
			DisableNotificationsScoped val4 = AssetDatabase.global.DisableNotificationsScoped();
			try
			{
				PackageAsset val5 = default(PackageAsset);
				if (finalDb.Exists<PackageAsset>(packagePath, ref val5))
				{
					Identifier id = ((AssetData)val5).id;
					((IAssetDatabase)finalDb).DeleteAsset<PackageAsset>(val5);
					val5 = finalDb.AddAsset<PackageAsset, ILocalAssetDatabase>(packagePath, db, Identifier.op_Implicit(id));
					((AssetData)val5).Save(false);
				}
				else
				{
					val5 = PackageAssetExtensions.AddAsset(finalDb, packagePath, db);
					((AssetData)val5).Save(false);
				}
			}
			finally
			{
				((IDisposable)(*(DisableNotificationsScoped*)(&val4))/*cast due to .constrained prefix*/).Dispose();
			}
			if (((IDataSourceAccessor)finalDb).dataSource.hasCache)
			{
				string text = await ((IAssetDatabase)finalDb).ResaveCache();
				if (!string.IsNullOrWhiteSpace(text))
				{
					Debug.Log((object)text);
				}
			}
			GameManager.instance.RunOnMainThread(delegate
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				PlatformManager.instance.UnlockAchievement(Game.Achievements.Achievements.Cartography);
				InitPreview(meta);
				if (callback != null)
				{
					callback(meta);
				}
			});
		}
		finally
		{
			((IDisposable)db)?.Dispose();
		}
	}

	private PrefabAsset SaveClimate(ILocalAssetDatabase database)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		ClimateSystem orCreateSystemManaged = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		ClimatePrefab prefab = m_PrefabSystem.GetPrefab<ClimatePrefab>(orCreateSystemManaged.currentClimate);
		if (prefab.builtin)
		{
			return null;
		}
		PrefabBase prefabBase = prefab.Clone();
		((Object)prefabBase).name = ((Object)prefab).name;
		prefabBase.asset = database.AddAsset<PrefabAsset, ScriptableObject>(AssetDataPath.op_Implicit(((Object)prefabBase).name), (ScriptableObject)(object)prefabBase, default(Hash128));
		((AssetData)prefabBase.asset).Save(false);
		return prefabBase.asset;
	}

	private LocaleAsset[] SaveLocalization(ILocalAssetDatabase db, string fileName)
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, LocaleData> dictionary = new Dictionary<string, LocaleData>();
		m_MapNameLocalization.BuildLocaleData($"Maps.MAP_TITLE[{m_MapMetadataSystem.mapName}]", dictionary, m_MapMetadataSystem.mapName);
		m_MapDescriptionLocalization.BuildLocaleData($"Maps.MAP_DESCRIPTION[{m_MapMetadataSystem.mapName}]", dictionary);
		List<LocaleAsset> list = new List<LocaleAsset>(dictionary.Keys.Count);
		foreach (string key in dictionary.Keys)
		{
			LocaleAsset val = db.AddAsset<LocaleAsset>(AssetDataPath.op_Implicit(fileName + "_" + key), default(Hash128));
			LocalizationManager localizationManager = GameManager.instance.localizationManager;
			val.SetData(dictionary[key], localizationManager.LocaleIdToSystemLanguage(key), GameManager.instance.localizationManager.GetLocalizedName(key));
			((AssetData)val).Save(false);
			list.Add(val);
		}
		return list.ToArray();
	}

	private void ShareMap(MapMetadata map)
	{
		m_AssetUploadPanel.Show((AssetData)(object)map);
		base.activeSubPanel = m_AssetUploadPanel;
	}

	private MapInfo GetMapInfo(MapInfo merge = null)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		MapInfo obj = merge ?? new MapInfo();
		obj.displayName = m_MapMetadataSystem.mapName;
		obj.theme = m_MapMetadataSystem.theme;
		obj.temperatureRange = m_MapMetadataSystem.temperatureRange;
		obj.cloudiness = m_MapMetadataSystem.cloudiness;
		obj.precipitation = m_MapMetadataSystem.precipitation;
		obj.latitude = m_MapMetadataSystem.latitude;
		obj.longitude = m_MapMetadataSystem.longitude;
		obj.area = m_MapMetadataSystem.area;
		obj.surfaceWaterAvailability = m_MapMetadataSystem.surfaceWaterAvailability;
		obj.groundWaterAvailability = m_MapMetadataSystem.groundWaterAvailability;
		obj.resources = m_MapMetadataSystem.resources;
		obj.connections = m_MapMetadataSystem.connections;
		obj.nameAsCityName = m_MapNameAsCityName;
		obj.startingYear = (m_CurrentYearAsStartingYear ? (-1) : m_StartingYear);
		obj.buildableLand = m_MapMetadataSystem.buildableLand;
		return obj;
	}

	private void CaptureCameraProperties()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (CameraController.TryGet(out var cameraController))
		{
			m_CityConfigurationSystem.m_CameraPivot = float3.op_Implicit(cameraController.pivot);
			m_CityConfigurationSystem.m_CameraAngle = cameraController.angle;
			m_CityConfigurationSystem.m_CameraZoom = cameraController.zoom;
		}
	}

	private void ToggleMapTileSelection()
	{
		m_MapTilePurchaseSystem.selecting = !m_MapTilePurchaseSystem.selecting;
	}

	private void UpdateStartingTiles()
	{
	}

	private void UpdateMapTileButton(string text)
	{
		if (m_MapTileSelectionButton.displayName.value != text)
		{
			m_MapTileSelectionButton.displayName = text;
			m_MapTileSelectionButton.SetPropertiesChanged();
		}
	}

	[Preserve]
	public MapPanelSystem()
	{
	}
}
