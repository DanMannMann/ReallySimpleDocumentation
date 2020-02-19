using System.Collections.Generic;

namespace Marsman.ReallySimpleDocumentation
{
    public class RedocUiOptions : IUIOptions
    {
        private const string DefaultCss = "redoc-reallysimpledocs.css";
        private const string DefaultJs = "redoc-reallysimpledocs.js";

        public string ApiReferenceGroupName { get; set; } = "API Reference";
        public List<string> AdditionalControllersToInclude { get; } = new List<string>();
        public string FaviconUrl { get; set; }
        public string LogoUrl { get; set; }
        public string LogoAltText { get; set; }
        public string LogoBackgroundColor { get; set; }
        public string DefaultJavascriptRoute { get; set; } = DefaultJs;
        public string DefaultCssRoute { get; set; } = DefaultCss;
        public bool ServeDefaultJavascript { get; set; } = true;
        public bool ServeDefaultCss { get; set; } = true;
        public bool ServeDefaultHtml { get; set; } = true;
        public List<string> AdditionalStylesheets { get; } = new List<string>();
        public List<string> AdditionalJavascript { get; } = new List<string>();
    }
}
