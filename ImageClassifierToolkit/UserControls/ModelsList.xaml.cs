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
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.UserControls
{
    /// <summary>
    /// Interaction logic for ModelsList.xaml
    /// </summary>
    public partial class ModelsList : UserControl
    {
        public ModelsList()
        {
            InitializeComponent();
        }
        private void SelectModelFolder_Click(object sender, RoutedEventArgs e)
        {
            var runCmd = new TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands.SelectModelsFolderCommand();

            var vm = DataContext as ModelsListViewModel;

            runCmd.Execute(new Action<string>(x => vm.ModelFolder = x));
        }

        //public static readonly DependencyProperty SelectedModelInfoProperty =
        //    DependencyProperty.Register("SelectedModelInfo", typeof(ModelDataInfo),
        //    typeof(ModelsList));

        //public ModelDataInfo SelectedModelInfo
        //{
        //    get
        //    {
        //        return (ModelDataInfo)GetValue(SelectedModelInfoProperty);
        //    }
        //    set
        //    {
        //        SetValue(SelectedModelInfoProperty, value);
        //    }
        //}

        //private void modelsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if ( e.AddedItems.Count > 0 )
        //    {
        //        SelectedModelInfo = e.AddedItems[0] as ModelDataInfo;
        //    }
        //    else
        //    {
        //        SelectedModelInfo = null;
        //    }
        //}
    }
}
