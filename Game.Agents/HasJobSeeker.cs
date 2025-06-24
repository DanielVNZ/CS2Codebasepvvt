using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Agents;

public struct HasJobSeeker : IComponentData, IQueryTypeParameter, ISerializable, IEnableableComponent
{
	public Entity m_Seeker;

	public uint m_LastJobSeekFrameIndex;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity seeker = m_Seeker;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(seeker);
		uint lastJobSeekFrameIndex = m_LastJobSeekFrameIndex;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lastJobSeekFrameIndex);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.seekerReferences)
		{
			ref Entity seeker = ref m_Seeker;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref seeker);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.findJobOptimize)
		{
			ref uint lastJobSeekFrameIndex = ref m_LastJobSeekFrameIndex;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref lastJobSeekFrameIndex);
		}
	}
}
