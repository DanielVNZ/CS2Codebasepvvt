using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Common;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[] { typeof(BuildingPrefab) })]
public class ExtractorFacility : ComponentBase
{
	public Bounds1 m_RotationRange;

	public Bounds1 m_HeightOffset;

	public bool m_RouteNeeded;

	public bool m_NetNeeded;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ExtractorFacilityData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.ExtractorFacility>());
		components.Add(ComponentType.ReadWrite<PointOfInterest>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		ExtractorFacilityData extractorFacilityData = default(ExtractorFacilityData);
		extractorFacilityData.m_RotationRange.min = math.radians(m_RotationRange.min);
		extractorFacilityData.m_RotationRange.max = math.radians(m_RotationRange.max);
		extractorFacilityData.m_HeightOffset = m_HeightOffset;
		extractorFacilityData.m_Requirements = ExtractorRequirementFlags.None;
		if (m_RouteNeeded)
		{
			extractorFacilityData.m_Requirements |= ExtractorRequirementFlags.RouteConnect;
		}
		if (m_NetNeeded)
		{
			extractorFacilityData.m_Requirements |= ExtractorRequirementFlags.NetConnect;
		}
		((EntityManager)(ref entityManager)).SetComponentData<ExtractorFacilityData>(entity, extractorFacilityData);
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(14));
	}
}
