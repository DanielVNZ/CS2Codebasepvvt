using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Tools;

public struct EditorContainer : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Prefab;

	public float3 m_Scale;

	public float m_Intensity;

	public int m_GroupIndex;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity prefab = m_Prefab;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(prefab);
		float3 scale = m_Scale;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(scale);
		float intensity = m_Intensity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(intensity);
		int groupIndex = m_GroupIndex;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(groupIndex);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ref Entity prefab = ref m_Prefab;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref prefab);
		ref float3 scale = ref m_Scale;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref scale);
		ref float intensity = ref m_Intensity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref intensity);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.editorContainerGroupIndex)
		{
			ref int groupIndex = ref m_GroupIndex;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref groupIndex);
		}
		else
		{
			m_GroupIndex = -1;
		}
	}
}
