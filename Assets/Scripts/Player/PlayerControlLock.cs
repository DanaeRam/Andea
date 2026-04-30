using UnityEngine;

public class PlayerControlLock : MonoBehaviour
{
    public bool canMove = true;
    public bool canAttack = true;
    public bool isInQuiz = false;

    public void LockForQuiz()
    {
        canMove = false;
        canAttack = false;
        isInQuiz = true;
    }

    public void LockFailedAnswer()
    {
        canMove = false;
        canAttack = false;
        isInQuiz = true;
    }

    public void UnlockAfterQuiz()
    {
        canMove = true;
        canAttack = true;
        isInQuiz = false;
    }

    public void ForceUnlock()
    {
        canMove = true;
        canAttack = true;
        isInQuiz = false;
    }
}