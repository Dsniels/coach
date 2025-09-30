using Microsoft.Extensions.Configuration;

public class Settings
{
    public IConfigurationRoot _config { get; set; }
    public T GetSettings<T>() => this._config.GetRequiredSection(typeof(T).Name).Get<T>();
    public OpenAISettings openAISettings => this.GetSettings<OpenAISettings>();
    public TokenSettings tokenSettings => this.GetSettings<TokenSettings>();

    public class OpenAISettings
    {
        public string Endpoint { get; set; }
        public string Model { get; set; }
        public string ApiKey { get; set; }
    }

    public class TokenSettings
    {
        public string Issuer { get; set; }
        public string Key { get; set; }
    }

    public Settings()
    {
        this._config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                   .AddJsonFile("appsettings.json", false)
                                   .AddEnvironmentVariables().Build();
    }

}