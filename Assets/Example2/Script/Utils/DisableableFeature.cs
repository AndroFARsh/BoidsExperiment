namespace Example2
{
    public abstract class DisableableFeature : Feature
    {
        public override void Initialize()
        {
            if (isEnambled())
                base.Initialize();
        }

        public override void Execute()
        {
            if (isEnambled())
                base.Execute();
        }

        public override void Cleanup()
        {
            if (isEnambled())
                base.Cleanup();
        }

        public override void TearDown()
        {
            if (isEnambled())
                base.TearDown();
        }

        protected abstract bool isEnambled();
    }
}