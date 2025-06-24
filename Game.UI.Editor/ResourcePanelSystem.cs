using System;
using System.Collections.Generic;
using Colossal;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.Serialization.Entities;
using Colossal.UI;
using Game.Common;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Game.UI.Localization;
using Game.UI.Widgets;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Editor;

public class ResourcePanelSystem : EditorPanelSystemBase
{
	private static readonly string kTextureImportFolder = "Heightmaps";

	private PrefabSystem m_PrefabSystem;

	private ToolSystem m_ToolSystem;

	private NaturalResourceSystem m_NaturalResourceSystem;

	private GroundWaterSystem m_GroundWaterSystem;

	private EntityQuery m_PrefabQuery;

	private IconButtonGroup m_ToolButtonGroup;

	private EditorSection m_TextureImportButtons;

	private List<PrefabBase> m_ToolPrefabs = new List<PrefabBase>();

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_NaturalResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NaturalResourceSystem>();
		m_GroundWaterSystem = ((ComponentSystemBase)this).World.GetExistingSystemManaged<GroundWaterSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<TerraformingData>(),
			ComponentType.ReadOnly<UIObjectData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		title = "Editor.RESOURCES";
		IWidget[] array2 = new IWidget[1];
		IWidget[] obj = new IWidget[2]
		{
			new EditorSection
			{
				displayName = "Editor.RESOURCE_TOOLS",
				tooltip = "Editor.RESOURCE_TOOLS_TOOLTIP",
				expanded = true,
				children = new IWidget[1] { m_ToolButtonGroup = new IconButtonGroup() }
			},
			null
		};
		EditorSection obj2 = new EditorSection
		{
			displayName = "Editor.RESOURCE_TEXTURE_LABEL",
			tooltip = "Editor.RESOURCE_TEXTURE_LABEL_TOOLTIP",
			expanded = true
		};
		EditorSection editorSection = obj2;
		m_TextureImportButtons = obj2;
		obj[1] = editorSection;
		array2[0] = Scrollable.WithChildren(obj);
		children = array2;
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		m_ToolPrefabs.Clear();
		List<IconButton> list = new List<IconButton>();
		List<IWidget> list2 = new List<IWidget>();
		NativeList<UIObjectInfo> sortedObjects = UIObjectInfo.GetSortedObjects(m_PrefabQuery, (Allocator)2);
		try
		{
			Enumerator<UIObjectInfo> enumerator = sortedObjects.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					UIObjectInfo current = enumerator.Current;
					TerraformingPrefab prefab = m_PrefabSystem.GetPrefab<TerraformingPrefab>(current.prefabData);
					if (IsResourceTerraformingPrefab(prefab))
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
						list2.Add(new ButtonRow
						{
							children = new Button[2]
							{
								new Button
								{
									displayName = $"Editor.IMPORT_RESOURCE[{prefab.m_Target}]",
									tooltip = $"Editor.IMPORT_RESOURCE_TOOLTIP[{prefab.m_Target}]",
									action = delegate
									{
										ImportTexture(prefab.m_Target);
									}
								},
								new Button
								{
									displayName = $"Editor.CLEAR_RESOURCE[{prefab.m_Target}]",
									tooltip = $"Editor.CLEAR_RESOURCE_TOOLTIP[{prefab.m_Target}]",
									action = delegate
									{
										Clear(prefab.m_Target);
									}
								}
							}
						});
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			m_ToolButtonGroup.children = list.ToArray();
			m_TextureImportButtons.children = list2;
		}
		finally
		{
			((IDisposable)sortedObjects/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private static bool IsResourceTerraformingPrefab(TerraformingPrefab prefab)
	{
		if (prefab.m_Target != TerraformingTarget.Height && prefab.m_Target != TerraformingTarget.Material)
		{
			return prefab.m_Target != TerraformingTarget.None;
		}
		return false;
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		((COSystemBase)this).OnStopRunning();
		if (m_ToolPrefabs.Contains(m_ToolSystem.activePrefab))
		{
			m_ToolSystem.ActivatePrefabTool(null);
		}
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

	private void ImportTexture(TerraformingTarget target)
	{
		base.activeSubPanel = new LoadAssetPanel("Import " + target, GetTextures(), delegate(Hash128 guid)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			OnLoadTexture(guid, target);
		}, base.CloseSubPanel);
	}

	private static IEnumerable<AssetItem> GetTextures()
	{
		foreach (ImageAsset asset in AssetDatabase.global.GetAssets<ImageAsset>(SearchFilter<ImageAsset>.ByCondition((Func<ImageAsset, bool>)((ImageAsset a) => ((AssetData)a).GetMeta().subPath?.StartsWith(kTextureImportFolder) ?? false), false)))
		{
			yield return new AssetItem
			{
				guid = Identifier.op_Implicit(((AssetData)asset).id),
				fileName = ((AssetData)asset).name,
				displayName = ((AssetData)asset).name,
				image = UIExtensions.ToUri((AssetData)(object)asset)
			};
		}
	}

	private void OnLoadTexture(Hash128 guid, TerraformingTarget target)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		CloseSubPanel();
		ImageAsset val = default(ImageAsset);
		if (AssetDatabase.global.TryGetAsset<ImageAsset>(guid, ref val))
		{
			ImageAsset val2 = val;
			try
			{
				Texture2D texture = val.Load(true);
				ApplyTexture(texture, target);
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
	}

	private void ApplyTexture(Texture2D texture, TerraformingTarget target)
	{
		switch (target)
		{
		case TerraformingTarget.GroundWater:
			ApplyTexture<GroundWater>(texture, (CellMapSystem<GroundWater>)m_GroundWaterSystem, (Action<Texture2D, CellMapData<GroundWater>, int, int, Func<GroundWater, ushort, GroundWater>>)ApplyGroundWater, (Func<GroundWater, ushort, GroundWater>)null);
			break;
		case TerraformingTarget.Ore:
			ApplyTexture<NaturalResourceCell>(texture, (CellMapSystem<NaturalResourceCell>)m_NaturalResourceSystem, (Action<Texture2D, CellMapData<NaturalResourceCell>, int, int, Func<NaturalResourceCell, ushort, NaturalResourceCell>>)ApplyResource, (Func<NaturalResourceCell, ushort, NaturalResourceCell>)ApplyOre);
			break;
		case TerraformingTarget.Oil:
			ApplyTexture<NaturalResourceCell>(texture, (CellMapSystem<NaturalResourceCell>)m_NaturalResourceSystem, (Action<Texture2D, CellMapData<NaturalResourceCell>, int, int, Func<NaturalResourceCell, ushort, NaturalResourceCell>>)ApplyResource, (Func<NaturalResourceCell, ushort, NaturalResourceCell>)ApplyOil);
			break;
		case TerraformingTarget.FertileLand:
			ApplyTexture<NaturalResourceCell>(texture, (CellMapSystem<NaturalResourceCell>)m_NaturalResourceSystem, (Action<Texture2D, CellMapData<NaturalResourceCell>, int, int, Func<NaturalResourceCell, ushort, NaturalResourceCell>>)ApplyResource, (Func<NaturalResourceCell, ushort, NaturalResourceCell>)ApplyFertile);
			break;
		}
	}

	private void ApplyTexture<TCell>(Texture2D texture, CellMapSystem<TCell> cellMapSystem, Action<Texture2D, CellMapData<TCell>, int, int, Func<TCell, ushort, TCell>> applyCallback, Func<TCell, ushort, TCell> resourceCallback) where TCell : struct, ISerializable
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		CellMapData<TCell> data = cellMapSystem.GetData(readOnly: false, out dependencies);
		((JobHandle)(ref dependencies)).Complete();
		for (int i = 0; i < data.m_TextureSize.y; i++)
		{
			for (int j = 0; j < data.m_TextureSize.x; j++)
			{
				applyCallback(texture, data, j, i, resourceCallback);
			}
		}
	}

	private void ApplyResource<TCell>(Texture2D texture, CellMapData<TCell> data, int x, int y, Func<TCell, ushort, TCell> applyCallback) where TCell : struct, ISerializable
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		int num = y * data.m_TextureSize.x + x;
		TCell arg = data.m_Buffer[num];
		ushort arg2 = (ushort)Sample(texture, data, x, y, 10000);
		data.m_Buffer[num] = applyCallback(arg, arg2);
	}

	private void ApplyGroundWater(Texture2D texture, CellMapData<GroundWater> data, int x, int y, Func<GroundWater, ushort, GroundWater> _)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		int num = y * data.m_TextureSize.x + x;
		short num2 = (short)Sample<GroundWater>(texture, data, x, y, 10000);
		data.m_Buffer[num] = new GroundWater
		{
			m_Amount = num2,
			m_Max = num2
		};
	}

	private NaturalResourceCell ApplyOre(NaturalResourceCell cell, ushort amount)
	{
		cell.m_Ore = new NaturalResourceAmount
		{
			m_Base = amount
		};
		return cell;
	}

	private NaturalResourceCell ApplyOil(NaturalResourceCell cell, ushort amount)
	{
		cell.m_Oil = new NaturalResourceAmount
		{
			m_Base = amount
		};
		return cell;
	}

	private NaturalResourceCell ApplyFertile(NaturalResourceCell cell, ushort amount)
	{
		cell.m_Fertility = new NaturalResourceAmount
		{
			m_Base = amount
		};
		return cell;
	}

	private int Sample<TCell>(Texture2D texture, CellMapData<TCell> data, int x, int y, int max) where TCell : struct, ISerializable
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return Mathf.RoundToInt(math.saturate(texture.GetPixelBilinear((float)x / (float)(data.m_TextureSize.x - 1), (float)y / (float)(data.m_TextureSize.y - 1)).r) * (float)max);
	}

	private void Clear(TerraformingTarget target)
	{
		switch (target)
		{
		case TerraformingTarget.GroundWater:
			ClearMap<GroundWater>((CellMapSystem<GroundWater>)m_GroundWaterSystem, (Func<GroundWater, GroundWater>)ClearGroundWater);
			break;
		case TerraformingTarget.Ore:
			ClearMap<NaturalResourceCell>((CellMapSystem<NaturalResourceCell>)m_NaturalResourceSystem, (Func<NaturalResourceCell, NaturalResourceCell>)ClearOre);
			break;
		case TerraformingTarget.Oil:
			ClearMap<NaturalResourceCell>((CellMapSystem<NaturalResourceCell>)m_NaturalResourceSystem, (Func<NaturalResourceCell, NaturalResourceCell>)ClearOil);
			break;
		case TerraformingTarget.FertileLand:
			ClearMap<NaturalResourceCell>((CellMapSystem<NaturalResourceCell>)m_NaturalResourceSystem, (Func<NaturalResourceCell, NaturalResourceCell>)ClearFertile);
			break;
		}
	}

	private void ClearMap<TCell>(CellMapSystem<TCell> cellMapSystem, Func<TCell, TCell> clearCallback) where TCell : struct, ISerializable
	{
		JobHandle dependencies;
		CellMapData<TCell> data = cellMapSystem.GetData(readOnly: false, out dependencies);
		((JobHandle)(ref dependencies)).Complete();
		for (int i = 0; i < data.m_Buffer.Length; i++)
		{
			data.m_Buffer[i] = clearCallback(data.m_Buffer[i]);
		}
	}

	private NaturalResourceCell ClearOre(NaturalResourceCell cell)
	{
		cell.m_Ore = default(NaturalResourceAmount);
		return cell;
	}

	private NaturalResourceCell ClearOil(NaturalResourceCell cell)
	{
		cell.m_Oil = default(NaturalResourceAmount);
		return cell;
	}

	private NaturalResourceCell ClearFertile(NaturalResourceCell cell)
	{
		cell.m_Fertility = default(NaturalResourceAmount);
		return cell;
	}

	private GroundWater ClearGroundWater(GroundWater _)
	{
		return default(GroundWater);
	}

	[Preserve]
	public ResourcePanelSystem()
	{
	}
}
