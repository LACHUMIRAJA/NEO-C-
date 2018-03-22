using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;

namespace OrderVault
{
    public class Contract1 : SmartContract
    {

        public static Object Main(string oper, params Object[] args)
        {

           byte[] vaultOwners=null;
           byte[] orderHashes = null;
           uint beginTime=0;
           uint endTime=0;
           byte[] approver;
      
            switch (oper)
            {

                case "addOwner":
                    {
                       
                        byte[] newOwner = (byte[])args[0];
                        
                        Runtime.Notify(newOwner+ " is Added");
                        Storage.Put(Storage.CurrentContext, newOwner, "True");
                        int i = vaultOwners.Length;
                        vaultOwners[i] = (newOwner[0]);
                        Storage.Put(Storage.CurrentContext, vaultOwners, newOwner);
                        break;
                    }
                case "removeOwner":

                    {
                        
                        byte[] owner = (byte[])args[0];
                        Runtime.Notify(owner+ " is Removed");
                        Storage.Put(Storage.CurrentContext, owner, "False");
                       
                        for (int i = 0; i <= vaultOwners.Length; i++)
                        {
                            if (vaultOwners[i] == owner[0])
                            { 
                                vaultOwners[i] = 0; 
                                vaultOwners[i]= vaultOwners[i+1];
                                

                                return vaultOwners;
                            }
                        }
                       
                        return false;
                    }

                case "openVault":
                    {
                        uint startTime = (uint)args[0];
                        uint closureTime = (uint)args[1];


                        byte[] check = Storage.Get(Storage.CurrentContext, "Vault is Open");
                        byte[] check1 = Storage.Get(Storage.CurrentContext, "Vault is Seal");


                        uint now = (uint)args[2];// Runtime.Time;

                        if (check1.Equals("False") && (check.Equals("False") && startTime <= now && closureTime >= now && closureTime >= startTime))
                        {
                            beginTime = startTime;
                            endTime = closureTime;
                           
                            Storage.Put(Storage.CurrentContext, "Vault is Open", "True");
                            Runtime.Log("Vault is Opened");
                           
                        }
                        else
                        {
                            Storage.Put(Storage.CurrentContext, "Vault is Open", "False");
                            Runtime.Log("Vault is not Opened");
                        }
                        return Storage.Get(Storage.CurrentContext,"Vault is Open");

                    }


                case "orderVault":
                    {
                        
                        approver = (byte[])args[0];

                        vaultOwners = approver;

                        Storage.Put(Storage.CurrentContext,"Approver" , approver);
                        Runtime.Log("Approver Setted Successfully");
                        Storage.Put(Storage.CurrentContext, "vaultOwners", approver);
                        return true;
                    }

                case "sealVault":
                    {
                        uint now = (uint)args[0];
                       
                        byte[] check1 = Storage.Get(Storage.CurrentContext, "Vault is Seal");

                        if (check1.Equals("False"))
                            {
                                endTime = now;
                              
                                Storage.Put(Storage.CurrentContext, "Vault is Seal", "True");
                              
                                Storage.Put(Storage.CurrentContext, "Vault is Open", "False");
                                Runtime.Log("Vault is closed and Sealed Successfully");
                           
                                 return Storage.Get(Storage.CurrentContext, "Vault is Seal");
                            
                        }
                        Runtime.Log("Vault is not Sealed");
                        return Storage.Get(Storage.CurrentContext, "Vault is Seal");
                    }

               
                case "getnumoforders":
                    {
                        Runtime.Notify(orderHashes.Length);
                        return orderHashes.Length;

                    }


            }

            if(oper == "extendVault")
            {
                uint closureTime = (uint)args[0];
                uint now = (uint)args[1];

                byte[] check = Storage.Get(Storage.CurrentContext, "Vault is Open");
                byte[] check1 = Storage.Get(Storage.CurrentContext, "Vault is Seal");
                if (check1.Equals("False") && check.Equals("True") && beginTime <= now && closureTime >= now && closureTime >= beginTime)
                {
                    endTime = closureTime;
                    Storage.Put(Storage.CurrentContext, "Vault is Open", "True");
                    Runtime.Log("Vault Time is extended");
                    return true;
                }

                Runtime.Log("Vault Time is not extended");
                return false;


            }


            if (oper == "storeVault")
            {


                byte[] oHash = (byte[])args[0];
                byte[] orderId = ((byte[])args[1]);
                uint now = (uint)args[2];

                byte[] check = Storage.Get(Storage.CurrentContext, "Vault is Open");
                byte[] check1 = Storage.Get(Storage.CurrentContext, "Vault is Seal");


                if (check1.Equals("False") && check.Equals("True") && (beginTime <= now) && !(endTime >= now) && (endTime >= beginTime))
                {


                    orderHashes = oHash;
                    // Storage.Put(Storage.CurrentContext, orderHashes, oHash);
                    Storage.Put(Storage.CurrentContext, orderHashes, "True");
                    Storage.Put(Storage.CurrentContext, orderId, "True");
                    Runtime.Log("Well Stored");
                }
                else
                {
                    Runtime.Log("Vault Not Stored");
                }
                return true;

            }



            if (oper == "closeVault")
             {
                byte[] check = Storage.Get(Storage.CurrentContext, "Vault is Open");
                byte[] check1 = Storage.Get(Storage.CurrentContext, "Vault is Seal");

                uint now = (uint)args[0];
                if (check1.Equals("False") && check.Equals("True"))
                {
                    endTime = now;
                    Storage.Put(Storage.CurrentContext, "Vault is Open", "False");
                    Runtime.Log("Vault is Closed Successfully");
                    return true;
                }
                Runtime.Log("Vault is does not Closed");
                return false; 
            }



            
            if (oper == "orderLocated")
            {
                byte[] hash = (byte[])args[0];
                byte[] orderId = (byte[])args[1];

                Runtime.Log("Order  Location Checked");
               // Storage.Get(Storage.CurrentContext, orderHashes);
                return Storage.Get(Storage.CurrentContext, orderId);
               
                

            }




            return false;


            }

            
         }
}

