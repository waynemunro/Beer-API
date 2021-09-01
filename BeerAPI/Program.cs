using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<BeerRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwaggerUI();

app.UseSwagger(x => x.SerializeAsV2 = true);

app.MapGet("/hello", ([FromServices] string get) => "Hello there!"); 

app.MapGet("/beers", async ([FromServices] BeerRepository repo) => await repo.GetAll());
app.MapGet("/beer/{name}", async ([FromServices] BeerRepository repo, string name) => await repo.Get(name));

app.Run();

class BeerRepository
{
    public async Task<IEnumerable<Beer>> GetAll()
    {
        using var httpClient = new HttpClient();
        Uri url = new Uri("https://raw.githubusercontent.com/SpiritChrusher/FavoriteBeer/master/src/main/Allbeers.json");
        Stream allstring = await httpClient.GetStreamAsync(url);
        List<Beer> BeerList = await JsonSerializer.DeserializeAsync<List<Beer>>(allstring);
        return BeerList;
    }

    public async Task<Beer> Get(string name) {

        using var httpClient = new HttpClient();
        Uri url = new Uri("https://raw.githubusercontent.com/SpiritChrusher/FavoriteBeer/master/src/main/Allbeers.json");
        Stream allstring = await httpClient.GetStreamAsync(url);
        List<Beer> BeerList = await JsonSerializer.DeserializeAsync<List<Beer>>(allstring);

        return BeerList.Where(x => x.name.Contains(name)).FirstOrDefault();
    }
}

record Beer(
    string name,
    decimal alcohol,
    IEnumerable<string> taste,
    string origin,
    IEnumerable<string> type,
    string manufacturer,
    string consumption,
    int price,
    decimal quality,
    IEnumerable<string> acquisition,
    decimal packFormat
    );