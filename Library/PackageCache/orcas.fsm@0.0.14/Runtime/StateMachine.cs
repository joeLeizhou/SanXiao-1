using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Fsm{
	public class StateMachine
	{
		private float currentTime = 0;
		protected StateBase currentState = null;
		protected bool paused = false;
		protected Stack<StateBase> breakedStates = null;
		protected Stack<float> lastStartTime = null;
		protected Stack<float> lastBreakTime = null;
		public Type currentStateType = null;
		private float lastStateStartTime = 0;
		private float pauseTime = 0;
		public float deltaTime = -1f;

		/// <summary>
		/// 由于ToLua框架整理，LuaState在框架里没有单例，此处靠传参传进来
		/// </summary>
		/// <param name="state"></param>
		public StateMachine(float dt = -1f)
		{
			breakedStates = new Stack<StateBase>();
			lastStartTime = new Stack<float>();
			lastBreakTime = new Stack<float>();
			deltaTime = dt;
		}
		
		
		public void Update ()
		{
			float dt = deltaTime > 0 ? deltaTime : Time.deltaTime;
			currentTime += dt;
			if (paused == true) {
				return;
			}
			if (currentState != null) {

				currentState.StateUpdate (currentTime - lastStateStartTime);
			}
		}

		public void FixedUpdate(){
			currentState.StateFixedUpdate(currentTime - lastStateStartTime);
		}
		

		public StateBase GetCurrentState(){
			return currentState;
		}


		public void ChangeState<T>(params object[] objects) where T : StateBase
		{
			ChangeState(typeof(T), objects);
		}
		
		public void ChangeState(Type nextState, params object[] objs)
		{
			if (currentState != null && nextState == currentStateType)
			{
				return;
			}
			
			if (StateFactory.Instance.ContainsState(nextState) == false)
			{
				return;
			}
			
			if (currentState != null) {
				currentState.StateEnd (currentTime - lastStateStartTime);
			}
			
			lastStateStartTime = currentTime;
			currentState = StateFactory.Instance.CreateState(nextState);
			currentStateType = nextState;

			if (currentState != null) {
				currentState.StateEnter (objs);
			}
		}

		public void ForceChangeState<T>(params object[] objects) where T : StateBase
		{
			ForceChangeState(typeof(T), objects);
		}

		public void ForceChangeState(Type nextState, params object[] objs){
			
			if (StateFactory.Instance.ContainsState(nextState) == false)
			{
				return;
			}
			
			if (currentState != null) {
				currentState.StateEnd (currentTime - lastStateStartTime);
			}
			lastStateStartTime = currentTime;
			currentState = StateFactory.Instance.CreateState(nextState);
			currentStateType = nextState;
			if (currentState != null) {
				currentState.StateEnter (objs);
			}
		}

		public void BreakState(Type nextState,params object[] objs){
			if (currentState != null && nextState == currentStateType)
				return;
			
			if (StateFactory.Instance.ContainsState(nextState) == false)
			{
				return;
			}
			
			breakedStates.Push(currentState);
			lastBreakTime.Push(currentTime);
			lastStartTime.Push(lastStateStartTime);
			lastStateStartTime = currentTime;
			currentState = StateFactory.Instance.CreateState(nextState);
			currentStateType = nextState;
			if (currentState != null) {
				currentState.StateEnter (objs);
			}
		}

		public void ResumeToBrokenState(params object[] objs){
			if	(breakedStates.Count == 0) {
				return;
			}
			StateBase nextState = breakedStates.Pop();
			float startTime = lastStartTime.Pop() + currentTime - lastBreakTime.Pop();
			if (currentState != null && nextState.GetType() == currentStateType)
				return;
			if (currentState != null) {
				currentState.StateEnd (currentTime - lastStateStartTime);
			}
			lastStateStartTime = startTime;
			currentState = nextState;
			currentStateType = nextState.GetType();
		}

		public void Pause(){
			if (paused == false) {
				paused = true;
				pauseTime = currentTime;
			}
		}

		public void Resume(){
			if (paused == true) {
				paused = false;
				lastStateStartTime += currentTime - pauseTime;
			}
		}

		public void Destroy(){
			currentState = null;
			currentStateType = null;
			Pause ();
			lastStateStartTime = pauseTime = 0;
		}
	}		
}