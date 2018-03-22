using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace TradeFeeCalculator
{
    public class Contract1 : SmartContract
    {
        private static BigInteger BigInteger;

        public static Object Main(string oper, params Object[] args)
        {
           


            switch (oper)
            {
                case "updateFeeSchedule":
                    {
                        uint baseTokenFee = (uint)args[0];
                        uint etherTokenFee = (uint)args[1];
                        uint normalTokenFee = (uint)args[2];

                        if (baseTokenFee >= 0 && baseTokenFee <= 1 && etherTokenFee >= 0 && etherTokenFee <= 1 && normalTokenFee >= 0 )
                        {
                            Storage.Put(Storage.CurrentContext, "0", baseTokenFee);
                            Storage.Put(Storage.CurrentContext, "1", etherTokenFee);
                            Storage.Put(Storage.CurrentContext, "2", normalTokenFee);
                           
                        }
                        return true;
                    }


                case "calcTradeFee":
                    {
                        BigInteger values = (BigInteger)args[0];
                        uint feeIndex = (uint)args[1];
                        byte[] check;

                        if (feeIndex >= 0 && feeIndex <= 2 && values > 0)
                        {
                            if (feeIndex == 0)
                                check = Storage.Get(Storage.CurrentContext,"0");
                            else if(feeIndex == 1)
                                check = Storage.Get(Storage.CurrentContext, "1");
                            else
                                check = Storage.Get(Storage.CurrentContext, "2");


                            BigInteger I = new BigInteger(check);
                            BigInteger totalFees = (I*values);///(1 NEO);

                            if (totalFees > 0)
                            {
                               
                                return totalFees;
                            }


                        }

                        return true;

                    }

                case "calcTradeFeeMulti":
                    {
                        uint[] values = (uint[])args[0];
                        uint[] feeIndexes = (uint[])args[1];

                        if (values.Length > 0 && feeIndexes.Length > 0 && values.Length == feeIndexes.Length)
                        {

                           BigInteger [] totalFees = new BigInteger[values.Length];
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

                    


            }
            return false;

        }
    }
}

