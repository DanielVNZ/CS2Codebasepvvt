using Colossal.Serialization.Entities;

namespace Game.Simulation;

public struct ZoneAmbiences : IStrideSerializable, ISerializable
{
	public float m_ResidentialLow;

	public float m_CommercialLow;

	public float m_Industrial;

	public float m_Agriculture;

	public float m_Forestry;

	public float m_Oil;

	public float m_Ore;

	public float m_OfficeLow;

	public float m_OfficeHigh;

	public float m_ResidentialMedium;

	public float m_ResidentialHigh;

	public float m_ResidentialMixed;

	public float m_CommercialHigh;

	public float m_ResidentialLowRent;

	public float m_Forest;

	public float m_WaterfrontLow;

	public float m_AquacultureLand;

	public float m_SeagullAmbience;

	public float GetAmbience(GroupAmbienceType type)
	{
		return type switch
		{
			GroupAmbienceType.ResidentialLow => m_ResidentialLow, 
			GroupAmbienceType.CommercialLow => m_CommercialLow, 
			GroupAmbienceType.Industrial => m_Industrial, 
			GroupAmbienceType.Agriculture => m_Agriculture, 
			GroupAmbienceType.Forestry => m_Forestry, 
			GroupAmbienceType.Oil => m_Oil, 
			GroupAmbienceType.Ore => m_Ore, 
			GroupAmbienceType.OfficeLow => m_OfficeLow, 
			GroupAmbienceType.OfficeHigh => m_OfficeHigh, 
			GroupAmbienceType.ResidentialMedium => m_ResidentialMedium, 
			GroupAmbienceType.ResidentialHigh => m_ResidentialHigh, 
			GroupAmbienceType.ResidentialMixed => m_ResidentialMixed, 
			GroupAmbienceType.CommercialHigh => m_CommercialHigh, 
			GroupAmbienceType.ResidentialLowRent => m_ResidentialLowRent, 
			GroupAmbienceType.Forest => m_Forest, 
			GroupAmbienceType.WaterfrontLow => m_WaterfrontLow, 
			GroupAmbienceType.AquacultureLand => m_AquacultureLand, 
			GroupAmbienceType.SeagullAmbience => m_SeagullAmbience, 
			_ => 0f, 
		};
	}

	public void AddAmbience(GroupAmbienceType type, float value)
	{
		switch (type)
		{
		case GroupAmbienceType.ResidentialLow:
			m_ResidentialLow += value;
			break;
		case GroupAmbienceType.CommercialLow:
			m_CommercialLow += value;
			break;
		case GroupAmbienceType.Industrial:
			m_Industrial += value;
			break;
		case GroupAmbienceType.Agriculture:
			m_Agriculture += value;
			break;
		case GroupAmbienceType.Forestry:
			m_Forestry += value;
			break;
		case GroupAmbienceType.Oil:
			m_Oil += value;
			break;
		case GroupAmbienceType.Ore:
			m_Ore += value;
			break;
		case GroupAmbienceType.OfficeLow:
			m_OfficeLow += value;
			break;
		case GroupAmbienceType.OfficeHigh:
			m_OfficeHigh += value;
			break;
		case GroupAmbienceType.ResidentialMedium:
			m_ResidentialMedium += value;
			break;
		case GroupAmbienceType.ResidentialHigh:
			m_ResidentialHigh += value;
			break;
		case GroupAmbienceType.ResidentialMixed:
			m_ResidentialMixed += value;
			break;
		case GroupAmbienceType.CommercialHigh:
			m_CommercialHigh += value;
			break;
		case GroupAmbienceType.ResidentialLowRent:
			m_ResidentialLowRent += value;
			break;
		case GroupAmbienceType.Forest:
			m_Forest += value;
			break;
		case GroupAmbienceType.WaterfrontLow:
			m_WaterfrontLow += value;
			break;
		case GroupAmbienceType.AquacultureLand:
			m_AquacultureLand += value;
			break;
		case GroupAmbienceType.SeagullAmbience:
			m_SeagullAmbience += value;
			break;
		case GroupAmbienceType.Traffic:
		case GroupAmbienceType.Rain:
		case GroupAmbienceType.NightForest:
			break;
		}
	}

	public static ZoneAmbiences operator +(ZoneAmbiences a, ZoneAmbiences b)
	{
		return new ZoneAmbiences
		{
			m_ResidentialLow = a.m_ResidentialLow + b.m_ResidentialLow,
			m_CommercialLow = a.m_CommercialLow + b.m_CommercialLow,
			m_Industrial = a.m_Industrial + b.m_Industrial,
			m_Agriculture = a.m_Agriculture + b.m_Agriculture,
			m_Forestry = a.m_Forestry + b.m_Forestry,
			m_Oil = a.m_Oil + b.m_Oil,
			m_Ore = a.m_Ore + b.m_Ore,
			m_OfficeLow = a.m_OfficeLow + b.m_OfficeLow,
			m_OfficeHigh = a.m_OfficeHigh + b.m_OfficeHigh,
			m_ResidentialMedium = a.m_ResidentialMedium + b.m_ResidentialMedium,
			m_ResidentialHigh = a.m_ResidentialHigh + b.m_ResidentialHigh,
			m_ResidentialMixed = a.m_ResidentialMixed + b.m_ResidentialMixed,
			m_CommercialHigh = a.m_CommercialHigh + b.m_CommercialHigh,
			m_ResidentialLowRent = a.m_ResidentialLowRent + b.m_ResidentialLowRent,
			m_Forest = a.m_Forest + b.m_Forest,
			m_WaterfrontLow = a.m_WaterfrontLow + b.m_WaterfrontLow,
			m_AquacultureLand = a.m_AquacultureLand + b.m_AquacultureLand,
			m_SeagullAmbience = a.m_SeagullAmbience + b.m_SeagullAmbience
		};
	}

	public static ZoneAmbiences operator /(ZoneAmbiences a, float b)
	{
		return new ZoneAmbiences
		{
			m_ResidentialLow = a.m_ResidentialLow / b,
			m_CommercialLow = a.m_CommercialLow / b,
			m_Industrial = a.m_Industrial / b,
			m_Agriculture = a.m_Agriculture / b,
			m_Forestry = a.m_Forestry / b,
			m_Oil = a.m_Oil / b,
			m_Ore = a.m_Ore / b,
			m_OfficeLow = a.m_OfficeLow / b,
			m_OfficeHigh = a.m_OfficeHigh / b,
			m_ResidentialMedium = a.m_ResidentialMedium / b,
			m_ResidentialHigh = a.m_ResidentialHigh / b,
			m_ResidentialMixed = a.m_ResidentialMixed / b,
			m_CommercialHigh = a.m_CommercialHigh / b,
			m_ResidentialLowRent = a.m_ResidentialLowRent / b,
			m_Forest = a.m_Forest / b,
			m_WaterfrontLow = a.m_WaterfrontLow / b,
			m_AquacultureLand = a.m_AquacultureLand / b,
			m_SeagullAmbience = a.m_SeagullAmbience / b
		};
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float residentialLow = m_ResidentialLow;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(residentialLow);
		float commercialHigh = m_CommercialHigh;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(commercialHigh);
		float industrial = m_Industrial;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(industrial);
		float agriculture = m_Agriculture;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(agriculture);
		float forestry = m_Forestry;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(forestry);
		float oil = m_Oil;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(oil);
		float ore = m_Ore;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(ore);
		float officeLow = m_OfficeLow;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(officeLow);
		float officeHigh = m_OfficeHigh;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(officeHigh);
		float residentialMedium = m_ResidentialMedium;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(residentialMedium);
		float residentialHigh = m_ResidentialHigh;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(residentialHigh);
		float residentialMixed = m_ResidentialMixed;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(residentialMixed);
		float commercialHigh2 = m_CommercialHigh;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(commercialHigh2);
		float residentialLowRent = m_ResidentialLowRent;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(residentialLowRent);
		float forest = m_Forest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(forest);
		float waterfrontLow = m_WaterfrontLow;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(waterfrontLow);
		float aquacultureLand = m_AquacultureLand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(aquacultureLand);
		float seagullAmbience = m_SeagullAmbience;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(seagullAmbience);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		ref float residentialLow = ref m_ResidentialLow;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref residentialLow);
		ref float commercialHigh = ref m_CommercialHigh;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref commercialHigh);
		ref float industrial = ref m_Industrial;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref industrial);
		ref float agriculture = ref m_Agriculture;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref agriculture);
		ref float forestry = ref m_Forestry;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref forestry);
		ref float oil = ref m_Oil;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref oil);
		ref float ore = ref m_Ore;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref ore);
		Context context = ((IReader)reader).context;
		if (!(((Context)(ref context)).version > Version.zoneAmbience))
		{
			return;
		}
		ref float officeLow = ref m_OfficeLow;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref officeLow);
		ref float officeHigh = ref m_OfficeHigh;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref officeHigh);
		ref float residentialMedium = ref m_ResidentialMedium;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref residentialMedium);
		ref float residentialHigh = ref m_ResidentialHigh;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref residentialHigh);
		ref float residentialMixed = ref m_ResidentialMixed;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref residentialMixed);
		ref float commercialHigh2 = ref m_CommercialHigh;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref commercialHigh2);
		ref float residentialLowRent = ref m_ResidentialLowRent;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref residentialLowRent);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version > Version.forestAmbience)
		{
			ref float forest = ref m_Forest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref forest);
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version > Version.waterfrontAmbience)
			{
				ref float waterfrontLow = ref m_WaterfrontLow;
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref waterfrontLow);
			}
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.forestAmbientFix)
		{
			m_Forest *= 0.0625f;
		}
		context = ((IReader)reader).context;
		ContextFormat format = ((Context)(ref context)).format;
		if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.AquacultureLandAmbience))
		{
			ref float aquacultureLand = ref m_AquacultureLand;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref aquacultureLand);
		}
		context = ((IReader)reader).context;
		format = ((Context)(ref context)).format;
		if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.SeagullAmbience))
		{
			ref float seagullAmbience = ref m_SeagullAmbience;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref seagullAmbience);
		}
	}

	public int GetStride(Context context)
	{
		return 72;
	}
}
