using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using CEK.CSharp;
using CEK.CSharp.Models;
using KatsuzetsuApp.Settings;

namespace KatsuzetsuApp
{
    public static class ClovaEndpoint
    {
        [FunctionName("clova")]
        public static async Task<IActionResult> Clova(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var clovaClient = new ClovaClient();
            var request = await clovaClient.GetRequest(req.Headers["SignatureCEK"], req.Body);

            var response = await HandleRequestAsync(request);

            return new OkObjectResult(response);
        }

        private static async Task<CEKResponse> HandleRequestAsync(CEKRequest request)
        {
            var response = new CEKResponse();
            switch (request.Request.Type)
            {
                case RequestType.LaunchRequest:
                    await HandleLaunchRequestAsync(request, response);
                    break;

                case RequestType.IntentRequest:
                    await HandleIntentRequestAsync(request, response);
                    break;

                case RequestType.SessionEndedRequest:
                    await HandleSessionEndedRequestAsync(request, response);
                    break;

                default:
                    break;
            }
            return response;
        }
        private static Task HandleIntentRequestAsync(CEKRequest request, CEKResponse response)
        {
            void fillSlotIfExist(string slotName, ref string word)
            {
                if (request.Request.Intent.Slots?.ContainsKey(slotName) ?? false)
                {
                    word = request.Request.Intent.Slots[slotName].Value;
                }
            }

            switch (request.Request.Intent.Name)
            {
                case IntentNames.DefaultIntent:
                {
                    string word = null;
                    fillSlotIfExist(SlotNames.NamamugiNamagomeNamatamago, ref word);
                    fillSlotIfExist(SlotNames.SyusyaSentaku, ref word);

                    if (!string.IsNullOrEmpty(word))
                    {
                        response.ShouldEndSession = true;
                        response.AddText(Messages.GenerateCongratsMessage(word));
                    }
                    else
                    {
                        response.ShouldEndSession = false;
                        response.AddText(Messages.WrongPronunciationMessage);
                    }

                    break;
                }
                default:
                    // noop
                    break;
            }

            return Task.CompletedTask;
        }

        private static Task HandleSessionEndedRequestAsync(CEKRequest request, CEKResponse response)
        {
            response.AddText(Messages.GoodbayMessage);
            return Task.CompletedTask;
        }

        private static Task HandleLaunchRequestAsync(CEKRequest request, CEKResponse response)
        {
            response.ShouldEndSession = false;
            response.AddText(Messages.WelcomeMessage);
            return Task.CompletedTask;
        }
    }
}
