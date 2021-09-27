namespace LSDView.Controllers.Interface
{
    public interface IFileFormatController<TFile, out TDocument>
    {
        TFile Load(string path);
        TDocument CreateDocument(TFile file);
    }
}
