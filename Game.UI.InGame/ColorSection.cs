using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Common;
using Game.Routes;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class ColorSection : InfoSectionBase
{
	private EntityArchetype m_ColorUpdateArchetype;

	protected override string group => "ColorSection";

	private Color32 color { get; set; }

	protected override void Reset()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		color = default(Color32);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		AddBinding((IBinding)(object)new TriggerBinding<Color>(group, "setColor", (Action<Color>)OnSetColor, (IReader<Color>)null));
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_ColorUpdateArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<ColorUpdated>()
		});
	}

	private void OnSetColor(Color uiColor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Route>(selectedEntity))
		{
			return;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<TransportLine>(selectedEntity))
		{
			return;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<RouteWaypoint>(selectedEntity))
		{
			return;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Color>(selectedEntity))
		{
			return;
		}
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		((EntityCommandBuffer)(ref val)).SetComponent<Color>(selectedEntity, new Color(Color32.op_Implicit(uiColor)));
		DynamicBuffer<RouteVehicle> val2 = default(DynamicBuffer<RouteVehicle>);
		if (EntitiesExtensions.TryGetBuffer<RouteVehicle>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val2))
		{
			for (int i = 0; i < val2.Length; i++)
			{
				((EntityCommandBuffer)(ref val)).AddComponent<Color>(val2[i].m_Vehicle, new Color(Color32.op_Implicit(uiColor)));
			}
		}
		Entity val3 = ((EntityCommandBuffer)(ref val)).CreateEntity(m_ColorUpdateArchetype);
		((EntityCommandBuffer)(ref val)).SetComponent<ColorUpdated>(val3, new ColorUpdated(selectedEntity));
		m_InfoUISystem.RequestUpdate();
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
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Route>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<TransportLine>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<RouteWaypoint>(selectedEntity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					return ((EntityManager)(ref entityManager)).HasComponent<Color>(selectedEntity);
				}
			}
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		color = ((EntityManager)(ref entityManager)).GetComponentData<Color>(selectedEntity).m_Color;
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("color");
		UnityWriters.Write(writer, color);
	}

	[Preserve]
	public ColorSection()
	{
	}
}
