using Swashbuckle.AspNetCore.SwaggerUI;
using System;

namespace Marsman.ReallySimpleDocumentation
{
    public class SwaggerUiOptions : IUIOptions
    {
        private const string DefaultCss = "swaggerui-custom.css";
        private const string DefaultJs = "swaggerui-custom.js";

        public SwaggerUiWikiOptions WikiOptions { get; } = new SwaggerUiWikiOptions();

        public string FaviconUrl { get; set; }
        public string LogoUrl { get; set; }
        public string LogoAltText { get; set; }
        public string LogoBackgroundColor { get; set; }
        public string JavascriptUrl { get; set; } = DefaultJs;
        public string CssUrl { get; set; } = DefaultCss;
        public Action<SwaggerUIOptions> SwaggerUIOptions { get; set; }
        public bool ServeDefaultJavascript { get; private set; } = true;
        public bool ServeDefaultCss { get; private set; } = true;
    }
}
