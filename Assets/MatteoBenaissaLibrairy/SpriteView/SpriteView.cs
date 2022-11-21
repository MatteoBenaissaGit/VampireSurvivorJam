using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteView : MonoBehaviour
{
    
    #region Serialize Fields

    [Header("States"), SerializeField] 
    private string _defaultStateName;

    [SerializeField] 
    private List<State> _statesList;

    [Header("Action"), SerializeField] 
    private List<State> _actionsList;

    #endregion

    #region Private values

    //sprite
    private SpriteRenderer _spriteRenderer;
    private float _changeCountdown;
    private int _currentSprite;
    //state
    private State _state;
    private int _currentState;
    //actions
    private bool _isPlayingAction;
    //dictionary
    private Dictionary<string, State> _stateDictionary = new();
    private Dictionary<string, State> _actionDictionary = new();

    #endregion
    
    
    private void Start()
    {
        //state
        _state = _statesList.First(x => x.Name == _defaultStateName);
        _currentState = _statesList.IndexOf(_state);

        //sprite
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _changeCountdown = _state.TimeBetweenFrames;
        ResetAnimation();
        
        //dictionary
        foreach (State state in _statesList)
        {
            _stateDictionary.Add(state.Name, state);
        }
        foreach (State action in _actionsList)
        {
            _actionDictionary.Add(action.Name, action);
        }
    }

    private void Update()
    {
        Animate();
    }

    #region Animation methods

    private void Animate()
    {
        _changeCountdown -= Time.deltaTime;

        if (_changeCountdown <= 0)
        {
            _changeCountdown = _state.TimeBetweenFrames;

            _currentSprite++;
            if (_currentSprite >= _state.SpriteSheet.Count)
            {
                _currentSprite = 0;
                
                //if action -> reset to current state
                if (_isPlayingAction)
                {
                    _isPlayingAction = false;
                    _state = _statesList[_currentState];
                    _spriteRenderer.sprite = _state.SpriteSheet[_currentSprite];
                }
            } 
            _spriteRenderer.sprite = _state.SpriteSheet[_currentSprite];
        }
    }
    
    private void ResetAnimation()
    {
        _spriteRenderer.sprite = _state.SpriteSheet[0];
        _currentSprite = 0;
    }

    #endregion

    #region Play State/Action public methods

    public void PlayState(string state)
    {
        
        _state = _stateDictionary[state];
        ResetAnimation();
    }

    public void PlayAction(string state)
    {
        
        _isPlayingAction = true;
        _state = _actionDictionary[state];
        ResetAnimation();
    }

    #endregion
    
    
}
