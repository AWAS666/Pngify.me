using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PngifyMe.Services.TTSPet.Settings
{
    public partial class TikTokSettings : ObservableObject, ITTSSettings
    {
        [ObservableProperty]
        private string endpoint = "https://tiktok-tts.weilnet.workers.dev/api/generation";

        private TikTokVoice voice;

        public TikTokVoice Voice
        {
            get
            {
                // Ensure the voice is always up-to-date with the reference list
                if (voice != null)
                {
                    voice = VoiceList.Find(v => v.Code == voice.Code);
                }
                return voice;
            }
            set
            {
                SetProperty(ref voice, value);
            }
        }

        [JsonIgnore]
        public static List<TikTokVoice> VoiceList { get; set; } = new()
        {
            new TikTokVoice(){ Name = "Jessie (English, American)", Code = "en_us_002"},
            new TikTokVoice(){ Name = "Narrator (Chris) (English, England)", Code = "en_uk_001"},
            new TikTokVoice(){ Name = "UK Male 2 (English, England)", Code = "en_uk_003"},
            new TikTokVoice(){ Name = "Peaceful (English, England)", Code = "en_female_emotional"},
            new TikTokVoice(){ Name = "Metro (Eddie) (English, Australian)", Code = "en_au_001"},
            new TikTokVoice(){ Name = "Smooth (Alex) (English, Australian)", Code = "en_au_002"},
            new TikTokVoice(){ Name = "Joey (English, American)", Code = "en_us_006"},
            new TikTokVoice(){ Name = "Professor (English, American)", Code = "en_us_007"},
            new TikTokVoice(){ Name = "Scientist (English, American)", Code = "en_us_009"},
            new TikTokVoice(){ Name = "Confidence (English, American)", Code = "en_us_010"},
            new TikTokVoice(){ Name = "Empathetic (English)", Code = "en_female_samc"},
            new TikTokVoice(){ Name = "Serious (English, American)", Code = "en_male_cody"},
            new TikTokVoice(){ Name = "Story Teller (English, American)", Code = "en_male_narration"},
            new TikTokVoice(){ Name = "Wacky (English, American)", Code = "en_male_funny"},
            new TikTokVoice(){ Name = "Alfred (English)", Code = "en_male_jarvis"},
            new TikTokVoice(){ Name = "Ash Magic (English, Australian)", Code = "en_male_ashmagic"},
            new TikTokVoice(){ Name = "Author (English, American)", Code = "en_male_santa_narration"},
            new TikTokVoice(){ Name = "Bae (English, American)", Code = "en_female_betty"},
            new TikTokVoice(){ Name = "Beauty Guru (English, American)", Code = "en_female_makeup"},
            new TikTokVoice(){ Name = "Bestie (English, American)", Code = "en_female_richgirl"},
            new TikTokVoice(){ Name = "Billy (English, American)", Code = "en_female_amie"},
            new TikTokVoice(){ Name = "Captain (English, American)", Code = "en_male_jason"},
            new TikTokVoice(){ Name = "Chris (English, American)", Code = "en_male_chris"},
            new TikTokVoice(){ Name = "Comedian (English, American)", Code = "en_male_miki"},
            new TikTokVoice(){ Name = "Cupid (English, American)", Code = "en_male_cupid"},
            new TikTokVoice(){ Name = "Debutante (English, American)", Code = "en_female_shenna"},
            new TikTokVoice(){ Name = "Designer (English, American)", Code = "en_male_whitney"},
            new TikTokVoice(){ Name = "Doll (English, American)", Code = "en_female_doll"},
            new TikTokVoice(){ Name = "Elf (English, American)", Code = "en_male_adam_elf"},
            new TikTokVoice(){ Name = "Foodie (English, England)", Code = "en_male_adrian"},
            new TikTokVoice(){ Name = "Game On (English, American)", Code = "en_male_jomboy"},
            new TikTokVoice(){ Name = "Ghost (English, American)", Code = "en_female_ghost"},
            new TikTokVoice(){ Name = "Ghost Host (English, American)", Code = "en_male_ghosthost"},
            new TikTokVoice(){ Name = "GingerChime (English, American)", Code = "en_male_david_gingerman"},
            new TikTokVoice(){ Name = "Grandma (English, American)", Code = "en_female_grandma"},
            new TikTokVoice(){ Name = "Lord Cringe (English, England)", Code = "en_male_ukneighbor"},
            new TikTokVoice(){ Name = "Magician (English)", Code = "en_male_wizard"},
            new TikTokVoice(){ Name = "Marty (English)", Code = "en_male_trevor"},
            new TikTokVoice(){ Name = "Mr. GoodGuy (Deadpool) (English, American)", Code = "en_male_deadpool"},
            new TikTokVoice(){ Name = "Mr. Meticulous (English, England)", Code = "en_male_ukbutler"},
            new TikTokVoice(){ Name = "Olantekkers (English, England)", Code = "en_male_olantekkers"},
            //new TikTokVoice(){ Name = "Optimus Prime (English)", Code = "en_male_petercullen"},
            new TikTokVoice(){ Name = "Pirate (English)", Code = "en_male_pirate"},
            new TikTokVoice(){ Name = "Santa (English)", Code = "en_male_santa"},
            new TikTokVoice(){ Name = "Santa (w/ effect) (English)", Code = "en_male_santa_effect"},
            new TikTokVoice(){ Name = "Santa (Corey) (English)", Code = "en_male_corey_santa"},
            new TikTokVoice(){ Name = "Stylist (English, American)", Code = "en_male_maxwell"},
            new TikTokVoice(){ Name = "Varsity (English, American)", Code = "en_female_pansino"},
            new TikTokVoice(){ Name = "Victory (English, American)", Code = "en_female_erika"},
            new TikTokVoice(){ Name = "Werewolf (English, American)", Code = "en_female_werewolf"},
            new TikTokVoice(){ Name = "Witch (English, American)", Code = "en_female_witch"},
            new TikTokVoice(){ Name = "Zombie (English, American)", Code = "en_female_zombie"},
            new TikTokVoice(){ Name = "Trickster (Grinch) (English)", Code = "en_male_grinch"},
            new TikTokVoice(){ Name = "Ghostface (Scream) (English, American)", Code = "en_us_ghostface"},
            new TikTokVoice(){ Name = "Chewbacca (Star Wars) (English)", Code = "en_us_chewbacca"},
            new TikTokVoice(){ Name = "C-3PO (Star Wars) (English)", Code = "en_us_c3po"},
            new TikTokVoice(){ Name = "Stormtrooper (Star Wars) (English)", Code = "en_us_stormtrooper"},
            new TikTokVoice(){ Name = "Stitch (Lilo &amp; Stitch) (English)", Code = "en_us_stitch"},
            new TikTokVoice(){ Name = "Rocket (Guardians of the Galaxy) (English, American)", Code = "en_us_rocket"},
            new TikTokVoice(){ Name = "Madame Leota (Haunted Mansion) (English)", Code = "en_female_madam_leota"},
            new TikTokVoice(){ Name = "Song: Caroler (English)", Code = "en_male_sing_deep_jingle"},
            new TikTokVoice(){ Name = "Song: Classic Electric (English)", Code = "en_male_m03_classical"},
            new TikTokVoice(){ Name = "Song: Cottagecore (Salut d'Amour) (English)", Code = "en_female_f08_salut_damour"},
            new TikTokVoice(){ Name = "Song: Cozy (English)", Code = "en_male_m2_xhxs_m03_christmas"},
            new TikTokVoice(){ Name = "Song: Open Mic (Warmy Breeze) (English)", Code = "en_female_f08_warmy_breeze"},
            new TikTokVoice(){ Name = "Song: Opera (Halloween) (English)", Code = "en_female_ht_f08_halloween"},
            new TikTokVoice(){ Name = "Song: Euphoric (Glorious) (English)", Code = "en_female_ht_f08_glorious"},
            new TikTokVoice(){ Name = "Song: Hypetrain (It Goes Up) (English)", Code = "en_male_sing_funny_it_goes_up"},
            new TikTokVoice(){ Name = "Song: Jingle (Lobby) (English)", Code = "en_male_m03_lobby"},
            new TikTokVoice(){ Name = "Song: Melodrama (Wonderful World) (English)", Code = "en_female_ht_f08_wonderful_world"},
            new TikTokVoice(){ Name = "Song: NYE 2023 (English)", Code = "en_female_ht_f08_newyear"},
            new TikTokVoice(){ Name = "Song: Thanksgiving (English)", Code = "en_male_sing_funny_thanksgiving"},
            new TikTokVoice(){ Name = "Song: Toon Beat (Sunshine Soon) (English)", Code = "en_male_m03_sunshine_soon"},
            new TikTokVoice(){ Name = "Song: Pop Lullaby (English)", Code = "en_female_f08_twinkle"},
            new TikTokVoice(){ Name = "Song: Quirky Time (English)", Code = "en_male_m2_xhxs_m03_silly"},
            new TikTokVoice(){ Name = "French Male 1 (French)", Code = "fr_001"},
            new TikTokVoice(){ Name = "French Male 2 (French)", Code = "fr_002"},
            new TikTokVoice(){ Name = "German Female (German)", Code = "de_001"},
            new TikTokVoice(){ Name = "German Male (German)", Code = "de_002"},
            new TikTokVoice(){ Name = "Darma (Indonesian)", Code = "id_male_darma"},
            new TikTokVoice(){ Name = "Icha (Indonesian)", Code = "id_female_icha"},
            new TikTokVoice(){ Name = "Noor (Indonesian)", Code = "id_female_noor"},
            new TikTokVoice(){ Name = "Putra (Indonesian)", Code = "id_male_putra"},
            new TikTokVoice(){ Name = "Italian Male (Italian)", Code = "it_male_m18"},
            new TikTokVoice(){ Name = "Miho (美穂) (Japanese)", Code = "jp_001"},
            new TikTokVoice(){ Name = "Keiko (恵子) (Japanese)", Code = "jp_003"},
            new TikTokVoice(){ Name = "Sakura (さくら) (Japanese)", Code = "jp_005"},
            new TikTokVoice(){ Name = "Naoki (直樹) (Japanese)", Code = "jp_006"},
            new TikTokVoice(){ Name = "モリスケ (Morisuke) (Japanese)", Code = "jp_male_osada"},
            new TikTokVoice(){ Name = "モジャオ (Matsuo) (Japanese)", Code = "jp_male_matsuo"},
            new TikTokVoice(){ Name = "まちこりーた (Machikoriiita) (Japanese)", Code = "jp_female_machikoriiita"},
            new TikTokVoice(){ Name = "マツダ家の日常 (Matsudake) (Japanese)", Code = "jp_male_matsudake"},
            new TikTokVoice(){ Name = "修一朗 (Shuichiro) (Japanese)", Code = "jp_male_shuichiro"},
            new TikTokVoice(){ Name = "丸山礼 (Maruyama Rei) (Japanese)", Code = "jp_female_rei"},
            new TikTokVoice(){ Name = "ヒカキン (Hikakin) (Japanese)", Code = "jp_male_hikakin"},
            new TikTokVoice(){ Name = "八木沙季 (Yagi Saki) (Japanese)", Code = "jp_female_yagishaki"},
            new TikTokVoice(){ Name = "Korean Male 1 (Korean)", Code = "kr_002"},
            new TikTokVoice(){ Name = "Korean Male 2 (Korean)", Code = "kr_004"},
            new TikTokVoice(){ Name = "Korean Female (Korean)", Code = "kr_003"},
            new TikTokVoice(){ Name = "Júlia (Portuguese, Brazilian)", Code = "br_003"},
            new TikTokVoice(){ Name = "Ana (Portuguese, Brazilian)", Code = "br_004"},
            new TikTokVoice(){ Name = "Lucas (Portuguese, Brazilian)", Code = "br_005"},
            new TikTokVoice(){ Name = "Lhays Macedo (Portuguese)", Code = "pt_female_lhays"},
            new TikTokVoice(){ Name = "Laizza (Portuguese)", Code = "pt_female_laizza"},
            //new TikTokVoice(){ Name = "Optimus Prime (Portuguese) (Portuguese)", Code = "pt_male_transformer"},
            new TikTokVoice(){ Name = "Spanish Male (Spanish)", Code = "es_002"},
            new TikTokVoice(){ Name = "Julio (Spanish)", Code = "es_male_m3"},
            new TikTokVoice(){ Name = "Alejandra (Spanish)", Code = "es_female_f6"},
            new TikTokVoice(){ Name = "Mariana (Spanish)", Code = "es_female_fp1"},
            new TikTokVoice(){ Name = "Álex (Warm) (Spanish, Mexico)", Code = "es_mx_002"},
            //new TikTokVoice(){ Name = "Optimus Prime (Mexican) (Spanish, Mexico)", Code = "es_mx_male_transformer"},
            new TikTokVoice(){ Name = "Super Mamá (Spanish, Mexico)", Code = "es_mx_female_supermom"},
        };


        public TikTokSettings()
        {
            voice = VoiceList.First();
        }
    }

    public class TikTokVoice
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
