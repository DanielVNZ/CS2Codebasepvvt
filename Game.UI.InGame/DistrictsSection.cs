using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Areas;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class DistrictsSection : InfoSectionBase
{
	private ToolSystem m_ToolSystem;

	private AreaToolSystem m_AreaToolSystem;

	private DefaultToolSystem m_DefaultToolSystem;

	private SelectionToolSystem m_SelectionToolSystem;

	private EntityQuery m_ConfigQuery;

	private EntityQuery m_DistrictQuery;

	private EntityQuery m_DistrictPrefabQuery;

	private EntityQuery m_DistrictModifiedQuery;

	private ValueBinding<bool> m_Selecting;

	protected override string group => "DistrictsSection";

	private NativeList<Entity> districts { get; set; }

	private bool districtMissing { get; set; }

	protected override void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		districts.Clear();
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Expected O, but got Unknown
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Expected O, but got Unknown
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_AreaToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AreaToolSystem>();
		m_DefaultToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DefaultToolSystem>();
		m_SelectionToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SelectionToolSystem>();
		ToolSystem toolSystem = m_ToolSystem;
		toolSystem.EventToolChanged = (Action<ToolBaseSystem>)Delegate.Combine(toolSystem.EventToolChanged, new Action<ToolBaseSystem>(OnToolChanged));
		districts = new NativeList<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
		m_ConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AreasConfigurationData>() });
		m_DistrictQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<District>(),
			ComponentType.Exclude<Temp>()
		});
		m_DistrictPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<DistrictData>(),
			ComponentType.Exclude<Locked>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<District>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		m_DistrictModifiedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		AddBinding((IBinding)(object)new TriggerBinding<Entity>(group, "removeDistrict", (Action<Entity>)RemoveServiceDistrict, (IReader<Entity>)null));
		AddBinding((IBinding)new TriggerBinding(group, "toggleSelectionTool", (Action)ToggleSelectionTool));
		AddBinding((IBinding)new TriggerBinding(group, "toggleDistrictTool", (Action)ToggleDistrictTool));
		AddBinding((IBinding)(object)(m_Selecting = new ValueBinding<bool>(group, "selecting", false, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
	}

	private void OnToolChanged(ToolBaseSystem tool)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		bool flag = tool == m_SelectionToolSystem && m_SelectionToolSystem.selectionType == SelectionType.ServiceDistrict;
		if (m_Selecting.value && !flag)
		{
			m_SelectionToolSystem.selectionOwner = Entity.Null;
			m_SelectionToolSystem.selectionType = SelectionType.None;
		}
		m_Selecting.Update(flag);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		districts.Dispose();
		base.OnDestroy();
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<ServiceDistrict>(selectedEntity))
		{
			return !((EntityQuery)(ref m_DistrictPrefabQuery)).IsEmpty;
		}
		return false;
	}

	protected override void OnPreUpdate()
	{
		base.OnPreUpdate();
		if (!((EntityQuery)(ref m_DistrictModifiedQuery)).IsEmptyIgnoreFilter)
		{
			RequestUpdate();
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
		districtMissing = ((EntityQuery)(ref m_DistrictQuery)).IsEmptyIgnoreFilter;
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<ServiceDistrict> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ServiceDistrict>(selectedEntity, true);
		for (int i = 0; i < buffer.Length; i++)
		{
			NativeList<Entity> val = districts;
			ServiceDistrict serviceDistrict = buffer[i];
			val.Add(ref serviceDistrict.m_District);
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("districtMissing");
		writer.Write(districtMissing);
		writer.PropertyName("districts");
		JsonWriterExtensions.ArrayBegin(writer, districts.Length);
		for (int i = 0; i < districts.Length; i++)
		{
			Entity val = districts[i];
			writer.TypeBegin("selectedInfo.District");
			writer.PropertyName("name");
			m_NameSystem.BindName(writer, val);
			writer.PropertyName("entity");
			UnityWriters.Write(writer, val);
			writer.TypeEnd();
		}
		writer.ArrayEnd();
	}

	public void RemoveServiceDistrict(Entity district)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<ServiceDistrict> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ServiceDistrict>(selectedEntity, false);
		bool flag = false;
		for (int i = 0; i < buffer.Length; i++)
		{
			if (buffer[i].m_District == district)
			{
				buffer.RemoveAt(i);
				flag = true;
			}
		}
		if (flag)
		{
			m_InfoUISystem.RequestUpdate();
		}
	}

	private void ToggleSelectionTool()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.activeTool == m_SelectionToolSystem)
		{
			m_ToolSystem.activeTool = m_DefaultToolSystem;
			return;
		}
		m_SelectionToolSystem.selectionType = SelectionType.ServiceDistrict;
		m_SelectionToolSystem.selectionOwner = selectedEntity;
		m_ToolSystem.activeTool = m_SelectionToolSystem;
	}

	private void ToggleDistrictTool()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.activeTool == m_AreaToolSystem)
		{
			m_ToolSystem.activeTool = m_DefaultToolSystem;
			return;
		}
		AreasConfigurationPrefab prefab = m_PrefabSystem.GetPrefab<AreasConfigurationPrefab>(((EntityQuery)(ref m_ConfigQuery)).GetSingletonEntity());
		m_AreaToolSystem.prefab = prefab.m_DefaultDistrictPrefab;
		m_ToolSystem.activeTool = m_AreaToolSystem;
	}

	[Preserve]
	public DistrictsSection()
	{
	}
}
