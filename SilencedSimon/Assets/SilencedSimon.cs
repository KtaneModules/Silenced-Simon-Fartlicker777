using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class SilencedSimon : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMColorblindMode Blind;
    public KMSelectable[] Buttons;//red blue green grey yellow white black
    public KMSelectable[] OtherButtons;
    public TextMesh Deciblometer;
    public GameObject[] SimonsSegments;
    public Material[] Colors;
    public GameObject[] BamboozledAgainFanfare;
    public Material[] IDontKnowIfINeedThis;
    public TextMesh[] RB_GAY_WK;
    public GameObject Blocker;

    int[] SmallNumbers = {1, 2, 3}; //One sound from 1-3 Decibels
    int[] MediumNumbers = {4, 5, 6, 7, 8, 9}; //Two sounds from 4-9 Decibels
    int[] LargeNumbers = {10, 11, 12, 13, 14, 15}; //One sound from 10-15 Decibels
    int[] PossibleButtons = {0, 1, 2, 3};
    int[] VolumeHolders = new int[4];
    int[] VolumeComparison = new int[4];
    int[] Submission = new int[4];
    int DecibelReader;

    string[] FuckYouDeaf = {"Lowest", "SecondLowest", "SecondHighest", "Highest"};
    string[] ColorNames = {"Blue", "Green", "Red", "Yellow"};

    bool[] PreviouslyPressed = {false, false, false, false};
    bool Positivity = true;
    bool SubmissionTime = false;
    bool ColorblindENGAGE;
    bool LeftMover = false;
    bool DownMover = false;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable Button in Buttons) {
            Button.OnInteract += delegate () { ButtonPress(Button); return false; };
        }
        foreach (KMSelectable Button in OtherButtons) {
            Button.OnInteract += delegate () { OtherButtonPress(Button); return false; };
        }
    }

    void Start () {
      if (Blind.ColorblindModeActive) {
        ColorblindENGAGE = true;
      }
      DecibelReader = UnityEngine.Random.Range(60, 70);
      Debug.LogFormat("[Silenced Simon #{0}] It starts at {1}.", moduleId, DecibelReader);
      PossibleButtons.Shuffle();
      for (int i = 0; i < 4; i++) {
        SimonsSegments[i].GetComponent<MeshRenderer>().material = Colors[PossibleButtons[i]];
      }
      SmallNumbers.Shuffle();
      MediumNumbers.Shuffle();
      LargeNumbers.Shuffle();
      VolumeHolders[0] = SmallNumbers[0];
      VolumeHolders[1] = MediumNumbers[0];
      VolumeHolders[2] = MediumNumbers[1];
      VolumeHolders[3] = LargeNumbers[0];
      VolumeHolders.Shuffle();
      if (ColorblindENGAGE) {
        for (int i = 0; i < 7; i++) {
          if (i == 3) {
            RB_GAY_WK[3].text = "A";
          }
          else if (i == 5) {
            RB_GAY_WK[5].text = "W";
          }
          else if (i == 6) {
            RB_GAY_WK[6].text = "K";
          }
          else if (i == 4) {
            RB_GAY_WK[4].text = ColorNames[PossibleButtons[i - 1]][0].ToString();
          }
          else {
            RB_GAY_WK[i].text = ColorNames[PossibleButtons[i]][0].ToString();
          }
        }
      }
      else {
        for (int i = 0; i < 7; i++) {
          RB_GAY_WK[i].text = "";
        }
      }
      Debug.LogFormat("[Silenced Simon #{0}] Colors are {1}, {2}, {3}, {4}.", moduleId, ColorNames[PossibleButtons[0]], ColorNames[PossibleButtons[1]], ColorNames[PossibleButtons[2]], ColorNames[PossibleButtons[3]]);
      Debug.LogFormat("[Silenced Simon #{0}] They have values of {1}, {2}, {3}, {4}.", moduleId, VolumeHolders[0], VolumeHolders[1], VolumeHolders[2], VolumeHolders[3]);
      SortValues();
    }

    void ButtonPress (KMSelectable Button) {
      Button.AddInteractionPunch();
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Button.transform);
      for (int i = 0; i < Buttons.Length; i++) {
        if (Button == Buttons[i]) {
          if (PreviouslyPressed[i]) {
            Audio.PlaySoundAtTransform(FuckYouDeaf[VolumeComparison[i]], transform);
            GetComponent<KMBombModule>().HandleStrike();
            return;
          }
          else {
            for (int j = 0; j < 4; j++) {
              PreviouslyPressed[j] = false;
            }
            PreviouslyPressed[i] = true;
            if (!SubmissionTime) {
              if (Positivity) {
                DecibelReader += VolumeHolders[i];
                if (DecibelReader > 99) {
                  DecibelReader -= VolumeHolders[i];
                  GetComponent<KMBombModule>().HandleStrike();
                }
              }
              else {
                DecibelReader -= VolumeHolders[i];
                if (DecibelReader < 0) {
                  DecibelReader += VolumeHolders[i];
                  GetComponent<KMBombModule>().HandleStrike();
                }
              }
              Debug.Log(VolumeComparison[i]);
              Audio.PlaySoundAtTransform(FuckYouDeaf[VolumeComparison[i]], transform);
              if (DecibelReader < 10) {
                Deciblometer.text = "0" + DecibelReader.ToString() + ".0 db";
              }
              else {
                Deciblometer.text = DecibelReader.ToString() + ".0 db";
              }
              Debug.LogFormat("[Silenced Simon #{0}] After pressing {1}, you are at {2}.", moduleId, ColorNames[PossibleButtons[i]], DecibelReader);
            }
            else {
              if (Positivity) {
                Submission[i]++;
                if (i != 3) {
                  RB_GAY_WK[i].text = Submission[i].ToString();
                }
                else {
                  RB_GAY_WK[4].text = Submission[i].ToString();
                }
              }
              else {
                Submission[i]--;
                if (i != 3) {
                  RB_GAY_WK[i].text = Submission[i].ToString();
                }
                else {
                  RB_GAY_WK[4].text = Submission[i].ToString();
                }
              }
            }
          }
        }
      }
    }

    void OtherButtonPress (KMSelectable Button) {
      Button.AddInteractionPunch();
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Button.transform);
      if (Button == OtherButtons[0]) {
        if (SubmissionTime) {
          bool Valid = true;
          for (int i = 0; i < 4; i++) {
            if (Submission[i] != VolumeHolders[i]) {
              Valid = false;
            }
          }
          if (Valid) {
            GetComponent<KMBombModule>().HandlePass();
            for (int i = 0; i < 7; i++) {
              RB_GAY_WK[i].text = "";
            }
            StartCoroutine(SolveAnimation());
          }
          else {
            Debug.LogFormat("[Silenced Simon #{0}] You submitted {1}, {2}, {3}, {4}.", moduleId, Submission[0], Submission[1], Submission[2], Submission[3]);
            GetComponent<KMBombModule>().HandleStrike();
            for (int i = 0; i < 4; i++) {
              Submission[i] &= 0;
            }
            for (int i = 0; i < 7; i++) {
              RB_GAY_WK[i].text = "";
            }
            SubmissionTime = false;
          }
        }
        else {
          for (int i = 0; i < 3; i++) {
            RB_GAY_WK[i].text = "0";
          }
          RB_GAY_WK[4].text = "0";
          SubmissionTime = true;
        }
      }
      else if (Button == OtherButtons[1]) {
        Positivity = true;
      }
      else if (Button == OtherButtons[2]) {
        Positivity = false;
      }
    }

    void SortValues () {
      int Highest = 0;
      int SecondHighest = 0;
      int SecondLowest = 0;
      for (int i = 0; i < 4; i++) {
        if (VolumeHolders[i] > Highest) {
          Highest = VolumeHolders[i];
        }
      }
      for (int i = 0; i < 4; i++) {
        if (Highest == VolumeHolders[i]) {
          VolumeComparison[i] = 3;
        }
      }
      for (int i = 0; i < 4; i++) {
        if (VolumeHolders[i] > SecondHighest && VolumeHolders[i] != Highest) {
          SecondHighest = VolumeHolders[i];
        }
      }
      for (int i = 0; i < 4; i++) {
        if (SecondHighest == VolumeHolders[i]) {
          VolumeComparison[i] = 2;
        }
      }
      for (int i = 0; i < 4; i++) {
        if (VolumeHolders[i] > SecondLowest && VolumeHolders[i] != Highest && VolumeHolders[i] != SecondHighest) {
          SecondLowest = VolumeHolders[i];
        }
      }
      for (int i = 0; i < 4; i++) {
        if (SecondLowest == VolumeHolders[i]) {
          VolumeComparison[i] = 1;
        }
      }
    }

    IEnumerator SolveAnimation () {
      int Iteration = 0;
      while (true) {
        BamboozledAgainFanfare[0].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
        yield return new WaitForSeconds(.1f);
        BamboozledAgainFanfare[1].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
        yield return new WaitForSeconds(.1f);
        BamboozledAgainFanfare[4].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
        yield return new WaitForSeconds(.1f);
        BamboozledAgainFanfare[6].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
        yield return new WaitForSeconds(.1f);
        BamboozledAgainFanfare[5].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
        yield return new WaitForSeconds(.1f);
        BamboozledAgainFanfare[2].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
        yield return new WaitForSeconds(.1f);
        BamboozledAgainFanfare[3].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
        yield return new WaitForSeconds(.1f);
        Iteration++;
      }
    }

    IEnumerator MoveLeft () {
      LeftMover = true;
      for (int i = 0; i < 4; i++) {
        Blocker.transform.Translate(-0.00255f, 0f, 0f);
        yield return new WaitForSeconds(.01f);
      }
    }

    IEnumerator MoveDown () {
      DownMover = true;
      for (int i = 0; i < 4; i++) {
        Blocker.transform.Translate(0f, 0f, 0.02986f);
        yield return new WaitForSecondsRealtime(.01f);
      }
    }

    void Update () {
      if (Bomb.GetStrikes() > 2 && !DownMover) {
        StartCoroutine(MoveDown());
      }
      else if (Bomb.GetStrikes() == 2 && !LeftMover) {
        StartCoroutine(MoveLeft());
      }
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} ######## to press that corresponding button in reading order.";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand (string Command) {
      Command = Command.Trim();
      yield return null;
      if (!(Command.Any(AVariable => "1234567".Contains(AVariable)))) {
        yield return "sendtochaterror I don't understand!";
      }
      else {
        yield return null;
        for (int i = 0; i < Command.Length; i++) {
          switch (Command[i]) {
            case '1':
            Buttons[0].OnInteract();
            yield return new WaitForSeconds(.1f);
            break;
            case '2':
            Buttons[1].OnInteract();
            yield return new WaitForSeconds(.1f);
            break;
            case '3':
            Buttons[2].OnInteract();
            yield return new WaitForSeconds(.1f);
            break;
            case '4':
            OtherButtons[0].OnInteract();
            yield return new WaitForSeconds(.1f);
            break;
            case '5':
            Buttons[3].OnInteract();
            yield return new WaitForSeconds(.1f);
            break;
            case '6':
            OtherButtons[1].OnInteract();
            yield return new WaitForSeconds(.1f);
            break;
            case '7':
            OtherButtons[2].OnInteract();
            yield return new WaitForSeconds(.1f);
            break;
          }
        }
      }
    }
    /*
    IEnumerator TwitchHandleForcedSolve () {
      yield return null;
    }*/
}
