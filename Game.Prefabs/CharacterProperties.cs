using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Rendering/", new Type[] { typeof(RenderPrefab) })]
public class CharacterProperties : ComponentBase
{
	[Flags]
	public enum BodyPart
	{
		Torso = 1,
		Head = 2,
		Face = 4,
		Legs = 8,
		Feet = 0x10,
		Beard = 0x20,
		Neck = 0x40
	}

	public BodyPart m_BodyParts;

	public string m_CorrectiveAnimationName;

	public string m_AnimatedPropName;

	public CharacterOverlay[] m_Overlays;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if (m_Overlays != null && m_Overlays.Length != 0)
		{
			for (int i = 0; i < m_Overlays.Length; i++)
			{
				prefabs.Add(m_Overlays[i]);
			}
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (m_Overlays != null && m_Overlays.Length != 0)
		{
			components.Add(ComponentType.ReadWrite<OverlayElement>());
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		if (m_Overlays != null && m_Overlays.Length != 0)
		{
			PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
			DynamicBuffer<OverlayElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<OverlayElement>(entity, false);
			int num = 0;
			for (int i = 0; i < m_Overlays.Length; i++)
			{
				CharacterOverlay characterOverlay = m_Overlays[i];
				num = math.max(num, characterOverlay.m_Index + 1);
			}
			buffer.Resize(num, (NativeArrayOptions)1);
			for (int j = 0; j < m_Overlays.Length; j++)
			{
				CharacterOverlay characterOverlay2 = m_Overlays[j];
				buffer[characterOverlay2.m_Index] = new OverlayElement
				{
					m_Overlay = existingSystemManaged.GetEntity(characterOverlay2),
					m_SortOrder = characterOverlay2.m_SortOrder
				};
			}
		}
	}
}
