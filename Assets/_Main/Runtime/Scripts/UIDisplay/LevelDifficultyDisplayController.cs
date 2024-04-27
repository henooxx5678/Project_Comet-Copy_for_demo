using System;
using UnityEngine;
using UnityEngine.UI;
using ProjectComet.Levels;

namespace ProjectComet.UIDisplay {
    
    public class LevelDifficultyDisplayController : MonoBehaviour, ILevelDifficultyDisplayController {

        [SerializeField] BehavingType _behave;
        [SerializeField] Color _selectedColor = Color.white;
        [SerializeField] Color _unselectedColor = Color.white;

        [SerializeField] LevelDifficultyDisplayComponentInfo[] _levelDifficultyDisplayComponentInfos;


        public void SetSelected (LevelDifficulty difficulty) {
            if (_levelDifficultyDisplayComponentInfos != null) {
                foreach (var info in _levelDifficultyDisplayComponentInfos) {

                    if (info != null) {

                        switch (_behave) {
                            case BehavingType.SwitchOnOff: {
                                
                                info.SetColorOfDisplayComponent(Color.white);
                                info.SetActiveOfDisplayObject(info.Difficulty == difficulty);
                                break;
                            }

                            case BehavingType.ChangeColor: {

                                info.SetActiveOfDisplayObject(true);

                                if (info.Difficulty == difficulty) {
                                    info.SetColorOfDisplayComponent(_selectedColor);
                                }
                                else {
                                    info.SetColorOfDisplayComponent(_unselectedColor);
                                }
                                break;
                            }
                        }
                    }

                }
            }
        }


        [Serializable]
        protected class LevelDifficultyDisplayComponentInfo {
            [SerializeField] LevelDifficulty _difficulty;
            public LevelDifficulty Difficulty => _difficulty;

            [SerializeField] Component _displayComponent;
            public Component DisplayComponent => _displayComponent;


            public void SetActiveOfDisplayObject (bool active) {
                if (DisplayComponent) {
                    DisplayComponent.gameObject.SetActive(active);
                }
            }

            public void SetColorOfDisplayComponent (Color color) {
                if (DisplayComponent) {
                    if (DisplayComponent is Image) {
                        Image displayImage = (Image) DisplayComponent;
                        displayImage.color = color;
                    }
                    else if (DisplayComponent is SpriteRenderer) {
                        SpriteRenderer displaySpriteRenderer = (SpriteRenderer) DisplayComponent;
                        displaySpriteRenderer.color = color;
                    }
                }
            }
            
        }


        public enum BehavingType {
            SwitchOnOff,
            ChangeColor
        }

    }

}
