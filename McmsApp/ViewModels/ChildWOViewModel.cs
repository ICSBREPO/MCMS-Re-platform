using System.Windows.Input;
using McmsApp.Models;
using McmsApp.Persistence;
using McmsApp.Views.Home;
using McmsApp.Views.Work.WorkDetail;
using Syncfusion.Maui.ListView;
using Syncfusion.Maui.TabView;

namespace McmsApp.ViewModels
{
    public class ChildWOViewModel : BaseViewModel
    {
        //Command
        public ICommand SelectedChildWOCommand { get; set; }

        //persistance SQLite
        IWorkorder sqlwo = new SQLiteWorkorder();

        //Navigation From Parent
        INavigation Navigation { get; set; }

        //Binding View
        public Workorder workorder { get; set; }
        public List<Workorder> ChildWOList { get; set; }
        public int ChildWOCount { get; set; } 
        public bool NoChild { get; set; }

        SfTabView sftabview;

        public ChildWOViewModel(Workorder wo, INavigation navigation, SfTabView sftabView)
        {
            Navigation = navigation;
            workorder = wo;
            _ = LoadChildWO();
            sftabview = sftabView;
            SelectedChildWOCommand = new Command(SelectedChildWO);
        }

        async Task LoadChildWO()
        {
            ChildWOList = await sqlwo.GetChildWorkorders(workorder.wonum);
            if (ChildWOList.Count <1)
            {
                NoChild = true;
            }
            else
            {
                NoChild = false;
                int i = 1;
                foreach (Workorder wo in ChildWOList)
                {
                    wo.sequence = i;
                    i++;
                }
                ChildWOCount = ChildWOList.Count;
            }
            
        }

        async void SelectedChildWO(object obj)
        {
            SfListView selected = obj as SfListView;
            Workorder wo = selected.SelectedItem as Workorder;
            selected.SelectedItem = null;
            //SfTabItem tabItem = WOListPage.TabView.Items[1];
            //tabItem.Content = new WorkorderDetail(wo,WOListPage).Content;
            await Navigation.PushAsync(new TemplateWODetail(wo, Navigation, sftabview));
        }
    }
}
