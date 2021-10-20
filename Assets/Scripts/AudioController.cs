using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class AudioController : MonoBehaviour
{

    public AudioSource m_audioSource;

    // A dictionary containing "grouped" list of AudioClips -> AudioClipGroups
    private IDictionary<string, List<AudioClip>> m_audioClips = new Dictionary<string, List<AudioClip>>();

    // This will return true if an AudioClipGroup is playing at the moment
    private IDictionary<string, bool> m_isPlaying = new Dictionary<string, bool>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadAllAudioClips());
    }

    // note: indexing here starts from 1!
    public void PlayOneShot(string name, int index = 1, bool playRandom = false)
    {
        List<AudioClip> audioClipGroup;

        // don't allow playing from the same AudioClipGroup if one is playing right now
        if (m_isPlaying[name]) {
            return;
        }

        if (m_audioClips.TryGetValue(name, out audioClipGroup))
        {
            index--;

            if (playRandom) index = UnityEngine.Random.Range(0, audioClipGroup.Count);

            m_audioSource.PlayOneShot(audioClipGroup[index]);

            StartCoroutine(AudioGroupIsPlaying(name, audioClipGroup[index].length));
        }
        else
        {
            Debug.LogError("Couldn't find AudioClip with name " + name + " to play");
        }
    }

    private IEnumerator AudioGroupIsPlaying(string name, float audioClipLength)
    {
        m_isPlaying[name] = true;
        yield return new WaitForSeconds(audioClipLength);
        m_isPlaying[name] = false;
    }

    private IEnumerator LoadAllAudioClips()
    {
        foreach (string file in System.IO.Directory.GetFiles(Path.Combine(Application.dataPath, "Sounds")))
        {
            if (Path.GetExtension(file) != ".wav") continue;

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(file, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);

                    /*
                    Group AudioClips based on filename here.

                    m_audioClips is a dictionary containing a list of "grouped" AudioClips.

                    For example, if we have two files with otherwise same names, but the last two characters of the filename are an underscore and a number,
                    e.g. "boss_hit_1.wav" and "boss_hit_2.wav", put them in the same list containing AudioClips.

                    In our example the AudioClips can now be accessed from the dictionary via a TitleCase key "BossHit":

                        m_audioClips["BossHit"][0]; // "boss_hit_1.wav"
                        m_audioClips["BossHit"][1]; // "boss_hit_2.wav"

                    AudioClips with other naming conventions will be put inside the dictionary normally, e.g. "boss_death.wav":

                        m_audioClips["BossDeath"][0]; // "boss_death.wav"
                    */

                    // Check if the last two characters of the filename are an underscore and a number.
                    Regex regex = new Regex(@"_\d$", RegexOptions.Singleline);

                    string key = fileName.ToTitleCase();

                    // Key without the last two characters
                    if (regex.IsMatch(fileName) && fileName.Length > 2) key = fileName.Substring(0, fileName.Length - 2).ToTitleCase();

                    m_audioClips.AddOrUpdate(key, DownloadHandlerAudioClip.GetContent(www));
                    m_isPlaying[key] = false;
                }
            }
        }
    }
}

public static class ExtensionMethods {
    // Add to list in dictionary if key exists, otherwise create a new list with the value
    public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, List<TValue>> dict, TKey key, TValue val)
    {
        try
        {
            dict[key].Add(val);
        }
        catch
        {
            dict[key] = new List<TValue> { val };
        }
    }

    // Convert under_score to TitleCase
    public static string ToTitleCase(this string s) => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.Replace("_", " ")).Replace(" ", string.Empty);
}
