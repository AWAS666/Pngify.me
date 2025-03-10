using PngifyMe.Services.Settings;

namespace PngifyMe.Layers
{
    public abstract class BaseLayer
    {
        public bool Unique = false;
        public float EnterTime = 3.0f;
        public float ExitTime = 3.0f;
        public float AutoRemoveTime = float.PositiveInfinity;

        public bool Entered = false;
        public bool IsExiting = false;

        protected float CurrentTime = 0;
        protected float CurrentExitingTime = 0;
        protected float GlobalTime = 0;

        public Layersetting AddedBy;


        public virtual void SetGlobalTime(float time)
        {
            GlobalTime = time;
        }

        public virtual bool Update(float dt)
        {
            CurrentTime += dt;
            if (!IsExiting)
            {
                if (AutoRemoveTime <= CurrentTime)
                {
                    IsExiting = true;
                }
                else if (CurrentTime <= EnterTime)
                {
                    OnUpdateEnter(dt, CurrentTime / EnterTime);
                }
                else
                {
                    OnUpdate(dt, CurrentTime - EnterTime);
                }
            }
            else
            {
                CurrentExitingTime += dt;
                if (CurrentExitingTime <= ExitTime)
                {
                    OnUpdateExit(dt, CurrentExitingTime / ExitTime);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void OnEnter()
        {
            Entered = true;
        }
        public abstract void OnUpdateEnter(float dt, float fraction);
        public abstract void OnUpdate(float dt, float time);
        public abstract void OnUpdateExit(float dt, float fraction);
        public abstract void OnExit();
        public abstract void OnCalculateParameters(float dt, ref LayerValues values);

        public BaseLayer Clone(Services.Settings.Layersetting adder)
        {
            AddedBy = adder;
            return (BaseLayer)this.MemberwiseClone();
        }
    }
}
