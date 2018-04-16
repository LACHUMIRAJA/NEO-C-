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


            //TRADEFEE_CONTRACT

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


            if (oper == "updateFeeSchedule")
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


            if (oper == "calcTradeFee")
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


            if (oper == "calcTradeFeeMulti")
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


            //INTEGRATOR_CONTRACT

            if (oper == "changeApprover")
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


            if (oper == "addAutherizedAddress")
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


            if (oper == "removeAutherizedAddress")
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


            //ECVERIFY_CONTRACT

            if (oper == "ecverify")  //parameters :Address, msghash, v,r,s, signer
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


            if (oper == "ecrecover")
            {
                byte _msgHash = (byte)args[0];
                uint v = (uint)args[1];
                byte r = (byte)args[2];
                byte s = (byte)args[3];
                String address1 = "ANiSxR6mgYZxhTXpJqzjcnyFCQ16HVaC4W";
                return address1;
            }

            //ORDERVAULT_CONTRACT

            if (oper == "addOwner")
            {

                byte[] newOwner = (byte[])args[0];

                Runtime.Notify(newOwner + " is Added");
                Storage.Put(Storage.CurrentContext, newOwner, 1);
                Storage.Put(Storage.CurrentContext, newOwner.Concat(Neo.SmartContract.Framework.Helper.AsByteArray("owner")), "owner");
                return true;

            }

            if (oper == "removeOwner")

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


            if (oper == "openVault")
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


            if (oper == "orderVault")
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


            if (oper == "sealVault")
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

            //----
            /*case "getnumoforders":
                {
                    BigInteger nooh = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, "OrderHash"));
                    Runtime.Notify("No. Of OrderHashes " +nooh);
                    return true;

                }*/

            //---


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




            //DEX1WAY_CONTRACT




            if (oper == "Dex1Wa")
            {

                byte[] _baseToken = (byte[])args[0];
                byte[] _etherToken = (byte[])args[1];

                if (_baseToken != null && _etherToken != null)
                {
                    byte[] wallet = Storage.Get(Storage.CurrentContext, "Approver");
                    Storage.Put(Storage.CurrentContext, "Wallet", wallet);
                    Storage.Put(Storage.CurrentContext, "Active", 1);
                    Storage.Put(Storage.CurrentContext, "BaseToken", _baseToken);
                    Storage.Put(Storage.CurrentContext, "EtherToken", _etherToken);

                    return true;
                }

                return false;

            }



            if (oper == "killExchange")
            {
                Storage.Put(Storage.CurrentContext, "Active", 0);
                if (Storage.Get(Storage.CurrentContext, "Active")[0] == 0)
                {
                    Storage.Delete(Storage.CurrentContext, "Active");
                }
                return true;
            }



            if (oper == "isOrderSigned")  //parameters :Address, msghash, v,r,s, signer
            {
                Runtime.Notify("Signature Verified");


                byte[] address = (byte[])args[0];
                byte _msgHash = (byte)args[1];
                uint v = (uint)args[2];
                byte r = (byte)args[3];
                byte s = (byte)args[4];
                byte[] _signer = (byte[])args[5];

                return Main("ecverify", address, _msgHash, v, r, s, _signer);

            }

            if (oper == "validExchangeFee")
            {
                byte[] _sellerFeeToken = (byte[])args[0];
                byte[] _buyerFeeToken = (byte[])args[1];
                BigInteger _sellerFeeValue = (BigInteger)args[2];
                BigInteger _buyrFeeValue = (BigInteger)args[3];

                if (_sellerFeeToken != null && _buyerFeeToken != null && _sellerFeeValue > 0 && _buyrFeeValue > 0)
                {
                    return true;
                }
                return false;
            }


            if (oper == "getFeeIndex")
            {
                byte[] _token = (byte[])args[0];

                byte[] baseToken = Storage.Get(Storage.CurrentContext, "BaseToken");
                byte[] etherToken = Storage.Get(Storage.CurrentContext, "EtherToken");
                if (_token.Length >=0)
                {
                    if (_token == baseToken)
                        return "0";
                    else if (_token == etherToken)
                        return "1";
                    return "2";
                }
                return false;
            }


            if (oper == "getTwoWayOrderHash")
            {
                byte[] _sellerTokens = (byte[])args[0];
                byte[] _buyerTokens = (byte[])args[1];
                BigInteger[] _sellerValues = (BigInteger[])args[2];
                BigInteger[] _buyerValues = (BigInteger[])args[3];
                byte[] _orderAddresses = new byte[5];
                _orderAddresses = (byte[])args[4];

                BigInteger[] _orderValues = new BigInteger[5];
                _orderValues = (BigInteger[])args[5];

                byte[] _orderID = (byte[])args[6];


                //sellerHash = Sha1(_sellerTokens.Concat(_sellerValues).Concat(_orderValues[3]).Concat(_orderValues[0]).Concat(_orderAddresses[3]).Concat(_orderAddresses[0]).Concat(_orderAddresses[1]).Concat(_orderID));
                //BuyerHash = Sha256(_buyerTokens, _buyerValues, _orderValues[4], _orderValues[1], _orderAddresses[4], _orderAddresses[0], _orderAddresses[2], _orderID);



                return true;
            }



            if (oper == "trasnferForTokens")
            {
                byte[] _sellerTokens = (byte[])args[0];
                byte[] _buyerTokens = (byte[])args[1];
                BigInteger[] _sellerValues = (BigInteger[])args[2];
                BigInteger[] _buyerValues = (BigInteger[])args[3];

                byte[] _orderAddresses = (byte[])args[4];


                BigInteger[] _orderValues = new BigInteger[5];
                _orderValues = (BigInteger[])args[5];

                int len = _sellerTokens.Length;

                for (uint i = 0; i < len; i++)
                {
                    Runtime.Log("In");
                    Main("transferFrom", _orderAddresses[1], _orderAddresses[2], _sellerValues[i]);
                }

                Runtime.Log("SellerValues Are Transferred Successfully");

                int len1 = _buyerTokens.Length;

                for (uint i = 0; i < len1; i++)
                {
                    Main("transferFrom", _orderAddresses[2], _orderAddresses[1], _buyerValues[i]);
                }

                byte[] wallet = Storage.Get(Storage.CurrentContext, "Wallet");

                Main("transferFrom", _orderAddresses[1], wallet, _orderValues[0]);
                Main("transferFrom", _orderAddresses[2], wallet, _orderValues[1]);


                return true;
            }



            if (oper == "transferFrom")
            {
                byte[] from = (byte[])args[0];
                byte[] to = (byte[])args[1];
                BigInteger value = (BigInteger)args[2];

                //if (value <= 0) return false;

                if (from == to) return true;

                Storage.Put(Storage.CurrentContext, from, 2000);

                BigInteger from_value =  Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, from));

                //if (from_value < value) return false;
                Runtime.Log("Transfer");

               

                //if (from_value > value)
                {
                    Storage.Put(Storage.CurrentContext, from, from_value - value);
                    BigInteger to_value = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, to));
                    Storage.Put(Storage.CurrentContext, to, to_value + value);
                }
            }

            if (oper == "basicSigValidations")
            {
               byte[] _orderAddresses = (byte[])args[0];

                
                uint[] _v = new uint[2];
                _v = (uint[])args[1];

               byte[] _sr = (byte[])args[2];
               byte[] _ss = (byte[])args[3];
               byte[] _br = (byte[])args[4];
               byte[] _bs = (byte[])args[5];
               byte[] _sellerHash = (byte[])args[6];
               byte[] _buyerHash = (byte[])args[7];
              

                if (!(Main("ecverify", _sellerHash, _v[0], _sr, _ss, _orderAddresses[1])).Equals(address0))
                {

                    return _orderAddresses[1];
                }

                if (!(Main("ecverify", _sellerHash, _v[1], _sr, _ss, _orderAddresses[2])).Equals(address0))
                {
                    return _orderAddresses[2];
                }
                return 0x0;


            }

            if(oper=="orderExists")
            {

                byte[] _hash=(byte[])args[0];
                byte[] _orderId = (byte[])args[0];

                if(Main("orderLocated", _hash, _orderId)!=null)
                {
                    return true;
                }

                return false;
                     
            }


            if(oper=="approve")
            {
                byte[] add1 = (byte[])args[0];
                byte[] add2 = (byte[])args[1];
                BigInteger value = (BigInteger)args[2];
                Storage.Put(Storage.CurrentContext, add1.Concat(add2), value);
                return true;
            }

            if(oper=="allowance")
            {
                byte[] add1 = (byte[])args[0];
                byte[] add2 = (byte[])args[1];
               return  Storage.Get(Storage.CurrentContext, add1.Concat(add2));

            }



            if(oper== "validateAuthorization")
            {
                byte[] _sellerTokens = (byte[])args[0];
                byte[] _buyerTokens = (byte[])args[1];
                BigInteger[] _sellerValues = (BigInteger[])args[2];
                BigInteger[] _buyerValues = (BigInteger[])args[3];

                byte[] _orderAddresses = (byte[])args[4];


                BigInteger[] _orderValues = new BigInteger[5];
                _orderValues = (BigInteger[])args[5];

                BigInteger allow = 0;// new BigInteger(( Main("allowance", _orderAddresses[2], address0)));

                if(allow <= _orderValues[1])
                {
                    return false;
                    
                }

               BigInteger allow1 = 0;// new BigInteger((Main("allowance", _orderAddresses[1], address0)));

                

                if (allow1 <= _orderValues[1])
                {
                    return false;

                }

                BigInteger allow2 = 0;// new BigInteger((Main("allowance", _orderAddresses[2], address0)));
                for (uint i = 0; i <= _buyerTokens.Length; i++)
                {
                    if (allow2 <= _buyerValues[i])
                    {
                        return false;

                    }
                }
                return true;

            }

            return false;
        }
    }
}


