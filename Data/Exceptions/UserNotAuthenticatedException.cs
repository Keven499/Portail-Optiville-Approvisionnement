namespace Portail_OptiVille.Data.Exceptions
{
    public class UserNotAuthenticatedException : Exception
    {
        public UserNotAuthenticatedException() : base("Utilisateur non connecté.") { }

        public UserNotAuthenticatedException(string message) : base(message) { }

        public UserNotAuthenticatedException(string message, Exception inner) : base(message, inner) { }
    }

}
