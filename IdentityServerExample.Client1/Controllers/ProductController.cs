using IdentityModel.Client;
using IdentityServerExample.Client1.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace IdentityServerExample.Client1.Controllers;

public class ProductController : Controller
{
    private readonly IConfiguration configuration;
    private  HttpClient client;

    public ProductController(IConfiguration configuration)
    {
        this.configuration = configuration;
        client = new HttpClient();
    }
    public async Task< IActionResult> Index()
    {
        DiscoveryDocumentResponse discovery = await client.GetDiscoveryDocumentAsync("https://localhost:7009");
        ClientCredentialsTokenRequest clientCredentialsTokenRequest= new ClientCredentialsTokenRequest() { ClientId = configuration["Client:ClientId"], ClientSecret = configuration["Client:ClientSecret"],Address=discovery.TokenEndpoint };
        TokenResponse token= await client.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest);
        client.SetBearerToken(token.AccessToken);
        HttpResponseMessage response = await client.GetAsync("https://localhost:7010/api/products/getproducts");
        if (response.IsSuccessStatusCode)
        {
            var content= await response.Content.ReadAsStringAsync();
            List<Product> products = new();
             products = JsonConvert.DeserializeObject<List<Product>>(content);

            return View(products);
        }
        else
        {
//loglama
        }

        return View();
    }
}
