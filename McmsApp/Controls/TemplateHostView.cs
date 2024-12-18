using McmsApp.Models;
using Microsoft.Maui.Controls.Internals;

namespace McmsApp.Controls
{
    [Preserve(AllMembers = true)]
    public class TemplateHostView : View
    {
        public Page Template { get; set; }
        public Workorder WOData { get; set; }
    }
}
