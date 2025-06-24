using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Net;

public struct Curve : IComponentData, IQueryTypeParameter, IStrideSerializable, ISerializable
{
	public Bezier4x3 m_Bezier;

	public float m_Length;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Bezier);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Bezier);
		m_Length = MathUtils.Length(m_Bezier);
	}

	public int GetStride(Context context)
	{
		return UnsafeUtility.SizeOf<float3>();
	}
}
