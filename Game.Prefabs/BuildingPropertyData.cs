using Colossal.Serialization.Entities;
using Game.Economy;
using Game.Zones;
using Unity.Entities;

namespace Game.Prefabs;

public struct BuildingPropertyData : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_ResidentialProperties;

	public Resource m_AllowedSold;

	public Resource m_AllowedInput;

	public Resource m_AllowedManufactured;

	public Resource m_AllowedStored;

	public float m_SpaceMultiplier;

	public int CountProperties(AreaType areaType)
	{
		switch (areaType)
		{
		case AreaType.Residential:
			return m_ResidentialProperties;
		case AreaType.Commercial:
			if (m_AllowedSold == Resource.NoResource)
			{
				return 0;
			}
			return 1;
		case AreaType.Industrial:
			if (m_AllowedStored != Resource.NoResource)
			{
				return 1;
			}
			if (m_AllowedManufactured == Resource.NoResource)
			{
				return 0;
			}
			return 1;
		default:
			return 0;
		}
	}

	public int CountProperties()
	{
		return CountProperties(AreaType.Residential) + CountProperties(AreaType.Commercial) + CountProperties(AreaType.Industrial);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int residentialProperties = m_ResidentialProperties;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(residentialProperties);
		float spaceMultiplier = m_SpaceMultiplier;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(spaceMultiplier);
		Resource allowedSold = m_AllowedSold;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((ulong)allowedSold);
		Resource allowedManufactured = m_AllowedManufactured;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((ulong)allowedManufactured);
		Resource allowedStored = m_AllowedStored;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((ulong)allowedStored);
		Resource allowedInput = m_AllowedInput;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((ulong)allowedInput);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		ref int residentialProperties = ref m_ResidentialProperties;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref residentialProperties);
		ref float spaceMultiplier = ref m_SpaceMultiplier;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref spaceMultiplier);
		ulong allowedSold = default(ulong);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref allowedSold);
		ulong allowedManufactured = default(ulong);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref allowedManufactured);
		ulong allowedStored = default(ulong);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref allowedStored);
		m_AllowedSold = (Resource)allowedSold;
		m_AllowedManufactured = (Resource)allowedManufactured;
		m_AllowedStored = (Resource)allowedStored;
		Context context = ((IReader)reader).context;
		ContextFormat format = ((Context)(ref context)).format;
		if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.BpPrefabData))
		{
			ulong allowedInput = default(ulong);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref allowedInput);
			m_AllowedInput = (Resource)allowedInput;
		}
	}
}
