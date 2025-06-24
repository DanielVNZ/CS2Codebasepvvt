using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Pathfind;

[InternalBufferCapacity(0)]
public struct PathElement : IBufferElementData, ISerializable
{
	public Entity m_Target;

	public float2 m_TargetDelta;

	public PathElementFlags m_Flags;

	public PathElement(Entity target, float2 targetDelta, PathElementFlags flags = ~(PathElementFlags.Secondary | PathElementFlags.PathStart | PathElementFlags.Action | PathElementFlags.Return | PathElementFlags.Reverse | PathElementFlags.WaitPosition | PathElementFlags.Leader | PathElementFlags.Hangaround))
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_Target = target;
		m_TargetDelta = targetDelta;
		m_Flags = flags;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		Entity target = m_Target;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(target);
		int2 val = math.select(math.select(new int2(2), new int2(0), m_TargetDelta == 0f), new int2(1), m_TargetDelta == 1f);
		byte num = (byte)(val.x | (val.y << 4));
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		if (val.x == 2)
		{
			float x = m_TargetDelta.x;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(x);
		}
		if (val.y == 2)
		{
			float y = m_TargetDelta.y;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(y);
		}
		PathElementFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		ref Entity target = ref m_Target;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref target);
		byte b = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref b);
		int2 val = default(int2);
		((int2)(ref val))._002Ector(b & 0xF, b >> 4);
		m_TargetDelta = math.select(float2.op_Implicit(0f), float2.op_Implicit(1f), val == 1);
		if (val.x == 2)
		{
			ref float x = ref m_TargetDelta.x;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref x);
		}
		if (val.y == 2)
		{
			ref float y = ref m_TargetDelta.y;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref y);
		}
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.taxiDispatchCenter)
		{
			byte flags = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (PathElementFlags)flags;
		}
	}
}
