using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PhonosemanticAnalyser : MonoBehaviour
{
    Renderer rend;
    private string[] _soundLetterSignificance = new[]
    {
        "","А", "Е", "Ё", "И", "О", "У", "Ы", "Э", "Ю", "Я", "Б", "Б'", "В", "В'", "Г", "Г'", "Д", "Д'", "Ж", "З", "З'",
        "Й'", "К", "К'", "Л", "Л'", "М", "М'", "Н", "Н'", "П", "П'", "Р", "Р'", "С", "С'", "Т", "Т'", "Ф", "Ф'", "Х",
        "Х'", "Ц", "Ч'", "Ш", "Щ'"
    };

    private double[] _bigSmallFeatures = new[]
    {
        0.0, 1.8, 2.8, 2.9, 3.2, 1.3, 2.2, 1.7, 1.8, 3.1, 2.1, 2.1, 3.1, 2.1, 3.1, 2.6, 3.9, 2.0, 3.9, 2.4, 2.9, 3.1, 3.8,
        3.3, 4.2, 2.2, 3.7, 2.5, 3.7, 2.1, 3.2, 3.4, 4.6, 2.1, 3.4, 3.5, 4.3, 3.1, 4.1, 2.8, 4.3, 3.4, 4.1, 3.7, 3.9,
        3.2, 3.8
    };

    private double[] _brightDimFeatures = new[]
    {
        0.0, 2.0, 3.6, 2.5, 2.6, 1.8, 3.7, 3.6, 2.5, 2.5, 1.6, 2.0, 2.3, 2.7, 2.7, 2.5, 2.3, 2.1, 2.2, 2.9, 2.5, 2.7, 2.4,
        4.0, 3.8, 2.4, 2.3, 3.6, 3.2, 2.7, 3.0, 4.1, 1.4, 2.1, 1.8, 3.8, 3.8, 3.8, 3.9, 4.4, 4.3, 4.1, 4.3, 3.6, 3.3,
        4.5, 3.9
    };

    private double[] _soundLetterFrequency = new[]
    {
        0.0, 0.049, 0.050, 0.050, 0.041, 0.067, 0.017, 0.010, 0.004, 0.004, 0.013, 0.013, 0.005, 0.028, 0.011, 0.012, 0.003,
        0.020, 0.017, 0.008, 0.013, 0.002, 0.013, 0.030, 0.003, 0.020, 0.017, 0.025, 0.007, 0.040, 0.024, 0.020, 0.006,
        0.024, 0.014, 0.032, 0.017, 0.055, 0.020, 0.002, 0.001, 0.008, 0.001, 0.004,
        0.020, 0.012, 0.003
    };

    private double[] _smoothRoughFeatures = new[]
    {
        0.0, 1.6, 2.4, 2.5, 2.0, 1.5, 1.8, 2.5, 2.2, 2.4, 2.2, 3.2, 3.2, 3.5, 3.0, 3.6, 3.2, 3.4, 3.0, 4.5, 4.0, 3.0, 3.4,
        4.2, 3.9, 2.6, 2.2, 2.8, 2.6, 2.8, 2.8, 4.0, 3.5, 4.0, 4.0, 3.6, 3.5, 3.8, 3.8, 4.4, 3.8, 4.2, 3.8, 3.9, 4.6,
        4.1, 4.4
    };

    private double[] _roundAngularFeatures = new[]
    {
        0.0, 1.4, 2.2, 2.5, 2.2, 1.4, 2.6, 2.9, 2.0, 2.0, 2.0, 3.4, 3.0, 2.9, 3.0, 4.0, 3.6, 3.5, 3.2, 3.4, 3.5, 3.0, 3.2,
        4.4, 3.8, 3.1, 2.1, 3.1, 2.8, 3.1, 3.0, 3.8, 3.5, 4.0, 4.0, 3.1, 3.0, 4.1, 3.5, 3.6, 3.2, 3.9, 3.2, 3.9, 3.8,
        3.5, 3.6
    };

    private double[] _fastSlowFeatures = new[]
    {
        0.0, 3.4, 3.8, 3.7, 3.6, 3.6, 4.3, 4.4, 3.7, 3.6, 3.2, 1.9, 2.2, 2.4, 3.0, 2.2, 2.4, 2.4, 4.2, 3.4, 2.7, 3.0, 1.8,
        2.0, 1.9, 3.5, 3.0, 3.7, 3.4, 3.9, 3.4, 1.9, 2.4, 2.7, 2.4, 3.0, 3.1, 2.0, 2.5, 3.4, 3.7, 3.7, 3.5, 2.3, 2.0,
        3.1, 3.8
    };

    private double[] _lightDarkFeatures = new[]
    {
        0.0, 2.2, 1.9, 2.5, 2.0, 2.2, 3.6, 3.8, 2.5, 2.3, 1.9, 3.2, 2.6, 3.9, 2.8, 3.3, 2.9, 3.2, 2.2, 3.8, 2.5, 2.8, 2.6,
        3.6, 3.2, 3.1, 2.0, 3.3, 2.6, 3.1, 2.8, 4.0, 3.3, 3.8, 2.9, 2.5, 2.4, 4.0, 3.6, 4.0, 3.7, 4.4, 3.5, 3.5, 3.3,
        4.3, 3.8
    };

    private List<int> _currentSoundsIndexes = new List<int>();
    private double _currentMaxFreq = 0.0;
    private double _k = 0.0;
    private double _smoothness = 0.0;
    private double _roundness = 0.0;
    private double _brightness = 0.0;
    private double _darkness = 0.0;
    private double _slowness = 0.0;
    private double _smallness = 0.0f;

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
        _currentMaxFreq = 0.0;

        foreach (var index in _currentSoundsIndexes)
        {
            if (index < 0) return;
            if (_currentMaxFreq < _soundLetterFrequency[index]) _currentMaxFreq = _soundLetterFrequency[index];
        }
        
        _k = 0;
        _smoothness = 0;
        _roundness = 0; 
        
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
            
            _smoothness += fiRoughFeature * ki;
            _roundness += fiAngularFeature * ki;
            _brightness += fiBrightnessFeature * ki;
            _slowness += fiFastSlowFeature * ki;
            _darkness += fiLightDarkFeature * ki;
            _smallness += fiSmallBigFeature * ki;
        }

        _smoothness /= _k;
        _roundness /= _k;
        _brightness /= _k;
        _slowness /= _k;
        _darkness /= _k;
        _smallness /= _k;
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

    void SetFeatures(string word, int shock)
    {
        rend = this.gameObject.GetComponent<Renderer>();
        rend.enabled = false;
        
        CountPhonosemanticFeatures(GetWordTranscription(word, shock));
        
        rend.material.SetFloat("_Size", Mathf.Lerp(4.5f, 0f, GetSlowness() / 5f));
        rend.material.SetFloat("_Frequency", Mathf.Lerp(0, 8f, GetRoundness() / 5f));
        rend.material.SetFloat("_Glossiness", Mathf.Lerp(1f, 0f, GetSmoothness() / 5f) );
        rend.material.SetColor("_Color", Color.Lerp(new Color(0.6f,0.0f, 1f), new Color(0.79f,0.64f, 0.9f),GetBrightness() / 5f));
        rend.material.SetFloat("_Metallic", Mathf.Lerp(0f, 1f, GetDarkness()/ 5f));

        var s = Mathf.Lerp(2, 0f, GetSmallness() / 5f);
        gameObject.transform.localScale = new Vector3(s,s,s);

        rend.enabled = true;
    }

    private Tuple<string[], int> GetWordTranscription(string word, int shock)
    {
        return new WordTranscriber.Phonetic(word, shock).get_phonetic();
    }

    private void Start()
    {
        SetFeatures("рэйчик", 1);
    }
}
