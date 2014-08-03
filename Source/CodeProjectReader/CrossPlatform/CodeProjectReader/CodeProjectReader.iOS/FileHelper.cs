using System;
using System.IO;
using System.Threading.Tasks;
using CodeProjectReader.iOS;
using Xamarin.Forms;

[assembly:Dependency(typeof(FileHelper))]
namespace CodeProjectReader.iOS
{
    public class FileHelper : IFileHelper
    {
        private string _appFolder;
        public string AppFolder
        {
            get
            {
                return string.IsNullOrWhiteSpace(_appFolder)
                    ? (_appFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal))
                    : _appFolder;
            }
        }

        public async Task SaveToFile(string fileName, string text)
        {
            var filePath = Path.Combine(AppFolder, fileName);
            await Task.Run(() => File.WriteAllText(filePath, text));
        }

        public async Task<string> LoadFile(string fileName)
        {
            var filePath = Path.Combine(AppFolder, fileName);
            return await Task.Run(() => File.ReadAllText(filePath));
        }
    }
}