using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using SimplyPasswordWin10.Abstract;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Model;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// Controleur pour l'import et l'export des données
    /// </summary>
    public partial class ImportExportViewModel : AbstractViewModel
    {
        //défini des informations pour la lecture et l'écriture de CSV
        private const string CsvCaractereSeparateur = ",";
        private const string CsvCaractereFinChaine = "\"";
        private const string CsvRetourLigne = "\r\n";
        

        /// <summary>
        /// Constructeur
        /// </summary>
        public ImportExportViewModel()
        {
            ListeFormat = new ObservableCollection<ExportFormat>(new List<ExportFormat> { new ExportFormat(1, "CSV", "csv"), new ExportFormat(2, "XML", "xml"), new ExportFormat(3, "KWI", "kwi") });
            EcraserDossier = false;
        }


        #region méthode générales
        /// <summary>
        /// Vérifie les informations entrés
        /// </summary>
        /// <returns>la liste des erreurs à afficher</returns>
        private string Validate()
        {
            var retour = "";

            if (SelectedDossier == null)
            {
                SelectedDossier = ContexteAppli.DossierMere;
            }

            if(Fichier == null)
            {
                retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurAucunFichier")+"\r\n" ;
            }

            if (FormatChoisi.Id <= 0)
            {
                retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurAucunFormat") + "\r\n";
            }

            if(FormatChoisi.Id.Equals(3))
            {
                if(string.IsNullOrWhiteSpace(MdpCypher))
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurMDPVide") + "\r\n";
                }
                
                if(MdpCypher.Length < 8)
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurNb8Caracteres") + "\r\n";
                }
            }
            return retour;
        }



        /// <summary>
        /// Retourne la liste des mots de passe d'un dossier et de ses sous dossiers
        /// </summary>
        /// <param name="dossier">le dossier dont ont souhaite les mots de passe</param>
        /// <returns>la liste des mots de passe</returns>
        private IEnumerable<MotDePasse> getAllMotDePasse(Dossier dossier)
        {
            var listeMot = new List<MotDePasse>();
            if (dossier.SousDossier != null && dossier.SousDossier.Count > 0)
            {
                foreach (var dos in dossier.SousDossier)
                {
                    var listeFinal = getAllMotDePasse(dos);
                    listeMot.AddRange(listeFinal);
                }
            }
            listeMot.AddRange(dossier.ListeMotDePasse);
            return listeMot;
        }

        /// <summary>
        /// Méthode récursive permettant de placer dans les objets correspondant les objets possesseur de leurs enfants
        /// les dossier parents de sous dossiers et les dossier possesseur des mots de passe)
        /// </summary>
        /// <param name="ori">le dossier à scanner</param>
        private static void RemiseEnPlaceParent(Dossier ori)
        {
            foreach (var m in ori.ListeMotDePasse)
            {
                m.DossierPossesseur = ori;
            }

            foreach (var d in ori.SousDossier)
            {
                d.DossierParent = ori;
                RemiseEnPlaceParent(d);
            }
        }

        /// <summary>
        /// Place les icones dans chaque mot de passes
        /// </summary>
        /// <param name="ori">le dossier à scanner (récursif)</param>
        private static void RemiseEnPlaceIcone(Dossier ori)
        {
            foreach (var m in ori.ListeMotDePasse)
            {
                var res = ContexteAppli.ListeIcone.Where(x => (int)x.Tag == m.IdIcone).ToList();
                if (res.Count > 0)
                {
                    m.Icone = (BitmapImage)res[0].Source;
                }
                else
                {
                    m.Icone = null;
                }

            }

            foreach (var d in ori.SousDossier)
            {
                RemiseEnPlaceIcone(d);
            }
        }
        #endregion

        
        #region gestion CSV
        /// <summary>
        /// Récupère les données d'un fichier CSV
        /// </summary>
        /// <returns>La liste des erreurs</returns>
        public async Task<string> ImporterCsv()
        {
            var retour = Validate();
            if (string.IsNullOrWhiteSpace(retour))
            {
                //ComFile fichierImport = new ComFile(fichier);
                var fichierImport = await FileIO.ReadTextAsync(Fichier, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                var listeMotDePasse = StringUtils.Split(fichierImport, CsvRetourLigne);

                foreach (var data in listeMotDePasse)
                {
                    var dataTmp = data;
                    if (!string.IsNullOrWhiteSpace(dataTmp))
                    {
                        dataTmp = dataTmp.Remove(0, 1);
                        dataTmp = dataTmp.Remove(dataTmp.Length - 1, 1);
                        var listeData = StringUtils.Split(dataTmp, CsvCaractereFinChaine + CsvCaractereSeparateur + CsvCaractereFinChaine);
                        var motTmp = new MotDePasse(listeData[0], listeData[1], listeData[2], SelectedDossier, listeData[3], 0,(listeData.Count <= 4)?"":listeData[4]);
                        SelectedDossier.ListeMotDePasse.Add(motTmp);
                    }

                }
                RemiseEnPlaceIcone(SelectedDossier);
            }
            return retour;
        }

        /// <summary>
        /// exporte en format CSV les mots de passe d'un dossier et de ses sous dossiers
        /// </summary>
        /// <returns>Retourne la liste des erreurs</returns>
        public async Task<string> ExporterCsv()
        {
            var retour = Validate();
            
            if (string.IsNullOrWhiteSpace(retour))
            {
                if (!StringUtils.GetExtension(Fichier.Name).Equals(FormatChoisi.Format))
                {
                   await Fichier.RenameAsync(Fichier.Name +"." + FormatChoisi.Format);
                }

                var chaine = new StringBuilder();

                var listeMotDePasse = getAllMotDePasse(SelectedDossier);

                foreach (var mot in listeMotDePasse)
                {
                    chaine.Append(CsvCaractereFinChaine + mot.Titre + CsvCaractereFinChaine + CsvCaractereSeparateur);
                    chaine.Append(CsvCaractereFinChaine + mot.Login + CsvCaractereFinChaine + CsvCaractereSeparateur);
                    chaine.Append(CsvCaractereFinChaine + mot.MotDePasseObjet + CsvCaractereFinChaine + CsvCaractereSeparateur);
                    chaine.Append(CsvCaractereFinChaine + mot.Commentaire + CsvCaractereFinChaine + CsvCaractereSeparateur);
                    chaine.Append(CsvCaractereFinChaine + mot.SiteWeb + CsvCaractereFinChaine);
                    chaine.Append(CsvRetourLigne);
                }

                await FileIO.WriteTextAsync(Fichier, chaine.ToString(), Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }
            return retour;
        }
        #endregion



        #region gestion XML
        /// <summary>
        /// Exporte en format xml le dossier avec le système de sérialization
        /// </summary>
        /// <returns>la liste des éventuelles erreurs</returns>
        public async Task<string> ExporterXml()
        {
            var retour = Validate();
            if (string.IsNullOrWhiteSpace(retour))
            {
                if (!StringUtils.GetExtension(Fichier.Name).Equals(FormatChoisi.Format))
                {
                    await Fichier.RenameAsync(Fichier.Name + "." + FormatChoisi.Format);
                }

                var xs = new XmlSerializer(typeof(Dossier));
                using (var wr = new StringWriter())
                {
                    xs.Serialize(wr, SelectedDossier);
                    await FileIO.WriteTextAsync(Fichier, wr.ToString(), Windows.Storage.Streams.UnicodeEncoding.Utf8);
                }
            }
            return retour;
        }

        /// <summary>
        /// Importer un fichier XML à partir de la serialization
        /// </summary>
        /// <returns>la liste des éventuelles erreurs</returns>
        public async Task<string> ImporterXml()
        {
            var retour = Validate();
            
            if (string.IsNullOrWhiteSpace(retour))
            {
                var xml = await FileIO.ReadTextAsync(Fichier, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                var xsb = new XmlSerializer(typeof(Dossier));
                Dossier resultImport;
                using (var rd = new StringReader(xml))
                {
                    resultImport = xsb.Deserialize(rd) as Dossier;
                    if (!EcraserDossier)
                    {
                        RemiseEnPlaceParent(resultImport);
                        if (resultImport != null) resultImport.DossierParent = SelectedDossier;
                        RemiseEnPlaceIcone(resultImport);
                        SelectedDossier.SousDossier.Add(resultImport);
                    }
                    else
                    {
                        RemiseEnPlaceParent(resultImport);
                        RemiseEnPlaceIcone(resultImport);
                        if (resultImport?.Titre != null)
                        {
                            SelectedDossier.Titre = resultImport.Titre;
                        }

                        if (resultImport.SousDossier.Count > 0)
                        {
                            SelectedDossier.SousDossier.Clear();
                            foreach (var dossier in resultImport.SousDossier)
                            {
                                SelectedDossier.SousDossier.Add(dossier);
                            }
                        }

                        if (resultImport.ListeMotDePasse.Count > 0)
                        {
                            SelectedDossier.ListeMotDePasse.Clear();
                            foreach (var mdp in resultImport.ListeMotDePasse)
                            {
                                SelectedDossier.ListeMotDePasse.Add(mdp);
                            }
                        }
                    }
                }
            }
            return retour;
        }
        #endregion


        #region gestion kwi
        /// <summary>
        /// Exporte le fichier chiffré directement
        /// </summary>
        /// <returns>la liste des erreurs</returns>
        public async Task<string> ExporterKwi()
        {
            var retour = Validate();

            if (string.IsNullOrWhiteSpace(retour))
            {
                if (!StringUtils.GetExtension(Fichier.Name).Equals(FormatChoisi.Format))
                {
                    await Fichier.RenameAsync(Fichier.Name + "." + FormatChoisi.Format);
                }

                var xs = new XmlSerializer(typeof(Dossier));
                using (var wr = new StringWriter())
                {
                    xs.Serialize(wr, SelectedDossier);
                    var dataToSave = CryptUtils.AesEncryptStringToByteArray(wr.ToString(), MdpCypher, MdpCypher);
                    await FileIO.WriteBytesAsync(Fichier, dataToSave);
                    //anciennement pour exporter tout le fichier
                    //await FileIO.WriteTextAsync(fichier, wr.ToString(), Windows.Storage.Streams.UnicodeEncoding.Utf8);
                }

                //await FileIO.WriteBytesAsync(fichier, await ContexteAppli.fichierPassword.lireByteArray());
            }
            return retour;
        }

        /// <summary>
        /// Importe le fichier chiffré dans le fichier de travail et le recharge
        /// </summary>
        /// <returns>la liste des erreurs</returns>
        public async Task<string> ImporterKwi()
        {
            var retour = Validate();

            if (string.IsNullOrWhiteSpace(retour))
            {
                var buffer = await FileIO.ReadBufferAsync(Fichier);
                byte[] inFile = buffer.ToArray();
                var xml = CryptUtils.AesDecryptByteArrayToString(inFile, MdpCypher, MdpCypher);
                var xsb = new XmlSerializer(typeof(Dossier));
                Dossier resultImport;
                using (var rd = new StringReader(xml))
                {
                    resultImport = xsb.Deserialize(rd) as Dossier;
                    if (!EcraserDossier)
                    {
                        RemiseEnPlaceParent(resultImport);
                        if (resultImport != null) resultImport.DossierParent = SelectedDossier;
                        RemiseEnPlaceIcone(resultImport);
                        SelectedDossier.SousDossier.Add(resultImport);
                    }
                    else
                    {
                        RemiseEnPlaceParent(resultImport);
                        RemiseEnPlaceIcone(resultImport);

                        if (resultImport?.Titre != null)
                        {
                            SelectedDossier.Titre = resultImport.Titre;
                        }

                        if (resultImport.SousDossier.Count > 0)
                        {
                            SelectedDossier.SousDossier.Clear();
                            foreach (var dossier in resultImport.SousDossier)
                            {
                                SelectedDossier.SousDossier.Add(dossier);
                            }
                        }

                        if (resultImport.ListeMotDePasse.Count > 0)
                        {
                            SelectedDossier.ListeMotDePasse.Clear();
                            foreach (var mdp in resultImport.ListeMotDePasse)
                            {
                                SelectedDossier.ListeMotDePasse.Add(mdp);
                            }
                        }
                    }
                }
            }
            return retour;
        }
        #endregion

        #region gestion dossier
        /// <summary>
        /// Permet de naviguer dans la dlg des dossiers
        /// </summary>
        /// <param name="dossier"></param>
        public void NaviguerDossier(Dossier dossier)
        {
            IsParentVisible = dossier.DossierParent != null;
            DossierEncours = dossier;
            ListeDossierAffiche = (dossier.SousDossier != null) ? new ObservableCollection<Dossier>(dossier.SousDossier) : new ObservableCollection<Dossier>();
        }

        /// <summary>
        /// Change l'emplacement du mot de passe avec le dossier en cours de navigation
        /// </summary>
        public void ChangerEmplacementDossier()
        {
            SelectedDossier = DossierEncours;
            SelectedDossierAbstract = DossierEncours;
        }

        #endregion
    }
}
