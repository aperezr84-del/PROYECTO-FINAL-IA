
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class Program
{
    private const string ApiUrl = "https://api.groq.com/openai/v1/chat/completions";
    private const string Modelo = "llama-3.3-70b-versatile";

    private static readonly string ApiKey =
        Environment.GetEnvironmentVariable("GROQ_API_KEY") ?? "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";

    public static async Task Main(string[] args)
    {
        Console.Title = "Asistente Gorge";

        Console.WriteLine("ASISTENTE GORGE");
        Console.WriteLine("Hola, soy Gorge.");
        Console.WriteLine("Escribe 'salir' para finalizar.\n");

        using HttpClient cliente = ConfigurarCliente();

        string mensaje;

        do
        {
            Console.Write("Tu > ");
            mensaje = Console.ReadLine() ?? "";

            if (!mensaje.Equals("salir", StringComparison.OrdinalIgnoreCase))
            {
                string respuesta = await ConsultarIA(cliente, mensaje);

                Console.WriteLine();
                Console.WriteLine($"Gorge > {respuesta}");
                Console.WriteLine();
            }

        } while (!mensaje.Equals("salir", StringComparison.OrdinalIgnoreCase));
    }

    private static HttpClient ConfigurarCliente()
    {
        HttpClient cliente = new HttpClient();

        cliente.DefaultRequestHeaders.Add(
            "Authorization",
            $"Bearer {ApiKey}");

        return cliente;
    }

    private static async Task<string> ConsultarIA(HttpClient cliente, string pregunta)
    {
        string promptSistema =
            "Tu nombre es Gorge. " +
            "Eres un asistente experto en programación C#. " +
            "Si te preguntan tu nombre responde que te llamas Gorge. " +
            "Responde siempre en español.";

        var datos = new
        {
            model = Modelo,
            messages = new object[]
            {
                new
                {
                    role = "system",
                    content = promptSistema
                },
                new
                {
                    role = "user",
                    content = pregunta
                }
            }
        };

        string contenidoJson = JsonSerializer.Serialize(datos);

        HttpResponseMessage respuesta = await cliente.PostAsync(
            ApiUrl,
            new StringContent(
                contenidoJson,
                Encoding.UTF8,
                "application/json"));

        string cuerpo = await respuesta.Content.ReadAsStringAsync();

        JsonDocument json = JsonDocument.Parse(cuerpo);

        return json.RootElement
                   .GetProperty("choices")[0]
                   .GetProperty("message")
                   .GetProperty("content")
                   .GetString() ?? "No se recibió respuesta.";
    }
}