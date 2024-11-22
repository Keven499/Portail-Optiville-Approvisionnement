using Microsoft.EntityFrameworkCore;
using Portail_OptiVille.Data.FormModels;
using Portail_OptiVille.Data.Models;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Portail_OptiVille.Data.Services
{
    public class ContactsService
    {
        private readonly A2024420517riGr1Eq6Context _context;
        private HistoriqueService _historiqueFiche;

        public ContactsService(A2024420517riGr1Eq6Context context, HistoriqueService historiqueFiche)
        {
            _context = context;
            _historiqueFiche = historiqueFiche;
        }

        public async Task SaveContactsData(ContactHosterFormModel contactHosterFormModelDto)
        {
            var lastFournisseurId = await _context.Fournisseurs.MaxAsync(f => (int?)f.IdFournisseur);
            foreach (var contactFromList in contactHosterFormModelDto.ContactList)
            {
                await AddContact(contactFromList, (int)lastFournisseurId);
            }
        }

        public async Task UpdateContactsData(ContactHosterFormModel contactHosterFormModel, string email)
        {
            int idFournisseur = 0;
            /*  USER CAN DO THE FOLLOWING ACTIONS:
                - MODIFY (Add & Remove)
                - ADD (Add)
                - REMOVE (Remove)
                ALL THESE ACTIONS NEEDS TO BE INSERTED IN THE TABLE HISTORIQUE IN ONE ADD
            */
            foreach (var contactFromList in contactHosterFormModel.ContactList)
            {
                var existingContact = await _context.Contacts.SingleOrDefaultAsync(c => c.IdContact == contactFromList.IdContact);
                if (existingContact != null)
                {
                    idFournisseur = (int)existingContact.Fournisseur;
                    existingContact.IdContact = contactFromList.IdContact;
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
                        existingTelephone.Contact = contactFromList.IdContact;
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
                    await AddContact(contactFromList, idFournisseur);
                }
            }
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
