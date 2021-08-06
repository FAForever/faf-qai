using System;

namespace Faforever.Qai.Core.Commands.Authorization
{
	[System.AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class RequireFafStaffAttribute : Attribute
	{

	}
}
