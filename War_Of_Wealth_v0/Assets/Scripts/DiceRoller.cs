using UnityEngine;
using DG.Tweening;
using System.Collections;

public class DiceRoller : MonoBehaviour
{
    public Dice diceA;
    public Dice diceB;

    public float delayBeforeTotal = 0.1f;

    public IEnumerator RollDice(System.Action<int> onComplete)
    {
        int rollA = Random.Range(1, 7);
        int rollB = Random.Range(1, 7);

        // Animate both dice
        Tween t1 = diceA.Roll(rollA);
        Tween t2 = diceB.Roll(rollB);

        // Wait for both animations to finish
        yield return t1.WaitForCompletion();
        yield return t2.WaitForCompletion();

        yield return new WaitForSeconds(delayBeforeTotal);

        int total = rollA + rollB;
        onComplete?.Invoke(total);
    }
}
