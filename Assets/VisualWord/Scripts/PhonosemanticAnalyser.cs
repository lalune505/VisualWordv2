using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class PhonosemanticAnalyser : MonoBehaviour
{
    [SerializeField] private VisualText visualText;
    [SerializeField] private List<string> words = new List<string>();
    [SerializeField] private Color[] vowelsColors;
    
    private Dictionary<float, Color> _colors = new Dictionary<float, Color>();
    
    private string[] _soundLetterSignificance = new[]
    {
        "А", "Е", "Ё", "И", "О", "У", "Ы", "Э", "Ю", "Я", "Б", "Б'", "В", "В'", "Г", "Г'", "Д", "Д'", "Ж", "З", "З'",
        "Й'", "К", "К'", "Л", "Л'", "М", "М'", "Н", "Н'", "П", "П'", "Р", "Р'", "С", "С'", "Т", "Т'", "Ф", "Ф'", "Х",
        "Х'", "Ц", "Ч'", "Ш", "Щ'", ""
    };
    private double[] _soundLetterFrequency = new[]
    {
        0.049, 0.050, 0.050, 0.041, 0.067, 0.017, 0.010, 0.004, 0.004, 0.013, 0.013, 0.005, 0.028, 0.011, 0.012, 0.003,
        0.020, 0.017, 0.008, 0.013, 0.002, 0.013, 0.030, 0.003, 0.020, 0.017, 0.025, 0.007, 0.040, 0.024, 0.020, 0.006,
        0.024, 0.014, 0.032, 0.017, 0.055, 0.020, 0.002, 0.001, 0.008, 0.001, 0.004,
        0.020, 0.012, 0.003, 0.0
    };

    private double[] _bigSmallFeatures = new[] //большой-маленький scale 
    {
        1.8, 2.8, 2.9, 3.2, 1.3, 2.2, 1.7, 1.8, 3.1, 2.1, 2.1, 3.1, 2.1, 3.1, 2.6, 3.9, 2.0, 3.9, 2.4, 2.9, 3.1, 3.8,
        3.3, 4.2, 2.2, 3.7, 2.5, 3.7, 2.1, 3.2, 3.4, 4.6, 2.1, 3.4, 3.5, 4.3, 3.1, 4.1, 2.8, 4.3, 3.4, 4.1, 3.7, 3.9,
        3.2, 3.8,0.0,
    };

    private double[] _brightDimFeatures = new[] //яркий-тусклый brightness or emission 
    {
        2.0, 3.6, 2.5, 2.6, 1.8, 3.7, 3.6, 2.5, 2.5, 1.6, 2.0, 2.3, 2.7, 2.7, 2.5, 2.3, 2.1, 2.2, 2.9, 2.5, 2.7, 2.4,
        4.0, 3.8, 2.4, 2.3, 3.6, 3.2, 2.7, 3.0, 4.1, 1.4, 2.1, 1.8, 3.8, 3.8, 3.8, 3.9, 4.4, 4.3, 4.1, 4.3, 3.6, 3.3,
        4.5, 3.9, 0.0
    };

    private double[] _smoothRoughFeatures = new[] //гладкий-шероховатый smoothness
    {
        1.6, 2.4, 2.5, 2.0, 1.5, 1.8, 2.5, 2.2, 2.4, 2.2, 3.2, 3.2, 3.5, 3.0, 3.6, 3.2, 3.4, 3.0, 4.5, 4.0, 3.0, 3.4,
        4.2, 3.9, 2.6, 2.2, 2.8, 2.6, 2.8, 2.8, 4.0, 3.5, 4.0, 4.0, 3.6, 3.5, 3.8, 3.8, 4.4, 3.8, 4.2, 3.8, 3.9, 4.6,
        4.1, 4.4, 0.0
    };

    private double[] _roundAngularFeatures = new[] //округлый-угловатый wave frequency
    {
        1.4, 2.2, 2.5, 2.2, 1.4, 2.6, 2.9, 2.0, 2.0, 2.0, 3.4, 3.0, 2.9, 3.0, 4.0, 3.6, 3.5, 3.2, 3.4, 3.5, 3.0, 3.2,
        4.4, 3.8, 3.1, 2.1, 3.1, 2.8, 3.1, 3.0, 3.8, 3.5, 4.0, 4.0, 3.1, 3.0, 4.1, 3.5, 3.6, 3.2, 3.9, 3.2, 3.9, 3.8,
        3.5, 3.6, 0.0
    };

    private double[] _fastSlowFeatures = new[] // быстрый-медленный speed
    {
        3.4, 3.8, 3.7, 3.6, 3.6, 4.3, 4.4, 3.7, 3.6, 3.2, 1.9, 2.2, 2.4, 3.0, 2.2, 2.4, 2.4, 4.2, 3.4, 2.7, 3.0, 1.8,
        2.0, 1.9, 3.5, 3.0, 3.7, 3.4, 3.9, 3.4, 1.9, 2.4, 2.7, 2.4, 3.0, 3.1, 2.0, 2.5, 3.4, 3.7, 3.7, 3.5, 2.3, 2.0,
        3.1, 3.8, 0.0
    };

    private double[] _lightDarkFeatures = new[] // светлый-темный metalness
    {
        2.2, 1.9, 2.5, 2.0, 2.2, 3.6, 3.8, 2.5, 2.3, 1.9, 3.2, 2.6, 3.9, 2.8, 3.3, 2.9, 3.2, 2.2, 3.8, 2.5, 2.8, 2.6,
        3.6, 3.2, 3.1, 2.0, 3.3, 2.6, 3.1, 2.8, 4.0, 3.3, 3.8, 2.9, 2.5, 2.4, 4.0, 3.6, 4.0, 3.7, 4.4, 3.5, 3.5, 3.3,
        4.3, 3.8, 0.0
    };
    
    private double[] _activePassiveFeatures = new[] // активный-пассивный wave size
    {
        2.1, 2.4, 3.4, 2.9 ,2.2, 3.2, 4.0, 3.6, 3.4, 2.6, 2.0, 3.0, 2.4, 3.2, 2.8, 2.9, 2.4, 2.0, 3.0, 2.8, 3.2, 2.2,
        2.8, 3.0, 2.5, 2.8, 4.0, 3.1, 2.8, 3.2, 3.4, 3.6, 2.0, 2.1, 3.2, 3.5, 3.2, 3.2, 4.1, 4.0, 3.8, 4.4, 3.4, 3.9,
        3.6, 4.0, 0.0
    };
    

    private List<int> _currentSoundsIndexes = new List<int>();
    private  Dictionary<int, int> _currentSoundOccurrences = new Dictionary<int, int>();
    private double _currentMaxFreq = 0.0;
    private int _currentStressedVowel = 0;
    private double _k = 0.0;
    private double _smoothness = 0.0;
    private double _roundness = 0.0;
    private double _brightness = 0.0;
    private double _darkness = 0.0;
    private double _slowness = 0.0;
    private double _smallness = 0.0f;
    private double _passivness = 0.0f;
     

    private List<int> FindSoundsIndexes(string[] sounds)
    {
        foreach (var sound in sounds)
        {
            Debug.Log(sound);
        }
        return sounds.Select(sound => Array.IndexOf(_soundLetterSignificance, sound.ToUpper())).ToList();
    }

    public void CountPhonosemanticFeatures(Tuple<string[], int> sounds)
    {
        _currentSoundsIndexes = FindSoundsIndexes(sounds.Item1);
        _currentStressedVowel = sounds.Item2;
        _currentMaxFreq = 0.0;

        foreach (var index in _currentSoundsIndexes)
        {
            if (index < 0) return;
            if (_currentMaxFreq < _soundLetterFrequency[index]) _currentMaxFreq = _soundLetterFrequency[index];
        }
        
        _k = 0;
        _smoothness = 0.0;
        _roundness = 0.0;
        _brightness = 0.0;
        _darkness = 0.0;
        _slowness = 0.0;
        _smallness = 0.0f;
        _passivness = 0.0f;
        
        for (int i = 0;i < _currentSoundsIndexes.Count;i++)
        {
            var n = 1;
            if (i == 0) {n = 4;}
            if (i == sounds.Item2) { n = 2;}

            double ki;
            if (_soundLetterFrequency[_currentSoundsIndexes[i]] == 0.0)
            { ki = 0.0;} else{ ki = n * _currentMaxFreq / _soundLetterFrequency[_currentSoundsIndexes[i]];}
            _k += ki;
            var fiRoughFeature = _smoothRoughFeatures[_currentSoundsIndexes[i]];
            var fiAngularFeature = _roundAngularFeatures[_currentSoundsIndexes[i]];
            var fiBrightnessFeature = _brightDimFeatures[_currentSoundsIndexes[i]];
            var fiFastSlowFeature = _fastSlowFeatures[_currentSoundsIndexes[i]];
            var fiLightDarkFeature = _lightDarkFeatures[_currentSoundsIndexes[i]];
            var fiSmallBigFeature = _bigSmallFeatures[_currentSoundsIndexes[i]];
            var fiActivePassive = _activePassiveFeatures[_currentSoundsIndexes[i]];         
            _smoothness += fiRoughFeature * ki;
            _roundness += fiAngularFeature * ki;
            _brightness += fiBrightnessFeature * ki;
            _slowness += fiFastSlowFeature * ki;
            _darkness += fiLightDarkFeature * ki;
            _smallness += fiSmallBigFeature * ki;
            _passivness += fiActivePassive * ki;
        }

        _smoothness /= _k;
        _roundness /= _k;
        _brightness /= _k;
        _slowness /= _k;
        _darkness /= _k;
        _smallness /= _k;
        _passivness /= _k;
    }

    public void CountColorValues()
    {
        _currentSoundOccurrences = new Dictionary<int, int>();
        _colors = new Dictionary<float, Color>();
        
        for (int i = 0; i < _currentSoundsIndexes.Count; i++)
        {
            if (!_currentSoundOccurrences.ContainsKey(_currentSoundsIndexes[i]))
            {

                if (i == _currentStressedVowel)
                {
                    _currentSoundOccurrences.Add(_currentSoundsIndexes[i],
                       2 * _currentSoundsIndexes.Count(x => x.Equals(_currentSoundsIndexes[i])));
                }
                else
                {
                    _currentSoundOccurrences.Add(_currentSoundsIndexes[i],
                        _currentSoundsIndexes.Count(x => x.Equals(_currentSoundsIndexes[i])));
                }
            }
            else
            {
                if (i == _currentStressedVowel)
                {
                    _currentSoundOccurrences[_currentSoundsIndexes[i]] =
                        2 * _currentSoundsIndexes.Count(x => x.Equals(_currentSoundsIndexes[i]));
                }
            }
        }

        var lettersCount = _currentSoundOccurrences.Sum(x => x.Value);
        foreach (var letterOccurrence in _currentSoundOccurrences)
        {
            float pn = (float) _soundLetterFrequency[letterOccurrence.Key];
            float s = Mathf.Sqrt(pn * (1 - pn) / lettersCount);
            float z = ((float)letterOccurrence.Value /  (float)lettersCount - pn) / s;
            if (letterOccurrence.Key < 10)
            {
                _colors.Add(z,vowelsColors[letterOccurrence.Key]);
            }
        }

        _colors = _colors.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
    }

    private float GetSmoothness()
    {
        Debug.Log(_smoothness);
        return (float)_smoothness;
    }

    private float GetRoundness()
    {
        Debug.Log(_roundness);
        return (float)_roundness;
    }

    private float GetBrightness()
    {
        Debug.Log(_brightness);
        return (float)_brightness;
    }

    private float GetDarkness()
    {
        Debug.Log(_darkness);
        return (float) _darkness;
    }

    private float GetSlowness()
    {
        Debug.Log(_slowness);
        return (float) _slowness;
    }

    private float GetSmallness()
    {
        Debug.Log(_smallness);
        return (float) _smallness;
    }
    private float GetPassivness()
    {
        Debug.Log(_passivness);
        return (float) _passivness;
    }

    private VisualWord GetVisualWord(string word, int shock)
    {
        CountPhonosemanticFeatures(GetWordTranscription(word, shock));
        CountColorValues();
        
        var size = Mathf.Lerp(4.0f, 0.2f, GetPassivness() / 5f);
        var scale = Mathf.Lerp(2, 0f, GetSmallness() / 5f);
        var speed = Mathf.Lerp(2.0f, 0.2f, GetSlowness() / 5f);
        var freq = Mathf.Lerp(0.1f, 5f, GetRoundness() / 5f);
        var glos = Mathf.Lerp(1f, 0.2f, GetSmoothness() / 5f) ;
        var maxColor = _colors.ElementAt(0).Value;
        var color = _colors.Count > 1 ? _colors.ElementAt(1).Value : _colors.ElementAt(0).Value;
        var rimColor = Color.Lerp(_colors.ElementAt(0).Value,Color.black, GetBrightness() / 5f);
        var metallic =  Mathf.Lerp(0f, 1f, GetDarkness()/ 5f);
        
        return new VisualWord(size, scale, color, rimColor, maxColor, speed, freq, glos, metallic);
    }

    private Tuple<string[], int> GetWordTranscription(string word, int shock)
    {
        return new WordTranscriber.Phonetic(word, shock).get_phonetic();
    }

    private void Awake()
    {
        visualText.Clear();
        
        foreach (var word in words)
        {
            var tuple = GetFormatedInput(word, ";");
            visualText.AddWord(GetVisualWord(tuple.Item1, tuple.Item2));
        }
    }
    
    public static Tuple<string, int> GetFormatedInput(string text, string stopAt = ";")
    {
        if (!String.IsNullOrWhiteSpace(text))
        {
            int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

            Debug.Log(charLocation);
            if (charLocation > 0)
            {
                return new Tuple<string, int>(text.Substring(0, charLocation),
                    Int32.Parse(text.Substring(charLocation + 1, text.Length - charLocation - 1)));
            }
        }

        return new Tuple<string, int>(String.Empty, 0);
    }
}
