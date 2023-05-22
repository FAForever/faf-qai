using Faforever.Qai.Core.Operations.PatchNotes;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Commands.Dual.Info
{
    public class PatchNotesResult
    {

    }

    public class PatchNotesCommand : DualCommandModule
    {
        private IFetchPatchNotesLinkOperation operation;

        public PatchNotesCommand(IFetchPatchNotesLinkOperation operation)
        {
            this.operation = operation;
        }

        [Command("patchnotes")]
        [Description("Get a link to the latest patch notes")]
        public async Task PatchNotesCommandAsync([Remainder] string? version = null)
        {
            var link = await operation.GetPatchNotesLinkAsync(version);

            if (link is null)
                await Context.ReplyAsync("Patch notes not found.");
            else await Context.ReplyAsync(link.Url);
        }
    }
}
