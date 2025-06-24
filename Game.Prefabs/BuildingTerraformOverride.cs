using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Buildings;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[] { typeof(StaticObjectPrefab) })]
public class BuildingTerraformOverride : ComponentBase
{
	[Serializable]
	public class SubLot
	{
		public Bounds2 m_Area = new Bounds2(float2.op_Implicit(-4f), float2.op_Implicit(4f));

		public float m_HeightOffset;

		public bool m_Circular;

		public bool m_DontRaise;

		public bool m_DontLower;
	}

	public float2 m_LevelMinOffset;

	public float2 m_LevelMaxOffset;

	public float2 m_LevelFrontLeft = new float2(1f, 1f);

	public float2 m_LevelFrontRight = new float2(1f, 1f);

	public float2 m_LevelBackLeft = new float2(1f, 1f);

	public float2 m_LevelBackRight = new float2(1f, 1f);

	public float2 m_SmoothMinOffset;

	public float2 m_SmoothMaxOffset;

	public float m_HeightOffset;

	public SubLot[] m_AdditionalSmoothAreas;

	public bool m_DontRaise;

	public bool m_DontLower;

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (base.prefab is BuildingExtensionPrefab)
		{
			components.Add(ComponentType.ReadWrite<Lot>());
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<BuildingTerraformData>());
		if (m_AdditionalSmoothAreas != null && m_AdditionalSmoothAreas.Length != 0)
		{
			components.Add(ComponentType.ReadWrite<AdditionalBuildingTerraformElement>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (m_AdditionalSmoothAreas != null && m_AdditionalSmoothAreas.Length != 0)
		{
			DynamicBuffer<AdditionalBuildingTerraformElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<AdditionalBuildingTerraformElement>(entity, false);
			buffer.ResizeUninitialized(m_AdditionalSmoothAreas.Length);
			for (int i = 0; i < m_AdditionalSmoothAreas.Length; i++)
			{
				SubLot subLot = m_AdditionalSmoothAreas[i];
				buffer[i] = new AdditionalBuildingTerraformElement
				{
					m_Area = subLot.m_Area,
					m_HeightOffset = subLot.m_HeightOffset,
					m_Circular = subLot.m_Circular,
					m_DontRaise = subLot.m_DontRaise,
					m_DontLower = subLot.m_DontLower
				};
			}
		}
	}
}
