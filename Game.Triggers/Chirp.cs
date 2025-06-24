using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Triggers;

public struct Chirp : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Sender;

	public uint m_CreationFrame;

	public uint m_Likes;

	public uint m_TargetLikes;

	public uint m_InactiveFrame;

	public int m_ViralFactor;

	public float m_ContinuousFactor;

	public ChirpFlags m_Flags;

	public Chirp(Entity sender, uint creationFrame)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Sender = sender;
		m_CreationFrame = creationFrame;
		m_Likes = 0u;
		m_Flags = (ChirpFlags)0;
		m_TargetLikes = 0u;
		m_InactiveFrame = 0u;
		m_ViralFactor = 1;
		m_ContinuousFactor = 0.2f;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity sender = m_Sender;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(sender);
		uint creationFrame = m_CreationFrame;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(creationFrame);
		uint likes = m_Likes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(likes);
		ChirpFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
		uint targetLikes = m_TargetLikes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetLikes);
		uint inactiveFrame = m_InactiveFrame;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(inactiveFrame);
		int viralFactor = m_ViralFactor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(viralFactor);
		float continuousFactor = m_ContinuousFactor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(continuousFactor);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		ref Entity sender = ref m_Sender;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref sender);
		ref uint creationFrame = ref m_CreationFrame;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref creationFrame);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.chirpLikes)
		{
			ref uint likes = ref m_Likes;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref likes);
			byte flags = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (ChirpFlags)flags;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.randomChirpLikes)
		{
			ref uint targetLikes = ref m_TargetLikes;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetLikes);
			ref uint inactiveFrame = ref m_InactiveFrame;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref inactiveFrame);
			ref int viralFactor = ref m_ViralFactor;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref viralFactor);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.continuousChirpLikes)
		{
			ref float continuousFactor = ref m_ContinuousFactor;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref continuousFactor);
		}
	}
}
