using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Windows.Forms;
using System.Xml.Linq;
using YamlDotNet.Core.Tokens;
using YamlDotNet.Serialization;

namespace OUVoiceBanker
{
    public partial class Form1 : Form
    {
        string current;
        string venv;
        string temp;
        string output;
        string diffsinger;
        string checkpoints;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            current = Application.StartupPath; // Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            venv = $"{current}\\venv\\Scripts\\python.exe";
            temp = $"{current}temp";
            output = $"{current}output";
            diffsinger = $"{current}diffsinger";
            checkpoints = $"{diffsinger}\\checkpoints";

            // MessageBox.Show(output);
            if (!File.Exists(venv))
            {
                MessageBox.Show("venv Error");
                Environment.Exit(1);
            }
            if (!Directory.Exists(temp))
            {
                Directory.CreateDirectory(temp);
            }
            if (!Directory.Exists(output))
            {
                Directory.CreateDirectory(output);
            }
            if (!Directory.Exists(diffsinger)) 
            {
                MessageBox.Show("diffsinger was not exist");
                Environment.Exit(1);
            }
            if (!Directory.Exists(checkpoints))
            {
                MessageBox.Show("checkpoints was not exist");
                Environment.Exit(1);
            }
        }

        private void btnOpenAcoCkpt_Click(object sender, EventArgs e)
        {
            fdCommon.FilterIndex = 1;
            if (fdCommon.ShowDialog() == DialogResult.OK && fdCommon.FileName != "")
            {
                txtAcoCkpt.Text = fdCommon.FileName;
            }
        }

        private void btnOpenVarCkpt_Click(object sender, EventArgs e)
        {
            fdCommon.FilterIndex = 1;
            if (fdCommon.ShowDialog() == DialogResult.OK && fdCommon.FileName != "")
            {
                txtVarCkpt.Text = fdCommon.FileName;
            }
        }


        private void button7_Click(object sender, EventArgs e)
        {
            if (txtAcoCkpt.Text == "" || txtVoiceName.Text == "" || txtVarCkpt.Text == "")
            {
                MessageBox.Show("You have to fill whole of text box");
                return;
            }

            // 임시 폴더 정리 및 재생성
            string onnxPath = $"{temp}\\onnx";

            if (Directory.Exists(onnxPath))
            {
                Directory.Delete(onnxPath, true); 
            }

            if (!Directory.Exists(onnxPath))
            {
                Directory.CreateDirectory(onnxPath);
            }

            string acousticPath = $"{onnxPath}\\acoustic";
            if (!Directory.Exists(acousticPath))
            {
                Directory.CreateDirectory(acousticPath);
            }

            string variancePath = $"{onnxPath}\\variance";
            if (!Directory.Exists(variancePath))
            {
                Directory.CreateDirectory(variancePath);
            }

            // 체크포인트 폴더 생성
            string srcAcousticPath = Path.GetDirectoryName(txtAcoCkpt.Text)!;
            string srcVariancePath = Path.GetDirectoryName(txtVarCkpt.Text)!;

            string acousticName = $"{new DirectoryInfo(srcAcousticPath).Name}_acoustic";
            string varianceName = $"{new DirectoryInfo(srcVariancePath).Name}_variance";

            string checkpointAcousticPath = $"{checkpoints}\\{acousticName}";
            string checkpointVariancePath = $"{checkpoints}\\{varianceName}";

            if (!Directory.Exists(checkpointAcousticPath))
            {
                Directory.CreateDirectory(checkpointAcousticPath);
            }
            if (!Directory.Exists(checkpointVariancePath))
            {
                Directory.CreateDirectory(checkpointVariancePath);
            }
            
            string acousticCheckpointName = Path.GetFileName(txtAcoCkpt.Text);
            string varianceCheckpointName = Path.GetFileName (txtVarCkpt.Text);

            string newAcousticCheckPointFile = $"{checkpointAcousticPath}\\{acousticCheckpointName}";
            string newVarianceCheckpointFile = $"{checkpointVariancePath}\\{varianceCheckpointName}";

            // 파일 복사
            File.Copy(txtAcoCkpt.Text, newAcousticCheckPointFile, true);
            File.Copy(txtVarCkpt.Text, newVarianceCheckpointFile, true);

            File.Copy($"{srcAcousticPath}\\config.yaml", $"{checkpointAcousticPath}\\config.yaml", true);
            File.Copy($"{srcVariancePath}\\config.yaml", $"{checkpointVariancePath}\\config.yaml", true);

            File.Copy($"{srcAcousticPath}\\spk_map.json", $"{checkpointAcousticPath}\\spk_map.json", true);
            File.Copy($"{srcVariancePath}\\spk_map.json", $"{checkpointVariancePath}\\spk_map.json", true);

            File.Copy($"{srcAcousticPath}\\dictionary.txt", $"{checkpointAcousticPath}\\dictionary.txt", true);
            File.Copy($"{srcVariancePath}\\dictionary.txt", $"{checkpointVariancePath}\\dictionary.txt", true);

            // 스크립트 실행
            string scriptPath = $"{diffsinger}\\scripts\\export.py";
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = venv;
            info.Arguments = $"{diffsinger}\\scripts\\export.py acoustic --exp {acousticName} --out {acousticPath}";
            info.UseShellExecute = false;

            Process process = new Process();
            process.StartInfo = info;
            process.Start();
            process.WaitForExit();

            info.Arguments = $"{diffsinger}\\scripts\\export.py variance --exp {varianceName} --out {variancePath}";
            process = new Process();
            process.StartInfo = info;
            process.Start();
            process.WaitForExit();

            // 출력 폴더
            string pathVoiceBank = $"{output}\\{txtVoiceName.Text}";
            if (!Directory.Exists(pathVoiceBank))
            {
                Directory.CreateDirectory(pathVoiceBank);
            }

            // 파일 이름 변경
            Dictionary<string, string> patterns = new Dictionary<string, string> {
                { "acoustic.onnx", "acoustic.onnx" },
                { "dur.onnx", "dur.onnx" },
                { "linguistic.onnx", "linguistic.onnx" },
                { "pitch.onnx", "pitch.onnx" },
                { "variance.onnx", "variance.onnx" },
                { "phonemes.txt", "phonemes.txt" }
            };

            string[] folder_paths = new string[] {
                acousticPath,
                variancePath
            };

            foreach (string folder_path in folder_paths)
            {
                foreach (string filename in Directory.GetFiles(folder_path))
                {
                    foreach (KeyValuePair<string, string> pattern in patterns)
                    {
                        if (filename.Contains(pattern.Key))
                        {
                            string old_path = Path.Combine(folder_path, filename);
                            string new_path = Path.Combine(folder_path, pattern.Value);
                            if (File.Exists(old_path))
                            {
                                File.Move(old_path, new_path, true);
                            }
                        }
                    }
                }
            }
            
            foreach (string folder_path in folder_paths)
            {
                foreach (string filename in Directory.GetFiles(folder_path))
                {
                    string new_filename = filename;
                    if (filename.Contains("acoustic_acoustic."))
                    {
                        new_filename = filename.Replace("acoustic_acoustic.", "acoustic_");
                    }
                    else if (filename.Contains("variance_variance."))
                    {
                        new_filename = filename.Replace("variance_variance.", "variance_");
                    }
                    string old_path = Path.Combine(folder_path, filename);
                    string new_path = Path.Combine(folder_path, new_filename);
                    File.Move(old_path, new_path);
                }
            }

            // 출력 폴더 생성 (내부)
            string contentPath = Path.Combine(pathVoiceBank, "content", "OU_voicebank", txtVoiceName.Text);
            if (!Directory.Exists(contentPath))
            {
                Directory.CreateDirectory(contentPath);
            }

            // dsmain과 임베드 폴더 생성
            if (!Directory.Exists(Path.Combine(contentPath, "dsmain")))
            {
                Directory.CreateDirectory(Path.Combine(contentPath, "dsmain", "embeds", "acoustic"));
                Directory.CreateDirectory(Path.Combine(contentPath, "dsmain", "embeds", "variance"));
            }

            // 임베드 파일 목록 및 복사
            string[] acousticEmbFiles = Directory.GetFiles(acousticPath, "*.emb");
            string[] varianceEmbFiles = Directory.GetFiles(variancePath, "*.emb");

            foreach (string file in acousticEmbFiles)
            {
                File.Copy(file, Path.Combine(contentPath, "dsmain", "embeds", "acoustic", Path.GetFileName(file)));
            }

            foreach (string file in varianceEmbFiles)
            {
                File.Copy(file, Path.Combine(contentPath, "dsmain", "embeds", "variance", Path.GetFileName(file)));
            }

            // dsmain에 파일 복사
            File.Copy(Path.Combine(acousticPath, "acoustic.onnx"), Path.Combine(contentPath, "dsmain", "acoustic.onnx"));
            File.Copy(Path.Combine(acousticPath, "phonemes.txt"), Path.Combine(contentPath, "dsmain", "phonemes.txt"));
            File.Copy(Path.Combine(variancePath, "linguistic.onnx"), Path.Combine(contentPath, "dsmain", "linguistic.onnx"));

            // character.txt 생성
            using (StreamWriter file = new StreamWriter(Path.Combine(contentPath, "character.txt")))
            {
                file.WriteLine($"name={txtVoiceName.Text}");
                file.WriteLine($"image=");
                file.WriteLine($"author=");
                file.WriteLine($"voice=");
                file.WriteLine($"web=");
            }

            // character.yaml 생성
            string jsonFilePath = $"{checkpointAcousticPath}\\spk_map.json";
            string jsonString = File.ReadAllText(jsonFilePath);
            Dictionary<string, int> spkIds = JsonSerializer.Deserialize<Dictionary<string, int>>(jsonString)!;

            var subbanks = new List<object>();
            string characterYamlPath = Path.Combine(contentPath, "character.yaml");
            using (StreamWriter file = new StreamWriter(characterYamlPath))
            {
                file.WriteLine("text_file_encoding: utf-8");
                file.WriteLine("default_phonemizer: OpenUtau.Core.DiffSinger.DiffSingerPhonemizer");
                file.WriteLine("singer_type: diffsinger");
                file.WriteLine("subbanks:");

                foreach (string embFile in acousticEmbFiles)
                {
                    string acoustic_embed_suffix = "dsmain/embeds/acoustic/" + Path.GetFileNameWithoutExtension(embFile);
                    string acoustic_embed_color = (spkIds[Path.GetFileNameWithoutExtension(embFile).Split('.')[1]] + 1).ToString().PadLeft(2, '0') + ": " + Path.GetFileNameWithoutExtension(embFile);
                    subbanks.Add(new { color = acoustic_embed_color, suffix = acoustic_embed_suffix });
                }

                var serializerCharacter = new SerializerBuilder()
                    .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
                    .Build();

                var yaml = serializerCharacter.Serialize(subbanks);
                file.WriteLine(yaml);
            }

            // dsconfig.yaml 생성
            string dsconfigPath = Path.Combine(contentPath, "dsconfig.yaml");
            using (StreamWriter file = new StreamWriter(dsconfigPath))
            {
                file.WriteLine("acoustic: dsmain/acoustic.onnx");
                file.WriteLine("phonemes: dsmain/phonemes.txt");
                file.WriteLine("vocoder: nsf_hifigan");
                file.WriteLine("singer_type: diffsinger");
            }

            // acoustic config.yaml 읽기
            Dictionary<string, object> acousticConfig;
            using (StreamReader configReader = new StreamReader(Path.Combine(checkpointAcousticPath, "config.yaml")))
            {
                var _deserializer = new DeserializerBuilder().Build();
                acousticConfig = _deserializer.Deserialize<Dictionary<string, object>>(configReader);
            }

            // dsconfig.yaml 업데이트
            Dictionary<string, object> dsconfigData;
            using (StreamReader configReader = new StreamReader(dsconfigPath))
            {
                var _deserializer = new DeserializerBuilder().Build();
                dsconfigData = _deserializer.Deserialize<Dictionary<string, object>>(configReader);
            }

            string[] updateKeys = [
                "augmentation_args",
                "use_energy_embed", 
                "use_breathiness_embed", 
                "use_shallow_diffusion", 
                "use_key_shift_embed", 
                "use_speed_embed",
                "use_voicing_embed",
                "use_tension_embed"
            ]; 

            foreach (string key in updateKeys)
            {
                dsconfigData[key] = acousticConfig.GetValueOrDefault(key)!;
            }
            dsconfigData["max_depth"] = acousticConfig.GetValueOrDefault("K_step")!;

            List<string> acousticEmbedFileNames = new List<string>();
            foreach (string embFile in acousticEmbFiles)
            {
                acousticEmbedFileNames.Add("dsmain/embeds/acoustic/" + Path.GetFileNameWithoutExtension(embFile));
            }

            if (acousticEmbFiles.Length > 0)
            {
                dsconfigData["speakers"] = acousticEmbedFileNames;
            }

            // dsconfig.yaml 저장
            var serializerDsconfig = new SerializerBuilder().Build();
            string updatedDsconfigYaml = serializerDsconfig.Serialize(dsconfigData);
            File.WriteAllText(dsconfigPath, updatedDsconfigYaml);

            // variance embed file 목록 생성
            List<string> varianceEmbedFileNames = new List<string>();
            foreach (string embFile in varianceEmbFiles)
            {
                varianceEmbedFileNames.Add("../dsmain/embeds/variance/" + Path.GetFileNameWithoutExtension(embFile));
            }

            string dictionaryPath = Path.Combine(checkpointAcousticPath, "dictionary.txt");
            string phonemeDictionaryPath = Path.Combine(acousticPath, "dictionary.txt");

            List<Dictionary<string, object>> entries = new List<Dictionary<string, object>>();
            List<string> vowelTypes = ["a", "i", "u", "e", "o", "N", "M", "NG", "cl", "vf"];
            List<Dictionary<string, object>> vowelData = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> stopData = new List<Dictionary<string, object>>();

            // 사전 파일 처리
            using (StreamReader reader = new StreamReader(dictionaryPath))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split('\t');
                    string word = parts[0];
                    string phonemesStr = parts[1];
                    List<string> phonemes = phonemesStr.Split().ToList();
                    Dictionary<string, object> entry = new Dictionary<string, object> {
                        { "grapheme", word },
                        { "phonemes", phonemes }
                    };
                    entries.Add(entry);
                }
            }

            // 음소 사전 처리
            using (StreamReader reader = new StreamReader(phonemeDictionaryPath))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    string phoneme = line.Split('\t')[0];
                    string phonemeType = vowelTypes.Contains($"{phoneme[0]}") ? "vowel" : "stop";
                    Dictionary<string, object> entry = new Dictionary<string, object> {
                        { "symbol", phoneme },
                        { "type", phonemeType }
                    };
                    if (phonemeType == "vowel")
                    {
                        vowelData.Add(entry);
                    }
                    else
                    {
                        stopData.Add(entry);
                    }
                }
            }

            // vowel과 stop 리스트 정렬
            vowelData = vowelData.OrderBy(e => e["symbol"]).ToList();
            stopData = stopData.OrderBy(e => e["symbol"]).ToList();

            // dsdict.yaml 생성
            string dsDictPath = Path.Combine(contentPath, "dsdict.yaml");
            using (StreamWriter writer = new StreamWriter(dsDictPath))
            {
                writer.WriteLine("entries:");
                foreach (var entry in entries)
                {
                    writer.WriteLine($"- grapheme: {entry["grapheme"]}");
                    writer.WriteLine("  phonemes:");
                    foreach (string phoneme in (List<string>)entry["phonemes"])
                    {
                        writer.WriteLine($"  - {phoneme}");
                    }
                }
                writer.WriteLine();
                writer.WriteLine("symbols:");
                foreach (var entry in vowelData.Concat(stopData))
                {
                    writer.WriteLine($"- symbol: {entry["symbol"]}");
                    writer.WriteLine($"  type: {entry["type"]}");
                }
            }

            // variance config.yaml 읽기
            Dictionary<string, object> varianceConfig;
            using (StreamReader configReader = new StreamReader(Path.Combine(checkpointVariancePath, "config.yaml")))
            {
                var _deserializer = new DeserializerBuilder().Build();
                varianceConfig = _deserializer.Deserialize<Dictionary<string, object>>(configReader);
            }

            // variance config에서 가져온 값을 임시로 저장
            Dictionary<string, object?> tempStore = new Dictionary<string, object>();
            string[] tempKeys = [
                "audio_sample_rate",
                "hop_size",
                "predict_dur",
                "predict_pitch",
                "use_melody_encoder",
                "predict_voicing",
                "predict_tension",
                "predict_energy",
                "predict_breathiness",
            ];

            foreach (string key in tempKeys)
            {
                tempStore[key] = varianceConfig.GetValueOrDefault(key);
            }

            // dur 처리 시작
            string durOnnxPath = Path.Combine(variancePath, "dur.onnx");
            Directory.CreateDirectory(Path.Combine(contentPath, "dsdur"));
            File.Copy(durOnnxPath, Path.Combine(contentPath, "dsdur", "dur.onnx"));
            File.Copy(dsDictPath, Path.Combine(contentPath, "dsdur", Path.GetFileName(dsDictPath)));

            // dur config 생성
            string dsdurConfigPath = Path.Combine(contentPath, "dsdur", "dsconfig.yaml");
            using (StreamWriter file = new StreamWriter(dsdurConfigPath))
            {
                file.WriteLine("phonemes: ../dsmain/phonemes.txt");
                file.WriteLine("linguistic: ../dsmain/linguistic.onnx");
                file.WriteLine("dur: dur.onnx");
            }

            string dsdurConfigContent = File.ReadAllText(dsdurConfigPath);
            var deserializer = new DeserializerBuilder().Build();
            var dsdurConfig = deserializer.Deserialize<Dictionary<string, object>>(dsdurConfigContent);

            dsdurConfig["sample_rate"] = tempStore["audio_sample_rate"]!;
            dsdurConfig["hop_size"] = tempStore["hop_size"]!;
            dsdurConfig["predict_dur"] = tempStore["predict_dur"]!;

            if (varianceEmbedFileNames.Count > 0)
            {
                dsdurConfig["speakers"] = varianceEmbedFileNames;
            }
            var serializerDsDur = new SerializerBuilder().Build();
            string updatedDsDurYaml = serializerDsDur.Serialize(dsdurConfig);
            File.WriteAllText(dsdurConfigPath, updatedDsDurYaml);

            // pitch onnx 및 dict 파일 복사
            string pitchOnnxPath = Path.Combine(variancePath, "pitch.onnx");
            Directory.CreateDirectory(Path.Combine(contentPath, "dspitch"));
            File.Copy(pitchOnnxPath, Path.Combine(contentPath, "dspitch", "pitch.onnx"));
            File.Copy(dsDictPath, Path.Combine(contentPath, "dspitch", Path.GetFileName(dsDictPath)));

            // config 생성
            string dspitchConfigPath = Path.Combine(contentPath, "dspitch", "dsconfig.yaml");
            using (StreamWriter file = new StreamWriter(dspitchConfigPath))
            {
                file.WriteLine("phonemes: ../dsmain/phonemes.txt");
                file.WriteLine("linguistic: ../dsmain/linguistic.onnx");
                file.WriteLine("pitch: pitch.onnx");
                file.WriteLine("use_expr: true");
            }

            // config 업데이트
            string dspitchConfigContent = File.ReadAllText(dspitchConfigPath);
            deserializer = new DeserializerBuilder().Build();
            var dsPitchConfig = deserializer.Deserialize<Dictionary<string, object>>(dspitchConfigContent);

            dsPitchConfig["sample_rate"] = tempStore["audio_sample_rate"]!;
            dsPitchConfig["hop_size"] = tempStore["hop_size"]!;
            dsPitchConfig["predict_dur"] = tempStore["predict_dur"]!;
            if (varianceEmbedFileNames.Count > 0)
            {
                dsPitchConfig["speakers"] = varianceEmbedFileNames;
            }
            dsPitchConfig["use_note_rest"] = tempStore["use_melody_encoder"]!;

            // dsconfig.yaml 저장
            var serializerDsPitchConfig = new SerializerBuilder().Build();
            string updatedDsPitchConfigYaml = serializerDsPitchConfig.Serialize(dsPitchConfig);
            File.WriteAllText(dspitchConfigPath, updatedDsPitchConfigYaml);

            // variance onnx 파일 복사
            string varianceOnnxPath = Path.Combine(variancePath, "variance.onnx");
            Directory.CreateDirectory(Path.Combine(contentPath, "dsvariance"));
            File.Copy(varianceOnnxPath, Path.Combine(contentPath, "dsvariance", "variance.onnx"));
            File.Copy(dsDictPath, Path.Combine(contentPath, "dsvariance", Path.GetFileName(dsDictPath)));

            // config 파일 생성
            string dsvarianceConfigPath = Path.Combine(contentPath, "dsvariance", "dsconfig.yaml");
            using (StreamWriter file = new StreamWriter(dsvarianceConfigPath))
            {
                file.WriteLine("phonemes: ../dsmain/phonemes.txt");
                file.WriteLine("linguistic: ../dsmain/linguistic.onnx");
                file.WriteLine("variance: variance.onnx");
            }

            // config 업데이트
            string dsvarianceConfigContent = File.ReadAllText(dsvarianceConfigPath);

            var deserializerVariance = new DeserializerBuilder().Build();
            var dsVarianceConfig = deserializerVariance.Deserialize<Dictionary<string, object>>(dsvarianceConfigContent);

            dsVarianceConfig["sample_rate"] = tempStore["audio_sample_rate"]!;
            dsVarianceConfig["hop_size"] = tempStore["hop_size"]!;
            dsVarianceConfig["predict_dur"] = true;
            dsVarianceConfig["predict_voicing"] = tempStore["predict_voicing"]!;
            dsVarianceConfig["predict_tension"] = tempStore["predict_tension"]!;
            dsVarianceConfig["predict_energy"] = tempStore["predict_energy"]!;
            dsVarianceConfig["predict_breathiness"] = tempStore["predict_breathiness"]!;
            if (varianceEmbedFileNames.Count > 0)
            {
                dsVarianceConfig["speakers"] = varianceEmbedFileNames;
            }

            // config 저장
            var serializerDsVarianceConfig = new SerializerBuilder().Build();
            string updatedDsVarianceConfigYaml = serializerDsVarianceConfig.Serialize(dsVarianceConfig);
            File.WriteAllText(dsvarianceConfigPath, updatedDsVarianceConfigYaml);

            Process.Start("explorer.exe", contentPath);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", temp);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", output);
        }
    }
}
