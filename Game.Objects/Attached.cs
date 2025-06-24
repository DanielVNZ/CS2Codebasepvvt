using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Objects;

public struct Attached : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Parent;

	public Entity m_OldParent;

	public float m_CurvePosition;

	public Attached(Entity parent, Entity oldParent, float curvePosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_Parent = parent;
		m_OldParent = oldParent;
		m_CurvePosition = curvePosition;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity parent = m_Parent;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(parent);
		float curvePosition = m_CurvePosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(curvePosition);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity parent = ref m_Parent;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref parent);
		ref float curvePosition = ref m_CurvePosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref curvePosition);
	}
}
