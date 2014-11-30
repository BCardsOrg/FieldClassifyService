using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;


namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.UserControls
{
    
    public class ImageViewerViewModel : INotifyPropertyChanged
    {
        #region Private Members

        //private System.Windows.Media.Imaging.BitmapSource m_BitmapSource;
        private WeakReference m_BitmapSource;
		// hold reference to the BitmapSource, in case we can't use WeakReference
		private System.Windows.Media.Imaging.BitmapSource m_BitmapSourceReference; 
        private string m_FileName;
        private string m_Description;
        private int m_PageIndex = -1;
        private PersistentViewState m_PersistentViewState;
        private VolatileViewState m_VolatileViewState;
        private TransformGroup m_TransformGroup;
        private readonly RoiCollection m_Rois = new RoiCollection();
        private readonly RoiCollection m_RecognitionRois = new RoiCollection();
        private readonly RoiCollection m_GoldenRois = new RoiCollection();
        private readonly RoiCollection m_SelectedRois = new RoiCollection();
        private readonly RoiCollection m_SelectedRoisSubGroup1 = new RoiCollection();
        private readonly RoiCollection m_SelectedRoisSubGroup2 = new RoiCollection();
        private readonly RoiCollection m_SelectedRoisSubGroup3 = new RoiCollection();
        private readonly RoiCollection m_TableRois = new RoiCollection();
        private readonly RoiCollection m_TableRegionRois = new RoiCollection();
        //private System.Windows.Media.Imaging.RenderTargetBitmap m_MaskBitmapSource;
        private WeakReference m_MaskBitmapSource;
        private bool m_MaskRedrawEnabled = true;        
        private System.Windows.Visibility m_ControlsVisibilty = System.Windows.Visibility.Visible;
        private double m_ControlsHeight = 25d;
        private double m_Zoom = 0.35;       
        private bool m_ScaleToBounds = false;
        private double m_PaddingThickness = 20;
        private string m_InstanceID = string.Empty;        
                    
        #region Tiff Frame Paging
        
        /// <summary>
        /// TODO: move this to child class which should be used by the multi-paged image viewer control
        /// </summary>
        private int m_FirstPageIndex;
        private int m_LastPageIndex;

        #endregion
        
        #endregion

        #region Constructors

        public ImageViewerViewModel()
        {
            m_Rois.CollectionChanged += Rois_CollectionChanged;
            m_SelectedRois.CollectionChanged += SelectedRois_CollectionChanged;
            m_RecognitionRois.CollectionChanged += RecognitionRois_CollectionChanged;
            m_GoldenRois.CollectionChanged += GoldenRois_CollectionChanged;
            m_SelectedRoisSubGroup1.CollectionChanged += SelectedRoisSubGroup_CollectionChanged;
            m_SelectedRoisSubGroup2.CollectionChanged += SelectedRoisSubGroup_CollectionChanged;
            m_SelectedRoisSubGroup3.CollectionChanged += SelectedRoisSubGroup_CollectionChanged;
            m_TableRois.CollectionChanged += TableRois_CollectionChanged;
            m_TableRegionRois.CollectionChanged += RoisTableRegionRois_CollectionChanged;
        }

		public ImageViewerViewModel(System.Windows.Media.Imaging.BitmapSource bitmapSource)
            : this(bitmapSource, TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.UserControls.Rotation.None, 0, 0, System.Drawing.PointF.Empty, "", -1, string.Empty)            
        {}

        public ImageViewerViewModel(System.Windows.Media.Imaging.BitmapSource bitmapSource, TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.UserControls.Rotation rotation, double skewAngleX, double skewAngleY, System.Drawing.PointF shift, string fileName /*= ""*/, int pageIndex/* = -1*/, string description = "")
            : this()
        {
            m_BitmapSource = new WeakReference(bitmapSource);
            m_PersistentViewState = new PersistentViewState(              
                bitmapSource.PixelWidth,
                bitmapSource.PixelHeight,
                skewAngleX,
                skewAngleY,
                rotation,
                shift);
            //m_VolatileViewState = new VolatileViewState(m_BitmapSource.Width, m_BitmapSource.Height);
            m_VolatileViewState = new VolatileViewState(bitmapSource.PixelWidth, bitmapSource.PixelHeight);
            m_Description = description;
            m_FileName = fileName;
            m_PageIndex = pageIndex;

			// In case we can't use the week reference, then we keep one reference to the image
			if (string.IsNullOrEmpty(m_FileName) == true)
			{
				m_BitmapSourceReference = bitmapSource;
			}
        }
        
        //private void InitMaskBitmap(System.Windows.Media.Imaging.BitmapSource bitmapSource)
        //{
        //    if (bitmapSource == null)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    //this.m_MaskBitmapSource = new System.Windows.Media.Imaging.RenderTargetBitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, 96, 96, PixelFormats.Default);
        //    this.m_MaskBitmapSource = UserControls.ImageResourceModerator.Instance.GetRenderTargetBitmap(this.FileName, this.PageIndex);

        //    #region debug - write bitmap to file            
        //    //DrawingVisual drawingVisual = new DrawingVisual();
        //    //DrawingContext drawingContext = drawingVisual.RenderOpen();
        //    //drawingContext.DrawRectangle(new SolidColorBrush(Colors.Transparent), null, new System.Windows.Rect(0, 0, bitmapSource.PixelWidth, bitmapSource.PixelHeight));
        //    //drawingContext.Close();
        //    //m_MaskBitmapSource.Render(drawingVisual);

        //    //System.Windows.Media.Imaging.PngBitmapEncoder encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
        //    //encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(m_MaskBitmapSource));
        //    //using (System.IO.Stream s = System.IO.File.Create(string.Format(@"C:\Users\Yoav.Tiran\Desktop\Screenshots\b\t{0}.png", a.ToString())))
        //    //{
        //    //    encoder.Save(s);
        //    //    a++;
        //    //}
        //    #endregion
        //}

        #endregion

        #region Public Methods

        public void Clear()
        {
            //order does matter..
            MaskBitmapSource.Clear();
            this.Rois.Clear();
            this.RecognitionRois.Clear();
            this.GoldenRois.Clear(); 
            this.SelectedRois.Clear(); //will clear also subgroups             
        }

        /// <summary>
        /// Opitimization option for fast draw of images with many ROIs such as pages that have tables.
        /// This turns off the drawing of the ROIs immediately on the MaskRenderBitmap.
        /// </summary>
        public void DisableMaskAutoDrawRois()
        {
            m_MaskRedrawEnabled = false;
        }

        /// <summary>
        /// Opitimization option for fast draw of images with many ROIs such as pages that have tables.
        /// This turns on the drawing of the ROIs immediately on the MaskRenderBitmap.
        /// Finally draws pending added ROIs.
        /// </summary>
        public void EnableMaskAutoDrawRois()
        {
            m_MaskRedrawEnabled = true;
            //draw pending added ROIs
            RedrawRoisOnMaskBitmapSource();
        }

        /// <summary>
        /// yoav 2013.05.16 un-necessary now
        /// every time method is called (on every UI change) the complete collection of ROIs is transformed, according to the original state (rect) of the ROI and the dynamic (volatile) transformation from the UI
        /// </summary>
        //public void ApplyTransfomationOnRois()
        //{
        //    foreach (var transRoi in m_Rois)
        //    {
        //        TransformRoi(transRoi);
        //    }
        //}
        
        #endregion

        #region Properties       
        
        public string InstanceID
        {
            get { return m_InstanceID; }
            set { m_InstanceID = value; }
        }

        public double PaddingThickness
        {
            get { return m_PaddingThickness; }
            set { m_PaddingThickness = value; }
        }

        public bool ScaleToBounds
        {
            get { return m_ScaleToBounds; }
            set { m_ScaleToBounds = value; }
        }

        /// <summary>
        /// When setting this from the outside, the value of the zoom will actually get the value of the nearest match from list of possible values
        /// </summary>
        public double Zoom
        {
            get { return m_Zoom; }
            set { m_Zoom = value; }
        }

        public System.Windows.Visibility ControlsVisibilty
        {
            get { return m_ControlsVisibilty; }
            set { m_ControlsVisibilty = value; }
        }

        public double ControlsHeight
        {
            get 
            {
                if (m_ControlsVisibilty == System.Windows.Visibility.Visible)
                {
                    return m_ControlsHeight;
                }
                else
                { 
                    return 0; 
                }
            }
            set { m_ControlsHeight = value; }
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

        public double Rotation
        {
            get
            {
                return m_PersistentViewState.RotateTransform.Angle;
            }
        }

        public double SkewAngleX
        {
            get
            {
                return m_PersistentViewState.SkewTransform.AngleX;
            }
        }

        public double SkewAngleY
        {
            get
            {
                return m_PersistentViewState.SkewTransform.AngleY;
            }
        }

        public TransformGroup TransformGroup
        {
            get
            {
                //order does(?) matter
                if (m_TransformGroup == null)
                {
                    m_TransformGroup = new TransformGroup();
                    m_TransformGroup.Children.Add(m_PersistentViewState.RotateTransform);
                    m_TransformGroup.Children.Add(m_VolatileViewState.RotateTransform);
                    m_TransformGroup.Children.Add(m_PersistentViewState.SkewTransform);
                    m_TransformGroup.Children.Add(m_VolatileViewState.SkewTransform);
                    //transformGroup.Children.Add(m_PersistentViewState.TranslateTransform);
                    m_TransformGroup.Children.Add(m_VolatileViewState.TranslateTransform);
                    //transformGroup.Children.Add(m_PersistentViewState.ScaleTransform);
                    m_TransformGroup.Children.Add(m_VolatileViewState.ScaleTransform);
                }

                return m_TransformGroup;
            }
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

        public RoiCollection SelectedRoisSubGroup3
        {
            get { return m_SelectedRoisSubGroup3; }
        }

        public RoiCollection TableRois
        {
            get { return m_TableRois; }
        }

        public RoiCollection TableRegionRois
        {
            get { return m_TableRegionRois; }
        }

        public PersistentViewState PersistentViewState
        {
            get { return m_PersistentViewState; }
            set
            {
                m_PersistentViewState = value;
                RaisePropertyChanged("PersistentViewState");
            }
        }

        public VolatileViewState VolatileViewState
        {
            get { return m_VolatileViewState; }
            set
            {
                m_VolatileViewState = value;
                RaisePropertyChanged("VolatileViewState");
            }
        }

        public System.Windows.Media.Imaging.BitmapSource BitmapSource
        {
            get 
            {
                System.Windows.Media.Imaging.BitmapSource bitmapSource = null;
                               
                if (m_BitmapSource != null && m_BitmapSource.IsAlive)
                {
                    bitmapSource = m_BitmapSource.Target as System.Windows.Media.Imaging.BitmapSource;
                    //UserControls.ImageResourceModerator.Instance.SignalToProcessFrames(m_FileName, m_PageIndex, m_InstanceID);
                }
                else if (m_BitmapSource != null && m_FileName.Length > 0 && m_PageIndex >= 0) //weak referenced object was alive, but was collected
                {
                    bitmapSource = UserControls.ImageResourceModerator.Instance.GetBitmapSource(m_FileName, m_PageIndex, m_InstanceID);
                    m_BitmapSource = new WeakReference(bitmapSource);
                }

                return bitmapSource;
            }           
        }

        /// <summary>
        /// as long as the bitmap source (tiff frame) is null, there mask bitmap is null
        /// </summary>
        public System.Windows.Media.Imaging.RenderTargetBitmap MaskBitmapSource
        {
            get
            {
                System.Windows.Media.Imaging.RenderTargetBitmap maskBitmapSource = null;
                if (BitmapSource != null)
                {
                    if (m_MaskBitmapSource != null && m_MaskBitmapSource.IsAlive)
                    {
                        maskBitmapSource = m_MaskBitmapSource.Target as System.Windows.Media.Imaging.RenderTargetBitmap;
                    }
					else if (m_FileName.Length > 0 && m_PageIndex >= 0) //weak referenced object was alive, but was collected
					{
						maskBitmapSource = UserControls.ImageResourceModerator.Instance.GetRenderTargetBitmap(m_FileName, m_PageIndex, m_InstanceID);
						if (m_MaskBitmapSource == null)
						{
							m_MaskBitmapSource = new WeakReference(maskBitmapSource);
						}
						else
						{
							m_MaskBitmapSource.Target = maskBitmapSource;
						}
					}
					else
					{
						maskBitmapSource = new System.Windows.Media.Imaging.RenderTargetBitmap(
							BitmapSource.PixelWidth, 
							BitmapSource.PixelHeight, 
							BitmapSource.DpiX, 
							BitmapSource.DpiY,
							System.Windows.Media.PixelFormats.Default);
						m_MaskBitmapSource = new WeakReference(maskBitmapSource);
					}
                }
                return maskBitmapSource;

            }           
        }

        public string FileName
        {
            get { return m_FileName; }
            set
            {
                m_FileName = value;
                RaisePropertyChanged("FileName");
            }
        }

        public string Description
        {
            get { return m_Description; }
            set
            {
                m_Description = value;
                RaisePropertyChanged("Description");
            }
        }

        public int PageIndex
        {
            get { return m_PageIndex; }
            set
            {
                m_PageIndex = value;
                RaisePropertyChanged("PageIndex");
            }
        }

        #endregion

        #region Private Methods
        private void ReplaceRoi(Roi roi)
        {
            throw new NotImplementedException();
        }

        private void MoveRoi(Roi roi, int toPosition)
        {
            throw new NotImplementedException();
        }

        private void RemoveRoi(Roi roi)
        {
            Roi removeRoi = m_PersistentViewState.RoisOriginalRects.Keys.FirstOrDefault<Roi>(r => r.LinkedObject.Equals(roi.LinkedObject));
            if (removeRoi == null)
            {
                throw new RoiNotExistsException("ROI does not exits, and therefor can't be removed.");
            }

            //m_Rois.Remove(removeRoi);            
            m_PersistentViewState.RoisOriginalRects.Remove(removeRoi);
        }

        /// <summary>
        /// When a ROI is added, the Rect property of the ROI is relative to the original non-transformed image (after skew of image and shift of ROI),
        /// so immediately after adding it, we must transform it to the current UI transformations state.
        /// If the ROI is added when no changes were applied via UI, there will be no change.
        /// </summary>
        /// <param name="roi"></param>
        private void AddRoi(Roi roi)
        {
            //m_Rois.Add(roi);
            //apply constant shift on every ROI which is added, currently seems that should never be used
            if (!m_PersistentViewState.Shift.IsEmpty)
            {
                roi.Rect = new System.Windows.Rect(m_PersistentViewState.Shift.X + roi.Rect.X, m_PersistentViewState.Shift.Y + roi.Rect.Y, + roi.Rect.Width, roi.Rect.Height);              
            }
            m_PersistentViewState.RoisOriginalRects.Add(roi, roi.Rect);
            //TransformRoi(roi); //yoav 2013.05.16 un-necessary now

            //AddRoiToMaskBitmapSource(roi); //2013.05.19 removed - now will clear mask bitmap and redraw all ROIs on every change of any ROIs collection.                      
        }

        private void RedrawRoisOnMaskBitmapSource(Roi roi)
        {
            if (!m_MaskRedrawEnabled || roi.FillColor == Colors.Transparent) //saves 50% of the work - mask does not draw borders, only fill. Borders are drawn via Rectangles on a canvas.
            {
                return;
            }

            RedrawRoisOnMaskBitmapSource();
        }

        private void RedrawRoisOnMaskBitmapSource()
        {          
            MaskBitmapSource.Clear();            
            //foreach (Roi r in m_Rois)
            //{
            //    AddRoiToMaskBitmapSource(r);
            //}
            AddRoisToMaskBitmapSource(m_Rois);
        }

        public void AddRoisToMaskBitmapSource(RoiCollection rois)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            foreach (var roi in rois)
            {
                System.Windows.Rect rotated = GetRotatedRect(roi);
                
                drawingContext.DrawRectangle(new SolidColorBrush(roi.FillColor), null, rotated);                               
            }

            drawingContext.Close(); 
            this.MaskBitmapSource.Render(drawingVisual);
        }

        public void AddRoiToMaskBitmapSource(Roi roi)
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            System.Windows.Rect rotated = GetRotatedRect(roi);

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(new SolidColorBrush(roi.FillColor), null, rotated);           
                drawingContext.Close();
            }            
            
            this.MaskBitmapSource.Render(drawingVisual);
          
        }

        private System.Windows.Rect GetRotatedRect(Roi roi)
        {
            System.Windows.Rect rotated;
            if (m_PersistentViewState.RotateTransform.Angle == 0)
            {
                rotated = roi.Rect;
            }
            else if (m_PersistentViewState.RotateTransform.Angle == 270)
            {
                rotated = System.Windows.Rect.Transform(roi.Rect, new RotateTransform(90, 0, 0).Value);
                rotated.X += MaskBitmapSource.Width;
            }
            else if (m_PersistentViewState.RotateTransform.Angle == 180)
            {
                rotated = System.Windows.Rect.Transform(roi.Rect, new RotateTransform(180, 0, 0).Value);
                rotated.X += MaskBitmapSource.Width;
                rotated.Y += MaskBitmapSource.Height;
            }
            else if (m_PersistentViewState.RotateTransform.Angle == 90)
            {
                rotated = System.Windows.Rect.Transform(roi.Rect, new RotateTransform(270, 0, 0).Value);
                rotated.Y += MaskBitmapSource.Height;
            }
            else
            {
                throw new NotImplementedException(string.Format("Unsported Rect rotation: {}", m_PersistentViewState.RotateTransform.Angle.ToString()));
            }
            return rotated;
        }

        //private void TransformRoi(Roi transRoi)
        //{            
        //    transRoi.Rect = m_PersistentViewState.RoisOriginalRects[transRoi];
        //    transRoi.Rect.Transform(m_VolatileViewState.TransformGroup.Value); //doesn't work anyway
        //}

        private void Rois_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //removed rois
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldRoi in e.OldItems)
                {
                    Roi removedRoi = oldRoi as Roi;
                    this.RemoveRoi(removedRoi);
                }
                RedrawRoisOnMaskBitmapSource(); //since ROI could have been drawn over another ROI, just clearing the area under it is not enough.
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                //added rois
                foreach (var newRoi in e.NewItems)
                {
                    Roi addedRoi = newRoi as Roi;
                    this.AddRoi(addedRoi);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                //TODO:
                throw new NotImplementedException();
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
            {
                //TODO:
                throw new NotImplementedException();
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                m_PersistentViewState.RoisOriginalRects.Clear();
            }
            else
            {
                //TODO:
                throw new NotImplementedException();
            }

            FormattableRoiCollectionChanged(e);
        }

        private void GoldenRois_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            FormattableRoiCollectionChanged(e);
        }

        private void RecognitionRois_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            FormattableRoiCollectionChanged(e);
        }

        private void TableRois_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            FormattableRoiCollectionChanged(e);
        }

        private void RoisTableRegionRois_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            FormattableRoiCollectionChanged(e);
        }

        private void SelectedRois_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //removed rois
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldRoi in e.OldItems)
                {
                    Roi removedRoi = oldRoi as Roi;
                    if (this.SelectedRoisSubGroup1.Contains(removedRoi))
                    {
                        this.SelectedRoisSubGroup1.Remove(removedRoi);
                    }
                    if (this.SelectedRoisSubGroup2.Contains(removedRoi))
                    {
                        this.SelectedRoisSubGroup2.Remove(removedRoi);
                    }
                    if (this.SelectedRoisSubGroup3.Contains(removedRoi))
                    {
                        this.SelectedRoisSubGroup3.Remove(removedRoi);
                    }
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                //do nothing
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                //TODO:
                throw new NotImplementedException();
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
            {
                //TODO:
                throw new NotImplementedException();
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                //beware of redundant circular calls from child to parent clear. call the clear if the child sub collection was not cleared before.
                if (this.SelectedRoisSubGroup1.Count > 0)
                {
                    this.SelectedRoisSubGroup1.Clear();
                }
                if (this.SelectedRoisSubGroup2.Count > 0)
                {
                    this.SelectedRoisSubGroup2.Clear();
                }
                if (this.SelectedRoisSubGroup3.Count > 0)
                {
                    this.SelectedRoisSubGroup3.Clear();
                }
            }
            else
            {
                //TODO:
                throw new NotImplementedException();
            }
            FormattableRoiCollectionChanged(e);
        }

        private void SelectedRoisSubGroup_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //removed rois
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldRoi in e.OldItems)
                {
                    Roi removedRoi = oldRoi as Roi;
                    if (this.SelectedRois.Contains(removedRoi))
                    {
                        this.SelectedRois.Remove(removedRoi);
                    }
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                //added rois
                foreach (var newRoi in e.NewItems)
                {
                    Roi addedRoi = newRoi as Roi;
                    if (!this.SelectedRois.Contains(addedRoi))
                    {
                        this.SelectedRois.Add(addedRoi);                        
                    }
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                //TODO:
                throw new NotImplementedException();
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
            {
                //TODO:
                throw new NotImplementedException();
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                //remove any ROI from selected parent group that does not belong in any of the 3 subgroups 
                foreach (Roi roi in this.Rois)
                {
                    if (this.SelectedRois.Contains(roi) && !this.SelectedRoisSubGroup1.Contains(roi) && !this.SelectedRoisSubGroup2.Contains(roi) && !this.SelectedRoisSubGroup3.Contains(roi))
                    {
                        this.SelectedRois.Remove(roi);
                    }
                }
            }
            else
            {
                //TODO:
                throw new NotImplementedException();
            }

            FormattableRoiCollectionChanged(e);
        }

        private void FormattableRoiCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //removed rois
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldRoi in e.OldItems)
                {
                    Roi removedRoi = oldRoi as Roi;
                    FormatRoiAccordingToGroupMembership(removedRoi);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                bool maskRedrawEnabled = m_MaskRedrawEnabled;
                //added rois
                foreach (var newRoi in e.NewItems)
                {
                    Roi addedRoi = newRoi as Roi;
                    FormatRoiAccordingToGroupMembership(addedRoi);
                    if (maskRedrawEnabled)
                    {
                        AddRoiToMaskBitmapSource(addedRoi);
                    }
                    m_MaskRedrawEnabled = false; //Yoav 2013.05.21 after change of no auto redraw
                    if (!this.m_Rois.Contains(addedRoi)) //Yoav 2013.05.20
                    {
                        this.m_Rois.Add(addedRoi);
                    }
                    m_MaskRedrawEnabled = maskRedrawEnabled;
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                //TODO:
                throw new NotImplementedException();
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
            {
                //TODO:
                throw new NotImplementedException();
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                //assuming that all the Rois are also in the this.Rois collection, otherwise they will retain their original format.  
                foreach (Roi roi in this.Rois)
                {
                    FormatRoiAccordingToGroupMembership(roi);
                }
            }
            else
            {
                //TODO:
                throw new NotImplementedException();
            }

        }

        private void FormatRoiAccordingToGroupMembership(Roi roi)
        {
            if (m_SelectedRoisSubGroup1.Contains(roi))
            {
                FormatRoiAccordingToGroup(roi, m_SelectedRoisSubGroup1);
            }
            else if (m_SelectedRoisSubGroup2.Contains(roi))
            {
                FormatRoiAccordingToGroup(roi, m_SelectedRoisSubGroup2);
            }
            else if (m_SelectedRoisSubGroup3.Contains(roi))
            {
                FormatRoiAccordingToGroup(roi, m_SelectedRoisSubGroup3);
            }
            else if (m_SelectedRois.Contains(roi))
            {
                FormatRoiAccordingToGroup(roi, m_SelectedRois);
            }
            else if (m_GoldenRois.Contains(roi))
            {
                FormatRoiAccordingToGroup(roi, m_GoldenRois);
            }
            else if (m_RecognitionRois.Contains(roi))
            {
                FormatRoiAccordingToGroup(roi, m_RecognitionRois);
            }
            else if (m_TableRois.Contains(roi))
            {
                FormatRoiAccordingToGroup(roi, m_TableRois);
            }
            else if (m_TableRegionRois.Contains(roi))
            {
                FormatRoiAccordingToGroup(roi, m_TableRegionRois);
            }
            else if (m_Rois.Contains(roi))
            {
                FormatRoiAccordingToGroup(roi, m_Rois);
            }
            else
            {
                throw new RoiNotMemeberOfAnyGroupException("Roi is not member of any group, and therefor can't be formatted.");
            }

            //RedrawRoisOnMaskBitmapSource(roi); //2013.05.19
        }

        public void ResizeBorderThicknessAccordingToGroupMembership(Roi roi, double scale)
        {
            if (m_SelectedRoisSubGroup1.Contains(roi))
            {
                ResizeBorderThicknessRoiAccordingToGroup(roi, m_SelectedRoisSubGroup1, scale);
            }
            else if (m_SelectedRoisSubGroup2.Contains(roi))
            {
                ResizeBorderThicknessRoiAccordingToGroup(roi, m_SelectedRoisSubGroup2, scale);
            }
            else if (m_SelectedRoisSubGroup3.Contains(roi))
            {
                ResizeBorderThicknessRoiAccordingToGroup(roi, m_SelectedRoisSubGroup3, scale);
            }
            else if (m_SelectedRois.Contains(roi))
            {
                ResizeBorderThicknessRoiAccordingToGroup(roi, m_SelectedRois, scale);
            }
            else if (m_GoldenRois.Contains(roi))
            {
                ResizeBorderThicknessRoiAccordingToGroup(roi, m_GoldenRois, scale);
            }
            else if (m_RecognitionRois.Contains(roi))
            {
                ResizeBorderThicknessRoiAccordingToGroup(roi, m_RecognitionRois, scale);
            }
            else if (m_Rois.Contains(roi))
            {
                ResizeBorderThicknessRoiAccordingToGroup(roi, m_Rois, scale);
            }
            else
            {
                throw new RoiNotMemeberOfAnyGroupException("Roi is not member of any group, and therefor can't be formatted.");
            }
        }

        private void FormatRoiAccordingToGroup(Roi roi, RoiCollection collection)
        {
            roi.FillColor = collection.FillColor;
            roi.BorderColor = collection.BorderColor;
            if (collection.CurrentUiBorderThickness > 0)
            {
                roi.BorderThickness = collection.CurrentUiBorderThickness;
            }
            else
            {
                roi.BorderThickness = collection.BorderThickness;
            }
        }

        private void ResizeBorderThicknessRoiAccordingToGroup(Roi roi, RoiCollection collection, double scale)
        {
            collection.CurrentUiBorderThickness = collection.BorderThickness / scale;
            roi.BorderThickness = collection.CurrentUiBorderThickness;
        }
        
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }


    public interface ITransformationBag
    {
        TransformGroup TransformGroup { get; }
    }

    /// <summary>
    /// Transformation Bag - Allows any kind of transformation.
    /// </summary>
    public class VolatileViewState : ITransformationBag, INotifyPropertyChanged
    {
        //transformations
        private readonly ScaleTransform m_ScaleTransform = new ScaleTransform();
        private readonly TranslateTransform m_TranslateTransform = new TranslateTransform();
        private readonly RotateTransform m_RotateTransform = new RotateTransform();
        private readonly SkewTransform m_SkewTransform = new SkewTransform();
        private TransformGroup m_TransformGroup;
     
        public VolatileViewState(double imageWidth, double imageHeight)
        {
            double centerX = imageWidth / 2;
            double centerY = imageHeight / 2;
            m_ScaleTransform.CenterX = centerX;
            m_ScaleTransform.CenterY = centerY;
            m_RotateTransform.CenterX = centerX;
            m_RotateTransform.CenterY = centerY;
            m_SkewTransform.CenterX = centerX;
            m_SkewTransform.CenterY = centerY;
        }

        public ScaleTransform ScaleTransform
        {
            get { return m_ScaleTransform; }
        }

        public TranslateTransform TranslateTransform
        {
            get { return m_TranslateTransform; }
        }

        public RotateTransform RotateTransform
        {
            get { return m_RotateTransform; }
        }

        public SkewTransform SkewTransform
        {
            get { return m_SkewTransform; }
        }

        public TransformGroup TransformGroup
        {
            get
            {
                if (m_TransformGroup == null)
                {
                    m_TransformGroup = new TransformGroup();
                    m_TransformGroup.Children.Add(this.RotateTransform);
                    m_TransformGroup.Children.Add(this.SkewTransform);
                    m_TransformGroup.Children.Add(this.TranslateTransform);
                    m_TransformGroup.Children.Add(this.ScaleTransform);
                }
                return m_TransformGroup;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

    }


    /// <summary>
    /// Intended for use in first stage of intialization of ViewModel, not for UI manipulation.
    /// The values here should not change once they are set, and therefor they are only to be set via the constructor (readonly).
    /// The only rotations allowed are 90 degree multiplications
    /// </summary>
    public class PersistentViewState : ITransformationBag
    {
        private readonly Dictionary<Roi, System.Windows.Rect> m_RoisOriginalRects = new Dictionary<Roi, System.Windows.Rect>();
        private readonly RotateTransform m_RotateTransform = new RotateTransform();
        private readonly SkewTransform m_SkewTransform = new SkewTransform();
        private TransformGroup m_TransformGroup;
        private double m_ImageWidth;
        private double m_ImageHeight;
        private System.Drawing.PointF m_Shift;

        public System.Drawing.PointF Shift
        {
            get { return m_Shift; }           
        }

        public PersistentViewState(double imageWidth, double imageHeight, double skewAngleX, double skewAngleY, Rotation rotation, System.Drawing.PointF shift)
        {
            double centerX = imageWidth / 2;
            double centerY = imageHeight / 2;
            this.m_ImageWidth = imageWidth;
            this.m_ImageHeight = imageHeight;
            this.m_RotateTransform.CenterX = centerX;
            this.m_RotateTransform.CenterY = centerY;
            this.m_RotateTransform.Angle = (int)rotation;
            if (shift.IsEmpty || (shift.X == 0 && shift.Y == 0))
            {
                this.m_Shift = System.Drawing.PointF.Empty;
            }
            else
            {
                this.m_Shift = shift;
            }
            this.m_SkewTransform.CenterX = centerY;
            this.m_SkewTransform.CenterY = centerY;
            this.m_SkewTransform.AngleX = skewAngleX;
            this.m_SkewTransform.AngleY = skewAngleY;
        }             

        public Dictionary<Roi, System.Windows.Rect> RoisOriginalRects
        {
            get { return m_RoisOriginalRects; }
        }

        public RotateTransform RotateTransform
        {
            get { return m_RotateTransform; }
        }

        public SkewTransform SkewTransform
        {
            get { return m_SkewTransform; }
        }

        public TransformGroup TransformGroup
        {
            get
            {
                if (m_TransformGroup == null)
                {
                    m_TransformGroup = new TransformGroup();
                    m_TransformGroup.Children.Add(this.RotateTransform);
                    m_TransformGroup.Children.Add(this.SkewTransform);
                }
                return m_TransformGroup;
            }

        }

        public double VisualWidth
        {
            get
            {
                return m_RotateTransform.Angle % 180 == 0 ? m_ImageWidth : m_ImageHeight;
            }
        }

        public double VisualHeight
        {
            get
            {
                return m_RotateTransform.Angle % 180 == 0 ? m_ImageHeight : m_ImageWidth;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }


    public class Roi : INotifyPropertyChanged
    {
        private object m_LinkedObject;
        private System.Windows.Shapes.Rectangle m_Rectangle;
        private System.Windows.Rect m_Rect;
        private Color m_FillColor =  Colors.Transparent; //Color.FromArgb(60, System.Drawing.Color.Yellow.R, System.Drawing.Color.Yellow.G, System.Drawing.Color.Yellow.B);//default in case not set
        private Color m_BorderColor = Colors.Transparent;
        private double m_BorderThickness = 2;
        private SolidColorBrush m_BorderBrush;
        private SolidColorBrush m_FillBrush;

        public Roi()
        {
        }

        public Roi(object linkedObject, System.Windows.Rect rect)
        {
            m_LinkedObject = linkedObject;
            this.Rect = rect;
        }

        public Roi(object linkedObject, System.Windows.Rect rect, System.Windows.Media.Color fillColor, System.Windows.Media.Color borderColor, double borderThickness)
        {
            m_LinkedObject = linkedObject;
            this.Rect = rect;
            this.FillColor = fillColor;
            this.BorderColor = borderColor;
            this.BorderThickness = borderThickness;
        }

        public object LinkedObject
        {
            get { return m_LinkedObject; }
            set
            {
                m_LinkedObject = value;
                RaisePropertyChanged("LinkedObject");
            }
        }

        public System.Windows.Shapes.Rectangle Rectangle
        {
            get { return m_Rectangle; }
        }

        public System.Windows.Rect Rect
        {
            get { return m_Rect; }
            set
            {
                m_Rect = value;

                if (m_Rectangle == null)
                {
                    m_Rectangle = new System.Windows.Shapes.Rectangle();
                }

                m_Rectangle.Width = m_Rect.Width;
                m_Rectangle.Height = m_Rect.Height;
                //m_Rectangle.RadiusX = 5;
                //m_Rectangle.RadiusY = 5;
                System.Windows.Controls.Canvas.SetLeft(m_Rectangle, m_Rect.X);
                System.Windows.Controls.Canvas.SetTop(m_Rectangle, m_Rect.Y);
                RaisePropertyChanged("Rect");
                RaisePropertyChanged("Rectangle");
            }
        }

        public System.Windows.Media.Color FillColor
        {
            get { return m_FillColor; }
            set
            {
                m_FillColor = value;
                if (m_FillBrush == null)
                {
                    m_FillBrush = new SolidColorBrush();
                }
                m_FillBrush.Color = m_FillColor;//Color.FromArgb(80, m_FillColor.R, m_FillColor.G, m_FillColor.B); //TODO: remove this line later, and restore the line above.                              
                this.Rectangle.Fill = m_FillBrush;
                RaisePropertyChanged("FillColor");
            }
        }

        public Color BorderColor
        {
            get { return m_BorderColor; }
            set
            {
                m_BorderColor = value;
                if (m_BorderBrush == null)
                {
                    m_BorderBrush = new SolidColorBrush();
                }
                m_BorderBrush.Color = m_BorderColor;//Color.FromArgb(160, m_BorderColor.R, m_BorderColor.G, m_BorderColor.B);
                this.Rectangle.Stroke = m_BorderBrush;
                RaisePropertyChanged("BorderColor");
            }
        }

        public double BorderThickness
        {
            get { return m_BorderThickness; }
            set
            {
                m_BorderThickness = value;
                this.Rectangle.StrokeThickness = m_BorderThickness;
                RaisePropertyChanged("BorderThickness");
                RaisePropertyChanged("Rectangle");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }

    public enum Rotation
    {
        None = 0,
        DegreesRight90 = 90,
        Degrees180 = 180,
        DegreesRight270 = 270
    }

    public interface IRoiFormat
    {
        Color FillColor
        {
            get;
            set;
        }

        Color BorderColor
        {
            get;
            set;
        }

        double BorderThickness
        {
            get;
            set;
        }
    }

    public class RoiCollection : ObservableCollection<Roi>, IRoiFormat
    {
        private Color m_FillColor = Colors.Yellow;
        private Color m_BorderColor = Colors.Green;
        private double m_BorderThickness = 2;
        private double m_CurrentUiBorderThickness = 0;

        public double CurrentUiBorderThickness
        {
            get { return m_CurrentUiBorderThickness; }
            set { m_CurrentUiBorderThickness = value; }
        }

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



    public class RoiNotMemeberOfAnyGroupException : Exception
    {
        public RoiNotMemeberOfAnyGroupException(string message) : base(message) { }
    }

    public class RoiNotExistsException : Exception
    {
        public RoiNotExistsException(string message) : base(message) { }
    }
}


