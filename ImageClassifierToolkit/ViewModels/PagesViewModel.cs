using TiS.Recognition.FieldClassifyService.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Statistic;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{
    public class PageViewModel : NotifyPropertyChanged
    {

        PageData m_pageData;
        public PageViewModel(PageData pageData)
        {
            m_pageData = pageData;
            m_goldClass = pageData.Setup.ClassName;
            m_isTrain = AppDataCenter.Singleton.TrainPages.Contains(m_pageData) == true;
        }


        bool m_isSelected = false;
        public bool IsSelected
        {
            get
            {
                return m_isSelected;
            }
            set
            {
                OnChange(ref m_isSelected, value, "IsSelected");
            }
        }

        bool m_isTrain;
        public bool IsTrain
        {
            get
            {
                return m_isTrain;
            }
            set
            {
                OnChange(ref m_isTrain, value, "IsTrain");
            }
        }

        public void Update()
        {
            m_pageData = AppDataCenter.Singleton.SetupData.Pages.First(x => x.Setup.FileName == m_pageData.Setup.FileName && x.Setup.PageNo == m_pageData.Setup.PageNo);
            IsTrain = AppDataCenter.Singleton.TrainPages.Contains(m_pageData);
            GoldClass = m_pageData.Setup.ClassName;
        }

        public string FileName
        {
            get
            {
                return m_pageData.Setup.FileName;
            }
        }

        public int PageOrder
        {
            get
            {
                return m_pageData.Setup.PageNo;
            }
        }



        string m_goldClass;
        public string GoldClass
        {
            get
            {
                return m_goldClass;
            }
            internal set
            {
                OnChange(ref m_goldClass, value, "GoldClass");
            }
        }

        public override bool Equals(object obj)
        {
            if ( obj == null || obj is PageViewModel == false )
            {
                return false;
            }

            var other = obj as PageViewModel;
            return other.FileName == FileName && other.PageOrder == PageOrder;
        }
    }

    public class GoldPagesViewModel : NotifyPropertyChanged
    {
        public GoldPagesViewModel(string goldClassName)
        {
            Pages = new ObservableCollection<PageViewModel>();
            GoldClass = goldClassName;
        }
        public ObservableCollection<PageViewModel> Pages { get; private set; }

        public string GoldClass { get; private set;}
    }


    public class PageReportViewModel : PageViewModel
    {
        PageReportData m_pageReportData;
        public PageReportViewModel(PageData page) :
            base(page)
        {
            m_pageReportData = null;
        }
        public PageReportViewModel(PageReportData pageReportData) :
            base(pageReportData.Page)
        {
            m_pageReportData = pageReportData;
        }

        public PageReportData Report
        {
            get
            {
                return m_pageReportData;
            }
        }

        public MatchType Match
        {
            get
            {
                if (Report == null) return 0;
                return StatisticMath.CalcPageMatchType(Report);
            }
        }

        public string MatchClass
        {
            get
            {
                if (Report == null) return null;
                if (Report.ClassMatch.Count > 0)
                {
                    return Report.ClassMatch.ElementAt(0).Key;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public double MatchConfidence
        {
            get
            {
                if (Report == null) return 0;
                if (Report.ClassMatch.Count > 0)
                {
                    return Report.ClassMatch.ElementAt(0).Value;
                }
                else
                {
                    return 0;
                }
            }
        }

        public string MatchClassGuess1
        {
            get
            {
                if (Report == null) return null;
                if (Report.ClassMatch.Count > 1)
                {
                    return Report.ClassMatch.ElementAt(1).Key;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public double MatchConfidenceGuess1
        {
            get
            {
                if (Report == null) return 0;
                if (Report.ClassMatch.Count > 1)
                {
                    return Report.ClassMatch.ElementAt(1).Value;
                }
                else
                {
                    return 0;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if ( obj == null || obj is PageReportViewModel == false )
            {
                return false;
            }

            var other = obj as PageReportViewModel;

            return base.Equals(obj) == true && Report == other.Report;
        }

    }


    public class PagesViewModel : BaseModelView
    {
        public static event EventHandler SelectedChanged;

        static public ObservableCollection<GoldPagesViewModel> Pages { get; private set; }

        static PagesViewModel()
        {
            Pages = new ObservableCollection<GoldPagesViewModel>();
        }

        public PagesViewModel()
        {
            RegisterProperty(new Action(() => UpdatePages()), NotifyGroup.Configuration);
            RegisterProperty(new Action(() => UpdatePages()), NotifyGroup.StatisticData);

            UpdatePages();
        }

        public BitmapSource SelectedImage
        {
            get
            {
                var selectedPages = Pages.SelectMany(x => x.Pages.Where(y => y.IsSelected == true)).ToList();
                if (selectedPages.Count == 1)
                {
                    var page = selectedPages[0];
                    using (var srcFile = System.IO.File.OpenRead(page.FileName))
                    {
                        var dec = TiffBitmapDecoder.Create(srcFile, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        return dec.Frames[page.PageOrder - 1];
                    }
                }
                else
                {
                    return null;
                }
            }
        }


        public string SelectedImageName
        {
            get
            {
                var selectedPages = Pages.SelectMany(x => x.Pages.Where(y => y.IsSelected == true)).ToList();
                if (selectedPages.Count == 1)
                {
                    return selectedPages[0].FileName;
                }
                else
                {
                    return "";
                }
            }
        }

        public virtual void SetReportData(ReportData reportData)
        {
            UpdatePages( reportData );
        }


        private void UpdatePages(ReportData newReportData = null)
        {
            List<PageViewModel> existPages = new List<PageViewModel>();

            var reportData = newReportData ?? AppDataCenter.Singleton.BaseReportData;

            var allPages = Pages.SelectMany(x => x.Pages).ToList();
                foreach (var pageData in AppDataCenter.Singleton.SetupData.Pages)
                {
                    var newP = new PageReportViewModel(pageData);
                    var oldP = allPages.FirstOrDefault(x => x.Equals(newP) == true);
                    if ( oldP == null )
                    {
                        AddPage(newP);
                        existPages.Add(newP);
                    }
                    else
                    {
                        existPages.Add(oldP);
                    }
                }

            // Remove old pages
            foreach (var goldPages in Pages.ToList())
            {
                var remove = goldPages.Pages.Except(existPages).ToList();
                foreach (var item in remove)
                {
                    goldPages.Pages.Remove(item);
                }
            }


            // Update each page
            foreach (var page in Pages.SelectMany(x => x.Pages))
            {
                page.Update();                
            }

            // Update goldPages list
            foreach (var goldPages in Pages.ToList())
            {
                foreach (var page in goldPages.Pages.Where( x => x.GoldClass != goldPages.GoldClass ).ToList() )
                {
                    goldPages.Pages.Remove(page);
                    AddPage(page);
                }
            }

            // Remove empty goldClass
            foreach (var goldPages in Pages.ToList())
            {
                if (goldPages.Pages.Count == 0)
                {
                    Pages.Remove(goldPages);
                }
            }
        }

        private GoldPagesViewModel AddPage(PageViewModel pageVm)
        {
            GoldPagesViewModel goldPages = Pages.FirstOrDefault(x => x.GoldClass == pageVm.GoldClass);
            if ( goldPages == null )
            {
                goldPages = new GoldPagesViewModel(pageVm.GoldClass);
                goldPages.Pages.Add(pageVm);
                Pages.Add(goldPages);
            }
            else
            {
                goldPages.Pages.Add(pageVm);
            }

            return goldPages;
        }
        public void UpdateSelectedPages()
        {
            OnPropertyChanged("SelectedImageName");
            OnPropertyChanged("SelectedImage");

            if (SelectedChanged != null)
            {
                SelectedChanged(this, EventArgs.Empty);
            }
        }
        static public void SetGoldClass(string goldClassName)
        {
            var selectedPages = Pages.SelectMany(x => x.Pages)
                .Where(x => x.IsSelected == true)
                .Select(x => AppDataCenter.Singleton.SetupData.Pages.First(y => y.Setup.FileName == x.FileName && y.Setup.PageNo == x.PageOrder));
            AppDataCenter.Singleton.SetGoldClass(selectedPages, goldClassName);
        }
    }
}
