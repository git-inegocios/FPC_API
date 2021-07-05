using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace HTTPGetAPP
{
    class Program
    {

        
        private const string connectionString = "Endpoint=sb://fpcstreaming.servicebus.windows.net/;SharedAccessKeyName=logicapp;SharedAccessKey=O1+DoTYDuMbSouCdZ+1CQYo+8zVknX3RRSo+mL8azFQ=;EntityPath=apimaquinas";
        

        static readonly HttpClient client = new HttpClient();


        static async Task Main(string[] args)
        {
            try
            {
                string url = "https://webapi.fpc.cl/papelespowerbi/powerbi/pantalla_maq";     // TOMA JSON DESDE LA API
                HttpResponseMessage response =
                    await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();                 //TRANSFORMA CONTENIDO JSON A STRING

                



                await using (var producerClient = new EventHubProducerClient(connectionString))
                {
                   
                    using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

                    
                    eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(responseBody)));  // carga json a eventhub
                    

                    
                    await producerClient.SendAsync(eventBatch);
                    Console.WriteLine("A batch of events has been published.");
                }

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ups {ex.Message}");
            }


        }
    }

   
}

