using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

public struct NetGeometryData : IComponentData, IQueryTypeParameter, ISerializable
{
	public EntityArchetype m_NodeCompositionArchetype;

	public EntityArchetype m_EdgeCompositionArchetype;

	public Entity m_AggregateType;

	public Entity m_StyleType;

	public Bounds1 m_DefaultHeightRange;

	public Bounds1 m_ElevatedHeightRange;

	public Bounds1 m_DefaultSurfaceHeight;

	public Bounds1 m_EdgeLengthRange;

	public Layer m_MergeLayers;

	public Layer m_IntersectLayers;

	public GeometryFlags m_Flags;

	public float m_DefaultWidth;

	public float m_ElevatedWidth;

	public float m_ElevatedLength;

	public float m_MinNodeOffset;

	public float m_ElevationLimit;

	public float m_MaxSlopeSteepness;

	public float m_Hanging;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float min = m_DefaultHeightRange.min;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(min);
		float max = m_DefaultHeightRange.max;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(max);
		float min2 = m_ElevatedHeightRange.min;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(min2);
		float max2 = m_ElevatedHeightRange.max;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(max2);
		float min3 = m_DefaultSurfaceHeight.min;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(min3);
		float max3 = m_DefaultSurfaceHeight.max;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(max3);
		float min4 = m_EdgeLengthRange.min;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(min4);
		float max4 = m_EdgeLengthRange.max;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(max4);
		Layer mergeLayers = m_MergeLayers;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)mergeLayers);
		Layer intersectLayers = m_IntersectLayers;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)intersectLayers);
		GeometryFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
		float defaultWidth = m_DefaultWidth;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(defaultWidth);
		float elevatedWidth = m_ElevatedWidth;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(elevatedWidth);
		float minNodeOffset = m_MinNodeOffset;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(minNodeOffset);
		float elevationLimit = m_ElevationLimit;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(elevationLimit);
		float maxSlopeSteepness = m_MaxSlopeSteepness;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxSlopeSteepness);
		float hanging = m_Hanging;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(hanging);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float min = ref m_DefaultHeightRange.min;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref min);
		ref float max = ref m_DefaultHeightRange.max;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref max);
		ref float min2 = ref m_ElevatedHeightRange.min;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref min2);
		ref float max2 = ref m_ElevatedHeightRange.max;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref max2);
		ref float min3 = ref m_DefaultSurfaceHeight.min;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref min3);
		ref float max3 = ref m_DefaultSurfaceHeight.max;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref max3);
		ref float min4 = ref m_EdgeLengthRange.min;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref min4);
		ref float max4 = ref m_EdgeLengthRange.max;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref max4);
		uint mergeLayers = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref mergeLayers);
		uint intersectLayers = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref intersectLayers);
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		ref float defaultWidth = ref m_DefaultWidth;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref defaultWidth);
		ref float elevatedWidth = ref m_ElevatedWidth;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref elevatedWidth);
		ref float minNodeOffset = ref m_MinNodeOffset;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref minNodeOffset);
		ref float elevationLimit = ref m_ElevationLimit;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref elevationLimit);
		ref float maxSlopeSteepness = ref m_MaxSlopeSteepness;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxSlopeSteepness);
		ref float hanging = ref m_Hanging;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref hanging);
		m_MergeLayers = (Layer)mergeLayers;
		m_IntersectLayers = (Layer)intersectLayers;
		m_Flags = (GeometryFlags)flags;
		m_ElevatedLength = m_EdgeLengthRange.max;
	}
}
