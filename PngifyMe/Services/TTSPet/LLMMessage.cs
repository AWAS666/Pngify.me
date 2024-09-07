using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.Services.TTSPet
{
    public class LLMMessage
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public bool Read { get; set; }
        public bool ReadInput { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

        public string ToRead => ReadInput ? Input : Output;

        public string UserName { get; internal set; }
    }
}
