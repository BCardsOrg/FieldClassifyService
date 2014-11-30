using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.UserControls
{
    public class ImageViewerMultiPageViewModel : NotifyPropertyChanged
    {
        private ImageViewerViewModel m_ImageViewerViewModel;
        private readonly Dictionary<int, ImageViewerPage> m_ImageViewerPages = new Dictionary<int, ImageViewerPage>();
        private readonly Dictionary<int, ImageViewerViewModel> m_ImageViewerViewModels = new Dictionary<int,ImageViewerViewModel>();        
        private readonly ConcreteRoiFormat m_Rois = new ConcreteRoiFormat();
        private readonly ConcreteRoiFormat m_RecognitionRois = new ConcreteRoiFormat();
        private readonly ConcreteRoiFormat m_GoldenRois = new ConcreteRoiFormat();
        private readonly ConcreteRoiFormat m_SelectedRois = new ConcreteRoiFormat();
        private readonly ConcreteRoiFormat m_SelectedRoisSubGroup1 = new ConcreteRoiFormat();
        private readonly ConcreteRoiFormat m_SelectedRoisSubGroup2 = new ConcreteRoiFormat();
        private int m_FirstPageIndex = -1;
        private int m_LastPageIndex = -1;
        private string m_FileName = null;
        private System.Windows.Media.Imaging.TiffBitmapDecoder m_TiffBitmapDecoder = null;        
              
        public ImageViewerViewModel ImageViewerViewModel
        {
            get { return m_ImageViewerViewModel; }
            set 
            {
                m_ImageViewerViewModel = value;
                OnPropertyChanged("ImageViewerViewModel");  
            }
        }

        public Dictionary<int, ImageViewerPage> ImageViewerPages
        {
            get { return m_ImageViewerPages; }
        }

        public Dictionary<int, ImageViewerViewModel> ImageViewerViewModels
        {
            get { return m_ImageViewerViewModels; }
        }

        public string FileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

        public int FirstPageIndex
        {
            get { return m_FirstPageIndex; }
            set { m_FirstPageIndex = value; }
        }

        public int LastPageIndex
        {
            get { return m_LastPageIndex; }
            set { m_LastPageIndex = value; }
        }

        public IRoiFormat Rois
        {
            get { return m_Rois; }
        }

        public IRoiFormat RecognitionRois
        {
            get { return m_RecognitionRois; }
        }

        public IRoiFormat GoldenRois
        {
            get { return m_GoldenRois; }
        }

        public IRoiFormat SelectedRois
        {
            get { return m_SelectedRois; }
        }

        public IRoiFormat SelectedRoisSubGroup1
        {
            get { return m_SelectedRoisSubGroup1; }
        }

        public IRoiFormat SelectedRoisSubGroup2
        {
            get { return m_SelectedRoisSubGroup2; }
        }        

        public void ClearRois()
        {
            m_ImageViewerViewModel.SelectedRois.Clear(); //will clear also selection sub-groups 1 & 2
            m_ImageViewerViewModel.RecognitionRois.Clear();
            m_ImageViewerViewModel.GoldenRois.Clear();
            m_ImageViewerViewModel.Rois.Clear();
        }

        //move this whole thing up to another ViewMode called "ImageViewerMultiPageViewMode"
        public void SwitchToPage(int pageIndex)
        {
            if (pageIndex >= 0 && m_ImageViewerPages != null && m_ImageViewerPages.Count > 0 && m_ImageViewerPages.ContainsKey(pageIndex)  && m_ImageViewerPages[pageIndex] != null)
            {                
                if (m_ImageViewerViewModels.ContainsKey(pageIndex) && m_ImageViewerViewModels[pageIndex] != null)
                {
                    m_ImageViewerViewModel = m_ImageViewerViewModels[pageIndex];
                }
                else
                {                    
                    System.Windows.Media.Imaging.BitmapSource bitmapSource = UserControls.ImageResourceModerator.Instance.GetBitmapSource(m_FileName, pageIndex);
               
                    m_ImageViewerViewModel = new ImageViewerViewModel(bitmapSource, m_ImageViewerPages[pageIndex].Rotation, m_ImageViewerPages[pageIndex].Skew.X, m_ImageViewerPages[pageIndex].Skew.Y, m_ImageViewerPages[pageIndex].Shift, m_FileName, pageIndex);
                    m_ImageViewerViewModel.FirstPageIndex = m_FirstPageIndex;
                    m_ImageViewerViewModel.LastPageIndex = m_LastPageIndex;

                    m_ImageViewerViewModel.DisableMaskAutoDrawRois();

                    m_ImageViewerViewModel.Rois.Clear();
                    m_ImageViewerPages[pageIndex].Rois.ToList().ForEach(x => m_ImageViewerViewModel.Rois.Add(x));
                    CopyFormat(m_ImageViewerPages[pageIndex].Rois, m_ImageViewerViewModel.Rois);
                
                    m_ImageViewerViewModel.RecognitionRois.Clear();
                    m_ImageViewerPages[pageIndex].RecognitionRois.ToList().ForEach(x => m_ImageViewerViewModel.RecognitionRois.Add(x));
                    CopyFormat(m_RecognitionRois, m_ImageViewerViewModel.RecognitionRois);

                    m_ImageViewerViewModel.GoldenRois.Clear();
                    m_ImageViewerPages[pageIndex].GoldenRois.ToList().ForEach(x => m_ImageViewerViewModel.GoldenRois.Add(x));
                    CopyFormat(m_GoldenRois, m_ImageViewerViewModel.GoldenRois);

                    m_ImageViewerViewModel.SelectedRois.Clear();
                    m_ImageViewerPages[pageIndex].SelectedRois.ToList().ForEach(x => m_ImageViewerViewModel.SelectedRois.Add(x));
                    CopyFormat(m_SelectedRois, m_ImageViewerViewModel.SelectedRois);

                    m_ImageViewerViewModel.SelectedRoisSubGroup1.Clear();
                    m_ImageViewerPages[pageIndex].SelectedRoisSubGroup1.ToList().ForEach(x => m_ImageViewerViewModel.SelectedRoisSubGroup1.Add(x));
                    CopyFormat(m_SelectedRoisSubGroup1, m_ImageViewerViewModel.SelectedRoisSubGroup1);

                    m_ImageViewerViewModel.SelectedRoisSubGroup2.Clear();
                    m_ImageViewerPages[pageIndex].SelectedRoisSubGroup2.ToList().ForEach(x => m_ImageViewerViewModel.SelectedRoisSubGroup2.Add(x));
                    CopyFormat(m_SelectedRoisSubGroup2, m_ImageViewerViewModel.SelectedRoisSubGroup2);

                    m_ImageViewerViewModel.EnableMaskAutoDrawRois();
                    m_ImageViewerViewModels.Add(pageIndex, m_ImageViewerViewModel); //for next time
                }

                UserControls.ImageResourceModerator.Instance.SignalToProcessFrames(m_FileName, pageIndex, m_ImageViewerViewModels[pageIndex].InstanceID);

                OnPropertyChanged("ImageViewerViewModel");  
            }
        }

        private void CopyFormat(IRoiFormat source, IRoiFormat target)
        {
            target.FillColor = source.FillColor;
            target.BorderColor = source.BorderColor;
            target.BorderThickness = source.BorderThickness;
        }

    }

    public class ImageViewerPage
    {        
        private int m_PageIndex = -1;       
        private readonly RoiCollection m_Rois = new RoiCollection();
        private readonly RoiCollection m_RecognitionRois = new RoiCollection();
        private readonly RoiCollection m_GoldenRois = new RoiCollection();
        private readonly RoiCollection m_SelectedRois = new RoiCollection();
        private readonly RoiCollection m_SelectedRoisSubGroup1 = new RoiCollection();
        private readonly RoiCollection m_SelectedRoisSubGroup2 = new RoiCollection();
        private System.Drawing.PointF m_Shift;
        private System.Drawing.PointF m_Skew;
        private Rotation m_Rotation;

        public int PageIndex
        {
            get { return m_PageIndex; }
            set { m_PageIndex = value; }
        }                 

        public System.Drawing.PointF Shift
        {
            get { return m_Shift; }
            set { m_Shift = value; }
        }        

        public System.Drawing.PointF Skew
        {
            get { return m_Skew; }
            set { m_Skew = value; }
        }        

        public Rotation Rotation
        {
            get { return m_Rotation; }
            set { m_Rotation = value; }
        }

        public RoiCollection Rois
        {
            get { return m_Rois; }
        }

        public RoiCollection RecognitionRois
        {
            get { return m_RecognitionRois; }
        }

        public RoiCollection GoldenRois
        {
            get { return m_GoldenRois; }
        }

        public RoiCollection SelectedRois
        {
            get { return m_SelectedRois; }
        }

        public RoiCollection SelectedRoisSubGroup1
        {
            get { return m_SelectedRoisSubGroup1; }
        }

        public RoiCollection SelectedRoisSubGroup2
        {
            get { return m_SelectedRoisSubGroup2; }
        }                       
    }

    internal class ConcreteRoiFormat : IRoiFormat
    {
        private Color m_FillColor = Colors.Yellow;
        private Color m_BorderColor = Colors.Green;
        private double m_BorderThickness = 2;

        public Color FillColor
        {
            get { return m_FillColor; }
            set { m_FillColor = value; }
        }

        public Color BorderColor
        {
            get { return m_BorderColor; }
            set { m_BorderColor = value; }
        }

        public double BorderThickness
        {
            get { return m_BorderThickness; }
            set { m_BorderThickness = value; }
        }
    }
}
