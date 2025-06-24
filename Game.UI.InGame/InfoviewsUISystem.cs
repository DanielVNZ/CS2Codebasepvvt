using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Annotations;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Prefabs;
using Game.Tools;
using Game.UI.Localization;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class InfoviewsUISystem : UISystemBase
{
	public readonly struct Infoview : IComparable<Infoview>
	{
		public Entity entity { get; }

		[NotNull]
		public string id { get; }

		[NotNull]
		public string icon { get; }

		public bool locked { get; }

		[NotNull]
		public string uiTag { get; }

		public int group { get; }

		private int priority { get; }

		private bool editor { get; }

		public Infoview(Entity entity, InfoviewPrefab prefab, bool locked)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			this.entity = entity;
			id = ((Object)prefab).name;
			icon = prefab.m_IconPath;
			this.locked = locked;
			uiTag = prefab.uiTag;
			priority = prefab.m_Priority;
			group = prefab.m_Group;
			editor = prefab.m_Editor;
		}

		public int CompareTo(Infoview other)
		{
			int num = group - other.group;
			int num2 = ((num != 0) ? num : (priority - other.priority));
			if (num2 == 0)
			{
				return string.Compare(id, other.id, StringComparison.Ordinal);
			}
			return num2;
		}

		public void Write(PrefabUISystem prefabUISystem, IJsonWriter writer)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			writer.TypeBegin("infoviews.Infoview");
			writer.PropertyName("entity");
			UnityWriters.Write(writer, entity);
			writer.PropertyName("id");
			writer.Write(id);
			writer.PropertyName("icon");
			writer.Write(icon);
			writer.PropertyName("locked");
			writer.Write(locked);
			writer.PropertyName("uiTag");
			writer.Write(uiTag);
			writer.PropertyName("group");
			writer.Write(group);
			writer.PropertyName("editor");
			writer.Write(editor);
			writer.PropertyName("requirements");
			prefabUISystem.BindPrefabRequirements(writer, entity);
			writer.TypeEnd();
		}
	}

	private const string kGroup = "infoviews";

	private ToolSystem m_ToolSystem;

	private PrefabSystem m_PrefabSystem;

	private UnlockSystem m_UnlockSystem;

	private PrefabUISystem m_PrefabUISystem;

	private InfoviewInitializeSystem m_InfoviewInitializeSystem;

	private RawValueBinding m_ActiveView;

	private List<Infoview> m_InfoviewsCache;

	private RawValueBinding m_Infoviews;

	private EntityQuery m_UnlockedInfoviewQuery;

	private bool m_InfoviewChanged;

	public override GameMode gameMode => GameMode.GameOrEditor;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_00ee: Expected O, but got Unknown
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_0118: Expected O, but got Unknown
		base.OnCreate();
		m_UnlockedInfoviewQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Unlock>() });
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_UnlockSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UnlockSystem>();
		m_PrefabUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabUISystem>();
		m_InfoviewInitializeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<InfoviewInitializeSystem>();
		m_InfoviewsCache = new List<Infoview>();
		AddBinding((IBinding)(object)new TriggerBinding<Entity>("infoviews", "setActiveInfoview", (Action<Entity>)SetActiveInfoview, (IReader<Entity>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity, bool, int>("infoviews", "setInfomodeActive", (Action<Entity, bool, int>)SetInfomodeActive, (IReader<Entity>)null, (IReader<bool>)null, (IReader<int>)null));
		RawValueBinding val = new RawValueBinding("infoviews", "infoviews", (Action<IJsonWriter>)BindInfoviews);
		RawValueBinding binding = val;
		m_Infoviews = val;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val2 = new RawValueBinding("infoviews", "activeInfoview", (Action<IJsonWriter>)BindActiveInfoview);
		binding = val2;
		m_ActiveView = val2;
		AddBinding((IBinding)(object)binding);
		ToolSystem toolSystem = m_ToolSystem;
		toolSystem.EventInfoviewChanged = (Action<InfoviewPrefab>)Delegate.Combine(toolSystem.EventInfoviewChanged, new Action<InfoviewPrefab>(OnInfoviewChanged));
		ToolSystem toolSystem2 = m_ToolSystem;
		toolSystem2.EventInfomodesChanged = (Action)Delegate.Combine(toolSystem2.EventInfomodesChanged, new Action(OnChanged));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		ToolSystem toolSystem = m_ToolSystem;
		toolSystem.EventInfoviewChanged = (Action<InfoviewPrefab>)Delegate.Remove(toolSystem.EventInfoviewChanged, new Action<InfoviewPrefab>(OnInfoviewChanged));
		ToolSystem toolSystem2 = m_ToolSystem;
		toolSystem2.EventInfomodesChanged = (Action)Delegate.Remove(toolSystem2.EventInfomodesChanged, new Action(OnChanged));
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (PrefabUtils.HasUnlockedPrefab<InfoviewData>(((ComponentSystemBase)this).EntityManager, m_UnlockedInfoviewQuery))
		{
			m_Infoviews.Update();
		}
		if (m_InfoviewChanged)
		{
			m_ActiveView.Update();
			m_InfoviewChanged = false;
		}
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		m_Infoviews.Update();
		m_ActiveView.Update();
	}

	private void OnInfoviewChanged(InfoviewPrefab prefab)
	{
		m_InfoviewChanged = true;
	}

	private void OnChanged()
	{
		m_InfoviewChanged = true;
	}

	private void BindInfoviews(IJsonWriter writer)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		m_InfoviewsCache.Clear();
		foreach (InfoviewPrefab infoview in m_InfoviewInitializeSystem.infoviews)
		{
			if (infoview.isValid)
			{
				bool locked = m_UnlockSystem.IsLocked(infoview);
				m_InfoviewsCache.Add(new Infoview(m_PrefabSystem.GetEntity(infoview), infoview, locked));
			}
		}
		m_InfoviewsCache.Sort();
		JsonWriterExtensions.ArrayBegin(writer, m_InfoviewsCache.Count);
		foreach (Infoview item in m_InfoviewsCache)
		{
			item.Write(m_PrefabUISystem, writer);
		}
		writer.ArrayEnd();
	}

	private void BindActiveInfoview(IJsonWriter writer)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		InfoviewPrefab activeInfoview = m_ToolSystem.activeInfoview;
		if ((Object)(object)activeInfoview != (Object)null)
		{
			Entity entity = m_PrefabSystem.GetEntity(activeInfoview);
			writer.TypeBegin("infoviews.ActiveInfoview");
			writer.PropertyName("entity");
			UnityWriters.Write(writer, entity);
			writer.PropertyName("id");
			writer.Write(((Object)activeInfoview).name);
			writer.PropertyName("icon");
			writer.Write(activeInfoview.m_IconPath);
			writer.PropertyName("uiTag");
			writer.Write(activeInfoview.uiTag);
			List<InfomodeInfo> infoviewInfomodes = m_ToolSystem.GetInfoviewInfomodes();
			writer.PropertyName("infomodes");
			JsonWriterExtensions.ArrayBegin(writer, infoviewInfomodes.Count);
			for (int i = 0; i < infoviewInfomodes.Count; i++)
			{
				BindInfomode(writer, infoviewInfomodes[i]);
			}
			writer.ArrayEnd();
			writer.PropertyName("editor");
			writer.Write(activeInfoview.m_Editor);
			writer.TypeEnd();
		}
		else
		{
			writer.WriteNull();
		}
	}

	private void BindInfomode(IJsonWriter writer, InfomodeInfo info)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		Entity entity = m_PrefabSystem.GetEntity(info.m_Mode);
		IColorInfomode colorInfomode = info.m_Mode as IColorInfomode;
		IGradientInfomode gradientInfomode = info.m_Mode as IGradientInfomode;
		writer.TypeBegin("infoviews.Infomode");
		writer.PropertyName("entity");
		UnityWriters.Write(writer, entity);
		writer.PropertyName("id");
		writer.Write(((Object)info.m_Mode).name);
		writer.PropertyName("uiTag");
		writer.Write(info.m_Mode.uiTag);
		writer.PropertyName("active");
		writer.Write(m_ToolSystem.IsInfomodeActive(info.m_Mode));
		writer.PropertyName("priority");
		writer.Write(info.m_Priority);
		writer.PropertyName("color");
		if (colorInfomode != null)
		{
			UnityWriters.Write(writer, colorInfomode.color);
		}
		else if (gradientInfomode != null && gradientInfomode.legendType == GradientLegendType.Fields && !gradientInfomode.lowLabel.HasValue)
		{
			UnityWriters.Write(writer, gradientInfomode.lowColor);
		}
		else
		{
			writer.WriteNull();
		}
		writer.PropertyName("gradientLegend");
		if (gradientInfomode != null && gradientInfomode.legendType == GradientLegendType.Gradient)
		{
			BindInfomodeGradientLegend(writer, gradientInfomode);
		}
		else
		{
			writer.WriteNull();
		}
		writer.PropertyName("colorLegends");
		if (gradientInfomode != null && gradientInfomode.legendType == GradientLegendType.Fields)
		{
			BindColorLegends(writer, gradientInfomode);
		}
		else
		{
			JsonWriterExtensions.WriteEmptyArray(writer);
		}
		writer.PropertyName("type");
		writer.Write(info.m_Mode.infomodeTypeLocaleKey);
		writer.TypeEnd();
	}

	private void BindInfomodeGradientLegend(IJsonWriter writer, IGradientInfomode gradientInfomode)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		writer.TypeBegin("infoviews.InfomodeGradientLegend");
		writer.PropertyName("lowLabel");
		JsonWriterExtensions.Write<LocalizedString>(writer, gradientInfomode.lowLabel);
		writer.PropertyName("highLabel");
		JsonWriterExtensions.Write<LocalizedString>(writer, gradientInfomode.highLabel);
		writer.PropertyName("gradient");
		writer.TypeBegin("infoviews.Gradient");
		writer.PropertyName("stops");
		writer.ArrayBegin(3u);
		BindGradientStop(writer, 0f, gradientInfomode.lowColor);
		BindGradientStop(writer, 0.5f, gradientInfomode.mediumColor);
		BindGradientStop(writer, 1f, gradientInfomode.highColor);
		writer.ArrayEnd();
		writer.TypeEnd();
		writer.TypeEnd();
	}

	private void BindGradientStop(IJsonWriter writer, float offset, Color color)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		writer.TypeBegin("infoviews.GradientStop");
		writer.PropertyName("offset");
		writer.Write(offset);
		writer.PropertyName("color");
		UnityWriters.Write(writer, color);
		writer.TypeEnd();
	}

	private void BindColorLegends(IJsonWriter writer, IGradientInfomode gradientInfomode)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		uint num = 0u;
		if (gradientInfomode.lowLabel.HasValue)
		{
			num++;
		}
		if (gradientInfomode.mediumLabel.HasValue)
		{
			num++;
		}
		if (gradientInfomode.highLabel.HasValue)
		{
			num++;
		}
		writer.ArrayBegin(num);
		if (gradientInfomode.lowLabel.HasValue)
		{
			BindColorLegend(writer, gradientInfomode.lowColor, gradientInfomode.lowLabel.Value);
		}
		if (gradientInfomode.mediumLabel.HasValue)
		{
			BindColorLegend(writer, gradientInfomode.mediumColor, gradientInfomode.mediumLabel.Value);
		}
		if (gradientInfomode.highLabel.HasValue)
		{
			BindColorLegend(writer, gradientInfomode.highColor, gradientInfomode.highLabel.Value);
		}
		writer.ArrayEnd();
	}

	private void BindColorLegend(IJsonWriter writer, Color color, LocalizedString label)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		writer.TypeBegin("infoviews.ColorLegend");
		writer.PropertyName("color");
		UnityWriters.Write(writer, color);
		writer.PropertyName("label");
		JsonWriterExtensions.Write<LocalizedString>(writer, label);
		writer.TypeEnd();
	}

	public void SetActiveInfoview(Entity entity)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		InfoviewPrefab infoview = null;
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).Exists(entity))
		{
			infoview = m_PrefabSystem.GetPrefab<InfoviewPrefab>(entity);
		}
		m_ToolSystem.infoview = infoview;
	}

	private void SetInfomodeActive(Entity entity, bool active, int priority)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_ToolSystem.SetInfomodeActive(entity, active, priority);
	}

	[Preserve]
	public InfoviewsUISystem()
	{
	}
}
