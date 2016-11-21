using System;
using System.Collections.Generic;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.VoiceCommands;
using SimplyPasswordWin10Shared.Business;
using SimplyPasswordWin10Shared.Context;

namespace PasswordService
{
    /// <summary>
    /// Tache en arrière plan pour gérer l'appli par des commandes vocales
    /// </summary>
    public sealed class PasswordService : IBackgroundTask
    {

        private readonly ResourceLoader _resourceLoader = new ResourceLoader();

        BackgroundTaskDeferral _serviceDeferral;

        private VoiceCommandServiceConnection _voiceCommandServiceConnection;

        /// <summary>
        /// Méthode se lancant lors de l'appel de cortana pour l'appli
        /// </summary>
        /// <param name="taskInstance"></param>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //lancement de cortana, récupération des objets nécéssaires
            _serviceDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += (sender, reason) => _serviceDeferral?.Complete();
            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            _voiceCommandServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
            _voiceCommandServiceConnection.VoiceCommandCompleted += (sender, args) => _serviceDeferral?.Complete();
            var identifiant = (await _voiceCommandServiceConnection.GetVoiceCommandAsync()).Properties["site"][0].ToLower();
            string message;
            var openApp = false;
            var openTiles = false;
            var tiles = new List<VoiceCommandContentTile>();
           
            //Initialisation de l'appli
            await ContexteAppli.Initialize(false,null);

            //si le fichier existe
            if(await PasswordBusiness.DoesFileCypherExist())
            {
                //si cortana est autorisé
                if (ContexteAppli.IsCortanaActive)
                {
                    //récupération du mot de passe
                    var mdp = await CortanaBusiness.GetPasswordUser();
                    if (!string.IsNullOrWhiteSpace(mdp))
                    {
                        //chargement du fichier
                        if (await PasswordBusiness.Load(mdp,false))
                        {
                            tiles = CortanaBusiness.GetMotDePasseTile(identifiant,ContexteAppli.DossierMere);

                            if (tiles.Count == 0)
                            {
                                message = GetString("cortanaAucunIdentifiant");
                            }
                            else if (tiles.Count > 10)
                            {
                                message = GetString("cortanaPlusieursResultats");
                                openApp = true;
                            }
                            else
                            {
                                message = GetString("cortanaIdentifiantsTrouves");
                                openTiles = true;
                            }
                        }
                        else
                        {
                            message = GetString("cortanaEchecFichier");
                            openApp = true;
                        }
                    }
                    else
                    {
                        message = GetString("cortanaEchecFichier");
                        openApp = true;
                    }
                }
                else
                {
                    message = GetString("cortanaLanceApp");
                    openApp = true;
                }
            }
            else
            {
                message = GetString("cortanaAucunIdentifiant");
            }

            var userPrompt = new VoiceCommandUserMessage
            {
                DisplayMessage = message,
                SpokenMessage = message
            };

            if (openApp)
            {
                var response = VoiceCommandResponse.CreateResponse(userPrompt);
                response.AppLaunchArgument = identifiant;
                await _voiceCommandServiceConnection.RequestAppLaunchAsync(response);
            }
            else if (openTiles)
            {
                var response = VoiceCommandResponse.CreateResponse(userPrompt, tiles);
                await _voiceCommandServiceConnection.ReportSuccessAsync(response);
            }
            else
            {
                var response = VoiceCommandResponse.CreateResponse(userPrompt);
                await _voiceCommandServiceConnection.ReportSuccessAsync(response);
            }
        }


        /* Au cas ou ca ne marche plus
           var fileParam = await ApplicationData.Current.LocalFolder.GetFileAsync("param");
           var fileHash = await ApplicationData.Current.LocalFolder.GetFileAsync("hash");
           var filePwd = await ApplicationData.Current.RoamingFolder.GetFileAsync(ContexteStatic.NomFichierPassword+"."+ContexteStatic.Extension);

           if (await ApplicationData.Current.RoamingFolder.TryGetItemAsync(ContexteStatic.NomFichierPassword + "." + ContexteStatic.Extension) != null)
           {
               if (await ApplicationData.Current.LocalFolder.TryGetItemAsync("param") != null)
               {
                   if ((await FileIO.ReadTextAsync(fileParam)).Contains("<cortana>true</cortana>") &&
                       await ApplicationData.Current.LocalFolder.TryGetItemAsync("hash") != null)
                   {
                       var mdpChiffre = await FileIO.ReadTextAsync(fileHash);
                       var mdp = CortanaBusiness.DechiffrementStringSurchiffre(mdpChiffre);

                       if (!string.IsNullOrWhiteSpace(mdp))
                       {
                           bool ok = false;
                           Dossier dossierMere;
                           try
                           {
                               var inFile = (await FileIO.ReadBufferAsync(filePwd)).ToArray();

                               //dechiffrement
                               var xmlIn = CryptUtils.AesDecryptByteArrayToString(inFile, mdp, mdp);

                               //deserialize
                               var xsb = new XmlSerializer(typeof(Dossier));
                               var rd = new StringReader(xmlIn);
                               dossierMere = xsb.Deserialize(rd) as Dossier;
                               ok = true;
                           }
                           catch (Exception)
                           {
                               ok = false;
                               throw;
                           }

                           if (ok)
                           {
                               tiles = CortanaBusiness.GetMotDePasseTile(identifiant,dossierMere);

                               if (tiles.Count == 0)
                               {
                                   message = GetString("cortanaAucunIdentifiant");
                               }
                               else if (tiles.Count > 10)
                               {
                                   message = GetString("cortanaPlusieursResultats");
                                   openApp = true;
                               }
                               else
                               {
                                   message = GetString("cortanaIdentifiantsTrouves");
                                   openTiles = true;
                               }
                           }
                           else
                           {
                               message = GetString("cortanaEchecFichier");
                           }
                       }
                       else
                       {
                           message = GetString("cortanaEchecFichier");
                       }
                   }
                   else
                   {
                       openApp = true;
                       message = GetString("cortanaLanceApp");
                   }
               }
               else
               {
                   openApp = true;
                   message = GetString("cortanaLanceApp");
               }
           }
           else
           {
               message = GetString("cortanaAucunIdentifiant");
           }

           var userPrompt = new VoiceCommandUserMessage
           {
               DisplayMessage = message,
               SpokenMessage = message
           };

           if (openApp)
           {
               var response = VoiceCommandResponse.CreateResponse(userPrompt);
               response.AppLaunchArgument = identifiant;
               await _voiceCommandServiceConnection.RequestAppLaunchAsync(response);
           }
           else if (openTiles)
           {
               var response = VoiceCommandResponse.CreateResponse(userPrompt, tiles);
               await _voiceCommandServiceConnection.ReportSuccessAsync(response);
           }
           else
           {
               var response = VoiceCommandResponse.CreateResponse(userPrompt);
               await _voiceCommandServiceConnection.ReportSuccessAsync(response);
           }
           */


        private string GetString(string key)
        {
            return _resourceLoader.GetString(key);
        }
    }
}
