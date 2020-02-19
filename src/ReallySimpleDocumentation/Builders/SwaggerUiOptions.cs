using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;

namespace Marsman.ReallySimpleDocumentation
{
    public class SwaggerUiOptions : IUIOptions
    {
        private const string DefaultCss = "swaggerui-reallysimpledocs.css";
        private const string DefaultJs = "swaggerui-reallysimpledocs.js";

        public SwaggerUIWikiOptions WikiOptions { get; } = new SwaggerUIWikiOptions();

        public string FaviconUrl { get; set; }
        public string LogoUrl { get; set; }
        public string LogoAltText { get; set; }
        public string LogoBackgroundColor { get; set; }
        public string DefaultJavascriptRoute { get; set; } = DefaultJs;
        public string DefaultCssRoute { get; set; } = DefaultCss;
        public Action<SwaggerUIOptions> SwaggerUIOptions { get; set; }
        public bool ServeDefaultJavascript { get; set; } = true;
        public bool ServeDefaultCss { get; set; } = true;
        public List<string> AdditionalStylesheets { get; } = new List<string>();
        public List<string> AdditionalJavascript { get; } = new List<string>();
    }
}
