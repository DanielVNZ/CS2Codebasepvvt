using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Prefabs;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class StatisticsUISystem : UISystemBase
{
	public struct StatCategory : IComparable<StatCategory>
	{
		public Entity m_Entity;

		public PrefabData m_PrefabData;

		public UIObjectData m_ObjectData;

		public StatCategory(Entity entity, UIObjectData objectData, PrefabData prefabData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Entity = entity;
			m_PrefabData = prefabData;
			m_ObjectData = objectData;
		}

		public int CompareTo(StatCategory other)
		{
			return m_ObjectData.m_Priority.CompareTo(other.m_ObjectData.m_Priority);
		}
	}

	public struct DataPoint : IJsonWritable
	{
		public long x;

		public long y;

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("x");
			writer.Write((float)x);
			writer.PropertyName("y");
			writer.Write((float)y);
			writer.TypeEnd();
		}
	}

	public struct StatItem : IJsonReadable, IJsonWritable, IComparable<StatItem>
	{
		public Entity category;

		public Entity group;

		public Entity entity;

		public int statisticType;

		public int unitType;

		public int parameterIndex;

		public string key;

		public Color color;

		public bool locked;

		public bool isGroup;

		public bool isSubgroup;

		public bool stacked;

		public int priority;

		public StatItem(int priority, Entity category, Entity group, Entity entity, int statisticType, StatisticUnitType unitType, int parameterIndex, string key, Color color, bool locked, bool isGroup = false, bool isSubgroup = false, bool stacked = true)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			this.category = category;
			this.group = group;
			this.entity = entity;
			this.statisticType = statisticType;
			this.unitType = (int)unitType;
			this.parameterIndex = parameterIndex;
			this.key = key;
			this.color = color;
			this.locked = locked;
			this.isGroup = isGroup;
			this.isSubgroup = isSubgroup;
			this.stacked = stacked;
			this.priority = priority;
		}

		public void Read(IJsonReader reader)
		{
			reader.ReadMapBegin();
			reader.ReadProperty("category");
			UnityReaders.Read(reader, ref category);
			reader.ReadProperty("group");
			UnityReaders.Read(reader, ref group);
			reader.ReadProperty("entity");
			UnityReaders.Read(reader, ref entity);
			reader.ReadProperty("statisticType");
			reader.Read(ref statisticType);
			reader.ReadProperty("unitType");
			reader.Read(ref unitType);
			reader.ReadProperty("parameterIndex");
			reader.Read(ref parameterIndex);
			reader.ReadProperty("key");
			reader.Read(ref key);
			reader.ReadProperty("color");
			UnityReaders.Read(reader, ref color);
			reader.ReadProperty("locked");
			reader.Read(ref locked);
			reader.ReadProperty("isGroup");
			reader.Read(ref isGroup);
			reader.ReadProperty("isSubgroup");
			reader.Read(ref isSubgroup);
			reader.ReadProperty("stacked");
			reader.Read(ref stacked);
			reader.ReadProperty("priority");
			reader.Read(ref priority);
			reader.ReadMapEnd();
		}

		public void Write(IJsonWriter writer)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			writer.TypeBegin("statistics.StatItem");
			writer.PropertyName("category");
			UnityWriters.Write(writer, category);
			writer.PropertyName("group");
			UnityWriters.Write(writer, group);
			writer.PropertyName("entity");
			UnityWriters.Write(writer, entity);
			writer.PropertyName("statisticType");
			writer.Write(statisticType);
			writer.PropertyName("unitType");
			writer.Write(unitType);
			writer.PropertyName("parameterIndex");
			writer.Write(parameterIndex);
			writer.PropertyName("key");
			writer.Write(key);
			writer.PropertyName("color");
			UnityWriters.Write(writer, color);
			writer.PropertyName("locked");
			writer.Write(locked);
			writer.PropertyName("isGroup");
			writer.Write(isGroup);
			writer.PropertyName("isSubgroup");
			writer.Write(isSubgroup);
			writer.PropertyName("stacked");
			writer.Write(stacked);
			writer.PropertyName("priority");
			writer.Write(priority);
			writer.TypeEnd();
		}

		public int CompareTo(StatItem other)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			if (isSubgroup != other.isSubgroup)
			{
				return isGroup.CompareTo(other.isGroup);
			}
			if (entity != other.entity)
			{
				return ((Entity)(ref entity)).CompareTo(other.entity);
			}
			return priority.CompareTo(other.priority);
		}
	}

	private const string kGroup = "statistics";

	private PrefabUISystem m_PrefabUISystem;

	private PrefabSystem m_PrefabSystem;

	private ResourceSystem m_ResourceSystem;

	private ICityStatisticsSystem m_CityStatisticsSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private MapTilePurchaseSystem m_MapTilePurchaseSystem;

	private TimeUISystem m_TimeUISystem;

	private EntityQuery m_StatisticsCategoryQuery;

	private EntityQuery m_TimeDataQuery;

	private EntityQuery m_UnlockedPrefabQuery;

	private List<StatItem> m_GroupCache;

	private List<StatItem> m_SubGroupCache;

	private List<StatItem> m_SelectedStatistics;

	private List<StatItem> m_SelectedStatisticsTracker;

	private Entity m_ActiveCategory;

	private Entity m_ActiveGroup;

	private int m_SampleRange;

	private bool m_Stacked;

	private RawMapBinding<Entity> m_GroupsMapBinding;

	private ValueBinding<int> m_SampleRangeBinding;

	private ValueBinding<int> m_SampleCountBinding;

	private GetterValueBinding<Entity> m_ActiveGroupBinding;

	private GetterValueBinding<Entity> m_ActiveCategoryBinding;

	private GetterValueBinding<bool> m_StackedBinding;

	private RawValueBinding m_SelectedStatisticsBinding;

	private RawValueBinding m_CategoriesBinding;

	private RawValueBinding m_DataBinding;

	private RawMapBinding<Entity> m_UnlockingRequirementsBinding;

	private bool m_ClearActive = true;

	private int m_UnlockRequirementVersion;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Expected O, but got Unknown
		//IL_0268: Expected O, but got Unknown
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Expected O, but got Unknown
		//IL_0294: Expected O, but got Unknown
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Expected O, but got Unknown
		//IL_02c0: Expected O, but got Unknown
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Expected O, but got Unknown
		base.OnCreate();
		m_StatisticsCategoryQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<UIObjectData>(),
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<UIStatisticsCategoryData>()
		});
		m_TimeDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeData>() });
		m_UnlockedPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Unlock>() });
		m_GroupCache = new List<StatItem>();
		m_SubGroupCache = new List<StatItem>();
		m_SelectedStatistics = new List<StatItem>();
		m_SelectedStatisticsTracker = new List<StatItem>();
		m_PrefabUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabUISystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		ICityStatisticsSystem cityStatisticsSystem = m_CityStatisticsSystem;
		cityStatisticsSystem.eventStatisticsUpdated = (Action)Delegate.Combine(cityStatisticsSystem.eventStatisticsUpdated, new Action(OnStatisticsUpdated));
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_TimeUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeUISystem>();
		m_MapTilePurchaseSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MapTilePurchaseSystem>();
		AddBinding((IBinding)(object)(m_GroupsMapBinding = new RawMapBinding<Entity>("statistics", "groups", (Action<IJsonWriter, Entity>)BindGroups, (IReader<Entity>)null, (IWriter<Entity>)null)));
		AddBinding((IBinding)(object)(m_SampleRangeBinding = new ValueBinding<int>("statistics", "sampleRange", m_SampleRange, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_SampleCountBinding = new ValueBinding<int>("statistics", "sampleCount", m_CityStatisticsSystem.sampleCount, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_ActiveGroupBinding = new GetterValueBinding<Entity>("statistics", "activeGroup", (Func<Entity>)(() => m_ActiveGroup), (IWriter<Entity>)null, (EqualityComparer<Entity>)null)));
		AddBinding((IBinding)(object)(m_ActiveCategoryBinding = new GetterValueBinding<Entity>("statistics", "activeCategory", (Func<Entity>)(() => m_ActiveCategory), (IWriter<Entity>)null, (EqualityComparer<Entity>)null)));
		AddBinding((IBinding)(object)(m_StackedBinding = new GetterValueBinding<bool>("statistics", "stacked", (Func<bool>)(() => m_Stacked), (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		RawValueBinding val = new RawValueBinding("statistics", "categories", (Action<IJsonWriter>)BindCategories);
		RawValueBinding binding = val;
		m_CategoriesBinding = val;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val2 = new RawValueBinding("statistics", "data", (Action<IJsonWriter>)BindData);
		binding = val2;
		m_DataBinding = val2;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val3 = new RawValueBinding("statistics", "selectedStatistics", (Action<IJsonWriter>)BindSelectedStatistics);
		binding = val3;
		m_SelectedStatisticsBinding = val3;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)(m_UnlockingRequirementsBinding = new RawMapBinding<Entity>("statistics", "unlockingRequirements", (Action<IJsonWriter, Entity>)BindUnlockingRequirements, (IReader<Entity>)null, (IWriter<Entity>)null)));
		AddBinding((IBinding)(object)new GetterValueBinding<int>("statistics", "updatesPerDay", (Func<int>)(() => 32), (IWriter<int>)null, (EqualityComparer<int>)null));
		AddBinding((IBinding)(object)new TriggerBinding<StatItem>("statistics", "addStat", (Action<StatItem>)ProcessAddStat, (IReader<StatItem>)(object)new ValueReader<StatItem>()));
		AddBinding((IBinding)(object)new TriggerBinding<StatItem>("statistics", "addStatChildren", (Action<StatItem>)ProcessAddStatChildren, (IReader<StatItem>)(object)new ValueReader<StatItem>()));
		AddBinding((IBinding)(object)new TriggerBinding<StatItem>("statistics", "removeStat", (Action<StatItem>)DeepRemoveStat, (IReader<StatItem>)(object)new ValueReader<StatItem>()));
		AddBinding((IBinding)new TriggerBinding("statistics", "clearStats", (Action)ClearStats));
		AddBinding((IBinding)(object)new TriggerBinding<int>("statistics", "setSampleRange", (Action<int>)SetSampleRange, (IReader<int>)null));
	}

	private void BindUnlockingRequirements(IJsonWriter writer, Entity prefabEntity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabUISystem.BindPrefabRequirements(writer, prefabEntity);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		m_SelectedStatistics.Clear();
		m_SampleRange = 32;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		m_SampleCountBinding.Update(m_CityStatisticsSystem.sampleCount);
		m_SampleRangeBinding.Update(m_SampleRange);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		int componentOrderVersion = ((EntityManager)(ref entityManager)).GetComponentOrderVersion<UnlockRequirementData>();
		if (PrefabUtils.HasUnlockedPrefab<UIObjectData>(((ComponentSystemBase)this).EntityManager, m_UnlockedPrefabQuery) || m_UnlockRequirementVersion != componentOrderVersion)
		{
			((MapBindingBase<Entity>)(object)m_UnlockingRequirementsBinding).UpdateAll();
			((MapBindingBase<Entity>)(object)m_GroupsMapBinding).UpdateAll();
			m_CategoriesBinding.Update();
		}
		m_UnlockRequirementVersion = componentOrderVersion;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		ICityStatisticsSystem cityStatisticsSystem = m_CityStatisticsSystem;
		cityStatisticsSystem.eventStatisticsUpdated = (Action)Delegate.Remove(cityStatisticsSystem.eventStatisticsUpdated, new Action(OnStatisticsUpdated));
		base.OnDestroy();
	}

	private void OnStatisticsUpdated()
	{
		m_DataBinding.Update();
	}

	private void BindSelectedStatistics(IJsonWriter binder)
	{
		JsonWriterExtensions.ArrayBegin(binder, m_SelectedStatistics.Count);
		for (int i = 0; i < m_SelectedStatistics.Count; i++)
		{
			StatItem statItem = m_SelectedStatistics[i];
			JsonWriterExtensions.Write<StatItem>(binder, statItem);
		}
		binder.ArrayEnd();
	}

	private void BindCategories(IJsonWriter binder)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		NativeList<StatCategory> sortedCategories = GetSortedCategories();
		JsonWriterExtensions.ArrayBegin(binder, sortedCategories.Length);
		for (int i = 0; i < sortedCategories.Length; i++)
		{
			StatCategory statCategory = sortedCategories[i];
			PrefabBase prefab = m_PrefabSystem.GetPrefab<PrefabBase>(statCategory.m_PrefabData);
			bool flag = EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, statCategory.m_Entity);
			binder.TypeBegin("statistics.StatCategory");
			binder.PropertyName("entity");
			UnityWriters.Write(binder, statCategory.m_Entity);
			binder.PropertyName("key");
			binder.Write(((Object)prefab).name);
			binder.PropertyName("locked");
			binder.Write(flag);
			binder.TypeEnd();
		}
		binder.ArrayEnd();
	}

	private NativeList<StatCategory> GetSortedCategories()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_StatisticsCategoryQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<UIObjectData> val2 = ((EntityQuery)(ref m_StatisticsCategoryQuery)).ToComponentDataArray<UIObjectData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<PrefabData> val3 = ((EntityQuery)(ref m_StatisticsCategoryQuery)).ToComponentDataArray<PrefabData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<StatCategory> val4 = default(NativeList<StatCategory>);
		val4._002Ector(val.Length, AllocatorHandle.op_Implicit((Allocator)2));
		for (int i = 0; i < val.Length; i++)
		{
			StatCategory statCategory = new StatCategory(val[i], val2[i], val3[i]);
			val4.Add(ref statCategory);
		}
		val.Dispose();
		val2.Dispose();
		val3.Dispose();
		NativeSortExtension.Sort<StatCategory>(val4);
		return val4;
	}

	private void CacheChildren(Entity parentEntity, List<StatItem> cache)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		cache.Clear();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		bool flag = ((EntityManager)(ref entityManager)).HasComponent<UIStatisticsCategoryData>(parentEntity);
		DynamicBuffer<UIGroupElement> elements = default(DynamicBuffer<UIGroupElement>);
		StatisticsData statisticsData2 = default(StatisticsData);
		PrefabData prefabData = default(PrefabData);
		if (EntitiesExtensions.TryGetBuffer<UIGroupElement>(((ComponentSystemBase)this).EntityManager, parentEntity, true, ref elements))
		{
			NativeList<UIObjectInfo> sortedObjects = UIObjectInfo.GetSortedObjects(((ComponentSystemBase)this).EntityManager, elements, (Allocator)3);
			Color color = default(Color);
			StatisticsData statisticsData = default(StatisticsData);
			UIStatisticsGroupData uIStatisticsGroupData = default(UIStatisticsGroupData);
			UIObjectData uIObjectData = default(UIObjectData);
			for (int i = 0; i < sortedObjects.Length; i++)
			{
				Entity category = Entity.Null;
				Entity val = Entity.Null;
				Entity entity = sortedObjects[i].entity;
				PrefabBase prefab = m_PrefabSystem.GetPrefab<PrefabBase>(entity);
				StatisticUnitType unitType = StatisticUnitType.None;
				StatisticType statisticType = StatisticType.Invalid;
				bool locked = EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, entity);
				int num;
				if (!flag)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<UIStatisticsGroupData>(entity))
					{
						num = 1;
						goto IL_00bd;
					}
				}
				num = ((prefab is ParametricStatistic parametricStatistic && parametricStatistic.GetParameters().Count() > 1) ? 1 : 0);
				goto IL_00bd;
				IL_00bd:
				bool isSubgroup = (byte)num != 0;
				bool stacked = true;
				((Color)(ref color))._002Ector(0f, 0f, 0f, 0f);
				if (!m_MapTilePurchaseSystem.GetMapTileUpkeepEnabled() && ((Object)prefab).name == "MapTileUpkeep")
				{
					continue;
				}
				if (EntitiesExtensions.TryGetComponent<StatisticsData>(((ComponentSystemBase)this).EntityManager, entity, ref statisticsData))
				{
					if (m_CityConfigurationSystem.unlimitedMoney && (statisticsData.m_StatisticType == StatisticType.Money || ((Object)prefab).name == "LoanInterest"))
					{
						continue;
					}
					unitType = statisticsData.m_UnitType;
					statisticType = statisticsData.m_StatisticType;
					val = statisticsData.m_Group;
					category = statisticsData.m_Category;
					color = statisticsData.m_Color;
					stacked = statisticsData.m_Stacked;
				}
				if (EntitiesExtensions.TryGetComponent<UIStatisticsGroupData>(((ComponentSystemBase)this).EntityManager, entity, ref uIStatisticsGroupData) && EntitiesExtensions.TryGetComponent<UIObjectData>(((ComponentSystemBase)this).EntityManager, entity, ref uIObjectData))
				{
					val = ((uIObjectData.m_Group == uIStatisticsGroupData.m_Category) ? entity : uIObjectData.m_Group);
					unitType = uIStatisticsGroupData.m_UnitType;
					category = uIStatisticsGroupData.m_Category;
					color = uIStatisticsGroupData.m_Color;
					stacked = uIStatisticsGroupData.m_Stacked;
				}
				cache.Add(new StatItem(i, category, val, entity, (int)statisticType, unitType, 0, ((Object)prefab).name, color, locked, flag, isSubgroup, stacked));
			}
			sortedObjects.Dispose();
		}
		else if (EntitiesExtensions.TryGetComponent<StatisticsData>(((ComponentSystemBase)this).EntityManager, parentEntity, ref statisticsData2) && EntitiesExtensions.TryGetComponent<PrefabData>(((ComponentSystemBase)this).EntityManager, parentEntity, ref prefabData))
		{
			bool locked2 = EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, parentEntity);
			CacheParameterChildren(parentEntity, locked2, statisticsData2, prefabData, cache);
		}
	}

	private void CacheParameterChildren(Entity parent, bool locked, StatisticsData statisticsData, PrefabData prefabData, List<StatItem> cache)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		ParametricStatistic prefab = m_PrefabSystem.GetPrefab<ParametricStatistic>(prefabData);
		DynamicBuffer<StatisticParameterData> val = default(DynamicBuffer<StatisticParameterData>);
		if (EntitiesExtensions.TryGetBuffer<StatisticParameterData>(((ComponentSystemBase)this).EntityManager, parent, true, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				cache.Add(new StatItem(i, statisticsData.m_Category, (statisticsData.m_Group == Entity.Null) ? parent : statisticsData.m_Group, parent, (int)prefab.m_StatisticsType, prefab.m_UnitType, i, ((Object)prefab).name + prefab.GetParameterName(val[i].m_Value), val[i].m_Color, locked, isGroup: false, isSubgroup: false, statisticsData.m_Stacked));
			}
		}
	}

	private void BindGroups(IJsonWriter binder, Entity parent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		CacheChildren(parent, m_GroupCache);
		JsonWriterExtensions.ArrayBegin(binder, m_GroupCache.Count);
		for (int i = 0; i < m_GroupCache.Count; i++)
		{
			JsonWriterExtensions.Write<StatItem>(binder, m_GroupCache[i]);
		}
		binder.ArrayEnd();
	}

	private void BindData(IJsonWriter binder)
	{
		JsonWriterExtensions.ArrayBegin(binder, m_SelectedStatistics.Count);
		for (int num = m_SelectedStatistics.Count - 1; num >= 0; num--)
		{
			BindData(binder, m_SelectedStatistics[num]);
		}
		binder.ArrayEnd();
	}

	private void BindData(IJsonWriter binder, StatItem stat)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		binder.TypeBegin("statistics.ChartDataSets");
		binder.PropertyName("label");
		binder.Write(stat.key);
		binder.PropertyName("data");
		NativeList<DataPoint> statisticData = GetStatisticData(stat);
		JsonWriterExtensions.ArrayBegin(binder, statisticData.Length);
		for (int i = 0; i < statisticData.Length; i++)
		{
			JsonWriterExtensions.Write<DataPoint>(binder, statisticData[i]);
		}
		binder.ArrayEnd();
		binder.PropertyName("borderColor");
		binder.Write(stat.color.ToHexCode());
		binder.PropertyName("backgroundColor");
		binder.Write($"rgba({Mathf.RoundToInt(stat.color.r * 255f)}, {Mathf.RoundToInt(stat.color.g * 255f)}, {Mathf.RoundToInt(stat.color.b * 255f)}, 0.5)");
		binder.PropertyName("fill");
		if (m_Stacked)
		{
			binder.Write("origin");
		}
		else
		{
			binder.Write("false");
		}
		binder.TypeEnd();
	}

	private NativeList<DataPoint> GetStatisticData(StatItem stat)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		m_CityStatisticsSystem.CompleteWriters();
		StatisticsPrefab prefab = m_PrefabSystem.GetPrefab<StatisticsPrefab>(stat.entity);
		ResourcePrefabs prefabs = m_ResourceSystem.GetPrefabs();
		TimeData singleton = TimeData.GetSingleton(m_TimeDataQuery);
		int sampleCount = m_CityStatisticsSystem.sampleCount;
		int num = math.min(m_SampleRange + 1, sampleCount);
		if (sampleCount <= 1)
		{
			NativeList<DataPoint> result = default(NativeList<DataPoint>);
			result._002Ector(1, AllocatorHandle.op_Implicit((Allocator)2));
			DataPoint dataPoint = new DataPoint
			{
				x = singleton.m_FirstFrame,
				y = 0L
			};
			result.Add(ref dataPoint);
			return result;
		}
		NativeArray<long> data = CollectionHelper.CreateNativeArray<long>(num, AllocatorHandle.op_Implicit((Allocator)2), (NativeArrayOptions)1);
		StatisticParameterData[] array = ((prefab is ParametricStatistic parametricStatistic) ? parametricStatistic.GetParameters().ToArray() : new StatisticParameterData[1]
		{
			new StatisticParameterData
			{
				m_Value = 0
			}
		});
		EntityManager entityManager;
		if (stat.isSubgroup)
		{
			for (int i = 0; i < array.Length; i++)
			{
				int value = array[i].m_Value;
				NativeArray<long> statisticDataArrayLong = m_CityStatisticsSystem.GetStatisticDataArrayLong((StatisticType)stat.statisticType, value);
				statisticDataArrayLong = EnsureDataSize(statisticDataArrayLong);
				for (int j = 0; j < num; j++)
				{
					long num2 = statisticDataArrayLong[statisticDataArrayLong.Length - num + j];
					if (stat.statisticType == 4 && prefab is ResourceStatistic resourceStatistic)
					{
						Resource resource = EconomyUtils.GetResource(resourceStatistic.m_Resources[i].m_Resource);
						Entity val = prefabs[resource];
						entityManager = ((ComponentSystemBase)this).EntityManager;
						ResourceData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ResourceData>(val);
						num2 *= (int)EconomyUtils.GetMarketPrice(componentData);
					}
					ref NativeArray<long> reference = ref data;
					int num3 = j;
					reference[num3] += num2;
				}
			}
		}
		else
		{
			int value2 = array[stat.parameterIndex].m_Value;
			NativeArray<long> statisticDataArrayLong2 = m_CityStatisticsSystem.GetStatisticDataArrayLong((StatisticType)stat.statisticType, value2);
			NativeArray<long> val2 = CollectionHelper.CreateNativeArray<long>(0, AllocatorHandle.op_Implicit((Allocator)2), (NativeArrayOptions)1);
			if (stat.statisticType == 16 || stat.statisticType == 15)
			{
				val2 = m_CityStatisticsSystem.GetStatisticDataArrayLong(StatisticType.Population);
				val2 = EnsureDataSize(val2);
			}
			statisticDataArrayLong2 = EnsureDataSize(statisticDataArrayLong2);
			for (int k = 0; k < num; k++)
			{
				long num4 = statisticDataArrayLong2[statisticDataArrayLong2.Length - num + k];
				if (stat.statisticType == 4 && prefab is ResourceStatistic resourceStatistic2)
				{
					Resource resource2 = EconomyUtils.GetResource(resourceStatistic2.m_Resources[stat.parameterIndex].m_Resource);
					Entity val3 = prefabs[resource2];
					entityManager = ((ComponentSystemBase)this).EntityManager;
					ResourceData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<ResourceData>(val3);
					num4 *= (int)EconomyUtils.GetMarketPrice(componentData2);
				}
				if (val2.Length > 0 && (stat.statisticType == 16 || stat.statisticType == 15))
				{
					long num5 = val2[val2.Length - num + k];
					if (num5 > 0)
					{
						num4 /= num5;
					}
				}
				ref NativeArray<long> reference = ref data;
				int num3 = k;
				reference[num3] += num4;
			}
		}
		return GetDataPoints(num, sampleCount, data, singleton);
	}

	private NativeArray<long> EnsureDataSize(NativeArray<long> data)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (data.Length < m_CityStatisticsSystem.sampleCount)
		{
			NativeArray<long> result = CollectionHelper.CreateNativeArray<long>(m_CityStatisticsSystem.sampleCount, AllocatorHandle.op_Implicit((Allocator)2), (NativeArrayOptions)1);
			int num = 0;
			for (int i = 0; i < result.Length; i++)
			{
				if (i < result.Length - data.Length)
				{
					result[i] = 0L;
				}
				else
				{
					result[i] = data[num++];
				}
			}
			return result;
		}
		return data;
	}

	private NativeList<DataPoint> GetDataPoints(int range, int samples, NativeArray<long> data, TimeData timeData)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		int sampleInterval = GetSampleInterval(range);
		NativeList<DataPoint> result = default(NativeList<DataPoint>);
		result._002Ector(data.Length / sampleInterval, AllocatorHandle.op_Implicit((Allocator)2));
		int num = 0;
		uint num2 = (uint)math.max((int)(m_CityStatisticsSystem.GetSampleFrameIndex(samples - range) - timeData.m_FirstFrame), 0);
		DataPoint dataPoint = new DataPoint
		{
			x = (uint)math.max((long)num2, (long)(m_TimeUISystem.GetTicks() - 8192 * m_SampleRange)),
			y = data[0]
		};
		result.Add(ref dataPoint);
		if (data.Length > 2)
		{
			for (int i = 1; i < data.Length - 1; i++)
			{
				if (num % sampleInterval == 0)
				{
					uint sampleFrameIndex = m_CityStatisticsSystem.GetSampleFrameIndex(samples - range + i);
					dataPoint = new DataPoint
					{
						x = sampleFrameIndex - timeData.m_FirstFrame,
						y = data[i]
					};
					result.Add(ref dataPoint);
				}
				num++;
			}
		}
		m_CityStatisticsSystem.GetSampleFrameIndex(samples);
		dataPoint = new DataPoint
		{
			x = (uint)(m_TimeUISystem.GetTicks() + 182 + 1)
		};
		dataPoint.y = data[data.Length - 1];
		result.Add(ref dataPoint);
		return result;
	}

	private int GetSampleInterval(int range)
	{
		int num = 32;
		if (range <= num)
		{
			return 1;
		}
		int num2 = num - 2;
		return Math.Max(1, (range - 2) / num2);
	}

	private void CheckActiveCategory(StatItem stat)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (stat.category != m_ActiveCategory)
		{
			m_SelectedStatistics.Clear();
			m_SelectedStatisticsTracker.Clear();
			m_ActiveCategory = stat.category;
			m_ActiveCategoryBinding.Update();
		}
	}

	private void CheckActiveGroup(StatItem stat)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (m_ActiveGroup == Entity.Null || stat.isGroup || stat.group != m_ActiveGroup)
		{
			m_SelectedStatistics.Clear();
			m_SelectedStatisticsTracker.Clear();
			m_ActiveGroup = (stat.isGroup ? stat.entity : stat.group);
			m_ActiveGroupBinding.Update();
		}
	}

	private void ProcessAddStat(StatItem stat)
	{
		if (stat.locked)
		{
			return;
		}
		CheckActiveCategory(stat);
		CheckActiveGroup(stat);
		if (stat.isGroup)
		{
			AddStat(stat, onlyTracker: true);
			if (!TryAddChildren(stat, m_GroupCache))
			{
				m_SelectedStatistics.Add(stat);
			}
		}
		else if (stat.isSubgroup)
		{
			RemoveStatChildren(stat);
			AddStat(stat, onlyTracker: false);
		}
		else
		{
			RemoveStatParent(stat);
			AddStat(stat, onlyTracker: false);
		}
		UpdateStackedStatus();
		UpdateStats();
	}

	private void ProcessAddStatChildren(StatItem stat)
	{
		if (!stat.locked)
		{
			CheckActiveCategory(stat);
			CheckActiveGroup(stat);
			if (stat.isSubgroup)
			{
				RemoveStat(stat, keepTracker: true);
				TryAddChildren(stat, m_SubGroupCache);
			}
		}
	}

	private void UpdateStackedStatus()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		UIStatisticsGroupData uIStatisticsGroupData = default(UIStatisticsGroupData);
		if (m_SelectedStatisticsTracker.Count((StatItem stat) => stat.isSubgroup && stat.group == m_ActiveGroup) > 1 && EntitiesExtensions.TryGetComponent<UIStatisticsGroupData>(((ComponentSystemBase)this).EntityManager, m_ActiveGroup, ref uIStatisticsGroupData))
		{
			m_Stacked = uIStatisticsGroupData.m_Stacked;
		}
		else if (m_SelectedStatisticsTracker.Count > 0)
		{
			m_Stacked = false;
			for (int num = 0; num < m_SelectedStatisticsTracker.Count; num++)
			{
				if (m_SelectedStatisticsTracker[num].stacked)
				{
					m_Stacked = true;
					break;
				}
			}
		}
		else
		{
			m_Stacked = false;
		}
		m_StackedBinding.Update();
	}

	private void AddStat(StatItem stat, bool onlyTracker)
	{
		if (!m_SelectedStatisticsTracker.Contains(stat))
		{
			m_SelectedStatisticsTracker.Add(stat);
		}
		if (!onlyTracker && !m_SelectedStatistics.Contains(stat))
		{
			m_SelectedStatistics.Add(stat);
		}
	}

	private bool TryAddChildren(StatItem stat, List<StatItem> cache)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		CacheChildren(stat.entity, cache);
		for (int i = 0; i < cache.Count; i++)
		{
			ProcessAddStat(cache[i]);
		}
		return cache.Count > 0;
	}

	private void DeepRemoveStat(StatItem stat)
	{
		if (!m_SelectedStatisticsTracker.Contains(stat))
		{
			int num = m_SelectedStatisticsTracker.FindIndex((StatItem s) => s.entity == stat.group);
			int num2 = m_SelectedStatisticsTracker.FindIndex((StatItem s) => s.entity == stat.entity && s.isSubgroup);
			if (num >= 0)
			{
				StatItem stat2 = m_SelectedStatisticsTracker[num];
				if (num2 >= 0)
				{
					StatItem stat3 = m_SelectedStatisticsTracker[num2];
					DeepRemoveStat(stat2);
					ProcessAddStat(stat3);
				}
				else
				{
					DeepRemoveStat(stat2);
				}
			}
		}
		int num3 = m_SelectedStatisticsTracker.Count((StatItem s) => s.isSubgroup);
		RemoveStat(stat, keepTracker: false);
		RemoveStatChildren(stat);
		int num4 = m_SelectedStatisticsTracker.Count((StatItem s) => s.isSubgroup);
		if (num3 > 1 && num4 == 1)
		{
			StatItem stat4 = m_SelectedStatisticsTracker.First((StatItem s) => s.isSubgroup);
			RemoveStat(stat4, keepTracker: false);
			ProcessAddStat(stat4);
		}
		if (m_ClearActive && m_SelectedStatistics.Count == 0 && m_SelectedStatisticsTracker.Count <= 1)
		{
			ClearStats();
		}
		else
		{
			UpdateStats();
		}
		m_ClearActive = true;
		UpdateStackedStatus();
	}

	private void RemoveStatParent(StatItem stat)
	{
		int num = m_SelectedStatisticsTracker.FindIndex((StatItem s) => s.entity == stat.entity && s.isSubgroup);
		if (num != -1)
		{
			StatItem stat2 = m_SelectedStatisticsTracker[num];
			RemoveStat(stat2, keepTracker: true);
		}
	}

	private void RemoveStatChildren(StatItem stat)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if (stat.isGroup)
		{
			for (int num = m_SelectedStatistics.Count - 1; num >= 0; num--)
			{
				if (m_SelectedStatistics[num].group == stat.entity)
				{
					m_SelectedStatistics.RemoveAt(num);
				}
			}
			for (int num2 = m_SelectedStatisticsTracker.Count - 1; num2 >= 0; num2--)
			{
				if (m_SelectedStatisticsTracker[num2].group == stat.entity)
				{
					m_SelectedStatisticsTracker.RemoveAt(num2);
				}
			}
		}
		else
		{
			if (!stat.isSubgroup)
			{
				return;
			}
			for (int num3 = m_SelectedStatistics.Count - 1; num3 >= 0; num3--)
			{
				if (m_SelectedStatistics[num3].entity == stat.entity)
				{
					m_SelectedStatistics.RemoveAt(num3);
				}
			}
			for (int num4 = m_SelectedStatisticsTracker.Count - 1; num4 >= 0; num4--)
			{
				if (m_SelectedStatisticsTracker[num4].entity == stat.entity)
				{
					m_SelectedStatisticsTracker.RemoveAt(num4);
				}
			}
		}
	}

	private void RemoveStat(StatItem stat, bool keepTracker)
	{
		m_SelectedStatistics.Remove(stat);
		if (!keepTracker)
		{
			m_SelectedStatisticsTracker.Remove(stat);
		}
	}

	private void ClearStats()
	{
		m_SelectedStatistics.Clear();
		m_SelectedStatisticsTracker.Clear();
		UpdateStats();
		ClearActive();
	}

	private void UpdateStats()
	{
		m_SelectedStatistics.Sort();
		m_SelectedStatisticsBinding.Update();
		m_DataBinding.Update();
	}

	private void ClearActive()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		m_ActiveGroup = Entity.Null;
		m_ActiveGroupBinding.Update();
		m_ActiveCategory = Entity.Null;
		m_ActiveCategoryBinding.Update();
	}

	private void SetSampleRange(int range)
	{
		m_SampleRange = range;
		m_SampleRangeBinding.Update(m_SampleRange);
		UpdateStats();
	}

	[Preserve]
	public StatisticsUISystem()
	{
	}
}
