using System.Threading.Tasks;

namespace Faforever.Qai.Core.Operations.PatchNotes
{
    public interface IFetchPatchNotesLinkOperation
    {
        Task<PatchNoteLink?> GetPatchNotesLinkAsync(string? version = null);
    }
}