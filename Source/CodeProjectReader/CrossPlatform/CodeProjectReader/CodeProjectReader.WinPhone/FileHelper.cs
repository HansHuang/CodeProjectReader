using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Storage;
using CodeProjectReader.WinPhone;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper))]
namespace CodeProjectReader.WinPhone
{
    public class FileHelper:IFileHelper
    {
        private StorageFolder _localFolder;
        protected StorageFolder LocalFolder
        {
            get { return _localFolder ?? (_localFolder = ApplicationData.Current.LocalFolder); }
        }

        private string _appFolder;
        public string AppFolder
        {
            get
            {
                return string.IsNullOrWhiteSpace(_appFolder)
                    ? (_appFolder = LocalFolder.Path)
                    : _appFolder;
            }
        }

        public async Task SaveToFile(string fileName, string text)
        {
            var file = await CreateFile(fileName);
            if (file == null) return;
            using (var writer = new StreamWriter(await file.OpenStreamForWriteAsync()))
            {
               await writer.WriteAsync(text);
            }
        }

        public async Task SaveToFile(string fileName, Stream stream)
        {
            var file = await CreateFile(fileName);
            if (file == null) return;
            using (var s = new FileStream(file.Path, FileMode.Open))
            {
                await stream.CopyToAsync(s);
            }
#if DEBUG
            var files = Directory.GetFiles(Path.GetDirectoryName(file.Path));

            foreach (var s in files)
            {
                System.Diagnostics.Debug.WriteLine(s);
            }
#endif
        }

        public async Task<string> LoadString(string fileName)
        {
            if (!HasFile(fileName)) return string.Empty;
            var file = await LocalFolder.GetItemAsync(fileName);
            using (var reader = new StreamReader(file.Path))
            {
                return await reader.ReadToEndAsync();
            }
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

        private async Task<StorageFile> CreateFile(string fileName)
        {
            //valid and apply folder
            var folder = Path.GetDirectoryName(AppFolder + "\\" + fileName);
            if (string.IsNullOrWhiteSpace(folder)) return null;
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            return await LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
        }

    }
}
