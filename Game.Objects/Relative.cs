using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Objects;

public struct Relative : IComponentData, IQueryTypeParameter, ISerializable
{
	public float3 m_Position;

	public quaternion m_Rotation;

	public int3 m_BoneIndex;

	public Relative(Transform localTransform, int3 boneIndex)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		m_Position = localTransform.m_Position;
		m_Rotation = localTransform.m_Rotation;
		m_BoneIndex = boneIndex;
	}

	public Transform ToTransform()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return new Transform(m_Position, m_Rotation);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		float3 position = m_Position;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(position);
		quaternion rotation = m_Rotation;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(rotation);
		int x = m_BoneIndex.x;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(x);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		ref float3 position = ref m_Position;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref position);
		ref quaternion rotation = ref m_Rotation;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref rotation);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.boneRelativeObjects)
		{
			ref int x = ref m_BoneIndex.x;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref x);
		}
		((int3)(ref m_BoneIndex)).yz = int2.op_Implicit(-1);
	}
}
