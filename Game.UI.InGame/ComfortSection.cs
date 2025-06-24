using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Routes;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class ComfortSection : InfoSectionBase
{
	protected override string group => "ComfortSection";

	private int comfort { get; set; }

	protected override void Reset()
	{
		comfort = 0;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<MailBox>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<TransportStop>(selectedEntity))
			{
				goto IL_006d;
			}
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<TransportStation>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<PublicTransportStation>(selectedEntity))
			{
				goto IL_006d;
			}
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<ParkingFacility>(selectedEntity);
		IL_006d:
		return true;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		TransportStop transportStop = default(TransportStop);
		TransportStation transportStation = default(TransportStation);
		ParkingFacility parkingFacility = default(ParkingFacility);
		if (EntitiesExtensions.TryGetComponent<TransportStop>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref transportStop))
		{
			num = transportStop.m_ComfortFactor;
			base.tooltipKeys.Add("TransportStop");
		}
		else if (EntitiesExtensions.TryGetComponent<TransportStation>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref transportStation))
		{
			num = transportStation.m_ComfortFactor;
			base.tooltipKeys.Add("TransportStation");
		}
		else if (EntitiesExtensions.TryGetComponent<ParkingFacility>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref parkingFacility))
		{
			num = parkingFacility.m_ComfortFactor;
			base.tooltipKeys.Add("Parking");
		}
		comfort = (int)math.round(100f * num);
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("comfort");
		writer.Write(comfort);
	}

	[Preserve]
	public ComfortSection()
	{
	}
}
