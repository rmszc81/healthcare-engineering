using System.Threading.Tasks;

namespace Healthcare.Engineering.Services.Interfaces;

public interface IDocumentService
{
    Task SyncDocumentsFromExternalSource(string email);
}