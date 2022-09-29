using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JPWF
{
    /// <summary> Plays all music/sfx with 2D spatialization, from the center of the screen </summary>
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;

        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] [Min(0.1f)] private float _musicFadeSpeed = 1.0f;

        [Header("Sound Fx")]
        [SerializeField] private AudioClip _rotatePiece;
        [SerializeField] private AudioClip[] _trayPick;
        [SerializeField] private AudioClip[] _trayDrop;
        [SerializeField] private AudioClip[] _connectPiece;
        [SerializeField] private AudioClip[] _almostDone;
        [SerializeField] private AudioClip _buttonPress;
        [SerializeField] private AudioClip _selectBackground;
        [SerializeField] private AudioClip _startPuzzle;

        [Header("Background Music")]
        [SerializeField] private AudioClip _menuMusic;
        [SerializeField] private AudioClip[] _puzzleMusic;

        private Coroutine _musicTransition;

        public static bool SfxMuted => _instance._sfxSource.mute;
        public static bool MusicMuted => _instance._musicSource.mute;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                PlayMenuMusic();
            }
            else 
            {
                Destroy(gameObject);
            }
        }

        /// <summary> Plays one of the given <paramref name="clips"/> selected at random. Does not loop. </summary>
        public static void PlaySfx(params AudioClip[] clips)
        {
            _instance._sfxSource.clip = clips[Random.Range(0, clips.Length)];
            _instance._sfxSource.loop = false;
            _instance._sfxSource.Play();
        }

        public static void PlayRotatePieceSfx()
        {
            PlaySfx(_instance._rotatePiece);
        }

        public static void PlayTrayPickSfx()
        {
            PlaySfx(_instance._trayPick);
        }

        public static void PlayTrayDropSfx()
        {
            PlaySfx(_instance._trayPick);
        }

        public static void PlayConnectPieceSfx()
        {
            PlaySfx(_instance._connectPiece);
        }

        /// <summary> 
        /// Plays ascending pitch clips indicating 2-1-0 pieces remaining. 
        /// Call before decrementing the piece count so the correct clip plays as the last piece is snapped. 
        /// </summary>
        public static void PlayAlmostDoneSfx(int piecesRemaining)
        {
            if (piecesRemaining <= 3 && piecesRemaining > 0)
            {
                PlaySfx(_instance._almostDone[piecesRemaining - 1]);
            }
        }

        public static void PlayButtonPressSfx()
        {
            PlaySfx(_instance._buttonPress);
        }

        public static void PlaySelectBackgroundSfx()
        {
            PlaySfx(_instance._selectBackground);
        }

        public static void PlayStartPuzzleSfx()
        {
            PlaySfx(_instance._startPuzzle);
        }

        /// <summary> Plays main menu music on a loop. </summary>
        public static void PlayMenuMusic()
        {
            if (_instance._musicTransition != null)
            {
                _instance.StopCoroutine(_instance._musicTransition);
            }

            _instance._musicTransition = _instance.StartCoroutine(_instance.TransitionMusic(_instance._menuMusic));
        }

        /// <summary> Plays a random music on a loop. </summary>
        public static void PlayPuzzleMusic()
        {
            if (_instance._musicTransition != null)
            {
                _instance.StopCoroutine(_instance._musicTransition);
            }

            AudioClip clip = _instance._puzzleMusic[Random.Range(0, _instance._puzzleMusic.Length)];
            _instance._musicTransition = _instance.StartCoroutine(_instance.TransitionMusic(clip));
        }

        private IEnumerator TransitionMusic(AudioClip clip)
        {
            while (_musicSource.volume > 0.0f)
            {
                _musicSource.volume = Mathf.MoveTowards(_musicSource.volume, 0.0f, 2.0f * _musicFadeSpeed * (float)AudioSettings.dspTime);
                yield return null;
            }

            _musicSource.clip = clip;
            _musicSource.loop = true;
            _musicSource.Play();

            while (_musicSource.volume < 1.0f)
            {
                _musicSource.volume = Mathf.MoveTowards(_musicSource.volume, 1.0f, 2.0f * _musicFadeSpeed * (float)AudioSettings.dspTime);
                yield return null;
            }

            _musicTransition = null;
        }

        public static void ToggleSfx()
        {
            _instance._sfxSource.mute = !_instance._sfxSource.mute;
        }

        public static void ToggleMusic()
        {
            _instance._musicSource.mute = !_instance._musicSource.mute;
        }
    }
}