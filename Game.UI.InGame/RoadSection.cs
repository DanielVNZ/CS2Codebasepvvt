using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Net;
using Game.Prefabs;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class RoadSection : InfoSectionBase
{
	private float[] m_Volume;

	private float[] m_Flow;

	protected override string group => "RoadSection";

	private float length { get; set; }

	private float bestCondition { get; set; }

	private float worstCondition { get; set; }

	private float condition { get; set; }

	private float upkeep { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_Volume = new float[5];
		m_Flow = new float[5];
	}

	protected override void Reset()
	{
		for (int i = 0; i < 5; i++)
		{
			m_Volume[i] = 0f;
			m_Flow[i] = 0f;
		}
		length = 0f;
		bestCondition = 100f;
		worstCondition = 0f;
		condition = 0f;
		upkeep = 0f;
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
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		int num;
		if (((EntityManager)(ref entityManager)).HasComponent<Aggregate>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			num = (((EntityManager)(ref entityManager)).HasComponent<AggregateElement>(selectedEntity) ? 1 : 0);
		}
		else
		{
			num = 0;
		}
		base.visible = (byte)num != 0;
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<AggregateElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<AggregateElement>(selectedEntity, true);
		Road road = default(Road);
		Curve curve = default(Curve);
		NetCondition netCondition = default(NetCondition);
		PrefabRef prefabRef = default(PrefabRef);
		PlaceableNetData placeableNetData = default(PlaceableNetData);
		for (int i = 0; i < buffer.Length; i++)
		{
			Entity edge = buffer[i].m_Edge;
			if (EntitiesExtensions.TryGetComponent<Road>(((ComponentSystemBase)this).EntityManager, edge, ref road) && EntitiesExtensions.TryGetComponent<Curve>(((ComponentSystemBase)this).EntityManager, edge, ref curve))
			{
				length += curve.m_Length;
				float4 val = (road.m_TrafficFlowDistance0 + road.m_TrafficFlowDistance1) * 16f;
				float4 val2 = NetUtils.GetTrafficFlowSpeed(road) * 100f;
				m_Volume[0] += val.x * 4f / 24f;
				m_Volume[1] += val.y * 4f / 24f;
				m_Volume[2] += val.z * 4f / 24f;
				m_Volume[3] += val.w * 4f / 24f;
				m_Flow[0] += val2.x;
				m_Flow[1] += val2.y;
				m_Flow[2] += val2.z;
				m_Flow[3] += val2.w;
			}
			if (EntitiesExtensions.TryGetComponent<NetCondition>(((ComponentSystemBase)this).EntityManager, edge, ref netCondition))
			{
				float2 wear = netCondition.m_Wear;
				if (wear.x > worstCondition)
				{
					worstCondition = wear.x;
				}
				if (wear.y > worstCondition)
				{
					worstCondition = wear.y;
				}
				if (wear.x < bestCondition)
				{
					bestCondition = wear.x;
				}
				if (wear.y < bestCondition)
				{
					bestCondition = wear.y;
				}
				condition += math.csum(wear) * 0.5f;
			}
			if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, edge, ref prefabRef) && EntitiesExtensions.TryGetComponent<PlaceableNetData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, ref placeableNetData))
			{
				upkeep += placeableNetData.m_DefaultUpkeepCost;
			}
		}
		m_Volume[0] /= buffer.Length;
		m_Volume[1] /= buffer.Length;
		m_Volume[2] /= buffer.Length;
		m_Volume[3] /= buffer.Length;
		m_Volume[4] = m_Volume[0];
		m_Flow[0] /= buffer.Length;
		m_Flow[1] /= buffer.Length;
		m_Flow[2] /= buffer.Length;
		m_Flow[3] /= buffer.Length;
		m_Flow[4] = m_Flow[0];
		bestCondition = 100f - bestCondition / 10f * 100f;
		worstCondition = 100f - worstCondition / 10f * 100f;
		condition = condition / 10f * 100f;
		condition = 100f - condition / (float)buffer.Length;
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("volumeData");
		JsonWriterExtensions.ArrayBegin(writer, m_Volume.Length);
		for (int i = 0; i < m_Volume.Length; i++)
		{
			writer.Write(m_Volume[i]);
		}
		writer.ArrayEnd();
		writer.PropertyName("flowData");
		JsonWriterExtensions.ArrayBegin(writer, m_Flow.Length);
		for (int j = 0; j < m_Flow.Length; j++)
		{
			writer.Write(m_Flow[j]);
		}
		writer.ArrayEnd();
		writer.PropertyName("length");
		writer.Write(length);
		writer.PropertyName("bestCondition");
		writer.Write(bestCondition);
		writer.PropertyName("worstCondition");
		writer.Write(worstCondition);
		writer.PropertyName("condition");
		writer.Write(condition);
		writer.PropertyName("upkeep");
		writer.Write(upkeep);
	}

	[Preserve]
	public RoadSection()
	{
	}
}
