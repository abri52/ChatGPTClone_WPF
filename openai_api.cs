namespace ai
{
    using Microsoft.Extensions.Configuration;
    using OpenAI;
    using OpenAI.Chat;
    using System.ClientModel;
    using System.Reflection;
    using System.Windows;

    /// <summary>
    /// Defines the <see cref="openai_api" />
    /// </summary>
    public class openai_api
    {
        /// <summary>
        /// The CreateChat
        /// </summary>
        /// <returns>The <see cref="Task{ChatClient}"/></returns>
        public static ChatClient CreateClient(string model)
        {
            var builder = new ConfigurationBuilder();
            builder.AddUserSecrets(Assembly.GetExecutingAssembly());
            IConfiguration Configuration = builder.Build();

            ChatClient client = new(
                model: model,
                credential: new ApiKeyCredential(Configuration["openai_key"]),
                options: new OpenAIClientOptions()
                {
                    Endpoint = new Uri("https://api.naga.ac/v1"),
                }
            );
            return client;
        }
    }
}
