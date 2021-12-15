using System.Collections;
using System.Linq;
using UnityEngine;

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
   public GameObject[] Blocker;

   int[] SmallNumbers = { 1, 2, 3 }; //One sound from 1-3 Decibels
   int[] MediumNumbers = { 4, 5, 6, 7, 8, 9 }; //Two sounds from 4-9 Decibels
   int[] LargeNumbers = { 10, 11, 12, 13, 14, 15 }; //One sound from 10-15 Decibels
   int[] PossibleButtons = { 0, 1, 2, 3 };
   int[] VolumeHolders = new int[4];
   int[] VolumeComparison = new int[4];
   int[] Submission = new int[4];
   int DecibelReader;
   int ButtonPressCount;

   string[] FuckYouDeaf = { "Lowest", "SecondLowest", "SecondHighest", "Highest" };
   string[] ColorNames = { "Blue", "Green", "Red", "Yellow" };

   bool[] PreviouslyPressed = new bool[4];
   bool Positivity = true;
   bool SubmissionTime;
   bool ColorblindENGAGE;

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
      Deciblometer.text = DecibelReader.ToString() + ".0 db";
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
            if (moduleSolved) {
               Audio.PlaySoundAtTransform(FuckYouDeaf[VolumeComparison[i]], transform);
               return;
            }
            else if (PreviouslyPressed[i]) {
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
                  Audio.PlaySoundAtTransform(FuckYouDeaf[VolumeComparison[i]], transform);
                  if (DecibelReader < 10) {
                     Deciblometer.text = "0" + DecibelReader.ToString() + ".0 db";
                  }
                  else {
                     Deciblometer.text = DecibelReader.ToString() + ".0 db";
                  }
                  ButtonPressCount++;
                  Debug.LogFormat("[Silenced Simon #{0}] After pressing {1}, you are at {2}.", moduleId, ColorNames[PossibleButtons[i]], DecibelReader);
               }
               else {
                  Audio.PlaySoundAtTransform(FuckYouDeaf[VolumeComparison[i]], transform);
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
      if (ButtonPressCount % 10 == 0 && ButtonPressCount != 0) {
         Blocker[0].gameObject.SetActive(true);
         Blocker[1].gameObject.SetActive(false);
      }
      else if (ButtonPressCount % 5 == 0) {
         Blocker[0].gameObject.SetActive(false);
         Blocker[1].gameObject.SetActive(false);
      }
      else {
         Blocker[0].gameObject.SetActive(false);
         Blocker[1].gameObject.SetActive(true);
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
               moduleSolved = true;
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
      switch (UnityEngine.Random.Range(0, 5)) {
         case 0:
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
         case 1:
            while (true) {
               BamboozledAgainFanfare[3].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[2].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[5].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[6].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[4].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[1].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[0].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               Iteration++;
            }
         case 2:
            while (true) {
               BamboozledAgainFanfare[0].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[2].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[3].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[1].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[4].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[6].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[5].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               Iteration++;
            }
         case 3:
            while (true) {
               BamboozledAgainFanfare[0].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[1].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[2].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[3].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[4].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[5].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[6].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               Iteration++;
            }
         case 4:
            while (true) {
               BamboozledAgainFanfare[0].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[1].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[3].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[2].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[5].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[6].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               BamboozledAgainFanfare[4].GetComponent<MeshRenderer>().material = IDontKnowIfINeedThis[Iteration % 2];
               yield return new WaitForSeconds(.1f);
               Iteration++;
            }
      }
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} R/B/G/A/Y/W/K to press that corresponding button in reading order.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      Command = Command.Trim().ToUpper();
      yield return null;
      if (!Command.Any(AVariable => "RBGAYWK".Contains(AVariable))) {
         yield return "sendtochaterror I don't understand!";
      }
      else {
         for (int i = 0; i < Command.Length; i++) {
            switch (Command[i]) {
               case 'B':
                  for (int j = 0; j < 4; j++) {
                     if (PossibleButtons[j] == 0) {
                        Buttons[j].OnInteract();
                     }
                  }
                  break;
               case 'G':
                  for (int j = 0; j < 4; j++) {
                     if (PossibleButtons[j] == 1) {
                        Buttons[j].OnInteract();
                     }
                  }
                  break;
               case 'R':
                  for (int j = 0; j < 4; j++) {
                     if (PossibleButtons[j] == 2) {
                        Buttons[j].OnInteract();
                     }
                  }
                  break;
               case 'Y':
                  for (int j = 0; j < 4; j++) {
                     if (PossibleButtons[j] == 3) {
                        Buttons[j].OnInteract();
                     }
                  }
                  break;
               case 'A':
                  OtherButtons[0].OnInteract();
                  break;
               case 'W':
                  OtherButtons[1].OnInteract();
                  break;
               case 'K':
                  OtherButtons[2].OnInteract();
                  break;
            }
            yield return new WaitForSeconds(.1f);
         }
      }
   }

   IEnumerator TwitchHandleForcedSolve () {
      while (!moduleSolved) {
         if (!SubmissionTime) {
            OtherButtons[0].OnInteract();
         }
         Restart:
         for (int i = 0; i < 4; i++) {
            if (Submission[i] < VolumeHolders[i] && !PreviouslyPressed[i]) {
               if (!Positivity) {
                  OtherButtons[1].OnInteract();
               }
               Buttons[i].OnInteract();
            }
            else if (Submission[i] > VolumeHolders[i] && !PreviouslyPressed[i]) {
               if (Positivity) {
                  OtherButtons[2].OnInteract();
               }
               Buttons[i].OnInteract();
            }
            yield return new WaitForSeconds(.1f);
            int QuickCheck = 0;
            for (int j = 0; j < 4; j++) {
               if (Submission[j] == VolumeHolders[j]) {
                  QuickCheck++;
               }
            }
            if (QuickCheck == 3) {
               int Temp2 = 0;
               do {
                  Temp2 = UnityEngine.Random.Range(0, 4);
               } while (PreviouslyPressed[Temp2]);
               Buttons[Temp2].OnInteract();
               yield return new WaitForSeconds(.1f);
            }
         }
         bool Check = true;
         for (int i = 0; i < 4; i++) {
            if (Submission[i] != VolumeHolders[i]) {
               Check = false;
            }
         }
         if (Check) {
            OtherButtons[0].OnInteract();
         }
         else {
            goto Restart;
         }
      }
   }
}
