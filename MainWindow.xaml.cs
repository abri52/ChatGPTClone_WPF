namespace ai
{
    using ai.Converters;
    using OpenAI.Chat;
    using System.ClientModel;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Defines the uiMessages
        /// </summary>
        private ObservableCollection<Message> uiMessages;

        /// <summary>
        /// Defines the apiMessages
        /// </summary>
        private List<ChatMessage> apiMessages = [
                new UserChatMessage("Язык по умолчанию: русский")
            ];

        /// <summary>
        /// Defines the client
        /// </summary>
        private ChatClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            uiMessages = new ObservableCollection<Message>();
            messagesItemsControl.ItemsSource = uiMessages;
        }

        /// <summary>
        /// The CopyContentsAction
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void CopyContentsAction(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Message message)
            {
                Clipboard.SetText(message.Text ?? "");
            }
        }

        /// <summary>
        /// The ReportMessageAction
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void ReportMessageAction(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// The sendMessage
        /// </summary>
        /// <param name="e">The e<see cref="KeyEventArgs?"/></param>
        private async void sendMessage(KeyEventArgs? e = null)
        {
            var request = new Message()
            {
                IsRequest = true,
                Text = messageTextBox.Text,
            };
            uiMessages.Add(request);
            apiMessages.Add(new UserChatMessage(request.Text));

            messageTextBox.Text = "";
            if (e != null)
            {
                e.Handled = true;
            }
            if (client == null)
            {
                client = openai_api.CreateClient("gpt-4o");
            }

            var responseFinal = "";

            var response = new Message()
            {
                IsRequest = false,
                Text = responseFinal
            };

            uiMessages.Add(response);

            try
            {
                AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = client.CompleteChatStreamingAsync(apiMessages);
                await foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
                {
                    if (completionUpdate.ContentUpdate.Count > 0)
                    {
                        responseFinal += completionUpdate.ContentUpdate[0].Text;
                        response.Text = responseFinal;

                        uiMessages.Remove(response);
                        uiMessages.Add(response);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Text = ex.ToString();

                uiMessages.Remove(response);
                uiMessages.Add(response);
            }

            //await Task.Delay(2000);

            //var response = new Message()
            //{
            //    IsRequest = false,
            //    Text = $"ChatGPT Response:\nLorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas vitae diam non risus dignissim suscipit. Aliquam posuere cursus tellus non sagittis. Duis non augue vel dui fermentum placerat. Suspendisse porttitor nunc ut dapibus lacinia."
            //};

            //uiMessages.Add(response);
            messagesScrollViewer.ScrollToEnd();
        }

        /// <summary>
        /// The messageTextBox_PreviewKeyDown
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="KeyEventArgs"/></param>
        private async void messageTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (string.IsNullOrWhiteSpace(messageTextBox.Text))
                        e.Handled = true;

                    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    {
                        Debug.WriteLine("shift+enter key");

                        messageTextBox.Text = $"{messageTextBox.Text}\n";
                        messageTextBox.CaretIndex = messageTextBox.Text.Length;

                        e.Handled = true;
                        return;
                    }

                    sendMessage(e);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// The sendButton_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private async void sendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(messageTextBox.Text))
                    return;

                sendMessage();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// The messageTextBox_TextChanged
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="System.Windows.Controls.TextChangedEventArgs"/></param>
        private void messageTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        }
    }

    /// <summary>
    /// Defines the <see cref="Message" />
    /// </summary>
    public class Message : INotifyPropertyChanged
    {
        /// <summary>
        /// Defines the PropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Defines the _isRequest
        /// </summary>
        private bool _isRequest = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsRequest
        /// </summary>
        public bool IsRequest
        {
            get { return _isRequest; }
            set { _isRequest = value; }
        }

        /// <summary>
        /// Gets or sets the Text
        /// </summary>
        public required string Text { get; set; }

        /// <summary>
        /// The ToString
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// The OnPropertyChanged
        /// </summary>
        /// <param name="propertyName">The propertyName<see cref="string"/></param>
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
