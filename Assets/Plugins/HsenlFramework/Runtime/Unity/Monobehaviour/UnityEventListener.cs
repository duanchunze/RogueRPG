using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnityEventListener : EventTrigger {
    protected const float CLICK_INTERVAL_TIME = 0.2f; //const click interval time
    protected const float CLICK_INTERVAL_POS = 2; //const click interval pos

    [HideInInspector]
    public float longPressTime = 0.5f;

    [HideInInspector]
    public float doubleIntervalTimer = 0.25f;

    [HideInInspector]
    public int doubleClickNeedCount = 2;

    public static bool sIsEvent = false; //can use in 3d camera to point out which event should be use
    public static bool sDisableEvent = false; //can use in toturial when u wanna allow user only can click this button
    public static bool sDisableEvent2 = false; //can use in toturial when u wanna allow user only can click this button
    public bool mIgnoreDisable = false;
    protected bool mAnyMove = false;

    private bool _isStay;

    public bool IsStay {
        get {
            if (this.gameObject.activeSelf == false) {
                this._isStay = false;
                return false;
            }

            return this._isStay;
        }
        set { this._isStay = value; }
    } // 光标是否呆在内部

    private bool _isDragging;

    public bool IsDragging {
        get {
            if (this.gameObject.activeSelf == false)
                return false;
            return this._isDragging;
        }
        set { this._isDragging = value; }
    }

    private readonly Dictionary<string, object> _arg = new();

    public Action<BaseEventData> onDeselect = null;
    public Action<PointerEventData> onBeginDrag = null;
    public Action<PointerEventData> onDrag = null;
    public Action<PointerEventData> onEndDrag = null;
    public Action<PointerEventData> onDrop = null;
    public Action<AxisEventData> onMove = null;
    public Action<PointerEventData> onClick = null;
    public Action<PointerEventData> onClick2 = null; // 这个click和上面不同的是，即便点击时，鼠标进行了滑动（检测到了拖拽操作），当抬起时，依然会判定为 click
    public Action<PointerEventData> onDown = null;
    public Action<PointerEventData> onEnter = null;
    public Action<PointerEventData> onExit = null;
    public Action<PointerEventData> onUp = null;
    public Action<PointerEventData> onScroll = null;
    public Action<BaseEventData> onSelect = null;
    public Action<BaseEventData> onUpdateSelect = null;
    public Action<BaseEventData> onCancel = null;
    public Action<PointerEventData> onInitializePotentialDrag = null;
    public Action<BaseEventData> onSubmit = null;
    public Action<GameObject> onLongPress = null;
    public Action<GameObject> onDoubleClick = null;

    protected float onDowntime; //on down time
    protected Vector2 vecOnDownPos; //on down pos

    protected float longPressTimer = 0;
    protected float doubleClickTimer = 0;
    protected float doubleClickCount = 0;

    //set arg
    public void SetData(string key, object val) {
        this._arg[key] = val;
    }

    public void RemoveData(string key) {
        if (this._arg.ContainsKey(key)) this._arg.Remove(key);
    }

    // get arg
    public D GetData<D>(string key) {
        if (this._arg.ContainsKey(key)) {
            return (D)this._arg[key];
        }

        return default(D);
    }

    public static UnityEventListener Get(Transform go) {
        return Get(go.gameObject);
    }

    public static UnityEventListener Get(Component go) {
        return Get(go.gameObject);
    }

    public static UnityEventListener Get(GameObject go) {
        var listener = go.GetComponent<UnityEventListener>();
        if (listener == null)
            listener = go.AddComponent<UnityEventListener>();
        return listener;
    }

    public static U Get<U>(Transform go) where U : UnityEventListener {
        return Get<U>(go.gameObject);
    }

    public static U Get<U>(Component go) where U : UnityEventListener {
        return Get<U>(go.gameObject);
    }

    public static U Get<U>(GameObject go) where U : UnityEventListener {
        var listener = go.GetComponent<U>();
        if (listener == null)
            listener = go.AddComponent<U>();
        return listener;
    }

    public override void OnDeselect(BaseEventData eventData) {
        if (sDisableEvent2)
            return;
        if (sDisableEvent)
            return;

        sIsEvent = true;
        this.onDeselect?.Invoke(eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData) {
        this.mAnyMove = true;

        if (sDisableEvent2 && !this.mIgnoreDisable)
            return;
        if (sDisableEvent && !this.mIgnoreDisable)
            return;

        sIsEvent = true;
        this._isDragging = true;
        this.onBeginDrag?.Invoke(eventData);
    }

    public override void OnDrag(PointerEventData eventData) {
        //Debug.Log ( "---" + sDisableEvent2 + "---" + mIgnoreDisable + "----" + sDisableEvent );

        if (sDisableEvent2 && !this.mIgnoreDisable)
            return;
        if (sDisableEvent && !this.mIgnoreDisable)
            return;

        sIsEvent = true;
        this._isDragging = true;
        this.onDrag?.Invoke(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData) {
        this.mAnyMove = false;

        if (sDisableEvent2 && !this.mIgnoreDisable)
            return;
        if (sDisableEvent && !this.mIgnoreDisable)
            return;

        sIsEvent = true;
        this._isDragging = false;
        this.onEndDrag?.Invoke(eventData);
    }

    public override void OnDrop(PointerEventData eventData) {
        if (sDisableEvent2)
            return;
        if (sDisableEvent)
            return;

        sIsEvent = true;
        this.onDrop?.Invoke(eventData);
    }

    public override void OnMove(AxisEventData eventData) {
        if (sDisableEvent2)
            return;
        if (sDisableEvent)
            return;

        sIsEvent = true;
        this.onMove?.Invoke(eventData);
    }

    public override void OnPointerClick(PointerEventData eventData) {
        if (sDisableEvent && !this.mIgnoreDisable)
            return;

        sIsEvent = true;
        this.onClick2?.Invoke(eventData);

        if (this.mAnyMove)
            return;

        sIsEvent = true;
        this.onClick?.Invoke(eventData);

        if (this.onDoubleClick != null) {
            this.doubleClickCount++;
            if (this.doubleClickCount >= this.doubleClickNeedCount) {
                this.doubleClickCount = 0;
                this.onDoubleClick?.Invoke(this.gameObject);
            }
            else {
                this.StopCoroutine(this.IE_RunDoubleClickTimer());
                this.StartCoroutine(this.IE_RunDoubleClickTimer());
            }
        }
    }

    public override void OnPointerDown(PointerEventData eventData) {
        if (sDisableEvent2)
            return;
        if (sDisableEvent)
            return;

        this.onDowntime = Time.realtimeSinceStartup;
        this.vecOnDownPos = eventData.position;

        sIsEvent = true;
        this.onDown?.Invoke(eventData);

        if (this.onLongPress != null) this.StartCoroutine(this.IE_RunLongPressTimer());
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        if (sDisableEvent2)
            return;
        if (sDisableEvent)
            return;

        sIsEvent = true;
        this.IsStay = true;
        this.onEnter?.Invoke(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData) {
        if (sDisableEvent2)
            return;
        if (sDisableEvent)
            return;

        sIsEvent = true;
        this.IsStay = false;
        this.onExit?.Invoke(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData) {
        if (sDisableEvent2)
            return;
        if (sDisableEvent)
            return;

        sIsEvent = true;
        this.onUp?.Invoke(eventData);

        if (this.onLongPress != null) this.StopCoroutine(this.IE_RunLongPressTimer());
    }

    public override void OnScroll(PointerEventData eventData) {
        if (sDisableEvent2)
            return;
        if (sDisableEvent)
            return;

        sIsEvent = true;
        this.onScroll?.Invoke(eventData);
    }

    public override void OnSelect(BaseEventData eventData) {
        if (sDisableEvent2)
            return;
        if (sDisableEvent)
            return;

        sIsEvent = true;
        this.onSelect?.Invoke(eventData);
    }

    public override void OnUpdateSelected(BaseEventData eventData) {
        if (sDisableEvent2)
            return;
        if (sDisableEvent)
            return;

        sIsEvent = true;
        this.onUpdateSelect?.Invoke(eventData);
    }

    public override void OnCancel(BaseEventData eventData) {
        if (sDisableEvent2)
            return;
        if (sDisableEvent)
            return;

        sIsEvent = true;
        this.onCancel?.Invoke(eventData);
    }

    public override void OnInitializePotentialDrag(PointerEventData eventData) {
        if (sDisableEvent2)
            return;
        if (sDisableEvent)
            return;

        sIsEvent = true;
        this.onInitializePotentialDrag?.Invoke(eventData);
    }

    public override void OnSubmit(BaseEventData eventData) {
        if (sDisableEvent2)
            return;
        if (sDisableEvent)
            return;

        sIsEvent = true;
        this.onSubmit?.Invoke(eventData);
    }


    IEnumerator IE_RunLongPressTimer() {
        this.longPressTimer = 0;

        while (this.longPressTimer < this.longPressTime) {
            this.longPressTimer += Time.deltaTime;
            yield return null;
        }

        this.onLongPress?.Invoke(this.gameObject);
    }

    IEnumerator IE_RunDoubleClickTimer() {
        this.doubleClickTimer = 0;
        while (this.doubleClickTimer < this.doubleIntervalTimer) {
            this.doubleClickTimer += Time.deltaTime;
            yield return null;
        }

        this.doubleClickCount = 0;
    }
}