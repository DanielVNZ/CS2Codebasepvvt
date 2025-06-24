using System;
using System.Collections.Generic;
using System.IO;
using Colossal;
using Colossal.AssetPipeline.Native;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.Serialization.Entities;
using Colossal.UI;
using Game.Common;
using Game.Prefabs;
using Game.Reflection;
using Game.SceneFlow;
using Game.Simulation;
using Game.Tools;
using Game.UI.Localization;
using Game.UI.Widgets;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.UI.Editor;

public class TerrainPanelSystem : EditorPanelSystemBase
{
	private struct UIPriorityComparer : IComparer<PrefabData>
	{
		private PrefabSystem m_PrefabSystem;

		public UIPriorityComparer(PrefabSystem prefabSystem)
		{
			m_PrefabSystem = prefabSystem;
		}

		public int Compare(PrefabData a, PrefabData b)
		{
			TerraformingPrefab prefab = m_PrefabSystem.GetPrefab<TerraformingPrefab>(a);
			PrefabBase prefab2 = m_PrefabSystem.GetPrefab<TerraformingPrefab>(b);
			if (prefab.TryGet<UIObject>(out var component) && prefab2.TryGet<UIObject>(out var component2))
			{
				return component.m_Priority - component2.m_Priority;
			}
			return -1;
		}
	}

	private static readonly string kHeightmapFolder = "Heightmaps";

	private PrefabSystem m_PrefabSystem;

	private ToolSystem m_ToolSystem;

	private TerrainSystem m_TerrainSystem;

	private EntityQuery m_PrefabQuery;

	private IconButtonGroup m_ToolButtonGroup;

	private IconButtonGroup m_MaterialButtonGroup = new IconButtonGroup();

	private List<PrefabBase> m_ToolPrefabs = new List<PrefabBase>();

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<TerraformingData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		title = "Editor.TOOL[TerrainTool]";
		children = new IWidget[1] { Scrollable.WithChildren(new IWidget[2]
		{
			new EditorSection
			{
				displayName = "Editor.TERRAIN_TOOLS",
				uiTag = "UITagPrefab:TerrainTools",
				tooltip = "Editor.TERRAIN_TOOLS_TOOLTIP",
				expanded = true,
				children = new IWidget[1] { m_ToolButtonGroup = new IconButtonGroup() }
			},
			new EditorSection
			{
				displayName = "Editor.HEIGHTMAPS",
				tooltip = "Editor.HEIGHTMAPS_TOOLTIP",
				uiTag = "UITagPrefab:Heightmaps",
				expanded = true,
				children = new IWidget[3]
				{
					new FloatInputField
					{
						displayName = "Editor.HEIGHT_SCALE",
						tooltip = "Editor.HEIGHT_SCALE_TOOLTIP",
						uiTag = "UITagPrefab:HeightScale",
						accessor = new DelegateAccessor<double>(() => m_TerrainSystem.heightScaleOffset.x, delegate(double num)
						{
							//IL_0006: Unknown result type (might be due to invalid IL or missing references)
							//IL_000b: Unknown result type (might be due to invalid IL or missing references)
							//IL_0016: Unknown result type (might be due to invalid IL or missing references)
							float2 heightScaleOffset = m_TerrainSystem.heightScaleOffset;
							heightScaleOffset.x = (float)num;
							RefreshTerrainProperties(heightScaleOffset);
						}),
						min = 200.0,
						max = 10000.0
					},
					Column.WithChildren(new Button[2]
					{
						new Button
						{
							displayName = "Editor.IMPORT_HEIGHTMAP",
							tooltip = "Editor.IMPORT_HEIGHTMAP_TOOLTIP",
							uiTag = "UITagPrefab:ImportHeightmap",
							action = ShowImportHeightmapPanel
						},
						new Button
						{
							displayName = "Editor.EXPORT_HEIGHTMAP",
							uiTag = "UITagPrefab:ExportHeightmap",
							tooltip = "Editor.EXPORT_HEIGHTMAP_TOOLTIP",
							action = ShowExportHeightmapPanel
						}
					}),
					Column.WithChildren(new Button[3]
					{
						new Button
						{
							displayName = "Editor.IMPORT_WORLDMAP",
							tooltip = "Editor.IMPORT_WORLDMAP_TOOLTIP",
							uiTag = "UITagPrefab:ImportWorldmap",
							action = ShowImportWorldmapPanel
						},
						new Button
						{
							displayName = "Editor.EXPORT_WORLDMAP",
							tooltip = "Editor.EXPORT_WORLDMAP_TOOLTIP",
							action = ShowExportWorldmapPanel,
							disabled = () => (Object)(object)m_TerrainSystem.worldHeightmap == (Object)null
						},
						new Button
						{
							displayName = "Editor.REMOVE_WORLDMAP",
							tooltip = "Editor.REMOVE_WORLDMAP_TOOLTIP",
							action = RemoveWorldmap,
							disabled = () => (Object)(object)m_TerrainSystem.worldHeightmap == (Object)null
						}
					})
				}
			}
		}) };
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		List<IconButton> list = new List<IconButton>();
		List<IconButton> list2 = new List<IconButton>();
		NativeArray<PrefabData> val = ((EntityQuery)(ref m_PrefabQuery)).ToComponentDataArray<PrefabData>(AllocatorHandle.op_Implicit((Allocator)2));
		NativeSortExtension.Sort<PrefabData, UIPriorityComparer>(val, new UIPriorityComparer(m_PrefabSystem));
		m_ToolPrefabs.Clear();
		Enumerator<PrefabData> enumerator = val.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				PrefabData current = enumerator.Current;
				TerraformingPrefab prefab = m_PrefabSystem.GetPrefab<TerraformingPrefab>(current);
				if (prefab.m_Target == TerraformingTarget.Height)
				{
					m_ToolPrefabs.Add(prefab);
					list.Add(new IconButton
					{
						icon = (ImageSystem.GetIcon(prefab) ?? "Media/Editor/Terrain.svg"),
						tooltip = LocalizedString.Id("Assets.NAME[" + ((Object)prefab).name + "]"),
						action = delegate
						{
							m_ToolSystem.ActivatePrefabTool(((Object)(object)m_ToolSystem.activePrefab != (Object)(object)prefab) ? prefab : null);
						},
						selected = () => (Object)(object)m_ToolSystem.activePrefab == (Object)(object)prefab
					});
				}
				if (prefab.m_Target == TerraformingTarget.Material)
				{
					m_ToolPrefabs.Add(prefab);
					list2.Add(new IconButton
					{
						icon = (ImageSystem.GetIcon(prefab) ?? "Media/Editor/Material.svg"),
						tooltip = LocalizedString.Id("Assets.NAME[" + ((Object)prefab).name + "]"),
						action = delegate
						{
							m_ToolSystem.ActivatePrefabTool(((Object)(object)m_ToolSystem.activePrefab != (Object)(object)prefab) ? prefab : null);
						},
						selected = () => (Object)(object)m_ToolSystem.activePrefab == (Object)(object)prefab
					});
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		val.Dispose();
		m_ToolButtonGroup.children = list.ToArray();
		m_MaterialButtonGroup.children = list2.ToArray();
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		((COSystemBase)this).OnStartRunning();
		base.activeSubPanel = null;
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		if (m_ToolPrefabs.Contains(m_ToolSystem.activePrefab))
		{
			m_ToolSystem.ActivatePrefabTool(null);
		}
		((COSystemBase)this).OnStopRunning();
	}

	protected override bool OnCancel()
	{
		if (m_ToolPrefabs.Contains(m_ToolSystem.activePrefab))
		{
			m_ToolSystem.ActivatePrefabTool(null);
			return false;
		}
		return base.OnCancel();
	}

	private void ShowImportHeightmapPanel()
	{
		base.activeSubPanel = new LoadAssetPanel("Editor.IMPORT_HEIGHTMAP", GetHeightmaps(), OnLoadHeightmap, base.CloseSubPanel);
	}

	private void ShowExportHeightmapPanel()
	{
		base.activeSubPanel = new SaveAssetPanel("Editor.EXPORT_HEIGHTMAP", GetHeightmaps(), null, OnSaveHeightmap, base.CloseSubPanel);
	}

	private void ShowImportWorldmapPanel()
	{
		base.activeSubPanel = new LoadAssetPanel("Editor.IMPORT_WORLDMAP", GetHeightmaps(), OnLoadWorldHeightmap, base.CloseSubPanel);
	}

	private void ShowExportWorldmapPanel()
	{
		base.activeSubPanel = new SaveAssetPanel("Editor.EXPORT_WORLDMAP", GetHeightmaps(), null, OnSaveWorldHeightmap, base.CloseSubPanel);
	}

	private void RemoveWorldmap()
	{
		m_TerrainSystem.ReplaceWorldHeightmap(null);
	}

	private void RefreshTerrainProperties(float2 heightScaleOffset)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_TerrainSystem.SetTerrainProperties(heightScaleOffset);
	}

	private static IEnumerable<AssetItem> GetHeightmaps()
	{
		foreach (ImageAsset asset in AssetDatabase.global.GetAssets<ImageAsset>(SearchFilter<ImageAsset>.ByCondition((Func<ImageAsset, bool>)((ImageAsset a) => ((AssetData)a).GetMeta().subPath?.StartsWith(kHeightmapFolder) ?? false), false)))
		{
			yield return new AssetItem
			{
				guid = Identifier.op_Implicit(((AssetData)asset).id),
				fileName = ((AssetData)asset).name,
				displayName = ((AssetData)asset).name,
				image = UIExtensions.ToUri((AssetData)(object)asset),
				tooltip = ((AssetData)asset).name
			};
		}
	}

	private void OnLoadHeightmap(Hash128 guid)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		CloseSubPanel();
		ImageAsset val = default(ImageAsset);
		if (!AssetDatabase.global.TryGetAsset<ImageAsset>(guid, ref val))
		{
			return;
		}
		ImageAsset val2 = val;
		try
		{
			Texture2D val3 = val.Load(true);
			if (!TerrainSystem.IsValidHeightmapFormat(val3))
			{
				DisplayHeightmapError();
			}
			else
			{
				m_TerrainSystem.ReplaceHeightmap(val3);
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private void OnLoadWorldHeightmap(Hash128 guid)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		CloseSubPanel();
		ImageAsset val = default(ImageAsset);
		if (!AssetDatabase.global.TryGetAsset<ImageAsset>(guid, ref val))
		{
			return;
		}
		ImageAsset val2 = val;
		try
		{
			Texture2D val3 = val.Load(true);
			if (!TerrainSystem.IsValidHeightmapFormat(val3))
			{
				DisplayHeightmapError();
			}
			else
			{
				m_TerrainSystem.ReplaceWorldHeightmap(val3);
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private void DisplayHeightmapError()
	{
		GameManager.instance.userInterface.appBindings.ShowMessageDialog(new MessageDialog(LocalizedString.Id("Editor.INCORRECT_HEIGHTMAP_TITLE"), new LocalizedString("Editor.INCORRECT_HEIGHTMAP_MESSAGE", null, new Dictionary<string, ILocElement>
		{
			{
				"WIDTH",
				LocalizedString.Value(TerrainSystem.kDefaultHeightmapWidth.ToString())
			},
			{
				"HEIGHT",
				LocalizedString.Value(TerrainSystem.kDefaultHeightmapHeight.ToString())
			}
		}), LocalizedString.Id("Common.ERROR_DIALOG_CONTINUE")), null);
	}

	private void OnSaveHeightmap(string fileName, Hash128? overwriteGuid)
	{
		OnSaveHeightmap(fileName, overwriteGuid, worldMap: false);
	}

	private void OnSaveWorldHeightmap(string fileName, Hash128? overwriteGuid)
	{
		OnSaveHeightmap(fileName, overwriteGuid, worldMap: true);
	}

	private unsafe void OnSaveHeightmap(string fileName, Hash128? overwriteGuid, bool worldMap)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		CloseSubPanel();
		bool flag = false;
		ImageAsset val = default(ImageAsset);
		if (overwriteGuid.HasValue && ((IAssetDatabase)AssetDatabase.user).TryGetAsset<ImageAsset>(overwriteGuid.Value, ref val))
		{
			((IAssetDatabase)AssetDatabase.user).DeleteAsset<ImageAsset>(val);
			flag = true;
		}
		Texture val2 = (worldMap ? m_TerrainSystem.worldHeightmap : m_TerrainSystem.heightmap);
		NativeArray<ushort> val3 = default(NativeArray<ushort>);
		val3._002Ector(val2.width * val2.height, (Allocator)4, (NativeArrayOptions)1);
		AsyncGPUReadbackRequest val4 = AsyncGPUReadback.RequestIntoNativeArray<ushort>(ref val3, val2, 0, (Action<AsyncGPUReadbackRequest>)null);
		((AsyncGPUReadbackRequest)(ref val4)).WaitForCompletion();
		try
		{
			byte[] array = TextureUtilities.SaveImage((IntPtr)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr<ushort>(val3), (long)(val3.Length * 2), val2.width, val2.height, 1, 16, (ImageFileFormat)1, 4);
			AssetDataPath val5 = AssetDataPath.Create(kHeightmapFolder, fileName, (EscapeStrategy)2);
			using Stream stream = ((AssetData)AssetDatabase.user.AddAsset<ImageAsset>(val5, (Hash128)(flag ? overwriteGuid.Value : default(Hash128)))).GetWriteStream();
			stream.Write(array, 0, array.Length);
		}
		finally
		{
			val3.Dispose();
		}
	}

	[Preserve]
	public TerrainPanelSystem()
	{
	}
}
