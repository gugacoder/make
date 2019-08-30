using System;
using System.Linq;
using PackDm.Actions;
using PackDm.Algorithms;
using PackDm.Helpers;

namespace PackDm.Model
{
  public class Credential
  {
    public string User { get; set; }
    public string Pass { get; set; }

    public string GetEncriptedData()
    {
      var encryption = new Cryptography();
      var token = User + ":" + Pass;
      var encriptedData = encryption.Encrypt(token);
      return encriptedData;
    }

    public void SetEncriptedData(string encryptedData)
    {
      var encryption = new Cryptography();
      var token = encryption.Decrypt(encryptedData);
      var parts = token.Split(':');

      this.User = parts.FirstOrDefault();
      this.Pass = parts.Skip(1).FirstOrDefault();
    }
  }
}

