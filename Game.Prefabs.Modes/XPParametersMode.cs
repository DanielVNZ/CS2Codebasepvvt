using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Parameters/", new Type[] { })]
public class XPParametersMode : EntityQueryModePrefab
{
	public float m_XPPerPopulation;

	public float m_XPPerHappiness;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<XPParameterData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<XPParameterData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref requestedQuery)).GetSingletonEntity();
		XPParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<XPParameterData>(singletonEntity);
		componentData.m_XPPerPopulation = m_XPPerPopulation;
		componentData.m_XPPerHappiness = m_XPPerHappiness;
		((EntityManager)(ref entityManager)).SetComponentData<XPParameterData>(singletonEntity, componentData);
		return deps;
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Entity val = entities[0];
		XPParametersPrefab xPParametersPrefab = prefabSystem.GetPrefab<XPParametersPrefab>(val);
		XPParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<XPParameterData>(val);
		componentData.m_XPPerPopulation = xPParametersPrefab.m_XPPerPopulation;
		componentData.m_XPPerHappiness = xPParametersPrefab.m_XPPerHappiness;
		((EntityManager)(ref entityManager)).SetComponentData<XPParameterData>(val, componentData);
	}
}
