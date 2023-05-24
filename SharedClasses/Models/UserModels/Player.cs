
namespace SharedClasses.Models.UserModels;

public class Player
{
    public Player(Name name, string Password, string username)
    {
        this.Id = Guid.NewGuid();
        this.Username = username;
        this.Name = name;
        this.Password = Password;
        this.Accounts = new List<Guid>();
    }


    [BsonElement]
    public Guid Id { get; set; }
    [BsonElement]
    public string Username { get; set; }
    [BsonElement]
    public Name Name { get; set; }
    [BsonElement]
    public string Password { get; set; }
    [BsonElement]
    public List<Guid> Accounts { get; set; }



    public async Task<IEnumerable<Account>> GetPlayerAccounts()
    {
        var AccountCollection = DataBaseClient.Database.GetCollection<Account>("BankAccountData");

        var filter = Builders<Account>.Filter.Eq(x => x.PlayerId, Id);

        var query = await AccountCollection.FindAsync(filter);

        if (query.ToEnumerable() == Enumerable.Empty<Account>()) return Enumerable.Empty<Account>();

        List<Account> accounts = new List<Account>();


        foreach (var result in query.ToList<Account>())
        {
            accounts.Add(result);
        }

        return accounts;
    }


    public static async Task<Code> CreatePlayer(Name _name, string _password, string _username)
    {
        if (
            ((Player)DataBaseClient.PlayerCollection.Find(
                Builders<Player>.Filter.Eq(x => x.Username, _username)
            ).FirstOrDefault()) is not null
        )
        {
            return Code.ExistingAccount;
        }

        var player = new Player(_name, _password, _username);

        await DataBaseClient.PlayerCollection.InsertOneAsync(player);

        return Code.Ok;
    }


}