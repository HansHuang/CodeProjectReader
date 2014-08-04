using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var filePath = GetFilePath(fileName);
            await Task.Run(() => File.WriteAllText(filePath, text));
        }

        public async Task SaveToFile(string fileName, Stream stream)
        {
            var filePath = GetFilePath(fileName);
            using (var file = File.Create(filePath))
            {
                await stream.CopyToAsync(file);
            }
#if DEBUG
            var files = Directory.GetFiles(Path.GetDirectoryName(filePath));
            foreach (var s in files)
            {
                System.Diagnostics.Debug.WriteLine(s);
            }
#endif
        }

        public async Task<string> LoadString(string fileName)
        {
            if (!HasFile(fileName)) return string.Empty;

            var filePath = AppFolder + "\\" + fileName;
            return await Task.Run(() => File.ReadAllText(filePath));
        }

        public bool HasFile(string fileName)
        {
            try
            {
                return File.Exists(AppFolder + "\\" + fileName);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<string> GetFiles(string folderName)
        {
            var path = AppFolder + "\\" + folderName;
            if (!Directory.Exists(path)) return null;
            var files = Directory.GetFiles(path);
            return files.ToList();
        }

        /// <summary>
        /// get file path and apply folder required
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>full file name</returns>
        private string GetFilePath(string fileName)
        {
            //valid and apply folder
            var fullName = AppFolder + "\\" + fileName;
            var folder = Path.GetDirectoryName(fullName);
            if (string.IsNullOrWhiteSpace(folder)) return "";
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            return fullName;
        }

    }
}