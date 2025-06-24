using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Routes;

[InternalBufferCapacity(0)]
public struct RouteModifier : IBufferElementData, ISerializable
{
	public float2 m_Delta;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Delta);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.modifierRefactoring)
		{
			ref float2 delta = ref m_Delta;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref delta);
		}
		else
		{
			ref float y = ref m_Delta.y;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref y);
		}
	}
}
