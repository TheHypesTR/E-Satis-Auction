namespace e_Sat_Auction.Common.Helpers;

public static class TCNoHelper
{
    public static bool IsValid(string? tcNumber)
    {
        if (string.IsNullOrWhiteSpace(tcNumber) ||
            tcNumber.Length is not 11 ||
            !tcNumber.All(char.IsDigit) ||
            tcNumber[0] is '0' ||
            (tcNumber[10] - '0') % 2 is not 0)
        {
            return false;
        }

        int sumOdd = 0;
        int sumEven = 0;
        int sumFirst10Digits = 0;

        for (int i = 0; i < 9; i++)
        {
            int digit = tcNumber[i] - '0';
            sumFirst10Digits += digit;
            if (i % 2 == 0)
            {
                sumOdd += digit;
            }
            else
            {
                sumEven += digit;
            }
        }

        int digit10 = ((sumOdd * 7) - sumEven) % 10;
        if (digit10 != (tcNumber[9] - '0'))
        {
            return false;
        }

        sumFirst10Digits += digit10;
        if (sumFirst10Digits % 10 != tcNumber[10] - '0')
        {
            return false;
        }

        return true;
    }
}