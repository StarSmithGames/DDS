using System.Collections.Generic;

using UnityEditor;

namespace Game.Editor
{
    public class EditorWindowTaskable : EditorWindow
    {
        public List<EditorTask> tasks = new List<EditorTask>();

        public bool IsInProgress => tasks.Count > 0;

        private int maxTasks = 1;

        public void Update()
        {
            if (IsInProgress)
            {
                float progress = (1 - tasks.Count / ((float)maxTasks + 1f));
                EditorUtility.DisplayProgressBar("Progress", (progress * 100) + "%", progress);
                if (!tasks[0].isStarted)
                {
                    tasks[0].Start();
                }
                if (tasks[0].IsDone())
                {
                    tasks[0].Stop();
                    tasks.RemoveAt(0);
                    Repaint();
                }
            }
            else
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public void PushTask(EditorTask task)
        {
            tasks.Add(task);
            maxTasks = tasks.Count;
        }
    }
    public class EditorTask
    {
        public bool isStarted = false;

        public virtual void Start()
        {
            isStarted = true;
        }

        public virtual bool IsDone()
        {
            return true;
        }

        public virtual void Stop() { }
    }
    public class EditorTaskFunction : EditorTask
    {
        public delegate void StartCallback();

        public delegate bool IsDoneCallback();

        public delegate void StopCallback();

        private StartCallback startCallback;
        private StopCallback stopCallback;
        private IsDoneCallback isDoneCallback;

        public EditorTaskFunction(StartCallback startCallback = null, StopCallback stopCallback = null, IsDoneCallback isDoneCallback = null)
        {
            this.startCallback = startCallback;
            this.stopCallback = stopCallback;
            this.isDoneCallback = isDoneCallback;
        }

        public override void Start()
        {
            base.Start();
            if (startCallback != null)
            {
                startCallback();
            }
        }
        public override void Stop()
        {
            base.Stop();
            if (stopCallback != null)
            {
                stopCallback();
            }
        }
        public override bool IsDone()
        {
            if (isDoneCallback != null)
            {
                return isDoneCallback();
            }
            else
            {
                return true;
            }
        }
    }
}