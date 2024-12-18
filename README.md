# Portail-OptiVille
Un portail pour les fournisseurs ainsi que les employés. Le but étant de créer un service centralisé.

Avant de pouvoir utiliser le projet, il faut renommer le fichier appsettings.json.exemple en appsettings.json
- Là où c'est écrit: "DefaultConnection", c'est la String de connexion à la BD. La changer avant l'utilisation.
- La section "DefaultMail" doit être changé également
- "MailAddress" -> Il faut mettre l'adresse ou l'alias que le destinataire des courriel va voir.
- "CredMail" -> L'adresse qui a l'alias. Ex.: C'est JohnDoe qui envoie au nom de no-reply@johndoe.ca
- "CredPassApp" -> Le mot de passe d'application de "MailAddress"
- "SmtpAddr" -> L'adresse du serveur SMTP.

- Le "SecretKey" de la section JWT est la clé privé utilisé pour l'encryption des cookies. Je vous conseille fortement de la changer et de ne pas la montrer à personne.
