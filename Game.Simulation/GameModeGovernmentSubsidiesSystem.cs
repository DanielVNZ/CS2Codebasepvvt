using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.City;
using Game.Prefabs.Modes;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class GameModeGovernmentSubsidiesSystem : GameSystemBase
{
	public static readonly int kUpdatesPerDay = 128;

	private int m_LastSubsidyCoverPerDay;

	private int m_MonthlySubsidy;

	private ICityServiceBudgetSystem m_CityServiceBudgetSystem;

	private CitySystem m_CitySystem;

	private EntityQuery m_GameModeSettingQuery;

	public int LastSubsidyCoverPerDay => m_LastSubsidyCoverPerDay;

	public int monthlySubsidy => m_MonthlySubsidy;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / kUpdatesPerDay;
	}

	public bool GetGovernmentSubsidiesEnabled()
	{
		return ((ComponentSystemBase)this).Enabled;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CityServiceBudgetSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityServiceBudgetSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_GameModeSettingQuery = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ModeSettingData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_GameModeSettingQuery);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		m_MonthlySubsidy = 0;
		if (((EntityQuery)(ref m_GameModeSettingQuery)).IsEmptyIgnoreFilter)
		{
			((ComponentSystemBase)this).Enabled = false;
			return;
		}
		ModeSettingData singleton = ((EntityQuery)(ref m_GameModeSettingQuery)).GetSingleton<ModeSettingData>();
		if (singleton.m_Enable && singleton.m_EnableGovernmentSubsidies)
		{
			((ComponentSystemBase)this).Enabled = true;
		}
		else
		{
			((ComponentSystemBase)this).Enabled = false;
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		m_MonthlySubsidy = 0;
		if (((EntityQuery)(ref m_GameModeSettingQuery)).IsEmptyIgnoreFilter || m_CitySystem.City == Entity.Null)
		{
			return;
		}
		ModeSettingData singleton = ((EntityQuery)(ref m_GameModeSettingQuery)).GetSingleton<ModeSettingData>();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PlayerMoney componentData = ((EntityManager)(ref entityManager)).GetComponentData<PlayerMoney>(m_CitySystem.City);
		if (componentData.money < singleton.m_MoneyCoverThreshold.x)
		{
			float num = singleton.m_MoneyCoverThreshold.x - singleton.m_MoneyCoverThreshold.y;
			float num2 = math.clamp(1f - (float)(componentData.money - singleton.m_MoneyCoverThreshold.y) / num, 0f, 1f) * ((float)singleton.m_MaxMoneyCoverPercentage / 100f);
			if (num2 > 0f)
			{
				m_MonthlySubsidy = math.abs((int)(num2 * (float)m_CityServiceBudgetSystem.GetTotalExpenses()));
				m_LastSubsidyCoverPerDay = m_MonthlySubsidy / kUpdatesPerDay;
			}
		}
	}

	[Preserve]
	public GameModeGovernmentSubsidiesSystem()
	{
	}
}
