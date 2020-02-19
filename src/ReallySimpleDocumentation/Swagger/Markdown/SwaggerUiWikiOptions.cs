using System;
using System.IO;

namespace Marsman.ReallySimpleDocumentation
{
    public class SwaggerUIWikiOptions
    {
        private const string SwaggerContentTemplateResourceName = "Marsman.ReallySimpleDocumentation.Templates.SwaggerContentTemplate.html";
        private const string SwaggerNavBarItemTemplateResourceName = "Marsman.ReallySimpleDocumentation.Templates.SwaggerNavBarItemTemplate.html";
        private const string SwaggerNavBarHeadingTemplateResourceName = "Marsman.ReallySimpleDocumentation.Templates.SwaggerNavBarHeadingTemplate.html";
        private const string SwaggerWikiTemplateResourceName = "Marsman.ReallySimpleDocumentation.Templates.SwaggerWikiTemplate.html";
        private static Lazy<SwaggerUIWikiOptions> defaults = new Lazy<SwaggerUIWikiOptions>(() =>
        {
            var assembly = typeof(SwaggerUIWikiOptions).Assembly;
            var swaggerContentTemplate = string.Empty;
            using (var stream = assembly.GetManifestResourceStream(SwaggerContentTemplateResourceName))
            using (var reader = new StreamReader(stream))
            {
                swaggerContentTemplate = reader.ReadToEnd();
            }

            var navHeaderTemplate = string.Empty;
            using (var stream = assembly.GetManifestResourceStream(SwaggerNavBarHeadingTemplateResourceName))
            using (var reader = new StreamReader(stream))
            {
                navHeaderTemplate = reader.ReadToEnd();
            }

            var navItemTemplate = string.Empty;
            using (var stream = assembly.GetManifestResourceStream(SwaggerNavBarItemTemplateResourceName))
            using (var reader = new StreamReader(stream))
            {
                navItemTemplate = reader.ReadToEnd();
            }

            var swaggerWikiTemplate = string.Empty;
            using (var stream = assembly.GetManifestResourceStream(SwaggerWikiTemplateResourceName))
            using (var reader = new StreamReader(stream))
            {
                swaggerWikiTemplate = reader.ReadToEnd();
            }

            return new SwaggerUIWikiOptions
            {
                MainTemplate = swaggerWikiTemplate,
                NavBarHeadingTemplate = navHeaderTemplate,
                NavBarItemTemplate = navItemTemplate,
                ContentTemplate = swaggerContentTemplate
            };
        });

        private string mainTemplate;
        private string navHeadingTemplate;
        private string navItemTemplate;
        private string contentTemplate;

        public string NavBarHeadingTemplate { get => navHeadingTemplate ?? defaults.Value.NavBarHeadingTemplate; set => navHeadingTemplate = value; }
        public string NavBarItemTemplate { get => navItemTemplate ?? defaults.Value.NavBarItemTemplate; set => navItemTemplate = value; }
        public string ContentTemplate { get => contentTemplate ?? defaults.Value.ContentTemplate; set => contentTemplate = value; }
        public string MainTemplate { get => mainTemplate ?? defaults.Value.MainTemplate; set => mainTemplate = value; }
    }
}
