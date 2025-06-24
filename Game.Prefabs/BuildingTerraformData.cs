using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct BuildingTerraformData : IComponentData, IQueryTypeParameter, ISerializable
{
	public float3 m_FlatX0;

	public float3 m_FlatZ0;

	public float3 m_FlatX1;

	public float3 m_FlatZ1;

	public float4 m_Smooth;

	public float m_HeightOffset;

	public bool m_DontRaise;

	public bool m_DontLower;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		float3 flatX = m_FlatX0;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(flatX);
		float3 flatZ = m_FlatZ0;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(flatZ);
		float3 flatX2 = m_FlatX1;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(flatX2);
		float3 flatZ2 = m_FlatZ1;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(flatZ2);
		float4 smooth = m_Smooth;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(smooth);
		float heightOffset = m_HeightOffset;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(heightOffset);
		bool dontRaise = m_DontRaise;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(dontRaise);
		bool dontLower = m_DontLower;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(dontLower);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		ref float3 flatX = ref m_FlatX0;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flatX);
		ref float3 flatZ = ref m_FlatZ0;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flatZ);
		ref float3 flatX2 = ref m_FlatX1;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flatX2);
		ref float3 flatZ2 = ref m_FlatZ1;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flatZ2);
		ref float4 smooth = ref m_Smooth;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref smooth);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.pillarTerrainModification)
		{
			ref float heightOffset = ref m_HeightOffset;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref heightOffset);
			ref bool dontRaise = ref m_DontRaise;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref dontRaise);
			ref bool dontLower = ref m_DontLower;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref dontLower);
		}
	}
}
