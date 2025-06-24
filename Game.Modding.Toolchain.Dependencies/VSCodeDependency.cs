using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;

namespace Game.Modding.Toolchain.Dependencies;

public class VSCodeDependency : BaseIDEDependency
{
	public override string name => "VS Code";

	public override string icon => "Media/Toolchain/VSCode.svg";

	public override bool canBeInstalled => false;

	public override bool canBeUninstalled => false;

	public override string minVersion => "1.86";

	protected override async Task<string> GetIDEVersion(CancellationToken token)
	{
		string installedVersion = string.Empty;
		List<string> errorText = new List<string>();
		try
		{
			await Cli.Wrap("code").WithArguments("--version").WithStandardOutputPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
			{
				if (string.IsNullOrEmpty(installedVersion))
				{
					installedVersion = l;
				}
			}))
				.WithStandardErrorPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
				{
					errorText.Add(l);
				}))
				.WithValidation((CommandResultValidation)0)
				.ExecuteAsync(token)
				.ConfigureAwait(false);
		}
		catch (Win32Exception ex)
		{
			if (ex.ErrorCode != -2147467259)
			{
				ToolchainDependencyManager.log.Error((Exception)ex, (object)"Failed to get VSCode version");
			}
		}
		catch (Exception ex2)
		{
			ToolchainDependencyManager.log.Error(ex2, (object)"Failed to get VSCode version");
		}
		if (errorText.Count > 0)
		{
			IToolchainDependency.log.Warn((object)string.Join<string>('\n', (IEnumerable<string>)errorText));
		}
		return installedVersion;
	}
}
