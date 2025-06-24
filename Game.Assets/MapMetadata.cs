using System;
using Colossal.IO.AssetDatabase;
using Colossal.PSI.Common;

namespace Game.Assets;

public class MapMetadata : Metadata<MapInfo>
{
	public const string kExtension = ".MapMetadata";

	public static readonly Func<string> kPersistentLocation = () => "Maps/" + PlatformManager.instance.userSpecificPath;

	protected override void OnPostLoad()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((AssetData)this).state == 4 && ((IDataSourceAccessor)((AssetData)this).database).dataSource.Contains(Identifier.op_Implicit(((AssetData)this).id)))
		{
			base.target.id = ((AssetData)this).identifier;
			SourceMeta meta = ((AssetData)this).GetMeta();
			base.target.metaData = this;
			base.target.isReadonly = !meta.belongsToCurrentUser;
			base.target.cloudTarget = meta.remoteStorageSourceName;
		}
	}
}
