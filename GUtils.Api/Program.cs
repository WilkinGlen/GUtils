using FastEndpoints;
using FastEndpoints.Swagger;
using GUtils.Api;
using Scalar.AspNetCore;

var bld = WebApplication.CreateBuilder();
bld.Services
   .AddFastEndpoints()
   .SwaggerDocument(o =>
   {
       o.DocumentSettings = s =>
       {
           s.Title = "GUtils ClassCopier API";
           s.Version = "v1";
           s.Description = """
               API for the GUtils ClassCopier utility
               
               # Author
               **Glen Wilkin**

               # My Tag
               This is text added in the DocumentSettings.Description
               
               # Links
               - [GitHub Repository](https://github.com/WilkinGlen/GUtils)
               - [License: MIT](https://opensource.org/licenses/MIT)
               """;
           s.DocumentProcessors.Add(new ContactInfoProcessor());
       };
       o.ShortSchemaNames = true;
   });

var app = bld.Build();

app.UseFastEndpoints()
   .UseSwaggerGen(uiConfig: c =>
   {
       c.DocumentTitle = "GUtils ClassCopier API";
       c.DocExpansion = "list";
       c.DefaultModelsExpandDepth = 1;
   });

// Keep Scalar available at /scalar/v1
app.MapScalarApiReference(o =>
{
    o.Title = "GUtils ClassCopier API";
    o.Theme = ScalarTheme.Purple;
    o.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
    o.ShowSidebar = true;
});

app.Run();
