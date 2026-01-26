using CrystalDecisions.CrystalReports.Engine;
using Informes;
using Models;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace ViewModel
{
    /// <summary>
    /// ViewModel responsable de la generación de informes.
    /// Exponen comandos para solicitar distintos informes y un evento
    /// que proporciona una fábrica (Func) para crear el ReportDocument cuando sea necesario.
    /// </summary>
    public class InformesViewModel : BaseViewModel
    {
        //private ReportDocument _reporteActual;
        /// <summary>
        /// Evento que solicita al consumidor la creación y visualización de un informe.
        /// Se usa una función (factory) para crear el ReportDocument en el hilo/ventana de UI.
        /// </summary>
        public event Action<Func<ReportDocument>> ReportRequested;
        private string _tituloInforme;
        private int _selectedActividadId;
        private ObservableCollection<Actividad> _actividades;
      

        public ICommand GenerarInformeSociosCommand { get; }
        public ICommand GenerarInformeActividadesCommand { get; }
        public ICommand GenerarInformeReservasCommand { get; }

        /// <summary>
        /// Título que se muestra en la UI para indicar el informe activo.
        /// </summary>
        public string TituloInforme
        {
            get => _tituloInforme;
            set
            {
                _tituloInforme = value;
                OnPropertyChanged(nameof(TituloInforme));
            }
        }

        /// <summary>
        /// Colección de actividades disponibles para filtrar informes.
        /// </summary>
        public ObservableCollection<Actividad> Actividades
        {
            get => _actividades;
            set
            {
                _actividades = value;
                OnPropertyChanged(nameof(Actividades));
            }
        }

        // SelectedValuePath bind (Id)
        public int SelectedActividadId
        {
            get => _selectedActividadId;
            set
            {
                if (_selectedActividadId != value)
                {
                    _selectedActividadId = value;
                    OnPropertyChanged(nameof(SelectedActividadId));
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Constructor: inicializa comandos y carga lista de actividades.
        /// </summary>
        public InformesViewModel()
        {
            TituloInforme = "Selecciona un informe para visualizar";

            GenerarInformeSociosCommand = new RelayCommand(GenerarInformeSocios);
            GenerarInformeActividadesCommand = new RelayCommand(GenerarInformeActividades);
            GenerarInformeReservasCommand = new RelayCommand(GenerateInformeReservas);

            CargarActividades();
        }

        /// <summary>
        /// Carga actividades desde Entity Framework y pone el primer elemento seleccionado por defecto.
        /// Manejo básico de errores mostrando un MessageBox.
        /// </summary>
        private void CargarActividades()
        {
            try
            {
                using (var ctx = new zenithzoneEntities())
                {
                    var list = ctx.Actividad
                                  .OrderBy(a => a.Nombre)
                                  .ToList();

                    Actividades = new ObservableCollection<Actividad>(list);

                    if (Actividades.Any())
                    {
                        SelectedActividadId = Actividades.First().Id;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"No se pudieron cargar actividades: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                Actividades = new ObservableCollection<Actividad>();
            }
        }

        /// <summary>
        /// Crea el ReportDocument para el informe de socios. Lanza excepciones si no hay datos.
        /// </summary>
        private ReportDocument CrearInformeSocios()
        {
            var repo = new Informes.SocioRepository();
            var dt = repo.ObtenerSocios() as DataTable
                ?? throw new InvalidOperationException("No se devolvió DataTable.");

            var rpt = new InformeSocios();
            rpt.SetDataSource(dt);
            rpt.SetParameterValue("TituloInforme", "Informe de Socios");

            return rpt;
        }

        /// <summary>
        /// Solicita la creación del informe de socios mediante el evento ReportRequested.
        /// El ReportDocument se crea cuando el consumidor invoque la fábrica.
        /// </summary>
        private void GenerarInformeSocios()
        {
            try
            {
                // Pasamos una fábrica en lugar de crear el ReportDocument aquí.
                ReportRequested?.Invoke(() => CrearInformeSocios());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Crea el ReportDocument para un informe de actividades filtrado por SelectedActividadId.
        /// </summary>
        private ReportDocument CrearInformeActividades()
        {
            if (SelectedActividadId == 0)
                throw new InvalidOperationException("Seleccione una actividad.");

            var repo = new Informes.ActividadRepository();
            var dt = repo.ObtenerActividades(SelectedActividadId) as DataTable
                ?? throw new InvalidOperationException("No se devolvió DataTable o no hay datos para la actividad seleccionada.");

            var rpt = new InformeActividades();
            rpt.SetDataSource(dt);

            var actividad = Actividades?.FirstOrDefault(a => a.Id == SelectedActividadId);
            var titulo = actividad != null ? $"Reservas - {actividad.Nombre}" : "Informe de Actividad";
            rpt.SetParameterValue("TituloInforme", titulo);
            TituloInforme = titulo;

            return rpt;
        }

        /// <summary>
        /// Crea el ReportDocument para el informe de reservas.
        /// </summary>
        private ReportDocument CrearInformeReservas()
        {
            var repo = new Informes.ReservaRepository();
            var dt = repo.ObtenerReservas() as DataTable
                ?? throw new InvalidOperationException("No se devolvió DataTable.");

            var rpt = new InformeReservas();
            rpt.SetDataSource(dt);
            rpt.SetParameterValue("TituloInforme", "Informe de Reservas");
            TituloInforme = "Informe de Reservas";

            return rpt;
        }

        /// <summary>
        /// Solicita la generación y visualización del informe de actividades mediante evento.
        /// </summary>
        private void GenerarInformeActividades()
        {
            try
            {
                ReportRequested?.Invoke(() => CrearInformeActividades());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al generar informe de actividades:\n\n{ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Solicita la generación y visualización del informe de reservas mediante evento.
        /// </summary>
        private void GenerateInformeReservas()
        {
            try
            {
                ReportRequested?.Invoke(() => CrearInformeReservas());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Error al generar el informe de reservas:\n\n{ex.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        //private void SetAllParameters(ReportDocument rpt, string titulo)
        //{
        //    foreach (CrystalDecisions.Shared.ParameterField param in rpt.ParameterFields)
        //    {
        //        if (param.Name == "TituloInforme")
        //        {
        //            rpt.SetParameterValue(param.Name, titulo);
        //        }
        //        else
        //        {
        //            // Valor por defecto seguro
        //            rpt.SetParameterValue(param.Name, "");
        //        }
        //    }
        //}
    }
}
