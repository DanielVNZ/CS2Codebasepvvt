using System;
using System.Collections.Generic;
using Game.Objects;
using Game.Pathfind;
using Game.Simulation;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Vehicles/", new Type[] { typeof(CarPrefab) })]
public class PostVan : ComponentBase
{
	public int m_MailCapacity;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PostVanData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Vehicles.PostVan>());
		if (components.Contains(ComponentType.ReadWrite<Moving>()))
		{
			components.Add(ComponentType.ReadWrite<PathInformation>());
			components.Add(ComponentType.ReadWrite<ServiceDispatch>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).SetComponentData<PostVanData>(entity, new PostVanData(m_MailCapacity));
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(9));
	}
}
