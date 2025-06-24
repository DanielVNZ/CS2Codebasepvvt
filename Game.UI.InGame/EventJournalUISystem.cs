using System;
using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Common;
using Game.Events;
using Game.Prefabs;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class EventJournalUISystem : UISystemBase
{
	private const string kGroup = "eventJournal";

	private const int kMaxMessages = 100;

	private IEventJournalSystem m_EventJournalSystem;

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_TimeDataQuery;

	private RawMapBinding<Entity> m_EventMap;

	private RawValueBinding m_Events;

	public Action eventJournalOpened { get; set; }

	public Action eventJournalClosed { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_00fa: Expected O, but got Unknown
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_EventJournalSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EventJournalSystem>();
		IEventJournalSystem eventJournalSystem = m_EventJournalSystem;
		eventJournalSystem.eventEventDataChanged = (Action<Entity>)Delegate.Combine(eventJournalSystem.eventEventDataChanged, new Action<Entity>(OnEventDataChanged));
		IEventJournalSystem eventJournalSystem2 = m_EventJournalSystem;
		eventJournalSystem2.eventEntryAdded = (Action)Delegate.Combine(eventJournalSystem2.eventEntryAdded, new Action(OnEntryAdded));
		m_TimeDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeData>() });
		AddBinding((IBinding)new TriggerBinding("eventJournal", "openJournal", (Action)delegate
		{
			eventJournalOpened?.Invoke();
		}));
		AddBinding((IBinding)new TriggerBinding("eventJournal", "closeJournal", (Action)delegate
		{
			eventJournalClosed?.Invoke();
		}));
		RawValueBinding val = new RawValueBinding("eventJournal", "events", (Action<IJsonWriter>)BindEvents);
		RawValueBinding binding = val;
		m_Events = val;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)(m_EventMap = new RawMapBinding<Entity>("eventJournal", "eventMap", (Action<IJsonWriter, Entity>)delegate(IJsonWriter binder, Entity entity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			BindJournalEntry(entity, binder);
		}, (IReader<Entity>)null, (IWriter<Entity>)null)));
	}

	[Preserve]
	protected override void OnUpdate()
	{
	}

	private void OnEventDataChanged(Entity entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((MapBindingBase<Entity>)(object)m_EventMap).Update(entity);
	}

	private void OnEntryAdded()
	{
		m_Events.Update();
	}

	private void BindEvents(IJsonWriter binder)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		JsonWriterExtensions.ArrayBegin(binder, Math.Min(m_EventJournalSystem.eventJournal.Length, 100));
		int num = m_EventJournalSystem.eventJournal.Length - 1;
		while (num >= 0 && num >= m_EventJournalSystem.eventJournal.Length - 100)
		{
			UnityWriters.Write(binder, m_EventJournalSystem.eventJournal[num]);
			num--;
		}
		binder.ArrayEnd();
	}

	private void BindJournalEntry(Entity entity, IJsonWriter binder)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		EventJournalEntry info = m_EventJournalSystem.GetInfo(entity);
		Entity prefab = m_EventJournalSystem.GetPrefab(entity);
		EventPrefab prefab2 = m_PrefabSystem.GetPrefab<EventPrefab>(prefab);
		JournalEventComponent component = prefab2.GetComponent<JournalEventComponent>();
		binder.TypeBegin("eventJournal.EventInfo");
		binder.PropertyName("id");
		binder.Write(((Object)prefab2).name);
		binder.PropertyName("icon");
		binder.Write(component.m_Icon);
		binder.PropertyName("date");
		binder.Write(info.m_StartFrame - TimeData.GetSingleton(m_TimeDataQuery).m_FirstFrame);
		binder.PropertyName("data");
		if (m_EventJournalSystem.TryGetData(entity, out var data))
		{
			JsonWriterExtensions.ArrayBegin(binder, data.Length);
			for (int i = 0; i < data.Length; i++)
			{
				binder.TypeBegin("eventJournal.UIEventData");
				binder.PropertyName("type");
				binder.Write(Enum.GetName(typeof(EventDataTrackingType), data[i].m_Type));
				binder.PropertyName("value");
				binder.Write(data[i].m_Value);
				binder.TypeEnd();
			}
			binder.ArrayEnd();
		}
		else
		{
			binder.WriteNull();
		}
		binder.PropertyName("effects");
		if (m_EventJournalSystem.TryGetCityEffects(entity, out var data2))
		{
			JsonWriterExtensions.ArrayBegin(binder, data2.Length);
			for (int j = 0; j < data2.Length; j++)
			{
				binder.TypeBegin("eventJournal.UIEventData");
				binder.PropertyName("type");
				binder.Write(Enum.GetName(typeof(EventCityEffectTrackingType), data2[j].m_Type));
				binder.PropertyName("value");
				binder.Write(EventJournalUtils.GetPercentileChange(data2[j]));
				binder.TypeEnd();
			}
			binder.ArrayEnd();
		}
		else
		{
			binder.WriteNull();
		}
		binder.TypeEnd();
	}

	[Preserve]
	public EventJournalUISystem()
	{
	}
}
