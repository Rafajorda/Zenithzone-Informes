using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Windows;
using System.Windows.Controls;
using ViewModel;

namespace Views
{
    /// <summary>
    /// UserControl que expone la interfaz para generar informes.
    /// Suscribe/Desuscribe el evento ReportRequested del ViewModel para
    /// crear y mostrar la ventana de informe cuando se solicite.
    /// </summary>
    public partial class UCInformes : UserControl
    {
        /// <summary>
        /// Inicializa el UserControl y registra el manejador para cambios en el DataContext.
        /// </summary>
        public UCInformes()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        /// <summary>
        /// Cuando cambia el DataContext suscribe o desuscribe el evento
        /// ReportRequested para recibir peticiones de generación de reportes.
        /// </summary>
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is InformesViewModel oldVm)
                oldVm.ReportRequested -= OnReportRequested;

            if (e.NewValue is InformesViewModel newVm)
                newVm.ReportRequested += OnReportRequested;
        }

        /// <summary>
        /// Manejador que se ejecuta cuando el ViewModel solicita mostrar un informe.
        /// Se reutiliza una única instancia de ReportWindow en la aplicación y
        /// se delega la creación del ReportDocument a la fábrica proporcionada.
        /// </summary>
        private void OnReportRequested(Func<ReportDocument> reportFactory)
        {
            if (App.ReportWindowInstance == null)
            {
                App.ReportWindowInstance = new ReportWindow();
                App.ReportWindowInstance.Owner = Application.Current.MainWindow;
            }

            App.ReportWindowInstance.ShowReport(reportFactory);
        }

    }
}