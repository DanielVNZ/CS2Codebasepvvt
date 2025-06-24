using Colossal.Entities;
using Game.Net;
using Game.Prefabs;
using Unity.Entities;

namespace Game.UI.InGame;

public static class ElectricityUIUtils
{
	public static bool HasVoltageLayers(Layer layers)
	{
		return (layers & (Layer.PowerlineLow | Layer.PowerlineHigh)) != 0;
	}

	public static VoltageLocaleKey GetVoltage(Layer layers)
	{
		return (layers & (Layer.PowerlineLow | Layer.PowerlineHigh)) switch
		{
			Layer.PowerlineLow => VoltageLocaleKey.Low, 
			Layer.PowerlineHigh => VoltageLocaleKey.High, 
			_ => VoltageLocaleKey.Both, 
		};
	}

	public static Layer GetPowerLineLayers(EntityManager entityManager, Entity prefabEntity)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Layer layer = Layer.None;
		if (((EntityManager)(ref entityManager)).HasComponent<TransformerData>(prefabEntity))
		{
			layer |= Layer.PowerlineLow;
		}
		DynamicBuffer<Game.Prefabs.SubNet> val = default(DynamicBuffer<Game.Prefabs.SubNet>);
		if (EntitiesExtensions.TryGetBuffer<Game.Prefabs.SubNet>(entityManager, prefabEntity, true, ref val))
		{
			NetData netData = default(NetData);
			for (int i = 0; i < val.Length; i++)
			{
				Entity prefab = val[i].m_Prefab;
				if (((EntityManager)(ref entityManager)).HasComponent<ElectricityConnectionData>(prefab) && EntitiesExtensions.TryGetComponent<NetData>(entityManager, prefab, ref netData))
				{
					layer |= netData.m_LocalConnectLayers;
				}
			}
		}
		return layer & (Layer.PowerlineLow | Layer.PowerlineHigh);
	}
}
