using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AI_3DGen
{
    public partial class MainWindow : Window
    {
        private readonly BlenderIntegration _blenderIntegration;
        private ScrollViewer? _scrollViewer;

        public MainWindow()
        {
            InitializeComponent();
            _blenderIntegration = new BlenderIntegration();
            
            // İlk mesajı ekle
            AddInitialMessage();

            // ScrollViewer referansını al
            _scrollViewer = FindName("ChatScrollViewer") as ScrollViewer;
            if (_scrollViewer == null)
            {
                throw new InvalidOperationException("ScrollViewer bulunamadı");
            }
        }

        private void AddInitialMessage()
        {
            var initialMessage = new Border
            {
                Style = (Style)FindResource("AIMessageStyle") ?? throw new InvalidOperationException("AIMessageStyle bulunamadı"),
                Margin = new Thickness(0, 0, 0, 20),
                Child = new TextBlock
                {
                    Text = "Merhaba! Ben AI 3DGen. Size nasıl yardımcı olabilirim?",
                    Margin = new Thickness(15),
                    FontSize = 14,
                    Foreground = Brushes.White
                }
            };

            ChatMessages.Children.Add(initialMessage);
        }

        private void InputTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "Model açıklamasını girin...")
            {
                textBox.Text = "";
                textBox.SelectAll();
            }
        }

        private void InputTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Model açıklamasını girin...";
            }
        }

        private async void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            var message = InputTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(message) && message != "Model açıklamasını girin...")
            {
                AddUserMessage(message);
                InputTextBox.Text = "Model açıklamasını girin...";
                
                if (string.IsNullOrEmpty(_blenderIntegration.BlenderPath))
                {
                    AddAIMessage("Lütfen Blender yolu seçin.");
                }
                else
                {
                    AddAIMessage("Model oluşturuluyor. Lütfen bekleyin...");
                    try
                    {
                        var result = await _blenderIntegration.GenerateModelAsync(message);
                        AddAIMessage($"Model başarıyla oluşturuldu: {result}");
                    }
                    catch (Exception ex)
                    {
                        AddAIMessage($"Hata: {ex.Message}");
                    }
                }
            }
        }

        private async void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !e.IsRepeat)
            {
                e.Handled = true;
                await Task.Run(() => 
                {
                    var dummyEventArgs = new RoutedEventArgs();
                    SendMessageButton_Click(this, dummyEventArgs);
                });
            }
        }

        private void AddUserMessage(string message)
        {
            var userMessage = new Border
            {
                Style = (Style)FindResource("UserMessageStyle") ?? throw new InvalidOperationException("UserMessageStyle bulunamadı"),
                Child = new TextBlock
                {
                    Text = message,
                    Margin = new Thickness(15),
                    FontSize = 14,
                    Foreground = Brushes.White
                }
            };

            ChatMessages.Children.Add(userMessage);
            _scrollViewer?.ScrollToBottom();
        }

        private void AddAIMessage(string message)
        {
            var aiMessage = new Border
            {
                Style = (Style)FindResource("AIMessageStyle") ?? throw new InvalidOperationException("AIMessageStyle bulunamadı"),
                Child = new TextBlock
                {
                    Text = message,
                    Margin = new Thickness(15),
                    FontSize = 14,
                    Foreground = Brushes.White
                }
            };

            ChatMessages.Children.Add(aiMessage);
            _scrollViewer?.ScrollToBottom();
        }

        private void BlenderSelect_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Blender Seç",
                Filter = "Blender Executable|blender.exe",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _blenderIntegration.BlenderPath = openFileDialog.FileName;
                AddAIMessage($"Blender yolu seçildi: {openFileDialog.FileName}");
            }
            else
            {
                AddAIMessage("Blender seçimi iptal edildi.");
            }
        }
    }
}
