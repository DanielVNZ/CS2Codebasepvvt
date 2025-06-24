using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct Building : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_RoadEdge;

	public float m_CurvePosition;

	public uint m_OptionMask;

	public BuildingFlags m_Flags;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity roadEdge = m_RoadEdge;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(roadEdge);
		float curvePosition = m_CurvePosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(curvePosition);
		uint optionMask = m_OptionMask;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(optionMask);
		BuildingFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		ref Entity roadEdge = ref m_RoadEdge;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref roadEdge);
		ref float curvePosition = ref m_CurvePosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref curvePosition);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.buildingOptions)
		{
			ref uint optionMask = ref m_OptionMask;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref optionMask);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.companyNotifications)
		{
			byte flags = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (BuildingFlags)flags;
		}
	}
}
