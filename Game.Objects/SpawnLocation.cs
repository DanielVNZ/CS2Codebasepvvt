using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Objects;

public struct SpawnLocation : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_AccessRestriction;

	public Entity m_ConnectedLane1;

	public Entity m_ConnectedLane2;

	public float m_CurvePosition1;

	public float m_CurvePosition2;

	public int m_GroupIndex;

	public SpawnLocationFlags m_Flags;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Entity accessRestriction = m_AccessRestriction;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(accessRestriction);
		Entity connectedLane = m_ConnectedLane1;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(connectedLane);
		Entity connectedLane2 = m_ConnectedLane2;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(connectedLane2);
		float curvePosition = m_CurvePosition1;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(curvePosition);
		float curvePosition2 = m_CurvePosition2;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(curvePosition2);
		int groupIndex = m_GroupIndex;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(groupIndex);
		SpawnLocationFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.pathfindAccessRestriction)
		{
			ref Entity accessRestriction = ref m_AccessRestriction;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref accessRestriction);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.spawnLocationRefactor)
		{
			ref Entity connectedLane = ref m_ConnectedLane1;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref connectedLane);
			ref Entity connectedLane2 = ref m_ConnectedLane2;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref connectedLane2);
			ref float curvePosition = ref m_CurvePosition1;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref curvePosition);
			ref float curvePosition2 = ref m_CurvePosition2;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref curvePosition2);
		}
		else
		{
			Entity val = default(Entity);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
			float num = default(float);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.spawnLocationGroup)
		{
			ref int groupIndex = ref m_GroupIndex;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref groupIndex);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.pathfindRestrictions)
		{
			uint flags = default(uint);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (SpawnLocationFlags)flags;
		}
	}
}
