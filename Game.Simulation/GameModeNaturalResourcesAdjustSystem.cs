using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Prefabs.Modes;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class GameModeNaturalResourcesAdjustSystem : GameSystemBase
{
	[BurstCompile]
	private struct BoostInitialNaturalResourcesJob : IJobParallelFor
	{
		public CellMapData<NaturalResourceCell> m_CellData;

		public float m_BoostMultiplier;

		public void Execute(int index)
		{
			NaturalResourceCell naturalResourceCell = m_CellData.m_Buffer[index];
			naturalResourceCell.m_Fertility.m_Base = (ushort)math.min((int)((float)(int)naturalResourceCell.m_Fertility.m_Base * m_BoostMultiplier), 65535);
			naturalResourceCell.m_Ore.m_Base = (ushort)math.min((int)((float)(int)naturalResourceCell.m_Ore.m_Base * m_BoostMultiplier), 65535);
			naturalResourceCell.m_Oil.m_Base = (ushort)math.min((int)((float)(int)naturalResourceCell.m_Oil.m_Base * m_BoostMultiplier), 65535);
			m_CellData.m_Buffer[index] = naturalResourceCell;
		}
	}

	[BurstCompile]
	private struct BoostInitialGroundWaterJob : IJobParallelFor
	{
		public CellMapData<GroundWater> m_CellData;

		public float m_BoostMultiplier;

		public void Execute(int index)
		{
			GroundWater groundWater = m_CellData.m_Buffer[index];
			groundWater.m_Amount = (short)math.min((int)((float)groundWater.m_Amount * m_BoostMultiplier), 32767);
			groundWater.m_Polluted = (short)math.min((int)((float)groundWater.m_Polluted * m_BoostMultiplier), 32767);
			groundWater.m_Max = (short)math.min((int)((float)groundWater.m_Max * m_BoostMultiplier), 32767);
			m_CellData.m_Buffer[index] = groundWater;
		}
	}

	[BurstCompile]
	private struct RefillNaturalResourcesJob : IJobParallelFor
	{
		public CellMapData<NaturalResourceCell> m_CellData;

		public ModeSettingData m_GlobalData;

		public void Execute(int index)
		{
			NaturalResourceCell naturalResourceCell = m_CellData.m_Buffer[index];
			naturalResourceCell.m_Oil.m_Used = (ushort)math.max(0f, (float)(int)naturalResourceCell.m_Oil.m_Used - (float)(int)naturalResourceCell.m_Oil.m_Base * ((float)m_GlobalData.m_PercentOilRefillAmountPerDay / 100f) / (float)kUpdatesPerDay);
			naturalResourceCell.m_Ore.m_Used = (ushort)math.max(0f, (float)(int)naturalResourceCell.m_Ore.m_Used - (float)(int)naturalResourceCell.m_Ore.m_Base * ((float)m_GlobalData.m_PercentOreRefillAmountPerDay / 100f) / (float)kUpdatesPerDay);
			m_CellData.m_Buffer[index] = naturalResourceCell;
		}
	}

	public static readonly int kUpdatesPerDay = 128;

	private NaturalResourceSystem m_NaturalResourceSystem;

	private GroundWaterSystem m_GroundWaterSystem;

	private EntityQuery m_GameModeSettingQuery;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / kUpdatesPerDay;
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
		m_NaturalResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NaturalResourceSystem>();
		m_GroundWaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundWaterSystem>();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_GameModeSettingQuery = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ModeSettingData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_GameModeSettingQuery);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I4
		base.OnGameLoaded(serializationContext);
		if (((EntityQuery)(ref m_GameModeSettingQuery)).IsEmptyIgnoreFilter)
		{
			((ComponentSystemBase)this).Enabled = false;
			return;
		}
		ModeSettingData singleton = ((EntityQuery)(ref m_GameModeSettingQuery)).GetSingleton<ModeSettingData>();
		if (singleton.m_Enable && singleton.m_EnableAdjustNaturalResources)
		{
			if ((int)((Context)(ref serializationContext)).purpose == 1)
			{
				BoostStartGameNaturalResources(singleton.m_InitialNaturalResourceBoostMultiplier);
			}
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
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_GameModeSettingQuery)).IsEmptyIgnoreFilter)
		{
			ModeSettingData singleton = ((EntityQuery)(ref m_GameModeSettingQuery)).GetSingleton<ModeSettingData>();
			if (singleton.m_Enable && singleton.m_EnableAdjustNaturalResources)
			{
				JobHandle dependencies;
				CellMapData<NaturalResourceCell> data = m_NaturalResourceSystem.GetData(readOnly: false, out dependencies);
				JobHandle jobHandle = IJobParallelForExtensions.Schedule<RefillNaturalResourcesJob>(new RefillNaturalResourcesJob
				{
					m_CellData = data,
					m_GlobalData = singleton
				}, data.m_TextureSize.x * data.m_TextureSize.y, 64, dependencies);
				m_NaturalResourceSystem.AddWriter(jobHandle);
			}
		}
	}

	private void BoostStartGameNaturalResources(float boostMultiplier)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		CellMapData<NaturalResourceCell> data = m_NaturalResourceSystem.GetData(readOnly: false, out dependencies);
		JobHandle jobHandle = IJobParallelForExtensions.Schedule<BoostInitialNaturalResourcesJob>(new BoostInitialNaturalResourcesJob
		{
			m_CellData = data,
			m_BoostMultiplier = boostMultiplier
		}, data.m_TextureSize.x * data.m_TextureSize.y, 64, dependencies);
		m_NaturalResourceSystem.AddWriter(jobHandle);
		JobHandle dependencies2;
		CellMapData<GroundWater> data2 = m_GroundWaterSystem.GetData(readOnly: false, out dependencies2);
		JobHandle jobHandle2 = IJobParallelForExtensions.Schedule<BoostInitialGroundWaterJob>(new BoostInitialGroundWaterJob
		{
			m_CellData = data2,
			m_BoostMultiplier = boostMultiplier
		}, data2.m_TextureSize.x * data2.m_TextureSize.y, 64, dependencies2);
		m_GroundWaterSystem.AddWriter(jobHandle2);
	}

	[Preserve]
	public GameModeNaturalResourcesAdjustSystem()
	{
	}
}
