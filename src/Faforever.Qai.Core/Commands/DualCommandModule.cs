using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Commands.Context;

using Qmmands;

namespace Faforever.Qai.Core.Commands
{
	public class DualCommandModule : ModuleBase<CustomCommandContext>
	{
		[Command("dualtest")]
		public async Task TestDual()
			=> await DualCommandTest.Execute(Context);
	}
}
