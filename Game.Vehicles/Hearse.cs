using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Vehicles;

public struct Hearse : IComponentData, IQueryTypeParameter, ISerializable
{
	public HearseFlags m_State;

	public Entity m_TargetCorpse;

	public Entity m_TargetRequest;

	public float m_PathElementTime;

	public Hearse(Entity targetCorpse, HearseFlags state)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		m_State = state;
		m_TargetCorpse = targetCorpse;
		m_TargetRequest = Entity.Null;
		m_PathElementTime = 0f;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		HearseFlags state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)state);
		Entity targetCorpse = m_TargetCorpse;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetCorpse);
		Entity targetRequest = m_TargetRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetRequest);
		float pathElementTime = m_PathElementTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(pathElementTime);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		uint state = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		ref Entity targetCorpse = ref m_TargetCorpse;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetCorpse);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.reverseServiceRequests2)
		{
			ref Entity targetRequest = ref m_TargetRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetRequest);
		}
		ref float pathElementTime = ref m_PathElementTime;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref pathElementTime);
		m_State = (HearseFlags)state;
	}
}
