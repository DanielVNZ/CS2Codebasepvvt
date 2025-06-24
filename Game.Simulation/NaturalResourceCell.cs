using Colossal.Serialization.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public struct NaturalResourceCell : IStrideSerializable, ISerializable
{
	public NaturalResourceAmount m_Fertility;

	public NaturalResourceAmount m_Ore;

	public NaturalResourceAmount m_Oil;

	public NaturalResourceAmount m_Fish;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		NaturalResourceAmount fertility = m_Fertility;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<NaturalResourceAmount>(fertility);
		NaturalResourceAmount ore = m_Ore;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<NaturalResourceAmount>(ore);
		NaturalResourceAmount oil = m_Oil;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<NaturalResourceAmount>(oil);
		NaturalResourceAmount fish = m_Fish;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<NaturalResourceAmount>(fish);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ref NaturalResourceAmount fertility = ref m_Fertility;
		((IReader)reader/*cast due to .constrained prefix*/).Read<NaturalResourceAmount>(ref fertility);
		ref NaturalResourceAmount ore = ref m_Ore;
		((IReader)reader/*cast due to .constrained prefix*/).Read<NaturalResourceAmount>(ref ore);
		ref NaturalResourceAmount oil = ref m_Oil;
		((IReader)reader/*cast due to .constrained prefix*/).Read<NaturalResourceAmount>(ref oil);
		Context context = ((IReader)reader).context;
		ContextFormat format = ((Context)(ref context)).format;
		if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.FishResource))
		{
			ref NaturalResourceAmount fish = ref m_Fish;
			((IReader)reader/*cast due to .constrained prefix*/).Read<NaturalResourceAmount>(ref fish);
		}
	}

	public int GetStride(Context context)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return m_Fertility.GetStride(context) + m_Ore.GetStride(context) + m_Oil.GetStride(context);
	}

	public float4 GetBaseResources()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return new float4((float)(int)m_Fertility.m_Base, (float)(int)m_Ore.m_Base, (float)(int)m_Oil.m_Base, (float)(int)m_Fish.m_Base);
	}

	public float4 GetUsedResources()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return new float4((float)(int)m_Fertility.m_Used, (float)(int)m_Ore.m_Used, (float)(int)m_Oil.m_Used, (float)(int)m_Oil.m_Used);
	}
}
