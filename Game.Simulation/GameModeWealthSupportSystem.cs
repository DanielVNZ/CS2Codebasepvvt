using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Citizens;
using Game.Common;
using Game.Economy;
using Game.Prefabs.Modes;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class GameModeWealthSupportSystem : GameSystemBase
{
	[BurstCompile]
	private struct SupportWageJob : IJobChunk
	{
		public uint m_UpdateFrameIndex;

		public int m_MinimumWealth;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public ComponentTypeHandle<Household> m_HouseholdType;

		public BufferTypeHandle<Resources> m_ResourcesType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			if (m_UpdateFrameIndex != ((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index)
			{
				return;
			}
			NativeArray<Household> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Household>(ref m_HouseholdType);
			BufferAccessor<Resources> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourcesType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Household householdData = nativeArray[i];
				DynamicBuffer<Resources> resources = bufferAccessor[i];
				int householdTotalWealth = EconomyUtils.GetHouseholdTotalWealth(householdData, resources);
				if (householdTotalWealth <= m_MinimumWealth)
				{
					EconomyUtils.AddResources(Resource.Money, m_MinimumWealth - householdTotalWealth, resources);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	public static readonly int kUpdatesPerDay = 32;

	private SimulationSystem m_SimulationSystem;

	private EntityQuery m_GameModeSettingQuery;

	private EntityQuery m_HouseholdGroup;

	private int m_MinimumWealth;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (kUpdatesPerDay * 16);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_GameModeSettingQuery = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ModeSettingData>() });
		m_HouseholdGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<Household>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadOnly<Resources>(),
			ComponentType.ReadOnly<HouseholdCitizen>(),
			ComponentType.Exclude<TouristHousehold>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_GameModeSettingQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_HouseholdGroup);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, kUpdatesPerDay, 16);
		SupportWageJob supportWageJob = new SupportWageJob
		{
			m_UpdateFrameIndex = updateFrame,
			m_MinimumWealth = m_MinimumWealth,
			m_UpdateFrameType = ((ComponentSystemBase)this).GetSharedComponentTypeHandle<UpdateFrame>(),
			m_HouseholdType = ((ComponentSystemBase)this).GetComponentTypeHandle<Household>(true),
			m_ResourcesType = ((ComponentSystemBase)this).GetBufferTypeHandle<Resources>(false)
		};
		((SystemBase)this).Dependency = JobChunkExtensions.Schedule<SupportWageJob>(supportWageJob, m_HouseholdGroup, ((SystemBase)this).Dependency);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		if (((EntityQuery)(ref m_GameModeSettingQuery)).IsEmptyIgnoreFilter)
		{
			((ComponentSystemBase)this).Enabled = false;
			return;
		}
		ModeSettingData singleton = ((EntityQuery)(ref m_GameModeSettingQuery)).GetSingleton<ModeSettingData>();
		if (singleton.m_Enable && singleton.m_SupportPoorCitizens)
		{
			m_MinimumWealth = singleton.m_MinimumWealth;
			((ComponentSystemBase)this).Enabled = true;
		}
		else
		{
			((ComponentSystemBase)this).Enabled = false;
		}
	}

	[Preserve]
	public GameModeWealthSupportSystem()
	{
	}
}
