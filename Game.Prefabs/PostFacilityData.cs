using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct PostFacilityData : IComponentData, IQueryTypeParameter, ICombineData<PostFacilityData>, ISerializable
{
	public int m_PostVanCapacity;

	public int m_PostTruckCapacity;

	public int m_MailCapacity;

	public int m_SortingRate;

	public void Combine(PostFacilityData otherData)
	{
		m_PostVanCapacity += otherData.m_PostVanCapacity;
		m_PostTruckCapacity += otherData.m_PostTruckCapacity;
		m_MailCapacity += otherData.m_MailCapacity;
		m_SortingRate += otherData.m_SortingRate;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int postVanCapacity = m_PostVanCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(postVanCapacity);
		int postTruckCapacity = m_PostTruckCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(postTruckCapacity);
		int mailCapacity = m_MailCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(mailCapacity);
		int sortingRate = m_SortingRate;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(sortingRate);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int postVanCapacity = ref m_PostVanCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref postVanCapacity);
		ref int postTruckCapacity = ref m_PostTruckCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref postTruckCapacity);
		ref int mailCapacity = ref m_MailCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref mailCapacity);
		ref int sortingRate = ref m_SortingRate;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref sortingRate);
	}
}
