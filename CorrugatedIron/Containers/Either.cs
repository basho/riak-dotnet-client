namespace CorrugatedIron.Containers
{
    public class Either<TLeft, TRight>
    {
        public bool IsLeft { get; private set; }
        public TLeft Left { get; private set; }
        public TRight Right { get; private set; }

        public Either(TLeft left)
        {
            Left = left;
            IsLeft = true;
        }

        public Either(TRight right)
        {
            Right = right;
            IsLeft = false;
        }
    }
}
