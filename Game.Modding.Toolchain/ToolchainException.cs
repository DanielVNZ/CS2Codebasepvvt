using System;

namespace Game.Modding.Toolchain;

public class ToolchainException : Exception
{
	public IToolchainDependency source { get; }

	public ToolchainError error { get; }

	public ToolchainException(ToolchainError error, IToolchainDependency source, string message = null, Exception innerException = null)
		: base(message, innerException)
	{
		this.source = source;
		this.error = error;
	}

	public ToolchainException(ToolchainError status, IToolchainDependency source, Exception innerException)
		: this(status, source, string.Empty, innerException)
	{
	}
}
