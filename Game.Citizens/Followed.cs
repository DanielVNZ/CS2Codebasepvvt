using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Citizens;

public struct Followed : IComponentData, IQueryTypeParameter, ISerializable
{
	public uint m_Priority;

	public bool m_StartedFollowingAsChild;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		uint priority = m_Priority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(priority);
		bool startedFollowingAsChild = m_StartedFollowingAsChild;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(startedFollowingAsChild);
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
		if (((Context)(ref context)).version >= Version.localizationIndex)
		{
			ref uint priority = ref m_Priority;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref priority);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.stalkerAchievement)
		{
			ref bool startedFollowingAsChild = ref m_StartedFollowingAsChild;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref startedFollowingAsChild);
		}
	}
}
