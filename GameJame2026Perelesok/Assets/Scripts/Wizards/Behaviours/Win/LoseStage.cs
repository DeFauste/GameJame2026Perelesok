using UnityEditor.SceneManagement;
using UnityEngine;
using Wizards.Animations;

namespace Wizards.Behaviours.Win
{
    public class LoseStage : WizardStage
    {
        public GameObject _symbolSystem;
        public override void StartStage()
        {
            Debug.Log("Победа! Стадия Win началась.");
            base.StartStage();
            _wizardStateController.ChangeCompressedDirection(CompressedDirection.Expansion);
            MusicService.Instance.StopAll();
            MusicService.Instance.Play("Sound_EnemyLaugh_Defeat");
            _symbolSystem.SetActive(false);
            WizardAnimationService.Instance.LoseStage();
            rightHand.SetActive(false);
            leftHand.SetActive(false);
        }
    }
}