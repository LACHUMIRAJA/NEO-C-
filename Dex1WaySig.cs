using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace Dex1WaySig
{
    public class Contract1 : SmartContract
    {
        public static Object Main(string oper, params Object[] args)
        {
           

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
                if (_token.Length >= 0)
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
                byte[][] _orderAddresses = new byte[5][];
                _orderAddresses = (byte[][])args[4];

                BigInteger[] _orderValues = new BigInteger[5];
                _orderValues = (BigInteger[])args[5];

                byte[] _orderID = (byte[])args[6];


                //sellerHash = Sha1(_sellerTokens.Concat(_sellerValues).Concat(_orderValues[3]).Concat(_orderValues[0]).Concat(_orderAddresses[3]).Concat(_orderAddresses[0]).Concat(_orderAddresses[1]).Concat(_orderID));
                //BuyerHash = Sha256(_buyerTokens, _buyerValues, _orderValues[4], _orderValues[1], _orderAddresses[4], _orderAddresses[0], _orderAddresses[2], _orderID);



                return true;
            }


            if (oper == "trasnferForTokens") //[["sellerT","sellerT1"],["buyerT","buyerT1"],[500,450],[458,444],["AA1","BB1","CC1"],[540,450,580]]
            {
                byte[][] _sellerTokens = (byte[][])args[0];
                byte[][] _buyerTokens = (byte[][])args[1];
                BigInteger[] _sellerValues = (BigInteger[])args[2];
                BigInteger[] _buyerValues = (BigInteger[])args[3];

                byte[][] _orderAddresses = new byte[5][];
                _orderAddresses = (byte[][])args[4];


                BigInteger[] _orderValues = new BigInteger[5];
                _orderValues = (BigInteger[])args[5];

                int len = _sellerTokens.Length;
                Runtime.Notify(_orderAddresses[1], _sellerTokens.Length);
                for (uint i = 0; i < len; i++)
                {
                    Runtime.Log("In");
                    Runtime.Notify(_orderAddresses[1], _orderAddresses[2], _sellerValues[0]);
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


            if (oper == "deposit")
            {
                byte[] acc = (byte[])args[0];
                BigInteger val = (BigInteger)args[1];
                Runtime.Log("Deposited Successfully Balance -");
                BigInteger balance = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, acc));
                BigInteger total = balance + val;
                Storage.Put(Storage.CurrentContext, acc, total);
                Runtime.Notify(total);
            }

            if (oper == "balanceOf")
            {
                byte[] acc = (byte[])args[0];
                BigInteger balance = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, acc));
                
                Runtime.Log("Balance - ");                
                Runtime.Notify(balance+0);

                return Storage.Get(Storage.CurrentContext, acc);
            }


            if (oper == "transferFrom")
            {
                byte[] from = (byte[])args[0];
                byte[] to = (byte[])args[1];
                BigInteger value = (BigInteger)args[2];

                BigInteger from_value = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, from));

                if (value <= 0 || from_value < value) {

                    Runtime.Log("Insufficient Balance... If You Want to Deposit please use Deposit Function..");
                    return false;
                }

                if (from == to) return true;


                //BigInteger from_value = 500000;// if you need to test choose this
              

               
                Runtime.Log("Transfer Process ");



                if (from_value > value)
                {
                    BigInteger bal = from_value - value;
                    Runtime.Notify(from);
                    Runtime.Log("Balance: ");
                    Runtime.Notify(bal);
                    Storage.Put(Storage.CurrentContext, from, bal);
                    BigInteger to_value = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, to));

                    BigInteger bal1 = to_value + value;
                    Storage.Put(Storage.CurrentContext, to, bal1);
                    Runtime.Notify(to);
                    Runtime.Log("Balance: ");
                    Runtime.Notify(bal1);
                    Runtime.Log("-------------------");
                    return true;
                }


            }

            if (oper == "basicSigValidations")
            {
                byte[][] _orderAddresses = new byte[5][];
                _orderAddresses = (byte[][])args[0];


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

            if (oper == "orderExists")
            {

                byte[] _hash = (byte[])args[0];
                byte[] _orderId = (byte[])args[0];

                if (Main("orderLocated", _hash, _orderId) != null)
                {
                    return true;
                }

                return false;

            }


            if (oper == "approve")
            {
                byte[] add1 = (byte[])args[0];
                byte[] add2 = (byte[])args[1];
                BigInteger value = (BigInteger)args[2];
                Storage.Put(Storage.CurrentContext, add1.Concat(add2), value);
                return true;
            }

            if (oper == "allowance")
            {
                byte[] add1 = (byte[])args[0];
                byte[] add2 = (byte[])args[1];
                return Storage.Get(Storage.CurrentContext, add1.Concat(add2));

            }



            if (oper == "validateAuthorization")
            {
                byte[] _sellerTokens = (byte[])args[0];
                byte[] _buyerTokens = (byte[])args[1];
                BigInteger[] _sellerValues = (BigInteger[])args[2];
                BigInteger[] _buyerValues = (BigInteger[])args[3];

                byte[] _orderAddresses = (byte[])args[4];


                BigInteger[] _orderValues = new BigInteger[5];
                _orderValues = (BigInteger[])args[5];

                BigInteger allow = 0;// new BigInteger(( Main("allowance", _orderAddresses[2], address0)));

                if (allow <= _orderValues[1])
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
    


