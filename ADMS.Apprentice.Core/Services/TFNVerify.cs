namespace ADMS.Apprentice.Core.Services
{
    public class TFNVerify : ITFNVerify
    {
        private readonly int[] checksumMultipliers = { 1, 4, 3, 7, 5, 8, 6, 9, 10 };

        public bool MatchesChecksum(string tfn)
        {
            if (tfn?.Length != 9)
                return false;

            int checksum = 0;
            for (int i = 0; i < 9; i++)
            {
                checksum += checksumMultipliers[i] * int.Parse(tfn.Substring(i, 1));
            }
            return checksum % 11 == 0;
        }
    }
}