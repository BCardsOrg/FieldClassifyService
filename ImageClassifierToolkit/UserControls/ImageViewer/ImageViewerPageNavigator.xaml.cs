using System;
using System.Collections.Generic;
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
    /// Interaction logic for ImageViewerPagingControl.xaml
    /// </summary>
    public partial class ImageViewerPageNavigator : UserControl
    {        
        public delegate void PageChangedEvent(int pageIndex);
        public event PageChangedEvent PageChanged;
               
        private ImageViewerViewModel m_ViewModel = null;      
   
        public ImageViewerPageNavigator()
        {
            DataContextChanged += new DependencyPropertyChangedEventHandler(ImageViewerPagingControl_DataContextChanged);
            InitializeComponent();
        }

        public ImageViewerViewModel ViewModel
        {
            get { return m_ViewModel; }
            set { m_ViewModel = value; }
        }

        private void ImageViewerPagingControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is ImageViewerViewModel)
            {
                m_ViewModel = this.DataContext as ImageViewerViewModel;                                        
            }
        }

        private void buttonFirstPage_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(m_ViewModel.FirstPageIndex);
        }

        private void buttonLastPage_Click(object sender, RoutedEventArgs e)
        {                        
            if (m_ViewModel != null)
            {               
                ChangePage(m_ViewModel.LastPageIndex);
            }
        }

        private void buttonNextPage_Click(object sender, RoutedEventArgs e)
        {     
            ChangePage(m_ViewModel.PageIndex + 1);        
        }

        private void buttonPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(m_ViewModel.PageIndex - 1);
        }

        private void OnChangePage(int pageIndex)
        {
            if (this.PageChanged != null)
            {
                this.PageChanged(pageIndex);
            }
        }

        private void ChangePage(int pageIndex)
        {
            if (m_ViewModel != null)
            {
                if (pageIndex > m_ViewModel.LastPageIndex || pageIndex < m_ViewModel.FirstPageIndex)
                {
                    return;
                }
                                
                OnChangePage(pageIndex);                            
            }             
        }        
       
    }

    /// <summary>
    /// This converter is used for the display of the current page number in the form, which is zero based, and should be displayed as 1-based.
    /// </summary>
    public class AddOneValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultur)
        {
            object result = value;            

            if (value != null)
            {
                result = (int)value + 1;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }                
    }
}
