using LoginService;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Services.UserService;

public class userService
{
    private readonly IMongoCollection<LoginModel> userCollection;

    public userService(
        IOptions<DatabaseSettings> DatabaseSettings)
    {
        var mongoClient = new MongoClient(
            DatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            DatabaseSettings.Value.DatabaseName);

        userCollection = mongoDatabase.GetCollection<LoginModel>(
            DatabaseSettings.Value.UserCollectionName);
    }

    public  LoginModel Login(LoginModel login) {
            var login_try_username = userCollection.Find<LoginModel>(login1 => login.Username == login1.Username ).Any();
            var login_try_password = userCollection.Find<LoginModel>(login1 => login.Password == login1.Password ).Any();
            if (login_try_password == false | login_try_username == false){
                return null;
            }
            return userCollection.Find<LoginModel>(login1 => login.Username == login1.Username && login.Password == login1.Password ).FirstOrDefault();
    }
    
    public List<LoginModel> getAllUsers() {

        return userCollection.Find(_ => true).ToList();
    }
       

    public async Task<LoginModel?> GetAsync(string id) =>
        await userCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(LoginModel newUser)
    {
            await userCollection.InsertOneAsync(newUser);
    }
        

    public async Task UpdateAsync(string id, LoginModel updatedUser) =>
        await userCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

    public async Task RemoveAsync(string id) {
         await userCollection.DeleteOneAsync(x => x.Id == id);
    }
       
}