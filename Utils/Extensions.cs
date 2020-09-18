using System.Collections.Generic;
using System.Data;
using UnityEngine;

public static class Extensions {

    public static void RandomizeSize(this Transform transform) {
        float r = Random.Range(0.58f, 1.23f);
        transform.localScale = new Vector3(r, r, r);
    }

    public static Option GetOption(this List<Option> options, Instrument instrument) {
        foreach(Option option in options)
            if(option.instrument == instrument)
                return option;
        return null;
    }

    public static void Trigger(this Animator anim, string triggerName) {
        anim.ResetTrigger(triggerName);
        anim.SetTrigger(triggerName);
    }

    public static Option GetOption(this Option[] options, Instrument instrument) {
        foreach(Option option in options)
            if(option.instrument == instrument)
                return option;
        return null;
    }

    public static Instrument GetRandomInstrument(this List<Instrument> list) =>
        list[Random.Range(0, list.Count)];

    public static void RandomizeRotation(this Transform transform) =>
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-27, 50));

    public static void RotateTowards(this Transform transform, float towards, float speed)
        => transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, towards), Time.deltaTime * speed);
}
