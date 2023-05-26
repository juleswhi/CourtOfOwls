namespace SharedClasses.Helpers;
public static class ParseExtensions
{
    public static T Parse<T>(this string input, IFormatProvider? formatProvider = null)
    where T : IParsable<T>
    {
        return T.Parse(input, formatProvider);
    }

    public static async Task<Player?> GetPlayer(this string username)
    {
        var player = await DataBaseClient.PlayerCollection.FindAsync(
            Builders<Player>.Filter.Eq(x => x.Username, username)
        );

        return await player.FirstOrDefaultAsync();
    }

    public static async Task<Bank?> GetBank(this string BankName) 
    {
        var bank = await DataBaseClient.BankCollection.FindAsync(
            Builders<Bank>.Filter.Eq(x => x.BankName, BankName)
        );

        return await bank.FirstOrDefaultAsync();
    }

    public static async Task<Account?> GetAccountId(this Guid AccountGuid)
    {
        var account = await DataBaseClient.AccountCollection.FindAsync(
            Builders<Account>.Filter.Eq(x => x.PlayerId, AccountGuid)
        );
        return await account.FirstOrDefaultAsync();
    }

    public static async Task<Account?> GetAccountPlayer(this Guid PlayerId)
    {
        var account = await DataBaseClient.AccountCollection.FindAsync(
            Builders<Account>.Filter.Eq(x => x.PlayerId, PlayerId)
        );
        return await account.FirstOrDefaultAsync();
    }
}