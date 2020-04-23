using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Sellenium
{
    class GoogleSS
    {


        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "MLM-Hipocrates";
        static readonly string SpreadsheetId = "1aQ4Otyiihcoce-icj6qabIun9ogBPqu-nFO4RYpDReY";
        static readonly string sheet = "HojaPacientes";
        static SheetsService service;
        

        public GoogleSS()
        {
            try
            {
                GoogleCredential credential;
                using (var stream = new FileStream("mlmcovid-8745379e99cd.json", FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream)
                        .CreateScoped(Scopes);
                }

                //Ver luego para el uso con proxy.

                /*
                WebProxy proxy = (WebProxy)WebRequest.DefaultWebProxy;
                if (proxy.Address.AbsoluteUri != string.Empty)
                {              
                    // Create Google Sheets API service.
                    service = new SheetsService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = ApplicationName,
                    });
                }
               else {

                    // Create Google Sheets API service.
                    service = new SheetsService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = ApplicationName,
                        HttpClientFactory = new ProxySupportedHttpClientFactory()

                    });
                } */

                service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public List<Paciente> ReadEntries()
        {
            try
            {
                List<Paciente> lista = new List<Paciente>();
                var range = $"{sheet}!A:M";
                SpreadsheetsResource.ValuesResource.GetRequest request =
                        service.Spreadsheets.Values.Get(SpreadsheetId, range);

                var response = request.Execute();
                IList<IList<object>> values = response.Values;
                if (values != null && values.Count > 0)
                {
                    int i = 0;
                    foreach (var row in values)
                    {
                        i++;
                        if (values[0] == row)
                            continue;
                        Paciente paciente = BindearPaciente(row, i);
                        if (!Object.Equals(null, paciente))
                            lista.Add(paciente);
                    }
                }
                else
                {
                    Console.WriteLine("No data found.");
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Paciente BindearPaciente(IList<object> row, int index)
        {
            try
            {

                const int CANTIDAD_CAMPOS = 5;
                Paciente model = new Paciente();

                String[] campos = new String[CANTIDAD_CAMPOS];

                for (int i = 0; i < CANTIDAD_CAMPOS; i++)
                {
                    campos[i] = (row.Count > i) ? row[i].ToString() : string.Empty;
                }
                model.index = index;
                string campoDone = campos[0].ToString();
                if (campoDone.Length > 0)
                    return null;

                string campoApellido = campos[2].ToString();
                string campoNombre = campos[3].ToString();
                string campoTelefono = campos[4].ToString();
                string campoFecha = campos[1].ToString();


                model.Apellido = campoApellido;
                model.Escaneado = false;
                model.FechaCreacion = DateTime.Parse(campoFecha);
                model.Nombre = campoNombre;

                model.Telefono = campoTelefono;
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string WriteInGoogleSS(List<IList<object>> data, int writtingRange)
        {
            String range = "HojaPacientes!A" + writtingRange.ToString() + ":B";
            string valueInputOption = "USER_ENTERED";

            // The new values to apply to the spreadsheet.
            List<Google.Apis.Sheets.v4.Data.ValueRange> updateData = new List<Google.Apis.Sheets.v4.Data.ValueRange>();
            var dataValueRange = new Google.Apis.Sheets.v4.Data.ValueRange();
            dataValueRange.Range = range;
            dataValueRange.Values = data;
            updateData.Add(dataValueRange);

            Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest requestBody = new Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest();
            requestBody.ValueInputOption = valueInputOption;
            requestBody.Data = updateData;

            var request = service.Spreadsheets.Values.BatchUpdate(requestBody, SpreadsheetId);

            Google.Apis.Sheets.v4.Data.BatchUpdateValuesResponse response = request.Execute();
            // Data.BatchUpdateValuesResponse response = await request.ExecuteAsync(); // For async 

            return JsonConvert.SerializeObject(response);
        }







    }
}
