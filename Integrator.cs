using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;

namespace OrderVault
{
    public class Contract1 : SmartContract
    {
        //public static event Action<string, string> LogApproverChanged;
        //public static event Action<string,string> LogAuthorizeAddressAdded;
        //public static event Action<string,string> LogAuthorizeAddressRemoved;
        public static Object Main(string oper, params Object[] args)
        {


            String address0 = "ANiSxR6mgYZxhTXpJqzjcnyFCQ16HVaC4W";

        string approver;

            switch(oper)
            {
                case "changeApprover":
                    {
                        string newApprover = (string)args[0];
                      if(!(newApprover.Equals(address0)))
                       {
                            approver = newApprover;
                            // LogApproverChanged(approver, newApprover);
                            Runtime.Log("Approver Changed");
                            Storage.Put(Storage.CurrentContext, "Approver", approver);
                            return true;

                      }
                      return false;

                    }

                case "addAutherizedAddress":
                    {
                        string appIntegrator = (string)args[0];
                        Storage.Put(Storage.CurrentContext, appIntegrator, "true");
                        //LogAuthorizedAddressAdded(appIntegrator,owner);
                        Runtime.Log("Authorized Address Added");
                        return true;
                    }
                case "removeAutherizedAddress":
                    {
                        string appIntegrator = (string)args[0];
                        Storage.Put(Storage.CurrentContext, appIntegrator, "false");
                        //LogAuthorizedAddressremoved(appIntegrator,owner);
                        Runtime.Log("Authorized Address Removed");
                        return true;
                    }
            }
return false;
        }

      
    }
    }
