using System.Runtime.CompilerServices;
using Game.City;
using Game.Common;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class MilestoneSystem : GameSystemBase, IMilestoneSystem
{
	private int m_LastRequired;

	private int m_NextRequired;

	private int m_Progress;

	private int m_NextMilestone;

	private CitySystem m_CitySystem;

	private ModificationEndBarrier m_ModificationEndBarrier;

	private EntityArchetype m_UnlockEventArchetype;

	private EntityArchetype m_MilestoneReachedEventArchetype;

	private EntityQuery m_MilestoneLevelGroup;

	private EntityQuery m_XPGroup;

	private EntityQuery m_MilestoneGroup;

	public int currentXP => m_Progress;

	public int requiredXP => nextRequiredXP - math.max(0, lastRequiredXP);

	public int lastRequiredXP => m_LastRequired;

	public int nextRequiredXP => m_NextRequired;

	public float progress => (float)m_Progress / (float)requiredXP;

	public int nextMilestone => m_NextMilestone;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_ModificationEndBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationEndBarrier>();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_UnlockEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<Unlock>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_MilestoneReachedEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<MilestoneReachedEvent>()
		});
		m_MilestoneLevelGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<MilestoneLevel>() });
		m_XPGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<XP>() });
		m_MilestoneGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MilestoneData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_MilestoneLevelGroup);
		((ComponentSystemBase)this).RequireForUpdate(m_XPGroup);
		((ComponentSystemBase)this).RequireForUpdate(m_MilestoneGroup);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		MilestoneLevel singleton = ((EntityQuery)(ref m_MilestoneLevelGroup)).GetSingleton<MilestoneLevel>();
		int achievedMilestone = singleton.m_AchievedMilestone;
		m_LastRequired = (TryGetMilestone(achievedMilestone, out var entity, out var milestone) ? milestone.m_XpRequried : 0);
		if (TryGetMilestone(achievedMilestone + 1, out entity, out var milestone2))
		{
			m_NextRequired = milestone2.m_XpRequried;
			if (m_CitySystem.XP >= m_NextRequired)
			{
				singleton.m_AchievedMilestone++;
				((EntityQuery)(ref m_MilestoneLevelGroup)).SetSingleton<MilestoneLevel>(singleton);
				NextMilestone(singleton.m_AchievedMilestone);
			}
		}
		m_Progress = m_CitySystem.XP - math.max(0, m_LastRequired);
		m_NextMilestone = singleton.m_AchievedMilestone + 1;
	}

	private void NextMilestone(int index)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		EntityCommandBuffer val = m_ModificationEndBarrier.CreateCommandBuffer();
		if (TryGetMilestone(index, out var entity, out var milestone))
		{
			Entity val2 = ((EntityCommandBuffer)(ref val)).CreateEntity(m_MilestoneReachedEventArchetype);
			((EntityCommandBuffer)(ref val)).SetComponent<MilestoneReachedEvent>(val2, new MilestoneReachedEvent(entity, index));
			Entity val3 = ((EntityCommandBuffer)(ref val)).CreateEntity(m_UnlockEventArchetype);
			((EntityCommandBuffer)(ref val)).SetComponent<Unlock>(val3, new Unlock(entity));
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			PlayerMoney componentData = ((EntityManager)(ref entityManager)).GetComponentData<PlayerMoney>(m_CitySystem.City);
			componentData.Add(milestone.m_Reward);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).SetComponentData<PlayerMoney>(m_CitySystem.City, componentData);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			Creditworthiness componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<Creditworthiness>(m_CitySystem.City);
			componentData2.m_Amount += milestone.m_LoanLimit;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).SetComponentData<Creditworthiness>(m_CitySystem.City, componentData2);
		}
		else
		{
			Entity val4 = ((EntityCommandBuffer)(ref val)).CreateEntity(m_MilestoneReachedEventArchetype);
			((EntityCommandBuffer)(ref val)).SetComponent<MilestoneReachedEvent>(val4, new MilestoneReachedEvent(Entity.Null, index));
			Debug.LogWarning((object)("Warning: did not find data for milestone " + index));
		}
	}

	private bool TryGetMilestone(int index, out Entity entity, out MilestoneData milestone)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_MilestoneGroup)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<MilestoneData> val2 = ((EntityQuery)(ref m_MilestoneGroup)).ToComponentDataArray<MilestoneData>(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			for (int i = 0; i < val2.Length; i++)
			{
				if (val2[i].m_Index == index)
				{
					entity = val[i];
					milestone = val2[i];
					return true;
				}
			}
		}
		finally
		{
			val.Dispose();
			val2.Dispose();
		}
		entity = default(Entity);
		milestone = default(MilestoneData);
		return false;
	}

	public void UnlockAllMilestones()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_MilestoneGroup)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<MilestoneData> val2 = ((EntityQuery)(ref m_MilestoneGroup)).ToComponentDataArray<MilestoneData>(AllocatorHandle.op_Implicit((Allocator)3));
		MilestoneLevel singleton = ((EntityQuery)(ref m_MilestoneLevelGroup)).GetSingleton<MilestoneLevel>();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PlayerMoney componentData = ((EntityManager)(ref entityManager)).GetComponentData<PlayerMoney>(m_CitySystem.City);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		Creditworthiness componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<Creditworthiness>(m_CitySystem.City);
		try
		{
			for (int i = singleton.m_AchievedMilestone; i < val2.Length; i++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				Entity val3 = ((EntityManager)(ref entityManager)).CreateEntity(m_UnlockEventArchetype);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<Unlock>(val3, new Unlock(val[i]));
				singleton.m_AchievedMilestone = math.max(singleton.m_AchievedMilestone, val2[i].m_Index);
				componentData.Add(val2[i].m_Reward);
				componentData2.m_Amount += val2[i].m_LoanLimit;
			}
		}
		finally
		{
			val.Dispose();
			val2.Dispose();
		}
		((EntityQuery)(ref m_MilestoneLevelGroup)).SetSingleton<MilestoneLevel>(singleton);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentData<PlayerMoney>(m_CitySystem.City, componentData);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentData<Creditworthiness>(m_CitySystem.City, componentData2);
	}

	[Preserve]
	public MilestoneSystem()
	{
	}
}
