using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public int currentPage = 0;
    public GameObject previousButton, nextButton;
    public GameObject[] menus, pages;
    public AudioMixer mixer;
    public AudioSource bgm, ui;
    public AudioClip music, ingameMusic, click, coin;
    public Slider uiValue, instrumentValue, musicValue;
    public TextMeshProUGUI uiPercent, instrumentPercent, musicPercent;

    private bool aa = false;
    private float startVolume;

    void Update() {
        if(uiPercent.gameObject.activeInHierarchy) {
            uiPercent.text = (uiValue.value + 80) + "%";
            instrumentPercent.text = (instrumentValue.value + 80) + "%";
            musicPercent.text = (musicValue.value + 80) + "%";
        }
    }

    void Start() {
        bgm.volume = 0f;
        StartCoroutine(StartVolume());
    }

    private IEnumerator StartVolume() {
        
        while(bgm.volume < 0.054f) {
            bgm.volume += 0.002f;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void ShowMenu(GameObject menuObject) {
        foreach(GameObject o in menus)
            if(o != menuObject)
                o.SetActive(false);

        menuObject.SetActive(true);
        if(menuObject.name == "How2PlayMenu") {
            currentPage = 0;
            UpdatePages(currentPage);
        }

        if(menuObject.name == "GameGUI") {
            bgm.volume = 0.083f;
            bgm.Stop();
            bgm.clip = ingameMusic;
            bgm.Play();
            aa = true;
        } else {
            if(aa) {
                aa = false;
                bgm.volume = 0f;
                StartCoroutine(StartVolume());
                bgm.Stop();
                bgm.clip = music;
                bgm.Play();
            }
        }
    }

    public void NextPage() {
        if(currentPage + 1 >= pages.Length) {
            ShowMenu(menus[0]);
            currentPage = 0;
        } else {
            currentPage++;
            UpdatePages(currentPage);
        }
    }

    public void PreviousPage() {
        if(currentPage < pages.Length) {
            currentPage--;
            UpdatePages(currentPage);
        }
    }

    public void ClickSound() {
        //0.37 - 1.18
        ui.pitch = Random.Range(0.37f, 1.19f);
        ui.PlayOneShot(click); 
    }

    public void UpdateUIVolume(float value) {
        mixer.SetFloat("uiVolume", value);
    }

    public void UpdateInstrumentsVolume(float value) {
        mixer.SetFloat("instrumentVolume", value);
    }

    public void UpdateMusicVolume(float value) {
        mixer.SetFloat("musicVolume", value);
    }

    private void UpdatePages(int page) {
        foreach(GameObject obj in pages)
            obj.SetActive(false);
        pages[page].SetActive(true);

        previousButton.SetActive(currentPage != 0);
    }

    public void Exit() => Application.Quit();


}
