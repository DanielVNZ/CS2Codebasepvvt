using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { typeof(RenderingSettingsPrefab) })]
public class MeshSettings : ComponentBase
{
	public RenderPrefab m_MissingObjectMesh;

	public RenderPrefab m_DefaultBaseMesh;

	public NetSectionPrefab m_MissingNetSection;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_MissingObjectMesh);
		prefabs.Add(m_DefaultBaseMesh);
		prefabs.Add(m_MissingNetSection);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<MeshSettingsData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		MeshSettingsData meshSettingsData = new MeshSettingsData
		{
			m_MissingObjectMesh = existingSystemManaged.GetEntity(m_MissingObjectMesh),
			m_DefaultBaseMesh = existingSystemManaged.GetEntity(m_DefaultBaseMesh),
			m_MissingNetSection = existingSystemManaged.GetEntity(m_MissingNetSection)
		};
		((EntityManager)(ref entityManager)).SetComponentData<MeshSettingsData>(entity, meshSettingsData);
	}
}
