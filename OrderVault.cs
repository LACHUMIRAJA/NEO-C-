using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Linq;
using System.Numerics;

namespace OrderVault
{
    public class Contract1 : SmartContract
    {

        public static Object Main(string oper, params Object[] args)
        {

            
            //BigInteger orderHashes = 0;
            BigInteger beginTime;
            BigInteger endTime;
            byte[] approver;

           // Storage.Put(Storage.CurrentContext, "OrderHash", orderHashes);

            switch (oper)
            {

                case "addOwner":
                    {

                        byte[] newOwner = (byte[])args[0];
                       
                        Runtime.Notify(newOwner + " is Added");
                        Storage.Put(Storage.CurrentContext, newOwner, 1);
                        Storage.Put(Storage.CurrentContext, newOwner.Concat(Neo.SmartContract.Framework.Helper.AsByteArray("owner")), "owner");
                        return true;
                        
                    }
                case "removeOwner":

                    {

                        byte[] reowner = (byte[])args[0];
                        Runtime.Notify(reowner + " is Removed");
                        Storage.Put(Storage.CurrentContext, reowner, 0);
                        if (Storage.Get(Storage.CurrentContext, reowner)[0] == 0)
                        {
                            Storage.Delete(Storage.CurrentContext, reowner);
                            Storage.Delete(Storage.CurrentContext, reowner.Concat(Neo.SmartContract.Framework.Helper.AsByteArray("owner")));
                            return true;
                        }

                        return false;
                    }

                case "openVault":
                    {
                       BigInteger startTime = (BigInteger)args[0];
                       BigInteger closureTime = (BigInteger)args[1];

                        byte[] check = Storage.Get(Storage.CurrentContext, "Vault is Open");
                        byte[] check1 = Storage.Get(Storage.CurrentContext, "Vault is Seal");


                       BigInteger now = (BigInteger)args[2];// Runtime.Time;

                        if (check1.Equals(0) && (check.Equals(0) && startTime <= now && closureTime >= now && closureTime >= startTime))
                        {
                            Storage.Put(Storage.CurrentContext, "starttime", startTime);
                            Storage.Put(Storage.CurrentContext, "closuretime", closureTime);

                            Storage.Put(Storage.CurrentContext, "Vault is Open", 1);
                            Runtime.Log("Vault is Opened");

                        }
                        else
                        {
                            Storage.Put(Storage.CurrentContext, "Vault is Open", 0);
                            if(Storage.Get(Storage.CurrentContext,"Vault is Open")[0]==0)
                            {
                                Storage.Delete(Storage.CurrentContext, "Vault is Open");
                            }

                            Runtime.Log("Vault is Closed");
                        }
                        return Storage.Get(Storage.CurrentContext, "Vault is Open");

                    }


                case "orderVault":
                    {
                        byte[] check = Storage.Get(Storage.CurrentContext, "Vault is Open");
                        byte[] check1 = Storage.Get(Storage.CurrentContext, "Vault is Seal");

                        if (check1.Equals(0) && (check.Equals(0)))
                            {

                            approver = (byte[])args[0];

                            Storage.Put(Storage.CurrentContext, "Approver", approver);
                            Runtime.Log("Approver Setted Successfully");
                            return true;
                        }
                        return false;
                    }

                case "sealVault":
                    {
                        BigInteger now = (BigInteger)args[0];

                        byte[] check1 = Storage.Get(Storage.CurrentContext, "Vault is Seal");

                        if (check1.Equals(0))
                        {
                            Storage.Put(Storage.CurrentContext, "closuretime", now);

                            Storage.Put(Storage.CurrentContext, "Vault is Seal", 1);

                            Storage.Put(Storage.CurrentContext, "Vault is Open", 0);
                            if (Storage.Get(Storage.CurrentContext, "Vault is Open")[0] == 0)
                            {
                                Storage.Delete(Storage.CurrentContext, "Vault is Open");
                            }

                            Runtime.Log("Vault is closed and Sealed Successfully");

                            return Storage.Get(Storage.CurrentContext, "Vault is Seal");

                        }
                        Runtime.Log("Vault is not Sealed");
                        return Storage.Get(Storage.CurrentContext, "Vault is Seal");
                    }


                /*case "getnumoforders":
                    {
                        BigInteger nooh = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, "OrderHash"));
                        Runtime.Notify("No. Of OrderHashes " +nooh);
                        return true;

                    }*/


            }

            if (oper == "extendVault")
            {
                BigInteger closureTime = (BigInteger)args[0];
                BigInteger now = (BigInteger)args[1];

               

                beginTime = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, "starttime"));



                byte[] check = Storage.Get(Storage.CurrentContext, "Vault is Open");
                byte[] check1 = Storage.Get(Storage.CurrentContext, "Vault is Seal");

                Runtime.Notify("Checking...");
                if (check1.Equals(0) && check.Equals(1) && beginTime <= now  && closureTime >= now && closureTime >= beginTime)
                {
                    Storage.Put(Storage.CurrentContext, "closuretime", closureTime);
                    Storage.Put(Storage.CurrentContext, "Vault is Open", 1);
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
                BigInteger now = (BigInteger)args[2];

                beginTime = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, "starttime"));
                endTime = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, "closuretime"));

                byte[] check = Storage.Get(Storage.CurrentContext, "Vault is Open");
                byte[] check1 = Storage.Get(Storage.CurrentContext, "Vault is Seal");

                //Storage.Put(Storage.CurrentContext, "OrderHash", Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, "OrderHash"))+ 1); ;

                Runtime.Log("Checking....");
                Runtime.Notify(beginTime, endTime);
                if (check1.Equals(0) && check.Equals(1) && beginTime <= now && endTime >= now  && endTime >= beginTime)
                {
                    Storage.Put(Storage.CurrentContext, oHash.Concat(Neo.SmartContract.Framework.Helper.AsByteArray(" ")),"orderhashes" );
                    Storage.Put(Storage.CurrentContext, oHash, 1);
                    Storage.Put(Storage.CurrentContext, orderId,1);
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
                if (check1.Equals(0) && check.Equals(1))
                {
                    Storage.Put(Storage.CurrentContext, "closuretime", now);
                    Storage.Put(Storage.CurrentContext, "Vault is Open", 0);

                    if (Storage.Get(Storage.CurrentContext, "Vault is Open")[0] == 0)
                    {
                        Storage.Delete(Storage.CurrentContext, "Vault is Open");
                    }

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
