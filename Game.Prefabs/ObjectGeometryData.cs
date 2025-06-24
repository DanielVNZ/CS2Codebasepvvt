using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Objects;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct ObjectGeometryData : IComponentData, IQueryTypeParameter, ISerializable
{
	public Bounds3 m_Bounds;

	public float3 m_Size;

	public float3 m_Pivot;

	public float3 m_LegSize;

	public float2 m_LegOffset;

	public GeometryFlags m_Flags;

	public int m_MinLod;

	public MeshLayer m_Layers;

	public ObjectRequirementFlags m_SubObjectMask;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		ref float3 min = ref m_Bounds.min;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref min);
		ref float3 max = ref m_Bounds.max;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref max);
		ref float3 size = ref m_Size;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref size);
		ref float3 pivot = ref m_Pivot;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref pivot);
		ref float3 legSize = ref m_LegSize;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref legSize);
		Context context = ((IReader)reader).context;
		ContextFormat format = ((Context)(ref context)).format;
		if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.StandingLegOffset))
		{
			ref float2 legOffset = ref m_LegOffset;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref legOffset);
		}
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_Flags = (GeometryFlags)flags;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		float3 min = m_Bounds.min;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(min);
		float3 max = m_Bounds.max;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(max);
		float3 size = m_Size;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(size);
		float3 pivot = m_Pivot;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(pivot);
		float3 legSize = m_LegSize;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(legSize);
		float2 legOffset = m_LegOffset;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(legOffset);
		GeometryFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
	}
}
