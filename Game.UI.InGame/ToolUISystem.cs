using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Colossal.UI.Binding;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

public class ToolUISystem : UISystemBase
{
	public struct Brush : IJsonWritable
	{
		public Entity m_Entity;

		public string m_Name;

		public string m_Icon;

		public int m_Priority;

		public void Write(IJsonWriter writer)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("entity");
			UnityWriters.Write(writer, m_Entity);
			writer.PropertyName("name");
			writer.Write(m_Name);
			writer.PropertyName("icon");
			writer.Write(m_Icon);
			writer.PropertyName("priority");
			writer.Write(m_Priority);
			writer.TypeEnd();
		}
	}

	public const string kGroup = "tool";

	private ToolSystem m_ToolSystem;

	private NetToolSystem m_NetToolSystem;

	private AreaToolSystem m_AreaToolSystem;

	private ZoneToolSystem m_ZoneToolSystem;

	private RouteToolSystem m_RouteToolSystem;

	private ObjectToolSystem m_ObjectToolSystem;

	private TerrainToolSystem m_TerrainToolSystem;

	private TerrainSystem m_TerrainSystem;

	private DefaultToolSystem m_DefaultToolSystem;

	private UpgradeToolSystem m_UpgradeToolSystem;

	private BulldozeToolSystem m_BulldozeToolSystem;

	private SelectionToolSystem m_SelectionToolSystem;

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_BulldozeQuery;

	private EntityQuery m_BrushQuery;

	private RawValueBinding m_ActiveToolBinding;

	private List<ToolMode> m_ToolModes;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Expected O, but got Unknown
		//IL_01d0: Expected O, but got Unknown
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Expected O, but got Unknown
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Expected O, but got Unknown
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Expected O, but got Unknown
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Expected O, but got Unknown
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Expected O, but got Unknown
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_NetToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NetToolSystem>();
		m_AreaToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AreaToolSystem>();
		m_ZoneToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ZoneToolSystem>();
		m_RouteToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RouteToolSystem>();
		m_ObjectToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ObjectToolSystem>();
		m_TerrainToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainToolSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_DefaultToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DefaultToolSystem>();
		m_UpgradeToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpgradeToolSystem>();
		m_BulldozeToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BulldozeToolSystem>();
		m_SelectionToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SelectionToolSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_BulldozeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<BulldozeData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		m_BrushQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<BrushData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		ToolSystem toolSystem = m_ToolSystem;
		toolSystem.EventToolChanged = (Action<ToolBaseSystem>)Delegate.Combine(toolSystem.EventToolChanged, new Action<ToolBaseSystem>(OnToolChanged));
		ToolSystem toolSystem2 = m_ToolSystem;
		toolSystem2.EventPrefabChanged = (Action<PrefabBase>)Delegate.Combine(toolSystem2.EventPrefabChanged, new Action<PrefabBase>(OnPrefabChanged));
		BulldozeToolSystem bulldozeToolSystem = m_BulldozeToolSystem;
		bulldozeToolSystem.EventConfirmationRequested = (Action)Delegate.Combine(bulldozeToolSystem.EventConfirmationRequested, new Action(OnBulldozeConfirmationRequested));
		RawValueBinding val = new RawValueBinding("tool", "activeTool", (Action<IJsonWriter>)BindActiveTool);
		RawValueBinding binding = val;
		m_ActiveToolBinding = val;
		AddBinding((IBinding)(object)binding);
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<uint>("tool", "availableSnapMask", (Func<uint>)delegate
		{
			if (m_ToolSystem.activeTool == null)
			{
				return 0u;
			}
			m_ToolSystem.activeTool.GetAvailableSnapMask(out var onMask, out var offMask);
			return (uint)(onMask & offMask);
		}, (IWriter<uint>)null, (EqualityComparer<uint>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<uint>("tool", "allSnapMask", (Func<uint>)delegate
		{
			if (m_ToolSystem.activeTool == null)
			{
				return 0u;
			}
			m_ToolSystem.activeTool.GetAvailableSnapMask(out var onMask, out var offMask);
			return (uint)(onMask & offMask) & 0xFFF8FFFFu;
		}, (IWriter<uint>)null, (EqualityComparer<uint>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<uint>("tool", "selectedSnapMask", (Func<uint>)(() => (uint)((m_ToolSystem.activeTool != null) ? m_ToolSystem.activeTool.selectedSnap : Snap.None)), (IWriter<uint>)null, (EqualityComparer<uint>)null));
		AddBinding((IBinding)(object)new ValueBinding<string[]>("tool", "snapOptionNames", InitSnapOptionNames(), (IWriter<string[]>)(object)new ArrayWriter<string>((IWriter<string>)new StringWriter(), false), (EqualityComparer<string[]>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("tool", "colorSupported", (Func<bool>)GetColorSupported, (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<Color32>("tool", "color", (Func<Color32>)(() => (Color32)(((_003F?)m_ToolSystem.activeTool?.color) ?? default(Color32))), (IWriter<Color32>)null, (EqualityComparer<Color32>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<Bounds1>("tool", "elevationRange", (Func<Bounds1>)GetElevationRange, (IWriter<Bounds1>)null, (EqualityComparer<Bounds1>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("tool", "elevation", (Func<float>)(() => m_NetToolSystem.elevation), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("tool", "elevationStep", (Func<float>)(() => m_NetToolSystem.elevationStep), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("tool", "parallelModeSupported", (Func<bool>)GetParallelModeSupported, (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("tool", "parallelMode", (Func<bool>)(() => GetParallelModeSupported() && m_NetToolSystem.parallelCount != 0), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("tool", "parallelOffset", (Func<float>)(() => m_NetToolSystem.parallelOffset), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("tool", "undergroundModeSupported", (Func<bool>)(() => m_ToolSystem.activeTool != null && m_ToolSystem.activeTool.allowUnderground), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("tool", "undergroundMode", (Func<bool>)(() => m_ToolSystem.activeTool != null && m_ToolSystem.activeTool.requireUnderground), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("tool", "elevationDownDisabled", (Func<bool>)GetElevationDownDisabled, (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("tool", "elevationUpDisabled", (Func<bool>)GetElevationUpDisabled, (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("tool", "replacingTrees", (Func<bool>)(() => !m_ObjectToolSystem.GetNetUpgradeStates(out var _).IsEmpty), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("tool", "distance", (Func<float>)(() => m_ObjectToolSystem.distance), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("tool", "distanceScale", (Func<float>)(() => m_ObjectToolSystem.distanceScale), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string>("tool", "selectTool", (Action<string>)SelectTool, (IReader<string>)null));
		AddBinding((IBinding)(object)new TriggerBinding<int>("tool", "selectToolMode", (Action<int>)SelectToolMode, (IReader<int>)null));
		AddBinding((IBinding)(object)new TriggerBinding<uint>("tool", "setSelectedSnapMask", (Action<uint>)SetSelectedSnapMask, (IReader<uint>)null));
		AddBinding((IBinding)new TriggerBinding("tool", "elevationUp", (Action)OnElevationUp));
		AddBinding((IBinding)new TriggerBinding("tool", "elevationDown", (Action)OnElevationDown));
		AddBinding((IBinding)new TriggerBinding("tool", "elevationScroll", (Action)OnElevationScroll));
		AddBinding((IBinding)(object)new TriggerBinding<float>("tool", "setElevationStep", (Action<float>)SetElevationStep, (IReader<float>)null));
		AddBinding((IBinding)new TriggerBinding("tool", "toggleParallelMode", (Action)ToggleParallelMode));
		AddBinding((IBinding)(object)new TriggerBinding<float>("tool", "setParallelOffset", (Action<float>)SetParallelOffset, (IReader<float>)null));
		AddBinding((IBinding)(object)new TriggerBinding<bool>("tool", "setUndergroundMode", (Action<bool>)SetUndergroundMode, (IReader<bool>)null));
		AddBinding((IBinding)(object)new TriggerBinding<float>("tool", "setDistance", (Action<float>)SetDistance, (IReader<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("tool", "allowBrush", (Func<bool>)AllowBrush, (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<Entity>("tool", "selectedBrush", (Func<Entity>)(() => (!AllowBrush() || !m_ToolSystem.activeTool.brushing) ? Entity.Null : m_PrefabSystem.GetEntity(m_ToolSystem.activeTool.brushType)), (IWriter<Entity>)null, (EqualityComparer<Entity>)null));
		AddBinding((IBinding)(object)new GetterValueBinding<Brush[]>("tool", "brushes", (Func<Brush[]>)BindBrushTypes, (IWriter<Brush[]>)(object)new ArrayWriter<Brush>((IWriter<Brush>)(object)new ValueWriter<Brush>(), false), (EqualityComparer<Brush[]>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("tool", "brushSize", (Func<float>)(() => (!AllowBrush()) ? 0f : m_ToolSystem.activeTool.brushSize), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float?>("tool", "brushHeight", (Func<float?>)(() => (m_ToolSystem.activeTool != m_TerrainToolSystem || m_TerrainToolSystem.prefab.m_Type != TerraformingType.Level) ? ((float?)null) : new float?(m_TerrainToolSystem.brushHeight - WaterSystem.SeaLevel)), (IWriter<float?>)(object)ValueWritersStruct.Nullable<float>(ValueWriters.Create<float>()), (EqualityComparer<float?>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("tool", "brushStrength", (Func<float>)(() => (!AllowBrush()) ? 0f : m_ToolSystem.activeTool.brushStrength), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("tool", "brushAngle", (Func<float>)(() => (!AllowBrush()) ? 0f : m_ToolSystem.activeTool.brushAngle), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("tool", "brushSizeMin", (Func<float>)(() => 10f), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("tool", "brushSizeMax", (Func<float>)(() => (!m_ToolSystem.actionMode.IsEditor()) ? 1000f : 5000f), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("tool", "brushHeightMin", (Func<float>)(() => 0f - m_TerrainSystem.heightScaleOffset.y - WaterSystem.SeaLevel), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("tool", "brushHeightMax", (Func<float>)(() => m_TerrainSystem.heightScaleOffset.x - m_TerrainSystem.heightScaleOffset.y - WaterSystem.SeaLevel), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity>("tool", "selectBrush", (Action<Entity>)SelectBrush, (IReader<Entity>)null));
		AddBinding((IBinding)(object)new TriggerBinding<float>("tool", "setBrushHeight", (Action<float>)SetBrushHeight, (IReader<float>)null));
		AddBinding((IBinding)(object)new TriggerBinding<float>("tool", "setBrushSize", (Action<float>)SetBrushSize, (IReader<float>)null));
		AddBinding((IBinding)(object)new TriggerBinding<float>("tool", "setBrushStrength", (Action<float>)SetBrushStrength, (IReader<float>)null));
		AddBinding((IBinding)(object)new TriggerBinding<float>("tool", "setBrushAngle", (Action<float>)SetBrushAngle, (IReader<float>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Color32>("tool", "setColor", (Action<Color32>)SetColor, (IReader<Color32>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("tool", "isEditor", (Func<bool>)IsEditor, (IWriter<bool>)null, (EqualityComparer<bool>)null));
		m_ToolModes = new List<ToolMode>();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		ToolSystem toolSystem = m_ToolSystem;
		toolSystem.EventToolChanged = (Action<ToolBaseSystem>)Delegate.Remove(toolSystem.EventToolChanged, new Action<ToolBaseSystem>(OnToolChanged));
		ToolSystem toolSystem2 = m_ToolSystem;
		toolSystem2.EventPrefabChanged = (Action<PrefabBase>)Delegate.Remove(toolSystem2.EventPrefabChanged, new Action<PrefabBase>(OnPrefabChanged));
		BulldozeToolSystem bulldozeToolSystem = m_BulldozeToolSystem;
		bulldozeToolSystem.EventConfirmationRequested = (Action)Delegate.Remove(bulldozeToolSystem.EventConfirmationRequested, new Action(OnBulldozeConfirmationRequested));
		base.OnDestroy();
	}

	private void BindActiveTool(IJsonWriter binder)
	{
		binder.TypeBegin("tool.UITool");
		binder.PropertyName("id");
		binder.Write(m_ToolSystem.activeTool.toolID);
		binder.PropertyName("modeIndex");
		binder.Write(m_ToolSystem.activeTool.uiModeIndex);
		binder.PropertyName("modes");
		BindToolModes(binder);
		binder.TypeEnd();
	}

	private void BindToolModes(IJsonWriter binder)
	{
		m_ToolModes.Clear();
		m_ToolSystem.activeTool.GetUIModes(m_ToolModes);
		JsonWriterExtensions.ArrayBegin(binder, m_ToolModes.Count);
		for (int i = 0; i < m_ToolModes.Count; i++)
		{
			ToolMode toolMode = m_ToolModes[i];
			binder.TypeBegin("tool.ToolMode");
			binder.PropertyName("id");
			binder.Write(toolMode.name);
			binder.PropertyName("index");
			binder.Write(toolMode.index);
			binder.PropertyName("icon");
			binder.Write("Media/Tools/" + m_ToolSystem.activeTool.toolID + "/" + toolMode.name + ".svg");
			binder.TypeEnd();
		}
		binder.ArrayEnd();
	}

	private void SetSelectedSnapMask(uint mask)
	{
		m_ToolSystem.activeTool.selectedSnap = (Snap)mask;
	}

	private string[] InitSnapOptionNames()
	{
		uint[] obj = (uint[])Enum.GetValues(typeof(Snap));
		List<string> list = new List<string>();
		uint[] array = obj;
		foreach (uint num in array)
		{
			if (num != uint.MaxValue && num != 0)
			{
				list.Add(Enum.GetName(typeof(Snap), num));
			}
		}
		return list.ToArray();
	}

	private void SelectTool(string tool)
	{
		SelectTool(GetToolSystem(tool));
	}

	public void SelectTool(ToolBaseSystem tool)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.activeTool == tool)
		{
			return;
		}
		m_ToolSystem.activeTool = tool;
		if (tool == m_BulldozeToolSystem && !((EntityQuery)(ref m_BulldozeQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<PrefabData> val = ((EntityQuery)(ref m_BulldozeQuery)).ToComponentDataArray<PrefabData>(AllocatorHandle.op_Implicit((Allocator)3));
			try
			{
				m_BulldozeToolSystem.prefab = m_PrefabSystem.GetPrefab<BulldozePrefab>(val[0]);
			}
			finally
			{
				val.Dispose();
			}
		}
	}

	private void SelectToolMode(int modeIndex)
	{
		ToolBaseSystem activeTool = m_ToolSystem.activeTool;
		if (!(activeTool is NetToolSystem netToolSystem))
		{
			if (!(activeTool is ZoneToolSystem zoneToolSystem))
			{
				if (!(activeTool is BulldozeToolSystem bulldozeToolSystem))
				{
					if (!(activeTool is AreaToolSystem areaToolSystem))
					{
						if (activeTool is ObjectToolSystem objectToolSystem)
						{
							objectToolSystem.mode = (ObjectToolSystem.Mode)modeIndex;
						}
					}
					else
					{
						areaToolSystem.mode = (AreaToolSystem.Mode)modeIndex;
					}
				}
				else
				{
					bulldozeToolSystem.mode = (BulldozeToolSystem.Mode)modeIndex;
				}
			}
			else
			{
				zoneToolSystem.mode = (ZoneToolSystem.Mode)modeIndex;
			}
		}
		else
		{
			netToolSystem.mode = (NetToolSystem.Mode)modeIndex;
		}
		m_ActiveToolBinding.Update();
	}

	private ToolBaseSystem GetToolSystem(string tool)
	{
		return tool switch
		{
			"Net Tool" => m_NetToolSystem, 
			"Area Tool" => m_AreaToolSystem, 
			"Zone Tool" => m_ZoneToolSystem, 
			"Route Tool" => m_RouteToolSystem, 
			"Object Tool" => m_ObjectToolSystem, 
			"Terrain Tool" => m_TerrainToolSystem, 
			"Upgrade Tool" => m_UpgradeToolSystem, 
			"Bulldoze Tool" => m_BulldozeToolSystem, 
			"Selection Tool" => m_SelectionToolSystem, 
			_ => m_DefaultToolSystem, 
		};
	}

	private void OnToolChanged(ToolBaseSystem tool)
	{
		if (tool != m_TerrainToolSystem)
		{
			m_TerrainToolSystem.SetDisableFX();
		}
		m_ActiveToolBinding.Update();
	}

	private void OnPrefabChanged(PrefabBase prefab)
	{
		m_ActiveToolBinding.Update();
	}

	private void OnBulldozeConfirmationRequested()
	{
		GameManager.instance.userInterface.appBindings.ShowConfirmationDialog(new ConfirmationDialog(null, "Common.DIALOG_MESSAGE[Bulldozer]", "Common.DIALOG_ACTION[Yes]", "Common.DIALOG_ACTION[No]"), OnConfirmBulldoze);
	}

	private void OnConfirmBulldoze(int msg)
	{
		m_BulldozeToolSystem.ConfirmAction(msg == 0);
	}

	private bool GetElevationUpDisabled()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.activeTool == m_NetToolSystem)
		{
			Bounds1 elevationRange = GetElevationRange();
			if (elevationRange != default(Bounds1))
			{
				return elevationRange.max <= m_NetToolSystem.elevation;
			}
		}
		return !m_ToolSystem.activeTool.requireUnderground;
	}

	private void OnElevationUp()
	{
		if (m_ToolSystem.activeTool != null)
		{
			m_ToolSystem.activeTool.ElevationUp();
		}
	}

	private bool GetElevationDownDisabled()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.activeTool == m_NetToolSystem)
		{
			Bounds1 elevationRange = GetElevationRange();
			if (elevationRange != default(Bounds1))
			{
				return elevationRange.min >= m_NetToolSystem.elevation;
			}
		}
		if (!m_ToolSystem.activeTool.requireUnderground)
		{
			return !m_ToolSystem.activeTool.allowUnderground;
		}
		return true;
	}

	private void OnElevationDown()
	{
		if (m_ToolSystem.activeTool != null)
		{
			m_ToolSystem.activeTool.ElevationDown();
		}
	}

	private void OnElevationScroll()
	{
		if (m_ToolSystem.activeTool != null)
		{
			m_ToolSystem.activeTool.ElevationScroll();
		}
	}

	private Bounds1 GetElevationRange()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.activeTool == m_NetToolSystem && m_NetToolSystem.mode != NetToolSystem.Mode.Replace && (Object)(object)m_NetToolSystem.prefab != (Object)null && m_NetToolSystem.prefab.TryGet<PlaceableNet>(out var component))
		{
			if ((Object)(object)component.m_UndergroundPrefab != (Object)null && component.m_UndergroundPrefab.TryGet<PlaceableNet>(out var component2))
			{
				return component.m_ElevationRange | component2.m_ElevationRange;
			}
			return component.m_ElevationRange;
		}
		return default(Bounds1);
	}

	private void SetElevationStep(float step)
	{
		m_NetToolSystem.elevationStep = step;
	}

	private bool GetParallelModeSupported()
	{
		if (m_ToolSystem.activeTool == m_NetToolSystem && m_NetToolSystem.mode != NetToolSystem.Mode.Grid && m_NetToolSystem.mode != NetToolSystem.Mode.Replace)
		{
			if ((Object)(object)m_NetToolSystem.prefab != (Object)null && m_NetToolSystem.prefab.TryGet<PlaceableNet>(out var component) && component.m_AllowParallelMode)
			{
				return true;
			}
			if ((Object)(object)m_NetToolSystem.lane != (Object)null)
			{
				return true;
			}
		}
		return false;
	}

	private void ToggleParallelMode()
	{
		m_NetToolSystem.parallelCount = ((m_NetToolSystem.parallelCount == 0) ? 1 : 0);
	}

	private void SetParallelOffset(float offset)
	{
		m_NetToolSystem.parallelOffset = offset;
	}

	private void SetUndergroundMode(bool enabled)
	{
		if (m_ToolSystem.activeTool != null)
		{
			m_ToolSystem.activeTool.SetUnderground(enabled);
		}
	}

	private void SetDistance(float distance)
	{
		m_ObjectToolSystem.distance = distance;
	}

	private Brush[] BindBrushTypes()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		PrefabSystem orCreateSystemManaged = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		NativeArray<Entity> val = ((EntityQuery)(ref m_BrushQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		Brush[] array = new Brush[val.Length];
		for (int i = 0; i < val.Length; i++)
		{
			BrushPrefab prefab = orCreateSystemManaged.GetPrefab<BrushPrefab>(val[i]);
			array[i] = new Brush
			{
				m_Entity = val[i],
				m_Name = ((Object)prefab).name,
				m_Icon = string.Empty,
				m_Priority = prefab.m_Priority
			};
		}
		val.Dispose();
		return array;
	}

	private void SelectBrush(Entity entity)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!AllowBrush())
		{
			return;
		}
		if (entity != Entity.Null)
		{
			BrushPrefab prefab = m_PrefabSystem.GetPrefab<BrushPrefab>(entity);
			m_ToolSystem.activeTool.brushType = prefab;
			if (m_ToolSystem.activeTool == m_ObjectToolSystem)
			{
				m_ObjectToolSystem.mode = ObjectToolSystem.Mode.Brush;
			}
		}
		else if (m_ToolSystem.activeTool == m_ObjectToolSystem && m_ObjectToolSystem.mode == ObjectToolSystem.Mode.Brush)
		{
			m_ObjectToolSystem.mode = ObjectToolSystem.Mode.Create;
		}
	}

	private bool AllowBrush()
	{
		if (m_ToolSystem.activeTool == m_ObjectToolSystem)
		{
			return m_ObjectToolSystem.allowBrush;
		}
		if (m_ToolSystem.activeTool == m_TerrainToolSystem)
		{
			return true;
		}
		return false;
	}

	private void SetBrushSize(float size)
	{
		m_ToolSystem.activeTool.brushSize = size;
	}

	private void SetBrushHeight(float height)
	{
		m_TerrainToolSystem.brushHeight = height + WaterSystem.SeaLevel;
	}

	private void SetBrushStrength(float strength)
	{
		m_ToolSystem.activeTool.brushStrength = strength;
	}

	private void SetBrushAngle(float angle)
	{
		m_ToolSystem.activeTool.brushAngle = angle;
	}

	private void SetColor(Color32 color)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		m_ToolSystem.activeTool.color = color;
	}

	private bool GetColorSupported()
	{
		return m_ToolSystem.activePrefab is IColored;
	}

	private bool IsEditor()
	{
		return GameManager.instance.gameMode.IsEditor();
	}

	[Preserve]
	public ToolUISystem()
	{
	}
}
