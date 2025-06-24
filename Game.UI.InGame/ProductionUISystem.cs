using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Companies;
using Game.Economy;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class ProductionUISystem : UISystemBase
{
	private const string kGroup = "production";

	private UIUpdateState m_UpdateState;

	private PrefabSystem m_PrefabSystem;

	private ResourceSystem m_ResourceSystem;

	private CommercialDemandSystem m_CommercialDemandSystem;

	private IndustrialDemandSystem m_IndustrialDemandSystem;

	private CountCompanyDataSystem m_CountCompanyDataSystem;

	private EntityQuery m_ResourceCategoryQuery;

	private EntityQuery m_IndustrialCompanyQuery;

	private EntityQuery m_CommercialCompanyQuery;

	private EntityQuery m_ServiceUpkeepQuery;

	private NativeParallelMultiHashMap<Entity, (Entity, Entity)> m_ProductionChain;

	private GetterValueBinding<int> m_MaxProgressBinding;

	private RawValueBinding m_ResourceCategoriesBinding;

	private RawMapBinding<Entity> m_ResourceDetailsBinding;

	private RawMapBinding<Entity> m_ResourceBinding;

	private RawMapBinding<Entity> m_ServiceBinding;

	private RawMapBinding<Entity> m_DataBinding;

	private NativeList<int> m_ProductionCache;

	private NativeList<int> m_CommercialConsumptionCache;

	private NativeList<int> m_IndustrialConsumptionCache;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Expected O, but got Unknown
		//IL_0199: Expected O, but got Unknown
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_UpdateState = UIUpdateState.Create(((ComponentSystemBase)this).World, 256);
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_CommercialDemandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CommercialDemandSystem>();
		m_IndustrialDemandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IndustrialDemandSystem>();
		m_CountCompanyDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountCompanyDataSystem>();
		m_ResourceCategoryQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<UIGroupElement>(),
			ComponentType.ReadOnly<UIResourceCategoryData>(),
			ComponentType.ReadOnly<UIObjectData>()
		});
		m_IndustrialCompanyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<IndustrialProcessData>(),
			ComponentType.Exclude<ServiceCompanyData>(),
			ComponentType.Exclude<StorageCompanyData>()
		});
		m_CommercialCompanyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<IndustrialProcessData>(),
			ComponentType.ReadOnly<ServiceCompanyData>(),
			ComponentType.Exclude<StorageCompanyData>()
		});
		m_ServiceUpkeepQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<ServiceUpkeepData>(),
			ComponentType.ReadOnly<ServiceObjectData>()
		});
		AddBinding((IBinding)(object)(m_MaxProgressBinding = new GetterValueBinding<int>("production", "maxProgress", (Func<int>)GetMaxProgress, (IWriter<int>)null, (EqualityComparer<int>)null)));
		RawValueBinding val = new RawValueBinding("production", "resourceCategories", (Action<IJsonWriter>)WriteResourceCategories);
		RawValueBinding binding = val;
		m_ResourceCategoriesBinding = val;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)(m_ResourceBinding = new RawMapBinding<Entity>("production", "resources", (Action<IJsonWriter, Entity>)WriteResource, (IReader<Entity>)null, (IWriter<Entity>)null)));
		AddBinding((IBinding)(object)(m_ResourceDetailsBinding = new RawMapBinding<Entity>("production", "resourceDetails", (Action<IJsonWriter, Entity>)WriteResourceDetails, (IReader<Entity>)null, (IWriter<Entity>)null)));
		AddBinding((IBinding)(object)(m_ServiceBinding = new RawMapBinding<Entity>("production", "services", (Action<IJsonWriter, Entity>)WriteService, (IReader<Entity>)null, (IWriter<Entity>)null)));
		AddBinding((IBinding)(object)(m_DataBinding = new RawMapBinding<Entity>("production", "data", (Action<IJsonWriter, Entity>)WriteData, (IReader<Entity>)null, (IWriter<Entity>)null)));
		m_ProductionChain = new NativeParallelMultiHashMap<Entity, (Entity, Entity)>(50, AllocatorHandle.op_Implicit((Allocator)4));
		m_ProductionCache = new NativeList<int>(AllocatorHandle.op_Implicit((Allocator)4));
		m_CommercialConsumptionCache = new NativeList<int>(AllocatorHandle.op_Implicit((Allocator)4));
		m_IndustrialConsumptionCache = new NativeList<int>(AllocatorHandle.op_Implicit((Allocator)4));
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		if (GameManager.instance.gameMode == GameMode.Game)
		{
			BuildProductionChain(m_ProductionChain);
			UpdateCache();
			m_MaxProgressBinding.Update();
			m_ResourceCategoriesBinding.Update();
			((MapBindingBase<Entity>)(object)m_ResourceBinding).Update();
			((MapBindingBase<Entity>)(object)m_ResourceDetailsBinding).Update();
			((MapBindingBase<Entity>)(object)m_ServiceBinding).Update();
		}
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_ProductionChain.Dispose();
		m_ProductionCache.Dispose();
		m_CommercialConsumptionCache.Dispose();
		m_IndustrialConsumptionCache.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (GameManager.instance.gameMode == GameMode.Game && m_UpdateState.Advance())
		{
			UpdateCache();
			((MapBindingBase<Entity>)(object)m_DataBinding).Update();
			m_MaxProgressBinding.Update();
		}
	}

	private void UpdateCache()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		NativeArray<int> production = m_CountCompanyDataSystem.GetProduction(out deps);
		JobHandle deps2;
		CountCompanyDataSystem.CommercialCompanyDatas commercialCompanyDatas = m_CountCompanyDataSystem.GetCommercialCompanyDatas(out deps2);
		JobHandle deps3;
		NativeArray<int> consumption = m_IndustrialDemandSystem.GetConsumption(out deps3);
		JobHandle deps4;
		NativeArray<int> consumption2 = m_CommercialDemandSystem.GetConsumption(out deps4);
		JobHandle.CompleteAll(ref deps, ref deps3, ref deps4);
		((JobHandle)(ref deps2)).Complete();
		m_ProductionCache.CopyFrom(ref production);
		m_IndustrialConsumptionCache.CopyFrom(ref consumption);
		m_CommercialConsumptionCache.CopyFrom(ref consumption2);
		for (int i = 0; i < m_ProductionCache.Length; i++)
		{
			ResourcePrefabs prefabs = m_ResourceSystem.GetPrefabs();
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).GetComponentData<ResourceData>(prefabs[EconomyUtils.GetResource(i)]).m_IsProduceable)
			{
				m_ProductionCache[i] = commercialCompanyDatas.m_ProduceCapacity[i];
			}
		}
	}

	private int GetMaxProgress()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		ResourceIterator iterator = ResourceIterator.GetIterator();
		ResourcePrefabs prefabs = m_ResourceSystem.GetPrefabs();
		while (iterator.Next())
		{
			Entity entity = prefabs[iterator.resource];
			ResourcePrefab prefab = m_PrefabSystem.GetPrefab<ResourcePrefab>(entity);
			if (prefab.m_IsLeisure || prefab.m_IsMaterial || prefab.m_IsProduceable)
			{
				(int, int, int) data = GetData(entity);
				int item = data.Item1;
				int item2 = data.Item2;
				int item3 = data.Item3;
				num = math.max(num, math.max(item, item2 + item3));
			}
		}
		return num;
	}

	private void WriteResourceCategories(IJsonWriter writer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		NativeList<UIObjectInfo> sortedObjects = UIObjectInfo.GetSortedObjects(m_ResourceCategoryQuery, (Allocator)3);
		try
		{
			JsonWriterExtensions.ArrayBegin(writer, sortedObjects.Length);
			for (int i = 0; i < sortedObjects.Length; i++)
			{
				Entity entity = sortedObjects[i].entity;
				UIResourceCategoryPrefab prefab = m_PrefabSystem.GetPrefab<UIResourceCategoryPrefab>(sortedObjects[i].prefabData);
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				EntityManager entityManager2 = ((ComponentSystemBase)this).EntityManager;
				NativeList<UIObjectInfo> objects = UIObjectInfo.GetObjects(entityManager, ((EntityManager)(ref entityManager2)).GetBuffer<UIGroupElement>(entity, true), (Allocator)3);
				NativeSortExtension.Sort<UIObjectInfo>(objects);
				try
				{
					writer.TypeBegin("production.ResourceCategory");
					writer.PropertyName("entity");
					UnityWriters.Write(writer, entity);
					writer.PropertyName("name");
					writer.Write(((Object)prefab).name);
					writer.PropertyName("resources");
					JsonWriterExtensions.ArrayBegin(writer, objects.Length);
					for (int j = 0; j < objects.Length; j++)
					{
						WriteResource(writer, objects[j].entity);
					}
					writer.ArrayEnd();
					writer.TypeEnd();
				}
				finally
				{
					objects.Dispose();
				}
			}
			writer.ArrayEnd();
		}
		finally
		{
			sortedObjects.Dispose();
		}
	}

	public void WriteResource(IJsonWriter writer, Entity entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		ResourcePrefab prefab = m_PrefabSystem.GetPrefab<ResourcePrefab>(entity);
		UIProductionLinks component = prefab.GetComponent<UIProductionLinks>();
		Resource resource = EconomyUtils.GetResource(prefab.m_Resource);
		try
		{
			writer.TypeBegin("production.Resource");
			writer.PropertyName("entity");
			UnityWriters.Write(writer, entity);
			writer.PropertyName("name");
			writer.Write(resource.ToString());
			writer.PropertyName("icon");
			writer.Write(ImageSystem.GetIcon(prefab));
			writer.PropertyName("tradable");
			writer.Write(prefab.m_IsTradable);
			writer.PropertyName("producer");
			WriteProductionLink(writer, component.m_Producer);
			writer.PropertyName("consumers");
			if (component.m_FinalConsumers != null)
			{
				JsonWriterExtensions.ArrayBegin(writer, component.m_FinalConsumers.Length);
				for (int i = 0; i < component.m_FinalConsumers.Length; i++)
				{
					WriteProductionLink(writer, component.m_FinalConsumers[i]);
				}
				writer.ArrayEnd();
			}
			else
			{
				JsonWriterExtensions.WriteEmptyArray(writer);
			}
			writer.TypeEnd();
		}
		catch (Exception ex)
		{
			writer.WriteNull();
			Debug.LogError((object)ex);
		}
	}

	private void WriteProductionLink(IJsonWriter writer, UIProductionLinkPrefab prefab)
	{
		writer.TypeBegin("ProductionLink");
		writer.PropertyName("name");
		writer.Write(prefab.m_Type.ToString());
		writer.PropertyName("icon");
		writer.Write(prefab.m_Icon);
		writer.TypeEnd();
	}

	private void WriteResourceDetails(IJsonWriter writer, Entity entity)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		NativeList<Entity> outputs = default(NativeList<Entity>);
		outputs._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<Entity> outputs2 = default(NativeList<Entity>);
		outputs2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeKeyValueArrays<Entity, (Entity, Entity)> keyValueArrays = m_ProductionChain.GetKeyValueArrays(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<Entity> serviceUpkeeps = ((EntityQuery)(ref m_ServiceUpkeepQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			writer.TypeBegin("production.ResourceDetails");
			writer.PropertyName("inputs");
			JsonWriterExtensions.ArrayBegin(writer, m_ProductionChain.CountValuesForKey(entity));
			Enumerator<Entity, (Entity, Entity)> enumerator = m_ProductionChain.GetValuesForKey(entity).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					(Entity, Entity) current = enumerator.Current;
					int num = 0;
					if (current.Item1 != Entity.Null)
					{
						num++;
					}
					if (current.Item2 != Entity.Null)
					{
						num++;
					}
					JsonWriterExtensions.ArrayBegin(writer, num);
					if (current.Item1 != Entity.Null)
					{
						UnityWriters.Write(writer, current.Item1);
					}
					if (current.Item2 != Entity.Null)
					{
						UnityWriters.Write(writer, current.Item2);
					}
					writer.ArrayEnd();
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			writer.ArrayEnd();
			FindOutputs(entity, outputs, keyValueArrays);
			writer.PropertyName("outputs");
			JsonWriterExtensions.ArrayBegin(writer, outputs.Length);
			for (int i = 0; i < outputs.Length; i++)
			{
				UnityWriters.Write(writer, outputs[i]);
			}
			writer.ArrayEnd();
			FindServiceOutputs(entity, outputs2, serviceUpkeeps);
			writer.PropertyName("serviceOutputs");
			JsonWriterExtensions.ArrayBegin(writer, outputs2.Length);
			for (int j = 0; j < outputs2.Length; j++)
			{
				UnityWriters.Write(writer, outputs2[j]);
			}
			writer.ArrayEnd();
			writer.TypeEnd();
		}
		finally
		{
			keyValueArrays.Dispose();
			outputs.Dispose();
			outputs2.Dispose();
			serviceUpkeeps.Dispose();
		}
	}

	private void FindServiceOutputs(Entity entity, NativeList<Entity> outputs, NativeArray<Entity> serviceUpkeeps)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		ResourcePrefabs prefabs = m_ResourceSystem.GetPrefabs();
		for (int i = 0; i < serviceUpkeeps.Length; i++)
		{
			Entity val = serviceUpkeeps[i];
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<ServiceUpkeepData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ServiceUpkeepData>(val, true);
			for (int j = 0; j < buffer.Length; j++)
			{
				Resource resource = buffer[j].m_Upkeep.m_Resource;
				if (resource == Resource.NoResource)
				{
					continue;
				}
				Entity val2 = prefabs[resource];
				if (entity == val2)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					Entity service = ((EntityManager)(ref entityManager)).GetComponentData<ServiceObjectData>(val).m_Service;
					if (!NativeListExtensions.Contains<Entity, Entity>(outputs, service))
					{
						outputs.Add(ref service);
					}
				}
			}
		}
	}

	private void FindOutputs(Entity entity, NativeList<Entity> outputs, NativeKeyValueArrays<Entity, (Entity, Entity)> keyValueArrays)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < keyValueArrays.Length; i++)
		{
			if (keyValueArrays.Values[i].Item1 == entity || keyValueArrays.Values[i].Item2 == entity)
			{
				Entity val = keyValueArrays.Keys[i];
				outputs.Add(ref val);
			}
		}
	}

	private void WriteService(IJsonWriter writer, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		PrefabData prefabData = default(PrefabData);
		if (EntitiesExtensions.TryGetComponent<PrefabData>(((ComponentSystemBase)this).EntityManager, entity, ref prefabData))
		{
			ServicePrefab prefab = m_PrefabSystem.GetPrefab<ServicePrefab>(prefabData);
			writer.TypeBegin("production.Service");
			writer.PropertyName("entity");
			UnityWriters.Write(writer, entity);
			writer.PropertyName("name");
			writer.Write(((Object)prefab).name);
			writer.PropertyName("icon");
			writer.Write(ImageSystem.GetIcon(prefab));
			writer.TypeEnd();
		}
		else
		{
			writer.WriteNull();
		}
	}

	private void WriteData(IJsonWriter writer, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		var (num, num2, num3) = GetData(entity);
		writer.TypeBegin("production.ResourceData");
		writer.PropertyName("production");
		writer.Write(num);
		writer.PropertyName("surplus");
		writer.Write(num2);
		writer.PropertyName("deficit");
		writer.Write(num3);
		writer.TypeEnd();
	}

	private (int, int, int) GetData(Entity entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		int resourceIndex = EconomyUtils.GetResourceIndex(EconomyUtils.GetResource(m_PrefabSystem.GetPrefab<ResourcePrefab>(entity).m_Resource));
		int num = m_ProductionCache[resourceIndex];
		int num2 = m_CommercialConsumptionCache[resourceIndex] + m_IndustrialConsumptionCache[resourceIndex];
		int num3 = math.min(num2, num);
		int num4 = math.min(num2, num3);
		int item = num - num3;
		int item2 = num2 - num4;
		return (num, item, item2);
	}

	private void BuildProductionChain(NativeParallelMultiHashMap<Entity, (Entity, Entity)> multiHashMap)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		ResourcePrefabs prefabs = m_ResourceSystem.GetPrefabs();
		NativeArray<IndustrialProcessData> datas = ((EntityQuery)(ref m_IndustrialCompanyQuery)).ToComponentDataArray<IndustrialProcessData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<IndustrialProcessData> datas2 = ((EntityQuery)(ref m_CommercialCompanyQuery)).ToComponentDataArray<IndustrialProcessData>(AllocatorHandle.op_Implicit((Allocator)3));
		ProcessProductionChainDatas(datas, prefabs, multiHashMap);
		ProcessProductionChainDatas(datas2, prefabs, multiHashMap);
		datas.Dispose();
		datas2.Dispose();
	}

	private void ProcessProductionChainDatas(NativeArray<IndustrialProcessData> datas, ResourcePrefabs resourcePrefabs, NativeParallelMultiHashMap<Entity, (Entity, Entity)> multiHashMap)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < datas.Length; i++)
		{
			IndustrialProcessData industrialProcessData = datas[i];
			Entity val = resourcePrefabs[industrialProcessData.m_Output.m_Resource];
			if (val != Entity.Null)
			{
				(Entity, Entity) value = (Entity.Null, Entity.Null);
				if (industrialProcessData.m_Input1.m_Resource != Resource.NoResource && industrialProcessData.m_Input1.m_Resource != industrialProcessData.m_Output.m_Resource)
				{
					value.Item1 = resourcePrefabs[industrialProcessData.m_Input1.m_Resource];
				}
				if (industrialProcessData.m_Input2.m_Resource != Resource.NoResource && industrialProcessData.m_Input2.m_Resource != industrialProcessData.m_Output.m_Resource)
				{
					value.Item2 = resourcePrefabs[industrialProcessData.m_Input2.m_Resource];
				}
				if (value.Item1 != Entity.Null || value.Item2 != Entity.Null)
				{
					TryAddUniqueValue(multiHashMap, val, value);
				}
			}
		}
	}

	private static void TryAddUniqueValue(NativeParallelMultiHashMap<Entity, (Entity, Entity)> multiHashMap, Entity key, (Entity, Entity) value)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<Entity, (Entity, Entity)> enumerator = multiHashMap.GetValuesForKey(key).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				(Entity, Entity) current = enumerator.Current;
				if (current.Item1 == value.Item1 && current.Item2 == value.Item2)
				{
					return;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		multiHashMap.Add(key, value);
	}

	[Preserve]
	public ProductionUISystem()
	{
	}
}
