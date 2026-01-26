using System;
using System.Windows;
using System.Windows.Forms.Integration;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Windows.Forms;

namespace ViewModel
{
    public static class CrystalHost
    {
        public static readonly DependencyProperty ReportProperty =
            DependencyProperty.RegisterAttached(
                "Report",
                typeof(ReportDocument),
                typeof(CrystalHost),
                new PropertyMetadata(null, OnReportChanged));

        public static void SetReport(DependencyObject element, ReportDocument value)
            => element.SetValue(ReportProperty, value);

        [AttachedPropertyBrowsableForType(typeof(WindowsFormsHost))]
        public static ReportDocument GetReport(DependencyObject element)
            => (ReportDocument)element.GetValue(ReportProperty);

        private static void OnReportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is WindowsFormsHost host)) return;

            try
            {
                // Crear el viewer si aún no existe
                if (!(host.Child is CrystalReportViewer viewer))
                {
                    viewer = new CrystalReportViewer
                    {
                        Dock = System.Windows.Forms.DockStyle.Fill,
                        ToolPanelView = ToolPanelViewType.None,
                        ShowGroupTreeButton = false,
                        BorderStyle = System.Windows.Forms.BorderStyle.None
                    };
                    host.Child = viewer;
                }

                var report = e.NewValue as ReportDocument;
                viewer.ReportSource = report;

                try
                {
                    // Refresh puede fallar en tiempo de diseño; capturamos para no romper el diseñador
                    viewer.Refresh();
                }
                catch
                {
                    // ignorar errores de refresh
                }
            }
            catch
            {
                // evitar que excepciones rompan el diseñador o la UI
            }
        }
    }
}