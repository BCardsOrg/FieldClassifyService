using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.UserControls
{
    /// <summary>
    /// The class which the ROI's LinkedObect property is (generic).
    /// </summary>
    //public class IFieldRecognition : IFieldRecognition
    //{ 
    //}


    /// <summary>
    /// Interaction logic for ImageViewer.xaml
    /// </summary>
    public partial class ImageViewer : UserControl
    {
        private Dictionary<double, double> m_SliderValueZoomMapping = null;
        private ImageViewerViewModel m_ViewModel;
        private const int HorizontalScrollbarThickness = 28;    //TODO replace this later with actual size
        private const int VerticalScrollbarThickness = 28;      //TODO replace this later with actual size
        private const int PaddingThickness = 20;

        private bool m_DataContextSwitchedToNewViewModel = false;
        private bool m_TopLeftized = false;
        private bool m_CenteredFirstTime = false;

        //for pan 
        private Point m_ScrollStartPoint;
        private Point m_ScrollTarget;
        private Point m_ScrollStartOffset;

        public ImageViewer()
        {
            DataContextChanged += new DependencyPropertyChangedEventHandler(ImageViewer_DataContextChanged);
            InitializeComponent();
            //this.DataContext = m_ViewModel;
            InitSlider();
            //this.scrollViewer.LayoutUpdated += scrollViewer_LayoutUpdated;
            //this.scrollViewer.ScrollChanged += scrollViewer_ScrollChanged;            
            //RenderOptions.SetBitmapScalingMode(this.displayedImage, BitmapScalingMode.HighQuality);
        }



        public ImageViewer(ImageViewerViewModel imageViewerViewModel)
        {
            InitializeComponent();
            this.ViewModel = imageViewerViewModel;
            this.DataContext = m_ViewModel;
            InitSlider();
            //RenderOptions.SetBitmapScalingMode(this.displayedImage, BitmapScalingMode.Fant);            
        }

        public ImageViewerViewModel ViewModel
        {
            get { return m_ViewModel; }
            set
            {
                if (m_ViewModel != null)
                {
                    //unscubscribe from the events associated with this object
                    m_ViewModel.Rois.CollectionChanged -= Rois_CollectionChanged;
                    m_ViewModel.SelectedRois.CollectionChanged -= CentralRois_CollectionChanged;
                }
                m_ViewModel = value;
                if (m_ViewModel != null)
                {
                    ApplyViewModelDependancies();
                }
            }
        }

        private void ImageViewer_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext != null && this.DataContext != this.ViewModel)
            {
                if (this.DataContext is ImageViewerViewModel)
                {
                    this.ViewModel = this.DataContext as ImageViewerViewModel;
                    m_DataContextSwitchedToNewViewModel = true;
                    m_TopLeftized = false;
                    m_CenteredFirstTime = false;

                    this.scrollViewer.LayoutUpdated += scrollViewer_LayoutUpdated;
                    this.scrollViewer.ScrollChanged += scrollViewer_ScrollChanged;
                }
            }
            if (this.DataContext == null)
            {
                this.ViewModel = null;
            }
        }


        /// <summary>
        /// This is the means of detecting whether the image displayed in the control has actually changed.
        /// If it has changed, the are a 2 actions that need to take place:
        /// 1. centering around selected ROIs
        /// 2. elimination of scrolling margins(top-leftize)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scrollViewer_LayoutUpdated(object sender, EventArgs e)
        {
            VerifyImageLayoutInitialized();
        }

        /// <summary>
        /// will fire after first part of layout update of scrollviewer occurs scrollViewer_LayoutUpdated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            VerifyImageLayoutInitialized();
        }

       
        void ImageViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {            
            if (m_ViewModel != null && m_ViewModel.ScaleToBounds)
            {
                ChangeSliderDefaultValue();
                CenterAroundRois(); 
            }            
        }      

        

        private void VerifyImageLayoutInitialized()
        {
            if (m_DataContextSwitchedToNewViewModel
               && !m_TopLeftized && this.scrollViewer.ExtentWidth > 0)
            {
                TopLeftizeViewport(true);
                ForceMeasureUi();
                m_TopLeftized = true;
                m_DataContextSwitchedToNewViewModel = false;
            }
            else if (m_TopLeftized && !m_CenteredFirstTime)
            {
                if (m_ViewModel.SelectedRois.Count > 0)
                {
                    CenterAroundRois();
                }
                m_CenteredFirstTime = true;
                ForceMeasureUi();

                this.scrollViewer.LayoutUpdated -= scrollViewer_LayoutUpdated;
                this.scrollViewer.ScrollChanged -= scrollViewer_ScrollChanged;
                this.SizeChanged += ImageViewer_SizeChanged;
            }
        }

        private void ForceMeasureUi()
        {
            this.scrollViewer.InvalidateVisual();
            #region a note on re-measureing
            //all following options cause the same effect, since  if you change something in the layout, it is re-measured..
            //a:
            //Button b = new Button();
            //this.gridContainer.Children.Add(b);
            //this.gridContainer.Children.Remove(b);
            //b:
            //this.scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            //this.scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            //c: this causes the child controls of dock panel to remeasure as well
            //(this.scrollViewer.Parent as UIElement).InvalidateVisual();

            //further documentation here:
            //http://stackoverflow.com/questions/7801680/how-can-i-manually-tell-an-owner-drawn-wpf-control-to-refresh-without-executing
            //http://msdn.microsoft.com/en-us/library/system.windows.uielement.invalidatevisual.aspx
            #endregion
        }

        private RectRelativeToViewport GetOverrideRectRelativeToViewport()
        {
            if (m_ViewModel.SelectedRois.Count == 0)
            {
                return null;
            }
            RectRelativeToViewport r = new RectRelativeToViewport();

            Rect rect = GetCentralRoisBoundingRect();
            if (m_ViewModel.PersistentViewState.VisualWidth - rect.Right < rect.Left)
            {
                r.HorizontalState = RectRelativeToViewport.HorizontalRelativeToViewPort.Right;
            }
            else
            {
                r.HorizontalState = RectRelativeToViewport.HorizontalRelativeToViewPort.Left;
            }

            if (m_ViewModel.PersistentViewState.VisualHeight - rect.Bottom < rect.Top)
            {
                r.VerticalState = RectRelativeToViewport.VerticalRelativeToViewPort.Bottom;
            }
            else
            {
                r.VerticalState = RectRelativeToViewport.VerticalRelativeToViewPort.Top;
            }

            return r;
        }

        private void ApplyViewModelDependancies()
        {
            ChangeSliderDefaultValue();

            //TODO: decide if to reset zoom on change of ViewModel change. 
            //currently, in case the control was already loaded before, and the image changed, preserve the zoom value and change the current view model scale to match.
            UpdateScale(); //TODO: in case the above is invalid - change the slider to 100%, and remove this line

            ApplyTransformations();
            //InitSliderValueZoomMapping(); //moved to constructors so that slidebar will be ok even if there is no view model.            
            BatchAddViewModelRoisToCanvas();
            m_ViewModel.Rois.CollectionChanged += Rois_CollectionChanged;
            m_ViewModel.SelectedRois.CollectionChanged += CentralRois_CollectionChanged;
        }

        /// <summary>
        /// Should be called only once from Init().
        /// </summary>
        private void BatchAddViewModelRoisToCanvas()
        {
            this.drawingCanvas.Children.Clear(); //there should be only ROI rectangles on the drawing Canvas. On init is should be empty anyway...
            //m_ViewModel.Rois.ToList<Roi<PlaceHolderClassForFieldsPagesInterface>>().ForEach(roi => this.drawingCanvas.Children.Add(roi.Rectangle));
            foreach (var roi in m_ViewModel.Rois)
            {
                if (roi.BorderColor != Colors.Transparent)
                {
                    this.drawingCanvas.Children.Add(roi.Rectangle);
                }
                //m_ViewModel.AddRoi(roi);
                m_ViewModel.AddRoiToMaskBitmapSource(roi);
            }
        }

        private void CenterAroundRois()
        {
            if (m_ViewModel.SelectedRois.Count > 0)
            {
                CenterAroundRect(GetCentralRoisBoundingRect());
            }
        }

        private void CenterAroundRois(RectRelativeToViewport overrideRectRelativeToViewport)
        {
            if (m_ViewModel.SelectedRois.Count > 0)
            {
                CenterAroundRect(GetCentralRoisBoundingRect(), overrideRectRelativeToViewport);
            }
        }

        /// <summary>
        /// This user control (Image Viewer) is listening to the changes in the ViewModel's ROI collection.
        /// When a ROI is added, or removed, the drawing canvas Rectangles (children) collection is updated, and the color of the last item is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rois_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //removed rois
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldRoi in e.OldItems)
                {
                    Roi removedRoi = oldRoi as Roi;
                    if (removedRoi.BorderColor != Colors.Transparent)
                    {
                        if (this.drawingCanvas.Children.Contains(removedRoi.Rectangle))
                        {
                            this.drawingCanvas.Children.Remove(removedRoi.Rectangle);
                        }
                    }
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                //added rois
                foreach (var newRoi in e.NewItems)
                {
                    Roi addedRoi = newRoi as Roi;
                    if (addedRoi.BorderColor != Colors.Transparent)
                    {
                        if (!this.drawingCanvas.Children.Contains(addedRoi.Rectangle))
                        {
                            this.drawingCanvas.Children.Add(addedRoi.Rectangle);
                        }
                    }
                }
                UpdateRectangleBorderThickness();
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                //TODO: in case ROI is last in list - need to change color
                throw new NotImplementedException();
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
            {
                //TODO: in case ROI moved to be last in list - need to change color, and change the previous ROI which was last to default color
                throw new NotImplementedException();
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                this.drawingCanvas.Children.Clear();
            }
            else
            {
                //TODO:
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///center according to bounding rectangle around central ROIs group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CentralRois_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CenterAroundRois();
        }

        private Rect GetCentralRoisBoundingRect()
        {
            Rect centerRect = new Rect();
            var minLeft = m_ViewModel.SelectedRois.Aggregate((curMin, rect) => (curMin.Rect.X <= rect.Rect.X ? curMin : rect));
            var maxRight = m_ViewModel.SelectedRois.Aggregate((curMax, rect) => (curMax.Rect.Right >= rect.Rect.Right ? curMax : rect));
            var minTop = m_ViewModel.SelectedRois.Aggregate((curMin, rect) => (curMin.Rect.Y <= rect.Rect.Y ? curMin : rect));
            var maxBottom = m_ViewModel.SelectedRois.Aggregate((curMax, rect) => (curMax.Rect.Bottom >= rect.Rect.Bottom ? curMax : rect));


            centerRect.X = minLeft.Rect.X;
            centerRect.Width = maxRight.Rect.Right - minLeft.Rect.X;
            centerRect.Y = minTop.Rect.Y;
            centerRect.Height = maxBottom.Rect.Bottom - minTop.Rect.Y;

            return centerRect;
        }

        /// <summary>
        /// Apply transformation on this user control's XAML controls: image, and canvas with rectangles (ROIs)
        /// </summary>
        private void ApplyTransformations()
        {
            this.displayedImage.RenderTransform = this.m_ViewModel.TransformGroup;
            this.drawingCanvas.LayoutTransform = this.m_ViewModel.VolatileViewState.TransformGroup;
            //this.drawingCanvas.RenderTransform = this.m_ViewModel.VolatileViewState.TransformGroup;
            //this.layoutCanvas.LayoutTransform = this.m_ViewModel.VolatileViewState.TransformGroup;          
            UpdateRectangleBorderThickness();
        }

        private void UpdateRectangleBorderThickness()
        {
            double scale = m_SliderValueZoomMapping[slider.Value];
            foreach (var r in m_ViewModel.Rois)
            {
                if (r.BorderColor != Colors.Transparent)
                {
                    m_ViewModel.ResizeBorderThicknessAccordingToGroupMembership(r, scale);
                }
            }
        }

        private void ScrollToBitmapCoordinatesPoint(Point p)
        {
            double marginX = (this.scrollViewer.ExtentWidth / 2) - (m_ViewModel.PersistentViewState.VisualWidth * m_ViewModel.VolatileViewState.ScaleTransform.ScaleX / 2);
            double marginY = (this.scrollViewer.ExtentHeight / 2) - (m_ViewModel.PersistentViewState.VisualHeight * m_ViewModel.VolatileViewState.ScaleTransform.ScaleY / 2);
            Point viewedPoint = new Point(p.X * m_ViewModel.VolatileViewState.ScaleTransform.ScaleX, p.Y * m_ViewModel.VolatileViewState.ScaleTransform.ScaleY);
            this.scrollViewer.ScrollToHorizontalOffset(marginX + viewedPoint.X - this.scrollViewer.ViewportWidth / 2);
            this.scrollViewer.ScrollToVerticalOffset(marginY + viewedPoint.Y - this.scrollViewer.ViewportHeight / 2);
        }

        /// <summary>
        /// Scroll viewport towards top-left corner of image only to omitt margins if any are visible (e.g. if is zoomed into middle, there will be no scrolling).
        /// </summary>
        /// <param name="updateLayout"></param>
        private void EliminateViewportMargins(bool updateLayout = false)
        {
            if (m_ViewModel == null)
            {
                return;
            }
            double marginX = (this.scrollViewer.ExtentWidth / 2) - (m_ViewModel.PersistentViewState.VisualWidth * m_ViewModel.VolatileViewState.ScaleTransform.ScaleX / 2);
            double marginY = (this.scrollViewer.ExtentHeight / 2) - (m_ViewModel.PersistentViewState.VisualHeight * m_ViewModel.VolatileViewState.ScaleTransform.ScaleY / 2);
            if (this.scrollViewer.HorizontalOffset < marginX)
            {
                this.scrollViewer.ScrollToHorizontalOffset(marginX);
            }
            else if (//this.scrollViewer.ViewportWidth > m_ViewModel.PersistentViewState.VisualWidth * m_ViewModel.VolatileViewState.ScaleTransform.ScaleX &&
                this.scrollViewer.HorizontalOffset + this.scrollViewer.ViewportWidth > this.scrollViewer.ExtentWidth - marginX)
            {
                //this.scrollViewer.ScrollToHorizontalOffset(this.scrollViewer.ExtentWidth - marginX - m_ViewModel.PersistentViewState.VisualWidth * m_ViewModel.VolatileViewState.ScaleTransform.ScaleX);
                this.scrollViewer.ScrollToHorizontalOffset(this.scrollViewer.ExtentWidth - marginX - this.scrollViewer.ViewportWidth);
            }

            if (this.scrollViewer.VerticalOffset < marginY)
            {
                this.scrollViewer.ScrollToVerticalOffset(marginY);
            }
            //now if still there is a bottom margin, scroll until before there is a top margin breech
            else if (//this.scrollViewer.VerticalOffset + this.scrollViewer.ViewportHeight > m_ViewModel.PersistentViewState.VisualHeight * m_ViewModel.VolatileViewState.ScaleTransform.ScaleY &&
                this.scrollViewer.VerticalOffset + this.scrollViewer.ViewportHeight > this.scrollViewer.ExtentHeight - marginY)
            {
                //this.scrollViewer.ScrollToVerticalOffset(this.scrollViewer.ExtentHeight - marginY - m_ViewModel.PersistentViewState.VisualHeight * m_ViewModel.VolatileViewState.ScaleTransform.ScaleY);
                this.scrollViewer.ScrollToVerticalOffset(this.scrollViewer.ExtentHeight - marginY - this.scrollViewer.ViewportHeight);
            }


            if (updateLayout)
            {
                (this.scrollViewer.Parent as DockPanel).UpdateLayout();
                //TODO: consider changing to:
                //this.scrollViewer.InvalidateVisual();
            }

        }

        /// <summary>
        /// Scroll viewport to top-left corner of image, regardless of current scroll position 
        /// </summary>
        /// <param name="updateLayout"></param>
        private void TopLeftizeViewport(bool updateLayout = false)
        {
            if (m_ViewModel == null)
            {
                return;
            }
            double marginX = (this.scrollViewer.ExtentWidth / 2) - (m_ViewModel.PersistentViewState.VisualWidth * m_ViewModel.VolatileViewState.ScaleTransform.ScaleX / 2);
            double marginY = (this.scrollViewer.ExtentHeight / 2) - (m_ViewModel.PersistentViewState.VisualHeight * m_ViewModel.VolatileViewState.ScaleTransform.ScaleY / 2);

            this.scrollViewer.ScrollToHorizontalOffset(marginX);
            this.scrollViewer.ScrollToVerticalOffset(marginY);

            if (updateLayout)
            {
                (this.scrollViewer.Parent as DockPanel).UpdateLayout();
                //TODO: consider changing to:
                //this.scrollViewer.InvalidateVisual();
            }

        }

        private Rect GetCurrentViewportBitmapCoordinatesRect()
        {
            double scaleBeforeZoomOut = m_ViewModel.VolatileViewState.ScaleTransform.ScaleX;
            double marginXbefore = (this.scrollViewer.ExtentWidth / 2) - (m_ViewModel.PersistentViewState.VisualWidth * m_ViewModel.VolatileViewState.ScaleTransform.ScaleX / 2);
            double marginYbefore = (this.scrollViewer.ExtentHeight / 2) - (m_ViewModel.PersistentViewState.VisualHeight * m_ViewModel.VolatileViewState.ScaleTransform.ScaleY / 2);


            double x0 = (this.scrollViewer.HorizontalOffset - marginXbefore) / m_ViewModel.VolatileViewState.ScaleTransform.ScaleX;
            double width = this.scrollViewer.ViewportWidth / m_ViewModel.VolatileViewState.ScaleTransform.ScaleX;
            double y0 = (this.scrollViewer.VerticalOffset - marginYbefore) / m_ViewModel.VolatileViewState.ScaleTransform.ScaleY;
            double height = this.scrollViewer.ViewportHeight / m_ViewModel.VolatileViewState.ScaleTransform.ScaleY;

            ////show rect for debug
            //Rectangle r = new Rectangle();
            //Canvas.SetLeft(r, x0);
            //Canvas.SetTop(r, y0);
            //r.Width = width;
            //r.Height = height;
            //SolidColorBrush b = new SolidColorBrush();
            //b.Color = Color.FromArgb(80, Colors.Blue.R, Colors.Blue.G, Colors.Blue.B);
            //r.Fill = b;
            //this.drawingCanvas.Children.Add(r);          

            return new Rect(x0, y0, width, height);
        }

        private void SetViewLimitsZoom()
        {
            if (!this.ViewModel.ScaleToBounds)
            {
                return;
            }

            var rect = GetCentralRoisBoundingRect();
            this.ViewModel.Zoom = Math.Min(1, Math.Min((this.ActualWidth - 2 * this.ViewModel.PaddingThickness - HorizontalScrollbarThickness) / (rect.Width), (this.ActualHeight - 2 * this.ViewModel.PaddingThickness - VerticalScrollbarThickness) / (rect.Height)));
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (slider.Value <= this.slider.Minimum)
            {
                return;
            }
            slider.Value--;
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (slider.Value >= this.slider.Maximum)
            {
                return;
            }
            slider.Value++;
        }

        private void UpdateScaleWithCentering()
        {
            Rect r0 = GetCurrentViewportBitmapCoordinatesRect();

            UpdateScale();

            (this.scrollViewer.Parent as DockPanel).UpdateLayout();
            //TODO: consider changing to:
            //this.scrollViewer.InvalidateVisual();
            ScrollToBitmapCoordinatesPoint(new Point((r0.X + r0.Width / 2), (r0.Y + r0.Height / 2)));
            (this.scrollViewer.Parent as DockPanel).UpdateLayout();
            //TODO: consider changing to:
            //this.scrollViewer.InvalidateVisual();
            EliminateViewportMargins();
        }

        private void UpdateScale()
        {
            if (!m_SliderValueZoomMapping.ContainsKey(slider.Value))
            {
                return;
            }

            double scale = m_SliderValueZoomMapping[slider.Value];
            this.textBlockSliderValue.Text = m_SliderValueZoomMapping[slider.Value].ToString("0%");
            m_ViewModel.VolatileViewState.ScaleTransform.ScaleX = scale;
            m_ViewModel.VolatileViewState.ScaleTransform.ScaleY = scale;
            m_ViewModel.VolatileViewState.ScaleTransform.CenterX = m_ViewModel.BitmapSource.PixelWidth / 2;
            m_ViewModel.VolatileViewState.ScaleTransform.CenterY = m_ViewModel.BitmapSource.PixelHeight / 2;
            ApplyTransformations();
        }

        /// <summary>
        /// maps the slider values to zoom scale  values
        /// </summary>
        private void InitSlider()
        {
            m_SliderValueZoomMapping = new Dictionary<double, double>(20);
            m_SliderValueZoomMapping.Add(1, 0.05);
            m_SliderValueZoomMapping.Add(2, 0.10);
            m_SliderValueZoomMapping.Add(3, 0.15);
            m_SliderValueZoomMapping.Add(4, 0.20);
            m_SliderValueZoomMapping.Add(5, 0.25);
            m_SliderValueZoomMapping.Add(6, 0.30);
            m_SliderValueZoomMapping.Add(7, 0.35);
            m_SliderValueZoomMapping.Add(8, 0.40);
            m_SliderValueZoomMapping.Add(9, 0.45);
            m_SliderValueZoomMapping.Add(10, 0.50);
            m_SliderValueZoomMapping.Add(11, 0.60);
            m_SliderValueZoomMapping.Add(12, 0.70);
            m_SliderValueZoomMapping.Add(13, 0.80);
            m_SliderValueZoomMapping.Add(14, 0.90);
            m_SliderValueZoomMapping.Add(15, 0.95);
            m_SliderValueZoomMapping.Add(16, 1.00);//middle
            m_SliderValueZoomMapping.Add(17, 1.05);
            m_SliderValueZoomMapping.Add(18, 1.10);
            m_SliderValueZoomMapping.Add(19, 1.15);
            m_SliderValueZoomMapping.Add(20, 1.20);
            m_SliderValueZoomMapping.Add(21, 1.25);
            m_SliderValueZoomMapping.Add(22, 1.30);
            m_SliderValueZoomMapping.Add(23, 1.35);
            m_SliderValueZoomMapping.Add(24, 1.40);
            m_SliderValueZoomMapping.Add(25, 1.45);
            m_SliderValueZoomMapping.Add(26, 1.50);
            m_SliderValueZoomMapping.Add(27, 1.75);
            m_SliderValueZoomMapping.Add(28, 2.00);
            m_SliderValueZoomMapping.Add(29, 2.50);
            m_SliderValueZoomMapping.Add(30, 3.00);
            m_SliderValueZoomMapping.Add(31, 4.00);

            this.slider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(slider_ValueChanged);
        }


        private void ChangeSliderDefaultValue()
        {
            if (this.ViewModel != null)
            {
                SetViewLimitsZoom();
                this.slider.Value = FindClosestSliderValue();
            }
        }

        private double FindClosestSliderValue(bool notGreater = true)
        {
            double closestMatch = 0.05;
            double closestMatchKey = 1;
            foreach (KeyValuePair<double, double> p in m_SliderValueZoomMapping)
            {
                double d = p.Value;
                if (
                    (!notGreater && Math.Abs(this.ViewModel.Zoom - d) <= Math.Abs(this.ViewModel.Zoom - closestMatch))
                    || (notGreater && this.ViewModel.Zoom - d >= 0 && (this.ViewModel.Zoom - d) <= (this.ViewModel.Zoom - closestMatch))
                    )
                {
                    closestMatch = d;
                    closestMatchKey = p.Key;
                }
            }

            return closestMatchKey;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_ViewModel == null)
            {
                return;
            }
            UpdateScaleWithCentering();
        }

        #region Center Around ROI

        /// <summary>
        /// TODO: Yoav 2013.05.01: Fix in cases where viewport is smaller that CenteredRois bounding rect, and then in some cases the scrolling is behaving strangely (don't know exactly which cases yet).
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="surroundBorderWidth"></param>
        /// <param name="surroundBorderHeight"></param>
        private void CenterAroundRect(Rect rect, double surroundBorderWidth = PaddingThickness, double surroundBorderHeight = PaddingThickness, RectRelativeToViewport overrideRectRelativeToViewport = null)
        {
            if (this.scrollViewer.ExtentWidth == 0)
            {
                return;
            }
            //actual width of displayed area inside the scrollViewer
            //this.scrollViewer.ExtentWidth
            //this.scrollViewer.ExtentHeight            
            //position after ScrollToVerticalOffset should equal  this.scrollViewer.VerticalOffset 
            //position after ScrollToHorizontalOffset should equal  this.scrollViewer.HorizontalOffset 

            //When trying to scroll to H/V offset:
            //center of scrollable area is determined ExtentHight / 2,  ExtentWidth / 2
            //after this normalize the 0,0 of image after scale to determine the actual location of the rectangle on the scrollViewer
            //then, when trying to resize, the ScrollableHeight, ScrollableWidth may be smaller than the value you want to pass ScrollToVerticalOffset, ScrollToHorizontalOffset, 
            //and in this case it will take the ScrollableHeight, ScrollableWidth value instead.

            double deltaX = (this.scrollViewer.ExtentWidth / 2) - (m_ViewModel.PersistentViewState.VisualWidth * m_ViewModel.VolatileViewState.ScaleTransform.ScaleX / 2);
            double deltaY = (this.scrollViewer.ExtentHeight / 2) - (m_ViewModel.PersistentViewState.VisualHeight * m_ViewModel.VolatileViewState.ScaleTransform.ScaleY / 2);

            //normalize to current viewport coordinates + scale
            double roiExtendedBoundLeft = rect.X * m_ViewModel.VolatileViewState.ScaleTransform.ScaleX + deltaX - surroundBorderWidth;
            double roiExtendedBoundTop = rect.Y * m_ViewModel.VolatileViewState.ScaleTransform.ScaleY + deltaY - surroundBorderHeight;
            double roiExtendedBoundRight = roiExtendedBoundLeft + rect.Width * m_ViewModel.VolatileViewState.ScaleTransform.ScaleX + 2 * surroundBorderWidth;
            double roiExtendedBoundBottom = roiExtendedBoundTop + rect.Height * m_ViewModel.VolatileViewState.ScaleTransform.ScaleY + 2 * surroundBorderHeight;

            //in case the deltas are negative (ROI outside visible area, can this actually happen?)
            roiExtendedBoundLeft = Math.Max(roiExtendedBoundLeft, 0);
            roiExtendedBoundLeft = Math.Max(roiExtendedBoundLeft, deltaX);  //in ROI is outside the margin (actually happens when the surroundBorderWidth (=20) makes the ROI extended borders go outside the image 
            roiExtendedBoundTop = Math.Max(roiExtendedBoundTop, 0);
            roiExtendedBoundTop = Math.Max(roiExtendedBoundTop, deltaY);    //in ROI is outside the margin (actually happens when the surroundBorderWidth (=20) makes the ROI extended borders go outside the image 
            //roiExtendedBoundRight = Math.Min(roiExtendedBoundRight, this.scrollViewer.ExtentWidth - deltaX);
            //roiExtendedBoundBottom = Math.Min(roiExtendedBoundBottom, this.scrollViewer.ExtentHeight - deltaY);  

            RectRelativeToViewport rp;
            if (overrideRectRelativeToViewport != null)
            {
                rp = overrideRectRelativeToViewport;
            }
            else
            {
                rp = GetRectRelativeToViewport(roiExtendedBoundLeft, roiExtendedBoundTop, roiExtendedBoundRight, roiExtendedBoundBottom);
            }

            //horizontal aspect
            double visibleRightMargin = GetVisibleRightMargin();
            double visibleLeftMargin = GetVisibleLeftMargin();
            double eliminateRightMargin = this.scrollViewer.HorizontalOffset - visibleRightMargin;
            double eliminateLeftMargin = this.scrollViewer.HorizontalOffset + visibleLeftMargin;
            if (rp.HorizontalState == RectRelativeToViewport.HorizontalRelativeToViewPort.Left)
            {
                this.scrollViewer.ScrollToHorizontalOffset(Math.Min( /*deltaX +*/ roiExtendedBoundLeft, eliminateRightMargin));
            }
            else if (rp.HorizontalState == RectRelativeToViewport.HorizontalRelativeToViewPort.Right)
            {
                this.scrollViewer.ScrollToHorizontalOffset(Math.Max(roiExtendedBoundRight - this.scrollViewer.ViewportWidth, eliminateLeftMargin));
            }
            else
            {
                if (visibleLeftMargin > 0)
                {
                    this.scrollViewer.ScrollToHorizontalOffset(eliminateLeftMargin);
                }
                else if (visibleRightMargin > 0)
                {
                    this.scrollViewer.ScrollToHorizontalOffset(eliminateRightMargin);
                }
            }


            //vertical aspect
            double visibleBottomMargin = GetVisibleBottomMargin();
            double visibleTopMargin = GetVisibleTopMargin();
            double eliminateBottomMargin = this.scrollViewer.VerticalOffset - visibleBottomMargin;
            double eliminateTopMargin = this.scrollViewer.VerticalOffset + visibleTopMargin;
            if (rp.VerticalState == RectRelativeToViewport.VerticalRelativeToViewPort.Top)
            {
                this.scrollViewer.ScrollToVerticalOffset(Math.Min(/*deltaY +*/ roiExtendedBoundTop, eliminateBottomMargin));
            }
            else if (rp.VerticalState == RectRelativeToViewport.VerticalRelativeToViewPort.Bottom)
            {
                this.scrollViewer.ScrollToVerticalOffset(Math.Max(roiExtendedBoundBottom - this.scrollViewer.ViewportHeight, eliminateTopMargin));
            }
            else
            {
                if (visibleTopMargin > 0)
                {
                    this.scrollViewer.ScrollToVerticalOffset(eliminateTopMargin);
                }
                else if (visibleBottomMargin > 0)
                {
                    this.scrollViewer.ScrollToVerticalOffset(eliminateBottomMargin);
                }
            }
        }


        private double GetVisibleRightMargin()
        {
            double marginX = (this.scrollViewer.ExtentWidth / 2) - (m_ViewModel.PersistentViewState.VisualWidth * m_ViewModel.VolatileViewState.ScaleTransform.ScaleX / 2);
            //double marginY = (this.scrollViewer.ExtentHeight / 2) - (m_ViewModel.PersistentViewState.VisualHeight * m_ViewModel.VolatileViewState.ScaleTransform.ScaleY / 2);
            double d = this.scrollViewer.HorizontalOffset + this.scrollViewer.ViewportWidth - marginX - m_ViewModel.PersistentViewState.VisualWidth * m_ViewModel.VolatileViewState.ScaleTransform.ScaleX;
            return Math.Max(d, 0);
        }

        private double GetVisibleLeftMargin()
        {
            double marginX = (this.scrollViewer.ExtentWidth / 2) - (m_ViewModel.PersistentViewState.VisualWidth * m_ViewModel.VolatileViewState.ScaleTransform.ScaleX / 2);
            //double marginY = (this.scrollViewer.ExtentHeight / 2) - (m_ViewModel.PersistentViewState.VisualHeight * m_ViewModel.VolatileViewState.ScaleTransform.ScaleY / 2);
            double d = marginX - this.scrollViewer.HorizontalOffset;
            return Math.Max(d, 0);
        }

        private double GetVisibleBottomMargin()
        {
            double marginY = (this.scrollViewer.ExtentHeight / 2) - (m_ViewModel.PersistentViewState.VisualHeight * m_ViewModel.VolatileViewState.ScaleTransform.ScaleY / 2);
            //double marginY = (this.scrollViewer.ExtentHeight / 2) - (m_ViewModel.PersistentViewState.VisualHeight * m_ViewModel.VolatileViewState.ScaleTransform.ScaleY / 2);
            double d = this.scrollViewer.VerticalOffset + this.scrollViewer.ViewportHeight - marginY - m_ViewModel.PersistentViewState.VisualHeight * m_ViewModel.VolatileViewState.ScaleTransform.ScaleY;
            return Math.Max(d, 0);
        }

        private double GetVisibleTopMargin()
        {
            double marginY = (this.scrollViewer.ExtentHeight / 2) - (m_ViewModel.PersistentViewState.VisualHeight * m_ViewModel.VolatileViewState.ScaleTransform.ScaleY / 2);
            //double marginY = (this.scrollViewer.ExtentHeight / 2) - (m_ViewModel.PersistentViewState.VisualHeight * m_ViewModel.VolatileViewState.ScaleTransform.ScaleY / 2);
            double d = marginY - this.scrollViewer.VerticalOffset;
            return Math.Max(d, 0);
        }

        private void CenterAroundRect(Rect rect, RectRelativeToViewport overrideRectRelativeToViewport)
        {
            //TODO: change 20 to ViewMode.PaddingThickness
            CenterAroundRect(rect, PaddingThickness, PaddingThickness, overrideRectRelativeToViewport);
        }

        private RectRelativeToViewport GetRectRelativeToViewport(double roiExtendedBoundLeft, double roiExtendedBoundTop, double roiExtendedBoundRight, double roiExtendedBoundBottom)
        {
            RectRelativeToViewport rectRelativeToViewport = new RectRelativeToViewport();
            //first, trim the ROI rectangle in case viewport is smaller ("clip to bounds" of viewport)
            //  aribitrarily decided (can change this) that the top left of the rect is the most important.
            if (roiExtendedBoundRight - roiExtendedBoundLeft > this.scrollViewer.ViewportWidth)
            {
                roiExtendedBoundRight = roiExtendedBoundLeft + this.scrollViewer.ViewportWidth - 1;
            }
            if (roiExtendedBoundBottom - roiExtendedBoundTop > this.scrollViewer.ViewportHeight)
            {
                roiExtendedBoundBottom = roiExtendedBoundTop + this.scrollViewer.ViewportHeight - 1;
            }

            //horizontal
            if (this.scrollViewer.HorizontalOffset <= roiExtendedBoundLeft
                && this.scrollViewer.ViewportWidth + this.scrollViewer.HorizontalOffset >= roiExtendedBoundRight)
            {
                rectRelativeToViewport.HorizontalState = RectRelativeToViewport.HorizontalRelativeToViewPort.Contained;
            }
            else if (this.scrollViewer.HorizontalOffset > roiExtendedBoundLeft)
            {
                rectRelativeToViewport.HorizontalState = RectRelativeToViewport.HorizontalRelativeToViewPort.Left;
            }
            else
            {
                rectRelativeToViewport.HorizontalState = RectRelativeToViewport.HorizontalRelativeToViewPort.Right;

            }

            //vertical
            if (this.scrollViewer.VerticalOffset <= roiExtendedBoundTop
                && this.scrollViewer.ViewportHeight + this.scrollViewer.VerticalOffset >= roiExtendedBoundBottom)
            {
                rectRelativeToViewport.VerticalState = RectRelativeToViewport.VerticalRelativeToViewPort.Contained;
            }
            else if (this.scrollViewer.VerticalOffset > roiExtendedBoundTop)
            {
                rectRelativeToViewport.VerticalState = RectRelativeToViewport.VerticalRelativeToViewPort.Top;
            }
            else
            {
                rectRelativeToViewport.VerticalState = RectRelativeToViewport.VerticalRelativeToViewPort.Bottom;
            }

            return rectRelativeToViewport;
        }

        #endregion

        private void ScrollToImageTopLeft()
        {
            double deltaX = (this.scrollViewer.ExtentWidth / 2) - (m_ViewModel.PersistentViewState.VisualWidth * m_ViewModel.VolatileViewState.ScaleTransform.ScaleX / 2);
            double deltaY = (this.scrollViewer.ExtentHeight / 2) - (m_ViewModel.PersistentViewState.VisualHeight * m_ViewModel.VolatileViewState.ScaleTransform.ScaleY / 2);
            this.scrollViewer.ScrollToHorizontalOffset(deltaX);
            this.scrollViewer.ScrollToVerticalOffset(deltaY);
        }

        #region Pan
        ///
        ///thanks to: http://www.codeproject.com/Articles/37349/Creating-A-Scrollable-Control-Surface-In-WPF
        ///


        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (this.scrollViewer.IsMouseOver)
            {
                // Save starting point, used later when determining how much to scroll.
                m_ScrollStartPoint = e.GetPosition(this);

                if (m_ScrollStartPoint.X < this.scrollViewer.ViewportWidth && m_ScrollStartPoint.Y < this.scrollViewer.ViewportHeight)
                {
                    m_ScrollStartOffset.X = this.scrollViewer.HorizontalOffset;
                    m_ScrollStartOffset.Y = this.scrollViewer.VerticalOffset;

                    // Update the cursor if can scroll or not.
                    this.Cursor = (this.scrollViewer.ExtentWidth > this.scrollViewer.ViewportWidth) || (this.scrollViewer.ExtentHeight > this.scrollViewer.ViewportHeight) ? Cursors.ScrollAll : Cursors.Arrow;

                    this.CaptureMouse();
                }
            }

            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                Point currentPoint = e.GetPosition(this);

                // Determine the new amount to scroll.
                Point delta = new Point(m_ScrollStartPoint.X - currentPoint.X, m_ScrollStartPoint.Y - currentPoint.Y);
                m_ScrollTarget.X = m_ScrollStartOffset.X + delta.X;
                m_ScrollTarget.Y = m_ScrollStartOffset.Y + delta.Y;

                // Scroll to the new position.
                this.scrollViewer.ScrollToHorizontalOffset(m_ScrollTarget.X);
                this.scrollViewer.ScrollToVerticalOffset(m_ScrollTarget.Y);
            }

            base.OnPreviewMouseMove(e);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                this.Cursor = Cursors.Arrow;
                this.ReleaseMouseCapture();
            }

            base.OnPreviewMouseUp(e);
        }

        #endregion
    }

    internal class RectRelativeToViewport
    {
        internal HorizontalRelativeToViewPort HorizontalState = HorizontalRelativeToViewPort.Undefined;
        internal VerticalRelativeToViewPort VerticalState = VerticalRelativeToViewPort.Undefined;

        internal enum HorizontalRelativeToViewPort
        {
            Undefined,
            Contained,
            Left,
            Right
        }

        internal enum VerticalRelativeToViewPort
        {
            Undefined,
            Contained,
            Top,
            Bottom
        }
    }


    /// <summary>
    /// in case view model is null, ShowControls should be Hidden 
    /// </summary>
    public class ControlsVisibiltyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultur)
        {
            Visibility result = Visibility.Visible;

            if (value != null)
            {
                result = (Visibility)value;
            }
            else
            {
                result = Visibility.Hidden;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// in case view model is null, Height should be 0, else 25
    /// </summary>
    public class ControlsHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultur)
        {
            double result = 25d;

            if (value != null)
            {
                result = (double)value;
            }
            else
            {
                result = 0d;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

