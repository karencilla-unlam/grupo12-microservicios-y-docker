using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TelegramBot.Logica.Interfaces;

namespace TelegramBot.Logica.Servicios
{
    public class ServicioClimaHttp : IServicioClima
    {
        private readonly HttpClient _clienteHttp;

        public ServicioClimaHttp()
        {
            _clienteHttp = new HttpClient();
        }

        public async Task<string> ObtenerClimaActualAsync(string ciudad)
        {
            var url = $"http://microservicio-clima/api/clima?ciudad={ciudad}";

            try
            {
                var respuestaTexto = await _clienteHttp.GetStringAsync(url);
                var datosJson = JObject.Parse(respuestaTexto);

                var descripcionClima = datosJson["descripcion"]?.ToString();
                var temperaturaActual = datosJson["temperatura"]?.ToObject<double>();

                return $"El clima en {ciudad} es {descripcionClima}, con una temperatura de {temperaturaActual}°C.";
            }
            catch
            {
                return "No se pudo obtener el clima en este momento.";
            }
        }
    }
}
