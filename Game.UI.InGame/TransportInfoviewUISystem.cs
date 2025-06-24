using System;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Common;
using Game.Prefabs;
using Game.Routes;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

public class TransportInfoviewUISystem : InfoviewUISystemBase
{
	public readonly struct PassengerSummary
	{
		private readonly Entity m_Prefab;

		public string id { get; }

		public string icon { get; }

		public bool locked { get; }

		public int lineCount { get; }

		public int touristCount { get; }

		public int citizenCount { get; }

		public PassengerSummary(Entity prefab, string id, string icon, bool locked, int lineCount, int touristCount, int citizenCount)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Prefab = prefab;
			this.id = id;
			this.icon = icon;
			this.locked = locked;
			this.lineCount = lineCount;
			this.touristCount = touristCount;
			this.citizenCount = citizenCount;
		}

		public void Write(PrefabUISystem prefabUISystem, IJsonWriter writer)
		{
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("id");
			writer.Write(id);
			writer.PropertyName("icon");
			writer.Write(icon);
			writer.PropertyName("locked");
			writer.Write(locked);
			writer.PropertyName("lineCount");
			writer.Write(lineCount);
			writer.PropertyName("touristCount");
			writer.Write(touristCount);
			writer.PropertyName("citizenCount");
			writer.Write(citizenCount);
			writer.PropertyName("requirements");
			prefabUISystem.BindPrefabRequirements(writer, m_Prefab);
			writer.TypeEnd();
		}
	}

	public readonly struct CargoSummary
	{
		private readonly Entity m_Prefab;

		public string id { get; }

		public string icon { get; }

		public bool locked { get; }

		public int lineCount { get; }

		public int cargoCount { get; }

		public CargoSummary(Entity prefab, string id, string icon, bool locked, int lineCount, int cargoCount)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Prefab = prefab;
			this.id = id;
			this.icon = icon;
			this.locked = locked;
			this.lineCount = lineCount;
			this.cargoCount = cargoCount;
		}

		public void Write(PrefabUISystem prefabUISystem, IJsonWriter writer)
		{
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("id");
			writer.Write(id);
			writer.PropertyName("icon");
			writer.Write(icon);
			writer.PropertyName("locked");
			writer.Write(locked);
			writer.PropertyName("lineCount");
			writer.Write(lineCount);
			writer.PropertyName("cargoCount");
			writer.Write(cargoCount);
			writer.PropertyName("requirements");
			prefabUISystem.BindPrefabRequirements(writer, m_Prefab);
			writer.TypeEnd();
		}
	}

	private const string kGroup = "transportInfo";

	private UnlockSystem m_UnlockSystem;

	private PrefabSystem m_PrefabSystem;

	private PrefabUISystem m_PrefabUISystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private EntityQuery m_ConfigQuery;

	private EntityQuery m_LineQuery;

	private EntityQuery m_ModifiedLineQuery;

	private RawValueBinding m_Summaries;

	private UITransportConfigurationPrefab m_Config;

	protected override bool Active
	{
		get
		{
			if (!base.Active)
			{
				return ((EventBindingBase)m_Summaries).active;
			}
			return true;
		}
	}

	protected override bool Modified => !((EntityQuery)(ref m_ModifiedLineQuery)).IsEmptyIgnoreFilter;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Expected O, but got Unknown
		//IL_01a8: Expected O, but got Unknown
		base.OnCreate();
		m_UnlockSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UnlockSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_PrefabUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabUISystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_ConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<UITransportConfigurationData>() });
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Route>(),
			ComponentType.ReadWrite<TransportLine>(),
			ComponentType.ReadOnly<RouteWaypoint>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_LineQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Route>(),
			ComponentType.ReadWrite<TransportLine>(),
			ComponentType.ReadOnly<RouteWaypoint>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array2[0] = val;
		m_ModifiedLineQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		RawValueBinding val2 = new RawValueBinding("transportInfo", "summaries", (Action<IJsonWriter>)BindSummaries);
		RawValueBinding binding = val2;
		m_Summaries = val2;
		AddBinding((IBinding)(object)binding);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentSystemBase)this).Enabled)
		{
			m_Config = m_PrefabSystem.GetSingletonPrefab<UITransportConfigurationPrefab>(m_ConfigQuery);
		}
	}

	protected override void PerformUpdate()
	{
		m_Summaries.Update();
	}

	private void BindSummaries(IJsonWriter writer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<UITransportLineData> sortedLines = TransportUIUtils.GetSortedLines(m_LineQuery, ((ComponentSystemBase)this).EntityManager, m_PrefabSystem);
		writer.TypeBegin(((object)this).GetType().FullName + "+TransportSummaries");
		writer.PropertyName("passengerSummaries");
		JsonWriterExtensions.ArrayBegin(writer, m_Config.m_PassengerSummaryItems.Length);
		UITransportSummaryItem[] passengerSummaryItems = m_Config.m_PassengerSummaryItems;
		foreach (UITransportSummaryItem uITransportSummaryItem in passengerSummaryItems)
		{
			new PassengerSummary(m_PrefabSystem.GetEntity(uITransportSummaryItem.m_Unlockable), Enum.GetName(typeof(TransportType), uITransportSummaryItem.m_Type), uITransportSummaryItem.m_Icon, m_UnlockSystem.IsLocked(uITransportSummaryItem.m_Unlockable), uITransportSummaryItem.m_ShowLines ? TransportUIUtils.CountLines(sortedLines, uITransportSummaryItem.m_Type) : 0, m_CityStatisticsSystem.GetStatisticValue(uITransportSummaryItem.m_Statistic, 1), m_CityStatisticsSystem.GetStatisticValue(uITransportSummaryItem.m_Statistic)).Write(m_PrefabUISystem, writer);
		}
		writer.ArrayEnd();
		writer.PropertyName("cargoSummaries");
		JsonWriterExtensions.ArrayBegin(writer, m_Config.m_CargoSummaryItems.Length);
		passengerSummaryItems = m_Config.m_CargoSummaryItems;
		foreach (UITransportSummaryItem uITransportSummaryItem2 in passengerSummaryItems)
		{
			new CargoSummary(m_PrefabSystem.GetEntity(uITransportSummaryItem2.m_Unlockable), Enum.GetName(typeof(TransportType), uITransportSummaryItem2.m_Type), uITransportSummaryItem2.m_Icon, m_UnlockSystem.IsLocked(uITransportSummaryItem2.m_Unlockable), uITransportSummaryItem2.m_ShowLines ? TransportUIUtils.CountLines(sortedLines, uITransportSummaryItem2.m_Type, cargo: true) : 0, m_CityStatisticsSystem.GetStatisticValue(uITransportSummaryItem2.m_Statistic)).Write(m_PrefabUISystem, writer);
		}
		writer.ArrayEnd();
		writer.TypeEnd();
	}

	[Preserve]
	public TransportInfoviewUISystem()
	{
	}
}
