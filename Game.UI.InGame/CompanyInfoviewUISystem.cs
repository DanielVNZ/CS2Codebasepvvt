using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Companies;
using Game.Economy;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class CompanyInfoviewUISystem : InfoviewUISystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
		}
	}

	private const string kGroup = "companyInfoview";

	private ResourceSystem m_ResourceSystem;

	private EntityQuery m_CommercialQuery;

	private EntityQuery m_IndustrialQuery;

	private GetterValueBinding<IndicatorValue> m_CommercialProfitability;

	private GetterValueBinding<IndicatorValue> m_IndustrialProfitability;

	private GetterValueBinding<IndicatorValue> m_OfficeProfitability;

	private TypeHandle __TypeHandle;

	protected override bool Active
	{
		get
		{
			if (!base.Active && !((EventBindingBase)m_CommercialProfitability).active && !((EventBindingBase)m_IndustrialProfitability).active)
			{
				return ((EventBindingBase)m_OfficeProfitability).active;
			}
			return true;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_CommercialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Profitability>(),
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadOnly<CommercialCompany>()
		});
		m_IndustrialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Profitability>(),
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadOnly<IndustrialCompany>()
		});
		AddBinding((IBinding)(object)(m_CommercialProfitability = new GetterValueBinding<IndicatorValue>("companyInfoview", "commercialProfitability", (Func<IndicatorValue>)GetCommercialProfitability, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		AddBinding((IBinding)(object)(m_IndustrialProfitability = new GetterValueBinding<IndicatorValue>("companyInfoview", "industrialProfitability", (Func<IndicatorValue>)GetIndustrialProfitability, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		AddBinding((IBinding)(object)(m_OfficeProfitability = new GetterValueBinding<IndicatorValue>("companyInfoview", "officeProfitability", (Func<IndicatorValue>)GetOfficeProfitability, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
	}

	protected override void PerformUpdate()
	{
		m_CommercialProfitability.Update();
		m_IndustrialProfitability.Update();
		m_OfficeProfitability.Update();
	}

	private IndicatorValue GetCommercialProfitability()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_CommercialQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		float current = 0f;
		try
		{
			int num = 0;
			int num2 = 0;
			Profitability profitability = default(Profitability);
			for (int i = 0; i < val.Length; i++)
			{
				if (EntitiesExtensions.TryGetComponent<Profitability>(((ComponentSystemBase)this).EntityManager, val[i], ref profitability))
				{
					num++;
					num2 += profitability.m_Profitability;
				}
			}
			current = ((num == 0) ? 0f : ((float)num2 / (float)num));
		}
		finally
		{
			val.Dispose();
		}
		return new IndicatorValue(0f, 255f, current);
	}

	private IndicatorValue GetOfficeProfitability()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		ResourcePrefabs prefabs = m_ResourceSystem.GetPrefabs();
		NativeArray<Entity> val = ((EntityQuery)(ref m_IndustrialQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		ComponentLookup<PrefabRef> componentLookup = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ResourceData> datas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<IndustrialProcessData> componentLookup2 = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		float current = 0f;
		try
		{
			int num = 0;
			int num2 = 0;
			Profitability profitability = default(Profitability);
			for (int i = 0; i < val.Length; i++)
			{
				if (EntitiesExtensions.TryGetComponent<Profitability>(((ComponentSystemBase)this).EntityManager, val[i], ref profitability) && componentLookup.HasComponent(val[i]))
				{
					PrefabRef prefabRef = componentLookup[val[i]];
					if (componentLookup2.HasComponent(prefabRef.m_Prefab) && !(Math.Abs(EconomyUtils.GetWeight(componentLookup2[prefabRef.m_Prefab].m_Output.m_Resource, prefabs, ref datas)) > float.Epsilon))
					{
						num++;
						num2 += profitability.m_Profitability;
					}
				}
			}
			current = ((num == 0) ? 0f : ((float)num2 / (float)num));
		}
		finally
		{
			val.Dispose();
		}
		return new IndicatorValue(0f, 255f, current);
	}

	private IndicatorValue GetIndustrialProfitability()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_IndustrialQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		ComponentLookup<PrefabRef> componentLookup = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ResourceData> datas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<IndustrialProcessData> componentLookup2 = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		float current = 0f;
		try
		{
			int num = 0;
			int num2 = 0;
			Profitability profitability = default(Profitability);
			for (int i = 0; i < val.Length; i++)
			{
				if (!EntitiesExtensions.TryGetComponent<Profitability>(((ComponentSystemBase)this).EntityManager, val[i], ref profitability) || !componentLookup.HasComponent(val[i]))
				{
					continue;
				}
				PrefabRef prefabRef = componentLookup[val[i]];
				if (componentLookup2.HasComponent(prefabRef.m_Prefab))
				{
					IndustrialProcessData industrialProcessData = componentLookup2[prefabRef.m_Prefab];
					if (!(Math.Abs(EconomyUtils.GetWeight(prefabs: m_ResourceSystem.GetPrefabs(), r: industrialProcessData.m_Output.m_Resource, datas: ref datas)) < float.Epsilon))
					{
						num++;
						num2 += profitability.m_Profitability;
					}
				}
			}
			current = ((num == 0) ? 0f : ((float)num2 / (float)num));
		}
		finally
		{
			val.Dispose();
		}
		return new IndicatorValue(0f, 255f, current);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public CompanyInfoviewUISystem()
	{
	}
}
