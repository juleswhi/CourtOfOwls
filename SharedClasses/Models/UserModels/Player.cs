using SharedClasses.Models.UserModels;
namespace SharedClasses.Models.UserModels;

public class Player
{
    public Player(Name name, string Password, string username, Guid CountryId, string colour, List<Guid> countryIds)
    {
        this.Colour = colour;
        this.CountryIds = countryIds;
        this.CountryId = CountryId;
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
    [BsonElement]
    public Guid CountryId { get; set; }
    [BsonElement]
    public List<Guid> CountryIds { get; set; }
    [BsonElement]
    public string Colour { get; set; }


    public List<LegionMarker> GetMarkers() {
        var players = DataBaseClient.PlayerCollection.Find(_ => true);

        


        List<LegionMarker> markers = new();
        return markers; 
    }






    public async Task<Code> JoinAlliance(string allianceName, Player player)
    {
        var alliance = await DataBaseClient.AllianceCollection.FindAsync(
            Builders<Alliance>.Filter.Eq(x => x.Name, allianceName)
        );
        if(this is null) Console.WriteLine("This is null");
        if(alliance is null) return Code.AccountNotFound;

        Alliance targetAlliance = await alliance.FirstOrDefaultAsync();

        if(targetAlliance is null) return Code.AccountNotFound;

        return await targetAlliance.AddAllianceMember(player);
    }



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




    public static async Task<Code> CreatePlayer(Name _name, string _password, string _username, string CountryName)
    {
        var filter = Builders<Country>.Filter.Eq(x => x.Name, CountryName);

        var country = DataBaseClient.CountryCollection.Find(filter).FirstOrDefault();

        if(country is null) return Code.AlreadyOccupied;

        if (
            ((Player)DataBaseClient.PlayerCollection.Find(
                Builders<Player>.Filter.Eq(x => x.Username, _username)
            ).FirstOrDefault()) is not null
        )
        {
            return Code.UsernameTaken;
        }

        List<Guid> CountryGuids = new();

        CountryGuids.Add(country.Id);

        string hashPassword = _password.Hash(ParseExtensions.salt);

        string colour = String.Format("#{0:X6}", new Random((int)DateTime.Now.Ticks).Next(0x1000000));

        var player = new Player(_name, hashPassword, _username, country.Id, colour, CountryGuids);

        await DataBaseClient.PlayerCollection.InsertOneAsync(player);

        return Code.Ok;
    }

    


}
