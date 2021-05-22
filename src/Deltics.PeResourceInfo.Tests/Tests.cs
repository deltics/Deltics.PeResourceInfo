using System.IO;
using System.Linq;
using Deltics.PeImageInfo;
using FluentAssertions;
using Xunit;


namespace Deltics.PeResourceInfo.Tests
{
    public class Tests
    {
        private static ResourceInfo LoadArtefact(string filename)
        {
            return new (new FileStream($"artefacts/{filename}", FileMode.Open));
        }

        
        [Theory]
        [InlineData("x86.exe", ResourceType.RCDATA, ResourceType.VERSION)]
        [InlineData("x86.dll", ResourceType.RCDATA, ResourceType.VERSION)]
        [InlineData("x64.exe", ResourceType.RCDATA, ResourceType.VERSION)]
        [InlineData("x64.dll", ResourceType.RCDATA, ResourceType.VERSION)]
        public void ImageHasExpectedResources(string filename, params ResourceType[] resourceTypes)
        {
            var sut = LoadArtefact(filename);

            sut.IsValid.Should().BeTrue();

            sut.Directory.Length.Should().Be(resourceTypes.Length);

            foreach (var i in Enumerable.Range(0, sut.Directory.Length))
                sut.Directory[i].Id.Should().Be((ulong) resourceTypes[i]);
        }
    }
}