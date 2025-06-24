using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Agents;

public struct PropertySeeker : IComponentData, IQueryTypeParameter, ISerializable, IEnableableComponent
{
	public Entity m_TargetProperty;

	public Entity m_BestProperty;

	public float m_BestPropertyScore;

	public uint m_LastPropertySeekFrame;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity targetProperty = m_TargetProperty;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetProperty);
		Entity bestProperty = m_BestProperty;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(bestProperty);
		float bestPropertyScore = m_BestPropertyScore;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(bestPropertyScore);
		uint lastPropertySeekFrame = m_LastPropertySeekFrame;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lastPropertySeekFrame);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ref Entity targetProperty = ref m_TargetProperty;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetProperty);
		ref Entity bestProperty = ref m_BestProperty;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref bestProperty);
		ref float bestPropertyScore = ref m_BestPropertyScore;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref bestPropertyScore);
		Context context = ((IReader)reader).context;
		ContextFormat format = ((Context)(ref context)).format;
		if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.HomelessAndWorkerFix))
		{
			ref uint lastPropertySeekFrame = ref m_LastPropertySeekFrame;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref lastPropertySeekFrame);
		}
		else
		{
			byte b = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref b);
		}
	}
}
