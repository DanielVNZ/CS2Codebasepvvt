using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.City;
using Game.Prefabs;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class ServiceBudgetUISystem : UISystemBase
{
	private struct ServiceInfo : IJsonWritable
	{
		public Entity entity;

		public string name;

		public string icon;

		public bool locked;

		public int budget;

		public void Write(IJsonWriter writer)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			writer.TypeBegin("serviceBudget.Service");
			writer.PropertyName("entity");
			UnityWriters.Write(writer, entity);
			writer.PropertyName("name");
			writer.Write(name);
			writer.PropertyName("icon");
			writer.Write(icon);
			writer.PropertyName("locked");
			writer.Write(locked);
			writer.PropertyName("budget");
			writer.Write(budget);
			writer.TypeEnd();
		}
	}

	private class PlayerResourceReader : IReader<PlayerResource>
	{
		public void Read(IJsonReader reader, out PlayerResource value)
		{
			int num = default(int);
			reader.Read(ref num);
			value = (PlayerResource)num;
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct TypeHandle
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
		}
	}

	private const string kGroup = "serviceBudget";

	private PrefabSystem m_PrefabSystem;

	private CitySystem m_CitySystem;

	private ICityServiceBudgetSystem m_CityServiceBudgetSystem;

	private IServiceFeeSystem m_ServiceFeeSystem;

	private EntityQuery m_ServiceQuery;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_2035132663_0;

	private EntityQuery __query_2035132663_1;

	private EntityQuery __query_2035132663_2;

	private EntityQuery __query_2035132663_3;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_CityServiceBudgetSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityServiceBudgetSystem>();
		m_ServiceFeeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ServiceFeeSystem>();
		m_ServiceQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<UIObjectData>(),
			ComponentType.ReadOnly<ServiceData>()
		});
		((ComponentSystemBase)this).RequireForUpdate<ServiceFeeParameterData>();
		((ComponentSystemBase)this).RequireForUpdate<OutsideTradeParameterData>();
		((ComponentSystemBase)this).RequireForUpdate<CitizenHappinessParameterData>();
		((ComponentSystemBase)this).RequireForUpdate<BuildingEfficiencyParameterData>();
		AddUpdateBinding((IUpdateBinding)new RawValueBinding("serviceBudget", "services", (Action<IJsonWriter>)WriteServices));
		AddUpdateBinding((IUpdateBinding)(object)new RawMapBinding<Entity>("serviceBudget", "serviceDetails", (Action<IJsonWriter, Entity>)WriteServiceDetails, (IReader<Entity>)null, (IWriter<Entity>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity, int>("serviceBudget", "setServiceBudget", (Action<Entity, int>)SetServiceBudget, (IReader<Entity>)null, (IReader<int>)null));
		AddBinding((IBinding)(object)new TriggerBinding<PlayerResource, float>("serviceBudget", "setServiceFee", (Action<PlayerResource, float>)SetServiceFee, (IReader<PlayerResource>)new PlayerResourceReader(), (IReader<float>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity>("serviceBudget", "resetService", (Action<Entity>)ResetService, (IReader<Entity>)null));
	}

	private void WriteServices(IJsonWriter writer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		NativeList<UIObjectInfo> sortedObjects = UIObjectInfo.GetSortedObjects(m_ServiceQuery, (Allocator)2);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<ServiceFee> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ServiceFee>(m_CitySystem.City, true);
		JsonWriterExtensions.ArrayBegin(writer, sortedObjects.Length);
		Enumerator<UIObjectInfo> enumerator = sortedObjects.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				UIObjectInfo current = enumerator.Current;
				ServicePrefab prefab = m_PrefabSystem.GetPrefab<ServicePrefab>(current.prefabData);
				int totalBudget = GetTotalBudget(current.entity, buffer);
				JsonWriterExtensions.Write<ServiceInfo>(writer, new ServiceInfo
				{
					entity = current.entity,
					name = ((Object)prefab).name,
					icon = ImageSystem.GetIcon(prefab),
					locked = EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, current.entity),
					budget = totalBudget
				});
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		writer.ArrayEnd();
		sortedObjects.Dispose();
	}

	private int GetTotalBudget(Entity service, DynamicBuffer<ServiceFee> fees)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		m_CityServiceBudgetSystem.GetEstimatedServiceBudget(service, out var upkeep);
		int num = -upkeep;
		DynamicBuffer<CollectedCityServiceFeeData> val = default(DynamicBuffer<CollectedCityServiceFeeData>);
		if (EntitiesExtensions.TryGetBuffer<CollectedCityServiceFeeData>(((ComponentSystemBase)this).EntityManager, service, true, ref val))
		{
			Enumerator<CollectedCityServiceFeeData> enumerator = val.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					PlayerResource playerResource = (PlayerResource)enumerator.Current.m_PlayerResource;
					float fee;
					int num2 = (ServiceFeeSystem.TryGetFee(playerResource, fees, out fee) ? m_ServiceFeeSystem.GetServiceFeeIncomeEstimate(playerResource, fee) : m_ServiceFeeSystem.GetServiceFees(playerResource).x);
					int3 serviceFees = m_ServiceFeeSystem.GetServiceFees(playerResource);
					num += num2;
					num += serviceFees.y;
					num -= serviceFees.z;
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}
		return num;
	}

	private void WriteServiceDetails(IJsonWriter writer, Entity serviceEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		ServiceData serviceData = default(ServiceData);
		PrefabData prefabData = default(PrefabData);
		if (EntitiesExtensions.TryGetComponent<ServiceData>(((ComponentSystemBase)this).EntityManager, serviceEntity, ref serviceData) && EntitiesExtensions.TryGetComponent<PrefabData>(((ComponentSystemBase)this).EntityManager, serviceEntity, ref prefabData))
		{
			ServicePrefab prefab = m_PrefabSystem.GetPrefab<ServicePrefab>(prefabData);
			m_CityServiceBudgetSystem.GetEstimatedServiceBudget(serviceEntity, out var upkeep);
			writer.TypeBegin("serviceBudget.ServiceDetails");
			writer.PropertyName("entity");
			UnityWriters.Write(writer, serviceEntity);
			writer.PropertyName("name");
			writer.Write(((Object)prefab).name);
			writer.PropertyName("icon");
			writer.Write(ImageSystem.GetIcon(prefab));
			writer.PropertyName("locked");
			writer.Write(EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, serviceEntity));
			writer.PropertyName("budgetAdjustable");
			writer.Write(serviceData.m_BudgetAdjustable);
			int serviceBudget = m_CityServiceBudgetSystem.GetServiceBudget(serviceEntity);
			writer.PropertyName("budgetPercentage");
			writer.Write(serviceBudget);
			writer.PropertyName("efficiency");
			writer.Write(m_CityServiceBudgetSystem.GetServiceEfficiency(serviceEntity, serviceBudget));
			writer.PropertyName("upkeep");
			writer.Write(-upkeep);
			writer.PropertyName("fees");
			WriteServiceFees(writer, serviceEntity);
			writer.TypeEnd();
		}
		else
		{
			writer.WriteNull();
		}
	}

	private void WriteServiceFees(IJsonWriter writer, Entity serviceEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<CollectedCityServiceFeeData> val = default(DynamicBuffer<CollectedCityServiceFeeData>);
		if (EntitiesExtensions.TryGetBuffer<CollectedCityServiceFeeData>(((ComponentSystemBase)this).EntityManager, serviceEntity, true, ref val) && val.Length > 0)
		{
			ServiceFeeParameterData feeParameters = ((EntityQuery)(ref __query_2035132663_0)).GetSingleton<ServiceFeeParameterData>();
			OutsideTradeParameterData singleton = ((EntityQuery)(ref __query_2035132663_1)).GetSingleton<OutsideTradeParameterData>();
			CitizenHappinessParameterData happinessParameters = ((EntityQuery)(ref __query_2035132663_2)).GetSingleton<CitizenHappinessParameterData>();
			BuildingEfficiencyParameterData efficiencyParameters = ((EntityQuery)(ref __query_2035132663_3)).GetSingleton<BuildingEfficiencyParameterData>();
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<ServiceFee> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ServiceFee>(m_CitySystem.City, true);
			JsonWriterExtensions.ArrayBegin(writer, val.Length);
			Enumerator<CollectedCityServiceFeeData> enumerator = val.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					PlayerResource playerResource = (PlayerResource)enumerator.Current.m_PlayerResource;
					FeeParameters feeParameters2 = feeParameters.GetFeeParameters(playerResource);
					float fee;
					int num = (ServiceFeeSystem.TryGetFee(playerResource, buffer, out fee) ? m_ServiceFeeSystem.GetServiceFeeIncomeEstimate(playerResource, fee) : m_ServiceFeeSystem.GetServiceFees(playerResource).x);
					float relativeFee = fee / feeParameters2.m_Default;
					int3 serviceFees = m_ServiceFeeSystem.GetServiceFees(playerResource);
					writer.TypeBegin("serviceBudget.ServiceFee");
					writer.PropertyName("resource");
					writer.Write((int)playerResource);
					writer.PropertyName("name");
					writer.Write(Enum.GetName(typeof(PlayerResource), playerResource));
					writer.PropertyName("fee");
					writer.Write(fee);
					writer.PropertyName("min");
					writer.Write(0);
					writer.PropertyName("max");
					writer.Write(feeParameters2.m_Max);
					writer.PropertyName("adjustable");
					writer.Write(feeParameters2.m_Adjustable);
					writer.PropertyName("importable");
					writer.Write(singleton.Importable(playerResource));
					writer.PropertyName("exportable");
					writer.Write(singleton.Exportable(playerResource));
					writer.PropertyName("incomeInternal");
					writer.Write(num);
					writer.PropertyName("incomeExports");
					writer.Write(serviceFees.y);
					writer.PropertyName("expenseImports");
					writer.Write(-serviceFees.z);
					writer.PropertyName("consumptionMultiplier");
					writer.Write(ServiceFeeSystem.GetConsumptionMultiplier(playerResource, relativeFee, in feeParameters));
					writer.PropertyName("efficiencyMultiplier");
					writer.Write(ServiceFeeSystem.GetEfficiencyMultiplier(playerResource, relativeFee, in efficiencyParameters));
					writer.PropertyName("happinessEffect");
					writer.Write(ServiceFeeSystem.GetHappinessEffect(playerResource, relativeFee, in happinessParameters));
					writer.TypeEnd();
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			writer.ArrayEnd();
		}
		else
		{
			JsonWriterExtensions.WriteEmptyArray(writer);
		}
	}

	private void SetServiceBudget(Entity service, int percentage)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_CityServiceBudgetSystem.SetServiceBudget(service, percentage);
	}

	private void SetServiceFee(PlayerResource resource, float amount)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (resource != PlayerResource.Parking)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<ServiceFee>(m_CitySystem.City))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				DynamicBuffer<ServiceFee> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ServiceFee>(m_CitySystem.City, false);
				ServiceFeeSystem.SetFee(resource, buffer, amount);
			}
		}
	}

	private void ResetService(Entity service)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		SetServiceBudget(service, 100);
		DynamicBuffer<CollectedCityServiceFeeData> val = default(DynamicBuffer<CollectedCityServiceFeeData>);
		if (!EntitiesExtensions.TryGetBuffer<CollectedCityServiceFeeData>(((ComponentSystemBase)this).EntityManager, service, true, ref val))
		{
			return;
		}
		ServiceFeeParameterData singleton = ((EntityQuery)(ref __query_2035132663_0)).GetSingleton<ServiceFeeParameterData>();
		Enumerator<CollectedCityServiceFeeData> enumerator = val.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				PlayerResource playerResource = (PlayerResource)enumerator.Current.m_PlayerResource;
				SetServiceFee(playerResource, singleton.GetFeeParameters(playerResource).m_Default);
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<ServiceFeeParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_2035132663_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<OutsideTradeParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_2035132663_1 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<CitizenHappinessParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_2035132663_2 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<BuildingEfficiencyParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_2035132663_3 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public ServiceBudgetUISystem()
	{
	}
}
