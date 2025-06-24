using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Economy;
using Game.Prefabs;
using Game.Routes;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class MailSection : InfoSectionBase
{
	private enum MailKey
	{
		ToDeliver,
		Collected,
		Unsorted,
		Local
	}

	private enum Type
	{
		PostFacility,
		MailBox
	}

	protected override string group => "MailSection";

	private int sortingRate { get; set; }

	private int sortingCapacity { get; set; }

	private int localAmount { get; set; }

	private int unsortedAmount { get; set; }

	private int outgoingAmount { get; set; }

	private int storedAmount { get; set; }

	private int storageCapacity { get; set; }

	private MailKey localKey { get; set; }

	private MailKey unsortedKey { get; set; }

	private Type type { get; set; }

	protected override void Reset()
	{
		sortingRate = 0;
		sortingCapacity = 0;
		localAmount = 0;
		unsortedAmount = 0;
		outgoingAmount = 0;
		storedAmount = 0;
		storageCapacity = 0;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.PostFacility>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Routes.MailBox>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				return ((EntityManager)(ref entityManager)).HasComponent<MailBoxData>(selectedPrefab);
			}
			return false;
		}
		return true;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		Game.Routes.MailBox mailBox2 = default(Game.Routes.MailBox);
		MailBoxData mailBoxData = default(MailBoxData);
		if (TryGetComponentWithUpgrades<PostFacilityData>(selectedEntity, selectedPrefab, out PostFacilityData data))
		{
			type = Type.PostFacility;
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			Game.Buildings.PostFacility componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Buildings.PostFacility>(selectedEntity);
			sortingRate = (data.m_SortingRate * componentData.m_ProcessingFactor + 50) / 100;
			sortingCapacity = data.m_SortingRate;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<Resources> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Resources>(selectedEntity, true);
			unsortedAmount = EconomyUtils.GetResources(Resource.UnsortedMail, buffer);
			localAmount = EconomyUtils.GetResources(Resource.LocalMail, buffer);
			outgoingAmount = EconomyUtils.GetResources(Resource.OutgoingMail, buffer);
			Game.Routes.MailBox mailBox = default(Game.Routes.MailBox);
			if (EntitiesExtensions.TryGetComponent<Game.Routes.MailBox>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref mailBox))
			{
				unsortedAmount += mailBox.m_MailAmount;
			}
			localKey = ((data.m_PostVanCapacity <= 0) ? MailKey.Local : MailKey.ToDeliver);
			unsortedKey = ((data.m_PostVanCapacity > 0) ? MailKey.Collected : MailKey.Unsorted);
			storedAmount = unsortedAmount + localAmount + outgoingAmount;
			storageCapacity = data.m_MailCapacity;
			base.tooltipKeys.Add(localKey.ToString());
			if (sortingCapacity > 0 || outgoingAmount > 0)
			{
				base.tooltipKeys.Add("Outgoing");
			}
			base.tooltipKeys.Add(unsortedKey.ToString());
			if (sortingCapacity > 0)
			{
				base.tooltipKeys.Add("Sorting");
			}
			if (storageCapacity > 0)
			{
				base.tooltipKeys.Add("Storage");
			}
		}
		else if (EntitiesExtensions.TryGetComponent<Game.Routes.MailBox>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref mailBox2) && EntitiesExtensions.TryGetComponent<MailBoxData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref mailBoxData))
		{
			type = Type.MailBox;
			storageCapacity = mailBoxData.m_MailCapacity;
			storedAmount = mailBox2.m_MailAmount;
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("sortingRate");
		writer.Write(sortingRate);
		writer.PropertyName("sortingCapacity");
		writer.Write(sortingCapacity);
		writer.PropertyName("localAmount");
		writer.Write(localAmount);
		writer.PropertyName("unsortedAmount");
		writer.Write(unsortedAmount);
		writer.PropertyName("outgoingAmount");
		writer.Write(outgoingAmount);
		writer.PropertyName("storedAmount");
		writer.Write(storedAmount);
		writer.PropertyName("storageCapacity");
		writer.Write(storageCapacity);
		writer.PropertyName("localKey");
		writer.Write(Enum.GetName(typeof(MailKey), localKey));
		writer.PropertyName("unsortedKey");
		writer.Write(Enum.GetName(typeof(MailKey), unsortedKey));
		writer.PropertyName("type");
		writer.Write((int)type);
	}

	[Preserve]
	public MailSection()
	{
	}
}
