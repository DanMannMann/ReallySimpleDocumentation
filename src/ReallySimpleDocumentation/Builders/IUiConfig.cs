using System.Collections.Generic;

namespace Marsman.ReallySimpleDocumentation
{
    public interface IUIOptions
    {
        string FaviconUrl { get; set; }
        string LogoUrl { get; set; }
        string LogoAltText { get; set; }
        string LogoBackgroundColor { get; set; }
        public List<string> AdditionalStylesheets { get; }
        public List<string> AdditionalJavascript { get; }
        string DefaultJavascriptRoute { get; set; }
        string DefaultCssRoute { get; set; }
        bool ServeDefaultJavascript { get; set; }
        bool ServeDefaultCss { get; set; }
    }
}
