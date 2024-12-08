using mcms.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mcms.Views.Work.WorkDetail.Workorderspec
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorkorderSpecification : ContentPage
    {
        WorkorderspecViewModel workorderspecViewModel { get; set; }
        public WorkorderSpecification(WorkorderspecViewModel workorderspecviewmodel)
        {
            BindingContext = workorderspecviewmodel;
            workorderspecViewModel = workorderspecviewmodel;
            InitializeComponent();
        }

        void SfPicker_OkButtonClicked(System.Object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            if (workorderspecViewModel.isasset)
            {
                workorderspecViewModel.selectedassetspec.alnvalue = workorderspecViewModel.selecteddomain;
            }else
            {
                workorderspecViewModel.selectedlocationspec.alnvalue = workorderspecViewModel.selecteddomain;
            }
        }
    }
}