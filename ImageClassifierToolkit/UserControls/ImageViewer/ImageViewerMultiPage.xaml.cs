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
    /// Interaction logic for ImageViewerMultiPage.xaml
    /// </summary>
    public partial class ImageViewerMultiPage : UserControl
    {       

        public ImageViewerMultiPage()
        {
            InitializeComponent();
            ImageViewerPageNavigator imageViewerPageNavigator = new ImageViewerPageNavigator();
            imageViewerPageNavigator.PageChanged += imageViewerPageNavigator_PageChanged;
            this.ImageViewer.PlaceHolderDockPanel.Children.Add(imageViewerPageNavigator);            
        }

        private void imageViewerPageNavigator_PageChanged(int pageIndex)
        {

            if (this.DataContext is ImageViewerMultiPageViewModel)
            {
                var vm = this.DataContext as ImageViewerMultiPageViewModel;
                vm.SwitchToPage(pageIndex);                
            }
        }        
       
    }
}
