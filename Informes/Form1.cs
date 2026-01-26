using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Informes
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
            // Instanciar el repositorio
            var repo = new SocioRepository();

            // Obtener el DataTable tipado
            var sociosTable = repo.ObtenerSocios();

            // Crear el DataSet y agregar la tabla
            var ds = new DsSocios();
            ds.Socios.Merge(sociosTable);

            // Instanciar el reporte y asignar el DataSource
            var reporte = new InformeSocios(); // Asegúrate que el nombre coincida con tu .rpt
            reporte.SetDataSource(ds);

            // Asignar el reporte al visor
            crystalReportViewer1.ReportSource = reporte;
            crystalReportViewer1.Refresh();
        }
    }
}
