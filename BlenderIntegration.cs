using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AI_3DGen
{
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