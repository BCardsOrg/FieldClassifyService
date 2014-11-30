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
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Views
{
	/// <summary>
	/// Interaction logic for ReportView.xaml
	/// </summary>
	public partial class ReportView : UserControl
	{
		public ReportView()
		{
			InitializeComponent();
		}

        private void SaveModel_Click(object sender, RoutedEventArgs e)
        {
            var cmd = new SaveModelDataCommand();

            cmd.Execute(null);
        }

        private void Best_Click(object sender, RoutedEventArgs e)
        {
            var cmd = new SelectBestReportDataCommand();

            cmd.Execute(null);
        }

    }
}
