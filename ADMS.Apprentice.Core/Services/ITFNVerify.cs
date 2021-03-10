namespace ADMS.Apprentice.Core.Services
{
    public interface ITFNVerify
    {
        bool MatchesChecksum(string tfn);
    }
}