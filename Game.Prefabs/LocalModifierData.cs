using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Unity.Entities;

namespace Game.Prefabs;

[InternalBufferCapacity(0)]
public struct LocalModifierData : IBufferElementData, ISerializable
{
	public LocalModifierType m_Type;

	public ModifierValueMode m_Mode;

	public ModifierRadiusCombineMode m_RadiusCombineMode;

	public Bounds1 m_Delta;

	public Bounds1 m_Radius;

	public LocalModifierData(LocalModifierType type, ModifierValueMode mode, ModifierRadiusCombineMode radiusMode, Bounds1 delta, Bounds1 radius)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		m_Type = type;
		m_Mode = mode;
		m_RadiusCombineMode = radiusMode;
		m_Delta = delta;
		m_Radius = radius;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float min = m_Delta.min;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(min);
		float max = m_Delta.max;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(max);
		float min2 = m_Radius.min;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(min2);
		float max2 = m_Radius.max;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(max2);
		byte num = (byte)m_Type;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		byte num2 = (byte)m_Mode;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
		byte num3 = (byte)m_RadiusCombineMode;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num3);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float min = ref m_Delta.min;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref min);
		ref float max = ref m_Delta.max;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref max);
		ref float min2 = ref m_Radius.min;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref min2);
		ref float max2 = ref m_Radius.max;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref max2);
		byte type = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref type);
		byte mode = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref mode);
		byte radiusCombineMode = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref radiusCombineMode);
		m_Type = (LocalModifierType)type;
		m_Mode = (ModifierValueMode)mode;
		m_RadiusCombineMode = (ModifierRadiusCombineMode)radiusCombineMode;
	}
}
