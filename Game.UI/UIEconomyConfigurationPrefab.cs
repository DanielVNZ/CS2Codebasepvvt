using System;
using System.Collections.Generic;
using Game.City;
using Game.Prefabs;
using Unity.Entities;

namespace Game.UI;

[ComponentMenu("Settings/", new Type[] { })]
public class UIEconomyConfigurationPrefab : PrefabBase
{
	public BudgetItem<IncomeSource>[] m_IncomeItems;

	public BudgetItem<ExpenseSource>[] m_ExpenseItems;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<UIEconomyConfigurationData>());
	}
}
