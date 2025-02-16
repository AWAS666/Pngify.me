using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class PngTuberPlusObject
{
    public int animSpeed { get; set; }
    public bool clipped { get; set; }
    public string costumeLayers { get; set; }
    public int drag { get; set; }
    public int frames { get; set; }
    public long identification { get; set; }
    public bool ignoreBounce { get; set; }
    public string imageData { get; set; }
    public string offset { get; set; }
    public long? parentId { get; set; }
    public string path { get; set; }
    public string pos { get; set; }
    public int rLimitMax { get; set; }
    public int rLimitMin { get; set; }
    public int rotDrag { get; set; }
    public int showBlink { get; set; }
    public int showTalk { get; set; }
    public float stretchAmount { get; set; }
    public string toggle { get; set; }
    public string type { get; set; }
    public int xAmp { get; set; }
    public float xFrq { get; set; }
    public int yAmp { get; set; }
    public float yFrq { get; set; }
    public int zindex { get; set; }
}
