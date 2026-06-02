using System;
using UnityEngine;
using Wizards;
using Wizards.Animations;
using Wizards.Behaviours;

namespace DefaultNamespace.Testing
{
    public class Testing : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                WizardAnimationService.Instance.Intro();
            }
            
            if (Input.GetKeyDown(KeyCode.F1))
            {
                WizardStateController.Instance.ChangeStage(StageWizard.FirstStage);
                Debug.Log(WizardStateController.Instance.ActionStage);
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                WizardStateController.Instance.ChangeStage(StageWizard.SecondStage);
                Debug.Log(WizardStateController.Instance.ActionStage);
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                WizardStateController.Instance.ChangeStage(StageWizard.ThirdStage);
                Debug.Log(WizardStateController.Instance.ActionStage);
            }
        }
    }
}