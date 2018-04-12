using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace IntegratorandTradeFee
{
    public class Contract1 : SmartContract
    {
        public static Object Main(string oper, params Object[] args)
        {

            byte[] address0 = Neo.SmartContract.Framework.Helper.AsByteArray("abavag");
            byte[] approver;

            //BigInteger orderHashes = 0;
            BigInteger beginTime;
            BigInteger endTime;
            // Storage.Put(Storage.CurrentContext, "OrderHash", orderHashes);

            if (oper == "Deploy")
            {
                approver = (byte[])args[0];
                Storage.Put(Storage.CurrentContext, "Approver", approver);
                return true;
            }


            if (oper=="updateFeeSchedule")
            {
                uint baseTokenFee = (uint)args[0];
                uint etherTokenFee = (uint)args[1];
                uint normalTokenFee = (uint)args[2];

                  if (baseTokenFee >= 0 && baseTokenFee <= 1 && etherTokenFee >= 0 && etherTokenFee <= 1 && normalTokenFee >= 0)
                   {
                         Storage.Put(Storage.CurrentContext, "0", baseTokenFee);
                         Storage.Put(Storage.CurrentContext, "1", etherTokenFee);
                         Storage.Put(Storage.CurrentContext, "2", normalTokenFee);

                   }
                        return true;
             }


             if(oper== "calcTradeFee")
             {
                BigInteger values = (BigInteger)args[0];
                uint feeIndex = (uint)args[1];
                byte[] check;

                   if (feeIndex >= 0 && feeIndex <= 2 && values > 0)
                    {
                            if (feeIndex == 0)
                                check = Storage.Get(Storage.CurrentContext, "0");
                            else if (feeIndex == 1)
                                check = Storage.Get(Storage.CurrentContext, "1");
                            else
                                check = Storage.Get(Storage.CurrentContext, "2");


                            BigInteger I = new BigInteger(check);
                            BigInteger totalFees = (I * values);///(1 NEO);

                            if (totalFees > 0)
                            {

                                return totalFees;
                            }


                   }

                        return true;

             }


             if(oper== "calcTradeFeeMulti")
              {
                   uint[] values = (uint[])args[0];
                   uint[] feeIndexes = (uint[])args[1];

                        if (values.Length > 0 && feeIndexes.Length > 0 && values.Length == feeIndexes.Length)
                        {
                            BigInteger[] totalFees = new BigInteger[values.Length];
                            byte[] check;

                            for (uint i = 0; i < values.Length; i++)
                            {
                                if (feeIndexes[i] >= 0 && feeIndexes[i] <= 2 && values[i] > 0)
                                {
                                    if (feeIndexes[i] == 0)
                                        check = Storage.Get(Storage.CurrentContext, "0");
                                    else if (feeIndexes[i] == 1)
                                        check = Storage.Get(Storage.CurrentContext, "1");
                                    else
                                        check = Storage.Get(Storage.CurrentContext, "2");


                                    BigInteger I = new BigInteger(check);

                                    totalFees[i] = (values[i] * I);
                                }
                            }
                            
                            return totalFees;

                        }
                    return false;
              }




                if(oper== "changeApprover")
                  {
                        if (Runtime.CheckWitness(Storage.Get(Storage.CurrentContext, "Approver")))
                        {

                            byte[] newApprover = (byte[])args[0];
                            if (!(newApprover.Equals(address0)))
                            {
                                approver = newApprover;
                                // LogApproverChanged(approver, newApprover);
                                Runtime.Log("Approver Changed");
                                Storage.Put(Storage.CurrentContext, "Approver", approver);

                                return true;

                            }
                        }

                        return false;

                 }


                if(oper== "addAutherizedAddress")
                 {
                        if (Runtime.CheckWitness(Storage.Get(Storage.CurrentContext, "Approver")))
                        {

                            byte[] appIntegrator = (byte[])args[0];
                            Storage.Put(Storage.CurrentContext, appIntegrator, 1);
                            //LogAuthorizedAddressAdded(appIntegrator,owner);
                            Runtime.Log("Authorized Address Added");
                        }
                        return true;
                 }


                if (oper== "removeAutherizedAddress")
                 {
                        if (Runtime.CheckWitness(Storage.Get(Storage.CurrentContext, "Approver")))
                        {

                            byte[] appIntegrator = (byte[])args[0];
                            Storage.Put(Storage.CurrentContext, appIntegrator, 0);
                            if (Storage.Get(Storage.CurrentContext, appIntegrator)[0] == 0)
                            {
                                Storage.Delete(Storage.CurrentContext, appIntegrator);

                            }
                            //LogAuthorizedAddressremoved(appIntegrator,owner);
                            Runtime.Log("Authorized Address Removed");
                        }
                        return true;
                 }

            
                if(oper=="ecverify")  //parameters :Address, msghash, v,r,s, signer
                {
                    Runtime.Notify("Signature Started Verification");
                    byte[] address001 = (byte[])args[0];
                    byte _msgHash = (byte)args[1];
                    uint v = (uint)args[2];
                    byte r = (byte)args[3];
                    byte s = (byte)args[4];
                    byte[] _signer = (byte[])args[5];

                    if (_signer == address001)
                     {
                    return false;
                     }


                    if (v < 27)
                    {
                    v += 27;
                    }

                    if (v != 27 && v != 28)
                    {
                    return address001 == _signer;
                    }

                    if (v == 27)
                    {
                    Runtime.Log("It Verified");
                    return Main("ecrecover", _msgHash, v, r, s) == _signer;
                    }
                    else if (v == 28)
                    {
                    Runtime.Log("It Verified");
                    return Main("ecrecover", _msgHash, v, r, s) == _signer;
                    }

                return address001 == _signer;
            }


                if(oper=="ecrecover")
                {
                    byte _msgHash = (byte)args[0];
                    uint v = (uint)args[1];
                    byte r = (byte)args[2];
                    byte s = (byte)args[3];
                    String address1 = "ANiSxR6mgYZxhTXpJqzjcnyFCQ16HVaC4W";
                    return address1;
                }



                if(oper== "addOwner")
                 {

                        byte[] newOwner = (byte[])args[0];

                        Runtime.Notify(newOwner + " is Added");
                        Storage.Put(Storage.CurrentContext, newOwner, 1);
                        Storage.Put(Storage.CurrentContext, newOwner.Concat(Neo.SmartContract.Framework.Helper.AsByteArray("owner")), "owner");
                        return true;

                 }

                if(oper== "removeOwner")

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


                if (oper=="openVault")
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
                            if (Storage.Get(Storage.CurrentContext, "Vault is Open")[0] == 0)
                            {
                                Storage.Delete(Storage.CurrentContext, "Vault is Open");
                            }

                            Runtime.Log("Vault is Closed");
                        }
                        return Storage.Get(Storage.CurrentContext, "Vault is Open");

                 }


                if(oper== "orderVault")
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


               if(oper== "sealVault")
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


            

            if (oper == "extendVault")
            {
                BigInteger closureTime = (BigInteger)args[0];
                BigInteger now = (BigInteger)args[1];



                beginTime = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, "starttime"));



                byte[] check = Storage.Get(Storage.CurrentContext, "Vault is Open");
                byte[] check1 = Storage.Get(Storage.CurrentContext, "Vault is Seal");

                Runtime.Notify("Checking...");
                if (check1.Equals(0) && check.Equals(1) && beginTime <= now && closureTime >= now && closureTime >= beginTime)
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
                if (check1.Equals(0) && check.Equals(1) && beginTime <= now && endTime >= now && endTime >= beginTime)
                {
                    Storage.Put(Storage.CurrentContext, oHash.Concat(Neo.SmartContract.Framework.Helper.AsByteArray(" ")), "orderhashes");
                    Storage.Put(Storage.CurrentContext, oHash, 1);
                    Storage.Put(Storage.CurrentContext, orderId, 1);
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

