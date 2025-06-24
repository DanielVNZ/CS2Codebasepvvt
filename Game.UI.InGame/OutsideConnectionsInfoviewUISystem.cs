using System;
using System.Collections.Generic;
using Colossal.UI.Binding;
using Game.Economy;
using Game.Prefabs;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

public class OutsideConnectionsInfoviewUISystem : InfoviewUISystemBase
{
	public struct TopResource : IComparable<TopResource>
	{
		public string id;

		public int amount;

		public Color color;

		public TopResource(string id, int amount, Color color)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			this.id = id;
			this.amount = amount;
			this.color = color;
		}

		public int CompareTo(TopResource other)
		{
			int num = other.amount - amount;
			if (num == 0)
			{
				return string.Compare(id, other.id, StringComparison.Ordinal);
			}
			return num;
		}
	}

	private const string kGroup = "outsideInfo";

	private CommercialDemandSystem m_CommercialDemandSystem;

	private IndustrialDemandSystem m_IndustrialDemandSystem;

	private CountCompanyDataSystem m_CountCompanyDataSystem;

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_ResourceQuery;

	private RawValueBinding m_TopImportNames;

	private RawValueBinding m_TopExportNames;

	private RawValueBinding m_TopImportColors;

	private RawValueBinding m_TopExportColors;

	private RawValueBinding m_TopImportData;

	private RawValueBinding m_TopExportData;

	private List<TopResource> m_TopImports;

	private List<TopResource> m_TopExports;

	protected override bool Active
	{
		get
		{
			if (!base.Active && !((EventBindingBase)m_TopImportNames).active && !((EventBindingBase)m_TopImportColors).active && !((EventBindingBase)m_TopImportData).active && !((EventBindingBase)m_TopExportNames).active && !((EventBindingBase)m_TopExportColors).active)
			{
				return ((EventBindingBase)m_TopExportData).active;
			}
			return true;
		}
	}

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
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_00c4: Expected O, but got Unknown
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_00ee: Expected O, but got Unknown
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_0118: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Expected O, but got Unknown
		//IL_0142: Expected O, but got Unknown
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		//IL_016c: Expected O, but got Unknown
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Expected O, but got Unknown
		//IL_0196: Expected O, but got Unknown
		base.OnCreate();
		m_CommercialDemandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CommercialDemandSystem>();
		m_IndustrialDemandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IndustrialDemandSystem>();
		m_CountCompanyDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountCompanyDataSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ResourceQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ResourceData>(),
			ComponentType.ReadOnly<TaxableResourceData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		m_TopImports = new List<TopResource>(42);
		m_TopExports = new List<TopResource>(42);
		UpdateCache();
		RawValueBinding val = new RawValueBinding("outsideInfo", "topImportNames", (Action<IJsonWriter>)UpdateImportNames);
		RawValueBinding binding = val;
		m_TopImportNames = val;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val2 = new RawValueBinding("outsideInfo", "topImportColors", (Action<IJsonWriter>)UpdateImportColors);
		binding = val2;
		m_TopImportColors = val2;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val3 = new RawValueBinding("outsideInfo", "topImportData", (Action<IJsonWriter>)UpdateImportData);
		binding = val3;
		m_TopImportData = val3;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val4 = new RawValueBinding("outsideInfo", "topExportNames", (Action<IJsonWriter>)UpdateExportNames);
		binding = val4;
		m_TopExportNames = val4;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val5 = new RawValueBinding("outsideInfo", "topExportColors", (Action<IJsonWriter>)UpdateExportColors);
		binding = val5;
		m_TopExportColors = val5;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val6 = new RawValueBinding("outsideInfo", "topExportData", (Action<IJsonWriter>)UpdateExportData);
		binding = val6;
		m_TopExportData = val6;
		AddBinding((IBinding)(object)binding);
	}

	protected override void PerformUpdate()
	{
		UpdateCache();
		m_TopImportNames.Update();
		m_TopImportColors.Update();
		m_TopImportData.Update();
		m_TopExportNames.Update();
		m_TopExportColors.Update();
		m_TopExportData.Update();
	}

	private void UpdateCache()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_ResourceQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<PrefabData> val2 = ((EntityQuery)(ref m_ResourceQuery)).ToComponentDataArray<PrefabData>(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle deps;
		NativeArray<int> production = m_CountCompanyDataSystem.GetProduction(out deps);
		JobHandle deps2;
		NativeArray<int> consumption = m_IndustrialDemandSystem.GetConsumption(out deps2);
		JobHandle deps3;
		NativeArray<int> consumption2 = m_CommercialDemandSystem.GetConsumption(out deps3);
		JobHandle.CompleteAll(ref deps, ref deps2, ref deps3);
		m_TopImports.Clear();
		m_TopExports.Clear();
		try
		{
			for (int i = 0; i < val.Length; i++)
			{
				ResourcePrefab prefab = m_PrefabSystem.GetPrefab<ResourcePrefab>(val2[i]);
				int resourceIndex = EconomyUtils.GetResourceIndex(EconomyUtils.GetResource(prefab.m_Resource));
				int num = production[resourceIndex];
				int num2 = consumption[resourceIndex];
				int num3 = consumption2[resourceIndex];
				int num4 = math.min(num2 + num3, num);
				int num5 = math.min(num2, num4);
				int num6 = num2 - num5;
				int num7 = math.min(num3, num4 - num5);
				int amount = num3 - num7 + num6;
				int amount2 = num - num4;
				m_TopImports.Add(new TopResource(((Object)prefab).name, amount, prefab.m_Color));
				m_TopExports.Add(new TopResource(((Object)prefab).name, amount2, prefab.m_Color));
			}
			m_TopImports.Sort();
			m_TopExports.Sort();
		}
		finally
		{
			val.Dispose();
			val2.Dispose();
		}
	}

	private void UpdateImportNames(IJsonWriter binder)
	{
		int num = 10;
		if (m_TopImports.Count < num)
		{
			num = m_TopImports.Count;
		}
		JsonWriterExtensions.ArrayBegin(binder, num);
		for (int i = 0; i < num; i++)
		{
			binder.Write(m_TopImports[i].id);
		}
		binder.ArrayEnd();
	}

	private void UpdateImportColors(IJsonWriter binder)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		int num = 10;
		if (m_TopImports.Count < num)
		{
			num = m_TopImports.Count;
		}
		JsonWriterExtensions.ArrayBegin(binder, num);
		for (int i = 0; i < num; i++)
		{
			binder.Write(m_TopImports[i].color.ToHexCode());
		}
		binder.ArrayEnd();
	}

	private void UpdateImportData(IJsonWriter binder)
	{
		int num = 0;
		int num2 = 10;
		if (m_TopImports.Count < num2)
		{
			num2 = m_TopImports.Count;
		}
		binder.TypeBegin("infoviews.ChartData");
		binder.PropertyName("values");
		JsonWriterExtensions.ArrayBegin(binder, num2);
		for (int i = 0; i < num2; i++)
		{
			binder.Write(m_TopImports[i].amount);
			num += m_TopImports[i].amount;
		}
		binder.ArrayEnd();
		binder.PropertyName("total");
		binder.Write(num);
		binder.TypeEnd();
	}

	private void UpdateExportNames(IJsonWriter binder)
	{
		int num = 10;
		if (m_TopExports.Count < num)
		{
			num = m_TopExports.Count;
		}
		JsonWriterExtensions.ArrayBegin(binder, num);
		for (int i = 0; i < num; i++)
		{
			binder.Write(m_TopExports[i].id);
		}
		binder.ArrayEnd();
	}

	private void UpdateExportColors(IJsonWriter binder)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		int num = 10;
		if (m_TopExports.Count < num)
		{
			num = m_TopExports.Count;
		}
		JsonWriterExtensions.ArrayBegin(binder, num);
		for (int i = 0; i < num; i++)
		{
			binder.Write(m_TopExports[i].color.ToHexCode());
		}
		binder.ArrayEnd();
	}

	private void UpdateExportData(IJsonWriter binder)
	{
		int num = 0;
		int num2 = 10;
		if (m_TopExports.Count < num2)
		{
			num2 = m_TopExports.Count;
		}
		binder.TypeBegin("infoviews.ChartData");
		binder.PropertyName("values");
		JsonWriterExtensions.ArrayBegin(binder, num2);
		for (int i = 0; i < num2; i++)
		{
			binder.Write(m_TopExports[i].amount);
			num += m_TopExports[i].amount;
		}
		binder.ArrayEnd();
		binder.PropertyName("total");
		binder.Write(num);
		binder.TypeEnd();
	}

	[Preserve]
	public OutsideConnectionsInfoviewUISystem()
	{
	}
}
