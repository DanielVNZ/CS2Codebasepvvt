using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct AdditionalBuildingTerraformElement : IBufferElementData, ISerializable
{
	public Bounds2 m_Area;

	public float m_HeightOffset;

	public bool m_Circular;

	public bool m_DontRaise;

	public bool m_DontLower;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		float2 min = m_Area.min;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(min);
		float2 max = m_Area.max;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(max);
		float heightOffset = m_HeightOffset;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(heightOffset);
		bool circular = m_Circular;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(circular);
		bool dontRaise = m_DontRaise;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(dontRaise);
		bool dontLower = m_DontLower;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(dontLower);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		ref float2 min = ref m_Area.min;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref min);
		ref float2 max = ref m_Area.max;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref max);
		ref float heightOffset = ref m_HeightOffset;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref heightOffset);
		ref bool circular = ref m_Circular;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref circular);
		ref bool dontRaise = ref m_DontRaise;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref dontRaise);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.pillarTerrainModification)
		{
			ref bool dontLower = ref m_DontLower;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref dontLower);
		}
	}
}
