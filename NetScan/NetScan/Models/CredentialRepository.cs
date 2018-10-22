using System.Net;

namespace NetScan
{
    /// <summary>
    /// Operates on user credential in Microsoft Credentials Manager
    /// </summary>
    public class CredentialRepository
    {
        #region Private Members

        /// <summary>
        /// Reference name that is used in Microsoft Credential Manager
        /// </summary>
        private readonly string _credentialsName = "NetScanCredentials";

        #endregion

        #region Public Methods

        /// <summary>
        /// Stores <see cref="NetworkCredential"/> in Windows' Credential Manager
        /// </summary>
        /// <param name="credential">NetworkCredential to save in Credential Manager</param>
        public void SaveCredential(NetworkCredential credential)
        {
            using (var credentialManager = new CredentialManagement.Credential())
            {    
                credentialManager.Username = string.IsNullOrEmpty(credential.Domain) ? credential.UserName : $"{credential.Domain}\\{credential.UserName}";
                credentialManager.Password = credential.Password;
                credentialManager.Target = _credentialsName;
                credentialManager.Type = CredentialManagement.CredentialType.Generic;
                credentialManager.PersistanceType = CredentialManagement.PersistanceType.LocalComputer;
                credentialManager.Save();
            }
        }

        /// <summary>
        /// Restores credential from Windows Credential Manager, and stores in <see cref="Credential"/>
        /// </summary>
        /// <returns>Credential from Credential Manager</returns>
        public NetworkCredential GetCredentials()
        {
            using (var credentialManager = new CredentialManagement.Credential())
            {
                var credential = new NetworkCredential();
                credentialManager.Target = _credentialsName;
                credentialManager.Load();
                if (credentialManager.Exists())
                {
                    credential.UserName = credentialManager.Username;
                    // Username was set with domain, for example "DOMAIN\Username"
                    if (credential.UserName.Contains("\\"))
                    {
                        // Domain will be set to a string before backslash.
                        credential.Domain = credential.UserName.Split('\\')[0];
                        // Username will be set to a string after backslash.
                        credential.UserName = credential.UserName.Split('\\')[1];
                    }
                    credential.Password = credentialManager.Password;
                }
                return credential;
            }
        }

        #endregion
    }
}
