using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class AnimatedText : MonoBehaviour {

    public void Animate(TextMeshProUGUI textComp, string msg) {
        textComp.text = "";
        StartCoroutine(TypeText(textComp, msg));
    }

    private IEnumerator TypeText(TextMeshProUGUI textComp, string msg) {
        foreach ( char letter in msg.ToCharArray() ) {
            textComp.text += letter;
            yield return 0;
            yield return new WaitForSeconds(GameEvents.instance.pauseBetweenLetters);
        }
    }
}
