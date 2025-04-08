using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Diagnostics;

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
                Style = (Style)FindResource("AIMessageStyle"),
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
            if (textBox != null)
            {
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

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
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
                        var result = _blenderIntegration.GenerateModelAsync(message).Result;
                        AddAIMessage($"Model başarıyla oluşturuldu: {result}");
                    }
                    catch (Exception ex)
                    {
                        AddAIMessage($"Hata: {ex.Message}");
                    }
                }
            }
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !e.IsRepeat)
            {
                e.Handled = true;
                SendMessageButton_Click(null, null);
            }
        }

        private void AddUserMessage(string message)
        {
            var userMessage = new Border
            {
                Style = (Style)FindResource("UserMessageStyle"),
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
                Style = (Style)FindResource("AIMessageStyle"),
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
            }
        }
    }

    public class BlenderIntegration
    {
        public string BlenderPath { get; set; }

        public async Task<string> GenerateModelAsync(string description)
        {
            if (string.IsNullOrEmpty(BlenderPath))
            {
                throw new InvalidOperationException("Blender yolu belirtilmemiş");
            }

            try
            {
                // Blender'ı başlat
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = BlenderPath,
                        Arguments = "--background --python-expr 'import bpy; bpy.ops.wm.save_as_mainfile(filepath=\"output.blend\")'",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    var error = await process.StandardError.ReadToEndAsync();
                    throw new Exception($"Blender hatası: {error}");
                }

                return "output.blend";
            }
            catch (Exception ex)
            {
                throw new Exception($"Model oluşturulurken hata oluştu: {ex.Message}", ex);
            }
        }
    }
}
