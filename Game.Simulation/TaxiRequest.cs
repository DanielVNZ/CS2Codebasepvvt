using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct TaxiRequest : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Seeker;

	public Entity m_District1;

	public Entity m_District2;

	public int m_Priority;

	public TaxiRequestType m_Type;

	public TaxiRequest(Entity seeker, Entity district1, Entity district2, TaxiRequestType type, int priority)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		m_Seeker = seeker;
		m_District1 = district1;
		m_District2 = district2;
		m_Priority = priority;
		m_Type = type;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Entity seeker = m_Seeker;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(seeker);
		Entity district = m_District1;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(district);
		Entity district2 = m_District2;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(district2);
		int priority = m_Priority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(priority);
		TaxiRequestType type = m_Type;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)type);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		ref Entity seeker = ref m_Seeker;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref seeker);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.taxiServiceDistricts)
		{
			ref Entity district = ref m_District1;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref district);
			ref Entity district2 = ref m_District2;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref district2);
		}
		ref int priority = ref m_Priority;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref priority);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.taxiDispatchCenter)
		{
			byte type = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref type);
			m_Type = (TaxiRequestType)type;
		}
	}
}
