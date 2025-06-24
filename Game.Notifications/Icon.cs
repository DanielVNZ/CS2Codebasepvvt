using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Notifications;

public struct Icon : IComponentData, IQueryTypeParameter, IEquatable<Icon>, ISerializable
{
	public float3 m_Location;

	public IconPriority m_Priority;

	public IconClusterLayer m_ClusterLayer;

	public IconFlags m_Flags;

	public int m_ClusterIndex;

	public bool Equals(Icon other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((float3)(ref m_Location)).Equals(other.m_Location) & (m_Priority == other.m_Priority) & (m_ClusterLayer == other.m_ClusterLayer) & (m_Flags == other.m_Flags);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_Location)/*cast due to .constrained prefix*/).GetHashCode();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		float3 location = m_Location;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(location);
		IconPriority priority = m_Priority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)priority);
		IconClusterLayer clusterLayer = m_ClusterLayer;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)clusterLayer);
		IconFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		ref float3 location = ref m_Location;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref location);
		byte priority = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref priority);
		m_Priority = (IconPriority)priority;
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.iconClusteringData)
		{
			byte clusterLayer = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref clusterLayer);
			byte flags = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_ClusterLayer = (IconClusterLayer)clusterLayer;
			m_Flags = (IconFlags)flags;
		}
	}
}
