using Colossal.Serialization.Entities;
using Game.Prefabs;
using Unity.Entities;

namespace Game.Simulation;

public struct PoliceEmergencyRequest : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Site;

	public Entity m_Target;

	public float m_Priority;

	public PolicePurpose m_Purpose;

	public PoliceEmergencyRequest(Entity site, Entity target, float priority, PolicePurpose purpose)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_Site = site;
		m_Target = target;
		m_Priority = priority;
		m_Purpose = purpose;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity site = m_Site;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(site);
		Entity target = m_Target;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(target);
		float priority = m_Priority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(priority);
		PolicePurpose purpose = m_Purpose;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((int)purpose);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ref Entity site = ref m_Site;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref site);
		ref Entity target = ref m_Target;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref target);
		ref float priority = ref m_Priority;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref priority);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.policeImprovement3)
		{
			int purpose = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref purpose);
			m_Purpose = (PolicePurpose)purpose;
		}
		else
		{
			m_Purpose = PolicePurpose.Patrol | PolicePurpose.Emergency;
		}
	}
}
