using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Common;
using Game.Creatures;
using Game.Notifications;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class SelectedUpdateSystem : GameSystemBase
{
	private ToolSystem m_ToolSystem;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.selected == Entity.Null)
		{
			return;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).Exists(m_ToolSystem.selected))
		{
			m_ToolSystem.selected = Entity.Null;
			return;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Deleted>(m_ToolSystem.selected))
		{
			return;
		}
		Entity val = m_ToolSystem.selected;
		entityManager = ((ComponentSystemBase)this).EntityManager;
		Owner owner = default(Owner);
		if (((EntityManager)(ref entityManager)).HasComponent<Icon>(val) && EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, val, ref owner))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).Exists(owner.m_Owner))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<Deleted>(owner.m_Owner))
				{
					m_ToolSystem.selected = owner.m_Owner;
					return;
				}
				val = owner.m_Owner;
			}
		}
		Resident resident = default(Resident);
		Pet pet = default(Pet);
		if (EntitiesExtensions.TryGetComponent<Resident>(((ComponentSystemBase)this).EntityManager, val, ref resident))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).Exists(resident.m_Citizen))
			{
				m_ToolSystem.selected = resident.m_Citizen;
			}
		}
		else if (EntitiesExtensions.TryGetComponent<Pet>(((ComponentSystemBase)this).EntityManager, val, ref pet))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).Exists(pet.m_HouseholdPet))
			{
				m_ToolSystem.selected = pet.m_HouseholdPet;
			}
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Deleted>(m_ToolSystem.selected))
		{
			m_ToolSystem.selected = Entity.Null;
		}
	}

	[Preserve]
	public SelectedUpdateSystem()
	{
	}
}
