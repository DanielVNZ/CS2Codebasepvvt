using System;
using Colossal;
using Colossal.IO.AssetDatabase;
using Colossal.PSI.Common;
using Colossal.UI.Binding;
using Game.CinematicCamera;
using Game.UI.Menu;

namespace Game.Assets;

public class CinematicCameraAsset : Metadata<CinematicCameraSequence>, IJsonWritable
{
	public const string kExtension = ".CinematicCamera";

	public static readonly Func<string> kPersistentLocation = () => "CinematicCamera/" + PlatformManager.instance.userSpecificPath;

	private static readonly string kCloudTargetProperty = "cloudTarget";

	private static readonly string kReadOnlyProperty = "isReadOnly";

	public unsafe void Write(IJsonWriter writer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		SourceMeta meta = ((AssetData)this).GetMeta();
		writer.TypeBegin("CinematicCameraAsset");
		writer.PropertyName("name");
		writer.Write(((AssetData)this).name);
		writer.PropertyName("guid");
		Hash128 guid = ((AssetData)this).id.guid;
		writer.Write(((object)(*(Hash128*)(&guid))/*cast due to .constrained prefix*/).ToString());
		writer.PropertyName("identifier");
		writer.Write(((AssetData)this).identifier);
		writer.PropertyName(kCloudTargetProperty);
		writer.Write(MenuHelpers.GetSanitizedCloudTarget(meta.remoteStorageSourceName).name);
		writer.PropertyName(kReadOnlyProperty);
		writer.Write(!meta.belongsToCurrentUser);
		writer.TypeEnd();
	}
}
