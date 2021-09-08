using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Orcas.Lua.Core
{
    
    //
    // 摘要:
    //     ///
    //     Receives events from the EventSystem and calls registered functions for each
    //     event.
    //     ///
    [AddComponentMenu("Event/Custom Event Trigger")]
    public class CustomEventTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IInitializePotentialDragHandler, IDropHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler, IEventSystemHandler
    {
        //
        // 摘要:
        //     ///
        //     All the functions registered in this EventTrigger (deprecated).
        //     ///
        [Obsolete("Please use triggers instead (UnityUpgradable) -> triggers", true)]
        public List<Entry> delegates;

        protected CustomEventTrigger() { }

        //
        // 摘要:
        //     ///
        //     All the functions registered in this EventTrigger.
        //     ///
        public List<Entry> triggers { get; set; }

        //
        // 摘要:
        //     ///
        //     Called by the EventSystem when a Cancel event occurs.
        //     ///
        //
        // 参数:
        //   eventData:
        //     Current event data.
        public virtual void OnCancel(BaseEventData eventData) { }
        //
        // 摘要:
        //     ///
        //     Called by the EventSystem when a new object is being selected.
        //     ///
        //
        // 参数:
        //   eventData:
        //     Current event data.
        public virtual void OnDeselect(BaseEventData eventData) { }
        //
        // 摘要:
        //     ///
        //     Called by the EventSystem when an object accepts a drop.
        //     ///
        //
        // 参数:
        //   eventData:
        //     Current event data.
        public virtual void OnDrop(PointerEventData eventData) { }
        //
        // 摘要:
        //     ///
        //     Called by the EventSystem when a drag has been found, but before it is valid
        //     to begin the drag.
        //     ///
        //
        // 参数:
        //   eventData:
        //     Current event data.
        public virtual void OnInitializePotentialDrag(PointerEventData eventData) { }
        //
        // 摘要:
        //     ///
        //     Called by the EventSystem when a Move event occurs.
        //     ///
        //
        // 参数:
        //   eventData:
        //     Current event data.
        public virtual void OnMove(AxisEventData eventData) { }
        //
        // 摘要:
        //     ///
        //     Called by the EventSystem when a Click event occurs.
        //     ///
        //
        // 参数:
        //   eventData:
        //     Current event data.
        public virtual void OnPointerClick(PointerEventData eventData) { }
        //
        // 摘要:
        //     ///
        //     Called by the EventSystem when a PointerDown event occurs.
        //     ///
        //
        // 参数:
        //   eventData:
        //     Current event data.
        public virtual void OnPointerDown(PointerEventData eventData) { }
        //
        // 摘要:
        //     ///
        //     Called by the EventSystem when the pointer enters the object associated with
        //     this EventTrigger.
        //     ///
        //
        // 参数:
        //   eventData:
        //     Current event data.
        public virtual void OnPointerEnter(PointerEventData eventData) { }
        //
        // 摘要:
        //     ///
        //     Called by the EventSystem when the pointer exits the object associated with this
        //     EventTrigger.
        //     ///
        //
        // 参数:
        //   eventData:
        //     Current event data.
        public virtual void OnPointerExit(PointerEventData eventData) { }
        //
        // 摘要:
        //     ///
        //     Called by the EventSystem when a PointerUp event occurs.
        //     ///
        //
        // 参数:
        //   eventData:
        //     Current event data.
        public virtual void OnPointerUp(PointerEventData eventData) { }
        //
        // 摘要:
        //     ///
        //     Called by the EventSystem when a Scroll event occurs.
        //     ///
        //
        // 参数:
        //   eventData:
        //     Current event data.
        public virtual void OnScroll(PointerEventData eventData) { }
        //
        // 摘要:
        //     ///
        //     Called by the EventSystem when a Select event occurs.
        //     ///
        //
        // 参数:
        //   eventData:
        //     Current event data.
        public virtual void OnSelect(BaseEventData eventData) { }
        //
        // 摘要:
        //     ///
        //     Called by the EventSystem when a Submit event occurs.
        //     ///
        //
        // 参数:
        //   eventData:
        //     Current event data.
        public virtual void OnSubmit(BaseEventData eventData) { }
        //
        // 摘要:
        //     ///
        //     Called by the EventSystem when the object associated with this EventTrigger is
        //     updated.
        //     ///
        //
        // 参数:
        //   eventData:
        //     Current event data.
        public virtual void OnUpdateSelected(BaseEventData eventData) { }

        //
        // 摘要:
        //     ///
        //     UnityEvent class for Triggers.
        //     ///
        public class TriggerEvent : UnityEvent<BaseEventData>
        {
            public TriggerEvent() { }
        }
        //
        // 摘要:
        //     ///
        //     An Entry in the EventSystem delegates list.
        //     ///
        public class Entry
        {
            //
            // 摘要:
            //     ///
            //     What type of event is the associated callback listening for.
            //     ///
            public EventTriggerType eventID;
            //
            // 摘要:
            //     ///
            //     The desired TriggerEvent to be Invoked.
            //     ///
            public TriggerEvent callback;

            public Entry() { }
        }
    }
}
