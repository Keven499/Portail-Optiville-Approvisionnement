namespace Portail_OptiVille.Data.FormModels {
    public class ContactHosterFormModel 
    {
        public ContactHosterFormModel() {
            ContactList = new List<ContactFormModel>();
        }

        public List<ContactFormModel>? ContactList { get ; set; }
    }
}