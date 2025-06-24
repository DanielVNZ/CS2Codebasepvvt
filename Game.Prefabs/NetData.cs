using Colossal.Serialization.Entities;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

public struct NetData : IComponentData, IQueryTypeParameter, ISerializable
{
	public EntityArchetype m_NodeArchetype;

	public EntityArchetype m_EdgeArchetype;

	public Layer m_RequiredLayers;

	public Layer m_ConnectLayers;

	public Layer m_LocalConnectLayers;

	public CompositionFlags.General m_GeneralFlagMask;

	public CompositionFlags.Side m_SideFlagMask;

	public float m_NodePriority;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		Layer requiredLayers = m_RequiredLayers;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)requiredLayers);
		Layer connectLayers = m_ConnectLayers;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)connectLayers);
		Layer localConnectLayers = m_LocalConnectLayers;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)localConnectLayers);
		float nodePriority = m_NodePriority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(nodePriority);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		uint requiredLayers = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref requiredLayers);
		uint connectLayers = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref connectLayers);
		uint localConnectLayers = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref localConnectLayers);
		ref float nodePriority = ref m_NodePriority;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref nodePriority);
		m_RequiredLayers = (Layer)requiredLayers;
		m_ConnectLayers = (Layer)connectLayers;
		m_LocalConnectLayers = (Layer)localConnectLayers;
	}
}
