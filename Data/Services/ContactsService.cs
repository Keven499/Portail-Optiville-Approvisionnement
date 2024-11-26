using Microsoft.EntityFrameworkCore;
using Portail_OptiVille.Data.FormModels;
using Portail_OptiVille.Data.Models;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;

namespace Portail_OptiVille.Data.Services
{
    public class ContactsService
    {
        private readonly A2024420517riGr1Eq6Context _context;
        private HistoriqueService _historiqueService;

        public ContactsService(A2024420517riGr1Eq6Context context, HistoriqueService historiqueService)
        {
            _context = context;
            _historiqueService = historiqueService;
        }

        public async Task SaveContactsData(ContactHosterFormModel contactHosterFormModelDto)
        {
            var lastFournisseurId = await _context.Fournisseurs.MaxAsync(f => (int?)f.IdFournisseur);
            foreach (var contactFromList in contactHosterFormModelDto.ContactList)
            {
                await AddContact(contactFromList, (int)lastFournisseurId);
            }
        }

        public async Task UpdateContactsData(ContactHosterFormModel contactHosterFormModel, int idFournisseur, string email)
        {
            bool isEqual = true;
            string[] keyData = {"Prénom", "Nom", "Fonction", 
                                "Adresse courriel", "Type de numéro", "Numéro de téléphone", "Poste"};
            var oldDict = new Dictionary<string, object> { { "Section", "Contacts" } };
            var newDict = new Dictionary<string, object> { { "Section", "Contacts" } };
            /*  USER CAN DO THE FOLLOWING ACTIONS:
                - MODIFY (Add & Remove)
                - ADD (Add)
                - REMOVE (Remove)
                ALL THESE ACTIONS NEEDS TO BE INSERTED IN THE TABLE HISTORIQUE IN ONE ADD
            */

            // - REMOVE (Remove)
            List<Contact> oldDataContacts = await _context.Contacts.Where(c => c.Fournisseur == idFournisseur).ToListAsync();
            List<ContactFormModel> newDataContacts = contactHosterFormModel.ContactList;

            // I NEED A LIST OF STRING FOR EACH KEY BECAUSE THE USER COULD REMOVE MORE THEN 1 CONTACT IN A SINGLE MODIFICATION
            List<string> catToAddPrenom = new List<string>();
            List<string> catToAddNom = new List<string>();
            List<string> catToAddFonction = new List<string>();
            List<string> catToAddAdresseCourriel = new List<string>();
            List<string> catToAddTypeNumeroTel = new List<string>();
            List<string> catToAddNumeroTel = new List<string>();
            List<string> catToAddPoste = new List<string>();

            List<string> catToRemovePrenom = new List<string>();
            List<string> catToRemoveNom = new List<string>();
            List<string> catToRemoveFonction = new List<string>();
            List<string> catToRemoveAdresseCourriel = new List<string>();
            List<string> catToRemoveTypeNumeroTel = new List<string>();
            List<string> catToRemoveNumeroTel = new List<string>();
            List<string> catToRemovePoste = new List<string>();

            List<Contact> missingContacts = oldDataContacts
                .Where(c => !newDataContacts
                    .Any(modelContact => modelContact.IdContact == c.IdContact))
                .ToList();
            foreach (var missingContact in missingContacts)
            {
                var phone = await _context.Telephones
                    .Where(t => t.Contact == missingContact.IdContact)
                    .FirstOrDefaultAsync();
                isEqual = false;
                _context.Contacts.Remove(missingContact);
                catToRemovePrenom.Add(missingContact.Prenom);
                catToRemoveNom.Add(missingContact.Nom);
                catToRemoveFonction.Add(missingContact.Fonction);
                catToRemoveAdresseCourriel.Add(missingContact.AdresseCourriel);
                catToRemoveTypeNumeroTel.Add(phone.Type);
                catToRemoveNumeroTel.Add(phone.NumTelephone);
                catToRemovePoste.Add(phone.Poste);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue lors l'effacement d'un contact", ex);
            }

            //- MODIFY (Add & Remove)
            foreach (var contactFromList in contactHosterFormModel.ContactList)
            {
                isEqual = false;
                var existingContact = await _context.Contacts.SingleOrDefaultAsync(c => c.IdContact == contactFromList.IdContact);
                if (existingContact != null)
                {
                    if (existingContact.Prenom != contactFromList.Prenom) { catToAddPrenom.Add(contactFromList.Prenom);
                                                                            catToRemovePrenom.Add(existingContact.Prenom); }
                    if (existingContact.Nom != contactFromList.Nom) { catToAddNom.Add(contactFromList.Nom);
                                                                        catToRemoveNom.Add(existingContact.Nom); }
                    if (existingContact.Fonction != contactFromList.Fonction) { catToAddFonction.Add(contactFromList.Fonction);
                                                                                catToRemoveFonction.Add(existingContact.Fonction); }
                    if (existingContact.AdresseCourriel != contactFromList.AdresseCourriel) { catToAddAdresseCourriel.Add(contactFromList.AdresseCourriel);
                                                                                                catToRemoveAdresseCourriel.Add(existingContact.AdresseCourriel); }
                    existingContact.Prenom = contactFromList.Prenom;
                    existingContact.Nom = contactFromList.Nom;
                    existingContact.Fonction = contactFromList.Fonction;
                    existingContact.AdresseCourriel = contactFromList.AdresseCourriel;
                    try
                    {
                        _context.Contacts.Update(existingContact);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Une erreur est survenue lors de la mise à jour du contact", ex);
                    }

                    var existingTelephone = await _context.Telephones.SingleOrDefaultAsync(t => t.Contact == existingContact.IdContact);
                    if (existingTelephone != null)
                    {
                        if (existingTelephone.Type != contactFromList.TypeTelephone) { catToAddTypeNumeroTel.Add(contactFromList.TypeTelephone);
                                                                                                catToRemoveTypeNumeroTel.Add(existingTelephone.Type); }
                        if (existingTelephone.NumTelephone != contactFromList.Telephone) { catToAddNumeroTel.Add(contactFromList.Telephone);
                                                                                        catToRemoveNumeroTel.Add(existingTelephone.NumTelephone); }    
                        if (existingTelephone.Poste != contactFromList.Poste) { catToAddPoste.Add(contactFromList.Poste);
                                                                                catToRemovePoste.Add(existingTelephone.Poste); }                                                                   
                        existingTelephone.Type = contactFromList.TypeTelephone;
                        existingTelephone.NumTelephone = contactFromList.Telephone;
                        existingTelephone.Poste = contactFromList.Poste;   
                        try
                        {
                            _context.Telephones.Update(existingTelephone);
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Une erreur est survenue lors de la mise à jour du téléphone du contact", ex);
                        }
                    }
                }
                else
                {
                    // CREATES THE NEW CONTACT IF NOT IN THE DATABASE MEANING IT'S A NEW CONTACT FROM THE MODIFICATION
                    // ADD (Add)
                    isEqual = false;
                    var contact = new Contact
                    {
                        Prenom = contactFromList.Prenom,
                        Nom = contactFromList.Nom,
                        Fonction = contactFromList.Fonction,
                        AdresseCourriel = contactFromList.AdresseCourriel,
                        Fournisseur = idFournisseur 
                    };

                    try
                    {
                        _context.Contacts.Add(contact);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Une erreur est survenue lors de la sauvegarde du contact", ex);
                    }

                    var lastContactId = await _context.Contacts.MaxAsync(f => (int?)f.IdContact);
                    var telephone = new Telephone
                    {
                        Type = contactFromList.TypeTelephone,
                        NumTelephone = contactFromList.Telephone,
                        Poste = contactFromList.Poste,
                        Contact = lastContactId,
                        Coordonnee = null
                    };

                    catToAddPrenom.Add(contact.Prenom);
                    catToAddNom.Add(contact.Nom);
                    catToAddFonction.Add(contact.Fonction);
                    catToAddAdresseCourriel.Add(contact.AdresseCourriel);
                    catToAddTypeNumeroTel.Add(telephone.Type);
                    catToAddNumeroTel.Add(telephone.NumTelephone);
                    catToAddPoste.Add(telephone.Poste);

                    try
                    {
                        _context.Telephones.Add(telephone);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Une erreur est survenue lors de la sauvegarde du téléphone", ex);
                    }
                }
            }
            if (!catToRemovePrenom.IsNullOrEmpty()) oldDict.Add(keyData[0], string.Join(":", catToRemovePrenom));
            if (!catToRemoveNom.IsNullOrEmpty()) oldDict.Add(keyData[1], string.Join(":", catToRemoveNom));
            if (!catToRemoveFonction.IsNullOrEmpty()) oldDict.Add(keyData[2], string.Join(":", catToRemoveFonction));
            if (!catToRemoveAdresseCourriel.IsNullOrEmpty()) oldDict.Add(keyData[3], string.Join(":", catToRemoveAdresseCourriel));
            if (!catToRemoveTypeNumeroTel.IsNullOrEmpty()) oldDict.Add(keyData[4], string.Join(":", catToRemoveTypeNumeroTel));
            if (!catToRemoveNumeroTel.IsNullOrEmpty()) oldDict.Add(keyData[5], string.Join(":", catToRemoveNumeroTel));
            if (!catToRemovePoste.IsNullOrEmpty()) oldDict.Add(keyData[6], string.Join(":", catToRemovePoste));

            if (!catToAddPrenom.IsNullOrEmpty()) newDict.Add(keyData[0], string.Join(":", catToAddPrenom));
            if (!catToAddNom.IsNullOrEmpty()) newDict.Add(keyData[1], string.Join(":", catToAddNom));
            if (!catToAddFonction.IsNullOrEmpty()) newDict.Add(keyData[2], string.Join(":", catToAddFonction));
            if (!catToAddAdresseCourriel.IsNullOrEmpty()) newDict.Add(keyData[3], string.Join(":", catToAddAdresseCourriel));
            if (!catToAddTypeNumeroTel.IsNullOrEmpty()) newDict.Add(keyData[4], string.Join(":", catToAddTypeNumeroTel));
            if (!catToAddNumeroTel.IsNullOrEmpty()) newDict.Add(keyData[5], string.Join(":", catToAddNumeroTel));
            if (!catToAddPoste.IsNullOrEmpty()) newDict.Add(keyData[6], string.Join(":", catToAddPoste));
            string oldJSON = JsonConvert.SerializeObject(oldDict, Formatting.None);
            string newJSON = JsonConvert.SerializeObject(newDict, Formatting.None);
            if (!isEqual)
                await _historiqueService.ModifyEtat("Modifiée", idFournisseur, email, null, oldJSON, newJSON);
        }

        public async Task AddContact(ContactFormModel contactFromList, int idFournisseur)
        {
            var contact = new Contact
            {
                Prenom = contactFromList.Prenom,
                Nom = contactFromList.Nom,
                Fonction = contactFromList.Fonction,
                AdresseCourriel = contactFromList.AdresseCourriel,
                Fournisseur = idFournisseur 
            };

            try
            {
                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue lors de la sauvegarde du contact", ex);
            }

            var lastContactId = await _context.Contacts.MaxAsync(f => (int?)f.IdContact);
            var telephone = new Telephone
            {
                Type = contactFromList.TypeTelephone,
                NumTelephone = contactFromList.Telephone,
                Poste = contactFromList.Poste,
                Contact = lastContactId,
                Coordonnee = null
            };

            try
            {
                _context.Telephones.Add(telephone);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue lors de la sauvegarde du téléphone", ex);
            }
        }
    }
}
