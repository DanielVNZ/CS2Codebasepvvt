using Unity.Entities;

namespace Game.Triggers;

public struct TriggerAction
{
	public TriggerType m_TriggerType;

	public Entity m_TriggerPrefab;

	public Entity m_PrimaryTarget;

	public Entity m_SecondaryTarget;

	public float m_Value;

	public TriggerAction(TriggerType triggerType, Entity triggerPrefab, Entity primaryTarget, Entity secondaryTarget, float value = 0f)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		m_TriggerType = triggerType;
		m_TriggerPrefab = triggerPrefab;
		m_PrimaryTarget = primaryTarget;
		m_SecondaryTarget = secondaryTarget;
		m_Value = value;
	}

	public TriggerAction(TriggerType triggerType, Entity triggerPrefab, float value)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		m_TriggerType = triggerType;
		m_TriggerPrefab = triggerPrefab;
		m_PrimaryTarget = Entity.Null;
		m_SecondaryTarget = Entity.Null;
		m_Value = value;
	}
}
