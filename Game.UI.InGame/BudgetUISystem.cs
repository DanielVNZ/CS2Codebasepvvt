using System;
using System.Collections.Generic;
using Colossal.UI.Binding;
using Game.City;
using Game.Prefabs;
using Game.Simulation;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

public class BudgetUISystem : UISystemBase
{
	private const string kGroup = "budget";

	private PrefabSystem m_PrefabSystem;

	private GameModeGovernmentSubsidiesSystem m_GovernmentSubsidiesSystem;

	private ICityServiceBudgetSystem m_CityServiceBudgetSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private MapTilePurchaseSystem m_MapTilePurchaseSystem;

	private EntityQuery m_ConfigQuery;

	private GetterValueBinding<int> m_TotalIncomeBinding;

	private GetterValueBinding<int> m_TotalExpensesBinding;

	private RawValueBinding m_IncomeItemsBinding;

	private RawValueBinding m_IncomeValuesBinding;

	private RawValueBinding m_ExpenseItemsBinding;

	private RawValueBinding m_ExpenseValuesBinding;

	private Dictionary<string, Func<bool>> m_BudgetsActivations;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		//IL_014f: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Expected O, but got Unknown
		//IL_0179: Expected O, but got Unknown
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Expected O, but got Unknown
		//IL_01a3: Expected O, but got Unknown
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Expected O, but got Unknown
		//IL_01cd: Expected O, but got Unknown
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_CityServiceBudgetSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityServiceBudgetSystem>();
		m_GovernmentSubsidiesSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GameModeGovernmentSubsidiesSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_MapTilePurchaseSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MapTilePurchaseSystem>();
		m_ConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<UIEconomyConfigurationData>() });
		m_BudgetsActivations = new Dictionary<string, Func<bool>>
		{
			{ "Government", m_GovernmentSubsidiesSystem.GetGovernmentSubsidiesEnabled },
			{
				"Loan Interest",
				() => !m_CityConfigurationSystem.unlimitedMoney
			},
			{ "Tile Upkeep", m_MapTilePurchaseSystem.GetMapTileUpkeepEnabled }
		};
		AddBinding((IBinding)(object)(m_TotalIncomeBinding = new GetterValueBinding<int>("budget", "totalIncome", (Func<int>)(() => m_CityServiceBudgetSystem.GetTotalIncome()), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_TotalExpensesBinding = new GetterValueBinding<int>("budget", "totalExpenses", (Func<int>)(() => m_CityServiceBudgetSystem.GetTotalExpenses()), (IWriter<int>)null, (EqualityComparer<int>)null)));
		RawValueBinding val = new RawValueBinding("budget", "incomeItems", (Action<IJsonWriter>)BindIncomeItems);
		RawValueBinding binding = val;
		m_IncomeItemsBinding = val;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val2 = new RawValueBinding("budget", "incomeValues", (Action<IJsonWriter>)BindIncomeValues);
		binding = val2;
		m_IncomeValuesBinding = val2;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val3 = new RawValueBinding("budget", "expenseItems", (Action<IJsonWriter>)BindExpenseItems);
		binding = val3;
		m_ExpenseItemsBinding = val3;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val4 = new RawValueBinding("budget", "expenseValues", (Action<IJsonWriter>)BindExpenseValues);
		binding = val4;
		m_ExpenseValuesBinding = val4;
		AddBinding((IBinding)(object)binding);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		m_TotalIncomeBinding.Update();
		m_TotalExpensesBinding.Update();
		m_IncomeValuesBinding.Update();
		m_ExpenseValuesBinding.Update();
	}

	private void BindIncomeItems(IJsonWriter writer)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		UIEconomyConfigurationPrefab config = GetConfig();
		JsonWriterExtensions.ArrayBegin(writer, config.m_IncomeItems.Length);
		BudgetItem<IncomeSource>[] incomeItems = config.m_IncomeItems;
		foreach (BudgetItem<IncomeSource> budgetItem in incomeItems)
		{
			writer.TypeBegin("Game.UI.InGame.BudgetItem");
			writer.PropertyName("id");
			writer.Write(budgetItem.m_ID);
			writer.PropertyName("color");
			UnityWriters.Write(writer, budgetItem.m_Color);
			writer.PropertyName("icon");
			writer.Write(budgetItem.m_Icon);
			writer.PropertyName("active");
			writer.Write(!m_BudgetsActivations.ContainsKey(budgetItem.m_ID) || m_BudgetsActivations[budgetItem.m_ID]());
			writer.PropertyName("sources");
			JsonWriterExtensions.ArrayBegin(writer, budgetItem.m_Sources.Length);
			IncomeSource[] sources = budgetItem.m_Sources;
			foreach (IncomeSource incomeSource in sources)
			{
				writer.TypeBegin("Game.UI.InGame.BudgetSource");
				writer.PropertyName("id");
				writer.Write(Enum.GetName(typeof(IncomeSource), incomeSource));
				writer.PropertyName("index");
				writer.Write((int)incomeSource);
				writer.TypeEnd();
			}
			writer.ArrayEnd();
			writer.TypeEnd();
		}
		writer.ArrayEnd();
	}

	private void BindIncomeValues(IJsonWriter writer)
	{
		writer.ArrayBegin(14u);
		for (int i = 0; i < 14; i++)
		{
			writer.Write(m_CityServiceBudgetSystem.GetIncome((IncomeSource)i));
		}
		writer.ArrayEnd();
	}

	private void BindExpenseItems(IJsonWriter writer)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		UIEconomyConfigurationPrefab config = GetConfig();
		JsonWriterExtensions.ArrayBegin(writer, config.m_ExpenseItems.Length);
		BudgetItem<ExpenseSource>[] expenseItems = config.m_ExpenseItems;
		foreach (BudgetItem<ExpenseSource> budgetItem in expenseItems)
		{
			writer.TypeBegin("Game.UI.InGame.BudgetItem");
			writer.PropertyName("id");
			writer.Write(budgetItem.m_ID);
			writer.PropertyName("color");
			UnityWriters.Write(writer, budgetItem.m_Color);
			writer.PropertyName("icon");
			writer.Write(budgetItem.m_Icon);
			writer.PropertyName("active");
			writer.Write(!m_BudgetsActivations.ContainsKey(budgetItem.m_ID) || m_BudgetsActivations[budgetItem.m_ID]());
			writer.PropertyName("sources");
			JsonWriterExtensions.ArrayBegin(writer, budgetItem.m_Sources.Length);
			ExpenseSource[] sources = budgetItem.m_Sources;
			foreach (ExpenseSource expenseSource in sources)
			{
				writer.TypeBegin("Game.UI.InGame.BudgetSource");
				writer.PropertyName("id");
				writer.Write(Enum.GetName(typeof(ExpenseSource), expenseSource));
				writer.PropertyName("index");
				writer.Write((int)expenseSource);
				writer.TypeEnd();
			}
			writer.ArrayEnd();
			writer.TypeEnd();
		}
		writer.ArrayEnd();
	}

	private void BindExpenseValues(IJsonWriter writer)
	{
		writer.ArrayBegin(15u);
		for (int i = 0; i < 15; i++)
		{
			writer.Write(-m_CityServiceBudgetSystem.GetExpense((ExpenseSource)i));
		}
		writer.ArrayEnd();
	}

	private UIEconomyConfigurationPrefab GetConfig()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return m_PrefabSystem.GetSingletonPrefab<UIEconomyConfigurationPrefab>(m_ConfigQuery);
	}

	[Preserve]
	public BudgetUISystem()
	{
	}
}
