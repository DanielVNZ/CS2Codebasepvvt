using System;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Global/", new Type[] { })]
public class PlaceableNetPieceGlobalMode : EntityQueryModePrefab
{
	[BurstCompile]
	private struct ModeJob : IJobChunk
	{
		public float m_ConstructionCostMultiplier;

		public float m_ElevationCostMultiplier;

		public float m_UpkeepCostMultiplier;

		public ComponentTypeHandle<PlaceableNetPieceData> m_PlaceableNetPieceType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PlaceableNetPieceData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PlaceableNetPieceData>(ref m_PlaceableNetPieceType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				PlaceableNetPieceData placeableNetPieceData = nativeArray[i];
				placeableNetPieceData.m_ConstructionCost = (uint)((float)placeableNetPieceData.m_ConstructionCost * m_ConstructionCostMultiplier);
				placeableNetPieceData.m_ElevationCost = (uint)((float)placeableNetPieceData.m_ElevationCost * m_ElevationCostMultiplier);
				placeableNetPieceData.m_UpkeepCost *= m_UpkeepCostMultiplier;
				nativeArray[i] = placeableNetPieceData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	public float m_ConstructionCostMultiplier;

	public float m_ElevationCostMultiplier;

	public float m_UpkeepCostMultiplier;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PlaceableNetPieceData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<PlaceableNetPieceData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		return JobChunkExtensions.ScheduleParallel<ModeJob>(new ModeJob
		{
			m_ConstructionCostMultiplier = m_ConstructionCostMultiplier,
			m_ElevationCostMultiplier = m_ElevationCostMultiplier,
			m_UpkeepCostMultiplier = m_UpkeepCostMultiplier,
			m_PlaceableNetPieceType = ((EntityManager)(ref entityManager)).GetComponentTypeHandle<PlaceableNetPieceData>(false)
		}, requestedQuery, deps);
	}

	public unsafe override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < entities.Length; i++)
		{
			Entity val = entities[i];
			if (!prefabSystem.TryGetPrefab<PrefabBase>(val, out var prefabBase) || !prefabBase.TryGetExactly<PlaceableNetPiece>(out var component))
			{
				ComponentBase.baseLog.Warn((object)$"Prefab data not found {this} : {((object)(*(Entity*)(&val))/*cast due to .constrained prefix*/).ToString()} : {prefabBase}");
				continue;
			}
			PlaceableNetPieceData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PlaceableNetPieceData>(val);
			componentData.m_ConstructionCost = component.m_ConstructionCost;
			componentData.m_ElevationCost = component.m_ElevationCost;
			componentData.m_UpkeepCost = component.m_UpkeepCost;
			((EntityManager)(ref entityManager)).SetComponentData<PlaceableNetPieceData>(val, componentData);
		}
	}
}
