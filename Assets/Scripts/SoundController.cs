using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class SoundController : MonoBehaviour
{

    public AudioSource m_audioSource;

    // A dictionary containing "grouped" list of AudioClips -> AudioClipGroups
    private IDictionary<string, List<AudioClip>> m_audioClips = new Dictionary<string, List<AudioClip>>();

    // From this dictionary you can determine if a certain AudioClipGroup is playing at the moment
    private IDictionary<string, bool> m_isPlaying = new Dictionary<string, bool>();

    // Start is called before the first frame update
    void Start()
    {
        ASyncLoadAllAudioClips();
    }

    // note: indexing here starts from 1!
    public void PlayOneShot(string name, int index = 1, bool playRandom = false)
    {
        List<AudioClip> audioClipGroup;

        // don't allow playing from the same AudioClipGroup if one is playing right now
        if (m_isPlaying.ContainsKey(name) && m_isPlaying[name]) {
            return;
        }

        if (m_audioClips.TryGetValue(name, out audioClipGroup))
        {
            index--;

            if (playRandom) index = UnityEngine.Random.Range(0, audioClipGroup.Count);

            m_audioSource.PlayOneShot(audioClipGroup[index]);

            StartCoroutine(AudioClipGroupIsPlaying(name, audioClipGroup[index].length));
        }
        else
        {
            Debug.LogError("Couldn't find AudioClip with name " + name + " to play");
        }
    }

    private IEnumerator AudioClipGroupIsPlaying(string name, float audioClipLength)
    {
        m_isPlaying[name] = true;
        yield return new WaitForSeconds(audioClipLength);
        m_isPlaying[name] = false;
    }

    private void ASyncLoadAllAudioClips()
    {
        foreach (Object file in Resources.LoadAll("Sounds"))
        {
            string fileName = file.name;

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
                m_audioClips["BossDeath"][1]; // error, this list has only length 1
            */

            // Check if the last two characters of the filename are an underscore and a number.
            Regex regex = new Regex(@"_\d$", RegexOptions.Singleline);

            string key = fileName.ToTitleCase();

            // Key without the last two characters
            if (regex.IsMatch(fileName) && fileName.Length > 2) key = fileName.Substring(0, fileName.Length - 2).ToTitleCase();

            m_audioClips.AddOrUpdate(key, file as AudioClip);
            m_isPlaying[key] = false;
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
