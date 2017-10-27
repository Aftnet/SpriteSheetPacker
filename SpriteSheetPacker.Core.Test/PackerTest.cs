using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace SpriteSheetPacker.Core.Test
{
    public class PackerTest
    {
        [Fact]
        public void PackingWorks()
        {
            var files = GetSpriteFiles();
            var packer = new ImagePacker();
            packer.PackImage(files, true, true, 4096, 4096, 2, out var image, out var map);
            Assert.NotNull(image);
            Assert.NotNull(map);
        }

        private IEnumerable<FileInfo> GetSpriteFiles()
        {
            var folder = new DirectoryInfo("Sprites");
            var files = folder.GetFiles();
            return files.Where(d => ImagePacker.SupportedImageExtensions.Contains(d.Extension)).ToArray();
        }
    }
}
