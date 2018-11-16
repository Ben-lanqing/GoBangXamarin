using System;
using System.Collections.Generic;
using System.Text;

namespace GoBangCL.Standard.Model
{
    public enum ColourEnum
    {
        Empty = 0,
        Black = 1,
        White = 2,
        Out = 9
    }
    public enum StateEnum
    {
        Win = 0,
        WVCF = 1,
        A = 2,
        D = 3,
        WAVCF = 4,
        Loss = 5,
        NVCF = 9,
    }
    public enum LevelEnum
    {
        Five_1 = 1,
        LF_2 = 2,
        FF_3 = 3,
        FTr_4 = 4,
        VCF_5 = 5,
        TrTr_6 = 6,
        FToTo_8 = 8,
        ToToTo_9 = 9,
        FTo_10 = 10,
        LT_11 = 11,
        ToTo_12 = 12,
        SF_13 = 13,
        STrSTo_14 = 14,
        E_99 = 99,
        LL_9999 = 9999,
        Loss_99999 = 99999,

    }
    public enum ProcessEnum
    {
        起始,
        己无VCF,
        敌无VCF
    }
}
