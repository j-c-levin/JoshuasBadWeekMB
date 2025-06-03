using System;
using System.IO;

try
{
    // Write startup log
    File.WriteAllText("startup.log", $"Starting game at {DateTime.Now}\n");
    
    using var game = new joshuas_bad_week.Game1();
    game.Run();
    
    // Write success log
    File.AppendAllText("startup.log", $"Game exited normally at {DateTime.Now}\n");
}
catch (Exception ex)
{
    // Write error log
    string errorMessage = $"Error at {DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n";
    File.WriteAllText("error.log", errorMessage);
    
    // Also try to show a message box on Windows
    try
    {
        System.Windows.Forms.MessageBox.Show($"Game failed to start:\n{ex.Message}", "Error", 
            System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
    }
    catch
    {
        // If MessageBox fails, just write to console
        Console.WriteLine($"Error: {ex.Message}");
    }
}
