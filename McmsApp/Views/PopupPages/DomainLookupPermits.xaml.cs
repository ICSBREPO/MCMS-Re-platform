using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using mcms.Models;
using mcms.Persistence;
using mcms.ViewModels;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;

namespace mcms.Views.PopupPages
{
    public partial class DomainLookupPermits : ContentPage
    {
        public MxDomain mx { get; set; }
        public PermitWork permit { get; set; }
        IMxDomain sqldomain = new SQLiteMxDomain();
        string domain { get; set; }
        string values { get; set; }

        public DomainLookupPermits(string domainid, PermitViewModel pervm, string value, PermitWork per)
        //public DomainLookupPermits(string domainid, PermitViewModel pervm, string value, bool isdesc = false)
        {
            InitializeComponent();
            BindingContext = pervm;
            permit = per;
            
            values = value;
            domain = domainid;
            /*
            DomainList.ItemSelected += async (sender, e) =>
            {
                if (DomainList.SelectedItem == null)
                    return;
                MxDomain dom = (MxDomain)(DomainList.SelectedItem);
                DomainList.SelectedItem = null;
                var propertyinfo = pervm.permit.GetType().GetProperty(values);
                propertyinfo.SetValue(pervm.permit, dom.value);
                
                if (isdesc)
                {
                    var propertydesc = pervm.permit.GetType().GetProperty($"{values}_description");
                    propertydesc.SetValue(pervm.permit, dom.description);
                }

                await Navigation.PopModalAsync();
            };
            LoadDomain();
            */

            DomainList.SelectionChanged += async (sender, e) =>
            {
                if (DomainList.SelectedItem == null)
                    return;
                MxDomain mx = (MxDomain)(DomainList.SelectedItem);
                DomainList.SelectedItem = null;

                if (domainid == "TNBSTATE")
                {
                    permit.tnbstate = mx.value;
                }
                else if (domainid == "TNBSUBZONE")
                {
                    permit.tnbsubzone = mx.value;
                }
                else if (domainid == "TNBPERMITTYPE")
                {
                    permit.tnbpermittype = mx.value;
                }
                else if (domainid == "TNBBSNSAREA")
                {
                    permit.tnbbusinessarea = mx.value;
                }
                else if (domainid == "PLUSGPERWORKSTATUS")
                {
                    permit.tnbstatus = mx.value;
                }
                else if (domainid == "TNBUNITGRP")
                {
                    permit.tnbunit = mx.value;
                }
                else if (value == "tnbissuedby")
                {
                    permit.tnbissuedby = mx.value;
                }
                else if (value == "tnbissuedto")
                {
                    permit.tnbissuedto = mx.value;
                }

                await Navigation.PopModalAsync();
            };

        }

        /*
        async void LoadDomain()
        {
            List<MxDomain> domains = await sqldomain.GetMxDomainAsync(domain);
            DomainList.ItemsSource = domains;
        }
        */
        
        async void Cancel_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
