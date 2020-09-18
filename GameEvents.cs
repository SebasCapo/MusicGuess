using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEvents : MonoBehaviour {

    public static GameEvents instance;
    public static AnimatedText animatedText;
    public static HelpMode mode = HelpMode.IMAGE;

    [Header("--- Dynamic Variables")]
    public Instrument chosenInstrument = Instrument.NONE;
    public Instrument[] displayInstruments;
    public RestartMode rmode = RestartMode.NONE;
    public uint points = 0, accumulated = 0, tempAccumulated, finalPoints;
    public bool letPlay, starsRun, hasWon, isIntroEnabled;

    [Header("--- Setup Variables")]
    public TMP_InputField guess;
    public Sprite specialSprite, mysteryBox;
    public TextMeshProUGUI pointsText, hintText, accumulatedText, finalPointsText, startsWith, debugList;
    public ParticleSystem system, finalSystem;
    public AudioSource audioSource;
    public Color mysteryBoxColor, defaultDisplayColor;
    public float musicalSpeed = 1.5f, rotateSpeed = 1.5f, pauseBetweenLetters = 0.1f, pauseBetweenStars = 0.05f, pauseBetweenStarsFinal = 0.01f;
    public OptionDisplay[] displays;
    public Option[] options;
    public Animator display1, display2, display3, buttons, failDialog, successDialog, finalDialog;
    public MainMenu menu;
    public GameObject gameUI, debug;

    private List<Instrument> selectedInstruments, poolInstance, oldInstruments;

    public void Awake() {
        instance = this;
        selectedInstruments = new List<Instrument>();
        animatedText = GetComponent<AnimatedText>();

        if(displays == null)
            displays = FindObjectsOfType<OptionDisplay>();

        displayInstruments = options.Select(i => i.instrument).ToArray();
        string a = "";
        foreach(Instrument i in displayInstruments)
            a += $"{i} ";
        Debug.Log("Copied instruments from available options: " + a);

        points = 0; accumulated = 0;

        poolInstance = displayInstruments.ToList();
        oldInstruments = displayInstruments.ToList();

        Setup();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.F1) && Input.GetKey(KeyCode.LeftShift)) {
            Awake();
            Debug.Log("Used Awake() admin command.");
        } else if(Input.GetKeyDown(KeyCode.F2) && Input.GetKey(KeyCode.LeftShift)) {
            Setup();
            Debug.Log("Used Setup() admin command.");
        } else if(Input.GetKeyDown(KeyCode.F3) && Input.GetKey(KeyCode.LeftShift)) {
            Reveal();
            Debug.Log("Used Reveal() admin command.");
        } else if(Input.GetKeyDown(KeyCode.F4) && Input.GetKey(KeyCode.LeftShift)) {
            letPlay = !letPlay;
            Debug.Log("Used !letPlay admin command.");
        } else if(Input.GetKeyDown(KeyCode.F5) && Input.GetKey(KeyCode.LeftShift)) {
            guess.text = chosenInstrument.ToString();
            Guess();
            Debug.Log("Used Guess() (cheat) admin command.");
        } else if(Input.GetKeyDown(KeyCode.F6) && Input.GetKey(KeyCode.LeftShift)) {
            string a = "Removed: ";
            for(int i = 0; i < oldInstruments.Count; i++) {
                if(oldInstruments.Count > 3) {
                    a += oldInstruments[i];
                    oldInstruments.Remove(oldInstruments[i]);
                }
            }
            Debug.Log("Used Remove admin command. " + a);
        } else if(Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Q)) {
            guess.text = chosenInstrument.ToString();
            Debug.Log("Used MEGA (cheat) admin command.");
        } else if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F12)) {
            debug.SetActive(!debug.activeInHierarchy);
            Debug.Log("Used Debug admin command.");
        } else if(Input.GetKeyDown(KeyCode.Return) && gameUI.activeInHierarchy && isIntroEnabled) {
            Guess();
        }

        if(debug.activeInHierarchy) {
            // private List<Instrument> selectedInstruments, poolInstance, oldInstruments;
            string slc = "Selected Instruments:\n", pin = "PoolInstance:\n", oin = "OldInstruments:\n", optionst = "Options:\n";
            int sln = 0, pinn = 0, oinn = 0, onn = 0;
            foreach(Instrument i in selectedInstruments) {
                slc += $"{i} - ";
                sln++;
            }

            foreach(Instrument i in poolInstance) {
                pin += $"{i} - ";
                pinn++;
            }

            foreach(Instrument i in oldInstruments) {
                oin += $"{i} - ";
                oinn++;
            }

            foreach(Option i in options) {
                optionst += $"{i.instrument} - ";
                onn++;
            }

            debugList.text = $"<color=#B22D2D>({sln}) {slc}</color>\n<color=#EC9627>({pinn}) {pin}</color>\n<color=#79BE21>({oinn}) {oin}</color>\n<color=#10ABFF>({onn}) {optionst}</color>";
        }

        if(letPlay) {
            if(!system.isPlaying) {
                system.Play();
            }
        } else {
            if(system.isPlaying) {
                system.Stop();
            }
        }
    }

    public void Setup() {
        try {
            selectedInstruments.Clear();
            rmode = RestartMode.NONE;
            mode = HelpMode.IMAGE;
            hasWon = false;
            accumulated = 100;
            startsWith.text = "";
            poolInstance = oldInstruments.ToList();
            StartCoroutine(ToggleIntro());

            foreach(OptionDisplay display in displays) {
                Instrument r = GetRandomInstrument();
                if(r == Instrument.NONE) {
                    Option o = ScriptableObject.CreateInstance<Option>();
                    o.image = mysteryBox;
                    o.instrumentName = "";
                    o.instrument = Instrument.NONE;
                    display.option = o;
                } else {
                    selectedInstruments.Add(r);
                    display.option = options.GetOption(r);
                }
                display.Setup();
            }

            chosenInstrument = selectedInstruments.GetRandomInstrument();
        } catch(Exception) {

        }

    }

    public void Reveal() {
        starsRun = false;

        foreach(OptionDisplay display in displays)
            display.Reveal(mode);

        switch(mode) {
            case HelpMode.END:
                if(accumulated > 60) {
                    accumulated -= 20;
                    startsWith.text = $"Instrument's Name\nStarts with: {chosenInstrument.ToString()[0]}";
                }
                return;
            case HelpMode.TEXT:
                accumulated -= 20;
                mode = HelpMode.END;
                break;
            case HelpMode.IMAGE:
                accumulated -= 20;
                mode = HelpMode.TEXT;
                break;
        }
    }

    public Instrument GetRandomInstrument() {
        Instrument i;
        try {
            i = poolInstance[UnityEngine.Random.Range(0, poolInstance.Count())];
            poolInstance.Remove(i);
            Debug.Log("GetRandomInstrument() " + i);
        } catch(Exception) {
            i = Instrument.NONE;
        }
        return i;
    }

    //     public Animator display1, display2, display3, buttons, failDialog, successDialog
    public void Guess() {
        try {
            isIntroEnabled = false;
            if(guess.text.ToUpper().Replace(" ", "") != chosenInstrument.ToString()) {
                failDialog.Play("Showup");
                rmode = RestartMode.FAIL;
            } else {
                successDialog.Play("Showup");
                rmode = RestartMode.CORRECT;

                accumulatedText.text = $"+{accumulated}";
                hasWon = true;

                oldInstruments.Remove(chosenInstrument);
            }

            guess.text = "";

            display1.Play("FadeOut1");
            display2.Play("FadeOut2");
            display3.Play("FadeOut3");
            buttons.Play("ButtonFadeOut");
        } catch(Exception) {

        }
    }

    public void Hint()
        => PlaySelectedSound();/*hintText.text = $"Starts with: {chosenInstrument.ToString()[0]}";*/

    public void Restart() {
        try {
            switch(rmode) {
                case RestartMode.FAIL:
                    failDialog.Play("Showdown");
                    break;
                case RestartMode.CORRECT:
                    successDialog.Play("Showdown");
                    break;
            }
            if(oldInstruments.Count == 0) {
                finalDialog.Play("Showup");
                StartCoroutine(PlayFinalEffect());
                points += accumulated;
                return;
            }

            display1.Play("FadeIn1");
            display2.Play("FadeIn2");
            display3.Play("FadeIn3");
            buttons.Play("ButtonFadeIn");

            if(hasWon) {
                StartCoroutine(PlayEffect());
                tempAccumulated = accumulated;
            }

            Setup();
        } catch(Exception) {

        }
    }

    public void FixJustInCase() {
        failDialog.Play("Fix");
        successDialog.Play("Fix");
        finalDialog.Play("Fix");
    }

    public void ShowdownFinal() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void PlaySelectedSound() => audioSource.PlayOneShot(options.GetOption(chosenInstrument).sound);

    private IEnumerator ToggleIntro() {
        yield return new WaitForSeconds(3f);
        isIntroEnabled = true;
    }

    private IEnumerator PlayEffect() {
        yield return new WaitForSeconds(2.2f);
        StartCoroutine(UpdatePoints());
    }

    private IEnumerator UpdatePoints() {
        for(int i = 0; i < tempAccumulated; i++) {
            points++;
            pointsText.text = points + "";
            if(i == tempAccumulated - 1)
                system.Emit(100);
            yield return new WaitForSeconds(pauseBetweenStars);
        }
    }
    private IEnumerator PlayFinalEffect() {
        yield return new WaitForSeconds(2.2f);
        StartCoroutine(UpdateFinalPoints());
    }

    private IEnumerator UpdateFinalPoints() {
        for(int i = 0; i < points; i++) {
            finalPoints += 5;
            if(finalPoints > points)
                finalPoints = points;
            finalPointsText.text = finalPoints + "";
            if(i >= points - 3)
                finalSystem.Emit(100);
            yield return 0;
            yield return new WaitForSeconds(pauseBetweenStarsFinal);
        }
    }

    public void Cheat() {
        if(Input.GetKey(KeyCode.AltGr))
            guess.text = chosenInstrument.ToString();
    }

    public event Action ShowHint;

    public void NoteRestart() {
        ShowHint?.Invoke();
    }
}
