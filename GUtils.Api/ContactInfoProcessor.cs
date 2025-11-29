namespace GUtils.Api;

using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

public class ContactInfoProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        context.Document.Info.Contact = new NSwag.OpenApiContact
        {
            Name = "Glen Wilkin",
            Url = "https://github.com/WilkinGlen/GUtils"
        };
        
        context.Document.Info.License = new NSwag.OpenApiLicense
        {
            Name = "MIT",
            Url = "https://opensource.org/licenses/MIT"
        };
    }
}