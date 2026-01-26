using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Windows;

namespace Views
{
    /// <summary>
    /// Ventana que muestra informes Crystal Reports.
    /// La ventana se reutiliza y oculta en lugar de cerrarse para evitar problemas
    /// con el control nativo de SAP que mantiene recursos en memoria.
    /// </summary>

    public partial class ReportWindow : Window
    {
        /// <summary>
        /// Constructor por defecto; inicializa componentes XAML.
        /// </summary>
        public ReportWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Muestra un informe utilizando una fábrica que crea un ReportDocument.
        /// Se asegura crear un ReportDocument nuevo cada vez para evitar compartir instancias.
        /// </summary>
        /// <param name="reportFactory">Fábrica que crea y devuelve el ReportDocument.</param>
        public void ShowReport(Func<ReportDocument> reportFactory)
        {
            // Crear SIEMPRE un reporte nuevo
            var report = reportFactory();

            // Asignar como ReportSource del viewer
            ReportViewer.ViewerCore.ReportSource = report;

            // Mostrar la ventana si no está visible y activarla
            if (!IsVisible)
                Show();

            Activate();
        }

        /// <summary>
        /// Intercepta el cierre de la ventana y lo transforma en ocultación para
        /// mantener la instancia viva y permitir liberar recursos manualmente.
        /// </summary>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Cancelar el cierre y ocultar para reutilizar la ventana
            e.Cancel = true;
            Hide();
        }
    }
}
