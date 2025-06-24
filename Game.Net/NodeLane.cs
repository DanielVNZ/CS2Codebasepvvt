using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Net;

public struct NodeLane : IComponentData, IQueryTypeParameter, ISerializable
{
	public float2 m_WidthOffset;

	public byte m_SharedStartCount;

	public byte m_SharedEndCount;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (math.all(m_WidthOffset != 0f))
		{
			((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)3);
			float x = m_WidthOffset.x;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(x);
			float y = m_WidthOffset.y;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(y);
		}
		else if (m_WidthOffset.x != 0f)
		{
			((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)1);
			float x2 = m_WidthOffset.x;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(x2);
		}
		else if (m_WidthOffset.y != 0f)
		{
			((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)2);
			float y2 = m_WidthOffset.y;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(y2);
		}
		else
		{
			((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)0);
		}
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.saveOptimizations)
		{
			byte b = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref b);
			if ((b & 1) != 0)
			{
				ref float x = ref m_WidthOffset.x;
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref x);
			}
			if ((b & 2) != 0)
			{
				ref float y = ref m_WidthOffset.y;
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref y);
			}
		}
		else
		{
			ref float2 widthOffset = ref m_WidthOffset;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref widthOffset);
		}
	}
}
