namespace ai
{
    using OpenAI;
    using OpenAI.Chat;
    using System.ClientModel;

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
            ChatClient client = new(
                model: model,
                credential: new ApiKeyCredential("ng-SVkJzaUjxN5tJCGTO493EpwRqoaGvuKR"),
                options: new OpenAIClientOptions()
                {
                    Endpoint = new Uri("https://api.naga.ac/v1"),
                }
            );
            return client;
        }
    }
}
