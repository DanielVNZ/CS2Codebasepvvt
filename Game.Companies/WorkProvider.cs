using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Companies;

public struct WorkProvider : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_MaxWorkers;

	public short m_UneducatedCooldown;

	public short m_EducatedCooldown;

	public Entity m_UneducatedNotificationEntity;

	public Entity m_EducatedNotificationEntity;

	public short m_EfficiencyCooldown;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		int maxWorkers = m_MaxWorkers;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxWorkers);
		short uneducatedCooldown = m_UneducatedCooldown;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(uneducatedCooldown);
		short educatedCooldown = m_EducatedCooldown;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(educatedCooldown);
		Entity uneducatedNotificationEntity = m_UneducatedNotificationEntity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(uneducatedNotificationEntity);
		Entity educatedNotificationEntity = m_EducatedNotificationEntity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(educatedNotificationEntity);
		short efficiencyCooldown = m_EfficiencyCooldown;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(efficiencyCooldown);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		ref int maxWorkers = ref m_MaxWorkers;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxWorkers);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.companyNotifications)
		{
			ref short uneducatedCooldown = ref m_UneducatedCooldown;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref uneducatedCooldown);
			ref short educatedCooldown = ref m_EducatedCooldown;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref educatedCooldown);
			ref Entity uneducatedNotificationEntity = ref m_UneducatedNotificationEntity;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref uneducatedNotificationEntity);
			ref Entity educatedNotificationEntity = ref m_EducatedNotificationEntity;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref educatedNotificationEntity);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.buildingEfficiencyRework)
		{
			ref short efficiencyCooldown = ref m_EfficiencyCooldown;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref efficiencyCooldown);
		}
	}
}
