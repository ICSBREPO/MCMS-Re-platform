
using mcms.Models;
using mcms.Persistence;
using Syncfusion.XForms.ProgressBar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace mcms.General
{


   public class ProgressBarUpdate
    {
        // Step Progress Bar
        public StepStatus prsqa { get; set; }
        public StepStatus prgps { get; set; }
        public StepStatus prinprg { get; set; }
        public StepStatus prte { get; set; }
        public StepStatus prphotos { get; set; }
        public StepStatus prcomp { get; set; }
        public ImageSource Iprsqa { get; internal set; }
        public ImageSource Iprgps { get; internal set; }
        public ImageSource Iprinprg { get; internal set; }
        public ImageSource Iprte { get; internal set; }
        public ImageSource Iprphotos { get; internal set; }
        public ImageSource Iprcomp { get; internal set; }
        public int totaldoclink { get; set; }

       
        public ObservableCollection<WoProgress> WoProgressList;
        public Workorder workorder { get; set; }


        public bool IsBusy { get; set; }

        public IPlusgaudit sqlsqa = new SQLitePlusgaudit();
        public IDoclinks sqldoc = new SQLiteDoclinks();
        public ITnbwometergroup sqlmetgroup = new SQLiteTnbwometergroup();
        ITnbwometer sqlmeter = new SQLiteTnbwometer();

       public ProgressBarUpdate(ObservableCollection<WoProgress> WoProgressList,StepStatus prsqa, 
            StepStatus prinprg, StepStatus prte, StepStatus prphotos, StepStatus prcomp, bool IsBusy)
        {

            this.WoProgressList = WoProgressList;
            //this.workorder = workorder;
            this.prsqa = prsqa;
            //this.prgps = prgps;
            this.prinprg = prinprg;
            this.prte = prte;
            this.prphotos = prphotos;
            this.prcomp = prcomp;
           // this.IsBusy = IsBusy;

            


        }

        async public Task LoadProgress(Workorder workorder=null)
        {
            

            if (IsBusy == false)
            {
                
                Debug.WriteLine("Load Progress function is run");
                Debug.WriteLine("Status is : " + workorder.status);
                IsBusy = true;

               
                   
                 

                prsqa = prgps = prinprg = prte = prphotos = prcomp = StepStatus.NotStarted;


               




                List<Plusgaudit> sqa = await sqlsqa.GetPlusgauditByWO(workorder.wonum);
                List<Doclinks> doc = await sqldoc.GetDoclinksFilter(workorder.id, "WORKORDER");
                totaldoclink = doc.Count;

                int totalmeterreading = 0;
                workorder.tnbwometergroup = await sqlmetgroup.GetTnbwometergroupByWO(workorder.wonum);
                if (workorder.tnbwometergroup != null)
                {
                    foreach (Tnbwometergroup tnbwometergroup in workorder.tnbwometergroup)
                    {
                        totalmeterreading += await sqlmeter.GetCountMeterReading(tnbwometergroup.tnbwometergroupid);
                    }
                }

                Debug.WriteLine("Number of Metter is : " + totalmeterreading);

                if (sqa.Where(x => x.status == "DRAFT").Count() == 0 && sqa.Count() > 0)
                {
                        

                        prsqa = StepStatus.Completed;
                        prgps = StepStatus.InProgress;

                    Iprsqa = ImageSource.FromFile("SqaProgress.png");



                    if (workorder.status == "INPRGS" || workorder.status == "TESTCOMP")
                    {
                        Debug.WriteLine("Updated Status is : " + workorder.status);
                        prinprg = StepStatus.Completed;
                        prte = StepStatus.InProgress;

                        Iprinprg = ImageSource.FromFile("StatusProgress.png");

                        if (totalmeterreading == 0)
                        {
                            prte = StepStatus.Completed;
                            prphotos = StepStatus.InProgress;
                            //skip meter dulu
                            Iprte = ImageSource.FromFile("TestingProgress.png");
                            if (totaldoclink > 1)
                            {
                                prphotos = StepStatus.Completed;
                                prcomp = StepStatus.InProgress;

                                Iprphotos = ImageSource.FromFile("UploadProgress.png");

                                if (workorder.status == "TESTCOMP")
                                {
                                    prcomp = StepStatus.Completed;

                                    Iprcomp = ImageSource.FromFile("CompleteProgress.png");

                                }
                                else
                                {
                                    Iprcomp = ImageSource.FromFile("complete_gray.png");
                                }
                            }
                            else
                            {
                                Iprphotos = ImageSource.FromFile("upload_gray.png");
                                Iprcomp = ImageSource.FromFile("complete_gray.png");
                            }
                        }
                        else
                        {
                            Iprte = ImageSource.FromFile("testing_gray.png");
                            Iprphotos = ImageSource.FromFile("upload_gray.png");
                            Iprcomp = ImageSource.FromFile("complete_gray.png");
                        }
                    }
                    else
                    {

                        Iprinprg = ImageSource.FromFile("inprg_gray.png");
                        Iprte = ImageSource.FromFile("testing_gray.png");
                        Iprphotos = ImageSource.FromFile("upload_gray.png");
                        Iprcomp = ImageSource.FromFile("complete_gray.png");
                    }
                }
                else
                {
                    Iprsqa = ImageSource.FromFile("sqa_gray.png");
                    Iprgps = ImageSource.FromFile("gps_gray.png");
                    Iprinprg = ImageSource.FromFile("inprg_gray.png");
                    Iprte = ImageSource.FromFile("testing_gray.png");
                    Iprphotos = ImageSource.FromFile("upload_gray.png");
                    Iprcomp = ImageSource.FromFile("complete_gray.png");
                }

                if (workorder.status == "INPRGS" || workorder.status == "TESTCOMP")
                {
                   
                    WoProgressList.Clear();

                    WoProgressList.Add(new WoProgress
                    {
                        ProgressImage = Iprsqa,
                        Title = "Create SQA",
                        Status = prsqa
                    });

                    /* WoProgressList.Add(new WoProgress
                     {
                         ProgressImage = Iprgps,
                         Title = "Capture GPS Cordinate",
                         Status = prgps
                     });*/

                    WoProgressList.Add(new WoProgress
                    {
                        ProgressImage = Iprinprg,
                        Title = "Change Status to INPRG - In Progress",
                        Status = prinprg
                    });

                    WoProgressList.Add(new WoProgress
                    {
                        ProgressImage = Iprte,
                        Title = "Testing",
                        Status = prte
                    });

                    WoProgressList.Add(new WoProgress
                    {
                        ProgressImage = Iprphotos,
                        Title = "Upload Photos",
                        Status = prphotos
                    });

                    WoProgressList.Add(new WoProgress
                    {
                        ProgressImage = Iprcomp,
                        Title = "Change Status to TESTCOMP - Complete",
                        Status = prcomp
                    });
                }
                else
                {

                    if (WoProgressList.Count() >= 4)
                    {

                        //for sqa
                        WoProgressList[0].Status = prsqa;
                        WoProgressList[0].ProgressImage = Iprsqa;



                        //for prinprg

                        WoProgressList[1].Status = prinprg;
                        WoProgressList[1].ProgressImage = Iprinprg;



                        //for Testing
                        WoProgressList[2].Status = prte;
                        WoProgressList[2].ProgressImage = Iprte;


                        //for photo
                        WoProgressList[3].Status = prphotos;
                        WoProgressList[3].ProgressImage = Iprphotos;


                        //for pr complete
                        WoProgressList[4].Status = prcomp;
                        WoProgressList[4].ProgressImage = Iprcomp;


                        Debug.WriteLine("Function is Run");

                    }

                }










                IsBusy = false;


            }

          

        }


    }
}
