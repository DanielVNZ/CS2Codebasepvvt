using System.Collections.Generic;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.PSI;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ExcludeGeneratedModTag]
public abstract class TrainPrefab : VehiclePrefab
{
	public TrackTypes m_TrackType = TrackTypes.Train;

	public EnergyTypes m_EnergyType = EnergyTypes.Electricity;

	public float m_MaxSpeed = 200f;

	public float m_Acceleration = 5f;

	public float m_Braking = 10f;

	public float2 m_Turning = new float2(90f, 15f);

	public float2 m_BogieOffset = new float2(4f, 4f);

	public float2 m_AttachOffset = new float2(0f, 0f);

	public override IEnumerable<string> modTags
	{
		get
		{
			foreach (string modTag in base.modTags)
			{
				yield return modTag;
			}
			foreach (string enumFlagTag in ModTags.GetEnumFlagTags(m_TrackType, TrackTypes.Train))
			{
				yield return enumFlagTag;
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<TrainData>());
		components.Add(ComponentType.ReadWrite<TrainObjectData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Train>());
		if (components.Contains(ComponentType.ReadWrite<Stopped>()))
		{
			components.Add(ComponentType.ReadWrite<ParkedTrain>());
		}
		if (components.Contains(ComponentType.ReadWrite<Moving>()))
		{
			components.Add(ComponentType.ReadWrite<TrainNavigation>());
			components.Add(ComponentType.ReadWrite<TrainCurrentLane>());
			components.Add(ComponentType.ReadWrite<TrainBogieFrame>());
			if (components.Contains(ComponentType.ReadWrite<LayoutElement>()))
			{
				components.Add(ComponentType.ReadWrite<PathOwner>());
				components.Add(ComponentType.ReadWrite<PathElement>());
				components.Add(ComponentType.ReadWrite<Target>());
				components.Add(ComponentType.ReadWrite<Blocker>());
				components.Add(ComponentType.ReadWrite<TrainNavigationLane>());
			}
		}
	}

	protected override void RefreshArchetype(EntityManager entityManager, Entity entity)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		List<ComponentBase> list = new List<ComponentBase>();
		GetComponents(list);
		ObjectData objectData = default(ObjectData);
		MovingObjectData movingObjectData = default(MovingObjectData);
		TrainObjectData trainObjectData = default(TrainObjectData);
		HashSet<ComponentType> hashSet = new HashSet<ComponentType>();
		hashSet.Add(ComponentType.ReadWrite<Controller>());
		hashSet.Add(ComponentType.ReadWrite<Moving>());
		for (int i = 0; i < list.Count; i++)
		{
			list[i].GetArchetypeComponents(hashSet);
		}
		hashSet.Add(ComponentType.ReadWrite<Created>());
		hashSet.Add(ComponentType.ReadWrite<Updated>());
		objectData.m_Archetype = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet));
		hashSet.Clear();
		hashSet.Add(ComponentType.ReadWrite<Controller>());
		hashSet.Add(ComponentType.ReadWrite<Stopped>());
		for (int j = 0; j < list.Count; j++)
		{
			list[j].GetArchetypeComponents(hashSet);
		}
		hashSet.Add(ComponentType.ReadWrite<Created>());
		hashSet.Add(ComponentType.ReadWrite<Updated>());
		movingObjectData.m_StoppedArchetype = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet));
		hashSet.Clear();
		hashSet.Add(ComponentType.ReadWrite<Controller>());
		hashSet.Add(ComponentType.ReadWrite<Moving>());
		hashSet.Add(ComponentType.ReadWrite<LayoutElement>());
		for (int k = 0; k < list.Count; k++)
		{
			list[k].GetArchetypeComponents(hashSet);
		}
		hashSet.Add(ComponentType.ReadWrite<Created>());
		hashSet.Add(ComponentType.ReadWrite<Updated>());
		trainObjectData.m_ControllerArchetype = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet));
		hashSet.Clear();
		hashSet.Add(ComponentType.ReadWrite<Controller>());
		hashSet.Add(ComponentType.ReadWrite<Stopped>());
		hashSet.Add(ComponentType.ReadWrite<LayoutElement>());
		for (int l = 0; l < list.Count; l++)
		{
			list[l].GetArchetypeComponents(hashSet);
		}
		hashSet.Add(ComponentType.ReadWrite<Created>());
		hashSet.Add(ComponentType.ReadWrite<Updated>());
		trainObjectData.m_StoppedControllerArchetype = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet));
		((EntityManager)(ref entityManager)).SetComponentData<ObjectData>(entity, objectData);
		((EntityManager)(ref entityManager)).SetComponentData<MovingObjectData>(entity, movingObjectData);
		((EntityManager)(ref entityManager)).SetComponentData<TrainObjectData>(entity, trainObjectData);
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(3));
	}
}
