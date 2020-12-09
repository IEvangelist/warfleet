using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class RangeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RangeEnumerator GetEnumerator(this Range range) => new(range);

    public class RangeEnumerator
    {
        readonly Range _sourceRange;
        readonly bool _isAscending;

        int _index = 0;

        public int Current { get; private set; }

        public RangeEnumerator(Range range) =>
            (_sourceRange, _isAscending) = (range, range.Start.Value < range.End.Value);

        public bool MoveNext()
        {
            if (_sourceRange.End.Value == Current)
            {
                return false;
            }

            if (_index == 0)
            {
                Current = _sourceRange.Start.Value;
            }

            Current = _isAscending ? _sourceRange.Start.Value + _index++ : _sourceRange.Start.Value + _index--;

            return true;
        }

        public void Reset() => _index = 0;
    }
}