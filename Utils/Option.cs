using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Option", menuName = "Instrument")]
public class Option : ScriptableObject {

    public Instrument instrument;
    public AudioClip sound;
    public string instrumentName;

    public Sprite image;
}
