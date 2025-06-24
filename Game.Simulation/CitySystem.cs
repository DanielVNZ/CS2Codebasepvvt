using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.City;
using Game.Companies;
using Game.Economy;
using Game.Policies;
using Game.Prefabs;
using Game.Serialization;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class CitySystem : GameSystemBase, ICitySystem, IDefaultSerializable, ISerializable, IPostDeserialize
{
	private EntityQuery m_ServiceFeeParameterQuery;

	private EntityQuery m_EconomyParameterQuery;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private Entity m_City;

	private int m_Money;

	private int m_XP;

	public Entity City => m_City;

	public int moneyAmount => m_Money;

	public int XP => m_XP;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_ServiceFeeParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ServiceFeeParameterData>() });
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (m_City != Entity.Null)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			m_Money = ((EntityManager)(ref entityManager)).GetComponentData<PlayerMoney>(m_City).money;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			m_XP = ((EntityManager)(ref entityManager)).GetComponentData<XP>(m_City).m_XP;
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_City);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_City);
		m_Money = 0;
	}

	public void SetDefaults(Context context)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_City = Entity.Null;
		m_Money = 0;
	}

	public void PostDeserialize(Context context)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Invalid comparison between Unknown and I4
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Invalid comparison between Unknown and I4
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Invalid comparison between Unknown and I4
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Invalid comparison between Unknown and I4
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Invalid comparison between Unknown and I4
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		EconomyParameterData singleton = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>();
		EntityManager entityManager;
		if ((int)((Context)(ref context)).purpose == 1)
		{
			if (m_City == Entity.Null)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				m_City = ((EntityManager)(ref entityManager)).CreateEntity((ComponentType[])(object)new ComponentType[1] { ComponentType.op_Implicit(typeof(Game.City.City)) });
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponentData<MilestoneLevel>(m_City, new MilestoneLevel
				{
					m_AchievedMilestone = 0
				});
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponentData<XP>(m_City, new XP
				{
					m_XP = 0
				});
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponentData<DevTreePoints>(m_City, new DevTreePoints
				{
					m_Points = 0
				});
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddBuffer<Policy>(m_City);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddBuffer<CityModifier>(m_City);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponentData<Loan>(m_City, default(Loan));
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponentData<Creditworthiness>(m_City, default(Creditworthiness));
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponentData<DangerLevel>(m_City, default(DangerLevel));
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddBuffer<TradeCost>(m_City);
				ServiceFeeParameterData singleton2 = ((EntityQuery)(ref m_ServiceFeeParameterQuery)).GetSingleton<ServiceFeeParameterData>();
				entityManager = ((ComponentSystemBase)this).EntityManager;
				DynamicBuffer<ServiceFee> val = ((EntityManager)(ref entityManager)).AddBuffer<ServiceFee>(m_City);
				foreach (ServiceFee defaultFee in singleton2.GetDefaultFees())
				{
					val.Add(defaultFee);
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponentData<PlayerMoney>(m_City, new PlayerMoney(singleton.m_PlayerStartMoney));
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddBuffer<SpecializationBonus>(m_City);
				Population population = default(Population);
				population.SetDefaults(context);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponentData<Population>(m_City, population);
				Tourism tourism = default(Tourism);
				tourism.SetDefaults(context);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponentData<Tourism>(m_City, tourism);
			}
			else
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PlayerMoney>(m_City, new PlayerMoney(singleton.m_PlayerStartMoney));
			}
		}
		if ((int)((Context)(ref context)).purpose == 2 && ((Context)(ref context)).version < Version.loanComponent)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponentData<Loan>(m_City, default(Loan));
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponentData<Creditworthiness>(m_City, default(Creditworthiness));
		}
		if ((int)((Context)(ref context)).purpose == 1 || (int)((Context)(ref context)).purpose == 2)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			PlayerMoney componentData = ((EntityManager)(ref entityManager)).GetComponentData<PlayerMoney>(m_City);
			componentData.m_Unlimited = m_CityConfigurationSystem.unlimitedMoney;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).SetComponentData<PlayerMoney>(m_City, componentData);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Resources>(m_City))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).RemoveComponent<Resources>(m_City);
			}
		}
		if ((int)((Context)(ref context)).purpose == 2 && ((Context)(ref context)).version < Version.dangerLevel)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponentData<DangerLevel>(m_City, default(DangerLevel));
		}
		if (((Context)(ref context)).version < Version.cityTradeCost && ((int)((Context)(ref context)).purpose == 1 || (int)((Context)(ref context)).purpose == 2))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<TradeCost> costs = ((EntityManager)(ref entityManager)).AddBuffer<TradeCost>(m_City);
			ResourceIterator iterator = ResourceIterator.GetIterator();
			ResourcePrefabs prefabs = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>().GetPrefabs();
			int num = 20000;
			while (iterator.Next())
			{
				Resource resource = iterator.resource;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				float num2 = (float)EconomyUtils.GetTransportCost(10000f, resource, num, ((EntityManager)(ref entityManager)).GetComponentData<ResourceData>(prefabs[iterator.resource]).m_Weight) / (float)num;
				EconomyUtils.SetTradeCost(iterator.resource, new TradeCost
				{
					m_BuyCost = num2,
					m_SellCost = num2,
					m_Resource = iterator.resource
				}, costs, keepLastTime: true);
			}
		}
	}

	[Preserve]
	public CitySystem()
	{
	}
}
