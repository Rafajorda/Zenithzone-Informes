using System.Net.Mail;
using Models;
using ViewModel;
namespace ZenithZone.Test
{
    [TestClass]
    public sealed class TestsSocios
    {

        /// <summary>
        ///     Test para verificar que el formato de email es válido.
        /// </summary>
        [TestMethod]
        public void EmailFormato_Valido()
        {
            var socio = new Socio { Email = "user@user.com" };
            var viewModel = new SociosViewModel();
            Assert.IsTrue(viewModel.EsEmailValido(socio.Email));
        }
        /// <summary>
        ///   Test para verificar que el formato de email es inválido.
        ///   </summary> 

        [TestMethod]
        public void EmailFormato_Invalido()
        {
            var socio = new Socio { Email = "user.com" };
            var viewModel = new SociosViewModel();
            Assert.IsFalse(viewModel.EsEmailValido(socio.Email));
        }



    }
}

