using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace Integrator
{
    public class Contract1 : SmartContract
    {
        public static event Action<byte[]> AuthourizationLog;
        public static object Main(String operation,object[] args)
        {
             /*if (operation == "changeapprover")
            {
                byte[] oldapprover = (byte[])args[0];
                byte[] newapprover=(byte[])args[1];
                return ChangeApprover(oldapprover,newapprover);
            }*/
            if (operation == "changeapprover")
            {
                
                byte[] newapprover = (byte[])args[0];
                return ChangeApprover(newapprover);
            }
            if (operation == "deploy")
                {
                    byte[] account = (byte[])args[0];
                    return Deploy(account);
            }
           /* if (operation == "addadress")
            {
                byte[] address = (byte[])args[0];                  
                return addauthorizedaddress(address);
            }
            if (operation == "removeadress")
            {
                byte[] address = (byte[])args[0];
                return removeauthorizedaddress(address);
            }*/
            if (operation == "addaddr")
            {
                byte[] value = (byte[])args[0];
                byte[] address = (byte[])args[1];
                return addaddr(value,address);
            }
            if (operation == "Getaddr")
            {
                byte[] value = (byte[])args[0];
                byte[] address = (byte[])args[1];
                return Getaddress(value, address);
            }
            if (operation == "removead")
            {
                byte[] value = (byte[])args[0];
                byte[] address = (byte[])args[1];
                return removead(value, address);
            }




            return false;
        }

        
        public static bool ChangeApprover(byte[] newapprover)
        {

            //byte[] oldapprover = Storage.Get(Storage.CurrentContext, "Approver");
            Storage.Delete(Storage.CurrentContext, "Approver");
            Storage.Put(Storage.CurrentContext, "NewApprover", newapprover);
            newapprover = Storage.Get(Storage.CurrentContext, "NewApprover");
            Runtime.Notify(newapprover);
            return true;
        }


        /*public static bool ChangeApprover(byte[] oldapprover,byte[] newapprover)
        {
           
            oldapprover = Storage.Get(Storage.CurrentContext, "Approver");
            Storage.Delete(Storage.CurrentContext, oldapprover);
            Storage.Put(Storage.CurrentContext, "NewApprover",newapprover);
            newapprover = Storage.Get(Storage.CurrentContext, "NewApprover");
            Runtime.Notify(newapprover);
            return true;
        }*/
        public static bool Deploy(byte[] account)
        {
                     
                Storage.Put(Storage.CurrentContext, "Approver", account);
                byte[] approver = Storage.Get(Storage.CurrentContext, "Approver");
                Runtime.Notify(approver);
                return true;
            
        }
       /* public static bool addauthorizedaddress(byte[] address)
        {

           
            byte[] AuthorizedAddress = address.Concat("AuthorizedAddress".AsByteArray());
            Storage.Put(Storage.CurrentContext, "AuthorizedAddress", address);
            byte[] addresses = Storage.Get(Storage.CurrentContext, "AuthorizedAddress");
            Runtime.Notify(addresses);
            AuthourizationLog(address);
            

            return true;
        }
        public static bool removeauthorizedaddress(byte[] address)
        {
            Storage.Delete(Storage.CurrentContext, "AuthorizedAddress");
            return true;
        }*/
        public static byte[] AddressIndexName( byte[] value,byte[] address)
        {
            return value.Concat(address);
        }
        public static bool addaddr(byte[] value, byte[] address)
        {
            byte[] indexName = AddressIndexName(value, address);
            Runtime.Notify("addaddr() indexName", indexName);
            Storage.Put(Storage.CurrentContext,indexName, address);
            //Runtime.Notify("GetBalanceOfCurrency() indexName", indexName);
            return true;
        }
        public static bool Getaddress(byte[] value,byte[] address)
        {
            byte[] indexName = AddressIndexName(value, address);
            Runtime.Notify("Getaddress() indexName", indexName);

            byte[] currentaddress = Storage.Get(Storage.CurrentContext, indexName);
            Runtime.Notify("Getaddress() value", value);
            Runtime.Notify("Getaddress() currentaddress", currentaddress);
            return true;
        }
        public static bool removead(byte[] value, byte[] address)
        {
            byte[] indexName = AddressIndexName(value, address);
            Runtime.Notify("Getaddress() indexName", indexName);
            Storage.Delete(Storage.CurrentContext, indexName);

            /*byte[] currentaddress = Storage.Get(Storage.CurrentContext, indexName);
            Runtime.Notify("Getaddress() value", value);
            Runtime.Notify("Getaddress() currentaddress", currentaddress);*/
            return true;
        }
    } 
}
