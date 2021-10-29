using UnityEngine;
using System;
public class MelounTimer : MonoBehaviour {

    public string timerName;
    public bool active { get; private set; }
	bool externalUpdate;
    public float duration { get; private set; }
	public float elapsed { get; private set; }
    Action<float> callbackDuring;
    Action callbackEnd;

    /// <summary>
    /// Sets and starts the Countdown of this timer.
    /// </summary>
    /// <param name="name">Name of countdown.</param>
    /// <param name="duration">Duration of countdown in seconds.</param>
    /// <param name="callbackDuring">Delegate during the process. Float represents duration in range of 0-1.</param>
    /// <param name="callbackEnd">Delegate to call after countdown is done.</param>
    public void SetCountdown(string name, float duration, Action<float> callbackDuring, Action callbackEnd, bool externalUpdate = false) {
        this.timerName = name;
        active = true;     
        this.duration = duration == 0 ? 0.00001f: duration;       
        this.callbackDuring = callbackDuring;
        this.callbackEnd = callbackEnd;
        this.externalUpdate = externalUpdate;
    }

    private void FixedUpdate() {
        if (active && !externalUpdate) {
            if (duration > 0) {
                elapsed += Time.fixedDeltaTime;
            }
            if (callbackDuring != null) {
                callbackDuring(elapsed / duration);
            }
            if (elapsed >= duration) {
                active = false;
                if (callbackEnd != null) {
                    callbackEnd();
                }
            }
        }
    }

    public void ExternalFixedUpdate() {
        if (active && externalUpdate) {
            if (duration > 0) {
                elapsed += Time.fixedDeltaTime;
            }
            if (callbackDuring != null) {
                callbackDuring(elapsed / duration);
            }
            if (elapsed >= duration) {
                active = false;
                if (callbackEnd != null) {
                    callbackEnd();
                }
            }
        }
    }

    public void Stop() {
        active = false;
    }

    public void Resume() {
        active = true;
    }

    public void StartAgain() {
        active = true;
        elapsed = 0;
    }

    public void SetElapsed(float elapsed) {
        this.elapsed = elapsed;
    }

    public void AddEndCallback(Action callbackEnd) {
        if(callbackEnd != null) {
            this.callbackEnd += callbackEnd;
        }
    }
    
    static public MelounTimer GetTimer(string name, GameObject obj) {
        foreach(MelounTimer mt in obj.GetComponents<MelounTimer>()) {
            if(mt.timerName == name) {
                return mt;
            }
        }
        return null;
    }

    static public void DeleteTimers(GameObject obj) {
        MelounTimer[] lastTimer = obj.GetComponents<MelounTimer>();
        for (int i = 0; i < lastTimer.Length; i++) {
            Component.Destroy(lastTimer[i]);
        }
    }

    static public void DeleteTimers(string name, GameObject obj) {
        MelounTimer[] lastTimer = obj.GetComponents<MelounTimer>();
        for (int i = 0; i < lastTimer.Length; i++) {
            if (lastTimer[i].timerName == name) {
                Component.Destroy(lastTimer[i]);
            }
        }
    }

    static public MelounTimer CreateTimer(GameObject go, string name, float duration, Action<float> callbackDuring, Action callbackEnd, bool externalUpdate = false) {
        MelounTimer timer = go.AddComponent<MelounTimer>();
        timer.SetCountdown(name, duration, callbackDuring, callbackEnd, externalUpdate);
        return timer;
    }
}
