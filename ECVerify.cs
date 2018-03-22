using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;

namespace ECVerify
{
    public class Contract1 : SmartContract
    {


        

        public static Object Main(string oper, params Object[] args)
        {


            switch (oper)
            {
                case "ecverify":  //parameters :Address, msghash, v,r,s, signer
                    {
                        Runtime.Notify("Signature Started Verification");
                        String address0 = (String)args[0];
                        byte _msgHash = (byte)args[1];
                        uint v = (uint)args[2];
                        byte r = (byte)args[3];
                        byte s = (byte)args[4];
                        String _signer = (String)args[5];
                        {
                            if (_signer == address0)
                            {
                                return false;
                            }


                            if (v < 27)
                            {
                                v += 27;
                            }

                            if (v != 27 && v != 28)
                            {

                                return address0 == _signer;
                            }

                            if (v == 27)
                            {
                                Runtime.Log("It Verified");
                                return ecrecover(_msgHash, v, r, s) == _signer;
                            }
                            else if (v == 28)
                            {
                                Runtime.Log("It Verified");
                                return ecrecover(_msgHash, v, r, s) == _signer;
                            }

                            return address0 == _signer;
                        }
                    }
            }
            Runtime.Log("Verification Failed");
            return false;


        }



        private static String ecrecover(byte msgHash, uint v, byte r, byte s)
        {
            String address1 = "ANiSxR6mgYZxhTXpJqzjcnyFCQ16HVaC4W";
            return address1;
        }
    }
}
