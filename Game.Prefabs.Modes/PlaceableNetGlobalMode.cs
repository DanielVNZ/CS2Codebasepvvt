using System;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Global/", new Type[] { })]
public class PlaceableNetGlobalMode : EntityQueryModePrefab
{
	[BurstCompile]
	private struct ModeJob : IJobChunk
	{
		public float m_XPRewardMultiplier;

		public ComponentTypeHandle<PlaceableNetData> m_PlaceableNetType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PlaceableNetData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PlaceableNetData>(ref m_PlaceableNetType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				PlaceableNetData placeableNetData = nativeArray[i];
				placeableNetData.m_XPReward = (int)((float)placeableNetData.m_XPReward * m_XPRewardMultiplier);
				nativeArray[i] = placeableNetData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	public float m_XPRewardMultiplier;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PlaceableNetData>(),
			ComponentType.ReadOnly<PlaceableInfoviewItem>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<FenceData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<PlaceableNetData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return JobChunkExtensions.ScheduleParallel<ModeJob>(new ModeJob
		{
			m_XPRewardMultiplier = m_XPRewardMultiplier,
			m_PlaceableNetType = ((EntityManager)(ref entityManager)).GetComponentTypeHandle<PlaceableNetData>(false)
		}, requestedQuery, deps);
	}

	public unsafe override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < entities.Length; i++)
		{
			Entity val = entities[i];
			if (!prefabSystem.TryGetPrefab<PrefabBase>(val, out var prefabBase) || !prefabBase.TryGetExactly<PlaceableNet>(out var component))
			{
				ComponentBase.baseLog.Warn((object)$"Prefab data not found {this} : {((object)(*(Entity*)(&val))/*cast due to .constrained prefix*/).ToString()} : {prefabBase}");
				continue;
			}
			PlaceableNetData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PlaceableNetData>(val);
			componentData.m_XPReward = component.m_XPReward;
			((EntityManager)(ref entityManager)).SetComponentData<PlaceableNetData>(val, componentData);
		}
	}
}
