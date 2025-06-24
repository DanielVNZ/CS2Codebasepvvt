using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Routes;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class LineSection : InfoSectionBase
{
	protected override string group => "LineSection";

	private float length { get; set; }

	private int stops { get; set; }

	private int cargo { get; set; }

	private float usage { get; set; }

	protected override void Reset()
	{
		length = 0f;
		stops = 0;
		cargo = 0;
		usage = 0f;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		int num;
		if (((EntityManager)(ref entityManager)).HasComponent<Route>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<TransportLine>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				num = (((EntityManager)(ref entityManager)).HasComponent<RouteWaypoint>(selectedEntity) ? 1 : 0);
				goto IL_0044;
			}
		}
		num = 0;
		goto IL_0044;
		IL_0044:
		base.visible = (byte)num != 0;
	}

	protected override void OnProcess()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		m_InfoUISystem.SetRoutesVisible();
		int num = 0;
		int capacity = 0;
		TransportUIUtils.GetRouteVehiclesCount(((ComponentSystemBase)this).EntityManager, selectedEntity, ref num, ref capacity);
		usage = ((capacity > 0) ? ((float)num / (float)capacity) : 0f);
		stops = TransportUIUtils.GetStopCount(((ComponentSystemBase)this).EntityManager, selectedEntity);
		length = TransportUIUtils.GetRouteLength(((ComponentSystemBase)this).EntityManager, selectedEntity);
		cargo = num;
		base.tooltipTags.Add(TooltipTags.CargoRoute.ToString());
		base.tooltipTags.Add(TooltipTags.TransportLine.ToString());
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("length");
		writer.Write(length);
		writer.PropertyName("stops");
		writer.Write(stops);
		writer.PropertyName("usage");
		writer.Write(usage);
		writer.PropertyName("cargo");
		writer.Write(cargo);
	}

	[Preserve]
	public LineSection()
	{
	}
}
