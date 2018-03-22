using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;


namespace Dex1WaySig
{
    public class Contract1 : SmartContract
    {
        public static Object Main(string oper, params Object[] args)
        {

            string version = "0.1";
            bool activated;
            string wallet;
            string baseToken = null;
            string etherToken = null;
            string activeVaultAddr;
            string histroyVaults;
            string feeCalculatorAddr;
            string approver;
            string sellerHash = null, BuyerHash = null;


            switch (oper)
            {
                case "Dex1Wa":
                    {
                        string _vault = (string)args[0];
                        string _feeCalculator = (string)args[1];
                        string _baseToken = (string)args[2];
                        string _etherToken = (string)args[3];
                        string _verifierAddr = (string)args[4];

                        if (_vault != null && _feeCalculator != null && _verifierAddr != null && _baseToken != null && _etherToken != null)
                        {
                            approver = "msg.sender";
                            wallet = approver;
                            baseToken = _baseToken;
                            etherToken = _etherToken;
                            feeCalculatorAddr = _feeCalculator;
                            activeVaultAddr = _vault;

                        }

                        return true;

                    }

                case "killExchange":
                    {
                        activated = false;
                        break;
                    }

                case "updateExConfig":
                    {
                        string _wallet = (string)args[0];
                        string _verifierAddr = (string)args[1];
                        string _feeCalculator = (string)args[2];
                        if (_wallet != null && _verifierAddr != null && _feeCalculator != null)
                        {
                            wallet = _wallet;
                            feeCalculatorAddr = _feeCalculator;
                        }
                        return true;
                    }

                case "isOrderSigned":  //parameters :Address, msghash, v,r,s, signer
                    {
                        Runtime.Notify("Signature Verified");


                        String address = (String)args[0];
                        string _msgHash = (string)args[1];
                        uint v = (uint)args[2];
                        string r = (string)args[3];
                        string s = (string)args[4];
                        String _signer = (String)args[3];
                        {
                            if (_signer == address)
                            {

                                return false;
                            }

                            // return case "ecrecovery": == _signer;

                            if (v < 27)
                            {
                                v += 27;
                            }

                            if (v != 27 && v != 28)
                            {

                                return address == _signer;
                            }

                            if (v == 27)
                            {
                                Runtime.Log("It Reaches");
                                return ecrecover(_msgHash, v, r, s) == _signer;
                            }
                            else if (v == 28)
                            {
                                return ecrecover(_msgHash, v, r, s) == _signer;
                            }

                            return address == _signer;
                        }
                    }


                case "validExchangeFee":
                    {
                        string _sellerFeeToken = (string)args[0];
                        string _buyerFeeToken = (string)args[1];
                        uint _sellerFeeValue = (uint)args[2];
                        uint _buyrFeeValue = (uint)args[3];

                        if (_sellerFeeToken != null && _buyerFeeToken != null && _sellerFeeValue > 0 && _buyrFeeValue > 0)
                        {
                            return true;
                        }
                        return false;
                    }

                case "getFeeIndex":
                    {
                        string _token = (string)args[0];

                        if (_token != null)
                        {
                            if (_token == baseToken)
                                return 0;
                            else if (_token == etherToken)
                                return 1;
                            return 2;
                        }
                        return false;
                    }
            }

            if (oper == "getTwoWayOrderHash")
            {
                string[] _sellerTokens = (string[])args[0];
                string[] _buyerTokens = (string[])args[1];
                uint[] _sellerValues = (uint[])args[2];
                uint[] _buyerValues = (uint[])args[3];

                string[] _orderAddresses = new string[5];
                _orderAddresses = (string[])args[4];




                uint[] _orderValues = new uint[5];
                _orderValues = (uint[])args[5];

                byte _orderID = (byte)args[6];


                // sellerHash = VerifySignatures(_sellerTokens, _sellerValues, _orderValues[3], _orderValues[0], _orderAddresses[3], _orderAddresses[0], _orderAddresses[1], _orderID);
                //BuyerHash = Hash256(_buyerTokens, _buyerValues, _orderValues[4], _orderValues[1], _orderAddresses[4], _orderAddresses[0], _orderAddresses[2], _orderID);


                return true;
            }

            if (oper == "trasnferTokens")
            {
                string[] _sellerTokens = (string[])args[0];
                string[] _buyerTokens = (string[])args[1];
                uint[] _sellerValues = (uint[])args[2];
                uint[] _buyerValues = (uint[])args[3];
                string[] _orderAddresses = new string[5];
                _orderAddresses = (string[])args[4];
                uint[] _orderValues = (uint[])args[5];

                int len = _sellerTokens.Length;

                for (uint i = 0; i < len; i++)
                {
                    //transferFrom(_orderAddresses[1], _orderAddresses[2], _sellerValues[i]);
                }

                //---------------------------------------------

                return true;
            }

            if (oper == "basicSigValidations")
            {
                string[] _orderAddresses = new string[5];
                _orderAddresses = (string[])args[0];

                uint[] _v = new uint[2];
                _v = (uint[])args[1];

                string _sr = (string)args[2];
                string _ss = (string)args[3];
                string _br = (string)args[4];
                string _bs = (string)args[5];
                string _sellerHash = (string)args[6];
                string _buyerHash = (string)args[7];
                string address = "address0";

                if (!(Main("isOrderSigned", address, sellerHash, _v[0], _sr, _ss, _orderAddresses[1])).Equals(address))
                {

                    return _orderAddresses[1];
                }

                if (!(Main("isOrderSigned", address, sellerHash, _v[1], _sr, _ss, _orderAddresses[2])).Equals(address))
                {
                    return _orderAddresses[2];
                }
                return 0x0;


            }

            return false;
        }


        
        private static String ecrecover(string msgHash, uint v, string r, string s)
        {
            String address1 = "ANiSxR6mgYZxhTXpJqzjcnyFCQ16HVaC4W";
            return address1;
        }
    }
}
