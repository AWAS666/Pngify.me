using System.IO;
using System.Threading.Tasks;

namespace PngifyMe.Services.TTSPet
{
    public interface ITTSProvider
    {
        Task<Stream?> GenerateSpeech(string input);
    }
}