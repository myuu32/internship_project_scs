using UnityEngine;

public class SelectMenuAnimation : MonoBehaviour
{
    public GameObject page1, page2, nextButton, playButton;
    private Animator page1Animator, page2Animator, nextButtonAnimator, playButtonAnimator;

    private int clicked;


    private void Start()
    {
        page1Animator = page1.GetComponent<Animator>();
        page2Animator = page2.GetComponent<Animator>();
        nextButtonAnimator = nextButton.GetComponent<Animator>();
        playButtonAnimator = playButton.GetComponent<Animator>();

        page1.SetActive(true);
        nextButton.SetActive(true);
    }

    //For Animation
    public void PlayPage1Animation()
    {
        page1Animator.SetTrigger("PlayAnimation");
    }

    public void PlayPage2Animation()
    {
        page2Animator.SetTrigger("PlayAnimation");
    }

    public void PlayNextButtonAnimation()
    {
        nextButtonAnimator.SetTrigger("PlayAnimation");
    }

    public void PlayPlayButtonAnimation()
    {
        playButtonAnimator.SetTrigger("PlayAnimation");
    }

    //For Button
    public void ClickNextButton()
    {
        if (clicked >= 1)
        {
            nextButtonAnimator.Play("NextButtonOut");
            playButton.SetActive(true);
        }
        else
        {
            page1Animator.Play("FadeOut");
            page2.SetActive(true);
            clicked++;
        }
    }


}
