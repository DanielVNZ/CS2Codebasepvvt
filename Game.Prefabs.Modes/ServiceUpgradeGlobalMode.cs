using System;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Global/", new Type[] { })]
public class ServiceUpgradeGlobalMode : EntityQueryModePrefab
{
	[BurstCompile]
	private struct ModeJob : IJobChunk
	{
		public float m_UpgradeCostMultiplier;

		public float m_XPRewardMultiplier;

		public ComponentTypeHandle<ServiceUpgradeData> m_ServiceUpgradeType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<ServiceUpgradeData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceUpgradeData>(ref m_ServiceUpgradeType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				ServiceUpgradeData serviceUpgradeData = nativeArray[i];
				serviceUpgradeData.m_UpgradeCost = (uint)((float)serviceUpgradeData.m_UpgradeCost * m_UpgradeCostMultiplier);
				serviceUpgradeData.m_XPReward = (int)((float)serviceUpgradeData.m_XPReward * m_XPRewardMultiplier);
				nativeArray[i] = serviceUpgradeData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	public float m_UpgradeCostMultiplier;

	public float m_XPRewardMultiplier;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ServiceUpgradeData>(),
			ComponentType.ReadOnly<ServiceUpgradeBuilding>()
		};
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<ServiceUpgradeData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		return JobChunkExtensions.ScheduleParallel<ModeJob>(new ModeJob
		{
			m_UpgradeCostMultiplier = m_UpgradeCostMultiplier,
			m_XPRewardMultiplier = m_XPRewardMultiplier,
			m_ServiceUpgradeType = ((EntityManager)(ref entityManager)).GetComponentTypeHandle<ServiceUpgradeData>(false)
		}, requestedQuery, deps);
	}

	public unsafe override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < entities.Length; i++)
		{
			Entity val = entities[i];
			if (!prefabSystem.TryGetPrefab<PrefabBase>(val, out var prefabBase) || !prefabBase.TryGetExactly<ServiceUpgrade>(out var component))
			{
				ComponentBase.baseLog.Warn((object)$"Prefab data not found {this} : {((object)(*(Entity*)(&val))/*cast due to .constrained prefix*/).ToString()} : {prefabBase}");
				continue;
			}
			ServiceUpgradeData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ServiceUpgradeData>(val);
			componentData.m_UpgradeCost = component.m_UpgradeCost;
			componentData.m_XPReward = component.m_XPReward;
			((EntityManager)(ref entityManager)).SetComponentData<ServiceUpgradeData>(val, componentData);
		}
	}
}
