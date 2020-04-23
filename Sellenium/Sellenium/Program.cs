using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sellenium
{
    class Program
    {
        static void Main(string[] args)
        {
            bool wasapIniciado = false;

            List<Paciente> lista = new List<Paciente>();
            GoogleSS google = new GoogleSS();
            lista = google.ReadEntries();


            try
            {
                IWebDriver driver = new ChromeDriver(@"C:\Users\BishopPC\source\repos\Sellenium\packages\Selenium.WebDriver.ChromeDriver.81.0.4044.6900\driver\win32");
                while (!wasapIniciado)
                {
                    Console.WriteLine("Por favor , iniciar una sesion de wasap para comenzar a operar");
                    Thread.Sleep(1000);
                    Console.WriteLine("Continuar? y/n ");
                    string lectura = Console.ReadLine();
                    if (lectura.Trim().ToUpper() == "YES" || lectura.Trim().ToUpper() == "Y")
                        wasapIniciado = true;
                    if (lectura.Trim().ToUpper() == "EXIT" || lectura.Trim().ToUpper() == "SALIR")
                    {
                        Console.WriteLine("Finalizando Programa");
                        Thread.Sleep(5000);
                        Console.WriteLine("...");
                        driver.Close();
                        Thread.Sleep(5000);
                        return;
                    }
                }

                System.Console.Clear();
                foreach (Paciente paciente in lista)
                {
                    int j = 1;
                    try
                    {

                        System.Console.WriteLine("Paciente " + j.ToString() + "de " + lista.Count.ToString());
                        string texto = string.Empty;

                        string numTelefono = string.Empty;
                        if (String.IsNullOrEmpty(paciente.Telefono))
                        {
                            EscribirEnGoogle(google, paciente.index, "Error datos");
                            continue;
                        }

                        string telefonoSinFormato = paciente.Telefono.Trim();

                        if (telefonoSinFormato.Contains("+"))
                        {
                            telefonoSinFormato = telefonoSinFormato.Substring(telefonoSinFormato.LastIndexOf("+"));
                        }

                        int numeroTelefonico;
                        if (!int.TryParse(telefonoSinFormato, out numeroTelefonico))
                        {
                            EscribirEnGoogle(google, paciente.index, "Error datos");
                            continue;
                        }
                        if (telefonoSinFormato.Length != 10)
                        {
                            EscribirEnGoogle(google, paciente.index, "Error datos");
                            continue;
                        }

                        var arrayTelefonico = telefonoSinFormato.ToCharArray();
                        if (arrayTelefonico[0] != '1')
                        {
                            EscribirEnGoogle(google, paciente.index, "Error datos");
                            continue;
                        }

                        if (arrayTelefonico[1] != '1' && arrayTelefonico[1] != '5')
                        {
                            EscribirEnGoogle(google, paciente.index, "Error datos");
                            continue;
                        }

                        if (arrayTelefonico[1] == '5')
                        {
                            arrayTelefonico[1] = '1';
                        }

                        string stringchar = new string(arrayTelefonico);

                        numTelefono = "54" + stringchar;
                        string url = string.Empty;
                        texto = "Hola, " + paciente.Nombre + " "
                            + " .Nos estamos Comunicando de la Secretaria de Salud de la Municipalidad de La Matanza." + System.Environment.NewLine
                            + " Hemos desarrollado una herramienta totalmente gratuita para nuestros pacientes ; " +
                            "donde podra realizar una breve autoevaluacion de su estado de salud y poder brindarle asistencia en el caso de que sea necesario." +
                            "Le aconsejamos que ingrese al portal una vez por día o las veces que usted crea necesario." +
                            "Para ingresar al autodiagnóstico te pedimos que ingreses en el siguiente link " + @"https://bit.ly/Pacientemlm" +
                            " .Para que se habilite el link debe registrar este número en sus contactos.Desde ya muchas gracias por su colaboración.Equipo de Salud";



                        url = "https://web.whatsapp.com/send?phone=" + numTelefono + "&text=" + texto + "&source&data&app_absent";

                        driver.Navigate().GoToUrl(url);
                        Thread.Sleep(10000);
                        var elementos = driver.FindElements(By.ClassName("copyable-area"));
                        var elemento = elementos.Last();
                        var button = elemento.FindElements(By.TagName("button")).Last();
                        string a = button.GetProperty("class");
                        button.Click();

                        Thread.Sleep(10000);
                        EscribirEnGoogle(google, paciente.index, "Enviado");
                    }
                    catch (Exception ex)
                    {
                        EscribirEnGoogle(google, paciente.index, "Sucedio un error." + ex.Message);
                        continue;
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private static void EscribirEnGoogle(GoogleSS google, int indice, string respuesta)
        {
            try
            {
                List<IList<object>> matrix = new List<IList<object>>();
                IList<object> fila = new List<object>();
                fila.Add(respuesta);
                matrix.Add(fila);
                google.WriteInGoogleSS(matrix, indice);
                return;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
