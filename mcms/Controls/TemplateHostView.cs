using System;
using mcms.Models;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace mcms.Controls
{
    [Preserve(AllMembers = true)]
    public class TemplateHostView : View
    {
        public Page Template { get; set; }
        public Workorder WOData { get; set; }
    }
}
