
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
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels;
using TiS.Recognition.FieldClassifyService.Models;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Views
{
    /// <summary>
    /// Interaction logic for BaseConfigurationView.xaml
    /// </summary>
    public partial class BaseConfigurationView : UserControl
    {
        public BaseConfigurationView()
        {
            InitializeComponent();

           // switch (FactoryServices.SvmEngineType)
           // {
             //   case SvmEngine.Svm:
                    svmHolder.Children.Add(new AccordConfigurationView());
           // FeatureListHolder.Features.ForEach(a=>FeatureHolder.Items.Add(a));
                    
               //     break;
              //  case SvmEngine.SvmNet:
              //      svmHolder.Children.Add(new SvmNetConfigurationView());
              //      break;
          //  }
            
        }
        private void SelectInputPath_Click(object sender, RoutedEventArgs e)
        {
            
            var configurationViewModel = DataContext as BaseConfigurationViewModel;
          
            var cmd = new TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands.SelectImagesFolderCommand();
            cmd.Execute(new Action<string>(x => configurationViewModel.InputFolder = x));
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
           // var configurationViewModel = DataContext as BaseConfigurationViewModel;

            MessageBox.Show(((sender as FrameworkElement).DataContext as FeatureSelectModel).SerialID.ToString());
        }

        private void OnAll_Click(object sender, RoutedEventArgs e)
        {
            AppDataCenter.Singleton.UpdateFeatures( x=> x.IsSelected = true ); 
        }

        private void OffAll_Click(object sender, RoutedEventArgs e)
        {
            AppDataCenter.Singleton.UpdateFeatures(x => x.IsSelected = false);
        }
    }
}
