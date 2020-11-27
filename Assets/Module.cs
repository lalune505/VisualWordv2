using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class Module : MonoBehaviour
{
    private void Start()
    {
        Debug.Log(new Phonetic("дом", 1).get_phonetic());
    }
}
public class Phonetic
{

    private string word = null;
    private string __work_word = null;
    private string _vowels = "аеёиоуыэюя";
    private int acc = 0;
    private Dictionary<string, string> replace_map = new Dictionary<string, string>()
    {
        {"вств", "ств"},
        {"дс", "ц"},
        {"дц", "цц"},
        {"дч", "чч"},
        {"жч", "щщ"},
        {"здн", "зн"},
        {"здц", "сц"},
        {"здч", "щщ"},
        {"зж", "жж"},
        {"зч", "щщ"},
        {"зш", "шш"},
        {"лнц", "нц"},
        {"ндск", "нск"},
        {"ндц", "нц"},
        {"ндш", "нш"},
        {"нтг", "нг"},
        {"нтск", "нск"},
        {"рдц", "рц"},
        {"рдч", "рч"},
        {"сж", "жж"},
        {"стл", "сл"},
        {"стн", "сн"},
        {"стс", "сс"},
        {"стч", "щщ"},
        {"стьс", "сс"},
        {"сч", "щщ"},
        {"сш", "шш"},
        {"сщ", "щщ"},
        {"тц", "цц"},
        {"тч", "чч"},
        {"тщ", "чщ"},
        {"ться", "цца"},
        {"шч", "щщ"}
    };

    private Dictionary<string, string> end_replace = new Dictionary<string, string>()
    {
        {"тс", "ц"},
        {"тся", "цца"},
        {"ого", "ово"},
        {"его", "ево"}
    };
    

    public Phonetic(string word, int acc)
    {

        this.word = this.__work_word = word.ToLower().Trim();

        if (string.IsNullOrEmpty(this.word))
        {
            throw new System.Exception("Слово не передано.");
        }

        var _sylls = get_sylls();
        this.acc = _sylls != 0 ? acc : 0;

        if (this.acc != 0)
        {
            if (!(0 < this.acc && this.acc <= _sylls))
            {
                throw new System.Exception("Передан неверный номер слога.");
            }
        }
        
        replace_ends();
        
        replace_maps();

    }

    private void replace_ends()
    {
        foreach (var item in end_replace.OrderByDescending(x => x.Key.Length).ToList())
        {
            if (__work_word.EndsWith(item.Key))
            {
                this.__work_word = this.__work_word.Substring(0,__work_word.Length - item.Key.Length) + item.Value;
            }
        }
    }

    private void replace_maps()
    {
        foreach (var item in replace_map.OrderByDescending(x => x.Key.Length).ToList())
        {
            this.__work_word = this.__work_word.Replace(item.Key, item.Value);
        }
    }

    private int get_sylls()
    {
        int syll_counter = 0;
        foreach ( var letter in this.word)
        {
            if (_vowels.Contains(letter))
            {
                syll_counter++;
            }
        }

        return syll_counter;
    }

    public string get_phonetic()
    {
        var _sounds = new List<Letter>();
        Letter _last_let = null;
        var syll_counter = 0;
        foreach (var s in __work_word) {
            var is_shock = false;
            if (_vowels.Contains(s)) {
                syll_counter += 1;
                if (syll_counter == this.acc) {
                    is_shock = true;
                }
            }
            var let = new Letter(s.ToString(), _last_let, is_shock);
            _sounds.Add(let);
            _last_let = let;
            
        }
        _sounds.Last().initialize_as_end();
        
         return String.Join("",_sounds.Select( x => x.get_sound()).ToArray());
    }
}

public class Letter
{
    public string _letter = null;
    public Letter _prev_letter = null;
    public int _sonority_level = 0;
    public bool _forced_hard = false;
    public bool _forsed_sonorus = false;
    public bool _forced_not_show = false;
    public bool _is_double = false;
    public bool _is_shock = false;
    private bool _last_letter = false;
    
    public string vowels = "аеёиоуыэюя";  // Гласные буквы
    public string consonants = "бвгджзйклмнпрстфхцчшщ";  // Согласные буквы
    public string marks = "ъь";  // Знаки

    public string forever_hard = "жшц";  // Всегда твёрдые.
    public string forever_soft = "йчщ"; // Всегда мягкие.

    public string vovels_set_hard = "аоуыэ";  // Делают предыдущую согласную твёрдой.
    public string vovels_set_soft = "еёиюя"; // Делают предыдущую согласную мягкой.
    
    public Dictionary<string, string> ioted_vowels = new Dictionary<string, string> {
        {
            "е",
            "э"},
        {
            "ё",
            "о"},
        {
            "ю",
            "у"},
        {
            "я",
            "а"}};
            
    public string forever_sonorus = "йлмнр";
            
    public string forever_deaf = "xцчщ";
            
    public List<(string, string)> sonorus_deaf_pairs =  new List<(string, string)>{("б", "п"), ("в", "ф"), ("г", "к"), ("д", "т"), ("ж", "ш"), ("з", "с")};

    public Letter(string letter, Letter prev_letter = null, bool shock = false)
    {
        if (prev_letter != null) {
            if ((prev_letter.GetType() != typeof(Letter))) {
                throw new Exception(String.Format("Предыдущая буква должна быть объектом класса {0!r}, или None (передан тип {1!r}).",this.GetType(), prev_letter.GetType()));
            }
        }

        this._letter = letter.ToLower().Trim();
        this._prev_letter = prev_letter;
        if (this.letter().Length != 1) {
            throw new Exception("Передано неверное количество символов.");
        }
        if (!(this.is_vowel() || this.is_consonant() || this.is_mark())) {
            throw new Exception("Передана не буква русского языка.");
        }
        this._is_shock = shock && this.is_vowel();
       // this._forced_hard = false;
       // this._forsed_sonorus = false;
        this._forced_not_show = false;
        this._is_double = false;
        this.set_prev_sonorus();
        this.set_prev_hard();
        this.set_double_sound();
        // Последняя ли буква в слове.
        this._last_letter = false;
        // Определяем степень звучности для последующего деления на слоги.
        if (this.marks.Contains(this.letter())) {
            this._sonority_level = 0;
        } else if (this.vowels.Contains(this.letter())) {
            this._sonority_level = 4;
        } else if (this.forever_sonorus.Contains(this.letter())) {
            this._sonority_level = 3;
        } else if (this.sonorus_deaf_pairs.Select(x => x.Item1).Contains(this.letter())) {
            this._sonority_level = 2;
        } else {
            this._sonority_level = 1;
        }
    }

    private void set_double_sound()
    {
        var prev = this.get_prev_letter();
        if (prev == null) {
            return;
        }
        prev._forced_not_show = false;
        prev._is_double = false;
        this._is_double = false;
        prev.set_double_sound();
        if (this.is_consonant() && prev.is_consonant()) {
            if (this._get_sound() == prev._get_sound()) {
                prev._forced_not_show = true;
                prev._is_double = true;
                this._is_double = true;
            }
        }
    }

    private void set_prev_sonorus()
    {
        var prev = this.get_prev_letter();
        if (prev == null) {
            return;
        }
        if (!(this.is_consonant() && prev.is_consonant())) {
            return;
        }
        if (this.is_sonorus() && this.is_paired_consonant()) {
            if (this._get_sound(false) != "в") {
                prev.set_sonorus(true);
            }
            return;
        }
        if (this.is_deaf()) {
            prev.set_sonorus(false);
            return;
        }
    }

    private void set_prev_hard()
    {
        var prev = this.get_prev_letter();
        if (prev == null) {
            return;
        }
        if (!prev.is_consonant()) {
            return;
        }
        if (this.is_softener(prev)) {
            prev.set_hard(false);
        } else if (this.vovels_set_hard.Contains(this.letter())) {
            prev.set_hard(true);
        }
    }

    private bool is_after_acc()
    {
        var prev = this.prev_letter();
        while (true) {
            if (prev == null) {
                return false;
            }
            if (prev.is_shock()) {
                return true;
            }
            prev = prev.prev_letter();
        }
    }

    public string get_sound()
    {
        if (this.is_mark() || this._forced_not_show) {
            return "";
        }
        var snd = this._get_sound();
        if (this._is_double && this.is_after_acc()) {
            snd += ":";
        }
        return snd;
    }

    private string _get_sound(bool return_soft_mark = true)
    {
        string let;
        if (this.is_mark()) {
            return "";
        }
        var prev = this.prev_letter();
        var letterNow = this.letter();
        if (this.is_vowel()) {
            if (this.ioted_vowels.ContainsKey(letterNow)) {
                let = this.ioted_vowels[letterNow];
                if (prev == null || prev.is_vowel() || prev.is_mark()) {
                    letterNow = $"й'{let}";
                } else if (!this.is_shock()) {
                    letterNow = "и";
                } else {
                    letterNow = let;
                }
            }
            if (letterNow == "о") {
                if (!this.is_shock()) {
                    letterNow = "а";
                }
            }
            if (letterNow == "и" && prev != null) {
                if (prev.letter() == "ь") {
                    letterNow = "й'и";
                } else if (prev.forever_hard.Contains(prev._letter)) {
                    letterNow = "ы";
                }
            }
            return letterNow;
        }
        let = this.get_variant(this.is_deaf());
        if (return_soft_mark && this.is_soft()) {
            let += "'";
        }
        return let;
    }

    public void initialize_as_end()
    {
        this._last_letter = true;
        if (this.is_consonant()) {
            this.set_sonorus(false);
        }
    }

    private void set_hard(bool new_value)
    {
        if ((this.forever_hard + this.forever_soft).Contains(this.letter())) {
            return;
        }
        this._forced_hard = new_value;
        this.set_prev_hard();
    }

    private void set_sonorus(bool new_value)
    {
        this._forsed_sonorus = new_value;
        this.set_prev_sonorus();
    }

    public string letter()
    {
        return this._letter;
    }
    
    public int sonority_level() {
        return this._sonority_level;
    }

    private Letter get_prev_letter()
    {
        var prev = this.prev_letter();
        while (true) {
            if (prev == null) {
                return null;
            }
            if (prev.marks.Contains(prev.letter())) {
                prev = prev.prev_letter();
                continue;
            }
            return prev;
        }
    }

    private Letter prev_letter()
    {
        return this._prev_letter;
    }
    public string get_variant(bool return_deaf) {
        
        foreach (var variants in this.sonorus_deaf_pairs) {
            if (variants.Item1 == this.letter()|| variants.Item2 == this.letter())
            {
                return return_deaf ? variants.Item2 : variants.Item1;
            }
        }
        return this.letter();
    }
    public bool is_paired_consonant() {
        if (!this.is_consonant()) {
            return false;
        }
        foreach (var variants in this.sonorus_deaf_pairs) {
            if (variants.Item1 == this.letter()|| variants.Item2 == this.letter())
            {
                return true;
            }
        }
        return false;
    }
            
    // 
    //         Звонкая ли согласная.
    //         
    public bool is_sonorus() {
        if (!this.is_consonant()) {
            return false;
        }
        if (this.forever_sonorus.Contains(this.letter())) {
            return true;
        }
        if (this.forever_deaf.Contains(this.letter())) {
            return false;
        }
        if (this._forsed_sonorus) {
            return true;
        }
        if (!_forsed_sonorus) {
            return false;
        }
        foreach (var _tup_1 in this.sonorus_deaf_pairs) {
            var son = _tup_1.Item1;
            if (this.letter() == son) {
                return true;
            }
        }
        return false;
    }

    public bool is_deaf() {
        if (!this.is_consonant()) {
            return false;
        }
        if (this.forever_deaf.Contains(this.letter())) {
            return true;
        }
        if (this.forever_sonorus.Contains(this.letter())) {
            return false;
        }
        if (this._forsed_sonorus) {
            return false;
        }
        if (!this._forsed_sonorus) {
            return true;
        }
        foreach (var tup1 in this.sonorus_deaf_pairs) {
            var df = tup1.Item2;
            if (this.letter() == df) {
                return true;
            }
        }
        return false;
    }
    public bool is_hard() {
        if (!this.is_consonant()) {
            return false;
        }
        if (this.forever_hard.Contains(this.letter())) {
            return true;
        }
        if (this.forever_soft.Contains(this.letter())) {
            return false;
        }
        if (this._forced_hard) {
            return true;
        }
        return false;
    }
            
    public bool is_soft() {
        if (!this.is_consonant()) {
            return false;
        }
        if (this.forever_soft.Contains(this.letter())) {
            return true;
        }
        if (this.forever_hard.Contains(this.letter())) {
            return false;
        }
        if (!_forced_hard) {
            return true;
        }
        return false;
    }
            
    // 
    //         Проверяет, заканчивается ли последовательность букв переданной строкой.
    //         Скан производится, без учёта текущей.
    //         
    public bool end(string s) {
        var prev = this.prev_letter();
        foreach (char i in new string(s.Reverse().ToArray())) {
            if (!prev.letter().Equals(i.ToString())) {
                return false;
            }
            if (prev == null) {
                return false;
            }
            prev = prev.prev_letter();
        }
        return true;
    }
    
    public bool is_softener(Letter let) {
        if (let.forever_hard.Contains(let.letter())) {
            return false;
        }
        if (!let.is_consonant()) {
            return false;
        }
        if (this.vovels_set_soft.Contains(this.letter())) {
            return true;
        }
        if (this.letter() == "ь") {
            return true;
        }
        if (this.is_soft() && "дзнст".Contains(let.letter())) {
            return true;
        }
        if (this.letter() == "ъ") {
            if (this.end("раз") || this.end("из") || this.end("с")) {
                return true;
            }
        }
        return false;
    }

    public bool is_vowel() {
        return this.vowels.Contains(this.letter());
    }

    public bool is_consonant()
    {
        return this.consonants.Contains(this.letter());
    }
    
    public bool is_mark() {
        return this.marks.Contains(this.letter());
    }
            
    public bool is_shock() {
        return this._is_shock;
    }

}